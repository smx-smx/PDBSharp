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

	public class LF_POINTER : LeafBase
	{
		public ILeafContainer UnderlyingType { get; set; }
		public PointerAttributes Attributes { get; set; }

		public LF_POINTER(IServiceContainer ctx, SpanStream stream) : base(ctx, stream){
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			UnderlyingType = r.ReadIndexedTypeLazy();
			Attributes = new PointerAttributes(r.ReadUInt32());
		}

		public override void Write() {
			TypeDataWriter w = CreateWriter(LeafType.LF_POINTER);
			w.WriteIndexedType(UnderlyingType);
			w.WriteUInt32((uint)Attributes);
			w.WriteHeader();
		}

		public override string ToString() {
			return $"LF_POINTER[UnderlyingType='{UnderlyingType}', Attributes='{Attributes}']";
		}
	}
}
