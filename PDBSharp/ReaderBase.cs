#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.Structures;
using Smx.PDBSharp.Thunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public abstract class ReaderBase : StreamCommon
	{

		private readonly BinaryReader Reader;

		public ReaderBase(Stream stream) : base(stream) {
			this.Reader = new BinaryReader(Stream);
		}

		public byte ReadByte() => Reader.ReadByte();
		public byte[] ReadBytes(int count) => Reader.ReadBytes(count);
		public Int16 ReadInt16() => Reader.ReadInt16();
		public Int32 ReadInt32() => Reader.ReadInt32();
		public Int64 ReadInt64() => Reader.ReadInt64();
		public UInt16 ReadUInt16() => Reader.ReadUInt16();
		public UInt32 ReadUInt32() => Reader.ReadUInt32();
		public UInt64 ReadUInt64() => Reader.ReadUInt64();
		public string ReadString() => Reader.ReadString();
		public float ReadSingle() => Reader.ReadSingle();
		public double ReadDouble() => Reader.ReadDouble();

		public T ReadFlagsEnum<T>() where T : struct, IConvertible {
			Type enumType = typeof(T);
			int enumSize = Marshal.SizeOf(Enum.GetUnderlyingType(enumType));

			object value;
			switch (enumSize) {
				case 1:
					value = ReadByte();
					break;
				case 2:
					value = ReadUInt16();
					break;
				case 4:
					value = ReadUInt32();
					break;
				case 8:
					value = ReadUInt64();
					break;
				default:
					throw new NotImplementedException();
			}

			return (T)value;
		}

		public T ReadEnum<T>() where T : struct, IConvertible {
			T value = ReadFlagsEnum<T>();

			Type enumType = typeof(T);
			if (!Enum.IsDefined(enumType, value)) {
				throw new InvalidDataException($"Value 0x{value:X} not defined in enum {enumType.FullName}");
			}

			return (T)value;
		}

		public string ReadCString() {
			StringBuilder sb = new StringBuilder();
			while (true) {
				byte ch = Reader.ReadByte();
				if (ch == 0x00)
					break;

				sb.Append(Convert.ToChar(ch));
			}
			return sb.ToString();
		}

		public T ReadStruct<T>() where T : struct {
			return new StructureReader<T>(new BinaryReader(Stream)).Read();
		}

		public virtual byte[] ReadRemaining() {
			long remaining = Stream.Length - Stream.Position;
			return Reader.ReadBytes((int)remaining);
		}
	}
}
