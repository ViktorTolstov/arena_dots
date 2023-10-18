using TMPro;

using UnityEngine;

namespace ArenaGames
{
    public class AGAuthUIController : MonoBehaviour
    {
        public UIElement_Login m_LoginPanel;
        public UIElement_SignUp m_SignupPanel;
        public UIElement_ConfirmEmail m_ConfirmEmailPanel;
        public UIElement_ConfirmSuccess m_ConfirmSuccessPanel;

        public GameObject m_ErrorBox;
        public TextMeshProUGUI m_TxtError;

        private void Start()
        {
            if (!AGCore.IsInitialized)
            {
                Destroy(gameObject);
                return;
            }

            m_LoginPanel.Open();
        }

        public void OnErrorMessage(string errorTxt)
        {
            if (IsInvoking(nameof(OnErrorMessageExpired)))
                CancelInvoke(nameof(OnErrorMessageExpired));

            if (string.IsNullOrEmpty(errorTxt))
            {
                OnErrorMessageExpired();
                return;
            }

            m_ErrorBox.SetActive(true);
            m_TxtError.text = errorTxt;

            Invoke(nameof(OnErrorMessageExpired), 3f);
        }

        private void OnErrorMessageExpired()
        {
            m_ErrorBox.SetActive(false);
        }
    }
}