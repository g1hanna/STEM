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
		public void Init()
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
			ILexRule escapeQuotRule = new SimpleLexRule(LexMatchType.LitStringEscape, "\\\\\\\"");
			ContainerLexRule stringRule = new ContainerLexRule(LexMatchType.LitString, stringQuotRule, stringQuotRule);

			stringRule.Add(escapeQuotRule);
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
		public void UnlexedRuleTest()
		{
			LexResultGroup lexedSource = _testLexer.Lex("   samIAm       ");

			LexResultValidator checker = lexed => lexed.MatchType == LexMatchType.None;
			LexResultParser parser = lexed => {
				return DetailedParseResult.CreateError(
					new ASTNode(ParseMatchType.None, lexed),
					$"Unlexed token `{lexed.Text}' detected at character {lexed.Start}."
				);
			};

			IParseRule unlexedTokenRule = new ParseRule(checker, parser);

			DetailedParseResult result = unlexedTokenRule.Parse(lexedSource[1]) as DetailedParseResult;

			Assert.AreEqual("Unlexed token `samIAm' detected at character 3.", result.Message);
		}

		[TestMethod]
		public void ProgramRuleTest()
		{
			LexResultGroup lexedSource = _testLexer.Lex("\"I have a \\\"thing\\\" for you.\" 33.45");

			ParseRule expressionRule = new ParseRule(
				l => l.MatchType != LexMatchType.Whitespace && l.MatchType != LexMatchType.None,
				l => new ParseResult(ParseStatus.Success, new ASTNode(ParseMatchType.Expression, l))
			);
			ParseRule ignoreRule = new ParseRule(
				l => l.MatchType == LexMatchType.Whitespace,
				l => new ParseResult(ParseStatus.Success, new ASTNode(ParseMatchType.Ignore, l))
			);
			ParseRule unlexedRule = new ParseRule(
				l => l.MatchType == LexMatchType.None,
				l => DetailedParseResult.CreateError(
					new ASTNode(ParseMatchType.Invalid, l),
					$"Unlexed token `{l.Text}' detected at character {l.Start}."
				)
			);

			ContainerParseRule programRule = new ContainerParseRule(ParseMatchType.Program, LexMatchType.Program);
			programRule.Add(unlexedRule);
			programRule.Add(expressionRule);
			programRule.Add(ignoreRule);

			IParseResult result = programRule.Parse(lexedSource);

			Assert.AreEqual("\"I have a \\\"thing\\\" for you.\"", result.Node[0].Text);
			Assert.IsTrue(result.Node[0].MatchType == ParseMatchType.Expression);

			Assert.AreEqual(" ", result.Node[1].Text);
			Assert.IsTrue(result.Node[1].MatchType == ParseMatchType.Ignore);

			Assert.AreEqual("33.45", result.Node[2].Text);
			Assert.IsTrue(result.Node[2].MatchType == ParseMatchType.Expression);
		}

		[TestMethod]
		public void ParserRuleTest()
		{
			// initialize parser and parse rules
			ParseRule expressionRule = new ParseRule(
				ParseMatchType.Expression,
				l => l.MatchType != LexMatchType.Whitespace && l.MatchType != LexMatchType.None,
				l => new ParseResult(ParseStatus.Success, new ASTNode(ParseMatchType.Expression, l))
			);
			ParseRule ignoreRule = new ParseRule(
				ParseMatchType.Ignore,
				l => l.MatchType == LexMatchType.Whitespace,
				l => new ParseResult(ParseStatus.Success, new ASTNode(ParseMatchType.Ignore, l))
			);
			ParseRule unlexedRule = new ParseRule(
				ParseMatchType.Invalid,
				l => l.MatchType == LexMatchType.None,
				l => DetailedParseResult.CreateError(
					new ASTNode(ParseMatchType.Invalid, l),
					$"Unlexed token `{l.Text}' detected at character {l.Start}."
				)
			);

			Parser myParser = new Parser();
			IParseResultGroup parsedSource;

			// lex the source
			LexResultGroup lexedSource = _testLexer.Lex("\"That\" 1 true 23.9  !");

			// test vigorously
			// Test 1: No rules
			parsedSource = myParser.Parse(lexedSource);

			Assert.IsTrue(parsedSource.Count == 0);

			// Test 2: With rules added
			myParser.AddMultiple(unlexedRule, expressionRule, ignoreRule);
			parsedSource = myParser.Parse(lexedSource);

			Assert.AreEqual("\"That\"", parsedSource[0].Node.Text);
			Assert.IsTrue(parsedSource[0].GetMatchType() == ParseMatchType.Expression);

			Assert.AreEqual("!", parsedSource[8].Node.Text);
			Assert.IsTrue(parsedSource[8].GetMatchType() == ParseMatchType.Invalid);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_testLexer.Clear();

			_testLexer = null;
		}
	}
}
