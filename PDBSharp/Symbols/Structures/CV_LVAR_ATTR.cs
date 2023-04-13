#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class CV_LVAR_ATTR : SpanStream
	{
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly CV_LVARFLAGS Flags;

		public CV_LVAR_ATTR(SpanStream stream) : base(stream) {
			Offset = ReadUInt32();
			Segment = ReadUInt16();
			Flags = ReadFlagsEnum<CV_LVARFLAGS>();
		}

		public void Write(SymbolDataWriter w) {
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.Write<CV_LVARFLAGS>(Flags);
		}
	}
}
