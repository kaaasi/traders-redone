; segment 16D6  file offset 0x1A360  size 0xA10
00000000  5C                pop sp
00000001  45                inc bp
00000002  01EB              add bx,bp
00000004  07                pop es
00000005  FEC1              inc cl
00000007  80F903            cmp cl,0x3
0000000A  7E98              jng 0xffa4
0000000C  5D                pop bp
0000000D  CB                retf

; ---- 16D6:000E ----
0000000E  55                push bp
0000000F  8BEC              mov bp,sp
00000011  9ACB062508        call 0x825:0x6cb
00000016  C606AA3700        mov byte [0x37aa],0x0
0000001B  EB41              jmp short 0x5e
0000001D  A0AA37            mov al,[0x37aa]
00000020  98                cbw
00000021  8BD8              mov bx,ax
00000023  80BF400801        cmp byte [bx+0x840],0x1
00000028  7530              jnz 0x5a
0000002A  9A6700D616        call 0x16d6:0x67
0000002F  9ADB05D616        call 0x16d6:0x5db
00000034  A0AA37            mov al,[0x37aa]
00000037  98                cbw
00000038  8BD8              mov bx,ax
0000003A  C6871E3000        mov byte [bx+0x301e],0x0
0000003F  A0AA37            mov al,[0x37aa]
00000042  98                cbw
00000043  BA0C00            mov dx,0xc
00000046  F7EA              imul dx
00000048  8BD8              mov bx,ax
0000004A  8A878A45          mov al,[bx+0x458a]
0000004E  50                push ax
0000004F  A0AA37            mov al,[0x37aa]
00000052  98                cbw
00000053  8BD8              mov bx,ax
00000055  58                pop ax
00000056  88870930          mov [bx+0x3009],al
0000005A  FE06AA37          inc byte [0x37aa]
0000005E  803EAA3704        cmp byte [0x37aa],0x4
00000063  7CB8              jl 0x1d
00000065  5D                pop bp
00000066  CB                retf

; ---- 16D6:0067 ----
00000067  55                push bp
00000068  8BEC              mov bp,sp
0000006A  83EC08            sub sp,byte +0x8
0000006D  56                push si
0000006E  33F6              xor si,si
00000070  EB17              jmp short 0x89
00000072  A0AA37            mov al,[0x37aa]
00000075  98                cbw
00000076  B104              mov cl,0x4
00000078  D3E0              shl ax,cl
0000007A  8BD6              mov dx,si
0000007C  D1E2              shl dx,1
0000007E  03C2              add ax,dx
00000080  8BD8              mov bx,ax
00000082  C78710460000      mov word [bx+0x4610],0x0
00000088  46                inc si
00000089  83FE07            cmp si,byte +0x7
0000008C  7EE4              jng 0x72
0000008E  33F6              xor si,si
00000090  EB11              jmp short 0xa3
00000092  A0AA37            mov al,[0x37aa]
00000095  98                cbw
00000096  BA0300            mov dx,0x3
00000099  F7EA              imul dx
0000009B  8BD8              mov bx,ax
0000009D  C6807E2E00        mov byte [bx+si+0x2e7e],0x0
000000A2  46                inc si
000000A3  83FE02            cmp si,byte +0x2
000000A6  7EEA              jng 0x92
000000A8  A0AA37            mov al,[0x37aa]
000000AB  98                cbw
000000AC  8BD8              mov bx,ax
000000AE  C6871E3DFF        mov byte [bx+0x3d1e],0xff
000000B3  A0AA37            mov al,[0x37aa]
000000B6  98                cbw
000000B7  D1E0              shl ax,1
000000B9  D1E0              shl ax,1
000000BB  8BD8              mov bx,ax
000000BD  8B876C37          mov ax,[bx+0x376c]
000000C1  A36E2E            mov [0x2e6e],ax
000000C4  A0AA37            mov al,[0x37aa]
000000C7  98                cbw
000000C8  D1E0              shl ax,1
000000CA  D1E0              shl ax,1
000000CC  8BD8              mov bx,ax
000000CE  8B876E37          mov ax,[bx+0x376e]
000000D2  A3702E            mov [0x2e70],ax
000000D5  A0AA37            mov al,[0x37aa]
000000D8  98                cbw
000000D9  D1E0              shl ax,1
000000DB  8BD8              mov bx,ax
000000DD  8B87F82E          mov ax,[bx+0x2ef8]
000000E1  A3722E            mov [0x2e72],ax
000000E4  A0AA37            mov al,[0x37aa]
000000E7  98                cbw
000000E8  8BD8              mov bx,ax
000000EA  8A877C37          mov al,[bx+0x377c]
000000EE  B400              mov ah,0x0
000000F0  A3742E            mov [0x2e74],ax
000000F3  A0AA37            mov al,[0x37aa]
000000F6  98                cbw
000000F7  D1E0              shl ax,1
000000F9  8BD8              mov bx,ax
000000FB  8B87EC40          mov ax,[bx+0x40ec]
000000FF  A3762E            mov [0x2e76],ax
00000102  A0AA37            mov al,[0x37aa]
00000105  98                cbw
00000106  D1E0              shl ax,1
00000108  8BD8              mov bx,ax
0000010A  8B87DC33          mov ax,[bx+0x33dc]
0000010E  A3782E            mov [0x2e78],ax
00000111  A0AA37            mov al,[0x37aa]
00000114  98                cbw
00000115  D1E0              shl ax,1
00000117  8BD8              mov bx,ax
00000119  8B871630          mov ax,[bx+0x3016]
0000011D  A37A2E            mov [0x2e7a],ax
00000120  33F6              xor si,si
00000122  EB10              jmp short 0x134
00000124  56                push si
00000125  9A280A681C        call 0x1c68:0xa28
0000012A  59                pop cx
0000012B  8BDE              mov bx,si
0000012D  D1E3              shl bx,1
0000012F  89875C2E          mov [bx+0x2e5c],ax
00000133  46                inc si
00000134  83FE07            cmp si,byte +0x7
00000137  7EEB              jng 0x124
00000139  A0AA37            mov al,[0x37aa]
0000013C  98                cbw
0000013D  D1E0              shl ax,1
0000013F  D1E0              shl ax,1
00000141  8BD8              mov bx,ax
00000143  8B87F64B          mov ax,[bx+0x4bf6]
00000147  8B97F44B          mov dx,[bx+0x4bf4]
0000014B  8956F8            mov [bp-0x8],dx
0000014E  8946FA            mov [bp-0x6],ax
00000151  9A561B0F15        call 0x150f:0x1b56
00000156  A0AA37            mov al,[0x37aa]
00000159  98                cbw
0000015A  8BD8              mov bx,ax
0000015C  80BFE74001        cmp byte [bx+0x40e7],0x1
00000161  7E18              jng 0x17b
00000163  33C0              xor ax,ax
00000165  BA0200            mov dx,0x2
00000168  50                push ax
00000169  52                push dx
0000016A  FF76FA            push word [bp-0x6]
0000016D  FF76F8            push word [bp-0x8]
00000170  9A894F0000        call 0x0:0x4f89
00000175  8946F8            mov [bp-0x8],ax
00000178  8956FA            mov [bp-0x6],dx
0000017B  A0AA37            mov al,[0x37aa]
0000017E  98                cbw
0000017F  8BD8              mov bx,ax
00000181  80BFE74003        cmp byte [bx+0x40e7],0x3
00000186  7E22              jng 0x1aa
00000188  A0AA37            mov al,[0x37aa]
0000018B  98                cbw
0000018C  8BD8              mov bx,ax
0000018E  8A87423D          mov al,[bx+0x3d42]
00000192  B400              mov ah,0x0
00000194  40                inc ax
00000195  50                push ax
00000196  9ABA01FD21        call 0x21fd:0x1ba
0000019B  59                pop cx
0000019C  0BC0              or ax,ax
0000019E  750A              jnz 0x1aa
000001A0  C746F80000        mov word [bp-0x8],0x0
000001A5  C746FA0000        mov word [bp-0x6],0x0
000001AA  A0AA37            mov al,[0x37aa]
000001AD  98                cbw
000001AE  D1E0              shl ax,1
000001B0  D1E0              shl ax,1
000001B2  8BD8              mov bx,ax
000001B4  8B87F64B          mov ax,[bx+0x4bf6]
000001B8  8B97F44B          mov dx,[bx+0x4bf4]
000001BC  2B56F8            sub dx,[bp-0x8]
000001BF  1B46FA            sbb ax,[bp-0x6]
000001C2  8956FC            mov [bp-0x4],dx
000001C5  8946FE            mov [bp-0x2],ax
000001C8  FF76FE            push word [bp-0x2]
000001CB  FF76FC            push word [bp-0x4]
000001CE  9A7C03D616        call 0x16d6:0x37c
000001D3  59                pop cx
000001D4  59                pop cx
000001D5  0146F8            add [bp-0x8],ax
000001D8  1156FA            adc [bp-0x6],dx
000001DB  FF76FA            push word [bp-0x6]
000001DE  FF76F8            push word [bp-0x8]
000001E1  9AED01D616        call 0x16d6:0x1ed
000001E6  59                pop cx
000001E7  59                pop cx
000001E8  5E                pop si
000001E9  8BE5              mov sp,bp
000001EB  5D                pop bp
000001EC  CB                retf

