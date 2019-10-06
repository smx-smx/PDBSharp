#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp
{
	public class LazyLeafProvider : ILeafContainer
	{
		private readonly TypeResolver resolver;

		private ILeafContainer ReadLeaf() {
			return resolver.GetTypeByIndex(typeIndex);
		}

		public void Write(PDBFile pdb, Stream stream) {
			Leaf.Data.Write(pdb, stream);
		}

		private readonly Lazy<ILeafContainer> lazy;
		public ILeafContainer Leaf => lazy.Value;

		public uint TypeIndex => Leaf.TypeIndex;

		public LeafType Type => Leaf.Type;

		public ILeaf Data => Leaf?.Data;

		private readonly uint typeIndex;

		public LazyLeafProvider(IServiceContainer ctx, uint typeIndex) {
			this.resolver = ctx.GetService<TypeResolver>();

			this.typeIndex = typeIndex;
			lazy = new Lazy<ILeafContainer>(ReadLeaf);
		}

		public LazyLeafProvider(IServiceContainer ctx, Lazy<ILeafContainer> lazyProvider) {
			this.resolver = ctx.GetService<TypeResolver>();
			lazy = lazyProvider;
		}

		public override string ToString() {
			return Data?.ToString();
		}
	}
}
