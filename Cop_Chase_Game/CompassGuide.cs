using UnityEngine;

public class CompassGuide : MonoBehaviour
{
    [Header("Σύνδεση με τον Παίκτη")]
    public Transform playerTransform;

    private RectTransform arrowRect;

    void Start()
    {
        // Ανάκτηση του στοιχείου RectTransform του γραφικού βέλους (UI).
        arrowRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Διακοπή της εκτέλεσης σε περίπτωση απουσίας του στόχου ή της αναφοράς του παίκτη.
        if (EscapeManager.activeTarget == null || playerTransform == null) return;

        // 1. Υπολογισμός του διανύσματος κατεύθυνσης από τον παίκτη προς τον στόχο, 
        // αποκλειστικά στο οριζόντιο επίπεδο (άξονες X, Z).
        Vector3 targetDirection = EscapeManager.activeTarget.position - playerTransform.position;
        targetDirection.y = 0;

        // 2. Ανάκτηση του διανύσματος εμπρόσθιας κατεύθυνσης (forward) του παίκτη.
        Vector3 playerForward = playerTransform.forward;
        playerForward.y = 0;

        // 3. Υπολογισμός της προσημασμένης γωνίας (σε μοίρες) μεταξύ της κατεύθυνσης του παίκτη και του στόχου.
        float angle = Vector3.SignedAngle(playerForward, targetDirection, Vector3.up);

        // 4. Εφαρμογή της περιστροφής στο γραφικό βέλος (UI). 
        // Το αρνητικό πρόσημο αντισταθμίζει την αντίστροφη φορά περιστροφής του συστήματος συντεταγμένων του UI.
        arrowRect.localRotation = Quaternion.Euler(0, 0, -angle);
    }
}