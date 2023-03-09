using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayer player;

    public override void OnStartServer()
    {
        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();

        health.ServerOnDie += ServerHandleDeath;
        GameOverHandler.ServerOnGameOver += ServerHandleGameover;
    }

    public override void OnStopServer()
    {

        health.ServerOnDie -= ServerHandleDeath;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameover;
    }

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            player.SetResources(player.GetPlayerResources() + resourcesPerInterval);
            timer += interval;
        }
    }

    private void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameover()
    {
        enabled = false;
    }
}
