; segment 12C8  file offset 0x16280  size 0x950
00000000  8BD8              mov bx,ax
00000002  58                pop ax
00000003  89873045          mov [bx+0x4530],ax
00000007  5F                pop di
00000008  5E                pop si
00000009  5D                pop bp
0000000A  CB                retf

; ---- 12C8:000B ----
0000000B  55                push bp
0000000C  8BEC              mov bp,sp
0000000E  B200              mov dl,0x0
00000010  EB22              jmp short 0x34
00000012  80FA01            cmp dl,0x1
00000015  7E05              jng 0x1c
00000017  80FA06            cmp dl,0x6
0000001A  7C0C              jl 0x28
0000001C  8AC2              mov al,dl
0000001E  98                cbw
0000001F  8BD8              mov bx,ax
00000021  C687C644C3        mov byte [bx+0x44c6],0xc3
00000026  EB0A              jmp short 0x32
00000028  8AC2              mov al,dl
0000002A  98                cbw
0000002B  8BD8              mov bx,ax
0000002D  C687C64400        mov byte [bx+0x44c6],0x0
00000032  FEC2              inc dl
00000034  80FA07            cmp dl,0x7
00000037  7ED9              jng 0x12
00000039  5D                pop bp
0000003A  CB                retf

; ---- 12C8:003B ----
0000003B  55                push bp
0000003C  8BEC              mov bp,sp
0000003E  A0AA37            mov al,[0x37aa]
00000041  98                cbw
00000042  8BD8              mov bx,ax
00000044  C687504600        mov byte [bx+0x4650],0x0
00000049  9A5A00C812        call 0x12c8:0x5a
0000004E  9A0B03C812        call 0x12c8:0x30b
00000053  9AD606C812        call 0x12c8:0x6d6
00000058  5D                pop bp
00000059  CB                retf

; ---- 12C8:005A ----
0000005A  55                push bp
0000005B  8BEC              mov bp,sp
0000005D  83EC04            sub sp,byte +0x4
00000060  56                push si
00000061  57                push di
00000062  33F6              xor si,si
00000064  EB44              jmp short 0xaa
00000066  33FF              xor di,di
00000068  EB37              jmp short 0xa1
0000006A  16                push ss
0000006B  8D46FC            lea ax,[bp-0x4]
0000006E  50                push ax
0000006F  16                push ss
00000070  8D46FE            lea ax,[bp-0x2]
00000073  50                push ax
00000074  57                push di
00000075  56                push si
00000076  9AB500C812        call 0x12c8:0xb5
0000007B  83C40C            add sp,byte +0xc
0000007E  8B5EFE            mov bx,[bp-0x2]
00000081  D1E3              shl bx,1
00000083  D1E3              shl bx,1
00000085  D1E3              shl bx,1
00000087  035EFC            add bx,[bp-0x4]
0000008A  80BF784400        cmp byte [bx+0x4478],0x0
0000008F  7502              jnz 0x93
00000091  EB0D              jmp short 0xa0
00000093  FF76FC            push word [bp-0x4]
00000096  FF76FE            push word [bp-0x2]
00000099  9A5C01C812        call 0x12c8:0x15c
0000009E  59                pop cx
0000009F  59                pop cx
000000A0  47                inc di
000000A1  8BC6              mov ax,si
000000A3  D1E0              shl ax,1
000000A5  3BC7              cmp ax,di
000000A7  7DC1              jnl 0x6a
000000A9  46                inc si
000000AA  83FE03            cmp si,byte +0x3
000000AD  7EB7              jng 0x66
000000AF  5F                pop di
000000B0  5E                pop si
000000B1  8BE5              mov sp,bp
000000B3  5D                pop bp
000000B4  CB                retf

; ---- 12C8:00B5 ----
000000B5  55                push bp
000000B6  8BEC              mov bp,sp
000000B8  56                push si
000000B9  57                push di
000000BA  8B7606            mov si,[bp+0x6]
000000BD  8B7E08            mov di,[bp+0x8]
000000C0  3BFE              cmp di,si
000000C2  7F46              jg 0x10a
000000C4  A0AA37            mov al,[0x37aa]
000000C7  98                cbw
000000C8  8BD8              mov bx,ax
000000CA  8A87C807          mov al,[bx+0x7c8]
000000CE  98                cbw
000000CF  F7EE              imul si
000000D1  50                push ax
000000D2  A0AA37            mov al,[0x37aa]
000000D5  98                cbw
000000D6  8BD8              mov bx,ax
000000D8  8A87C007          mov al,[bx+0x7c0]
000000DC  98                cbw
000000DD  5A                pop dx
000000DE  03C2              add ax,dx
000000E0  C45E0A            les bx,[bp+0xa]
000000E3  268907            mov [es:bx],ax
000000E6  A0AA37            mov al,[0x37aa]
000000E9  98                cbw
000000EA  8BD8              mov bx,ax
000000EC  8A87CC07          mov al,[bx+0x7cc]
000000F0  98                cbw
000000F1  F7EF              imul di
000000F3  50                push ax
000000F4  A0AA37            mov al,[0x37aa]
000000F7  98                cbw
000000F8  8BD8              mov bx,ax
000000FA  8A87C407          mov al,[bx+0x7c4]
000000FE  98                cbw
000000FF  5A                pop dx
00000100  03C2              add ax,dx
00000102  C45E0E            les bx,[bp+0xe]
00000105  268907            mov [es:bx],ax
00000108  EB4E              jmp short 0x158
0000010A  8BC6              mov ax,si
0000010C  D1E0              shl ax,1
0000010E  2BC7              sub ax,di
00000110  50                push ax
00000111  A0AA37            mov al,[0x37aa]
00000114  98                cbw
00000115  8BD8              mov bx,ax
00000117  8A87C807          mov al,[bx+0x7c8]
0000011B  98                cbw
0000011C  8BD0              mov dx,ax
0000011E  58                pop ax
0000011F  F7EA              imul dx
00000121  50                push ax
00000122  A0AA37            mov al,[0x37aa]
00000125  98                cbw
00000126  8BD8              mov bx,ax
00000128  8A87C007          mov al,[bx+0x7c0]
0000012C  98                cbw
0000012D  5A                pop dx
0000012E  03C2              add ax,dx
00000130  C45E0A            les bx,[bp+0xa]
00000133  268907            mov [es:bx],ax
00000136  A0AA37            mov al,[0x37aa]
00000139  98                cbw
0000013A  8BD8              mov bx,ax
0000013C  8A87CC07          mov al,[bx+0x7cc]
00000140  98                cbw
00000141  F7EE              imul si
00000143  50                push ax
00000144  A0AA37            mov al,[0x37aa]
00000147  98                cbw
00000148  8BD8              mov bx,ax
0000014A  8A87C407          mov al,[bx+0x7c4]
0000014E  98                cbw
0000014F  5A                pop dx
00000150  03C2              add ax,dx
00000152  C45E0E            les bx,[bp+0xe]
00000155  268907            mov [es:bx],ax
00000158  5F                pop di
00000159  5E                pop si
0000015A  5D                pop bp
0000015B  CB                retf

