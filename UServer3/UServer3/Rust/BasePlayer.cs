using System;
using System.Collections.Generic;
using ProtoBuf;
using RakNet.Network;
using SapphireEngine;
using UServer3.Reflection;

namespace UServer3.Rust
{
    public class BasePlayer : BaseCombatEntity
    {
        public static List<BasePlayer> ListPlayers = new List<BasePlayer>();
        public static BasePlayer LocalPlayer = null;
        public static bool IsHaveLocalPlayer => LocalPlayer != null;
        
        public UInt64 SteamID;
        public String Username;
        public BaseHeldEntity ActiveItem;
        public OpCodes.E_PlayerFlags PlayerFlags;
        
        public ModelState ModelState;
        public bool IsSleeping => this.HasPlayerFlag(OpCodes.E_PlayerFlags.Sleeping);
        public bool IsWounded => this.HasPlayerFlag(OpCodes.E_PlayerFlags.Wounded);
        public bool IsActiveItem => this.ActiveItem != null;
        public bool IsLocalPlayer => this == LocalPlayer;
        
        public bool IsDucked => ModelState.ducked;
        public float GetHeight() => IsDucked ? 0.7f : 1.5f;


        public override void OnEntityCreate(Entity entity)
        {
            base.OnEntityCreate(entity);
            ListPlayers.Add(this);
        }
        
        public override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            ListPlayers.Remove(this);
        }

        public override void OnEntityUpdate(Entity entity)
        {
            base.OnEntityUpdate(entity);
            if (entity.basePlayer != null)
            {
                SteamID = entity.basePlayer.userid;
                Username = entity.basePlayer.name;
                ModelState = entity.basePlayer.modelState.Copy();
                PlayerFlags = (OpCodes.E_PlayerFlags) entity.basePlayer.playerFlags;

                if (entity.basePlayer.heldEntity == 0)
                    this.ActiveItem = null;
                else if ((this.ActiveItem == null || this.ActiveItem.UID != entity.basePlayer.heldEntity + 1) &&
                         HasNetworkable(entity.basePlayer.heldEntity + 1))
                {
                    OnChangeActiveItem(entity.basePlayer.heldEntity);
                }
            }
        }
        
        [RPCMethod(OpCodes.ERPCMethodUID.StartLoading)]
        public bool RPC_StartLoading(OpCodes.ERPCMethodUID type, Message message)
        {
            ConsoleSystem.Log("StartLoading");
            
            ListNetworkables.Clear();
            ListPlayers.Clear();
            
            ListNetworkables.Add(this.UID, this);
            ListPlayers.Add(this);

            return false;
        }

        
        public void OnChangeActiveItem(UInt32 activeItem)
        {
            this.ActiveItem = (BaseHeldEntity)ListNetworkables[activeItem + 1];
            if (this.IsLocalPlayer && this.IsActiveItem)
            {
                ConsoleSystem.Log("You use: " + this.ActiveItem.PrefabID);
            }
        }

        #region [Methods] Has and Set Player Flags
        public bool HasPlayerFlag(OpCodes.E_PlayerFlags f)=> ((this.PlayerFlags & f) == f);

        public void SetPlayerFlag(OpCodes.E_PlayerFlags f, bool b)
        {
            if (b)
            {
                if (!this.HasPlayerFlag(f))
                    this.PlayerFlags |= f;
            }
            else
            {
                if (this.HasPlayerFlag(f))
                    this.PlayerFlags &= ~f;
            }
        }
        #endregion
    }
}