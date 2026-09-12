package com.unipi.unipicityvibe;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.List;

public class EventsAdapter extends RecyclerView.Adapter<EventsAdapter.EventViewHolder> {

    private Context context;
    private List<Event> eventList;
    private OnEventClickListener listener;

    // Interface για τη διαχείριση των κλικ στα αντικείμενα της λίστας
    public interface OnEventClickListener {
        void onEventClick(Event event);
    }

    // Constructor: Αρχικοποίηση του Adapter με τα δεδομένα και τον Listener
    public EventsAdapter(Context context, List<Event> eventList, OnEventClickListener listener) {
        this.context = context;
        this.eventList = eventList;
        this.listener = listener;
    }

    // Δημιουργία του ViewHolder και φόρτωση του Layout
    @NonNull
    @Override
    public EventViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_event, parent, false);
        return new EventViewHolder(view);
    }

    // Σύνδεση των δεδομένων της εκδήλωσης με τα γραφικά στοιχεία
    @Override
    public void onBindViewHolder(@NonNull EventViewHolder holder, int position) {
        Event currentEvent = eventList.get(position);

        // Ανάθεση τιμών στα TextViews
        holder.tvTitle.setText(currentEvent.getTitle());
        holder.tvDate.setText(currentEvent.getDate());
        holder.tvLocation.setText(currentEvent.getLocationName());

        // Μορφοποίηση και εμφάνιση της τιμής (π.χ. 15.00 €)
        holder.tvPrice.setText(String.format("%.2f €", currentEvent.getPrice()));

        // Διαχείριση κλικ στην κάρτα της εκδήλωσης
        holder.itemView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                listener.onEventClick(currentEvent);
            }
        });
    }

    // Επιστροφή του πλήθους των αντικειμένων στη λίστα
    @Override
    public int getItemCount() {
        return eventList.size();
    }

    // Εσωτερική κλάση που κρατά τις αναφορές στα Views του layout
    public class EventViewHolder extends RecyclerView.ViewHolder {
        TextView tvTitle, tvDate, tvLocation, tvPrice;

        public EventViewHolder(@NonNull View itemView) {
            super(itemView);
            // Σύνδεση των μεταβλητών με τα IDs από το XML
            tvTitle = itemView.findViewById(R.id.tvTitle);
            tvDate = itemView.findViewById(R.id.tvDate);
            tvLocation = itemView.findViewById(R.id.tvLocation);
            tvPrice = itemView.findViewById(R.id.tvPrice);
        }
    }
}