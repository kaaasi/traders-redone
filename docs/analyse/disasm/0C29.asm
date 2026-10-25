; segment 0C29  file offset 0xF890  size 0x14A0
00000000  CB                retf

; ---- 0C29:0001 ----
00000001  55                push bp
00000002  8BEC              mov bp,sp
00000004  8B4606            mov ax,[bp+0x6]
00000007  A3D045            mov [0x45d0],ax
0000000A  8A4608            mov al,[bp+0x8]
0000000D  A2C52B            mov [0x2bc5],al
00000010  837E0800          cmp word [bp+0x8],byte +0x0
00000014  750C              jnz 0x22
00000016  FF36D608          push word [0x8d6]
0000001A  9A02006C18        call 0x186c:0x2
0000001F  59                pop cx
00000020  EB0A              jmp short 0x2c
00000022  FF36D808          push word [0x8d8]
00000026  9A02006C18        call 0x186c:0x2
0000002B  59                pop cx
0000002C  B80200            mov ax,0x2
0000002F  50                push ax
00000030  9ADB05FD21        call 0x21fd:0x5db
00000035  59                pop cx
00000036  9A4600290C        call 0xc29:0x46
0000003B  33C0              xor ax,ax
0000003D  50                push ax
0000003E  9ADB05FD21        call 0x21fd:0x5db
00000043  59                pop cx
00000044  5D                pop bp
00000045  CB                retf

; ---- 0C29:0046 ----
00000046  55                push bp
00000047  8BEC              mov bp,sp
00000049  C606463D0E        mov byte [0x3d46],0xe
0000004E  9A5C009F0B        call 0xb9f:0x5c
00000053  9A9E009F0B        call 0xb9f:0x9e
00000058  9A8600290C        call 0xc29:0x86
0000005D  9A1B00960A        call 0xa96:0x1b
00000062  9A5C01290C        call 0xc29:0x15c
00000067  9AE1019F0B        call 0xb9f:0x1e1
0000006C  833EBC2B00        cmp word [0x2bbc],byte +0x0
00000071  7502              jnz 0x75
00000073  EB0F              jmp short 0x84
00000075  9A64041D14        call 0x141d:0x464
0000007A  9AB701290C        call 0xc29:0x1b7
0000007F  9A8A079F0B        call 0xb9f:0x78a
00000084  5D                pop bp
00000085  CB                retf

; ---- 0C29:0086 ----
00000086  55                push bp
00000087  8BEC              mov bp,sp
00000089  C606AA3700        mov byte [0x37aa],0x0
0000008E  EB0F              jmp short 0x9f
00000090  A0AA37            mov al,[0x37aa]
00000093  98                cbw
00000094  8BD8              mov bx,ax
00000096  C687B24301        mov byte [bx+0x43b2],0x1
0000009B  FE06AA37          inc byte [0x37aa]
0000009F  803EAA3704        cmp byte [0x37aa],0x4
000000A4  7CEA              jl 0x90
000000A6  9A1A05290C        call 0xc29:0x51a
000000AB  9A0C089F0B        call 0xb9f:0x80c
000000B0  9A3506290C        call 0xc29:0x635
000000B5  C606AA3700        mov byte [0x37aa],0x0
000000BA  EB3B              jmp short 0xf7
000000BC  9A9F10290C        call 0xc29:0x109f
000000C1  9A1B00960A        call 0xa96:0x1b
000000C6  9ABB09290C        call 0xc29:0x9bb
000000CB  9A600B290C        call 0xc29:0xb60
000000D0  9A1A021D14        call 0x141d:0x21a
000000D5  A0AA37            mov al,[0x37aa]
000000D8  98                cbw
000000D9  8BD8              mov bx,ax
000000DB  C687B24300        mov byte [bx+0x43b2],0x0
000000E0  A0AA37            mov al,[0x37aa]
000000E3  98                cbw
000000E4  D1E0              shl ax,1
000000E6  8BD8              mov bx,ax
000000E8  C787A243FDFF      mov word [bx+0x43a2],0xfffd
000000EE  9AA806960A        call 0xa96:0x6a8
000000F3  FE06AA37          inc byte [0x37aa]
000000F7  803EAA3704        cmp byte [0x37aa],0x4
000000FC  7CBE              jl 0xbc
000000FE  9A010E290C        call 0xc29:0xe01
00000103  9A1B00960A        call 0xa96:0x1b
00000108  C7061C3D0000      mov word [0x3d1c],0x0
0000010E  C706EE430A00      mov word [0x43ee],0xa
00000114  B81400            mov ax,0x14
00000117  50                push ax
00000118  A18E2E            mov ax,[0x2e8e]
0000011B  BA0500            mov dx,0x5
0000011E  F7EA              imul dx
00000120  BA1900            mov dx,0x19
00000123  2BD0              sub dx,ax
00000125  52                push dx
00000126  9AE504FD21        call 0x21fd:0x4e5
0000012B  59                pop cx
0000012C  59                pop cx
0000012D  9AC10C290C        call 0xc29:0xcc1
00000132  9AA806960A        call 0xa96:0x6a8
00000137  FF362E08          push word [0x82e]
0000013B  9A3F0C960A        call 0xa96:0xc3f
00000140  59                pop cx
00000141  B84D00            mov ax,0x4d
00000144  50                push ax
00000145  B80200            mov ax,0x2
00000148  50                push ax
00000149  9A5400A721        call 0x21a7:0x54
0000014E  59                pop cx
0000014F  59                pop cx
00000150  9ABD04290C        call 0xc29:0x4bd
00000155  9A2903FD21        call 0x21fd:0x329
0000015A  5D                pop bp
0000015B  CB                retf

; ---- 0C29:015C ----
0000015C  55                push bp
0000015D  8BEC              mov bp,sp
0000015F  EB2C              jmp short 0x18d
00000161  9A1F05FD21        call 0x21fd:0x51f
00000166  9AA806960A        call 0xa96:0x6a8
0000016B  9A1B00960A        call 0xa96:0x1b
00000170  9AFE009F0B        call 0xb9f:0xfe
00000175  9AF904FD21        call 0x21fd:0x4f9
0000017A  3C01              cmp al,0x1
0000017C  7505              jnz 0x183
0000017E  9A6A0D290C        call 0xc29:0xd6a
00000183  9AA806960A        call 0xa96:0x6a8
00000188  9A010E290C        call 0xc29:0xe01
0000018D  833E803700        cmp word [0x3780],byte +0x0
00000192  75CD              jnz 0x161
00000194  9A6506FD21        call 0x21fd:0x665
00000199  33C0              xor ax,ax
0000019B  50                push ax
0000019C  B80200            mov ax,0x2
0000019F  50                push ax
000001A0  9A5400A721        call 0x21a7:0x54
000001A5  59                pop cx
000001A6  59                pop cx
000001A7  33C0              xor ax,ax
000001A9  BA1900            mov dx,0x19
000001AC  50                push ax
000001AD  52                push dx
000001AE  9A8501FD21        call 0x21fd:0x185
000001B3  59                pop cx
000001B4  59                pop cx
000001B5  5D                pop bp
000001B6  CB                retf

; ---- 0C29:01B7 ----
000001B7  55                push bp
000001B8  8BEC              mov bp,sp
000001BA  9A1A05290C        call 0xc29:0x51a
000001BF  33C0              xor ax,ax
000001C1  50                push ax
000001C2  9AB403FD21        call 0x21fd:0x3b4
000001C7  59                pop cx
000001C8  9A3506290C        call 0xc29:0x635
000001CD  C606AA3700        mov byte [0x37aa],0x0
000001D2  EB1D              jmp short 0x1f1
000001D4  9A9F10290C        call 0xc29:0x109f
000001D9  9A1B00960A        call 0xa96:0x1b
000001DE  9ABB09290C        call 0xc29:0x9bb
000001E3  9A600B290C        call 0xc29:0xb60
000001E8  9AA806960A        call 0xa96:0x6a8
000001ED  FE06AA37          inc byte [0x37aa]
000001F1  803EAA3704        cmp byte [0x37aa],0x4
000001F6  7CDC              jl 0x1d4
000001F8  9A010E290C        call 0xc29:0xe01
000001FD  9A1B00960A        call 0xa96:0x1b
00000202  B81E00            mov ax,0x1e
00000205  50                push ax
00000206  9A4A029F0B        call 0xb9f:0x24a
0000020B  50                push ax
0000020C  9AE504FD21        call 0x21fd:0x4e5
00000211  59                pop cx
00000212  59                pop cx
00000213  9AC10C290C        call 0xc29:0xcc1
00000218  9AA806960A        call 0xa96:0x6a8
0000021D  FF362E08          push word [0x82e]
00000221  9A3F0C960A        call 0xa96:0xc3f
00000226  59                pop cx
00000227  9A5303FD21        call 0x21fd:0x353
0000022C  B8FFFF            mov ax,0xffff
0000022F  A30641            mov [0x4106],ax
00000232  A3F244            mov [0x44f2],ax
00000235  C7061C3D0000      mov word [0x3d1c],0x0
0000023B  C706EE43B400      mov word [0x43ee],0xb4
00000241  B84D00            mov ax,0x4d
00000244  50                push ax
00000245  B80200            mov ax,0x2
00000248  50                push ax
00000249  9A5400A721        call 0x21a7:0x54
0000024E  59                pop cx
0000024F  59                pop cx
00000250  9A1B00960A        call 0xa96:0x1b
00000255  EB7C              jmp short 0x2d3
00000257  9A1F05FD21        call 0x21fd:0x51f
0000025C  9AA806960A        call 0xa96:0x6a8
00000261  9A7A039E0E        call 0xe9e:0x37a
00000266  9ABE029F0B        call 0xb9f:0x2be
0000026B  C606C42B00        mov byte [0x2bc4],0x0
00000270  C606AA3700        mov byte [0x37aa],0x0
00000275  EB1B              jmp short 0x292
00000277  A0AA37            mov al,[0x37aa]
0000027A  98                cbw
0000027B  8BD8              mov bx,ax
0000027D  80BF400801        cmp byte [bx+0x840],0x1
00000282  7505              jnz 0x289
00000284  9AF3091D14        call 0x141d:0x9f3
00000289  9A21039F0B        call 0xb9f:0x321
0000028E  FE06AA37          inc byte [0x37aa]
00000292  803EAA3704        cmp byte [0x37aa],0x4
00000297  7CDE              jl 0x277
00000299  9A010E290C        call 0xc29:0xe01
0000029E  9A49059F0B        call 0xb9f:0x549
000002A3  9A1B00960A        call 0xa96:0x1b
000002A8  9AB90F290C        call 0xc29:0xfb9
000002AD  A15A45            mov ax,[0x455a]
000002B0  3B06E42B          cmp ax,[0x2be4]
000002B4  7405              jz 0x2bb
000002B6  9A1F0B290C        call 0xc29:0xb1f
000002BB  9AF904FD21        call 0x21fd:0x4f9
000002C0  3C01              cmp al,0x1
000002C2  7505              jnz 0x2c9
000002C4  9A6A0D290C        call 0xc29:0xd6a
000002C9  9A2303290C        call 0xc29:0x323
000002CE  9AA806960A        call 0xa96:0x6a8
000002D3  833E803700        cmp word [0x3780],byte +0x0
000002D8  740A              jz 0x2e4
000002DA  833EBC2B00        cmp word [0x2bbc],byte +0x0
000002DF  7403              jz 0x2e4
000002E1  E973FF            jmp 0x257
000002E4  803EAE3C00        cmp byte [0x3cae],0x0
000002E9  7403              jz 0x2ee
000002EB  E969FF            jmp 0x257
000002EE  9A6506FD21        call 0x21fd:0x665
000002F3  33C0              xor ax,ax
000002F5  50                push ax
000002F6  B80200            mov ax,0x2
000002F9  50                push ax
000002FA  9A5400A721        call 0x21a7:0x54
000002FF  59                pop cx
00000300  59                pop cx
00000301  33C0              xor ax,ax
00000303  50                push ax
00000304  33C0              xor ax,ax
00000306  50                push ax
00000307  9A5400A721        call 0x21a7:0x54
0000030C  59                pop cx
0000030D  59                pop cx
0000030E  33C0              xor ax,ax
00000310  BA1900            mov dx,0x19
00000313  50                push ax
00000314  52                push dx
00000315  9A8501FD21        call 0x21fd:0x185
0000031A  59                pop cx
0000031B  59                pop cx
0000031C  C606CC2B00        mov byte [0x2bcc],0x0
00000321  5D                pop bp
00000322  CB                retf

