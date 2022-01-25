using Microsoft.AspNet.SignalR;
using StoryPointCalculator.Models;

namespace StoryPointCalculator.Controllers
{
	public class SignalRHub : Hub
	{
		public void Send(string name, string message)
		{
			Clients.All.addNewMessageToPage(name, Story.Current());
		}
	}
}