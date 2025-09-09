#include <SPI.h>
#include <RF24.h>
#include <string.h>

#define CE_PIN 5
#define CSN_PIN 17
#define LED_PIN 2

RF24 radio(CE_PIN, CSN_PIN);
const byte addresses[][6] = {"1Node", "2Node"};

char winnerID[2] = "";  
bool winnerLocked = false;
bool isGameOn = false;

void setup() {
  Serial.begin(115200);
  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);

  if (!radio.begin() || !radio.isChipConnected()) {
    while (1);  
  }

  radio.setPALevel(RF24_PA_LOW);
  radio.setDataRate(RF24_1MBPS);
  radio.openReadingPipe(1, addresses[1]);  
  radio.openWritingPipe(addresses[0]);     
  radio.startListening();
}

void loop() {
  // Handle serial commands from Unity
  if (Serial.available() > 0) {
    char received = Serial.read();

    if (received == 'n') {
      isGameOn = true;
      winnerLocked = false;
      winnerID[0] = '\0';

      // Flush stale buzzer signals
      while (radio.available()) {
        char flushBuf[10];
        radio.read(&flushBuf, sizeof(flushBuf));
      }

      // Notify buzzers to enable
      radio.stopListening();
      char startSignal = 's';
      radio.write(&startSignal, sizeof(startSignal));
      radio.startListening();
    }

    else if (received == 'f') {
      isGameOn = false;
      winnerLocked = false;
      winnerID[0] = '\0';

      radio.stopListening();
      char stopSignal = 'x';
      radio.write(&stopSignal, sizeof(stopSignal));
      radio.startListening();
    }
  }

  // Handle buzzer press
  if (radio.available() && isGameOn && !winnerLocked) {
    char incoming[2] = "";
    radio.read(&incoming, sizeof(incoming));
    strncpy(winnerID, incoming, 1);
    winnerID[1] = '\0'; // null-terminate safely
    winnerLocked = true;

    Serial.println(winnerID); // send winner ID to Unity

    delay(20);
    radio.stopListening();

    // Reply winner to buzzers
    char reply = winnerID[0];
    radio.write(&reply, sizeof(reply));

    digitalWrite(LED_PIN, HIGH);
    delay(100);
    digitalWrite(LED_PIN, LOW);

    isGameOn = false;  // stop game after one winner
    radio.startListening();
  }
}
