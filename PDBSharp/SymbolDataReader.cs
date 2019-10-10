#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.PDBSharp.Thunks;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;

namespace Smx.PDBSharp
{
	public class SymbolDataReader : TypeDataReader
	{
		protected readonly SymbolHeader Header;

		protected readonly long startOffset;
		protected readonly long endOffset;

		public new int Remaining => (int)(endOffset - startOffset + Position);

		public SymbolDataReader(IServiceContainer ctx, SymbolHeader header, ReaderSpan stream) : base(ctx, stream) {
			startOffset = stream.Position - Marshal.SizeOf<SymbolHeader>();
			Header = header;
			endOffset = startOffset + Header.Length;
			CheckHeader();
		}

		public SymbolDataReader(IServiceContainer ctx, ReaderSpan stream) : base(ctx, stream) {
			startOffset = stream.Position;
			Header = ReadHeader();
			endOffset = startOffset + sizeof(UInt16) + Header.Length;
			CheckHeader();
		}

		public bool HasMoreData => Position < (startOffset + Header.Length);

		private void CheckHeader() {
			if (!Enum.IsDefined(typeof(SymbolType), Header.Type)) {
				throw new InvalidDataException($"Invalid Symbol Type {Header.Type}");
			}
		}

		public Symbol ReadSymbol(IModule mod, uint offset) {
			if (offset == 0)
				return null;

			if (!(mod is CodeViewModuleReader cv)) {
				throw new InvalidOperationException();
			}


			return cv.PerformAt(offset, () => {
				return new SymbolsReader(ctx, mod, cv).ReadSymbol();
			});
		}

		private SymbolHeader ReadHeader() {
			return Read<SymbolHeader>();
		}

		public string ReadSymbolString() {
			if (Header.Type < SymbolType.S_ST_MAX) {
				return ReadString();
			} else {
				return ReadCString();
			}
		}

		public IThunk ReadThunk(ThunkType type) {
			if (!Enum.IsDefined(typeof(ThunkType), type)) {
				throw new InvalidDataException();
			}

			switch (type) {
				case ThunkType.ADJUSTOR:
					return new ADJUSTOR(ctx, Header, this);
				case ThunkType.NOTYPE:
					return new NOTYPE(ctx, Header, this);
				case ThunkType.PCODE:
					return new PCODE(ctx, Header, this);
				case ThunkType.VCALL:
					return new VCALL(ctx, Header, this);
				default:
					throw new NotImplementedException($"Thunk '{type}' not implemented yet");
			}
		}

		public override byte[] ReadRemaining() {
			return ReadBytes(Remaining);
		}
	}
}
