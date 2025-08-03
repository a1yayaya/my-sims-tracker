using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PopupController : MonoBehaviour
{
    [Header("Wiring")]
    public TMP_InputField inputField;
    public TMP_Text titleLabel;
    public UnityEvent<float> onConfirm = new();   // outsiders subscribe

    public void Init(string title)
    {
        titleLabel.text = title;
        inputField.text = "";
        inputField.Select();
    }

    public void ClickOK()
    {
        if (float.TryParse(inputField.text, out float calories))
            onConfirm.Invoke(calories);
        Destroy(gameObject);  // close
    }
    public void ClickCancel() => Destroy(gameObject);
}
