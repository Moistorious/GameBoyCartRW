#include <Arduino.h>

#define CART_RESET 2
//#define CART_AUDIO_IN 3
#define CART_CHIP_SELECT 4
#define CART_READ_ENABLE 5
#define CART_WRITE_ENABLE 6
#define CART_CLOCK 7

#define ADDRESS_REGISTER_DATA 8
#define ADDRESS_REGISTER_ENABLE 9
#define ADDRESS_REGISTER_LATCH 10
#define ADDRESS_REGISTER_CLOCK 11
#define ADDRESS_REGISTER_CLEAR 12


#define CART_DATA_0 14 //A0 ...
#define CART_DATA_1 15
#define CART_DATA_2 16
#define CART_DATA_3 17
#define CART_DATA_4 18
#define CART_DATA_5 19 // A5
#define CART_DATA_6 3
#define CART_DATA_7 13

void SetCurrentAddress(int address);
struct SerialCommand
{
    uint8_t write;
    uint16_t address;
    uint8_t length;
    uint8_t *data;
};

SerialCommand CurrentCommand;

uint8_t serialBuffer[255];
void GetSerialCommand()
{
        while (Serial.available() <= 0){/***/}

        Serial.readBytes((uint8_t*)&CurrentCommand, sizeof(SerialCommand) -2);

        Serial.readBytes(CurrentCommand.data, CurrentCommand.length);
}

void setup()
{
    Serial.begin(9600);

    CurrentCommand.data = (uint8_t*)&serialBuffer;
    // for board bodge
    pinMode(A6, INPUT);
    pinMode(A7, INPUT);

    pinMode(ADDRESS_REGISTER_CLEAR, OUTPUT);
    pinMode(ADDRESS_REGISTER_CLOCK, OUTPUT);
    pinMode(ADDRESS_REGISTER_LATCH, OUTPUT);
    pinMode(ADDRESS_REGISTER_ENABLE, OUTPUT);
    pinMode(ADDRESS_REGISTER_DATA, OUTPUT);

    pinMode(CART_DATA_0, INPUT);
    pinMode(CART_DATA_1, INPUT);
    pinMode(CART_DATA_2, INPUT);
    pinMode(CART_DATA_3, INPUT);
    pinMode(CART_DATA_4, INPUT);
    pinMode(CART_DATA_5, INPUT);
    pinMode(CART_DATA_6, INPUT);
    pinMode(CART_DATA_7, INPUT);

    pinMode(CART_CLOCK, OUTPUT);
    pinMode(CART_WRITE_ENABLE, OUTPUT);
    pinMode(CART_READ_ENABLE, OUTPUT);
    pinMode(CART_CHIP_SELECT, OUTPUT);
    pinMode(CART_RESET, OUTPUT);
    digitalWrite(CART_RESET, HIGH);

    digitalWrite(ADDRESS_REGISTER_CLEAR, HIGH);
    digitalWrite(ADDRESS_REGISTER_CLOCK, LOW);
    digitalWrite(ADDRESS_REGISTER_LATCH, LOW);
    digitalWrite(ADDRESS_REGISTER_ENABLE, LOW);
    digitalWrite(ADDRESS_REGISTER_DATA, LOW);
    Serial.begin(9600);

    while (!Serial)
    {
        ; // wait for serial port to connect. Needed for native USB
    }
}

bool read = true;

void SetCurrentAddress(int address)
{
    digitalWrite(ADDRESS_REGISTER_LATCH, LOW);
    shiftOut(ADDRESS_REGISTER_DATA, ADDRESS_REGISTER_CLOCK, MSBFIRST, (address >> 8));
    shiftOut(ADDRESS_REGISTER_DATA, ADDRESS_REGISTER_CLOCK, MSBFIRST, (address & 0xFF));
    digitalWrite(ADDRESS_REGISTER_LATCH, HIGH);
}

byte ReadByte(int address)
{
    digitalWrite(CART_CHIP_SELECT, LOW);
    digitalWrite(CART_READ_ENABLE, LOW);
    
    SetCurrentAddress(address);
    byte currentByte = 0;
    //PD2 PD3

    bitWrite(currentByte, 0, digitalRead(CART_DATA_0));
    bitWrite(currentByte, 1, digitalRead(CART_DATA_1));
    bitWrite(currentByte, 2, digitalRead(CART_DATA_2));
    bitWrite(currentByte, 3, digitalRead(CART_DATA_3));
    bitWrite(currentByte, 4, digitalRead(CART_DATA_4));
    bitWrite(currentByte, 5, digitalRead(CART_DATA_5));
    bitWrite(currentByte, 6, digitalRead(CART_DATA_6));
    bitWrite(currentByte, 7, digitalRead(CART_DATA_7));

    digitalWrite(CART_READ_ENABLE, HIGH);
    digitalWrite(CART_CHIP_SELECT, HIGH);

    return currentByte;
}

void WriteByte(int address, byte value)
{
    // TODO: use registers directly...
    pinMode(CART_DATA_0, OUTPUT);
    pinMode(CART_DATA_1, OUTPUT);
    pinMode(CART_DATA_2, OUTPUT);
    pinMode(CART_DATA_3, OUTPUT);
    pinMode(CART_DATA_4, OUTPUT);
    pinMode(CART_DATA_5, OUTPUT);
    pinMode(CART_DATA_6, OUTPUT);
    pinMode(CART_DATA_7, OUTPUT);

    SetCurrentAddress(address);

    // Write out data (excluding highest 2 bits)
    PORTC |= value & 0b00111111;

    digitalWrite(CART_DATA_6, bitRead(value, 6));
    digitalWrite(CART_DATA_7, bitRead(value, 7));

    digitalWrite(CART_WRITE_ENABLE, LOW);
    digitalWrite(CART_WRITE_ENABLE, HIGH);

    // TODO: use registers directly...

    pinMode(CART_DATA_0, INPUT);
    pinMode(CART_DATA_1, INPUT);
    pinMode(CART_DATA_2, INPUT);
    pinMode(CART_DATA_3, INPUT);
    pinMode(CART_DATA_4, INPUT);
    pinMode(CART_DATA_5, INPUT);
    pinMode(CART_DATA_6, INPUT);
    pinMode(CART_DATA_7, INPUT);
}

void SelectBank(byte cartType, byte bank)
{
    
    digitalWrite(CART_READ_ENABLE, HIGH);
    digitalWrite(CART_CHIP_SELECT, HIGH);

    if (cartType >= 5)
    {                            // MBC2 and above
        WriteByte(0x2100, bank); // Set ROM bank
    }
    else
    {
        WriteByte(0x6000, 0);
        WriteByte(0x4000, bank >> 5);
        WriteByte(0x2000, bank & 0x1F);
    }
}

int Read(uint16_t startAddress, uint8_t *buffer, uint8_t length)
{
    for (uint16_t addressOffset = 0; addressOffset < length; addressOffset++)
    {
        buffer[addressOffset] = ReadByte(startAddress + addressOffset);
    }
    return length;
}

void loop()
{
    GetSerialCommand();
    if(!CurrentCommand.write){      
        Read(CurrentCommand.address, serialBuffer, CurrentCommand.length);
        Serial.write(serialBuffer, CurrentCommand.length);
    }else if(CurrentCommand.address == 0xFFFF){
        SelectBank(CurrentCommand.data[0], CurrentCommand.data[1]);
    }else{
        for(int i=0; i < CurrentCommand.length; i++){
            WriteByte(CurrentCommand.address, CurrentCommand.data[i]);
        }
        Serial.write("Done");
    }
}
