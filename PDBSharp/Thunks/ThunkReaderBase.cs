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

namespace Smx.PDBSharp.Thunks
{
	public class ThunkReaderBase : ReaderBase
	{
		protected readonly THUNKSYM32 Thunk;
		public readonly SymbolHeader Header;

		public ThunkReaderBase(SymbolHeader header, THUNKSYM32 thunkSym, Stream stream) : base(stream) {
			this.Header = header;
			this.Thunk = thunkSym;
		}
	}
}
