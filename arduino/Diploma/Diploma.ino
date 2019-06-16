#include <ArduinoJson.h>

#include <MQ135.h>
#include <MQ9.h>
#include "DHT.h"
#include <SFE_BMP180.h>
#include <Wire.h>

#define MQ135Pin A2// аналоговый выход MQ135 подключен к пину A6 Arduino
#define MQ9Pin A3
#define DHTPIN 3
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
String apiKey = "";
const size_t CAPACITY = JSON_OBJECT_SIZE(11);

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
  /*---------------------------------------------------------------------- ПЫЛЬ*/
  duration = pulseIn(pin, LOW);
  lowpulseoccupancy = lowpulseoccupancy + duration;
  if ((millis() - starttime) > sampletime_ms)
  {

    ratio = lowpulseoccupancy / (sampletime_ms * 10.0); // Integer percentage 0=>100
    concentration = 1.1 * pow(ratio, 3) - 3.8 * pow(ratio, 2) + 520 * ratio + 0.62;
    lowpulseoccupancy = 0;
    starttime = millis();
  }// using spec sheet curve

  /*------------------------------------------------------------------------*/
  if (Serial.available() != 0) {
    byte b = Serial.read();
    double T, P;

    pressure.startTemperature();
    delay(100);
    pressure.getTemperature(T);
    pressure.startPressure(3);
    delay(100);
    pressure.getPressure(P, T);
    Serial.print("Давление: ");
    Serial.print(P * 100); Serial.print(" Pa | ");
    Serial.print(P * 0.75006375541921); Serial.print(" мм рт ст | ");
    Serial.print(P); Serial.println(" гПа");
    Serial.print("Температура: "); Serial.print(T); Serial.println(" *C");

    /*----------------------------------------------------------------------*/

    float H = dht.readHumidity(); // чтение данных влажностb воздуха
    Serial.print("Влажность: "); Serial.print(H); Serial.println(" %"); //Serial.print(T);

    /*----------------------------------------------------------------------*/

    double mol_co2 = 44.01; //молярная масса углекислого газа
    double co2 = mq135.getPPM(); // чтение данных концентрации CO2
    double k = conversion(mol_co2, co2, T, P);

    Serial.print("CO2(Углекислый газ): "); Serial.print(k, 4); Serial.print(" мг/м3 | "); Serial.print(co2); Serial.println(" ppm");

    /*----------------------------------------------------------------------*/

    float mol_lpg = 0; //молярная масса газа пропан-бутан
    float lpg = mq9.readLPG(); // чтение данных концентрации LPG
    Serial.print("LPG(Пропан-бутан): "); Serial.print(lpg); Serial.println(" ppm");

    /*----------------------------------------------------------------------*/

    float mol_co = 28.01;// молярная масса угарного газа
    float co = mq9.readCarbonMonoxide(); // чтение данных концентрации CO
    k = conversion(mol_co, co, T, P);
    Serial.print("CO(Угарный газ): "); Serial.print(k, 4); Serial.print(" мг/м3 | "); Serial.print(co); Serial.println(" ppm");

    /*----------------------------------------------------------------------*/

    float mol_ch4 = 16.04;// молярная масса Метана
    float ch4 = mq9.readMethane(); // чтение данных концентрации Метан (CH4)
    k = conversion(mol_ch4, ch4, T, P);
    Serial.print("CH4(Метан): "); Serial.print(k, 4); Serial.print(" мг/м3 | "); Serial.print(ch4); Serial.println(" ppm");


    /*---------------------------------------------------------------------- ПЫЛЬ*/
    Serial.print("PM10: "); Serial.print(concentration); Serial.println(" pcs/0.01cf");
    /*------------------------------------------------------------------------*/
    
    StaticJsonDocument<CAPACITY> doc;
    JsonObject data = doc.to<JsonObject>();
    data["ApiKey"] = apiKey;
    data["CO2"] = co2;
    data["LPG"] = lpg;
    data["CO"] = co;
    data["ch4"] = ch4;
    data["Dust"] = concentration;
    data["Temp"] = T;
    data["Preassure"] = P;
    data["Hum"] = H;
    data["Latitude"] = true;
    data["Longitude"] = false;

    String dataStr;
    serializeJson(data, dataStr);
    Serial.print("ApiKey: "); Serial.println(apiKey);
    Serial.print("JSON: ");Serial.println(dataStr);
 
    Serial.println("-------------------------------------------------------------------------------");

  }

}

double conversion (double a, double b, double c, double d) {

  return ( (a * b) / ((22.4 * (273 + c) * 101325000) / (273 * d * 100)));

}
