; segment 21A7  file offset 0x25070  size 0x80
00000000  33DB              xor bx,bx
00000002  CD64              int 0x64
00000004  B80500            mov ax,0x5
00000007  33DB              xor bx,bx
00000009  CD64              int 0x64
0000000B  5D                pop bp
0000000C  CB                retf

; ---- 21A7:000D ----
0000000D  55                push bp
0000000E  8BEC              mov bp,sp
00000010  833E100901        cmp word [0x910],byte +0x1
00000015  7516              jnz 0x2d
00000017  9AFE060000        call 0x0:0x6fe
0000001C  B80000            mov ax,0x0
0000001F  BA3207            mov dx,0x732
00000022  50                push ax
00000023  52                push dx
00000024  9A1B04C627        call 0x27c6:0x41b
00000029  59                pop cx
0000002A  59                pop cx
0000002B  EB0F              jmp short 0x3c
0000002D  B85028            mov ax,0x2850
00000030  BA6100            mov dx,0x61
00000033  50                push ax
00000034  52                push dx
00000035  9A1B04C627        call 0x27c6:0x41b
0000003A  59                pop cx
0000003B  59                pop cx
0000003C  B80100            mov ax,0x1
0000003F  EB00              jmp short 0x41
00000041  5D                pop bp
00000042  CB                retf

; ---- 21A7:0043 ----
00000043  55                push bp
00000044  8BEC              mov bp,sp
00000046  833E100901        cmp word [0x910],byte +0x1
0000004B  7505              jnz 0x52
0000004D  9A24070000        call 0x0:0x724
00000052  5D                pop bp
00000053  CB                retf

; ---- 21A7:0054 ----
00000054  55                push bp
00000055  8BEC              mov bp,sp
00000057  833EA51000        cmp word [0x10a5],byte +0x0
0000005C  7409              jz 0x67
0000005E  9A53018B21        call 0x218b:0x153
00000063  0E                push cs
00000064  E8A6FF            call 0xd
00000067  833EA71000        cmp word [0x10a7],byte +0x0
0000006C  7502              jnz 0x70
0000006E  EB1B              jmp short 0x8b
00000070  833E100901        cmp word [0x910],byte +0x1
00000075  750B              jnz 0x82
00000077  FF7608            push word [bp+0x8]
0000007A  9A50070000        call 0x0:0x750
0000007F  59                pop cx
