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
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols.Structures
{
	public struct CALLSITEINFO
	{
		public SymbolHeader Heaedr;
		public UInt32 Offset;
		public UInt16 SectionIndex;
		private UInt16 _pad_0;
		public UInt32 TypeIndex;
	}

	public class CallSiteInfoReader : ReaderBase
	{
		public readonly CALLSITEINFO Data;
		public CallSiteInfoReader(Stream stream) : base(stream) {
			Data = ReadStruct<CALLSITEINFO>();
		}
	}
}
