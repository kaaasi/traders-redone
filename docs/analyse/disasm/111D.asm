; segment 111D  file offset 0x147D0  size 0x860
00000000  59                pop cx
00000001  A0F440            mov al,[0x40f4]
00000004  A27E07            mov [0x77e],al
00000007  5D                pop bp
00000008  CB                retf

; ---- 111D:0009 ----
00000009  55                push bp
0000000A  8BEC              mov bp,sp
0000000C  56                push si
0000000D  833E9A4B02        cmp word [0x4b9a],byte +0x2
00000012  7E07              jng 0x1b
00000014  803E302F00        cmp byte [0x2f30],0x0
00000019  7503              jnz 0x1e
0000001B  E9F100            jmp 0x10f
0000001E  B80A00            mov ax,0xa
00000021  50                push ax
00000022  9ABA01FD21        call 0x21fd:0x1ba
00000027  59                pop cx
00000028  0BC0              or ax,ax
0000002A  7410              jz 0x3c
0000002C  A19A4B            mov ax,[0x4b9a]
0000002F  BB0F00            mov bx,0xf
00000032  99                cwd
00000033  F7FB              idiv bx
00000035  0BD2              or dx,dx
00000037  7403              jz 0x3c
00000039  E9D300            jmp 0x10f
0000003C  B80200            mov ax,0x2
0000003F  50                push ax
00000040  9ABA01FD21        call 0x21fd:0x1ba
00000045  59                pop cx
00000046  8BF0              mov si,ax
00000048  9ACB062508        call 0x825:0x6cb
0000004D  FF36CE08          push word [0x8ce]
00000051  9A02006C18        call 0x186c:0x2
00000056  59                pop cx
00000057  B85005            mov ax,0x550
0000005A  50                push ax
0000005B  9A7D03FD21        call 0x21fd:0x37d
00000060  59                pop cx
00000061  9A1B00960A        call 0xa96:0x1b
00000066  9AD91E0906        call 0x609:0x1ed9
0000006B  B82C00            mov ax,0x2c
0000006E  50                push ax
0000006F  B8A000            mov ax,0xa0
00000072  50                push ax
00000073  B88800            mov ax,0x88
00000076  50                push ax
00000077  B88C00            mov ax,0x8c
0000007A  50                push ax
0000007B  9A56120906        call 0x609:0x1256
00000080  83C408            add sp,byte +0x8
00000083  B80100            mov ax,0x1
00000086  50                push ax
00000087  B8FFFF            mov ax,0xffff
0000008A  50                push ax
0000008B  B80B00            mov ax,0xb
0000008E  50                push ax
0000008F  B80500            mov ax,0x5
00000092  50                push ax
00000093  FF365E38          push word [0x385e]
00000097  FF365C38          push word [0x385c]
0000009B  B8A000            mov ax,0xa0
0000009E  50                push ax
0000009F  B8A000            mov ax,0xa0
000000A2  50                push ax
000000A3  B88C00            mov ax,0x8c
000000A6  50                push ax
000000A7  9AD300690A        call 0xa69:0xd3
000000AC  83C412            add sp,byte +0x12
000000AF  9A0D06FD21        call 0x21fd:0x60d
000000B4  1E                push ds
000000B5  B8A800            mov ax,0xa8
000000B8  50                push ax
000000B9  9A6C02FD21        call 0x21fd:0x26c
000000BE  59                pop cx
000000BF  59                pop cx
000000C0  9A2903FD21        call 0x21fd:0x329
000000C5  FF36D208          push word [0x8d2]
000000C9  9A02006C18        call 0x186c:0x2
000000CE  59                pop cx
000000CF  B85005            mov ax,0x550
000000D2  50                push ax
000000D3  9A7D03FD21        call 0x21fd:0x37d
000000D8  59                pop cx
000000D9  9A12011D11        call 0x111d:0x112
000000DE  8BC6              mov ax,si
000000E0  0BC0              or ax,ax
000000E2  740C              jz 0xf0
000000E4  3D0100            cmp ax,0x1
000000E7  740E              jz 0xf7
000000E9  3D0200            cmp ax,0x2
000000EC  7410              jz 0xfe
000000EE  EB15              jmp short 0x105
000000F0  9A64021D11        call 0x111d:0x264
000000F5  EB0E              jmp short 0x105
000000F7  9AB7031D11        call 0x111d:0x3b7
000000FC  EB07              jmp short 0x105
000000FE  9A19051D11        call 0x111d:0x519
00000103  EB00              jmp short 0x105
00000105  B80300            mov ax,0x3
00000108  50                push ax
00000109  9A0400FD21        call 0x21fd:0x4
0000010E  59                pop cx
0000010F  5E                pop si
00000110  5D                pop bp
00000111  CB                retf

