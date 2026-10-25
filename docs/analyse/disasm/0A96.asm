; segment 0A96  file offset 0xDF60  size 0xD10
00000000  10463B            adc [bp+0x3b],al
00000003  7610              jna 0x15
00000005  7CB9              jl 0xffc0
00000007  5E                pop si
00000008  5D                pop bp
00000009  CB                retf

; ---- 0A96:000A ----
0000000A  55                push bp
0000000B  8BEC              mov bp,sp
0000000D  8A4606            mov al,[bp+0x6]
00000010  A24201            mov [0x142],al
00000013  8A4608            mov al,[bp+0x8]
00000016  A24301            mov [0x143],al
00000019  5D                pop bp
0000001A  CB                retf

; ---- 0A96:001B ----
0000001B  55                push bp
0000001C  8BEC              mov bp,sp
0000001E  C706BE150000      mov word [0x15be],0x0
00000024  C706BA15C015      mov word [0x15ba],0x15c0
0000002A  8C1EBC15          mov [0x15bc],ds
0000002E  5D                pop bp
0000002F  CB                retf

; ---- 0A96:0030 ----
00000030  55                push bp
00000031  8BEC              mov bp,sp
00000033  C41EBA15          les bx,[0x15ba]
00000037  8B4606            mov ax,[bp+0x6]
0000003A  268907            mov [es:bx],ax
0000003D  C41EBA15          les bx,[0x15ba]
00000041  8B4608            mov ax,[bp+0x8]
00000044  26894702          mov [es:bx+0x2],ax
00000048  C41EBA15          les bx,[0x15ba]
0000004C  8B460A            mov ax,[bp+0xa]
0000004F  2689470C          mov [es:bx+0xc],ax
00000053  C41EBA15          les bx,[0x15ba]
00000057  8B460C            mov ax,[bp+0xc]
0000005A  2689470E          mov [es:bx+0xe],ax
0000005E  C41EBA15          les bx,[0x15ba]
00000062  8B460E            mov ax,[bp+0xe]
00000065  26894704          mov [es:bx+0x4],ax
00000069  C41EBA15          les bx,[0x15ba]
0000006D  8B4610            mov ax,[bp+0x10]
00000070  26894706          mov [es:bx+0x6],ax
00000074  C41EBA15          les bx,[0x15ba]
00000078  26C7471A0000      mov word [es:bx+0x1a],0x0
0000007E  FF06BE15          inc word [0x15be]
00000082  8306BA151C        add word [0x15ba],byte +0x1c
00000087  5D                pop bp
00000088  CB                retf

; ---- 0A96:0089 ----
00000089  55                push bp
0000008A  8BEC              mov bp,sp
0000008C  C41EBA15          les bx,[0x15ba]
00000090  8B4606            mov ax,[bp+0x6]
00000093  268907            mov [es:bx],ax
00000096  C41EBA15          les bx,[0x15ba]
0000009A  8B4608            mov ax,[bp+0x8]
0000009D  26894702          mov [es:bx+0x2],ax
000000A1  C41EBA15          les bx,[0x15ba]
000000A5  8B460A            mov ax,[bp+0xa]
000000A8  2689470C          mov [es:bx+0xc],ax
000000AC  C41EBA15          les bx,[0x15ba]
000000B0  8B460C            mov ax,[bp+0xc]
000000B3  2689470E          mov [es:bx+0xe],ax
000000B7  C41EBA15          les bx,[0x15ba]
000000BB  8B460E            mov ax,[bp+0xe]
000000BE  26894704          mov [es:bx+0x4],ax
000000C2  C41EBA15          les bx,[0x15ba]
000000C6  8B4610            mov ax,[bp+0x10]
000000C9  26894706          mov [es:bx+0x6],ax
000000CD  C41EBA15          les bx,[0x15ba]
000000D1  26C7471A0100      mov word [es:bx+0x1a],0x1
000000D7  FF06BE15          inc word [0x15be]
000000DB  8306BA151C        add word [0x15ba],byte +0x1c
000000E0  5D                pop bp
000000E1  CB                retf

; ---- 0A96:00E2 ----
000000E2  55                push bp
000000E3  8BEC              mov bp,sp
000000E5  C41EBA15          les bx,[0x15ba]
000000E9  8B4606            mov ax,[bp+0x6]
000000EC  268907            mov [es:bx],ax
000000EF  C41EBA15          les bx,[0x15ba]
000000F3  8B4608            mov ax,[bp+0x8]
000000F6  26894702          mov [es:bx+0x2],ax
000000FA  C41EBA15          les bx,[0x15ba]
000000FE  8B460A            mov ax,[bp+0xa]
00000101  2689470C          mov [es:bx+0xc],ax
00000105  C41EBA15          les bx,[0x15ba]
00000109  8B460C            mov ax,[bp+0xc]
0000010C  2689470E          mov [es:bx+0xe],ax
00000110  C41EBA15          les bx,[0x15ba]
00000114  8B460E            mov ax,[bp+0xe]
00000117  26894704          mov [es:bx+0x4],ax
0000011B  C41EBA15          les bx,[0x15ba]
0000011F  8B4610            mov ax,[bp+0x10]
00000122  26894706          mov [es:bx+0x6],ax
00000126  C41EBA15          les bx,[0x15ba]
0000012A  26C7471A0200      mov word [es:bx+0x1a],0x2
00000130  FF06BE15          inc word [0x15be]
00000134  8306BA151C        add word [0x15ba],byte +0x1c
00000139  5D                pop bp
0000013A  CB                retf

; ---- 0A96:013B ----
0000013B  55                push bp
0000013C  8BEC              mov bp,sp
0000013E  C41EBA15          les bx,[0x15ba]
00000142  8B4606            mov ax,[bp+0x6]
00000145  268907            mov [es:bx],ax
00000148  C41EBA15          les bx,[0x15ba]
0000014C  8B4608            mov ax,[bp+0x8]
0000014F  26894702          mov [es:bx+0x2],ax
00000153  C41EBA15          les bx,[0x15ba]
00000157  8B460E            mov ax,[bp+0xe]
0000015A  2689470C          mov [es:bx+0xc],ax
0000015E  C41EBA15          les bx,[0x15ba]
00000162  8B4610            mov ax,[bp+0x10]
00000165  2689470E          mov [es:bx+0xe],ax
00000169  C41EBA15          les bx,[0x15ba]
0000016D  8B4612            mov ax,[bp+0x12]
00000170  26894704          mov [es:bx+0x4],ax
00000174  C41EBA15          les bx,[0x15ba]
00000178  8B4614            mov ax,[bp+0x14]
0000017B  26894706          mov [es:bx+0x6],ax
0000017F  C41EBA15          les bx,[0x15ba]
00000183  8B460A            mov ax,[bp+0xa]
00000186  26894708          mov [es:bx+0x8],ax
0000018A  C41EBA15          les bx,[0x15ba]
0000018E  8B460C            mov ax,[bp+0xc]
00000191  2689470A          mov [es:bx+0xa],ax
00000195  C41EBA15          les bx,[0x15ba]
00000199  26C7471A0300      mov word [es:bx+0x1a],0x3
0000019F  FF06BE15          inc word [0x15be]
000001A3  8306BA151C        add word [0x15ba],byte +0x1c
000001A8  5D                pop bp
000001A9  CB                retf

