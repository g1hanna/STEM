using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{
	public class ParseResultGroup : IParseResultGroup
	{
		#region FIELDS AND AUTOS
		private IList<IParseResult> _results;
		public ParseStatus Status { get; set; }
		public ASTNode Node { get; set; }
		#endregion

		#region PROPERTIES
		public IParseResult this[int index] => _results.OrderBy(pr => pr.Start).ElementAt(index);
		public int Count => _results.Count;
		public bool IsReadOnly => _results.IsReadOnly;
		public int Start { get => Node.Start; set => Node.Start = value; }
		public int Length => Node.Length;
		#endregion

		#region CONSTRUCTORS
		private ParseResultGroup()
		{
			return;
		}

		public ParseResultGroup(ParseStatus status, ASTNode node, IList<IParseResult> results)
		{
			Status = status;
			Node = node;
			_results = results;
		}

		public ParseResultGroup(ParseStatus status, ASTNode node)
		: this(status, node, new List<IParseResult>())
		{
			return;
		}

		public ParseResultGroup(ParseStatus status)
		: this(status, ASTNode.Empty)
		{
			return;
		}

		public ParseResultGroup(ASTNode node)
		: this(ParseStatus.None, node)
		{
			return;
		}
		#endregion

		#region METHODS
		public void Add(IParseResult item)
		{
			_results.Add(item);
		}

		public void Clear()
		{
			_results.Clear();
		}

		public object Clone()
		{
			return Clone(true);
		}

		public object Clone(bool cloneContents)
		{
			IList<IParseResult> results = new List<IParseResult>();
			
			// add duplicates of this object's results
			if (cloneContents)
				foreach (IParseResult result in _results)
					results.Add(result.Clone() as IParseResult);
			
			// just make a new list; leave the results alone
			else
				foreach (IParseResult result in _results)
					results.Add(result);

			return new ParseResultGroup()
			{
				_results = results,
				Status = Status,
				Node = Node.Clone() as ASTNode
			};
		}

		public bool Contains(IParseResult item)
		{
			return _results.Contains(item);
		}

		public void CopyTo(IParseResult[] array, int arrayIndex)
		{
			_results.CopyTo(array, arrayIndex);
		}

		public IEnumerator<IParseResult> GetEnumerator()
		{
			return _results.GetEnumerator();
		}

		public IParseResult Offset(int offset)
		{
			throw new NotImplementedException();
		}

		public bool Remove(IParseResult item)
		{
			return _results.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _results.GetEnumerator();
		}

		public IParseResult Find(Func<IParseResult, bool> selector)
		{
			foreach (IParseResult result in _results) {
				if (selector(result)) return result;
			}

			return null;
		}

		public IParseResult[] FindAll(Func<IParseResult, bool> selector)
		{
			List<IParseResult> matches = new List<IParseResult>();

			foreach (IParseResult result in _results) {
				if (selector(result)) matches.Add(result);
			}

			return matches.ToArray();
		}

		public bool Remove(Func<IParseResult, bool> selector)
		{
			return Remove(Find(selector));
		}

		public bool RemoveAll(Func<IParseResult, bool> selector)
		{
			IParseResult[] matches = FindAll(selector);

			bool allRemoved = true;
			foreach (IParseResult match in matches) {
				if (!Remove(match)) {
					if (allRemoved) allRemoved = false;
				}
			}

			return allRemoved;
		}
		#endregion
	}
}