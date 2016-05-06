using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotBits;
using BotBits.Commands;

namespace BotCake
{
    public class BotBase : IDisposable
    {
        private readonly BotBitsClient _botBits;
        private int _commandsLoaded;

        internal bool MainBot { get; set; }
        public ConnectionManager ConnectionManager => ConnectionManager.Of(this);
        public Login Login => Login.Of(this);
        public EventLoader EventLoader => EventLoader.Of(this);
        public Blocks Blocks => Blocks.Of(this);
        public Players Players => Players.Of(this);
        public Room Room => Room.Of(this);
        public Actions Actions => Actions.Of(this);
        public BlockChecker BlockChecker => BlockChecker.Of(this);
        public Chat Chat => Chat.Of(this);
        public Scheduler Scheduler => Scheduler.Of(this);

        public CommandManager CommandManager => CommandManager.Of(this);
        public CommandLoader CommandLoader => CommandLoader.Of(this);

        public BotBase()
        {
            this._botBits = CakeServices.Client;
            if (this._botBits == null)
                throw new InvalidOperationException(
                    "Please call CakeServices.WithClient before creating new BotBase objects.");

            this.EventLoader.Load(this);
            this.LoadCommands(); // Try to load commands as soon as possible
        }

        [EventListener]
        private void OnInitComplete(CakeStartedEvent e)
        {
            this.LoadCommands();
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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.EventLoader.Unload(this);
                if (this._commandsLoaded == 1)
                    this.CommandLoader.Unload(this);

                if (this.MainBot)
                {
                    this._botBits.Dispose();
                }
            }
        }
    }
}
