
using System;

using UnityEngine;



public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int numTorpedos = 0;
    public int NumTorpedos { get => numTorpedos; }
    public bool OutOfTorpedoes { get => NumTorpedos <= 0; }

    [SerializeField]
    private float health = 0.0f;
    public float Health { get => health; }
    public bool OutOfHealth { get => Health <= 0.0f; }

    [SerializeField]
    private int maxTorpedoes = 0;
    public int MaxTorpedoes { get => maxTorpedoes; }
    public bool MaxTorpedoesReached { get => NumTorpedos >= MaxTorpedoes; }

    [SerializeField]
    private float maxHealth = float.MaxValue;
    public float MaxHealth { get => maxHealth; }
    public bool MaxHealthReached { get => Health >= MaxHealth; }



    public void Initialize(int numTorpedoes = 0, float health = 0.0f, int maxTorpedoes = int.MaxValue, float maxHealth = float.MaxValue)
    {
        this.numTorpedos = numTorpedoes;
        this.health = health;
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



    public void AddTorpedoes(int amount)
    {
        AddTorpedoes(amount, out _);
    }



    public void AddTorpedoes(int amount, out bool fullOfTorpedoes)
    {
        fullOfTorpedoes = false;
        numTorpedos += amount;

        if (numTorpedos >= maxTorpedoes)
        {
            numTorpedos = maxTorpedoes;
            fullOfTorpedoes = true;
        }
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



    public void SetMaxTorpedoes(int amount)
    {
        SetMaxTorpedoes(amount, out _);
    }



    public void SetMaxTorpedoes(int amount, out bool ejectedTorpedoes)
    {
        maxTorpedoes = amount;
        ejectedTorpedoes = false;

        if (NumTorpedos > maxTorpedoes)
        {
            numTorpedos = maxTorpedoes;
            ejectedTorpedoes = true;
        }
    }



    public void AddHealth(float amount)
    {
        AddHealth(amount, out _);
    }



    public void AddHealth(float amount, out bool fullOfHealth)
    {
        fullOfHealth = false;

        health += amount;
        if (health >= maxHealth)
        {
            health = maxHealth;
            fullOfHealth = true;
        }
    }



    public void TakeHealth(float amount)
    {
        TakeHealth(amount, out _);
    }



    public void TakeHealth(float amount, out bool outOfHealth)
    {
        outOfHealth = false;

        health -= amount;
        if (health <= 0.0f)
        {
            health = 0.0f;
            outOfHealth = true;
        }
    }



    public void SetMaxHealth(float amount)
    {
        SetMaxHealth(amount, out _);
    }



    public void SetMaxHealth(float amount, out bool removedHealth)
    {
        maxHealth = amount;
        removedHealth = false;

        if (health > maxHealth)
        {
            health = maxHealth;
            removedHealth = true;
        }
    }
}