; ---- 111D:0112 ----
00000112  55                push bp
00000113  8BEC              mov bp,sp
00000115  81EC6A01          sub sp,0x16a
00000119  56                push si
0000011A  57                push di
0000011B  9A1B00960A        call 0xa96:0x1b
00000120  33C0              xor ax,ax
00000122  50                push ax
00000123  33C0              xor ax,ax
00000125  50                push ax
00000126  B8C800            mov ax,0xc8
00000129  50                push ax
0000012A  B84001            mov ax,0x140
0000012D  50                push ax
0000012E  33C0              xor ax,ax
00000130  50                push ax
00000131  33C0              xor ax,ax
00000133  50                push ax
00000134  9A3000960A        call 0xa96:0x30
00000139  83C40C            add sp,byte +0xc
0000013C  9AA806960A        call 0xa96:0x6a8
00000141  9A1B00960A        call 0xa96:0x1b
00000146  33F6              xor si,si
00000148  EB65              jmp short 0x1af
0000014A  B83F01            mov ax,0x13f
0000014D  50                push ax
0000014E  9ABA01FD21        call 0x21fd:0x1ba
00000153  59                pop cx
00000154  8946FE            mov [bp-0x2],ax
00000157  B89600            mov ax,0x96
0000015A  50                push ax
0000015B  9ABA01FD21        call 0x21fd:0x1ba
00000160  59                pop cx
00000161  8BF8              mov di,ax
00000163  8BDE              mov bx,si
00000165  D1E3              shl bx,1
00000167  D1E3              shl bx,1
00000169  8D8696FE          lea ax,[bp-0x16a]
0000016D  03D8              add bx,ax
0000016F  8B46FE            mov ax,[bp-0x2]
00000172  368907            mov [ss:bx],ax
00000175  8BDE              mov bx,si
00000177  D1E3              shl bx,1
00000179  D1E3              shl bx,1
0000017B  8D8698FE          lea ax,[bp-0x168]
0000017F  03D8              add bx,ax
00000181  36893F            mov [ss:bx],di
00000184  57                push di
00000185  FF76FE            push word [bp-0x2]
00000188  9ADE06CD23        call 0x23cd:0x6de
0000018D  59                pop cx
0000018E  59                pop cx
0000018F  3D0100            cmp ax,0x1
00000192  751A              jnz 0x1ae
00000194  B80200            mov ax,0x2
00000197  50                push ax
00000198  9ABA01FD21        call 0x21fd:0x1ba
0000019D  59                pop cx
0000019E  050800            add ax,0x8
000001A1  50                push ax
000001A2  57                push di
000001A3  FF76FE            push word [bp-0x2]
000001A6  9AFB03960A        call 0xa96:0x3fb
000001AB  83C406            add sp,byte +0x6
000001AE  46                inc si
000001AF  83FE5A            cmp si,byte +0x5a
000001B2  7C96              jl 0x14a
000001B4  9AA806960A        call 0xa96:0x6a8
000001B9  9A1F05FD21        call 0x21fd:0x51f
000001BE  9A1B00960A        call 0xa96:0x1b
000001C3  B80300            mov ax,0x3
000001C6  50                push ax
000001C7  33C0              xor ax,ax
000001C9  50                push ax
000001CA  B8C500            mov ax,0xc5
000001CD  50                push ax
000001CE  B84001            mov ax,0x140
000001D1  50                push ax
000001D2  33C0              xor ax,ax
000001D4  50                push ax
000001D5  33C0              xor ax,ax
000001D7  50                push ax
000001D8  9A3000960A        call 0xa96:0x30
000001DD  83C40C            add sp,byte +0xc
000001E0  B80100            mov ax,0x1
000001E3  50                push ax
000001E4  B80300            mov ax,0x3
000001E7  50                push ax
000001E8  B84001            mov ax,0x140
000001EB  50                push ax
000001EC  33C0              xor ax,ax
000001EE  50                push ax
000001EF  33C0              xor ax,ax
000001F1  50                push ax
000001F2  9AD704960A        call 0xa96:0x4d7
000001F7  83C40A            add sp,byte +0xa
000001FA  33F6              xor si,si
000001FC  EB56              jmp short 0x254
000001FE  8BDE              mov bx,si
00000200  D1E3              shl bx,1
00000202  D1E3              shl bx,1
00000204  8D8696FE          lea ax,[bp-0x16a]
00000208  03D8              add bx,ax
0000020A  368B07            mov ax,[ss:bx]
0000020D  8946FE            mov [bp-0x2],ax
00000210  8BDE              mov bx,si
00000212  D1E3              shl bx,1
00000214  D1E3              shl bx,1
00000216  8D8698FE          lea ax,[bp-0x168]
0000021A  03D8              add bx,ax
0000021C  368B07            mov ax,[ss:bx]
0000021F  05FDFF            add ax,0xfffd
00000222  8BF8              mov di,ax
00000224  57                push di
00000225  FF76FE            push word [bp-0x2]
00000228  9ADE06CD23        call 0x23cd:0x6de
0000022D  59                pop cx
0000022E  59                pop cx
0000022F  3D0100            cmp ax,0x1
00000232  751F              jnz 0x253
00000234  B80200            mov ax,0x2
00000237  50                push ax
00000238  9ABA01FD21        call 0x21fd:0x1ba
0000023D  59                pop cx
0000023E  050800            add ax,0x8
00000241  50                push ax
00000242  8BC7              mov ax,di
00000244  050300            add ax,0x3
00000247  50                push ax
00000248  FF76FE            push word [bp-0x2]
0000024B  9AFB03960A        call 0xa96:0x3fb
00000250  83C406            add sp,byte +0x6
00000253  46                inc si
00000254  83FE5A            cmp si,byte +0x5a
00000257  7CA5              jl 0x1fe
00000259  9AA806960A        call 0xa96:0x6a8
0000025E  5F                pop di
0000025F  5E                pop si
00000260  8BE5              mov sp,bp
00000262  5D                pop bp
00000263  CB                retf

