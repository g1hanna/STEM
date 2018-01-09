using System;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public interface ILexRule
	{
		LexMatchType MatchType { get; set; }
		ILexResult Match(string target);
	}

}