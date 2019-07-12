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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.PDBSharp.Leaves
{
	public abstract class Leaf : LeafBase {
		private string GetUdtName() {
			switch (this.Data) {
				case LF_CLASS lfClass:
					return lfClass.Name;
				default:
					throw new NotImplementedException();
			}
		}

		private bool IsUdtAnon() {
			string[] utag = new string[] {
				"::<unnamed-tag>",
				"::__unnamed"
			};

			string UdtName = GetUdtName();
			foreach (string tag in utag) {
				if (UdtName.Contains(tag))
					return true;
			}
			return false;
		}

		public bool IsUdtSourceLine() {
			switch (this) {
				//$TODO
				/*case LF_UDT_SRC_LINE:
				case LF_UDT_MOD_SRC_LINE:
					return true;*/
				default:
					return false;
			}
		}

		public bool IsGlobalDefnUdtWithUniqueName() {
			switch (this.Data) {
				case LF_CLASS _:
				case LF_UNION _:
				case LF_ENUM _:
					//$TODO
					//case LF_INTERFACE _:
					break;
				default:
					return false;
			}

			LF_CLASS leaf = (LF_CLASS)this.Data;
			return (
				!leaf.FieldProperties.HasFlag(TypeProperties.IsForwardReference) &&
				!leaf.FieldProperties.HasFlag(TypeProperties.IsScoped) &&
				leaf.FieldProperties.HasFlag(TypeProperties.HasUniqueName) &&
				!IsUdtAnon()
			);

		}

		public bool IsGlobalDefnUdt() {
			switch (this.Data) {
				//$TODO
				/*case LF_ALIAS _:
					return true;*/
				case LF_CLASS _:
				case LF_UNION _:
				case LF_ENUM _:
					//case LF_INTERFACE _:
					break;
				default:
					return false;
			}

			//$TODO: not tested
			LF_CLASS leaf = (LF_CLASS)this.Data;
			return (
				!leaf.FieldProperties.HasFlag(TypeProperties.IsForwardReference) &&
				!leaf.FieldProperties.HasFlag(TypeProperties.IsScoped) &&
				!IsUdtAnon()
			);
		}
	}
}
