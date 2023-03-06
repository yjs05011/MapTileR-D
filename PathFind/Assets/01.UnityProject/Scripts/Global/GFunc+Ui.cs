using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static partial class GFunc
{
    public static Vector2 GetCameraSize()
    {
        Vector2 cameraSize = Vector2.zero;
        cameraSize.y = Camera.main.orthographicSize * 2.0f;
        cameraSize.x = cameraSize.y * Camera.main.aspect;
        return cameraSize;
    }
    //! �ؽ�Ʈ�޽����� ������ ������Ʈ�� �ؽ�Ʈ�� �����ϴ� �Լ�
    public static void SetTmpText(this GameObject obj_, string text_)
    {
        TMP_Text tmpTxt = obj_.GetComponent<TMP_Text>();
        if (tmpTxt == null || tmpTxt == default(TMP_Text))
        {
            return;
        }


        tmpTxt.text = text_;
    }       // SetTextMeshPro()0
}
