; segment 11A3  file offset 0x15030  size 0x1250

; ---- 11A3:0000 ----
00000000  55                push bp
00000001  8BEC              mov bp,sp
00000003  9A7D00A311        call 0x11a3:0x7d
00000008  9A8001A311        call 0x11a3:0x180
0000000D  C606AA3700        mov byte [0x37aa],0x0
00000012  EB1B              jmp short 0x2f
00000014  A0AA37            mov al,[0x37aa]
00000017  98                cbw
00000018  8BD8              mov bx,ax
0000001A  80BF400801        cmp byte [bx+0x840],0x1
0000001F  750A              jnz 0x2b
00000021  9A3F04A311        call 0x11a3:0x43f
00000026  9A9905A311        call 0x11a3:0x599
0000002B  FE06AA37          inc byte [0x37aa]
0000002F  803EAA3704        cmp byte [0x37aa],0x4
00000034  7CDE              jl 0x14
00000036  9A1A06A311        call 0x11a3:0x61a
0000003B  9A7D06A311        call 0x11a3:0x67d
00000040  C606AA3700        mov byte [0x37aa],0x0
00000045  EB2D              jmp short 0x74
00000047  A0AA37            mov al,[0x37aa]
0000004A  98                cbw
0000004B  8BD8              mov bx,ax
0000004D  80BF400801        cmp byte [bx+0x840],0x1
00000052  751C              jnz 0x70
00000054  9A8A07A311        call 0x11a3:0x78a
00000059  807E0600          cmp byte [bp+0x6],0x0
0000005D  750C              jnz 0x6b
0000005F  9A3B0BA311        call 0x11a3:0xb3b
00000064  9A2E0FA311        call 0x11a3:0xf2e
00000069  EB05              jmp short 0x70
0000006B  9A4B10A311        call 0x11a3:0x104b
00000070  FE06AA37          inc byte [0x37aa]
00000074  803EAA3704        cmp byte [0x37aa],0x4
00000079  7CCC              jl 0x47
0000007B  5D                pop bp
0000007C  CB                retf

; ---- 11A3:007D ----
0000007D  55                push bp
0000007E  8BEC              mov bp,sp
00000080  56                push si
00000081  57                push di
00000082  C606AA3700        mov byte [0x37aa],0x0
00000087  EB12              jmp short 0x9b
00000089  A0AA37            mov al,[0x37aa]
0000008C  98                cbw
0000008D  D1E0              shl ax,1
0000008F  8BD8              mov bx,ax
00000091  C787742C0300      mov word [bx+0x2c74],0x3
00000097  FE06AA37          inc byte [0x37aa]
0000009B  803EAA3704        cmp byte [0x37aa],0x4
000000A0  7CE7              jl 0x89
000000A2  C606AA3700        mov byte [0x37aa],0x0
000000A7  EB65              jmp short 0x10e
000000A9  A0AA37            mov al,[0x37aa]
000000AC  98                cbw
000000AD  40                inc ax
000000AE  8BF0              mov si,ax
000000B0  EB53              jmp short 0x105
000000B2  A0AA37            mov al,[0x37aa]
000000B5  98                cbw
000000B6  D1E0              shl ax,1
000000B8  D1E0              shl ax,1
000000BA  D1E0              shl ax,1
000000BC  8BD8              mov bx,ax
000000BE  8B87FA3C          mov ax,[bx+0x3cfa]
000000C2  8BDE              mov bx,si
000000C4  D1E3              shl bx,1
000000C6  D1E3              shl bx,1
000000C8  D1E3              shl bx,1
000000CA  3B87FA3C          cmp ax,[bx+0x3cfa]
000000CE  7C0C              jl 0xdc
000000D0  A0AA37            mov al,[0x37aa]
000000D3  98                cbw
000000D4  D1E0              shl ax,1
000000D6  8BD8              mov bx,ax
000000D8  FF8F742C          dec word [bx+0x2c74]
000000DC  8BDE              mov bx,si
000000DE  D1E3              shl bx,1
000000E0  D1E3              shl bx,1
000000E2  D1E3              shl bx,1
000000E4  8B87FA3C          mov ax,[bx+0x3cfa]
000000E8  50                push ax
000000E9  A0AA37            mov al,[0x37aa]
000000EC  98                cbw
000000ED  D1E0              shl ax,1
000000EF  D1E0              shl ax,1
000000F1  D1E0              shl ax,1
000000F3  8BD8              mov bx,ax
000000F5  58                pop ax
000000F6  3B87FA3C          cmp ax,[bx+0x3cfa]
000000FA  7C08              jl 0x104
000000FC  8BDE              mov bx,si
000000FE  D1E3              shl bx,1
00000100  FF8F742C          dec word [bx+0x2c74]
00000104  46                inc si
00000105  83FE03            cmp si,byte +0x3
00000108  7EA8              jng 0xb2
0000010A  FE06AA37          inc byte [0x37aa]
0000010E  803EAA3704        cmp byte [0x37aa],0x4
00000113  7C94              jl 0xa9
00000115  33FF              xor di,di
00000117  C606AA3700        mov byte [0x37aa],0x0
0000011C  EB2F              jmp short 0x14d
0000011E  A0AA37            mov al,[0x37aa]
00000121  98                cbw
00000122  D1E0              shl ax,1
00000124  8BD8              mov bx,ax
00000126  83BF742C00        cmp word [bx+0x2c74],byte +0x0
0000012B  7507              jnz 0x134
0000012D  47                inc di
0000012E  A0AA37            mov al,[0x37aa]
00000131  98                cbw
00000132  8BC8              mov cx,ax
00000134  A0AA37            mov al,[0x37aa]
00000137  98                cbw
00000138  D1E0              shl ax,1
0000013A  8BD8              mov bx,ax
0000013C  83BF742C01        cmp word [bx+0x2c74],byte +0x1
00000141  7506              jnz 0x149
00000143  A0AA37            mov al,[0x37aa]
00000146  98                cbw
00000147  8BD0              mov dx,ax
00000149  FE06AA37          inc byte [0x37aa]
0000014D  803EAA3704        cmp byte [0x37aa],0x4
00000152  7CCA              jl 0x11e
00000154  83FF01            cmp di,byte +0x1
00000157  7E08              jng 0x161
00000159  C7067C2C0000      mov word [0x2c7c],0x0
0000015F  EB1B              jmp short 0x17c
00000161  8BD9              mov bx,cx
00000163  D1E3              shl bx,1
00000165  D1E3              shl bx,1
00000167  D1E3              shl bx,1
00000169  8B87FA3C          mov ax,[bx+0x3cfa]
0000016D  8BDA              mov bx,dx
0000016F  D1E3              shl bx,1
00000171  D1E3              shl bx,1
00000173  D1E3              shl bx,1
00000175  2B87FA3C          sub ax,[bx+0x3cfa]
00000179  A37C2C            mov [0x2c7c],ax
0000017C  5F                pop di
0000017D  5E                pop si
0000017E  5D                pop bp
0000017F  CB                retf

