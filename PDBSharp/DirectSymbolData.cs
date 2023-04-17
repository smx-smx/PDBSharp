#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿namespace Smx.PDBSharp;

public class DirectSymbolData : ISymbolResolver
{
	public SymbolContext? Data { get; }

	public DirectSymbolData(SymbolContext? data) {
		Data = data;
	}

	public override string ToString() {
		return $"[DirectSymbolData]: {Data}";
	}
}