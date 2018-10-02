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
	public enum TrampolineType : UInt16
	{
		Incremental,
		BranchIsland
	}

	public struct TRAMPOLINESYM
	{
		public TrampolineType TrampolineType;
		public UInt16 ThunkSize;
		public UInt32 ThunkOffset;
		public UInt32 TargetOffset;
		public UInt16 ThunkSection;
		public UInt16 TargetSection;
	}

	public class TrampolineSymReader : SymbolReaderBase
	{
		public readonly TRAMPOLINESYM Data;
		public TrampolineSymReader(Stream stream) : base(stream) {
			Data = ReadStruct<TRAMPOLINESYM>();
		}
	}
}
