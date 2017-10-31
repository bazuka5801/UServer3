using ProtoBuf;
using UnityEngine;

namespace UServer3.Rust
{
    public class BaseEntity : BaseNetworkable
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public int Flags;
        
        public override void OnEntityUpdate(Entity entity)
        {
            base.OnEntityUpdate(entity);
            if (entity.baseEntity != null)
            {
                Position = entity.baseEntity.pos;
                Rotation = entity.baseEntity.rot;
                Flags = entity.baseEntity.flags;
            }
        }

        public void OnPositionUpdate(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}