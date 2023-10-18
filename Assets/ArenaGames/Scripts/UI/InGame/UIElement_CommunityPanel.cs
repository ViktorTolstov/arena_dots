using ArenaGames;

using UnityEngine;
using UnityEngine.UI;

public class UIElement_CommunityPanel : UIElementBase
{
    public Button m_BtnClose;
    public Button m_BtnTwitter;
    public Button m_BtnDiscord;
    public Button m_BtnTelegram;
    public Button m_BtnMedium;
    public Button m_BtnReddit;

    private void OnEnable()
    {
        m_BtnTwitter.onClick.AddListener(() => { OnSocialBtnClicked("https://twitter.com/Arenaweb3"); });
        m_BtnDiscord.onClick.AddListener(() => { OnSocialBtnClicked("https://discord.gg/FxVyTPtF7f"); });
        m_BtnTelegram.onClick.AddListener(() => { OnSocialBtnClicked("https://t.me/Arenaweb3games"); });
        m_BtnMedium.onClick.AddListener(() => { OnSocialBtnClicked("https://medium.com/@Arena_Games_Platform"); });

        m_BtnClose.onClick.AddListener(OnCloseBtnClicked);
    }

    private void OnDisable()
    {
        m_BtnTwitter.onClick.RemoveAllListeners();
        m_BtnDiscord.onClick.RemoveAllListeners();
        m_BtnTelegram.onClick.RemoveAllListeners();
        m_BtnMedium.onClick.RemoveAllListeners();

        m_BtnClose.onClick.RemoveListener(OnCloseBtnClicked);
    }

    private void OnSocialBtnClicked(string _URL)
    {
        Application.OpenURL(_URL);
    }

    private void OnCloseBtnClicked()
    {
        Close();
    }
}
