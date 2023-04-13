#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_MEMBER : LeafBase
	{
		public FieldAttributes Attributes { get; set; }
		public ILeafContainer FieldType { get; set; }

		public ILeafContainer Offset { get; set; }

		public string Name { get; set; }

		public LF_MEMBER(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			Attributes = new FieldAttributes(r.ReadUInt16());
			FieldType = r.ReadIndexedType32Lazy();

			Offset = r.ReadVaryingType(out uint dataSize);

			Name = r.ReadCString();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_MEMBER);
			w.WriteUInt16((ushort)Attributes);
			w.WriteIndexedType(FieldType);
			w.WriteVaryingType(Offset);
			w.WriteCString(Name);
			w.WriteHeader();
		}

		public override string ToString() {
			return $"LF_MEMBER[Attributes='{Attributes}', FieldType='{FieldType}', Offset='{Offset}', Name='{Name}']";
		}
	}
}
