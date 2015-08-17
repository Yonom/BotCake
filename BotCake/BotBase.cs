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
    public class BotBase
    {
        private readonly BotBitsClient _botBits;
        private bool _commandsLoaded;

        public ConnectionManager ConnectionManager { get { return ConnectionManager.Of(this); } }
        public EventLoader EventLoader { get { return EventLoader.Of(this); } }
        public Blocks Blocks { get { return Blocks.Of(this); } }
        public Players Players { get { return Players.Of(this); } }
        public Room Room { get { return Room.Of(this); } }
        public Actions Actions { get { return Actions.Of(this); } }
        public BlockChecker BlockChecker { get { return BlockChecker.Of(this); } }
        public Chat Chat { get { return Chat.Of(this); } }

        public CommandManager CommandManager { get { return CommandManager.Of(this); } }
        public CommandLoader CommandLoader { get { return CommandLoader.Of(this); } }

        public BotBase()
        {
            this._botBits = CakeServices.Client;
            if (this._botBits == null)
                throw new InvalidOperationException(
                    "Please call CakeServices.WithClient before creating new BotBase objects.");

            this.LoadCommands();
            this.EventLoader.Load(this);
        }

        [EventListener]
        private void OnInitComplete(InitCompleteEvent e)
        {
            this.LoadCommands();
        }

        internal void LoadCommands()
        {
            if (this._commandsLoaded || !CommandsExtension.IsLoadedInto(this._botBits)) return;
            this._commandsLoaded = true;
            this.CommandLoader.Load(this);
        }

        public static implicit operator BotBitsClient(BotBase bot)
        {
            return bot._botBits;
        }
    }
}
