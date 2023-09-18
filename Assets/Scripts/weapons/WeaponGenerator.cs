using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour
{

    public static WeaponGenerator _instance;
    public List<GameObject> gunPrefabs = new List<GameObject>();
    public float generationDelay;
    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        gunPrefabs = Resources.LoadAll<GameObject>("Weapons\\").ToList();
    }
    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());

    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            int randomItem = UnityEngine.Random.Range(0, gunPrefabs.Count);
            Vector2 pos = new Vector2(0, 2);
            PhotonNetwork.Instantiate($"Weapons\\{gunPrefabs[randomItem].name}", pos, gunPrefabs[randomItem].transform.rotation);

        }
    }
}