; ---- 16D6:01ED ----
000001ED  55                push bp
000001EE  8BEC              mov bp,sp
000001F0  56                push si
000001F1  8B4608            mov ax,[bp+0x8]
000001F4  8B5606            mov dx,[bp+0x6]
000001F7  89168A2E          mov [0x2e8a],dx
000001FB  A38C2E            mov [0x2e8c],ax
000001FE  33F6              xor si,si
00000200  EB0B              jmp short 0x20d
00000202  8BDE              mov bx,si
00000204  D1E3              shl bx,1
00000206  C787562E0000      mov word [bx+0x2e56],0x0
0000020C  46                inc si
0000020D  83FE02            cmp si,byte +0x2
00000210  7EF0              jng 0x202
00000212  B81400            mov ax,0x14
00000215  50                push ax
00000216  B80100            mov ax,0x1
00000219  50                push ax
0000021A  9A9904D616        call 0x16d6:0x499
0000021F  59                pop cx
00000220  59                pop cx
00000221  33F6              xor si,si
00000223  EB72              jmp short 0x297
00000225  83FE02            cmp si,byte +0x2
00000228  7C2C              jl 0x256
0000022A  A0AA37            mov al,[0x37aa]
0000022D  98                cbw
0000022E  8BD8              mov bx,ax
00000230  80BF423D00        cmp byte [bx+0x3d42],0x0
00000235  761F              jna 0x256
00000237  8BC6              mov ax,si
00000239  05FEFF            add ax,0xfffe
0000023C  50                push ax
0000023D  33C0              xor ax,ax
0000023F  50                push ax
00000240  9AF702D616        call 0x16d6:0x2f7
00000245  59                pop cx
00000246  59                pop cx
00000247  B80300            mov ax,0x3
0000024A  50                push ax
0000024B  B80100            mov ax,0x1
0000024E  50                push ax
0000024F  9A9904D616        call 0x16d6:0x499
00000254  59                pop cx
00000255  59                pop cx
00000256  83FE06            cmp si,byte +0x6
00000259  7F18              jg 0x273
0000025B  56                push si
0000025C  B80100            mov ax,0x1
0000025F  50                push ax
00000260  9AF702D616        call 0x16d6:0x2f7
00000265  59                pop cx
00000266  59                pop cx
00000267  56                push si
00000268  B80200            mov ax,0x2
0000026B  50                push ax
0000026C  9AF702D616        call 0x16d6:0x2f7
00000271  59                pop cx
00000272  59                pop cx
00000273  B80100            mov ax,0x1
00000276  50                push ax
00000277  B80200            mov ax,0x2
0000027A  50                push ax
0000027B  9A9904D616        call 0x16d6:0x499
00000280  59                pop cx
00000281  59                pop cx
00000282  83FE04            cmp si,byte +0x4
00000285  750F              jnz 0x296
00000287  B80100            mov ax,0x1
0000028A  50                push ax
0000028B  B80400            mov ax,0x4
0000028E  50                push ax
0000028F  9A9904D616        call 0x16d6:0x499
00000294  59                pop cx
00000295  59                pop cx
00000296  46                inc si
00000297  83FE08            cmp si,byte +0x8
0000029A  7E89              jng 0x225
0000029C  B80100            mov ax,0x1
0000029F  50                push ax
000002A0  B80200            mov ax,0x2
000002A3  50                push ax
000002A4  9A9904D616        call 0x16d6:0x499
000002A9  59                pop cx
000002AA  59                pop cx
000002AB  B80A00            mov ax,0xa
000002AE  50                push ax
000002AF  B80100            mov ax,0x1
000002B2  50                push ax
000002B3  9A9904D616        call 0x16d6:0x499
000002B8  59                pop cx
000002B9  59                pop cx
000002BA  0AC0              or al,al
000002BC  7502              jnz 0x2c0
000002BE  EB02              jmp short 0x2c2
000002C0  EBDA              jmp short 0x29c
000002C2  B8E803            mov ax,0x3e8
000002C5  50                push ax
000002C6  B80300            mov ax,0x3
000002C9  50                push ax
000002CA  9A9904D616        call 0x16d6:0x499
000002CF  59                pop cx
000002D0  59                pop cx
000002D1  A0AA37            mov al,[0x37aa]
000002D4  98                cbw
000002D5  8BD8              mov bx,ax
000002D7  80BF423D00        cmp byte [bx+0x3d42],0x0
000002DC  7516              jnz 0x2f4
000002DE  BE0100            mov si,0x1
000002E1  EB0C              jmp short 0x2ef
000002E3  56                push si
000002E4  33C0              xor ax,ax
000002E6  50                push ax
000002E7  9AF702D616        call 0x16d6:0x2f7
000002EC  59                pop cx
000002ED  59                pop cx
000002EE  46                inc si
000002EF  83FE06            cmp si,byte +0x6
000002F2  7EEF              jng 0x2e3
000002F4  5E                pop si
000002F5  5D                pop bp
000002F6  CB                retf

