; segment 1DEF  file offset 0x214F0  size 0x8F0
00000000  00A72159          add [bx+0x5921],ah
00000004  59                pop cx
00000005  C6060A0F00        mov byte [0xf0a],0x0
0000000A  5D                pop bp
0000000B  CB                retf

; ---- 1DEF:000C ----
0000000C  55                push bp
0000000D  8BEC              mov bp,sp
0000000F  81ECD200          sub sp,0xd2
00000013  8D46AC            lea ax,[bp-0x54]
00000016  A3A654            mov [0x54a6],ax
00000019  8C16A854          mov [0x54a8],ss
0000001D  8D8658FF          lea ax,[bp-0xa8]
00000021  A37C54            mov [0x547c],ax
00000024  8C167E54          mov [0x547e],ss
00000028  8D862EFF          lea ax,[bp-0xd2]
0000002C  A3A254            mov [0x54a2],ax
0000002F  8C16A454          mov [0x54a4],ss
00000033  9ADB00EF1D        call 0x1def:0xdb
00000038  0AC0              or al,al
0000003A  7503              jnz 0x3f
0000003C  E99800            jmp 0xd7
0000003F  FF36CE08          push word [0x8ce]
00000043  9A02006C18        call 0x186c:0x2
00000048  59                pop cx
00000049  9A2D1F0906        call 0x609:0x1f2d
0000004E  33C0              xor ax,ax
00000050  50                push ax
00000051  9A7D03FD21        call 0x21fd:0x37d
00000056  59                pop cx
00000057  1E                push ds
00000058  B8A800            mov ax,0xa8
0000005B  50                push ax
0000005C  9A6C02FD21        call 0x21fd:0x26c
00000061  59                pop cx
00000062  59                pop cx
00000063  9AE7110906        call 0x609:0x11e7
00000068  9A0501EF1D        call 0x1def:0x105
0000006D  33C0              xor ax,ax
0000006F  50                push ax
00000070  9A7D03FD21        call 0x21fd:0x37d
00000075  59                pop cx
00000076  9A7301EF1D        call 0x1def:0x173
0000007B  9AAF01EF1D        call 0x1def:0x1af
00000080  9A0E02EF1D        call 0x1def:0x20e
00000085  9A2702EF1D        call 0x1def:0x227
0000008A  9ACD03EF1D        call 0x1def:0x3cd
0000008F  9A2903FD21        call 0x21fd:0x329
00000094  33C0              xor ax,ax
00000096  BA1400            mov dx,0x14
00000099  50                push ax
0000009A  52                push dx
0000009B  9A8501FD21        call 0x21fd:0x185
000000A0  59                pop cx
000000A1  59                pop cx
000000A2  C70680540000      mov word [0x5480],0x0
000000A8  9A1B00960A        call 0xa96:0x1b
000000AD  9AA904EF1D        call 0x1def:0x4a9
000000B2  33C0              xor ax,ax
000000B4  50                push ax
000000B5  9A0400FD21        call 0x21fd:0x4
000000BA  59                pop cx
000000BB  C606AA3700        mov byte [0x37aa],0x0
000000C0  EB09              jmp short 0xcb
000000C2  9ABA210906        call 0x609:0x21ba
000000C7  FE06AA37          inc byte [0x37aa]
000000CB  803EAA3704        cmp byte [0x37aa],0x4
000000D0  7CF0              jl 0xc2
000000D2  9A5107EF1D        call 0x1def:0x751
000000D7  8BE5              mov sp,bp
000000D9  5D                pop bp
000000DA  CB                retf

; ---- 1DEF:00DB ----
000000DB  55                push bp
000000DC  8BEC              mov bp,sp
000000DE  B81400            mov ax,0x14
000000E1  50                push ax
000000E2  9ABA01FD21        call 0x21fd:0x1ba
000000E7  59                pop cx
000000E8  0BC0              or ax,ax
000000EA  7504              jnz 0xf0
000000EC  B001              mov al,0x1
000000EE  EB13              jmp short 0x103
000000F0  A1F257            mov ax,[0x57f2]
000000F3  D1E0              shl ax,1
000000F5  3B06F057          cmp ax,[0x57f0]
000000F9  7D04              jnl 0xff
000000FB  B001              mov al,0x1
000000FD  EB04              jmp short 0x103
000000FF  B000              mov al,0x0
00000101  EB00              jmp short 0x103
00000103  5D                pop bp
00000104  CB                retf

