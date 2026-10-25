; segment 1B03  file offset 0x1E630  size 0x1650
00000000  00EB              add bl,ch
00000002  005F5E            add [bx+0x5e],bl
00000005  8BE5              mov sp,bp
00000007  5D                pop bp
00000008  CB                retf

; ---- 1B03:0009 ----
00000009  55                push bp
0000000A  8BEC              mov bp,sp
0000000C  9A7900031B        call 0x1b03:0x79
00000011  803EEA2E00        cmp byte [0x2eea],0x0
00000016  7517              jnz 0x2f
00000018  803EEE2E00        cmp byte [0x2eee],0x0
0000001D  7510              jnz 0x2f
0000001F  803EF22E00        cmp byte [0x2ef2],0x0
00000024  7509              jnz 0x2f
00000026  803EF62E00        cmp byte [0x2ef6],0x0
0000002B  7502              jnz 0x2f
0000002D  EB48              jmp short 0x77
0000002F  FF36D008          push word [0x8d0]
00000033  9A02006C18        call 0x186c:0x2
00000038  59                pop cx
00000039  9A8B14031B        call 0x1b03:0x148b
0000003E  B80200            mov ax,0x2
00000041  50                push ax
00000042  9A7D03FD21        call 0x21fd:0x37d
00000047  59                pop cx
00000048  9A7811031B        call 0x1b03:0x1178
0000004D  1E                push ds
0000004E  B8D80C            mov ax,0xcd8
00000051  50                push ax
00000052  9A6C02FD21        call 0x21fd:0x26c
00000057  59                pop cx
00000058  59                pop cx
00000059  9A2903FD21        call 0x21fd:0x329
0000005E  9AD400031B        call 0x1b03:0xd4
00000063  9A0507031B        call 0x1b03:0x705
00000068  9AAC0C031B        call 0x1b03:0xcac
0000006D  9A110E031B        call 0x1b03:0xe11
00000072  9AA90E031B        call 0x1b03:0xea9
00000077  5D                pop bp
00000078  CB                retf

; ---- 1B03:0079 ----
00000079  55                push bp
0000007A  8BEC              mov bp,sp
0000007C  56                push si
0000007D  57                push di
0000007E  33FF              xor di,di
00000080  EB49              jmp short 0xcb
00000082  B80B00            mov ax,0xb
00000085  50                push ax
00000086  9ABA01FD21        call 0x21fd:0x1ba
0000008B  59                pop cx
0000008C  8BF0              mov si,ax
0000008E  8BDE              mov bx,si
00000090  D1E3              shl bx,1
00000092  83BF8A34FF        cmp word [bx+0x348a],byte -0x1
00000097  7527              jnz 0xc0
00000099  B80200            mov ax,0x2
0000009C  50                push ax
0000009D  9ABA01FD21        call 0x21fd:0x1ba
000000A2  59                pop cx
000000A3  0BC0              or ax,ax
000000A5  7405              jz 0xac
000000A7  B80A00            mov ax,0xa
000000AA  EB0A              jmp short 0xb6
000000AC  B80300            mov ax,0x3
000000AF  50                push ax
000000B0  9ABA01FD21        call 0x21fd:0x1ba
000000B5  59                pop cx
000000B6  8BDE              mov bx,si
000000B8  D1E3              shl bx,1
000000BA  89878A34          mov [bx+0x348a],ax
000000BE  EB0A              jmp short 0xca
000000C0  8BDE              mov bx,si
000000C2  D1E3              shl bx,1
000000C4  C7878A34FFFF      mov word [bx+0x348a],0xffff
000000CA  47                inc di
000000CB  83FF03            cmp di,byte +0x3
000000CE  7EB2              jng 0x82
000000D0  5F                pop di
000000D1  5E                pop si
000000D2  5D                pop bp
000000D3  CB                retf

; ---- 1B03:00D4 ----
000000D4  55                push bp
000000D5  8BEC              mov bp,sp
000000D7  9A0101031B        call 0x1b03:0x101
000000DC  9A5301031B        call 0x1b03:0x153
000000E1  9AC301031B        call 0x1b03:0x1c3
000000E6  9A0102031B        call 0x1b03:0x201
000000EB  9A1B00960A        call 0xa96:0x1b
000000F0  9AC002031B        call 0x1b03:0x2c0
000000F5  9AD715031B        call 0x1b03:0x15d7
000000FA  9AEE13031B        call 0x1b03:0x13ee
000000FF  5D                pop bp
00000100  CB                retf

; ---- 1B03:0101 ----
00000101  55                push bp
00000102  8BEC              mov bp,sp
00000104  B80600            mov ax,0x6
00000107  50                push ax
00000108  9ABA01FD21        call 0x21fd:0x1ba
0000010D  59                pop cx
0000010E  0BC0              or ax,ax
00000110  7402              jz 0x114
00000112  EB3D              jmp short 0x151
00000114  B80500            mov ax,0x5
00000117  50                push ax
00000118  B80700            mov ax,0x7
0000011B  50                push ax
0000011C  B80300            mov ax,0x3
0000011F  50                push ax
00000120  1E                push ds
00000121  B8D03A            mov ax,0x3ad0
00000124  50                push ax
00000125  B85600            mov ax,0x56
00000128  50                push ax
00000129  B8B400            mov ax,0xb4
0000012C  50                push ax
0000012D  B87100            mov ax,0x71
00000130  50                push ax
00000131  9A8802690A        call 0xa69:0x288
00000136  83C410            add sp,byte +0x10
00000139  9A3D02310E        call 0xe31:0x23d
0000013E  33C0              xor ax,ax
00000140  BA3200            mov dx,0x32
00000143  50                push ax
00000144  52                push dx
00000145  9A8501FD21        call 0x21fd:0x185
0000014A  59                pop cx
0000014B  59                pop cx
0000014C  9AEE13031B        call 0x1b03:0x13ee
00000151  5D                pop bp
00000152  CB                retf

; ---- 1B03:0153 ----
00000153  55                push bp
00000154  8BEC              mov bp,sp
00000156  56                push si
00000157  57                push di
00000158  9A1B00960A        call 0xa96:0x1b
0000015D  33F6              xor si,si
0000015F  EB54              jmp short 0x1b5
00000161  8BDE              mov bx,si
00000163  D1E3              shl bx,1
00000165  83BF8A34FF        cmp word [bx+0x348a],byte -0x1
0000016A  7502              jnz 0x16e
0000016C  EB46              jmp short 0x1b4
0000016E  8BDE              mov bx,si
00000170  D1E3              shl bx,1
00000172  83BF8A340A        cmp word [bx+0x348a],byte +0xa
00000177  7505              jnz 0x17e
00000179  BF0500            mov di,0x5
0000017C  EB0E              jmp short 0x18c
0000017E  8BDE              mov bx,si
00000180  D1E3              shl bx,1
00000182  8B9F8A34          mov bx,[bx+0x348a]
00000186  D1E3              shl bx,1
00000188  8BBF000D          mov di,[bx+0xd00]
0000018C  B8FFFF            mov ax,0xffff
0000018F  50                push ax
00000190  57                push di
00000191  1E                push ds
00000192  B83E0E            mov ax,0xe3e
00000195  50                push ax
00000196  8A849C0C          mov al,[si+0xc9c]
0000019A  B400              mov ah,0x0
0000019C  050900            add ax,0x9
0000019F  50                push ax
000001A0  8BDE              mov bx,si
000001A2  D1E3              shl bx,1
000001A4  8B87840C          mov ax,[bx+0xc84]
000001A8  051200            add ax,0x12
000001AB  50                push ax
000001AC  9A2505960A        call 0xa96:0x525
000001B1  83C40C            add sp,byte +0xc
000001B4  46                inc si
000001B5  83FE0B            cmp si,byte +0xb
000001B8  7EA7              jng 0x161
000001BA  9A0D06FD21        call 0x21fd:0x60d
000001BF  5F                pop di
000001C0  5E                pop si
000001C1  5D                pop bp
000001C2  CB                retf

; ---- 1B03:01C3 ----
000001C3  55                push bp
000001C4  8BEC              mov bp,sp
000001C6  833E000901        cmp word [0x900],byte +0x1
000001CB  7C2D              jl 0x1fa
000001CD  B80200            mov ax,0x2
000001D0  50                push ax
000001D1  9ABA01FD21        call 0x21fd:0x1ba
000001D6  59                pop cx
000001D7  0BC0              or ax,ax
000001D9  751F              jnz 0x1fa
000001DB  A10009            mov ax,[0x900]
000001DE  48                dec ax
000001DF  50                push ax
000001E0  9ABA01FD21        call 0x21fd:0x1ba
000001E5  59                pop cx
000001E6  A26E54            mov [0x546e],al
000001E9  9A1B00960A        call 0xa96:0x1b
000001EE  9A5712031B        call 0x1b03:0x1257
000001F3  9A0D06FD21        call 0x21fd:0x60d
000001F8  EB05              jmp short 0x1ff
000001FA  C6066E54FF        mov byte [0x546e],0xff
000001FF  5D                pop bp
00000200  CB                retf

; ---- 1B03:0201 ----
00000201  55                push bp
00000202  8BEC              mov bp,sp
00000204  56                push si
00000205  57                push di
00000206  33F6              xor si,si
00000208  EB06              jmp short 0x210
0000020A  C684285400        mov byte [si+0x5428],0x0
0000020F  46                inc si
00000210  83FE0B            cmp si,byte +0xb
00000213  7EF5              jng 0x20a
00000215  9A1B00960A        call 0xa96:0x1b
0000021A  C606AA3700        mov byte [0x37aa],0x0
0000021F  E98B00            jmp 0x2ad
00000222  A0AA37            mov al,[0x37aa]
00000225  98                cbw
00000226  D1E0              shl ax,1
00000228  8BD8              mov bx,ax
0000022A  8BBF0E45          mov di,[bx+0x450e]
0000022E  A0AA37            mov al,[0x37aa]
00000231  98                cbw
00000232  BA0300            mov dx,0x3
00000235  F7EA              imul dx
00000237  99                cwd
00000238  50                push ax
00000239  A0AA37            mov al,[0x37aa]
0000023C  98                cbw
0000023D  D1E0              shl ax,1
0000023F  D1E0              shl ax,1
00000241  8BD8              mov bx,ax
00000243  58                pop ax
00000244  89878637          mov [bx+0x3786],ax
00000248  89978837          mov [bx+0x3788],dx
0000024C  83FFFF            cmp di,byte -0x1
0000024F  752C              jnz 0x27d
00000251  A0AA37            mov al,[0x37aa]
00000254  98                cbw
00000255  D1E0              shl ax,1
00000257  50                push ax
00000258  A0AA37            mov al,[0x37aa]
0000025B  98                cbw
0000025C  D1E0              shl ax,1
0000025E  8BD8              mov bx,ax
00000260  58                pop ax
00000261  89878848          mov [bx+0x4888],ax
00000265  9A1716031B        call 0x1b03:0x1617
0000026A  A0AA37            mov al,[0x37aa]
0000026D  98                cbw
0000026E  BA1A00            mov dx,0x1a
00000271  F7EA              imul dx
00000273  8BD8              mov bx,ax
00000275  C78724330000      mov word [bx+0x3324],0x0
0000027B  EB27              jmp short 0x2a4
0000027D  A0AA37            mov al,[0x37aa]
00000280  98                cbw
00000281  D1E0              shl ax,1
00000283  8BD8              mov bx,ax
00000285  89BF8848          mov [bx+0x4888],di
00000289  9A1716031B        call 0x1b03:0x1617
0000028E  A0AA37            mov al,[0x37aa]
00000291  98                cbw
00000292  BA1A00            mov dx,0x1a
00000295  F7EA              imul dx
00000297  8BD8              mov bx,ax
00000299  C78724330100      mov word [bx+0x3324],0x1
0000029F  C685285401        mov byte [di+0x5428],0x1
000002A4  9A4805031B        call 0x1b03:0x548
000002A9  FE06AA37          inc byte [0x37aa]
000002AD  803EAA3704        cmp byte [0x37aa],0x4
000002B2  7D03              jnl 0x2b7
000002B4  E96BFF            jmp 0x222
000002B7  9A0D06FD21        call 0x21fd:0x60d
000002BC  5F                pop di
000002BD  5E                pop si
000002BE  5D                pop bp
000002BF  CB                retf

