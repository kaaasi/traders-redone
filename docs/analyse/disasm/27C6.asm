; segment 27C6  file offset 0x2B260  size 0x5B0

; ---- 27C6:0000 ----
00000000  57                push di
00000001  1E                push ds
00000002  B8DC28            mov ax,0x28dc
00000005  8ED8              mov ds,ax
00000007  06                push es
00000008  B435              mov ah,0x35
0000000A  B009              mov al,0x9
0000000C  CD21              int 0x21
0000000E  BF5911            mov di,0x1159
00000011  8C05              mov [di],es
00000013  895D02            mov [di+0x2],bx
00000016  07                pop es
00000017  1E                push ds
00000018  0E                push cs
00000019  1F                pop ds
0000001A  BA4300            mov dx,0x43
0000001D  B425              mov ah,0x25
0000001F  B009              mov al,0x9
00000021  CD21              int 0x21
00000023  1F                pop ds
00000024  1F                pop ds
00000025  5F                pop di
00000026  CB                retf

; ---- 27C6:0027 ----
00000027  57                push di
00000028  1E                push ds
00000029  B8DC28            mov ax,0x28dc
0000002C  8ED8              mov ds,ax
0000002E  1E                push ds
0000002F  BF5911            mov di,0x1159
00000032  8B5502            mov dx,[di+0x2]
00000035  8B05              mov ax,[di]
00000037  8ED8              mov ds,ax
00000039  B425              mov ah,0x25
0000003B  B009              mov al,0x9
0000003D  CD21              int 0x21
0000003F  1F                pop ds
00000040  1F                pop ds
00000041  5F                pop di
00000042  CB                retf
00000043  50                push ax
00000044  53                push bx
00000045  1E                push ds
00000046  06                push es
00000047  B8DC28            mov ax,0x28dc
0000004A  8ED8              mov ds,ax
0000004C  E460              in al,0x60
0000004E  8AD8              mov bl,al
00000050  E461              in al,0x61
00000052  8AE0              mov ah,al
00000054  0C80              or al,0x80
00000056  E661              out 0x61,al
00000058  86E0              xchg ah,al
0000005A  E661              out 0x61,al
0000005C  8AC3              mov al,bl
0000005E  32E4              xor ah,ah
00000060  BBDC28            mov bx,0x28dc
00000063  8EC3              mov es,bx
00000065  BB473D            mov bx,0x3d47
00000068  A880              test al,0x80
0000006A  7512              jnz 0x7e
0000006C  A25811            mov [0x1158],al
0000006F  FE065D11          inc byte [0x115d]
00000073  03D8              add bx,ax
00000075  26C60701          mov byte [es:bx],0x1
00000079  3C07              cmp al,0x7
0000007B  EB09              jmp short 0x86
0000007D  90                nop
0000007E  247F              and al,0x7f
00000080  03D8              add bx,ax
00000082  26C60700          mov byte [es:bx],0x0
00000086  B061              mov al,0x61
00000088  E620              out 0x20,al
0000008A  07                pop es
0000008B  1F                pop ds
0000008C  5B                pop bx
0000008D  58                pop ax
0000008E  CF                iret
0000008F  B061              mov al,0x61
00000091  E620              out 0x20,al
00000093  FB                sti
00000094  0E                push cs
00000095  E8DE04            call 0x576
00000098  90                nop
00000099  0E                push cs
0000009A  E88AFF            call 0x27
0000009D  0E                push cs
0000009E  E82203            call 0x3c3
000000A1  90                nop
000000A2  B400              mov ah,0x0
000000A4  B003              mov al,0x3
000000A6  CD10              int 0x10
000000A8  B44C              mov ah,0x4c
000000AA  CD21              int 0x21

; ---- 27C6:00AC ----
000000AC  57                push di
000000AD  56                push si
000000AE  1E                push ds
000000AF  B8DC28            mov ax,0x28dc
000000B2  8EC0              mov es,ax
000000B4  8ED8              mov ds,ax
000000B6  BF473D            mov di,0x3d47
000000B9  B98000            mov cx,0x80
000000BC  32C0              xor al,al
000000BE  F3AA              rep stosb
000000C0  FA                cli
000000C1  33C0              xor ax,ax
000000C3  8EC0              mov es,ax
000000C5  26A01A04          mov al,[es:0x41a]
000000C9  26A21C04          mov [es:0x41c],al
000000CD  FB                sti
000000CE  1F                pop ds
000000CF  5E                pop si
000000D0  5F                pop di
000000D1  CB                retf

