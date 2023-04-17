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

namespace Smx.PDBSharp.Leaves.LF_UNION
{
	public class Data : ILeafData, ILeafType {
		public UInt16 NumberOfElements { get; set; }
		public TypeProperties Properties { get; set; }
		public ILeafResolver? FieldType { get; set; }

		public ILeafResolver? StructSize { get; set; }

		public string Name { get; set; }


		public string? UdtName => Name;

		public bool IsUdtSourceLine => throw new NotImplementedException();

		public bool IsGlobalDefnUdtWithUniqueName => throw new NotImplementedException();

		public bool IsLocalDefnUdtWithUniqueName => (
					!Properties.HasFlag(TypeProperties.IsForwardReference) &&
					Properties.HasFlag(TypeProperties.IsScoped) &&
					Properties.HasFlag(TypeProperties.HasUniqueName) &&
					!LeafTypeHelper.IsUdtAnon(this)
				);

		public bool IsDefnUdt => Properties.HasFlag(TypeProperties.IsForwardReference);

		public bool IsGlobalDefnUdt => (
					Properties.HasFlag(TypeProperties.IsForwardReference) &&
					Properties.HasFlag(TypeProperties.IsScoped) &&
					!LeafTypeHelper.IsUdtAnon(this)
				);

		public Data(ushort numberOfElements, TypeProperties properties, ILeafResolver? fieldType, ILeafResolver? structSize, string name) {
			NumberOfElements = numberOfElements;
			Properties = properties;
			FieldType = fieldType;
			StructSize = structSize;
			Name = name;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public void Read() {
			TypeDataReader r = CreateReader();

			var NumberOfElements = r.ReadUInt16();
			var Properties = r.ReadFlagsEnum<TypeProperties>();
			var FieldType = r.ReadIndexedType32Lazy();

			var StructSize = r.ReadVaryingType(out uint dataSize);
			var Name = r.ReadCString();

			Data = new Data(
				numberOfElements: NumberOfElements,
				properties: Properties,
				fieldType: FieldType,
				structSize: StructSize,
				name: Name
			);
		}

		public void Write() {
			throw new NotImplementedException();
		}

		public void Write(PDBFile pdb, Stream stream) {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_UNION);
			w.WriteUInt16(data.NumberOfElements);
			w.Write<TypeProperties>(data.Properties);
			w.WriteIndexedType(data.FieldType);
			w.WriteVaryingType(data.StructSize);
			w.WriteCString(data.Name);
		}
	}
}
