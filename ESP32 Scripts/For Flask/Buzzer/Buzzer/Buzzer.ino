#include <WiFi.h>
#include <HTTPClient.h>
#include <Adafruit_NeoPixel.h>

#define TRIGGER_PIN 15
#define STATUS_LED 2
#define LED_PIN 14
#define NUM_LEDS 24 
Adafruit_NeoPixel strip(NUM_LEDS, LED_PIN, NEO_GRB + NEO_KHZ800);

const char* ssid = "Experience";
const char* password = "payforpassword";
const char* flaskServer = "http://192.168.0.105:5000";

String myID = "G"; // Unique buzzer ID
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
  strip.begin();
  strip.clear();
  strip.show();
  flashReadySignal();
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

    if (response.indexOf("\"winner\":\"" + myID + "\"") != -1) {
      Serial.println("I am the winner! Running light show...");
      runLightShow();
    }
  } else {
    Serial.println("Error sending press: " + String(httpResponseCode));
    
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
  while (millis() - startTime < 5000) {
    strip.fill(strip.Color(255, 0, 0));  // Green flash
    strip.show();
    delay(200);
    strip.clear();
    strip.show();
    delay(100);
  }
}
