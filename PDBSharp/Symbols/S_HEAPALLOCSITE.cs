using Smx.PDBSharp.Leaves;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Symbols
{
	[SymbolReader(SymbolType.S_HEAPALLOCSITE)]
	public class S_HEAPALLOCSITE : SymbolDataReader
	{
		public readonly UInt32 CallSiteOffset;
		public readonly UInt16 SectionIndex;
		public readonly UInt16 HeapAllocationInstructionSize;
		public readonly ILeaf FunctionSignature;

		public S_HEAPALLOCSITE(PDBFile pdb, Stream stream) : base(pdb, stream) {
			CallSiteOffset = ReadUInt32();

			SectionIndex = ReadUInt16();
			HeapAllocationInstructionSize = ReadUInt16();

			FunctionSignature = ReadIndexedTypeLazy();
		}
	}
}
