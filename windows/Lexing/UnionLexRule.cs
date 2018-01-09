using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StemInterpretter.Lexing {

	public class UnionLexRule : ILexRule, ICollection<ILexRule>
	{
		private LexMatchType _matchType;
		private ICollection<ILexRule> _rules;

		public LexMatchType MatchType { get => _matchType; set => _matchType = value; }

		public int Count => _rules.Count;

		public bool IsReadOnly => _rules.IsReadOnly;

		public UnionLexRule(LexMatchType matchType)
		: this(matchType, new List<ILexRule>())
		{
			return;
		}

		public UnionLexRule(LexMatchType matchType, ICollection<ILexRule> rules)
		{
			_matchType = matchType;
			_rules = rules;
		}

		public ILexResult Match(string target)
		{
			foreach (ILexRule rule in _rules) {
				ILexResult result = rule.Match(target);

				if (result.IsSuccessful()) {
					return new LayeredLexNode(_matchType, result);
				}
			}

			return LexNode.NoMatch;
		}

		public void Add(ILexRule item)
		{
			_rules.Add(item);
		}

		public void Clear()
		{
			_rules.Clear();
		}

		public bool Contains(ILexRule item)
		{
			return _rules.Contains(item);
		}

		public void CopyTo(ILexRule[] array, int arrayIndex)
		{
			_rules.CopyTo(array, arrayIndex);
		}

		public bool Remove(ILexRule item)
		{
			return _rules.Remove(item);
		}

		public IEnumerator<ILexRule> GetEnumerator()
		{
			return _rules.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _rules.GetEnumerator();
		}
	}

}