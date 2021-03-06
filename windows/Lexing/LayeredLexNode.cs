using System;
using System.Collections;
using System.Collections.Generic;

namespace StemInterpretter.Lexing {

	public struct LayeredLexNode : ILexResult {
		#region FIELDS AND AUTOS
		private ILexResult _node;
		public LexMatchType MatchType { get; set; }
		#endregion
		
		#region PROPERTIES
		public LexMatchType InnerMatchType { get => ((ILexResult)_node).MatchType; set => ((ILexResult)_node).MatchType = value; }
		public string Text => ((ILexResult)_node).Text;
		public int Start { get => ((ILexResult)_node).Start; set => ((ILexResult)_node).Start = value; }
		public int Length => ((ILexResult)_node).Length;
		#endregion

		#region CONSTRUCTORS
		public LayeredLexNode(LexMatchType matchType, LexMatchType innerMatchType, int start, string text)
		: this(matchType, new LexNode(innerMatchType, start, text))
		{
			return;
		}

		public LayeredLexNode(LexMatchType matchType, ILexResult innerNode)
		{
			_node = innerNode;
			MatchType = matchType;
		}
		#endregion

		#region METHODS
		#region STATIC
		public static LayeredLexNode CreatePartial(ILexResult unfinished)
		{
			return new LayeredLexNode(LexMatchType.Partial, unfinished);
		}
		#endregion

		public object Clone() => new LayeredLexNode(MatchType, _node.Clone() as ILexResult);

		public ILexResult Offset(int offset)
		{
			LayeredLexNode copy = (LayeredLexNode)Clone();
			copy._node = copy._node.Offset(offset);

			return copy;
		}

		public ILexResult Move(int position)
		{
			return Offset(position - Start);
		}
		#endregion
	}

}