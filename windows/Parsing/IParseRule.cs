using System;
using System.Collections.Generic;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{

	public interface IParseRule
	{
		ParseMatchType MatchType { get; }

		bool Accepts(ILexResult target);
		IParseResult Parse(ILexResult target);
	}

}