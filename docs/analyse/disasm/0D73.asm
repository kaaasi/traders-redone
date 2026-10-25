; segment 0D73  file offset 0x10D30  size 0x8A0
00000000  0496              add al,0x96
00000002  0A83C40A          or al,[bp+di+0xac4]
00000006  5E                pop si
00000007  5D                pop bp
00000008  CB                retf

; ---- 0D73:0009 ----
00000009  55                push bp
0000000A  8BEC              mov bp,sp
0000000C  83EC18            sub sp,byte +0x18
0000000F  C646E910          mov byte [bp-0x17],0x10
00000013  C646E802          mov byte [bp-0x18],0x2
00000017  8B4606            mov ax,[bp+0x6]
0000001A  8946EE            mov [bp-0x12],ax
0000001D  8B4608            mov ax,[bp+0x8]
00000020  8946F8            mov [bp-0x8],ax
00000023  16                push ss
00000024  8D46F8            lea ax,[bp-0x8]
00000027  50                push ax
00000028  16                push ss
00000029  8D46E8            lea ax,[bp-0x18]
0000002C  50                push ax
0000002D  16                push ss
0000002E  8D46E8            lea ax,[bp-0x18]
00000031  50                push ax
00000032  B81000            mov ax,0x10
00000035  50                push ax
00000036  9AA03C0000        call 0x0:0x3ca0
0000003B  83C40E            add sp,byte +0xe
0000003E  8BE5              mov sp,bp
00000040  5D                pop bp
00000041  CB                retf
00000042  55                push bp
00000043  8BEC              mov bp,sp
00000045  1E                push ds
00000046  B84C06            mov ax,0x64c
00000049  50                push ax
0000004A  0E                push cs
0000004B  E8BBFF            call 0x9
0000004E  59                pop cx
0000004F  59                pop cx
00000050  5D                pop bp
00000051  CB                retf

; ---- 0D73:0052 ----
00000052  55                push bp
00000053  8BEC              mov bp,sp
00000055  833E0E0900        cmp word [0x90e],byte +0x0
0000005A  7502              jnz 0x5e
0000005C  EB0E              jmp short 0x6c
0000005E  C4560A            les dx,[bp+0xa]
00000061  8B4E08            mov cx,[bp+0x8]
00000064  8B5E06            mov bx,[bp+0x6]
00000067  9A2E04C627        call 0x27c6:0x42e
0000006C  5D                pop bp
0000006D  CB                retf
0000006E  55                push bp
0000006F  8BEC              mov bp,sp
00000071  83EC18            sub sp,byte +0x18
00000074  C646E910          mov byte [bp-0x17],0x10
00000078  C646E815          mov byte [bp-0x18],0x15
0000007C  8B4606            mov ax,[bp+0x6]
0000007F  8946EA            mov [bp-0x16],ax
00000082  16                push ss
00000083  8D46F8            lea ax,[bp-0x8]
00000086  50                push ax
00000087  16                push ss
00000088  8D46E8            lea ax,[bp-0x18]
0000008B  50                push ax
0000008C  16                push ss
0000008D  8D46E8            lea ax,[bp-0x18]
00000090  50                push ax
00000091  B81000            mov ax,0x10
00000094  50                push ax
00000095  9AA03C0000        call 0x0:0x3ca0
0000009A  83C40E            add sp,byte +0xe
0000009D  C45E08            les bx,[bp+0x8]
000000A0  8A46EF            mov al,[bp-0x11]
000000A3  268807            mov [es:bx],al
000000A6  C45E08            les bx,[bp+0x8]
000000A9  8A46ED            mov al,[bp-0x13]
000000AC  26884701          mov [es:bx+0x1],al
000000B0  C45E08            les bx,[bp+0x8]
000000B3  8A46EC            mov al,[bp-0x14]
000000B6  26884702          mov [es:bx+0x2],al
000000BA  8BE5              mov sp,bp
000000BC  5D                pop bp
000000BD  CB                retf
000000BE  55                push bp
000000BF  8BEC              mov bp,sp
000000C1  EB00              jmp short 0xc3
000000C3  B8DA03            mov ax,0x3da
000000C6  50                push ax
000000C7  9A633C0000        call 0x0:0x3c63
000000CC  59                pop cx
000000CD  A808              test al,0x8
000000CF  74F2              jz 0xc3
000000D1  5D                pop bp
000000D2  CB                retf

; ---- 0D73:00D3 ----
000000D3  55                push bp
000000D4  8BEC              mov bp,sp
000000D6  EB00              jmp short 0xd8
000000D8  B8DA03            mov ax,0x3da
000000DB  50                push ax
000000DC  9A633C0000        call 0x0:0x3c63
000000E1  59                pop cx
000000E2  A808              test al,0x8
000000E4  75F2              jnz 0xd8
000000E6  EB00              jmp short 0xe8
000000E8  B8DA03            mov ax,0x3da
000000EB  50                push ax
000000EC  9A633C0000        call 0x0:0x3c63
000000F1  59                pop cx
000000F2  A808              test al,0x8
000000F4  74F2              jz 0xe8
000000F6  5D                pop bp
000000F7  CB                retf

