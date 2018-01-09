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

}