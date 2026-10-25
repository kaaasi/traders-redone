; segment 2850  file offset 0x2BB00  size 0x8A0
00000000  0000              add [bx+si],al
00000002  0000              add [bx+si],al
00000004  0000              add [bx+si],al
00000006  0000              add [bx+si],al
00000008  0000              add [bx+si],al
0000000A  0000              add [bx+si],al
0000000C  0000              add [bx+si],al
0000000E  0000              add [bx+si],al
00000010  0000              add [bx+si],al
00000012  0000              add [bx+si],al
00000014  0000              add [bx+si],al
00000016  0000              add [bx+si],al
00000018  0000              add [bx+si],al
0000001A  0000              add [bx+si],al
0000001C  0000              add [bx+si],al
0000001E  0000              add [bx+si],al
00000020  0000              add [bx+si],al
00000022  0000              add [bx+si],al
00000024  0000              add [bx+si],al
00000026  0000              add [bx+si],al
00000028  0000              add [bx+si],al
0000002A  0000              add [bx+si],al
0000002C  0000              add [bx+si],al
0000002E  0000              add [bx+si],al
00000030  0000              add [bx+si],al
00000032  0000              add [bx+si],al
00000034  0000              add [bx+si],al
00000036  0000              add [bx+si],al
00000038  0000              add [bx+si],al
0000003A  0000              add [bx+si],al
0000003C  0000              add [bx+si],al
0000003E  0000              add [bx+si],al
00000040  0000              add [bx+si],al
00000042  0000              add [bx+si],al
00000044  0000              add [bx+si],al
00000046  0000              add [bx+si],al
00000048  00E9              add cl,ch
0000004A  086A08            or [bp+si+0x8],ch
0000004D  F1                int1
0000004E  07                pop es
0000004F  7E07              jng 0x58
00000051  1307              adc ax,[bx]
00000053  AD                lodsw
00000054  06                push es
00000055  4C                dec sp
00000056  06                push es
00000057  F2059C05          repne add ax,0x59c
0000005B  4C                dec sp
0000005C  050005            add ax,0x500
0000005F  B80450            mov ax,0x5004
00000062  53                push bx
00000063  51                push cx
00000064  52                push dx
00000065  57                push di
00000066  56                push si
00000067  1E                push ds
00000068  8CC8              mov ax,cs
0000006A  8ED8              mov ds,ax
0000006C  E8A700            call 0x116
0000006F  BF0200            mov di,0x2
00000072  E83001            call 0x1a5
00000075  BF1300            mov di,0x13
00000078  E82A01            call 0x1a5
0000007B  BF2400            mov di,0x24
0000007E  E82401            call 0x1a5
00000081  BF0200            mov di,0x2
00000084  8B5D03            mov bx,[di+0x3]
00000087  23DB              and bx,bx
00000089  7510              jnz 0x9b
0000008B  83C711            add di,byte +0x11
0000008E  8B5D03            mov bx,[di+0x3]
00000091  23DB              and bx,bx
00000093  7506              jnz 0x9b
00000095  83C711            add di,byte +0x11
00000098  8B5D03            mov bx,[di+0x3]
0000009B  F6450104          test byte [di+0x1],0x4
0000009F  7511              jnz 0xb2
000000A1  3B1E0000          cmp bx,[0x0]
000000A5  7403              jz 0xaa
000000A7  E88900            call 0x133
000000AA  1F                pop ds
000000AB  5E                pop si
000000AC  5F                pop di
000000AD  5A                pop dx
000000AE  59                pop cx
000000AF  5B                pop bx
000000B0  58                pop ax
000000B1  CB                retf
000000B2  8B750F            mov si,[di+0xf]
000000B5  8A24              mov ah,[si]
000000B7  46                inc si
000000B8  81E6FF07          and si,0x7ff
000000BC  89750F            mov [di+0xf],si
000000BF  80E402            and ah,0x2
000000C2  E461              in al,0x61
000000C4  24FC              and al,0xfc
000000C6  0AC4              or al,ah
000000C8  E661              out 0x61,al
000000CA  EBDE              jmp short 0xaa

