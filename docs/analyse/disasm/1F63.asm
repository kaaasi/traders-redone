; segment 1F63  file offset 0x22C30  size 0x1500
00000000  2000              and [bx+si],al
00000002  50                push ax
00000003  56                push si
00000004  9A3B01960A        call 0xa96:0x13b
00000009  83C410            add sp,byte +0x10
0000000C  5E                pop si
0000000D  5D                pop bp
0000000E  CB                retf

; ---- 1F63:000F ----
0000000F  55                push bp
00000010  8BEC              mov bp,sp
00000012  FF36D408          push word [0x8d4]
00000016  9A02006C18        call 0x186c:0x2
0000001B  59                pop cx
0000001C  9A4712631F        call 0x1f63:0x1247
00000021  33C0              xor ax,ax
00000023  50                push ax
00000024  9A7D03FD21        call 0x21fd:0x37d
00000029  59                pop cx
0000002A  9A240D631F        call 0x1f63:0xd24
0000002F  C606B15400        mov byte [0x54b1],0x0
00000034  1E                push ds
00000035  B8FC0F            mov ax,0xffc
00000038  50                push ax
00000039  9A6C02FD21        call 0x21fd:0x26c
0000003E  59                pop cx
0000003F  59                pop cx
00000040  9AFE13631F        call 0x1f63:0x13fe
00000045  9A2903FD21        call 0x21fd:0x329
0000004A  9AB201631F        call 0x1f63:0x1b2
0000004F  B80300            mov ax,0x3
00000052  50                push ax
00000053  B80800            mov ax,0x8
00000056  50                push ax
00000057  B80200            mov ax,0x2
0000005A  50                push ax
0000005B  1E                push ds
0000005C  B8643B            mov ax,0x3b64
0000005F  50                push ax
00000060  B88F00            mov ax,0x8f
00000063  50                push ax
00000064  B80A00            mov ax,0xa
00000067  50                push ax
00000068  B8B100            mov ax,0xb1
0000006B  50                push ax
0000006C  9A8802690A        call 0xa69:0x288
00000071  83C410            add sp,byte +0x10
00000074  33C0              xor ax,ax
00000076  BA3200            mov dx,0x32
00000079  50                push ax
0000007A  52                push dx
0000007B  9A8501FD21        call 0x21fd:0x185
00000080  59                pop cx
00000081  59                pop cx
00000082  B80600            mov ax,0x6
00000085  50                push ax
00000086  9ABA01FD21        call 0x21fd:0x1ba
0000008B  59                pop cx
0000008C  0BC0              or ax,ax
0000008E  7505              jnz 0x95
00000090  9A0503631F        call 0x1f63:0x305
00000095  FF362E08          push word [0x82e]
00000099  9A1C07CD23        call 0x23cd:0x71c
0000009E  59                pop cx
0000009F  9A9003631F        call 0x1f63:0x390
000000A4  C606E63C04        mov byte [0x3ce6],0x4
000000A9  C606AA3700        mov byte [0x37aa],0x0
000000AE  EB12              jmp short 0xc2
000000B0  A0AA37            mov al,[0x37aa]
000000B3  98                cbw
000000B4  D1E0              shl ax,1
000000B6  8BD8              mov bx,ax
000000B8  C787BA540000      mov word [bx+0x54ba],0x0
000000BE  FE06AA37          inc byte [0x37aa]
000000C2  803EAA3704        cmp byte [0x37aa],0x4
000000C7  7CE7              jl 0xb0
000000C9  C706CC540000      mov word [0x54cc],0x0
000000CF  EB77              jmp short 0x148
000000D1  8B1ECC54          mov bx,[0x54cc]
000000D5  D1E3              shl bx,1
000000D7  8B87C254          mov ax,[bx+0x54c2]
000000DB  A3B854            mov [0x54b8],ax
000000DE  FF36B854          push word [0x54b8]
000000E2  9A7B14631F        call 0x1f63:0x147b
000000E7  59                pop cx
000000E8  833ECC5402        cmp word [0x54cc],byte +0x2
000000ED  750A              jnz 0xf9
000000EF  B80100            mov ax,0x1
000000F2  50                push ax
000000F3  9A840F631F        call 0x1f63:0xf84
000000F8  59                pop cx
000000F9  A1CC54            mov ax,[0x54cc]
000000FC  0BC0              or ax,ax
000000FE  740C              jz 0x10c
00000100  3D0100            cmp ax,0x1
00000103  7418              jz 0x11d
00000105  3D0200            cmp ax,0x2
00000108  7424              jz 0x12e
0000010A  EB33              jmp short 0x13f
0000010C  B82100            mov ax,0x21
0000010F  50                push ax
00000110  B80200            mov ax,0x2
00000113  50                push ax
00000114  9A5400A721        call 0x21a7:0x54
00000119  59                pop cx
0000011A  59                pop cx
0000011B  EB22              jmp short 0x13f
0000011D  B82200            mov ax,0x22
00000120  50                push ax
00000121  B80200            mov ax,0x2
00000124  50                push ax
00000125  9A5400A721        call 0x21a7:0x54
0000012A  59                pop cx
0000012B  59                pop cx
0000012C  EB11              jmp short 0x13f
0000012E  B82300            mov ax,0x23
00000131  50                push ax
00000132  B80200            mov ax,0x2
00000135  50                push ax
00000136  9A5400A721        call 0x21a7:0x54
0000013B  59                pop cx
0000013C  59                pop cx
0000013D  EB00              jmp short 0x13f
0000013F  9AD804631F        call 0x1f63:0x4d8
00000144  FF06CC54          inc word [0x54cc]
00000148  833ECC5402        cmp word [0x54cc],byte +0x2
0000014D  7F0A              jg 0x159
0000014F  803EE63C00        cmp byte [0x3ce6],0x0
00000154  7603              jna 0x159
00000156  E978FF            jmp 0xd1
00000159  33C0              xor ax,ax
0000015B  50                push ax
0000015C  B80200            mov ax,0x2
0000015F  50                push ax
00000160  9A5400A721        call 0x21a7:0x54
00000165  59                pop cx
00000166  59                pop cx
00000167  803EE63C04        cmp byte [0x3ce6],0x4
0000016C  7504              jnz 0x172
0000016E  B001              mov al,0x1
00000170  EB02              jmp short 0x174
00000172  B000              mov al,0x0
00000174  A2C854            mov [0x54c8],al
00000177  803EE63C00        cmp byte [0x3ce6],0x0
0000017C  750B              jnz 0x189
0000017E  33C0              xor ax,ax
00000180  50                push ax
00000181  9A840F631F        call 0x1f63:0xf84
00000186  59                pop cx
00000187  EB0A              jmp short 0x193
00000189  B80200            mov ax,0x2
0000018C  50                push ax
0000018D  9A840F631F        call 0x1f63:0xf84
00000192  59                pop cx
00000193  33C0              xor ax,ax
00000195  BA6400            mov dx,0x64
00000198  50                push ax
00000199  52                push dx
0000019A  9A8501FD21        call 0x21fd:0x185
0000019F  59                pop cx
000001A0  59                pop cx
000001A1  9A1107631F        call 0x1f63:0x711
000001A6  9A7204AF21        call 0x21af:0x472
000001AB  9A9A07631F        call 0x1f63:0x79a
000001B0  5D                pop bp
000001B1  CB                retf

; ---- 1F63:01B2 ----
000001B2  55                push bp
000001B3  8BEC              mov bp,sp
000001B5  4C                dec sp
000001B6  4C                dec sp
000001B7  56                push si
000001B8  57                push di
000001B9  9A820C631F        call 0x1f63:0xc82
000001BE  8946FE            mov [bp-0x2],ax
000001C1  8B46FE            mov ax,[bp-0x2]
000001C4  BB7800            mov bx,0x78
000001C7  99                cwd
000001C8  F7FB              idiv bx
000001CA  40                inc ax
000001CB  A3B254            mov [0x54b2],ax
000001CE  9A1B00960A        call 0xa96:0x1b
000001D3  BE1E00            mov si,0x1e
000001D6  EB67              jmp short 0x23f
000001D8  33C0              xor ax,ax
000001DA  50                push ax
000001DB  B80500            mov ax,0x5
000001DE  50                push ax
000001DF  B80B00            mov ax,0xb
000001E2  50                push ax
000001E3  8BC6              mov ax,si
000001E5  052700            add ax,0x27
000001E8  50                push ax
000001E9  9A2C00690A        call 0xa69:0x2c
000001EE  83C408            add sp,byte +0x8
000001F1  B80B00            mov ax,0xb
000001F4  50                push ax
000001F5  B80500            mov ax,0x5
000001F8  50                push ax
000001F9  B80C00            mov ax,0xc
000001FC  50                push ax
000001FD  8BC6              mov ax,si
000001FF  052600            add ax,0x26
00000202  50                push ax
00000203  9A2C00690A        call 0xa69:0x2c
00000208  83C408            add sp,byte +0x8
0000020B  B80100            mov ax,0x1
0000020E  50                push ax
0000020F  B8FFFF            mov ax,0xffff
00000212  50                push ax
00000213  33C0              xor ax,ax
00000215  50                push ax
00000216  B80800            mov ax,0x8
00000219  50                push ax
0000021A  B80400            mov ax,0x4
0000021D  50                push ax
0000021E  A1B254            mov ax,[0x54b2]
00000221  F7EE              imul si
00000223  99                cwd
00000224  52                push dx
00000225  50                push ax
00000226  B80F00            mov ax,0xf
00000229  50                push ax
0000022A  8BC6              mov ax,si
0000022C  050D00            add ax,0xd
0000022F  50                push ax
00000230  9A1B01690A        call 0xa69:0x11b
00000235  83C412            add sp,byte +0x12
00000238  8BC6              mov ax,si
0000023A  051E00            add ax,0x1e
0000023D  8BF0              mov si,ax
0000023F  83FE78            cmp si,byte +0x78
00000242  7E94              jng 0x1d8
00000244  C606AA3700        mov byte [0x37aa],0x0
00000249  EB24              jmp short 0x26f
0000024B  803EB15401        cmp byte [0x54b1],0x1
00000250  7514              jnz 0x266
00000252  A0AA37            mov al,[0x37aa]
00000255  98                cbw
00000256  8BD8              mov bx,ax
00000258  80BFC24B01        cmp byte [bx+0x4bc2],0x1
0000025D  7507              jnz 0x266
0000025F  9A460F631F        call 0x1f63:0xf46
00000264  EB05              jmp short 0x26b
00000266  9AC70E631F        call 0x1f63:0xec7
0000026B  FE06AA37          inc byte [0x37aa]
0000026F  803EAA3704        cmp byte [0x37aa],0x4
00000274  7CD5              jl 0x24b
00000276  9A0D06FD21        call 0x21fd:0x60d
0000027B  B82400            mov ax,0x24
0000027E  50                push ax
0000027F  33C0              xor ax,ax
00000281  50                push ax
00000282  9A5400A721        call 0x21a7:0x54
00000287  59                pop cx
00000288  59                pop cx
00000289  BF0100            mov di,0x1
0000028C  EB67              jmp short 0x2f5
0000028E  8B46FE            mov ax,[bp-0x2]
00000291  99                cwd
00000292  F73EB254          idiv word [0x54b2]
00000296  3BC7              cmp ax,di
00000298  7D02              jnl 0x29c
0000029A  EB5E              jmp short 0x2fa
0000029C  9A1B00960A        call 0xa96:0x1b
000002A1  C606AA3700        mov byte [0x37aa],0x0
000002A6  EB40              jmp short 0x2e8
000002A8  803EB15401        cmp byte [0x54b1],0x1
000002AD  750F              jnz 0x2be
000002AF  A0AA37            mov al,[0x37aa]
000002B2  98                cbw
000002B3  8BD8              mov bx,ax
000002B5  80BFC24B01        cmp byte [bx+0x4bc2],0x1
000002BA  7502              jnz 0x2be
000002BC  EB26              jmp short 0x2e4
000002BE  A0AA37            mov al,[0x37aa]
000002C1  98                cbw
000002C2  D1E0              shl ax,1
000002C4  D1E0              shl ax,1
000002C6  D1E0              shl ax,1
000002C8  8BD8              mov bx,ax
000002CA  8B87FA3C          mov ax,[bx+0x3cfa]
000002CE  99                cwd
000002CF  F73EB254          idiv word [0x54b2]
000002D3  3BC7              cmp ax,di
000002D5  7C0D              jl 0x2e4
000002D7  57                push di
000002D8  A0AA37            mov al,[0x37aa]
000002DB  98                cbw
000002DC  50                push ax
000002DD  9A5C0E631F        call 0x1f63:0xe5c
000002E2  59                pop cx
000002E3  59                pop cx
000002E4  FE06AA37          inc byte [0x37aa]
000002E8  803EAA3704        cmp byte [0x37aa],0x4
000002ED  7CB9              jl 0x2a8
000002EF  9A0D06FD21        call 0x21fd:0x60d
000002F4  47                inc di
000002F5  83FF78            cmp di,byte +0x78
000002F8  7E94              jng 0x28e
000002FA  8B46FE            mov ax,[bp-0x2]
000002FD  EB00              jmp short 0x2ff
000002FF  5F                pop di
00000300  5E                pop si
00000301  8BE5              mov sp,bp
00000303  5D                pop bp
00000304  CB                retf

