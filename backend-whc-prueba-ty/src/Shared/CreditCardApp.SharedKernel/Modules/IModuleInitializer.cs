namespace CreditCardApp.SharedKernel.Modules;

public interface IModuleInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
