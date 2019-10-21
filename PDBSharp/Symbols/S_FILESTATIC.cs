#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Symbols.Structures;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Symbols
{
	public class S_FILESTATIC : SymbolBase
	{
		public ILeafContainer Type { get; set; }
		public UInt32 ModuleFilenameOffset { get; set; }
		public CV_LVARFLAGS Flags { get; set; }
		public string Name { get; set; }

		public S_FILESTATIC(IServiceContainer ctx, IModule mod, SpanStream stream) : base(ctx, mod, stream) {
		}

		public override void Read() {
			var r = CreateReader();
			Type = r.ReadIndexedTypeLazy();
			ModuleFilenameOffset = r.ReadUInt32();
			Flags = r.ReadFlagsEnum<CV_LVARFLAGS>();
			Name = r.ReadSymbolString();
		}

		public override void Write() {
			var w = CreateWriter(SymbolType.S_FILESTATIC);
			w.WriteIndexedType(Type);
			w.WriteUInt32(ModuleFilenameOffset);
			w.Write<CV_LVARFLAGS>(Flags);
			w.WriteSymbolString(Name);

			w.WriteHeader();
		}
	}
}