; ---- 1F63:0305 ----
00000305  55                push bp
00000306  8BEC              mov bp,sp
00000308  4C                dec sp
00000309  4C                dec sp
0000030A  56                push si
0000030B  57                push di
0000030C  B80300            mov ax,0x3
0000030F  50                push ax
00000310  9ABA01FD21        call 0x21fd:0x1ba
00000315  59                pop cx
00000316  8BF8              mov di,ax
00000318  33F6              xor si,si
0000031A  EB6A              jmp short 0x386
0000031C  B80100            mov ax,0x1
0000031F  50                push ax
00000320  9ABA01FD21        call 0x21fd:0x1ba
00000325  59                pop cx
00000326  40                inc ax
00000327  8946FE            mov [bp-0x2],ax
0000032A  8A46FE            mov al,[bp-0x2]
0000032D  8884ED08          mov [si+0x8ed],al
00000331  9A1B00960A        call 0xa96:0x1b
00000336  B000              mov al,0x0
00000338  50                push ax
00000339  FF76FE            push word [bp-0x2]
0000033C  9A4B10631F        call 0x1f63:0x104b
00000341  59                pop cx
00000342  59                pop cx
00000343  9AA806960A        call 0xa96:0x6a8
00000348  9A1F05FD21        call 0x21fd:0x51f
0000034D  33C0              xor ax,ax
0000034F  BA0A00            mov dx,0xa
00000352  50                push ax
00000353  52                push dx
00000354  9A8501FD21        call 0x21fd:0x185
00000359  59                pop cx
0000035A  59                pop cx
0000035B  9A1F05FD21        call 0x21fd:0x51f
00000360  9A1B00960A        call 0xa96:0x1b
00000365  B001              mov al,0x1
00000367  50                push ax
00000368  FF76FE            push word [bp-0x2]
0000036B  9A4B10631F        call 0x1f63:0x104b
00000370  59                pop cx
00000371  59                pop cx
00000372  9AA806960A        call 0xa96:0x6a8
00000377  33C0              xor ax,ax
00000379  BA1400            mov dx,0x14
0000037C  50                push ax
0000037D  52                push dx
0000037E  9A8501FD21        call 0x21fd:0x185
00000383  59                pop cx
00000384  59                pop cx
00000385  46                inc si
00000386  3BF7              cmp si,di
00000388  7E92              jng 0x31c
0000038A  5F                pop di
0000038B  5E                pop si
0000038C  8BE5              mov sp,bp
0000038E  5D                pop bp
0000038F  CB                retf

; ---- 1F63:0390 ----
00000390  55                push bp
00000391  8BEC              mov bp,sp
00000393  56                push si
00000394  33F6              xor si,si
00000396  C706B4540000      mov word [0x54b4],0x0
0000039C  C606AA3700        mov byte [0x37aa],0x0
000003A1  EB18              jmp short 0x3bb
000003A3  A0AA37            mov al,[0x37aa]
000003A6  98                cbw
000003A7  D1E0              shl ax,1
000003A9  D1E0              shl ax,1
000003AB  D1E0              shl ax,1
000003AD  8BD8              mov bx,ax
000003AF  8B87FA3C          mov ax,[bx+0x3cfa]
000003B3  0106B454          add [0x54b4],ax
000003B7  FE06AA37          inc byte [0x37aa]
000003BB  803EAA3704        cmp byte [0x37aa],0x4
000003C0  7CE1              jl 0x3a3
000003C2  A1B454            mov ax,[0x54b4]
000003C5  BB1800            mov bx,0x18
000003C8  99                cwd
000003C9  F7FB              idiv bx
000003CB  A3B454            mov [0x54b4],ax
000003CE  833EB4541E        cmp word [0x54b4],byte +0x1e
000003D3  7E06              jng 0x3db
000003D5  C706B4541E00      mov word [0x54b4],0x1e
000003DB  C706CC540000      mov word [0x54cc],0x0
000003E1  E9B200            jmp 0x496
000003E4  833E9A4B04        cmp word [0x4b9a],byte +0x4
000003E9  7C47              jl 0x432
000003EB  B80600            mov ax,0x6
000003EE  50                push ax
000003EF  9ABA01FD21        call 0x21fd:0x1ba
000003F4  59                pop cx
000003F5  0BC0              or ax,ax
000003F7  7502              jnz 0x3fb
000003F9  33F6              xor si,si
000003FB  B80F00            mov ax,0xf
000003FE  50                push ax
000003FF  9ABA01FD21        call 0x21fd:0x1ba
00000404  59                pop cx
00000405  0BC0              or ax,ax
00000407  7503              jnz 0x40c
00000409  BE0100            mov si,0x1
0000040C  B81400            mov ax,0x14
0000040F  50                push ax
00000410  9ABA01FD21        call 0x21fd:0x1ba
00000415  59                pop cx
00000416  0BC0              or ax,ax
00000418  7415              jz 0x42f
0000041A  803E052F04        cmp byte [0x2f05],0x4
0000041F  7D11              jnl 0x432
00000421  B80500            mov ax,0x5
00000424  50                push ax
00000425  9ABA01FD21        call 0x21fd:0x1ba
0000042A  59                pop cx
0000042B  0BC0              or ax,ax
0000042D  7503              jnz 0x432
0000042F  BE0200            mov si,0x2
00000432  B80500            mov ax,0x5
00000435  50                push ax
00000436  9ABA01FD21        call 0x21fd:0x1ba
0000043B  59                pop cx
0000043C  40                inc ax
0000043D  F72EB454          imul word [0x54b4]
00000441  A3B854            mov [0x54b8],ax
00000444  83FE01            cmp si,byte +0x1
00000447  7514              jnz 0x45d
00000449  B80500            mov ax,0x5
0000044C  50                push ax
0000044D  9ABA01FD21        call 0x21fd:0x1ba
00000452  59                pop cx
00000453  050300            add ax,0x3
00000456  F72EB454          imul word [0x54b4]
0000045A  A3B854            mov [0x54b8],ax
0000045D  83FE02            cmp si,byte +0x2
00000460  7506              jnz 0x468
00000462  C706B854E703      mov word [0x54b8],0x3e7
00000468  833EB8540A        cmp word [0x54b8],byte +0xa
0000046D  7D06              jnl 0x475
0000046F  C706B8540A00      mov word [0x54b8],0xa
00000475  A1B854            mov ax,[0x54b8]
00000478  99                cwd
00000479  F73EB254          idiv word [0x54b2]
0000047D  A1B854            mov ax,[0x54b8]
00000480  2BC2              sub ax,dx
00000482  A3B854            mov [0x54b8],ax
00000485  8B1ECC54          mov bx,[0x54cc]
00000489  D1E3              shl bx,1
0000048B  A1B854            mov ax,[0x54b8]
0000048E  8987C254          mov [bx+0x54c2],ax
00000492  FF06CC54          inc word [0x54cc]
00000496  833ECC5402        cmp word [0x54cc],byte +0x2
0000049B  7F03              jg 0x4a0
0000049D  E944FF            jmp 0x3e4
000004A0  C706D2540000      mov word [0x54d2],0x0
000004A6  C706CC540100      mov word [0x54cc],0x1
000004AC  EB20              jmp short 0x4ce
000004AE  8B1ECC54          mov bx,[0x54cc]
000004B2  D1E3              shl bx,1
000004B4  8B87C254          mov ax,[bx+0x54c2]
000004B8  8B1ED254          mov bx,[0x54d2]
000004BC  D1E3              shl bx,1
000004BE  3B87C254          cmp ax,[bx+0x54c2]
000004C2  7D06              jnl 0x4ca
000004C4  A1CC54            mov ax,[0x54cc]
000004C7  A3D254            mov [0x54d2],ax
000004CA  FF06CC54          inc word [0x54cc]
000004CE  833ECC5402        cmp word [0x54cc],byte +0x2
000004D3  7ED9              jng 0x4ae
000004D5  5E                pop si
000004D6  5D                pop bp
000004D7  CB                retf

