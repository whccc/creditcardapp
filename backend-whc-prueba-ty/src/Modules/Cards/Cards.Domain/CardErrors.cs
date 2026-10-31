using CreditCardApp.SharedKernel.Results;

namespace CreditCardApp.Modules.Cards.Domain;

public static class CardErrors
{
    public static readonly Error InvalidNumber =
        Error.Validation("Card.InvalidNumber", "El número de tarjeta no es válido.");

    public static readonly Error UnsupportedBrand =
        Error.Validation("Card.UnsupportedBrand", "Solo se aceptan tarjetas Visa, Mastercard o American Express.");

    public static readonly Error InvalidExpiry =
        Error.Validation("Card.InvalidExpiry", "La fecha de vencimiento no es válida.");

    public static readonly Error ExpiryInThePast =
        Error.Validation("Card.ExpiryInThePast", "La fecha de vencimiento debe ser posterior al mes actual.");

    public static readonly Error InvalidHolderName =
        Error.Validation("Card.InvalidHolderName", $"El nombre del titular debe tener entre {Card.HolderNameMinLength} y {Card.HolderNameMaxLength} caracteres.");

    public static readonly Error InvalidNickname =
        Error.Validation("Card.InvalidNickname", $"El alias no puede superar los {Card.NicknameMaxLength} caracteres.");

    public static readonly Error InvalidCreditLimit =
        Error.Validation("Card.InvalidCreditLimit", $"El cupo debe ser mayor a 0, menor o igual a {Card.MaxCreditLimit:N0} y tener como máximo 2 decimales.");

    public static readonly Error InvalidChargeAmount =
        Error.Validation("Card.InvalidChargeAmount", "El monto debe ser mayor a 0 y tener como máximo 2 decimales.");

    public static readonly Error CreditLimitBelowUsage =
        Error.BusinessRule("Card.CreditLimitBelowUsage", "El nuevo cupo no puede ser menor al monto ya utilizado.");

    public static readonly Error Blocked =
        Error.BusinessRule("Card.Blocked", "La tarjeta se encuentra bloqueada.");

    public static readonly Error Expired =
        Error.BusinessRule("Card.Expired", "La tarjeta se encuentra vencida.");

    public static readonly Error InsufficientCredit =
        Error.BusinessRule("Card.InsufficientCredit", "Cupo disponible insuficiente.");

    public static readonly Error NotFound =
        Error.NotFound("Card.NotFound", "La tarjeta no existe.");

    public static readonly Error ConcurrentUpdate =
        Error.Conflict("Card.ConcurrentUpdate", "La tarjeta fue modificada por otra operación. Intenta nuevamente.");
}
