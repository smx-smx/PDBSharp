#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Smx.PDBSharp
{
	public class WriterBase : StreamCommon
	{
		private readonly BinaryWriter Writer;

		public WriterBase(Stream stream) : base(stream) {
			this.Writer = new BinaryWriter(Stream);
		}

		public void WriteByte(byte value) => Writer.Write(value);
		public void WriteBytes(byte[] value) => Writer.Write(value);
		public void WriteInt16(Int16 value) => Writer.Write(value);
		public void WriteInt32(Int32 value) => Writer.Write(value);
		public void WriteInt64(Int64 value) => Writer.Write(value);
		public void WriteUInt16(UInt16 value) => Writer.Write(value);
		public void WriteUInt32(UInt32 value) => Writer.Write(value);
		public void WriteUInt64(UInt64 value) => Writer.Write(value);
		public void WriteString(string value) => Writer.Write(value);
		public void WriteSingle(float value) => Writer.Write(value);
		public void WriteDouble(double value) => Writer.Write(value);

		public void WriteEnum<T>(T value) where T : struct, IConvertible {
			Type enumType = typeof(T);
			int enumSize = Marshal.SizeOf(Enum.GetUnderlyingType(enumType));

			object obj = value;
			switch (enumSize) {
				case 1:
					WriteByte((byte)obj);
					break;
				case 2:
					WriteUInt16((ushort)obj);
					break;
				case 4:
					WriteUInt32((uint)obj);
					break;
				case 8:
					WriteUInt64((ulong)obj);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public void WriteCString(string value) {
			WriteBytes(Encoding.ASCII.GetBytes(value));
			WriteByte(0x00);
		}

		private byte[] StructToBytes<T>(T data) {
			byte[] rawData = new byte[Marshal.SizeOf(data)];
			GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
			try {
				IntPtr rawDataPtr = handle.AddrOfPinnedObject();
				Marshal.StructureToPtr(data, rawDataPtr, false);
			} finally {
				handle.Free();
			}

			return rawData;
		}

		public void WriteStruct<T>(T value) where T : struct {
			WriteBytes(StructToBytes<T>(value));
		}
	}
}
