; segment 22A3  file offset 0x26030  size 0x930
00000000  27                daa
00000001  5D                pop bp
00000002  CB                retf

; ---- 22A3:0003 ----
00000003  55                push bp
00000004  8BEC              mov bp,sp
00000006  83EC10            sub sp,byte +0x10
00000009  56                push si
0000000A  57                push di
0000000B  9A5007A322        call 0x22a3:0x750
00000010  C606AA3700        mov byte [0x37aa],0x0
00000015  EB43              jmp short 0x5a
00000017  33F6              xor si,si
00000019  EB36              jmp short 0x51
0000001B  A0AA37            mov al,[0x37aa]
0000001E  98                cbw
0000001F  BA0300            mov dx,0x3
00000022  F7EA              imul dx
00000024  8BD8              mov bx,ax
00000026  8A802C34          mov al,[bx+si+0x342c]
0000002A  B400              mov ah,0x0
0000002C  BA0300            mov dx,0x3
0000002F  F7EA              imul dx
00000031  BB0400            mov bx,0x4
00000034  99                cwd
00000035  F7FB              idiv bx
00000037  0419              add al,0x19
00000039  50                push ax
0000003A  A0AA37            mov al,[0x37aa]
0000003D  98                cbw
0000003E  BA0300            mov dx,0x3
00000041  F7EA              imul dx
00000043  03C6              add ax,si
00000045  8D56F0            lea dx,[bp-0x10]
00000048  03C2              add ax,dx
0000004A  8BD8              mov bx,ax
0000004C  58                pop ax
0000004D  368807            mov [ss:bx],al
00000050  46                inc si
00000051  83FE02            cmp si,byte +0x2
00000054  7EC5              jng 0x1b
00000056  FE06AA37          inc byte [0x37aa]
0000005A  803EAA3704        cmp byte [0x37aa],0x4
0000005F  7CB6              jl 0x17
00000061  C706F0570000      mov word [0x57f0],0x0
00000067  C706F2570000      mov word [0x57f2],0x0
0000006D  C606002F00        mov byte [0x2f00],0x0
00000072  E9E001            jmp 0x255
00000075  C606B02E00        mov byte [0x2eb0],0x0
0000007A  E9C401            jmp 0x241
0000007D  A0B02E            mov al,[0x2eb0]
00000080  98                cbw
00000081  BA1A00            mov dx,0x1a
00000084  F7EA              imul dx
00000086  50                push ax
00000087  A0002F            mov al,[0x2f00]
0000008A  98                cbw
0000008B  5B                pop bx
0000008C  03D8              add bx,ax
0000008E  33C0              xor ax,ax
00000090  8946FC            mov [bp-0x4],ax
00000093  8887F448          mov [bx+0x48f4],al
00000097  A0B02E            mov al,[0x2eb0]
0000009A  98                cbw
0000009B  BA0600            mov dx,0x6
0000009E  F7EA              imul dx
000000A0  50                push ax
000000A1  A0002F            mov al,[0x2f00]
000000A4  98                cbw
000000A5  5B                pop bx
000000A6  03D8              add bx,ax
000000A8  8A874244          mov al,[bx+0x4442]
000000AC  A2AA37            mov [0x37aa],al
000000AF  A0B02E            mov al,[0x2eb0]
000000B2  98                cbw
000000B3  BA0600            mov dx,0x6
000000B6  F7EA              imul dx
000000B8  50                push ax
000000B9  A0002F            mov al,[0x2f00]
000000BC  98                cbw
000000BD  5B                pop bx
000000BE  03D8              add bx,ax
000000C0  8A87B12E          mov al,[bx+0x2eb1]
000000C4  98                cbw
000000C5  8BF0              mov si,ax
000000C7  A0B02E            mov al,[0x2eb0]
000000CA  98                cbw
000000CB  BA0600            mov dx,0x6
000000CE  F7EA              imul dx
000000D0  50                push ax
000000D1  A0002F            mov al,[0x2f00]
000000D4  98                cbw
000000D5  5B                pop bx
000000D6  03D8              add bx,ax
000000D8  8A87DA45          mov al,[bx+0x45da]
000000DC  98                cbw
000000DD  8946FE            mov [bp-0x2],ax
000000E0  803EAA37FF        cmp byte [0x37aa],0xff
000000E5  7503              jnz 0xea
000000E7  E95301            jmp 0x23d
000000EA  83FEFE            cmp si,byte -0x2
000000ED  7507              jnz 0xf6
000000EF  FF06F057          inc word [0x57f0]
000000F3  E94701            jmp 0x23d
000000F6  83FE04            cmp si,byte +0x4
000000F9  7503              jnz 0xfe
000000FB  E93F01            jmp 0x23d
000000FE  83FE03            cmp si,byte +0x3
00000101  754A              jnz 0x14d
00000103  A0AA37            mov al,[0x37aa]
00000106  98                cbw
00000107  D1E0              shl ax,1
00000109  8BD8              mov bx,ax
0000010B  8B9F0E45          mov bx,[bx+0x450e]
0000010F  C6871A2F01        mov byte [bx+0x2f1a],0x1
00000114  A0AA37            mov al,[0x37aa]
00000117  98                cbw
00000118  D1E0              shl ax,1
0000011A  D1E0              shl ax,1
0000011C  8BD8              mov bx,ax
0000011E  8A87EA2E          mov al,[bx+0x2eea]
00000122  98                cbw
00000123  BA0600            mov dx,0x6
00000126  F7EA              imul dx
00000128  8946FC            mov [bp-0x4],ax
0000012B  A0AA37            mov al,[0x37aa]
0000012E  98                cbw
0000012F  D1E0              shl ax,1
00000131  8BD8              mov bx,ax
00000133  8B9F0E45          mov bx,[bx+0x450e]
00000137  D1E3              shl bx,1
00000139  8B87F444          mov ax,[bx+0x44f4]
0000013D  BA0A00            mov dx,0xa
00000140  F7EA              imul dx
00000142  8B56FC            mov dx,[bp-0x4]
00000145  03D0              add dx,ax
00000147  8956FC            mov [bp-0x4],dx
0000014A  E9C200            jmp 0x20f
0000014D  8A84E408          mov al,[si+0x8e4]
00000151  98                cbw
00000152  8BF8              mov di,ax
00000154  A0AA37            mov al,[0x37aa]
00000157  98                cbw
00000158  D1E0              shl ax,1
0000015A  D1E0              shl ax,1
0000015C  D1E0              shl ax,1
0000015E  8BD7              mov dx,di
00000160  D1E2              shl dx,1
00000162  03C2              add ax,dx
00000164  50                push ax
00000165  A0262F            mov al,[0x2f26]
00000168  98                cbw
00000169  5B                pop bx
0000016A  3987F43C          cmp [bx+0x3cf4],ax
0000016E  7D07              jnl 0x177
00000170  FF06F057          inc word [0x57f0]
00000174  E99800            jmp 0x20f
00000177  FF06F257          inc word [0x57f2]
0000017B  A0AA37            mov al,[0x37aa]
0000017E  98                cbw
0000017F  D1E0              shl ax,1
00000181  D1E0              shl ax,1
00000183  D1E0              shl ax,1
00000185  8BD7              mov dx,di
00000187  D1E2              shl dx,1
00000189  03C2              add ax,dx
0000018B  50                push ax
0000018C  A0262F            mov al,[0x2f26]
0000018F  98                cbw
00000190  5B                pop bx
00000191  2987F43C          sub [bx+0x3cf4],ax
00000195  A0AA37            mov al,[0x37aa]
00000198  98                cbw
00000199  D1E0              shl ax,1
0000019B  D1E0              shl ax,1
0000019D  8BD8              mov bx,ax
0000019F  8A80E72E          mov al,[bx+si+0x2ee7]
000001A3  98                cbw
000001A4  BA0300            mov dx,0x3
000001A7  F7EA              imul dx
000001A9  8946FC            mov [bp-0x4],ax
000001AC  8BDE              mov bx,si
000001AE  D1E3              shl bx,1
000001B0  D1E3              shl bx,1
000001B2  035EFE            add bx,[bp-0x2]
000001B5  8A870209          mov al,[bx+0x902]
000001B9  98                cbw
000001BA  BA0A00            mov dx,0xa
000001BD  F7EA              imul dx
000001BF  8B56FC            mov dx,[bp-0x4]
000001C2  03D0              add dx,ax
000001C4  8956FC            mov [bp-0x4],dx
000001C7  A0AA37            mov al,[0x37aa]
000001CA  98                cbw
000001CB  BA0300            mov dx,0x3
000001CE  F7EA              imul dx
000001D0  03C6              add ax,si
000001D2  8D56F0            lea dx,[bp-0x10]
000001D5  03C2              add ax,dx
000001D7  8BD8              mov bx,ax
000001D9  368A07            mov al,[ss:bx]
000001DC  B400              mov ah,0x0
000001DE  F76EFC            imul word [bp-0x4]
000001E1  BB6400            mov bx,0x64
000001E4  99                cwd
000001E5  F7FB              idiv bx
000001E7  8946FC            mov [bp-0x4],ax
000001EA  8BC6              mov ax,si
000001EC  BA0600            mov dx,0x6
000001EF  F7EA              imul dx
000001F1  50                push ax
000001F2  A0E640            mov al,[0x40e6]
000001F5  98                cbw
000001F6  5B                pop bx
000001F7  03D8              add bx,ax
000001F9  8A872C11          mov al,[bx+0x112c]
000001FD  98                cbw
000001FE  F76EFC            imul word [bp-0x4]
00000201  BB6400            mov bx,0x64
00000204  99                cwd
00000205  F7FB              idiv bx
00000207  8B56FC            mov dx,[bp-0x4]
0000020A  03D0              add dx,ax
0000020C  8956FC            mov [bp-0x4],dx
0000020F  837EFC04          cmp word [bp-0x4],byte +0x4
00000213  7D05              jnl 0x21a
00000215  C746FC0400        mov word [bp-0x4],0x4
0000021A  837EFC3C          cmp word [bp-0x4],byte +0x3c
0000021E  7E05              jng 0x225
00000220  C746FC3C00        mov word [bp-0x4],0x3c
00000225  A0B02E            mov al,[0x2eb0]
00000228  98                cbw
00000229  BA1A00            mov dx,0x1a
0000022C  F7EA              imul dx
0000022E  50                push ax
0000022F  A0002F            mov al,[0x2f00]
00000232  98                cbw
00000233  5B                pop bx
00000234  03D8              add bx,ax
00000236  8A46FC            mov al,[bp-0x4]
00000239  8887F448          mov [bx+0x48f4],al
0000023D  FE06B02E          inc byte [0x2eb0]
00000241  A0B02E            mov al,[0x2eb0]
00000244  98                cbw
00000245  8B16FC08          mov dx,[0x8fc]
00000249  4A                dec dx
0000024A  3BC2              cmp ax,dx
0000024C  7F03              jg 0x251
0000024E  E92CFE            jmp 0x7d
00000251  FE06002F          inc byte [0x2f00]
00000255  A0002F            mov al,[0x2f00]
00000258  98                cbw
00000259  8B16FE08          mov dx,[0x8fe]
0000025D  4A                dec dx
0000025E  3BC2              cmp ax,dx
00000260  7F03              jg 0x265
00000262  E910FE            jmp 0x75
00000265  5F                pop di
00000266  5E                pop si
00000267  8BE5              mov sp,bp
00000269  5D                pop bp
0000026A  CB                retf

