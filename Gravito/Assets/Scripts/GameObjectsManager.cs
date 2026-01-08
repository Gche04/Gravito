using UnityEngine;

public class GameObjectsManager : MonoBehaviour
{
    public static GameObjectsManager Instance { get; private set; }

    public bool PlayerIsBackingCamera { get; private set; }
    public bool PlayerSideIsToCamera { get; private set; }
    public bool PlayerIsFacingCamera { get; private set; }
    public bool PlayerJetParkIsOn { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {

            Destroy(gameObject);
            return;
        }
        Instance = this;


        DontDestroyOnLoad(gameObject);

    }

    public void SetPlayerIsBackingCamera(bool val) { PlayerIsBackingCamera = val; }
    
    public void SetPlayerSideIsToCamera(bool val) { PlayerSideIsToCamera = val; }

    public void SetPlayerIsFacingCamera(bool val) { PlayerIsFacingCamera = val; }
    public void SetPlayerJetParkIsOn(bool val) { PlayerJetParkIsOn = val; }
}
