namespace PaymentGateway.Api.Extensions;

// I would put this in a shared lib between many services - no need for that, since we just have one :D
public static class StringExtensions
{
    public static bool IsNumeric(this string s) =>  s.All(char.IsNumber);
}