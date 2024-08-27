using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    void Start()
    {
        MeteroiteController.OnWaveChange += OnWaveChange;
        PlayerController.OnPlayerDeath += OnPlayerDeath;
    }
    void OnWaveChange(int wave)
    {
        var currentScale = transform.localScale;
        currentScale.x += .1f;
        currentScale.z += .1f;
        transform.localScale = currentScale;
    }
    void OnPlayerDeath()
    {
        MeteroiteController.OnWaveChange -= OnWaveChange;
        PlayerController.OnPlayerDeath -= OnPlayerDeath;
    }
}