; ---- 22A3:026B ----
0000026B  55                push bp
0000026C  8BEC              mov bp,sp
0000026E  C606AA3700        mov byte [0x37aa],0x0
00000273  EB55              jmp short 0x2ca
00000275  C706D0450000      mov word [0x45d0],0x0
0000027B  EB42              jmp short 0x2bf
0000027D  A0AA37            mov al,[0x37aa]
00000280  98                cbw
00000281  D1E0              shl ax,1
00000283  D1E0              shl ax,1
00000285  D1E0              shl ax,1
00000287  8B16D045          mov dx,[0x45d0]
0000028B  D1E2              shl dx,1
0000028D  03C2              add ax,dx
0000028F  8BD8              mov bx,ax
00000291  8B87F43C          mov ax,[bx+0x3cf4]
00000295  D1E0              shl ax,1
00000297  D1E0              shl ax,1
00000299  D1E0              shl ax,1
0000029B  BB0A00            mov bx,0xa
0000029E  99                cwd
0000029F  F7FB              idiv bx
000002A1  50                push ax
000002A2  A0AA37            mov al,[0x37aa]
000002A5  98                cbw
000002A6  D1E0              shl ax,1
000002A8  D1E0              shl ax,1
000002AA  D1E0              shl ax,1
000002AC  8B16D045          mov dx,[0x45d0]
000002B0  D1E2              shl dx,1
000002B2  03C2              add ax,dx
000002B4  8BD8              mov bx,ax
000002B6  58                pop ax
000002B7  8987F43C          mov [bx+0x3cf4],ax
000002BB  FF06D045          inc word [0x45d0]
000002BF  833ED04503        cmp word [0x45d0],byte +0x3
000002C4  7EB7              jng 0x27d
000002C6  FE06AA37          inc byte [0x37aa]
000002CA  803EAA3704        cmp byte [0x37aa],0x4
000002CF  7CA4              jl 0x275
000002D1  5D                pop bp
000002D2  CB                retf

