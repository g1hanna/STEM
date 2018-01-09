using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StemInterpretter.Lexing {

	public class LexResultGroup : ILexResult, ICollection<ILexResult> {
		#region FIELDS
		private List<ILexResult> _matches;
		#endregion

		#region PROPERTIES
		public int Count => ((IList<ILexResult>)_matches).Count;
		public bool IsReadOnly => ((IList<ILexResult>)_matches).IsReadOnly;
		public LexMatchType MatchType { get; set; }
		public string Text {
			get {
				int length = Length;

				string text = "";
				for (int i = 0; i < Count; i++) {
					text = text.Insert(text.Length, _matches[i].Text);
				}

				return text;
			}
		}
		public int Start { get; set; }
		public int Length {
			get {
				// get the farthestmost result
				var farthest = _matches.OrderByDescending(r => r.Start + r.Length).FirstOrDefault();

				if (farthest == default(ILexResult) || farthest == null) {
					// return 0 if no items
					return 0;
				}
				else {
					// get the end of that result and subtract if from the start
					return farthest.GetEnd() - Start;
				}
			}
		}
		#endregion

		#region CONSTRUCTORS
		private LexResultGroup() {
			return;
		}

		public LexResultGroup(LexMatchType matchType, int start) {
			_matches = new List<ILexResult>();
			MatchType = matchType;
			Start = start;
		}

		public LexResultGroup(LexMatchType matchType) : this(matchType, 0) {
			return;
		}
		#endregion

		#region METHODS
		private void sort() {
			_matches.Sort((r1, r2) => {
				if (r1.Start > r2.Start) return 1;
				else if (r1.Start < r2.Start) return -1;
				else return 0;
			});
		}

		public bool Add(ILexResult item)
		{
			if (Overlaps(item)) return false;

			((IList<ILexResult>)_matches).Add(item);
			sort();
			return false;
		}

		void ICollection<ILexResult>.Add(ILexResult item)
		{
			Add(item);
		}

		public void Clear()
		{
			((IList<ILexResult>)_matches).Clear();
		}

		public bool Contains(ILexResult item)
		{
			return ((IList<ILexResult>)_matches).Contains(item);
		}

		public void CopyTo(ILexResult[] array, int arrayIndex)
		{
			((IList<ILexResult>)_matches).CopyTo(array, arrayIndex);
		}

		public IEnumerator<ILexResult> GetEnumerator()
		{
			return ((IList<ILexResult>)_matches).GetEnumerator();
		}

		public bool Overlaps(ILexResult target)
		{
			foreach (var item in _matches) {
				if (item.Overlaps(target)) return true;
			}

			return false;
		}

		public bool Remove(ILexResult item)
		{
			bool status = ((IList<ILexResult>)_matches).Remove(item);
			//sort();
			return status;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IList<ILexResult>)_matches).GetEnumerator();
		}

		public object Clone()
		{
			List<ILexResult> matchesList = new List<ILexResult>();
			foreach (var item in _matches) {
				matchesList.Add(item);
			}

			return new LexResultGroup() {
				_matches = matchesList,
				MatchType = this.MatchType,
				Start = this.Start
			};
		}
		#endregion
	}

}