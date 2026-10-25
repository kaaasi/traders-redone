; segment 135D  file offset 0x16BD0  size 0xC00
00000000  0BA0AA37          or sp,[bx+si+0x37aa]
00000004  98                cbw
00000005  8BD8              mov bx,ax
00000007  C687C33710        mov byte [bx+0x37c3],0x10
0000000C  5D                pop bp
0000000D  CB                retf

; ---- 135D:000E ----
0000000E  55                push bp
0000000F  8BEC              mov bp,sp
00000011  4C                dec sp
00000012  4C                dec sp
00000013  C746FEFFFF        mov word [bp-0x2],0xffff
00000018  A0AA37            mov al,[0x37aa]
0000001B  98                cbw
0000001C  D1E0              shl ax,1
0000001E  D1E0              shl ax,1
00000020  8BD8              mov bx,ax
00000022  83BF883700        cmp word [bx+0x3788],byte +0x0
00000027  7C0B              jl 0x34
00000029  7F07              jg 0x32
0000002B  83BF863700        cmp word [bx+0x3786],byte +0x0
00000030  7602              jna 0x34
00000032  EB6C              jmp short 0xa0
00000034  8B4606            mov ax,[bp+0x6]
00000037  A3F643            mov [0x43f6],ax
0000003A  8B4608            mov ax,[bp+0x8]
0000003D  A3F843            mov [0x43f8],ax
00000040  A0AA37            mov al,[0x37aa]
00000043  98                cbw
00000044  8BD8              mov bx,ax
00000046  8A87B243          mov al,[bx+0x43b2]
0000004A  B400              mov ah,0x0
0000004C  48                dec ax
0000004D  48                dec ax
0000004E  8BD8              mov bx,ax
00000050  83FB03            cmp bx,byte +0x3
00000053  773E              ja 0x93
00000055  D1E3              shl bx,1
00000057  2EFFA7A400        jmp [cs:bx+0xa4]
0000005C  9AAC005D13        call 0x135d:0xac
00000061  9AF5045D13        call 0x135d:0x4f5
00000066  8946FE            mov [bp-0x2],ax
00000069  EB28              jmp short 0x93
0000006B  9ABD025D13        call 0x135d:0x2bd
00000070  9AF5045D13        call 0x135d:0x4f5
00000075  8946FE            mov [bp-0x2],ax
00000078  EB19              jmp short 0x93
0000007A  9A590A5D13        call 0x135d:0xa59
0000007F  8946FE            mov [bp-0x2],ax
00000082  EB0F              jmp short 0x93
00000084  9A80045D13        call 0x135d:0x480
00000089  9AF5045D13        call 0x135d:0x4f5
0000008E  8946FE            mov [bp-0x2],ax
00000091  EB00              jmp short 0x93
00000093  A0AA37            mov al,[0x37aa]
00000096  98                cbw
00000097  8A56FE            mov dl,[bp-0x2]
0000009A  8BD8              mov bx,ax
0000009C  88970833          mov [bx+0x3308],dl
000000A0  8BE5              mov sp,bp
000000A2  5D                pop bp
000000A3  CB                retf
000000A4  5C                pop sp
000000A5  006B00            add [bp+di+0x0],ch
000000A8  7A00              jpe 0xaa
000000AA  8400              test [bx+si],al

; ---- 135D:00AC ----
000000AC  55                push bp
000000AD  8BEC              mov bp,sp
000000AF  83EC0A            sub sp,byte +0xa
000000B2  C646F864          mov byte [bp-0x8],0x64
000000B6  C646F69C          mov byte [bp-0xa],0x9c
000000BA  C646FF00          mov byte [bp-0x1],0x0
000000BE  EB12              jmp short 0xd2
000000C0  8A46FF            mov al,[bp-0x1]
000000C3  98                cbw
000000C4  8D56FA            lea dx,[bp-0x6]
000000C7  03C2              add ax,dx
000000C9  8BD8              mov bx,ax
000000CB  36C60700          mov byte [ss:bx],0x0
000000CF  FE46FF            inc byte [bp-0x1]
000000D2  807EFF03          cmp byte [bp-0x1],0x3
000000D6  7EE8              jng 0xc0
000000D8  C646FE00          mov byte [bp-0x2],0x0
000000DC  EB4B              jmp short 0x129
000000DE  8A46FE            mov al,[bp-0x2]
000000E1  3A06AA37          cmp al,[0x37aa]
000000E5  743F              jz 0x126
000000E7  8A46FE            mov al,[bp-0x2]
000000EA  98                cbw
000000EB  8BD8              mov bx,ax
000000ED  80BFFE40FF        cmp byte [bx+0x40fe],0xff
000000F2  7502              jnz 0xf6
000000F4  EB30              jmp short 0x126
000000F6  8A46FE            mov al,[bp-0x2]
000000F9  98                cbw
000000FA  8BD8              mov bx,ax
000000FC  8A870241          mov al,[bx+0x4102]
00000100  98                cbw
00000101  50                push ax
00000102  8A46FE            mov al,[bp-0x2]
00000105  98                cbw
00000106  8BD8              mov bx,ax
00000108  8A87FE40          mov al,[bx+0x40fe]
0000010C  98                cbw
0000010D  50                push ax
0000010E  9A40025D13        call 0x135d:0x240
00000113  59                pop cx
00000114  59                pop cx
00000115  8846FF            mov [bp-0x1],al
00000118  8A46FF            mov al,[bp-0x1]
0000011B  98                cbw
0000011C  8D56FA            lea dx,[bp-0x6]
0000011F  03C2              add ax,dx
00000121  8BD8              mov bx,ax
00000123  36FE07            inc byte [ss:bx]
00000126  FE46FE            inc byte [bp-0x2]
00000129  807EFE03          cmp byte [bp-0x2],0x3
0000012D  7EAF              jng 0xde
0000012F  C606002F00        mov byte [0x2f00],0x0
00000134  E9F500            jmp 0x22c
00000137  C606B02E00        mov byte [0x2eb0],0x0
0000013C  E9D900            jmp 0x218
0000013F  9A4D070906        call 0x609:0x74d
00000144  3C01              cmp al,0x1
00000146  7403              jz 0x14b
00000148  E9C900            jmp 0x214
0000014B  A1FC08            mov ax,[0x8fc]
0000014E  BB0200            mov bx,0x2
00000151  99                cwd
00000152  F7FB              idiv bx
00000154  50                push ax
00000155  A0B02E            mov al,[0x2eb0]
00000158  98                cbw
00000159  5A                pop dx
0000015A  3BD0              cmp dx,ax
0000015C  7F0B              jg 0x169
0000015E  A0FC08            mov al,[0x8fc]
00000161  FEC8              dec al
00000163  2A06B02E          sub al,[0x2eb0]
00000167  EB03              jmp short 0x16c
00000169  A0B02E            mov al,[0x2eb0]
0000016C  8846F7            mov [bp-0x9],al
0000016F  A1FE08            mov ax,[0x8fe]
00000172  BB0200            mov bx,0x2
00000175  99                cwd
00000176  F7FB              idiv bx
00000178  50                push ax
00000179  A0002F            mov al,[0x2f00]
0000017C  98                cbw
0000017D  5A                pop dx
0000017E  3BD0              cmp dx,ax
00000180  7F0B              jg 0x18d
00000182  A0FE08            mov al,[0x8fe]
00000185  FEC8              dec al
00000187  2A06002F          sub al,[0x2f00]
0000018B  EB03              jmp short 0x190
0000018D  A0002F            mov al,[0x2f00]
00000190  8A56F7            mov dl,[bp-0x9]
00000193  02D0              add dl,al
00000195  8856F7            mov [bp-0x9],dl
00000198  A0002F            mov al,[0x2f00]
0000019B  98                cbw
0000019C  50                push ax
0000019D  A0B02E            mov al,[0x2eb0]
000001A0  98                cbw
000001A1  50                push ax
000001A2  9A40025D13        call 0x135d:0x240
000001A7  59                pop cx
000001A8  59                pop cx
000001A9  8846FF            mov [bp-0x1],al
000001AC  8A46FF            mov al,[bp-0x1]
000001AF  98                cbw
000001B0  8D56FA            lea dx,[bp-0x6]
000001B3  03C2              add ax,dx
000001B5  8BD8              mov bx,ax
000001B7  368A07            mov al,[ss:bx]
000001BA  D0E0              shl al,1
000001BC  8A56F7            mov dl,[bp-0x9]
000001BF  2AD0              sub dl,al
000001C1  8856F7            mov [bp-0x9],dl
000001C4  8A46F6            mov al,[bp-0xa]
000001C7  3A46F7            cmp al,[bp-0x9]
000001CA  7E02              jng 0x1ce
000001CC  EB46              jmp short 0x214
000001CE  9A63025D13        call 0x135d:0x263
000001D3  8846F9            mov [bp-0x7],al
000001D6  8A46F6            mov al,[bp-0xa]
000001D9  3A46F7            cmp al,[bp-0x9]
000001DC  750A              jnz 0x1e8
000001DE  8A46F9            mov al,[bp-0x7]
000001E1  3A46F8            cmp al,[bp-0x8]
000001E4  7E02              jng 0x1e8
000001E6  EB2C              jmp short 0x214
000001E8  8A46F7            mov al,[bp-0x9]
000001EB  8846F6            mov [bp-0xa],al
000001EE  8A46F9            mov al,[bp-0x7]
000001F1  8846F8            mov [bp-0x8],al
000001F4  A0AA37            mov al,[0x37aa]
000001F7  98                cbw
000001F8  D1E0              shl ax,1
000001FA  50                push ax
000001FB  A0B02E            mov al,[0x2eb0]
000001FE  98                cbw
000001FF  5B                pop bx
00000200  8987CE40          mov [bx+0x40ce],ax
00000204  A0AA37            mov al,[0x37aa]
00000207  98                cbw
00000208  D1E0              shl ax,1
0000020A  50                push ax
0000020B  A0002F            mov al,[0x2f00]
0000020E  98                cbw
0000020F  5B                pop bx
00000210  8987D640          mov [bx+0x40d6],ax
00000214  FE06B02E          inc byte [0x2eb0]
00000218  A0B02E            mov al,[0x2eb0]
0000021B  98                cbw
0000021C  8B16FC08          mov dx,[0x8fc]
00000220  4A                dec dx
00000221  3BC2              cmp ax,dx
00000223  7F03              jg 0x228
00000225  E917FF            jmp 0x13f
00000228  FE06002F          inc byte [0x2f00]
0000022C  A0002F            mov al,[0x2f00]
0000022F  98                cbw
00000230  8B16FE08          mov dx,[0x8fe]
00000234  4A                dec dx
00000235  3BC2              cmp ax,dx
00000237  7F03              jg 0x23c
00000239  E9FBFE            jmp 0x137
0000023C  8BE5              mov sp,bp
0000023E  5D                pop bp
0000023F  CB                retf

