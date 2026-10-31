namespace CreditCardApp.Modules.Cards.Domain;

public static class CardDeletionPolicy
{
    public static readonly TimeSpan RecentPaymentWindow = TimeSpan.FromDays(30);

    public static DateTimeOffset RecentPaymentsSince(DateTimeOffset now) => now - RecentPaymentWindow;
}
