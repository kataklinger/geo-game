using System;

namespace GG.Model.Contracts.Game
{
	public class GameGenerationException : Exception
	{
		public GameGenerationException() { }
		public GameGenerationException(string message) : base(message) { }
		public GameGenerationException(string message, Exception inner) : base(message, inner) { }
	}
}
