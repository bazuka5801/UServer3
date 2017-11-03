using System;
using UnityEngine;

namespace UServer3.CSharp.ExtensionMethods
{
    public static class QuaternionEx
    {
        public static Quaternion ToQuaternion(this Vector3 euler )
        {
            euler *= 0.0174532924f;
            float heading = euler.y;
            float attitude = euler.z;
            float bank = euler.x;

            // Assuming the angles are in radians.
            float c1 = Mathf.Cos( heading );
            float s1 = Mathf.Sin( heading );
            float c2 = Mathf.Cos( attitude );
            float s2 = Mathf.Sin( attitude );
            float c3 = Mathf.Cos( bank );
            float s3 = Mathf.Sin( bank );
            float w = (float) (Math.Sqrt( 1.0f + c1 * c2 + c1 * c3 - s1 * s2 * s3 + c2 * c3 ) / 2.0f);
            float w4 = ( 4.0f * w );
            var x = ( c2 * s3 + c1 * s3 + s1 * s2 * c3 ) / w4;
            var y = ( s1 * c2 + s1 * c3 + c1 * s2 * s3 ) / w4;
            var z = ( -s1 * s3 + c1 * s2 * c3 + s2 ) / w4;

            return new Quaternion( x, y, z, w );
        }
    }
}