; ---- 16D6:02F7 ----
000002F7  55                push bp
000002F8  8BEC              mov bp,sp
000002FA  56                push si
000002FB  57                push di
000002FC  8B7E06            mov di,[bp+0x6]
000002FF  8BC7              mov ax,di
00000301  BA0700            mov dx,0x7
00000304  F7EA              imul dx
00000306  034608            add ax,[bp+0x8]
00000309  050C00            add ax,0xc
0000030C  50                push ax
0000030D  9A280A681C        call 0x1c68:0xa28
00000312  59                pop cx
00000313  8BF0              mov si,ax
00000315  0BF6              or si,si
00000317  7F04              jg 0x31d
00000319  B001              mov al,0x1
0000031B  EB5B              jmp short 0x378
0000031D  8BC6              mov ax,si
0000031F  99                cwd
00000320  8BDF              mov bx,di
00000322  D1E3              shl bx,1
00000324  50                push ax
00000325  8B87562E          mov ax,[bx+0x2e56]
00000329  52                push dx
0000032A  99                cwd
0000032B  8B1E8C2E          mov bx,[0x2e8c]
0000032F  8B0E8A2E          mov cx,[0x2e8a]
00000333  03C8              add cx,ax
00000335  13DA              adc bx,dx
00000337  58                pop ax
00000338  3BC3              cmp ax,bx
0000033A  58                pop ax
0000033B  7C0A              jl 0x347
0000033D  7F04              jg 0x343
0000033F  3BC1              cmp ax,cx
00000341  7604              jna 0x347
00000343  B000              mov al,0x0
00000345  EB31              jmp short 0x378
00000347  A0AA37            mov al,[0x37aa]
0000034A  98                cbw
0000034B  BA0300            mov dx,0x3
0000034E  F7EA              imul dx
00000350  8A5608            mov dl,[bp+0x8]
00000353  8BD8              mov bx,ax
00000355  88917E2E          mov [bx+di+0x2e7e],dl
00000359  8BDF              mov bx,di
0000035B  D1E3              shl bx,1
0000035D  8BC6              mov ax,si
0000035F  2B87562E          sub ax,[bx+0x2e56]
00000363  99                cwd
00000364  29068A2E          sub [0x2e8a],ax
00000368  19168C2E          sbb [0x2e8c],dx
0000036C  8BDF              mov bx,di
0000036E  D1E3              shl bx,1
00000370  89B7562E          mov [bx+0x2e56],si
00000374  B001              mov al,0x1
00000376  EB00              jmp short 0x378
00000378  5F                pop di
00000379  5E                pop si
0000037A  5D                pop bp
0000037B  CB                retf

