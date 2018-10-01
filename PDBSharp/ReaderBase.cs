#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
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
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public struct ReaderCBArg
	{
		public Stream Stream;
		public BinaryReader Reader;
	}

	public abstract class ReaderBase
	{
		private static readonly Dictionary<LeafType, ConstructorInfo> leafReaders;
		private static readonly Dictionary<ThunkType, ConstructorInfo> thunkReaders;

		static ReaderBase() {
			leafReaders = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.GetCustomAttribute<LeafReaderAttribute>() != null)
				.ToDictionary(
					// key
					t => t.GetCustomAttribute<LeafReaderAttribute>().Type,
					// value
					t => t.GetConstructor(new Type[] { typeof(Stream) }
				));

			thunkReaders = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.GetCustomAttribute<ThunkReaderAttribute>() != null)
				.ToDictionary(
					// key
					t => t.GetCustomAttribute<ThunkReaderAttribute>().Type,
					// value
					t => t.GetConstructor(new Type[] { typeof(THUNKSYM32), typeof(Stream) }
				));
		}

		protected readonly Stream Stream;
		protected readonly BinaryReader Reader;

		public ReaderBase(Stream stream) {
			this.Stream = stream;
			this.Reader = new BinaryReader(Stream);
		}

		public T PerformAt<T>(long offset, Func<T> action) {
			long curPos = Stream.Position;
			Stream.Position = offset;
			T result = action.Invoke();
			Stream.Position = curPos;
			return result;
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

		public int AlignTo(uint alignment) {
			long position = (Stream.Position + alignment - 1) & ~(alignment - 1);
			long skipped = position - Stream.Position;
			Stream.Position = position;
			return (int)skipped;
		}

		public byte[] ReadRemaining() {
			long remaining = Stream.Length - Stream.Position;
			return Reader.ReadBytes((int)remaining);
		}

		public string ReadSymbolString(SymbolHeader symHdr) {
			if (symHdr.Type < SymbolType.S_ST_MAX) {
				return Reader.ReadString();
			} else {
				return ReadCString();
			}
		}

		public ILeaf ReadNumericLeaf(LeafType type) {
			if (!Enum.IsDefined(typeof(LeafType), type)) {
				throw new InvalidDataException();
			}

			return (ILeaf)leafReaders[type].Invoke(new object[] { Stream });
		}

		public IThunk ReadThunk(THUNKSYM32 thunk) {
			ThunkType type = thunk.Ordinal;
			if (!Enum.IsDefined(typeof(ThunkType), type)) {
				throw new InvalidDataException();
			}

			return (IThunk)thunkReaders[type].Invoke(new object[] { thunk, Stream });
		}
	}
}
