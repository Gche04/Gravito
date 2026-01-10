using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollusionManager : MonoBehaviour
{
    public bool isOnGround;
    public bool wasAirBorne;
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

        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }


    }

    void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            //isOnGround = false;
            wasAirBorne = true;
        }


    }
}
