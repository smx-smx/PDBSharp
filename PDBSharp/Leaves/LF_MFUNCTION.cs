#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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

	public class LF_MFUNCTION : ILeaf
	{
		public readonly ILeafContainer ReturnValueType;
		public readonly ILeafContainer ContainingClassType;
		public readonly ILeafContainer ThisPointerType;
		public readonly CallingConvention CallingConvention;
		public readonly UInt16 NumberOfParameters;
		public readonly ILeafContainer ArgumentListType;
		public readonly UInt32 ThisAdjustor;

		public readonly FunctionAttributes Attributes;

		public LF_MFUNCTION(PDBFile pdb, Stream stream) {
			TypeDataReader r = new TypeDataReader(pdb, stream);

			ReturnValueType = r.ReadIndexedTypeLazy();
			ContainingClassType = r.ReadIndexedTypeLazy();
			ThisPointerType = r.ReadIndexedTypeLazy();
			CallingConvention = r.ReadEnum<CallingConvention>();
			Attributes = r.ReadFlagsEnum<FunctionAttributes>();
			NumberOfParameters = r.ReadUInt16();
			ArgumentListType = r.ReadIndexedTypeLazy();
			ThisAdjustor = r.ReadUInt32();
		}

		public void Write(PDBFile pdb, Stream stream) {
			TypeDataWriter w = new TypeDataWriter(pdb, stream, LeafType.LF_MFUNCTION);
			w.WriteIndexedType(ReturnValueType);
			w.WriteIndexedType(ContainingClassType);
			w.WriteIndexedType(ThisPointerType);
			w.WriteEnum<CallingConvention>(CallingConvention);
			w.WriteEnum<FunctionAttributes>(Attributes);
			w.WriteUInt16(NumberOfParameters);
			w.WriteIndexedType(ArgumentListType);
			w.WriteUInt32(ThisAdjustor);
			w.WriteLeafHeader();
		}
	}
}
