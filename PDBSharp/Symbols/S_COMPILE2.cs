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
	[SymbolReader(SymbolType.S_COMPILE2)]
	public class S_COMPILE2 : SymbolDataReader
	{
		public readonly CompileSymFlags Flags;
		public readonly UInt16 Machine;
		public readonly UInt16 FrontendVersionMajor;
		public readonly UInt16 FrontendVersionMinor;
		public readonly UInt16 FrontendVersionBuild;
		public readonly UInt16 BackendVersionMajor;
		public readonly UInt16 BackendVersionMinor;
		public readonly UInt16 BackendVersionBuild;
		public readonly string VersionString;
		public readonly string[] OptionalData;

		public S_COMPILE2(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Flags = new CompileSymFlags(ReadUInt32());
			Machine = ReadUInt16();
			FrontendVersionMajor = ReadUInt16();
			FrontendVersionMinor = ReadUInt16();
			FrontendVersionBuild = ReadUInt16();
			BackendVersionMajor = ReadUInt16();
			BackendVersionMinor = ReadUInt16();
			BackendVersionBuild = ReadUInt16();
			VersionString = ReadSymbolString();

			List<string> optionalData = new List<string>();
			while(Stream.Position < Stream.Length) {
				string str = ReadSymbolString();
				if (str.Length == 0)
					break;
				optionalData.Add(str);
			}
			OptionalData = optionalData.ToArray();
		}
	}
}
