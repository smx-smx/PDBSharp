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
	[LeafReader(LeafType.LF_CLASS)]
	public class LF_CLASS : TypeDataReader
	{
		public readonly UInt16 NumberOfElements;
		public TypeProperties FieldProperties;
		public UInt32 FieldIndex;
		public UInt32 DerivedTypeIndex;
		public UInt32 VShapeTableTypeIndex;

		//public UInt16 StructSize;

		public readonly string Name;

		public LF_CLASS(Stream stream) : base(stream) {
			NumberOfElements = Reader.ReadUInt16();
			FieldProperties = (TypeProperties)Reader.ReadUInt16();
			FieldIndex = Reader.ReadUInt32();
			DerivedTypeIndex = Reader.ReadUInt32();
			VShapeTableTypeIndex = Reader.ReadUInt32();

			var StructSize = ReadVaryingType(out uint dataSize);
	
			Name = ReadCString();
		}
	}

	[LeafReader(LeafType.LF_STRUCTURE)]
	public class LF_STRUCTURE : LF_CLASS
	{
		public LF_STRUCTURE(Stream stream) : base(stream) {
		}
	}
}