; ---- 0D73:00F8 ----
000000F8  55                push bp
000000F9  8BEC              mov bp,sp
000000FB  83EC10            sub sp,byte +0x10
000000FE  C746F00000        mov word [bp-0x10],0x0
00000103  16                push ss
00000104  8D46F0            lea ax,[bp-0x10]
00000107  50                push ax
00000108  16                push ss
00000109  8D46F0            lea ax,[bp-0x10]
0000010C  50                push ax
0000010D  B83300            mov ax,0x33
00000110  50                push ax
00000111  9A6E3C0000        call 0x0:0x3c6e
00000116  83C40A            add sp,byte +0xa
00000119  8B46F0            mov ax,[bp-0x10]
0000011C  A3E62B            mov [0x2be6],ax
0000011F  C746F00B00        mov word [bp-0x10],0xb
00000124  16                push ss
00000125  8D46F0            lea ax,[bp-0x10]
00000128  50                push ax
00000129  16                push ss
0000012A  8D46F0            lea ax,[bp-0x10]
0000012D  50                push ax
0000012E  B83300            mov ax,0x33
00000131  50                push ax
00000132  9A6E3C0000        call 0x0:0x3c6e
00000137  83C40A            add sp,byte +0xa
0000013A  8BE5              mov sp,bp
0000013C  5D                pop bp
0000013D  CB                retf
0000013E  55                push bp
0000013F  8BEC              mov bp,sp
00000141  83EC12            sub sp,byte +0x12
00000144  56                push si
00000145  57                push di
00000146  C7064A060000      mov word [0x64a],0x0
0000014C  833EE62B00        cmp word [0x2be6],byte +0x0
00000151  7474              jz 0x1c7
00000153  C746EE0300        mov word [bp-0x12],0x3
00000158  16                push ss
00000159  8D46EE            lea ax,[bp-0x12]
0000015C  50                push ax
0000015D  16                push ss
0000015E  8D46EE            lea ax,[bp-0x12]
00000161  50                push ax
00000162  B83300            mov ax,0x33
00000165  50                push ax
00000166  9A6E3C0000        call 0x0:0x3c6e
0000016B  83C40A            add sp,byte +0xa
0000016E  F746F00100        test word [bp-0x10],0x1
00000173  7423              jz 0x198
00000175  16                push ss
00000176  8D46EE            lea ax,[bp-0x12]
00000179  50                push ax
0000017A  16                push ss
0000017B  8D46EE            lea ax,[bp-0x12]
0000017E  50                push ax
0000017F  B83300            mov ax,0x33
00000182  50                push ax
00000183  9A6E3C0000        call 0x0:0x3c6e
00000188  83C40A            add sp,byte +0xa
0000018B  F746F00100        test word [bp-0x10],0x1
00000190  75E3              jnz 0x175
00000192  B80D00            mov ax,0xd
00000195  E98100            jmp 0x219
00000198  F746F00200        test word [bp-0x10],0x2
0000019D  7428              jz 0x1c7
0000019F  16                push ss
000001A0  8D46EE            lea ax,[bp-0x12]
000001A3  50                push ax
000001A4  16                push ss
000001A5  8D46EE            lea ax,[bp-0x12]
000001A8  50                push ax
000001A9  B83300            mov ax,0x33
000001AC  50                push ax
000001AD  9A6E3C0000        call 0x0:0x3c6e
000001B2  83C40A            add sp,byte +0xa
000001B5  F746F00200        test word [bp-0x10],0x2
000001BA  75E3              jnz 0x19f
000001BC  C7064A060100      mov word [0x64a],0x1
000001C2  B81B00            mov ax,0x1b
000001C5  EB52              jmp short 0x219
000001C7  833EEA2B00        cmp word [0x2bea],byte +0x0
000001CC  7E1A              jng 0x1e8
000001CE  16                push ss
000001CF  8D46FE            lea ax,[bp-0x2]
000001D2  50                push ax
000001D3  B80100            mov ax,0x1
000001D6  50                push ax
000001D7  9A6802C627        call 0x27c6:0x268
000001DC  83C406            add sp,byte +0x6
000001DF  0BC0              or ax,ax
000001E1  7405              jz 0x1e8
000001E3  B80D00            mov ax,0xd
000001E6  EB31              jmp short 0x219
000001E8  B80100            mov ax,0x1
000001EB  50                push ax
000001EC  9A823A0000        call 0x0:0x3a82
000001F1  59                pop cx
000001F2  0BC0              or ax,ax
000001F4  7505              jnz 0x1fb
000001F6  B8FFFF            mov ax,0xffff
000001F9  EB1E              jmp short 0x219
000001FB  33C0              xor ax,ax
000001FD  50                push ax
000001FE  9A823A0000        call 0x0:0x3a82
00000203  59                pop cx
00000204  8BF8              mov di,ax
00000206  8BC7              mov ax,di
00000208  25FF00            and ax,0xff
0000020B  8BF0              mov si,ax
0000020D  0BF6              or si,si
0000020F  7404              jz 0x215
00000211  8BC6              mov ax,si
00000213  EB04              jmp short 0x219
00000215  8BC7              mov ax,di
00000217  EB00              jmp short 0x219
00000219  5F                pop di
0000021A  5E                pop si
0000021B  8BE5              mov sp,bp
0000021D  5D                pop bp
0000021E  CB                retf
0000021F  55                push bp
00000220  8BEC              mov bp,sp
00000222  56                push si
00000223  57                push di
00000224  B80100            mov ax,0x1
00000227  50                push ax
00000228  9A823A0000        call 0x0:0x3a82
0000022D  59                pop cx
0000022E  0BC0              or ax,ax
00000230  7505              jnz 0x237
00000232  B8FFFF            mov ax,0xffff
00000235  EB1E              jmp short 0x255
00000237  33C0              xor ax,ax
00000239  50                push ax
0000023A  9A823A0000        call 0x0:0x3a82
0000023F  59                pop cx
00000240  8BF8              mov di,ax
00000242  8BC7              mov ax,di
00000244  25FF00            and ax,0xff
00000247  8BF0              mov si,ax
00000249  0BF6              or si,si
0000024B  7404              jz 0x251
0000024D  8BC6              mov ax,si
0000024F  EB04              jmp short 0x255
00000251  8BC7              mov ax,di
00000253  EB00              jmp short 0x255
00000255  5F                pop di
00000256  5E                pop si
00000257  5D                pop bp
00000258  CB                retf

