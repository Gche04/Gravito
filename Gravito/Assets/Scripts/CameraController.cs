using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera mainCamera;
    GameObject player;

    Vector3 cameraOffset;

    [SerializeField] Vector3 cameraPlayerFrontViewOffset = new(0, 1.4f, -7f);
    [SerializeField] Vector3 cameraPlayerBackViewOffset = new(0, 1.4f, -4f);
    [SerializeField] Vector3 cameraSideViewOffset = new(0, 1f, -6f);
    float cameraMoveSpeed;

    float cameraNormalMoveSpeed = 10;
    float cameraJetParkMoveSpeed = 80;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player");

        if (mainCamera == null && player == null)
        {
            Debug.LogError("camera or player is Null");
        }

    }

    void Update()
    {
        TogglePlayerDirToCamera();
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
        }
        else
        {
            if (GameObjectsManager.Instance.PlayerSideIsToCamera)
            {
                cameraOffset = cameraSideViewOffset;
            }
            else if (GameObjectsManager.Instance.PlayerIsBackingCamera)
            {
                cameraOffset = cameraPlayerBackViewOffset;
            }
            else if (GameObjectsManager.Instance.PlayerIsFacingCamera)
            {
                cameraOffset = cameraPlayerFrontViewOffset;
            }
        }

    }


    bool IsPlayerFacingCamera()
    {
        //Calculate the direction vector from the player to the camera
        Vector3 directionToCamera = (mainCamera.transform.position - player.transform.position).normalized;

        //Get the player's forward direction
        Vector3 playerForward = player.transform.forward;

        //Calculate the dot product between the two direction vectors
        //dot product of 1 means the vectors are perfectly aligned (facing the same way).
        //dot product of 0 means they are perpendicular.
        //dot product of -1 means they are opposite (facing away).
        float dotProduct = Vector3.Dot(playerForward, directionToCamera);

        //Compare the dot product result with the threshold
        // If the dot product is greater than the threshold, the player is facing the camera within the defined angle.
        return dotProduct > 0.5f;
    }

    bool IsPlayerBackingCamera()
    {
        Vector3 directionToCamera = (mainCamera.transform.position - player.transform.position).normalized;

        Vector3 playerForward = player.transform.forward;

        float dotProduct = Vector3.Dot(playerForward, directionToCamera);

        return dotProduct < -0.5f;
    }

    void TogglePlayerDirToCamera()
    {
        if (IsPlayerFacingCamera())
        {
            GameObjectsManager.Instance.SetPlayerIsFacingCamera(true);
            GameObjectsManager.Instance.SetPlayerIsBackingCamera(false);
            GameObjectsManager.Instance.SetPlayerSideIsToCamera(false);
        }
        else if (IsPlayerBackingCamera())
        {
            GameObjectsManager.Instance.SetPlayerIsBackingCamera(true);
            GameObjectsManager.Instance.SetPlayerIsFacingCamera(false);
            GameObjectsManager.Instance.SetPlayerSideIsToCamera(false);
        }
        else
        {
            GameObjectsManager.Instance.SetPlayerSideIsToCamera(true);
            GameObjectsManager.Instance.SetPlayerIsBackingCamera(false);
            GameObjectsManager.Instance.SetPlayerIsFacingCamera(false);
        }
    }
}
