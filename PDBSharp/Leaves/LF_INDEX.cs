using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	[LeafReader(LeafType.LF_INDEX)]
	class LF_INDEX : TypeDataReader
	{
		public readonly ILeaf Referenced;

		public LF_INDEX(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Referenced = ReadIndexedTypeLazy();
		}
	}
}
