using BotBits;
using BotBits.ChatExtras;
using BotBits.Commands;
using BotBits.Events;
using BotBits.Permissions;

namespace BotCake.Demo
{
    class MyBot : BotBase
    {
        public MyBot()
        {
            CommandsExtension.LoadInto(this, '!');
            ChatFormatsExtension.LoadInto(this, new BasicChatSyntaxProvider("Bot"));
            PermissionsExtension.LoadInto(this, Group.Moderator, 
                new SimplePermissionProvider("processor"));

            CommandManager.CreateConsoleCommandReaderThread().Start();
            ConnectionManager
                .GuestLogin()
                .CreateJoinRoom("PW01");
        }

        [EventListener]
        void OnJoin(JoinEvent e)
        {
            Chat.Say("Hiiiiiii {0}", e.Player.Username);
        }

        [Command(0, "hi")]
        void HiCommand(IInvokeSource source, ParsedRequest request)
        {
            Group.Moderator.RequireFor(source);

            source.Reply("Hello");
        }
    }
}