; ---- 1DEF:0105 ----
00000105  55                push bp
00000106  8BEC              mov bp,sp
00000108  9A1B00960A        call 0xa96:0x1b
0000010D  9AD91E0906        call 0x609:0x1ed9
00000112  B83200            mov ax,0x32
00000115  50                push ax
00000116  B8F000            mov ax,0xf0
00000119  50                push ax
0000011A  B84B00            mov ax,0x4b
0000011D  50                push ax
0000011E  B82800            mov ax,0x28
00000121  50                push ax
00000122  9A56120906        call 0x609:0x1256
00000127  83C408            add sp,byte +0x8
0000012A  9AA806960A        call 0xa96:0x6a8
0000012F  FF362E08          push word [0x82e]
00000133  9A3F0C960A        call 0xa96:0xc3f
00000138  59                pop cx
00000139  9A2903FD21        call 0x21fd:0x329
0000013E  B80500            mov ax,0x5
00000141  50                push ax
00000142  B80500            mov ax,0x5
00000145  50                push ax
00000146  B80200            mov ax,0x2
00000149  50                push ax
0000014A  1E                push ds
0000014B  B85C3B            mov ax,0x3b5c
0000014E  50                push ax
0000014F  B8F000            mov ax,0xf0
00000152  50                push ax
00000153  B86400            mov ax,0x64
00000156  50                push ax
00000157  B82800            mov ax,0x28
0000015A  50                push ax
0000015B  9A8802690A        call 0xa69:0x288
00000160  83C410            add sp,byte +0x10
00000163  33C0              xor ax,ax
00000165  BA1400            mov dx,0x14
00000168  50                push ax
00000169  52                push dx
0000016A  9A8501FD21        call 0x21fd:0x185
0000016F  59                pop cx
00000170  59                pop cx
00000171  5D                pop bp
00000172  CB                retf

; ---- 1DEF:0173 ----
00000173  55                push bp
00000174  8BEC              mov bp,sp
00000176  56                push si
00000177  33F6              xor si,si
00000179  EB2C              jmp short 0x1a7
0000017B  B80600            mov ax,0x6
0000017E  50                push ax
0000017F  9ABA01FD21        call 0x21fd:0x1ba
00000184  59                pop cx
00000185  8BD6              mov dx,si
00000187  D1E2              shl dx,1
00000189  C41EA654          les bx,[0x54a6]
0000018D  03DA              add bx,dx
0000018F  268907            mov [es:bx],ax
00000192  3D0300            cmp ax,0x3
00000195  7E0F              jng 0x1a6
00000197  8BC6              mov ax,si
00000199  D1E0              shl ax,1
0000019B  C41EA654          les bx,[0x54a6]
0000019F  03D8              add bx,ax
000001A1  26C7070000        mov word [es:bx],0x0
000001A6  46                inc si
000001A7  83FE29            cmp si,byte +0x29
000001AA  7ECF              jng 0x17b
000001AC  5E                pop si
000001AD  5D                pop bp
000001AE  CB                retf

; ---- 1DEF:01AF ----
000001AF  55                push bp
000001B0  8BEC              mov bp,sp
000001B2  56                push si
000001B3  57                push di
000001B4  33F6              xor si,si
000001B6  EB10              jmp short 0x1c8
000001B8  8BC6              mov ax,si
000001BA  D1E0              shl ax,1
000001BC  C41E7C54          les bx,[0x547c]
000001C0  03D8              add bx,ax
000001C2  26C707FFFF        mov word [es:bx],0xffff
000001C7  46                inc si
000001C8  83FE29            cmp si,byte +0x29
000001CB  7EEB              jng 0x1b8
000001CD  33F6              xor si,si
000001CF  EB34              jmp short 0x205
000001D1  B82900            mov ax,0x29
000001D4  50                push ax
000001D5  9ABA01FD21        call 0x21fd:0x1ba
000001DA  59                pop cx
000001DB  8BF8              mov di,ax
000001DD  8BC7              mov ax,di
000001DF  D1E0              shl ax,1
000001E1  C41E7C54          les bx,[0x547c]
000001E5  03D8              add bx,ax
000001E7  26833FFF          cmp word [es:bx],byte -0x1
000001EB  75E4              jnz 0x1d1
000001ED  B80300            mov ax,0x3
000001F0  50                push ax
000001F1  9ABA01FD21        call 0x21fd:0x1ba
000001F6  59                pop cx
000001F7  8BD7              mov dx,di
000001F9  D1E2              shl dx,1
000001FB  C41E7C54          les bx,[0x547c]
000001FF  03DA              add bx,dx
00000201  268907            mov [es:bx],ax
00000204  46                inc si
00000205  83FE18            cmp si,byte +0x18
00000208  7EC7              jng 0x1d1
0000020A  5F                pop di
0000020B  5E                pop si
0000020C  5D                pop bp
0000020D  CB                retf

; ---- 1DEF:020E ----
0000020E  55                push bp
0000020F  8BEC              mov bp,sp
00000211  56                push si
00000212  33F6              xor si,si
00000214  EB09              jmp short 0x21f
00000216  C41EA254          les bx,[0x54a2]
0000021A  26C600FF          mov byte [es:bx+si],0xff
0000021E  46                inc si
0000021F  83FE29            cmp si,byte +0x29
00000222  7EF2              jng 0x216
00000224  5E                pop si
00000225  5D                pop bp
00000226  CB                retf

