
# GameBoyCartRW

  

A simple (and incredibly slow) Arduino Nano based Gameboy cart reader/writer.

  

## Important

**Do not insert a GBA Cartridge!**

I don't know if GBA Carts are 5v tolerant, so it's possible that this would damage the cartridge!



**The PCB files are incorrect**! I currently have bodge wires between A6->D3 and A7->D13. This should be fixed in a future revision (If I get around to it)

## Utilities

The UI uses Windows forms

The Firmware project was done using PlatformIO in VSCode

PCB was designed with EasyEDA

Cartridge slot is a replacement [GBA cart slot](https://www.aliexpress.com/item/1005005016059093.html?spm=a2g0o.order_list.order_list_main.41.42031802yNwN0F)

## Features

* Make backups of cartridge ROMs

* Make backups of cartridge SRAM

* Write backups to cartridge SRAM

## Supported Cartridges

As far as I'm aware, all features should with any Gameboy/Gameboy Colour cartridge.

## References

[GBDev.io Pan Docs](https://gbdev.io/pandocs) is an absolutely phenomenal resource with all the information you could possibly want.

I used Inside Gadgets' "[GBCartRead](https://github.com/insidegadgets/GBCartRead)" as a resource when I was having issues.

## License

[MIT](https://choosealicense.com/licenses/mit/)
