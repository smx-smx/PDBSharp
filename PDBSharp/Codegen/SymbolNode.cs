#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.Collections.Generic;

namespace Smx.PDBSharp.Codegen
{
	public class SymbolNode : INode
	{
		public IList<TypeNode> TypeDependencies = new List<TypeNode>();
		public bool Visited { get; set; }

		public readonly SymbolContext Symbol;

		public SymbolNode(SymbolContext sym) {
			this.Symbol = sym;
		}

		public TypeNode AddDependency(LeafContext type) {
			TypeNode node = new TypeNode(type);
			TypeDependencies.Add(node);
			return node;
		}
	}
}
