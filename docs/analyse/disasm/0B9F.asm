; segment 0B9F  file offset 0xEFF0  size 0x8A0
00000000  4F                dec di
00000001  03558B            add dx,[di-0x75]
00000004  EC                in al,dx
00000005  9AA0069F0B        call 0xb9f:0x6a0
0000000A  9A08001D14        call 0x141d:0x8
0000000F  B02F              mov al,0x2f
00000011  50                push ax
00000012  9A62009522        call 0x2295:0x62
00000017  59                pop cx
00000018  33C0              xor ax,ax
0000001A  50                push ax
0000001B  B80100            mov ax,0x1
0000001E  50                push ax
0000001F  9A0100290C        call 0xc29:0x1
00000024  59                pop cx
00000025  59                pop cx
00000026  B80100            mov ax,0x1
00000029  50                push ax
0000002A  33C0              xor ax,ax
0000002C  50                push ax
0000002D  9A0100290C        call 0xc29:0x1
00000032  59                pop cx
00000033  59                pop cx
00000034  33C0              xor ax,ax
00000036  50                push ax
00000037  B80200            mov ax,0x2
0000003A  50                push ax
0000003B  9A0100290C        call 0xc29:0x1
00000040  59                pop cx
00000041  59                pop cx
00000042  B80100            mov ax,0x1
00000045  50                push ax
00000046  B80300            mov ax,0x3
00000049  50                push ax
0000004A  9A0100290C        call 0xc29:0x1
0000004F  59                pop cx
00000050  59                pop cx
00000051  B020              mov al,0x20
00000053  50                push ax
00000054  9A62009522        call 0x2295:0x62
00000059  59                pop cx
0000005A  5D                pop bp
0000005B  CB                retf

; ---- 0B9F:005C ----
0000005C  55                push bp
0000005D  8BEC              mov bp,sp
0000005F  C606AA3700        mov byte [0x37aa],0x0
00000064  EB0F              jmp short 0x75
00000066  A0AA37            mov al,[0x37aa]
00000069  98                cbw
0000006A  8BD8              mov bx,ax
0000006C  C687C62B00        mov byte [bx+0x2bc6],0x0
00000071  FE06AA37          inc byte [0x37aa]
00000075  803EAA3704        cmp byte [0x37aa],0x4
0000007A  7CEA              jl 0x66
0000007C  C606AE3C00        mov byte [0x3cae],0x0
00000081  C606183D00        mov byte [0x3d18],0x0
00000086  8B1ED045          mov bx,[0x45d0]
0000008A  D1E3              shl bx,1
0000008C  8B879848          mov ax,[bx+0x4898]
00000090  A35A45            mov [0x455a],ax
00000093  A3C22B            mov [0x2bc2],ax
00000096  C706BE2B0000      mov word [0x2bbe],0x0
0000009C  5D                pop bp
0000009D  CB                retf

; ---- 0B9F:009E ----
0000009E  55                push bp
0000009F  8BEC              mov bp,sp
000000A1  56                push si
000000A2  A1D045            mov ax,[0x45d0]
000000A5  40                inc ax
000000A6  8BF0              mov si,ax
000000A8  3D0300            cmp ax,0x3
000000AB  7502              jnz 0xaf
000000AD  33F6              xor si,si
000000AF  C606AA3700        mov byte [0x37aa],0x0
000000B4  EB3E              jmp short 0xf4
000000B6  833ED04503        cmp word [0x45d0],byte +0x3
000000BB  7505              jnz 0xc2
000000BD  B86400            mov ax,0x64
000000C0  EB19              jmp short 0xdb
000000C2  A0AA37            mov al,[0x37aa]
000000C5  98                cbw
000000C6  D1E0              shl ax,1
000000C8  D1E0              shl ax,1
000000CA  8BD8              mov bx,ax
000000CC  8A80E72E          mov al,[bx+si+0x2ee7]
000000D0  98                cbw
000000D1  50                push ax
000000D2  A0262F            mov al,[0x2f26]
000000D5  98                cbw
000000D6  8BD0              mov dx,ax
000000D8  58                pop ax
000000D9  F7EA              imul dx
000000DB  99                cwd
000000DC  50                push ax
000000DD  A0AA37            mov al,[0x37aa]
000000E0  98                cbw
000000E1  D1E0              shl ax,1
000000E3  D1E0              shl ax,1
000000E5  8BD8              mov bx,ax
000000E7  58                pop ax
000000E8  8987B643          mov [bx+0x43b6],ax
000000EC  8997B843          mov [bx+0x43b8],dx
000000F0  FE06AA37          inc byte [0x37aa]
000000F4  803EAA3704        cmp byte [0x37aa],0x4
000000F9  7CBB              jl 0xb6
000000FB  5E                pop si
000000FC  5D                pop bp
000000FD  CB                retf

