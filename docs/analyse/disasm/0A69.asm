; segment 0A69  file offset 0xDC90  size 0x2D0
00000000  50                push ax
00000001  9A7A05960A        call 0xa96:0x57a
00000006  83C40E            add sp,byte +0xe
00000009  5D                pop bp
0000000A  CB                retf

; ---- 0A69:000B ----
0000000B  55                push bp
0000000C  8BEC              mov bp,sp
0000000E  FF760C            push word [bp+0xc]
00000011  FF7608            push word [bp+0x8]
00000014  8B4606            mov ax,[bp+0x6]
00000017  03460A            add ax,[bp+0xa]
0000001A  48                dec ax
0000001B  50                push ax
0000001C  FF7608            push word [bp+0x8]
0000001F  FF7606            push word [bp+0x6]
00000022  9A3304960A        call 0xa96:0x433
00000027  83C40A            add sp,byte +0xa
0000002A  5D                pop bp
0000002B  CB                retf

; ---- 0A69:002C ----
0000002C  55                push bp
0000002D  8BEC              mov bp,sp
0000002F  FF760C            push word [bp+0xc]
00000032  8B4608            mov ax,[bp+0x8]
00000035  03460A            add ax,[bp+0xa]
00000038  48                dec ax
00000039  50                push ax
0000003A  FF7606            push word [bp+0x6]
0000003D  FF7608            push word [bp+0x8]
00000040  FF7606            push word [bp+0x6]
00000043  9A3304960A        call 0xa96:0x433
00000048  83C40A            add sp,byte +0xa
0000004B  5D                pop bp
0000004C  CB                retf

; ---- 0A69:004D ----
0000004D  55                push bp
0000004E  8BEC              mov bp,sp
00000050  B8FFFF            mov ax,0xffff
00000053  50                push ax
00000054  FF7610            push word [bp+0x10]
00000057  FF760C            push word [bp+0xc]
0000005A  FF760A            push word [bp+0xa]
0000005D  8B4608            mov ax,[bp+0x8]
00000060  034614            add ax,[bp+0x14]
00000063  50                push ax
00000064  8B4606            mov ax,[bp+0x6]
00000067  034612            add ax,[bp+0x12]
0000006A  50                push ax
0000006B  9A2505960A        call 0xa96:0x525
00000070  83C40C            add sp,byte +0xc
00000073  B8FFFF            mov ax,0xffff
00000076  50                push ax
00000077  FF760E            push word [bp+0xe]
0000007A  FF760C            push word [bp+0xc]
0000007D  FF760A            push word [bp+0xa]
00000080  FF7608            push word [bp+0x8]
00000083  FF7606            push word [bp+0x6]
00000086  9A2505960A        call 0xa96:0x525
0000008B  83C40C            add sp,byte +0xc
0000008E  5D                pop bp
0000008F  CB                retf

; ---- 0A69:0090 ----
00000090  55                push bp
00000091  8BEC              mov bp,sp
00000093  8B460C            mov ax,[bp+0xc]
00000096  0B460E            or ax,[bp+0xe]
00000099  7436              jz 0xd1
0000009B  FF7612            push word [bp+0x12]
0000009E  FF7610            push word [bp+0x10]
000000A1  FF760E            push word [bp+0xe]
000000A4  FF760C            push word [bp+0xc]
000000A7  FF7608            push word [bp+0x8]
000000AA  FF760E            push word [bp+0xe]
000000AD  FF760C            push word [bp+0xc]
000000B0  9AE5420000        call 0x0:0x42e5
000000B5  59                pop cx
000000B6  59                pop cx
000000B7  F72EA83C          imul word [0x3ca8]
000000BB  8B560A            mov dx,[bp+0xa]
000000BE  2BD0              sub dx,ax
000000C0  D1EA              shr dx,1
000000C2  8B4606            mov ax,[bp+0x6]
000000C5  40                inc ax
000000C6  03C2              add ax,dx
000000C8  50                push ax
000000C9  9A2505960A        call 0xa96:0x525
000000CE  83C40C            add sp,byte +0xc
000000D1  5D                pop bp
000000D2  CB                retf

