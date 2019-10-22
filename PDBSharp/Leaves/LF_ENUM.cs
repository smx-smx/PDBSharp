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

	public class LF_ENUM : LeafBase
	{
		public UInt16 NumElements { get; set; }
		public TypeProperties Properties { get; set; }

		public ILeafContainer UnderlyingType { get; set; }
		public ILeafContainer FieldType { get; set; }

		public string Name { get; set; }

		public override string UdtName => Name;

		public override bool IsDefnUdt {
			get {
				return Properties.HasFlag(TypeProperties.IsForwardReference);
			}
		}

		public override bool IsGlobalDefnUdt {
			get {
				return (
					Properties.HasFlag(TypeProperties.IsForwardReference) &&
					Properties.HasFlag(TypeProperties.IsScoped) &&
					!IsUdtAnon
				);
			}
		}

		public override bool IsLocalDefnUdtWithUniqueName {
			get {
				return (
					!Properties.HasFlag(TypeProperties.IsForwardReference) &&
					Properties.HasFlag(TypeProperties.IsScoped) &&
					Properties.HasFlag(TypeProperties.HasUniqueName) &&
					!IsUdtAnon
				);
			}
		}

		public LF_ENUM(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			NumElements = r.ReadUInt16();
			Properties = r.ReadFlagsEnum<TypeProperties>();
			UnderlyingType = r.ReadIndexedType32Lazy();
			FieldType = r.ReadIndexedType32Lazy();
			Name = r.ReadCString();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_ENUM);
			w.WriteUInt16(NumElements);
			w.Write<TypeProperties>(Properties);
			w.WriteIndexedType(UnderlyingType);
			w.WriteIndexedType(FieldType);
			w.WriteCString(Name);
			w.WriteHeader();
		}

		public override string ToString() {
			return $"LF_ENUM[NumElemens='{NumElements}'," +
				$"Properties='{Properties}', " +
				$"UnderlyingType='{UnderlyingType}', " +
				$"FieldType='{FieldType}', " +
				$"FieldName='{Name}]";
		}
	}
}
