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
	public class LF_METHODLIST : LeafBase
	{
		public FieldAttributes Attributes { get; set; }
		public ILeafContainer ProcedureTypeRecord { get; set; }

		public UInt32 VBaseOffset { get; set; }

		public LF_METHODLIST(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			Attributes = new FieldAttributes(r.ReadUInt16());
			ProcedureTypeRecord = r.ReadIndexedTypeLazy();

			switch (Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					VBaseOffset = r.ReadUInt32();
					break;
				default:
					VBaseOffset = 0;
					break;
			}
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_METHODLIST);
			w.WriteUInt16((ushort)Attributes);
			w.WriteIndexedType(ProcedureTypeRecord);
			switch (Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					w.WriteUInt32(VBaseOffset);
					break;
			}
			w.WriteHeader();
		}

		public override string ToString() {
			return $"LF_METHODLIST[Attributes='{Attributes}', ProcedureTypeRecord='{ProcedureTypeRecord}', VBaseOffset='{VBaseOffset}']";
		}
	}
}
