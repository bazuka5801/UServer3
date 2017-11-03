using System;
using UnityEngine;

namespace UServer3.Rust.Struct
{
    public struct HitInfo
    {
        public UInt32 HitBone;
        public UInt32 HitPartID;
        public Vector3 HitLocalPos;
        public Vector3 HitNormalPos;
    }
}