; ---- 27C6:00D2 ----
000000D2  1E                push ds
000000D3  B8DC28            mov ax,0x28dc
000000D6  8ED8              mov ds,ax
000000D8  C6065D1100        mov byte [0x115d],0x0
000000DD  F6065D11FF        test byte [0x115d],0xff
000000E2  74F9              jz 0xdd
000000E4  1F                pop ds
000000E5  CB                retf

; ---- 27C6:00E6 ----
000000E6  55                push bp
000000E7  8BEC              mov bp,sp
000000E9  57                push di
000000EA  56                push si
000000EB  1E                push ds
000000EC  B8DC28            mov ax,0x28dc
000000EF  8ED8              mov ds,ax
000000F1  8A4606            mov al,[bp+0x6]
000000F4  E81C01            call 0x213
000000F7  0AC0              or al,al
000000F9  7576              jnz 0x171
000000FB  BF5E11            mov di,0x115e
000000FE  8A4606            mov al,[bp+0x6]
00000101  E8D100            call 0x1d5
00000104  8A4608            mov al,[bp+0x8]
00000107  0AC0              or al,al
00000109  746E              jz 0x179
0000010B  FEC8              dec al
0000010D  7467              jz 0x176
0000010F  8A4606            mov al,[bp+0x6]
00000112  3C01              cmp al,0x1
00000114  756A              jnz 0x180
00000116  891E6611          mov [0x1166],bx
0000011A  890E6811          mov [0x1168],cx
0000011E  8BC3              mov ax,bx
00000120  2B05              sub ax,[di]
00000122  D1E8              shr ax,1
00000124  D1E8              shr ax,1
00000126  0BC0              or ax,ax
00000128  7503              jnz 0x12d
0000012A  B80100            mov ax,0x1
0000012D  A36A11            mov [0x116a],ax
00000130  8B4504            mov ax,[di+0x4]
00000133  2BC3              sub ax,bx
00000135  D1E8              shr ax,1
00000137  D1E8              shr ax,1
00000139  0BC0              or ax,ax
0000013B  7503              jnz 0x140
0000013D  B80100            mov ax,0x1
00000140  A36C11            mov [0x116c],ax
00000143  8BC1              mov ax,cx
00000145  2B4502            sub ax,[di+0x2]
00000148  D1E8              shr ax,1
0000014A  D1E8              shr ax,1
0000014C  0BC0              or ax,ax
0000014E  7503              jnz 0x153
00000150  B80100            mov ax,0x1
00000153  A36E11            mov [0x116e],ax
00000156  8B4506            mov ax,[di+0x6]
00000159  2BC1              sub ax,cx
0000015B  D1E8              shr ax,1
0000015D  D1E8              shr ax,1
0000015F  0BC0              or ax,ax
00000161  7503              jnz 0x166
00000163  B80100            mov ax,0x1
00000166  A37011            mov [0x1170],ax
00000169  B80000            mov ax,0x0
0000016C  1F                pop ds
0000016D  5E                pop si
0000016E  5F                pop di
0000016F  5D                pop bp
00000170  CB                retf
00000171  B80100            mov ax,0x1
00000174  EBF6              jmp short 0x16c
00000176  83C704            add di,byte +0x4
00000179  891D              mov [di],bx
0000017B  894D02            mov [di+0x2],cx
0000017E  EBE9              jmp short 0x169
00000180  891E7211          mov [0x1172],bx
00000184  890E7411          mov [0x1174],cx
00000188  8BC3              mov ax,bx
0000018A  2B05              sub ax,[di]
0000018C  D1E8              shr ax,1
0000018E  D1E8              shr ax,1
00000190  0BC0              or ax,ax
00000192  7503              jnz 0x197
00000194  B80100            mov ax,0x1
00000197  A37611            mov [0x1176],ax
0000019A  8B4504            mov ax,[di+0x4]
0000019D  2BC3              sub ax,bx
0000019F  D1E8              shr ax,1
000001A1  D1E8              shr ax,1
000001A3  0BC0              or ax,ax
000001A5  7503              jnz 0x1aa
000001A7  B80100            mov ax,0x1
000001AA  A37811            mov [0x1178],ax
000001AD  8BC1              mov ax,cx
000001AF  2B4502            sub ax,[di+0x2]
000001B2  D1E8              shr ax,1
000001B4  D1E8              shr ax,1
000001B6  0BC0              or ax,ax
000001B8  7503              jnz 0x1bd
000001BA  B80100            mov ax,0x1
000001BD  A37A11            mov [0x117a],ax
000001C0  8B4506            mov ax,[di+0x6]
000001C3  2BC1              sub ax,cx
000001C5  D1E8              shr ax,1
000001C7  D1E8              shr ax,1
000001C9  0BC0              or ax,ax
000001CB  7503              jnz 0x1d0
000001CD  B80100            mov ax,0x1
000001D0  A37C11            mov [0x117c],ax
000001D3  EB94              jmp short 0x169
000001D5  57                push di
000001D6  56                push si
000001D7  33DB              xor bx,bx
000001D9  8BFB              mov di,bx
000001DB  BE50C3            mov si,0xc350
000001DE  B100              mov cl,0x0
000001E0  B503              mov ch,0x3
000001E2  3C01              cmp al,0x1
000001E4  7404              jz 0x1ea
000001E6  B102              mov cl,0x2
000001E8  B50C              mov ch,0xc
000001EA  BA0102            mov dx,0x201
000001ED  FA                cli
000001EE  EE                out dx,al
000001EF  90                nop
000001F0  90                nop
000001F1  90                nop
000001F2  90                nop
000001F3  90                nop
000001F4  90                nop
000001F5  90                nop
000001F6  90                nop
000001F7  90                nop
000001F8  90                nop
000001F9  90                nop
000001FA  32E4              xor ah,ah
000001FC  EC                in al,dx
000001FD  22C5              and al,ch
000001FF  740C              jz 0x20d
00000201  D3E8              shr ax,cl
00000203  D1D8              rcr ax,1
00000205  83D300            adc bx,byte +0x0
00000208  03F8              add di,ax
0000020A  46                inc si
0000020B  75ED              jnz 0x1fa
0000020D  FB                sti
0000020E  8BCF              mov cx,di
00000210  5E                pop si
00000211  5F                pop di
00000212  C3                ret
00000213  B130              mov cl,0x30
00000215  8AE8              mov ch,al
00000217  3C01              cmp al,0x1
00000219  7402              jz 0x21d
0000021B  B1C0              mov cl,0xc0
0000021D  BF473D            mov di,0x3d47
00000220  B8DC28            mov ax,0x28dc
00000223  8EC0              mov es,ax
00000225  BA0102            mov dx,0x201
00000228  268A4501          mov al,[es:di+0x1]
0000022C  0AC0              or al,al
0000022E  752D              jnz 0x25d
00000230  51                push cx
00000231  8AC5              mov al,ch
00000233  E89FFF            call 0x1d5
00000236  59                pop cx
00000237  EC                in al,dx
00000238  22C1              and al,cl
0000023A  3AC1              cmp al,cl
0000023C  74EA              jz 0x228
0000023E  E82201            call 0x363
00000241  268A4501          mov al,[es:di+0x1]
00000245  0AC0              or al,al
00000247  7514              jnz 0x25d
00000249  51                push cx
0000024A  8AC5              mov al,ch
0000024C  E886FF            call 0x1d5
0000024F  59                pop cx
00000250  EC                in al,dx
00000251  22C1              and al,cl
00000253  3AC1              cmp al,cl
00000255  75EA              jnz 0x241
00000257  E80901            call 0x363
0000025A  32C0              xor al,al
0000025C  C3                ret
0000025D  268A4501          mov al,[es:di+0x1]
00000261  0AC0              or al,al
00000263  75F8              jnz 0x25d
00000265  B001              mov al,0x1
00000267  C3                ret

