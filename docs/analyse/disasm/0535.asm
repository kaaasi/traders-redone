; segment 0535  file offset 0x8950  size 0xD40
00000000  A37100            mov [0x71],ax
00000003  A17615            mov ax,[0x1576]
00000006  A36F00            mov [0x6f],ax
00000009  C3                ret

; ---- 0535:000A ----
0000000A  55                push bp
0000000B  8BEC              mov bp,sp
0000000D  B83800            mov ax,0x38
00000010  50                push ax
00000011  33C0              xor ax,ax
00000013  50                push ax
00000014  9A5400A721        call 0x21a7:0x54
00000019  59                pop cx
0000001A  59                pop cx
0000001B  B83900            mov ax,0x39
0000001E  50                push ax
0000001F  B80100            mov ax,0x1
00000022  50                push ax
00000023  9A5400A721        call 0x21a7:0x54
00000028  59                pop cx
00000029  59                pop cx
0000002A  B83A00            mov ax,0x3a
0000002D  50                push ax
0000002E  B80200            mov ax,0x2
00000031  50                push ax
00000032  9A5400A721        call 0x21a7:0x54
00000037  59                pop cx
00000038  59                pop cx
00000039  9AFD0B3505        call 0x535:0xbfd
0000003E  9AB4003505        call 0x535:0xb4
00000043  9A880C3505        call 0x535:0xc88
00000048  C606E63C04        mov byte [0x3ce6],0x4
0000004D  1E                push ds
0000004E  B84C0E            mov ax,0xe4c
00000051  50                push ax
00000052  9A6C02FD21        call 0x21fd:0x26c
00000057  59                pop cx
00000058  59                pop cx
00000059  9A8D0B3505        call 0x535:0xb8d
0000005E  9A2903FD21        call 0x21fd:0x329
00000063  9A1B00960A        call 0xa96:0x1b
00000068  9A2B013505        call 0x535:0x12b
0000006D  C606AA3700        mov byte [0x37aa],0x0
00000072  EB0E              jmp short 0x82
00000074  9A7204AF21        call 0x21af:0x472
00000079  9A3A063505        call 0x535:0x63a
0000007E  FE06AA37          inc byte [0x37aa]
00000082  803EAA3704        cmp byte [0x37aa],0x4
00000087  7CEB              jl 0x74
00000089  33C0              xor ax,ax
0000008B  50                push ax
0000008C  33C0              xor ax,ax
0000008E  50                push ax
0000008F  9A5400A721        call 0x21a7:0x54
00000094  59                pop cx
00000095  59                pop cx
00000096  33C0              xor ax,ax
00000098  50                push ax
00000099  B80100            mov ax,0x1
0000009C  50                push ax
0000009D  9A5400A721        call 0x21a7:0x54
000000A2  59                pop cx
000000A3  59                pop cx
000000A4  33C0              xor ax,ax
000000A6  50                push ax
000000A7  B80200            mov ax,0x2
000000AA  50                push ax
000000AB  9A5400A721        call 0x21a7:0x54
000000B0  59                pop cx
000000B1  59                pop cx
000000B2  5D                pop bp
000000B3  CB                retf

; ---- 0535:00B4 ----
000000B4  55                push bp
000000B5  8BEC              mov bp,sp
000000B7  C606AA3700        mov byte [0x37aa],0x0
000000BC  EB64              jmp short 0x122
000000BE  9A1B00960A        call 0xa96:0x1b
000000C3  B000              mov al,0x0
000000C5  50                push ax
000000C6  9A1E033505        call 0x535:0x31e
000000CB  59                pop cx
000000CC  9A0D06FD21        call 0x21fd:0x60d
000000D1  A0AA37            mov al,[0x37aa]
000000D4  98                cbw
000000D5  8BD8              mov bx,ax
000000D7  C687B2430E        mov byte [bx+0x43b2],0xe
000000DC  A0AA37            mov al,[0x37aa]
000000DF  98                cbw
000000E0  D1E0              shl ax,1
000000E2  50                push ax
000000E3  A0AA37            mov al,[0x37aa]
000000E6  98                cbw
000000E7  8BD8              mov bx,ax
000000E9  8A874408          mov al,[bx+0x844]
000000ED  B400              mov ah,0x0
000000EF  5B                pop bx
000000F0  89875230          mov [bx+0x3052],ax
000000F4  A0AA37            mov al,[0x37aa]
000000F7  98                cbw
000000F8  D1E0              shl ax,1
000000FA  50                push ax
000000FB  A0AA37            mov al,[0x37aa]
000000FE  98                cbw
000000FF  8BD8              mov bx,ax
00000101  8A874808          mov al,[bx+0x848]
00000105  B400              mov ah,0x0
00000107  5B                pop bx
00000108  89875E30          mov [bx+0x305e],ax
0000010C  A0AA37            mov al,[0x37aa]
0000010F  98                cbw
00000110  8BD8              mov bx,ax
00000112  80BF400801        cmp byte [bx+0x840],0x1
00000117  7505              jnz 0x11e
00000119  9A3B00C812        call 0x12c8:0x3b
0000011E  FE06AA37          inc byte [0x37aa]
00000122  803EAA3704        cmp byte [0x37aa],0x4
00000127  7C95              jl 0xbe
00000129  5D                pop bp
0000012A  CB                retf

