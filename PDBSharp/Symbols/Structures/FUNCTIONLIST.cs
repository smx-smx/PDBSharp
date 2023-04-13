#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Smx.PDBSharp.Symbols.Structures
{
	public class FUNCTIONLIST : SymbolBase
	{
		public UInt32 NumberOfFunctions { get; set; }
		public ILeafContainer[] Functions { get; set; }

		public FUNCTIONLIST(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) { 			
		}

		public override void Read() {
			var r = CreateReader();

			NumberOfFunctions = r.ReadUInt32();
			Functions = Enumerable
				.Range(1, (int)NumberOfFunctions)
				.Select(_ => r.ReadIndexedType32Lazy())
				.ToArray();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_CALLEES);
			w.WriteUInt32(NumberOfFunctions);
			foreach (LeafContainerBase fn in Functions) {
				w.WriteIndexedType(fn);
			}

			w.WriteHeader();
		}
	}
}
