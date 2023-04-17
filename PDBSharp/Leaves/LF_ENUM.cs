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

namespace Smx.PDBSharp.Leaves.LF_ENUM
{
	public class Data : ILeafData, ILeafType
	{
		public UInt16 NumElements { get; set; }
		public TypeProperties Properties { get; set; }

		public ILeafResolver? UnderlyingType { get; set; }
		public ILeafResolver? FieldType { get; set; }

		public string Name { get; set; }

		public Data(ushort numElements, TypeProperties properties, ILeafResolver? underlyingType, ILeafResolver? fieldType, string name) {
			NumElements = numElements;
			Properties = properties;
			UnderlyingType = underlyingType;
			FieldType = fieldType;
			Name = name;
		}


		public string UdtName => Name;

		public bool IsDefnUdt {
			get {
				return Properties.HasFlag(TypeProperties.IsForwardReference);
			}
		}

		public bool IsGlobalDefnUdt {
			get {
				return (
					Properties.HasFlag(TypeProperties.IsForwardReference) &&
					Properties.HasFlag(TypeProperties.IsScoped) &&
					!LeafTypeHelper.IsUdtAnon(this)
				);
			}
		}

		public bool IsLocalDefnUdtWithUniqueName {
			get {
				return (
					!Properties.HasFlag(TypeProperties.IsForwardReference) &&
					Properties.HasFlag(TypeProperties.IsScoped) &&
					Properties.HasFlag(TypeProperties.HasUniqueName) &&
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

		public void Read() {
			TypeDataReader r = CreateReader();

			var NumElements = r.ReadUInt16();
			var Properties = r.ReadFlagsEnum<TypeProperties>();
			var UnderlyingType = r.ReadIndexedType32Lazy();
			var FieldType = r.ReadIndexedType32Lazy();
			var Name = r.ReadCString();

			Data = new Data(
				numElements: NumElements,
				properties: Properties,
				underlyingType: UnderlyingType,
				fieldType: FieldType,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_ENUM);
			w.WriteUInt16(data.NumElements);
			w.Write<TypeProperties>(data.Properties);
			w.WriteIndexedType(data.UnderlyingType);
			w.WriteIndexedType(data.FieldType);
			w.WriteCString(data.Name);
			w.WriteHeader();
		}

		public override string ToString() {
			var data = Data;
			return $"LF_ENUM[NumElemens='{data?.NumElements}'," +
				$"Properties='{data?.Properties}', " +
				$"UnderlyingType='{data?.UnderlyingType}', " +
				$"FieldType='{data?.FieldType}', " +
				$"FieldName='{data?.Name}]";
		}
	}
}
