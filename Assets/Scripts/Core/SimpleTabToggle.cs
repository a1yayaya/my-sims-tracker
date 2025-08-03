using UnityEngine;
using UnityEngine.UI;

public class SimpleTabToggle : MonoBehaviour
{
    [Header("Buttons ➔ Panels (same order)")]
    public Toggle[] toggles;          // Needs, Skills, …
    public GameObject[] panels;       // NeedsPanel, SkillsPanel, …

    void Awake()
    {
        // Start with everything hidden
        for (int i = 0; i < panels.Length; i++)
        {
            int index = i;   // local copy for lambda
            toggles[i].isOn = false;
            panels[i].SetActive(false);

            toggles[i].onValueChanged.AddListener(on =>
            {
                // When this toggle changes, show/hide its panel
                panels[index].SetActive(on);

                // Turn off all other toggles & panels if this one turned on
                if (on)
                {
                    for (int j = 0; j < toggles.Length; j++)
                        if (j != index)
                        {
                            toggles[j].SetIsOnWithoutNotify(false);
                            panels[j].SetActive(false);
                        }
                }
            });
        }
    }
}
