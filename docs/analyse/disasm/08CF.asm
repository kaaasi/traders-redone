; segment 08CF  file offset 0xC2F0  size 0x19A0
00000000  8BE5              mov sp,bp
00000002  5D                pop bp
00000003  CB                retf

; ---- 08CF:0004 ----
00000004  55                push bp
00000005  8BEC              mov bp,sp
00000007  FF36CE08          push word [0x8ce]
0000000B  9A02006C18        call 0x186c:0x2
00000010  59                pop cx
00000011  9A2D1F0906        call 0x609:0x1f2d
00000016  9A8200CF08        call 0x8cf:0x82
0000001B  B82800            mov ax,0x28
0000001E  50                push ax
0000001F  A0F808            mov al,[0x8f8]
00000022  B400              mov ah,0x0
00000024  50                push ax
00000025  9AE504FD21        call 0x21fd:0x4e5
0000002A  59                pop cx
0000002B  59                pop cx
0000002C  33C0              xor ax,ax
0000002E  50                push ax
0000002F  9A7D03FD21        call 0x21fd:0x37d
00000034  59                pop cx
00000035  9A4813CF08        call 0x8cf:0x1348
0000003A  9ACB062508        call 0x825:0x6cb
0000003F  9ACD00CF08        call 0x8cf:0xcd
00000044  1E                push ds
00000045  B8A800            mov ax,0xa8
00000048  50                push ax
00000049  9A6C02FD21        call 0x21fd:0x26c
0000004E  59                pop cx
0000004F  59                pop cx
00000050  9AE7110906        call 0x609:0x11e7
00000055  9A2903FD21        call 0x21fd:0x329
0000005A  9A1B00960A        call 0xa96:0x1b
0000005F  9ABD01CF08        call 0x8cf:0x1bd
00000064  C606AA3700        mov byte [0x37aa],0x0
00000069  EB09              jmp short 0x74
0000006B  9ABA210906        call 0x609:0x21ba
00000070  FE06AA37          inc byte [0x37aa]
00000074  803EAA3704        cmp byte [0x37aa],0x4
00000079  7CF0              jl 0x6b
0000007B  9A0D00AF21        call 0x21af:0xd
00000080  5D                pop bp
00000081  CB                retf

; ---- 08CF:0082 ----
00000082  55                push bp
00000083  8BEC              mov bp,sp
00000085  56                push si
00000086  A0BA45            mov al,[0x45ba]
00000089  B400              mov ah,0x0
0000008B  8A16F24B          mov dl,[0x4bf2]
0000008F  B600              mov dh,0x0
00000091  03C2              add ax,dx
00000093  B105              mov cl,0x5
00000095  D3E0              shl ax,cl
00000097  BA6000            mov dx,0x60
0000009A  2BD0              sub dx,ax
0000009C  8BF2              mov si,dx
0000009E  8936A015          mov [0x15a0],si
000000A2  8BC6              mov ax,si
000000A4  054000            add ax,0x40
000000A7  8BF0              mov si,ax
000000A9  803EBA4501        cmp byte [0x45ba],0x1
000000AE  750B              jnz 0xbb
000000B0  8936B015          mov [0x15b0],si
000000B4  8BC6              mov ax,si
000000B6  054000            add ax,0x40
000000B9  8BF0              mov si,ax
000000BB  8936B815          mov [0x15b8],si
000000BF  8BC6              mov ax,si
000000C1  054000            add ax,0x40
000000C4  8BF0              mov si,ax
000000C6  8936B215          mov [0x15b2],si
000000CA  5E                pop si
000000CB  5D                pop bp
000000CC  CB                retf

; ---- 08CF:00CD ----
000000CD  55                push bp
000000CE  8BEC              mov bp,sp
000000D0  C606AA3700        mov byte [0x37aa],0x0
000000D5  E9D900            jmp 0x1b1
000000D8  9A1B00960A        call 0xa96:0x1b
000000DD  A0AA37            mov al,[0x37aa]
000000E0  98                cbw
000000E1  BA1A00            mov dx,0x1a
000000E4  F7EA              imul dx
000000E6  50                push ax
000000E7  A0AA37            mov al,[0x37aa]
000000EA  98                cbw
000000EB  8BD8              mov bx,ax
000000ED  8A87E400          mov al,[bx+0xe4]
000000F1  B400              mov ah,0x0
000000F3  8B16A015          mov dx,[0x15a0]
000000F7  03D0              add dx,ax
000000F9  5B                pop bx
000000FA  89972033          mov [bx+0x3320],dx
000000FE  A0AA37            mov al,[0x37aa]
00000101  98                cbw
00000102  BA0300            mov dx,0x3
00000105  F7EA              imul dx
00000107  057900            add ax,0x79
0000010A  50                push ax
0000010B  A0AA37            mov al,[0x37aa]
0000010E  98                cbw
0000010F  BA1A00            mov dx,0x1a
00000112  F7EA              imul dx
00000114  8BD8              mov bx,ax
00000116  58                pop ax
00000117  89872233          mov [bx+0x3322],ax
0000011B  A0AA37            mov al,[0x37aa]
0000011E  98                cbw
0000011F  8BD8              mov bx,ax
00000121  8A87423D          mov al,[bx+0x3d42]
00000125  B400              mov ah,0x0
00000127  50                push ax
00000128  B80600            mov ax,0x6
0000012B  50                push ax
0000012C  A0AA37            mov al,[0x37aa]
0000012F  98                cbw
00000130  B106              mov cl,0x6
00000132  D3E0              shl ax,cl
00000134  052000            add ax,0x20
00000137  50                push ax
00000138  9A79180906        call 0x609:0x1879
0000013D  83C406            add sp,byte +0x6
00000140  9AC01A0906        call 0x609:0x1ac0
00000145  A0AA37            mov al,[0x37aa]
00000148  98                cbw
00000149  8BD8              mov bx,ax
0000014B  C687B2430A        mov byte [bx+0x43b2],0xa
00000150  A0AA37            mov al,[0x37aa]
00000153  98                cbw
00000154  8BD8              mov bx,ax
00000156  C687AB1500        mov byte [bx+0x15ab],0x0
0000015B  A0AA37            mov al,[0x37aa]
0000015E  98                cbw
0000015F  D1E0              shl ax,1
00000161  8BD8              mov bx,ax
00000163  C787D24B0000      mov word [bx+0x4bd2],0x0
00000169  A0AA37            mov al,[0x37aa]
0000016C  98                cbw
0000016D  D1E0              shl ax,1
0000016F  8BD8              mov bx,ax
00000171  C78722300000      mov word [bx+0x3022],0x0
00000177  A0AA37            mov al,[0x37aa]
0000017A  98                cbw
0000017B  D1E0              shl ax,1
0000017D  D1E0              shl ax,1
0000017F  8BD8              mov bx,ax
00000181  C78786370000      mov word [bx+0x3786],0x0
00000187  C78788370000      mov word [bx+0x3788],0x0
0000018D  A0AA37            mov al,[0x37aa]
00000190  98                cbw
00000191  8BD8              mov bx,ax
00000193  C687861500        mov byte [bx+0x1586],0x0
00000198  A0AA37            mov al,[0x37aa]
0000019B  98                cbw
0000019C  8BD8              mov bx,ax
0000019E  C6879C1500        mov byte [bx+0x159c],0x0
000001A3  9ACC1F0906        call 0x609:0x1fcc
000001A8  9A0D06FD21        call 0x21fd:0x60d
000001AD  FE06AA37          inc byte [0x37aa]
000001B1  803EAA3704        cmp byte [0x37aa],0x4
000001B6  7D03              jnl 0x1bb
000001B8  E91DFF            jmp 0xd8
000001BB  5D                pop bp
000001BC  CB                retf

; ---- 08CF:01BD ----
000001BD  55                push bp
000001BE  8BEC              mov bp,sp
000001C0  9A1F05FD21        call 0x21fd:0x51f
000001C5  FF362E08          push word [0x82e]
000001C9  9A1C07CD23        call 0x23cd:0x71c
000001CE  59                pop cx
000001CF  9AA806960A        call 0xa96:0x6a8
000001D4  9A1B00960A        call 0xa96:0x1b
000001D9  33C0              xor ax,ax
000001DB  BA0200            mov dx,0x2
000001DE  50                push ax
000001DF  52                push dx
000001E0  9A8501FD21        call 0x21fd:0x185
000001E5  59                pop cx
000001E6  59                pop cx
000001E7  9A7A039E0E        call 0xe9e:0x37a
000001EC  A1B415            mov ax,[0x15b4]
000001EF  48                dec ax
000001F0  A3B415            mov [0x15b4],ax
000001F3  0BC0              or ax,ax
000001F5  7D06              jnl 0x1fd
000001F7  C706B4150500      mov word [0x15b4],0x5
000001FD  C606AA3700        mov byte [0x37aa],0x0
00000202  EB44              jmp short 0x248
00000204  A0AA37            mov al,[0x37aa]
00000207  98                cbw
00000208  8BD8              mov bx,ax
0000020A  8A87B243          mov al,[bx+0x43b2]
0000020E  B400              mov ah,0x0
00000210  2D0A00            sub ax,0xa
00000213  8BD8              mov bx,ax
00000215  83FB04            cmp bx,byte +0x4
00000218  772A              ja 0x244
0000021A  D1E3              shl bx,1
0000021C  2EFFA79002        jmp [cs:bx+0x290]
00000221  9A9A02CF08        call 0x8cf:0x29a
00000226  EB1C              jmp short 0x244
00000228  9AF109CF08        call 0x8cf:0x9f1
0000022D  EB15              jmp short 0x244
0000022F  9A9F06CF08        call 0x8cf:0x69f
00000234  EB0E              jmp short 0x244
00000236  9ABA04CF08        call 0x8cf:0x4ba
0000023B  EB07              jmp short 0x244
0000023D  9AD50CCF08        call 0x8cf:0xcd5
00000242  EB00              jmp short 0x244
00000244  FE06AA37          inc byte [0x37aa]
00000248  803EAA3704        cmp byte [0x37aa],0x4
0000024D  7CB5              jl 0x204
0000024F  803EA21503        cmp byte [0x15a2],0x3
00000254  7505              jnz 0x25b
00000256  9A5C11CF08        call 0x8cf:0x115c
0000025B  9A4D0FCF08        call 0x8cf:0xf4d
00000260  9AF904FD21        call 0x21fd:0x4f9
00000265  3C01              cmp al,0x1
00000267  7505              jnz 0x26e
00000269  9A3519CF08        call 0x8cf:0x1935
0000026E  9AA806960A        call 0xa96:0x6a8
00000273  FF362E08          push word [0x82e]
00000277  9AE906CD23        call 0x23cd:0x6e9
0000027C  59                pop cx
0000027D  833E803700        cmp word [0x3780],byte +0x0
00000282  7502              jnz 0x286
00000284  EB03              jmp short 0x289
00000286  E937FF            jmp 0x1c0
00000289  9A6506FD21        call 0x21fd:0x665
0000028E  5D                pop bp
0000028F  CB                retf
00000290  2102              and [bp+si],ax
00000292  2802              sub [bp+si],al
00000294  36023D            add bh,[ss:di]
00000297  022F              add ch,[bx]
00000299  02558B            add dl,[di-0x75]
0000029C  EC                in al,dx
0000029D  4C                dec sp
0000029E  4C                dec sp
0000029F  A0AA37            mov al,[0x37aa]
000002A2  98                cbw
000002A3  D1E0              shl ax,1
000002A5  8BD8              mov bx,ax
000002A7  83BF223000        cmp word [bx+0x3022],byte +0x0
000002AC  7408              jz 0x2b6
000002AE  9AE01F0906        call 0x609:0x1fe0
000002B3  E9E200            jmp 0x398
000002B6  A0AA37            mov al,[0x37aa]
000002B9  98                cbw
000002BA  8BD8              mov bx,ax
000002BC  80BF400801        cmp byte [bx+0x840],0x1
000002C1  7505              jnz 0x2c8
000002C3  9AE503CF08        call 0x8cf:0x3e5
000002C8  A0AA37            mov al,[0x37aa]
000002CB  98                cbw
000002CC  8BD8              mov bx,ax
000002CE  8A87AB15          mov al,[bx+0x15ab]
000002D2  8846FF            mov [bp-0x1],al
000002D5  A0AA37            mov al,[0x37aa]
000002D8  98                cbw
000002D9  8BD8              mov bx,ax
000002DB  8A870833          mov al,[bx+0x3308]
000002DF  98                cbw
000002E0  0BC0              or ax,ax
000002E2  7457              jz 0x33b
000002E4  3D0100            cmp ax,0x1
000002E7  740B              jz 0x2f4
000002E9  3D0200            cmp ax,0x2
000002EC  7503              jnz 0x2f1
000002EE  E9A000            jmp 0x391
000002F1  E9A400            jmp 0x398
000002F4  FE4EFF            dec byte [bp-0x1]
000002F7  803EBA4500        cmp byte [0x45ba],0x0
000002FC  7509              jnz 0x307
000002FE  807EFF01          cmp byte [bp-0x1],0x1
00000302  7503              jnz 0x307
00000304  FE4EFF            dec byte [bp-0x1]
00000307  807EFF00          cmp byte [bp-0x1],0x0
0000030B  7D03              jnl 0x310
0000030D  E98800            jmp 0x398
00000310  A0AA37            mov al,[0x37aa]
00000313  98                cbw
00000314  8A56FF            mov dl,[bp-0x1]
00000317  8BD8              mov bx,ax
00000319  8897AB15          mov [bx+0x15ab],dl
0000031D  A0AA37            mov al,[0x37aa]
00000320  98                cbw
00000321  D1E0              shl ax,1
00000323  8BD8              mov bx,ax
00000325  C787CA4B0100      mov word [bx+0x4bca],0x1
0000032B  A0AA37            mov al,[0x37aa]
0000032E  98                cbw
0000032F  D1E0              shl ax,1
00000331  8BD8              mov bx,ax
00000333  C78722302100      mov word [bx+0x3022],0x21
00000339  EB5D              jmp short 0x398
0000033B  FE46FF            inc byte [bp-0x1]
0000033E  803EBA4500        cmp byte [0x45ba],0x0
00000343  7509              jnz 0x34e
00000345  807EFF01          cmp byte [bp-0x1],0x1
00000349  7503              jnz 0x34e
0000034B  FE46FF            inc byte [bp-0x1]
0000034E  803EF24B00        cmp byte [0x4bf2],0x0
00000353  7509              jnz 0x35e
00000355  807EFF03          cmp byte [bp-0x1],0x3
00000359  7503              jnz 0x35e
0000035B  FE46FF            inc byte [bp-0x1]
0000035E  807EFF03          cmp byte [bp-0x1],0x3
00000362  7E02              jng 0x366
00000364  EB32              jmp short 0x398
00000366  A0AA37            mov al,[0x37aa]
00000369  98                cbw
0000036A  8A56FF            mov dl,[bp-0x1]
0000036D  8BD8              mov bx,ax
0000036F  8897AB15          mov [bx+0x15ab],dl
00000373  A0AA37            mov al,[0x37aa]
00000376  98                cbw
00000377  D1E0              shl ax,1
00000379  8BD8              mov bx,ax
0000037B  C787CA4B0000      mov word [bx+0x4bca],0x0
00000381  A0AA37            mov al,[0x37aa]
00000384  98                cbw
00000385  D1E0              shl ax,1
00000387  8BD8              mov bx,ax
00000389  C78722302100      mov word [bx+0x3022],0x21
0000038F  EB07              jmp short 0x398
00000391  9A9C03CF08        call 0x8cf:0x39c
00000396  EB00              jmp short 0x398
00000398  8BE5              mov sp,bp
0000039A  5D                pop bp
0000039B  CB                retf

