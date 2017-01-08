using System;
using BotBits.Commands;

namespace BotCake
{
    internal class CommandsExtensionServices
    {
        public static bool IsAvailable()
        {
            try
            {
                AttemptUse();
            }
            catch
            {
                return false;
            }

            return true;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static Type AttemptUse()
        {
            return typeof(CommandsExtension);
        }
    }
}