; ---- 0A96:01AA ----
000001AA  55                push bp
000001AB  8BEC              mov bp,sp
000001AD  C41EBA15          les bx,[0x15ba]
000001B1  8B4606            mov ax,[bp+0x6]
000001B4  268907            mov [es:bx],ax
000001B7  C41EBA15          les bx,[0x15ba]
000001BB  8B4608            mov ax,[bp+0x8]
000001BE  26894702          mov [es:bx+0x2],ax
000001C2  C41EBA15          les bx,[0x15ba]
000001C6  8B460A            mov ax,[bp+0xa]
000001C9  2689470C          mov [es:bx+0xc],ax
000001CD  C41EBA15          les bx,[0x15ba]
000001D1  8B460C            mov ax,[bp+0xc]
000001D4  2689470E          mov [es:bx+0xe],ax
000001D8  C41EBA15          les bx,[0x15ba]
000001DC  8B460E            mov ax,[bp+0xe]
000001DF  26894704          mov [es:bx+0x4],ax
000001E3  C41EBA15          les bx,[0x15ba]
000001E7  8B4610            mov ax,[bp+0x10]
000001EA  26894706          mov [es:bx+0x6],ax
000001EE  C41EBA15          les bx,[0x15ba]
000001F2  8B4612            mov ax,[bp+0x12]
000001F5  26894710          mov [es:bx+0x10],ax
000001F9  C41EBA15          les bx,[0x15ba]
000001FD  26C7471A0400      mov word [es:bx+0x1a],0x4
00000203  FF06BE15          inc word [0x15be]
00000207  8306BA151C        add word [0x15ba],byte +0x1c
0000020C  5D                pop bp
0000020D  CB                retf

; ---- 0A96:020E ----
0000020E  55                push bp
0000020F  8BEC              mov bp,sp
00000211  C41EBA15          les bx,[0x15ba]
00000215  8B4606            mov ax,[bp+0x6]
00000218  268907            mov [es:bx],ax
0000021B  C41EBA15          les bx,[0x15ba]
0000021F  8B4608            mov ax,[bp+0x8]
00000222  26894702          mov [es:bx+0x2],ax
00000226  C41EBA15          les bx,[0x15ba]
0000022A  8B460A            mov ax,[bp+0xa]
0000022D  2689470C          mov [es:bx+0xc],ax
00000231  C41EBA15          les bx,[0x15ba]
00000235  8B460C            mov ax,[bp+0xc]
00000238  2689470E          mov [es:bx+0xe],ax
0000023C  C41EBA15          les bx,[0x15ba]
00000240  8B460E            mov ax,[bp+0xe]
00000243  26894704          mov [es:bx+0x4],ax
00000247  C41EBA15          les bx,[0x15ba]
0000024B  8B4610            mov ax,[bp+0x10]
0000024E  26894706          mov [es:bx+0x6],ax
00000252  C41EBA15          les bx,[0x15ba]
00000256  26C7471A0500      mov word [es:bx+0x1a],0x5
0000025C  FF06BE15          inc word [0x15be]
00000260  8306BA151C        add word [0x15ba],byte +0x1c
00000265  5D                pop bp
00000266  CB                retf

; ---- 0A96:0267 ----
00000267  55                push bp
00000268  8BEC              mov bp,sp
0000026A  C41EBA15          les bx,[0x15ba]
0000026E  8B4606            mov ax,[bp+0x6]
00000271  268907            mov [es:bx],ax
00000274  C41EBA15          les bx,[0x15ba]
00000278  8B4608            mov ax,[bp+0x8]
0000027B  26894702          mov [es:bx+0x2],ax
0000027F  C41EBA15          les bx,[0x15ba]
00000283  8B460A            mov ax,[bp+0xa]
00000286  2689470C          mov [es:bx+0xc],ax
0000028A  C41EBA15          les bx,[0x15ba]
0000028E  8B460C            mov ax,[bp+0xc]
00000291  2689470E          mov [es:bx+0xe],ax
00000295  C41EBA15          les bx,[0x15ba]
00000299  8B460E            mov ax,[bp+0xe]
0000029C  26894704          mov [es:bx+0x4],ax
000002A0  C41EBA15          les bx,[0x15ba]
000002A4  8B4610            mov ax,[bp+0x10]
000002A7  26894706          mov [es:bx+0x6],ax
000002AB  C41EBA15          les bx,[0x15ba]
000002AF  8B4612            mov ax,[bp+0x12]
000002B2  26894708          mov [es:bx+0x8],ax
000002B6  C41EBA15          les bx,[0x15ba]
000002BA  8B4614            mov ax,[bp+0x14]
000002BD  2689470A          mov [es:bx+0xa],ax
000002C1  C41EBA15          les bx,[0x15ba]
000002C5  26C7471A0600      mov word [es:bx+0x1a],0x6
000002CB  FF06BE15          inc word [0x15be]
000002CF  8306BA151C        add word [0x15ba],byte +0x1c
000002D4  5D                pop bp
000002D5  CB                retf

; ---- 0A96:02D6 ----
000002D6  55                push bp
000002D7  8BEC              mov bp,sp
000002D9  56                push si
000002DA  57                push di
000002DB  8B7E0C            mov di,[bp+0xc]
000002DE  33F6              xor si,si
000002E0  EB26              jmp short 0x308
000002E2  FF760E            push word [bp+0xe]
000002E5  57                push di
000002E6  FF760A            push word [bp+0xa]
000002E9  FF7608            push word [bp+0x8]
000002EC  FF7606            push word [bp+0x6]
000002EF  9A1103960A        call 0xa96:0x311
000002F4  83C40A            add sp,byte +0xa
000002F7  8B4606            mov ax,[bp+0x6]
000002FA  050800            add ax,0x8
000002FD  894606            mov [bp+0x6],ax
00000300  8BC7              mov ax,di
00000302  050800            add ax,0x8
00000305  8BF8              mov di,ax
00000307  46                inc si
00000308  83FE05            cmp si,byte +0x5
0000030B  7CD5              jl 0x2e2
0000030D  5F                pop di
0000030E  5E                pop si
0000030F  5D                pop bp
00000310  CB                retf

; ---- 0A96:0311 ----
00000311  55                push bp
00000312  8BEC              mov bp,sp
00000314  C41EBA15          les bx,[0x15ba]
00000318  8B4606            mov ax,[bp+0x6]
0000031B  268907            mov [es:bx],ax
0000031E  C41EBA15          les bx,[0x15ba]
00000322  8B4608            mov ax,[bp+0x8]
00000325  26894702          mov [es:bx+0x2],ax
00000329  C41EBA15          les bx,[0x15ba]
0000032D  8B460A            mov ax,[bp+0xa]
00000330  2689470E          mov [es:bx+0xe],ax
00000334  C41EBA15          les bx,[0x15ba]
00000338  8B460C            mov ax,[bp+0xc]
0000033B  26894704          mov [es:bx+0x4],ax
0000033F  C41EBA15          les bx,[0x15ba]
00000343  8B460E            mov ax,[bp+0xe]
00000346  26894706          mov [es:bx+0x6],ax
0000034A  C41EBA15          les bx,[0x15ba]
0000034E  26C7471A0700      mov word [es:bx+0x1a],0x7
00000354  FF06BE15          inc word [0x15be]
00000358  8306BA151C        add word [0x15ba],byte +0x1c
0000035D  5D                pop bp
0000035E  CB                retf