; ---- 1B03:02C0 ----
000002C0  55                push bp
000002C1  8BEC              mov bp,sp
000002C3  83EC06            sub sp,byte +0x6
000002C6  C646FF04          mov byte [bp-0x1],0x4
000002CA  C606AA3700        mov byte [0x37aa],0x0
000002CF  EB13              jmp short 0x2e4
000002D1  A0AA37            mov al,[0x37aa]
000002D4  98                cbw
000002D5  8D56FA            lea dx,[bp-0x6]
000002D8  03C2              add ax,dx
000002DA  8BD8              mov bx,ax
000002DC  36C60701          mov byte [ss:bx],0x1
000002E0  FE06AA37          inc byte [0x37aa]
000002E4  803EAA3704        cmp byte [0x37aa],0x4
000002E9  7CE6              jl 0x2d1
000002EB  E91301            jmp 0x401
000002EE  9A1F05FD21        call 0x21fd:0x51f
000002F3  FF362E08          push word [0x82e]
000002F7  9A1C07CD23        call 0x23cd:0x71c
000002FC  59                pop cx
000002FD  9AA806960A        call 0xa96:0x6a8
00000302  9A1B00960A        call 0xa96:0x1b
00000307  B80A00            mov ax,0xa
0000030A  50                push ax
0000030B  9AD712031B        call 0x1b03:0x12d7
00000310  59                pop cx
00000311  9A4502FD21        call 0x21fd:0x245
00000316  9AE705031B        call 0x1b03:0x5e7
0000031B  9A0E009E0E        call 0xe9e:0xe
00000320  C606AA3700        mov byte [0x37aa],0x0
00000325  E9C000            jmp 0x3e8
00000328  A0AA37            mov al,[0x37aa]
0000032B  98                cbw
0000032C  8D56FA            lea dx,[bp-0x6]
0000032F  03C2              add ax,dx
00000331  8BD8              mov bx,ax
00000333  36803F00          cmp byte [ss:bx],0x0
00000337  7503              jnz 0x33c
00000339  E9A800            jmp 0x3e4
0000033C  A0AA37            mov al,[0x37aa]
0000033F  98                cbw
00000340  8BD8              mov bx,ax
00000342  80BFF04300        cmp byte [bx+0x43f0],0x0
00000347  740D              jz 0x356
00000349  B8FFFF            mov ax,0xffff
0000034C  50                push ax
0000034D  9A1304031B        call 0x1b03:0x413
00000352  59                pop cx
00000353  E98E00            jmp 0x3e4
00000356  A0AA37            mov al,[0x37aa]
00000359  98                cbw
0000035A  8BD8              mov bx,ax
0000035C  80BFBE4400        cmp byte [bx+0x44be],0x0
00000361  740C              jz 0x36f
00000363  B80100            mov ax,0x1
00000366  50                push ax
00000367  9A1304031B        call 0x1b03:0x413
0000036C  59                pop cx
0000036D  EB75              jmp short 0x3e4
0000036F  A0AA37            mov al,[0x37aa]
00000372  98                cbw
00000373  8BD8              mov bx,ax
00000375  80BF944800        cmp byte [bx+0x4894],0x0
0000037A  7468              jz 0x3e4
0000037C  A0AA37            mov al,[0x37aa]
0000037F  98                cbw
00000380  D1E0              shl ax,1
00000382  8BD8              mov bx,ax
00000384  83BF0E45FF        cmp word [bx+0x450e],byte -0x1
00000389  7507              jnz 0x392
0000038B  9AD404031B        call 0x1b03:0x4d4
00000390  EB52              jmp short 0x3e4
00000392  A0AA37            mov al,[0x37aa]
00000395  98                cbw
00000396  D1E0              shl ax,1
00000398  8BD8              mov bx,ax
0000039A  8B9F0E45          mov bx,[bx+0x450e]
0000039E  C687285402        mov byte [bx+0x5428],0x2
000003A3  FE4EFF            dec byte [bp-0x1]
000003A6  A0AA37            mov al,[0x37aa]
000003A9  98                cbw
000003AA  8D56FA            lea dx,[bp-0x6]
000003AD  03C2              add ax,dx
000003AF  8BD8              mov bx,ax
000003B1  36C60700          mov byte [ss:bx],0x0
000003B5  B8FFFF            mov ax,0xffff
000003B8  50                push ax
000003B9  A0AA37            mov al,[0x37aa]
000003BC  98                cbw
000003BD  D1E0              shl ax,1
000003BF  8BD8              mov bx,ax
000003C1  FFB7F80C          push word [bx+0xcf8]
000003C5  1E                push ds
000003C6  B8400E            mov ax,0xe40
000003C9  50                push ax
000003CA  A0AA37            mov al,[0x37aa]
000003CD  98                cbw
000003CE  D1E0              shl ax,1
000003D0  D1E0              shl ax,1
000003D2  D1E0              shl ax,1
000003D4  050A00            add ax,0xa
000003D7  50                push ax
000003D8  B80200            mov ax,0x2
000003DB  50                push ax
000003DC  9A2505960A        call 0xa96:0x525
000003E1  83C40C            add sp,byte +0xc
000003E4  FE06AA37          inc byte [0x37aa]
000003E8  803EAA3704        cmp byte [0x37aa],0x4
000003ED  7D03              jnl 0x3f2
000003EF  E936FF            jmp 0x328
000003F2  9AA806960A        call 0xa96:0x6a8
000003F7  FF362E08          push word [0x82e]
000003FB  9AE906CD23        call 0x23cd:0x6e9
00000400  59                pop cx
00000401  807EFF00          cmp byte [bp-0x1],0x0
00000405  7403              jz 0x40a
00000407  E9E4FE            jmp 0x2ee
0000040A  9A6506FD21        call 0x21fd:0x665
0000040F  8BE5              mov sp,bp
00000411  5D                pop bp
00000412  CB                retf

; ---- 1B03:0413 ----
00000413  55                push bp
00000414  8BEC              mov bp,sp
00000416  56                push si
00000417  A0AA37            mov al,[0x37aa]
0000041A  98                cbw
0000041B  D1E0              shl ax,1
0000041D  8BD8              mov bx,ax
0000041F  83BF0E45FF        cmp word [bx+0x450e],byte -0x1
00000424  7430              jz 0x456
00000426  A0AA37            mov al,[0x37aa]
00000429  98                cbw
0000042A  D1E0              shl ax,1
0000042C  8BD8              mov bx,ax
0000042E  8B9F0E45          mov bx,[bx+0x450e]
00000432  C687285400        mov byte [bx+0x5428],0x0
00000437  A0AA37            mov al,[0x37aa]
0000043A  98                cbw
0000043B  D1E0              shl ax,1
0000043D  8BD8              mov bx,ax
0000043F  C7870E45FFFF      mov word [bx+0x450e],0xffff
00000445  A0AA37            mov al,[0x37aa]
00000448  98                cbw
00000449  BA1A00            mov dx,0x1a
0000044C  F7EA              imul dx
0000044E  8BD8              mov bx,ax
00000450  C78724330000      mov word [bx+0x3324],0x0
00000456  A0AA37            mov al,[0x37aa]
00000459  98                cbw
0000045A  D1E0              shl ax,1
0000045C  8BD8              mov bx,ax
0000045E  8B878848          mov ax,[bx+0x4888]
00000462  034606            add ax,[bp+0x6]
00000465  8BF0              mov si,ax
00000467  83FE0B            cmp si,byte +0xb
0000046A  7E04              jng 0x470
0000046C  33F6              xor si,si
0000046E  EB07              jmp short 0x477
00000470  0BF6              or si,si
00000472  7D03              jnl 0x477
00000474  BE0B00            mov si,0xb
00000477  A0AA37            mov al,[0x37aa]
0000047A  98                cbw
0000047B  D1E0              shl ax,1
0000047D  8BD8              mov bx,ax
0000047F  89B78848          mov [bx+0x4888],si
00000483  9A1716031B        call 0x1b03:0x1617
00000488  B81000            mov ax,0x10
0000048B  50                push ax
0000048C  33C0              xor ax,ax
0000048E  50                push ax
0000048F  9A5400A721        call 0x21a7:0x54
00000494  59                pop cx
00000495  59                pop cx
00000496  9A4805031B        call 0x1b03:0x548
0000049B  A0AA37            mov al,[0x37aa]
0000049E  98                cbw
0000049F  8BD8              mov bx,ax
000004A1  80BF400801        cmp byte [bx+0x840],0x1
000004A6  7511              jnz 0x4b9
000004A8  A18E2E            mov ax,[0x2e8e]
000004AB  BA0A00            mov dx,0xa
000004AE  F7EA              imul dx
000004B0  50                push ax
000004B1  B82B00            mov ax,0x2b
000004B4  5A                pop dx
000004B5  2BC2              sub ax,dx
000004B7  EB03              jmp short 0x4bc
000004B9  B81400            mov ax,0x14
000004BC  99                cwd
000004BD  50                push ax
000004BE  A0AA37            mov al,[0x37aa]
000004C1  98                cbw
000004C2  D1E0              shl ax,1
000004C4  D1E0              shl ax,1
000004C6  8BD8              mov bx,ax
000004C8  58                pop ax
000004C9  89878637          mov [bx+0x3786],ax
000004CD  89978837          mov [bx+0x3788],dx
000004D1  5E                pop si
000004D2  5D                pop bp
000004D3  CB                retf

; ---- 1B03:04D4 ----
000004D4  55                push bp
000004D5  8BEC              mov bp,sp
000004D7  56                push si
000004D8  A0AA37            mov al,[0x37aa]
000004DB  98                cbw
000004DC  D1E0              shl ax,1
000004DE  8BD8              mov bx,ax
000004E0  8BB78848          mov si,[bx+0x4888]
000004E4  80BC285400        cmp byte [si+0x5428],0x0
000004E9  7402              jz 0x4ed
000004EB  EB58              jmp short 0x545
000004ED  A0AA37            mov al,[0x37aa]
000004F0  98                cbw
000004F1  D1E0              shl ax,1
000004F3  8BD8              mov bx,ax
000004F5  89B70E45          mov [bx+0x450e],si
000004F9  C684285401        mov byte [si+0x5428],0x1
000004FE  A0AA37            mov al,[0x37aa]
00000501  98                cbw
00000502  BA1A00            mov dx,0x1a
00000505  F7EA              imul dx
00000507  8BD8              mov bx,ax
00000509  C78724330100      mov word [bx+0x3324],0x1
0000050F  B81100            mov ax,0x11
00000512  50                push ax
00000513  33C0              xor ax,ax
00000515  50                push ax
00000516  9A5400A721        call 0x21a7:0x54
0000051B  59                pop cx
0000051C  59                pop cx
0000051D  9A4805031B        call 0x1b03:0x548
00000522  A0AA37            mov al,[0x37aa]
00000525  98                cbw
00000526  8BD8              mov bx,ax
00000528  80BF400800        cmp byte [bx+0x840],0x0
0000052D  7516              jnz 0x545
0000052F  A0AA37            mov al,[0x37aa]
00000532  98                cbw
00000533  D1E0              shl ax,1
00000535  D1E0              shl ax,1
00000537  8BD8              mov bx,ax
00000539  C7872E301027      mov word [bx+0x302e],0x2710
0000053F  C78730300000      mov word [bx+0x3030],0x0
00000545  5E                pop si
00000546  5D                pop bp
00000547  CB                retf

; ---- 1B03:0548 ----
00000548  55                push bp
00000549  8BEC              mov bp,sp
0000054B  56                push si
0000054C  A0AA37            mov al,[0x37aa]
0000054F  98                cbw
00000550  D1E0              shl ax,1
00000552  8BD8              mov bx,ax
00000554  8BB78848          mov si,[bx+0x4888]
00000558  9A1D14031B        call 0x1b03:0x141d
0000055D  33C0              xor ax,ax
0000055F  50                push ax
00000560  B8FFFF            mov ax,0xffff
00000563  50                push ax
00000564  33C0              xor ax,ax
00000566  50                push ax
00000567  A0AA37            mov al,[0x37aa]
0000056A  98                cbw
0000056B  D1E0              shl ax,1
0000056D  8BD8              mov bx,ax
0000056F  FFB7000D          push word [bx+0xd00]
00000573  8BDE              mov bx,si
00000575  D1E3              shl bx,1
00000577  D1E3              shl bx,1
00000579  FFB7AA0C          push word [bx+0xcaa]
0000057D  FFB7A80C          push word [bx+0xca8]
00000581  A0AA37            mov al,[0x37aa]
00000584  98                cbw
00000585  BA0600            mov dx,0x6
00000588  F7EA              imul dx
0000058A  05B400            add ax,0xb4
0000058D  50                push ax
0000058E  B88900            mov ax,0x89
00000591  50                push ax
00000592  9A4D00690A        call 0xa69:0x4d
00000597  83C410            add sp,byte +0x10
0000059A  80BC1A2F01        cmp byte [si+0x2f1a],0x1
0000059F  7543              jnz 0x5e4
000005A1  33C0              xor ax,ax
000005A3  50                push ax
000005A4  B8FFFF            mov ax,0xffff
000005A7  50                push ax
000005A8  33C0              xor ax,ax
000005AA  50                push ax
000005AB  A0AA37            mov al,[0x37aa]
000005AE  98                cbw
000005AF  D1E0              shl ax,1
000005B1  8BD8              mov bx,ax
000005B3  FFB7000D          push word [bx+0xd00]
000005B7  8BDE              mov bx,si
000005B9  D1E3              shl bx,1
000005BB  8B9FF444          mov bx,[bx+0x44f4]
000005BF  D1E3              shl bx,1
000005C1  D1E3              shl bx,1
000005C3  FFB70A0D          push word [bx+0xd0a]
000005C7  FFB7080D          push word [bx+0xd08]
000005CB  A0AA37            mov al,[0x37aa]
000005CE  98                cbw
000005CF  BA0600            mov dx,0x6
000005D2  F7EA              imul dx
000005D4  05B400            add ax,0xb4
000005D7  50                push ax
000005D8  B87100            mov ax,0x71
000005DB  50                push ax
000005DC  9A4D00690A        call 0xa69:0x4d
000005E1  83C410            add sp,byte +0x10
000005E4  5E                pop si
000005E5  5D                pop bp
000005E6  CB                retf

