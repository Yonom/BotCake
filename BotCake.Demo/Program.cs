namespace BotCake.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            CakeServices.Run(bot => new MyBot());
        }
    }
}
