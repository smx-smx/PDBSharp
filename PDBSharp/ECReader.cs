#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System.IO;

namespace Smx.PDBSharp
{
	public class ECReader : SpanStream
	{
		public readonly NameTable.Data NameTable;

		public ECReader(SpanStream stream) : base(stream) {
			NameTable = Deserializers.ReadNameTable(this);
		}
	}
}
