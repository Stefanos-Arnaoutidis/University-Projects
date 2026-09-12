package com.unipi.unipiaudiostories;

import android.content.Intent;
import android.os.Bundle;
import android.speech.RecognizerIntent;
import android.widget.Button;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class MainActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private StoriesAdapter adapter;
    private List<Story> storyList;
    private DatabaseReference databaseReference;

    // Launcher για τη διαχείριση του αποτελέσματος της φωνητικής εντολής
    private ActivityResultLauncher<Intent> speechLauncher;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Αρχικοποίηση του RecyclerView και ορισμός του LayoutManager
        recyclerView = findViewById(R.id.storiesRecyclerView);
        recyclerView.setHasFixedSize(true);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));

        // Δημιουργία της λίστας και σύνδεση με τον Adapter
        storyList = new ArrayList<>();
        adapter = new StoriesAdapter(this, storyList);
        recyclerView.setAdapter(adapter);

        // Σύνδεση με τη βάση δεδομένων Firebase (πίνακας "stories")
        databaseReference = FirebaseDatabase.getInstance().getReference("stories");

        // Ανάκτηση δεδομένων από τη βάση και παρακολούθηση για αλλαγές
        databaseReference.addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot snapshot) {
                // Καθαρισμός της λίστας πριν την ενημέρωση για αποφυγή διπλότυπων
                storyList.clear();

                // Προσθήκη κάθε ιστορίας που βρέθηκε στη βάση, μέσα στη λίστα
                for (DataSnapshot postSnapshot : snapshot.getChildren()) {
                    Story story = postSnapshot.getValue(Story.class);
                    storyList.add(story);
                }
                adapter.notifyDataSetChanged();
            }

            @Override
            public void onCancelled(@NonNull DatabaseError error) {
                Toast.makeText(MainActivity.this, "Error: " + error.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });

        // Ρύθμιση του κουμπιού για μετάβαση στην οθόνη Στατιστικών
        Button btnStats = findViewById(R.id.btnStats);
        btnStats.setOnClickListener(v -> openStatistics());

        // Εντοπισμός του κουμπιού μικροφώνου
        FloatingActionButton btnMic = findViewById(R.id.btnMic);

        // Προετοιμασία του Launcher για τη λήψη της φωνητικής εντολής
        speechLauncher = registerForActivityResult(
                new ActivityResultContracts.StartActivityForResult(),
                result -> {
                    if (result.getResultCode() == RESULT_OK && result.getData() != null) {
                        // Ανάκτηση των πιθανών φράσεων που αναγνωρίστηκαν
                        ArrayList<String> matches = result.getData().getStringArrayListExtra(RecognizerIntent.EXTRA_RESULTS);
                        if (matches != null && !matches.isEmpty()) {
                            // Παίρνουμε την πρώτη φράση και την κάνουμε πεζά γράμματα
                            String command = matches.get(0).toLowerCase();
                            checkCommand(command);
                        }
                    }
                }
        );

        // Λειτουργία κουμπιού μικροφώνου: Έναρξη αναγνώρισης φωνής
        btnMic.setOnClickListener(v -> {
            Intent intent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
            intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
            intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE, Locale.getDefault());
            intent.putExtra(RecognizerIntent.EXTRA_PROMPT, "Say 'Statistics' or 'Στατιστικά'");

            try {
                speechLauncher.launch(intent);
            } catch (Exception e) {
                Toast.makeText(this, "Speech not supported", Toast.LENGTH_SHORT).show();
            }
        });
    }

    // Έλεγχος της φωνητικής εντολής που έδωσε ο χρήστης
    private void checkCommand(String command) {
        // Ελέγχουμε αν η εντολή περιέχει λέξεις κλειδιά σε Αγγλικά, Ελληνικά ή Ρουμανικά
        if (command.contains("statistics") || command.contains("στατιστικά") || command.contains("statistici")) {
            openStatistics();
            Toast.makeText(this, "Command Recognized!", Toast.LENGTH_SHORT).show();
        } else {
            Toast.makeText(this, "Command not understood: " + command, Toast.LENGTH_SHORT).show();
        }
    }

    // Μέθοδος για άνοιγμα του StatisticsActivity
    private void openStatistics() {
        Intent intent = new Intent(MainActivity.this, StatisticsActivity.class);
        startActivity(intent);
    }
}