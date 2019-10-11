#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class LabelSym32
	{
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public CV_PROCFLAGS Flags { get; set; }
		public string Name { get; set; }
	}

	public class S_LABEL32 : ISymbol
	{
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly CV_PROCFLAGS Flags;
		public readonly string Name;

		public S_LABEL32(IServiceContainer ctx, IModule mod, SpanReader stream) {
			var r = new SymbolDataReader(ctx, stream);
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			Flags = r.ReadFlagsEnum<CV_PROCFLAGS>();
			Name = r.ReadSymbolString();
		}

		public S_LABEL32(LabelSym32 data) {
			Offset = data.Offset;
			Segment = data.Segment;
			Flags = data.Flags;
			Name = data.Name;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_LABEL32);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.WriteEnum<CV_PROCFLAGS>(Flags);
			w.WriteSymbolString(Name);

			w.WriteSymbolHeader();
		}
	}
}
