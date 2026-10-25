; segment 141D  file offset 0x177D0  size 0xF20
00000000  45                inc bp
00000001  027EC5            add bh,[bp-0x3b]
00000004  8BE5              mov sp,bp
00000006  5D                pop bp
00000007  CB                retf

; ---- 141D:0008 ----
00000008  55                push bp
00000009  8BEC              mov bp,sp
0000000B  C606AA3700        mov byte [0x37aa],0x0
00000010  EB0E              jmp short 0x20
00000012  9A29001D14        call 0x141d:0x29
00000017  9A8C001D14        call 0x141d:0x8c
0000001C  FE06AA37          inc byte [0x37aa]
00000020  803EAA3704        cmp byte [0x37aa],0x4
00000025  7CEB              jl 0x12
00000027  5D                pop bp
00000028  CB                retf

; ---- 141D:0029 ----
00000029  55                push bp
0000002A  8BEC              mov bp,sp
0000002C  56                push si
0000002D  33C0              xor ax,ax
0000002F  BA5300            mov dx,0x53
00000032  50                push ax
00000033  52                push dx
00000034  A0AA37            mov al,[0x37aa]
00000037  98                cbw
00000038  D1E0              shl ax,1
0000003A  D1E0              shl ax,1
0000003C  33D2              xor dx,dx
0000003E  BB1027            mov bx,0x2710
00000041  87D8              xchg bx,ax
00000043  2B87D23C          sub ax,[bx+0x3cd2]
00000047  1B97D43C          sbb dx,[bx+0x3cd4]
0000004B  52                push dx
0000004C  50                push ax
0000004D  9A894F0000        call 0x0:0x4f89
00000052  BA9600            mov dx,0x96
00000055  2BD0              sub dx,ax
00000057  8BF2              mov si,dx
00000059  8BC6              mov ax,si
0000005B  99                cwd
0000005C  52                push dx
0000005D  50                push ax
0000005E  A0AA37            mov al,[0x37aa]
00000061  98                cbw
00000062  D1E0              shl ax,1
00000064  D1E0              shl ax,1
00000066  8BD8              mov bx,ax
00000068  FFB7D43C          push word [bx+0x3cd4]
0000006C  FFB7D23C          push word [bx+0x3cd2]
00000070  9A894F0000        call 0x0:0x4f89
00000075  50                push ax
00000076  A0AA37            mov al,[0x37aa]
00000079  98                cbw
0000007A  D1E0              shl ax,1
0000007C  D1E0              shl ax,1
0000007E  8BD8              mov bx,ax
00000080  58                pop ax
00000081  89873E2D          mov [bx+0x2d3e],ax
00000085  8997402D          mov [bx+0x2d40],dx
00000089  5E                pop si
0000008A  5D                pop bp
0000008B  CB                retf

; ---- 141D:008C ----
0000008C  55                push bp
0000008D  8BEC              mov bp,sp
0000008F  83EC1C            sub sp,byte +0x1c
00000092  C746F40000        mov word [bp-0xc],0x0
00000097  C746F60000        mov word [bp-0xa],0x0
0000009C  C706D0450000      mov word [0x45d0],0x0
000000A2  E99600            jmp 0x13b
000000A5  9A1A021D14        call 0x141d:0x21a
000000AA  A0AA37            mov al,[0x37aa]
000000AD  98                cbw
000000AE  D1E0              shl ax,1
000000B0  8BD8              mov bx,ax
000000B2  8B87782D          mov ax,[bx+0x2d78]
000000B6  BB0A00            mov bx,0xa
000000B9  99                cwd
000000BA  F7FB              idiv bx
000000BC  99                cwd
000000BD  50                push ax
000000BE  A0AA37            mov al,[0x37aa]
000000C1  98                cbw
000000C2  D1E0              shl ax,1
000000C4  D1E0              shl ax,1
000000C6  8BD8              mov bx,ax
000000C8  52                push dx
000000C9  8B97402D          mov dx,[bx+0x2d40]
000000CD  8B873E2D          mov ax,[bx+0x2d3e]
000000D1  59                pop cx
000000D2  5B                pop bx
000000D3  9A4E4F0000        call 0x0:0x4f4e
000000D8  8B1ED045          mov bx,[0x45d0]
000000DC  D1E3              shl bx,1
000000DE  D1E3              shl bx,1
000000E0  8D4EE4            lea cx,[bp-0x1c]
000000E3  03D9              add bx,cx
000000E5  368907            mov [ss:bx],ax
000000E8  36895702          mov [ss:bx+0x2],dx
000000EC  A0AA37            mov al,[0x37aa]
000000EF  98                cbw
000000F0  D1E0              shl ax,1
000000F2  8BD8              mov bx,ax
000000F4  8B87642D          mov ax,[bx+0x2d64]
000000F8  BB1E00            mov bx,0x1e
000000FB  99                cwd
000000FC  F7FB              idiv bx
000000FE  8B1ED045          mov bx,[0x45d0]
00000102  D1E3              shl bx,1
00000104  F7AF9848          imul word [bx+0x4898]
00000108  99                cwd
00000109  8B1ED045          mov bx,[0x45d0]
0000010D  D1E3              shl bx,1
0000010F  D1E3              shl bx,1
00000111  8D4EE4            lea cx,[bp-0x1c]
00000114  03D9              add bx,cx
00000116  360107            add [ss:bx],ax
00000119  36115702          adc [ss:bx+0x2],dx
0000011D  8B1ED045          mov bx,[0x45d0]
00000121  D1E3              shl bx,1
00000123  D1E3              shl bx,1
00000125  8D46E4            lea ax,[bp-0x1c]
00000128  03D8              add bx,ax
0000012A  368B4702          mov ax,[ss:bx+0x2]
0000012E  368B17            mov dx,[ss:bx]
00000131  0156F4            add [bp-0xc],dx
00000134  1146F6            adc [bp-0xa],ax
00000137  FF06D045          inc word [0x45d0]
0000013B  833ED04503        cmp word [0x45d0],byte +0x3
00000140  7F03              jg 0x145
00000142  E960FF            jmp 0xa5
00000145  C706D0450000      mov word [0x45d0],0x0
0000014B  E9BE00            jmp 0x20c
0000014E  8B46F4            mov ax,[bp-0xc]
00000151  0B46F6            or ax,[bp-0xa]
00000154  7523              jnz 0x179
00000156  A0AA37            mov al,[0x37aa]
00000159  98                cbw
0000015A  B104              mov cl,0x4
0000015C  D3E0              shl ax,cl
0000015E  8B16D045          mov dx,[0x45d0]
00000162  D1E2              shl dx,1
00000164  D1E2              shl dx,1
00000166  03C2              add ax,dx
00000168  8BD8              mov bx,ax
0000016A  C787882D0000      mov word [bx+0x2d88],0x0
00000170  C7878A2D0000      mov word [bx+0x2d8a],0x0
00000176  E98F00            jmp 0x208
00000179  FF76F6            push word [bp-0xa]
0000017C  FF76F4            push word [bp-0xc]
0000017F  8B1ED045          mov bx,[0x45d0]
00000183  D1E3              shl bx,1
00000185  D1E3              shl bx,1
00000187  8D46E4            lea ax,[bp-0x1c]
0000018A  03D8              add bx,ax
0000018C  368B4F02          mov cx,[ss:bx+0x2]
00000190  368B1F            mov bx,[ss:bx]
00000193  33D2              xor dx,dx
00000195  B86400            mov ax,0x64
00000198  9A4E4F0000        call 0x0:0x4f4e
0000019D  52                push dx
0000019E  50                push ax
0000019F  9A894F0000        call 0x0:0x4f89
000001A4  8B1ED045          mov bx,[0x45d0]
000001A8  D1E3              shl bx,1
000001AA  8D56F8            lea dx,[bp-0x8]
000001AD  03DA              add bx,dx
000001AF  368907            mov [ss:bx],ax
000001B2  33C0              xor ax,ax
000001B4  BA6400            mov dx,0x64
000001B7  50                push ax
000001B8  52                push dx
000001B9  8B1ED045          mov bx,[0x45d0]
000001BD  D1E3              shl bx,1
000001BF  8D46F8            lea ax,[bp-0x8]
000001C2  03D8              add bx,ax
000001C4  368B07            mov ax,[ss:bx]
000001C7  99                cwd
000001C8  50                push ax
000001C9  A0AA37            mov al,[0x37aa]
000001CC  98                cbw
000001CD  D1E0              shl ax,1
000001CF  D1E0              shl ax,1
000001D1  8BD8              mov bx,ax
000001D3  52                push dx
000001D4  8B97D43C          mov dx,[bx+0x3cd4]
000001D8  8B87D23C          mov ax,[bx+0x3cd2]
000001DC  59                pop cx
000001DD  5B                pop bx
000001DE  9A4E4F0000        call 0x0:0x4f4e
000001E3  52                push dx
000001E4  50                push ax
000001E5  9A894F0000        call 0x0:0x4f89
000001EA  50                push ax
000001EB  A0AA37            mov al,[0x37aa]
000001EE  98                cbw
000001EF  B104              mov cl,0x4
000001F1  D3E0              shl ax,cl
000001F3  8B1ED045          mov bx,[0x45d0]
000001F7  D1E3              shl bx,1
000001F9  D1E3              shl bx,1
000001FB  03C3              add ax,bx
000001FD  8BD8              mov bx,ax
000001FF  58                pop ax
00000200  8987882D          mov [bx+0x2d88],ax
00000204  89978A2D          mov [bx+0x2d8a],dx
00000208  FF06D045          inc word [0x45d0]
0000020C  833ED04503        cmp word [0x45d0],byte +0x3
00000211  7F03              jg 0x216
00000213  E938FF            jmp 0x14e
00000216  8BE5              mov sp,bp
00000218  5D                pop bp
00000219  CB                retf

