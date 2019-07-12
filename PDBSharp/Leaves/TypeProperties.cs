#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;

namespace Smx.PDBSharp.Leaves
{
	[Flags]
	public enum TypeProperties : UInt16
	{
		/// <summary>
		/// true if structure is packed
		/// </summary>
		IsPacked = 1 << 0,
		/// <summary>
		/// true if constructors or destructors present
		/// </summary>
		CtorDtor = 1 << 1,
		/// <summary>
		/// true if overloaded operators present
		/// </summary>
		HasOverloadedOperators = 1 << 2,
		/// <summary>
		/// true if this is a nested class
		/// </summary>
		IsNested = 1 << 3,
		/// <summary>
		/// true if this class contains nested types
		/// </summary>
		ClassNested = 1 << 4,
		/// <summary>
		/// true if overloaded assignment (=)
		/// </summary>
		OverloadedAssignment = 1 << 5,
		/// <summary>
		/// true if casting methods
		/// </summary>
		HasOpCast = 1 << 7,
		/// <summary>
		/// true if forward reference (incomplete defn)
		/// </summary>
		IsForwardReference = 1 << 8,
		/// <summary>
		/// scoped definition
		/// </summary>
		IsScoped =  1 << 9,
		/// <summary>
		/// true if there is a decorated name following the regular name
		/// </summary>
		HasUniqueName = 1 << 10,
		/// <summary>
		/// true if class cannot be used as a base class
		/// </summary>
		IsSealed = 1 << 11,
		//$TODO: hfa, intrinsic, mocom
	}
}
