; segment 20B3  file offset 0x24130  size 0xD80
00000000  5E                pop si
00000001  8BE5              mov sp,bp
00000003  5D                pop bp
00000004  CB                retf

; ---- 20B3:0005 ----
00000005  55                push bp
00000006  8BEC              mov bp,sp
00000008  9ACB062508        call 0x825:0x6cb
0000000D  C606AA3700        mov byte [0x37aa],0x0
00000012  EB26              jmp short 0x3a
00000014  A0AA37            mov al,[0x37aa]
00000017  98                cbw
00000018  D1E0              shl ax,1
0000001A  D1E0              shl ax,1
0000001C  8BD8              mov bx,ax
0000001E  8B87082F          mov ax,[bx+0x2f08]
00000022  8B97062F          mov dx,[bx+0x2f06]
00000026  3B06F608          cmp ax,[0x8f6]
0000002A  720A              jc 0x36
0000002C  7506              jnz 0x34
0000002E  3B16F408          cmp dx,[0x8f4]
00000032  7202              jc 0x36
00000034  EB0B              jmp short 0x41
00000036  FE06AA37          inc byte [0x37aa]
0000003A  803EAA3704        cmp byte [0x37aa],0x4
0000003F  7CD3              jl 0x14
00000041  803EAA3704        cmp byte [0x37aa],0x4
00000046  750B              jnz 0x53
00000048  803E8B3D00        cmp byte [0x3d8b],0x0
0000004D  7504              jnz 0x53
0000004F  B000              mov al,0x0
00000051  EB39              jmp short 0x8c
00000053  9A8E00B320        call 0x20b3:0x8e
00000058  B80300            mov ax,0x3
0000005B  50                push ax
0000005C  B80200            mov ax,0x2
0000005F  50                push ax
00000060  9A94008B21        call 0x218b:0x94
00000065  59                pop cx
00000066  59                pop cx
00000067  9A2505B320        call 0x20b3:0x525
0000006C  9A6308B320        call 0x20b3:0x863
00000071  FF069A4B          inc word [0x4b9a]
00000075  9AC4072508        call 0x825:0x7c4
0000007A  B000              mov al,0x0
0000007C  50                push ax
0000007D  9A0A002508        call 0x825:0xa
00000082  59                pop cx
00000083  9AD200C627        call 0x27c6:0xd2
00000088  B001              mov al,0x1
0000008A  EB00              jmp short 0x8c
0000008C  5D                pop bp
0000008D  CB                retf

; ---- 20B3:008E ----
0000008E  55                push bp
0000008F  8BEC              mov bp,sp
00000091  FF36CE08          push word [0x8ce]
00000095  9A02006C18        call 0x186c:0x2
0000009A  59                pop cx
0000009B  9A2D1F0906        call 0x609:0x1f2d
000000A0  33C0              xor ax,ax
000000A2  50                push ax
000000A3  9A7D03FD21        call 0x21fd:0x37d
000000A8  59                pop cx
000000A9  1E                push ds
000000AA  B8A800            mov ax,0xa8
000000AD  50                push ax
000000AE  9A6C02FD21        call 0x21fd:0x26c
000000B3  59                pop cx
000000B4  59                pop cx
000000B5  9AE7110906        call 0x609:0x11e7
000000BA  9A5B01B320        call 0x20b3:0x15b
000000BF  9A6702B320        call 0x20b3:0x267
000000C4  9A2903FD21        call 0x21fd:0x329
000000C9  9A7B04B320        call 0x20b3:0x47b
000000CE  33C0              xor ax,ax
000000D0  BA9600            mov dx,0x96
000000D3  50                push ax
000000D4  52                push dx
000000D5  9A8501FD21        call 0x21fd:0x185
000000DA  59                pop cx
000000DB  59                pop cx
000000DC  C606D65400        mov byte [0x54d6],0x0
000000E1  C606AA3700        mov byte [0x37aa],0x0
000000E6  EB3C              jmp short 0x124
000000E8  A0AA37            mov al,[0x37aa]
000000EB  98                cbw
000000EC  8BD8              mov bx,ax
000000EE  80BF423D00        cmp byte [bx+0x3d42],0x0
000000F3  752B              jnz 0x120
000000F5  9A0403B320        call 0x20b3:0x304
000000FA  9A4F03B320        call 0x20b3:0x34f
000000FF  9ABF03B320        call 0x20b3:0x3bf
00000104  9AE503B320        call 0x20b3:0x3e5
00000109  33C0              xor ax,ax
0000010B  BA1400            mov dx,0x14
0000010E  50                push ax
0000010F  52                push dx
00000110  9A8501FD21        call 0x21fd:0x185
00000115  59                pop cx
00000116  59                pop cx
00000117  9A3204B320        call 0x20b3:0x432
0000011C  FE06D654          inc byte [0x54d6]
00000120  FE06AA37          inc byte [0x37aa]
00000124  803EAA3704        cmp byte [0x37aa],0x4
00000129  7CBD              jl 0xe8
0000012B  33C0              xor ax,ax
0000012D  BA3200            mov dx,0x32
00000130  50                push ax
00000131  52                push dx
00000132  9A8501FD21        call 0x21fd:0x185
00000137  59                pop cx
00000138  59                pop cx
00000139  33C0              xor ax,ax
0000013B  50                push ax
0000013C  9A7D03FD21        call 0x21fd:0x37d
00000141  59                pop cx
00000142  C606AA3700        mov byte [0x37aa],0x0
00000147  EB09              jmp short 0x152
00000149  9ABA210906        call 0x609:0x21ba
0000014E  FE06AA37          inc byte [0x37aa]
00000152  803EAA3704        cmp byte [0x37aa],0x4
00000157  7CF0              jl 0x149
00000159  5D                pop bp
0000015A  CB                retf