; ---- 141D:021A ----
0000021A  55                push bp
0000021B  8BEC              mov bp,sp
0000021D  83EC04            sub sp,byte +0x4
00000220  56                push si
00000221  A0AA37            mov al,[0x37aa]
00000224  98                cbw
00000225  8BD8              mov bx,ax
00000227  80BF400801        cmp byte [bx+0x840],0x1
0000022C  7569              jnz 0x297
0000022E  A0AA37            mov al,[0x37aa]
00000231  98                cbw
00000232  D1E0              shl ax,1
00000234  D1E0              shl ax,1
00000236  D1E0              shl ax,1
00000238  8B16D045          mov dx,[0x45d0]
0000023C  D1E2              shl dx,1
0000023E  03C2              add ax,dx
00000240  8BD8              mov bx,ax
00000242  8B876045          mov ax,[bx+0x4560]
00000246  50                push ax
00000247  A0AA37            mov al,[0x37aa]
0000024A  98                cbw
0000024B  D1E0              shl ax,1
0000024D  D1E0              shl ax,1
0000024F  D1E0              shl ax,1
00000251  8B16D045          mov dx,[0x45d0]
00000255  D1E2              shl dx,1
00000257  03C2              add ax,dx
00000259  8BD8              mov bx,ax
0000025B  58                pop ax
0000025C  2B87F43C          sub ax,[bx+0x3cf4]
00000260  50                push ax
00000261  A0AA37            mov al,[0x37aa]
00000264  98                cbw
00000265  D1E0              shl ax,1
00000267  8BD8              mov bx,ax
00000269  58                pop ax
0000026A  8987782D          mov [bx+0x2d78],ax
0000026E  A0AA37            mov al,[0x37aa]
00000271  98                cbw
00000272  D1E0              shl ax,1
00000274  D1E0              shl ax,1
00000276  D1E0              shl ax,1
00000278  8B16D045          mov dx,[0x45d0]
0000027C  D1E2              shl dx,1
0000027E  03C2              add ax,dx
00000280  8BD8              mov bx,ax
00000282  8B872A45          mov ax,[bx+0x452a]
00000286  50                push ax
00000287  A0AA37            mov al,[0x37aa]
0000028A  98                cbw
0000028B  D1E0              shl ax,1
0000028D  8BD8              mov bx,ax
0000028F  58                pop ax
00000290  8987642D          mov [bx+0x2d64],ax
00000294  E98700            jmp 0x31e
00000297  833ED04503        cmp word [0x45d0],byte +0x3
0000029C  751E              jnz 0x2bc
0000029E  A1FA3C            mov ax,[0x3cfa]
000002A1  0306023D          add ax,[0x3d02]
000002A5  03060A3D          add ax,[0x3d0a]
000002A9  0306123D          add ax,[0x3d12]
000002AD  BB0400            mov bx,0x4
000002B0  99                cwd
000002B1  F7FB              idiv bx
000002B3  99                cwd
000002B4  8946FC            mov [bp-0x4],ax
000002B7  8956FE            mov [bp-0x2],dx
000002BA  EB2D              jmp short 0x2e9
000002BC  A1D045            mov ax,[0x45d0]
000002BF  40                inc ax
000002C0  8BF0              mov si,ax
000002C2  3D0300            cmp ax,0x3
000002C5  7502              jnz 0x2c9
000002C7  33F6              xor si,si
000002C9  A0AA37            mov al,[0x37aa]
000002CC  98                cbw
000002CD  D1E0              shl ax,1
000002CF  D1E0              shl ax,1
000002D1  8BD8              mov bx,ax
000002D3  8A80E72E          mov al,[bx+si+0x2ee7]
000002D7  98                cbw
000002D8  50                push ax
000002D9  A0262F            mov al,[0x2f26]
000002DC  98                cbw
000002DD  8BD0              mov dx,ax
000002DF  58                pop ax
000002E0  F7EA              imul dx
000002E2  99                cwd
000002E3  8946FC            mov [bp-0x4],ax
000002E6  8956FE            mov [bp-0x2],dx
000002E9  A0AA37            mov al,[0x37aa]
000002EC  98                cbw
000002ED  D1E0              shl ax,1
000002EF  D1E0              shl ax,1
000002F1  D1E0              shl ax,1
000002F3  8B16D045          mov dx,[0x45d0]
000002F7  D1E2              shl dx,1
000002F9  03C2              add ax,dx
000002FB  8B56FC            mov dx,[bp-0x4]
000002FE  8BD8              mov bx,ax
00000300  2B97F43C          sub dx,[bx+0x3cf4]
00000304  A0AA37            mov al,[0x37aa]
00000307  98                cbw
00000308  D1E0              shl ax,1
0000030A  8BD8              mov bx,ax
0000030C  8997782D          mov [bx+0x2d78],dx
00000310  A0AA37            mov al,[0x37aa]
00000313  98                cbw
00000314  D1E0              shl ax,1
00000316  8BD8              mov bx,ax
00000318  C787642D1E00      mov word [bx+0x2d64],0x1e
0000031E  A0AA37            mov al,[0x37aa]
00000321  98                cbw
00000322  D1E0              shl ax,1
00000324  8BD8              mov bx,ax
00000326  8B87782D          mov ax,[bx+0x2d78]
0000032A  50                push ax
0000032B  A0AA37            mov al,[0x37aa]
0000032E  98                cbw
0000032F  D1E0              shl ax,1
00000331  8BD8              mov bx,ax
00000333  58                pop ax
00000334  0387642D          add ax,[bx+0x2d64]
00000338  F7D8              neg ax
0000033A  50                push ax
0000033B  A0AA37            mov al,[0x37aa]
0000033E  98                cbw
0000033F  D1E0              shl ax,1
00000341  8BD8              mov bx,ax
00000343  58                pop ax
00000344  8987802D          mov [bx+0x2d80],ax
00000348  A0AA37            mov al,[0x37aa]
0000034B  98                cbw
0000034C  D1E0              shl ax,1
0000034E  8BD8              mov bx,ax
00000350  83BF782D00        cmp word [bx+0x2d78],byte +0x0
00000355  7D0E              jnl 0x365
00000357  A0AA37            mov al,[0x37aa]
0000035A  98                cbw
0000035B  D1E0              shl ax,1
0000035D  8BD8              mov bx,ax
0000035F  C787782D0000      mov word [bx+0x2d78],0x0
00000365  A0AA37            mov al,[0x37aa]
00000368  98                cbw
00000369  D1E0              shl ax,1
0000036B  8BD8              mov bx,ax
0000036D  83BF802D00        cmp word [bx+0x2d80],byte +0x0
00000372  7D10              jnl 0x384
00000374  A0AA37            mov al,[0x37aa]
00000377  98                cbw
00000378  D1E0              shl ax,1
0000037A  8BD8              mov bx,ax
0000037C  C787802D0000      mov word [bx+0x2d80],0x0
00000382  EB0E              jmp short 0x392
00000384  A0AA37            mov al,[0x37aa]
00000387  98                cbw
00000388  D1E0              shl ax,1
0000038A  8BD8              mov bx,ax
0000038C  C787642D0000      mov word [bx+0x2d64],0x0
00000392  5E                pop si
00000393  8BE5              mov sp,bp
00000395  5D                pop bp
00000396  CB                retf