; ---- 0C29:0323 ----
00000323  55                push bp
00000324  8BEC              mov bp,sp
00000326  803EAE3C00        cmp byte [0x3cae],0x0
0000032B  7E2F              jng 0x35c
0000032D  A0AE3C            mov al,[0x3cae]
00000330  FEC8              dec al
00000332  A2AE3C            mov [0x3cae],al
00000335  3C02              cmp al,0x2
00000337  7505              jnz 0x33e
00000339  C606CC2B00        mov byte [0x2bcc],0x0
0000033E  A0AE3C            mov al,[0x3cae]
00000341  98                cbw
00000342  50                push ax
00000343  A0463D            mov al,[0x3d46]
00000346  98                cbw
00000347  05FEFF            add ax,0xfffe
0000034A  5A                pop dx
0000034B  3BD0              cmp dx,ax
0000034D  750B              jnz 0x35a
0000034F  A0F244            mov al,[0x44f2]
00000352  A2AA37            mov [0x37aa],al
00000355  9ABB09290C        call 0xc29:0x9bb
0000035A  EB54              jmp short 0x3b0
0000035C  803E183D00        cmp byte [0x3d18],0x0
00000361  7E0B              jng 0x36e
00000363  FE0E183D          dec byte [0x3d18]
00000367  C606463D0E        mov byte [0x3d46],0xe
0000036C  EB42              jmp short 0x3b0
0000036E  A11C3D            mov ax,[0x3d1c]
00000371  3B06EE43          cmp ax,[0x43ee]
00000375  7539              jnz 0x3b0
00000377  9A0A069F0B        call 0xb9f:0x60a
0000037C  8B1EF244          mov bx,[0x44f2]
00000380  8A87C608          mov al,[bx+0x8c6]
00000384  A2CC2B            mov [0x2bcc],al
00000387  833E06410A        cmp word [0x4106],byte +0xa
0000038C  740C              jz 0x39a
0000038E  8B1E0641          mov bx,[0x4106]
00000392  8A87C608          mov al,[bx+0x8c6]
00000396  0006CC2B          add [0x2bcc],al
0000039A  9AB203290C        call 0xc29:0x3b2
0000039F  803E463D05        cmp byte [0x3d46],0x5
000003A4  7E04              jng 0x3aa
000003A6  FE0E463D          dec byte [0x3d46]
000003AA  A0463D            mov al,[0x3d46]
000003AD  A2AE3C            mov [0x3cae],al
000003B0  5D                pop bp
000003B1  CB                retf

; ---- 0C29:03B2 ----
000003B2  55                push bp
000003B3  8BEC              mov bp,sp
000003B5  56                push si
000003B6  A11C3D            mov ax,[0x3d1c]
000003B9  BB0200            mov bx,0x2
000003BC  99                cwd
000003BD  F7FB              idiv bx
000003BF  03065A45          add ax,[0x455a]
000003C3  8BF0              mov si,ax
000003C5  833E06410A        cmp word [0x4106],byte +0xa
000003CA  7414              jz 0x3e0
000003CC  A10641            mov ax,[0x4106]
000003CF  051B00            add ax,0x1b
000003D2  50                push ax
000003D3  B80100            mov ax,0x1
000003D6  50                push ax
000003D7  9A5400A721        call 0x21a7:0x54
000003DC  59                pop cx
000003DD  59                pop cx
000003DE  EB0F              jmp short 0x3ef
000003E0  B81F00            mov ax,0x1f
000003E3  50                push ax
000003E4  B80100            mov ax,0x1
000003E7  50                push ax
000003E8  9A5400A721        call 0x21a7:0x54
000003ED  59                pop cx
000003EE  59                pop cx
000003EF  9A95069F0B        call 0xb9f:0x695
000003F4  8BC6              mov ax,si
000003F6  99                cwd
000003F7  8B1EF244          mov bx,[0x44f2]
000003FB  D1E3              shl bx,1
000003FD  D1E3              shl bx,1
000003FF  0187D23C          add [bx+0x3cd2],ax
00000403  1197D43C          adc [bx+0x3cd4],dx
00000407  8B1EF244          mov bx,[0x44f2]
0000040B  D1E3              shl bx,1
0000040D  D1E3              shl bx,1
0000040F  D1E3              shl bx,1
00000411  A1D045            mov ax,[0x45d0]
00000414  D1E0              shl ax,1
00000416  03D8              add bx,ax
00000418  8B87F43C          mov ax,[bx+0x3cf4]
0000041C  05F6FF            add ax,0xfff6
0000041F  8B1EF244          mov bx,[0x44f2]
00000423  D1E3              shl bx,1
00000425  D1E3              shl bx,1
00000427  D1E3              shl bx,1
00000429  8B16D045          mov dx,[0x45d0]
0000042D  D1E2              shl dx,1
0000042F  03DA              add bx,dx
00000431  8987F43C          mov [bx+0x3cf4],ax
00000435  A0F244            mov al,[0x44f2]
00000438  A2AA37            mov [0x37aa],al
0000043B  B8FFFF            mov ax,0xffff
0000043E  50                push ax
0000043F  9A0811290C        call 0xc29:0x1108
00000444  59                pop cx
00000445  9A670E1D14        call 0x141d:0xe67
0000044A  833E06410A        cmp word [0x4106],byte +0xa
0000044F  7502              jnz 0x453
00000451  EB67              jmp short 0x4ba
00000453  8BC6              mov ax,si
00000455  99                cwd
00000456  8B1E0641          mov bx,[0x4106]
0000045A  D1E3              shl bx,1
0000045C  D1E3              shl bx,1
0000045E  2987D23C          sub [bx+0x3cd2],ax
00000462  1997D43C          sbb [bx+0x3cd4],dx
00000466  8B1E0641          mov bx,[0x4106]
0000046A  D1E3              shl bx,1
0000046C  D1E3              shl bx,1
0000046E  D1E3              shl bx,1
00000470  A1D045            mov ax,[0x45d0]
00000473  D1E0              shl ax,1
00000475  03D8              add bx,ax
00000477  8B87F43C          mov ax,[bx+0x3cf4]
0000047B  050A00            add ax,0xa
0000047E  8B1E0641          mov bx,[0x4106]
00000482  D1E3              shl bx,1
00000484  D1E3              shl bx,1
00000486  D1E3              shl bx,1
00000488  8B16D045          mov dx,[0x45d0]
0000048C  D1E2              shl dx,1
0000048E  03DA              add bx,dx
00000490  8987F43C          mov [bx+0x3cf4],ax
00000494  A00641            mov al,[0x4106]
00000497  A2AA37            mov [0x37aa],al
0000049A  9ABB09290C        call 0xc29:0x9bb
0000049F  B80100            mov ax,0x1
000004A2  50                push ax
000004A3  9A0811290C        call 0xc29:0x1108
000004A8  59                pop cx
000004A9  56                push si
000004AA  9ACA0C1D14        call 0x141d:0xcca
000004AF  59                pop cx
000004B0  3B36BE2B          cmp si,[0x2bbe]
000004B4  7E04              jng 0x4ba
000004B6  8936BE2B          mov [0x2bbe],si
000004BA  5E                pop si
000004BB  5D                pop bp
000004BC  CB                retf

; ---- 0C29:04BD ----
000004BD  55                push bp
000004BE  8BEC              mov bp,sp
000004C0  56                push si
000004C1  33C0              xor ax,ax
000004C3  BA0100            mov dx,0x1
000004C6  50                push ax
000004C7  52                push dx
000004C8  9A6C02FD21        call 0x21fd:0x26c
000004CD  59                pop cx
000004CE  59                pop cx
000004CF  833ED04502        cmp word [0x45d0],byte +0x2
000004D4  751D              jnz 0x4f3
000004D6  33F6              xor si,si
000004D8  EB14              jmp short 0x4ee
000004DA  8BDE              mov bx,si
000004DC  D1E3              shl bx,1
000004DE  FFB74802          push word [bx+0x248]
000004E2  8BC6              mov ax,si
000004E4  40                inc ax
000004E5  50                push ax
000004E6  9AC902FD21        call 0x21fd:0x2c9
000004EB  59                pop cx
000004EC  59                pop cx
000004ED  46                inc si
000004EE  83FE04            cmp si,byte +0x4
000004F1  7EE7              jng 0x4da
000004F3  833ED04503        cmp word [0x45d0],byte +0x3
000004F8  751D              jnz 0x517
000004FA  33F6              xor si,si
000004FC  EB14              jmp short 0x512
000004FE  8BDE              mov bx,si
00000500  D1E3              shl bx,1
00000502  FFB75202          push word [bx+0x252]
00000506  8BC6              mov ax,si
00000508  40                inc ax
00000509  50                push ax
0000050A  9AC902FD21        call 0x21fd:0x2c9
0000050F  59                pop cx
00000510  59                pop cx
00000511  46                inc si
00000512  83FE02            cmp si,byte +0x2
00000515  7EE7              jng 0x4fe
00000517  5E                pop si
00000518  5D                pop bp
00000519  CB                retf

