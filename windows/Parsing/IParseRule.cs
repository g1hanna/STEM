using System;
using System.Collections.Generic;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{

	public interface IParseRule
	{
		ParseMatchType MatchType { get; set; }

		bool Accepts(ILexResult target);
		IParseResult Parse(ILexResult target);
	}

}