using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StemInterpretter.Lexing {

	public class ContainerLexResult : ILexResult, ICollection<ILexResult> {
		#region FIELDS AND AUTOS
		private LexResultGroup _innerResults;
		private ILexResult _beginResult;
		private ILexResult _endResult;
		public string ContainerText { get; set; }
		public LexMatchType MatchType { get; set; }
		#endregion

		#region PROPERTIES
		public string Text {
			get {
				string result = "";

				// add beginning result text first
				result = result.Insert(0, _beginResult.Text);

				if (string.IsNullOrEmpty(ContainerText)) {
					result = result.Insert(result.Length, _innerResults.Text);
				}
				else {
					result = result.Insert(result.Length, ContainerText);
				}

				// add ending result text last
				result = result.Insert(result.Length, _endResult.Text);

				return result;
			}
		}

		public int Length {
			get {
				if (IsBeginningless()) return _innerResults.Length;
				else if (IsInconclusive()) {
					if (ContainerText != null) {
						return ContainerText.Length;
					}
					else {
						return _innerResults.Length;
					}
				}
				else {
					return _endResult.GetEnd() - Start;
				}
			}
		}

		public int Count => ((ICollection<ILexResult>)_innerResults).Count;

		public bool IsReadOnly => ((ICollection<ILexResult>)_innerResults).IsReadOnly;

		public ILexResult BeginResult { get => _beginResult; set => _beginResult = value; }
		public ILexResult EndResult { get => _endResult; set => _endResult = value; }
		public int Start { get => _beginResult.Start; set => _beginResult.Start = value; }

		#endregion

		#region CONSTRUCTORS
		private ContainerLexResult()
		{
			return;
		}
		
		public ContainerLexResult(LexMatchType matchType, ILexResult beginResult, ILexResult endResult, string containerText)
		{
			MatchType = matchType;
			_beginResult = beginResult;
			_endResult = endResult;
			_innerResults = new LexResultGroup(MatchType, Start);
			ContainerText = containerText;
		}

		public ContainerLexResult(LexMatchType matchType, ILexResult beginResult, ILexResult endResult)
		: this(matchType, beginResult, endResult, "")
		{
			return;
		}

		public ContainerLexResult(LexMatchType matchType, int start)
		{
			MatchType = matchType;
			_beginResult = LexNode.NoMatch;
			_endResult = LexNode.NoMatch;

			Start = start;
			_innerResults = new LexResultGroup(MatchType, start);
		}

		public ContainerLexResult(LexMatchType matchType)
		: this(matchType, 0)
		{
			return;
		}
		#endregion

		#region METHODS
		public void Add(ILexResult item)
		{
			if (overlapsBoundaries(item)) return;

			((ICollection<ILexResult>)_innerResults).Add(item);
		}

		public void Clear()
		{
			((ICollection<ILexResult>)_innerResults).Clear();
		}

		public object Clone()
		{
			return new ContainerLexResult() {
				_beginResult = _beginResult.Clone() as ILexResult,
				_endResult = _endResult.Clone() as ILexResult,
				_innerResults = _innerResults.Clone() as LexResultGroup,
				ContainerText = ContainerText.Clone() as string,
				MatchType = MatchType
			};
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

		public LexResultGroup ToGroup()
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
			copy._innerResults = copy._innerResults.Offset(offset);

			return copy;
		}

		public ILexResult Move(int position)
		{
			return Offset(position - Start);
		}
		#endregion
	}

}