#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Smx.PDBSharp.Symbols
{
	internal class S_ANNOTATION : SymbolBase
	{
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public UInt16 NumberOfStrings { get; set; }
		public string[] Annotations { get; set; }

		public S_ANNOTATION(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			NumberOfStrings = r.ReadUInt16();

			Annotations = Enumerable.Range(1, NumberOfStrings)
				.Select(_ => r.ReadCString())
				.ToArray();
		}
	}
}
