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

			if (position > result.Start && position < result.GetEnd()) {
				return true;
			}
			else if (end > result.Start && end < result.GetEnd()) {
				return true;
			}
			else {
				return false;
			}
		}
	}

}