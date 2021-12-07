using System;
using System.Collections.Generic;
using Banks.Service;

namespace Banks.UI
{
    public class Ui
    {
        public const string UnknownOptionMsg = "Unknown option. Please try again.\n";

        private readonly ClientMainLoop _clientMainLoop;
        private readonly BankerMainLoop _bankerMainLoop;

        public Ui(IBanksService service)
        {
            _clientMainLoop = new ClientMainLoop(service);
            _bankerMainLoop = new BankerMainLoop(service);
        }

        public static void GreetUser() => Console.WriteLine("Welcome to the banks application!\n");

        public static string BuildMenu(string header, List<string> options)
        {
            uint count = 0;
            string menu = $"{header}:";
            options.ForEach(option => menu += $"\n{++count}. {option}");
            return menu;
        }

        public IMainLoop SelectMainLoop()
        {
            IMainLoop mainLoop = null;
            var menu = new Menu("Choose your type", new List<Menu.Item>
            {
                new Menu.Item("Client", () => mainLoop = _clientMainLoop),
                new Menu.Item("Banker", () => mainLoop = _bankerMainLoop),
                new Menu.Item("Exit", () => mainLoop = null),
            });
            menu.Render();
            return mainLoop;
        }
    }
}