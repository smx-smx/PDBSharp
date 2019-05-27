using System;
using System.Collections.Generic;
using System.Text;
using Smx.PDBSharp.Leaves;

namespace Smx.PDBSharp
{
	public class DirectLeafProvider : ILeaf
	{
		private readonly LeafType type;
		private readonly ILeafData data;

		public DirectLeafProvider(LeafType type, ILeafData data) {
			this.type = type;
			this.data = data;
		}

		public LeafType Type => type;
		public ILeafData Data => data;
	}
}
