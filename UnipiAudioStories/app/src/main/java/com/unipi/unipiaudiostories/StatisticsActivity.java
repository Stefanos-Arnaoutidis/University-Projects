package com.unipi.unipiaudiostories;

import android.content.SharedPreferences;
import android.os.Bundle;
import android.widget.TextView;
import androidx.appcompat.app.AppCompatActivity;

public class StatisticsActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_statistics);

        // Σύνδεση με τα γραφικά στοιχεία του layout (Views)
        TextView tvStatsList = findViewById(R.id.tvStatsList);
        TextView tvFavorite = findViewById(R.id.tvFavorite);

        // Ανάκτηση των αποθηκευμένων στατιστικών από SharedPreferences
        SharedPreferences prefs = getSharedPreferences("UserStats", MODE_PRIVATE);

        // Αρχικοποίηση μεταβλητών για τον υπολογισμό των στατιστικών
        StringBuilder statsBuilder = new StringBuilder();
        String favoriteStory = "None";
        int maxCount = 0;

        // Πίνακας με τους τίτλους των ιστοριών
        String[] storyTitles = {
                "", // Θέση 0 κενή
                "The Three Little Pigs",      // ID 1
                "Little Red Riding Hood",     // ID 2
                "The Wolf and the Seven Goats", // ID 3
                "The Boy Who Cried Wolf",     // ID 4
                "The Fox and the Crow"        // ID 5
        };

        // Επανάληψη για κάθε ιστορία για τη συλλογή δεδομένων
        for (int i = 1; i <= 5; i++) {
            // Ανάκτηση πλήθους αναγνώσεων για το συγκεκριμένο ID
            int count = prefs.getInt("count_" + i, 0);

            // Διαμόρφωση του κειμένου της λίστας αποτελεσμάτων
            if (i < storyTitles.length) {
                statsBuilder.append(storyTitles[i])
                        .append(": ")
                        .append(count)
                        .append(" ")
                        .append(getString(R.string.word_times)) // Χρήση πολυγλωσσίας
                        .append("\n");
            }

            // Έλεγχος για την εύρεση της αγαπημένης ιστορίας (αυτή με τις περισσότερες προβολές)
            if (count > maxCount) {
                maxCount = count;
                if (i < storyTitles.length) {
                    favoriteStory = storyTitles[i];
                }
            }
        }

        // Εμφάνιση της λίστας στατιστικών στην οθόνη
        tvStatsList.setText(statsBuilder.toString());

        // Εμφάνιση της αγαπημένης ιστορίας ή μηνύματος αν δεν υπάρχουν δεδομένα
        if (maxCount > 0) {
            tvFavorite.setText(getString(R.string.label_favorite) + "\n" + favoriteStory + " (" + maxCount + ")");
        } else {
            tvFavorite.setText(getString(R.string.msg_no_stories));
        }
    }
}