; ---- 141D:0397 ----
00000397  55                push bp
00000398  8BEC              mov bp,sp
0000039A  4C                dec sp
0000039B  4C                dec sp
0000039C  A1182F            mov ax,[0x2f18]
0000039F  3B06162F          cmp ax,[0x2f16]
000003A3  750D              jnz 0x3b2
000003A5  A18037            mov ax,[0x3780]
000003A8  BB0400            mov bx,0x4
000003AB  99                cwd
000003AC  F7FB              idiv bx
000003AE  0BD2              or dx,dx
000003B0  7408              jz 0x3ba
000003B2  C746FEFFFF        mov word [bp-0x2],0xffff
000003B7  E99900            jmp 0x453
000003BA  833E803704        cmp word [0x3780],byte +0x4
000003BF  7E28              jng 0x3e9
000003C1  B80300            mov ax,0x3
000003C4  50                push ax
000003C5  9ABA01FD21        call 0x21fd:0x1ba
000003CA  59                pop cx
000003CB  0BC0              or ax,ax
000003CD  7505              jnz 0x3d4
000003CF  C746FE0300        mov word [bp-0x2],0x3
000003D4  B80300            mov ax,0x3
000003D7  50                push ax
000003D8  9ABA01FD21        call 0x21fd:0x1ba
000003DD  59                pop cx
000003DE  0BC0              or ax,ax
000003E0  7505              jnz 0x3e7
000003E2  C746FE0200        mov word [bp-0x2],0x2
000003E7  EB6A              jmp short 0x453
000003E9  A0AA37            mov al,[0x37aa]
000003EC  98                cbw
000003ED  D1E0              shl ax,1
000003EF  8BD8              mov bx,ax
000003F1  83BF802D0A        cmp word [bx+0x2d80],byte +0xa
000003F6  7C07              jl 0x3ff
000003F8  C746FE0200        mov word [bp-0x2],0x2
000003FD  EB05              jmp short 0x404
000003FF  C746FE0300        mov word [bp-0x2],0x3
00000404  833ED04503        cmp word [0x45d0],byte +0x3
00000409  7548              jnz 0x453
0000040B  A0AA37            mov al,[0x37aa]
0000040E  98                cbw
0000040F  8BD8              mov bx,ax
00000411  80BFBE4B01        cmp byte [bx+0x4bbe],0x1
00000416  750C              jnz 0x424
00000418  803E052F08        cmp byte [0x2f05],0x8
0000041D  7C05              jl 0x424
0000041F  C746FE0300        mov word [bp-0x2],0x3
00000424  A0AA37            mov al,[0x37aa]
00000427  3A068045          cmp al,[0x4580]
0000042B  7505              jnz 0x432
0000042D  C746FE0300        mov word [bp-0x2],0x3
00000432  A0AA37            mov al,[0x37aa]
00000435  98                cbw
00000436  8BD8              mov bx,ax
00000438  80BF423D01        cmp byte [bx+0x3d42],0x1
0000043D  760F              jna 0x44e
0000043F  A0AA37            mov al,[0x37aa]
00000442  98                cbw
00000443  D1E0              shl ax,1
00000445  8BD8              mov bx,ax
00000447  83BF742C00        cmp word [bx+0x2c74],byte +0x0
0000044C  7505              jnz 0x453
0000044E  C746FE0300        mov word [bp-0x2],0x3
00000453  A0AA37            mov al,[0x37aa]
00000456  98                cbw
00000457  8A56FE            mov dl,[bp-0x2]
0000045A  8BD8              mov bx,ax
0000045C  88970833          mov [bx+0x3308],dl
00000460  8BE5              mov sp,bp
00000462  5D                pop bp
00000463  CB                retf

; ---- 141D:0464 ----
00000464  55                push bp
00000465  8BEC              mov bp,sp
00000467  C606AA3700        mov byte [0x37aa],0x0
0000046C  EB1F              jmp short 0x48d
0000046E  0E                push cs
0000046F  E8A8FD            call 0x21a
00000472  A0AA37            mov al,[0x37aa]
00000475  98                cbw
00000476  8BD8              mov bx,ax
00000478  80BFB24300        cmp byte [bx+0x43b2],0x0
0000047D  750A              jnz 0x489
0000047F  9AA5041D14        call 0x141d:0x4a5
00000484  9AA3051D14        call 0x141d:0x5a3
00000489  FE06AA37          inc byte [0x37aa]
0000048D  803EAA3704        cmp byte [0x37aa],0x4
00000492  7CDA              jl 0x46e
00000494  9A090E1D14        call 0x141d:0xe09
00000499  9A06081D14        call 0x141d:0x806
0000049E  9A010F1D14        call 0x141d:0xf01
000004A3  5D                pop bp
000004A4  CB                retf

; ---- 141D:04A5 ----
000004A5  55                push bp
000004A6  8BEC              mov bp,sp
000004A8  4C                dec sp
000004A9  4C                dec sp
000004AA  56                push si
000004AB  57                push di
000004AC  A0AA37            mov al,[0x37aa]
000004AF  98                cbw
000004B0  D1E0              shl ax,1
000004B2  D1E0              shl ax,1
000004B4  8BD8              mov bx,ax
000004B6  8B873E2D          mov ax,[bx+0x2d3e]
000004BA  8946FE            mov [bp-0x2],ax
000004BD  8B46FE            mov ax,[bp-0x2]
000004C0  BB0A00            mov bx,0xa
000004C3  99                cwd
000004C4  F7FB              idiv bx
000004C6  8BF0              mov si,ax
000004C8  83FE02            cmp si,byte +0x2
000004CB  7D03              jnl 0x4d0
000004CD  BE0200            mov si,0x2
000004D0  83FE14            cmp si,byte +0x14
000004D3  7E03              jng 0x4d8
000004D5  BE1400            mov si,0x14
000004D8  8B46FE            mov ax,[bp-0x2]
000004DB  03C6              add ax,si
000004DD  99                cwd
000004DE  50                push ax
000004DF  A0AA37            mov al,[0x37aa]
000004E2  98                cbw
000004E3  B104              mov cl,0x4
000004E5  D3E0              shl ax,cl
000004E7  8BD8              mov bx,ax
000004E9  58                pop ax
000004EA  89877E2C          mov [bx+0x2c7e],ax
000004EE  8997802C          mov [bx+0x2c80],dx
000004F2  8B46FE            mov ax,[bp-0x2]
000004F5  99                cwd
000004F6  50                push ax
000004F7  A0AA37            mov al,[0x37aa]
000004FA  98                cbw
000004FB  B104              mov cl,0x4
000004FD  D3E0              shl ax,cl
000004FF  8BD8              mov bx,ax
00000501  58                pop ax
00000502  8987822C          mov [bx+0x2c82],ax
00000506  8997842C          mov [bx+0x2c84],dx
0000050A  8B46FE            mov ax,[bp-0x2]
0000050D  2BC6              sub ax,si
0000050F  99                cwd
00000510  50                push ax
00000511  A0AA37            mov al,[0x37aa]
00000514  98                cbw
00000515  B104              mov cl,0x4
00000517  D3E0              shl ax,cl
00000519  8BD8              mov bx,ax
0000051B  58                pop ax
0000051C  8987862C          mov [bx+0x2c86],ax
00000520  8997882C          mov [bx+0x2c88],dx
00000524  8B1ED045          mov bx,[0x45d0]
00000528  D1E3              shl bx,1
0000052A  8B879848          mov ax,[bx+0x4898]
0000052E  99                cwd
0000052F  50                push ax
00000530  A0AA37            mov al,[0x37aa]
00000533  98                cbw
00000534  B104              mov cl,0x4
00000536  D3E0              shl ax,cl
00000538  8BD8              mov bx,ax
0000053A  58                pop ax
0000053B  89878A2C          mov [bx+0x2c8a],ax
0000053F  89978C2C          mov [bx+0x2c8c],dx
00000543  33FF              xor di,di
00000545  EB51              jmp short 0x598
00000547  A0AA37            mov al,[0x37aa]
0000054A  98                cbw
0000054B  B104              mov cl,0x4
0000054D  D3E0              shl ax,cl
0000054F  8BD7              mov dx,di
00000551  D1E2              shl dx,1
00000553  D1E2              shl dx,1
00000555  03C2              add ax,dx
00000557  8B1ED045          mov bx,[0x45d0]
0000055B  D1E3              shl bx,1
0000055D  50                push ax
0000055E  8B879848          mov ax,[bx+0x4898]
00000562  99                cwd
00000563  5B                pop bx
00000564  3997802C          cmp [bx+0x2c80],dx
00000568  7F2D              jg 0x597
0000056A  7C06              jl 0x572
0000056C  39877E2C          cmp [bx+0x2c7e],ax
00000570  7325              jnc 0x597
00000572  A0AA37            mov al,[0x37aa]
00000575  98                cbw
00000576  B104              mov cl,0x4
00000578  D3E0              shl ax,cl
0000057A  8BD7              mov dx,di
0000057C  D1E2              shl dx,1
0000057E  D1E2              shl dx,1
00000580  03C2              add ax,dx
00000582  8B1ED045          mov bx,[0x45d0]
00000586  D1E3              shl bx,1
00000588  50                push ax
00000589  8B879848          mov ax,[bx+0x4898]
0000058D  99                cwd
0000058E  5B                pop bx
0000058F  89877E2C          mov [bx+0x2c7e],ax
00000593  8997802C          mov [bx+0x2c80],dx
00000597  47                inc di
00000598  83FF02            cmp di,byte +0x2
0000059B  7EAA              jng 0x547
0000059D  5F                pop di
0000059E  5E                pop si
0000059F  8BE5              mov sp,bp
000005A1  5D                pop bp
000005A2  CB                retf