; ---- 11A3:0180 ----
00000180  55                push bp
00000181  8BEC              mov bp,sp
00000183  83EC10            sub sp,byte +0x10
00000186  56                push si
00000187  C646F2FF          mov byte [bp-0xe],0xff
0000018B  C646F100          mov byte [bp-0xf],0x0
0000018F  C606AA3700        mov byte [0x37aa],0x0
00000194  E90801            jmp 0x29f
00000197  A0AA37            mov al,[0x37aa]
0000019A  98                cbw
0000019B  8BD8              mov bx,ax
0000019D  80BF400801        cmp byte [bx+0x840],0x1
000001A2  7503              jnz 0x1a7
000001A4  FE46F1            inc byte [bp-0xf]
000001A7  A0AA37            mov al,[0x37aa]
000001AA  98                cbw
000001AB  D1E0              shl ax,1
000001AD  8BD8              mov bx,ax
000001AF  8BB70E45          mov si,[bx+0x450e]
000001B3  83FEFF            cmp si,byte -0x1
000001B6  7504              jnz 0x1bc
000001B8  B000              mov al,0x0
000001BA  EB1C              jmp short 0x1d8
000001BC  80BC1A2F00        cmp byte [si+0x2f1a],0x0
000001C1  7504              jnz 0x1c7
000001C3  B000              mov al,0x0
000001C5  EB11              jmp short 0x1d8
000001C7  8BDE              mov bx,si
000001C9  D1E3              shl bx,1
000001CB  83BFF44400        cmp word [bx+0x44f4],byte +0x0
000001D0  7504              jnz 0x1d6
000001D2  B000              mov al,0x0
000001D4  EB02              jmp short 0x1d8
000001D6  B001              mov al,0x1
000001D8  50                push ax
000001D9  A0AA37            mov al,[0x37aa]
000001DC  98                cbw
000001DD  8D56F4            lea dx,[bp-0xc]
000001E0  03C2              add ax,dx
000001E2  8BD8              mov bx,ax
000001E4  58                pop ax
000001E5  368807            mov [ss:bx],al
000001E8  A0AA37            mov al,[0x37aa]
000001EB  98                cbw
000001EC  D1E0              shl ax,1
000001EE  8D56F8            lea dx,[bp-0x8]
000001F1  03C2              add ax,dx
000001F3  8BD8              mov bx,ax
000001F5  36C7070000        mov word [ss:bx],0x0
000001FA  C606002F00        mov byte [0x2f00],0x0
000001FF  E98900            jmp 0x28b
00000202  C606B02E00        mov byte [0x2eb0],0x0
00000207  EB71              jmp short 0x27a
00000209  A0B02E            mov al,[0x2eb0]
0000020C  98                cbw
0000020D  BA0600            mov dx,0x6
00000210  F7EA              imul dx
00000212  50                push ax
00000213  A0002F            mov al,[0x2f00]
00000216  98                cbw
00000217  5B                pop bx
00000218  03D8              add bx,ax
0000021A  8A874244          mov al,[bx+0x4442]
0000021E  3A06AA37          cmp al,[0x37aa]
00000222  7552              jnz 0x276
00000224  A0B02E            mov al,[0x2eb0]
00000227  98                cbw
00000228  BA0600            mov dx,0x6
0000022B  F7EA              imul dx
0000022D  50                push ax
0000022E  A0002F            mov al,[0x2f00]
00000231  98                cbw
00000232  5B                pop bx
00000233  03D8              add bx,ax
00000235  80BFB12E04        cmp byte [bx+0x2eb1],0x4
0000023A  743A              jz 0x276
0000023C  A0B02E            mov al,[0x2eb0]
0000023F  98                cbw
00000240  BA0600            mov dx,0x6
00000243  F7EA              imul dx
00000245  50                push ax
00000246  A0002F            mov al,[0x2f00]
00000249  98                cbw
0000024A  5B                pop bx
0000024B  03D8              add bx,ax
0000024D  80BFDA4503        cmp byte [bx+0x45da],0x3
00000252  7422              jz 0x276
00000254  A0B02E            mov al,[0x2eb0]
00000257  98                cbw
00000258  BA0600            mov dx,0x6
0000025B  F7EA              imul dx
0000025D  50                push ax
0000025E  A0002F            mov al,[0x2f00]
00000261  98                cbw
00000262  5B                pop bx
00000263  03D8              add bx,ax
00000265  8A874244          mov al,[bx+0x4442]
00000269  98                cbw
0000026A  D1E0              shl ax,1
0000026C  8D56F8            lea dx,[bp-0x8]
0000026F  03C2              add ax,dx
00000271  8BD8              mov bx,ax
00000273  36FF07            inc word [ss:bx]
00000276  FE06B02E          inc byte [0x2eb0]
0000027A  A0B02E            mov al,[0x2eb0]
0000027D  98                cbw
0000027E  8B16FC08          mov dx,[0x8fc]
00000282  4A                dec dx
00000283  3BC2              cmp ax,dx
00000285  7E82              jng 0x209
00000287  FE06002F          inc byte [0x2f00]
0000028B  A0002F            mov al,[0x2f00]
0000028E  98                cbw
0000028F  8B16FE08          mov dx,[0x8fe]
00000293  4A                dec dx
00000294  3BC2              cmp ax,dx
00000296  7F03              jg 0x29b
00000298  E967FF            jmp 0x202
0000029B  FE06AA37          inc byte [0x37aa]
0000029F  803EAA3704        cmp byte [0x37aa],0x4
000002A4  7D03              jnl 0x2a9
000002A6  E9EEFE            jmp 0x197
000002A9  C606AA3700        mov byte [0x37aa],0x0
000002AE  EB6A              jmp short 0x31a
000002B0  A0AA37            mov al,[0x37aa]
000002B3  98                cbw
000002B4  8BD8              mov bx,ax
000002B6  80BF400801        cmp byte [bx+0x840],0x1
000002BB  7559              jnz 0x316
000002BD  A0AA37            mov al,[0x37aa]
000002C0  98                cbw
000002C1  8BD8              mov bx,ax
000002C3  80BFBE4B01        cmp byte [bx+0x4bbe],0x1
000002C8  754C              jnz 0x316
000002CA  803E052F08        cmp byte [0x2f05],0x8
000002CF  7E45              jng 0x316
000002D1  A0AA37            mov al,[0x37aa]
000002D4  98                cbw
000002D5  8D56F4            lea dx,[bp-0xc]
000002D8  03C2              add ax,dx
000002DA  8BD8              mov bx,ax
000002DC  36803F00          cmp byte [ss:bx],0x0
000002E0  7502              jnz 0x2e4
000002E2  EB32              jmp short 0x316
000002E4  807EF2FF          cmp byte [bp-0xe],0xff
000002E8  7426              jz 0x310
000002EA  A0AA37            mov al,[0x37aa]
000002ED  98                cbw
000002EE  D1E0              shl ax,1
000002F0  8D56F8            lea dx,[bp-0x8]
000002F3  03C2              add ax,dx
000002F5  8BD8              mov bx,ax
000002F7  368B07            mov ax,[ss:bx]
000002FA  50                push ax
000002FB  8A46F2            mov al,[bp-0xe]
000002FE  98                cbw
000002FF  D1E0              shl ax,1
00000301  8D56F8            lea dx,[bp-0x8]
00000304  03C2              add ax,dx
00000306  8BD8              mov bx,ax
00000308  58                pop ax
00000309  363B07            cmp ax,[ss:bx]
0000030C  7F02              jg 0x310
0000030E  EB06              jmp short 0x316
00000310  A0AA37            mov al,[0x37aa]
00000313  8846F2            mov [bp-0xe],al
00000316  FE06AA37          inc byte [0x37aa]
0000031A  803EAA3704        cmp byte [0x37aa],0x4
0000031F  7C8F              jl 0x2b0
00000321  807EF2FF          cmp byte [bp-0xe],0xff
00000325  7403              jz 0x32a
00000327  E90A01            jmp 0x434
0000032A  803E052F10        cmp byte [0x2f05],0x10
0000032F  7D03              jnl 0x334
00000331  E90001            jmp 0x434
00000334  A08045            mov al,[0x4580]
00000337  8846F2            mov [bp-0xe],al
0000033A  C646F301          mov byte [bp-0xd],0x1
0000033E  C606AA3700        mov byte [0x37aa],0x0
00000343  EB24              jmp short 0x369
00000345  A0AA37            mov al,[0x37aa]
00000348  98                cbw
00000349  8BD8              mov bx,ax
0000034B  80BF400801        cmp byte [bx+0x840],0x1
00000350  7513              jnz 0x365
00000352  A0AA37            mov al,[0x37aa]
00000355  98                cbw
00000356  D1E0              shl ax,1
00000358  8BD8              mov bx,ax
0000035A  83BF742C00        cmp word [bx+0x2c74],byte +0x0
0000035F  7504              jnz 0x365
00000361  C646F300          mov byte [bp-0xd],0x0
00000365  FE06AA37          inc byte [0x37aa]
00000369  803EAA3704        cmp byte [0x37aa],0x4
0000036E  7CD5              jl 0x345
00000370  807EF101          cmp byte [bp-0xf],0x1
00000374  7F04              jg 0x37a
00000376  C646F300          mov byte [bp-0xd],0x0
0000037A  803E8045FF        cmp byte [0x4580],0xff
0000037F  7403              jz 0x384
00000381  E98700            jmp 0x40b
00000384  B80500            mov ax,0x5
00000387  50                push ax
00000388  9ABA01FD21        call 0x21fd:0x1ba
0000038D  59                pop cx
0000038E  0BC0              or ax,ax
00000390  7406              jz 0x398
00000392  807EF301          cmp byte [bp-0xd],0x1
00000396  7571              jnz 0x409
00000398  C606AA3700        mov byte [0x37aa],0x0
0000039D  EB63              jmp short 0x402
0000039F  A0AA37            mov al,[0x37aa]
000003A2  98                cbw
000003A3  8BD8              mov bx,ax
000003A5  80BF400801        cmp byte [bx+0x840],0x1
000003AA  7552              jnz 0x3fe
000003AC  A0AA37            mov al,[0x37aa]
000003AF  98                cbw
000003B0  8BD8              mov bx,ax
000003B2  80BF423D00        cmp byte [bx+0x3d42],0x0
000003B7  7645              jna 0x3fe
000003B9  A0AA37            mov al,[0x37aa]
000003BC  98                cbw
000003BD  8D56F4            lea dx,[bp-0xc]
000003C0  03C2              add ax,dx
000003C2  8BD8              mov bx,ax
000003C4  36803F00          cmp byte [ss:bx],0x0
000003C8  7502              jnz 0x3cc
000003CA  EB32              jmp short 0x3fe
000003CC  807EF2FF          cmp byte [bp-0xe],0xff
000003D0  7426              jz 0x3f8
000003D2  A0AA37            mov al,[0x37aa]
000003D5  98                cbw
000003D6  D1E0              shl ax,1
000003D8  8D56F8            lea dx,[bp-0x8]
000003DB  03C2              add ax,dx
000003DD  8BD8              mov bx,ax
000003DF  368B07            mov ax,[ss:bx]
000003E2  50                push ax
000003E3  8A46F2            mov al,[bp-0xe]
000003E6  98                cbw
000003E7  D1E0              shl ax,1
000003E9  8D56F8            lea dx,[bp-0x8]
000003EC  03C2              add ax,dx
000003EE  8BD8              mov bx,ax
000003F0  58                pop ax
000003F1  363B07            cmp ax,[ss:bx]
000003F4  7F02              jg 0x3f8
000003F6  EB06              jmp short 0x3fe
000003F8  A0AA37            mov al,[0x37aa]
000003FB  8846F2            mov [bp-0xe],al
000003FE  FE06AA37          inc byte [0x37aa]
00000402  803EAA3704        cmp byte [0x37aa],0x4
00000407  7C96              jl 0x39f
00000409  EB29              jmp short 0x434
0000040B  B80500            mov ax,0x5
0000040E  50                push ax
0000040F  9ABA01FD21        call 0x21fd:0x1ba
00000414  59                pop cx
00000415  0BC0              or ax,ax
00000417  7506              jnz 0x41f
00000419  807EF300          cmp byte [bp-0xd],0x0
0000041D  7411              jz 0x430
0000041F  A0AA37            mov al,[0x37aa]
00000422  98                cbw
00000423  8D56F4            lea dx,[bp-0xc]
00000426  03C2              add ax,dx
00000428  8BD8              mov bx,ax
0000042A  36803F00          cmp byte [ss:bx],0x0
0000042E  7504              jnz 0x434
00000430  C646F2FF          mov byte [bp-0xe],0xff
00000434  8A46F2            mov al,[bp-0xe]
00000437  A28045            mov [0x4580],al
0000043A  5E                pop si
0000043B  8BE5              mov sp,bp
0000043D  5D                pop bp
0000043E  CB                retf

