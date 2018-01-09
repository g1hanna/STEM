using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StemInterpretter.Lexing {

	public class SimpleLexRule : ILexRule
	{
		private LexMatchType _matchType;
		private string _patternString;

		public LexMatchType MatchType { get => _matchType; set => _matchType = value; }

		public SimpleLexRule(LexMatchType matchType, string patternString)
		{
			_matchType = matchType;
			_patternString = patternString;
		}

		public ILexResult Match(string target)
		{
			Regex pattern = new Regex(_patternString);
			Match match = pattern.Match(target);

			if (match.Success) {
				return new LexNode(_matchType, match.Index, match.Value);
			}
			else {
				return LexNode.NoMatch;
			}
		}
	}

}