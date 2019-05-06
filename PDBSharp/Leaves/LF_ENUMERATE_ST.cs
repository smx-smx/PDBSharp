#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	[LeafReader(LeafType.LF_ENUMERATE_ST)]
	public class LF_ENUMERATE_ST : LF_ENUMERATE
	{
		public LF_ENUMERATE_ST(PDBFile pdb, Stream stream) : base(pdb, stream) {
		}
	}
}
