using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Managerクラスをリストで保持（Inspectorに表示される）
    public DungeonManager dungeonManager;
    public EnemyManager enemyManager;
    public EventManager eventManager;
    public ItemManager itemManager;
    public SpecialObjectManager specialObjectManager;
    public static bool debug = false;
    private List<MonoBehaviour> managers = new List<MonoBehaviour>();

    void Start(){
        // ダンジョンの生成
        
    }

















    void Awake()
    {
        // 子オブジェクトからすべてのManagerを検出してリストに追加
        DetectManagers();
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

                    if (manager is DungeonManager) dungeonManager = (DungeonManager)manager;
                    if (manager is EnemyManager) enemyManager = (EnemyManager)manager;
                    if (manager is EventManager) eventManager = (EventManager)manager;
                    if (manager is ItemManager) itemManager = (ItemManager)manager;
                    if (manager is SpecialObjectManager) specialObjectManager = (SpecialObjectManager)manager;

                }
            }
        }
    }
}
