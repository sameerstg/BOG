using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.Device;
using UnityEngine.InputSystem;

public class PowerSpawner : MonoBehaviour
{
 
    public List<GameObject> gunPrefabs = new List<GameObject>();
    public GameObject levelSize;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());

    }

    // Update is called once per frame
    void Update()
    {
        
      

    }

    IEnumerator Spawn()
    {
        int randomItem = 0;
        GameObject toSpawn;
        BoxCollider2D mC= levelSize.GetComponent<BoxCollider2D>();

        float screenX, screenY;
        Vector2 pos;
        
        randomItem = Random.Range(0, gunPrefabs.Count);
        toSpawn= gunPrefabs[randomItem];
        screenX = Random.Range(mC.bounds.min.x, mC.bounds.max.x);
        screenY = Random.Range(mC.bounds.min.y, mC.bounds.max.y);
        pos= new Vector2(screenX, screenY);
        GameObject GnPower =  Instantiate(toSpawn, pos, toSpawn.transform.rotation);

        yield return new WaitForSeconds(5f);
        StartCoroutine(Spawn());
        

    }
}

