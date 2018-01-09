using System;
using System.Collections;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public class FunctionalLexRule : ILexRule
	{
		public LexMatcher LexFunction { get; set; }
		public LexMatchType MatchType { get; set; }

		public FunctionalLexRule(LexMatchType matchType, LexMatcher lexFunction)
		{
			LexFunction = lexFunction;
			MatchType = matchType;
		}

		public ILexResult Match(string target)
		{
			return LexFunction(target);
		}
	}

}