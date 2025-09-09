#include <SPI.h>
#include <RF24.h>
#include <string.h>

#define CE_PIN 5
#define CSN_PIN 17
#define LED_PIN 2

RF24 radio(CE_PIN, CSN_PIN);
const byte addresses[][6] = {"1Node", "2Node"};

char winnerID[1] = "";  
bool winnerLocked = false;
bool isGameOn = false;

void setup() {
  Serial.begin(115200);
  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);

  if (!radio.begin() || !radio.isChipConnected()) {
    Serial.println("NRF24 not found");
    while (1);  
  }

  radio.setPALevel(RF24_PA_LOW);
  radio.setDataRate(RF24_1MBPS);
  radio.openReadingPipe(1, addresses[1]);  // From buzzers
  radio.openWritingPipe(addresses[0]);     // To buzzers
  radio.startListening();
  delay(1000);

  Serial.println("Receiver ready");
}

void loop() {
  // Handle serial commands from Unity
  if (Serial.available() > 0) {
    char received = Serial.read();

    if (received == 'n') {
      isGameOn = true;

      // Flush stale buzzer signals
      while (radio.available()) {
        char flushBuf[10];
        delay(10);
        radio.read(&flushBuf, sizeof(flushBuf));
      }

      // Notify buzzers to enable
      radio.stopListening();
      char startSignal = 's';
      bool success = radio.write(&startSignal, sizeof(startSignal));
      delay(100);
      radio.startListening();
    }

    else if (received == 'f') {
      isGameOn = false;
      winnerLocked = false;
      winnerID[0] = '\0';
      radio.stopListening();
      char stopSignal = 'x';
      bool success = radio.write(&stopSignal, sizeof(stopSignal));
      delay(100);
      radio.startListening();
    }
  }

  // Handle buzzer press
  if (radio.available() && isGameOn) {
    char incoming[2] = "";
    radio.read(&incoming, sizeof(incoming));

    if (!winnerLocked) {
      strncpy(winnerID, incoming, 1);
      winnerID[1] = '\0'; // Ensure null-termination
      winnerLocked = true;
      Serial.println(winnerID);

      delay(30);  // Buffer delay before switching to TX

      radio.stopListening();
      radio.openWritingPipe(addresses[0]);

      // Reply to all buzzers to light up only the winner
      char reply = winnerID[0];
      for (int i = 0; i < 3; i++) {
        radio.write(&reply, sizeof(reply));
        delay(20);  // Small gap between writes
      }

      digitalWrite(LED_PIN, HIGH);
      delay(100);
      digitalWrite(LED_PIN, LOW);

      winnerLocked = false;
      winnerID[0] = '\0';
      isGameOn = false;
      delay(5000);  // Cooldown before next round
      radio.startListening();
    }
  }
}
