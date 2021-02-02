using System;
using System.Collections.Generic;
using System.Linq;
using Value;

namespace SeatsSuggestions.Domain
{
    public class ShowId : ValueType<ShowId>
    {
        public string Id { get; }

        public ShowId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"{nameof(id)} should be not empty");
            }

            if (id.Select(char.IsDigit).Count() != id.Length)
            {
                throw new ArgumentException($"{nameof(id)} should be a number");
            }

            Id = id;
        }

        public override string ToString()
        {
            return Id;
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] {Id};
        }
    }
}