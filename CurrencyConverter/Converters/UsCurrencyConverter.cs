using System.Text;
using System.Text.RegularExpressions;
using CurrencyConverter.Dto;

namespace CurrencyConverter.Converters;

public class UsCurrencyConverter : ICurrencyConverter
{
    private static readonly IDictionary<int, string> WordNumberDictionary = new Dictionary<int, string>
    {
        { 0, "zero" },
        { 1, "one" },
        { 2, "two" },
        { 3, "three" },
        { 4, "four" },
        { 5, "five" },
        { 6, "six" },
        { 7, "seven" },
        { 8, "eight" },
        { 9, "nine" },
        { 10, "ten"},
        { 11, "eleven"},
        { 12, "twelve"},
        { 13, "thirteen"},
        { 14, "fourteen" },
        { 15, "fifteen"},
        { 16, "sixteen"},
        { 17, "seventeen"},
        { 18, "eighteen"},
        { 19, "nineteen"},
        { 20, "twenty"},
        { 30, "thirty"},
        { 40, "forty"},
        { 50, "fifty"},
        { 60, "sixty"},
        { 70, "seventy"},
        { 80, "eighty"},
        { 90, "ninety"},
    };

    public ConvertResult Convert(string input)
    {
        var inputWithoutSpaces = input.Replace(" ", string.Empty);
        if (!IsValid(inputWithoutSpaces))
        {
            return new ConvertResult(false, "Provided input is not valid.");
        }

        var currencyParts = inputWithoutSpaces.Split(',', StringSplitOptions.TrimEntries);
        var dollars = currencyParts.First();
        var wordResult = new StringBuilder();
        wordResult = ParseDollars(wordResult, dollars);

        if (currencyParts.Length == 2)
        {
            var cents = currencyParts.Last();
            wordResult = ParseCents(cents, wordResult);
        }

        return new ConvertResult(true, wordResult.ToString());
    }

    private static bool IsValid(string input)
    {
        const string maxValueString = "999999999,99";
        var currencyRegex = new Regex("^(\\d{1,3} ?){1,3}(,\\d{1,2})?|$");
        var match = currencyRegex.Match(input);
        return match.Success && match.Value.Length == input.Length && input.Length <= maxValueString.Length;
    }

    /// <summary>
    /// Parse dollar number part and add 'dollars' word
    /// </summary>
    /// <param name="wordResult"></param>
    /// <param name="dollars"></param>
    /// <returns></returns>
    private static StringBuilder ParseDollars(StringBuilder wordResult, string dollars)
    {
        wordResult = ParseNumber(int.Parse(dollars), wordResult);
        wordResult.Append("dollar");
        if (dollars.Length != 1 || dollars.First() != '1')
        {
            wordResult.Append('s');
        }

        return wordResult;
    }

    /// <summary>
    /// Parse cents number part and add 'cents' word
    /// </summary>
    /// <param name="cents"></param>
    /// <param name="wordResult"></param>
    /// <returns></returns>
    private static StringBuilder ParseCents(string cents, StringBuilder wordResult)
    {
        wordResult.Append(" and ");
        var centNumber = int.Parse(cents);
        if (cents.Length == 1)
        {
            centNumber *= 10;
        }

        wordResult = ParseNumber(centNumber, wordResult);

        wordResult.Append("cent");
        if (cents.Length == 1 || !cents.Equals("01"))
        {
            wordResult.Append('s');
        }
        return wordResult;
    }

    private static StringBuilder ParseNumber(int number, StringBuilder wordResult)
    {
        switch (number)
        {
            case <= 20:
                return wordResult.Append($"{WordNumberDictionary[number]} ");
            case < 100:
                return ParseTens(number, wordResult);
            case < 1000:
                return ParseHundreds(number, wordResult);
            case < 1000000:
                return ParseThousands(number, wordResult);
            case < 1000000000:
                return ParseMillions(number, wordResult);
        }

        throw new InvalidOperationException();
    }

    private static StringBuilder ParseMillions(int number, StringBuilder wordResult)
    {
        return ParseNumberPart(number, 1000000, "million ", wordResult);
    }

    private static StringBuilder ParseThousands(int number, StringBuilder wordResult)
    {
        return ParseNumberPart(number, 1000, "thousand ", wordResult);
    }

    private static StringBuilder ParseHundreds(int number, StringBuilder wordResult)
    {
        return ParseNumberPart(number, 100, "hundred ", wordResult);
    }

    private static StringBuilder ParseTens(int number, StringBuilder wordResult)
    {
        var tens = (int)Math.Floor(number / 10.0) * 10;
        var units = number - tens;
        return wordResult.Append($"{WordNumberDictionary[tens]}-{WordNumberDictionary[units]} ");
    }

    private static StringBuilder ParseNumberPart(int number, int nBase, string word, StringBuilder wordResult)
    {
        var root = (int)Math.Floor(number / (double)nBase);
        var rest = number - root * nBase;
        wordResult = ParseNumber(root, wordResult);
        wordResult.Append(word);
        if (rest != 0)
        {
            wordResult = ParseNumber(number - root * nBase, wordResult);
        }

        return wordResult;
    }
}