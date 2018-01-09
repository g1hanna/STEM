using Microsoft.VisualStudio.TestTools.UnitTesting;
using StemInterpretter.Lexing;

namespace LexingTest
{
	[TestClass]
	public class LexingTest
	{
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

			Assert.AreEqual(result.Text, expected);
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

			Assert.AreEqual(result.Text, expected);
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

			Assert.AreEqual(result.Text, expected);
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
	}
}