; ---- 135D:0240 ----
00000240  55                push bp
00000241  8BEC              mov bp,sp
00000243  8B4608            mov ax,[bp+0x8]
00000246  D1E0              shl ax,1
00000248  99                cwd
00000249  F73EFE08          idiv word [0x8fe]
0000024D  D1E0              shl ax,1
0000024F  50                push ax
00000250  8B4606            mov ax,[bp+0x6]
00000253  D1E0              shl ax,1
00000255  99                cwd
00000256  F73EFC08          idiv word [0x8fc]
0000025A  8BD0              mov dx,ax
0000025C  58                pop ax
0000025D  03C2              add ax,dx
0000025F  EB00              jmp short 0x261
00000261  5D                pop bp
00000262  CB                retf

; ---- 135D:0263 ----
00000263  55                push bp
00000264  8BEC              mov bp,sp
00000266  FF36F843          push word [0x43f8]
0000026A  FF36F643          push word [0x43f6]
0000026E  A0002F            mov al,[0x2f00]
00000271  98                cbw
00000272  50                push ax
00000273  A0B02E            mov al,[0x2eb0]
00000276  98                cbw
00000277  50                push ax
00000278  9A84025D13        call 0x135d:0x284
0000027D  83C408            add sp,byte +0x8
00000280  EB00              jmp short 0x282
00000282  5D                pop bp
00000283  CB                retf

; ---- 135D:0284 ----
00000284  55                push bp
00000285  8BEC              mov bp,sp
00000287  56                push si
00000288  57                push di
00000289  8B4E06            mov cx,[bp+0x6]
0000028C  8B5E08            mov bx,[bp+0x8]
0000028F  8B7E0A            mov di,[bp+0xa]
00000292  8B760C            mov si,[bp+0xc]
00000295  3BCF              cmp cx,di
00000297  7E06              jng 0x29f
00000299  8BC1              mov ax,cx
0000029B  2BC7              sub ax,di
0000029D  EB04              jmp short 0x2a3
0000029F  8BC7              mov ax,di
000002A1  2BC1              sub ax,cx
000002A3  50                push ax
000002A4  3BDE              cmp bx,si
000002A6  7E06              jng 0x2ae
000002A8  8BC3              mov ax,bx
000002AA  2BC6              sub ax,si
000002AC  EB04              jmp short 0x2b2
000002AE  8BC6              mov ax,si
000002B0  2BC3              sub ax,bx
000002B2  8BD0              mov dx,ax
000002B4  58                pop ax
000002B5  03C2              add ax,dx
000002B7  EB00              jmp short 0x2b9
000002B9  5F                pop di
000002BA  5E                pop si
000002BB  5D                pop bp
000002BC  CB                retf