; ---- 20B3:015B ----
0000015B  55                push bp
0000015C  8BEC              mov bp,sp
0000015E  9A1B00960A        call 0xa96:0x1b
00000163  9AD91E0906        call 0x609:0x1ed9
00000168  B83600            mov ax,0x36
0000016B  50                push ax
0000016C  B84001            mov ax,0x140
0000016F  50                push ax
00000170  33C0              xor ax,ax
00000172  50                push ax
00000173  33C0              xor ax,ax
00000175  50                push ax
00000176  9A56120906        call 0x609:0x1256
0000017B  83C408            add sp,byte +0x8
0000017E  B84200            mov ax,0x42
00000181  50                push ax
00000182  B8E000            mov ax,0xe0
00000185  50                push ax
00000186  B86400            mov ax,0x64
00000189  50                push ax
0000018A  B83000            mov ax,0x30
0000018D  50                push ax
0000018E  9AC5130906        call 0x609:0x13c5
00000193  83C408            add sp,byte +0x8
00000196  B84200            mov ax,0x42
00000199  50                push ax
0000019A  B8E000            mov ax,0xe0
0000019D  50                push ax
0000019E  B86400            mov ax,0x64
000001A1  50                push ax
000001A2  B83000            mov ax,0x30
000001A5  50                push ax
000001A6  B81600            mov ax,0x16
000001A9  50                push ax
000001AA  B82000            mov ax,0x20
000001AD  50                push ax
000001AE  B84200            mov ax,0x42
000001B1  50                push ax
000001B2  B8A000            mov ax,0xa0
000001B5  50                push ax
000001B6  9A6702960A        call 0xa96:0x267
000001BB  83C410            add sp,byte +0x10
000001BE  B81600            mov ax,0x16
000001C1  50                push ax
000001C2  B8A100            mov ax,0xa1
000001C5  50                push ax
000001C6  B86400            mov ax,0x64
000001C9  50                push ax
000001CA  B87000            mov ax,0x70
000001CD  50                push ax
000001CE  B81600            mov ax,0x16
000001D1  50                push ax
000001D2  B82000            mov ax,0x20
000001D5  50                push ax
000001D6  B8D000            mov ax,0xd0
000001D9  50                push ax
000001DA  B82001            mov ax,0x120
000001DD  50                push ax
000001DE  9A6702960A        call 0xa96:0x267
000001E3  83C410            add sp,byte +0x10
000001E6  B81600            mov ax,0x16
000001E9  50                push ax
000001EA  B8A100            mov ax,0xa1
000001ED  50                push ax
000001EE  B89000            mov ax,0x90
000001F1  50                push ax
000001F2  B87000            mov ax,0x70
000001F5  50                push ax
000001F6  B81600            mov ax,0x16
000001F9  50                push ax
000001FA  B82000            mov ax,0x20
000001FD  50                push ax
000001FE  B8E600            mov ax,0xe6
00000201  50                push ax
00000202  B82001            mov ax,0x120
00000205  50                push ax
00000206  9A6702960A        call 0xa96:0x267
0000020B  83C410            add sp,byte +0x10
0000020E  B85400            mov ax,0x54
00000211  50                push ax
00000212  B84A00            mov ax,0x4a
00000215  50                push ax
00000216  B81C00            mov ax,0x1c
00000219  50                push ax
0000021A  B83B00            mov ax,0x3b
0000021D  50                push ax
0000021E  B81E00            mov ax,0x1e
00000221  50                push ax
00000222  B8D000            mov ax,0xd0
00000225  50                push ax
00000226  B8E300            mov ax,0xe3
00000229  50                push ax
0000022A  B8D500            mov ax,0xd5
0000022D  50                push ax
0000022E  9A3B01960A        call 0xa96:0x13b
00000233  83C410            add sp,byte +0x10
00000236  B87B00            mov ax,0x7b
00000239  50                push ax
0000023A  B82E00            mov ax,0x2e
0000023D  50                push ax
0000023E  B81200            mov ax,0x12
00000241  50                push ax
00000242  B83B00            mov ax,0x3b
00000245  50                push ax
00000246  B8EE00            mov ax,0xee
00000249  50                push ax
0000024A  B89000            mov ax,0x90
0000024D  50                push ax
0000024E  9A3000960A        call 0xa96:0x30
00000253  83C40C            add sp,byte +0xc
00000256  9AA806960A        call 0xa96:0x6a8
0000025B  FF362E08          push word [0x82e]
0000025F  9A3F0C960A        call 0xa96:0xc3f
00000264  59                pop cx
00000265  5D                pop bp
00000266  CB                retf

; ---- 20B3:0267 ----
00000267  55                push bp
00000268  8BEC              mov bp,sp
0000026A  9A1B00960A        call 0xa96:0x1b
0000026F  C606AA3700        mov byte [0x37aa],0x0
00000274  EB7D              jmp short 0x2f3
00000276  B80100            mov ax,0x1
00000279  50                push ax
0000027A  B80600            mov ax,0x6
0000027D  50                push ax
0000027E  A0AA37            mov al,[0x37aa]
00000281  98                cbw
00000282  B106              mov cl,0x6
00000284  D3E0              shl ax,cl
00000286  052000            add ax,0x20
00000289  50                push ax
0000028A  9A79180906        call 0x609:0x1879
0000028F  83C406            add sp,byte +0x6
00000292  A0AA37            mov al,[0x37aa]
00000295  98                cbw
00000296  D1E0              shl ax,1
00000298  8BD8              mov bx,ax
0000029A  C787D24B0000      mov word [bx+0x4bd2],0x0
000002A0  A0AA37            mov al,[0x37aa]
000002A3  98                cbw
000002A4  BA1A00            mov dx,0x1a
000002A7  F7EA              imul dx
000002A9  8BD8              mov bx,ax
000002AB  C7872033FA00      mov word [bx+0x3320],0xfa
000002B1  A0AA37            mov al,[0x37aa]
000002B4  98                cbw
000002B5  BA0600            mov dx,0x6
000002B8  F7EA              imul dx
000002BA  057800            add ax,0x78
000002BD  50                push ax
000002BE  A0AA37            mov al,[0x37aa]
000002C1  98                cbw
000002C2  BA1A00            mov dx,0x1a
000002C5  F7EA              imul dx
000002C7  8BD8              mov bx,ax
000002C9  58                pop ax
000002CA  89872233          mov [bx+0x3322],ax
000002CE  A0AA37            mov al,[0x37aa]
000002D1  98                cbw
000002D2  D1E0              shl ax,1
000002D4  8BD8              mov bx,ax
000002D6  C787CA4B0100      mov word [bx+0x4bca],0x1
000002DC  A0AA37            mov al,[0x37aa]
000002DF  98                cbw
000002E0  D1E0              shl ax,1
000002E2  8BD8              mov bx,ax
000002E4  C78722303200      mov word [bx+0x3022],0x32
000002EA  9ACC1F0906        call 0x609:0x1fcc
000002EF  FE06AA37          inc byte [0x37aa]
000002F3  803EAA3704        cmp byte [0x37aa],0x4
000002F8  7D03              jnl 0x2fd
000002FA  E979FF            jmp 0x276
000002FD  9A0D06FD21        call 0x21fd:0x60d
00000302  5D                pop bp
00000303  CB                retf

; ---- 20B3:0304 ----
00000304  55                push bp
00000305  8BEC              mov bp,sp
00000307  A0AA37            mov al,[0x37aa]
0000030A  98                cbw
0000030B  D1E0              shl ax,1
0000030D  8BD8              mov bx,ax
0000030F  C78722300A00      mov word [bx+0x3022],0xa
00000315  9ADE04B320        call 0x20b3:0x4de
0000031A  9A2006FD21        call 0x21fd:0x620
0000031F  33C0              xor ax,ax
00000321  50                push ax
00000322  B80600            mov ax,0x6
00000325  50                push ax
00000326  A0AA37            mov al,[0x37aa]
00000329  98                cbw
0000032A  B106              mov cl,0x6
0000032C  D3E0              shl ax,cl
0000032E  052000            add ax,0x20
00000331  50                push ax
00000332  9A79180906        call 0x609:0x1879
00000337  83C406            add sp,byte +0x6
0000033A  9A3406FD21        call 0x21fd:0x634
0000033F  33C0              xor ax,ax
00000341  BA6400            mov dx,0x64
00000344  50                push ax
00000345  52                push dx
00000346  9A8501FD21        call 0x21fd:0x185
0000034B  59                pop cx
0000034C  59                pop cx
0000034D  5D                pop bp
0000034E  CB                retf

