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
	public class LF_ONEMETHOD : LeafBase
	{
		public FieldAttributes Attributes { get; set; }
		public ILeafContainer ProcedureType { get; set; }
		public UInt32 VBaseOffset { get; set; }

		public string Name { get; set; }

		public LF_ONEMETHOD(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			Attributes = new FieldAttributes(r.ReadUInt16());
			ProcedureType = r.ReadIndexedType32Lazy();

			switch (Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					VBaseOffset = r.ReadUInt32();
					break;
				default:
					VBaseOffset = 0;
					break;
			}

			Name = r.ReadCString(); 
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_ONEMETHOD);
			w.WriteUInt16((ushort)Attributes);
			w.WriteIndexedType(ProcedureType);
			switch (Attributes.MethodProperties) {
				case MethodProperties.Intro:
				case MethodProperties.PureIntro:
					w.WriteUInt32(VBaseOffset);
					break;
			}
			w.WriteCString(Name);
			w.WriteHeader();
		}
	}
}
