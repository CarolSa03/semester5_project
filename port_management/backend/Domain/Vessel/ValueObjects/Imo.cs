using System.Text.RegularExpressions;

namespace PortManagement.Domain.Vessel.ValueObjects
{
    // DDD Value Object representing a valid IMO number (7 digits with check digit)
    public readonly record struct Imo
    {
        public int Value { get; init; }

        public Imo(int value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString("D7");

        public static bool TryCreate(int number, out Imo imo)
        {
            var s = number.ToString();
            if (IsValid(s))
            {
                imo = new Imo(number);
                return true;
            }
            imo = default;
            return false;
        }

        public static bool TryCreate(string text, out Imo imo)
        {
            if (!string.IsNullOrWhiteSpace(text) && IsValid(text) && int.TryParse(text, out var n))
            {
                imo = new Imo(n);
                return true;
            }
            imo = default;
            return false;
        }

        public static bool IsValid(string imo)
        {
            if (!Regex.IsMatch(imo, @"^\d{7}$"))
                return false;

            int checkDigit = imo[^1] - '0';
            int sum = 0;
            for (int i = 0; i < 6; i++)
            {
                int digit = imo[i] - '0';
                int weight = 7 - i;
                sum += digit * weight;
            }
            return (sum % 10) == checkDigit;
        }
    }
}
