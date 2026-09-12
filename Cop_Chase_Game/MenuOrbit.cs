using UnityEngine;

public class MenuOrbit : MonoBehaviour
{
    [Header("Ταχύτητα Περιστροφής")]
    public float spinSpeed = 5f;

    void Update()
    {
        // Ομαλή περιστροφή του αντικειμένου (κάμερας) γύρω από τον κατακόρυφο άξονα (Y) //
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }
}