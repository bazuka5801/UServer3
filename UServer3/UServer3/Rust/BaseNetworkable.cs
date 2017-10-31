using System;
using System.Collections.Generic;
using ProtoBuf;

namespace UServer3.Rust
{
    public class BaseNetworkable
    {
        public static Dictionary<UInt32, BaseNetworkable> ListNetworkables = new Dictionary<uint, BaseNetworkable>();
        public static bool HasNetworkable(UInt32 uid) => ListNetworkables.ContainsKey(uid);
        public static T GetNetworkable<T>(UInt32 uid) where T : BaseNetworkable => HasNetworkable(uid) ? ListNetworkables[uid] as T : null;

        
        public UInt32 UID;
        public UInt32 PrefabID;
        public UInt32 GroupID;

        public BaseNetworkable(Entity entity)
        {
            ListNetworkables[entity.baseNetworkable.uid] = this;
        }

        public virtual void OnEntityUpdate(Entity entity)
        {
            
        }

        public virtual bool OnEntity(Entity entity)
        {

            return false;
        }

        public virtual void OnEntityDestroy()
        {
            if (HasNetworkable(this.UID))
                ListNetworkables.Remove(this.UID);
        }
    }
}