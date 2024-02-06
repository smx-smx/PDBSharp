using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Smx.PDBSharp.Symbols.Structures
{
	namespace REFSYM {
		public class Data : ISymbolData {
			public uint SumName;
			public uint SymbolOffset;
			public ushort ModuleIndex;
			public ushort Fill;

			public SymbolType Type { get; set; }
		}

		public class Serializer : SymbolSerializerBase, ISymbolSerializer
		{
			public Data Data = new Data();
			public Serializer(IServiceContainer sc, SpanStream stream, SymbolType type) : base(sc, stream){
				Data.Type = type;
			}

			public ISymbolData? GetData() {
				return Data;
			}

			public void Read() {
				var r = CreateReader();
				var sumName = r.ReadUInt32();
				var symbolOffset = r.ReadUInt32();
				var moduleIndex = r.ReadUInt16();
				var fill = r.ReadUInt16();
				Data = new Data {
					Type = Data.Type,
					SumName = sumName,
					SymbolOffset = symbolOffset,
					ModuleIndex = moduleIndex,
					Fill = fill
				};
			}

			public void Write() {
				throw new NotImplementedException();
			}
		}
	}
}
