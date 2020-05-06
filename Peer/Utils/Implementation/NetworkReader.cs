using System;
using System.Text;
using UnityEngine;

namespace MicroLightServerRuntime.Peer.Utils.Implementation
{
	public class NetworkReader
	{
		private NetBuffer m_buf;

		private const int k_MaxStringLength = 32768;

		private const int k_InitialStringBufferSize = 1024;

		private static byte[] s_StringReaderBuffer;

		private static Encoding s_Encoding;

		public uint Position
		{
			get
			{
				return this.m_buf.Position;
			}
		}

		public int Length
		{
			get
			{
				return this.m_buf.Length;
			}
		}

		public NetworkReader()
		{
			this.m_buf = new NetBuffer();
			NetworkReader.Initialize();
		}

		public NetworkReader(NetworkWriter writer)
		{
			this.m_buf = new NetBuffer(writer.AsArray());
			NetworkReader.Initialize();
		}

		public NetworkReader(byte[] buffer)
		{
			this.m_buf = new NetBuffer(buffer);
			NetworkReader.Initialize();
		}
        public void SetBuffer(byte[] buffer)
        {
            this.m_buf = new NetBuffer(buffer);
            NetworkReader.Initialize();
        }
        private static void Initialize()
		{
			if (NetworkReader.s_Encoding == null)
			{
				NetworkReader.s_StringReaderBuffer = new byte[1024];
				NetworkReader.s_Encoding = new UTF8Encoding();
			}
		}

		public void SeekZero()
		{
			this.m_buf.SeekZero();
		}

		internal void Replace(byte[] buffer)
		{
			this.m_buf.Replace(buffer);
		}

		public uint ReadPackedUInt32()
		{
			byte b = this.ReadByte();
			uint result;
			if (b < 241)
			{
				result = (uint)b;
			}
			else
			{
				byte b2 = this.ReadByte();
				if (b >= 241 && b <= 248)
				{
					result = 240u + 256u * (uint)(b - 241) + (uint)b2;
				}
				else
				{
					byte b3 = this.ReadByte();
					if (b == 249)
					{
						result = 2288u + 256u * (uint)b2 + (uint)b3;
					}
					else
					{
						byte b4 = this.ReadByte();
						if (b == 250)
						{
							result = (uint)((int)b2 + ((int)b3 << 8) + ((int)b4 << 16));
						}
						else
						{
							byte b5 = this.ReadByte();
							if (b < 251)
							{
								throw new IndexOutOfRangeException("ReadPackedUInt32() failure: " + b);
							}
							result = (uint)((int)b2 + ((int)b3 << 8) + ((int)b4 << 16) + ((int)b5 << 24));
						}
					}
				}
			}
			return result;
		}

