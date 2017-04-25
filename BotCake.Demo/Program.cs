using System.Threading;
using BotBits;
using BotBits.ChatExtras;
using BotBits.Permissions;

namespace BotCake.Demo
{
    class Program
    {
        static void Main()
        {
            CakeSetup
                .WithBot(bot => new MyBot())
                .WithCommandsExtension('!')
                .ListenToConsole()
                .Do(bot => ChatFormatsExtension.LoadInto(bot, new BasicChatSyntaxProvider("Bot")))
                .Do(bot => PermissionsExtension.WithCommandsLoadInto(bot, Group.Moderator, new SimplePermissionProvider("processor")))
                .AsGuest()
                .CreateJoinRoomAsync("PW01");
        }
    }
}
