#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.PDBSharp
{
	/// <summary>
	/// A virtual ILeaf representing a builtin type
	/// </summary>
	public class BuiltinTypeLeaf : ILeafData
	{
		public readonly SpecialType SpecialType;
		public readonly SpecialTypeMode TypeMode;

		public BuiltinTypeLeaf(UInt32 TypeIndex) {
			// Remove type mode
			SpecialType = (SpecialType)(TypeIndex & 0xFF);
			
			// Retain type mode only
			TypeMode = (SpecialTypeMode)(TypeIndex & 0xF00);

			if(!Enum.IsDefined(typeof(SpecialType), SpecialType)) {
				throw new InvalidDataException($"Unknown SpecialType 0x{SpecialType:X}");
			}

			if(!Enum.IsDefined(typeof(SpecialTypeMode), TypeMode)) {
				throw new InvalidDataException($"Unknown TypeMode 0x{TypeMode:X}");
			}
		}

		public override string ToString() {
			string typeName = Enum.GetName(typeof(SpecialType), SpecialType);
			string typeMode = Enum.GetName(typeof(SpecialTypeMode), TypeMode);
			return $"{typeName} [{typeMode}]";
		}
	}
}
