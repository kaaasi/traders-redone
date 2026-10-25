; segment 0B67  file offset 0xEC70  size 0x380
00000000  5D                pop bp
00000001  CB                retf

; ---- 0B67:0002 ----
00000002  55                push bp
00000003  8BEC              mov bp,sp
00000005  83EC06            sub sp,byte +0x6
00000008  56                push si
00000009  57                push di
0000000A  833E0E0963        cmp word [0x90e],byte +0x63
0000000F  7521              jnz 0x32
00000011  B80100            mov ax,0x1
00000014  33D2              xor dx,dx
00000016  50                push ax
00000017  52                push dx
00000018  9AC6026C18        call 0x186c:0x2c6
0000001D  59                pop cx
0000001E  59                pop cx
0000001F  A3A82B            mov [0x2ba8],ax
00000022  8916AA2B          mov [0x2baa],dx
00000026  A1A82B            mov ax,[0x2ba8]
00000029  0B06AA2B          or ax,[0x2baa]
0000002D  7503              jnz 0x32
0000002F  E91E01            jmp 0x150
00000032  FF360E09          push word [0x90e]
00000036  9AF604CD23        call 0x23cd:0x4f6
0000003B  59                pop cx
0000003C  A10E09            mov ax,[0x90e]
0000003F  8946FA            mov [bp-0x6],ax
00000042  B90400            mov cx,0x4
00000045  BB5B01            mov bx,0x15b
00000048  2E8B07            mov ax,[cs:bx]
0000004B  3B46FA            cmp ax,[bp-0x6]
0000004E  7407              jz 0x57
00000050  43                inc bx
00000051  43                inc bx
00000052  E2F4              loop 0x48
00000054  E99700            jmp 0xee
00000057  2EFF6708          jmp [cs:bx+0x8]
0000005B  C746FC1040        mov word [bp-0x4],0x4010
00000060  C746FE0000        mov word [bp-0x2],0x0
00000065  C706A02B0000      mov word [0x2ba0],0x0
0000006B  C706A22B00A0      mov word [0x2ba2],0xa000
00000071  C706A42B0000      mov word [0x2ba4],0x0
00000077  C706A62B00A2      mov word [0x2ba6],0xa200
0000007D  EB6F              jmp short 0xee
0000007F  C746FC0005        mov word [bp-0x4],0x500
00000084  C746FE0000        mov word [bp-0x2],0x0
00000089  C706A02B0000      mov word [0x2ba0],0x0
0000008F  C706A22B00A0      mov word [0x2ba2],0xa000
00000095  C706A42B0000      mov word [0x2ba4],0x0
0000009B  C706A62B00A4      mov word [0x2ba6],0xa400
000000A1  EB4B              jmp short 0xee
000000A3  C746FC4E01        mov word [bp-0x4],0x14e
000000A8  C746FE0000        mov word [bp-0x2],0x0
000000AD  C706A02B0000      mov word [0x2ba0],0x0
000000B3  C706A22B0000      mov word [0x2ba2],0x0
000000B9  C706A42B0100      mov word [0x2ba4],0x1
000000BF  C706A62B0000      mov word [0x2ba6],0x0
000000C5  EB27              jmp short 0xee
000000C7  C746FC4E01        mov word [bp-0x4],0x14e
000000CC  C746FE0000        mov word [bp-0x2],0x0
000000D1  8B36AA2B          mov si,[0x2baa]
000000D5  46                inc si
000000D6  33FF              xor di,di
000000D8  893EA02B          mov [0x2ba0],di
000000DC  8936A22B          mov [0x2ba2],si
000000E0  8BC6              mov ax,si
000000E2  050008            add ax,0x800
000000E5  893EA42B          mov [0x2ba4],di
000000E9  A3A62B            mov [0x2ba6],ax
000000EC  EB00              jmp short 0xee
000000EE  FF76FE            push word [bp-0x2]
000000F1  FF76FC            push word [bp-0x4]
000000F4  9AC6026C18        call 0x186c:0x2c6
000000F9  59                pop cx
000000FA  59                pop cx
000000FB  A3AC2B            mov [0x2bac],ax
000000FE  8916AE2B          mov [0x2bae],dx
00000102  A1AC2B            mov ax,[0x2bac]
00000105  0B06AE2B          or ax,[0x2bae]
00000109  7502              jnz 0x10d
0000010B  EB43              jmp short 0x150
0000010D  9AD401670B        call 0xb67:0x1d4
00000112  33C0              xor ax,ax
00000114  50                push ax
00000115  9AC305CD23        call 0x23cd:0x5c3
0000011A  59                pop cx
0000011B  B8C800            mov ax,0xc8
0000011E  50                push ax
0000011F  B84001            mov ax,0x140
00000122  50                push ax
00000123  33C0              xor ax,ax
00000125  50                push ax
00000126  33C0              xor ax,ax
00000128  50                push ax
00000129  9A9B06CD23        call 0x23cd:0x69b
0000012E  83C408            add sp,byte +0x8
00000131  9A0602670B        call 0xb67:0x206
00000136  B8C800            mov ax,0xc8
00000139  50                push ax
0000013A  B84001            mov ax,0x140
0000013D  50                push ax
0000013E  33C0              xor ax,ax
00000140  50                push ax
00000141  33C0              xor ax,ax
00000143  50                push ax
00000144  9A9B06CD23        call 0x23cd:0x69b
00000149  83C408            add sp,byte +0x8
0000014C  33C0              xor ax,ax
0000014E  EB05              jmp short 0x155
00000150  B80100            mov ax,0x1
00000153  EB00              jmp short 0x155
00000155  5F                pop di
00000156  5E                pop si
00000157  8BE5              mov sp,bp
00000159  5D                pop bp
0000015A  CB                retf
0000015B  0000              add [bx+si],al
0000015D  0100              add [bx+si],ax
0000015F  0200              add al,[bx+si]
00000161  6300              arpl [bx+si],ax
00000163  5B                pop bx
00000164  007F00            add [bx+0x0],bh
00000167  A300C7            mov [0xc700],ax
0000016A  00558B            add [di-0x75],dl
0000016D  EC                in al,dx
0000016E  833E380200        cmp word [0x238],byte +0x0
00000173  7507              jnz 0x17c
00000175  9AB501670B        call 0xb67:0x1b5
0000017A  EB05              jmp short 0x181
0000017C  9A9601670B        call 0xb67:0x196
00000181  833E3A0200        cmp word [0x23a],byte +0x0
00000186  7407              jz 0x18f
00000188  9A0602670B        call 0xb67:0x206
0000018D  EB05              jmp short 0x194
0000018F  9AD401670B        call 0xb67:0x1d4
00000194  5D                pop bp
00000195  CB                retf

