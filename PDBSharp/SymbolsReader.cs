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
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp
{
	public delegate void OnSymbolDataDelegate(byte[] data);

	public class SymbolsReader : SpanStream
	{
		private readonly IServiceContainer ctx;
		private readonly IModule mod;

		public event OnSymbolDataDelegate OnSymbolData;

		public SymbolsReader(IServiceContainer ctx, IModule mod, SpanStream data) : base(data) {
			this.ctx = ctx;
			this.mod = mod;
		}

		public SymbolsReader(IServiceContainer ctx, IModule mod, byte[] data) : base(data) {
			this.ctx = ctx;
			this.mod = mod;
		}

		public Symbol ReadSymbol() {
			long startOffset = Position;

			SymbolHeader hdr = Read<SymbolHeader>();
			Position = startOffset;

			long endOffset = Position + sizeof(UInt16) + hdr.Length;


			ISymbol sym;
			switch (hdr.Type) {
				case SymbolType.S_ANNOTATION:
					sym = new S_ANNOTATION(ctx, mod, this); break;
				case SymbolType.S_BLOCK32:
					sym = new S_BLOCK32(ctx, mod, this); break;
				case SymbolType.S_BPREL32:
					sym = new S_BPREL32(ctx, mod, this); break;
				case SymbolType.S_BUILDINFO:
					sym = new S_BUILDINFO(ctx, mod, this); break;
				case SymbolType.S_CALLEES:
					sym = new S_CALLEES(ctx, mod, this); break;
				case SymbolType.S_CALLERS:
					sym = new S_CALLERS(ctx, mod, this); break;
				case SymbolType.S_INLINEES:
					sym = new S_INLINEES(ctx, mod, this); break;
				case SymbolType.S_CALLSITEINFO:
					sym = new S_CALLSITEINFO(ctx, mod, this); break;
				case SymbolType.S_COFFGROUP:
					sym = new S_COFFGROUP(ctx, mod, this); break;
				case SymbolType.S_COMPILE:
					sym = new S_COMPILE(ctx, mod, this); break;
				case SymbolType.S_COMPILE2:
					sym = new S_COMPILE2(ctx, mod, this); break;
				case SymbolType.S_COMPILE3:
					sym = new S_COMPILE3(ctx, mod, this); break;
				case SymbolType.S_DEFRANGE_FRAMEPOINTER_REL:
					sym = new S_DEFRANGE_FRAMEPOINTER_REL(ctx, mod, this); break;
				case SymbolType.S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE:
					sym = new S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE(ctx, mod, this); break;
				case SymbolType.S_DEFRANGE_REGISTER:
					sym = new S_DEFRANGE_REGISTER(ctx, mod, this); break;
				case SymbolType.S_DEFRANGE_REGISTER_REL:
					sym = new S_DEFRANGE_REGISTER_REL(ctx, mod, this); break;
				case SymbolType.S_DEFRANGE_SUBFIELD_REGISTER:
					sym = new S_DEFRANGE_SUBFIELD_REGISTER(ctx, mod, this); break;
				case SymbolType.S_ENVBLOCK:
					sym = new S_ENVBLOCK(ctx, mod, this); break;
				case SymbolType.S_EXPORT:
					sym = new S_EXPORT(ctx, mod, this); break;
				case SymbolType.S_FILESTATIC:
					sym = new S_FILESTATIC(ctx, mod, this); break;
				case SymbolType.S_FRAMECOOKIE:
					sym = new S_FRAMECOOKIE(ctx, mod, this); break;
				case SymbolType.S_FRAMEPROC:
					sym = new S_FRAMEPROC(ctx, mod, this); break;
				case SymbolType.S_GDATA32:
					sym = new S_GDATA32(ctx, mod, this); break;
				case SymbolType.S_INLINESITE:
					sym = new S_INLINESITE(ctx, mod, this); break;
				case SymbolType.S_LDATA32:
					sym = new S_LDATA32(ctx, mod, this); break;
				case SymbolType.S_LMANDATA:
					sym = new S_LMANDATA(ctx, mod, this); break;
				case SymbolType.S_GMANPROC:
					sym = new S_GMANPROC(ctx, mod, this); break;
				case SymbolType.S_LMANPROC:
					sym = new S_LMANPROC(ctx, mod, this); break;
				case SymbolType.S_GPROC32:
					sym = new S_GPROC32(ctx, mod, this); break;
				case SymbolType.S_LPROC32:
					sym = new S_LPROC32(ctx, mod, this); break;
				case SymbolType.S_HEAPALLOCSITE:
					sym = new S_HEAPALLOCSITE(ctx, mod, this); break;
				case SymbolType.S_LABEL32:
					sym = new S_LABEL32(ctx, mod, this); break;
				case SymbolType.S_LOCAL:
					sym = new S_LOCAL(ctx, mod, this); break;
				case SymbolType.S_CONSTANT:
					sym = new S_CONSTANT(ctx, mod, this); break;
				case SymbolType.S_MANCONSTANT:
					sym = new S_MANCONSTANT(ctx, mod, this); break;
				case SymbolType.S_MANSLOT:
					sym = new S_MANSLOT(ctx, mod, this); break;
				case SymbolType.S_OBJNAME:
					sym = new S_OBJNAME(ctx, mod, this); break;
				case SymbolType.S_OEM:
					sym = new S_OEM(ctx, mod, this); break;
				case SymbolType.S_REGISTER:
					sym = new S_REGISTER(ctx, mod, this); break;
				case SymbolType.S_REGREL32:
					sym = new S_REGREL32(ctx, mod, this); break;
				case SymbolType.S_SECTION:
					sym = new S_SECTION(ctx, mod, this); break;
				case SymbolType.S_SEPCODE:
					sym = new S_SEPCODE(ctx, mod, this); break;
				case SymbolType.S_THUNK32:
					sym = new S_THUNK32(ctx, mod, this); break;
				case SymbolType.S_TRAMPOLINE:
					sym = new S_TRAMPOLINE(ctx, mod, this); break;
				case SymbolType.S_COBOLUDT:
					sym = new S_COBOLUDT(ctx, mod, this); break;
				case SymbolType.S_UDT:
					sym = new S_UDT(ctx, mod, this); break;
				case SymbolType.S_UNAMESPACE:
					sym = new S_UNAMESPACE(ctx, mod, this); break;
				case SymbolType.S_WITH32:
				case SymbolType.S_WITH32_ST:
					sym = new S_WITH32(ctx, mod, this); break;
				case SymbolType.S_END:
				case SymbolType.S_SKIP:
				case SymbolType.S_INLINESITE_END:
					sym = null;
					break;
				default:
					string typeName = Enum.GetName(typeof(SymbolType), hdr.Type);
					throw new NotImplementedException($"Symbol type {typeName} (0x{hdr.Type:X}) not implemented yet");
			}

			Position = endOffset;

			if (sym == null)
				return null;

			return new Symbol(hdr.Type, sym);
		}

		public IEnumerable<Symbol> ReadSymbols() {
			while (Position < Length) {
				Symbol sym = ReadSymbol();
				if (sym != null) {
					yield return sym;
				}
			}
		}
	}
}
