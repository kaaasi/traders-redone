; segment 0EDE  file offset 0x123E0  size 0x23F0
00000000  FE03              inc byte [bp+di]
00000002  7E84              jng 0xff88
00000004  5E                pop si
00000005  8BE5              mov sp,bp
00000007  5D                pop bp
00000008  CB                retf

; ---- 0EDE:0009 ----
00000009  55                push bp
0000000A  8BEC              mov bp,sp
0000000C  33C0              xor ax,ax
0000000E  50                push ax
0000000F  9AB403FD21        call 0x21fd:0x3b4
00000014  59                pop cx
00000015  9AAC20DE0E        call 0xede:0x20ac
0000001A  9A4C000F15        call 0x150f:0x4c
0000001F  9A8803DE0E        call 0xede:0x388
00000024  C606C73700        mov byte [0x37c7],0x0
00000029  C60688340F        mov byte [0x3488],0xf
0000002E  C7061A3D0100      mov word [0x3d1a],0x1
00000034  C6063E2C00        mov byte [0x2c3e],0x0
00000039  9ABB1CDE0E        call 0xede:0x1cbb
0000003E  9A5303FD21        call 0x21fd:0x353
00000043  9A1B00960A        call 0xa96:0x1b
00000048  EB6A              jmp short 0xb4
0000004A  803E483D00        cmp byte [0x3d48],0x0
0000004F  7402              jz 0x53
00000051  EB76              jmp short 0xc9
00000053  9A1F05FD21        call 0x21fd:0x51f
00000058  FF362E08          push word [0x82e]
0000005C  9A1C07CD23        call 0x23cd:0x71c
00000061  59                pop cx
00000062  9AA806960A        call 0xa96:0x6a8
00000067  9A1B00960A        call 0xa96:0x1b
0000006C  A11A3D            mov ax,[0x3d1a]
0000006F  48                dec ax
00000070  A31A3D            mov [0x3d1a],ax
00000073  0BC0              or ax,ax
00000075  7506              jnz 0x7d
00000077  C7061A3D1027      mov word [0x3d1a],0x2710
0000007D  9A4502FD21        call 0x21fd:0x245
00000082  9A6203DE0E        call 0xede:0x362
00000087  9AE4060F15        call 0x150f:0x6e4
0000008C  9A0E009E0E        call 0xe9e:0xe
00000091  C606F44000        mov byte [0x40f4],0x0
00000096  9AF100DE0E        call 0xede:0xf1
0000009B  9ABA23DE0E        call 0xede:0x23ba
000000A0  9ACA0B7E1E        call 0x1e7e:0xbca
000000A5  9AA806960A        call 0xa96:0x6a8
000000AA  FF362E08          push word [0x82e]
000000AE  9AE906CD23        call 0x23cd:0x6e9
000000B3  59                pop cx
000000B4  803E883400        cmp byte [0x3488],0x0
000000B9  758F              jnz 0x4a
000000BB  803EC73700        cmp byte [0x37c7],0x0
000000C0  7588              jnz 0x4a
000000C2  803E3E2C00        cmp byte [0x2c3e],0x0
000000C7  7581              jnz 0x4a
000000C9  9A6506FD21        call 0x21fd:0x665
000000CE  C606AA3700        mov byte [0x37aa],0x0
000000D3  EB13              jmp short 0xe8
000000D5  9A3323DE0E        call 0xede:0x2333
000000DA  9AAA23DE0E        call 0xede:0x23aa
000000DF  9A2D1CDE0E        call 0xede:0x1c2d
000000E4  FE06AA37          inc byte [0x37aa]
000000E8  803EAA3704        cmp byte [0x37aa],0x4
000000ED  7CE6              jl 0xd5
000000EF  5D                pop bp
000000F0  CB                retf

; ---- 0EDE:00F1 ----
000000F1  55                push bp
000000F2  8BEC              mov bp,sp
000000F4  C606AA3700        mov byte [0x37aa],0x0
000000F9  EB69              jmp short 0x164
000000FB  A0AA37            mov al,[0x37aa]
000000FE  98                cbw
000000FF  8BD8              mov bx,ax
00000101  8A87B243          mov al,[bx+0x43b2]
00000105  B400              mov ah,0x0
00000107  3D0F00            cmp ax,0xf
0000010A  740E              jz 0x11a
0000010C  3D1000            cmp ax,0x10
0000010F  7402              jz 0x113
00000111  EB0E              jmp short 0x121
00000113  9A6D01DE0E        call 0xede:0x16d
00000118  EB07              jmp short 0x121
0000011A  9A2402DE0E        call 0xede:0x224
0000011F  EB00              jmp short 0x121
00000121  A0AA37            mov al,[0x37aa]
00000124  98                cbw
00000125  D1E0              shl ax,1
00000127  8BD8              mov bx,ax
00000129  8B87A82E          mov ax,[bx+0x2ea8]
0000012D  3D0100            cmp ax,0x1
00000130  7407              jz 0x139
00000132  3D0200            cmp ax,0x2
00000135  7409              jz 0x140
00000137  EB0E              jmp short 0x147
00000139  9AEF0BDE0E        call 0xede:0xbef
0000013E  EB07              jmp short 0x147
00000140  9AA10FDE0E        call 0xede:0xfa1
00000145  EB00              jmp short 0x147
00000147  A0AA37            mov al,[0x37aa]
0000014A  98                cbw
0000014B  D1E0              shl ax,1
0000014D  8BD8              mov bx,ax
0000014F  83BFE83C00        cmp word [bx+0x3ce8],byte +0x0
00000154  7405              jz 0x15b
00000156  9A261DDE0E        call 0xede:0x1d26
0000015B  9A9C18DE0E        call 0xede:0x189c
00000160  FE06AA37          inc byte [0x37aa]
00000164  803EAA3704        cmp byte [0x37aa],0x4
00000169  7C90              jl 0xfb
0000016B  5D                pop bp
0000016C  CB                retf

; ---- 0EDE:016D ----
0000016D  55                push bp
0000016E  8BEC              mov bp,sp
00000170  A0AA37            mov al,[0x37aa]
00000173  98                cbw
00000174  D1E0              shl ax,1
00000176  8BD8              mov bx,ax
00000178  83BF262C00        cmp word [bx+0x2c26],byte +0x0
0000017D  7624              jna 0x1a3
0000017F  A0AA37            mov al,[0x37aa]
00000182  98                cbw
00000183  D1E0              shl ax,1
00000185  8BD8              mov bx,ax
00000187  8B87262C          mov ax,[bx+0x2c26]
0000018B  48                dec ax
0000018C  50                push ax
0000018D  A0AA37            mov al,[0x37aa]
00000190  98                cbw
00000191  D1E0              shl ax,1
00000193  8BD8              mov bx,ax
00000195  58                pop ax
00000196  8987262C          mov [bx+0x2c26],ax
0000019A  0BC0              or ax,ax
0000019C  7505              jnz 0x1a3
0000019E  9AB21DDE0E        call 0xede:0x1db2
000001A3  A0AA37            mov al,[0x37aa]
000001A6  98                cbw
000001A7  8BD8              mov bx,ax
000001A9  80BFA63700        cmp byte [bx+0x37a6],0x0
000001AE  7E05              jng 0x1b5
000001B0  9AD216DE0E        call 0xede:0x16d2
000001B5  A0AA37            mov al,[0x37aa]
000001B8  98                cbw
000001B9  8BD8              mov bx,ax
000001BB  80BFBE4400        cmp byte [bx+0x44be],0x0
000001C0  7407              jz 0x1c9
000001C2  9AB006DE0E        call 0xede:0x6b0
000001C7  EB12              jmp short 0x1db
000001C9  A0AA37            mov al,[0x37aa]
000001CC  98                cbw
000001CD  8BD8              mov bx,ax
000001CF  80BFF04300        cmp byte [bx+0x43f0],0x0
000001D4  7405              jz 0x1db
000001D6  9A2306DE0E        call 0xede:0x623
000001DB  A0AA37            mov al,[0x37aa]
000001DE  98                cbw
000001DF  8BD8              mov bx,ax
000001E1  80BF9D4300        cmp byte [bx+0x439d],0x0
000001E6  740B              jz 0x1f3
000001E8  B001              mov al,0x1
000001EA  50                push ax
000001EB  9A3E07DE0E        call 0xede:0x73e
000001F0  59                pop cx
000001F1  EB16              jmp short 0x209
000001F3  A0AA37            mov al,[0x37aa]
000001F6  98                cbw
000001F7  8BD8              mov bx,ax
000001F9  80BFBA4400        cmp byte [bx+0x44ba],0x0
000001FE  7409              jz 0x209
00000200  B0FF              mov al,0xff
00000202  50                push ax
00000203  9A3E07DE0E        call 0xede:0x73e
00000208  59                pop cx
00000209  A0AA37            mov al,[0x37aa]
0000020C  98                cbw
0000020D  8BD8              mov bx,ax
0000020F  80BF944800        cmp byte [bx+0x4894],0x0
00000214  7407              jz 0x21d
00000216  9AD11CDE0E        call 0xede:0x1cd1
0000021B  EB05              jmp short 0x222
0000021D  9A4805DE0E        call 0xede:0x548
00000222  5D                pop bp
00000223  CB                retf

; ---- 0EDE:0224 ----
00000224  55                push bp
00000225  8BEC              mov bp,sp
00000227  B83C00            mov ax,0x3c
0000022A  50                push ax
0000022B  9AE10A3505        call 0x535:0xae1
00000230  59                pop cx
00000231  A0AA37            mov al,[0x37aa]
00000234  98                cbw
00000235  8A168834          mov dl,[0x3488]
00000239  8BD8              mov bx,ax
0000023B  8497C608          test [bx+0x8c6],dl
0000023F  7503              jnz 0x244
00000241  E9BC00            jmp 0x300
00000244  A0AA37            mov al,[0x37aa]
00000247  98                cbw
00000248  8BD8              mov bx,ax
0000024A  80BF944800        cmp byte [bx+0x4894],0x0
0000024F  744B              jz 0x29c
00000251  A0AA37            mov al,[0x37aa]
00000254  98                cbw
00000255  8BD8              mov bx,ax
00000257  80BFD24501        cmp byte [bx+0x45d2],0x1
0000025C  753E              jnz 0x29c
0000025E  B000              mov al,0x0
00000260  50                push ax
00000261  9A3F0B3505        call 0x535:0xb3f
00000266  59                pop cx
00000267  A0AA37            mov al,[0x37aa]
0000026A  98                cbw
0000026B  8BD8              mov bx,ax
0000026D  C687B24310        mov byte [bx+0x43b2],0x10
00000272  9A5F22DE0E        call 0xede:0x225f
00000277  A0AA37            mov al,[0x37aa]
0000027A  98                cbw
0000027B  8BD8              mov bx,ax
0000027D  80BF400800        cmp byte [bx+0x840],0x0
00000282  7516              jnz 0x29a
00000284  A0AA37            mov al,[0x37aa]
00000287  98                cbw
00000288  D1E0              shl ax,1
0000028A  D1E0              shl ax,1
0000028C  8BD8              mov bx,ax
0000028E  C7872E30E803      mov word [bx+0x302e],0x3e8
00000294  C78730300000      mov word [bx+0x3030],0x0
0000029A  EB62              jmp short 0x2fe
0000029C  A0AA37            mov al,[0x37aa]
0000029F  98                cbw
000002A0  8BD8              mov bx,ax
000002A2  80BF9D4300        cmp byte [bx+0x439d],0x0
000002A7  7455              jz 0x2fe
000002A9  9A5C093505        call 0x535:0x95c
000002AE  B80B00            mov ax,0xb
000002B1  50                push ax
000002B2  B80100            mov ax,0x1
000002B5  50                push ax
000002B6  FF362E38          push word [0x382e]
000002BA  FF362C38          push word [0x382c]
000002BE  B82C00            mov ax,0x2c
000002C1  50                push ax
000002C2  A0AA37            mov al,[0x37aa]
000002C5  98                cbw
000002C6  D1E0              shl ax,1
000002C8  8BD8              mov bx,ax
000002CA  8B877008          mov ax,[bx+0x870]
000002CE  052700            add ax,0x27
000002D1  50                push ax
000002D2  A0AA37            mov al,[0x37aa]
000002D5  98                cbw
000002D6  D1E0              shl ax,1
000002D8  8BD8              mov bx,ax
000002DA  8B876808          mov ax,[bx+0x868]
000002DE  050800            add ax,0x8
000002E1  50                push ax
000002E2  9A9000690A        call 0xa69:0x90
000002E7  83C40E            add sp,byte +0xe
000002EA  A0AA37            mov al,[0x37aa]
000002ED  98                cbw
000002EE  B2FF              mov dl,0xff
000002F0  8BD8              mov bx,ax
000002F2  2A97C608          sub dl,[bx+0x8c6]
000002F6  A08834            mov al,[0x3488]
000002F9  22C2              and al,dl
000002FB  A28834            mov [0x3488],al
000002FE  EB20              jmp short 0x320
00000300  A0AA37            mov al,[0x37aa]
00000303  98                cbw
00000304  8BD8              mov bx,ax
00000306  80BFBA4400        cmp byte [bx+0x44ba],0x0
0000030B  7413              jz 0x320
0000030D  9AB21DDE0E        call 0xede:0x1db2
00000312  A0AA37            mov al,[0x37aa]
00000315  98                cbw
00000316  8BD8              mov bx,ax
00000318  8A87C608          mov al,[bx+0x8c6]
0000031C  08068834          or [0x3488],al
00000320  5D                pop bp
00000321  CB                retf

; ---- 0EDE:0322 ----
00000322  55                push bp
00000323  8BEC              mov bp,sp
00000325  4C                dec sp
00000326  4C                dec sp
00000327  9A2618DE0E        call 0xede:0x1826
0000032C  3C01              cmp al,0x1
0000032E  7506              jnz 0x336
00000330  C646FF00          mov byte [bp-0x1],0x0
00000334  EB1B              jmp short 0x351
00000336  A0AA37            mov al,[0x37aa]
00000339  98                cbw
0000033A  D1E0              shl ax,1
0000033C  D1E0              shl ax,1
0000033E  8BD8              mov bx,ax
00000340  83BF6C3700        cmp word [bx+0x376c],byte +0x0
00000345  7506              jnz 0x34d
00000347  C646FF00          mov byte [bp-0x1],0x0
0000034B  EB04              jmp short 0x351
0000034D  C646FF01          mov byte [bp-0x1],0x1
00000351  A0AA37            mov al,[0x37aa]
00000354  98                cbw
00000355  8A56FF            mov dl,[bp-0x1]
00000358  8BD8              mov bx,ax
0000035A  8897D245          mov [bx+0x45d2],dl
0000035E  8BE5              mov sp,bp
00000360  5D                pop bp
00000361  CB                retf

; ---- 0EDE:0362 ----
00000362  55                push bp
00000363  8BEC              mov bp,sp
00000365  803EC73700        cmp byte [0x37c7],0x0
0000036A  7502              jnz 0x36e
0000036C  EB18              jmp short 0x386
0000036E  F606C73701        test byte [0x37c7],0x1
00000373  7405              jz 0x37a
00000375  9A2E010F15        call 0x150f:0x12e
0000037A  F606C73702        test byte [0x37c7],0x2
0000037F  7405              jz 0x386
00000381  9A7E1DDE0E        call 0xede:0x1d7e
00000386  5D                pop bp
00000387  CB                retf

; ---- 0EDE:0388 ----
00000388  55                push bp
00000389  8BEC              mov bp,sp
0000038B  C606AA3700        mov byte [0x37aa],0x0
00000390  E9BB00            jmp 0x44e
00000393  B000              mov al,0x0
00000395  50                push ax
00000396  9A671DDE0E        call 0xede:0x1d67
0000039B  59                pop cx
0000039C  9A8904DE0E        call 0xede:0x489
000003A1  A0AA37            mov al,[0x37aa]
000003A4  98                cbw
000003A5  D1E0              shl ax,1
000003A7  8BD8              mov bx,ax
000003A9  C787E6430000      mov word [bx+0x43e6],0x0
000003AF  9A1C05DE0E        call 0xede:0x51c
000003B4  A0AA37            mov al,[0x37aa]
000003B7  98                cbw
000003B8  8BD8              mov bx,ax
000003BA  C687B2430F        mov byte [bx+0x43b2],0xf
000003BF  A0AA37            mov al,[0x37aa]
000003C2  98                cbw
000003C3  D1E0              shl ax,1
000003C5  8BD8              mov bx,ax
000003C7  C787A82E0000      mov word [bx+0x2ea8],0x0
000003CD  A0AA37            mov al,[0x37aa]
000003D0  98                cbw
000003D1  8BD8              mov bx,ax
000003D3  C687A63700        mov byte [bx+0x37a6],0x0
000003D8  A0AA37            mov al,[0x37aa]
000003DB  98                cbw
000003DC  8BD8              mov bx,ax
000003DE  C687823700        mov byte [bx+0x3782],0x0
000003E3  A0AA37            mov al,[0x37aa]
000003E6  98                cbw
000003E7  D1E0              shl ax,1
000003E9  D1E0              shl ax,1
000003EB  8BD8              mov bx,ax
000003ED  C787983C0000      mov word [bx+0x3c98],0x0
000003F3  C7879A3C0000      mov word [bx+0x3c9a],0x0
000003F9  A0AA37            mov al,[0x37aa]
000003FC  98                cbw
000003FD  D1E0              shl ax,1
000003FF  D1E0              shl ax,1
00000401  8BD8              mov bx,ax
00000403  C7872E300000      mov word [bx+0x302e],0x0
00000409  C78730300000      mov word [bx+0x3030],0x0
0000040F  C706D0450000      mov word [0x45d0],0x0
00000415  EB1E              jmp short 0x435
00000417  A0AA37            mov al,[0x37aa]
0000041A  98                cbw
0000041B  D1E0              shl ax,1
0000041D  D1E0              shl ax,1
0000041F  D1E0              shl ax,1
00000421  8B16D045          mov dx,[0x45d0]
00000425  D1E2              shl dx,1
00000427  03C2              add ax,dx
00000429  8BD8              mov bx,ax
0000042B  C787223D0000      mov word [bx+0x3d22],0x0
00000431  FF06D045          inc word [0x45d0]
00000435  833ED04503        cmp word [0x45d0],byte +0x3
0000043A  7EDB              jng 0x417
0000043C  A0AA37            mov al,[0x37aa]
0000043F  98                cbw
00000440  D1E0              shl ax,1
00000442  8BD8              mov bx,ax
00000444  C787DA4B0000      mov word [bx+0x4bda],0x0
0000044A  FE06AA37          inc byte [0x37aa]
0000044E  803EAA3704        cmp byte [0x37aa],0x4
00000453  7D03              jnl 0x458
00000455  E93BFF            jmp 0x393
00000458  C606AA3700        mov byte [0x37aa],0x0
0000045D  EB21              jmp short 0x480
0000045F  A0AA37            mov al,[0x37aa]
00000462  98                cbw
00000463  8BD8              mov bx,ax
00000465  80BF400801        cmp byte [bx+0x840],0x1
0000046A  7510              jnz 0x47c
0000046C  A0AA37            mov al,[0x37aa]
0000046F  98                cbw
00000470  50                push ax
00000471  9A0B000F15        call 0x150f:0xb
00000476  59                pop cx
00000477  9AA0180F15        call 0x150f:0x18a0
0000047C  FE06AA37          inc byte [0x37aa]
00000480  803EAA3704        cmp byte [0x37aa],0x4
00000485  7CD8              jl 0x45f
00000487  5D                pop bp
00000488  CB                retf

; ---- 0EDE:0489 ----
00000489  55                push bp
0000048A  8BEC              mov bp,sp
0000048C  A0AA37            mov al,[0x37aa]
0000048F  98                cbw
00000490  D1E0              shl ax,1
00000492  50                push ax
00000493  A0AA37            mov al,[0x37aa]
00000496  98                cbw
00000497  8BD8              mov bx,ax
00000499  8A874408          mov al,[bx+0x844]
0000049D  B400              mov ah,0x0
0000049F  5B                pop bx
000004A0  8987E24B          mov [bx+0x4be2],ax
000004A4  A0AA37            mov al,[0x37aa]
000004A7  98                cbw
000004A8  D1E0              shl ax,1
000004AA  50                push ax
000004AB  A0AA37            mov al,[0x37aa]
000004AE  98                cbw
000004AF  8BD8              mov bx,ax
000004B1  8A874808          mov al,[bx+0x848]
000004B5  B400              mov ah,0x0
000004B7  5B                pop bx
000004B8  8987EA4B          mov [bx+0x4bea],ax
000004BC  A0AA37            mov al,[0x37aa]
000004BF  98                cbw
000004C0  D1E0              shl ax,1
000004C2  8BD8              mov bx,ax
000004C4  8B87E24B          mov ax,[bx+0x4be2]
000004C8  D1E0              shl ax,1
000004CA  D1E0              shl ax,1
000004CC  D1E0              shl ax,1
000004CE  053500            add ax,0x35
000004D1  BA0A00            mov dx,0xa
000004D4  F7EA              imul dx
000004D6  50                push ax
000004D7  A0AA37            mov al,[0x37aa]
000004DA  98                cbw
000004DB  D1E0              shl ax,1
000004DD  8BD8              mov bx,ax
000004DF  58                pop ax
000004E0  89875C37          mov [bx+0x375c],ax
000004E4  A0AA37            mov al,[0x37aa]
000004E7  98                cbw
000004E8  D1E0              shl ax,1
000004EA  8BD8              mov bx,ax
000004EC  8B87EA4B          mov ax,[bx+0x4bea]
000004F0  D1E0              shl ax,1
000004F2  D1E0              shl ax,1
000004F4  D1E0              shl ax,1
000004F6  05FCFF            add ax,0xfffc
000004F9  BA0A00            mov dx,0xa
000004FC  F7EA              imul dx
000004FE  50                push ax
000004FF  A0AA37            mov al,[0x37aa]
00000502  98                cbw
00000503  D1E0              shl ax,1
00000505  8BD8              mov bx,ax
00000507  58                pop ax
00000508  89876437          mov [bx+0x3764],ax
0000050C  A0AA37            mov al,[0x37aa]
0000050F  98                cbw
00000510  8A16AA37          mov dl,[0x37aa]
00000514  8BD8              mov bx,ax
00000516  8897143D          mov [bx+0x3d14],dl
0000051A  5D                pop bp
0000051B  CB                retf

