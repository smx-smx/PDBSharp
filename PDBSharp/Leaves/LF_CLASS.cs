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
		public ILeaf DerivedType;
		public ILeaf VShapeTableType;

		//public UInt16 StructSize;

		public readonly string Name;

		public LF_CLASS(PDBFile pdb, Stream stream) : base(pdb, stream) {
			NumberOfElements = ReadUInt16();
			FieldProperties = ReadFlagsEnum<TypeProperties>();
			FieldIndex = ReadUInt32();
			DerivedType = ReadIndexedTypeLazy();
			VShapeTableType = ReadIndexedTypeLazy();

			var StructSize = ReadVaryingType(out uint dataSize);
	
			Name = ReadCString();
		}
	}
}
