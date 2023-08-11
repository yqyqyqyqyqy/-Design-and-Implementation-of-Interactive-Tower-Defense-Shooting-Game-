using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Edwon.VR;
using Edwon.VR.Gesture;

public class GestureEditPanel : MonoBehaviour
{
    public GameObject go_GestureExampleItem;
    public Material m_Mat;
    private Text txt_GestureName;
    private Button btn_Back;
    private Button btn_Record;

    private VRGestureSettings gestureSettings;
    private VRGestureRig gestureRig;
    /// <summary>
    /// 手势所对应的所有的手势记录list
    /// </summary>
    private List<GestureExample> gestureExamples = new List<GestureExample>();
    private List<GameObject> exampleObjList = new List<GameObject>();
    /// <summary>
    /// 手势名字
    /// </summary>
    private string m_GestureName;
    /// <summary>
    /// 判断是否开始录制
    /// </summary>
    private bool m_IsStartRecord = false;

    private void Awake()
    {
        gestureSettings = Utils.GetGestureSettings();
        gestureRig = VRGestureRig.GetPlayerRig(gestureSettings.playerID);

        EventCenter.AddListener<string>(EventDefine.ShowGestureEditPanel, Show);
        EventCenter.AddListener(EventDefine.FinishedOnceRecord, FinishedOnceRecord);
        EventCenter.AddListener<bool>(EventDefine.UIPointHovering, UIPointHovering);
        Init();
    }
    private void Init()
    {
        txt_GestureName = transform.Find("txt_GestureName").GetComponent<Text>();
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Back.onClick.AddListener(() =>
        {
            BeginTraining();
            m_IsStartRecord = false;
            EventCenter.Broadcast(EventDefine.ShowGestureInfoPanel);
            gameObject.SetActive(false);
        });
        btn_Record = transform.Find("btn_Record").GetComponent<Button>();
        btn_Record.onClick.AddListener(BeginRecord);

        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventDefine.ShowGestureEditPanel, Show);
        EventCenter.RemoveListener(EventDefine.FinishedOnceRecord, FinishedOnceRecord);
        EventCenter.RemoveListener<bool>(EventDefine.UIPointHovering, UIPointHovering);
    }
    private void Show(string gestureName)
    {
        m_GestureName = gestureName;
        gameObject.SetActive(true);
        txt_GestureName.text = gestureName;
        BeginEditGesture(gestureName);
    }
    /// <summary>
    /// 开始训练
    /// </summary>
    private void BeginTraining()
    {
        gestureSettings.BeginTraining(FinishTraining);
    }
    private void FinishTraining(string netName)
    {

    }
    /// <summary>
    /// 获取射线是否检测到UI
    /// </summary>
    /// <param name="value"></param>
    private void UIPointHovering(bool value)
    {
        if (m_IsStartRecord)
        {
            if (value)
            {
                gestureRig.uiState = VRGestureUIState.Gestures;
            }
            else
            {
                BeginRecord();
            }
        }
    }
    /// <summary>
    /// 完成一次手势记录得录制会调用到这个方法
    /// </summary>
    private void FinishedOnceRecord()
    {
        GetGestureAllExample(m_GestureName);
        GenerateExamplesGrid();
    }
    /// <summary>
    /// 开始录制手势
    /// </summary>
    private void BeginRecord()
    {
        m_IsStartRecord = true;
        gestureRig.BeginReadyToRecord(m_GestureName);
    }
    /// <summary>
    /// 开始编辑手势
    /// </summary>
    /// <param name="gestureName"></param>
    private void BeginEditGesture(string gestureName)
    {
        gestureRig.uiState = VRGestureUIState.Editing;
        gestureRig.BeginEditing(gestureName);
        GetGestureAllExample(gestureName);
        GenerateExamplesGrid();
    }
    /// <summary>
    /// 获取手势的所有记录
    /// </summary>
    /// <param name="gestureName"></param>
    public void GetGestureAllExample(string gestureName)
    {
        gestureExamples = Utils.GetGestureExamples(gestureName, gestureSettings.currentNeuralNet);
        foreach (var item in gestureExamples)
        {
            if (item.raw)
            {
                item.data = Utils.SubDivideLine(item.data);
                item.data = Utils.DownScaleLine(item.data);
            }
        }
    }
    /// <summary>
    /// 生成所有得记录
    /// </summary>
    public void GenerateExamplesGrid()
    {
        foreach (var obj in exampleObjList)
        {
            Destroy(obj);
        }
        exampleObjList.Clear();

        for (int i = 0; i < gestureExamples.Count; i++)
        {
            GameObject go = Instantiate(go_GestureExampleItem, transform.Find("Parent"));
            go.GetComponent<GestureExampleItem>().Init(gestureExamples[i].name, i);
            LineRenderer line = go.GetComponentInChildren<LineRenderer>();
            line.useWorldSpace = false;
            line.material = m_Mat;
            line.startColor = Color.blue;
            line.endColor = Color.green;
            float lineWidth = 0.01f;
            line.startWidth = lineWidth - (lineWidth * 0.5f);
            line.endWidth = lineWidth + (lineWidth * 0.5f);
            line.positionCount = gestureExamples[i].data.Count;

            for (int j = 0; j < gestureExamples[i].data.Count; j++)
            {
                gestureExamples[i].data[j] = gestureExamples[i].data[j] * 40;
            }
            line.SetPositions(gestureExamples[i].data.ToArray());

            exampleObjList.Add(go);
        }
    }
}
