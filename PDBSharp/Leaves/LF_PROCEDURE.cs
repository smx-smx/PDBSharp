#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_PROCEDURE : LeafBase
	{
		public ILeafContainer ReturnValueType { get; set; }
		public CallingConvention CallingConvention { get; set; }
		public UInt16 NumberOfParameters { get; set; }
		public ILeafContainer ArgumentListType { get; set; }

		public LF_PROCEDURE(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			ReturnValueType = r.ReadIndexedTypeLazy();
			CallingConvention = r.ReadEnum<CallingConvention>();
			r.ReadByte(); //reserved
			NumberOfParameters = r.ReadUInt16();
			ArgumentListType = r.ReadIndexedTypeLazy();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_PROCEDURE);
			w.WriteIndexedType(ReturnValueType);
			w.Write<CallingConvention>(CallingConvention);
			w.WriteByte(0x00);
			w.WriteUInt16(NumberOfParameters);
			w.WriteIndexedType(ArgumentListType);
			w.WriteHeader();
		}
	}
}
