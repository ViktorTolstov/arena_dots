using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class AchievementsEntry : MonoBehaviour
{
    public TextMeshProUGUI m_TxtTitle;
    public TextMeshProUGUI m_TxtReward;
    public TextMeshProUGUI m_TxtMissionNum;

    public Image m_ImgStatus;
    public Sprite[] m_AchievementStatusIcons;

    public void Setup(string _Title, string _Reward, bool _Completed)
    {
        if (m_TxtTitle != null)
            m_TxtTitle.text = _Title;

        if (m_TxtReward != null)
            m_TxtReward.text = _Reward;

        if (m_TxtMissionNum != null)
        {
            if (!_Completed)
            {
                m_ImgStatus.sprite = m_AchievementStatusIcons[0];
                m_TxtMissionNum.text = "In Progress";
            }
            else
            {
                m_ImgStatus.sprite = m_AchievementStatusIcons[1];
                m_TxtMissionNum.text = "Completed";
            }
        }
    }
}