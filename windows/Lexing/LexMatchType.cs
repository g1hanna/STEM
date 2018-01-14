using System;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public enum LexMatchType {
		/// <summary>
		/// No match occurred
		/// </summary>
		None,
		/// <summary>
		/// Invalid pattern (invalid escape sequences, letters after numbers, etc.)
		/// </summary>
		Invalid,
		/// <summary>
		/// Partial match occurred;
		/// inidicates possible existence of later matches
		/// </summary>
		Partial,
		Null,
		Whitespace,
		LitInt,
		LitBool,
		LitFloat,
		LitFloatSep,
		LitStringQuot,
		LitString,
		LitStringEscape,
		Program
	}

	public static class LexMatchTypeTools
	{
		public static bool IsNullable(this LexMatchType matchType) {
			switch (matchType)
			{
				case LexMatchType.Null:
					return true;
				default:
					return false;
			}
		}
	}

}