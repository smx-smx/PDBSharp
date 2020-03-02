#region License
/*
 * Copyright (C) 2020 Stefano Moioli <smxdev4@gmail.com>
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
	public unsafe ref struct SpanReader {
		private readonly Span<byte> span;
		private long Position;

		public SpanReader(Span<byte> span) {
			this.span = span;
			Position = 0;
		}

		public T Read<T>() where T : unmanaged {
			T ret = span.Read<T>((int)Position);
			Position += sizeof(T);
			return ret;
		}
	}
}