; ---- 1F63:04D8 ----
000004D8  55                push bp
000004D9  8BEC              mov bp,sp
000004DB  56                push si
000004DC  33F6              xor si,si
000004DE  E94A01            jmp 0x62b
000004E1  9A1B00960A        call 0xa96:0x1b
000004E6  833ECC5401        cmp word [0x54cc],byte +0x1
000004EB  7F05              jg 0x4f2
000004ED  9A3E11631F        call 0x1f63:0x113e
000004F2  9A7A039E0E        call 0xe9e:0x37a
000004F7  9A4502FD21        call 0x21fd:0x245
000004FC  C606AA3700        mov byte [0x37aa],0x0
00000501  E90001            jmp 0x604
00000504  A0AA37            mov al,[0x37aa]
00000507  98                cbw
00000508  D1E0              shl ax,1
0000050A  8BD8              mov bx,ax
0000050C  83BFBA5401        cmp word [bx+0x54ba],byte +0x1
00000511  7503              jnz 0x516
00000513  E9EA00            jmp 0x600
00000516  A0AA37            mov al,[0x37aa]
00000519  98                cbw
0000051A  8BD8              mov bx,ax
0000051C  80BF400801        cmp byte [bx+0x840],0x1
00000521  7524              jnz 0x547
00000523  8B1E8E2E          mov bx,[0x2e8e]
00000527  8A872010          mov al,[bx+0x1020]
0000052B  98                cbw
0000052C  BA1F00            mov dx,0x1f
0000052F  F7EA              imul dx
00000531  50                push ax
00000532  A0AA37            mov al,[0x37aa]
00000535  98                cbw
00000536  BA1400            mov dx,0x14
00000539  F7EA              imul dx
0000053B  5A                pop dx
0000053C  2BD0              sub dx,ax
0000053E  3BD6              cmp dx,si
00000540  7505              jnz 0x547
00000542  9A4A06631F        call 0x1f63:0x64a
00000547  A0AA37            mov al,[0x37aa]
0000054A  98                cbw
0000054B  8BD8              mov bx,ax
0000054D  80BF08330A        cmp byte [bx+0x3308],0xa
00000552  7403              jz 0x557
00000554  E9A900            jmp 0x600
00000557  A0AA37            mov al,[0x37aa]
0000055A  98                cbw
0000055B  D1E0              shl ax,1
0000055D  D1E0              shl ax,1
0000055F  D1E0              shl ax,1
00000561  8BD8              mov bx,ax
00000563  8B87FA3C          mov ax,[bx+0x3cfa]
00000567  3B06B854          cmp ax,[0x54b8]
0000056B  7D03              jnl 0x570
0000056D  E99000            jmp 0x600
00000570  A0AA37            mov al,[0x37aa]
00000573  98                cbw
00000574  D1E0              shl ax,1
00000576  D1E0              shl ax,1
00000578  D1E0              shl ax,1
0000057A  8B16B854          mov dx,[0x54b8]
0000057E  8BD8              mov bx,ax
00000580  2997FA3C          sub [bx+0x3cfa],dx
00000584  B80100            mov ax,0x1
00000587  50                push ax
00000588  B8FFFF            mov ax,0xffff
0000058B  50                push ax
0000058C  33C0              xor ax,ax
0000058E  50                push ax
0000058F  A0AA37            mov al,[0x37aa]
00000592  98                cbw
00000593  8BD8              mov bx,ax
00000595  8A871C10          mov al,[bx+0x101c]
00000599  B400              mov ah,0x0
0000059B  50                push ax
0000059C  1E                push ds
0000059D  B83C10            mov ax,0x103c
000005A0  50                push ax
000005A1  A0AA37            mov al,[0x37aa]
000005A4  98                cbw
000005A5  BA2100            mov dx,0x21
000005A8  F7EA              imul dx
000005AA  052D00            add ax,0x2d
000005AD  50                push ax
000005AE  B89300            mov ax,0x93
000005B1  50                push ax
000005B2  9A4D00690A        call 0xa69:0x4d
000005B7  83C410            add sp,byte +0x10
000005BA  B82500            mov ax,0x25
000005BD  50                push ax
000005BE  33C0              xor ax,ax
000005C0  50                push ax
000005C1  9A5400A721        call 0x21a7:0x54
000005C6  59                pop cx
000005C7  59                pop cx
000005C8  9AC70E631F        call 0x1f63:0xec7
000005CD  A0AA37            mov al,[0x37aa]
000005D0  98                cbw
000005D1  D1E0              shl ax,1
000005D3  D1E0              shl ax,1
000005D5  D1E0              shl ax,1
000005D7  8BD8              mov bx,ax
000005D9  8B87FA3C          mov ax,[bx+0x3cfa]
000005DD  99                cwd
000005DE  F73EB254          idiv word [0x54b2]
000005E2  BA7800            mov dx,0x78
000005E5  2BD0              sub dx,ax
000005E7  52                push dx
000005E8  9A8F0E631F        call 0x1f63:0xe8f
000005ED  59                pop cx
000005EE  A0AA37            mov al,[0x37aa]
000005F1  98                cbw
000005F2  D1E0              shl ax,1
000005F4  8BD8              mov bx,ax
000005F6  C787BA540100      mov word [bx+0x54ba],0x1
000005FC  FE0EE63C          dec byte [0x3ce6]
00000600  FE06AA37          inc byte [0x37aa]
00000604  803EAA3704        cmp byte [0x37aa],0x4
00000609  7D03              jnl 0x60e
0000060B  E9F6FE            jmp 0x504
0000060E  833EBE1500        cmp word [0x15be],byte +0x0
00000613  7E07              jng 0x61c
00000615  9A8511631F        call 0x1f63:0x1185
0000061A  EB0E              jmp short 0x62a
0000061C  33C0              xor ax,ax
0000061E  BA0200            mov dx,0x2
00000621  50                push ax
00000622  52                push dx
00000623  9A8501FD21        call 0x21fd:0x185
00000628  59                pop cx
00000629  59                pop cx
0000062A  46                inc si
0000062B  8B1E8E2E          mov bx,[0x2e8e]
0000062F  8A872010          mov al,[bx+0x1020]
00000633  98                cbw
00000634  BA1F00            mov dx,0x1f
00000637  F7EA              imul dx
00000639  3BC6              cmp ax,si
0000063B  7C0A              jl 0x647
0000063D  803EE63C00        cmp byte [0x3ce6],0x0
00000642  7603              jna 0x647
00000644  E99AFE            jmp 0x4e1
00000647  5E                pop si
00000648  5D                pop bp
00000649  CB                retf

; ---- 1F63:064A ----
0000064A  55                push bp
0000064B  8BEC              mov bp,sp
0000064D  4C                dec sp
0000064E  4C                dec sp
0000064F  C746FEFFFF        mov word [bp-0x2],0xffff
00000654  A19A4B            mov ax,[0x4b9a]
00000657  BB0700            mov bx,0x7
0000065A  99                cwd
0000065B  F7FB              idiv bx
0000065D  A0AA37            mov al,[0x37aa]
00000660  98                cbw
00000661  3BD0              cmp dx,ax
00000663  7511              jnz 0x676
00000665  A1CC54            mov ax,[0x54cc]
00000668  3B06D254          cmp ax,[0x54d2]
0000066C  7505              jnz 0x673
0000066E  C746FE0A00        mov word [bp-0x2],0xa
00000673  E98A00            jmp 0x700
00000676  A0AA37            mov al,[0x37aa]
00000679  98                cbw
0000067A  8BD8              mov bx,ax
0000067C  80BFC24B01        cmp byte [bx+0x4bc2],0x1
00000681  7507              jnz 0x68a
00000683  C746FE0A00        mov word [bp-0x2],0xa
00000688  EB76              jmp short 0x700
0000068A  A0AA37            mov al,[0x37aa]
0000068D  98                cbw
0000068E  D1E0              shl ax,1
00000690  D1E0              shl ax,1
00000692  D1E0              shl ax,1
00000694  8B16B854          mov dx,[0x54b8]
00000698  D1E2              shl dx,1
0000069A  D1E2              shl dx,1
0000069C  8BD8              mov bx,ax
0000069E  3997FA3C          cmp [bx+0x3cfa],dx
000006A2  7C07              jl 0x6ab
000006A4  C746FE0A00        mov word [bp-0x2],0xa
000006A9  EB55              jmp short 0x700
000006AB  B80500            mov ax,0x5
000006AE  50                push ax
000006AF  9ABA01FD21        call 0x21fd:0x1ba
000006B4  59                pop cx
000006B5  0BC0              or ax,ax
000006B7  7507              jnz 0x6c0
000006B9  C746FE0A00        mov word [bp-0x2],0xa
000006BE  EB40              jmp short 0x700
000006C0  A1CC54            mov ax,[0x54cc]
000006C3  0BC0              or ax,ax
000006C5  740C              jz 0x6d3
000006C7  3D0100            cmp ax,0x1
000006CA  7419              jz 0x6e5
000006CC  3D0200            cmp ax,0x2
000006CF  7428              jz 0x6f9
000006D1  EB2D              jmp short 0x700
000006D3  A1B454            mov ax,[0x54b4]
000006D6  D1E0              shl ax,1
000006D8  3B06B854          cmp ax,[0x54b8]
000006DC  7C05              jl 0x6e3
000006DE  C746FE0A00        mov word [bp-0x2],0xa
000006E3  EB1B              jmp short 0x700
000006E5  A1B454            mov ax,[0x54b4]
000006E8  D1E0              shl ax,1
000006EA  D1E0              shl ax,1
000006EC  3B06B854          cmp ax,[0x54b8]
000006F0  7C05              jl 0x6f7
000006F2  C746FE0A00        mov word [bp-0x2],0xa
000006F7  EB07              jmp short 0x700
000006F9  C746FE0A00        mov word [bp-0x2],0xa
000006FE  EB00              jmp short 0x700
00000700  A0AA37            mov al,[0x37aa]
00000703  98                cbw
00000704  8A56FE            mov dl,[bp-0x2]
00000707  8BD8              mov bx,ax
00000709  88970833          mov [bx+0x3308],dl
0000070D  8BE5              mov sp,bp
0000070F  5D                pop bp
00000710  CB                retf

; ---- 1F63:0711 ----
00000711  55                push bp
00000712  8BEC              mov bp,sp
00000714  C606AA3700        mov byte [0x37aa],0x0
00000719  EB76              jmp short 0x791
0000071B  A0AA37            mov al,[0x37aa]
0000071E  98                cbw
0000071F  D1E0              shl ax,1
00000721  8BD8              mov bx,ax
00000723  83BFBA5401        cmp word [bx+0x54ba],byte +0x1
00000728  7518              jnz 0x742
0000072A  A0AA37            mov al,[0x37aa]
0000072D  98                cbw
0000072E  8BD8              mov bx,ax
00000730  C687CE5400        mov byte [bx+0x54ce],0x0
00000735  A0AA37            mov al,[0x37aa]
00000738  98                cbw
00000739  8BD8              mov bx,ax
0000073B  C687C24B00        mov byte [bx+0x4bc2],0x0
00000740  EB4B              jmp short 0x78d
00000742  A0AA37            mov al,[0x37aa]
00000745  98                cbw
00000746  8BD8              mov bx,ax
00000748  80BFC24B01        cmp byte [bx+0x4bc2],0x1
0000074D  7407              jz 0x756
0000074F  803EC85401        cmp byte [0x54c8],0x1
00000754  750D              jnz 0x763
00000756  A0AA37            mov al,[0x37aa]
00000759  98                cbw
0000075A  8BD8              mov bx,ax
0000075C  C687CE5402        mov byte [bx+0x54ce],0x2
00000761  EB0B              jmp short 0x76e
00000763  A0AA37            mov al,[0x37aa]
00000766  98                cbw
00000767  8BD8              mov bx,ax
00000769  C687CE5401        mov byte [bx+0x54ce],0x1
0000076E  A0AA37            mov al,[0x37aa]
00000771  98                cbw
00000772  8BD8              mov bx,ax
00000774  C687C24B01        mov byte [bx+0x4bc2],0x1
00000779  9A8A0A631F        call 0x1f63:0xa8a
0000077E  0BC0              or ax,ax
00000780  7F0B              jg 0x78d
00000782  A0AA37            mov al,[0x37aa]
00000785  98                cbw
00000786  8BD8              mov bx,ax
00000788  C687CE5400        mov byte [bx+0x54ce],0x0
0000078D  FE06AA37          inc byte [0x37aa]
00000791  803EAA3704        cmp byte [0x37aa],0x4
00000796  7C83              jl 0x71b
00000798  5D                pop bp
00000799  CB                retf

