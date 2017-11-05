using UnityEngine;

namespace UServer3.Rust.Struct
{
    public struct Bounds
	{
		// Token: 0x060009C9 RID: 2505 RVA: 0x0000E580 File Offset: 0x0000C780
		public Bounds(Vector3 center, Vector3 size)
		{
			this.m_Center = center;
			this.m_Extents = size * 0.5f;
		}


		// Token: 0x060009D7 RID: 2519 RVA: 0x0000E6C4 File Offset: 0x0000C8C4
		public override int GetHashCode()
		{
			return this.center.GetHashCode() ^ this.extents.GetHashCode() << 2;
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x0000E704 File Offset: 0x0000C904
		public override bool Equals(object other)
		{
			bool result;
			if (!(other is Bounds))
			{
				result = false;
			}
			else
			{
				Bounds bounds = (Bounds)other;
				result = (this.center.Equals(bounds.center) && this.extents.Equals(bounds.extents));
			}
			return result;
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x060009D9 RID: 2521 RVA: 0x0000E77C File Offset: 0x0000C97C
		// (set) Token: 0x060009DA RID: 2522 RVA: 0x0000E798 File Offset: 0x0000C998
		public Vector3 center
		{
			get => this.m_Center;
			set => this.m_Center = value;
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x060009DB RID: 2523 RVA: 0x0000E7A4 File Offset: 0x0000C9A4
		// (set) Token: 0x060009DC RID: 2524 RVA: 0x0000E7CC File Offset: 0x0000C9CC
		public Vector3 size
		{
			get => this.m_Extents * 2f;
			set => this.m_Extents = value * 0.5f;
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x060009DD RID: 2525 RVA: 0x0000E7E0 File Offset: 0x0000C9E0
		// (set) Token: 0x060009DE RID: 2526 RVA: 0x0000E7FC File Offset: 0x0000C9FC
		public Vector3 extents
		{
			get => this.m_Extents;
			set => this.m_Extents = value;
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x060009DF RID: 2527 RVA: 0x0000E808 File Offset: 0x0000CA08
		// (set) Token: 0x060009E0 RID: 2528 RVA: 0x0000E830 File Offset: 0x0000CA30
		public Vector3 min
		{
			get => this.center - this.extents;
			set => this.SetMinMax(value, this.max);
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x060009E1 RID: 2529 RVA: 0x0000E840 File Offset: 0x0000CA40
		// (set) Token: 0x060009E2 RID: 2530 RVA: 0x0000E868 File Offset: 0x0000CA68
		public Vector3 max
		{
			get => this.center + this.extents;
			set => this.SetMinMax(this.min, value);
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x0000E878 File Offset: 0x0000CA78
		public static bool operator ==(Bounds lhs, Bounds rhs)
		{
			return lhs.center == rhs.center && lhs.extents == rhs.extents;
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x0000E8BC File Offset: 0x0000CABC
		public static bool operator !=(Bounds lhs, Bounds rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x0000E8DC File Offset: 0x0000CADC
		public void SetMinMax(Vector3 min, Vector3 max)
		{
			this.extents = (max - min) * 0.5f;
			this.center = min + this.extents;
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x0000E908 File Offset: 0x0000CB08
		public void Encapsulate(Vector3 point)
		{
			this.SetMinMax(Vector3.Min(this.min, point), Vector3.Max(this.max, point));
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x0000E92C File Offset: 0x0000CB2C
		public void Encapsulate(Bounds bounds)
		{
			this.Encapsulate(bounds.center - bounds.extents);
			this.Encapsulate(bounds.center + bounds.extents);
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x0000E964 File Offset: 0x0000CB64
		public void Expand(float amount)
		{
			amount *= 0.5f;
			this.extents += new Vector3(amount, amount, amount);
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x0000E98C File Offset: 0x0000CB8C
		public void Expand(Vector3 amount)
		{
			this.extents += amount * 0.5f;
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x0000E9AC File Offset: 0x0000CBAC
		public bool Intersects(Bounds bounds)
		{
			return this.min.x <= bounds.max.x && this.max.x >= bounds.min.x && this.min.y <= bounds.max.y && this.max.y >= bounds.min.y && this.min.z <= bounds.max.z && this.max.z >= bounds.min.z;
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x0000EA9C File Offset: 0x0000CC9C
		public override string ToString()
		{
			return string.Format("Center: {0}, Extents: {1}", new object[]
			{
				this.m_Center,
				this.m_Extents
			});
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x0000EAE0 File Offset: 0x0000CCE0
		public string ToString(string format)
		{
			return string.Format("Center: {0}, Extents: {1}", new object[]
			{
				this.m_Center.ToString(format),
				this.m_Extents.ToString(format)
			});
		}

		// Token: 0x040000F7 RID: 247
		private Vector3 m_Center;

		// Token: 0x040000F8 RID: 248
		private Vector3 m_Extents;
	}
}