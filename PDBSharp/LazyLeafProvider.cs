#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Smx.PDBSharp.Leaves;

namespace Smx.PDBSharp
{
	public class LazyLeafProvider : ILeaf
	{
		private ILeaf ReadLeaf() {
			return pdb.TPI.GetTypeByIndex(TypeIndex);
		}

		private readonly Lazy<ILeaf> lazy;
		public ILeaf Leaf => lazy.Value;


		private readonly PDBFile pdb;
		private readonly uint TypeIndex;

		public LazyLeafProvider(PDBFile pdb, uint typeIndex) {
			this.pdb = pdb;
			TypeIndex = typeIndex;
			lazy = new Lazy<ILeaf>(ReadLeaf);
		}

		public LazyLeafProvider(Lazy<ILeaf> lazyProvider) {
			lazy = lazyProvider;
		}

		public LeafType Type => Leaf.Type;
		public ILeafData Data => Leaf.Data;
	}
}
