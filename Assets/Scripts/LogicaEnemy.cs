using UnityEngine;

public class LogicaEnemy : MonoBehaviour
{
    public int hp = 3;
    public int dañoArma = 1;
    public Animator anim;

    private bool puedeRecibirDaño = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arma") && puedeRecibirDaño)
        {
            RecibirDaño();
        }
    }

    private void RecibirDaño()
    {
        puedeRecibirDaño = false;

        // Animación
        if (anim != null)
        {
            anim.SetTrigger("Damage");
        }

        // Restar vida
        hp -= dañoArma;

        // Muerte
        if (hp <= 0)
        {
            Destroy(gameObject);
        }

        // Evitar daño continuo
        Invoke(nameof(ResetDaño), 0.5f); // ajusta tiempo según tu animación
    }

    private void ResetDaño()
    {
        puedeRecibirDaño = true;
    }
}