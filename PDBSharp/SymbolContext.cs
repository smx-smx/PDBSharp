#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using Smx.PDBSharp.Symbols;
using Smx.PDBSharp.Symbols.S_SEPCODE;
using Smx.SharpIO;

namespace Smx.PDBSharp;

public class SymbolContext
{
	
	public SymbolType Type { get;  }
	public ISymbolData? Data { get; }

	public SymbolContext(SymbolType type, ISymbolData? data) {
		Type = type;
		Data = data;
	}

	public ISymbolSerializer CreateSerializer(SpanStream stream) {
		throw new NotImplementedException();
	}

	public override string ToString() {
		return $"Symbol: {Data}";
	}
}