using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{
	public class Parser : ICollection<IParseRule>
	{
		#region FIELDS
		private ICollection<IParseRule> _parserRules;
		#endregion

		#region PROPERTIES
		public int RuleCount => _parserRules.Count;

		int ICollection<IParseRule>.Count => RuleCount;

		bool ICollection<IParseRule>.IsReadOnly => _parserRules.IsReadOnly;
		#endregion
		
		#region CONSTRUCTORS
		public Parser()
		{
			_parserRules = new List<IParseRule>();
		}
		#endregion

		#region METHODS
		public IParseResultGroup Parse(ILexResultGroup lexedSource)
		{
			// initialize group
			IParseResultGroup parseGroup = new ParseResultGroup(
				ParseStatus.Success,
				new ASTNode(ParseMatchType.Program, lexedSource)
			);

			Action<IParseResult> addItem = pr => {
				if (pr.Status != ParseStatus.None) {
					parseGroup.Node.Add(pr.Node);
					parseGroup.Add(pr);
				}
			};

			foreach (IParseRule rule in _parserRules) {
				foreach (ILexResult lexResult in lexedSource) {
					if (!rule.Accepts(lexResult)) continue;

					IParseResult parseResult = rule.Parse(lexResult);
					addItem(parseResult);
				}
			}

			return parseGroup;
		}

		public void Add(IParseRule item)
		{
			_parserRules.Add(item);
		}

		public void AddMultiple(IEnumerable<IParseRule> items)
		{
			foreach (IParseRule item in items) Add(item);
		}

		public void AddMultiple(params IParseRule[] items)
		{
			AddMultiple(items as IEnumerable<IParseRule>);
		}

		public void Clear()
		{
			_parserRules.Clear();
		}

		public bool Contains(IParseRule item)
		{
			return _parserRules.Contains(item);
		}

		public void CopyTo(IParseRule[] array, int arrayIndex)
		{
			_parserRules.CopyTo(array, arrayIndex);
		}

		public IEnumerator<IParseRule> GetEnumerator()
		{
			return _parserRules.GetEnumerator();
		}

		public bool Remove(IParseRule item)
		{
			return _parserRules.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _parserRules.GetEnumerator();
		}
		#endregion
	}
}