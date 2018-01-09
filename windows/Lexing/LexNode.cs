using System;
using System.Collections;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public struct LexNode : ILexResult {
		public LexMatchType MatchType { get; set; }
		public string Text { get; set; }
		public int Start { get; set; }

		public int Length => Text.Length;
		
		public LexNode(LexMatchType matchType, int start, string text)
		{
			MatchType = matchType;
			Text = text;
			Start = start;
		}

		public bool Matches(LexMatchType matchType)
		{
			return MatchType == matchType;
		}

		public static readonly LexNode NoMatch = new LexNode(LexMatchType.None, 0, "");

		public static IEnumerable<LexNode> WrapSingle(LexNode node)
		{
			return new LexNode[1] { node };
		}

		public object Clone()
		{
			return new LexNode(MatchType, Start, Text);
		}
	}

}