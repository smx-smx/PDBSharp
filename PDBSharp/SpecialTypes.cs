#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Smx.PDBSharp
{
	/// <summary>
	/// Custom made flags enum that used in conjunction with the attribute
	/// to specify the allowed type modifiers for a special/simple type
	/// </summary>
	[Flags]
	public enum AllowedSpecialTypeMode
	{
		NearPointer = 1 << 1,
		FarPointer = 1 << 2,
		HugePointer = 1 << 3,
		NearPointer32 = 1 << 4,
		FarPointer32 = 1 << 5,
		NearPointer64 = 1 << 6,
		NearPointer128 = 1 << 7,

		Any32 = NearPointer | FarPointer |
				HugePointer | NearPointer32 | FarPointer32,

		Any64 = Any32 | NearPointer64,
		Any128 = Any64 | NearPointer128
	}

	public class AllowedSpecialTypeModifiersAttribute : Attribute
	{
		public readonly AllowedSpecialTypeMode AllowedModifiers;
		public AllowedSpecialTypeModifiersAttribute(AllowedSpecialTypeMode flags = AllowedSpecialTypeMode.Any64) {
			AllowedModifiers = flags;
		}
	}


	public enum SpecialType : UInt32
	{

		None = 0x0000,          // uncharacterized type (no type)
		AbsoluteSymbol = 0x0001,
		NotTranslated = 0x0007, // type not translated by cvpack

		[AllowedSpecialTypeModifiers(AllowedSpecialTypeMode.NearPointer32 | AllowedSpecialTypeMode.NearPointer64)]
		HResult = 0x0008,       // OLE/COM HRESULT
		[AllowedSpecialTypeModifiers()]
		Void = 0x0003,          // void
		[AllowedSpecialTypeModifiers()]
		SignedCharacter = 0x0010,   // 8 bit signed
		[AllowedSpecialTypeModifiers()]
		UnsignedCharacter = 0x0020, // 8 bit unsigned
		[AllowedSpecialTypeModifiers()]
		NarrowCharacter = 0x0070,   // really a char
		[AllowedSpecialTypeModifiers()]
		WideCharacter = 0x0071,     // wide char
		[AllowedSpecialTypeModifiers()]
		Character16 = 0x007a,       // char16_t
		[AllowedSpecialTypeModifiers()]
		Character32 = 0x007b,       // char32_t
		[AllowedSpecialTypeModifiers()]
		SByte = 0x0068,       // 8 bit signed int
		[AllowedSpecialTypeModifiers()]
		Byte = 0x0069,        // 8 bit unsigned int
		[AllowedSpecialTypeModifiers()]
		Int16Short = 0x0011,  // 16 bit signed
		[AllowedSpecialTypeModifiers()]
		UInt16Short = 0x0021, // 16 bit unsigned
		[AllowedSpecialTypeModifiers()]
		Int16 = 0x0072,       // 16 bit signed int
		[AllowedSpecialTypeModifiers()]
		UInt16 = 0x0073,      // 16 bit unsigned int
		[AllowedSpecialTypeModifiers()]
		Int32Long = 0x0012,   // 32 bit signed
		[AllowedSpecialTypeModifiers()]
		UInt32Long = 0x0022,  // 32 bit unsigned
		[AllowedSpecialTypeModifiers()]
		Int32 = 0x0074,       // 32 bit signed int
		[AllowedSpecialTypeModifiers()]
		UInt32 = 0x0075,      // 32 bit unsigned int
		[AllowedSpecialTypeModifiers()]
		Int64Quad = 0x0013,   // 64 bit signed
		[AllowedSpecialTypeModifiers()]
		UInt64Quad = 0x0023,  // 64 bit unsigned
		[AllowedSpecialTypeModifiers()]
		Int64 = 0x0076,       // 64 bit signed int
		[AllowedSpecialTypeModifiers()]
		UInt64 = 0x0077,      // 64 bit unsigned int
		[AllowedSpecialTypeModifiers()]
		Int128Oct = 0x0014,   // 128 bit signed int
		[AllowedSpecialTypeModifiers()]
		UInt128Oct = 0x0024,  // 128 bit unsigned int
		[AllowedSpecialTypeModifiers()]
		Int128 = 0x0078,      // 128 bit signed int
		[AllowedSpecialTypeModifiers()]
		UInt128 = 0x0079,     // 128 bit unsigned int
		[AllowedSpecialTypeModifiers()]
		Float16 = 0x0046,                 // 16 bit real
		[AllowedSpecialTypeModifiers()]
		Float32 = 0x0040,                 // 32 bit real
		[AllowedSpecialTypeModifiers()]
		Float32PartialPrecision = 0x0045, // 32 bit PP real
		[AllowedSpecialTypeModifiers()]
		Float48 = 0x0044,                 // 48 bit real
		[AllowedSpecialTypeModifiers()]
		Float64 = 0x0041,                 // 64 bit real
		[AllowedSpecialTypeModifiers()]
		Float80 = 0x0042,                 // 80 bit real
		[AllowedSpecialTypeModifiers()]
		Float128 = 0x0043,                // 128 bit real
		[AllowedSpecialTypeModifiers()]
		Complex16 = 0x0056,                 // 16 bit complex
		[AllowedSpecialTypeModifiers()]
		Complex32 = 0x0050,                 // 32 bit complex
		[AllowedSpecialTypeModifiers()]
		Complex32PartialPrecision = 0x0055, // 32 bit PP complex
		[AllowedSpecialTypeModifiers()]
		Complex48 = 0x0054,                 // 48 bit complex
		[AllowedSpecialTypeModifiers()]
		Complex64 = 0x0051,                 // 64 bit complex
		[AllowedSpecialTypeModifiers()]
		Complex80 = 0x0052,                 // 80 bit complex
		[AllowedSpecialTypeModifiers()]
		Complex128 = 0x0053,                // 128 bit complex
		[AllowedSpecialTypeModifiers()]
		Boolean8 = 0x0030,   // 8 bit boolean
		[AllowedSpecialTypeModifiers()]
		Boolean16 = 0x0031,  // 16 bit boolean
		[AllowedSpecialTypeModifiers()]
		Boolean32 = 0x0032,  // 32 bit boolean
		[AllowedSpecialTypeModifiers()]
		Boolean64 = 0x0033,  // 64 bit boolean
		[AllowedSpecialTypeModifiers()]
		Boolean128 = 0x0034, // 128 bit boolean
	}

	public enum PointerType : byte
	{
		Near = 0x00,
		Far = 0x01,
		Huge = 0x02,
		BaseSeg = 0x03,
		BaseVal = 0x04,
		BaseSegVal = 0x05,
		BaseAddress = 0x06,
		BaseSegAddr = 0x07,
		BaseType = 0x08,
		BaseSelf = 0x09,
		Near32 = 0x0a,
		Far32 = 0x0b,
		Ptr64 = 0x0c,
		Unused = 0x0d
	}

	public enum PointerMode : byte
	{
		Pointer = 0x00,
		Reference = 0x01,
		LValueReference = 0x01,
		PointerMember = 0x02,
		PointerFunction = 0x03,
		RValueReference = 0x04,
		Reserved = 0x05
	}


	public enum SpecialTypeMode : UInt32
	{
		Direct = 0,
		NearPointer = 0x100,
		FarPointer = 0x200,
		HugePointer = 0x300,
		NearPointer32 = 0x400,
		FarPointer32 = 0x500,
		NearPointer64 = 0x600,
		NearPointer128 = 0x700
	}
}