; ---- 0535:012B ----
0000012B  55                push bp
0000012C  8BEC              mov bp,sp
0000012E  E93801            jmp 0x269
00000131  9A1F05FD21        call 0x21fd:0x51f
00000136  FF362E08          push word [0x82e]
0000013A  9A1C07CD23        call 0x23cd:0x71c
0000013F  59                pop cx
00000140  9AA806960A        call 0xa96:0x6a8
00000145  9A1B00960A        call 0xa96:0x1b
0000014A  9A4502FD21        call 0x21fd:0x245
0000014F  9A3905C812        call 0x12c8:0x539
00000154  9A0E009E0E        call 0xe9e:0xe
00000159  C606AA3700        mov byte [0x37aa],0x0
0000015E  E9EA00            jmp 0x24b
00000161  A0AA37            mov al,[0x37aa]
00000164  98                cbw
00000165  8BD8              mov bx,ax
00000167  8A87B243          mov al,[bx+0x43b2]
0000016B  B400              mov ah,0x0
0000016D  3D0D00            cmp ax,0xd
00000170  7408              jz 0x17a
00000172  3D0E00            cmp ax,0xe
00000175  7450              jz 0x1c7
00000177  E9CD00            jmp 0x247
0000017A  B81A00            mov ax,0x1a
0000017D  50                push ax
0000017E  9AE10A3505        call 0x535:0xae1
00000183  59                pop cx
00000184  A0AA37            mov al,[0x37aa]
00000187  98                cbw
00000188  8BD8              mov bx,ax
0000018A  80BFF04300        cmp byte [bx+0x43f0],0x0
0000018F  740A              jz 0x19b
00000191  B8FFFF            mov ax,0xffff
00000194  50                push ax
00000195  9A7A023505        call 0x535:0x27a
0000019A  59                pop cx
0000019B  A0AA37            mov al,[0x37aa]
0000019E  98                cbw
0000019F  8BD8              mov bx,ax
000001A1  80BFBE4400        cmp byte [bx+0x44be],0x0
000001A6  740A              jz 0x1b2
000001A8  B80100            mov ax,0x1
000001AB  50                push ax
000001AC  9A7A023505        call 0x535:0x27a
000001B1  59                pop cx
000001B2  A0AA37            mov al,[0x37aa]
000001B5  98                cbw
000001B6  8BD8              mov bx,ax
000001B8  80BF944800        cmp byte [bx+0x4894],0x0
000001BD  7405              jz 0x1c4
000001BF  9AC1023505        call 0x535:0x2c1
000001C4  E98000            jmp 0x247
000001C7  A0AA37            mov al,[0x37aa]
000001CA  98                cbw
000001CB  8BD8              mov bx,ax
000001CD  80BF9D4300        cmp byte [bx+0x439d],0x0
000001D2  740E              jz 0x1e2
000001D4  B8FFFF            mov ax,0xffff
000001D7  50                push ax
000001D8  33C0              xor ax,ax
000001DA  50                push ax
000001DB  9A60033505        call 0x535:0x360
000001E0  59                pop cx
000001E1  59                pop cx
000001E2  A0AA37            mov al,[0x37aa]
000001E5  98                cbw
000001E6  8BD8              mov bx,ax
000001E8  80BFBA4400        cmp byte [bx+0x44ba],0x0
000001ED  740E              jz 0x1fd
000001EF  B80100            mov ax,0x1
000001F2  50                push ax
000001F3  33C0              xor ax,ax
000001F5  50                push ax
000001F6  9A60033505        call 0x535:0x360
000001FB  59                pop cx
000001FC  59                pop cx
000001FD  A0AA37            mov al,[0x37aa]
00000200  98                cbw
00000201  8BD8              mov bx,ax
00000203  80BFF04300        cmp byte [bx+0x43f0],0x0
00000208  740E              jz 0x218
0000020A  33C0              xor ax,ax
0000020C  50                push ax
0000020D  B8FFFF            mov ax,0xffff
00000210  50                push ax
00000211  9A60033505        call 0x535:0x360
00000216  59                pop cx
00000217  59                pop cx
00000218  A0AA37            mov al,[0x37aa]
0000021B  98                cbw
0000021C  8BD8              mov bx,ax
0000021E  80BFBE4400        cmp byte [bx+0x44be],0x0
00000223  740E              jz 0x233
00000225  33C0              xor ax,ax
00000227  50                push ax
00000228  B80100            mov ax,0x1
0000022B  50                push ax
0000022C  9A60033505        call 0x535:0x360
00000231  59                pop cx
00000232  59                pop cx
00000233  A0AA37            mov al,[0x37aa]
00000236  98                cbw
00000237  8BD8              mov bx,ax
00000239  80BF944800        cmp byte [bx+0x4894],0x0
0000023E  7405              jz 0x245
00000240  9A1F043505        call 0x535:0x41f
00000245  EB00              jmp short 0x247
00000247  FE06AA37          inc byte [0x37aa]
0000024B  803EAA3704        cmp byte [0x37aa],0x4
00000250  7D03              jnl 0x255
00000252  E90CFF            jmp 0x161
00000255  9A040D3505        call 0x535:0xd04
0000025A  9AA806960A        call 0xa96:0x6a8
0000025F  FF362E08          push word [0x82e]
00000263  9AE906CD23        call 0x23cd:0x6e9
00000268  59                pop cx
00000269  803EE63C00        cmp byte [0x3ce6],0x0
0000026E  7403              jz 0x273
00000270  E9BEFE            jmp 0x131
00000273  9A6506FD21        call 0x21fd:0x665
00000278  5D                pop bp
00000279  CB                retf

; ---- 0535:027A ----
0000027A  55                push bp
0000027B  8BEC              mov bp,sp
0000027D  4C                dec sp
0000027E  4C                dec sp
0000027F  A0AA37            mov al,[0x37aa]
00000282  98                cbw
00000283  8BD8              mov bx,ax
00000285  8A87902E          mov al,[bx+0x2e90]
00000289  024606            add al,[bp+0x6]
0000028C  8846FF            mov [bp-0x1],al
0000028F  807EFF00          cmp byte [bp-0x1],0x0
00000293  7C06              jl 0x29b
00000295  807EFF03          cmp byte [bp-0x1],0x3
00000299  7E02              jng 0x29d
0000029B  EB20              jmp short 0x2bd
0000029D  8A46FF            mov al,[bp-0x1]
000002A0  50                push ax
000002A1  9A1E033505        call 0x535:0x31e
000002A6  59                pop cx
000002A7  A0AA37            mov al,[0x37aa]
000002AA  98                cbw
000002AB  D1E0              shl ax,1
000002AD  D1E0              shl ax,1
000002AF  8BD8              mov bx,ax
000002B1  C78786370F00      mov word [bx+0x3786],0xf
000002B7  C78788370000      mov word [bx+0x3788],0x0
000002BD  8BE5              mov sp,bp
000002BF  5D                pop bp
000002C0  CB                retf

; ---- 0535:02C1 ----
000002C1  55                push bp
000002C2  8BEC              mov bp,sp
000002C4  A0AA37            mov al,[0x37aa]
000002C7  98                cbw
000002C8  8BD8              mov bx,ax
000002CA  80BF902E03        cmp byte [bx+0x2e90],0x3
000002CF  7521              jnz 0x2f2
000002D1  A0AA37            mov al,[0x37aa]
000002D4  98                cbw
000002D5  50                push ax
000002D6  9A5E04AF21        call 0x21af:0x45e
000002DB  59                pop cx
000002DC  9A5C093505        call 0x535:0x95c
000002E1  FE0EE63C          dec byte [0x3ce6]
000002E5  A0AA37            mov al,[0x37aa]
000002E8  98                cbw
000002E9  8BD8              mov bx,ax
000002EB  C687B24306        mov byte [bx+0x43b2],0x6
000002F0  EB0B              jmp short 0x2fd
000002F2  A0AA37            mov al,[0x37aa]
000002F5  98                cbw
000002F6  8BD8              mov bx,ax
000002F8  C687B2430E        mov byte [bx+0x43b2],0xe
000002FD  A0AA37            mov al,[0x37aa]
00000300  98                cbw
00000301  D1E0              shl ax,1
00000303  D1E0              shl ax,1
00000305  8BD8              mov bx,ax
00000307  C7872E301027      mov word [bx+0x302e],0x2710
0000030D  C78730300000      mov word [bx+0x3030],0x0
00000313  B000              mov al,0x0
00000315  50                push ax
00000316  9A3F0B3505        call 0x535:0xb3f
0000031B  59                pop cx
0000031C  5D                pop bp
0000031D  CB                retf

; ---- 0535:031E ----
0000031E  55                push bp
0000031F  8BEC              mov bp,sp
00000321  9A5C093505        call 0x535:0x95c
00000326  8A4606            mov al,[bp+0x6]
00000329  B400              mov ah,0x0
0000032B  D1E0              shl ax,1
0000032D  D1E0              shl ax,1
0000032F  8BD8              mov bx,ax
00000331  FFB7D237          push word [bx+0x37d2]
00000335  FFB7D037          push word [bx+0x37d0]
00000339  8A4606            mov al,[bp+0x6]
0000033C  B400              mov ah,0x0
0000033E  50                push ax
0000033F  9A95093505        call 0x535:0x995
00000344  83C406            add sp,byte +0x6
00000347  8A4606            mov al,[bp+0x6]
0000034A  50                push ax
0000034B  9A070A3505        call 0x535:0xa07
00000350  59                pop cx
00000351  A0AA37            mov al,[0x37aa]
00000354  98                cbw
00000355  8A5606            mov dl,[bp+0x6]
00000358  8BD8              mov bx,ax
0000035A  8897902E          mov [bx+0x2e90],dl
0000035E  5D                pop bp
0000035F  CB                retf

