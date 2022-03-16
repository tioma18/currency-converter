using CurrencyConverter.Dto;

namespace CurrencyConverter;

public interface ICurrencyConverter
{
    ConvertResult Convert(string input);
}