; ---- 0C29:051A ----
0000051A  55                push bp
0000051B  8BEC              mov bp,sp
0000051D  56                push si
0000051E  33F6              xor si,si
00000520  C606AA3700        mov byte [0x37aa],0x0
00000525  EB3C              jmp short 0x563
00000527  A0AA37            mov al,[0x37aa]
0000052A  98                cbw
0000052B  8BD8              mov bx,ax
0000052D  80BFB24300        cmp byte [bx+0x43b2],0x0
00000532  752B              jnz 0x55f
00000534  A0AA37            mov al,[0x37aa]
00000537  98                cbw
00000538  D1E0              shl ax,1
0000053A  8BD8              mov bx,ax
0000053C  89B7D02B          mov [bx+0x2bd0],si
00000540  A0AA37            mov al,[0x37aa]
00000543  98                cbw
00000544  D1E0              shl ax,1
00000546  50                push ax
00000547  A0C52B            mov al,[0x2bc5]
0000054A  98                cbw
0000054B  8BD8              mov bx,ax
0000054D  8A875802          mov al,[bx+0x258]
00000551  B400              mov ah,0x0
00000553  5B                pop bx
00000554  8987DA2B          mov [bx+0x2bda],ax
00000558  8BC6              mov ax,si
0000055A  054000            add ax,0x40
0000055D  8BF0              mov si,ax
0000055F  FE06AA37          inc byte [0x37aa]
00000563  803EAA3704        cmp byte [0x37aa],0x4
00000568  7CBD              jl 0x527
0000056A  8936CE2B          mov [0x2bce],si
0000056E  8BC6              mov ax,si
00000570  054000            add ax,0x40
00000573  8BF0              mov si,ax
00000575  C706BC2B0000      mov word [0x2bbc],0x0
0000057B  C606AA3700        mov byte [0x37aa],0x0
00000580  EB40              jmp short 0x5c2
00000582  A0AA37            mov al,[0x37aa]
00000585  98                cbw
00000586  8BD8              mov bx,ax
00000588  80BFB24301        cmp byte [bx+0x43b2],0x1
0000058D  752F              jnz 0x5be
0000058F  FF06BC2B          inc word [0x2bbc]
00000593  A0AA37            mov al,[0x37aa]
00000596  98                cbw
00000597  D1E0              shl ax,1
00000599  8BD8              mov bx,ax
0000059B  89B7D02B          mov [bx+0x2bd0],si
0000059F  A0AA37            mov al,[0x37aa]
000005A2  98                cbw
000005A3  D1E0              shl ax,1
000005A5  50                push ax
000005A6  A0C52B            mov al,[0x2bc5]
000005A9  98                cbw
000005AA  8BD8              mov bx,ax
000005AC  8A875A02          mov al,[bx+0x25a]
000005B0  B400              mov ah,0x0
000005B2  5B                pop bx
000005B3  8987DA2B          mov [bx+0x2bda],ax
000005B7  8BC6              mov ax,si
000005B9  054000            add ax,0x40
000005BC  8BF0              mov si,ax
000005BE  FE06AA37          inc byte [0x37aa]
000005C2  803EAA3704        cmp byte [0x37aa],0x4
000005C7  7CB9              jl 0x582
000005C9  803EC52B00        cmp byte [0x2bc5],0x0
000005CE  7550              jnz 0x620
000005D0  833EBC2B02        cmp word [0x2bbc],byte +0x2
000005D5  7F0E              jg 0x5e5
000005D7  C706D82B4000      mov word [0x2bd8],0x40
000005DD  C706E22B0000      mov word [0x2be2],0x0
000005E3  EB21              jmp short 0x606
000005E5  833EBC2B03        cmp word [0x2bbc],byte +0x3
000005EA  750E              jnz 0x5fa
000005EC  C706D82B0000      mov word [0x2bd8],0x0
000005F2  C706E22B0000      mov word [0x2be2],0x0
000005F8  EB0C              jmp short 0x606
000005FA  C706D82BC000      mov word [0x2bd8],0xc0
00000600  C706E22B9600      mov word [0x2be2],0x96
00000606  833EBC2B03        cmp word [0x2bbc],byte +0x3
0000060B  7508              jnz 0x615
0000060D  C706CA2BC000      mov word [0x2bca],0xc0
00000613  EB09              jmp short 0x61e
00000615  A1CE2B            mov ax,[0x2bce]
00000618  054000            add ax,0x40
0000061B  A3CA2B            mov [0x2bca],ax
0000061E  EB12              jmp short 0x632
00000620  C706D82B0001      mov word [0x2bd8],0x100
00000626  C706E22B0000      mov word [0x2be2],0x0
0000062C  A1CE2B            mov ax,[0x2bce]
0000062F  A3CA2B            mov [0x2bca],ax
00000632  5E                pop si
00000633  5D                pop bp
00000634  CB                retf

; ---- 0C29:0635 ----
00000635  55                push bp
00000636  8BEC              mov bp,sp
00000638  9A1B00960A        call 0xa96:0x1b
0000063D  803EC52B00        cmp byte [0x2bc5],0x0
00000642  7507              jnz 0x64b
00000644  9A5C06290C        call 0xc29:0x65c
00000649  EB05              jmp short 0x650
0000064B  9A3907290C        call 0xc29:0x739
00000650  9A1F0B290C        call 0xc29:0xb1f
00000655  9AA806960A        call 0xa96:0x6a8
0000065A  5D                pop bp
0000065B  CB                retf

; ---- 0C29:065C ----
0000065C  55                push bp
0000065D  8BEC              mov bp,sp
0000065F  56                push si
00000660  57                push di
00000661  C606AA3700        mov byte [0x37aa],0x0
00000666  EB5B              jmp short 0x6c3
00000668  A0AA37            mov al,[0x37aa]
0000066B  98                cbw
0000066C  D1E0              shl ax,1
0000066E  8BD8              mov bx,ax
00000670  8BB7D02B          mov si,[bx+0x2bd0]
00000674  A0AA37            mov al,[0x37aa]
00000677  98                cbw
00000678  D1E0              shl ax,1
0000067A  8BD8              mov bx,ax
0000067C  8BBFDA2B          mov di,[bx+0x2bda]
00000680  57                push di
00000681  56                push si
00000682  A0AA37            mov al,[0x37aa]
00000685  98                cbw
00000686  50                push ax
00000687  9AD90B290C        call 0xc29:0xbd9
0000068C  83C406            add sp,byte +0x6
0000068F  A0AA37            mov al,[0x37aa]
00000692  98                cbw
00000693  8BD8              mov bx,ax
00000695  80BFB24300        cmp byte [bx+0x43b2],0x0
0000069A  7512              jnz 0x6ae
0000069C  33C0              xor ax,ax
0000069E  50                push ax
0000069F  56                push si
000006A0  B80800            mov ax,0x8
000006A3  50                push ax
000006A4  9AD90B290C        call 0xc29:0xbd9
000006A9  83C406            add sp,byte +0x6
000006AC  EB11              jmp short 0x6bf
000006AE  B89600            mov ax,0x96
000006B1  50                push ax
000006B2  56                push si
000006B3  B80600            mov ax,0x6
000006B6  50                push ax
000006B7  9AD90B290C        call 0xc29:0xbd9
000006BC  83C406            add sp,byte +0x6
000006BF  FE06AA37          inc byte [0x37aa]
000006C3  803EAA3704        cmp byte [0x37aa],0x4
000006C8  7C9E              jl 0x668
000006CA  33C0              xor ax,ax
000006CC  50                push ax
000006CD  FF36CE2B          push word [0x2bce]
000006D1  B80400            mov ax,0x4
000006D4  50                push ax
000006D5  9AD90B290C        call 0xc29:0xbd9
000006DA  83C406            add sp,byte +0x6
000006DD  FF36E22B          push word [0x2be2]
000006E1  FF36D82B          push word [0x2bd8]
000006E5  B80900            mov ax,0x9
000006E8  50                push ax
000006E9  9AD90B290C        call 0xc29:0xbd9
000006EE  83C406            add sp,byte +0x6
000006F1  FF36E22B          push word [0x2be2]
000006F5  FF36D82B          push word [0x2bd8]
000006F9  9A5010290C        call 0xc29:0x1050
000006FE  59                pop cx
000006FF  59                pop cx
00000700  B89600            mov ax,0x96
00000703  50                push ax
00000704  FF36CA2B          push word [0x2bca]
00000708  B80500            mov ax,0x5
0000070B  50                push ax
0000070C  9AD90B290C        call 0xc29:0xbd9
00000711  83C406            add sp,byte +0x6
00000714  833ECE2B00        cmp word [0x2bce],byte +0x0
00000719  7E0D              jng 0x728
0000071B  A1CE2B            mov ax,[0x2bce]
0000071E  05FCFF            add ax,0xfffc
00000721  50                push ax
00000722  9A8A0C290C        call 0xc29:0xc8a
00000727  59                pop cx
00000728  A1CE2B            mov ax,[0x2bce]
0000072B  054000            add ax,0x40
0000072E  50                push ax
0000072F  9A8A0C290C        call 0xc29:0xc8a
00000734  59                pop cx
00000735  5F                pop di
00000736  5E                pop si
00000737  5D                pop bp
00000738  CB                retf

