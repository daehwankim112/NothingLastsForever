using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CubeTorpedo : MonoBehaviour
{
    private GameManager gameManager => GameManager.Instance;


    private Rigidbody rb;


    public TMP_Text NumTorpedoesText;


    public Inventory PlayerInventory;



    void Start()
    {
        rb = GetComponent<Rigidbody>();


        gameManager.OnExplosion += OnExplosion;
    }



    private void Update()
    {
        NumTorpedoesText.text = $"{PlayerInventory.NumTorpedos} torpedoes left";
    }



    private void OnDestroy()
    {
        gameManager.OnExplosion -= OnExplosion;
    }



    private void OnExplosion(object sender, GameManager.OnExplosionArgs args)
    {
        rb.AddExplosionForce(args.Power, args.Position, 10);
    }
}