; ---- 12C8:015C ----
0000015C  55                push bp
0000015D  8BEC              mov bp,sp
0000015F  83EC06            sub sp,byte +0x6
00000162  56                push si
00000163  57                push di
00000164  C646FB00          mov byte [bp-0x5],0x0
00000168  C646FA00          mov byte [bp-0x6],0x0
0000016C  33F6              xor si,si
0000016E  EB55              jmp short 0x1c5
00000170  8A847808          mov al,[si+0x878]
00000174  98                cbw
00000175  8B5606            mov dx,[bp+0x6]
00000178  03D0              add dx,ax
0000017A  8956FC            mov [bp-0x4],dx
0000017D  8A848008          mov al,[si+0x880]
00000181  98                cbw
00000182  8B5608            mov dx,[bp+0x8]
00000185  03D0              add dx,ax
00000187  8BFA              mov di,dx
00000189  57                push di
0000018A  FF76FC            push word [bp-0x4]
0000018D  9A4E02C812        call 0x12c8:0x24e
00000192  59                pop cx
00000193  59                pop cx
00000194  8946FE            mov [bp-0x2],ax
00000197  3DFFFF            cmp ax,0xffff
0000019A  7502              jnz 0x19e
0000019C  EB26              jmp short 0x1c4
0000019E  8B5EFC            mov bx,[bp-0x4]
000001A1  D1E3              shl bx,1
000001A3  D1E3              shl bx,1
000001A5  D1E3              shl bx,1
000001A7  80B9784400        cmp byte [bx+di+0x4478],0x0
000001AC  7502              jnz 0x1b0
000001AE  EB14              jmp short 0x1c4
000001B0  837EFE04          cmp word [bp-0x2],byte +0x4
000001B4  7504              jnz 0x1ba
000001B6  C646FB01          mov byte [bp-0x5],0x1
000001BA  837EFE02          cmp word [bp-0x2],byte +0x2
000001BE  7504              jnz 0x1c4
000001C0  C646FA01          mov byte [bp-0x6],0x1
000001C4  46                inc si
000001C5  83FE07            cmp si,byte +0x7
000001C8  7EA6              jng 0x170
000001CA  807EFB00          cmp byte [bp-0x5],0x0
000001CE  7406              jz 0x1d6
000001D0  807EFA00          cmp byte [bp-0x6],0x0
000001D4  7502              jnz 0x1d8
000001D6  EB70              jmp short 0x248
000001D8  B001              mov al,0x1
000001DA  50                push ax
000001DB  FF7608            push word [bp+0x8]
000001DE  FF7606            push word [bp+0x6]
000001E1  9ABB02C812        call 0x12c8:0x2bb
000001E6  83C406            add sp,byte +0x6
000001E9  33F6              xor si,si
000001EB  EB56              jmp short 0x243
000001ED  8A847808          mov al,[si+0x878]
000001F1  98                cbw
000001F2  8B5606            mov dx,[bp+0x6]
000001F5  03D0              add dx,ax
000001F7  8956FC            mov [bp-0x4],dx
000001FA  8A848008          mov al,[si+0x880]
000001FE  98                cbw
000001FF  8B5608            mov dx,[bp+0x8]
00000202  03D0              add dx,ax
00000204  8BFA              mov di,dx
00000206  57                push di
00000207  FF76FC            push word [bp-0x4]
0000020A  9A4E02C812        call 0x12c8:0x24e
0000020F  59                pop cx
00000210  59                pop cx
00000211  8946FE            mov [bp-0x2],ax
00000214  3DFFFF            cmp ax,0xffff
00000217  7502              jnz 0x21b
00000219  EB27              jmp short 0x242
0000021B  8B5EFC            mov bx,[bp-0x4]
0000021E  D1E3              shl bx,1
00000220  D1E3              shl bx,1
00000222  D1E3              shl bx,1
00000224  80B9784400        cmp byte [bx+di+0x4478],0x0
00000229  7502              jnz 0x22d
0000022B  EB15              jmp short 0x242
0000022D  837EFE02          cmp word [bp-0x2],byte +0x2
00000231  750F              jnz 0x242
00000233  B001              mov al,0x1
00000235  50                push ax
00000236  57                push di
00000237  FF76FC            push word [bp-0x4]
0000023A  9ABB02C812        call 0x12c8:0x2bb
0000023F  83C406            add sp,byte +0x6
00000242  46                inc si
00000243  83FE07            cmp si,byte +0x7
00000246  7EA5              jng 0x1ed
00000248  5F                pop di
00000249  5E                pop si
0000024A  8BE5              mov sp,bp
0000024C  5D                pop bp
0000024D  CB                retf

