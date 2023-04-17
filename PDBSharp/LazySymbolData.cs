using System.Diagnostics;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp;

public class LazySymbolData : ISymbolResolver
{
	private readonly ILazy<ISymbolResolver?> lazy;

	public SymbolContext? Data {
		get {
			Debug.Assert(lazy.Value != null);
			return lazy.Value.Data;
		}
	}

	public LazySymbolData(ILazy<ISymbolResolver?> lazy) {
		this.lazy = lazy;
	}
}