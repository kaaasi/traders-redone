# Traders.Vga (Arbeitspaket AP2)
Persistenter 320×200-Framebuffer (indiziert) mit den Zeichenoperationen des Originals
(Grafiktreiber 0A96, Hilfen 0A69, Text/Schriften, maskierte Blits).

`Layers/` (R3, Spur `schichten`): Schichten über dem Treiber – Seitenverwaltung `0B67`, Paletten/Blenden `21FD`,
Seitenwechsel `21FD:051F`, Farbregister `2295`; Zustand im DS-Abbild, Warten über `IRetraceWait`.
