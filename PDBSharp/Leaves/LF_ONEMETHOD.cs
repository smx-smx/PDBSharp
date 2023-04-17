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

namespace Smx.PDBSharp.Leaves.LF_ONEMETHOD
{
	public class Data : ILeafData {
		public FieldAttributes Attributes { get; set; }
		public ILeafResolver? ProcedureType { get; set; }
		public UInt32 VBaseOffset { get; set; }

		public string Name { get; set; }

		public Data(
			FieldAttributes attributes,
			ILeafResolver? procedureType,
			UInt32 vBaseOffset,
			string name
		) {
			Attributes = attributes;
			ProcedureType = procedureType;
			VBaseOffset = vBaseOffset;
			Name = name;
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

			var Attributes = new FieldAttributes(r.ReadUInt16());
			var ProcedureType = r.ReadIndexedType32Lazy();
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

			var Name = r.ReadCString();
			
			Data = new Data(
				attributes: Attributes,
				procedureType: ProcedureType,
				vBaseOffset: VBaseOffset,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_ONEMETHOD);
			w.WriteUInt16((ushort)data.Attributes);
			w.WriteIndexedType(data.ProcedureType);
			switch (data.Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					w.WriteUInt32(data.VBaseOffset);
					break;
			}
			w.WriteCString(data.Name);
			w.WriteHeader();
		}
	}
}
