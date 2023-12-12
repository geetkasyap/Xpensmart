using CoreModels.Entities;
using DataAccessLayer.Infrastructure;

namespace Testing.UsersTests;

[NonParallelizable]
public class PublicationKeyTests
{
    private string? _previous;

    [SetUp]
    public void RememberEnvironment()
    {
        _previous = Environment.GetEnvironmentVariable("XPENSMART_ENCRYPTION_KEY");
    }

    [TearDown]
    public void RestoreEnvironment()
    {
        Environment.SetEnvironmentVariable("XPENSMART_ENCRYPTION_KEY", _previous);
    }

    [Test]
    public void MissingKeyIsAnExplicitConfigurationError()
    {
        Environment.SetEnvironmentVariable("XPENSMART_ENCRYPTION_KEY", null);
        using var context = new ApplicationDbContext();
        var repository = new UsersRepository(context);
        Assert.Throws<InvalidOperationException>(() => repository.Encryption("fixture-only"));
    }

    [Test]
    public void ConfiguredKeyRoundTripsDemoData()
    {
        Environment.SetEnvironmentVariable("XPENSMART_ENCRYPTION_KEY", "fixture-only-key");
        using var context = new ApplicationDbContext();
        var repository = new UsersRepository(context);
        var encoded = repository.Encryption("fixture-only");
        Assert.That(encoded, Is.Not.EqualTo("fixture-only"));
        Assert.That(repository.Decryption(encoded), Is.EqualTo("fixture-only"));
    }

    [Test]
    public void WhitespaceKeyIsRejected()
    {
        Environment.SetEnvironmentVariable("XPENSMART_ENCRYPTION_KEY", "   ");
        using var context = new ApplicationDbContext();
        var repository = new UsersRepository(context);
        Assert.Throws<InvalidOperationException>(() => repository.Encryption("fixture-only"));
    }
}
