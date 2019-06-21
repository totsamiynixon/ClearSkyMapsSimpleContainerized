#include <SoftwareSerial.h>
SoftwareSerial SIM800(6, 7); // 8 - RX Arduino (TX SIM800L), 9 - TX Arduino (RX SIM800L)

void setup() {
  // Open serial communications and wait for port to open:
  Serial.begin(9600);
  Serial.println("Start!");
  SIM800.begin(9600);
  delay(10000);
  //  SIM800.println("AT+IPR=9600"); // Скорость порта GSM модуля (на всякий случай);
  //  Serial.println(ReadGSM());
  //  delay(1000);
  SIM800.println("AT");
  Serial.println(ReadGSM());
  delay(1500);
  SIM800.println("AT+COPS?");
  Serial.println(ReadGSM());
  delay(1500);
  SIM800.println("AT+CPAS");
  Serial.println(ReadGSM());
  delay(1500);
  SIM800.println("AT+CREG?");
  Serial.println(ReadGSM());
  delay(1500);
  SIM800.println("ATI");
  Serial.println(ReadGSM());
  //  delay(1500);
  //  SIM800.println("AT+CMGF=1"); // Configuring TEXT mode
  //  Serial.println(ReadGSM());
  delay(1500);
  //  SIM800.println("AT+CMGS=\"375259132721\"");//change ZZ with country code and xxxxxxxxxxx with phone number to sms
  //  Serial.println(ReadGSM());
  //  SIM800.print("AT+CMGS=\"");
  //  SIM800.print("+375259132721");
  //  SIM800.write(0x22);
  //  SIM800.write(0x0D);
  //  SIM800.write(0x0A);
  //  delay(5000);
  //  SIM800.print("Last Minute Engineers | lastminuteengineers.com");
  //  Serial.println(ReadGSM());
  //  delay(3000);
  //  SIM800.write(26);
  //  Serial.println(ReadGSM());
  //  delay(2000);
  delay(1500);
}

void loop() {
  Serial.println("Loop");
  Serial.println("Trying to setup module");
  bool isSetup = false;
  while (SIM800.available() && !isSetup) {
    SIM800.println("AT+IPR=9600");
    if (ReadGSM().length() > 0) {
      isSetup = true;
    }
  }
  Serial.println("Module has been setup");
  //  SIM800.print("AT+CMGS=\"");
  //  SIM800.print("375259132721");
  //  SIM800.write(0x22);
  //  SIM800.write(0x0D);
  //  SIM800.write(0x0A);
  //  delay(5000);
  //  SIM800.print("Last Minute Engineers | lastminuteengineers.com");
  //  delay(3000);
  //  Serial.println(ReadGSM());
  //  SIM800.write(26);
  SIM800.println("AT+CIPSHUT\r"); //RESPONSE= OK
  Serial.println(ReadGSM());
  delay(1000);
  SIM800.println("AT+CIPMUX=0\r"); //RESPONSE= OK
  Serial.println(ReadGSM());
  delay(2000);
  SIM800.println("AT+CGATT=1\r"); //RESPONSE= OK
  Serial.println(ReadGSM());
  delay(1000);
  SIM800.println("AT+CSTT=\"vmi.velcom.by\",\"\",\"\"\r"); //RESPONSE= OK
  Serial.println(ReadGSM());
  delay(5000);
  SIM800.println("AT+CIICR\r"); //RESPONSE= OK
  Serial.println(ReadGSM());
  delay(5000);
  SIM800.println("AT+CIFSR\r"); //RESPONSE= Returns an IP
  Serial.println(ReadGSM());
  delay(2000);
  SIM800.println("AT+CIPSTART=\"TCP\",\"91.149.139.247\", 80\r"); //RESPONSE= CONNECTED OK
  Serial.println(ReadGSM());
  delay(3000);
  SIM800.println("AT+CIPSEND\r"); //RESPONSE= >
  Serial.println(ReadGSM());
  delay(500);
  SIM800.println("POST http://91.149.139.247/api/integration HTTP/1.1");
  Serial.println(ReadGSM());
  delay(500);
  SIM800.println("Host: 91.149.139.247");
  Serial.println(ReadGSM());
  delay(500);
  SIM800.println("Content-Type: application/json");
  Serial.println(ReadGSM());
  delay(500);
  SIM800.println("Content-Length: 25\r\n");
  Serial.println(ReadGSM());
  delay(500);
  SIM800.println("{\"Celsius\":\"TEMPERATURE\"}");
  Serial.println(ReadGSM());
  delay(500);
  SIM800.write(0x1A); // Ctrl Z
  Serial.println(ReadGSM());
  delay(10000);
  /*
    After sending all these instructions, I get the following response,
    OK
    HTTP/1.1 200 OK
    Friday December, 22
    +TCPCLOSE=0
    OK
  */
  SIM800.println("AT+CIPCLOSE"); //RESPONSE= OK
  Serial.println(ReadGSM());
  delay(1000);
  SIM800.println("AT+CIPSHUT"); //RESPONSE= OK
  Serial.println(ReadGSM());
  delay(1000);
  delay(2000);
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
