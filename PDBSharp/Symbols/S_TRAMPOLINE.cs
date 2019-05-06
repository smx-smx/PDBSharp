#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{
	[SymbolReader(SymbolType.S_TRAMPOLINE)]
	public class S_TRAMPOLINE : SymbolDataReader
	{
		/// <summary>
		/// trampoline sym subtype
		/// </summary>
		public readonly TrampolineType TrampolineType;
		/// <summary>
		/// size of the thunk
		/// </summary>
		public readonly UInt16 ThunkSize;
		/// <summary>
		/// offset of the thunk
		/// </summary>
		public readonly UInt32 ThunkOffset;
		/// <summary>
		/// offset of the target of the thunk
		/// </summary>
		public readonly UInt32 TargetOffset;
		/// <summary>
		/// section index of the thunk
		/// </summary>
		public readonly UInt16 ThunkSection;
		/// <summary>
		/// section index of the target of the thunk
		/// </summary>
		public readonly UInt16 TargetSection;

		public S_TRAMPOLINE(PDBFile pdb, Stream stream) : base(pdb, stream) {
			TrampolineType = ReadEnum<TrampolineType>();
			ThunkSize = ReadUInt16();
			ThunkOffset = ReadUInt32();
			TargetOffset = ReadUInt32();
			ThunkSection = ReadUInt16();
			TargetSection = ReadUInt16();
		}
	}
}
