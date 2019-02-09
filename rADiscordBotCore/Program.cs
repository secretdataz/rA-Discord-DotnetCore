using System;
using System.Linq;
using System.Diagnostics;

namespace rADiscordBotCore
{
	public partial class rAthenaBot
	{
        public static void Main(String[] args) => new rAthenaBot().MainAsync().GetAwaiter().GetResult();
    }
}
