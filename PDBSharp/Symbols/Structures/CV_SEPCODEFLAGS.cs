using System;

namespace Smx.PDBSharp.Symbols.Structures
{
	[Flags]
	public enum CV_SEPCODEFLAGS : uint
	{
		IsLexicalScope = 1 << 0,
		ReturnsToParent = 1 << 1
	}
}