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

namespace Smx.PDBSharp.Symbols.S_COMPILE3
{
	public class Data : ISymbolData {
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

		public Data(CompileSym3Flags flags, ushort machine, ushort frontendVersionMajor, ushort frontendVersionMinor, ushort frontendVersionBuild, ushort frontendQfeVersion, ushort backendVersionMajor, ushort backendVersionMinor, ushort backendVersionBuild, ushort backendQfeVersion, string versionString) {
			Flags = flags;
			Machine = machine;
			FrontendVersionMajor = frontendVersionMajor;
			FrontendVersionMinor = frontendVersionMinor;
			FrontendVersionBuild = frontendVersionBuild;
			FrontendQFEVersion = frontendQfeVersion;
			BackendVersionMajor = backendVersionMajor;
			BackendVersionMinor = backendVersionMinor;
			BackendVersionBuild = backendVersionBuild;
			BackendQFEVersion = backendQfeVersion;
			VersionString = versionString;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		private Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();
			var Flags = new CompileSym3Flags(r.ReadUInt32());
			var Machine = r.ReadUInt16();
			var FrontendVersionMajor = r.ReadUInt16();
			var FrontendVersionMinor = r.ReadUInt16();
			var FrontendVersionBuild = r.ReadUInt16();
			var FrontendQFEVersion = r.ReadUInt16();
			var BackendVersionMajor = r.ReadUInt16();
			var BackendVersionMinor = r.ReadUInt16();
			var BackendVersionBuild = r.ReadUInt16();
			var BackendQFEVersion = r.ReadUInt16();
			var VersionString = r.ReadSymbolString();
			Data = new Data(
				flags: Flags,
				machine: Machine,
				frontendVersionMajor: FrontendVersionMajor,
				frontendVersionMinor: FrontendVersionMinor,
				frontendVersionBuild: FrontendVersionBuild,
				frontendQfeVersion: FrontendQFEVersion,
				backendVersionMajor: BackendVersionMajor,
				backendVersionMinor: BackendVersionMinor,
				backendVersionBuild: BackendVersionBuild,
				backendQfeVersion: BackendQFEVersion,
				versionString: VersionString
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_COMPILE3);
			w.Write<CompileSym3FlagsEnum>((CompileSym3FlagsEnum)data.Flags);
			w.WriteUInt16(data.Machine);
			w.WriteUInt16(data.FrontendVersionMajor);
			w.WriteUInt16(data.FrontendVersionMinor);
			w.WriteUInt16(data.FrontendVersionBuild);
			w.WriteUInt16(data.FrontendQFEVersion);
			w.WriteUInt16(data.BackendVersionMajor);
			w.WriteUInt16(data.BackendVersionMinor);
			w.WriteUInt16(data.BackendVersionBuild);
			w.WriteUInt16(data.BackendQFEVersion);
			w.WriteSymbolString(data.VersionString);
			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;

		public override string ToString() {
			var data = Data;
			return $"S_COMPILE3(" +
				$"Flags='{data?.Flags}', " +
				$"Machine='{data?.Machine}', " +
				$"FrontendVersionMajor='{data?.FrontendVersionMajor}', " +
				$"FrontendVersionMinor='{data?.FrontendVersionMinor}', " +
				$"FrontendVersionBuild='{data?.FrontendVersionBuild}', " +
				$"FrontendQFEVersion='{data?.FrontendQFEVersion}', " +
				$"BackendVersionMajor='{data?.BackendVersionMajor}', " +
				$"BackendVersionMinor='{data?.BackendVersionMinor}', " +
				$"BackendVersionBuild='{data?.BackendVersionBuild}', " +
				$"BackendQFEVersion='{data?.BackendQFEVersion}', " +
				$"VersionString='{data?.VersionString}'" +
			")";
		}
	}
}