; ---- 20B3:034F ----
0000034F  55                push bp
00000350  8BEC              mov bp,sp
00000352  56                push si
00000353  A0AA37            mov al,[0x37aa]
00000356  98                cbw
00000357  BA1A00            mov dx,0x1a
0000035A  F7EA              imul dx
0000035C  BA7800            mov dx,0x78
0000035F  8BD8              mov bx,ax
00000361  2B972233          sub dx,[bx+0x3322]
00000365  8BF2              mov si,dx
00000367  0BF6              or si,si
00000369  7C26              jl 0x391
0000036B  8BC6              mov ax,si
0000036D  BB0200            mov bx,0x2
00000370  99                cwd
00000371  F7FB              idiv bx
00000373  50                push ax
00000374  A0AA37            mov al,[0x37aa]
00000377  98                cbw
00000378  D1E0              shl ax,1
0000037A  8BD8              mov bx,ax
0000037C  58                pop ax
0000037D  89872230          mov [bx+0x3022],ax
00000381  A0AA37            mov al,[0x37aa]
00000384  98                cbw
00000385  D1E0              shl ax,1
00000387  8BD8              mov bx,ax
00000389  C787CA4B0300      mov word [bx+0x4bca],0x3
0000038F  EB26              jmp short 0x3b7
00000391  8BC6              mov ax,si
00000393  F7D8              neg ax
00000395  BB0200            mov bx,0x2
00000398  99                cwd
00000399  F7FB              idiv bx
0000039B  50                push ax
0000039C  A0AA37            mov al,[0x37aa]
0000039F  98                cbw
000003A0  D1E0              shl ax,1
000003A2  8BD8              mov bx,ax
000003A4  58                pop ax
000003A5  89872230          mov [bx+0x3022],ax
000003A9  A0AA37            mov al,[0x37aa]
000003AC  98                cbw
000003AD  D1E0              shl ax,1
000003AF  8BD8              mov bx,ax
000003B1  C787CA4B0200      mov word [bx+0x4bca],0x2
000003B7  9ADE04B320        call 0x20b3:0x4de
000003BC  5E                pop si
000003BD  5D                pop bp
000003BE  CB                retf

; ---- 20B3:03BF ----
000003BF  55                push bp
000003C0  8BEC              mov bp,sp
000003C2  A0AA37            mov al,[0x37aa]
000003C5  98                cbw
000003C6  D1E0              shl ax,1
000003C8  8BD8              mov bx,ax
000003CA  C78722301500      mov word [bx+0x3022],0x15
000003D0  A0AA37            mov al,[0x37aa]
000003D3  98                cbw
000003D4  D1E0              shl ax,1
000003D6  8BD8              mov bx,ax
000003D8  C787CA4B0100      mov word [bx+0x4bca],0x1
000003DE  9ADE04B320        call 0x20b3:0x4de
000003E3  5D                pop bp
000003E4  CB                retf

; ---- 20B3:03E5 ----
000003E5  55                push bp
000003E6  8BEC              mov bp,sp
000003E8  56                push si
000003E9  33F6              xor si,si
000003EB  EB3D              jmp short 0x42a
000003ED  9A2006FD21        call 0x21fd:0x620
000003F2  8BC6              mov ax,si
000003F4  057000            add ax,0x70
000003F7  50                push ax
000003F8  B86300            mov ax,0x63
000003FB  50                push ax
000003FC  B80100            mov ax,0x1
000003FF  50                push ax
00000400  B80800            mov ax,0x8
00000403  50                push ax
00000404  8BC6              mov ax,si
00000406  05E300            add ax,0xe3
00000409  50                push ax
0000040A  B8CD00            mov ax,0xcd
0000040D  50                push ax
0000040E  9A3000960A        call 0xa96:0x30
00000413  83C40C            add sp,byte +0xc
00000416  9A3406FD21        call 0x21fd:0x634
0000041B  33C0              xor ax,ax
0000041D  BA0100            mov dx,0x1
00000420  50                push ax
00000421  52                push dx
00000422  9A8501FD21        call 0x21fd:0x185
00000427  59                pop cx
00000428  59                pop cx
00000429  46                inc si
0000042A  83FE13            cmp si,byte +0x13
0000042D  7EBE              jng 0x3ed
0000042F  5E                pop si
00000430  5D                pop bp
00000431  CB                retf

; ---- 20B3:0432 ----
00000432  55                push bp
00000433  8BEC              mov bp,sp
00000435  56                push si
00000436  A0AA37            mov al,[0x37aa]
00000439  98                cbw
0000043A  50                push ax
0000043B  9A5E04AF21        call 0x21af:0x45e
00000440  59                pop cx
00000441  FF362E08          push word [0x82e]
00000445  9A1C07CD23        call 0x23cd:0x71c
0000044A  59                pop cx
0000044B  FF362E08          push word [0x82e]
0000044F  9AE906CD23        call 0x23cd:0x6e9
00000454  59                pop cx
00000455  33F6              xor si,si
00000457  EB06              jmp short 0x45f
00000459  9A1F05FD21        call 0x21fd:0x51f
0000045E  46                inc si
0000045F  83FE33            cmp si,byte +0x33
00000462  7CF5              jl 0x459
00000464  FF362E08          push word [0x82e]
00000468  9A1C07CD23        call 0x23cd:0x71c
0000046D  59                pop cx
0000046E  FF362E08          push word [0x82e]
00000472  9AE906CD23        call 0x23cd:0x6e9
00000477  59                pop cx
00000478  5E                pop si
00000479  5D                pop bp
0000047A  CB                retf

; ---- 20B3:047B ----
0000047B  55                push bp
0000047C  8BEC              mov bp,sp
0000047E  56                push si
0000047F  9A1F05FD21        call 0x21fd:0x51f
00000484  FF362E08          push word [0x82e]
00000488  9A1C07CD23        call 0x23cd:0x71c
0000048D  59                pop cx
0000048E  33C0              xor ax,ax
00000490  BA0300            mov dx,0x3
00000493  50                push ax
00000494  52                push dx
00000495  9A8501FD21        call 0x21fd:0x185
0000049A  59                pop cx
0000049B  59                pop cx
0000049C  BE0400            mov si,0x4
0000049F  C606AA3700        mov byte [0x37aa],0x0
000004A4  EB1B              jmp short 0x4c1
000004A6  A0AA37            mov al,[0x37aa]
000004A9  98                cbw
000004AA  D1E0              shl ax,1
000004AC  8BD8              mov bx,ax
000004AE  83BF223000        cmp word [bx+0x3022],byte +0x0
000004B3  7E07              jng 0x4bc
000004B5  9AE01F0906        call 0x609:0x1fe0
000004BA  EB01              jmp short 0x4bd
000004BC  4E                dec si
000004BD  FE06AA37          inc byte [0x37aa]
000004C1  803EAA3704        cmp byte [0x37aa],0x4
000004C6  7CDE              jl 0x4a6
000004C8  FF362E08          push word [0x82e]
000004CC  9AE906CD23        call 0x23cd:0x6e9
000004D1  59                pop cx
000004D2  0BF6              or si,si
000004D4  75A9              jnz 0x47f
000004D6  9A6506FD21        call 0x21fd:0x665
000004DB  5E                pop si
000004DC  5D                pop bp
000004DD  CB                retf

; ---- 20B3:04DE ----
000004DE  55                push bp
000004DF  8BEC              mov bp,sp
000004E1  EB2C              jmp short 0x50f
000004E3  9A1F05FD21        call 0x21fd:0x51f
000004E8  FF362E08          push word [0x82e]
000004EC  9A1C07CD23        call 0x23cd:0x71c
000004F1  59                pop cx
000004F2  9AE01F0906        call 0x609:0x1fe0
000004F7  FF362E08          push word [0x82e]
000004FB  9AE906CD23        call 0x23cd:0x6e9
00000500  59                pop cx
00000501  33C0              xor ax,ax
00000503  BA0300            mov dx,0x3
00000506  50                push ax
00000507  52                push dx
00000508  9A8501FD21        call 0x21fd:0x185
0000050D  59                pop cx
0000050E  59                pop cx
0000050F  A0AA37            mov al,[0x37aa]
00000512  98                cbw
00000513  D1E0              shl ax,1
00000515  8BD8              mov bx,ax
00000517  83BF223000        cmp word [bx+0x3022],byte +0x0
0000051C  7FC5              jg 0x4e3
0000051E  9A6506FD21        call 0x21fd:0x665
00000523  5D                pop bp
00000524  CB                retf