; ---- 16D6:037C ----
0000037C  55                push bp
0000037D  8BEC              mov bp,sp
0000037F  56                push si
00000380  57                push di
00000381  8B4608            mov ax,[bp+0x8]
00000384  8B5606            mov dx,[bp+0x6]
00000387  89168A2E          mov [0x2e8a],dx
0000038B  A38C2E            mov [0x2e8c],ax
0000038E  C7067C2E0000      mov word [0x2e7c],0x0
00000394  B81B00            mov ax,0x1b
00000397  50                push ax
00000398  B80600            mov ax,0x6
0000039B  50                push ax
0000039C  9A9904D616        call 0x16d6:0x499
000003A1  59                pop cx
000003A2  59                pop cx
000003A3  33F6              xor si,si
000003A5  EB56              jmp short 0x3fd
000003A7  B80100            mov ax,0x1
000003AA  50                push ax
000003AB  B80700            mov ax,0x7
000003AE  50                push ax
000003AF  9A9904D616        call 0x16d6:0x499
000003B4  59                pop cx
000003B5  59                pop cx
000003B6  3C01              cmp al,0x1
000003B8  7402              jz 0x3bc
000003BA  EB46              jmp short 0x402
000003BC  B81400            mov ax,0x14
000003BF  50                push ax
000003C0  B80300            mov ax,0x3
000003C3  50                push ax
000003C4  9A9904D616        call 0x16d6:0x499
000003C9  59                pop cx
000003CA  59                pop cx
000003CB  3C01              cmp al,0x1
000003CD  7402              jz 0x3d1
000003CF  EB31              jmp short 0x402
000003D1  B81000            mov ax,0x10
000003D4  50                push ax
000003D5  B80500            mov ax,0x5
000003D8  50                push ax
000003D9  9A9904D616        call 0x16d6:0x499
000003DE  59                pop cx
000003DF  59                pop cx
000003E0  3C01              cmp al,0x1
000003E2  7402              jz 0x3e6
000003E4  EB1C              jmp short 0x402
000003E6  83FE02            cmp si,byte +0x2
000003E9  7C11              jl 0x3fc
000003EB  83FE05            cmp si,byte +0x5
000003EE  7F0C              jg 0x3fc
000003F0  8BC6              mov ax,si
000003F2  05FEFF            add ax,0xfffe
000003F5  50                push ax
000003F6  9A7C05D616        call 0x16d6:0x57c
000003FB  59                pop cx
000003FC  46                inc si
000003FD  83FE07            cmp si,byte +0x7
00000400  7CA5              jl 0x3a7
00000402  A0AA37            mov al,[0x37aa]
00000405  98                cbw
00000406  B104              mov cl,0x4
00000408  D3E0              shl ax,cl
0000040A  8BD8              mov bx,ax
0000040C  83BF1E4607        cmp word [bx+0x461e],byte +0x7
00000411  753E              jnz 0x451
00000413  33F6              xor si,si
00000415  EB35              jmp short 0x44c
00000417  33FF              xor di,di
00000419  EB2B              jmp short 0x446
0000041B  B80600            mov ax,0x6
0000041E  50                push ax
0000041F  B80300            mov ax,0x3
00000422  50                push ax
00000423  9A9904D616        call 0x16d6:0x499
00000428  59                pop cx
00000429  59                pop cx
0000042A  3C01              cmp al,0x1
0000042C  7402              jz 0x430
0000042E  EB1B              jmp short 0x44b
00000430  B80800            mov ax,0x8
00000433  50                push ax
00000434  B80500            mov ax,0x5
00000437  50                push ax
00000438  9A9904D616        call 0x16d6:0x499
0000043D  59                pop cx
0000043E  59                pop cx
0000043F  3C01              cmp al,0x1
00000441  7402              jz 0x445
00000443  EB06              jmp short 0x44b
00000445  47                inc di
00000446  83FF06            cmp di,byte +0x6
00000449  7ED0              jng 0x41b
0000044B  46                inc si
0000044C  83FE04            cmp si,byte +0x4
0000044F  7CC6              jl 0x417
00000451  B86C00            mov ax,0x6c
00000454  50                push ax
00000455  B80600            mov ax,0x6
00000458  50                push ax
00000459  9A9904D616        call 0x16d6:0x499
0000045E  59                pop cx
0000045F  59                pop cx
00000460  B80600            mov ax,0x6
00000463  50                push ax
00000464  B80300            mov ax,0x3
00000467  50                push ax
00000468  9A9904D616        call 0x16d6:0x499
0000046D  59                pop cx
0000046E  59                pop cx
0000046F  3C01              cmp al,0x1
00000471  7402              jz 0x475
00000473  EB17              jmp short 0x48c
00000475  B80800            mov ax,0x8
00000478  50                push ax
00000479  B80500            mov ax,0x5
0000047C  50                push ax
0000047D  9A9904D616        call 0x16d6:0x499
00000482  59                pop cx
00000483  59                pop cx
00000484  3C01              cmp al,0x1
00000486  7402              jz 0x48a
00000488  EB02              jmp short 0x48c
0000048A  EBD4              jmp short 0x460
0000048C  8B168C2E          mov dx,[0x2e8c]
00000490  A18A2E            mov ax,[0x2e8a]
00000493  EB00              jmp short 0x495
00000495  5F                pop di
00000496  5E                pop si
00000497  5D                pop bp
00000498  CB                retf

; ---- 16D6:0499 ----
00000499  55                push bp
0000049A  8BEC              mov bp,sp
0000049C  4C                dec sp
0000049D  4C                dec sp
0000049E  56                push si
0000049F  57                push di
000004A0  8B7606            mov si,[bp+0x6]
000004A3  8BDE              mov bx,si
000004A5  D1E3              shl bx,1
000004A7  8B87A208          mov ax,[bx+0x8a2]
000004AB  8946FE            mov [bp-0x2],ax
000004AE  8BDE              mov bx,si
000004B0  D1E3              shl bx,1
000004B2  8B875C2E          mov ax,[bx+0x2e5c]
000004B6  99                cwd
000004B7  52                push dx
000004B8  50                push ax
000004B9  FF368C2E          push word [0x2e8c]
000004BD  FF368A2E          push word [0x2e8a]
000004C1  9A894F0000        call 0x0:0x4f89
000004C6  8BDE              mov bx,si
000004C8  D1E3              shl bx,1
000004CA  8B976C2E          mov dx,[bx+0x2e6c]
000004CE  03D0              add dx,ax
000004D0  8BFA              mov di,dx
000004D2  A0AA37            mov al,[0x37aa]
000004D5  98                cbw
000004D6  B104              mov cl,0x4
000004D8  D3E0              shl ax,cl
000004DA  8BD6              mov dx,si
000004DC  D1E2              shl dx,1
000004DE  03C2              add ax,dx
000004E0  8BD8              mov bx,ax
000004E2  8B871046          mov ax,[bx+0x4610]
000004E6  03C7              add ax,di
000004E8  3B46FE            cmp ax,[bp-0x2]
000004EB  7619              jna 0x506
000004ED  A0AA37            mov al,[0x37aa]
000004F0  98                cbw
000004F1  B104              mov cl,0x4
000004F3  D3E0              shl ax,cl
000004F5  8BD6              mov dx,si
000004F7  D1E2              shl dx,1
000004F9  03C2              add ax,dx
000004FB  8B56FE            mov dx,[bp-0x2]
000004FE  8BD8              mov bx,ax
00000500  2B971046          sub dx,[bx+0x4610]
00000504  8BFA              mov di,dx
00000506  397E08            cmp [bp+0x8],di
00000509  7E03              jng 0x50e
0000050B  897E08            mov [bp+0x8],di
0000050E  8BDE              mov bx,si
00000510  D1E3              shl bx,1
00000512  8B876C2E          mov ax,[bx+0x2e6c]
00000516  3B4608            cmp ax,[bp+0x8]
00000519  7C0D              jl 0x528
0000051B  8BDE              mov bx,si
0000051D  D1E3              shl bx,1
0000051F  8B4608            mov ax,[bp+0x8]
00000522  29876C2E          sub [bx+0x2e6c],ax
00000526  EB28              jmp short 0x550
00000528  8BDE              mov bx,si
0000052A  D1E3              shl bx,1
0000052C  8B875C2E          mov ax,[bx+0x2e5c]
00000530  8BDE              mov bx,si
00000532  D1E3              shl bx,1
00000534  8B5608            mov dx,[bp+0x8]
00000537  2B976C2E          sub dx,[bx+0x2e6c]
0000053B  F7EA              imul dx
0000053D  99                cwd
0000053E  29068A2E          sub [0x2e8a],ax
00000542  19168C2E          sbb [0x2e8c],dx
00000546  8BDE              mov bx,si
00000548  D1E3              shl bx,1
0000054A  C7876C2E0000      mov word [bx+0x2e6c],0x0
00000550  A0AA37            mov al,[0x37aa]
00000553  98                cbw
00000554  B104              mov cl,0x4
00000556  D3E0              shl ax,cl
00000558  8BD6              mov dx,si
0000055A  D1E2              shl dx,1
0000055C  03C2              add ax,dx
0000055E  8B5608            mov dx,[bp+0x8]
00000561  8BD8              mov bx,ax
00000563  01971046          add [bx+0x4610],dx
00000567  397E08            cmp [bp+0x8],di
0000056A  7506              jnz 0x572
0000056C  B000              mov al,0x0
0000056E  EB06              jmp short 0x576
00000570  EB04              jmp short 0x576
00000572  B001              mov al,0x1
00000574  EB00              jmp short 0x576
00000576  5F                pop di
00000577  5E                pop si
00000578  8BE5              mov sp,bp
0000057A  5D                pop bp
0000057B  CB                retf

