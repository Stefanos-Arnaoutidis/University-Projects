using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Μουσικά Θέματα (Audio Clips)")]
    public AudioClip musicStandard;
    public AudioClip musicChase;
    public AudioClip musicVictory;

    private AudioSource audioSource;
    // Μετρητής ενεργών εχθρών που βρίσκονται σε κατάσταση καταδίωξης.
    private int copsChasingMe = 0;
    // Σημαία (flag) που αποτρέπει την αλλαγή μουσικής μετά την επίτευξη της νίκης.
    private bool gameWon = false;

    void Awake()
    {
        // Αρχικοποίηση της μοναδικής παρουσίας (Singleton pattern) του διαχειριστή μουσικής.
        if (instance == null) instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();

        // Προεπιλεγμένη ρύθμιση για συνεχή αναπαραγωγή (loop) της μουσικής υπόκρουσης.
        audioSource.loop = true;
    }

    void Start()
    {
        PlayStandardMusic();
    }

    public void CopStartedChasing()
    {
        // Έλεγχος αποτροπής αλλαγής της μουσικής εάν το επίπεδο έχει ήδη ολοκληρωθεί επιτυχώς.
        if (gameWon) return;

        // Αύξηση του μετρητή καταδίωξης. Εάν πρόκειται για τον πρώτο εχθρό που εντοπίζει τον στόχο, εκκινείται το μουσικό θέμα δράσης.
        copsChasingMe++;
        if (copsChasingMe == 1)
        {
            audioSource.clip = musicChase;
            audioSource.Play();
        }
    }

    public void CopStoppedChasing()
    {
        if (gameWon) return;

        // Μείωση του μετρητή καταδίωξης. Εάν δεν εκκρεμεί άλλη ενεργή καταδίωξη, επαναφέρεται το βασικό θέμα περιήγησης.
        copsChasingMe--;
        if (copsChasingMe <= 0)
        {
            copsChasingMe = 0;
            PlayStandardMusic();
        }
    }

    public void PlayStandardMusic()
    {
        if (gameWon) return;
        audioSource.clip = musicStandard;
        audioSource.Play();
    }

    public void PlayVictoryMusic()
    {
        gameWon = true;
        audioSource.clip = musicVictory;

        // Απενεργοποίηση της συνεχούς αναπαραγωγής (loop) αποκλειστικά για το μουσικό θέμα νίκης.
        audioSource.loop = false;
        audioSource.Play();
    }

    // --- Μέθοδοι Διαχείρισης Κατάστασης Απώλειας Ζωής ---

    public void StopMusicForDeath()
    {
        // ’μεση διακοπή της τρέχουσας μουσικής αναπαραγωγής κατά την απώλεια ζωής.
        audioSource.Stop();
    }

    public void ResetMusicAfterRespawn()
    {
        if (gameWon) return;

        // Επαναφορά του μετρητή καταδίωξης για την αποφυγή εσφαλμένης διατήρησης της κατάστασης συναγερμού.
        copsChasingMe = 0;

        // Επανεκκίνηση του βασικού μουσικού θέματος κατά την επαναφορά (respawn) του παίκτη.
        PlayStandardMusic();
    }
}