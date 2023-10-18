using UnityEngine.UI;

namespace ArenaGames
{
    public class UIElement_ConfirmSuccess : UIElementBase
    {
        public Button Btn_Signin;

        private void OnEnable()
        {
            Btn_Signin.onClick.AddListener(OnLoginBtnClicked);
        }

        private void OnDisable()
        {
            Btn_Signin.onClick.RemoveListener(OnLoginBtnClicked);
        }

        private void OnLoginBtnClicked()
        {
            m_AGAuthUIController.m_LoginPanel.Open(this);
        }
    }
}