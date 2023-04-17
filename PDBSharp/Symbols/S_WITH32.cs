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

namespace Smx.PDBSharp.Symbols.S_WITH32
{
	public class Data : ISymbolData {
		public UInt32 ParentOffset { get; set; }
		public ISymbolResolver? Parent { get; set; }
		public UInt32 EndOffset { get; set; }
		public UInt32 Length { get; set; }
		public UInt32 SegmentOffset { get; set; }
		public UInt16 Segment { get; set; }
		public string Expression { get; set; }

		public Data(uint parentOffset, ISymbolResolver? parent, uint endOffset, uint length, uint segmentOffset, ushort segment, string expression) {
			ParentOffset = parentOffset;
			Parent = parent;
			EndOffset = endOffset;
			Length = length;
			SegmentOffset = segmentOffset;
			Segment = segment;
			Expression = expression;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		private Data? Data;

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();
			var ParentOffset = r.ReadUInt32();
			var Parent = r.ReadSymbol(Module, ParentOffset);
			var EndOffset = r.ReadUInt32();
			var Length = r.ReadUInt32();
			var SegmentOffset = r.ReadUInt32();
			var Segment = r.ReadUInt16();
			var Expression = r.ReadSymbolString();

			Data = new Data(
				parentOffset: ParentOffset,
				parent: Parent,
				endOffset: EndOffset,
				length: Length,
				segmentOffset: SegmentOffset,
				segment: Segment,
				expression: Expression
			);
		}

		public void Write() {
			SymbolDataWriter w = CreateWriter(SymbolType.S_WITH32);
			w.WriteHeader();
			throw new NotImplementedException();
		}

		public ISymbolData? GetData() => Data;
	}
}
