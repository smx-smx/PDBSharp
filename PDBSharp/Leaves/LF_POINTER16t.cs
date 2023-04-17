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
using Smx.PDBSharp.LeafResolver;

namespace Smx.PDBSharp.Leaves.LF_POINTER16t
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

	public class Data : ILeafData {
		public ILeafResolver? UnderlyingType { get; set; }
		public PointerAttributes16 Attributes { get; set; }

		public ILeafResolver? ContainingClass { get; set; }
		public MemberPointerType MemberPointerType { get; set; }

		public UInt16 BaseSegment { get; set; }

		public ILeafResolver? BaseType { get; set; }
		public string? BaseTypeName { get; set; }

		public Data(
			ILeafResolver? underlyingType,
			PointerAttributes16 attributes,
			ILeafResolver? containingClass,
			MemberPointerType memberPointerType,
			ushort baseSegment,
			ILeafResolver? baseType,
			string? baseTypeName
		) {
			UnderlyingType = underlyingType;
			Attributes = attributes;
			ContainingClass = containingClass;
			MemberPointerType = memberPointerType;
			BaseSegment = baseSegment;
			BaseType = baseType;
			BaseTypeName = baseTypeName;
		}
	}

	public class Serializer : LeafBase, ILeafSerializer
	{
		public Data? Data { get; set; }
		public ILeafData? GetData() => Data;
		
		

		public Serializer(IServiceContainer ctx, SpanStream stream) : base(ctx, stream) {
		}

		public void Read() {
			TypeDataReader r = CreateReader();

			//// header

			var Attributes = new PointerAttributes16(r.ReadUInt16());
			var UnderlyingType = r.ReadIndexedType16Lazy();
			var BaseSegment = new UInt16();
			ILeafResolver? BaseType = null;
			string? BaseTypeName = null;
			ILeafResolver? ContainingClass = null;
			var memberPointerType = MemberPointerType.Undefined;

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
					memberPointerType = r.ReadEnum<MemberPointerType>();
					break;
			}

			Data = new Data(
				underlyingType: UnderlyingType,
				attributes: Attributes,
				containingClass: ContainingClass,
				memberPointerType: memberPointerType,
				baseSegment: BaseSegment,
				baseType: BaseType,
				baseTypeName: BaseTypeName
			);
		}

		public void Write() {
			throw new NotImplementedException();
		}
	}
}
