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
	class LF_INDEX : LeafBase
	{
		public ILeafContainer Referenced { get; set; }

		public LF_INDEX(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();
			Referenced = r.ReadIndexedType32Lazy();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_INDEX);
			w.WriteIndexedType(Referenced);
			w.WriteHeader();
		}
	}
}
