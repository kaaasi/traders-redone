# 16-Bit-IDIV: Quotientenüberlauf

Intel-IDIV mit 16-Bit-Divisor berechnet einen vorzeichenbehafteten Quotienten in AX und Rest
in DX. Division durch null oder ein nicht als short darstellbarer Quotient löst Interrupt 0
aus; auch der Rest ist dann nicht verfügbar. Für sign-erweiterte short-Dividenden betrifft
Überlauf ausschließlich -32768 / -1. Int16Math bildet dies mit OverflowException ab;
Division durch null bleibt DivideByZeroException. Normale Division rundet gegen null.

Test: Int16MathTests vergleicht den Fehlerpfad mit F7 FB (IDIV BX) im RefEmu und dem
Interrupt-0-Vektor. Dies prüft Instruktionssemantik, keine neu zugeordnete T.EXE-Routine.