; ---- 111D:0264 ----
00000264  55                push bp
00000265  8BEC              mov bp,sp
00000267  56                push si
00000268  1E                push ds
00000269  B8A007            mov ax,0x7a0
0000026C  50                push ax
0000026D  9A6C02FD21        call 0x21fd:0x26c
00000272  59                pop cx
00000273  59                pop cx
00000274  9A2903FD21        call 0x21fd:0x329
00000279  33C0              xor ax,ax
0000027B  BA6400            mov dx,0x64
0000027E  50                push ax
0000027F  52                push dx
00000280  9A8501FD21        call 0x21fd:0x185
00000285  59                pop cx
00000286  59                pop cx
00000287  B84800            mov ax,0x48
0000028A  50                push ax
0000028B  33C0              xor ax,ax
0000028D  50                push ax
0000028E  9A5400A721        call 0x21a7:0x54
00000293  59                pop cx
00000294  59                pop cx
00000295  33F6              xor si,si
00000297  EB2C              jmp short 0x2c5
00000299  9A4502FD21        call 0x21fd:0x245
0000029E  9A1F05FD21        call 0x21fd:0x51f
000002A3  33C0              xor ax,ax
000002A5  BA0100            mov dx,0x1
000002A8  50                push ax
000002A9  52                push dx
000002AA  9A8501FD21        call 0x21fd:0x185
000002AF  59                pop cx
000002B0  59                pop cx
000002B1  9A1F05FD21        call 0x21fd:0x51f
000002B6  33C0              xor ax,ax
000002B8  BA0100            mov dx,0x1
000002BB  50                push ax
000002BC  52                push dx
000002BD  9A8501FD21        call 0x21fd:0x185
000002C2  59                pop cx
000002C3  59                pop cx
000002C4  46                inc si
000002C5  83FE50            cmp si,byte +0x50
000002C8  7CCF              jl 0x299
000002CA  33C0              xor ax,ax
000002CC  50                push ax
000002CD  33C0              xor ax,ax
000002CF  50                push ax
000002D0  9A5400A721        call 0x21a7:0x54
000002D5  59                pop cx
000002D6  59                pop cx
000002D7  B80200            mov ax,0x2
000002DA  50                push ax
000002DB  1E                push ds
000002DC  B85438            mov ax,0x3854
000002DF  50                push ax
000002E0  9A57071D11        call 0x111d:0x757
000002E5  83C406            add sp,byte +0x6
000002E8  9A6103310E        call 0xe31:0x361
000002ED  C606002F00        mov byte [0x2f00],0x0
000002F2  E9A100            jmp 0x396
000002F5  C606B02E00        mov byte [0x2eb0],0x0
000002FA  E98500            jmp 0x382
000002FD  A0B02E            mov al,[0x2eb0]
00000300  98                cbw
00000301  BA0600            mov dx,0x6
00000304  F7EA              imul dx
00000306  50                push ax
00000307  A0002F            mov al,[0x2f00]
0000030A  98                cbw
0000030B  5B                pop bx
0000030C  03D8              add bx,ax
0000030E  80BFB12E04        cmp byte [bx+0x2eb1],0x4
00000313  7518              jnz 0x32d
00000315  A0B02E            mov al,[0x2eb0]
00000318  98                cbw
00000319  BA0600            mov dx,0x6
0000031C  F7EA              imul dx
0000031E  50                push ax
0000031F  A0002F            mov al,[0x2f00]
00000322  98                cbw
00000323  5B                pop bx
00000324  03D8              add bx,ax
00000326  C687DA4503        mov byte [bx+0x45da],0x3
0000032B  EB51              jmp short 0x37e
0000032D  A0B02E            mov al,[0x2eb0]
00000330  98                cbw
00000331  BA0600            mov dx,0x6
00000334  F7EA              imul dx
00000336  50                push ax
00000337  A0002F            mov al,[0x2f00]
0000033A  98                cbw
0000033B  5B                pop bx
0000033C  03D8              add bx,ax
0000033E  80BFB12E03        cmp byte [bx+0x2eb1],0x3
00000343  7539              jnz 0x37e
00000345  A0B02E            mov al,[0x2eb0]
00000348  98                cbw
00000349  BA0600            mov dx,0x6
0000034C  F7EA              imul dx
0000034E  50                push ax
0000034F  A0002F            mov al,[0x2f00]
00000352  98                cbw
00000353  5B                pop bx
00000354  03D8              add bx,ax
00000356  80BFDA4503        cmp byte [bx+0x45da],0x3
0000035B  7521              jnz 0x37e
0000035D  B80200            mov ax,0x2
00000360  50                push ax
00000361  9ABA01FD21        call 0x21fd:0x1ba
00000366  59                pop cx
00000367  50                push ax
00000368  A0B02E            mov al,[0x2eb0]
0000036B  98                cbw
0000036C  BA0600            mov dx,0x6
0000036F  F7EA              imul dx
00000371  50                push ax
00000372  A0002F            mov al,[0x2f00]
00000375  98                cbw
00000376  5B                pop bx
00000377  03D8              add bx,ax
00000379  58                pop ax
0000037A  8887DA45          mov [bx+0x45da],al
0000037E  FE06B02E          inc byte [0x2eb0]
00000382  A0B02E            mov al,[0x2eb0]
00000385  98                cbw
00000386  8B16FC08          mov dx,[0x8fc]
0000038A  4A                dec dx
0000038B  3BC2              cmp ax,dx
0000038D  7F03              jg 0x392
0000038F  E96BFF            jmp 0x2fd
00000392  FE06002F          inc byte [0x2f00]
00000396  A0002F            mov al,[0x2f00]
00000399  98                cbw
0000039A  8B16FE08          mov dx,[0x8fe]
0000039E  4A                dec dx
0000039F  3BC2              cmp ax,dx
000003A1  7F03              jg 0x3a6
000003A3  E94FFF            jmp 0x2f5
000003A6  33C0              xor ax,ax
000003A8  BA6400            mov dx,0x64
000003AB  50                push ax
000003AC  52                push dx
000003AD  9A8501FD21        call 0x21fd:0x185
000003B2  59                pop cx
000003B3  59                pop cx
000003B4  5E                pop si
000003B5  5D                pop bp
000003B6  CB                retf

