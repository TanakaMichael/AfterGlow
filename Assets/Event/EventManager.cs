// EventManager.cs
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
        if (listener == null)
        {
            Debug.LogWarning($"Attempted to add a null listener to event '{eventName}'");
            return;
        }

        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            // 既存のイベントにリスナーを追加
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            // 新規のイベントを作成
            eventDictionary.Add(eventName, listener);
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
        
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent -= listener;
            if (thisEvent == null)
            {
                eventDictionary.Remove(eventName);
            }
            else
            {
                eventDictionary[eventName] = thisEvent;
            }
        }
    }

    /// <summary>
    /// イベントを発行する
    /// </summary>
    /// <param name="eventName">イベント名</param>
    /// <param name="parameters">イベントに渡すパラメータ</param>
    public void TriggerEvent(string eventName, params object[] parameters)
    {
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.Invoke(parameters);
        }
        else
        {
            Debug.LogWarning($"Event '{eventName}' triggered, but no listeners are registered.");
        }
    }

    private void OnDestroy()
    {
        if (eventDictionary != null)
        {
            eventDictionary.Clear();
        }
    }
}
