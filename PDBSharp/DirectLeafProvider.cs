#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Smx.PDBSharp.Leaves;

namespace Smx.PDBSharp
{
	public class DirectLeafProvider : ILeaf
	{
		private readonly LeafType type;
		private readonly ILeafData data;

		public DirectLeafProvider(LeafType type, ILeafData data) {
			this.type = type;
			this.data = data;
		}

		public LeafType Type => type;
		public ILeafData Data => data;
	}
}
