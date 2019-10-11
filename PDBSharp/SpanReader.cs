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
	public class SpanReader : IReader
	{
        public SpanReaderImpl BaseReader { get; }

        public long Position {
			get => BaseReader.Position;
			set => BaseReader.Position = value;
		}

		public long Remaining => BaseReader.Remaining;
		public long Length => BaseReader.Length;

		public SpanReader(IReader reader) {
			switch (reader) {
				case SpanReader otherBase:
					BaseReader = new SpanReaderImpl(otherBase.BaseReader);
					break;
				case SpanReaderImpl other:
					BaseReader = new SpanReaderImpl(other);
					break;
				default:
					throw new InvalidOperationException();
			}
		}

		public ReadOnlyMemory<byte> Memory => BaseReader.Memory;
		public ReadOnlySpan<byte> Span => BaseReader.Span;

		public SpanReader(ReadOnlyMemory<byte> data) {
			this.BaseReader = new SpanReaderImpl(data);
		}

		public SpanReader(byte[] data) {
			this.BaseReader = new SpanReaderImpl(data);
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
					value = BaseReader.ReadByte();
					break;
				case 2:
					value = BaseReader.ReadUInt16();
					break;
				case 4:
					value = BaseReader.ReadUInt32();
					break;
				case 8:
					value = BaseReader.ReadUInt64();
					break;
				default:
					throw new NotImplementedException();
			}

			return (T)value;
		}

		public int AlignStream(uint alignment) {
			long position = (BaseReader.Position + alignment - 1) & ~(alignment - 1);
			long skipped = position - BaseReader.Position;
			BaseReader.Position = position;
			return (int)skipped;
		}

		public void PerformAt(long offset, Action action) {
			long curPos = Position;
			Position = offset;
			action.Invoke();
			Position = curPos;
		}

		public T PerformAt<T>(long offset, Func<T> action) {
			long curPos = BaseReader.Position;
			BaseReader.Position = offset;
			T result = action.Invoke();
			BaseReader.Position = curPos;
			return result;
		}

		public void Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.Begin:
					BaseReader.Position = offset;
					break;
				case SeekOrigin.Current:
					BaseReader.Position += offset;
					break;
				case SeekOrigin.End:
					BaseReader.Position = BaseReader.Length - offset;
					break;
			}
		}

		public virtual byte[] ReadRemaining() {
			return ReadBytes((int)Remaining);
		}

		public byte ReadByte() => BaseReader.ReadByte();
		public byte[] ReadBytes(int count) => BaseReader.ReadBytes(count);
		public short ReadInt16() => BaseReader.ReadInt16();
		public int ReadInt32() => BaseReader.ReadInt32();
		public long ReadInt64() => BaseReader.ReadInt64();
		public ushort ReadUInt16() => BaseReader.ReadUInt16();
		public uint ReadUInt32() => BaseReader.ReadUInt32();
		public ulong ReadUInt64() => BaseReader.ReadUInt64();
		public string ReadString() => BaseReader.ReadString();
		public float ReadSingle() =>  BaseReader.ReadSingle();
		public double ReadDouble() => BaseReader.ReadDouble();
		public T Read<T>() where T : unmanaged => BaseReader.Read<T>();
	}
}