; ---- 135D:02BD ----
000002BD  55                push bp
000002BE  8BEC              mov bp,sp
000002C0  83EC04            sub sp,byte +0x4
000002C3  56                push si
000002C4  57                push di
000002C5  C646FD64          mov byte [bp-0x3],0x64
000002C9  A0AA37            mov al,[0x37aa]
000002CC  98                cbw
000002CD  D1E0              shl ax,1
000002CF  8BD8              mov bx,ax
000002D1  C787903C0000      mov word [bx+0x3c90],0x0
000002D7  A0AA37            mov al,[0x37aa]
000002DA  98                cbw
000002DB  8BD8              mov bx,ax
000002DD  80BFFA4301        cmp byte [bx+0x43fa],0x1
000002E2  7572              jnz 0x356
000002E4  9AAF055D13        call 0x135d:0x5af
000002E9  A0AA37            mov al,[0x37aa]
000002EC  98                cbw
000002ED  8BD8              mov bx,ax
000002EF  80BFBE4B00        cmp byte [bx+0x4bbe],0x0
000002F4  7560              jnz 0x356
000002F6  A0AA37            mov al,[0x37aa]
000002F9  98                cbw
000002FA  D1E0              shl ax,1
000002FC  8BD8              mov bx,ax
000002FE  8A870A41          mov al,[bx+0x410a]
00000302  A2B02E            mov [0x2eb0],al
00000305  A0AA37            mov al,[0x37aa]
00000308  98                cbw
00000309  D1E0              shl ax,1
0000030B  8BD8              mov bx,ax
0000030D  8A871241          mov al,[bx+0x4112]
00000311  A2002F            mov [0x2f00],al
00000314  0E                push cs
00000315  E84BFF            call 0x263
00000318  8946FE            mov [bp-0x2],ax
0000031B  803EBA4500        cmp byte [0x45ba],0x0
00000320  7411              jz 0x333
00000322  A0AA37            mov al,[0x37aa]
00000325  98                cbw
00000326  D1E0              shl ax,1
00000328  8BD8              mov bx,ax
0000032A  8B872045          mov ax,[bx+0x4520]
0000032E  3B46FE            cmp ax,[bp-0x2]
00000331  7C23              jl 0x356
00000333  A0AA37            mov al,[0x37aa]
00000336  98                cbw
00000337  D1E0              shl ax,1
00000339  50                push ax
0000033A  A0B02E            mov al,[0x2eb0]
0000033D  98                cbw
0000033E  5B                pop bx
0000033F  8987CE40          mov [bx+0x40ce],ax
00000343  A0AA37            mov al,[0x37aa]
00000346  98                cbw
00000347  D1E0              shl ax,1
00000349  50                push ax
0000034A  A0002F            mov al,[0x2f00]
0000034D  98                cbw
0000034E  5B                pop bx
0000034F  8987D640          mov [bx+0x40d6],ax
00000353  E92401            jmp 0x47a
00000356  33F6              xor si,si
00000358  E9D700            jmp 0x432
0000035B  A0AA37            mov al,[0x37aa]
0000035E  98                cbw
0000035F  BA3600            mov dx,0x36
00000362  F7EA              imul dx
00000364  8BD8              mov bx,ax
00000366  8A805446          mov al,[bx+si+0x4654]
0000036A  A2B02E            mov [0x2eb0],al
0000036D  3CFF              cmp al,0xff
0000036F  7503              jnz 0x374
00000371  E9BD00            jmp 0x431
00000374  A0AA37            mov al,[0x37aa]
00000377  98                cbw
00000378  BA3600            mov dx,0x36
0000037B  F7EA              imul dx
0000037D  8BD8              mov bx,ax
0000037F  8A802C47          mov al,[bx+si+0x472c]
00000383  A2002F            mov [0x2f00],al
00000386  0E                push cs
00000387  E8D9FE            call 0x263
0000038A  8946FE            mov [bp-0x2],ax
0000038D  50                push ax
0000038E  8A46FD            mov al,[bp-0x3]
00000391  98                cbw
00000392  5A                pop dx
00000393  3BD0              cmp dx,ax
00000395  7E03              jng 0x39a
00000397  E99700            jmp 0x431
0000039A  A0AA37            mov al,[0x37aa]
0000039D  98                cbw
0000039E  BA3600            mov dx,0x36
000003A1  F7EA              imul dx
000003A3  8BD8              mov bx,ax
000003A5  8A80312F          mov al,[bx+si+0x2f31]
000003A9  98                cbw
000003AA  8BF8              mov di,ax
000003AC  3D0300            cmp ax,0x3
000003AF  7423              jz 0x3d4
000003B1  A0AA37            mov al,[0x37aa]
000003B4  98                cbw
000003B5  D1E0              shl ax,1
000003B7  D1E0              shl ax,1
000003B9  D1E0              shl ax,1
000003BB  50                push ax
000003BC  8A85E708          mov al,[di+0x8e7]
000003C0  98                cbw
000003C1  D1E0              shl ax,1
000003C3  5B                pop bx
000003C4  03D8              add bx,ax
000003C6  A0A248            mov al,[0x48a2]
000003C9  98                cbw
000003CA  3987F43C          cmp [bx+0x3cf4],ax
000003CE  7D02              jnl 0x3d2
000003D0  EB5F              jmp short 0x431
000003D2  EB11              jmp short 0x3e5
000003D4  A0AA37            mov al,[0x37aa]
000003D7  98                cbw
000003D8  D1E0              shl ax,1
000003DA  8BD8              mov bx,ax
000003DC  83BFAC4B00        cmp word [bx+0x4bac],byte +0x0
000003E1  7502              jnz 0x3e5
000003E3  EB4C              jmp short 0x431
000003E5  803EBA4501        cmp byte [0x45ba],0x1
000003EA  7513              jnz 0x3ff
000003EC  A0AA37            mov al,[0x37aa]
000003EF  98                cbw
000003F0  D1E0              shl ax,1
000003F2  8BD8              mov bx,ax
000003F4  8B872045          mov ax,[bx+0x4520]
000003F8  3B46FE            cmp ax,[bp-0x2]
000003FB  7D02              jnl 0x3ff
000003FD  EB32              jmp short 0x431
000003FF  8A46FE            mov al,[bp-0x2]
00000402  8846FD            mov [bp-0x3],al
00000405  A0AA37            mov al,[0x37aa]
00000408  98                cbw
00000409  D1E0              shl ax,1
0000040B  50                push ax
0000040C  A0B02E            mov al,[0x2eb0]
0000040F  98                cbw
00000410  5B                pop bx
00000411  8987CE40          mov [bx+0x40ce],ax
00000415  A0AA37            mov al,[0x37aa]
00000418  98                cbw
00000419  D1E0              shl ax,1
0000041B  50                push ax
0000041C  A0002F            mov al,[0x2f00]
0000041F  98                cbw
00000420  5B                pop bx
00000421  8987D640          mov [bx+0x40d6],ax
00000425  A0AA37            mov al,[0x37aa]
00000428  98                cbw
00000429  D1E0              shl ax,1
0000042B  8BD8              mov bx,ax
0000042D  89B7903C          mov [bx+0x3c90],si
00000431  46                inc si
00000432  A0AA37            mov al,[0x37aa]
00000435  98                cbw
00000436  D1E0              shl ax,1
00000438  8BD8              mov bx,ax
0000043A  8B87EC48          mov ax,[bx+0x48ec]
0000043E  48                dec ax
0000043F  3BC6              cmp ax,si
00000441  7C03              jl 0x446
00000443  E915FF            jmp 0x35b
00000446  807EFD64          cmp byte [bp-0x3],0x64
0000044A  752E              jnz 0x47a
0000044C  A0AA37            mov al,[0x37aa]
0000044F  98                cbw
00000450  D1E0              shl ax,1
00000452  50                push ax
00000453  A0AA37            mov al,[0x37aa]
00000456  98                cbw
00000457  8BD8              mov bx,ax
00000459  8A87FE40          mov al,[bx+0x40fe]
0000045D  98                cbw
0000045E  5B                pop bx
0000045F  8987CE40          mov [bx+0x40ce],ax
00000463  A0AA37            mov al,[0x37aa]
00000466  98                cbw
00000467  D1E0              shl ax,1
00000469  50                push ax
0000046A  A0AA37            mov al,[0x37aa]
0000046D  98                cbw
0000046E  8BD8              mov bx,ax
00000470  8A870241          mov al,[bx+0x4102]
00000474  98                cbw
00000475  5B                pop bx
00000476  8987D640          mov [bx+0x40d6],ax
0000047A  5F                pop di
0000047B  5E                pop si
0000047C  8BE5              mov sp,bp
0000047E  5D                pop bp
0000047F  CB                retf

