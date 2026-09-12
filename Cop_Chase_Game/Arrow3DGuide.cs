using UnityEngine;

public class Arrow3DGuide : MonoBehaviour
{
    void Update()
    {
        // Έλεγχος ύπαρξης ενεργού στόχου. Σε περίπτωση απουσίας, διακόπτεται η εκτέλεση.
        if (EscapeManager.activeTarget == null) return;

        // Ανάκτηση των συντεταγμένων του στόχου.
        Vector3 targetPosition = EscapeManager.activeTarget.position;

        // Κλείδωμα του ύψους (άξονας Y) ώστε η περιστροφή του βέλους να διατηρείται αυστηρά σε οριζόντιο επίπεδο.
        targetPosition.y = transform.position.y;

        // Στρέψη του αντικειμένου ώστε να κατευθύνεται προς τη θέση του στόχου.
        transform.LookAt(targetPosition);

        // Περιστροφή κατά 180 μοίρες στον άξονα Y για τη διόρθωση του αρχικού προσανατολισμού του 3D μοντέλου.
        transform.Rotate(0, 180, 0);
    }
}