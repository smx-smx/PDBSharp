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

namespace Smx.PDBSharp
{
	public struct TPISlice
	{
		public UInt32 Offset;
		public UInt32 Size;
	}

	public struct TPIHash
	{
		public UInt16 HashStream;
		public UInt16 AuxHashStream;

		public UInt32 HashKeySize;
		public UInt32 NumHashBuckets;

		public TPISlice HashValues;
		public TPISlice TypeOffsets;
		public TPISlice HashHeadList;
	}

	public struct TPIHeader
	{
		public UInt32 Version;
		public UInt32 Size;

		public UInt16 MinTypeIndex;
		public UInt16 MaxTypeIndex;

		public UInt32 GpRecSize;
		public TPIHash Hash;
	}

	public class TPIReader : ReaderBase
	{
		public readonly TPIHeader Data;

		public TPIReader(Stream stream) : base(stream) {
			Data = ReadStruct<TPIHeader>();
		}
	}
}
