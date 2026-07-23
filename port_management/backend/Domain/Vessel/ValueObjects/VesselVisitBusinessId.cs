using System.Text.RegularExpressions;
using PortManagement.Domain.Common.Exceptions;

namespace PortManagement.Domain.Vessel.ValueObjects
{
    public record VesselVisitBusinessId
    {
        public int Year { get; }
        public string PortCode { get; }
        public int SequentialNumber { get; }
        public string Value { get; }

        private VesselVisitBusinessId(int year, string portCode, int sequentialNumber)
        {
            Year = year;
            PortCode = portCode;
            SequentialNumber = sequentialNumber;
            Value = $"{year}-{portCode}-{sequentialNumber:D6}";
        }

        public static VesselVisitBusinessId Create(int year, string portCode, int sequentialNumber)
        {
            if (year < 2000 || year > 2100)
                throw new DomainValidationException("Year must be between 2000 and 2100");

            if (!IsValidPortCode(portCode))
                throw new DomainValidationException("Port code must be 2-10 uppercase alphanumeric characters");

            if (sequentialNumber < 1)
                throw new DomainValidationException("Sequential number must be positive");

            return new VesselVisitBusinessId(year, portCode, sequentialNumber);
        }

        public static VesselVisitBusinessId Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainValidationException("Business ID cannot be empty");

            var match = Regex.Match(value, @"^(\d{4})-([A-Z0-9]{2,10})-(\d{6})$");
            if (!match.Success)
                throw new DomainValidationException("Invalid business ID format. Expected: YYYY-PORTCODE-000001");

            var year = int.Parse(match.Groups[1].Value);
            var portCode = match.Groups[2].Value;
            var sequentialNumber = int.Parse(match.Groups[3].Value);

            return Create(year, portCode, sequentialNumber);
        }

        public static bool TryParse(string value, out VesselVisitBusinessId result)
        {
            result = null;

            try
            {
                result = Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidPortCode(string portCode)
        {
            return !string.IsNullOrWhiteSpace(portCode) &&
                   portCode.Length >= 2 &&
                   portCode.Length <= 10 &&
                   portCode.All(c => char.IsUpper(c) || char.IsDigit(c));
        }

        public bool IsFromSamePortAndYear(VesselVisitBusinessId other)
        {
            return Year == other.Year && PortCode == other.PortCode;
        }

        public bool IsSequentialTo(VesselVisitBusinessId other)
        {
            return IsFromSamePortAndYear(other) && SequentialNumber == other.SequentialNumber + 1;
        }

        public override string ToString() => Value;

        // Implicit conversion to string for convenience
        public static implicit operator string(VesselVisitBusinessId id) => id.Value;
    }
}
