from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

# Global game state
game_state = {
    "is_on": False,
    "winner": None
}

@app.route("/game_state", methods=["GET"])
def get_game_state():
    return jsonify(game_state)

@app.route("/buzzer_press", methods=["POST"])
def buzzer_press():
    global game_state
    data = request.json
    buzzer_id = data.get("id")
    remote = data.get("remote", False)

    if remote:
        # Toggle game state
        game_state["is_on"] = not game_state["is_on"]
        game_state["winner"] = None
        print(f"Remote toggled game: {game_state['is_on']}")
        return jsonify({"status": "ok", "game_state": game_state})

    if game_state["is_on"] and game_state["winner"] is None:
        game_state["winner"] = buzzer_id
        print(f"Winner: {buzzer_id}")
        return jsonify({"status": "ok", "winner": buzzer_id})

    return jsonify({"status": "ignored"}), 400

@app.route('/winner')
def winner():
    buzzer_id = request.args.get('buzzer')
    print("Winner received:", buzzer_id)
    return jsonify({"message": f"Winner {buzzer_id} received"}), 200

@app.route('/game_state')
def game_state():
    state = request.args.get('state')
    print("Game state received:", state)
    return jsonify({"message": f"Game state {state} received"}), 200


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)
