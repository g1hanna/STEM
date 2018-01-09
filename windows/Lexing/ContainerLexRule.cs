using System;
using System.Collections;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public class ContainerLexRule : ILexRule, ICollection<ILexRule>
	{
		#region FIELDS
		private IList<ILexRule> _innerRules;
		private ILexRule _beginRule;
		private ILexRule _endRule;
		private LexMatchType _matchType;
		#endregion

		#region PROPERTIES
		public LexMatchType MatchType { get => _matchType; set => _matchType = value; }
		public int Count => _innerRules.Count;
		public bool IsReadOnly => _innerRules.IsReadOnly;
		public ILexRule BeginRule { get => _beginRule; set => _beginRule = value; }
		public ILexRule EndRule { get => _endRule; set => _endRule = value; }
		#endregion

		#region CONSTRUCTORS
		public ContainerLexRule(LexMatchType matchType, ILexRule beginRule, ILexRule endRule)
		{
			_matchType = matchType;
			_beginRule = beginRule;
			_endRule = endRule;
			_innerRules = new List<ILexRule>();
		}

		public ContainerLexRule(LexMatchType matchType)
		: this(matchType, null, null)
		{
			return;
		}
		#endregion

		#region METHODS
		public void Add(ILexRule item)
		{
			_innerRules.Add(item);
		}

		public void Clear()
		{
			_innerRules.Clear();
		}

		public bool Contains(ILexRule item)
		{
			return getAllRules().Contains(item);
		}

		public void CopyTo(ILexRule[] array, int arrayIndex)
		{
			getAllRules().CopyTo(array, arrayIndex);
		}

		public IEnumerator<ILexRule> GetEnumerator()
		{
			return getAllRules().GetEnumerator();
		}

		private ICollection<ILexRule> getAllRules() {
			List<ILexRule> _totalRules = new List<ILexRule>();

			// add begin and end rules
			_totalRules.Add(_beginRule);
			foreach (var rule in _innerRules) _totalRules.Add(rule);
			_totalRules.Add(_endRule);

			return _totalRules;
		}

		public ILexResult Match(string target)
		{
			ContainerLexResult container = new ContainerLexResult(_matchType);
			string eaten = target.Clone() as string;
			int offset = 0;

			// match beginning and ending rules first
			if (BeginRule != null) {
				container.BeginResult = BeginRule.Match(eaten);
				offset = container.Start;

				if (!container.BeginResult.IsUnmatched()) {
					eaten = eaten.Remove(0, container.BeginResult.GetEnd());
				}
			}

			if (EndRule == null) {
				// offset the result if there is any
				container.EndResult = EndRule.Match(eaten).Offset(offset);

				if (!container.EndResult.IsUnmatched()) {
					eaten = eaten.Remove(container.EndResult.Start, eaten.Length);
				}
			}

			// for each rule, find matches associated with it
			foreach (var rule in _innerRules) {
				string ruleEaten = eaten;
				int ruleOffset = offset;

				while (ruleEaten.Length > 0) {
					ILexResult result = rule.Match(ruleEaten);

					// break if no more matches can be found
					if (result.IsUnmatched()) {
						break;
					}
					else {
						// add an offset result to the container
						if (!container.Overlaps(result))
							container.Add(result.Offset(ruleOffset));

						// increment offset by 1 for zero-length matches
						if (result.Length == 0) {
							ruleEaten = ruleEaten.Remove(0, 1);
							ruleOffset += 1;
						}
						else {
							// eat the string and increment offset
							int resultEnd = result.GetEnd();
							ruleEaten = ruleEaten.Remove(0, resultEnd);
							ruleOffset += resultEnd;
						}
					}
				}
			}

			return container;
		}

		public bool Remove(ILexRule item)
		{
			return _innerRules.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion
	}

}