; ---- 0C29:0739 ----
00000739  55                push bp
0000073A  8BEC              mov bp,sp
0000073C  56                push si
0000073D  C606AA3700        mov byte [0x37aa],0x0
00000742  E9E101            jmp 0x926
00000745  A0AA37            mov al,[0x37aa]
00000748  98                cbw
00000749  D1E0              shl ax,1
0000074B  8BD8              mov bx,ax
0000074D  8BB7D02B          mov si,[bx+0x2bd0]
00000751  A0AA37            mov al,[0x37aa]
00000754  98                cbw
00000755  8BD8              mov bx,ax
00000757  80BFB24300        cmp byte [bx+0x43b2],0x0
0000075C  7403              jz 0x761
0000075E  E9E000            jmp 0x841
00000761  33C0              xor ax,ax
00000763  50                push ax
00000764  56                push si
00000765  B85700            mov ax,0x57
00000768  50                push ax
00000769  B84000            mov ax,0x40
0000076C  50                push ax
0000076D  B82C00            mov ax,0x2c
00000770  50                push ax
00000771  33C0              xor ax,ax
00000773  50                push ax
00000774  9A3000960A        call 0xa96:0x30
00000779  83C40C            add sp,byte +0xc
0000077C  B81300            mov ax,0x13
0000077F  50                push ax
00000780  8BC6              mov ax,si
00000782  050900            add ax,0x9
00000785  50                push ax
00000786  B82E00            mov ax,0x2e
00000789  50                push ax
0000078A  B83000            mov ax,0x30
0000078D  50                push ax
0000078E  A0AA37            mov al,[0x37aa]
00000791  98                cbw
00000792  BB0200            mov bx,0x2
00000795  99                cwd
00000796  F7FB              idiv bx
00000798  BA2E00            mov dx,0x2e
0000079B  F7EA              imul dx
0000079D  053B00            add ax,0x3b
000007A0  50                push ax
000007A1  A0AA37            mov al,[0x37aa]
000007A4  98                cbw
000007A5  BB0200            mov bx,0x2
000007A8  99                cwd
000007A9  F7FB              idiv bx
000007AB  BB3000            mov bx,0x30
000007AE  8BC2              mov ax,dx
000007B0  F7EB              imul bx
000007B2  05C000            add ax,0xc0
000007B5  50                push ax
000007B6  9A0E02960A        call 0xa96:0x20e
000007BB  83C40C            add sp,byte +0xc
000007BE  33C0              xor ax,ax
000007C0  50                push ax
000007C1  B82C00            mov ax,0x2c
000007C4  50                push ax
000007C5  B84000            mov ax,0x40
000007C8  50                push ax
000007C9  B85700            mov ax,0x57
000007CC  50                push ax
000007CD  56                push si
000007CE  9AD704960A        call 0xa96:0x4d7
000007D3  83C40A            add sp,byte +0xa
000007D6  B85700            mov ax,0x57
000007D9  50                push ax
000007DA  8BC6              mov ax,si
000007DC  050800            add ax,0x8
000007DF  50                push ax
000007E0  B82C00            mov ax,0x2c
000007E3  50                push ax
000007E4  B83000            mov ax,0x30
000007E7  50                push ax
000007E8  33C0              xor ax,ax
000007EA  50                push ax
000007EB  A0AA37            mov al,[0x37aa]
000007EE  98                cbw
000007EF  BA3000            mov dx,0x30
000007F2  F7EA              imul dx
000007F4  50                push ax
000007F5  9A3000960A        call 0xa96:0x30
000007FA  83C40C            add sp,byte +0xc
000007FD  0BF6              or si,si
000007FF  7520              jnz 0x821
00000801  B88300            mov ax,0x83
00000804  50                push ax
00000805  33C0              xor ax,ax
00000807  50                push ax
00000808  B84500            mov ax,0x45
0000080B  50                push ax
0000080C  B84000            mov ax,0x40
0000080F  50                push ax
00000810  B88300            mov ax,0x83
00000813  50                push ax
00000814  33C0              xor ax,ax
00000816  50                push ax
00000817  9A3000960A        call 0xa96:0x30
0000081C  83C40C            add sp,byte +0xc
0000081F  EB1D              jmp short 0x83e
00000821  B88300            mov ax,0x83
00000824  50                push ax
00000825  56                push si
00000826  B84500            mov ax,0x45
00000829  50                push ax
0000082A  B84000            mov ax,0x40
0000082D  50                push ax
0000082E  B88300            mov ax,0x83
00000831  50                push ax
00000832  B84000            mov ax,0x40
00000835  50                push ax
00000836  9A3000960A        call 0xa96:0x30
0000083B  83C40C            add sp,byte +0xc
0000083E  E9E100            jmp 0x922
00000841  B86300            mov ax,0x63
00000844  50                push ax
00000845  56                push si
00000846  B86500            mov ax,0x65
00000849  50                push ax
0000084A  B84000            mov ax,0x40
0000084D  50                push ax
0000084E  B86300            mov ax,0x63
00000851  50                push ax
00000852  B88000            mov ax,0x80
00000855  50                push ax
00000856  9A3000960A        call 0xa96:0x30
0000085B  83C40C            add sp,byte +0xc
0000085E  B87C00            mov ax,0x7c
00000861  50                push ax
00000862  8BC6              mov ax,si
00000864  050900            add ax,0x9
00000867  50                push ax
00000868  B82E00            mov ax,0x2e
0000086B  50                push ax
0000086C  B83000            mov ax,0x30
0000086F  50                push ax
00000870  A0AA37            mov al,[0x37aa]
00000873  98                cbw
00000874  BB0200            mov bx,0x2
00000877  99                cwd
00000878  F7FB              idiv bx
0000087A  BA2E00            mov dx,0x2e
0000087D  F7EA              imul dx
0000087F  053B00            add ax,0x3b
00000882  50                push ax
00000883  A0AA37            mov al,[0x37aa]
00000886  98                cbw
00000887  BB0200            mov bx,0x2
0000088A  99                cwd
0000088B  F7FB              idiv bx
0000088D  BB3000            mov bx,0x30
00000890  8BC2              mov ax,dx
00000892  F7EB              imul bx
00000894  05C000            add ax,0xc0
00000897  50                push ax
00000898  9A0E02960A        call 0xa96:0x20e
0000089D  83C40C            add sp,byte +0xc
000008A0  33C0              xor ax,ax
000008A2  50                push ax
000008A3  B82C00            mov ax,0x2c
000008A6  50                push ax
000008A7  B84000            mov ax,0x40
000008AA  50                push ax
000008AB  B83700            mov ax,0x37
000008AE  50                push ax
000008AF  56                push si
000008B0  9AD704960A        call 0xa96:0x4d7
000008B5  83C40A            add sp,byte +0xa
000008B8  B83700            mov ax,0x37
000008BB  50                push ax
000008BC  8BC6              mov ax,si
000008BE  050800            add ax,0x8
000008C1  50                push ax
000008C2  B82C00            mov ax,0x2c
000008C5  50                push ax
000008C6  B83000            mov ax,0x30
000008C9  50                push ax
000008CA  33C0              xor ax,ax
000008CC  50                push ax
000008CD  A0AA37            mov al,[0x37aa]
000008D0  98                cbw
000008D1  BA3000            mov dx,0x30
000008D4  F7EA              imul dx
000008D6  50                push ax
000008D7  9A3000960A        call 0xa96:0x30
000008DC  83C40C            add sp,byte +0xc
000008DF  81FE0001          cmp si,0x100
000008E3  7521              jnz 0x906
000008E5  33C0              xor ax,ax
000008E7  50                push ax
000008E8  B80001            mov ax,0x100
000008EB  50                push ax
000008EC  B83700            mov ax,0x37
000008EF  50                push ax
000008F0  B84000            mov ax,0x40
000008F3  50                push ax
000008F4  B82C00            mov ax,0x2c
000008F7  50                push ax
000008F8  B88000            mov ax,0x80
000008FB  50                push ax
000008FC  9A3000960A        call 0xa96:0x30
00000901  83C40C            add sp,byte +0xc
00000904  EB1C              jmp short 0x922
00000906  33C0              xor ax,ax
00000908  50                push ax
00000909  56                push si
0000090A  B83700            mov ax,0x37
0000090D  50                push ax
0000090E  B84000            mov ax,0x40
00000911  50                push ax
00000912  B82C00            mov ax,0x2c
00000915  50                push ax
00000916  B84000            mov ax,0x40
00000919  50                push ax
0000091A  9A3000960A        call 0xa96:0x30
0000091F  83C40C            add sp,byte +0xc
00000922  FE06AA37          inc byte [0x37aa]
00000926  803EAA3704        cmp byte [0x37aa],0x4
0000092B  7D03              jnl 0x930
0000092D  E915FE            jmp 0x745
00000930  33C0              xor ax,ax
00000932  50                push ax
00000933  B8C800            mov ax,0xc8
00000936  50                push ax
00000937  B84000            mov ax,0x40
0000093A  50                push ax
0000093B  33C0              xor ax,ax
0000093D  50                push ax
0000093E  FF36CE2B          push word [0x2bce]
00000942  9AD704960A        call 0xa96:0x4d7
00000947  83C40A            add sp,byte +0xa
0000094A  33C0              xor ax,ax
0000094C  50                push ax
0000094D  FF36CE2B          push word [0x2bce]
00000951  B80D00            mov ax,0xd
00000954  50                push ax
00000955  B84000            mov ax,0x40
00000958  50                push ax
00000959  33C0              xor ax,ax
0000095B  50                push ax
0000095C  B8F000            mov ax,0xf0
0000095F  50                push ax
00000960  9A3000960A        call 0xa96:0x30
00000965  83C40C            add sp,byte +0xc
00000968  B8A100            mov ax,0xa1
0000096B  50                push ax
0000096C  FF36CE2B          push word [0x2bce]
00000970  B82700            mov ax,0x27
00000973  50                push ax
00000974  B84000            mov ax,0x40
00000977  50                push ax
00000978  B80D00            mov ax,0xd
0000097B  50                push ax
0000097C  B8F000            mov ax,0xf0
0000097F  50                push ax
00000980  9A3000960A        call 0xa96:0x30
00000985  83C40C            add sp,byte +0xc
00000988  FF36E22B          push word [0x2be2]
0000098C  FF36D82B          push word [0x2bd8]
00000990  9A5010290C        call 0xc29:0x1050
00000995  59                pop cx
00000996  59                pop cx
00000997  833ECE2B00        cmp word [0x2bce],byte +0x0
0000099C  7E0D              jng 0x9ab
0000099E  A1CE2B            mov ax,[0x2bce]
000009A1  05FCFF            add ax,0xfffc
000009A4  50                push ax
000009A5  9A8A0C290C        call 0xc29:0xc8a
000009AA  59                pop cx
000009AB  A1CE2B            mov ax,[0x2bce]
000009AE  054000            add ax,0x40
000009B1  50                push ax
000009B2  9A8A0C290C        call 0xc29:0xc8a
000009B7  59                pop cx
000009B8  5E                pop si
000009B9  5D                pop bp
000009BA  CB                retf

; ---- 0C29:09BB ----
000009BB  55                push bp
000009BC  8BEC              mov bp,sp
000009BE  56                push si
000009BF  57                push di
000009C0  A0AA37            mov al,[0x37aa]
000009C3  98                cbw
000009C4  D1E0              shl ax,1
000009C6  8BD8              mov bx,ax
000009C8  8BBFD02B          mov di,[bx+0x2bd0]
000009CC  A0AA37            mov al,[0x37aa]
000009CF  98                cbw
000009D0  D1E0              shl ax,1
000009D2  8BD8              mov bx,ax
000009D4  8BB7DA2B          mov si,[bx+0x2bda]
000009D8  803EC52B00        cmp byte [0x2bc5],0x0
000009DD  7403              jz 0x9e2
000009DF  E98A00            jmp 0xa6c
000009E2  33C0              xor ax,ax
000009E4  50                push ax
000009E5  A0C52B            mov al,[0x2bc5]
000009E8  98                cbw
000009E9  D1E0              shl ax,1
000009EB  D1E0              shl ax,1
000009ED  50                push ax
000009EE  A0AA37            mov al,[0x37aa]
000009F1  98                cbw
000009F2  5B                pop bx
000009F3  03D8              add bx,ax
000009F5  8A874002          mov al,[bx+0x240]
000009F9  B400              mov ah,0x0
000009FB  50                push ax
000009FC  B80500            mov ax,0x5
000009FF  50                push ax
00000A00  A0AA37            mov al,[0x37aa]
00000A03  98                cbw
00000A04  D1E0              shl ax,1
00000A06  D1E0              shl ax,1
00000A08  8BD8              mov bx,ax
00000A0A  FFB7D43C          push word [bx+0x3cd4]
00000A0E  FFB7D23C          push word [bx+0x3cd2]
00000A12  8BC6              mov ax,si
00000A14  059000            add ax,0x90
00000A17  50                push ax
00000A18  8BC7              mov ax,di
00000A1A  050D00            add ax,0xd
00000A1D  50                push ax
00000A1E  9A7A05960A        call 0xa96:0x57a
00000A23  83C40E            add sp,byte +0xe
00000A26  33C0              xor ax,ax
00000A28  50                push ax
00000A29  A0C52B            mov al,[0x2bc5]
00000A2C  98                cbw
00000A2D  8BD8              mov bx,ax
00000A2F  8A875C02          mov al,[bx+0x25c]
00000A33  B400              mov ah,0x0
00000A35  50                push ax
00000A36  B80300            mov ax,0x3
00000A39  50                push ax
00000A3A  A0AA37            mov al,[0x37aa]
00000A3D  98                cbw
00000A3E  D1E0              shl ax,1
00000A40  D1E0              shl ax,1
00000A42  D1E0              shl ax,1
00000A44  8B16D045          mov dx,[0x45d0]
00000A48  D1E2              shl dx,1
00000A4A  03C2              add ax,dx
00000A4C  8BD8              mov bx,ax
00000A4E  8B87F43C          mov ax,[bx+0x3cf4]
00000A52  99                cwd
00000A53  52                push dx
00000A54  50                push ax
00000A55  8BC6              mov ax,si
00000A57  059000            add ax,0x90
00000A5A  50                push ax
00000A5B  8BC7              mov ax,di
00000A5D  052900            add ax,0x29
00000A60  50                push ax
00000A61  9A7A05960A        call 0xa96:0x57a
00000A66  83C40E            add sp,byte +0xe
00000A69  E9AF00            jmp 0xb1b
00000A6C  33C0              xor ax,ax
00000A6E  50                push ax
00000A6F  A0C52B            mov al,[0x2bc5]
00000A72  98                cbw
00000A73  D1E0              shl ax,1
00000A75  D1E0              shl ax,1
00000A77  50                push ax
00000A78  A0AA37            mov al,[0x37aa]
00000A7B  98                cbw
00000A7C  5B                pop bx
00000A7D  03D8              add bx,ax
00000A7F  8A874002          mov al,[bx+0x240]
00000A83  B400              mov ah,0x0
00000A85  50                push ax
00000A86  B80500            mov ax,0x5
00000A89  50                push ax
00000A8A  A0AA37            mov al,[0x37aa]
00000A8D  98                cbw
00000A8E  D1E0              shl ax,1
00000A90  D1E0              shl ax,1
00000A92  8BD8              mov bx,ax
00000A94  FFB7D43C          push word [bx+0x3cd4]
00000A98  FFB7D23C          push word [bx+0x3cd2]
00000A9C  A0AA37            mov al,[0x37aa]
00000A9F  98                cbw
00000AA0  8BD8              mov bx,ax
00000AA2  80BFB24300        cmp byte [bx+0x43b2],0x0
00000AA7  7505              jnz 0xaae
00000AA9  B84C00            mov ax,0x4c
00000AAC  EB03              jmp short 0xab1
00000AAE  B81500            mov ax,0x15
00000AB1  8BD6              mov dx,si
00000AB3  03D0              add dx,ax
00000AB5  52                push dx
00000AB6  8BC7              mov ax,di
00000AB8  051700            add ax,0x17
00000ABB  50                push ax
00000ABC  9A7A05960A        call 0xa96:0x57a
00000AC1  83C40E            add sp,byte +0xe
00000AC4  33C0              xor ax,ax
00000AC6  50                push ax
00000AC7  A0C52B            mov al,[0x2bc5]
00000ACA  98                cbw
00000ACB  8BD8              mov bx,ax
00000ACD  8A875C02          mov al,[bx+0x25c]
00000AD1  B400              mov ah,0x0
00000AD3  50                push ax
00000AD4  B80300            mov ax,0x3
00000AD7  50                push ax
00000AD8  A0AA37            mov al,[0x37aa]
00000ADB  98                cbw
00000ADC  D1E0              shl ax,1
00000ADE  D1E0              shl ax,1
00000AE0  D1E0              shl ax,1
00000AE2  8B16D045          mov dx,[0x45d0]
00000AE6  D1E2              shl dx,1
00000AE8  03C2              add ax,dx
00000AEA  8BD8              mov bx,ax
00000AEC  8B87F43C          mov ax,[bx+0x3cf4]
00000AF0  99                cwd
00000AF1  52                push dx
00000AF2  50                push ax
00000AF3  A0AA37            mov al,[0x37aa]
00000AF6  98                cbw
00000AF7  8BD8              mov bx,ax
00000AF9  80BFB24300        cmp byte [bx+0x43b2],0x0
00000AFE  7505              jnz 0xb05
00000B00  B84400            mov ax,0x44
00000B03  EB03              jmp short 0xb08
00000B05  B84A00            mov ax,0x4a
00000B08  8BD6              mov dx,si
00000B0A  03D0              add dx,ax
00000B0C  52                push dx
00000B0D  8BC7              mov ax,di
00000B0F  051A00            add ax,0x1a
00000B12  50                push ax
00000B13  9A7A05960A        call 0xa96:0x57a
00000B18  83C40E            add sp,byte +0xe
00000B1B  5F                pop di
00000B1C  5E                pop si
00000B1D  5D                pop bp
00000B1E  CB                retf

