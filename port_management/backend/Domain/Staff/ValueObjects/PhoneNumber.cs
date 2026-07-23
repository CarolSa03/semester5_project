using System;
using System.Text.RegularExpressions;

namespace PortManagement.Domain.Staff.ValueObjects
{
    public sealed record PhoneNumber
    {
        public string Value { get; }

        // Expressão regular simples: permite +, dígitos, espaços e hífens
        private static readonly Regex PhoneRegex = new(
            @"^\+?[0-9\s\-]{7,15}$",
            RegexOptions.Compiled
        );

        // Construtor sem parâmetros (necessário para EF Core)
        private PhoneNumber() { }

        // Construtor público com validação
        public PhoneNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Phone number cannot be empty.", nameof(number));

            var normalized = number.Trim();

            if (!PhoneRegex.IsMatch(normalized))
                throw new ArgumentException("Invalid phone number format.", nameof(number));

            Value = normalized;
        }

        public override string ToString() => Value;
    }
}

