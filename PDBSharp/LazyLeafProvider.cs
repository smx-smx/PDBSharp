#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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
	public class LazyLeafProvider : ILeafContainer
	{
		private ILeafContainer ReadLeaf() {
			return pdb.TPI.GetTypeByIndex(typeIndex);
		}

		public void Write(PDBFile pdb, Stream stream) {
			Leaf.Data.Write(pdb, stream);
		}

		private readonly Lazy<ILeafContainer> lazy;
		public ILeafContainer Leaf => lazy.Value;

		public uint TypeIndex => Leaf.TypeIndex;

		public LeafType Type => Leaf.Type;

		public ILeaf Data => Leaf.Data;

		private readonly PDBFile pdb;
		private readonly uint typeIndex;

		public LazyLeafProvider(PDBFile pdb, uint typeIndex) {
			this.pdb = pdb;
			this.typeIndex = typeIndex;
			lazy = new Lazy<ILeafContainer>(ReadLeaf);
		}

		public LazyLeafProvider(Lazy<ILeafContainer> lazyProvider) {
			lazy = lazyProvider;
		}
	}
}
