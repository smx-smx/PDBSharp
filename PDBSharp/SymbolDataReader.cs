#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using Smx.PDBSharp.Thunks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public class SymbolDataReader: TypeDataReader, ISymbol {
		private static readonly Dictionary<ThunkType, ConstructorInfo> thunkReaders;

		static SymbolDataReader() {
			thunkReaders = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.GetCustomAttribute<ThunkReaderAttribute>() != null)
				.ToDictionary(
					// key
					t => t.GetCustomAttribute<ThunkReaderAttribute>().Type,
					// value
					t => t.GetConstructor(new Type[] { typeof(SymbolHeader), typeof(Stream) }
				));
		}

		protected readonly SymbolHeader Header;
		SymbolHeader ISymbol.Header => Header;

		protected readonly PDBFile PDB;

		public SymbolDataReader(PDBFile pdb, SymbolHeader header, Stream stream) : base(pdb, stream) {
			this.PDB = pdb;
			Header = header;
			CheckHeader();
		}

		public SymbolDataReader(PDBFile pdb, Stream stream) : base(pdb, stream) {
			this.PDB = pdb;
			Header = ReadHeader();
			CheckHeader();
		}

		private void CheckHeader() {
			if (!Enum.IsDefined(typeof(SymbolType), Header.Type)) {
				throw new InvalidDataException($"Invalid Symbol Type {Header.Type}");
			}
		}

		private SymbolHeader ReadHeader() {
			return ReadStruct<SymbolHeader>();
		}

		public string ReadSymbolString() {
			try {
				if (Header.Type < SymbolType.S_ST_MAX) {
					return ReadString();
				} else {
					return ReadCString();
				}
			} catch (EndOfStreamException) {
				return null;
			}
		}

		public IThunk ReadThunk(ThunkType type) {
			if (!Enum.IsDefined(typeof(ThunkType), type)) {
				throw new InvalidDataException();
			}

			return (IThunk)thunkReaders[type].Invoke(new object[] { Header, Stream });
		}
	}
}
