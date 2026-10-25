; segment 0E31  file offset 0x11910  size 0x6D0
00000000  59                pop cx
00000001  5D                pop bp
00000002  CB                retf

; ---- 0E31:0003 ----
00000003  55                push bp
00000004  8BEC              mov bp,sp
00000006  9ACD01310E        call 0xe31:0x1cd
0000000B  9A3D02310E        call 0xe31:0x23d
00000010  9A1F02FD21        call 0x21fd:0x21f
00000015  C606AA3700        mov byte [0x37aa],0x0
0000001A  E9B700            jmp 0xd4
0000001D  A0AA37            mov al,[0x37aa]
00000020  98                cbw
00000021  8BD8              mov bx,ax
00000023  C687FE40FF        mov byte [bx+0x40fe],0xff
00000028  A0AA37            mov al,[0x37aa]
0000002B  98                cbw
0000002C  D1E0              shl ax,1
0000002E  8BD8              mov bx,ax
00000030  C787AC4B0500      mov word [bx+0x4bac],0x5
00000036  A0AA37            mov al,[0x37aa]
00000039  98                cbw
0000003A  8BD8              mov bx,ax
0000003C  C687A84B00        mov byte [bx+0x4ba8],0x0
00000041  A0AA37            mov al,[0x37aa]
00000044  98                cbw
00000045  D1E0              shl ax,1
00000047  8BD8              mov bx,ax
00000049  C78720452800      mov word [bx+0x4520],0x28
0000004F  A1F208            mov ax,[0x8f2]
00000052  99                cwd
00000053  50                push ax
00000054  A0AA37            mov al,[0x37aa]
00000057  98                cbw
00000058  D1E0              shl ax,1
0000005A  D1E0              shl ax,1
0000005C  8BD8              mov bx,ax
0000005E  58                pop ax
0000005F  8987D23C          mov [bx+0x3cd2],ax
00000063  8997D43C          mov [bx+0x3cd4],dx
00000067  A0AA37            mov al,[0x37aa]
0000006A  98                cbw
0000006B  8BD8              mov bx,ax
0000006D  C687C24B00        mov byte [bx+0x4bc2],0x0
00000072  9A4901310E        call 0xe31:0x149
00000077  A0AA37            mov al,[0x37aa]
0000007A  98                cbw
0000007B  D1E0              shl ax,1
0000007D  8BD8              mov bx,ax
0000007F  C7870E303200      mov word [bx+0x300e],0x32
00000085  A0AA37            mov al,[0x37aa]
00000088  98                cbw
00000089  8BD8              mov bx,ax
0000008B  C687AF3702        mov byte [bx+0x37af],0x2
00000090  C706D0450000      mov word [0x45d0],0x0
00000096  EB31              jmp short 0xc9
00000098  A0AA37            mov al,[0x37aa]
0000009B  98                cbw
0000009C  D1E0              shl ax,1
0000009E  D1E0              shl ax,1
000000A0  0306D045          add ax,[0x45d0]
000000A4  8BD8              mov bx,ax
000000A6  C687E72E00        mov byte [bx+0x2ee7],0x0
000000AB  A0AA37            mov al,[0x37aa]
000000AE  98                cbw
000000AF  D1E0              shl ax,1
000000B1  D1E0              shl ax,1
000000B3  D1E0              shl ax,1
000000B5  8B16D045          mov dx,[0x45d0]
000000B9  D1E2              shl dx,1
000000BB  03C2              add ax,dx
000000BD  8BD8              mov bx,ax
000000BF  C787F43C4600      mov word [bx+0x3cf4],0x46
000000C5  FF06D045          inc word [0x45d0]
000000C9  833ED04503        cmp word [0x45d0],byte +0x3
000000CE  7EC8              jng 0x98
000000D0  FE06AA37          inc byte [0x37aa]
000000D4  803EAA3704        cmp byte [0x37aa],0x4
000000D9  7D03              jnl 0xde
000000DB  E93FFF            jmp 0x1d
000000DE  A1F208            mov ax,[0x8f2]
000000E1  BB2800            mov bx,0x28
000000E4  99                cwd
000000E5  F7FB              idiv bx
000000E7  99                cwd
000000E8  A3F03C            mov [0x3cf0],ax
000000EB  8916F23C          mov [0x3cf2],dx
000000EF  B87800            mov ax,0x78
000000F2  50                push ax
000000F3  9ABA01FD21        call 0x21fd:0x1ba
000000F8  59                pop cx
000000F9  052800            add ax,0x28
000000FC  A33644            mov [0x4436],ax
000000FF  C70638440000      mov word [0x4438],0x0
00000105  C706D0450000      mov word [0x45d0],0x0
0000010B  EB10              jmp short 0x11d
0000010D  8B1ED045          mov bx,[0x45d0]
00000111  D1E3              shl bx,1
00000113  C78798480300      mov word [bx+0x4898],0x3
00000119  FF06D045          inc word [0x45d0]
0000011D  833ED04503        cmp word [0x45d0],byte +0x3
00000122  7EE9              jng 0x10d
00000124  C7069A4B0000      mov word [0x4b9a],0x0
0000012A  C6068045FF        mov byte [0x4580],0xff
0000012F  C706F6400000      mov word [0x40f6],0x0
00000135  C706F8400200      mov word [0x40f8],0x2
0000013B  C706FA400100      mov word [0x40fa],0x1
00000141  C706FC400000      mov word [0x40fc],0x0
00000147  5D                pop bp
00000148  CB                retf

