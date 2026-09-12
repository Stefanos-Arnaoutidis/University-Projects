package com.unipi.unipiaudiostories;

import android.content.Context;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;

import java.util.List;

public class StoriesAdapter extends RecyclerView.Adapter<StoriesAdapter.StoryViewHolder> {

    private Context context;
    private List<Story> storyList;

    // Κατασκευαστής: Αρχικοποιεί τον Adapter με το context και τη λίστα δεδομένων
    public StoriesAdapter(Context context, List<Story> storyList) {
        this.context = context;
        this.storyList = storyList;
    }

    // Δημιουργεί το View για κάθε αντικείμενο της λίστας
    @NonNull
    @Override
    public StoryViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_story, parent, false);
        return new StoryViewHolder(view);
    }

    // Συνδέει τα δεδομένα της ιστορίας με τα γραφικά στοιχεία (Views)
    @Override
    public void onBindViewHolder(@NonNull StoryViewHolder holder, int position) {
        // Ανάκτηση του τρέχοντος αντικειμένου Story με βάση τη θέση
        Story story = storyList.get(position);

        // Ενημέρωση των πεδίων κειμένου (Τίτλος, Συγγραφέας)
        holder.title.setText(story.getTitle());
        holder.author.setText(story.getAuthor());

        // Φόρτωση εικόνας από URL χρησιμοποιώντας τη βιβλιοθήκη Glide
        Glide.with(context)
                .load(story.getImage())
                .into(holder.image);

        // Διαχείριση του συμβάντος κλικ στο αντικείμενο
        holder.itemView.setOnClickListener(v -> {
            Intent intent = new Intent(context, PlayerActivity.class);

            // Πέρασμα των δεδομένων της ιστορίας στο PlayerActivity
            intent.putExtra("story_title", story.getTitle());
            intent.putExtra("story_content", story.getContent());
            intent.putExtra("story_image", story.getImage());
            intent.putExtra("story_id", story.getId()); // Χρήσιμο για τα στατιστικά

            context.startActivity(intent);
        });
    }

    // Επιστρέφει το συνολικό πλήθος των αντικειμένων στη λίστα
    @Override
    public int getItemCount() {
        return storyList.size();
    }

    // Εσωτερική κλάση ViewHolder για τη διαχείριση των Views κάθε στοιχείου
    public static class StoryViewHolder extends RecyclerView.ViewHolder {
        TextView title, author;
        ImageView image;

        public StoryViewHolder(@NonNull View itemView) {
            super(itemView);
            // Σύνδεση με τα στοιχεία του layout item_story.xml
            title = itemView.findViewById(R.id.storyTitle);
            author = itemView.findViewById(R.id.storyAuthor);
            image = itemView.findViewById(R.id.storyImage);
        }
    }
}