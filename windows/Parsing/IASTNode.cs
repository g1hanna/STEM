// using System;
// using System.Collections.Generic;
// using StemInterpretter.Lexing;

// namespace StemInterpretter.Parsing
// {
// 	public interface ASTNode : ICollection<ASTNode>, ICloneable
// 	{
// 		string Text { get; }
// 		ParseMatchType MatchType { get; set; }
// 		int Start { get; set; }
// 		int Length { get; }

// 		ASTNode Offset(int offset);
// 	}
// }