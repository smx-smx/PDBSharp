#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Leaves
{
	public class LF_VFTABLE : LeafBase
	{
		public ILeafContainer Type { get; set; }
		public ILeafContainer BaseVfTable { get; set; }
		public UInt32 OffsetInObjectLayout { get; set; }
		/// <summary>
		/// Size in Bytes
		/// </summary>
		public UInt32 NamesSize { get; set; }
		public string[] Names { get; set; }

		public LF_VFTABLE(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			Type = r.ReadIndexedTypeLazy();
			BaseVfTable = r.ReadIndexedTypeLazy();
			OffsetInObjectLayout = r.ReadUInt32();
			NamesSize = r.ReadUInt32();

			List<string> lstNames = new List<string>();

			uint read = 0;
			long savedPos = stream.Position;
			while (read < NamesSize) {
				lstNames.Add(r.ReadCString());
				read += (uint)(stream.Position - savedPos);
				savedPos = stream.Position;
			}
			Names = lstNames.ToArray();
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_VFTABLE);
			w.WriteIndexedType(Type);
			w.WriteIndexedType(BaseVfTable);
			w.WriteUInt32(OffsetInObjectLayout);
			w.WriteUInt32(NamesSize);
			foreach (string name in Names) {
				w.WriteCString(name);
			}
			w.WriteHeader();
		}
	}
}