; ---- 0A69:00D3 ----
000000D3  55                push bp
000000D4  8BEC              mov bp,sp
000000D6  8B460C            mov ax,[bp+0xc]
000000D9  0B460E            or ax,[bp+0xe]
000000DC  743B              jz 0x119
000000DE  FF7616            push word [bp+0x16]
000000E1  FF7614            push word [bp+0x14]
000000E4  FF7612            push word [bp+0x12]
000000E7  FF7610            push word [bp+0x10]
000000EA  FF760E            push word [bp+0xe]
000000ED  FF760C            push word [bp+0xc]
000000F0  FF7608            push word [bp+0x8]
000000F3  FF760E            push word [bp+0xe]
000000F6  FF760C            push word [bp+0xc]
000000F9  9AE5420000        call 0x0:0x42e5
000000FE  59                pop cx
000000FF  59                pop cx
00000100  F72EA83C          imul word [0x3ca8]
00000104  8B560A            mov dx,[bp+0xa]
00000107  2BD0              sub dx,ax
00000109  D1EA              shr dx,1
0000010B  8B4606            mov ax,[bp+0x6]
0000010E  40                inc ax
0000010F  03C2              add ax,dx
00000111  50                push ax
00000112  0E                push cs
00000113  E837FF            call 0x4d
00000116  83C410            add sp,byte +0x10
00000119  5D                pop bp
0000011A  CB                retf

; ---- 0A69:011B ----
0000011B  55                push bp
0000011C  8BEC              mov bp,sp
0000011E  B8FFFF            mov ax,0xffff
00000121  50                push ax
00000122  FF7612            push word [bp+0x12]
00000125  FF760E            push word [bp+0xe]
00000128  FF760C            push word [bp+0xc]
0000012B  FF760A            push word [bp+0xa]
0000012E  8B4608            mov ax,[bp+0x8]
00000131  034616            add ax,[bp+0x16]
00000134  50                push ax
00000135  8B4606            mov ax,[bp+0x6]
00000138  034614            add ax,[bp+0x14]
0000013B  50                push ax
0000013C  9A7A05960A        call 0xa96:0x57a
00000141  83C40E            add sp,byte +0xe
00000144  B8FFFF            mov ax,0xffff
00000147  50                push ax
00000148  FF7610            push word [bp+0x10]
0000014B  FF760E            push word [bp+0xe]
0000014E  FF760C            push word [bp+0xc]
00000151  FF760A            push word [bp+0xa]
00000154  FF7608            push word [bp+0x8]
00000157  FF7606            push word [bp+0x6]
0000015A  9A7A05960A        call 0xa96:0x57a
0000015F  83C40E            add sp,byte +0xe
00000162  5D                pop bp
00000163  CB                retf

; ---- 0A69:0164 ----
00000164  55                push bp
00000165  8BEC              mov bp,sp
00000167  B8FFFF            mov ax,0xffff
0000016A  50                push ax
0000016B  FF7612            push word [bp+0x12]
0000016E  FF760E            push word [bp+0xe]
00000171  FF760C            push word [bp+0xc]
00000174  FF760A            push word [bp+0xa]
00000177  8B4608            mov ax,[bp+0x8]
0000017A  034616            add ax,[bp+0x16]
0000017D  50                push ax
0000017E  8B4606            mov ax,[bp+0x6]
00000181  034614            add ax,[bp+0x14]
00000184  50                push ax
00000185  9AFB05960A        call 0xa96:0x5fb
0000018A  83C40E            add sp,byte +0xe
0000018D  B8FFFF            mov ax,0xffff
00000190  50                push ax
00000191  FF7610            push word [bp+0x10]
00000194  FF760E            push word [bp+0xe]
00000197  FF760C            push word [bp+0xc]
0000019A  FF760A            push word [bp+0xa]
0000019D  FF7608            push word [bp+0x8]
000001A0  FF7606            push word [bp+0x6]
000001A3  9AFB05960A        call 0xa96:0x5fb
000001A8  83C40E            add sp,byte +0xe
000001AB  5D                pop bp
000001AC  CB                retf

