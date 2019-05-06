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
	[LeafReader(LeafType.LF_METHODLIST)]
	public class LF_METHODLIST : TypeDataReader
	{
		public readonly FieldAttributes Attributes;
		public readonly Lazy<ILeaf> ProcedureTypeRecord;

		public readonly UInt32 VBaseOffset;

		public LF_METHODLIST(PDBFile pdb, Stream stream) : base(pdb, stream) {
			Attributes = new FieldAttributes(ReadUInt16());
			ProcedureTypeRecord = ReadIndexedTypeLazy();

			switch (Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					VBaseOffset = ReadUInt32();
					break;
				default:
					VBaseOffset = 0;
					break;
			}

		}
	}
}
