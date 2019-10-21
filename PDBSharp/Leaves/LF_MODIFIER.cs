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

	public class LF_MODIFIER : LeafBase
	{
		public CVModifier Flags { get; set; }
		public ILeafContainer ModifiedType { get; set; }

		public LF_MODIFIER(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			ModifiedType = r.ReadIndexedTypeLazy();
			Flags = r.ReadFlagsEnum<CVModifier>();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_MODIFIER);
			w.WriteIndexedType(ModifiedType);
			w.Write<CVModifier>(Flags);
			w.WriteHeader();
		}

		public override string ToString() {
			return $"LF_MODIFIER[ModifiedType='{ModifiedType}', Flags='{Flags}']";
		}
	}
}
