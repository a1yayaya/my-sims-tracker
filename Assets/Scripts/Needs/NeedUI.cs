using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NeedUI : MonoBehaviour
{
    public NeedDefinition definition;   // set by spawner
    public Slider slider;
    public Button addButton;
    public TMP_Text nameLabel;
    public GameObject popupPrefab;

    GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        nameLabel.text = definition.needName;
        addButton.onClick.AddListener(OpenPopup);
    }

    void Update()
    {
        var rt = gm.GetRuntime(definition);
        slider.value = rt.value / definition.maxValue;
    }

    void OpenPopup()
    {
        var canvas = GetComponentInParent<Canvas>();
        var go = Instantiate(popupPrefab, canvas.transform);
        var pc = go.GetComponent<PopupController>();
        pc.Init($"Add points to {definition.needName}");
        pc.onConfirm.AddListener(points => gm.AddNeedPoints(definition, points));
    }
}
