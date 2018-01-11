using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StemInterpretter.Lexing {

	public class ContainerLexResult : ILexResultGroup
	{
		#region FIELDS AND AUTOS
		private ILexResultGroup _innerResults;
		private ILexResult _beginResult;
		private ILexResult _endResult;
		public LexMatchType MatchType { get; set; }
		#endregion

		#region PROPERTIES
		private ILexResultGroup innerResults {
			get => _innerResults;
			set {
				_innerResults = value;

				snapResults();
			}
		}

		public string ContainerText {
			get => innerResults.ContainerText;
			set {
				innerResults.ContainerText = value;

				snapResults();
			}
		}

		public string Text {
			get {
				return $"{BeginResult.Text}{innerResults.Text}{EndResult.Text}"; 
			}
		}

		public int Length {
			get {
				if (IsBeginningless()) return _innerResults.Length;
				else if (IsInconclusive()) {
					if (ContainerText != null) {
						return ContainerText.Length + _beginResult.Length + Start;
					}
					else {
						return _innerResults.Length;
					}
				}
				else {
					int length = _endResult.GetEnd() - Start;
					return length;
				}
			}
		}

		public int Count => ((ICollection<ILexResult>)_innerResults).Count + 2;

		public bool IsReadOnly => ((ICollection<ILexResult>)_innerResults).IsReadOnly;

		public ILexResult BeginResult {
			get => _beginResult;
			set {
				_beginResult = value;

				snapResults();
			}
		}
		public ILexResult EndResult {
			get => _endResult;
			set {
				_endResult = value;

				snapResults();
			}
		}
		public int Start {
			get => BeginResult.Start;
			set {
				BeginResult.Start = value;

				snapResults();
			}
		}

		public bool RequiresFilledSpace 
		{
			get => _innerResults.RequiresFilledSpace;
			set => _innerResults.RequiresFilledSpace = value;
		}

		public ILexResult this[int index] {
			get {
				int count = Count;

				// return begin result as first item
				if (index == 0) return _beginResult;
				// return end result as last item
				else if (index == count - 1) return _endResult;
				// else, return from inner results
				else return _innerResults[index];
			}
		}
		#endregion

		#region CONSTRUCTORS
		private ContainerLexResult()
		{
			return;
		}

		public ContainerLexResult(LexMatchType matchType, ILexResult beginResult, ILexResult endResult,
			string containerText, bool requiresFilledSpace)
		{
			MatchType = matchType;
			_beginResult = beginResult;
			_endResult = endResult;
			_innerResults = new LexResultGroup(MatchType, Start);

			ContainerText = containerText;
			RequiresFilledSpace = requiresFilledSpace;
		}
		
		public ContainerLexResult(LexMatchType matchType, ILexResult beginResult, ILexResult endResult, string containerText)
		: this(matchType, beginResult, endResult, containerText, false)
		{
			return;
		}

		public ContainerLexResult(LexMatchType matchType, ILexResult beginResult, ILexResult endResult)
		: this(matchType, beginResult, endResult, "")
		{
			return;
		}

		public ContainerLexResult(LexMatchType matchType, int start, string containerText, bool requiresFilledSpace)
		{
			// set fields and auto-implemented properties first
			MatchType = matchType;
			_beginResult = LexNode.NoMatch;
			_endResult = LexNode.NoMatch;
			_innerResults = new LexResultGroup(MatchType, start);

			Start = start;
			ContainerText = containerText;
		}

		public ContainerLexResult(LexMatchType matchType, int start, string containerText)
		: this(matchType, start, containerText, false)
		{
			return;
		}

		public ContainerLexResult(LexMatchType matchType, int start)
		: this(matchType, start, "")
		{
			return;
		}

		public ContainerLexResult(LexMatchType matchType)
		: this(matchType, 0, "")
		{
			return;
		}
		#endregion

		#region METHODS
		private void snapResults()
		{
			_innerResults.Start = _beginResult.GetEnd();

			_endResult.Start = innerResults.GetEnd();
		}

		public bool Add(ILexResult item)
		{
			if (overlapsBoundaries(item)) return false;

			if (!_innerResults.IsInside(item)) return false;

			return _innerResults.Add(item);
		}

		void ICollection<ILexResult>.Add(ILexResult item)
		{
			Add(item);
		}

		public void Clear()
		{
			((ICollection<ILexResult>)_innerResults).Clear();
		}

		public object Clone()
		{
			ContainerLexResult clone = new ContainerLexResult() {
				_beginResult = _beginResult.Clone() as ILexResult,
				_endResult = _endResult.Clone() as ILexResult,
				_innerResults = _innerResults.Clone() as LexResultGroup,
				MatchType = MatchType
			};
			
			clone.snapResults();

			return clone;
		}

		private ICollection<ILexResult> getAllResults() {
			List<ILexResult> allResults = new List<ILexResult>();

			allResults.Add(_beginResult);
			foreach (ILexResult result in _innerResults) allResults.Add(result);
			allResults.Add(_endResult);

			return allResults;
		}

		public bool Contains(ILexResult item)
		{
			return getAllResults().Contains(item);
		}

		public void CopyTo(ILexResult[] array, int arrayIndex)
		{
			getAllResults().CopyTo(array, arrayIndex);
		}

		public IEnumerator<ILexResult> GetEnumerator()
		{
			return getAllResults().GetEnumerator();
		}

		public bool IsBeginningless()
		{
			return (BeginResult.IsUnmatched());
		}

		public bool IsInconclusive()
		{
			return (EndResult.IsUnmatched());
		}

		public bool IsBoundless()
		{
			return IsBeginningless() && IsInconclusive();
		}

		public bool IsConfined()
		{
			return !IsBeginningless() && !IsInconclusive();
		}

		private bool overlapsBoundaries(ILexResult target) {
			if (_beginResult != null) {
				if (!_beginResult.IsUnmatched()) {
					if (_beginResult.Overlaps(target)) return true;
				}
			}

			if (_endResult != null) {
				if (!_endResult.IsUnmatched()) {
					if (_endResult.Overlaps(target)) return true;
				}
			}

			return false;
		}

		public bool Remove(ILexResult item)
		{
			return ((ICollection<ILexResult>)_innerResults).Remove(item);
		}

		public ILexResultGroup ToGroup()
		{
			return (Clone() as ContainerLexResult)._innerResults;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public ILexResult Offset(int offset)
		{
			ContainerLexResult copy = Clone() as ContainerLexResult;

			copy._beginResult = copy._beginResult.Offset(offset);
			copy._endResult = copy._endResult.Offset(offset);
			copy._innerResults = copy._innerResults.Offset(offset) as ILexResultGroup;

			return copy;
		}

		public ILexResult Move(int position)
		{
			return Offset(position - Start);
		}

		public bool AddEmptyNodes()
		{
			return _innerResults.AddEmptyNodes();
		}

		public bool OverlapsAt(int position, int length)
		{
			if (_beginResult.OverlapsAt(position, length)) return true;
			else if (_endResult.OverlapsAt(position, length)) return true;
			else return ((ILexResultGroup)_innerResults).OverlapsAt(position, length);
		}
		#endregion
	}

}