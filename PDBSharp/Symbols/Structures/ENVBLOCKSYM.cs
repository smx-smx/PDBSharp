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
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ENVBLOCKSYM
	{
		public SymbolHeader Header;
		private byte flags;
	}

	public struct EnvBlockSymInstance
	{
		public ENVBLOCKSYM Header;
		public List<string> Environment;
	}

	public class EnvBlockSymReader : ReaderBase
	{
		public readonly EnvBlockSymInstance Data;
		public EnvBlockSymReader(Stream stream) : base(stream) {

			ENVBLOCKSYM header = ReadStruct<ENVBLOCKSYM>();
			List<string> env = new List<string>();

			while(Stream.Position < stream.Length) {
				string envStr = ReadSymbolString(header.Header);
				if(string.IsNullOrEmpty(envStr))
					break;

				env.Add(envStr);
			}

			Data = new EnvBlockSymInstance() {
				Header = header,
				Environment = env
			};
		}
	}
}