; ---- 1DEF:0227 ----
00000227  55                push bp
00000228  8BEC              mov bp,sp
0000022A  56                push si
0000022B  9A1B00960A        call 0xa96:0x1b
00000230  9AD91E0906        call 0x609:0x1ed9
00000235  B8C000            mov ax,0xc0
00000238  50                push ax
00000239  B85000            mov ax,0x50
0000023C  50                push ax
0000023D  B80400            mov ax,0x4
00000240  50                push ax
00000241  33C0              xor ax,ax
00000243  50                push ax
00000244  9A56120906        call 0x609:0x1256
00000249  83C408            add sp,byte +0x8
0000024C  B89A00            mov ax,0x9a
0000024F  50                push ax
00000250  B8C000            mov ax,0xc0
00000253  50                push ax
00000254  B81700            mov ax,0x17
00000257  50                push ax
00000258  B87000            mov ax,0x70
0000025B  50                push ax
0000025C  9AC5130906        call 0x609:0x13c5
00000261  83C408            add sp,byte +0x8
00000264  33F6              xor si,si
00000266  EB08              jmp short 0x270
00000268  56                push si
00000269  9A0803EF1D        call 0x1def:0x308
0000026E  59                pop cx
0000026F  46                inc si
00000270  83FE29            cmp si,byte +0x29
00000273  7EF3              jng 0x268
00000275  9AA806960A        call 0xa96:0x6a8
0000027A  9A1B00960A        call 0xa96:0x1b
0000027F  C606AA3700        mov byte [0x37aa],0x0
00000284  EB69              jmp short 0x2ef
00000286  A0AA37            mov al,[0x37aa]
00000289  98                cbw
0000028A  BA2C00            mov dx,0x2c
0000028D  F7EA              imul dx
0000028F  050C00            add ax,0xc
00000292  50                push ax
00000293  B81000            mov ax,0x10
00000296  50                push ax
00000297  B82C00            mov ax,0x2c
0000029A  50                push ax
0000029B  B83000            mov ax,0x30
0000029E  50                push ax
0000029F  B8B000            mov ax,0xb0
000002A2  50                push ax
000002A3  B8C000            mov ax,0xc0
000002A6  50                push ax
000002A7  9A3000960A        call 0xa96:0x30
000002AC  83C40C            add sp,byte +0xc
000002AF  C706D0450000      mov word [0x45d0],0x0
000002B5  EB2D              jmp short 0x2e4
000002B7  A0AA37            mov al,[0x37aa]
000002BA  98                cbw
000002BB  BA2C00            mov dx,0x2c
000002BE  F7EA              imul dx
000002C0  8B16D045          mov dx,[0x45d0]
000002C4  D1E2              shl dx,1
000002C6  D1E2              shl dx,1
000002C8  D1E2              shl dx,1
000002CA  03C2              add ax,dx
000002CC  051300            add ax,0x13
000002CF  50                push ax
000002D0  B81000            mov ax,0x10
000002D3  50                push ax
000002D4  FF36D045          push word [0x45d0]
000002D8  9A12190906        call 0x609:0x1912
000002DD  83C406            add sp,byte +0x6
000002E0  FF06D045          inc word [0x45d0]
000002E4  833ED04503        cmp word [0x45d0],byte +0x3
000002E9  7ECC              jng 0x2b7
000002EB  FE06AA37          inc byte [0x37aa]
000002EF  803EAA3704        cmp byte [0x37aa],0x4
000002F4  7C90              jl 0x286
000002F6  9AA806960A        call 0xa96:0x6a8
000002FB  FF362E08          push word [0x82e]
000002FF  9A3F0C960A        call 0xa96:0xc3f
00000304  59                pop cx
00000305  5E                pop si
00000306  5D                pop bp
00000307  CB                retf

; ---- 1DEF:0308 ----
00000308  55                push bp
00000309  8BEC              mov bp,sp
0000030B  83EC04            sub sp,byte +0x4
0000030E  56                push si
0000030F  57                push di
00000310  8B4606            mov ax,[bp+0x6]
00000313  D1E0              shl ax,1
00000315  C41EA654          les bx,[0x54a6]
00000319  03D8              add bx,ax
0000031B  268B07            mov ax,[es:bx]
0000031E  8946FE            mov [bp-0x2],ax
00000321  8B4606            mov ax,[bp+0x6]
00000324  D1E0              shl ax,1
00000326  C41E7C54          les bx,[0x547c]
0000032A  03D8              add bx,ax
0000032C  268B37            mov si,[es:bx]
0000032F  8B4606            mov ax,[bp+0x6]
00000332  BB0600            mov bx,0x6
00000335  99                cwd
00000336  F7FB              idiv bx
00000338  8956FC            mov [bp-0x4],dx
0000033B  8B4606            mov ax,[bp+0x6]
0000033E  BB0600            mov bx,0x6
00000341  99                cwd
00000342  F7FB              idiv bx
00000344  8BF8              mov di,ax
00000346  8BC7              mov ax,di
00000348  BA1600            mov dx,0x16
0000034B  F7EA              imul dx
0000034D  051700            add ax,0x17
00000350  50                push ax
00000351  8B46FC            mov ax,[bp-0x4]
00000354  B105              mov cl,0x5
00000356  D3E0              shl ax,cl
00000358  057000            add ax,0x70
0000035B  50                push ax
0000035C  B81600            mov ax,0x16
0000035F  50                push ax
00000360  B82000            mov ax,0x20
00000363  50                push ax
00000364  8B5EFE            mov bx,[bp-0x2]
00000367  D1E3              shl bx,1
00000369  FFB72A0F          push word [bx+0xf2a]
0000036D  8B5EFE            mov bx,[bp-0x2]
00000370  D1E3              shl bx,1
00000372  FFB7220F          push word [bx+0xf22]
00000376  9A3000960A        call 0xa96:0x30
0000037B  83C40C            add sp,byte +0xc
0000037E  83FEFF            cmp si,byte -0x1
00000381  7444              jz 0x3c7
00000383  8BC7              mov ax,di
00000385  BA1600            mov dx,0x16
00000388  F7EA              imul dx
0000038A  052600            add ax,0x26
0000038D  50                push ax
0000038E  8B46FC            mov ax,[bp-0x4]
00000391  B105              mov cl,0x5
00000393  D3E0              shl ax,cl
00000395  057800            add ax,0x78
00000398  50                push ax
00000399  B80700            mov ax,0x7
0000039C  50                push ax
0000039D  B81000            mov ax,0x10
000003A0  50                push ax
000003A1  8BC6              mov ax,si
000003A3  BA0700            mov dx,0x7
000003A6  F7EA              imul dx
000003A8  052800            add ax,0x28
000003AB  50                push ax
000003AC  B88000            mov ax,0x80
000003AF  50                push ax
000003B0  8BC6              mov ax,si
000003B2  BA0700            mov dx,0x7
000003B5  F7EA              imul dx
000003B7  05B000            add ax,0xb0
000003BA  50                push ax
000003BB  B83001            mov ax,0x130
000003BE  50                push ax
000003BF  9A3B01960A        call 0xa96:0x13b
000003C4  83C410            add sp,byte +0x10
000003C7  5F                pop di
000003C8  5E                pop si
000003C9  8BE5              mov sp,bp
000003CB  5D                pop bp
000003CC  CB                retf

