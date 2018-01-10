using StemInterpretter.Lexing;
using StemInterpretter.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StemInterpretter.Tests.Parsing
{
	[TestClass]
	public class ParsingTest
	{
		private Lexer _testLexer;

		[TestInitialize]
		public void InitParsingTest()
		{
			#region INIT LEX RULES
			// initialize lex rules
			// - null
			ILexRule nullRule = new FunctionalLexRule(
				LexMatchType.Null,
				target => {
					if (string.IsNullOrEmpty(target))
						return new LexNode(LexMatchType.Null, 0, "");
					else
						return LexNode.NoMatch;
				}
			);
			// - literal integer
			ILexRule intRule = new SimpleLexRule(LexMatchType.LitInt, "\\b[0-9]+\\b");
			// - whitespace
			ILexRule whitespaceRule = new SimpleLexRule(LexMatchType.Whitespace, "\\s+");
			// - literal boolean
			ILexRule boolRule = new UnionLexRule(LexMatchType.LitBool,
				new ILexRule[] {
					// true pattern
					new SimpleLexRule(LexMatchType.LitBool, "\\btrue\\b"),
					// false pattern
					new SimpleLexRule(LexMatchType.LitBool, "\\bfalse\\b")
				}
			);
			// - literal floating-point number
			SequenceLexRule floatRule = new SequenceLexRule(LexMatchType.LitFloat);

			floatRule.Add(intRule);
			floatRule.Add(new SimpleLexRule(LexMatchType.LitFloatSep, "\\."));
			floatRule.Add(intRule);

			// - literal string
			ILexRule stringQuotRule = new SimpleLexRule(LexMatchType.LitStringQuot, "(?<!\\\\)\"");
			ILexRule escapeQuotRule = new SimpleLexRule(LexMatchType.LitStringEscape, "\\\"");
			ContainerLexRule stringRule = new ContainerLexRule(LexMatchType.LitString, stringQuotRule, stringQuotRule);
			#endregion

			#region INIT LEXER
			// initialize lexer
			Lexer myLexer = new Lexer();

			myLexer.AddMultiple(
				stringRule,
				boolRule,
				floatRule,
				intRule,
				whitespaceRule,
				nullRule
			);

			_testLexer = myLexer;
			#endregion
		}

		[TestMethod]
		public void ParseRuleTest()
		{
			
		}

		[TestCleanup]
		public void DisposeParsingTest()
		{
			_testLexer.Clear();

			_testLexer = null;
		}
	}
}