; ---- 0E31:0149 ----
00000149  55                push bp
0000014A  8BEC              mov bp,sp
0000014C  56                push si
0000014D  33F6              xor si,si
0000014F  EB11              jmp short 0x162
00000151  A0AA37            mov al,[0x37aa]
00000154  98                cbw
00000155  BA0600            mov dx,0x6
00000158  F7EA              imul dx
0000015A  8BD8              mov bx,ax
0000015C  C680CE4300        mov byte [bx+si+0x43ce],0x0
00000161  46                inc si
00000162  83FE05            cmp si,byte +0x5
00000165  7EEA              jng 0x151
00000167  A0AA37            mov al,[0x37aa]
0000016A  98                cbw
0000016B  D1E0              shl ax,1
0000016D  D1E0              shl ax,1
0000016F  8BD8              mov bx,ax
00000171  C7876C370000      mov word [bx+0x376c],0x0
00000177  A0AA37            mov al,[0x37aa]
0000017A  98                cbw
0000017B  D1E0              shl ax,1
0000017D  D1E0              shl ax,1
0000017F  8BD8              mov bx,ax
00000181  C7876E370000      mov word [bx+0x376e],0x0
00000187  A0AA37            mov al,[0x37aa]
0000018A  98                cbw
0000018B  D1E0              shl ax,1
0000018D  8BD8              mov bx,ax
0000018F  C787F82E0000      mov word [bx+0x2ef8],0x0
00000195  A0AA37            mov al,[0x37aa]
00000198  98                cbw
00000199  8BD8              mov bx,ax
0000019B  C6877C3701        mov byte [bx+0x377c],0x1
000001A0  A0AA37            mov al,[0x37aa]
000001A3  98                cbw
000001A4  D1E0              shl ax,1
000001A6  8BD8              mov bx,ax
000001A8  C787EC403200      mov word [bx+0x40ec],0x32
000001AE  A0AA37            mov al,[0x37aa]
000001B1  98                cbw
000001B2  D1E0              shl ax,1
000001B4  8BD8              mov bx,ax
000001B6  C787DC331E00      mov word [bx+0x33dc],0x1e
000001BC  A0AA37            mov al,[0x37aa]
000001BF  98                cbw
000001C0  D1E0              shl ax,1
000001C2  8BD8              mov bx,ax
000001C4  C78716300000      mov word [bx+0x3016],0x0
000001CA  5E                pop si
000001CB  5D                pop bp
000001CC  CB                retf

