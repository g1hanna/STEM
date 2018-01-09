using System;
using System.Collections;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public class SequenceLexRule : ILexRule, ICollection<ILexRule>
	{
		private IList<ILexRule> _rules;

		public int Count => _rules.Count;

		public bool IsReadOnly => _rules.IsReadOnly;

		public LexMatchType MatchType { get; set; }

		public SequenceLexRule(LexMatchType matchType, IList<ILexRule> rules)
		{
			MatchType = matchType;
			_rules = rules;
		}

		public SequenceLexRule(LexMatchType matchType)
		: this(matchType, new List<ILexRule>())
		{
			return;
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

		public IEnumerator<ILexRule> GetEnumerator()
		{
			return _rules.GetEnumerator();
		}

		public ILexResult Match(string target)
		{
			// if no rules, no match
			if (_rules.Count == 0) return LexNode.NoMatch;

			LexResultGroup group = new LexResultGroup(MatchType);

			// get first match, then offset
			ILexResult firstResult = _rules[0].Match(target);
			if (!firstResult.IsSuccessful()) return LexNode.NoMatch;
			
			string eaten = target.Clone() as string;

			group.Start = firstResult.Start;
			group.Add(firstResult);

			// eat the string until after the match
			eaten = eaten.Remove(0, firstResult.GetEnd());
			int offset = firstResult.GetEnd();

			// go through each match after the first
			for (int i = 1; i < Count; i++) {
				ILexResult result = _rules[i].Match(eaten);

				// ensure all pieces in the sequence succeed
				if (result.IsSuccessful()) {
					// matches must be "squared up"
					if (result.Start != 0) {
						return LexNode.NoMatch;
					}
					else {
						// eat the string
						eaten = eaten.Remove(0, result.GetEnd());

						// store an offset copy to the group
						group.Add(result.Offset(offset));
						
						// offset the offset
						offset += result.GetEnd();
					}
				}
				else if (result.MatchType == LexMatchType.Invalid) {
					return result.Offset(offset);
				}
				else {
					return LexNode.NoMatch;
				}
			}

			return group;
		}

		public bool Remove(ILexRule item)
		{
			return _rules.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _rules.GetEnumerator();
		}
	}

}