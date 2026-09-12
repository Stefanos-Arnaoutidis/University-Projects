package com.unipi.unipicityvibe;

public class Event {

    // Mοναδικό ID του εγγράφου στη βάση
    private String documentId;

    // Βασικά στοιχεία της εκδήλωσης
    private String title;
    private String description;
    private String date;
    private String locationName;
    private double price;

    // Συντεταγμένες GPS για τον γεωεντοπισμό και τον υπολογισμό απόστασης
    private double latitude;  // Γεωγραφικό Πλάτος
    private double longitude; // Γεωγραφικό Μήκος

    // Kενός constructor για την αυτόματη σειριοποίηση της Firebase
    public Event() {}

    // Constructor για δημιουργία αντικειμένου με δεδομένα
    public Event(String title, String description, String date, String locationName, double price, double latitude, double longitude) {
        this.title = title;
        this.description = description;
        this.date = date;
        this.locationName = locationName;
        this.price = price;
        this.latitude = latitude;
        this.longitude = longitude;
    }

    // Getters & Setters

    // Διαχείριση του ID εγγράφου
    public String getDocumentId() { return documentId; }
    public void setDocumentId(String documentId) { this.documentId = documentId; }

    // Διαχείριση Τίτλου
    public String getTitle() { return title; }
    public void setTitle(String title) { this.title = title; }

    // Διαχείριση Περιγραφής
    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    // Διαχείριση Ημερομηνίας
    public String getDate() { return date; }
    public void setDate(String date) { this.date = date; }

    // Διαχείριση Ονόματος Τοποθεσίας
    public String getLocationName() { return locationName; }
    public void setLocationName(String locationName) { this.locationName = locationName; }

    // Διαχείριση Τιμής
    public double getPrice() { return price; }
    public void setPrice(double price) { this.price = price; }

    // Διαχείριση Συντεταγμένων (GPS)
    public double getLatitude() { return latitude; }
    public void setLatitude(double latitude) { this.latitude = latitude; }

    public double getLongitude() { return longitude; }
    public void setLongitude(double longitude) { this.longitude = longitude; }
}