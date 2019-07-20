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
	public class DirectLeafProvider : LeafContainerBase
	{
		private readonly uint typeIndex;
		private readonly LeafType type;
		private readonly ILeaf data;

		public DirectLeafProvider(uint typeIndex, LeafType type, ILeaf data) {
			this.typeIndex = typeIndex;
			this.type = type;
			this.data = data;
		}

		public override uint TypeIndex => typeIndex;

		public override LeafType Type => type;

		public override ILeaf Data => data;

		public override void Write(PDBFile pdb, Stream stream) {
			data.Write(pdb, stream);
		}
	}
}