; ---- 0B67:0196 ----
00000196  55                push bp
00000197  8BEC              mov bp,sp
00000199  C70638020000      mov word [0x238],0x0
0000019F  FF36A22B          push word [0x2ba2]
000001A3  FF36A02B          push word [0x2ba0]
000001A7  9A7505CD23        call 0x23cd:0x575
000001AC  59                pop cx
000001AD  59                pop cx
000001AE  9AD300730D        call 0xd73:0xd3
000001B3  5D                pop bp
000001B4  CB                retf

; ---- 0B67:01B5 ----
000001B5  55                push bp
000001B6  8BEC              mov bp,sp
000001B8  C70638020100      mov word [0x238],0x1
000001BE  FF36A62B          push word [0x2ba6]
000001C2  FF36A42B          push word [0x2ba4]
000001C6  9A7505CD23        call 0x23cd:0x575
000001CB  59                pop cx
000001CC  59                pop cx
000001CD  9AD300730D        call 0xd73:0xd3
000001D2  5D                pop bp
000001D3  CB                retf

; ---- 0B67:01D4 ----
000001D4  55                push bp
000001D5  8BEC              mov bp,sp
000001D7  C7063A020000      mov word [0x23a],0x0
000001DD  833E380200        cmp word [0x238],byte +0x0
000001E2  7511              jnz 0x1f5
000001E4  FF36A22B          push word [0x2ba2]
000001E8  FF36A02B          push word [0x2ba0]
000001EC  9A7005CD23        call 0x23cd:0x570
000001F1  59                pop cx
000001F2  59                pop cx
000001F3  EB0F              jmp short 0x204
000001F5  FF36A62B          push word [0x2ba6]
000001F9  FF36A42B          push word [0x2ba4]
000001FD  9A7005CD23        call 0x23cd:0x570
00000202  59                pop cx
00000203  59                pop cx
00000204  5D                pop bp
00000205  CB                retf