; ---- 12C8:024E ----
0000024E  55                push bp
0000024F  8BEC              mov bp,sp
00000251  56                push si
00000252  57                push di
00000253  8B4E08            mov cx,[bp+0x8]
00000256  837E0600          cmp word [bp+0x6],byte +0x0
0000025A  7C0F              jl 0x26b
0000025C  837E0607          cmp word [bp+0x6],byte +0x7
00000260  7F09              jg 0x26b
00000262  0BC9              or cx,cx
00000264  7C05              jl 0x26b
00000266  83F907            cmp cx,byte +0x7
00000269  7E05              jng 0x270
0000026B  B8FFFF            mov ax,0xffff
0000026E  EB47              jmp short 0x2b7
00000270  A0AA37            mov al,[0x37aa]
00000273  98                cbw
00000274  8BD8              mov bx,ax
00000276  8A87C007          mov al,[bx+0x7c0]
0000027A  98                cbw
0000027B  8B5606            mov dx,[bp+0x6]
0000027E  2BD0              sub dx,ax
00000280  8BFA              mov di,dx
00000282  8BC2              mov ax,dx
00000284  0BC0              or ax,ax
00000286  7D06              jnl 0x28e
00000288  8BC7              mov ax,di
0000028A  F7D8              neg ax
0000028C  8BF8              mov di,ax
0000028E  A0AA37            mov al,[0x37aa]
00000291  98                cbw
00000292  8BD8              mov bx,ax
00000294  8A87C407          mov al,[bx+0x7c4]
00000298  98                cbw
00000299  8BD1              mov dx,cx
0000029B  2BD0              sub dx,ax
0000029D  8BF2              mov si,dx
0000029F  8BC2              mov ax,dx
000002A1  0BC0              or ax,ax
000002A3  7D06              jnl 0x2ab
000002A5  8BC6              mov ax,si
000002A7  F7D8              neg ax
000002A9  8BF0              mov si,ax
000002AB  3BFE              cmp di,si
000002AD  7E04              jng 0x2b3
000002AF  8BC7              mov ax,di
000002B1  EB02              jmp short 0x2b5
000002B3  8BC6              mov ax,si
000002B5  EB00              jmp short 0x2b7
000002B7  5F                pop di
000002B8  5E                pop si
000002B9  5D                pop bp
000002BA  CB                retf

; ---- 12C8:02BB ----
000002BB  55                push bp
000002BC  8BEC              mov bp,sp
000002BE  56                push si
000002BF  57                push di
000002C0  8B7E06            mov di,[bp+0x6]
000002C3  8B7608            mov si,[bp+0x8]
000002C6  807E0A01          cmp byte [bp+0xa],0x1
000002CA  750A              jnz 0x2d6
000002CC  8A85C608          mov al,[di+0x8c6]
000002D0  0884C644          or [si+0x44c6],al
000002D4  EB10              jmp short 0x2e6
000002D6  8A84C644          mov al,[si+0x44c6]
000002DA  B2FF              mov dl,0xff
000002DC  2A95C608          sub dl,[di+0x8c6]
000002E0  22C2              and al,dl
000002E2  8884C644          mov [si+0x44c6],al
000002E6  5F                pop di
000002E7  5E                pop si
000002E8  5D                pop bp
000002E9  CB                retf

; ---- 12C8:02EA ----
000002EA  55                push bp
000002EB  8BEC              mov bp,sp
000002ED  56                push si
000002EE  8B5E08            mov bx,[bp+0x8]
000002F1  8B7606            mov si,[bp+0x6]
000002F4  8A84C608          mov al,[si+0x8c6]
000002F8  8487C644          test [bx+0x44c6],al
000002FC  7406              jz 0x304
000002FE  B001              mov al,0x1
00000300  EB06              jmp short 0x308
00000302  EB04              jmp short 0x308
00000304  B000              mov al,0x0
00000306  EB00              jmp short 0x308
00000308  5E                pop si
00000309  5D                pop bp
0000030A  CB                retf

