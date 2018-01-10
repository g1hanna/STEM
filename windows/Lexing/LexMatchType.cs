using System;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public enum LexMatchType {
		None,
		Invalid,
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