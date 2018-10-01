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
	public struct DEFRANGESYM
	{
		public SymbolHeader Header;
		public UInt32 Program; //"DIA Program"
		CV_LVAR_ADDR_RANGE Range;
	}

	public struct DefRangeSymInstance
	{
		public DEFRANGESYM Header;
		public CV_LVAR_ADDR_GAP[] Gaps;
	}

	public class DefRangeSymReader : ReaderBase
	{
		public readonly DefRangeSymInstance Data;

		public DefRangeSymReader(Stream stream) : base(stream) {
			DEFRANGESYM header = ReadStruct<DEFRANGESYM>();

			// interpret remaining data as gaps
			int numGaps = (int)(Stream.Length - stream.Position) / Marshal.SizeOf<CV_LVAR_ADDR_GAP>();

			CV_LVAR_ADDR_GAP[] gaps = new CV_LVAR_ADDR_GAP[numGaps];
			for(int i=0; i<numGaps; i++) {
				gaps[i] = ReadStruct<CV_LVAR_ADDR_GAP>();
			}

			Data = new DefRangeSymInstance() {
				Header = header,
				Gaps = gaps
			};
		}
	}
}
