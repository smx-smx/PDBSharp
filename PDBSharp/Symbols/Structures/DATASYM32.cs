#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class DataSym32 : SymbolDataReader
	{
		public readonly UInt32 TypeIndex;
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly string Name;

		public DataSym32(Stream stream) : base(stream) {
			TypeIndex = ReadUInt32();
			Offset = ReadUInt32();
			Segment = ReadUInt16();
			Name = ReadSymbolString();
		}
	}
}
