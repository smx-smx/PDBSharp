#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_BLOCK32
{
	public class Data : ISymbolData {
		public UInt32 ParentOffset { get; set; }
		public ISymbolResolver? Parent { get; set; }
		public UInt32 End { get; set; }
		public UInt32 Length { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public string Name { get; set; }

		public Data(uint parentOffset, ISymbolResolver? parent, uint end, uint length, uint offset, ushort segment, string name) {
			ParentOffset = parentOffset;
			Parent = parent;
			End = end;
			Length = length;
			Offset = offset;
			Segment = segment;
			Name = name;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, SpanStream stream, IModule cvStream) : base(ctx, stream, cvStream) {
		}

		public void Read() {
			SymbolData.Reader r = CreateReader();
			var ParentOffset = r.ReadUInt32();
			var Parent = r.ReadSymbol(ParentOffset);

			var End = r.ReadUInt32();
			var Length = r.ReadUInt32();
			var Offset = r.ReadUInt32();
			var Segment = r.ReadUInt16();
			var Name = r.ReadSymbolString();

			Data = new Data(
				parentOffset: ParentOffset,
				parent: Parent,
				end: End,
				length: Length,
				offset: Offset,
				segment: Segment,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			SymbolDataWriter w = CreateWriter(SymbolType.S_BLOCK32);
			w.WriteUInt32(data.ParentOffset);
			w.WriteUInt32(data.End);
			w.WriteUInt32(data.Length);
			w.WriteUInt32(data.Offset);
			w.WriteUInt16(data.Segment);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}
	}
}