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
	public class LF_UNION : LeafBase, ILeaf
	{
		public UInt16 NumberOfElements { get; set; }
		public TypeProperties Properties { get; set; }
		public ILeafContainer FieldType { get; set; }

		public ILeafContainer StructSize { get; set; }

		public string Name { get; set; }

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

		public override string UdtName => Name;

		public LF_UNION(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			NumberOfElements = r.ReadUInt16();
			Properties = r.ReadFlagsEnum<TypeProperties>();
			FieldType = r.ReadIndexedType32Lazy();

			StructSize = r.ReadVaryingType(out uint dataSize);
			Name = r.ReadCString();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = CreateWriter(LeafType.LF_UNION);
			w.WriteUInt16(NumberOfElements);
			w.Write<TypeProperties>(Properties);
			w.WriteIndexedType(FieldType);
			w.WriteVaryingType(StructSize);
			w.WriteCString(Name);
		}
	}
}
