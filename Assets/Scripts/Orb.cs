using UnityEngine;

public class Orb: MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject effect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            playerMovement.Jump();
            effect.SetActive(true);

            playerMovement.DJReady = true;
        }
    }
}
