using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.VR;
using Edwon.VR.Gesture;
using UnityEngine.UI;

public class GestureInfoPanel : MonoBehaviour
{
    public GameObject go_GestureItem;

    private VRGestureSettings gestureSettings;
    private VRGestureRig gestureRig;
    private Button btn_Back;

    private void Awake()
    {
        gestureSettings = Utils.GetGestureSettings();
        gestureRig = VRGestureRig.GetPlayerRig(gestureSettings.playerID);
        EventCenter.AddListener(EventDefine.ShowGestureInfoPanel, Show);

        Init();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGestureInfoPanel, Show);
    }
    private void Init()
    {
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Back.onClick.AddListener(() =>
        {
            EventCenter.Broadcast(EventDefine.ShowGestureMainPanel);
            gameObject.SetActive(false);
        });
        foreach (var gesture in GetGestureList())
        {
            GameObject go = Instantiate(go_GestureItem, transform.Find("Parent"));
            go.GetComponent<GestureInfoItem>().Init(gesture);
        }
        gameObject.SetActive(false);
    }
    private List<Gesture> GetGestureList()
    {
        gestureSettings.RefreshGestureBank(true);
        return gestureSettings.gestureBank;
    }
    private void Show()
    {
        gestureRig.uiState = VRGestureUIState.Gestures;
        gameObject.SetActive(true);
    }
}
