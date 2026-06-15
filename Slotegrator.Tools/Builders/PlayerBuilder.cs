using Slotegrator.Tools.Constants;
using Slotegrator.Tools.Models;

namespace Slotegrator.Tools.Builders;

public class PlayerBuilder
{
    private const int UsernameLength = 16;

    private string _email = UniqueEmail();
    private string _username = UniqueUsername();
    private string _name = TestData.BaseName;
    private string _surname = TestData.BaseSurname;
    private string _currencyCode = TestData.DefaultCurrencyCode;
    private string _password = TestData.DefaultPassword;

    public PlayerBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public PlayerBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public PlayerBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public PlayerBuilder WithSurname(string surname)
    {
        _surname = surname;
        return this;
    }

    
    public PlayerRequestDto Build() => new()
    {
        CurrencyCode = _currencyCode,
        Email = _email,
        Name = _name,
        PasswordChange = _password,
        PasswordRepeat = _password,
        Surname = _surname,
        Username = _username
    };

    private static string UniqueEmail() => $"auto_{Guid.NewGuid():N}@{TestData.EmailDomain}";

    private static string UniqueUsername() => $"user_{Guid.NewGuid():N}"[..UsernameLength];
}