; ---- 0C29:0B1F ----
00000B1F  55                push bp
00000B20  8BEC              mov bp,sp
00000B22  33C0              xor ax,ax
00000B24  50                push ax
00000B25  A0C52B            mov al,[0x2bc5]
00000B28  98                cbw
00000B29  8BD8              mov bx,ax
00000B2B  8A875E02          mov al,[bx+0x25e]
00000B2F  B400              mov ah,0x0
00000B31  50                push ax
00000B32  B80300            mov ax,0x3
00000B35  50                push ax
00000B36  A15A45            mov ax,[0x455a]
00000B39  99                cwd
00000B3A  52                push dx
00000B3B  50                push ax
00000B3C  A0C52B            mov al,[0x2bc5]
00000B3F  98                cbw
00000B40  8BD8              mov bx,ax
00000B42  8A876002          mov al,[bx+0x260]
00000B46  B400              mov ah,0x0
00000B48  50                push ax
00000B49  A1CE2B            mov ax,[0x2bce]
00000B4C  052700            add ax,0x27
00000B4F  50                push ax
00000B50  9A7A05960A        call 0xa96:0x57a
00000B55  83C40E            add sp,byte +0xe
00000B58  A15A45            mov ax,[0x455a]
00000B5B  A3E42B            mov [0x2be4],ax
00000B5E  5D                pop bp
00000B5F  CB                retf

; ---- 0C29:0B60 ----
00000B60  55                push bp
00000B61  8BEC              mov bp,sp
00000B63  A0AA37            mov al,[0x37aa]
00000B66  98                cbw
00000B67  D1E0              shl ax,1
00000B69  D1E0              shl ax,1
00000B6B  8BD8              mov bx,ax
00000B6D  83BFB84300        cmp word [bx+0x43b8],byte +0x0
00000B72  7C10              jl 0xb84
00000B74  7F07              jg 0xb7d
00000B76  83BFB64300        cmp word [bx+0x43b6],byte +0x0
00000B7B  7607              jna 0xb84
00000B7D  803EC52B01        cmp byte [0x2bc5],0x1
00000B82  7502              jnz 0xb86
00000B84  EB51              jmp short 0xbd7
00000B86  33C0              xor ax,ax
00000B88  50                push ax
00000B89  B80800            mov ax,0x8
00000B8C  50                push ax
00000B8D  33C0              xor ax,ax
00000B8F  BA0A00            mov dx,0xa
00000B92  50                push ax
00000B93  52                push dx
00000B94  A0AA37            mov al,[0x37aa]
00000B97  98                cbw
00000B98  D1E0              shl ax,1
00000B9A  D1E0              shl ax,1
00000B9C  8BD8              mov bx,ax
00000B9E  FFB7B843          push word [bx+0x43b8]
00000BA2  FFB7B643          push word [bx+0x43b6]
00000BA6  9A894F0000        call 0x0:0x4f89
00000BAB  50                push ax
00000BAC  A0AA37            mov al,[0x37aa]
00000BAF  98                cbw
00000BB0  D1E0              shl ax,1
00000BB2  8BD8              mov bx,ax
00000BB4  8B87DA2B          mov ax,[bx+0x2bda]
00000BB8  058900            add ax,0x89
00000BBB  5A                pop dx
00000BBC  2BC2              sub ax,dx
00000BBE  50                push ax
00000BBF  A0AA37            mov al,[0x37aa]
00000BC2  98                cbw
00000BC3  D1E0              shl ax,1
00000BC5  8BD8              mov bx,ax
00000BC7  8B87D02B          mov ax,[bx+0x2bd0]
00000BCB  051800            add ax,0x18
00000BCE  50                push ax
00000BCF  9A0B00690A        call 0xa69:0xb
00000BD4  83C408            add sp,byte +0x8
00000BD7  5D                pop bp
00000BD8  CB                retf

; ---- 0C29:0BD9 ----
00000BD9  55                push bp
00000BDA  8BEC              mov bp,sp
00000BDC  56                push si
00000BDD  57                push di
00000BDE  8B7E06            mov di,[bp+0x6]
00000BE1  8B7608            mov si,[bp+0x8]
00000BE4  83FF04            cmp di,byte +0x4
00000BE7  7575              jnz 0xc5e
00000BE9  33C0              xor ax,ax
00000BEB  50                push ax
00000BEC  B8C800            mov ax,0xc8
00000BEF  50                push ax
00000BF0  B84000            mov ax,0x40
00000BF3  50                push ax
00000BF4  33C0              xor ax,ax
00000BF6  50                push ax
00000BF7  56                push si
00000BF8  9AD704960A        call 0xa96:0x4d7
00000BFD  83C40A            add sp,byte +0xa
00000C00  B80500            mov ax,0x5
00000C03  50                push ax
00000C04  56                push si
00000C05  B80800            mov ax,0x8
00000C08  50                push ax
00000C09  B84000            mov ax,0x40
00000C0C  50                push ax
00000C0D  B89600            mov ax,0x96
00000C10  50                push ax
00000C11  33C0              xor ax,ax
00000C13  50                push ax
00000C14  9A3000960A        call 0xa96:0x30
00000C19  83C40C            add sp,byte +0xc
00000C1C  B80500            mov ax,0x5
00000C1F  50                push ax
00000C20  B84000            mov ax,0x40
00000C23  50                push ax
00000C24  33C0              xor ax,ax
00000C26  50                push ax
00000C27  56                push si
00000C28  B80100            mov ax,0x1
00000C2B  50                push ax
00000C2C  B84000            mov ax,0x40
00000C2F  50                push ax
00000C30  B8C800            mov ax,0xc8
00000C33  50                push ax
00000C34  B84000            mov ax,0x40
00000C37  50                push ax
00000C38  9A6702960A        call 0xa96:0x267
00000C3D  83C410            add sp,byte +0x10
00000C40  B8A000            mov ax,0xa0
00000C43  50                push ax
00000C44  56                push si
00000C45  B82800            mov ax,0x28
00000C48  50                push ax
00000C49  B84000            mov ax,0x40
00000C4C  50                push ax
00000C4D  B8A000            mov ax,0xa0
00000C50  50                push ax
00000C51  33C0              xor ax,ax
00000C53  50                push ax
00000C54  9A3000960A        call 0xa96:0x30
00000C59  83C40C            add sp,byte +0xc
00000C5C  EB28              jmp short 0xc86
00000C5E  FF760A            push word [bp+0xa]
00000C61  56                push si
00000C62  8BDF              mov bx,di
00000C64  D1E3              shl bx,1
00000C66  FFB78A02          push word [bx+0x28a]
00000C6A  B84000            mov ax,0x40
00000C6D  50                push ax
00000C6E  8BDF              mov bx,di
00000C70  D1E3              shl bx,1
00000C72  FFB77602          push word [bx+0x276]
00000C76  8BDF              mov bx,di
00000C78  D1E3              shl bx,1
00000C7A  FFB76202          push word [bx+0x262]
00000C7E  9A3000960A        call 0xa96:0x30
00000C83  83C40C            add sp,byte +0xc
00000C86  5F                pop di
00000C87  5E                pop si
00000C88  5D                pop bp
00000C89  CB                retf

; ---- 0C29:0C8A ----
00000C8A  55                push bp
00000C8B  8BEC              mov bp,sp
00000C8D  56                push si
00000C8E  33F6              xor si,si
00000C90  EB27              jmp short 0xcb9
00000C92  A0C52B            mov al,[0x2bc5]
00000C95  98                cbw
00000C96  D1E0              shl ax,1
00000C98  D1E0              shl ax,1
00000C9A  8BD8              mov bx,ax
00000C9C  8A809E02          mov al,[bx+si+0x29e]
00000CA0  B400              mov ah,0x0
00000CA2  50                push ax
00000CA3  B8C800            mov ax,0xc8
00000CA6  50                push ax
00000CA7  33C0              xor ax,ax
00000CA9  50                push ax
00000CAA  8B4606            mov ax,[bp+0x6]
00000CAD  03C6              add ax,si
00000CAF  50                push ax
00000CB0  9A2C00690A        call 0xa69:0x2c
00000CB5  83C408            add sp,byte +0x8
00000CB8  46                inc si
00000CB9  83FE03            cmp si,byte +0x3
00000CBC  7ED4              jng 0xc92
00000CBE  5E                pop si
00000CBF  5D                pop bp
00000CC0  CB                retf

; ---- 0C29:0CC1 ----
00000CC1  55                push bp
00000CC2  8BEC              mov bp,sp
00000CC4  56                push si
00000CC5  57                push di
00000CC6  803EC52B00        cmp byte [0x2bc5],0x0
00000CCB  7570              jnz 0xd3d
00000CCD  A18037            mov ax,[0x3780]
00000CD0  8BF0              mov si,ax
00000CD2  3D1E00            cmp ax,0x1e
00000CD5  7E03              jng 0xcda
00000CD7  BE1E00            mov si,0x1e
00000CDA  B8C100            mov ax,0xc1
00000CDD  2BC6              sub ax,si
00000CDF  50                push ax
00000CE0  A1CA2B            mov ax,[0x2bca]
00000CE3  050A00            add ax,0xa
00000CE6  50                push ax
00000CE7  56                push si
00000CE8  B81000            mov ax,0x10
00000CEB  50                push ax
00000CEC  B85000            mov ax,0x50
00000CEF  2BC6              sub ax,si
00000CF1  50                push ax
00000CF2  B83001            mov ax,0x130
00000CF5  50                push ax
00000CF6  9A3000960A        call 0xa96:0x30
00000CFB  83C40C            add sp,byte +0xc
00000CFE  833E80371E        cmp word [0x3780],byte +0x1e
00000D03  7F02              jg 0xd07
00000D05  EB5F              jmp short 0xd66
00000D07  A18037            mov ax,[0x3780]
00000D0A  05E2FF            add ax,0xffe2
00000D0D  8BF0              mov si,ax
00000D0F  3D1E00            cmp ax,0x1e
00000D12  7E03              jng 0xd17
00000D14  BE1E00            mov si,0x1e
00000D17  B8C100            mov ax,0xc1
00000D1A  2BC6              sub ax,si
00000D1C  50                push ax
00000D1D  A1CA2B            mov ax,[0x2bca]
00000D20  052700            add ax,0x27
00000D23  50                push ax
00000D24  56                push si
00000D25  B81000            mov ax,0x10
00000D28  50                push ax
00000D29  B85000            mov ax,0x50
00000D2C  2BC6              sub ax,si
00000D2E  50                push ax
00000D2F  B83001            mov ax,0x130
00000D32  50                push ax
00000D33  9A3000960A        call 0xa96:0x30
00000D38  83C40C            add sp,byte +0xc
00000D3B  EB29              jmp short 0xd66
00000D3D  A18037            mov ax,[0x3780]
00000D40  8BF8              mov di,ax
00000D42  3D3900            cmp ax,0x39
00000D45  7E03              jng 0xd4a
00000D47  BF3900            mov di,0x39
00000D4A  B80D00            mov ax,0xd
00000D4D  50                push ax
00000D4E  B80300            mov ax,0x3
00000D51  50                push ax
00000D52  57                push di
00000D53  B80400            mov ax,0x4
00000D56  50                push ax
00000D57  A1CA2B            mov ax,[0x2bca]
00000D5A  050400            add ax,0x4
00000D5D  50                push ax
00000D5E  9AD704960A        call 0xa96:0x4d7
00000D63  83C40A            add sp,byte +0xa
00000D66  5F                pop di
00000D67  5E                pop si
00000D68  5D                pop bp
00000D69  CB                retf