; ---- 22A3:02D3 ----
000002D3  55                push bp
000002D4  8BEC              mov bp,sp
000002D6  FF36CE08          push word [0x8ce]
000002DA  9A02006C18        call 0x186c:0x2
000002DF  59                pop cx
000002E0  9A2D1F0906        call 0x609:0x1f2d
000002E5  B80700            mov ax,0x7
000002E8  50                push ax
000002E9  9A7D03FD21        call 0x21fd:0x37d
000002EE  59                pop cx
000002EF  9AE51B0906        call 0x609:0x1be5
000002F4  C606E63C04        mov byte [0x3ce6],0x4
000002F9  1E                push ds
000002FA  B8A800            mov ax,0xa8
000002FD  50                push ax
000002FE  9A6C02FD21        call 0x21fd:0x26c
00000303  59                pop cx
00000304  59                pop cx
00000305  9AE7110906        call 0x609:0x11e7
0000030A  9A2903FD21        call 0x21fd:0x329
0000030F  9AC108A322        call 0x22a3:0x8c1
00000314  9A4003A322        call 0x22a3:0x340
00000319  9A1B00960A        call 0xa96:0x1b
0000031E  B81700            mov ax,0x17
00000321  50                push ax
00000322  33C0              xor ax,ax
00000324  50                push ax
00000325  9A5400A721        call 0x21a7:0x54
0000032A  59                pop cx
0000032B  59                pop cx
0000032C  9A2604A322        call 0x22a3:0x426
00000331  33C0              xor ax,ax
00000333  50                push ax
00000334  33C0              xor ax,ax
00000336  50                push ax
00000337  9A5400A721        call 0x21a7:0x54
0000033C  59                pop cx
0000033D  59                pop cx
0000033E  5D                pop bp
0000033F  CB                retf