; ---- 0535:0360 ----
00000360  55                push bp
00000361  8BEC              mov bp,sp
00000363  56                push si
00000364  A0AA37            mov al,[0x37aa]
00000367  98                cbw
00000368  D1E0              shl ax,1
0000036A  8BD8              mov bx,ax
0000036C  8B875230          mov ax,[bx+0x3052]
00000370  034606            add ax,[bp+0x6]
00000373  8BF0              mov si,ax
00000375  A0AA37            mov al,[0x37aa]
00000378  98                cbw
00000379  8BD8              mov bx,ax
0000037B  8A875408          mov al,[bx+0x854]
0000037F  B400              mov ah,0x0
00000381  3BC6              cmp ax,si
00000383  7F1C              jg 0x3a1
00000385  A0AA37            mov al,[0x37aa]
00000388  98                cbw
00000389  8BD8              mov bx,ax
0000038B  8A875808          mov al,[bx+0x858]
0000038F  B400              mov ah,0x0
00000391  3BC6              cmp ax,si
00000393  7C0C              jl 0x3a1
00000395  A0AA37            mov al,[0x37aa]
00000398  98                cbw
00000399  D1E0              shl ax,1
0000039B  8BD8              mov bx,ax
0000039D  89B75230          mov [bx+0x3052],si
000003A1  A0AA37            mov al,[0x37aa]
000003A4  98                cbw
000003A5  D1E0              shl ax,1
000003A7  8BD8              mov bx,ax
000003A9  8B875E30          mov ax,[bx+0x305e]
000003AD  034608            add ax,[bp+0x8]
000003B0  8BF0              mov si,ax
000003B2  A0AA37            mov al,[0x37aa]
000003B5  98                cbw
000003B6  8BD8              mov bx,ax
000003B8  8A875C08          mov al,[bx+0x85c]
000003BC  B400              mov ah,0x0
000003BE  3BC6              cmp ax,si
000003C0  7F1C              jg 0x3de
000003C2  A0AA37            mov al,[0x37aa]
000003C5  98                cbw
000003C6  8BD8              mov bx,ax
000003C8  8A876008          mov al,[bx+0x860]
000003CC  B400              mov ah,0x0
000003CE  3BC6              cmp ax,si
000003D0  7C0C              jl 0x3de
000003D2  A0AA37            mov al,[0x37aa]
000003D5  98                cbw
000003D6  D1E0              shl ax,1
000003D8  8BD8              mov bx,ax
000003DA  89B75E30          mov [bx+0x305e],si
000003DE  9AB30C3505        call 0x535:0xcb3
000003E3  A0AA37            mov al,[0x37aa]
000003E6  98                cbw
000003E7  8BD8              mov bx,ax
000003E9  80BF400800        cmp byte [bx+0x840],0x0
000003EE  752C              jnz 0x41c
000003F0  A0AA37            mov al,[0x37aa]
000003F3  98                cbw
000003F4  D1E0              shl ax,1
000003F6  D1E0              shl ax,1
000003F8  8BD8              mov bx,ax
000003FA  C78786370600      mov word [bx+0x3786],0x6
00000400  C78788370000      mov word [bx+0x3788],0x0
00000406  A0AA37            mov al,[0x37aa]
00000409  98                cbw
0000040A  D1E0              shl ax,1
0000040C  D1E0              shl ax,1
0000040E  8BD8              mov bx,ax
00000410  C7872E300000      mov word [bx+0x302e],0x0
00000416  C78730300000      mov word [bx+0x3030],0x0
0000041C  5E                pop si
0000041D  5D                pop bp
0000041E  CB                retf

; ---- 0535:041F ----
0000041F  55                push bp
00000420  8BEC              mov bp,sp
00000422  83EC04            sub sp,byte +0x4
00000425  A0AA37            mov al,[0x37aa]
00000428  98                cbw
00000429  D1E0              shl ax,1
0000042B  8BD8              mov bx,ax
0000042D  8A875230          mov al,[bx+0x3052]
00000431  8846FF            mov [bp-0x1],al
00000434  A0AA37            mov al,[0x37aa]
00000437  98                cbw
00000438  D1E0              shl ax,1
0000043A  8BD8              mov bx,ax
0000043C  8A875E30          mov al,[bx+0x305e]
00000440  8846FE            mov [bp-0x2],al
00000443  8A46FF            mov al,[bp-0x1]
00000446  B400              mov ah,0x0
00000448  BA1A00            mov dx,0x1a
0000044B  F7EA              imul dx
0000044D  8A56FE            mov dl,[bp-0x2]
00000450  B600              mov dh,0x0
00000452  03C2              add ax,dx
00000454  8BD8              mov bx,ax
00000456  8A87F448          mov al,[bx+0x48f4]
0000045A  B400              mov ah,0x0
0000045C  BB0800            mov bx,0x8
0000045F  99                cwd
00000460  F7FB              idiv bx
00000462  8846FC            mov [bp-0x4],al
00000465  807EFC01          cmp byte [bp-0x4],0x1
00000469  7524              jnz 0x48f
0000046B  A0AA37            mov al,[0x37aa]
0000046E  98                cbw
0000046F  D1E0              shl ax,1
00000471  D1E0              shl ax,1
00000473  8BD8              mov bx,ax
00000475  C7872E301027      mov word [bx+0x302e],0x2710
0000047B  C78730300000      mov word [bx+0x3030],0x0
00000481  A0AA37            mov al,[0x37aa]
00000484  98                cbw
00000485  8BD8              mov bx,ax
00000487  C687B2430D        mov byte [bx+0x43b2],0xd
0000048C  E95501            jmp 0x5e4
0000048F  A0AA37            mov al,[0x37aa]
00000492  98                cbw
00000493  D1E0              shl ax,1
00000495  D1E0              shl ax,1
00000497  8BD8              mov bx,ax
00000499  C7872E300700      mov word [bx+0x302e],0x7
0000049F  C78730300000      mov word [bx+0x3030],0x0
000004A5  807EFC05          cmp byte [bp-0x4],0x5
000004A9  740C              jz 0x4b7
000004AB  807EFC06          cmp byte [bp-0x4],0x6
000004AF  7406              jz 0x4b7
000004B1  807EFC0D          cmp byte [bp-0x4],0xd
000004B5  7206              jc 0x4bd
000004B7  E92A01            jmp 0x5e4
000004BA  E92701            jmp 0x5e4
000004BD  A0AA37            mov al,[0x37aa]
000004C0  98                cbw
000004C1  8BD8              mov bx,ax
000004C3  8A87902E          mov al,[bx+0x2e90]
000004C7  B400              mov ah,0x0
000004C9  0BC0              or ax,ax
000004CB  745D              jz 0x52a
000004CD  3D0100            cmp ax,0x1
000004D0  7503              jnz 0x4d5
000004D2  E9A100            jmp 0x576
000004D5  3D0200            cmp ax,0x2
000004D8  7403              jz 0x4dd
000004DA  E90701            jmp 0x5e4
000004DD  A0AA37            mov al,[0x37aa]
000004E0  98                cbw
000004E1  D1E0              shl ax,1
000004E3  8BD8              mov bx,ax
000004E5  83BFDC3300        cmp word [bx+0x33dc],byte +0x0
000004EA  7406              jz 0x4f2
000004EC  807EFC03          cmp byte [bp-0x4],0x3
000004F0  7503              jnz 0x4f5
000004F2  E9EF00            jmp 0x5e4
000004F5  8A46FC            mov al,[bp-0x4]
000004F8  50                push ax
000004F9  9AE8053505        call 0x535:0x5e8
000004FE  59                pop cx
000004FF  8A46FE            mov al,[bp-0x2]
00000502  B400              mov ah,0x0
00000504  50                push ax
00000505  8A46FF            mov al,[bp-0x1]
00000508  B400              mov ah,0x0
0000050A  50                push ax
0000050B  9A70087E1E        call 0x1e7e:0x870
00000510  59                pop cx
00000511  59                pop cx
00000512  A0AA37            mov al,[0x37aa]
00000515  98                cbw
00000516  D1E0              shl ax,1
00000518  8BD8              mov bx,ax
0000051A  FF8FDC33          dec word [bx+0x33dc]
0000051E  B002              mov al,0x2
00000520  50                push ax
00000521  9A070A3505        call 0x535:0xa07
00000526  59                pop cx
00000527  E9BA00            jmp 0x5e4
0000052A  A0AA37            mov al,[0x37aa]
0000052D  98                cbw
0000052E  D1E0              shl ax,1
00000530  8BD8              mov bx,ax
00000532  83BF163000        cmp word [bx+0x3016],byte +0x0
00000537  7406              jz 0x53f
00000539  807EFC04          cmp byte [bp-0x4],0x4
0000053D  7503              jnz 0x542
0000053F  E9A200            jmp 0x5e4
00000542  8A46FC            mov al,[bp-0x4]
00000545  50                push ax
00000546  9AE8053505        call 0x535:0x5e8
0000054B  59                pop cx
0000054C  8A46FE            mov al,[bp-0x2]
0000054F  B400              mov ah,0x0
00000551  50                push ax
00000552  8A46FF            mov al,[bp-0x1]
00000555  B400              mov ah,0x0
00000557  50                push ax
00000558  9A8D087E1E        call 0x1e7e:0x88d
0000055D  59                pop cx
0000055E  59                pop cx
0000055F  A0AA37            mov al,[0x37aa]
00000562  98                cbw
00000563  D1E0              shl ax,1
00000565  8BD8              mov bx,ax
00000567  FF8F1630          dec word [bx+0x3016]
0000056B  B000              mov al,0x0
0000056D  50                push ax
0000056E  9A070A3505        call 0x535:0xa07
00000573  59                pop cx
00000574  EB6E              jmp short 0x5e4
00000576  A0AA37            mov al,[0x37aa]
00000579  98                cbw
0000057A  D1E0              shl ax,1
0000057C  8BD8              mov bx,ax
0000057E  83BFEC4000        cmp word [bx+0x40ec],byte +0x0
00000583  7502              jnz 0x587
00000585  EB5D              jmp short 0x5e4
00000587  807EFC07          cmp byte [bp-0x4],0x7
0000058B  7218              jc 0x5a5
0000058D  8A46FC            mov al,[bp-0x4]
00000590  04F9              add al,0xf9
00000592  8846FD            mov [bp-0x3],al
00000595  8A46FD            mov al,[bp-0x3]
00000598  FEC0              inc al
0000059A  8846FD            mov [bp-0x3],al
0000059D  3C05              cmp al,0x5
0000059F  7602              jna 0x5a3
000005A1  EB41              jmp short 0x5e4
000005A3  EB0E              jmp short 0x5b3
000005A5  8A46FC            mov al,[bp-0x4]
000005A8  50                push ax
000005A9  9AE8053505        call 0x535:0x5e8
000005AE  59                pop cx
000005AF  C646FD00          mov byte [bp-0x3],0x0
000005B3  8A46FD            mov al,[bp-0x3]
000005B6  B400              mov ah,0x0
000005B8  50                push ax
000005B9  8A46FE            mov al,[bp-0x2]
000005BC  B400              mov ah,0x0
000005BE  50                push ax
000005BF  8A46FF            mov al,[bp-0x1]
000005C2  B400              mov ah,0x0
000005C4  50                push ax
000005C5  9A3F087E1E        call 0x1e7e:0x83f
000005CA  83C406            add sp,byte +0x6
000005CD  A0AA37            mov al,[0x37aa]
000005D0  98                cbw
000005D1  D1E0              shl ax,1
000005D3  8BD8              mov bx,ax
000005D5  FF8FEC40          dec word [bx+0x40ec]
000005D9  B001              mov al,0x1
000005DB  50                push ax
000005DC  9A070A3505        call 0x535:0xa07
000005E1  59                pop cx
000005E2  EB00              jmp short 0x5e4
000005E4  8BE5              mov sp,bp
000005E6  5D                pop bp
000005E7  CB                retf