; ---- 0A96:035F ----
0000035F  55                push bp
00000360  8BEC              mov bp,sp
00000362  C41EBA15          les bx,[0x15ba]
00000366  8B4606            mov ax,[bp+0x6]
00000369  268907            mov [es:bx],ax
0000036C  C41EBA15          les bx,[0x15ba]
00000370  8B4608            mov ax,[bp+0x8]
00000373  26894702          mov [es:bx+0x2],ax
00000377  C41EBA15          les bx,[0x15ba]
0000037B  8B460A            mov ax,[bp+0xa]
0000037E  2689470E          mov [es:bx+0xe],ax
00000382  C41EBA15          les bx,[0x15ba]
00000386  8B460C            mov ax,[bp+0xc]
00000389  26894704          mov [es:bx+0x4],ax
0000038D  C41EBA15          les bx,[0x15ba]
00000391  8B460E            mov ax,[bp+0xe]
00000394  26894706          mov [es:bx+0x6],ax
00000398  C41EBA15          les bx,[0x15ba]
0000039C  26C7471A0800      mov word [es:bx+0x1a],0x8
000003A2  FF06BE15          inc word [0x15be]
000003A6  8306BA151C        add word [0x15ba],byte +0x1c
000003AB  5D                pop bp
000003AC  CB                retf

; ---- 0A96:03AD ----
000003AD  55                push bp
000003AE  8BEC              mov bp,sp
000003B0  C41EBA15          les bx,[0x15ba]
000003B4  8B4606            mov ax,[bp+0x6]
000003B7  268907            mov [es:bx],ax
000003BA  C41EBA15          les bx,[0x15ba]
000003BE  8B4608            mov ax,[bp+0x8]
000003C1  26894702          mov [es:bx+0x2],ax
000003C5  C41EBA15          les bx,[0x15ba]
000003C9  8B460A            mov ax,[bp+0xa]
000003CC  2689470E          mov [es:bx+0xe],ax
000003D0  C41EBA15          les bx,[0x15ba]
000003D4  8B460C            mov ax,[bp+0xc]
000003D7  26894704          mov [es:bx+0x4],ax
000003DB  C41EBA15          les bx,[0x15ba]
000003DF  8B460E            mov ax,[bp+0xe]
000003E2  26894706          mov [es:bx+0x6],ax
000003E6  C41EBA15          les bx,[0x15ba]
000003EA  26C7471A1000      mov word [es:bx+0x1a],0x10
000003F0  FF06BE15          inc word [0x15be]
000003F4  8306BA151C        add word [0x15ba],byte +0x1c
000003F9  5D                pop bp
000003FA  CB                retf

; ---- 0A96:03FB ----
000003FB  55                push bp
000003FC  8BEC              mov bp,sp
000003FE  C41EBA15          les bx,[0x15ba]
00000402  8B4606            mov ax,[bp+0x6]
00000405  268907            mov [es:bx],ax
00000408  C41EBA15          les bx,[0x15ba]
0000040C  8B4608            mov ax,[bp+0x8]
0000040F  26894702          mov [es:bx+0x2],ax
00000413  C41EBA15          les bx,[0x15ba]
00000417  8B460A            mov ax,[bp+0xa]
0000041A  26894710          mov [es:bx+0x10],ax
0000041E  C41EBA15          les bx,[0x15ba]
00000422  26C7471A0F00      mov word [es:bx+0x1a],0xf
00000428  FF06BE15          inc word [0x15be]
0000042C  8306BA151C        add word [0x15ba],byte +0x1c
00000431  5D                pop bp
00000432  CB                retf

; ---- 0A96:0433 ----
00000433  55                push bp
00000434  8BEC              mov bp,sp
00000436  C41EBA15          les bx,[0x15ba]
0000043A  8B4606            mov ax,[bp+0x6]
0000043D  268907            mov [es:bx],ax
00000440  C41EBA15          les bx,[0x15ba]
00000444  8B4608            mov ax,[bp+0x8]
00000447  26894702          mov [es:bx+0x2],ax
0000044B  C41EBA15          les bx,[0x15ba]
0000044F  8B460A            mov ax,[bp+0xa]
00000452  26894704          mov [es:bx+0x4],ax
00000456  C41EBA15          les bx,[0x15ba]
0000045A  8B460C            mov ax,[bp+0xc]
0000045D  26894706          mov [es:bx+0x6],ax
00000461  C41EBA15          les bx,[0x15ba]
00000465  8B460E            mov ax,[bp+0xe]
00000468  26894710          mov [es:bx+0x10],ax
0000046C  C41EBA15          les bx,[0x15ba]
00000470  26C7471A0900      mov word [es:bx+0x1a],0x9
00000476  FF06BE15          inc word [0x15be]
0000047A  8306BA151C        add word [0x15ba],byte +0x1c
0000047F  5D                pop bp
00000480  CB                retf

; ---- 0A96:0481 ----
00000481  55                push bp
00000482  8BEC              mov bp,sp
00000484  C41EBA15          les bx,[0x15ba]
00000488  8B4606            mov ax,[bp+0x6]
0000048B  268907            mov [es:bx],ax
0000048E  C41EBA15          les bx,[0x15ba]
00000492  8B4608            mov ax,[bp+0x8]
00000495  26894702          mov [es:bx+0x2],ax
00000499  C41EBA15          les bx,[0x15ba]
0000049D  8B4606            mov ax,[bp+0x6]
000004A0  03460A            add ax,[bp+0xa]
000004A3  48                dec ax
000004A4  26894704          mov [es:bx+0x4],ax
000004A8  C41EBA15          les bx,[0x15ba]
000004AC  8B4608            mov ax,[bp+0x8]
000004AF  03460C            add ax,[bp+0xc]
000004B2  48                dec ax
000004B3  26894706          mov [es:bx+0x6],ax
000004B7  C41EBA15          les bx,[0x15ba]
000004BB  8B460E            mov ax,[bp+0xe]
000004BE  26894710          mov [es:bx+0x10],ax
000004C2  C41EBA15          les bx,[0x15ba]
000004C6  26C7471A0A00      mov word [es:bx+0x1a],0xa
000004CC  FF06BE15          inc word [0x15be]
000004D0  8306BA151C        add word [0x15ba],byte +0x1c
000004D5  5D                pop bp
000004D6  CB                retf

