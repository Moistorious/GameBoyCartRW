#include <Arduino.h>

#define ADDRESS_REGISTER_CLEAR 9
#define ADDRESS_REGISTER_CLOCK 10
#define ADDRESS_REGISTER_LATCH 11
#define ADDRESS_REGISTER_ENABLE 12
#define ADDRESS_REGISTER_DATA 13

#define CART_CLOCK A0
#define CART_WRITE_ENABLE A1
#define CART_READ_ENABLE A2
#define CART_CHIP_SELECT A3

#define CART_DATA_0 8
#define CART_DATA_1 7
#define CART_DATA_2 6
#define CART_DATA_3 5
#define CART_DATA_4 4
#define CART_DATA_5 3
#define CART_DATA_6 2
#define CART_DATA_7 A5
// header: $$014F

void setup()
{
  Serial.begin(9600);

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

  digitalWrite(ADDRESS_REGISTER_CLEAR, HIGH);
  digitalWrite(ADDRESS_REGISTER_CLOCK, LOW);
  digitalWrite(ADDRESS_REGISTER_LATCH, LOW);
  digitalWrite(ADDRESS_REGISTER_ENABLE, LOW);
  digitalWrite(ADDRESS_REGISTER_DATA, LOW);
}

bool read = true;

void SetCurrentAddress(int address)
{

  digitalWrite(ADDRESS_REGISTER_LATCH, LOW);
  shiftOut(ADDRESS_REGISTER_DATA, ADDRESS_REGISTER_CLOCK, MSBFIRST, (address >> 8));
  shiftOut(ADDRESS_REGISTER_DATA, ADDRESS_REGISTER_CLOCK, MSBFIRST, (address & 0xFF));
  digitalWrite(ADDRESS_REGISTER_LATCH, HIGH);
  delayMicroseconds(50);
}

byte ReadByte(int address)
{
  digitalWrite(CART_READ_ENABLE, LOW);
  digitalWrite(CART_WRITE_ENABLE, HIGH);
  byte currentByte = 0;
  SetCurrentAddress(address);
  bitWrite(currentByte, 0, digitalRead(CART_DATA_0));
  bitWrite(currentByte, 1, digitalRead(CART_DATA_1));
  bitWrite(currentByte, 2, digitalRead(CART_DATA_2));
  bitWrite(currentByte, 3, digitalRead(CART_DATA_3));
  bitWrite(currentByte, 4, digitalRead(CART_DATA_4));
  bitWrite(currentByte, 5, digitalRead(CART_DATA_5));
  bitWrite(currentByte, 6, digitalRead(CART_DATA_6));
  bitWrite(currentByte, 7, digitalRead(CART_DATA_7));
  return currentByte;
}

void WriteByte(int address, byte value)
{
  pinMode(CART_DATA_0, OUTPUT);
  pinMode(CART_DATA_1, OUTPUT);
  pinMode(CART_DATA_2, OUTPUT);
  pinMode(CART_DATA_3, OUTPUT);
  pinMode(CART_DATA_4, OUTPUT);
  pinMode(CART_DATA_5, OUTPUT);
  pinMode(CART_DATA_6, OUTPUT);
  pinMode(CART_DATA_7, OUTPUT);

  SetCurrentAddress(address);

  digitalWrite(CART_DATA_0, bitRead(value, 0));
  digitalWrite(CART_DATA_1, bitRead(value, 1));
  digitalWrite(CART_DATA_2, bitRead(value, 2));
  digitalWrite(CART_DATA_3, bitRead(value, 3));
  digitalWrite(CART_DATA_4, bitRead(value, 4));
  digitalWrite(CART_DATA_5, bitRead(value, 5));
  digitalWrite(CART_DATA_6, bitRead(value, 6));
  digitalWrite(CART_DATA_7, bitRead(value, 7));

  // Setup the address and data before we toggle the read/write pins instead of the other way around
  digitalWrite(CART_READ_ENABLE, HIGH);
  digitalWrite(CART_WRITE_ENABLE, LOW);
  delayMicroseconds(50);
  digitalWrite(CART_READ_ENABLE, LOW);
  digitalWrite(CART_WRITE_ENABLE, HIGH);

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
  if (cartType >= 5)
  {                          // MBC2 and above
    WriteByte(0x2100, bank); // Set ROM bank
  }
  else
  {
    WriteByte(0x6000, 0);           // Set ROM Mode
    WriteByte(0x4000, bank >> 5);   // Set bits 5 & 6 (01100000) of ROM bank
    WriteByte(0x2000, bank & 0x1F); // Set bits 0 & 4 (00011111) of ROM bank
  }
}

