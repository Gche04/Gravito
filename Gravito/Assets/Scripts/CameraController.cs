using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera mainCamera;
    GameObject player;

    Vector3 cameraOffset;
    [SerializeField] Vector3 cameraDefaultOffset = new(0, 1.4f, -3f);
    [SerializeField] Vector3 cameraBackwardOffset = new(0, 1.4f, -3f);
    [SerializeField] Vector3 cameraVerticalviewOffset = new(0, 1.4f, -4f);
    [SerializeField] Vector3 cameraHorizontalViewOffset = new(0, 1f, -6f);
    [SerializeField] float cameraMoveSpeed = 10;

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
        if (IsPlayerFacingCamera())
        {
            GameObjectsManager.Instance.SetPlayerIsFacingCamera(true);
        }else
        {
            GameObjectsManager.Instance.SetPlayerIsFacingCamera(false);
        }

        SetCameraOffset();
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
        if (GameObjectsManager.Instance.PlayerIsOnHorizontalFloor)
        {
            cameraOffset = cameraHorizontalViewOffset;
        }else if (GameObjectsManager.Instance.PlayerIsOnVerticalFloor)
        {
            cameraOffset = cameraVerticalviewOffset;
        }else if (GameObjectsManager.Instance.PlayerIsFacingCamera)
        {
            cameraOffset = cameraBackwardOffset;
        }
        else
        {
            cameraOffset = cameraDefaultOffset;
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
}
