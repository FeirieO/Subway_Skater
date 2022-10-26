using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : MonoBehaviour
{
    public int maxCoin = 5;
    public float chanceToSpawn = 0.5f;
    public bool forceSpawnAll = false;

    private GameObject[] _coins;

    private void Awake()
    {
        _coins = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            _coins[i] = transform.GetChild(i).gameObject;
        }

        OnDisable();
    }

    private void OnEnable()
    {
        if (Random.Range(0.0f, 1.0f) > chanceToSpawn)
            return;

        if (forceSpawnAll)
        {
            for (int i = 0; i < maxCoin; i++)
            {
                _coins[i].SetActive(true);
            }
        }
        else
        {
            int r = Random.Range(0, maxCoin);
            for (int i = 0; i < r; i++)
            {
                _coins[i].SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        foreach (GameObject go in _coins)
        {
            go.SetActive(false);
        }
    }
}