; ---- 20B3:0525 ----
00000525  55                push bp
00000526  8BEC              mov bp,sp
00000528  FF36E008          push word [0x8e0]
0000052C  9A02006C18        call 0x186c:0x2
00000531  59                pop cx
00000532  33C0              xor ax,ax
00000534  50                push ax
00000535  9A0D0DB320        call 0x20b3:0xd0d
0000053A  59                pop cx
0000053B  9A1B00960A        call 0xa96:0x1b
00000540  33C0              xor ax,ax
00000542  50                push ax
00000543  33C0              xor ax,ax
00000545  50                push ax
00000546  B8C800            mov ax,0xc8
00000549  50                push ax
0000054A  B84001            mov ax,0x140
0000054D  50                push ax
0000054E  33C0              xor ax,ax
00000550  50                push ax
00000551  33C0              xor ax,ax
00000553  50                push ax
00000554  9A3000960A        call 0xa96:0x30
00000559  83C40C            add sp,byte +0xc
0000055C  33C0              xor ax,ax
0000055E  50                push ax
0000055F  B81400            mov ax,0x14
00000562  50                push ax
00000563  B80500            mov ax,0x5
00000566  50                push ax
00000567  B88F00            mov ax,0x8f
0000056A  50                push ax
0000056B  33C0              xor ax,ax
0000056D  50                push ax
0000056E  9AD704960A        call 0xa96:0x4d7
00000573  83C40A            add sp,byte +0xa
00000576  9A0D06FD21        call 0x21fd:0x60d
0000057B  1E                push ds
0000057C  B86010            mov ax,0x1060
0000057F  50                push ax
00000580  9A6C02FD21        call 0x21fd:0x26c
00000585  59                pop cx
00000586  59                pop cx
00000587  9A2903FD21        call 0x21fd:0x329
0000058C  33C0              xor ax,ax
0000058E  BA6400            mov dx,0x64
00000591  50                push ax
00000592  52                push dx
00000593  9A8501FD21        call 0x21fd:0x185
00000598  59                pop cx
00000599  59                pop cx
0000059A  9AB405B320        call 0x20b3:0x5b4
0000059F  9ABA07B320        call 0x20b3:0x7ba
000005A4  33C0              xor ax,ax
000005A6  BA3200            mov dx,0x32
000005A9  50                push ax
000005AA  52                push dx
000005AB  9A8501FD21        call 0x21fd:0x185
000005B0  59                pop cx
000005B1  59                pop cx
000005B2  5D                pop bp
000005B3  CB                retf

; ---- 20B3:05B4 ----
000005B4  55                push bp
000005B5  8BEC              mov bp,sp
000005B7  56                push si
000005B8  33F6              xor si,si
000005BA  E9CD00            jmp 0x68a
000005BD  C606AA3700        mov byte [0x37aa],0x0
000005C2  E9BA00            jmp 0x67f
000005C5  A0AA37            mov al,[0x37aa]
000005C8  98                cbw
000005C9  8BD8              mov bx,ax
000005CB  8A87423D          mov al,[bx+0x3d42]
000005CF  B400              mov ah,0x0
000005D1  3BC6              cmp ax,si
000005D3  7403              jz 0x5d8
000005D5  E9A300            jmp 0x67b
000005D8  9A1B00960A        call 0xa96:0x1b
000005DD  B80900            mov ax,0x9
000005E0  50                push ax
000005E1  B80D00            mov ax,0xd
000005E4  50                push ax
000005E5  B82400            mov ax,0x24
000005E8  50                push ax
000005E9  B80600            mov ax,0x6
000005EC  50                push ax
000005ED  33C0              xor ax,ax
000005EF  50                push ax
000005F0  9AD704960A        call 0xa96:0x4d7
000005F5  83C40A            add sp,byte +0xa
000005F8  B8FFFF            mov ax,0xffff
000005FB  50                push ax
000005FC  B80100            mov ax,0x1
000005FF  50                push ax
00000600  1E                push ds
00000601  B88F10            mov ax,0x108f
00000604  50                push ax
00000605  B80A00            mov ax,0xa
00000608  50                push ax
00000609  33C0              xor ax,ax
0000060B  50                push ax
0000060C  9A2505960A        call 0xa96:0x525
00000611  83C40C            add sp,byte +0xc
00000614  B8FFFF            mov ax,0xffff
00000617  50                push ax
00000618  B80100            mov ax,0x1
0000061B  50                push ax
0000061C  B80100            mov ax,0x1
0000061F  50                push ax
00000620  8BC6              mov ax,si
00000622  40                inc ax
00000623  99                cwd
00000624  52                push dx
00000625  50                push ax
00000626  B80A00            mov ax,0xa
00000629  50                push ax
0000062A  B81E00            mov ax,0x1e
0000062D  50                push ax
0000062E  9A7A05960A        call 0xa96:0x57a
00000633  83C40E            add sp,byte +0xe
00000636  B8FFFF            mov ax,0xffff
00000639  50                push ax
0000063A  B80100            mov ax,0x1
0000063D  50                push ax
0000063E  B80500            mov ax,0x5
00000641  50                push ax
00000642  A0AA37            mov al,[0x37aa]
00000645  98                cbw
00000646  D1E0              shl ax,1
00000648  D1E0              shl ax,1
0000064A  8BD8              mov bx,ax
0000064C  FFB7082F          push word [bx+0x2f08]
00000650  FFB7062F          push word [bx+0x2f06]
00000654  B81200            mov ax,0x12
00000657  50                push ax
00000658  33C0              xor ax,ax
0000065A  50                push ax
0000065B  9AFB05960A        call 0xa96:0x5fb
00000660  83C40E            add sp,byte +0xe
00000663  9AA806960A        call 0xa96:0x6a8
00000668  9A9A06B320        call 0x20b3:0x69a
0000066D  33C0              xor ax,ax
0000066F  BAC800            mov dx,0xc8
00000672  50                push ax
00000673  52                push dx
00000674  9A8501FD21        call 0x21fd:0x185
00000679  59                pop cx
0000067A  59                pop cx
0000067B  FE06AA37          inc byte [0x37aa]
0000067F  803EAA3704        cmp byte [0x37aa],0x4
00000684  7D03              jnl 0x689
00000686  E93CFF            jmp 0x5c5
00000689  46                inc si
0000068A  83FE03            cmp si,byte +0x3
0000068D  7F03              jg 0x692
0000068F  E92BFF            jmp 0x5bd
00000692  9A9A06B320        call 0x20b3:0x69a
00000697  5E                pop si
00000698  5D                pop bp
00000699  CB                retf

