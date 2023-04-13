#region License
/*
 * Copyright (C) 2020 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace Smx.PDBSharp.Leaves
{
	public class PointerAttributes16 {
		private readonly UInt16 attrs;

		public static explicit operator UInt16(PointerAttributes16 attrs) => attrs.attrs;

		public PointerAttributes16(UInt16 attrs) {
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
	}

	public enum MemberPointerType : UInt16
	{
		/// <summary>
		/// not specified (pre VC8)
		/// </summary>
		Undefined = 0x00,
		/// <summary>
		/// member data, single inheritance
		/// </summary>
		DataSingle = 0x01,
		/// <summary>
		/// member data, multiple inheritance
		/// </summary>
		DataMultiple = 0x02,
		/// <summary>
		/// member data, virtual inheritance
		/// </summary>
		DataVirtual = 0x03,
		/// <summary>
		/// member data, most general
		/// </summary>
		DataGeneral = 0x04,
		/// <summary>
		/// member function, single inheritance
		/// </summary>
		FunctionSingle = 0x05,
		/// <summary>
		/// member function, multiple inheritance
		/// </summary>
		FunctionMultiple = 0x06,
		/// <summary>
		/// member function, virtual inheritance
		/// </summary>
		FunctionVirtual = 0x07,
		/// <summary>
		/// member function, most general
		/// </summary>
		FunctionGeneral = 0x08
	}

	public class LF_POINTER16t : LeafBase
	{
		public ILeafContainer UnderlyingType { get; set; }
		public PointerAttributes16 Attributes { get; set; }

		public ILeafContainer ContainingClass { get; set; }
		public MemberPointerType MemberPointerType { get; set; }

		public UInt16 BaseSegment { get; set; }

		public ILeafContainer BaseType { get; set; }
		public string BaseTypeName { get; set; }

		public LF_POINTER16t(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public override void Read() {
			TypeDataReader r = CreateReader();

			//// header

			Attributes = new PointerAttributes16(r.ReadUInt16());
			UnderlyingType = r.ReadIndexedType16Lazy();

			//// body
			switch (Attributes.PointerMode) {
				case PointerMode.Pointer:
				case PointerMode.Reference:
					switch (Attributes.PointerType) {
						case PointerType.BaseSeg:
							BaseSegment = r.ReadUInt16();
							throw new NotImplementedException("LF_REFSYM");
							break;
						case PointerType.BaseType:
							BaseType = r.ReadIndexedType16Lazy();
							BaseTypeName = r.ReadString16();
							break;
					}
					break;
				case PointerMode.PointerMember:
				case PointerMode.PointerFunction:
					ContainingClass = r.ReadIndexedType16Lazy();
					MemberPointerType = r.ReadEnum<MemberPointerType>();
					break;
			}
		}
	}
}
