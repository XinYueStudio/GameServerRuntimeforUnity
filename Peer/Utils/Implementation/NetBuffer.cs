using System;
using UnityEngine;

namespace MicroLightServerRuntime.Peer.Utils.Implementation
{
	internal class NetBuffer
	{
		private byte[] m_Buffer;

		private uint m_Pos;

		private const int k_InitialSize = 64;

		private const float k_GrowthFactor = 1.5f;

		private const int k_BufferSizeWarning = 134217728;

		public uint Position
		{
			get
			{
				return this.m_Pos;
			}
		}

		public int Length
		{
			get
			{
				return this.m_Buffer.Length;
			}
		}

		public NetBuffer()
		{
			this.m_Buffer = new byte[64];
		}

		public NetBuffer(byte[] buffer)
		{
			this.m_Buffer = buffer;
		}

		public byte ReadByte()
		{
			if ((ulong)this.m_Pos >= (ulong)((long)this.m_Buffer.Length))
			{
				throw new IndexOutOfRangeException("NetworkReader:ReadByte out of range:" + this.ToString());
			}
			return this.m_Buffer[(int)((UIntPtr)(this.m_Pos++))];
		}

		public void ReadBytes(byte[] buffer, uint count)
		{
			if ((ulong)(this.m_Pos + count) > (ulong)((long)this.m_Buffer.Length))
			{
				throw new IndexOutOfRangeException(string.Concat(new object[]
				{
					"NetworkReader:ReadBytes out of range: (",
					count,
					") ",
					this.ToString()
				}));
			}
			ushort num = 0;
			while ((uint)num < count)
			{
				buffer[(int)num] = this.m_Buffer[(int)((UIntPtr)(this.m_Pos + (uint)num))];
				num += 1;
			}
			this.m_Pos += count;
		}

		internal ArraySegment<byte> AsArraySegment()
		{
			return new ArraySegment<byte>(this.m_Buffer, 0, (int)this.m_Pos);
		}

		public void WriteByte(byte value)
		{
			this.WriteCheckForSpace(1);
			this.m_Buffer[(int)((UIntPtr)this.m_Pos)] = value;
			this.m_Pos += 1u;
		}

		public void WriteByte2(byte value0, byte value1)
		{
			this.WriteCheckForSpace(2);
			this.m_Buffer[(int)((UIntPtr)this.m_Pos)] = value0;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 1u))] = value1;
			this.m_Pos += 2u;
		}

		public void WriteByte4(byte value0, byte value1, byte value2, byte value3)
		{
			this.WriteCheckForSpace(4);
			this.m_Buffer[(int)((UIntPtr)this.m_Pos)] = value0;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 1u))] = value1;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 2u))] = value2;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 3u))] = value3;
			this.m_Pos += 4u;
		}

		public void WriteByte8(byte value0, byte value1, byte value2, byte value3, byte value4, byte value5, byte value6, byte value7)
		{
			this.WriteCheckForSpace(8);
			this.m_Buffer[(int)((UIntPtr)this.m_Pos)] = value0;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 1u))] = value1;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 2u))] = value2;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 3u))] = value3;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 4u))] = value4;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 5u))] = value5;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 6u))] = value6;
			this.m_Buffer[(int)((UIntPtr)(this.m_Pos + 7u))] = value7;
			this.m_Pos += 8u;
		}

		public void WriteBytesAtOffset(byte[] buffer, ushort targetOffset, ushort count)
		{
			uint num = (uint)(count + targetOffset);
			this.WriteCheckForSpace((ushort)num);
			if (targetOffset == 0 && (int)count == buffer.Length)
			{
				buffer.CopyTo(this.m_Buffer, (int)this.m_Pos);
			}
			else
			{
				for (int i = 0; i < (int)count; i++)
				{
					this.m_Buffer[(int)targetOffset + i] = buffer[i];
				}
			}
			if (num > this.m_Pos)
			{
				this.m_Pos = num;
			}
		}

		public void WriteBytes(byte[] buffer, ushort count)
		{
			this.WriteCheckForSpace(count);
			if ((int)count == buffer.Length)
			{
				buffer.CopyTo(this.m_Buffer, (int)this.m_Pos);
			}
			else
			{
				for (int i = 0; i < (int)count; i++)
				{
					this.m_Buffer[(int)(checked((IntPtr)(unchecked((ulong)this.m_Pos + (ulong)((long)i)))))] = buffer[i];
				}
			}
			this.m_Pos += (uint)count;
		}

		private void WriteCheckForSpace(ushort count)
		{
			if ((ulong)(this.m_Pos + (uint)count) >= (ulong)((long)this.m_Buffer.Length))
			{
				int num = (int)Math.Ceiling((double)((float)this.m_Buffer.Length * 1.5f));
				while ((ulong)(this.m_Pos + (uint)count) >= (ulong)((long)num))
				{
					num = (int)Math.Ceiling((double)((float)num * 1.5f));
					if (num > 134217728)
					{
						Debug.LogWarning("NetworkBuffer size is " + num + " bytes!");
					}
				}
				byte[] array = new byte[num];
				this.m_Buffer.CopyTo(array, 0);
				this.m_Buffer = array;
			}
		}

		public void FinishMessage()
		{
			ushort num = (ushort)(this.m_Pos - 4u);
			this.m_Buffer[0] = (byte)(num & 255);
			this.m_Buffer[1] = (byte)(num >> 8 & 255);
		}

		public void SeekZero()
		{
			this.m_Pos = 0u;
		}

		public void Replace(byte[] buffer)
		{
			this.m_Buffer = buffer;
			this.m_Pos = 0u;
		}

		public override string ToString()
		{
			return string.Format("NetBuf sz:{0} pos:{1}", this.m_Buffer.Length, this.m_Pos);
		}
	}
}