; ---- 12C8:030B ----
0000030B  55                push bp
0000030C  8BEC              mov bp,sp
0000030E  83EC16            sub sp,byte +0x16
00000311  56                push si
00000312  57                push di
00000313  33F6              xor si,si
00000315  EB53              jmp short 0x36a
00000317  8BDE              mov bx,si
00000319  D1E3              shl bx,1
0000031B  8D46F2            lea ax,[bp-0xe]
0000031E  03D8              add bx,ax
00000320  36C7070000        mov word [ss:bx],0x0
00000325  C746FA0000        mov word [bp-0x6],0x0
0000032A  EB34              jmp short 0x360
0000032C  16                push ss
0000032D  8D46FC            lea ax,[bp-0x4]
00000330  50                push ax
00000331  16                push ss
00000332  8D46FE            lea ax,[bp-0x2]
00000335  50                push ax
00000336  FF76FA            push word [bp-0x6]
00000339  56                push si
0000033A  0E                push cs
0000033B  E877FD            call 0xb5
0000033E  83C40C            add sp,byte +0xc
00000341  FF76FC            push word [bp-0x4]
00000344  FF76FE            push word [bp-0x2]
00000347  0E                push cs
00000348  E89FFF            call 0x2ea
0000034B  59                pop cx
0000034C  59                pop cx
0000034D  3C01              cmp al,0x1
0000034F  750C              jnz 0x35d
00000351  8BDE              mov bx,si
00000353  D1E3              shl bx,1
00000355  8D46F2            lea ax,[bp-0xe]
00000358  03D8              add bx,ax
0000035A  36FF07            inc word [ss:bx]
0000035D  FF46FA            inc word [bp-0x6]
00000360  8BC6              mov ax,si
00000362  D1E0              shl ax,1
00000364  3B46FA            cmp ax,[bp-0x6]
00000367  7DC3              jnl 0x32c
00000369  46                inc si
0000036A  83FE03            cmp si,byte +0x3
0000036D  7EA8              jng 0x317
0000036F  A0AA37            mov al,[0x37aa]
00000372  98                cbw
00000373  D1E0              shl ax,1
00000375  8BD8              mov bx,ax
00000377  8B87F82E          mov ax,[bx+0x2ef8]
0000037B  BB1400            mov bx,0x14
0000037E  33D2              xor dx,dx
00000380  F7F3              div bx
00000382  8946F0            mov [bp-0x10],ax
00000385  A0AA37            mov al,[0x37aa]
00000388  98                cbw
00000389  D1E0              shl ax,1
0000038B  8BD8              mov bx,ax
0000038D  8B871630          mov ax,[bx+0x3016]
00000391  3B46F0            cmp ax,[bp-0x10]
00000394  730F              jnc 0x3a5
00000396  A0AA37            mov al,[0x37aa]
00000399  98                cbw
0000039A  D1E0              shl ax,1
0000039C  8BD8              mov bx,ax
0000039E  8B871630          mov ax,[bx+0x3016]
000003A2  8946F0            mov [bp-0x10],ax
000003A5  A0AA37            mov al,[0x37aa]
000003A8  98                cbw
000003A9  D1E0              shl ax,1
000003AB  8BD8              mov bx,ax
000003AD  8BBFEC40          mov di,[bx+0x40ec]
000003B1  8BC7              mov ax,di
000003B3  BB1000            mov bx,0x10
000003B6  99                cwd
000003B7  F7FB              idiv bx
000003B9  8946EE            mov [bp-0x12],ax
000003BC  8B46EE            mov ax,[bp-0x12]
000003BF  3B46F0            cmp ax,[bp-0x10]
000003C2  7E06              jng 0x3ca
000003C4  8B46F0            mov ax,[bp-0x10]
000003C7  8946EE            mov [bp-0x12],ax
000003CA  33F6              xor si,si
000003CC  EB79              jmp short 0x447
000003CE  0BF6              or si,si
000003D0  7E56              jng 0x428
000003D2  8BDE              mov bx,si
000003D4  D1E3              shl bx,1
000003D6  8D46F2            lea ax,[bp-0xe]
000003D9  03D8              add bx,ax
000003DB  368B07            mov ax,[ss:bx]
000003DE  3B46EE            cmp ax,[bp-0x12]
000003E1  7F45              jg 0x428
000003E3  A0AA37            mov al,[0x37aa]
000003E6  98                cbw
000003E7  D1E0              shl ax,1
000003E9  D1E0              shl ax,1
000003EB  8BD8              mov bx,ax
000003ED  C680CE4402        mov byte [bx+si+0x44ce],0x2
000003F2  A0AA37            mov al,[0x37aa]
000003F5  98                cbw
000003F6  D1E0              shl ax,1
000003F8  D1E0              shl ax,1
000003FA  8BD8              mov bx,ax
000003FC  C6804C3701        mov byte [bx+si+0x374c],0x1
00000401  8BDE              mov bx,si
00000403  D1E3              shl bx,1
00000405  8D46F2            lea ax,[bp-0xe]
00000408  03D8              add bx,ax
0000040A  368B07            mov ax,[ss:bx]
0000040D  2946EE            sub [bp-0x12],ax
00000410  8BDE              mov bx,si
00000412  D1E3              shl bx,1
00000414  8D46F2            lea ax,[bp-0xe]
00000417  03D8              add bx,ax
00000419  368B07            mov ax,[ss:bx]
0000041C  B104              mov cl,0x4
0000041E  D3E0              shl ax,cl
00000420  8BD7              mov dx,di
00000422  2BD0              sub dx,ax
00000424  8BFA              mov di,dx
00000426  EB1E              jmp short 0x446
00000428  A0AA37            mov al,[0x37aa]
0000042B  98                cbw
0000042C  D1E0              shl ax,1
0000042E  D1E0              shl ax,1
00000430  8BD8              mov bx,ax
00000432  C680CE4400        mov byte [bx+si+0x44ce],0x0
00000437  A0AA37            mov al,[0x37aa]
0000043A  98                cbw
0000043B  D1E0              shl ax,1
0000043D  D1E0              shl ax,1
0000043F  8BD8              mov bx,ax
00000441  C6804C3700        mov byte [bx+si+0x374c],0x0
00000446  46                inc si
00000447  83FE03            cmp si,byte +0x3
0000044A  7E82              jng 0x3ce
0000044C  BE0300            mov si,0x3
0000044F  EB64              jmp short 0x4b5
00000451  0BF6              or si,si
00000453  7507              jnz 0x45c
00000455  C746EC0400        mov word [bp-0x14],0x4
0000045A  EB28              jmp short 0x484
0000045C  A0AA37            mov al,[0x37aa]
0000045F  98                cbw
00000460  D1E0              shl ax,1
00000462  D1E0              shl ax,1
00000464  8BD8              mov bx,ax
00000466  80B84C3700        cmp byte [bx+si+0x374c],0x0
0000046B  7502              jnz 0x46f
0000046D  EB45              jmp short 0x4b4
0000046F  8BDE              mov bx,si
00000471  D1E3              shl bx,1
00000473  8D46F2            lea ax,[bp-0xe]
00000476  03D8              add bx,ax
00000478  368B07            mov ax,[ss:bx]
0000047B  D1E0              shl ax,1
0000047D  D1E0              shl ax,1
0000047F  D1E0              shl ax,1
00000481  8946EC            mov [bp-0x14],ax
00000484  EB29              jmp short 0x4af
00000486  2B7EEC            sub di,[bp-0x14]
00000489  A0AA37            mov al,[0x37aa]
0000048C  98                cbw
0000048D  D1E0              shl ax,1
0000048F  D1E0              shl ax,1
00000491  8BD8              mov bx,ax
00000493  8A80CE44          mov al,[bx+si+0x44ce]
00000497  FEC0              inc al
00000499  50                push ax
0000049A  A0AA37            mov al,[0x37aa]
0000049D  98                cbw
0000049E  D1E0              shl ax,1
000004A0  D1E0              shl ax,1
000004A2  8BD8              mov bx,ax
000004A4  58                pop ax
000004A5  8880CE44          mov [bx+si+0x44ce],al
000004A9  3C06              cmp al,0x6
000004AB  7C02              jl 0x4af
000004AD  EB05              jmp short 0x4b4
000004AF  3B7EEC            cmp di,[bp-0x14]
000004B2  7DD2              jnl 0x486
000004B4  4E                dec si
000004B5  0BF6              or si,si
000004B7  7D98              jnl 0x451
000004B9  A0AA37            mov al,[0x37aa]
000004BC  98                cbw
000004BD  D1E0              shl ax,1
000004BF  8BD8              mov bx,ax
000004C1  8B87DC33          mov ax,[bx+0x33dc]
000004C5  8946EA            mov [bp-0x16],ax
000004C8  BE0100            mov si,0x1
000004CB  EB61              jmp short 0x52e
000004CD  A0AA37            mov al,[0x37aa]
000004D0  98                cbw
000004D1  D1E0              shl ax,1
000004D3  D1E0              shl ax,1
000004D5  8BD8              mov bx,ax
000004D7  80B8CE4400        cmp byte [bx+si+0x44ce],0x0
000004DC  7516              jnz 0x4f4
000004DE  8BDE              mov bx,si
000004E0  D1E3              shl bx,1
000004E2  8D46F2            lea ax,[bp-0xe]
000004E5  03D8              add bx,ax
000004E7  368B07            mov ax,[ss:bx]
000004EA  BA0900            mov dx,0x9
000004ED  F7EA              imul dx
000004EF  3B46EA            cmp ax,[bp-0x16]
000004F2  7E11              jng 0x505
000004F4  A0AA37            mov al,[0x37aa]
000004F7  98                cbw
000004F8  D1E0              shl ax,1
000004FA  D1E0              shl ax,1
000004FC  8BD8              mov bx,ax
000004FE  C680982E00        mov byte [bx+si+0x2e98],0x0
00000503  EB28              jmp short 0x52d
00000505  A0AA37            mov al,[0x37aa]
00000508  98                cbw
00000509  D1E0              shl ax,1
0000050B  D1E0              shl ax,1
0000050D  8BD8              mov bx,ax
0000050F  C680982E01        mov byte [bx+si+0x2e98],0x1
00000514  8BDE              mov bx,si
00000516  D1E3              shl bx,1
00000518  8D46F2            lea ax,[bp-0xe]
0000051B  03D8              add bx,ax
0000051D  368B07            mov ax,[ss:bx]
00000520  BA0900            mov dx,0x9
00000523  F7EA              imul dx
00000525  8B56EA            mov dx,[bp-0x16]
00000528  2BD0              sub dx,ax
0000052A  8956EA            mov [bp-0x16],dx
0000052D  46                inc si
0000052E  83FE03            cmp si,byte +0x3
00000531  7E9A              jng 0x4cd
00000533  5F                pop di
00000534  5E                pop si
00000535  8BE5              mov sp,bp
00000537  5D                pop bp
00000538  CB                retf

