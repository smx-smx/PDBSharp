#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_COMPILE : SymbolBase
	{
		public CompileSymFlags Flags { get; set; }
		public byte Machine { get; set; }
		public string VersionString { get; set; }

		public S_COMPILE(IServiceContainer ctx, IModule mod, SpanStream stream): base(ctx, mod, stream) {
		}

		public override void Read() {
			var r = CreateReader();
			Machine = r.ReadByte();
			uint flags = (uint)(r.ReadByte() | (r.ReadByte() << 8) | (r.ReadByte() << 16));
			Flags = new CompileSymFlags(flags);
			VersionString = r.ReadSymbolString();
		}

		public override string ToString() {
			return $"S_COMPILE[Flags='{Flags}', Machine='{Machine}', VersionString='{VersionString}']";
		}
	}
}
