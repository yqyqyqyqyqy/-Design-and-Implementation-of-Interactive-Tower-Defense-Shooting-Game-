using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Edwon.VR;
using Edwon.VR.Gesture;

public class GestureMainPanel : MonoBehaviour
{
    private Button btn_GestureInfo;
    private Button btn_GestureDetect;
    private VRGestureSettings gestureSettings;
    private VRGestureRig gestureRig;

    private void Awake()
    {
        gestureSettings = Utils.GetGestureSettings();
        gestureRig = VRGestureRig.GetPlayerRig(gestureSettings.playerID);

        btn_GestureInfo = transform.Find("btn_GestureInfo").GetComponent<Button>();
        btn_GestureInfo.onClick.AddListener(() =>
        {
            //进入手势信息页面
            EventCenter.Broadcast(EventDefine.ShowGestureInfoPanel);
            gameObject.SetActive(false);
        });
        btn_GestureDetect = transform.Find("btn_GestureDetect").GetComponent<Button>();
        btn_GestureDetect.onClick.AddListener(() =>
        {
            //进入手势测试界面
            EventCenter.Broadcast(EventDefine.ShowGextureDetectedPanel);
            gameObject.SetActive(false);
        });
        EventCenter.AddListener(EventDefine.ShowGestureMainPanel, Show);

        Show();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGestureMainPanel, Show);
    }
    private void Show()
    {
        gestureRig.uiState = VRGestureUIState.Idle;
        gameObject.SetActive(true);
    }
}