; ---- 22A3:0340 ----
00000340  55                push bp
00000341  8BEC              mov bp,sp
00000343  C606AA3700        mov byte [0x37aa],0x0
00000348  E9CF00            jmp 0x41a
0000034B  9A1B00960A        call 0xa96:0x1b
00000350  B80600            mov ax,0x6
00000353  50                push ax
00000354  A0AA37            mov al,[0x37aa]
00000357  98                cbw
00000358  B106              mov cl,0x6
0000035A  D3E0              shl ax,cl
0000035C  052000            add ax,0x20
0000035F  50                push ax
00000360  9A54180906        call 0x609:0x1854
00000365  59                pop cx
00000366  59                pop cx
00000367  9AC01A0906        call 0x609:0x1ac0
0000036C  9ACC1F0906        call 0x609:0x1fcc
00000371  A0AA37            mov al,[0x37aa]
00000374  98                cbw
00000375  D1E0              shl ax,1
00000377  D1E0              shl ax,1
00000379  8BD8              mov bx,ax
0000037B  C78786370000      mov word [bx+0x3786],0x0
00000381  C78788370000      mov word [bx+0x3788],0x0
00000387  9AE5180906        call 0x609:0x18e5
0000038C  C706D0450000      mov word [0x45d0],0x0
00000392  EB2E              jmp short 0x3c2
00000394  A1D045            mov ax,[0x45d0]
00000397  D1E0              shl ax,1
00000399  D1E0              shl ax,1
0000039B  D1E0              shl ax,1
0000039D  050D00            add ax,0xd
000003A0  50                push ax
000003A1  A0AA37            mov al,[0x37aa]
000003A4  98                cbw
000003A5  B106              mov cl,0x6
000003A7  D3E0              shl ax,cl
000003A9  052000            add ax,0x20
000003AC  50                push ax
000003AD  FF36D045          push word [0x45d0]
000003B1  9A12190906        call 0x609:0x1912
000003B6  83C406            add sp,byte +0x6
000003B9  9A7408A322        call 0x22a3:0x874
000003BE  FF06D045          inc word [0x45d0]
000003C2  833ED04503        cmp word [0x45d0],byte +0x3
000003C7  7ECB              jng 0x394
000003C9  A0AA37            mov al,[0x37aa]
000003CC  98                cbw
000003CD  8BD8              mov bx,ax
000003CF  C6874E30FF        mov byte [bx+0x304e],0xff
000003D4  A0AA37            mov al,[0x37aa]
000003D7  98                cbw
000003D8  8BD8              mov bx,ax
000003DA  C6875A3000        mov byte [bx+0x305a],0x0
000003DF  9AF704A322        call 0x22a3:0x4f7
000003E4  A0AA37            mov al,[0x37aa]
000003E7  98                cbw
000003E8  D1E0              shl ax,1
000003EA  8BD8              mov bx,ax
000003EC  C787E8570000      mov word [bx+0x57e8],0x0
000003F2  A0AA37            mov al,[0x37aa]
000003F5  98                cbw
000003F6  8BD8              mov bx,ax
000003F8  80BF400801        cmp byte [bx+0x840],0x1
000003FD  7504              jnz 0x403
000003FF  B001              mov al,0x1
00000401  EB02              jmp short 0x405
00000403  B000              mov al,0x0
00000405  50                push ax
00000406  A0AA37            mov al,[0x37aa]
00000409  98                cbw
0000040A  8BD8              mov bx,ax
0000040C  58                pop ax
0000040D  8887F457          mov [bx+0x57f4],al
00000411  9A0D06FD21        call 0x21fd:0x60d
00000416  FE06AA37          inc byte [0x37aa]
0000041A  803EAA3704        cmp byte [0x37aa],0x4
0000041F  7D03              jnl 0x424
00000421  E927FF            jmp 0x34b
00000424  5D                pop bp
00000425  CB                retf

; ---- 22A3:0426 ----
00000426  55                push bp
00000427  8BEC              mov bp,sp
00000429  EB78              jmp short 0x4a3
0000042B  9A1F05FD21        call 0x21fd:0x51f
00000430  33C0              xor ax,ax
00000432  BA0100            mov dx,0x1
00000435  50                push ax
00000436  52                push dx
00000437  9A8501FD21        call 0x21fd:0x185
0000043C  59                pop cx
0000043D  59                pop cx
0000043E  FF362E08          push word [0x82e]
00000442  9A1C07CD23        call 0x23cd:0x71c
00000447  59                pop cx
00000448  9AA806960A        call 0xa96:0x6a8
0000044D  9A1B00960A        call 0xa96:0x1b
00000452  9A7A039E0E        call 0xe9e:0x37a
00000457  9A4502FD21        call 0x21fd:0x245
0000045C  C606AA3700        mov byte [0x37aa],0x0
00000461  EB2A              jmp short 0x48d
00000463  A0AA37            mov al,[0x37aa]
00000466  98                cbw
00000467  8BD8              mov bx,ax
00000469  8A87B243          mov al,[bx+0x43b2]
0000046D  B400              mov ah,0x0
0000046F  3D0700            cmp ax,0x7
00000472  7407              jz 0x47b
00000474  3D0800            cmp ax,0x8
00000477  7409              jz 0x482
00000479  EB0E              jmp short 0x489
0000047B  9AB104A322        call 0x22a3:0x4b1
00000480  EB07              jmp short 0x489
00000482  9A2E06A322        call 0x22a3:0x62e
00000487  EB00              jmp short 0x489
00000489  FE06AA37          inc byte [0x37aa]
0000048D  803EAA3704        cmp byte [0x37aa],0x4
00000492  7CCF              jl 0x463
00000494  9AA806960A        call 0xa96:0x6a8
00000499  FF362E08          push word [0x82e]
0000049D  9AE906CD23        call 0x23cd:0x6e9
000004A2  59                pop cx
000004A3  803EE63C00        cmp byte [0x3ce6],0x0
000004A8  7581              jnz 0x42b
000004AA  9A6506FD21        call 0x21fd:0x665
000004AF  5D                pop bp
000004B0  CB                retf