; ---- 20B3:069A ----
0000069A  55                push bp
0000069B  8BEC              mov bp,sp
0000069D  83EC0A            sub sp,byte +0xa
000006A0  56                push si
000006A1  57                push di
000006A2  833E0E0901        cmp word [0x90e],byte +0x1
000006A7  7507              jnz 0x6b0
000006A9  C746F60200        mov word [bp-0xa],0x2
000006AE  EB05              jmp short 0x6b5
000006B0  C746F60600        mov word [bp-0xa],0x6
000006B5  C746FE0000        mov word [bp-0x2],0x0
000006BA  E9C800            jmp 0x785
000006BD  B82300            mov ax,0x23
000006C0  50                push ax
000006C1  9ABA01FD21        call 0x21fd:0x1ba
000006C6  59                pop cx
000006C7  8BF8              mov di,ax
000006C9  B80C00            mov ax,0xc
000006CC  50                push ax
000006CD  9ABA01FD21        call 0x21fd:0x1ba
000006D2  59                pop cx
000006D3  8BF0              mov si,ax
000006D5  8BC6              mov ax,si
000006D7  050600            add ax,0x6
000006DA  50                push ax
000006DB  57                push di
000006DC  9ADE06CD23        call 0x23cd:0x6de
000006E1  59                pop cx
000006E2  59                pop cx
000006E3  8946F8            mov [bp-0x8],ax
000006E6  3B46F6            cmp ax,[bp-0xa]
000006E9  7402              jz 0x6ed
000006EB  EB02              jmp short 0x6ef
000006ED  EBCE              jmp short 0x6bd
000006EF  837EF801          cmp word [bp-0x8],byte +0x1
000006F3  7516              jnz 0x70b
000006F5  C746FC0000        mov word [bp-0x4],0x0
000006FA  A0AA37            mov al,[0x37aa]
000006FD  98                cbw
000006FE  BA0500            mov dx,0x5
00000701  F7EA              imul dx
00000703  058F00            add ax,0x8f
00000706  8946FA            mov [bp-0x6],ax
00000709  EB1A              jmp short 0x725
0000070B  8BC7              mov ax,di
0000070D  BA0500            mov dx,0x5
00000710  F7EA              imul dx
00000712  058800            add ax,0x88
00000715  8946FC            mov [bp-0x4],ax
00000718  8BC6              mov ax,si
0000071A  BA0500            mov dx,0x5
0000071D  F7EA              imul dx
0000071F  051400            add ax,0x14
00000722  8946FA            mov [bp-0x6],ax
00000725  9AD401670B        call 0xb67:0x1d4
0000072A  9A1B00960A        call 0xa96:0x1b
0000072F  8BC6              mov ax,si
00000731  BA0500            mov dx,0x5
00000734  F7EA              imul dx
00000736  051400            add ax,0x14
00000739  50                push ax
0000073A  8BC7              mov ax,di
0000073C  BA0500            mov dx,0x5
0000073F  F7EA              imul dx
00000741  058800            add ax,0x88
00000744  50                push ax
00000745  B80500            mov ax,0x5
00000748  50                push ax
00000749  B80500            mov ax,0x5
0000074C  50                push ax
0000074D  FF76FA            push word [bp-0x6]
00000750  FF76FC            push word [bp-0x4]
00000753  9A3000960A        call 0xa96:0x30
00000758  83C40C            add sp,byte +0xc
0000075B  9AA806960A        call 0xa96:0x6a8
00000760  9A0602670B        call 0xb67:0x206
00000765  9A1B00960A        call 0xa96:0x1b
0000076A  B80200            mov ax,0x2
0000076D  50                push ax
0000076E  8BC6              mov ax,si
00000770  050600            add ax,0x6
00000773  50                push ax
00000774  57                push di
00000775  9AFB03960A        call 0xa96:0x3fb
0000077A  83C406            add sp,byte +0x6
0000077D  9AA806960A        call 0xa96:0x6a8
00000782  FF46FE            inc word [bp-0x2]
00000785  817EFED401        cmp word [bp-0x2],0x1d4
0000078A  7D03              jnl 0x78f
0000078C  E92EFF            jmp 0x6bd
0000078F  9A1B00960A        call 0xa96:0x1b
00000794  B80900            mov ax,0x9
00000797  50                push ax
00000798  B80D00            mov ax,0xd
0000079B  50                push ax
0000079C  B82400            mov ax,0x24
0000079F  50                push ax
000007A0  B80600            mov ax,0x6
000007A3  50                push ax
000007A4  33C0              xor ax,ax
000007A6  50                push ax
000007A7  9AD704960A        call 0xa96:0x4d7
000007AC  83C40A            add sp,byte +0xa
000007AF  9AA806960A        call 0xa96:0x6a8
000007B4  5F                pop di
000007B5  5E                pop si
000007B6  8BE5              mov sp,bp
000007B8  5D                pop bp
000007B9  CB                retf

; ---- 20B3:07BA ----
000007BA  55                push bp
000007BB  8BEC              mov bp,sp
000007BD  56                push si
000007BE  9A650DB320        call 0x20b3:0xd65
000007C3  B80500            mov ax,0x5
000007C6  50                push ax
000007C7  9A770DB320        call 0x20b3:0xd77
000007CC  59                pop cx
000007CD  BE7800            mov si,0x78
000007D0  EB72              jmp short 0x844
000007D2  9A1F05FD21        call 0x21fd:0x51f
000007D7  FF362E08          push word [0x82e]
000007DB  9A1C07CD23        call 0x23cd:0x71c
000007E0  59                pop cx
000007E1  33C0              xor ax,ax
000007E3  BA0200            mov dx,0x2
000007E6  50                push ax
000007E7  52                push dx
000007E8  9A8501FD21        call 0x21fd:0x185
000007ED  59                pop cx
000007EE  59                pop cx
000007EF  56                push si
000007F0  9AF00BB320        call 0x20b3:0xbf0
000007F5  59                pop cx
000007F6  BB0800            mov bx,0x8
000007F9  99                cwd
000007FA  F7FB              idiv bx
000007FC  05A000            add ax,0xa0
000007FF  A32033            mov [0x3320],ax
00000802  56                push si
00000803  9A7D0BB320        call 0x20b3:0xb7d
00000808  59                pop cx
00000809  BB0B00            mov bx,0xb
0000080C  99                cwd
0000080D  F7FB              idiv bx
0000080F  BA6400            mov dx,0x64
00000812  2BD0              sub dx,ax
00000814  89162233          mov [0x3322],dx
00000818  83FE32            cmp si,byte +0x32
0000081B  7D1C              jnl 0x839
0000081D  8BC6              mov ax,si
0000081F  BB0A00            mov bx,0xa
00000822  99                cwd
00000823  F7FB              idiv bx
00000825  83FA09            cmp dx,byte +0x9
00000828  750F              jnz 0x839
0000082A  8BC6              mov ax,si
0000082C  BB0A00            mov bx,0xa
0000082F  99                cwd
00000830  F7FB              idiv bx
00000832  50                push ax
00000833  9A770DB320        call 0x20b3:0xd77
00000838  59                pop cx
00000839  FF362E08          push word [0x82e]
0000083D  9AE906CD23        call 0x23cd:0x6e9
00000842  59                pop cx
00000843  4E                dec si
00000844  0BF6              or si,si
00000846  7D8A              jnl 0x7d2
00000848  33C0              xor ax,ax
0000084A  50                push ax
0000084B  9A5E04AF21        call 0x21af:0x45e
00000850  59                pop cx
00000851  FF362E08          push word [0x82e]
00000855  9A1C07CD23        call 0x23cd:0x71c
0000085A  59                pop cx
0000085B  9A1F05FD21        call 0x21fd:0x51f
00000860  5E                pop si
00000861  5D                pop bp
00000862  CB                retf

