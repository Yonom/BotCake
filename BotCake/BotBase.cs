using System;
using System.Threading;
using BotBits;
using BotBits.Commands;

//using BotBits.Commands;

namespace BotCake
{
    public abstract class BotBase : IDisposable
    {
        private readonly BotBitsClient _botBits;
        private int _commandsLoaded;

        protected BotBase()
        {
            this._botBits = CakeServices.Client;
            if (this._botBits == null)
                throw new InvalidOperationException(
                    "Please call CakeServices.WithClient before creating new BotBase objects.");

            if (CakeServices.Starting)
                CakeStartedEvent.Of(this._botBits).Bind(this.OnInitComplete);

            this.EventLoader.Load(this);

            if (CommandsExtensionServices.IsAvailable())
            {
                this.LoadCommands(); // Try to load commands as soon as possible
            }
        }

        internal bool MainBot { get; set; }
        public ConnectionManager ConnectionManager => ConnectionManager.Of(this);
        public Login Login => Login.Of(this);
        public EventLoader EventLoader => EventLoader.Of(this);
        public Blocks Blocks => Blocks.Of(this);
        public Players Players => Players.Of(this);
        public Room Room => Room.Of(this);
        public Actions Actions => Actions.Of(this);
        public BlockChecker BlockChecker => BlockChecker.Of(this);
        public MessageSender MessageSender => MessageSender.Of(this);
        public Chat Chat => Chat.Of(this);
        public Scheduler Scheduler => Scheduler.Of(this);

        public CommandManager CommandManager => CommandManager.Of(this);
        public CommandLoader CommandLoader => CommandLoader.Of(this);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void OnInitComplete(CakeStartedEvent e)
        {
            if (CommandsExtensionServices.IsAvailable())
            {
                this.LoadCommands();
            }
        }

        internal void LoadCommands()
        {
            if (!CommandsExtension.IsLoadedInto(this._botBits)) return;
            if (Interlocked.Exchange(ref this._commandsLoaded, 1) == 1) return;
            this.CommandLoader.Load(this);
        }

        public static implicit operator BotBitsClient(BotBase bot)
        {
            return bot._botBits;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.EventLoader.Unload(this);
                if (this._commandsLoaded == 1) this.DisposeCommands();

                if (this.MainBot)
                {
                    this._botBits.Dispose();
                }
            }
        }

        private void DisposeCommands()
        {
            this.CommandLoader.Unload(this);
        }
    }
}