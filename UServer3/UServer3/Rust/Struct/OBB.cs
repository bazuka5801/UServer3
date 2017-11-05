using UnityEngine;

namespace UServer3.Rust.Struct
{
	public struct OBB
	{
		// Token: 0x04000028 RID: 40
		public Quaternion rotation;

		// Token: 0x04000029 RID: 41
		public Vector3 position;

		// Token: 0x0400002A RID: 42
		public Vector3 extents;

		// Token: 0x0400002B RID: 43
		public Vector3 forward;

		// Token: 0x0400002C RID: 44
		public Vector3 right;

		// Token: 0x0400002D RID: 45
		public Vector3 up;

		// Token: 0x0400002E RID: 46
		public float reject;
		
		// Token: 0x0600005D RID: 93 RVA: 0x000033EC File Offset: 0x000015EC
		public OBB(Vector3 position, Vector3 scale, Quaternion rotation, Bounds bounds)
		{
			this.rotation = rotation;
			this.position = position + rotation * Vector3.Scale(scale, bounds.center);
			this.extents = Vector3.Scale(scale, bounds.extents);
			this.forward = rotation * Vector3.forward;
			this.right = rotation * Vector3.right;
			this.up = rotation * Vector3.up;
			this.reject = this.extents.sqrMagnitude;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003478 File Offset: 0x00001678
		public OBB(Vector3 position, Quaternion rotation, Bounds bounds)
		{
			this.rotation = rotation;
			this.position = position + rotation * bounds.center;
			this.extents = bounds.extents;
			this.forward = rotation * Vector3.forward;
			this.right = rotation * Vector3.right;
			this.up = rotation * Vector3.up;
			this.reject = this.extents.sqrMagnitude;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000034F8 File Offset: 0x000016F8
		public OBB(Vector3 position, Vector3 size, Quaternion rotation)
		{
			this.rotation = rotation;
			this.position = position;
			this.extents = size * 0.5f;
			this.forward = rotation * Vector3.forward;
			this.right = rotation * Vector3.right;
			this.up = rotation * Vector3.up;
			this.reject = this.extents.sqrMagnitude;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003568 File Offset: 0x00001768
		public void Transform(Vector3 position, Vector3 scale, Quaternion rotation)
		{
			this.rotation *= rotation;
			this.position = position + rotation * Vector3.Scale(scale, this.position);
			this.extents = Vector3.Scale(scale, this.extents);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000035B8 File Offset: 0x000017B8
		public Bounds ToBounds()
		{
			Vector3 b = this.extents.x * this.right;
			Vector3 b2 = this.extents.y * this.up;
			Vector3 b3 = this.extents.z * this.forward;
			Bounds result = new Bounds(this.position, Vector3.zero);
			result.Encapsulate(this.position + b2 + b + b3);
			result.Encapsulate(this.position + b2 + b - b3);
			result.Encapsulate(this.position + b2 - b + b3);
			result.Encapsulate(this.position + b2 - b - b3);
			result.Encapsulate(this.position - b2 + b + b3);
			result.Encapsulate(this.position - b2 + b - b3);
			result.Encapsulate(this.position - b2 - b + b3);
			result.Encapsulate(this.position - b2 - b - b3);
			return result;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003718 File Offset: 0x00001918
		public bool Contains(Vector3 target)
		{
			return (target - this.position).sqrMagnitude <= this.reject && this.ClosestPoint(target) == target;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003750 File Offset: 0x00001950
		public bool Intersects(OBB target)
		{
			return target.Contains(this.ClosestPoint(target.position));
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003768 File Offset: 0x00001968
		public bool Intersects(Ray ray)
		{
			RaycastHit raycastHit;
			return this.Trace(ray, out raycastHit, float.PositiveInfinity);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003784 File Offset: 0x00001984
		public bool Trace(Ray ray, out RaycastHit hit, float maxDistance = float.PositiveInfinity)
		{
			hit = default(RaycastHit);
			Vector3 rhs = this.right;
			Vector3 rhs2 = this.up;
			Vector3 rhs3 = this.forward;
			float x = this.extents.x;
			float y = this.extents.y;
			float z = this.extents.z;
			Vector3 arg_75_0 = ray.origin - this.position;
			Vector3 expr_5B = ray.direction;
			float num = Vector3.Dot(expr_5B, rhs);
			float num2 = Vector3.Dot(expr_5B, rhs2);
			float num3 = Vector3.Dot(expr_5B, rhs3);
			float num4 = Vector3.Dot(arg_75_0, rhs);
			float num5 = Vector3.Dot(arg_75_0, rhs2);
			float num6 = Vector3.Dot(arg_75_0, rhs3);
			float f;
			float f2;
			if (num > 0f)
			{
				f = (-x - num4) / num;
				f2 = (x - num4) / num;
			}
			else if (num < 0f)
			{
				f = (x - num4) / num;
				f2 = (-x - num4) / num;
			}
			else
			{
				f = -3.40282347E+38f;
				f2 = 3.40282347E+38f;
			}
			float f3;
			float f4;
			if (num2 > 0f)
			{
				f3 = (-y - num5) / num2;
				f4 = (y - num5) / num2;
			}
			else if (num2 < 0f)
			{
				f3 = (y - num5) / num2;
				f4 = (-y - num5) / num2;
			}
			else
			{
				f3 = -3.40282347E+38f;
				f4 = 3.40282347E+38f;
			}
			float f5;
			float f6;
			if (num3 > 0f)
			{
				f5 = (-z - num6) / num3;
				f6 = (z - num6) / num3;
			}
			else if (num3 < 0f)
			{
				f5 = (z - num6) / num3;
				f6 = (-z - num6) / num3;
			}
			else
			{
				f5 = -3.40282347E+38f;
				f6 = 3.40282347E+38f;
			}
			float num7 = Mathx.Min(f2, f4, f6);
			if (num7 < 0f)
			{
				return false;
			}
			float num8 = Mathx.Max(f, f3, f5);
			if (num8 > num7)
			{
				return false;
			}
			float num9 = Mathf.Clamp(0f, num8, num7);
			if (num9 > maxDistance)
			{
				return false;
			}
			hit.point = ray.origin + ray.direction * num9;
			hit.distance = num9;
			return true;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003974 File Offset: 0x00001B74
		public Vector3 ClosestPoint(Vector3 target)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Vector3 vector = this.position;
			Vector3 expr_19 = target - this.position;
			float num = Vector3.Dot(expr_19, this.right);
			if (num > this.extents.x)
			{
				vector += this.right * this.extents.x;
			}
			else if (num < -this.extents.x)
			{
				vector -= this.right * this.extents.x;
			}
			else
			{
				flag = true;
				vector += this.right * num;
			}
			float num2 = Vector3.Dot(expr_19, this.up);
			if (num2 > this.extents.y)
			{
				vector += this.up * this.extents.y;
			}
			else if (num2 < -this.extents.y)
			{
				vector -= this.up * this.extents.y;
			}
			else
			{
				flag2 = true;
				vector += this.up * num2;
			}
			float num3 = Vector3.Dot(expr_19, this.forward);
			if (num3 > this.extents.z)
			{
				vector += this.forward * this.extents.z;
			}
			else if (num3 < -this.extents.z)
			{
				vector -= this.forward * this.extents.z;
			}
			else
			{
				flag3 = true;
				vector += this.forward * num3;
			}
			if (flag & flag2 & flag3)
			{
				return target;
			}
			return vector;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003B28 File Offset: 0x00001D28
		public float Distance(OBB other)
		{
			OBB oBB = this;
			OBB oBB2 = other;
			Vector3 vector = oBB.position;
			Vector3 vector2 = oBB2.position;
			vector = oBB.ClosestPoint(vector2);
			vector2 = oBB2.ClosestPoint(vector);
			vector = oBB.ClosestPoint(vector2);
			vector2 = oBB2.ClosestPoint(vector);
			return Vector3.Distance(vector, vector2);
		}
	}
}