; ---- 27C6:0268 ----
00000268  55                push bp
00000269  8BEC              mov bp,sp
0000026B  57                push di
0000026C  56                push si
0000026D  1E                push ds
0000026E  8A4606            mov al,[bp+0x6]
00000271  E861FF            call 0x1d5
00000274  BA0102            mov dx,0x201
00000277  EC                in al,dx
00000278  50                push ax
00000279  B8DC28            mov ax,0x28dc
0000027C  8ED8              mov ds,ax
0000027E  8A4606            mov al,[bp+0x6]
00000281  C47E08            les di,[bp+0x8]
00000284  3C01              cmp al,0x1
00000286  7571              jnz 0x2f9
00000288  B400              mov ah,0x0
0000028A  8B166611          mov dx,[0x1166]
0000028E  2BDA              sub bx,dx
00000290  7243              jc 0x2d5
00000292  8B166C11          mov dx,[0x116c]
00000296  FEC4              inc ah
00000298  2BDA              sub bx,dx
0000029A  73FA              jnc 0x296
0000029C  FECC              dec ah
0000029E  268825            mov [es:di],ah
000002A1  47                inc di
000002A2  B400              mov ah,0x0
000002A4  8B166811          mov dx,[0x1168]
000002A8  2BCA              sub cx,dx
000002AA  723B              jc 0x2e7
000002AC  8B167011          mov dx,[0x1170]
000002B0  FEC4              inc ah
000002B2  2BCA              sub cx,dx
000002B4  73FA              jnc 0x2b0
000002B6  FECC              dec ah
000002B8  268825            mov [es:di],ah
000002BB  58                pop ax
000002BC  B400              mov ah,0x0
000002BE  A810              test al,0x10
000002C0  7503              jnz 0x2c5
000002C2  80CC01            or ah,0x1
000002C5  A820              test al,0x20
000002C7  7503              jnz 0x2cc
000002C9  80CC02            or ah,0x2
000002CC  8AC4              mov al,ah
000002CE  B400              mov ah,0x0
000002D0  1F                pop ds
000002D1  5E                pop si
000002D2  5F                pop di
000002D3  5D                pop bp
000002D4  CB                retf
000002D5  8B166A11          mov dx,[0x116a]
000002D9  F7DB              neg bx
000002DB  FEC4              inc ah
000002DD  2BDA              sub bx,dx
000002DF  73FA              jnc 0x2db
000002E1  FECC              dec ah
000002E3  F6DC              neg ah
000002E5  EBB7              jmp short 0x29e
000002E7  8B166E11          mov dx,[0x116e]
000002EB  F7D9              neg cx
000002ED  FEC4              inc ah
000002EF  2BCA              sub cx,dx
000002F1  73FA              jnc 0x2ed
000002F3  FECC              dec ah
000002F5  F6DC              neg ah
000002F7  EBBF              jmp short 0x2b8
000002F9  B400              mov ah,0x0
000002FB  8B167211          mov dx,[0x1172]
000002FF  2BDA              sub bx,dx
00000301  723C              jc 0x33f
00000303  8B167811          mov dx,[0x1178]
00000307  FEC4              inc ah
00000309  2BDA              sub bx,dx
0000030B  73FA              jnc 0x307
0000030D  FECC              dec ah
0000030F  268825            mov [es:di],ah
00000312  47                inc di
00000313  B400              mov ah,0x0
00000315  8B167411          mov dx,[0x1174]
00000319  2BCA              sub cx,dx
0000031B  7234              jc 0x351
0000031D  8B167C11          mov dx,[0x117c]
00000321  FEC4              inc ah
00000323  2BCA              sub cx,dx
00000325  73FA              jnc 0x321
00000327  FECC              dec ah
00000329  268825            mov [es:di],ah
0000032C  58                pop ax
0000032D  B400              mov ah,0x0
0000032F  A840              test al,0x40
00000331  7503              jnz 0x336
00000333  80CC01            or ah,0x1
00000336  A880              test al,0x80
00000338  7592              jnz 0x2cc
0000033A  80CC02            or ah,0x2
0000033D  EB8D              jmp short 0x2cc
0000033F  8B167611          mov dx,[0x1176]
00000343  F7DB              neg bx
00000345  FEC4              inc ah
00000347  2BDA              sub bx,dx
00000349  73FA              jnc 0x345
0000034B  FECC              dec ah
0000034D  F6DC              neg ah
0000034F  EBBE              jmp short 0x30f
00000351  8B167A11          mov dx,[0x117a]
00000355  F7D9              neg cx
00000357  FEC4              inc ah
00000359  2BCA              sub cx,dx
0000035B  73FA              jnc 0x357
0000035D  FECC              dec ah
0000035F  F6DC              neg ah
00000361  EBC6              jmp short 0x329
00000363  50                push ax
00000364  53                push bx
00000365  51                push cx
00000366  52                push dx
00000367  06                push es
00000368  B91900            mov cx,0x19
0000036B  51                push cx
0000036C  9AD300730D        call 0xd73:0xd3
00000371  59                pop cx
00000372  E2F7              loop 0x36b
00000374  07                pop es
00000375  5A                pop dx
00000376  59                pop cx
00000377  5B                pop bx
00000378  58                pop ax
00000379  C3                ret

