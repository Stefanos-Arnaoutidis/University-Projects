using UnityEngine;

public class EscapeItemTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Έλεγχος αν το αντικείμενο που εισήλθε στην περιοχή αλληλεπίδρασης (trigger) φέρει την ετικέτα "Player".
        if (other.CompareTag("Player"))
        {
            // Κλήση του κεντρικού διαχειριστή (EscapeManager) για την εκκίνηση της διαδικασίας νίκης.
            EscapeManager.instance.TriggerWin();
        }
    }
}