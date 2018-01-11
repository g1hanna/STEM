using System;
using System.Collections;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public class Lexer : IList<ILexRule>
	{
		#region FIELDS
		private List<ILexRule> _rules;
		#endregion

		#region PROPERTIES
		public ILexRule this[int index] { get => ((IList<ILexRule>)_rules)[index]; set => ((IList<ILexRule>)_rules)[index] = value; }
		public int Count => ((IList<ILexRule>)_rules).Count;
		public bool IsReadOnly => ((IList<ILexRule>)_rules).IsReadOnly;
		#endregion

		#region CONSTRUCTORS
		public Lexer()
		{
			_rules = new List<ILexRule>();
		}

		public Lexer(IEnumerable<ILexRule> rules) : this()
		{
			AddMultiple(rules);
		}

		public Lexer(params ILexRule[] rules) : this(rules as IEnumerable<ILexRule>)
		{
			return;
		}
		#endregion

		#region METHODS
		#region ILIST<ILEXRULE> SUPPORT
		public void Add(ILexRule item)
		{
			((IList<ILexRule>)_rules).Add(item);
		}

		public void Clear()
		{
			((IList<ILexRule>)_rules).Clear();
		}

		public bool Contains(ILexRule item)
		{
			return ((IList<ILexRule>)_rules).Contains(item);
		}

		public void CopyTo(ILexRule[] array, int arrayIndex)
		{
			((IList<ILexRule>)_rules).CopyTo(array, arrayIndex);
		}

		public IEnumerator<ILexRule> GetEnumerator()
		{
			return ((IList<ILexRule>)_rules).GetEnumerator();
		}

		public int IndexOf(ILexRule item)
		{
			return ((IList<ILexRule>)_rules).IndexOf(item);
		}

		public void Insert(int index, ILexRule item)
		{
			((IList<ILexRule>)_rules).Insert(index, item);
		}

		public bool Remove(ILexRule item)
		{
			return ((IList<ILexRule>)_rules).Remove(item);
		}

		public void RemoveAt(int index)
		{
			((IList<ILexRule>)_rules).RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IList<ILexRule>)_rules).GetEnumerator();
		}
		#endregion
		
		public void AddMultiple(IEnumerable<ILexRule> rules)
		{
			foreach (ILexRule rule in rules) {
				Add(rule);
			}
		}

		public void AddMultiple(params ILexRule[] rules)
		{
			AddMultiple(rules as IEnumerable<ILexRule>);
		}

		public LexResultGroup Lex(string source, bool addEmptyNodes)
		{
			LexResultGroup group = new LexResultGroup(LexMatchType.Program);
			group.ContainerText = source;

			// match to the source per rule
			foreach (ILexRule rule in _rules) {
				string eaten = source.Clone() as string;
				int offset = 0;

				do
				{
					ILexResult result = rule.Match(eaten);

					if (result.IsSuccessful()) {
						// add it to the group
						ILexResult offsetResult = result.Offset(offset);
						if (!group.Overlaps(offsetResult)) {
							group.Add(offsetResult);
						}

						// eat the string to the end of the match
						eaten = eaten.Remove(0, result.GetEnd());

						// increment the offset
						offset += result.GetEnd();

						// handle nullable results
						if (result.Length == 0) {
							// break on empty
							if (eaten.Length == 0) break;

							// step forward one
							eaten = eaten.Remove(0, 1);
							offset += 1;
						}
					}
					else {
						break;
					}
				}
				while (eaten.Length > 0);
			}

			if (addEmptyNodes)
			{
				group.AddEmptyNodes();
			}

			return group;
		}

		public LexResultGroup Lex(string source)
		{
			return Lex(source, true);
		}
		#endregion
	}

}