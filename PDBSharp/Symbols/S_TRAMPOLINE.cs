#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class TrampolineSym
	{
		/// <summary>
		/// trampoline sym subtype
		/// </summary>
		public TrampolineType TrampolineType { get; set; }
		/// <summary>
		/// size of the thunk
		/// </summary>
		public UInt16 ThunkSize { get; set; }
		/// <summary>
		/// offset of the thunk
		/// </summary>
		public UInt32 ThunkOffset { get; set; }
		/// <summary>
		/// offset of the target of the thunk
		/// </summary>
		public UInt32 TargetOffset { get; set; }
		/// <summary>
		/// section index of the thunk
		/// </summary>
		public UInt16 ThunkSection { get; set; }
		/// <summary>
		/// section index of the target of the thunk
		/// </summary>
		public UInt16 TargetSection { get; set; }
	}

	public class S_TRAMPOLINE : ISymbol
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

		public S_TRAMPOLINE(IServiceContainer ctx, IModule mod, ReaderSpan stream) {
			var r = new SymbolDataReader(ctx, stream);

			TrampolineType = r.ReadEnum<TrampolineType>();
			ThunkSize = r.ReadUInt16();
			ThunkOffset = r.ReadUInt32();
			TargetOffset = r.ReadUInt32();
			ThunkSection = r.ReadUInt16();
			TargetSection = r.ReadUInt16();
		}

		public S_TRAMPOLINE(TrampolineSym data) {
			TrampolineType = data.TrampolineType;
			ThunkSize = data.ThunkSize;
			ThunkOffset = data.ThunkOffset;
			TargetOffset = data.TargetOffset;
			ThunkSection = data.ThunkSection;
			TargetSection = data.TargetSection;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_TRAMPOLINE);
			w.WriteEnum<TrampolineType>(TrampolineType);
			w.WriteUInt16(ThunkSize);
			w.WriteUInt32(ThunkOffset);
			w.WriteUInt32(TargetOffset);
			w.WriteUInt16(ThunkSection);
			w.WriteUInt16(TargetSection);

			w.WriteSymbolHeader();
		}
	}
}
