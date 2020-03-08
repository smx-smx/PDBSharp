#region License
/*
 * Copyright (C) 2020 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	public class LF_CLASS_STRUCTURE_INTERFACE16 : LeafBase, ILeaf
	{
		public UInt16 NumberOfElements { get; set; }
		public ILeafContainer FieldIndex { get; set; }
		public TypeProperties FieldProperties { get; set; }
		public ILeafContainer DerivedType { get; set; }
		public ILeafContainer VShapeTableType { get; set; }

		public ILeafContainer StructSize { get; set; }
		public string Name { get; set; }

		public LF_CLASS_STRUCTURE_INTERFACE16(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			NumberOfElements = r.ReadUInt16();
			FieldIndex = r.ReadIndexedType16Lazy();
			FieldProperties = r.ReadFlagsEnum<TypeProperties>();
			DerivedType = r.ReadIndexedType16Lazy();
			VShapeTableType = r.ReadIndexedType16Lazy();

			StructSize = r.ReadVaryingType(out uint dataSize);
			Name = r.ReadString16();
		}
	}
}
