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
	public class LF_ALIAS : LeafBase, ILeaf
	{
		public readonly ILeafContainer UnderlyingType;
		public readonly string Name;

		public override string UdtName => Name;

		public void Write(PDBFile pdb, Stream stream) {
			throw new NotImplementedException();
		}

		public LF_ALIAS(IServiceContainer ctx, Stream stream) {
			TypeDataReader r = new TypeDataReader(ctx, stream);
			UnderlyingType = r.ReadIndexedTypeLazy();
			Name = r.ReadCString();
		}
	}
}
