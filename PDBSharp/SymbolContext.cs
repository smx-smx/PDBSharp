using System;
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.S_SEPCODE;
using Smx.SharpIO;

namespace Smx.PDBSharp;

public class SymbolContext
{
	
	public SymbolType Type { get;  }
	public ISymbolData? Data { get; }

	public SymbolContext(SymbolType type, ISymbolData? data) {
		Type = type;
		Data = data;
	}

	public ISymbolSerializer CreateSerializer(SpanStream stream) {
		throw new NotImplementedException();
	}

	public override string ToString() {
		return $"Symbol: {Data}";
	}
}