; ---- 1DEF:03CD ----
000003CD  55                push bp
000003CE  8BEC              mov bp,sp
000003D0  C606AA3700        mov byte [0x37aa],0x0
000003D5  E9C500            jmp 0x49d
000003D8  A0AA37            mov al,[0x37aa]
000003DB  98                cbw
000003DC  D1E0              shl ax,1
000003DE  8BD8              mov bx,ax
000003E0  C787D24B0000      mov word [bx+0x4bd2],0x0
000003E6  A0AA37            mov al,[0x37aa]
000003E9  98                cbw
000003EA  D1E0              shl ax,1
000003EC  8BD8              mov bx,ax
000003EE  C78722300000      mov word [bx+0x3022],0x0
000003F4  B80500            mov ax,0x5
000003F7  50                push ax
000003F8  9ABA01FD21        call 0x21fd:0x1ba
000003FD  59                pop cx
000003FE  50                push ax
000003FF  A0AA37            mov al,[0x37aa]
00000402  98                cbw
00000403  8BD8              mov bx,ax
00000405  58                pop ax
00000406  88874E30          mov [bx+0x304e],al
0000040A  B80600            mov ax,0x6
0000040D  50                push ax
0000040E  9ABA01FD21        call 0x21fd:0x1ba
00000413  59                pop cx
00000414  50                push ax
00000415  A0AA37            mov al,[0x37aa]
00000418  98                cbw
00000419  8BD8              mov bx,ax
0000041B  58                pop ax
0000041C  88875A30          mov [bx+0x305a],al
00000420  A0AA37            mov al,[0x37aa]
00000423  98                cbw
00000424  8BD8              mov bx,ax
00000426  8A874E30          mov al,[bx+0x304e]
0000042A  98                cbw
0000042B  B105              mov cl,0x5
0000042D  D3E0              shl ax,cl
0000042F  057800            add ax,0x78
00000432  50                push ax
00000433  A0AA37            mov al,[0x37aa]
00000436  98                cbw
00000437  BA1A00            mov dx,0x1a
0000043A  F7EA              imul dx
0000043C  8BD8              mov bx,ax
0000043E  58                pop ax
0000043F  89872033          mov [bx+0x3320],ax
00000443  A0AA37            mov al,[0x37aa]
00000446  98                cbw
00000447  8BD8              mov bx,ax
00000449  8A875A30          mov al,[bx+0x305a]
0000044D  98                cbw
0000044E  BA1600            mov dx,0x16
00000451  F7EA              imul dx
00000453  052300            add ax,0x23
00000456  50                push ax
00000457  A0AA37            mov al,[0x37aa]
0000045A  98                cbw
0000045B  BA1A00            mov dx,0x1a
0000045E  F7EA              imul dx
00000460  8BD8              mov bx,ax
00000462  58                pop ax
00000463  89872233          mov [bx+0x3322],ax
00000467  9ACC1F0906        call 0x609:0x1fcc
0000046C  C706D0450000      mov word [0x45d0],0x0
00000472  EB1E              jmp short 0x492
00000474  A0AA37            mov al,[0x37aa]
00000477  98                cbw
00000478  D1E0              shl ax,1
0000047A  D1E0              shl ax,1
0000047C  D1E0              shl ax,1
0000047E  8B16D045          mov dx,[0x45d0]
00000482  D1E2              shl dx,1
00000484  03C2              add ax,dx
00000486  8BD8              mov bx,ax
00000488  C78782540000      mov word [bx+0x5482],0x0
0000048E  FF06D045          inc word [0x45d0]
00000492  833ED04503        cmp word [0x45d0],byte +0x3
00000497  7EDB              jng 0x474
00000499  FE06AA37          inc byte [0x37aa]
0000049D  803EAA3704        cmp byte [0x37aa],0x4
000004A2  7D03              jnl 0x4a7
000004A4  E931FF            jmp 0x3d8
000004A7  5D                pop bp
000004A8  CB                retf

