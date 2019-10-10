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
using System.Text;

namespace Smx.PDBSharp
{
	public class DebuggableLazy<T> : ILazy<T> where T : class {
		private readonly Func<T> valueFactory;

		private bool invoked = false;
		private T value = null;

		public T Value {
			get {
				if (invoked)
					return value;

				invoked = true;
				value = valueFactory();
				return value;
			}
		}

		public DebuggableLazy(Func<T> valueFactory) {
			this.valueFactory = valueFactory;
		}
	}
}
