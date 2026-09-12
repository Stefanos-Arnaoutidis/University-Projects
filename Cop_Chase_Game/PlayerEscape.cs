using UnityEngine;
using System.Collections; // Βιβλιοθήκη απαραίτητη για τη χρήση ασύγχρονων ρουτινών (Coroutines).

public class PlayerEscape : MonoBehaviour
{
    [Header("Οθόνη Νίκης (Απαιτείται Canvas Group)")]
    public CanvasGroup winScreenGroup;

    [Header("Διάρκεια Σταδιακής Εμφάνισης (σε δευτερόλεπτα)")]
    public float fadeDuration = 2f;

    private bool hasEscaped = false;

    private void OnTriggerEnter(Collider other)
    {
        // Έλεγχος σύγκρουσης με το αντικείμενο απόδρασης και επιβεβαίωση ότι η συνθήκη νίκης δεν έχει ήδη εκπληρωθεί.
        if (other.CompareTag("EscapeTool") && !hasEscaped)
        {
            hasEscaped = true;

            // Εκκίνηση της ασύγχρονης ρουτίνας για τη σταδιακή οπτική μετάβαση (fade-in).
            StartCoroutine(FadeInAndFreeze());
        }
    }

    IEnumerator FadeInAndFreeze()
    {
        float currentTime = 0f;

        // Σταδιακή αύξηση της αδιαφάνειας (Alpha) της οθόνης νίκης μέσω γραμμικής παρεμβολής (Lerp).
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            winScreenGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
            yield return null; // Αναμονή έως την εκτέλεση του επόμενου καρέ (frame).
        }

        // Διασφάλιση της πλήρους εμφάνισης της οθόνης (100% αδιαφάνεια) στο τέλος της διαδικασίας.
        winScreenGroup.alpha = 1f;

        // Παύση της ροής του χρόνου εντός του παιχνιδιού, σηματοδοτώντας τη λήξη της ενεργού αλληλεπίδρασης.
        Time.timeScale = 0;
    }
}