using System;
using System.Collections.Generic;
using ProtoBuf;
using RakNet.Network;
using SapphireEngine;
using UServer3.Rust;
using BaseEntity = UServer3.Rust.BaseEntity;
using BaseNetworkable = UServer3.Rust.BaseNetworkable;
using BasePlayer = UServer3.Rust.BasePlayer;

namespace UServer3.Network
{
    public class NetworkManager : SapphireType
    {
        public static NetworkManager Instance;

        public override void OnAwake() => Instance = this;

        public bool IN_Entity(Entity entity)
        {
            if (BaseNetworkable.HasNetworkable(entity.baseNetworkable.uid) == false)
            {
                switch ((OpCodes.EPrefabUID) entity.baseNetworkable.prefabID)
                {
                    case OpCodes.EPrefabUID.BasePlayer:
                        new BasePlayer(entity);
                        break;
                    default:
                        if (entity.heldEntity != null)
                            new BaseHeldEntity(entity);
                        else
                            new BaseEntity(entity);
                        break;
                }
            }
            else
                BaseNetworkable.ListNetworkables[entity.baseNetworkable.uid].OnEntityUpdate(entity);
            return BaseNetworkable.ListNetworkables[entity.baseNetworkable.uid].OnEntity(entity);
        }

        public bool IN_NetworkMessage(Message message)
        {

            return false;
        }

        public bool Out_NetworkMessage(Message message)
        {

            return false;
        }

        public void OnDisconnected()
        {
            
        }
    }
}