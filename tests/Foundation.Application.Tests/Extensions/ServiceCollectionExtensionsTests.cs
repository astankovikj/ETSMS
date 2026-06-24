using Foundation.Application.Extensions;
using Foundation.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace Foundation.Application.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_RegistersIEmployeeServiceAsScoped()
    {
        var services = new ServiceCollection();

        services.AddApplicationServices();

        var descriptor = services.SingleOrDefault(sd => sd.ServiceType == typeof(IEmployeeService));
        descriptor.Should().NotBeNull("IEmployeeService must be registered");
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
        descriptor.ImplementationType.Should().Be(typeof(EmployeeService));
    }

    [Fact]
    public void AddApplicationServices_ReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();

        var returned = services.AddApplicationServices();

        returned.Should().BeSameAs(services);
    }
}
