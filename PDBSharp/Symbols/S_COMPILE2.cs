#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_COMPILE2 : SymbolBase
	{
		public CompileSym2Flags Flags { get; set; }
		public UInt16 Machine { get; set; }
		public UInt16 FrontendVersionMajor { get; set; }
		public UInt16 FrontendVersionMinor { get; set; }
		public UInt16 FrontendVersionBuild { get; set; }
		public UInt16 BackendVersionMajor { get; set; }
		public UInt16 BackendVersionMinor { get; set; }
		public UInt16 BackendVersionBuild { get; set; }
		public string VersionString { get; set; }
		public string[] OptionalData { get; set; }

		public S_COMPILE2(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();
			Flags = new CompileSym2Flags(r.ReadUInt32());
			Machine = r.ReadUInt16();
			FrontendVersionMajor = r.ReadUInt16();
			FrontendVersionMinor = r.ReadUInt16();
			FrontendVersionBuild = r.ReadUInt16();
			BackendVersionMajor = r.ReadUInt16();
			BackendVersionMinor = r.ReadUInt16();
			BackendVersionBuild = r.ReadUInt16();
			VersionString = r.ReadSymbolString();

			List<string> optionalData = new List<string>();
			while (r.HasMoreData) {
				string str = r.ReadSymbolString();
				if (str.Length == 0)
					break;
				optionalData.Add(str);
			}
			OptionalData = optionalData.ToArray();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_COMPILE2);
			w.Write<CompileSym2FlagsEnum>((CompileSym2FlagsEnum)Flags);
			w.WriteUInt16(Machine);
			w.WriteUInt16(FrontendVersionMajor);
			w.WriteUInt16(FrontendVersionMinor);
			w.WriteUInt16(FrontendVersionBuild);
			w.WriteUInt16(BackendVersionMajor);
			w.WriteUInt16(BackendVersionMinor);
			w.WriteUInt16(BackendVersionBuild);
			w.WriteSymbolString(VersionString);

			foreach (string str in OptionalData) {
				w.WriteSymbolString(str);
			}

			w.WriteHeader();
		}

		public override string ToString() {
			return $"S_COMPILE2[" +
				$"Flags='{Flags}', " +
				$"Machine='{Machine}', " +
				$"FrontendVersionMajor='{FrontendVersionMajor}', " +
				$"FrontendVersionMinor='{FrontendVersionMinor}', " +
				$"FrontendVersionBuild='{FrontendVersionBuild}', " +
				$"BackendVersionMajor='{BackendVersionMajor}', " +
				$"BackendVersionMinor='{BackendVersionMinor}', " +
				$"BackendVersionBuild='{BackendVersionBuild}'" +
				$"VersionString='{VersionString}'" +
			"]";
		}
	}
}
