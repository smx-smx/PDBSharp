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

namespace Smx.PDBSharp.Leaves
{
	public abstract class LeafBase
	{
		public bool IsUdtAnon {
			get {
				string[] utag = new string[] {
					"::<unnamed-tag>",
					"::__unnamed"
				};

				string udtName = UdtName;
				foreach (string tag in utag) {
					if (udtName == tag.Substring(2))
						return true;

					if (udtName.EndsWith(tag))
						return true;
				}
				return false;
			}
		}

		public virtual string UdtName => null;
		public virtual bool IsUdtSourceLine => false;
		public virtual bool IsGlobalDefnUdtWithUniqueName => false;
		public virtual bool IsLocalDefnUdtWithUniqueName => false;
		public virtual bool IsDefnUdt => false;
		public virtual bool IsGlobalDefnUdt => false;
	}
}
