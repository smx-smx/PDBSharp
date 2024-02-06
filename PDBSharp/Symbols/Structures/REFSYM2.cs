#region License
/*
 * Copyright (C) 2024 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Smx.PDBSharp.Symbols.Structures
{
	namespace REFSYM2
	{
		public class Data : ISymbolData
		{
			public uint SumName;
			public uint SymbolOffset;
			public ushort ModuleIndex;
			public string Name = string.Empty;

			public SymbolType Type { get; set; }

			public override string ToString() {
				return $"REFSYM2(Name='{Name}', ModuleIndex='{ModuleIndex}', SymbolOffset='{SymbolOffset}')";
			}
		}

		public class Serializer : SymbolSerializerBase, ISymbolSerializer
		{
			public Data Data = new Data();

			public Serializer(IServiceContainer sc, SpanStream stream, SymbolType type) : base(sc, stream) {
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
				var name = r.ReadSymbolString();
				Data = new Data {
					Type = Data.Type,
					SumName = sumName,
					SymbolOffset = symbolOffset,
					ModuleIndex = moduleIndex,
					Name = name
				};
			}

			public void Write() {
			}
		}
	}

}
