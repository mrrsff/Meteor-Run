using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { HPRegen, StaminaRegen, Shield, Speed}
public class PowerUpController : MonoBehaviour
{
    public List<PowerUp> powerUps;
    public BoxCollider spawnArea;
    public GameObject powerUpPrefab;
    public static event System.Action<PowerUpType> OnPowerUpCollected;

    private void Start() {
        PlayerController.OnPlayerDeath += OnPlayerDeath;
        CreatePowerUp(50);
        StartCoroutine(SpawnPowerUpRoutine());
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        while (true)
        {
            var time = UnityEngine.Random.Range(2, 8);
            yield return new WaitForSeconds(time);
            SpawnPowerUp();
        }
    }

    Vector3 GetRandomPos()
    {
        var x = UnityEngine.Random.Range(spawnArea.bounds.min.x + 5f, spawnArea.bounds.max.x - 5f);
        var z = UnityEngine.Random.Range(spawnArea.bounds.min.z + 5f, spawnArea.bounds.max.z - 5f);
        return new Vector3(x, 1, z);
    }
    private void CreatePowerUp(int v)
    {
        for (int i = 0; i < v; i++)
        {
            var powerUp = Instantiate(powerUpPrefab).GetComponent<PowerUp>();
            powerUp.transform.SetParent(transform);
            powerUps.Add(powerUp);
            powerUp.gameObject.SetActive(false);
        }
    }
    public void SpawnPowerUp()
    {
        var powerUp = powerUps.Find(x => !x.gameObject.activeSelf);
        if(powerUp != null)
        {
            powerUp.gameObject.SetActive(true);
            powerUp.transform.position = GetRandomPos();
            var random = UnityEngine.Random.Range(0, Enum.GetValues(typeof(PowerUpType)).Length);
            powerUp.SetData(this, (PowerUpType)random);
        }
    }
    public void CollectPowerUp(PowerUp obj)
    {
        var type = obj.GetPowerUpType();
        OnPowerUpCollected?.Invoke(type);
        obj.Destroy();
    }

    void OnPlayerDeath()
    {
        PlayerController.OnPlayerDeath -= OnPlayerDeath;
        StopAllCoroutines();
        foreach(var powerUp in powerUps)
        {
            Destroy(powerUp.gameObject);
        }
        powerUps.Clear();
    }

}
