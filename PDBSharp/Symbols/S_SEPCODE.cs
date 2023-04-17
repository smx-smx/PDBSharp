#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols.S_SEPCODE
{
	public class Data : ISymbolData {
		public UInt32 ParentSymOffset { get; set; }
		public ISymbolResolver? Parent { get; set; }
		public UInt32 End { get; set; }
		public UInt32 Size { get; set; }
		public CV_SEPCODEFLAGS Flags { get; set; }
		public UInt32 Offset { get; set; }
		public UInt32 ParentOffset { get; set; }
		public UInt16 Section { get; set; }
		public UInt16 ParentSection { get; set; }

		public Data(uint parentSymOffset, ISymbolResolver? parent, uint end, uint size, CV_SEPCODEFLAGS flags, uint offset, uint parentOffset, ushort section, ushort parentSection) {
			ParentSymOffset = parentSymOffset;
			Parent = parent;
			End = end;
			Size = size;
			Flags = flags;
			Offset = offset;
			ParentOffset = parentOffset;
			Section = section;
			ParentSection = parentSection;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		private Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();

			var ParentSymOffset = r.ReadUInt32();
			var Parent = r.ReadSymbol(Module, ParentSymOffset);
			var End = r.ReadUInt32();
			var Size = r.ReadUInt32();
			var Flags = r.ReadFlagsEnum<CV_SEPCODEFLAGS>();
			var Offset = r.ReadUInt32();
			var ParentOffset = r.ReadUInt32();
			var Section = r.ReadUInt16();
			var ParentSection = r.ReadUInt16();

			Data = new Data(
				parentSymOffset: ParentSymOffset,
				parent: Parent,
				end: End,
				size: Size,
				flags: Flags,
				offset: Offset,
				parentOffset: ParentOffset,
				section: Section,
				parentSection: ParentSection
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_SEPCODE);
			w.WriteUInt32(data.ParentSymOffset);
			w.WriteUInt32(data.End);
			w.WriteUInt32(data.Size);
			w.Write<CV_SEPCODEFLAGS>(data.Flags);
			w.WriteUInt32(data.Offset);
			w.WriteUInt32(data.ParentOffset);
			w.WriteUInt16(data.Section);
			w.WriteUInt16(data.ParentSection);

			w.WriteHeader();
		}
	}
}
