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

	public class LF_MFUNCTION : LeafBase
	{
		public ILeafContainer ReturnValueType { get; set; }
		public ILeafContainer ContainingClassType { get; set; }
		public ILeafContainer ThisPointerType { get; set; }
		public CallingConvention CallingConvention { get; set; }
		public UInt16 NumberOfParameters { get; set; }
		public ILeafContainer ArgumentListType { get; set; }
		public UInt32 ThisAdjustor { get; set; }

		public FunctionAttributes Attributes { get; set; }

		public LF_MFUNCTION(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			ReturnValueType = r.ReadIndexedType32Lazy();
			ContainingClassType = r.ReadIndexedType32Lazy();
			ThisPointerType = r.ReadIndexedType32Lazy();
			CallingConvention = r.ReadEnum<CallingConvention>();
			Attributes = r.ReadFlagsEnum<FunctionAttributes>();
			NumberOfParameters = r.ReadUInt16();
			ArgumentListType = r.ReadIndexedType32Lazy();
			ThisAdjustor = r.ReadUInt32();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_MFUNCTION);
			w.WriteIndexedType(ReturnValueType);
			w.WriteIndexedType(ContainingClassType);
			w.WriteIndexedType(ThisPointerType);
			w.Write<CallingConvention>(CallingConvention);
			w.Write<FunctionAttributes>(Attributes);
			w.WriteUInt16(NumberOfParameters);
			w.WriteIndexedType(ArgumentListType);
			w.WriteUInt32(ThisAdjustor);
			w.WriteHeader();
		}

		public override string ToString() {
			return $"LF_MFUNCTION[ReturnValueType='{ReturnValueType}', " +
				$"ContainingClassType='{ContainingClassType}', " +
				$"ThisPointerType='{ThisPointerType}', " +
				$"CallingConvention='{CallingConvention}', " +
				$"Attributes='{Attributes}', " +
				$"NumberOfParameters='{NumberOfParameters}', " +
				$"ArgumentListType='{ArgumentListType}', " +
				$"ThisAdjustor='{ThisAdjustor}']";
		}
	}
}
