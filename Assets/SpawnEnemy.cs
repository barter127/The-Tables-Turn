using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    //Declare type of enemies that can spawn.
    [SerializeField] private GameObject genericPunk;

    //Transforms here will be used to spawn an enemy at a certain position.
    [SerializeField] private Transform[] enemySpawnerLocation;

    void Start()
    {
        
    }

    void Update()
    {        

    }
}
