using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    Transform jetPackFlame;

    [SerializeField] Quaternion usingJetIdle = Quaternion.Euler(0f, 0f, 0f );
    [SerializeField] Quaternion usingJetMove = Quaternion.Euler(0f, 0f, 0f );

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jetPackFlame = transform.Find("Jetpack Flame");

        if (jetPackFlame == null)
        {
            Debug.LogError("Jetpack flame not found");
        }
    }


    public void ToggleFlameOnAndOff(bool isJetpackOn)
    {
        if (isJetpackOn)
        {
            jetPackFlame.gameObject.SetActive(true);
        }
        else
        {
            jetPackFlame.gameObject.SetActive(false);
        }
    }

    public void ToggleFlameRotWithMoveAndIdle(bool isMoveingWithJet)
    {
        if (isMoveingWithJet)
        {
            jetPackFlame.localRotation = usingJetMove;
        }
        else
        {
            jetPackFlame.localRotation = usingJetIdle;
        }
    }

    
}
