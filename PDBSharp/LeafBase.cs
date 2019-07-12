#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Smx.PDBSharp.Leaves;

namespace Smx.PDBSharp
{
	public abstract class LeafBase : ILeaf, ILeafContainer
	{
		public abstract uint TypeIndex { get; }
		public abstract LeafType Type { get; }
		public abstract ILeaf Data { get; }

		public abstract void Write(PDBFile pdb, Stream stream);
	}
}
