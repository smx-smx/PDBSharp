#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols;
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
	public class SymbolDataReader : ReaderBase {
		public delegate void SymbolDataReaderHook(ISymbol parsedData, byte[] rawData);
		public static event SymbolDataReaderHook OnDataRead;

		private static readonly Dictionary<SymbolType, ConstructorInfo> parsers;
		static SymbolDataReader() {
			parsers = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.GetCustomAttribute<SymbolReaderAttribute>() != null)
				.ToDictionary(
					// key
					t => t.GetCustomAttribute<SymbolReaderAttribute>().Type,
					// value
					t => t.GetConstructor(new Type[] { typeof(Stream) }
				));
		}

		private ISymbol symbol;

		public ISymbol Symbol {
			get {
				if (symbol == null)
					symbol = GetSymbol();
				return symbol;
			}
		}

		public SymbolDataReader(Stream stream) : base(stream) {
		}

		public SymbolType Type { get; private set; }

		private ISymbol GetSymbol() {
			byte[] data = ReadRemaining();
			Stream.Position = 0;

			UInt16 size = Reader.ReadUInt16();
			UInt16 type = Reader.ReadUInt16();
			if(!Enum.IsDefined(typeof(SymbolType), type)) {
				throw new InvalidDataException();
			}

			// the structures include size and type, so reset position to 0
			Stream.Position = 0;

			this.Type = (SymbolType)type;

			ISymbol sym = null;
			if (parsers.ContainsKey(Type)) {
				sym = (ISymbol)parsers[Type].Invoke(new object[] { Stream });
				OnDataRead?.Invoke(sym, data);
			} else {
				OnDataRead?.Invoke(null, data);
				throw new NotImplementedException();
			}

			return sym;

#if false
			if(Stream.Position != Stream.Length) {
				Trace.WriteLine($"WARNING: {Type.ToString()} didn't consume {Stream.Length - Stream.Position} bytes");
			}
#endif
		}
	}
}
