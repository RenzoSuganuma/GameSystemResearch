using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerController : MonoBehaviour
{
    /* properties */
    //MOVE PROPERTIES
    [SerializeField] float _moveSpd, _lookSen;
    private Vector2 _moveVel, _lookVel;

    //SHOOTING PROPERTIES
    [SerializeField] GameObject _crossHair;

    /* nesessary component to process this code order */
    //COMPONENTS TO MOVE
    [SerializeField] private PlayerInputModule _PLAYERINPUTMODULE;
    private CharacterController _CHARACTERCONTROLLER;

    //FOR CAMERA MOVING
    [SerializeField] private CinemachineVirtualCamera _VIRTUALCAMERA;
    private Camera _camera;

    //FOR CAM DISPLAY
    [SerializeField] private float _fov = 60;
    [SerializeField] private float _zoomRaito = 1;


    //GUNFIRE PROPERTIES
    [SerializeField] private float rayDistance = 100f; // 光線の飛距離
    [SerializeField] private GameObject muzzle;
    [SerializeField] private GameObject muzzleTarget = null;

    // Start is called before the first frame update
    void Awake()
    {
        //CharacterControllerを取得
        if (this.GetComponent<CharacterController>() != null)
        {
            this._CHARACTERCONTROLLER = this.GetComponent<CharacterController>();
        }

        //カーソルのロック
        Cursor.lockState = CursorLockMode.Locked;

        //ゲーム画面を描写しているカメラを取得
        this._camera = Camera.main;

        //シネマシーン仮想カメラの取得
        this._VIRTUALCAMERA = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();

        //感度の設定
        this._lookSen *= Time.deltaTime;
    }

    private void Update()
    {
        #region  プレイヤー操作回り
        //マウス感度設定
        this._VIRTUALCAMERA.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisValue = Mathf.Clamp(this._VIRTUALCAMERA.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisValue, -1, 1);//インスペクタでのプロパティSpeed
        this._VIRTUALCAMERA.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = Mathf.Clamp(this._VIRTUALCAMERA.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisValue, -1, 1);
        this._VIRTUALCAMERA.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = this._lookSen;//インスペクタでのプロパティSpeed
        this._VIRTUALCAMERA.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = this._lookSen;

        //クロスヘア表示非表示
        if (this._PLAYERINPUTMODULE.GetAiming() && this._crossHair != null)
        {
            this._crossHair.SetActive(true);
            this._VIRTUALCAMERA.m_Lens.FieldOfView = this._fov / this._zoomRaito;//ズーム
        }
        else
        {
            this._crossHair.SetActive(false);
            this._VIRTUALCAMERA.m_Lens.FieldOfView = this._fov;//ズームを等倍
        }

        //移動、視点移動のベロシティ代入
        this._moveVel = this._PLAYERINPUTMODULE.GetMoveInput().normalized;
        this._lookVel = this._PLAYERINPUTMODULE.GetLookInput().normalized;
        #endregion
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region  プレイヤー操作回り
        //キャラ移動
        this._CHARACTERCONTROLLER.Move(this.transform.forward * this._moveVel.y * this._moveSpd * Time.deltaTime);
        this._CHARACTERCONTROLLER.Move(this.transform.right * this._moveVel.x * this._moveSpd * Time.deltaTime);

        //カメラのｙ軸の回転量をキャラのｙ軸の回転量に代入
        float camRot_Y;
        camRot_Y = this._camera.transform.rotation.y;

        float objRot_X, objRot_Z, objRot_W;
        objRot_X = this.gameObject.transform.rotation.x;
        objRot_Z = this.gameObject.transform.rotation.z;
        objRot_W = this.gameObject.transform.rotation.w;

        //カメラの正面をキャラも向いてｙ軸回転量を両方同じ大きさ、（カメラのｙ軸回転量）にする
        this.gameObject.transform.forward = this._camera.transform.forward;
        this.gameObject.transform.rotation = new Quaternion(objRot_X,camRot_Y,objRot_Z,objRot_W);
        #endregion

        #region  射撃操作処理周り

        // 画面中央の位置を取得
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // メインカメラから画面中央への光線を作成
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        // 光線との衝突を検出
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance) && this._PLAYERINPUTMODULE.GetFiring())
        {
            // 光線が物体に衝突した場合の処理
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
            Debug.Log("Hit object: " + hit.collider.gameObject.transform.position);
            muzzleTarget = hit.collider.gameObject;
            if (muzzleTarget != null && muzzleTarget.CompareTag("Shootable") && muzzleTarget.TryGetComponent<TargetModule>(out TargetModule targetModule))
            {
                targetModule.ReCoordinate();
            }
        }

        Ray rayMuzlle = (muzzleTarget != null) ? new Ray(muzzle.transform.position, muzzleTarget.transform.position - muzzle.transform.position) : ray;

        RaycastHit hitMuzzle;

        if (Physics.Raycast(rayMuzlle, out hitMuzzle, rayDistance) && this._PLAYERINPUTMODULE.GetFiring())
        {
            // 光線が物体に衝突した場合の処理
            Debug.Log("Hit object Muzzle: " + hitMuzzle.collider.gameObject.name);
            Debug.Log("Hit object Muzzle: " + hitMuzzle.collider.gameObject.transform.position);
        }
        #endregion
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(10, 10, 200, 100), $"{this._moveVel},{_lookVel}");
    }
}