; ---- 0D73:0259 ----
00000259  55                push bp
0000025A  8BEC              mov bp,sp
0000025C  56                push si
0000025D  33F6              xor si,si
0000025F  EB05              jmp short 0x266
00000261  0E                push cs
00000262  E86EFE            call 0xd3
00000265  46                inc si
00000266  8B4606            mov ax,[bp+0x6]
00000269  050F00            add ax,0xf
0000026C  BB1000            mov bx,0x10
0000026F  99                cwd
00000270  F7FB              idiv bx
00000272  3BC6              cmp ax,si
00000274  7FEB              jg 0x261
00000276  5E                pop si
00000277  5D                pop bp
00000278  CB                retf
00000279  55                push bp
0000027A  8BEC              mov bp,sp
0000027C  B89600            mov ax,0x96
0000027F  50                push ax
00000280  9A09420000        call 0x0:0x4209
00000285  59                pop cx
00000286  B89600            mov ax,0x96
00000289  50                push ax
0000028A  0E                push cs
0000028B  E8CBFF            call 0x259
0000028E  59                pop cx
0000028F  9A35420000        call 0x0:0x4235
00000294  5D                pop bp
00000295  CB                retf
00000296  55                push bp
00000297  8BEC              mov bp,sp
00000299  B82C01            mov ax,0x12c
0000029C  50                push ax
0000029D  9A09420000        call 0x0:0x4209
000002A2  59                pop cx
000002A3  B83200            mov ax,0x32
000002A6  50                push ax
000002A7  0E                push cs
000002A8  E8AEFF            call 0x259
000002AB  59                pop cx
000002AC  9A35420000        call 0x0:0x4235
000002B1  5D                pop bp
000002B2  CB                retf
000002B3  55                push bp
000002B4  8BEC              mov bp,sp
000002B6  A14A06            mov ax,[0x64a]
000002B9  EB00              jmp short 0x2bb
000002BB  5D                pop bp
000002BC  CB                retf
000002BD  55                push bp
000002BE  8BEC              mov bp,sp
000002C0  83EC34            sub sp,byte +0x34
000002C3  56                push si
000002C4  57                push di
000002C5  833E0E0900        cmp word [0x90e],byte +0x0
000002CA  7517              jnz 0x2e3
000002CC  FF760C            push word [bp+0xc]
000002CF  FF760A            push word [bp+0xa]
000002D2  FF7608            push word [bp+0x8]
000002D5  FF7606            push word [bp+0x6]
000002D8  9A9603730D        call 0xd73:0x396
000002DD  83C408            add sp,byte +0x8
000002E0  E9AD00            jmp 0x390
000002E3  8B460C            mov ax,[bp+0xc]
000002E6  050900            add ax,0x9
000002E9  BB0A00            mov bx,0xa
000002EC  99                cwd
000002ED  F7FB              idiv bx
000002EF  8946FC            mov [bp-0x4],ax
000002F2  B80A00            mov ax,0xa
000002F5  99                cwd
000002F6  F77E0C            idiv word [bp+0xc]
000002F9  8946FE            mov [bp-0x2],ax
000002FC  837EFE00          cmp word [bp-0x2],byte +0x0
00000300  7505              jnz 0x307
00000302  C746FE0100        mov word [bp-0x2],0x1
00000307  33FF              xor di,di
00000309  EB5F              jmp short 0x36a
0000030B  33F6              xor si,si
0000030D  EB32              jmp short 0x341
0000030F  837E0A01          cmp word [bp+0xa],byte +0x1
00000313  7514              jnz 0x329
00000315  C45E06            les bx,[bp+0x6]
00000318  268A00            mov al,[es:bx+si]
0000031B  98                cbw
0000031C  F7EF              imul di
0000031E  BB4000            mov bx,0x40
00000321  99                cwd
00000322  F7FB              idiv bx
00000324  8842CC            mov [bp+si-0x34],al
00000327  EB17              jmp short 0x340
00000329  C45E06            les bx,[bp+0x6]
0000032C  268A00            mov al,[es:bx+si]
0000032F  98                cbw
00000330  BA4000            mov dx,0x40
00000333  2BD7              sub dx,di
00000335  F7EA              imul dx
00000337  BB4000            mov bx,0x40
0000033A  99                cwd
0000033B  F7FB              idiv bx
0000033D  8842CC            mov [bp+si-0x34],al
00000340  46                inc si
00000341  83FE30            cmp si,byte +0x30
00000344  7CC9              jl 0x30f
00000346  33F6              xor si,si
00000348  EB05              jmp short 0x34f
0000034A  0E                push cs
0000034B  E885FD            call 0xd3
0000034E  46                inc si
0000034F  3B76FC            cmp si,[bp-0x4]
00000352  7CF6              jl 0x34a
00000354  16                push ss
00000355  8D46CC            lea ax,[bp-0x34]
00000358  50                push ax
00000359  B81000            mov ax,0x10
0000035C  50                push ax
0000035D  33C0              xor ax,ax
0000035F  50                push ax
00000360  0E                push cs
00000361  E8EEFC            call 0x52
00000364  83C408            add sp,byte +0x8
00000367  037EFE            add di,[bp-0x2]
0000036A  83FF40            cmp di,byte +0x40
0000036D  7E9C              jng 0x30b
0000036F  837E0A00          cmp word [bp+0xa],byte +0x0
00000373  7507              jnz 0x37c
00000375  9AB403730D        call 0xd73:0x3b4
0000037A  EB14              jmp short 0x390
0000037C  FF7608            push word [bp+0x8]
0000037F  FF7606            push word [bp+0x6]
00000382  B81000            mov ax,0x10
00000385  50                push ax
00000386  33C0              xor ax,ax
00000388  50                push ax
00000389  0E                push cs
0000038A  E8C5FC            call 0x52
0000038D  83C408            add sp,byte +0x8
00000390  5F                pop di
00000391  5E                pop si
00000392  8BE5              mov sp,bp
00000394  5D                pop bp
00000395  CB                retf