; ---- 0B9F:00FE ----
000000FE  55                push bp
000000FF  8BEC              mov bp,sp
00000101  9A7A039E0E        call 0xe9e:0x37a
00000106  C606AA3700        mov byte [0x37aa],0x0
0000010B  E9C700            jmp 0x1d5
0000010E  A0AA37            mov al,[0x37aa]
00000111  98                cbw
00000112  8BD8              mov bx,ax
00000114  80BF400801        cmp byte [bx+0x840],0x1
00000119  7505              jnz 0x120
0000011B  9A97031D14        call 0x141d:0x397
00000120  A0AA37            mov al,[0x37aa]
00000123  98                cbw
00000124  8BD8              mov bx,ax
00000126  8A870833          mov al,[bx+0x3308]
0000012A  98                cbw
0000012B  3D0200            cmp ax,0x2
0000012E  7446              jz 0x176
00000130  3D0300            cmp ax,0x3
00000133  7403              jz 0x138
00000135  E99900            jmp 0x1d1
00000138  A0AA37            mov al,[0x37aa]
0000013B  98                cbw
0000013C  8BD8              mov bx,ax
0000013E  80BFB24300        cmp byte [bx+0x43b2],0x0
00000143  7503              jnz 0x148
00000145  E98900            jmp 0x1d1
00000148  B81900            mov ax,0x19
0000014B  50                push ax
0000014C  33C0              xor ax,ax
0000014E  50                push ax
0000014F  9A5400A721        call 0x21a7:0x54
00000154  59                pop cx
00000155  59                pop cx
00000156  9A680F290C        call 0xc29:0xf68
0000015B  A0AA37            mov al,[0x37aa]
0000015E  98                cbw
0000015F  8BD8              mov bx,ax
00000161  C687B24300        mov byte [bx+0x43b2],0x0
00000166  A0AA37            mov al,[0x37aa]
00000169  98                cbw
0000016A  D1E0              shl ax,1
0000016C  8BD8              mov bx,ax
0000016E  C787A243FDFF      mov word [bx+0x43a2],0xfffd
00000174  EB5B              jmp short 0x1d1
00000176  A0AA37            mov al,[0x37aa]
00000179  98                cbw
0000017A  8BD8              mov bx,ax
0000017C  80BFB24301        cmp byte [bx+0x43b2],0x1
00000181  7502              jnz 0x185
00000183  EB4C              jmp short 0x1d1
00000185  A0AA37            mov al,[0x37aa]
00000188  98                cbw
00000189  D1E0              shl ax,1
0000018B  D1E0              shl ax,1
0000018D  D1E0              shl ax,1
0000018F  8B16D045          mov dx,[0x45d0]
00000193  D1E2              shl dx,1
00000195  03C2              add ax,dx
00000197  8BD8              mov bx,ax
00000199  83BFF43C0A        cmp word [bx+0x3cf4],byte +0xa
0000019E  7D02              jnl 0x1a2
000001A0  EB2F              jmp short 0x1d1
000001A2  B81800            mov ax,0x18
000001A5  50                push ax
000001A6  B80100            mov ax,0x1
000001A9  50                push ax
000001AA  9A5400A721        call 0x21a7:0x54
000001AF  59                pop cx
000001B0  59                pop cx
000001B1  9A680F290C        call 0xc29:0xf68
000001B6  A0AA37            mov al,[0x37aa]
000001B9  98                cbw
000001BA  8BD8              mov bx,ax
000001BC  C687B24301        mov byte [bx+0x43b2],0x1
000001C1  A0AA37            mov al,[0x37aa]
000001C4  98                cbw
000001C5  D1E0              shl ax,1
000001C7  8BD8              mov bx,ax
000001C9  C787A2436700      mov word [bx+0x43a2],0x67
000001CF  EB00              jmp short 0x1d1
000001D1  FE06AA37          inc byte [0x37aa]
000001D5  803EAA3704        cmp byte [0x37aa],0x4
000001DA  7D03              jnl 0x1df
000001DC  E92FFF            jmp 0x10e
000001DF  5D                pop bp
000001E0  CB                retf

; ---- 0B9F:01E1 ----
000001E1  55                push bp
000001E2  8BEC              mov bp,sp
000001E4  C706BC2B0000      mov word [0x2bbc],0x0
000001EA  C6069C4301        mov byte [0x439c],0x1
000001EF  C6060C4501        mov byte [0x450c],0x1
000001F4  C606AA3700        mov byte [0x37aa],0x0
000001F9  EB46              jmp short 0x241
000001FB  A0AA37            mov al,[0x37aa]
000001FE  98                cbw
000001FF  8BD8              mov bx,ax
00000201  80BFB24301        cmp byte [bx+0x43b2],0x1
00000206  7516              jnz 0x21e
00000208  FF06BC2B          inc word [0x2bbc]
0000020C  A0AA37            mov al,[0x37aa]
0000020F  98                cbw
00000210  8BD8              mov bx,ax
00000212  80BF400800        cmp byte [bx+0x840],0x0
00000217  7505              jnz 0x21e
00000219  C6060C4500        mov byte [0x450c],0x0
0000021E  A0AA37            mov al,[0x37aa]
00000221  98                cbw
00000222  8BD8              mov bx,ax
00000224  80BFB24300        cmp byte [bx+0x43b2],0x0
00000229  7512              jnz 0x23d
0000022B  A0AA37            mov al,[0x37aa]
0000022E  98                cbw
0000022F  8BD8              mov bx,ax
00000231  80BF400800        cmp byte [bx+0x840],0x0
00000236  7505              jnz 0x23d
00000238  C6069C4300        mov byte [0x439c],0x0
0000023D  FE06AA37          inc byte [0x37aa]
00000241  803EAA3704        cmp byte [0x37aa],0x4
00000246  7CB3              jl 0x1fb
00000248  5D                pop bp
00000249  CB                retf

