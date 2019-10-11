#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Smx.PDBSharp
{
	public class SpanReaderImpl : IReader
	{
		private int pos;

		public long Position {
			get => pos;
			set => pos = (int)value;
		}

        public ReadOnlyMemory<byte> Memory { get; }
        public ReadOnlySpan<byte> Span => Memory.Span;

		public long Remaining => Length - Position;
		public long Length => Memory.Length;

		public byte ReadByte() => Read<byte>();

		public byte[] ReadBytes(int count) {
			byte[] ret = Memory.Slice(pos, count).ToArray();
			pos += count;
			return ret;
		}

		public unsafe T Read<T>() where T : unmanaged {
			var start = Memory.Span.Slice(pos, sizeof(T));
			T ret = MemoryMarshal.Cast<byte, T>(start)[0];
			pos += sizeof(T);
			return ret;
		}

		public double ReadDouble() => Read<double>();
		public short ReadInt16() => Read<short>();
		public int ReadInt32() => Read<int>();
		public long ReadInt64() => Read<long>();
		public float ReadSingle() => Read<float>();

		public string ReadString() {
			int length = ReadInt32();
			string str = Encoding.ASCII.GetString(ReadBytes(length));
			pos += length + sizeof(int);
			return str;
		}

		public unsafe T ReadStruct<T>() where T : unmanaged {
			int length = sizeof(T);
			var start = Memory.Span.Slice(pos, length);
			T ret = MemoryMarshal.Cast<byte, T>(start)[0];
			pos += length;
			return ret;
		}

		public ushort ReadUInt16() => Read<ushort>();
		public uint ReadUInt32() => Read<uint>();
		public ulong ReadUInt64() => Read<ulong>();


		public SpanReaderImpl(ReadOnlyMemory<byte> data) {
			this.Memory = data;
		}

		public SpanReaderImpl(byte[] data){
			Memory = new ReadOnlyMemory<byte>(data);
		}

		public SpanReaderImpl(SpanReaderImpl other) {
			this.Memory = other.Memory.Slice(other.pos);
		}
	}
}
