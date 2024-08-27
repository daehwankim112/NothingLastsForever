using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PalmMenuInventoryUI : MonoBehaviour
{

    // Reference to the text component for number of torpedos remaining
    public TextMeshProUGUI torpedoText;
    // Reference to the text component for health remaining
    public TextMeshProUGUI healthText;

    //reference to the inventory
    public Inventory ourInventory;

    private void Start()
    {
        ourInventory = FindObjectOfType<Inventory>();
    }

    void Update()
    {

        // Update the text components with the value of someVariable
        torpedoText.text = ourInventory.NumTorpedos.ToString();
        healthText.text = ourInventory.Health.ToString();
    }
}
