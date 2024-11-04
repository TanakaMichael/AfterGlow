// PlayerController.cs
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LightSource playerLight;

    void Start()
    {
        // LightingManagerから動的ライトを取得
        LightingManager lightingManager = GetComponent<LightingManager>();
        if (lightingManager != null)
        {
            playerLight = lightingManager.lightSources.Find(ls => ls.isDynamic);
            if (playerLight == null)
            {
                Debug.LogWarning("No dynamic LightSource found for the player.");
            }
        }
    }

    void Update()
    {
        // プレイヤーの移動処理（例としてキーボード入力を使用）
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveX, moveY, 0) * Time.deltaTime * 5f; // 速度調整

        transform.position += move;

        if (playerLight != null)
        {
            // ライトの位置をプレイヤーの位置に同期
            playerLight.transform.position = transform.position;
            playerLight.position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        }
    }
}
