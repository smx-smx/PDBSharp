#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using Smx.SharpIO;
using System;
using System.ComponentModel.Design;
using System.IO;
using Smx.PDBSharp.LeafResolver;
using Smx.PDBSharp.Symbols.S_SEPCODE;

namespace Smx.PDBSharp.Symbols.S_FILESTATIC
{
	public class Data : ISymbolData {
		public ILeafResolver? Type { get; set; }
		public UInt32 ModuleFilenameOffset { get; set; }
		public CV_LVARFLAGS Flags { get; set; }
		public string Name { get; set; }

		public Data(ILeafResolver? type, uint moduleFilenameOffset, CV_LVARFLAGS flags, string name) {
			Type = type;
			ModuleFilenameOffset = moduleFilenameOffset;
			Flags = flags;
			Name = name;
		}
	}

	public class Serializer : SymbolSerializerBase, ISymbolSerializer
	{
		public Data? Data { get; set; }

		public Serializer(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public void Read() {
			var r = CreateReader();
			var Type = r.ReadIndexedType32Lazy();
			var ModuleFilenameOffset = r.ReadUInt32();
			var Flags = r.ReadFlagsEnum<CV_LVARFLAGS>();
			var Name = r.ReadSymbolString();
			Data = new Data(
				type: Type,
				moduleFilenameOffset: ModuleFilenameOffset,
				flags: Flags,
				name: Name
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			var w = CreateWriter(SymbolType.S_FILESTATIC);
			w.WriteIndexedType(data.Type);
			w.WriteUInt32(data.ModuleFilenameOffset);
			w.Write<CV_LVARFLAGS>(data.Flags);
			w.WriteSymbolString(data.Name);

			w.WriteHeader();
		}

		public ISymbolData? GetData() => Data;
	}
}
