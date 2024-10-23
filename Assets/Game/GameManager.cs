using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    // デバッグモードのフラグ
    public bool DebugMode = false;

    // 各種Managerへの参照
    public RoomManager RoomManager;
    public AreaManager AreaManager;
    public EnemyManager enemyManager;
    public EventManager eventManager;
    public ItemManager itemManager;
    public SpecialObjectManager specialObjectManager;
    public static bool debug = false;
    private List<MonoBehaviour> managers = new List<MonoBehaviour>();

    void Start(){
        // ダンジョンの生成
        AreaManager.AreaPartition();
    }

















    void Awake()
    {
        // シングルトンのインスタンスを設定
        if (Instance == null)
        {
            Instance = this;
            // 子オブジェクトからすべてのManagerを検出してリストに追加
            DetectManagers();
        }
        else
        {
            // 既にインスタンスが存在する場合、このGameManagerを破棄
            Destroy(gameObject);
        }
    }











    // 子供のManagerを自動で検出し、リストに追加
    private void DetectManagers()
    {
        managers.Clear();

        foreach (Transform child in transform)
        {
            MonoBehaviour[] childManagers = child.GetComponents<MonoBehaviour>();
            foreach (var manager in childManagers)
            {
                if (manager != null && manager.GetType().Name.EndsWith("Manager"))
                {
                    managers.Add(manager);
                    if (manager is RoomManager) RoomManager = (RoomManager) manager;
                    if (manager is AreaManager) AreaManager = (AreaManager)manager;
                    if (manager is EnemyManager) enemyManager = (EnemyManager)manager;
                    if (manager is EventManager) eventManager = (EventManager)manager;
                    if (manager is ItemManager) itemManager = (ItemManager)manager;
                    if (manager is SpecialObjectManager) specialObjectManager = (SpecialObjectManager)manager;

                }
            }
        }
    }
    // デバッグログを表示するメソッド
    public static void Log(string message)
    {
        if (Instance != null && Instance.DebugMode)
        {
            Debug.Log(message);
        }
    }
}