; ---- 11A3:043F ----
0000043F  55                push bp
00000440  8BEC              mov bp,sp
00000442  83EC08            sub sp,byte +0x8
00000445  56                push si
00000446  33F6              xor si,si
00000448  C706D0450000      mov word [0x45d0],0x0
0000044E  E9D800            jmp 0x529
00000451  8B1ED045          mov bx,[0x45d0]
00000455  D1E3              shl bx,1
00000457  8D46F8            lea ax,[bp-0x8]
0000045A  03D8              add bx,ax
0000045C  36C7070000        mov word [ss:bx],0x0
00000461  C606002F00        mov byte [0x2f00],0x0
00000466  E9AC00            jmp 0x515
00000469  C606B02E00        mov byte [0x2eb0],0x0
0000046E  E99000            jmp 0x501
00000471  A0B02E            mov al,[0x2eb0]
00000474  98                cbw
00000475  BA0600            mov dx,0x6
00000478  F7EA              imul dx
0000047A  50                push ax
0000047B  A0002F            mov al,[0x2f00]
0000047E  98                cbw
0000047F  5B                pop bx
00000480  03D8              add bx,ax
00000482  8A874244          mov al,[bx+0x4442]
00000486  3A06AA37          cmp al,[0x37aa]
0000048A  7571              jnz 0x4fd
0000048C  A0B02E            mov al,[0x2eb0]
0000048F  98                cbw
00000490  BA0600            mov dx,0x6
00000493  F7EA              imul dx
00000495  50                push ax
00000496  A0002F            mov al,[0x2f00]
00000499  98                cbw
0000049A  5B                pop bx
0000049B  03D8              add bx,ax
0000049D  80BFB12E04        cmp byte [bx+0x2eb1],0x4
000004A2  7459              jz 0x4fd
000004A4  A0B02E            mov al,[0x2eb0]
000004A7  98                cbw
000004A8  BA0600            mov dx,0x6
000004AB  F7EA              imul dx
000004AD  50                push ax
000004AE  A0002F            mov al,[0x2f00]
000004B1  98                cbw
000004B2  5B                pop bx
000004B3  03D8              add bx,ax
000004B5  8A87DA45          mov al,[bx+0x45da]
000004B9  98                cbw
000004BA  8B1ED045          mov bx,[0x45d0]
000004BE  D1E3              shl bx,1
000004C0  D1E3              shl bx,1
000004C2  03D8              add bx,ax
000004C4  8A870209          mov al,[bx+0x902]
000004C8  98                cbw
000004C9  0BC0              or ax,ax
000004CB  740C              jz 0x4d9
000004CD  3D0100            cmp ax,0x1
000004D0  740B              jz 0x4dd
000004D2  3D0200            cmp ax,0x2
000004D5  740A              jz 0x4e1
000004D7  EB0C              jmp short 0x4e5
000004D9  B100              mov cl,0x0
000004DB  EB08              jmp short 0x4e5
000004DD  B102              mov cl,0x2
000004DF  EB04              jmp short 0x4e5
000004E1  B105              mov cl,0x5
000004E3  EB00              jmp short 0x4e5
000004E5  8B1ED045          mov bx,[0x45d0]
000004E9  D1E3              shl bx,1
000004EB  8D46F8            lea ax,[bp-0x8]
000004EE  03D8              add bx,ax
000004F0  8AC1              mov al,cl
000004F2  B400              mov ah,0x0
000004F4  360107            add [ss:bx],ax
000004F7  8AC1              mov al,cl
000004F9  B400              mov ah,0x0
000004FB  03F0              add si,ax
000004FD  FE06B02E          inc byte [0x2eb0]
00000501  A0B02E            mov al,[0x2eb0]
00000504  98                cbw
00000505  8B16FC08          mov dx,[0x8fc]
00000509  4A                dec dx
0000050A  3BC2              cmp ax,dx
0000050C  7F03              jg 0x511
0000050E  E960FF            jmp 0x471
00000511  FE06002F          inc byte [0x2f00]
00000515  A0002F            mov al,[0x2f00]
00000518  98                cbw
00000519  8B16FE08          mov dx,[0x8fe]
0000051D  4A                dec dx
0000051E  3BC2              cmp ax,dx
00000520  7F03              jg 0x525
00000522  E944FF            jmp 0x469
00000525  FF06D045          inc word [0x45d0]
00000529  833ED04502        cmp word [0x45d0],byte +0x2
0000052E  7F03              jg 0x533
00000530  E91EFF            jmp 0x451
00000533  C706D0450000      mov word [0x45d0],0x0
00000539  EB52              jmp short 0x58d
0000053B  0BF6              or si,si
0000053D  751C              jnz 0x55b
0000053F  A0AA37            mov al,[0x37aa]
00000542  98                cbw
00000543  D1E0              shl ax,1
00000545  D1E0              shl ax,1
00000547  D1E0              shl ax,1
00000549  8B16D045          mov dx,[0x45d0]
0000054D  D1E2              shl dx,1
0000054F  03C2              add ax,dx
00000551  8BD8              mov bx,ax
00000553  C7874C2C0000      mov word [bx+0x2c4c],0x0
00000559  EB2E              jmp short 0x589
0000055B  A0AA37            mov al,[0x37aa]
0000055E  98                cbw
0000055F  D1E0              shl ax,1
00000561  D1E0              shl ax,1
00000563  D1E0              shl ax,1
00000565  8B16D045          mov dx,[0x45d0]
00000569  D1E2              shl dx,1
0000056B  03C2              add ax,dx
0000056D  8B1ED045          mov bx,[0x45d0]
00000571  D1E3              shl bx,1
00000573  8D56F8            lea dx,[bp-0x8]
00000576  03DA              add bx,dx
00000578  50                push ax
00000579  368B07            mov ax,[ss:bx]
0000057C  BA6400            mov dx,0x64
0000057F  F7EA              imul dx
00000581  99                cwd
00000582  F7FE              idiv si
00000584  5B                pop bx
00000585  89874C2C          mov [bx+0x2c4c],ax
00000589  FF06D045          inc word [0x45d0]
0000058D  833ED04502        cmp word [0x45d0],byte +0x2
00000592  7EA7              jng 0x53b
00000594  5E                pop si
00000595  8BE5              mov sp,bp
00000597  5D                pop bp
00000598  CB                retf

; ---- 11A3:0599 ----
00000599  55                push bp
0000059A  8BEC              mov bp,sp
0000059C  A0AA37            mov al,[0x37aa]
0000059F  98                cbw
000005A0  D1E0              shl ax,1
000005A2  D1E0              shl ax,1
000005A4  D1E0              shl ax,1
000005A6  50                push ax
000005A7  A0AA37            mov al,[0x37aa]
000005AA  98                cbw
000005AB  D1E0              shl ax,1
000005AD  8BD8              mov bx,ax
000005AF  8B87F640          mov ax,[bx+0x40f6]
000005B3  D1E0              shl ax,1
000005B5  5B                pop bx
000005B6  03D8              add bx,ax
000005B8  8A874C2C          mov al,[bx+0x2c4c]
000005BC  8AC8              mov cl,al
000005BE  C706D0450000      mov word [0x45d0],0x0
000005C4  EB4B              jmp short 0x611
000005C6  A0AA37            mov al,[0x37aa]
000005C9  98                cbw
000005CA  D1E0              shl ax,1
000005CC  D1E0              shl ax,1
000005CE  D1E0              shl ax,1
000005D0  8B16D045          mov dx,[0x45d0]
000005D4  D1E2              shl dx,1
000005D6  03C2              add ax,dx
000005D8  50                push ax
000005D9  8AC1              mov al,cl
000005DB  98                cbw
000005DC  5B                pop bx
000005DD  39874C2C          cmp [bx+0x2c4c],ax
000005E1  7E2A              jng 0x60d
000005E3  A0AA37            mov al,[0x37aa]
000005E6  98                cbw
000005E7  D1E0              shl ax,1
000005E9  8B16D045          mov dx,[0x45d0]
000005ED  8BD8              mov bx,ax
000005EF  8997F640          mov [bx+0x40f6],dx
000005F3  A0AA37            mov al,[0x37aa]
000005F6  98                cbw
000005F7  D1E0              shl ax,1
000005F9  D1E0              shl ax,1
000005FB  D1E0              shl ax,1
000005FD  8B16D045          mov dx,[0x45d0]
00000601  D1E2              shl dx,1
00000603  03C2              add ax,dx
00000605  8BD8              mov bx,ax
00000607  8A874C2C          mov al,[bx+0x2c4c]
0000060B  8AC8              mov cl,al
0000060D  FF06D045          inc word [0x45d0]
00000611  833ED04502        cmp word [0x45d0],byte +0x2
00000616  7EAE              jng 0x5c6
00000618  5D                pop bp
00000619  CB                retf

; ---- 11A3:061A ----
0000061A  55                push bp
0000061B  8BEC              mov bp,sp
0000061D  56                push si
0000061E  9A5007A322        call 0x22a3:0x750
00000623  C606AA3700        mov byte [0x37aa],0x0
00000628  EB49              jmp short 0x673
0000062A  A0AA37            mov al,[0x37aa]
0000062D  98                cbw
0000062E  8BD8              mov bx,ax
00000630  80BF400800        cmp byte [bx+0x840],0x0
00000635  7538              jnz 0x66f
00000637  A0AA37            mov al,[0x37aa]
0000063A  98                cbw
0000063B  D1E0              shl ax,1
0000063D  8BD8              mov bx,ax
0000063F  C787F640FFFF      mov word [bx+0x40f6],0xffff
00000645  33F6              xor si,si
00000647  EB21              jmp short 0x66a
00000649  A0AA37            mov al,[0x37aa]
0000064C  98                cbw
0000064D  BA0300            mov dx,0x3
00000650  F7EA              imul dx
00000652  8BD8              mov bx,ax
00000654  80B82C3446        cmp byte [bx+si+0x342c],0x46
00000659  720E              jc 0x669
0000065B  A0AA37            mov al,[0x37aa]
0000065E  98                cbw
0000065F  D1E0              shl ax,1
00000661  8BD8              mov bx,ax
00000663  89B7F640          mov [bx+0x40f6],si
00000667  EB06              jmp short 0x66f
00000669  46                inc si
0000066A  83FE02            cmp si,byte +0x2
0000066D  7EDA              jng 0x649
0000066F  FE06AA37          inc byte [0x37aa]
00000673  803EAA3704        cmp byte [0x37aa],0x4
00000678  7CB0              jl 0x62a
0000067A  5E                pop si
0000067B  5D                pop bp
0000067C  CB                retf

; ---- 11A3:067D ----
0000067D  55                push bp
0000067E  8BEC              mov bp,sp
00000680  83EC08            sub sp,byte +0x8
00000683  56                push si
00000684  C706D0450000      mov word [0x45d0],0x0
0000068A  EB14              jmp short 0x6a0
0000068C  8B1ED045          mov bx,[0x45d0]
00000690  D1E3              shl bx,1
00000692  8D46F8            lea ax,[bp-0x8]
00000695  03D8              add bx,ax
00000697  36C7070000        mov word [ss:bx],0x0
0000069C  FF06D045          inc word [0x45d0]
000006A0  833ED04502        cmp word [0x45d0],byte +0x2
000006A5  7EE5              jng 0x68c
000006A7  C606AA3700        mov byte [0x37aa],0x0
000006AC  EB37              jmp short 0x6e5
000006AE  C706D0450000      mov word [0x45d0],0x0
000006B4  EB24              jmp short 0x6da
000006B6  A0AA37            mov al,[0x37aa]
000006B9  98                cbw
000006BA  D1E0              shl ax,1
000006BC  8BD8              mov bx,ax
000006BE  8B87F640          mov ax,[bx+0x40f6]
000006C2  3B06D045          cmp ax,[0x45d0]
000006C6  750E              jnz 0x6d6
000006C8  8B1ED045          mov bx,[0x45d0]
000006CC  D1E3              shl bx,1
000006CE  8D46F8            lea ax,[bp-0x8]
000006D1  03D8              add bx,ax
000006D3  36FF07            inc word [ss:bx]
000006D6  FF06D045          inc word [0x45d0]
000006DA  833ED04502        cmp word [0x45d0],byte +0x2
000006DF  7ED5              jng 0x6b6
000006E1  FE06AA37          inc byte [0x37aa]
000006E5  803EAA3704        cmp byte [0x37aa],0x4
000006EA  7CC2              jl 0x6ae
000006EC  C706D0450000      mov word [0x45d0],0x0
000006F2  E98600            jmp 0x77b
000006F5  8B1ED045          mov bx,[0x45d0]
000006F9  D1E3              shl bx,1
000006FB  8D46F8            lea ax,[bp-0x8]
000006FE  03D8              add bx,ax
00000700  36833F00          cmp word [ss:bx],byte +0x0
00000704  7571              jnz 0x777
00000706  C606AA3700        mov byte [0x37aa],0x0
0000070B  EB63              jmp short 0x770
0000070D  A0AA37            mov al,[0x37aa]
00000710  98                cbw
00000711  8BD8              mov bx,ax
00000713  80BF400801        cmp byte [bx+0x840],0x1
00000718  7552              jnz 0x76c
0000071A  A0AA37            mov al,[0x37aa]
0000071D  98                cbw
0000071E  D1E0              shl ax,1
00000720  8BD8              mov bx,ax
00000722  8BB7F640          mov si,[bx+0x40f6]
00000726  A0AA37            mov al,[0x37aa]
00000729  98                cbw
0000072A  D1E0              shl ax,1
0000072C  D1E0              shl ax,1
0000072E  D1E0              shl ax,1
00000730  8B16D045          mov dx,[0x45d0]
00000734  D1E2              shl dx,1
00000736  03C2              add ax,dx
00000738  8BD8              mov bx,ax
0000073A  83BF4C2C21        cmp word [bx+0x2c4c],byte +0x21
0000073F  7C2B              jl 0x76c
00000741  8BDE              mov bx,si
00000743  D1E3              shl bx,1
00000745  8D46F8            lea ax,[bp-0x8]
00000748  03D8              add bx,ax
0000074A  36833F02          cmp word [ss:bx],byte +0x2
0000074E  7C1C              jl 0x76c
00000750  8BDE              mov bx,si
00000752  D1E3              shl bx,1
00000754  8D46F8            lea ax,[bp-0x8]
00000757  03D8              add bx,ax
00000759  36FF0F            dec word [ss:bx]
0000075C  A0AA37            mov al,[0x37aa]
0000075F  98                cbw
00000760  D1E0              shl ax,1
00000762  8B16D045          mov dx,[0x45d0]
00000766  8BD8              mov bx,ax
00000768  8997F640          mov [bx+0x40f6],dx
0000076C  FE06AA37          inc byte [0x37aa]
00000770  803EAA3704        cmp byte [0x37aa],0x4
00000775  7C96              jl 0x70d
00000777  FF06D045          inc word [0x45d0]
0000077B  833ED04502        cmp word [0x45d0],byte +0x2
00000780  7F03              jg 0x785
00000782  E970FF            jmp 0x6f5
00000785  5E                pop si
00000786  8BE5              mov sp,bp
00000788  5D                pop bp
00000789  CB                retf

