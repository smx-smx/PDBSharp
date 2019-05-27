using Smx.PDBSharp.Symbols;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.PDBSharp
{
	public class Symbol
	{
		public readonly SymbolType Type;
		public readonly ISymbol Data;

		public Symbol(SymbolType type, ISymbol symbol) {
			this.Type = type;
			this.Data = symbol;
		}
	}
}
