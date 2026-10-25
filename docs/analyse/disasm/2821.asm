; segment 2821  file offset 0x2B810  size 0x2F0
00000000  2E8C5D02          mov [cs:di+0x2],ds
00000004  9D                popf
00000005  5F                pop di
00000006  9D                popf
00000007  CF                iret

; ---- 2821:0008 ----
00000008  55                push bp
00000009  8BEC              mov bp,sp
0000000B  83EC08            sub sp,byte +0x8
0000000E  57                push di
0000000F  56                push si
00000010  1E                push ds
00000011  C47E0A            les di,[bp+0xa]
00000014  C57606            lds si,[bp+0x6]
00000017  E80E00            call 0x28
0000001A  8B46F8            mov ax,[bp-0x8]
0000001D  8B56FA            mov dx,[bp-0x6]
00000020  1F                pop ds
00000021  5E                pop si
00000022  5F                pop di
00000023  83C408            add sp,byte +0x8
00000026  5D                pop bp
00000027  CB                retf
00000028  BA0100            mov dx,0x1
0000002B  83EE01            sub si,byte +0x1
0000002E  7322              jnc 0x52
00000030  90                nop
00000031  90                nop
00000032  90                nop
00000033  50                push ax
00000034  8CD8              mov ax,ds
00000036  2D0010            sub ax,0x1000
00000039  8ED8              mov ds,ax
0000003B  58                pop ax
0000003C  83FEFF            cmp si,byte -0x1
0000003F  7511              jnz 0x52
00000041  90                nop
00000042  90                nop
00000043  90                nop
00000044  81EE0080          sub si,0x8000
00000048  56                push si
00000049  8CDE              mov si,ds
0000004B  81C60008          add si,0x800
0000004F  8EDE              mov ds,si
00000051  5E                pop si
00000052  8A0C              mov cl,[si]
00000054  51                push cx
00000055  83EE01            sub si,byte +0x1
00000058  7322              jnc 0x7c
0000005A  90                nop
0000005B  90                nop
0000005C  90                nop
0000005D  50                push ax
0000005E  8CD8              mov ax,ds
00000060  2D0010            sub ax,0x1000
00000063  8ED8              mov ds,ax
00000065  58                pop ax
00000066  83FEFF            cmp si,byte -0x1
00000069  7511              jnz 0x7c
0000006B  90                nop
0000006C  90                nop
0000006D  90                nop
0000006E  81EE0080          sub si,0x8000
00000072  56                push si
00000073  8CDE              mov si,ds
00000075  81C60008          add si,0x800
00000079  8EDE              mov ds,si
0000007B  5E                pop si
0000007C  8A0C              mov cl,[si]
0000007E  83EE01            sub si,byte +0x1
00000081  7322              jnc 0xa5
00000083  90                nop
00000084  90                nop
00000085  90                nop
00000086  50                push ax
00000087  8CD8              mov ax,ds
00000089  2D0010            sub ax,0x1000
0000008C  8ED8              mov ds,ax
0000008E  58                pop ax
0000008F  83FEFF            cmp si,byte -0x1
00000092  7511              jnz 0xa5
00000094  90                nop
00000095  90                nop
00000096  90                nop
00000097  81EE0080          sub si,0x8000
0000009B  56                push si
0000009C  8CDE              mov si,ds
0000009E  81C60008          add si,0x800
000000A2  8EDE              mov ds,si
000000A4  5E                pop si
000000A5  8A2C              mov ch,[si]
000000A7  83EE01            sub si,byte +0x1
000000AA  7322              jnc 0xce
000000AC  90                nop
000000AD  90                nop
000000AE  90                nop
000000AF  50                push ax
000000B0  8CD8              mov ax,ds
000000B2  2D0010            sub ax,0x1000
000000B5  8ED8              mov ds,ax
000000B7  58                pop ax
000000B8  83FEFF            cmp si,byte -0x1
000000BB  7511              jnz 0xce
000000BD  90                nop
000000BE  90                nop
000000BF  90                nop
000000C0  81EE0080          sub si,0x8000
000000C4  56                push si
000000C5  8CDE              mov si,ds
000000C7  81C60008          add si,0x800
000000CB  8EDE              mov ds,si
000000CD  5E                pop si
000000CE  8A04              mov al,[si]
000000D0  B400              mov ah,0x0
000000D2  894EFC            mov [bp-0x4],cx
000000D5  8946FE            mov [bp-0x2],ax
000000D8  894EF8            mov [bp-0x8],cx
000000DB  8946FA            mov [bp-0x6],ax
000000DE  03F9              add di,cx
000000E0  730C              jnc 0xee
000000E2  90                nop
000000E3  90                nop
000000E4  90                nop
000000E5  50                push ax
000000E6  8CC0              mov ax,es
000000E8  050010            add ax,0x1000
000000EB  8EC0              mov es,ax
000000ED  58                pop ax
000000EE  83FFFF            cmp di,byte -0x1
000000F1  7511              jnz 0x104
000000F3  90                nop
000000F4  90                nop
000000F5  90                nop
000000F6  81EF0080          sub di,0x8000
000000FA  57                push di
000000FB  8CC7              mov di,es
000000FD  81C70008          add di,0x800
00000101  8EC7              mov es,di
00000103  5F                pop di
00000104  D1C8              ror ax,1
00000106  D1C8              ror ax,1
00000108  D1C8              ror ax,1
0000010A  D1C8              ror ax,1
0000010C  8CC1              mov cx,es
0000010E  03C8              add cx,ax
00000110  8EC1              mov es,cx
00000112  59                pop cx
00000113  0AC9              or cl,cl
00000115  740A              jz 0x121
00000117  90                nop
00000118  90                nop
00000119  90                nop
0000011A  E84701            call 0x264
0000011D  FEC9              dec cl
0000011F  75F9              jnz 0x11a
00000121  E84001            call 0x264
00000124  7258              jc 0x17e
00000126  90                nop
00000127  90                nop
00000128  90                nop
00000129  BB0000            mov bx,0x0
0000012C  B102              mov cl,0x2
0000012E  E86D01            call 0x29e
00000131  03D8              add bx,ax
00000133  3D0300            cmp ax,0x3
00000136  74F4              jz 0x12c
00000138  43                inc bx
00000139  295EFC            sub [bp-0x4],bx
0000013C  835EFE00          sbb word [bp-0x2],byte +0x0
00000140  B108              mov cl,0x8
00000142  E85901            call 0x29e
00000145  83EF01            sub di,byte +0x1
00000148  7322              jnc 0x16c
0000014A  90                nop
0000014B  90                nop
0000014C  90                nop
0000014D  50                push ax
0000014E  8CC0              mov ax,es
00000150  2D0010            sub ax,0x1000
00000153  8EC0              mov es,ax
00000155  58                pop ax
00000156  83FFFF            cmp di,byte -0x1
00000159  7511              jnz 0x16c
0000015B  90                nop
0000015C  90                nop
0000015D  90                nop
0000015E  81EF0080          sub di,0x8000
00000162  57                push di
00000163  8CC7              mov di,es
00000165  81C70008          add di,0x800
00000169  8EC7              mov es,di
0000016B  5F                pop di
0000016C  268805            mov [es:di],al
0000016F  4B                dec bx
00000170  75CE              jnz 0x140
00000172  8B5EFE            mov bx,[bp-0x2]
00000175  0B5EFC            or bx,[bp-0x4]
00000178  7504              jnz 0x17e
0000017A  90                nop
0000017B  90                nop
0000017C  90                nop
0000017D  C3                ret
0000017E  B102              mov cl,0x2
00000180  E81B01            call 0x29e
00000183  8AE8              mov ch,al
00000185  1E                push ds
00000186  C55E0E            lds bx,[bp+0xe]
00000189  D7                xlatb
0000018A  1F                pop ds
0000018B  80FD03            cmp ch,0x3
0000018E  7403              jz 0x193
00000190  E9C300            jmp 0x256
00000193  E8CE00            call 0x264
00000196  7205              jc 0x19d
00000198  90                nop
00000199  90                nop
0000019A  90                nop
0000019B  B007              mov al,0x7
0000019D  8AC8              mov cl,al
0000019F  E8FC00            call 0x29e
000001A2  8BD8              mov bx,ax
000001A4  53                push bx
000001A5  8ADD              mov bl,ch
000001A7  B700              mov bh,0x0
000001A9  B103              mov cl,0x3
000001AB  E8F000            call 0x29e
000001AE  03D8              add bx,ax
000001B0  3D0700            cmp ax,0x7
000001B3  74F4              jz 0x1a9
000001B5  8BCB              mov cx,bx
000001B7  5B                pop bx
000001B8  41                inc cx
000001B9  41                inc cx
000001BA  294EFC            sub [bp-0x4],cx
000001BD  835EFE00          sbb word [bp-0x2],byte +0x0
000001C1  1E                push ds
000001C2  56                push si
000001C3  8CC6              mov si,es
000001C5  8EDE              mov ds,si
000001C7  8BF7              mov si,di
000001C9  03F3              add si,bx
000001CB  730C              jnc 0x1d9
000001CD  90                nop
000001CE  90                nop
000001CF  90                nop
000001D0  50                push ax
000001D1  8CD8              mov ax,ds
000001D3  050010            add ax,0x1000
000001D6  8ED8              mov ds,ax
000001D8  58                pop ax
000001D9  83FEFF            cmp si,byte -0x1
000001DC  7511              jnz 0x1ef
000001DE  90                nop
000001DF  90                nop
000001E0  90                nop
000001E1  81EE0080          sub si,0x8000
000001E5  56                push si
000001E6  8CDE              mov si,ds
000001E8  81C60008          add si,0x800
000001EC  8EDE              mov ds,si
000001EE  5E                pop si
000001EF  8A04              mov al,[si]
000001F1  83EE01            sub si,byte +0x1
000001F4  7322              jnc 0x218
000001F6  90                nop
000001F7  90                nop
000001F8  90                nop
000001F9  50                push ax
000001FA  8CD8              mov ax,ds
000001FC  2D0010            sub ax,0x1000
000001FF  8ED8              mov ds,ax
00000201  58                pop ax
00000202  83FEFF            cmp si,byte -0x1
00000205  7511              jnz 0x218
00000207  90                nop
00000208  90                nop
00000209  90                nop
0000020A  81EE0080          sub si,0x8000
0000020E  56                push si
0000020F  8CDE              mov si,ds
00000211  81C60008          add si,0x800
00000215  8EDE              mov ds,si
00000217  5E                pop si
00000218  83EF01            sub di,byte +0x1
0000021B  7322              jnc 0x23f
0000021D  90                nop
0000021E  90                nop
0000021F  90                nop
00000220  50                push ax
00000221  8CC0              mov ax,es
00000223  2D0010            sub ax,0x1000
00000226  8EC0              mov es,ax
00000228  58                pop ax
00000229  83FFFF            cmp di,byte -0x1
0000022C  7511              jnz 0x23f
0000022E  90                nop
0000022F  90                nop
00000230  90                nop
00000231  81EF0080          sub di,0x8000
00000235  57                push di
00000236  8CC7              mov di,es
00000238  81C70008          add di,0x800
0000023C  8EC7              mov es,di
0000023E  5F                pop di
0000023F  268805            mov [es:di],al
00000242  49                dec cx
00000243  75AA              jnz 0x1ef
00000245  5E                pop si
00000246  1F                pop ds
00000247  8B5EFE            mov bx,[bp-0x2]
0000024A  0B5EFC            or bx,[bp-0x4]
0000024D  7406              jz 0x255
0000024F  90                nop
00000250  90                nop
00000251  90                nop
00000252  E9CCFE            jmp 0x121
00000255  C3                ret
00000256  8AC8              mov cl,al
00000258  E84300            call 0x29e
0000025B  8BD8              mov bx,ax
0000025D  8ACD              mov cl,ch
0000025F  B500              mov ch,0x0
00000261  E954FF            jmp 0x1b8
00000264  F8                clc
00000265  D1DA              rcr dx,1
00000267  42                inc dx
00000268  4A                dec dx
00000269  7404              jz 0x26f
0000026B  90                nop
0000026C  90                nop
0000026D  90                nop
0000026E  C3                ret
0000026F  83EE02            sub si,byte +0x2
00000272  7322              jnc 0x296
00000274  90                nop
00000275  90                nop
00000276  90                nop
00000277  50                push ax
00000278  8CD8              mov ax,ds
0000027A  2D0010            sub ax,0x1000
0000027D  8ED8              mov ds,ax
0000027F  58                pop ax
00000280  83FEFF            cmp si,byte -0x1
00000283  7511              jnz 0x296
00000285  90                nop
00000286  90                nop
00000287  90                nop
00000288  81EE0080          sub si,0x8000
0000028C  56                push si
0000028D  8CDE              mov si,ds
0000028F  81C60008          add si,0x800
00000293  8EDE              mov ds,si
00000295  5E                pop si
00000296  8B14              mov dx,[si]
00000298  86D6              xchg dl,dh
0000029A  F9                stc
0000029B  D1DA              rcr dx,1
0000029D  C3                ret
0000029E  B80000            mov ax,0x0
000002A1  F8                clc
000002A2  D1DA              rcr dx,1
000002A4  42                inc dx
000002A5  4A                dec dx
000002A6  740A              jz 0x2b2
000002A8  90                nop
000002A9  90                nop
000002AA  90                nop
000002AB  D1D0              rcl ax,1
000002AD  FEC9              dec cl
000002AF  75F0              jnz 0x2a1
000002B1  C3                ret
000002B2  83EE02            sub si,byte +0x2
000002B5  7322              jnc 0x2d9
000002B7  90                nop
000002B8  90                nop
000002B9  90                nop
000002BA  50                push ax
000002BB  8CD8              mov ax,ds
000002BD  2D0010            sub ax,0x1000
000002C0  8ED8              mov ds,ax
000002C2  58                pop ax
000002C3  83FEFF            cmp si,byte -0x1
000002C6  7511              jnz 0x2d9
000002C8  90                nop
000002C9  90                nop
000002CA  90                nop
000002CB  81EE0080          sub si,0x8000
000002CF  56                push si
000002D0  8CDE              mov si,ds
000002D2  81C60008          add si,0x800
000002D6  8EDE              mov ds,si
000002D8  5E                pop si
000002D9  8B14              mov dx,[si]
000002DB  86D6              xchg dl,dh
000002DD  F9                stc
000002DE  D1DA              rcr dx,1
000002E0  D1D0              rcl ax,1
000002E2  FEC9              dec cl
000002E4  75BB              jnz 0x2a1
000002E6  C3                ret
000002E7  0000              add [bx+si],al
000002E9  0000              add [bx+si],al
000002EB  0000              add [bx+si],al
000002ED  0000              add [bx+si],al
000002EF  00                db 0x00