; ---- 0E31:01CD ----
000001CD  55                push bp
000001CE  8BEC              mov bp,sp
000001D0  9A6103310E        call 0xe31:0x361
000001D5  A1FC08            mov ax,[0x8fc]
000001D8  F72EFE08          imul word [0x8fe]
000001DC  A2052F            mov [0x2f05],al
000001DF  C606002F00        mov byte [0x2f00],0x0
000001E4  EB48              jmp short 0x22e
000001E6  C606B02E00        mov byte [0x2eb0],0x0
000001EB  EB30              jmp short 0x21d
000001ED  A0B02E            mov al,[0x2eb0]
000001F0  98                cbw
000001F1  BA0600            mov dx,0x6
000001F4  F7EA              imul dx
000001F6  50                push ax
000001F7  A0002F            mov al,[0x2f00]
000001FA  98                cbw
000001FB  5B                pop bx
000001FC  03D8              add bx,ax
000001FE  C6874244FF        mov byte [bx+0x4442],0xff
00000203  A0B02E            mov al,[0x2eb0]
00000206  98                cbw
00000207  BA0600            mov dx,0x6
0000020A  F7EA              imul dx
0000020C  50                push ax
0000020D  A0002F            mov al,[0x2f00]
00000210  98                cbw
00000211  5B                pop bx
00000212  03D8              add bx,ax
00000214  C687B12EFE        mov byte [bx+0x2eb1],0xfe
00000219  FE06B02E          inc byte [0x2eb0]
0000021D  A0B02E            mov al,[0x2eb0]
00000220  98                cbw
00000221  8B16FC08          mov dx,[0x8fc]
00000225  4A                dec dx
00000226  3BC2              cmp ax,dx
00000228  7EC3              jng 0x1ed
0000022A  FE06002F          inc byte [0x2f00]
0000022E  A0002F            mov al,[0x2f00]
00000231  98                cbw
00000232  8B16FE08          mov dx,[0x8fe]
00000236  4A                dec dx
00000237  3BC2              cmp ax,dx
00000239  7EAB              jng 0x1e6
0000023B  5D                pop bp
0000023C  CB                retf

