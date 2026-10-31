using System.Reflection;
using CreditCardApp.Modules.Authentication.Application.Ports.In;
using CreditCardApp.Modules.Authentication.Domain;
using CreditCardApp.Modules.Authentication.Infrastructure;
using CreditCardApp.Modules.Cards.Application.Ports.In;
using CreditCardApp.Modules.Cards.Domain;
using CreditCardApp.Modules.Cards.Infrastructure;
using CreditCardApp.Modules.Payments.Application.Ports.In;
using CreditCardApp.Modules.Payments.Domain;
using CreditCardApp.Modules.Payments.Infrastructure;
using NetArchTest.Rules;

namespace CreditCardApp.UnitTests.Architecture;

public sealed class ModuleBoundariesTests
{
    private const string ModulesNamespace = "CreditCardApp.Modules";

    private static readonly Dictionary<string, ModuleAssemblies> Modules = new()
    {
        ["Authentication"] = new(typeof(User).Assembly, typeof(ILoginUseCase).Assembly, typeof(AuthenticationModule).Assembly),
        ["Cards"] = new(typeof(Card).Assembly, typeof(ICreateCardUseCase).Assembly, typeof(CardsModule).Assembly),
        ["Payments"] = new(typeof(Payment).Assembly, typeof(IProcessPaymentUseCase).Assembly, typeof(PaymentsModule).Assembly)
    };

    public static TheoryData<string> ModuleNames => new(Modules.Keys);

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Domain_DependsOnNothingButTheSharedKernel(string module)
    {
        var result = Types.InAssembly(Modules[module].Domain)
            .ShouldNot().HaveDependencyOnAny(
                $"{ModulesNamespace}.{module}.Application",
                $"{ModulesNamespace}.{module}.Infrastructure",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Microsoft.Extensions")
            .GetResult();

        AssertSuccessful(result);
    }

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Application_DoesNotDependOnInfrastructureOrPersistence(string module)
    {
        var result = Types.InAssembly(Modules[module].Application)
            .ShouldNot().HaveDependencyOnAny(
                $"{ModulesNamespace}.{module}.Infrastructure",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore")
            .GetResult();

        AssertSuccessful(result);
    }

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Modules_DoNotReferenceEachOther(string module)
    {
        var otherModules = Modules.Keys
            .Where(name => name != module)
            .Select(name => $"{ModulesNamespace}.{name}")
            .ToArray();

        foreach (var assembly in Modules[module].All)
        {
            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(otherModules).GetResult();
            AssertSuccessful(result);
        }
    }

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void UseCaseImplementations_AreHiddenBehindTheirPorts(string module)
    {
        var result = Types.InAssembly(Modules[module].Application)
            .That().ResideInNamespace($"{ModulesNamespace}.{module}.Application.UseCases")
            .Should().NotBePublic()
            .GetResult();

        AssertSuccessful(result);
    }

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Infrastructure_OnlyExposesModuleRegistrationAndOptions(string module)
    {
        var exposed = Modules[module].Infrastructure.GetExportedTypes()
            .Where(type => !type.Name.EndsWith("Module", StringComparison.Ordinal)
                && !type.Name.EndsWith("Options", StringComparison.Ordinal))
            .Select(type => type.FullName)
            .ToList();

        Assert.True(exposed.Count == 0, $"Public infrastructure types: {string.Join(", ", exposed)}");
    }

    private static void AssertSuccessful(TestResult result) =>
        Assert.True(result.IsSuccessful, $"Violating types: {string.Join(", ", result.FailingTypeNames ?? [])}");

    private sealed record ModuleAssemblies(Assembly Domain, Assembly Application, Assembly Infrastructure)
    {
        public IEnumerable<Assembly> All => [Domain, Application, Infrastructure];
    }
}
