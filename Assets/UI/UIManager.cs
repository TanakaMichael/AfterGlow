using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image dialogueOverlay; // 会話中に表示するオーバーレイ

    private void OnEnable()
    {
        EventManager.Instance.StartListening("OnConversationStart", HandleConversationStart);
        EventManager.Instance.StartListening("OnConversationEnd", HandleConversationEnd);
    }

    private void OnDisable()
    {
        EventManager.Instance.StopListening("OnConversationStart", HandleConversationStart);
        EventManager.Instance.StopListening("OnConversationEnd", HandleConversationEnd);
    }

    private void HandleConversationStart(object[] parameters)
    {
        if (dialogueOverlay != null)
        {
            dialogueOverlay.gameObject.SetActive(true);
        }
    }

    private void HandleConversationEnd(object[] parameters)
    {
        if (dialogueOverlay != null)
        {
            dialogueOverlay.gameObject.SetActive(false);
        }
    }
}