; ---- 0C29:0D6A ----
00000D6A  55                push bp
00000D6B  8BEC              mov bp,sp
00000D6D  4C                dec sp
00000D6E  4C                dec sp
00000D6F  56                push si
00000D70  57                push di
00000D71  833E803700        cmp word [0x3780],byte +0x0
00000D76  7D03              jnl 0xd7b
00000D78  E98000            jmp 0xdfb
00000D7B  A18037            mov ax,[0x3780]
00000D7E  40                inc ax
00000D7F  8BF0              mov si,ax
00000D81  803EC52B00        cmp byte [0x2bc5],0x0
00000D86  7550              jnz 0xdd8
00000D88  83FE3C            cmp si,byte +0x3c
00000D8B  7E02              jng 0xd8f
00000D8D  EB6C              jmp short 0xdfb
00000D8F  8BC6              mov ax,si
00000D91  8BF8              mov di,ax
00000D93  3D1E00            cmp ax,0x1e
00000D96  7E07              jng 0xd9f
00000D98  8BC7              mov ax,di
00000D9A  05E2FF            add ax,0xffe2
00000D9D  8BF8              mov di,ax
00000D9F  83FE1E            cmp si,byte +0x1e
00000DA2  7E05              jng 0xda9
00000DA4  B82700            mov ax,0x27
00000DA7  EB03              jmp short 0xdac
00000DA9  B80A00            mov ax,0xa
00000DAC  8946FE            mov [bp-0x2],ax
00000DAF  B8C100            mov ax,0xc1
00000DB2  2BC7              sub ax,di
00000DB4  50                push ax
00000DB5  A1CA2B            mov ax,[0x2bca]
00000DB8  0346FE            add ax,[bp-0x2]
00000DBB  50                push ax
00000DBC  B80100            mov ax,0x1
00000DBF  50                push ax
00000DC0  B81000            mov ax,0x10
00000DC3  50                push ax
00000DC4  B8C100            mov ax,0xc1
00000DC7  2BC7              sub ax,di
00000DC9  50                push ax
00000DCA  B84A00            mov ax,0x4a
00000DCD  50                push ax
00000DCE  9A3000960A        call 0xa96:0x30
00000DD3  83C40C            add sp,byte +0xc
00000DD6  EB23              jmp short 0xdfb
00000DD8  83FE39            cmp si,byte +0x39
00000DDB  7E02              jng 0xddf
00000DDD  EB1C              jmp short 0xdfb
00000DDF  33C0              xor ax,ax
00000DE1  50                push ax
00000DE2  B80300            mov ax,0x3
00000DE5  50                push ax
00000DE6  B80400            mov ax,0x4
00000DE9  50                push ax
00000DEA  A1CA2B            mov ax,[0x2bca]
00000DED  03C6              add ax,si
00000DEF  050300            add ax,0x3
00000DF2  50                push ax
00000DF3  9A2C00690A        call 0xa69:0x2c
00000DF8  83C408            add sp,byte +0x8
00000DFB  5F                pop di
00000DFC  5E                pop si
00000DFD  8BE5              mov sp,bp
00000DFF  5D                pop bp
00000E00  CB                retf

; ---- 0C29:0E01 ----
00000E01  55                push bp
00000E02  8BEC              mov bp,sp
00000E04  83EC06            sub sp,byte +0x6
00000E07  56                push si
00000E08  57                push di
00000E09  C746FE0000        mov word [bp-0x2],0x0
00000E0E  C606AA3700        mov byte [0x37aa],0x0
00000E13  E94201            jmp 0xf58
00000E16  A0AA37            mov al,[0x37aa]
00000E19  98                cbw
00000E1A  D1E0              shl ax,1
00000E1C  8BD8              mov bx,ax
00000E1E  8B87A243          mov ax,[bx+0x43a2]
00000E22  8946FA            mov [bp-0x6],ax
00000E25  A0AA37            mov al,[0x37aa]
00000E28  98                cbw
00000E29  8BD8              mov bx,ax
00000E2B  80BFB24300        cmp byte [bx+0x43b2],0x0
00000E30  7527              jnz 0xe59
00000E32  A0AA37            mov al,[0x37aa]
00000E35  98                cbw
00000E36  8A16CC2B          mov dl,[0x2bcc]
00000E3A  8BD8              mov bx,ax
00000E3C  8497C608          test [bx+0x8c6],dl
00000E40  7408              jz 0xe4a
00000E42  BE8000            mov si,0x80
00000E45  BFC800            mov di,0xc8
00000E48  EB05              jmp short 0xe4f
00000E4A  33F6              xor si,si
00000E4C  BFC800            mov di,0xc8
00000E4F  0376FE            add si,[bp-0x2]
00000E52  C746FC0200        mov word [bp-0x4],0x2
00000E57  EB40              jmp short 0xe99
00000E59  A0AA37            mov al,[0x37aa]
00000E5C  98                cbw
00000E5D  8BD8              mov bx,ax
00000E5F  80BFB24301        cmp byte [bx+0x43b2],0x1
00000E64  7528              jnz 0xe8e
00000E66  A0AA37            mov al,[0x37aa]
00000E69  98                cbw
00000E6A  8A16CC2B          mov dl,[0x2bcc]
00000E6E  8BD8              mov bx,ax
00000E70  8497C608          test [bx+0x8c6],dl
00000E74  7408              jz 0xe7e
00000E76  BEC000            mov si,0xc0
00000E79  BFC800            mov di,0xc8
00000E7C  EB06              jmp short 0xe84
00000E7E  BE4000            mov si,0x40
00000E81  BFC800            mov di,0xc8
00000E84  C746FC0000        mov word [bp-0x4],0x0
00000E89  0376FE            add si,[bp-0x2]
00000E8C  EB0B              jmp short 0xe99
00000E8E  BE0001            mov si,0x100
00000E91  BFC800            mov di,0xc8
00000E94  C746FC0000        mov word [bp-0x4],0x0
00000E99  B87400            mov ax,0x74
00000E9C  2B46FA            sub ax,[bp-0x6]
00000E9F  0346FC            add ax,[bp-0x4]
00000EA2  50                push ax
00000EA3  A1CE2B            mov ax,[0x2bce]
00000EA6  0346FE            add ax,[bp-0x2]
00000EA9  50                push ax
00000EAA  B82800            mov ax,0x28
00000EAD  50                push ax
00000EAE  B80200            mov ax,0x2
00000EB1  50                push ax
00000EB2  57                push di
00000EB3  56                push si
00000EB4  9AD406CD23        call 0x23cd:0x6d4
00000EB9  83C40C            add sp,byte +0xc
00000EBC  8B46FE            mov ax,[bp-0x2]
00000EBF  051000            add ax,0x10
00000EC2  8946FE            mov [bp-0x2],ax
00000EC5  A0AA37            mov al,[0x37aa]
00000EC8  98                cbw
00000EC9  8BD8              mov bx,ax
00000ECB  80BFB24306        cmp byte [bx+0x43b2],0x6
00000ED0  7503              jnz 0xed5
00000ED2  E97F00            jmp 0xf54
00000ED5  837EFA00          cmp word [bp-0x6],byte +0x0
00000ED9  7C79              jl 0xf54
00000EDB  837EFA64          cmp word [bp-0x6],byte +0x64
00000EDF  7D73              jnl 0xf54
00000EE1  FF36A802          push word [0x2a8]
00000EE5  FF36A602          push word [0x2a6]
00000EE9  B80300            mov ax,0x3
00000EEC  50                push ax
00000EED  8B46FA            mov ax,[bp-0x6]
00000EF0  BB0200            mov bx,0x2
00000EF3  99                cwd
00000EF4  F7FB              idiv bx
00000EF6  03065A45          add ax,[0x455a]
00000EFA  99                cwd
00000EFB  52                push dx
00000EFC  50                push ax
00000EFD  9A01009522        call 0x2295:0x1
00000F02  83C40A            add sp,byte +0xa
00000F05  A0C52B            mov al,[0x2bc5]
00000F08  98                cbw
00000F09  D1E0              shl ax,1
00000F0B  D1E0              shl ax,1
00000F0D  50                push ax
00000F0E  A0AA37            mov al,[0x37aa]
00000F11  98                cbw
00000F12  5B                pop bx
00000F13  03D8              add bx,ax
00000F15  8A874002          mov al,[bx+0x240]
00000F19  B400              mov ah,0x0
00000F1B  50                push ax
00000F1C  9AC305CD23        call 0x23cd:0x5c3
00000F21  59                pop cx
00000F22  FF36A802          push word [0x2a8]
00000F26  FF36A602          push word [0x2a6]
00000F2A  A0AA37            mov al,[0x37aa]
00000F2D  98                cbw
00000F2E  D1E0              shl ax,1
00000F30  BA7400            mov dx,0x74
00000F33  8BD8              mov bx,ax
00000F35  2B97A243          sub dx,[bx+0x43a2]
00000F39  83C223            add dx,byte +0x23
00000F3C  52                push dx
00000F3D  A0AA37            mov al,[0x37aa]
00000F40  98                cbw
00000F41  B104              mov cl,0x4
00000F43  D3E0              shl ax,cl
00000F45  8B16CE2B          mov dx,[0x2bce]
00000F49  03D0              add dx,ax
00000F4B  52                push dx
00000F4C  9ADD05CD23        call 0x23cd:0x5dd
00000F51  83C408            add sp,byte +0x8
00000F54  FE06AA37          inc byte [0x37aa]
00000F58  803EAA3704        cmp byte [0x37aa],0x4
00000F5D  7D03              jnl 0xf62
00000F5F  E9B4FE            jmp 0xe16
00000F62  5F                pop di
00000F63  5E                pop si
00000F64  8BE5              mov sp,bp
00000F66  5D                pop bp
00000F67  CB                retf

; ---- 0C29:0F68 ----
00000F68  55                push bp
00000F69  8BEC              mov bp,sp
00000F6B  56                push si
00000F6C  A0AA37            mov al,[0x37aa]
00000F6F  98                cbw
00000F70  8BD8              mov bx,ax
00000F72  80BFB24300        cmp byte [bx+0x43b2],0x0
00000F77  7505              jnz 0xf7e
00000F79  B80200            mov ax,0x2
00000F7C  EB02              jmp short 0xf80
00000F7E  33C0              xor ax,ax
00000F80  8BF0              mov si,ax
00000F82  33C0              xor ax,ax
00000F84  50                push ax
00000F85  B82800            mov ax,0x28
00000F88  50                push ax
00000F89  B81000            mov ax,0x10
00000F8C  50                push ax
00000F8D  A0AA37            mov al,[0x37aa]
00000F90  98                cbw
00000F91  D1E0              shl ax,1
00000F93  BA7400            mov dx,0x74
00000F96  8BD8              mov bx,ax
00000F98  2B97A243          sub dx,[bx+0x43a2]
00000F9C  03D6              add dx,si
00000F9E  52                push dx
00000F9F  A0AA37            mov al,[0x37aa]
00000FA2  98                cbw
00000FA3  B104              mov cl,0x4
00000FA5  D3E0              shl ax,cl
00000FA7  8B16CE2B          mov dx,[0x2bce]
00000FAB  03D0              add dx,ax
00000FAD  52                push dx
00000FAE  9AD704960A        call 0xa96:0x4d7
00000FB3  83C40A            add sp,byte +0xa
00000FB6  5E                pop si
00000FB7  5D                pop bp
00000FB8  CB                retf