; ---- 135D:0480 ----
00000480  55                push bp
00000481  8BEC              mov bp,sp
00000483  803E052F00        cmp byte [0x2f05],0x0
00000488  7530              jnz 0x4ba
0000048A  A0AA37            mov al,[0x37aa]
0000048D  98                cbw
0000048E  D1E0              shl ax,1
00000490  50                push ax
00000491  A0AA37            mov al,[0x37aa]
00000494  98                cbw
00000495  8BD8              mov bx,ax
00000497  8A87FE40          mov al,[bx+0x40fe]
0000049B  98                cbw
0000049C  5B                pop bx
0000049D  8987CE40          mov [bx+0x40ce],ax
000004A1  A0AA37            mov al,[0x37aa]
000004A4  98                cbw
000004A5  D1E0              shl ax,1
000004A7  50                push ax
000004A8  A0AA37            mov al,[0x37aa]
000004AB  98                cbw
000004AC  8BD8              mov bx,ax
000004AE  8A870241          mov al,[bx+0x4102]
000004B2  98                cbw
000004B3  5B                pop bx
000004B4  8987D640          mov [bx+0x40d6],ax
000004B8  EB39              jmp short 0x4f3
000004BA  9AAF055D13        call 0x135d:0x5af
000004BF  A0AA37            mov al,[0x37aa]
000004C2  98                cbw
000004C3  D1E0              shl ax,1
000004C5  8BD8              mov bx,ax
000004C7  8B870A41          mov ax,[bx+0x410a]
000004CB  50                push ax
000004CC  A0AA37            mov al,[0x37aa]
000004CF  98                cbw
000004D0  D1E0              shl ax,1
000004D2  8BD8              mov bx,ax
000004D4  58                pop ax
000004D5  8987CE40          mov [bx+0x40ce],ax
000004D9  A0AA37            mov al,[0x37aa]
000004DC  98                cbw
000004DD  D1E0              shl ax,1
000004DF  8BD8              mov bx,ax
000004E1  8B871241          mov ax,[bx+0x4112]
000004E5  50                push ax
000004E6  A0AA37            mov al,[0x37aa]
000004E9  98                cbw
000004EA  D1E0              shl ax,1
000004EC  8BD8              mov bx,ax
000004EE  58                pop ax
000004EF  8987D640          mov [bx+0x40d6],ax
000004F3  5D                pop bp
000004F4  CB                retf

; ---- 135D:04F5 ----
000004F5  55                push bp
000004F6  8BEC              mov bp,sp
000004F8  A0AA37            mov al,[0x37aa]
000004FB  98                cbw
000004FC  D1E0              shl ax,1
000004FE  8BD8              mov bx,ax
00000500  8B87D640          mov ax,[bx+0x40d6]
00000504  3B06F843          cmp ax,[0x43f8]
00000508  7E1B              jng 0x525
0000050A  A1F643            mov ax,[0x43f6]
0000050D  BA0600            mov dx,0x6
00000510  F7EA              imul dx
00000512  0306F843          add ax,[0x43f8]
00000516  8BD8              mov bx,ax
00000518  80BFB22E04        cmp byte [bx+0x2eb2],0x4
0000051D  7406              jz 0x525
0000051F  B80300            mov ax,0x3
00000522  E98800            jmp 0x5ad
00000525  A0AA37            mov al,[0x37aa]
00000528  98                cbw
00000529  D1E0              shl ax,1
0000052B  8BD8              mov bx,ax
0000052D  8B87D640          mov ax,[bx+0x40d6]
00000531  3B06F843          cmp ax,[0x43f8]
00000535  7D1A              jnl 0x551
00000537  A1F643            mov ax,[0x43f6]
0000053A  BA0600            mov dx,0x6
0000053D  F7EA              imul dx
0000053F  0306F843          add ax,[0x43f8]
00000543  8BD8              mov bx,ax
00000545  80BFB12E04        cmp byte [bx+0x2eb1],0x4
0000054A  7405              jz 0x551
0000054C  B80200            mov ax,0x2
0000054F  EB5C              jmp short 0x5ad
00000551  A0AA37            mov al,[0x37aa]
00000554  98                cbw
00000555  D1E0              shl ax,1
00000557  8BD8              mov bx,ax
00000559  8B87CE40          mov ax,[bx+0x40ce]
0000055D  3B06F643          cmp ax,[0x43f6]
00000561  7E04              jng 0x567
00000563  33C0              xor ax,ax
00000565  EB46              jmp short 0x5ad
00000567  A0AA37            mov al,[0x37aa]
0000056A  98                cbw
0000056B  D1E0              shl ax,1
0000056D  8BD8              mov bx,ax
0000056F  8B87CE40          mov ax,[bx+0x40ce]
00000573  3B06F643          cmp ax,[0x43f6]
00000577  7D05              jnl 0x57e
00000579  B80100            mov ax,0x1
0000057C  EB2F              jmp short 0x5ad
0000057E  A0AA37            mov al,[0x37aa]
00000581  98                cbw
00000582  D1E0              shl ax,1
00000584  8BD8              mov bx,ax
00000586  8B87D640          mov ax,[bx+0x40d6]
0000058A  3B06F843          cmp ax,[0x43f8]
0000058E  7418              jz 0x5a8
00000590  A1FC08            mov ax,[0x8fc]
00000593  BB0200            mov bx,0x2
00000596  99                cwd
00000597  F7FB              idiv bx
00000599  3B06F643          cmp ax,[0x43f6]
0000059D  7F05              jg 0x5a4
0000059F  B80100            mov ax,0x1
000005A2  EB09              jmp short 0x5ad
000005A4  33C0              xor ax,ax
000005A6  EB05              jmp short 0x5ad
000005A8  B80A00            mov ax,0xa
000005AB  EB00              jmp short 0x5ad
000005AD  5D                pop bp
000005AE  CB                retf