; ---- 111D:03B7 ----
000003B7  55                push bp
000003B8  8BEC              mov bp,sp
000003BA  4C                dec sp
000003BB  4C                dec sp
000003BC  56                push si
000003BD  57                push di
000003BE  1E                push ds
000003BF  B88007            mov ax,0x780
000003C2  50                push ax
000003C3  9A6C02FD21        call 0x21fd:0x26c
000003C8  59                pop cx
000003C9  59                pop cx
000003CA  9A2903FD21        call 0x21fd:0x329
000003CF  33C0              xor ax,ax
000003D1  BA6400            mov dx,0x64
000003D4  50                push ax
000003D5  52                push dx
000003D6  9A8501FD21        call 0x21fd:0x185
000003DB  59                pop cx
000003DC  59                pop cx
000003DD  33FF              xor di,di
000003DF  EB32              jmp short 0x413
000003E1  BE0200            mov si,0x2
000003E4  EB15              jmp short 0x3fb
000003E6  8BDE              mov bx,si
000003E8  D1E3              shl bx,1
000003EA  8B878007          mov ax,[bx+0x780]
000003EE  03C7              add ax,di
000003F0  40                inc ax
000003F1  50                push ax
000003F2  56                push si
000003F3  9A6D009522        call 0x2295:0x6d
000003F8  59                pop cx
000003F9  59                pop cx
000003FA  46                inc si
000003FB  B80700            mov ax,0x7
000003FE  2BC7              sub ax,di
00000400  3BC6              cmp ax,si
00000402  7DE2              jnl 0x3e6
00000404  33C0              xor ax,ax
00000406  BA0A00            mov dx,0xa
00000409  50                push ax
0000040A  52                push dx
0000040B  9A8501FD21        call 0x21fd:0x185
00000410  59                pop cx
00000411  59                pop cx
00000412  47                inc di
00000413  83FF05            cmp di,byte +0x5
00000416  7EC9              jng 0x3e1
00000418  B84900            mov ax,0x49
0000041B  50                push ax
0000041C  33C0              xor ax,ax
0000041E  50                push ax
0000041F  9A5400A721        call 0x21a7:0x54
00000424  59                pop cx
00000425  59                pop cx
00000426  33C0              xor ax,ax
00000428  BAFA00            mov dx,0xfa
0000042B  50                push ax
0000042C  52                push dx
0000042D  9A8501FD21        call 0x21fd:0x185
00000432  59                pop cx
00000433  59                pop cx
00000434  33C0              xor ax,ax
00000436  50                push ax
00000437  33C0              xor ax,ax
00000439  50                push ax
0000043A  9A5400A721        call 0x21a7:0x54
0000043F  59                pop cx
00000440  59                pop cx
00000441  BF0500            mov di,0x5
00000444  EB31              jmp short 0x477
00000446  BE0200            mov si,0x2
00000449  EB14              jmp short 0x45f
0000044B  8BDE              mov bx,si
0000044D  D1E3              shl bx,1
0000044F  8B878007          mov ax,[bx+0x780]
00000453  03C7              add ax,di
00000455  50                push ax
00000456  56                push si
00000457  9A6D009522        call 0x2295:0x6d
0000045C  59                pop cx
0000045D  59                pop cx
0000045E  46                inc si
0000045F  B80700            mov ax,0x7
00000462  2BC7              sub ax,di
00000464  3BC6              cmp ax,si
00000466  7DE3              jnl 0x44b
00000468  33C0              xor ax,ax
0000046A  BA0A00            mov dx,0xa
0000046D  50                push ax
0000046E  52                push dx
0000046F  9A8501FD21        call 0x21fd:0x185
00000474  59                pop cx
00000475  59                pop cx
00000476  4F                dec di
00000477  0BFF              or di,di
00000479  7DCB              jnl 0x446
0000047B  B80200            mov ax,0x2
0000047E  50                push ax
0000047F  1E                push ds
00000480  B84C38            mov ax,0x384c
00000483  50                push ax
00000484  9A57071D11        call 0x111d:0x757
00000489  83C406            add sp,byte +0x6
0000048C  B80F00            mov ax,0xf
0000048F  50                push ax
00000490  9ABA01FD21        call 0x21fd:0x1ba
00000495  59                pop cx
00000496  050A00            add ax,0xa
00000499  8946FE            mov [bp-0x2],ax
0000049C  33F6              xor si,si
0000049E  EB6B              jmp short 0x50b
000004A0  A1FC08            mov ax,[0x8fc]
000004A3  48                dec ax
000004A4  50                push ax
000004A5  9ABA01FD21        call 0x21fd:0x1ba
000004AA  59                pop cx
000004AB  A2B02E            mov [0x2eb0],al
000004AE  A1FE08            mov ax,[0x8fe]
000004B1  48                dec ax
000004B2  50                push ax
000004B3  9ABA01FD21        call 0x21fd:0x1ba
000004B8  59                pop cx
000004B9  A2002F            mov [0x2f00],al
000004BC  A0B02E            mov al,[0x2eb0]
000004BF  98                cbw
000004C0  BA0600            mov dx,0x6
000004C3  F7EA              imul dx
000004C5  50                push ax
000004C6  A0002F            mov al,[0x2f00]
000004C9  98                cbw
000004CA  5B                pop bx
000004CB  03D8              add bx,ax
000004CD  80BFB12E04        cmp byte [bx+0x2eb1],0x4
000004D2  7502              jnz 0x4d6
000004D4  EB34              jmp short 0x50a
000004D6  B80200            mov ax,0x2
000004D9  50                push ax
000004DA  9ABA01FD21        call 0x21fd:0x1ba
000004DF  59                pop cx
000004E0  50                push ax
000004E1  A0B02E            mov al,[0x2eb0]
000004E4  98                cbw
000004E5  BA0600            mov dx,0x6
000004E8  F7EA              imul dx
000004EA  50                push ax
000004EB  A0002F            mov al,[0x2f00]
000004EE  98                cbw
000004EF  5B                pop bx
000004F0  03D8              add bx,ax
000004F2  8A874244          mov al,[bx+0x4442]
000004F6  98                cbw
000004F7  50                push ax
000004F8  A0002F            mov al,[0x2f00]
000004FB  98                cbw
000004FC  50                push ax
000004FD  A0B02E            mov al,[0x2eb0]
00000500  98                cbw
00000501  50                push ax
00000502  9AD2100906        call 0x609:0x10d2
00000507  83C408            add sp,byte +0x8
0000050A  46                inc si
0000050B  8B46FE            mov ax,[bp-0x2]
0000050E  48                dec ax
0000050F  3BC6              cmp ax,si
00000511  7D8D              jnl 0x4a0
00000513  5F                pop di
00000514  5E                pop si
00000515  8BE5              mov sp,bp
00000517  5D                pop bp
00000518  CB                retf

