/* Copyright 2011 (c) Intel Corporation
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * The name of the copyright holder may not be used to endorse or promote products
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
using System.Net;
using System.Reflection;
using log4net;
using OpenMetaverse;
using OpenMetaverse.Packets;
using OpenMetaverse.StructuredData;
using OpenSim.Framework;
using OpenSim.Framework.Client;
using OpenSim.Region.Framework.Scenes;

namespace OpenSim.Region.CoreModules.RegionSync.RegionSyncModule
{
    public class RegionSyncAvatar : IClientAPI, IClientCore
    {
        private uint movementFlag = 0;
        private short flyState = 0;
        private Quaternion bodyDirection = Quaternion.Identity;
        private short count = 0;
        private short frame = 0;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

// disable warning: public events, part of the public API
#pragma warning disable 67

        public event Action<IClientAPI> OnLogout;
        public event ObjectPermissions OnObjectPermissions;

        public event MoneyTransferRequest OnMoneyTransferRequest;
        public event ParcelBuy OnParcelBuy;
        public event Action<IClientAPI> OnConnectionClosed;

        public event ImprovedInstantMessage OnInstantMessage;
        public event ChatMessage OnChatFromClient;
        public event ChatMessageRaw OnChatFromClientRaw;
        public event TextureRequest OnRequestTexture;
        public event RezObject OnRezObject;
        public event ModifyTerrain OnModifyTerrain;
        public event BakeTerrain OnBakeTerrain;
        public event SetAppearance OnSetAppearance;
        public event AvatarNowWearing OnAvatarNowWearing;
        public event RezSingleAttachmentFromInv OnRezSingleAttachmentFromInv;
        public event RezMultipleAttachmentsFromInv OnRezMultipleAttachmentsFromInv;
        public event UUIDNameRequest OnDetachAttachmentIntoInv;
        public event ObjectAttach OnObjectAttach;
        public event ObjectDeselect OnObjectDetach;
        public event ObjectDrop OnObjectDrop;
        public event StartAnim OnStartAnim;
        public event StopAnim OnStopAnim;
        public event LinkObjects OnLinkObjects;
        public event DelinkObjects OnDelinkObjects;
        public event RequestMapBlocks OnRequestMapBlocks;
        public event RequestMapName OnMapNameRequest;
        public event TeleportLocationRequest OnTeleportLocationRequest;
        public event TeleportLandmarkRequest OnTeleportLandmarkRequest;
        public event DisconnectUser OnDisconnectUser;
        public event RequestAvatarProperties OnRequestAvatarProperties;
        public event SetAlwaysRun OnSetAlwaysRun;

        public event DeRezObject OnDeRezObject;
        public event Action<IClientAPI> OnRegionHandShakeReply;
        public event GenericCall1 OnRequestWearables;
        public event GenericCall1 OnCompleteMovementToRegion;
        public event UpdateAgent OnPreAgentUpdate;
        public event UpdateAgent OnAgentUpdate;
        public event UpdateAgentRaw OnAgentUpdateRaw;
        public event AgentRequestSit OnAgentRequestSit;
        public event AgentSit OnAgentSit;
        public event AvatarPickerRequest OnAvatarPickerRequest;
        public event Action<IClientAPI> OnRequestAvatarsData;
        public event AddNewPrim OnAddPrim;
        public event RequestGodlikePowers OnRequestGodlikePowers;
        public event GodKickUser OnGodKickUser;
        public event ObjectDuplicate OnObjectDuplicate;
        public event GrabObject OnGrabObject;
        public event DeGrabObject OnDeGrabObject;
        public event MoveObject OnGrabUpdate;
        public event SpinStart OnSpinStart;
        public event SpinObject OnSpinUpdate;
        public event SpinStop OnSpinStop;
        public event ViewerEffectEventHandler OnViewerEffect;

        public event FetchInventory OnAgentDataUpdateRequest;
        public event TeleportLocationRequest OnSetStartLocationRequest;

        public event UpdateShape OnUpdatePrimShape;
        public event ObjectExtraParams OnUpdateExtraParams;
        public event RequestObjectPropertiesFamily OnRequestObjectPropertiesFamily;
        public event ObjectRequest OnObjectRequest;
        public event ObjectSelect OnObjectSelect;
        public event GenericCall7 OnObjectDescription;
        public event GenericCall7 OnObjectName;
        public event GenericCall7 OnObjectClickAction;
        public event GenericCall7 OnObjectMaterial;
        public event UpdatePrimFlags OnUpdatePrimFlags;
        public event UpdatePrimTexture OnUpdatePrimTexture;
        public event UpdateVector OnUpdatePrimGroupPosition;
        public event UpdateVector OnUpdatePrimSinglePosition;
        public event UpdatePrimRotation OnUpdatePrimGroupRotation;
        public event UpdatePrimSingleRotationPosition OnUpdatePrimSingleRotationPosition;
        public event UpdatePrimSingleRotation OnUpdatePrimSingleRotation;
        public event UpdatePrimGroupRotation OnUpdatePrimGroupMouseRotation;
        public event UpdateVector OnUpdatePrimScale;
        public event UpdateVector OnUpdatePrimGroupScale;
        public event StatusChange OnChildAgentStatus;
        public event GenericCall2 OnStopMovement;
        public event Action<UUID> OnRemoveAvatar;

        public event CreateNewInventoryItem OnCreateNewInventoryItem;
        public event LinkInventoryItem OnLinkInventoryItem;
        public event CreateInventoryFolder OnCreateNewInventoryFolder;
        public event UpdateInventoryFolder OnUpdateInventoryFolder;
        public event MoveInventoryFolder OnMoveInventoryFolder;
        public event RemoveInventoryFolder OnRemoveInventoryFolder;
        public event RemoveInventoryItem OnRemoveInventoryItem;
        public event FetchInventoryDescendents OnFetchInventoryDescendents;
        public event PurgeInventoryDescendents OnPurgeInventoryDescendents;
        public event FetchInventory OnFetchInventory;
        public event RequestTaskInventory OnRequestTaskInventory;
        public event UpdateInventoryItem OnUpdateInventoryItem;
        public event CopyInventoryItem OnCopyInventoryItem;
        public event MoveInventoryItem OnMoveInventoryItem;
        public event UDPAssetUploadRequest OnAssetUploadRequest;
        public event RequestTerrain OnRequestTerrain;
        public event RequestTerrain OnUploadTerrain;
        public event XferReceive OnXferReceive;
        public event RequestXfer OnRequestXfer;
        public event ConfirmXfer OnConfirmXfer;
        public event AbortXfer OnAbortXfer;
        public event RezScript OnRezScript;
        public event UpdateTaskInventory OnUpdateTaskInventory;
        public event MoveTaskInventory OnMoveTaskItem;
        public event RemoveTaskInventory OnRemoveTaskItem;
        public event RequestAsset OnRequestAsset;
        public event GenericMessage OnGenericMessage;
        public event UUIDNameRequest OnNameFromUUIDRequest;
        public event UUIDNameRequest OnUUIDGroupNameRequest;

        public event ParcelPropertiesRequest OnParcelPropertiesRequest;
        public event ParcelDivideRequest OnParcelDivideRequest;
        public event ParcelJoinRequest OnParcelJoinRequest;
        public event ParcelPropertiesUpdateRequest OnParcelPropertiesUpdateRequest;
        public event ParcelAbandonRequest OnParcelAbandonRequest;
        public event ParcelGodForceOwner OnParcelGodForceOwner;
        public event ParcelReclaim OnParcelReclaim;
        public event ParcelReturnObjectsRequest OnParcelReturnObjectsRequest;
        public event ParcelAccessListRequest OnParcelAccessListRequest;
        public event ParcelAccessListUpdateRequest OnParcelAccessListUpdateRequest;
        public event ParcelSelectObjects OnParcelSelectObjects;
        public event ParcelObjectOwnerRequest OnParcelObjectOwnerRequest;
        public event ParcelDeedToGroup OnParcelDeedToGroup;
        public event ObjectDeselect OnObjectDeselect;
        public event RegionInfoRequest OnRegionInfoRequest;
        public event EstateCovenantRequest OnEstateCovenantRequest;
        public event EstateChangeInfo OnEstateChangeInfo;

        public event ObjectDuplicateOnRay OnObjectDuplicateOnRay;

        public event FriendActionDelegate OnApproveFriendRequest;
        public event FriendActionDelegate OnDenyFriendRequest;
        public event FriendshipTermination OnTerminateFriendship;
        public event GrantUserFriendRights OnGrantUserRights;

        public event EconomyDataRequest OnEconomyDataRequest;
        public event MoneyBalanceRequest OnMoneyBalanceRequest;
        public event UpdateAvatarProperties OnUpdateAvatarProperties;

        public event ObjectIncludeInSearch OnObjectIncludeInSearch;
        public event UUIDNameRequest OnTeleportHomeRequest;

        public event ScriptAnswer OnScriptAnswer;
        public event RequestPayPrice OnRequestPayPrice;
        public event ObjectSaleInfo OnObjectSaleInfo;
        public event ObjectBuy OnObjectBuy;
        public event BuyObjectInventory OnBuyObjectInventory;
        public event AgentSit OnUndo;
        public event AgentSit OnRedo;
        public event LandUndo OnLandUndo;

        public event ForceReleaseControls OnForceReleaseControls;

        public event GodLandStatRequest OnLandStatRequest;
        public event RequestObjectPropertiesFamily OnObjectGroupRequest;

        public event DetailedEstateDataRequest OnDetailedEstateDataRequest;
        public event SetEstateFlagsRequest OnSetEstateFlagsRequest;
        public event SetEstateTerrainBaseTexture OnSetEstateTerrainBaseTexture;
        public event SetEstateTerrainDetailTexture OnSetEstateTerrainDetailTexture;
        public event SetEstateTerrainTextureHeights OnSetEstateTerrainTextureHeights;
        public event CommitEstateTerrainTextureRequest OnCommitEstateTerrainTextureRequest;
        public event SetRegionTerrainSettings OnSetRegionTerrainSettings;
        public event EstateRestartSimRequest OnEstateRestartSimRequest;
        public event EstateChangeCovenantRequest OnEstateChangeCovenantRequest;
        public event UpdateEstateAccessDeltaRequest OnUpdateEstateAccessDeltaRequest;
        public event SimulatorBlueBoxMessageRequest OnSimulatorBlueBoxMessageRequest;
        public event EstateBlueBoxMessageRequest OnEstateBlueBoxMessageRequest;
        public event EstateDebugRegionRequest OnEstateDebugRegionRequest;
        public event EstateTeleportOneUserHomeRequest OnEstateTeleportOneUserHomeRequest;
        public event EstateTeleportAllUsersHomeRequest OnEstateTeleportAllUsersHomeRequest;
        public event ScriptReset OnScriptReset;
        public event GetScriptRunning OnGetScriptRunning;
        public event SetScriptRunning OnSetScriptRunning;
        public event UpdateVector OnAutoPilotGo;

        public event TerrainUnacked OnUnackedTerrain;

        public event RegionHandleRequest OnRegionHandleRequest;
        public event ParcelInfoRequest OnParcelInfoRequest;

        public event ActivateGesture OnActivateGesture;
        public event DeactivateGesture OnDeactivateGesture;
        public event ObjectOwner OnObjectOwner;
 
        public event DirPlacesQuery OnDirPlacesQuery;
        public event DirFindQuery OnDirFindQuery;
        public event DirLandQuery OnDirLandQuery;
        public event DirPopularQuery OnDirPopularQuery;
        public event DirClassifiedQuery OnDirClassifiedQuery;
        public event EventInfoRequest OnEventInfoRequest;
        public event ParcelSetOtherCleanTime OnParcelSetOtherCleanTime;

        public event MapItemRequest OnMapItemRequest;

        public event OfferCallingCard OnOfferCallingCard;
        public event AcceptCallingCard OnAcceptCallingCard;
        public event DeclineCallingCard OnDeclineCallingCard;
        public event SoundTrigger OnSoundTrigger;

        public event StartLure OnStartLure;
        public event TeleportLureRequest OnTeleportLureRequest;
        public event NetworkStats OnNetworkStatsUpdate;

        public event ClassifiedInfoRequest OnClassifiedInfoRequest;
        public event ClassifiedInfoUpdate OnClassifiedInfoUpdate;
        public event ClassifiedDelete OnClassifiedDelete;
        public event ClassifiedDelete OnClassifiedGodDelete;

        public event EventNotificationAddRequest OnEventNotificationAddRequest;
        public event EventNotificationRemoveRequest OnEventNotificationRemoveRequest;
        public event EventGodDelete OnEventGodDelete;

        public event ParcelDwellRequest OnParcelDwellRequest;
        public event UserInfoRequest OnUserInfoRequest;
        public event UpdateUserInfo OnUpdateUserInfo;

        public event RetrieveInstantMessages OnRetrieveInstantMessages;

        public event PickDelete OnPickDelete;
        public event PickGodDelete OnPickGodDelete;
        public event PickInfoUpdate OnPickInfoUpdate;
        public event AvatarNotesUpdate OnAvatarNotesUpdate;

        public event MuteListRequest OnMuteListRequest;

        public event AvatarInterestUpdate OnAvatarInterestUpdate;

        public event PlacesQuery OnPlacesQuery;

        public event FindAgentUpdate OnFindAgent;
        public event TrackAgentUpdate OnTrackAgent;
        public event NewUserReport OnUserReport;
        public event SaveStateHandler OnSaveState;
        public event GroupAccountSummaryRequest OnGroupAccountSummaryRequest;
        public event GroupAccountDetailsRequest OnGroupAccountDetailsRequest;
        public event GroupAccountTransactionsRequest OnGroupAccountTransactionsRequest;
        public event FreezeUserUpdate OnParcelFreezeUser;
        public event EjectUserUpdate OnParcelEjectUser;
        public event ParcelBuyPass OnParcelBuyPass;
        public event ParcelGodMark OnParcelGodMark;
        public event GroupActiveProposalsRequest OnGroupActiveProposalsRequest;
        public event GroupVoteHistoryRequest OnGroupVoteHistoryRequest;
        public event SimWideDeletesDelegate OnSimWideDeletes;
        public event SendPostcard OnSendPostcard;
        public event MuteListEntryUpdate OnUpdateMuteListEntry;
        public event MuteListEntryRemove OnRemoveMuteListEntry;
        public event GodlikeMessage onGodlikeMessage;
        public event GodUpdateRegionInfoUpdate OnGodUpdateRegionInfoUpdate;

#pragma warning restore 67

        private Scene m_scene;
        private UUID m_agentID;
        private string m_firstName;
        private string m_lastName;
        private Vector3 m_startPos;
        private RegionSyncClientView m_clientView;

        /*
                            // Throw away duplicate or insignificant updates
                if (!m_bodyRot.ApproxEquals(m_lastRotation, ROTATION_TOLERANCE) ||
                    !Velocity.ApproxEquals(m_lastVelocity, VELOCITY_TOLERANCE) ||
                    !m_pos.ApproxEquals(m_lastPosition, POSITION_TOLERANCE))
         * */

        public RegionSyncAvatar(Scene scene, UUID agentID, string first, string last, Vector3 startPos)
        {
            m_scene = scene;
            m_agentID = agentID;
            m_firstName = first;
            m_lastName = last;
            m_startPos = startPos;
            m_clientView = null;

            m_log.DebugFormat("[REGION SYNC AVATAR] instance: uuid={0}, first={1}, last={2}, startPos={3}", agentID, first, last, startPos.ToString());

            //m_scene.EventManager.OnFrame += Update;
        }

        public RegionSyncAvatar(Scene scene, UUID agentID, string first, string last, Vector3 startPos, 
                        RegionSyncClientView view)
        {
            m_scene = scene;
            m_agentID = agentID;
            m_firstName = first;
            m_lastName = last;
            m_startPos = startPos;
            m_clientView = view;

            m_log.DebugFormat("[REGION SYNC AVATAR] instance: uuid={0}, first={1}, last={2}, startPos={3}, RSCV", agentID, first, last, startPos.ToString());

            //m_scene.EventManager.OnFrame += Update;
        }

        public bool AgentUpdate(AgentUpdateArgs arg)
        {
            UpdateAgent handlerAgentUpdate = OnAgentUpdate;
            if (handlerAgentUpdate != null)
            {
                OnAgentUpdate(this, arg);
                handlerAgentUpdate = null;
                return true;
            }
            handlerAgentUpdate = null;
            return false;
        }

        public virtual Vector3 StartPos
        {
            get { return m_startPos; }
            set { }
        }

        public virtual UUID AgentId
        {
            get { return m_agentID; }
        }

        public UUID SessionId
        {
            get { return UUID.Zero; }
        }

        public UUID SecureSessionId
        {
            get { return UUID.Zero; }
        }

        public virtual string FirstName
        {
            get { return m_firstName; }
        }

        public virtual string LastName
        {
            get { return m_lastName; }
        }

        public virtual String Name
        {
            get { return FirstName + " " + LastName; }
        }

        public bool IsActive
        {
            get { return true; }
            set { }
        }

        public UUID ActiveGroupId
        {
            get { return UUID.Zero; }
        }

        public string ActiveGroupName
        {
            get { return String.Empty; }
        }

        public ulong ActiveGroupPowers
        {
            get { return 0; }
        }

        public bool IsGroupMember(UUID groupID)
        {
            return false;
        }

        public ulong GetGroupPowers(UUID groupID)
        {
            return 0;
        }

        private int m_animationSequenceNumber = 1;
        public int NextAnimationSequenceNumber { get { return m_animationSequenceNumber++; } }

        public IScene Scene
        {
            get { return m_scene; }
        }

        public bool SendLogoutPacketWhenClosing
        {
            set { }
        }

        public uint MaxCoarseLocations
        {
            get { return 0; }
        }

        public virtual void ActivateGesture(UUID assetId, UUID gestureId)
        {
        }

        public virtual void SendWearables(AvatarWearable[] wearables, int serial)
        {
            m_log.Debug("[REGION SYNC AVATAR] SendWearables");
        }

        public virtual void SendAppearance(UUID agentID, byte[] visualParams, byte[] textureEntry)
        {
            m_log.Debug("[REGION SYNC AVATAR] SendAppearance");
        }

        public virtual void Kick(string message)
        {
        }

        public virtual void SendStartPingCheck(byte seq)
        {
        }

        public virtual void SendAvatarPickerReply(AvatarPickerReplyAgentDataArgs AgentData, List<AvatarPickerReplyDataArgs> Data)
        {
        }

        public virtual void SendAgentDataUpdate(UUID agentid, UUID activegroupid, string firstname, string lastname, ulong grouppowers, string groupname, string grouptitle)
        {

        }

        public virtual void SendKillObject(ulong regionHandle, uint localID)
        {
        }

        public virtual void SetChildAgentThrottle(byte[] throttle)
        {
        }
        public byte[] GetThrottlesPacked(float multiplier)
        {
            return new byte[0];
        }


        public virtual void SendAnimations(UUID[] animations, int[] seqs, UUID sourceAgentId, UUID[] objectIDs)
        {
            if (m_clientView != null)
            {
                // m_log.DebugFormat("[REGION SYNC AVATAR] SendAnimations for {0}", m_agentID.ToString());
                m_scene.RegionSyncServerModule.SendAnimations(m_agentID, animations, seqs, sourceAgentId, objectIDs);
            }
        }

        public virtual void SendChatMessage(string message, byte type, Vector3 fromPos, string fromName,
                                            UUID fromAgentID, byte source, byte audible)
        {
        }

        public virtual void SendChatMessage(byte[] message, byte type, Vector3 fromPos, string fromName,
                                            UUID fromAgentID, byte source, byte audible)
        {
        }

        public void SendInstantMessage(GridInstantMessage im)
        {
            
        }

        public void SendGenericMessage(string method, List<string> message)
        {

        }

        public virtual void SendLayerData(float[] map)
        {
        }

        public virtual void SendLayerData(int px, int py, float[] map)
        {
        }
        public virtual void SendLayerData(int px, int py, float[] map, bool track)
        {
        }

        public virtual void SendWindData(Vector2[] windSpeeds) { }

        public virtual void SendCloudData(float[] cloudCover) { }

        public virtual void MoveAgentIntoRegion(RegionInfo regInfo, Vector3 pos, Vector3 look)
        {
        }

        public virtual void InformClientOfNeighbour(ulong neighbourHandle, IPEndPoint neighbourExternalEndPoint)
        {
        }

        public virtual AgentCircuitData RequestClientInfo()
        {
            return new AgentCircuitData();
        }

        public virtual void CrossRegion(ulong newRegionHandle, Vector3 pos, Vector3 lookAt,
                                        IPEndPoint newRegionExternalEndPoint, string capsURL)
        {
        }

        public virtual void SendMapBlock(List<MapBlockData> mapBlocks, uint flag)
        {
        }

        public virtual void SendLocalTeleport(Vector3 position, Vector3 lookAt, uint flags)
        {
        }

        public virtual void SendRegionTeleport(ulong regionHandle, byte simAccess, IPEndPoint regionExternalEndPoint,
                                               uint locationID, uint flags, string capsURL)
        {
        }

        public virtual void SendTeleportFailed(string reason)
        {
        }

        public void SendTeleportStart(uint flags)
        {
        }

        public void SendTeleportProgress(uint flags, string message)
        {
        }

        public virtual void SendTeleportLocationStart()
        {
        }

        public virtual void SendMoneyBalance(UUID transaction, bool success, byte[] description, int balance)
        {
        }

        public virtual void SendPayPrice(UUID objectID, int[] payPrice)
        {
        }

        public virtual void SendAvatarDataImmediate(ISceneEntity avatar)
        {
        }

        public virtual void SendCoarseLocationUpdate(List<UUID> users, List<Vector3> CoarseLocations)
        {
        }

        public void SendPrimUpdate(ISceneEntity entity, PrimUpdateFlags updateFlags)
        {
            // m_log.Debug("[REGION SYNC AVATAR] SendPrimUpdate");
        }

        public virtual void AttachObject(uint localID, Quaternion rotation, byte attachPoint, UUID ownerID)
        {
            m_log.Debug("[REGION SYNC AVATAR] AttachObject");
        }

        public virtual void SendDialog(string objectname, UUID objectID, string ownerFirstName, string ownerLastName, string msg, UUID textureID, int ch, string[] buttonlabels)
        {
        }

        public virtual void ReprioritizeUpdates()
        {
        }

        public void FlushPrimUpdates()
        {
        }

        public virtual void SendInventoryFolderDetails(UUID ownerID, UUID folderID,
                                                       List<InventoryItemBase> items,
                                                       List<InventoryFolderBase> folders,
                                                       int version,
                                                       bool fetchFolders,
                                                       bool fetchItems)
        {
        }

        public virtual void SendInventoryItemDetails(UUID ownerID, InventoryItemBase item)
        {
        }

        public virtual void SendInventoryItemCreateUpdate(InventoryItemBase Item, uint callbackID)
        {
        }

        public virtual void SendRemoveInventoryItem(UUID itemID)
        {
        }

        public virtual void SendBulkUpdateInventory(InventoryNodeBase node)
        {
        }

        public UUID GetDefaultAnimation(string name)
        {
            return UUID.Zero;
        }

        public void SendTakeControls(int controls, bool passToAgent, bool TakeControls)
        {
        }

        public virtual void SendTaskInventory(UUID taskID, short serial, byte[] fileName)
        {
        }

        public virtual void SendXferPacket(ulong xferID, uint packet, byte[] data)
        {
        }
        public virtual void SendAbortXferPacket(ulong xferID)
        {
        }

        public virtual void SendEconomyData(float EnergyEfficiency, int ObjectCapacity, int ObjectCount, int PriceEnergyUnit,
            int PriceGroupCreate, int PriceObjectClaim, float PriceObjectRent, float PriceObjectScaleFactor,
            int PriceParcelClaim, float PriceParcelClaimFactor, int PriceParcelRent, int PricePublicObjectDecay,
            int PricePublicObjectDelete, int PriceRentLight, int PriceUpload, int TeleportMinPrice, float TeleportPriceExponent)
        {

        }
        public virtual void SendNameReply(UUID profileId, string firstname, string lastname)
        {
        }

        public virtual void SendPreLoadSound(UUID objectID, UUID ownerID, UUID soundID)
        {
        }

        public virtual void SendPlayAttachedSound(UUID soundID, UUID objectID, UUID ownerID, float gain,
                                                  byte flags)
        {
        }

        public void SendTriggeredSound(UUID soundID, UUID ownerID, UUID objectID, UUID parentID, ulong handle, Vector3 position, float gain)
        {
        }

        public void SendAttachedSoundGainChange(UUID objectID, float gain)
        {

        }

        public void SendAlertMessage(string message)
        {
        }

        public void SendAgentAlertMessage(string message, bool modal)
        {
        }

        public void SendSystemAlertMessage(string message)
        {
        }

        public void SendLoadURL(string objectname, UUID objectID, UUID ownerID, bool groupOwned, string message,
                                string url)
        {
        }

        public virtual void SendRegionHandshake(RegionInfo regionInfo, RegionHandshakeArgs args)
        {
            if (OnRegionHandShakeReply != null)
            {
                OnRegionHandShakeReply(this);
            }

            if (OnCompleteMovementToRegion != null)
            {
                OnCompleteMovementToRegion(this);
            }
        }
        public void SendAssetUploadCompleteMessage(sbyte AssetType, bool Success, UUID AssetFullID)
        {
        }

        public void SendConfirmXfer(ulong xferID, uint PacketID)
        {
        }

        public void SendXferRequest(ulong XferID, short AssetType, UUID vFileID, byte FilePath, byte[] FileName)
        {
        }

        public void SendInitiateDownload(string simFileName, string clientFileName)
        {
        }

        public void SendImageFirstPart(ushort numParts, UUID ImageUUID, uint ImageSize, byte[] ImageData, byte imageCodec)
        {
        }
        
        public void SendImageNextPart(ushort partNumber, UUID imageUuid, byte[] imageData)
        {
        }
         
        public void SendImageNotFound(UUID imageid)
        {
        }
        
        public void SendShutdownConnectionNotice()
        {
        }

        public void SendSimStats(SimStats stats)
        {
        }

        public void SendObjectPropertiesFamilyData(uint RequestFlags, UUID ObjectUUID, UUID OwnerID, UUID GroupID,
                                                    uint BaseMask, uint OwnerMask, uint GroupMask, uint EveryoneMask,
                                                    uint NextOwnerMask, int OwnershipCost, byte SaleType,int SalePrice, uint Category,
                                                    UUID LastOwnerID, string ObjectName, string Description)
        {
        }

        public void SendObjectPropertiesReply(UUID ItemID, ulong CreationDate, UUID CreatorUUID, UUID FolderUUID, UUID FromTaskUUID,
                                              UUID GroupUUID, short InventorySerial, UUID LastOwnerUUID, UUID ObjectUUID,
                                              UUID OwnerUUID, string TouchTitle, byte[] TextureID, string SitTitle, string ItemName,
                                              string ItemDescription, uint OwnerMask, uint NextOwnerMask, uint GroupMask, uint EveryoneMask,
                                              uint BaseMask, byte saleType, int salePrice)
        {
        }

        public void SendAgentOffline(UUID[] agentIDs)
        {

        }

        public void SendAgentOnline(UUID[] agentIDs)
        {

        }

        public void SendSitResponse(UUID TargetID, Vector3 OffsetPos, Quaternion SitOrientation, bool autopilot,
                                        Vector3 CameraAtOffset, Vector3 CameraEyeOffset, bool ForceMouseLook)
        {
            m_log.Debug("[REGION SYNC AVATAR] SendSitResponse");
            if (m_clientView != null)
            {
                OSDMap data = new OSDMap();
                data["agentID"] = OSD.FromUUID(m_agentID);
                data["targetID"] = OSD.FromUUID(TargetID);
                data["offsetPos"] = OSD.FromVector3(OffsetPos);
                data["sitOrientation"] = OSD.FromQuaternion(SitOrientation);
                data["autoPilot"] = OSD.FromBoolean(autopilot);
                data["cameraAtOffset"] = OSD.FromVector3(CameraAtOffset);
                data["cameraEyeOffset"] = OSD.FromVector3(CameraEyeOffset);
                data["forceMouseLook"] = OSD.FromBoolean(ForceMouseLook);
                RegionSyncMessage rsm = new RegionSyncMessage(RegionSyncMessage.MsgType.SitResponse, OSDParser.SerializeJsonString(data));
                m_clientView.Send(rsm);
            }
        }

        public void SendAdminResponse(UUID Token, uint AdminLevel)
        {

        }

        public void SendGroupMembership(GroupMembershipData[] GroupMembership)
        {

        }

        private void Update()
        {
            /*
            frame++;
            if (frame > 20)
            {
                frame = 0;
                if (OnAgentUpdate != null)
                {
                    AgentUpdateArgs pack = new AgentUpdateArgs();
                    pack.ControlFlags = movementFlag;
                    pack.BodyRotation = bodyDirection;

                    OnAgentUpdate(this, pack);
                }
                if (flyState == 0)
                {
                    movementFlag = (uint)AgentManager.ControlFlags.AGENT_CONTROL_FLY |
                                   (uint)AgentManager.ControlFlags.AGENT_CONTROL_UP_NEG;
                    flyState = 1;
                }
                else if (flyState == 1)
                {
                    movementFlag = (uint)AgentManager.ControlFlags.AGENT_CONTROL_FLY |
                                   (uint)AgentManager.ControlFlags.AGENT_CONTROL_UP_POS;
                    flyState = 2;
                }
                else
                {
                    movementFlag = (uint)AgentManager.ControlFlags.AGENT_CONTROL_FLY;
                    flyState = 0;
                }

                if (count >= 10)
                {
                    if (OnChatFromClient != null)
                    {
                        OSChatMessage args = new OSChatMessage();
                        args.Message = "";//Hey You! Get out of my Home. This is my Region";
                        args.Channel = 0;
                        args.From = FirstName + " " + LastName;
                        args.Scene = m_scene;
                        args.Position = new Vector3(128, 128, 26);
                        args.Sender = this;
                        args.Type = ChatTypeEnum.Shout;

                        OnChatFromClient(this, args);
                    }
                    count = -1;
                }

                count++;
            }
             * */
        }

        public bool AddMoney(int debit)
        {
            return false;
        }

        public void SendSunPos(Vector3 sunPos, Vector3 sunVel, ulong time, uint dlen, uint ylen, float phase)
        {
        }
        
        public void SendViewerEffect(ViewerEffectPacket.EffectBlock[] effectBlocks)
        {
        }

        public void SendViewerTime(int phase)
        {
        }

        public void SendAvatarProperties(UUID avatarID, string aboutText, string bornOn, Byte[] charterMember,
                                         string flAbout, uint flags, UUID flImageID, UUID imageID, string profileURL,
                                         UUID partnerID)
        {
        }

        public void SetDebugPacketLevel(int newDebug)
        {
        }

        public void InPacket(object NewPack)
        {
        }

        public void ProcessInPacket(Packet NewPack)
        {
        }

        public void Close()
        {
        }

        public void Start()
        {
        }
        
        public void Stop()
        {
        }

        private uint m_circuitCode;

        public uint CircuitCode
        {
            get { return m_circuitCode; }
            set { m_circuitCode = value; }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return new IPEndPoint(IPAddress.Loopback, (ushort)m_circuitCode); }
        }

        public void SendBlueBoxMessage(UUID FromAvatarID, String FromAvatarName, String Message)
        {

        }
        public void SendLogoutPacket()
        {
        }

        public void Terminate()
        {
        }

        public EndPoint GetClientEP()
        {
            return null;
        }

        public ClientInfo GetClientInfo()
        {
            return null;
        }

        public void SetClientInfo(ClientInfo info)
        {
        }

        public void SendScriptQuestion(UUID objectID, string taskName, string ownerName, UUID itemID, int question)
        {
        }
        public void SendHealth(float health)
        {
        }

        public void SendEstateManagersList(UUID invoice, UUID[] EstateManagers, uint estateID)
        {
        }

        public void SendBannedUserList(UUID invoice, EstateBan[] banlist, uint estateID)
        {
        }

        public void SendRegionInfoToEstateMenu(RegionInfoForEstateMenuArgs args)
        {
        }
        
        public void SendEstateCovenantInformation(UUID covenant)
        {
        }
        
        public void SendDetailedEstateData(UUID invoice, string estateName, uint estateID, uint parentEstate, uint estateFlags, uint sunPosition, UUID covenant, string abuseEmail, UUID estateOwner)
        {
        }

        public void SendLandProperties(int sequence_id, bool snap_selection, int request_result, LandData landData, float simObjectBonusFactor, int parcelObjectCapacity, int simObjectCapacity, uint regionFlags)
        {
        }
        
        public void SendLandAccessListData(List<UUID> avatars, uint accessFlag, int localLandID)
        {
        }
        
        public void SendForceClientSelectObjects(List<uint> objectIDs)
        {
        }
        
        public void SendLandObjectOwners(LandData land, List<UUID> groups, Dictionary<UUID, int> ownersAndCount)
        {
        }

        public void SendCameraConstraint(Vector4 ConstraintPlane)
        {

        }
        
        public void SendLandParcelOverlay(byte[] data, int sequence_id)
        {
        }

        public void SendParcelMediaCommand(uint flags, ParcelMediaCommandEnum command, float time)
        {
        }

        public void SendParcelMediaUpdate(string mediaUrl, UUID mediaTextureID, byte autoScale, string mediaType,
                                          string mediaDesc, int mediaWidth, int mediaHeight, byte mediaLoop)
        {
        }

        public void SendGroupNameReply(UUID groupLLUID, string GroupName)
        {
        }

        public void SendLandStatReply(uint reportType, uint requestFlags, uint resultCount, LandStatReportItem[] lsrpia)
        {
        }

        public void SendScriptRunningReply(UUID objectID, UUID itemID, bool running)
        {
        }

        public void SendAsset(AssetRequestToClient req)
        {
        }

        public void SendTexture(AssetBase TextureAsset)
        {

        }

        public void SendSetFollowCamProperties (UUID objectID, SortedDictionary<int, float> parameters)
        {
        }

        public void SendClearFollowCamProperties (UUID objectID)
        {
        }

        public void SendRegionHandle (UUID regoinID, ulong handle)
        {
        }

        public void SendParcelInfo (RegionInfo info, LandData land, UUID parcelID, uint x, uint y)
        {
        }

        public void SetClientOption(string option, string value)
        {
        }

        public string GetClientOption(string option)
        {
            return string.Empty;
        }

        public void SendScriptTeleportRequest(string objName, string simName, Vector3 pos, Vector3 lookAt)
        {
        }

        public void SendDirPlacesReply(UUID queryID, DirPlacesReplyData[] data)
        {
        }

        public void SendDirPeopleReply(UUID queryID, DirPeopleReplyData[] data)
        {
        }

        public void SendDirEventsReply(UUID queryID, DirEventsReplyData[] data)
        {
        }

        public void SendDirGroupsReply(UUID queryID, DirGroupsReplyData[] data)
        {
        }

        public void SendDirClassifiedReply(UUID queryID, DirClassifiedReplyData[] data)
        {
        }

        public void SendDirLandReply(UUID queryID, DirLandReplyData[] data)
        {
        }

        public void SendDirPopularReply(UUID queryID, DirPopularReplyData[] data)
        {
        }

        public void SendMapItemReply(mapItemReply[] replies, uint mapitemtype, uint flags)
        {
        }

        public void KillEndDone()
        {
        }

        public void SendEventInfoReply (EventData info)
        {
        }

        public void SendOfferCallingCard (UUID destID, UUID transactionID)
        {
        }

        public void SendAcceptCallingCard (UUID transactionID)
        {
        }

        public void SendDeclineCallingCard (UUID transactionID)
        {
        }

        public void SendAvatarGroupsReply(UUID avatarID, GroupMembershipData[] data)
        {
        }

        public void SendJoinGroupReply(UUID groupID, bool success)
        {
        }

        public void SendEjectGroupMemberReply(UUID agentID, UUID groupID, bool succss)
        {
        }

        public void SendLeaveGroupReply(UUID groupID, bool success)
        {
        }

        public void SendTerminateFriend(UUID exFriendID)
        {
        }

        #region IClientAPI Members


        public bool AddGenericPacketHandler(string MethodName, GenericMessage handler)
        {
            return true;
        }

        public void SendAvatarClassifiedReply(UUID targetID, UUID[] classifiedID, string[] name)
        {
        }

        public void SendClassifiedInfoReply(UUID classifiedID, UUID creatorID, uint creationDate, uint expirationDate, uint category, string name, string description, UUID parcelID, uint parentEstate, UUID snapshotID, string simName, Vector3 globalPos, string parcelName, byte classifiedFlags, int price)
        {
        }

        public void SendAgentDropGroup(UUID groupID)
        {
        }

        public void SendAvatarNotesReply(UUID targetID, string text)
        {
        }

        public void SendAvatarPicksReply(UUID targetID, Dictionary<UUID, string> picks)
        {
        }

        public void SendAvatarClassifiedReply(UUID targetID, Dictionary<UUID, string> classifieds)
        {
        }

        public void SendParcelDwellReply(int localID, UUID parcelID, float dwell)
        {
        }

        public void SendUserInfoReply(bool imViaEmail, bool visible, string email)
        {
        }

        public void SendCreateGroupReply(UUID groupID, bool success, string message)
        {
        }

        public void RefreshGroupMembership()
        {
        }

        public void SendUseCachedMuteList()
        {
        }

        public void SendMuteListUpdate(string filename)
        {
        }

        public void SendPickInfoReply(UUID pickID,UUID creatorID, bool topPick, UUID parcelID, string name, string desc, UUID snapshotID, string user, string originalName, string simName, Vector3 posGlobal, int sortOrder, bool enabled)
        {
        }
        #endregion
        
        public void SendRebakeAvatarTextures(UUID textureID)
        {
        }

        public bool IsLoggingOut
        {
            get { return false; }
            set { }
        }
        public void SendAvatarInterestsReply(UUID avatarID, uint wantMask, string wantText, uint skillsMask, string skillsText, string languages)
        {
            throw new System.NotImplementedException();
        }

        public void SendGroupAccountingDetails(IClientAPI sender, UUID groupID, UUID transactionID, UUID sessionID, int amt)
        {
            throw new System.NotImplementedException();
        }

        public void SendGroupAccountingSummary(IClientAPI sender, UUID groupID, uint moneyAmt, int totalTier, int usedTier)
        {
            throw new System.NotImplementedException();
        }

        public void SendGroupTransactionsSummaryDetails(IClientAPI sender, UUID groupID, UUID transactionID, UUID sessionID, int amt)
        {
            throw new System.NotImplementedException();
        }

        public void SendGroupVoteHistory(UUID groupID, UUID transactionID, GroupVoteHistory[] Votes)
        {
            throw new System.NotImplementedException();
        }

        public void SendGenericMessage(string method, List<byte[]> message)
        {
            throw new System.NotImplementedException();
        }

        public void SendGroupActiveProposals(UUID groupID, UUID transactionID, GroupActiveProposals[] Proposals)
        {
            throw new System.NotImplementedException();
        }

        public void SendChangeUserRights(UUID agentID, UUID friendID, int rights)
        {
            throw new System.NotImplementedException();
        }

        public void SendTextBoxRequest(string message, int chatChannel, string objectname, string ownerFirstName, string ownerLastName, UUID objectId)
        {
            throw new System.NotImplementedException();
        }

        public void SendEstateList(UUID invoice, int code, UUID[] Data, uint estateID)
        {
            throw new System.NotImplementedException();
        }

        public void StopFlying(ISceneEntity p)
        {
            throw new System.NotImplementedException();
        }

        public void SendPlacesReply(UUID queryID, UUID transactionID, PlacesReplyData[] data)
        {
            throw new System.NotImplementedException();
        }

        #region IClientCore

        private readonly Dictionary<Type, object> m_clientInterfaces = new Dictionary<Type, object>();

        /// <summary>
        /// Register an interface on this client, should only be called in the constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iface"></param>
        protected void RegisterInterface<T>(T iface)
        {
            lock (m_clientInterfaces)
            {
                if (!m_clientInterfaces.ContainsKey(typeof(T)))
                {
                    m_clientInterfaces.Add(typeof(T), iface);
                }
            }
        }

        public bool TryGet<T>(out T iface)
        {
            if (m_clientInterfaces.ContainsKey(typeof(T)))
            {
                iface = (T)m_clientInterfaces[typeof(T)];
                return true;
            }
            iface = default(T);
            return false;
        }

        public T Get<T>()
        {
            return (T)m_clientInterfaces[typeof(T)];
        }

        public void Disconnect(string reason)
        {
            Close();
        }

        public void Disconnect()
        {
            Close();
        }

        #endregion

    }
}
