#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_CLASS_STRUCTURE_INTERFACE : LeafBase, ILeaf
	{
		public UInt16 NumberOfElements { get; set; }
		public TypeProperties FieldProperties { get; set; }
		public ILeafContainer FieldIndex { get; set; }
		public ILeafContainer DerivedType { get; set; }
		public ILeafContainer VShapeTableType { get; set; }

		//public UInt16 StructSize;

		public ILeafContainer StructSize { get; set; }

		public string Name { get; set; }

		public override string UdtName => Name;

		public override bool IsDefnUdt {
			get {
				return FieldProperties.HasFlag(TypeProperties.IsForwardReference);
			}
		}

		public override bool IsGlobalDefnUdt {
			get {
				return (
					FieldProperties.HasFlag(TypeProperties.IsForwardReference) &&
					FieldProperties.HasFlag(TypeProperties.IsScoped) &&
					!IsUdtAnon
				);
			}
		}

		public override bool IsLocalDefnUdtWithUniqueName {
			get {
				return (
					!FieldProperties.HasFlag(TypeProperties.IsForwardReference) &&
					FieldProperties.HasFlag(TypeProperties.IsScoped) &&
					FieldProperties.HasFlag(TypeProperties.HasUniqueName) &&
					!IsUdtAnon
				);
			}
		}

		public LF_CLASS_STRUCTURE_INTERFACE(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			NumberOfElements = r.ReadUInt16();
			FieldProperties = r.ReadFlagsEnum<TypeProperties>();
			FieldIndex = r.ReadIndexedType32Lazy();
			DerivedType = r.ReadIndexedType32Lazy();
			VShapeTableType = r.ReadIndexedType32Lazy();

			StructSize = r.ReadVaryingType(out uint dataSize);

			Name = r.ReadCString();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_CLASS);
			w.WriteUInt16(NumberOfElements);
			w.Write<TypeProperties>(FieldProperties);
			w.WriteIndexedType(FieldIndex);
			w.WriteIndexedType(VShapeTableType);
			w.WriteVaryingType(StructSize);
			w.WriteCString(Name);
			w.WriteHeader();
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
