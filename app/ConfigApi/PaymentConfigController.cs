using Microsoft.AspNetCore.Mvc;

namespace ConfigApi;

public class PaymentConfigController
{
    private static readonly List<PaymentConfig> PaymentConfigs =
    [
        new PaymentConfig
        {
            PaymentMethod = "CreditCard",
            MaxAmount = 1000
        },

        new PaymentConfig
        {
            PaymentMethod = "PayPal",
            MaxAmount = 500
        }
    ];
    
    [HttpPatch("api/v1/payment-config")]
    public List<PaymentConfig> UpdatePaymentConfigMaxAmount([FromBody] PaymentConfig config)
    {
        var paymentConfig = PaymentConfigs.FirstOrDefault(x => x.PaymentMethod == config.PaymentMethod);
        if (paymentConfig != null)
        {
            paymentConfig.MaxAmount = config.MaxAmount;
        }

        return PaymentConfigs;
    }
}