using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{
	public delegate bool LexResultValidator(ILexResult lexResult);

	public delegate IParseResult LexResultParser(ILexResult lexResult);
}