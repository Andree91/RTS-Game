using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourcesCost = 10;
    [SerializeField] private Health health = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeSelected = null;

    public static event Action<Unit> ServerOnUnitSpawned; // This start with the key word "Server", because it is only called on server
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public int GetResourcesCost()
    {
        return resourcesCost;
    }

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region  Server

    // Server tell every client that these event happenend
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie += ServerHandleDeath;
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDeath;
    }

    private void ServerHandleDeath()
    {
        // Reward money to the killer
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned) { return; }
        
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void SelectUnit()
    {
        if (!isOwned) { return; }

        onSelected?.Invoke(); // ?.Invoke() because if Event is null and it wouldn't even d oanything, then don't invoke it. Just a safety method
    }

    [Client]
    public void DeSelectUnit()
    {
        if (!isOwned) { return; }

        onDeSelected?.Invoke();
    }

    #endregion
}
