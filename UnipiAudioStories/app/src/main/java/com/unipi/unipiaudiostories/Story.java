package com.unipi.unipiaudiostories;

public class Story {
    // Ιδιότητες της κλάσης
    private int id;
    private String title;
    private String author;
    private String content;
    private String image;

    // Κενός κατασκευαστής
    public Story() {
    }

    // Κατασκευαστής
    public Story(int id, String title, String author, String content, String image) {
        this.id = id;
        this.title = title;
        this.author = author;
        this.content = content;
        this.image = image;
    }

    // Getters και Setters

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getAuthor() {
        return author;
    }

    public void setAuthor(String author) {
        this.author = author;
    }

    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }

    public String getImage() {
        return image;
    }

    public void setImage(String image) {
        this.image = image;
    }
}