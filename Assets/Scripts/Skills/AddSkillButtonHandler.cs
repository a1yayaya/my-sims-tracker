using UnityEngine;

public class AddSkillButtonHandler : MonoBehaviour
{
    public GameObject createSkillPopup;   // drag prefab here

    public void OnClick()
    {
        var canvas = GetComponentInParent<Canvas>();
        var go = Instantiate(createSkillPopup, canvas.transform);
        var pop = go.GetComponent<CreateSkillPopup>();
        var mgr = FindObjectOfType<SkillsManager>();

        pop.onCreate.AddListener((name, diff, icon) => mgr.CreateUserSkill(name, diff, icon));

    }
}
