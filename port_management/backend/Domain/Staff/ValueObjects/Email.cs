using System;
using System.Text.RegularExpressions;

namespace PortManagement.Domain.Staff.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; }

        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );
        
        private Email() { }
        
        // Construtor público com validação
        public Email(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Email address cannot be empty.", nameof(address));

            var normalized = address.Trim().ToLowerInvariant();

            if (!EmailRegex.IsMatch(normalized))
                throw new ArgumentException("Invalid email format.", nameof(address));

            Value = normalized;
        }

        public override string ToString() => Value;
    }
}