; ---- 11A3:078A ----
0000078A  55                push bp
0000078B  8BEC              mov bp,sp
0000078D  83EC0E            sub sp,byte +0xe
00000790  56                push si
00000791  57                push di
00000792  C646F600          mov byte [bp-0xa],0x0
00000796  33FF              xor di,di
00000798  A0AA37            mov al,[0x37aa]
0000079B  98                cbw
0000079C  D1E0              shl ax,1
0000079E  8BD8              mov bx,ax
000007A0  8A87F640          mov al,[bx+0x40f6]
000007A4  8846F5            mov [bp-0xb],al
000007A7  8A46F5            mov al,[bp-0xb]
000007AA  98                cbw
000007AB  8BD8              mov bx,ax
000007AD  8A87E408          mov al,[bx+0x8e4]
000007B1  8846F4            mov [bp-0xc],al
000007B4  C706D0450000      mov word [0x45d0],0x0
000007BA  EB1C              jmp short 0x7d8
000007BC  8B1ED045          mov bx,[0x45d0]
000007C0  D1E3              shl bx,1
000007C2  C7876C2C0000      mov word [bx+0x2c6c],0x0
000007C8  8B1ED045          mov bx,[0x45d0]
000007CC  D1E3              shl bx,1
000007CE  C787442C0000      mov word [bx+0x2c44],0x0
000007D4  FF06D045          inc word [0x45d0]
000007D8  833ED04503        cmp word [0x45d0],byte +0x3
000007DD  7EDD              jng 0x7bc
000007DF  C606002F00        mov byte [0x2f00],0x0
000007E4  EB6B              jmp short 0x851
000007E6  C606B02E00        mov byte [0x2eb0],0x0
000007EB  EB53              jmp short 0x840
000007ED  A0B02E            mov al,[0x2eb0]
000007F0  98                cbw
000007F1  BA0600            mov dx,0x6
000007F4  F7EA              imul dx
000007F6  50                push ax
000007F7  A0002F            mov al,[0x2f00]
000007FA  98                cbw
000007FB  5B                pop bx
000007FC  03D8              add bx,ax
000007FE  8A874244          mov al,[bx+0x4442]
00000802  3A06AA37          cmp al,[0x37aa]
00000806  7534              jnz 0x83c
00000808  A0B02E            mov al,[0x2eb0]
0000080B  98                cbw
0000080C  BA0600            mov dx,0x6
0000080F  F7EA              imul dx
00000811  50                push ax
00000812  A0002F            mov al,[0x2f00]
00000815  98                cbw
00000816  5B                pop bx
00000817  03D8              add bx,ax
00000819  80BFB12E04        cmp byte [bx+0x2eb1],0x4
0000081E  741C              jz 0x83c
00000820  FE46F6            inc byte [bp-0xa]
00000823  A0B02E            mov al,[0x2eb0]
00000826  98                cbw
00000827  BA0600            mov dx,0x6
0000082A  F7EA              imul dx
0000082C  50                push ax
0000082D  A0002F            mov al,[0x2f00]
00000830  98                cbw
00000831  5B                pop bx
00000832  03D8              add bx,ax
00000834  80BFDA4503        cmp byte [bx+0x45da],0x3
00000839  7401              jz 0x83c
0000083B  47                inc di
0000083C  FE06B02E          inc byte [0x2eb0]
00000840  A0B02E            mov al,[0x2eb0]
00000843  98                cbw
00000844  8B16FC08          mov dx,[0x8fc]
00000848  4A                dec dx
00000849  3BC2              cmp ax,dx
0000084B  7EA0              jng 0x7ed
0000084D  FE06002F          inc byte [0x2f00]
00000851  A0002F            mov al,[0x2f00]
00000854  98                cbw
00000855  8B16FE08          mov dx,[0x8fe]
00000859  4A                dec dx
0000085A  3BC2              cmp ax,dx
0000085C  7E88              jng 0x7e6
0000085E  C646F301          mov byte [bp-0xd],0x1
00000862  C646F700          mov byte [bp-0x9],0x0
00000866  EB28              jmp short 0x890
00000868  8A46F7            mov al,[bp-0x9]
0000086B  B400              mov ah,0x0
0000086D  8BD8              mov bx,ax
0000086F  80BF400801        cmp byte [bx+0x840],0x1
00000874  7517              jnz 0x88d
00000876  8A46F7            mov al,[bp-0x9]
00000879  B400              mov ah,0x0
0000087B  D1E0              shl ax,1
0000087D  50                push ax
0000087E  8A46F4            mov al,[bp-0xc]
00000881  98                cbw
00000882  5B                pop bx
00000883  3987F640          cmp [bx+0x40f6],ax
00000887  7504              jnz 0x88d
00000889  C646F300          mov byte [bp-0xd],0x0
0000088D  FE46F7            inc byte [bp-0x9]
00000890  807EF703          cmp byte [bp-0x9],0x3
00000894  76D2              jna 0x868
00000896  A0AA37            mov al,[0x37aa]
00000899  3A068045          cmp al,[0x4580]
0000089D  755E              jnz 0x8fd
0000089F  A0AA37            mov al,[0x37aa]
000008A2  98                cbw
000008A3  D1E0              shl ax,1
000008A5  8BD8              mov bx,ax
000008A7  83BF742C00        cmp word [bx+0x2c74],byte +0x0
000008AC  7404              jz 0x8b2
000008AE  8BF7              mov si,di
000008B0  EB49              jmp short 0x8fb
000008B2  833E7C2C64        cmp word [0x2c7c],byte +0x64
000008B7  7D04              jnl 0x8bd
000008B9  8BF7              mov si,di
000008BB  EB3E              jmp short 0x8fb
000008BD  813E7C2CC800      cmp word [0x2c7c],0xc8
000008C3  7D10              jnl 0x8d5
000008C5  8A46F6            mov al,[bp-0xa]
000008C8  B400              mov ah,0x0
000008CA  BB0200            mov bx,0x2
000008CD  99                cwd
000008CE  F7FB              idiv bx
000008D0  40                inc ax
000008D1  8BF0              mov si,ax
000008D3  EB26              jmp short 0x8fb
000008D5  813E7C2C2C01      cmp word [0x2c7c],0x12c
000008DB  7D10              jnl 0x8ed
000008DD  8A46F6            mov al,[bp-0xa]
000008E0  B400              mov ah,0x0
000008E2  BB0300            mov bx,0x3
000008E5  99                cwd
000008E6  F7FB              idiv bx
000008E8  40                inc ax
000008E9  8BF0              mov si,ax
000008EB  EB0E              jmp short 0x8fb
000008ED  8A46F6            mov al,[bp-0xa]
000008F0  B400              mov ah,0x0
000008F2  BB0400            mov bx,0x4
000008F5  99                cwd
000008F6  F7FB              idiv bx
000008F8  40                inc ax
000008F9  8BF0              mov si,ax
000008FB  EB59              jmp short 0x956
000008FD  A0AA37            mov al,[0x37aa]
00000900  98                cbw
00000901  8BD8              mov bx,ax
00000903  80BFC24B01        cmp byte [bx+0x4bc2],0x1
00000908  750F              jnz 0x919
0000090A  8A46F6            mov al,[bp-0xa]
0000090D  B400              mov ah,0x0
0000090F  BB0300            mov bx,0x3
00000912  99                cwd
00000913  F7FB              idiv bx
00000915  8BF0              mov si,ax
00000917  EB0D              jmp short 0x926
00000919  8A46F6            mov al,[bp-0xa]
0000091C  B400              mov ah,0x0
0000091E  BB0400            mov bx,0x4
00000921  99                cwd
00000922  F7FB              idiv bx
00000924  8BF0              mov si,ax
00000926  A0AA37            mov al,[0x37aa]
00000929  98                cbw
0000092A  D1E0              shl ax,1
0000092C  D1E0              shl ax,1
0000092E  D1E0              shl ax,1
00000930  8BD8              mov bx,ax
00000932  81BFFA3CC800      cmp word [bx+0x3cfa],0xc8
00000938  7D01              jnl 0x93b
0000093A  46                inc si
0000093B  A0AA37            mov al,[0x37aa]
0000093E  98                cbw
0000093F  D1E0              shl ax,1
00000941  D1E0              shl ax,1
00000943  D1E0              shl ax,1
00000945  8BD8              mov bx,ax
00000947  83BFFA3C50        cmp word [bx+0x3cfa],byte +0x50
0000094C  7D08              jnl 0x956
0000094E  83FE02            cmp si,byte +0x2
00000951  7D03              jnl 0x956
00000953  BE0200            mov si,0x2
00000956  3BF7              cmp si,di
00000958  7E02              jng 0x95c
0000095A  8BF7              mov si,di
0000095C  807EF301          cmp byte [bp-0xd],0x1
00000960  750F              jnz 0x971
00000962  8A46F6            mov al,[bp-0xa]
00000965  B400              mov ah,0x0
00000967  2BC6              sub ax,si
00000969  BB0500            mov bx,0x5
0000096C  99                cwd
0000096D  F7FB              idiv bx
0000096F  EB02              jmp short 0x973
00000971  33C0              xor ax,ax
00000973  8946FC            mov [bp-0x4],ax
00000976  8A46F6            mov al,[bp-0xa]
00000979  B400              mov ah,0x0
0000097B  8BD6              mov dx,si
0000097D  0356FC            add dx,[bp-0x4]
00000980  2BC2              sub ax,dx
00000982  8946FE            mov [bp-0x2],ax
00000985  8A46F5            mov al,[bp-0xb]
00000988  98                cbw
00000989  D1E0              shl ax,1
0000098B  8B56FE            mov dx,[bp-0x2]
0000098E  8BD8              mov bx,ax
00000990  8997442C          mov [bx+0x2c44],dx
00000994  8A46F4            mov al,[bp-0xc]
00000997  98                cbw
00000998  D1E0              shl ax,1
0000099A  8B56FC            mov dx,[bp-0x4]
0000099D  8BD8              mov bx,ax
0000099F  8997442C          mov [bx+0x2c44],dx
000009A3  89364A2C          mov [0x2c4a],si
000009A7  A0AA37            mov al,[0x37aa]
000009AA  98                cbw
000009AB  D1E0              shl ax,1
000009AD  D1E0              shl ax,1
000009AF  8BD8              mov bx,ax
000009B1  8A87EA2E          mov al,[bx+0x2eea]
000009B5  98                cbw
000009B6  50                push ax
000009B7  A0AA37            mov al,[0x37aa]
000009BA  98                cbw
000009BB  D1E0              shl ax,1
000009BD  8BD8              mov bx,ax
000009BF  58                pop ax
000009C0  0387AC4B          add ax,[bx+0x4bac]
000009C4  8946FA            mov [bp-0x6],ax
000009C7  397EFA            cmp [bp-0x6],di
000009CA  7D03              jnl 0x9cf
000009CC  8B7EFA            mov di,[bp-0x6]
000009CF  3BF7              cmp si,di
000009D1  7E02              jng 0x9d5
000009D3  8BF7              mov si,di
000009D5  33C0              xor ax,ax
000009D7  50                push ax
000009D8  8A46F4            mov al,[bp-0xc]
000009DB  98                cbw
000009DC  50                push ax
000009DD  9AA50AA311        call 0x11a3:0xaa5
000009E2  59                pop cx
000009E3  59                pop cx
000009E4  8946F8            mov [bp-0x8],ax
000009E7  807EF301          cmp byte [bp-0xd],0x1
000009EB  750F              jnz 0x9fc
000009ED  8A46F6            mov al,[bp-0xa]
000009F0  B400              mov ah,0x0
000009F2  2BC6              sub ax,si
000009F4  BB0500            mov bx,0x5
000009F7  99                cwd
000009F8  F7FB              idiv bx
000009FA  EB02              jmp short 0x9fe
000009FC  33C0              xor ax,ax
000009FE  8946FC            mov [bp-0x4],ax
00000A01  8B46FC            mov ax,[bp-0x4]
00000A04  3B46F8            cmp ax,[bp-0x8]
00000A07  7E06              jng 0xa0f
00000A09  8B46F8            mov ax,[bp-0x8]
00000A0C  8946FC            mov [bp-0x4],ax
00000A0F  8A46F6            mov al,[bp-0xa]
00000A12  B400              mov ah,0x0
00000A14  8BD6              mov dx,si
00000A16  0356FC            add dx,[bp-0x4]
00000A19  2BC2              sub ax,dx
00000A1B  8946FE            mov [bp-0x2],ax
00000A1E  A0262F            mov al,[0x2f26]
00000A21  98                cbw
00000A22  F76EFC            imul word [bp-0x4]
00000A25  50                push ax
00000A26  8A46F5            mov al,[bp-0xb]
00000A29  98                cbw
00000A2A  50                push ax
00000A2B  9AA50AA311        call 0x11a3:0xaa5
00000A30  59                pop cx
00000A31  59                pop cx
00000A32  8946F8            mov [bp-0x8],ax
00000A35  8B46FE            mov ax,[bp-0x2]
00000A38  3B46F8            cmp ax,[bp-0x8]
00000A3B  7E06              jng 0xa43
00000A3D  8B46F8            mov ax,[bp-0x8]
00000A40  8946FE            mov [bp-0x2],ax
00000A43  8A46F5            mov al,[bp-0xb]
00000A46  98                cbw
00000A47  D1E0              shl ax,1
00000A49  8B56FE            mov dx,[bp-0x2]
00000A4C  8BD8              mov bx,ax
00000A4E  89976C2C          mov [bx+0x2c6c],dx
00000A52  8A46F4            mov al,[bp-0xc]
00000A55  98                cbw
00000A56  D1E0              shl ax,1
00000A58  8B56FC            mov dx,[bp-0x4]
00000A5B  8BD8              mov bx,ax
00000A5D  89976C2C          mov [bx+0x2c6c],dx
00000A61  8936722C          mov [0x2c72],si
00000A65  C706D0450000      mov word [0x45d0],0x0
00000A6B  EB2B              jmp short 0xa98
00000A6D  A0AA37            mov al,[0x37aa]
00000A70  98                cbw
00000A71  D1E0              shl ax,1
00000A73  D1E0              shl ax,1
00000A75  0306D045          add ax,[0x45d0]
00000A79  8BD8              mov bx,ax
00000A7B  8A87E72E          mov al,[bx+0x2ee7]
00000A7F  98                cbw
00000A80  8B1ED045          mov bx,[0x45d0]
00000A84  D1E3              shl bx,1
00000A86  2B87442C          sub ax,[bx+0x2c44]
00000A8A  8B1ED045          mov bx,[0x45d0]
00000A8E  D1E3              shl bx,1
00000A90  89873E30          mov [bx+0x303e],ax
00000A94  FF06D045          inc word [0x45d0]
00000A98  833ED04503        cmp word [0x45d0],byte +0x3
00000A9D  7ECE              jng 0xa6d
00000A9F  5F                pop di
00000AA0  5E                pop si
00000AA1  8BE5              mov sp,bp
00000AA3  5D                pop bp
00000AA4  CB                retf

