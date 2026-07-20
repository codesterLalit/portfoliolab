using PortfolioLab.Domain;
using NetArchTest.Rules;

namespace PortfolioLab.Application.Tests.Architecture;

public class DomainPurityTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Impure_Namespace()
    {
        var result = Types.InAssembly(typeof(Portfolio).Assembly)
        .Should()
        .NotHaveDependencyOnAny(
            "System.IO",
            "System.Net",
            "System.Console",
            "System.Threading"
        )
        .GetResult();

        Assert.True(result.IsSuccessful, "Domain types must be pure. Offending types: " + string.Join(", ", result.FailingTypeNames ?? []));
    }
}