; ---- 1F63:079A ----
0000079A  55                push bp
0000079B  8BEC              mov bp,sp
0000079D  56                push si
0000079E  33F6              xor si,si
000007A0  C606AA3700        mov byte [0x37aa],0x0
000007A5  EB12              jmp short 0x7b9
000007A7  A0AA37            mov al,[0x37aa]
000007AA  98                cbw
000007AB  8BD8              mov bx,ax
000007AD  80BFCE5400        cmp byte [bx+0x54ce],0x0
000007B2  7401              jz 0x7b5
000007B4  46                inc si
000007B5  FE06AA37          inc byte [0x37aa]
000007B9  803EAA3704        cmp byte [0x37aa],0x4
000007BE  7CE7              jl 0x7a7
000007C0  0BF6              or si,si
000007C2  7503              jnz 0x7c7
000007C4  E91001            jmp 0x8d7
000007C7  FF36CE08          push word [0x8ce]
000007CB  9A02006C18        call 0x186c:0x2
000007D0  59                pop cx
000007D1  B80500            mov ax,0x5
000007D4  50                push ax
000007D5  9A7D03FD21        call 0x21fd:0x37d
000007DA  59                pop cx
000007DB  9A0D00AF21        call 0x21af:0xd
000007E0  1E                push ds
000007E1  B8A800            mov ax,0xa8
000007E4  50                push ax
000007E5  9A6C02FD21        call 0x21fd:0x26c
000007EA  59                pop cx
000007EB  59                pop cx
000007EC  9AE7110906        call 0x609:0x11e7
000007F1  803EC85401        cmp byte [0x54c8],0x1
000007F6  7569              jnz 0x861
000007F8  9A1B00960A        call 0xa96:0x1b
000007FD  9AD91E0906        call 0x609:0x1ed9
00000802  B83200            mov ax,0x32
00000805  50                push ax
00000806  B8F000            mov ax,0xf0
00000809  50                push ax
0000080A  B84B00            mov ax,0x4b
0000080D  50                push ax
0000080E  B82800            mov ax,0x28
00000811  50                push ax
00000812  9A56120906        call 0x609:0x1256
00000817  83C408            add sp,byte +0x8
0000081A  9A0D06FD21        call 0x21fd:0x60d
0000081F  9A2903FD21        call 0x21fd:0x329
00000824  B80500            mov ax,0x5
00000827  50                push ax
00000828  B80500            mov ax,0x5
0000082B  50                push ax
0000082C  B80200            mov ax,0x2
0000082F  50                push ax
00000830  1E                push ds
00000831  B8743B            mov ax,0x3b74
00000834  50                push ax
00000835  B8F000            mov ax,0xf0
00000838  50                push ax
00000839  B86300            mov ax,0x63
0000083C  50                push ax
0000083D  B82800            mov ax,0x28
00000840  50                push ax
00000841  9A8802690A        call 0xa69:0x288
00000846  83C410            add sp,byte +0x10
00000849  33C0              xor ax,ax
0000084B  BA6400            mov dx,0x64
0000084E  50                push ax
0000084F  52                push dx
00000850  9A8501FD21        call 0x21fd:0x185
00000855  59                pop cx
00000856  59                pop cx
00000857  B80500            mov ax,0x5
0000085A  50                push ax
0000085B  9A7D03FD21        call 0x21fd:0x37d
00000860  59                pop cx
00000861  9AE51B0906        call 0x609:0x1be5
00000866  9A2903FD21        call 0x21fd:0x329
0000086B  B80100            mov ax,0x1
0000086E  50                push ax
0000086F  9ADB05FD21        call 0x21fd:0x5db
00000874  59                pop cx
00000875  803EC85401        cmp byte [0x54c8],0x1
0000087A  7525              jnz 0x8a1
0000087C  B80500            mov ax,0x5
0000087F  50                push ax
00000880  B80500            mov ax,0x5
00000883  50                push ax
00000884  B80300            mov ax,0x3
00000887  50                push ax
00000888  1E                push ds
00000889  B8943B            mov ax,0x3b94
0000088C  50                push ax
0000088D  B8E000            mov ax,0xe0
00000890  50                push ax
00000891  B81300            mov ax,0x13
00000894  50                push ax
00000895  B81000            mov ax,0x10
00000898  50                push ax
00000899  9A8802690A        call 0xa69:0x288
0000089E  83C410            add sp,byte +0x10
000008A1  C606AA3700        mov byte [0x37aa],0x0
000008A6  EB16              jmp short 0x8be
000008A8  A0AA37            mov al,[0x37aa]
000008AB  98                cbw
000008AC  8BD8              mov bx,ax
000008AE  80BFCE5400        cmp byte [bx+0x54ce],0x0
000008B3  7405              jz 0x8ba
000008B5  9ADA08631F        call 0x1f63:0x8da
000008BA  FE06AA37          inc byte [0x37aa]
000008BE  803EAA3704        cmp byte [0x37aa],0x4
000008C3  7CE3              jl 0x8a8
000008C5  33C0              xor ax,ax
000008C7  50                push ax
000008C8  9ADB05FD21        call 0x21fd:0x5db
000008CD  59                pop cx
000008CE  33C0              xor ax,ax
000008D0  50                push ax
000008D1  9A0400FD21        call 0x21fd:0x4
000008D6  59                pop cx
000008D7  5E                pop si
000008D8  5D                pop bp
000008D9  CB                retf

; ---- 1F63:08DA ----
000008DA  55                push bp
000008DB  8BEC              mov bp,sp
000008DD  56                push si
000008DE  57                push di
000008DF  9A8A0A631F        call 0x1f63:0xa8a
000008E4  8BF0              mov si,ax
000008E6  0BF6              or si,si
000008E8  7F03              jg 0x8ed
000008EA  E99901            jmp 0xa86
000008ED  9A1B00960A        call 0xa96:0x1b
000008F2  B80300            mov ax,0x3
000008F5  50                push ax
000008F6  B80600            mov ax,0x6
000008F9  50                push ax
000008FA  B8F000            mov ax,0xf0
000008FD  50                push ax
000008FE  9A79180906        call 0x609:0x1879
00000903  83C406            add sp,byte +0x6
00000906  9A0D06FD21        call 0x21fd:0x60d
0000090B  803EC85400        cmp byte [0x54c8],0x0
00000910  7574              jnz 0x986
00000912  9A1B00960A        call 0xa96:0x1b
00000917  B82C00            mov ax,0x2c
0000091A  50                push ax
0000091B  B8E000            mov ax,0xe0
0000091E  50                push ax
0000091F  B80600            mov ax,0x6
00000922  50                push ax
00000923  B81000            mov ax,0x10
00000926  50                push ax
00000927  B81600            mov ax,0x16
0000092A  50                push ax
0000092B  B82000            mov ax,0x20
0000092E  50                push ax
0000092F  B8B000            mov ax,0xb0
00000932  50                push ax
00000933  B8F000            mov ax,0xf0
00000936  50                push ax
00000937  9A6702960A        call 0xa96:0x267
0000093C  83C410            add sp,byte +0x10
0000093F  9A0D06FD21        call 0x21fd:0x60d
00000944  A0AA37            mov al,[0x37aa]
00000947  98                cbw
00000948  8BD8              mov bx,ax
0000094A  80BFCE5402        cmp byte [bx+0x54ce],0x2
0000094F  7505              jnz 0x956
00000951  B8EC00            mov ax,0xec
00000954  EB03              jmp short 0x959
00000956  B8EF00            mov ax,0xef
00000959  8BF8              mov di,ax
0000095B  B80500            mov ax,0x5
0000095E  50                push ax
0000095F  B80500            mov ax,0x5
00000962  50                push ax
00000963  B80300            mov ax,0x3
00000966  50                push ax
00000967  1E                push ds
00000968  8BC7              mov ax,di
0000096A  D1E0              shl ax,1
0000096C  D1E0              shl ax,1
0000096E  05CC37            add ax,0x37cc
00000971  50                push ax
00000972  B8E000            mov ax,0xe0
00000975  50                push ax
00000976  B81300            mov ax,0x13
00000979  50                push ax
0000097A  B81000            mov ax,0x10
0000097D  50                push ax
0000097E  9A8802690A        call 0xa69:0x288
00000983  83C410            add sp,byte +0x10
00000986  E9F600            jmp 0xa7f
00000989  A1FC08            mov ax,[0x8fc]
0000098C  48                dec ax
0000098D  50                push ax
0000098E  9ABA01FD21        call 0x21fd:0x1ba
00000993  59                pop cx
00000994  A2B02E            mov [0x2eb0],al
00000997  A1FE08            mov ax,[0x8fe]
0000099A  48                dec ax
0000099B  50                push ax
0000099C  9ABA01FD21        call 0x21fd:0x1ba
000009A1  59                pop cx
000009A2  A2002F            mov [0x2f00],al
000009A5  A0B02E            mov al,[0x2eb0]
000009A8  98                cbw
000009A9  BA0600            mov dx,0x6
000009AC  F7EA              imul dx
000009AE  50                push ax
000009AF  A0002F            mov al,[0x2f00]
000009B2  98                cbw
000009B3  5B                pop bx
000009B4  03D8              add bx,ax
000009B6  8A874244          mov al,[bx+0x4442]
000009BA  3A06AA37          cmp al,[0x37aa]
000009BE  7518              jnz 0x9d8
000009C0  A0B02E            mov al,[0x2eb0]
000009C3  98                cbw
000009C4  BA0600            mov dx,0x6
000009C7  F7EA              imul dx
000009C9  50                push ax
000009CA  A0002F            mov al,[0x2f00]
000009CD  98                cbw
000009CE  5B                pop bx
000009CF  03D8              add bx,ax
000009D1  80BFB12E04        cmp byte [bx+0x2eb1],0x4
000009D6  7503              jnz 0x9db
000009D8  E9A400            jmp 0xa7f
000009DB  A0AA37            mov al,[0x37aa]
000009DE  98                cbw
000009DF  8BD8              mov bx,ax
000009E1  80BFCE5402        cmp byte [bx+0x54ce],0x2
000009E6  752A              jnz 0xa12
000009E8  B8FEFF            mov ax,0xfffe
000009EB  50                push ax
000009EC  B8FFFF            mov ax,0xffff
000009EF  50                push ax
000009F0  A0002F            mov al,[0x2f00]
000009F3  98                cbw
000009F4  50                push ax
000009F5  A0B02E            mov al,[0x2eb0]
000009F8  98                cbw
000009F9  50                push ax
000009FA  9AD2100906        call 0x609:0x10d2
000009FF  83C408            add sp,byte +0x8
00000A02  B82700            mov ax,0x27
00000A05  50                push ax
00000A06  33C0              xor ax,ax
00000A08  50                push ax
00000A09  9A5400A721        call 0x21a7:0x54
00000A0E  59                pop cx
00000A0F  59                pop cx
00000A10  EB43              jmp short 0xa55
00000A12  A0B02E            mov al,[0x2eb0]
00000A15  98                cbw
00000A16  BA0600            mov dx,0x6
00000A19  F7EA              imul dx
00000A1B  50                push ax
00000A1C  A0002F            mov al,[0x2f00]
00000A1F  98                cbw
00000A20  5B                pop bx
00000A21  03D8              add bx,ax
00000A23  80BFB12EFE        cmp byte [bx+0x2eb1],0xfe
00000A28  7502              jnz 0xa2c
00000A2A  EB53              jmp short 0xa7f
00000A2C  B8FEFF            mov ax,0xfffe
00000A2F  50                push ax
00000A30  A0AA37            mov al,[0x37aa]
00000A33  98                cbw
00000A34  50                push ax
00000A35  A0002F            mov al,[0x2f00]
00000A38  98                cbw
00000A39  50                push ax
00000A3A  A0B02E            mov al,[0x2eb0]
00000A3D  98                cbw
00000A3E  50                push ax
00000A3F  9AD2100906        call 0x609:0x10d2
00000A44  83C408            add sp,byte +0x8
00000A47  B82600            mov ax,0x26
00000A4A  50                push ax
00000A4B  33C0              xor ax,ax
00000A4D  50                push ax
00000A4E  9A5400A721        call 0x21a7:0x54
00000A53  59                pop cx
00000A54  59                pop cx
00000A55  4E                dec si
00000A56  9A1B00960A        call 0xa96:0x1b
00000A5B  A0002F            mov al,[0x2f00]
00000A5E  98                cbw
00000A5F  50                push ax
00000A60  A0B02E            mov al,[0x2eb0]
00000A63  98                cbw
00000A64  50                push ax
00000A65  9AB11C0906        call 0x609:0x1cb1
00000A6A  59                pop cx
00000A6B  59                pop cx
00000A6C  9A0D06FD21        call 0x21fd:0x60d
00000A71  33C0              xor ax,ax
00000A73  BA1900            mov dx,0x19
00000A76  50                push ax
00000A77  52                push dx
00000A78  9A8501FD21        call 0x21fd:0x185
00000A7D  59                pop cx
00000A7E  59                pop cx
00000A7F  0BF6              or si,si
00000A81  7E03              jng 0xa86
00000A83  E903FF            jmp 0x989
00000A86  5F                pop di
00000A87  5E                pop si
00000A88  5D                pop bp
00000A89  CB                retf

