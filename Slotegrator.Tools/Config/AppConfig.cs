namespace Slotegrator.Tools.Config;

public class AppConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public UserCredentials UserCredentials { get; set; } = new();
}

public class UserCredentials
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
