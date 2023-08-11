using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public string LoadSceneName;
    private Text text;
    private AsyncOperation m_Ao;
    private bool m_IsLoad = false;

    private void Awake()
    {
        text = GetComponent<Text>();
        gameObject.SetActive(false);
        EventCenter.AddListener(EventDefine.StartLoadScene, StartLoadScene);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.StartLoadScene, StartLoadScene);
    }
    private void StartLoadScene()
    {
        gameObject.SetActive(true);
        StartCoroutine("Load");
    }
    IEnumerator Load()
    {
        int startProcess = -1;
        int endProcess = 100;
        while (startProcess < endProcess)
        {
            startProcess++;
            Show(startProcess);
            if (m_IsLoad == false)
            {
                m_Ao = SceneManager.LoadSceneAsync(LoadSceneName);
                m_Ao.allowSceneActivation = false;
                m_IsLoad = true;
            }
            yield return new WaitForEndOfFrame();
        }
        if (startProcess == 100)
        {
            m_Ao.allowSceneActivation = true;
            StopCoroutine("Load");
        }
    }
    private void Show(int value)
    {
        text.text = value.ToString() + "%";
    }
}
