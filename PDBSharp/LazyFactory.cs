#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Smx.PDBSharp
{
	class WrappedLazy<T> : ILazy<T>
	{
		private readonly Lazy<T?> lazy;
		public T? Value => lazy.Value;

		public WrappedLazy(Func<T> valueFactory) {
			lazy = new Lazy<T?>(valueFactory, LazyThreadSafetyMode.None);
		}
	}

	class LazyFactory
	{
		public static ILazy<T> CreateLazy<T>(Func<T> valueFactory) where T : class? {
#if false
			return new WrappedLazy<T>(valueFactory);
#else
			return new DebuggableLazy<T>(valueFactory);
#endif
		}
	}
}