; ---- 1F63:0A8A ----
00000A8A  55                push bp
00000A8B  8BEC              mov bp,sp
00000A8D  56                push si
00000A8E  57                push di
00000A8F  A0AA37            mov al,[0x37aa]
00000A92  98                cbw
00000A93  8BD8              mov bx,ax
00000A95  8A87A84B          mov al,[bx+0x4ba8]
00000A99  B400              mov ah,0x0
00000A9B  48                dec ax
00000A9C  8BF8              mov di,ax
00000A9E  803EC85401        cmp byte [0x54c8],0x1
00000AA3  750C              jnz 0xab1
00000AA5  8BC7              mov ax,di
00000AA7  BB0400            mov bx,0x4
00000AAA  99                cwd
00000AAB  F7FB              idiv bx
00000AAD  EB50              jmp short 0xaff
00000AAF  EB4E              jmp short 0xaff
00000AB1  A0AA37            mov al,[0x37aa]
00000AB4  98                cbw
00000AB5  8BD8              mov bx,ax
00000AB7  80BFCE5401        cmp byte [bx+0x54ce],0x1
00000ABC  7536              jnz 0xaf4
00000ABE  33F6              xor si,si
00000AC0  C706D0450000      mov word [0x45d0],0x0
00000AC6  EB19              jmp short 0xae1
00000AC8  A0AA37            mov al,[0x37aa]
00000ACB  98                cbw
00000ACC  D1E0              shl ax,1
00000ACE  D1E0              shl ax,1
00000AD0  0306D045          add ax,[0x45d0]
00000AD4  8BD8              mov bx,ax
00000AD6  8A87E72E          mov al,[bx+0x2ee7]
00000ADA  98                cbw
00000ADB  03F0              add si,ax
00000ADD  FF06D045          inc word [0x45d0]
00000AE1  833ED04503        cmp word [0x45d0],byte +0x3
00000AE6  7EE0              jng 0xac8
00000AE8  8BC6              mov ax,si
00000AEA  BB0400            mov bx,0x4
00000AED  99                cwd
00000AEE  F7FB              idiv bx
00000AF0  EB0D              jmp short 0xaff
00000AF2  EB0B              jmp short 0xaff
00000AF4  8BC7              mov ax,di
00000AF6  40                inc ax
00000AF7  BB0500            mov bx,0x5
00000AFA  99                cwd
00000AFB  F7FB              idiv bx
00000AFD  EB00              jmp short 0xaff
00000AFF  5F                pop di
00000B00  5E                pop si
00000B01  5D                pop bp
00000B02  CB                retf

; ---- 1F63:0B03 ----
00000B03  55                push bp
00000B04  8BEC              mov bp,sp
00000B06  56                push si
00000B07  57                push di
00000B08  A19A4B            mov ax,[0x4b9a]
00000B0B  BA3200            mov dx,0x32
00000B0E  F7EA              imul dx
00000B10  8BF0              mov si,ax
00000B12  A19A4B            mov ax,[0x4b9a]
00000B15  BB0300            mov bx,0x3
00000B18  99                cwd
00000B19  F7FB              idiv bx
00000B1B  83FA02            cmp dx,byte +0x2
00000B1E  7403              jz 0xb23
00000B20  E95B01            jmp 0xc7e
00000B23  FF36D408          push word [0x8d4]
00000B27  9A02006C18        call 0x186c:0x2
00000B2C  59                pop cx
00000B2D  B85000            mov ax,0x50
00000B30  50                push ax
00000B31  9A7D03FD21        call 0x21fd:0x37d
00000B36  59                pop cx
00000B37  9A240D631F        call 0x1f63:0xd24
00000B3C  1E                push ds
00000B3D  B8FC0F            mov ax,0xffc
00000B40  50                push ax
00000B41  9A6C02FD21        call 0x21fd:0x26c
00000B46  59                pop cx
00000B47  59                pop cx
00000B48  9A2903FD21        call 0x21fd:0x329
00000B4D  C606B15401        mov byte [0x54b1],0x1
00000B52  0E                push cs
00000B53  E85CF6            call 0x1b2
00000B56  8BF8              mov di,ax
00000B58  B8FFFF            mov ax,0xffff
00000B5B  50                push ax
00000B5C  B80200            mov ax,0x2
00000B5F  50                push ax
00000B60  B80800            mov ax,0x8
00000B63  50                push ax
00000B64  FF366E3B          push word [0x3b6e]
00000B68  FF366C3B          push word [0x3b6c]
00000B6C  B88F00            mov ax,0x8f
00000B6F  50                push ax
00000B70  B80A00            mov ax,0xa
00000B73  50                push ax
00000B74  B8B100            mov ax,0xb1
00000B77  50                push ax
00000B78  9A4302690A        call 0xa69:0x243
00000B7D  83C410            add sp,byte +0x10
00000B80  B80100            mov ax,0x1
00000B83  50                push ax
00000B84  9ADB05FD21        call 0x21fd:0x5db
00000B89  59                pop cx
00000B8A  33C0              xor ax,ax
00000B8C  BA3200            mov dx,0x32
00000B8F  50                push ax
00000B90  52                push dx
00000B91  9A8501FD21        call 0x21fd:0x185
00000B96  59                pop cx
00000B97  59                pop cx
00000B98  9AAD11631F        call 0x1f63:0x11ad
00000B9D  C606AA3700        mov byte [0x37aa],0x0
00000BA2  E9AB00            jmp 0xc50
00000BA5  A0AA37            mov al,[0x37aa]
00000BA8  98                cbw
00000BA9  8BD8              mov bx,ax
00000BAB  80BF904801        cmp byte [bx+0x4890],0x1
00000BB0  7403              jz 0xbb5
00000BB2  E99700            jmp 0xc4c
00000BB5  9A1B00960A        call 0xa96:0x1b
00000BBA  83FF46            cmp di,byte +0x46
00000BBD  7C07              jl 0xbc6
00000BBF  803E052F00        cmp byte [0x2f05],0x0
00000BC4  753D              jnz 0xc03
00000BC6  B8FFFF            mov ax,0xffff
00000BC9  50                push ax
00000BCA  33C0              xor ax,ax
00000BCC  50                push ax
00000BCD  B80500            mov ax,0x5
00000BD0  50                push ax
00000BD1  33C0              xor ax,ax
00000BD3  50                push ax
00000BD4  56                push si
00000BD5  A0AA37            mov al,[0x37aa]
00000BD8  98                cbw
00000BD9  BA2100            mov dx,0x21
00000BDC  F7EA              imul dx
00000BDE  052200            add ax,0x22
00000BE1  50                push ax
00000BE2  B82900            mov ax,0x29
00000BE5  50                push ax
00000BE6  9AFB05960A        call 0xa96:0x5fb
00000BEB  83C40E            add sp,byte +0xe
00000BEE  A0AA37            mov al,[0x37aa]
00000BF1  98                cbw
00000BF2  D1E0              shl ax,1
00000BF4  D1E0              shl ax,1
00000BF6  8BD8              mov bx,ax
00000BF8  01B7D23C          add [bx+0x3cd2],si
00000BFC  8397D43C00        adc word [bx+0x3cd4],byte +0x0
00000C01  EB28              jmp short 0xc2b
00000C03  B8FFFF            mov ax,0xffff
00000C06  50                push ax
00000C07  33C0              xor ax,ax
00000C09  50                push ax
00000C0A  FF36723B          push word [0x3b72]
00000C0E  FF36703B          push word [0x3b70]
00000C12  A0AA37            mov al,[0x37aa]
00000C15  98                cbw
00000C16  BA2100            mov dx,0x21
00000C19  F7EA              imul dx
00000C1B  052100            add ax,0x21
00000C1E  50                push ax
00000C1F  B82900            mov ax,0x29
00000C22  50                push ax
00000C23  9A2505960A        call 0xa96:0x525
00000C28  83C40C            add sp,byte +0xc
00000C2B  9A0D06FD21        call 0x21fd:0x60d
00000C30  B82800            mov ax,0x28
00000C33  50                push ax
00000C34  33C0              xor ax,ax
00000C36  50                push ax
00000C37  9A5400A721        call 0x21a7:0x54
00000C3C  59                pop cx
00000C3D  59                pop cx
00000C3E  33C0              xor ax,ax
00000C40  BA3200            mov dx,0x32
00000C43  50                push ax
00000C44  52                push dx
00000C45  9A8501FD21        call 0x21fd:0x185
00000C4A  59                pop cx
00000C4B  59                pop cx
00000C4C  FE06AA37          inc byte [0x37aa]
00000C50  803EAA3704        cmp byte [0x37aa],0x4
00000C55  7D03              jnl 0xc5a
00000C57  E94BFF            jmp 0xba5
00000C5A  33C0              xor ax,ax
00000C5C  50                push ax
00000C5D  9ADB05FD21        call 0x21fd:0x5db
00000C62  59                pop cx
00000C63  B80400            mov ax,0x4
00000C66  50                push ax
00000C67  9A0400FD21        call 0x21fd:0x4
00000C6C  59                pop cx
00000C6D  83FF46            cmp di,byte +0x46
00000C70  7C0C              jl 0xc7e
00000C72  803E052F00        cmp byte [0x2f05],0x0
00000C77  7E05              jng 0xc7e
00000C79  9A80000906        call 0x609:0x80
00000C7E  5F                pop di
00000C7F  5E                pop si
00000C80  5D                pop bp
00000C81  CB                retf

; ---- 1F63:0C82 ----
00000C82  55                push bp
00000C83  8BEC              mov bp,sp
00000C85  56                push si
00000C86  33F6              xor si,si
00000C88  C606AA3700        mov byte [0x37aa],0x0
00000C8D  EB47              jmp short 0xcd6
00000C8F  A0AA37            mov al,[0x37aa]
00000C92  98                cbw
00000C93  8BD8              mov bx,ax
00000C95  C687904800        mov byte [bx+0x4890],0x0
00000C9A  803EB15401        cmp byte [0x54b1],0x1
00000C9F  750F              jnz 0xcb0
00000CA1  A0AA37            mov al,[0x37aa]
00000CA4  98                cbw
00000CA5  8BD8              mov bx,ax
00000CA7  80BFC24B01        cmp byte [bx+0x4bc2],0x1
00000CAC  7502              jnz 0xcb0
00000CAE  EB22              jmp short 0xcd2
00000CB0  A0AA37            mov al,[0x37aa]
00000CB3  98                cbw
00000CB4  D1E0              shl ax,1
00000CB6  D1E0              shl ax,1
00000CB8  D1E0              shl ax,1
00000CBA  8BD8              mov bx,ax
00000CBC  39B7FA3C          cmp [bx+0x3cfa],si
00000CC0  7E10              jng 0xcd2
00000CC2  A0AA37            mov al,[0x37aa]
00000CC5  98                cbw
00000CC6  D1E0              shl ax,1
00000CC8  D1E0              shl ax,1
00000CCA  D1E0              shl ax,1
00000CCC  8BD8              mov bx,ax
00000CCE  8BB7FA3C          mov si,[bx+0x3cfa]
00000CD2  FE06AA37          inc byte [0x37aa]
00000CD6  803EAA3704        cmp byte [0x37aa],0x4
00000CDB  7CB2              jl 0xc8f
00000CDD  0BF6              or si,si
00000CDF  7E3C              jng 0xd1d
00000CE1  C606AA3700        mov byte [0x37aa],0x0
00000CE6  EB2E              jmp short 0xd16
00000CE8  A0AA37            mov al,[0x37aa]
00000CEB  98                cbw
00000CEC  D1E0              shl ax,1
00000CEE  D1E0              shl ax,1
00000CF0  D1E0              shl ax,1
00000CF2  8BD8              mov bx,ax
00000CF4  39B7FA3C          cmp [bx+0x3cfa],si
00000CF8  7518              jnz 0xd12
00000CFA  A0AA37            mov al,[0x37aa]
00000CFD  98                cbw
00000CFE  8BD8              mov bx,ax
00000D00  80BFC24B00        cmp byte [bx+0x4bc2],0x0
00000D05  750B              jnz 0xd12
00000D07  A0AA37            mov al,[0x37aa]
00000D0A  98                cbw
00000D0B  8BD8              mov bx,ax
00000D0D  C687904801        mov byte [bx+0x4890],0x1
00000D12  FE06AA37          inc byte [0x37aa]
00000D16  803EAA3704        cmp byte [0x37aa],0x4
00000D1B  7CCB              jl 0xce8
00000D1D  8BC6              mov ax,si
00000D1F  EB00              jmp short 0xd21
00000D21  5E                pop si
00000D22  5D                pop bp
00000D23  CB                retf

