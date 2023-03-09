using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody _rb = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float lauchForce = 10f;

    private void Start() 
    {
        _rb.velocity = transform.forward * lauchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds); // No typing errors on string argument
        //Invoke("DestroySelf", destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other) 
    {
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) { return; }
        }

        if (other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
        }
        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
