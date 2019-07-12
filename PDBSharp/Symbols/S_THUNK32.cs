#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.PDBSharp.Symbols.Structures;
using Smx.PDBSharp.Thunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Symbols
{
	public class ThunkSym32
	{
		public UInt32 Parent { get; set; }
		public UInt32 End { get; set; }
		public UInt32 Next { get; set; }
		public UInt32 Offset { get; set; }
		public UInt16 Segment { get; set; }
		public UInt16 ThunkLength { get; set; }
		public ThunkType ThunkType { get; set; }
		public string Name { get; set; }

		public IThunk Thunk { get; set; }
	}

	public class S_THUNK32 : ISymbol
	{
		public readonly UInt32 Parent;
		public readonly UInt32 End;
		public readonly UInt32 Next;
		public readonly UInt32 Offset;
		public readonly UInt16 Segment;
		public readonly UInt16 ThunkLength;
		public readonly ThunkType ThunkType;
		public readonly string Name;

		public readonly IThunk Thunk;

		public S_THUNK32(PDBFile pdb, Stream stream) {
			var r = new SymbolDataReader(pdb, stream);

			Parent = r.ReadUInt32();
			End = r.ReadUInt32();
			Next = r.ReadUInt32();
			Offset = r.ReadUInt32();
			Segment = r.ReadUInt16();
			ThunkLength = r.ReadUInt16();
			ThunkType = r.ReadEnum<ThunkType>();
			Name = r.ReadSymbolString();
			Thunk = r.ReadThunk(ThunkType);
		}

		public S_THUNK32(ThunkSym32 data) {
			Parent = data.Parent;
			End = data.End;
			Next = data.Next;
			Offset = data.Offset;
			Segment = data.Segment;
			ThunkLength = data.ThunkLength;
			ThunkType = data.ThunkType;
			Name = data.Name;
			Thunk = data.Thunk;
		}

		public void Write(PDBFile pdb, Stream stream) {
			var w = new SymbolDataWriter(pdb, stream, SymbolType.S_THUNK32);
			w.WriteUInt32(Parent);
			w.WriteUInt32(End);
			w.WriteUInt32(Next);
			w.WriteUInt32(Offset);
			w.WriteUInt16(Segment);
			w.WriteUInt16(ThunkLength);
			w.WriteEnum<ThunkType>(ThunkType);
			w.WriteSymbolString(Name);
			Thunk.Write(w);

			w.WriteSymbolHeader();
		}
	}
}
