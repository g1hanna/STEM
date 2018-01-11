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

			IParseRule unlexedTokenRule = new ParseRule(parser, checker);

			DetailedParseResult result = unlexedTokenRule.Parse(lexedSource[1]) as DetailedParseResult;

			Assert.AreEqual("Unlexed token `samIAm' detected at character 3.", result.Message);
		}

		[TestMethod]
		public void ProgramRuleTest()
		{
			LexResultGroup lexedSource = _testLexer.Lex("\"I have a \\\"thing\\\" for you.\" 33.45");

			LexResultValidator checker = lexed => lexed.MatchType == LexMatchType.Program && lexed is LexResultGroup;
			LexResultParser parser = lexed => {
				ASTNode programNode = new ASTNode(ParseMatchType.Program, lexed);
				LexResultGroup group = lexed as LexResultGroup;

				foreach (ILexResult item in group) {
					ASTNode node = new ASTNode(item);

					if (item.MatchType != LexMatchType.Whitespace && item.MatchType != LexMatchType.Null) {
						node.MatchType = ParseMatchType.Expression;
					}
					else {
						node.MatchType = ParseMatchType.Ignore;
					}

					programNode.Add(node);
				}

				return new ParseResult(ParseStatus.Success, programNode);
			};

			IParseRule programRule = new ParseRule(parser, checker);

			IParseResult result = programRule.Parse(lexedSource);

			Assert.AreEqual("\"I have a \\\"thing\\\" for you.\"", result.Node[0].Text);
			Assert.IsTrue(result.Node[0].MatchType == ParseMatchType.Expression);

			Assert.AreEqual(" ", result.Node[1].Text);
			Assert.IsTrue(result.Node[1].MatchType == ParseMatchType.Ignore);

			Assert.AreEqual("33.45", result.Node[2].Text);
			Assert.IsTrue(result.Node[2].MatchType == ParseMatchType.Expression);
		}

		[TestCleanup]
		public void DisposeParsingTest()
		{
			_testLexer.Clear();

			_testLexer = null;
		}
	}
}