; ---- 27C6:037A ----
0000037A  57                push di
0000037B  1E                push ds
0000037C  B8DC28            mov ax,0x28dc
0000037F  8ED8              mov ds,ax
00000381  B435              mov ah,0x35
00000383  B008              mov al,0x8
00000385  CD21              int 0x21
00000387  BF0E04            mov di,0x40e
0000038A  2E891D            mov [cs:di],bx
0000038D  2E8C4502          mov [cs:di+0x2],es
00000391  BF1204            mov di,0x412
00000394  2E891D            mov [cs:di],bx
00000397  2E8C4502          mov [cs:di+0x2],es
0000039B  0E                push cs
0000039C  1F                pop ds
0000039D  BAE403            mov dx,0x3e4
000003A0  B425              mov ah,0x25
000003A2  B008              mov al,0x8
000003A4  CD21              int 0x21
000003A6  1F                pop ds
000003A7  FA                cli
000003A8  B036              mov al,0x36
000003AA  E643              out 0x43,al
000003AC  B000              mov al,0x0
000003AE  E640              out 0x40,al
000003B0  B040              mov al,0x40
000003B2  E640              out 0x40,al
000003B4  C7067E110000      mov word [0x117e],0x0
000003BA  C70680110000      mov word [0x1180],0x0
000003C0  FB                sti
000003C1  5F                pop di
000003C2  CB                retf