; ---- 0B67:0206 ----
00000206  55                push bp
00000207  8BEC              mov bp,sp
00000209  C7063A020100      mov word [0x23a],0x1
0000020F  833E380200        cmp word [0x238],byte +0x0
00000214  7511              jnz 0x227
00000216  FF36A62B          push word [0x2ba6]
0000021A  FF36A42B          push word [0x2ba4]
0000021E  9A7005CD23        call 0x23cd:0x570
00000223  59                pop cx
00000224  59                pop cx
00000225  EB0F              jmp short 0x236
00000227  FF36A22B          push word [0x2ba2]
0000022B  FF36A02B          push word [0x2ba0]
0000022F  9A7005CD23        call 0x23cd:0x570
00000234  59                pop cx
00000235  59                pop cx
00000236  5D                pop bp
00000237  CB                retf

; ---- 0B67:0238 ----
00000238  55                push bp
00000239  8BEC              mov bp,sp
0000023B  837E0C00          cmp word [bp+0xc],byte +0x0
0000023F  750A              jnz 0x24b
00000241  837E0A01          cmp word [bp+0xa],byte +0x1
00000245  7504              jnz 0x24b
00000247  0E                push cs
00000248  E84BFF            call 0x196
0000024B  837E0C00          cmp word [bp+0xc],byte +0x0
0000024F  750A              jnz 0x25b
00000251  837E0A02          cmp word [bp+0xa],byte +0x2
00000255  7504              jnz 0x25b
00000257  0E                push cs
00000258  E85AFF            call 0x1b5
0000025B  837E0800          cmp word [bp+0x8],byte +0x0
0000025F  7506              jnz 0x267
00000261  837E0601          cmp word [bp+0x6],byte +0x1
00000265  740C              jz 0x273
00000267  837E0800          cmp word [bp+0x8],byte +0x0
0000026B  7520              jnz 0x28d
0000026D  837E0602          cmp word [bp+0x6],byte +0x2
00000271  751A              jnz 0x28d
00000273  8B4608            mov ax,[bp+0x8]
00000276  8B5606            mov dx,[bp+0x6]
00000279  3B460C            cmp ax,[bp+0xc]
0000027C  750B              jnz 0x289
0000027E  3B560A            cmp dx,[bp+0xa]
00000281  7506              jnz 0x289
00000283  0E                push cs
00000284  E84DFF            call 0x1d4
00000287  EB04              jmp short 0x28d
00000289  0E                push cs
0000028A  E879FF            call 0x206
0000028D  5D                pop bp
0000028E  CB                retf

; ---- 0B67:028F ----
0000028F  55                push bp
00000290  8BEC              mov bp,sp
00000292  9AD300730D        call 0xd73:0xd3
00000297  5D                pop bp
00000298  CB                retf

