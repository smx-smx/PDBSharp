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
	public struct UsingNamespaceInstance
	{
		public UNAMESPACE Header;
		public string NamespaceName;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UNAMESPACE
	{
	}

	public class UsingNamespaceReader : SymbolReaderBase
	{
		public readonly UsingNamespaceInstance Data;

		public UsingNamespaceReader(Stream stream) : base(stream) {
			UNAMESPACE header = ReadStruct<UNAMESPACE>();
			string name = ReadSymbolString(Header);

			Data = new UsingNamespaceInstance() {
				Header = header,
				NamespaceName = name
			};
		}
	}
}
