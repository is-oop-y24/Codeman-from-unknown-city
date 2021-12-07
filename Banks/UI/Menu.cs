using System;
using System.Collections.Generic;

namespace Banks.UI
{
    public class Menu
    {
        private string _header;

        public Menu(string header, List<Item> items = null)
        {
            Header = header;
            Items = items ?? new List<Item>();
        }

        public string Header
        {
            get => _header;
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Menu header mustn't be empty.");
                _header = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public List<Item> Items { get; }

        public void Render(bool withBackOption = false)
        {
            if (Items.Count == 0)
                throw new MenuException("Menu should contains items");

            if (withBackOption)
                Items.Add(new Item("Back", () => Console.WriteLine("\n")));

            Console.WriteLine(Header);
            int index;
            do
            {
                int counter = 0;
                Items.ForEach(item => Console.WriteLine($"{++counter}. {item.Name}"));
                index = Parse(Console.ReadLine(), Items.Count, out string err);
                if (index == -1)
                    Console.Error.WriteLine($"{err}\n");
            }
            while (index == -1);

            Items[index].Handler();
        }

        private static int Parse(string input, int nMenuItems, out string err)
        {
            err = null;
            try
            {
                int index = Convert.ToInt32(input) - 1;
                if (index > -1 && index < nMenuItems)
                    return index;
                err = "Unknown option.";
                return -1;
            }
            catch (Exception)
            {
                err = "Invalid menu item number.";
                return -1;
            }
        }

        public readonly struct Item
        {
            public readonly string Name;
            public readonly Action Handler;

            public Item(string name, Action handler)
            {
                if (name == string.Empty)
                    throw new ArgumentException("Menu item name mustn't be empty");
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }
        }
    }
}