; ---- 0EDE:051C ----
0000051C  55                push bp
0000051D  8BEC              mov bp,sp
0000051F  56                push si
00000520  9A1B00960A        call 0xa96:0x1b
00000525  9AB21DDE0E        call 0xede:0x1db2
0000052A  33F6              xor si,si
0000052C  EB08              jmp short 0x536
0000052E  56                push si
0000052F  9AFF1DDE0E        call 0xede:0x1dff
00000534  59                pop cx
00000535  46                inc si
00000536  83FE05            cmp si,byte +0x5
00000539  7EF3              jng 0x52e
0000053B  9A561B0F15        call 0x150f:0x1b56
00000540  9A0D06FD21        call 0x21fd:0x60d
00000545  5E                pop si
00000546  5D                pop bp
00000547  CB                retf

; ---- 0EDE:0548 ----
00000548  55                push bp
00000549  8BEC              mov bp,sp
0000054B  83EC04            sub sp,byte +0x4
0000054E  A0AA37            mov al,[0x37aa]
00000551  98                cbw
00000552  D1E0              shl ax,1
00000554  D1E0              shl ax,1
00000556  8BD8              mov bx,ax
00000558  83BF9A3C00        cmp word [bx+0x3c9a],byte +0x0
0000055D  770C              ja 0x56b
0000055F  7207              jc 0x568
00000561  83BF983C01        cmp word [bx+0x3c98],byte +0x1
00000566  7303              jnc 0x56b
00000568  E9B400            jmp 0x61f
0000056B  A0AA37            mov al,[0x37aa]
0000056E  98                cbw
0000056F  D1E0              shl ax,1
00000571  D1E0              shl ax,1
00000573  8BD8              mov bx,ax
00000575  8B879A3C          mov ax,[bx+0x3c9a]
00000579  8B97983C          mov dx,[bx+0x3c98]
0000057D  8956FC            mov [bp-0x4],dx
00000580  8946FE            mov [bp-0x2],ax
00000583  837EFE00          cmp word [bp-0x2],byte +0x0
00000587  7713              ja 0x59c
00000589  7206              jc 0x591
0000058B  837EFC19          cmp word [bp-0x4],byte +0x19
0000058F  730B              jnc 0x59c
00000591  B000              mov al,0x0
00000593  50                push ax
00000594  9A370ADE0E        call 0xede:0xa37
00000599  59                pop cx
0000059A  EB64              jmp short 0x600
0000059C  837EFE00          cmp word [bp-0x2],byte +0x0
000005A0  7713              ja 0x5b5
000005A2  7206              jc 0x5aa
000005A4  837EFC32          cmp word [bp-0x4],byte +0x32
000005A8  730B              jnc 0x5b5
000005AA  B001              mov al,0x1
000005AC  50                push ax
000005AD  9A370ADE0E        call 0xede:0xa37
000005B2  59                pop cx
000005B3  EB4B              jmp short 0x600
000005B5  A0AA37            mov al,[0x37aa]
000005B8  98                cbw
000005B9  D1E0              shl ax,1
000005BB  50                push ax
000005BC  A0AA37            mov al,[0x37aa]
000005BF  98                cbw
000005C0  8BD8              mov bx,ax
000005C2  8A874408          mov al,[bx+0x844]
000005C6  B400              mov ah,0x0
000005C8  5B                pop bx
000005C9  3987E24B          cmp [bx+0x4be2],ax
000005CD  752C              jnz 0x5fb
000005CF  A0AA37            mov al,[0x37aa]
000005D2  98                cbw
000005D3  D1E0              shl ax,1
000005D5  50                push ax
000005D6  A0AA37            mov al,[0x37aa]
000005D9  98                cbw
000005DA  8BD8              mov bx,ax
000005DC  8A874808          mov al,[bx+0x848]
000005E0  B400              mov ah,0x0
000005E2  5B                pop bx
000005E3  3987EA4B          cmp [bx+0x4bea],ax
000005E7  7512              jnz 0x5fb
000005E9  9A3323DE0E        call 0xede:0x2333
000005EE  A0AA37            mov al,[0x37aa]
000005F1  98                cbw
000005F2  8BD8              mov bx,ax
000005F4  C687B2430F        mov byte [bx+0x43b2],0xf
000005F9  EB05              jmp short 0x600
000005FB  9AEA09DE0E        call 0xede:0x9ea
00000600  B000              mov al,0x0
00000602  50                push ax
00000603  9A671DDE0E        call 0xede:0x1d67
00000608  59                pop cx
00000609  A0AA37            mov al,[0x37aa]
0000060C  98                cbw
0000060D  D1E0              shl ax,1
0000060F  D1E0              shl ax,1
00000611  8BD8              mov bx,ax
00000613  C787983C0000      mov word [bx+0x3c98],0x0
00000619  C7879A3C0000      mov word [bx+0x3c9a],0x0
0000061F  8BE5              mov sp,bp
00000621  5D                pop bp
00000622  CB                retf

; ---- 0EDE:0623 ----
00000623  55                push bp
00000624  8BEC              mov bp,sp
00000626  C606F44001        mov byte [0x40f4],0x1
0000062B  A0AA37            mov al,[0x37aa]
0000062E  98                cbw
0000062F  D1E0              shl ax,1
00000631  8BD8              mov bx,ax
00000633  8B87CC06          mov ax,[bx+0x6cc]
00000637  48                dec ax
00000638  50                push ax
00000639  A0AA37            mov al,[0x37aa]
0000063C  98                cbw
0000063D  D1E0              shl ax,1
0000063F  8BD8              mov bx,ax
00000641  58                pop ax
00000642  8987CC06          mov [bx+0x6cc],ax
00000646  0BC0              or ax,ax
00000648  7564              jnz 0x6ae
0000064A  A0AA37            mov al,[0x37aa]
0000064D  98                cbw
0000064E  D1E0              shl ax,1
00000650  8BD8              mov bx,ax
00000652  8B87E643          mov ax,[bx+0x43e6]
00000656  48                dec ax
00000657  50                push ax
00000658  A0AA37            mov al,[0x37aa]
0000065B  98                cbw
0000065C  D1E0              shl ax,1
0000065E  8BD8              mov bx,ax
00000660  58                pop ax
00000661  8987E643          mov [bx+0x43e6],ax
00000665  0BC0              or ax,ax
00000667  7D0E              jnl 0x677
00000669  A0AA37            mov al,[0x37aa]
0000066C  98                cbw
0000066D  D1E0              shl ax,1
0000066F  8BD8              mov bx,ax
00000671  C787E6430F00      mov word [bx+0x43e6],0xf
00000677  9ACF22DE0E        call 0xede:0x22cf
0000067C  9AC31FDE0E        call 0xede:0x1fc3
00000681  A0AA37            mov al,[0x37aa]
00000684  98                cbw
00000685  BA0600            mov dx,0x6
00000688  F7EA              imul dx
0000068A  8BD8              mov bx,ax
0000068C  8A87D243          mov al,[bx+0x43d2]
00000690  B400              mov ah,0x0
00000692  BB0A00            mov bx,0xa
00000695  99                cwd
00000696  F7FB              idiv bx
00000698  D1E0              shl ax,1
0000069A  8BD8              mov bx,ax
0000069C  8B87BE06          mov ax,[bx+0x6be]
000006A0  50                push ax
000006A1  A0AA37            mov al,[0x37aa]
000006A4  98                cbw
000006A5  D1E0              shl ax,1
000006A7  8BD8              mov bx,ax
000006A9  58                pop ax
000006AA  8987CC06          mov [bx+0x6cc],ax
000006AE  5D                pop bp
000006AF  CB                retf

; ---- 0EDE:06B0 ----
000006B0  55                push bp
000006B1  8BEC              mov bp,sp
000006B3  C606F44001        mov byte [0x40f4],0x1
000006B8  A0AA37            mov al,[0x37aa]
000006BB  98                cbw
000006BC  D1E0              shl ax,1
000006BE  8BD8              mov bx,ax
000006C0  8B87D406          mov ax,[bx+0x6d4]
000006C4  48                dec ax
000006C5  50                push ax
000006C6  A0AA37            mov al,[0x37aa]
000006C9  98                cbw
000006CA  D1E0              shl ax,1
000006CC  8BD8              mov bx,ax
000006CE  58                pop ax
000006CF  8987D406          mov [bx+0x6d4],ax
000006D3  0BC0              or ax,ax
000006D5  7565              jnz 0x73c
000006D7  A0AA37            mov al,[0x37aa]
000006DA  98                cbw
000006DB  D1E0              shl ax,1
000006DD  8BD8              mov bx,ax
000006DF  8B87E643          mov ax,[bx+0x43e6]
000006E3  40                inc ax
000006E4  50                push ax
000006E5  A0AA37            mov al,[0x37aa]
000006E8  98                cbw
000006E9  D1E0              shl ax,1
000006EB  8BD8              mov bx,ax
000006ED  58                pop ax
000006EE  8987E643          mov [bx+0x43e6],ax
000006F2  3D0F00            cmp ax,0xf
000006F5  7E0E              jng 0x705
000006F7  A0AA37            mov al,[0x37aa]
000006FA  98                cbw
000006FB  D1E0              shl ax,1
000006FD  8BD8              mov bx,ax
000006FF  C787E6430000      mov word [bx+0x43e6],0x0
00000705  9ACF22DE0E        call 0xede:0x22cf
0000070A  9AC31FDE0E        call 0xede:0x1fc3
0000070F  A0AA37            mov al,[0x37aa]
00000712  98                cbw
00000713  BA0600            mov dx,0x6
00000716  F7EA              imul dx
00000718  8BD8              mov bx,ax
0000071A  8A87D243          mov al,[bx+0x43d2]
0000071E  B400              mov ah,0x0
00000720  BB0A00            mov bx,0xa
00000723  99                cwd
00000724  F7FB              idiv bx
00000726  D1E0              shl ax,1
00000728  8BD8              mov bx,ax
0000072A  8B87BE06          mov ax,[bx+0x6be]
0000072E  50                push ax
0000072F  A0AA37            mov al,[0x37aa]
00000732  98                cbw
00000733  D1E0              shl ax,1
00000735  8BD8              mov bx,ax
00000737  58                pop ax
00000738  8987D406          mov [bx+0x6d4],ax
0000073C  5D                pop bp
0000073D  CB                retf

; ---- 0EDE:073E ----
0000073E  55                push bp
0000073F  8BEC              mov bp,sp
00000741  83EC08            sub sp,byte +0x8
00000744  56                push si
00000745  57                push di
00000746  C606F44001        mov byte [0x40f4],0x1
0000074B  A0AA37            mov al,[0x37aa]
0000074E  98                cbw
0000074F  D1E0              shl ax,1
00000751  8BD8              mov bx,ax
00000753  8B870707          mov ax,[bx+0x707]
00000757  48                dec ax
00000758  50                push ax
00000759  A0AA37            mov al,[0x37aa]
0000075C  98                cbw
0000075D  D1E0              shl ax,1
0000075F  8BD8              mov bx,ax
00000761  58                pop ax
00000762  89870707          mov [bx+0x707],ax
00000766  0BC0              or ax,ax
00000768  7403              jz 0x76d
0000076A  E9AC01            jmp 0x919
0000076D  A0AA37            mov al,[0x37aa]
00000770  98                cbw
00000771  D1E0              shl ax,1
00000773  8BD8              mov bx,ax
00000775  8BB7E643          mov si,[bx+0x43e6]
00000779  8BC6              mov ax,si
0000077B  BB0400            mov bx,0x4
0000077E  99                cwd
0000077F  F7FB              idiv bx
00000781  8BDA              mov bx,dx
00000783  8A87FC06          mov al,[bx+0x6fc]
00000787  B400              mov ah,0x0
00000789  50                push ax
0000078A  A0AA37            mov al,[0x37aa]
0000078D  98                cbw
0000078E  BA0600            mov dx,0x6
00000791  F7EA              imul dx
00000793  8BD8              mov bx,ax
00000795  8A87D343          mov al,[bx+0x43d3]
00000799  B400              mov ah,0x0
0000079B  BB0A00            mov bx,0xa
0000079E  99                cwd
0000079F  F7FB              idiv bx
000007A1  8BD8              mov bx,ax
000007A3  8A870007          mov al,[bx+0x700]
000007A7  B400              mov ah,0x0
000007A9  8BD0              mov dx,ax
000007AB  58                pop ax
000007AC  F7EA              imul dx
000007AE  50                push ax
000007AF  A0AA37            mov al,[0x37aa]
000007B2  98                cbw
000007B3  D1E0              shl ax,1
000007B5  8BD8              mov bx,ax
000007B7  58                pop ax
000007B8  89870707          mov [bx+0x707],ax
000007BC  807E06FF          cmp byte [bp+0x6],0xff
000007C0  751C              jnz 0x7de
000007C2  A0AA37            mov al,[0x37aa]
000007C5  98                cbw
000007C6  D1E0              shl ax,1
000007C8  8BD8              mov bx,ax
000007CA  8B870707          mov ax,[bx+0x707]
000007CE  D1E0              shl ax,1
000007D0  50                push ax
000007D1  A0AA37            mov al,[0x37aa]
000007D4  98                cbw
000007D5  D1E0              shl ax,1
000007D7  8BD8              mov bx,ax
000007D9  58                pop ax
000007DA  89870707          mov [bx+0x707],ax
000007DE  A0AA37            mov al,[0x37aa]
000007E1  98                cbw
000007E2  D1E0              shl ax,1
000007E4  8BD8              mov bx,ax
000007E6  8B875C37          mov ax,[bx+0x375c]
000007EA  50                push ax
000007EB  8A84DC06          mov al,[si+0x6dc]
000007EF  98                cbw
000007F0  50                push ax
000007F1  8A4606            mov al,[bp+0x6]
000007F4  98                cbw
000007F5  8BD0              mov dx,ax
000007F7  58                pop ax
000007F8  F7EA              imul dx
000007FA  5A                pop dx
000007FB  03D0              add dx,ax
000007FD  8956FE            mov [bp-0x2],dx
00000800  A0AA37            mov al,[0x37aa]
00000803  98                cbw
00000804  D1E0              shl ax,1
00000806  8BD8              mov bx,ax
00000808  8B876437          mov ax,[bx+0x3764]
0000080C  50                push ax
0000080D  8A84EC06          mov al,[si+0x6ec]
00000811  98                cbw
00000812  50                push ax
00000813  8A4606            mov al,[bp+0x6]
00000816  98                cbw
00000817  8BD0              mov dx,ax
00000819  58                pop ax
0000081A  F7EA              imul dx
0000081C  5A                pop dx
0000081D  03D0              add dx,ax
0000081F  8BFA              mov di,dx
00000821  8B46FE            mov ax,[bp-0x2]
00000824  BB0A00            mov bx,0xa
00000827  99                cwd
00000828  F7FB              idiv bx
0000082A  05CEFF            add ax,0xffce
0000082D  BB0800            mov bx,0x8
00000830  99                cwd
00000831  F7FB              idiv bx
00000833  8946FC            mov [bp-0x4],ax
00000836  8BC7              mov ax,di
00000838  BB0A00            mov bx,0xa
0000083B  99                cwd
0000083C  F7FB              idiv bx
0000083E  050900            add ax,0x9
00000841  BB0800            mov bx,0x8
00000844  99                cwd
00000845  F7FB              idiv bx
00000847  8946FA            mov [bp-0x6],ax
0000084A  A0AA37            mov al,[0x37aa]
0000084D  98                cbw
0000084E  D1E0              shl ax,1
00000850  8BD8              mov bx,ax
00000852  8B87E24B          mov ax,[bx+0x4be2]
00000856  3B46FC            cmp ax,[bp-0x4]
00000859  7514              jnz 0x86f
0000085B  A0AA37            mov al,[0x37aa]
0000085E  98                cbw
0000085F  D1E0              shl ax,1
00000861  8BD8              mov bx,ax
00000863  8B87EA4B          mov ax,[bx+0x4bea]
00000867  3B46FA            cmp ax,[bp-0x6]
0000086A  7503              jnz 0x86f
0000086C  E98500            jmp 0x8f4
0000086F  FF76FA            push word [bp-0x6]
00000872  FF76FC            push word [bp-0x4]
00000875  9AC309DE0E        call 0xede:0x9c3
0000087A  59                pop cx
0000087B  59                pop cx
0000087C  8846F9            mov [bp-0x7],al
0000087F  8A46F9            mov al,[bp-0x7]
00000882  50                push ax
00000883  8A46FA            mov al,[bp-0x6]
00000886  50                push ax
00000887  8A46FC            mov al,[bp-0x4]
0000088A  50                push ax
0000088B  9A1F09DE0E        call 0xede:0x91f
00000890  83C406            add sp,byte +0x6
00000893  0AC0              or al,al
00000895  751C              jnz 0x8b3
00000897  A0AA37            mov al,[0x37aa]
0000089A  98                cbw
0000089B  8A56FC            mov dl,[bp-0x4]
0000089E  8BD8              mov bx,ax
000008A0  8897C643          mov [bx+0x43c6],dl
000008A4  A0AA37            mov al,[0x37aa]
000008A7  98                cbw
000008A8  8A56FA            mov dl,[bp-0x6]
000008AB  8BD8              mov bx,ax
000008AD  8897CA43          mov [bx+0x43ca],dl
000008B1  EB66              jmp short 0x919
000008B3  A0AA37            mov al,[0x37aa]
000008B6  98                cbw
000008B7  D1E0              shl ax,1
000008B9  8B56FC            mov dx,[bp-0x4]
000008BC  8BD8              mov bx,ax
000008BE  8997E24B          mov [bx+0x4be2],dx
000008C2  A0AA37            mov al,[0x37aa]
000008C5  98                cbw
000008C6  D1E0              shl ax,1
000008C8  8B56FA            mov dx,[bp-0x6]
000008CB  8BD8              mov bx,ax
000008CD  8997EA4B          mov [bx+0x4bea],dx
000008D1  A0AA37            mov al,[0x37aa]
000008D4  98                cbw
000008D5  8A56F9            mov dl,[bp-0x7]
000008D8  8BD8              mov bx,ax
000008DA  8897143D          mov [bx+0x3d14],dl
000008DE  A0AA37            mov al,[0x37aa]
000008E1  98                cbw
000008E2  8BD8              mov bx,ax
000008E4  C687C04500        mov byte [bx+0x45c0],0x0
000008E9  A0AA37            mov al,[0x37aa]
000008EC  98                cbw
000008ED  8BD8              mov bx,ax
000008EF  C687BA4BFF        mov byte [bx+0x4bba],0xff
000008F4  A0AA37            mov al,[0x37aa]
000008F7  98                cbw
000008F8  D1E0              shl ax,1
000008FA  8B56FE            mov dx,[bp-0x2]
000008FD  8BD8              mov bx,ax
000008FF  89975C37          mov [bx+0x375c],dx
00000903  A0AA37            mov al,[0x37aa]
00000906  98                cbw
00000907  D1E0              shl ax,1
00000909  8BD8              mov bx,ax
0000090B  89BF6437          mov [bx+0x3764],di
0000090F  9AC31FDE0E        call 0xede:0x1fc3
00000914  9A8422DE0E        call 0xede:0x2284
00000919  5F                pop di
0000091A  5E                pop si
0000091B  8BE5              mov sp,bp
0000091D  5D                pop bp
0000091E  CB                retf

; ---- 0EDE:091F ----
0000091F  55                push bp
00000920  8BEC              mov bp,sp
00000922  4C                dec sp
00000923  4C                dec sp
00000924  8A4606            mov al,[bp+0x6]
00000927  98                cbw
00000928  BA1A00            mov dx,0x1a
0000092B  F7EA              imul dx
0000092D  50                push ax
0000092E  8A4608            mov al,[bp+0x8]
00000931  98                cbw
00000932  5B                pop bx
00000933  03D8              add bx,ax
00000935  8A87F448          mov al,[bx+0x48f4]
00000939  8846FF            mov [bp-0x1],al
0000093C  8A46FF            mov al,[bp-0x1]
0000093F  B400              mov ah,0x0
00000941  BB0800            mov bx,0x8
00000944  99                cwd
00000945  F7FB              idiv bx
00000947  3D0200            cmp ax,0x2
0000094A  7D04              jnl 0x950
0000094C  B001              mov al,0x1
0000094E  EB6F              jmp short 0x9bf
00000950  8A460A            mov al,[bp+0xa]
00000953  3A06AA37          cmp al,[0x37aa]
00000957  750C              jnz 0x965
00000959  F646FF02          test byte [bp-0x1],0x2
0000095D  7406              jz 0x965
0000095F  B001              mov al,0x1
00000961  EB5C              jmp short 0x9bf
00000963  EB56              jmp short 0x9bb
00000965  8A46FF            mov al,[bp-0x1]
00000968  B400              mov ah,0x0
0000096A  BB0800            mov bx,0x8
0000096D  99                cwd
0000096E  F7FB              idiv bx
00000970  3D0300            cmp ax,0x3
00000973  7546              jnz 0x9bb
00000975  A0AA37            mov al,[0x37aa]
00000978  98                cbw
00000979  8BD8              mov bx,ax
0000097B  8A87A637          mov al,[bx+0x37a6]
0000097F  040A              add al,0xa
00000981  50                push ax
00000982  A0AA37            mov al,[0x37aa]
00000985  98                cbw
00000986  8BD8              mov bx,ax
00000988  58                pop ax
00000989  8887A637          mov [bx+0x37a6],al
0000098D  A0AA37            mov al,[0x37aa]
00000990  98                cbw
00000991  8BD8              mov bx,ax
00000993  C687823704        mov byte [bx+0x3782],0x4
00000998  8A4608            mov al,[bp+0x8]
0000099B  98                cbw
0000099C  50                push ax
0000099D  8A4606            mov al,[bp+0x6]
000009A0  98                cbw
000009A1  50                push ax
000009A2  9AD3087E1E        call 0x1e7e:0x8d3
000009A7  59                pop cx
000009A8  59                pop cx
000009A9  B83B00            mov ax,0x3b
000009AC  50                push ax
000009AD  33C0              xor ax,ax
000009AF  50                push ax
000009B0  9A5400A721        call 0x21a7:0x54
000009B5  59                pop cx
000009B6  59                pop cx
000009B7  B001              mov al,0x1
000009B9  EB04              jmp short 0x9bf
000009BB  B000              mov al,0x0
000009BD  EB00              jmp short 0x9bf
000009BF  8BE5              mov sp,bp
000009C1  5D                pop bp
000009C2  CB                retf

