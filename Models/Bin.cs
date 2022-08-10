using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompletionAlgorithm.Models
{
    public class Bin
    {
        public string Label { get; set; }
        public int Capacity { get; set; }
        public HashSet<Item> Items { get; private set; } = new();
        public int Rest { get { return Capacity - CurrentSum; } }
        public int CurrentSum { get; private set; }

        // Constructor To Check For Input Errors ?
        public Bin() { }
        public Bin(string label, int capacity)
        {
            Label = label;
            Capacity = capacity;
        }

        public Bin(int capacity)
        {
            Capacity = capacity;
            Label = capacity + "ItemBin";
        }

        public bool CanFit(Item item)
        {
            return (CurrentSum + item.Value) <= Capacity;
        }

        public bool AddItem(Item item)
        {
            if (CanFit(item))
            {
                Items.Add(item);
                CurrentSum += item.Value;
                return true;
            }
            return false;
        }

        public bool RemoveItem(Item item)
        {
            if (Items.Contains(item))
            {
                Items.Remove(item);
                CurrentSum -= item.Value;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            string res = $"{Label}  ({Capacity} Units)";
            if (Items.Any())
            {
                res += " : [ ";
                res += String.Join(',', Items.Select(i => { return $"{i.Label}({i.Value})"; }));
                res += $" ] - Rest = {Rest}";
            }
            return res;
        }
    }
}