; ---- 0A96:04D7 ----
000004D7  55                push bp
000004D8  8BEC              mov bp,sp
000004DA  C41EBA15          les bx,[0x15ba]
000004DE  8B4606            mov ax,[bp+0x6]
000004E1  268907            mov [es:bx],ax
000004E4  C41EBA15          les bx,[0x15ba]
000004E8  8B4608            mov ax,[bp+0x8]
000004EB  26894702          mov [es:bx+0x2],ax
000004EF  C41EBA15          les bx,[0x15ba]
000004F3  8B460A            mov ax,[bp+0xa]
000004F6  2689470C          mov [es:bx+0xc],ax
000004FA  C41EBA15          les bx,[0x15ba]
000004FE  8B460C            mov ax,[bp+0xc]
00000501  2689470E          mov [es:bx+0xe],ax
00000505  C41EBA15          les bx,[0x15ba]
00000509  8B460E            mov ax,[bp+0xe]
0000050C  26894710          mov [es:bx+0x10],ax
00000510  C41EBA15          les bx,[0x15ba]
00000514  26C7471A0B00      mov word [es:bx+0x1a],0xb
0000051A  FF06BE15          inc word [0x15be]
0000051E  8306BA151C        add word [0x15ba],byte +0x1c
00000523  5D                pop bp
00000524  CB                retf

; ---- 0A96:0525 ----
00000525  55                push bp
00000526  8BEC              mov bp,sp
00000528  C41EBA15          les bx,[0x15ba]
0000052C  8B460C            mov ax,[bp+0xc]
0000052F  8B560A            mov dx,[bp+0xa]
00000532  26895712          mov [es:bx+0x12],dx
00000536  26894714          mov [es:bx+0x14],ax
0000053A  C41EBA15          les bx,[0x15ba]
0000053E  8B4606            mov ax,[bp+0x6]
00000541  268907            mov [es:bx],ax
00000544  C41EBA15          les bx,[0x15ba]
00000548  8B4608            mov ax,[bp+0x8]
0000054B  26894702          mov [es:bx+0x2],ax
0000054F  C41EBA15          les bx,[0x15ba]
00000553  8B460E            mov ax,[bp+0xe]
00000556  26894710          mov [es:bx+0x10],ax
0000055A  C41EBA15          les bx,[0x15ba]
0000055E  8B4610            mov ax,[bp+0x10]
00000561  26894708          mov [es:bx+0x8],ax
00000565  C41EBA15          les bx,[0x15ba]
00000569  26C7471A0C00      mov word [es:bx+0x1a],0xc
0000056F  FF06BE15          inc word [0x15be]
00000573  8306BA151C        add word [0x15ba],byte +0x1c
00000578  5D                pop bp
00000579  CB                retf

; ---- 0A96:057A ----
0000057A  55                push bp
0000057B  8BEC              mov bp,sp
0000057D  83EC04            sub sp,byte +0x4
00000580  56                push si
00000581  8B760E            mov si,[bp+0xe]
00000584  A1BC15            mov ax,[0x15bc]
00000587  8B16BA15          mov dx,[0x15ba]
0000058B  83C212            add dx,byte +0x12
0000058E  8956FC            mov [bp-0x4],dx
00000591  8946FE            mov [bp-0x2],ax
00000594  C41EBA15          les bx,[0x15ba]
00000598  8B4606            mov ax,[bp+0x6]
0000059B  268907            mov [es:bx],ax
0000059E  C41EBA15          les bx,[0x15ba]
000005A2  8B4608            mov ax,[bp+0x8]
000005A5  26894702          mov [es:bx+0x2],ax
000005A9  C41EBA15          les bx,[0x15ba]
000005AD  8B4610            mov ax,[bp+0x10]
000005B0  26894710          mov [es:bx+0x10],ax
000005B4  C41EBA15          les bx,[0x15ba]
000005B8  8B4612            mov ax,[bp+0x12]
000005BB  26894708          mov [es:bx+0x8],ax
000005BF  C41EBA15          les bx,[0x15ba]
000005C3  26897704          mov [es:bx+0x4],si
000005C7  C41EBA15          les bx,[0x15ba]
000005CB  26C7471A0D00      mov word [es:bx+0x1a],0xd
000005D1  FF76FE            push word [bp-0x2]
000005D4  FF76FC            push word [bp-0x4]
000005D7  56                push si
000005D8  FF760C            push word [bp+0xc]
000005DB  FF760A            push word [bp+0xa]
000005DE  9A01009522        call 0x2295:0x1
000005E3  83C40A            add sp,byte +0xa
000005E6  C45EFC            les bx,[bp-0x4]
000005E9  26C60000          mov byte [es:bx+si],0x0
000005ED  FF06BE15          inc word [0x15be]
000005F1  8306BA151C        add word [0x15ba],byte +0x1c
000005F6  5E                pop si
000005F7  8BE5              mov sp,bp
000005F9  5D                pop bp
000005FA  CB                retf

; ---- 0A96:05FB ----
000005FB  55                push bp
000005FC  8BEC              mov bp,sp
000005FE  83EC04            sub sp,byte +0x4
00000601  56                push si
00000602  8B760E            mov si,[bp+0xe]
00000605  A1BC15            mov ax,[0x15bc]
00000608  8B16BA15          mov dx,[0x15ba]
0000060C  83C212            add dx,byte +0x12
0000060F  8956FC            mov [bp-0x4],dx
00000612  8946FE            mov [bp-0x2],ax
00000615  C41EBA15          les bx,[0x15ba]
00000619  8B4606            mov ax,[bp+0x6]
0000061C  268907            mov [es:bx],ax
0000061F  C41EBA15          les bx,[0x15ba]
00000623  8B4608            mov ax,[bp+0x8]
00000626  26894702          mov [es:bx+0x2],ax
0000062A  C41EBA15          les bx,[0x15ba]
0000062E  8B4610            mov ax,[bp+0x10]
00000631  26894710          mov [es:bx+0x10],ax
00000635  C41EBA15          les bx,[0x15ba]
00000639  8B4612            mov ax,[bp+0x12]
0000063C  26894708          mov [es:bx+0x8],ax
00000640  C41EBA15          les bx,[0x15ba]
00000644  26897704          mov [es:bx+0x4],si
00000648  C41EBA15          les bx,[0x15ba]
0000064C  26C7471A0D00      mov word [es:bx+0x1a],0xd
00000652  C45EFC            les bx,[bp-0x4]
00000655  26C60724          mov byte [es:bx],0x24
00000659  FF46FC            inc word [bp-0x4]
0000065C  FF76FE            push word [bp-0x2]
0000065F  FF76FC            push word [bp-0x4]
00000662  56                push si
00000663  FF760C            push word [bp+0xc]
00000666  FF760A            push word [bp+0xa]
00000669  9A01009522        call 0x2295:0x1
0000066E  83C40A            add sp,byte +0xa
00000671  C45EFC            les bx,[bp-0x4]
00000674  26C60000          mov byte [es:bx+si],0x0
00000678  FF06BE15          inc word [0x15be]
0000067C  8306BA151C        add word [0x15ba],byte +0x1c
00000681  5E                pop si
00000682  8BE5              mov sp,bp
00000684  5D                pop bp
00000685  CB                retf