; ---- 20B3:0863 ----
00000863  55                push bp
00000864  8BEC              mov bp,sp
00000866  56                push si
00000867  57                push di
00000868  BF9001            mov di,0x190
0000086B  BEEE02            mov si,0x2ee
0000086E  FF36DC08          push word [0x8dc]
00000872  9A02006C18        call 0x186c:0x2
00000877  59                pop cx
00000878  33C0              xor ax,ax
0000087A  50                push ax
0000087B  9A7D03FD21        call 0x21fd:0x37d
00000880  59                pop cx
00000881  B80100            mov ax,0x1
00000884  50                push ax
00000885  9A0D0DB320        call 0x20b3:0xd0d
0000088A  59                pop cx
0000088B  9A1B00960A        call 0xa96:0x1b
00000890  33C0              xor ax,ax
00000892  50                push ax
00000893  33C0              xor ax,ax
00000895  50                push ax
00000896  B8C800            mov ax,0xc8
00000899  50                push ax
0000089A  B84001            mov ax,0x140
0000089D  50                push ax
0000089E  B81400            mov ax,0x14
000008A1  50                push ax
000008A2  33C0              xor ax,ax
000008A4  50                push ax
000008A5  9A3000960A        call 0xa96:0x30
000008AA  83C40C            add sp,byte +0xc
000008AD  9A0D06FD21        call 0x21fd:0x60d
000008B2  A18010            mov ax,[0x1080]
000008B5  BB0D00            mov bx,0xd
000008B8  99                cwd
000008B9  F7FB              idiv bx
000008BB  50                push ax
000008BC  9A770DB320        call 0x20b3:0xd77
000008C1  59                pop cx
000008C2  9A650DB320        call 0x20b3:0xd65
000008C7  1E                push ds
000008C8  B84010            mov ax,0x1040
000008CB  50                push ax
000008CC  9A6C02FD21        call 0x21fd:0x26c
000008D1  59                pop cx
000008D2  59                pop cx
000008D3  9A2903FD21        call 0x21fd:0x329
000008D8  C706D4540000      mov word [0x54d4],0x0
000008DE  9A1B00960A        call 0xa96:0x1b
000008E3  EB7B              jmp short 0x960
000008E5  9A1F05FD21        call 0x21fd:0x51f
000008EA  FF362E08          push word [0x82e]
000008EE  9A1C07CD23        call 0x23cd:0x71c
000008F3  59                pop cx
000008F4  9AA806960A        call 0xa96:0x6a8
000008F9  9A1B00960A        call 0xa96:0x1b
000008FE  9A8609B320        call 0x20b3:0x986
00000903  8B1ED454          mov bx,[0x54d4]
00000907  83FB06            cmp bx,byte +0x6
0000090A  7745              ja 0x951
0000090C  D1E3              shl bx,1
0000090E  2EFFA77809        jmp [cs:bx+0x978]
00000913  8BC7              mov ax,di
00000915  48                dec ax
00000916  8BF8              mov di,ax
00000918  0BC0              or ax,ax
0000091A  7506              jnz 0x922
0000091C  C706D4540100      mov word [0x54d4],0x1
00000922  EB2D              jmp short 0x951
00000924  9AD409B320        call 0x20b3:0x9d4
00000929  EB26              jmp short 0x951
0000092B  9A9F0AB320        call 0x20b3:0xa9f
00000930  EB1F              jmp short 0x951
00000932  9ADC0AB320        call 0x20b3:0xadc
00000937  EB18              jmp short 0x951
00000939  9A400BB320        call 0x20b3:0xb40
0000093E  EB11              jmp short 0x951
00000940  8BC6              mov ax,si
00000942  48                dec ax
00000943  8BF0              mov si,ax
00000945  0BC0              or ax,ax
00000947  7506              jnz 0x94f
00000949  C706D4540700      mov word [0x54d4],0x7
0000094F  EB00              jmp short 0x951
00000951  9AA806960A        call 0xa96:0x6a8
00000956  FF362E08          push word [0x82e]
0000095A  9AE906CD23        call 0x23cd:0x6e9
0000095F  59                pop cx
00000960  833ED45407        cmp word [0x54d4],byte +0x7
00000965  7403              jz 0x96a
00000967  E97BFF            jmp 0x8e5
0000096A  9A6506FD21        call 0x21fd:0x665
0000096F  9A050CB320        call 0x20b3:0xc05
00000974  5F                pop di
00000975  5E                pop si
00000976  5D                pop bp
00000977  CB                retf
00000978  1309              adc cx,[bx+di]
0000097A  2409              and al,0x9
0000097C  2B09              sub cx,[bx+di]
0000097E  3209              xor cl,[bx+di]
00000980  3909              cmp [bx+di],cx
00000982  2409              and al,0x9
00000984  40                inc ax
00000985  09558B            or [di-0x75],dx
00000988  EC                in al,dx
00000989  A18210            mov ax,[0x1082]
0000098C  48                dec ax
0000098D  A38210            mov [0x1082],ax
00000990  0BC0              or ax,ax
00000992  7E02              jng 0x996
00000994  EB3C              jmp short 0x9d2
00000996  A08410            mov al,[0x1084]
00000999  FEC0              inc al
0000099B  A28410            mov [0x1084],al
0000099E  3C03              cmp al,0x3
000009A0  7E05              jng 0x9a7
000009A2  C606841000        mov byte [0x1084],0x0
000009A7  B8A100            mov ax,0xa1
000009AA  50                push ax
000009AB  B8D600            mov ax,0xd6
000009AE  50                push ax
000009AF  B81500            mov ax,0x15
000009B2  50                push ax
000009B3  B81000            mov ax,0x10
000009B6  50                push ax
000009B7  B8EB00            mov ax,0xeb
000009BA  50                push ax
000009BB  A08410            mov al,[0x1084]
000009BE  98                cbw
000009BF  B104              mov cl,0x4
000009C1  D3E0              shl ax,cl
000009C3  50                push ax
000009C4  9A3000960A        call 0xa96:0x30
000009C9  83C40C            add sp,byte +0xc
000009CC  C70682100600      mov word [0x1082],0x6
000009D2  5D                pop bp
000009D3  CB                retf

; ---- 20B3:09D4 ----
000009D4  55                push bp
000009D5  8BEC              mov bp,sp
000009D7  56                push si
000009D8  833ED45401        cmp word [0x54d4],byte +0x1
000009DD  7506              jnz 0x9e5
000009DF  FF068010          inc word [0x1080]
000009E3  EB04              jmp short 0x9e9
000009E5  FF0E8010          dec word [0x1080]
000009E9  833E801000        cmp word [0x1080],byte +0x0
000009EE  7D12              jnl 0xa02
000009F0  33C0              xor ax,ax
000009F2  50                push ax
000009F3  9A5E04AF21        call 0x21af:0x45e
000009F8  59                pop cx
000009F9  C706D4540600      mov word [0x54d4],0x6
000009FF  E99A00            jmp 0xa9c
00000A02  833E801044        cmp word [0x1080],byte +0x44
00000A07  7D2F              jnl 0xa38
00000A09  A18010            mov ax,[0x1080]
00000A0C  051100            add ax,0x11
00000A0F  A32033            mov [0x3320],ax
00000A12  C70622337300      mov word [0x3322],0x73
00000A18  A18010            mov ax,[0x1080]
00000A1B  BB0B00            mov bx,0xb
00000A1E  99                cwd
00000A1F  F7FB              idiv bx
00000A21  83FA05            cmp dx,byte +0x5
00000A24  7510              jnz 0xa36
00000A26  A18010            mov ax,[0x1080]
00000A29  BB0B00            mov bx,0xb
00000A2C  99                cwd
00000A2D  F7FB              idiv bx
00000A2F  50                push ax
00000A30  9A770DB320        call 0x20b3:0xd77
00000A35  59                pop cx
00000A36  EB64              jmp short 0xa9c
00000A38  813E8010D901      cmp word [0x1080],0x1d9
00000A3E  7D3D              jnl 0xa7d
00000A40  A18010            mov ax,[0x1080]
00000A43  05BCFF            add ax,0xffbc
00000A46  D1E0              shl ax,1
00000A48  BAB400            mov dx,0xb4
00000A4B  2BD0              sub dx,ax
00000A4D  8BF2              mov si,dx
00000A4F  56                push si
00000A50  9AF00BB320        call 0x20b3:0xbf0
00000A55  59                pop cx
00000A56  BB2100            mov bx,0x21
00000A59  99                cwd
00000A5A  F7FB              idiv bx
00000A5C  055500            add ax,0x55
00000A5F  A32033            mov [0x3320],ax
00000A62  56                push si
00000A63  9A7D0BB320        call 0x20b3:0xb7d
00000A68  59                pop cx
00000A69  BB2100            mov bx,0x21
00000A6C  99                cwd
00000A6D  F7FB              idiv bx
00000A6F  BA6900            mov dx,0x69
00000A72  2BD0              sub dx,ax
00000A74  83C2EC            add dx,byte -0x14
00000A77  89162233          mov [0x3322],dx
00000A7B  EB1F              jmp short 0xa9c
00000A7D  813E8010EC01      cmp word [0x1080],0x1ec
00000A83  7D11              jnl 0xa96
00000A85  A18010            mov ax,[0x1080]
00000A88  059AFE            add ax,0xfe9a
00000A8B  A32033            mov [0x3320],ax
00000A8E  C70622335500      mov word [0x3322],0x55
00000A94  EB06              jmp short 0xa9c
00000A96  C706D4540200      mov word [0x54d4],0x2
00000A9C  5E                pop si
00000A9D  5D                pop bp
00000A9E  CB                retf