; ---- 1F63:0D24 ----
00000D24  55                push bp
00000D25  8BEC              mov bp,sp
00000D27  56                push si
00000D28  9A1B00960A        call 0xa96:0x1b
00000D2D  33F6              xor si,si
00000D2F  EB25              jmp short 0xd56
00000D31  B84300            mov ax,0x43
00000D34  50                push ax
00000D35  8BC6              mov ax,si
00000D37  055800            add ax,0x58
00000D3A  50                push ax
00000D3B  B86D00            mov ax,0x6d
00000D3E  50                push ax
00000D3F  B80100            mov ax,0x1
00000D42  50                push ax
00000D43  B84300            mov ax,0x43
00000D46  50                push ax
00000D47  B85700            mov ax,0x57
00000D4A  2BC6              sub ax,si
00000D4C  50                push ax
00000D4D  9A8900960A        call 0xa96:0x89
00000D52  83C40C            add sp,byte +0xc
00000D55  46                inc si
00000D56  83FE1F            cmp si,byte +0x1f
00000D59  7ED6              jng 0xd31
00000D5B  33C0              xor ax,ax
00000D5D  50                push ax
00000D5E  33C0              xor ax,ax
00000D60  50                push ax
00000D61  B8C800            mov ax,0xc8
00000D64  50                push ax
00000D65  B84001            mov ax,0x140
00000D68  50                push ax
00000D69  B83800            mov ax,0x38
00000D6C  50                push ax
00000D6D  33C0              xor ax,ax
00000D6F  50                push ax
00000D70  9A3000960A        call 0xa96:0x30
00000D75  83C40C            add sp,byte +0xc
00000D78  B80B00            mov ax,0xb
00000D7B  50                push ax
00000D7C  B89500            mov ax,0x95
00000D7F  50                push ax
00000D80  B82100            mov ax,0x21
00000D83  50                push ax
00000D84  B80C00            mov ax,0xc
00000D87  50                push ax
00000D88  B80B00            mov ax,0xb
00000D8B  50                push ax
00000D8C  B82C00            mov ax,0x2c
00000D8F  50                push ax
00000D90  9AE200960A        call 0xa96:0xe2
00000D95  83C40C            add sp,byte +0xc
00000D98  B81600            mov ax,0x16
00000D9B  50                push ax
00000D9C  B87800            mov ax,0x78
00000D9F  50                push ax
00000DA0  B81100            mov ax,0x11
00000DA3  50                push ax
00000DA4  B82800            mov ax,0x28
00000DA7  50                push ax
00000DA8  B81600            mov ax,0x16
00000DAB  50                push ax
00000DAC  B80A00            mov ax,0xa
00000DAF  50                push ax
00000DB0  B84900            mov ax,0x49
00000DB3  50                push ax
00000DB4  B82C00            mov ax,0x2c
00000DB7  50                push ax
00000DB8  9A6702960A        call 0xa96:0x267
00000DBD  83C410            add sp,byte +0x10
00000DC0  B80500            mov ax,0x5
00000DC3  50                push ax
00000DC4  B86D00            mov ax,0x6d
00000DC7  50                push ax
00000DC8  B82700            mov ax,0x27
00000DCB  50                push ax
00000DCC  B82800            mov ax,0x28
00000DCF  50                push ax
00000DD0  B80500            mov ax,0x5
00000DD3  50                push ax
00000DD4  B80A00            mov ax,0xa
00000DD7  50                push ax
00000DD8  B85F00            mov ax,0x5f
00000DDB  50                push ax
00000DDC  B82300            mov ax,0x23
00000DDF  50                push ax
00000DE0  9A6702960A        call 0xa96:0x267
00000DE5  83C410            add sp,byte +0x10
00000DE8  33F6              xor si,si
00000DEA  EB28              jmp short 0xe14
00000DEC  B80B00            mov ax,0xb
00000DEF  50                push ax
00000DF0  8BC6              mov ax,si
00000DF2  BA1600            mov dx,0x16
00000DF5  F7EA              imul dx
00000DF7  053800            add ax,0x38
00000DFA  50                push ax
00000DFB  B80600            mov ax,0x6
00000DFE  50                push ax
00000DFF  B81600            mov ax,0x16
00000E02  50                push ax
00000E03  B80B00            mov ax,0xb
00000E06  50                push ax
00000E07  B82200            mov ax,0x22
00000E0A  50                push ax
00000E0B  9AE200960A        call 0xa96:0xe2
00000E10  83C40C            add sp,byte +0xc
00000E13  46                inc si
00000E14  83FE04            cmp si,byte +0x4
00000E17  7ED3              jng 0xdec
00000E19  33F6              xor si,si
00000E1B  EB28              jmp short 0xe45
00000E1D  8BC6              mov ax,si
00000E1F  BA2100            mov dx,0x21
00000E22  F7EA              imul dx
00000E24  052C00            add ax,0x2c
00000E27  50                push ax
00000E28  B82200            mov ax,0x22
00000E2B  50                push ax
00000E2C  B82100            mov ax,0x21
00000E2F  50                push ax
00000E30  B87F00            mov ax,0x7f
00000E33  50                push ax
00000E34  B80B00            mov ax,0xb
00000E37  50                push ax
00000E38  B82200            mov ax,0x22
00000E3B  50                push ax
00000E3C  9AE200960A        call 0xa96:0xe2
00000E41  83C40C            add sp,byte +0xc
00000E44  46                inc si
00000E45  83FE02            cmp si,byte +0x2
00000E48  7ED3              jng 0xe1d
00000E4A  9AA806960A        call 0xa96:0x6a8
00000E4F  FF362E08          push word [0x82e]
00000E53  9A3F0C960A        call 0xa96:0xc3f
00000E58  59                pop cx
00000E59  5E                pop si
00000E5A  5D                pop bp
00000E5B  CB                retf

; ---- 1F63:0E5C ----
00000E5C  55                push bp
00000E5D  8BEC              mov bp,sp
00000E5F  8B4606            mov ax,[bp+0x6]
00000E62  BA2100            mov dx,0x21
00000E65  F7EA              imul dx
00000E67  051100            add ax,0x11
00000E6A  50                push ax
00000E6B  8B4608            mov ax,[bp+0x8]
00000E6E  052700            add ax,0x27
00000E71  50                push ax
00000E72  B81400            mov ax,0x14
00000E75  50                push ax
00000E76  B80100            mov ax,0x1
00000E79  50                push ax
00000E7A  B84900            mov ax,0x49
00000E7D  50                push ax
00000E7E  8B4606            mov ax,[bp+0x6]
00000E81  052800            add ax,0x28
00000E84  50                push ax
00000E85  9A3000960A        call 0xa96:0x30
00000E8A  83C40C            add sp,byte +0xc
00000E8D  5D                pop bp
00000E8E  CB                retf

; ---- 1F63:0E8F ----
00000E8F  55                push bp
00000E90  8BEC              mov bp,sp
00000E92  B81000            mov ax,0x10
00000E95  50                push ax
00000E96  FF7606            push word [bp+0x6]
00000E99  A0AA37            mov al,[0x37aa]
00000E9C  98                cbw
00000E9D  BA2100            mov dx,0x21
00000EA0  F7EA              imul dx
00000EA2  051400            add ax,0x14
00000EA5  50                push ax
00000EA6  B8A000            mov ax,0xa0
00000EA9  2B4606            sub ax,[bp+0x6]
00000EAC  50                push ax
00000EAD  B81000            mov ax,0x10
00000EB0  50                push ax
00000EB1  B80A00            mov ax,0xa
00000EB4  50                push ax
00000EB5  B84C00            mov ax,0x4c
00000EB8  50                push ax
00000EB9  B82C00            mov ax,0x2c
00000EBC  50                push ax
00000EBD  9A6702960A        call 0xa96:0x267
00000EC2  83C410            add sp,byte +0x10
00000EC5  5D                pop bp
00000EC6  CB                retf

; ---- 1F63:0EC7 ----
00000EC7  55                push bp
00000EC8  8BEC              mov bp,sp
00000ECA  A0AA37            mov al,[0x37aa]
00000ECD  98                cbw
00000ECE  BA2100            mov dx,0x21
00000ED1  F7EA              imul dx
00000ED3  052800            add ax,0x28
00000ED6  50                push ax
00000ED7  B82800            mov ax,0x28
00000EDA  50                push ax
00000EDB  B80700            mov ax,0x7
00000EDE  50                push ax
00000EDF  B81400            mov ax,0x14
00000EE2  50                push ax
00000EE3  A0AA37            mov al,[0x37aa]
00000EE6  98                cbw
00000EE7  BA2100            mov dx,0x21
00000EEA  F7EA              imul dx
00000EEC  052800            add ax,0x28
00000EEF  50                push ax
00000EF0  B84600            mov ax,0x46
00000EF3  50                push ax
00000EF4  9AE200960A        call 0xa96:0xe2
00000EF9  83C40C            add sp,byte +0xc
00000EFC  B80100            mov ax,0x1
00000EFF  50                push ax
00000F00  B8FFFF            mov ax,0xffff
00000F03  50                push ax
00000F04  33C0              xor ax,ax
00000F06  50                push ax
00000F07  A0AA37            mov al,[0x37aa]
00000F0A  98                cbw
00000F0B  8BD8              mov bx,ax
00000F0D  8A871C10          mov al,[bx+0x101c]
00000F11  B400              mov ah,0x0
00000F13  50                push ax
00000F14  B80300            mov ax,0x3
00000F17  50                push ax
00000F18  A0AA37            mov al,[0x37aa]
00000F1B  98                cbw
00000F1C  D1E0              shl ax,1
00000F1E  D1E0              shl ax,1
00000F20  D1E0              shl ax,1
00000F22  8BD8              mov bx,ax
00000F24  8B87FA3C          mov ax,[bx+0x3cfa]
00000F28  99                cwd
00000F29  52                push dx
00000F2A  50                push ax
00000F2B  A0AA37            mov al,[0x37aa]
00000F2E  98                cbw
00000F2F  BA2100            mov dx,0x21
00000F32  F7EA              imul dx
00000F34  052D00            add ax,0x2d
00000F37  50                push ax
00000F38  B82900            mov ax,0x29
00000F3B  50                push ax
00000F3C  9A1B01690A        call 0xa69:0x11b
00000F41  83C412            add sp,byte +0x12
00000F44  5D                pop bp
00000F45  CB                retf

; ---- 1F63:0F46 ----
00000F46  55                push bp
00000F47  8BEC              mov bp,sp
00000F49  B80100            mov ax,0x1
00000F4C  50                push ax
00000F4D  B8FFFF            mov ax,0xffff
00000F50  50                push ax
00000F51  33C0              xor ax,ax
00000F53  50                push ax
00000F54  A0AA37            mov al,[0x37aa]
00000F57  98                cbw
00000F58  8BD8              mov bx,ax
00000F5A  8A871C10          mov al,[bx+0x101c]
00000F5E  B400              mov ah,0x0
00000F60  50                push ax
00000F61  FF36A23B          push word [0x3ba2]
00000F65  FF36A03B          push word [0x3ba0]
00000F69  A0AA37            mov al,[0x37aa]
00000F6C  98                cbw
00000F6D  BA2100            mov dx,0x21
00000F70  F7EA              imul dx
00000F72  052C00            add ax,0x2c
00000F75  50                push ax
00000F76  B82900            mov ax,0x29
00000F79  50                push ax
00000F7A  9A4D00690A        call 0xa69:0x4d
00000F7F  83C410            add sp,byte +0x10
00000F82  5D                pop bp
00000F83  CB                retf

; ---- 1F63:0F84 ----
00000F84  55                push bp
00000F85  8BEC              mov bp,sp
00000F87  9A1B00960A        call 0xa96:0x1b
00000F8C  8B4606            mov ax,[bp+0x6]
00000F8F  0BC0              or ax,ax
00000F91  740C              jz 0xf9f
00000F93  3D0100            cmp ax,0x1
00000F96  7417              jz 0xfaf
00000F98  3D0200            cmp ax,0x2
00000F9B  7422              jz 0xfbf
00000F9D  EB39              jmp short 0xfd8
00000F9F  B001              mov al,0x1
00000FA1  50                push ax
00000FA2  B80300            mov ax,0x3
00000FA5  50                push ax
00000FA6  9A4B10631F        call 0x1f63:0x104b
00000FAB  59                pop cx
00000FAC  59                pop cx
00000FAD  EB29              jmp short 0xfd8
00000FAF  B002              mov al,0x2
00000FB1  50                push ax
00000FB2  B80300            mov ax,0x3
00000FB5  50                push ax
00000FB6  9A4B10631F        call 0x1f63:0x104b
00000FBB  59                pop cx
00000FBC  59                pop cx
00000FBD  EB19              jmp short 0xfd8
00000FBF  B002              mov al,0x2
00000FC1  50                push ax
00000FC2  B80300            mov ax,0x3
00000FC5  50                push ax
00000FC6  9A4B10631F        call 0x1f63:0x104b
00000FCB  59                pop cx
00000FCC  59                pop cx
00000FCD  B001              mov al,0x1
00000FCF  50                push ax
00000FD0  9ADF0F631F        call 0x1f63:0xfdf
00000FD5  59                pop cx
00000FD6  EB00              jmp short 0xfd8
00000FD8  9A8511631F        call 0x1f63:0x1185
00000FDD  5D                pop bp
00000FDE  CB                retf

