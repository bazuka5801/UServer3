using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UServer3.Rust
{
    public class BaseNetworkable
    {
        public static Dictionary<UInt32, BaseNetworkable> ListNetworkables = new Dictionary<uint, BaseNetworkable>();
      
        public static bool HasNetworkable(UInt32 uid) => ListNetworkables.ContainsKey(uid);
        public static BaseNetworkable Get(UInt32 uid) => ListNetworkables.TryGetValue(uid, out BaseNetworkable entity) ? entity : null;
        public static T Get<T>(UInt32 uid) where T : BaseNetworkable => Get(uid) as T;
        public static void Destroy(UInt32 uid) => Get(uid)?.OnEntityDestroy();

        public UInt32 UID;
        public UInt32 PrefabID;
        public UInt32 GroupID;

        public virtual void OnEntityCreate(Entity entity)
        {
            ListNetworkables[entity.baseNetworkable.uid] = this;
            OnEntityUpdate(entity);
        }

        public virtual void OnEntityUpdate(Entity entity)
        {
            if (entity.baseNetworkable != null)
            {
                UID = entity.baseNetworkable.uid;
                GroupID = entity.baseNetworkable.@group;
                PrefabID = entity.baseNetworkable.prefabID;
            }
        }
        
        public virtual bool OnEntity(Entity entity) => false;
        
        public virtual void OnEntityDestroy()
        {
            ListNetworkables.Remove(this.UID);
        }

        
        public static void DestroyAll()
        {
            for (int i = ListNetworkables.Count - 1; i >= 0; i--)
            {
                var e = ListNetworkables.ElementAt(i);
                e.Value.OnEntityDestroy();
            }
        }

        public static implicit operator bool(BaseNetworkable obj) => obj != null;
    }
}