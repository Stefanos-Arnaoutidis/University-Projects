using UnityEngine;
using TMPro;
using System.Collections;

public class EscapeManager : MonoBehaviour
{
    public static EscapeManager instance;
    public static Transform activeTarget;

    // Καθολική μεταβλητή (global state) που υποδεικνύει την επίτευξη της νίκης, αποτρέποντας περαιτέρω αλληλεπιδράσεις (π.χ. λήψη ζημιάς).
    public static bool hasWon = false;

    [Header("Τα 4 Αντικείμενα")]
    public GameObject[] escapeItems;

    [Header("Το κείμενο στην οθόνη")]
    public TextMeshProUGUI screenMessage;

    [Header("Ηχητικά Νίκης")]
    public AudioSource winAudioSource;
    public AudioClip sfxWinVoice;

    [Header("Οθόνη Νίκης (UI)")]
    public GameObject winScreenPanel;

    void Awake()
    {
        // Αρχικοποίηση της μοναδικής παρουσίας (Singleton pattern) του διαχειριστή.
        instance = this;
    }

    void Start()
    {
        // Επαναφορά της κατάστασης νίκης κατά την έναρξη του επιπέδου.
        hasWon = false;

        // Αρχική απόκρυψη όλων των πιθανών αντικειμένων απόδρασης.
        foreach (GameObject item in escapeItems)
        {
            item.SetActive(false);
        }

        // Τυχαία επιλογή ενός αντικειμένου απόδρασης και ενεργοποίησή του ως κύριου στόχου.
        int randomIndex = Random.Range(0, escapeItems.Length);
        GameObject winnerItem = escapeItems[randomIndex];
        winnerItem.SetActive(true);

        activeTarget = winnerItem.transform;

        // Δυναμική προσθήκη πράσινου φωτισμού (Point Light) στο αντικείμενο στόχο για διευκόλυνση του εντοπισμού του.
        Light beacon = winnerItem.AddComponent<Light>();
        beacon.type = LightType.Point;
        beacon.color = Color.green;
        beacon.range = 250f;
        beacon.intensity = 100f;

        // Ενημέρωση του γραφικού περιβάλλοντος (UI) με τις αρχικές οδηγίες.
        if (screenMessage != null)
        {
            screenMessage.text = "Find the escape tool!";
        }

        if (winAudioSource == null) winAudioSource = GetComponent<AudioSource>();

        // Διασφάλιση της απόκρυψης της οθόνης νίκης κατά την έναρξη.
        if (winScreenPanel != null) winScreenPanel.SetActive(false);
    }

    public void TriggerWin()
    {
        // Εκτέλεση ηχητικού εφέ νίκης.
        if (winAudioSource != null && sfxWinVoice != null)
        {
            winAudioSource.PlayOneShot(sfxWinVoice);
        }

        // Ενημέρωση του κειμένου στο UI.
        if (screenMessage != null)
        {
            screenMessage.text = "You Escaped!";
        }

        // Ενημέρωση της καθολικής μεταβλητής για τον τερματισμό της δυνατότητας λήψης ζημιάς από τον παίκτη.
        hasWon = true;

        // Ειδοποίηση του διαχειριστή μουσικής για την αναπαραγωγή του μουσικού θέματος νίκης.
        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayVictoryMusic();
        }

        // Εκκίνηση της ασύγχρονης ρουτίνας μετάβασης.
        StartCoroutine(WinFadeRoutine());
    }

    IEnumerator WinFadeRoutine()
    {
        // Σταδιακή εμφάνιση (fade-in) της οθόνης νίκης μέσω ομαλής αύξησης της διαφάνειάς της (alpha).
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);

            CanvasGroup canvasGroup = winScreenPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                while (canvasGroup.alpha < 1f)
                {
                    canvasGroup.alpha += Time.unscaledDeltaTime * 0.5f;
                    yield return null;
                }
            }
        }

        // Παύση της ροής του χρόνου εντός του παιχνιδιού.
        Time.timeScale = 0;

        // Αναμονή 4 δευτερολέπτων σε πραγματικό χρόνο (real-time) για την ολοκλήρωση της οπτικοακουστικής ανατροφοδότησης.
        yield return new WaitForSecondsRealtime(4f);

        // Επαναφορά της ροής του χρόνου.
        Time.timeScale = 1;

        // Δυναμικός έλεγχος των διαθέσιμων σκηνών (επιπέδων) για την επιλογή του επόμενου προορισμού φόρτωσης.
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        int totalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        if (nextSceneIndex >= totalScenes)
        {
            // Εάν δεν υφίσταται επόμενο επίπεδο, πραγματοποιείται επιστροφή στο κεντρικό μενού (Σκηνή 0).
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        else
        {
            // Φόρτωση του επόμενου διαθέσιμου επιπέδου.
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
        }
    }
}