using System;
using System.Collections.Generic;
using ProtoBuf;
using RakNet.Network;
using UServer3.Data;
using UServer3.Network;

namespace UServer3.Rust
{
    public class CollectibleEntity : BaseEntity
    {
        public static List<CollectibleEntity> ListCollectibles = new List<CollectibleEntity>();

        public override void OnEntityCreate(Entity entity)
        {
            base.OnEntityCreate(entity);
            ListCollectibles.Add(this);
        }

        public override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            ListCollectibles.Remove(this);
        }
        
        public void PickUp()
        {
            if (VirtualServer.BaseServer.write.Start())
            {
                VirtualServer.BaseServer.write.PacketID(Message.Type.RPCMessage);
                VirtualServer.BaseServer.write.EntityID(this.UID);
                VirtualServer.BaseServer.write.UInt32((UInt32)ERPCMethodUID.Pickup);
                VirtualServer.BaseServer.Send();
            }
        }
    }
}