using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryPointCalculator.Controllers
{
	public class SignalRHub : Hub
	{
		public void Send(string name, string message)
		{
			Clients.All.addNewMessageToPage(name, message);
		}
	}
}