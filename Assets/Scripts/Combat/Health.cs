using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        UnitBase.ServerOnPlayerDeath += ServerHandelPlayerDeath;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDeath -= ServerHandelPlayerDeath;
    }

    [Server]
    private void ServerHandelPlayerDeath(int playerID)
    {
        if (connectionToClient.connectionId != playerID) { return; }

        DealDamage(currentHealth);
        //NetworkServer.Destroy(gameObject);
    }

    [Server]
    public void DealDamage(int damage)
    {
        if (currentHealth == 0) { return; }

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        // currentHealth -= damage;

        // if (currentHealth < 0)
        // {
        //     currentHealth = 0;
        // }

        if (currentHealth != 0) { return; }

        ServerOnDie?.Invoke();

        Debug.Log("This Unit/Building was destroid");
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}
