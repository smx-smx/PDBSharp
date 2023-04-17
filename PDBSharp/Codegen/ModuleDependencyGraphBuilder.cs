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

namespace Smx.PDBSharp.Codegen
{
	public class ModuleDependencyGraphBuilder
	{
		private readonly IModuleContainer module;

		private readonly IList<SymbolNode> nodes = new List<SymbolNode>();

		public ModuleDependencyGraphBuilder(IModuleContainer module) {
			this.module = module;
		}

		private SymbolNode AddSymbolNode(SymbolContext sym) {
			SymbolNode node = new SymbolNode(sym);
			nodes.Add(node);
			return node;
		}

		public IList<SymbolNode> BuildTree() {
			if (!(module.Module is CodeViewModuleReader cvm))
				return nodes;

			foreach (var sym in cvm.Symbols) {
				SymbolNode node;
				switch (sym.Data?.Data) {
					case Symbols.ProcSym32.Data proc:
						node = AddSymbolNode(sym.Data);
						if (proc.Type != null) {
							node.AddDependency(proc.Type.Ctx);
						}

						break;
					case Symbols.DataSym32.Data data:
						node = AddSymbolNode(sym.Data);
						if (data.Type != null) {
							node.AddDependency(data.Type.Ctx);
						}

						break;
					default:
						//Console.Error.WriteLine($"Unsupported symbol type {sym.Type}");
						break;
				}
			}

			return nodes;
		}
	}
}
