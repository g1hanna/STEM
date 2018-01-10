using System;
using System.Collections.Generic;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{

	public class ParseRule : IParseRule
	{
		#region FIELDS
		private LexResultValidator _validator;
		private LexResultParser _parsePattern;
		private ParseMatchType _matchType;
		#endregion

		#region PROPERTIES
		public LexResultValidator Validator { get => _validator; set => _validator = value; }
		public LexResultParser ParsePattern { get => _parsePattern; set => _parsePattern = value; }
		public ParseMatchType MatchType { get => _matchType; set => _matchType = value; }
		#endregion

		#region CONSTRUCTORS
		public ParseRule(LexResultParser parseLogic, LexResultValidator validator)
		{
			if (parseLogic == null || validator == null)
			{
				throw new ArgumentNullException("A parse rule cannot have null parse patterns or validators.");
			}

			_parsePattern = parseLogic;
			_validator = validator;
		}
		#endregion

		#region METHODS
		public bool Accepts(ILexResult target)
		{
			return _validator(target);
		}

		public IParseResult Parse(ILexResult target)
		{
			if (!Accepts(target)) return ParseResult.Empty;

			return _parsePattern(target);
		}
		#endregion
	}

}