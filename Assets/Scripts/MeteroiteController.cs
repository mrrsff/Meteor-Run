using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteroiteController : MonoBehaviour
{
    int wave;
    public static event System.Action<int> OnWaveChange;
    List<Meteorite> meteorites;
    GameObject meteoriteParent;
    [SerializeField] BoxCollider spawnArea;
    [SerializeField] GameObject meteoritePrefab;
    
    void Start()
    {
        wave = 0;
        meteoriteParent = gameObject;
        meteorites = new List<Meteorite>();
        AddMeteorite();
        StartCoroutine(SpawnMeteorites());
        PlayerController.OnPlayerDeath += OnPlayerDeath;
    }

    private void AddMeteorite(int count = 100)
    {
        for (int i = 0; i < count; i++)
        {
            var meteorite = Instantiate(meteoritePrefab, meteoriteParent.transform);
            meteorite.SetActive(false);
            meteorites.Add(meteorite.GetComponent<Meteorite>());
        }
    }

    void OnPlayerDeath()
    {
        PlayerController.OnPlayerDeath -= OnPlayerDeath;
        StopAllCoroutines();
        foreach (Transform child in meteoriteParent.transform)
        {
            Destroy(child.gameObject);
        }
        meteorites.Clear();
    }
    void SpawnMeteorites(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var meteorite = GetMeteorite();
            var radius = Random.Range(0.2f, 4f) + wave / 10f;
            meteorite.SetData(radius);
            meteorite.GetComponent<Rigidbody>().velocity = Vector3.down * (wave + 1) / 5f;
            meteorite.transform.position = GetRandomPosition(radius);
            meteorite.transform.rotation = Quaternion.identity;
            meteorite.gameObject.SetActive(true);
        }
    }
    Meteorite GetMeteorite()
    {
        foreach (var meteorite in meteorites)
        {
            if (!meteorite.gameObject.activeSelf)
            {
                return meteorite;
            }
        }
        AddMeteorite(15);
        return GetMeteorite();
    }
    Vector3 GetRandomPosition(float radius)
    {
        // create a random position within the spawn area with a radius
        radius *= 3;
        var x = Random.Range(spawnArea.bounds.min.x + radius, spawnArea.bounds.max.x - radius);
        var y = Random.Range(10,100);
        var z = Random.Range(spawnArea.bounds.min.z + radius, spawnArea.bounds.max.z - radius);
        return new Vector3(x, y, z);
    }
    IEnumerator SpawnMeteorites()
    {
        while(true)
        {
            SpawnMeteorites(10 + wave);
            yield return new WaitUntil(() => {
                if(meteorites.TrueForAll(m => !m.gameObject.activeSelf))
                {
                    wave++;
                    OnWaveChange?.Invoke(wave);
                    return true;
                }
                return false;
            });
        }
    }
}
