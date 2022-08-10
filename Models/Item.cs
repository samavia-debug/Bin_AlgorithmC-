using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinCompletionAlgorithm.Models
{
    public class Item : IEquatable<Item>
    {
        public int Value { get; set; }
        public string Label { get; set; }

        public override int GetHashCode()
        {
            return Value;
        }
        public bool Equals(Item? other)
        {
            if (ReferenceEquals(null, other)) return false;

            return this.Value == other.Value && Label == other.Label;
        }
        public override string ToString()
        {
            return $"{Value} - ({Label})";
        }
    }
}
