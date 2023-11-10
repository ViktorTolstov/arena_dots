using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ArenaGames
{
    public class UIElement_ConfirmEmail : UIElementBase
    {
        [SerializeField] private TMP_InputField _codeField;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _resendCodeButton;
        [SerializeField] private TextMeshProUGUI _resendCodeText;

        private bool _isWaitingForResponse;
        private RegistrationData _regData;
        private float _resetCodeDelta;
        private int _prevTime;
        private bool _needToUpdate;
        
        public void Open(UIElementBase previousUI = null, UnityAction action = null, RegistrationData regData = null)
        {
            base.Open(previousUI, action);
            _regData = regData;
            UpdateResendData();
        }

        private async void UpdateResendData()
        {
            var resetTime = await ArenaGamesController.Instance.NetworkController.GetResetData(_regData.email);
            _resetCodeDelta = resetTime - AGTimeController.Timestamp;
            _resendCodeButton.interactable = false;
            
            _needToUpdate = _resetCodeDelta > 0;
            if (!_needToUpdate)
            {
                OnTimeFinish();
            }
        }

        private void OnEnable()
        {
            _codeField.onValueChanged.AddListener(_ => OnInputFieldChanged());
            _confirmButton.onClick.AddListener(OnLoginBtnClicked);
            _backButton.onClick.AddListener(OnBackBtnClicked);
            _resendCodeButton.onClick.AddListener(OnCodeResend);
        }

        private void OnDisable()
        {
            _codeField.onValueChanged.RemoveAllListeners();
            _confirmButton.onClick.RemoveAllListeners();
            _backButton.onClick.RemoveAllListeners();
        }

        private void OnLoginBtnClicked()
        {
            if (_isWaitingForResponse) return;

            OnError("");

            _regData.code = _codeField.text;
            ArenaGamesController.Instance.NetworkController.ConfirmEmail(_regData, OnSuccess, OnError);

            _confirmButton.interactable = false;

            _isWaitingForResponse = true;
        }

        private void OnBackBtnClicked()
        {
            if (_isWaitingForResponse)
                return;

            m_AGAuthUIController.m_SignupPanel.Open(this);
        }

        private void OnInputFieldChanged()
        {
            if (_isWaitingForResponse)
                return;

            _confirmButton.interactable = !string.IsNullOrEmpty(_codeField.text);
        }

        private void OnSuccess()
        {
            m_AGAuthUIController.m_ConfirmSuccessPanel.Open(this);

            _isWaitingForResponse = false;
        }

        private async void OnCodeResend()
        {
            _resendCodeButton.interactable = false;
            await ArenaGamesController.Instance.NetworkController.ResendCode(_regData.email);
            UpdateResendData();
        }

        private void OnError(string error)
        {
            _confirmButton.interactable = true;

            m_AGAuthUIController.OnErrorMessage(error);

            _isWaitingForResponse = false;
        }

        
        // TODO: replace with time tick event
        private void Update()
        {
            if (!_needToUpdate) return;
            
            if (_resetCodeDelta < 0)
            {
                OnTimeFinish();
                return;
            }

            var intDelta = (int) _resetCodeDelta;
            _resetCodeDelta -= Time.deltaTime;

            if (_prevTime == intDelta) return;
            
            _prevTime = intDelta;

            var dateTime = DateTimeOffset.FromUnixTimeSeconds(intDelta).UtcDateTime;
            var formattedTime = dateTime.ToString("mm:ss");
            _resendCodeText.text = "Resend code in " + formattedTime;
        }

        private void OnTimeFinish()
        {
            _needToUpdate = false;
            _resendCodeButton.interactable = true;
            _resendCodeText.text = "Resend code";
        }
    }
}