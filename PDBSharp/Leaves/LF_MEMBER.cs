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
	[LeafReader(LeafType.LF_MEMBER)]
	public class LF_MEMBER : TypeDataReader
	{
		public readonly FieldAttributes Attributes;
		public readonly UInt32 FieldTypeIndex;

		public readonly string Name;

		public LF_MEMBER(Stream stream) : base(stream) {
			Attributes = new FieldAttributes(ReadUInt16());
			FieldTypeIndex = ReadUInt32();

			var varLength = ReadVaryingType(out uint dataSize);

			Name = ReadCString();
		}
	}
}
