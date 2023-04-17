#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_TRAMPOLINE
{
	public class Data : ISymbolData {
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

		public Data(TrampolineType trampolineType, ushort thunkSize, uint thunkOffset, uint targetOffset, ushort thunkSection, ushort targetSection) {
			TrampolineType = trampolineType;
			ThunkSize = thunkSize;
			ThunkOffset = thunkOffset;
			TargetOffset = targetOffset;
			ThunkSection = thunkSection;
			TargetSection = targetSection;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }
		public ISymbolData? GetData() => Data;

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();

			var TrampolineType = r.ReadEnum<TrampolineType>();
			var ThunkSize = r.ReadUInt16();
			var ThunkOffset = r.ReadUInt32();
			var TargetOffset = r.ReadUInt32();
			var ThunkSection = r.ReadUInt16();
			var TargetSection = r.ReadUInt16();

			Data = new Data(
				trampolineType: TrampolineType,
				thunkSize: ThunkSize,
				thunkOffset: ThunkOffset,
				targetOffset: TargetOffset,
				thunkSection: ThunkSection,
				targetSection: TargetSection
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();
			
			var w = CreateWriter(SymbolType.S_TRAMPOLINE);
			w.Write<TrampolineType>(data.TrampolineType);
			w.WriteUInt16(data.ThunkSize);
			w.WriteUInt32(data.ThunkOffset);
			w.WriteUInt32(data.TargetOffset);
			w.WriteUInt16(data.ThunkSection);
			w.WriteUInt16(data.TargetSection);
			w.WriteHeader();
		}
	}
}
