#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
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
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Cached<T>(this IEnumerable<T> source) {
			if (source == null)
				throw new ArgumentNullException("source");
			return new CachedEnumerable<T>(source);
		}

		public static IList<T> GetCachedList<T>(this IEnumerable<T> source) {
			if (!(source is CachedEnumerable<T> cachedEnumerable)) {
				throw new InvalidCastException("Supplied Enumerable is not a CachedEnumerable");
			}
			return cachedEnumerable.ToList();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>https://stackoverflow.com/a/12428250</remarks>
	/// <typeparam name="T"></typeparam>
	class CachedEnumerable<T> : IEnumerable<T>
	{
		readonly Object gate = new Object();
		readonly IEnumerable<T> source;
		readonly List<T> cache = new List<T>();
		IEnumerator<T> enumerator;
		bool isCacheComplete;

		public CachedEnumerable(IEnumerable<T> source) {
			this.source = source;
		}

		public List<T> ToList() {
			return cache;
		}

		public IEnumerator<T> GetEnumerator() {
			lock (this.gate) {
				if (this.isCacheComplete)
					return this.cache.GetEnumerator();
				if (this.enumerator == null)
					this.enumerator = source.GetEnumerator();
			}
			return GetCacheBuildingEnumerator();
		}

		public IEnumerator<T> GetCacheBuildingEnumerator() {
			var index = 0;
			T item;
			while (TryGetItem(index, out item)) {
				yield return item;
				index += 1;
			}
		}

		bool TryGetItem(Int32 index, out T item) {
			lock (this.gate) {
				if (!IsItemInCache(index)) {
					// The iteration may have completed while waiting for the lock.
					if (this.isCacheComplete) {
						item = default(T);
						return false;
					}
					if (!this.enumerator.MoveNext()) {
						item = default(T);
						this.isCacheComplete = true;
						this.enumerator.Dispose();
						return false;
					}
					this.cache.Add(this.enumerator.Current);
				}
				item = this.cache[index];
				return true;
			}
		}

		bool IsItemInCache(Int32 index) {
			return index < this.cache.Count;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
