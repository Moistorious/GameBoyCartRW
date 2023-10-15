#include <Arduino.h>
#include "CartReader.h"

    const uint16_t HeaderAddress = 0x100;
    const uint16_t BankSize = 0x4000;
    const uint16_t LogoAddress = 0x133;

    CartReader::CartReader(){
        RamBankSize = 0;
        RomBanks = 0;
        RamSize = 0;
        RomSize = 0;
        FullRomSize = 0;
        RamBanks = 0;
        RamSizeCode = 0;
        CartridgeTypeCode = 0;
    }

    byte nintendoLogo[] = {0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B, 0x03, 0x73, 0x00, 0x83, 0x00, 0x0C, 0x00, 0x0D,
                           0x00, 0x08, 0x11, 0x1F, 0x88, 0x89, 0x00, 0x0E, 0xDC, 0xCC, 0x6E, 0xE6, 0xDD, 0xDD, 0xD9, 0x99,
                           0xBB, 0xBB, 0x67, 0x63, 0x6E, 0x0E, 0xEC, 0xCC, 0xDD, 0xDC, 0x99, 0x9F, 0xBB, 0xB9, 0x33, 0x3E};

    bool CartReader::SanityCheck()
    {
        for (unsigned long long i = 0; i < sizeof(nintendoLogo); i++)
        {
            
            if (ReadByte(LogoAddress + i) != nintendoLogo[i])
            {
                return false;
            }
        }
        return true;
    }

    void CartReader::SetCurrentAddress(int address)
    {
        digitalWrite(ADDRESS_REGISTER_LATCH, LOW);
        shiftOut(ADDRESS_REGISTER_DATA, ADDRESS_REGISTER_CLOCK, MSBFIRST, (address >> 8));
        shiftOut(ADDRESS_REGISTER_DATA, ADDRESS_REGISTER_CLOCK, MSBFIRST, (address & 0xFF));
        digitalWrite(ADDRESS_REGISTER_LATCH, HIGH);
    }

    byte CartReader::ReadByte(int address)
    {
        digitalWrite(CART_CHIP_SELECT, LOW);
        digitalWrite(CART_READ_ENABLE, LOW);

        SetCurrentAddress(address);
        byte currentByte = 0;
        // PD2 PD3

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

    void CartReader::WriteByte(int address, byte value)
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
        delayMicroseconds(1);
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

    void CartReader::WriteRAMByte(int address, byte value)
    {
        digitalWrite(CART_CHIP_SELECT, LOW);
        WriteByte(address, value);
        delayMicroseconds(3);
        digitalWrite(CART_CHIP_SELECT, HIGH);
    }

    void CartReader::SelectBank(byte cartType, byte bank)
    {
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

    int CartReader::ReadRange(uint16_t startAddress, uint8_t *buffer, uint16_t length)
    {
        for (uint16_t addressOffset = 0; addressOffset < length; addressOffset++)
        {
            buffer[addressOffset] = ReadByte(startAddress + addressOffset);
        }
        return length;
    }

void CartReader::WriteRange(uint16_t startAddress, uint8_t *buffer, uint16_t length){
    for (uint16_t i = 0; i < length; i++)
    {
        WriteByte(startAddress + i, buffer[i]);
    }
}

void CartReader::WriteRamRange(uint16_t startAddress, uint8_t *buffer, uint16_t length){
    for (uint16_t i = 0; i < length; i++)
    {
        WriteRAMByte(startAddress + i, buffer[i]);
    }
}

void CartReader::Reset()
{
    digitalWrite(CART_RESET, LOW);
    delayMicroseconds(10);
    digitalWrite(CART_RESET, HIGH);
    SetCurrentAddress(0);
    digitalWrite(CART_CHIP_SELECT, HIGH);
    digitalWrite(CART_WRITE_ENABLE, HIGH);
    digitalWrite(CART_READ_ENABLE, HIGH);
}

    void CartReader::EnableRAM()
    {
        ReadByte(0x134); // Hack? needed?
        if (this->CartridgeTypeCode <= 4)
        {
            WriteByte(0x6000, 1); // RAM mode
        }

        WriteByte(0x0000, 0x0A); // MBC?
    }

    void CartReader::DisableRAM()
    {
        WriteByte(0x0000, 0x00);
    }

    void CartReader::init()
    {
        RamBanks = 0;
        RamBankSize = 0;
        RomBanks = 0;
        CartridgeTypeCode = ReadByte(CART_TYPE_OFFSET);
        RamSizeCode = ReadByte(RAM_SIZE_OFFSET);

        // header starts at 0x100, so to get 0x143, we'll just read it 0x43

        RamBanks = GetRamBanks(RamSizeCode, CartridgeTypeCode);
        RamBankSize = GetRamBankSize(RamSizeCode, CartridgeTypeCode);
        RomBanks = GetRomBanks(RamSizeCode);

        FullRomSize = RomBanks * BankSize;
        RamSize = RamBankSize * RamBanks;
    }

    uint16_t CartReader::GetRamBankSize(byte ramSize, byte cartridgeType)
    {
        uint16_t ramBankSize = 0;
        if (cartridgeType == 6)
        {
            ramBankSize = 0x200;
        }
        if (ramSize == 1)
        {
            ramBankSize = 0x800;
        }
        if (ramSize > 1)
        {
            ramBankSize = 0x2000;
        }
        return ramBankSize;
    }

    uint16_t CartReader::GetRomBanks(byte romSize)
    {
        return romSize > 0 ? uint16_t(2 << romSize) : 2;
    }

    byte CartReader::GetRamBanks(byte ramSize, byte cartridgeType)
    {
        byte ramBanks = 0;

        if (cartridgeType == 6)
        {
            ramBanks = 1;
        }
        switch (ramSize)
        {
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
        return ramBanks;
    }
