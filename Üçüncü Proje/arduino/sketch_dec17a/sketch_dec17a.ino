#include <Servo.h>
Servo sg90;
Servo sg91;

void setup() 
{
  sg90.attach(12);
  sg91.attach(13);
  
  Serial.begin(9600);
  int a=0;

}

void loop() 
{
  while(1)
  {
    char a = Serial.read();
    if(a=='a')
    { 
      sg90.write(170);     
    }
    else if(a=='b')
    {
      sg90.write(90);      
    }
    else if(a=='c')
    {
      sg90.write(20); 
    }
    else if(a=='d')
    {
      sg91.write(170);
    }
    else if(a=='e')
    {
      sg91.write(90);
    }
    else if(a=='f')
    {
      sg91.write(20);
    }
  } 
}
