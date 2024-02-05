#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
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
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_COMPILE
{
	public class Data : ISymbolData {
		public CompileSymFlags Flags { get; set; }
		public byte Machine { get; set; }
		public string VersionString { get; set; }

		public Data(CompileSymFlags flags, byte machine, string versionString) {
			Flags = flags;
			Machine = machine;
			VersionString = versionString;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public void Read() {
			var r = CreateReader();
			var Machine = r.ReadByte();
			uint flags = (uint)(r.ReadByte() | (r.ReadByte() << 8) | (r.ReadByte() << 16));
			var Flags = new CompileSymFlags(flags);
			var VersionString = r.ReadSymbolString();

			Data = new Data(
				flags: Flags,
				machine: Machine,
				versionString: VersionString
			);
		}

		public void Write() {
			throw new NotImplementedException();
		}

		public ISymbolData? GetData() => Data;

		public override string ToString() {
			var data = Data;
			return $"S_COMPILE(Flags='{data?.Flags}'" +
				$", Machine='{data?.Machine}'" +
				$", VersionString='{data?.VersionString}')";
		}
	}
}
