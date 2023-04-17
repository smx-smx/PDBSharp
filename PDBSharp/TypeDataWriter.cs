#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp
{
	public class TypeDataWriter : SpanStream
	{
		protected readonly IServiceContainer ctx;

		private readonly LeafType type;
		private readonly bool hasSize;

		protected TypeDataWriter(
			IServiceContainer ctx, SpanStream stream
		) : base(stream){
			this.ctx = ctx;
		}

		public TypeDataWriter(
			IServiceContainer ctx, SpanStream stream,
			LeafType type, bool hasSize = true
		) : base(stream) {
			this.ctx = ctx;
			this.type = type;
			this.hasSize = hasSize;
		}

		public void WriteIndexedType(ILeafResolver? leaf) {
			if (leaf == null) throw new ArgumentNullException(nameof(leaf));
			var ctx = leaf.Ctx;
			if (ctx == null) throw new InvalidOperationException();
			
			WriteUInt32(ctx.TypeIndex);
			ctx.CreateSerializer(this).Write();
		}

		public void WriteIndexedType16(ILeafResolver? leaf) {
			if (leaf == null) throw new ArgumentNullException(nameof(leaf));
			var ctx = leaf.Ctx;
			if(ctx == null) throw new InvalidOperationException();
			WriteUInt16((ushort)ctx.TypeIndex);
			ctx.CreateSerializer(this).Write();
		}

		public void WriteVaryingType(ILeafResolver? leaf) {
			if (leaf == null) throw new ArgumentNullException(nameof(leaf));
			var ctx = leaf.Ctx;
			if (ctx == null) throw new InvalidOperationException();
			
			if ((ushort)ctx.Type < (ushort)LeafType.LF_NUMERIC) {
				throw new NotImplementedException();
			}

			ctx.CreateSerializer(this).Write();
		}

		public void WriteHeader() {
			if (hasSize) {
				WriteUInt16((ushort)this.Length);
			}
			Write<LeafType>(this.type);
		}

		public void WriteShortString(string str) {
			WriteUInt16((ushort)str.Length);
			WriteBytes(Encoding.ASCII.GetBytes(str));
		}
	}
}