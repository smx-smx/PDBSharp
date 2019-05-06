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

	[LeafReader(LeafType.LF_MFUNCTION)]
	public class LF_MFUNCTION : TypeDataReader
	{
		public readonly Lazy<ILeaf> ReturnValueType;
		public readonly Lazy<ILeaf> ContainingClassType;
		public readonly Lazy<ILeaf> ThisPointerType;
		public readonly CallingConvention CallingConvention;
		public readonly UInt16 NumberOfParameters;
		public readonly Lazy<ILeaf> ArgumentListType;
		public readonly UInt32 ThisAdjustor;

		public readonly FunctionAttributes Attributes;

		public LF_MFUNCTION(PDBFile pdb, Stream stream) : base(pdb, stream) {
			ReturnValueType = ReadIndexedTypeLazy();
			ContainingClassType = ReadIndexedTypeLazy();
			ThisPointerType = ReadIndexedTypeLazy();
			CallingConvention = ReadEnum<CallingConvention>();
			Attributes = ReadFlagsEnum<FunctionAttributes>();
			NumberOfParameters = ReadUInt16();
			ArgumentListType = ReadIndexedTypeLazy();
			ThisAdjustor = ReadUInt32();
		}
	}
}
