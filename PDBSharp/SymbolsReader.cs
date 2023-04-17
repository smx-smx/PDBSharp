#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
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

		public event OnSymbolDataDelegate? OnSymbolData;

		public SymbolsReader(IServiceContainer ctx, IModule mod, SpanStream data) : base(data) {
			this.ctx = ctx;
			this.mod = mod;
		}

		public SymbolsReader(IServiceContainer ctx, IModule mod, byte[] data) : base(data) {
			this.ctx = ctx;
			this.mod = mod;
		}


		private ISymbolSerializer? CreateSymbolSerializer(SymbolHeader hdr) {
			switch (hdr.Type) { 
				case SymbolType.S_ANNOTATION:
					return new Symbols.S_ANNOTATION.Serializer(ctx, mod, this); 
				case SymbolType.S_BLOCK32:
					return new Symbols.S_BLOCK32.Serializer(ctx, mod, this); 
				case SymbolType.S_BPREL32:
				case SymbolType.S_BPREL32_ST:
					return new Symbols.S_BPREL32.Serializer(ctx, mod, this); 
				case SymbolType.S_BUILDINFO:
					return new Symbols.S_BUILDINFO.Serializer(ctx, mod, this); 
				case SymbolType.S_CALLEES:
					return new Symbols.S_CALLEES.Serializer(ctx, mod, this); 
				case SymbolType.S_CALLERS:
					return new Symbols.S_CALLERS.Serializer(ctx, mod, this); 
				case SymbolType.S_INLINEES:
					return new Symbols.S_INLINEES.Serializer(ctx, mod, this); 
				case SymbolType.S_CALLSITEINFO:
					return new Symbols.S_CALLSITEINFO.Serializer(ctx, mod, this); 
				case SymbolType.S_COFFGROUP:
					return new Symbols.S_COFFGROUP.Serializer(ctx, mod, this); 
				case SymbolType.S_COMPILE:
					return new Symbols.S_COMPILE.Serializer(ctx, mod, this); 
				case SymbolType.S_COMPILE2:
					return new Symbols.S_COMPILE2.Serializer(ctx, mod, this); 
				case SymbolType.S_COMPILE3:
					return new Symbols.S_COMPILE3.Serializer(ctx, mod, this); 
				case SymbolType.S_DEFRANGE_FRAMEPOINTER_REL:
					return new Symbols.S_DEFRANGE_FRAMEPOINTER_REL.Serializer(ctx, mod, this); 
				case SymbolType.S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE:
					return new Symbols.S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE.Serializer(ctx, mod, this); 
				case SymbolType.S_DEFRANGE_REGISTER:
					return new Symbols.S_DEFRANGE_REGISTER.Serializer(ctx, mod, this); 
				case SymbolType.S_DEFRANGE_REGISTER_REL:
					return new Symbols.S_DEFRANGE_REGISTER_REL.Serializer(ctx, mod, this); 
				case SymbolType.S_DEFRANGE_SUBFIELD_REGISTER:
					return new Symbols.S_DEFRANGE_SUBFIELD_REGISTER.Serializer(ctx, mod, this); 
				case SymbolType.S_ENVBLOCK:
					return new Symbols.S_ENVBLOCK.Serializer(ctx, mod, this); 
				case SymbolType.S_EXPORT:
					return new Symbols.S_EXPORT.Serializer(ctx, mod, this); 
				case SymbolType.S_FILESTATIC:
					return new Symbols.S_FILESTATIC.Serializer(ctx, mod, this); 
				case SymbolType.S_FRAMECOOKIE:
					return new Symbols.S_FRAMECOOKIE.Serializer(ctx, mod, this); 
				case SymbolType.S_FRAMEPROC:
					return new Symbols.S_FRAMEPROC.Serializer(ctx, mod, this); 
				case SymbolType.S_GDATA32:
				case SymbolType.S_GDATA32_ST:
					return new Symbols.S_GDATA32.Serializer(ctx, mod, this); 
				case SymbolType.S_INLINESITE:
					return new Symbols.S_INLINESITE.Serializer(ctx, mod, this); 
				case SymbolType.S_LDATA32:
				case SymbolType.S_LDATA32_ST:
					return new Symbols.S_LDATA32.Serializer(ctx, mod, this); 
				case SymbolType.S_LMANDATA:
				case SymbolType.S_LMANDATA_ST:
					return new Symbols.S_LMANDATA.Serializer(ctx, mod, this); 
				case SymbolType.S_GMANPROC:
					return new Symbols.S_GMANPROC.Serializer(ctx, mod, this); 
				case SymbolType.S_LMANPROC:
				case SymbolType.S_LMANPROC_ST:
					return new Symbols.S_LMANPROC.Serializer(ctx, mod, this); 
				case SymbolType.S_GPROC32:
				case SymbolType.S_GPROC32_ST:
					return new Symbols.S_GPROC32.Serializer(ctx, mod, this); 
				case SymbolType.S_LPROC32:
				case SymbolType.S_LPROC32_ST:
					return new Symbols.S_LPROC32.Serializer(ctx, mod, this); 
				case SymbolType.S_HEAPALLOCSITE:
					return new Symbols.S_HEAPALLOCSITE.Serializer(ctx, mod, this); 
				case SymbolType.S_LABEL32:
				case SymbolType.S_LABEL32_ST:
					return new Symbols.S_LABEL32.Serializer(ctx, mod, this); 
				case SymbolType.S_LOCAL:
					return new Symbols.S_LOCAL.Serializer(ctx, mod, this);
				case SymbolType.S_CONSTANT:
				case SymbolType.S_CONSTANT_ST:
					return new Symbols.S_CONSTANT.Serializer(ctx, mod, this); 
				case SymbolType.S_MANCONSTANT:
					return new Symbols.S_MANCONSTANT.Serializer(ctx, mod, this); 
				case SymbolType.S_MANSLOT:
				case SymbolType.S_MANSLOT_ST:
					return new Symbols.S_MANSLOT.Serializer(ctx, mod, this); 
				case SymbolType.S_OBJNAME:
				case SymbolType.S_OBJNAME_ST:
					return new Symbols.S_OBJNAME.Serializer(ctx, mod, this); 
				case SymbolType.S_OEM:
					return new Symbols.S_OEM.Serializer(ctx, mod, this); 
				case SymbolType.S_REGISTER:
				case SymbolType.S_REGISTER_ST:
					return new Symbols.S_REGISTER.Serializer(ctx, mod, this); 
				case SymbolType.S_REGREL32:
					return new Symbols.S_REGREL32.Serializer(ctx, mod, this); 
				case SymbolType.S_SECTION:
					return new Symbols.S_SECTION.Serializer(ctx, mod, this); 
				case SymbolType.S_SEPCODE:
					return new Symbols.S_SEPCODE.Serializer(ctx, mod, this);
				case SymbolType.S_THUNK32:
				case SymbolType.S_THUNK32_ST:
					return new Symbols.S_THUNK32.Serializer(ctx, mod, this); 
				case SymbolType.S_TRAMPOLINE:
					return new Symbols.S_TRAMPOLINE.Serializer(ctx, mod, this); 
				case SymbolType.S_COBOLUDT:
					return new Symbols.S_COBOLUDT.Serializer(ctx, mod, this);
				case SymbolType.S_UDT_ST:
				case SymbolType.S_UDT:
					return new Symbols.S_UDT.Serializer(ctx, mod, this); 
				case SymbolType.S_UNAMESPACE:
					return new Symbols.S_UNAMESPACE.Serializer(ctx, mod, this); 
				case SymbolType.S_WITH32:
				case SymbolType.S_WITH32_ST:
					return new Symbols.S_WITH32.Serializer(ctx, mod, this);
				case SymbolType.S_END:
				case SymbolType.S_SKIP:
				case SymbolType.S_INLINESITE_END:
					return null;
				default:
					string typeName = Enum.GetName(typeof(SymbolType), hdr.Type);
					throw new NotImplementedException($"Symbol type {typeName} (0x{hdr.Type:X}) not implemented yet");
			}
		}

		public ISymbolResolver ReadSymbolLazy() {
			long startOffset = Position;
			
			ILazy<ISymbolResolver?> delayedSym = LazyFactory.CreateLazy<ISymbolResolver?>(() => {
				Position = startOffset;
				return ReadSymbolDirect();
			});

			return new LazySymbolData(delayedSym);

		}
		
		public ISymbolResolver? ReadSymbolDirect() {
			long startOffset = Position;

			SymbolHeader hdr = Read<SymbolHeader>();
			Position = startOffset;

			long endOffset = Position + sizeof(UInt16) + hdr.Length;


			ISymbolSerializer? sym = CreateSymbolSerializer(hdr);
			if(sym != null) {
				sym.Read();
			}
			Position = endOffset;

			if (sym == null)
				return null;

			return new DirectSymbolData(new SymbolContext(
					type: hdr.Type,
					data: sym.GetData()
			));
		}

		public IEnumerable<ISymbolResolver> ReadSymbols() {
			while (Position < Length) {
				ISymbolResolver? sym = ReadSymbolDirect();
				if (sym != null) {
					yield return sym;
				}
			}
		}
	}
}
