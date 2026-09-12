package com.unipi.unipicityvibe;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.app.NotificationCompat;
import androidx.core.app.NotificationManagerCompat;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.Manifest;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.location.Location;
import android.os.Build;
import android.os.Bundle;
import android.speech.RecognizerIntent;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.Toast;

import com.google.android.gms.location.FusedLocationProviderClient;
import com.google.android.gms.location.LocationServices;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.firebase.firestore.DocumentSnapshot;
import com.google.firebase.firestore.FirebaseFirestore;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private EventsAdapter adapter;
    private List<Event> eventList;
    private ProgressBar progressBar;
    private FirebaseFirestore db;

    // Μεταβλητές για GPS και Άδειες
    private FusedLocationProviderClient fusedLocationClient;
    private static final int PERMISSION_REQUEST_CODE = 100;

    // Launcher για τις φωνητικές εντολές
    private ActivityResultLauncher<Intent> voiceLauncher;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Ρύθμιση του Voice Launcher για τη λήψη αποτελεσμάτων ομιλίας
        voiceLauncher = registerForActivityResult(
                new ActivityResultContracts.StartActivityForResult(),
                result -> {
                    if (result.getResultCode() == RESULT_OK && result.getData() != null) {
                        ArrayList<String> matches = result.getData().getStringArrayListExtra(RecognizerIntent.EXTRA_RESULTS);
                        if (matches != null && !matches.isEmpty()) {
                            // Μετατροπή σε πεζά για ευκολότερο έλεγχο
                            String command = matches.get(0).toLowerCase();
                            handleVoiceCommand(command);
                        }
                    }
                }
        );

        // Αρχικοποίηση Notification Channel και Location Client
        createNotificationChannel();
        fusedLocationClient = LocationServices.getFusedLocationProviderClient(this);

        // Αρχικοποίηση Firebase και γραφικών στοιχείων (Views)
        db = FirebaseFirestore.getInstance();
        recyclerView = findViewById(R.id.recyclerViewEvents);
        progressBar = findViewById(R.id.progressBar);

        // Κουμπί μετάβασης στις ρυθμίσεις
        android.widget.ImageButton btnSettings = findViewById(R.id.btnSettings);
        btnSettings.setOnClickListener(v -> {
            startActivity(new Intent(MainActivity.this, SettingsActivity.class));
        });

        // Κουμπί έναρξης φωνητικής αναγνώρισης
        FloatingActionButton fabVoice = findViewById(R.id.fabVoice);
        fabVoice.setOnClickListener(v -> startVoiceRecognition());

        // Ρύθμιση λίστας (RecyclerView) και Adapter
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        eventList = new ArrayList<>();

        adapter = new EventsAdapter(this, eventList, event -> {
            // Μετάβαση στις λεπτομέρειες της επιλεγμένης εκδήλωσης
            Intent intent = new Intent(MainActivity.this, DetailsActivity.class);
            intent.putExtra("eventId", event.getDocumentId());
            intent.putExtra("title", event.getTitle());
            intent.putExtra("description", event.getDescription());
            intent.putExtra("date", event.getDate());
            intent.putExtra("location", event.getLocationName());
            intent.putExtra("price", event.getPrice());
            startActivity(intent);
        });

        recyclerView.setAdapter(adapter);

        // Έλεγχος αδειών και έναρξη φόρτωσης
        checkPermissionsAndLoadData();
    }

    // Έναρξη της διαδικασίας αναγνώρισης φωνής
    private void startVoiceRecognition() {
        Intent intent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
        intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
        intent.putExtra(RecognizerIntent.EXTRA_PROMPT, "Πείτε: 'Settings' ή 'Exit'");

        try {
            voiceLauncher.launch(intent);
        } catch (Exception e) {
            Toast.makeText(this, "Η φωνητική αναγνώριση δεν υποστηρίζεται", Toast.LENGTH_SHORT).show();
        }
    }

    // Διαχείριση των εντολών που επιστρέφει η φωνητική αναγνώριση
    private void handleVoiceCommand(String command) {
        Toast.makeText(this, "Εντολή: " + command, Toast.LENGTH_SHORT).show();

        if (command.contains("settings") || command.contains("ρυθμίσεις") || command.contains("setari")) {
            startActivity(new Intent(MainActivity.this, SettingsActivity.class));
        }
        else if (command.contains("exit") || command.contains("έξοδος") || command.contains("iesire")) {
            finishAffinity(); // Κλείνει τελείως την εφαρμογή
        }
        else {
            Toast.makeText(this, "Άγνωστη εντολή. Δοκιμάστε 'Settings' ή 'Exit'", Toast.LENGTH_SHORT).show();
        }
    }

    // Έλεγχος απαραίτητων αδειών (GPS, Notifications)
    private void checkPermissionsAndLoadData() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED ||
                ContextCompat.checkSelfPermission(this, Manifest.permission.POST_NOTIFICATIONS) != PackageManager.PERMISSION_GRANTED) {

            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.ACCESS_FINE_LOCATION, Manifest.permission.POST_NOTIFICATIONS},
                    PERMISSION_REQUEST_CODE);
        } else {
            loadEventsFromFirebase();
        }
    }

    // Callback για το αποτέλεσμα του αιτήματος αδειών
    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode == PERMISSION_REQUEST_CODE) {
            loadEventsFromFirebase();
        }
    }

    // Ανάκτηση εκδηλώσεων από το Firestore
    private void loadEventsFromFirebase() {
        progressBar.setVisibility(View.VISIBLE);

        db.collection("events").get()
                .addOnSuccessListener(queryDocumentSnapshots -> {
                    progressBar.setVisibility(View.GONE);
                    eventList.clear();

                    if (!queryDocumentSnapshots.isEmpty()) {
                        for (DocumentSnapshot snapshot : queryDocumentSnapshots) {
                            Event event = snapshot.toObject(Event.class);
                            if (event != null) {
                                event.setDocumentId(snapshot.getId());
                                eventList.add(event);
                            }
                        }
                        adapter.notifyDataSetChanged();

                        // Αφού φορτώσουν τα δεδομένα, ελέγχουμε τη θέση του χρήστη
                        checkUserLocation();
                    } else {
                        Toast.makeText(MainActivity.this, getString(R.string.events_empty), Toast.LENGTH_SHORT).show();
                    }
                })
                .addOnFailureListener(e -> {
                    progressBar.setVisibility(View.GONE);
                    Toast.makeText(MainActivity.this, "Error: " + e.getMessage(), Toast.LENGTH_LONG).show();
                });
    }

    // Εύρεση της τελευταίας γνωστής τοποθεσίας του χρήστη
    private void checkUserLocation() {
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            return;
        }

        fusedLocationClient.getLastLocation()
                .addOnSuccessListener(this, location -> {
                    if (location != null) {
                        for (Event event : eventList) {
                            checkDistance(location, event);
                        }
                    }
                });
    }

    // Υπολογισμός απόστασης και έλεγχος αν είναι < 200μ
    private void checkDistance(Location userLocation, Event event) {
        if (event.getLatitude() == 0 && event.getLongitude() == 0) return;

        float[] results = new float[1];
        Location.distanceBetween(
                userLocation.getLatitude(), userLocation.getLongitude(),
                event.getLatitude(), event.getLongitude(),
                results);

        float distanceInMeters = results[0];

        // Αν ο χρήστης είναι κοντά, στέλνουμε ειδοποίηση
        if (distanceInMeters < 200) {
            sendNotification(event);
        }
    }

    // Αποστολή ειδοποίησης (Notification) στον χρήστη
    private void sendNotification(Event event) {
        // Έλεγχος άδειας για Android 13+
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
            if (ActivityCompat.checkSelfPermission(this, Manifest.permission.POST_NOTIFICATIONS) != PackageManager.PERMISSION_GRANTED) {
                return;
            }
        }
        Intent intent = new Intent(this, DetailsActivity.class);
        intent.putExtra("eventId", event.getDocumentId());
        intent.putExtra("title", event.getTitle());
        intent.putExtra("description", event.getDescription());
        intent.putExtra("date", event.getDate());
        intent.putExtra("location", event.getLocationName());
        intent.putExtra("price", event.getPrice());

        android.app.PendingIntent pendingIntent = android.app.PendingIntent.getActivity(
                this,
                event.getTitle().hashCode(),
                intent,
                android.app.PendingIntent.FLAG_UPDATE_CURRENT | android.app.PendingIntent.FLAG_IMMUTABLE
        );

        String contentText = getString(R.string.notification_body_start) + " '" + event.getTitle() + "' " + getString(R.string.notification_body_end);

        NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "EVENT_CHANNEL_ID")
                .setSmallIcon(android.R.drawable.ic_dialog_map)
                .setContentTitle(getString(R.string.notification_title))
                .setContentText(contentText)
                .setPriority(NotificationCompat.PRIORITY_HIGH)
                .setAutoCancel(true)
                .setContentIntent(pendingIntent);

        NotificationManagerCompat notificationManager = NotificationManagerCompat.from(this);
        notificationManager.notify(event.getTitle().hashCode(), builder.build());
    }

    // Δημιουργία καναλιού ειδοποιήσεων
    private void createNotificationChannel() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            CharSequence name = "Event Alerts";
            String description = "Notifications for nearby events";
            int importance = NotificationManager.IMPORTANCE_HIGH;
            NotificationChannel channel = new NotificationChannel("EVENT_CHANNEL_ID", name, importance);
            channel.setDescription(description);

            NotificationManager notificationManager = getSystemService(NotificationManager.class);
            notificationManager.createNotificationChannel(channel);
        }
    }
}