; ---- 1B03:05E7 ----
000005E7  55                push bp
000005E8  8BEC              mov bp,sp
000005EA  C606AA3700        mov byte [0x37aa],0x0
000005EF  EB38              jmp short 0x629
000005F1  A0AA37            mov al,[0x37aa]
000005F4  98                cbw
000005F5  8BD8              mov bx,ax
000005F7  80BF400801        cmp byte [bx+0x840],0x1
000005FC  7527              jnz 0x625
000005FE  9A3206031B        call 0x1b03:0x632
00000603  50                push ax
00000604  A0AA37            mov al,[0x37aa]
00000607  98                cbw
00000608  D1E0              shl ax,1
0000060A  8BD8              mov bx,ax
0000060C  58                pop ax
0000060D  3B878848          cmp ax,[bx+0x4888]
00000611  7504              jnz 0x617
00000613  B010              mov al,0x10
00000615  EB02              jmp short 0x619
00000617  B001              mov al,0x1
00000619  50                push ax
0000061A  A0AA37            mov al,[0x37aa]
0000061D  98                cbw
0000061E  8BD8              mov bx,ax
00000620  58                pop ax
00000621  8887C337          mov [bx+0x37c3],al
00000625  FE06AA37          inc byte [0x37aa]
00000629  803EAA3704        cmp byte [0x37aa],0x4
0000062E  7CC1              jl 0x5f1
00000630  5D                pop bp
00000631  CB                retf

; ---- 1B03:0632 ----
00000632  55                push bp
00000633  8BEC              mov bp,sp
00000635  83EC04            sub sp,byte +0x4
00000638  56                push si
00000639  57                push di
0000063A  C746FC18FC        mov word [bp-0x4],0xfc18
0000063F  A0AA37            mov al,[0x37aa]
00000642  98                cbw
00000643  D1E0              shl ax,1
00000645  8BD8              mov bx,ax
00000647  8BB78848          mov si,[bx+0x4888]
0000064B  33C9              xor cx,cx
0000064D  E9A200            jmp 0x6f2
00000650  83FE0B            cmp si,byte +0xb
00000653  7E02              jng 0x657
00000655  33F6              xor si,si
00000657  8BDE              mov bx,si
00000659  D1E3              shl bx,1
0000065B  8B878A34          mov ax,[bx+0x348a]
0000065F  8BD0              mov dx,ax
00000661  80BC285402        cmp byte [si+0x5428],0x2
00000666  7503              jnz 0x66b
00000668  E98500            jmp 0x6f0
0000066B  80BC285401        cmp byte [si+0x5428],0x1
00000670  7518              jnz 0x68a
00000672  A0AA37            mov al,[0x37aa]
00000675  98                cbw
00000676  3BC2              cmp ax,dx
00000678  7410              jz 0x68a
0000067A  A0AA37            mov al,[0x37aa]
0000067D  98                cbw
0000067E  D1E0              shl ax,1
00000680  8BD8              mov bx,ax
00000682  39B70E45          cmp [bx+0x450e],si
00000686  7402              jz 0x68a
00000688  EB66              jmp short 0x6f0
0000068A  80BC1A2F00        cmp byte [si+0x2f1a],0x0
0000068F  7505              jnz 0x696
00000691  B80300            mov ax,0x3
00000694  EB1D              jmp short 0x6b3
00000696  8BDE              mov bx,si
00000698  D1E3              shl bx,1
0000069A  83BFF44400        cmp word [bx+0x44f4],byte +0x0
0000069F  7505              jnz 0x6a6
000006A1  B80100            mov ax,0x1
000006A4  EB0D              jmp short 0x6b3
000006A6  8BDE              mov bx,si
000006A8  D1E3              shl bx,1
000006AA  8B87F444          mov ax,[bx+0x44f4]
000006AE  D1E0              shl ax,1
000006B0  050300            add ax,0x3
000006B3  8BF8              mov di,ax
000006B5  83FAFF            cmp dx,byte -0x1
000006B8  7509              jnz 0x6c3
000006BA  8BC7              mov ax,di
000006BC  050A00            add ax,0xa
000006BF  8BF8              mov di,ax
000006C1  EB20              jmp short 0x6e3
000006C3  A0AA37            mov al,[0x37aa]
000006C6  98                cbw
000006C7  3BC2              cmp ax,dx
000006C9  7509              jnz 0x6d4
000006CB  8BC7              mov ax,di
000006CD  050900            add ax,0x9
000006D0  8BF8              mov di,ax
000006D2  EB0F              jmp short 0x6e3
000006D4  8BDA              mov bx,dx
000006D6  80BF400801        cmp byte [bx+0x840],0x1
000006DB  7506              jnz 0x6e3
000006DD  8BC7              mov ax,di
000006DF  F7D8              neg ax
000006E1  8BF8              mov di,ax
000006E3  3B7EFC            cmp di,[bp-0x4]
000006E6  7F02              jg 0x6ea
000006E8  EB06              jmp short 0x6f0
000006EA  8976FE            mov [bp-0x2],si
000006ED  897EFC            mov [bp-0x4],di
000006F0  41                inc cx
000006F1  46                inc si
000006F2  83F90B            cmp cx,byte +0xb
000006F5  7F03              jg 0x6fa
000006F7  E956FF            jmp 0x650
000006FA  8B46FE            mov ax,[bp-0x2]
000006FD  EB00              jmp short 0x6ff
000006FF  5F                pop di
00000700  5E                pop si
00000701  8BE5              mov sp,bp
00000703  5D                pop bp
00000704  CB                retf

; ---- 1B03:0705 ----
00000705  55                push bp
00000706  8BEC              mov bp,sp
00000708  9A4007031B        call 0x1b03:0x740
0000070D  9A6307031B        call 0x1b03:0x763
00000712  9AC407031B        call 0x1b03:0x7c4
00000717  9A4908031B        call 0x1b03:0x849
0000071C  9A1B00960A        call 0xa96:0x1b
00000721  9AED08031B        call 0x1b03:0x8ed
00000726  33C0              xor ax,ax
00000728  BA6400            mov dx,0x64
0000072B  50                push ax
0000072C  52                push dx
0000072D  9A8501FD21        call 0x21fd:0x185
00000732  59                pop cx
00000733  59                pop cx
00000734  9AEE13031B        call 0x1b03:0x13ee
00000739  9AC00D031B        call 0x1b03:0xdc0
0000073E  5D                pop bp
0000073F  CB                retf

; ---- 1B03:0740 ----
00000740  55                push bp
00000741  8BEC              mov bp,sp
00000743  C606B45300        mov byte [0x53b4],0x0
00000748  C606B25300        mov byte [0x53b2],0x0
0000074D  C606B553FF        mov byte [0x53b5],0xff
00000752  C606385300        mov byte [0x5338],0x0
00000757  C606B35300        mov byte [0x53b3],0x0
0000075C  C6066F5400        mov byte [0x546f],0x0
00000761  5D                pop bp
00000762  CB                retf

; ---- 1B03:0763 ----
00000763  55                push bp
00000764  8BEC              mov bp,sp
00000766  56                push si
00000767  C606AA3700        mov byte [0x37aa],0x0
0000076C  EB4C              jmp short 0x7ba
0000076E  A0AA37            mov al,[0x37aa]
00000771  98                cbw
00000772  D1E0              shl ax,1
00000774  8BD8              mov bx,ax
00000776  8B9F0E45          mov bx,[bx+0x450e]
0000077A  D1E3              shl bx,1
0000077C  8BB78A34          mov si,[bx+0x348a]
00000780  83FEFF            cmp si,byte -0x1
00000783  7426              jz 0x7ab
00000785  A0AA37            mov al,[0x37aa]
00000788  98                cbw
00000789  3BC6              cmp ax,si
0000078B  741E              jz 0x7ab
0000078D  B80400            mov ax,0x4
00000790  50                push ax
00000791  1E                push ds
00000792  B8DC3A            mov ax,0x3adc
00000795  50                push ax
00000796  9AEA10031B        call 0x1b03:0x10ea
0000079B  83C406            add sp,byte +0x6
0000079E  A0AA37            mov al,[0x37aa]
000007A1  98                cbw
000007A2  8BD8              mov bx,ax
000007A4  C6876A5401        mov byte [bx+0x546a],0x1
000007A9  EB0B              jmp short 0x7b6
000007AB  A0AA37            mov al,[0x37aa]
000007AE  98                cbw
000007AF  8BD8              mov bx,ax
000007B1  C6876A5400        mov byte [bx+0x546a],0x0
000007B6  FE06AA37          inc byte [0x37aa]
000007BA  803EAA3704        cmp byte [0x37aa],0x4
000007BF  7CAD              jl 0x76e
000007C1  5E                pop si
000007C2  5D                pop bp
000007C3  CB                retf

; ---- 1B03:07C4 ----
000007C4  55                push bp
000007C5  8BEC              mov bp,sp
000007C7  9A1B00960A        call 0xa96:0x1b
000007CC  C606AA3700        mov byte [0x37aa],0x0
000007D1  EB68              jmp short 0x83b
000007D3  A0AA37            mov al,[0x37aa]
000007D6  98                cbw
000007D7  8BD8              mov bx,ax
000007D9  C687A65300        mov byte [bx+0x53a6],0x0
000007DE  A0AA37            mov al,[0x37aa]
000007E1  98                cbw
000007E2  8BD8              mov bx,ax
000007E4  C687AD5300        mov byte [bx+0x53ad],0x0
000007E9  A0AA37            mov al,[0x37aa]
000007EC  98                cbw
000007ED  D1E0              shl ax,1
000007EF  D1E0              shl ax,1
000007F1  8BD8              mov bx,ax
000007F3  8A87EA2E          mov al,[bx+0x2eea]
000007F7  0006B253          add [0x53b2],al
000007FB  33C0              xor ax,ax
000007FD  50                push ax
000007FE  B8FFFF            mov ax,0xffff
00000801  50                push ax
00000802  33C0              xor ax,ax
00000804  50                push ax
00000805  A0AA37            mov al,[0x37aa]
00000808  98                cbw
00000809  D1E0              shl ax,1
0000080B  8BD8              mov bx,ax
0000080D  FFB7000D          push word [bx+0xd00]
00000811  FF36563B          push word [0x3b56]
00000815  FF36543B          push word [0x3b54]
00000819  A0AA37            mov al,[0x37aa]
0000081C  98                cbw
0000081D  BA0600            mov dx,0x6
00000820  F7EA              imul dx
00000822  05B400            add ax,0xb4
00000825  50                push ax
00000826  B87100            mov ax,0x71
00000829  50                push ax
0000082A  9A4D00690A        call 0xa69:0x4d
0000082F  83C410            add sp,byte +0x10
00000832  9A750D031B        call 0x1b03:0xd75
00000837  FE06AA37          inc byte [0x37aa]
0000083B  803EAA3704        cmp byte [0x37aa],0x4
00000840  7C91              jl 0x7d3
00000842  9A0D06FD21        call 0x21fd:0x60d
00000847  5D                pop bp
00000848  CB                retf