; ---- 141D:05A3 ----
000005A3  55                push bp
000005A4  8BEC              mov bp,sp
000005A6  83EC0E            sub sp,byte +0xe
000005A9  56                push si
000005AA  57                push di
000005AB  A0AA37            mov al,[0x37aa]
000005AE  98                cbw
000005AF  B104              mov cl,0x4
000005B1  D3E0              shl ax,cl
000005B3  8B16D045          mov dx,[0x45d0]
000005B7  D1E2              shl dx,1
000005B9  D1E2              shl dx,1
000005BB  03C2              add ax,dx
000005BD  8BD8              mov bx,ax
000005BF  8B878A2D          mov ax,[bx+0x2d8a]
000005C3  8B97882D          mov dx,[bx+0x2d88]
000005C7  8956FC            mov [bp-0x4],dx
000005CA  8946FE            mov [bp-0x2],ax
000005CD  33F6              xor si,si
000005CF  EB19              jmp short 0x5ea
000005D1  A0AA37            mov al,[0x37aa]
000005D4  98                cbw
000005D5  D1E0              shl ax,1
000005D7  D1E0              shl ax,1
000005D9  D1E0              shl ax,1
000005DB  8BD6              mov dx,si
000005DD  D1E2              shl dx,1
000005DF  03C2              add ax,dx
000005E1  8BD8              mov bx,ax
000005E3  C787FE2C0000      mov word [bx+0x2cfe],0x0
000005E9  46                inc si
000005EA  83FE03            cmp si,byte +0x3
000005ED  7EE2              jng 0x5d1
000005EF  A0AA37            mov al,[0x37aa]
000005F2  98                cbw
000005F3  D1E0              shl ax,1
000005F5  8BD8              mov bx,ax
000005F7  8B87782D          mov ax,[bx+0x2d78]
000005FB  8946F8            mov [bp-0x8],ax
000005FE  A0AA37            mov al,[0x37aa]
00000601  98                cbw
00000602  D1E0              shl ax,1
00000604  8BD8              mov bx,ax
00000606  8B87642D          mov ax,[bx+0x2d64]
0000060A  8946FA            mov [bp-0x6],ax
0000060D  33F6              xor si,si
0000060F  EB0F              jmp short 0x620
00000611  8BDE              mov bx,si
00000613  D1E3              shl bx,1
00000615  8D46F2            lea ax,[bp-0xe]
00000618  03D8              add bx,ax
0000061A  36C7070000        mov word [ss:bx],0x0
0000061F  46                inc si
00000620  83FE02            cmp si,byte +0x2
00000623  7EEC              jng 0x611
00000625  E9A700            jmp 0x6cf
00000628  837EF800          cmp word [bp-0x8],byte +0x0
0000062C  7F03              jg 0x631
0000062E  E9B900            jmp 0x6ea
00000631  33F6              xor si,si
00000633  E99100            jmp 0x6c7
00000636  A0AA37            mov al,[0x37aa]
00000639  98                cbw
0000063A  B104              mov cl,0x4
0000063C  D3E0              shl ax,cl
0000063E  8BD6              mov dx,si
00000640  D1E2              shl dx,1
00000642  D1E2              shl dx,1
00000644  03C2              add ax,dx
00000646  8BD8              mov bx,ax
00000648  8B87802C          mov ax,[bx+0x2c80]
0000064C  8B977E2C          mov dx,[bx+0x2c7e]
00000650  3B46FE            cmp ax,[bp-0x2]
00000653  7C17              jl 0x66c
00000655  7505              jnz 0x65c
00000657  3B56FC            cmp dx,[bp-0x4]
0000065A  7210              jc 0x66c
0000065C  8BDE              mov bx,si
0000065E  D1E3              shl bx,1
00000660  8D46F2            lea ax,[bp-0xe]
00000663  03D8              add bx,ax
00000665  36C7070100        mov word [ss:bx],0x1
0000066A  EB5A              jmp short 0x6c6
0000066C  A0AA37            mov al,[0x37aa]
0000066F  98                cbw
00000670  D1E0              shl ax,1
00000672  D1E0              shl ax,1
00000674  D1E0              shl ax,1
00000676  8BD6              mov dx,si
00000678  D1E2              shl dx,1
0000067A  03C2              add ax,dx
0000067C  8BD8              mov bx,ax
0000067E  8B87FE2C          mov ax,[bx+0x2cfe]
00000682  050A00            add ax,0xa
00000685  50                push ax
00000686  A0AA37            mov al,[0x37aa]
00000689  98                cbw
0000068A  D1E0              shl ax,1
0000068C  D1E0              shl ax,1
0000068E  D1E0              shl ax,1
00000690  8BD6              mov dx,si
00000692  D1E2              shl dx,1
00000694  03C2              add ax,dx
00000696  8BD8              mov bx,ax
00000698  58                pop ax
00000699  8987FE2C          mov [bx+0x2cfe],ax
0000069D  8B46F8            mov ax,[bp-0x8]
000006A0  05F6FF            add ax,0xfff6
000006A3  8946F8            mov [bp-0x8],ax
000006A6  A0AA37            mov al,[0x37aa]
000006A9  98                cbw
000006AA  B104              mov cl,0x4
000006AC  D3E0              shl ax,cl
000006AE  8BD6              mov dx,si
000006B0  D1E2              shl dx,1
000006B2  D1E2              shl dx,1
000006B4  03C2              add ax,dx
000006B6  8BD8              mov bx,ax
000006B8  8B87802C          mov ax,[bx+0x2c80]
000006BC  8B977E2C          mov dx,[bx+0x2c7e]
000006C0  2956FC            sub [bp-0x4],dx
000006C3  1946FE            sbb [bp-0x2],ax
000006C6  46                inc si
000006C7  83FE02            cmp si,byte +0x2
000006CA  7F03              jg 0x6cf
000006CC  E967FF            jmp 0x636
000006CF  837EF200          cmp word [bp-0xe],byte +0x0
000006D3  7503              jnz 0x6d8
000006D5  E950FF            jmp 0x628
000006D8  837EF400          cmp word [bp-0xc],byte +0x0
000006DC  7503              jnz 0x6e1
000006DE  E947FF            jmp 0x628
000006E1  837EF600          cmp word [bp-0xa],byte +0x0
000006E5  7503              jnz 0x6ea
000006E7  E93EFF            jmp 0x628
000006EA  837EF800          cmp word [bp-0x8],byte +0x0
000006EE  7E06              jng 0x6f6
000006F0  8B46F8            mov ax,[bp-0x8]
000006F3  0146FA            add [bp-0x6],ax
000006F6  EB4E              jmp short 0x746
000006F8  837EFA00          cmp word [bp-0x6],byte +0x0
000006FC  7F02              jg 0x700
000006FE  EB64              jmp short 0x764
00000700  A0AA37            mov al,[0x37aa]
00000703  98                cbw
00000704  D1E0              shl ax,1
00000706  D1E0              shl ax,1
00000708  D1E0              shl ax,1
0000070A  8BD8              mov bx,ax
0000070C  8B87042D          mov ax,[bx+0x2d04]
00000710  050A00            add ax,0xa
00000713  50                push ax
00000714  A0AA37            mov al,[0x37aa]
00000717  98                cbw
00000718  D1E0              shl ax,1
0000071A  D1E0              shl ax,1
0000071C  D1E0              shl ax,1
0000071E  8BD8              mov bx,ax
00000720  58                pop ax
00000721  8987042D          mov [bx+0x2d04],ax
00000725  8B46FA            mov ax,[bp-0x6]
00000728  05F6FF            add ax,0xfff6
0000072B  8946FA            mov [bp-0x6],ax
0000072E  A0AA37            mov al,[0x37aa]
00000731  98                cbw
00000732  B104              mov cl,0x4
00000734  D3E0              shl ax,cl
00000736  8BD8              mov bx,ax
00000738  8B878C2C          mov ax,[bx+0x2c8c]
0000073C  8B978A2C          mov dx,[bx+0x2c8a]
00000740  2956FC            sub [bp-0x4],dx
00000743  1946FE            sbb [bp-0x2],ax
00000746  A0AA37            mov al,[0x37aa]
00000749  98                cbw
0000074A  B104              mov cl,0x4
0000074C  D3E0              shl ax,cl
0000074E  8BD8              mov bx,ax
00000750  8B878C2C          mov ax,[bx+0x2c8c]
00000754  8B978A2C          mov dx,[bx+0x2c8a]
00000758  3B46FE            cmp ax,[bp-0x2]
0000075B  7C9B              jl 0x6f8
0000075D  7F05              jg 0x764
0000075F  3B56FC            cmp dx,[bp-0x4]
00000762  7694              jna 0x6f8
00000764  33FF              xor di,di
00000766  EB1C              jmp short 0x784
00000768  A0AA37            mov al,[0x37aa]
0000076B  98                cbw
0000076C  D1E0              shl ax,1
0000076E  D1E0              shl ax,1
00000770  D1E0              shl ax,1
00000772  8BD7              mov dx,di
00000774  D1E2              shl dx,1
00000776  03C2              add ax,dx
00000778  8BD8              mov bx,ax
0000077A  83BFFE2C00        cmp word [bx+0x2cfe],byte +0x0
0000077F  7E02              jng 0x783
00000781  EB06              jmp short 0x789
00000783  47                inc di
00000784  83FF03            cmp di,byte +0x3
00000787  7EDF              jng 0x768
00000789  A0AA37            mov al,[0x37aa]
0000078C  98                cbw
0000078D  D1E0              shl ax,1
0000078F  8BD8              mov bx,ax
00000791  89BF6C2D          mov [bx+0x2d6c],di
00000795  83FF04            cmp di,byte +0x4
00000798  7538              jnz 0x7d2
0000079A  A0AA37            mov al,[0x37aa]
0000079D  98                cbw
0000079E  D1E0              shl ax,1
000007A0  D1E0              shl ax,1
000007A2  8BD8              mov bx,ax
000007A4  C787502D0000      mov word [bx+0x2d50],0x0
000007AA  C787522D0000      mov word [bx+0x2d52],0x0
000007B0  A0AA37            mov al,[0x37aa]
000007B3  98                cbw
000007B4  B104              mov cl,0x4
000007B6  D3E0              shl ax,cl
000007B8  8B16D045          mov dx,[0x45d0]
000007BC  D1E2              shl dx,1
000007BE  D1E2              shl dx,1
000007C0  03C2              add ax,dx
000007C2  8BD8              mov bx,ax
000007C4  C787882D0000      mov word [bx+0x2d88],0x0
000007CA  C7878A2D0000      mov word [bx+0x2d8a],0x0
000007D0  EB2E              jmp short 0x800
000007D2  A0AA37            mov al,[0x37aa]
000007D5  98                cbw
000007D6  B104              mov cl,0x4
000007D8  D3E0              shl ax,cl
000007DA  8BD7              mov dx,di
000007DC  D1E2              shl dx,1
000007DE  D1E2              shl dx,1
000007E0  03C2              add ax,dx
000007E2  8BD8              mov bx,ax
000007E4  8B87802C          mov ax,[bx+0x2c80]
000007E8  8B977E2C          mov dx,[bx+0x2c7e]
000007EC  50                push ax
000007ED  A0AA37            mov al,[0x37aa]
000007F0  98                cbw
000007F1  D1E0              shl ax,1
000007F3  D1E0              shl ax,1
000007F5  8BD8              mov bx,ax
000007F7  8997502D          mov [bx+0x2d50],dx
000007FB  58                pop ax
000007FC  8987522D          mov [bx+0x2d52],ax
00000800  5F                pop di
00000801  5E                pop si
00000802  8BE5              mov sp,bp
00000804  5D                pop bp
00000805  CB                retf

