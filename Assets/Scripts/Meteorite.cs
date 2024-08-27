using System.Collections;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [SerializeField] GameObject flames, explosion, meteorite;
    float damage;
    void Explode()
    {
        meteorite.gameObject.SetActive(false);
        flames.gameObject.SetActive(false);
        explosion.gameObject.SetActive(true);
        StartCoroutine(_Explode());
    }
    IEnumerator _Explode()
    {
        GetComponent<SphereCollider>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
    private void Update() {
        if(transform.position.y < -10) {
            Explode();
        }
    }
    public void SetData(float radius)
    {
        this.damage = radius * 10;
        transform.localScale = new Vector3(radius, radius, radius);
        explosion.transform.localScale = new Vector3(radius, radius, radius);
    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
        else{
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerController>().AddScore(damage);
        }
        if(!other.gameObject.CompareTag("Meteorite")) Explode();
    }
    private void OnEnable() {
        meteorite.gameObject.SetActive(true);
        flames.gameObject.SetActive(true);
        explosion.gameObject.SetActive(false);
        GetComponent<SphereCollider>().enabled = true;
    }
}