; ---- 08CF:039C ----
0000039C  55                push bp
0000039D  8BEC              mov bp,sp
0000039F  A0AA37            mov al,[0x37aa]
000003A2  98                cbw
000003A3  8BD8              mov bx,ax
000003A5  8A87AB15          mov al,[bx+0x15ab]
000003A9  98                cbw
000003AA  8BD8              mov bx,ax
000003AC  83FB03            cmp bx,byte +0x3
000003AF  772A              ja 0x3db
000003B1  D1E3              shl bx,1
000003B3  2EFFA7DD03        jmp [cs:bx+0x3dd]
000003B8  9A9204CF08        call 0x8cf:0x492
000003BD  EB1C              jmp short 0x3db
000003BF  9A6D06CF08        call 0x8cf:0x66d
000003C4  EB15              jmp short 0x3db
000003C6  9AC409CF08        call 0x8cf:0x9c4
000003CB  EB0E              jmp short 0x3db
000003CD  803EA215FF        cmp byte [0x15a2],0xff
000003D2  7505              jnz 0x3d9
000003D4  9AE10BCF08        call 0x8cf:0xbe1
000003D9  EB00              jmp short 0x3db
000003DB  5D                pop bp
000003DC  CB                retf
000003DD  CD03              int 0x3
000003DF  B803C6            mov ax,0xc603
000003E2  03BF0355          add di,[bx+0x5503]
000003E6  8BEC              mov bp,sp
000003E8  9AEF0DCF08        call 0x8cf:0xdef
000003ED  3C01              cmp al,0x1
000003EF  7518              jnz 0x409
000003F1  A0AA37            mov al,[0x37aa]
000003F4  98                cbw
000003F5  8BD8              mov bx,ax
000003F7  80BF861504        cmp byte [bx+0x1586],0x4
000003FC  7D0B              jnl 0x409
000003FE  33C0              xor ax,ax
00000400  50                push ax
00000401  9A5504CF08        call 0x8cf:0x455
00000406  59                pop cx
00000407  EB4A              jmp short 0x453
00000409  9AF105CF08        call 0x8cf:0x5f1
0000040E  3C01              cmp al,0x1
00000410  750C              jnz 0x41e
00000412  B80100            mov ax,0x1
00000415  50                push ax
00000416  9A5504CF08        call 0x8cf:0x455
0000041B  59                pop cx
0000041C  EB35              jmp short 0x453
0000041E  9AF00ACF08        call 0x8cf:0xaf0
00000423  3C01              cmp al,0x1
00000425  750C              jnz 0x433
00000427  B80200            mov ax,0x2
0000042A  50                push ax
0000042B  9A5504CF08        call 0x8cf:0x455
00000430  59                pop cx
00000431  EB20              jmp short 0x453
00000433  9A1409CF08        call 0x8cf:0x914
00000438  0BC0              or ax,ax
0000043A  740C              jz 0x448
0000043C  B80300            mov ax,0x3
0000043F  50                push ax
00000440  9A5504CF08        call 0x8cf:0x455
00000445  59                pop cx
00000446  EB0B              jmp short 0x453
00000448  A0AA37            mov al,[0x37aa]
0000044B  98                cbw
0000044C  8BD8              mov bx,ax
0000044E  C6870833FF        mov byte [bx+0x3308],0xff
00000453  5D                pop bp
00000454  CB                retf

; ---- 08CF:0455 ----
00000455  55                push bp
00000456  8BEC              mov bp,sp
00000458  A0AA37            mov al,[0x37aa]
0000045B  98                cbw
0000045C  8BD8              mov bx,ax
0000045E  8A87AB15          mov al,[bx+0x15ab]
00000462  98                cbw
00000463  3B4606            cmp ax,[bp+0x6]
00000466  7D04              jnl 0x46c
00000468  33C9              xor cx,cx
0000046A  EB18              jmp short 0x484
0000046C  A0AA37            mov al,[0x37aa]
0000046F  98                cbw
00000470  8BD8              mov bx,ax
00000472  8A87AB15          mov al,[bx+0x15ab]
00000476  98                cbw
00000477  3B4606            cmp ax,[bp+0x6]
0000047A  7E05              jng 0x481
0000047C  B90100            mov cx,0x1
0000047F  EB03              jmp short 0x484
00000481  B90200            mov cx,0x2
00000484  A0AA37            mov al,[0x37aa]
00000487  98                cbw
00000488  8AD1              mov dl,cl
0000048A  8BD8              mov bx,ax
0000048C  88970833          mov [bx+0x3308],dl
00000490  5D                pop bp
00000491  CB                retf

; ---- 08CF:0492 ----
00000492  55                push bp
00000493  8BEC              mov bp,sp
00000495  A0AA37            mov al,[0x37aa]
00000498  98                cbw
00000499  8BD8              mov bx,ax
0000049B  C687B2430C        mov byte [bx+0x43b2],0xc
000004A0  9AE5180906        call 0x609:0x18e5
000004A5  9A3E19CF08        call 0x8cf:0x193e
000004AA  B85000            mov ax,0x50
000004AD  50                push ax
000004AE  33C0              xor ax,ax
000004B0  50                push ax
000004B1  9A5400A721        call 0x21a7:0x54
000004B6  59                pop cx
000004B7  59                pop cx
000004B8  5D                pop bp
000004B9  CB                retf

; ---- 08CF:04BA ----
000004BA  55                push bp
000004BB  8BEC              mov bp,sp
000004BD  A0AA37            mov al,[0x37aa]
000004C0  98                cbw
000004C1  8BD8              mov bx,ax
000004C3  80BF400801        cmp byte [bx+0x840],0x1
000004C8  753B              jnz 0x505
000004CA  9AF105CF08        call 0x8cf:0x5f1
000004CF  0AC0              or al,al
000004D1  750D              jnz 0x4e0
000004D3  A0AA37            mov al,[0x37aa]
000004D6  98                cbw
000004D7  8BD8              mov bx,ax
000004D9  C687083303        mov byte [bx+0x3308],0x3
000004DE  EB25              jmp short 0x505
000004E0  A0AA37            mov al,[0x37aa]
000004E3  98                cbw
000004E4  D1E0              shl ax,1
000004E6  D1E0              shl ax,1
000004E8  8BD8              mov bx,ax
000004EA  83BF883700        cmp word [bx+0x3788],byte +0x0
000004EF  7F14              jg 0x505
000004F1  7507              jnz 0x4fa
000004F3  83BF863700        cmp word [bx+0x3786],byte +0x0
000004F8  770B              ja 0x505
000004FA  A0AA37            mov al,[0x37aa]
000004FD  98                cbw
000004FE  8BD8              mov bx,ax
00000500  C687083300        mov byte [bx+0x3308],0x0
00000505  A0AA37            mov al,[0x37aa]
00000508  98                cbw
00000509  8BD8              mov bx,ax
0000050B  8A870833          mov al,[bx+0x3308]
0000050F  98                cbw
00000510  0BC0              or ax,ax
00000512  744E              jz 0x562
00000514  3D0300            cmp ax,0x3
00000517  7408              jz 0x521
00000519  3D0A00            cmp ax,0xa
0000051C  7444              jz 0x562
0000051E  E9CE00            jmp 0x5ef
00000521  A0AA37            mov al,[0x37aa]
00000524  98                cbw
00000525  8BD8              mov bx,ax
00000527  8A87423D          mov al,[bx+0x3d42]
0000052B  B400              mov ah,0x0
0000052D  50                push ax
0000052E  B80600            mov ax,0x6
00000531  50                push ax
00000532  A0AA37            mov al,[0x37aa]
00000535  98                cbw
00000536  B106              mov cl,0x6
00000538  D3E0              shl ax,cl
0000053A  052000            add ax,0x20
0000053D  50                push ax
0000053E  9A79180906        call 0x609:0x1879
00000543  83C406            add sp,byte +0x6
00000546  A0AA37            mov al,[0x37aa]
00000549  98                cbw
0000054A  8BD8              mov bx,ax
0000054C  C687B2430A        mov byte [bx+0x43b2],0xa
00000551  B85100            mov ax,0x51
00000554  50                push ax
00000555  33C0              xor ax,ax
00000557  50                push ax
00000558  9A5400A721        call 0x21a7:0x54
0000055D  59                pop cx
0000055E  59                pop cx
0000055F  E98D00            jmp 0x5ef
00000562  A0AA37            mov al,[0x37aa]
00000565  98                cbw
00000566  D1E0              shl ax,1
00000568  D1E0              shl ax,1
0000056A  50                push ax
0000056B  A1F443            mov ax,[0x43f4]
0000056E  99                cwd
0000056F  5B                pop bx
00000570  3997D43C          cmp [bx+0x3cd4],dx
00000574  7F0A              jg 0x580
00000576  7C06              jl 0x57e
00000578  3987D23C          cmp [bx+0x3cd2],ax
0000057C  7302              jnc 0x580
0000057E  EB6F              jmp short 0x5ef
00000580  A0AA37            mov al,[0x37aa]
00000583  98                cbw
00000584  D1E0              shl ax,1
00000586  8BD8              mov bx,ax
00000588  83BF204528        cmp word [bx+0x4520],byte +0x28
0000058D  7C02              jl 0x591
0000058F  EB5E              jmp short 0x5ef
00000591  833E984B00        cmp word [0x4b98],byte +0x0
00000596  7502              jnz 0x59a
00000598  EB55              jmp short 0x5ef
0000059A  B001              mov al,0x1
0000059C  50                push ax
0000059D  9A721B0906        call 0x609:0x1b72
000005A2  59                pop cx
000005A3  FF0E984B          dec word [0x4b98]
000005A7  9A8119CF08        call 0x8cf:0x1981
000005AC  A1F443            mov ax,[0x43f4]
000005AF  99                cwd
000005B0  50                push ax
000005B1  A0AA37            mov al,[0x37aa]
000005B4  98                cbw
000005B5  D1E0              shl ax,1
000005B7  D1E0              shl ax,1
000005B9  8BD8              mov bx,ax
000005BB  58                pop ax
000005BC  2987D23C          sub [bx+0x3cd2],ax
000005C0  1997D43C          sbb [bx+0x3cd4],dx
000005C4  B85200            mov ax,0x52
000005C7  50                push ax
000005C8  33C0              xor ax,ax
000005CA  50                push ax
000005CB  9A5400A721        call 0x21a7:0x54
000005D0  59                pop cx
000005D1  59                pop cx
000005D2  9A3E19CF08        call 0x8cf:0x193e
000005D7  A0AA37            mov al,[0x37aa]
000005DA  98                cbw
000005DB  D1E0              shl ax,1
000005DD  D1E0              shl ax,1
000005DF  8BD8              mov bx,ax
000005E1  C78786370300      mov word [bx+0x3786],0x3
000005E7  C78788370000      mov word [bx+0x3788],0x0
000005ED  EB00              jmp short 0x5ef
000005EF  5D                pop bp
000005F0  CB                retf

; ---- 08CF:05F1 ----
000005F1  55                push bp
000005F2  8BEC              mov bp,sp
000005F4  56                push si
000005F5  57                push di
000005F6  A0AA37            mov al,[0x37aa]
000005F9  98                cbw
000005FA  D1E0              shl ax,1
000005FC  8BD8              mov bx,ax
000005FE  8BB72045          mov si,[bx+0x4520]
00000602  803EBA4500        cmp byte [0x45ba],0x0
00000607  7405              jz 0x60e
00000609  83FE28            cmp si,byte +0x28
0000060C  7C04              jl 0x612
0000060E  B000              mov al,0x0
00000610  EB57              jmp short 0x669
00000612  833E984B00        cmp word [0x4b98],byte +0x0
00000617  7504              jnz 0x61d
00000619  B000              mov al,0x0
0000061B  EB4C              jmp short 0x669
0000061D  83FE0A            cmp si,byte +0xa
00000620  7D05              jnl 0x627
00000622  B86400            mov ax,0x64
00000625  EB0A              jmp short 0x631
00000627  8BC6              mov ax,si
00000629  BA0A00            mov dx,0xa
0000062C  F7EA              imul dx
0000062E  05C800            add ax,0xc8
00000631  8BF8              mov di,ax
00000633  8BC7              mov ax,di
00000635  99                cwd
00000636  52                push dx
00000637  50                push ax
00000638  A0AA37            mov al,[0x37aa]
0000063B  98                cbw
0000063C  D1E0              shl ax,1
0000063E  D1E0              shl ax,1
00000640  8BD8              mov bx,ax
00000642  FFB7D43C          push word [bx+0x3cd4]
00000646  FFB7D23C          push word [bx+0x3cd2]
0000064A  9A894F0000        call 0x0:0x4f89
0000064F  50                push ax
00000650  A1F443            mov ax,[0x43f4]
00000653  52                push dx
00000654  99                cwd
00000655  5B                pop bx
00000656  3BDA              cmp bx,dx
00000658  5A                pop dx
00000659  7C0A              jl 0x665
0000065B  7504              jnz 0x661
0000065D  3BD0              cmp dx,ax
0000065F  7204              jc 0x665
00000661  B001              mov al,0x1
00000663  EB04              jmp short 0x669
00000665  B000              mov al,0x0
00000667  EB00              jmp short 0x669
00000669  5F                pop di
0000066A  5E                pop si
0000066B  5D                pop bp
0000066C  CB                retf

; ---- 08CF:066D ----
0000066D  55                push bp
0000066E  8BEC              mov bp,sp
00000670  A0AA37            mov al,[0x37aa]
00000673  98                cbw
00000674  8BD8              mov bx,ax
00000676  C687B2430E        mov byte [bx+0x43b2],0xe
0000067B  9AE5180906        call 0x609:0x18e5
00000680  9A3E19CF08        call 0x8cf:0x193e
00000685  9A2A17CF08        call 0x8cf:0x172a
0000068A  9A5918CF08        call 0x8cf:0x1859
0000068F  B85000            mov ax,0x50
00000692  50                push ax
00000693  33C0              xor ax,ax
00000695  50                push ax
00000696  9A5400A721        call 0x21a7:0x54
0000069B  59                pop cx
0000069C  59                pop cx
0000069D  5D                pop bp
0000069E  CB                retf

