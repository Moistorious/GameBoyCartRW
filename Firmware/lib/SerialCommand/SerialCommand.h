#include <Arduino.h>

enum class CommandType {
    InitializeCart, // Set the cart type and stuff in memory (so the other commands work correctly)
    Validate, // Just make sure the Nintendo Logo is correct (Basically, is the cart seated?)
    Reset, // Reset all variables, and reset the cart
    SelectBank,

    ReadByte,
    ReadRange,

    WriteByte,
    WriteRange,
    
    WriteRamByte,
    WriteRamRange,

    GetHeader, // Send full header
    GetTitle, // Send just the title
    
    WriteFullRAM,
    WriteFullROM,

    GetFullROM, // Get the entire ROM
    GetFullRAM, // Get the entire RAM
};


struct SerialCommand
{
    CommandType command;
    uint16_t address;
    uint16_t length;
};
