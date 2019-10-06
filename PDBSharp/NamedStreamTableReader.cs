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
using System.ComponentModel.Design;
using System.Text;

namespace Smx.PDBSharp
{
	public class NamedStreamTableReader
	{
		private readonly StreamTableReader streamTable;
		private readonly PdbStreamReader streamReader;

		public NamedStreamTableReader(IServiceContainer ctx) {
			streamReader = ctx.GetService<PdbStreamReader>();
			streamTable = ctx.GetService<StreamTableReader>();
		}

		public byte[] GetStreamByName(string streamName) {
			if (streamReader == null)
				return null;

			if (!streamReader
				.NameTable
				.GetIndex(streamName, out uint streamNumber)
			) {
				return null;
			}

			return streamTable.GetStream((int)streamNumber);
		}
	}
}