; ---- 16D6:057C ----
0000057C  55                push bp
0000057D  8BEC              mov bp,sp
0000057F  56                push si
00000580  8B4606            mov ax,[bp+0x6]
00000583  050800            add ax,0x8
00000586  50                push ax
00000587  9A280A681C        call 0x1c68:0xa28
0000058C  59                pop cx
0000058D  8BF0              mov si,ax
0000058F  8BC6              mov ax,si
00000591  99                cwd
00000592  50                push ax
00000593  A17C2E            mov ax,[0x2e7c]
00000596  52                push dx
00000597  99                cwd
00000598  8B1E8C2E          mov bx,[0x2e8c]
0000059C  8B0E8A2E          mov cx,[0x2e8a]
000005A0  03C8              add cx,ax
000005A2  13DA              adc bx,dx
000005A4  58                pop ax
000005A5  3BC3              cmp ax,bx
000005A7  58                pop ax
000005A8  7C0A              jl 0x5b4
000005AA  7F04              jg 0x5b0
000005AC  3BC1              cmp ax,cx
000005AE  7604              jna 0x5b4
000005B0  B000              mov al,0x0
000005B2  EB24              jmp short 0x5d8
000005B4  A0AA37            mov al,[0x37aa]
000005B7  98                cbw
000005B8  8A5606            mov dl,[bp+0x6]
000005BB  8BD8              mov bx,ax
000005BD  88971E3D          mov [bx+0x3d1e],dl
000005C1  8BC6              mov ax,si
000005C3  2B067C2E          sub ax,[0x2e7c]
000005C7  99                cwd
000005C8  29068A2E          sub [0x2e8a],ax
000005CC  19168C2E          sbb [0x2e8c],dx
000005D0  89367C2E          mov [0x2e7c],si
000005D4  B001              mov al,0x1
000005D6  EB00              jmp short 0x5d8
000005D8  5E                pop si
000005D9  5D                pop bp
000005DA  CB                retf

