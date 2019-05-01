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
	[LeafReader(LeafType.LF_BITFIELD)]
	public class LF_BITFIELD : TypeDataReader
	{
		public readonly UInt32 TypeIndex;
		public readonly byte Length;
		public readonly byte Position;

		public LF_BITFIELD(Stream stream) : base(stream) {
			TypeIndex = Reader.ReadUInt32();
			Length = Reader.ReadByte();
			Position = Reader.ReadByte();
		}
	}
}