; ---- 0D73:0396 ----
00000396  55                push bp
00000397  8BEC              mov bp,sp
00000399  837E0A00          cmp word [bp+0xa],byte +0x0
0000039D  7507              jnz 0x3a6
0000039F  9AB403730D        call 0xd73:0x3b4
000003A4  EB0C              jmp short 0x3b2
000003A6  FF7608            push word [bp+0x8]
000003A9  FF7606            push word [bp+0x6]
000003AC  0E                push cs
000003AD  E859FC            call 0x9
000003B0  59                pop cx
000003B1  59                pop cx
000003B2  5D                pop bp
000003B3  CB                retf

; ---- 0D73:03B4 ----
000003B4  55                push bp
000003B5  8BEC              mov bp,sp
000003B7  83EC30            sub sp,byte +0x30
000003BA  56                push si
000003BB  33F6              xor si,si
000003BD  EB05              jmp short 0x3c4
000003BF  C642D000          mov byte [bp+si-0x30],0x0
000003C3  46                inc si
000003C4  83FE30            cmp si,byte +0x30
000003C7  7CF6              jl 0x3bf
000003C9  0E                push cs
000003CA  E806FD            call 0xd3
000003CD  833E0E0901        cmp word [0x90e],byte +0x1
000003D2  7515              jnz 0x3e9
000003D4  16                push ss
000003D5  8D46D0            lea ax,[bp-0x30]
000003D8  50                push ax
000003D9  B81000            mov ax,0x10
000003DC  50                push ax
000003DD  33C0              xor ax,ax
000003DF  50                push ax
000003E0  0E                push cs
000003E1  E86EFC            call 0x52
000003E4  83C408            add sp,byte +0x8
000003E7  EB0B              jmp short 0x3f4
000003E9  16                push ss
000003EA  8D46D0            lea ax,[bp-0x30]
000003ED  50                push ax
000003EE  0E                push cs
000003EF  E817FC            call 0x9
000003F2  59                pop cx
000003F3  59                pop cx
000003F4  5E                pop si
000003F5  8BE5              mov sp,bp
000003F7  5D                pop bp
000003F8  CB                retf

; ---- 0D73:03F9 ----
000003F9  55                push bp
000003FA  8BEC              mov bp,sp
000003FC  C706EA2B0000      mov word [0x2bea],0x0
00000402  5D                pop bp
00000403  CB                retf
00000404  55                push bp
00000405  8BEC              mov bp,sp
00000407  A1EA2B            mov ax,[0x2bea]
0000040A  EB00              jmp short 0x40c
0000040C  5D                pop bp
0000040D  CB                retf