; ---- 16D6:05DB ----
000005DB  55                push bp
000005DC  8BEC              mov bp,sp
000005DE  83EC04            sub sp,byte +0x4
000005E1  C646FD00          mov byte [bp-0x3],0x0
000005E5  C646FF00          mov byte [bp-0x1],0x0
000005E9  EB2F              jmp short 0x61a
000005EB  8A46FF            mov al,[bp-0x1]
000005EE  98                cbw
000005EF  50                push ax
000005F0  9A2308D616        call 0x16d6:0x823
000005F5  59                pop cx
000005F6  3C01              cmp al,0x1
000005F8  7502              jnz 0x5fc
000005FA  EB1B              jmp short 0x617
000005FC  A0AA37            mov al,[0x37aa]
000005FF  98                cbw
00000600  BA0C00            mov dx,0xc
00000603  F7EA              imul dx
00000605  50                push ax
00000606  8A46FD            mov al,[bp-0x3]
00000609  98                cbw
0000060A  5B                pop bx
0000060B  03D8              add bx,ax
0000060D  8A46FF            mov al,[bp-0x1]
00000610  88878A45          mov [bx+0x458a],al
00000614  FE46FD            inc byte [bp-0x3]
00000617  FE46FF            inc byte [bp-0x1]
0000061A  807EFF07          cmp byte [bp-0x1],0x7
0000061E  7ECB              jng 0x5eb
00000620  A0AA37            mov al,[0x37aa]
00000623  98                cbw
00000624  8BD8              mov bx,ax
00000626  80BF1E3DFF        cmp byte [bx+0x3d1e],0xff
0000062B  7439              jz 0x666
0000062D  A0AA37            mov al,[0x37aa]
00000630  98                cbw
00000631  8BD8              mov bx,ax
00000633  8A871E3D          mov al,[bx+0x3d1e]
00000637  0408              add al,0x8
00000639  8846FF            mov [bp-0x1],al
0000063C  8A46FF            mov al,[bp-0x1]
0000063F  98                cbw
00000640  50                push ax
00000641  9A2308D616        call 0x16d6:0x823
00000646  59                pop cx
00000647  0AC0              or al,al
00000649  751B              jnz 0x666
0000064B  A0AA37            mov al,[0x37aa]
0000064E  98                cbw
0000064F  BA0C00            mov dx,0xc
00000652  F7EA              imul dx
00000654  50                push ax
00000655  8A46FD            mov al,[bp-0x3]
00000658  98                cbw
00000659  5B                pop bx
0000065A  03D8              add bx,ax
0000065C  8A46FF            mov al,[bp-0x1]
0000065F  88878A45          mov [bx+0x458a],al
00000663  FE46FD            inc byte [bp-0x3]
00000666  C646FE00          mov byte [bp-0x2],0x0
0000066A  EB6A              jmp short 0x6d6
0000066C  A0AA37            mov al,[0x37aa]
0000066F  98                cbw
00000670  BA0300            mov dx,0x3
00000673  F7EA              imul dx
00000675  50                push ax
00000676  8A46FE            mov al,[bp-0x2]
00000679  98                cbw
0000067A  5B                pop bx
0000067B  03D8              add bx,ax
0000067D  80BF7E2E00        cmp byte [bx+0x2e7e],0x0
00000682  7502              jnz 0x686
00000684  EB4D              jmp short 0x6d3
00000686  A0AA37            mov al,[0x37aa]
00000689  98                cbw
0000068A  BA0300            mov dx,0x3
0000068D  F7EA              imul dx
0000068F  50                push ax
00000690  8A46FE            mov al,[bp-0x2]
00000693  98                cbw
00000694  5B                pop bx
00000695  03D8              add bx,ax
00000697  8A46FE            mov al,[bp-0x2]
0000069A  98                cbw
0000069B  BA0700            mov dx,0x7
0000069E  F7EA              imul dx
000006A0  02877E2E          add al,[bx+0x2e7e]
000006A4  040C              add al,0xc
000006A6  8846FF            mov [bp-0x1],al
000006A9  8A46FF            mov al,[bp-0x1]
000006AC  98                cbw
000006AD  50                push ax
000006AE  9A2308D616        call 0x16d6:0x823
000006B3  59                pop cx
000006B4  0AC0              or al,al
000006B6  751B              jnz 0x6d3
000006B8  A0AA37            mov al,[0x37aa]
000006BB  98                cbw
000006BC  BA0C00            mov dx,0xc
000006BF  F7EA              imul dx
000006C1  50                push ax
000006C2  8A46FD            mov al,[bp-0x3]
000006C5  98                cbw
000006C6  5B                pop bx
000006C7  03D8              add bx,ax
000006C9  8A46FF            mov al,[bp-0x1]
000006CC  88878A45          mov [bx+0x458a],al
000006D0  FE46FD            inc byte [bp-0x3]
000006D3  FE46FE            inc byte [bp-0x2]
000006D6  807EFE02          cmp byte [bp-0x2],0x2
000006DA  7E90              jng 0x66c
000006DC  A0AA37            mov al,[0x37aa]
000006DF  98                cbw
000006E0  BA0C00            mov dx,0xc
000006E3  F7EA              imul dx
000006E5  50                push ax
000006E6  8A46FD            mov al,[bp-0x3]
000006E9  98                cbw
000006EA  5B                pop bx
000006EB  03D8              add bx,ax
000006ED  C6878A4500        mov byte [bx+0x458a],0x0
000006F2  8BE5              mov sp,bp
000006F4  5D                pop bp
000006F5  CB                retf

; ---- 16D6:06F6 ----
000006F6  55                push bp
000006F7  8BEC              mov bp,sp
000006F9  C606AA3700        mov byte [0x37aa],0x0
000006FE  EB42              jmp short 0x742
00000700  A0AA37            mov al,[0x37aa]
00000703  98                cbw
00000704  8BD8              mov bx,ax
00000706  80BF400801        cmp byte [bx+0x840],0x1
0000070B  7531              jnz 0x73e
0000070D  A0AA37            mov al,[0x37aa]
00000710  98                cbw
00000711  8BD8              mov bx,ax
00000713  C687C33700        mov byte [bx+0x37c3],0x0
00000718  A0AA37            mov al,[0x37aa]
0000071B  98                cbw
0000071C  8BD8              mov bx,ax
0000071E  8A87B243          mov al,[bx+0x43b2]
00000722  B400              mov ah,0x0
00000724  3D0B00            cmp ax,0xb
00000727  740E              jz 0x737
00000729  3D0C00            cmp ax,0xc
0000072C  7402              jz 0x730
0000072E  EB0E              jmp short 0x73e
00000730  9A4B07D616        call 0x16d6:0x74b
00000735  EB07              jmp short 0x73e
00000737  9AD608D616        call 0x16d6:0x8d6
0000073C  EB00              jmp short 0x73e
0000073E  FE06AA37          inc byte [0x37aa]
00000742  803EAA3704        cmp byte [0x37aa],0x4
00000747  7CB7              jl 0x700
00000749  5D                pop bp
0000074A  CB                retf

