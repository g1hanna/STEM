using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StemInterpretter.Lexing {

	public interface ILexResult : ICloneable {
		LexMatchType MatchType { get; set; }
		string Text { get; }
		int Start { get; set; }
		int Length { get; }
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

		public static ILexResult Move(this ILexResult result, int position) {
			ILexResult movedResult = result.Clone() as ILexResult;

			movedResult.Start = position;
			return movedResult;
		}

		public static ILexResult Offset(this ILexResult result, int offset) {
			return result.Move(result.Start + offset);
		}

		public static bool Overlaps(this ILexResult result, ILexResult target) {
			if (target.Start > result.Start && target.Start <= result.GetEnd()) {
				return true;
			}
			else if (target.GetEnd() > result.Start && target.GetEnd() <= result.GetEnd()) {
				return true;
			}
			else {
				return false;
			}
		}
	}

}