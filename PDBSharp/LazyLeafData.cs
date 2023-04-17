#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System.ComponentModel.Design;
using System.Diagnostics;
using Smx.PDBSharp.LeafResolver;
using Smx.PDBSharp.Leaves;

namespace Smx.PDBSharp;

public class LazyLeafData : ILeafResolver
{
	private readonly ILazy<ILeafResolver?> lazy;

	public LeafContext Ctx {
		get {
			Debug.Assert(lazy.Value != null);
			return lazy.Value.Ctx;
		}
	}

	public LazyLeafData(ILazy<ILeafResolver?> lazyProvider) {
		lazy = lazyProvider;
	}
}