using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUp : MonoBehaviour
{
    public Transform cube;
    PowerUpController controller;
    PowerUpType type;
    TextMeshPro typeText;
    private void OnEnable() {
        StartCoroutine(Rotate());
        typeText = GetComponentInChildren<TextMeshPro>();
    }
    public void SetData(PowerUpController controller, PowerUpType type)
    {
        this.controller = controller;
        this.type = type;
        typeText.text = this.type.ToString();
    }
    public PowerUpType GetPowerUpType()
    {
        return type;
    }
    private void Update() {
        typeText.transform.LookAt(typeText.transform.position - Camera.main.transform.position);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            controller.CollectPowerUp(this);
        }
    }
    public void Destroy()
    {
        gameObject.SetActive(false);
    }
    IEnumerator Rotate()
    {
        while (true)
        {
            cube.transform.Rotate(0, 1, 0);
            yield return new WaitForSeconds(.01f);
        }
    }
    private void OnDisable() {
        StopAllCoroutines();
    }
}
