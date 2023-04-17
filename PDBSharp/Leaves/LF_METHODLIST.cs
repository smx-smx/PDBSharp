#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
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

namespace Smx.PDBSharp.Leaves.LF_METHODLIST
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>Struct `mlMethod`</remarks>
	public class Data : ILeafData {
		public FieldAttributes Attributes { get; set; } = new FieldAttributes(0);
		public ILeafResolver? ProcedureTypeRecord { get; set; }
		public UInt32 VBaseOffset { get; set; }

		public Data(
			FieldAttributes attributes,
			ILeafResolver? procedureTypeRecord,
			UInt32 vBaseOffset
		) {
			this.Attributes = attributes;
			this.ProcedureTypeRecord = procedureTypeRecord;
			this.VBaseOffset = vBaseOffset;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public void Read() {
			TypeDataReader r = CreateReader();

			var Attributes = new FieldAttributes(r.ReadUInt16());
			var ProcedureTypeRecord = r.ReadIndexedType32Lazy();
			var VBaseOffset = new UInt32();

			switch (Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					VBaseOffset = r.ReadUInt32();
					break;
				default:
					VBaseOffset = 0;
					break;
			}

			Data = new Data(
				attributes: Attributes,
				procedureTypeRecord: ProcedureTypeRecord,
				vBaseOffset: VBaseOffset);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_METHODLIST);
			w.WriteUInt16((ushort)data.Attributes);
			w.WriteIndexedType(data.ProcedureTypeRecord);
			switch (data.Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					w.WriteUInt32(data.VBaseOffset);
					break;
			}
			w.WriteHeader();
		}

		public override string ToString() {
			var data = Data;
			return $"LF_METHODLIST[Attributes='{data?.Attributes}'" +
				$", ProcedureTypeRecord='{data?.ProcedureTypeRecord}'" +
				$", VBaseOffset='{data?.VBaseOffset}']";
		}

		
	}
}
