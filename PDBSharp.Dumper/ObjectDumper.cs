#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Smx.PDBSharp.Dumper
{
	class ObjectDumper
	{
		private readonly object obj;
		private readonly int depth;

		private ObjectDumper(object obj, int depth = 0) {
			this.obj = obj;
			this.depth = depth;
		}

		private string GetString(Type t) {
			StringBuilder sb = new StringBuilder($"{{{t.FullName}}}");
			sb.AppendLine();
			foreach(FieldInfo field in t.GetFields()) {
				for (int i = 0; i < depth; i++)
					sb.Append("\t");

				object value = field.GetValue(obj);

				sb.Append($" [{field.Name}] => ");
				if(value == null) {
					sb.Append("null");
				} else if (field.FieldType.IsPrimitive) {
					sb.Append(value.ToString());
					sb.AppendFormat(" (0x{0:X})", value);
				} else if(field.FieldType == typeof(string)) {
					sb.Append(value.ToString());
				} else if (field.FieldType.IsEnum) {
					sb.Append(Enum.GetName(field.FieldType, value));
				} else if (field.FieldType != t) {
					sb.Append(new ObjectDumper(value, depth + 1).ToString());
				} else {
					sb.Append(value.ToString());
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}

		public override string ToString() {
			return GetString(obj.GetType());
		}


		public static string Dump(object obj) {
			Type t = obj.GetType();
			return new ObjectDumper(obj).ToString();
		}
	}
}
