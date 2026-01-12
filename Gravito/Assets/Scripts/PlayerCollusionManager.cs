using UnityEngine;

public class PlayerCollusionManager : MonoBehaviour
{
    public bool isOnGround;

    float rayLenght = 0.2f;

    RaycastHit hitData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckIfPlayerIsReallyAirBorne();
    }

    void CheckIfPlayerIsReallyAirBorne() // for sloped floor // when on sloped floor collider doesnt detect floor
    {
        if (!isOnGround)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.01f;

            Ray myRay = new(rayOrigin, Vector3.down);

            if (Physics.Raycast(myRay.origin, myRay.direction, out hitData, rayLenght))
            {
                if (hitData.collider.CompareTag("Ground"))
                {
                    isOnGround = true;
                }
            }
        }
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
            isOnGround = false;
        }
    }
}