; ---- 1DEF:04A9 ----
000004A9  55                push bp
000004AA  8BEC              mov bp,sp
000004AC  E98701            jmp 0x636
000004AF  9A1F05FD21        call 0x21fd:0x51f
000004B4  FF362E08          push word [0x82e]
000004B8  9A1C07CD23        call 0x23cd:0x71c
000004BD  59                pop cx
000004BE  9AA806960A        call 0xa96:0x6a8
000004C3  9A1B00960A        call 0xa96:0x1b
000004C8  33C0              xor ax,ax
000004CA  BA0300            mov dx,0x3
000004CD  50                push ax
000004CE  52                push dx
000004CF  9A8501FD21        call 0x21fd:0x185
000004D4  59                pop cx
000004D5  59                pop cx
000004D6  9A7A039E0E        call 0xe9e:0x37a
000004DB  9A4502FD21        call 0x21fd:0x245
000004E0  C606AA3700        mov byte [0x37aa],0x0
000004E5  E93501            jmp 0x61d
000004E8  A0AA37            mov al,[0x37aa]
000004EB  98                cbw
000004EC  D1E0              shl ax,1
000004EE  8BD8              mov bx,ax
000004F0  83BF223000        cmp word [bx+0x3022],byte +0x0
000004F5  7408              jz 0x4ff
000004F7  9AE01F0906        call 0x609:0x1fe0
000004FC  E91A01            jmp 0x619
000004FF  9A5106EF1D        call 0x1def:0x651
00000504  A0AA37            mov al,[0x37aa]
00000507  98                cbw
00000508  8BD8              mov bx,ax
0000050A  80BF400801        cmp byte [bx+0x840],0x1
0000050F  7505              jnz 0x516
00000511  9AAD07EF1D        call 0x1def:0x7ad
00000516  A0AA37            mov al,[0x37aa]
00000519  98                cbw
0000051A  8BD8              mov bx,ax
0000051C  8A870833          mov al,[bx+0x3308]
00000520  98                cbw
00000521  40                inc ax
00000522  8BD8              mov bx,ax
00000524  83FB04            cmp bx,byte +0x4
00000527  7603              jna 0x52c
00000529  E9ED00            jmp 0x619
0000052C  D1E3              shl bx,1
0000052E  2EFFA74706        jmp [cs:bx+0x647]
00000533  A0AA37            mov al,[0x37aa]
00000536  98                cbw
00000537  8BD8              mov bx,ax
00000539  80BF4E3000        cmp byte [bx+0x304e],0x0
0000053E  7503              jnz 0x543
00000540  E9D600            jmp 0x619
00000543  A0AA37            mov al,[0x37aa]
00000546  98                cbw
00000547  D1E0              shl ax,1
00000549  8BD8              mov bx,ax
0000054B  C787CA4B0100      mov word [bx+0x4bca],0x1
00000551  A0AA37            mov al,[0x37aa]
00000554  98                cbw
00000555  D1E0              shl ax,1
00000557  8BD8              mov bx,ax
00000559  C78722301100      mov word [bx+0x3022],0x11
0000055F  A0AA37            mov al,[0x37aa]
00000562  98                cbw
00000563  8BD8              mov bx,ax
00000565  FE8F4E30          dec byte [bx+0x304e]
00000569  E9AD00            jmp 0x619
0000056C  A0AA37            mov al,[0x37aa]
0000056F  98                cbw
00000570  8BD8              mov bx,ax
00000572  80BF4E3005        cmp byte [bx+0x304e],0x5
00000577  7503              jnz 0x57c
00000579  E99D00            jmp 0x619
0000057C  A0AA37            mov al,[0x37aa]
0000057F  98                cbw
00000580  D1E0              shl ax,1
00000582  8BD8              mov bx,ax
00000584  C787CA4B0000      mov word [bx+0x4bca],0x0
0000058A  A0AA37            mov al,[0x37aa]
0000058D  98                cbw
0000058E  D1E0              shl ax,1
00000590  8BD8              mov bx,ax
00000592  C78722301100      mov word [bx+0x3022],0x11
00000598  A0AA37            mov al,[0x37aa]
0000059B  98                cbw
0000059C  8BD8              mov bx,ax
0000059E  FE874E30          inc byte [bx+0x304e]
000005A2  EB75              jmp short 0x619
000005A4  A0AA37            mov al,[0x37aa]
000005A7  98                cbw
000005A8  8BD8              mov bx,ax
000005AA  80BF5A3000        cmp byte [bx+0x305a],0x0
000005AF  7502              jnz 0x5b3
000005B1  EB66              jmp short 0x619
000005B3  A0AA37            mov al,[0x37aa]
000005B6  98                cbw
000005B7  D1E0              shl ax,1
000005B9  8BD8              mov bx,ax
000005BB  C787CA4B0200      mov word [bx+0x4bca],0x2
000005C1  A0AA37            mov al,[0x37aa]
000005C4  98                cbw
000005C5  D1E0              shl ax,1
000005C7  8BD8              mov bx,ax
000005C9  C78722300C00      mov word [bx+0x3022],0xc
000005CF  A0AA37            mov al,[0x37aa]
000005D2  98                cbw
000005D3  8BD8              mov bx,ax
000005D5  FE8F5A30          dec byte [bx+0x305a]
000005D9  EB3E              jmp short 0x619
000005DB  A0AA37            mov al,[0x37aa]
000005DE  98                cbw
000005DF  8BD8              mov bx,ax
000005E1  80BF5A3006        cmp byte [bx+0x305a],0x6
000005E6  7502              jnz 0x5ea
000005E8  EB2F              jmp short 0x619
000005EA  A0AA37            mov al,[0x37aa]
000005ED  98                cbw
000005EE  D1E0              shl ax,1
000005F0  8BD8              mov bx,ax
000005F2  C787CA4B0300      mov word [bx+0x4bca],0x3
000005F8  A0AA37            mov al,[0x37aa]
000005FB  98                cbw
000005FC  D1E0              shl ax,1
000005FE  8BD8              mov bx,ax
00000600  C78722300C00      mov word [bx+0x3022],0xc
00000606  A0AA37            mov al,[0x37aa]
00000609  98                cbw
0000060A  8BD8              mov bx,ax
0000060C  FE875A30          inc byte [bx+0x305a]
00000610  EB07              jmp short 0x619
00000612  9A5E210906        call 0x609:0x215e
00000617  EB00              jmp short 0x619
00000619  FE06AA37          inc byte [0x37aa]
0000061D  803EAA3704        cmp byte [0x37aa],0x4
00000622  7D03              jnl 0x627
00000624  E9C1FE            jmp 0x4e8
00000627  9AA806960A        call 0xa96:0x6a8
0000062C  FF362E08          push word [0x82e]
00000630  9AE906CD23        call 0x23cd:0x6e9
00000635  59                pop cx
00000636  833E805419        cmp word [0x5480],byte +0x19
0000063B  7D03              jnl 0x640
0000063D  E96FFE            jmp 0x4af
00000640  9A6506FD21        call 0x21fd:0x665
00000645  5D                pop bp
00000646  CB                retf
00000647  12066C05          adc al,[0x56c]
0000064B  3305              xor ax,[di]
0000064D  A4                movsb
0000064E  05DB05            add ax,0x5db