; ---- 0B9F:024A ----
0000024A  55                push bp
0000024B  8BEC              mov bp,sp
0000024D  56                push si
0000024E  33F6              xor si,si
00000250  C606AA3700        mov byte [0x37aa],0x0
00000255  EB41              jmp short 0x298
00000257  A0AA37            mov al,[0x37aa]
0000025A  98                cbw
0000025B  8BD8              mov bx,ax
0000025D  80BFB24301        cmp byte [bx+0x43b2],0x1
00000262  7530              jnz 0x294
00000264  A0AA37            mov al,[0x37aa]
00000267  98                cbw
00000268  D1E0              shl ax,1
0000026A  D1E0              shl ax,1
0000026C  D1E0              shl ax,1
0000026E  8B16D045          mov dx,[0x45d0]
00000272  D1E2              shl dx,1
00000274  03C2              add ax,dx
00000276  8BD8              mov bx,ax
00000278  8B87F43C          mov ax,[bx+0x3cf4]
0000027C  50                push ax
0000027D  A18E2E            mov ax,[0x2e8e]
00000280  BA0500            mov dx,0x5
00000283  F7EA              imul dx
00000285  050F00            add ax,0xf
00000288  8BD8              mov bx,ax
0000028A  58                pop ax
0000028B  99                cwd
0000028C  F7FB              idiv bx
0000028E  8BD6              mov dx,si
00000290  03D0              add dx,ax
00000292  8BF2              mov si,dx
00000294  FE06AA37          inc byte [0x37aa]
00000298  803EAA3704        cmp byte [0x37aa],0x4
0000029D  7CB8              jl 0x257
0000029F  83FE0F            cmp si,byte +0xf
000002A2  7D03              jnl 0x2a7
000002A4  BE0F00            mov si,0xf
000002A7  83FE78            cmp si,byte +0x78
000002AA  7E03              jng 0x2af
000002AC  BE7800            mov si,0x78
000002AF  8BC6              mov ax,si
000002B1  05FBFF            add ax,0xfffb
000002B4  A3BA2B            mov [0x2bba],ax
000002B7  8BC6              mov ax,si
000002B9  EB00              jmp short 0x2bb
000002BB  5E                pop si
000002BC  5D                pop bp
000002BD  CB                retf

; ---- 0B9F:02BE ----
000002BE  55                push bp
000002BF  8BEC              mov bp,sp
000002C1  C606C02B01        mov byte [0x2bc0],0x1
000002C6  C606B82B00        mov byte [0x2bb8],0x0
000002CB  C606AA3700        mov byte [0x37aa],0x0
000002D0  EB39              jmp short 0x30b
000002D2  A0AA37            mov al,[0x37aa]
000002D5  98                cbw
000002D6  8BD8              mov bx,ax
000002D8  80BFB24300        cmp byte [bx+0x43b2],0x0
000002DD  7528              jnz 0x307
000002DF  A0AA37            mov al,[0x37aa]
000002E2  98                cbw
000002E3  D1E0              shl ax,1
000002E5  8BD8              mov bx,ax
000002E7  8B87A243          mov ax,[bx+0x43a2]
000002EB  3B061C3D          cmp ax,[0x3d1c]
000002EF  7516              jnz 0x307
000002F1  FE06B82B          inc byte [0x2bb8]
000002F5  A0AA37            mov al,[0x37aa]
000002F8  98                cbw
000002F9  8BD8              mov bx,ax
000002FB  80BF400800        cmp byte [bx+0x840],0x0
00000300  7505              jnz 0x307
00000302  C606C02B00        mov byte [0x2bc0],0x0
00000307  FE06AA37          inc byte [0x37aa]
0000030B  803EAA3704        cmp byte [0x37aa],0x4
00000310  7CC0              jl 0x2d2
00000312  803EB82B00        cmp byte [0x2bb8],0x0
00000317  7506              jnz 0x31f
00000319  A1C22B            mov ax,[0x2bc2]
0000031C  A35A45            mov [0x455a],ax
0000031F  5D                pop bp
00000320  CB                retf

