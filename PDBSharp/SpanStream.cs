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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Smx.PDBSharp
{
	public class SpanStream
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

		public unsafe void Write<T>(T value) where T : unmanaged {
			var start = Memory.Span.Slice(pos, sizeof(T));
			MemoryMarshal.Cast<byte, T>(start)[0] = value;
			pos += sizeof(T);
		}

		public unsafe void WriteAt<T>(long offset, T value) where T : unmanaged {
			Memory.Span.Write<T>((int)offset, value);
		}

		public void WriteMemory<T>(Memory<T> data) where T : unmanaged {
			data.CopyTo(Memory, pos);
			pos += data.Length;
		}

		public void WriteSpan<T>(Span<T> data) where T : unmanaged {
			data.CopyTo(Span, pos);
			pos += data.Length;
		}

		public string ReadBSTR16() {
			int length = ReadByte();
			string str = Encoding.ASCII.GetString(ReadBytes(length));
			pos += length + sizeof(byte) + 1;
			return str;
		}

		public string ReadBSTR32() {
			int length = ReadInt32();
			string str = Encoding.ASCII.GetString(ReadBytes(length));
			pos += length + sizeof(int) + 1;
			return str;
		}

		public void WriteString(string str) {
			WriteUInt32((uint)str.Length);
			WriteBytes(Encoding.ASCII.GetBytes(str));
			WriteByte(0x00);
		}

		public void WriteCString(string str) {
			WriteBytes(Encoding.ASCII.GetBytes(str));
			WriteByte(0x00);
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
			Replace(sizeInBytes);
		}

		public void Replace(int newSizeEmpty) {
			byte[] data = new byte[newSizeEmpty];
			this.Memory = new Memory<byte>(data);
		}

		public void Replace(byte[] newData) {
			this.Memory = new Memory<byte>(newData);
		}

		public void Extend(int newSize) {
			if(newSize <= Memory.Length) {
				throw new ArgumentOutOfRangeException($"New size {newSize} is shorter than current {Memory.Length}");
			}
			byte[] data = new byte[newSize];
			var newMem = new Memory<byte>(data);
			Memory.CopyTo(newMem);

			this.Memory = newMem;
		}

		public unsafe T ReadEnum<T>() where T : unmanaged {
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

		public unsafe T ReadFlagsEnum<T>() where T : unmanaged {
			object value;
			switch (sizeof(T)) {
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
		public void WriteByte(byte value) => Write(value);

		public void WriteBytes(byte[] data) {
			var start = Memory.Span.Slice(pos, data.Length);
			var dspan = new Span<byte>(data);
			dspan.CopyTo(start);
		}

		public short ReadInt16() => Read<short>();
		public void WriteInt16(Int16 value) => Write(value);

		public int ReadInt32() => Read<int>();
		public void WriteInt32(Int32 value) => Write(value);

		public long ReadInt64() => Read<long>();
		public void WriteInt64(Int64 value) => Write(value);

		public float ReadSingle() => Read<float>();
		public void WriteSingle(float value) => Write(value);

		public double ReadDouble() => Read<double>();
		public void WriteDouble(double value) => Write(value);

		public ushort ReadUInt16() => Read<ushort>();
		public void WriteUInt16(UInt16 value) => Write(value);

		public uint ReadUInt32() => Read<uint>();
		public void WriteUInt32(UInt32 value) => Write(value);

		public ulong ReadUInt64() => Read<ulong>();
		public void WriteUInt64(UInt64 value) => Write(value);
	}
}
