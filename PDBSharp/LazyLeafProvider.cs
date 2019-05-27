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

		public LeafType Type => Leaf.Type;
		public ILeafData Data => Leaf.Data;
	}
}
