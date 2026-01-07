using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollusionManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Horizontal Floor"))
        {
            GameObjectsManager.Instance.SetPlayerIsOnHorizontalFloor(true);
            GameObjectsManager.Instance.SetPlayerIsOnVerticalFloor(false);
        }

        if (collision.gameObject.CompareTag("Vertical Floor"))
        {
            GameObjectsManager.Instance.SetPlayerIsOnHorizontalFloor(false);
            GameObjectsManager.Instance.SetPlayerIsOnVerticalFloor(true);
        }
    }
}
