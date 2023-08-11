using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Edwon.VR;
using Edwon.VR.Gesture;

public class GestureExampleItem : MonoBehaviour
{
    private VRGestureSettings gestureSettings;
    private string m_GestureName;
    private int m_LineNumber;

    private void Awake()
    {
        gestureSettings = Utils.GetGestureSettings();
        GetComponent<Button>().onClick.AddListener(DeleteExample);
    }
    public void Init(string gestureName, int lineNumber)
    {
        m_GestureName = gestureName;
        m_LineNumber = lineNumber;
    }
    private void DeleteExample()
    {
        //让该手势记录总数减一
        Gesture gesture = gestureSettings.FindGesture(m_GestureName);
        gesture.exampleCount--;

        Utils.DeleteGestureExample(gestureSettings.currentNeuralNet, m_GestureName, m_LineNumber);
        GetComponentInParent<GestureEditPanel>().GetGestureAllExample(m_GestureName);
        GetComponentInParent<GestureEditPanel>().GenerateExamplesGrid();
    }
}
