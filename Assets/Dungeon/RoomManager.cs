using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Dungeon Area Settings")]
    public int width = 100;
    public int height = 100;
    
    public ScriptableObject selectedRoomGenerationAlgorithm;

    private IPartitioningAlgorithm PartitionAlgorithm;


    private Node node;
    // 宣言の初期設定
    void Awake()
    {
        if (selectedRoomGenerationAlgorithm is IPartitioningAlgorithm)
        {
            PartitionAlgorithm = (IPartitioningAlgorithm)selectedRoomGenerationAlgorithm;
        }
        else
        {
            Debug.LogError("選択されたアルゴリズムが無効です。");
        }
        node = new Node(0, 0, width, height);
    }
    public void AreaPartition(){
        PartitionAlgorithm.Partition(node);
    }
}
