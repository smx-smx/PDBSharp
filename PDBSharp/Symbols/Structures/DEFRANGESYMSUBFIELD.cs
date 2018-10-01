#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols.Structures
{
	public struct DEFRANGESYMSUBFIELD
	{
		public SymbolHeader Header;
		public UInt32 Program;
		public UInt32 OffsetParent;
		public CV_LVAR_ADDR_RANGE Range;
	}

	public struct DefRangeSymSubFieldInstance
	{
		public DEFRANGESYMSUBFIELD Header;
		public CV_LVAR_ADDR_GAP[] Gaps;
	}

	public class DefRangeSymSubFieldReader : ReaderBase
	{
		public readonly DefRangeSymSubFieldInstance Data;
		public DefRangeSymSubFieldReader(Stream stream) : base(stream) {
			DEFRANGESYMSUBFIELD header = ReadStruct<DEFRANGESYMSUBFIELD>();

			int numGaps = (int)(Stream.Length - Stream.Position) / Marshal.SizeOf<DEFRANGESYMSUBFIELD>();
			CV_LVAR_ADDR_GAP[] gaps = new CV_LVAR_ADDR_GAP[numGaps];
			for(int i=0; i<numGaps; i++) {
				gaps[i] = ReadStruct<CV_LVAR_ADDR_GAP>();
			}

			Data = new DefRangeSymSubFieldInstance() {
				Header = header,
				Gaps = gaps
			};
		}
	}
}
