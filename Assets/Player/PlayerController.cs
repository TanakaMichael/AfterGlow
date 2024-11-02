using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 移動速度

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // 入力を取得
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 移動方向のベクトルを計算
        Vector3 direction = new Vector3(horizontal, vertical, 0).normalized;

        // キャラクターの位置を更新
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
