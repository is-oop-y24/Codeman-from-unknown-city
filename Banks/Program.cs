using Banks.Service;
using Banks.UI;

namespace Banks
{
    internal static class Program
    {
        private static int Main()
        {
            IBanksService service = new BanksService(30);
            Ui.GreetUser();
            var ui = new Ui(service);
            IMainLoop mainLoop;
            do
            {
                mainLoop = ui.SelectMainLoop();
                mainLoop?.Run();
            }
            while (mainLoop != null);

            return 0;
        }
    }
}