; ---- 141D:0806 ----
00000806  55                push bp
00000807  8BEC              mov bp,sp
00000809  83EC20            sub sp,byte +0x20
0000080C  56                push si
0000080D  57                push di
0000080E  33F6              xor si,si
00000810  EB21              jmp short 0x833
00000812  8BDE              mov bx,si
00000814  D1E3              shl bx,1
00000816  8D46E0            lea ax,[bp-0x20]
00000819  03D8              add bx,ax
0000081B  36C7070000        mov word [ss:bx],0x0
00000820  8BDE              mov bx,si
00000822  D1E3              shl bx,1
00000824  D1E3              shl bx,1
00000826  C787BE2C0000      mov word [bx+0x2cbe],0x0
0000082C  C787C02C0000      mov word [bx+0x2cc0],0x0
00000832  46                inc si
00000833  83FE0F            cmp si,byte +0xf
00000836  7EDA              jng 0x812
00000838  33F6              xor si,si
0000083A  E9CE00            jmp 0x90b
0000083D  8BC6              mov ax,si
0000083F  40                inc ax
00000840  8BF8              mov di,ax
00000842  E9BD00            jmp 0x902
00000845  8BC6              mov ax,si
00000847  BB0400            mov bx,0x4
0000084A  99                cwd
0000084B  F7FB              idiv bx
0000084D  B104              mov cl,0x4
0000084F  D3E0              shl ax,cl
00000851  50                push ax
00000852  8BC6              mov ax,si
00000854  BB0400            mov bx,0x4
00000857  99                cwd
00000858  F7FB              idiv bx
0000085A  D1E2              shl dx,1
0000085C  D1E2              shl dx,1
0000085E  5B                pop bx
0000085F  03DA              add bx,dx
00000861  8B87802C          mov ax,[bx+0x2c80]
00000865  8B977E2C          mov dx,[bx+0x2c7e]
00000869  50                push ax
0000086A  8BC7              mov ax,di
0000086C  BB0400            mov bx,0x4
0000086F  52                push dx
00000870  99                cwd
00000871  F7FB              idiv bx
00000873  B104              mov cl,0x4
00000875  D3E0              shl ax,cl
00000877  50                push ax
00000878  8BC7              mov ax,di
0000087A  BB0400            mov bx,0x4
0000087D  99                cwd
0000087E  F7FB              idiv bx
00000880  D1E2              shl dx,1
00000882  D1E2              shl dx,1
00000884  5B                pop bx
00000885  03DA              add bx,dx
00000887  58                pop ax
00000888  5A                pop dx
00000889  3B97802C          cmp dx,[bx+0x2c80]
0000088D  7C14              jl 0x8a3
0000088F  7F06              jg 0x897
00000891  3B877E2C          cmp ax,[bx+0x2c7e]
00000895  760C              jna 0x8a3
00000897  8BDF              mov bx,di
00000899  D1E3              shl bx,1
0000089B  8D46E0            lea ax,[bp-0x20]
0000089E  03D8              add bx,ax
000008A0  36FF07            inc word [ss:bx]
000008A3  8BC7              mov ax,di
000008A5  BB0400            mov bx,0x4
000008A8  99                cwd
000008A9  F7FB              idiv bx
000008AB  B104              mov cl,0x4
000008AD  D3E0              shl ax,cl
000008AF  50                push ax
000008B0  8BC7              mov ax,di
000008B2  BB0400            mov bx,0x4
000008B5  99                cwd
000008B6  F7FB              idiv bx
000008B8  D1E2              shl dx,1
000008BA  D1E2              shl dx,1
000008BC  5B                pop bx
000008BD  03DA              add bx,dx
000008BF  8B87802C          mov ax,[bx+0x2c80]
000008C3  8B977E2C          mov dx,[bx+0x2c7e]
000008C7  50                push ax
000008C8  8BC6              mov ax,si
000008CA  BB0400            mov bx,0x4
000008CD  52                push dx
000008CE  99                cwd
000008CF  F7FB              idiv bx
000008D1  B104              mov cl,0x4
000008D3  D3E0              shl ax,cl
000008D5  50                push ax
000008D6  8BC6              mov ax,si
000008D8  BB0400            mov bx,0x4
000008DB  99                cwd
000008DC  F7FB              idiv bx
000008DE  D1E2              shl dx,1
000008E0  D1E2              shl dx,1
000008E2  5B                pop bx
000008E3  03DA              add bx,dx
000008E5  58                pop ax
000008E6  5A                pop dx
000008E7  3B97802C          cmp dx,[bx+0x2c80]
000008EB  7C14              jl 0x901
000008ED  7506              jnz 0x8f5
000008EF  3B877E2C          cmp ax,[bx+0x2c7e]
000008F3  720C              jc 0x901
000008F5  8BDE              mov bx,si
000008F7  D1E3              shl bx,1
000008F9  8D46E0            lea ax,[bp-0x20]
000008FC  03D8              add bx,ax
000008FE  36FF07            inc word [ss:bx]
00000901  47                inc di
00000902  83FF0F            cmp di,byte +0xf
00000905  7F03              jg 0x90a
00000907  E93BFF            jmp 0x845
0000090A  46                inc si
0000090B  83FE0F            cmp si,byte +0xf
0000090E  7F03              jg 0x913
00000910  E92AFF            jmp 0x83d
00000913  33F6              xor si,si
00000915  E9AD00            jmp 0x9c5
00000918  33FF              xor di,di
0000091A  E99F00            jmp 0x9bc
0000091D  8BDF              mov bx,di
0000091F  D1E3              shl bx,1
00000921  8D46E0            lea ax,[bp-0x20]
00000924  03D8              add bx,ax
00000926  363937            cmp [ss:bx],si
00000929  7403              jz 0x92e
0000092B  E98D00            jmp 0x9bb
0000092E  8BC7              mov ax,di
00000930  BB0400            mov bx,0x4
00000933  99                cwd
00000934  F7FB              idiv bx
00000936  8BD8              mov bx,ax
00000938  80BFB24300        cmp byte [bx+0x43b2],0x0
0000093D  755E              jnz 0x99d
0000093F  8BC7              mov ax,di
00000941  BB0400            mov bx,0x4
00000944  99                cwd
00000945  F7FB              idiv bx
00000947  B104              mov cl,0x4
00000949  D3E0              shl ax,cl
0000094B  50                push ax
0000094C  8BC7              mov ax,di
0000094E  BB0400            mov bx,0x4
00000951  99                cwd
00000952  F7FB              idiv bx
00000954  D1E2              shl dx,1
00000956  D1E2              shl dx,1
00000958  5B                pop bx
00000959  03DA              add bx,dx
0000095B  8B87802C          mov ax,[bx+0x2c80]
0000095F  8B977E2C          mov dx,[bx+0x2c7e]
00000963  8BDE              mov bx,si
00000965  D1E3              shl bx,1
00000967  D1E3              shl bx,1
00000969  8997BE2C          mov [bx+0x2cbe],dx
0000096D  8987C02C          mov [bx+0x2cc0],ax
00000971  8BC7              mov ax,di
00000973  BB0400            mov bx,0x4
00000976  99                cwd
00000977  F7FB              idiv bx
00000979  D1E0              shl ax,1
0000097B  D1E0              shl ax,1
0000097D  D1E0              shl ax,1
0000097F  50                push ax
00000980  8BC7              mov ax,di
00000982  BB0400            mov bx,0x4
00000985  99                cwd
00000986  F7FB              idiv bx
00000988  D1E2              shl dx,1
0000098A  5B                pop bx
0000098B  03DA              add bx,dx
0000098D  8B87FE2C          mov ax,[bx+0x2cfe]
00000991  8BDE              mov bx,si
00000993  D1E3              shl bx,1
00000995  89871E2D          mov [bx+0x2d1e],ax
00000999  EB20              jmp short 0x9bb
0000099B  EB1E              jmp short 0x9bb
0000099D  8BDE              mov bx,si
0000099F  D1E3              shl bx,1
000009A1  D1E3              shl bx,1
000009A3  C787BE2C0000      mov word [bx+0x2cbe],0x0
000009A9  C787C02C0000      mov word [bx+0x2cc0],0x0
000009AF  8BDE              mov bx,si
000009B1  D1E3              shl bx,1
000009B3  C7871E2D0000      mov word [bx+0x2d1e],0x0
000009B9  EB00              jmp short 0x9bb
000009BB  47                inc di
000009BC  83FF0F            cmp di,byte +0xf
000009BF  7F03              jg 0x9c4
000009C1  E959FF            jmp 0x91d
000009C4  46                inc si
000009C5  83FE0F            cmp si,byte +0xf
000009C8  7F03              jg 0x9cd
000009CA  E94BFF            jmp 0x918
000009CD  33F6              xor si,si
000009CF  EB0E              jmp short 0x9df
000009D1  8BDE              mov bx,si
000009D3  D1E3              shl bx,1
000009D5  83BF1E2D00        cmp word [bx+0x2d1e],byte +0x0
000009DA  7E02              jng 0x9de
000009DC  EB06              jmp short 0x9e4
000009DE  46                inc si
000009DF  83FE0F            cmp si,byte +0xf
000009E2  7EED              jng 0x9d1
000009E4  8936742D          mov [0x2d74],si
000009E8  9AD70E1D14        call 0x141d:0xed7
000009ED  5F                pop di
000009EE  5E                pop si
000009EF  8BE5              mov sp,bp
000009F1  5D                pop bp
000009F2  CB                retf

