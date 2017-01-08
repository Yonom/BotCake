using System;
using BotBits;
using BotBits.Commands;

namespace BotCake
{
    public static class CakeServices
    {
        [ThreadStatic]
        private static BotBitsClient _client;

        internal static BotBitsClient Client => _client;

        public static void WithClient(BotBitsClient client, Action callback)
        {
            var oldClient = client;
            _client = client;
            callback();
            _client = oldClient;
        }

        public static void Run(Func<BotBitsClient, BotBase> callback)
        {
            BotServices.Run(bot =>
                WithClient(bot, () =>
                {
                    callback(bot).MainBot = true;
                    new CakeStartedEvent().RaiseIn(bot);
                }));
        }

        public static void Exit()
        {
            if (Client == null) throw new InvalidOperationException("CakeServices.Run is not running on this thread.");
            Client.Dispose();
        }
    }
}