using System;

namespace GG.ModelView.Messages
{
	public class WebNavigateNotification
	{
		public WebNavigateNotification(Uri location)
		{
			Location = location;
		}

		public Uri Location { get; private set; }
	}
}