; ---- 0EDE:09C3 ----
000009C3  55                push bp
000009C4  8BEC              mov bp,sp
000009C6  837E060D          cmp word [bp+0x6],byte +0xd
000009CA  7C0E              jl 0x9da
000009CC  837E080D          cmp word [bp+0x8],byte +0xd
000009D0  7C04              jl 0x9d6
000009D2  B002              mov al,0x2
000009D4  EB02              jmp short 0x9d8
000009D6  B001              mov al,0x1
000009D8  EB0C              jmp short 0x9e6
000009DA  837E080D          cmp word [bp+0x8],byte +0xd
000009DE  7C04              jl 0x9e4
000009E0  B003              mov al,0x3
000009E2  EB02              jmp short 0x9e6
000009E4  B000              mov al,0x0
000009E6  EB00              jmp short 0x9e8
000009E8  5D                pop bp
000009E9  CB                retf

; ---- 0EDE:09EA ----
000009EA  55                push bp
000009EB  8BEC              mov bp,sp
000009ED  A0AA37            mov al,[0x37aa]
000009F0  98                cbw
000009F1  8BD8              mov bx,ax
000009F3  80BF7C3700        cmp byte [bx+0x377c],0x0
000009F8  7502              jnz 0x9fc
000009FA  EB39              jmp short 0xa35
000009FC  9AFD097E1E        call 0x1e7e:0x9fd
00000A01  3C01              cmp al,0x1
00000A03  7530              jnz 0xa35
00000A05  A0AA37            mov al,[0x37aa]
00000A08  98                cbw
00000A09  8BD8              mov bx,ax
00000A0B  8A877C37          mov al,[bx+0x377c]
00000A0F  04FF              add al,0xff
00000A11  50                push ax
00000A12  A0AA37            mov al,[0x37aa]
00000A15  98                cbw
00000A16  8BD8              mov bx,ax
00000A18  58                pop ax
00000A19  88877C37          mov [bx+0x377c],al
00000A1D  0AC0              or al,al
00000A1F  7505              jnz 0xa26
00000A21  9A3F1FDE0E        call 0xede:0x1f3f
00000A26  B83C00            mov ax,0x3c
00000A29  50                push ax
00000A2A  B80200            mov ax,0x2
00000A2D  50                push ax
00000A2E  9A5400A721        call 0x21a7:0x54
00000A33  59                pop cx
00000A34  59                pop cx
00000A35  5D                pop bp
00000A36  CB                retf

; ---- 0EDE:0A37 ----
00000A37  55                push bp
00000A38  8BEC              mov bp,sp
00000A3A  56                push si
00000A3B  A0AA37            mov al,[0x37aa]
00000A3E  98                cbw
00000A3F  D1E0              shl ax,1
00000A41  8BD8              mov bx,ax
00000A43  83BFA82E00        cmp word [bx+0x2ea8],byte +0x0
00000A48  7403              jz 0xa4d
00000A4A  E99F01            jmp 0xbec
00000A4D  A0AA37            mov al,[0x37aa]
00000A50  98                cbw
00000A51  D1E0              shl ax,1
00000A53  8BD8              mov bx,ax
00000A55  83BFE83C00        cmp word [bx+0x3ce8],byte +0x0
00000A5A  7E03              jng 0xa5f
00000A5C  E98D01            jmp 0xbec
00000A5F  A0AA37            mov al,[0x37aa]
00000A62  98                cbw
00000A63  D1E0              shl ax,1
00000A65  D1E0              shl ax,1
00000A67  50                push ax
00000A68  8A4606            mov al,[bp+0x6]
00000A6B  98                cbw
00000A6C  D1E0              shl ax,1
00000A6E  5B                pop bx
00000A6F  03D8              add bx,ax
00000A71  83BF6C3700        cmp word [bx+0x376c],byte +0x0
00000A76  7503              jnz 0xa7b
00000A78  E97101            jmp 0xbec
00000A7B  A0AA37            mov al,[0x37aa]
00000A7E  98                cbw
00000A7F  D1E0              shl ax,1
00000A81  8BD8              mov bx,ax
00000A83  8BB7E643          mov si,[bx+0x43e6]
00000A87  807E0600          cmp byte [bp+0x6],0x0
00000A8B  7566              jnz 0xaf3
00000A8D  A0AA37            mov al,[0x37aa]
00000A90  98                cbw
00000A91  D1E0              shl ax,1
00000A93  8BD8              mov bx,ax
00000A95  89B77834          mov [bx+0x3478],si
00000A99  8BC6              mov ax,si
00000A9B  BB0400            mov bx,0x4
00000A9E  99                cwd
00000A9F  F7FB              idiv bx
00000AA1  8BDA              mov bx,dx
00000AA3  8A87BA06          mov al,[bx+0x6ba]
00000AA7  B400              mov ah,0x0
00000AA9  50                push ax
00000AAA  A0AA37            mov al,[0x37aa]
00000AAD  98                cbw
00000AAE  BA0600            mov dx,0x6
00000AB1  F7EA              imul dx
00000AB3  8BD8              mov bx,ax
00000AB5  8A87D143          mov al,[bx+0x43d1]
00000AB9  B400              mov ah,0x0
00000ABB  BB0A00            mov bx,0xa
00000ABE  99                cwd
00000ABF  F7FB              idiv bx
00000AC1  40                inc ax
00000AC2  8BD0              mov dx,ax
00000AC4  58                pop ax
00000AC5  F7EA              imul dx
00000AC7  50                push ax
00000AC8  A0AA37            mov al,[0x37aa]
00000ACB  98                cbw
00000ACC  D1E0              shl ax,1
00000ACE  8BD8              mov bx,ax
00000AD0  58                pop ax
00000AD1  8987282F          mov [bx+0x2f28],ax
00000AD5  A0AA37            mov al,[0x37aa]
00000AD8  98                cbw
00000AD9  D1E0              shl ax,1
00000ADB  8BD8              mov bx,ax
00000ADD  C787E83C5000      mov word [bx+0x3ce8],0x50
00000AE3  B83D00            mov ax,0x3d
00000AE6  50                push ax
00000AE7  33C0              xor ax,ax
00000AE9  50                push ax
00000AEA  9A5400A721        call 0x21a7:0x54
00000AEF  59                pop cx
00000AF0  59                pop cx
00000AF1  EB6C              jmp short 0xb5f
00000AF3  B8E803            mov ax,0x3e8
00000AF6  50                push ax
00000AF7  A0AA37            mov al,[0x37aa]
00000AFA  98                cbw
00000AFB  D1E0              shl ax,1
00000AFD  8BD8              mov bx,ax
00000AFF  FFB7EA4B          push word [bx+0x4bea]
00000B03  A0AA37            mov al,[0x37aa]
00000B06  98                cbw
00000B07  D1E0              shl ax,1
00000B09  8BD8              mov bx,ax
00000B0B  FFB7E24B          push word [bx+0x4be2]
00000B0F  9AAC1ADE0E        call 0xede:0x1aac
00000B14  83C406            add sp,byte +0x6
00000B17  50                push ax
00000B18  A0AA37            mov al,[0x37aa]
00000B1B  98                cbw
00000B1C  8BD8              mov bx,ax
00000B1E  58                pop ax
00000B1F  8887222C          mov [bx+0x2c22],al
00000B23  3CFF              cmp al,0xff
00000B25  7503              jnz 0xb2a
00000B27  E9C200            jmp 0xbec
00000B2A  A0AA37            mov al,[0x37aa]
00000B2D  98                cbw
00000B2E  D1E0              shl ax,1
00000B30  50                push ax
00000B31  8A840F07          mov al,[si+0x70f]
00000B35  98                cbw
00000B36  5B                pop bx
00000B37  89872E2C          mov [bx+0x2c2e],ax
00000B3B  A0AA37            mov al,[0x37aa]
00000B3E  98                cbw
00000B3F  D1E0              shl ax,1
00000B41  50                push ax
00000B42  8A841F07          mov al,[si+0x71f]
00000B46  98                cbw
00000B47  5B                pop bx
00000B48  8987362C          mov [bx+0x2c36],ax
00000B4C  FE063E2C          inc byte [0x2c3e]
00000B50  B83E00            mov ax,0x3e
00000B53  50                push ax
00000B54  B80200            mov ax,0x2
00000B57  50                push ax
00000B58  9A5400A721        call 0x21a7:0x54
00000B5D  59                pop cx
00000B5E  59                pop cx
00000B5F  A0AA37            mov al,[0x37aa]
00000B62  98                cbw
00000B63  8A5606            mov dl,[bp+0x6]
00000B66  8BD8              mov bx,ax
00000B68  88974837          mov [bx+0x3748],dl
00000B6C  A0AA37            mov al,[0x37aa]
00000B6F  98                cbw
00000B70  D1E0              shl ax,1
00000B72  8BD8              mov bx,ax
00000B74  8B875C37          mov ax,[bx+0x375c]
00000B78  50                push ax
00000B79  A0AA37            mov al,[0x37aa]
00000B7C  98                cbw
00000B7D  D1E0              shl ax,1
00000B7F  8BD8              mov bx,ax
00000B81  58                pop ax
00000B82  8987E244          mov [bx+0x44e2],ax
00000B86  A0AA37            mov al,[0x37aa]
00000B89  98                cbw
00000B8A  D1E0              shl ax,1
00000B8C  8BD8              mov bx,ax
00000B8E  8B876437          mov ax,[bx+0x3764]
00000B92  50                push ax
00000B93  A0AA37            mov al,[0x37aa]
00000B96  98                cbw
00000B97  D1E0              shl ax,1
00000B99  8BD8              mov bx,ax
00000B9B  58                pop ax
00000B9C  8987EA44          mov [bx+0x44ea],ax
00000BA0  9A4623DE0E        call 0xede:0x2346
00000BA5  A0AA37            mov al,[0x37aa]
00000BA8  98                cbw
00000BA9  D1E0              shl ax,1
00000BAB  8BD8              mov bx,ax
00000BAD  C78724340000      mov word [bx+0x3424],0x0
00000BB3  A0AA37            mov al,[0x37aa]
00000BB6  98                cbw
00000BB7  D1E0              shl ax,1
00000BB9  8BD8              mov bx,ax
00000BBB  C787A82E0100      mov word [bx+0x2ea8],0x1
00000BC1  A0AA37            mov al,[0x37aa]
00000BC4  98                cbw
00000BC5  8BD8              mov bx,ax
00000BC7  C6873F2C01        mov byte [bx+0x2c3f],0x1
00000BCC  A0AA37            mov al,[0x37aa]
00000BCF  98                cbw
00000BD0  D1E0              shl ax,1
00000BD2  D1E0              shl ax,1
00000BD4  50                push ax
00000BD5  8A4606            mov al,[bp+0x6]
00000BD8  98                cbw
00000BD9  D1E0              shl ax,1
00000BDB  5B                pop bx
00000BDC  03D8              add bx,ax
00000BDE  FF8F6C37          dec word [bx+0x376c]
00000BE2  8A4606            mov al,[bp+0x6]
00000BE5  50                push ax
00000BE6  9AA81EDE0E        call 0xede:0x1ea8
00000BEB  59                pop cx
00000BEC  5E                pop si
00000BED  5D                pop bp
00000BEE  CB                retf

; ---- 0EDE:0BEF ----
00000BEF  55                push bp
00000BF0  8BEC              mov bp,sp
00000BF2  A0AA37            mov al,[0x37aa]
00000BF5  98                cbw
00000BF6  8BD8              mov bx,ax
00000BF8  80BF483700        cmp byte [bx+0x3748],0x0
00000BFD  7507              jnz 0xc06
00000BFF  9A0D0CDE0E        call 0xede:0xc0d
00000C04  EB05              jmp short 0xc0b
00000C06  9AC70DDE0E        call 0xede:0xdc7
00000C0B  5D                pop bp
00000C0C  CB                retf

; ---- 0EDE:0C0D ----
00000C0D  55                push bp
00000C0E  8BEC              mov bp,sp
00000C10  83EC04            sub sp,byte +0x4
00000C13  A0AA37            mov al,[0x37aa]
00000C16  98                cbw
00000C17  D1E0              shl ax,1
00000C19  8BD8              mov bx,ax
00000C1B  8A877834          mov al,[bx+0x3478]
00000C1F  8846FF            mov [bp-0x1],al
00000C22  A0AA37            mov al,[0x37aa]
00000C25  98                cbw
00000C26  D1E0              shl ax,1
00000C28  50                push ax
00000C29  8A46FF            mov al,[bp-0x1]
00000C2C  98                cbw
00000C2D  8BD8              mov bx,ax
00000C2F  8A87DC06          mov al,[bx+0x6dc]
00000C33  98                cbw
00000C34  5B                pop bx
00000C35  0187E244          add [bx+0x44e2],ax
00000C39  A0AA37            mov al,[0x37aa]
00000C3C  98                cbw
00000C3D  D1E0              shl ax,1
00000C3F  50                push ax
00000C40  8A46FF            mov al,[bp-0x1]
00000C43  98                cbw
00000C44  8BD8              mov bx,ax
00000C46  8A87EC06          mov al,[bx+0x6ec]
00000C4A  98                cbw
00000C4B  5B                pop bx
00000C4C  0187EA44          add [bx+0x44ea],ax
00000C50  A0AA37            mov al,[0x37aa]
00000C53  98                cbw
00000C54  D1E0              shl ax,1
00000C56  8BD8              mov bx,ax
00000C58  8B87E244          mov ax,[bx+0x44e2]
00000C5C  BB0A00            mov bx,0xa
00000C5F  99                cwd
00000C60  F7FB              idiv bx
00000C62  05CEFF            add ax,0xffce
00000C65  BB0800            mov bx,0x8
00000C68  99                cwd
00000C69  F7FB              idiv bx
00000C6B  8846FE            mov [bp-0x2],al
00000C6E  A0AA37            mov al,[0x37aa]
00000C71  98                cbw
00000C72  D1E0              shl ax,1
00000C74  8BD8              mov bx,ax
00000C76  8B87EA44          mov ax,[bx+0x44ea]
00000C7A  BB0A00            mov bx,0xa
00000C7D  99                cwd
00000C7E  F7FB              idiv bx
00000C80  050900            add ax,0x9
00000C83  BB0800            mov bx,0x8
00000C86  99                cwd
00000C87  F7FB              idiv bx
00000C89  8846FD            mov [bp-0x3],al
00000C8C  9A5F23DE0E        call 0xede:0x235f
00000C91  A0AA37            mov al,[0x37aa]
00000C94  98                cbw
00000C95  D1E0              shl ax,1
00000C97  8BD8              mov bx,ax
00000C99  FF8F282F          dec word [bx+0x2f28]
00000C9D  A0AA37            mov al,[0x37aa]
00000CA0  98                cbw
00000CA1  D1E0              shl ax,1
00000CA3  8BD8              mov bx,ax
00000CA5  FF872434          inc word [bx+0x3424]
00000CA9  9A3910DE0E        call 0xede:0x1039
00000CAE  A0AA37            mov al,[0x37aa]
00000CB1  98                cbw
00000CB2  8BD8              mov bx,ax
00000CB4  8A872F07          mov al,[bx+0x72f]
00000CB8  3A46FE            cmp al,[bp-0x2]
00000CBB  7512              jnz 0xccf
00000CBD  A0AA37            mov al,[0x37aa]
00000CC0  98                cbw
00000CC1  8BD8              mov bx,ax
00000CC3  8A87122C          mov al,[bx+0x2c12]
00000CC7  3A46FD            cmp al,[bp-0x3]
00000CCA  7503              jnz 0xccf
00000CCC  E98400            jmp 0xd53
00000CCF  8A46FD            mov al,[bp-0x3]
00000CD2  98                cbw
00000CD3  50                push ax
00000CD4  8A46FE            mov al,[bp-0x2]
00000CD7  98                cbw
00000CD8  50                push ax
00000CD9  9A0710DE0E        call 0xede:0x1007
00000CDE  59                pop cx
00000CDF  59                pop cx
00000CE0  0AC0              or al,al
00000CE2  7506              jnz 0xcea
00000CE4  C646FC01          mov byte [bp-0x4],0x1
00000CE8  EB67              jmp short 0xd51
00000CEA  8A46FD            mov al,[bp-0x3]
00000CED  98                cbw
00000CEE  50                push ax
00000CEF  8A46FE            mov al,[bp-0x2]
00000CF2  98                cbw
00000CF3  50                push ax
00000CF4  9ADA11DE0E        call 0xede:0x11da
00000CF9  59                pop cx
00000CFA  59                pop cx
00000CFB  3C01              cmp al,0x1
00000CFD  7506              jnz 0xd05
00000CFF  C646FC01          mov byte [bp-0x4],0x1
00000D03  EB4C              jmp short 0xd51
00000D05  8A46FD            mov al,[bp-0x3]
00000D08  98                cbw
00000D09  50                push ax
00000D0A  8A46FE            mov al,[bp-0x2]
00000D0D  98                cbw
00000D0E  50                push ax
00000D0F  9AE812DE0E        call 0xede:0x12e8
00000D14  59                pop cx
00000D15  59                pop cx
00000D16  3C01              cmp al,0x1
00000D18  7506              jnz 0xd20
00000D1A  C646FC01          mov byte [bp-0x4],0x1
00000D1E  EB31              jmp short 0xd51
00000D20  A0AA37            mov al,[0x37aa]
00000D23  98                cbw
00000D24  8BD8              mov bx,ax
00000D26  80BF3F2C00        cmp byte [bx+0x2c3f],0x0
00000D2B  7506              jnz 0xd33
00000D2D  C646FC01          mov byte [bp-0x4],0x1
00000D31  EB1E              jmp short 0xd51
00000D33  A0AA37            mov al,[0x37aa]
00000D36  98                cbw
00000D37  8A56FE            mov dl,[bp-0x2]
00000D3A  8BD8              mov bx,ax
00000D3C  88972F07          mov [bx+0x72f],dl
00000D40  A0AA37            mov al,[0x37aa]
00000D43  98                cbw
00000D44  8A56FD            mov dl,[bp-0x3]
00000D47  8BD8              mov bx,ax
00000D49  8897122C          mov [bx+0x2c12],dl
00000D4D  C646FC00          mov byte [bp-0x4],0x0
00000D51  EB1F              jmp short 0xd72
00000D53  8A46FD            mov al,[bp-0x3]
00000D56  98                cbw
00000D57  50                push ax
00000D58  8A46FE            mov al,[bp-0x2]
00000D5B  98                cbw
00000D5C  50                push ax
00000D5D  9AE812DE0E        call 0xede:0x12e8
00000D62  59                pop cx
00000D63  59                pop cx
00000D64  3C01              cmp al,0x1
00000D66  7506              jnz 0xd6e
00000D68  C646FC01          mov byte [bp-0x4],0x1
00000D6C  EB04              jmp short 0xd72
00000D6E  C646FC00          mov byte [bp-0x4],0x0
00000D72  807EFC01          cmp byte [bp-0x4],0x1
00000D76  754B              jnz 0xdc3
00000D78  A0AA37            mov al,[0x37aa]
00000D7B  98                cbw
00000D7C  D1E0              shl ax,1
00000D7E  8BD8              mov bx,ax
00000D80  C787282F0000      mov word [bx+0x2f28],0x0
00000D86  A0AA37            mov al,[0x37aa]
00000D89  98                cbw
00000D8A  D1E0              shl ax,1
00000D8C  8BD8              mov bx,ax
00000D8E  C787C4450000      mov word [bx+0x45c4],0x0
00000D94  A0AA37            mov al,[0x37aa]
00000D97  98                cbw
00000D98  D1E0              shl ax,1
00000D9A  8BD8              mov bx,ax
00000D9C  C787A82E0200      mov word [bx+0x2ea8],0x2
00000DA2  A0AA37            mov al,[0x37aa]
00000DA5  98                cbw
00000DA6  8BD8              mov bx,ax
00000DA8  C6872F07FF        mov byte [bx+0x72f],0xff
00000DAD  A0AA37            mov al,[0x37aa]
00000DB0  98                cbw
00000DB1  D1E0              shl ax,1
00000DB3  D1E0              shl ax,1
00000DB5  8BD8              mov bx,ax
00000DB7  83BF6C3700        cmp word [bx+0x376c],byte +0x0
00000DBC  7505              jnz 0xdc3
00000DBE  9A5118DE0E        call 0xede:0x1851
00000DC3  8BE5              mov sp,bp
00000DC5  5D                pop bp
00000DC6  CB                retf

