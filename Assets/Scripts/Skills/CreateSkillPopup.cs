using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CreateSkillPopup : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_Dropdown difficultyDropdown;
    public Transform iconContent;      // Content of the scroll view
    public Image iconPreview;      // (optional)

    public UnityEngine.Events.UnityEvent<
        string, Difficulty, string> onCreate  // name, diff, iconId
        = new();

    Sprite[] iconSprites;
    string selectedId;

    void Start()
    {
        // Load sprites once
        iconSprites = Resources.LoadAll<Sprite>("SkillIcons");

        foreach (Sprite s in iconSprites)
        {
            var btn = new GameObject(s.name, typeof(Image), typeof(Button));
            btn.transform.SetParent(iconContent, false);

            var img = btn.GetComponent<Image>();
            img.sprite = s;

            var b = btn.GetComponent<Button>();
            string id = s.name;             // capture
            b.onClick.AddListener(() => SelectIcon(id, s));
        }

        // Auto-select first icon
        if (iconSprites.Any())
            SelectIcon(iconSprites[0].name, iconSprites[0]);
    }

    void SelectIcon(string id, Sprite sprite)
    {
        selectedId = id;
        if (iconPreview) iconPreview.sprite = sprite;

        // simple visual highlight: tint all to white, selected to green
        foreach (Image img in iconContent.GetComponentsInChildren<Image>())
            img.color = (img.sprite == sprite) ? new Color(0.3f, 1f, 0.3f) : Color.white;
    }

    /*------------- existing OK/Cancel -------------*/
    public void ClickOK()
    {
        string nm = nameInput.text.Trim();
        if (string.IsNullOrEmpty(nm) || selectedId == null) return;

        Difficulty diff = (Difficulty)difficultyDropdown.value;
        onCreate.Invoke(nm, diff, selectedId);
        Destroy(gameObject);
    }
    public void ClickCancel() => Destroy(gameObject);
}
