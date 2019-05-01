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

	[LeafReader(LeafType.LF_ENUM)]
	public class LF_ENUM : TypeDataReader
	{
		public readonly UInt16 NumElements;
		public readonly TypeProperties Property;

		public readonly UInt32 UnderlyingTypeIndex;
		public readonly UInt32 FieldTypeIndex;

		public readonly string FieldName;

		public LF_ENUM(Stream stream) : base(stream) {
			NumElements = Reader.ReadUInt16();
			Property = (TypeProperties)Reader.ReadUInt16();
			UnderlyingTypeIndex = Reader.ReadUInt32();
			FieldTypeIndex = Reader.ReadUInt32();
			FieldName = ReadCString();

		}
	}
}
