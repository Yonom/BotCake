using System;
using System.Threading;
using BotBits;
using BotBits.Commands;
using PlayerIOClient;

namespace BotCake
{
    public sealed class CakeSetup : ILogin<LoginClient>, IPlayerIOGame<LoginClient>, IDisposable
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
                        var res = callback(bot);
                        resetEvent.Set();
                        return res;
                    });
                }) { IsBackground = isBackground, Name = "BotCake.Thread" }.Start();
                resetEvent.WaitOne();
            }
        }

        public void Dispose()
        {
            this._client.Dispose();
        }

        public LoginClient WithClient(Client client)
        {
            return Login.Of(this._client).WithClient(client);
        }

        public string GameId => Login.Of(this._client).GameId;
        ILogin<LoginClient> IPlayerIOGame<LoginClient>.Login => this;

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
            if (!CommandsExtensionServices.IsAvailable() || !this.ListenToConsoleInternal())
                throw new Exception("You must first load CommandsExtension!");
            return this;
        }

        public bool ListenToConsoleInternal()
        {
            if (!CommandsExtension.IsLoadedInto(this._client)) return false;
            CommandManager.Of(this._client).CreateConsoleCommandReaderThread().Start();
            return true;
        }

        public CakeSetup Do(Action<BotBitsClient> callback)
        {
            callback(this._client);
            return this;
        }

        public PlayerIOGame WithGameId(string gameId)
        {
            return new PlayerIOGame(this, gameId);
        }

        public NonFutureProofLogin WithoutFutureProof()
        {
            return Login.Of(this._client).WithoutFutureProof();
        }
    }
}