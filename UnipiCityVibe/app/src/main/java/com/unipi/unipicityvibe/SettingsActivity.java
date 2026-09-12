package com.unipi.unipicityvibe;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.app.AppCompatDelegate;

import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.Toast;

import com.google.android.material.switchmaterial.SwitchMaterial;

public class SettingsActivity extends AppCompatActivity {

    EditText etName;
    RadioGroup radioGroupFont;
    RadioButton rbSmall, rbNormal, rbLarge;
    SwitchMaterial switchDarkMode;
    Button btnSave;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);

        // Αρχικοποίηση γραφικών στοιχείων (Views)
        etName = findViewById(R.id.etSettingsName);
        radioGroupFont = findViewById(R.id.radioGroupFont);
        rbSmall = findViewById(R.id.rbSmall);
        rbNormal = findViewById(R.id.rbNormal);
        rbLarge = findViewById(R.id.rbLarge);
        switchDarkMode = findViewById(R.id.switchDarkMode);
        btnSave = findViewById(R.id.btnSaveSettings);

        // Ανάκτηση ρυθμίσεων από τα SharedPreferences
        SharedPreferences preferences = getSharedPreferences("UnipiCityVibePrefs", MODE_PRIVATE);

        // Εμφάνιση αποθηκευμένου ονόματος
        etName.setText(preferences.getString("fullname", ""));

        // Εμφάνιση αποθηκευμένης επιλογής γραμματοσειράς
        String fontSize = preferences.getString("fontsize", "normal");
        if (fontSize.equals("small")) {
            rbSmall.setChecked(true);
        } else if (fontSize.equals("large")) {
            rbLarge.setChecked(true);
        } else {
            rbNormal.setChecked(true);
        }

        // Εμφάνιση αποθηκευμένης επιλογής Dark Mode
        boolean isDarkMode = preferences.getBoolean("darkmode", false);
        switchDarkMode.setChecked(isDarkMode);

        // Λειτουργία κουμπιού αποθήκευσης
        btnSave.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String newName = etName.getText().toString().trim();

                // Έλεγχος εγκυρότητας ονόματος
                if (newName.isEmpty()) {
                    etName.setError(getString(R.string.error_empty_name));
                    return;
                }

                // Καθορισμός επιλεγμένης γραμματοσειράς
                String newFontSize = "normal";
                if (rbSmall.isChecked()) newFontSize = "small";
                if (rbLarge.isChecked()) newFontSize = "large";

                // Ανάκτηση επιθυμητής κατάστασης Dark Mode
                boolean wantDarkMode = switchDarkMode.isChecked();

                // Αποθήκευση αλλαγών στα SharedPreferences
                SharedPreferences.Editor editor = preferences.edit();
                editor.putString("fullname", newName);
                editor.putString("fontsize", newFontSize);
                editor.putBoolean("darkmode", wantDarkMode);
                editor.apply();

                // Eφαρμογή του θέματος (Dark/Light)
                if (wantDarkMode) {
                    AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES);
                } else {
                    AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO);
                }

                // Εμφάνιση μηνύματος επιτυχίας
                Toast.makeText(SettingsActivity.this, getString(R.string.settings_saved_toast), Toast.LENGTH_SHORT).show();

                finish(); // Κλείσιμο της οθόνης ρυθμίσεων
            }
        });
    }
}