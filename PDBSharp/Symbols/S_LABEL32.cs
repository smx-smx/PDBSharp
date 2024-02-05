#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
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
using Smx.PDBSharp.Leaves;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_LABEL32
{
	public class Data : ISymbolData {
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public CV_PROCFLAGS Flags { get; set; }
		public string Name { get; set; }

		public Data(uint offset, ushort segment, CV_PROCFLAGS flags, string name) {
			Offset = offset;
			Segment = segment;
			Flags = flags;
			Name = name;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		private Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public void Read() {
			var r = CreateReader();
			var Offset = r.ReadUInt32();
			var Segment = r.ReadUInt16();
			var Flags = r.ReadFlagsEnum<CV_PROCFLAGS>();
			var Name = r.ReadSymbolString();
			Data = new Data(
				offset: Offset,
				segment: Segment,
				flags: Flags,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_LABEL32);
			w.WriteUInt32(data.Offset);
			w.WriteUInt16(data.Segment);
			w.Write<CV_PROCFLAGS>(data.Flags);
			w.WriteSymbolString(data.Name);
			w.WriteHeader();
		}
	}
}
