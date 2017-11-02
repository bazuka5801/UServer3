using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

namespace UServer3.Rust
{
    public class BaseResource : BaseEntity
    {
        public static List<BaseResource> ListResources = new List<BaseResource>();

        public override void OnEntityCreate(Entity entity)
        {
            base.OnEntityCreate(entity);
            ListResources.Add(this);
        }

        public override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            ListResources.Remove(this);
        }

        public virtual Vector3 GetHitPosition() => base.Position;
    }
}