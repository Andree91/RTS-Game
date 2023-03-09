using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<int> ServerOnPlayerDeath;
    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;
    public static event Action<UnitBase> clientOnBaseSpawned;

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDeath;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);

        health.ServerOnDie -= ServerHandleDeath;
    }

    [Server]
    private void ServerHandleDeath()
    {
        ServerOnPlayerDeath?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }

    #endregion

    public override void OnStartClient()
    {
        if (!isOwned) { return; }

        clientOnBaseSpawned?.Invoke(this);
    }

    #region Client

    #endregion
}