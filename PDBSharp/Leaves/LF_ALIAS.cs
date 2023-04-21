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

namespace Smx.PDBSharp.Leaves.LF_ALIAS
{
	public class Data : ILeafData {
		public ILeafResolver? UnderlyingType { get; set; }
		public string Name { get; set; }

		public Data(ILeafResolver? underlyingType, string name) {
			UnderlyingType = underlyingType;
			Name = name;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public long Read() {
			TypeDataReader r = CreateReader();
			var UnderlyingType = r.ReadIndexedType32Lazy();
			var Name = r.ReadCString();

			Data = new Data(
				underlyingType: UnderlyingType,
				name: Name
			);

			return r.Position;
		}

		public void Write() {
			throw new NotImplementedException();
		}

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			
		}
	}
}
