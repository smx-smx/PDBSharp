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

		public event OnSymbolDataDelegate? OnSymbolData;

		private readonly IModule? moduleStream = null;

		public SymbolsReader(IServiceContainer ctx, SpanStream data, IModule? moduleStream = null) : base(data) {
			this.ctx = ctx;
			this.moduleStream = moduleStream;
		}

		public SymbolsReader(IServiceContainer ctx, SpanStream data) : base(data) {
			this.ctx = ctx;
		}

		public SymbolsReader(IServiceContainer ctx, byte[] data) : base(data) {
			this.ctx = ctx;
		}


		private ISymbolSerializer? CreateSymbolSerializer(SymbolHeader hdr) {
			var subStream = SliceHere(sizeof(UInt16) + hdr.Length);
			switch (hdr.Type) { 
				case SymbolType.S_ANNOTATION:
					return new Symbols.S_ANNOTATION.Serializer(ctx, subStream);
				case SymbolType.S_BLOCK32:
					if (moduleStream == null) throw new InvalidOperationException(nameof(moduleStream));
					return new Symbols.S_BLOCK32.Serializer(ctx, subStream, moduleStream);
				case SymbolType.S_BPREL32:
				case SymbolType.S_BPREL32_ST:
					return new Symbols.S_BPREL32.Serializer(ctx, subStream);
				case SymbolType.S_BUILDINFO:
					return new Symbols.S_BUILDINFO.Serializer(ctx, subStream);
				case SymbolType.S_CALLEES:
					return new Symbols.S_CALLEES.Serializer(ctx, subStream);
				case SymbolType.S_CALLERS:
					return new Symbols.S_CALLERS.Serializer(ctx, subStream);
				case SymbolType.S_INLINEES:
					return new Symbols.S_INLINEES.Serializer(ctx, subStream);
				case SymbolType.S_CALLSITEINFO:
					return new Symbols.S_CALLSITEINFO.Serializer(ctx, subStream);
				case SymbolType.S_COFFGROUP:
					return new Symbols.S_COFFGROUP.Serializer(ctx, subStream);
				case SymbolType.S_COMPILE:
					return new Symbols.S_COMPILE.Serializer(ctx, subStream);
				case SymbolType.S_COMPILE2:
					return new Symbols.S_COMPILE2.Serializer(ctx, subStream);
				case SymbolType.S_COMPILE3:
					return new Symbols.S_COMPILE3.Serializer(ctx, subStream);
				case SymbolType.S_DEFRANGE_FRAMEPOINTER_REL:
					return new Symbols.S_DEFRANGE_FRAMEPOINTER_REL.Serializer(ctx, subStream);
				case SymbolType.S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE:
					return new Symbols.S_DEFRANGE_FRAMEPOINTER_REL_FULL_SCOPE.Serializer(ctx, subStream);
				case SymbolType.S_DEFRANGE_REGISTER:
					return new Symbols.S_DEFRANGE_REGISTER.Serializer(ctx, subStream);
				case SymbolType.S_DEFRANGE_REGISTER_REL:
					return new Symbols.S_DEFRANGE_REGISTER_REL.Serializer(ctx, subStream);
				case SymbolType.S_DEFRANGE_SUBFIELD_REGISTER:
					return new Symbols.S_DEFRANGE_SUBFIELD_REGISTER.Serializer(ctx, subStream);
				case SymbolType.S_ENVBLOCK:
					return new Symbols.S_ENVBLOCK.Serializer(ctx, subStream);
				case SymbolType.S_EXPORT:
					return new Symbols.S_EXPORT.Serializer(ctx, subStream);
				case SymbolType.S_FILESTATIC:
					return new Symbols.S_FILESTATIC.Serializer(ctx, subStream);
				case SymbolType.S_FRAMECOOKIE:
					return new Symbols.S_FRAMECOOKIE.Serializer(ctx, subStream);
				case SymbolType.S_FRAMEPROC:
					return new Symbols.S_FRAMEPROC.Serializer(ctx, subStream);
				case SymbolType.S_GDATA32:
				case SymbolType.S_GDATA32_ST:
					return new Symbols.S_GDATA32.Serializer(ctx, subStream);
				case SymbolType.S_INLINESITE:
					if (moduleStream == null) throw new InvalidOperationException(nameof(moduleStream));
					return new Symbols.S_INLINESITE.Serializer(ctx, subStream, moduleStream);
				case SymbolType.S_LDATA32:
				case SymbolType.S_LDATA32_ST:
					return new Symbols.S_LDATA32.Serializer(ctx, subStream);
				case SymbolType.S_LMANDATA:
				case SymbolType.S_LMANDATA_ST:
					return new Symbols.S_LMANDATA.Serializer(ctx, subStream);
				case SymbolType.S_GMANPROC:
					if (moduleStream == null) throw new InvalidOperationException(nameof(moduleStream));
					return new Symbols.S_GMANPROC.Serializer(ctx, subStream, moduleStream);
				case SymbolType.S_LMANPROC:
				case SymbolType.S_LMANPROC_ST:
					if (moduleStream == null) throw new InvalidOperationException(nameof(moduleStream));
					return new Symbols.S_LMANPROC.Serializer(ctx, subStream, moduleStream);
				case SymbolType.S_GPROC32:
				case SymbolType.S_GPROC32_ST:
					if (moduleStream == null) throw new InvalidOperationException(nameof(moduleStream));
					return new Symbols.S_GPROC32.Serializer(ctx, subStream, moduleStream);
				case SymbolType.S_LPROC32:
				case SymbolType.S_LPROC32_ST:
					if (moduleStream == null) throw new InvalidOperationException(nameof(moduleStream));
					return new Symbols.S_LPROC32.Serializer(ctx, subStream, moduleStream);
				case SymbolType.S_PROCREF:
				case SymbolType.S_LPROCREF:
				case SymbolType.S_DATAREF:
					return new Symbols.Structures.REFSYM2.Serializer(ctx, subStream);
				case SymbolType.S_HEAPALLOCSITE:
					return new Symbols.S_HEAPALLOCSITE.Serializer(ctx, subStream);
				case SymbolType.S_LABEL32:
				case SymbolType.S_LABEL32_ST:
					return new Symbols.S_LABEL32.Serializer(ctx, subStream);
				case SymbolType.S_LOCAL:
					return new Symbols.S_LOCAL.Serializer(ctx, subStream);
				case SymbolType.S_CONSTANT:
				case SymbolType.S_CONSTANT_ST:
					return new Symbols.S_CONSTANT.Serializer(ctx, subStream);
				case SymbolType.S_MANCONSTANT:
					return new Symbols.S_MANCONSTANT.Serializer(ctx, subStream);
				case SymbolType.S_MANSLOT:
				case SymbolType.S_MANSLOT_ST:
					return new Symbols.S_MANSLOT.Serializer(ctx, subStream);
				case SymbolType.S_OBJNAME:
				case SymbolType.S_OBJNAME_ST:
					return new Symbols.S_OBJNAME.Serializer(ctx, subStream);
				case SymbolType.S_OEM:
					return new Symbols.S_OEM.Serializer(ctx, subStream);
				case SymbolType.S_PUB32:
					return new Symbols.S_PUB32.Serializer(ctx, subStream);
				case SymbolType.S_REGISTER:
				case SymbolType.S_REGISTER_ST:
					return new Symbols.S_REGISTER.Serializer(ctx, subStream);
				case SymbolType.S_REGREL32:
					return new Symbols.S_REGREL32.Serializer(ctx, subStream);
				case SymbolType.S_SECTION:
					return new Symbols.S_SECTION.Serializer(ctx, subStream);
				case SymbolType.S_SEPCODE:
					if (moduleStream == null) throw new InvalidOperationException(nameof(moduleStream));
					return new Symbols.S_SEPCODE.Serializer(ctx, subStream, moduleStream);
				case SymbolType.S_THUNK32:
				case SymbolType.S_THUNK32_ST:
					if (moduleStream == null) throw new InvalidOperationException(nameof(moduleStream));
					return new Symbols.S_THUNK32.Serializer(ctx, subStream, moduleStream);
				case SymbolType.S_LTHREAD32:
				case SymbolType.S_GTHREAD32:
					return new Symbols.Structures.THREADSYM32.Serializer(ctx, subStream);
				case SymbolType.S_TRAMPOLINE:
					return new Symbols.S_TRAMPOLINE.Serializer(ctx, subStream);
				case SymbolType.S_COBOLUDT:
					return new Symbols.S_COBOLUDT.Serializer(ctx, subStream);
				case SymbolType.S_UDT_ST:
				case SymbolType.S_UDT:
					return new Symbols.S_UDT.Serializer(ctx, subStream);
				case SymbolType.S_UNAMESPACE:
					return new Symbols.S_UNAMESPACE.Serializer(ctx, subStream);
				case SymbolType.S_WITH32:
				case SymbolType.S_WITH32_ST:
					if (moduleStream == null) throw new InvalidOperationException(nameof(moduleStream));
					return new Symbols.S_WITH32.Serializer(ctx, subStream, moduleStream);
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

			var symbolLength = ReadUInt16();
			if(symbolLength < 1) {
				return null;
			}
			var symbolType = ReadEnum<SymbolType>();

			Position = startOffset;
			long endOffset = Position + sizeof(UInt16) + symbolLength;

			var hdr = new SymbolHeader {
				Length = symbolLength,
				Type = symbolType
			};

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
			var prevPos = Position;

			while (Position < Length) {
				ISymbolResolver? sym = ReadSymbolDirect();
				if (sym != null) {
					yield return sym;
				}
			}
			Position = prevPos;
		}
	}
}
