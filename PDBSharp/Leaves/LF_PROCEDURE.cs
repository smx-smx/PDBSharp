#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_PROCEDURE
{
	public class Data : ILeafData {
		public ILeafResolver? ReturnValueType { get; set; }
		public CallingConvention CallingConvention { get; set; }
		public UInt16 NumberOfParameters { get; set; }
		public ILeafResolver? ArgumentListType { get; set; }

		public Data(ILeafResolver? returnValueType, CallingConvention callingConvention, ushort numberOfParameters, ILeafResolver? argumentListType) {
			ReturnValueType = returnValueType;
			CallingConvention = callingConvention;
			NumberOfParameters = numberOfParameters;
			ArgumentListType = argumentListType;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public void Read() {
			TypeDataReader r = CreateReader();

			var ReturnValueType = r.ReadIndexedType32Lazy();
			var CallingConvention = r.ReadEnum<CallingConvention>();
			r.ReadByte(); //reserved
			var NumberOfParameters = r.ReadUInt16();
			var ArgumentListType = r.ReadIndexedType32Lazy();

			Data = new Data(
				returnValueType: ReturnValueType,
				callingConvention: CallingConvention,
				numberOfParameters: NumberOfParameters,
				argumentListType: ArgumentListType
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_PROCEDURE);
			w.WriteIndexedType(data.ReturnValueType);
			w.Write<CallingConvention>(data.CallingConvention);
			w.WriteByte(0x00);
			w.WriteUInt16(data.NumberOfParameters);
			w.WriteIndexedType(data.ArgumentListType);
			w.WriteHeader();
		}
	}
}
