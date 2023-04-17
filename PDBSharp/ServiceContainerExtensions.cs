#region License
/*
 * Copyright (C) 2023 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.ComponentModel.Design;

namespace Smx.PDBSharp
{
	public static class IServiceContainerExtensions
	{
		public static void AddService<T>(this IServiceContainer @this, object instance) {
			@this.AddService(typeof(T), instance);
		}

		public static T GetService<T>(this IServiceContainer @this) {
			return (T)@this.GetService(typeof(T));
		}
	}
}
