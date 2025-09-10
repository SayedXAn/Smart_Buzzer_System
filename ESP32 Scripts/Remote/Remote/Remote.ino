#include <SPI.h>
#include <RF24.h>
#include <Adafruit_NeoPixel.h>

#define LED_PIN      14      // NeoPixel GPIO
#define NUM_LEDS     24 
#define TRIGGER_PIN  15
#define STATUS_LED   2

RF24 radio(5, 17); // CE, CSN
const byte addresses[][6] = {"1Node", "2Node"};
Adafruit_NeoPixel strip(NUM_LEDS, LED_PIN, NEO_GRB + NEO_KHZ800);

const char REMOTE_ID = 'H';

void setup() {
  Serial.begin(115200);
  pinMode(TRIGGER_PIN, INPUT_PULLUP);
  pinMode(STATUS_LED, OUTPUT);
  digitalWrite(STATUS_LED, LOW);

  if (!radio.begin()) {
    Serial.println("NRF24 init failed!");
    while (1);
  }

  radio.setPALevel(RF24_PA_LOW);
  radio.setDataRate(RF24_1MBPS);

  radio.openWritingPipe(addresses[0]);
  radio.stopListening(); // transmit only

  Serial.println("Remote buzzer ready!");

  // Small startup delay
  delay(500);
}

void loop() {
  if (digitalRead(TRIGGER_PIN) == LOW) {
    bool success = false;

    // Send 3 times for reliability
    for (int i = 0; i < 3; i++) {
      success = radio.write(&REMOTE_ID, 1);
      delay(10);
    }

    if (success) {
      Serial.println("Remote button pressed!");
      digitalWrite(STATUS_LED, HIGH);
      flashReadySignal();
      delay(100);
      digitalWrite(STATUS_LED, LOW);
    } else {
      Serial.println("Failed to send!");
    }

    // Debounce
    delay(300);
  }
}

void flashReadySignal() {
  for (int i = 0; i < 3; i++) {
    strip.fill(strip.Color(0, 255, 0));
    strip.show();
    delay(150);
    strip.clear();
    strip.show();
    delay(100);
  }
}
