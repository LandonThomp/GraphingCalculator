import json
import sys
import os

def read_combined_string():
    data = sys.stdin.read().strip()
    return data

def update_json_file(json_filename, combined_string):
    if not os.path.exists(json_filename):
        with open(json_filename, 'w') as file:
            json.dump([], file)

    with open(json_filename, 'r') as file:
        data = json.load(file)

    data.append(combined_string)

    if len(data) > 3:
        data.pop(0)

    with open(json_filename, 'w') as file:
        json.dump(data, file, indent=4)

def get_last_three_entries(json_filename):
    if os.path.exists(json_filename):
        with open(json_filename, 'r') as file:
            data = json.load(file)
        return data[-3:]
    else:
        return []

def get_entry(json_filename, key):
    entry_number = int(key.split(':')[1]) - 1
    if os.path.exists(json_filename):
        with open(json_filename, 'r') as file:
            data = json.load(file)
        if 0 <= entry_number < len(data):
            return data[entry_number]
        else:
            return "Invalid key."
    else:
        return "No data found."

def main():
    json_filename = "calculatorhistory.json"
    command = sys.argv[1] if len(sys.argv) > 1 else ""

    if command.startswith("Key:"):
        result = get_entry(json_filename, command)
        print(result)
    elif command == "get_last_three":
        entries = get_last_three_entries(json_filename)
        for entry in entries:
            print(entry)
    else:
        combined_string = read_combined_string()
        update_json_file(json_filename, combined_string)
        print("Stored.")

if __name__ == "__main__":
    main()