; ---- 1B03:0849 ----
00000849  55                push bp
0000084A  8BEC              mov bp,sp
0000084C  56                push si
0000084D  33F6              xor si,si
0000084F  B80300            mov ax,0x3
00000852  50                push ax
00000853  9ABA01FD21        call 0x21fd:0x1ba
00000858  59                pop cx
00000859  A2AA37            mov [0x37aa],al
0000085C  C606AC5300        mov byte [0x53ac],0x0
00000861  EB7B              jmp short 0x8de
00000863  A0AA37            mov al,[0x37aa]
00000866  98                cbw
00000867  D1E0              shl ax,1
00000869  D1E0              shl ax,1
0000086B  8BD8              mov bx,ax
0000086D  8A87EA2E          mov al,[bx+0x2eea]
00000871  50                push ax
00000872  A0AA37            mov al,[0x37aa]
00000875  98                cbw
00000876  8BD8              mov bx,ax
00000878  58                pop ax
00000879  3A87A653          cmp al,[bx+0x53a6]
0000087D  7E4E              jng 0x8cd
0000087F  A0AC53            mov al,[0x53ac]
00000882  98                cbw
00000883  D1E0              shl ax,1
00000885  8BD8              mov bx,ax
00000887  89B7B653          mov [bx+0x53b6],si
0000088B  8BC6              mov ax,si
0000088D  05FBFF            add ax,0xfffb
00000890  8BF0              mov si,ax
00000892  B82B01            mov ax,0x12b
00000895  50                push ax
00000896  9ABA01FD21        call 0x21fd:0x1ba
0000089B  59                pop cx
0000089C  050A00            add ax,0xa
0000089F  50                push ax
000008A0  A0AC53            mov al,[0x53ac]
000008A3  98                cbw
000008A4  D1E0              shl ax,1
000008A6  8BD8              mov bx,ax
000008A8  58                pop ax
000008A9  89873A53          mov [bx+0x533a],ax
000008AD  A0AC53            mov al,[0x53ac]
000008B0  98                cbw
000008B1  8A16AA37          mov dl,[0x37aa]
000008B5  8BD8              mov bx,ax
000008B7  88973454          mov [bx+0x5434],dl
000008BB  A0AA37            mov al,[0x37aa]
000008BE  98                cbw
000008BF  8BD8              mov bx,ax
000008C1  FE87A653          inc byte [bx+0x53a6]
000008C5  FE06B453          inc byte [0x53b4]
000008C9  FE06AC53          inc byte [0x53ac]
000008CD  A0AA37            mov al,[0x37aa]
000008D0  FEC0              inc al
000008D2  A2AA37            mov [0x37aa],al
000008D5  3C03              cmp al,0x3
000008D7  7E05              jng 0x8de
000008D9  C606AA3700        mov byte [0x37aa],0x0
000008DE  A0AC53            mov al,[0x53ac]
000008E1  3A06B253          cmp al,[0x53b2]
000008E5  7D03              jnl 0x8ea
000008E7  E979FF            jmp 0x863
000008EA  5E                pop si
000008EB  5D                pop bp
000008EC  CB                retf

; ---- 1B03:08ED ----
000008ED  55                push bp
000008EE  8BEC              mov bp,sp
000008F0  E95101            jmp 0xa44
000008F3  9A1F05FD21        call 0x21fd:0x51f
000008F8  33C0              xor ax,ax
000008FA  BA0300            mov dx,0x3
000008FD  50                push ax
000008FE  52                push dx
000008FF  9A8501FD21        call 0x21fd:0x185
00000904  59                pop cx
00000905  59                pop cx
00000906  FF362E08          push word [0x82e]
0000090A  9A1C07CD23        call 0x23cd:0x71c
0000090F  59                pop cx
00000910  9AA806960A        call 0xa96:0x6a8
00000915  9A1B00960A        call 0xa96:0x1b
0000091A  9A4502FD21        call 0x21fd:0x245
0000091F  C606AC5300        mov byte [0x53ac],0x0
00000924  E9E800            jmp 0xa0f
00000927  A0AC53            mov al,[0x53ac]
0000092A  98                cbw
0000092B  D1E0              shl ax,1
0000092D  8BD8              mov bx,ax
0000092F  83BFB65359        cmp word [bx+0x53b6],byte +0x59
00000934  7E03              jng 0x939
00000936  E9D200            jmp 0xa0b
00000939  A0AC53            mov al,[0x53ac]
0000093C  98                cbw
0000093D  D1E0              shl ax,1
0000093F  8BD8              mov bx,ax
00000941  83BFB65300        cmp word [bx+0x53b6],byte +0x0
00000946  7D03              jnl 0x94b
00000948  E9B400            jmp 0x9ff
0000094B  A0AC53            mov al,[0x53ac]
0000094E  98                cbw
0000094F  8BD8              mov bx,ax
00000951  8A873454          mov al,[bx+0x5434]
00000955  A2AA37            mov [0x37aa],al
00000958  A0AC53            mov al,[0x53ac]
0000095B  98                cbw
0000095C  D1E0              shl ax,1
0000095E  8BD8              mov bx,ax
00000960  83BFB65300        cmp word [bx+0x53b6],byte +0x0
00000965  750E              jnz 0x975
00000967  B81200            mov ax,0x12
0000096A  50                push ax
0000096B  33C0              xor ax,ax
0000096D  50                push ax
0000096E  9A5400A721        call 0x21a7:0x54
00000973  59                pop cx
00000974  59                pop cx
00000975  A0AC53            mov al,[0x53ac]
00000978  98                cbw
00000979  D1E0              shl ax,1
0000097B  8BD8              mov bx,ax
0000097D  FFB7B653          push word [bx+0x53b6]
00000981  A0AC53            mov al,[0x53ac]
00000984  98                cbw
00000985  D1E0              shl ax,1
00000987  8BD8              mov bx,ax
00000989  FFB73A53          push word [bx+0x533a]
0000098D  9ACF0C031B        call 0x1b03:0xccf
00000992  59                pop cx
00000993  59                pop cx
00000994  9A7A0B031B        call 0x1b03:0xb7a
00000999  A0AC53            mov al,[0x53ac]
0000099C  3A06B553          cmp al,[0x53b5]
000009A0  7507              jnz 0x9a9
000009A2  9AE70B031B        call 0x1b03:0xbe7
000009A7  EB54              jmp short 0x9fd
000009A9  FF362654          push word [0x5426]
000009AD  FF362454          push word [0x5424]
000009B1  9ADE06CD23        call 0x23cd:0x6de
000009B6  59                pop cx
000009B7  59                pop cx
000009B8  3D0100            cmp ax,0x1
000009BB  751C              jnz 0x9d9
000009BD  A0AA37            mov al,[0x37aa]
000009C0  98                cbw
000009C1  D1E0              shl ax,1
000009C3  8BD8              mov bx,ax
000009C5  FFB7F80C          push word [bx+0xcf8]
000009C9  FF362654          push word [0x5426]
000009CD  FF362454          push word [0x5424]
000009D1  9AFB03960A        call 0xa96:0x3fb
000009D6  83C406            add sp,byte +0x6
000009D9  A0AC53            mov al,[0x53ac]
000009DC  98                cbw
000009DD  D1E0              shl ax,1
000009DF  8BD8              mov bx,ax
000009E1  8B87B653          mov ax,[bx+0x53b6]
000009E5  40                inc ax
000009E6  50                push ax
000009E7  A0AC53            mov al,[0x53ac]
000009EA  98                cbw
000009EB  D1E0              shl ax,1
000009ED  8BD8              mov bx,ax
000009EF  58                pop ax
000009F0  8987B653          mov [bx+0x53b6],ax
000009F4  3D5900            cmp ax,0x59
000009F7  7E04              jng 0x9fd
000009F9  FE0EB453          dec byte [0x53b4]
000009FD  EB0C              jmp short 0xa0b
000009FF  A0AC53            mov al,[0x53ac]
00000A02  98                cbw
00000A03  D1E0              shl ax,1
00000A05  8BD8              mov bx,ax
00000A07  FF87B653          inc word [bx+0x53b6]
00000A0B  FE06AC53          inc byte [0x53ac]
00000A0F  A0AC53            mov al,[0x53ac]
00000A12  98                cbw
00000A13  50                push ax
00000A14  A0B253            mov al,[0x53b2]
00000A17  98                cbw
00000A18  48                dec ax
00000A19  5A                pop dx
00000A1A  3BD0              cmp dx,ax
00000A1C  7F03              jg 0xa21
00000A1E  E906FF            jmp 0x927
00000A21  B80300            mov ax,0x3
00000A24  50                push ax
00000A25  9AD712031B        call 0x1b03:0x12d7
00000A2A  59                pop cx
00000A2B  9A620A031B        call 0x1b03:0xa62
00000A30  9A890C031B        call 0x1b03:0xc89
00000A35  9AA806960A        call 0xa96:0x6a8
00000A3A  FF362E08          push word [0x82e]
00000A3E  9AE906CD23        call 0x23cd:0x6e9
00000A43  59                pop cx
00000A44  803EB45300        cmp byte [0x53b4],0x0
00000A49  7E03              jng 0xa4e
00000A4B  E9A5FE            jmp 0x8f3
00000A4E  33C0              xor ax,ax
00000A50  50                push ax
00000A51  33C0              xor ax,ax
00000A53  50                push ax
00000A54  9A5400A721        call 0x21a7:0x54
00000A59  59                pop cx
00000A5A  59                pop cx
00000A5B  9A6506FD21        call 0x21fd:0x665
00000A60  5D                pop bp
00000A61  CB                retf

; ---- 1B03:0A62 ----
00000A62  55                push bp
00000A63  8BEC              mov bp,sp
00000A65  803E385300        cmp byte [0x5338],0x0
00000A6A  7536              jnz 0xaa2
00000A6C  803E6E54FF        cmp byte [0x546e],0xff
00000A71  7503              jnz 0xa76
00000A73  E9E200            jmp 0xb58
00000A76  803EB553FF        cmp byte [0x53b5],0xff
00000A7B  7403              jz 0xa80
00000A7D  E9D800            jmp 0xb58
00000A80  A06E54            mov al,[0x546e]
00000A83  98                cbw
00000A84  8BD8              mov bx,ax
00000A86  8A87140D          mov al,[bx+0xd14]
00000A8A  B400              mov ah,0x0
00000A8C  50                push ax
00000A8D  9ABA01FD21        call 0x21fd:0x1ba
00000A92  59                pop cx
00000A93  0BC0              or ax,ax
00000A95  7403              jz 0xa9a
00000A97  E9BE00            jmp 0xb58
00000A9A  C606385301        mov byte [0x5338],0x1
00000A9F  E9B600            jmp 0xb58
00000AA2  A03853            mov al,[0x5338]
00000AA5  FEC0              inc al
00000AA7  A23853            mov [0x5338],al
00000AAA  98                cbw
00000AAB  48                dec ax
00000AAC  48                dec ax
00000AAD  8BD8              mov bx,ax
00000AAF  83FB0F            cmp bx,byte +0xf
00000AB2  7603              jna 0xab7
00000AB4  E9A100            jmp 0xb58
00000AB7  D1E3              shl bx,1
00000AB9  2EFFA75A0B        jmp [cs:bx+0xb5a]
00000ABE  B80500            mov ax,0x5
00000AC1  50                push ax
00000AC2  9A1B13031B        call 0x1b03:0x131b
00000AC7  59                pop cx
00000AC8  B80100            mov ax,0x1
00000ACB  50                push ax
00000ACC  9A4513031B        call 0x1b03:0x1345
00000AD1  59                pop cx
00000AD2  E98300            jmp 0xb58
00000AD5  33C0              xor ax,ax
00000AD7  50                push ax
00000AD8  9A4513031B        call 0x1b03:0x1345
00000ADD  59                pop cx
00000ADE  EB78              jmp short 0xb58
00000AE0  B80A00            mov ax,0xa
00000AE3  50                push ax
00000AE4  9ABA01FD21        call 0x21fd:0x1ba
00000AE9  59                pop cx
00000AEA  0BC0              or ax,ax
00000AEC  7538              jnz 0xb26
00000AEE  B001              mov al,0x1
00000AF0  50                push ax
00000AF1  9A8D12031B        call 0x1b03:0x128d
00000AF6  59                pop cx
00000AF7  B80400            mov ax,0x4
00000AFA  50                push ax
00000AFB  9A1B13031B        call 0x1b03:0x131b
00000B00  59                pop cx
00000B01  9AC913031B        call 0x1b03:0x13c9
00000B06  C6066F5407        mov byte [0x546f],0x7
00000B0B  B81300            mov ax,0x13
00000B0E  50                push ax
00000B0F  B80200            mov ax,0x2
00000B12  50                push ax
00000B13  9A5400A721        call 0x21a7:0x54
00000B18  59                pop cx
00000B19  59                pop cx
00000B1A  C6066E54FF        mov byte [0x546e],0xff
00000B1F  C606385300        mov byte [0x5338],0x0
00000B24  EB19              jmp short 0xb3f
00000B26  B80100            mov ax,0x1
00000B29  50                push ax
00000B2A  9A9D13031B        call 0x1b03:0x139d
00000B2F  59                pop cx
00000B30  B81400            mov ax,0x14
00000B33  50                push ax
00000B34  B80100            mov ax,0x1
00000B37  50                push ax
00000B38  9A5400A721        call 0x21a7:0x54
00000B3D  59                pop cx
00000B3E  59                pop cx
00000B3F  EB17              jmp short 0xb58
00000B41  33C0              xor ax,ax
00000B43  50                push ax
00000B44  9A9D13031B        call 0x1b03:0x139d
00000B49  59                pop cx
00000B4A  EB0C              jmp short 0xb58
00000B4C  C606B35301        mov byte [0x53b3],0x1
00000B51  C606385300        mov byte [0x5338],0x0
00000B56  EB00              jmp short 0xb58
00000B58  5D                pop bp
00000B59  CB                retf
00000B5A  BE0A58            mov si,0x580a
00000B5D  0B580B            or bx,[bx+si+0xb]
00000B60  58                pop ax
00000B61  0B580B            or bx,[bx+si+0xb]
00000B64  58                pop ax
00000B65  0BD5              or dx,bp
00000B67  0A580B            or bl,[bx+si+0xb]
00000B6A  58                pop ax
00000B6B  0BE0              or sp,ax
00000B6D  0A580B            or bl,[bx+si+0xb]
00000B70  58                pop ax
00000B71  0B410B            or ax,[bx+di+0xb]
00000B74  58                pop ax
00000B75  0B580B            or bx,[bx+si+0xb]
00000B78  4C                dec sp
00000B79  0B558B            or dx,[di-0x75]
00000B7C  EC                in al,dx
00000B7D  A0AA37            mov al,[0x37aa]
00000B80  98                cbw
00000B81  8BD8              mov bx,ax
00000B83  80BF6A5401        cmp byte [bx+0x546a],0x1
00000B88  7415              jz 0xb9f
00000B8A  803E6E54FF        cmp byte [0x546e],0xff
00000B8F  7522              jnz 0xbb3
00000B91  B88813            mov ax,0x1388
00000B94  50                push ax
00000B95  9ABA01FD21        call 0x21fd:0x1ba
00000B9A  59                pop cx
00000B9B  0BC0              or ax,ax
00000B9D  7514              jnz 0xbb3
00000B9F  803EB553FF        cmp byte [0x53b5],0xff
00000BA4  7402              jz 0xba8
00000BA6  EB3D              jmp short 0xbe5
00000BA8  803E385300        cmp byte [0x5338],0x0
00000BAD  7E02              jng 0xbb1
00000BAF  EB34              jmp short 0xbe5
00000BB1  EB09              jmp short 0xbbc
00000BB3  803EB35300        cmp byte [0x53b3],0x0
00000BB8  7502              jnz 0xbbc
00000BBA  EB29              jmp short 0xbe5
00000BBC  813E2654BE00      cmp word [0x5426],0xbe
00000BC2  7E02              jng 0xbc6
00000BC4  EB1F              jmp short 0xbe5
00000BC6  C606225400        mov byte [0x5422],0x0
00000BCB  A0AC53            mov al,[0x53ac]
00000BCE  A2B553            mov [0x53b5],al
00000BD1  B81500            mov ax,0x15
00000BD4  50                push ax
00000BD5  B80200            mov ax,0x2
00000BD8  50                push ax
00000BD9  9A5400A721        call 0x21a7:0x54
00000BDE  59                pop cx
00000BDF  59                pop cx
00000BE0  C606B35300        mov byte [0x53b3],0x0
00000BE5  5D                pop bp
00000BE6  CB                retf