; ---- 12C8:0539 ----
00000539  55                push bp
0000053A  8BEC              mov bp,sp
0000053C  56                push si
0000053D  57                push di
0000053E  C606AA3700        mov byte [0x37aa],0x0
00000543  E9CD00            jmp 0x613
00000546  A0AA37            mov al,[0x37aa]
00000549  98                cbw
0000054A  8BD8              mov bx,ax
0000054C  80BF400801        cmp byte [bx+0x840],0x1
00000551  7403              jz 0x556
00000553  E9B900            jmp 0x60f
00000556  A0AA37            mov al,[0x37aa]
00000559  98                cbw
0000055A  D1E0              shl ax,1
0000055C  8BD8              mov bx,ax
0000055E  8BBF5230          mov di,[bx+0x3052]
00000562  A0AA37            mov al,[0x37aa]
00000565  98                cbw
00000566  D1E0              shl ax,1
00000568  8BD8              mov bx,ax
0000056A  8BB75E30          mov si,[bx+0x305e]
0000056E  A0AA37            mov al,[0x37aa]
00000571  98                cbw
00000572  8BD8              mov bx,ax
00000574  C687C33700        mov byte [bx+0x37c3],0x0
00000579  A0AA37            mov al,[0x37aa]
0000057C  98                cbw
0000057D  D1E0              shl ax,1
0000057F  D1E0              shl ax,1
00000581  8BD8              mov bx,ax
00000583  83BF883700        cmp word [bx+0x3788],byte +0x0
00000588  7F23              jg 0x5ad
0000058A  7507              jnz 0x593
0000058C  83BF863700        cmp word [bx+0x3786],byte +0x0
00000591  771A              ja 0x5ad
00000593  A0AA37            mov al,[0x37aa]
00000596  98                cbw
00000597  D1E0              shl ax,1
00000599  D1E0              shl ax,1
0000059B  8BD8              mov bx,ax
0000059D  83BF303000        cmp word [bx+0x3030],byte +0x0
000005A2  7C0B              jl 0x5af
000005A4  7F07              jg 0x5ad
000005A6  83BF2E3000        cmp word [bx+0x302e],byte +0x0
000005AB  7602              jna 0x5af
000005AD  EB60              jmp short 0x60f
000005AF  A0AA37            mov al,[0x37aa]
000005B2  98                cbw
000005B3  8BD8              mov bx,ax
000005B5  80BFB2430E        cmp byte [bx+0x43b2],0xe
000005BA  754E              jnz 0x60a
000005BC  A0AA37            mov al,[0x37aa]
000005BF  98                cbw
000005C0  D1E0              shl ax,1
000005C2  8BD8              mov bx,ax
000005C4  39BF4A45          cmp [bx+0x454a],di
000005C8  750E              jnz 0x5d8
000005CA  A0AA37            mov al,[0x37aa]
000005CD  98                cbw
000005CE  D1E0              shl ax,1
000005D0  8BD8              mov bx,ax
000005D2  39B75245          cmp [bx+0x4552],si
000005D6  7407              jz 0x5df
000005D8  9A2106C812        call 0x12c8:0x621
000005DD  EB29              jmp short 0x608
000005DF  A0AA37            mov al,[0x37aa]
000005E2  98                cbw
000005E3  8BD8              mov bx,ax
000005E5  80BFCC4500        cmp byte [bx+0x45cc],0x0
000005EA  7617              jna 0x603
000005EC  A0AA37            mov al,[0x37aa]
000005EF  98                cbw
000005F0  8BD8              mov bx,ax
000005F2  C687C33710        mov byte [bx+0x37c3],0x10
000005F7  A0AA37            mov al,[0x37aa]
000005FA  98                cbw
000005FB  8BD8              mov bx,ax
000005FD  FE8FCC45          dec byte [bx+0x45cc]
00000601  EB05              jmp short 0x608
00000603  9AD606C812        call 0x12c8:0x6d6
00000608  EB05              jmp short 0x60f
0000060A  9AEA08C812        call 0x12c8:0x8ea
0000060F  FE06AA37          inc byte [0x37aa]
00000613  803EAA3704        cmp byte [0x37aa],0x4
00000618  7D03              jnl 0x61d
0000061A  E929FF            jmp 0x546
0000061D  5F                pop di
0000061E  5E                pop si
0000061F  5D                pop bp
00000620  CB                retf