; ---- 135D:05AF ----
000005AF  55                push bp
000005B0  8BEC              mov bp,sp
000005B2  83EC04            sub sp,byte +0x4
000005B5  56                push si
000005B6  C646FD64          mov byte [bp-0x3],0x64
000005BA  BE9CFF            mov si,0xff9c
000005BD  C606002F00        mov byte [0x2f00],0x0
000005C2  E9B000            jmp 0x675
000005C5  C606B02E00        mov byte [0x2eb0],0x0
000005CA  E99400            jmp 0x661
000005CD  A0B02E            mov al,[0x2eb0]
000005D0  98                cbw
000005D1  BA0600            mov dx,0x6
000005D4  F7EA              imul dx
000005D6  50                push ax
000005D7  A0002F            mov al,[0x2f00]
000005DA  98                cbw
000005DB  5B                pop bx
000005DC  03D8              add bx,ax
000005DE  80BF4244FF        cmp byte [bx+0x4442],0xff
000005E3  7578              jnz 0x65d
000005E5  A0AA37            mov al,[0x37aa]
000005E8  98                cbw
000005E9  8BD8              mov bx,ax
000005EB  80BFB24305        cmp byte [bx+0x43b2],0x5
000005F0  7417              jz 0x609
000005F2  A0002F            mov al,[0x2f00]
000005F5  98                cbw
000005F6  50                push ax
000005F7  A0B02E            mov al,[0x2eb0]
000005FA  98                cbw
000005FB  50                push ax
000005FC  9A0F100906        call 0x609:0x100f
00000601  59                pop cx
00000602  59                pop cx
00000603  0AC0              or al,al
00000605  7502              jnz 0x609
00000607  EB54              jmp short 0x65d
00000609  9AA1065D13        call 0x135d:0x6a1
0000060E  8846FF            mov [bp-0x1],al
00000611  98                cbw
00000612  3BC6              cmp ax,si
00000614  7D02              jnl 0x618
00000616  EB45              jmp short 0x65d
00000618  0E                push cs
00000619  E847FC            call 0x263
0000061C  8846FE            mov [bp-0x2],al
0000061F  8A46FF            mov al,[bp-0x1]
00000622  98                cbw
00000623  3BC6              cmp ax,si
00000625  750A              jnz 0x631
00000627  8A46FE            mov al,[bp-0x2]
0000062A  3A46FD            cmp al,[bp-0x3]
0000062D  7E02              jng 0x631
0000062F  EB2C              jmp short 0x65d
00000631  A0AA37            mov al,[0x37aa]
00000634  98                cbw
00000635  D1E0              shl ax,1
00000637  50                push ax
00000638  A0B02E            mov al,[0x2eb0]
0000063B  98                cbw
0000063C  5B                pop bx
0000063D  89870A41          mov [bx+0x410a],ax
00000641  A0AA37            mov al,[0x37aa]
00000644  98                cbw
00000645  D1E0              shl ax,1
00000647  50                push ax
00000648  A0002F            mov al,[0x2f00]
0000064B  98                cbw
0000064C  5B                pop bx
0000064D  89871241          mov [bx+0x4112],ax
00000651  8A46FF            mov al,[bp-0x1]
00000654  98                cbw
00000655  8BF0              mov si,ax
00000657  8A46FE            mov al,[bp-0x2]
0000065A  8846FD            mov [bp-0x3],al
0000065D  FE06B02E          inc byte [0x2eb0]
00000661  A0B02E            mov al,[0x2eb0]
00000664  98                cbw
00000665  8B16FC08          mov dx,[0x8fc]
00000669  4A                dec dx
0000066A  3BC2              cmp ax,dx
0000066C  7F03              jg 0x671
0000066E  E95CFF            jmp 0x5cd
00000671  FE06002F          inc byte [0x2f00]
00000675  A0002F            mov al,[0x2f00]
00000678  98                cbw
00000679  8B16FE08          mov dx,[0x8fe]
0000067D  4A                dec dx
0000067E  3BC2              cmp ax,dx
00000680  7F03              jg 0x685
00000682  E940FF            jmp 0x5c5
00000685  83FE9C            cmp si,byte -0x64
00000688  7504              jnz 0x68e
0000068A  B001              mov al,0x1
0000068C  EB02              jmp short 0x690
0000068E  B000              mov al,0x0
00000690  50                push ax
00000691  A0AA37            mov al,[0x37aa]
00000694  98                cbw
00000695  8BD8              mov bx,ax
00000697  58                pop ax
00000698  8887BE4B          mov [bx+0x4bbe],al
0000069C  5E                pop si
0000069D  8BE5              mov sp,bp
0000069F  5D                pop bp
000006A0  CB                retf

; ---- 135D:06A1 ----
000006A1  55                push bp
000006A2  8BEC              mov bp,sp
000006A4  83EC04            sub sp,byte +0x4
000006A7  A0AA37            mov al,[0x37aa]
000006AA  98                cbw
000006AB  8BD8              mov bx,ax
000006AD  80BFA84B03        cmp byte [bx+0x4ba8],0x3
000006B2  771E              ja 0x6d2
000006B4  A0B02E            mov al,[0x2eb0]
000006B7  98                cbw
000006B8  BA0600            mov dx,0x6
000006BB  F7EA              imul dx
000006BD  50                push ax
000006BE  A0002F            mov al,[0x2f00]
000006C1  98                cbw
000006C2  5B                pop bx
000006C3  03D8              add bx,ax
000006C5  80BFDA4503        cmp byte [bx+0x45da],0x3
000006CA  7506              jnz 0x6d2
000006CC  C646FF00          mov byte [bp-0x1],0x0
000006D0  EB37              jmp short 0x709
000006D2  A0B02E            mov al,[0x2eb0]
000006D5  98                cbw
000006D6  BA0600            mov dx,0x6
000006D9  F7EA              imul dx
000006DB  50                push ax
000006DC  A0002F            mov al,[0x2f00]
000006DF  98                cbw
000006E0  5B                pop bx
000006E1  03D8              add bx,ax
000006E3  8A87DA45          mov al,[bx+0x45da]
000006E7  98                cbw
000006E8  50                push ax
000006E9  A0AA37            mov al,[0x37aa]
000006EC  98                cbw
000006ED  D1E0              shl ax,1
000006EF  8BD8              mov bx,ax
000006F1  8B9FF640          mov bx,[bx+0x40f6]
000006F5  D1E3              shl bx,1
000006F7  D1E3              shl bx,1
000006F9  58                pop ax
000006FA  03D8              add bx,ax
000006FC  8A870209          mov al,[bx+0x902]
00000700  98                cbw
00000701  BA0300            mov dx,0x3
00000704  F7EA              imul dx
00000706  8846FF            mov [bp-0x1],al
00000709  A1FC08            mov ax,[0x8fc]
0000070C  BB0200            mov bx,0x2
0000070F  99                cwd
00000710  F7FB              idiv bx
00000712  50                push ax
00000713  A0B02E            mov al,[0x2eb0]
00000716  98                cbw
00000717  5A                pop dx
00000718  3BD0              cmp dx,ax
0000071A  7F0B              jg 0x727
0000071C  A0FC08            mov al,[0x8fc]
0000071F  FEC8              dec al
00000721  2A06B02E          sub al,[0x2eb0]
00000725  EB03              jmp short 0x72a
00000727  A0B02E            mov al,[0x2eb0]
0000072A  8846FE            mov [bp-0x2],al
0000072D  A1FE08            mov ax,[0x8fe]
00000730  BB0200            mov bx,0x2
00000733  99                cwd
00000734  F7FB              idiv bx
00000736  50                push ax
00000737  A0002F            mov al,[0x2f00]
0000073A  98                cbw
0000073B  5A                pop dx
0000073C  3BD0              cmp dx,ax
0000073E  7F0B              jg 0x74b
00000740  A0FE08            mov al,[0x8fe]
00000743  FEC8              dec al
00000745  2A06002F          sub al,[0x2f00]
00000749  EB03              jmp short 0x74e
0000074B  A0002F            mov al,[0x2f00]
0000074E  8A56FE            mov dl,[bp-0x2]
00000751  02D0              add dl,al
00000753  8856FE            mov [bp-0x2],dl
00000756  A0002F            mov al,[0x2f00]
00000759  98                cbw
0000075A  50                push ax
0000075B  A0B02E            mov al,[0x2eb0]
0000075E  98                cbw
0000075F  40                inc ax
00000760  50                push ax
00000761  9A51085D13        call 0x135d:0x851
00000766  59                pop cx
00000767  59                pop cx
00000768  8846FD            mov [bp-0x3],al
0000076B  A0002F            mov al,[0x2f00]
0000076E  98                cbw
0000076F  50                push ax
00000770  A0B02E            mov al,[0x2eb0]
00000773  98                cbw
00000774  48                dec ax
00000775  50                push ax
00000776  9A51085D13        call 0x135d:0x851
0000077B  59                pop cx
0000077C  59                pop cx
0000077D  8A56FD            mov dl,[bp-0x3]
00000780  02D0              add dl,al
00000782  8856FD            mov [bp-0x3],dl
00000785  A0002F            mov al,[0x2f00]
00000788  98                cbw
00000789  40                inc ax
0000078A  50                push ax
0000078B  A0B02E            mov al,[0x2eb0]
0000078E  98                cbw
0000078F  50                push ax
00000790  9A51085D13        call 0x135d:0x851
00000795  59                pop cx
00000796  59                pop cx
00000797  8A56FD            mov dl,[bp-0x3]
0000079A  02D0              add dl,al
0000079C  8856FD            mov [bp-0x3],dl
0000079F  A0002F            mov al,[0x2f00]
000007A2  98                cbw
000007A3  48                dec ax
000007A4  50                push ax
000007A5  A0B02E            mov al,[0x2eb0]
000007A8  98                cbw
000007A9  50                push ax
000007AA  9A51085D13        call 0x135d:0x851
000007AF  59                pop cx
000007B0  59                pop cx
000007B1  8A56FD            mov dl,[bp-0x3]
000007B4  02D0              add dl,al
000007B6  8856FD            mov [bp-0x3],dl
000007B9  A0002F            mov al,[0x2f00]
000007BC  98                cbw
000007BD  40                inc ax
000007BE  50                push ax
000007BF  A0B02E            mov al,[0x2eb0]
000007C2  98                cbw
000007C3  40                inc ax
000007C4  50                push ax
000007C5  9A51085D13        call 0x135d:0x851
000007CA  59                pop cx
000007CB  59                pop cx
000007CC  BB0200            mov bx,0x2
000007CF  99                cwd
000007D0  F7FB              idiv bx
000007D2  8A56FD            mov dl,[bp-0x3]
000007D5  02D0              add dl,al
000007D7  8856FD            mov [bp-0x3],dl
000007DA  A0002F            mov al,[0x2f00]
000007DD  98                cbw
000007DE  48                dec ax
000007DF  50                push ax
000007E0  A0B02E            mov al,[0x2eb0]
000007E3  98                cbw
000007E4  40                inc ax
000007E5  50                push ax
000007E6  9A51085D13        call 0x135d:0x851
000007EB  59                pop cx
000007EC  59                pop cx
000007ED  BB0200            mov bx,0x2
000007F0  99                cwd
000007F1  F7FB              idiv bx
000007F3  8A56FD            mov dl,[bp-0x3]
000007F6  02D0              add dl,al
000007F8  8856FD            mov [bp-0x3],dl
000007FB  A0002F            mov al,[0x2f00]
000007FE  98                cbw
000007FF  40                inc ax
00000800  50                push ax
00000801  A0B02E            mov al,[0x2eb0]
00000804  98                cbw
00000805  48                dec ax
00000806  50                push ax
00000807  9A51085D13        call 0x135d:0x851
0000080C  59                pop cx
0000080D  59                pop cx
0000080E  BB0200            mov bx,0x2
00000811  99                cwd
00000812  F7FB              idiv bx
00000814  8A56FD            mov dl,[bp-0x3]
00000817  02D0              add dl,al
00000819  8856FD            mov [bp-0x3],dl
0000081C  A0002F            mov al,[0x2f00]
0000081F  98                cbw
00000820  48                dec ax
00000821  50                push ax
00000822  A0B02E            mov al,[0x2eb0]
00000825  98                cbw
00000826  48                dec ax
00000827  50                push ax
00000828  9A51085D13        call 0x135d:0x851
0000082D  59                pop cx
0000082E  59                pop cx
0000082F  BB0200            mov bx,0x2
00000832  99                cwd
00000833  F7FB              idiv bx
00000835  8A56FD            mov dl,[bp-0x3]
00000838  02D0              add dl,al
0000083A  8856FD            mov [bp-0x3],dl
0000083D  8A46FF            mov al,[bp-0x1]
00000840  98                cbw
00000841  50                push ax
00000842  8A46FD            mov al,[bp-0x3]
00000845  98                cbw
00000846  8BD0              mov dx,ax
00000848  58                pop ax
00000849  03C2              add ax,dx
0000084B  EB00              jmp short 0x84d
0000084D  8BE5              mov sp,bp
0000084F  5D                pop bp
00000850  CB                retf

