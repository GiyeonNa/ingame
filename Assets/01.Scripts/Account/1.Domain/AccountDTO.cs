public class AccountDTO
{
    public string Email { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }

    public AccountDTO() { }

    public AccountDTO(string email, string nickname, string password)
    {
        Email = email;
        Nickname = nickname;
        Password = password;
    }

    // Account ������ ��ü�� DTO�� ��ȯ�ϴ� ������
    public AccountDTO(Account account)
    {
        Email = account.Email;
        Nickname = account.Nickname;
        Password = account.Password;
    }

    // DTO�� Account ������ ��ü�� ��ȯ�ϴ� �޼���
    public Account ToDomain()
    {
        return new Account(Email, Nickname, Password);
    }
}