; ---- 0EDE:0DC7 ----
00000DC7  55                push bp
00000DC8  8BEC              mov bp,sp
00000DCA  4C                dec sp
00000DCB  4C                dec sp
00000DCC  56                push si
00000DCD  57                push di
00000DCE  A0AA37            mov al,[0x37aa]
00000DD1  98                cbw
00000DD2  8BD8              mov bx,ax
00000DD4  8A873307          mov al,[bx+0x733]
00000DD8  04FF              add al,0xff
00000DDA  50                push ax
00000DDB  A0AA37            mov al,[0x37aa]
00000DDE  98                cbw
00000DDF  8BD8              mov bx,ax
00000DE1  58                pop ax
00000DE2  88873307          mov [bx+0x733],al
00000DE6  0AC0              or al,al
00000DE8  7403              jz 0xded
00000DEA  E9AE01            jmp 0xf9b
00000DED  A0AA37            mov al,[0x37aa]
00000DF0  98                cbw
00000DF1  8BD8              mov bx,ax
00000DF3  8A87222C          mov al,[bx+0x2c22]
00000DF7  8846FF            mov [bp-0x1],al
00000DFA  A0AA37            mov al,[0x37aa]
00000DFD  98                cbw
00000DFE  D1E0              shl ax,1
00000E00  8BD8              mov bx,ax
00000E02  8BBF2E2C          mov di,[bx+0x2c2e]
00000E06  A0AA37            mov al,[0x37aa]
00000E09  98                cbw
00000E0A  D1E0              shl ax,1
00000E0C  8BD8              mov bx,ax
00000E0E  8BB7362C          mov si,[bx+0x2c36]
00000E12  8A46FF            mov al,[bp-0x1]
00000E15  98                cbw
00000E16  D1E0              shl ax,1
00000E18  8BD8              mov bx,ax
00000E1A  8B875C37          mov ax,[bx+0x375c]
00000E1E  50                push ax
00000E1F  A0AA37            mov al,[0x37aa]
00000E22  98                cbw
00000E23  D1E0              shl ax,1
00000E25  8BD8              mov bx,ax
00000E27  58                pop ax
00000E28  3B87E244          cmp ax,[bx+0x44e2]
00000E2C  7E08              jng 0xe36
00000E2E  83FF0A            cmp di,byte +0xa
00000E31  7D01              jnl 0xe34
00000E33  47                inc di
00000E34  EB06              jmp short 0xe3c
00000E36  83FFF6            cmp di,byte -0xa
00000E39  7E01              jng 0xe3c
00000E3B  4F                dec di
00000E3C  8A46FF            mov al,[bp-0x1]
00000E3F  98                cbw
00000E40  D1E0              shl ax,1
00000E42  8BD8              mov bx,ax
00000E44  8B876437          mov ax,[bx+0x3764]
00000E48  50                push ax
00000E49  A0AA37            mov al,[0x37aa]
00000E4C  98                cbw
00000E4D  D1E0              shl ax,1
00000E4F  8BD8              mov bx,ax
00000E51  58                pop ax
00000E52  3B87EA44          cmp ax,[bx+0x44ea]
00000E56  7E08              jng 0xe60
00000E58  83FE0A            cmp si,byte +0xa
00000E5B  7D01              jnl 0xe5e
00000E5D  46                inc si
00000E5E  EB06              jmp short 0xe66
00000E60  83FEF6            cmp si,byte -0xa
00000E63  7E01              jng 0xe66
00000E65  4E                dec si
00000E66  A0AA37            mov al,[0x37aa]
00000E69  98                cbw
00000E6A  D1E0              shl ax,1
00000E6C  8BD8              mov bx,ax
00000E6E  89BF2E2C          mov [bx+0x2c2e],di
00000E72  A0AA37            mov al,[0x37aa]
00000E75  98                cbw
00000E76  D1E0              shl ax,1
00000E78  8BD8              mov bx,ax
00000E7A  89B7362C          mov [bx+0x2c36],si
00000E7E  A0AA37            mov al,[0x37aa]
00000E81  98                cbw
00000E82  D1E0              shl ax,1
00000E84  8BD8              mov bx,ax
00000E86  8B87E244          mov ax,[bx+0x44e2]
00000E8A  50                push ax
00000E8B  8BC7              mov ax,di
00000E8D  BA0A00            mov dx,0xa
00000E90  F7EA              imul dx
00000E92  BB0A00            mov bx,0xa
00000E95  99                cwd
00000E96  F7FB              idiv bx
00000E98  5A                pop dx
00000E99  03D0              add dx,ax
00000E9B  A0AA37            mov al,[0x37aa]
00000E9E  98                cbw
00000E9F  D1E0              shl ax,1
00000EA1  8BD8              mov bx,ax
00000EA3  8997E244          mov [bx+0x44e2],dx
00000EA7  A0AA37            mov al,[0x37aa]
00000EAA  98                cbw
00000EAB  D1E0              shl ax,1
00000EAD  8BD8              mov bx,ax
00000EAF  8B87EA44          mov ax,[bx+0x44ea]
00000EB3  50                push ax
00000EB4  8BC6              mov ax,si
00000EB6  BA0A00            mov dx,0xa
00000EB9  F7EA              imul dx
00000EBB  BB0A00            mov bx,0xa
00000EBE  99                cwd
00000EBF  F7FB              idiv bx
00000EC1  5A                pop dx
00000EC2  03D0              add dx,ax
00000EC4  A0AA37            mov al,[0x37aa]
00000EC7  98                cbw
00000EC8  D1E0              shl ax,1
00000ECA  8BD8              mov bx,ax
00000ECC  8997EA44          mov [bx+0x44ea],dx
00000ED0  9A5F23DE0E        call 0xede:0x235f
00000ED5  A0AA37            mov al,[0x37aa]
00000ED8  98                cbw
00000ED9  8BD8              mov bx,ax
00000EDB  8A873707          mov al,[bx+0x737]
00000EDF  FEC8              dec al
00000EE1  50                push ax
00000EE2  A0AA37            mov al,[0x37aa]
00000EE5  98                cbw
00000EE6  8BD8              mov bx,ax
00000EE8  58                pop ax
00000EE9  88873707          mov [bx+0x737],al
00000EED  0AC0              or al,al
00000EEF  7540              jnz 0xf31
00000EF1  A0AA37            mov al,[0x37aa]
00000EF4  98                cbw
00000EF5  8BD8              mov bx,ax
00000EF7  8A873B07          mov al,[bx+0x73b]
00000EFB  3401              xor al,0x1
00000EFD  50                push ax
00000EFE  A0AA37            mov al,[0x37aa]
00000F01  98                cbw
00000F02  8BD8              mov bx,ax
00000F04  58                pop ax
00000F05  88873B07          mov [bx+0x73b],al
00000F09  A0AA37            mov al,[0x37aa]
00000F0C  98                cbw
00000F0D  BA1A00            mov dx,0x1a
00000F10  F7EA              imul dx
00000F12  50                push ax
00000F13  A0AA37            mov al,[0x37aa]
00000F16  98                cbw
00000F17  8BD8              mov bx,ax
00000F19  8A873B07          mov al,[bx+0x73b]
00000F1D  98                cbw
00000F1E  050400            add ax,0x4
00000F21  5B                pop bx
00000F22  89872433          mov [bx+0x3324],ax
00000F26  A0AA37            mov al,[0x37aa]
00000F29  98                cbw
00000F2A  8BD8              mov bx,ax
00000F2C  C68737070A        mov byte [bx+0x737],0xa
00000F31  9A3910DE0E        call 0xede:0x1039
00000F36  3C01              cmp al,0x1
00000F38  7420              jz 0xf5a
00000F3A  A0AA37            mov al,[0x37aa]
00000F3D  98                cbw
00000F3E  D1E0              shl ax,1
00000F40  8BD8              mov bx,ax
00000F42  8B872434          mov ax,[bx+0x3424]
00000F46  40                inc ax
00000F47  50                push ax
00000F48  A0AA37            mov al,[0x37aa]
00000F4B  98                cbw
00000F4C  D1E0              shl ax,1
00000F4E  8BD8              mov bx,ax
00000F50  58                pop ax
00000F51  89872434          mov [bx+0x3424],ax
00000F55  3D9001            cmp ax,0x190
00000F58  7536              jnz 0xf90
00000F5A  A03E2C            mov al,[0x2c3e]
00000F5D  FEC8              dec al
00000F5F  A23E2C            mov [0x2c3e],al
00000F62  0AC0              or al,al
00000F64  750E              jnz 0xf74
00000F66  33C0              xor ax,ax
00000F68  50                push ax
00000F69  B80200            mov ax,0x2
00000F6C  50                push ax
00000F6D  9A5400A721        call 0x21a7:0x54
00000F72  59                pop cx
00000F73  59                pop cx
00000F74  A0AA37            mov al,[0x37aa]
00000F77  98                cbw
00000F78  D1E0              shl ax,1
00000F7A  8BD8              mov bx,ax
00000F7C  C787C4450000      mov word [bx+0x45c4],0x0
00000F82  A0AA37            mov al,[0x37aa]
00000F85  98                cbw
00000F86  D1E0              shl ax,1
00000F88  8BD8              mov bx,ax
00000F8A  C787A82E0200      mov word [bx+0x2ea8],0x2
00000F90  A0AA37            mov al,[0x37aa]
00000F93  98                cbw
00000F94  8BD8              mov bx,ax
00000F96  C687330702        mov byte [bx+0x733],0x2
00000F9B  5F                pop di
00000F9C  5E                pop si
00000F9D  8BE5              mov sp,bp
00000F9F  5D                pop bp
00000FA0  CB                retf

; ---- 0EDE:0FA1 ----
00000FA1  55                push bp
00000FA2  8BEC              mov bp,sp
00000FA4  A11A3D            mov ax,[0x3d1a]
00000FA7  BB0600            mov bx,0x6
00000FAA  99                cwd
00000FAB  F7FB              idiv bx
00000FAD  0BD2              or dx,dx
00000FAF  7402              jz 0xfb3
00000FB1  EB52              jmp short 0x1005
00000FB3  A0AA37            mov al,[0x37aa]
00000FB6  98                cbw
00000FB7  D1E0              shl ax,1
00000FB9  8BD8              mov bx,ax
00000FBB  8B87C445          mov ax,[bx+0x45c4]
00000FBF  40                inc ax
00000FC0  50                push ax
00000FC1  A0AA37            mov al,[0x37aa]
00000FC4  98                cbw
00000FC5  D1E0              shl ax,1
00000FC7  8BD8              mov bx,ax
00000FC9  58                pop ax
00000FCA  8987C445          mov [bx+0x45c4],ax
00000FCE  3D0400            cmp ax,0x4
00000FD1  7D1F              jnl 0xff2
00000FD3  A0AA37            mov al,[0x37aa]
00000FD6  98                cbw
00000FD7  D1E0              shl ax,1
00000FD9  8BD8              mov bx,ax
00000FDB  8B87C445          mov ax,[bx+0x45c4]
00000FDF  50                push ax
00000FE0  A0AA37            mov al,[0x37aa]
00000FE3  98                cbw
00000FE4  BA1A00            mov dx,0x1a
00000FE7  F7EA              imul dx
00000FE9  8BD8              mov bx,ax
00000FEB  58                pop ax
00000FEC  89872433          mov [bx+0x3324],ax
00000FF0  EB13              jmp short 0x1005
00000FF2  9AAA23DE0E        call 0xede:0x23aa
00000FF7  A0AA37            mov al,[0x37aa]
00000FFA  98                cbw
00000FFB  D1E0              shl ax,1
00000FFD  8BD8              mov bx,ax
00000FFF  C787A82E0000      mov word [bx+0x2ea8],0x0
00001005  5D                pop bp
00001006  CB                retf

; ---- 0EDE:1007 ----
00001007  55                push bp
00001008  8BEC              mov bp,sp
0000100A  837E0600          cmp word [bp+0x6],byte +0x0
0000100E  7C12              jl 0x1022
00001010  837E0619          cmp word [bp+0x6],byte +0x19
00001014  7F0C              jg 0x1022
00001016  837E0801          cmp word [bp+0x8],byte +0x1
0000101A  7C06              jl 0x1022
0000101C  837E0818          cmp word [bp+0x8],byte +0x18
00001020  7E11              jng 0x1033
00001022  A0AA37            mov al,[0x37aa]
00001025  98                cbw
00001026  8BD8              mov bx,ax
00001028  C687BA4B00        mov byte [bx+0x4bba],0x0
0000102D  B000              mov al,0x0
0000102F  EB06              jmp short 0x1037
00001031  EB04              jmp short 0x1037
00001033  B001              mov al,0x1
00001035  EB00              jmp short 0x1037
00001037  5D                pop bp
00001038  CB                retf

; ---- 0EDE:1039 ----
00001039  55                push bp
0000103A  8BEC              mov bp,sp
0000103C  83EC06            sub sp,byte +0x6
0000103F  56                push si
00001040  57                push di
00001041  A0AA37            mov al,[0x37aa]
00001044  98                cbw
00001045  D1E0              shl ax,1
00001047  8BD8              mov bx,ax
00001049  8BBFE244          mov di,[bx+0x44e2]
0000104D  A0AA37            mov al,[0x37aa]
00001050  98                cbw
00001051  D1E0              shl ax,1
00001053  8BD8              mov bx,ax
00001055  8BB7EA44          mov si,[bx+0x44ea]
00001059  C746FE0000        mov word [bp-0x2],0x0
0000105E  E96601            jmp 0x11c7
00001061  A0AA37            mov al,[0x37aa]
00001064  98                cbw
00001065  3B46FE            cmp ax,[bp-0x2]
00001068  740A              jz 0x1074
0000106A  8B5EFE            mov bx,[bp-0x2]
0000106D  80BFB24310        cmp byte [bx+0x43b2],0x10
00001072  7403              jz 0x1077
00001074  E94D01            jmp 0x11c4
00001077  8B5EFE            mov bx,[bp-0x2]
0000107A  D1E3              shl bx,1
0000107C  8BC7              mov ax,di
0000107E  05D8FF            add ax,0xffd8
00001081  39875C37          cmp [bx+0x375c],ax
00001085  7D03              jnl 0x108a
00001087  E93A01            jmp 0x11c4
0000108A  8B5EFE            mov bx,[bp-0x2]
0000108D  D1E3              shl bx,1
0000108F  8BC7              mov ax,di
00001091  052800            add ax,0x28
00001094  39875C37          cmp [bx+0x375c],ax
00001098  7E03              jng 0x109d
0000109A  E92701            jmp 0x11c4
0000109D  8B5EFE            mov bx,[bp-0x2]
000010A0  D1E3              shl bx,1
000010A2  8BC6              mov ax,si
000010A4  05D8FF            add ax,0xffd8
000010A7  39876437          cmp [bx+0x3764],ax
000010AB  7D03              jnl 0x10b0
000010AD  E91401            jmp 0x11c4
000010B0  8B5EFE            mov bx,[bp-0x2]
000010B3  D1E3              shl bx,1
000010B5  8BC6              mov ax,si
000010B7  052800            add ax,0x28
000010BA  39876437          cmp [bx+0x3764],ax
000010BE  7E03              jng 0x10c3
000010C0  E90101            jmp 0x11c4
000010C3  A0AA37            mov al,[0x37aa]
000010C6  98                cbw
000010C7  8BD8              mov bx,ax
000010C9  80BF483700        cmp byte [bx+0x3748],0x0
000010CE  7403              jz 0x10d3
000010D0  E9BC00            jmp 0x118f
000010D3  A0AA37            mov al,[0x37aa]
000010D6  98                cbw
000010D7  8BD8              mov bx,ax
000010D9  80BF3F2C00        cmp byte [bx+0x2c3f],0x0
000010DE  7505              jnz 0x10e5
000010E0  B000              mov al,0x0
000010E2  E9EF00            jmp 0x11d4
000010E5  A0AA37            mov al,[0x37aa]
000010E8  98                cbw
000010E9  BA0600            mov dx,0x6
000010EC  F7EA              imul dx
000010EE  8BD8              mov bx,ax
000010F0  8A87D043          mov al,[bx+0x43d0]
000010F4  B400              mov ah,0x0
000010F6  BB0A00            mov bx,0xa
000010F9  99                cwd
000010FA  F7FB              idiv bx
000010FC  FEC0              inc al
000010FE  D0E0              shl al,1
00001100  8B5EFE            mov bx,[bp-0x2]
00001103  8A97A637          mov dl,[bx+0x37a6]
00001107  02D0              add dl,al
00001109  8B5EFE            mov bx,[bp-0x2]
0000110C  8897A637          mov [bx+0x37a6],dl
00001110  A0AA37            mov al,[0x37aa]
00001113  98                cbw
00001114  D1E0              shl ax,1
00001116  8BD8              mov bx,ax
00001118  8B877834          mov ax,[bx+0x3478]
0000111C  8946FC            mov [bp-0x4],ax
0000111F  8B46FC            mov ax,[bp-0x4]
00001122  BB0400            mov bx,0x4
00001125  99                cwd
00001126  F7FB              idiv bx
00001128  8BDA              mov bx,dx
0000112A  8A87BA06          mov al,[bx+0x6ba]
0000112E  B400              mov ah,0x0
00001130  50                push ax
00001131  8B46FE            mov ax,[bp-0x2]
00001134  BA0600            mov dx,0x6
00001137  F7EA              imul dx
00001139  8BD8              mov bx,ax
0000113B  8A87D143          mov al,[bx+0x43d1]
0000113F  B400              mov ah,0x0
00001141  BB0A00            mov bx,0xa
00001144  99                cwd
00001145  F7FB              idiv bx
00001147  40                inc ax
00001148  8BD0              mov dx,ax
0000114A  58                pop ax
0000114B  F7EA              imul dx
0000114D  8946FA            mov [bp-0x6],ax
00001150  A0AA37            mov al,[0x37aa]
00001153  98                cbw
00001154  D1E0              shl ax,1
00001156  8BD8              mov bx,ax
00001158  8B872434          mov ax,[bx+0x3424]
0000115C  3B46FA            cmp ax,[bp-0x6]
0000115F  7F05              jg 0x1166
00001161  B80100            mov ax,0x1
00001164  EB02              jmp short 0x1168
00001166  33C0              xor ax,ax
00001168  50                push ax
00001169  8A46FE            mov al,[bp-0x2]
0000116C  50                push ax
0000116D  9A74170F15        call 0x150f:0x1774
00001172  59                pop cx
00001173  59                pop cx
00001174  A0AA37            mov al,[0x37aa]
00001177  98                cbw
00001178  8BD8              mov bx,ax
0000117A  C6873F2C00        mov byte [bx+0x2c3f],0x0
0000117F  B83F00            mov ax,0x3f
00001182  50                push ax
00001183  33C0              xor ax,ax
00001185  50                push ax
00001186  9A5400A721        call 0x21a7:0x54
0000118B  59                pop cx
0000118C  59                pop cx
0000118D  EB1E              jmp short 0x11ad
0000118F  8B5EFE            mov bx,[bp-0x2]
00001192  8A87A637          mov al,[bx+0x37a6]
00001196  0428              add al,0x28
00001198  8B5EFE            mov bx,[bp-0x2]
0000119B  8887A637          mov [bx+0x37a6],al
0000119F  B84000            mov ax,0x40
000011A2  50                push ax
000011A3  33C0              xor ax,ax
000011A5  50                push ax
000011A6  9A5400A721        call 0x21a7:0x54
000011AB  59                pop cx
000011AC  59                pop cx
000011AD  8B5EFE            mov bx,[bp-0x2]
000011B0  C687823704        mov byte [bx+0x3782],0x4
000011B5  A0AA37            mov al,[0x37aa]
000011B8  98                cbw
000011B9  8BD8              mov bx,ax
000011BB  C687BA4B01        mov byte [bx+0x4bba],0x1
000011C0  B001              mov al,0x1
000011C2  EB10              jmp short 0x11d4
000011C4  FF46FE            inc word [bp-0x2]
000011C7  837EFE03          cmp word [bp-0x2],byte +0x3
000011CB  7F03              jg 0x11d0
000011CD  E991FE            jmp 0x1061
000011D0  B000              mov al,0x0
000011D2  EB00              jmp short 0x11d4
000011D4  5F                pop di
000011D5  5E                pop si
000011D6  8BE5              mov sp,bp
000011D8  5D                pop bp
000011D9  CB                retf

