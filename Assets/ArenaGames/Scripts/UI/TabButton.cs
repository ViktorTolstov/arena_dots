using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaGames
{
    public class TabButton : Button
    {
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private Image _buttonImage;

        private const string PressedColor = "#FF5385";
        private const string DefaultColor = "#88AAE5";

        private Color _pressedColor;
        private Color _defaultColor;

        private void TryParseColors()
        {
            ColorUtility.TryParseHtmlString(PressedColor, out _pressedColor);
            ColorUtility.TryParseHtmlString(DefaultColor, out _defaultColor);
        }
        
        public void UpdatePress(bool isPressed)
        {
            if (_pressedColor == default || _defaultColor == default)
            {
                TryParseColors();
            }
            
            _buttonText.color = isPressed ? _pressedColor : _defaultColor;
            _buttonImage.color = isPressed ? _pressedColor : _defaultColor;
        }
    }
}