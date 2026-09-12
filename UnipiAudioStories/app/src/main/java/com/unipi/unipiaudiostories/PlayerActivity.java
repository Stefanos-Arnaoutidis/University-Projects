package com.unipi.unipiaudiostories;

import android.content.SharedPreferences;
import android.os.Bundle;
import android.speech.tts.TextToSpeech;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.bumptech.glide.Glide;

import java.util.Locale;

public class PlayerActivity extends AppCompatActivity {

    private TextView tvTitle, tvContent;
    private ImageView ivImage;
    private Button btnPlay, btnStop;

    private TextToSpeech textToSpeech;
    private String storyContent;
    private int storyId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_player);

        // Σύνδεση με τα γραφικά στοιχεία του layout
        tvTitle = findViewById(R.id.playerTitle);
        tvContent = findViewById(R.id.playerContent);
        ivImage = findViewById(R.id.playerImage);
        btnPlay = findViewById(R.id.btnPlay);
        btnStop = findViewById(R.id.btnStop);

        // Ανάκτηση δεδομένων που στάλθηκαν από το προηγούμενο Activity
        String title = getIntent().getStringExtra("story_title");
        storyContent = getIntent().getStringExtra("story_content");
        String imageUrl = getIntent().getStringExtra("story_image");
        storyId = getIntent().getIntExtra("story_id", -1);

        // Ενημέρωση του UI με τα δεδομένα της ιστορίας
        tvTitle.setText(title);
        tvContent.setText(storyContent);

        // Φόρτωση και εμφάνιση της εικόνας χρησιμοποιώντας τη βιβλιοθήκη Glide
        Glide.with(this).load(imageUrl).into(ivImage);

        // Αρχικοποίηση της μηχανής TextToSpeech (TTS)
        textToSpeech = new TextToSpeech(getApplicationContext(), status -> {
            if (status == TextToSpeech.SUCCESS) {
                // Ορίζουμε τη γλώσσα στα Αγγλικά (US) γιατί το κείμενο είναι στα αγγλικό
                int result = textToSpeech.setLanguage(Locale.US);

                if (result == TextToSpeech.LANG_MISSING_DATA || result == TextToSpeech.LANG_NOT_SUPPORTED) {
                    Toast.makeText(this, getString(R.string.msg_lang_error), Toast.LENGTH_SHORT).show();
                }
            } else {
                Toast.makeText(this, getString(R.string.msg_tts_error), Toast.LENGTH_SHORT).show();
            }
        });

        // Ορισμός λειτουργίας για το κουμπί Αναπαραγωγής (Play)
        btnPlay.setOnClickListener(v -> {
            speakStory();
            updateStatistics(storyId); // Καταγραφή της ανάγνωσης στα στατιστικά
        });

        // Ορισμός λειτουργίας για το κουμπί Διακοπής (Stop)
        btnStop.setOnClickListener(v -> {
            if (textToSpeech != null) {
                textToSpeech.stop();
            }
        });
    }

    // Ξεκινάει την ανάγνωση του κειμένου της ιστορίας
    private void speakStory() {
        if (textToSpeech != null && storyContent != null) {
            // QUEUE_FLUSH: Σταματάει ό,τι έλεγε πριν και ξεκινάει το νέο κείμενο
            textToSpeech.speak(storyContent, TextToSpeech.QUEUE_FLUSH, null, null);
        }
    }

    // Ενημερώνει τα στατιστικά χρήσης στο SharedPreferences
    private void updateStatistics(int id) {
        if (id == -1) return;

        SharedPreferences prefs = getSharedPreferences("UserStats", MODE_PRIVATE);

        // Ανάκτηση του τρέχοντος μετρητή για τη συγκεκριμένη ιστορία
        int currentCount = prefs.getInt("count_" + id, 0);

        // Αποθήκευση της νέας τιμής (τρέχουσα + 1)
        SharedPreferences.Editor editor = prefs.edit();
        editor.putInt("count_" + id, currentCount + 1);
        editor.apply();

        Toast.makeText(this, getString(R.string.msg_story_started), Toast.LENGTH_SHORT).show();
    }

    // Απελευθέρωση των πόρων του TTS όταν κλείνει το Activity
    @Override
    protected void onDestroy() {
        if (textToSpeech != null) {
            textToSpeech.stop();
            textToSpeech.shutdown();
        }
        super.onDestroy();
    }
}