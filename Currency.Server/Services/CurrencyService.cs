using CurrencyConverter;
using Grpc.Core;

namespace Currency.Server.Services;

public class CurrencyService : Currency.CurrencyBase
{
    private readonly ILogger<CurrencyService> _logger;
    private readonly ConvertersFactory _convertersFactory;

    public CurrencyService(ILogger<CurrencyService> logger, ConvertersFactory convertersFactory)
    {
        _logger = logger;
        _convertersFactory = convertersFactory;
    }

    public override Task<ConvertReply> Convert(ConvertRequest request, ServerCallContext context)
    {
        _logger.LogDebug("Receive ConvertRequest with input: {Input}", request.NumberInput);

        var converter = _convertersFactory.Build();
        var (isSuccessful, conversionOutput) = converter.Convert(request.NumberInput);

        var result = new ConvertReply
        {
            IsSuccess = isSuccessful,
            ConvertedOutput = conversionOutput
        };

        _logger.LogDebug("Send ConvertReply with result: {Result}", result.ConvertedOutput);

        return Task.FromResult(result);
    }
}