; ---- 2850:00CC ----
000000CC  55                push bp
000000CD  8BEC              mov bp,sp
000000CF  1E                push ds
000000D0  8CCB              mov bx,cs
000000D2  8EDB              mov ds,bx
000000D4  8B1E4500          mov bx,[0x45]
000000D8  8B4606            mov ax,[bp+0x6]
000000DB  88873500          mov [bx+0x35],al
000000DF  43                inc bx
000000E0  83E30F            and bx,byte +0xf
000000E3  891E4500          mov [0x45],bx
000000E7  1F                pop ds
000000E8  5D                pop bp
000000E9  CB                retf
000000EA  55                push bp
000000EB  8BEC              mov bp,sp
000000ED  837E0600          cmp word [bp+0x6],byte +0x0
000000F1  7504              jnz 0xf7
000000F3  8CC8              mov ax,cs
000000F5  EB1D              jmp short 0x114
000000F7  837E0601          cmp word [bp+0x6],byte +0x1
000000FB  7505              jnz 0x102
000000FD  B8CD03            mov ax,0x3cd
00000100  EB12              jmp short 0x114
00000102  837E0602          cmp word [bp+0x6],byte +0x2
00000106  7503              jnz 0x10b
00000108  B8FFFF            mov ax,0xffff
0000010B  837E0603          cmp word [bp+0x6],byte +0x3
0000010F  7503              jnz 0x114
00000111  B8FFFF            mov ax,0xffff
00000114  5D                pop bp
00000115  CB                retf
00000116  8B1E4700          mov bx,[0x47]
0000011A  3B1E4500          cmp bx,[0x45]
0000011E  7412              jz 0x132
00000120  8A873500          mov al,[bx+0x35]
00000124  E80601            call 0x22d
00000127  FF064700          inc word [0x47]
0000012B  832647000F        and word [0x47],byte +0xf
00000130  EBE4              jmp short 0x116
00000132  C3                ret
00000133  3B1E0000          cmp bx,[0x0]
00000137  741D              jz 0x156
00000139  891E0000          mov [0x0],bx
0000013D  23DB              and bx,bx
0000013F  7416              jz 0x157
00000141  9C                pushf
00000142  FA                cli
00000143  E461              in al,0x61
00000145  0C03              or al,0x3
00000147  E661              out 0x61,al
00000149  B0B6              mov al,0xb6
0000014B  E643              out 0x43,al
0000014D  8AC3              mov al,bl
0000014F  E642              out 0x42,al
00000151  8AC7              mov al,bh
00000153  E642              out 0x42,al
00000155  9D                popf
00000156  C3                ret
00000157  E461              in al,0x61
00000159  24FC              and al,0xfc
0000015B  E661              out 0x61,al
0000015D  EBF7              jmp short 0x156
0000015F  53                push bx
00000160  51                push cx
00000161  8AC8              mov cl,al
00000163  80E1F0            and cl,0xf0
00000166  D0E9              shr cl,1
00000168  D0E9              shr cl,1
0000016A  D0E9              shr cl,1
0000016C  D0E9              shr cl,1
0000016E  250F00            and ax,0xf
00000171  3C0B              cmp al,0xb
00000173  7E04              jng 0x179
00000175  FEC1              inc cl
00000177  2C0C              sub al,0xc
00000179  BB4900            mov bx,0x49
0000017C  D1E0              shl ax,1
0000017E  03D8              add bx,ax
00000180  8B07              mov ax,[bx]
00000182  80F905            cmp cl,0x5
00000185  7C08              jl 0x18f
00000187  80E905            sub cl,0x5
0000018A  D3E8              shr ax,cl
0000018C  59                pop cx
0000018D  5B                pop bx
0000018E  C3                ret
0000018F  80E905            sub cl,0x5
00000192  F6D9              neg cl
00000194  D3E0              shl ax,cl
00000196  80F905            cmp cl,0x5
00000199  75F1              jnz 0x18c
0000019B  3DD084            cmp ax,0x84d0
0000019E  73EC              jnc 0x18c
000001A0  B8FFFF            mov ax,0xffff
000001A3  EBE7              jmp short 0x18c
000001A5  F605FF            test byte [di],0xff
000001A8  7426              jz 0x1d0
000001AA  807D0200          cmp byte [di+0x2],0x0
000001AE  7540              jnz 0x1f0
000001B0  8B7505            mov si,[di+0x5]
000001B3  8A04              mov al,[si]
000001B5  A880              test al,0x80
000001B7  741D              jz 0x1d6
000001B9  3CD0              cmp al,0xd0
000001BB  7D14              jnl 0x1d1
000001BD  BB2D03            mov bx,0x32d
000001C0  32E4              xor ah,ah
000001C2  2D8000            sub ax,0x80
000001C5  D1E0              shl ax,1
000001C7  03D8              add bx,ax
000001C9  FF17              call [bx]
000001CB  75E6              jnz 0x1b3
000001CD  897505            mov [di+0x5],si
000001D0  C3                ret
000001D1  E89A00            call 0x26e
000001D4  EBFA              jmp short 0x1d0
000001D6  806501F9          and byte [di+0x1],0xf9
000001DA  32E4              xor ah,ah
000001DC  E880FF            call 0x15f
000001DF  894503            mov [di+0x3],ax
000001E2  8A4401            mov al,[si+0x1]
000001E5  884502            mov [di+0x2],al
000001E8  83C602            add si,byte +0x2
000001EB  897505            mov [di+0x5],si
000001EE  EBE0              jmp short 0x1d0
000001F0  FE4D02            dec byte [di+0x2]
000001F3  F6450102          test byte [di+0x1],0x2
000001F7  7416              jz 0x20f
000001F9  8B750F            mov si,[di+0xf]
000001FC  AC                lodsb
000001FD  81E6FF07          and si,0x7ff
00000201  89750F            mov [di+0xf],si
00000204  32E4              xor ah,ah
00000206  F6650C            mul byte [di+0xc]
00000209  03450D            add ax,[di+0xd]
0000020C  894503            mov [di+0x3],ax
0000020F  F6450101          test byte [di+0x1],0x1
00000213  7416              jz 0x22b
00000215  8B4503            mov ax,[di+0x3]
00000218  23C0              and ax,ax
0000021A  740F              jz 0x22b
0000021C  034509            add ax,[di+0x9]
0000021F  894503            mov [di+0x3],ax
00000222  8B450D            mov ax,[di+0xd]
00000225  034509            add ax,[di+0x9]
00000228  89450D            mov [di+0xd],ax
0000022B  EBA3              jmp short 0x1d0
0000022D  32E4              xor ah,ah
0000022F  D1E0              shl ax,1
00000231  BECD03            mov si,0x3cd
00000234  03F0              add si,ax
00000236  8B34              mov si,[si]
00000238  8A04              mov al,[si]
0000023A  46                inc si
0000023B  3C02              cmp al,0x2
0000023D  7E06              jng 0x245
0000023F  3C0A              cmp al,0xa
00000241  742A              jz 0x26d
00000243  B002              mov al,0x2
00000245  32E4              xor ah,ah
00000247  BA1100            mov dx,0x11
0000024A  F7E2              mul dx
0000024C  BF0200            mov di,0x2
0000024F  03F8              add di,ax
00000251  8A04              mov al,[si]
00000253  46                inc si
00000254  3A4507            cmp al,[di+0x7]
00000257  7214              jc 0x26d
00000259  884507            mov [di+0x7],al
0000025C  897505            mov [di+0x5],si
0000025F  33C0              xor ax,ax
00000261  884502            mov [di+0x2],al
00000264  884501            mov [di+0x1],al
00000267  894503            mov [di+0x3],ax
0000026A  C60501            mov byte [di],0x1
0000026D  C3                ret
0000026E  C745030000        mov word [di+0x3],0x0
00000273  C60500            mov byte [di],0x0
00000276  C6450700          mov byte [di+0x7],0x0
0000027A  32C0              xor al,al
0000027C  C3                ret
0000027D  46                inc si
0000027E  C3                ret
0000027F  83C602            add si,byte +0x2
00000282  C3                ret
00000283  83C603            add si,byte +0x3
00000286  C3                ret
00000287  83C605            add si,byte +0x5
0000028A  C3                ret
0000028B  83C606            add si,byte +0x6
0000028E  C3                ret
0000028F  83C60A            add si,byte +0xa
00000292  C3                ret
00000293  8A4401            mov al,[si+0x1]
00000296  884508            mov [di+0x8],al
00000299  83C602            add si,byte +0x2
0000029C  C3                ret
0000029D  FE4D08            dec byte [di+0x8]
000002A0  7504              jnz 0x2a6
000002A2  83C603            add si,byte +0x3
000002A5  C3                ret
000002A6  8B7401            mov si,[si+0x1]
000002A9  C3                ret
000002AA  C745030000        mov word [di+0x3],0x0
000002AF  8A4401            mov al,[si+0x1]
000002B2  884502            mov [di+0x2],al
000002B5  83C602            add si,byte +0x2
000002B8  32C0              xor al,al
000002BA  C3                ret
000002BB  8A4401            mov al,[si+0x1]
000002BE  88450B            mov [di+0xb],al
000002C1  8A6402            mov ah,[si+0x2]
000002C4  8A4403            mov al,[si+0x3]
000002C7  F7D8              neg ax
000002C9  894509            mov [di+0x9],ax
000002CC  804D0101          or byte [di+0x1],0x1
000002D0  83C604            add si,byte +0x4
000002D3  C3                ret
000002D4  806501FE          and byte [di+0x1],0xfe
000002D8  46                inc si
000002D9  C3                ret
000002DA  57                push di
000002DB  56                push si
000002DC  8A4401            mov al,[si+0x1]
000002DF  3C02              cmp al,0x2
000002E1  7E02              jng 0x2e5
000002E3  B002              mov al,0x2
000002E5  32E4              xor ah,ah
000002E7  BA1100            mov dx,0x11
000002EA  F7E2              mul dx
000002EC  BF0200            mov di,0x2
000002EF  03F8              add di,ax
000002F1  E87AFF            call 0x26e
000002F4  5E                pop si
000002F5  5F                pop di
000002F6  83C602            add si,byte +0x2
000002F9  C3                ret
000002FA  8A6401            mov ah,[si+0x1]
000002FD  32C0              xor al,al
000002FF  89450D            mov [di+0xd],ax
00000302  8A4402            mov al,[si+0x2]
00000305  88450C            mov [di+0xc],al
00000308  8A4403            mov al,[si+0x3]
0000030B  884502            mov [di+0x2],al
0000030E  804D0102          or byte [di+0x1],0x2
00000312  83C604            add si,byte +0x4
00000315  32C0              xor al,al
00000317  C3                ret
00000318  8A4401            mov al,[si+0x1]
0000031B  884502            mov [di+0x2],al
0000031E  83C602            add si,byte +0x2
00000321  804D0104          or byte [di+0x1],0x4
00000325  C74503FFFF        mov word [di+0x3],0xffff
0000032A  32C0              xor al,al
0000032C  C3                ret
0000032D  93                xchg ax,bx
0000032E  029D027F          add bl,[di+0x7f02]
00000332  027F02            add bh,[bx+0x2]
00000335  A6                cmpsb
00000336  0283027D          add al,[bp+di+0x7d02]
0000033A  027F02            add bh,[bx+0x2]
0000033D  6E                outsb
0000033E  02AA0283          add ch,[bp+si-0x7cfe]
00000342  0283027F          add al,[bp+di+0x7f02]
00000346  028B027F          add cl,[bp+di+0x7f02]
0000034A  027D02            add bh,[di+0x2]
0000034D  7F02              jg 0x351
0000034F  BB02D4            mov bx,0xd402
00000352  027F02            add bh,[bx+0x2]
00000355  7D02              jnl 0x359
00000357  8702              xchg ax,[bp+si]
00000359  7D02              jnl 0x35d
0000035B  7D02              jnl 0x35f
0000035D  7D02              jnl 0x361
0000035F  7D02              jnl 0x363
00000361  7F02              jg 0x365
00000363  7D02              jnl 0x367
00000365  7F02              jg 0x369
00000367  7F02              jg 0x36b
00000369  7F02              jg 0x36d
0000036B  7D02              jnl 0x36f
0000036D  AA                stosb
0000036E  027F02            add bh,[bx+0x2]
00000371  7D02              jnl 0x375
00000373  7D02              jnl 0x377
00000375  7F02              jg 0x379
00000377  7D02              jnl 0x37b
00000379  7F02              jg 0x37d
0000037B  7D02              jnl 0x37f
0000037D  7D02              jnl 0x381
0000037F  7F02              jg 0x383
00000381  7D02              jnl 0x385
00000383  7F02              jg 0x387
00000385  830283            add word [bp+si],byte -0x7d
00000388  027F02            add bh,[bx+0x2]
0000038B  7F02              jg 0x38f
0000038D  7F02              jg 0x391
0000038F  7D02              jnl 0x393
00000391  7D02              jnl 0x395
00000393  DA02              fiadd dword [bp+si]
00000395  83027D            add word [bp+si],byte +0x7d
00000398  027D02            add bh,[di+0x2]
0000039B  7D02              jnl 0x39f
0000039D  7D02              jnl 0x3a1
0000039F  7D02              jnl 0x3a3
000003A1  7D02              jnl 0x3a5
000003A3  7D02              jnl 0x3a7
000003A5  7F02              jg 0x3a9
000003A7  7F02              jg 0x3ab
000003A9  7D02              jnl 0x3ad
000003AB  83027D            add word [bp+si],byte +0x7d
000003AE  028F027F          add cl,[bx+0x7f02]
000003B2  027D02            add bh,[di+0x2]
000003B5  830283            add word [bp+si],byte -0x7d
000003B8  0283027D          add al,[bp+di+0x7d02]
000003BC  027D02            add bh,[di+0x2]
000003BF  83027D            add word [bp+si],byte +0x7d
000003C2  027D02            add bh,[di+0x2]
000003C5  7D02              jnl 0x3c9
000003C7  7D02              jnl 0x3cb
000003C9  FA                cli
000003CA  0218              add bl,[bx+si]
000003CC  037504            add si,[di+0x4]
000003CF  81048C04          add word [si],0x48c
000003D3  97                xchg ax,di
000003D4  04A8              add al,0xa8
000003D6  04B9              add al,0xb9
000003D8  04CA              add al,0xca
000003DA  04DB              add al,0xdb
000003DC  04EA              add al,0xea
000003DE  04F9              add al,0xf9
000003E0  0404              add al,0x4
000003E2  051305            add ax,0x513
000003E5  1E                push ds
000003E6  052D05            add ax,0x52d
000003E9  4B                dec bx
000003EA  055605            add ax,0x556
000003ED  61                popa
000003EE  056C05            add ax,0x56c
000003F1  7705              ja 0x3f8
000003F3  8605              xchg al,[di]
000003F5  91                xchg ax,cx
000003F6  059A05            add ax,0x59a
000003F9  A5                movsw
000003FA  05B005            add ax,0x5b0
000003FD  CE                into
000003FE  05D905            add ax,0x5d9
00000401  E405              in al,0x5
00000403  F5                cmc
00000404  050006            add ax,0x600
00000407  0B061606          or ax,[0x616]
0000040B  21062C06          and [0x62c],ax
0000040F  3B064E06          cmp ax,[0x64e]
00000413  61                popa
00000414  06                push es
00000415  7406              jz 0x41d
00000417  84069406          test [0x694],al
0000041B  9D                popf
0000041C  06                push es
0000041D  A4                movsb
0000041E  06                push es
0000041F  BD06C8            mov bp,0xc806
00000422  06                push es
00000423  D306DE06          rol word [0x6de],cl
00000427  E906F8            jmp 0xfc30
0000042A  06                push es
0000042B  07                pop es
0000042C  07                pop es
0000042D  16                push ss
0000042E  07                pop es
0000042F  250737            and ax,0x3707
00000432  07                pop es
00000433  42                inc dx
00000434  07                pop es
00000435  4D                dec bp
00000436  07                pop es
00000437  5A                pop dx
00000438  07                pop es
00000439  6707              pop es
0000043B  7207              jc 0x444
0000043D  7E04              jng 0x443
0000043F  7E04              jng 0x445
00000441  7E04              jng 0x447
00000443  8907              mov [bx],ax
00000445  98                cbw
00000446  07                pop es
00000447  A907B4            test ax,0xb407
0000044A  07                pop es
0000044B  C507              lds ax,[bx]
0000044D  8907              mov [bx],ax
0000044F  D6                salc
00000450  07                pop es
00000451  DF07              fild word [bx]
00000453  EF                out dx,ax
00000454  07                pop es
00000455  E807EF            call 0xf35f
00000458  07                pop es
00000459  F8                clc
0000045A  07                pop es
0000045B  0308              add cx,[bx+si]
0000045D  1A08              sbb cl,[bx+si]
0000045F  2E081A            or [cs:bp+si],bl
00000462  084608            or [bp+0x8],al
00000465  5A                pop dx
00000466  084E06            or [bp+0x6],cl
00000469  830871            or word [bx+si],byte +0x71
0000046C  0837              or [bx],dh
0000046E  07                pop es
0000046F  42                inc dx
00000470  07                pop es
00000471  6707              pop es
00000473  95                xchg ax,bp
00000474  0800              or [bx+si],al
00000476  FFB301B3          push word [bp+di-0x4cff]
0000047A  02B30088          add dh,[bp+di-0x7800]
0000047E  0000              add [bx+si],al
00000480  8800              mov [bx+si],al
00000482  00AB0883          add [bp+di-0x7cf8],ch
00000486  01900640          add [bx+si+0x4006],dx
0000048A  10880000          adc [bx+si+0x0],cl
0000048E  AB                stosw
0000048F  08830190          or [bp+di-0x6fff],al
00000493  06                push es
00000494  7004              jo 0x49a
00000496  8800              mov [bx+si],al
00000498  00AB0883          add [bp+di-0x7cf8],ch
0000049C  01900660          add [bx+si+0x6006],dx
000004A0  085D08            or [di+0x8],bl
000004A3  51                push cx
000004A4  085314            or [bp+di+0x14],dl
000004A7  8800              mov [bx+si],al
000004A9  00AB0883          add [bp+di-0x7cf8],ch
000004AD  01900666          add [bx+si+0x6606],dx
000004B1  086008            or [bx+si+0x8],ah
000004B4  6308              arpl [bx+si],cx
000004B6  671488            adc al,0x88
000004B9  0000              add [bx+si],al
000004BB  AB                stosw
000004BC  08830190          or [bp+di-0x6fff],al
000004C0  06                push es
000004C1  60                pusha
000004C2  086108            or [bx+di+0x8],ah
000004C5  6208              bound cx,[bx+si]
000004C7  54                push sp
000004C8  1488              adc al,0x88
000004CA  0000              add [bx+si],al
000004CC  AB                stosw
000004CD  08830190          or [bp+di-0x6fff],al
000004D1  06                push es
000004D2  60                pusha
000004D3  085708            or [bx+0x8],dl
000004D6  60                pusha
000004D7  085914            or [bx+di+0x14],bl
000004DA  8800              mov [bx+si],al
000004DC  00AB0883          add [bp+di-0x7cf8],ch
000004E0  01900050          add [bx+si+0x5000],dx
000004E4  1460              adc al,0x60
000004E6  1456              adc al,0x56
000004E8  1488              adc al,0x88
000004EA  0000              add [bx+si],al
000004EC  AB                stosw
000004ED  00830190          add [bp+di-0x6fff],al
000004F1  08917F00          or [bx+di+0x7f],dl
000004F5  0460              add al,0x60
000004F7  32880000          xor cl,[bx+si+0x0]
000004FB  AB                stosw
000004FC  08830190          or [bp+di-0x6fff],al
00000500  03400C            add ax,[bx+si+0xc]
00000503  8800              mov [bx+si],al
00000505  00AB0883          add [bp+di-0x7cf8],ch
00000509  01900860          add [bx+si+0x6008],dx
0000050D  0E                push cs
0000050E  58                pop ax
0000050F  0E                push cs
00000510  641488            fs adc al,0x88
00000513  0000              add [bx+si],al
00000515  AB                stosw
00000516  0483              add al,0x83
00000518  01900640          add [bx+si+0x4006],dx
0000051C  1488              adc al,0x88
0000051E  0000              add [bx+si],al
00000520  AB                stosw
00000521  08830190          or [bp+di-0x6fff],al
00000525  08480E            or [bx+si+0xe],cl
00000528  52                push dx
00000529  0E                push cs
0000052A  5A                pop dx
0000052B  1488              adc al,0x88
0000052D  0000              add [bx+si],al
0000052F  AB                stosw
00000530  02830190          add al,[bp+di-0x6fff]
00000534  104808            adc [bx+si+0x8],cl
00000537  8903              mov [bp+di],ax
00000539  48                dec ax
0000053A  08890348          or [bx+di+0x4803],cl
0000053E  08890391          or [bx+di-0x6efd],cl
00000542  050001            add ax,0x100
00000545  40                inc ax
00000546  30925014          xor [bp+si+0x1450],dl
0000054A  8801              mov [bx+di],al
0000054C  00AB0283          add [bp+di-0x7cfe],ch
00000550  01900658          add [bx+si+0x5806],dx
00000554  0A880200          or cl,[bx+si+0x2]
00000558  AB                stosw
00000559  02830190          add al,[bp+di-0x6fff]
0000055D  024002            add al,[bx+si+0x2]
00000560  8800              mov [bx+si],al
00000562  00AB0883          add [bp+di-0x7cf8],ch
00000566  01900058          add [bx+si+0x5800],dx
0000056A  0488              add al,0x88
0000056C  0000              add [bx+si],al
0000056E  AB                stosw
0000056F  08830190          or [bp+di-0x6fff],al
00000573  06                push es
00000574  54                push sp
00000575  08880200          or [bx+si+0x2],cl
00000579  AB                stosw
0000057A  004004            add [bx+si+0x4],al
0000057D  91                xchg ax,cx
0000057E  0000              add [bx+si],al
00000580  14CE              adc al,0xce
00000582  2832              sub [bp+si],dh
00000584  FF880000          dec word [bx+si+0x0]
00000588  91                xchg ax,cx
00000589  00FF              add bh,bh
0000058B  C0CE50            ror dh,byte 0x50
0000058E  2828              sub [bx+si],ch
00000590  8801              mov [bx+di],al
00000592  00830190          add [bp+di-0x6fff],al
00000596  BC1406            mov sp,0x614
00000599  8800              mov [bx+si],al
0000059B  009100FF          add [bx+di-0x100],dl
0000059F  A0CE28            mov al,[0x28ce]
000005A2  2828              sub [bx+si],ch
000005A4  8800              mov [bx+si],al
000005A6  00AB0C83          add [bp+di-0x7cf4],ch
000005AA  01900260          add [bx+si+0x6002],dx
000005AE  02880000          add cl,[bx+si+0x0]
000005B2  AB                stosw
000005B3  0C83              or al,0x83
000005B5  01900750          add [bx+si+0x5007],dx
000005B9  0E                push cs
000005BA  52                push dx
000005BB  0E                push cs
000005BC  54                push sp
000005BD  0E                push cs
000005BE  57                push di
000005BF  0E                push cs
000005C0  60                pusha
000005C1  0E                push cs
000005C2  640E              fs push cs
000005C4  680E71            push word 0x710e
000005C7  0E                push cs
000005C8  A00184            mov al,[0x8401]
000005CB  B80588            mov ax,0x8805
000005CE  0100              add [bx+si],ax
000005D0  AB                stosw
000005D1  02830190          add al,[bp+di-0x6fff]
000005D5  06                push es
000005D6  48                dec ax
000005D7  1488              adc al,0x88
000005D9  0000              add [bx+si],al
000005DB  AB                stosw
000005DC  02830190          add al,[bp+di-0x6fff]
000005E0  06                push es
000005E1  40                inc ax
000005E2  1488              adc al,0x88
000005E4  0000              add [bx+si],al
000005E6  AB                stosw
000005E7  08830190          or [bp+di-0x6fff],al
000005EB  819101FFFF36      adc word [bx+di-0xff],0x36ff
000005F1  1484              adc al,0x84
000005F3  F0050100          lock add ax,0x1
000005F7  AB                stosw
000005F8  02830190          add al,[bp+di-0x6fff]
000005FC  086410            or [si+0x10],ah
000005FF  8801              mov [bx+di],al
00000601  00AB0283          add [bp+di-0x7cfe],ch
00000605  01900860          add [bx+si+0x6008],dx
00000609  10880100          adc [bx+si+0x1],cl
0000060D  AB                stosw
0000060E  02830190          add al,[bp+di-0x6fff]
00000612  085810            or [bx+si+0x10],bl
00000615  8801              mov [bx+di],al
00000617  00AB0283          add [bp+di-0x7cfe],ch
0000061B  01900854          add [bx+si+0x5408],dx
0000061F  10880100          adc [bx+si+0x1],cl
00000623  AB                stosw
00000624  02830190          add al,[bp+di-0x6fff]
00000628  084810            or [bx+si+0x10],cl
0000062B  8801              mov [bx+di],al
0000062D  00AB0283          add [bp+di-0x7cfe],ch
00000631  01900960          add [bx+si+0x6009],dx
00000635  0A620A            or ah,[bp+si+0xa]
00000638  640A880200        or cl,[fs:bx+si+0x2]
0000063D  AB                stosw
0000063E  02830190          add al,[bp+di-0x6fff]
00000642  025803            add bl,[bx+si+0x3]
00000645  A02055            mov al,[0x5520]
00000648  03A02084          add sp,[bx+si-0x7be0]
0000064C  43                inc bx
0000064D  06                push es
0000064E  0200              add al,[bx+si]
00000650  AB                stosw
00000651  02830190          add al,[bp+di-0x6fff]
00000655  025803            add bl,[bx+si+0x3]
00000658  A01455            mov al,[0x5514]
0000065B  03A01484          add sp,[bx+si-0x7bec]
0000065F  56                push si
00000660  06                push es
00000661  0200              add al,[bx+si]
00000663  AB                stosw
00000664  02830190          add al,[bp+di-0x6fff]
00000668  025803            add bl,[bx+si+0x3]
0000066B  A00E55            mov al,[0x550e]
0000066E  03A00E84          add sp,[bx+si-0x7bf2]
00000672  69060000AB00      imul ax,[0x0],word 0xab
00000678  830190            add word [bx+di],byte -0x70
0000067B  0491              add al,0x91
0000067D  7F00              jg 0x67f
0000067F  0154FF            add [si-0x1],dx
00000682  92                xchg ax,dx
00000683  8800              mov [bx+si],al
00000685  00AB0883          add [bp+di-0x7cf8],ch
00000689  01900691          add [bx+si-0x6efa],dx
0000068D  FF                db 0xff
0000068E  FF                db 0xff
0000068F  F8                clc
00000690  60                pusha
00000691  1492              adc al,0x92
00000693  8800              mov [bx+si],al
00000695  07                pop es
00000696  9E                sahf
00000697  0ACE              or cl,dh
00000699  2828              sub [bx+si],ch
0000069B  1488              adc al,0x88
0000069D  0000              add [bx+si],al
0000069F  CE                into
000006A0  1428              adc al,0x28
000006A2  0C88              or al,0x88
000006A4  0000              add [bx+si],al
000006A6  AB                stosw
000006A7  08830190          or [bp+di-0x6fff],al
000006AB  06                push es
000006AC  58                pop ax
000006AD  105B10            adc [bp+di+0x10],bl
000006B0  6210              bound dx,[bx+si]
000006B2  7010              jo 0x6c4
000006B4  8903              mov [bp+di],ax
000006B6  7010              jo 0x6c8
000006B8  8903              mov [bp+di],ax
000006BA  7010              jo 0x6cc
000006BC  8800              mov [bx+si],al
000006BE  00AB0883          add [bp+di-0x7cf8],ch
000006C2  01900658          add [bx+si+0x5806],dx
000006C6  1488              adc al,0x88
000006C8  0000              add [bx+si],al
000006CA  AB                stosw
000006CB  08830190          or [bp+di-0x6fff],al
000006CF  06                push es
000006D0  50                push ax
000006D1  1488              adc al,0x88
000006D3  0000              add [bx+si],al
000006D5  AB                stosw
000006D6  08830190          or [bp+di-0x6fff],al
000006DA  086014            or [bx+si+0x14],ah
000006DD  8800              mov [bx+si],al
000006DF  00AB0883          add [bp+di-0x7cf8],ch
000006E3  01900858          add [bx+si+0x5808],dx
000006E7  1488              adc al,0x88
000006E9  0000              add [bx+si],al
000006EB  AB                stosw
000006EC  08830190          or [bp+di-0x6fff],al
000006F0  0491              add al,0x91
000006F2  7FFF              jg 0x6f3
000006F4  FE                db 0xfe
000006F5  641488            fs adc al,0x88
000006F8  0000              add [bx+si],al
000006FA  AB                stosw
000006FB  08830190          or [bp+di-0x6fff],al
000006FF  0491              add al,0x91
00000701  7F00              jg 0x703
00000703  016014            add [bx+si+0x14],sp
00000706  8801              mov [bx+di],al
00000708  00AB0883          add [bp+di-0x7cf8],ch
0000070C  01904B91          add [bx+si-0x6eb5],dx
00000710  40                inc ax
00000711  FF                db 0xff
00000712  FE                db 0xfe
00000713  6450              fs push ax
00000715  8801              mov [bx+di],al
00000717  00AB0883          add [bp+di-0x7cf8],ch
0000071B  01904B91          add [bx+si-0x6eb5],dx
0000071F  40                inc ax
00000720  0002              add [bp+si],al
00000722  5A                pop dx
00000723  50                push ax
00000724  8800              mov [bx+si],al
00000726  00AB0883          add [bp+di-0x7cf8],ch
0000072A  01900691          add [bx+si-0x6efa],dx
0000072E  FF00              inc word [bx+si]
00000730  2F                das
00000731  58                pop ax
00000732  1492              adc al,0x92
00000734  6410880000        adc [fs:bx+si+0x0],cl
00000739  AB                stosw
0000073A  08830190          or [bp+di-0x6fff],al
0000073E  06                push es
0000073F  58                pop ax
00000740  1E                push ds
00000741  8800              mov [bx+si],al
00000743  00AB0883          add [bp+di-0x7cf8],ch
00000747  01900648          add [bx+si+0x4806],dx
0000074B  1E                push ds
0000074C  8800              mov [bx+si],al
0000074E  00AB0883          add [bp+di-0x7cf8],ch
00000752  01900858          add [bx+si+0x5808],dx
00000756  04A0              add al,0xa0
00000758  02880000          add cl,[bx+si+0x0]
0000075C  AB                stosw
0000075D  08830190          or [bp+di-0x6fff],al
00000761  085004            or [bx+si+0x4],dl
00000764  A00288            mov al,[0x8802]
00000767  0100              add [bx+si],ax
00000769  AB                stosw
0000076A  08830190          or [bp+di-0x6fff],al
0000076E  0440              add al,0x40
00000770  06                push es
00000771  8802              mov [bp+si],al
00000773  00AB1683          add [bp+di-0x7cea],ch
00000777  01904591          add [bx+si-0x6ebb],dx
0000077B  39FF              cmp di,di
0000077D  FF21              jmp [bx+di]
0000077F  8091390001        adc byte [bx+di+0x39],0x1
00000784  2080847A          and [bx+si+0x7a84],al
00000788  07                pop es
00000789  0000              add [bx+si],al
0000078B  1008              adc [bx+si],cl
0000078D  91                xchg ax,cx
0000078E  FF                db 0xff
0000078F  FFA0CE3C          jmp [bx+si+0x3cce]
00000793  2828              sub [bx+si],ch
00000795  06                push es
00000796  28880000          sub [bx+si+0x0],cl
0000079A  AB                stosw
0000079B  08830190          or [bp+di-0x6fff],al
0000079F  0491              add al,0x91
000007A1  30FF              xor bh,bh
000007A3  FF4880            dec word [bx+si-0x80]
000007A6  50                push ax
000007A7  08880004          or [bx+si+0x400],cl
000007AB  90                nop
000007AC  2191FFFF          and [bx+di-0x1],dx
000007B0  FA                cli
000007B1  1824              sbb [si],ah
000007B3  8801              mov [bx+di],al
000007B5  00AB0883          add [bp+di-0x7cf8],ch
000007B9  01901991          add [bx+si-0x6ee7],dx
000007BD  90                nop
000007BE  0001              add [bx+di],al
000007C0  60                pusha
000007C1  2084C007          and [si+0x7c0],al
000007C5  0000              add [bx+si],al
000007C7  AB                stosw
000007C8  08900A16          or [bx+si+0x160a],dl
000007CC  08902691          or [bx+si-0x6eda],dl
000007D0  FF                db 0xff
000007D1  FF                db 0xff
000007D2  FF162888          call [0x8828]
000007D6  0004              add [si],al
000007D8  91                xchg ax,cx
000007D9  FF                db 0xff
000007DA  FF                db 0xff
000007DB  FA                cli
000007DC  3024              xor [si],ah
000007DE  8800              mov [bx+si],al
000007E0  0491              add al,0x91
000007E2  FF                db 0xff
000007E3  FF                db 0xff
000007E4  FA                cli
000007E5  262488            es and al,0x88
000007E8  0007              add [bx],al
000007EA  CE                into
000007EB  641424            fs adc al,0x24
000007EE  8800              mov [bx+si],al
000007F0  0491              add al,0x91
000007F2  FF                db 0xff
000007F3  FF                db 0xff
000007F4  FA                cli
000007F5  2824              sub [si],ah
000007F7  8800              mov [bx+si],al
000007F9  0091FFFF          add [bx+di-0x1],dl
000007FD  FF1A              call far [bp+si]
000007FF  20890288          and [bx+di-0x77fe],cl
00000803  0200              add al,[bx+si]
00000805  AB                stosw
00000806  0C83              or al,0x83
00000808  01907991          add [bx+si-0x6e87],dx
0000080C  01FF              add di,di
0000080E  FF20              jmp [bx+si]
00000810  60                pusha
00000811  91                xchg ax,cx
00000812  0100              add [bx+si],ax
00000814  0124              add [si],sp
00000816  60                pusha
00000817  840B              test [bp+di],cl
00000819  0800              or [bx+si],al
0000081B  00AB0883          add [bp+di-0x7cf8],ch
0000081F  01902A91          add [bx+si-0x6ed6],dx
00000823  0000              add [bx+si],al
00000825  14CE              adc al,0xce
00000827  2832              sub [bp+si],dh
00000829  50                push ax
0000082A  8422              test [bp+si],ah
0000082C  08880000          or [bx+si+0x0],cl
00000830  AB                stosw
00000831  08830190          or [bp+di-0x6fff],al
00000835  199170FF          sbb [bx+di-0x90],dx
00000839  FF7550            push word [di+0x50]
0000083C  91                xchg ax,cx
0000083D  7000              jo 0x83f
0000083F  016550            add [di+0x50],sp
00000842  813608880100      xor word [0x8808],0x1
00000848  830190            add word [bx+di],byte -0x70
0000084B  BE95FF            mov si,0xff95
0000084E  06                push es
0000084F  0300              add ax,[bx+si]
00000851  243A              and al,0x3a
00000853  91                xchg ax,cx
00000854  FF                db 0xff
00000855  FF                db 0xff
00000856  F9                stc
00000857  A02588            mov al,[0x8825]
0000085A  0000              add [bx+si],al
0000085C  AB                stosw
0000085D  08830190          or [bp+di-0x6fff],al
00000861  1E                push ds
00000862  0102              add [bp+si],ax
00000864  90                nop
00000865  3691              ss xchg ax,cx
00000867  FF                db 0xff
00000868  FFD7              call di
0000086A  7830              js 0x89c
0000086C  AB                stosw
0000086D  FF900088          call [bx+si-0x7800]
00000871  0000              add [bx+si],al
00000873  AB                stosw
00000874  08830190          or [bp+di-0x6fff],al
00000878  06                push es
00000879  91                xchg ax,cx
0000087A  50                push ax
0000087B  FF                db 0xff
0000087C  FF6040            jmp [bx+si+0x40]
0000087F  92                xchg ax,dx
00000880  40                inc ax
00000881  29880000          sub [bx+si+0x0],cx
00000885  AB                stosw
00000886  08830190          or [bp+di-0x6fff],al
0000088A  06                push es
0000088B  91                xchg ax,cx
0000088C  50                push ax
0000088D  0030              add [bx+si],dh
0000088F  58                pop ax
00000890  40                inc ax
00000891  92                xchg ax,dx
00000892  6318              arpl [bx+si],bx
00000894  8800              mov [bx+si],al
00000896  00CF              add bh,cl
00000898  FF880000          dec word [bx+si+0x0]
0000089C  0000              add [bx+si],al
0000089E  0000              add [bx+si],al
