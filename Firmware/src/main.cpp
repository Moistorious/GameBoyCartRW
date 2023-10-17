#include <Arduino.h>

#include "SerialCommand.h"
#include "CartReader.h"

#define BAUD_RATE 115200
#define BUFFER_SIZE 1024
// #define DEBUG_ENABLE

uint8_t serialBuffer[BUFFER_SIZE];

void SetCurrentAddress(int address);

SerialCommand CurrentCommand;
CartReader Cart;

void GetSerialCommand()
{
    while (Serial.available() <= 0)
    {
    }

    Serial.readBytes((uint8_t *)&CurrentCommand, sizeof(SerialCommand));

    Serial.readBytes(serialBuffer, CurrentCommand.length);
}

void setup()
{
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

    Serial.begin(BAUD_RATE);
    while (!Serial)
    {
        ; // wait for serial port to connect. Needed for native USB
    }
}

void ReadBytesToSerial(uint16_t address, uint16_t length)
{
    uint16_t bytesRead = 0;
    while (bytesRead < length)
    {
        uint16_t bytesToRead = min(BUFFER_SIZE, length - bytesRead);
        Cart.ReadRange(address + bytesRead, serialBuffer, bytesToRead);
        Serial.write(serialBuffer, bytesToRead);
        bytesRead += bytesToRead;
    }
}

void WriteRamBytesFromSerial(uint16_t address, uint16_t length)
{
    uint16_t bytesRead = 0;
    while (bytesRead < length)
    {
        uint16_t bytesToRead = min(BUFFER_SIZE, length - bytesRead);

        Serial.readBytes(serialBuffer, bytesToRead);

        Cart.WriteRamRange(address + bytesRead, serialBuffer, bytesToRead);
        bytesRead += bytesToRead;
    }
}
void GetROM()
{
    ReadBytesToSerial(0x00, Cart.BankSize);
    for (byte bank = 1; bank < Cart.RomBanks; bank++)
    {
        Cart.SelectBank(Cart.CartridgeTypeCode, bank);
        ReadBytesToSerial(0x4000, Cart.BankSize);
    }
}

void GetRAM()
{
    if (Cart.RamBankSize == 0)
    {
        return;
    }
    Cart.EnableRAM();
    for (byte bank = 0; bank < Cart.RamBanks; bank++)
    {
        Cart.WriteByte(0x4000, bank);
        ReadBytesToSerial(0xA000, Cart.RamBankSize);
    }
    Cart.DisableRAM();
}

void WriteRAM()
{
    if (Cart.RamBankSize == 0)
    {
        return;
    }
    Cart.EnableRAM();
    for (byte bank = 0; bank < Cart.RamBanks; bank++)
    {
        Cart.WriteByte(0x4000, bank);
        WriteRamBytesFromSerial(0xA000, Cart.RamBankSize);
    }
    Cart.DisableRAM();
}
/*


    void WriteRAM()
    {
        if (RamBankSize == 0)
        {
            return;
        }
        EnableRAM();

        for (byte currentBank = 0; currentBank < RamBanks; currentBank++)
        {
            WriteByte(0x4000, currentBank);
            byte bankData[bytes(currentBank * RamBankSize)];
            // Initialize bankData as needed
            WriteBytes(0xA000, bankData, true);
        }
        DisableRAM();
    }

*/
bool Sanity = true;
void loop()
{
    GetSerialCommand();

    switch (CurrentCommand.command)
    {
    case CommandType::Reset:
        Cart.Reset();
        break;

    case CommandType::SelectBank:
        Cart.SelectBank(serialBuffer[0], serialBuffer[1]);
        break;

    case CommandType::ReadByte:
        CurrentCommand.length = 1; // fall through to Read Range
    case CommandType::ReadRange:
        Cart.ReadRange(CurrentCommand.address, serialBuffer, CurrentCommand.length);
        Serial.write(serialBuffer, CurrentCommand.length);
        break;

    case CommandType::WriteByte:
        CurrentCommand.length = 1; // fall through to Write Range
    case CommandType::WriteRange:
        Cart.WriteRange(CurrentCommand.address, serialBuffer, CurrentCommand.length);
        Serial.write("Done");
        break;

    case CommandType::WriteRamByte:
        CurrentCommand.length = 1; // fall through to Write Ram Range
    case CommandType::WriteRamRange:
        Cart.WriteRamRange(CurrentCommand.address, serialBuffer, CurrentCommand.length);
        Serial.write("Done");
        break;

    case CommandType::GetHeader:
        Cart.ReadRange(0x100, serialBuffer, 0x150);
        Serial.write(serialBuffer, 0x150);
        break;

    case CommandType::GetTitle:
        Cart.ReadRange(0x134, serialBuffer, 0x10);
        Serial.write(serialBuffer, 0x10);
        break;

    case CommandType::WriteFullRAM:
        WriteRAM();
        break;

    case CommandType::WriteFullROM:
        break;
    case CommandType::GetFullROM:
        GetROM();
        break;
    case CommandType::GetFullRAM:
        GetRAM();
        break;
    case CommandType::InitializeCart:
        Cart.init();
        //        Sanity = Cart.SanityCheck();
        break;
    case CommandType::Validate:
        break;
    }
}
