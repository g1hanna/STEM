using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StemInterpretter.Lexing {

	public interface ILexResult : ICloneable {
		LexMatchType MatchType { get; set; }
		string Text { get; }
		int Start { get; set; }
		int Length { get; }

		ILexResult Offset(int offset);
		ILexResult Move(int position);
	}

	public static class ILexResultTools {
		public static int GetEnd(this ILexResult result) {
			return result.Start + result.Length;
		}

		public static bool IsSuccessful(this ILexResult result) {
			return result.MatchType != LexMatchType.None
				&& result.MatchType != LexMatchType.Invalid;
		}

		public static bool IsUnmatched(this ILexResult result) {
			return result.MatchType == LexMatchType.None;
		}

		public static bool IsInvalid(this ILexResult result) {
			return result.MatchType == LexMatchType.Invalid;
		}

		public static bool IsPartial(this ILexResult result) {
			return result.MatchType == LexMatchType.Partial;
		}
	}

}