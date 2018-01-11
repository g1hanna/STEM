using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StemInterpretter.Lexing {

	public class LexResultGroup : ILexResultGroup
	{
		#region FIELDS AND AUTOS
		private List<ILexResult> _matches;
		public LexMatchType MatchType { get; set; }
		public string ContainerText { get; set; }
		private int _start;
		public bool RequiresFilledSpace { get; set; }
		#endregion

		#region PROPERTIES
		public int Count => ((IList<ILexResult>)_matches).Count;
		public bool IsReadOnly => ((IList<ILexResult>)_matches).IsReadOnly;
		public int Length => ContainerText.Length;

		public ILexResult this[int index] => _matches[index];
		public string Text {
			get => ContainerText;
		}

		public int Start {
			get => _start;
			set {
				int offset = value - _start;
				_start = value;

				// move each piece in the group
				for (int i = 0; i < _matches.Count; i++) {
					_matches[i].Start += offset;
				}
			}
		}
		#endregion

		#region CONSTRUCTORS
		private LexResultGroup()
		{
			return;
		}

		public LexResultGroup(LexMatchType matchType, int start, string text, bool requiresFilled)
		{
			ContainerText = text;
			_start = start;
			MatchType = matchType;
			RequiresFilledSpace = requiresFilled;
			_matches = new List<ILexResult>();
		}

		public LexResultGroup(LexMatchType matchType, int start, string text)
		: this(matchType, start, text, false)
		{
			return;
		}

		public LexResultGroup(LexMatchType matchType, int start)
		: this(matchType, start, "")
		{
			return;
		}

		public LexResultGroup(LexMatchType matchType, string text)
		: this(matchType, 0, text)
		{
			return;
		}

		public LexResultGroup(LexMatchType matchType) : this(matchType, 0, "")
		{
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
			else {
				((IList<ILexResult>)_matches).Add(item);
				sort();

				// expand the container to "catch" the item
				if (!this.IsInside(item)) {
					if (item.Start < Start) {
						// extend container to the left
						int expandedLength = (Start - item.Start) + ContainerText.Length;
						ContainerText = ContainerText.PadLeft(expandedLength);
						Start = item.Start;
					}
					if (item.GetEnd() > this.GetEnd()) {
						// extend container to the right
						int expandedLength = (item.GetEnd() - this.GetEnd()) + ContainerText.Length;
						ContainerText = ContainerText.PadRight(expandedLength);
					}
				}

				// it is now safe to assume that the item is in the container
				int start = item.Start - Start;

				// rewrite the container's string where the item is
				ContainerText = ContainerText.Remove(start, item.Length);
				ContainerText = ContainerText.Insert(start, item.Text);

				return true;
			}
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

		public LexResultGroup Offset(int offset)
		{
			LexResultGroup copy = Clone() as LexResultGroup;

			copy.Start += offset;

			for (int i = 0; i < _matches.Count; i++) {
				_matches[i].Start += offset;
			}

			return copy;
		}

		public LexResultGroup Move(int position)
		{
			return Offset(position - Start);
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
				matchesList.Add(item.Clone() as ILexResult);
			}

			return new LexResultGroup() {
				_matches = matchesList,
				ContainerText = ContainerText.Clone() as string,
				MatchType = MatchType,
				Start = Start,
				RequiresFilledSpace = RequiresFilledSpace
			};
		}

		ILexResult ILexResult.Offset(int offset)
		{
			LexResultGroup copy = Clone() as LexResultGroup;
			copy.Start += offset;

			for (int i = 0; i < copy._matches.Count; i++) {
				copy._matches[i] = copy._matches[i].Offset(offset);
			}

			return copy;
		}

		ILexResult ILexResult.Move(int position)
		{
			return Offset(position - Start);
		}

		public bool OverlapsAt(int position, int length)
		{
			int end = position + length;
			foreach (var result in _matches)
			{
				if (result.OverlapsAt(position, length)) return true;
			}

			return false;
		}

		public bool AddEmptyNodes()
		{
			// add empty nodes to empty slots
			bool slotsAdded = false;
			int end = Text.Length;
			for (int i = 0; i < end; i++)
			{
				bool placeEmpty = false;
				for (int j = 0; j + i <= end; j++)
				{
					if (OverlapsAt(i, j))
					{
						if (placeEmpty)
						{
							if (!slotsAdded) slotsAdded = true;

							LexNode emptyNode = LexNode.NoMatch;
							emptyNode.Start = i;
							emptyNode.Text = Text.Substring(i, j - 1);

							Add(emptyNode);

							i += j;
						}

						break;
					}
					else if (i + j == end)
					{
						if (placeEmpty)
						{
							if (!slotsAdded) slotsAdded = true;

							LexNode emptyNode = LexNode.NoMatch;
							emptyNode.Start = i;
							emptyNode.Text = Text.Substring(i, j);

							Add(emptyNode);

							i += j;
						}

						break;
					}
					else
					{
						if (j > 0 && !placeEmpty) placeEmpty = true;

						continue;
					}
				}
			}

			return slotsAdded;
		}
		#endregion
	}

}