; ---- 1DEF:0651 ----
00000651  55                push bp
00000652  8BEC              mov bp,sp
00000654  56                push si
00000655  A0AA37            mov al,[0x37aa]
00000658  98                cbw
00000659  8BD8              mov bx,ax
0000065B  8A875A30          mov al,[bx+0x305a]
0000065F  98                cbw
00000660  BA0600            mov dx,0x6
00000663  F7EA              imul dx
00000665  50                push ax
00000666  A0AA37            mov al,[0x37aa]
00000669  98                cbw
0000066A  8BD8              mov bx,ax
0000066C  8A874E30          mov al,[bx+0x304e]
00000670  98                cbw
00000671  5A                pop dx
00000672  03D0              add dx,ax
00000674  8BF2              mov si,dx
00000676  8BC6              mov ax,si
00000678  D1E0              shl ax,1
0000067A  C41E7C54          les bx,[0x547c]
0000067E  03D8              add bx,ax
00000680  268B07            mov ax,[es:bx]
00000683  A3D045            mov [0x45d0],ax
00000686  833ED045FF        cmp word [0x45d0],byte -0x1
0000068B  7502              jnz 0x68f
0000068D  EB66              jmp short 0x6f5
0000068F  FF068054          inc word [0x5480]
00000693  A0AA37            mov al,[0x37aa]
00000696  98                cbw
00000697  D1E0              shl ax,1
00000699  D1E0              shl ax,1
0000069B  D1E0              shl ax,1
0000069D  8B16D045          mov dx,[0x45d0]
000006A1  D1E2              shl dx,1
000006A3  03C2              add ax,dx
000006A5  8BD8              mov bx,ax
000006A7  8B878254          mov ax,[bx+0x5482]
000006AB  50                push ax
000006AC  A0262F            mov al,[0x2f26]
000006AF  98                cbw
000006B0  D1E0              shl ax,1
000006B2  5A                pop dx
000006B3  03D0              add dx,ax
000006B5  A0AA37            mov al,[0x37aa]
000006B8  98                cbw
000006B9  D1E0              shl ax,1
000006BB  D1E0              shl ax,1
000006BD  D1E0              shl ax,1
000006BF  8B1ED045          mov bx,[0x45d0]
000006C3  D1E3              shl bx,1
000006C5  03C3              add ax,bx
000006C7  8BD8              mov bx,ax
000006C9  89978254          mov [bx+0x5482],dx
000006CD  9AF806EF1D        call 0x1def:0x6f8
000006D2  8BC6              mov ax,si
000006D4  D1E0              shl ax,1
000006D6  C41E7C54          les bx,[0x547c]
000006DA  03D8              add bx,ax
000006DC  26C707FFFF        mov word [es:bx],0xffff
000006E1  56                push si
000006E2  0E                push cs
000006E3  E822FC            call 0x308
000006E6  59                pop cx
000006E7  B83100            mov ax,0x31
000006EA  50                push ax
000006EB  33C0              xor ax,ax
000006ED  50                push ax
000006EE  9A5400A721        call 0x21a7:0x54
000006F3  59                pop cx
000006F4  59                pop cx
000006F5  5E                pop si
000006F6  5D                pop bp
000006F7  CB                retf

