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
using System.Collections.Generic;
using System.ComponentModel.Design;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_COMPILE2
{
	public class Data : ISymbolData {
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


		public Data(CompileSym2Flags flags, ushort machine, ushort frontendVersionMajor, ushort frontendVersionMinor, ushort frontendVersionBuild, ushort backendVersionMajor, ushort backendVersionMinor, ushort backendVersionBuild, string versionString, string[] optionalData) {
			Flags = flags;
			Machine = machine;
			FrontendVersionMajor = frontendVersionMajor;
			FrontendVersionMinor = frontendVersionMinor;
			FrontendVersionBuild = frontendVersionBuild;
			BackendVersionMajor = backendVersionMajor;
			BackendVersionMinor = backendVersionMinor;
			BackendVersionBuild = backendVersionBuild;
			VersionString = versionString;
			OptionalData = optionalData;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			var r = CreateReader();
			var Flags = new CompileSym2Flags(r.ReadUInt32());
			var Machine = r.ReadUInt16();
			var FrontendVersionMajor = r.ReadUInt16();
			var FrontendVersionMinor = r.ReadUInt16();
			var FrontendVersionBuild = r.ReadUInt16();
			var BackendVersionMajor = r.ReadUInt16();
			var BackendVersionMinor = r.ReadUInt16();
			var BackendVersionBuild = r.ReadUInt16();
			var VersionString = r.ReadSymbolString();

			List<string> optionalData = new List<string>();
			while (r.HasMoreData) {
				string str = r.ReadSymbolString();
				if (str.Length == 0)
					break;
				optionalData.Add(str);
			}
			var OptionalData = optionalData.ToArray();

			Data = new Data(
				flags: Flags,
				machine: Machine,
				frontendVersionMajor: FrontendVersionMajor,
				frontendVersionMinor: FrontendVersionMinor,
				frontendVersionBuild: FrontendVersionBuild,
				backendVersionMajor: BackendVersionMajor,
				backendVersionMinor: BackendVersionMinor,
				backendVersionBuild: BackendVersionBuild,
				versionString: VersionString,
				optionalData: OptionalData
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_COMPILE2);
			w.Write<CompileSym2FlagsEnum>((CompileSym2FlagsEnum)data.Flags);
			w.WriteUInt16(data.Machine);
			w.WriteUInt16(data.FrontendVersionMajor);
			w.WriteUInt16(data.FrontendVersionMinor);
			w.WriteUInt16(data.FrontendVersionBuild);
			w.WriteUInt16(data.BackendVersionMajor);
			w.WriteUInt16(data.BackendVersionMinor);
			w.WriteUInt16(data.BackendVersionBuild);
			w.WriteSymbolString(data.VersionString);

			foreach (string str in data.OptionalData) {
				w.WriteSymbolString(str);
			}

			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;

		public override string ToString() {
			var data = Data;
			return $"S_COMPILE2(" +
				$"Flags='{data?.Flags}', " +
				$"Machine='{data?.Machine}', " +
				$"FrontendVersionMajor='{data?.FrontendVersionMajor}', " +
				$"FrontendVersionMinor='{data?.FrontendVersionMinor}', " +
				$"FrontendVersionBuild='{data?.FrontendVersionBuild}', " +
				$"BackendVersionMajor='{data?.BackendVersionMajor}', " +
				$"BackendVersionMinor='{data?.BackendVersionMinor}', " +
				$"BackendVersionBuild='{data?.BackendVersionBuild}'" +
				$"VersionString='{data?.VersionString}'" +
			")";
		}
	}
}
