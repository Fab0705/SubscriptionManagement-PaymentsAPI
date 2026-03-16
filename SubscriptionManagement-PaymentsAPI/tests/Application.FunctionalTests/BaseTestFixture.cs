
using static SubscriptionManagement_PaymentsAPI.Application.FunctionalTests.Testing;

namespace SubscriptionManagement_PaymentsAPI.Application.FunctionalTests;

[TestFixture]
public abstract class BaseTestFixture
{
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
    }
}
