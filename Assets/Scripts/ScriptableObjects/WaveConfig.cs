﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] private GameObject enemyPrefab = null;
    [SerializeField] private GameObject pathPrefab = null;
    [SerializeField] private float timeBetweenSpawns = 0.5f;
    [SerializeField] private float spawnRandomFactor = 0.3f;
    [SerializeField] private int numberOfEnemies = 5;

    public GameObject GetEnemyPrefab() => enemyPrefab;
    public GameObject GetPathPrefab() => pathPrefab;
    public float GetTimeBetweenSpawns() => timeBetweenSpawns;
    public float GetSpawnRandomFactor() => spawnRandomFactor;
    public int GetNumberOfEnemies() => numberOfEnemies;

    public List<Transform> GetWaypoints()
    {
        var waveWaypoints = new List<Transform>();
        foreach (Transform child in pathPrefab.transform)
        {
            waveWaypoints.Add(child.transform);
        }

        return waveWaypoints;
    }
}