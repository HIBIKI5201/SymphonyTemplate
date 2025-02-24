using BLINDED_AM_ME;
using UnityEngine;

public class Cutter : MonoBehaviour
{
    public GameObject targetObject; // 切断したいオブジェクト
    public GameObject cuttingPlane; // 切断平面
    public Material capMaterial; // 切断面に適用するマテリアル

    [ContextMenu("Cut")]
    public void Cut()
    {
        if (targetObject != null && cuttingPlane != null && capMaterial != null)
        {
            // オブジェクトを切断
            GameObject[] pieces = MeshCut.Cut(targetObject, cuttingPlane.transform.position, cuttingPlane.transform.up, capMaterial);

            // 切断されたオブジェクトが存在する場合のみ処理を行う
            if (pieces != null)
            {
                GameObject leftSide = pieces[0];
                GameObject rightSide = pieces[1];

                // 例: 右側のオブジェクトを少し移動
                rightSide.transform.position += rightSide.transform.right * 0.1f;
            }
            else
            {
                Debug.LogError("オブジェクトの切断に失敗しました。");
            }
        }
        else
        {
            Debug.LogError("切断に必要な情報が不足しています。");
        }
    }
}