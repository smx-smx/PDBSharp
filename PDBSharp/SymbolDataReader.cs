#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.PDBSharp.Thunks;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static Smx.PDBSharp.TypeDataReader;

namespace Smx.PDBSharp
{
	namespace SymbolData
	{
		public class State
		{
			public bool UseUnicodeStrings { get; internal set; }
			public long EndOffset { get; internal set; }
			public long StartOffset { get; internal set; }
		}
		public class Reader : TypeDataReader {

			public State State = new State();
			private readonly IServiceContainer sc;
			private readonly PDBStream.Data pdbStream;
			private readonly ReadStringDelegate ReadString;

			private long Length => State.EndOffset - State.StartOffset;
			public new int Remaining => (int)(Length - Position);
			public bool HasMoreData => Position < (State.StartOffset + Header.Length);

			protected SymbolHeader Header;

			public Reader(IServiceContainer sc, SpanStream stream) : base(sc, stream) {
				this.sc = sc;
				this.pdbStream = sc.GetService<PDBStream.Data>();

				switch (sc.GetService<MSFReader>().FileType) {
					case PDBType.Big:
						ReadString = ReadString32;
						break;
					case PDBType.Small:
						ReadString = ReadString16;
						break;
					default:
						throw new InvalidDataException();
				}
			}

			private SymbolHeader ReadHeader() {
				return Read<SymbolHeader>();
			}

			private static void CheckHeader(SymbolHeader header) {
				if (!Enum.IsDefined(typeof(SymbolType), header.Type)) {
					throw new InvalidDataException($"Invalid Symbol Type {header.Type}");
				}
			}

			public ISymbolResolver? ReadSymbol(uint offset) {
				if (offset == 0)
					return null;

				return new SymbolsReader(sc, this).ReadSymbolDirect();
			}

			public string ReadSymbolString() {
				if (State.UseUnicodeStrings) {
					return ReadCString(Encoding.UTF8);
				} else {
					return ReadString();
				}
			}

			public IThunk ReadThunk(ThunkType type) {
				if (!Enum.IsDefined(typeof(ThunkType), type)) {
					throw new InvalidDataException();
				}

				switch (type) {
					case ThunkType.ADJUSTOR:
						var adjustor = new Thunks.ADJUSTOR.Serializer(sc, Header, this);
						return adjustor.Read();
					case ThunkType.NOTYPE:
						var notype = new Thunks.NOTYPE.Serializer(sc, Header, this);
						return notype.Read();
					case ThunkType.PCODE:
						var pcode = new Thunks.PCODE.Serializer(sc, Header, this);
						return pcode.Read();
					case ThunkType.VCALL:
						var vcall = new Thunks.VCALL.Serializer(sc, Header, this);
						return vcall.Read();
					default:
						throw new NotImplementedException($"Thunk '{type}' not implemented yet");
				}
			}

			public override byte[] ReadRemaining() {
				return ReadBytes(Remaining);
			}

			public State Initialize(SymbolHeader? header = null) {
				var startOffset = Position;
				var endOffset = startOffset;

				if (header == null) {
					header = ReadHeader();
				} else {
					startOffset -= Marshal.SizeOf<SymbolHeader>();
					endOffset += sizeof(ushort);
				}
				Header = header.Value;
				endOffset += Header.Length;

				CheckHeader(Header);
				var useUnicodeStrings = pdbStream.Version >= PDBPublicVersion.VC70;

				State = new State {
					StartOffset = startOffset,
					EndOffset = endOffset,
					UseUnicodeStrings = useUnicodeStrings
				};
				return State;
			}
		}
	}
}