; ---- 1B03:0BE7 ----
00000BE7  55                push bp
00000BE8  8BEC              mov bp,sp
00000BEA  803E225400        cmp byte [0x5422],0x0
00000BEF  7545              jnz 0xc36
00000BF1  C6066F5407        mov byte [0x546f],0x7
00000BF6  FF362654          push word [0x5426]
00000BFA  FF362454          push word [0x5424]
00000BFE  9ADE06CD23        call 0x23cd:0x6de
00000C03  59                pop cx
00000C04  59                pop cx
00000C05  3D0100            cmp ax,0x1
00000C08  7527              jnz 0xc31
00000C0A  C606365301        mov byte [0x5336],0x1
00000C0F  33C0              xor ax,ax
00000C11  50                push ax
00000C12  B80400            mov ax,0x4
00000C15  50                push ax
00000C16  9A1204AF21        call 0x21af:0x412
00000C1B  59                pop cx
00000C1C  59                pop cx
00000C1D  A12454            mov ax,[0x5424]
00000C20  05F8FF            add ax,0xfff8
00000C23  A38833            mov [0x3388],ax
00000C26  A12654            mov ax,[0x5426]
00000C29  05FAFF            add ax,0xfffa
00000C2C  A38A33            mov [0x338a],ax
00000C2F  EB05              jmp short 0xc36
00000C31  C606365300        mov byte [0x5336],0x0
00000C36  803E22540F        cmp byte [0x5422],0xf
00000C3B  7D16              jnl 0xc53
00000C3D  803E365301        cmp byte [0x5336],0x1
00000C42  750F              jnz 0xc53
00000C44  A02254            mov al,[0x5422]
00000C47  98                cbw
00000C48  BB0300            mov bx,0x3
00000C4B  99                cwd
00000C4C  F7FB              idiv bx
00000C4E  A38C33            mov [0x338c],ax
00000C51  EB30              jmp short 0xc83
00000C53  B80400            mov ax,0x4
00000C56  50                push ax
00000C57  9A5E04AF21        call 0x21af:0x45e
00000C5C  59                pop cx
00000C5D  A0AC53            mov al,[0x53ac]
00000C60  98                cbw
00000C61  D1E0              shl ax,1
00000C63  8BD8              mov bx,ax
00000C65  C787B6535A00      mov word [bx+0x53b6],0x5a
00000C6B  FE0EB453          dec byte [0x53b4]
00000C6F  A0AA37            mov al,[0x37aa]
00000C72  98                cbw
00000C73  8BD8              mov bx,ax
00000C75  FE87AD53          inc byte [bx+0x53ad]
00000C79  9A750D031B        call 0x1b03:0xd75
00000C7E  C606B553FF        mov byte [0x53b5],0xff
00000C83  FE062254          inc byte [0x5422]
00000C87  5D                pop bp
00000C88  CB                retf

; ---- 1B03:0C89 ----
00000C89  55                push bp
00000C8A  8BEC              mov bp,sp
00000C8C  803E6F5400        cmp byte [0x546f],0x0
00000C91  7F02              jg 0xc95
00000C93  EB15              jmp short 0xcaa
00000C95  A06F54            mov al,[0x546f]
00000C98  FEC8              dec al
00000C9A  A26F54            mov [0x546f],al
00000C9D  98                cbw
00000C9E  50                push ax
00000C9F  B87707            mov ax,0x777
00000CA2  50                push ax
00000CA3  9AEB03FD21        call 0x21fd:0x3eb
00000CA8  59                pop cx
00000CA9  59                pop cx
00000CAA  5D                pop bp
00000CAB  CB                retf

; ---- 1B03:0CAC ----
00000CAC  55                push bp
00000CAD  8BEC              mov bp,sp
00000CAF  56                push si
00000CB0  33F6              xor si,si
00000CB2  EB13              jmp short 0xcc7
00000CB4  0E                push cs
00000CB5  E8D1FF            call 0xc89
00000CB8  33C0              xor ax,ax
00000CBA  BA0400            mov dx,0x4
00000CBD  50                push ax
00000CBE  52                push dx
00000CBF  9A8501FD21        call 0x21fd:0x185
00000CC4  59                pop cx
00000CC5  59                pop cx
00000CC6  46                inc si
00000CC7  83FE0A            cmp si,byte +0xa
00000CCA  7CE8              jl 0xcb4
00000CCC  5E                pop si
00000CCD  5D                pop bp
00000CCE  CB                retf

; ---- 1B03:0CCF ----
00000CCF  55                push bp
00000CD0  8BEC              mov bp,sp
00000CD2  4C                dec sp
00000CD3  4C                dec sp
00000CD4  56                push si
00000CD5  57                push di
00000CD6  8B4E06            mov cx,[bp+0x6]
00000CD9  A0AA37            mov al,[0x37aa]
00000CDC  98                cbw
00000CDD  D1E0              shl ax,1
00000CDF  8BD8              mov bx,ax
00000CE1  8B9F0E45          mov bx,[bx+0x450e]
00000CE5  D1E3              shl bx,1
00000CE7  8B87840C          mov ax,[bx+0xc84]
00000CEB  050800            add ax,0x8
00000CEE  8BF0              mov si,ax
00000CF0  A0AA37            mov al,[0x37aa]
00000CF3  98                cbw
00000CF4  D1E0              shl ax,1
00000CF6  8BD8              mov bx,ax
00000CF8  8B9F0E45          mov bx,[bx+0x450e]
00000CFC  8A879C0C          mov al,[bx+0xc9c]
00000D00  B400              mov ah,0x0
00000D02  050700            add ax,0x7
00000D05  8946FE            mov [bp-0x2],ax
00000D08  BFC700            mov di,0xc7
00000D0B  3BCE              cmp cx,si
00000D0D  7501              jnz 0xd10
00000D0F  49                dec cx
00000D10  8B5E08            mov bx,[bp+0x8]
00000D13  D1E3              shl bx,1
00000D15  8B87170D          mov ax,[bx+0xd17]
00000D19  BA0A00            mov dx,0xa
00000D1C  F7EA              imul dx
00000D1E  8BD9              mov bx,cx
00000D20  2BDE              sub bx,si
00000D22  50                push ax
00000D23  B81027            mov ax,0x2710
00000D26  99                cwd
00000D27  F7FB              idiv bx
00000D29  8BD8              mov bx,ax
00000D2B  58                pop ax
00000D2C  99                cwd
00000D2D  F7FB              idiv bx
00000D2F  8BD6              mov dx,si
00000D31  03D0              add dx,ax
00000D33  89162454          mov [0x5424],dx
00000D37  833E245400        cmp word [0x5424],byte +0x0
00000D3C  7D06              jnl 0xd44
00000D3E  C70624540000      mov word [0x5424],0x0
00000D44  BB5900            mov bx,0x59
00000D47  2B5E08            sub bx,[bp+0x8]
00000D4A  D1E3              shl bx,1
00000D4C  8B87170D          mov ax,[bx+0xd17]
00000D50  BA0A00            mov dx,0xa
00000D53  F7EA              imul dx
00000D55  8BDF              mov bx,di
00000D57  2B5EFE            sub bx,[bp-0x2]
00000D5A  50                push ax
00000D5B  B81027            mov ax,0x2710
00000D5E  99                cwd
00000D5F  F7FB              idiv bx
00000D61  8BD8              mov bx,ax
00000D63  58                pop ax
00000D64  99                cwd
00000D65  F7FB              idiv bx
00000D67  8BD7              mov dx,di
00000D69  2BD0              sub dx,ax
00000D6B  89162654          mov [0x5426],dx
00000D6F  5F                pop di
00000D70  5E                pop si
00000D71  8BE5              mov sp,bp
00000D73  5D                pop bp
00000D74  CB                retf

; ---- 1B03:0D75 ----
00000D75  55                push bp
00000D76  8BEC              mov bp,sp
00000D78  9A5414031B        call 0x1b03:0x1454
00000D7D  33C0              xor ax,ax
00000D7F  50                push ax
00000D80  B8FFFF            mov ax,0xffff
00000D83  50                push ax
00000D84  33C0              xor ax,ax
00000D86  50                push ax
00000D87  A0AA37            mov al,[0x37aa]
00000D8A  98                cbw
00000D8B  D1E0              shl ax,1
00000D8D  8BD8              mov bx,ax
00000D8F  FFB7000D          push word [bx+0xd00]
00000D93  B80300            mov ax,0x3
00000D96  50                push ax
00000D97  A0AA37            mov al,[0x37aa]
00000D9A  98                cbw
00000D9B  8BD8              mov bx,ax
00000D9D  8A87AD53          mov al,[bx+0x53ad]
00000DA1  98                cbw
00000DA2  99                cwd
00000DA3  52                push dx
00000DA4  50                push ax
00000DA5  A0AA37            mov al,[0x37aa]
00000DA8  98                cbw
00000DA9  BA0600            mov dx,0x6
00000DAC  F7EA              imul dx
00000DAE  05B400            add ax,0xb4
00000DB1  50                push ax
00000DB2  B8AD00            mov ax,0xad
00000DB5  50                push ax
00000DB6  9A1B01690A        call 0xa69:0x11b
00000DBB  83C412            add sp,byte +0x12
00000DBE  5D                pop bp
00000DBF  CB                retf

; ---- 1B03:0DC0 ----
00000DC0  55                push bp
00000DC1  8BEC              mov bp,sp
00000DC3  C606AA3700        mov byte [0x37aa],0x0
00000DC8  EB3E              jmp short 0xe08
00000DCA  A0AA37            mov al,[0x37aa]
00000DCD  98                cbw
00000DCE  8BD8              mov bx,ax
00000DD0  80BFAD5303        cmp byte [bx+0x53ad],0x3
00000DD5  7E2D              jng 0xe04
00000DD7  A0AA37            mov al,[0x37aa]
00000DDA  98                cbw
00000DDB  D1E0              shl ax,1
00000DDD  D1E0              shl ax,1
00000DDF  8BD8              mov bx,ax
00000DE1  8A87EA2E          mov al,[bx+0x2eea]
00000DE5  50                push ax
00000DE6  A0AA37            mov al,[0x37aa]
00000DE9  98                cbw
00000DEA  8BD8              mov bx,ax
00000DEC  58                pop ax
00000DED  3A87AD53          cmp al,[bx+0x53ad]
00000DF1  7511              jnz 0xe04
00000DF3  B80700            mov ax,0x7
00000DF6  50                push ax
00000DF7  1E                push ds
00000DF8  B8EC3A            mov ax,0x3aec
00000DFB  50                push ax
00000DFC  9AEA10031B        call 0x1b03:0x10ea
00000E01  83C406            add sp,byte +0x6
00000E04  FE06AA37          inc byte [0x37aa]
00000E08  803EAA3704        cmp byte [0x37aa],0x4
00000E0D  7CBB              jl 0xdca
00000E0F  5D                pop bp
00000E10  CB                retf

