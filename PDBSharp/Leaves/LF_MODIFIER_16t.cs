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
	public class LF_MODIFIER_16t : LeafBase
	{
		public CVModifier Attributes { get; set; }
		public ILeafContainer ModifiedType { get; set; }

		public LF_MODIFIER_16t(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			Attributes = r.ReadFlagsEnum<CVModifier>();
			ModifiedType = r.ReadIndexedType16Lazy();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_MODIFIER_16t);
			w.Write<CVModifier>(Attributes);
			w.WriteIndexedType16(ModifiedType);
			w.WriteHeader();
		}
	}
}