; ---- 111D:0519 ----
00000519  55                push bp
0000051A  8BEC              mov bp,sp
0000051C  4C                dec sp
0000051D  4C                dec sp
0000051E  56                push si
0000051F  57                push di
00000520  1E                push ds
00000521  B88007            mov ax,0x780
00000524  50                push ax
00000525  9A6C02FD21        call 0x21fd:0x26c
0000052A  59                pop cx
0000052B  59                pop cx
0000052C  9A2903FD21        call 0x21fd:0x329
00000531  33C0              xor ax,ax
00000533  BA6400            mov dx,0x64
00000536  50                push ax
00000537  52                push dx
00000538  9A8501FD21        call 0x21fd:0x185
0000053D  59                pop cx
0000053E  59                pop cx
0000053F  9A9005FD21        call 0x21fd:0x590
00000544  B87007            mov ax,0x770
00000547  50                push ax
00000548  B80C00            mov ax,0xc
0000054B  50                push ax
0000054C  9A6D009522        call 0x2295:0x6d
00000551  59                pop cx
00000552  59                pop cx
00000553  C746FE0000        mov word [bp-0x2],0x0
00000558  EB54              jmp short 0x5ae
0000055A  9A4502FD21        call 0x21fd:0x245
0000055F  B84C00            mov ax,0x4c
00000562  50                push ax
00000563  B80100            mov ax,0x1
00000566  50                push ax
00000567  9A5400A721        call 0x21a7:0x54
0000056C  59                pop cx
0000056D  59                pop cx
0000056E  BEE500            mov si,0xe5
00000571  EB0C              jmp short 0x57f
00000573  B001              mov al,0x1
00000575  50                push ax
00000576  56                push si
00000577  9AE2071D11        call 0x111d:0x7e2
0000057C  59                pop cx
0000057D  59                pop cx
0000057E  4E                dec si
0000057F  0BF6              or si,si
00000581  7DF0              jnl 0x573
00000583  33C0              xor ax,ax
00000585  BA0500            mov dx,0x5
00000588  50                push ax
00000589  52                push dx
0000058A  9A8501FD21        call 0x21fd:0x185
0000058F  59                pop cx
00000590  59                pop cx
00000591  9A4502FD21        call 0x21fd:0x245
00000596  BEE500            mov si,0xe5
00000599  EB0C              jmp short 0x5a7
0000059B  B000              mov al,0x0
0000059D  50                push ax
0000059E  56                push si
0000059F  9AE2071D11        call 0x111d:0x7e2
000005A4  59                pop cx
000005A5  59                pop cx
000005A6  4E                dec si
000005A7  0BF6              or si,si
000005A9  7DF0              jnl 0x59b
000005AB  FF46FE            inc word [bp-0x2]
000005AE  837EFE04          cmp word [bp-0x2],byte +0x4
000005B2  7CA6              jl 0x55a
000005B4  33C0              xor ax,ax
000005B6  BA6400            mov dx,0x64
000005B9  50                push ax
000005BA  52                push dx
000005BB  9A8501FD21        call 0x21fd:0x185
000005C0  59                pop cx
000005C1  59                pop cx
000005C2  B80007            mov ax,0x700
000005C5  50                push ax
000005C6  B80C00            mov ax,0xc
000005C9  50                push ax
000005CA  9A6D009522        call 0x2295:0x6d
000005CF  59                pop cx
000005D0  59                pop cx
000005D1  B84B00            mov ax,0x4b
000005D4  50                push ax
000005D5  B80100            mov ax,0x1
000005D8  50                push ax
000005D9  9A5400A721        call 0x21a7:0x54
000005DE  59                pop cx
000005DF  59                pop cx
000005E0  33C0              xor ax,ax
000005E2  BA0100            mov dx,0x1
000005E5  50                push ax
000005E6  52                push dx
000005E7  9A8501FD21        call 0x21fd:0x185
000005EC  59                pop cx
000005ED  59                pop cx
000005EE  B84B00            mov ax,0x4b
000005F1  50                push ax
000005F2  B80200            mov ax,0x2
000005F5  50                push ax
000005F6  9A5400A721        call 0x21a7:0x54
000005FB  59                pop cx
000005FC  59                pop cx
000005FD  33F6              xor si,si
000005FF  EB0C              jmp short 0x60d
00000601  B001              mov al,0x1
00000603  50                push ax
00000604  56                push si
00000605  9AE2071D11        call 0x111d:0x7e2
0000060A  59                pop cx
0000060B  59                pop cx
0000060C  46                inc si
0000060D  81FEE500          cmp si,0xe5
00000611  7EEE              jng 0x601
00000613  33C0              xor ax,ax
00000615  BA0A00            mov dx,0xa
00000618  50                push ax
00000619  52                push dx
0000061A  9A8501FD21        call 0x21fd:0x185
0000061F  59                pop cx
00000620  59                pop cx
00000621  9A4502FD21        call 0x21fd:0x245
00000626  33F6              xor si,si
00000628  EB0C              jmp short 0x636
0000062A  B000              mov al,0x0
0000062C  50                push ax
0000062D  56                push si
0000062E  9AE2071D11        call 0x111d:0x7e2
00000633  59                pop cx
00000634  59                pop cx
00000635  46                inc si
00000636  81FEE500          cmp si,0xe5
0000063A  7EEE              jng 0x62a
0000063C  B84A00            mov ax,0x4a
0000063F  50                push ax
00000640  33C0              xor ax,ax
00000642  50                push ax
00000643  9A5400A721        call 0x21a7:0x54
00000648  59                pop cx
00000649  59                pop cx
0000064A  33FF              xor di,di
0000064C  EB2C              jmp short 0x67a
0000064E  9A4502FD21        call 0x21fd:0x245
00000653  9A1F05FD21        call 0x21fd:0x51f
00000658  33C0              xor ax,ax
0000065A  BA0100            mov dx,0x1
0000065D  50                push ax
0000065E  52                push dx
0000065F  9A8501FD21        call 0x21fd:0x185
00000664  59                pop cx
00000665  59                pop cx
00000666  9A1F05FD21        call 0x21fd:0x51f
0000066B  33C0              xor ax,ax
0000066D  BA0100            mov dx,0x1
00000670  50                push ax
00000671  52                push dx
00000672  9A8501FD21        call 0x21fd:0x185
00000677  59                pop cx
00000678  59                pop cx
00000679  47                inc di
0000067A  83FF32            cmp di,byte +0x32
0000067D  7CCF              jl 0x64e
0000067F  33C0              xor ax,ax
00000681  50                push ax
00000682  33C0              xor ax,ax
00000684  50                push ax
00000685  9A5400A721        call 0x21a7:0x54
0000068A  59                pop cx
0000068B  59                pop cx
0000068C  B80600            mov ax,0x6
0000068F  50                push ax
00000690  1E                push ds
00000691  B83438            mov ax,0x3834
00000694  50                push ax
00000695  9A57071D11        call 0x111d:0x757
0000069A  83C406            add sp,byte +0x6
0000069D  C606002F00        mov byte [0x2f00],0x0
000006A2  E99C00            jmp 0x741
000006A5  C606B02E00        mov byte [0x2eb0],0x0
000006AA  E98000            jmp 0x72d
000006AD  A0B02E            mov al,[0x2eb0]
000006B0  98                cbw
000006B1  BA0600            mov dx,0x6
000006B4  F7EA              imul dx
000006B6  50                push ax
000006B7  A0002F            mov al,[0x2f00]
000006BA  98                cbw
000006BB  5B                pop bx
000006BC  03D8              add bx,ax
000006BE  8A874244          mov al,[bx+0x4442]
000006C2  A2AA37            mov [0x37aa],al
000006C5  803EAA37FF        cmp byte [0x37aa],0xff
000006CA  7418              jz 0x6e4
000006CC  A0B02E            mov al,[0x2eb0]
000006CF  98                cbw
000006D0  BA0600            mov dx,0x6
000006D3  F7EA              imul dx
000006D5  50                push ax
000006D6  A0002F            mov al,[0x2f00]
000006D9  98                cbw
000006DA  5B                pop bx
000006DB  03D8              add bx,ax
000006DD  80BFB12E04        cmp byte [bx+0x2eb1],0x4
000006E2  7502              jnz 0x6e6
000006E4  EB43              jmp short 0x729
000006E6  B81300            mov ax,0x13
000006E9  50                push ax
000006EA  9ABA01FD21        call 0x21fd:0x1ba
000006EF  59                pop cx
000006F0  50                push ax
000006F1  A0AA37            mov al,[0x37aa]
000006F4  98                cbw
000006F5  8BD8              mov bx,ax
000006F7  8A87423D          mov al,[bx+0x3d42]
000006FB  B400              mov ah,0x0
000006FD  BA0300            mov dx,0x3
00000700  F7EA              imul dx
00000702  BA0B00            mov dx,0xb
00000705  2BD0              sub dx,ax
00000707  58                pop ax
00000708  3BC2              cmp ax,dx
0000070A  7E02              jng 0x70e
0000070C  EB1B              jmp short 0x729
0000070E  B8FEFF            mov ax,0xfffe
00000711  50                push ax
00000712  A0AA37            mov al,[0x37aa]
00000715  98                cbw
00000716  50                push ax
00000717  A0002F            mov al,[0x2f00]
0000071A  98                cbw
0000071B  50                push ax
0000071C  A0B02E            mov al,[0x2eb0]
0000071F  98                cbw
00000720  50                push ax
00000721  9AD2100906        call 0x609:0x10d2
00000726  83C408            add sp,byte +0x8
00000729  FE06B02E          inc byte [0x2eb0]
0000072D  A0B02E            mov al,[0x2eb0]
00000730  98                cbw
00000731  8B16FC08          mov dx,[0x8fc]
00000735  4A                dec dx
00000736  3BC2              cmp ax,dx
00000738  7F03              jg 0x73d
0000073A  E970FF            jmp 0x6ad
0000073D  FE06002F          inc byte [0x2f00]
00000741  A0002F            mov al,[0x2f00]
00000744  98                cbw
00000745  8B16FE08          mov dx,[0x8fe]
00000749  4A                dec dx
0000074A  3BC2              cmp ax,dx
0000074C  7F03              jg 0x751
0000074E  E954FF            jmp 0x6a5
00000751  5F                pop di
00000752  5E                pop si
00000753  8BE5              mov sp,bp
00000755  5D                pop bp
00000756  CB                retf

