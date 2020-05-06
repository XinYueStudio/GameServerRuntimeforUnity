using System;
using System.Text;
using UnityEngine;

namespace MicroLightServerRuntime.Peer.Utils.Implementation
{
	public class NetworkWriter
	{
		private const int k_MaxStringLength = 32768;

		private NetBuffer m_Buffer;

		private static Encoding s_Encoding;

		private static byte[] s_StringWriteBuffer;

		private static UIntFloat s_FloatConverter;
       
        public short Position
		{
			get
			{
				return (short)this.m_Buffer.Position;
			}
		}

		public NetworkWriter()
		{
			this.m_Buffer = new NetBuffer();
			if (NetworkWriter.s_Encoding == null)
			{
				NetworkWriter.s_Encoding = new UTF8Encoding();
				NetworkWriter.s_StringWriteBuffer = new byte[32768];
			}
		}

		public NetworkWriter(byte[] buffer)
		{
			this.m_Buffer = new NetBuffer(buffer);
			if (NetworkWriter.s_Encoding == null)
			{
				NetworkWriter.s_Encoding = new UTF8Encoding();
				NetworkWriter.s_StringWriteBuffer = new byte[32768];
			}
		}

		public byte[] ToArray()
		{
			byte[] array = new byte[this.m_Buffer.AsArraySegment().Count];
			Array.Copy(this.m_Buffer.AsArraySegment().Array, array, this.m_Buffer.AsArraySegment().Count);

			return array;
		}

		public byte[] AsArray()
		{
			return this.AsArraySegment().Array;
		}

		internal ArraySegment<byte> AsArraySegment()
		{
			return this.m_Buffer.AsArraySegment();
		}

		public void WritePackedUInt32(uint value)
		{
			if (value <= 240u)
			{
				this.Write((byte)value);
			}
			else if (value <= 2287u)
			{
				this.Write((byte)((value - 240u) / 256u + 241u));
				this.Write((byte)((value - 240u) % 256u));
			}
			else if (value <= 67823u)
			{
				this.Write(249);
				this.Write((byte)((value - 2288u) / 256u));
				this.Write((byte)((value - 2288u) % 256u));
			}
			else if (value <= 16777215u)
			{
				this.Write(250);
				this.Write((byte)(value & 255u));
				this.Write((byte)(value >> 8 & 255u));
				this.Write((byte)(value >> 16 & 255u));
			}
			else
			{
				this.Write(251);
				this.Write((byte)(value & 255u));
				this.Write((byte)(value >> 8 & 255u));
				this.Write((byte)(value >> 16 & 255u));
				this.Write((byte)(value >> 24 & 255u));
			}
		}

