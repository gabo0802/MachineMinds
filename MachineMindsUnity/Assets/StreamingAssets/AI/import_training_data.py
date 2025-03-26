import firebase_admin
from firebase_admin import credentials
from firebase_admin import firestore
import csv
import os
import datetime

def fetch_firestore_data_to_csv(service_account_path, output_path=None):
    # Initialize Firebase with your service account credentials
    cred = credentials.Certificate(service_account_path)
    firebase_admin.initialize_app(cred)
    
    # Initialize Firestore client
    db = firestore.client()
    
    # Reference to your specific collection
    collection_ref = db.collection('ai-training-data')
    
    # Get all documents from the collection
    docs = collection_ref.get()
    
    # Process the data
    training_data = []
    all_fields = set(['id'])  # Start with document ID as a field
    
    # First pass: collect all possible fields across documents
    for doc in docs:
        data = doc.to_dict()
        all_fields.update(data.keys())
    
    # Convert set to sorted list for consistent column order
    all_fields = sorted(list(all_fields))
    
    # Second pass: create data rows with all fields
    for doc in docs:
        data = doc.to_dict()
        data['id'] = doc.id  # Add document ID to the data
        
        # Create a row with all fields (using None for missing fields)
        row = {field: data.get(field, None) for field in all_fields}
        training_data.append(row)
    
    # Generate default output path if not provided
    if not output_path:
        timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
        output_path = f"./Data/firestore_ai_training_data_{timestamp}.csv"
    
    # Write to CSV
    with open(output_path, 'w', newline='', encoding='utf-8') as csvfile:
        writer = csv.DictWriter(csvfile, fieldnames=all_fields)
        writer.writeheader()
        writer.writerows(training_data)
    
    print(f"Retrieved {len(training_data)} documents from the ai-training-data collection")
    print(f"Data exported to {os.path.abspath(output_path)}")
    
    return training_data, output_path

if __name__ == "__main__":
    # Path to your service account JSON file
    service_account_path = "./API_KEY.json"
    
    # Fetch the data and export to CSV
    training_data, csv_path = fetch_firestore_data_to_csv(service_account_path)
