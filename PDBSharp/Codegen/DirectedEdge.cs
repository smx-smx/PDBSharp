#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion

namespace Smx.PDBSharp.Codegen
{
	public class DirectedEdge
	{
		public readonly INode source;
		public readonly INode dest;
		public DirectedEdge(INode source, INode dest) {
			this.source = source;
			this.dest = dest;
		}
	}
}