; ---- 0D73:040E ----
0000040E  55                push bp
0000040F  8BEC              mov bp,sp
00000411  56                push si
00000412  8B7606            mov si,[bp+0x6]
00000415  F7C60100          test si,0x1
00000419  740F              jz 0x42a
0000041B  F70648060100      test word [0x648],0x1
00000421  7505              jnz 0x428
00000423  9A0000C627        call 0x27c6:0x0
00000428  EB0D              jmp short 0x437
0000042A  F70648060100      test word [0x648],0x1
00000430  7405              jz 0x437
00000432  9A2700C627        call 0x27c6:0x27
00000437  F7C60200          test si,0x2
0000043B  7433              jz 0x470
0000043D  833E5D0600        cmp word [0x65d],byte +0x0
00000442  740B              jz 0x44f
00000444  9A7605C627        call 0x27c6:0x576
00000449  C7065D060000      mov word [0x65d],0x0
0000044F  F70648060200      test word [0x648],0x2
00000455  7505              jnz 0x45c
00000457  9A7A03C627        call 0x27c6:0x37a
0000045C  833E5D0600        cmp word [0x65d],byte +0x0
00000461  750B              jnz 0x46e
00000463  9A5105C627        call 0x27c6:0x551
00000468  C7065D060100      mov word [0x65d],0x1
0000046E  EB1F              jmp short 0x48f
00000470  833E5D0600        cmp word [0x65d],byte +0x0
00000475  740B              jz 0x482
00000477  9A7605C627        call 0x27c6:0x576
0000047C  C7065D060000      mov word [0x65d],0x0
00000482  F70648060200      test word [0x648],0x2
00000488  7405              jz 0x48f
0000048A  9AC303C627        call 0x27c6:0x3c3
0000048F  89364806          mov [0x648],si
00000493  9AAC00C627        call 0x27c6:0xac
00000498  5E                pop si
00000499  5D                pop bp
0000049A  CB                retf

; ---- 0D73:049B ----
0000049B  55                push bp
0000049C  8BEC              mov bp,sp
0000049E  B80300            mov ax,0x3
000004A1  50                push ax
000004A2  0E                push cs
000004A3  E868FF            call 0x40e
000004A6  59                pop cx
000004A7  5D                pop bp
000004A8  CB                retf

; ---- 0D73:04A9 ----
000004A9  55                push bp
000004AA  8BEC              mov bp,sp
000004AC  33C0              xor ax,ax
000004AE  50                push ax
000004AF  0E                push cs
000004B0  E85BFF            call 0x40e
000004B3  59                pop cx
000004B4  5D                pop bp
000004B5  CB                retf
000004B6  55                push bp
000004B7  8BEC              mov bp,sp
000004B9  33C0              xor ax,ax
000004BB  33D2              xor dx,dx
000004BD  50                push ax
000004BE  52                push dx
000004BF  9A05340000        call 0x0:0x3405
000004C4  59                pop cx
000004C5  59                pop cx
000004C6  EB00              jmp short 0x4c8
000004C8  5D                pop bp
000004C9  CB                retf

; ---- 0D73:04CA ----
000004CA  55                push bp
000004CB  8BEC              mov bp,sp
000004CD  C706E82B8803      mov word [0x2be8],0x388
000004D3  9AE704730D        call 0xd73:0x4e7
000004D8  0BC0              or ax,ax
000004DA  7504              jnz 0x4e0
000004DC  33C0              xor ax,ax
000004DE  EB05              jmp short 0x4e5
000004E0  B80100            mov ax,0x1
000004E3  EB00              jmp short 0x4e5
000004E5  5D                pop bp
000004E6  CB                retf

; ---- 0D73:04E7 ----
000004E7  55                push bp
000004E8  8BEC              mov bp,sp
000004EA  4C                dec sp
000004EB  4C                dec sp
000004EC  56                push si
000004ED  57                push di
000004EE  B86000            mov ax,0x60
000004F1  50                push ax
000004F2  B80400            mov ax,0x4
000004F5  50                push ax
000004F6  9A1105C627        call 0x27c6:0x511
000004FB  59                pop cx
000004FC  59                pop cx
000004FD  B88000            mov ax,0x80
00000500  50                push ax
00000501  B80400            mov ax,0x4
00000504  50                push ax
00000505  9A1105C627        call 0x27c6:0x511
0000050A  59                pop cx
0000050B  59                pop cx
0000050C  FF36E82B          push word [0x2be8]
00000510  9A633C0000        call 0x0:0x3c63
00000515  59                pop cx
00000516  B400              mov ah,0x0
00000518  8946FE            mov [bp-0x2],ax
0000051B  B8FF00            mov ax,0xff
0000051E  50                push ax
0000051F  B80200            mov ax,0x2
00000522  50                push ax
00000523  9A1105C627        call 0x27c6:0x511
00000528  59                pop cx
00000529  59                pop cx
0000052A  B82100            mov ax,0x21
0000052D  50                push ax
0000052E  B80400            mov ax,0x4
00000531  50                push ax
00000532  9A1105C627        call 0x27c6:0x511
00000537  59                pop cx
00000538  59                pop cx
00000539  33F6              xor si,si
0000053B  EB0B              jmp short 0x548
0000053D  FF36E82B          push word [0x2be8]
00000541  9A633C0000        call 0x0:0x3c63
00000546  59                pop cx
00000547  46                inc si
00000548  81FEC800          cmp si,0xc8
0000054C  72EF              jc 0x53d
0000054E  FF36E82B          push word [0x2be8]
00000552  9A633C0000        call 0x0:0x3c63
00000557  59                pop cx
00000558  B400              mov ah,0x0
0000055A  8BF8              mov di,ax
0000055C  B86000            mov ax,0x60
0000055F  50                push ax
00000560  B80400            mov ax,0x4
00000563  50                push ax
00000564  9A1105C627        call 0x27c6:0x511
00000569  59                pop cx
0000056A  59                pop cx
0000056B  B88000            mov ax,0x80
0000056E  50                push ax
0000056F  B80400            mov ax,0x4
00000572  50                push ax
00000573  9A1105C627        call 0x27c6:0x511
00000578  59                pop cx
00000579  59                pop cx
0000057A  F746FEE000        test word [bp-0x2],0xe0
0000057F  750F              jnz 0x590
00000581  8BC7              mov ax,di
00000583  25E000            and ax,0xe0
00000586  3DC000            cmp ax,0xc0
00000589  7505              jnz 0x590
0000058B  B80100            mov ax,0x1
0000058E  EB02              jmp short 0x592
00000590  33C0              xor ax,ax
00000592  EB00              jmp short 0x594
00000594  5F                pop di
00000595  5E                pop si
00000596  8BE5              mov sp,bp
00000598  5D                pop bp
00000599  CB                retf

