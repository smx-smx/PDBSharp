#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{
	[SymbolReader(SymbolType.S_FRAMEPROC)]
	public class S_FRAMEPROC : SymbolDataReader
	{
		public readonly UInt32 FrameSize;
		public readonly UInt32 PaddingSize;
		public readonly UInt32 PaddingOffset;
		public readonly UInt32 SavedRegistersSize;
		public readonly UInt32 ExceptionHandlerOffset;
		public readonly UInt16 ExceptionHandlerSection;
		public readonly FrameProcSymFlags Flags;

		public S_FRAMEPROC(Stream stream) : base(stream) {
			FrameSize = ReadUInt32();
			PaddingSize = ReadUInt32();
			PaddingOffset = ReadUInt32();
			SavedRegistersSize = ReadUInt32();
			ExceptionHandlerOffset = ReadUInt32();
			ExceptionHandlerSection = ReadUInt16();
			Flags = new FrameProcSymFlags(ReadUInt16());
		}
	}
}
