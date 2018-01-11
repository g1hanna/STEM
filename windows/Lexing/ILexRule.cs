using System;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public interface ILexRule
	{
		LexMatchType MatchType { get; set; }
		ILexResult Match(string target);
	}

	public interface ILexGroupRule : ILexRule
	{
		ILexResult Match(string target, bool insertEmptyNodes);
		bool RequiresFilledSpace { get; set; }
	}

}