; ---- 0D73:059A ----
0000059A  55                push bp
0000059B  8BEC              mov bp,sp
0000059D  83EC20            sub sp,byte +0x20
000005A0  56                push si
000005A1  57                push di
000005A2  C746FC0000        mov word [bp-0x4],0x0
000005A7  C746FE0000        mov word [bp-0x2],0x0
000005AC  8B460E            mov ax,[bp+0xe]
000005AF  8B560C            mov dx,[bp+0xc]
000005B2  8956F8            mov [bp-0x8],dx
000005B5  8946FA            mov [bp-0x6],ax
000005B8  837E0EFF          cmp word [bp+0xe],byte -0x1
000005BC  7506              jnz 0x5c4
000005BE  837E0CFF          cmp word [bp+0xc],byte -0x1
000005C2  740F              jz 0x5d3
000005C4  837EFA00          cmp word [bp-0x6],byte +0x0
000005C8  720E              jc 0x5d8
000005CA  7707              ja 0x5d3
000005CC  817EF800FA        cmp word [bp-0x8],0xfa00
000005D1  7605              jna 0x5d8
000005D3  BE00FA            mov si,0xfa00
000005D6  EB03              jmp short 0x5db
000005D8  8B76F8            mov si,[bp-0x8]
000005DB  0BF6              or si,si
000005DD  7502              jnz 0x5e1
000005DF  EB5F              jmp short 0x640
000005E1  8B460A            mov ax,[bp+0xa]
000005E4  8946F6            mov [bp-0xa],ax
000005E7  8B4608            mov ax,[bp+0x8]
000005EA  8946E6            mov [bp-0x1a],ax
000005ED  8B4606            mov ax,[bp+0x6]
000005F0  8946E2            mov [bp-0x1e],ax
000005F3  8976E4            mov [bp-0x1c],si
000005F6  C646E13F          mov byte [bp-0x1f],0x3f
000005FA  16                push ss
000005FB  8D46F0            lea ax,[bp-0x10]
000005FE  50                push ax
000005FF  16                push ss
00000600  8D46E0            lea ax,[bp-0x20]
00000603  50                push ax
00000604  16                push ss
00000605  8D46E0            lea ax,[bp-0x20]
00000608  50                push ax
00000609  9A683D0000        call 0x0:0x3d68
0000060E  83C40C            add sp,byte +0xc
00000611  8BF8              mov di,ax
00000613  837EEC00          cmp word [bp-0x14],byte +0x0
00000617  7402              jz 0x61b
00000619  EB25              jmp short 0x640
0000061B  017EFC            add [bp-0x4],di
0000061E  8356FE00          adc word [bp-0x2],byte +0x0
00000622  297EF8            sub [bp-0x8],di
00000625  835EFA00          sbb word [bp-0x6],byte +0x0
00000629  3BFE              cmp di,si
0000062B  7402              jz 0x62f
0000062D  EB11              jmp short 0x640
0000062F  8BDF              mov bx,di
00000631  33C9              xor cx,cx
00000633  8CD2              mov dx,ss
00000635  8D4608            lea ax,[bp+0x8]
00000638  9A79500000        call 0x0:0x5079
0000063D  E978FF            jmp 0x5b8
00000640  8B56FE            mov dx,[bp-0x2]
00000643  8B46FC            mov ax,[bp-0x4]
00000646  EB00              jmp short 0x648
00000648  5F                pop di
00000649  5E                pop si
0000064A  8BE5              mov sp,bp
0000064C  5D                pop bp
0000064D  CB                retf