; ---- 0EDE:11DA ----
000011DA  55                push bp
000011DB  8BEC              mov bp,sp
000011DD  83EC04            sub sp,byte +0x4
000011E0  56                push si
000011E1  57                push di
000011E2  8B7E06            mov di,[bp+0x6]
000011E5  8B7608            mov si,[bp+0x8]
000011E8  8BC7              mov ax,di
000011EA  BA1A00            mov dx,0x1a
000011ED  F7EA              imul dx
000011EF  8BD8              mov bx,ax
000011F1  8A80F448          mov al,[bx+si+0x48f4]
000011F5  8846FE            mov [bp-0x2],al
000011F8  F646FE01          test byte [bp-0x2],0x1
000011FC  7505              jnz 0x1203
000011FE  B000              mov al,0x0
00001200  E9DF00            jmp 0x12e2
00001203  8A46FE            mov al,[bp-0x2]
00001206  B400              mov ah,0x0
00001208  BB0800            mov bx,0x8
0000120B  99                cwd
0000120C  F7FB              idiv bx
0000120E  8846FF            mov [bp-0x1],al
00001211  A0AA37            mov al,[0x37aa]
00001214  98                cbw
00001215  D1E0              shl ax,1
00001217  8BD8              mov bx,ax
00001219  83BF243410        cmp word [bx+0x3424],byte +0x10
0000121E  7F14              jg 0x1234
00001220  56                push si
00001221  57                push di
00001222  0E                push cs
00001223  E89DF7            call 0x9c3
00001226  59                pop cx
00001227  59                pop cx
00001228  3A06AA37          cmp al,[0x37aa]
0000122C  7524              jnz 0x1252
0000122E  807EFF02          cmp byte [bp-0x1],0x2
00001232  741E              jz 0x1252
00001234  A0AA37            mov al,[0x37aa]
00001237  98                cbw
00001238  8BD8              mov bx,ax
0000123A  C687BA4B00        mov byte [bx+0x4bba],0x0
0000123F  B84100            mov ax,0x41
00001242  50                push ax
00001243  33C0              xor ax,ax
00001245  50                push ax
00001246  9A5400A721        call 0x21a7:0x54
0000124B  59                pop cx
0000124C  59                pop cx
0000124D  B001              mov al,0x1
0000124F  E99000            jmp 0x12e2
00001252  807EFF05          cmp byte [bp-0x1],0x5
00001256  7518              jnz 0x1270
00001258  56                push si
00001259  57                push di
0000125A  9A6C13DE0E        call 0xede:0x136c
0000125F  59                pop cx
00001260  59                pop cx
00001261  A0AA37            mov al,[0x37aa]
00001264  98                cbw
00001265  8BD8              mov bx,ax
00001267  C687BA4B03        mov byte [bx+0x4bba],0x3
0000126C  B001              mov al,0x1
0000126E  EB72              jmp short 0x12e2
00001270  A0AA37            mov al,[0x37aa]
00001273  98                cbw
00001274  8BD8              mov bx,ax
00001276  C687BA4B02        mov byte [bx+0x4bba],0x2
0000127B  807EFF04          cmp byte [bp-0x1],0x4
0000127F  750B              jnz 0x128c
00001281  56                push si
00001282  57                push di
00001283  9A7B16DE0E        call 0xede:0x167b
00001288  59                pop cx
00001289  59                pop cx
0000128A  EB3B              jmp short 0x12c7
0000128C  807EFF07          cmp byte [bp-0x1],0x7
00001290  7235              jc 0x12c7
00001292  8A46FF            mov al,[bp-0x1]
00001295  04F9              add al,0xf9
00001297  8846FD            mov [bp-0x3],al
0000129A  8A46FD            mov al,[bp-0x3]
0000129D  FEC8              dec al
0000129F  8846FD            mov [bp-0x3],al
000012A2  0AC0              or al,al
000012A4  7C21              jl 0x12c7
000012A6  8A46FD            mov al,[bp-0x3]
000012A9  98                cbw
000012AA  50                push ax
000012AB  56                push si
000012AC  57                push di
000012AD  9A3F087E1E        call 0x1e7e:0x83f
000012B2  83C406            add sp,byte +0x6
000012B5  B84200            mov ax,0x42
000012B8  50                push ax
000012B9  33C0              xor ax,ax
000012BB  50                push ax
000012BC  9A5400A721        call 0x21a7:0x54
000012C1  59                pop cx
000012C2  59                pop cx
000012C3  B001              mov al,0x1
000012C5  EB1B              jmp short 0x12e2
000012C7  B84200            mov ax,0x42
000012CA  50                push ax
000012CB  33C0              xor ax,ax
000012CD  50                push ax
000012CE  9A5400A721        call 0x21a7:0x54
000012D3  59                pop cx
000012D4  59                pop cx
000012D5  56                push si
000012D6  57                push di
000012D7  9AD3087E1E        call 0x1e7e:0x8d3
000012DC  59                pop cx
000012DD  59                pop cx
000012DE  B001              mov al,0x1
000012E0  EB00              jmp short 0x12e2
000012E2  5F                pop di
000012E3  5E                pop si
000012E4  8BE5              mov sp,bp
000012E6  5D                pop bp
000012E7  CB                retf

; ---- 0EDE:12E8 ----
000012E8  55                push bp
000012E9  8BEC              mov bp,sp
000012EB  A0AA37            mov al,[0x37aa]
000012EE  98                cbw
000012EF  D1E0              shl ax,1
000012F1  8BD8              mov bx,ax
000012F3  83BF282F00        cmp word [bx+0x2f28],byte +0x0
000012F8  756C              jnz 0x1366
000012FA  8B4606            mov ax,[bp+0x6]
000012FD  BA1A00            mov dx,0x1a
00001300  F7EA              imul dx
00001302  034608            add ax,[bp+0x8]
00001305  8BD8              mov bx,ax
00001307  8A87F448          mov al,[bx+0x48f4]
0000130B  B400              mov ah,0x0
0000130D  BB0800            mov bx,0x8
00001310  99                cwd
00001311  F7FB              idiv bx
00001313  0BC0              or ax,ax
00001315  7530              jnz 0x1347
00001317  A0AA37            mov al,[0x37aa]
0000131A  98                cbw
0000131B  D1E0              shl ax,1
0000131D  8BD8              mov bx,ax
0000131F  8B87EA44          mov ax,[bx+0x44ea]
00001323  BB0A00            mov bx,0xa
00001326  99                cwd
00001327  F7FB              idiv bx
00001329  50                push ax
0000132A  A0AA37            mov al,[0x37aa]
0000132D  98                cbw
0000132E  D1E0              shl ax,1
00001330  8BD8              mov bx,ax
00001332  8B87E244          mov ax,[bx+0x44e2]
00001336  BB0A00            mov bx,0xa
00001339  99                cwd
0000133A  F7FB              idiv bx
0000133C  050400            add ax,0x4
0000133F  50                push ax
00001340  9A210E7E1E        call 0x1e7e:0xe21
00001345  59                pop cx
00001346  59                pop cx
00001347  A0AA37            mov al,[0x37aa]
0000134A  98                cbw
0000134B  8BD8              mov bx,ax
0000134D  C687BA4B00        mov byte [bx+0x4bba],0x0
00001352  B84300            mov ax,0x43
00001355  50                push ax
00001356  33C0              xor ax,ax
00001358  50                push ax
00001359  9A5400A721        call 0x21a7:0x54
0000135E  59                pop cx
0000135F  59                pop cx
00001360  B001              mov al,0x1
00001362  EB06              jmp short 0x136a
00001364  EB04              jmp short 0x136a
00001366  B000              mov al,0x0
00001368  EB00              jmp short 0x136a
0000136A  5D                pop bp
0000136B  CB                retf

; ---- 0EDE:136C ----
0000136C  55                push bp
0000136D  8BEC              mov bp,sp
0000136F  4C                dec sp
00001370  4C                dec sp
00001371  C746FE0000        mov word [bp-0x2],0x0
00001376  E99400            jmp 0x140d
00001379  A0AA37            mov al,[0x37aa]
0000137C  98                cbw
0000137D  3B46FE            cmp ax,[bp-0x2]
00001380  7503              jnz 0x1385
00001382  E98500            jmp 0x140a
00001385  8B5EFE            mov bx,[bp-0x2]
00001388  8A874C08          mov al,[bx+0x84c]
0000138C  B400              mov ah,0x0
0000138E  3B4606            cmp ax,[bp+0x6]
00001391  7411              jz 0x13a4
00001393  8B5EFE            mov bx,[bp-0x2]
00001396  8A874C08          mov al,[bx+0x84c]
0000139A  B400              mov ah,0x0
0000139C  40                inc ax
0000139D  3B4606            cmp ax,[bp+0x6]
000013A0  7402              jz 0x13a4
000013A2  EB66              jmp short 0x140a
000013A4  8B5EFE            mov bx,[bp-0x2]
000013A7  8A875008          mov al,[bx+0x850]
000013AB  B400              mov ah,0x0
000013AD  3B4608            cmp ax,[bp+0x8]
000013B0  7411              jz 0x13c3
000013B2  8B5EFE            mov bx,[bp-0x2]
000013B5  8A875008          mov al,[bx+0x850]
000013B9  B400              mov ah,0x0
000013BB  40                inc ax
000013BC  3B4608            cmp ax,[bp+0x8]
000013BF  7402              jz 0x13c3
000013C1  EB47              jmp short 0x140a
000013C3  B001              mov al,0x1
000013C5  50                push ax
000013C6  FF76FE            push word [bp-0x2]
000013C9  9AF1087E1E        call 0x1e7e:0x8f1
000013CE  59                pop cx
000013CF  59                pop cx
000013D0  A0C737            mov al,[0x37c7]
000013D3  0C02              or al,0x2
000013D5  A2C737            mov [0x37c7],al
000013D8  B84400            mov ax,0x44
000013DB  50                push ax
000013DC  33C0              xor ax,ax
000013DE  50                push ax
000013DF  9A5400A721        call 0x21a7:0x54
000013E4  59                pop cx
000013E5  59                pop cx
000013E6  B84400            mov ax,0x44
000013E9  50                push ax
000013EA  B80200            mov ax,0x2
000013ED  50                push ax
000013EE  9A5400A721        call 0x21a7:0x54
000013F3  59                pop cx
000013F4  59                pop cx
000013F5  8A46FE            mov al,[bp-0x2]
000013F8  50                push ax
000013F9  9A36170F15        call 0x150f:0x1736
000013FE  59                pop cx
000013FF  FF76FE            push word [bp-0x2]
00001402  9A1A14DE0E        call 0xede:0x141a
00001407  59                pop cx
00001408  EB0C              jmp short 0x1416
0000140A  FF46FE            inc word [bp-0x2]
0000140D  837EFE03          cmp word [bp-0x2],byte +0x3
00001411  7F03              jg 0x1416
00001413  E963FF            jmp 0x1379
00001416  8BE5              mov sp,bp
00001418  5D                pop bp
00001419  CB                retf

; ---- 0EDE:141A ----
0000141A  55                push bp
0000141B  8BEC              mov bp,sp
0000141D  83EC14            sub sp,byte +0x14
00001420  56                push si
00001421  57                push di
00001422  A0AA37            mov al,[0x37aa]
00001425  98                cbw
00001426  D1E0              shl ax,1
00001428  8BD8              mov bx,ax
0000142A  8B876808          mov ax,[bx+0x868]
0000142E  050800            add ax,0x8
00001431  8BF8              mov di,ax
00001433  A0AA37            mov al,[0x37aa]
00001436  98                cbw
00001437  D1E0              shl ax,1
00001439  8BD8              mov bx,ax
0000143B  8B877008          mov ax,[bx+0x870]
0000143F  050400            add ax,0x4
00001442  8BF0              mov si,ax
00001444  A0AA37            mov al,[0x37aa]
00001447  98                cbw
00001448  BA0600            mov dx,0x6
0000144B  F7EA              imul dx
0000144D  8BD8              mov bx,ax
0000144F  8A87CE43          mov al,[bx+0x43ce]
00001453  B400              mov ah,0x0
00001455  BB0A00            mov bx,0xa
00001458  99                cwd
00001459  F7FB              idiv bx
0000145B  40                inc ax
0000145C  BA0A00            mov dx,0xa
0000145F  F7EA              imul dx
00001461  99                cwd
00001462  8946F0            mov [bp-0x10],ax
00001465  8956F2            mov [bp-0xe],dx
00001468  0BD2              or dx,dx
0000146A  7F11              jg 0x147d
0000146C  7C05              jl 0x1473
0000146E  3D1400            cmp ax,0x14
00001471  730A              jnc 0x147d
00001473  C746F01400        mov word [bp-0x10],0x14
00001478  C746F20000        mov word [bp-0xe],0x0
0000147D  A0AA37            mov al,[0x37aa]
00001480  98                cbw
00001481  BA0600            mov dx,0x6
00001484  F7EA              imul dx
00001486  8BD8              mov bx,ax
00001488  8A87CF43          mov al,[bx+0x43cf]
0000148C  B400              mov ah,0x0
0000148E  BB0A00            mov bx,0xa
00001491  99                cwd
00001492  F7FB              idiv bx
00001494  40                inc ax
00001495  D1E0              shl ax,1
00001497  99                cwd
00001498  8946EC            mov [bp-0x14],ax
0000149B  8956EE            mov [bp-0x12],dx
0000149E  0BD2              or dx,dx
000014A0  7F11              jg 0x14b3
000014A2  7C05              jl 0x14a9
000014A4  3D0400            cmp ax,0x4
000014A7  730A              jnc 0x14b3
000014A9  C746EC0400        mov word [bp-0x14],0x4
000014AE  C746EE0000        mov word [bp-0x12],0x0
000014B3  9A5C093505        call 0x535:0x95c
000014B8  B8FFFF            mov ax,0xffff
000014BB  50                push ax
000014BC  B80100            mov ax,0x1
000014BF  50                push ax
000014C0  FF363238          push word [0x3832]
000014C4  FF363038          push word [0x3830]
000014C8  B82C00            mov ax,0x2c
000014CB  50                push ax
000014CC  8BC6              mov ax,si
000014CE  050F00            add ax,0xf
000014D1  50                push ax
000014D2  57                push di
000014D3  9A9000690A        call 0xa69:0x90
000014D8  83C40E            add sp,byte +0xe
000014DB  8BC6              mov ax,si
000014DD  051500            add ax,0x15
000014E0  50                push ax
000014E1  8BC7              mov ax,di
000014E3  050800            add ax,0x8
000014E6  50                push ax
000014E7  B82000            mov ax,0x20
000014EA  50                push ax
000014EB  B8A000            mov ax,0xa0
000014EE  50                push ax
000014EF  B84800            mov ax,0x48
000014F2  50                push ax
000014F3  9A1103960A        call 0xa96:0x311
000014F8  83C40A            add sp,byte +0xa
000014FB  C706D0450000      mov word [0x45d0],0x0
00001501  E9D500            jmp 0x15d9
00001504  8B5E06            mov bx,[bp+0x6]
00001507  D1E3              shl bx,1
00001509  D1E3              shl bx,1
0000150B  D1E3              shl bx,1
0000150D  A1D045            mov ax,[0x45d0]
00001510  D1E0              shl ax,1
00001512  03D8              add bx,ax
00001514  8B87F43C          mov ax,[bx+0x3cf4]
00001518  99                cwd
00001519  8946F4            mov [bp-0xc],ax
0000151C  8956F6            mov [bp-0xa],dx
0000151F  33C0              xor ax,ax
00001521  BA6400            mov dx,0x64
00001524  50                push ax
00001525  52                push dx
00001526  8B4EF6            mov cx,[bp-0xa]
00001529  8B5EF4            mov bx,[bp-0xc]
0000152C  8B56F2            mov dx,[bp-0xe]
0000152F  8B46F0            mov ax,[bp-0x10]
00001532  9A4E4F0000        call 0x0:0x4f4e
00001537  52                push dx
00001538  50                push ax
00001539  9A894F0000        call 0x0:0x4f89
0000153E  8946FC            mov [bp-0x4],ax
00001541  8956FE            mov [bp-0x2],dx
00001544  B8FFFF            mov ax,0xffff
00001547  50                push ax
00001548  33C0              xor ax,ax
0000154A  50                push ax
0000154B  B80300            mov ax,0x3
0000154E  50                push ax
0000154F  FF76FE            push word [bp-0x2]
00001552  FF76FC            push word [bp-0x4]
00001555  A1D045            mov ax,[0x45d0]
00001558  D1E0              shl ax,1
0000155A  D1E0              shl ax,1
0000155C  D1E0              shl ax,1
0000155E  8BD6              mov dx,si
00001560  03D0              add dx,ax
00001562  83C21A            add dx,byte +0x1a
00001565  52                push dx
00001566  8BC7              mov ax,di
00001568  051100            add ax,0x11
0000156B  50                push ax
0000156C  9A7A05960A        call 0xa96:0x57a
00001571  83C40E            add sp,byte +0xe
00001574  8B5E06            mov bx,[bp+0x6]
00001577  D1E3              shl bx,1
00001579  D1E3              shl bx,1
0000157B  D1E3              shl bx,1
0000157D  A1D045            mov ax,[0x45d0]
00001580  D1E0              shl ax,1
00001582  03D8              add bx,ax
00001584  8B87F43C          mov ax,[bx+0x3cf4]
00001588  2B46FC            sub ax,[bp-0x4]
0000158B  8B5E06            mov bx,[bp+0x6]
0000158E  D1E3              shl bx,1
00001590  D1E3              shl bx,1
00001592  D1E3              shl bx,1
00001594  8B16D045          mov dx,[0x45d0]
00001598  D1E2              shl dx,1
0000159A  03DA              add bx,dx
0000159C  8987F43C          mov [bx+0x3cf4],ax
000015A0  A0AA37            mov al,[0x37aa]
000015A3  98                cbw
000015A4  D1E0              shl ax,1
000015A6  D1E0              shl ax,1
000015A8  D1E0              shl ax,1
000015AA  8B16D045          mov dx,[0x45d0]
000015AE  D1E2              shl dx,1
000015B0  03C2              add ax,dx
000015B2  8BD8              mov bx,ax
000015B4  8B87223D          mov ax,[bx+0x3d22]
000015B8  0346FC            add ax,[bp-0x4]
000015BB  50                push ax
000015BC  A0AA37            mov al,[0x37aa]
000015BF  98                cbw
000015C0  D1E0              shl ax,1
000015C2  D1E0              shl ax,1
000015C4  D1E0              shl ax,1
000015C6  8B16D045          mov dx,[0x45d0]
000015CA  D1E2              shl dx,1
000015CC  03C2              add ax,dx
000015CE  8BD8              mov bx,ax
000015D0  58                pop ax
000015D1  8987223D          mov [bx+0x3d22],ax
000015D5  FF06D045          inc word [0x45d0]
000015D9  833ED04503        cmp word [0x45d0],byte +0x3
000015DE  7F03              jg 0x15e3
000015E0  E921FF            jmp 0x1504
000015E3  33C0              xor ax,ax
000015E5  BA6400            mov dx,0x64
000015E8  50                push ax
000015E9  52                push dx
000015EA  8B5E06            mov bx,[bp+0x6]
000015ED  D1E3              shl bx,1
000015EF  D1E3              shl bx,1
000015F1  8B8FD43C          mov cx,[bx+0x3cd4]
000015F5  8B9FD23C          mov bx,[bx+0x3cd2]
000015F9  8B56EE            mov dx,[bp-0x12]
000015FC  8B46EC            mov ax,[bp-0x14]
000015FF  9A4E4F0000        call 0x0:0x4f4e
00001604  52                push dx
00001605  50                push ax
00001606  9A894F0000        call 0x0:0x4f89
0000160B  8946F8            mov [bp-0x8],ax
0000160E  8956FA            mov [bp-0x6],dx
00001611  B8FFFF            mov ax,0xffff
00001614  50                push ax
00001615  33C0              xor ax,ax
00001617  50                push ax
00001618  B80500            mov ax,0x5
0000161B  50                push ax
0000161C  FF76FA            push word [bp-0x6]
0000161F  FF76F8            push word [bp-0x8]
00001622  8BC6              mov ax,si
00001624  053A00            add ax,0x3a
00001627  50                push ax
00001628  8BC7              mov ax,di
0000162A  40                inc ax
0000162B  40                inc ax
0000162C  50                push ax
0000162D  9AFB05960A        call 0xa96:0x5fb
00001632  83C40E            add sp,byte +0xe
00001635  8B5E06            mov bx,[bp+0x6]
00001638  D1E3              shl bx,1
0000163A  D1E3              shl bx,1
0000163C  8B46FA            mov ax,[bp-0x6]
0000163F  8B56F8            mov dx,[bp-0x8]
00001642  2997D23C          sub [bx+0x3cd2],dx
00001646  1987D43C          sbb [bx+0x3cd4],ax
0000164A  A0AA37            mov al,[0x37aa]
0000164D  98                cbw
0000164E  D1E0              shl ax,1
00001650  8BD8              mov bx,ax
00001652  8B87DA4B          mov ax,[bx+0x4bda]
00001656  0346F8            add ax,[bp-0x8]
00001659  50                push ax
0000165A  A0AA37            mov al,[0x37aa]
0000165D  98                cbw
0000165E  D1E0              shl ax,1
00001660  8BD8              mov bx,ax
00001662  58                pop ax
00001663  8987DA4B          mov [bx+0x4bda],ax
00001667  A0AA37            mov al,[0x37aa]
0000166A  98                cbw
0000166B  D1E0              shl ax,1
0000166D  8BD8              mov bx,ax
0000166F  C787262CE803      mov word [bx+0x2c26],0x3e8
00001675  5F                pop di
00001676  5E                pop si
00001677  8BE5              mov sp,bp
00001679  5D                pop bp
0000167A  CB                retf

; ---- 0EDE:167B ----
0000167B  55                push bp
0000167C  8BEC              mov bp,sp
0000167E  56                push si
0000167F  57                push di
00001680  33FF              xor di,di
00001682  EB45              jmp short 0x16c9
00001684  33F6              xor si,si
00001686  EB3B              jmp short 0x16c3
00001688  8BC7              mov ax,di
0000168A  BA0700            mov dx,0x7
0000168D  F7EA              imul dx
0000168F  8BD8              mov bx,ax
00001691  8A80FE43          mov al,[bx+si+0x43fe]
00001695  98                cbw
00001696  3B4606            cmp ax,[bp+0x6]
00001699  7402              jz 0x169d
0000169B  EB25              jmp short 0x16c2
0000169D  8BC7              mov ax,di
0000169F  BA0700            mov dx,0x7
000016A2  F7EA              imul dx
000016A4  8BD8              mov bx,ax
000016A6  8A801A44          mov al,[bx+si+0x441a]
000016AA  98                cbw
000016AB  3B4608            cmp ax,[bp+0x8]
000016AE  7402              jz 0x16b2
000016B0  EB10              jmp short 0x16c2
000016B2  8BC7              mov ax,di
000016B4  BA0700            mov dx,0x7
000016B7  F7EA              imul dx
000016B9  8BD8              mov bx,ax
000016BB  C680FE43FF        mov byte [bx+si+0x43fe],0xff
000016C0  EB0C              jmp short 0x16ce
000016C2  46                inc si
000016C3  83FE07            cmp si,byte +0x7
000016C6  7CC0              jl 0x1688
000016C8  47                inc di
000016C9  83FF04            cmp di,byte +0x4
000016CC  7CB6              jl 0x1684
000016CE  5F                pop di
000016CF  5E                pop si
000016D0  5D                pop bp
000016D1  CB                retf

