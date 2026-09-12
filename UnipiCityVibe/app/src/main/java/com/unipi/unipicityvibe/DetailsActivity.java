package com.unipi.unipicityvibe;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.speech.tts.TextToSpeech;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.gms.tasks.OnSuccessListener;
import com.google.firebase.Timestamp;
import com.google.firebase.firestore.DocumentReference;
import com.google.firebase.firestore.FirebaseFirestore;

import java.util.Date;
import java.util.HashMap;
import java.util.Locale;
import java.util.Map;

public class DetailsActivity extends AppCompatActivity implements TextToSpeech.OnInitListener {

    TextView tvTitle, tvDate, tvLocation, tvDesc, tvPrice;
    EditText etCustomerName;
    Button btnBook;
    ImageButton btnSpeak;

    FirebaseFirestore db;
    String eventId;
    TextToSpeech tts;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_details);

        // Αρχικοποίηση Text-to-Speech (TTS)
        tts = new TextToSpeech(this, this);

        // Αρχικοποίηση Firestore
        db = FirebaseFirestore.getInstance();

        // Σύνδεση με τα στοιχεία του Layout
        tvTitle = findViewById(R.id.tvDetailTitle);
        tvDate = findViewById(R.id.tvDetailDate);
        tvLocation = findViewById(R.id.tvDetailLocation);
        tvDesc = findViewById(R.id.tvDetailDescription);
        tvPrice = findViewById(R.id.tvDetailPrice);
        etCustomerName = findViewById(R.id.etCustomerName);
        btnBook = findViewById(R.id.btnBook);
        btnSpeak = findViewById(R.id.btnSpeak);

        // Ανάκτηση αποθηκευμένων ρυθμίσεων (Shared Preferences)
        android.content.SharedPreferences preferences = getSharedPreferences("UnipiCityVibePrefs", MODE_PRIVATE);

        // Αυτόματη συμπλήρωση ονόματος αν υπάρχει
        String savedName = preferences.getString("fullname", "");
        if (!savedName.isEmpty()) etCustomerName.setText(savedName);

        // Εφαρμογή μεγέθους γραμματοσειράς
        String fontSize = preferences.getString("fontsize", "normal");
        if (fontSize.equals("small")) tvDesc.setTextSize(14);
        else if (fontSize.equals("large")) tvDesc.setTextSize(24);
        else tvDesc.setTextSize(18);

        // Λήψη δεδομένων από το Intent
        eventId = getIntent().getStringExtra("eventId");
        String title = getIntent().getStringExtra("title");
        String description = getIntent().getStringExtra("description");
        String date = getIntent().getStringExtra("date");
        String location = getIntent().getStringExtra("location");
        double price = getIntent().getDoubleExtra("price", 0.0);

        // Εμφάνιση δεδομένων στα πεδία
        tvTitle.setText(title);
        tvDesc.setText(description);
        tvDate.setText(date);
        tvLocation.setText(location);

        // Εμφάνιση τιμής με υποστήριξη πολυγλωσσικότητας
        tvPrice.setText(getString(R.string.details_price) + " " + price + " €");

        // Λειτουργία κουμπιού Εκφώνησης (Text-to-Speech)
        btnSpeak.setOnClickListener(v -> {
            if (description != null && !description.isEmpty()) {
                tts.speak(description, TextToSpeech.QUEUE_FLUSH, null, null);
            }
        });

        // Λειτουργία κουμπιού Κράτησης
        btnBook.setOnClickListener(v -> {
            String customerName = etCustomerName.getText().toString().trim();

            // Έλεγχος εγκυρότητας ονόματος
            if (TextUtils.isEmpty(customerName)) {
                etCustomerName.setError(getString(R.string.error_empty_name));
                return;
            }
            saveBooking(customerName, title);
        });
    }

    // Αποθήκευση κράτησης στη βάση δεδομένων
    private void saveBooking(String customerName, String eventTitle) {
        // Δημιουργία αντικειμένου κράτησης
        Map<String, Object> booking = new HashMap<>();
        booking.put("customerName", customerName);
        booking.put("eventId", eventId);
        booking.put("eventTitle", eventTitle);
        booking.put("timestamp", new Timestamp(new Date()));

        // Ενημέρωση UI κατά την αποστολή
        btnBook.setEnabled(false);
        btnBook.setText(getString(R.string.details_booking_progress));

        // Αποστολή στη συλλογή "bookings"
        db.collection("bookings")
                .add(booking)
                .addOnSuccessListener(documentReference -> {
                    Toast.makeText(DetailsActivity.this, getString(R.string.details_success_toast), Toast.LENGTH_LONG).show();
                    finish(); // Επιστροφή στην προηγούμενη οθόνη
                })
                .addOnFailureListener(e -> {
                    Toast.makeText(DetailsActivity.this, "Error: " + e.getMessage(), Toast.LENGTH_SHORT).show();
                    btnBook.setEnabled(true);
                    btnBook.setText(getString(R.string.details_book_btn));
                });
    }

    // Καλείται όταν η μηχανή TTS είναι έτοιμη
    @Override
    public void onInit(int status) {
        if (status == TextToSpeech.SUCCESS) {
            // Ορισμός γλώσσας με βάση τη συσκευή (fallback σε US)
            int result = tts.setLanguage(Locale.getDefault());

            if (result == TextToSpeech.LANG_MISSING_DATA || result == TextToSpeech.LANG_NOT_SUPPORTED) {
                Log.e("TTS", "Language not supported, falling back to English");
                tts.setLanguage(Locale.US);
            }
        } else {
            Log.e("TTS", "Initialization failed");
        }
    }

    // Απελευθέρωση πόρων όταν κλείνει η οθόνη
    @Override
    protected void onDestroy() {
        if (tts != null) {
            tts.stop();
            tts.shutdown();
        }
        super.onDestroy();
    }
}