#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;

namespace Smx.PDBSharp
{
	public interface IReader
	{
		long Position { get; set; }
		long Remaining { get; }
		long Length { get; }

		byte ReadByte();
		byte[] ReadBytes(int count);
		Int16 ReadInt16();
		Int32 ReadInt32();
		Int64 ReadInt64();
		UInt16 ReadUInt16();
		UInt32 ReadUInt32();
		UInt64 ReadUInt64();
		string ReadString();
		float ReadSingle();
		double ReadDouble();
		T Read<T>() where T : unmanaged;
	}
}
