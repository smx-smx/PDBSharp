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
	public class LF_VFTPATH_16t : LeafBase
	{
		public UInt16 NumElements { get; set; }
		public ILeafContainer Bases { get; set; }

		public LF_VFTPATH_16t(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();
			NumElements = r.ReadUInt16();
			Bases = r.ReadIndexedType16Lazy();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_VFTPATH_16t);
			w.WriteUInt16(NumElements);
			w.WriteIndexedType16(Bases);
			w.WriteHeader();
		}
	}
}