; ---- 141D:09F3 ----
000009F3  55                push bp
000009F4  8BEC              mov bp,sp
000009F6  A0AA37            mov al,[0x37aa]
000009F9  98                cbw
000009FA  8BD8              mov bx,ax
000009FC  8A87B243          mov al,[bx+0x43b2]
00000A00  B400              mov ah,0x0
00000A02  0BC0              or ax,ax
00000A04  7407              jz 0xa0d
00000A06  3D0100            cmp ax,0x1
00000A09  7409              jz 0xa14
00000A0B  EB0E              jmp short 0xa1b
00000A0D  9A1D0A1D14        call 0x141d:0xa1d
00000A12  EB07              jmp short 0xa1b
00000A14  9AFD0B1D14        call 0x141d:0xbfd
00000A19  EB00              jmp short 0xa1b
00000A1B  5D                pop bp
00000A1C  CB                retf

; ---- 141D:0A1D ----
00000A1D  55                push bp
00000A1E  8BEC              mov bp,sp
00000A20  83EC0A            sub sp,byte +0xa
00000A23  56                push si
00000A24  57                push di
00000A25  A11C3D            mov ax,[0x3d1c]
00000A28  BB0200            mov bx,0x2
00000A2B  99                cwd
00000A2C  F7FB              idiv bx
00000A2E  8B165A45          mov dx,[0x455a]
00000A32  03D0              add dx,ax
00000A34  8BFA              mov di,dx
00000A36  A0AA37            mov al,[0x37aa]
00000A39  98                cbw
00000A3A  D1E0              shl ax,1
00000A3C  D1E0              shl ax,1
00000A3E  8BD8              mov bx,ax
00000A40  8B87502D          mov ax,[bx+0x2d50]
00000A44  8946FE            mov [bp-0x2],ax
00000A47  A0AA37            mov al,[0x37aa]
00000A4A  98                cbw
00000A4B  8BD8              mov bx,ax
00000A4D  C6870833FF        mov byte [bx+0x3308],0xff
00000A52  A0AA37            mov al,[0x37aa]
00000A55  98                cbw
00000A56  D1E0              shl ax,1
00000A58  8BD8              mov bx,ax
00000A5A  83BFA24300        cmp word [bx+0x43a2],byte +0x0
00000A5F  7C22              jl 0xa83
00000A61  A0AA37            mov al,[0x37aa]
00000A64  98                cbw
00000A65  D1E0              shl ax,1
00000A67  8BD8              mov bx,ax
00000A69  8B87A243          mov ax,[bx+0x43a2]
00000A6D  BB0200            mov bx,0x2
00000A70  99                cwd
00000A71  F7FB              idiv bx
00000A73  50                push ax
00000A74  A15A45            mov ax,[0x455a]
00000A77  5A                pop dx
00000A78  03C2              add ax,dx
00000A7A  99                cwd
00000A7B  8946FA            mov [bp-0x6],ax
00000A7E  8956FC            mov [bp-0x4],dx
00000A81  EB21              jmp short 0xaa4
00000A83  A0AA37            mov al,[0x37aa]
00000A86  98                cbw
00000A87  D1E0              shl ax,1
00000A89  8BD8              mov bx,ax
00000A8B  8B87A243          mov ax,[bx+0x43a2]
00000A8F  48                dec ax
00000A90  BB0200            mov bx,0x2
00000A93  99                cwd
00000A94  F7FB              idiv bx
00000A96  50                push ax
00000A97  A15A45            mov ax,[0x455a]
00000A9A  5A                pop dx
00000A9B  03C2              add ax,dx
00000A9D  99                cwd
00000A9E  8946FA            mov [bp-0x6],ax
00000AA1  8956FC            mov [bp-0x4],dx
00000AA4  A0AA37            mov al,[0x37aa]
00000AA7  98                cbw
00000AA8  D1E0              shl ax,1
00000AAA  8BD8              mov bx,ax
00000AAC  8B87A243          mov ax,[bx+0x43a2]
00000AB0  3B061C3D          cmp ax,[0x3d1c]
00000AB4  7D2A              jnl 0xae0
00000AB6  A0B82B            mov al,[0x2bb8]
00000AB9  98                cbw
00000ABA  40                inc ax
00000ABB  50                push ax
00000ABC  A1762D            mov ax,[0x2d76]
00000ABF  99                cwd
00000AC0  5B                pop bx
00000AC1  F7FB              idiv bx
00000AC3  50                push ax
00000AC4  A0AA37            mov al,[0x37aa]
00000AC7  98                cbw
00000AC8  D1E0              shl ax,1
00000ACA  8BD8              mov bx,ax
00000ACC  58                pop ax
00000ACD  3B87782D          cmp ax,[bx+0x2d78]
00000AD1  7F04              jg 0xad7
00000AD3  8BF7              mov si,di
00000AD5  EB07              jmp short 0xade
00000AD7  8BC7              mov ax,di
00000AD9  05FEFF            add ax,0xfffe
00000ADC  8BF0              mov si,ax
00000ADE  EB4B              jmp short 0xb2b
00000AE0  A0B82B            mov al,[0x2bb8]
00000AE3  98                cbw
00000AE4  50                push ax
00000AE5  A1762D            mov ax,[0x2d76]
00000AE8  99                cwd
00000AE9  5B                pop bx
00000AEA  F7FB              idiv bx
00000AEC  50                push ax
00000AED  A0AA37            mov al,[0x37aa]
00000AF0  98                cbw
00000AF1  D1E0              shl ax,1
00000AF3  8BD8              mov bx,ax
00000AF5  8B87782D          mov ax,[bx+0x2d78]
00000AF9  053200            add ax,0x32
00000AFC  5A                pop dx
00000AFD  3BD0              cmp dx,ax
00000AFF  7F23              jg 0xb24
00000B01  803EC02B01        cmp byte [0x2bc0],0x1
00000B06  7518              jnz 0xb20
00000B08  8BC7              mov ax,di
00000B0A  99                cwd
00000B0B  3B16CA2D          cmp dx,[0x2dca]
00000B0F  7C0F              jl 0xb20
00000B11  7F06              jg 0xb19
00000B13  3B06C82D          cmp ax,[0x2dc8]
00000B17  7607              jna 0xb20
00000B19  8BC7              mov ax,di
00000B1B  48                dec ax
00000B1C  8BF0              mov si,ax
00000B1E  EB02              jmp short 0xb22
00000B20  8BF7              mov si,di
00000B22  EB07              jmp short 0xb2b
00000B24  8BC7              mov ax,di
00000B26  05FEFF            add ax,0xfffe
00000B29  8BF0              mov si,ax
00000B2B  3B76FE            cmp si,[bp-0x2]
00000B2E  7D03              jnl 0xb33
00000B30  8B76FE            mov si,[bp-0x2]
00000B33  3B365A45          cmp si,[0x455a]
00000B37  7D04              jnl 0xb3d
00000B39  8B365A45          mov si,[0x455a]
00000B3D  A0AA37            mov al,[0x37aa]
00000B40  98                cbw
00000B41  D1E0              shl ax,1
00000B43  8BD8              mov bx,ax
00000B45  83BF6C2D03        cmp word [bx+0x2d6c],byte +0x3
00000B4A  750C              jnz 0xb58
00000B4C  8B46FE            mov ax,[bp-0x2]
00000B4F  99                cwd
00000B50  8946F6            mov [bp-0xa],ax
00000B53  8956F8            mov [bp-0x8],dx
00000B56  EB0D              jmp short 0xb65
00000B58  8B46FE            mov ax,[bp-0x2]
00000B5B  053C00            add ax,0x3c
00000B5E  99                cwd
00000B5F  8946F6            mov [bp-0xa],ax
00000B62  8956F8            mov [bp-0x8],dx
00000B65  A0AA37            mov al,[0x37aa]
00000B68  98                cbw
00000B69  B104              mov cl,0x4
00000B6B  D3E0              shl ax,cl
00000B6D  8B16D045          mov dx,[0x45d0]
00000B71  D1E2              shl dx,1
00000B73  D1E2              shl dx,1
00000B75  03C2              add ax,dx
00000B77  8BD8              mov bx,ax
00000B79  8B878A2D          mov ax,[bx+0x2d8a]
00000B7D  8B97882D          mov dx,[bx+0x2d88]
00000B81  3B46F8            cmp ax,[bp-0x8]
00000B84  7F29              jg 0xbaf
00000B86  7C05              jl 0xb8d
00000B88  3B56F6            cmp dx,[bp-0xa]
00000B8B  7322              jnc 0xbaf
00000B8D  A0AA37            mov al,[0x37aa]
00000B90  98                cbw
00000B91  B104              mov cl,0x4
00000B93  D3E0              shl ax,cl
00000B95  8B16D045          mov dx,[0x45d0]
00000B99  D1E2              shl dx,1
00000B9B  D1E2              shl dx,1
00000B9D  03C2              add ax,dx
00000B9F  8BD8              mov bx,ax
00000BA1  8B878A2D          mov ax,[bx+0x2d8a]
00000BA5  8B97882D          mov dx,[bx+0x2d88]
00000BA9  8956F6            mov [bp-0xa],dx
00000BAC  8946F8            mov [bp-0x8],ax
00000BAF  8BC6              mov ax,si
00000BB1  99                cwd
00000BB2  3B56F8            cmp dx,[bp-0x8]
00000BB5  7C0A              jl 0xbc1
00000BB7  7F05              jg 0xbbe
00000BB9  3B46F6            cmp ax,[bp-0xa]
00000BBC  7603              jna 0xbc1
00000BBE  8B76F6            mov si,[bp-0xa]
00000BC1  8BC6              mov ax,si
00000BC3  99                cwd
00000BC4  3B56FC            cmp dx,[bp-0x4]
00000BC7  7C14              jl 0xbdd
00000BC9  7F05              jg 0xbd0
00000BCB  3B46FA            cmp ax,[bp-0x6]
00000BCE  760D              jna 0xbdd
00000BD0  A0AA37            mov al,[0x37aa]
00000BD3  98                cbw
00000BD4  8BD8              mov bx,ax
00000BD6  C687083302        mov byte [bx+0x3308],0x2
00000BDB  EB1A              jmp short 0xbf7
00000BDD  8BC6              mov ax,si
00000BDF  99                cwd
00000BE0  3B56FC            cmp dx,[bp-0x4]
00000BE3  7F12              jg 0xbf7
00000BE5  7C05              jl 0xbec
00000BE7  3B46FA            cmp ax,[bp-0x6]
00000BEA  730B              jnc 0xbf7
00000BEC  A0AA37            mov al,[0x37aa]
00000BEF  98                cbw
00000BF0  8BD8              mov bx,ax
00000BF2  C687083303        mov byte [bx+0x3308],0x3
00000BF7  5F                pop di
00000BF8  5E                pop si
00000BF9  8BE5              mov sp,bp
00000BFB  5D                pop bp
00000BFC  CB                retf

