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
	public class CompileSym
	{
		public CompileSym2Flags Flags { get; set; }
		public UInt16 Machine { get; set; }
		public UInt16 FrontendVersionMajor { get; set; }
		public UInt16 FrontentVersionMinor { get; set; }
		public UInt16 FrontendVersionBuild { get; set; }
		public UInt16 BackendVersionMajor { get; set; }
		public UInt16 BackendVersionMinor { get; set; }
		public UInt16 BackendVersionBuild { get; set; }
		public string VersionString { get; set; }
		public string[] OptionalData { get; set; }
	}

	public class S_COMPILE2 : ISymbol
	{
		public readonly CompileSym2Flags Flags;
		public readonly UInt16 Machine;
		public readonly UInt16 FrontendVersionMajor;
		public readonly UInt16 FrontendVersionMinor;
		public readonly UInt16 FrontendVersionBuild;
		public readonly UInt16 BackendVersionMajor;
		public readonly UInt16 BackendVersionMinor;
		public readonly UInt16 BackendVersionBuild;
		public readonly string VersionString;
		public readonly string[] OptionalData;

		public S_COMPILE2(PDBFile pdb, Stream stream){
			var r = new SymbolDataReader(pdb, stream);
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
			while(r.HasMoreData) {
				string str = r.ReadSymbolString();
				if (str.Length == 0)
					break;
				optionalData.Add(str);
			}
			OptionalData = optionalData.ToArray();
		}

		public S_COMPILE2(CompileSym data) {
			Flags = data.Flags;
			Machine = data.Machine;
			FrontendVersionMajor = data.FrontendVersionMajor;
			FrontendVersionMinor = data.FrontentVersionMinor;
			FrontendVersionBuild = data.FrontendVersionBuild;
			BackendVersionMajor = data.BackendVersionMajor;
			BackendVersionMinor = data.BackendVersionMinor;
			BackendVersionBuild = data.BackendVersionBuild;
			VersionString = data.VersionString;
			OptionalData = data.OptionalData;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_COMPILE2);
			w.WriteEnum<CompileSym2FlagsEnum>((CompileSym2FlagsEnum)Flags);
			w.WriteUInt16(Machine);
			w.WriteUInt16(FrontendVersionMajor);
			w.WriteUInt16(FrontendVersionMinor);
			w.WriteUInt16(FrontendVersionBuild);
			w.WriteUInt16(BackendVersionMajor);
			w.WriteUInt16(BackendVersionMinor);
			w.WriteUInt16(BackendVersionBuild);
			w.WriteSymbolString(VersionString);

			foreach(string str in OptionalData) {
				w.WriteSymbolString(str);
			}

			w.WriteSymbolHeader();
		}
	}
}
