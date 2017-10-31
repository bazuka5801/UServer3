using System;
using System.IO;
using RakNet.Network;

namespace RakNet
{
    internal class StreamRead : Read
    {
        // Token: 0x06000099 RID: 153 RVA: 0x00003D64 File Offset: 0x00001F64
		public StreamRead(NetworkPeer net, Peer peer)
		{
			this.net = net;
			this.peer = peer;
			this.stream = new MemoryStream();
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00003D85 File Offset: 0x00001F85
		public void Shutdown()
		{
			this.stream.Dispose();
			this.stream = null;
			this.peer = null;
			this.net = null;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00003DA8 File Offset: 0x00001FA8
		public override bool Start()
		{
			if (this.peer == null)
			{
				return false;
			}
			this.stream.Position = 0L;
			this.stream.SetLength(0L);
			this.peer.ReadBytes(this.stream, this.peer.incomingBytesUnread);
			return true;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00003DF8 File Offset: 0x00001FF8
		public override bool Start(Connection connection)
		{
			if (!this.Start())
			{
				return false;
			}
			if (this.stream.Length > 1L && this.net.cryptography != null && this.net.cryptography.IsEnabledIncoming(connection))
			{
				this.net.cryptography.Decrypt(connection, this.stream, 1);
			}
			return true;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00003E57 File Offset: 0x00002057
		public override byte PacketID()
		{
			return this.UInt8();
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00003E5F File Offset: 0x0000205F
		public override bool Bit()
		{
			return base.unread >= 1 && this.stream.ReadByte() != 0;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00003E7A File Offset: 0x0000207A
		public override byte UInt8()
		{
			if (base.unread < 1)
			{
				return 0;
			}
			return this.Read8().u;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00003E92 File Offset: 0x00002092
		public override ushort UInt16()
		{
			if (base.unread < 2)
			{
				return 0;
			}
			return this.Read16().u;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00003EAA File Offset: 0x000020AA
		public override uint UInt32()
		{
			if (base.unread < 4)
			{
				return 0u;
			}
			return this.Read32().u;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00003EC2 File Offset: 0x000020C2
		public override ulong UInt64()
		{
			if (base.unread < 8)
			{
				return 0uL;
			}
			return this.Read64().u;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003EDB File Offset: 0x000020DB
		public override sbyte Int8()
		{
			if (base.unread < 1)
			{
				return 0;
			}
			return this.Read8().i;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003EF3 File Offset: 0x000020F3
		public override short Int16()
		{
			if (base.unread < 2)
			{
				return 0;
			}
			return this.Read16().i;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00003F0B File Offset: 0x0000210B
		public override int Int32()
		{
			if (base.unread < 4)
			{
				return 0;
			}
			return this.Read32().i;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003F23 File Offset: 0x00002123
		public override long Int64()
		{
			if (base.unread < 8)
			{
				return 0L;
			}
			return this.Read64().i;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00003F3C File Offset: 0x0000213C
		public override float Float()
		{
			if (base.unread < 4)
			{
				return 0f;
			}
			return this.Read32().f;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003F58 File Offset: 0x00002158
		public override double Double()
		{
			if (base.unread < 8)
			{
				return 0.0;
			}
			return this.Read64().f;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00003F78 File Offset: 0x00002178
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.stream.Read(buffer, offset, count);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003F88 File Offset: 0x00002188
		public override int ReadByte()
		{
			return this.stream.ReadByte();
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00003F95 File Offset: 0x00002195
		public override long Length => this.stream.Length;

	    // Token: 0x17000012 RID: 18
		// (get) Token: 0x060000AC RID: 172 RVA: 0x00003FA2 File Offset: 0x000021A2
		// (set) Token: 0x060000AD RID: 173 RVA: 0x00003FAF File Offset: 0x000021AF
		public override long Position
		{
			get => this.stream.Position;
			set => this.stream.Position = value;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00003FBD File Offset: 0x000021BD
		private byte[] GetReadBuffer()
		{
			return this.stream.GetBuffer();
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003FCC File Offset: 0x000021CC
		private long GetReadOffset(long i)
		{
			long position = this.stream.Position;
			this.stream.Position = position + i;
			return position;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00003FF4 File Offset: 0x000021F4
		private Union8 Read8()
		{
			long readOffset = this.GetReadOffset(1L);
			byte[] readBuffer = this.GetReadBuffer();
			Union8 result = default(Union8);
			result.b1 = readBuffer[(int)(checked((IntPtr)readOffset))];
			return result;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00004028 File Offset: 0x00002228
		private Union16 Read16()
		{
			long readOffset = this.GetReadOffset(2L);
			byte[] readBuffer = this.GetReadBuffer();
			Union16 result = default(Union16);
			checked
			{
				result.b1 = readBuffer[(int)((IntPtr)readOffset)];
				result.b2 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 1L)))];
				return result;
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00004068 File Offset: 0x00002268
		private Union32 Read32()
		{
			long readOffset = this.GetReadOffset(4L);
			byte[] readBuffer = this.GetReadBuffer();
			Union32 result = default(Union32);
			checked
			{
				result.b1 = readBuffer[(int)((IntPtr)readOffset)];
				result.b2 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 1L)))];
				result.b3 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 2L)))];
				result.b4 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 3L)))];
				return result;
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000040C4 File Offset: 0x000022C4
		private Union64 Read64()
		{
			long readOffset = this.GetReadOffset(8L);
			byte[] readBuffer = this.GetReadBuffer();
			Union64 result = default(Union64);
			checked
			{
				result.b1 = readBuffer[(int)((IntPtr)readOffset)];
				result.b2 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 1L)))];
				result.b3 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 2L)))];
				result.b4 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 3L)))];
				result.b5 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 4L)))];
				result.b6 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 5L)))];
				result.b7 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 6L)))];
				result.b8 = readBuffer[(int)((IntPtr)(unchecked(readOffset + 7L)))];
				return result;
			}
		}

		// Token: 0x04000021 RID: 33
		private NetworkPeer net;

		// Token: 0x04000022 RID: 34
		private Peer peer;

		// Token: 0x04000023 RID: 35
		private MemoryStream stream;
    }
}