; ---- 135D:0851 ----
00000851  55                push bp
00000852  8BEC              mov bp,sp
00000854  56                push si
00000855  8B4606            mov ax,[bp+0x6]
00000858  BA0600            mov dx,0x6
0000085B  F7EA              imul dx
0000085D  034608            add ax,[bp+0x8]
00000860  8BD8              mov bx,ax
00000862  8A874244          mov al,[bx+0x4442]
00000866  98                cbw
00000867  8BF0              mov si,ax
00000869  FF7608            push word [bp+0x8]
0000086C  FF7606            push word [bp+0x6]
0000086F  9AA7100906        call 0x609:0x10a7
00000874  59                pop cx
00000875  59                pop cx
00000876  0AC0              or al,al
00000878  7504              jnz 0x87e
0000087A  33C0              xor ax,ax
0000087C  EB36              jmp short 0x8b4
0000087E  83FEFF            cmp si,byte -0x1
00000881  7408              jz 0x88b
00000883  A0AA37            mov al,[0x37aa]
00000886  98                cbw
00000887  3BC6              cmp ax,si
00000889  7504              jnz 0x88f
0000088B  33C0              xor ax,ax
0000088D  EB25              jmp short 0x8b4
0000088F  A0AA37            mov al,[0x37aa]
00000892  98                cbw
00000893  8BD8              mov bx,ax
00000895  80BFB24305        cmp byte [bx+0x43b2],0x5
0000089A  7505              jnz 0x8a1
0000089C  B8FFFF            mov ax,0xffff
0000089F  EB13              jmp short 0x8b4
000008A1  80BC400800        cmp byte [si+0x840],0x0
000008A6  7508              jnz 0x8b0
000008A8  A18E2E            mov ax,[0x2e8e]
000008AB  050300            add ax,0x3
000008AE  EB04              jmp short 0x8b4
000008B0  33C0              xor ax,ax
000008B2  EB00              jmp short 0x8b4
000008B4  5E                pop si
000008B5  5D                pop bp
000008B6  CB                retf
000008B7  55                push bp
000008B8  8BEC              mov bp,sp
000008BA  A0AA37            mov al,[0x37aa]
000008BD  98                cbw
000008BE  8BD8              mov bx,ax
000008C0  C687BE4B01        mov byte [bx+0x4bbe],0x1
000008C5  C606002F00        mov byte [0x2f00],0x0
000008CA  EB5A              jmp short 0x926
000008CC  C606B02E00        mov byte [0x2eb0],0x0
000008D1  EB42              jmp short 0x915
000008D3  A0B02E            mov al,[0x2eb0]
000008D6  98                cbw
000008D7  BA0600            mov dx,0x6
000008DA  F7EA              imul dx
000008DC  50                push ax
000008DD  A0002F            mov al,[0x2f00]
000008E0  98                cbw
000008E1  5B                pop bx
000008E2  03D8              add bx,ax
000008E4  80BF4244FF        cmp byte [bx+0x4442],0xff
000008E9  7402              jz 0x8ed
000008EB  EB24              jmp short 0x911
000008ED  A0002F            mov al,[0x2f00]
000008F0  98                cbw
000008F1  50                push ax
000008F2  A0B02E            mov al,[0x2eb0]
000008F5  98                cbw
000008F6  50                push ax
000008F7  9A0F100906        call 0x609:0x100f
000008FC  59                pop cx
000008FD  59                pop cx
000008FE  0AC0              or al,al
00000900  7502              jnz 0x904
00000902  EB0D              jmp short 0x911
00000904  A0AA37            mov al,[0x37aa]
00000907  98                cbw
00000908  8BD8              mov bx,ax
0000090A  C687BE4B00        mov byte [bx+0x4bbe],0x0
0000090F  EB11              jmp short 0x922
00000911  FE06B02E          inc byte [0x2eb0]
00000915  A0B02E            mov al,[0x2eb0]
00000918  98                cbw
00000919  8B16FC08          mov dx,[0x8fc]
0000091D  4A                dec dx
0000091E  3BC2              cmp ax,dx
00000920  7EB1              jng 0x8d3
00000922  FE06002F          inc byte [0x2f00]
00000926  A0002F            mov al,[0x2f00]
00000929  98                cbw
0000092A  8B16FE08          mov dx,[0x8fe]
0000092E  4A                dec dx
0000092F  3BC2              cmp ax,dx
00000931  7E99              jng 0x8cc
00000933  5D                pop bp
00000934  CB                retf