; ---- 0A96:0686 ----
00000686  55                push bp
00000687  8BEC              mov bp,sp
00000689  C41EBA15          les bx,[0x15ba]
0000068D  8B4606            mov ax,[bp+0x6]
00000690  268907            mov [es:bx],ax
00000693  C41EBA15          les bx,[0x15ba]
00000697  26C7471A0E00      mov word [es:bx+0x1a],0xe
0000069D  FF06BE15          inc word [0x15be]
000006A1  8306BA151C        add word [0x15ba],byte +0x1c
000006A6  5D                pop bp
000006A7  CB                retf

; ---- 0A96:06A8 ----
000006A8  55                push bp
000006A9  8BEC              mov bp,sp
000006AB  56                push si
000006AC  8B36BE15          mov si,[0x15be]
000006B0  C706BA15C015      mov word [0x15ba],0x15c0
000006B6  8C1EBC15          mov [0x15bc],ds
000006BA  C706BE150000      mov word [0x15be],0x0
000006C0  EB19              jmp short 0x6db
000006C2  C41EBA15          les bx,[0x15ba]
000006C6  268B5F1A          mov bx,[es:bx+0x1a]
000006CA  D1E3              shl bx,1
000006CC  D1E3              shl bx,1
000006CE  FF9FF401          call far [bx+0x1f4]
000006D2  8306BA151C        add word [0x15ba],byte +0x1c
000006D7  FF06BE15          inc word [0x15be]
000006DB  3936BE15          cmp [0x15be],si
000006DF  7CE1              jl 0x6c2
000006E1  5E                pop si
000006E2  5D                pop bp
000006E3  CB                retf
000006E4  55                push bp
000006E5  8BEC              mov bp,sp
000006E7  C41EBA15          les bx,[0x15ba]
000006EB  26FF7706          push word [es:bx+0x6]
000006EF  C41EBA15          les bx,[0x15ba]
000006F3  26FF7704          push word [es:bx+0x4]
000006F7  C41EBA15          les bx,[0x15ba]
000006FB  26FF770E          push word [es:bx+0xe]
000006FF  C41EBA15          les bx,[0x15ba]
00000703  26FF770C          push word [es:bx+0xc]
00000707  C41EBA15          les bx,[0x15ba]
0000070B  26FF7702          push word [es:bx+0x2]
0000070F  C41EBA15          les bx,[0x15ba]
00000713  26FF37            push word [es:bx]
00000716  9AA006CD23        call 0x23cd:0x6a0
0000071B  83C40C            add sp,byte +0xc
0000071E  5D                pop bp
0000071F  CB                retf
00000720  55                push bp
00000721  8BEC              mov bp,sp
00000723  C41EBA15          les bx,[0x15ba]
00000727  26FF7706          push word [es:bx+0x6]
0000072B  C41EBA15          les bx,[0x15ba]
0000072F  26FF7704          push word [es:bx+0x4]
00000733  C41EBA15          les bx,[0x15ba]
00000737  26FF770E          push word [es:bx+0xe]
0000073B  C41EBA15          les bx,[0x15ba]
0000073F  26FF770C          push word [es:bx+0xc]
00000743  C41EBA15          les bx,[0x15ba]
00000747  26FF7702          push word [es:bx+0x2]
0000074B  C41EBA15          les bx,[0x15ba]
0000074F  26FF37            push word [es:bx]
00000752  9AAF06CD23        call 0x23cd:0x6af
00000757  83C40C            add sp,byte +0xc
0000075A  5D                pop bp
0000075B  CB                retf
0000075C  55                push bp
0000075D  8BEC              mov bp,sp
0000075F  C41EBA15          les bx,[0x15ba]
00000763  26FF7706          push word [es:bx+0x6]
00000767  C41EBA15          les bx,[0x15ba]
0000076B  26FF7704          push word [es:bx+0x4]
0000076F  C41EBA15          les bx,[0x15ba]
00000773  26FF770E          push word [es:bx+0xe]
00000777  C41EBA15          les bx,[0x15ba]
0000077B  26FF770C          push word [es:bx+0xc]
0000077F  C41EBA15          les bx,[0x15ba]
00000783  26FF7702          push word [es:bx+0x2]
00000787  C41EBA15          les bx,[0x15ba]
0000078B  26FF37            push word [es:bx]
0000078E  9AB406CD23        call 0x23cd:0x6b4
00000793  83C40C            add sp,byte +0xc
00000796  5D                pop bp
00000797  CB                retf
00000798  55                push bp
00000799  8BEC              mov bp,sp
0000079B  C41EBA15          les bx,[0x15ba]
0000079F  26FF770A          push word [es:bx+0xa]
000007A3  C41EBA15          les bx,[0x15ba]
000007A7  26FF7708          push word [es:bx+0x8]
000007AB  C41EBA15          les bx,[0x15ba]
000007AF  26FF7706          push word [es:bx+0x6]
000007B3  C41EBA15          les bx,[0x15ba]
000007B7  26FF7704          push word [es:bx+0x4]
000007BB  C41EBA15          les bx,[0x15ba]
000007BF  26FF770E          push word [es:bx+0xe]
000007C3  C41EBA15          les bx,[0x15ba]
000007C7  26FF770C          push word [es:bx+0xc]
000007CB  C41EBA15          les bx,[0x15ba]
000007CF  26FF7702          push word [es:bx+0x2]
000007D3  C41EBA15          les bx,[0x15ba]
000007D7  26FF37            push word [es:bx]
000007DA  9AA506CD23        call 0x23cd:0x6a5
000007DF  83C410            add sp,byte +0x10
000007E2  5D                pop bp
000007E3  CB                retf
000007E4  55                push bp
000007E5  8BEC              mov bp,sp
000007E7  56                push si
000007E8  C41EBA15          les bx,[0x15ba]
000007EC  268B7710          mov si,[es:bx+0x10]
000007F0  833E0E0901        cmp word [0x90e],byte +0x1
000007F5  7416              jz 0x80d
000007F7  8B1EC809          mov bx,[0x9c8]
000007FB  B104              mov cl,0x4
000007FD  D3E3              shl bx,cl
000007FF  8BC6              mov ax,si
00000801  250F00            and ax,0xf
00000804  03D8              add bx,ax
00000806  8A875401          mov al,[bx+0x154]
0000080A  98                cbw
0000080B  8BF0              mov si,ax
0000080D  56                push si
0000080E  C41EBA15          les bx,[0x15ba]
00000812  26FF7706          push word [es:bx+0x6]
00000816  C41EBA15          les bx,[0x15ba]
0000081A  26FF7704          push word [es:bx+0x4]
0000081E  C41EBA15          les bx,[0x15ba]
00000822  26FF770E          push word [es:bx+0xe]
00000826  C41EBA15          les bx,[0x15ba]
0000082A  26FF770C          push word [es:bx+0xc]
0000082E  C41EBA15          les bx,[0x15ba]
00000832  26FF7702          push word [es:bx+0x2]
00000836  C41EBA15          les bx,[0x15ba]
0000083A  26FF37            push word [es:bx]
0000083D  9AE306CD23        call 0x23cd:0x6e3
00000842  83C40E            add sp,byte +0xe
00000845  5E                pop si
00000846  5D                pop bp
00000847  CB                retf
00000848  55                push bp
00000849  8BEC              mov bp,sp
0000084B  C41EBA15          les bx,[0x15ba]
0000084F  26FF7706          push word [es:bx+0x6]
00000853  C41EBA15          les bx,[0x15ba]
00000857  26FF7704          push word [es:bx+0x4]
0000085B  C41EBA15          les bx,[0x15ba]
0000085F  26FF770E          push word [es:bx+0xe]
00000863  C41EBA15          les bx,[0x15ba]
00000867  26FF770C          push word [es:bx+0xc]
0000086B  C41EBA15          les bx,[0x15ba]
0000086F  26FF7702          push word [es:bx+0x2]
00000873  C41EBA15          les bx,[0x15ba]
00000877  26FF37            push word [es:bx]
0000087A  9AAA06CD23        call 0x23cd:0x6aa
0000087F  83C40C            add sp,byte +0xc
00000882  5D                pop bp
00000883  CB                retf
00000884  55                push bp
00000885  8BEC              mov bp,sp
00000887  83EC14            sub sp,byte +0x14
0000088A  56                push si
0000088B  57                push di
0000088C  C41EBA15          les bx,[0x15ba]
00000890  268B07            mov ax,[es:bx]
00000893  8946FE            mov [bp-0x2],ax
00000896  C41EBA15          les bx,[0x15ba]
0000089A  268B4702          mov ax,[es:bx+0x2]
0000089E  8946FC            mov [bp-0x4],ax
000008A1  C41EBA15          les bx,[0x15ba]
000008A5  268B4704          mov ax,[es:bx+0x4]
000008A9  8946FA            mov [bp-0x6],ax
000008AC  C41EBA15          les bx,[0x15ba]
000008B0  268B4706          mov ax,[es:bx+0x6]
000008B4  8946F8            mov [bp-0x8],ax
000008B7  C41EBA15          les bx,[0x15ba]
000008BB  268B470C          mov ax,[es:bx+0xc]
000008BF  8946F6            mov [bp-0xa],ax
000008C2  C41EBA15          les bx,[0x15ba]
000008C6  268B470E          mov ax,[es:bx+0xe]
000008CA  8946F4            mov [bp-0xc],ax
000008CD  C41EBA15          les bx,[0x15ba]
000008D1  268B4708          mov ax,[es:bx+0x8]
000008D5  8946F2            mov [bp-0xe],ax
000008D8  C41EBA15          les bx,[0x15ba]
000008DC  268B470A          mov ax,[es:bx+0xa]
000008E0  8946F0            mov [bp-0x10],ax
000008E3  33F6              xor si,si
000008E5  EB5D              jmp short 0x944
000008E7  8BC6              mov ax,si
000008E9  0346F4            add ax,[bp-0xc]
000008EC  3B46F0            cmp ax,[bp-0x10]
000008EF  7F05              jg 0x8f6
000008F1  8B46F4            mov ax,[bp-0xc]
000008F4  EB05              jmp short 0x8fb
000008F6  8B46F0            mov ax,[bp-0x10]
000008F9  2BC6              sub ax,si
000008FB  8946EC            mov [bp-0x14],ax
000008FE  33FF              xor di,di
00000900  EB3A              jmp short 0x93c
00000902  8BC7              mov ax,di
00000904  0346F6            add ax,[bp-0xa]
00000907  3B46F2            cmp ax,[bp-0xe]
0000090A  7F05              jg 0x911
0000090C  8B46F6            mov ax,[bp-0xa]
0000090F  EB05              jmp short 0x916
00000911  8B46F2            mov ax,[bp-0xe]
00000914  2BC7              sub ax,di
00000916  8946EE            mov [bp-0x12],ax
00000919  8B46F8            mov ax,[bp-0x8]
0000091C  03C6              add ax,si
0000091E  50                push ax
0000091F  8B46FA            mov ax,[bp-0x6]
00000922  03C7              add ax,di
00000924  50                push ax
00000925  FF76EC            push word [bp-0x14]
00000928  FF76EE            push word [bp-0x12]
0000092B  FF76FC            push word [bp-0x4]
0000092E  FF76FE            push word [bp-0x2]
00000931  9AA006CD23        call 0x23cd:0x6a0
00000936  83C40C            add sp,byte +0xc
00000939  037EF6            add di,[bp-0xa]
0000093C  3B7EF2            cmp di,[bp-0xe]
0000093F  7CC1              jl 0x902
00000941  0376F4            add si,[bp-0xc]
00000944  3B76F0            cmp si,[bp-0x10]
00000947  7C9E              jl 0x8e7
00000949  5F                pop di
0000094A  5E                pop si
0000094B  8BE5              mov sp,bp
0000094D  5D                pop bp
0000094E  CB                retf
0000094F  55                push bp
00000950  8BEC              mov bp,sp
00000952  C41EBA15          les bx,[0x15ba]
00000956  26FF7706          push word [es:bx+0x6]
0000095A  C41EBA15          les bx,[0x15ba]
0000095E  26FF7704          push word [es:bx+0x4]
00000962  C41EBA15          les bx,[0x15ba]
00000966  26FF770E          push word [es:bx+0xe]
0000096A  B80100            mov ax,0x1
0000096D  50                push ax
0000096E  C41EBA15          les bx,[0x15ba]
00000972  26FF7702          push word [es:bx+0x2]
00000976  C41EBA15          les bx,[0x15ba]
0000097A  26FF37            push word [es:bx]
0000097D  9AD406CD23        call 0x23cd:0x6d4
00000982  83C40C            add sp,byte +0xc
00000985  5D                pop bp
00000986  CB                retf
00000987  55                push bp
00000988  8BEC              mov bp,sp
0000098A  C41EBA15          les bx,[0x15ba]
0000098E  26FF7706          push word [es:bx+0x6]
00000992  C41EBA15          les bx,[0x15ba]
00000996  26FF7704          push word [es:bx+0x4]
0000099A  C41EBA15          les bx,[0x15ba]
0000099E  26FF770E          push word [es:bx+0xe]
000009A2  B80200            mov ax,0x2
000009A5  50                push ax
000009A6  C41EBA15          les bx,[0x15ba]
000009AA  26FF7702          push word [es:bx+0x2]
000009AE  C41EBA15          les bx,[0x15ba]
000009B2  26FF37            push word [es:bx]
000009B5  9AD406CD23        call 0x23cd:0x6d4
000009BA  83C40C            add sp,byte +0xc
000009BD  5D                pop bp
000009BE  CB                retf
000009BF  55                push bp
000009C0  8BEC              mov bp,sp
000009C2  C41EBA15          les bx,[0x15ba]
000009C6  26FF7706          push word [es:bx+0x6]
000009CA  C41EBA15          les bx,[0x15ba]
000009CE  26FF7704          push word [es:bx+0x4]
000009D2  C41EBA15          les bx,[0x15ba]
000009D6  26FF770E          push word [es:bx+0xe]
000009DA  B80400            mov ax,0x4
000009DD  50                push ax
000009DE  C41EBA15          les bx,[0x15ba]
000009E2  26FF7702          push word [es:bx+0x2]
000009E6  C41EBA15          les bx,[0x15ba]
000009EA  26FF37            push word [es:bx]
000009ED  9AD406CD23        call 0x23cd:0x6d4
000009F2  83C40C            add sp,byte +0xc
000009F5  5D                pop bp
000009F6  CB                retf
000009F7  55                push bp
000009F8  8BEC              mov bp,sp
000009FA  56                push si
000009FB  C41EBA15          les bx,[0x15ba]
000009FF  268B7710          mov si,[es:bx+0x10]
00000A03  833E0E0901        cmp word [0x90e],byte +0x1
00000A08  7416              jz 0xa20
00000A0A  8B1EC809          mov bx,[0x9c8]
00000A0E  B104              mov cl,0x4
00000A10  D3E3              shl bx,cl
00000A12  8BC6              mov ax,si
00000A14  250F00            and ax,0xf
00000A17  03D8              add bx,ax
00000A19  8A875401          mov al,[bx+0x154]
00000A1D  98                cbw
00000A1E  8BF0              mov si,ax
00000A20  56                push si
00000A21  9AC305CD23        call 0x23cd:0x5c3
00000A26  59                pop cx
00000A27  C41EBA15          les bx,[0x15ba]
00000A2B  26FF7706          push word [es:bx+0x6]
00000A2F  C41EBA15          les bx,[0x15ba]
00000A33  26FF7704          push word [es:bx+0x4]
00000A37  C41EBA15          les bx,[0x15ba]
00000A3B  26FF7702          push word [es:bx+0x2]
00000A3F  C41EBA15          les bx,[0x15ba]
00000A43  26FF37            push word [es:bx]
00000A46  9ABE06CD23        call 0x23cd:0x6be
00000A4B  83C408            add sp,byte +0x8
00000A4E  5E                pop si
00000A4F  5D                pop bp
00000A50  CB                retf
00000A51  55                push bp
00000A52  8BEC              mov bp,sp
00000A54  56                push si
00000A55  C41EBA15          les bx,[0x15ba]
00000A59  268B7710          mov si,[es:bx+0x10]
00000A5D  833E0E0901        cmp word [0x90e],byte +0x1
00000A62  7416              jz 0xa7a
00000A64  8B1EC809          mov bx,[0x9c8]
00000A68  B104              mov cl,0x4
00000A6A  D3E3              shl bx,cl
00000A6C  8BC6              mov ax,si
00000A6E  250F00            and ax,0xf
00000A71  03D8              add bx,ax
00000A73  8A875401          mov al,[bx+0x154]
00000A77  98                cbw
00000A78  8BF0              mov si,ax
00000A7A  56                push si
00000A7B  9AC305CD23        call 0x23cd:0x5c3
00000A80  59                pop cx
00000A81  C41EBA15          les bx,[0x15ba]
00000A85  26FF7702          push word [es:bx+0x2]
00000A89  C41EBA15          les bx,[0x15ba]
00000A8D  26FF7704          push word [es:bx+0x4]
00000A91  C41EBA15          les bx,[0x15ba]
00000A95  26FF7702          push word [es:bx+0x2]
00000A99  C41EBA15          les bx,[0x15ba]
00000A9D  26FF37            push word [es:bx]
00000AA0  9ABE06CD23        call 0x23cd:0x6be
00000AA5  83C408            add sp,byte +0x8
00000AA8  C41EBA15          les bx,[0x15ba]
00000AAC  26FF7706          push word [es:bx+0x6]
00000AB0  C41EBA15          les bx,[0x15ba]
00000AB4  26FF7704          push word [es:bx+0x4]
00000AB8  C41EBA15          les bx,[0x15ba]
00000ABC  26FF7702          push word [es:bx+0x2]
00000AC0  C41EBA15          les bx,[0x15ba]
00000AC4  26FF7704          push word [es:bx+0x4]
00000AC8  9ABE06CD23        call 0x23cd:0x6be
00000ACD  83C408            add sp,byte +0x8
00000AD0  C41EBA15          les bx,[0x15ba]
00000AD4  26FF7706          push word [es:bx+0x6]
00000AD8  C41EBA15          les bx,[0x15ba]
00000ADC  26FF37            push word [es:bx]
00000ADF  C41EBA15          les bx,[0x15ba]
00000AE3  26FF7706          push word [es:bx+0x6]
00000AE7  C41EBA15          les bx,[0x15ba]
00000AEB  26FF7704          push word [es:bx+0x4]
00000AEF  9ABE06CD23        call 0x23cd:0x6be
00000AF4  83C408            add sp,byte +0x8
00000AF7  C41EBA15          les bx,[0x15ba]
00000AFB  26FF7702          push word [es:bx+0x2]
00000AFF  C41EBA15          les bx,[0x15ba]
00000B03  26FF37            push word [es:bx]
00000B06  C41EBA15          les bx,[0x15ba]
00000B0A  26FF7706          push word [es:bx+0x6]
00000B0E  C41EBA15          les bx,[0x15ba]
00000B12  26FF37            push word [es:bx]
00000B15  9ABE06CD23        call 0x23cd:0x6be
00000B1A  83C408            add sp,byte +0x8
00000B1D  5E                pop si
00000B1E  5D                pop bp
00000B1F  CB                retf
00000B20  55                push bp
00000B21  8BEC              mov bp,sp
00000B23  56                push si
00000B24  C41EBA15          les bx,[0x15ba]
00000B28  268B7710          mov si,[es:bx+0x10]
00000B2C  833E0E0901        cmp word [0x90e],byte +0x1
00000B31  7416              jz 0xb49
00000B33  8B1EC809          mov bx,[0x9c8]
00000B37  B104              mov cl,0x4
00000B39  D3E3              shl bx,cl
00000B3B  8BC6              mov ax,si
00000B3D  250F00            and ax,0xf
00000B40  03D8              add bx,ax
00000B42  8A875401          mov al,[bx+0x154]
00000B46  98                cbw
00000B47  8BF0              mov si,ax
00000B49  56                push si
00000B4A  9AC305CD23        call 0x23cd:0x5c3
00000B4F  59                pop cx
00000B50  C41EBA15          les bx,[0x15ba]
00000B54  26FF770E          push word [es:bx+0xe]
00000B58  C41EBA15          les bx,[0x15ba]
00000B5C  26FF770C          push word [es:bx+0xc]
00000B60  C41EBA15          les bx,[0x15ba]
00000B64  26FF7702          push word [es:bx+0x2]
00000B68  C41EBA15          les bx,[0x15ba]
00000B6C  26FF37            push word [es:bx]
00000B6F  9A9B06CD23        call 0x23cd:0x69b
00000B74  83C408            add sp,byte +0x8
00000B77  5E                pop si
00000B78  5D                pop bp
00000B79  CB                retf
00000B7A  55                push bp
00000B7B  8BEC              mov bp,sp
00000B7D  C41EBA15          les bx,[0x15ba]
00000B81  26FF7708          push word [es:bx+0x8]
00000B85  C41EBA15          les bx,[0x15ba]
00000B89  26FF7710          push word [es:bx+0x10]
00000B8D  C41EBA15          les bx,[0x15ba]
00000B91  26FF7714          push word [es:bx+0x14]
00000B95  26FF7712          push word [es:bx+0x12]
00000B99  C41EBA15          les bx,[0x15ba]
00000B9D  26FF7702          push word [es:bx+0x2]
00000BA1  C41EBA15          les bx,[0x15ba]
00000BA5  26FF37            push word [es:bx]
00000BA8  9A680C960A        call 0xa96:0xc68
00000BAD  83C40C            add sp,byte +0xc
00000BB0  5D                pop bp
00000BB1  CB                retf
00000BB2  55                push bp
00000BB3  8BEC              mov bp,sp
00000BB5  C41EBA15          les bx,[0x15ba]
00000BB9  26FF7708          push word [es:bx+0x8]
00000BBD  C41EBA15          les bx,[0x15ba]
00000BC1  26FF7710          push word [es:bx+0x10]
00000BC5  A1BA15            mov ax,[0x15ba]
00000BC8  051200            add ax,0x12
00000BCB  FF36BC15          push word [0x15bc]
00000BCF  50                push ax
00000BD0  C41EBA15          les bx,[0x15ba]
00000BD4  26FF7702          push word [es:bx+0x2]
00000BD8  C41EBA15          les bx,[0x15ba]
00000BDC  26FF37            push word [es:bx]
00000BDF  9A680C960A        call 0xa96:0xc68
00000BE4  83C40C            add sp,byte +0xc
00000BE7  5D                pop bp
00000BE8  CB                retf
00000BE9  55                push bp
00000BEA  8BEC              mov bp,sp
00000BEC  C41EBA15          les bx,[0x15ba]
00000BF0  26FF37            push word [es:bx]
00000BF3  9AD005CD23        call 0x23cd:0x5d0
00000BF8  59                pop cx
00000BF9  5D                pop bp
00000BFA  CB                retf
00000BFB  55                push bp
00000BFC  8BEC              mov bp,sp
00000BFE  56                push si
00000BFF  C41EBA15          les bx,[0x15ba]
00000C03  268B7710          mov si,[es:bx+0x10]
00000C07  833E0E0901        cmp word [0x90e],byte +0x1
00000C0C  7416              jz 0xc24
00000C0E  8B1EC809          mov bx,[0x9c8]
00000C12  B104              mov cl,0x4
00000C14  D3E3              shl bx,cl
00000C16  8BC6              mov ax,si
00000C18  250F00            and ax,0xf
00000C1B  03D8              add bx,ax
00000C1D  8A875401          mov al,[bx+0x154]
00000C21  98                cbw
00000C22  8BF0              mov si,ax
00000C24  56                push si
00000C25  C41EBA15          les bx,[0x15ba]
00000C29  26FF7702          push word [es:bx+0x2]
00000C2D  C41EBA15          les bx,[0x15ba]
00000C31  26FF37            push word [es:bx]
00000C34  9AD906CD23        call 0x23cd:0x6d9
00000C39  83C406            add sp,byte +0x6
00000C3C  5E                pop si
00000C3D  5D                pop bp
00000C3E  CB                retf

