#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class CV_LVAR_ADDR_RANGE : ReaderSpan
	{
		public readonly UInt32 OffsetStart;
		public readonly UInt16 IndexSectionStart;
		public readonly UInt16 Length;

		public CV_LVAR_ADDR_RANGE(ReaderSpan stream) : base(stream) {
			OffsetStart = ReadUInt32();
			IndexSectionStart = ReadUInt16();
			Length = ReadUInt16();
		}

		public void Write(SymbolDataWriter w) {
			w.WriteUInt32(OffsetStart);
			w.WriteUInt16(IndexSectionStart);
			w.WriteUInt16(Length);
		}
	}
}
