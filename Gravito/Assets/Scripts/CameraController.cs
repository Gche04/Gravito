using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera mainCamera;
    GameObject player;

    Vector3 cameraOffset;

    //views are of player. player backing camera is back view
    [SerializeField] Vector3 cameraPlayerFrontViewOffset = new(0, 1.4f, -7f);
    [SerializeField] Vector3 cameraPlayerBackViewOffset = new(1.7f, 1.4f, -4f);
    [SerializeField] Vector3 cameraSideViewOffset = new(0, 1f, -6f);

    [SerializeField] Vector3 jetpackFrontViewOffset = new(0.5f, 1.5f, -5.5f);
    [SerializeField] Vector3 jetpackBackViewOffset = new(0.5f, 1.5f, -5.5f);
    [SerializeField] Vector3 jetpackSideViewOffset = new(0, 1f, -6f);

    float cameraMoveSpeed;

    [SerializeField] float cameraNormalMoveSpeed = 10;
    [SerializeField] float cameraJetParkMoveSpeed = 60;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player");

        if (mainCamera == null && player == null)
        {
            Debug.LogError("camera or player is Null");
        }

        cameraOffset = cameraPlayerBackViewOffset;
    }

    void Update()
    {
        SetCameraOffset();

        if (GameObjectsManager.Instance.PlayerJetParkIsOn)
        {
            cameraMoveSpeed = cameraJetParkMoveSpeed;
        }
        else
        {
            cameraMoveSpeed = cameraNormalMoveSpeed;
        }
    }

    void LateUpdate()
    {
        CameraMovement();
    }

    void CameraMovement()
    {
        mainCamera.transform.position = Vector3.MoveTowards(
            mainCamera.transform.position,
            player.transform.position + cameraOffset,
            cameraMoveSpeed * Time.deltaTime
        );
    }

    void SetCameraOffset()
    {
        if (GameObjectsManager.Instance.PlayerJetParkIsOn)
        {
            cameraOffset = cameraSideViewOffset;
            //CameraOffsetValue(jetpackSideViewOffset, jetpackFrontViewOffset, jetpackBackViewOffset);
        }else
        {
            CameraOffsetValue(cameraSideViewOffset, cameraPlayerFrontViewOffset, cameraPlayerBackViewOffset);
        }
    }

    void CameraOffsetValue(Vector3 side, Vector3 front, Vector3 back)
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            cameraOffset = side;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            cameraOffset = front;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            cameraOffset = back;
        }
    }

}