; ---- 22A3:04B1 ----
000004B1  55                push bp
000004B2  8BEC              mov bp,sp
000004B4  A0AA37            mov al,[0x37aa]
000004B7  98                cbw
000004B8  8BD8              mov bx,ax
000004BA  80BF083300        cmp byte [bx+0x3308],0x0
000004BF  740D              jz 0x4ce
000004C1  A0AA37            mov al,[0x37aa]
000004C4  98                cbw
000004C5  8BD8              mov bx,ax
000004C7  80BFF45701        cmp byte [bx+0x57f4],0x1
000004CC  7505              jnz 0x4d3
000004CE  9AF704A322        call 0x22a3:0x4f7
000004D3  9ADA04A322        call 0x22a3:0x4da
000004D8  5D                pop bp
000004D9  CB                retf

; ---- 22A3:04DA ----
000004DA  55                push bp
000004DB  8BEC              mov bp,sp
000004DD  A0AA37            mov al,[0x37aa]
000004E0  98                cbw
000004E1  8BD8              mov bx,ax
000004E3  80BF08330A        cmp byte [bx+0x3308],0xa
000004E8  750B              jnz 0x4f5
000004EA  A0AA37            mov al,[0x37aa]
000004ED  98                cbw
000004EE  8BD8              mov bx,ax
000004F0  C687F45701        mov byte [bx+0x57f4],0x1
000004F5  5D                pop bp
000004F6  CB                retf

; ---- 22A3:04F7 ----
000004F7  55                push bp
000004F8  8BEC              mov bp,sp
000004FA  9A0608A322        call 0x22a3:0x806
000004FF  A0AA37            mov al,[0x37aa]
00000502  98                cbw
00000503  8BD8              mov bx,ax
00000505  8A874E30          mov al,[bx+0x304e]
00000509  FEC0              inc al
0000050B  50                push ax
0000050C  A0AA37            mov al,[0x37aa]
0000050F  98                cbw
00000510  8BD8              mov bx,ax
00000512  58                pop ax
00000513  88874E30          mov [bx+0x304e],al
00000517  98                cbw
00000518  8B16FC08          mov dx,[0x8fc]
0000051C  4A                dec dx
0000051D  3BC2              cmp ax,dx
0000051F  7E44              jng 0x565
00000521  A0AA37            mov al,[0x37aa]
00000524  98                cbw
00000525  8BD8              mov bx,ax
00000527  C6874E3000        mov byte [bx+0x304e],0x0
0000052C  A0AA37            mov al,[0x37aa]
0000052F  98                cbw
00000530  8BD8              mov bx,ax
00000532  8A875A30          mov al,[bx+0x305a]
00000536  FEC0              inc al
00000538  50                push ax
00000539  A0AA37            mov al,[0x37aa]
0000053C  98                cbw
0000053D  8BD8              mov bx,ax
0000053F  58                pop ax
00000540  88875A30          mov [bx+0x305a],al
00000544  98                cbw
00000545  8B16FE08          mov dx,[0x8fe]
00000549  4A                dec dx
0000054A  3BC2              cmp ax,dx
0000054C  7E17              jng 0x565
0000054E  9ABA210906        call 0x609:0x21ba
00000553  FE0EE63C          dec byte [0x3ce6]
00000557  A0AA37            mov al,[0x37aa]
0000055A  98                cbw
0000055B  8BD8              mov bx,ax
0000055D  C687B24306        mov byte [bx+0x43b2],0x6
00000562  E9C700            jmp 0x62c
00000565  A0AA37            mov al,[0x37aa]
00000568  98                cbw
00000569  8BD8              mov bx,ax
0000056B  8A874E30          mov al,[bx+0x304e]
0000056F  A2B02E            mov [0x2eb0],al
00000572  A0AA37            mov al,[0x37aa]
00000575  98                cbw
00000576  8BD8              mov bx,ax
00000578  8A875A30          mov al,[bx+0x305a]
0000057C  A2002F            mov [0x2f00],al
0000057F  A0B02E            mov al,[0x2eb0]
00000582  98                cbw
00000583  BA0600            mov dx,0x6
00000586  F7EA              imul dx
00000588  50                push ax
00000589  A0002F            mov al,[0x2f00]
0000058C  98                cbw
0000058D  5B                pop bx
0000058E  03D8              add bx,ax
00000590  8A874244          mov al,[bx+0x4442]
00000594  3A06AA37          cmp al,[0x37aa]
00000598  7403              jz 0x59d
0000059A  E962FF            jmp 0x4ff
0000059D  A0B02E            mov al,[0x2eb0]
000005A0  98                cbw
000005A1  BA1A00            mov dx,0x1a
000005A4  F7EA              imul dx
000005A6  50                push ax
000005A7  A0002F            mov al,[0x2f00]
000005AA  98                cbw
000005AB  5B                pop bx
000005AC  03D8              add bx,ax
000005AE  80BFF44800        cmp byte [bx+0x48f4],0x0
000005B3  7503              jnz 0x5b8
000005B5  E947FF            jmp 0x4ff
000005B8  A0AA37            mov al,[0x37aa]
000005BB  98                cbw
000005BC  8BD8              mov bx,ax
000005BE  8A874E30          mov al,[bx+0x304e]
000005C2  98                cbw
000005C3  B105              mov cl,0x5
000005C5  D3E0              shl ax,cl
000005C7  8B161E45          mov dx,[0x451e]
000005CB  03D0              add dx,ax
000005CD  83C208            add dx,byte +0x8
000005D0  A0AA37            mov al,[0x37aa]
000005D3  98                cbw
000005D4  BB1A00            mov bx,0x1a
000005D7  52                push dx
000005D8  F7EB              imul bx
000005DA  8BD8              mov bx,ax
000005DC  58                pop ax
000005DD  89872033          mov [bx+0x3320],ax
000005E1  A0AA37            mov al,[0x37aa]
000005E4  98                cbw
000005E5  8BD8              mov bx,ax
000005E7  8A875A30          mov al,[bx+0x305a]
000005EB  98                cbw
000005EC  BA1600            mov dx,0x16
000005EF  F7EA              imul dx
000005F1  8B162845          mov dx,[0x4528]
000005F5  03D0              add dx,ax
000005F7  83C20C            add dx,byte +0xc
000005FA  A0AA37            mov al,[0x37aa]
000005FD  98                cbw
000005FE  BB1A00            mov bx,0x1a
00000601  52                push dx
00000602  F7EB              imul bx
00000604  8BD8              mov bx,ax
00000606  58                pop ax
00000607  89872233          mov [bx+0x3322],ax
0000060B  A0AA37            mov al,[0x37aa]
0000060E  98                cbw
0000060F  D1E0              shl ax,1
00000611  D1E0              shl ax,1
00000613  8BD8              mov bx,ax
00000615  C7878637D007      mov word [bx+0x3786],0x7d0
0000061B  C78788370000      mov word [bx+0x3788],0x0
00000621  A0AA37            mov al,[0x37aa]
00000624  98                cbw
00000625  8BD8              mov bx,ax
00000627  C687B24308        mov byte [bx+0x43b2],0x8
0000062C  5D                pop bp
0000062D  CB                retf

