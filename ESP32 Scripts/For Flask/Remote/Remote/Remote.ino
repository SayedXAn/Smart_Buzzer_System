#include <WiFi.h>
#include <HTTPClient.h>

#define REMOTE_ID "REMOTE"
#define TRIGGER_PIN 15
#define STATUS_LED 2

const char* ssid = "Experience";
const char* password = "payforpassword";
const char* flaskServer = "http://192.168.0.103:5000";

unsigned long lastPressTime = 0;
const unsigned long cooldown = 3000; 

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
    unsigned long now = millis();
    if (now - lastPressTime > cooldown) {
      lastPressTime = now; // update cooldown
      sendRemoteToggle(); // your HTTP POST function
    }
  }
}

void sendRemoteToggle() {
  if (WiFi.status() != WL_CONNECTED) return;

  HTTPClient http;
  String url = String(flaskServer) + "/remote_toggle";
  http.begin(url);
  int code = http.POST("");
  if (code > 0) {
    Serial.println("Remote toggle sent, response: " + String(code));
  } else {
    Serial.println("Error sending remote toggle");
  }
  http.end();
}
