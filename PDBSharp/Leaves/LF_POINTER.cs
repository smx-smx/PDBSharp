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

namespace Smx.PDBSharp.Leaves.LF_POINTER
{
	public class PointerAttributes
	{
		private readonly UInt32 attrs;

		public static explicit operator UInt32(PointerAttributes attrs) => attrs.attrs;

		public PointerAttributes(UInt32 attrs) {
			this.attrs = attrs;

			if (!Enum.IsDefined(typeof(PointerType), this.PointerType)) {
				throw new InvalidDataException();
			}
			if (!Enum.IsDefined(typeof(PointerMode), this.PointerMode)) {
				throw new InvalidDataException();
			}
		}

		public PointerType PointerType => (PointerType)(attrs & 0x1F);
		public PointerMode PointerMode => (PointerMode)((attrs >> 5) & 0x7);
		public bool IsFlat32 => ((attrs >> 8) & 1) == 1;
		public bool IsVolatile => ((attrs >> 9) & 1) == 1;
		public bool IsConst => ((attrs >> 10) & 1) == 1;
		public bool IsUnaligned => ((attrs >> 11) & 1) == 1;
		public bool IsRestricted => ((attrs >> 12) & 1) == 1;
	}

	public class Data : ILeafData {
		public ILeafResolver? UnderlyingType { get; set; }
		public PointerAttributes Attributes { get; set; }

		public Data(ILeafResolver? underlyingType, PointerAttributes attributes) {
			UnderlyingType = underlyingType;
			Attributes = attributes;
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

			var UnderlyingType = r.ReadIndexedType32Lazy();
			var Attributes = new PointerAttributes(r.ReadUInt32());
			
			Data = new Data(
				underlyingType: UnderlyingType,
				attributes: Attributes
			);
		}

		public void Write() {
			var data = Data;
			if (data == null) throw new InvalidOperationException();

			TypeDataWriter w = CreateWriter(LeafType.LF_POINTER);
			w.WriteIndexedType(data.UnderlyingType);
			w.WriteUInt32((uint)data.Attributes);
			w.WriteHeader();
		}

		public override string ToString() {
			var data = Data;
			return $"LF_POINTER[UnderlyingType='{data?.UnderlyingType}', Attributes='{data?.Attributes}']";
		}
	}
}
