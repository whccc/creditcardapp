using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Payments.Domain;

public static class PaymentErrors
{
    public const string DefaultDeclineReason = "El pago fue rechazado por el emisor.";

    public static readonly Error InvalidAmount =
        Error.Validation("Payment.InvalidAmount", $"El monto debe ser mayor a 0, no superar {PaymentAmount.MaxValue:N0} y tener como máximo 2 decimales.");

    public static readonly Error InvalidMerchant =
        Error.Validation("Payment.InvalidMerchant", $"El comercio debe tener entre {Merchant.MinLength} y {Merchant.MaxLength} caracteres.");

    public static readonly Error InvalidPagination =
        Error.Validation("Payment.InvalidPagination", "La página debe ser mayor a 0 y el tamaño de página estar entre 1 y 50.");
}
