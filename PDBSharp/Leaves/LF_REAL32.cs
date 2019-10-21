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
	public class LF_REAL32 : LeafBase
	{
		public float Value { get; set; }
		public LF_REAL32(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();
			Value = r.ReadSingle();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = CreateWriter(LeafType.LF_REAL32);
			w.WriteSingle(Value);
			w.WriteHeader();
		}
	}
}
