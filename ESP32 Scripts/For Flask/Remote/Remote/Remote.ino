#include <WiFi.h>
#include <HTTPClient.h>

#define REMOTE_ID "REMOTE"
#define TRIGGER_PIN 15
#define STATUS_LED 2

const char* ssid = "Experience";
const char* password = "payforpassword";
const char* flaskServer = "http://192.168.0.103:5000";

String myID = "I"; // Unique buzzer ID
bool gameActive = false;

void setup() {
  Serial.begin(115200);
  pinMode(TRIGGER_PIN, INPUT_PULLUP);
  pinMode(STATUS_LED, OUTPUT);
  digitalWrite(STATUS_LED, LOW);

  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi connected.");
}

void loop() {
  if (digitalRead(TRIGGER_PIN) == LOW) {
    toggleGame();
    delay(500); // debounce
  }
}

void toggleGame() {
  if (WiFi.status() != WL_CONNECTED) return;

  HTTPClient http;
  String url = String(flaskServer) + (gameActive ? "/stop_game" : "/start_game");
  http.begin(url);
  int code = http.POST("");
  if (code > 0) {
    Serial.println("Toggled game, response: " + String(code));
    gameActive = !gameActive;
  }
  http.end();
}