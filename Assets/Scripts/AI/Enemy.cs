using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyCombat enemyCombat;

    private void Start()
    {
        enemyCombat = GetComponentInParent<EnemyCombat>();
    }

    // Detectar colisiones con el jugador o sus ataques
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerWeapon"))
        {       
            var player = FindAnyObjectByType<PlayerCombat>();
            
            if (player.currentAttack.Equals("BasicAttackPlayer"))
            {
                enemyCombat.TakeHit(20);
            }
            else if (player.currentAttack.Equals("ComboAttackPlayer"))
            {
                enemyCombat.TakeHit(100);
            } 
        }
    }
}