; ---- 12C8:0621 ----
00000621  55                push bp
00000622  8BEC              mov bp,sp
00000624  56                push si
00000625  57                push di
00000626  A0AA37            mov al,[0x37aa]
00000629  98                cbw
0000062A  D1E0              shl ax,1
0000062C  8BD8              mov bx,ax
0000062E  8B874A45          mov ax,[bx+0x454a]
00000632  8BC8              mov cx,ax
00000634  A0AA37            mov al,[0x37aa]
00000637  98                cbw
00000638  D1E0              shl ax,1
0000063A  8BD8              mov bx,ax
0000063C  8BBF5245          mov di,[bx+0x4552]
00000640  A0AA37            mov al,[0x37aa]
00000643  98                cbw
00000644  D1E0              shl ax,1
00000646  8BD8              mov bx,ax
00000648  8B875230          mov ax,[bx+0x3052]
0000064C  8BD0              mov dx,ax
0000064E  A0AA37            mov al,[0x37aa]
00000651  98                cbw
00000652  D1E0              shl ax,1
00000654  8BD8              mov bx,ax
00000656  8BB75E30          mov si,[bx+0x305e]
0000065A  8BC1              mov ax,cx
0000065C  3BC2              cmp ax,dx
0000065E  7E1A              jng 0x67a
00000660  A0AA37            mov al,[0x37aa]
00000663  98                cbw
00000664  8BD8              mov bx,ax
00000666  8A87C337          mov al,[bx+0x37c3]
0000066A  0C01              or al,0x1
0000066C  50                push ax
0000066D  A0AA37            mov al,[0x37aa]
00000670  98                cbw
00000671  8BD8              mov bx,ax
00000673  58                pop ax
00000674  8887C337          mov [bx+0x37c3],al
00000678  EB1E              jmp short 0x698
0000067A  8BC1              mov ax,cx
0000067C  3BC2              cmp ax,dx
0000067E  7D18              jnl 0x698
00000680  A0AA37            mov al,[0x37aa]
00000683  98                cbw
00000684  8BD8              mov bx,ax
00000686  8A87C337          mov al,[bx+0x37c3]
0000068A  0C02              or al,0x2
0000068C  50                push ax
0000068D  A0AA37            mov al,[0x37aa]
00000690  98                cbw
00000691  8BD8              mov bx,ax
00000693  58                pop ax
00000694  8887C337          mov [bx+0x37c3],al
00000698  3BFE              cmp di,si
0000069A  7E1A              jng 0x6b6
0000069C  A0AA37            mov al,[0x37aa]
0000069F  98                cbw
000006A0  8BD8              mov bx,ax
000006A2  8A87C337          mov al,[bx+0x37c3]
000006A6  0C08              or al,0x8
000006A8  50                push ax
000006A9  A0AA37            mov al,[0x37aa]
000006AC  98                cbw
000006AD  8BD8              mov bx,ax
000006AF  58                pop ax
000006B0  8887C337          mov [bx+0x37c3],al
000006B4  EB1C              jmp short 0x6d2
000006B6  3BFE              cmp di,si
000006B8  7D18              jnl 0x6d2
000006BA  A0AA37            mov al,[0x37aa]
000006BD  98                cbw
000006BE  8BD8              mov bx,ax
000006C0  8A87C337          mov al,[bx+0x37c3]
000006C4  0C04              or al,0x4
000006C6  50                push ax
000006C7  A0AA37            mov al,[0x37aa]
000006CA  98                cbw
000006CB  8BD8              mov bx,ax
000006CD  58                pop ax
000006CE  8887C337          mov [bx+0x37c3],al
000006D2  5F                pop di
000006D3  5E                pop si
000006D4  5D                pop bp
000006D5  CB                retf

