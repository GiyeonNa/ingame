using System.Text.RegularExpressions;
using UnityEngine;

public class Account 
{
    public readonly string Email;
    public readonly string Nickname;
    public readonly string Password;

    private static readonly string[] ForbiddenWords = { "�ٺ�", "��û��" };


    public Account(string email, string nickname, string password)
    {
        //��Ģ�� ��ü�� ĸ��ȭ?�Ͽ� �и��ϱ�
        //DDD ��
        // �� ��ü ����
        var emailSpec = new AccountEmailSpecification();
        var nicknameSpec = new AccountNicknameSpecification();
        var passwordSpec = new AccountPasswordSpecification();

        // �̸��� ����
        if (!emailSpec.IsSatisfiedBy(email))
            throw new System.ArgumentException(emailSpec.GetErrorMessage(email), nameof(email));

        // �г��� ����
        if (!nicknameSpec.IsSatisfiedBy(nickname))
            throw new System.ArgumentException(nicknameSpec.GetErrorMessage(nickname), nameof(nickname));

        // ��й�ȣ ����
        if (!passwordSpec.IsSatisfiedBy(password))
            throw new System.ArgumentException(passwordSpec.GetErrorMessage(password), nameof(password));

        Email = email;
        Nickname = nickname;
        Password = password;
    }

    public AccountDTO ToDTO()
    {
        return new AccountDTO(Email, Nickname, Password);
    }
}