; ---- 141D:0BFD ----
00000BFD  55                push bp
00000BFE  8BEC              mov bp,sp
00000C00  4C                dec sp
00000C01  4C                dec sp
00000C02  56                push si
00000C03  57                push di
00000C04  A0AA37            mov al,[0x37aa]
00000C07  98                cbw
00000C08  D1E0              shl ax,1
00000C0A  8BD8              mov bx,ax
00000C0C  8B87A243          mov ax,[bx+0x43a2]
00000C10  BB0200            mov bx,0x2
00000C13  99                cwd
00000C14  F7FB              idiv bx
00000C16  8B165A45          mov dx,[0x455a]
00000C1A  03D0              add dx,ax
00000C1C  8BFA              mov di,dx
00000C1E  A1BA2B            mov ax,[0x2bba]
00000C21  2B068037          sub ax,[0x3780]
00000C25  8BF0              mov si,ax
00000C27  A0AA37            mov al,[0x37aa]
00000C2A  98                cbw
00000C2B  D1E0              shl ax,1
00000C2D  8BD8              mov bx,ax
00000C2F  83BF802D0A        cmp word [bx+0x2d80],byte +0xa
00000C34  7D07              jnl 0xc3d
00000C36  C746FE0A00        mov word [bp-0x2],0xa
00000C3B  EB7A              jmp short 0xcb7
00000C3D  83FE0C            cmp si,byte +0xc
00000C40  7C07              jl 0xc49
00000C42  C746FE0A00        mov word [bp-0x2],0xa
00000C47  EB6E              jmp short 0xcb7
00000C49  83FE03            cmp si,byte +0x3
00000C4C  7C34              jl 0xc82
00000C4E  A0AA37            mov al,[0x37aa]
00000C51  98                cbw
00000C52  D1E0              shl ax,1
00000C54  8BD8              mov bx,ax
00000C56  81BF802D5802      cmp word [bx+0x2d80],0x258
00000C5C  7D0E              jnl 0xc6c
00000C5E  B89001            mov ax,0x190
00000C61  50                push ax
00000C62  9ABA01FD21        call 0x21fd:0x1ba
00000C67  59                pop cx
00000C68  0BC0              or ax,ax
00000C6A  7516              jnz 0xc82
00000C6C  FF06742D          inc word [0x2d74]
00000C70  9AD70E1D14        call 0x141d:0xed7
00000C75  A18037            mov ax,[0x3780]
00000C78  A3BA2B            mov [0x2bba],ax
00000C7B  C746FEFFFF        mov word [bp-0x2],0xffff
00000C80  EB35              jmp short 0xcb7
00000C82  8BC7              mov ax,di
00000C84  99                cwd
00000C85  3B16622D          cmp dx,[0x2d62]
00000C89  7C0F              jl 0xc9a
00000C8B  7F06              jg 0xc93
00000C8D  3B06602D          cmp ax,[0x2d60]
00000C91  7607              jna 0xc9a
00000C93  C746FE0300        mov word [bp-0x2],0x3
00000C98  EB1D              jmp short 0xcb7
00000C9A  8BC7              mov ax,di
00000C9C  99                cwd
00000C9D  3B16622D          cmp dx,[0x2d62]
00000CA1  7F0F              jg 0xcb2
00000CA3  7C06              jl 0xcab
00000CA5  3B06602D          cmp ax,[0x2d60]
00000CA9  7307              jnc 0xcb2
00000CAB  C746FE0200        mov word [bp-0x2],0x2
00000CB0  EB05              jmp short 0xcb7
00000CB2  C746FEFFFF        mov word [bp-0x2],0xffff
00000CB7  A0AA37            mov al,[0x37aa]
00000CBA  98                cbw
00000CBB  8A56FE            mov dl,[bp-0x2]
00000CBE  8BD8              mov bx,ax
00000CC0  88970833          mov [bx+0x3308],dl
00000CC4  5F                pop di
00000CC5  5E                pop si
00000CC6  8BE5              mov sp,bp
00000CC8  5D                pop bp
00000CC9  CB                retf

