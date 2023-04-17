#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.IO;

namespace Smx.PDBSharp
{
	public class DBIWriter
	{
		private readonly PDBFile pdb;

		public DBIWriter(PDBFile pdb, StreamTableWriter stWr, Stream stream) {
			this.pdb = pdb;
		}

		public void WriteModule(ModuleInfo mod) {

		}
	}
}