; ---- 22A3:062E ----
0000062E  55                push bp
0000062F  8BEC              mov bp,sp
00000631  0E                push cs
00000632  E8A5FE            call 0x4da
00000635  A0AA37            mov al,[0x37aa]
00000638  98                cbw
00000639  8BD8              mov bx,ax
0000063B  8A874E30          mov al,[bx+0x304e]
0000063F  A2B02E            mov [0x2eb0],al
00000642  A0AA37            mov al,[0x37aa]
00000645  98                cbw
00000646  8BD8              mov bx,ax
00000648  8A875A30          mov al,[bx+0x305a]
0000064C  A2002F            mov [0x2f00],al
0000064F  A0B02E            mov al,[0x2eb0]
00000652  98                cbw
00000653  BA0600            mov dx,0x6
00000656  F7EA              imul dx
00000658  50                push ax
00000659  A0002F            mov al,[0x2f00]
0000065C  98                cbw
0000065D  5B                pop bx
0000065E  03D8              add bx,ax
00000660  8A87B12E          mov al,[bx+0x2eb1]
00000664  98                cbw
00000665  A3D045            mov [0x45d0],ax
00000668  A0AA37            mov al,[0x37aa]
0000066B  98                cbw
0000066C  D1E0              shl ax,1
0000066E  8BD8              mov bx,ax
00000670  8B87E857          mov ax,[bx+0x57e8]
00000674  40                inc ax
00000675  50                push ax
00000676  A0AA37            mov al,[0x37aa]
00000679  98                cbw
0000067A  D1E0              shl ax,1
0000067C  8BD8              mov bx,ax
0000067E  58                pop ax
0000067F  8987E857          mov [bx+0x57e8],ax
00000683  50                push ax
00000684  A0B02E            mov al,[0x2eb0]
00000687  98                cbw
00000688  BA1A00            mov dx,0x1a
0000068B  F7EA              imul dx
0000068D  50                push ax
0000068E  A0002F            mov al,[0x2f00]
00000691  98                cbw
00000692  5B                pop bx
00000693  03D8              add bx,ax
00000695  8A87F448          mov al,[bx+0x48f4]
00000699  B400              mov ah,0x0
0000069B  BB0200            mov bx,0x2
0000069E  99                cwd
0000069F  F7FB              idiv bx
000006A1  5A                pop dx
000006A2  3BD0              cmp dx,ax
000006A4  7F38              jg 0x6de
000006A6  9AC207A322        call 0x22a3:0x7c2
000006AB  A0AA37            mov al,[0x37aa]
000006AE  98                cbw
000006AF  D1E0              shl ax,1
000006B1  D1E0              shl ax,1
000006B3  D1E0              shl ax,1
000006B5  8B16D045          mov dx,[0x45d0]
000006B9  D1E2              shl dx,1
000006BB  03C2              add ax,dx
000006BD  8BD8              mov bx,ax
000006BF  8B87F43C          mov ax,[bx+0x3cf4]
000006C3  50                push ax
000006C4  A0AA37            mov al,[0x37aa]
000006C7  98                cbw
000006C8  D1E0              shl ax,1
000006CA  8BD8              mov bx,ax
000006CC  8B87E857          mov ax,[bx+0x57e8]
000006D0  D1E0              shl ax,1
000006D2  5A                pop dx
000006D3  03D0              add dx,ax
000006D5  52                push dx
000006D6  9A2F08A322        call 0x22a3:0x82f
000006DB  59                pop cx
000006DC  EB70              jmp short 0x74e
000006DE  A0AA37            mov al,[0x37aa]
000006E1  98                cbw
000006E2  D1E0              shl ax,1
000006E4  D1E0              shl ax,1
000006E6  D1E0              shl ax,1
000006E8  8B16D045          mov dx,[0x45d0]
000006EC  D1E2              shl dx,1
000006EE  03C2              add ax,dx
000006F0  50                push ax
000006F1  A0B02E            mov al,[0x2eb0]
000006F4  98                cbw
000006F5  BA1A00            mov dx,0x1a
000006F8  F7EA              imul dx
000006FA  50                push ax
000006FB  A0002F            mov al,[0x2f00]
000006FE  98                cbw
000006FF  5B                pop bx
00000700  03D8              add bx,ax
00000702  8A87F448          mov al,[bx+0x48f4]
00000706  B400              mov ah,0x0
00000708  5B                pop bx
00000709  0187F43C          add [bx+0x3cf4],ax
0000070D  8B87F43C          mov ax,[bx+0x3cf4]
00000711  3DE703            cmp ax,0x3e7
00000714  7E1A              jng 0x730
00000716  A0AA37            mov al,[0x37aa]
00000719  98                cbw
0000071A  D1E0              shl ax,1
0000071C  D1E0              shl ax,1
0000071E  D1E0              shl ax,1
00000720  8B16D045          mov dx,[0x45d0]
00000724  D1E2              shl dx,1
00000726  03C2              add ax,dx
00000728  8BD8              mov bx,ax
0000072A  C787F43CE703      mov word [bx+0x3cf4],0x3e7
00000730  9A7408A322        call 0x22a3:0x874
00000735  A0AA37            mov al,[0x37aa]
00000738  98                cbw
00000739  D1E0              shl ax,1
0000073B  8BD8              mov bx,ax
0000073D  C787E8570000      mov word [bx+0x57e8],0x0
00000743  A0AA37            mov al,[0x37aa]
00000746  98                cbw
00000747  8BD8              mov bx,ax
00000749  C687B24307        mov byte [bx+0x43b2],0x7
0000074E  5D                pop bp
0000074F  CB                retf

