#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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
	public delegate void OnSymbolDataDelegate(byte[] data);

	public class SymbolsReader : ReaderBase
	{
		private readonly PDBFile pdb;

		public event OnSymbolDataDelegate OnSymbolData;

		public SymbolsReader(PDBFile pdb, Stream stream) : base(stream) {
			this.pdb = pdb;
		}

		private ISymbol ReadSymbol(SymbolType symbolType, Stream Stream) {
			switch (symbolType) {
				case SymbolType.S_BLOCK32:
					return new S_BLOCK32(pdb, Stream);
				case SymbolType.S_BPREL32:
					return new S_BPREL32(pdb, Stream);
				case SymbolType.S_BUILDINFO:
					return new S_BUILDINFO(pdb, Stream);
				case SymbolType.S_CALLEES:
					return new S_CALLEES(pdb, Stream);
				case SymbolType.S_CALLSITEINFO:
					return new S_CALLSITEINFO(pdb, Stream);
				case SymbolType.S_COFFGROUP:
					return new S_COFFGROUP(pdb, Stream);
				case SymbolType.S_COMPILE:
					return new S_COMPILE(pdb, Stream);
				case SymbolType.S_COMPILE2:
					return new S_COMPILE2(pdb, Stream);
				case SymbolType.S_COMPILE3:
					return new S_COMPILE3(pdb, Stream);
				case SymbolType.S_DEFRANGE_FRAMEPOINTER_REL:
					return new S_DEFRANGE_FRAMEPOINTER_REL(pdb, Stream);
				case SymbolType.S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE:
					return new S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE(pdb, Stream);
				case SymbolType.S_DEFRANGE_REGISTER:
					return new S_DEFRANGE_REGISTER(pdb, Stream);
				case SymbolType.S_DEFRANGE_REGISTER_REL:
					return new S_DEFRANGE_REGISTER_REL(pdb, Stream);
				case SymbolType.S_DEFRANGE_SUBFIELD_REGISTER:
					return new S_DEFRANGE_SUBFIELD_REGISTER(pdb, Stream);
				case SymbolType.S_END:
					// S_END has no data because it's used as marker
					return null;
				case SymbolType.S_ENVBLOCK:
					return new S_ENVBLOCK(pdb, Stream);
				case SymbolType.S_EXPORT:
					return new S_EXPORT(pdb, Stream);
				case SymbolType.S_FILESTATIC:
					return new S_FILESTATIC(pdb, Stream);
				case SymbolType.S_FRAMECOOKIE:
					return new S_FRAMECOOKIE(pdb, Stream);
				case SymbolType.S_FRAMEPROC:
					return new S_FRAMEPROC(pdb, Stream);
				case SymbolType.S_GDATA32:
					return new S_GDATA32(pdb, Stream);
				case SymbolType.S_INLINESITE:
					return new S_INLINESITE(pdb, Stream);
				case SymbolType.S_LDATA32:
					return new S_LDATA32(pdb, Stream);
				case SymbolType.S_LMANDATA:
					return new S_LMANDATA(pdb, Stream);
				case SymbolType.S_GMANPROC:
					return new S_GMANPROC(pdb, Stream);
				case SymbolType.S_LMANPROC:
					return new S_LMANPROC(pdb, Stream);
				case SymbolType.S_GPROC32:
					return new S_GPROC32(pdb, Stream);
				case SymbolType.S_LPROC32:
					return new S_LPROC32(pdb, Stream);
				case SymbolType.S_HEAPALLOCSITE:
					return new S_HEAPALLOCSITE(pdb, Stream);
				case SymbolType.S_LABEL32:
					return new S_LABEL32(pdb, Stream);
				case SymbolType.S_LOCAL:
					return new S_LOCAL(pdb, Stream);
				case SymbolType.S_CONSTANT:
					return new S_CONSTANT(pdb, Stream);
				case SymbolType.S_MANCONSTANT:
					return new S_MANCONSTANT(pdb, Stream);
				case SymbolType.S_MANSLOT:
					return new S_MANSLOT(pdb, Stream);
				case SymbolType.S_OBJNAME:
					return new S_OBJNAME(pdb, Stream);
				case SymbolType.S_OEM:
					return new S_OEM(pdb, Stream);
				case SymbolType.S_REGISTER:
					return new S_REGISTER(pdb, Stream);
				case SymbolType.S_REGREL32:
					return new S_REGREL32(pdb, Stream);
				case SymbolType.S_SECTION:
					return new S_SECTION(pdb, Stream);
				case SymbolType.S_SEPCODE:
					return new S_SEPCODE(pdb, Stream);
				case SymbolType.S_SKIP:
				case SymbolType.S_INLINESITE_END:
					return null;
				case SymbolType.S_THUNK32:
					return new S_THUNK32(pdb, Stream);
				case SymbolType.S_TRAMPOLINE:
					return new S_TRAMPOLINE(pdb, Stream);
				case SymbolType.S_COBOLUDT:
					return new S_COBOLUDT(pdb, Stream);
				case SymbolType.S_UDT:
					return new S_UDT(pdb, Stream);
				case SymbolType.S_UNAMESPACE:
					return new S_UNAMESPACE(pdb, Stream);
				case SymbolType.S_WITH32:
				case SymbolType.S_WITH32_ST:
					return new S_WITH32(pdb, Stream);
				default:
					throw new NotImplementedException($"Symbol type {symbolType} not implemented yet");
			}
		}

		public IEnumerable<Symbol> ReadSymbols() {
			var remaining = Stream.Length;

			while (remaining > 0) {
				// number of bytes that follow, including symbolType
				UInt16 length = ReadUInt16();

				SymbolType symbolType = ReadEnum<SymbolType>();

				// including symbol length
				int dataSize = length + sizeof(UInt16);
				Stream symDataStream = new MemoryStream(new byte[dataSize]);

				byte[] data = ReadBytes((int)length - sizeof(UInt16));
				OnSymbolData?.Invoke(data);

				BinaryWriter wr = new BinaryWriter(symDataStream);
				wr.Write(length);
				wr.Write((UInt16)symbolType);
				wr.Write(data);
				symDataStream.Position = 0;

				ISymbol sym = ReadSymbol(symbolType, symDataStream);
				if (sym != null) {
					yield return new Symbol(symbolType, sym);
				}

#if false
				if (symDataStream.Position != symDataStream.Length) {
					Trace.WriteLine($"WARNING: {symbolType} didn't consume {symDataStream.Length - symDataStream.Position} bytes");
				}
#endif

				remaining -= dataSize;
			}
		}
	}
}
