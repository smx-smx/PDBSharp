namespace Smx.PDBSharp;

public class DirectSymbolData : ISymbolResolver
{
	public SymbolContext? Data { get; }

	public DirectSymbolData(SymbolContext? data) {
		Data = data;
	}

	public override string ToString() {
		return $"[DirectSymbolData]: {Data}";
	}
}