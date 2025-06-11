using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CryptoUtil : MonoBehaviour
{
    public static string Encryption(string text, string salt)
    {
        // �ؽ� ��ȣȭ �˰��� �ν��Ͻ��� �����Ѵ�.
        SHA256 sha256 = SHA256.Create();

        // �ü�� Ȥ�� ���α׷��� ���� string ǥ���ϴ� ����� �� �ٸ��Ƿ�
        // UTF8 ���� ����Ʈ�� �迭�� �ٲ���Ѵ�.
        byte[] bytes = Encoding.UTF8.GetBytes(text + salt);
        byte[] hash = sha256.ComputeHash(bytes);

        string resultText = string.Empty;
        foreach (byte b in hash)
        {
            // byte�� �ٽ� string���� �ٲ㼭 �̾���̱�
            resultText += b.ToString("X2");
        }

        return resultText;
    }

    public static bool Verify(string text, string hash, string salt = null)
    {
        // �Էµ� �ؽ�Ʈ�� �ؽ�ȭ�� ����� ����� �ؽð��� ���Ѵ�.
        return Encryption(text, salt) == hash;
    }
}
