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
	public class LF_REAL64 : LeafBase
	{
		public double Value { get; set; }

		public LF_REAL64(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();
			Value = r.ReadDouble();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_REAL64);
			w.WriteDouble(Value);
			w.WriteHeader();
		}
	}
}
