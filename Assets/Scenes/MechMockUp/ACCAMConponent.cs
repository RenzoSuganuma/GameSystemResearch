using System.Collections.Generic;
using UnityEngine;
using RuntimeLog;
/// <summary>ACのカメラ動作コンポーネント</summary>
public class ACCAMComponentAlpha : MonoBehaviour
{
    /// <summary>ランタイムログ</summary>
    RuntimeLogComponent _logComponent;
    /// <summary>入力ハンドラー</summary>
    ACInputHandler _inputHandler;
    /// <summary>オクルージョンしたオブジェクトを格納しておく</summary>
    GameObject _occuludedObject;
    /// <summary>カメラの中心座標</summary>
    [SerializeField] Transform _centerTransform;
    /// <summary>カメラ位置のオフセット</summary>
    [SerializeField] Vector3 _offset;
    /// <summary>入力感度</summary>
    [SerializeField] Vector2 _sencitivity;
    /// <summary>カメラの高さ</summary>
    [SerializeField] float _camHeight;
    /// <summary>回転半径</summary>
    [SerializeField] float _rotateRadius;
    /// <summary>回転の反転を有効にするかのフラグ</summary>
    [SerializeField] bool _inverseRotation;
    /// <summary>オクルージョンさせるのにアサインする透明の描写をするためのマテリアル</summary>
    [SerializeField] Material _assignTransparentMaterial;
    /// <summary>カメラ移動に必要な三角関数のシータに対応する値X軸</summary>
    float _thetaX = 0;
    /// <summary>カメラ移動に必要な三角関数のシータに対応する値Y軸</summary>
    float _thetaY = 0;
    void Start()
    {
        //ログコンポーネントのインスタンス化
        _logComponent = new(new Rect(0, 0, 100, 100));
        //入力インスタンス化
        _inputHandler = GameObject.FindFirstObjectByType<ACInputHandler>();
        //NULLだったら警告ログを吐き出す
        if (GetComponent<Camera>() == null) Debug.LogWarning("プレイヤーカメラが見つからない");
        if (_centerTransform == null) Debug.LogWarning("ターゲットの座標がnullだよ");
        if (GetComponent<Rigidbody>() == null) Debug.LogWarning("剛体コンポーネントがない");
        //ターゲットを向く
        TargettingSequence(_centerTransform);
        //Rigidbodyプロパティ初期化 これがないと衝突の判定ができない
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.freezeRotation = true;
    }
    void Update()
    {
        //回転処理 横回転
        RotateSequence();
        //ターゲットを向く
        TargettingSequence(_centerTransform);
        //オクルージョン
        OcculusionSequence();
    }
    #region privateメソッド
    /// <summary>オクルージョン処理</summary>
    private void OcculusionSequence()
    {
        //ターゲットとの距離の算出
        var dis = Vector3.Distance(_centerTransform.position, this.transform.position);
        //ターゲットに向かう向きのベクトルの算出
        var dir = _centerTransform.position - this.transform.position;
        //光線の生成
        Ray ray = new(this.transform.position, dir);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction, Color.magenta, dis);
        //光線が何かに当たったら
        if (Physics.Raycast(ray, out hit))
        {
            //オクルージョン処理
            if (hit.transform.gameObject.TryGetComponent<OcculutionTarget>(out OcculutionTarget target))
            {
                target.OverwriteMaterial(_assignTransparentMaterial);
                _occuludedObject = target.gameObject;
            }
            //オクルージョン解除処理
            else if (_occuludedObject != null)
            {
                if (_occuludedObject.TryGetComponent<OcculutionTarget>(out OcculutionTarget component))
                {
                    component.OverwriteMaterial(component.Material);
                }
            }
            //Debug.Log($"{nameof(OcculusionSequence)}:{hit.transform.gameObject.name}");
        }
    }
    /// <summary>Y軸回転処理</summary>
    private void RotateSequence()
    {
        //入力処理
        float inputX = _inputHandler.LookInput.x * _sencitivity.x * .01f;
        _thetaX += inputX;
        float inputY = _inputHandler.LookInput.y * _sencitivity.y * .01f;
        _thetaY += inputY;
        var sign = (_inverseRotation) ? -1 : 1;
        //座標更新
        this.transform.position =
            new Vector3(Mathf.Cos(_thetaX) * sign, Mathf.Sin(_thetaY) * sign, Mathf.Sin(_thetaX) * sign)
            * _rotateRadius + _centerTransform.position + _offset;
    }
    /// <summary>捕捉処理</summary>
    private void TargettingSequence(Transform targetTransform)
    {
        //LookRotationの第一引数に正面方向のベクトルを指定してターゲットのオブジェクトを向く
        this.transform.rotation =
            Quaternion.LookRotation(targetTransform.position - this.transform.position
            , Vector3.up);
    }
    #endregion
    private void OnGUI()
    {
        //ログの描写表示
        _logComponent.DisplayLog("ログ出力テスト");
    }
    private void OnDrawGizmos()
    {
        //回転半径の球メッシュ描写
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_centerTransform.position, _rotateRadius);
    }
}