#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System;
using System.IO;

namespace Smx.PDBSharp
{
	public class TypeDataWriter : WriterBase
	{
		protected readonly PDBFile PDB;

		private readonly LeafType type;

		public TypeDataWriter(PDBFile pdb, Stream stream, LeafType type) : base(stream) {
			this.PDB = pdb;
			this.type = type;
		}

		public void WriteIndexedType(ILeafContainer leaf) {
			WriteUInt32(leaf.TypeIndex);
			leaf.Data.Write(PDB, Stream);
		}

		public void WriteIndexedType16(ILeafContainer leaf) {
			WriteUInt16((ushort)leaf.TypeIndex);
			leaf.Data.Write(PDB, Stream);
		}

		public void WriteVaryingType(ILeafContainer leaf) {
			if((ushort)leaf.Type < (ushort)LeafType.LF_NUMERIC) {
				throw new NotImplementedException();
			}

			leaf.Data.Write(PDB, Stream);
		}

		public void WriteLeafHeader(bool hasSize = true) {
			if (hasSize) {
				WriteUInt16((ushort)Stream.Position);
			}
			WriteEnum<LeafType>(this.type);
		}
	}
}