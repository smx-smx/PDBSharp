#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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
	public class LF_CLASS : ILeaf
	{
		public readonly UInt16 NumberOfElements;
		public readonly TypeProperties FieldProperties;
		public readonly UInt32 FieldIndex;
		public readonly ILeafContainer DerivedType;
		public readonly ILeafContainer VShapeTableType;

		//public UInt16 StructSize;

		public readonly ILeafContainer StructSize;

		public readonly string Name;

		public LF_CLASS(Context pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			NumberOfElements = r.ReadUInt16();
			FieldProperties = r.ReadFlagsEnum<TypeProperties>();
			FieldIndex = r.ReadUInt32();
			DerivedType = r.ReadIndexedTypeLazy();
			VShapeTableType = r.ReadIndexedTypeLazy();

			StructSize = r.ReadVaryingType(out uint dataSize);
	
			Name = r.ReadCString();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_CLASS);
			w.WriteUInt16(NumberOfElements);
			w.WriteEnum<TypeProperties>(FieldProperties);
			w.WriteUInt32(FieldIndex);
			w.WriteIndexedType(VShapeTableType);
			w.WriteVaryingType(StructSize);
			w.WriteCString(Name);
			w.WriteLeafHeader();
		}

		public override string ToString() {
			return $"LF_CLASS[NumberOfElements='{NumberOfElements}', " +
				$"FieldProperties='{FieldProperties}', " +
				$"FieldIndex='{FieldIndex}', " +
				$"VShapeTableType='{VShapeTableType}', " +
				$"StructSize='{StructSize}', " +
				$"Name='{Name}']";
		}
	}
}