; ---- 0E31:023D ----
0000023D  55                push bp
0000023E  8BEC              mov bp,sp
00000240  83EC16            sub sp,byte +0x16
00000243  C746EA0600        mov word [bp-0x16],0x6
00000248  C746EC0000        mov word [bp-0x14],0x0
0000024D  C746EE0400        mov word [bp-0x12],0x4
00000252  C746F00000        mov word [bp-0x10],0x0
00000257  C746F20200        mov word [bp-0xe],0x2
0000025C  C746F40000        mov word [bp-0xc],0x0
00000261  C746FC0000        mov word [bp-0x4],0x0
00000266  C746FE0000        mov word [bp-0x2],0x0
0000026B  E9BB00            jmp 0x329
0000026E  8B5EFC            mov bx,[bp-0x4]
00000271  C6871A2F00        mov byte [bx+0x2f1a],0x0
00000276  B80200            mov ax,0x2
00000279  50                push ax
0000027A  9ABA01FD21        call 0x21fd:0x1ba
0000027F  59                pop cx
00000280  0BC0              or ax,ax
00000282  7405              jz 0x289
00000284  B8FFFF            mov ax,0xffff
00000287  EB1D              jmp short 0x2a6
00000289  B80200            mov ax,0x2
0000028C  50                push ax
0000028D  9ABA01FD21        call 0x21fd:0x1ba
00000292  59                pop cx
00000293  0BC0              or ax,ax
00000295  7405              jz 0x29c
00000297  B80A00            mov ax,0xa
0000029A  EB0A              jmp short 0x2a6
0000029C  B80300            mov ax,0x3
0000029F  50                push ax
000002A0  9ABA01FD21        call 0x21fd:0x1ba
000002A5  59                pop cx
000002A6  8B5EFC            mov bx,[bp-0x4]
000002A9  D1E3              shl bx,1
000002AB  89878A34          mov [bx+0x348a],ax
000002AF  C746F8FFFF        mov word [bp-0x8],0xffff
000002B4  C746FAFFFF        mov word [bp-0x6],0xffff
000002B9  EB4E              jmp short 0x309
000002BB  B80200            mov ax,0x2
000002BE  50                push ax
000002BF  9ABA01FD21        call 0x21fd:0x1ba
000002C4  59                pop cx
000002C5  8846F7            mov [bp-0x9],al
000002C8  8A46F7            mov al,[bp-0x9]
000002CB  B400              mov ah,0x0
000002CD  D1E0              shl ax,1
000002CF  D1E0              shl ax,1
000002D1  8D56EA            lea dx,[bp-0x16]
000002D4  03C2              add ax,dx
000002D6  8BD8              mov bx,ax
000002D8  368B07            mov ax,[ss:bx]
000002DB  360B4702          or ax,[ss:bx+0x2]
000002DF  7502              jnz 0x2e3
000002E1  EB26              jmp short 0x309
000002E3  8A46F7            mov al,[bp-0x9]
000002E6  B400              mov ah,0x0
000002E8  D1E0              shl ax,1
000002EA  D1E0              shl ax,1
000002EC  8D56EA            lea dx,[bp-0x16]
000002EF  03C2              add ax,dx
000002F1  8BD8              mov bx,ax
000002F3  36832F01          sub word [ss:bx],byte +0x1
000002F7  36835F0200        sbb word [ss:bx+0x2],byte +0x0
000002FC  8A46F7            mov al,[bp-0x9]
000002FF  B400              mov ah,0x0
00000301  8946F8            mov [bp-0x8],ax
00000304  C746FA0000        mov word [bp-0x6],0x0
00000309  837EFAFF          cmp word [bp-0x6],byte -0x1
0000030D  7506              jnz 0x315
0000030F  837EF8FF          cmp word [bp-0x8],byte -0x1
00000313  74A6              jz 0x2bb
00000315  8B5EFC            mov bx,[bp-0x4]
00000318  D1E3              shl bx,1
0000031A  8B46F8            mov ax,[bp-0x8]
0000031D  8987F444          mov [bx+0x44f4],ax
00000321  8346FC01          add word [bp-0x4],byte +0x1
00000325  8356FE00          adc word [bp-0x2],byte +0x0
00000329  837EFE00          cmp word [bp-0x2],byte +0x0
0000032D  7D03              jnl 0x332
0000032F  E93CFF            jmp 0x26e
00000332  7F09              jg 0x33d
00000334  837EFC0B          cmp word [bp-0x4],byte +0xb
00000338  7703              ja 0x33d
0000033A  E931FF            jmp 0x26e
0000033D  C606AA3700        mov byte [0x37aa],0x0
00000342  EB12              jmp short 0x356
00000344  A0AA37            mov al,[0x37aa]
00000347  98                cbw
00000348  D1E0              shl ax,1
0000034A  8BD8              mov bx,ax
0000034C  C7870E45FFFF      mov word [bx+0x450e],0xffff
00000352  FE06AA37          inc byte [0x37aa]
00000356  803EAA3704        cmp byte [0x37aa],0x4
0000035B  7CE7              jl 0x344
0000035D  8BE5              mov sp,bp
0000035F  5D                pop bp
00000360  CB                retf

; ---- 0E31:0361 ----
00000361  55                push bp
00000362  8BEC              mov bp,sp
00000364  803EF10800        cmp byte [0x8f1],0x0
00000369  7507              jnz 0x372
0000036B  9A7903310E        call 0xe31:0x379
00000370  EB05              jmp short 0x377
00000372  9A6504310E        call 0xe31:0x465
00000377  5D                pop bp
00000378  CB                retf

