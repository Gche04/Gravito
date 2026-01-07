using NUnit.Framework;
using UnityEngine;

public class GameObjectsManager : MonoBehaviour
{
    public static GameObjectsManager Instance { get; private set; }

    public bool PlayerIsOnHorizontalFloor { get; private set; }
    public bool PlayerIsOnVerticalFloor { get; private set; }
    public bool PlayerIsFacingCamera { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

    
        DontDestroyOnLoad(this.gameObject);

    }
    
    public void SetPlayerIsOnHorizontalFloor(bool val){PlayerIsOnHorizontalFloor = val;}

    public void SetPlayerIsOnVerticalFloor(bool val){PlayerIsOnVerticalFloor = val;}

    public void SetPlayerIsFacingCamera(bool val){PlayerIsFacingCamera = val;}
}
