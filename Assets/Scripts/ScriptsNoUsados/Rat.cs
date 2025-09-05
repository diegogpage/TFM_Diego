using UnityEngine;

public class Rat : Enemy, ICanAttack
{
    public void Attack()
    {
        Debug.Log(this.gameObject.name + " has attacked!");
    }

    public void StopAttacking()
    {
        
    }
}