; ---- 0E31:0379 ----
00000379  55                push bp
0000037A  8BEC              mov bp,sp
0000037C  83EC06            sub sp,byte +0x6
0000037F  A1FC08            mov ax,[0x8fc]
00000382  F72EFE08          imul word [0x8fe]
00000386  BB0400            mov bx,0x4
00000389  99                cwd
0000038A  F7FB              idiv bx
0000038C  FEC8              dec al
0000038E  8846FC            mov [bp-0x4],al
00000391  8846FE            mov [bp-0x2],al
00000394  8846FD            mov [bp-0x3],al
00000397  C606002F00        mov byte [0x2f00],0x0
0000039C  EB32              jmp short 0x3d0
0000039E  C606B02E00        mov byte [0x2eb0],0x0
000003A3  EB1A              jmp short 0x3bf
000003A5  A0B02E            mov al,[0x2eb0]
000003A8  98                cbw
000003A9  BA0600            mov dx,0x6
000003AC  F7EA              imul dx
000003AE  50                push ax
000003AF  A0002F            mov al,[0x2f00]
000003B2  98                cbw
000003B3  5B                pop bx
000003B4  03D8              add bx,ax
000003B6  C687DA4503        mov byte [bx+0x45da],0x3
000003BB  FE06B02E          inc byte [0x2eb0]
000003BF  A0B02E            mov al,[0x2eb0]
000003C2  98                cbw
000003C3  8B16FC08          mov dx,[0x8fc]
000003C7  4A                dec dx
000003C8  3BC2              cmp ax,dx
000003CA  7ED9              jng 0x3a5
000003CC  FE06002F          inc byte [0x2f00]
000003D0  A0002F            mov al,[0x2f00]
000003D3  98                cbw
000003D4  8B16FE08          mov dx,[0x8fe]
000003D8  4A                dec dx
000003D9  3BC2              cmp ax,dx
000003DB  7EC1              jng 0x39e
000003DD  C646FB00          mov byte [bp-0x5],0x0
000003E1  EB78              jmp short 0x45b
000003E3  EB61              jmp short 0x446
000003E5  A1FC08            mov ax,[0x8fc]
000003E8  48                dec ax
000003E9  50                push ax
000003EA  9ABA01FD21        call 0x21fd:0x1ba
000003EF  59                pop cx
000003F0  A2B02E            mov [0x2eb0],al
000003F3  A1FE08            mov ax,[0x8fe]
000003F6  48                dec ax
000003F7  50                push ax
000003F8  9ABA01FD21        call 0x21fd:0x1ba
000003FD  59                pop cx
000003FE  A2002F            mov [0x2f00],al
00000401  A0B02E            mov al,[0x2eb0]
00000404  98                cbw
00000405  BA0600            mov dx,0x6
00000408  F7EA              imul dx
0000040A  50                push ax
0000040B  A0002F            mov al,[0x2f00]
0000040E  98                cbw
0000040F  5B                pop bx
00000410  03D8              add bx,ax
00000412  80BFDA4503        cmp byte [bx+0x45da],0x3
00000417  7402              jz 0x41b
00000419  EB1A              jmp short 0x435
0000041B  A0B02E            mov al,[0x2eb0]
0000041E  98                cbw
0000041F  BA0600            mov dx,0x6
00000422  F7EA              imul dx
00000424  50                push ax
00000425  A0002F            mov al,[0x2f00]
00000428  98                cbw
00000429  5B                pop bx
0000042A  03D8              add bx,ax
0000042C  8A46FB            mov al,[bp-0x5]
0000042F  8887DA45          mov [bx+0x45da],al
00000433  EB02              jmp short 0x437
00000435  EBAE              jmp short 0x3e5
00000437  8A46FB            mov al,[bp-0x5]
0000043A  B400              mov ah,0x0
0000043C  8D56FC            lea dx,[bp-0x4]
0000043F  03C2              add ax,dx
00000441  8BD8              mov bx,ax
00000443  36FE0F            dec byte [ss:bx]
00000446  8A46FB            mov al,[bp-0x5]
00000449  B400              mov ah,0x0
0000044B  8D56FC            lea dx,[bp-0x4]
0000044E  03C2              add ax,dx
00000450  8BD8              mov bx,ax
00000452  36803F00          cmp byte [ss:bx],0x0
00000456  7F8D              jg 0x3e5
00000458  FE46FB            inc byte [bp-0x5]
0000045B  807EFB02          cmp byte [bp-0x5],0x2
0000045F  7682              jna 0x3e3
00000461  8BE5              mov sp,bp
00000463  5D                pop bp
00000464  CB                retf

