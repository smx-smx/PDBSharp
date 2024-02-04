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
using System.Linq;
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_ARGLIST
{
	public class Data : ILeafData {
		public UInt16 NumberOfArguments { get; set; }
		public ILeafResolver?[] ArgumentTypes { get; set; }

		public Data(ushort numberOfArguments, ILeafResolver?[] argumentTypes) {
			NumberOfArguments = numberOfArguments;
			ArgumentTypes = argumentTypes;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;

		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
			
		}

		public long Read() {
			TypeDataReader r = CreateReader();

			var NumberOfArguments = r.ReadUInt16();
			r.ReadUInt16(); //padding
			var ArgumentTypes = Enumerable.Range(1, NumberOfArguments)
											.Select(_ => r.ReadIndexedType32Lazy())
											.ToArray();

			Data = new Data(
				numberOfArguments: NumberOfArguments,
				argumentTypes: ArgumentTypes
			);

			return r.Position;
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_ARGLIST);
			w.WriteUInt16(data.NumberOfArguments);
			w.WriteUInt16(0x00);

			foreach (ILeafResolver? leaf in data.ArgumentTypes) {
				w.WriteIndexedType(leaf);
			}

			w.WriteHeader();
		}

		public override string ToString() {
			var data = Data;
			return $"LF_ARGLIST(NumberOfArguments='{data?.NumberOfArguments}', " +
				$"ArgumentTypes='{string.Join(", ",
					data?.ArgumentTypes.Select(a => data.ToString()) ?? Array.Empty<string>())
				}')";
		}
	}
}