; ---- 0B67:0299 ----
00000299  55                push bp
0000029A  8BEC              mov bp,sp
0000029C  83EC08            sub sp,byte +0x8
0000029F  56                push si
000002A0  837E0601          cmp word [bp+0x6],byte +0x1
000002A4  751C              jnz 0x2c2
000002A6  A1A22B            mov ax,[0x2ba2]
000002A9  8B16A02B          mov dx,[0x2ba0]
000002AD  8956FC            mov [bp-0x4],dx
000002B0  8946FE            mov [bp-0x2],ax
000002B3  A1A62B            mov ax,[0x2ba6]
000002B6  8B16A42B          mov dx,[0x2ba4]
000002BA  8956F8            mov [bp-0x8],dx
000002BD  8946FA            mov [bp-0x6],ax
000002C0  EB1A              jmp short 0x2dc
000002C2  A1A22B            mov ax,[0x2ba2]
000002C5  8B16A02B          mov dx,[0x2ba0]
000002C9  8956F8            mov [bp-0x8],dx
000002CC  8946FA            mov [bp-0x6],ax
000002CF  A1A62B            mov ax,[0x2ba6]
000002D2  8B16A42B          mov dx,[0x2ba4]
000002D6  8956FC            mov [bp-0x4],dx
000002D9  8946FE            mov [bp-0x2],ax
000002DC  8B360E09          mov si,[0x90e]
000002E0  B90400            mov cx,0x4
000002E3  BB7203            mov bx,0x372
000002E6  2E3937            cmp [cs:bx],si
000002E9  7406              jz 0x2f1
000002EB  43                inc bx
000002EC  43                inc bx
000002ED  E2F7              loop 0x2e6
000002EF  EB7C              jmp short 0x36d
000002F1  2EFF6708          jmp [cs:bx+0x8]
000002F5  B80100            mov ax,0x1
000002F8  50                push ax
000002F9  B8401F            mov ax,0x1f40
000002FC  50                push ax
000002FD  FF76FA            push word [bp-0x6]
00000300  FF76F8            push word [bp-0x8]
00000303  FF76FE            push word [bp-0x2]
00000306  FF76FC            push word [bp-0x4]
00000309  9AB906CD23        call 0x23cd:0x6b9
0000030E  83C40C            add sp,byte +0xc
00000311  EB5A              jmp short 0x36d
00000313  B80100            mov ax,0x1
00000316  50                push ax
00000317  B8803E            mov ax,0x3e80
0000031A  50                push ax
0000031B  FF76FA            push word [bp-0x6]
0000031E  FF76F8            push word [bp-0x8]
00000321  FF76FE            push word [bp-0x2]
00000324  FF76FC            push word [bp-0x4]
00000327  9AB906CD23        call 0x23cd:0x6b9
0000032C  83C40C            add sp,byte +0xc
0000032F  EB3C              jmp short 0x36d
00000331  B8C800            mov ax,0xc8
00000334  50                push ax
00000335  B8A000            mov ax,0xa0
00000338  50                push ax
00000339  FF76FA            push word [bp-0x6]
0000033C  FF76F8            push word [bp-0x8]
0000033F  FF76FE            push word [bp-0x2]
00000342  FF76FC            push word [bp-0x4]
00000345  9AB906CD23        call 0x23cd:0x6b9
0000034A  83C40C            add sp,byte +0xc
0000034D  EB1E              jmp short 0x36d
0000034F  B8C800            mov ax,0xc8
00000352  50                push ax
00000353  B8A000            mov ax,0xa0
00000356  50                push ax
00000357  FF76FA            push word [bp-0x6]
0000035A  FF76F8            push word [bp-0x8]
0000035D  FF76FE            push word [bp-0x2]
00000360  FF76FC            push word [bp-0x4]
00000363  9AB906CD23        call 0x23cd:0x6b9
00000368  83C40C            add sp,byte +0xc
0000036B  EB00              jmp short 0x36d
0000036D  5E                pop si
0000036E  8BE5              mov sp,bp
00000370  5D                pop bp
00000371  CB                retf
00000372  0000              add [bx+si],al
00000374  0100              add [bx+si],ax
00000376  0200              add al,[bx+si]
00000378  6300              arpl [bx+si],ax
0000037A  F5                cmc
0000037B  0213              add dl,[bp+di]
0000037D  0331              add si,[bx+di]
0000037F  03                db 0x03
