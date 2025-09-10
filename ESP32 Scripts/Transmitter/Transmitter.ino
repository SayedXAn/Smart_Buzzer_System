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
const char myID[] = "H";      // Unique ID of this buzzer
const char REMOTE_ID = 'H';   // Remote buzzer ID

bool gameActive = false;

void setup() {
  Serial.begin(115200);

  pinMode(TRIGGER_PIN, INPUT_PULLUP);
  pinMode(STATUS_LED, OUTPUT);
  digitalWrite(STATUS_LED, LOW);

  strip.begin();
  strip.clear();
  strip.show();

  if (!radio.begin()) {
    Serial.println("NRF24 init failed!");
    while (1);
  }

  radio.setPALevel(RF24_PA_LOW);
  radio.setDataRate(RF24_1MBPS);

  // Pipes
  radio.openWritingPipe(addresses[1]);   
  radio.openReadingPipe(1, addresses[0]);
  radio.startListening();

  flashReadySignal();
  Serial.println("Buzzer ready!");
}

void loop() {
  // Listen for start/stop signals from receiver
  if (radio.available()) {
    char signal = 0;
    radio.read(&signal, 1);  // read exactly 1 byte

    if (signal == 's') {
      gameActive = true;
    } else if (signal == 'x') {
      gameActive = false;
    }
  }

  // Check button press
  if (gameActive && digitalRead(TRIGGER_PIN) == LOW) {
    // Stop listening before sending
    radio.stopListening();
    radio.write(&myID, sizeof(myID));
    radio.startListening();

    // Feedback LED (optional quick blink)
    digitalWrite(STATUS_LED, HIGH);
    delay(100);
    digitalWrite(STATUS_LED, LOW);

    // Wait for acknowledgement for up to 1 second
    unsigned long timeout = millis() + 1000;
    bool ackReceived = false;
    while (millis() < timeout) {
        if (radio.available()) {
            char received;
            radio.read(&received, sizeof(received));

            if (received == myID[0]) {
                ackReceived = true;
                break;  // Correct ID received
            }
        }
    }

    // Only run light show if ack received
    if (ackReceived) {
        runLightShow();
    }

    // Prevent multiple presses
    gameActive = false;
}

  delay(50);  // Debounce
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
  while (millis() - startTime < 8000) {
    strip.fill(strip.Color(255, 0, 0));  // Green flash
    strip.show();
    delay(200);
    strip.clear();
    strip.show();
    delay(100);
  }
}
