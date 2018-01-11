using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{

	public class ASTNode : ICollection<ASTNode>, ICloneable
	{
		#region STATIC MEMBERS
		public static readonly ASTNode Empty = new ASTNode() {
			Start = 0,
			Text = "",
			MatchType = ParseMatchType.None,
			_children = new List<ASTNode>()
		};
		#endregion
		
		#region FIELDS AND AUTOS
		private List<ASTNode> _children;
		public string Text { get; set; }
		public ParseMatchType MatchType { get; set; }
		public int Start { get; set; }
		#endregion

		#region PROPERTIES
		public ASTNode this[int index] { get => _children[index]; }

		public int Count => _children.Count;

		public bool IsReadOnly => ((ICollection<ASTNode>)_children).IsReadOnly;

		public int Length {
			get {
				if (Text != null) return Text.Length;
				else return 0;
			}
		}
		#endregion

		#region CONSTRUCTORS
		private ASTNode()
		{
			return;
		}

		public ASTNode(ParseMatchType matchType, int start, string text)
		{
			_children = new List<ASTNode>();
			MatchType = matchType;
			Start = start;
			Text = text;
		}

		public ASTNode(ParseMatchType matchType, string text)
		: this(matchType, 0, text)
		{
			return;
		}

		public ASTNode(ParseMatchType matchType, ILexResult lexResult)
		: this(matchType, lexResult.Start, lexResult.Text)
		{
			return;
		}

		public ASTNode(ILexResult lexResult)
		: this(ParseMatchType.None, lexResult)
		{
			return;
		}
		#endregion

		#region METHODS
		#region STATIC
		#endregion

		#region ILIST<IASTNODE> SUPPORT
		public void Add(ASTNode item)
		{
			_children.Add(item);
			sort();
		}

		public void Clear()
		{
			_children.Clear();
		}

		public bool Contains(ASTNode item)
		{
			return _children.Contains(item);
		}

		public void CopyTo(ASTNode[] array, int arrayIndex)
		{
			_children.CopyTo(array, arrayIndex);
		}

		public IEnumerator<ASTNode> GetEnumerator()
		{
			return _children.GetEnumerator();
		}

		public bool Remove(ASTNode item)
		{
			bool status = _children.Remove(item);
			sort();
			return status;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _children.GetEnumerator();
		}
		#endregion
		
		private void sort()
		{
			// use existing compare functions to simplify sorting
			_children.Sort(
				(ast1, ast2) =>  ast1.Start.CompareTo(ast2.Start)
			);
		}

		public object Clone()
		{
			List<ASTNode> children = new List<ASTNode>();
			foreach (ASTNode node in _children) children.Add(node.Clone() as ASTNode);

			return new ASTNode() {
				_children = children,
				Text = Text.Clone() as string,
				Start = Start,
				MatchType = MatchType
			};
		}

		public ASTNode Offset(int offset)
		{
			ASTNode offsetClone = Clone() as ASTNode;

			offsetClone.Start += offset;
			for (int i = 0; i < offsetClone._children.Count; i++)
				offsetClone._children[i] = offsetClone._children[i].Offset(offset);

			return offsetClone;
		}
		#endregion
	}

}