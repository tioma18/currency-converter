using CurrencyConverter.Converters;

namespace CurrencyConverter;

public class ConvertersFactory
{
    public ICurrencyConverter Build(string locale = "")
    {
        // we can add any other converter support
        return new UsCurrencyConverter();
    }
}