; ---- 1F63:0FDF ----
00000FDF  55                push bp
00000FE0  8BEC              mov bp,sp
00000FE2  B84600            mov ax,0x46
00000FE5  50                push ax
00000FE6  B8C600            mov ax,0xc6
00000FE9  50                push ax
00000FEA  B82600            mov ax,0x26
00000FED  50                push ax
00000FEE  B81A00            mov ax,0x1a
00000FF1  50                push ax
00000FF2  8A4606            mov al,[bp+0x6]
00000FF5  B400              mov ah,0x0
00000FF7  8BD8              mov bx,ax
00000FF9  8A872610          mov al,[bx+0x1026]
00000FFD  B400              mov ah,0x0
00000FFF  50                push ax
00001000  8A4606            mov al,[bp+0x6]
00001003  B400              mov ah,0x0
00001005  8BD8              mov bx,ax
00001007  8A872410          mov al,[bx+0x1024]
0000100B  B400              mov ah,0x0
0000100D  50                push ax
0000100E  9A3000960A        call 0xa96:0x30
00001013  83C40C            add sp,byte +0xc
00001016  B84600            mov ax,0x46
00001019  50                push ax
0000101A  B80401            mov ax,0x104
0000101D  50                push ax
0000101E  B82600            mov ax,0x26
00001021  50                push ax
00001022  B81A00            mov ax,0x1a
00001025  50                push ax
00001026  8A4606            mov al,[bp+0x6]
00001029  B400              mov ah,0x0
0000102B  8BD8              mov bx,ax
0000102D  8A872C10          mov al,[bx+0x102c]
00001031  B400              mov ah,0x0
00001033  50                push ax
00001034  8A4606            mov al,[bp+0x6]
00001037  B400              mov ah,0x0
00001039  D1E0              shl ax,1
0000103B  8BD8              mov bx,ax
0000103D  FFB72810          push word [bx+0x1028]
00001041  9A3000960A        call 0xa96:0x30
00001046  83C40C            add sp,byte +0xc
00001049  5D                pop bp
0000104A  CB                retf

; ---- 1F63:104B ----
0000104B  55                push bp
0000104C  8BEC              mov bp,sp
0000104E  F746060100        test word [bp+0x6],0x1
00001053  7470              jz 0x10c5
00001055  807E0801          cmp byte [bp+0x8],0x1
00001059  7522              jnz 0x107d
0000105B  B83200            mov ax,0x32
0000105E  50                push ax
0000105F  B8D000            mov ax,0xd0
00001062  50                push ax
00001063  B82800            mov ax,0x28
00001066  50                push ax
00001067  B82000            mov ax,0x20
0000106A  50                push ax
0000106B  B86A00            mov ax,0x6a
0000106E  50                push ax
0000106F  B8D000            mov ax,0xd0
00001072  50                push ax
00001073  9A3000960A        call 0xa96:0x30
00001078  83C40C            add sp,byte +0xc
0000107B  EB48              jmp short 0x10c5
0000107D  807E0802          cmp byte [bp+0x8],0x2
00001081  7522              jnz 0x10a5
00001083  B83200            mov ax,0x32
00001086  50                push ax
00001087  B8D000            mov ax,0xd0
0000108A  50                push ax
0000108B  B82800            mov ax,0x28
0000108E  50                push ax
0000108F  B82000            mov ax,0x20
00001092  50                push ax
00001093  B84300            mov ax,0x43
00001096  50                push ax
00001097  B83800            mov ax,0x38
0000109A  50                push ax
0000109B  9A3000960A        call 0xa96:0x30
000010A0  83C40C            add sp,byte +0xc
000010A3  EB20              jmp short 0x10c5
000010A5  B83B00            mov ax,0x3b
000010A8  50                push ax
000010A9  B8D000            mov ax,0xd0
000010AC  50                push ax
000010AD  B81F00            mov ax,0x1f
000010B0  50                push ax
000010B1  B82000            mov ax,0x20
000010B4  50                push ax
000010B5  B86B00            mov ax,0x6b
000010B8  50                push ax
000010B9  B83800            mov ax,0x38
000010BC  50                push ax
000010BD  9A3000960A        call 0xa96:0x30
000010C2  83C40C            add sp,byte +0xc
000010C5  F746060200        test word [bp+0x6],0x2
000010CA  7470              jz 0x113c
000010CC  807E0801          cmp byte [bp+0x8],0x1
000010D0  7522              jnz 0x10f4
000010D2  B83200            mov ax,0x32
000010D5  50                push ax
000010D6  B8F400            mov ax,0xf4
000010D9  50                push ax
000010DA  B82800            mov ax,0x28
000010DD  50                push ax
000010DE  B82000            mov ax,0x20
000010E1  50                push ax
000010E2  B86A00            mov ax,0x6a
000010E5  50                push ax
000010E6  B8F400            mov ax,0xf4
000010E9  50                push ax
000010EA  9A3000960A        call 0xa96:0x30
000010EF  83C40C            add sp,byte +0xc
000010F2  EB48              jmp short 0x113c
000010F4  807E0802          cmp byte [bp+0x8],0x2
000010F8  7522              jnz 0x111c
000010FA  B83200            mov ax,0x32
000010FD  50                push ax
000010FE  B8F400            mov ax,0xf4
00001101  50                push ax
00001102  B82800            mov ax,0x28
00001105  50                push ax
00001106  B82000            mov ax,0x20
00001109  50                push ax
0000110A  B84300            mov ax,0x43
0000110D  50                push ax
0000110E  B85800            mov ax,0x58
00001111  50                push ax
00001112  9A3000960A        call 0xa96:0x30
00001117  83C40C            add sp,byte +0xc
0000111A  EB20              jmp short 0x113c
0000111C  B83B00            mov ax,0x3b
0000111F  50                push ax
00001120  B8F400            mov ax,0xf4
00001123  50                push ax
00001124  B81F00            mov ax,0x1f
00001127  50                push ax
00001128  B82000            mov ax,0x20
0000112B  50                push ax
0000112C  B86B00            mov ax,0x6b
0000112F  50                push ax
00001130  B85800            mov ax,0x58
00001133  50                push ax
00001134  9A3000960A        call 0xa96:0x30
00001139  83C40C            add sp,byte +0xc
0000113C  5D                pop bp
0000113D  CB                retf

; ---- 1F63:113E ----
0000113E  55                push bp
0000113F  8BEC              mov bp,sp
00001141  803EB05400        cmp byte [0x54b0],0x0
00001146  7E1B              jng 0x1163
00001148  A0B054            mov al,[0x54b0]
0000114B  FEC8              dec al
0000114D  A2B054            mov [0x54b0],al
00001150  0AC0              or al,al
00001152  750D              jnz 0x1161
00001154  B001              mov al,0x1
00001156  50                push ax
00001157  B80300            mov ax,0x3
0000115A  50                push ax
0000115B  0E                push cs
0000115C  E8ECFE            call 0x104b
0000115F  59                pop cx
00001160  59                pop cx
00001161  EB20              jmp short 0x1183
00001163  B8C800            mov ax,0xc8
00001166  50                push ax
00001167  9ABA01FD21        call 0x21fd:0x1ba
0000116C  59                pop cx
0000116D  0BC0              or ax,ax
0000116F  7512              jnz 0x1183
00001171  B000              mov al,0x0
00001173  50                push ax
00001174  B80300            mov ax,0x3
00001177  50                push ax
00001178  0E                push cs
00001179  E8CFFE            call 0x104b
0000117C  59                pop cx
0000117D  59                pop cx
0000117E  C606B05408        mov byte [0x54b0],0x8
00001183  5D                pop bp
00001184  CB                retf

; ---- 1F63:1185 ----
00001185  55                push bp
00001186  8BEC              mov bp,sp
00001188  9AA806960A        call 0xa96:0x6a8
0000118D  FF362E08          push word [0x82e]
00001191  9AE906CD23        call 0x23cd:0x6e9
00001196  59                pop cx
00001197  9A1F05FD21        call 0x21fd:0x51f
0000119C  FF362E08          push word [0x82e]
000011A0  9A1C07CD23        call 0x23cd:0x71c
000011A5  59                pop cx
000011A6  9AA806960A        call 0xa96:0x6a8
000011AB  5D                pop bp
000011AC  CB                retf

; ---- 1F63:11AD ----
000011AD  55                push bp
000011AE  8BEC              mov bp,sp
000011B0  56                push si
000011B1  33F6              xor si,si
000011B3  E98600            jmp 0x123c
000011B6  C606AA3700        mov byte [0x37aa],0x0
000011BB  EB69              jmp short 0x1226
000011BD  A0AA37            mov al,[0x37aa]
000011C0  98                cbw
000011C1  8BD8              mov bx,ax
000011C3  80BF904801        cmp byte [bx+0x4890],0x1
000011C8  7558              jnz 0x1222
000011CA  8BC6              mov ax,si
000011CC  BB0200            mov bx,0x2
000011CF  99                cwd
000011D0  F7FB              idiv bx
000011D2  0BD2              or dx,dx
000011D4  7523              jnz 0x11f9
000011D6  B8FF0F            mov ax,0xfff
000011D9  50                push ax
000011DA  A0AA37            mov al,[0x37aa]
000011DD  98                cbw
000011DE  050C00            add ax,0xc
000011E1  50                push ax
000011E2  9A6D009522        call 0x2295:0x6d
000011E7  59                pop cx
000011E8  59                pop cx
000011E9  B82900            mov ax,0x29
000011EC  50                push ax
000011ED  33C0              xor ax,ax
000011EF  50                push ax
000011F0  9A5400A721        call 0x21a7:0x54
000011F5  59                pop cx
000011F6  59                pop cx
000011F7  EB29              jmp short 0x1222
000011F9  A0AA37            mov al,[0x37aa]
000011FC  98                cbw
000011FD  D1E0              shl ax,1
000011FF  8BD8              mov bx,ax
00001201  FFB71410          push word [bx+0x1014]
00001205  A0AA37            mov al,[0x37aa]
00001208  98                cbw
00001209  050C00            add ax,0xc
0000120C  50                push ax
0000120D  9A6D009522        call 0x2295:0x6d
00001212  59                pop cx
00001213  59                pop cx
00001214  B82A00            mov ax,0x2a
00001217  50                push ax
00001218  33C0              xor ax,ax
0000121A  50                push ax
0000121B  9A5400A721        call 0x21a7:0x54
00001220  59                pop cx
00001221  59                pop cx
00001222  FE06AA37          inc byte [0x37aa]
00001226  803EAA3704        cmp byte [0x37aa],0x4
0000122B  7C90              jl 0x11bd
0000122D  33C0              xor ax,ax
0000122F  BA0F00            mov dx,0xf
00001232  50                push ax
00001233  52                push dx
00001234  9A8501FD21        call 0x21fd:0x185
00001239  59                pop cx
0000123A  59                pop cx
0000123B  46                inc si
0000123C  83FE07            cmp si,byte +0x7
0000123F  7F03              jg 0x1244
00001241  E972FF            jmp 0x11b6
00001244  5E                pop si
00001245  5D                pop bp
00001246  CB                retf

