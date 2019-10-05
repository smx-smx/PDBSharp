#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Smx.PDBSharp.Codegen
{
	public class ModuleDependencyGraphBuilder
	{
		private readonly Context ctx;
		private readonly IModuleContainer module;

		private readonly IList<SymbolNode> nodes = new List<SymbolNode>();

		public ModuleDependencyGraphBuilder(Context ctx, IModuleContainer module) {
			this.ctx = ctx;
			this.module = module;
		}

		private SymbolNode AddSymbolNode(Symbol sym) {
			SymbolNode node = new SymbolNode(sym);
			nodes.Add(node);
			return node;
		}

		public IList<SymbolNode> BuildTree() {
			if (!(module.Module is CodeViewModuleReader cvm))
				return null;

			foreach (var sym in cvm.Symbols) {
				SymbolNode node;
				switch (sym.Data) {
					case S_GPROC32 gproc:
						node = AddSymbolNode(sym);
						node.AddDependency(gproc.Type);
						break;
					case S_GDATA32 gdata:
						node = AddSymbolNode(sym);
						node.AddDependency(gdata.Type);
						break;
					default:
						break;
				}
			}

			return nodes;
		}
	}
}
