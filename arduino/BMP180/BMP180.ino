#include <SFE_BMP180.h>
#include <Wire.h>

SFE_BMP180 pressure;

void setup(){
    Serial.begin(9600);
    pressure.begin();
}

void loop(){
    double P;
    P = getPressure();
    Serial.println(P, 2); 
    delay(1000);
}

double getPressure(){
    char status;
    double T,P;

    status = pressure.startTemperature();
    if (status != 0){
        // ожидание замера температуры
        delay(status);
        status = pressure.getTemperature(T);
        if (status != 0){
            status = pressure.startPressure(3);
            if (status != 0){
                // ожидание замера давления
                delay(status);
                status = pressure.getPressure(P,T);
                if (status != 0){
                  Serial.println(T,2);
                    return(P);
                }
            }
        }
    }
}