; ---- 0535:05E8 ----
000005E8  55                push bp
000005E9  8BEC              mov bp,sp
000005EB  8A4E06            mov cl,[bp+0x6]
000005EE  80F904            cmp cl,0x4
000005F1  750E              jnz 0x601
000005F3  A0AA37            mov al,[0x37aa]
000005F6  98                cbw
000005F7  D1E0              shl ax,1
000005F9  8BD8              mov bx,ax
000005FB  FF871630          inc word [bx+0x3016]
000005FF  EB37              jmp short 0x638
00000601  80F903            cmp cl,0x3
00000604  750E              jnz 0x614
00000606  A0AA37            mov al,[0x37aa]
00000609  98                cbw
0000060A  D1E0              shl ax,1
0000060C  8BD8              mov bx,ax
0000060E  FF87DC33          inc word [bx+0x33dc]
00000612  EB24              jmp short 0x638
00000614  80F907            cmp cl,0x7
00000617  721F              jc 0x638
00000619  A0AA37            mov al,[0x37aa]
0000061C  98                cbw
0000061D  D1E0              shl ax,1
0000061F  8AD1              mov dl,cl
00000621  B600              mov dh,0x0
00000623  8BD8              mov bx,ax
00000625  0397EC40          add dx,[bx+0x40ec]
00000629  83C2FA            add dx,byte -0x6
0000062C  A0AA37            mov al,[0x37aa]
0000062F  98                cbw
00000630  D1E0              shl ax,1
00000632  8BD8              mov bx,ax
00000634  8997EC40          mov [bx+0x40ec],dx
00000638  5D                pop bp
00000639  CB                retf

; ---- 0535:063A ----
0000063A  55                push bp
0000063B  8BEC              mov bp,sp
0000063D  4C                dec sp
0000063E  4C                dec sp
0000063F  56                push si
00000640  33F6              xor si,si
00000642  A0AA37            mov al,[0x37aa]
00000645  98                cbw
00000646  8BD8              mov bx,ax
00000648  8A875C08          mov al,[bx+0x85c]
0000064C  B400              mov ah,0x0
0000064E  8BC8              mov cx,ax
00000650  EB67              jmp short 0x6b9
00000652  A0AA37            mov al,[0x37aa]
00000655  98                cbw
00000656  8BD8              mov bx,ax
00000658  8A875408          mov al,[bx+0x854]
0000065C  B400              mov ah,0x0
0000065E  8946FE            mov [bp-0x2],ax
00000661  EB44              jmp short 0x6a7
00000663  8B46FE            mov ax,[bp-0x2]
00000666  BA1A00            mov dx,0x1a
00000669  F7EA              imul dx
0000066B  03C1              add ax,cx
0000066D  8BD8              mov bx,ax
0000066F  8A87F448          mov al,[bx+0x48f4]
00000673  B400              mov ah,0x0
00000675  BB0800            mov bx,0x8
00000678  99                cwd
00000679  F7FB              idiv bx
0000067B  3D0400            cmp ax,0x4
0000067E  7524              jnz 0x6a4
00000680  A0AA37            mov al,[0x37aa]
00000683  98                cbw
00000684  BA0700            mov dx,0x7
00000687  F7EA              imul dx
00000689  8A56FE            mov dl,[bp-0x2]
0000068C  8BD8              mov bx,ax
0000068E  8890FE43          mov [bx+si+0x43fe],dl
00000692  A0AA37            mov al,[0x37aa]
00000695  98                cbw
00000696  BA0700            mov dx,0x7
00000699  F7EA              imul dx
0000069B  8AD1              mov dl,cl
0000069D  8BD8              mov bx,ax
0000069F  88901A44          mov [bx+si+0x441a],dl
000006A3  46                inc si
000006A4  FF46FE            inc word [bp-0x2]
000006A7  A0AA37            mov al,[0x37aa]
000006AA  98                cbw
000006AB  8BD8              mov bx,ax
000006AD  8A875808          mov al,[bx+0x858]
000006B1  B400              mov ah,0x0
000006B3  3B46FE            cmp ax,[bp-0x2]
000006B6  7DAB              jnl 0x663
000006B8  41                inc cx
000006B9  A0AA37            mov al,[0x37aa]
000006BC  98                cbw
000006BD  8BD8              mov bx,ax
000006BF  8A876008          mov al,[bx+0x860]
000006C3  B400              mov ah,0x0
000006C5  3BC1              cmp ax,cx
000006C7  7D89              jnl 0x652
000006C9  5E                pop si
000006CA  8BE5              mov sp,bp
000006CC  5D                pop bp
000006CD  CB                retf

