#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System.IO;

namespace Smx.PDBSharp
{
	public abstract class LeafContainerBase : ILeafContainer, ILeaf
	{
		public abstract uint TypeIndex { get; }
		public abstract LeafType Type { get; }
		public abstract ILeaf Data { get; }

		public abstract void Read();
		public abstract void Write();
	}
}