; ---- 27C6:03C3 ----
000003C3  57                push di
000003C4  1E                push ds
000003C5  B8DC28            mov ax,0x28dc
000003C8  8ED8              mov ds,ax
000003CA  FA                cli
000003CB  B036              mov al,0x36
000003CD  E643              out 0x43,al
000003CF  B0FF              mov al,0xff
000003D1  E640              out 0x40,al
000003D3  E640              out 0x40,al
000003D5  FB                sti
000003D6  2EC5160E04        lds dx,[cs:0x40e]
000003DB  B425              mov ah,0x25
000003DD  B008              mov al,0x8
000003DF  CD21              int 0x21
000003E1  1F                pop ds
000003E2  5F                pop di
000003E3  CB                retf
000003E4  50                push ax
000003E5  1E                push ds
000003E6  2EFF1E1604        call far [cs:0x416]
000003EB  B8DC28            mov ax,0x28dc
000003EE  8ED8              mov ds,ax
000003F0  FF067E11          inc word [0x117e]
000003F4  FF068011          inc word [0x1180]
000003F8  A18011            mov ax,[0x1180]
000003FB  250300            and ax,0x3
000003FE  7407              jz 0x407
00000400  B020              mov al,0x20
00000402  E620              out 0x20,al
00000404  1F                pop ds
00000405  58                pop ax
00000406  CF                iret
00000407  1F                pop ds
00000408  58                pop ax
00000409  2EFF2E1204        jmp far [cs:0x412]
0000040E  0000              add [bx+si],al
00000410  0000              add [bx+si],al
00000412  0000              add [bx+si],al
00000414  0000              add [bx+si],al
00000416  1A04              sbb al,[si]
00000418  C6                db 0xc6
00000419  27                daa
0000041A  CB                retf

; ---- 27C6:041B ----
0000041B  55                push bp
0000041C  8BEC              mov bp,sp
0000041E  C44606            les ax,[bp+0x6]
00000421  FA                cli
00000422  2EA31604          mov [cs:0x416],ax
00000426  2E8C061804        mov [cs:0x418],es
0000042B  FB                sti
0000042C  5D                pop bp
0000042D  CB                retf