; ---- 22A3:0750 ----
00000750  55                push bp
00000751  8BEC              mov bp,sp
00000753  56                push si
00000754  57                push di
00000755  C606AA3700        mov byte [0x37aa],0x0
0000075A  EB5B              jmp short 0x7b7
0000075C  33FF              xor di,di
0000075E  33F6              xor si,si
00000760  EB12              jmp short 0x774
00000762  A0AA37            mov al,[0x37aa]
00000765  98                cbw
00000766  D1E0              shl ax,1
00000768  D1E0              shl ax,1
0000076A  8BD8              mov bx,ax
0000076C  8A80E72E          mov al,[bx+si+0x2ee7]
00000770  98                cbw
00000771  03F8              add di,ax
00000773  46                inc si
00000774  83FE02            cmp si,byte +0x2
00000777  7EE9              jng 0x762
00000779  33F6              xor si,si
0000077B  EB31              jmp short 0x7ae
0000077D  0BFF              or di,di
0000077F  7504              jnz 0x785
00000781  B000              mov al,0x0
00000783  EB17              jmp short 0x79c
00000785  A0AA37            mov al,[0x37aa]
00000788  98                cbw
00000789  D1E0              shl ax,1
0000078B  D1E0              shl ax,1
0000078D  8BD8              mov bx,ax
0000078F  8A80E72E          mov al,[bx+si+0x2ee7]
00000793  98                cbw
00000794  BA6400            mov dx,0x64
00000797  F7EA              imul dx
00000799  99                cwd
0000079A  F7FF              idiv di
0000079C  50                push ax
0000079D  A0AA37            mov al,[0x37aa]
000007A0  98                cbw
000007A1  BA0300            mov dx,0x3
000007A4  F7EA              imul dx
000007A6  8BD8              mov bx,ax
000007A8  58                pop ax
000007A9  88802C34          mov [bx+si+0x342c],al
000007AD  46                inc si
000007AE  83FE02            cmp si,byte +0x2
000007B1  7ECA              jng 0x77d
000007B3  FE06AA37          inc byte [0x37aa]
000007B7  803EAA3704        cmp byte [0x37aa],0x4
000007BC  7C9E              jl 0x75c
000007BE  5F                pop di
000007BF  5E                pop si
000007C0  5D                pop bp
000007C1  CB                retf

; ---- 22A3:07C2 ----
000007C2  55                push bp
000007C3  8BEC              mov bp,sp
000007C5  56                push si
000007C6  57                push di
000007C7  A0AA37            mov al,[0x37aa]
000007CA  98                cbw
000007CB  B106              mov cl,0x6
000007CD  D3E0              shl ax,cl
000007CF  054600            add ax,0x46
000007D2  8BF8              mov di,ax
000007D4  A0AA37            mov al,[0x37aa]
000007D7  98                cbw
000007D8  D1E0              shl ax,1
000007DA  BA2A00            mov dx,0x2a
000007DD  8BD8              mov bx,ax
000007DF  2B97E857          sub dx,[bx+0x57e8]
000007E3  8BF2              mov si,dx
000007E5  A0AA37            mov al,[0x37aa]
000007E8  98                cbw
000007E9  D1E0              shl ax,1
000007EB  8BD8              mov bx,ax
000007ED  FFB7C800          push word [bx+0xc8]
000007F1  56                push si
000007F2  8BC7              mov ax,di
000007F4  050300            add ax,0x3
000007F7  50                push ax
000007F8  56                push si
000007F9  57                push di
000007FA  9A3304960A        call 0xa96:0x433
000007FF  83C40A            add sp,byte +0xa
00000802  5F                pop di
00000803  5E                pop si
00000804  5D                pop bp
00000805  CB                retf