; ---- 0EDE:16D2 ----
000016D2  55                push bp
000016D3  8BEC              mov bp,sp
000016D5  83EC06            sub sp,byte +0x6
000016D8  56                push si
000016D9  57                push di
000016DA  A11A3D            mov ax,[0x3d1a]
000016DD  BB0600            mov bx,0x6
000016E0  99                cwd
000016E1  F7FB              idiv bx
000016E3  0BD2              or dx,dx
000016E5  7403              jz 0x16ea
000016E7  E93601            jmp 0x1820
000016EA  A0AA37            mov al,[0x37aa]
000016ED  98                cbw
000016EE  8BD8              mov bx,ax
000016F0  8A878237          mov al,[bx+0x3782]
000016F4  04FF              add al,0xff
000016F6  50                push ax
000016F7  A0AA37            mov al,[0x37aa]
000016FA  98                cbw
000016FB  8BD8              mov bx,ax
000016FD  58                pop ax
000016FE  88878237          mov [bx+0x3782],al
00001702  0AC0              or al,al
00001704  7608              jna 0x170e
00001706  9ACF22DE0E        call 0xede:0x22cf
0000170B  E91201            jmp 0x1820
0000170E  B80500            mov ax,0x5
00001711  50                push ax
00001712  9ABA01FD21        call 0x21fd:0x1ba
00001717  59                pop cx
00001718  8BF0              mov si,ax
0000171A  33FF              xor di,di
0000171C  8BC6              mov ax,si
0000171E  40                inc ax
0000171F  8BF0              mov si,ax
00001721  3D0500            cmp ax,0x5
00001724  7E02              jng 0x1728
00001726  33F6              xor si,si
00001728  8BC7              mov ax,di
0000172A  40                inc ax
0000172B  8BF8              mov di,ax
0000172D  3D0600            cmp ax,0x6
00001730  7E03              jng 0x1735
00001732  E9CC00            jmp 0x1801
00001735  A0AA37            mov al,[0x37aa]
00001738  98                cbw
00001739  BA0600            mov dx,0x6
0000173C  F7EA              imul dx
0000173E  8BD8              mov bx,ax
00001740  8A80CE43          mov al,[bx+si+0x43ce]
00001744  B400              mov ah,0x0
00001746  8946FC            mov [bp-0x4],ax
00001749  837EFC0A          cmp word [bp-0x4],byte +0xa
0000174D  7D03              jnl 0x1752
0000174F  E99F00            jmp 0x17f1
00001752  8BC6              mov ax,si
00001754  BB0200            mov bx,0x2
00001757  99                cwd
00001758  F7FB              idiv bx
0000175A  50                push ax
0000175B  9A020B681C        call 0x1c68:0xb02
00001760  59                pop cx
00001761  8946FA            mov [bp-0x6],ax
00001764  A0AA37            mov al,[0x37aa]
00001767  98                cbw
00001768  8BD8              mov bx,ax
0000176A  8A87A637          mov al,[bx+0x37a6]
0000176E  98                cbw
0000176F  3B46FC            cmp ax,[bp-0x4]
00001772  7F0D              jg 0x1781
00001774  A0AA37            mov al,[0x37aa]
00001777  98                cbw
00001778  8BD8              mov bx,ax
0000177A  8A87A637          mov al,[bx+0x37a6]
0000177E  98                cbw
0000177F  EB03              jmp short 0x1784
00001781  8B46FC            mov ax,[bp-0x4]
00001784  8946FE            mov [bp-0x2],ax
00001787  8B46FE            mov ax,[bp-0x2]
0000178A  2946FC            sub [bp-0x4],ax
0000178D  A0AA37            mov al,[0x37aa]
00001790  98                cbw
00001791  8BD8              mov bx,ax
00001793  8A87A637          mov al,[bx+0x37a6]
00001797  2A46FE            sub al,[bp-0x2]
0000179A  50                push ax
0000179B  A0AA37            mov al,[0x37aa]
0000179E  98                cbw
0000179F  8BD8              mov bx,ax
000017A1  58                pop ax
000017A2  8887A637          mov [bx+0x37a6],al
000017A6  A0AA37            mov al,[0x37aa]
000017A9  98                cbw
000017AA  BA0600            mov dx,0x6
000017AD  F7EA              imul dx
000017AF  8A56FC            mov dl,[bp-0x4]
000017B2  8BD8              mov bx,ax
000017B4  8890CE43          mov [bx+si+0x43ce],dl
000017B8  8BC6              mov ax,si
000017BA  BB0200            mov bx,0x2
000017BD  99                cwd
000017BE  F7FB              idiv bx
000017C0  50                push ax
000017C1  9A020B681C        call 0x1c68:0xb02
000017C6  59                pop cx
000017C7  3B46FA            cmp ax,[bp-0x6]
000017CA  741E              jz 0x17ea
000017CC  A0AA37            mov al,[0x37aa]
000017CF  98                cbw
000017D0  D1E0              shl ax,1
000017D2  8BD8              mov bx,ax
000017D4  83BF262C00        cmp word [bx+0x2c26],byte +0x0
000017D9  750F              jnz 0x17ea
000017DB  8BC6              mov ax,si
000017DD  BB0200            mov bx,0x2
000017E0  99                cwd
000017E1  F7FB              idiv bx
000017E3  50                push ax
000017E4  9ACF1DDE0E        call 0xede:0x1dcf
000017E9  59                pop cx
000017EA  56                push si
000017EB  9AFF1DDE0E        call 0xede:0x1dff
000017F0  59                pop cx
000017F1  A0AA37            mov al,[0x37aa]
000017F4  98                cbw
000017F5  8BD8              mov bx,ax
000017F7  80BFA63700        cmp byte [bx+0x37a6],0x0
000017FC  7E03              jng 0x1801
000017FE  E91BFF            jmp 0x171c
00001801  9A561B0F15        call 0x150f:0x1b56
00001806  9A2618DE0E        call 0xede:0x1826
0000180B  3C01              cmp al,0x1
0000180D  750C              jnz 0x181b
0000180F  9A781FDE0E        call 0xede:0x1f78
00001814  9A5118DE0E        call 0xede:0x1851
00001819  EB05              jmp short 0x1820
0000181B  9ACF22DE0E        call 0xede:0x22cf
00001820  5F                pop di
00001821  5E                pop si
00001822  8BE5              mov sp,bp
00001824  5D                pop bp
00001825  CB                retf

; ---- 0EDE:1826 ----
00001826  55                push bp
00001827  8BEC              mov bp,sp
00001829  56                push si
0000182A  33F6              xor si,si
0000182C  EB17              jmp short 0x1845
0000182E  A0AA37            mov al,[0x37aa]
00001831  98                cbw
00001832  BA0600            mov dx,0x6
00001835  F7EA              imul dx
00001837  8BD8              mov bx,ax
00001839  80B8CE430A        cmp byte [bx+si+0x43ce],0xa
0000183E  7204              jc 0x1844
00001840  B000              mov al,0x0
00001842  EB0A              jmp short 0x184e
00001844  46                inc si
00001845  83FE05            cmp si,byte +0x5
00001848  7EE4              jng 0x182e
0000184A  B001              mov al,0x1
0000184C  EB00              jmp short 0x184e
0000184E  5E                pop si
0000184F  5D                pop bp
00001850  CB                retf

; ---- 0EDE:1851 ----
00001851  55                push bp
00001852  8BEC              mov bp,sp
00001854  0E                push cs
00001855  E8CAEA            call 0x322
00001858  A0AA37            mov al,[0x37aa]
0000185B  98                cbw
0000185C  8BD8              mov bx,ax
0000185E  C687A63700        mov byte [bx+0x37a6],0x0
00001863  9A3323DE0E        call 0xede:0x2333
00001868  0E                push cs
00001869  E81DEC            call 0x489
0000186C  A0AA37            mov al,[0x37aa]
0000186F  98                cbw
00001870  8BD8              mov bx,ax
00001872  C687B2430F        mov byte [bx+0x43b2],0xf
00001877  A0AA37            mov al,[0x37aa]
0000187A  98                cbw
0000187B  8BD8              mov bx,ax
0000187D  80BF400800        cmp byte [bx+0x840],0x0
00001882  7516              jnz 0x189a
00001884  A0AA37            mov al,[0x37aa]
00001887  98                cbw
00001888  D1E0              shl ax,1
0000188A  D1E0              shl ax,1
0000188C  8BD8              mov bx,ax
0000188E  C78786371027      mov word [bx+0x3786],0x2710
00001894  C78788370000      mov word [bx+0x3788],0x0
0000189A  5D                pop bp
0000189B  CB                retf

; ---- 0EDE:189C ----
0000189C  55                push bp
0000189D  8BEC              mov bp,sp
0000189F  4C                dec sp
000018A0  4C                dec sp
000018A1  A0AA37            mov al,[0x37aa]
000018A4  98                cbw
000018A5  8BD8              mov bx,ax
000018A7  8A875107          mov al,[bx+0x751]
000018AB  8846FF            mov [bp-0x1],al
000018AE  A0AA37            mov al,[0x37aa]
000018B1  98                cbw
000018B2  8BD8              mov bx,ax
000018B4  8A87012F          mov al,[bx+0x2f01]
000018B8  FEC0              inc al
000018BA  8846FE            mov [bp-0x2],al
000018BD  A0AA37            mov al,[0x37aa]
000018C0  98                cbw
000018C1  D1E0              shl ax,1
000018C3  8BD8              mov bx,ax
000018C5  8B874907          mov ax,[bx+0x749]
000018C9  48                dec ax
000018CA  50                push ax
000018CB  A0AA37            mov al,[0x37aa]
000018CE  98                cbw
000018CF  D1E0              shl ax,1
000018D1  8BD8              mov bx,ax
000018D3  58                pop ax
000018D4  89874907          mov [bx+0x749],ax
000018D8  50                push ax
000018D9  8A46FE            mov al,[bp-0x2]
000018DC  98                cbw
000018DD  D1E0              shl ax,1
000018DF  8BD8              mov bx,ax
000018E1  8B873F07          mov ax,[bx+0x73f]
000018E5  2D0700            sub ax,0x7
000018E8  5A                pop dx
000018E9  3BD0              cmp dx,ax
000018EB  7515              jnz 0x1902
000018ED  A0AA37            mov al,[0x37aa]
000018F0  98                cbw
000018F1  D1E0              shl ax,1
000018F3  8BD8              mov bx,ax
000018F5  FFB7162C          push word [bx+0x2c16]
000018F9  9A831BDE0E        call 0xede:0x1b83
000018FE  59                pop cx
000018FF  E9A601            jmp 0x1aa8
00001902  A0AA37            mov al,[0x37aa]
00001905  98                cbw
00001906  D1E0              shl ax,1
00001908  50                push ax
00001909  8A46FE            mov al,[bp-0x2]
0000190C  98                cbw
0000190D  D1E0              shl ax,1
0000190F  8BD8              mov bx,ax
00001911  8B873F07          mov ax,[bx+0x73f]
00001915  2D0800            sub ax,0x8
00001918  5B                pop bx
00001919  39874907          cmp [bx+0x749],ax
0000191D  755F              jnz 0x197e
0000191F  A0AA37            mov al,[0x37aa]
00001922  98                cbw
00001923  D1E0              shl ax,1
00001925  8BD8              mov bx,ax
00001927  83BF162CFF        cmp word [bx+0x2c16],byte -0x1
0000192C  744D              jz 0x197b
0000192E  A0AA37            mov al,[0x37aa]
00001931  98                cbw
00001932  BA0700            mov dx,0x7
00001935  F7EA              imul dx
00001937  50                push ax
00001938  8A46FF            mov al,[bp-0x1]
0000193B  98                cbw
0000193C  5B                pop bx
0000193D  03D8              add bx,ax
0000193F  80BFFE43FF        cmp byte [bx+0x43fe],0xff
00001944  7435              jz 0x197b
00001946  A0AA37            mov al,[0x37aa]
00001949  98                cbw
0000194A  BA0700            mov dx,0x7
0000194D  F7EA              imul dx
0000194F  50                push ax
00001950  8A46FF            mov al,[bp-0x1]
00001953  98                cbw
00001954  5B                pop bx
00001955  03D8              add bx,ax
00001957  8A871A44          mov al,[bx+0x441a]
0000195B  98                cbw
0000195C  50                push ax
0000195D  A0AA37            mov al,[0x37aa]
00001960  98                cbw
00001961  BA0700            mov dx,0x7
00001964  F7EA              imul dx
00001966  50                push ax
00001967  8A46FF            mov al,[bp-0x1]
0000196A  98                cbw
0000196B  5B                pop bx
0000196C  03D8              add bx,ax
0000196E  8A87FE43          mov al,[bx+0x43fe]
00001972  98                cbw
00001973  50                push ax
00001974  9A8D087E1E        call 0x1e7e:0x88d
00001979  59                pop cx
0000197A  59                pop cx
0000197B  E92A01            jmp 0x1aa8
0000197E  A0AA37            mov al,[0x37aa]
00001981  98                cbw
00001982  D1E0              shl ax,1
00001984  8BD8              mov bx,ax
00001986  83BF490700        cmp word [bx+0x749],byte +0x0
0000198B  7403              jz 0x1990
0000198D  E91801            jmp 0x1aa8
00001990  A0AA37            mov al,[0x37aa]
00001993  98                cbw
00001994  D1E0              shl ax,1
00001996  8BD8              mov bx,ax
00001998  C787162CFFFF      mov word [bx+0x2c16],0xffff
0000199E  A0AA37            mov al,[0x37aa]
000019A1  98                cbw
000019A2  D1E0              shl ax,1
000019A4  8BD8              mov bx,ax
000019A6  83BFF82E00        cmp word [bx+0x2ef8],byte +0x0
000019AB  7703              ja 0x19b0
000019AD  E9DE00            jmp 0x1a8e
000019B0  8A46FF            mov al,[bp-0x1]
000019B3  FEC0              inc al
000019B5  8846FF            mov [bp-0x1],al
000019B8  3C07              cmp al,0x7
000019BA  7C04              jl 0x19c0
000019BC  C646FF00          mov byte [bp-0x1],0x0
000019C0  A0AA37            mov al,[0x37aa]
000019C3  98                cbw
000019C4  8A56FF            mov dl,[bp-0x1]
000019C7  8BD8              mov bx,ax
000019C9  88975107          mov [bx+0x751],dl
000019CD  A0AA37            mov al,[0x37aa]
000019D0  98                cbw
000019D1  BA0700            mov dx,0x7
000019D4  F7EA              imul dx
000019D6  50                push ax
000019D7  8A46FF            mov al,[bp-0x1]
000019DA  98                cbw
000019DB  5B                pop bx
000019DC  03D8              add bx,ax
000019DE  80BFFE43FF        cmp byte [bx+0x43fe],0xff
000019E3  7503              jnz 0x19e8
000019E5  E9A600            jmp 0x1a8e
000019E8  B80300            mov ax,0x3
000019EB  50                push ax
000019EC  A0AA37            mov al,[0x37aa]
000019EF  98                cbw
000019F0  BA0700            mov dx,0x7
000019F3  F7EA              imul dx
000019F5  50                push ax
000019F6  8A46FF            mov al,[bp-0x1]
000019F9  98                cbw
000019FA  5B                pop bx
000019FB  03D8              add bx,ax
000019FD  8A871A44          mov al,[bx+0x441a]
00001A01  98                cbw
00001A02  50                push ax
00001A03  A0AA37            mov al,[0x37aa]
00001A06  98                cbw
00001A07  BA0700            mov dx,0x7
00001A0A  F7EA              imul dx
00001A0C  50                push ax
00001A0D  8A46FF            mov al,[bp-0x1]
00001A10  98                cbw
00001A11  5B                pop bx
00001A12  03D8              add bx,ax
00001A14  8A87FE43          mov al,[bx+0x43fe]
00001A18  98                cbw
00001A19  50                push ax
00001A1A  9AAC1ADE0E        call 0xede:0x1aac
00001A1F  83C406            add sp,byte +0x6
00001A22  50                push ax
00001A23  A0AA37            mov al,[0x37aa]
00001A26  98                cbw
00001A27  D1E0              shl ax,1
00001A29  8BD8              mov bx,ax
00001A2B  58                pop ax
00001A2C  8987162C          mov [bx+0x2c16],ax
00001A30  A0AA37            mov al,[0x37aa]
00001A33  98                cbw
00001A34  D1E0              shl ax,1
00001A36  8BD8              mov bx,ax
00001A38  83BF162CFF        cmp word [bx+0x2c16],byte -0x1
00001A3D  744F              jz 0x1a8e
00001A3F  A0AA37            mov al,[0x37aa]
00001A42  98                cbw
00001A43  BA0700            mov dx,0x7
00001A46  F7EA              imul dx
00001A48  50                push ax
00001A49  8A46FF            mov al,[bp-0x1]
00001A4C  98                cbw
00001A4D  5B                pop bx
00001A4E  03D8              add bx,ax
00001A50  8A871A44          mov al,[bx+0x441a]
00001A54  98                cbw
00001A55  50                push ax
00001A56  A0AA37            mov al,[0x37aa]
00001A59  98                cbw
00001A5A  BA0700            mov dx,0x7
00001A5D  F7EA              imul dx
00001A5F  50                push ax
00001A60  8A46FF            mov al,[bp-0x1]
00001A63  98                cbw
00001A64  5B                pop bx
00001A65  03D8              add bx,ax
00001A67  8A87FE43          mov al,[bx+0x43fe]
00001A6B  98                cbw
00001A6C  50                push ax
00001A6D  9AB5087E1E        call 0x1e7e:0x8b5
00001A72  59                pop cx
00001A73  59                pop cx
00001A74  B84500            mov ax,0x45
00001A77  50                push ax
00001A78  33C0              xor ax,ax
00001A7A  50                push ax
00001A7B  9A5400A721        call 0x21a7:0x54
00001A80  59                pop cx
00001A81  59                pop cx
00001A82  A0AA37            mov al,[0x37aa]
00001A85  98                cbw
00001A86  D1E0              shl ax,1
00001A88  8BD8              mov bx,ax
00001A8A  FF8FF82E          dec word [bx+0x2ef8]
00001A8E  8A46FE            mov al,[bp-0x2]
00001A91  98                cbw
00001A92  D1E0              shl ax,1
00001A94  8BD8              mov bx,ax
00001A96  8B873F07          mov ax,[bx+0x73f]
00001A9A  50                push ax
00001A9B  A0AA37            mov al,[0x37aa]
00001A9E  98                cbw
00001A9F  D1E0              shl ax,1
00001AA1  8BD8              mov bx,ax
00001AA3  58                pop ax
00001AA4  89874907          mov [bx+0x749],ax
00001AA8  8BE5              mov sp,bp
00001AAA  5D                pop bp
00001AAB  CB                retf

; ---- 0EDE:1AAC ----
00001AAC  55                push bp
00001AAD  8BEC              mov bp,sp
00001AAF  83EC0A            sub sp,byte +0xa
00001AB2  56                push si
00001AB3  57                push di
00001AB4  C746F81027        mov word [bp-0x8],0x2710
00001AB9  C746F6FFFF        mov word [bp-0xa],0xffff
00001ABE  33F6              xor si,si
00001AC0  E9AD00            jmp 0x1b70
00001AC3  A0AA37            mov al,[0x37aa]
00001AC6  98                cbw
00001AC7  3BC6              cmp ax,si
00001AC9  7407              jz 0x1ad2
00001ACB  80BCB24310        cmp byte [si+0x43b2],0x10
00001AD0  7403              jz 0x1ad5
00001AD2  E99A00            jmp 0x1b6f
00001AD5  8BDE              mov bx,si
00001AD7  D1E3              shl bx,1
00001AD9  8B87E24B          mov ax,[bx+0x4be2]
00001ADD  8946FC            mov [bp-0x4],ax
00001AE0  8BDE              mov bx,si
00001AE2  D1E3              shl bx,1
00001AE4  8B87EA4B          mov ax,[bx+0x4bea]
00001AE8  8946FA            mov [bp-0x6],ax
00001AEB  8B4606            mov ax,[bp+0x6]
00001AEE  3B46FC            cmp ax,[bp-0x4]
00001AF1  7E08              jng 0x1afb
00001AF3  8B4606            mov ax,[bp+0x6]
00001AF6  2B46FC            sub ax,[bp-0x4]
00001AF9  EB06              jmp short 0x1b01
00001AFB  8B46FC            mov ax,[bp-0x4]
00001AFE  2B4606            sub ax,[bp+0x6]
00001B01  8BC8              mov cx,ax
00001B03  3B460A            cmp ax,[bp+0xa]
00001B06  7E02              jng 0x1b0a
00001B08  EB65              jmp short 0x1b6f
00001B0A  8B4608            mov ax,[bp+0x8]
00001B0D  3B46FA            cmp ax,[bp-0x6]
00001B10  7E08              jng 0x1b1a
00001B12  8B4608            mov ax,[bp+0x8]
00001B15  2B46FA            sub ax,[bp-0x6]
00001B18  EB06              jmp short 0x1b20
00001B1A  8B46FA            mov ax,[bp-0x6]
00001B1D  2B4608            sub ax,[bp+0x8]
00001B20  8BF8              mov di,ax
00001B22  3B460A            cmp ax,[bp+0xa]
00001B25  7E02              jng 0x1b29
00001B27  EB46              jmp short 0x1b6f
00001B29  3BCF              cmp cx,di
00001B2B  7E18              jng 0x1b45
00001B2D  8BC1              mov ax,cx
00001B2F  2BC7              sub ax,di
00001B31  BA0500            mov dx,0x5
00001B34  F7EA              imul dx
00001B36  50                push ax
00001B37  8BC7              mov ax,di
00001B39  BA0700            mov dx,0x7
00001B3C  F7EA              imul dx
00001B3E  8BD0              mov dx,ax
00001B40  58                pop ax
00001B41  03C2              add ax,dx
00001B43  EB16              jmp short 0x1b5b
00001B45  8BC7              mov ax,di
00001B47  2BC1              sub ax,cx
00001B49  BA0500            mov dx,0x5
00001B4C  F7EA              imul dx
00001B4E  50                push ax
00001B4F  8BC1              mov ax,cx
00001B51  BA0700            mov dx,0x7
00001B54  F7EA              imul dx
00001B56  8BD0              mov dx,ax
00001B58  58                pop ax
00001B59  03C2              add ax,dx
00001B5B  8946FE            mov [bp-0x2],ax
00001B5E  8B46FE            mov ax,[bp-0x2]
00001B61  3B46F8            cmp ax,[bp-0x8]
00001B64  7D09              jnl 0x1b6f
00001B66  8B46FE            mov ax,[bp-0x2]
00001B69  8946F8            mov [bp-0x8],ax
00001B6C  8976F6            mov [bp-0xa],si
00001B6F  46                inc si
00001B70  83FE03            cmp si,byte +0x3
00001B73  7F03              jg 0x1b78
00001B75  E94BFF            jmp 0x1ac3
00001B78  8B46F6            mov ax,[bp-0xa]
00001B7B  EB00              jmp short 0x1b7d
00001B7D  5F                pop di
00001B7E  5E                pop si
00001B7F  8BE5              mov sp,bp
00001B81  5D                pop bp
00001B82  CB                retf

