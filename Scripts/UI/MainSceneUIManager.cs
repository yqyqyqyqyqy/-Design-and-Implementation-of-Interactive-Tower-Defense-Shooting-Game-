using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MainSceneUIManager : MonoBehaviour
{
    private GameObject go_StartPanel;
    private GameObject go_AboutPanel;
    private GameObject go_GesturePanel;
    private Button btn_StartGame;
    /// <summary>
    /// 开始界面是否显示
    /// 用来判断语音唤醒 开始游戏 是否可以开始
    /// </summary>
    private bool m_StartPanelIsShow = false;

    private void Awake()
    {
        go_StartPanel = transform.Find("StartPanel").gameObject;
        go_AboutPanel = transform.Find("AboutPanel").gameObject;
        go_GesturePanel = transform.Find("GesturePanel").gameObject;
        btn_StartGame = go_StartPanel.transform.Find("btn_Start").GetComponent<Button>();
        btn_StartGame.onClick.AddListener(() =>
        {
            btn_StartGame.gameObject.SetActive(false);
            EventCenter.Broadcast(EventDefine.StartLoadScene);
        });
        EventCenter.AddListener<bool>(EventDefine.IsShowAboutPanel, IsShowAboutPanel);
        EventCenter.AddListener<bool>(EventDefine.IsShowStartPanel, IsShowStartPanel);
        EventCenter.AddListener<bool>(EventDefine.IsShowGesturePanel, IsShowGesturePanel);
        EventCenter.AddListener(EventDefine.StartGame, StartGame);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<bool>(EventDefine.IsShowAboutPanel, IsShowAboutPanel);
        EventCenter.RemoveListener<bool>(EventDefine.IsShowStartPanel, IsShowStartPanel);
        EventCenter.RemoveListener<bool>(EventDefine.IsShowGesturePanel, IsShowGesturePanel);
        EventCenter.RemoveListener(EventDefine.StartGame, StartGame);
    }
    /// <summary>
    /// 语音唤醒开始游戏的调用
    /// </summary>
    private void StartGame()
    {
        if (m_StartPanelIsShow)
        {
            btn_StartGame.gameObject.SetActive(false);
            EventCenter.Broadcast(EventDefine.StartLoadScene);
        }
    }
    private void IsActive(bool value, GameObject go)
    {
        if (value)
        {
            go.transform.DOLocalMoveY(0, 0.5f);
        }
        else
        {
            go.transform.DOLocalMoveY(1080, 0.5f);
        }
    }
    private void IsShowAboutPanel(bool value)
    {
        IsActive(value, go_AboutPanel);
    }
    private void IsShowStartPanel(bool value)
    {
        m_StartPanelIsShow = value;
        IsActive(value, go_StartPanel);
    }
    private void IsShowGesturePanel(bool value)
    {
        IsActive(value, go_GesturePanel);
    }
}