; ---- 0535:06CE ----
000006CE  55                push bp
000006CF  8BEC              mov bp,sp
000006D1  83EC04            sub sp,byte +0x4
000006D4  C646FE00          mov byte [bp-0x2],0x0
000006D8  E9A200            jmp 0x77d
000006DB  C646FF00          mov byte [bp-0x1],0x0
000006DF  E98F00            jmp 0x771
000006E2  8A46FF            mov al,[bp-0x1]
000006E5  B400              mov ah,0x0
000006E7  BA1A00            mov dx,0x1a
000006EA  F7EA              imul dx
000006EC  8A56FE            mov dl,[bp-0x2]
000006EF  B600              mov dh,0x0
000006F1  03C2              add ax,dx
000006F3  8BD8              mov bx,ax
000006F5  8A87F448          mov al,[bx+0x48f4]
000006F9  B400              mov ah,0x0
000006FB  BB0800            mov bx,0x8
000006FE  99                cwd
000006FF  F7FB              idiv bx
00000701  8846FD            mov [bp-0x3],al
00000704  8A46FE            mov al,[bp-0x2]
00000707  B400              mov ah,0x0
00000709  50                push ax
0000070A  8A46FF            mov al,[bp-0x1]
0000070D  B400              mov ah,0x0
0000070F  50                push ax
00000710  9AC309DE0E        call 0xede:0x9c3
00000715  59                pop cx
00000716  59                pop cx
00000717  A2AA37            mov [0x37aa],al
0000071A  807EFD04          cmp byte [bp-0x3],0x4
0000071E  750E              jnz 0x72e
00000720  A0AA37            mov al,[0x37aa]
00000723  98                cbw
00000724  D1E0              shl ax,1
00000726  8BD8              mov bx,ax
00000728  FF871630          inc word [bx+0x3016]
0000072C  EB40              jmp short 0x76e
0000072E  807EFD03          cmp byte [bp-0x3],0x3
00000732  750E              jnz 0x742
00000734  A0AA37            mov al,[0x37aa]
00000737  98                cbw
00000738  D1E0              shl ax,1
0000073A  8BD8              mov bx,ax
0000073C  FF87DC33          inc word [bx+0x33dc]
00000740  EB2C              jmp short 0x76e
00000742  807EFD07          cmp byte [bp-0x3],0x7
00000746  7226              jc 0x76e
00000748  807EFD0C          cmp byte [bp-0x3],0xc
0000074C  7720              ja 0x76e
0000074E  A0AA37            mov al,[0x37aa]
00000751  98                cbw
00000752  D1E0              shl ax,1
00000754  8A56FD            mov dl,[bp-0x3]
00000757  B600              mov dh,0x0
00000759  8BD8              mov bx,ax
0000075B  0397EC40          add dx,[bx+0x40ec]
0000075F  83C2FA            add dx,byte -0x6
00000762  A0AA37            mov al,[0x37aa]
00000765  98                cbw
00000766  D1E0              shl ax,1
00000768  8BD8              mov bx,ax
0000076A  8997EC40          mov [bx+0x40ec],dx
0000076E  FE46FF            inc byte [bp-0x1]
00000771  807EFF19          cmp byte [bp-0x1],0x19
00000775  7703              ja 0x77a
00000777  E968FF            jmp 0x6e2
0000077A  FE46FE            inc byte [bp-0x2]
0000077D  807EFE19          cmp byte [bp-0x2],0x19
00000781  7703              ja 0x786
00000783  E955FF            jmp 0x6db
00000786  8BE5              mov sp,bp
00000788  5D                pop bp
00000789  CB                retf

; ---- 0535:078A ----
0000078A  55                push bp
0000078B  8BEC              mov bp,sp
0000078D  9A1B00960A        call 0xa96:0x1b
00000792  C606AA3700        mov byte [0x37aa],0x0
00000797  EB23              jmp short 0x7bc
00000799  A0AA37            mov al,[0x37aa]
0000079C  98                cbw
0000079D  D1E0              shl ax,1
0000079F  8BD8              mov bx,ax
000007A1  FFB77008          push word [bx+0x870]
000007A5  A0AA37            mov al,[0x37aa]
000007A8  98                cbw
000007A9  D1E0              shl ax,1
000007AB  8BD8              mov bx,ax
000007AD  FFB76808          push word [bx+0x868]
000007B1  9ACA073505        call 0x535:0x7ca
000007B6  59                pop cx
000007B7  59                pop cx
000007B8  FE06AA37          inc byte [0x37aa]
000007BC  803EAA3704        cmp byte [0x37aa],0x4
000007C1  7CD6              jl 0x799
000007C3  9A0D06FD21        call 0x21fd:0x60d
000007C8  5D                pop bp
000007C9  CB                retf

; ---- 0535:07CA ----
000007CA  55                push bp
000007CB  8BEC              mov bp,sp
000007CD  56                push si
000007CE  57                push di
000007CF  8B7E06            mov di,[bp+0x6]
000007D2  8B7608            mov si,[bp+0x8]
000007D5  B80B00            mov ax,0xb
000007D8  50                push ax
000007D9  B84C00            mov ax,0x4c
000007DC  50                push ax
000007DD  B83400            mov ax,0x34
000007E0  50                push ax
000007E1  56                push si
000007E2  8BC7              mov ax,di
000007E4  050400            add ax,0x4
000007E7  50                push ax
000007E8  9AD704960A        call 0xa96:0x4d7
000007ED  83C40A            add sp,byte +0xa
000007F0  A0AA37            mov al,[0x37aa]
000007F3  98                cbw
000007F4  8BD8              mov bx,ax
000007F6  8A876408          mov al,[bx+0x864]
000007FA  B400              mov ah,0x0
000007FC  50                push ax
000007FD  B85B00            mov ax,0x5b
00000800  50                push ax
00000801  B83200            mov ax,0x32
00000804  50                push ax
00000805  8BC6              mov ax,si
00000807  40                inc ax
00000808  50                push ax
00000809  8BC7              mov ax,di
0000080B  050500            add ax,0x5
0000080E  50                push ax
0000080F  9AD704960A        call 0xa96:0x4d7
00000814  83C40A            add sp,byte +0xa
00000817  A0AA37            mov al,[0x37aa]
0000081A  98                cbw
0000081B  8BD8              mov bx,ax
0000081D  8A876408          mov al,[bx+0x864]
00000821  B400              mov ah,0x0
00000823  50                push ax
00000824  B80300            mov ax,0x3
00000827  50                push ax
00000828  B82E00            mov ax,0x2e
0000082B  50                push ax
0000082C  8BC6              mov ax,si
0000082E  055C00            add ax,0x5c
00000831  50                push ax
00000832  8BC7              mov ax,di
00000834  050700            add ax,0x7
00000837  50                push ax
00000838  9AD704960A        call 0xa96:0x4d7
0000083D  83C40A            add sp,byte +0xa
00000840  B80B00            mov ax,0xb
00000843  50                push ax
00000844  B84600            mov ax,0x46
00000847  50                push ax
00000848  B82E00            mov ax,0x2e
0000084B  50                push ax
0000084C  8BC6              mov ax,si
0000084E  050400            add ax,0x4
00000851  50                push ax
00000852  8BC7              mov ax,di
00000854  050600            add ax,0x6
00000857  50                push ax
00000858  9AD704960A        call 0xa96:0x4d7
0000085D  83C40A            add sp,byte +0xa
00000860  B80100            mov ax,0x1
00000863  50                push ax
00000864  B84600            mov ax,0x46
00000867  50                push ax
00000868  B82E00            mov ax,0x2e
0000086B  50                push ax
0000086C  8BC6              mov ax,si
0000086E  050300            add ax,0x3
00000871  50                push ax
00000872  8BC7              mov ax,di
00000874  050700            add ax,0x7
00000877  50                push ax
00000878  9A8104960A        call 0xa96:0x481
0000087D  83C40A            add sp,byte +0xa
00000880  B80C00            mov ax,0xc
00000883  50                push ax
00000884  B84800            mov ax,0x48
00000887  50                push ax
00000888  B80300            mov ax,0x3
0000088B  50                push ax
0000088C  8BC6              mov ax,si
0000088E  050400            add ax,0x4
00000891  50                push ax
00000892  8BC7              mov ax,di
00000894  40                inc ax
00000895  50                push ax
00000896  9AD704960A        call 0xa96:0x4d7
0000089B  83C40A            add sp,byte +0xa
0000089E  8BC6              mov ax,si
000008A0  054C00            add ax,0x4c
000008A3  50                push ax
000008A4  57                push di
000008A5  B81800            mov ax,0x18
000008A8  50                push ax
000008A9  B83800            mov ax,0x38
000008AC  50                push ax
000008AD  B82000            mov ax,0x20
000008B0  50                push ax
000008B1  B84800            mov ax,0x48
000008B4  50                push ax
000008B5  9A3000960A        call 0xa96:0x30
000008BA  83C40C            add sp,byte +0xc
000008BD  A0AA37            mov al,[0x37aa]
000008C0  98                cbw
000008C1  8BD8              mov bx,ax
000008C3  8A876408          mov al,[bx+0x864]
000008C7  B400              mov ah,0x0
000008C9  50                push ax
000008CA  8BC6              mov ax,si
000008CC  054C00            add ax,0x4c
000008CF  50                push ax
000008D0  57                push di
000008D1  B81300            mov ax,0x13
000008D4  50                push ax
000008D5  B83800            mov ax,0x38
000008D8  50                push ax
000008D9  B84B00            mov ax,0x4b
000008DC  50                push ax
000008DD  B8F000            mov ax,0xf0
000008E0  50                push ax
000008E1  9AAA01960A        call 0xa96:0x1aa
000008E6  83C40E            add sp,byte +0xe
000008E9  5F                pop di
000008EA  5E                pop si
000008EB  5D                pop bp
000008EC  CB                retf

