using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

    private void Awake()
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    public void OnPointerEnter(PointerEventData eventData) // OnMouseEnter
    {
        healthBarParent.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) // OnMouseExit
    {
        healthBarParent.SetActive(false);
    }
}
