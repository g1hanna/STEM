using System;
using System.Collections.Generic;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{

	public class ParseRule : IParseRule
	{
		#region FIELDS AND AUTOS
		private LexResultValidator _validator;
		private LexResultParser _parsePattern;
		public ParseMatchType MatchType { get; set; }
		#endregion

		#region PROPERTIES
		public LexResultValidator Validator { get => _validator; set => _validator = value; }
		public LexResultParser ParsePattern { get => _parsePattern; set => _parsePattern = value; }
		#endregion

		#region CONSTRUCTORS
		public ParseRule(LexResultValidator validator, LexResultParser parseLogic)
		: this(ParseMatchType.None, validator, parseLogic)
		{
			return;
		}

		public ParseRule(ParseMatchType matchType, LexResultValidator validator, LexResultParser parseLogic)
		{
			if (parseLogic == null || validator == null)
			{
				throw new ArgumentNullException("A parse rule cannot have null parse patterns or validators.");
			}

			MatchType = matchType;
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