using System.ComponentModel.Design;
using System.Diagnostics;
using Smx.PDBSharp.LeafResolver;
using Smx.PDBSharp.Leaves;

namespace Smx.PDBSharp;

public class LazyLeafData : ILeafResolver
{
	private readonly ILazy<ILeafResolver?> lazy;

	public LeafContext Ctx {
		get {
			Debug.Assert(lazy.Value != null);
			return lazy.Value.Ctx;
		}
	}

	public LazyLeafData(ILazy<ILeafResolver?> lazyProvider) {
		lazy = lazyProvider;
	}
}