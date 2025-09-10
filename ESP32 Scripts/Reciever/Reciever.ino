// Receiver ESP32 - clean single-char serial protocol + remote toggle
#include <SPI.h>
#include <RF24.h>

#define CE_PIN 5
#define CSN_PIN 17
#define LED_PIN 2

RF24 radio(CE_PIN, CSN_PIN);
const byte addresses[][6] = {"1Node", "2Node"};

const char REMOTE_ID = 'H'; // remote buzzer ID
char winnerID[2] = { '\0', '\0' };
bool winnerLocked = false;
bool isGameOn = false;

void setup() {
  Serial.begin(115200);
  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);

  if (!radio.begin() || !radio.isChipConnected()) {
    // no debug prints per request - hang if radio fails
    while (1);
  }

  radio.setPALevel(RF24_PA_LOW);
  radio.setDataRate(RF24_1MBPS);
  radio.openReadingPipe(1, addresses[1]);  // From buzzers
  radio.openWritingPipe(addresses[0]);     // To buzzers
  radio.startListening();

  delay(200);
  // Send initial state to Unity (F = game off)
  Serial.write('F');
  Serial.write('\n');
}

void loop() {
  // Handle incoming serial commands from Unity (single char commands)
  if (Serial.available() > 0) {
    char cmd = Serial.read();
    if (cmd == 'n') {
      startGame();
    } else if (cmd == 'f') {
      stopGame();
    } else if (cmd == 'q') { // status query from Unity
      if (isGameOn) {
        Serial.write('N');
        Serial.write('\n');
      } else {
        Serial.write('F');
        Serial.write('\n');
      }
    }
  }

  // Handle radio messages
  if (radio.available()) {
    char incoming = 0;
    radio.read(&incoming, 1); // read exactly one byte

    // Remote control buzzer toggles game state
    if (incoming == REMOTE_ID) {
      if (!isGameOn) startGame();
      else stopGame();
      return; // do not treat remote as winner
    }

    // Normal buzzer pressed: only count if game on and no winner locked
    if (isGameOn && !winnerLocked) {
      winnerLocked = true;
      winnerID[0] = incoming;
      winnerID[1] = '\0';

      // Send single char + newline to Unity
      Serial.write(winnerID[0]);
      Serial.write('\n');

      // Reply to buzzers to indicate winner (send winner char multiple times)
      radio.stopListening();
      for (int i = 0; i < 3; ++i) {
        radio.write(&winnerID[0], 1);
        delay(20);
      }
      radio.startListening();

      // Visual feedback on receiver
      digitalWrite(LED_PIN, HIGH);
      delay(120);
      digitalWrite(LED_PIN, LOW);

      // stop game until next start (receiver will require startGame to unlock)
      isGameOn = false;
      // keep winnerLocked true so no further winners until startGame resets it
    }
  }
}

void startGame() {
  isGameOn = true;
  winnerLocked = false;
  winnerID[0] = '\0';

  // flush any stale radio data
  clearRadioBuffer();

  // notify buzzers to enable
  radio.stopListening();
  char startSignal = 's';
  radio.write(&startSignal, 1);
  radio.startListening();

  // notify Unity
  Serial.write('N');
  Serial.write('\n');
}

void stopGame() {
  isGameOn = false;
  winnerLocked = false;
  winnerID[0] = '\0';

  // notify buzzers to disable
  radio.stopListening();
  char stopSignal = 'x';
  radio.write(&stopSignal, 1);
  radio.startListening();

  // notify Unity
  Serial.write('F');
  Serial.write('\n');
}

void clearRadioBuffer() {
  // drain any packets
  while (radio.available()) {
    char dummy;
    radio.read(&dummy, 1);
    delay(2);
  }
}
