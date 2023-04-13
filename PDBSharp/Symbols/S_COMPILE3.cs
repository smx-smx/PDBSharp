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
	public class S_COMPILE3 : SymbolBase
	{
		public CompileSym3Flags Flags { get; set; }
		public UInt16 Machine { get; set; }
		public UInt16 FrontendVersionMajor { get; set; }
		public UInt16 FrontendVersionMinor { get; set; }
		public UInt16 FrontendVersionBuild { get; set; }
		public UInt16 FrontendQFEVersion { get; set; }
		public UInt16 BackendVersionMajor { get; set; }
		public UInt16 BackendVersionMinor { get; set; }
		public UInt16 BackendVersionBuild { get; set; }
		public UInt16 BackendQFEVersion { get; set; }
		public string VersionString { get; set; }

		public S_COMPILE3(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream){
		}

		public override void Read() {
			var r = CreateReader();
			Flags = new CompileSym3Flags(r.ReadUInt32());
			Machine = r.ReadUInt16();
			FrontendVersionMajor = r.ReadUInt16();
			FrontendVersionMinor = r.ReadUInt16();
			FrontendVersionBuild = r.ReadUInt16();
			FrontendQFEVersion = r.ReadUInt16();
			BackendVersionMajor = r.ReadUInt16();
			BackendVersionMinor = r.ReadUInt16();
			BackendVersionBuild = r.ReadUInt16();
			BackendQFEVersion = r.ReadUInt16();
			VersionString = r.ReadSymbolString();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_COMPILE3);
			w.Write<CompileSym3FlagsEnum>((CompileSym3FlagsEnum)Flags);
			w.WriteUInt16(Machine);
			w.WriteUInt16(FrontendVersionMajor);
			w.WriteUInt16(FrontendVersionMinor);
			w.WriteUInt16(FrontendVersionBuild);
			w.WriteUInt16(FrontendQFEVersion);
			w.WriteUInt16(BackendVersionMajor);
			w.WriteUInt16(BackendVersionMinor);
			w.WriteUInt16(BackendVersionBuild);
			w.WriteUInt16(BackendQFEVersion);
			w.WriteSymbolString(VersionString);

			w.WriteHeader();
		}

		public override string ToString() {
			return $"S_COMPILE3[" +
				$"Flags='{Flags}', " +
				$"Machine='{Machine}', " +
				$"FrontendVersionMajor='{FrontendVersionMajor}', " +
				$"FrontendVersionMinor='{FrontendVersionMinor}', " +
				$"FrontendVersionBuild='{FrontendVersionBuild}', " +
				$"FrontendQFEVersion='{FrontendQFEVersion}', " +
				$"BackendVersionMajor='{BackendVersionMajor}', " +
				$"BackendVersionMinor='{BackendVersionMinor}', " +
				$"BackendVersionBuild='{BackendVersionBuild}', " +
				$"BackendQFEVersion='{BackendQFEVersion}', " +
				$"VersionString='{VersionString}'" +
			"]";
		}
	}
}
