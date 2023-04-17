#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
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

namespace Smx.PDBSharp.Leaves.LF_MODIFIER
{

	public class Data : ILeafData {
		public CVModifier Flags { get; set; }
		public ILeafResolver? ModifiedType { get; set; }

		public Data(CVModifier flags, ILeafResolver? modifiedType) {
			Flags = flags;
			ModifiedType = modifiedType;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		

		public void Read() {
			TypeDataReader r = CreateReader();

			var ModifiedType = r.ReadIndexedType32Lazy();
			var Flags = r.ReadFlagsEnum<CVModifier>();
			Data = new Data(
				modifiedType: ModifiedType,
				flags: Flags
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_MODIFIER);
			w.WriteIndexedType(data.ModifiedType);
			w.Write<CVModifier>(data.Flags);
			w.WriteHeader();
		}

		public override string ToString() {
			var data = Data;
			return $"LF_MODIFIER[ModifiedType='{data?.ModifiedType}', Flags='{data?.Flags}']";
		}
	}
}
