using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEditor.UIElements;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject objItem;
    public Vector2 launchforce = new Vector2(10,10);
    public Vector2 randomLaunchOffset = new Vector2(2, 2);
    public ItemObject[] allItens;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q)) { 
            GameObject spawnedItem = Instantiate(objItem, transform.position, Quaternion.identity);
            spawnedItem.layer = 3 ;
            spawnedItem.GetComponent<Rigidbody2D>().velocity = new Vector2 (Random.Range(launchforce.x - randomLaunchOffset.x, launchforce.x + randomLaunchOffset.x), Random.Range(launchforce.y - randomLaunchOffset.y, launchforce.y + randomLaunchOffset.y));
            spawnedItem.GetComponent<Item>().item = allItens[Random.Range(0, allItens.Length)];
        }
    }
}