; ---- 0535:08ED ----
000008ED  55                push bp
000008EE  8BEC              mov bp,sp
000008F0  56                push si
000008F1  9A1B00960A        call 0xa96:0x1b
000008F6  C606AA3700        mov byte [0x37aa],0x0
000008FB  EB50              jmp short 0x94d
000008FD  A0AA37            mov al,[0x37aa]
00000900  98                cbw
00000901  D1E0              shl ax,1
00000903  8BD8              mov bx,ax
00000905  C7877A150100      mov word [bx+0x157a],0x1
0000090B  33F6              xor si,si
0000090D  EB11              jmp short 0x920
0000090F  A0AA37            mov al,[0x37aa]
00000912  98                cbw
00000913  BA0700            mov dx,0x7
00000916  F7EA              imul dx
00000918  8BD8              mov bx,ax
0000091A  C680FE43FF        mov byte [bx+si+0x43fe],0xff
0000091F  46                inc si
00000920  83FE07            cmp si,byte +0x7
00000923  7CEA              jl 0x90f
00000925  A0AA37            mov al,[0x37aa]
00000928  98                cbw
00000929  8BD8              mov bx,ax
0000092B  80BF7C3700        cmp byte [bx+0x377c],0x0
00000930  7505              jnz 0x937
00000932  9A3F1FDE0E        call 0xede:0x1f3f
00000937  B000              mov al,0x0
00000939  50                push ax
0000093A  9AA81EDE0E        call 0xede:0x1ea8
0000093F  59                pop cx
00000940  B001              mov al,0x1
00000942  50                push ax
00000943  9AA81EDE0E        call 0xede:0x1ea8
00000948  59                pop cx
00000949  FE06AA37          inc byte [0x37aa]
0000094D  803EAA3704        cmp byte [0x37aa],0x4
00000952  7CA9              jl 0x8fd
00000954  9A0D06FD21        call 0x21fd:0x60d
00000959  5E                pop si
0000095A  5D                pop bp
0000095B  CB                retf

; ---- 0535:095C ----
0000095C  55                push bp
0000095D  8BEC              mov bp,sp
0000095F  B80B00            mov ax,0xb
00000962  50                push ax
00000963  B84400            mov ax,0x44
00000966  50                push ax
00000967  B82C00            mov ax,0x2c
0000096A  50                push ax
0000096B  A0AA37            mov al,[0x37aa]
0000096E  98                cbw
0000096F  D1E0              shl ax,1
00000971  8BD8              mov bx,ax
00000973  8B877008          mov ax,[bx+0x870]
00000977  050400            add ax,0x4
0000097A  50                push ax
0000097B  A0AA37            mov al,[0x37aa]
0000097E  98                cbw
0000097F  D1E0              shl ax,1
00000981  8BD8              mov bx,ax
00000983  8B876808          mov ax,[bx+0x868]
00000987  050800            add ax,0x8
0000098A  50                push ax
0000098B  9AD704960A        call 0xa96:0x4d7
00000990  83C40A            add sp,byte +0xa
00000993  5D                pop bp
00000994  CB                retf

; ---- 0535:0995 ----
00000995  55                push bp
00000996  8BEC              mov bp,sp
00000998  56                push si
00000999  57                push di
0000099A  A0AA37            mov al,[0x37aa]
0000099D  98                cbw
0000099E  D1E0              shl ax,1
000009A0  8BD8              mov bx,ax
000009A2  8BBF6808          mov di,[bx+0x868]
000009A6  A0AA37            mov al,[0x37aa]
000009A9  98                cbw
000009AA  D1E0              shl ax,1
000009AC  8BD8              mov bx,ax
000009AE  8BB77008          mov si,[bx+0x870]
000009B2  837E0602          cmp word [bp+0x6],byte +0x2
000009B6  7F26              jg 0x9de
000009B8  8BC6              mov ax,si
000009BA  050A00            add ax,0xa
000009BD  50                push ax
000009BE  8BC7              mov ax,di
000009C0  050800            add ax,0x8
000009C3  50                push ax
000009C4  B81800            mov ax,0x18
000009C7  50                push ax
000009C8  B8A000            mov ax,0xa0
000009CB  50                push ax
000009CC  8B5E06            mov bx,[bp+0x6]
000009CF  8A879000          mov al,[bx+0x90]
000009D3  B400              mov ah,0x0
000009D5  50                push ax
000009D6  9AD602960A        call 0xa96:0x2d6
000009DB  83C40A            add sp,byte +0xa
000009DE  B80B00            mov ax,0xb
000009E1  50                push ax
000009E2  33C0              xor ax,ax
000009E4  50                push ax
000009E5  FF760A            push word [bp+0xa]
000009E8  FF7608            push word [bp+0x8]
000009EB  B82C00            mov ax,0x2c
000009EE  50                push ax
000009EF  8BC6              mov ax,si
000009F1  052D00            add ax,0x2d
000009F4  50                push ax
000009F5  8BC7              mov ax,di
000009F7  050800            add ax,0x8
000009FA  50                push ax
000009FB  9A9000690A        call 0xa69:0x90
00000A00  83C40E            add sp,byte +0xe
00000A03  5F                pop di
00000A04  5E                pop si
00000A05  5D                pop bp
00000A06  CB                retf

