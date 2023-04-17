using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp;

public class DirectLeafData : ILeafResolver
{
	public LeafContext Ctx { get; }
		
	public DirectLeafData(LeafContext data) {
		Ctx = data;
	}
}