namespace InnomateApp.Domain.ValueObjects
{
    /// <summary>
    /// Value Object for Money - prevents decimal misuse and ensures currency safety
    /// </summary>
    public record Money
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; }

        public Money(decimal amount, string currency = "USD")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            Amount = amount;
            Currency = currency ?? "USD";
        }

        public static Money Zero => new(0);

        // Arithmetic operators
        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException($"Cannot add different currencies: {a.Currency} and {b.Currency}");
            
            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException($"Cannot subtract different currencies: {a.Currency} and {b.Currency}");
            
            return new Money(a.Amount - b.Amount, a.Currency);
        }

        public static Money operator *(Money money, decimal multiplier)
        {
            return new Money(money.Amount * multiplier, money.Currency);
        }

        public static Money operator /(Money money, decimal divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException("Cannot divide money by zero");
            
            return new Money(money.Amount / divisor, money.Currency);
        }

        // Comparison operators
        public static bool operator >(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException($"Cannot compare different currencies: {a.Currency} and {b.Currency}");
            
            return a.Amount > b.Amount;
        }

        public static bool operator <(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException($"Cannot compare different currencies: {a.Currency} and {b.Currency}");
            
            return a.Amount < b.Amount;
        }

        public static bool operator >=(Money a, Money b) => !(a < b);
        public static bool operator <=(Money a, Money b) => !(a > b);

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}