; ---- 141D:0CCA ----
00000CCA  55                push bp
00000CCB  8BEC              mov bp,sp
00000CCD  56                push si
00000CCE  8B1E0641          mov bx,[0x4106]
00000CD2  80BF400800        cmp byte [bx+0x840],0x0
00000CD7  7503              jnz 0xcdc
00000CD9  E92A01            jmp 0xe06
00000CDC  8B1E0641          mov bx,[0x4106]
00000CE0  D1E3              shl bx,1
00000CE2  8B87782D          mov ax,[bx+0x2d78]
00000CE6  05F6FF            add ax,0xfff6
00000CE9  8B1E0641          mov bx,[0x4106]
00000CED  D1E3              shl bx,1
00000CEF  8987782D          mov [bx+0x2d78],ax
00000CF3  0BC0              or ax,ax
00000CF5  7D0C              jnl 0xd03
00000CF7  8B1E0641          mov bx,[0x4106]
00000CFB  D1E3              shl bx,1
00000CFD  C787782D0000      mov word [bx+0x2d78],0x0
00000D03  8B1E0641          mov bx,[0x4106]
00000D07  D1E3              shl bx,1
00000D09  8BB76C2D          mov si,[bx+0x2d6c]
00000D0D  8B1E0641          mov bx,[0x4106]
00000D11  D1E3              shl bx,1
00000D13  D1E3              shl bx,1
00000D15  D1E3              shl bx,1
00000D17  8BC6              mov ax,si
00000D19  D1E0              shl ax,1
00000D1B  03D8              add bx,ax
00000D1D  8B87FE2C          mov ax,[bx+0x2cfe]
00000D21  05F6FF            add ax,0xfff6
00000D24  8B1E0641          mov bx,[0x4106]
00000D28  D1E3              shl bx,1
00000D2A  D1E3              shl bx,1
00000D2C  D1E3              shl bx,1
00000D2E  8BD6              mov dx,si
00000D30  D1E2              shl dx,1
00000D32  03DA              add bx,dx
00000D34  8987FE2C          mov [bx+0x2cfe],ax
00000D38  8B1E0641          mov bx,[0x4106]
00000D3C  B104              mov cl,0x4
00000D3E  D3E3              shl bx,cl
00000D40  A1D045            mov ax,[0x45d0]
00000D43  D1E0              shl ax,1
00000D45  D1E0              shl ax,1
00000D47  03D8              add bx,ax
00000D49  8B4606            mov ax,[bp+0x6]
00000D4C  99                cwd
00000D4D  2987882D          sub [bx+0x2d88],ax
00000D51  19978A2D          sbb [bx+0x2d8a],dx
00000D55  8B1E0641          mov bx,[0x4106]
00000D59  D1E3              shl bx,1
00000D5B  D1E3              shl bx,1
00000D5D  D1E3              shl bx,1
00000D5F  8BC6              mov ax,si
00000D61  D1E0              shl ax,1
00000D63  03D8              add bx,ax
00000D65  83BFFE2C00        cmp word [bx+0x2cfe],byte +0x0
00000D6A  7E03              jng 0xd6f
00000D6C  E99700            jmp 0xe06
00000D6F  46                inc si
00000D70  8B1E0641          mov bx,[0x4106]
00000D74  D1E3              shl bx,1
00000D76  8B876C2D          mov ax,[bx+0x2d6c]
00000D7A  40                inc ax
00000D7B  8B1E0641          mov bx,[0x4106]
00000D7F  D1E3              shl bx,1
00000D81  89876C2D          mov [bx+0x2d6c],ax
00000D85  3D0300            cmp ax,0x3
00000D88  7E33              jng 0xdbd
00000D8A  8B1E0641          mov bx,[0x4106]
00000D8E  D1E3              shl bx,1
00000D90  D1E3              shl bx,1
00000D92  C787502D0000      mov word [bx+0x2d50],0x0
00000D98  C787522D0000      mov word [bx+0x2d52],0x0
00000D9E  8B1E0641          mov bx,[0x4106]
00000DA2  B104              mov cl,0x4
00000DA4  D3E3              shl bx,cl
00000DA6  A1D045            mov ax,[0x45d0]
00000DA9  D1E0              shl ax,1
00000DAB  D1E0              shl ax,1
00000DAD  03D8              add bx,ax
00000DAF  C787882D0000      mov word [bx+0x2d88],0x0
00000DB5  C7878A2D0000      mov word [bx+0x2d8a],0x0
00000DBB  EB44              jmp short 0xe01
00000DBD  8B1E0641          mov bx,[0x4106]
00000DC1  D1E3              shl bx,1
00000DC3  D1E3              shl bx,1
00000DC5  D1E3              shl bx,1
00000DC7  8BC6              mov ax,si
00000DC9  D1E0              shl ax,1
00000DCB  03D8              add bx,ax
00000DCD  83BFFE2C00        cmp word [bx+0x2cfe],byte +0x0
00000DD2  7E2A              jng 0xdfe
00000DD4  8B1E0641          mov bx,[0x4106]
00000DD8  B104              mov cl,0x4
00000DDA  D3E3              shl bx,cl
00000DDC  8BC6              mov ax,si
00000DDE  D1E0              shl ax,1
00000DE0  D1E0              shl ax,1
00000DE2  03D8              add bx,ax
00000DE4  8B87802C          mov ax,[bx+0x2c80]
00000DE8  8B977E2C          mov dx,[bx+0x2c7e]
00000DEC  8B1E0641          mov bx,[0x4106]
00000DF0  D1E3              shl bx,1
00000DF2  D1E3              shl bx,1
00000DF4  8997502D          mov [bx+0x2d50],dx
00000DF8  8987522D          mov [bx+0x2d52],ax
00000DFC  EB03              jmp short 0xe01
00000DFE  E96EFF            jmp 0xd6f
00000E01  9A090E1D14        call 0x141d:0xe09
00000E06  5E                pop si
00000E07  5D                pop bp
00000E08  CB                retf

; ---- 141D:0E09 ----
00000E09  55                push bp
00000E0A  8BEC              mov bp,sp
00000E0C  56                push si
00000E0D  C706C82D0000      mov word [0x2dc8],0x0
00000E13  C706CA2D0000      mov word [0x2dca],0x0
00000E19  33F6              xor si,si
00000E1B  EB42              jmp short 0xe5f
00000E1D  80BCB24300        cmp byte [si+0x43b2],0x0
00000E22  753A              jnz 0xe5e
00000E24  80BC400800        cmp byte [si+0x840],0x0
00000E29  7502              jnz 0xe2d
00000E2B  EB31              jmp short 0xe5e
00000E2D  8BDE              mov bx,si
00000E2F  D1E3              shl bx,1
00000E31  D1E3              shl bx,1
00000E33  8B87522D          mov ax,[bx+0x2d52]
00000E37  8B97502D          mov dx,[bx+0x2d50]
00000E3B  3B06CA2D          cmp ax,[0x2dca]
00000E3F  7C1D              jl 0xe5e
00000E41  7F06              jg 0xe49
00000E43  3B16C82D          cmp dx,[0x2dc8]
00000E47  7615              jna 0xe5e
00000E49  8BDE              mov bx,si
00000E4B  D1E3              shl bx,1
00000E4D  D1E3              shl bx,1
00000E4F  8B87522D          mov ax,[bx+0x2d52]
00000E53  8B97502D          mov dx,[bx+0x2d50]
00000E57  8916C82D          mov [0x2dc8],dx
00000E5B  A3CA2D            mov [0x2dca],ax
00000E5E  46                inc si
00000E5F  83FE03            cmp si,byte +0x3
00000E62  7EB9              jng 0xe1d
00000E64  5E                pop si
00000E65  5D                pop bp
00000E66  CB                retf

; ---- 141D:0E67 ----
00000E67  55                push bp
00000E68  8BEC              mov bp,sp
00000E6A  8B1EF244          mov bx,[0x44f2]
00000E6E  D1E3              shl bx,1
00000E70  8B87802D          mov ax,[bx+0x2d80]
00000E74  05F6FF            add ax,0xfff6
00000E77  8B1EF244          mov bx,[0x44f2]
00000E7B  D1E3              shl bx,1
00000E7D  8987802D          mov [bx+0x2d80],ax
00000E81  0BC0              or ax,ax
00000E83  7D0C              jnl 0xe91
00000E85  8B1EF244          mov bx,[0x44f2]
00000E89  D1E3              shl bx,1
00000E8B  C787802D0000      mov word [bx+0x2d80],0x0
00000E91  9A010F1D14        call 0x141d:0xf01
00000E96  833E742D0F        cmp word [0x2d74],byte +0xf
00000E9B  7E02              jng 0xe9f
00000E9D  EB36              jmp short 0xed5
00000E9F  8B1E742D          mov bx,[0x2d74]
00000EA3  D1E3              shl bx,1
00000EA5  8B871E2D          mov ax,[bx+0x2d1e]
00000EA9  05F6FF            add ax,0xfff6
00000EAC  8B1E742D          mov bx,[0x2d74]
00000EB0  D1E3              shl bx,1
00000EB2  89871E2D          mov [bx+0x2d1e],ax
00000EB6  EB04              jmp short 0xebc
00000EB8  FF06742D          inc word [0x2d74]
00000EBC  8B1E742D          mov bx,[0x2d74]
00000EC0  D1E3              shl bx,1
00000EC2  83BF1E2D00        cmp word [bx+0x2d1e],byte +0x0
00000EC7  7F07              jg 0xed0
00000EC9  833E742D0F        cmp word [0x2d74],byte +0xf
00000ECE  7EE8              jng 0xeb8
00000ED0  9AD70E1D14        call 0x141d:0xed7
00000ED5  5D                pop bp
00000ED6  CB                retf

; ---- 141D:0ED7 ----
00000ED7  55                push bp
00000ED8  8BEC              mov bp,sp
00000EDA  833E742D0F        cmp word [0x2d74],byte +0xf
00000EDF  7E07              jng 0xee8
00000EE1  33D2              xor dx,dx
00000EE3  B80100            mov ax,0x1
00000EE6  EB10              jmp short 0xef8
00000EE8  8B1E742D          mov bx,[0x2d74]
00000EEC  D1E3              shl bx,1
00000EEE  D1E3              shl bx,1
00000EF0  8B97C02C          mov dx,[bx+0x2cc0]
00000EF4  8B87BE2C          mov ax,[bx+0x2cbe]
00000EF8  A3602D            mov [0x2d60],ax
00000EFB  8916622D          mov [0x2d62],dx
00000EFF  5D                pop bp
00000F00  CB                retf

; ---- 141D:0F01 ----
00000F01  55                push bp
00000F02  8BEC              mov bp,sp
00000F04  56                push si
00000F05  C706762D0000      mov word [0x2d76],0x0
00000F0B  33F6              xor si,si
00000F0D  EB14              jmp short 0xf23
00000F0F  80BCB24301        cmp byte [si+0x43b2],0x1
00000F14  750C              jnz 0xf22
00000F16  8BDE              mov bx,si
00000F18  D1E3              shl bx,1
00000F1A  8B87802D          mov ax,[bx+0x2d80]
00000F1E  01                db 0x01
00000F1F  06                push es