		public void WritePackedUInt64(ulong value)
		{
			if (value <= 240uL)
			{
				this.Write((byte)value);
			}
			else if (value <= 2287uL)
			{
				this.Write((byte)((value - 240uL) / 256uL + 241uL));
				this.Write((byte)((value - 240uL) % 256uL));
			}
			else if (value <= 67823uL)
			{
				this.Write(249);
				this.Write((byte)((value - 2288uL) / 256uL));
				this.Write((byte)((value - 2288uL) % 256uL));
			}
			else if (value <= 16777215uL)
			{
				this.Write(250);
				this.Write((byte)(value & 255uL));
				this.Write((byte)(value >> 8 & 255uL));
				this.Write((byte)(value >> 16 & 255uL));
			}
			else if (value < (ulong)0)
			{
				this.Write(251);
				this.Write((byte)(value & 255uL));
				this.Write((byte)(value >> 8 & 255uL));
				this.Write((byte)(value >> 16 & 255uL));
				this.Write((byte)(value >> 24 & 255uL));
			}
			else if (value <= 1099511627775uL)
			{
				this.Write(252);
				this.Write((byte)(value & 255uL));
				this.Write((byte)(value >> 8 & 255uL));
				this.Write((byte)(value >> 16 & 255uL));
				this.Write((byte)(value >> 24 & 255uL));
				this.Write((byte)(value >> 32 & 255uL));
			}
			else if (value <= 281474976710655uL)
			{
				this.Write(253);
				this.Write((byte)(value & 255uL));
				this.Write((byte)(value >> 8 & 255uL));
				this.Write((byte)(value >> 16 & 255uL));
				this.Write((byte)(value >> 24 & 255uL));
				this.Write((byte)(value >> 32 & 255uL));
				this.Write((byte)(value >> 40 & 255uL));
			}
			else if (value <= 72057594037927935uL)
			{
				this.Write(254);
				this.Write((byte)(value & 255uL));
				this.Write((byte)(value >> 8 & 255uL));
				this.Write((byte)(value >> 16 & 255uL));
				this.Write((byte)(value >> 24 & 255uL));
				this.Write((byte)(value >> 32 & 255uL));
				this.Write((byte)(value >> 40 & 255uL));
				this.Write((byte)(value >> 48 & 255uL));
			}
			else
			{
				this.Write(255);
				this.Write((byte)(value & 255uL));
				this.Write((byte)(value >> 8 & 255uL));
				this.Write((byte)(value >> 16 & 255uL));
				this.Write((byte)(value >> 24 & 255uL));
				this.Write((byte)(value >> 32 & 255uL));
				this.Write((byte)(value >> 40 & 255uL));
				this.Write((byte)(value >> 48 & 255uL));
				this.Write((byte)(value >> 56 & 255uL));
			}
		}

		
		public void Write(char value)
		{
			this.m_Buffer.WriteByte((byte)value);
		}

		public void Write(byte value)
		{
			this.m_Buffer.WriteByte(value);
		}

		public void Write(sbyte value)
		{
			this.m_Buffer.WriteByte((byte)value);
		}

		public void Write(short value)
		{
			this.m_Buffer.WriteByte2((byte)(value & 255), (byte)(value >> 8 & 255));
		}

		public void Write(ushort value)
		{
			this.m_Buffer.WriteByte2((byte)(value & 255), (byte)(value >> 8 & 255));
		}

		public void Write(int value)
		{
			this.m_Buffer.WriteByte4((byte)(value & 255), (byte)(value >> 8 & 255), (byte)(value >> 16 & 255), (byte)(value >> 24 & 255));
		}

		public void Write(uint value)
		{
			this.m_Buffer.WriteByte4((byte)(value & 255u), (byte)(value >> 8 & 255u), (byte)(value >> 16 & 255u), (byte)(value >> 24 & 255u));
		}

		public void Write(long value)
		{
			this.m_Buffer.WriteByte8((byte)(value & 255L), (byte)(value >> 8 & 255L), (byte)(value >> 16 & 255L), (byte)(value >> 24 & 255L), (byte)(value >> 32 & 255L), (byte)(value >> 40 & 255L), (byte)(value >> 48 & 255L), (byte)(value >> 56 & 255L));
		}

		public void Write(ulong value)
		{
			this.m_Buffer.WriteByte8((byte)(value & 255uL), (byte)(value >> 8 & 255uL), (byte)(value >> 16 & 255uL), (byte)(value >> 24 & 255uL), (byte)(value >> 32 & 255uL), (byte)(value >> 40 & 255uL), (byte)(value >> 48 & 255uL), (byte)(value >> 56 & 255uL));
		}

		public void Write(float value)
		{
			NetworkWriter.s_FloatConverter.floatValue = value;
			this.Write(NetworkWriter.s_FloatConverter.intValue);
		}

		public void Write(double value)
		{
			NetworkWriter.s_FloatConverter.doubleValue = value;
			this.Write(NetworkWriter.s_FloatConverter.longValue);
		}

		public void Write(decimal value)
		{
			int[] bits = decimal.GetBits(value);
			this.Write(bits[0]);
			this.Write(bits[1]);
			this.Write(bits[2]);
			this.Write(bits[3]);
		}

