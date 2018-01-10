using System;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{

	public struct ParseResult : IParseResult
	{
		#region FIELDS AND AUTOS
		#region STATIC
		public static readonly ParseResult Empty = new ParseResult()
		{
			Status = ParseStatus.None,
			Node = ASTNode.Empty
		};
		#endregion

		public ParseStatus Status { get; set; }
		public ASTNode Node { get; set; }
		#endregion

		#region PROPERTIES
		public int Start { get => Node.Start; set => Node.Start = value; }
		public int Length => Node.Length;
		#endregion

		#region CONSTRUCTORS
		public ParseResult(ParseStatus status, ASTNode node)
		{
			Status = status;
			Node = node;
		}

		public ParseResult(ParseStatus status)
		: this(status, ASTNode.Empty)
		{
			return;
		}
		#endregion

		#region METHODS
		#region STATIC
		#endregion

		public object Clone()
		{
			return new ParseResult()
			{
				Status = Status,
				Node = Node.Clone() as ASTNode
			};
		}

		public IParseResult Offset(int offset)
		{
			ParseResult offsetCopy = (ParseResult)Clone();

			offsetCopy.Node = offsetCopy.Node.Offset(offset);

			return offsetCopy;
		}
		#endregion
	}

}