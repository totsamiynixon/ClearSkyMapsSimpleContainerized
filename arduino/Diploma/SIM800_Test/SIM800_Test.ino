//#include <SoftwareSerial.h>
//
////SIM800 TX is connected to Arduino D8
//#define SIM800_TX_PIN 6
//
////SIM800 RX is connected to Arduino D7
//#define SIM800_RX_PIN 7
//
////Create software serial object to communicate with SIM800
//SoftwareSerial serialSIM800(SIM800_TX_PIN, SIM800_RX_PIN);
//
//void setup() {
//  //Begin serial comunication with Arduino and Arduino IDE (Serial Monitor)
//  Serial.begin(9600);
//  while (!Serial);
//
//  //Being serial communication witj Arduino and SIM800
//  serialSIM800.begin(9600);
//  serialSIM800.println("AT+IPR=9600");
//  delay(1000);
//  delay(1000);
//  serialSIM800.println("AT");
//  delay(1500);
//  serialSIM800.println("AT+COPS?");
//  delay(1500);
//  serialSIM800.println("AT+CPAS");
//  delay(1500);
//  serialSIM800.println("AT+CREG?");
//  delay(1500);
//  serialSIM800.println("ATI");
//  delay(1500);
//  Serial.println("Setup Complete!");
//}
//
//void loop() {
//  //Read Arduino IDE Serial Monitor inputs (if available) and send them to SIM800
//  if (Serial.available()) {
//    serialSIM800.write(Serial.read());
//  }
//  //Read SIM800 output (if available) and print it in Arduino IDE Serial Monitor
//  if (serialSIM800.available()) {
//    Serial.write(serialSIM800.read());
//  }
//  delay(1500);
//
//}

#include <SoftwareSerial.h>
SoftwareSerial SIM800(6, 7); // 8 - RX Arduino (TX SIM800L), 9 - TX Arduino (RX SIM800L)

void setup() {
  // Open serial communications and wait for port to open:
  Serial.begin(9600);
  Serial.println("Start!");
  delay(2000);

  SIM800.begin(9600);
  SIM800.println("AT+IPR=9600"); // Скорость порта GSM модуля (на всякий случай);
  delay(1000);
  gprs_init();
}
void loop() {

}

void gprs_init()
{ //Процедура начальной инициализации GSM модуля
  int ATsCount = 17;
  String ATs[] = {
    "AT+CIPCLOSE",
    "AT+CIPSTATUS",
    "AT",
    "AT+CGATT?",
    "AT+CGATT=1",
    "AT+CIPSTATUS",
    "AT+CGDCONT=1,\"IP\",\"free\"",
    "AT+CIPSTATUS",
    "AT+CGACT=1,1",
    "AT+CIPSTATUS",
    "AT+CIFSR",
    "AT+CIPSTATUS",
    "AT+CIPSTART=\"TCP\",\"91.149.139.247\",45455",
    "AT+CIPSEND",
    "AT+CIPSTATUS",
    "AT+CIPCLOSE",
    "AT+CIPSTATUS"
  };
  Serial.println("GPRG init start");
  for (int i = 0; i < ATsCount; i++)
  {
    //    Serial.println(ATs[i]);  //посылаем в монитор порта
    SIM800.println(ATs[i]);  //посылаем в GSM модуль
    delay(1000);
    Serial.println(ReadGSM());  //показываем ответ от GSM модуля
    delay(1000);
  }
  Serial.println("GPRG init complete");
}

String ReadGSM()
{ //функция чтения данных от GSM модуля
  int c;
  String v;
  while (SIM800.available())
  { //сохраняем входную строку в переменную v
    c = SIM800.read();
    v += char(c);
    delay(10);
  }
  return v;
}
