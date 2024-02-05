#region License
/*
 * Copyright (C) 2024 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.PE;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Smx.PDBSharp
{
	public class PDBPeSession {
		private PEFile pe;
		private readonly DBI.Data dbi;
		public PDBPeSession(IServiceContainer sc, string peFilePath) {
			this.pe = PEFile.Open(peFilePath);
			this.dbi = sc.GetService<DBI.Data>();
		}

		public bool TryGetSymbolByName(string name, out uint virtualAddress) {
			// $FIXME: hashing / provide alternative indexing
			foreach (var mod in dbi.Modules) {
				if (mod == null || mod.Module == null) continue;
				var sym = mod.Module.Symbols.SingleOrDefault(
					s => s.Data.Data is Symbols.ProcSym32.Data data && data.Name == name)
					.Data.Data as Symbols.ProcSym32.Data;
				if (sym == null) continue;

				
				var section = pe.NtHeaders.SectionHeaders.ElementAtOrDefault(sym.Segment - 1);
				if (section == null) continue;

				virtualAddress = section.VirtualAddress + sym.Offset;
				return true;
			}
			virtualAddress = 0;
			return false;
		}
	}

	public class PDBFacade : IPDBService
	{
		private readonly IServiceContainer sc;
		public PDBFacade(IServiceContainer sc) {
			this.sc = sc;
		}

		public PDBPeSession OpenPE(string peFilePath) {
			return new PDBPeSession(sc, peFilePath);
		}
	}
}
