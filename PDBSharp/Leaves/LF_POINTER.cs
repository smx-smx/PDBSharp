#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	public class PointerAttributes
	{
		private readonly UInt32 attrs;
		public PointerAttributes(UInt32 attrs) {
			this.attrs = attrs;
		}

		public UInt32 PointerType => attrs & 0x1F;
		public UInt32 PointerMode => (attrs >> 5) & 0x7;
		public bool IsFlat32 => ((attrs >> 8) & 1) == 1;
		public bool IsVolatile => ((attrs >> 9) & 1) == 1;
		public bool IsConst => ((attrs >> 10) & 1) == 1;
		public bool IsUnaligned => ((attrs >> 11) & 1) == 1;
		public bool IsRestricted => ((attrs >> 12) & 1) == 1;
	}

	[LeafReader(LeafType.LF_POINTER)]
	public class LF_POINTER : TypeDataReader
	{
		public readonly UInt32 UnderlyingTypeIndex;
		public readonly PointerAttributes Attributes;

		public LF_POINTER(Stream stream) : base(stream) {
			UnderlyingTypeIndex = Reader.ReadUInt32();
			Attributes = new PointerAttributes(Reader.ReadUInt32());
		}
	}
}
