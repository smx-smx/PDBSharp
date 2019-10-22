#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Smx.PDBSharp
{
	public class CachedEnumerable<T> : IEnumerable<T>, IDisposable
	{
		private readonly IList<T> cache = new List<T>();
		private readonly IEnumerator<T> items;

		public CachedEnumerable(IEnumerable<T> items) {
			this.items = items.GetEnumerator();
		}

		public void Dispose() {
			this.items?.Dispose();
		}

		public IEnumerator<T> GetEnumerator() {
			foreach (var item in this.cache) {
				yield return item;
			}

			while (this.items.MoveNext()) {
				var item = this.items.Current;
				this.cache.Add(item);
				yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