; ---- 0A96:0C3F ----
00000C3F  55                push bp
00000C40  8BEC              mov bp,sp
00000C42  833E0E0902        cmp word [0x90e],byte +0x2
00000C47  7407              jz 0xc50
00000C49  833E0E0963        cmp word [0x90e],byte +0x63
00000C4E  7505              jnz 0xc55
00000C50  9A1F05FD21        call 0x21fd:0x51f
00000C55  8B5E06            mov bx,[bp+0x6]
00000C58  D1E3              shl bx,1
00000C5A  D1E3              shl bx,1
00000C5C  FFB7DE40          push word [bx+0x40de]
00000C60  9A9902670B        call 0xb67:0x299
00000C65  59                pop cx
00000C66  5D                pop bp
00000C67  CB                retf

; ---- 0A96:0C68 ----
00000C68  55                push bp
00000C69  8BEC              mov bp,sp
00000C6B  56                push si
00000C6C  57                push di
00000C6D  8B7E0E            mov di,[bp+0xe]
00000C70  8B7610            mov si,[bp+0x10]
00000C73  8B460A            mov ax,[bp+0xa]
00000C76  0B460C            or ax,[bp+0xc]
00000C79  7503              jnz 0xc7e
00000C7B  E99000            jmp 0xd0e
00000C7E  833E0E0901        cmp word [0x90e],byte +0x1
00000C83  7430              jz 0xcb5
00000C85  8B1EC809          mov bx,[0x9c8]
00000C89  B104              mov cl,0x4
00000C8B  D3E3              shl bx,cl
00000C8D  8BC7              mov ax,di
00000C8F  250F00            and ax,0xf
00000C92  03D8              add bx,ax
00000C94  8A875401          mov al,[bx+0x154]
00000C98  98                cbw
00000C99  8BF8              mov di,ax
00000C9B  0BF6              or si,si
00000C9D  7C16              jl 0xcb5
00000C9F  8B1EC809          mov bx,[0x9c8]
00000CA3  B104              mov cl,0x4
00000CA5  D3E3              shl bx,cl
00000CA7  8BC6              mov ax,si
00000CA9  250F00            and ax,0xf
00000CAC  03D8              add bx,ax
00000CAE  8A875401          mov al,[bx+0x154]
00000CB2  98                cbw
00000CB3  8BF0              mov si,ax
00000CB5  0BF6              or si,si
00000CB7  7C3A              jl 0xcf3
00000CB9  56                push si
00000CBA  9AC305CD23        call 0x23cd:0x5c3
00000CBF  59                pop cx
00000CC0  A04301            mov al,[0x143]
00000CC3  98                cbw
00000CC4  50                push ax
00000CC5  FF760C            push word [bp+0xc]
00000CC8  FF760A            push word [bp+0xa]
00000CCB  9AE5420000        call 0x0:0x42e5
00000CD0  59                pop cx
00000CD1  59                pop cx
00000CD2  50                push ax
00000CD3  A04201            mov al,[0x142]
00000CD6  98                cbw
00000CD7  8BD0              mov dx,ax
00000CD9  58                pop ax
00000CDA  F7EA              imul dx
00000CDC  50                push ax
00000CDD  A04301            mov al,[0x143]
00000CE0  98                cbw
00000CE1  8B5608            mov dx,[bp+0x8]
00000CE4  2BD0              sub dx,ax
00000CE6  42                inc dx
00000CE7  52                push dx
00000CE8  FF7606            push word [bp+0x6]
00000CEB  9A9B06CD23        call 0x23cd:0x69b
00000CF0  83C408            add sp,byte +0x8
00000CF3  57                push di
00000CF4  9AC305CD23        call 0x23cd:0x5c3
00000CF9  59                pop cx
00000CFA  FF760C            push word [bp+0xc]
00000CFD  FF760A            push word [bp+0xa]
00000D00  FF7608            push word [bp+0x8]
00000D03  FF7606            push word [bp+0x6]
00000D06  9ADD05CD23        call 0x23cd:0x5dd
00000D0B  83C408            add sp,byte +0x8
00000D0E  5F                pop di
00000D0F  5E                pop si
