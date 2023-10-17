#include <Arduino.h>
#ifndef CART_H
#define CART_H

#define CART_TYPE_OFFSET 0x147
#define ROM_SIZE_OFFSET 0x148
#define RAM_SIZE_OFFSET 0x149

#define CART_RESET 2
// #define CART_AUDIO_IN 3 // bodged out of existance...
#define CART_CHIP_SELECT 4
#define CART_READ_ENABLE 5
#define CART_WRITE_ENABLE 6
#define CART_CLOCK 7

#define ADDRESS_REGISTER_DATA 8
#define ADDRESS_REGISTER_ENABLE 9
#define ADDRESS_REGISTER_LATCH 10
#define ADDRESS_REGISTER_CLOCK 11
#define ADDRESS_REGISTER_CLEAR 12

#define CART_DATA_0 14 // A0 ...
#define CART_DATA_1 15
#define CART_DATA_2 16
#define CART_DATA_3 17
#define CART_DATA_4 18
#define CART_DATA_5 19 // A5
#define CART_DATA_6 3
#define CART_DATA_7 13
class CartReader
{
public:
    CartReader();
    static const uint16_t HeaderAddress = 0x100;
    static const uint16_t BankSize = 0x4000;

    bool SanityCheck();
    void SetCurrentAddress(int address);
    byte ReadByte(int address);
    void WriteByte(int address, byte value);
    void WriteRAMByte(int address, byte value);
    void SelectBank(byte cartType, byte bank);
    int ReadRange(uint16_t startAddress, uint8_t *buffer, uint16_t length);
    void WriteRange(uint16_t startAddress, uint8_t *buffer, uint16_t length);
    void WriteRamRange(uint16_t startAddress, uint8_t *buffer, uint16_t length);
    void EnableRAM();
    void DisableRAM();
    void WriteBytes(uint16_t address, byte *buffer, bool RAM = false);
    void Reset();
    void WriteRAM(byte *buffer);
    void init();
    uint16_t GetRamBankSize(byte ramSize, byte cartridgeType);
    uint16_t GetRomBanks(byte romSize);
    byte GetRamBanks(byte ramSize, byte cartridgeType);

    uint16_t RamBankSize;
    uint16_t RomBanks;
    uint16_t RamSize;
    byte RomSize;
    uint32_t FullRomSize;
    byte RamBanks;
    byte RamSizeCode;
    byte CartridgeTypeCode;

};
#endif