; ---- 0D73:064E ----
0000064E  55                push bp
0000064F  8BEC              mov bp,sp
00000651  83EC20            sub sp,byte +0x20
00000654  56                push si
00000655  57                push di
00000656  C746FC0000        mov word [bp-0x4],0x0
0000065B  C746FE0000        mov word [bp-0x2],0x0
00000660  8B460E            mov ax,[bp+0xe]
00000663  8B560C            mov dx,[bp+0xc]
00000666  8956F8            mov [bp-0x8],dx
00000669  8946FA            mov [bp-0x6],ax
0000066C  837EFA00          cmp word [bp-0x6],byte +0x0
00000670  720E              jc 0x680
00000672  7707              ja 0x67b
00000674  817EF800FA        cmp word [bp-0x8],0xfa00
00000679  7605              jna 0x680
0000067B  BE00FA            mov si,0xfa00
0000067E  EB03              jmp short 0x683
00000680  8B76F8            mov si,[bp-0x8]
00000683  0BF6              or si,si
00000685  7502              jnz 0x689
00000687  EB5E              jmp short 0x6e7
00000689  8B460A            mov ax,[bp+0xa]
0000068C  8946F6            mov [bp-0xa],ax
0000068F  8B4608            mov ax,[bp+0x8]
00000692  8946E6            mov [bp-0x1a],ax
00000695  8B4606            mov ax,[bp+0x6]
00000698  8946E2            mov [bp-0x1e],ax
0000069B  8976E4            mov [bp-0x1c],si
0000069E  C646E140          mov byte [bp-0x1f],0x40
000006A2  16                push ss
000006A3  8D46F0            lea ax,[bp-0x10]
000006A6  50                push ax
000006A7  16                push ss
000006A8  8D46E0            lea ax,[bp-0x20]
000006AB  50                push ax
000006AC  16                push ss
000006AD  8D46E0            lea ax,[bp-0x20]
000006B0  50                push ax
000006B1  9A683D0000        call 0x0:0x3d68
000006B6  83C40C            add sp,byte +0xc
000006B9  8BF8              mov di,ax
000006BB  837EEC00          cmp word [bp-0x14],byte +0x0
000006BF  7402              jz 0x6c3
000006C1  EB24              jmp short 0x6e7
000006C3  017EFC            add [bp-0x4],di
000006C6  8356FE00          adc word [bp-0x2],byte +0x0
000006CA  297EF8            sub [bp-0x8],di
000006CD  835EFA00          sbb word [bp-0x6],byte +0x0
000006D1  3BFE              cmp di,si
000006D3  7402              jz 0x6d7
000006D5  EB10              jmp short 0x6e7
000006D7  8BDF              mov bx,di
000006D9  33C9              xor cx,cx
000006DB  8CD2              mov dx,ss
000006DD  8D4608            lea ax,[bp+0x8]
000006E0  9A79500000        call 0x0:0x5079
000006E5  EB85              jmp short 0x66c
000006E7  8B56FE            mov dx,[bp-0x2]
000006EA  8B46FC            mov ax,[bp-0x4]
000006ED  EB00              jmp short 0x6ef
000006EF  5F                pop di
000006F0  5E                pop si
000006F1  8BE5              mov sp,bp
000006F3  5D                pop bp
000006F4  CB                retf

; ---- 0D73:06F5 ----
000006F5  55                push bp
000006F6  8BEC              mov bp,sp
000006F8  B8730D            mov ax,0xd73
000006FB  50                push ax
000006FC  B80907            mov ax,0x709
000006FF  50                push ax
00000700  9AE93B0000        call 0x0:0x3be9
00000705  59                pop cx
00000706  59                pop cx
00000707  5D                pop bp
00000708  CB                retf
00000709  55                push bp
0000070A  8BEC              mov bp,sp
0000070C  33C0              xor ax,ax
0000070E  EB00              jmp short 0x710
00000710  5D                pop bp
00000711  CB                retf

; ---- 0D73:0712 ----
00000712  55                push bp
00000713  8BEC              mov bp,sp
00000715  83EC10            sub sp,byte +0x10
00000718  C7060E09FFFF      mov word [0x90e],0xffff
0000071E  C646F11A          mov byte [bp-0xf],0x1a
00000722  C646F000          mov byte [bp-0x10],0x0
00000726  16                push ss
00000727  8D46F0            lea ax,[bp-0x10]
0000072A  50                push ax
0000072B  16                push ss
0000072C  8D46F0            lea ax,[bp-0x10]
0000072F  50                push ax
00000730  B81000            mov ax,0x10
00000733  50                push ax
00000734  9A6E3C0000        call 0x0:0x3c6e
00000739  83C40A            add sp,byte +0xa
0000073C  807EF01A          cmp byte [bp-0x10],0x1a
00000740  751E              jnz 0x760
00000742  807EF207          cmp byte [bp-0xe],0x7
00000746  7406              jz 0x74e
00000748  807EF208          cmp byte [bp-0xe],0x8
0000074C  7506              jnz 0x754
0000074E  C7060E090100      mov word [0x90e],0x1
00000754  807EF204          cmp byte [bp-0xe],0x4
00000758  7506              jnz 0x760
0000075A  C7060E090000      mov word [0x90e],0x0
00000760  833E0E09FF        cmp word [0x90e],byte -0x1
00000765  7530              jnz 0x797
00000767  C646F210          mov byte [bp-0xe],0x10
0000076B  C646F112          mov byte [bp-0xf],0x12
0000076F  16                push ss
00000770  8D46F0            lea ax,[bp-0x10]
00000773  50                push ax
00000774  16                push ss
00000775  8D46F0            lea ax,[bp-0x10]
00000778  50                push ax
00000779  B81000            mov ax,0x10
0000077C  50                push ax
0000077D  9A6E3C0000        call 0x0:0x3c6e
00000782  83C40A            add sp,byte +0xa
00000785  807EF210          cmp byte [bp-0xe],0x10
00000789  740C              jz 0x797
0000078B  807EF300          cmp byte [bp-0xd],0x0
0000078F  7506              jnz 0x797
00000791  C7060E090000      mov word [0x90e],0x0
00000797  833E0E09FF        cmp word [0x90e],byte -0x1
0000079C  750F              jnz 0x7ad
0000079E  9A1517CD23        call 0x23cd:0x1715
000007A3  0BC0              or ax,ax
000007A5  7406              jz 0x7ad
000007A7  C7060E090200      mov word [0x90e],0x2
000007AD  A10E09            mov ax,[0x90e]
000007B0  EB00              jmp short 0x7b2
000007B2  8BE5              mov sp,bp
000007B4  5D                pop bp
000007B5  CB                retf

