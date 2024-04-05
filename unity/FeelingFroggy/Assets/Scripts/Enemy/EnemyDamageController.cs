using UnityEngine;

public class EnemyDamageController : MonoBehaviour
{
    public int damage = 10;
    public float cooldown = 1;

    private float lastHit;

    public void HitPlayer()
    {
        if (Time.time - lastHit < cooldown)
        {
            return;
        }

        lastHit = Time.time;

        PlayerController.Instance.Damage(damage);
    }
}
