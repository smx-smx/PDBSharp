#region License
/*
 * Copyright (C) 2020 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_CLASS_STRUCTURE_INTERFACE16
{
	public class Data : ILeafData {
		public UInt16 NumberOfElements { get; set; }
		public ILeafResolver? FieldIndex { get; set; }
		public TypeProperties FieldProperties { get; set; }
		public ILeafResolver? DerivedType { get; set; }
		public ILeafResolver? VShapeTableType { get; set; }

		public ILeafResolver? StructSize { get; set; }
		public string Name { get; set; }

		public Data(ushort numberOfElements, ILeafResolver? fieldIndex, TypeProperties fieldProperties, ILeafResolver? derivedType, ILeafResolver? vShapeTableType, ILeafResolver? structSize, string name) {
			NumberOfElements = numberOfElements;
			FieldIndex = fieldIndex;
			FieldProperties = fieldProperties;
			DerivedType = derivedType;
			VShapeTableType = vShapeTableType;
			StructSize = structSize;
			Name = name;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data {  get; set; }

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public ILeafData? GetData() => Data;

		public void Read() {
			TypeDataReader r = CreateReader();

			var NumberOfElements = r.ReadUInt16();
			var FieldIndex = r.ReadIndexedType16Lazy();
			var FieldProperties = r.ReadFlagsEnum<TypeProperties>();
			var DerivedType = r.ReadIndexedType16Lazy();
			var VShapeTableType = r.ReadIndexedType16Lazy();

			var StructSize = r.ReadVaryingType(out uint dataSize);
			var Name = r.ReadString16();

			Data = new Data(
				numberOfElements: NumberOfElements,
				fieldIndex: FieldIndex,
				fieldProperties: FieldProperties,
				derivedType: DerivedType,
				vShapeTableType: VShapeTableType,
				structSize: StructSize,
				name: Name
			);
		}

		public void Write() {
			throw new NotImplementedException();
		}
	}
}
