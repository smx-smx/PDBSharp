#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_ALIAS : LeafBase
	{
		public ILeafContainer UnderlyingType { get; set; }
		public string Name { get; set; }

		public override string UdtName => Name;

		public override void Read() {
			TypeDataReader r = CreateReader();
			UnderlyingType = r.ReadIndexedType32Lazy();
			Name = r.ReadCString();
		}

		public LF_ALIAS(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			
		}

		public LF_ALIAS(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			
		}
	}
}
