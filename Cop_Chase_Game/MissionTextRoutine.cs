using UnityEngine;
using TMPro;
using System.Collections; // Βιβλιοθήκη απαραίτητη για τη χρήση ασύγχρονων ρουτινών (Coroutines) και χρονικών καθυστερήσεων.

public class MissionTextRoutine : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();

        // Εκκίνηση της ασύγχρονης ρουτίνας κατά την αρχικοποίηση της σκηνής.
        StartCoroutine(SequenceRoutine());
    }

    IEnumerator SequenceRoutine()
    {
        // 1. Διατήρηση του κεντρικού μηνύματος στην οθόνη για προκαθορισμένο χρονικό διάστημα (10 δευτερόλεπτα).
        yield return new WaitForSeconds(10f);

        // 2. Διαδικασία σταδιακής απόκρυψης (Fade Out) του κειμένου με συνολική διάρκεια 2 δευτερολέπτων.
        float fadeTime = 2f;
        float currentTime = 0f;
        Color originalColor = textMesh.color;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            // Γραμμική παρεμβολή (Lerp) της τιμής Alpha από 1 (πλήρως ορατό) σε 0 (διαφανές).
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeTime);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 3. Επανατοποθέτηση του στοιχείου UI στην άνω δεξιά γωνία της οθόνης, με προσαρμογή μεγέθους και στοίχισης.
        textMesh.fontSize = 24; // Μείωση του μεγέθους της γραμματοσειράς.
        textMesh.alignment = TextAlignmentOptions.TopRight;

        // Επαναπροσδιορισμός των σημείων αγκίστρωσης (Anchors) και του κέντρου αναφοράς (Pivot) στην άνω δεξιά γωνία.
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);

        // Εφαρμογή περιθωρίου (padding) για την αποφυγή πλήρους επικάλυψης με τα όρια της οθόνης.
        rectTransform.anchoredPosition = new Vector2(-20, -20);

        // 4. ’μεση επαναφορά της ορατότητας του κειμένου στη νέα του θέση (Alpha = 1).
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }
}