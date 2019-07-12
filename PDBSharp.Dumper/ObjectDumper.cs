#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp.Leaves;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Smx.PDBSharp.Dumper
{
	class ObjectDumper
	{
		private readonly StringBuilder sb = new StringBuilder();
		private readonly Type t;
		private readonly object obj;
		private readonly int depth;
		private int depthOffset = 0;

		private ObjectDumper(object obj, int depth = 0) {
			this.obj = obj;
			this.depth = depth;
			if (obj != null)
				t = obj.GetType();
		}

		private static void AppendIndent(StringBuilder sb, int depth) {
			for (int i = 0; i < depth; i++)
				sb.Append("\t");
		}

		private static void NewIndentedLine(StringBuilder sb, int depth) {
			sb.AppendLine();
			AppendIndent(sb, depth);
		}

		private void CrIndent(StringBuilder sb) {
			NewIndentedLine(sb, depth + depthOffset);
		}

		private void CrIndentPush(StringBuilder sb) {
			++depthOffset;
			CrIndent(sb);
		}

		private string HandleNullType() {
			sb.Append("null");
			CrIndent(sb);
			return sb.ToString();
		}

		private string HandleStringType() {
			// string: print inline
			sb.Append($"\"{obj.ToString()}\"");
			return sb.ToString();
		}

		private string HandlePrimitiveType() {
			// primitive: print inline
			sb.Append(obj.ToString());

			if (t != typeof(bool)) {
				sb.AppendFormat(" (0x{0:X})", obj);
			}
			return sb.ToString();
		}

		private static Type GetEnumerableType(Type type) {
			foreach (Type intType in type.GetInterfaces()) {
				if (intType.IsGenericType
					&& intType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
					return intType.GetGenericArguments()[0];
				}
			}
			return null;
		}

		public static int CountEnumerable(IEnumerable data) {
			ICollection list = data as ICollection;
			if (list != null) return list.Count;
			int count = 0;
			IEnumerator iter = data.GetEnumerator();
			using (iter as IDisposable) {
				while (iter.MoveNext()) count++;
			}
			return count;
		}

		private string HandleGenericType() {
			Type baseType = t.GetGenericTypeDefinition();
			if(GetEnumerableType(t) != null) {
				IEnumerable ienum = (IEnumerable)obj;
				int numElements = CountEnumerable(ienum);
				if(numElements > 0) {
					CrIndentPush(sb);
				}

				int i = 0;
				foreach (var item in ienum) {
					sb.AppendFormat("[{0:D}]: {1}", i++, new ObjectDumper(item, depth + depthOffset).GetString());
					if(i + 1 < numElements) {
						CrIndent(sb);
					}
				}
				return sb.ToString();
			}

			if (baseType == typeof(Lazy<>)) {
				object value = t.GetProperty("Value").GetValue(obj);
				CrIndentPush(sb);
				sb.Append(new ObjectDumper(value, depth + depthOffset).GetString());
			}

			return sb.ToString();
		}

		private string HandleArrayType() {
			Array array = (Array)obj;
			for (int i = 0; i < array.Length; i++) {
				sb.AppendFormat("[{0:D}]: {1}", i, new ObjectDumper(array.GetValue(i), depth).GetString());
			}

			return sb.ToString();
		}

		private string HandleEnumType() {
			FlagsAttribute flags = t.GetCustomAttribute<FlagsAttribute>();
			if (flags != null) {
				CrIndentPush(sb);
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

			return sb.ToString();
		}

		private string HandleComplexTypeRecursive() {
			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			CrIndentPush(sb);

			if (fields.Length == 0 && props.Length == 0) {
				sb.Append(obj.ToString());
				return sb.ToString();
			}

			bool shouldGetValue;

			for (int i = 0; i < props.Length; i++) {
				PropertyInfo prop = props[i];

				sb.Append($"[{prop.Name}] => ");

				object value = null;
				shouldGetValue = true;

				switch (obj) {
					case LazyLeafProvider llp:
						if (llp.Leaf == null || !Program.OptVerbose) {
							shouldGetValue = false;
							if (llp.Leaf != null) {
								value = "<...>";
							}
						}
						break;
				}
			
				if (shouldGetValue) {
					value = prop.GetValue(obj);
				}
				sb.Append(new ObjectDumper(value, depth + depthOffset).GetString());

				if (i + 1 < props.Length) {
					CrIndent(sb);
				}
			}

			if (props.Length > 0 && fields.Length > 0)
				CrIndent(sb);

			for(int i=0; i<fields.Length; i++) {
				FieldInfo field = fields[i];
				// workaround for stack overflow on async methods
				if (field.Name.StartsWith("<>") && field.Name.EndsWith("__this")) {
					continue;
				}

				sb.Append($"[{field.Name}] => ");

				shouldGetValue = true;
				object value = null;

				if (shouldGetValue) {
					value = field.GetValue(obj);
				}

				sb.Append(new ObjectDumper(value, depth + depthOffset).GetString());

				if (i + 1 < fields.Length) {
					CrIndent(sb);
				}
			}

			return sb.ToString();
		}

		private string HandleComplexType() {
			if (t.IsGenericType) {
				return HandleGenericType();
			} else if (t.IsArray) {
				return HandleArrayType();
			} else if (t.IsEnum) {
				return HandleEnumType();
			} else {
				return HandleComplexTypeRecursive();
			}
		}

		private string GetString() {
			if (obj == null) {
				return HandleNullType();
			}

			sb.Append($"{{{t.FullName}}}\t");

			if (t == typeof(string)) {
				return HandleStringType();
			}

			if (t.IsPrimitive) {
				return HandlePrimitiveType();
			}

			return HandleComplexType();
		}


		public static void Dump(object obj) {
			Console.WriteLine(new ObjectDumper(obj).GetString() + Environment.NewLine);
		}
	}
}
