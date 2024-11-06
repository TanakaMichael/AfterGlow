using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // シングルトンインスタンス
    private static EventManager _instance;

    public static EventManager Instance
    {
        get
        {
            // インスタンスが存在しない場合は新規作成
            if (_instance == null)
            {
                GameObject obj = new GameObject("EventManager");
                _instance = obj.AddComponent<EventManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    // イベントハンドラーの辞書
    private Dictionary<string, Action<object[]>> eventDictionary;

    private void Awake()
    {
        // シングルトンの確保
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            eventDictionary = new Dictionary<string, Action<object[]>>();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// イベントにリスナーを登録する
    /// </summary>
    /// <param name="eventName">イベント名</param>
    /// <param name="listener">リスナーのデリゲート</param>
    public void StartListening(string eventName, Action<object[]> listener)
    {
        Action<object[]> thisEvent;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // 既存のイベントにリスナーを追加
            thisEvent += listener;
            Instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            // 新規のイベントを作成
            thisEvent += listener;
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// イベントからリスナーを解除する
    /// </summary>
    /// <param name="eventName">イベント名</param>
    /// <param name="listener">リスナーのデリゲート</param>
    public void StopListening(string eventName, Action<object[]> listener)
    {
        if (_instance == null) return;
        Action<object[]> thisEvent;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // イベントからリスナーを削除
            thisEvent -= listener;
            Instance.eventDictionary[eventName] = thisEvent;
        }
    }

    /// <summary>
    /// イベントを発行する
    /// </summary>
    /// <param name="eventName">イベント名</param>
    /// <param name="parameters">イベントに渡すパラメータ</param>
    public void TriggerEvent(string eventName, params object[] parameters)
    {
        Action<object[]> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(parameters);
        }
    }
}