; ---- 0B9F:0321 ----
00000321  55                push bp
00000322  8BEC              mov bp,sp
00000324  56                push si
00000325  A0AA37            mov al,[0x37aa]
00000328  98                cbw
00000329  D1E0              shl ax,1
0000032B  8BD8              mov bx,ax
0000032D  8BB7A243          mov si,[bx+0x43a2]
00000331  A0AA37            mov al,[0x37aa]
00000334  98                cbw
00000335  8BD8              mov bx,ax
00000337  80BFB24300        cmp byte [bx+0x43b2],0x0
0000033C  7403              jz 0x341
0000033E  E9FD00            jmp 0x43e
00000341  8BC6              mov ax,si
00000343  BB0200            mov bx,0x2
00000346  99                cwd
00000347  F7FB              idiv bx
00000349  03065A45          add ax,[0x455a]
0000034D  99                cwd
0000034E  50                push ax
0000034F  A0AA37            mov al,[0x37aa]
00000352  98                cbw
00000353  D1E0              shl ax,1
00000355  D1E0              shl ax,1
00000357  8BD8              mov bx,ax
00000359  3B97D43C          cmp dx,[bx+0x3cd4]
0000035D  58                pop ax
0000035E  7F24              jg 0x384
00000360  7506              jnz 0x368
00000362  3B87D23C          cmp ax,[bx+0x3cd2]
00000366  771C              ja 0x384
00000368  A0AA37            mov al,[0x37aa]
0000036B  98                cbw
0000036C  D1E0              shl ax,1
0000036E  D1E0              shl ax,1
00000370  D1E0              shl ax,1
00000372  8B16D045          mov dx,[0x45d0]
00000376  D1E2              shl dx,1
00000378  03C2              add ax,dx
0000037A  8BD8              mov bx,ax
0000037C  81BFF43CDE03      cmp word [bx+0x3cf4],0x3de
00000382  7C0B              jl 0x38f
00000384  A0AA37            mov al,[0x37aa]
00000387  98                cbw
00000388  8BD8              mov bx,ax
0000038A  C687083303        mov byte [bx+0x3308],0x3
0000038F  A0AA37            mov al,[0x37aa]
00000392  98                cbw
00000393  8BD8              mov bx,ax
00000395  8A870833          mov al,[bx+0x3308]
00000399  98                cbw
0000039A  3DFFFF            cmp ax,0xffff
0000039D  740D              jz 0x3ac
0000039F  3D0200            cmp ax,0x2
000003A2  7411              jz 0x3b5
000003A4  3D0300            cmp ax,0x3
000003A7  7478              jz 0x421
000003A9  E98F00            jmp 0x43b
000003AC  F7C60100          test si,0x1
000003B0  7403              jz 0x3b5
000003B2  E98600            jmp 0x43b
000003B5  3B36EE43          cmp si,[0x43ee]
000003B9  7502              jnz 0x3bd
000003BB  EB7E              jmp short 0x43b
000003BD  8BC6              mov ax,si
000003BF  BB0200            mov bx,0x2
000003C2  99                cwd
000003C3  F7FB              idiv bx
000003C5  03065A45          add ax,[0x455a]
000003C9  99                cwd
000003CA  50                push ax
000003CB  A0AA37            mov al,[0x37aa]
000003CE  98                cbw
000003CF  D1E0              shl ax,1
000003D1  D1E0              shl ax,1
000003D3  8BD8              mov bx,ax
000003D5  3B97D43C          cmp dx,[bx+0x3cd4]
000003D9  58                pop ax
000003DA  7506              jnz 0x3e2
000003DC  3B87D23C          cmp ax,[bx+0x3cd2]
000003E0  7411              jz 0x3f3
000003E2  8BC6              mov ax,si
000003E4  BB0200            mov bx,0x2
000003E7  99                cwd
000003E8  F7FB              idiv bx
000003EA  03065A45          add ax,[0x455a]
000003EE  3DE703            cmp ax,0x3e7
000003F1  7502              jnz 0x3f5
000003F3  EB46              jmp short 0x43b
000003F5  A0AA37            mov al,[0x37aa]
000003F8  98                cbw
000003F9  D1E0              shl ax,1
000003FB  8BD8              mov bx,ax
000003FD  8B87A243          mov ax,[bx+0x43a2]
00000401  40                inc ax
00000402  50                push ax
00000403  A0AA37            mov al,[0x37aa]
00000406  98                cbw
00000407  D1E0              shl ax,1
00000409  8BD8              mov bx,ax
0000040B  58                pop ax
0000040C  8987A243          mov [bx+0x43a2],ax
00000410  3D6300            cmp ax,0x63
00000413  7E05              jng 0x41a
00000415  9AFC049F0B        call 0xb9f:0x4fc
0000041A  C606C42B01        mov byte [0x2bc4],0x1
0000041F  EB1A              jmp short 0x43b
00000421  83FEFD            cmp si,byte -0x3
00000424  7F02              jg 0x428
00000426  EB13              jmp short 0x43b
00000428  A0AA37            mov al,[0x37aa]
0000042B  98                cbw
0000042C  D1E0              shl ax,1
0000042E  8BD8              mov bx,ax
00000430  FF8FA243          dec word [bx+0x43a2]
00000434  C606C42B01        mov byte [0x2bc4],0x1
00000439  EB00              jmp short 0x43b
0000043B  E99E00            jmp 0x4dc
0000043E  A0AA37            mov al,[0x37aa]
00000441  98                cbw
00000442  8BD8              mov bx,ax
00000444  80BFB24301        cmp byte [bx+0x43b2],0x1
00000449  7403              jz 0x44e
0000044B  E98E00            jmp 0x4dc
0000044E  A0AA37            mov al,[0x37aa]
00000451  98                cbw
00000452  D1E0              shl ax,1
00000454  D1E0              shl ax,1
00000456  D1E0              shl ax,1
00000458  8B16D045          mov dx,[0x45d0]
0000045C  D1E2              shl dx,1
0000045E  03C2              add ax,dx
00000460  8BD8              mov bx,ax
00000462  83BFF43C0A        cmp word [bx+0x3cf4],byte +0xa
00000467  7D0B              jnl 0x474
00000469  A0AA37            mov al,[0x37aa]
0000046C  98                cbw
0000046D  8BD8              mov bx,ax
0000046F  C68708330A        mov byte [bx+0x3308],0xa
00000474  A0AA37            mov al,[0x37aa]
00000477  98                cbw
00000478  8BD8              mov bx,ax
0000047A  8A870833          mov al,[bx+0x3308]
0000047E  98                cbw
0000047F  40                inc ax
00000480  8BD8              mov bx,ax
00000482  83FB0B            cmp bx,byte +0xb
00000485  7755              ja 0x4dc
00000487  D1E3              shl bx,1
00000489  2EFFA7E404        jmp [cs:bx+0x4e4]
0000048E  F7C60100          test si,0x1
00000492  7502              jnz 0x496
00000494  EB46              jmp short 0x4dc
00000496  0BF6              or si,si
00000498  7E06              jng 0x4a0
0000049A  3B361C3D          cmp si,[0x3d1c]
0000049E  7502              jnz 0x4a2
000004A0  EB3A              jmp short 0x4dc
000004A2  A0AA37            mov al,[0x37aa]
000004A5  98                cbw
000004A6  D1E0              shl ax,1
000004A8  8BD8              mov bx,ax
000004AA  FF8FA243          dec word [bx+0x43a2]
000004AE  C606C42B01        mov byte [0x2bc4],0x1
000004B3  EB27              jmp short 0x4dc
000004B5  83FE67            cmp si,byte +0x67
000004B8  7C02              jl 0x4bc
000004BA  EB20              jmp short 0x4dc
000004BC  A0AA37            mov al,[0x37aa]
000004BF  98                cbw
000004C0  D1E0              shl ax,1
000004C2  8BD8              mov bx,ax
000004C4  FF87A243          inc word [bx+0x43a2]
000004C8  C606C42B01        mov byte [0x2bc4],0x1
000004CD  EB0D              jmp short 0x4dc
000004CF  A0AA37            mov al,[0x37aa]
000004D2  98                cbw
000004D3  8BD8              mov bx,ax
000004D5  C687C62B01        mov byte [bx+0x2bc6],0x1
000004DA  EB00              jmp short 0x4dc
000004DC  9A86059F0B        call 0xb9f:0x586
000004E1  5E                pop si
000004E2  5D                pop bp
000004E3  CB                retf
000004E4  8E04              mov es,[si]
000004E6  DC04              fadd qword [si]
000004E8  DC04              fadd qword [si]
000004EA  B504              mov ch,0x4
000004EC  96                xchg ax,si
000004ED  04DC              add al,0xdc
000004EF  04DC              add al,0xdc
000004F1  04DC              add al,0xdc
000004F3  04DC              add al,0xdc
000004F5  04DC              add al,0xdc
000004F7  04DC              add al,0xdc
000004F9  04CF              add al,0xcf
000004FB  0455              add al,0x55
000004FD  8BEC              mov bp,sp
000004FF  56                push si
00000500  33F6              xor si,si
00000502  EB28              jmp short 0x52c
00000504  80BCB24300        cmp byte [si+0x43b2],0x0
00000509  7520              jnz 0x52b
0000050B  8BDE              mov bx,si
0000050D  D1E3              shl bx,1
0000050F  8B87A243          mov ax,[bx+0x43a2]
00000513  48                dec ax
00000514  8BDE              mov bx,si
00000516  D1E3              shl bx,1
00000518  8987A243          mov [bx+0x43a2],ax
0000051C  3DFDFF            cmp ax,0xfffd
0000051F  7D0A              jnl 0x52b
00000521  8BDE              mov bx,si
00000523  D1E3              shl bx,1
00000525  C787A243FDFF      mov word [bx+0x43a2],0xfffd
0000052B  46                inc si
0000052C  83FE04            cmp si,byte +0x4
0000052F  7CD3              jl 0x504
00000531  A13C02            mov ax,[0x23c]
00000534  48                dec ax
00000535  A33C02            mov [0x23c],ax
00000538  0BC0              or ax,ax
0000053A  750A              jnz 0x546
0000053C  FF065A45          inc word [0x455a]
00000540  C7063C020200      mov word [0x23c],0x2
00000546  5E                pop si
00000547  5D                pop bp
00000548  CB                retf

