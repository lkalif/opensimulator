﻿/*
 * Copyright (c) Intel Corporation
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Intel Corporation nor the
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
using Nini.Config;
using log4net;
using OpenSim.Framework;
using OpenSim.Region.Physics.Manager;
using OpenMetaverse;
using OpenSim.Region.Framework;
using OpenSim.Region.CoreModules.RegionSync.RegionSyncModule;

namespace OpenSim.Region.Physics.PEPlugin
{
public class PEScene : PhysicsScene
{
    private static readonly ILog m_log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private List<PECharacter> m_avatars = new List<PECharacter>();
    private List<PEPrim> m_prims = new List<PEPrim>();
    private float[] m_heightMap;

    public PEScene(string identifier)
    {
    }

    public override void Initialise(IMesher meshmerizer, IConfigSource config)
    {
    }

    public override PhysicsActor AddAvatar(string avName, Vector3 position, Vector3 size, bool isFlying)
    {
        PECharacter actor = new PECharacter(avName, this, position, null, size, 0f, 0f, .5f, 1f,
            1f, 1f, .5f, .5f);
        m_avatars.Add(actor);
        return actor;
    }

    public override void RemoveAvatar(PhysicsActor actor)
    {
        try
        {
            m_avatars.Remove((PECharacter)actor);
        }
        catch (Exception e)
        {
            m_log.WarnFormat("[RPE]: Attempt to remove avatar that is not in physics scene: {0}", e);
        }
    }

    public override void RemovePrim(PhysicsActor prim)
    {
        try
        {
            m_prims.Remove((PEPrim)prim);
        }
        catch (Exception e)
        {
            m_log.WarnFormat("[RPE]: Attempt to remove prim that is not in physics scene: {0}", e);
        }
    }

    public override PhysicsActor AddPrimShape(string primName, PrimitiveBaseShape pbs, Vector3 position,
                                              Vector3 size, Quaternion rotation)  // deprecated
    {
        return null;
    }
    public override PhysicsActor AddPrimShape(string primName, PrimitiveBaseShape pbs, Vector3 position,
                                              Vector3 size, Quaternion rotation, bool isPhysical)
    {
        PEPrim prim = new PEPrim(primName, this, position, size, rotation, null, pbs, isPhysical, null);
        m_prims.Add(prim);
        return prim;
    }

    public override void AddPhysicsActorTaint(PhysicsActor prim) { }

    public override float Simulate(float timeStep)
    {
        // if we are a physics engine server, send update information
        if (SceneToPhysEngineSyncServer.IsPhysEngineScene2S())
        {
            if (SceneToPhysEngineSyncServer.IsActivePhysEngineScene2S())
            {
                // m_log.DebugFormat("[RPE]: Simulate. p={0}, a={1}", m_prims.Count, m_avatars.Count);
                foreach (PEPrim prim in m_prims)
                {
                    if (prim.lastValues.Changed(prim))
                    {
                        SceneToPhysEngineSyncServer.RouteUpdate(prim);
                    }
                }
                foreach (PECharacter actor in m_avatars)
                {
                    // m_log.DebugFormat("[RPE]: Simulate. p={0}, a={1}", m_prims.Count, m_avatars.Count);
                    if (actor.lastValues.Changed(actor))
                    {
                        SceneToPhysEngineSyncServer.RouteUpdate(actor);
                    }
                }
            }
            return 60f;
        }
        /*
        // code borrowed from BasicPhysics to do just avatar movement
        foreach (PECharacter actor in m_avatars)
        {
            Vector3 actorPosition = actor.Position;
            Vector3 actorVelocity = actor.Velocity;

            actorPosition.X += actor.Velocity.X*timeStep;
            actorPosition.Y += actor.Velocity.Y*timeStep;

            actorPosition.Y = Math.Max(actorPosition.Y, 0.1f);
            actorPosition.Y = Math.Min(actorPosition.Y, Constants.RegionSize - 0.1f);
            actorPosition.X = Math.Max(actorPosition.X, 0.1f);
            actorPosition.X = Math.Min(actorPosition.X, Constants.RegionSize - 0.1f);

            float height = 25.0F;
            try
            {
                height = m_heightMap[(int)actor.Position.Y * Constants.RegionSize + (int)actor.Position.X] + actor.Size.Z;
            }
            catch (OverflowException)
            {
                m_log.WarnFormat("[RPE]: Actor out of range: {0}", actor.SOPName, actor.Position.ToString());
            }

            if (actor.Flying)
            {
                if (actor.Position.Z + (actor.Velocity.Z*timeStep) <
                    m_heightMap[(int)actor.Position.Y * Constants.RegionSize + (int)actor.Position.X] + 2)
                {
                    actorPosition.Z = height;
                    actorVelocity.Z = 0;
                    actor.IsColliding = true;
                }
                else
                {
                    actorPosition.Z += actor.Velocity.Z*timeStep;
                    actor.IsColliding = false;
                }
            }
            else
            {
                actorPosition.Z = height;
                actorVelocity.Z = 0;
                actor.IsColliding = true;
            }

            actor.Position = actorPosition;
            actor.Velocity = actorVelocity;
        }
         */
        return 60f; // returns frames per second
    }

    public override void GetResults() { }

    public override void SetTerrain(float[] heightMap) {
        m_heightMap = heightMap;
    }

    public override void SetWaterLevel(float baseheight) { }

    public override void DeleteTerrain() { }

    public override void Dispose() { }

    public override Dictionary<uint, float> GetTopColliders()
    {
        return new Dictionary<uint, float>();
    }

    public override bool IsThreaded { get { return false;  } }

}
}
