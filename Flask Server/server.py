from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

game_state = {
    "isGameOn": False,
    "winners": []
}

# --- Game control endpoints ---
@app.route('/start_game', methods=['POST'])
def start_game():
    game_state['isGameOn'] = True
    game_state['winners'] = []
    print("Game started")
    return jsonify({"status": "game started"})

@app.route('/stop_game', methods=['POST'])
def stop_game():
    game_state['isGameOn'] = False
    print("Game stopped")
    return jsonify({"status": "game stopped"})

# --- Buzzer press endpoint ---
@app.route('/press_buzzer', methods=['POST'])
def press_buzzer():
    data = request.get_json()
    buzzer_id = data.get('id')
    if buzzer_id and game_state['isGameOn']:
        game_state['winners'].append(buzzer_id)
        print(f"Buzzer pressed: {buzzer_id}")
    return jsonify({"status": "ok"})

# --- State polling endpoint for Unity ---
@app.route('/state', methods=['GET'])
def state():
    # Return all pending presses and clear them after polling
    winners_to_send = game_state['winners'][:]
    game_state['winners'] = []
    return jsonify({
        "isGameOn": game_state['isGameOn'],
        "winners": winners_to_send
    })

if __name__ == '__main__':
    # Start server on all interfaces, port 5000
    app.run(host='0.0.0.0', port=5000)
