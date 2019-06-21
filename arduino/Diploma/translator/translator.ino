#include <SoftwareSerial.h>

SoftwareSerial A6board (6, 7);

void setup() {
  Serial.begin(9600);
  A6board.begin(9600);
  Serial.println("START");

}

void loop() {
  Serial.println("Loop");
  delay(3000);
  while (Serial.available()) {
    A6board.write(Serial.read());
  }
  while (A6board.available()) {
    Serial.write(A6board.read());
  }

}
