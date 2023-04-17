#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion

using System;
using Smx.PDBSharp.Leaves;
using System.IO;
using Smx.SharpIO;

namespace Smx.PDBSharp
{
	public class LeafContext
	{
		public ILeafData? Data { get; }
		public uint TypeIndex { get; }
		public LeafType Type { get; }

		public LeafContext(uint typeIndex, LeafType type, ILeafData? data) {
			Data = data;
			TypeIndex = typeIndex;
			Type = type;
		}

		public ILeafSerializer CreateSerializer(SpanStream stream) {
			switch (Type) {
				
			}
			throw new NotImplementedException();
		}
	}
}
