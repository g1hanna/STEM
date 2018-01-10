using System;
using StemInterpretter.Lexing;

namespace StemInterpretter.Parsing
{

	public class DetailedParseResult : IParseResult
	{
		#region FIELDS AND AUTOS
		public ParseStatus Status { get; set; }
		public ASTNode Node { get; set; }
		public string Message { get; set; }
		#endregion

		#region PROPERTIES
		public int Start {
			get {
				if (Node != null) return Node.Start;
				else return 0;
			}
			set {
				if (Node != null) Node.Start = value;
			}
		}

		public int Length {
			get {
				if (Node != null) return Node.Length;
				else return 0;
			}
		}
		#endregion

		#region CONSTRUCTORS
		private DetailedParseResult()
		{
			return;
		}

		public DetailedParseResult(ParseStatus status, ASTNode node, string message)
		{
			Status = status;
			Node = node;
			Message = message;
		}

		public DetailedParseResult(ParseStatus status, string message)
		: this(status, ASTNode.Empty, message)
		{
			return;
		}

		public DetailedParseResult(IParseResult result, string message)
		: this(result.Status, result.Node, message)
		{
			return;
		}
		#endregion

		#region METHODS
		#region STATIC
		public static DetailedParseResult CreateError(ASTNode node, string message)
		{
			return new DetailedParseResult(ParseStatus.Error, node, message);
		}

		public static DetailedParseResult CreateWarning(ASTNode node, string message)
		{
			return new DetailedParseResult(ParseStatus.Warning, node, message);
		}

		public static DetailedParseResult CreateSuccess(ASTNode node, string message)
		{
			return new DetailedParseResult(ParseStatus.Success, node, message);
		}
		#endregion

		public ParseResult ToParseResult()
		{
			return new ParseResult(Status, Node);
		}

		public object Clone()
		{
			return new DetailedParseResult() {
				Status = Status,
				Node = Node.Clone() as ASTNode,
				Message = Message.Clone() as string
			};
		}

		public IParseResult Offset(int offset)
		{
			DetailedParseResult offsetClone = Clone() as DetailedParseResult;

			offsetClone.Start += offset;
			offsetClone.Node = offsetClone.Node.Offset(offset);

			return offsetClone;
		}
		#endregion
	}

}