; ---- 0C29:0FB9 ----
00000FB9  55                push bp
00000FBA  8BEC              mov bp,sp
00000FBC  C606AA3700        mov byte [0x37aa],0x0
00000FC1  E98000            jmp 0x1044
00000FC4  A0AA37            mov al,[0x37aa]
00000FC7  98                cbw
00000FC8  8BD8              mov bx,ax
00000FCA  80BFC62B01        cmp byte [bx+0x2bc6],0x1
00000FCF  756F              jnz 0x1040
00000FD1  A0AA37            mov al,[0x37aa]
00000FD4  98                cbw
00000FD5  8A16CC2B          mov dl,[0x2bcc]
00000FD9  8BD8              mov bx,ax
00000FDB  8497C608          test [bx+0x8c6],dl
00000FDF  7402              jz 0xfe3
00000FE1  EB5D              jmp short 0x1040
00000FE3  B82000            mov ax,0x20
00000FE6  50                push ax
00000FE7  B80100            mov ax,0x1
00000FEA  50                push ax
00000FEB  9A5400A721        call 0x21a7:0x54
00000FF0  59                pop cx
00000FF1  59                pop cx
00000FF2  A0AA37            mov al,[0x37aa]
00000FF5  98                cbw
00000FF6  8BD8              mov bx,ax
00000FF8  C687B24306        mov byte [bx+0x43b2],0x6
00000FFD  A0AA37            mov al,[0x37aa]
00001000  98                cbw
00001001  D1E0              shl ax,1
00001003  8BD8              mov bx,ax
00001005  C787A2436700      mov word [bx+0x43a2],0x67
0000100B  FF0EBC2B          dec word [0x2bbc]
0000100F  33C0              xor ax,ax
00001011  50                push ax
00001012  B88D00            mov ax,0x8d
00001015  50                push ax
00001016  B81000            mov ax,0x10
00001019  50                push ax
0000101A  B80D00            mov ax,0xd
0000101D  50                push ax
0000101E  A0AA37            mov al,[0x37aa]
00001021  98                cbw
00001022  B104              mov cl,0x4
00001024  D3E0              shl ax,cl
00001026  8B16CE2B          mov dx,[0x2bce]
0000102A  03D0              add dx,ax
0000102C  52                push dx
0000102D  9AD704960A        call 0xa96:0x4d7
00001032  83C40A            add sp,byte +0xa
00001035  A0AA37            mov al,[0x37aa]
00001038  98                cbw
00001039  8BD8              mov bx,ax
0000103B  C687C62B00        mov byte [bx+0x2bc6],0x0
00001040  FE06AA37          inc byte [0x37aa]
00001044  803EAA3704        cmp byte [0x37aa],0x4
00001049  7D03              jnl 0x104e
0000104B  E976FF            jmp 0xfc4
0000104E  5D                pop bp
0000104F  CB                retf

; ---- 0C29:1050 ----
00001050  55                push bp
00001051  8BEC              mov bp,sp
00001053  8B1ED045          mov bx,[0x45d0]
00001057  8A87BE02          mov al,[bx+0x2be]
0000105B  98                cbw
0000105C  8B5608            mov dx,[bp+0x8]
0000105F  03D0              add dx,ax
00001061  52                push dx
00001062  8B1ED045          mov bx,[0x45d0]
00001066  8A87BA02          mov al,[bx+0x2ba]
0000106A  98                cbw
0000106B  8B5606            mov dx,[bp+0x6]
0000106E  03D0              add dx,ax
00001070  52                push dx
00001071  8B1ED045          mov bx,[0x45d0]
00001075  8A87B602          mov al,[bx+0x2b6]
00001079  B400              mov ah,0x0
0000107B  50                push ax
0000107C  B83000            mov ax,0x30
0000107F  50                push ax
00001080  8B1ED045          mov bx,[0x45d0]
00001084  8A87B202          mov al,[bx+0x2b2]
00001088  B400              mov ah,0x0
0000108A  50                push ax
0000108B  8B1ED045          mov bx,[0x45d0]
0000108F  D1E3              shl bx,1
00001091  FFB7AA02          push word [bx+0x2aa]
00001095  9A0E02960A        call 0xa96:0x20e
0000109A  83C40C            add sp,byte +0xc
0000109D  5D                pop bp
0000109E  CB                retf

; ---- 0C29:109F ----
0000109F  55                push bp
000010A0  8BEC              mov bp,sp
000010A2  56                push si
000010A3  803EC52B00        cmp byte [0x2bc5],0x0
000010A8  754C              jnz 0x10f6
000010AA  33F6              xor si,si
000010AC  EB16              jmp short 0x10c4
000010AE  9A1B00960A        call 0xa96:0x1b
000010B3  B001              mov al,0x1
000010B5  50                push ax
000010B6  56                push si
000010B7  9A6311290C        call 0xc29:0x1163
000010BC  59                pop cx
000010BD  59                pop cx
000010BE  9AA806960A        call 0xa96:0x6a8
000010C3  46                inc si
000010C4  A0AA37            mov al,[0x37aa]
000010C7  98                cbw
000010C8  D1E0              shl ax,1
000010CA  D1E0              shl ax,1
000010CC  D1E0              shl ax,1
000010CE  8B16D045          mov dx,[0x45d0]
000010D2  D1E2              shl dx,1
000010D4  03C2              add ax,dx
000010D6  8BD8              mov bx,ax
000010D8  8B87F43C          mov ax,[bx+0x3cf4]
000010DC  BB0A00            mov bx,0xa
000010DF  99                cwd
000010E0  F7FB              idiv bx
000010E2  3BC6              cmp ax,si
000010E4  7FC8              jg 0x10ae
000010E6  9A1B00960A        call 0xa96:0x1b
000010EB  0E                push cs
000010EC  E871FA            call 0xb60
000010EF  9AA806960A        call 0xa96:0x6a8
000010F4  EB0F              jmp short 0x1105
000010F6  9A1B00960A        call 0xa96:0x1b
000010FB  9A4D13290C        call 0xc29:0x134d
00001100  9AA806960A        call 0xa96:0x6a8
00001105  5E                pop si
00001106  5D                pop bp
00001107  CB                retf

; ---- 0C29:1108 ----
00001108  55                push bp
00001109  8BEC              mov bp,sp
0000110B  56                push si
0000110C  803EC52B00        cmp byte [0x2bc5],0x0
00001111  7548              jnz 0x115b
00001113  A0AA37            mov al,[0x37aa]
00001116  98                cbw
00001117  D1E0              shl ax,1
00001119  D1E0              shl ax,1
0000111B  D1E0              shl ax,1
0000111D  8B16D045          mov dx,[0x45d0]
00001121  D1E2              shl dx,1
00001123  03C2              add ax,dx
00001125  8BD8              mov bx,ax
00001127  8B87F43C          mov ax,[bx+0x3cf4]
0000112B  BB0A00            mov bx,0xa
0000112E  99                cwd
0000112F  F7FB              idiv bx
00001131  48                dec ax
00001132  8BF0              mov si,ax
00001134  837E0601          cmp word [bp+0x6],byte +0x1
00001138  750D              jnz 0x1147
0000113A  B001              mov al,0x1
0000113C  50                push ax
0000113D  56                push si
0000113E  9A6311290C        call 0xc29:0x1163
00001143  59                pop cx
00001144  59                pop cx
00001145  EB0E              jmp short 0x1155
00001147  B000              mov al,0x0
00001149  50                push ax
0000114A  8BC6              mov ax,si
0000114C  40                inc ax
0000114D  50                push ax
0000114E  9A6311290C        call 0xc29:0x1163
00001153  59                pop cx
00001154  59                pop cx
00001155  0E                push cs
00001156  E807FA            call 0xb60
00001159  EB05              jmp short 0x1160
0000115B  9A4D13290C        call 0xc29:0x134d
00001160  5E                pop si
00001161  5D                pop bp
00001162  CB                retf