; ---- 1DEF:06F8 ----
000006F8  55                push bp
000006F9  8BEC              mov bp,sp
000006FB  B80500            mov ax,0x5
000006FE  50                push ax
000006FF  A0AA37            mov al,[0x37aa]
00000702  98                cbw
00000703  D1E0              shl ax,1
00000705  8BD8              mov bx,ax
00000707  FFB7C800          push word [bx+0xc8]
0000070B  B80300            mov ax,0x3
0000070E  50                push ax
0000070F  A0AA37            mov al,[0x37aa]
00000712  98                cbw
00000713  D1E0              shl ax,1
00000715  D1E0              shl ax,1
00000717  D1E0              shl ax,1
00000719  8B16D045          mov dx,[0x45d0]
0000071D  D1E2              shl dx,1
0000071F  03C2              add ax,dx
00000721  8BD8              mov bx,ax
00000723  8B878254          mov ax,[bx+0x5482]
00000727  99                cwd
00000728  52                push dx
00000729  50                push ax
0000072A  A0AA37            mov al,[0x37aa]
0000072D  98                cbw
0000072E  BA2C00            mov dx,0x2c
00000731  F7EA              imul dx
00000733  8B16D045          mov dx,[0x45d0]
00000737  D1E2              shl dx,1
00000739  D1E2              shl dx,1
0000073B  D1E2              shl dx,1
0000073D  03C2              add ax,dx
0000073F  051800            add ax,0x18
00000742  50                push ax
00000743  B82200            mov ax,0x22
00000746  50                push ax
00000747  9A7A05960A        call 0xa96:0x57a
0000074C  83C40E            add sp,byte +0xe
0000074F  5D                pop bp
00000750  CB                retf

; ---- 1DEF:0751 ----
00000751  55                push bp
00000752  8BEC              mov bp,sp
00000754  C606AA3700        mov byte [0x37aa],0x0
00000759  EB49              jmp short 0x7a4
0000075B  C706D0450000      mov word [0x45d0],0x0
00000761  EB36              jmp short 0x799
00000763  A0AA37            mov al,[0x37aa]
00000766  98                cbw
00000767  D1E0              shl ax,1
00000769  D1E0              shl ax,1
0000076B  D1E0              shl ax,1
0000076D  8B16D045          mov dx,[0x45d0]
00000771  D1E2              shl dx,1
00000773  03C2              add ax,dx
00000775  8BD8              mov bx,ax
00000777  8B878254          mov ax,[bx+0x5482]
0000077B  50                push ax
0000077C  A0AA37            mov al,[0x37aa]
0000077F  98                cbw
00000780  D1E0              shl ax,1
00000782  D1E0              shl ax,1
00000784  D1E0              shl ax,1
00000786  8B16D045          mov dx,[0x45d0]
0000078A  D1E2              shl dx,1
0000078C  03C2              add ax,dx
0000078E  8BD8              mov bx,ax
00000790  58                pop ax
00000791  0187F43C          add [bx+0x3cf4],ax
00000795  FF06D045          inc word [0x45d0]
00000799  833ED04503        cmp word [0x45d0],byte +0x3
0000079E  7EC3              jng 0x763
000007A0  FE06AA37          inc byte [0x37aa]
000007A4  803EAA3704        cmp byte [0x37aa],0x4
000007A9  7CB0              jl 0x75b
000007AB  5D                pop bp
000007AC  CB                retf

