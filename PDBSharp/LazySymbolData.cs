#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System.Diagnostics;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp;

public class LazySymbolData : ISymbolResolver
{
	private readonly ILazy<ISymbolResolver?> lazy;

	public SymbolContext Data {
		get {
			Debug.Assert(lazy.Value != null);
			return lazy.Value.Data;
		}
	}

	public LazySymbolData(ILazy<ISymbolResolver?> lazy) {
		this.lazy = lazy;
	}
}