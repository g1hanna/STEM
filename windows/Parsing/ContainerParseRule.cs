using System;
using System.Collections;
using System.Collections.Generic;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{

	public class ContainerParseRule : IParseRule, ICollection<IParseRule>
	{
		#region FIELDS AND AUTOS
		private ICollection<IParseRule> _parseRules;
		public ParseMatchType MatchType { get; set; }
		public LexResultValidator Validator { get; set; }
		#endregion

		#region PROPERTIES
		public int Count => _parseRules.Count;

		public bool IsReadOnly => _parseRules.IsReadOnly;
		#endregion

		#region CONSTRUCTORS
		public ContainerParseRule(ParseMatchType matchType, LexResultValidator validator, ICollection<IParseRule> parseRules)
		{
			_parseRules = parseRules;
			MatchType = matchType;
			Validator = validator;
		}

		public ContainerParseRule(ParseMatchType matchType, LexResultValidator validator)
		: this(matchType, validator, new List<IParseRule>())
		{
			return;
		}

		public ContainerParseRule(ParseMatchType matchType, LexMatchType lexMatchType)
		: this(matchType, l => l.MatchType == lexMatchType, new List<IParseRule>())
		{
			return;
		}
		#endregion

		#region METHODS
		#region ICOLLECTION<IPARSERULE> SUPPORT
		public void Add(IParseRule item)
		{
			_parseRules.Add(item);
		}

		public void Clear()
		{
			_parseRules.Clear();
		}

		public bool Contains(IParseRule item)
		{
			return _parseRules.Contains(item);
		}

		public void CopyTo(IParseRule[] array, int arrayIndex)
		{
			_parseRules.CopyTo(array, arrayIndex);
		}

		public IEnumerator<IParseRule> GetEnumerator()
		{
			return _parseRules.GetEnumerator();
		}

		public bool Remove(IParseRule item)
		{
			return _parseRules.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _parseRules.GetEnumerator();
		}
		#endregion

		public bool Accepts(ILexResult target)
		{
			return Validator(target);
		}

		public IParseResult Parse(ILexResult target)
		{
			if (!Accepts(target)) return ParseResult.Empty;

			ASTNode containerNode = new ASTNode(target);
			containerNode.MatchType = MatchType;
			IParseResultGroup resultGroup = new ParseResultGroup(ParseStatus.Success, containerNode);

			Action<IParseResult> prAdder = pr => {
				if (pr.Status != ParseStatus.None && pr.Node.MatchType != ParseMatchType.None) {
					containerNode.Add(pr.Node);
					resultGroup.Add(pr);
				}
			};

			foreach (IParseRule rule in _parseRules) {
				if (target is ILexResultGroup) {
					ILexResultGroup targetGroup = target as ILexResultGroup;
					foreach (ILexResult lexResult in targetGroup) {
						IParseResult parseResult = rule.Parse(lexResult);
						prAdder(parseResult);
					}
				}
				else {
					IParseResult parseResult = rule.Parse(target);
					prAdder(parseResult);
				}
			}

			return resultGroup;
		}
		#endregion
	}

}