// SkillUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays one skill row (XP bar + label + “+ Hours” button) and
/// handles the popup that lets the player log hours.
/// </summary>
public class SkillUI : MonoBehaviour
{
    [Header("UI wiring")]
    public Slider bar;                 // ← drag the XPBar (Slider) here
    public TMP_Text label;             // ← drag SkillLabel (TMP_Text)
    public Button addBtn;              // ← drag AddHoursBtn (Button)
    [Tooltip("Popup prefab with a PopupController on the root")]
    public GameObject addHoursPopup;   // ← drag your AddHoursPopup prefab
    public Button removeBtn;

    [Header("Runtime data")]
    public SkillRuntime runtime;       // ← set by the spawner script

    /* ────────────  PRIVATE  ─────────── */
    SkillsManager mgr;

    /* ────────────  LIFECYCLE  ───────── */
    public Image iconImg;
    void Start()
    {
        mgr = FindObjectOfType<SkillsManager>();
        addBtn.onClick.AddListener(OpenPopup);
        removeBtn.onClick.AddListener(ConfirmRemove);
        if (runtime != null)
            iconImg.sprite = Resources.Load<Sprite>("SkillIcons/" + runtime.iconId);
        Refresh();
    }

    void Update() => Refresh();

    /* ────────────  UI REFRESH  ───────── */
    void Refresh()
    {
        if (runtime == null || mgr == null) return;

        // XP % toward the *next* level
        float needed = mgr.XpNeeded(runtime.level, runtime.difficulty);
        float pct = needed == 0 ? 0 : runtime.xp / needed;

        bar.value = pct;
        label.text = $"{runtime.name} – Lv {runtime.level} ({pct * 100f:0} %)";
    }

    /* ────────────  POPUP  ────────────── */
    void OpenPopup()
    {
        if (addHoursPopup == null) return;

        var canvas = GetComponentInParent<Canvas>();
        var go = Instantiate(addHoursPopup, canvas.transform);
        var popup = go.GetComponent<PopupController>();

        popup.Init($"Hours spent on {runtime.name}");
        popup.onConfirm.AddListener(hours => mgr.AddHours(runtime, hours));
    }
    void ConfirmRemove()
    {
    #if UNITY_EDITOR
        bool ok = UnityEditor.EditorUtility.DisplayDialog(
            "Remove skill?",
            $"Delete '{runtime.name}' permanently?",
            "Remove", "Cancel");
        if (!ok) return;
    #endif
        mgr.RemoveSkill(runtime);   // calls the new method
    }
}