; ---- 08CF:069F ----
0000069F  55                push bp
000006A0  8BEC              mov bp,sp
000006A2  4C                dec sp
000006A3  4C                dec sp
000006A4  A0AA37            mov al,[0x37aa]
000006A7  98                cbw
000006A8  8BD8              mov bx,ax
000006AA  80BF400801        cmp byte [bx+0x840],0x1
000006AF  7546              jnz 0x6f7
000006B1  833EB41500        cmp word [0x15b4],byte +0x0
000006B6  753F              jnz 0x6f7
000006B8  9A1409CF08        call 0x8cf:0x914
000006BD  3D0100            cmp ax,0x1
000006C0  7413              jz 0x6d5
000006C2  3D0200            cmp ax,0x2
000006C5  7415              jz 0x6dc
000006C7  3D0300            cmp ax,0x3
000006CA  7402              jz 0x6ce
000006CC  EB15              jmp short 0x6e3
000006CE  C746FE0A00        mov word [bp-0x2],0xa
000006D3  EB15              jmp short 0x6ea
000006D5  C746FE0100        mov word [bp-0x2],0x1
000006DA  EB0E              jmp short 0x6ea
000006DC  C746FE0000        mov word [bp-0x2],0x0
000006E1  EB07              jmp short 0x6ea
000006E3  C746FE0300        mov word [bp-0x2],0x3
000006E8  EB00              jmp short 0x6ea
000006EA  A0AA37            mov al,[0x37aa]
000006ED  98                cbw
000006EE  8A56FE            mov dl,[bp-0x2]
000006F1  8BD8              mov bx,ax
000006F3  88970833          mov [bx+0x3308],dl
000006F7  A0AA37            mov al,[0x37aa]
000006FA  98                cbw
000006FB  8BD8              mov bx,ax
000006FD  8A870833          mov al,[bx+0x3308]
00000701  98                cbw
00000702  8BD8              mov bx,ax
00000704  83FB0A            cmp bx,byte +0xa
00000707  7603              jna 0x70c
00000709  E9ED00            jmp 0x7f9
0000070C  D1E3              shl bx,1
0000070E  2EFFA7FD07        jmp [cs:bx+0x7fd]
00000713  A0AA37            mov al,[0x37aa]
00000716  98                cbw
00000717  8BD8              mov bx,ax
00000719  8A87423D          mov al,[bx+0x3d42]
0000071D  B400              mov ah,0x0
0000071F  50                push ax
00000720  B80600            mov ax,0x6
00000723  50                push ax
00000724  A0AA37            mov al,[0x37aa]
00000727  98                cbw
00000728  B106              mov cl,0x6
0000072A  D3E0              shl ax,cl
0000072C  052000            add ax,0x20
0000072F  50                push ax
00000730  9A79180906        call 0x609:0x1879
00000735  83C406            add sp,byte +0x6
00000738  A0AA37            mov al,[0x37aa]
0000073B  98                cbw
0000073C  8BD8              mov bx,ax
0000073E  C687B2430A        mov byte [bx+0x43b2],0xa
00000743  B85100            mov ax,0x51
00000746  50                push ax
00000747  33C0              xor ax,ax
00000749  50                push ax
0000074A  9A5400A721        call 0x21a7:0x54
0000074F  59                pop cx
00000750  59                pop cx
00000751  E9A500            jmp 0x7f9
00000754  9A1308CF08        call 0x8cf:0x813
00000759  0AC0              or al,al
0000075B  7503              jnz 0x760
0000075D  E99900            jmp 0x7f9
00000760  B0FF              mov al,0xff
00000762  50                push ax
00000763  9A8608CF08        call 0x8cf:0x886
00000768  59                pop cx
00000769  A0AA37            mov al,[0x37aa]
0000076C  98                cbw
0000076D  8BD8              mov bx,ax
0000076F  FE879C15          inc byte [bx+0x159c]
00000773  E98300            jmp 0x7f9
00000776  9A4F08CF08        call 0x8cf:0x84f
0000077B  0AC0              or al,al
0000077D  7502              jnz 0x781
0000077F  EB78              jmp short 0x7f9
00000781  B001              mov al,0x1
00000783  50                push ax
00000784  9A8608CF08        call 0x8cf:0x886
00000789  59                pop cx
0000078A  A0AA37            mov al,[0x37aa]
0000078D  98                cbw
0000078E  8BD8              mov bx,ax
00000790  FE879C15          inc byte [bx+0x159c]
00000794  EB63              jmp short 0x7f9
00000796  A0AA37            mov al,[0x37aa]
00000799  98                cbw
0000079A  8BD8              mov bx,ax
0000079C  80BFAF3700        cmp byte [bx+0x37af],0x0
000007A1  7502              jnz 0x7a5
000007A3  EB54              jmp short 0x7f9
000007A5  B0FF              mov al,0xff
000007A7  50                push ax
000007A8  9A8D17CF08        call 0x8cf:0x178d
000007AD  59                pop cx
000007AE  33C0              xor ax,ax
000007B0  BA6400            mov dx,0x64
000007B3  50                push ax
000007B4  52                push dx
000007B5  8B0EF23C          mov cx,[0x3cf2]
000007B9  8B1EF03C          mov bx,[0x3cf0]
000007BD  8B163844          mov dx,[0x4438]
000007C1  A13644            mov ax,[0x4436]
000007C4  9A4E4F0000        call 0x0:0x4f4e
000007C9  52                push dx
000007CA  50                push ax
000007CB  9A894F0000        call 0x0:0x4f89
000007D0  50                push ax
000007D1  A0AA37            mov al,[0x37aa]
000007D4  98                cbw
000007D5  D1E0              shl ax,1
000007D7  D1E0              shl ax,1
000007D9  8BD8              mov bx,ax
000007DB  58                pop ax
000007DC  0187D23C          add [bx+0x3cd2],ax
000007E0  1197D43C          adc [bx+0x3cd4],dx
000007E4  9A3E19CF08        call 0x8cf:0x193e
000007E9  B85200            mov ax,0x52
000007EC  50                push ax
000007ED  33C0              xor ax,ax
000007EF  50                push ax
000007F0  9A5400A721        call 0x21a7:0x54
000007F5  59                pop cx
000007F6  59                pop cx
000007F7  EB00              jmp short 0x7f9
000007F9  8BE5              mov sp,bp
000007FB  5D                pop bp
000007FC  CB                retf
000007FD  7607              jna 0x806
000007FF  54                push sp
00000800  07                pop es
00000801  F9                stc
00000802  07                pop es
00000803  1307              adc ax,[bx]
00000805  F9                stc
00000806  07                pop es
00000807  F9                stc
00000808  07                pop es
00000809  F9                stc
0000080A  07                pop es
0000080B  F9                stc
0000080C  07                pop es
0000080D  F9                stc
0000080E  07                pop es
0000080F  F9                stc
00000810  07                pop es
00000811  96                xchg ax,si
00000812  07                pop es

; ---- 08CF:0813 ----
00000813  55                push bp
00000814  8BEC              mov bp,sp
00000816  A0AA37            mov al,[0x37aa]
00000819  98                cbw
0000081A  D1E0              shl ax,1
0000081C  8BD8              mov bx,ax
0000081E  8B870E30          mov ax,[bx+0x300e]
00000822  99                cwd
00000823  03063644          add ax,[0x4436]
00000827  13163844          adc dx,[0x4438]
0000082B  0BD2              or dx,dx
0000082D  7F14              jg 0x843
0000082F  7505              jnz 0x836
00000831  3DD007            cmp ax,0x7d0
00000834  770D              ja 0x843
00000836  A0AA37            mov al,[0x37aa]
00000839  98                cbw
0000083A  8BD8              mov bx,ax
0000083C  80BFAF3700        cmp byte [bx+0x37af],0x0
00000841  7506              jnz 0x849
00000843  B000              mov al,0x0
00000845  EB06              jmp short 0x84d
00000847  EB04              jmp short 0x84d
00000849  B001              mov al,0x1
0000084B  EB00              jmp short 0x84d
0000084D  5D                pop bp
0000084E  CB                retf

; ---- 08CF:084F ----
0000084F  55                push bp
00000850  8BEC              mov bp,sp
00000852  A0AA37            mov al,[0x37aa]
00000855  98                cbw
00000856  8BD8              mov bx,ax
00000858  80BFAF370A        cmp byte [bx+0x37af],0xa
0000085D  741B              jz 0x87a
0000085F  A0AA37            mov al,[0x37aa]
00000862  98                cbw
00000863  D1E0              shl ax,1
00000865  8BD8              mov bx,ax
00000867  8B870E30          mov ax,[bx+0x300e]
0000086B  99                cwd
0000086C  3B163844          cmp dx,[0x4438]
00000870  7F0E              jg 0x880
00000872  7C06              jl 0x87a
00000874  3B063644          cmp ax,[0x4436]
00000878  7306              jnc 0x880
0000087A  B000              mov al,0x0
0000087C  EB06              jmp short 0x884
0000087E  EB04              jmp short 0x884
00000880  B001              mov al,0x1
00000882  EB00              jmp short 0x884
00000884  5D                pop bp
00000885  CB                retf

; ---- 08CF:0886 ----
00000886  55                push bp
00000887  8BEC              mov bp,sp
00000889  8A4606            mov al,[bp+0x6]
0000088C  50                push ax
0000088D  9A8D17CF08        call 0x8cf:0x178d
00000892  59                pop cx
00000893  8A4606            mov al,[bp+0x6]
00000896  98                cbw
00000897  99                cwd
00000898  50                push ax
00000899  52                push dx
0000089A  8B163844          mov dx,[0x4438]
0000089E  A13644            mov ax,[0x4436]
000008A1  59                pop cx
000008A2  5B                pop bx
000008A3  9A4E4F0000        call 0x0:0x4f4e
000008A8  50                push ax
000008A9  A0AA37            mov al,[0x37aa]
000008AC  98                cbw
000008AD  D1E0              shl ax,1
000008AF  8BD8              mov bx,ax
000008B1  8B870E30          mov ax,[bx+0x300e]
000008B5  5A                pop dx
000008B6  2BC2              sub ax,dx
000008B8  50                push ax
000008B9  A0AA37            mov al,[0x37aa]
000008BC  98                cbw
000008BD  D1E0              shl ax,1
000008BF  8BD8              mov bx,ax
000008C1  58                pop ax
000008C2  89870E30          mov [bx+0x300e],ax
000008C6  9A5918CF08        call 0x8cf:0x1859
000008CB  807E0601          cmp byte [bp+0x6],0x1
000008CF  7510              jnz 0x8e1
000008D1  B82B00            mov ax,0x2b
000008D4  50                push ax
000008D5  33C0              xor ax,ax
000008D7  50                push ax
000008D8  9A5400A721        call 0x21a7:0x54
000008DD  59                pop cx
000008DE  59                pop cx
000008DF  EB0E              jmp short 0x8ef
000008E1  B82C00            mov ax,0x2c
000008E4  50                push ax
000008E5  33C0              xor ax,ax
000008E7  50                push ax
000008E8  9A5400A721        call 0x21a7:0x54
000008ED  59                pop cx
000008EE  59                pop cx
000008EF  A0AA37            mov al,[0x37aa]
000008F2  98                cbw
000008F3  8BD8              mov bx,ax
000008F5  80BF400800        cmp byte [bx+0x840],0x0
000008FA  7516              jnz 0x912
000008FC  A0AA37            mov al,[0x37aa]
000008FF  98                cbw
00000900  D1E0              shl ax,1
00000902  D1E0              shl ax,1
00000904  8BD8              mov bx,ax
00000906  C78786376400      mov word [bx+0x3786],0x64
0000090C  C78788370000      mov word [bx+0x3788],0x0
00000912  5D                pop bp
00000913  CB                retf

; ---- 08CF:0914 ----
00000914  55                push bp
00000915  8BEC              mov bp,sp
00000917  56                push si
00000918  57                push di
00000919  A19A4B            mov ax,[0x4b9a]
0000091C  056400            add ax,0x64
0000091F  8B163644          mov dx,[0x4436]
00000923  2BD0              sub dx,ax
00000925  8BF2              mov si,dx
00000927  803EF24B00        cmp byte [0x4bf2],0x0
0000092C  7505              jnz 0x933
0000092E  33C0              xor ax,ax
00000930  E98D00            jmp 0x9c0
00000933  0BF6              or si,si
00000935  7E56              jng 0x98d
00000937  A0AA37            mov al,[0x37aa]
0000093A  98                cbw
0000093B  8BD8              mov bx,ax
0000093D  80BFAF3705        cmp byte [bx+0x37af],0x5
00000942  7E1E              jng 0x962
00000944  A0AA37            mov al,[0x37aa]
00000947  98                cbw
00000948  8BD8              mov bx,ax
0000094A  8A87AF37          mov al,[bx+0x37af]
0000094E  98                cbw
0000094F  BA0500            mov dx,0x5
00000952  F7EA              imul dx
00000954  BA4B00            mov dx,0x4b
00000957  2BD0              sub dx,ax
00000959  3BD6              cmp dx,si
0000095B  7F05              jg 0x962
0000095D  B80300            mov ax,0x3
00000960  EB5E              jmp short 0x9c0
00000962  0E                push cs
00000963  E8ADFE            call 0x813
00000966  3C01              cmp al,0x1
00000968  7521              jnz 0x98b
0000096A  8BC6              mov ax,si
0000096C  05F6FF            add ax,0xfff6
0000096F  BB0400            mov bx,0x4
00000972  99                cwd
00000973  F7FB              idiv bx
00000975  8BF8              mov di,ax
00000977  A0AA37            mov al,[0x37aa]
0000097A  98                cbw
0000097B  8BD8              mov bx,ax
0000097D  8A879C15          mov al,[bx+0x159c]
00000981  98                cbw
00000982  3BC7              cmp ax,di
00000984  7D05              jnl 0x98b
00000986  B80100            mov ax,0x1
00000989  EB35              jmp short 0x9c0
0000098B  EB2F              jmp short 0x9bc
0000098D  0BF6              or si,si
0000098F  7D2B              jnl 0x9bc
00000991  0E                push cs
00000992  E8BAFE            call 0x84f
00000995  3C01              cmp al,0x1
00000997  7523              jnz 0x9bc
00000999  8BC6              mov ax,si
0000099B  050A00            add ax,0xa
0000099E  F7D8              neg ax
000009A0  BB0400            mov bx,0x4
000009A3  99                cwd
000009A4  F7FB              idiv bx
000009A6  8BF8              mov di,ax
000009A8  A0AA37            mov al,[0x37aa]
000009AB  98                cbw
000009AC  8BD8              mov bx,ax
000009AE  8A879C15          mov al,[bx+0x159c]
000009B2  98                cbw
000009B3  3BC7              cmp ax,di
000009B5  7D05              jnl 0x9bc
000009B7  B80200            mov ax,0x2
000009BA  EB04              jmp short 0x9c0
000009BC  33C0              xor ax,ax
000009BE  EB00              jmp short 0x9c0
000009C0  5F                pop di
000009C1  5E                pop si
000009C2  5D                pop bp
000009C3  CB                retf