; ---- 111D:0757 ----
00000757  55                push bp
00000758  8BEC              mov bp,sp
0000075A  FF36CE08          push word [0x8ce]
0000075E  9A02006C18        call 0x186c:0x2
00000763  59                pop cx
00000764  B80400            mov ax,0x4
00000767  50                push ax
00000768  9A7D03FD21        call 0x21fd:0x37d
0000076D  59                pop cx
0000076E  9A1B00960A        call 0xa96:0x1b
00000773  9AD91E0906        call 0x609:0x1ed9
00000778  B83200            mov ax,0x32
0000077B  50                push ax
0000077C  B8F000            mov ax,0xf0
0000077F  50                push ax
00000780  B84B00            mov ax,0x4b
00000783  50                push ax
00000784  B82800            mov ax,0x28
00000787  50                push ax
00000788  9A56120906        call 0x609:0x1256
0000078D  83C408            add sp,byte +0x8
00000790  9A0D06FD21        call 0x21fd:0x60d
00000795  1E                push ds
00000796  B8A800            mov ax,0xa8
00000799  50                push ax
0000079A  9A6C02FD21        call 0x21fd:0x26c
0000079F  59                pop cx
000007A0  59                pop cx
000007A1  9A2903FD21        call 0x21fd:0x329
000007A6  B80300            mov ax,0x3
000007A9  50                push ax
000007AA  B80500            mov ax,0x5
000007AD  50                push ax
000007AE  FF760A            push word [bp+0xa]
000007B1  FF7608            push word [bp+0x8]
000007B4  FF7606            push word [bp+0x6]
000007B7  B8F000            mov ax,0xf0
000007BA  50                push ax
000007BB  8B460A            mov ax,[bp+0xa]
000007BE  BA0600            mov dx,0x6
000007C1  F7EA              imul dx
000007C3  50                push ax
000007C4  B83200            mov ax,0x32
000007C7  5A                pop dx
000007C8  2BC2              sub ax,dx
000007CA  BB0200            mov bx,0x2
000007CD  99                cwd
000007CE  F7FB              idiv bx
000007D0  055100            add ax,0x51
000007D3  50                push ax
000007D4  B82800            mov ax,0x28
000007D7  50                push ax
000007D8  9A8802690A        call 0xa69:0x288
000007DD  83C410            add sp,byte +0x10
000007E0  5D                pop bp
000007E1  CB                retf