; ---- 1B03:0E11 ----
00000E11  55                push bp
00000E12  8BEC              mov bp,sp
00000E14  56                push si
00000E15  33F6              xor si,si
00000E17  EB7E              jmp short 0xe97
00000E19  8BC6              mov ax,si
00000E1B  99                cwd
00000E1C  F73EFC08          idiv word [0x8fc]
00000E20  8816B02E          mov [0x2eb0],dl
00000E24  8BC6              mov ax,si
00000E26  99                cwd
00000E27  F73EFC08          idiv word [0x8fc]
00000E2B  A2002F            mov [0x2f00],al
00000E2E  A0B02E            mov al,[0x2eb0]
00000E31  98                cbw
00000E32  BA0600            mov dx,0x6
00000E35  F7EA              imul dx
00000E37  50                push ax
00000E38  A0002F            mov al,[0x2f00]
00000E3B  98                cbw
00000E3C  5B                pop bx
00000E3D  03D8              add bx,ax
00000E3F  8A874244          mov al,[bx+0x4442]
00000E43  A2AA37            mov [0x37aa],al
00000E46  3CFF              cmp al,0xff
00000E48  7502              jnz 0xe4c
00000E4A  EB4A              jmp short 0xe96
00000E4C  A0AA37            mov al,[0x37aa]
00000E4F  98                cbw
00000E50  8BD8              mov bx,ax
00000E52  80BFAD5300        cmp byte [bx+0x53ad],0x0
00000E57  7E3D              jng 0xe96
00000E59  A0B02E            mov al,[0x2eb0]
00000E5C  98                cbw
00000E5D  BA0600            mov dx,0x6
00000E60  F7EA              imul dx
00000E62  50                push ax
00000E63  A0002F            mov al,[0x2f00]
00000E66  98                cbw
00000E67  5B                pop bx
00000E68  03D8              add bx,ax
00000E6A  80BFB12E03        cmp byte [bx+0x2eb1],0x3
00000E6F  7525              jnz 0xe96
00000E71  B8FEFF            mov ax,0xfffe
00000E74  50                push ax
00000E75  A0AA37            mov al,[0x37aa]
00000E78  98                cbw
00000E79  50                push ax
00000E7A  A0002F            mov al,[0x2f00]
00000E7D  98                cbw
00000E7E  50                push ax
00000E7F  A0B02E            mov al,[0x2eb0]
00000E82  98                cbw
00000E83  50                push ax
00000E84  9AD2100906        call 0x609:0x10d2
00000E89  83C408            add sp,byte +0x8
00000E8C  A0AA37            mov al,[0x37aa]
00000E8F  98                cbw
00000E90  8BD8              mov bx,ax
00000E92  FE8FAD53          dec byte [bx+0x53ad]
00000E96  46                inc si
00000E97  A1FC08            mov ax,[0x8fc]
00000E9A  F72EFE08          imul word [0x8fe]
00000E9E  48                dec ax
00000E9F  3BC6              cmp ax,si
00000EA1  7C03              jl 0xea6
00000EA3  E973FF            jmp 0xe19
00000EA6  5E                pop si
00000EA7  5D                pop bp
00000EA8  CB                retf

; ---- 1B03:0EA9 ----
00000EA9  55                push bp
00000EAA  8BEC              mov bp,sp
00000EAC  83EC04            sub sp,byte +0x4
00000EAF  C606AA3700        mov byte [0x37aa],0x0
00000EB4  E92502            jmp 0x10dc
00000EB7  A0AA37            mov al,[0x37aa]
00000EBA  98                cbw
00000EBB  D1E0              shl ax,1
00000EBD  D1E0              shl ax,1
00000EBF  8BD8              mov bx,ax
00000EC1  80BFEA2E00        cmp byte [bx+0x2eea],0x0
00000EC6  7503              jnz 0xecb
00000EC8  E90D02            jmp 0x10d8
00000ECB  A0AA37            mov al,[0x37aa]
00000ECE  98                cbw
00000ECF  8BD8              mov bx,ax
00000ED1  8A87423D          mov al,[bx+0x3d42]
00000ED5  B400              mov ah,0x0
00000ED7  D1E0              shl ax,1
00000ED9  D1E0              shl ax,1
00000EDB  BA1600            mov dx,0x16
00000EDE  2BD0              sub dx,ax
00000EE0  52                push dx
00000EE1  9ABA01FD21        call 0x21fd:0x1ba
00000EE6  59                pop cx
00000EE7  3D0500            cmp ax,0x5
00000EEA  7578              jnz 0xf64
00000EEC  803EF24B01        cmp byte [0x4bf2],0x1
00000EF1  7571              jnz 0xf64
00000EF3  A13644            mov ax,[0x4436]
00000EF6  D1E0              shl ax,1
00000EF8  50                push ax
00000EF9  9ABA01FD21        call 0x21fd:0x1ba
00000EFE  59                pop cx
00000EFF  40                inc ax
00000F00  40                inc ax
00000F01  8946FC            mov [bp-0x4],ax
00000F04  C746FE0000        mov word [bp-0x2],0x0
00000F09  FF36223B          push word [0x3b22]
00000F0D  FF36203B          push word [0x3b20]
00000F11  B80300            mov ax,0x3
00000F14  50                push ax
00000F15  FF76FE            push word [bp-0x2]
00000F18  FF76FC            push word [bp-0x4]
00000F1B  9A01009522        call 0x2295:0x1
00000F20  83C40A            add sp,byte +0xa
00000F23  B80400            mov ax,0x4
00000F26  50                push ax
00000F27  1E                push ds
00000F28  B8183B            mov ax,0x3b18
00000F2B  50                push ax
00000F2C  9AEA10031B        call 0x1b03:0x10ea
00000F31  83C406            add sp,byte +0x6
00000F34  A0AA37            mov al,[0x37aa]
00000F37  98                cbw
00000F38  D1E0              shl ax,1
00000F3A  8BD8              mov bx,ax
00000F3C  8B870E30          mov ax,[bx+0x300e]
00000F40  0346FC            add ax,[bp-0x4]
00000F43  50                push ax
00000F44  A0AA37            mov al,[0x37aa]
00000F47  98                cbw
00000F48  D1E0              shl ax,1
00000F4A  8BD8              mov bx,ax
00000F4C  58                pop ax
00000F4D  89870E30          mov [bx+0x300e],ax
00000F51  3DD007            cmp ax,0x7d0
00000F54  7E0E              jng 0xf64
00000F56  A0AA37            mov al,[0x37aa]
00000F59  98                cbw
00000F5A  D1E0              shl ax,1
00000F5C  8BD8              mov bx,ax
00000F5E  C7870E30D007      mov word [bx+0x300e],0x7d0
00000F64  A0AA37            mov al,[0x37aa]
00000F67  98                cbw
00000F68  8BD8              mov bx,ax
00000F6A  8A87423D          mov al,[bx+0x3d42]
00000F6E  B400              mov ah,0x0
00000F70  D1E0              shl ax,1
00000F72  D1E0              shl ax,1
00000F74  BA1600            mov dx,0x16
00000F77  2BD0              sub dx,ax
00000F79  52                push dx
00000F7A  9ABA01FD21        call 0x21fd:0x1ba
00000F7F  59                pop cx
00000F80  0BC0              or ax,ax
00000F82  7576              jnz 0xffa
00000F84  803EBA4501        cmp byte [0x45ba],0x1
00000F89  756F              jnz 0xffa
00000F8B  B81400            mov ax,0x14
00000F8E  50                push ax
00000F8F  9ABA01FD21        call 0x21fd:0x1ba
00000F94  59                pop cx
00000F95  40                inc ax
00000F96  40                inc ax
00000F97  8946FC            mov [bp-0x4],ax
00000F9A  C746FE0000        mov word [bp-0x2],0x0
00000F9F  FF36123B          push word [0x3b12]
00000FA3  FF36103B          push word [0x3b10]
00000FA7  B80300            mov ax,0x3
00000FAA  50                push ax
00000FAB  FF76FE            push word [bp-0x2]
00000FAE  FF76FC            push word [bp-0x4]
00000FB1  9A01009522        call 0x2295:0x1
00000FB6  83C40A            add sp,byte +0xa
00000FB9  B80400            mov ax,0x4
00000FBC  50                push ax
00000FBD  1E                push ds
00000FBE  B8083B            mov ax,0x3b08
00000FC1  50                push ax
00000FC2  9AEA10031B        call 0x1b03:0x10ea
00000FC7  83C406            add sp,byte +0x6
00000FCA  A0AA37            mov al,[0x37aa]
00000FCD  98                cbw
00000FCE  D1E0              shl ax,1
00000FD0  8BD8              mov bx,ax
00000FD2  8B872045          mov ax,[bx+0x4520]
00000FD6  0346FC            add ax,[bp-0x4]
00000FD9  50                push ax
00000FDA  A0AA37            mov al,[0x37aa]
00000FDD  98                cbw
00000FDE  D1E0              shl ax,1
00000FE0  8BD8              mov bx,ax
00000FE2  58                pop ax
00000FE3  89872045          mov [bx+0x4520],ax
00000FE7  3D2800            cmp ax,0x28
00000FEA  7E0E              jng 0xffa
00000FEC  A0AA37            mov al,[0x37aa]
00000FEF  98                cbw
00000FF0  D1E0              shl ax,1
00000FF2  8BD8              mov bx,ax
00000FF4  C78720452800      mov word [bx+0x4520],0x28
00000FFA  A0AA37            mov al,[0x37aa]
00000FFD  98                cbw
00000FFE  8BD8              mov bx,ax
00001000  8A87423D          mov al,[bx+0x3d42]
00001004  B400              mov ah,0x0
00001006  BA0600            mov dx,0x6
00001009  F7EA              imul dx
0000100B  BA3100            mov dx,0x31
0000100E  2BD0              sub dx,ax
00001010  52                push dx
00001011  9ABA01FD21        call 0x21fd:0x1ba
00001016  59                pop cx
00001017  0BC0              or ax,ax
00001019  7403              jz 0x101e
0000101B  E9BA00            jmp 0x10d8
0000101E  A0AA37            mov al,[0x37aa]
00001021  98                cbw
00001022  8BD8              mov bx,ax
00001024  80BF423D00        cmp byte [bx+0x3d42],0x0
00001029  7703              ja 0x102e
0000102B  E9AA00            jmp 0x10d8
0000102E  A0AA37            mov al,[0x37aa]
00001031  98                cbw
00001032  D1E0              shl ax,1
00001034  D1E0              shl ax,1
00001036  8BD8              mov bx,ax
00001038  83BFD43C00        cmp word [bx+0x3cd4],byte +0x0
0000103D  7F21              jg 0x1060
0000103F  7C08              jl 0x1049
00001041  81BFD23CE803      cmp word [bx+0x3cd2],0x3e8
00001047  7317              jnc 0x1060
00001049  B81E00            mov ax,0x1e
0000104C  50                push ax
0000104D  9ABA01FD21        call 0x21fd:0x1ba
00001052  59                pop cx
00001053  056400            add ax,0x64
00001056  8946FC            mov [bp-0x4],ax
00001059  C746FE0000        mov word [bp-0x2],0x0
0000105E  EB33              jmp short 0x1093
00001060  A0AA37            mov al,[0x37aa]
00001063  98                cbw
00001064  8BD8              mov bx,ax
00001066  8A87423D          mov al,[bx+0x3d42]
0000106A  B400              mov ah,0x0
0000106C  50                push ax
0000106D  B80A00            mov ax,0xa
00001070  5A                pop dx
00001071  2BC2              sub ax,dx
00001073  99                cwd
00001074  52                push dx
00001075  50                push ax
00001076  A0AA37            mov al,[0x37aa]
00001079  98                cbw
0000107A  D1E0              shl ax,1
0000107C  D1E0              shl ax,1
0000107E  8BD8              mov bx,ax
00001080  FFB7D43C          push word [bx+0x3cd4]
00001084  FFB7D23C          push word [bx+0x3cd2]
00001088  9A894F0000        call 0x0:0x4f89
0000108D  8946FC            mov [bp-0x4],ax
00001090  8956FE            mov [bp-0x2],dx
00001093  A14C3B            mov ax,[0x3b4c]
00001096  40                inc ax
00001097  40                inc ax
00001098  FF364E3B          push word [0x3b4e]
0000109C  50                push ax
0000109D  B80500            mov ax,0x5
000010A0  50                push ax
000010A1  FF76FE            push word [bp-0x2]
000010A4  FF76FC            push word [bp-0x4]
000010A7  9A01009522        call 0x2295:0x1
000010AC  83C40A            add sp,byte +0xa
000010AF  A0AA37            mov al,[0x37aa]
000010B2  98                cbw
000010B3  D1E0              shl ax,1
000010B5  D1E0              shl ax,1
000010B7  8B56FE            mov dx,[bp-0x2]
000010BA  8B5EFC            mov bx,[bp-0x4]
000010BD  87D8              xchg bx,ax
000010BF  0187D23C          add [bx+0x3cd2],ax
000010C3  1197D43C          adc [bx+0x3cd4],dx
000010C7  B80B00            mov ax,0xb
000010CA  50                push ax
000010CB  1E                push ds
000010CC  B8283B            mov ax,0x3b28
000010CF  50                push ax
000010D0  9AEA10031B        call 0x1b03:0x10ea
000010D5  83C406            add sp,byte +0x6
000010D8  FE06AA37          inc byte [0x37aa]
000010DC  803EAA3704        cmp byte [0x37aa],0x4
000010E1  7D03              jnl 0x10e6
000010E3  E9D1FD            jmp 0xeb7
000010E6  8BE5              mov sp,bp
000010E8  5D                pop bp
000010E9  CB                retf