; ---- 08CF:09C4 ----
000009C4  55                push bp
000009C5  8BEC              mov bp,sp
000009C7  A0AA37            mov al,[0x37aa]
000009CA  98                cbw
000009CB  8BD8              mov bx,ax
000009CD  C687B2430B        mov byte [bx+0x43b2],0xb
000009D2  9AE5180906        call 0x609:0x18e5
000009D7  9A3E19CF08        call 0x8cf:0x193e
000009DC  9A78190906        call 0x609:0x1978
000009E1  B85000            mov ax,0x50
000009E4  50                push ax
000009E5  33C0              xor ax,ax
000009E7  50                push ax
000009E8  9A5400A721        call 0x21a7:0x54
000009ED  59                pop cx
000009EE  59                pop cx
000009EF  5D                pop bp
000009F0  CB                retf

; ---- 08CF:09F1 ----
000009F1  55                push bp
000009F2  8BEC              mov bp,sp
000009F4  A0AA37            mov al,[0x37aa]
000009F7  98                cbw
000009F8  8BD8              mov bx,ax
000009FA  80BF400801        cmp byte [bx+0x840],0x1
000009FF  7528              jnz 0xa29
00000A01  833EB41500        cmp word [0x15b4],byte +0x0
00000A06  7521              jnz 0xa29
00000A08  9AF00ACF08        call 0x8cf:0xaf0
00000A0D  3C01              cmp al,0x1
00000A0F  750D              jnz 0xa1e
00000A11  A0AA37            mov al,[0x37aa]
00000A14  98                cbw
00000A15  8BD8              mov bx,ax
00000A17  C68708330A        mov byte [bx+0x3308],0xa
00000A1C  EB0B              jmp short 0xa29
00000A1E  A0AA37            mov al,[0x37aa]
00000A21  98                cbw
00000A22  8BD8              mov bx,ax
00000A24  C687083303        mov byte [bx+0x3308],0x3
00000A29  A0AA37            mov al,[0x37aa]
00000A2C  98                cbw
00000A2D  8BD8              mov bx,ax
00000A2F  8A870833          mov al,[bx+0x3308]
00000A33  98                cbw
00000A34  3D0300            cmp ax,0x3
00000A37  7408              jz 0xa41
00000A39  3D0A00            cmp ax,0xa
00000A3C  7443              jz 0xa81
00000A3E  E9AD00            jmp 0xaee
00000A41  A0AA37            mov al,[0x37aa]
00000A44  98                cbw
00000A45  8BD8              mov bx,ax
00000A47  8A87423D          mov al,[bx+0x3d42]
00000A4B  B400              mov ah,0x0
00000A4D  50                push ax
00000A4E  B80600            mov ax,0x6
00000A51  50                push ax
00000A52  A0AA37            mov al,[0x37aa]
00000A55  98                cbw
00000A56  B106              mov cl,0x6
00000A58  D3E0              shl ax,cl
00000A5A  052000            add ax,0x20
00000A5D  50                push ax
00000A5E  9A79180906        call 0x609:0x1879
00000A63  83C406            add sp,byte +0x6
00000A66  A0AA37            mov al,[0x37aa]
00000A69  98                cbw
00000A6A  8BD8              mov bx,ax
00000A6C  C687B2430A        mov byte [bx+0x43b2],0xa
00000A71  B85100            mov ax,0x51
00000A74  50                push ax
00000A75  33C0              xor ax,ax
00000A77  50                push ax
00000A78  9A5400A721        call 0x21a7:0x54
00000A7D  59                pop cx
00000A7E  59                pop cx
00000A7F  EB6D              jmp short 0xaee
00000A81  A0AA37            mov al,[0x37aa]
00000A84  98                cbw
00000A85  D1E0              shl ax,1
00000A87  D1E0              shl ax,1
00000A89  50                push ax
00000A8A  A1A048            mov ax,[0x48a0]
00000A8D  99                cwd
00000A8E  5B                pop bx
00000A8F  3997D43C          cmp [bx+0x3cd4],dx
00000A93  7F0A              jg 0xa9f
00000A95  7C06              jl 0xa9d
00000A97  3987D23C          cmp [bx+0x3cd2],ax
00000A9B  7302              jnc 0xa9f
00000A9D  EB4F              jmp short 0xaee
00000A9F  A0AA37            mov al,[0x37aa]
00000AA2  98                cbw
00000AA3  D1E0              shl ax,1
00000AA5  8BD8              mov bx,ax
00000AA7  83BFAC4B0A        cmp word [bx+0x4bac],byte +0xa
00000AAC  7502              jnz 0xab0
00000AAE  EB3E              jmp short 0xaee
00000AB0  A0AA37            mov al,[0x37aa]
00000AB3  98                cbw
00000AB4  D1E0              shl ax,1
00000AB6  8BD8              mov bx,ax
00000AB8  FF87AC4B          inc word [bx+0x4bac]
00000ABC  9A78190906        call 0x609:0x1978
00000AC1  A1A048            mov ax,[0x48a0]
00000AC4  99                cwd
00000AC5  50                push ax
00000AC6  A0AA37            mov al,[0x37aa]
00000AC9  98                cbw
00000ACA  D1E0              shl ax,1
00000ACC  D1E0              shl ax,1
00000ACE  8BD8              mov bx,ax
00000AD0  58                pop ax
00000AD1  2987D23C          sub [bx+0x3cd2],ax
00000AD5  1997D43C          sbb [bx+0x3cd4],dx
00000AD9  9A3E19CF08        call 0x8cf:0x193e
00000ADE  B85200            mov ax,0x52
00000AE1  50                push ax
00000AE2  33C0              xor ax,ax
00000AE4  50                push ax
00000AE5  9A5400A721        call 0x21a7:0x54
00000AEA  59                pop cx
00000AEB  59                pop cx
00000AEC  EB00              jmp short 0xaee
00000AEE  5D                pop bp
00000AEF  CB                retf

; ---- 08CF:0AF0 ----
00000AF0  55                push bp
00000AF1  8BEC              mov bp,sp
00000AF3  83EC04            sub sp,byte +0x4
00000AF6  A0AA37            mov al,[0x37aa]
00000AF9  98                cbw
00000AFA  D1E0              shl ax,1
00000AFC  8BD8              mov bx,ax
00000AFE  8B87AC4B          mov ax,[bx+0x4bac]
00000B02  50                push ax
00000B03  A0AA37            mov al,[0x37aa]
00000B06  98                cbw
00000B07  D1E0              shl ax,1
00000B09  8BD8              mov bx,ax
00000B0B  58                pop ax
00000B0C  3B874630          cmp ax,[bx+0x3046]
00000B10  7C05              jl 0xb17
00000B12  B000              mov al,0x0
00000B14  E9C600            jmp 0xbdd
00000B17  A0AA37            mov al,[0x37aa]
00000B1A  98                cbw
00000B1B  8BD8              mov bx,ax
00000B1D  80BFC24B01        cmp byte [bx+0x4bc2],0x1
00000B22  7424              jz 0xb48
00000B24  A0AA37            mov al,[0x37aa]
00000B27  98                cbw
00000B28  D1E0              shl ax,1
00000B2A  8BD8              mov bx,ax
00000B2C  8B87AC4B          mov ax,[bx+0x4bac]
00000B30  50                push ax
00000B31  A0AA37            mov al,[0x37aa]
00000B34  98                cbw
00000B35  D1E0              shl ax,1
00000B37  D1E0              shl ax,1
00000B39  8BD8              mov bx,ax
00000B3B  8A87EA2E          mov al,[bx+0x2eea]
00000B3F  98                cbw
00000B40  5A                pop dx
00000B41  03D0              add dx,ax
00000B43  83FA03            cmp dx,byte +0x3
00000B46  7D59              jnl 0xba1
00000B48  A0AA37            mov al,[0x37aa]
00000B4B  98                cbw
00000B4C  8BD8              mov bx,ax
00000B4E  80BF423D02        cmp byte [bx+0x3d42],0x2
00000B53  7226              jc 0xb7b
00000B55  33C0              xor ax,ax
00000B57  BA0A00            mov dx,0xa
00000B5A  50                push ax
00000B5B  52                push dx
00000B5C  A0AA37            mov al,[0x37aa]
00000B5F  98                cbw
00000B60  D1E0              shl ax,1
00000B62  D1E0              shl ax,1
00000B64  8BD8              mov bx,ax
00000B66  FFB7D43C          push word [bx+0x3cd4]
00000B6A  FFB7D23C          push word [bx+0x3cd2]
00000B6E  9A894F0000        call 0x0:0x4f89
00000B73  8946FC            mov [bp-0x4],ax
00000B76  8956FE            mov [bp-0x2],dx
00000B79  EB24              jmp short 0xb9f
00000B7B  33C0              xor ax,ax
00000B7D  BA1E00            mov dx,0x1e
00000B80  50                push ax
00000B81  52                push dx
00000B82  A0AA37            mov al,[0x37aa]
00000B85  98                cbw
00000B86  D1E0              shl ax,1
00000B88  D1E0              shl ax,1
00000B8A  8BD8              mov bx,ax
00000B8C  FFB7D43C          push word [bx+0x3cd4]
00000B90  FFB7D23C          push word [bx+0x3cd2]
00000B94  9A894F0000        call 0x0:0x4f89
00000B99  8946FC            mov [bp-0x4],ax
00000B9C  8956FE            mov [bp-0x2],dx
00000B9F  EB24              jmp short 0xbc5
00000BA1  33C0              xor ax,ax
00000BA3  BA3200            mov dx,0x32
00000BA6  50                push ax
00000BA7  52                push dx
00000BA8  A0AA37            mov al,[0x37aa]
00000BAB  98                cbw
00000BAC  D1E0              shl ax,1
00000BAE  D1E0              shl ax,1
00000BB0  8BD8              mov bx,ax
00000BB2  FFB7D43C          push word [bx+0x3cd4]
00000BB6  FFB7D23C          push word [bx+0x3cd2]
00000BBA  9A894F0000        call 0x0:0x4f89
00000BBF  8946FC            mov [bp-0x4],ax
00000BC2  8956FE            mov [bp-0x2],dx
00000BC5  A1A048            mov ax,[0x48a0]
00000BC8  99                cwd
00000BC9  3B56FE            cmp dx,[bp-0x2]
00000BCC  720B              jc 0xbd9
00000BCE  7705              ja 0xbd5
00000BD0  3B46FC            cmp ax,[bp-0x4]
00000BD3  7604              jna 0xbd9
00000BD5  B000              mov al,0x0
00000BD7  EB04              jmp short 0xbdd
00000BD9  B001              mov al,0x1
00000BDB  EB00              jmp short 0xbdd
00000BDD  8BE5              mov sp,bp
00000BDF  5D                pop bp
00000BE0  CB                retf

; ---- 08CF:0BE1 ----
00000BE1  55                push bp
00000BE2  8BEC              mov bp,sp
00000BE4  56                push si
00000BE5  A0AA37            mov al,[0x37aa]
00000BE8  98                cbw
00000BE9  8BD8              mov bx,ax
00000BEB  C687B2430D        mov byte [bx+0x43b2],0xd
00000BF0  9AE5180906        call 0x609:0x18e5
00000BF5  9A3E19CF08        call 0x8cf:0x193e
00000BFA  B85000            mov ax,0x50
00000BFD  50                push ax
00000BFE  33C0              xor ax,ax
00000C00  50                push ax
00000C01  9A5400A721        call 0x21a7:0x54
00000C06  59                pop cx
00000C07  59                pop cx
00000C08  A0AA37            mov al,[0x37aa]
00000C0B  98                cbw
00000C0C  8BD8              mov bx,ax
00000C0E  C687861500        mov byte [bx+0x1586],0x0
00000C13  9AF516CF08        call 0x8cf:0x16f5
00000C18  A0AA37            mov al,[0x37aa]
00000C1B  98                cbw
00000C1C  D1E0              shl ax,1
00000C1E  D1E0              shl ax,1
00000C20  50                push ax
00000C21  A1CC40            mov ax,[0x40cc]
00000C24  99                cwd
00000C25  5B                pop bx
00000C26  3997D43C          cmp [bx+0x3cd4],dx
00000C2A  7C56              jl 0xc82
00000C2C  7506              jnz 0xc34
00000C2E  3987D23C          cmp [bx+0x3cd2],ax
00000C32  724E              jc 0xc82
00000C34  B80500            mov ax,0x5
00000C37  50                push ax
00000C38  B80200            mov ax,0x2
00000C3B  50                push ax
00000C3C  FF361A38          push word [0x381a]
00000C40  FF361838          push word [0x3818]
00000C44  B81A00            mov ax,0x1a
00000C47  50                push ax
00000C48  A0AA37            mov al,[0x37aa]
00000C4B  98                cbw
00000C4C  B106              mov cl,0x6
00000C4E  D3E0              shl ax,cl
00000C50  052600            add ax,0x26
00000C53  50                push ax
00000C54  9A2505960A        call 0xa96:0x525
00000C59  83C40C            add sp,byte +0xc
00000C5C  33F6              xor si,si
00000C5E  EB1B              jmp short 0xc7b
00000C60  A0AA37            mov al,[0x37aa]
00000C63  98                cbw
00000C64  D1E0              shl ax,1
00000C66  D1E0              shl ax,1
00000C68  8BD8              mov bx,ax
00000C6A  C6808A1500        mov byte [bx+si+0x158a],0x0
00000C6F  33C0              xor ax,ax
00000C71  50                push ax
00000C72  56                push si
00000C73  9AB416CF08        call 0x8cf:0x16b4
00000C78  59                pop cx
00000C79  59                pop cx
00000C7A  46                inc si
00000C7B  83FE03            cmp si,byte +0x3
00000C7E  7EE0              jng 0xc60
00000C80  EB50              jmp short 0xcd2
00000C82  B80500            mov ax,0x5
00000C85  50                push ax
00000C86  B80800            mov ax,0x8
00000C89  50                push ax
00000C8A  FF361238          push word [0x3812]
00000C8E  FF361038          push word [0x3810]
00000C92  B81A00            mov ax,0x1a
00000C95  50                push ax
00000C96  A0AA37            mov al,[0x37aa]
00000C99  98                cbw
00000C9A  B106              mov cl,0x6
00000C9C  D3E0              shl ax,cl
00000C9E  052600            add ax,0x26
00000CA1  50                push ax
00000CA2  9A2505960A        call 0xa96:0x525
00000CA7  83C40C            add sp,byte +0xc
00000CAA  B80500            mov ax,0x5
00000CAD  50                push ax
00000CAE  B80800            mov ax,0x8
00000CB1  50                push ax
00000CB2  FF361638          push word [0x3816]
00000CB6  FF361438          push word [0x3814]
00000CBA  B82000            mov ax,0x20
00000CBD  50                push ax
00000CBE  A0AA37            mov al,[0x37aa]
00000CC1  98                cbw
00000CC2  B106              mov cl,0x6
00000CC4  D3E0              shl ax,cl
00000CC6  052600            add ax,0x26
00000CC9  50                push ax
00000CCA  9A2505960A        call 0xa96:0x525
00000CCF  83C40C            add sp,byte +0xc
00000CD2  5E                pop si
00000CD3  5D                pop bp
00000CD4  CB                retf

