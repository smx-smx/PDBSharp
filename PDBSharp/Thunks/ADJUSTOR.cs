#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Thunks
{
	public class ADJUSTOR : SymbolDataReader, IThunk
	{
		public readonly UInt16 Delta;
		public readonly string Name;

		public ADJUSTOR(PDBFile pdb, SymbolHeader symHeader, Stream stream) : base(pdb, symHeader, stream) {
			Delta = ReadUInt16();
			Name = ReadSymbolString();
		}

		public void Write(SymbolDataWriter w) {
			w.WriteUInt16(Delta);
			w.WriteSymbolString(Name);
		}
	}
}
