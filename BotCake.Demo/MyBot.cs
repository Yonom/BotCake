using BotBits;
using BotBits.Commands;
using BotBits.Events;
using BotBits.Permissions;

namespace BotCake.Demo
{
    class MyBot : BotBase
    {
        [EventListener]
        void On(JoinEvent e)
        {
            Chat.Say("Hi {0}", e.Player.Username);
        }

        [RestrictedCommand(Group.Moderator, 0, "hi")]
        void HiCommand(IInvokeSource source, ParsedRequest request)
        {
            source.Reply("Hello");
        }
    }
}