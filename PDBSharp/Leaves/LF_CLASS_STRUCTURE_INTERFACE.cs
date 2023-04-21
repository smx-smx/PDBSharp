#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_CLASS_STRUCTURE_INTERFACE
{
	public class Data : ILeafData, ILeafType {
		public UInt16 NumberOfElements { get; set; }
		public TypeProperties FieldProperties { get; set; }
		public ILeafResolver? FieldIndex { get; set; }
		public ILeafResolver? DerivedType { get; set; }
		public ILeafResolver? VShapeTableType { get; set; }

		//public UInt16 StructSize;

		public ILeafResolver? StructSize { get; set; }

		public string Name { get; set; }

		public Data(ushort numberOfElements, TypeProperties fieldProperties, ILeafResolver? fieldIndex, ILeafResolver? derivedType, ILeafResolver? vShapeTableType, ILeafResolver? structSize, string name) {
			NumberOfElements = numberOfElements;
			FieldProperties = fieldProperties;
			FieldIndex = fieldIndex;
			DerivedType = derivedType;
			VShapeTableType = vShapeTableType;
			StructSize = structSize;
			Name = name;
		}

		public string UdtName => Name;

		public bool IsDefnUdt {
			get {
				return FieldProperties.HasFlag(TypeProperties.IsForwardReference);
			}
		}

		public bool IsGlobalDefnUdt {
			get {
				return (
					FieldProperties.HasFlag(TypeProperties.IsForwardReference) &&
					FieldProperties.HasFlag(TypeProperties.IsScoped) &&
					!LeafTypeHelper.IsUdtAnon(this)
				);
			}
		}

		public bool IsLocalDefnUdtWithUniqueName {
			get {
				return (
					!FieldProperties.HasFlag(TypeProperties.IsForwardReference) &&
					FieldProperties.HasFlag(TypeProperties.IsScoped) &&
					FieldProperties.HasFlag(TypeProperties.HasUniqueName) &&
					!LeafTypeHelper.IsUdtAnon(this)
				);
			}
		}

		public bool IsUdtSourceLine => throw new NotImplementedException();

		public bool IsGlobalDefnUdtWithUniqueName => throw new NotImplementedException();
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public long Read() {
			TypeDataReader r = CreateReader();

			var NumberOfElements = r.ReadUInt16();
			var FieldProperties = r.ReadFlagsEnum<TypeProperties>();
			var FieldIndex = r.ReadIndexedType32Lazy();
			var DerivedType = r.ReadIndexedType32Lazy();
			var VShapeTableType = r.ReadIndexedType32Lazy();

			var StructSize = r.ReadVaryingType(out uint dataSize);
			var Name = r.ReadCString();

			Data = new Data(
				numberOfElements: NumberOfElements,
				fieldProperties: FieldProperties,
				fieldIndex: FieldIndex,
				derivedType: DerivedType,
				vShapeTableType: VShapeTableType,
				structSize: StructSize,
				name: Name
			);

			return r.Position;
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_CLASS);
			w.WriteUInt16(data.NumberOfElements);
			w.Write<TypeProperties>(data.FieldProperties);
			w.WriteIndexedType(data.FieldIndex);
			w.WriteIndexedType(data.VShapeTableType);
			w.WriteVaryingType(data.StructSize);
			w.WriteCString(data.Name);
			w.WriteHeader();
		}

		public override string ToString() {
			var data = Data;
			return $"LF_CLASS[NumberOfElements='{data?.NumberOfElements}', " +
				$"FieldProperties='{data?.FieldProperties}', " +
				$"FieldIndex='{data?.FieldIndex}', " +
				$"VShapeTableType='{data?.VShapeTableType}', " +
				$"StructSize='{data?.StructSize}', " +
				$"Name='{data?.Name}']";
		}
	}
}
