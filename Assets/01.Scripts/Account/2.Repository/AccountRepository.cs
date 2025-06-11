using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine;

public class AccountRepository
{
    public const string SAVE_PREFIX = "Account_";

    public AccountSaveData Find(string email)
    {
        string json = PlayerPrefs.GetString(SAVE_PREFIX + email, null);

        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<AccountSaveData>(json);
    }

    public void Save(AccountDTO accountDTO)
    {
        AccountSaveData data = new AccountSaveData(accountDTO);
        string json = JsonUtility.ToJson(data, true);

        PlayerPrefs.SetString(SAVE_PREFIX + accountDTO.Email, json);
        PlayerPrefs.Save();
    }


    public void Delete(string email)
    {
        PlayerPrefs.DeleteKey(SAVE_PREFIX + email);
        PlayerPrefs.Save();
    }

    public List<AccountSaveData> Load()
    {
        List<AccountSaveData> accounts = new List<AccountSaveData>();
        foreach (var key in GetAllKeys())
        {
            if (key.StartsWith(SAVE_PREFIX))
            {
                string json = PlayerPrefs.GetString(key);
                AccountSaveData account = JsonUtility.FromJson<AccountSaveData>(json);
                accounts.Add(account);
            }
        }
        return accounts;
    }

    private IEnumerable<string> GetAllKeys()
    {
        List<string> keys = new List<string>();
        foreach (var key in PlayerPrefs.GetString("PlayerPrefsKeys", "").Split(','))
        {
            if (!string.IsNullOrEmpty(key))
                keys.Add(key);
        }
        return keys;
    }
}

public class AccountSaveData
{
    public string Email;
    public string Nickname;
    public string Password;

    public AccountSaveData(string email, string nickname, string password)
    {
        Email = email;
        Nickname = nickname;
        Password = password;
    }

    public AccountSaveData(AccountDTO accountDTO)
    {
        Email = accountDTO.Email;
        Nickname = accountDTO.Nickname;
        Password = accountDTO.Password;
    }
}
