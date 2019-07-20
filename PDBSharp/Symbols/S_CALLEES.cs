#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{
	public class S_CALLEES : ISymbol
	{
		public readonly UInt32 NumberOfFunctions;
		public readonly ILeafContainer[] Functions;

		public S_CALLEES(Context ctx, IModule mod, Stream stream) {
			var r = new SymbolDataReader(ctx, stream);

			NumberOfFunctions = r.ReadUInt32();
			Functions = Enumerable
				.Range(1, (int)NumberOfFunctions)
				.Select(_ => r.ReadIndexedTypeLazy())
				.ToArray();
		}

		public S_CALLEES(IEnumerable<LeafContainerBase> functionsList) {
			Functions = functionsList.ToArray();
			NumberOfFunctions = (uint)Functions.Length;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_CALLEES);
			w.WriteUInt32(NumberOfFunctions);
			foreach(LeafContainerBase fn in Functions) {
				w.WriteIndexedType(fn);
			}

			w.WriteSymbolHeader();
		}
	}
}