; ---- 20B3:0A9F ----
00000A9F  55                push bp
00000AA0  8BEC              mov bp,sp
00000AA2  A18510            mov ax,[0x1085]
00000AA5  48                dec ax
00000AA6  A38510            mov [0x1085],ax
00000AA9  3D0A00            cmp ax,0xa
00000AAC  750C              jnz 0xaba
00000AAE  B80600            mov ax,0x6
00000AB1  50                push ax
00000AB2  9A770DB320        call 0x20b3:0xd77
00000AB7  59                pop cx
00000AB8  EB20              jmp short 0xada
00000ABA  833E851005        cmp word [0x1085],byte +0x5
00000ABF  750C              jnz 0xacd
00000AC1  B80700            mov ax,0x7
00000AC4  50                push ax
00000AC5  9A770DB320        call 0x20b3:0xd77
00000ACA  59                pop cx
00000ACB  EB0D              jmp short 0xada
00000ACD  833E851000        cmp word [0x1085],byte +0x0
00000AD2  7506              jnz 0xada
00000AD4  C706D4540300      mov word [0x54d4],0x3
00000ADA  5D                pop bp
00000ADB  CB                retf

; ---- 20B3:0ADC ----
00000ADC  55                push bp
00000ADD  8BEC              mov bp,sp
00000ADF  33C0              xor ax,ax
00000AE1  50                push ax
00000AE2  B85B00            mov ax,0x5b
00000AE5  50                push ax
00000AE6  A18710            mov ax,[0x1087]
00000AE9  BB1400            mov bx,0x14
00000AEC  99                cwd
00000AED  F7FB              idiv bx
00000AEF  059100            add ax,0x91
00000AF2  50                push ax
00000AF3  9AFB03960A        call 0xa96:0x3fb
00000AF8  83C406            add sp,byte +0x6
00000AFB  B80100            mov ax,0x1
00000AFE  50                push ax
00000AFF  B85B00            mov ax,0x5b
00000B02  50                push ax
00000B03  A18710            mov ax,[0x1087]
00000B06  BB1400            mov bx,0x14
00000B09  99                cwd
00000B0A  F7FB              idiv bx
00000B0C  059000            add ax,0x90
00000B0F  50                push ax
00000B10  9AFB03960A        call 0xa96:0x3fb
00000B15  83C406            add sp,byte +0x6
00000B18  A18710            mov ax,[0x1087]
00000B1B  40                inc ax
00000B1C  A38710            mov [0x1087],ax
00000B1F  3D6400            cmp ax,0x64
00000B22  7E1A              jng 0xb3e
00000B24  A0D654            mov al,[0x54d6]
00000B27  FEC8              dec al
00000B29  A2D654            mov [0x54d6],al
00000B2C  0AC0              or al,al
00000B2E  7E08              jng 0xb38
00000B30  C70687100000      mov word [0x1087],0x0
00000B36  EB06              jmp short 0xb3e
00000B38  C706D4540400      mov word [0x54d4],0x4
00000B3E  5D                pop bp
00000B3F  CB                retf

; ---- 20B3:0B40 ----
00000B40  55                push bp
00000B41  8BEC              mov bp,sp
00000B43  A18910            mov ax,[0x1089]
00000B46  48                dec ax
00000B47  A38910            mov [0x1089],ax
00000B4A  3D0A00            cmp ax,0xa
00000B4D  750C              jnz 0xb5b
00000B4F  B80600            mov ax,0x6
00000B52  50                push ax
00000B53  9A770DB320        call 0x20b3:0xd77
00000B58  59                pop cx
00000B59  EB20              jmp short 0xb7b
00000B5B  833E891005        cmp word [0x1089],byte +0x5
00000B60  750C              jnz 0xb6e
00000B62  B80500            mov ax,0x5
00000B65  50                push ax
00000B66  9A770DB320        call 0x20b3:0xd77
00000B6B  59                pop cx
00000B6C  EB0D              jmp short 0xb7b
00000B6E  833E891000        cmp word [0x1089],byte +0x0
00000B73  7506              jnz 0xb7b
00000B75  C706D4540500      mov word [0x54d4],0x5
00000B7B  5D                pop bp
00000B7C  CB                retf

; ---- 20B3:0B7D ----
00000B7D  55                push bp
00000B7E  8BEC              mov bp,sp
00000B80  56                push si
00000B81  8B7606            mov si,[bp+0x6]
00000B84  EB07              jmp short 0xb8d
00000B86  8BC6              mov ax,si
00000B88  0598FE            add ax,0xfe98
00000B8B  8BF0              mov si,ax
00000B8D  81FE6801          cmp si,0x168
00000B91  7DF3              jnl 0xb86
00000B93  EB07              jmp short 0xb9c
00000B95  8BC6              mov ax,si
00000B97  056801            add ax,0x168
00000B9A  8BF0              mov si,ax
00000B9C  0BF6              or si,si
00000B9E  7CF5              jl 0xb95
00000BA0  83FE5A            cmp si,byte +0x5a
00000BA3  7D0A              jnl 0xbaf
00000BA5  8BDE              mov bx,si
00000BA7  D1E3              shl bx,1
00000BA9  8B87170D          mov ax,[bx+0xd17]
00000BAD  EB3E              jmp short 0xbed
00000BAF  81FEB400          cmp si,0xb4
00000BB3  7D14              jnl 0xbc9
00000BB5  8BC6              mov ax,si
00000BB7  05A6FF            add ax,0xffa6
00000BBA  BB5A00            mov bx,0x5a
00000BBD  2BD8              sub bx,ax
00000BBF  D1E3              shl bx,1
00000BC1  8B87170D          mov ax,[bx+0xd17]
00000BC5  F7D8              neg ax
00000BC7  EB24              jmp short 0xbed
00000BC9  81FE0E01          cmp si,0x10e
00000BCD  7D0C              jnl 0xbdb
00000BCF  8BDE              mov bx,si
00000BD1  D1E3              shl bx,1
00000BD3  8B87AF0B          mov ax,[bx+0xbaf]
00000BD7  F7D8              neg ax
00000BD9  EB12              jmp short 0xbed
00000BDB  8BC6              mov ax,si
00000BDD  05F2FE            add ax,0xfef2
00000BE0  BB5A00            mov bx,0x5a
00000BE3  2BD8              sub bx,ax
00000BE5  D1E3              shl bx,1
00000BE7  8B87170D          mov ax,[bx+0xd17]
00000BEB  EB00              jmp short 0xbed
00000BED  5E                pop si
00000BEE  5D                pop bp
00000BEF  CB                retf

; ---- 20B3:0BF0 ----
00000BF0  55                push bp
00000BF1  8BEC              mov bp,sp
00000BF3  8B4606            mov ax,[bp+0x6]
00000BF6  055A00            add ax,0x5a
00000BF9  50                push ax
00000BFA  0E                push cs
00000BFB  E87FFF            call 0xb7d
00000BFE  59                pop cx
00000BFF  F7D8              neg ax
00000C01  EB00              jmp short 0xc03
00000C03  5D                pop bp
00000C04  CB                retf