; ---- 0B9F:0549 ----
00000549  55                push bp
0000054A  8BEC              mov bp,sp
0000054C  803EC42B00        cmp byte [0x2bc4],0x0
00000551  7516              jnz 0x569
00000553  803E3E0201        cmp byte [0x23e],0x1
00000558  750D              jnz 0x567
0000055A  33C0              xor ax,ax
0000055C  50                push ax
0000055D  33C0              xor ax,ax
0000055F  50                push ax
00000560  9A5400A721        call 0x21a7:0x54
00000565  59                pop cx
00000566  59                pop cx
00000567  EB15              jmp short 0x57e
00000569  803E3E0200        cmp byte [0x23e],0x0
0000056E  750E              jnz 0x57e
00000570  B81A00            mov ax,0x1a
00000573  50                push ax
00000574  33C0              xor ax,ax
00000576  50                push ax
00000577  9A5400A721        call 0x21a7:0x54
0000057C  59                pop cx
0000057D  59                pop cx
0000057E  A0C42B            mov al,[0x2bc4]
00000581  A23E02            mov [0x23e],al
00000584  5D                pop bp
00000585  CB                retf

; ---- 0B9F:0586 ----
00000586  55                push bp
00000587  8BEC              mov bp,sp
00000589  56                push si
0000058A  57                push di
0000058B  A11C3D            mov ax,[0x3d1c]
0000058E  8BD0              mov dx,ax
00000590  8B3EEE43          mov di,[0x43ee]
00000594  C7061C3D0000      mov word [0x3d1c],0x0
0000059A  C706EE436600      mov word [0x43ee],0x66
000005A0  33F6              xor si,si
000005A2  EB4A              jmp short 0x5ee
000005A4  80BCB24300        cmp byte [si+0x43b2],0x0
000005A9  751B              jnz 0x5c6
000005AB  8BDE              mov bx,si
000005AD  D1E3              shl bx,1
000005AF  8B87A243          mov ax,[bx+0x43a2]
000005B3  3B061C3D          cmp ax,[0x3d1c]
000005B7  7E0B              jng 0x5c4
000005B9  8BDE              mov bx,si
000005BB  D1E3              shl bx,1
000005BD  8B87A243          mov ax,[bx+0x43a2]
000005C1  A31C3D            mov [0x3d1c],ax
000005C4  EB27              jmp short 0x5ed
000005C6  80BCB24301        cmp byte [si+0x43b2],0x1
000005CB  7520              jnz 0x5ed
000005CD  80BCC62B00        cmp byte [si+0x2bc6],0x0
000005D2  7519              jnz 0x5ed
000005D4  8BDE              mov bx,si
000005D6  D1E3              shl bx,1
000005D8  8B87A243          mov ax,[bx+0x43a2]
000005DC  3B06EE43          cmp ax,[0x43ee]
000005E0  7D0B              jnl 0x5ed
000005E2  8BDE              mov bx,si
000005E4  D1E3              shl bx,1
000005E6  8B87A243          mov ax,[bx+0x43a2]
000005EA  A3EE43            mov [0x43ee],ax
000005ED  46                inc si
000005EE  83FE03            cmp si,byte +0x3
000005F1  7EB1              jng 0x5a4
000005F3  8BC2              mov ax,dx
000005F5  3B061C3D          cmp ax,[0x3d1c]
000005F9  7506              jnz 0x601
000005FB  3B3EEE43          cmp di,[0x43ee]
000005FF  7405              jz 0x606
00000601  C606183D03        mov byte [0x3d18],0x3
00000606  5F                pop di
00000607  5E                pop si
00000608  5D                pop bp
00000609  CB                retf

