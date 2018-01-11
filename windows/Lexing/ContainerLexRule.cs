using System;
using System.Collections;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public class ContainerLexRule : ILexGroupRule, ICollection<ILexRule>
	{
		#region FIELDS
		private IList<ILexRule> _innerRules;
		private ILexRule _beginRule;
		private ILexRule _endRule;
		private LexMatchType _matchType;
		private bool _requiresFilledSpace;
		#endregion

		#region PROPERTIES
		public LexMatchType MatchType { get => _matchType; set => _matchType = value; }
		public int Count => _innerRules.Count;
		public bool IsReadOnly => _innerRules.IsReadOnly;
		public ILexRule BeginRule { get => _beginRule; set => _beginRule = value; }
		public ILexRule EndRule { get => _endRule; set => _endRule = value; }
		public bool RequiresFilledSpace { get => _requiresFilledSpace; set => _requiresFilledSpace = value; }
		#endregion

		#region CONSTRUCTORS
		public ContainerLexRule(LexMatchType matchType, ILexRule beginRule, ILexRule endRule, bool requiresFilledSpace)
		{
			_matchType = matchType;
			_beginRule = beginRule;
			_endRule = endRule;
			_innerRules = new List<ILexRule>();
			_requiresFilledSpace = requiresFilledSpace;
		}

		public ContainerLexRule(LexMatchType matchType, ILexRule beginRule, ILexRule endRule)
		: this(matchType, beginRule, endRule, false)
		{
			return;
		}

		public ContainerLexRule(LexMatchType matchType)
		: this(matchType, null, null, false)
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

		private ICollection<ILexRule> getAllRules()
		{
			List<ILexRule> _totalRules = new List<ILexRule>();

			// add begin and end rules
			_totalRules.Add(_beginRule);
			foreach (var rule in _innerRules) _totalRules.Add(rule);
			_totalRules.Add(_endRule);

			return _totalRules;
		}

		public bool IsConfined()
		{
			return !IsBeginningless() && !IsInconclusive();
		}

		public bool IsBoundless()
		{
			return IsBeginningless() && IsInconclusive();
		}

		public bool IsBeginningless()
		{
			if (BeginRule == null) {
				return true;
			}
			else {
				return (BeginRule.MatchType == LexMatchType.None);
			}
		}

		public bool IsInconclusive()
		{
			if (EndRule == null) {
				return true;
			}
			else {
				return (EndRule.MatchType == LexMatchType.None);
			}
		}

		public ILexResult Match(string target)
		{
			return Match(target, RequiresFilledSpace);
		}

		public bool Remove(ILexRule item)
		{
			return _innerRules.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public ILexResult Match(string target, bool insertEmptyNodes)
		{
			ContainerLexResult container = new ContainerLexResult(_matchType);
			string eaten = target.Clone() as string;
			int offset = 0;

			// match beginning and ending rules first
			if (BeginRule != null) {
				var beginResult = BeginRule.Match(eaten);

				if (!beginResult.IsSuccessful()) return LexNode.NoMatch;

				container.BeginResult = beginResult;
				offset = beginResult.GetEnd();

				if (!beginResult.IsUnmatched()) {
					eaten = eaten.Remove(0, beginResult.GetEnd());
				}
			}
			else {
				// return no match if there is no beginning
				return LexNode.NoMatch;
			}

			if (EndRule != null) {
				// offset the result if there is any
				var endResult = EndRule.Match(eaten);

				container.EndResult = endResult.Offset(offset);

				if (!endResult.IsUnmatched()) {
					eaten = eaten.Remove(endResult.Start);
				}
			}
			else {
				container.EndResult = LexNode.NoMatch;
			}

			container.ContainerText = eaten;

			// for each rule, find matches associated with it
			foreach (var rule in _innerRules) {
				string ruleEaten = eaten;
				int ruleOffset = offset;

				do
				{
					ILexResult result = rule.Match(ruleEaten);

					// break if no more matches can be found
					if (result.IsUnmatched()) {
						break;
					}
					else {
						// add an offset result to the container
						ILexResult offsetResult = result.Offset(offset);
						if (!container.Overlaps(offsetResult))
							container.Add(offsetResult);

						// increment offset by 1 for zero-length matches
						if (result.Length == 0) {
							if (ruleEaten.Length == 0) break;
							
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
				while (ruleEaten.Length > 0);
			}

			if (insertEmptyNodes) container.AddEmptyNodes();

			return container;
		}
		#endregion
	}

}