#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols.Structures
{
	public abstract class ConstSymBase : SymbolBase
	{
		public ILeafContainer Type { get; set; }
		public ILeafContainer Value { get; set; }
		public string Name { get; set; }

		public ConstSymBase(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();

			Type = r.ReadIndexedType32Lazy();

			Value = r.ReadVaryingType(out uint dataSize);
			Name = r.ReadSymbolString();
		}

		public void Write(SymbolType symbolType) {
			var w = CreateWriter(symbolType);

			w.WriteIndexedType(Type);
			w.WriteVaryingType(Value);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
