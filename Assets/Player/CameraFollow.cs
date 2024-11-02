using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         // 追従する対象（プレイヤーなど）
    public float smoothSpeed = 0.125f; // カメラの追従速度
    public Vector3 offset;           // カメラのオフセット位置

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("追従対象が設定されていません");
            return;
        }

        // 目標位置を計算
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, -10);

        // スムーズに補間して位置を更新
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
