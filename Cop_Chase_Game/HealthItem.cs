using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [Header("Ποσότητα Αναπλήρωσης Υγείας")]
    public int healAmount = 2;

    private void OnTriggerEnter(Collider other)
    {
        // Έλεγχος ταυτοποίησης του αντικειμένου που εισήλθε στην περιοχή αλληλεπίδρασης (trigger) μέσω της ετικέτας "Player".
        if (other.CompareTag("Player"))
        {
            // Ανάκτηση του στοιχείου διαχείρισης υγείας του παίκτη.
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            // Επιβεβαίωση ύπαρξης της σχετικής κλάσης στο αντικείμενο.
            if (playerHealth != null)
            {
                // Έλεγχος εάν η τρέχουσα υγεία του παίκτη υπολείπεται της μέγιστης δυνατής τιμής.
                if (!playerHealth.IsHealthFull())
                {
                    // Εκτέλεση της μεθόδου αναπλήρωσης με την προκαθορισμένη ποσότητα υγείας.
                    playerHealth.RestoreHealth(healAmount);

                    // Απενεργοποίηση της οντότητας (απόκρυψη) αντί για οριστική καταστροφή (Destroy),
                    // ώστε να καθίσταται δυνατή η επαναφορά του αντικειμένου (respawn) σε περίπτωση επανεκκίνησης του επιπέδου.
                    gameObject.SetActive(false);
                }
            }
        }
    }
}