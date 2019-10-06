using System;
using System.IO;
using System.Linq;
using Smx.PDBSharp.Symbols;

namespace Smx.PDBSharp.Symbols
{
	internal class S_ANNOTATION : ISymbol
	{
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly UInt16 NumberOfStrings;
		public readonly string[] Annotations;

		public S_ANNOTATION(Context ctx, IModule mod, Stream stream) {
			var r = new SymbolDataReader(ctx, stream);
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			NumberOfStrings = r.ReadUInt16();

			Annotations = Enumerable.Range(1, NumberOfStrings)
				.Select(_ => r.ReadCString())
				.ToArray();
		}

		public void Write(PDBFile pdb, Stream stream) {
			throw new System.NotImplementedException();
		}
	}
}