		public void Write(string value)
		{
			if (value == null)
			{
				this.m_Buffer.WriteByte2(0, 0);
			}
			else
			{
				int byteCount = NetworkWriter.s_Encoding.GetByteCount(value);
				if (byteCount >= 32768)
				{
					throw new IndexOutOfRangeException("Serialize(string) too long: " + value.Length);
				}
				this.Write((ushort)byteCount);
				int bytes = NetworkWriter.s_Encoding.GetBytes(value, 0, value.Length, NetworkWriter.s_StringWriteBuffer, 0);
				this.m_Buffer.WriteBytes(NetworkWriter.s_StringWriteBuffer, (ushort)bytes);
			}
		}

		public void Write(bool value)
		{
			if (value)
			{
				this.m_Buffer.WriteByte(1);
			}
			else
			{
				this.m_Buffer.WriteByte(0);
			}
		}

		public void Write(byte[] buffer, int count)
		{
			if (count > 65535)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
				}
			}
			else
			{
				this.m_Buffer.WriteBytes(buffer, (ushort)count);
			}
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			if (count > 65535)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
				}
			}
			else
			{
				this.m_Buffer.WriteBytesAtOffset(buffer, (ushort)offset, (ushort)count);
			}
		}

		public void WriteBytesAndSize(byte[] buffer, int count)
		{
			if (buffer == null || count == 0)
			{
				this.Write(0);
			}
			else if (count > 65535)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkWriter WriteBytesAndSize: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
				}
			}
			else
			{
				this.Write((ushort)count);
				this.m_Buffer.WriteBytes(buffer, (ushort)count);
			}
		}

		public void WriteBytesFull(byte[] buffer)
		{
			if (buffer == null)
			{
				this.Write(0);
			}
			else if (buffer.Length > 65535)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("NetworkWriter WriteBytes: buffer is too large (" + buffer.Length + ") bytes. The maximum buffer size is 64K bytes.");
				}
			}
			else
			{
				this.Write((ushort)buffer.Length);
				this.m_Buffer.WriteBytes(buffer, (ushort)buffer.Length);
			}
		}

		public void Write(Vector2 value)
		{
			this.Write(value.x);
			this.Write(value.y);
		}

		public void Write(Vector3 value)
		{
			this.Write(value.x);
			this.Write(value.y);
			this.Write(value.z);
		}

		public void Write(Vector4 value)
		{
			this.Write(value.x);
			this.Write(value.y);
			this.Write(value.z);
			this.Write(value.w);
		}

		public void Write(Color value)
		{
			this.Write(value.r);
			this.Write(value.g);
			this.Write(value.b);
			this.Write(value.a);
		}

		public void Write(Color32 value)
		{
			this.Write(value.r);
			this.Write(value.g);
			this.Write(value.b);
			this.Write(value.a);
		}

		public void Write(Quaternion value)
		{
			this.Write(value.x);
			this.Write(value.y);
			this.Write(value.z);
			this.Write(value.w);
		}

		public void Write(Rect value)
		{
			this.Write(value.xMin);
			this.Write(value.yMin);
			this.Write(value.width);
			this.Write(value.height);
		}

		public void Write(Plane value)
		{
			this.Write(value.normal);
			this.Write(value.distance);
		}

		public void Write(Ray value)
		{
			this.Write(value.direction);
			this.Write(value.origin);
		}

		public void Write(Matrix4x4 value)
		{
			this.Write(value.m00);
			this.Write(value.m01);
			this.Write(value.m02);
			this.Write(value.m03);
			this.Write(value.m10);
			this.Write(value.m11);
			this.Write(value.m12);
			this.Write(value.m13);
			this.Write(value.m20);
			this.Write(value.m21);
			this.Write(value.m22);
			this.Write(value.m23);
			this.Write(value.m30);
			this.Write(value.m31);
			this.Write(value.m32);
			this.Write(value.m33);
		}

		public void SeekZero()
		{
			this.m_Buffer.SeekZero();
		}

		public void StartMessage(short msgType)
		{
			this.SeekZero();
			this.m_Buffer.WriteByte2(0, 0);
			this.Write(msgType);
		}

		public void FinishMessage()
		{
			this.m_Buffer.FinishMessage();
		}
	}
}
