using BLINDED_AM_ME;
using UnityEngine;

public class Cutter : MonoBehaviour
{
    public GameObject _targetObject; // 切断したいオブジェクト
    public GameObject _cuttingPlane; // 切断平面
    public Material _capMaterial; // 切断面に適用するマテリアル

    [ContextMenu("Cut")]
    public void Cut()
    {
        if (_targetObject == null || _cuttingPlane == null || _capMaterial == null) Debug.LogError("切断に必要な情報が不足しています。");

        // オブジェクトを切断
        var pieces = MeshCut.Cut(_targetObject, _cuttingPlane.transform.position, _cuttingPlane.transform.up,
            _capMaterial);

        // 切断されたオブジェクトが存在する場合のみ処理を行う
        if (pieces != null)
        {
            var leftSide = pieces[0];
            var rightSide = pieces[1];

            // 右側のオブジェクトを少し移動
            rightSide.transform.position += rightSide.transform.right * 0.1f;
        }
        else
        {
            Debug.LogError("オブジェクトの切断に失敗しました。");
        }
    }
}