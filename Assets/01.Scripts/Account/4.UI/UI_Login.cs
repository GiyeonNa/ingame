using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Security.Cryptography;
using System.Text;
using DG.Tweening;
using UnityEngine.SceneManagement;

[Serializable]
public class InputField
{
    public TMP_InputField IDInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField PasswordCheckInput;
}

public class UI_Login : MonoBehaviour
{
    [SerializeField]
    private GameObject loginPanel;
    [SerializeField]
    private GameObject registerPanel;

    [Header("Login Fields")]
    [SerializeField]
    private Button registerButton;
    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private InputField loginInputField;
    [SerializeField]
    private TextMeshProUGUI loginResultText;

    [Header("Register Fields")]
    [SerializeField]
    private InputField registerInputField;
    [SerializeField]
    private Button signUpButton;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private TextMeshProUGUI registerResultText;

    private void Awake()
    {
        registerButton.onClick.AddListener(OnClickRegister);
        loginButton.onClick.AddListener(OnClickLogin);
        signUpButton.onClick.AddListener(OnClickSignUp);
        backButton.onClick.AddListener(OnClickBackToLogin);

        loginInputField.IDInput.onValueChanged.AddListener(delegate { LoginCheck(); });
        loginInputField.PasswordInput.onValueChanged.AddListener(delegate { LoginCheck(); });
    }

    private void Start()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        LoginCheck();
    }

    //회원가입 창으로 이동
    private void OnClickRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);

        registerResultText.text = string.Empty;
        registerResultText.gameObject.SetActive(false);

        registerInputField.IDInput.text = string.Empty;
        registerInputField.PasswordInput.text = string.Empty;
        registerInputField.PasswordCheckInput.text = string.Empty;
    }

    //로그인 창으로 이동
    private void OnClickBackToLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);

        loginResultText.text = string.Empty;
        loginResultText.gameObject.SetActive(false);

        loginInputField.IDInput.text = string.Empty;
        loginInputField.PasswordInput.text = string.Empty;
    }

    //로그인 시도
    private void OnClickLogin()
    {
        string email = loginInputField.IDInput.text; // 이메일은 ID 입력 필드에서 가져옴
        //var emailSpecifi = new AccountemilSpecification();



        string id = loginInputField.IDInput.text;
        string password = loginInputField.PasswordInput.text;

        //string email = loginInputField.IDInput.text; // 이메일은 ID 입력 필드에서 가져옴
        if (string.IsNullOrEmpty(id))
        {
            loginResultText.text = "아이디를 입력하세요.";
            loginResultText.gameObject.SetActive(true);
            ShakeText(loginResultText);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            loginResultText.text = "비밀번호를 입력하세요.";
            loginResultText.gameObject.SetActive(true);
            ShakeText(loginResultText);
            return;
        }

        // 입력된 비밀번호를 해싱  
        string hashedInputPassword = HashPassword(password);

        // 저장된 해시값과 비교  
        if (id == PlayerPrefs.GetString("Username")
            && hashedInputPassword == PlayerPrefs.GetString("Password"))
        {
            loginResultText.text = "로그인 성공!";
            loginResultText.gameObject.SetActive(true);
            SceneManager.LoadScene(1);
        }
        else
        {
            loginResultText.text = "아이디 또는 비밀번호가 틀립니다.";
            loginResultText.gameObject.SetActive(true);
            ShakeText(loginResultText);
        }

        if (AccountManager.Instance.TryLogin(id, password))
        {
            // 로그인 성공  
        }
    }

    //회원가입 시도
    private void OnClickSignUp()
    {
        string id = registerInputField.IDInput.text; 
        string password = registerInputField.PasswordInput.text; 




        if (string.IsNullOrEmpty(id))
        {
            registerResultText.text = "아이디를 입력하세요.";
            registerResultText.gameObject.SetActive(true);
            ShakeText(registerResultText);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            registerResultText.text = "비밀번호를 입력하세요.";
            registerResultText.gameObject.SetActive(true);
            ShakeText(registerResultText);
            return;
        }

        if (password != registerInputField.PasswordCheckInput.text)
        {
            registerResultText.text = "비밀번호가 일치하지 않습니다.";
            registerResultText.gameObject.SetActive(true);
            ShakeText(registerResultText);
            return;
        }

        string nickname = "TestNickname"; // 닉네임은 임시로 설정, 필요시 수정  
        
        string nicknname = registerInputField.IDInput.text; // 닉네임은 ID 입력 필드에서 가져옴

        if (AccountManager.Instance.TryRegister(id, nickname, password))
        {
            // 로그인 창으로 돌아간다.  
            OnClickBackToLogin();
        }
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // 16진수 문자열로 변환
            }
            return builder.ToString();
        }
    }


    private void ShakeText(TextMeshProUGUI text)
    {
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.DOShakeAnchorPos(0.5f, new Vector2(10f, 0f), 10, 90, false, true);
        }
    }


    private void LoginCheck()
    {
        string id = loginInputField.IDInput.text;
        string password = loginInputField.PasswordInput.text;

        loginButton.interactable = !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password);
    }
}