; ---- 0EDE:1B83 ----
00001B83  55                push bp
00001B84  8BEC              mov bp,sp
00001B86  4C                dec sp
00001B87  4C                dec sp
00001B88  56                push si
00001B89  57                push di
00001B8A  8B7606            mov si,[bp+0x6]
00001B8D  83FEFF            cmp si,byte -0x1
00001B90  7503              jnz 0x1b95
00001B92  E99200            jmp 0x1c27
00001B95  B80100            mov ax,0x1
00001B98  50                push ax
00001B99  9ABA01FD21        call 0x21fd:0x1ba
00001B9E  59                pop cx
00001B9F  0BC0              or ax,ax
00001BA1  751F              jnz 0x1bc2
00001BA3  8A84A637          mov al,[si+0x37a6]
00001BA7  0408              add al,0x8
00001BA9  8884A637          mov [si+0x37a6],al
00001BAD  C684823704        mov byte [si+0x3782],0x4
00001BB2  B84600            mov ax,0x46
00001BB5  50                push ax
00001BB6  33C0              xor ax,ax
00001BB8  50                push ax
00001BB9  9A5400A721        call 0x21a7:0x54
00001BBE  59                pop cx
00001BBF  59                pop cx
00001BC0  EB65              jmp short 0x1c27
00001BC2  8BDE              mov bx,si
00001BC4  D1E3              shl bx,1
00001BC6  8B87E24B          mov ax,[bx+0x4be2]
00001BCA  8946FE            mov [bp-0x2],ax
00001BCD  8BDE              mov bx,si
00001BCF  D1E3              shl bx,1
00001BD1  8BBFEA4B          mov di,[bx+0x4bea]
00001BD5  8B46FE            mov ax,[bp-0x2]
00001BD8  BA1A00            mov dx,0x1a
00001BDB  F7EA              imul dx
00001BDD  8BD8              mov bx,ax
00001BDF  8A81F448          mov al,[bx+di+0x48f4]
00001BE3  B400              mov ah,0x0
00001BE5  BB0800            mov bx,0x8
00001BE8  99                cwd
00001BE9  F7FB              idiv bx
00001BEB  0BC0              or ax,ax
00001BED  7538              jnz 0x1c27
00001BEF  B80600            mov ax,0x6
00001BF2  50                push ax
00001BF3  9ABA01FD21        call 0x21fd:0x1ba
00001BF8  59                pop cx
00001BF9  8BD7              mov dx,di
00001BFB  D1E2              shl dx,1
00001BFD  D1E2              shl dx,1
00001BFF  D1E2              shl dx,1
00001C01  83C2F9            add dx,byte -0x7
00001C04  03D0              add dx,ax
00001C06  52                push dx
00001C07  B80600            mov ax,0x6
00001C0A  50                push ax
00001C0B  9ABA01FD21        call 0x21fd:0x1ba
00001C10  59                pop cx
00001C11  8B56FE            mov dx,[bp-0x2]
00001C14  D1E2              shl dx,1
00001C16  D1E2              shl dx,1
00001C18  D1E2              shl dx,1
00001C1A  83C234            add dx,byte +0x34
00001C1D  03D0              add dx,ax
00001C1F  52                push dx
00001C20  9A210E7E1E        call 0x1e7e:0xe21
00001C25  59                pop cx
00001C26  59                pop cx
00001C27  5F                pop di
00001C28  5E                pop si
00001C29  8BE5              mov sp,bp
00001C2B  5D                pop bp
00001C2C  CB                retf

; ---- 0EDE:1C2D ----
00001C2D  55                push bp
00001C2E  8BEC              mov bp,sp
00001C30  C706D0450000      mov word [0x45d0],0x0
00001C36  EB59              jmp short 0x1c91
00001C38  A0AA37            mov al,[0x37aa]
00001C3B  98                cbw
00001C3C  D1E0              shl ax,1
00001C3E  D1E0              shl ax,1
00001C40  D1E0              shl ax,1
00001C42  8B16D045          mov dx,[0x45d0]
00001C46  D1E2              shl dx,1
00001C48  03C2              add ax,dx
00001C4A  8BD8              mov bx,ax
00001C4C  8B87223D          mov ax,[bx+0x3d22]
00001C50  50                push ax
00001C51  A0AA37            mov al,[0x37aa]
00001C54  98                cbw
00001C55  D1E0              shl ax,1
00001C57  D1E0              shl ax,1
00001C59  D1E0              shl ax,1
00001C5B  8B16D045          mov dx,[0x45d0]
00001C5F  D1E2              shl dx,1
00001C61  03C2              add ax,dx
00001C63  8BD8              mov bx,ax
00001C65  58                pop ax
00001C66  0187F43C          add [bx+0x3cf4],ax
00001C6A  8B87F43C          mov ax,[bx+0x3cf4]
00001C6E  3DE703            cmp ax,0x3e7
00001C71  7E1A              jng 0x1c8d
00001C73  A0AA37            mov al,[0x37aa]
00001C76  98                cbw
00001C77  D1E0              shl ax,1
00001C79  D1E0              shl ax,1
00001C7B  D1E0              shl ax,1
00001C7D  8B16D045          mov dx,[0x45d0]
00001C81  D1E2              shl dx,1
00001C83  03C2              add ax,dx
00001C85  8BD8              mov bx,ax
00001C87  C787F43CE703      mov word [bx+0x3cf4],0x3e7
00001C8D  FF06D045          inc word [0x45d0]
00001C91  833ED04503        cmp word [0x45d0],byte +0x3
00001C96  7EA0              jng 0x1c38
00001C98  A0AA37            mov al,[0x37aa]
00001C9B  98                cbw
00001C9C  D1E0              shl ax,1
00001C9E  8BD8              mov bx,ax
00001CA0  8B87DA4B          mov ax,[bx+0x4bda]
00001CA4  99                cwd
00001CA5  50                push ax
00001CA6  A0AA37            mov al,[0x37aa]
00001CA9  98                cbw
00001CAA  D1E0              shl ax,1
00001CAC  D1E0              shl ax,1
00001CAE  8BD8              mov bx,ax
00001CB0  58                pop ax
00001CB1  0187D23C          add [bx+0x3cd2],ax
00001CB5  1197D43C          adc [bx+0x3cd4],dx
00001CB9  5D                pop bp
00001CBA  CB                retf

; ---- 0EDE:1CBB ----
00001CBB  55                push bp
00001CBC  8BEC              mov bp,sp
00001CBE  1E                push ds
00001CBF  B84C0E            mov ax,0xe4c
00001CC2  50                push ax
00001CC3  9A6C02FD21        call 0x21fd:0x26c
00001CC8  59                pop cx
00001CC9  59                pop cx
00001CCA  9A8D0B3505        call 0x535:0xb8d
00001CCF  5D                pop bp
00001CD0  CB                retf

; ---- 0EDE:1CD1 ----
00001CD1  55                push bp
00001CD2  8BEC              mov bp,sp
00001CD4  83EC04            sub sp,byte +0x4
00001CD7  A0AA37            mov al,[0x37aa]
00001CDA  98                cbw
00001CDB  D1E0              shl ax,1
00001CDD  D1E0              shl ax,1
00001CDF  8BD8              mov bx,ax
00001CE1  8387983C01        add word [bx+0x3c98],byte +0x1
00001CE6  8B87983C          mov ax,[bx+0x3c98]
00001CEA  83979A3C00        adc word [bx+0x3c9a],byte +0x0
00001CEF  8B979A3C          mov dx,[bx+0x3c9a]
00001CF3  8946FC            mov [bp-0x4],ax
00001CF6  8956FE            mov [bp-0x2],dx
00001CF9  0BD2              or dx,dx
00001CFB  7510              jnz 0x1d0d
00001CFD  3D1900            cmp ax,0x19
00001D00  750B              jnz 0x1d0d
00001D02  B001              mov al,0x1
00001D04  50                push ax
00001D05  9A671DDE0E        call 0xede:0x1d67
00001D0A  59                pop cx
00001D0B  EB15              jmp short 0x1d22
00001D0D  837EFE00          cmp word [bp-0x2],byte +0x0
00001D11  750F              jnz 0x1d22
00001D13  837EFC32          cmp word [bp-0x4],byte +0x32
00001D17  7509              jnz 0x1d22
00001D19  B002              mov al,0x2
00001D1B  50                push ax
00001D1C  9A671DDE0E        call 0xede:0x1d67
00001D21  59                pop cx
00001D22  8BE5              mov sp,bp
00001D24  5D                pop bp
00001D25  CB                retf

; ---- 0EDE:1D26 ----
00001D26  55                push bp
00001D27  8BEC              mov bp,sp
00001D29  56                push si
00001D2A  A0AA37            mov al,[0x37aa]
00001D2D  98                cbw
00001D2E  D1E0              shl ax,1
00001D30  8BD8              mov bx,ax
00001D32  8B87E83C          mov ax,[bx+0x3ce8]
00001D36  48                dec ax
00001D37  50                push ax
00001D38  A0AA37            mov al,[0x37aa]
00001D3B  98                cbw
00001D3C  D1E0              shl ax,1
00001D3E  8BD8              mov bx,ax
00001D40  58                pop ax
00001D41  8987E83C          mov [bx+0x3ce8],ax
00001D45  8BF0              mov si,ax
00001D47  83FE08            cmp si,byte +0x8
00001D4A  750B              jnz 0x1d57
00001D4C  B002              mov al,0x2
00001D4E  50                push ax
00001D4F  9A671DDE0E        call 0xede:0x1d67
00001D54  59                pop cx
00001D55  EB0D              jmp short 0x1d64
00001D57  0BF6              or si,si
00001D59  7509              jnz 0x1d64
00001D5B  B000              mov al,0x0
00001D5D  50                push ax
00001D5E  9A671DDE0E        call 0xede:0x1d67
00001D63  59                pop cx
00001D64  5E                pop si
00001D65  5D                pop bp
00001D66  CB                retf

; ---- 0EDE:1D67 ----
00001D67  55                push bp
00001D68  8BEC              mov bp,sp
00001D6A  A0AA37            mov al,[0x37aa]
00001D6D  98                cbw
00001D6E  8A5606            mov dl,[bp+0x6]
00001D71  8BD8              mov bx,ax
00001D73  88971E2C          mov [bx+0x2c1e],dl
00001D77  9ACF22DE0E        call 0xede:0x22cf
00001D7C  5D                pop bp
00001D7D  CB                retf

; ---- 0EDE:1D7E ----
00001D7E  55                push bp
00001D7F  8BEC              mov bp,sp
00001D81  A05D07            mov al,[0x75d]
00001D84  98                cbw
00001D85  BB0300            mov bx,0x3
00001D88  99                cwd
00001D89  F7FB              idiv bx
00001D8B  50                push ax
00001D8C  B87707            mov ax,0x777
00001D8F  50                push ax
00001D90  9AEB03FD21        call 0x21fd:0x3eb
00001D95  59                pop cx
00001D96  59                pop cx
00001D97  A05D07            mov al,[0x75d]
00001D9A  FEC8              dec al
00001D9C  A25D07            mov [0x75d],al
00001D9F  0AC0              or al,al
00001DA1  7D0D              jnl 0x1db0
00001DA3  A0C737            mov al,[0x37c7]
00001DA6  24FD              and al,0xfd
00001DA8  A2C737            mov [0x37c7],al
00001DAB  C6065D0715        mov byte [0x75d],0x15
00001DB0  5D                pop bp
00001DB1  CB                retf

; ---- 0EDE:1DB2 ----
00001DB2  55                push bp
00001DB3  8BEC              mov bp,sp
00001DB5  56                push si
00001DB6  9A5C093505        call 0x535:0x95c
00001DBB  33F6              xor si,si
00001DBD  EB08              jmp short 0x1dc7
00001DBF  56                push si
00001DC0  9ACF1DDE0E        call 0xede:0x1dcf
00001DC5  59                pop cx
00001DC6  46                inc si
00001DC7  83FE02            cmp si,byte +0x2
00001DCA  7EF3              jng 0x1dbf
00001DCC  5E                pop si
00001DCD  5D                pop bp
00001DCE  CB                retf

; ---- 0EDE:1DCF ----
00001DCF  55                push bp
00001DD0  8BEC              mov bp,sp
00001DD2  FF7606            push word [bp+0x6]
00001DD5  A0AA37            mov al,[0x37aa]
00001DD8  98                cbw
00001DD9  D1E0              shl ax,1
00001DDB  8BD8              mov bx,ax
00001DDD  8B877008          mov ax,[bx+0x870]
00001DE1  050400            add ax,0x4
00001DE4  50                push ax
00001DE5  A0AA37            mov al,[0x37aa]
00001DE8  98                cbw
00001DE9  D1E0              shl ax,1
00001DEB  8BD8              mov bx,ax
00001DED  8B876808          mov ax,[bx+0x868]
00001DF1  050800            add ax,0x8
00001DF4  50                push ax
00001DF5  9A870E681C        call 0x1c68:0xe87
00001DFA  83C406            add sp,byte +0x6
00001DFD  5D                pop bp
00001DFE  CB                retf

; ---- 0EDE:1DFF ----
00001DFF  55                push bp
00001E00  8BEC              mov bp,sp
00001E02  83EC04            sub sp,byte +0x4
00001E05  56                push si
00001E06  57                push di
00001E07  8B7606            mov si,[bp+0x6]
00001E0A  A0AA37            mov al,[0x37aa]
00001E0D  98                cbw
00001E0E  D1E0              shl ax,1
00001E10  8BD8              mov bx,ax
00001E12  8B876808          mov ax,[bx+0x868]
00001E16  50                push ax
00001E17  8BC6              mov ax,si
00001E19  BB0200            mov bx,0x2
00001E1C  99                cwd
00001E1D  F7FB              idiv bx
00001E1F  BA0C00            mov dx,0xc
00001E22  F7EA              imul dx
00001E24  5A                pop dx
00001E25  03D0              add dx,ax
00001E27  83C20C            add dx,byte +0xc
00001E2A  8956FE            mov [bp-0x2],dx
00001E2D  A0AA37            mov al,[0x37aa]
00001E30  98                cbw
00001E31  D1E0              shl ax,1
00001E33  8BD8              mov bx,ax
00001E35  8B877008          mov ax,[bx+0x870]
00001E39  50                push ax
00001E3A  8BC6              mov ax,si
00001E3C  BB0200            mov bx,0x2
00001E3F  99                cwd
00001E40  F7FB              idiv bx
00001E42  D1E2              shl dx,1
00001E44  D1E2              shl dx,1
00001E46  58                pop ax
00001E47  03C2              add ax,dx
00001E49  054E00            add ax,0x4e
00001E4C  8946FC            mov [bp-0x4],ax
00001E4F  A0AA37            mov al,[0x37aa]
00001E52  98                cbw
00001E53  BA0600            mov dx,0x6
00001E56  F7EA              imul dx
00001E58  8BD8              mov bx,ax
00001E5A  8A80CE43          mov al,[bx+si+0x43ce]
00001E5E  B400              mov ah,0x0
00001E60  BB0A00            mov bx,0xa
00001E63  99                cwd
00001E64  F7FB              idiv bx
00001E66  D1E0              shl ax,1
00001E68  8BF8              mov di,ax
00001E6A  B80100            mov ax,0x1
00001E6D  50                push ax
00001E6E  B80300            mov ax,0x3
00001E71  50                push ax
00001E72  B80C00            mov ax,0xc
00001E75  50                push ax
00001E76  FF76FC            push word [bp-0x4]
00001E79  FF76FE            push word [bp-0x2]
00001E7C  9AD704960A        call 0xa96:0x4d7
00001E81  83C40A            add sp,byte +0xa
00001E84  0BFF              or di,di
00001E86  7E1A              jng 0x1ea2
00001E88  8A846C0E          mov al,[si+0xe6c]
00001E8C  B400              mov ah,0x0
00001E8E  50                push ax
00001E8F  B80300            mov ax,0x3
00001E92  50                push ax
00001E93  57                push di
00001E94  FF76FC            push word [bp-0x4]
00001E97  FF76FE            push word [bp-0x2]
00001E9A  9AD704960A        call 0xa96:0x4d7
00001E9F  83C40A            add sp,byte +0xa
00001EA2  5F                pop di
00001EA3  5E                pop si
00001EA4  8BE5              mov sp,bp
00001EA6  5D                pop bp
00001EA7  CB                retf

; ---- 0EDE:1EA8 ----
00001EA8  55                push bp
00001EA9  8BEC              mov bp,sp
00001EAB  807E0600          cmp byte [bp+0x6],0x0
00001EAF  7547              jnz 0x1ef8
00001EB1  B80100            mov ax,0x1
00001EB4  50                push ax
00001EB5  B80800            mov ax,0x8
00001EB8  50                push ax
00001EB9  B80300            mov ax,0x3
00001EBC  50                push ax
00001EBD  A0AA37            mov al,[0x37aa]
00001EC0  98                cbw
00001EC1  D1E0              shl ax,1
00001EC3  D1E0              shl ax,1
00001EC5  33D2              xor dx,dx
00001EC7  52                push dx
00001EC8  8BD8              mov bx,ax
00001ECA  FFB76C37          push word [bx+0x376c]
00001ECE  A0AA37            mov al,[0x37aa]
00001ED1  98                cbw
00001ED2  D1E0              shl ax,1
00001ED4  8BD8              mov bx,ax
00001ED6  8B877008          mov ax,[bx+0x870]
00001EDA  055A00            add ax,0x5a
00001EDD  50                push ax
00001EDE  A0AA37            mov al,[0x37aa]
00001EE1  98                cbw
00001EE2  D1E0              shl ax,1
00001EE4  8BD8              mov bx,ax
00001EE6  8B876808          mov ax,[bx+0x868]
00001EEA  050C00            add ax,0xc
00001EED  50                push ax
00001EEE  9A7A05960A        call 0xa96:0x57a
00001EF3  83C40E            add sp,byte +0xe
00001EF6  EB45              jmp short 0x1f3d
00001EF8  B80100            mov ax,0x1
00001EFB  50                push ax
00001EFC  B80B00            mov ax,0xb
00001EFF  50                push ax
00001F00  B80100            mov ax,0x1
00001F03  50                push ax
00001F04  A0AA37            mov al,[0x37aa]
00001F07  98                cbw
00001F08  D1E0              shl ax,1
00001F0A  D1E0              shl ax,1
00001F0C  33D2              xor dx,dx
00001F0E  52                push dx
00001F0F  8BD8              mov bx,ax
00001F11  FFB76E37          push word [bx+0x376e]
00001F15  A0AA37            mov al,[0x37aa]
00001F18  98                cbw
00001F19  D1E0              shl ax,1
00001F1B  8BD8              mov bx,ax
00001F1D  8B877008          mov ax,[bx+0x870]
00001F21  055A00            add ax,0x5a
00001F24  50                push ax
00001F25  A0AA37            mov al,[0x37aa]
00001F28  98                cbw
00001F29  D1E0              shl ax,1
00001F2B  8BD8              mov bx,ax
00001F2D  8B876808          mov ax,[bx+0x868]
00001F31  052500            add ax,0x25
00001F34  50                push ax
00001F35  9A7A05960A        call 0xa96:0x57a
00001F3A  83C40E            add sp,byte +0xe
00001F3D  5D                pop bp
00001F3E  CB                retf

; ---- 0EDE:1F3F ----
00001F3F  55                push bp
00001F40  8BEC              mov bp,sp
00001F42  B80100            mov ax,0x1
00001F45  50                push ax
00001F46  B80200            mov ax,0x2
00001F49  50                push ax
00001F4A  B80200            mov ax,0x2
00001F4D  50                push ax
00001F4E  A0AA37            mov al,[0x37aa]
00001F51  98                cbw
00001F52  D1E0              shl ax,1
00001F54  8BD8              mov bx,ax
00001F56  8B877008          mov ax,[bx+0x870]
00001F5A  055600            add ax,0x56
00001F5D  50                push ax
00001F5E  A0AA37            mov al,[0x37aa]
00001F61  98                cbw
00001F62  D1E0              shl ax,1
00001F64  8BD8              mov bx,ax
00001F66  8B876808          mov ax,[bx+0x868]
00001F6A  050900            add ax,0x9
00001F6D  50                push ax
00001F6E  9AD704960A        call 0xa96:0x4d7
00001F73  83C40A            add sp,byte +0xa
00001F76  5D                pop bp
00001F77  CB                retf

