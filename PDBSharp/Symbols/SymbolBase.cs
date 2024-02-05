#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Symbols
{
	public abstract class SymbolSerializerBase
	{
		protected readonly IServiceContainer ctx;
		protected readonly SpanStream stream;
		protected readonly IModule? moduleStream;

		public SymbolSerializerBase(IServiceContainer ctx, SpanStream stream, IModule? moduleStream = null) {
			this.ctx = ctx;
			this.stream = stream;
			this.moduleStream = moduleStream;
		}

		protected SymbolData.Reader CreateReader() {
			var r = new SymbolData.Reader(ctx, stream, moduleStream);
			r.Initialize();
			return r;
		}

		protected SymbolDataWriter CreateWriter(SymbolType type) {
			return new SymbolDataWriter(ctx, stream, type);
		}
	}
}
