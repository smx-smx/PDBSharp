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
	[LeafReader(LeafType.LF_METHOD)]
	public class LF_METHOD : TypeDataReader
	{
		public readonly UInt16 NumberOfOccurrences;
		public readonly UInt32 MethodListRecordIndex;

		public readonly string Name;

		public LF_METHOD(Stream stream) : base(stream) {
			NumberOfOccurrences = ReadUInt16();
			MethodListRecordIndex = ReadUInt32();
			Name = ReadCString();
		}
	}
}
