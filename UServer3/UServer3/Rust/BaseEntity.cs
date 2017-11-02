using System;
using System.Collections.Generic;
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
        
        public static T FindNearEntity<T>(List<T> list, float max_distance = float.PositiveInfinity) where T : BaseEntity
        {
            if (BasePlayer.LocalPlayer == null) return null;
            T nearEntity = null;
            Single min_distance = Single.MaxValue;
            for (int i = 0; i < list.Count; i++)
            {
                var entity = list[i];
                if (entity == BasePlayer.LocalPlayer) continue;
                var distance = Vector3.Distance(entity.Position, BasePlayer.LocalPlayer.Position);
                if (distance < min_distance)
                {
                    min_distance = distance;
                    nearEntity = entity;
                }
            }
            return min_distance < max_distance ? nearEntity : null;
        }
    }
}