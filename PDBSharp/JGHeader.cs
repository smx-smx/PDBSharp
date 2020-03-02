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
	public unsafe struct JGHeader : IHeader
	{
		public const int MAGIC_SIZE = 44;

		public fixed byte Magic[MAGIC_SIZE];
		public UInt32 PageSize;
		public UInt16 FpmPageNumber;
		public UInt16 NumPages;
		public UInt32 DirectorySize;
		public UInt32 PageMap;

		string IHeader.Magic {
			get {
				throw new NotImplementedException();
			}

			set {
				throw new NotImplementedException();
			}
		}

		private void EnsureShort(uint value) {
			if(value > ushort.MaxValue) {
				throw new ArgumentException($"{value} does not fit in an u16");
			}
		}

		uint IHeader.PageSize {
			get => PageSize;
			set => PageSize = value;
		}
		uint IHeader.FpmPageNumber {
			get => FpmPageNumber;
			set => FpmPageNumber = (ushort)value;
		}
		uint IHeader.NumPages {
			get => NumPages;
			set => NumPages = (ushort)value;
		}
		uint IHeader.DirectorySize {
			get => DirectorySize;
			set => DirectorySize = value;
		}
	}
}
