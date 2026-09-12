using UnityEngine;
using UnityEngine.SceneManagement; // Βιβλιοθήκη απαραίτητη για τη διαχείριση και φόρτωση των σκηνών (επιπέδων).

public class MainMenuController : MonoBehaviour
{
    // Μέθοδος που καλείται κατά την αλληλεπίδραση του χρήστη με το κουμπί εκκίνησης (Play).
    public void PlayGame()
    {
        // Φόρτωση της αμέσως επόμενης διαθέσιμης σκηνής βάσει της προκαθορισμένης σειράς στο Build Settings.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Μέθοδος που καλείται κατά την αλληλεπίδραση του χρήστη με το κουμπί εξόδου (Quit).
    public void QuitGame()
    {
        // Εκτύπωση ενημερωτικού μηνύματος στην κονσόλα του Unity για σκοπούς ελέγχου (debugging) κατά την ανάπτυξη.
        Debug.Log("Το παιχνίδι έκλεισε!");

        // Τερματισμός της εκτέλεσης της εφαρμογής
        Application.Quit();
    }
}