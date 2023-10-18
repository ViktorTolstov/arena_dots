using UnityEngine;
using UnityEngine.Events;

namespace ArenaGames
{
    public class UIElementBase : MonoBehaviour
    {
        public AGAuthUIController m_AGAuthUIController;
        public AGInGameUIController m_InGameUIController;

        public void Open(UIElementBase previousUI = null, UnityAction action = null)
        {
            if (previousUI != null)
                previousUI.Close();

            gameObject.SetActive(true);
            action?.Invoke();
        }

        public void Close(UnityAction action = null)
        {
            gameObject.SetActive(false);

            action?.Invoke();
        }
    }
}