; ---- 1F63:1247 ----
00001247  55                push bp
00001248  8BEC              mov bp,sp
0000124A  C7060E338200      mov word [0x330e],0x82
00001250  B84600            mov ax,0x46
00001253  50                push ax
00001254  B82001            mov ax,0x120
00001257  50                push ax
00001258  B84600            mov ax,0x46
0000125B  50                push ax
0000125C  33C0              xor ax,ax
0000125E  50                push ax
0000125F  33C0              xor ax,ax
00001261  50                push ax
00001262  9A2B02AF21        call 0x21af:0x22b
00001267  83C40A            add sp,byte +0xa
0000126A  33C0              xor ax,ax
0000126C  50                push ax
0000126D  B83001            mov ax,0x130
00001270  50                push ax
00001271  B84300            mov ax,0x43
00001274  50                push ax
00001275  B89000            mov ax,0x90
00001278  50                push ax
00001279  33C0              xor ax,ax
0000127B  50                push ax
0000127C  33C0              xor ax,ax
0000127E  50                push ax
0000127F  9ACA00AF21        call 0x21af:0xca
00001284  83C40C            add sp,byte +0xc
00001287  C70628331E00      mov word [0x3328],0x1e
0000128D  B8E200            mov ax,0xe2
00001290  50                push ax
00001291  B8E000            mov ax,0xe0
00001294  50                push ax
00001295  B8C400            mov ax,0xc4
00001298  50                push ax
00001299  B8E000            mov ax,0xe0
0000129C  50                push ax
0000129D  B80100            mov ax,0x1
000012A0  50                push ax
000012A1  9A2B02AF21        call 0x21af:0x22b
000012A6  83C40A            add sp,byte +0xa
000012A9  33C0              xor ax,ax
000012AB  50                push ax
000012AC  B8D000            mov ax,0xd0
000012AF  50                push ax
000012B0  33C0              xor ax,ax
000012B2  50                push ax
000012B3  33C0              xor ax,ax
000012B5  50                push ax
000012B6  33C0              xor ax,ax
000012B8  50                push ax
000012B9  B80100            mov ax,0x1
000012BC  50                push ax
000012BD  9ACA00AF21        call 0x21af:0xca
000012C2  83C40C            add sp,byte +0xc
000012C5  C70642331E00      mov word [0x3342],0x1e
000012CB  B8E200            mov ax,0xe2
000012CE  50                push ax
000012CF  B80001            mov ax,0x100
000012D2  50                push ax
000012D3  B8C400            mov ax,0xc4
000012D6  50                push ax
000012D7  B80001            mov ax,0x100
000012DA  50                push ax
000012DB  B80200            mov ax,0x2
000012DE  50                push ax
000012DF  9A2B02AF21        call 0x21af:0x22b
000012E4  83C40A            add sp,byte +0xa
000012E7  33C0              xor ax,ax
000012E9  50                push ax
000012EA  B8E000            mov ax,0xe0
000012ED  50                push ax
000012EE  33C0              xor ax,ax
000012F0  50                push ax
000012F1  B81000            mov ax,0x10
000012F4  50                push ax
000012F5  33C0              xor ax,ax
000012F7  50                push ax
000012F8  B80200            mov ax,0x2
000012FB  50                push ax
000012FC  9ACA00AF21        call 0x21af:0xca
00001301  83C40C            add sp,byte +0xc
00001304  C7065C331E00      mov word [0x335c],0x1e
0000130A  B81E00            mov ax,0x1e
0000130D  50                push ax
0000130E  B8C000            mov ax,0xc0
00001311  50                push ax
00001312  33C0              xor ax,ax
00001314  50                push ax
00001315  B8C000            mov ax,0xc0
00001318  50                push ax
00001319  B80300            mov ax,0x3
0000131C  50                push ax
0000131D  9A2B02AF21        call 0x21af:0x22b
00001322  83C40A            add sp,byte +0xa
00001325  33C0              xor ax,ax
00001327  50                push ax
00001328  B8F000            mov ax,0xf0
0000132B  50                push ax
0000132C  33C0              xor ax,ax
0000132E  50                push ax
0000132F  B82000            mov ax,0x20
00001332  50                push ax
00001333  33C0              xor ax,ax
00001335  50                push ax
00001336  B80300            mov ax,0x3
00001339  50                push ax
0000133A  9ACA00AF21        call 0x21af:0xca
0000133F  83C40C            add sp,byte +0xc
00001342  C70676331E00      mov word [0x3376],0x1e
00001348  B81E00            mov ax,0x1e
0000134B  50                push ax
0000134C  B8E000            mov ax,0xe0
0000134F  50                push ax
00001350  33C0              xor ax,ax
00001352  50                push ax
00001353  B8E000            mov ax,0xe0
00001356  50                push ax
00001357  B80400            mov ax,0x4
0000135A  50                push ax
0000135B  9A2B02AF21        call 0x21af:0x22b
00001360  83C40A            add sp,byte +0xa
00001363  33C0              xor ax,ax
00001365  50                push ax
00001366  B80001            mov ax,0x100
00001369  50                push ax
0000136A  33C0              xor ax,ax
0000136C  50                push ax
0000136D  B83000            mov ax,0x30
00001370  50                push ax
00001371  33C0              xor ax,ax
00001373  50                push ax
00001374  B80400            mov ax,0x4
00001377  50                push ax
00001378  9ACA00AF21        call 0x21af:0xca
0000137D  83C40C            add sp,byte +0xc
00001380  C70690331E00      mov word [0x3390],0x1e
00001386  B81E00            mov ax,0x1e
00001389  50                push ax
0000138A  B80001            mov ax,0x100
0000138D  50                push ax
0000138E  33C0              xor ax,ax
00001390  50                push ax
00001391  B80001            mov ax,0x100
00001394  50                push ax
00001395  B80500            mov ax,0x5
00001398  50                push ax
00001399  9A2B02AF21        call 0x21af:0x22b
0000139E  83C40A            add sp,byte +0xa
000013A1  33C0              xor ax,ax
000013A3  50                push ax
000013A4  B81001            mov ax,0x110
000013A7  50                push ax
000013A8  33C0              xor ax,ax
000013AA  50                push ax
000013AB  B84000            mov ax,0x40
000013AE  50                push ax
000013AF  33C0              xor ax,ax
000013B1  50                push ax
000013B2  B80500            mov ax,0x5
000013B5  50                push ax
000013B6  9ACA00AF21        call 0x21af:0xca
000013BB  83C40C            add sp,byte +0xc
000013BE  C706AA331E00      mov word [0x33aa],0x1e
000013C4  B81E00            mov ax,0x1e
000013C7  50                push ax
000013C8  B82001            mov ax,0x120
000013CB  50                push ax
000013CC  33C0              xor ax,ax
000013CE  50                push ax
000013CF  B82001            mov ax,0x120
000013D2  50                push ax
000013D3  B80600            mov ax,0x6
000013D6  50                push ax
000013D7  9A2B02AF21        call 0x21af:0x22b
000013DC  83C40A            add sp,byte +0xa
000013DF  33C0              xor ax,ax
000013E1  50                push ax
000013E2  B82001            mov ax,0x120
000013E5  50                push ax
000013E6  33C0              xor ax,ax
000013E8  50                push ax
000013E9  B85000            mov ax,0x50
000013EC  50                push ax
000013ED  33C0              xor ax,ax
000013EF  50                push ax
000013F0  B80600            mov ax,0x6
000013F3  50                push ax
000013F4  9ACA00AF21        call 0x21af:0xca
000013F9  83C40C            add sp,byte +0xc
000013FC  5D                pop bp
000013FD  CB                retf

; ---- 1F63:13FE ----
000013FE  55                push bp
000013FF  8BEC              mov bp,sp
00001401  56                push si
00001402  C706CA549400      mov word [0x54ca],0x94
00001408  9A3E14631F        call 0x1f63:0x143e
0000140D  33F6              xor si,si
0000140F  EB0C              jmp short 0x141d
00001411  33C0              xor ax,ax
00001413  50                push ax
00001414  56                push si
00001415  9A1204AF21        call 0x21af:0x412
0000141A  59                pop cx
0000141B  59                pop cx
0000141C  46                inc si
0000141D  83FE07            cmp si,byte +0x7
00001420  7CEF              jl 0x1411
00001422  FF362E08          push word [0x82e]
00001426  9AE906CD23        call 0x23cd:0x6e9
0000142B  59                pop cx
0000142C  9A1F05FD21        call 0x21fd:0x51f
00001431  FF362E08          push word [0x82e]
00001435  9AE906CD23        call 0x23cd:0x6e9
0000143A  59                pop cx
0000143B  5E                pop si
0000143C  5D                pop bp
0000143D  CB                retf

; ---- 1F63:143E ----
0000143E  55                push bp
0000143F  8BEC              mov bp,sp
00001441  56                push si
00001442  33F6              xor si,si
00001444  EB2D              jmp short 0x1473
00001446  8BC6              mov ax,si
00001448  BA1A00            mov dx,0x1a
0000144B  F7EA              imul dx
0000144D  8A942E10          mov dl,[si+0x102e]
00001451  B600              mov dh,0x0
00001453  8B1ECA54          mov bx,[0x54ca]
00001457  03DA              add bx,dx
00001459  87D8              xchg bx,ax
0000145B  89872033          mov [bx+0x3320],ax
0000145F  8BC6              mov ax,si
00001461  BA1A00            mov dx,0x1a
00001464  F7EA              imul dx
00001466  8A943510          mov dl,[si+0x1035]
0000146A  B600              mov dh,0x0
0000146C  8BD8              mov bx,ax
0000146E  89972233          mov [bx+0x3322],dx
00001472  46                inc si
00001473  83FE07            cmp si,byte +0x7
00001476  7CCE              jl 0x1446
00001478  5E                pop si
00001479  5D                pop bp
0000147A  CB                retf

; ---- 1F63:147B ----
0000147B  55                push bp
0000147C  8BEC              mov bp,sp
0000147E  4C                dec sp
0000147F  4C                dec sp
00001480  56                push si
00001481  57                push di
00001482  8B4606            mov ax,[bp+0x6]
00001485  99                cwd
00001486  F73EB254          idiv word [0x54b2]
0000148A  051400            add ax,0x14
0000148D  8BF8              mov di,ax
0000148F  81FF0001          cmp di,0x100
00001493  7E03              jng 0x1498
00001495  BF0001            mov di,0x100
00001498  8BC7              mov ax,di
0000149A  2B06CA54          sub ax,[0x54ca]
0000149E  8946FE            mov [bp-0x2],ax
000014A1  837EFE00          cmp word [bp-0x2],byte +0x0
000014A5  7502              jnz 0x14a9
000014A7  EB56              jmp short 0x14ff
000014A9  8B46FE            mov ax,[bp-0x2]
000014AC  BB0800            mov bx,0x8
000014AF  99                cwd
000014B0  F7FB              idiv bx
000014B2  8BF0              mov si,ax
000014B4  0BF6              or si,si
000014B6  7512              jnz 0x14ca
000014B8  837EFE00          cmp word [bp-0x2],byte +0x0
000014BC  7D05              jnl 0x14c3
000014BE  B8FFFF            mov ax,0xffff
000014C1  EB03              jmp short 0x14c6
000014C3  B80100            mov ax,0x1
000014C6  8BF0              mov si,ax
000014C8  EB12              jmp short 0x14dc
000014CA  83FE04            cmp si,byte +0x4
000014CD  7E05              jng 0x14d4
000014CF  BE0400            mov si,0x4
000014D2  EB08              jmp short 0x14dc
000014D4  83FEFC            cmp si,byte -0x4
000014D7  7D03              jnl 0x14dc
000014D9  BEFCFF            mov si,0xfffc
000014DC  0136CA54          add [0x54ca],si
000014E0  0E                push cs
000014E1  E85AFF            call 0x143e
000014E4  FF362E08          push word [0x82e]
000014E8  9AE906CD23        call 0x23cd:0x6e9
000014ED  59                pop cx
000014EE  9A1F05FD21        call 0x21fd:0x51f
000014F3  FF362E08          push word [0x82e]
000014F7  9A1C07CD23        call 0x23cd:0x71c
000014FC  59                pop cx
000014FD  EB99              jmp short 0x1498
000014FF  5F                pop di
