// LightSource.cs
using UnityEngine;

public class LightSource : MonoBehaviour
{
    public Vector2Int position; // タイル座標
    public Color color = Color.white; // 光の色
    public float intensity = 1.0f; // 光の強度
    public float range = 10f; // 光の範囲
    public bool isDynamic = false; // 動的か静的か

    private void Update()
    {
        if (isDynamic)
        {
            // 例えば、プレイヤーに追従する場合
            Vector3 pos = transform.position;
            position = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        }
    }
}
