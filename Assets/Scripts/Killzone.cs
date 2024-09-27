using UnityEngine;

public class KillZone : MonoBehaviour{
    public Transform player;
    public Vector3 spawnPoint;

    void Start(){
        spawnPoint = player.position;
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")){
            MovePlayerToSpawn();
        }
    }

    void MovePlayerToSpawn(){
        player.position = spawnPoint;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null){
            rb.velocity = Vector3.zero;
        }
    }
}