; ---- 16D6:074B ----
0000074B  55                push bp
0000074C  8BEC              mov bp,sp
0000074E  83EC04            sub sp,byte +0x4
00000751  A0AA37            mov al,[0x37aa]
00000754  98                cbw
00000755  3B06720E          cmp ax,[0xe72]
00000759  7403              jz 0x75e
0000075B  E9C100            jmp 0x81f
0000075E  A0AA37            mov al,[0x37aa]
00000761  98                cbw
00000762  8BD8              mov bx,ax
00000764  8A870930          mov al,[bx+0x3009]
00000768  8846FE            mov [bp-0x2],al
0000076B  A0AA37            mov al,[0x37aa]
0000076E  98                cbw
0000076F  8BD8              mov bx,ax
00000771  8A87B73C          mov al,[bx+0x3cb7]
00000775  8846FD            mov [bp-0x3],al
00000778  8A46FD            mov al,[bp-0x3]
0000077B  98                cbw
0000077C  8BD8              mov bx,ax
0000077E  8A879808          mov al,[bx+0x898]
00000782  3A46FE            cmp al,[bp-0x2]
00000785  7F0F              jg 0x796
00000787  8A46FD            mov al,[bp-0x3]
0000078A  98                cbw
0000078B  8BD8              mov bx,ax
0000078D  8A879D08          mov al,[bx+0x89d]
00000791  3A46FE            cmp al,[bp-0x2]
00000794  7D0D              jnl 0x7a3
00000796  A0AA37            mov al,[0x37aa]
00000799  98                cbw
0000079A  8BD8              mov bx,ax
0000079C  C687C33708        mov byte [bx+0x37c3],0x8
000007A1  EB7C              jmp short 0x81f
000007A3  A0AA37            mov al,[0x37aa]
000007A6  98                cbw
000007A7  8BD8              mov bx,ax
000007A9  8A87C244          mov al,[bx+0x44c2]
000007AD  3A46FE            cmp al,[bp-0x2]
000007B0  740D              jz 0x7bf
000007B2  A0AA37            mov al,[0x37aa]
000007B5  98                cbw
000007B6  8BD8              mov bx,ax
000007B8  C687C33701        mov byte [bx+0x37c3],0x1
000007BD  EB60              jmp short 0x81f
000007BF  807EFE00          cmp byte [bp-0x2],0x0
000007C3  7502              jnz 0x7c7
000007C5  EB58              jmp short 0x81f
000007C7  8A46FE            mov al,[bp-0x2]
000007CA  98                cbw
000007CB  50                push ax
000007CC  9A2308D616        call 0x16d6:0x823
000007D1  59                pop cx
000007D2  0AC0              or al,al
000007D4  750D              jnz 0x7e3
000007D6  A0AA37            mov al,[0x37aa]
000007D9  98                cbw
000007DA  8BD8              mov bx,ax
000007DC  C687C33710        mov byte [bx+0x37c3],0x10
000007E1  EB3C              jmp short 0x81f
000007E3  A0AA37            mov al,[0x37aa]
000007E6  98                cbw
000007E7  8BD8              mov bx,ax
000007E9  8A871E30          mov al,[bx+0x301e]
000007ED  FEC0              inc al
000007EF  50                push ax
000007F0  A0AA37            mov al,[0x37aa]
000007F3  98                cbw
000007F4  8BD8              mov bx,ax
000007F6  58                pop ax
000007F7  88871E30          mov [bx+0x301e],al
000007FB  8846FF            mov [bp-0x1],al
000007FE  A0AA37            mov al,[0x37aa]
00000801  98                cbw
00000802  BA0C00            mov dx,0xc
00000805  F7EA              imul dx
00000807  50                push ax
00000808  8A46FF            mov al,[bp-0x1]
0000080B  98                cbw
0000080C  5B                pop bx
0000080D  03D8              add bx,ax
0000080F  8A878A45          mov al,[bx+0x458a]
00000813  50                push ax
00000814  A0AA37            mov al,[0x37aa]
00000817  98                cbw
00000818  8BD8              mov bx,ax
0000081A  58                pop ax
0000081B  88870930          mov [bx+0x3009],al
0000081F  8BE5              mov sp,bp
00000821  5D                pop bp
00000822  CB                retf

; ---- 16D6:0823 ----
00000823  55                push bp
00000824  8BEC              mov bp,sp
00000826  4C                dec sp
00000827  4C                dec sp
00000828  56                push si
00000829  8B7606            mov si,[bp+0x6]
0000082C  83FE08            cmp si,byte +0x8
0000082F  7D38              jnl 0x869
00000831  A0AA37            mov al,[0x37aa]
00000834  98                cbw
00000835  B104              mov cl,0x4
00000837  D3E0              shl ax,cl
00000839  8BD6              mov dx,si
0000083B  D1E2              shl dx,1
0000083D  03C2              add ax,dx
0000083F  8BD8              mov bx,ax
00000841  8B87A448          mov ax,[bx+0x48a4]
00000845  50                push ax
00000846  A0AA37            mov al,[0x37aa]
00000849  98                cbw
0000084A  B104              mov cl,0x4
0000084C  D3E0              shl ax,cl
0000084E  8BD6              mov dx,si
00000850  D1E2              shl dx,1
00000852  03C2              add ax,dx
00000854  8BD8              mov bx,ax
00000856  58                pop ax
00000857  3B871046          cmp ax,[bx+0x4610]
0000085B  7206              jc 0x863
0000085D  B001              mov al,0x1
0000085F  EB70              jmp short 0x8d1
00000861  EB04              jmp short 0x867
00000863  B000              mov al,0x0
00000865  EB6A              jmp short 0x8d1
00000867  EB68              jmp short 0x8d1
00000869  83FE0C            cmp si,byte +0xc
0000086C  7D24              jnl 0x892
0000086E  A0AA37            mov al,[0x37aa]
00000871  98                cbw
00000872  8BD8              mov bx,ax
00000874  8A87012F          mov al,[bx+0x2f01]
00000878  50                push ax
00000879  A0AA37            mov al,[0x37aa]
0000087C  98                cbw
0000087D  8BD8              mov bx,ax
0000087F  58                pop ax
00000880  3A871E3D          cmp al,[bx+0x3d1e]
00000884  7506              jnz 0x88c
00000886  B001              mov al,0x1
00000888  EB47              jmp short 0x8d1
0000088A  EB04              jmp short 0x890
0000088C  B000              mov al,0x0
0000088E  EB41              jmp short 0x8d1
00000890  EB3F              jmp short 0x8d1
00000892  8BC6              mov ax,si
00000894  05F4FF            add ax,0xfff4
00000897  BB0700            mov bx,0x7
0000089A  99                cwd
0000089B  F7FB              idiv bx
0000089D  8846FF            mov [bp-0x1],al
000008A0  8A46FF            mov al,[bp-0x1]
000008A3  98                cbw
000008A4  50                push ax
000008A5  9A020B681C        call 0x1c68:0xb02
000008AA  59                pop cx
000008AB  50                push ax
000008AC  A0AA37            mov al,[0x37aa]
000008AF  98                cbw
000008B0  BA0300            mov dx,0x3
000008B3  F7EA              imul dx
000008B5  50                push ax
000008B6  8A46FF            mov al,[bp-0x1]
000008B9  98                cbw
000008BA  5B                pop bx
000008BB  03D8              add bx,ax
000008BD  8A877E2E          mov al,[bx+0x2e7e]
000008C1  98                cbw
000008C2  5A                pop dx
000008C3  3BD0              cmp dx,ax
000008C5  7C06              jl 0x8cd
000008C7  B001              mov al,0x1
000008C9  EB06              jmp short 0x8d1
000008CB  EB04              jmp short 0x8d1
000008CD  B000              mov al,0x0
000008CF  EB00              jmp short 0x8d1
000008D1  5E                pop si
000008D2  8BE5              mov sp,bp
000008D4  5D                pop bp
000008D5  CB                retf

