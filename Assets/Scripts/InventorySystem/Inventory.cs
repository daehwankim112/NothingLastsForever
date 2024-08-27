
using System;

using UnityEngine;



public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int numTorpedos = 0;
    public int NumTorpedos { get => numTorpedos; }
    public bool OutOfTorpedoes { get => NumTorpedos <= 0; }

    [SerializeField]
    private int health = 0;
    public int Health { get => health; }
    public bool OutOfHealth { get => Health <= 0; }

    [SerializeField]
    private int maxTorpedoes = 0;
    public int MaxTorpedoes { get => maxTorpedoes; }
    public bool MaxTorpedoesReached { get => NumTorpedos >= MaxTorpedoes; }

    [SerializeField]
    private int maxHealth = 0;
    public int MaxHealth { get => maxHealth; }
    public bool MaxHealthReached { get => Health >= MaxHealth; }



    public Inventory(int initalTorpedoes = 0, int initalHealth = 0, int maxTorpedoes = int.MaxValue, int maxHealth = int.MaxValue)
    {
        numTorpedos = initalTorpedoes;
        health = initalHealth;
        this.maxTorpedoes = maxTorpedoes;
        this.maxHealth = maxHealth;
    }



    [Obsolete("Use MergeContents instead")]
    public void AddContents(Inventory otherInventory)
    {
        numTorpedos += otherInventory.NumTorpedos;
        health += otherInventory.Health;
    }



    public void MergeContents(Inventory otherInventory)
    {
        numTorpedos += otherInventory.NumTorpedos;
        health += otherInventory.Health;
    }



    [Obsolete("Use TakeTorpedoes instead")]
    public int GetTorpedoes(int amount = 1)
    {
        if (NumTorpedos >= amount)
        {
            numTorpedos -= amount;
            return amount;
        }

        int torpedoesToReturn = NumTorpedos;
        numTorpedos = 0;
        return torpedoesToReturn;
    }



    public int TakeTorpedoes(int amount = 1)
    {
        return TakeTorpedoes(out _, amount);
    }



    public int TakeTorpedoes(out bool outOfTorpedoes, int amount = 1)
    {
        outOfTorpedoes = false;
        if (NumTorpedos >= amount)
        {
            numTorpedos -= amount;
            return amount;
        }

        int torpedoesToReturn = NumTorpedos;
        outOfTorpedoes = true;
        numTorpedos = 0;
        return torpedoesToReturn;
    }



    public void TakeHealth(int amount)
    {
        TakeHealth(amount, out _);
    }



    public void TakeHealth(int amount, out bool outOfHealth)
    {
        outOfHealth = false;

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            outOfHealth = true;
        }
    }
}