; ---- 11A3:0AA5 ----
00000AA5  55                push bp
00000AA6  8BEC              mov bp,sp
00000AA8  83EC08            sub sp,byte +0x8
00000AAB  56                push si
00000AAC  57                push di
00000AAD  8B7E06            mov di,[bp+0x6]
00000AB0  8A85E408          mov al,[di+0x8e4]
00000AB4  98                cbw
00000AB5  8946FE            mov [bp-0x2],ax
00000AB8  8A85E708          mov al,[di+0x8e7]
00000ABC  98                cbw
00000ABD  8946FC            mov [bp-0x4],ax
00000AC0  A0AA37            mov al,[0x37aa]
00000AC3  98                cbw
00000AC4  D1E0              shl ax,1
00000AC6  D1E0              shl ax,1
00000AC8  D1E0              shl ax,1
00000ACA  8B56FE            mov dx,[bp-0x2]
00000ACD  D1E2              shl dx,1
00000ACF  03C2              add ax,dx
00000AD1  8BD8              mov bx,ax
00000AD3  8B87F43C          mov ax,[bx+0x3cf4]
00000AD7  50                push ax
00000AD8  A0262F            mov al,[0x2f26]
00000ADB  98                cbw
00000ADC  8BD8              mov bx,ax
00000ADE  58                pop ax
00000ADF  99                cwd
00000AE0  F7FB              idiv bx
00000AE2  8946FA            mov [bp-0x6],ax
00000AE5  8B76FA            mov si,[bp-0x6]
00000AE8  A0AA37            mov al,[0x37aa]
00000AEB  98                cbw
00000AEC  D1E0              shl ax,1
00000AEE  D1E0              shl ax,1
00000AF0  D1E0              shl ax,1
00000AF2  8B56FC            mov dx,[bp-0x4]
00000AF5  D1E2              shl dx,1
00000AF7  03C2              add ax,dx
00000AF9  8BD8              mov bx,ax
00000AFB  8B87F43C          mov ax,[bx+0x3cf4]
00000AFF  2B4608            sub ax,[bp+0x8]
00000B02  50                push ax
00000B03  A0A248            mov al,[0x48a2]
00000B06  98                cbw
00000B07  8BD8              mov bx,ax
00000B09  58                pop ax
00000B0A  99                cwd
00000B0B  F7FB              idiv bx
00000B0D  8946F8            mov [bp-0x8],ax
00000B10  A0AA37            mov al,[0x37aa]
00000B13  98                cbw
00000B14  D1E0              shl ax,1
00000B16  D1E0              shl ax,1
00000B18  8BD8              mov bx,ax
00000B1A  8A81E72E          mov al,[bx+di+0x2ee7]
00000B1E  98                cbw
00000B1F  8BC8              mov cx,ax
00000B21  8B46F8            mov ax,[bp-0x8]
00000B24  03C1              add ax,cx
00000B26  3BC6              cmp ax,si
00000B28  7D07              jnl 0xb31
00000B2A  8B46F8            mov ax,[bp-0x8]
00000B2D  03C1              add ax,cx
00000B2F  8BF0              mov si,ax
00000B31  8BC6              mov ax,si
00000B33  EB00              jmp short 0xb35
00000B35  5F                pop di
00000B36  5E                pop si
00000B37  8BE5              mov sp,bp
00000B39  5D                pop bp
00000B3A  CB                retf

