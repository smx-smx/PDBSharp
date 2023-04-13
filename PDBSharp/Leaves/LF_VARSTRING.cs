#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	public class LF_VARSTRING : LeafBase
	{
		public string Value { get; set; }

		public LF_VARSTRING(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {			
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			UInt16 length = r.ReadUInt16();
			byte[] data = r.ReadBytes((int)length);
			Value = Encoding.ASCII.GetString(data);
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_VARSTRING);
			w.WriteShortString(Value);
			w.WriteHeader();
		}
	}
}
