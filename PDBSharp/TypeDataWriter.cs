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
using System.Text;

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

		public void WriteIndexedType(ILeafContainer leaf) {
			WriteUInt32(leaf.TypeIndex);
			leaf.Data.Write();
		}

		public void WriteIndexedType16(ILeafContainer leaf) {
			WriteUInt16((ushort)leaf.TypeIndex);
			leaf.Data.Write();
		}

		public void WriteVaryingType(ILeafContainer leaf) {
			if ((ushort)leaf.Type < (ushort)LeafType.LF_NUMERIC) {
				throw new NotImplementedException();
			}

			leaf.Data.Write();
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