; ---- 1B03:10EA ----
000010EA  55                push bp
000010EB  8BEC              mov bp,sp
000010ED  56                push si
000010EE  57                push di
000010EF  33FF              xor di,di
000010F1  33F6              xor si,si
000010F3  B8FFFF            mov ax,0xffff
000010F6  50                push ax
000010F7  B80500            mov ax,0x5
000010FA  50                push ax
000010FB  A0AA37            mov al,[0x37aa]
000010FE  98                cbw
000010FF  D1E0              shl ax,1
00001101  8BD8              mov bx,ax
00001103  FFB7000D          push word [bx+0xd00]
00001107  8BC7              mov ax,di
00001109  D1E0              shl ax,1
0000110B  D1E0              shl ax,1
0000110D  C45E06            les bx,[bp+0x6]
00001110  03D8              add bx,ax
00001112  26FF7702          push word [es:bx+0x2]
00001116  26FF37            push word [es:bx]
00001119  B85600            mov ax,0x56
0000111C  50                push ax
0000111D  8BC6              mov ax,si
0000111F  BA0600            mov dx,0x6
00001122  F7EA              imul dx
00001124  05B400            add ax,0xb4
00001127  50                push ax
00001128  B87100            mov ax,0x71
0000112B  50                push ax
0000112C  9A4302690A        call 0xa69:0x243
00001131  83C410            add sp,byte +0x10
00001134  8BC7              mov ax,di
00001136  40                inc ax
00001137  8BF8              mov di,ax
00001139  3B460A            cmp ax,[bp+0xa]
0000113C  7C02              jl 0x1140
0000113E  EB21              jmp short 0x1161
00001140  8BC6              mov ax,si
00001142  40                inc ax
00001143  8BF0              mov si,ax
00001145  3D0400            cmp ax,0x4
00001148  7C15              jl 0x115f
0000114A  33C0              xor ax,ax
0000114C  BA1400            mov dx,0x14
0000114F  50                push ax
00001150  52                push dx
00001151  9A8501FD21        call 0x21fd:0x185
00001156  59                pop cx
00001157  59                pop cx
00001158  9AEE13031B        call 0x1b03:0x13ee
0000115D  33F6              xor si,si
0000115F  EB92              jmp short 0x10f3
00001161  33C0              xor ax,ax
00001163  BA3200            mov dx,0x32
00001166  50                push ax
00001167  52                push dx
00001168  9A8501FD21        call 0x21fd:0x185
0000116D  59                pop cx
0000116E  59                pop cx
0000116F  9AEE13031B        call 0x1b03:0x13ee
00001174  5F                pop di
00001175  5E                pop si
00001176  5D                pop bp
00001177  CB                retf

; ---- 1B03:1178 ----
00001178  55                push bp
00001179  8BEC              mov bp,sp
0000117B  4C                dec sp
0000117C  4C                dec sp
0000117D  56                push si
0000117E  57                push di
0000117F  9A1B00960A        call 0xa96:0x1b
00001184  33C0              xor ax,ax
00001186  50                push ax
00001187  33C0              xor ax,ax
00001189  50                push ax
0000118A  B8C800            mov ax,0xc8
0000118D  50                push ax
0000118E  B84001            mov ax,0x140
00001191  50                push ax
00001192  33C0              xor ax,ax
00001194  50                push ax
00001195  33C0              xor ax,ax
00001197  50                push ax
00001198  9A3000960A        call 0xa96:0x30
0000119D  83C40C            add sp,byte +0xc
000011A0  9AA806960A        call 0xa96:0x6a8
000011A5  33F6              xor si,si
000011A7  EB53              jmp short 0x11fc
000011A9  B83F01            mov ax,0x13f
000011AC  50                push ax
000011AD  9ABA01FD21        call 0x21fd:0x1ba
000011B2  59                pop cx
000011B3  8946FE            mov [bp-0x2],ax
000011B6  B8C700            mov ax,0xc7
000011B9  50                push ax
000011BA  9ABA01FD21        call 0x21fd:0x1ba
000011BF  59                pop cx
000011C0  8BF8              mov di,ax
000011C2  9A1B00960A        call 0xa96:0x1b
000011C7  57                push di
000011C8  FF76FE            push word [bp-0x2]
000011CB  9ADE06CD23        call 0x23cd:0x6de
000011D0  59                pop cx
000011D1  59                pop cx
000011D2  3D0100            cmp ax,0x1
000011D5  751F              jnz 0x11f6
000011D7  B80200            mov ax,0x2
000011DA  50                push ax
000011DB  9ABA01FD21        call 0x21fd:0x1ba
000011E0  59                pop cx
000011E1  8BD8              mov bx,ax
000011E3  8A87D90D          mov al,[bx+0xdd9]
000011E7  B400              mov ah,0x0
000011E9  50                push ax
000011EA  57                push di
000011EB  FF76FE            push word [bp-0x2]
000011EE  9AFB03960A        call 0xa96:0x3fb
000011F3  83C406            add sp,byte +0x6
000011F6  9AA806960A        call 0xa96:0x6a8
000011FB  46                inc si
000011FC  81FEC700          cmp si,0xc7
00001200  7EA7              jng 0x11a9
00001202  9A1B00960A        call 0xa96:0x1b
00001207  33F6              xor si,si
00001209  EB32              jmp short 0x123d
0000120B  8A849C0C          mov al,[si+0xc9c]
0000120F  B400              mov ah,0x0
00001211  50                push ax
00001212  8BDE              mov bx,si
00001214  D1E3              shl bx,1
00001216  FFB7840C          push word [bx+0xc84]
0000121A  B80B00            mov ax,0xb
0000121D  50                push ax
0000121E  B81000            mov ax,0x10
00001221  50                push ax
00001222  B8C800            mov ax,0xc8
00001225  50                push ax
00001226  8A84CD0D          mov al,[si+0xdcd]
0000122A  B400              mov ah,0x0
0000122C  B104              mov cl,0x4
0000122E  D3E0              shl ax,cl
00001230  050001            add ax,0x100
00001233  50                push ax
00001234  9A3000960A        call 0xa96:0x30
00001239  83C40C            add sp,byte +0xc
0000123C  46                inc si
0000123D  83FE0B            cmp si,byte +0xb
00001240  7EC9              jng 0x120b
00001242  9AA806960A        call 0xa96:0x6a8
00001247  FF362E08          push word [0x82e]
0000124B  9A3F0C960A        call 0xa96:0xc3f
00001250  59                pop cx
00001251  5F                pop di
00001252  5E                pop si
00001253  8BE5              mov sp,bp
00001255  5D                pop bp
00001256  CB                retf

; ---- 1B03:1257 ----
00001257  55                push bp
00001258  8BEC              mov bp,sp
0000125A  B82000            mov ax,0x20
0000125D  50                push ax
0000125E  B80500            mov ax,0x5
00001261  50                push ax
00001262  B83500            mov ax,0x35
00001265  50                push ax
00001266  B84000            mov ax,0x40
00001269  50                push ax
0000126A  B8C800            mov ax,0xc8
0000126D  50                push ax
0000126E  33C0              xor ax,ax
00001270  50                push ax
00001271  9A3000960A        call 0xa96:0x30
00001276  83C40C            add sp,byte +0xc
00001279  B000              mov al,0x0
0000127B  50                push ax
0000127C  9A8D12031B        call 0x1b03:0x128d
00001281  59                pop cx
00001282  33C0              xor ax,ax
00001284  50                push ax
00001285  9A4513031B        call 0x1b03:0x1345
0000128A  59                pop cx
0000128B  5D                pop bp
0000128C  CB                retf

; ---- 1B03:128D ----
0000128D  55                push bp
0000128E  8BEC              mov bp,sp
00001290  B81A00            mov ax,0x1a
00001293  50                push ax
00001294  A06E54            mov al,[0x546e]
00001297  98                cbw
00001298  8BD8              mov bx,ax
0000129A  8A87E40D          mov al,[bx+0xde4]
0000129E  B400              mov ah,0x0
000012A0  50                push ax
000012A1  B81900            mov ax,0x19
000012A4  50                push ax
000012A5  A06E54            mov al,[0x546e]
000012A8  98                cbw
000012A9  8BD8              mov bx,ax
000012AB  8A87E10D          mov al,[bx+0xde1]
000012AF  B400              mov ah,0x0
000012B1  50                push ax
000012B2  8A4606            mov al,[bp+0x6]
000012B5  B400              mov ah,0x0
000012B7  8BD8              mov bx,ax
000012B9  8A87DF0D          mov al,[bx+0xddf]
000012BD  B400              mov ah,0x0
000012BF  50                push ax
000012C0  A06E54            mov al,[0x546e]
000012C3  98                cbw
000012C4  8BD8              mov bx,ax
000012C6  8A87DC0D          mov al,[bx+0xddc]
000012CA  B400              mov ah,0x0
000012CC  50                push ax
000012CD  9A3000960A        call 0xa96:0x30
000012D2  83C40C            add sp,byte +0xc
000012D5  5D                pop bp
000012D6  CB                retf

; ---- 1B03:12D7 ----
000012D7  55                push bp
000012D8  8BEC              mov bp,sp
000012DA  803E6E54FF        cmp byte [0x546e],0xff
000012DF  7407              jz 0x12e8
000012E1  803EB15301        cmp byte [0x53b1],0x1
000012E6  7502              jnz 0x12ea
000012E8  EB2F              jmp short 0x1319
000012EA  A1AA53            mov ax,[0x53aa]
000012ED  40                inc ax
000012EE  A3AA53            mov [0x53aa],ax
000012F1  3B4606            cmp ax,[bp+0x6]
000012F4  7C23              jl 0x1319
000012F6  C706AA530000      mov word [0x53aa],0x0
000012FC  A03753            mov al,[0x5337]
000012FF  FEC0              inc al
00001301  A23753            mov [0x5337],al
00001304  3C03              cmp al,0x3
00001306  7605              jna 0x130d
00001308  C606375300        mov byte [0x5337],0x0
0000130D  A03753            mov al,[0x5337]
00001310  B400              mov ah,0x0
00001312  50                push ax
00001313  9A1B13031B        call 0x1b03:0x131b
00001318  59                pop cx
00001319  5D                pop bp
0000131A  CB                retf

; ---- 1B03:131B ----
0000131B  55                push bp
0000131C  8BEC              mov bp,sp
0000131E  B82400            mov ax,0x24
00001321  50                push ax
00001322  B80800            mov ax,0x8
00001325  50                push ax
00001326  B81000            mov ax,0x10
00001329  50                push ax
0000132A  B8E800            mov ax,0xe8
0000132D  50                push ax
0000132E  8B4606            mov ax,[bp+0x6]
00001331  D1E0              shl ax,1
00001333  D1E0              shl ax,1
00001335  D1E0              shl ax,1
00001337  05D000            add ax,0xd0
0000133A  50                push ax
0000133B  9A1103960A        call 0xa96:0x311
00001340  83C40A            add sp,byte +0xa
00001343  5D                pop bp
00001344  CB                retf

; ---- 1B03:1345 ----
00001345  55                push bp
00001346  8BEC              mov bp,sp
00001348  837E0600          cmp word [bp+0x6],byte +0x0
0000134C  7521              jnz 0x136f
0000134E  B83300            mov ax,0x33
00001351  50                push ax
00001352  B80500            mov ax,0x5
00001355  50                push ax
00001356  B81000            mov ax,0x10
00001359  50                push ax
0000135A  B82000            mov ax,0x20
0000135D  50                push ax
0000135E  B8DB00            mov ax,0xdb
00001361  50                push ax
00001362  33C0              xor ax,ax
00001364  50                push ax
00001365  9A3000960A        call 0xa96:0x30
0000136A  83C40C            add sp,byte +0xc
0000136D  EB20              jmp short 0x138f
0000136F  B83300            mov ax,0x33
00001372  50                push ax
00001373  B80500            mov ax,0x5
00001376  50                push ax
00001377  B81000            mov ax,0x10
0000137A  50                push ax
0000137B  B82000            mov ax,0x20
0000137E  50                push ax
0000137F  B8C800            mov ax,0xc8
00001382  50                push ax
00001383  B8B000            mov ax,0xb0
00001386  50                push ax
00001387  9A3000960A        call 0xa96:0x30
0000138C  83C40C            add sp,byte +0xc
0000138F  8A4606            mov al,[bp+0x6]
00001392  A2B153            mov [0x53b1],al
00001395  C706AA53E803      mov word [0x53aa],0x3e8
0000139B  5D                pop bp
0000139C  CB                retf