; ---- 27C6:042E ----
0000042E  57                push di
0000042F  56                push si
00000430  8BF2              mov si,dx
00000432  8BF9              mov di,cx
00000434  26AD              es lodsw
00000436  8AEC              mov ch,ah
00000438  8AE0              mov ah,al
0000043A  26AC              es lodsb
0000043C  8AC8              mov cl,al
0000043E  E80700            call 0x448
00000441  43                inc bx
00000442  4F                dec di
00000443  75EF              jnz 0x434
00000445  5E                pop si
00000446  5F                pop di
00000447  CB                retf
00000448  9C                pushf
00000449  FA                cli
0000044A  BAC803            mov dx,0x3c8
0000044D  8AC3              mov al,bl
0000044F  EE                out dx,al
00000450  EB00              jmp short 0x452
00000452  42                inc dx
00000453  8AC4              mov al,ah
00000455  EE                out dx,al
00000456  EB00              jmp short 0x458
00000458  8AC5              mov al,ch
0000045A  EE                out dx,al
0000045B  EB00              jmp short 0x45d
0000045D  8AC1              mov al,cl
0000045F  EE                out dx,al
00000460  9D                popf
00000461  C3                ret
00000462  52                push dx
00000463  BAC403            mov dx,0x3c4
00000466  B001              mov al,0x1
00000468  E81500            call 0x480
0000046B  80CC14            or ah,0x14
0000046E  EF                out dx,ax
0000046F  5A                pop dx
00000470  C3                ret
00000471  52                push dx
00000472  BAC403            mov dx,0x3c4
00000475  B001              mov al,0x1
00000477  E80600            call 0x480
0000047A  80E4DF            and ah,0xdf
0000047D  EF                out dx,ax
0000047E  5A                pop dx
0000047F  C3                ret
00000480  EE                out dx,al
00000481  EB00              jmp short 0x483
00000483  42                inc dx
00000484  8AE0              mov ah,al
00000486  EC                in al,dx
00000487  4A                dec dx
00000488  86E0              xchg ah,al
0000048A  C3                ret

; ---- 27C6:048B ----
0000048B  55                push bp
0000048C  8BEC              mov bp,sp
0000048E  57                push di
0000048F  56                push si
00000490  1E                push ds
00000491  C57606            lds si,[bp+0x6]
00000494  C47E0A            les di,[bp+0xa]
00000497  83EE01            sub si,byte +0x1
0000049A  7309              jnc 0x4a5
0000049C  50                push ax
0000049D  8CD8              mov ax,ds
0000049F  2D0010            sub ax,0x1000
000004A2  8ED8              mov ds,ax
000004A4  58                pop ax
000004A5  83EF01            sub di,byte +0x1
000004A8  7309              jnc 0x4b3
000004AA  50                push ax
000004AB  8CC0              mov ax,es
000004AD  2D0010            sub ax,0x1000
000004B0  8EC0              mov es,ax
000004B2  58                pop ax
000004B3  8A04              mov al,[si]
000004B5  268805            mov [es:di],al
000004B8  836E0E01          sub word [bp+0xe],byte +0x1
000004BC  835E1000          sbb word [bp+0x10],byte +0x0
000004C0  75D5              jnz 0x497
000004C2  F7460EFFFF        test word [bp+0xe],0xffff
000004C7  75CE              jnz 0x497
000004C9  1F                pop ds
000004CA  5E                pop si
000004CB  5F                pop di
000004CC  5D                pop bp
000004CD  CB                retf

; ---- 27C6:04CE ----
000004CE  55                push bp
000004CF  8BEC              mov bp,sp
000004D1  57                push di
000004D2  56                push si
000004D3  1E                push ds
000004D4  C57606            lds si,[bp+0x6]
000004D7  C47E0A            les di,[bp+0xa]
000004DA  8A04              mov al,[si]
000004DC  268805            mov [es:di],al
000004DF  83C601            add si,byte +0x1
000004E2  7309              jnc 0x4ed
000004E4  50                push ax
000004E5  8CD8              mov ax,ds
000004E7  050010            add ax,0x1000
000004EA  8ED8              mov ds,ax
000004EC  58                pop ax
000004ED  83C701            add di,byte +0x1
000004F0  7309              jnc 0x4fb
000004F2  50                push ax
000004F3  8CC0              mov ax,es
000004F5  050010            add ax,0x1000
000004F8  8EC0              mov es,ax
000004FA  58                pop ax
000004FB  836E0E01          sub word [bp+0xe],byte +0x1
000004FF  835E1000          sbb word [bp+0x10],byte +0x0
00000503  75D5              jnz 0x4da
00000505  F7460EFFFF        test word [bp+0xe],0xffff
0000050A  75CE              jnz 0x4da
0000050C  1F                pop ds
0000050D  5E                pop si
0000050E  5F                pop di
0000050F  5D                pop bp
00000510  CB                retf