; ---- 08CF:0CD5 ----
00000CD5  55                push bp
00000CD6  8BEC              mov bp,sp
00000CD8  4C                dec sp
00000CD9  4C                dec sp
00000CDA  A0AA37            mov al,[0x37aa]
00000CDD  98                cbw
00000CDE  8BD8              mov bx,ax
00000CE0  8A878615          mov al,[bx+0x1586]
00000CE4  8846FE            mov [bp-0x2],al
00000CE7  807EFE03          cmp byte [bp-0x2],0x3
00000CEB  7E0B              jng 0xcf8
00000CED  A0AA37            mov al,[0x37aa]
00000CF0  98                cbw
00000CF1  8BD8              mov bx,ax
00000CF3  C687083303        mov byte [bx+0x3308],0x3
00000CF8  A0AA37            mov al,[0x37aa]
00000CFB  98                cbw
00000CFC  8BD8              mov bx,ax
00000CFE  80BF400801        cmp byte [bx+0x840],0x1
00000D03  755C              jnz 0xd61
00000D05  833EB41500        cmp word [0x15b4],byte +0x0
00000D0A  7555              jnz 0xd61
00000D0C  8A46FE            mov al,[bp-0x2]
00000D0F  98                cbw
00000D10  8BD8              mov bx,ax
00000D12  80BFED08FF        cmp byte [bx+0x8ed],0xff
00000D17  740F              jz 0xd28
00000D19  8A46FE            mov al,[bp-0x2]
00000D1C  98                cbw
00000D1D  8BD8              mov bx,ax
00000D1F  8A87ED08          mov al,[bx+0x8ed]
00000D23  8846FF            mov [bp-0x1],al
00000D26  EB0D              jmp short 0xd35
00000D28  B80100            mov ax,0x1
00000D2B  50                push ax
00000D2C  9ABA01FD21        call 0x21fd:0x1ba
00000D31  59                pop cx
00000D32  8846FF            mov [bp-0x1],al
00000D35  807EFF01          cmp byte [bp-0x1],0x1
00000D39  7504              jnz 0xd3f
00000D3B  B001              mov al,0x1
00000D3D  EB02              jmp short 0xd41
00000D3F  B000              mov al,0x0
00000D41  50                push ax
00000D42  A0AA37            mov al,[0x37aa]
00000D45  98                cbw
00000D46  8BD8              mov bx,ax
00000D48  58                pop ax
00000D49  88870833          mov [bx+0x3308],al
00000D4D  9AEF0DCF08        call 0x8cf:0xdef
00000D52  0AC0              or al,al
00000D54  750B              jnz 0xd61
00000D56  A0AA37            mov al,[0x37aa]
00000D59  98                cbw
00000D5A  8BD8              mov bx,ax
00000D5C  C687083303        mov byte [bx+0x3308],0x3
00000D61  803EA215FF        cmp byte [0x15a2],0xff
00000D66  740B              jz 0xd73
00000D68  A0AA37            mov al,[0x37aa]
00000D6B  98                cbw
00000D6C  8BD8              mov bx,ax
00000D6E  C687083303        mov byte [bx+0x3308],0x3
00000D73  A0AA37            mov al,[0x37aa]
00000D76  98                cbw
00000D77  8BD8              mov bx,ax
00000D79  8A870833          mov al,[bx+0x3308]
00000D7D  98                cbw
00000D7E  0BC0              or ax,ax
00000D80  745D              jz 0xddf
00000D82  3D0100            cmp ax,0x1
00000D85  744C              jz 0xdd3
00000D87  3D0300            cmp ax,0x3
00000D8A  7402              jz 0xd8e
00000D8C  EB5D              jmp short 0xdeb
00000D8E  9AF516CF08        call 0x8cf:0x16f5
00000D93  A0AA37            mov al,[0x37aa]
00000D96  98                cbw
00000D97  8BD8              mov bx,ax
00000D99  8A87423D          mov al,[bx+0x3d42]
00000D9D  B400              mov ah,0x0
00000D9F  50                push ax
00000DA0  B80600            mov ax,0x6
00000DA3  50                push ax
00000DA4  A0AA37            mov al,[0x37aa]
00000DA7  98                cbw
00000DA8  B106              mov cl,0x6
00000DAA  D3E0              shl ax,cl
00000DAC  052000            add ax,0x20
00000DAF  50                push ax
00000DB0  9A79180906        call 0x609:0x1879
00000DB5  83C406            add sp,byte +0x6
00000DB8  A0AA37            mov al,[0x37aa]
00000DBB  98                cbw
00000DBC  8BD8              mov bx,ax
00000DBE  C687B2430A        mov byte [bx+0x43b2],0xa
00000DC3  B85100            mov ax,0x51
00000DC6  50                push ax
00000DC7  33C0              xor ax,ax
00000DC9  50                push ax
00000DCA  9A5400A721        call 0x21a7:0x54
00000DCF  59                pop cx
00000DD0  59                pop cx
00000DD1  EB18              jmp short 0xdeb
00000DD3  B80100            mov ax,0x1
00000DD6  50                push ax
00000DD7  9A530ECF08        call 0x8cf:0xe53
00000DDC  59                pop cx
00000DDD  EB0C              jmp short 0xdeb
00000DDF  B80200            mov ax,0x2
00000DE2  50                push ax
00000DE3  9A530ECF08        call 0x8cf:0xe53
00000DE8  59                pop cx
00000DE9  EB00              jmp short 0xdeb
00000DEB  8BE5              mov sp,bp
00000DED  5D                pop bp
00000DEE  CB                retf

; ---- 08CF:0DEF ----
00000DEF  55                push bp
00000DF0  8BEC              mov bp,sp
00000DF2  A0AA37            mov al,[0x37aa]
00000DF5  98                cbw
00000DF6  D1E0              shl ax,1
00000DF8  D1E0              shl ax,1
00000DFA  50                push ax
00000DFB  A1CC40            mov ax,[0x40cc]
00000DFE  99                cwd
00000DFF  5B                pop bx
00000E00  3997D43C          cmp [bx+0x3cd4],dx
00000E04  7C47              jl 0xe4d
00000E06  7506              jnz 0xe0e
00000E08  3987D23C          cmp [bx+0x3cd2],ax
00000E0C  723F              jc 0xe4d
00000E0E  803EEE08FF        cmp byte [0x8ee],0xff
00000E13  7404              jz 0xe19
00000E15  B001              mov al,0x1
00000E17  EB38              jmp short 0xe51
00000E19  33C0              xor ax,ax
00000E1B  BA0500            mov dx,0x5
00000E1E  50                push ax
00000E1F  52                push dx
00000E20  A0AA37            mov al,[0x37aa]
00000E23  98                cbw
00000E24  D1E0              shl ax,1
00000E26  D1E0              shl ax,1
00000E28  8BD8              mov bx,ax
00000E2A  FFB7D43C          push word [bx+0x3cd4]
00000E2E  FFB7D23C          push word [bx+0x3cd2]
00000E32  9A894F0000        call 0x0:0x4f89
00000E37  50                push ax
00000E38  A1CC40            mov ax,[0x40cc]
00000E3B  52                push dx
00000E3C  99                cwd
00000E3D  5B                pop bx
00000E3E  3BDA              cmp bx,dx
00000E40  5A                pop dx
00000E41  7C0A              jl 0xe4d
00000E43  7504              jnz 0xe49
00000E45  3BD0              cmp dx,ax
00000E47  7204              jc 0xe4d
00000E49  B001              mov al,0x1
00000E4B  EB04              jmp short 0xe51
00000E4D  B000              mov al,0x0
00000E4F  EB00              jmp short 0xe51
00000E51  5D                pop bp
00000E52  CB                retf

; ---- 08CF:0E53 ----
00000E53  55                push bp
00000E54  8BEC              mov bp,sp
00000E56  56                push si
00000E57  A0AA37            mov al,[0x37aa]
00000E5A  98                cbw
00000E5B  8BD8              mov bx,ax
00000E5D  8A878615          mov al,[bx+0x1586]
00000E61  98                cbw
00000E62  8BF0              mov si,ax
00000E64  A0AA37            mov al,[0x37aa]
00000E67  98                cbw
00000E68  D1E0              shl ax,1
00000E6A  D1E0              shl ax,1
00000E6C  50                push ax
00000E6D  A1CC40            mov ax,[0x40cc]
00000E70  99                cwd
00000E71  5B                pop bx
00000E72  3997D43C          cmp [bx+0x3cd4],dx
00000E76  7F0B              jg 0xe83
00000E78  7C06              jl 0xe80
00000E7A  3987D23C          cmp [bx+0x3cd2],ax
00000E7E  7303              jnc 0xe83
00000E80  E9C700            jmp 0xf4a
00000E83  A0AA37            mov al,[0x37aa]
00000E86  98                cbw
00000E87  D1E0              shl ax,1
00000E89  D1E0              shl ax,1
00000E8B  8A5606            mov dl,[bp+0x6]
00000E8E  8BD8              mov bx,ax
00000E90  88908A15          mov [bx+si+0x158a],dl
00000E94  FF7606            push word [bp+0x6]
00000E97  56                push si
00000E98  9AB416CF08        call 0x8cf:0x16b4
00000E9D  59                pop cx
00000E9E  59                pop cx
00000E9F  837E0601          cmp word [bp+0x6],byte +0x1
00000EA3  7510              jnz 0xeb5
00000EA5  B82D00            mov ax,0x2d
00000EA8  50                push ax
00000EA9  33C0              xor ax,ax
00000EAB  50                push ax
00000EAC  9A5400A721        call 0x21a7:0x54
00000EB1  59                pop cx
00000EB2  59                pop cx
00000EB3  EB0E              jmp short 0xec3
00000EB5  B82E00            mov ax,0x2e
00000EB8  50                push ax
00000EB9  33C0              xor ax,ax
00000EBB  50                push ax
00000EBC  9A5400A721        call 0x21a7:0x54
00000EC1  59                pop cx
00000EC2  59                pop cx
00000EC3  A0AA37            mov al,[0x37aa]
00000EC6  98                cbw
00000EC7  8BD8              mov bx,ax
00000EC9  80BF400800        cmp byte [bx+0x840],0x0
00000ECE  7516              jnz 0xee6
00000ED0  A0AA37            mov al,[0x37aa]
00000ED3  98                cbw
00000ED4  D1E0              shl ax,1
00000ED6  D1E0              shl ax,1
00000ED8  8BD8              mov bx,ax
00000EDA  C78786372C01      mov word [bx+0x3786],0x12c
00000EE0  C78788370000      mov word [bx+0x3788],0x0
00000EE6  A0AA37            mov al,[0x37aa]
00000EE9  98                cbw
00000EEA  8BD8              mov bx,ax
00000EEC  8A878615          mov al,[bx+0x1586]
00000EF0  FEC0              inc al
00000EF2  50                push ax
00000EF3  A0AA37            mov al,[0x37aa]
00000EF6  98                cbw
00000EF7  8BD8              mov bx,ax
00000EF9  58                pop ax
00000EFA  88878615          mov [bx+0x1586],al
00000EFE  3C03              cmp al,0x3
00000F00  7E48              jng 0xf4a
00000F02  A1CC40            mov ax,[0x40cc]
00000F05  99                cwd
00000F06  50                push ax
00000F07  A0AA37            mov al,[0x37aa]
00000F0A  98                cbw
00000F0B  D1E0              shl ax,1
00000F0D  D1E0              shl ax,1
00000F0F  8BD8              mov bx,ax
00000F11  58                pop ax
00000F12  2987D23C          sub [bx+0x3cd2],ax
00000F16  1997D43C          sbb [bx+0x3cd4],dx
00000F1A  A0AA37            mov al,[0x37aa]
00000F1D  98                cbw
00000F1E  8BD8              mov bx,ax
00000F20  8A87423D          mov al,[bx+0x3d42]
00000F24  B400              mov ah,0x0
00000F26  50                push ax
00000F27  B80600            mov ax,0x6
00000F2A  50                push ax
00000F2B  A0AA37            mov al,[0x37aa]
00000F2E  98                cbw
00000F2F  B106              mov cl,0x6
00000F31  D3E0              shl ax,cl
00000F33  052000            add ax,0x20
00000F36  50                push ax
00000F37  9A79180906        call 0x609:0x1879
00000F3C  83C406            add sp,byte +0x6
00000F3F  A0AA37            mov al,[0x37aa]
00000F42  98                cbw
00000F43  8BD8              mov bx,ax
00000F45  C687B2430A        mov byte [bx+0x43b2],0xa
00000F4A  5E                pop si
00000F4B  5D                pop bp
00000F4C  CB                retf