; ---- 0E31:0465 ----
00000465  55                push bp
00000466  8BEC              mov bp,sp
00000468  83EC12            sub sp,byte +0x12
0000046B  56                push si
0000046C  A1FC08            mov ax,[0x8fc]
0000046F  3B06FE08          cmp ax,[0x8fe]
00000473  7E07              jng 0x47c
00000475  A0FC08            mov al,[0x8fc]
00000478  04FF              add al,0xff
0000047A  EB05              jmp short 0x481
0000047C  A0FE08            mov al,[0x8fe]
0000047F  04FF              add al,0xff
00000481  8846EE            mov [bp-0x12],al
00000484  A1FC08            mov ax,[0x8fc]
00000487  F72EFE08          imul word [0x8fe]
0000048B  BB0400            mov bx,0x4
0000048E  99                cwd
0000048F  F7FB              idiv bx
00000491  FEC8              dec al
00000493  8846FC            mov [bp-0x4],al
00000496  8846FE            mov [bp-0x2],al
00000499  8846FD            mov [bp-0x3],al
0000049C  8A46FD            mov al,[bp-0x3]
0000049F  98                cbw
000004A0  50                push ax
000004A1  8A46FE            mov al,[bp-0x2]
000004A4  98                cbw
000004A5  5A                pop dx
000004A6  03D0              add dx,ax
000004A8  8A46FC            mov al,[bp-0x4]
000004AB  98                cbw
000004AC  03D0              add dx,ax
000004AE  8BF2              mov si,dx
000004B0  C606002F00        mov byte [0x2f00],0x0
000004B5  EB32              jmp short 0x4e9
000004B7  C606B02E00        mov byte [0x2eb0],0x0
000004BC  EB1A              jmp short 0x4d8
000004BE  A0B02E            mov al,[0x2eb0]
000004C1  98                cbw
000004C2  BA0600            mov dx,0x6
000004C5  F7EA              imul dx
000004C7  50                push ax
000004C8  A0002F            mov al,[0x2f00]
000004CB  98                cbw
000004CC  5B                pop bx
000004CD  03D8              add bx,ax
000004CF  C687DA4503        mov byte [bx+0x45da],0x3
000004D4  FE06B02E          inc byte [0x2eb0]
000004D8  A0B02E            mov al,[0x2eb0]
000004DB  98                cbw
000004DC  8B16FC08          mov dx,[0x8fc]
000004E0  4A                dec dx
000004E1  3BC2              cmp ax,dx
000004E3  7ED9              jng 0x4be
000004E5  FE06002F          inc byte [0x2f00]
000004E9  A0002F            mov al,[0x2f00]
000004EC  98                cbw
000004ED  8B16FE08          mov dx,[0x8fe]
000004F1  4A                dec dx
000004F2  3BC2              cmp ax,dx
000004F4  7EC1              jng 0x4b7
000004F6  C646F000          mov byte [bp-0x10],0x0
000004FA  E98500            jmp 0x582
000004FD  A1FC08            mov ax,[0x8fc]
00000500  48                dec ax
00000501  50                push ax
00000502  9ABA01FD21        call 0x21fd:0x1ba
00000507  59                pop cx
00000508  A2B02E            mov [0x2eb0],al
0000050B  A1FE08            mov ax,[0x8fe]
0000050E  48                dec ax
0000050F  50                push ax
00000510  9ABA01FD21        call 0x21fd:0x1ba
00000515  59                pop cx
00000516  A2002F            mov [0x2f00],al
00000519  A0B02E            mov al,[0x2eb0]
0000051C  98                cbw
0000051D  BA0600            mov dx,0x6
00000520  F7EA              imul dx
00000522  50                push ax
00000523  A0002F            mov al,[0x2f00]
00000526  98                cbw
00000527  5B                pop bx
00000528  03D8              add bx,ax
0000052A  80BFDA4503        cmp byte [bx+0x45da],0x3
0000052F  75CC              jnz 0x4fd
00000531  8A46F0            mov al,[bp-0x10]
00000534  B400              mov ah,0x0
00000536  8D56F6            lea dx,[bp-0xa]
00000539  03C2              add ax,dx
0000053B  8A16B02E          mov dl,[0x2eb0]
0000053F  8BD8              mov bx,ax
00000541  368817            mov [ss:bx],dl
00000544  8A46F0            mov al,[bp-0x10]
00000547  B400              mov ah,0x0
00000549  8D56F2            lea dx,[bp-0xe]
0000054C  03C2              add ax,dx
0000054E  8A16002F          mov dl,[0x2f00]
00000552  8BD8              mov bx,ax
00000554  368817            mov [ss:bx],dl
00000557  A0B02E            mov al,[0x2eb0]
0000055A  98                cbw
0000055B  BA0600            mov dx,0x6
0000055E  F7EA              imul dx
00000560  50                push ax
00000561  A0002F            mov al,[0x2f00]
00000564  98                cbw
00000565  5B                pop bx
00000566  03D8              add bx,ax
00000568  8A46F0            mov al,[bp-0x10]
0000056B  8887DA45          mov [bx+0x45da],al
0000056F  8A46F0            mov al,[bp-0x10]
00000572  B400              mov ah,0x0
00000574  8D56FC            lea dx,[bp-0x4]
00000577  03C2              add ax,dx
00000579  8BD8              mov bx,ax
0000057B  36FE0F            dec byte [ss:bx]
0000057E  4E                dec si
0000057F  FE46F0            inc byte [bp-0x10]
00000582  807EF002          cmp byte [bp-0x10],0x2
00000586  7703              ja 0x58b
00000588  E972FF            jmp 0x4fd
0000058B  C646F101          mov byte [bp-0xf],0x1
0000058F  E94001            jmp 0x6d2
00000592  C646EF00          mov byte [bp-0x11],0x0
00000596  E91F01            jmp 0x6b8
00000599  C646F000          mov byte [bp-0x10],0x0
0000059D  E90C01            jmp 0x6ac
000005A0  8A46F0            mov al,[bp-0x10]
000005A3  B400              mov ah,0x0
000005A5  8D56FC            lea dx,[bp-0x4]
000005A8  03C2              add ax,dx
000005AA  8BD8              mov bx,ax
000005AC  36803F00          cmp byte [ss:bx],0x0
000005B0  7F03              jg 0x5b5
000005B2  E9F400            jmp 0x6a9
000005B5  B80100            mov ax,0x1
000005B8  50                push ax
000005B9  9ABA01FD21        call 0x21fd:0x1ba
000005BE  59                pop cx
000005BF  0BC0              or ax,ax
000005C1  7516              jnz 0x5d9
000005C3  8A46F1            mov al,[bp-0xf]
000005C6  8846FB            mov [bp-0x5],al
000005C9  8A46F1            mov al,[bp-0xf]
000005CC  98                cbw
000005CD  50                push ax
000005CE  9ABA01FD21        call 0x21fd:0x1ba
000005D3  59                pop cx
000005D4  8846FA            mov [bp-0x6],al
000005D7  EB14              jmp short 0x5ed
000005D9  8A46F1            mov al,[bp-0xf]
000005DC  98                cbw
000005DD  50                push ax
000005DE  9ABA01FD21        call 0x21fd:0x1ba
000005E3  59                pop cx
000005E4  8846FB            mov [bp-0x5],al
000005E7  8A46F1            mov al,[bp-0xf]
000005EA  8846FA            mov [bp-0x6],al
000005ED  B80100            mov ax,0x1
000005F0  50                push ax
000005F1  9ABA01FD21        call 0x21fd:0x1ba
000005F6  59                pop cx
000005F7  0BC0              or ax,ax
000005F9  7508              jnz 0x603
000005FB  8A46FB            mov al,[bp-0x5]
000005FE  F6D8              neg al
00000600  8846FB            mov [bp-0x5],al
00000603  B80100            mov ax,0x1
00000606  50                push ax
00000607  9ABA01FD21        call 0x21fd:0x1ba
0000060C  59                pop cx
0000060D  0BC0              or ax,ax
0000060F  7508              jnz 0x619
00000611  8A46FA            mov al,[bp-0x6]
00000614  F6D8              neg al
00000616  8846FA            mov [bp-0x6],al
00000619  8A46F0            mov al,[bp-0x10]
0000061C  B400              mov ah,0x0
0000061E  8D56F6            lea dx,[bp-0xa]
00000621  03C2              add ax,dx
00000623  8BD8              mov bx,ax
00000625  368A07            mov al,[ss:bx]
00000628  0246FB            add al,[bp-0x5]
0000062B  A2B02E            mov [0x2eb0],al
0000062E  8A46F0            mov al,[bp-0x10]
00000631  B400              mov ah,0x0
00000633  8D56F2            lea dx,[bp-0xe]
00000636  03C2              add ax,dx
00000638  8BD8              mov bx,ax
0000063A  368A07            mov al,[ss:bx]
0000063D  0246FA            add al,[bp-0x6]
00000640  A2002F            mov [0x2f00],al
00000643  803EB02E00        cmp byte [0x2eb0],0x0
00000648  7C1B              jl 0x665
0000064A  A0B02E            mov al,[0x2eb0]
0000064D  98                cbw
0000064E  3B06FC08          cmp ax,[0x8fc]
00000652  7D11              jnl 0x665
00000654  803E002F00        cmp byte [0x2f00],0x0
00000659  7C0A              jl 0x665
0000065B  A0002F            mov al,[0x2f00]
0000065E  98                cbw
0000065F  3B06FE08          cmp ax,[0x8fe]
00000663  7C02              jl 0x667
00000665  EB42              jmp short 0x6a9
00000667  A0B02E            mov al,[0x2eb0]
0000066A  98                cbw
0000066B  BA0600            mov dx,0x6
0000066E  F7EA              imul dx
00000670  50                push ax
00000671  A0002F            mov al,[0x2f00]
00000674  98                cbw
00000675  5B                pop bx
00000676  03D8              add bx,ax
00000678  80BFDA4503        cmp byte [bx+0x45da],0x3
0000067D  7402              jz 0x681
0000067F  EB28              jmp short 0x6a9
00000681  A0B02E            mov al,[0x2eb0]
00000684  98                cbw
00000685  BA0600            mov dx,0x6
00000688  F7EA              imul dx
0000068A  50                push ax
0000068B  A0002F            mov al,[0x2f00]
0000068E  98                cbw
0000068F  5B                pop bx
00000690  03D8              add bx,ax
00000692  8A46F0            mov al,[bp-0x10]
00000695  8887DA45          mov [bx+0x45da],al
00000699  8A46F0            mov al,[bp-0x10]
0000069C  B400              mov ah,0x0
0000069E  8D56FC            lea dx,[bp-0x4]
000006A1  03C2              add ax,dx
000006A3  8BD8              mov bx,ax
000006A5  36FE0F            dec byte [ss:bx]
000006A8  4E                dec si
000006A9  FE46F0            inc byte [bp-0x10]
000006AC  807EF002          cmp byte [bp-0x10],0x2
000006B0  7703              ja 0x6b5
000006B2  E9EBFE            jmp 0x5a0
000006B5  FE46EF            inc byte [bp-0x11]
000006B8  807EEF05          cmp byte [bp-0x11],0x5
000006BC  7703              ja 0x6c1
000006BE  E9D8FE            jmp 0x599
000006C1  8A46F1            mov al,[bp-0xf]
000006C4  FEC0              inc al
000006C6  8846F1            mov [bp-0xf],al
000006C9  3A46EE            cmp al,[bp-0x12]
000006CC  7604              jna 0x6d2
000006CE  C6                db 0xc6
000006CF  46                inc si
