namespace Domain.Entities;

public sealed class Seller : Entity
{
    public string Login { get; private set; }
    public string Password { get; private set; }
    public string Name { get; private set; }

    public Seller(string login, string password, string name)
    {
        Login = login;
        Password = password;
        Name = name;
    }

    public void Update(string login, string password, string name)
    {
        Login = login;
        Password = password;
        Name = name;
    }
}