; ---- 0B9F:060A ----
0000060A  55                push bp
0000060B  8BEC              mov bp,sp
0000060D  56                push si
0000060E  33F6              xor si,si
00000610  EB36              jmp short 0x648
00000612  A1B42B            mov ax,[0x2bb4]
00000615  40                inc ax
00000616  A3B42B            mov [0x2bb4],ax
00000619  3D0300            cmp ax,0x3
0000061C  7E06              jng 0x624
0000061E  C706B42B0000      mov word [0x2bb4],0x0
00000624  8B1EB42B          mov bx,[0x2bb4]
00000628  80BFB24301        cmp byte [bx+0x43b2],0x1
0000062D  7518              jnz 0x647
0000062F  8B1EB42B          mov bx,[0x2bb4]
00000633  D1E3              shl bx,1
00000635  8B87A243          mov ax,[bx+0x43a2]
00000639  3B06EE43          cmp ax,[0x43ee]
0000063D  7508              jnz 0x647
0000063F  A1B42B            mov ax,[0x2bb4]
00000642  A3F244            mov [0x44f2],ax
00000645  EB06              jmp short 0x64d
00000647  46                inc si
00000648  83FE04            cmp si,byte +0x4
0000064B  7CC5              jl 0x612
0000064D  33F6              xor si,si
0000064F  EB36              jmp short 0x687
00000651  A1B62B            mov ax,[0x2bb6]
00000654  40                inc ax
00000655  A3B62B            mov [0x2bb6],ax
00000658  3D0300            cmp ax,0x3
0000065B  7E06              jng 0x663
0000065D  C706B62B0000      mov word [0x2bb6],0x0
00000663  8B1EB62B          mov bx,[0x2bb6]
00000667  80BFB24300        cmp byte [bx+0x43b2],0x0
0000066C  7518              jnz 0x686
0000066E  8B1EB62B          mov bx,[0x2bb6]
00000672  D1E3              shl bx,1
00000674  8B87A243          mov ax,[bx+0x43a2]
00000678  3B061C3D          cmp ax,[0x3d1c]
0000067C  7508              jnz 0x686
0000067E  A1B62B            mov ax,[0x2bb6]
00000681  A30641            mov [0x4106],ax
00000684  EB0C              jmp short 0x692
00000686  46                inc si
00000687  83FE04            cmp si,byte +0x4
0000068A  7CC5              jl 0x651
0000068C  C70606410A00      mov word [0x4106],0xa
00000692  5E                pop si
00000693  5D                pop bp
00000694  CB                retf

