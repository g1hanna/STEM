using System;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{
	public interface IParseResult : ICloneable
	{
		ParseStatus Status { get; set; }
		ASTNode Node { get; set; }
		int Start { get; set; }
		int Length { get; }

		IParseResult Offset(int offset);
	}

	public static class IParseResultTools
	{
		public static int GetEnd(this IParseResult result)
		{
			return result.Start + result.Length;
		}

		public static IParseResult Move(this IParseResult result, int position)
		{
			return result.Offset(position - result.Start);
		}

		public static ParseMatchType GetMatchType(this IParseResult result)
		{
			if (result.Node == null) return ParseMatchType.None;
			else return result.Node.MatchType;
		}
	}
}