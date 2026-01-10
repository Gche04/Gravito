using UnityEngine;

public class GameObjectsManager : MonoBehaviour
{
    public static GameObjectsManager Instance { get; private set; }
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
    public void SetPlayerJetParkIsOn(bool val) { PlayerJetParkIsOn = val; }
    
}