; ---- 0535:0A07 ----
00000A07  55                push bp
00000A08  8BEC              mov bp,sp
00000A0A  83EC04            sub sp,byte +0x4
00000A0D  56                push si
00000A0E  57                push di
00000A0F  A0AA37            mov al,[0x37aa]
00000A12  98                cbw
00000A13  D1E0              shl ax,1
00000A15  8BD8              mov bx,ax
00000A17  8BBF6808          mov di,[bx+0x868]
00000A1B  A0AA37            mov al,[0x37aa]
00000A1E  98                cbw
00000A1F  D1E0              shl ax,1
00000A21  8BD8              mov bx,ax
00000A23  8BB77008          mov si,[bx+0x870]
00000A27  B005              mov al,0x5
00000A29  8846FE            mov [bp-0x2],al
00000A2C  8846FD            mov [bp-0x3],al
00000A2F  8846FC            mov [bp-0x4],al
00000A32  807E0602          cmp byte [bp+0x6],0x2
00000A36  7710              ja 0xa48
00000A38  8A4606            mov al,[bp+0x6]
00000A3B  B400              mov ah,0x0
00000A3D  8D56FC            lea dx,[bp-0x4]
00000A40  03C2              add ax,dx
00000A42  8BD8              mov bx,ax
00000A44  36C60707          mov byte [ss:bx],0x7
00000A48  B80B00            mov ax,0xb
00000A4B  50                push ax
00000A4C  8A46FC            mov al,[bp-0x4]
00000A4F  B400              mov ah,0x0
00000A51  50                push ax
00000A52  B80300            mov ax,0x3
00000A55  50                push ax
00000A56  A0AA37            mov al,[0x37aa]
00000A59  98                cbw
00000A5A  D1E0              shl ax,1
00000A5C  33D2              xor dx,dx
00000A5E  52                push dx
00000A5F  8BD8              mov bx,ax
00000A61  FFB71630          push word [bx+0x3016]
00000A65  8BC6              mov ax,si
00000A67  053700            add ax,0x37
00000A6A  50                push ax
00000A6B  8BC7              mov ax,di
00000A6D  051500            add ax,0x15
00000A70  50                push ax
00000A71  9A7A05960A        call 0xa96:0x57a
00000A76  83C40E            add sp,byte +0xe
00000A79  B80B00            mov ax,0xb
00000A7C  50                push ax
00000A7D  8A46FD            mov al,[bp-0x3]
00000A80  B400              mov ah,0x0
00000A82  50                push ax
00000A83  B80300            mov ax,0x3
00000A86  50                push ax
00000A87  A0AA37            mov al,[0x37aa]
00000A8A  98                cbw
00000A8B  D1E0              shl ax,1
00000A8D  33D2              xor dx,dx
00000A8F  52                push dx
00000A90  8BD8              mov bx,ax
00000A92  FFB7EC40          push word [bx+0x40ec]
00000A96  8BC6              mov ax,si
00000A98  053D00            add ax,0x3d
00000A9B  50                push ax
00000A9C  8BC7              mov ax,di
00000A9E  051500            add ax,0x15
00000AA1  50                push ax
00000AA2  9A7A05960A        call 0xa96:0x57a
00000AA7  83C40E            add sp,byte +0xe
00000AAA  B80B00            mov ax,0xb
00000AAD  50                push ax
00000AAE  8A46FE            mov al,[bp-0x2]
00000AB1  B400              mov ah,0x0
00000AB3  50                push ax
00000AB4  B80300            mov ax,0x3
00000AB7  50                push ax
00000AB8  A0AA37            mov al,[0x37aa]
00000ABB  98                cbw
00000ABC  D1E0              shl ax,1
00000ABE  33D2              xor dx,dx
00000AC0  52                push dx
00000AC1  8BD8              mov bx,ax
00000AC3  FFB7DC33          push word [bx+0x33dc]
00000AC7  8BC6              mov ax,si
00000AC9  054300            add ax,0x43
00000ACC  50                push ax
00000ACD  8BC7              mov ax,di
00000ACF  051500            add ax,0x15
00000AD2  50                push ax
00000AD3  9A7A05960A        call 0xa96:0x57a
00000AD8  83C40E            add sp,byte +0xe
00000ADB  5F                pop di
00000ADC  5E                pop si
00000ADD  8BE5              mov sp,bp
00000ADF  5D                pop bp
00000AE0  CB                retf

; ---- 0535:0AE1 ----
00000AE1  55                push bp
00000AE2  8BEC              mov bp,sp
00000AE4  A0AA37            mov al,[0x37aa]
00000AE7  98                cbw
00000AE8  D1E0              shl ax,1
00000AEA  8BD8              mov bx,ax
00000AEC  8B877A15          mov ax,[bx+0x157a]
00000AF0  48                dec ax
00000AF1  50                push ax
00000AF2  A0AA37            mov al,[0x37aa]
00000AF5  98                cbw
00000AF6  D1E0              shl ax,1
00000AF8  8BD8              mov bx,ax
00000AFA  58                pop ax
00000AFB  89877A15          mov [bx+0x157a],ax
00000AFF  0BC0              or ax,ax
00000B01  751A              jnz 0xb1d
00000B03  B000              mov al,0x0
00000B05  50                push ax
00000B06  9A3F0B3505        call 0x535:0xb3f
00000B0B  59                pop cx
00000B0C  A0AA37            mov al,[0x37aa]
00000B0F  98                cbw
00000B10  D1E0              shl ax,1
00000B12  8B5606            mov dx,[bp+0x6]
00000B15  8BD8              mov bx,ax
00000B17  89977A15          mov [bx+0x157a],dx
00000B1B  EB20              jmp short 0xb3d
00000B1D  A0AA37            mov al,[0x37aa]
00000B20  98                cbw
00000B21  D1E0              shl ax,1
00000B23  50                push ax
00000B24  8B4606            mov ax,[bp+0x6]
00000B27  BB0200            mov bx,0x2
00000B2A  99                cwd
00000B2B  F7FB              idiv bx
00000B2D  5B                pop bx
00000B2E  39877A15          cmp [bx+0x157a],ax
00000B32  7509              jnz 0xb3d
00000B34  B001              mov al,0x1
00000B36  50                push ax
00000B37  9A3F0B3505        call 0x535:0xb3f
00000B3C  59                pop cx
00000B3D  5D                pop bp
00000B3E  CB                retf

; ---- 0535:0B3F ----
00000B3F  55                push bp
00000B40  8BEC              mov bp,sp
00000B42  4C                dec sp
00000B43  4C                dec sp
00000B44  807E0600          cmp byte [bp+0x6],0x0
00000B48  7504              jnz 0xb4e
00000B4A  B001              mov al,0x1
00000B4C  EB02              jmp short 0xb50
00000B4E  B004              mov al,0x4
00000B50  8846FF            mov [bp-0x1],al
00000B53  8A46FF            mov al,[bp-0x1]
00000B56  B400              mov ah,0x0
00000B58  50                push ax
00000B59  B84600            mov ax,0x46
00000B5C  50                push ax
00000B5D  B82E00            mov ax,0x2e
00000B60  50                push ax
00000B61  A0AA37            mov al,[0x37aa]
00000B64  98                cbw
00000B65  D1E0              shl ax,1
00000B67  8BD8              mov bx,ax
00000B69  8B877008          mov ax,[bx+0x870]
00000B6D  050300            add ax,0x3
00000B70  50                push ax
00000B71  A0AA37            mov al,[0x37aa]
00000B74  98                cbw
00000B75  D1E0              shl ax,1
00000B77  8BD8              mov bx,ax
00000B79  8B876808          mov ax,[bx+0x868]
00000B7D  050700            add ax,0x7
00000B80  50                push ax
00000B81  9A8104960A        call 0xa96:0x481
00000B86  83C40A            add sp,byte +0xa
00000B89  8BE5              mov sp,bp
00000B8B  5D                pop bp
00000B8C  CB                retf

