#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smx.PDBSharp.Symbols.Structures;

namespace Smx.PDBSharp.Thunks
{
	public class NOTYPE : SymbolDataReader, IThunk
	{
		public NOTYPE(Context pdb, SymbolHeader symHeader, Stream stream) : base(pdb, symHeader, stream) {
		}

		public void Write(SymbolDataWriter w) {
		}
	}
}