; ---- 08CF:0F4D ----
00000F4D  55                push bp
00000F4E  8BEC              mov bp,sp
00000F50  56                push si
00000F51  803EA215FF        cmp byte [0x15a2],0xff
00000F56  753C              jnz 0xf94
00000F58  A18037            mov ax,[0x3780]
00000F5B  2B069A15          sub ax,[0x159a]
00000F5F  3B06E800          cmp ax,[0xe8]
00000F63  742F              jz 0xf94
00000F65  A18037            mov ax,[0x3780]
00000F68  2B069A15          sub ax,[0x159a]
00000F6C  A3E800            mov [0xe8],ax
00000F6F  B80500            mov ax,0x5
00000F72  50                push ax
00000F73  B80E00            mov ax,0xe
00000F76  50                push ax
00000F77  B80100            mov ax,0x1
00000F7A  50                push ax
00000F7B  A1E800            mov ax,[0xe8]
00000F7E  99                cwd
00000F7F  52                push dx
00000F80  50                push ax
00000F81  B87200            mov ax,0x72
00000F84  50                push ax
00000F85  A1A015            mov ax,[0x15a0]
00000F88  052900            add ax,0x29
00000F8B  50                push ax
00000F8C  9A7A05960A        call 0xa96:0x57a
00000F91  83C40E            add sp,byte +0xe
00000F94  A18037            mov ax,[0x3780]
00000F97  3B069A15          cmp ax,[0x159a]
00000F9B  7F07              jg 0xfa4
00000F9D  803EA215F6        cmp byte [0x15a2],0xf6
00000FA2  7503              jnz 0xfa7
00000FA4  E9B201            jmp 0x1159
00000FA7  A0A215            mov al,[0x15a2]
00000FAA  FEC0              inc al
00000FAC  A2A215            mov [0x15a2],al
00000FAF  3C03              cmp al,0x3
00000FB1  7E03              jng 0xfb6
00000FB3  E97C01            jmp 0x1132
00000FB6  A0A215            mov al,[0x15a2]
00000FB9  98                cbw
00000FBA  8BD8              mov bx,ax
00000FBC  80BFED08FF        cmp byte [bx+0x8ed],0xff
00000FC1  7423              jz 0xfe6
00000FC3  A0A215            mov al,[0x15a2]
00000FC6  98                cbw
00000FC7  8BD8              mov bx,ax
00000FC9  8A87ED08          mov al,[bx+0x8ed]
00000FCD  50                push ax
00000FCE  A0A215            mov al,[0x15a2]
00000FD1  98                cbw
00000FD2  8BD8              mov bx,ax
00000FD4  58                pop ax
00000FD5  8887A715          mov [bx+0x15a7],al
00000FD9  A0A215            mov al,[0x15a2]
00000FDC  98                cbw
00000FDD  8BD8              mov bx,ax
00000FDF  C687ED08FF        mov byte [bx+0x8ed],0xff
00000FE4  EB18              jmp short 0xffe
00000FE6  B80100            mov ax,0x1
00000FE9  50                push ax
00000FEA  9ABA01FD21        call 0x21fd:0x1ba
00000FEF  59                pop cx
00000FF0  FEC0              inc al
00000FF2  50                push ax
00000FF3  A0A215            mov al,[0x15a2]
00000FF6  98                cbw
00000FF7  8BD8              mov bx,ax
00000FF9  58                pop ax
00000FFA  8887A715          mov [bx+0x15a7],al
00000FFE  B8A800            mov ax,0xa8
00001001  50                push ax
00001002  A0A215            mov al,[0x15a2]
00001005  98                cbw
00001006  B105              mov cl,0x5
00001008  D3E0              shl ax,cl
0000100A  052000            add ax,0x20
0000100D  50                push ax
0000100E  B81600            mov ax,0x16
00001011  50                push ax
00001012  B82000            mov ax,0x20
00001015  50                push ax
00001016  A0A215            mov al,[0x15a2]
00001019  98                cbw
0000101A  8BD8              mov bx,ax
0000101C  8A87A715          mov al,[bx+0x15a7]
00001020  98                cbw
00001021  BA1600            mov dx,0x16
00001024  F7EA              imul dx
00001026  50                push ax
00001027  B8A000            mov ax,0xa0
0000102A  50                push ax
0000102B  9A3000960A        call 0xa96:0x30
00001030  83C40C            add sp,byte +0xc
00001033  A0A215            mov al,[0x15a2]
00001036  98                cbw
00001037  8BD8              mov bx,ax
00001039  80BFA71501        cmp byte [bx+0x15a7],0x1
0000103E  7511              jnz 0x1051
00001040  B82F00            mov ax,0x2f
00001043  50                push ax
00001044  B80100            mov ax,0x1
00001047  50                push ax
00001048  9A5400A721        call 0x21a7:0x54
0000104D  59                pop cx
0000104E  59                pop cx
0000104F  EB0F              jmp short 0x1060
00001051  B83000            mov ax,0x30
00001054  50                push ax
00001055  B80100            mov ax,0x1
00001058  50                push ax
00001059  9A5400A721        call 0x21a7:0x54
0000105E  59                pop cx
0000105F  59                pop cx
00001060  A18037            mov ax,[0x3780]
00001063  48                dec ax
00001064  A39A15            mov [0x159a],ax
00001067  803EA21503        cmp byte [0x15a2],0x3
0000106C  7403              jz 0x1071
0000106E  E9BF00            jmp 0x1130
00001071  C606AA3700        mov byte [0x37aa],0x0
00001076  E99A00            jmp 0x1113
00001079  A0AA37            mov al,[0x37aa]
0000107C  98                cbw
0000107D  8BD8              mov bx,ax
0000107F  C687A31500        mov byte [bx+0x15a3],0x0
00001084  A0AA37            mov al,[0x37aa]
00001087  98                cbw
00001088  8BD8              mov bx,ax
0000108A  80BF861503        cmp byte [bx+0x1586],0x3
0000108F  7F02              jg 0x1093
00001091  EB7C              jmp short 0x110f
00001093  33F6              xor si,si
00001095  EB1F              jmp short 0x10b6
00001097  A0AA37            mov al,[0x37aa]
0000109A  98                cbw
0000109B  D1E0              shl ax,1
0000109D  D1E0              shl ax,1
0000109F  8BD8              mov bx,ax
000010A1  8A808A15          mov al,[bx+si+0x158a]
000010A5  3A84A715          cmp al,[si+0x15a7]
000010A9  750A              jnz 0x10b5
000010AB  A0AA37            mov al,[0x37aa]
000010AE  98                cbw
000010AF  8BD8              mov bx,ax
000010B1  FE87A315          inc byte [bx+0x15a3]
000010B5  46                inc si
000010B6  83FE03            cmp si,byte +0x3
000010B9  7EDC              jng 0x1097
000010BB  A0AA37            mov al,[0x37aa]
000010BE  98                cbw
000010BF  8BD8              mov bx,ax
000010C1  80BFA31504        cmp byte [bx+0x15a3],0x4
000010C6  7547              jnz 0x110f
000010C8  A1A234            mov ax,[0x34a2]
000010CB  99                cwd
000010CC  50                push ax
000010CD  A0AA37            mov al,[0x37aa]
000010D0  98                cbw
000010D1  D1E0              shl ax,1
000010D3  D1E0              shl ax,1
000010D5  8BD8              mov bx,ax
000010D7  58                pop ax
000010D8  0187D23C          add [bx+0x3cd2],ax
000010DC  1197D43C          adc [bx+0x3cd4],dx
000010E0  A0AA37            mov al,[0x37aa]
000010E3  98                cbw
000010E4  8BD8              mov bx,ax
000010E6  80BFB2430A        cmp byte [bx+0x43b2],0xa
000010EB  7407              jz 0x10f4
000010ED  9A3E19CF08        call 0x8cf:0x193e
000010F2  EB1B              jmp short 0x110f
000010F4  33C0              xor ax,ax
000010F6  50                push ax
000010F7  B80600            mov ax,0x6
000010FA  50                push ax
000010FB  A0AA37            mov al,[0x37aa]
000010FE  98                cbw
000010FF  B106              mov cl,0x6
00001101  D3E0              shl ax,cl
00001103  052000            add ax,0x20
00001106  50                push ax
00001107  9A79180906        call 0x609:0x1879
0000110C  83C406            add sp,byte +0x6
0000110F  FE06AA37          inc byte [0x37aa]
00001113  803EAA3704        cmp byte [0x37aa],0x4
00001118  7D03              jnl 0x111d
0000111A  E95CFF            jmp 0x1079
0000111D  9ACB062508        call 0x825:0x6cb
00001122  C606AF1500        mov byte [0x15af],0x0
00001127  A18037            mov ax,[0x3780]
0000112A  05FEFF            add ax,0xfffe
0000112D  A39A15            mov [0x159a],ax
00001130  EB27              jmp short 0x1159
00001132  9A1F16CF08        call 0x8cf:0x161f
00001137  C606AA3700        mov byte [0x37aa],0x0
0000113C  EB14              jmp short 0x1152
0000113E  9AF516CF08        call 0x8cf:0x16f5
00001143  A0AA37            mov al,[0x37aa]
00001146  98                cbw
00001147  8BD8              mov bx,ax
00001149  C687861500        mov byte [bx+0x1586],0x0
0000114E  FE06AA37          inc byte [0x37aa]
00001152  803EAA3704        cmp byte [0x37aa],0x4
00001157  7CE5              jl 0x113e
00001159  5E                pop si
0000115A  5D                pop bp
0000115B  CB                retf

; ---- 08CF:115C ----
0000115C  55                push bp
0000115D  8BEC              mov bp,sp
0000115F  A1EA00            mov ax,[0xea]
00001162  48                dec ax
00001163  A3EA00            mov [0xea],ax
00001166  0BC0              or ax,ax
00001168  756B              jnz 0x11d5
0000116A  C606AA3700        mov byte [0x37aa],0x0
0000116F  EB46              jmp short 0x11b7
00001171  A0AA37            mov al,[0x37aa]
00001174  98                cbw
00001175  8BD8              mov bx,ax
00001177  80BFA31504        cmp byte [bx+0x15a3],0x4
0000117C  7402              jz 0x1180
0000117E  EB33              jmp short 0x11b3
00001180  803EAF1503        cmp byte [0x15af],0x3
00001185  7F11              jg 0x1198
00001187  33C0              xor ax,ax
00001189  50                push ax
0000118A  A0AF15            mov al,[0x15af]
0000118D  98                cbw
0000118E  50                push ax
0000118F  9AB416CF08        call 0x8cf:0x16b4
00001194  59                pop cx
00001195  59                pop cx
00001196  EB1B              jmp short 0x11b3
00001198  A0AF15            mov al,[0x15af]
0000119B  98                cbw
0000119C  8BD8              mov bx,ax
0000119E  8A87A315          mov al,[bx+0x15a3]
000011A2  98                cbw
000011A3  50                push ax
000011A4  A0AF15            mov al,[0x15af]
000011A7  98                cbw
000011A8  05FCFF            add ax,0xfffc
000011AB  50                push ax
000011AC  9AB416CF08        call 0x8cf:0x16b4
000011B1  59                pop cx
000011B2  59                pop cx
000011B3  FE06AA37          inc byte [0x37aa]
000011B7  803EAA3704        cmp byte [0x37aa],0x4
000011BC  7CB3              jl 0x1171
000011BE  A0AF15            mov al,[0x15af]
000011C1  FEC0              inc al
000011C3  A2AF15            mov [0x15af],al
000011C6  3C08              cmp al,0x8
000011C8  7505              jnz 0x11cf
000011CA  C606AF1500        mov byte [0x15af],0x0
000011CF  C706EA000200      mov word [0xea],0x2
000011D5  5D                pop bp
000011D6  CB                retf

; ---- 08CF:11D7 ----
000011D7  55                push bp
000011D8  8BEC              mov bp,sp
000011DA  83EC08            sub sp,byte +0x8
000011DD  C746FC0000        mov word [bp-0x4],0x0
000011E2  C746FE0000        mov word [bp-0x2],0x0
000011E7  C606AA3700        mov byte [0x37aa],0x0
000011EC  EB1C              jmp short 0x120a
000011EE  A0AA37            mov al,[0x37aa]
000011F1  98                cbw
000011F2  D1E0              shl ax,1
000011F4  D1E0              shl ax,1
000011F6  8BD8              mov bx,ax
000011F8  8B87D43C          mov ax,[bx+0x3cd4]
000011FC  8B97D23C          mov dx,[bx+0x3cd2]
00001200  0156FC            add [bp-0x4],dx
00001203  1146FE            adc [bp-0x2],ax
00001206  FE06AA37          inc byte [0x37aa]
0000120A  803EAA3704        cmp byte [0x37aa],0x4
0000120F  7CDD              jl 0x11ee
00001211  33C0              xor ax,ax
00001213  BAA000            mov dx,0xa0
00001216  50                push ax
00001217  52                push dx
00001218  FF76FE            push word [bp-0x2]
0000121B  FF76FC            push word [bp-0x4]
0000121E  9A894F0000        call 0x0:0x4f89
00001223  8946F8            mov [bp-0x8],ax
00001226  8956FA            mov [bp-0x6],dx
00001229  8B46FA            mov ax,[bp-0x6]
0000122C  8B56F8            mov dx,[bp-0x8]
0000122F  8916F03C          mov [0x3cf0],dx
00001233  A3F23C            mov [0x3cf2],ax
00001236  A19A4B            mov ax,[0x4b9a]
00001239  059B00            add ax,0x9b
0000123C  99                cwd
0000123D  3B163844          cmp dx,[0x4438]
00001241  7C16              jl 0x1259
00001243  7F06              jg 0x124b
00001245  3B063644          cmp ax,[0x4436]
00001249  760E              jna 0x1259
0000124B  B80100            mov ax,0x1
0000124E  50                push ax
0000124F  9ABA01FD21        call 0x21fd:0x1ba
00001254  59                pop cx
00001255  0BC0              or ax,ax
00001257  7515              jnz 0x126e
00001259  A19A4B            mov ax,[0x4b9a]
0000125C  052D00            add ax,0x2d
0000125F  99                cwd
00001260  3B163844          cmp dx,[0x4438]
00001264  7C74              jl 0x12da
00001266  7F06              jg 0x126e
00001268  3B063644          cmp ax,[0x4436]
0000126C  766C              jna 0x12da
0000126E  A19A4B            mov ax,[0x4b9a]
00001271  056400            add ax,0x64
00001274  99                cwd
00001275  3B163844          cmp dx,[0x4438]
00001279  7C27              jl 0x12a2
0000127B  7F06              jg 0x1283
0000127D  3B063644          cmp ax,[0x4436]
00001281  761F              jna 0x12a2
00001283  B81E00            mov ax,0x1e
00001286  50                push ax
00001287  9ABA01FD21        call 0x21fd:0x1ba
0000128C  59                pop cx
0000128D  8B169A4B          mov dx,[0x4b9a]
00001291  83C264            add dx,byte +0x64
00001294  03D0              add dx,ax
00001296  89163644          mov [0x4436],dx
0000129A  C70638440000      mov word [0x4438],0x0
000012A0  EB13              jmp short 0x12b5
000012A2  B81400            mov ax,0x14
000012A5  50                push ax
000012A6  9ABA01FD21        call 0x21fd:0x1ba
000012AB  59                pop cx
000012AC  01063644          add [0x4436],ax
000012B0  8316384400        adc word [0x4438],byte +0x0
000012B5  A19A4B            mov ax,[0x4b9a]
000012B8  05A000            add ax,0xa0
000012BB  99                cwd
000012BC  3B163844          cmp dx,[0x4438]
000012C0  7F16              jg 0x12d8
000012C2  7C06              jl 0x12ca
000012C4  3B063644          cmp ax,[0x4436]
000012C8  730E              jnc 0x12d8
000012CA  A19A4B            mov ax,[0x4b9a]
000012CD  05A000            add ax,0xa0
000012D0  99                cwd
000012D1  A33644            mov [0x4436],ax
000012D4  89163844          mov [0x4438],dx
000012D8  EB6A              jmp short 0x1344
000012DA  A19A4B            mov ax,[0x4b9a]
000012DD  056400            add ax,0x64
000012E0  99                cwd
000012E1  3B163844          cmp dx,[0x4438]
000012E5  7F27              jg 0x130e
000012E7  7C06              jl 0x12ef
000012E9  3B063644          cmp ax,[0x4436]
000012ED  731F              jnc 0x130e
000012EF  B81E00            mov ax,0x1e
000012F2  50                push ax
000012F3  9ABA01FD21        call 0x21fd:0x1ba
000012F8  59                pop cx
000012F9  8B169A4B          mov dx,[0x4b9a]
000012FD  83C264            add dx,byte +0x64
00001300  2BD0              sub dx,ax
00001302  89163644          mov [0x4436],dx
00001306  C70638440000      mov word [0x4438],0x0
0000130C  EB13              jmp short 0x1321
0000130E  B81400            mov ax,0x14
00001311  50                push ax
00001312  9ABA01FD21        call 0x21fd:0x1ba
00001317  59                pop cx
00001318  29063644          sub [0x4436],ax
0000131C  831E384400        sbb word [0x4438],byte +0x0
00001321  A19A4B            mov ax,[0x4b9a]
00001324  052800            add ax,0x28
00001327  99                cwd
00001328  3B163844          cmp dx,[0x4438]
0000132C  7C16              jl 0x1344
0000132E  7F06              jg 0x1336
00001330  3B063644          cmp ax,[0x4436]
00001334  760E              jna 0x1344
00001336  A19A4B            mov ax,[0x4b9a]
00001339  052800            add ax,0x28
0000133C  99                cwd
0000133D  A33644            mov [0x4436],ax
00001340  89163844          mov [0x4438],dx
00001344  8BE5              mov sp,bp
00001346  5D                pop bp
00001347  CB                retf

