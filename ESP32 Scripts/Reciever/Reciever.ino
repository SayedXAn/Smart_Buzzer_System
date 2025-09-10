#include <SPI.h>
#include <RF24.h>
#include <WiFi.h>
#include <HTTPClient.h>

#define CE_PIN 5
#define CSN_PIN 17
#define LED_PIN 2

RF24 radio(CE_PIN, CSN_PIN);
const byte addresses[][6] = {"1Node", "2Node"};

const char REMOTE_ID = 'H'; // remote buzzer

char winnerID[2] = {'\0', '\0'};
bool winnerLocked = false;
bool isGameOn = false;

// WiFi settings
const char* ssid = "Experience";
const char* password = "payforpassword";
const char* flaskServer = "http://192.168.0.103:5000"; // Flask server URL

void setup() {
  Serial.begin(115200);
  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);

  // Connect WiFi
  WiFi.begin(ssid, password);
  Serial.print("Connecting WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi connected.");

  // Initialize NRF24
  if (!radio.begin()) {
    Serial.println("NRF24 init failed!");
    while (1);
  }
  radio.setPALevel(RF24_PA_LOW);
  radio.setDataRate(RF24_1MBPS);
  radio.openReadingPipe(1, addresses[1]);  // From buzzers
  radio.openWritingPipe(addresses[0]);     // To buzzers
  radio.startListening();

  Serial.println("Receiver ready!");
}

void loop() {
  // Always check radio
  while (radio.available()) {
    char incoming = 0;
    radio.read(&incoming, 1);
    Serial.print("Received buzzer: ");
    Serial.println(incoming);

    // Remote always toggles game
    if (incoming == REMOTE_ID) {
      Serial.println("Remote pressed!");
      if (isGameOn) stopGame();
      else startGame();
      continue; // allow more packets in the buffer
    }

    // Normal buzzer
    if (isGameOn && !winnerLocked) {
      winnerLocked = true;
      winnerID[0] = incoming;
      winnerID[1] = '\0';

      Serial.print("Winner detected: ");
      Serial.println(winnerID);

      // Notify buzzers
      radio.stopListening();
      for (int i = 0; i < 3; ++i) {
        radio.write(&winnerID[0], 1);
        delay(20);
      }
      radio.startListening();

      // LED feedback
      digitalWrite(LED_PIN, HIGH);
      delay(100);
      digitalWrite(LED_PIN, LOW);

      // Send winner to Flask
      sendWinnerToFlask(winnerID[0]);

      // Stop game until next start
      isGameOn = false;
      winnerLocked = false; // unlock for next round
      break; // exit inner while to continue main loop
    }
  }

  // Optional serial commands
  if (Serial.available() > 0) {
    char cmd = Serial.read();
    if (cmd == 'n') startGame();
    else if (cmd == 'f') stopGame();
  }

  delay(5); // short loop delay
}




// --- Game control functions ---
void startGame() {
  isGameOn = true;
  winnerLocked = false;
  winnerID[0] = '\0';

  // Flush stale NRF packets
  while (radio.available()) {
    char dummy;
    radio.read(&dummy, 1);
    delay(2);
  }

  // Notify buzzers
  radio.stopListening();
  char startSignal = 's';
  radio.write(&startSignal, 1);
  radio.startListening();

  Serial.println("Game started!");
  sendGameStateToFlask("N");
}

void stopGame() {
  isGameOn = false;
  winnerLocked = false;
  winnerID[0] = '\0';

  // Notify buzzers
  radio.stopListening();
  char stopSignal = 'x';
  radio.write(&stopSignal, 1);
  radio.startListening();

  Serial.println("Game stopped!");
  sendGameStateToFlask("F");
}

// --- Flask communication ---
void sendWinnerToFlask(char winner) {
  if (WiFi.status() != WL_CONNECTED) return;

  HTTPClient http;
  String url = String(flaskServer) + "/winner?buzzer=" + winner;
  http.begin(url);
  int code = http.GET();
  if (code > 0) Serial.print("Flask winner response code: "), Serial.println(code);
  http.end();
}

void sendGameStateToFlask(String state) {
  if (WiFi.status() != WL_CONNECTED) return;

  HTTPClient http;
  String url = String(flaskServer) + "/game_state?state=" + state;
  http.begin(url);
  int code = http.GET();
  if (code > 0) Serial.print("Flask game_state response code: "), Serial.println(code);
  http.end();
}
