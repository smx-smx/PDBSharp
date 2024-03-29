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
using System.ComponentModel.Design;
using System.IO;

namespace Smx.PDBSharp.Thunks
{
	namespace PCODE
	{
		public class Data : IThunk { }
		public class Serializer : ISerializer<Data>
		{
			public Serializer(IServiceContainer sc, SymbolHeader header, SpanStreamEx stream) { }

			public Data Data = new Data();
			public Data Read() {
				return Data;
			}
			public void Write(Data data) {

			}
		}
	}
}
