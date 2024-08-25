
using UnityEngine;



public class Inventory : MonoBehaviour
{
    public int NumTorpedos = 0;

    public int Health = 100;



    public void AddContents(Inventory otherInventory)
    {
        NumTorpedos += otherInventory.NumTorpedos;
        Health += otherInventory.Health;
    }



    public int GetTorpedoes(int amount = 1)
    {
        if (NumTorpedos >= amount)
        {
            NumTorpedos -= amount;
            return amount;
        }

        int torpedoesToReturn = NumTorpedos;
        NumTorpedos = 0;
        return torpedoesToReturn;
    }
}
