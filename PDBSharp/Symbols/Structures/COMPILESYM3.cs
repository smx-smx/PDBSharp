#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols.Structures
{
	public struct CompileSym3Instance
	{
		public COMPILESYM3 Header;
		public string VersionString;
	}

	[Flags]
	public enum CompileSym3Flags
	{
		CompiledForEC = 1 << 8,
		NoDebugInfo = 1 << 9,
		HasLTCG = 1 << 10,
		NoDataAlign = 1 << 11,
		IsManaged = 1 << 12,
		HasSecurityChecks = 1 << 13,
		HasHotPatch = 1 << 14,
		ConvertedWithCVTCIL = 1 << 15,
		IsMSILMModule = 1 << 16,
		HasSDL = 1 << 17,
		HasPGO = 1 << 18,
		IsExpModule = 1 << 19
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct COMPILESYM3
	{
		private UInt32 flags;
		public UInt16 Machine;

		public UInt16 FrontendMajorVersion;
		public UInt16 FrontendMinorVersion;
		public UInt16 FrontendBuildVersion;
		public UInt16 FrontendQFEVersion;

		public UInt16 BackendMajorVersion;
		public UInt16 BackendMinorVersion;
		public UInt16 BackendBuildVersion;
		public UInt16 BackendQFEVersion;

		public int LanguageIndex => (int)(flags & 8);
		public CompileSym3Flags Flags => (CompileSym3Flags)flags;

	}

	public class CompileSym3Reader : SymbolReaderBase
	{
		public readonly CompileSym3Instance Data;
		public CompileSym3Reader(Stream stream) : base(stream) {
			COMPILESYM3 header = ReadStruct<COMPILESYM3>();
			string versionString = ReadSymbolString(Header);

			Data = new CompileSym3Instance() {
				Header = header,
				VersionString = versionString
			};
		}
	}
}
