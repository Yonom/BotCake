using System;
using BotBits;
using BotBits.Commands;

namespace BotCake
{
    public static class CakeServices
    {
        [ThreadStatic]
        private static BotBitsClient _client;
        [ThreadStatic]
        private static bool _starting;

        internal static BotBitsClient Client => _client;
        internal static bool Starting => _starting;

        public static void WithClient(BotBitsClient client, Action callback)
        {
            var oldClient = _client;
            _client = client;
            try
            {
                callback();
            }
            finally
            {
                _client = oldClient;
            }
        }

        public static void WithStarting(Action callback)
        {
            var oldStarting = _starting;
            _starting = true;
            try
            {
                callback();
            }
            finally
            {
                _starting = oldStarting;
            }
        }

        public static void Run(Func<BotBitsClient, BotBase> callback)
        {
            Exception exception = null;

            var oldClient = _client;
            BotServices.Run(bot => WithStarting(() =>
            {
                _client = bot;
                try
                {
                    callback(bot).MainBot = true;
                    new CakeStartedEvent().RaiseIn(bot);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    bot.Dispose();
                }
            }));
            _client = oldClient;

            if (exception != null) throw exception;
        }

        public static void Exit()
        {
            if (Client == null) throw new InvalidOperationException("CakeServices.Run is not running on this thread.");
            Client.Dispose();
        }
    }
}