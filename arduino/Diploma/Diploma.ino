#include <SoftwareSerial.h>
#include <MQ135.h>
#include <MQ9.h>
#include "DHT.h"
#include <SFE_BMP180.h>
#include <Wire.h>

#define MQ135Pin A2
#define MQ9Pin A3
#define DHTPIN 3


//CONSTANTS
const String API_KEY = "";
const String SERVER_URL = "SERVER_URL";
const double MOL_CO2 = 44.01;
const float MOL_LPG = 0;
const float MOL_CO = 28.01;
const float MOL_CH4 = 16.04;

SFE_BMP180 pressure;             // Объявляем переменную для доступа к SFE_BMP180

MQ135 mq135 = MQ135( MQ135Pin);  // инициализация объекта датчика
MQ9 mq9( MQ9Pin);
DHT dht(DHTPIN, DHT11);
/*Для пыли*/
int pin = 8;
unsigned long duration;
unsigned long starttime;
unsigned long sampletime_ms = 30000;
unsigned long lowpulseoccupancy = 0;
float ratio = 0;
float concentration = 0;
/*---------*/

void setup() {

  Serial.begin(9600); // последовательный порт для отображения данных
  mq9.calibrate();
  mq9.getRo();
  dht.begin();
  pressure.begin(); // Инициализация датчика
  pinMode(8, INPUT);
  starttime = millis(); //задержка для измерерния пыли
}

void loop() {

  duration = pulseIn(pin, LOW);
  lowpulseoccupancy = lowpulseoccupancy + duration;
  if ((millis() - starttime) > sampletime_ms)
  {

    ratio = lowpulseoccupancy / (sampletime_ms * 10.0); // Integer percentage 0=>100
    concentration = 1.1 * pow(ratio, 3) - 3.8 * pow(ratio, 2) + 520 * ratio + 0.62;
    lowpulseoccupancy = 0;
    starttime = millis();
  }

  //Readings
  double T, P, Ppa, Pmm;
  float latitude, longitude;
  
  pressure.startTemperature();
  delay(100);
  pressure.getTemperature(T);
  pressure.startPressure(3);
  delay(100);
  pressure.getPressure(P, T);
  float H = dht.readHumidity();
  double co2 = mq135.getPPM(); // чтение данных концентрации CO2
  double co2PDK = conversion(MOL_CO2, co2, T, P);
  float lpg = mq9.readLPG(); // чтение данных концентрации LPG
  float co = mq9.readCarbonMonoxide(); // чтение данных концентрации CO
  double coPDK = conversion(MOL_CO, co, T, P);
  float ch4 = mq9.readMethane(); // чтение данных концентрации Метан (CH4)
  double ch4PDK = conversion(MOL_CH4, ch4, T, P);
  
  //JSON Convert
  String jsonString = "{\"data\":\""
                      + API_KEY + "; "
                      + String(T) + "; "
                      + String(H) + "; "
                      + String(P) + "; "
                      + String(co2PDK) + "; "
                      + String(lpg) + "; "
                      + String(coPDK) + "; "
                      + String(ch4PDK) + "; "
                      + String(concentration) + "; "
                      + String(longitude) + "; " // Longitude
                      + String(latitude)// Latitude
                      + "\"}";
//  Serial.print("JSON: "); 
  Serial.println(jsonString);
  delay(500);
}

double conversion (double a, double b, double c, double d) {
  return ( (a * b) / ((22.4 * (273 + c) * 101325000) / (273 * d * 100)));
}
