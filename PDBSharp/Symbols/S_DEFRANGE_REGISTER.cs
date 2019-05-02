#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
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

	[SymbolReader(SymbolType.S_DEFRANGE_REGISTER)]
	public class S_DEFRANGE_REGISTER : SymbolDataReader
	{
		public readonly UInt16 Register;
		public readonly RangeAttributes Attributes;
		public readonly CV_LVAR_ADDR_RANGE Range;
		public readonly CV_LVAR_ADDR_GAP[] Gaps;

		public S_DEFRANGE_REGISTER(Stream stream) : base(stream) {
			Register = ReadUInt16();
			Attributes = ReadEnum<RangeAttributes>();
			Range = new CV_LVAR_ADDR_RANGE(stream);
			Gaps = CV_LVAR_ADDR_GAP.ReadGaps(stream);
		}
	}
}
