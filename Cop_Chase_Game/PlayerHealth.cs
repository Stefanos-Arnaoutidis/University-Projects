using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Ρυθμίσεις Υγείας & Ζωών")]
    public int maxHealth = 8;
    private int currentHealth;
    public int totalLives = 3;

    [Header("Σύνδεση με UI")]
    public Image healthPieImage;
    public TextMeshProUGUI livesText;
    public GameObject gameOverScreen;
    public TextMeshProUGUI centerMessageText;

    [Header("Ηχητικά Εφέ Παίκτη (SFX)")]
    public AudioSource playerAudio;
    public AudioClip sfxDamage;
    public AudioClip sfxHeal;
    public AudioClip sfxDeathJingle;
    public AudioClip sfxGameOver;
    public AudioClip sfxPlayerDeath;

    private Vector3 startingPosition;
    private bool isDead = false;

    void Start()
    {
        // Ανάκτηση του δείκτη της τρέχουσας ενεργής σκηνής (επιπέδου).
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == 1)
        {
            // Αρχικοποίηση των ζωών κατά την έναρξη του πρώτου επιπέδου και αποθήκευσή τους στη μνήμη (PlayerPrefs).
            totalLives = 3;
            PlayerPrefs.SetInt("SavedLives", 3);
        }
        else
        {
            // Ανάκτηση των εναπομεινάντων ζωών από τα προηγούμενα επίπεδα.
            totalLives = PlayerPrefs.GetInt("SavedLives", 3);
        }

        currentHealth = maxHealth;
        startingPosition = transform.position;

        if (gameOverScreen != null) gameOverScreen.SetActive(false);

        if (playerAudio == null) playerAudio = GetComponent<AudioSource>();

        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        // Αποτροπή λήψης ζημιάς στην περίπτωση που ο παίκτης έχει ήδη χάσει τη ζωή του ή έχει επιτευχθεί η συνθήκη νίκης.
        if (isDead || EscapeManager.hasWon) return;

        currentHealth -= damage;

        if (playerAudio != null && sfxDamage != null)
        {
            playerAudio.PlayOneShot(sfxDamage);
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public bool IsHealthFull()
    {
        return currentHealth >= maxHealth;
    }

    public void RestoreHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (playerAudio != null && sfxHeal != null)
        {
            playerAudio.PlayOneShot(sfxHeal);
        }

        UpdateHealthUI();
    }

    void HandleDeath()
    {
        // Διακοπή της τρέχουσας μουσικής αναπαραγωγής κατά την απώλεια ζωής.
        if (MusicManager.instance != null)
        {
            MusicManager.instance.StopMusicForDeath();
        }

        totalLives--;

        // ’μεση ενημέρωση της μνήμης με τη νέα τιμή των διαθέσιμων ζωών.
        PlayerPrefs.SetInt("SavedLives", totalLives);

        UpdateHealthUI();

        // Έλεγχος διαθεσιμότητας υπολειπόμενων ζωών για την επιλογή μεταξύ επαναφοράς (respawn) ή πλήρους ήττας (Game Over).
        if (totalLives > 0)
        {
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            isDead = true;
            StartCoroutine(GameOverFadeRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        // Αναπαραγωγή ηχητικών εφέ απώλειας ζωής.
        if (playerAudio != null)
        {
            if (sfxPlayerDeath != null) playerAudio.PlayOneShot(sfxPlayerDeath);
            if (sfxDeathJingle != null) playerAudio.PlayOneShot(sfxDeathJingle);
        }

        // Προσωρινή παύση της ροής του χρόνου εντός του παιχνιδιού.
        Time.timeScale = 0;

        // Σταδιακή εμφάνιση (fade-in) του μηνύματος αποτυχίας.
        if (centerMessageText != null)
        {
            centerMessageText.text = "Too bad...";

            Color textColor = centerMessageText.color;
            textColor.a = 0f;
            centerMessageText.color = textColor;

            float fadeTimer = 0f;
            float fadeDuration = 1f;

            while (fadeTimer < fadeDuration)
            {
                fadeTimer += Time.unscaledDeltaTime;
                textColor.a = fadeTimer / fadeDuration;
                centerMessageText.color = textColor;
                yield return null;
            }

            textColor.a = 1f;
            centerMessageText.color = textColor;
        }

        // Αναμονή 2 δευτερολέπτων σε πραγματικό χρόνο (real-time).
        yield return new WaitForSecondsRealtime(2f);

        // Απενεργοποίηση του CharacterController για την ασφαλή μεταφορά του παίκτη στην αρχική θέση (αποφυγή συγκρούσεων).
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        transform.position = startingPosition;

        if (controller != null) controller.enabled = true;

        // Επαναφορά όλων των εχθρικών πρακτόρων (AI) στις αντίστοιχες αρχικές τους θέσεις.
        EnemyCopAI[] allEnemies = FindObjectsByType<EnemyCopAI>(FindObjectsSortMode.None);
        foreach (EnemyCopAI enemy in allEnemies)
        {
            enemy.ResetEnemyPosition();
        }

        // Επανενεργοποίηση όλων των συλλέξιμων αντικειμένων υγείας στον χάρτη.
        HealthItem[] allFruits = FindObjectsByType<HealthItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (HealthItem fruit in allFruits)
        {
            fruit.gameObject.SetActive(true);
        }

        // Μηδενισμός των δυνάμεων φυσικής (εφόσον υφίστανται) για την αποφυγή ανεπιθύμητης κίνησης κατά το respawn.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Επαναφορά της ροής του χρόνου.
        Time.timeScale = 1;

        // Πλήρης αναπλήρωση της υγείας του παίκτη.
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Επαναφορά του προεπιλεγμένου μουσικού θέματος.
        if (MusicManager.instance != null)
        {
            MusicManager.instance.ResetMusicAfterRespawn();
        }

        // Απόκρυψη του μηνύματος αποτυχίας.
        if (centerMessageText != null)
        {
            centerMessageText.text = "";
        }
    }

    IEnumerator GameOverFadeRoutine()
    {
        // Ταυτόχρονη αναπαραγωγή ηχητικών εφέ ολοκληρωτικής ήττας (Game Over).
        if (playerAudio != null)
        {
            if (sfxPlayerDeath != null) playerAudio.PlayOneShot(sfxPlayerDeath);
            if (sfxGameOver != null) playerAudio.PlayOneShot(sfxGameOver);
        }

        // Σταδιακή εμφάνιση της οθόνης Game Over μέσω γραμμικής παρεμβολής της αδιαφάνειας (Alpha).
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            CanvasGroup canvasGroup = gameOverScreen.GetComponent<CanvasGroup>();
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

        yield return new WaitForSecondsRealtime(1.5f);

        Time.timeScale = 1;

        // Επαναφορά των διαθέσιμων ζωών στην προεπιλεγμένη τιμή για το επόμενο παιχνίδι.
        PlayerPrefs.SetInt("SavedLives", 3);

        // Φόρτωση της αρχικής σκηνής (Main Menu).
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void UpdateHealthUI()
    {
        // Ενημέρωση του κυκλικού γραφήματος υγείας (Pie Image) και χρωματική προσαρμογή βάσει του τρέχοντος ποσοστού.
        if (healthPieImage != null)
        {
            float fillVal = (float)currentHealth / maxHealth;
            healthPieImage.fillAmount = fillVal;

            if (fillVal > 0.75f) healthPieImage.color = Color.blue;
            else if (fillVal > 0.50f) healthPieImage.color = Color.green;
            else if (fillVal > 0.25f) healthPieImage.color = Color.yellow;
            else healthPieImage.color = Color.red;
        }

        // Ενημέρωση του κειμένου υπολειπόμενων ζωών.
        if (livesText != null)
        {
            livesText.text = "Lives: " + totalLives;
        }
    }
}