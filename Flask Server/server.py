from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

game_state = {
    "isGameOn": False,
    "winner": None   # store only one winner
}

# --- Game control via remote ---
@app.route('/remote_toggle', methods=['POST'])
def remote_toggle():
    # Remote always resets winner and flips game state
    if not game_state['isGameOn']:
        # Turn game ON
        game_state['isGameOn'] = True
        game_state['winner'] = None
        print("Remote pressed -> Game started, winner cleared")
        return jsonify({"status": "game started"})
    else:
        # Turn game OFF
        game_state['isGameOn'] = False
        game_state['winner'] = None
        print("Remote pressed -> Game stopped, winner cleared")
        return jsonify({"status": "game stopped"})


# --- Buzzer press endpoint ---
@app.route('/press_buzzer', methods=['POST'])
def press_buzzer():
    data = request.get_json()
    buzzer_id = data.get('id')

    if buzzer_id and game_state['isGameOn']:
        if game_state['winner'] is None:  # accept only the first press
            game_state['winner'] = buzzer_id
            game_state['isGameOn'] = False  # stop game immediately
            print(f"Winner locked: {buzzer_id}")
        else:
            print(f"Ignored buzzer {buzzer_id}, winner already set")
    else:
        print(f"Ignored buzzer {buzzer_id}, game not active")

    return jsonify({
        "status": "ok",
        "winner": game_state['winner']
    })



# --- State polling endpoint for Unity ---
@app.route('/state', methods=['GET'])
def state():
    return jsonify({
        "isGameOn": game_state['isGameOn'],
        "winner": game_state['winner']
    })


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
