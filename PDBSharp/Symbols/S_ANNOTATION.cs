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
using System.Linq;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_ANNOTATION
{
	public class Data : ISymbolData {
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public UInt16 NumberOfStrings { get; set; }
		public string[] Annotations { get; set; }

		public Data(uint offset, ushort segment, ushort numberOfStrings, string[] annotations) {
			Offset = offset;
			Segment = segment;
			NumberOfStrings = numberOfStrings;
			Annotations = annotations;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public void Write() {
			throw new NotImplementedException();
		}

		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			var r = CreateReader();
			var Offset = r.ReadUInt32();
			var Segment = r.ReadUInt16();
			var NumberOfStrings = r.ReadUInt16();

			var Annotations = Enumerable.Range(1, NumberOfStrings)
				.Select(_ => r.ReadCString())
				.ToArray();

			Data = new Data(
				offset: Offset,
				segment: Segment,
				numberOfStrings: NumberOfStrings,
				annotations: Annotations
			);
		}
	}
}