; ---- 12C8:06D6 ----
000006D6  55                push bp
000006D7  8BEC              mov bp,sp
000006D9  83EC06            sub sp,byte +0x6
000006DC  56                push si
000006DD  A0AA37            mov al,[0x37aa]
000006E0  98                cbw
000006E1  D1E0              shl ax,1
000006E3  8BD8              mov bx,ax
000006E5  8A875230          mov al,[bx+0x3052]
000006E9  8846FF            mov [bp-0x1],al
000006EC  A0AA37            mov al,[0x37aa]
000006EF  98                cbw
000006F0  D1E0              shl ax,1
000006F2  8BD8              mov bx,ax
000006F4  8A875E30          mov al,[bx+0x305e]
000006F8  8846FE            mov [bp-0x2],al
000006FB  A0AA37            mov al,[0x37aa]
000006FE  98                cbw
000006FF  8BD8              mov bx,ax
00000701  8A874408          mov al,[bx+0x844]
00000705  3A46FF            cmp al,[bp-0x1]
00000708  752B              jnz 0x735
0000070A  A0AA37            mov al,[0x37aa]
0000070D  98                cbw
0000070E  8BD8              mov bx,ax
00000710  8A874808          mov al,[bx+0x848]
00000714  3A46FE            cmp al,[bp-0x2]
00000717  751C              jnz 0x735
00000719  A0AA37            mov al,[0x37aa]
0000071C  98                cbw
0000071D  8BD8              mov bx,ax
0000071F  8A875408          mov al,[bx+0x854]
00000723  FEC8              dec al
00000725  8846FF            mov [bp-0x1],al
00000728  A0AA37            mov al,[0x37aa]
0000072B  98                cbw
0000072C  8BD8              mov bx,ax
0000072E  8A875C08          mov al,[bx+0x85c]
00000732  8846FE            mov [bp-0x2],al
00000735  8A46FF            mov al,[bp-0x1]
00000738  FEC0              inc al
0000073A  8846FF            mov [bp-0x1],al
0000073D  50                push ax
0000073E  A0AA37            mov al,[0x37aa]
00000741  98                cbw
00000742  8BD8              mov bx,ax
00000744  58                pop ax
00000745  3A875808          cmp al,[bx+0x858]
00000749  7655              jna 0x7a0
0000074B  A0AA37            mov al,[0x37aa]
0000074E  98                cbw
0000074F  8BD8              mov bx,ax
00000751  8A875408          mov al,[bx+0x854]
00000755  8846FF            mov [bp-0x1],al
00000758  8A46FE            mov al,[bp-0x2]
0000075B  FEC0              inc al
0000075D  8846FE            mov [bp-0x2],al
00000760  50                push ax
00000761  A0AA37            mov al,[0x37aa]
00000764  98                cbw
00000765  8BD8              mov bx,ax
00000767  58                pop ax
00000768  3A876008          cmp al,[bx+0x860]
0000076C  7632              jna 0x7a0
0000076E  A0AA37            mov al,[0x37aa]
00000771  98                cbw
00000772  8BD8              mov bx,ax
00000774  FE875046          inc byte [bx+0x4650]
00000778  A0AA37            mov al,[0x37aa]
0000077B  98                cbw
0000077C  8BD8              mov bx,ax
0000077E  8A874408          mov al,[bx+0x844]
00000782  8846FF            mov [bp-0x1],al
00000785  A0AA37            mov al,[0x37aa]
00000788  98                cbw
00000789  8BD8              mov bx,ax
0000078B  8A874808          mov al,[bx+0x848]
0000078F  8846FE            mov [bp-0x2],al
00000792  A0AA37            mov al,[0x37aa]
00000795  98                cbw
00000796  8BD8              mov bx,ax
00000798  C687CC4501        mov byte [bx+0x45cc],0x1
0000079D  E92501            jmp 0x8c5
000007A0  8A46FF            mov al,[bp-0x1]
000007A3  98                cbw
000007A4  BA1A00            mov dx,0x1a
000007A7  F7EA              imul dx
000007A9  50                push ax
000007AA  8A46FE            mov al,[bp-0x2]
000007AD  98                cbw
000007AE  5B                pop bx
000007AF  03D8              add bx,ax
000007B1  8A87F448          mov al,[bx+0x48f4]
000007B5  B400              mov ah,0x0
000007B7  BB0800            mov bx,0x8
000007BA  99                cwd
000007BB  F7FB              idiv bx
000007BD  8846FD            mov [bp-0x3],al
000007C0  807EFD00          cmp byte [bp-0x3],0x0
000007C4  7409              jz 0x7cf
000007C6  807EFD02          cmp byte [bp-0x3],0x2
000007CA  7403              jz 0x7cf
000007CC  E9F300            jmp 0x8c2
000007CF  8A46FF            mov al,[bp-0x1]
000007D2  98                cbw
000007D3  48                dec ax
000007D4  BB0300            mov bx,0x3
000007D7  99                cwd
000007D8  F7FB              idiv bx
000007DA  8846FC            mov [bp-0x4],al
000007DD  8A46FE            mov al,[bp-0x2]
000007E0  98                cbw
000007E1  48                dec ax
000007E2  BB0300            mov bx,0x3
000007E5  99                cwd
000007E6  F7FB              idiv bx
000007E8  8846FB            mov [bp-0x5],al
000007EB  8A46FB            mov al,[bp-0x5]
000007EE  98                cbw
000007EF  50                push ax
000007F0  8A46FC            mov al,[bp-0x4]
000007F3  98                cbw
000007F4  50                push ax
000007F5  0E                push cs
000007F6  E8F1FA            call 0x2ea
000007F9  59                pop cx
000007FA  59                pop cx
000007FB  0AC0              or al,al
000007FD  7503              jnz 0x802
000007FF  E9C000            jmp 0x8c2
00000802  8A46FB            mov al,[bp-0x5]
00000805  98                cbw
00000806  50                push ax
00000807  8A46FC            mov al,[bp-0x4]
0000080A  98                cbw
0000080B  50                push ax
0000080C  0E                push cs
0000080D  E83EFA            call 0x24e
00000810  59                pop cx
00000811  59                pop cx
00000812  8BF0              mov si,ax
00000814  A0AA37            mov al,[0x37aa]
00000817  98                cbw
00000818  8BD8              mov bx,ax
0000081A  80BF504602        cmp byte [bx+0x4650],0x2
0000081F  7521              jnz 0x842
00000821  A0AA37            mov al,[0x37aa]
00000824  98                cbw
00000825  D1E0              shl ax,1
00000827  D1E0              shl ax,1
00000829  8BD8              mov bx,ax
0000082B  80B8982E00        cmp byte [bx+si+0x2e98],0x0
00000830  7503              jnz 0x835
00000832  E98D00            jmp 0x8c2
00000835  A0AA37            mov al,[0x37aa]
00000838  98                cbw
00000839  8BD8              mov bx,ax
0000083B  C687CC4501        mov byte [bx+0x45cc],0x1
00000840  EB7E              jmp short 0x8c0
00000842  A0AA37            mov al,[0x37aa]
00000845  98                cbw
00000846  D1E0              shl ax,1
00000848  D1E0              shl ax,1
0000084A  8BD8              mov bx,ax
0000084C  80B8CE4400        cmp byte [bx+si+0x44ce],0x0
00000851  7502              jnz 0x855
00000853  EB6D              jmp short 0x8c2
00000855  A0AA37            mov al,[0x37aa]
00000858  98                cbw
00000859  8BD8              mov bx,ax
0000085B  80BF504600        cmp byte [bx+0x4650],0x0
00000860  7544              jnz 0x8a6
00000862  A0AA37            mov al,[0x37aa]
00000865  98                cbw
00000866  D1E0              shl ax,1
00000868  D1E0              shl ax,1
0000086A  8BD8              mov bx,ax
0000086C  80B84C3700        cmp byte [bx+si+0x374c],0x0
00000871  7502              jnz 0x875
00000873  EB4D              jmp short 0x8c2
00000875  8A46FF            mov al,[bp-0x1]
00000878  98                cbw
00000879  05FEFF            add ax,0xfffe
0000087C  BB0300            mov bx,0x3
0000087F  99                cwd
00000880  F7FB              idiv bx
00000882  0BD2              or dx,dx
00000884  7511              jnz 0x897
00000886  8A46FE            mov al,[bp-0x2]
00000889  98                cbw
0000088A  05FEFF            add ax,0xfffe
0000088D  BB0300            mov bx,0x3
00000890  99                cwd
00000891  F7FB              idiv bx
00000893  0BD2              or dx,dx
00000895  7402              jz 0x899
00000897  EB29              jmp short 0x8c2
00000899  A0AA37            mov al,[0x37aa]
0000089C  98                cbw
0000089D  8BD8              mov bx,ax
0000089F  C687CC4501        mov byte [bx+0x45cc],0x1
000008A4  EB1A              jmp short 0x8c0
000008A6  A0AA37            mov al,[0x37aa]
000008A9  98                cbw
000008AA  D1E0              shl ax,1
000008AC  D1E0              shl ax,1
000008AE  8BD8              mov bx,ax
000008B0  8A80CE44          mov al,[bx+si+0x44ce]
000008B4  50                push ax
000008B5  A0AA37            mov al,[0x37aa]
000008B8  98                cbw
000008B9  8BD8              mov bx,ax
000008BB  58                pop ax
000008BC  8887CC45          mov [bx+0x45cc],al
000008C0  EB03              jmp short 0x8c5
000008C2  E970FE            jmp 0x735
000008C5  A0AA37            mov al,[0x37aa]
000008C8  98                cbw
000008C9  D1E0              shl ax,1
000008CB  50                push ax
000008CC  8A46FF            mov al,[bp-0x1]
000008CF  98                cbw
000008D0  5B                pop bx
000008D1  89874A45          mov [bx+0x454a],ax
000008D5  A0AA37            mov al,[0x37aa]
000008D8  98                cbw
000008D9  D1E0              shl ax,1
000008DB  50                push ax
000008DC  8A46FE            mov al,[bp-0x2]
000008DF  98                cbw
000008E0  5B                pop bx
000008E1  89875245          mov [bx+0x4552],ax
000008E5  5E                pop si
000008E6  8BE5              mov sp,bp
000008E8  5D                pop bp
000008E9  CB                retf

