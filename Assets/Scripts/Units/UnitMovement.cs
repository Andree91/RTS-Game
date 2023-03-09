using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10.0f;
    [SerializeField] Canvas displayCanvas;
    //private Camera mainCamera;

    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();

        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange) // Vector3.Distance() use squareroot which is heavier for performance that square Magitude which is only square
            {
                // Chase
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                // Stop and Attack
                agent.ResetPath();
            }
            return;
        }

        if (!agent.hasPath) { return; }
        if (agent.remainingDistance > agent.stoppingDistance) { return; }

        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    [Server]
    public void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();
        // Server Validation
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        agent.SetDestination(hit.position);
    }

    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    #endregion

    // #region Client

    // // Start method for client
    // public override void OnStartAuthority()
    // {
    //     //base.OnStartAuthority();
    //     mainCamera = Camera.main;
    // }

    // [ClientCallback]
    // private void Update()
    // {
    //     if (!isOwned) { return; } // At Old version it was hasAuthority

    //     if (displayCanvas != null)
    //     {
    //         displayCanvas.transform.rotation = Quaternion.LookRotation((transform.position - Camera.main.transform.position).normalized);
    //     }

    //     // New Input System
    //     if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }
    //     Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

    //     // Old Input System
    //     //if (!Input.GetMouseButtonDown(1)) { return; }
    //     //Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

    //     if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }

    //     CmdMove(hit.point);
    // }
    // #endregion

}
