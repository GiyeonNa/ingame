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

    //ȸ������ â���� �̵�
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

    //�α��� â���� �̵�
    private void OnClickBackToLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);

        loginResultText.text = string.Empty;
        loginResultText.gameObject.SetActive(false);

        loginInputField.IDInput.text = string.Empty;
        loginInputField.PasswordInput.text = string.Empty;
    }

    //�α��� �õ�
    private void OnClickLogin()
    {
        string email = loginInputField.IDInput.text; // �̸����� ID �Է� �ʵ忡�� ������
        //var emailSpecifi = new AccountemilSpecification();



        string id = loginInputField.IDInput.text;
        string password = loginInputField.PasswordInput.text;

        //string email = loginInputField.IDInput.text; // �̸����� ID �Է� �ʵ忡�� ������
        if (string.IsNullOrEmpty(id))
        {
            loginResultText.text = "���̵� �Է��ϼ���.";
            loginResultText.gameObject.SetActive(true);
            ShakeText(loginResultText);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            loginResultText.text = "��й�ȣ�� �Է��ϼ���.";
            loginResultText.gameObject.SetActive(true);
            ShakeText(loginResultText);
            return;
        }

        // �Էµ� ��й�ȣ�� �ؽ�  
        string hashedInputPassword = HashPassword(password);

        // ����� �ؽð��� ��  
        if (id == PlayerPrefs.GetString("Username")
            && hashedInputPassword == PlayerPrefs.GetString("Password"))
        {
            loginResultText.text = "�α��� ����!";
            loginResultText.gameObject.SetActive(true);
            SceneManager.LoadScene(1);
        }
        else
        {
            loginResultText.text = "���̵� �Ǵ� ��й�ȣ�� Ʋ���ϴ�.";
            loginResultText.gameObject.SetActive(true);
            ShakeText(loginResultText);
        }

        if (AccountManager.Instance.TryLogin(id, password))
        {
            // �α��� ����  
        }
    }

    //ȸ������ �õ�
    private void OnClickSignUp()
    {
        string id = registerInputField.IDInput.text; 
        string password = registerInputField.PasswordInput.text; 




        if (string.IsNullOrEmpty(id))
        {
            registerResultText.text = "���̵� �Է��ϼ���.";
            registerResultText.gameObject.SetActive(true);
            ShakeText(registerResultText);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            registerResultText.text = "��й�ȣ�� �Է��ϼ���.";
            registerResultText.gameObject.SetActive(true);
            ShakeText(registerResultText);
            return;
        }

        if (password != registerInputField.PasswordCheckInput.text)
        {
            registerResultText.text = "��й�ȣ�� ��ġ���� �ʽ��ϴ�.";
            registerResultText.gameObject.SetActive(true);
            ShakeText(registerResultText);
            return;
        }

        string nickname = "TestNickname"; // �г����� �ӽ÷� ����, �ʿ�� ����  
        
        string nicknname = registerInputField.IDInput.text; // �г����� ID �Է� �ʵ忡�� ������

        if (AccountManager.Instance.TryRegister(id, nickname, password))
        {
            // �α��� â���� ���ư���.  
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
                builder.Append(b.ToString("x2")); // 16���� ���ڿ��� ��ȯ
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
