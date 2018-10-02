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
using System.Text;
using System.Threading.Tasks;
using Smx.PDBSharp.Symbols.Structures;

namespace Smx.PDBSharp.Thunks
{
	[ThunkReader(ThunkType.NOTYPE)]
	public class NOTYPE : ThunkReaderBase, IThunk
	{
		public NOTYPE(SymbolHeader symHeader, THUNKSYM32 thunk, Stream stream) : base(symHeader, thunk, stream) {
		}

		THUNKSYM32 IThunk.Thunk => this.Thunk;
	}
}
