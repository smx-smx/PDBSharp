#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.Collections.Generic;

namespace Smx.PDBSharp
{
	public interface IModule
	{
		event OnSymbolDataDelegate OnSymbolData;
		IEnumerable<ISymbolResolver?> Symbols { get; }
	}
}