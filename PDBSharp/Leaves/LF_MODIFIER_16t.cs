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
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves
{
	public class LF_MODIFIER_16t_Data {
		public CVModifier Attributes { get; set; }
		public ILeafResolver? ModifiedType { get; set; }

		public LF_MODIFIER_16t_Data(CVModifier attributes, ILeafResolver? modifiedType) {
			Attributes = attributes;
			ModifiedType = modifiedType;
		}
	}

	public class LF_MODIFIER_16t : LeafBase
	{
		public LF_MODIFIER_16t_Data? Data { get; set;  }

		

		public LF_MODIFIER_16t(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			TypeDataReader r = CreateReader();

			var Attributes = r.ReadFlagsEnum<CVModifier>();
			var ModifiedType = r.ReadIndexedType16Lazy();

			Data = new LF_MODIFIER_16t_Data(
				attributes: Attributes,
				modifiedType: ModifiedType
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_MODIFIER_16t);
			w.Write<CVModifier>(data.Attributes);
			w.WriteIndexedType16(data.ModifiedType);
			w.WriteHeader();
		}
	}
}