; ---- 0535:0B8D ----
00000B8D  55                push bp
00000B8E  8BEC              mov bp,sp
00000B90  56                push si
00000B91  803EE64003        cmp byte [0x40e6],0x3
00000B96  751F              jnz 0xbb7
00000B98  33F6              xor si,si
00000B9A  EB16              jmp short 0xbb2
00000B9C  8BDE              mov bx,si
00000B9E  D1E3              shl bx,1
00000BA0  FFB79300          push word [bx+0x93]
00000BA4  8BC6              mov ax,si
00000BA6  050400            add ax,0x4
00000BA9  50                push ax
00000BAA  9AC902FD21        call 0x21fd:0x2c9
00000BAF  59                pop cx
00000BB0  59                pop cx
00000BB1  46                inc si
00000BB2  83FE03            cmp si,byte +0x3
00000BB5  7EE5              jng 0xb9c
00000BB7  803EE64005        cmp byte [0x40e6],0x5
00000BBC  753C              jnz 0xbfa
00000BBE  33F6              xor si,si
00000BC0  EB16              jmp short 0xbd8
00000BC2  8BDE              mov bx,si
00000BC4  D1E3              shl bx,1
00000BC6  FFB79B00          push word [bx+0x9b]
00000BCA  8BC6              mov ax,si
00000BCC  050400            add ax,0x4
00000BCF  50                push ax
00000BD0  9AC902FD21        call 0x21fd:0x2c9
00000BD5  59                pop cx
00000BD6  59                pop cx
00000BD7  46                inc si
00000BD8  83FE03            cmp si,byte +0x3
00000BDB  7EE5              jng 0xbc2
00000BDD  B86705            mov ax,0x567
00000BE0  50                push ax
00000BE1  33C0              xor ax,ax
00000BE3  50                push ax
00000BE4  9AC902FD21        call 0x21fd:0x2c9
00000BE9  59                pop cx
00000BEA  59                pop cx
00000BEB  B84702            mov ax,0x247
00000BEE  50                push ax
00000BEF  B80C00            mov ax,0xc
00000BF2  50                push ax
00000BF3  9AC902FD21        call 0x21fd:0x2c9
00000BF8  59                pop cx
00000BF9  59                pop cx
00000BFA  5E                pop si
00000BFB  5D                pop bp
00000BFC  CB                retf

; ---- 0535:0BFD ----
00000BFD  55                push bp
00000BFE  8BEC              mov bp,sp
00000C00  56                push si
00000C01  C606AA3700        mov byte [0x37aa],0x0
00000C06  EB76              jmp short 0xc7e
00000C08  A0AA37            mov al,[0x37aa]
00000C0B  98                cbw
00000C0C  BA1A00            mov dx,0x1a
00000C0F  F7EA              imul dx
00000C11  8BD8              mov bx,ax
00000C13  C7870E330A00      mov word [bx+0x330e],0xa
00000C19  B8DA00            mov ax,0xda
00000C1C  50                push ax
00000C1D  A0AA37            mov al,[0x37aa]
00000C20  98                cbw
00000C21  B105              mov cl,0x5
00000C23  D3E0              shl ax,cl
00000C25  50                push ax
00000C26  B8D000            mov ax,0xd0
00000C29  50                push ax
00000C2A  A0AA37            mov al,[0x37aa]
00000C2D  98                cbw
00000C2E  B105              mov cl,0x5
00000C30  D3E0              shl ax,cl
00000C32  50                push ax
00000C33  A0AA37            mov al,[0x37aa]
00000C36  98                cbw
00000C37  50                push ax
00000C38  9A2B02AF21        call 0x21af:0x22b
00000C3D  83C40A            add sp,byte +0xa
00000C40  33F6              xor si,si
00000C42  EB31              jmp short 0xc75
00000C44  B83A00            mov ax,0x3a
00000C47  50                push ax
00000C48  8BC6              mov ax,si
00000C4A  B104              mov cl,0x4
00000C4C  D3E0              shl ax,cl
00000C4E  05D000            add ax,0xd0
00000C51  50                push ax
00000C52  A0AA37            mov al,[0x37aa]
00000C55  98                cbw
00000C56  BA0A00            mov dx,0xa
00000C59  F7EA              imul dx
00000C5B  50                push ax
00000C5C  8BC6              mov ax,si
00000C5E  B104              mov cl,0x4
00000C60  D3E0              shl ax,cl
00000C62  05E000            add ax,0xe0
00000C65  50                push ax
00000C66  56                push si
00000C67  A0AA37            mov al,[0x37aa]
00000C6A  98                cbw
00000C6B  50                push ax
00000C6C  9ACA00AF21        call 0x21af:0xca
00000C71  83C40C            add sp,byte +0xc
00000C74  46                inc si
00000C75  83FE05            cmp si,byte +0x5
00000C78  7ECA              jng 0xc44
00000C7A  FE06AA37          inc byte [0x37aa]
00000C7E  803EAA3704        cmp byte [0x37aa],0x4
00000C83  7C83              jl 0xc08
00000C85  5E                pop si
00000C86  5D                pop bp
00000C87  CB                retf

; ---- 0535:0C88 ----
00000C88  55                push bp
00000C89  8BEC              mov bp,sp
00000C8B  C606AA3700        mov byte [0x37aa],0x0
00000C90  EB18              jmp short 0xcaa
00000C92  9AB30C3505        call 0x535:0xcb3
00000C97  33C0              xor ax,ax
00000C99  50                push ax
00000C9A  A0AA37            mov al,[0x37aa]
00000C9D  98                cbw
00000C9E  50                push ax
00000C9F  9A1204AF21        call 0x21af:0x412
00000CA4  59                pop cx
00000CA5  59                pop cx
00000CA6  FE06AA37          inc byte [0x37aa]
00000CAA  803EAA3704        cmp byte [0x37aa],0x4
00000CAF  7CE1              jl 0xc92
00000CB1  5D                pop bp
00000CB2  CB                retf

; ---- 0535:0CB3 ----
00000CB3  55                push bp
00000CB4  8BEC              mov bp,sp
00000CB6  A0AA37            mov al,[0x37aa]
00000CB9  98                cbw
00000CBA  D1E0              shl ax,1
00000CBC  8BD8              mov bx,ax
00000CBE  8B875230          mov ax,[bx+0x3052]
00000CC2  D1E0              shl ax,1
00000CC4  D1E0              shl ax,1
00000CC6  D1E0              shl ax,1
00000CC8  053700            add ax,0x37
00000CCB  50                push ax
00000CCC  A0AA37            mov al,[0x37aa]
00000CCF  98                cbw
00000CD0  BA1A00            mov dx,0x1a
00000CD3  F7EA              imul dx
00000CD5  8BD8              mov bx,ax
00000CD7  58                pop ax
00000CD8  89872033          mov [bx+0x3320],ax
00000CDC  A0AA37            mov al,[0x37aa]
00000CDF  98                cbw
00000CE0  D1E0              shl ax,1
00000CE2  8BD8              mov bx,ax
00000CE4  8B875E30          mov ax,[bx+0x305e]
00000CE8  D1E0              shl ax,1
00000CEA  D1E0              shl ax,1
00000CEC  D1E0              shl ax,1
00000CEE  05FBFF            add ax,0xfffb
00000CF1  50                push ax
00000CF2  A0AA37            mov al,[0x37aa]
00000CF5  98                cbw
00000CF6  BA1A00            mov dx,0x1a
00000CF9  F7EA              imul dx
00000CFB  8BD8              mov bx,ax
00000CFD  58                pop ax
00000CFE  89872233          mov [bx+0x3322],ax
00000D02  5D                pop bp
00000D03  CB                retf

; ---- 0535:0D04 ----
00000D04  55                push bp
00000D05  8BEC              mov bp,sp
00000D07  56                push si
00000D08  FF0EA500          dec word [0xa5]
00000D0C  7533              jnz 0xd41
00000D0E  FF06A300          inc word [0xa3]
00000D12  A1A300            mov ax,[0xa3]
00000D15  3D0500            cmp ax,0x5
00000D18  7E06              jng 0xd20
00000D1A  C706A3000000      mov word [0xa3],0x0
00000D20  33F6              xor si,si
00000D22  EB12              jmp short 0xd36
00000D24  8BC6              mov ax,si
00000D26  BA1A00            mov dx,0x1a
00000D29  F7EA              imul dx
00000D2B  8B16A300          mov dx,[0xa3]
00000D2F  8BD8              mov bx,ax
00000D31  89972433          mov [bx+0x3324],dx
00000D35  46                inc si
00000D36  83FE03            cmp si,byte +0x3
00000D39  7EE9              jng 0xd24
00000D3B  C7                db 0xc7
00000D3C  06                push es
00000D3D  A5                movsw
00000D3E  0004              add [si],al
