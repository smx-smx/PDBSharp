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
	[Flags]
	public enum CompileSymFlags
	{
		CompiledForEC = 1 << 8,
		NoDebugInfo = 1 << 9,
		HasLTCG = 1 << 10,
		NoDataAlign = 1 << 11,
		IsManaged = 1 << 12,
		HasSecurityChecks = 1 << 13,
		HasHotPatch = 1 << 14,
		ConvertedWithCVTCIL = 1 << 15,
		IsMSILMModule = 1 << 16
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct COMPILESYM
	{
		private UInt32 flags;
		public UInt16 Machine;

		public UInt16 FrontendMajorVersion;
		public UInt16 FrontendMinorVersion;
		public UInt16 FrontendBuildVersion;

		public UInt16 BackendMajorVersion;
		public UInt16 BackendMinorVersion;
		public UInt16 BackendBuildVersion;

		public CompileSymFlags Flags => (CompileSymFlags)flags;
		public int LanguageIndex => (int)(flags & 8);
	}

	public struct CompileSymInstance
	{
		public COMPILESYM Header;
		public string VersionString;
	}

	public class CompileSymReader : SymbolReaderBase
	{
		public readonly CompileSymInstance Data;
		public CompileSymReader(Stream stream) : base(stream) {
			COMPILESYM header = ReadStruct<COMPILESYM>();
			string versionString = ReadSymbolString(Header);

			Data = new CompileSymInstance() {
				Header = header,
				VersionString = versionString
			};
		}
	}
}