; ---- 08CF:1348 ----
00001348  55                push bp
00001349  8BEC              mov bp,sp
0000134B  56                push si
0000134C  57                push di
0000134D  9A1B00960A        call 0xa96:0x1b
00001352  9AD91E0906        call 0x609:0x1ed9
00001357  B84C00            mov ax,0x4c
0000135A  50                push ax
0000135B  B84001            mov ax,0x140
0000135E  50                push ax
0000135F  33C0              xor ax,ax
00001361  50                push ax
00001362  33C0              xor ax,ax
00001364  50                push ax
00001365  9A56120906        call 0x609:0x1256
0000136A  83C408            add sp,byte +0x8
0000136D  B86E00            mov ax,0x6e
00001370  50                push ax
00001371  B80001            mov ax,0x100
00001374  50                push ax
00001375  B85000            mov ax,0x50
00001378  50                push ax
00001379  B82000            mov ax,0x20
0000137C  50                push ax
0000137D  9AC5130906        call 0x609:0x13c5
00001382  83C408            add sp,byte +0x8
00001385  B86E00            mov ax,0x6e
00001388  50                push ax
00001389  B80001            mov ax,0x100
0000138C  50                push ax
0000138D  B85000            mov ax,0x50
00001390  50                push ax
00001391  B82000            mov ax,0x20
00001394  50                push ax
00001395  B81600            mov ax,0x16
00001398  50                push ax
00001399  B82000            mov ax,0x20
0000139C  50                push ax
0000139D  B84200            mov ax,0x42
000013A0  50                push ax
000013A1  B8A000            mov ax,0xa0
000013A4  50                push ax
000013A5  9A6702960A        call 0xa96:0x267
000013AA  83C410            add sp,byte +0x10
000013AD  33F6              xor si,si
000013AF  EB41              jmp short 0x13f2
000013B1  33FF              xor di,di
000013B3  EB37              jmp short 0x13ec
000013B5  B80100            mov ax,0x1
000013B8  50                push ax
000013B9  9ABA01FD21        call 0x21fd:0x1ba
000013BE  59                pop cx
000013BF  0BC0              or ax,ax
000013C1  7528              jnz 0x13eb
000013C3  B80200            mov ax,0x2
000013C6  50                push ax
000013C7  9ABA01FD21        call 0x21fd:0x1ba
000013CC  59                pop cx
000013CD  50                push ax
000013CE  8BC6              mov ax,si
000013D0  BA1600            mov dx,0x16
000013D3  F7EA              imul dx
000013D5  059200            add ax,0x92
000013D8  50                push ax
000013D9  8BC7              mov ax,di
000013DB  B105              mov cl,0x5
000013DD  D3E0              shl ax,cl
000013DF  052000            add ax,0x20
000013E2  50                push ax
000013E3  9AF515CF08        call 0x8cf:0x15f5
000013E8  83C406            add sp,byte +0x6
000013EB  47                inc di
000013EC  83FF07            cmp di,byte +0x7
000013EF  7EC4              jng 0x13b5
000013F1  46                inc si
000013F2  83FE01            cmp si,byte +0x1
000013F5  7EBA              jng 0x13b1
000013F7  FF36A015          push word [0x15a0]
000013FB  33C0              xor ax,ax
000013FD  50                push ax
000013FE  9AC615CF08        call 0x8cf:0x15c6
00001403  59                pop cx
00001404  59                pop cx
00001405  803EBA4501        cmp byte [0x45ba],0x1
0000140A  7514              jnz 0x1420
0000140C  FF36B015          push word [0x15b0]
00001410  B80100            mov ax,0x1
00001413  50                push ax
00001414  9AC615CF08        call 0x8cf:0x15c6
00001419  59                pop cx
0000141A  59                pop cx
0000141B  9A8119CF08        call 0x8cf:0x1981
00001420  FF36B815          push word [0x15b8]
00001424  B80200            mov ax,0x2
00001427  50                push ax
00001428  9AC615CF08        call 0x8cf:0x15c6
0000142D  59                pop cx
0000142E  59                pop cx
0000142F  803EF24B01        cmp byte [0x4bf2],0x1
00001434  7536              jnz 0x146c
00001436  FF36B215          push word [0x15b2]
0000143A  B80300            mov ax,0x3
0000143D  50                push ax
0000143E  9AC615CF08        call 0x8cf:0x15c6
00001443  59                pop cx
00001444  59                pop cx
00001445  B80C00            mov ax,0xc
00001448  50                push ax
00001449  B80200            mov ax,0x2
0000144C  50                push ax
0000144D  B80300            mov ax,0x3
00001450  50                push ax
00001451  FF363844          push word [0x4438]
00001455  FF363644          push word [0x4436]
00001459  B86A00            mov ax,0x6a
0000145C  50                push ax
0000145D  A1B215            mov ax,[0x15b2]
00001460  051400            add ax,0x14
00001463  50                push ax
00001464  9A7A05960A        call 0xa96:0x57a
00001469  83C40E            add sp,byte +0xe
0000146C  9AB518CF08        call 0x8cf:0x18b5
00001471  9A1F16CF08        call 0x8cf:0x161f
00001476  C706B6155800      mov word [0x15b6],0x58
0000147C  803EBA4501        cmp byte [0x45ba],0x1
00001481  7509              jnz 0x148c
00001483  A1B615            mov ax,[0x15b6]
00001486  05E3FF            add ax,0xffe3
00001489  A3B615            mov [0x15b6],ax
0000148C  803EF24B01        cmp byte [0x4bf2],0x1
00001491  7509              jnz 0x149c
00001493  A1B615            mov ax,[0x15b6]
00001496  05D4FF            add ax,0xffd4
00001499  A3B615            mov [0x15b6],ax
0000149C  B80300            mov ax,0x3
0000149F  50                push ax
000014A0  A1CC40            mov ax,[0x40cc]
000014A3  99                cwd
000014A4  52                push dx
000014A5  50                push ax
000014A6  FF362A38          push word [0x382a]
000014AA  FF362838          push word [0x3828]
000014AE  9A4315CF08        call 0x8cf:0x1543
000014B3  83C40A            add sp,byte +0xa
000014B6  803EBA4501        cmp byte [0x45ba],0x1
000014BB  751A              jnz 0x14d7
000014BD  B80300            mov ax,0x3
000014C0  50                push ax
000014C1  A1F443            mov ax,[0x43f4]
000014C4  99                cwd
000014C5  52                push dx
000014C6  50                push ax
000014C7  FF362638          push word [0x3826]
000014CB  FF362438          push word [0x3824]
000014CF  9A4315CF08        call 0x8cf:0x1543
000014D4  83C40A            add sp,byte +0xa
000014D7  B80300            mov ax,0x3
000014DA  50                push ax
000014DB  A1A048            mov ax,[0x48a0]
000014DE  99                cwd
000014DF  52                push dx
000014E0  50                push ax
000014E1  FF362238          push word [0x3822]
000014E5  FF362038          push word [0x3820]
000014E9  9A4315CF08        call 0x8cf:0x1543
000014EE  83C40A            add sp,byte +0xa
000014F1  803EF24B01        cmp byte [0x4bf2],0x1
000014F6  7538              jnz 0x1530
000014F8  B80400            mov ax,0x4
000014FB  50                push ax
000014FC  33C0              xor ax,ax
000014FE  BA6400            mov dx,0x64
00001501  50                push ax
00001502  52                push dx
00001503  8B0EF23C          mov cx,[0x3cf2]
00001507  8B1EF03C          mov bx,[0x3cf0]
0000150B  8B163844          mov dx,[0x4438]
0000150F  A13644            mov ax,[0x4436]
00001512  9A4E4F0000        call 0x0:0x4f4e
00001517  52                push dx
00001518  50                push ax
00001519  9A894F0000        call 0x0:0x4f89
0000151E  52                push dx
0000151F  50                push ax
00001520  FF361E38          push word [0x381e]
00001524  FF361C38          push word [0x381c]
00001528  9A4315CF08        call 0x8cf:0x1543
0000152D  83C40A            add sp,byte +0xa
00001530  9AA806960A        call 0xa96:0x6a8
00001535  FF362E08          push word [0x82e]
00001539  9A3F0C960A        call 0xa96:0xc3f
0000153E  59                pop cx
0000153F  5F                pop di
00001540  5E                pop si
00001541  5D                pop bp
00001542  CB                retf

; ---- 08CF:1543 ----
00001543  55                push bp
00001544  8BEC              mov bp,sp
00001546  B80100            mov ax,0x1
00001549  50                push ax
0000154A  B8FFFF            mov ax,0xffff
0000154D  50                push ax
0000154E  B80B00            mov ax,0xb
00001551  50                push ax
00001552  B80900            mov ax,0x9
00001555  50                push ax
00001556  FF7608            push word [bp+0x8]
00001559  FF7606            push word [bp+0x6]
0000155C  B84500            mov ax,0x45
0000155F  50                push ax
00001560  FF36B615          push word [0x15b6]
00001564  9A4D00690A        call 0xa69:0x4d
00001569  83C410            add sp,byte +0x10
0000156C  FF7608            push word [bp+0x8]
0000156F  FF7606            push word [bp+0x6]
00001572  9AE5420000        call 0x0:0x42e5
00001577  59                pop cx
00001578  59                pop cx
00001579  BA0600            mov dx,0x6
0000157C  F7EA              imul dx
0000157E  8B16B615          mov dx,[0x15b6]
00001582  03D0              add dx,ax
00001584  8916B615          mov [0x15b6],dx
00001588  B80100            mov ax,0x1
0000158B  50                push ax
0000158C  B8FFFF            mov ax,0xffff
0000158F  50                push ax
00001590  B80B00            mov ax,0xb
00001593  50                push ax
00001594  B80900            mov ax,0x9
00001597  50                push ax
00001598  FF760E            push word [bp+0xe]
0000159B  FF760C            push word [bp+0xc]
0000159E  FF760A            push word [bp+0xa]
000015A1  B84500            mov ax,0x45
000015A4  50                push ax
000015A5  FF36B615          push word [0x15b6]
000015A9  9A6401690A        call 0xa69:0x164
000015AE  83C412            add sp,byte +0x12
000015B1  8B460E            mov ax,[bp+0xe]
000015B4  40                inc ax
000015B5  BA0600            mov dx,0x6
000015B8  F7EA              imul dx
000015BA  0306B615          add ax,[0x15b6]
000015BE  050B00            add ax,0xb
000015C1  A3B615            mov [0x15b6],ax
000015C4  5D                pop bp
000015C5  CB                retf

; ---- 08CF:15C6 ----
000015C6  55                push bp
000015C7  8BEC              mov bp,sp
000015C9  56                push si
000015CA  8B7606            mov si,[bp+0x6]
000015CD  B85000            mov ax,0x50
000015D0  50                push ax
000015D1  FF7608            push word [bp+0x8]
000015D4  B82C00            mov ax,0x2c
000015D7  50                push ax
000015D8  B84000            mov ax,0x40
000015DB  50                push ax
000015DC  8A84F000          mov al,[si+0xf0]
000015E0  B400              mov ah,0x0
000015E2  50                push ax
000015E3  8A84EC00          mov al,[si+0xec]
000015E7  B400              mov ah,0x0
000015E9  50                push ax
000015EA  9A3000960A        call 0xa96:0x30
000015EF  83C40C            add sp,byte +0xc
000015F2  5E                pop si
000015F3  5D                pop bp
000015F4  CB                retf

; ---- 08CF:15F5 ----
000015F5  55                push bp
000015F6  8BEC              mov bp,sp
000015F8  FF7608            push word [bp+0x8]
000015FB  FF7606            push word [bp+0x6]
000015FE  B81600            mov ax,0x16
00001601  50                push ax
00001602  B82000            mov ax,0x20
00001605  50                push ax
00001606  8B460A            mov ax,[bp+0xa]
00001609  BA1600            mov dx,0x16
0000160C  F7EA              imul dx
0000160E  055800            add ax,0x58
00001611  50                push ax
00001612  33C0              xor ax,ax
00001614  50                push ax
00001615  9A3000960A        call 0xa96:0x30
0000161A  83C40C            add sp,byte +0xc
0000161D  5D                pop bp
0000161E  CB                retf

; ---- 08CF:161F ----
0000161F  55                push bp
00001620  8BEC              mov bp,sp
00001622  9A8816CF08        call 0x8cf:0x1688
00001627  A18037            mov ax,[0x3780]
0000162A  05FBFF            add ax,0xfffb
0000162D  A39A15            mov [0x159a],ax
00001630  3D0500            cmp ax,0x5
00001633  7D27              jnl 0x165c
00001635  B80500            mov ax,0x5
00001638  50                push ax
00001639  B80800            mov ax,0x8
0000163C  50                push ax
0000163D  1E                push ds
0000163E  B83601            mov ax,0x136
00001641  50                push ax
00001642  B86700            mov ax,0x67
00001645  50                push ax
00001646  A1A015            mov ax,[0x15a0]
00001649  051600            add ax,0x16
0000164C  50                push ax
0000164D  9A2505960A        call 0xa96:0x525
00001652  83C40C            add sp,byte +0xc
00001655  C606A215F6        mov byte [0x15a2],0xf6
0000165A  EB2A              jmp short 0x1686
0000165C  B80500            mov ax,0x5
0000165F  50                push ax
00001660  B80100            mov ax,0x1
00001663  50                push ax
00001664  B80400            mov ax,0x4
00001667  50                push ax
00001668  A1A234            mov ax,[0x34a2]
0000166B  99                cwd
0000166C  52                push dx
0000166D  50                push ax
0000166E  B86700            mov ax,0x67
00001671  50                push ax
00001672  A1A015            mov ax,[0x15a0]
00001675  051500            add ax,0x15
00001678  50                push ax
00001679  9AFB05960A        call 0xa96:0x5fb
0000167E  83C40E            add sp,byte +0xe
00001681  C606A215FF        mov byte [0x15a2],0xff
00001686  5D                pop bp
00001687  CB                retf

