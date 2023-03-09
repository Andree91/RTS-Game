using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target;

    public Targetable GetTarget()
    {
        return target;
    }

    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    //    // .... Removed some parts from the standard solution .... //

    //     [SerializeField] private Transform turretObject = null;
    //     [SerializeField] private float turnSpeed = 20f;

    //    // .... Removed some parts from the standard solution .... //

    //     private void Update()
    //     {
    //         // Object might not have a turret object that can be turned
    //         if (turretObject != null)
    //         {
    //             // Tasrget might not be set.
    //             if(target != null)
    //             {
    //                 // Getting the look direction to the target
    //                 Vector3 dir = target.transform.position - turretObject.transform.position;
    //                 Quaternion lookRotation = Quaternion.LookRotation(dir);

    //                 // Using a lerp to turn the cannon toward the target.
    //                 Vector3 rotation = Quaternion.Lerp(turretObject.rotation, lookRotation, Time.deltaTime * turnSpeed)
    //                     .eulerAngles;

    //                 // doing the actual turn.
    //                 turretObject.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    //             }
    //             else if (Mathf.Abs(turretObject.transform.rotation.y) > Mathf.Epsilon)
    //             {
    //                 // if there I no target, turn the cannon slowly back to the local zero position.
    //                 Vector3 rotation = Quaternion.Lerp(turretObject.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * turnSpeed).eulerAngles;
    //                 turretObject.localRotation = Quaternion.Euler(0f, rotation.y, 0f);
    //             }
    //         }
    //     }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) { return; }

        //this.target = target;
        target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        if (target != null)
        {
            target = null;
        }
    }

    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
        //enabled = false;
    }

    #endregion

    #region Client



    #endregion
}
