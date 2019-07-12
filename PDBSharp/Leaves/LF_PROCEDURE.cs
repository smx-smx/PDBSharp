#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	public class LF_PROCEDURE : ILeaf
	{
		public readonly ILeafContainer ReturnValueType;
		public readonly CallingConvention CallingConvention;
		public readonly UInt16 NumberOfParameters;
		public readonly ILeafContainer ArgumentListType;

		public LF_PROCEDURE(PDBFile pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			ReturnValueType = r.ReadIndexedTypeLazy();
			CallingConvention = r.ReadEnum<CallingConvention>();
			r.ReadByte(); //reserved
			NumberOfParameters = r.ReadUInt16();
			ArgumentListType = r.ReadIndexedTypeLazy();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_PROCEDURE);
			w.WriteIndexedType(ReturnValueType);
			w.WriteEnum<CallingConvention>(CallingConvention);
			w.WriteByte(0x00);
			w.WriteUInt16(NumberOfParameters);
			w.WriteIndexedType(ArgumentListType);
			w.WriteLeafHeader();
		}
	}
}
