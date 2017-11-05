using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;
using UServer3.CSharp.ExtensionMethods;
using Bounds = UServer3.Rust.Struct.Bounds;
using OBB = UServer3.Rust.Struct.OBB;

namespace UServer3.Rust
{
    public class BaseEntity : BaseNetworkable
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public int Flags;
        public Bounds Bounds;

        public override void OnEntityCreate(Entity entity)
        {
            base.OnEntityCreate(entity);
            Bounds = GetBounds();
        }

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

        public virtual void OnPositionUpdate(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        #region [Geometry]

        public Struct.OBB WorldSpaceBounds()
        {
            return new Struct.OBB(this.Position, Vector3.one, Rotation.ToQuaternion(), this.Bounds);
        }

        public virtual Bounds GetBounds()
        {
            return new Bounds(Position, Vector3.one);
        }
        
        public Vector3 ClosestPoint(Vector3 position)
        {
            return this.WorldSpaceBounds().ClosestPoint(position);
        }
        
        #endregion
        
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