using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ForiDots.UI
{
    public class TimerHolder : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerValue;

        public void UpdateTimer(float time)
        {
            _timerValue.text = Mathf.RoundToInt(time).ToString();
        }
    }
}