; ---- 16D6:08D6 ----
000008D6  55                push bp
000008D7  8BEC              mov bp,sp
000008D9  4C                dec sp
000008DA  4C                dec sp
000008DB  A0AA37            mov al,[0x37aa]
000008DE  98                cbw
000008DF  8BD8              mov bx,ax
000008E1  8A870930          mov al,[bx+0x3009]
000008E5  8846FF            mov [bp-0x1],al
000008E8  A0AA37            mov al,[0x37aa]
000008EB  98                cbw
000008EC  8BD8              mov bx,ax
000008EE  80BF3E4400        cmp byte [bx+0x443e],0x0
000008F3  7403              jz 0x8f8
000008F5  E9F600            jmp 0x9ee
000008F8  B100              mov cl,0x0
000008FA  EB20              jmp short 0x91c
000008FC  8AC1              mov al,cl
000008FE  98                cbw
000008FF  8BD8              mov bx,ax
00000901  8A879808          mov al,[bx+0x898]
00000905  3A46FF            cmp al,[bp-0x1]
00000908  7F10              jg 0x91a
0000090A  8AC1              mov al,cl
0000090C  98                cbw
0000090D  8BD8              mov bx,ax
0000090F  8A879D08          mov al,[bx+0x89d]
00000913  3A46FF            cmp al,[bp-0x1]
00000916  7C02              jl 0x91a
00000918  EB07              jmp short 0x921
0000091A  FEC1              inc cl
0000091C  80F904            cmp cl,0x4
0000091F  7EDB              jng 0x8fc
00000921  A0AA37            mov al,[0x37aa]
00000924  98                cbw
00000925  8AD1              mov dl,cl
00000927  8BD8              mov bx,ax
00000929  88971A45          mov [bx+0x451a],dl
0000092D  8AC1              mov al,cl
0000092F  98                cbw
00000930  D1E0              shl ax,1
00000932  D1E0              shl ax,1
00000934  8BD8              mov bx,ax
00000936  8B87B408          mov ax,[bx+0x8b4]
0000093A  50                push ax
0000093B  8AC1              mov al,cl
0000093D  98                cbw
0000093E  D1E0              shl ax,1
00000940  D1E0              shl ax,1
00000942  8BD8              mov bx,ax
00000944  58                pop ax
00000945  2B87B208          sub ax,[bx+0x8b2]
00000949  BB0500            mov bx,0x5
0000094C  99                cwd
0000094D  F7FB              idiv bx
0000094F  50                push ax
00000950  A0AA37            mov al,[0x37aa]
00000953  98                cbw
00000954  40                inc ax
00000955  8BD0              mov dx,ax
00000957  58                pop ax
00000958  F7EA              imul dx
0000095A  50                push ax
0000095B  8AC1              mov al,cl
0000095D  98                cbw
0000095E  D1E0              shl ax,1
00000960  D1E0              shl ax,1
00000962  8BD8              mov bx,ax
00000964  8B87B208          mov ax,[bx+0x8b2]
00000968  5A                pop dx
00000969  03C2              add ax,dx
0000096B  50                push ax
0000096C  A0AA37            mov al,[0x37aa]
0000096F  98                cbw
00000970  D1E0              shl ax,1
00000972  8BD8              mov bx,ax
00000974  58                pop ax
00000975  89874E2E          mov [bx+0x2e4e],ax
00000979  A0AA37            mov al,[0x37aa]
0000097C  98                cbw
0000097D  D1E0              shl ax,1
0000097F  8BD8              mov bx,ax
00000981  8B874E2E          mov ax,[bx+0x2e4e]
00000985  25FEFF            and ax,0xfffe
00000988  50                push ax
00000989  A0AA37            mov al,[0x37aa]
0000098C  98                cbw
0000098D  D1E0              shl ax,1
0000098F  8BD8              mov bx,ax
00000991  58                pop ax
00000992  89874E2E          mov [bx+0x2e4e],ax
00000996  A0AA37            mov al,[0x37aa]
00000999  98                cbw
0000099A  BA1A00            mov dx,0x1a
0000099D  F7EA              imul dx
0000099F  8BD8              mov bx,ax
000009A1  8B872033          mov ax,[bx+0x3320]
000009A5  50                push ax
000009A6  A0AA37            mov al,[0x37aa]
000009A9  98                cbw
000009AA  D1E0              shl ax,1
000009AC  8BD8              mov bx,ax
000009AE  58                pop ax
000009AF  3B874E2E          cmp ax,[bx+0x2e4e]
000009B3  7E0D              jng 0x9c2
000009B5  A0AA37            mov al,[0x37aa]
000009B8  98                cbw
000009B9  8BD8              mov bx,ax
000009BB  C687C33702        mov byte [bx+0x37c3],0x2
000009C0  EB2A              jmp short 0x9ec
000009C2  A0AA37            mov al,[0x37aa]
000009C5  98                cbw
000009C6  BA1A00            mov dx,0x1a
000009C9  F7EA              imul dx
000009CB  8BD8              mov bx,ax
000009CD  8B872033          mov ax,[bx+0x3320]
000009D1  50                push ax
000009D2  A0AA37            mov al,[0x37aa]
000009D5  98                cbw
000009D6  D1E0              shl ax,1
000009D8  8BD8              mov bx,ax
000009DA  58                pop ax
000009DB  3B874E2E          cmp ax,[bx+0x2e4e]
000009DF  7D0B              jnl 0x9ec
000009E1  A0AA37            mov al,[0x37aa]
000009E4  98                cbw
000009E5  8BD8              mov bx,ax
000009E7  C687C33701        mov byte [bx+0x37c3],0x1
000009EC  EB2A              jmp short 0xa18
000009EE  A0AA37            mov al,[0x37aa]
000009F1  98                cbw
000009F2  BA1A00            mov dx,0x1a
000009F5  F7EA              imul dx
000009F7  8BD8              mov bx,ax
000009F9  8B872033          mov ax,[bx+0x3320]
000009FD  50                push ax
000009FE  A0AA37            mov al,[0x37aa]
00000A01  98                cbw
00000A02  D1E0              shl ax,1
00000A04  8BD8              mov bx,ax
00000A06  58                pop ax
00000A07  3B874E2E          cmp ax,[bx+0x2e4e]
00000A0B  750B              jnz 0xa18
00000A0D  A0AA37            mov al,[0x37aa]
