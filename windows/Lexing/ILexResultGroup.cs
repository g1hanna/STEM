using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StemInterpretter.Lexing {

	public interface ILexResultGroup : ILexResult, ICollection<ILexResult>
	{
		bool RequiresFilledSpace { get; set; }
		string ContainerText { get; set; }
		ILexResult this[int index] { get; }

		new bool Add(ILexResult item);
		bool AddEmptyNodes();
		bool OverlapsAt(int position, int length);
	}

	public static class ILexResultGroupTools
	{
		public static bool Overlaps(this ILexResultGroup group, ILexResult target)
		{
			return group.OverlapsAt(target.Start, target.Length);
		}

		public static bool IsInside(this ILexResultGroup group, ILexResult target)
		{
			int targetEnd = target.GetEnd();

			if ((target.Start >= group.Start && target.Start <= group.GetEnd())
				&& (targetEnd >= group.Start && targetEnd <= group.GetEnd())) return true;
			else return false;
		}

		public static bool Overlaps(this ILexResult result, ILexResult target)
		{
			return result.OverlapsAt(target.Start, target.Length);
		}

		public static bool OverlapsAt(this ILexResult result, int position, int length)
		{
			int end = position + length;

			// has some length
			if (length > 0) {
				return ((position >= result.Start && position < result.GetEnd())
				|| (end > result.Start && end <= result.GetEnd()));
			}
			// zero-length nodes
			else if (length == 0) {
				return (position > result.Start && position < result.GetEnd());
			}
			// negative lengths canceled
			else {
				throw new InvalidOperationException("Negative lengths not allowed.");
			}
		}
	}

}