#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Smx.PDBSharp.Codegen
{
	public class GraphBuilder
	{
		private readonly DBIReader Dbi;

		public GraphBuilder(IServiceContainer ctx) {
			this.Dbi = ctx.GetService<DBIReader>();
		}

		private IEnumerable<SymbolNode> BuildGraph(IModuleContainer module) {
			return new ModuleDependencyGraphBuilder(module).BuildTree();
		}

		public IEnumerable<SymbolNode> Build() {
			IEnumerable<SymbolNode> tree = Dbi.Modules
				?.Select(mod => BuildGraph(mod))
				//skip modules without graph
				.Where(graph => graph != null)
				.SelectMany(graph => graph)
				.OrderBy(node => node.TypeDependencies.Count)
				.ToList();
			return tree;
		}
	}
}
