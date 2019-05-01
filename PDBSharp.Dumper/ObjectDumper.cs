#region License
/*
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections;
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

		private void AppendIndent(StringBuilder sb, int depth) {
			for (int i = 0; i < depth; i++)
				sb.Append("\t");
		}

		private string GetString(Type t) {
			StringBuilder sb = new StringBuilder($"{{{t.FullName}}}\t");

			if(!t.IsPrimitive && t != typeof(string))
				sb.AppendLine();

			AppendIndent(sb, depth);

			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);

			if (obj == null) {
				sb.Append("null");
			} else if (t.IsPrimitive) {
				sb.Append(obj.ToString());
				sb.AppendFormat(" (0x{0:X})", obj);
			} else if (t == typeof(string)) {
				sb.Append($"\"{obj.ToString()}\"");
			} else if (typeof(IEnumerable<object>).IsAssignableFrom(t)) {
				int i = 0;
				foreach (var item in (IEnumerable)obj) {
					AppendIndent(sb, depth + 1);
					sb.AppendFormat("[{0:D}]: {1}", i++, new ObjectDumper(item, depth + 2).ToString());
					sb.AppendLine();
				}
			} else if (t.IsArray) {
				sb.AppendLine();
				Type elType = t.GetElementType();
				Array array = (Array)obj;
				for (int i = 0; i < array.Length; i++) {
					AppendIndent(sb, depth + 1);
					sb.AppendFormat("[{0:D}]: {1}", i, new ObjectDumper(array.GetValue(i), depth + 2).ToString());
				}
			} else if (t.IsEnum) {
				FlagsAttribute flags = t.GetCustomAttribute<FlagsAttribute>();
				if (flags != null) {
					sb.Append("[FLAGS]: <");
					int numFlags = 0;
					foreach (Enum value in Enum.GetValues(t)) {
						if (((Enum)obj).HasFlag(value)) {
							sb.AppendFormat($"{value.ToString()},");
							numFlags++;
						}
					}
					if (numFlags > 0) {
						sb.Length--;
					}
					sb.Append(">");
				} else {
					sb.Append(Enum.GetName(t, obj));
				}
			} else if(fields.Length > 0) {
				foreach (FieldInfo field in fields) {
					AppendIndent(sb, depth + 1);
					sb.Append($"[{field.Name}] => ");

					object value = field.GetValue(obj);
					if (field.FieldType != t) {
						sb.Append(new ObjectDumper(value, depth).ToString());
					}
				}
			} else {
				sb.Append(obj.ToString());
			}

			sb.AppendLine();
			return sb.ToString();
		}

		public override string ToString() {
			if (obj == null)
				return "null";
			return GetString(obj.GetType());
		}


		public static void Dump(object obj) {
			Console.WriteLine(new ObjectDumper(obj).ToString());
		}
	}
}
