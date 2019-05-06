#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp
{
	public class SymbolsReader : ReaderBase
	{
		private static readonly Dictionary<SymbolType, ConstructorInfo> symbolReaders;

		static SymbolsReader() {
			var readers = Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.GetCustomAttribute<SymbolReaderAttribute>() != null);

			var withStream = readers
				.ToDictionary(
					// key
					t => t.GetCustomAttribute<SymbolReaderAttribute>().Type,
					// value
					t => t.GetConstructor(new Type[] {
						typeof(Stream)
					}
				)).Where(p => p.Value != null);

			var withContext = readers
				.ToDictionary(
					// key
					t => t.GetCustomAttribute<SymbolReaderAttribute>().Type,
					// value
					t => t.GetConstructor(new Type[] {
						typeof(PDBFile),
						typeof(Stream)
					}
				)).Where(p => p.Value != null);

			symbolReaders = withStream.Concat(withContext)
				.ToDictionary(i => i.Key, i => i.Value);
		}

		private readonly PDBFile pdb;

		public SymbolsReader(PDBFile pdb, Stream stream) : base(stream) {
			this.pdb = pdb;
		}

		public IEnumerable<ISymbol> ReadSymbols() {
			var remaining = Stream.Length;

			while (remaining > 0) {
				// number of bytes that follow, including symbolType
				UInt16 length = ReadUInt16();

				SymbolType symbolType = ReadEnum<SymbolType>();

				// including symbol length
				int dataSize = length + sizeof(UInt16);
				Stream symDataStream = new MemoryStream(new byte[dataSize]);

				byte[] data = ReadBytes((int)length - sizeof(UInt16));

				BinaryWriter wr = new BinaryWriter(symDataStream);
				wr.Write(length);
				wr.Write((UInt16)symbolType);
				wr.Write(data);
				symDataStream.Position = 0;

				if (symbolReaders.ContainsKey(symbolType)) {
					ConstructorInfo ctor = symbolReaders[symbolType];
					object[] args;
					switch (ctor.GetParameters().Length) {
						case 1:
							args = new object[] { symDataStream };
							break;
						case 2:
							args = new object[] { pdb, symDataStream };
							break;
						default:
							throw new NotSupportedException();
					}
					yield return (ISymbol)symbolReaders[symbolType].Invoke(args);
				} else {
					throw new NotImplementedException($"Symbol type {symbolType} not supported yet");
				}

				if (symDataStream.Position != symDataStream.Length) {
					Trace.WriteLine($"WARNING: {symbolType} didn't consume {symDataStream.Length - symDataStream.Position} bytes");
				}

				remaining -= dataSize;
			}
		}
	}
}