; ---- 0C29:1163 ----
00001163  55                push bp
00001164  8BEC              mov bp,sp
00001166  56                push si
00001167  57                push di
00001168  8B7E06            mov di,[bp+0x6]
0000116B  A0AA37            mov al,[0x37aa]
0000116E  98                cbw
0000116F  8BD8              mov bx,ax
00001171  83FB03            cmp bx,byte +0x3
00001174  7603              jna 0x1179
00001176  E9F900            jmp 0x1272
00001179  D1E3              shl bx,1
0000117B  2EFFA77612        jmp [cs:bx+0x1276]
00001180  8BC7              mov ax,di
00001182  BB1900            mov bx,0x19
00001185  99                cwd
00001186  F7FB              idiv bx
00001188  8BF2              mov si,dx
0000118A  8A4608            mov al,[bp+0x8]
0000118D  50                push ax
0000118E  8A840D03          mov al,[si+0x30d]
00001192  B400              mov ah,0x0
00001194  50                push ax
00001195  8A84F402          mov al,[si+0x2f4]
00001199  B400              mov ah,0x0
0000119B  50                push ax
0000119C  8A84DB02          mov al,[si+0x2db]
000011A0  B400              mov ah,0x0
000011A2  50                push ax
000011A3  8A84C202          mov al,[si+0x2c2]
000011A7  B400              mov ah,0x0
000011A9  50                push ax
000011AA  57                push di
000011AB  9A7E12290C        call 0xc29:0x127e
000011B0  83C40C            add sp,byte +0xc
000011B3  E9BC00            jmp 0x1272
000011B6  B86300            mov ax,0x63
000011B9  2BC7              sub ax,di
000011BB  8BF0              mov si,ax
000011BD  8A4608            mov al,[bp+0x8]
000011C0  50                push ax
000011C1  8A845204          mov al,[si+0x452]
000011C5  B400              mov ah,0x0
000011C7  50                push ax
000011C8  8A84EE03          mov al,[si+0x3ee]
000011CC  B400              mov ah,0x0
000011CE  50                push ax
000011CF  8A848A03          mov al,[si+0x38a]
000011D3  B400              mov ah,0x0
000011D5  50                push ax
000011D6  8A842603          mov al,[si+0x326]
000011DA  B400              mov ah,0x0
000011DC  50                push ax
000011DD  57                push di
000011DE  9A7E12290C        call 0xc29:0x127e
000011E3  83C40C            add sp,byte +0xc
000011E6  E98900            jmp 0x1272
000011E9  83FF09            cmp di,byte +0x9
000011EC  7F04              jg 0x11f2
000011EE  8BC7              mov ax,di
000011F0  EB0F              jmp short 0x1201
000011F2  83FF5A            cmp di,byte +0x5a
000011F5  7C07              jl 0x11fe
000011F7  B86300            mov ax,0x63
000011FA  2BC7              sub ax,di
000011FC  EB03              jmp short 0x1201
000011FE  B80900            mov ax,0x9
00001201  8BF0              mov si,ax
00001203  8A4608            mov al,[bp+0x8]
00001206  50                push ax
00001207  8A84D404          mov al,[si+0x4d4]
0000120B  B400              mov ah,0x0
0000120D  50                push ax
0000120E  8A84CA04          mov al,[si+0x4ca]
00001212  B400              mov ah,0x0
00001214  50                push ax
00001215  8A84C004          mov al,[si+0x4c0]
00001219  B400              mov ah,0x0
0000121B  50                push ax
0000121C  8A84B604          mov al,[si+0x4b6]
00001220  B400              mov ah,0x0
00001222  50                push ax
00001223  57                push di
00001224  9A7E12290C        call 0xc29:0x127e
00001229  83C40C            add sp,byte +0xc
0000122C  EB44              jmp short 0x1272
0000122E  83FF21            cmp di,byte +0x21
00001231  7F04              jg 0x1237
00001233  8BC7              mov ax,di
00001235  EB0E              jmp short 0x1245
00001237  8BC7              mov ax,di
00001239  48                dec ax
0000123A  BB2100            mov bx,0x21
0000123D  99                cwd
0000123E  F7FB              idiv bx
00001240  B82000            mov ax,0x20
00001243  2BC2              sub ax,dx
00001245  8BF0              mov si,ax
00001247  8A4608            mov al,[bp+0x8]
0000124A  50                push ax
0000124B  8A844405          mov al,[si+0x544]
0000124F  B400              mov ah,0x0
00001251  50                push ax
00001252  8A842205          mov al,[si+0x522]
00001256  B400              mov ah,0x0
00001258  50                push ax
00001259  8A840005          mov al,[si+0x500]
0000125D  B400              mov ah,0x0
0000125F  50                push ax
00001260  8A84DE04          mov al,[si+0x4de]
00001264  B400              mov ah,0x0
00001266  50                push ax
00001267  57                push di
00001268  9A7E12290C        call 0xc29:0x127e
0000126D  83C40C            add sp,byte +0xc
00001270  EB00              jmp short 0x1272
00001272  5F                pop di
00001273  5E                pop si
00001274  5D                pop bp
00001275  CB                retf
00001276  8011B6            adc byte [bx+di],0xb6
00001279  11E9              adc cx,bp
0000127B  112E1255          adc [0x5512],bp
0000127F  8BEC              mov bp,sp
00001281  4C                dec sp
00001282  4C                dec sp
00001283  56                push si
00001284  57                push di
00001285  807E1001          cmp byte [bp+0x10],0x1
00001289  751F              jnz 0x12aa
0000128B  A0AA37            mov al,[0x37aa]
0000128E  98                cbw
0000128F  8BD8              mov bx,ax
00001291  8A876605          mov al,[bx+0x566]
00001295  B400              mov ah,0x0
00001297  8946FE            mov [bp-0x2],ax
0000129A  A0AA37            mov al,[0x37aa]
0000129D  98                cbw
0000129E  8BD8              mov bx,ax
000012A0  8A874002          mov al,[bx+0x240]
000012A4  B400              mov ah,0x0
000012A6  8BF8              mov di,ax
000012A8  EB12              jmp short 0x12bc
000012AA  A0AA37            mov al,[0x37aa]
000012AD  98                cbw
000012AE  8BD8              mov bx,ax
000012B0  8A874002          mov al,[bx+0x240]
000012B4  B400              mov ah,0x0
000012B6  8946FE            mov [bp-0x2],ax
000012B9  BF0500            mov di,0x5
000012BC  A0AA37            mov al,[0x37aa]
000012BF  98                cbw
000012C0  D1E0              shl ax,1
000012C2  8BD8              mov bx,ax
000012C4  8B87DA2B          mov ax,[bx+0x2bda]
000012C8  058800            add ax,0x88
000012CB  2B4606            sub ax,[bp+0x6]
000012CE  8BF0              mov si,ax
000012D0  807E1001          cmp byte [bp+0x10],0x1
000012D4  753E              jnz 0x1314
000012D6  FF76FE            push word [bp-0x2]
000012D9  FF760A            push word [bp+0xa]
000012DC  56                push si
000012DD  A0AA37            mov al,[0x37aa]
000012E0  98                cbw
000012E1  D1E0              shl ax,1
000012E3  8BD8              mov bx,ax
000012E5  8B87D02B          mov ax,[bx+0x2bd0]
000012E9  034608            add ax,[bp+0x8]
000012EC  50                push ax
000012ED  9A0B00690A        call 0xa69:0xb
000012F2  83C408            add sp,byte +0x8
000012F5  57                push di
000012F6  FF760E            push word [bp+0xe]
000012F9  56                push si
000012FA  A0AA37            mov al,[0x37aa]
000012FD  98                cbw
000012FE  D1E0              shl ax,1
00001300  8BD8              mov bx,ax
00001302  8B87D02B          mov ax,[bx+0x2bd0]
00001306  03460C            add ax,[bp+0xc]
00001309  50                push ax
0000130A  9A0B00690A        call 0xa69:0xb
0000130F  83C408            add sp,byte +0x8
00001312  EB33              jmp short 0x1347
00001314  56                push si
00001315  A0AA37            mov al,[0x37aa]
00001318  98                cbw
00001319  D1E0              shl ax,1
0000131B  8BD8              mov bx,ax
0000131D  8B87D02B          mov ax,[bx+0x2bd0]
00001321  034608            add ax,[bp+0x8]
00001324  50                push ax
00001325  B80100            mov ax,0x1
00001328  50                push ax
00001329  FF760A            push word [bp+0xa]
0000132C  B88800            mov ax,0x88
0000132F  2B4606            sub ax,[bp+0x6]
00001332  50                push ax
00001333  A0AA37            mov al,[0x37aa]
00001336  98                cbw
00001337  B106              mov cl,0x6
00001339  D3E0              shl ax,cl
0000133B  034608            add ax,[bp+0x8]
0000133E  50                push ax
0000133F  9A3000960A        call 0xa96:0x30
00001344  83C40C            add sp,byte +0xc
00001347  5F                pop di
00001348  5E                pop si
00001349  8BE5              mov sp,bp
0000134B  5D                pop bp
0000134C  CB                retf

; ---- 0C29:134D ----
0000134D  55                push bp
0000134E  8BEC              mov bp,sp
00001350  56                push si
00001351  A0AA37            mov al,[0x37aa]
00001354  98                cbw
00001355  8BD8              mov bx,ax
00001357  80BFB24300        cmp byte [bx+0x43b2],0x0
0000135C  7505              jnz 0x1363
0000135E  B81700            mov ax,0x17
00001361  EB03              jmp short 0x1366
00001363  B88000            mov ax,0x80
00001366  8BF0              mov si,ax
00001368  833E0E0901        cmp word [0x90e],byte +0x1
0000136D  7505              jnz 0x1374
0000136F  C6064206DA        mov byte [0x642],0xda
00001374  A0AA37            mov al,[0x37aa]
00001377  98                cbw
00001378  8BD8              mov bx,ax
0000137A  8A874006          mov al,[bx+0x640]
0000137E  B400              mov ah,0x0
00001380  50                push ax
00001381  A0AA37            mov al,[0x37aa]
00001384  98                cbw
00001385  D1E0              shl ax,1
00001387  8BD8              mov bx,ax
00001389  FFB73406          push word [bx+0x634]
0000138D  56                push si
0000138E  A0AA37            mov al,[0x37aa]
00001391  98                cbw
00001392  D1E0              shl ax,1
00001394  8BD8              mov bx,ax
00001396  FFB7D02B          push word [bx+0x2bd0]
0000139A  9A6B14290C        call 0xc29:0x146b
0000139F  83C408            add sp,byte +0x8
000013A2  833ED04503        cmp word [0x45d0],byte +0x3
000013A7  745B              jz 0x1404
000013A9  A0AA37            mov al,[0x37aa]
000013AC  98                cbw
000013AD  D1E0              shl ax,1
000013AF  D1E0              shl ax,1
000013B1  8BD8              mov bx,ax
000013B3  83BFB84300        cmp word [bx+0x43b8],byte +0x0
000013B8  7C4A              jl 0x1404
000013BA  7F07              jg 0x13c3
000013BC  83BFB64300        cmp word [bx+0x43b6],byte +0x0
000013C1  7641              jna 0x1404
000013C3  A0AA37            mov al,[0x37aa]
000013C6  98                cbw
000013C7  8BD8              mov bx,ax
000013C9  8A873C06          mov al,[bx+0x63c]
000013CD  B400              mov ah,0x0
000013CF  50                push ax
000013D0  33C0              xor ax,ax
000013D2  BA0A00            mov dx,0xa
000013D5  50                push ax
000013D6  52                push dx
000013D7  A0AA37            mov al,[0x37aa]
000013DA  98                cbw
000013DB  D1E0              shl ax,1
000013DD  D1E0              shl ax,1
000013DF  8BD8              mov bx,ax
000013E1  FFB7B843          push word [bx+0x43b8]
000013E5  FFB7B643          push word [bx+0x43b6]
000013E9  9A894F0000        call 0x0:0x4f89
000013EE  50                push ax
000013EF  56                push si
000013F0  A0AA37            mov al,[0x37aa]
000013F3  98                cbw
000013F4  D1E0              shl ax,1
000013F6  8BD8              mov bx,ax
000013F8  FFB7D02B          push word [bx+0x2bd0]
000013FC  9A6B14290C        call 0xc29:0x146b
00001401  83C408            add sp,byte +0x8
00001404  B80300            mov ax,0x3
00001407  50                push ax
00001408  A0AA37            mov al,[0x37aa]
0000140B  98                cbw
0000140C  D1E0              shl ax,1
0000140E  D1E0              shl ax,1
00001410  D1E0              shl ax,1
00001412  8B16D045          mov dx,[0x45d0]
00001416  D1E2              shl dx,1
00001418  03C2              add ax,dx
0000141A  8BD8              mov bx,ax
0000141C  8B87F43C          mov ax,[bx+0x3cf4]
00001420  BB0A00            mov bx,0xa
00001423  99                cwd
00001424  F7FB              idiv bx
00001426  50                push ax
00001427  56                push si
00001428  A0AA37            mov al,[0x37aa]
0000142B  98                cbw
0000142C  D1E0              shl ax,1
0000142E  8BD8              mov bx,ax
00001430  FFB7D02B          push word [bx+0x2bd0]
00001434  9A6B14290C        call 0xc29:0x146b
00001439  83C408            add sp,byte +0x8
0000143C  A0AA37            mov al,[0x37aa]
0000143F  98                cbw
00001440  D1E0              shl ax,1
00001442  D1E0              shl ax,1
00001444  D1E0              shl ax,1
00001446  8B16D045          mov dx,[0x45d0]
0000144A  D1E2              shl dx,1
0000144C  03C2              add ax,dx
0000144E  8BD8              mov bx,ax
00001450  8B87F43C          mov ax,[bx+0x3cf4]
00001454  BB0A00            mov bx,0xa
00001457  99                cwd
00001458  F7FB              idiv bx
0000145A  50                push ax
0000145B  A0AA37            mov al,[0x37aa]
0000145E  98                cbw
0000145F  D1E0              shl ax,1
00001461  8BD8              mov bx,ax
00001463  58                pop ax
00001464  89873406          mov [bx+0x634],ax
00001468  5E                pop si
00001469  5D                pop bp
0000146A  CB                retf

; ---- 0C29:146B ----
0000146B  55                push bp
0000146C  8BEC              mov bp,sp
0000146E  56                push si
0000146F  8B760A            mov si,[bp+0xa]
00001472  FF760C            push word [bp+0xc]
00001475  8A84CF05          mov al,[si+0x5cf]
00001479  B400              mov ah,0x0
0000147B  8B5608            mov dx,[bp+0x8]
0000147E  03D0              add dx,ax
00001480  52                push dx
00001481  8A846A05          mov al,[si+0x56a]
00001485  B400              mov ah,0x0
00001487  8B5606            mov dx,[bp+0x6]
0000148A  03D0              add dx,ax
0000148C  83C204            add dx,byte +0x4
0000148F  52                push dx
00001490  8B4608            mov ax,[bp+0x8]
00001493  051300            add ax,0x13
00001496  50                push ax
00001497  8B4606            mov ax,[bp+0x6]
0000149A  052000            add ax,0x20
0000149D  50                push ax
0000149E  9A                db 0x9a
0000149F  33                db 0x33