; ---- 11A3:0B3B ----
00000B3B  55                push bp
00000B3C  8BEC              mov bp,sp
00000B3E  83EC08            sub sp,byte +0x8
00000B41  A0AA37            mov al,[0x37aa]
00000B44  98                cbw
00000B45  D1E0              shl ax,1
00000B47  8BD8              mov bx,ax
00000B49  8A87F640          mov al,[bx+0x40f6]
00000B4D  8846FA            mov [bp-0x6],al
00000B50  8A46FA            mov al,[bp-0x6]
00000B53  B400              mov ah,0x0
00000B55  8BD8              mov bx,ax
00000B57  8A87E408          mov al,[bx+0x8e4]
00000B5B  8846F9            mov [bp-0x7],al
00000B5E  C606002F00        mov byte [0x2f00],0x0
00000B63  EB4D              jmp short 0xbb2
00000B65  C606B02E00        mov byte [0x2eb0],0x0
00000B6A  EB35              jmp short 0xba1
00000B6C  A0B02E            mov al,[0x2eb0]
00000B6F  98                cbw
00000B70  BA0600            mov dx,0x6
00000B73  F7EA              imul dx
00000B75  50                push ax
00000B76  A0002F            mov al,[0x2f00]
00000B79  98                cbw
00000B7A  5B                pop bx
00000B7B  03D8              add bx,ax
00000B7D  8A874244          mov al,[bx+0x4442]
00000B81  3A06AA37          cmp al,[0x37aa]
00000B85  7516              jnz 0xb9d
00000B87  A0B02E            mov al,[0x2eb0]
00000B8A  98                cbw
00000B8B  BA1A00            mov dx,0x1a
00000B8E  F7EA              imul dx
00000B90  50                push ax
00000B91  A0002F            mov al,[0x2f00]
00000B94  98                cbw
00000B95  5B                pop bx
00000B96  03D8              add bx,ax
00000B98  C687F44864        mov byte [bx+0x48f4],0x64
00000B9D  FE06B02E          inc byte [0x2eb0]
00000BA1  A0B02E            mov al,[0x2eb0]
00000BA4  98                cbw
00000BA5  8B16FC08          mov dx,[0x8fc]
00000BA9  4A                dec dx
00000BAA  3BC2              cmp ax,dx
00000BAC  7EBE              jng 0xb6c
00000BAE  FE06002F          inc byte [0x2f00]
00000BB2  A0002F            mov al,[0x2f00]
00000BB5  98                cbw
00000BB6  8B16FE08          mov dx,[0x8fe]
00000BBA  4A                dec dx
00000BBB  3BC2              cmp ax,dx
00000BBD  7EA6              jng 0xb65
00000BBF  C646FE00          mov byte [bp-0x2],0x0
00000BC3  E96201            jmp 0xd28
00000BC6  8A46FE            mov al,[bp-0x2]
00000BC9  B400              mov ah,0x0
00000BCB  0BC0              or ax,ax
00000BCD  740C              jz 0xbdb
00000BCF  3D0100            cmp ax,0x1
00000BD2  740F              jz 0xbe3
00000BD4  3D0200            cmp ax,0x2
00000BD7  7412              jz 0xbeb
00000BD9  EB16              jmp short 0xbf1
00000BDB  8A46F9            mov al,[bp-0x7]
00000BDE  8846FC            mov [bp-0x4],al
00000BE1  EB0E              jmp short 0xbf1
00000BE3  8A46FA            mov al,[bp-0x6]
00000BE6  8846FC            mov [bp-0x4],al
00000BE9  EB06              jmp short 0xbf1
00000BEB  C646FC03          mov byte [bp-0x4],0x3
00000BEF  EB00              jmp short 0xbf1
00000BF1  8A46FC            mov al,[bp-0x4]
00000BF4  B400              mov ah,0x0
00000BF6  D1E0              shl ax,1
00000BF8  8BD8              mov bx,ax
00000BFA  8A876C2C          mov al,[bx+0x2c6c]
00000BFE  8846FD            mov [bp-0x3],al
00000C01  C646FF00          mov byte [bp-0x1],0x0
00000C05  E91401            jmp 0xd1c
00000C08  807EFD00          cmp byte [bp-0x3],0x0
00000C0C  7703              ja 0xc11
00000C0E  E91401            jmp 0xd25
00000C11  C606002F00        mov byte [0x2f00],0x0
00000C16  E9F000            jmp 0xd09
00000C19  C606B02E00        mov byte [0x2eb0],0x0
00000C1E  E9D400            jmp 0xcf5
00000C21  807EFD00          cmp byte [bp-0x3],0x0
00000C25  7703              ja 0xc2a
00000C27  E9DB00            jmp 0xd05
00000C2A  A0B02E            mov al,[0x2eb0]
00000C2D  98                cbw
00000C2E  BA0600            mov dx,0x6
00000C31  F7EA              imul dx
00000C33  50                push ax
00000C34  A0002F            mov al,[0x2f00]
00000C37  98                cbw
00000C38  5B                pop bx
00000C39  03D8              add bx,ax
00000C3B  8A874244          mov al,[bx+0x4442]
00000C3F  3A06AA37          cmp al,[0x37aa]
00000C43  7518              jnz 0xc5d
00000C45  A0B02E            mov al,[0x2eb0]
00000C48  98                cbw
00000C49  BA0600            mov dx,0x6
00000C4C  F7EA              imul dx
00000C4E  50                push ax
00000C4F  A0002F            mov al,[0x2f00]
00000C52  98                cbw
00000C53  5B                pop bx
00000C54  03D8              add bx,ax
00000C56  80BFB12E04        cmp byte [bx+0x2eb1],0x4
00000C5B  7503              jnz 0xc60
00000C5D  E99100            jmp 0xcf1
00000C60  A0B02E            mov al,[0x2eb0]
00000C63  98                cbw
00000C64  BA1A00            mov dx,0x1a
00000C67  F7EA              imul dx
00000C69  50                push ax
00000C6A  A0002F            mov al,[0x2f00]
00000C6D  98                cbw
00000C6E  5B                pop bx
00000C6F  03D8              add bx,ax
00000C71  80BFF44864        cmp byte [bx+0x48f4],0x64
00000C76  7402              jz 0xc7a
00000C78  EB77              jmp short 0xcf1
00000C7A  8A46FE            mov al,[bp-0x2]
00000C7D  B400              mov ah,0x0
00000C7F  0BC0              or ax,ax
00000C81  740C              jz 0xc8f
00000C83  3D0100            cmp ax,0x1
00000C86  741F              jz 0xca7
00000C88  3D0200            cmp ax,0x2
00000C8B  7432              jz 0xcbf
00000C8D  EB41              jmp short 0xcd0
00000C8F  8A46FC            mov al,[bp-0x4]
00000C92  B400              mov ah,0x0
00000C94  50                push ax
00000C95  8A46FF            mov al,[bp-0x1]
00000C98  B400              mov ah,0x0
00000C9A  50                push ax
00000C9B  9A350DA311        call 0x11a3:0xd35
00000CA0  59                pop cx
00000CA1  59                pop cx
00000CA2  8846FB            mov [bp-0x5],al
00000CA5  EB29              jmp short 0xcd0
00000CA7  8A46FC            mov al,[bp-0x4]
00000CAA  B400              mov ah,0x0
00000CAC  50                push ax
00000CAD  8A46FF            mov al,[bp-0x1]
00000CB0  B400              mov ah,0x0
00000CB2  50                push ax
00000CB3  9A010EA311        call 0x11a3:0xe01
00000CB8  59                pop cx
00000CB9  59                pop cx
00000CBA  8846FB            mov [bp-0x5],al
00000CBD  EB11              jmp short 0xcd0
00000CBF  8A46FF            mov al,[bp-0x1]
00000CC2  B400              mov ah,0x0
00000CC4  50                push ax
00000CC5  9AB80EA311        call 0x11a3:0xeb8
00000CCA  59                pop cx
00000CCB  8846FB            mov [bp-0x5],al
00000CCE  EB00              jmp short 0xcd0
00000CD0  807EFB01          cmp byte [bp-0x5],0x1
00000CD4  751B              jnz 0xcf1
00000CD6  A0B02E            mov al,[0x2eb0]
00000CD9  98                cbw
00000CDA  BA1A00            mov dx,0x1a
00000CDD  F7EA              imul dx
00000CDF  50                push ax
00000CE0  A0002F            mov al,[0x2f00]
00000CE3  98                cbw
00000CE4  5B                pop bx
00000CE5  03D8              add bx,ax
00000CE7  8A46FC            mov al,[bp-0x4]
00000CEA  8887F448          mov [bx+0x48f4],al
00000CEE  FE4EFD            dec byte [bp-0x3]
00000CF1  FE06B02E          inc byte [0x2eb0]
00000CF5  A0B02E            mov al,[0x2eb0]
00000CF8  98                cbw
00000CF9  8B16FC08          mov dx,[0x8fc]
00000CFD  4A                dec dx
00000CFE  3BC2              cmp ax,dx
00000D00  7F03              jg 0xd05
00000D02  E91CFF            jmp 0xc21
00000D05  FE06002F          inc byte [0x2f00]
00000D09  A0002F            mov al,[0x2f00]
00000D0C  98                cbw
00000D0D  8B16FE08          mov dx,[0x8fe]
00000D11  4A                dec dx
00000D12  3BC2              cmp ax,dx
00000D14  7F03              jg 0xd19
00000D16  E900FF            jmp 0xc19
00000D19  FE46FF            inc byte [bp-0x1]
00000D1C  807EFF0B          cmp byte [bp-0x1],0xb
00000D20  7703              ja 0xd25
00000D22  E9E3FE            jmp 0xc08
00000D25  FE46FE            inc byte [bp-0x2]
00000D28  807EFE02          cmp byte [bp-0x2],0x2
00000D2C  7703              ja 0xd31
00000D2E  E995FE            jmp 0xbc6
00000D31  8BE5              mov sp,bp
00000D33  5D                pop bp
00000D34  CB                retf

; ---- 11A3:0D35 ----
00000D35  55                push bp
00000D36  8BEC              mov bp,sp
00000D38  56                push si
00000D39  57                push di
00000D3A  A0B02E            mov al,[0x2eb0]
00000D3D  98                cbw
00000D3E  BA0600            mov dx,0x6
00000D41  F7EA              imul dx
00000D43  50                push ax
00000D44  A0002F            mov al,[0x2f00]
00000D47  98                cbw
00000D48  5B                pop bx
00000D49  03D8              add bx,ax
00000D4B  8A87DA45          mov al,[bx+0x45da]
00000D4F  98                cbw
00000D50  8B5E08            mov bx,[bp+0x8]
00000D53  D1E3              shl bx,1
00000D55  D1E3              shl bx,1
00000D57  03D8              add bx,ax
00000D59  8A870209          mov al,[bx+0x902]
00000D5D  98                cbw
00000D5E  8BC8              mov cx,ax
00000D60  83F902            cmp cx,byte +0x2
00000D63  751D              jnz 0xd82
00000D65  A0B02E            mov al,[0x2eb0]
00000D68  98                cbw
00000D69  BA0600            mov dx,0x6
00000D6C  F7EA              imul dx
00000D6E  50                push ax
00000D6F  A0002F            mov al,[0x2f00]
00000D72  98                cbw
00000D73  5B                pop bx
00000D74  03D8              add bx,ax
00000D76  80BFDA4503        cmp byte [bx+0x45da],0x3
00000D7B  7405              jz 0xd82
00000D7D  BE0100            mov si,0x1
00000D80  EB2A              jmp short 0xdac
00000D82  A0B02E            mov al,[0x2eb0]
00000D85  98                cbw
00000D86  BA0600            mov dx,0x6
00000D89  F7EA              imul dx
00000D8B  50                push ax
00000D8C  A0002F            mov al,[0x2f00]
00000D8F  98                cbw
00000D90  5B                pop bx
00000D91  03D8              add bx,ax
00000D93  80BFDA4503        cmp byte [bx+0x45da],0x3
00000D98  7505              jnz 0xd9f
00000D9A  BE0200            mov si,0x2
00000D9D  EB0D              jmp short 0xdac
00000D9F  83F901            cmp cx,byte +0x1
00000DA2  7505              jnz 0xda9
00000DA4  BE0300            mov si,0x3
00000DA7  EB03              jmp short 0xdac
00000DA9  BE0400            mov si,0x4
00000DAC  A0B02E            mov al,[0x2eb0]
00000DAF  98                cbw
00000DB0  BA0600            mov dx,0x6
00000DB3  F7EA              imul dx
00000DB5  50                push ax
00000DB6  A0002F            mov al,[0x2f00]
00000DB9  98                cbw
00000DBA  5B                pop bx
00000DBB  03D8              add bx,ax
00000DBD  8A87B12E          mov al,[bx+0x2eb1]
00000DC1  98                cbw
00000DC2  3B4608            cmp ax,[bp+0x8]
00000DC5  7505              jnz 0xdcc
00000DC7  BF0100            mov di,0x1
00000DCA  EB20              jmp short 0xdec
00000DCC  A0B02E            mov al,[0x2eb0]
00000DCF  98                cbw
00000DD0  BA0600            mov dx,0x6
00000DD3  F7EA              imul dx
00000DD5  50                push ax
00000DD6  A0002F            mov al,[0x2f00]
00000DD9  98                cbw
00000DDA  5B                pop bx
00000DDB  03D8              add bx,ax
00000DDD  80BFB12EFE        cmp byte [bx+0x2eb1],0xfe
00000DE2  7505              jnz 0xde9
00000DE4  BF0200            mov di,0x2
00000DE7  EB03              jmp short 0xdec
00000DE9  BF0300            mov di,0x3
00000DEC  8BC6              mov ax,si
00000DEE  F7EF              imul di
00000DF0  3B4606            cmp ax,[bp+0x6]
00000DF3  7504              jnz 0xdf9
00000DF5  B001              mov al,0x1
00000DF7  EB04              jmp short 0xdfd
00000DF9  B000              mov al,0x0
00000DFB  EB00              jmp short 0xdfd
00000DFD  5F                pop di
00000DFE  5E                pop si
00000DFF  5D                pop bp
00000E00  CB                retf

