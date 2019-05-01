#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using Smx.PDBSharp.Thunks;

namespace Smx.PDBSharp
{
	public class SymbolReaderBase : ReaderBase, ISymbol
	{
		private static readonly Dictionary<LeafType, ConstructorInfo> leafReaders;
		private static readonly Dictionary<ThunkType, ConstructorInfo> thunkReaders;

		static SymbolReaderBase() {
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
					t => t.GetConstructor(new Type[] { typeof(SymbolHeader), typeof(THUNKSYM32), typeof(Stream) }
				));
		}

		public readonly SymbolHeader Header;

		SymbolHeader ISymbol.Header {
			get {
				return Header;
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

			return (IThunk)thunkReaders[type].Invoke(new object[] { Header, thunk, Stream });
		}

		public SymbolReaderBase(Stream stream) : base(stream) {
			Header = ReadStruct<SymbolHeader>();
		}
	}
}
