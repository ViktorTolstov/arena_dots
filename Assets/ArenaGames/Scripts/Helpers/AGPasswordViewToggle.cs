using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class AGPasswordViewToggle : MonoBehaviour
{
    public TMP_InputField m_PasswordField;
    public Toggle m_ShowPassword;

    public GameObject m_ImgPasswordNotVisible;
    public GameObject m_ImgPasswordVisible;

    private void OnEnable()
    {
        m_ShowPassword.onValueChanged.AddListener((value) => OnShowPassword(value));
    }

    private void OnDisable()
    {
        m_ShowPassword.onValueChanged.RemoveListener((value) => OnShowPassword(value));
    }

    private void OnShowPassword(bool _ShowPass)
    {
        m_ImgPasswordNotVisible.SetActive(!_ShowPass);
        m_ImgPasswordVisible.SetActive(_ShowPass);

        if (_ShowPass)
            m_PasswordField.contentType = TMP_InputField.ContentType.Standard;
        else
            m_PasswordField.contentType = TMP_InputField.ContentType.Password;

        m_PasswordField.ForceLabelUpdate();
    }
}