; ---- 0D73:07B6 ----
000007B6  55                push bp
000007B7  8BEC              mov bp,sp
000007B9  83EC10            sub sp,byte +0x10
000007BC  C646F100          mov byte [bp-0xf],0x0
000007C0  8A4606            mov al,[bp+0x6]
000007C3  8846F0            mov [bp-0x10],al
000007C6  16                push ss
000007C7  8D46F0            lea ax,[bp-0x10]
000007CA  50                push ax
000007CB  16                push ss
000007CC  8D46F0            lea ax,[bp-0x10]
000007CF  50                push ax
000007D0  B81000            mov ax,0x10
000007D3  50                push ax
000007D4  9A6E3C0000        call 0x0:0x3c6e
000007D9  83C40A            add sp,byte +0xa
000007DC  8BE5              mov sp,bp
000007DE  5D                pop bp
000007DF  CB                retf

; ---- 0D73:07E0 ----
000007E0  55                push bp
000007E1  8BEC              mov bp,sp
000007E3  83EC12            sub sp,byte +0x12
000007E6  56                push si
000007E7  57                push di
000007E8  33F6              xor si,si
000007EA  833EE62B00        cmp word [0x2be6],byte +0x0
000007EF  7505              jnz 0x7f6
000007F1  33C0              xor ax,ax
000007F3  E9B100            jmp 0x8a7
000007F6  C746EE0B00        mov word [bp-0x12],0xb
000007FB  16                push ss
000007FC  8D46EE            lea ax,[bp-0x12]
000007FF  50                push ax
00000800  16                push ss
00000801  8D46EE            lea ax,[bp-0x12]
00000804  50                push ax
00000805  B83300            mov ax,0x33
00000808  50                push ax
00000809  9A6E3C0000        call 0x0:0x3c6e
0000080E  83C40A            add sp,byte +0xa
00000811  8B46F2            mov ax,[bp-0xe]
00000814  99                cwd
00000815  33C2              xor ax,dx
00000817  2BC2              sub ax,dx
00000819  8BF8              mov di,ax
0000081B  8B46F4            mov ax,[bp-0xc]
0000081E  99                cwd
0000081F  33C2              xor ax,dx
00000821  2BC2              sub ax,dx
00000823  8946FE            mov [bp-0x2],ax
00000826  0BFF              or di,di
00000828  7421              jz 0x84b
0000082A  3B7EFE            cmp di,[bp-0x2]
0000082D  7C1C              jl 0x84b
0000082F  837EF201          cmp word [bp-0xe],byte +0x1
00000833  7E09              jng 0x83e
00000835  8BC6              mov ax,si
00000837  0D0100            or ax,0x1
0000083A  8BF0              mov si,ax
0000083C  EB0D              jmp short 0x84b
0000083E  837EF2FF          cmp word [bp-0xe],byte -0x1
00000842  7D07              jnl 0x84b
00000844  8BC6              mov ax,si
00000846  0D0200            or ax,0x2
00000849  8BF0              mov si,ax
0000084B  397EFE            cmp [bp-0x2],di
0000084E  7E1C              jng 0x86c
00000850  837EF401          cmp word [bp-0xc],byte +0x1
00000854  7E09              jng 0x85f
00000856  8BC6              mov ax,si
00000858  0D0800            or ax,0x8
0000085B  8BF0              mov si,ax
0000085D  EB0D              jmp short 0x86c
0000085F  837EF4FF          cmp word [bp-0xc],byte -0x1
00000863  7D07              jnl 0x86c
00000865  8BC6              mov ax,si
00000867  0D0400            or ax,0x4
0000086A  8BF0              mov si,ax
0000086C  C746EE0300        mov word [bp-0x12],0x3
00000871  16                push ss
00000872  8D46EE            lea ax,[bp-0x12]
00000875  50                push ax
00000876  16                push ss
00000877  8D46EE            lea ax,[bp-0x12]
0000087A  50                push ax
0000087B  B83300            mov ax,0x33
0000087E  50                push ax
0000087F  9A6E3C0000        call 0x0:0x3c6e
00000884  83C40A            add sp,byte +0xa
00000887  F746F00100        test word [bp-0x10],0x1
0000088C  7407              jz 0x895
0000088E  8BC6              mov ax,si
00000890  0D1000            or ax,0x10
00000893  8BF0              mov si,ax
00000895  F746F00200        test word [bp-0x10],0x2
0000089A  7407              jz 0x8a3
0000089C  8BC6              mov ax,si
0000089E  0D                db 0x0d
0000089F  20                db 0x20
