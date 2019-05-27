using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Smx.PDBSharp.Symbols.Structures;

namespace Smx.PDBSharp.Thunks
{
	[ThunkReader(ThunkType.PCODE)]
	public class PCODE : SymbolDataReader, IThunk
	{
		public PCODE(PDBFile pdb, SymbolHeader header, Stream stream) : base(pdb, header, stream) {
		}
	}
}
