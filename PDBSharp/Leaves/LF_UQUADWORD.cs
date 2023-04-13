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
	public class LF_UQUADWORD : LeafBase
	{
		public ulong Value { get; set; }

		public LF_UQUADWORD(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();
			Value = r.ReadUInt64();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_UQUADWORD);
			w.WriteUInt64(Value);
			w.WriteHeader();
		}
	}
}
