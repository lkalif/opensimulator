﻿/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyrightD
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.Text;

using OMV = OpenMetaverse;
using OpenSim.Framework;
using OpenSim.Region.Physics.Manager;

namespace OpenSim.Region.Physics.BulletSPlugin
{
// Class to wrap all objects.
// The rest of BulletSim doesn't need to keep checking for avatars or prims
// unless the difference is significant.
public abstract class BSPhysObject : PhysicsActor
{
    protected void BaseInitialize(BSScene parentScene)
    {
        PhysicsScene = parentScene;
        Linkset = new BSLinkset(PhysicsScene, this);

        CollisionCollection = new CollisionEventUpdate();
        SubscribedEventsMs = 0;
        CollidingStep = 0;
        CollidingGroundStep = 0;
    }

    public BSScene PhysicsScene { get; protected set; }

    public BSLinkset Linkset { get; set; }

    // Return the object mass without calculating it or having side effects
    public abstract float MassRaw { get; }

    // Reference to the physical body (btCollisionObject) of this object
    public BulletBody BSBody { get; protected set; }
    // Reference to the physical shape (btCollisionShape) of this object
    public BulletShape BSShape { get; protected set; }

    // Stop all physical motion.
    public abstract void ZeroMotion();

    // Step the vehicle simulation for this object. A NOOP if the vehicle was not configured.
    public virtual void StepVehicle(float timeStep) { }

    // Update the physical location and motion of the object. Called with data from Bullet.
    public abstract void UpdateProperties(EntityProperties entprop);

    // Tell the object to clean up.
    public abstract void Destroy();

    #region Collisions

    // Requested number of milliseconds between collision events. Zero means disabled.
    protected int SubscribedEventsMs { get; set; }
    // Given subscription, the time that a collision may be passed up
    protected int NextCollisionOkTime { get; set; }
    // The simulation step that last had a collision
    protected long CollidingStep { get; set; }
    // The simulation step that last had a collision with the ground
    protected long CollidingGroundStep { get; set; }
    // The collision flags we think are set in Bullet
    protected CollisionFlags CurrentCollisionFlags { get; set; }

    // The collisions that have been collected this tick
    protected CollisionEventUpdate CollisionCollection;

    // The simulation step is telling this object about a collision.
    // Return 'true' if a collision was processed and should be sent up.
    // Called at taint time from within the Step() function
    public virtual bool Collide(uint collidingWith, BSPhysObject collidee, 
                    OMV.Vector3 contactPoint, OMV.Vector3 contactNormal, float pentrationDepth)
    {
        bool ret = false;

        // The following lines make IsColliding() and IsCollidingGround() work
        CollidingStep = PhysicsScene.SimulationStep;
        if (collidingWith <= PhysicsScene.TerrainManager.HighestTerrainID)
        {
            CollidingGroundStep = PhysicsScene.SimulationStep;
        }

        // prims in the same linkset cannot collide with each other
        if (collidee != null && (this.Linkset.LinksetID == collidee.Linkset.LinksetID))
        {
            return ret;
        }

        PhysicsScene.PhysicsLogging.Write("{0},BSPhysObject.Collison,call,with={1}", LocalID, collidingWith);

        // if someone has subscribed for collision events....
        if (SubscribedEvents()) {
            CollisionCollection.AddCollider(collidingWith, new ContactPoint(contactPoint, contactNormal, pentrationDepth));
            PhysicsScene.PhysicsLogging.Write("{0},BSPhysObject.Collison.AddCollider,call,with={1},point={2},normal={3},depth={4},next={5}", 
                    LocalID, collidingWith, contactPoint, contactNormal, pentrationDepth, NextCollisionOkTime.ToString("yyyyMMddHHmmssfff"));
            ret = true;
        }
        return ret;
    }

    // Routine to send the collected collisions into the simulator.
    // Also handles removal of this from the collection of objects with collisions if
    //      there are no collisions from this object. Mechanism is create one last
    //      collision event to make collision_end work.
    public virtual void SendCollisions()
    {
        // throttle the collisions to the number of milliseconds specified in the subscription
        int nowTime = PhysicsScene.SimulationNowTime;
        if (nowTime >= NextCollisionOkTime)
        {
            NextCollisionOkTime = nowTime + SubscribedEventsMs;

            // We are called if we previously had collisions. If there are no collisions
            //   this time, send up one last empty event so OpenSim can sense collision end.
            if (CollisionCollection.Count == 0)
                PhysicsScene.ObjectsWithNoMoreCollisions.Add(this);

            base.SendCollisionUpdate(CollisionCollection);

            // The collisionCollection structure is passed around in the simulator.
            // Make sure we don't have a handle to that one and that a new one is used next time.
            CollisionCollection = new CollisionEventUpdate();
        }
    }

    // Subscribe for collision events.
    // Parameter is the millisecond rate the caller wishes collision events to occur.
    public override void SubscribeEvents(int ms) { 
        SubscribedEventsMs = ms;
        if (ms > 0)
        {
            // make sure first collision happens
            NextCollisionOkTime = Util.EnvironmentTickCountSubtract(SubscribedEventsMs);
            PhysicsScene.PhysicsLogging.Write("{0},SubscribeEvents,call,ms={1},nextOKTime={2}", 
                    LocalID, SubscribedEventsMs, NextCollisionOkTime.ToString("yyyyMMddHHmmssfff"));

            PhysicsScene.TaintedObject("BSPhysObject.SubscribeEvents", delegate()
            {
                CurrentCollisionFlags = BulletSimAPI.AddToCollisionFlags2(BSBody.Ptr, CollisionFlags.BS_SUBSCRIBE_COLLISION_EVENTS);
            });
        }
        else
        {
            // Subscribing for zero or less is the same as unsubscribing
            UnSubscribeEvents();
        }
    }
    public override void UnSubscribeEvents() { 
        SubscribedEventsMs = 0;
        PhysicsScene.PhysicsLogging.Write("{0},UnSubscribeEvents,call", LocalID);
        PhysicsScene.TaintedObject("BSPhysObject.UnSubscribeEvents", delegate()
        {
            CurrentCollisionFlags = BulletSimAPI.RemoveFromCollisionFlags2(BSBody.Ptr, CollisionFlags.BS_SUBSCRIBE_COLLISION_EVENTS);
        });
    }
    // Return 'true' if the simulator wants collision events
    public override bool SubscribedEvents() { 
        return (SubscribedEventsMs > 0);
    }

    #endregion // Collisions
}
}