using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillChoosePanel : MonoBehaviour
{
    private string m_GestureName;

    private void Awake()
    {
        EventCenter.AddListener<string>(EventDefine.ShowSkillChoosePanel, Show);
        Init();
    }
    private void Init()
    {
        transform.Find("btn_Back").GetComponent<Button>().onClick.AddListener(() =>
        {
            //更换技能
            for (int i = 0; i < transform.Find("Parent").childCount; i++)
            {
                if (transform.Find("Parent").GetChild(i).GetComponent<Toggle>().isOn)
                {
                    GestureSkillManager.ChangeSkill(m_GestureName, transform.Find("Parent").GetChild(i).name);
                }
            }
            EventCenter.Broadcast(EventDefine.ShowGestureInfoPanel);
            gameObject.SetActive(false);
        });
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.AddListener<string>(EventDefine.ShowSkillChoosePanel, Show);
    }
    private void Update()
    {
        for (int i = 0; i < transform.Find("Parent").childCount; i++)
        {
            if (transform.Find("Parent").GetChild(i).GetComponent<Toggle>().isOn)
            {
                transform.Find("Parent").GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                transform.Find("Parent").GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    private void Show(string gestureName)
    {
        gameObject.SetActive(true);
        m_GestureName = gestureName;
        string skillName = GestureSkillManager.GetSkillNameByGestureName(gestureName);

        for (int i = 0; i < transform.Find("Parent").childCount; i++)
        {
            if (transform.Find("Parent").GetChild(i).name == skillName)
            {
                transform.Find("Parent").GetChild(i).GetComponent<Toggle>().isOn = true;
                transform.Find("Parent").GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                transform.Find("Parent").GetChild(i).GetComponent<Toggle>().isOn = false;
                transform.Find("Parent").GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
