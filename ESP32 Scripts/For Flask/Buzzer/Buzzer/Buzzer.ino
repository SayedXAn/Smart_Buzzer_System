#include <WiFi.h>
#include <HTTPClient.h>

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
    sendBuzzerPress();
    delay(300); // debounce
  }
}

void sendBuzzerPress() {
  if (WiFi.status() != WL_CONNECTED) return;

  HTTPClient http;
  String url = String(flaskServer) + "/press_buzzer";
  http.begin(url);
  http.addHeader("Content-Type", "application/json");
  String payload = "{\"id\":\"" + myID + "\"}";
  int httpResponseCode = http.POST(payload);

  if (httpResponseCode > 0) {
    String response = http.getString();
    Serial.println("Server response: " + response);

    // Light LED if winner
    if (response.indexOf(myID) != -1) {
      digitalWrite(STATUS_LED, HIGH);
      delay(500);
      digitalWrite(STATUS_LED, LOW);
    }
  } else {
    Serial.println("Error sending press: " + String(httpResponseCode));
  }

  http.end();
}
