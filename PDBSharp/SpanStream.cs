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
	public class SpanStream : IReader
	{
		private int pos;

        public long Position {
			get => pos;
			set => pos = (int)value;
		}
		public long Remaining => Length - Position;
		public long Length => Memory.Length;

		public Memory<byte> Memory { get; private set; }
		public Span<byte> Span => Memory.Span;

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

		public string ReadString() {
			int length = ReadInt32();
			string str = Encoding.ASCII.GetString(ReadBytes(length));
			pos += length + sizeof(int) + 1;
			return str;
		}

		public unsafe T ReadStruct<T>() where T : unmanaged {
			int length = sizeof(T);
			var start = Memory.Span.Slice(pos, length);
			T ret = MemoryMarshal.Cast<byte, T>(start)[0];
			pos += length;
			return ret;
		}

		public SpanStream(SpanStream other) {
			this.Memory = other.Memory.Slice(other.pos);
		}


		public SpanStream(Memory<byte> data) {
			this.Memory = data;
		}

		public SpanStream(byte[] data) {
			this.Memory = new Memory<byte>(data);
		}

		public SpanStream(int sizeInBytes) {
			byte[] data = new byte[sizeInBytes];
			this.Memory = new Memory<byte>(data);
		}

		public T ReadEnum<T>() where T : struct, IConvertible {
			T value = ReadFlagsEnum<T>();

			Type enumType = typeof(T);
			if (!Enum.IsDefined(enumType, value)) {
				throw new InvalidDataException($"Value 0x{value:X} not defined in enum {enumType.FullName}");
			}

			return value;
		}

		public string ReadCString() {
			int start = (int)Position;
			while (Span[(int)(Position++)] != 0x00);

			// ignore trailing NULL
			byte[] data = Span.Slice(start, (int)(Position - start - 1)).ToArray();
			return Encoding.ASCII.GetString(data);
		}

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

		public int AlignStream(uint alignment) {
			long position = (Position + alignment - 1) & ~(alignment - 1);
			long skipped = position - Position;
			Position = position;
			return (int)skipped;
		}

		public void PerformAt(long offset, Action action) {
			long curPos = Position;
			Position = offset;
			action.Invoke();
			Position = curPos;
		}

		public T PerformAt<T>(long offset, Func<T> action) {
			long curPos = Position;
			Position = offset;
			T result = action.Invoke();
			Position = curPos;
			return result;
		}

		public void Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.Begin:
					Position = offset;
					break;
				case SeekOrigin.Current:
					Position += offset;
					break;
				case SeekOrigin.End:
					Position = Length - offset;
					break;
			}
		}

		public virtual byte[] ReadRemaining() {
			return ReadBytes((int)Remaining);
		}

		public byte ReadByte() => Read<byte>();
		public double ReadDouble() => Read<double>();
		public short ReadInt16() => Read<short>();
		public int ReadInt32() => Read<int>();
		public long ReadInt64() => Read<long>();
		public float ReadSingle() => Read<float>();
		public ushort ReadUInt16() => Read<ushort>();
		public uint ReadUInt32() => Read<uint>();
		public ulong ReadUInt64() => Read<ulong>();
	}
}
