using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StemInterpretter.Lexing;

namespace LexingTest
{
	[TestClass]
	public class LexingTest
	{
		// This was for recalling the results of
		// string.PadLeft(int) and string.PadRight(int).

		// [TestMethod]
		// public void StringTest()
		// {
		// 	string myString = "I am a string!";

		// 	Assert.AreEqual("    I am a string!", myString.PadLeft(18));
		// 	Assert.AreEqual("I am a string!    ", myString.PadRight(18));
		// }

		[TestMethod]
		public void EmptyLexTest()
		{
			ILexRule nullRule = new FunctionalLexRule(
				LexMatchType.Null,
				target => {
					if (string.IsNullOrEmpty(target))
						return new LexNode(LexMatchType.Null, 0, "");
					else
						return LexNode.NoMatch;
				}
			);

			string source = "";
			string expected = "";

			ILexResult result = nullRule.Match(source);

			Assert.AreEqual(result.Text, expected);
		}

		[TestMethod]
		public void IntegerLexTest()
		{
			ILexRule intRule = new SimpleLexRule(LexMatchType.LitInt, "[0-9]+");

			string source = "834";
			string expected = "834";

			ILexResult result = intRule.Match(source);

			Assert.AreEqual(result.Text, expected);
		}

		[TestMethod]
		public void WhitespeceLexTest()
		{
			ILexRule whitespaceRule = new SimpleLexRule(LexMatchType.LitInt, "\\s+");

			string source = "     834   ";
			string expected = "     ";

			ILexResult result = whitespaceRule.Match(source);

			Assert.AreEqual(expected, result.Text);
		}

		[TestMethod]
		public void BoolLexTest()
		{
			ILexRule boolRule = new UnionLexRule(LexMatchType.LitBool,
			new ILexRule[] {
				// true pattern
				new SimpleLexRule(LexMatchType.LitBool, "\\btrue\\b"),
				// false pattern
				new SimpleLexRule(LexMatchType.LitBool, "\\bfalse\\b")
			});

			string source = "     true   ";
			string expected = "true";

			ILexResult result = boolRule.Match(source);

			Assert.AreEqual(result.Text, expected);

			source = "     false   ";
			expected = "false";

			result = boolRule.Match(source);

			Assert.AreEqual(expected, result.Text);
		}

		[TestMethod]
		public void FloatLexTest()
		{
			ILexRule intRule = new SimpleLexRule(LexMatchType.LitInt, "[0-9]+");
			SequenceLexRule floatRule = new SequenceLexRule(LexMatchType.LitFloat);

			floatRule.Add(intRule);
			floatRule.Add(new SimpleLexRule(LexMatchType.LitFloatSep, "\\."));
			floatRule.Add(intRule);

			string source = " 452.39  ";
			string expected = "452.39";

			ILexResult result = floatRule.Match(source);

			Assert.AreEqual(expected, result.Text);
		}

		[TestMethod]
		public void StringLexTest()
		{
			ILexRule stringQuotRule = new SimpleLexRule(LexMatchType.LitStringQuot, "(?<!\\\\)\"");
			ILexRule escapeQuotRule = new SimpleLexRule(LexMatchType.LitStringEscape, "\\\"");
			ContainerLexRule stringRule = new ContainerLexRule(LexMatchType.LitString, stringQuotRule, stringQuotRule);

			stringRule.Add(escapeQuotRule);

			string source = "  \"I am a string!\"  ";
			string expected = "\"I am a string!\"";

			ILexResult result = stringRule.Match(source);

			Assert.AreEqual(expected, result.Text);
		}

		[TestMethod]
		public void LexerTest()
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
			#endregion

			// prepare general variables
			string source;
			string expected;
			ILexResultGroup lexedSource;

			// test vigorously
			// 1) null
			source = "";
			expected = "";
			lexedSource = myLexer.Lex(source);

			Assert.AreEqual(expected, lexedSource[0].Text);

			// 2) whitespace, integers, and booleans
			source = "  672    true   786  ";
			lexedSource = myLexer.Lex(source);

			Assert.AreEqual("  ", lexedSource[0].Text);
			Assert.AreEqual("672", lexedSource[1].Text);
			Assert.AreEqual("    ", lexedSource[2].Text);
			Assert.AreEqual("true", lexedSource[3].Text);
			Assert.AreEqual("   ", lexedSource[4].Text);
			Assert.AreEqual("786", lexedSource[5].Text);
			Assert.AreEqual("  ", lexedSource[6].Text);

			// 3) whitespace, floats, and strings
			source = "  \"My value is 972.987.\"  972.987";
			lexedSource = myLexer.Lex(source);

			Assert.AreEqual("  ", lexedSource[0].Text);
			Assert.AreEqual("\"My value is 972.987.\"", lexedSource[1].Text);
			Assert.AreEqual("  ", lexedSource[2].Text);
			Assert.AreEqual("972.987", lexedSource[3].Text);

			// 4) unlexed
			source = "645true";
			lexedSource = myLexer.Lex(source);

			Assert.AreEqual("645true", lexedSource[0].Text);
			Assert.IsTrue(lexedSource[0].MatchType == LexMatchType.None);

			source = "   true  samIAm   ";
			lexedSource = myLexer.Lex(source);

			Assert.AreEqual("samIAm", lexedSource[3].Text);
			Assert.IsTrue(lexedSource[3].MatchType == LexMatchType.None);
		}
	}
}
