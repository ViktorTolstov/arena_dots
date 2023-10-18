using ArenaGames;

using TMPro;

using UnityEngine.UI;

public class UIElement_SignUp : UIElementBase
{
    public TMP_InputField m_FieldEmail;
    public TMP_InputField m_FieldUsername;
    public TMP_InputField m_FieldPassword;
    public TMP_InputField m_FieldConfirmPassword;

    public Button m_BtnRegister;
    public Button m_BtnLogin;

    private bool m_IsWaitingForResponse = false;

    private RegistrationData m_RegData;

    private void OnEnable()
    {
        m_FieldEmail.onValueChanged.AddListener((value) => { OnInputFieldChanged(); });
        m_FieldUsername.onValueChanged.AddListener((value) => { OnInputFieldChanged(); });
        m_FieldPassword.onValueChanged.AddListener((value) => { OnInputFieldChanged(); });
        m_FieldConfirmPassword.onValueChanged.AddListener((value) => { OnInputFieldChanged(); });
        m_FieldConfirmPassword.onDeselect.AddListener((value) => { OnConfirmDeselect(); });
        m_BtnLogin.onClick.AddListener(OnLoginBtnClicked);
        m_BtnRegister.onClick.AddListener(OnRegisterBtnClicked);

    }

    private void OnDisable()
    {
        m_FieldUsername.onValueChanged.RemoveAllListeners();
        m_FieldPassword.onValueChanged.RemoveAllListeners();
        m_BtnLogin.onClick.RemoveListener(OnLoginBtnClicked);
        m_BtnRegister.onClick.RemoveListener(OnRegisterBtnClicked);
    }

    private void OnLoginBtnClicked()
    {
        m_AGAuthUIController.m_LoginPanel.Open(this);
    }

    private void OnRegisterBtnClicked()
    {
        m_RegData = new RegistrationData();
        m_RegData.email = m_FieldEmail.text;
        m_RegData.password = m_FieldPassword.text;
        m_RegData.username = m_FieldUsername.text;

        ArenaGamesController.Instance.NetworkControllerOld.TryRegisterUser(m_RegData, OnSuccessfulLogin, OnError);

        m_BtnLogin.interactable = false;

        m_IsWaitingForResponse = true;
    }

    private void OnConfirmDeselect()
    {
        if (!string.IsNullOrEmpty(m_FieldPassword.text) && !string.IsNullOrEmpty(m_FieldConfirmPassword.text) && m_FieldPassword.text != m_FieldConfirmPassword.text)
        {
            m_BtnRegister.interactable = false;
            OnError("Passwords don't match");
            return;
        }
        
        OnError("");
    }

    private void OnInputFieldChanged()
    {
        if (m_IsWaitingForResponse)
            return;

        m_BtnRegister.interactable = !string.IsNullOrEmpty(m_FieldEmail.text) && !string.IsNullOrEmpty(m_FieldUsername.text);
    }

    private void OnSuccessfulLogin()
    {
        m_AGAuthUIController.m_ConfirmEmailPanel.Open(this, null, m_RegData);
        m_IsWaitingForResponse = false;
    }

    private void OnError(string error)
    {
        m_AGAuthUIController.OnErrorMessage(error);
        m_IsWaitingForResponse = false;
        
        m_BtnLogin.interactable = true;
    }
}
