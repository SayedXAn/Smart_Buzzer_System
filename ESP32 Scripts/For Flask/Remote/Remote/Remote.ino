#include <WiFi.h>
#include <HTTPClient.h>
#include <Adafruit_NeoPixel.h>

#define REMOTE_ID "REMOTE"
#define TRIGGER_PIN 15
#define STATUS_LED 2
#define LED_PIN 14      
#define NUM_LEDS 24 

Adafruit_NeoPixel strip(NUM_LEDS, LED_PIN, NEO_GRB + NEO_KHZ800);

const char* ssid = "Dark_Experience";
const char* password = "payforpassword";
const char* flaskServer = "http://192.168.0.101:5000";

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
  strip.begin();
  strip.clear();
  strip.show();
  flashReadySignal();
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
    runLightShow();
  } else {
    Serial.println("Error sending remote toggle");
  }
  http.end();
}

void flashReadySignal() {
  for (int i = 0; i < 3; i++) {
    strip.fill(strip.Color(255, 0, 0));
    strip.show();
    delay(150);
    strip.clear();
    strip.show();
    delay(100);
  }
}

void runLightShow() {
  unsigned long startTime = millis();
  while (millis() - startTime < 2000) {
    strip.fill(strip.Color(255, 255, 255));  // Green flash
    strip.show();
    delay(200);
    strip.clear();
    strip.show();
    delay(100);
  }
}