; ---- 0B9F:0695 ----
00000695  55                push bp
00000696  8BEC              mov bp,sp
00000698  A18037            mov ax,[0x3780]
0000069B  A3BA2B            mov [0x2bba],ax
0000069E  5D                pop bp
0000069F  CB                retf

; ---- 0B9F:06A0 ----
000006A0  55                push bp
000006A1  8BEC              mov bp,sp
000006A3  83EC04            sub sp,byte +0x4
000006A6  56                push si
000006A7  57                push di
000006A8  C746FC4042        mov word [bp-0x4],0x4240
000006AD  C746FE0F00        mov word [bp-0x2],0xf
000006B2  C606AA3700        mov byte [0x37aa],0x0
000006B7  EB3A              jmp short 0x6f3
000006B9  A0AA37            mov al,[0x37aa]
000006BC  98                cbw
000006BD  D1E0              shl ax,1
000006BF  D1E0              shl ax,1
000006C1  8BD8              mov bx,ax
000006C3  8B87D43C          mov ax,[bx+0x3cd4]
000006C7  8B97D23C          mov dx,[bx+0x3cd2]
000006CB  3B46FE            cmp ax,[bp-0x2]
000006CE  7F1F              jg 0x6ef
000006D0  7C05              jl 0x6d7
000006D2  3B56FC            cmp dx,[bp-0x4]
000006D5  7318              jnc 0x6ef
000006D7  A0AA37            mov al,[0x37aa]
000006DA  98                cbw
000006DB  D1E0              shl ax,1
000006DD  D1E0              shl ax,1
000006DF  8BD8              mov bx,ax
000006E1  8B87D43C          mov ax,[bx+0x3cd4]
000006E5  8B97D23C          mov dx,[bx+0x3cd2]
000006E9  8956FC            mov [bp-0x4],dx
000006EC  8946FE            mov [bp-0x2],ax
000006EF  FE06AA37          inc byte [0x37aa]
000006F3  803EAA3704        cmp byte [0x37aa],0x4
000006F8  7CBF              jl 0x6b9
000006FA  33C0              xor ax,ax
000006FC  BA3A00            mov dx,0x3a
000006FF  50                push ax
00000700  52                push dx
00000701  33C0              xor ax,ax
00000703  BA1027            mov dx,0x2710
00000706  2B56FC            sub dx,[bp-0x4]
00000709  1B46FE            sbb ax,[bp-0x2]
0000070C  50                push ax
0000070D  52                push dx
0000070E  9A894F0000        call 0x0:0x4f89
00000713  BA2C01            mov dx,0x12c
00000716  2BD0              sub dx,ax
00000718  8BFA              mov di,dx
0000071A  C706D0450000      mov word [0x45d0],0x0
00000720  EB5B              jmp short 0x77d
00000722  833ED04503        cmp word [0x45d0],byte +0x3
00000727  750E              jnz 0x737
00000729  A19A4B            mov ax,[0x4b9a]
0000072C  BB0200            mov bx,0x2
0000072F  99                cwd
00000730  F7FB              idiv bx
00000732  050300            add ax,0x3
00000735  EB10              jmp short 0x747
00000737  8BC7              mov ax,di
00000739  99                cwd
0000073A  52                push dx
0000073B  50                push ax
0000073C  FF76FE            push word [bp-0x2]
0000073F  FF76FC            push word [bp-0x4]
00000742  9A894F0000        call 0x0:0x4f89
00000747  8BF0              mov si,ax
00000749  8B1ED045          mov bx,[0x45d0]
0000074D  D1E3              shl bx,1
0000074F  39B79848          cmp [bx+0x4898],si
00000753  7D0A              jnl 0x75f
00000755  8B1ED045          mov bx,[0x45d0]
00000759  D1E3              shl bx,1
0000075B  89B79848          mov [bx+0x4898],si
0000075F  8B1ED045          mov bx,[0x45d0]
00000763  D1E3              shl bx,1
00000765  81BF98482C01      cmp word [bx+0x4898],0x12c
0000076B  7E0C              jng 0x779
0000076D  8B1ED045          mov bx,[0x45d0]
00000771  D1E3              shl bx,1
00000773  C78798482C01      mov word [bx+0x4898],0x12c
00000779  FF06D045          inc word [0x45d0]
0000077D  833ED04503        cmp word [0x45d0],byte +0x3
00000782  7E9E              jng 0x722
00000784  5F                pop di
00000785  5E                pop si
00000786  8BE5              mov sp,bp
00000788  5D                pop bp
00000789  CB                retf