		public ulong ReadPackedUInt64()
		{
			byte b = this.ReadByte();
			ulong result;
			if (b < 241)
			{
				result = (ulong)b;
			}
			else
			{
				byte b2 = this.ReadByte();
				if (b >= 241 && b <= 248)
				{
					result = 240uL + 256uL * ((ulong)b - 241uL) + (ulong)b2;
				}
				else
				{
					byte b3 = this.ReadByte();
					if (b == 249)
					{
						result = 2288uL + 256uL * (ulong)b2 + (ulong)b3;
					}
					else
					{
						byte b4 = this.ReadByte();
						if (b == 250)
						{
							result = (ulong)b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16);
						}
						else
						{
							byte b5 = this.ReadByte();
							if (b == 251)
							{
								result = (ulong)b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24);
							}
							else
							{
								byte b6 = this.ReadByte();
								if (b == 252)
								{
									result = (ulong)b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32);
								}
								else
								{
									byte b7 = this.ReadByte();
									if (b == 253)
									{
										result = (ulong)b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32) + ((ulong)b7 << 40);
									}
									else
									{
										byte b8 = this.ReadByte();
										if (b == 254)
										{
											result = (ulong)b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32) + ((ulong)b7 << 40) + ((ulong)b8 << 48);
										}
										else
										{
											byte b9 = this.ReadByte();
											if (b != 255)
											{
												throw new IndexOutOfRangeException("ReadPackedUInt64() failure: " + b);
											}
											result = (ulong)b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32) + ((ulong)b7 << 40) + ((ulong)b8 << 48) + ((ulong)b9 << 56);
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		

		public byte ReadByte()
		{
			return this.m_buf.ReadByte();
		}

		public sbyte ReadSByte()
		{
			return (sbyte)this.m_buf.ReadByte();
		}

		public short ReadInt16()
		{
			ushort num = 0;
			num |= (ushort)this.m_buf.ReadByte();
			num |= (ushort)(this.m_buf.ReadByte() << 8);
			return (short)num;
		}

		public ushort ReadUInt16()
		{
			ushort num = 0;
			num |= (ushort)this.m_buf.ReadByte();
			return (ushort)(num | (ushort)((this.m_buf.ReadByte() << 8)));
		}

		public int ReadInt32()
		{
			uint num = 0u;
			num |= (uint)this.m_buf.ReadByte();
			num |= (uint)((uint)this.m_buf.ReadByte() << 8);
			num |= (uint)((uint)this.m_buf.ReadByte() << 16);
			return (int)(num | (uint)((uint)this.m_buf.ReadByte() << 24));
		}

		public uint ReadUInt32()
		{
			uint num = 0u;
			num |= (uint)this.m_buf.ReadByte();
			num |= (uint)((uint)this.m_buf.ReadByte() << 8);
			num |= (uint)((uint)this.m_buf.ReadByte() << 16);
			return num | (uint)((uint)this.m_buf.ReadByte() << 24);
		}

		public long ReadInt64()
		{
			ulong num = 0uL;
			ulong num2 = (ulong)this.m_buf.ReadByte();
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 8;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 16;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 24;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 32;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 40;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 48;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 56;
			return (long)(num | num2);
		}

		public ulong ReadUInt64()
		{
			ulong num = 0uL;
			ulong num2 = (ulong)this.m_buf.ReadByte();
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 8;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 16;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 24;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 32;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 40;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 48;
			num |= num2;
			num2 = (ulong)this.m_buf.ReadByte() << 56;
			return num | num2;
		}

		public decimal ReadDecimal()
		{
			return new decimal(new int[]
			{
				this.ReadInt32(),
				this.ReadInt32(),
				this.ReadInt32(),
				this.ReadInt32()
			});
		}

		public float ReadSingle()
		{
			uint value = this.ReadUInt32();
			return FloatConversion.ToSingle(value);
		}

		public double ReadDouble()
		{
			ulong value = this.ReadUInt64();
			return FloatConversion.ToDouble(value);
		}

		public string ReadString()
		{
			ushort num = this.ReadUInt16();
			string result;
			if (num == 0)
			{
				result = "";
			}
			else
			{
				if (num >= 32768)
				{
					throw new IndexOutOfRangeException("ReadString() too long: " + num);
				}
				while ((int)num > NetworkReader.s_StringReaderBuffer.Length)
				{
					NetworkReader.s_StringReaderBuffer = new byte[NetworkReader.s_StringReaderBuffer.Length * 2];
				}
				this.m_buf.ReadBytes(NetworkReader.s_StringReaderBuffer, (uint)num);
				char[] chars = NetworkReader.s_Encoding.GetChars(NetworkReader.s_StringReaderBuffer, 0, (int)num);
				result = new string(chars);
			}
			return result;
		}

		public char ReadChar()
		{
			return (char)this.m_buf.ReadByte();
		}

		public bool ReadBoolean()
		{
			int num = (int)this.m_buf.ReadByte();
			return num == 1;
		}

		public byte[] ReadBytes(int count)
		{
			if (count < 0)
			{
				throw new IndexOutOfRangeException("NetworkReader ReadBytes " + count);
			}
			byte[] array = new byte[count];
			this.m_buf.ReadBytes(array, (uint)count);
			return array;
		}

		public byte[] ReadBytesAndSize()
		{
			ushort num = this.ReadUInt16();
			byte[] result;
			if (num == 0)
			{
				result = null;
			}
			else
			{
				result = this.ReadBytes((int)num);
			}
			return result;
		}

		public Vector2 ReadVector2()
		{
			return new Vector2(this.ReadSingle(), this.ReadSingle());
		}

		public Vector3 ReadVector3()
		{
			return new Vector3(this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
		}

		public Vector4 ReadVector4()
		{
			return new Vector4(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
		}

		public Color ReadColor()
		{
			return new Color(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
		}

		public Color32 ReadColor32()
		{
			return new Color32(this.ReadByte(), this.ReadByte(), this.ReadByte(), this.ReadByte());
		}

		public Quaternion ReadQuaternion()
		{
			return new Quaternion(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
		}

		public Rect ReadRect()
		{
			return new Rect(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
		}

		public Plane ReadPlane()
		{
			return new Plane(this.ReadVector3(), this.ReadSingle());
		}

		public Ray ReadRay()
		{
			return new Ray(this.ReadVector3(), this.ReadVector3());
		}

		public Matrix4x4 ReadMatrix4x4()
		{
			return new Matrix4x4
			{
				m00 = this.ReadSingle(),
				m01 = this.ReadSingle(),
				m02 = this.ReadSingle(),
				m03 = this.ReadSingle(),
				m10 = this.ReadSingle(),
				m11 = this.ReadSingle(),
				m12 = this.ReadSingle(),
				m13 = this.ReadSingle(),
				m20 = this.ReadSingle(),
				m21 = this.ReadSingle(),
				m22 = this.ReadSingle(),
				m23 = this.ReadSingle(),
				m30 = this.ReadSingle(),
				m31 = this.ReadSingle(),
				m32 = this.ReadSingle(),
				m33 = this.ReadSingle()
			};
		}

	
	
	
		public override string ToString()
		{
			return this.m_buf.ToString();
		}

	
	}
}
