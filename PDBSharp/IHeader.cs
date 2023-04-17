#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.PDBSharp
{
	public interface IHeader
	{
		string Magic { get; set; }
		UInt32 PageSize { get; set; }
		UInt32 FpmPageNumber { get; set; }
		UInt32 NumPages { get; set; }
		UInt32 DirectorySize { get; set; }
	}
}