; ---- 27C6:0511 ----
00000511  55                push bp
00000512  8BEC              mov bp,sp
00000514  8B16E82B          mov dx,[0x2be8]
00000518  8B4606            mov ax,[bp+0x6]
0000051B  EE                out dx,al
0000051C  EC                in al,dx
0000051D  EC                in al,dx
0000051E  EC                in al,dx
0000051F  EC                in al,dx
00000520  EC                in al,dx
00000521  EC                in al,dx
00000522  42                inc dx
00000523  8B4608            mov ax,[bp+0x8]
00000526  EE                out dx,al
00000527  4A                dec dx
00000528  EC                in al,dx
00000529  EC                in al,dx
0000052A  EC                in al,dx
0000052B  EC                in al,dx
0000052C  EC                in al,dx
0000052D  EC                in al,dx
0000052E  EC                in al,dx
0000052F  EC                in al,dx
00000530  EC                in al,dx
00000531  EC                in al,dx
00000532  EC                in al,dx
00000533  EC                in al,dx
00000534  EC                in al,dx
00000535  EC                in al,dx
00000536  EC                in al,dx
00000537  EC                in al,dx
00000538  EC                in al,dx
00000539  EC                in al,dx
0000053A  EC                in al,dx
0000053B  EC                in al,dx
0000053C  EC                in al,dx
0000053D  EC                in al,dx
0000053E  EC                in al,dx
0000053F  EC                in al,dx
00000540  EC                in al,dx
00000541  EC                in al,dx
00000542  EC                in al,dx
00000543  EC                in al,dx
00000544  EC                in al,dx
00000545  EC                in al,dx
00000546  EC                in al,dx
00000547  EC                in al,dx
00000548  EC                in al,dx
00000549  EC                in al,dx
0000054A  EC                in al,dx
0000054B  5D                pop bp
0000054C  CB                retf
0000054D  0000              add [bx+si],al
0000054F  0000              add [bx+si],al

; ---- 27C6:0551 ----
00000551  57                push di
00000552  1E                push ds
00000553  B8DC28            mov ax,0x28dc
00000556  8ED8              mov ds,ax
00000558  B435              mov ah,0x35
0000055A  B021              mov al,0x21
0000055C  CD21              int 0x21
0000055E  BF4D05            mov di,0x54d
00000561  2E891D            mov [cs:di],bx
00000564  2E8C4502          mov [cs:di+0x2],es
00000568  0E                push cs
00000569  1F                pop ds
0000056A  BA8B05            mov dx,0x58b
0000056D  B425              mov ah,0x25
0000056F  B021              mov al,0x21
00000571  CD21              int 0x21
00000573  1F                pop ds
00000574  5F                pop di
00000575  CB                retf

; ---- 27C6:0576 ----
00000576  57                push di
00000577  1E                push ds
00000578  B8DC28            mov ax,0x28dc
0000057B  8ED8              mov ds,ax
0000057D  2EC5164D05        lds dx,[cs:0x54d]
00000582  B425              mov ah,0x25
00000584  B021              mov al,0x21
00000586  CD21              int 0x21
00000588  1F                pop ds
00000589  5F                pop di
0000058A  CB                retf
0000058B  9C                pushf
0000058C  3C08              cmp al,0x8
0000058E  750A              jnz 0x59a
00000590  80FC25            cmp ah,0x25
00000593  7412              jz 0x5a7
00000595  80FC35            cmp ah,0x35
00000598  7406              jz 0x5a0
0000059A  9D                popf
0000059B  2EFF2E4D05        jmp far [cs:0x54d]
000005A0  2EC41E0E04        les bx,[cs:0x40e]
000005A5  9D                popf
000005A6  CF                iret
000005A7  57                push di
000005A8  BF1204            mov di,0x412
000005AB  9C                pushf
000005AC  FA                cli
000005AD  2E8915            mov [cs:di],dx