; ---- 22A3:0806 ----
00000806  55                push bp
00000807  8BEC              mov bp,sp
00000809  B80500            mov ax,0x5
0000080C  50                push ax
0000080D  B81E00            mov ax,0x1e
00000810  50                push ax
00000811  B80400            mov ax,0x4
00000814  50                push ax
00000815  B80C00            mov ax,0xc
00000818  50                push ax
00000819  A0AA37            mov al,[0x37aa]
0000081C  98                cbw
0000081D  B106              mov cl,0x6
0000081F  D3E0              shl ax,cl
00000821  054600            add ax,0x46
00000824  50                push ax
00000825  9AD704960A        call 0xa96:0x4d7
0000082A  83C40A            add sp,byte +0xa
0000082D  5D                pop bp
0000082E  CB                retf

; ---- 22A3:082F ----
0000082F  55                push bp
00000830  8BEC              mov bp,sp
00000832  56                push si
00000833  8B7606            mov si,[bp+0x6]
00000836  81FEE703          cmp si,0x3e7
0000083A  7E03              jng 0x83f
0000083C  BEE703            mov si,0x3e7
0000083F  B80500            mov ax,0x5
00000842  50                push ax
00000843  B80200            mov ax,0x2
00000846  50                push ax
00000847  B80300            mov ax,0x3
0000084A  50                push ax
0000084B  8BC6              mov ax,si
0000084D  99                cwd
0000084E  52                push dx
0000084F  50                push ax
00000850  A1D045            mov ax,[0x45d0]
00000853  D1E0              shl ax,1
00000855  D1E0              shl ax,1
00000857  D1E0              shl ax,1
00000859  051200            add ax,0x12
0000085C  50                push ax
0000085D  A0AA37            mov al,[0x37aa]
00000860  98                cbw
00000861  B106              mov cl,0x6
00000863  D3E0              shl ax,cl
00000865  053200            add ax,0x32
00000868  50                push ax
00000869  9A7A05960A        call 0xa96:0x57a
0000086E  83C40E            add sp,byte +0xe
00000871  5E                pop si
00000872  5D                pop bp
00000873  CB                retf

; ---- 22A3:0874 ----
00000874  55                push bp
00000875  8BEC              mov bp,sp
00000877  B80500            mov ax,0x5
0000087A  50                push ax
0000087B  B80D00            mov ax,0xd
0000087E  50                push ax
0000087F  B80300            mov ax,0x3
00000882  50                push ax
00000883  A0AA37            mov al,[0x37aa]
00000886  98                cbw
00000887  D1E0              shl ax,1
00000889  D1E0              shl ax,1
0000088B  D1E0              shl ax,1
0000088D  8B16D045          mov dx,[0x45d0]
00000891  D1E2              shl dx,1
00000893  03C2              add ax,dx
00000895  8BD8              mov bx,ax
00000897  8B87F43C          mov ax,[bx+0x3cf4]
0000089B  99                cwd
0000089C  52                push dx
0000089D  50                push ax
0000089E  A1D045            mov ax,[0x45d0]
000008A1  D1E0              shl ax,1
000008A3  D1E0              shl ax,1
000008A5  D1E0              shl ax,1
000008A7  051200            add ax,0x12
000008AA  50                push ax
000008AB  A0AA37            mov al,[0x37aa]
000008AE  98                cbw
000008AF  B106              mov cl,0x6
000008B1  D3E0              shl ax,cl
000008B3  053200            add ax,0x32
000008B6  50                push ax
000008B7  9A7A05960A        call 0xa96:0x57a
000008BC  83C40E            add sp,byte +0xe
000008BF  5D                pop bp
000008C0  CB                retf

; ---- 22A3:08C1 ----
000008C1  55                push bp
000008C2  8BEC              mov bp,sp
000008C4  B80100            mov ax,0x1
000008C7  50                push ax
000008C8  9ADB05FD21        call 0x21fd:0x5db
000008CD  59                pop cx
000008CE  B80500            mov ax,0x5
000008D1  50                push ax
000008D2  B80900            mov ax,0x9
000008D5  50                push ax
000008D6  B80300            mov ax,0x3
000008D9  50                push ax
000008DA  1E                push ds
000008DB  A0E640            mov al,[0x40e6]
000008DE  98                cbw
000008DF  BA0300            mov dx,0x3
000008E2  F7EA              imul dx
000008E4  D1E0              shl ax,1
000008E6  D1E0              shl ax,1
000008E8  05A43B            add ax,0x3ba4
000008EB  50                push ax
000008EC  B84001            mov ax,0x140
000008EF  50                push ax
000008F0  B81400            mov ax,0x14
000008F3  50                push ax
000008F4  33C0              xor ax,ax
000008F6  50                push ax
000008F7  9A8802690A        call 0xa69:0x288
000008FC  83C410            add sp,byte +0x10
000008FF  33C0              xor ax,ax
00000901  50                push ax
00000902  9ADB05FD21        call 0x21fd:0x5db
00000907  59                pop cx
00000908  33C0              xor ax,ax
0000090A  BA5000            mov dx,0x50
0000090D  50                push ax
0000090E  52                push dx
0000090F  9A8501FD21        call 0x21fd:0x185
00000914  59                pop cx
00000915  59                pop cx
00000916  9A1B00960A        call 0xa96:0x1b
0000091B  B82B00            mov ax,0x2b
0000091E  50                push ax
0000091F  B82001            mov ax,0x120
00000922  50                push ax
00000923  B80600            mov ax,0x6
00000926  50                push ax
00000927  B81000            mov ax,0x10
0000092A  50                push ax
0000092B  9A041F0906        call 0x609:0x1f04
