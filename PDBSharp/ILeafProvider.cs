using Smx.PDBSharp.Leaves;

namespace Smx.PDBSharp
{
	public interface ILeaf
	{
		LeafType Type { get; }
		ILeafData Data { get; }
	}
}