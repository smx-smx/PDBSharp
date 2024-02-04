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
using System.ComponentModel.Design;
using System.Text;

namespace Smx.PDBSharp
{
	public class NamedStreamTableReader : IPDBService
	{
		private readonly StreamTable.Serializer streamTable;
		private readonly PDBStream.Data pdbStream;

		public NamedStreamTableReader(IServiceContainer ctx) {
			pdbStream = ctx.GetService<PDBStream.Data>();
			streamTable = ctx.GetService<StreamTable.Serializer>();
		}

		public byte[]? GetStreamByName(string streamName) {
			uint streamNumber = 0;
			var res = pdbStream.NameTable?.GetIndex(streamName, out streamNumber) ?? false;
			if (!res) {
				return null;
			}

			return streamTable?.GetStream((int)streamNumber);
		}
	}
}
