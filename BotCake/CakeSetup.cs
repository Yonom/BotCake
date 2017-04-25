using System;
using System.Threading;
using System.Threading.Tasks;
using BotBits;
using BotBits.Commands;
using PlayerIOClient;

namespace BotCake
{
    public sealed class CakeSetup : ILogin<LoginClient>, IPlayerIOGame<LoginClient>
    {
        private readonly Func<BotBitsClient, BotBase> _factory;
        private Action<BotBitsClient> _actions;
        private int _runs;
        private BotBitsClient _client;
        
        private CakeSetup(Func<BotBitsClient, BotBase> factory)
        {
            this._factory = factory;
        }

        public static CakeSetup WithBot(Func<BotBitsClient, BotBase> callback)
        {
            return new CakeSetup(callback);
        }

        public CakeSetup WithSendTimerFrequency(double frequency)
        {
            this._actions += bot => MessageSender.Of(bot).SendTimerFrequency = frequency;
            return this;
        }

        public CakeSetup WithCommandsExtension(params char[] commandPrefixes)
        {
            this._actions += bot => CommandsExtension.LoadInto(bot, commandPrefixes);
            return this;
        }

        public CakeSetup WithCommandsExtension(ListeningBehavior listeningBehavior, params char[] commandPrefixes)
        {
            this._actions += bot => CommandsExtension.LoadInto(bot, listeningBehavior, commandPrefixes);
            return this;
        }

        public CakeSetup ListenToConsole()
        {
            this._actions += bot =>
            {
                if (!CommandsExtensionServices.IsAvailable() || !this.ListenToConsoleInternal(bot))
                    throw new InvalidOperationException("You must first load CommandsExtension before calling ListenToConsole!");
            };
            return this;
        }

        public bool ListenToConsoleInternal(BotBitsClient client)
        {
            if (!CommandsExtension.IsLoadedInto(client)) return false;
            CommandManager.Of(client).CreateConsoleCommandReaderThread().Start();
            return true;
        }

        public CakeSetup Do(Action<BotBitsClient> callback)
        {
            this._actions += callback;
            return this;
        }

        private Login TryRun()
        {
            if (this._runs == 2)
                return Login.Of(this._client);
            else if (this._runs == 1)
                throw new InvalidOperationException("Cannot access this property while Run is in progress.");
            return this.Run(false);
        }

        public Login Run(bool background)
        {
            if (Interlocked.CompareExchange(ref this._runs, 1, 0) == 1)
                throw new InvalidOperationException("Run has already been called on this CakeSetup.");

            var tcs = new TaskCompletionSource<BotBitsClient>();
            new Thread(() =>
            {
                try
                {
                    CakeServices.Run(bot =>
                    {
                        var res = this._factory(bot);
                        this._actions(bot);

                        this._client = bot;
                        this._runs = 2;

                        tcs.TrySetResult(bot);
                        return res;
                    });
                }
                catch (Exception ex)
                {
                    this._runs = 0;
                    tcs.TrySetException(ex);
                }
            }) { IsBackground = background, Name = "BotCake.Thread" }.Start();

            try
            {

                return Login.Of(tcs.Task.Result);
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        public LoginClient WithClient(Client client)
        {
            return this.TryRun().WithClient(client);
        }

        public NonFutureProofLogin WithoutFutureProof()
        {
            return this.TryRun().WithoutFutureProof();
        }

        string IPlayerIOGame<LoginClient>.GameId => this.TryRun().GameId;

        public PlayerIOGame WithGameId(string gameId)
        {
            return new PlayerIOGame(this, gameId);
        }
        ILogin<LoginClient> IPlayerIOGame<LoginClient>.Login => this;
    }
}