; ---- 0EDE:1F78 ----
00001F78  55                push bp
00001F79  8BEC              mov bp,sp
00001F7B  A0AA37            mov al,[0x37aa]
00001F7E  98                cbw
00001F7F  D1E0              shl ax,1
00001F81  8BD8              mov bx,ax
00001F83  8B876437          mov ax,[bx+0x3764]
00001F87  BB0A00            mov bx,0xa
00001F8A  99                cwd
00001F8B  F7FB              idiv bx
00001F8D  50                push ax
00001F8E  A0AA37            mov al,[0x37aa]
00001F91  98                cbw
00001F92  D1E0              shl ax,1
00001F94  8BD8              mov bx,ax
00001F96  8B875C37          mov ax,[bx+0x375c]
00001F9A  BB0A00            mov bx,0xa
00001F9D  99                cwd
00001F9E  F7FB              idiv bx
00001FA0  50                push ax
00001FA1  B80800            mov ax,0x8
00001FA4  50                push ax
00001FA5  B81000            mov ax,0x10
00001FA8  50                push ax
00001FA9  B83F00            mov ax,0x3f
00001FAC  50                push ax
00001FAD  B85000            mov ax,0x50
00001FB0  50                push ax
00001FB1  B81800            mov ax,0x18
00001FB4  50                push ax
00001FB5  B81000            mov ax,0x10
00001FB8  50                push ax
00001FB9  9A3B01960A        call 0xa96:0x13b
00001FBE  83C410            add sp,byte +0x10
00001FC1  5D                pop bp
00001FC2  CB                retf

; ---- 0EDE:1FC3 ----
00001FC3  55                push bp
00001FC4  8BEC              mov bp,sp
00001FC6  83EC06            sub sp,byte +0x6
00001FC9  56                push si
00001FCA  57                push di
00001FCB  A0AA37            mov al,[0x37aa]
00001FCE  98                cbw
00001FCF  D1E0              shl ax,1
00001FD1  8BD8              mov bx,ax
00001FD3  8B875C37          mov ax,[bx+0x375c]
00001FD7  BB0A00            mov bx,0xa
00001FDA  99                cwd
00001FDB  F7FB              idiv bx
00001FDD  40                inc ax
00001FDE  8946FE            mov [bp-0x2],ax
00001FE1  A0AA37            mov al,[0x37aa]
00001FE4  98                cbw
00001FE5  D1E0              shl ax,1
00001FE7  8BD8              mov bx,ax
00001FE9  8B876437          mov ax,[bx+0x3764]
00001FED  BB0A00            mov bx,0xa
00001FF0  99                cwd
00001FF1  F7FB              idiv bx
00001FF3  8946FC            mov [bp-0x4],ax
00001FF6  A0AA37            mov al,[0x37aa]
00001FF9  98                cbw
00001FFA  D1E0              shl ax,1
00001FFC  8BD8              mov bx,ax
00001FFE  8BB7E643          mov si,[bx+0x43e6]
00002002  B80300            mov ax,0x3
00002005  50                push ax
00002006  9ABA01FD21        call 0x21fd:0x1ba
0000200B  59                pop cx
0000200C  0BC0              or ax,ax
0000200E  7444              jz 0x2054
00002010  8A845E07          mov al,[si+0x75e]
00002014  B400              mov ah,0x0
00002016  BB1000            mov bx,0x10
00002019  99                cwd
0000201A  F7FB              idiv bx
0000201C  8B56FE            mov dx,[bp-0x2]
0000201F  03D0              add dx,ax
00002021  8956FA            mov [bp-0x6],dx
00002024  8A845E07          mov al,[si+0x75e]
00002028  B400              mov ah,0x0
0000202A  250F00            and ax,0xf
0000202D  8B56FC            mov dx,[bp-0x4]
00002030  03D0              add dx,ax
00002032  8BFA              mov di,dx
00002034  57                push di
00002035  FF76FA            push word [bp-0x6]
00002038  9ADE06CD23        call 0x23cd:0x6de
0000203D  59                pop cx
0000203E  59                pop cx
0000203F  3D0600            cmp ax,0x6
00002042  7510              jnz 0x2054
00002044  B80E00            mov ax,0xe
00002047  50                push ax
00002048  57                push di
00002049  FF76FA            push word [bp-0x6]
0000204C  9AFB03960A        call 0xa96:0x3fb
00002051  83C406            add sp,byte +0x6
00002054  B80300            mov ax,0x3
00002057  50                push ax
00002058  9ABA01FD21        call 0x21fd:0x1ba
0000205D  59                pop cx
0000205E  0BC0              or ax,ax
00002060  7444              jz 0x20a6
00002062  8A846E07          mov al,[si+0x76e]
00002066  B400              mov ah,0x0
00002068  BB1000            mov bx,0x10
0000206B  99                cwd
0000206C  F7FB              idiv bx
0000206E  8B56FE            mov dx,[bp-0x2]
00002071  03D0              add dx,ax
00002073  8956FA            mov [bp-0x6],dx
00002076  8A846E07          mov al,[si+0x76e]
0000207A  B400              mov ah,0x0
0000207C  250F00            and ax,0xf
0000207F  8B56FC            mov dx,[bp-0x4]
00002082  03D0              add dx,ax
00002084  8BFA              mov di,dx
00002086  57                push di
00002087  FF76FA            push word [bp-0x6]
0000208A  9ADE06CD23        call 0x23cd:0x6de
0000208F  59                pop cx
00002090  59                pop cx
00002091  3D0600            cmp ax,0x6
00002094  7510              jnz 0x20a6
00002096  B80E00            mov ax,0xe
00002099  50                push ax
0000209A  57                push di
0000209B  FF76FA            push word [bp-0x6]
0000209E  9AFB03960A        call 0xa96:0x3fb
000020A3  83C406            add sp,byte +0x6
000020A6  5F                pop di
000020A7  5E                pop si
000020A8  8BE5              mov sp,bp
000020AA  5D                pop bp
000020AB  CB                retf

; ---- 0EDE:20AC ----
000020AC  55                push bp
000020AD  8BEC              mov bp,sp
000020AF  56                push si
000020B0  C606AA3700        mov byte [0x37aa],0x0
000020B5  E99A01            jmp 0x2252
000020B8  A0AA37            mov al,[0x37aa]
000020BB  98                cbw
000020BC  BA1A00            mov dx,0x1a
000020BF  F7EA              imul dx
000020C1  8BD8              mov bx,ax
000020C3  C78776330900      mov word [bx+0x3376],0x9
000020C9  B8D900            mov ax,0xd9
000020CC  50                push ax
000020CD  A0AA37            mov al,[0x37aa]
000020D0  98                cbw
000020D1  B105              mov cl,0x5
000020D3  D3E0              shl ax,cl
000020D5  50                push ax
000020D6  B8D000            mov ax,0xd0
000020D9  50                push ax
000020DA  A0AA37            mov al,[0x37aa]
000020DD  98                cbw
000020DE  B105              mov cl,0x5
000020E0  D3E0              shl ax,cl
000020E2  50                push ax
000020E3  A0AA37            mov al,[0x37aa]
000020E6  98                cbw
000020E7  050400            add ax,0x4
000020EA  50                push ax
000020EB  9A2B02AF21        call 0x21af:0x22b
000020F0  83C40A            add sp,byte +0xa
000020F3  33F6              xor si,si
000020F5  EB34              jmp short 0x212b
000020F7  B85F00            mov ax,0x5f
000020FA  50                push ax
000020FB  8BC6              mov ax,si
000020FD  B104              mov cl,0x4
000020FF  D3E0              shl ax,cl
00002101  50                push ax
00002102  A0AA37            mov al,[0x37aa]
00002105  98                cbw
00002106  BA0900            mov dx,0x9
00002109  F7EA              imul dx
0000210B  057C00            add ax,0x7c
0000210E  50                push ax
0000210F  8BC6              mov ax,si
00002111  B104              mov cl,0x4
00002113  D3E0              shl ax,cl
00002115  054000            add ax,0x40
00002118  50                push ax
00002119  56                push si
0000211A  A0AA37            mov al,[0x37aa]
0000211D  98                cbw
0000211E  050400            add ax,0x4
00002121  50                push ax
00002122  9ACA00AF21        call 0x21af:0xca
00002127  83C40C            add sp,byte +0xc
0000212A  46                inc si
0000212B  83FE10            cmp si,byte +0x10
0000212E  7CC7              jl 0x20f7
00002130  33F6              xor si,si
00002132  EB2F              jmp short 0x2163
00002134  B83F00            mov ax,0x3f
00002137  50                push ax
00002138  8BC6              mov ax,si
0000213A  B104              mov cl,0x4
0000213C  D3E0              shl ax,cl
0000213E  50                push ax
0000213F  33C0              xor ax,ax
00002141  50                push ax
00002142  8BC6              mov ax,si
00002144  B104              mov cl,0x4
00002146  D3E0              shl ax,cl
00002148  059000            add ax,0x90
0000214B  50                push ax
0000214C  8BC6              mov ax,si
0000214E  051000            add ax,0x10
00002151  50                push ax
00002152  A0AA37            mov al,[0x37aa]
00002155  98                cbw
00002156  050400            add ax,0x4
00002159  50                push ax
0000215A  9ACA00AF21        call 0x21af:0xca
0000215F  83C40C            add sp,byte +0xc
00002162  46                inc si
00002163  83FE03            cmp si,byte +0x3
00002166  7CCC              jl 0x2134
00002168  33F6              xor si,si
0000216A  EB3C              jmp short 0x21a8
0000216C  B85F00            mov ax,0x5f
0000216F  50                push ax
00002170  8BC6              mov ax,si
00002172  B104              mov cl,0x4
00002174  D3E0              shl ax,cl
00002176  050001            add ax,0x100
00002179  50                push ax
0000217A  8BC6              mov ax,si
0000217C  BA0900            mov dx,0x9
0000217F  F7EA              imul dx
00002181  051D00            add ax,0x1d
00002184  50                push ax
00002185  A0AA37            mov al,[0x37aa]
00002188  98                cbw
00002189  B104              mov cl,0x4
0000218B  D3E0              shl ax,cl
0000218D  058000            add ax,0x80
00002190  50                push ax
00002191  8BC6              mov ax,si
00002193  051300            add ax,0x13
00002196  50                push ax
00002197  A0AA37            mov al,[0x37aa]
0000219A  98                cbw
0000219B  050400            add ax,0x4
0000219E  50                push ax
0000219F  9ACA00AF21        call 0x21af:0xca
000021A4  83C40C            add sp,byte +0xc
000021A7  46                inc si
000021A8  83FE02            cmp si,byte +0x2
000021AB  7CBF              jl 0x216c
000021AD  A0AA37            mov al,[0x37aa]
000021B0  98                cbw
000021B1  BA1A00            mov dx,0x1a
000021B4  F7EA              imul dx
000021B6  8BD8              mov bx,ax
000021B8  C7870E330900      mov word [bx+0x330e],0x9
000021BE  B8EB00            mov ax,0xeb
000021C1  50                push ax
000021C2  A0AA37            mov al,[0x37aa]
000021C5  98                cbw
000021C6  B105              mov cl,0x5
000021C8  D3E0              shl ax,cl
000021CA  50                push ax
000021CB  B8E200            mov ax,0xe2
000021CE  50                push ax
000021CF  A0AA37            mov al,[0x37aa]
000021D2  98                cbw
000021D3  B105              mov cl,0x5
000021D5  D3E0              shl ax,cl
000021D7  50                push ax
000021D8  A0AA37            mov al,[0x37aa]
000021DB  98                cbw
000021DC  50                push ax
000021DD  9A2B02AF21        call 0x21af:0x22b
000021E2  83C40A            add sp,byte +0xa
000021E5  33F6              xor si,si
000021E7  EB28              jmp short 0x2211
000021E9  B84800            mov ax,0x48
000021EC  50                push ax
000021ED  8BC6              mov ax,si
000021EF  B104              mov cl,0x4
000021F1  D3E0              shl ax,cl
000021F3  50                push ax
000021F4  B80900            mov ax,0x9
000021F7  50                push ax
000021F8  8BC6              mov ax,si
000021FA  B104              mov cl,0x4
000021FC  D3E0              shl ax,cl
000021FE  059000            add ax,0x90
00002201  50                push ax
00002202  56                push si
00002203  A0AA37            mov al,[0x37aa]
00002206  98                cbw
00002207  50                push ax
00002208  9ACA00AF21        call 0x21af:0xca
0000220D  83C40C            add sp,byte +0xc
00002210  46                inc si
00002211  83FE04            cmp si,byte +0x4
00002214  7CD3              jl 0x21e9
00002216  33F6              xor si,si
00002218  EB2F              jmp short 0x2249
0000221A  B83F00            mov ax,0x3f
0000221D  50                push ax
0000221E  8BC6              mov ax,si
00002220  B104              mov cl,0x4
00002222  D3E0              shl ax,cl
00002224  053000            add ax,0x30
00002227  50                push ax
00002228  33C0              xor ax,ax
0000222A  50                push ax
0000222B  8BC6              mov ax,si
0000222D  B104              mov cl,0x4
0000222F  D3E0              shl ax,cl
00002231  05C000            add ax,0xc0
00002234  50                push ax
00002235  8BC6              mov ax,si
00002237  050400            add ax,0x4
0000223A  50                push ax
0000223B  A0AA37            mov al,[0x37aa]
0000223E  98                cbw
0000223F  50                push ax
00002240  9ACA00AF21        call 0x21af:0xca
00002245  83C40C            add sp,byte +0xc
00002248  46                inc si
00002249  83FE02            cmp si,byte +0x2
0000224C  7CCC              jl 0x221a
0000224E  FE06AA37          inc byte [0x37aa]
00002252  803EAA3704        cmp byte [0x37aa],0x4
00002257  7D03              jnl 0x225c
00002259  E95CFE            jmp 0x20b8
0000225C  5E                pop si
0000225D  5D                pop bp
0000225E  CB                retf

; ---- 0EDE:225F ----
0000225F  55                push bp
00002260  8BEC              mov bp,sp
00002262  9A8422DE0E        call 0xede:0x2284
00002267  A0AA37            mov al,[0x37aa]
0000226A  98                cbw
0000226B  D1E0              shl ax,1
0000226D  8BD8              mov bx,ax
0000226F  FFB7E643          push word [bx+0x43e6]
00002273  A0AA37            mov al,[0x37aa]
00002276  98                cbw
00002277  050400            add ax,0x4
0000227A  50                push ax
0000227B  9A1204AF21        call 0x21af:0x412
00002280  59                pop cx
00002281  59                pop cx
00002282  5D                pop bp
00002283  CB                retf

; ---- 0EDE:2284 ----
00002284  55                push bp
00002285  8BEC              mov bp,sp
00002287  A0AA37            mov al,[0x37aa]
0000228A  98                cbw
0000228B  D1E0              shl ax,1
0000228D  8BD8              mov bx,ax
0000228F  8B875C37          mov ax,[bx+0x375c]
00002293  BB0A00            mov bx,0xa
00002296  99                cwd
00002297  F7FB              idiv bx
00002299  50                push ax
0000229A  A0AA37            mov al,[0x37aa]
0000229D  98                cbw
0000229E  BA1A00            mov dx,0x1a
000022A1  F7EA              imul dx
000022A3  8BD8              mov bx,ax
000022A5  58                pop ax
000022A6  89878833          mov [bx+0x3388],ax
000022AA  A0AA37            mov al,[0x37aa]
000022AD  98                cbw
000022AE  D1E0              shl ax,1
000022B0  8BD8              mov bx,ax
000022B2  8B876437          mov ax,[bx+0x3764]
000022B6  BB0A00            mov bx,0xa
000022B9  99                cwd
000022BA  F7FB              idiv bx
000022BC  50                push ax
000022BD  A0AA37            mov al,[0x37aa]
000022C0  98                cbw
000022C1  BA1A00            mov dx,0x1a
000022C4  F7EA              imul dx
000022C6  8BD8              mov bx,ax
000022C8  58                pop ax
000022C9  89878A33          mov [bx+0x338a],ax
000022CD  5D                pop bp
000022CE  CB                retf

; ---- 0EDE:22CF ----
000022CF  55                push bp
000022D0  8BEC              mov bp,sp
000022D2  56                push si
000022D3  A0AA37            mov al,[0x37aa]
000022D6  98                cbw
000022D7  8BD8              mov bx,ax
000022D9  80BF823700        cmp byte [bx+0x3782],0x0
000022DE  7415              jz 0x22f5
000022E0  A0AA37            mov al,[0x37aa]
000022E3  98                cbw
000022E4  8BD8              mov bx,ax
000022E6  8A878237          mov al,[bx+0x3782]
000022EA  B400              mov ah,0x0
000022EC  BA1300            mov dx,0x13
000022EF  2BD0              sub dx,ax
000022F1  8BF2              mov si,dx
000022F3  EB2C              jmp short 0x2321
000022F5  A0AA37            mov al,[0x37aa]
000022F8  98                cbw
000022F9  8BD8              mov bx,ax
000022FB  80BF1E2C00        cmp byte [bx+0x2c1e],0x0
00002300  7413              jz 0x2315
00002302  A0AA37            mov al,[0x37aa]
00002305  98                cbw
00002306  8BD8              mov bx,ax
00002308  8A871E2C          mov al,[bx+0x2c1e]
0000230C  B400              mov ah,0x0
0000230E  051200            add ax,0x12
00002311  8BF0              mov si,ax
00002313  EB0C              jmp short 0x2321
00002315  A0AA37            mov al,[0x37aa]
00002318  98                cbw
00002319  D1E0              shl ax,1
0000231B  8BD8              mov bx,ax
0000231D  8BB7E643          mov si,[bx+0x43e6]
00002321  A0AA37            mov al,[0x37aa]
00002324  98                cbw
00002325  BA1A00            mov dx,0x1a
00002328  F7EA              imul dx
0000232A  8BD8              mov bx,ax
0000232C  89B78C33          mov [bx+0x338c],si
00002330  5E                pop si
00002331  5D                pop bp
00002332  CB                retf

; ---- 0EDE:2333 ----
00002333  55                push bp
00002334  8BEC              mov bp,sp
00002336  A0AA37            mov al,[0x37aa]
00002339  98                cbw
0000233A  050400            add ax,0x4
0000233D  50                push ax
0000233E  9A5E04AF21        call 0x21af:0x45e
00002343  59                pop cx
00002344  5D                pop bp
00002345  CB                retf

; ---- 0EDE:2346 ----
00002346  55                push bp
00002347  8BEC              mov bp,sp
00002349  9A5F23DE0E        call 0xede:0x235f
0000234E  33C0              xor ax,ax
00002350  50                push ax
00002351  A0AA37            mov al,[0x37aa]
00002354  98                cbw
00002355  50                push ax
00002356  9A1204AF21        call 0x21af:0x412
0000235B  59                pop cx
0000235C  59                pop cx
0000235D  5D                pop bp
0000235E  CB                retf

; ---- 0EDE:235F ----
0000235F  55                push bp
00002360  8BEC              mov bp,sp
00002362  A0AA37            mov al,[0x37aa]
00002365  98                cbw
00002366  D1E0              shl ax,1
00002368  8BD8              mov bx,ax
0000236A  8B87E244          mov ax,[bx+0x44e2]
0000236E  BB0A00            mov bx,0xa
00002371  99                cwd
00002372  F7FB              idiv bx
00002374  50                push ax
00002375  A0AA37            mov al,[0x37aa]
00002378  98                cbw
00002379  BA1A00            mov dx,0x1a
0000237C  F7EA              imul dx
0000237E  8BD8              mov bx,ax
00002380  58                pop ax
00002381  89872033          mov [bx+0x3320],ax
00002385  A0AA37            mov al,[0x37aa]
00002388  98                cbw
00002389  D1E0              shl ax,1
0000238B  8BD8              mov bx,ax
0000238D  8B87EA44          mov ax,[bx+0x44ea]
00002391  BB0A00            mov bx,0xa
00002394  99                cwd
00002395  F7FB              idiv bx
00002397  50                push ax
00002398  A0AA37            mov al,[0x37aa]
0000239B  98                cbw
0000239C  BA1A00            mov dx,0x1a
0000239F  F7EA              imul dx
000023A1  8BD8              mov bx,ax
000023A3  58                pop ax
000023A4  89872233          mov [bx+0x3322],ax
000023A8  5D                pop bp
000023A9  CB                retf

; ---- 0EDE:23AA ----
000023AA  55                push bp
000023AB  8BEC              mov bp,sp
000023AD  A0AA37            mov al,[0x37aa]
000023B0  98                cbw
000023B1  50                push ax
000023B2  9A5E04AF21        call 0x21af:0x45e
000023B7  59                pop cx
000023B8  5D                pop bp
000023B9  CB                retf

; ---- 0EDE:23BA ----
000023BA  55                push bp
000023BB  8BEC              mov bp,sp
000023BD  803EF44000        cmp byte [0x40f4],0x0
000023C2  7517              jnz 0x23db
000023C4  803E7E0701        cmp byte [0x77e],0x1
000023C9  750E              jnz 0x23d9
000023CB  33C0              xor ax,ax
000023CD  50                push ax
000023CE  B80100            mov ax,0x1
000023D1  50                push ax
000023D2  9A5400A721        call 0x21a7:0x54
000023D7  59                pop cx
000023D8  59                pop cx
000023D9  EB16              jmp short 0x23f1
000023DB  803E7E0700        cmp byte [0x77e],0x0
000023E0  750F              jnz 0x23f1
000023E2  B84700            mov ax,0x47
000023E5  50                push ax
000023E6  B80100            mov ax,0x1
000023E9  50                push ax
000023EA  9A5400A721        call 0x21a7:0x54
000023EF  59                pop cx
