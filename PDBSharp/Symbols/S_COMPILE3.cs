#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
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
	public class CompileSym3
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
	}

	public class S_COMPILE3 : ISymbol
	{
		public readonly CompileSym3Flags Flags;
		public readonly UInt16 Machine;
		public readonly UInt16 FrontendVersionMajor;
		public readonly UInt16 FrontendVersionMinor;
		public readonly UInt16 FrontendVersionBuild;
		public readonly UInt16 FrontendQFEVersion;
		public readonly UInt16 BackendVersionMajor;
		public readonly UInt16 BackendVersionMinor;
		public readonly UInt16 BackendVersionBuild;
		public readonly UInt16 BackendQFEVersion;
		public readonly string VersionString;

		public S_COMPILE3(PDBFile pdb, Stream stream) {
			var r = new SymbolDataReader(pdb, stream);
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

		public S_COMPILE3(CompileSym3 data) {
			Flags = data.Flags;
			Machine = data.Machine;
			FrontendVersionMajor = data.FrontendVersionMajor;
			FrontendVersionMinor = data.FrontendVersionMinor;
			FrontendVersionBuild = data.FrontendVersionBuild;
			FrontendQFEVersion = data.FrontendQFEVersion;
			BackendVersionMajor = data.BackendVersionMajor;
			BackendVersionMinor = data.BackendVersionMinor;
			BackendQFEVersion = data.BackendQFEVersion;
			VersionString = data.VersionString;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_COMPILE3);
			w.WriteEnum<CompileSym3FlagsEnum>((CompileSym3FlagsEnum)Flags);
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

			w.WriteSymbolHeader();
		}
	}
}
