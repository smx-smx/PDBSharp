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

namespace Smx.PDBSharp.Leaves.LF_MFUNCTION
{

	public class Data : ILeafData {
		public ILeafResolver? ReturnValueType { get; set; }
		public ILeafResolver? ContainingClassType { get; set; }
		public ILeafResolver? ThisPointerType { get; set; }
		public CallingConvention CallingConvention { get; set; }
		public UInt16 NumberOfParameters { get; set; }
		public ILeafResolver? ArgumentListType { get; set; }
		public UInt32 ThisAdjustor { get; set; }

		public FunctionAttributes Attributes { get; set; }

		public Data(ILeafResolver? returnValueType, ILeafResolver? containingClassType, ILeafResolver? thisPointerType, CallingConvention callingConvention, ushort numberOfParameters, ILeafResolver? argumentListType, uint thisAdjustor, FunctionAttributes attributes) {
			ReturnValueType = returnValueType;
			ContainingClassType = containingClassType;
			ThisPointerType = thisPointerType;
			CallingConvention = callingConvention;
			NumberOfParameters = numberOfParameters;
			ArgumentListType = argumentListType;
			ThisAdjustor = thisAdjustor;
			Attributes = attributes;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer {
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public long Read() {
			TypeDataReader r = CreateReader();

			var ReturnValueType = r.ReadIndexedType32Lazy();
			var ContainingClassType = r.ReadIndexedType32Lazy();
			var ThisPointerType = r.ReadIndexedType32Lazy();
			var CallingConvention = r.ReadEnum<CallingConvention>();
			var Attributes = r.ReadFlagsEnum<FunctionAttributes>();
			var NumberOfParameters = r.ReadUInt16();
			var ArgumentListType = r.ReadIndexedType32Lazy();
			var ThisAdjustor = r.ReadUInt32();
			Data = new Data(
				returnValueType: ReturnValueType,
				containingClassType: ContainingClassType,
				thisPointerType: ThisPointerType,
				callingConvention: CallingConvention,
				attributes: Attributes,
				numberOfParameters: NumberOfParameters,
				argumentListType: ArgumentListType,
				thisAdjustor: ThisAdjustor
			);
			
			return r.Position;
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_MFUNCTION);
			w.WriteIndexedType(data.ReturnValueType);
			w.WriteIndexedType(data.ContainingClassType);
			w.WriteIndexedType(data.ThisPointerType);
			w.Write<CallingConvention>(data.CallingConvention);
			w.Write<FunctionAttributes>(data.Attributes);
			w.WriteUInt16(data.NumberOfParameters);
			w.WriteIndexedType(data.ArgumentListType);
			w.WriteUInt32(data.ThisAdjustor);
			w.WriteHeader();
		}

		public override string ToString() {
			var data = Data;
			return $"LF_MFUNCTION(ReturnValueType='{data?.ReturnValueType}', " +
				$"ContainingClassType='{data?.ContainingClassType}', " +
				$"ThisPointerType='{data?.ThisPointerType}', " +
				$"CallingConvention='{data?.CallingConvention}', " +
				$"Attributes='{data?.Attributes}', " +
				$"NumberOfParameters='{data?.NumberOfParameters}', " +
				$"ArgumentListType='{data?.ArgumentListType}', " +
				$"ThisAdjustor='{data?.ThisAdjustor}')";
		}
	}
}