int Read(uint16_t startAddress, byte *buffer, uint16_t length)
{
  for (uint16_t addressOffset = 0; addressOffset < length; addressOffset++)
  {
    buffer[addressOffset] = ReadByte(startAddress + addressOffset);
  }
  return length;
}

String GetSerialCommand()
{
  while (Serial.available() <= 0)
  {
  }
  String command = Serial.readString();
  command.trim();
  return command;
}
const uint16_t HEADERADDRESS = 0x100;
const uint16_t HEADERSIZE = 0x4f;
uint8_t nintendoLogo[] = {0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B, 0x03, 0x73, 0x00, 0x83, 0x00, 0x0C, 0x00, 0x0D,
                          0x00, 0x08, 0x11, 0x1F, 0x88, 0x89, 0x00, 0x0E, 0xDC, 0xCC, 0x6E, 0xE6, 0xDD, 0xDD, 0xD9, 0x99,
                          0xBB, 0xBB, 0x67, 0x63, 0x6E, 0x0E, 0xEC, 0xCC, 0xDD, 0xDC, 0x99, 0x9F, 0xBB, 0xB9, 0x33, 0x3E};

bool CheckNintendoLogo()
{
  // Nintendo Logo Check

  uint8_t logoCheck = 1;
  uint8_t x = 0;
  for (uint16_t romAddress = 0x0104; romAddress <= 0x133; romAddress++)
  {
    if (nintendoLogo[x] != ReadByte(romAddress))
    {
      logoCheck = 0;
      return false;
    }
    x++;
  }
  return true;
}

void loop()
{
  String command;

  byte buffer[1024];
  byte cartridgeType = ReadByte(0x0147);
  uint16_t romSize = ReadByte(0x0148);
  uint16_t romBanks = 0;
  uint16_t ramSize = ReadByte(0x0149);
  uint16_t ramBanks = 0;
  uint16_t ramEndAddress = 0;
  uint16_t address;
  uint16_t bytesRead;
  
  romBanks = 2; 
    if (romSize > 0)
    {
      romBanks = 2 << romSize;
    }

  // RAM banks
  ramBanks = 0; // Default 0K RAM
  if (cartridgeType == 6)
  {
    ramBanks = 1;
  }
  switch(ramSize){
    case 2:
    ramBanks = 1;
    break;
    case 3:
    ramBanks = 4;
    break;
    case 4:
    ramBanks = 16;
    break;
    case 5:
    ramBanks = 8;
    break;
  }

  // RAM end address
  if (cartridgeType == 6)
  {
    ramEndAddress = 0xA1FF;
  } // MBC2 512bytes (nibbles)
  if (ramSize == 1)
  {
    ramEndAddress = 0xA7FF;
  } // 2K RAM
  if (ramSize > 1)
  {
    ramEndAddress = 0xBFFF;
  } // 8K RAM

  command = GetSerialCommand();
  if (!CheckNintendoLogo())
  {
    Serial.println("READ ERROR");
    while (true)
      ;
  }
  if (command == "HEADER")
  {
    uint32_t sizo = HEADERSIZE;
    Serial.write((byte *)&sizo, 4);
    bytesRead = Read(HEADERADDRESS, buffer, HEADERSIZE);
    Serial.write(buffer, HEADERSIZE);
  }
  else if (command == "DUMPROM")
  {


    uint32_t RomSize = (uint32_t)romBanks * (uint32_t)(0x7FFF - 0x4000);
    Serial.write((byte *)&RomSize, 4);

    uint16_t endAddress = 0x7FFF;
    for (int bank = 1; bank < romBanks; bank++)
    {
      SelectBank(cartridgeType, bank);
      address = (bank > 1) ? 0x4000 : 0;

      while (address < endAddress)
      {
        bytesRead = Read(address, buffer, min(sizeof(buffer), (endAddress + 1) - address));
        Serial.write(buffer, bytesRead);
        address += bytesRead;
      }
    }
  }
  else
  {
    Serial.println("Boy, what is you doin?");
  }
  command = "";
}
