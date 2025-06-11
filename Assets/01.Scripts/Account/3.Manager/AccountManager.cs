using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance { get; private set; }

    private const string SALT = "123456";

    private AccountRepository _accountRepository;
    private Account _myAccount;

    public AccountDTO CurrentAccoutn => _myAccount?.ToDTO();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Init();
    }

    private void Init()
    {
        _accountRepository = new AccountRepository();
    }


    public bool TryLogin(string email, string password)
    {
        // 저장소에서 계정 데이터 조회
        var saveData = _accountRepository.Find(email);
        if (saveData == null)
            return false;

        // 입력 비밀번호 암호화
        //string encryptedInputPassword = Encryption(password + SALT);

        //// 저장된 암호화 비밀번호와 비교
        //if (saveData.Password == encryptedInputPassword)
        //    return true;

        if(CryptoUtil.Verify(password, saveData.Password)) 
            return false;

        if(CryptoUtil.Encryption(password, SALT) != saveData.Password)
            return false;

        return false;
    }

    public bool TryRegister(string email, string nickname, string password)
    {
        // 이미 존재하는 이메일인지 확인
        if (_accountRepository.Find(email) != null)
            return false;

        string encryptedpassword = CryptoUtil.Encryption(password, SALT);
        Account account = new Account(email, nickname, encryptedpassword);
        _accountRepository.Save(account.ToDTO());

        return true;
    }

    
}
