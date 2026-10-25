; segment 28DA  file offset 0x2C3A0  size 0x10
00000000  0002              add [bp+si],al
00000002  B131              mov cl,0x31
00000004  0000              add [bx+si],al
00000006  0010              add [bx+si],dl
00000008  E351              jcxz 0x5b
0000000A  0000              add [bx+si],al
0000000C  0010              add [bx+si],dl
0000000E  08                db 0x08
0000000F  53                push bx
