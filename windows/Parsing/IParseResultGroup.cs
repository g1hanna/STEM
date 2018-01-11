using System;
using System.Collections;
using System.Collections.Generic;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{
	public interface IParseResultGroup : IParseResult, ICollection<IParseResult>
	{
		IParseResult Find(Func<IParseResult, bool> selector);
		IParseResult[] FindAll(Func<IParseResult, bool> selector);
		bool Remove(Func<IParseResult, bool> selector);
		bool RemoveAll(Func<IParseResult, bool> selector);
	}

	public static class IParseResultGroupTools
	{
		public static bool ContainsParseWarnings(this IParseResultGroup group)
		{
			return group.Find(r => r.Status == ParseStatus.Warning) != null;
		}

		public static IParseResult[] GetParseWarnings(this IParseResultGroup group)
		{
			return group.FindAll(r => r.Status == ParseStatus.Warning);
		}

		public static bool ContainsParseErrors(this IParseResultGroup group)
		{
			return group.Find(r => r.Status == ParseStatus.Error) != null;
		}

		public static IParseResult[] GetParseErrors(this IParseResultGroup group)
		{
			return group.FindAll(r => r.Status == ParseStatus.Error);
		}
	}
}