using System;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public TextMeshProUGUI m_TxtPosition;
    public TextMeshProUGUI m_TxtUsername;
    public TextMeshProUGUI m_TxtScore;
    public TextMeshProUGUI m_TxtReward;

    public Image m_CurrentSprite;
    public Sprite m_HighlightSprite;

    public Image m_TopRankIconObj;
    public Sprite[] m_TopRankSprites;

    public void SetupEntry(string _Pos, string _Username, string _Score, string _Reward, bool _IsMine = false)
    {
        if (m_TxtPosition != null)
            m_TxtPosition.text = _Pos;

        if (m_TxtUsername != null)
            m_TxtUsername.text = _Username;

        if (m_TxtScore != null)
            m_TxtScore.text = _Score;

        if (m_TxtReward != null)
            m_TxtReward.text = _Reward;

        if (int.Parse(_Pos) <= 3)
        {
            m_TopRankIconObj.gameObject.SetActive(true);
            m_TopRankIconObj.sprite = m_TopRankSprites[int.Parse(_Pos) - 1];
        }

        if (_IsMine)
        {
            if (m_CurrentSprite != null)
                m_CurrentSprite.sprite = m_HighlightSprite;
        }
    }

    public void SetupButton(Action onButtonClick)
    {
        
    }
}
