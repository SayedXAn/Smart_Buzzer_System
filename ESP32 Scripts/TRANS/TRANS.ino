#include <SPI.h>
#include <RF24.h>
#include <Adafruit_NeoPixel.h>

#define LED_PIN      14      // NeoPixel GPIO
#define NUM_LEDS     24      // Number of LEDs in ring
#define TRIGGER_PIN  15      // Buzzer button
#define STATUS_LED   2       // Built-in LED for feedback

RF24 radio(5, 17);           // NRF24 CE and CSN pins
Adafruit_NeoPixel strip(NUM_LEDS, LED_PIN, NEO_GRB + NEO_KHZ800);

const byte addresses[][6] = {"1Node", "2Node"};
const char myID[] = "D";

bool gameActive = false;
char reply[2] = "";

void setup() {
  Serial.begin(115200);
  pinMode(TRIGGER_PIN, INPUT_PULLUP);
  pinMode(STATUS_LED, OUTPUT);
  digitalWrite(STATUS_LED, LOW);

  strip.begin();
  strip.clear();
  strip.show();

  if (!radio.begin()) {
    while (1);
  }

  radio.setPALevel(RF24_PA_LOW);
  radio.setDataRate(RF24_1MBPS);

  radio.openWritingPipe(addresses[1]);   
  radio.openReadingPipe(1, addresses[0]);
  radio.startListening();

  flashReadySignal();
}

void loop() {
  if (radio.available()) {
    char signal = 0;
    radio.read(&signal, sizeof(signal));

    if (signal == 's') {
      gameActive = true;
    } else if (signal == 'x') {
      gameActive = false;
    }
  }
  if (gameActive && digitalRead(TRIGGER_PIN) == LOW) {

    radio.stopListening();
    bool sent = radio.write(&myID, sizeof(myID));
    radio.startListening();

    digitalWrite(STATUS_LED, HIGH);
    delay(100);
    digitalWrite(STATUS_LED, LOW);

    // 3. Wait for confirmation reply (from receiver)
    unsigned long start = millis();
    while (millis() - start < 1000) {
      if (radio.available()) {
        char received;
        radio.read(&received, sizeof(received));

        if (received == myID[0]) {
          runLightShow();  // Only this buzzer will flash
        }
        break;
      }
    }

    // 4. Reset for next round
    memset(reply, 0, sizeof(reply));
    gameActive = false; // Prevent multiple presses per round
  }

  delay(50); // Debounce delay
}

void flashReadySignal() {
  for (int i = 0; i < 3; i++) { 
    strip.fill(strip.Color(255, 0, 0)); // Red flash
    strip.show();
    delay(150);
    strip.clear();
    strip.show();
    delay(100);
  }
}

void runLightShow() {
  unsigned long startTime = millis();
  while (millis() - startTime < 8000) {
    strip.fill(strip.Color(255, 0, 0));
    strip.show();
    delay(200);
    strip.clear();
    strip.show();
    delay(100);
  }
}