; ---- 20B3:0C05 ----
00000C05  55                push bp
00000C06  8BEC              mov bp,sp
00000C08  56                push si
00000C09  57                push di
00000C0A  33FF              xor di,di
00000C0C  9A9005FD21        call 0x21fd:0x590
00000C11  33F6              xor si,si
00000C13  EB36              jmp short 0xc4b
00000C15  9A1B00960A        call 0xa96:0x1b
00000C1A  33C0              xor ax,ax
00000C1C  50                push ax
00000C1D  B8C800            mov ax,0xc8
00000C20  50                push ax
00000C21  33C0              xor ax,ax
00000C23  50                push ax
00000C24  56                push si
00000C25  9A2C00690A        call 0xa69:0x2c
00000C2A  83C408            add sp,byte +0x8
00000C2D  33C0              xor ax,ax
00000C2F  50                push ax
00000C30  B8C800            mov ax,0xc8
00000C33  50                push ax
00000C34  33C0              xor ax,ax
00000C36  50                push ax
00000C37  B83F01            mov ax,0x13f
00000C3A  2BC6              sub ax,si
00000C3C  50                push ax
00000C3D  9A2C00690A        call 0xa69:0x2c
00000C42  83C408            add sp,byte +0x8
00000C45  9AA806960A        call 0xa96:0x6a8
00000C4A  46                inc si
00000C4B  83FE3B            cmp si,byte +0x3b
00000C4E  7EC5              jng 0xc15
00000C50  33F6              xor si,si
00000C52  E99800            jmp 0xced
00000C55  9A1B00960A        call 0xa96:0x1b
00000C5A  33C0              xor ax,ax
00000C5C  50                push ax
00000C5D  56                push si
00000C5E  B80301            mov ax,0x103
00000C61  50                push ax
00000C62  33C0              xor ax,ax
00000C64  50                push ax
00000C65  B83C00            mov ax,0x3c
00000C68  50                push ax
00000C69  9A3304960A        call 0xa96:0x433
00000C6E  83C40A            add sp,byte +0xa
00000C71  33C0              xor ax,ax
00000C73  50                push ax
00000C74  B8C700            mov ax,0xc7
00000C77  50                push ax
00000C78  B80301            mov ax,0x103
00000C7B  2BC6              sub ax,si
00000C7D  50                push ax
00000C7E  33C0              xor ax,ax
00000C80  50                push ax
00000C81  B80301            mov ax,0x103
00000C84  50                push ax
00000C85  9A3304960A        call 0xa96:0x433
00000C8A  83C40A            add sp,byte +0xa
00000C8D  33C0              xor ax,ax
00000C8F  50                push ax
00000C90  B8C700            mov ax,0xc7
00000C93  2BC6              sub ax,si
00000C95  50                push ax
00000C96  B83C00            mov ax,0x3c
00000C99  50                push ax
00000C9A  B8C700            mov ax,0xc7
00000C9D  50                push ax
00000C9E  B80301            mov ax,0x103
00000CA1  50                push ax
00000CA2  9A3304960A        call 0xa96:0x433
00000CA7  83C40A            add sp,byte +0xa
00000CAA  33C0              xor ax,ax
00000CAC  50                push ax
00000CAD  33C0              xor ax,ax
00000CAF  50                push ax
00000CB0  8BC6              mov ax,si
00000CB2  053C00            add ax,0x3c
00000CB5  50                push ax
00000CB6  B8C700            mov ax,0xc7
00000CB9  50                push ax
00000CBA  B83C00            mov ax,0x3c
00000CBD  50                push ax
00000CBE  9A3304960A        call 0xa96:0x433
00000CC3  83C40A            add sp,byte +0xa
00000CC6  83FE50            cmp si,byte +0x50
00000CC9  7C1C              jl 0xce7
00000CCB  8BC6              mov ax,si
00000CCD  BB1000            mov bx,0x10
00000CD0  99                cwd
00000CD1  F7FB              idiv bx
00000CD3  0BD2              or dx,dx
00000CD5  7510              jnz 0xce7
00000CD7  8BC7              mov ax,di
00000CD9  40                inc ax
00000CDA  8BF8              mov di,ax
00000CDC  50                push ax
00000CDD  33C0              xor ax,ax
00000CDF  50                push ax
00000CE0  9AEB03FD21        call 0x21fd:0x3eb
00000CE5  59                pop cx
00000CE6  59                pop cx
00000CE7  9AA806960A        call 0xa96:0x6a8
00000CEC  46                inc si
00000CED  81FEC700          cmp si,0xc7
00000CF1  7F03              jg 0xcf6
00000CF3  E95FFF            jmp 0xc55
00000CF6  33C0              xor ax,ax
00000CF8  50                push ax
00000CF9  9ADA04FD21        call 0x21fd:0x4da
00000CFE  59                pop cx
00000CFF  9A1F05FD21        call 0x21fd:0x51f
00000D04  9ACD04AF21        call 0x21af:0x4cd
00000D09  5F                pop di
00000D0A  5E                pop si
00000D0B  5D                pop bp
00000D0C  CB                retf

; ---- 20B3:0D0D ----
00000D0D  55                push bp
00000D0E  8BEC              mov bp,sp
00000D10  56                push si
00000D11  C7060E330700      mov word [0x330e],0x7
00000D17  B80700            mov ax,0x7
00000D1A  50                push ax
00000D1B  B82001            mov ax,0x120
00000D1E  50                push ax
00000D1F  33C0              xor ax,ax
00000D21  50                push ax
00000D22  B82001            mov ax,0x120
00000D25  50                push ax
00000D26  33C0              xor ax,ax
00000D28  50                push ax
00000D29  9A2B02AF21        call 0x21af:0x22b
00000D2E  83C40A            add sp,byte +0xa
00000D31  33F6              xor si,si
00000D33  EB28              jmp short 0xd5d
00000D35  B83700            mov ax,0x37
00000D38  50                push ax
00000D39  8BC6              mov ax,si
00000D3B  B104              mov cl,0x4
00000D3D  D3E0              shl ax,cl
00000D3F  50                push ax
00000D40  8B5E06            mov bx,[bp+0x6]
00000D43  D1E3              shl bx,1
00000D45  FFB78B10          push word [bx+0x108b]
00000D49  8BC6              mov ax,si
00000D4B  B104              mov cl,0x4
00000D4D  D3E0              shl ax,cl
00000D4F  50                push ax
00000D50  56                push si
00000D51  33C0              xor ax,ax
00000D53  50                push ax
00000D54  9ACA00AF21        call 0x21af:0xca
00000D59  83C40C            add sp,byte +0xc
00000D5C  46                inc si
00000D5D  83FE07            cmp si,byte +0x7
00000D60  7ED3              jng 0xd35
00000D62  5E                pop si
00000D63  5D                pop bp
00000D64  CB                retf

; ---- 20B3:0D65 ----
00000D65  55                push bp
00000D66  8BEC              mov bp,sp
00000D68  33C0              xor ax,ax
00000D6A  50                push ax
00000D6B  33C0              xor ax,ax
00000D6D  50                push ax
00000D6E  9A1204AF21        call 0x21af:0x412
00000D73  59                pop cx
00000D74  59                pop cx
00000D75  5D                pop bp
00000D76  CB                retf

; ---- 20B3:0D77 ----
00000D77  55                push bp
00000D78  8BEC              mov bp,sp
00000D7A  8B4606            mov ax,[bp+0x6]
00000D7D  A32433            mov [0x3324],ax
