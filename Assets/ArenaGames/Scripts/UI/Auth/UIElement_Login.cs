using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class UIElement_Login : UIElementBase
    {
        public TMP_InputField m_FieldUsername;
        public TMP_InputField m_FieldPassword;

        public Button m_BtnLogin;
        public Button m_BtnRegister;

        public Toggle m_ShowPassword;
        public Toggle m_RememberMe;

        public GameObject m_ImgPasswordNotVisible;
        public GameObject m_ImgPasswordVisible;

        private bool m_IsWaitingForResponse = false;

        private void Start()
        {
            OnInputFieldChanged();

            m_FieldUsername.text = PlayerPrefs.GetString("Username", "");
            m_FieldPassword.text = PlayerPrefs.GetString("Password", "");
        }

        private void OnEnable()
        {
            m_FieldUsername.onValueChanged.AddListener((value) => { OnInputFieldChanged(); });
            m_FieldPassword.onValueChanged.AddListener((value) => { OnInputFieldChanged(); });
            m_BtnLogin.onClick.AddListener(OnLoginBtnClicked);
            m_BtnRegister.onClick.AddListener(OnRegisterBtnClicked);
            m_ShowPassword.onValueChanged.AddListener((value) => OnShowPassword(value));
        }

        private void OnDisable()
        {
            m_FieldUsername.onValueChanged.RemoveListener((value) => { OnInputFieldChanged(); });
            m_FieldPassword.onValueChanged.RemoveListener((value) => { OnInputFieldChanged(); });
            m_BtnLogin.onClick.RemoveListener(OnLoginBtnClicked);
            m_BtnRegister.onClick.RemoveListener(OnRegisterBtnClicked);
            m_ShowPassword.onValueChanged.RemoveListener((value) => OnShowPassword(value));
        }

        private void OnLoginBtnClicked()
        {
            ArenaGamesController.Instance.User.SignInUser(new LoginData() { username = m_FieldUsername.text, password = m_FieldPassword.text }, OnSuccessfulLogin, OnError);
            
            m_BtnLogin.interactable = false;
            m_IsWaitingForResponse = true;
        }

        private void OnRegisterBtnClicked()
        {
            if (m_IsWaitingForResponse) return;

            m_AGAuthUIController.m_SignupPanel.Open(this);
        }

        private void OnInputFieldChanged()
        {
            if (m_IsWaitingForResponse) return;

            m_BtnLogin.interactable = m_FieldUsername.text.Length > 0 && m_FieldPassword.text.Length > 0;
        }

        private void OnShowPassword(bool showPass)
        {
            m_ImgPasswordNotVisible.SetActive(!showPass);
            m_ImgPasswordVisible.SetActive(showPass);

            m_FieldPassword.contentType = showPass ? 
                TMP_InputField.ContentType.Standard : 
                TMP_InputField.ContentType.Password;

            m_FieldPassword.ForceLabelUpdate();
        }

        private void OnSuccessfulLogin()
        {
            if (m_RememberMe.isOn)
            {
                PlayerPrefs.SetString("Username", m_FieldUsername.text);
                PlayerPrefs.SetString("Password", m_FieldPassword.text);
            }
            else
            {
                PlayerPrefs.SetString("Username", "");
                PlayerPrefs.SetString("Password", "");
            }

            Close();

            m_IsWaitingForResponse = false;

            Destroy(m_AGAuthUIController.gameObject);
        }

        private void OnError(string error)
        {
            m_AGAuthUIController.OnErrorMessage(error);
            m_IsWaitingForResponse = false;
        }
    }
}