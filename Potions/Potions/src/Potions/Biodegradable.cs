using Photon.Pun;
using UnityEngine;

namespace Potions;

public class Biodegradable: MonoBehaviour
{
    private float lifetime;
    
    private void Awake()
    {
        lifetime = Random.Range(25f, 50f);
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0 && PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }
}
