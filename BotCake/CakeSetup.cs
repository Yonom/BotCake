using System;
using System.Threading;
using BotBits;
using BotBits.Commands;
using PlayerIOClient;

namespace BotCake
{
    public class CakeSetup : ILogin<LoginClient>, IPlayerIOGame<LoginClient>, IDisposable
    {
        private BotBitsClient _client;

        private CakeSetup(Func<BotBitsClient, BotBase> callback, bool isBackground)
        {
            // ReSharper disable once AccessToDisposedClosure
            using (var resetEvent = new ManualResetEvent(false))
            {
                new Thread(() =>
                {
                    CakeServices.Run(bot =>
                    {
                        this._client = bot;
                        resetEvent.Set();
                        return callback(bot);
                    });
                }) { IsBackground = isBackground, Name = "BotCake.Thread"}.Start();
                resetEvent.WaitOne();
            }
        }

        public static CakeSetup WithBot(Func<BotBitsClient, BotBase> callback, bool isBackground = false)
        {
            return new CakeSetup(callback, isBackground);
        }

        public CakeSetup WithCommandsExtension(params char[] commandPrefixes)
        {
            CommandsExtension.LoadInto(this._client, commandPrefixes);
            return this;
        }

        public CakeSetup WithCommandsExtension(ListeningBehavior listeningBehavior, params char[] commandPrefixes)
        {
            CommandsExtension.LoadInto(this._client, listeningBehavior, commandPrefixes);
            return this;
        }

        public CakeSetup ListenToConsole()
        {
            CommandManager.Of(this._client).CreateConsoleCommandReaderThread().Start();
            return this;
        }

        public CakeSetup Do(Action<BotBitsClient> callback)
        {
            callback(this._client);
            return this;
        }

        public LoginClient WithClient(Client client)
        {
            return Login.Of(this._client).WithClient(client);
        }

        public void Dispose()
        {
            this._client.Dispose();
        }

        public string GameId => Login.Of(this._client).GameId;
        ILogin<LoginClient> IPlayerIOGame<LoginClient>.Login => this;
    }
}