; ---- 1DEF:07AD ----
000007AD  55                push bp
000007AE  8BEC              mov bp,sp
000007B0  83EC0A            sub sp,byte +0xa
000007B3  C646FE64          mov byte [bp-0x2],0x64
000007B7  C646FD00          mov byte [bp-0x3],0x0
000007BB  A0AA37            mov al,[0x37aa]
000007BE  98                cbw
000007BF  8BD8              mov bx,ax
000007C1  8A874E30          mov al,[bx+0x304e]
000007C5  98                cbw
000007C6  8946F8            mov [bp-0x8],ax
000007C9  A0AA37            mov al,[0x37aa]
000007CC  98                cbw
000007CD  8BD8              mov bx,ax
000007CF  8A875A30          mov al,[bx+0x305a]
000007D3  98                cbw
000007D4  8946F6            mov [bp-0xa],ax
000007D7  33C9              xor cx,cx
000007D9  E99500            jmp 0x871
000007DC  8BC1              mov ax,cx
000007DE  D1E0              shl ax,1
000007E0  C41E7C54          les bx,[0x547c]
000007E4  03D8              add bx,ax
000007E6  26833FFF          cmp word [es:bx],byte -0x1
000007EA  7503              jnz 0x7ef
000007EC  E98100            jmp 0x870
000007EF  C41EA254          les bx,[0x54a2]
000007F3  03D9              add bx,cx
000007F5  26803FFF          cmp byte [es:bx],0xff
000007F9  7411              jz 0x80c
000007FB  C41EA254          les bx,[0x54a2]
000007FF  03D9              add bx,cx
00000801  268A07            mov al,[es:bx]
00000804  3A06AA37          cmp al,[0x37aa]
00000808  7402              jz 0x80c
0000080A  EB64              jmp short 0x870
0000080C  8BC1              mov ax,cx
0000080E  BB0600            mov bx,0x6
00000811  99                cwd
00000812  F7FB              idiv bx
00000814  8856FC            mov [bp-0x4],dl
00000817  8A46FC            mov al,[bp-0x4]
0000081A  98                cbw
0000081B  3B46F8            cmp ax,[bp-0x8]
0000081E  7D08              jnl 0x828
00000820  8A46F8            mov al,[bp-0x8]
00000823  2A46FC            sub al,[bp-0x4]
00000826  EB06              jmp short 0x82e
00000828  8A46FC            mov al,[bp-0x4]
0000082B  2A46F8            sub al,[bp-0x8]
0000082E  8846FF            mov [bp-0x1],al
00000831  8BC1              mov ax,cx
00000833  BB0600            mov bx,0x6
00000836  99                cwd
00000837  F7FB              idiv bx
00000839  8846FB            mov [bp-0x5],al
0000083C  8A46FB            mov al,[bp-0x5]
0000083F  98                cbw
00000840  3B46F6            cmp ax,[bp-0xa]
00000843  7D08              jnl 0x84d
00000845  8A46F6            mov al,[bp-0xa]
00000848  2A46FB            sub al,[bp-0x5]
0000084B  EB06              jmp short 0x853
0000084D  8A46FB            mov al,[bp-0x5]
00000850  2A46F6            sub al,[bp-0xa]
00000853  8A56FF            mov dl,[bp-0x1]
00000856  02D0              add dl,al
00000858  8856FF            mov [bp-0x1],dl
0000085B  8A46FF            mov al,[bp-0x1]
0000085E  3A46FE            cmp al,[bp-0x2]
00000861  7E02              jng 0x865
00000863  EB0B              jmp short 0x870
00000865  8A46FF            mov al,[bp-0x1]
00000868  8846FE            mov [bp-0x2],al
0000086B  8AC1              mov al,cl
0000086D  8846FD            mov [bp-0x3],al
00000870  41                inc cx
00000871  83F929            cmp cx,byte +0x29
00000874  7F03              jg 0x879
00000876  E963FF            jmp 0x7dc
00000879  8A46FD            mov al,[bp-0x3]
0000087C  98                cbw
0000087D  BB0600            mov bx,0x6
00000880  99                cwd
00000881  F7FB              idiv bx
00000883  8856FC            mov [bp-0x4],dl
00000886  8A46FD            mov al,[bp-0x3]
00000889  98                cbw
0000088A  BB0600            mov bx,0x6
0000088D  99                cwd
0000088E  F7FB              idiv bx
00000890  8846FB            mov [bp-0x5],al
00000893  8A46FC            mov al,[bp-0x4]
00000896  98                cbw
00000897  3B46F8            cmp ax,[bp-0x8]
0000089A  7D0D              jnl 0x8a9
0000089C  A0AA37            mov al,[0x37aa]
0000089F  98                cbw
000008A0  8BD8              mov bx,ax
000008A2  C687083301        mov byte [bx+0x3308],0x1
000008A7  EB37              jmp short 0x8e0
000008A9  8A46FC            mov al,[bp-0x4]
000008AC  98                cbw
000008AD  3B46F8            cmp ax,[bp-0x8]
000008B0  7E0D              jng 0x8bf
000008B2  A0AA37            mov al,[0x37aa]
000008B5  98                cbw
000008B6  8BD8              mov bx,ax
000008B8  C687083300        mov byte [bx+0x3308],0x0
000008BD  EB21              jmp short 0x8e0
000008BF  8A46FB            mov al,[bp-0x5]
000008C2  98                cbw
000008C3  3B46F6            cmp ax,[bp-0xa]
000008C6  7D0D              jnl 0x8d5
000008C8  A0AA37            mov al,[0x37aa]
000008CB  98                cbw
000008CC  8BD8              mov bx,ax
000008CE  C687083302        mov byte [bx+0x3308],0x2
000008D3  EB0B              jmp short 0x8e0
000008D5  A0AA37            mov al,[0x37aa]
000008D8  98                cbw
000008D9  8BD8              mov bx,ax
000008DB  C687083303        mov byte [bx+0x3308],0x3
000008E0  8A46FD            mov al,[bp-0x3]
000008E3  98                cbw
000008E4  C41EA254          les bx,[0x54a2]
000008E8  03D8              add bx,ax
000008EA  A0AA37            mov al,[0x37aa]
000008ED  268807            mov [es:bx],al