; ---- 111D:07E2 ----
000007E2  55                push bp
000007E3  8BEC              mov bp,sp
000007E5  4C                dec sp
000007E6  4C                dec sp
000007E7  56                push si
000007E8  57                push di
000007E9  8B7E06            mov di,[bp+0x6]
000007EC  8BC7              mov ax,di
000007EE  BB0300            mov bx,0x3
000007F1  99                cwd
000007F2  F7FB              idiv bx
000007F4  BA2D00            mov dx,0x2d
000007F7  2BD0              sub dx,ax
000007F9  8BF2              mov si,dx
000007FB  8BC2              mov ax,dx
000007FD  0BC0              or ax,ax
000007FF  7D02              jnl 0x803
00000801  33F6              xor si,si
00000803  8BC7              mov ax,di
00000805  BB0500            mov bx,0x5
00000808  99                cwd
00000809  F7FB              idiv bx
0000080B  BA2E00            mov dx,0x2e
0000080E  2BD0              sub dx,ax
00000810  2BD6              sub dx,si
00000812  8956FE            mov [bp-0x2],dx
00000815  9A1B00960A        call 0xa96:0x1b
0000081A  807E0801          cmp byte [bp+0x8],0x1
0000081E  7518              jnz 0x838
00000820  B80C00            mov ax,0xc
00000823  50                push ax
00000824  FF76FE            push word [bp-0x2]
00000827  56                push si
00000828  8BC7              mov ax,di
0000082A  053C00            add ax,0x3c
0000082D  50                push ax
0000082E  9A2C00690A        call 0xa69:0x2c
00000833  83C408            add sp,byte +0x8
00000836  EB1D              jmp short 0x855
00000838  56                push si
00000839  8BC7              mov ax,di
0000083B  053C00            add ax,0x3c
0000083E  50                push ax
0000083F  FF76FE            push word [bp-0x2]
00000842  B80100            mov ax,0x1
00000845  50                push ax
00000846  56                push si
00000847  8BC7              mov ax,di
00000849  053C00            add ax,0x3c
0000084C  50                push ax
0000084D  9A3000960A        call 0xa96:0x30
00000852  83C40C            add sp,byte +0xc
00000855  9AA806960A        call 0xa96:0x6a8
0000085A  5F                pop di
0000085B  5E                pop si
0000085C  8BE5              mov sp,bp
0000085E  5D                pop bp
0000085F  CB                retf
