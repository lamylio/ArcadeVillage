using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


/* 
<note>
    TODO: Implement the butterfly spawner

    The idea is, after unlocking a bugnet in the shop (not implemented yet), the player can catch butterflies
    in order to complete quests and get money. The butterflies will be spawned at random locations, but using a Pool.

    Remark: this will not be implemented yet.
</note>
 */

public class Butterflies : MonoBehaviour
{
    public GameObject butterflyPrefab;
    ObjectPool<GameObject> butterflyPool;

    GameObject OnCreateEntity(){
        
        GameObject entity = Instantiate(butterflyPrefab, transform.position, transform.rotation);
        entity.AddComponent<MeshCollider>().convex = true;
        // entity.AddComponent<>().SetPool(butterflyPool);
        return entity;
    }

    
    void OnTakeEntity(GameObject poolObject)
    {
        // Vector3 randomPosition = GetNewRandomPosition();
        // poolObject.transform.position = transform.position + randomPosition;
        // poolObject.transform.rotation = transform.rotation;

        poolObject.SetActive(true);
    }

    void OnReleaseEntity(GameObject poolObject)
    {
        poolObject.SetActive(false);
    }

    void OnDestroyEntity(GameObject poolObject){
        Destroy(poolObject);
    }
}