; ---- 11A3:0E01 ----
00000E01  55                push bp
00000E02  8BEC              mov bp,sp
00000E04  56                push si
00000E05  57                push di
00000E06  A0B02E            mov al,[0x2eb0]
00000E09  98                cbw
00000E0A  BA0600            mov dx,0x6
00000E0D  F7EA              imul dx
00000E0F  50                push ax
00000E10  A0002F            mov al,[0x2f00]
00000E13  98                cbw
00000E14  5B                pop bx
00000E15  03D8              add bx,ax
00000E17  8A87DA45          mov al,[bx+0x45da]
00000E1B  98                cbw
00000E1C  8B5E08            mov bx,[bp+0x8]
00000E1F  D1E3              shl bx,1
00000E21  D1E3              shl bx,1
00000E23  03D8              add bx,ax
00000E25  8A870209          mov al,[bx+0x902]
00000E29  98                cbw
00000E2A  8BC8              mov cx,ax
00000E2C  A0B02E            mov al,[0x2eb0]
00000E2F  98                cbw
00000E30  BA0600            mov dx,0x6
00000E33  F7EA              imul dx
00000E35  50                push ax
00000E36  A0002F            mov al,[0x2f00]
00000E39  98                cbw
00000E3A  5B                pop bx
00000E3B  03D8              add bx,ax
00000E3D  80BFDA4503        cmp byte [bx+0x45da],0x3
00000E42  7505              jnz 0xe49
00000E44  BE0100            mov si,0x1
00000E47  EB17              jmp short 0xe60
00000E49  83F902            cmp cx,byte +0x2
00000E4C  7505              jnz 0xe53
00000E4E  BE0200            mov si,0x2
00000E51  EB0D              jmp short 0xe60
00000E53  83F901            cmp cx,byte +0x1
00000E56  7505              jnz 0xe5d
00000E58  BE0300            mov si,0x3
00000E5B  EB03              jmp short 0xe60
00000E5D  BE0400            mov si,0x4
00000E60  A0B02E            mov al,[0x2eb0]
00000E63  98                cbw
00000E64  BA0600            mov dx,0x6
00000E67  F7EA              imul dx
00000E69  50                push ax
00000E6A  A0002F            mov al,[0x2f00]
00000E6D  98                cbw
00000E6E  5B                pop bx
00000E6F  03D8              add bx,ax
00000E71  8A87B12E          mov al,[bx+0x2eb1]
00000E75  98                cbw
00000E76  3B4608            cmp ax,[bp+0x8]
00000E79  7505              jnz 0xe80
00000E7B  BF0100            mov di,0x1
00000E7E  EB20              jmp short 0xea0
00000E80  A0B02E            mov al,[0x2eb0]
00000E83  98                cbw
00000E84  BA0600            mov dx,0x6
00000E87  F7EA              imul dx
00000E89  50                push ax
00000E8A  A0002F            mov al,[0x2f00]
00000E8D  98                cbw
00000E8E  5B                pop bx
00000E8F  03D8              add bx,ax
00000E91  80BFB12EFE        cmp byte [bx+0x2eb1],0xfe
00000E96  7505              jnz 0xe9d
00000E98  BF0200            mov di,0x2
00000E9B  EB03              jmp short 0xea0
00000E9D  BF0300            mov di,0x3
00000EA0  8BC6              mov ax,si
00000EA2  F7EF              imul di
00000EA4  8B5606            mov dx,[bp+0x6]
00000EA7  42                inc dx
00000EA8  3BC2              cmp ax,dx
00000EAA  7504              jnz 0xeb0
00000EAC  B001              mov al,0x1
00000EAE  EB04              jmp short 0xeb4
00000EB0  B000              mov al,0x0
00000EB2  EB00              jmp short 0xeb4
00000EB4  5F                pop di
00000EB5  5E                pop si
00000EB6  5D                pop bp
00000EB7  CB                retf

; ---- 11A3:0EB8 ----
00000EB8  55                push bp
00000EB9  8BEC              mov bp,sp
00000EBB  B100              mov cl,0x0
00000EBD  A0B02E            mov al,[0x2eb0]
00000EC0  98                cbw
00000EC1  BA0600            mov dx,0x6
00000EC4  F7EA              imul dx
00000EC6  50                push ax
00000EC7  A0002F            mov al,[0x2f00]
00000ECA  98                cbw
00000ECB  5B                pop bx
00000ECC  03D8              add bx,ax
00000ECE  80BFDA4503        cmp byte [bx+0x45da],0x3
00000ED3  7504              jnz 0xed9
00000ED5  B000              mov al,0x0
00000ED7  EB53              jmp short 0xf2c
00000ED9  8B4606            mov ax,[bp+0x6]
00000EDC  0BC0              or ax,ax
00000EDE  740C              jz 0xeec
00000EE0  3D0100            cmp ax,0x1
00000EE3  7423              jz 0xf08
00000EE5  3D0200            cmp ax,0x2
00000EE8  743A              jz 0xf24
00000EEA  EB3C              jmp short 0xf28
00000EEC  A0B02E            mov al,[0x2eb0]
00000EEF  98                cbw
00000EF0  BA0600            mov dx,0x6
00000EF3  F7EA              imul dx
00000EF5  50                push ax
00000EF6  A0002F            mov al,[0x2f00]
00000EF9  98                cbw
00000EFA  5B                pop bx
00000EFB  03D8              add bx,ax
00000EFD  80BFB12E03        cmp byte [bx+0x2eb1],0x3
00000F02  7502              jnz 0xf06
00000F04  B101              mov cl,0x1
00000F06  EB20              jmp short 0xf28
00000F08  A0B02E            mov al,[0x2eb0]
00000F0B  98                cbw
00000F0C  BA0600            mov dx,0x6
00000F0F  F7EA              imul dx
00000F11  50                push ax
00000F12  A0002F            mov al,[0x2f00]
00000F15  98                cbw
00000F16  5B                pop bx
00000F17  03D8              add bx,ax
00000F19  80BFB12EFE        cmp byte [bx+0x2eb1],0xfe
00000F1E  7502              jnz 0xf22
00000F20  B101              mov cl,0x1
00000F22  EB04              jmp short 0xf28
00000F24  B101              mov cl,0x1
00000F26  EB00              jmp short 0xf28
00000F28  8AC1              mov al,cl
00000F2A  EB00              jmp short 0xf2c
00000F2C  5D                pop bp
00000F2D  CB                retf

; ---- 11A3:0F2E ----
00000F2E  55                push bp
00000F2F  8BEC              mov bp,sp
00000F31  56                push si
00000F32  33F6              xor si,si
00000F34  C606002F00        mov byte [0x2f00],0x0
00000F39  E9F000            jmp 0x102c
00000F3C  C606B02E00        mov byte [0x2eb0],0x0
00000F41  E9D400            jmp 0x1018
00000F44  A0B02E            mov al,[0x2eb0]
00000F47  98                cbw
00000F48  BA0600            mov dx,0x6
00000F4B  F7EA              imul dx
00000F4D  50                push ax
00000F4E  A0002F            mov al,[0x2f00]
00000F51  98                cbw
00000F52  5B                pop bx
00000F53  03D8              add bx,ax
00000F55  8A874244          mov al,[bx+0x4442]
00000F59  3A06AA37          cmp al,[0x37aa]
00000F5D  7403              jz 0xf62
00000F5F  E9B200            jmp 0x1014
00000F62  A0B02E            mov al,[0x2eb0]
00000F65  98                cbw
00000F66  BA0600            mov dx,0x6
00000F69  F7EA              imul dx
00000F6B  50                push ax
00000F6C  A0002F            mov al,[0x2f00]
00000F6F  98                cbw
00000F70  5B                pop bx
00000F71  03D8              add bx,ax
00000F73  80BFB12E04        cmp byte [bx+0x2eb1],0x4
00000F78  7503              jnz 0xf7d
00000F7A  E99700            jmp 0x1014
00000F7D  A0B02E            mov al,[0x2eb0]
00000F80  98                cbw
00000F81  BA1A00            mov dx,0x1a
00000F84  F7EA              imul dx
00000F86  50                push ax
00000F87  A0002F            mov al,[0x2f00]
00000F8A  98                cbw
00000F8B  5B                pop bx
00000F8C  03D8              add bx,ax
00000F8E  80BFF44864        cmp byte [bx+0x48f4],0x64
00000F93  7502              jnz 0xf97
00000F95  EB7D              jmp short 0x1014
00000F97  A0B02E            mov al,[0x2eb0]
00000F9A  98                cbw
00000F9B  BA0600            mov dx,0x6
00000F9E  F7EA              imul dx
00000FA0  50                push ax
00000FA1  A0002F            mov al,[0x2f00]
00000FA4  98                cbw
00000FA5  5B                pop bx
00000FA6  03D8              add bx,ax
00000FA8  8A87B12E          mov al,[bx+0x2eb1]
00000FAC  50                push ax
00000FAD  A0B02E            mov al,[0x2eb0]
00000FB0  98                cbw
00000FB1  BA1A00            mov dx,0x1a
00000FB4  F7EA              imul dx
00000FB6  50                push ax
00000FB7  A0002F            mov al,[0x2f00]
00000FBA  98                cbw
00000FBB  5B                pop bx
00000FBC  03D8              add bx,ax
00000FBE  58                pop ax
00000FBF  3A87F448          cmp al,[bx+0x48f4]
00000FC3  7502              jnz 0xfc7
00000FC5  EB4D              jmp short 0x1014
00000FC7  A0B02E            mov al,[0x2eb0]
00000FCA  98                cbw
00000FCB  BA1A00            mov dx,0x1a
00000FCE  F7EA              imul dx
00000FD0  50                push ax
00000FD1  A0002F            mov al,[0x2f00]
00000FD4  98                cbw
00000FD5  5B                pop bx
00000FD6  03D8              add bx,ax
00000FD8  8A87F448          mov al,[bx+0x48f4]
00000FDC  50                push ax
00000FDD  A0AA37            mov al,[0x37aa]
00000FE0  98                cbw
00000FE1  BA3600            mov dx,0x36
00000FE4  F7EA              imul dx
00000FE6  8BD8              mov bx,ax
00000FE8  58                pop ax
00000FE9  8880312F          mov [bx+si+0x2f31],al
00000FED  A0AA37            mov al,[0x37aa]
00000FF0  98                cbw
00000FF1  BA3600            mov dx,0x36
00000FF4  F7EA              imul dx
00000FF6  8A16B02E          mov dl,[0x2eb0]
00000FFA  8BD8              mov bx,ax
00000FFC  88905446          mov [bx+si+0x4654],dl
00001000  A0AA37            mov al,[0x37aa]
00001003  98                cbw
00001004  BA3600            mov dx,0x36
00001007  F7EA              imul dx
00001009  8A16002F          mov dl,[0x2f00]
0000100D  8BD8              mov bx,ax
0000100F  88902C47          mov [bx+si+0x472c],dl
00001013  46                inc si
00001014  FE06B02E          inc byte [0x2eb0]
00001018  A0B02E            mov al,[0x2eb0]
0000101B  98                cbw
0000101C  8B16FC08          mov dx,[0x8fc]
00001020  4A                dec dx
00001021  3BC2              cmp ax,dx
00001023  7F03              jg 0x1028
00001025  E91CFF            jmp 0xf44
00001028  FE06002F          inc byte [0x2f00]
0000102C  A0002F            mov al,[0x2f00]
0000102F  98                cbw
00001030  8B16FE08          mov dx,[0x8fe]
00001034  4A                dec dx
00001035  3BC2              cmp ax,dx
00001037  7F03              jg 0x103c
00001039  E900FF            jmp 0xf3c
0000103C  A0AA37            mov al,[0x37aa]
0000103F  98                cbw
00001040  D1E0              shl ax,1
00001042  8BD8              mov bx,ax
00001044  89B7EC48          mov [bx+0x48ec],si
00001048  5E                pop si
00001049  5D                pop bp
0000104A  CB                retf

