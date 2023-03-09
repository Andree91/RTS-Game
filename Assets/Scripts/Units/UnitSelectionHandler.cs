using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : NetworkBehaviour
{
    [SerializeField] private RectTransform unitSelectonArea = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Vector2 startPosition;
    private RTSPlayer player;
    private Camera mainCamera;

    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        mainCamera = Camera.main;

        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Start units section area
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // Stop unit section area
            ClearUnitSectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }

        // if (Mouse.current.leftButton.wasPressedThisFrame) StartSelectionArea();
        // if (Mouse.current.leftButton.isPressed) UpdateSelectionArea();
        // if (Mouse.current.leftButton.wasReleasedThisFrame) ClearSelectionArea();
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.DeSelectUnit();
            }

            SelectedUnits.Clear();
        }

        unitSelectonArea.gameObject.SetActive(true);
        startPosition = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        unitSelectonArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Math.Abs(areaHeight));
        unitSelectonArea.anchoredPosition = startPosition + new Vector2((areaWidth / 2), (areaHeight / 2));
    }

    private void ClearUnitSectionArea()
    {
        unitSelectonArea.gameObject.SetActive(false);

        if (unitSelectonArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

            if (!unit.isOwned) { return; }

            SelectedUnits.Add(unit);

            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.SelectUnit();
            }
            return;
        }

        Vector2 min = unitSelectonArea.anchoredPosition - (unitSelectonArea.sizeDelta / 2);
        Vector2 max = unitSelectonArea.anchoredPosition + (unitSelectonArea.sizeDelta / 2);

        foreach (Unit unit in player.GetPlayerUnits())
        {
            if (SelectedUnits.Contains(unit)) { continue; }

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if (screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y && screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.SelectUnit();
            }
        }
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }

     private void ClientHandleGameOver(string winner)
    {
        enabled = false;
    }
}
