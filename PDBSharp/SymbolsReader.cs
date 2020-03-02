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


		private ISymbol CreateSymbolStream(SymbolHeader hdr) {
			switch (hdr.Type) { 
				case SymbolType.S_ANNOTATION:
					return new S_ANNOTATION(ctx, mod, this); 
				case SymbolType.S_BLOCK32:
					return new S_BLOCK32(ctx, mod, this); 
				case SymbolType.S_BPREL32:
				case SymbolType.S_BPREL32_ST:
					return new S_BPREL32(ctx, mod, this); 
				case SymbolType.S_BUILDINFO:
					return new S_BUILDINFO(ctx, mod, this); 
				case SymbolType.S_CALLEES:
					return new S_CALLEES(ctx, mod, this); 
				case SymbolType.S_CALLERS:
					return new S_CALLERS(ctx, mod, this); 
				case SymbolType.S_INLINEES:
					return new S_INLINEES(ctx, mod, this); 
				case SymbolType.S_CALLSITEINFO:
					return new S_CALLSITEINFO(ctx, mod, this); 
				case SymbolType.S_COFFGROUP:
					return new S_COFFGROUP(ctx, mod, this); 
				case SymbolType.S_COMPILE:
					return new S_COMPILE(ctx, mod, this); 
				case SymbolType.S_COMPILE2:
					return new S_COMPILE2(ctx, mod, this); 
				case SymbolType.S_COMPILE3:
					return new S_COMPILE3(ctx, mod, this); 
				case SymbolType.S_DEFRANGE_FRAMEPOINTER_REL:
					return new S_DEFRANGE_FRAMEPOINTER_REL(ctx, mod, this); 
				case SymbolType.S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE:
					return new S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE(ctx, mod, this); 
				case SymbolType.S_DEFRANGE_REGISTER:
					return new S_DEFRANGE_REGISTER(ctx, mod, this); 
				case SymbolType.S_DEFRANGE_REGISTER_REL:
					return new S_DEFRANGE_REGISTER_REL(ctx, mod, this); 
				case SymbolType.S_DEFRANGE_SUBFIELD_REGISTER:
					return new S_DEFRANGE_SUBFIELD_REGISTER(ctx, mod, this); 
				case SymbolType.S_ENVBLOCK:
					return new S_ENVBLOCK(ctx, mod, this); 
				case SymbolType.S_EXPORT:
					return new S_EXPORT(ctx, mod, this); 
				case SymbolType.S_FILESTATIC:
					return new S_FILESTATIC(ctx, mod, this); 
				case SymbolType.S_FRAMECOOKIE:
					return new S_FRAMECOOKIE(ctx, mod, this); 
				case SymbolType.S_FRAMEPROC:
					return new S_FRAMEPROC(ctx, mod, this); 
				case SymbolType.S_GDATA32:
					return new S_GDATA32(ctx, mod, this); 
				case SymbolType.S_INLINESITE:
					return new S_INLINESITE(ctx, mod, this); 
				case SymbolType.S_LDATA32:
				case SymbolType.S_LDATA32_ST:
					return new S_LDATA32(ctx, mod, this); 
				case SymbolType.S_LMANDATA:
					return new S_LMANDATA(ctx, mod, this); 
				case SymbolType.S_GMANPROC:
					return new S_GMANPROC(ctx, mod, this); 
				case SymbolType.S_LMANPROC:
					return new S_LMANPROC(ctx, mod, this); 
				case SymbolType.S_GPROC32:
				case SymbolType.S_GPROC32_ST:
					return new S_GPROC32(ctx, mod, this); 
				case SymbolType.S_LPROC32:
				case SymbolType.S_LPROC32_ST:
					return new S_LPROC32(ctx, mod, this); 
				case SymbolType.S_HEAPALLOCSITE:
					return new S_HEAPALLOCSITE(ctx, mod, this); 
				case SymbolType.S_LABEL32:
				case SymbolType.S_LABEL32_ST:
					return new S_LABEL32(ctx, mod, this); 
				case SymbolType.S_LOCAL:
					return new S_LOCAL(ctx, mod, this);
				case SymbolType.S_CONSTANT_ST:
				case SymbolType.S_CONSTANT:
					return new S_CONSTANT(ctx, mod, this); 
				case SymbolType.S_MANCONSTANT:
					return new S_MANCONSTANT(ctx, mod, this); 
				case SymbolType.S_MANSLOT:
					return new S_MANSLOT(ctx, mod, this); 
				case SymbolType.S_OBJNAME:
				case SymbolType.S_OBJNAME_ST:
					return new S_OBJNAME(ctx, mod, this); 
				case SymbolType.S_OEM:
					return new S_OEM(ctx, mod, this); 
				case SymbolType.S_REGISTER:
				case SymbolType.S_REGISTER_ST:
					return new S_REGISTER(ctx, mod, this); 
				case SymbolType.S_REGREL32:
					return new S_REGREL32(ctx, mod, this); 
				case SymbolType.S_SECTION:
					return new S_SECTION(ctx, mod, this); 
				case SymbolType.S_SEPCODE:
					return new S_SEPCODE(ctx, mod, this);
				case SymbolType.S_THUNK32:
				case SymbolType.S_THUNK32_ST:
					return new S_THUNK32(ctx, mod, this); 
				case SymbolType.S_TRAMPOLINE:
					return new S_TRAMPOLINE(ctx, mod, this); 
				case SymbolType.S_COBOLUDT:
					return new S_COBOLUDT(ctx, mod, this);
				case SymbolType.S_UDT_ST:
				case SymbolType.S_UDT:
					return new S_UDT(ctx, mod, this); 
				case SymbolType.S_UNAMESPACE:
					return new S_UNAMESPACE(ctx, mod, this); 
				case SymbolType.S_WITH32:
				case SymbolType.S_WITH32_ST:
					return new S_WITH32(ctx, mod, this);
				case SymbolType.S_END:
				case SymbolType.S_SKIP:
				case SymbolType.S_INLINESITE_END:
					return null;
				default:
					string typeName = Enum.GetName(typeof(SymbolType), hdr.Type);
					throw new NotImplementedException($"Symbol type {typeName} (0x{hdr.Type:X}) not implemented yet");
			}
		}

		public Symbol ReadSymbol() {
			long startOffset = Position;

			SymbolHeader hdr = Read<SymbolHeader>();
			Position = startOffset;

			long endOffset = Position + sizeof(UInt16) + hdr.Length;


			ISymbol sym = CreateSymbolStream(hdr);
			if(sym != null) {
				sym.Read();
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