; ---- 08CF:1688 ----
00001688  55                push bp
00001689  8BEC              mov bp,sp
0000168B  B81600            mov ax,0x16
0000168E  50                push ax
0000168F  B88000            mov ax,0x80
00001692  50                push ax
00001693  B8A800            mov ax,0xa8
00001696  50                push ax
00001697  B82000            mov ax,0x20
0000169A  50                push ax
0000169B  B81600            mov ax,0x16
0000169E  50                push ax
0000169F  B82000            mov ax,0x20
000016A2  50                push ax
000016A3  33C0              xor ax,ax
000016A5  50                push ax
000016A6  B8A000            mov ax,0xa0
000016A9  50                push ax
000016AA  9A6702960A        call 0xa96:0x267
000016AF  83C410            add sp,byte +0x10
000016B2  5D                pop bp
000016B3  CB                retf

; ---- 08CF:16B4 ----
000016B4  55                push bp
000016B5  8BEC              mov bp,sp
000016B7  B83500            mov ax,0x35
000016BA  50                push ax
000016BB  A0AA37            mov al,[0x37aa]
000016BE  98                cbw
000016BF  B106              mov cl,0x6
000016C1  D3E0              shl ax,cl
000016C3  50                push ax
000016C4  8B4606            mov ax,[bp+0x6]
000016C7  BA0B00            mov dx,0xb
000016CA  F7EA              imul dx
000016CC  5A                pop dx
000016CD  03D0              add dx,ax
000016CF  83C21A            add dx,byte +0x1a
000016D2  52                push dx
000016D3  B80A00            mov ax,0xa
000016D6  50                push ax
000016D7  B80A00            mov ax,0xa
000016DA  50                push ax
000016DB  B8CF00            mov ax,0xcf
000016DE  50                push ax
000016DF  8B4608            mov ax,[bp+0x8]
000016E2  BA0A00            mov dx,0xa
000016E5  F7EA              imul dx
000016E7  05F000            add ax,0xf0
000016EA  50                push ax
000016EB  9A3000960A        call 0xa96:0x30
000016F0  83C40C            add sp,byte +0xc
000016F3  5D                pop bp
000016F4  CB                retf

; ---- 08CF:16F5 ----
000016F5  55                push bp
000016F6  8BEC              mov bp,sp
000016F8  B80A00            mov ax,0xa
000016FB  50                push ax
000016FC  B82C00            mov ax,0x2c
000016FF  50                push ax
00001700  B83500            mov ax,0x35
00001703  50                push ax
00001704  A0AA37            mov al,[0x37aa]
00001707  98                cbw
00001708  B106              mov cl,0x6
0000170A  D3E0              shl ax,cl
0000170C  051A00            add ax,0x1a
0000170F  50                push ax
00001710  B81600            mov ax,0x16
00001713  50                push ax
00001714  B82000            mov ax,0x20
00001717  50                push ax
00001718  B8B000            mov ax,0xb0
0000171B  50                push ax
0000171C  B8F000            mov ax,0xf0
0000171F  50                push ax
00001720  9A6702960A        call 0xa96:0x267
00001725  83C410            add sp,byte +0x10
00001728  5D                pop bp
00001729  CB                retf

; ---- 08CF:172A ----
0000172A  55                push bp
0000172B  8BEC              mov bp,sp
0000172D  56                push si
0000172E  33F6              xor si,si
00001730  EB49              jmp short 0x177b
00001732  8BC6              mov ax,si
00001734  BB0500            mov bx,0x5
00001737  99                cwd
00001738  F7FB              idiv bx
0000173A  D1E0              shl ax,1
0000173C  D1E0              shl ax,1
0000173E  D1E0              shl ax,1
00001740  051400            add ax,0x14
00001743  50                push ax
00001744  A0AA37            mov al,[0x37aa]
00001747  98                cbw
00001748  B106              mov cl,0x6
0000174A  D3E0              shl ax,cl
0000174C  50                push ax
0000174D  8BC6              mov ax,si
0000174F  BB0500            mov bx,0x5
00001752  99                cwd
00001753  F7FB              idiv bx
00001755  D1E2              shl dx,1
00001757  D1E2              shl dx,1
00001759  D1E2              shl dx,1
0000175B  58                pop ax
0000175C  03C2              add ax,dx
0000175E  052600            add ax,0x26
00001761  50                push ax
00001762  B80700            mov ax,0x7
00001765  50                push ax
00001766  B80700            mov ax,0x7
00001769  50                push ax
0000176A  B8C600            mov ax,0xc6
0000176D  50                push ax
0000176E  B80901            mov ax,0x109
00001771  50                push ax
00001772  9A3000960A        call 0xa96:0x30
00001777  83C40C            add sp,byte +0xc
0000177A  46                inc si
0000177B  A0AA37            mov al,[0x37aa]
0000177E  98                cbw
0000177F  8BD8              mov bx,ax
00001781  8A87AF37          mov al,[bx+0x37af]
00001785  98                cbw
00001786  3BC6              cmp ax,si
00001788  7FA8              jg 0x1732
0000178A  5E                pop si
0000178B  5D                pop bp
0000178C  CB                retf

; ---- 08CF:178D ----
0000178D  55                push bp
0000178E  8BEC              mov bp,sp
00001790  A0AA37            mov al,[0x37aa]
00001793  98                cbw
00001794  8A5606            mov dl,[bp+0x6]
00001797  8BD8              mov bx,ax
00001799  0097AF37          add [bx+0x37af],dl
0000179D  807E0601          cmp byte [bp+0x6],0x1
000017A1  755E              jnz 0x1801
000017A3  A0AA37            mov al,[0x37aa]
000017A6  98                cbw
000017A7  8BD8              mov bx,ax
000017A9  8A87AF37          mov al,[bx+0x37af]
000017AD  98                cbw
000017AE  48                dec ax
000017AF  BB0500            mov bx,0x5
000017B2  99                cwd
000017B3  F7FB              idiv bx
000017B5  D1E0              shl ax,1
000017B7  D1E0              shl ax,1
000017B9  D1E0              shl ax,1
000017BB  051400            add ax,0x14
000017BE  50                push ax
000017BF  A0AA37            mov al,[0x37aa]
000017C2  98                cbw
000017C3  B106              mov cl,0x6
000017C5  D3E0              shl ax,cl
000017C7  50                push ax
000017C8  A0AA37            mov al,[0x37aa]
000017CB  98                cbw
000017CC  8BD8              mov bx,ax
000017CE  8A87AF37          mov al,[bx+0x37af]
000017D2  98                cbw
000017D3  48                dec ax
000017D4  BB0500            mov bx,0x5
000017D7  99                cwd
000017D8  F7FB              idiv bx
000017DA  D1E2              shl dx,1
000017DC  D1E2              shl dx,1
000017DE  D1E2              shl dx,1
000017E0  58                pop ax
000017E1  03C2              add ax,dx
000017E3  052600            add ax,0x26
000017E6  50                push ax
000017E7  B80700            mov ax,0x7
000017EA  50                push ax
000017EB  B80700            mov ax,0x7
000017EE  50                push ax
000017EF  B8C600            mov ax,0xc6
000017F2  50                push ax
000017F3  B80901            mov ax,0x109
000017F6  50                push ax
000017F7  9A3000960A        call 0xa96:0x30
000017FC  83C40C            add sp,byte +0xc
000017FF  EB56              jmp short 0x1857
00001801  B80500            mov ax,0x5
00001804  50                push ax
00001805  B80700            mov ax,0x7
00001808  50                push ax
00001809  B80700            mov ax,0x7
0000180C  50                push ax
0000180D  A0AA37            mov al,[0x37aa]
00001810  98                cbw
00001811  8BD8              mov bx,ax
00001813  8A87AF37          mov al,[bx+0x37af]
00001817  98                cbw
00001818  BB0500            mov bx,0x5
0000181B  99                cwd
0000181C  F7FB              idiv bx
0000181E  D1E0              shl ax,1
00001820  D1E0              shl ax,1
00001822  D1E0              shl ax,1
00001824  051400            add ax,0x14
00001827  50                push ax
00001828  A0AA37            mov al,[0x37aa]
0000182B  98                cbw
0000182C  B106              mov cl,0x6
0000182E  D3E0              shl ax,cl
00001830  50                push ax
00001831  A0AA37            mov al,[0x37aa]
00001834  98                cbw
00001835  8BD8              mov bx,ax
00001837  8A87AF37          mov al,[bx+0x37af]
0000183B  98                cbw
0000183C  BB0500            mov bx,0x5
0000183F  99                cwd
00001840  F7FB              idiv bx
00001842  D1E2              shl dx,1
00001844  D1E2              shl dx,1
00001846  D1E2              shl dx,1
00001848  58                pop ax
00001849  03C2              add ax,dx
0000184B  052600            add ax,0x26
0000184E  50                push ax
0000184F  9AD704960A        call 0xa96:0x4d7
00001854  83C40A            add sp,byte +0xa
00001857  5D                pop bp
00001858  CB                retf

; ---- 08CF:1859 ----
00001859  55                push bp
0000185A  8BEC              mov bp,sp
0000185C  B80500            mov ax,0x5
0000185F  50                push ax
00001860  B80D00            mov ax,0xd
00001863  50                push ax
00001864  1E                push ds
00001865  B83C01            mov ax,0x13c
00001868  50                push ax
00001869  B82A00            mov ax,0x2a
0000186C  50                push ax
0000186D  A0AA37            mov al,[0x37aa]
00001870  98                cbw
00001871  B106              mov cl,0x6
00001873  D3E0              shl ax,cl
00001875  052600            add ax,0x26
00001878  50                push ax
00001879  9A2505960A        call 0xa96:0x525
0000187E  83C40C            add sp,byte +0xc
00001881  B80500            mov ax,0x5
00001884  50                push ax
00001885  33C0              xor ax,ax
00001887  50                push ax
00001888  B80400            mov ax,0x4
0000188B  50                push ax
0000188C  A0AA37            mov al,[0x37aa]
0000188F  98                cbw
00001890  D1E0              shl ax,1
00001892  8BD8              mov bx,ax
00001894  8B870E30          mov ax,[bx+0x300e]
00001898  99                cwd
00001899  52                push dx
0000189A  50                push ax
0000189B  B82A00            mov ax,0x2a
0000189E  50                push ax
0000189F  A0AA37            mov al,[0x37aa]
000018A2  98                cbw
000018A3  B106              mov cl,0x6
000018A5  D3E0              shl ax,cl
000018A7  053400            add ax,0x34
000018AA  50                push ax
000018AB  9A7A05960A        call 0xa96:0x57a
000018B0  83C40E            add sp,byte +0xe
000018B3  5D                pop bp
000018B4  CB                retf

; ---- 08CF:18B5 ----
000018B5  55                push bp
000018B6  8BEC              mov bp,sp
000018B8  56                push si
000018B9  A18037            mov ax,[0x3780]
000018BC  8BF0              mov si,ax
000018BE  3D1E00            cmp ax,0x1e
000018C1  7E03              jng 0x18c6
000018C3  BE1E00            mov si,0x1e
000018C6  B80600            mov ax,0x6
000018C9  50                push ax
000018CA  B81001            mov ax,0x110
000018CD  50                push ax
000018CE  B82000            mov ax,0x20
000018D1  50                push ax
000018D2  B82000            mov ax,0x20
000018D5  50                push ax
000018D6  B8B000            mov ax,0xb0
000018D9  50                push ax
000018DA  B81001            mov ax,0x110
000018DD  50                push ax
000018DE  9A3000960A        call 0xa96:0x30
000018E3  83C40C            add sp,byte +0xc
000018E6  B80F00            mov ax,0xf
000018E9  50                push ax
000018EA  8A841301          mov al,[si+0x113]
000018EE  98                cbw
000018EF  050700            add ax,0x7
000018F2  50                push ax
000018F3  8A84F400          mov al,[si+0xf4]
000018F7  98                cbw
000018F8  050F01            add ax,0x10f
000018FB  50                push ax
000018FC  B81600            mov ax,0x16
000018FF  50                push ax
00001900  B81F01            mov ax,0x11f
00001903  50                push ax
00001904  9A3304960A        call 0xa96:0x433
00001909  83C40A            add sp,byte +0xa
0000190C  B80500            mov ax,0x5
0000190F  50                push ax
00001910  8A841301          mov al,[si+0x113]
00001914  98                cbw
00001915  050600            add ax,0x6
00001918  50                push ax
00001919  8A84F400          mov al,[si+0xf4]
0000191D  98                cbw
0000191E  051001            add ax,0x110
00001921  50                push ax
00001922  B81500            mov ax,0x15
00001925  50                push ax
00001926  B82001            mov ax,0x120
00001929  50                push ax
0000192A  9A3304960A        call 0xa96:0x433
0000192F  83C40A            add sp,byte +0xa
00001932  5E                pop si
00001933  5D                pop bp
00001934  CB                retf

; ---- 08CF:1935 ----
00001935  55                push bp
00001936  8BEC              mov bp,sp
00001938  0E                push cs
00001939  E879FF            call 0x18b5
0000193C  5D                pop bp
0000193D  CB                retf

; ---- 08CF:193E ----
0000193E  55                push bp
0000193F  8BEC              mov bp,sp
00001941  B80500            mov ax,0x5
00001944  50                push ax
00001945  A0AA37            mov al,[0x37aa]
00001948  98                cbw
00001949  8BD8              mov bx,ax
0000194B  8A873201          mov al,[bx+0x132]
0000194F  98                cbw
00001950  50                push ax
00001951  B80500            mov ax,0x5
00001954  50                push ax
00001955  A0AA37            mov al,[0x37aa]
00001958  98                cbw
00001959  D1E0              shl ax,1
0000195B  D1E0              shl ax,1
0000195D  8BD8              mov bx,ax
0000195F  FFB7D43C          push word [bx+0x3cd4]
00001963  FFB7D23C          push word [bx+0x3cd2]
00001967  B81100            mov ax,0x11
0000196A  50                push ax
0000196B  A0AA37            mov al,[0x37aa]
0000196E  98                cbw
0000196F  B106              mov cl,0x6
00001971  D3E0              shl ax,cl
00001973  052600            add ax,0x26
00001976  50                push ax
00001977  9AFB05960A        call 0xa96:0x5fb
0000197C  83C40E            add sp,byte +0xe
0000197F  5D                pop bp
00001980  CB                retf

; ---- 08CF:1981 ----
00001981  55                push bp
00001982  8BEC              mov bp,sp
00001984  B80A00            mov ax,0xa
00001987  50                push ax
00001988  B80500            mov ax,0x5
0000198B  50                push ax
0000198C  B80300            mov ax,0x3
0000198F  50                push ax
00001990  A1984B            mov ax,[0x4b98]
00001993  99                cwd
00001994  52                push dx
00001995  50                push ax
00001996  B87700            mov ax,0x77
00001999  50                push ax
0000199A  A1B015            mov ax,[0x15b0]
0000199D  052B00            add ax,0x2b
