#include <MQ2.h>
#define MQ2Pin A3
MQ2 mq2( MQ2Pin);
int  serIn;
void setup() {
  Serial.begin(9600); // последовательный порт для отображения данных
  mq2.calibrate();
}

void loop () {
 
  //simple feedback from Arduino  Serial.println("Hello World"); 
  
  // only if there are bytes in the serial buffer execute the following code
  if(Serial.available()) {    
    //inform that Arduino heard you saying something
    serIn = Serial.read();
     //keep reading and printing from serial untill there are bytes in the serial buffer
     while (Serial.available()>0){
      
       Serial.print(mq2.readMethane());
     }
     
    //the serial buffer is over just go to the line (or pass your favorite stop char)               
    Serial.println(mq2.readMethane());
  }
  
  //slows down the visualization in the terminal
  delay(1000);
}