; ---- 135D:0935 ----
00000935  55                push bp
00000936  8BEC              mov bp,sp
00000938  4C                dec sp
00000939  4C                dec sp
0000093A  56                push si
0000093B  A0AA37            mov al,[0x37aa]
0000093E  98                cbw
0000093F  D1E0              shl ax,1
00000941  8BD8              mov bx,ax
00000943  8BB7EC48          mov si,[bx+0x48ec]
00000947  A0AA37            mov al,[0x37aa]
0000094A  98                cbw
0000094B  D1E0              shl ax,1
0000094D  8BD8              mov bx,ax
0000094F  8A87F640          mov al,[bx+0x40f6]
00000953  8846FF            mov [bp-0x1],al
00000956  8A46FF            mov al,[bp-0x1]
00000959  8AC8              mov cl,al
0000095B  A0AA37            mov al,[0x37aa]
0000095E  98                cbw
0000095F  D1E0              shl ax,1
00000961  8BD8              mov bx,ax
00000963  8A870A41          mov al,[bx+0x410a]
00000967  A2B02E            mov [0x2eb0],al
0000096A  A0AA37            mov al,[0x37aa]
0000096D  98                cbw
0000096E  D1E0              shl ax,1
00000970  8BD8              mov bx,ax
00000972  8A871241          mov al,[bx+0x4112]
00000976  A2002F            mov [0x2f00],al
00000979  A0AA37            mov al,[0x37aa]
0000097C  3A068045          cmp al,[0x4580]
00000980  741E              jz 0x9a0
00000982  A0AA37            mov al,[0x37aa]
00000985  98                cbw
00000986  8BD8              mov bx,ax
00000988  80BFC24B01        cmp byte [bx+0x4bc2],0x1
0000098D  7411              jz 0x9a0
0000098F  A0AA37            mov al,[0x37aa]
00000992  98                cbw
00000993  D1E0              shl ax,1
00000995  D1E0              shl ax,1
00000997  8BD8              mov bx,ax
00000999  80BFEA2E02        cmp byte [bx+0x2eea],0x2
0000099E  7D23              jnl 0x9c3
000009A0  B8AC4B            mov ax,0x4bac
000009A3  0BC0              or ax,ax
000009A5  761C              jna 0x9c3
000009A7  A0B02E            mov al,[0x2eb0]
000009AA  98                cbw
000009AB  BA0600            mov dx,0x6
000009AE  F7EA              imul dx
000009B0  50                push ax
000009B1  A0002F            mov al,[0x2f00]
000009B4  98                cbw
000009B5  5B                pop bx
000009B6  03D8              add bx,ax
000009B8  80BFDA4503        cmp byte [bx+0x45da],0x3
000009BD  7404              jz 0x9c3
000009BF  B103              mov cl,0x3
000009C1  EB33              jmp short 0x9f6
000009C3  A0B02E            mov al,[0x2eb0]
000009C6  98                cbw
000009C7  BA0600            mov dx,0x6
000009CA  F7EA              imul dx
000009CC  50                push ax
000009CD  A0002F            mov al,[0x2f00]
000009D0  98                cbw
000009D1  5B                pop bx
000009D2  03D8              add bx,ax
000009D4  8A87DA45          mov al,[bx+0x45da]
000009D8  98                cbw
000009D9  8A56FF            mov dl,[bp-0x1]
000009DC  B600              mov dh,0x0
000009DE  D1E2              shl dx,1
000009E0  D1E2              shl dx,1
000009E2  03D0              add dx,ax
000009E4  8BDA              mov bx,dx
000009E6  80BF020900        cmp byte [bx+0x902],0x0
000009EB  7504              jnz 0x9f1
000009ED  EB65              jmp short 0xa54
000009EF  EB05              jmp short 0x9f6
000009F1  8A46FF            mov al,[bp-0x1]
000009F4  8AC8              mov cl,al
000009F6  A0B02E            mov al,[0x2eb0]
000009F9  98                cbw
000009FA  BA0600            mov dx,0x6
000009FD  F7EA              imul dx
000009FF  50                push ax
00000A00  A0002F            mov al,[0x2f00]
00000A03  98                cbw
00000A04  5B                pop bx
00000A05  03D8              add bx,ax
00000A07  8A87B12E          mov al,[bx+0x2eb1]
00000A0B  3AC1              cmp al,cl
00000A0D  7502              jnz 0xa11
00000A0F  EB43              jmp short 0xa54
00000A11  A0AA37            mov al,[0x37aa]
00000A14  98                cbw
00000A15  BA3600            mov dx,0x36
00000A18  F7EA              imul dx
00000A1A  8AD1              mov dl,cl
00000A1C  8BD8              mov bx,ax
00000A1E  8890312F          mov [bx+si+0x2f31],dl
00000A22  A0AA37            mov al,[0x37aa]
00000A25  98                cbw
00000A26  BA3600            mov dx,0x36
00000A29  F7EA              imul dx
00000A2B  8A16B02E          mov dl,[0x2eb0]
00000A2F  8BD8              mov bx,ax
00000A31  88905446          mov [bx+si+0x4654],dl
00000A35  A0AA37            mov al,[0x37aa]
00000A38  98                cbw
00000A39  BA3600            mov dx,0x36
00000A3C  F7EA              imul dx
00000A3E  8A16002F          mov dl,[0x2f00]
00000A42  8BD8              mov bx,ax
00000A44  88902C47          mov [bx+si+0x472c],dl
00000A48  A0AA37            mov al,[0x37aa]
00000A4B  98                cbw
00000A4C  D1E0              shl ax,1
00000A4E  8BD8              mov bx,ax
00000A50  FF87EC48          inc word [bx+0x48ec]
00000A54  5E                pop si
00000A55  8BE5              mov sp,bp
00000A57  5D                pop bp
00000A58  CB                retf