; ---- 1B03:139D ----
0000139D  55                push bp
0000139E  8BEC              mov bp,sp
000013A0  B83A00            mov ax,0x3a
000013A3  50                push ax
000013A4  B82F00            mov ax,0x2f
000013A7  50                push ax
000013A8  B81000            mov ax,0x10
000013AB  50                push ax
000013AC  B82000            mov ax,0x20
000013AF  50                push ax
000013B0  8B4606            mov ax,[bp+0x6]
000013B3  B104              mov cl,0x4
000013B5  D3E0              shl ax,cl
000013B7  05D800            add ax,0xd8
000013BA  50                push ax
000013BB  B8B000            mov ax,0xb0
000013BE  50                push ax
000013BF  9A3000960A        call 0xa96:0x30
000013C4  83C40C            add sp,byte +0xc
000013C7  5D                pop bp
000013C8  CB                retf

; ---- 1B03:13C9 ----
000013C9  55                push bp
000013CA  8BEC              mov bp,sp
000013CC  B83300            mov ax,0x33
000013CF  50                push ax
000013D0  B81B00            mov ax,0x1b
000013D3  50                push ax
000013D4  B82000            mov ax,0x20
000013D7  50                push ax
000013D8  B83000            mov ax,0x30
000013DB  50                push ax
000013DC  B8C800            mov ax,0xc8
000013DF  50                push ax
000013E0  B8D000            mov ax,0xd0
000013E3  50                push ax
000013E4  9A3000960A        call 0xa96:0x30
000013E9  83C40C            add sp,byte +0xc
000013EC  5D                pop bp
000013ED  CB                retf

; ---- 1B03:13EE ----
000013EE  55                push bp
000013EF  8BEC              mov bp,sp
000013F1  9A1B00960A        call 0xa96:0x1b
000013F6  B8AF00            mov ax,0xaf
000013F9  50                push ax
000013FA  B87000            mov ax,0x70
000013FD  50                push ax
000013FE  B81900            mov ax,0x19
00001401  50                push ax
00001402  B86000            mov ax,0x60
00001405  50                push ax
00001406  B8AF00            mov ax,0xaf
00001409  50                push ax
0000140A  B87000            mov ax,0x70
0000140D  50                push ax
0000140E  9A3000960A        call 0xa96:0x30
00001413  83C40C            add sp,byte +0xc
00001416  9A0D06FD21        call 0x21fd:0x60d
0000141B  5D                pop bp
0000141C  CB                retf

; ---- 1B03:141D ----
0000141D  55                push bp
0000141E  8BEC              mov bp,sp
00001420  A0AA37            mov al,[0x37aa]
00001423  98                cbw
00001424  BA0600            mov dx,0x6
00001427  F7EA              imul dx
00001429  05AF00            add ax,0xaf
0000142C  50                push ax
0000142D  B87000            mov ax,0x70
00001430  50                push ax
00001431  B80600            mov ax,0x6
00001434  50                push ax
00001435  B86000            mov ax,0x60
00001438  50                push ax
00001439  A0AA37            mov al,[0x37aa]
0000143C  98                cbw
0000143D  BA0600            mov dx,0x6
00001440  F7EA              imul dx
00001442  05AF00            add ax,0xaf
00001445  50                push ax
00001446  B87000            mov ax,0x70
00001449  50                push ax
0000144A  9A3000960A        call 0xa96:0x30
0000144F  83C40C            add sp,byte +0xc
00001452  5D                pop bp
00001453  CB                retf

; ---- 1B03:1454 ----
00001454  55                push bp
00001455  8BEC              mov bp,sp
00001457  A0AA37            mov al,[0x37aa]
0000145A  98                cbw
0000145B  BA0600            mov dx,0x6
0000145E  F7EA              imul dx
00001460  05AF00            add ax,0xaf
00001463  50                push ax
00001464  B8B000            mov ax,0xb0
00001467  50                push ax
00001468  B80600            mov ax,0x6
0000146B  50                push ax
0000146C  B81000            mov ax,0x10
0000146F  50                push ax
00001470  A0AA37            mov al,[0x37aa]
00001473  98                cbw
00001474  BA0600            mov dx,0x6
00001477  F7EA              imul dx
00001479  05AF00            add ax,0xaf
0000147C  50                push ax
0000147D  B8B000            mov ax,0xb0
00001480  50                push ax
00001481  9A3000960A        call 0xa96:0x30
00001486  83C40C            add sp,byte +0xc
00001489  5D                pop bp
0000148A  CB                retf

; ---- 1B03:148B ----
0000148B  55                push bp
0000148C  8BEC              mov bp,sp
0000148E  56                push si
0000148F  C70676330D00      mov word [0x3376],0xd
00001495  B88500            mov ax,0x85
00001498  50                push ax
00001499  33C0              xor ax,ax
0000149B  50                push ax
0000149C  B87800            mov ax,0x78
0000149F  50                push ax
000014A0  33C0              xor ax,ax
000014A2  50                push ax
000014A3  B80400            mov ax,0x4
000014A6  50                push ax
000014A7  9A2B02AF21        call 0x21af:0x22b
000014AC  83C40A            add sp,byte +0xa
000014AF  33F6              xor si,si
000014B1  EB2A              jmp short 0x14dd
000014B3  B82800            mov ax,0x28
000014B6  50                push ax
000014B7  8BC6              mov ax,si
000014B9  B104              mov cl,0x4
000014BB  D3E0              shl ax,cl
000014BD  055000            add ax,0x50
000014C0  50                push ax
000014C1  B8D300            mov ax,0xd3
000014C4  50                push ax
000014C5  8BC6              mov ax,si
000014C7  B104              mov cl,0x4
000014C9  D3E0              shl ax,cl
000014CB  050001            add ax,0x100
000014CE  50                push ax
000014CF  56                push si
000014D0  B80400            mov ax,0x4
000014D3  50                push ax
000014D4  9ACA00AF21        call 0x21af:0xca
000014D9  83C40C            add sp,byte +0xc
000014DC  46                inc si
000014DD  83FE03            cmp si,byte +0x3
000014E0  7CD1              jl 0x14b3
000014E2  33F6              xor si,si
000014E4  EB33              jmp short 0x1519
000014E6  B82800            mov ax,0x28
000014E9  50                push ax
000014EA  8BC6              mov ax,si
000014EC  B104              mov cl,0x4
000014EE  D3E0              shl ax,cl
000014F0  BA6000            mov dx,0x60
000014F3  2BD0              sub dx,ax
000014F5  52                push dx
000014F6  B8D300            mov ax,0xd3
000014F9  50                push ax
000014FA  8BC6              mov ax,si
000014FC  B104              mov cl,0x4
000014FE  D3E0              shl ax,cl
00001500  BA1001            mov dx,0x110
00001503  2BD0              sub dx,ax
00001505  52                push dx
00001506  8BC6              mov ax,si
00001508  050300            add ax,0x3
0000150B  50                push ax
0000150C  B80400            mov ax,0x4
0000150F  50                push ax
00001510  9ACA00AF21        call 0x21af:0xca
00001515  83C40C            add sp,byte +0xc
00001518  46                inc si
00001519  83FE02            cmp si,byte +0x2
0000151C  7CC8              jl 0x14e6
0000151E  C606AA3700        mov byte [0x37aa],0x0
00001523  E9A400            jmp 0x15ca
00001526  A0AA37            mov al,[0x37aa]
00001529  98                cbw
0000152A  BA1A00            mov dx,0x1a
0000152D  F7EA              imul dx
0000152F  8BD8              mov bx,ax
00001531  C7870E330F00      mov word [bx+0x330e],0xf
00001537  A0AA37            mov al,[0x37aa]
0000153A  98                cbw
0000153B  BA0F00            mov dx,0xf
0000153E  F7EA              imul dx
00001540  053C00            add ax,0x3c
00001543  50                push ax
00001544  33C0              xor ax,ax
00001546  50                push ax
00001547  A0AA37            mov al,[0x37aa]
0000154A  98                cbw
0000154B  BA0F00            mov dx,0xf
0000154E  F7EA              imul dx
00001550  50                push ax
00001551  33C0              xor ax,ax
00001553  50                push ax
00001554  A0AA37            mov al,[0x37aa]
00001557  98                cbw
00001558  50                push ax
00001559  9A2B02AF21        call 0x21af:0x22b
0000155E  83C40A            add sp,byte +0xa
00001561  B82800            mov ax,0x28
00001564  50                push ax
00001565  A0AA37            mov al,[0x37aa]
00001568  98                cbw
00001569  B104              mov cl,0x4
0000156B  D3E0              shl ax,cl
0000156D  50                push ax
0000156E  B8E200            mov ax,0xe2
00001571  50                push ax
00001572  A0AA37            mov al,[0x37aa]
00001575  98                cbw
00001576  B104              mov cl,0x4
00001578  D3E0              shl ax,cl
0000157A  050001            add ax,0x100
0000157D  50                push ax
0000157E  33C0              xor ax,ax
00001580  50                push ax
00001581  A0AA37            mov al,[0x37aa]
00001584  98                cbw
00001585  50                push ax
00001586  9ACA00AF21        call 0x21af:0xca
0000158B  83C40C            add sp,byte +0xc
0000158E  B82800            mov ax,0x28
00001591  50                push ax
00001592  B84000            mov ax,0x40
00001595  50                push ax
00001596  B8F100            mov ax,0xf1
00001599  50                push ax
0000159A  A0AA37            mov al,[0x37aa]
0000159D  98                cbw
0000159E  B104              mov cl,0x4
000015A0  D3E0              shl ax,cl
000015A2  050001            add ax,0x100
000015A5  50                push ax
000015A6  B80100            mov ax,0x1
000015A9  50                push ax
000015AA  A0AA37            mov al,[0x37aa]
000015AD  98                cbw
000015AE  50                push ax
000015AF  9ACA00AF21        call 0x21af:0xca
000015B4  83C40C            add sp,byte +0xc
000015B7  33C0              xor ax,ax
000015B9  50                push ax
000015BA  A0AA37            mov al,[0x37aa]
000015BD  98                cbw
000015BE  50                push ax
000015BF  9A1204AF21        call 0x21af:0x412
000015C4  59                pop cx
000015C5  59                pop cx
000015C6  FE06AA37          inc byte [0x37aa]
000015CA  803EAA3704        cmp byte [0x37aa],0x4
000015CF  7D03              jnl 0x15d4
000015D1  E952FF            jmp 0x1526
000015D4  5E                pop si
000015D5  5D                pop bp
000015D6  CB                retf

; ---- 1B03:15D7 ----
000015D7  55                push bp
000015D8  8BEC              mov bp,sp
000015DA  C606AA3700        mov byte [0x37aa],0x0
000015DF  EB0F              jmp short 0x15f0
000015E1  A0AA37            mov al,[0x37aa]
000015E4  98                cbw
000015E5  50                push ax
000015E6  9A5E04AF21        call 0x21af:0x45e
000015EB  59                pop cx
000015EC  FE06AA37          inc byte [0x37aa]
000015F0  803EAA3704        cmp byte [0x37aa],0x4
000015F5  7CEA              jl 0x15e1
000015F7  FF362E08          push word [0x82e]
000015FB  9A1C07CD23        call 0x23cd:0x71c
00001600  59                pop cx
00001601  9AD401670B        call 0xb67:0x1d4
00001606  FF363008          push word [0x830]
0000160A  9A1C07CD23        call 0x23cd:0x71c
0000160F  59                pop cx
00001610  9A0602670B        call 0xb67:0x206
00001615  5D                pop bp
00001616  CB                retf

; ---- 1B03:1617 ----
00001617  55                push bp
00001618  8BEC              mov bp,sp
0000161A  56                push si
0000161B  A0AA37            mov al,[0x37aa]
0000161E  98                cbw
0000161F  D1E0              shl ax,1
00001621  8BD8              mov bx,ax
00001623  8BB78848          mov si,[bx+0x4888]
00001627  8BDE              mov bx,si
00001629  D1E3              shl bx,1
0000162B  8B87840C          mov ax,[bx+0xc84]
0000162F  50                push ax
00001630  A0AA37            mov al,[0x37aa]
00001633  98                cbw
00001634  BA1A00            mov dx,0x1a
00001637  F7EA              imul dx
00001639  8BD8              mov bx,ax
0000163B  58                pop ax
0000163C  89872033          mov [bx+0x3320],ax
00001640  A0AA37            mov al,[0x37aa]
00001643  98                cbw
00001644  BA1A00            mov dx,0x1a
00001647  F7EA              imul dx
00001649  8A949C0C          mov dl,[si+0xc9c]
0000164D  B600              mov dh,0x0
0000164F  83                db 0x83
