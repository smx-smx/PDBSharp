#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Smx.PDBSharp
{
	public static class SpanExtensions
	{
		public unsafe static T Read<T>(this ReadOnlySpan<byte> data, int offset) where T : unmanaged {
			int length = sizeof(T);
			return Cast<T>(data.Slice(offset, length))[0];
		}

		public unsafe static T Read<T>(this Span<byte> data, int offset) where T : unmanaged {
			int length = sizeof(T);
			return Cast<T>(data.Slice(offset, length))[0];
		}

		public static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<byte> data) where T : struct {
			return MemoryMarshal.Cast<byte, T>(data);
		}

		public static Span<T> Cast<T>(this Span<byte> data) where T : struct {
			return MemoryMarshal.Cast<byte, T>(data);
		}
	}
}
