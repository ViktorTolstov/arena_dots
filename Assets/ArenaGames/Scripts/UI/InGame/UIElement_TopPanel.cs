using TMPro;
using UnityEngine;

namespace ArenaGames
{
    public class UIElement_TopPanel : UIElementBase
    {
        [SerializeField] private TextMeshProUGUI _nickName;
        [SerializeField] private TextMeshProUGUI _amtTokenCount;
        
        // TODO: add callback to update info
        public async void UpdateData()
        {
            await ArenaGamesController.Instance.NetworkController.RefreshUserCurrency();
            _nickName.text = ArenaGamesController.Instance.User.PlayerInfo.username;
            _amtTokenCount.text = ArenaGamesController.Instance.User.CurrencyInfo.CurrencyInfo[0].balanceAmount.ToString();
        }
    }
}