from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

game_state = {
    "isGameOn": False,
    "winner": None
}

@app.route("/start_game", methods=["POST"])
def start_game():
    game_state["isGameOn"] = True
    game_state["winner"] = None
    return jsonify({"status": "game started"})

@app.route("/stop_game", methods=["POST"])
def stop_game():
    game_state["isGameOn"] = False
    return jsonify({"status": "game stopped"})

@app.route("/press_buzzer", methods=["POST"])
def press_buzzer():
    buzzer_id = request.json.get("id")
    if not game_state["isGameOn"]:
        return jsonify({"status": "game not active"}), 400

    if game_state["winner"] is None:
        game_state["winner"] = buzzer_id
        return jsonify({"winner": buzzer_id})
    else:
        return jsonify({"status": "already has winner"}), 400

@app.route("/state", methods=["GET"])
def state():
    return jsonify(game_state)

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)