; ---- 11A3:104B ----
0000104B  55                push bp
0000104C  8BEC              mov bp,sp
0000104E  56                push si
0000104F  57                push di
00001050  C706D0450000      mov word [0x45d0],0x0
00001056  EB1E              jmp short 0x1076
00001058  A0AA37            mov al,[0x37aa]
0000105B  98                cbw
0000105C  D1E0              shl ax,1
0000105E  D1E0              shl ax,1
00001060  D1E0              shl ax,1
00001062  8B16D045          mov dx,[0x45d0]
00001066  D1E2              shl dx,1
00001068  03C2              add ax,dx
0000106A  8BD8              mov bx,ax
0000106C  C78760450000      mov word [bx+0x4560],0x0
00001072  FF06D045          inc word [0x45d0]
00001076  833ED04503        cmp word [0x45d0],byte +0x3
0000107B  7EDB              jng 0x1058
0000107D  C706D0450000      mov word [0x45d0],0x0
00001083  E9BB00            jmp 0x1141
00001086  A0AA37            mov al,[0x37aa]
00001089  98                cbw
0000108A  D1E0              shl ax,1
0000108C  D1E0              shl ax,1
0000108E  D1E0              shl ax,1
00001090  8B1ED045          mov bx,[0x45d0]
00001094  50                push ax
00001095  8A87E408          mov al,[bx+0x8e4]
00001099  98                cbw
0000109A  D1E0              shl ax,1
0000109C  5B                pop bx
0000109D  03D8              add bx,ax
0000109F  8B876045          mov ax,[bx+0x4560]
000010A3  8B1ED045          mov bx,[0x45d0]
000010A7  D1E3              shl bx,1
000010A9  50                push ax
000010AA  8B87442C          mov ax,[bx+0x2c44]
000010AE  50                push ax
000010AF  A0262F            mov al,[0x2f26]
000010B2  98                cbw
000010B3  8BD0              mov dx,ax
000010B5  58                pop ax
000010B6  F7EA              imul dx
000010B8  5A                pop dx
000010B9  03D0              add dx,ax
000010BB  A0AA37            mov al,[0x37aa]
000010BE  98                cbw
000010BF  D1E0              shl ax,1
000010C1  D1E0              shl ax,1
000010C3  D1E0              shl ax,1
000010C5  8B1ED045          mov bx,[0x45d0]
000010C9  52                push dx
000010CA  50                push ax
000010CB  8A87E408          mov al,[bx+0x8e4]
000010CF  98                cbw
000010D0  D1E0              shl ax,1
000010D2  5B                pop bx
000010D3  03D8              add bx,ax
000010D5  58                pop ax
000010D6  89876045          mov [bx+0x4560],ax
000010DA  8B1ED045          mov bx,[0x45d0]
000010DE  D1E3              shl bx,1
000010E0  83BF3E3000        cmp word [bx+0x303e],byte +0x0
000010E5  7D56              jnl 0x113d
000010E7  A0AA37            mov al,[0x37aa]
000010EA  98                cbw
000010EB  D1E0              shl ax,1
000010ED  D1E0              shl ax,1
000010EF  D1E0              shl ax,1
000010F1  8B1ED045          mov bx,[0x45d0]
000010F5  50                push ax
000010F6  8A87E708          mov al,[bx+0x8e7]
000010FA  98                cbw
000010FB  D1E0              shl ax,1
000010FD  5B                pop bx
000010FE  03D8              add bx,ax
00001100  8B876045          mov ax,[bx+0x4560]
00001104  8B1ED045          mov bx,[0x45d0]
00001108  D1E3              shl bx,1
0000110A  50                push ax
0000110B  8B873E30          mov ax,[bx+0x303e]
0000110F  F7D8              neg ax
00001111  50                push ax
00001112  A0A248            mov al,[0x48a2]
00001115  98                cbw
00001116  8BD0              mov dx,ax
00001118  58                pop ax
00001119  F7EA              imul dx
0000111B  5A                pop dx
0000111C  03D0              add dx,ax
0000111E  A0AA37            mov al,[0x37aa]
00001121  98                cbw
00001122  D1E0              shl ax,1
00001124  D1E0              shl ax,1
00001126  D1E0              shl ax,1
00001128  8B1ED045          mov bx,[0x45d0]
0000112C  52                push dx
0000112D  50                push ax
0000112E  8A87E708          mov al,[bx+0x8e7]
00001132  98                cbw
00001133  D1E0              shl ax,1
00001135  5B                pop bx
00001136  03D8              add bx,ax
00001138  58                pop ax
00001139  89876045          mov [bx+0x4560],ax
0000113D  FF06D045          inc word [0x45d0]
00001141  833ED04502        cmp word [0x45d0],byte +0x2
00001146  7F03              jg 0x114b
00001148  E93BFF            jmp 0x1086
0000114B  A1FA3C            mov ax,[0x3cfa]
0000114E  0306023D          add ax,[0x3d02]
00001152  03060A3D          add ax,[0x3d0a]
00001156  0306123D          add ax,[0x3d12]
0000115A  BB0800            mov bx,0x8
0000115D  99                cwd
0000115E  F7FB              idiv bx
00001160  50                push ax
00001161  A0AA37            mov al,[0x37aa]
00001164  98                cbw
00001165  D1E0              shl ax,1
00001167  D1E0              shl ax,1
00001169  D1E0              shl ax,1
0000116B  8BD8              mov bx,ax
0000116D  58                pop ax
0000116E  89876645          mov [bx+0x4566],ax
00001172  833E443000        cmp word [0x3044],byte +0x0
00001177  7C10              jl 0x1189
00001179  A0AA37            mov al,[0x37aa]
0000117C  98                cbw
0000117D  D1E0              shl ax,1
0000117F  8BD8              mov bx,ax
00001181  C78746300000      mov word [bx+0x3046],0x0
00001187  EB12              jmp short 0x119b
00001189  A0AA37            mov al,[0x37aa]
0000118C  98                cbw
0000118D  D1E0              shl ax,1
0000118F  8B164430          mov dx,[0x3044]
00001193  F7DA              neg dx
00001195  8BD8              mov bx,ax
00001197  89974630          mov [bx+0x3046],dx
0000119B  C706D0450000      mov word [0x45d0],0x0
000011A1  E98700            jmp 0x122b
000011A4  A0AA37            mov al,[0x37aa]
000011A7  98                cbw
000011A8  D1E0              shl ax,1
000011AA  D1E0              shl ax,1
000011AC  0306D045          add ax,[0x45d0]
000011B0  8BD8              mov bx,ax
000011B2  8A87E72E          mov al,[bx+0x2ee7]
000011B6  98                cbw
000011B7  BB0300            mov bx,0x3
000011BA  99                cwd
000011BB  F7FB              idiv bx
000011BD  BA0A00            mov dx,0xa
000011C0  F7EA              imul dx
000011C2  BA3C00            mov dx,0x3c
000011C5  2BD0              sub dx,ax
000011C7  8BFA              mov di,dx
000011C9  83FF0A            cmp di,byte +0xa
000011CC  7D03              jnl 0x11d1
000011CE  BF0A00            mov di,0xa
000011D1  8B1ED045          mov bx,[0x45d0]
000011D5  D1E3              shl bx,1
000011D7  8B879848          mov ax,[bx+0x4898]
000011DB  F7EF              imul di
000011DD  99                cwd
000011DE  52                push dx
000011DF  50                push ax
000011E0  A0AA37            mov al,[0x37aa]
000011E3  98                cbw
000011E4  D1E0              shl ax,1
000011E6  D1E0              shl ax,1
000011E8  8BD8              mov bx,ax
000011EA  FFB7D43C          push word [bx+0x3cd4]
000011EE  FFB7D23C          push word [bx+0x3cd2]
000011F2  9A894F0000        call 0x0:0x4f89
000011F7  50                push ax
000011F8  52                push dx
000011F9  33D2              xor dx,dx
000011FB  B80A00            mov ax,0xa
000011FE  59                pop cx
000011FF  5B                pop bx
00001200  9A4E4F0000        call 0x0:0x4f4e
00001205  8BF0              mov si,ax
00001207  83FE1E            cmp si,byte +0x1e
0000120A  7D03              jnl 0x120f
0000120C  BE1E00            mov si,0x1e
0000120F  A0AA37            mov al,[0x37aa]
00001212  98                cbw
00001213  D1E0              shl ax,1
00001215  D1E0              shl ax,1
00001217  D1E0              shl ax,1
00001219  8B16D045          mov dx,[0x45d0]
0000121D  D1E2              shl dx,1
0000121F  03C2              add ax,dx
00001221  8BD8              mov bx,ax
00001223  89B72A45          mov [bx+0x452a],si
00001227  FF06D045          inc word [0x45d0]
0000122B  833ED04502        cmp word [0x45d0],byte +0x2
00001230  7F03              jg 0x1235
00001232  E96FFF            jmp 0x11a4
00001235  A0AA37            mov al,[0x37aa]
00001238  98                cbw
00001239  D1E0              shl ax,1
0000123B  D1E0              shl ax,1
0000123D  D1E0              shl ax,1
0000123F  8BD8              mov bx,ax
00001241  8B876645          mov ax,[bx+0x4566]
00001245  50                push ax
00001246  A0AA37            mov al,[0x37aa]
00001249  98                cbw
0000124A  D1E0              shl ax,1
0000124C  D1E0              shl ax,1
0000124E  D1E0              shl ax,1
