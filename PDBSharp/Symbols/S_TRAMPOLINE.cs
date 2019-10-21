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
	public class S_TRAMPOLINE : SymbolBase
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

		public S_TRAMPOLINE(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
			
		}

		public override void Read() {
			var r = CreateReader();

			TrampolineType = r.ReadEnum<TrampolineType>();
			ThunkSize = r.ReadUInt16();
			ThunkOffset = r.ReadUInt32();
			TargetOffset = r.ReadUInt32();
			ThunkSection = r.ReadUInt16();
			TargetSection = r.ReadUInt16();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_TRAMPOLINE);
			w.Write<TrampolineType>(TrampolineType);
			w.WriteUInt16(ThunkSize);
			w.WriteUInt32(ThunkOffset);
			w.WriteUInt32(TargetOffset);
			w.WriteUInt16(ThunkSection);
			w.WriteUInt16(TargetSection);

			w.WriteHeader();
		}
	}
}
