using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Edwon.VR;
using Edwon.VR.Gesture;

public class GestureDetectedPanel : BaseGestureRecognition
{
    private Button btn_Back;
    private Text txt_GestureName;
    private Text txt_GestureConfidence;

    public override void Awake()
    {
        base.Awake();
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Back.onClick.AddListener(() =>
        {
            EventCenter.Broadcast(EventDefine.ShowGestureMainPanel);
            gameObject.SetActive(false);
        });
        txt_GestureName = transform.Find("txt_GestureName").GetComponent<Text>();
        txt_GestureConfidence = transform.Find("txt_GestureConfidence").GetComponent<Text>();
        EventCenter.AddListener(EventDefine.ShowGextureDetectedPanel, Show);

        gameObject.SetActive(false);
    }

    IEnumerator Dealy(string gestureName, double confidence)
    {
        txt_GestureName.text = gestureName;
        txt_GestureConfidence.text = confidence.ToString("F3");
        yield return new WaitForSeconds(0.5f);
        txt_GestureName.text = "";
        txt_GestureConfidence.text = "";
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.RemoveListener(EventDefine.ShowGextureDetectedPanel, Show);
    }
    private void Show()
    {
        gameObject.SetActive(true);
        BeginRecognition();
    }

    public override void OnGestureDetectedEvent(string gestureName, double confidence)
    {
        StartCoroutine(Dealy(gestureName, confidence));
    }
}