; ---- 0A69:01AD ----
000001AD  55                push bp
000001AE  8BEC              mov bp,sp
000001B0  56                push si
000001B1  8B460A            mov ax,[bp+0xa]
000001B4  0B460C            or ax,[bp+0xc]
000001B7  7474              jz 0x22d
000001B9  33F6              xor si,si
000001BB  EB5F              jmp short 0x21c
000001BD  C45E0A            les bx,[bp+0xa]
000001C0  268A00            mov al,[es:bx+si]
000001C3  A24001            mov [0x140],al
000001C6  9A1B00960A        call 0xa96:0x1b
000001CB  B8FFFF            mov ax,0xffff
000001CE  50                push ax
000001CF  FF760E            push word [bp+0xe]
000001D2  1E                push ds
000001D3  B84001            mov ax,0x140
000001D6  50                push ax
000001D7  FF7608            push word [bp+0x8]
000001DA  8BC6              mov ax,si
000001DC  F72EA83C          imul word [0x3ca8]
000001E0  8B5606            mov dx,[bp+0x6]
000001E3  03D0              add dx,ax
000001E5  52                push dx
000001E6  9A2505960A        call 0xa96:0x525
000001EB  83C40C            add sp,byte +0xc
000001EE  9A0D06FD21        call 0x21fd:0x60d
000001F3  803E400120        cmp byte [0x140],0x20
000001F8  740F              jz 0x209
000001FA  B81600            mov ax,0x16
000001FD  50                push ax
000001FE  B80100            mov ax,0x1
00000201  50                push ax
00000202  9A5400A721        call 0x21a7:0x54
00000207  59                pop cx
00000208  59                pop cx
00000209  9A4502FD21        call 0x21fd:0x245
0000020E  8B4610            mov ax,[bp+0x10]
00000211  99                cwd
00000212  52                push dx
00000213  50                push ax
00000214  9A8501FD21        call 0x21fd:0x185
00000219  59                pop cx
0000021A  59                pop cx
0000021B  46                inc si
0000021C  FF760C            push word [bp+0xc]
0000021F  FF760A            push word [bp+0xa]
00000222  9AE5420000        call 0x0:0x42e5
00000227  59                pop cx
00000228  59                pop cx
00000229  3BC6              cmp ax,si
0000022B  7790              ja 0x1bd
0000022D  837E1200          cmp word [bp+0x12],byte +0x0
00000231  7E0D              jng 0x240
00000233  8B4612            mov ax,[bp+0x12]
00000236  99                cwd
00000237  52                push dx
00000238  50                push ax
00000239  9A8501FD21        call 0x21fd:0x185
0000023E  59                pop cx
0000023F  59                pop cx
00000240  5E                pop si
00000241  5D                pop bp
00000242  CB                retf

; ---- 0A69:0243 ----
00000243  55                push bp
00000244  8BEC              mov bp,sp
00000246  8B460C            mov ax,[bp+0xc]
00000249  0B460E            or ax,[bp+0xe]
0000024C  7438              jz 0x286
0000024E  FF7614            push word [bp+0x14]
00000251  FF7612            push word [bp+0x12]
00000254  FF7610            push word [bp+0x10]
00000257  FF760E            push word [bp+0xe]
0000025A  FF760C            push word [bp+0xc]
0000025D  FF7608            push word [bp+0x8]
00000260  FF760E            push word [bp+0xe]
00000263  FF760C            push word [bp+0xc]
00000266  9AE5420000        call 0x0:0x42e5
0000026B  59                pop cx
0000026C  59                pop cx
0000026D  F72EA83C          imul word [0x3ca8]
00000271  8B560A            mov dx,[bp+0xa]
00000274  2BD0              sub dx,ax
00000276  D1EA              shr dx,1
00000278  8B4606            mov ax,[bp+0x6]
0000027B  40                inc ax
0000027C  03C2              add ax,dx
0000027E  50                push ax
0000027F  0E                push cs
00000280  E82AFF            call 0x1ad
00000283  83C40E            add sp,byte +0xe
00000286  5D                pop bp
00000287  CB                retf

; ---- 0A69:0288 ----
00000288  55                push bp
00000289  8BEC              mov bp,sp
0000028B  56                push si
0000028C  33F6              xor si,si
0000028E  EB42              jmp short 0x2d2
00000290  B8FFFF            mov ax,0xffff
00000293  50                push ax
00000294  FF7614            push word [bp+0x14]
00000297  FF7612            push word [bp+0x12]
0000029A  8BC6              mov ax,si
0000029C  D1E0              shl ax,1
0000029E  D1E0              shl ax,1
000002A0  C45E0C            les bx,[bp+0xc]
000002A3  03D8              add bx,ax
000002A5  26FF7702          push word [es:bx+0x2]
000002A9  26FF37            push word [es:bx]
000002AC  FF760A            push word [bp+0xa]
000002AF  A1A434            mov ax,[0x34a4]
000002B2  BB0500            mov bx,0x5
000002B5  99                cwd
000002B6  F7FB              idiv bx
000002B8  50                push ax
000002B9  A1A434            mov ax,[0x34a4]
000002BC  5A                pop dx
000002BD  03C2              add ax,dx
000002BF  F7EE              imul si
000002C1  8B5608            mov dx,[bp+0x8]
000002C4  03D0              add dx,ax
000002C6  52                push dx
000002C7  FF7606            push word [bp+0x6]
000002CA  0E                push cs
000002CB  E875FF            call 0x243
000002CE  83                db 0x83
000002CF  C4                db 0xc4
