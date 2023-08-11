using Edwon.VR.Gesture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureInfoItem : MonoBehaviour
{
    private Button btn_Edit;
    private Gesture m_CurrentGesture;
    private Button btn_SkillChoose;

    private void Awake()
    {
        btn_Edit = transform.Find("btn_Edit").GetComponent<Button>();
        btn_Edit.onClick.AddListener(() =>
        {
            EventCenter.Broadcast(EventDefine.ShowGestureEditPanel, m_CurrentGesture.name);
            transform.parent.parent.gameObject.SetActive(false);
        });
        btn_SkillChoose = transform.Find("btn_SkillChoose").GetComponent<Button>();
        btn_SkillChoose.onClick.AddListener(() =>
        {
            EventCenter.Broadcast(EventDefine.ShowSkillChoosePanel, m_CurrentGesture.name);
            transform.parent.parent.gameObject.SetActive(false);
        });

    }
    public void Init(Gesture gesture)
    {
        m_CurrentGesture = gesture;
        OnEnable();
    }
    private void OnEnable()
    {
        if (m_CurrentGesture == null) return;
        transform.Find("txt_Name").GetComponent<Text>().text = m_CurrentGesture.name;
        transform.Find("txt_ExampleCount").GetComponent<Text>().text = m_CurrentGesture.exampleCount.ToString();
        btn_SkillChoose.GetComponent<Image>().sprite = GestureSkillManager.GetSkilSpriteByGestureName(m_CurrentGesture.name);
    }
}
