using System;
using UnityEngine;

namespace UServer3.Struct
{
    public struct ProjectileHitInfo
    {
        public UInt32 HitBone;
        public UInt32 HitPartID;
        public Vector3 HitLocalPos;
        public Vector3 HitNormalPos;
    }
}