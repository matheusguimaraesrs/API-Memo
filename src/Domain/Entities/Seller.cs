using Domain.Exceptions;

namespace Domain.Entities;

public sealed class Seller : Entity
{
    public string Login { get; private set; }
    public string Password { get; private set; }
    public string Name { get; private set; }

    public Seller(string name, string login, string password)
    {
        Validation(name, login, password);

        Name = name;
        Login = login;
        Password = password;
    }

    public void Update(string name, string login, string password)
    {
        Validation(name, login, password);

        Name = name;
        Login = login;
        Password = password;
    }

    private void Validation(string name, string login, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório");
        if (string.IsNullOrWhiteSpace(login))
            throw new DomainException("Login é obrigatório");
        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Senha é obrigatória");
    }
}