; ---- 0B9F:078A ----
0000078A  55                push bp
0000078B  8BEC              mov bp,sp
0000078D  56                push si
0000078E  833ED04503        cmp word [0x45d0],byte +0x3
00000793  7510              jnz 0x7a5
00000795  A19A4B            mov ax,[0x4b9a]
00000798  BB0200            mov bx,0x2
0000079B  99                cwd
0000079C  F7FB              idiv bx
0000079E  050300            add ax,0x3
000007A1  8BF0              mov si,ax
000007A3  EB5A              jmp short 0x7ff
000007A5  833EBE2B00        cmp word [0x2bbe],byte +0x0
000007AA  7512              jnz 0x7be
000007AC  A1C22B            mov ax,[0x2bc2]
000007AF  BA0700            mov dx,0x7
000007B2  F7EA              imul dx
000007B4  BB0A00            mov bx,0xa
000007B7  99                cwd
000007B8  F7FB              idiv bx
000007BA  8BF0              mov si,ax
000007BC  EB30              jmp short 0x7ee
000007BE  A1BE2B            mov ax,[0x2bbe]
000007C1  3B06C22B          cmp ax,[0x2bc2]
000007C5  7512              jnz 0x7d9
000007C7  A1C22B            mov ax,[0x2bc2]
000007CA  BA0900            mov dx,0x9
000007CD  F7EA              imul dx
000007CF  BB0A00            mov bx,0xa
000007D2  99                cwd
000007D3  F7FB              idiv bx
000007D5  8BF0              mov si,ax
000007D7  EB15              jmp short 0x7ee
000007D9  A1BE2B            mov ax,[0x2bbe]
000007DC  2B06C22B          sub ax,[0x2bc2]
000007E0  BB0300            mov bx,0x3
000007E3  99                cwd
000007E4  F7FB              idiv bx
000007E6  8B16C22B          mov dx,[0x2bc2]
000007EA  03D0              add dx,ax
000007EC  8BF2              mov si,dx
000007EE  83FE03            cmp si,byte +0x3
000007F1  7D03              jnl 0x7f6
000007F3  BE0300            mov si,0x3
000007F6  81FE2C01          cmp si,0x12c
000007FA  7E03              jng 0x7ff
000007FC  BE2C01            mov si,0x12c
000007FF  8B1ED045          mov bx,[0x45d0]
00000803  D1E3              shl bx,1
00000805  89B79848          mov [bx+0x4898],si
00000809  5E                pop si
0000080A  5D                pop bp
0000080B  CB                retf

; ---- 0B9F:080C ----
0000080C  55                push bp
0000080D  8BEC              mov bp,sp
0000080F  56                push si
00000810  57                push di
00000811  33FF              xor di,di
00000813  9A9005FD21        call 0x21fd:0x590
00000818  33F6              xor si,si
0000081A  EB6D              jmp short 0x889
0000081C  9A1B00960A        call 0xa96:0x1b
00000821  33C0              xor ax,ax
00000823  50                push ax
00000824  B84001            mov ax,0x140
00000827  50                push ax
00000828  56                push si
00000829  33C0              xor ax,ax
0000082B  50                push ax
0000082C  9A0B00690A        call 0xa69:0xb
00000831  83C408            add sp,byte +0x8
00000834  33C0              xor ax,ax
00000836  50                push ax
00000837  B84001            mov ax,0x140
0000083A  50                push ax
0000083B  B8C700            mov ax,0xc7
0000083E  2BC6              sub ax,si
00000840  50                push ax
00000841  33C0              xor ax,ax
00000843  50                push ax
00000844  9A0B00690A        call 0xa69:0xb
00000849  83C408            add sp,byte +0x8
0000084C  83FE31            cmp si,byte +0x31
0000084F  7C1D              jl 0x86e
00000851  8BC6              mov ax,si
00000853  BB0700            mov bx,0x7
00000856  99                cwd
00000857  F7FB              idiv bx
00000859  0BD2              or dx,dx
0000085B  7511              jnz 0x86e
0000085D  8BC7              mov ax,di
0000085F  40                inc ax
00000860  8BF8              mov di,ax
00000862  50                push ax
00000863  B80300            mov ax,0x3
00000866  50                push ax
00000867  9AEB03FD21        call 0x21fd:0x3eb
0000086C  59                pop cx
0000086D  59                pop cx
0000086E  833E0E0901        cmp word [0x90e],byte +0x1
00000873  750E              jnz 0x883
00000875  33C0              xor ax,ax
00000877  BA0100            mov dx,0x1
0000087A  50                push ax
0000087B  52                push dx
0000087C  9A8501FD21        call 0x21fd:0x185
00000881  59                pop cx
00000882  59                pop cx
00000883  9AA806960A        call 0xa96:0x6a8
00000888  46                inc si
00000889  83FE64            cmp si,byte +0x64
0000088C  7C8E              jl 0x81c
0000088E  B80300            mov ax,0x3
00000891  50                push ax
00000892  9ADA04FD21        call 0x21fd:0x4da
00000897  59                pop cx
00000898  9A1F05FD21        call 0x21fd:0x51f
0000089D  5F                pop di
0000089E  5E                pop si
0000089F  5D                pop bp
