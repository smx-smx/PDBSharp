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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{

	public class DefrangeSymRegister
	{
		public UInt16 Register { get; set; }
		public RangeAttributes Attributes { get; set; }
		public CV_LVAR_ADDR_RANGE Range { get; set; }
		public CV_LVAR_ADDR_GAP[] Gaps { get; set; }
	}

	public class S_DEFRANGE_REGISTER : ISymbol
	{
		public readonly UInt16 Register;
		public readonly RangeAttributes Attributes;
		public readonly CV_LVAR_ADDR_RANGE Range;
		public readonly CV_LVAR_ADDR_GAP[] Gaps;

		public S_DEFRANGE_REGISTER(Context ctx, IModule mod, Stream stream) {
			var r = new SymbolDataReader(ctx, stream);
			Register = r.ReadUInt16();
			Attributes = r.ReadFlagsEnum<RangeAttributes>();
			Range = new CV_LVAR_ADDR_RANGE(stream);
			Gaps = CV_LVAR_ADDR_GAP.ReadGaps(r);
		}

		public S_DEFRANGE_REGISTER(DefrangeSymRegister data) {
			Register = data.Register;
			Attributes = data.Attributes;
			Range = data.Range;
			Gaps = data.Gaps;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_DEFRANGE_REGISTER);
			w.WriteUInt16(Register);
			w.WriteEnum<RangeAttributes>(Attributes);
			Range.Write(w);
			foreach(CV_LVAR_ADDR_GAP gap in Gaps) {
				gap.Write(w);
			}

			w.WriteSymbolHeader();
		}
	}
}