; ---- 135D:0A59 ----
00000A59  55                push bp
00000A5A  8BEC              mov bp,sp
00000A5C  A0AA37            mov al,[0x37aa]
00000A5F  98                cbw
00000A60  BA3600            mov dx,0x36
00000A63  F7EA              imul dx
00000A65  50                push ax
00000A66  A0AA37            mov al,[0x37aa]
00000A69  98                cbw
00000A6A  D1E0              shl ax,1
00000A6C  8BD8              mov bx,ax
00000A6E  58                pop ax
00000A6F  0387903C          add ax,[bx+0x3c90]
00000A73  8BD8              mov bx,ax
00000A75  8A87312F          mov al,[bx+0x2f31]
00000A79  8AC8              mov cl,al
00000A7B  A0AA37            mov al,[0x37aa]
00000A7E  98                cbw
00000A7F  8BD8              mov bx,ax
00000A81  8A87942E          mov al,[bx+0x2e94]
00000A85  3AC1              cmp al,cl
00000A87  7D05              jnl 0xa8e
00000A89  B80300            mov ax,0x3
00000A8C  EB23              jmp short 0xab1
00000A8E  A0AA37            mov al,[0x37aa]
00000A91  98                cbw
00000A92  BA3600            mov dx,0x36
00000A95  F7EA              imul dx
00000A97  50                push ax
00000A98  A0AA37            mov al,[0x37aa]
00000A9B  98                cbw
00000A9C  D1E0              shl ax,1
00000A9E  8BD8              mov bx,ax
00000AA0  58                pop ax
00000AA1  0387903C          add ax,[bx+0x3c90]
00000AA5  8BD8              mov bx,ax
00000AA7  C6875446FF        mov byte [bx+0x4654],0xff
00000AAC  B80A00            mov ax,0xa
00000AAF  EB00              jmp short 0xab1
00000AB1  5D                pop bp
00000AB2  CB                retf

; ---- 135D:0AB3 ----
00000AB3  55                push bp
00000AB4  8BEC              mov bp,sp
00000AB6  83EC06            sub sp,byte +0x6
00000AB9  C706D0450000      mov word [0x45d0],0x0
00000ABF  EB11              jmp short 0xad2
00000AC1  8D46FC            lea ax,[bp-0x4]
00000AC4  8B1ED045          mov bx,[0x45d0]
00000AC8  03D8              add bx,ax
00000ACA  36C60700          mov byte [ss:bx],0x0
00000ACE  FF06D045          inc word [0x45d0]
00000AD2  833ED04503        cmp word [0x45d0],byte +0x3
00000AD7  7EE8              jng 0xac1
00000AD9  A0AA37            mov al,[0x37aa]
00000ADC  98                cbw
00000ADD  8BD8              mov bx,ax
00000ADF  8A870241          mov al,[bx+0x4102]
00000AE3  FEC8              dec al
00000AE5  A2002F            mov [0x2f00],al
00000AE8  E9A600            jmp 0xb91
00000AEB  A0AA37            mov al,[0x37aa]
00000AEE  98                cbw
00000AEF  8BD8              mov bx,ax
00000AF1  8A87FE40          mov al,[bx+0x40fe]
00000AF5  FEC8              dec al
00000AF7  A2B02E            mov [0x2eb0],al
00000AFA  EB78              jmp short 0xb74
00000AFC  A0002F            mov al,[0x2f00]
00000AFF  98                cbw
00000B00  50                push ax
00000B01  A0B02E            mov al,[0x2eb0]
00000B04  98                cbw
00000B05  50                push ax
00000B06  9AA7100906        call 0x609:0x10a7
00000B0B  59                pop cx
00000B0C  59                pop cx
00000B0D  0AC0              or al,al
00000B0F  7502              jnz 0xb13
00000B11  EB5D              jmp short 0xb70
00000B13  A0B02E            mov al,[0x2eb0]
00000B16  98                cbw
00000B17  BA0600            mov dx,0x6
00000B1A  F7EA              imul dx
00000B1C  50                push ax
00000B1D  A0002F            mov al,[0x2f00]
00000B20  98                cbw
00000B21  5B                pop bx
00000B22  03D8              add bx,ax
00000B24  80BF4244FF        cmp byte [bx+0x4442],0xff
00000B29  7402              jz 0xb2d
00000B2B  EB43              jmp short 0xb70
00000B2D  C706D0450000      mov word [0x45d0],0x0
00000B33  EB34              jmp short 0xb69
00000B35  A0B02E            mov al,[0x2eb0]
00000B38  98                cbw
00000B39  BA0600            mov dx,0x6
00000B3C  F7EA              imul dx
00000B3E  50                push ax
00000B3F  A0002F            mov al,[0x2f00]
00000B42  98                cbw
00000B43  5B                pop bx
00000B44  03D8              add bx,ax
00000B46  8A87DA45          mov al,[bx+0x45da]
00000B4A  98                cbw
00000B4B  8B1ED045          mov bx,[0x45d0]
00000B4F  D1E3              shl bx,1
00000B51  D1E3              shl bx,1
00000B53  03D8              add bx,ax
00000B55  8A870209          mov al,[bx+0x902]
00000B59  8D56FC            lea dx,[bp-0x4]
00000B5C  8B1ED045          mov bx,[0x45d0]
00000B60  03DA              add bx,dx
00000B62  360007            add [ss:bx],al
00000B65  FF06D045          inc word [0x45d0]
00000B69  833ED04502        cmp word [0x45d0],byte +0x2
00000B6E  7EC5              jng 0xb35
00000B70  FE06B02E          inc byte [0x2eb0]
00000B74  A0B02E            mov al,[0x2eb0]
00000B77  98                cbw
00000B78  50                push ax
00000B79  A0AA37            mov al,[0x37aa]
00000B7C  98                cbw
00000B7D  8BD8              mov bx,ax
00000B7F  8A87FE40          mov al,[bx+0x40fe]
00000B83  98                cbw
00000B84  40                inc ax
00000B85  5A                pop dx
00000B86  3BD0              cmp dx,ax
00000B88  7F03              jg 0xb8d
00000B8A  E96FFF            jmp 0xafc
00000B8D  FE06002F          inc byte [0x2f00]
00000B91  A0002F            mov al,[0x2f00]
00000B94  98                cbw
00000B95  50                push ax
00000B96  A0AA37            mov al,[0x37aa]
00000B99  98                cbw
00000B9A  8BD8              mov bx,ax
00000B9C  8A870241          mov al,[bx+0x4102]
00000BA0  98                cbw
00000BA1  40                inc ax
00000BA2  5A                pop dx
00000BA3  3BD0              cmp dx,ax
00000BA5  7F03              jg 0xbaa
00000BA7  E941FF            jmp 0xaeb
00000BAA  A0AA37            mov al,[0x37aa]
00000BAD  98                cbw
00000BAE  D1E0              shl ax,1
00000BB0  8BD8              mov bx,ax
00000BB2  8B9FF640          mov bx,[bx+0x40f6]
00000BB6  8D46FC            lea ax,[bp-0x4]
00000BB9  03D8              add bx,ax
00000BBB  368A07            mov al,[ss:bx]
00000BBE  8846FB            mov [bp-0x5],al
00000BC1  C706D0450000      mov word [0x45d0],0x0
00000BC7  EB34              jmp short 0xbfd
00000BC9  8D46FC            lea ax,[bp-0x4]
00000BCC  8B1ED045          mov bx,[0x45d0]
00000BD0  03D8              add bx,ax
00000BD2  368A07            mov al,[ss:bx]
00000BD5  3A46FB            cmp al,[bp-0x5]
00000BD8  7E1F              jng 0xbf9
00000BDA  8D46FC            lea ax,[bp-0x4]
00000BDD  8B1ED045          mov bx,[0x45d0]
00000BE1  03D8              add bx,ax
00000BE3  368A07            mov al,[ss:bx]
00000BE6  8846FB            mov [bp-0x5],al
00000BE9  A0AA37            mov al,[0x37aa]
00000BEC  98                cbw
00000BED  D1E0              shl ax,1
00000BEF  8B16D045          mov dx,[0x45d0]
00000BF3  8BD8              mov bx,ax
00000BF5  8997F640          mov [bx+0x40f6],dx
00000BF9  FF06D045          inc word [0x45d0]
00000BFD  83                db 0x83
00000BFE  3E                ds
00000BFF  D0                db 0xd0
