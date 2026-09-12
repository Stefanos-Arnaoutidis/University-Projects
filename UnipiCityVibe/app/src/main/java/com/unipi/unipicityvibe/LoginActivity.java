package com.unipi.unipicityvibe;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.app.AppCompatDelegate;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

public class LoginActivity extends AppCompatActivity {

    EditText etFullName;
    Button btnEnter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // Ανάκτηση προτιμήσεων για το Dark Mode πριν τη φόρτωση του Layout
        SharedPreferences preferences = getSharedPreferences("UnipiCityVibePrefs", MODE_PRIVATE);
        boolean isDarkMode = preferences.getBoolean("darkmode", false);

        // Εφαρμογή του επιλεγμένου θέματος (Dark/Light)
        if (isDarkMode) {
            AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES);
        } else {
            AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO);
        }

        setContentView(R.layout.activity_login);

        etFullName = findViewById(R.id.etFullName);
        btnEnter = findViewById(R.id.btnEnter);

        // Έλεγχος αν υπάρχει ήδη αποθηκευμένο όνομα χρήστη
        String savedName = preferences.getString("fullname", "");

        if (!savedName.isEmpty()) {
            etFullName.setText(savedName);
        }

        // Λειτουργία κουμπιού εισόδου
        btnEnter.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String fullName = etFullName.getText().toString().trim();

                // Έλεγχος εγκυρότητας εισόδου
                if (TextUtils.isEmpty(fullName)) {
                    etFullName.setError(getString(R.string.error_empty_name));
                    return;
                }

                // Αποθήκευση του ονόματος στα SharedPreferences
                SharedPreferences.Editor editor = preferences.edit();
                editor.putString("fullname", fullName);
                editor.apply();

                // Μετάβαση στην Κεντρική Οθόνη
                Intent intent = new Intent(LoginActivity.this, MainActivity.class);
                startActivity(intent);
                finish(); // Τερματισμός του Login Activity
            }
        });
    }
}