#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{

	public class LF_ENUMERATE : LeafBase
	{
		public FieldAttributes Attributes;
		public ILeafContainer Length;
		public string FieldName;

		public LF_ENUMERATE(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			Attributes = new FieldAttributes(r.ReadUInt16());
			Length = r.ReadVaryingType(out uint ILeafSize);
			FieldName = r.ReadCString();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_ENUMERATE);
			w.WriteUInt16((ushort)Attributes);
			w.WriteVaryingType(Length);
			w.WriteCString(FieldName);
			w.WriteHeader();
		}
	}
}