; ---- 12C8:08EA ----
000008EA  55                push bp
000008EB  8BEC              mov bp,sp
000008ED  A0AA37            mov al,[0x37aa]
000008F0  98                cbw
000008F1  8BD8              mov bx,ax
000008F3  8A87902E          mov al,[bx+0x2e90]
000008F7  50                push ax
000008F8  A0AA37            mov al,[0x37aa]
000008FB  98                cbw
000008FC  8BD8              mov bx,ax
000008FE  58                pop ax
000008FF  3A875046          cmp al,[bx+0x4650]
00000903  731A              jnc 0x91f
00000905  A0AA37            mov al,[0x37aa]
00000908  98                cbw
00000909  8BD8              mov bx,ax
0000090B  8A87C337          mov al,[bx+0x37c3]
0000090F  0C01              or al,0x1
00000911  50                push ax
00000912  A0AA37            mov al,[0x37aa]
00000915  98                cbw
00000916  8BD8              mov bx,ax
00000918  58                pop ax
00000919  8887C337          mov [bx+0x37c3],al
0000091D  EB3D              jmp short 0x95c
0000091F  A0AA37            mov al,[0x37aa]
00000922  98                cbw
00000923  8BD8              mov bx,ax
00000925  8A87902E          mov al,[bx+0x2e90]
00000929  50                push ax
0000092A  A0AA37            mov al,[0x37aa]
0000092D  98                cbw
0000092E  8BD8              mov bx,ax
00000930  58                pop ax
00000931  3A875046          cmp al,[bx+0x4650]
00000935  761A              jna 0x951
00000937  A0AA37            mov al,[0x37aa]
0000093A  98                cbw
0000093B  8BD8              mov bx,ax
0000093D  8A87C337          mov al,[bx+0x37c3]
00000941  0C02              or al,0x2
00000943  50                push ax
00000944  A0AA37            mov al,[0x37aa]
00000947  98                cbw
00000948  8BD8              mov bx,ax
0000094A  58                pop ax
0000094B  8887C337          mov [bx+0x37c3],al
0000094F  EB                db 0xeb
