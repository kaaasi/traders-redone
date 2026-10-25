; segment 192D  file offset 0x1C8D0  size 0x1D60
00000000  9A0D06FD21        call 0x21fd:0x60d
00000005  5D                pop bp
00000006  CB                retf

; ---- 192D:0007 ----
00000007  55                push bp
00000008  8BEC              mov bp,sp
0000000A  FF36DA08          push word [0x8da]
0000000E  9A02006C18        call 0x186c:0x2
00000013  59                pop cx
00000014  9AE3032D19        call 0x192d:0x3e3
00000019  9A92002D19        call 0x192d:0x92
0000001E  833EEA08FF        cmp word [0x8ea],byte -0x1
00000023  7405              jz 0x2a
00000025  9A92052D19        call 0x192d:0x592
0000002A  9AB4002D19        call 0x192d:0xb4
0000002F  0AC0              or al,al
00000031  7504              jnz 0x37
00000033  B000              mov al,0x0
00000035  EB1F              jmp short 0x56
00000037  9A7F192D19        call 0x192d:0x197f
0000003C  9AA6122D19        call 0x192d:0x12a6
00000041  803ECC3C01        cmp byte [0x3ccc],0x1
00000046  7505              jnz 0x4d
00000048  9AA3002D19        call 0x192d:0xa3
0000004D  9A58002D19        call 0x192d:0x58
00000052  B001              mov al,0x1
00000054  EB00              jmp short 0x56
00000056  5D                pop bp
00000057  CB                retf

; ---- 192D:0058 ----
00000058  55                push bp
00000059  8BEC              mov bp,sp
0000005B  A1FC08            mov ax,[0x8fc]
0000005E  B105              mov cl,0x5
00000060  D3E0              shl ax,cl
00000062  50                push ax
00000063  B84001            mov ax,0x140
00000066  5A                pop dx
00000067  2BC2              sub ax,dx
00000069  BB0200            mov bx,0x2
0000006C  99                cwd
0000006D  F7FB              idiv bx
0000006F  A31E45            mov [0x451e],ax
00000072  A1FE08            mov ax,[0x8fe]
00000075  BA1600            mov dx,0x16
00000078  F7EA              imul dx
0000007A  50                push ax
0000007B  B89200            mov ax,0x92
0000007E  5A                pop dx
0000007F  2BC2              sub ax,dx
00000081  05F2FF            add ax,0xfff2
00000084  BB0200            mov bx,0x2
00000087  99                cwd
00000088  F7FB              idiv bx
0000008A  053A00            add ax,0x3a
0000008D  A32845            mov [0x4528],ax
00000090  5D                pop bp
00000091  CB                retf

; ---- 192D:0092 ----
00000092  55                push bp
00000093  8BEC              mov bp,sp
00000095  A1FC08            mov ax,[0x8fc]
00000098  A33053            mov [0x5330],ax
0000009B  A1FE08            mov ax,[0x8fe]
0000009E  A33253            mov [0x5332],ax
000000A1  5D                pop bp
000000A2  CB                retf

; ---- 192D:00A3 ----
000000A3  55                push bp
000000A4  8BEC              mov bp,sp
000000A6  A13053            mov ax,[0x5330]
000000A9  A3FC08            mov [0x8fc],ax
000000AC  A13253            mov ax,[0x5332]
000000AF  A3FE08            mov [0x8fe],ax
000000B2  5D                pop bp
000000B3  CB                retf

; ---- 192D:00B4 ----
000000B4  55                push bp
000000B5  8BEC              mov bp,sp
000000B7  9AC5002D19        call 0x192d:0xc5
000000BC  9A34022D19        call 0x192d:0x234
000000C1  EB00              jmp short 0xc3
000000C3  5D                pop bp
000000C4  CB                retf

; ---- 192D:00C5 ----
000000C5  55                push bp
000000C6  8BEC              mov bp,sp
000000C8  83EC04            sub sp,byte +0x4
000000CB  B80300            mov ax,0x3
000000CE  50                push ax
000000CF  1E                push ds
000000D0  B89439            mov ax,0x3994
000000D3  50                push ax
000000D4  B86400            mov ax,0x64
000000D7  50                push ax
000000D8  B8BB00            mov ax,0xbb
000000DB  50                push ax
000000DC  9AAF162D19        call 0x192d:0x16af
000000E1  83C40A            add sp,byte +0xa
000000E4  9A1B00960A        call 0xa96:0x1b
000000E9  A13453            mov ax,[0x5334]
000000EC  050600            add ax,0x6
000000EF  50                push ax
000000F0  B84A00            mov ax,0x4a
000000F3  50                push ax
000000F4  B81C00            mov ax,0x1c
000000F7  50                push ax
000000F8  B8AB00            mov ax,0xab
000000FB  50                push ax
000000FC  B82800            mov ax,0x28
000000FF  50                push ax
00000100  B84700            mov ax,0x47
00000103  50                push ax
00000104  9A3000960A        call 0xa96:0x30
00000109  83C40C            add sp,byte +0xc
0000010C  C746FC0000        mov word [bp-0x4],0x0
00000111  C746FE0000        mov word [bp-0x2],0x0
00000116  E99B00            jmp 0x1b4
00000119  B80100            mov ax,0x1
0000011C  50                push ax
0000011D  B8FFFF            mov ax,0xffff
00000120  50                push ax
00000121  B80300            mov ax,0x3
00000124  50                push ax
00000125  33C0              xor ax,ax
00000127  50                push ax
00000128  8B5EFC            mov bx,[bp-0x4]
0000012B  D1E3              shl bx,1
0000012D  D1E3              shl bx,1
0000012F  D1E3              shl bx,1
00000131  FFB7720B          push word [bx+0xb72]
00000135  FFB7700B          push word [bx+0xb70]
00000139  B84001            mov ax,0x140
0000013C  50                push ax
0000013D  8B4EFE            mov cx,[bp-0x2]
00000140  8B5EFC            mov bx,[bp-0x4]
00000143  33D2              xor dx,dx
00000145  B80E00            mov ax,0xe
00000148  9A4E4F0000        call 0x0:0x4f4e
0000014D  8B163453          mov dx,[0x5334]
00000151  03D0              add dx,ax
00000153  83C22A            add dx,byte +0x2a
00000156  52                push dx
00000157  33C0              xor ax,ax
00000159  50                push ax
0000015A  9AD300690A        call 0xa69:0xd3
0000015F  83C412            add sp,byte +0x12
00000162  B80100            mov ax,0x1
00000165  50                push ax
00000166  B8FFFF            mov ax,0xffff
00000169  50                push ax
0000016A  B80300            mov ax,0x3
0000016D  50                push ax
0000016E  B80E00            mov ax,0xe
00000171  50                push ax
00000172  8B5EFC            mov bx,[bp-0x4]
00000175  D1E3              shl bx,1
00000177  D1E3              shl bx,1
00000179  D1E3              shl bx,1
0000017B  FFB7760B          push word [bx+0xb76]
0000017F  FFB7740B          push word [bx+0xb74]
00000183  B84001            mov ax,0x140
00000186  50                push ax
00000187  8B4EFE            mov cx,[bp-0x2]
0000018A  8B5EFC            mov bx,[bp-0x4]
0000018D  33D2              xor dx,dx
0000018F  B80E00            mov ax,0xe
00000192  9A4E4F0000        call 0x0:0x4f4e
00000197  8B163453          mov dx,[0x5334]
0000019B  03D0              add dx,ax
0000019D  83C231            add dx,byte +0x31
000001A0  52                push dx
000001A1  33C0              xor ax,ax
000001A3  50                push ax
000001A4  9AD300690A        call 0xa69:0xd3
000001A9  83C412            add sp,byte +0x12
000001AC  8346FC01          add word [bp-0x4],byte +0x1
000001B0  8356FE00          adc word [bp-0x2],byte +0x0
000001B4  837EFE00          cmp word [bp-0x2],byte +0x0
000001B8  7D03              jnl 0x1bd
000001BA  E95CFF            jmp 0x119
000001BD  7509              jnz 0x1c8
000001BF  837EFC04          cmp word [bp-0x4],byte +0x4
000001C3  7303              jnc 0x1c8
000001C5  E951FF            jmp 0x119
000001C8  C606AA3700        mov byte [0x37aa],0x0
000001CD  EB0F              jmp short 0x1de
000001CF  A0AA37            mov al,[0x37aa]
000001D2  98                cbw
000001D3  50                push ax
000001D4  9AAB032D19        call 0x192d:0x3ab
000001D9  59                pop cx
000001DA  FE06AA37          inc byte [0x37aa]
000001DE  803EAA3704        cmp byte [0x37aa],0x4
000001E3  7CEA              jl 0x1cf
000001E5  B80100            mov ax,0x1
000001E8  50                push ax
000001E9  B8FFFF            mov ax,0xffff
000001EC  50                push ax
000001ED  B80300            mov ax,0x3
000001F0  50                push ax
000001F1  33C0              xor ax,ax
000001F3  50                push ax
000001F4  FF36823C          push word [0x3c82]
000001F8  FF36803C          push word [0x3c80]
000001FC  A13453            mov ax,[0x5334]
000001FF  05AC00            add ax,0xac
00000202  50                push ax
00000203  B8BE00            mov ax,0xbe
00000206  50                push ax
00000207  9A4D00690A        call 0xa69:0x4d
0000020C  83C410            add sp,byte +0x10
0000020F  9AA806960A        call 0xa96:0x6a8
00000214  FF362E08          push word [0x82e]
00000218  9A3F0C960A        call 0xa96:0xc3f
0000021D  59                pop cx
0000021E  33C0              xor ax,ax
00000220  33D2              xor dx,dx
00000222  50                push ax
00000223  52                push dx
00000224  9A6C02FD21        call 0x21fd:0x26c
00000229  59                pop cx
0000022A  59                pop cx
0000022B  9A5303FD21        call 0x21fd:0x353
00000230  8BE5              mov sp,bp
00000232  5D                pop bp
00000233  CB                retf

; ---- 192D:0234 ----
00000234  55                push bp
00000235  8BEC              mov bp,sp
00000237  4C                dec sp
00000238  4C                dec sp
00000239  C646FF00          mov byte [bp-0x1],0x0
0000023D  9A1B00960A        call 0xa96:0x1b
00000242  E90601            jmp 0x34b
00000245  9A1F05FD21        call 0x21fd:0x51f
0000024A  9AA806960A        call 0xa96:0x6a8
0000024F  9A1B00960A        call 0xa96:0x1b
00000254  9A4502FD21        call 0x21fd:0x245
00000259  803E823D00        cmp byte [0x3d82],0x0
0000025E  7412              jz 0x272
00000260  33C0              xor ax,ax
00000262  50                push ax
00000263  9A76032D19        call 0x192d:0x376
00000268  59                pop cx
00000269  EB00              jmp short 0x26b
0000026B  803E823D00        cmp byte [0x3d82],0x0
00000270  75F9              jnz 0x26b
00000272  803E833D00        cmp byte [0x3d83],0x0
00000277  7413              jz 0x28c
00000279  B80100            mov ax,0x1
0000027C  50                push ax
0000027D  9A76032D19        call 0x192d:0x376
00000282  59                pop cx
00000283  EB00              jmp short 0x285
00000285  803E833D00        cmp byte [0x3d83],0x0
0000028A  75F9              jnz 0x285
0000028C  803E843D00        cmp byte [0x3d84],0x0
00000291  7413              jz 0x2a6
00000293  B80200            mov ax,0x2
00000296  50                push ax
00000297  9A76032D19        call 0x192d:0x376
0000029C  59                pop cx
0000029D  EB00              jmp short 0x29f
0000029F  803E843D00        cmp byte [0x3d84],0x0
000002A4  75F9              jnz 0x29f
000002A6  803E853D00        cmp byte [0x3d85],0x0
000002AB  7413              jz 0x2c0
000002AD  B80300            mov ax,0x3
000002B0  50                push ax
000002B1  9A76032D19        call 0x192d:0x376
000002B6  59                pop cx
000002B7  EB00              jmp short 0x2b9
000002B9  803E853D00        cmp byte [0x3d85],0x0
000002BE  75F9              jnz 0x2b9
000002C0  803E873D00        cmp byte [0x3d87],0x0
000002C5  740E              jz 0x2d5
000002C7  9A0E042D19        call 0x192d:0x40e
000002CC  EB00              jmp short 0x2ce
000002CE  803E873D00        cmp byte [0x3d87],0x0
000002D3  75F9              jnz 0x2ce
000002D5  803E883D00        cmp byte [0x3d88],0x0
000002DA  740E              jz 0x2ea
000002DC  9A050A2D19        call 0x192d:0xa05
000002E1  EB00              jmp short 0x2e3
000002E3  803E883D00        cmp byte [0x3d88],0x0
000002E8  75F9              jnz 0x2e3
000002EA  803E893D00        cmp byte [0x3d89],0x0
000002EF  740E              jz 0x2ff
000002F1  9A960F2D19        call 0x192d:0xf96
000002F6  EB00              jmp short 0x2f8
000002F8  803E893D00        cmp byte [0x3d89],0x0
000002FD  75F9              jnz 0x2f8
000002FF  803E8A3D00        cmp byte [0x3d8a],0x0
00000304  740E              jz 0x314
00000306  9A63122D19        call 0x192d:0x1263
0000030B  EB00              jmp short 0x30d
0000030D  803E8A3D00        cmp byte [0x3d8a],0x0
00000312  75F9              jnz 0x30d
00000314  803E573D00        cmp byte [0x3d57],0x0
00000319  740D              jz 0x328
0000031B  C646FF63          mov byte [bp-0x1],0x63
0000031F  EB00              jmp short 0x321
00000321  803E573D00        cmp byte [0x3d57],0x0
00000326  75F9              jnz 0x321
00000328  803E633D00        cmp byte [0x3d63],0x0
0000032D  740D              jz 0x33c
0000032F  C646FF01          mov byte [bp-0x1],0x1
00000333  EB00              jmp short 0x335
00000335  803E633D00        cmp byte [0x3d63],0x0
0000033A  75F9              jnz 0x335
0000033C  9AA806960A        call 0xa96:0x6a8
00000341  B80100            mov ax,0x1
00000344  50                push ax
00000345  9ABA01FD21        call 0x21fd:0x1ba
0000034A  59                pop cx
0000034B  807EFF00          cmp byte [bp-0x1],0x0
0000034F  7503              jnz 0x354
00000351  E9F1FE            jmp 0x245
00000354  B80100            mov ax,0x1
00000357  50                push ax
00000358  33C0              xor ax,ax
0000035A  50                push ax
0000035B  9A5400A721        call 0x21a7:0x54
00000360  59                pop cx
00000361  59                pop cx
00000362  807EFF63          cmp byte [bp-0x1],0x63
00000366  7506              jnz 0x36e
00000368  B000              mov al,0x0
0000036A  EB06              jmp short 0x372
0000036C  EB04              jmp short 0x372
0000036E  B001              mov al,0x1
00000370  EB00              jmp short 0x372
00000372  8BE5              mov sp,bp
00000374  5D                pop bp
00000375  CB                retf

; ---- 192D:0376 ----
00000376  55                push bp
00000377  8BEC              mov bp,sp
00000379  56                push si
0000037A  8B7606            mov si,[bp+0x6]
0000037D  80BC400800        cmp byte [si+0x840],0x0
00000382  7504              jnz 0x388
00000384  B001              mov al,0x1
00000386  EB02              jmp short 0x38a
00000388  B000              mov al,0x0
0000038A  88844008          mov [si+0x840],al
0000038E  56                push si
0000038F  9AAB032D19        call 0x192d:0x3ab
00000394  59                pop cx
00000395  B80200            mov ax,0x2
00000398  50                push ax
00000399  33C0              xor ax,ax
0000039B  50                push ax
0000039C  9A5400A721        call 0x21a7:0x54
000003A1  59                pop cx
000003A2  59                pop cx
000003A3  9AE3032D19        call 0x192d:0x3e3
000003A8  5E                pop si
000003A9  5D                pop bp
000003AA  CB                retf

; ---- 192D:03AB ----
000003AB  55                push bp
000003AC  8BEC              mov bp,sp
000003AE  56                push si
000003AF  8B7606            mov si,[bp+0x6]
000003B2  B009              mov al,0x9
000003B4  50                push ax
000003B5  8A844008          mov al,[si+0x840]
000003B9  B400              mov ah,0x0
000003BB  D1E0              shl ax,1
000003BD  D1E0              shl ax,1
000003BF  8BD8              mov bx,ax
000003C1  FFB7EE3B          push word [bx+0x3bee]
000003C5  FFB7EC3B          push word [bx+0x3bec]
000003C9  8BC6              mov ax,si
000003CB  BA0700            mov dx,0x7
000003CE  F7EA              imul dx
000003D0  057800            add ax,0x78
000003D3  50                push ax
000003D4  B87800            mov ax,0x78
000003D7  50                push ax
000003D8  9AF9172D19        call 0x192d:0x17f9
000003DD  83C40A            add sp,byte +0xa
000003E0  5E                pop si
000003E1  5D                pop bp
000003E2  CB                retf

; ---- 192D:03E3 ----
000003E3  55                push bp
000003E4  8BEC              mov bp,sp
000003E6  56                push si
000003E7  57                push di
000003E8  33FF              xor di,di
000003EA  33F6              xor si,si
000003EC  EB09              jmp short 0x3f7
000003EE  80BC400801        cmp byte [si+0x840],0x1
000003F3  7501              jnz 0x3f6
000003F5  47                inc di
000003F6  46                inc si
000003F7  83FE03            cmp si,byte +0x3
000003FA  7EF2              jng 0x3ee
000003FC  83FF04            cmp di,byte +0x4
000003FF  7504              jnz 0x405
00000401  B001              mov al,0x1
00000403  EB02              jmp short 0x407
00000405  B000              mov al,0x0
00000407  A28145            mov [0x4581],al
0000040A  5F                pop di
0000040B  5E                pop si
0000040C  5D                pop bp
0000040D  CB                retf

; ---- 192D:040E ----
0000040E  55                push bp
0000040F  8BEC              mov bp,sp
00000411  B80200            mov ax,0x2
00000414  50                push ax
00000415  33C0              xor ax,ax
00000417  50                push ax
00000418  9A5400A721        call 0x21a7:0x54
0000041D  59                pop cx
0000041E  59                pop cx
0000041F  9A2F042D19        call 0x192d:0x42f
00000424  9A95042D19        call 0x192d:0x495
00000429  0E                push cs
0000042A  E898FC            call 0xc5
0000042D  5D                pop bp
0000042E  CB                retf

; ---- 192D:042F ----
0000042F  55                push bp
00000430  8BEC              mov bp,sp
00000432  B82800            mov ax,0x28
00000435  50                push ax
00000436  1E                push ds
00000437  B8C839            mov ax,0x39c8
0000043A  50                push ax
0000043B  B81E00            mov ax,0x1e
0000043E  50                push ax
0000043F  B8A200            mov ax,0xa2
00000442  50                push ax
00000443  9AAF162D19        call 0x192d:0x16af
00000448  83C40A            add sp,byte +0xa
0000044B  9A1B00960A        call 0xa96:0x1b
00000450  833EEA08FF        cmp word [0x8ea],byte -0x1
00000455  7505              jnz 0x45c
00000457  9A53062D19        call 0x192d:0x653
0000045C  9A6C072D19        call 0x192d:0x76c
00000461  9AAC072D19        call 0x192d:0x7ac
00000466  9AFC072D19        call 0x192d:0x7fc
0000046B  9A4A082D19        call 0x192d:0x84a
00000470  9A99082D19        call 0x192d:0x899
00000475  9AE7082D19        call 0x192d:0x8e7
0000047A  9A35092D19        call 0x192d:0x935
0000047F  9AA806960A        call 0xa96:0x6a8
00000484  FF362E08          push word [0x82e]
00000488  9A3F0C960A        call 0xa96:0xc3f
0000048D  59                pop cx
0000048E  9A5303FD21        call 0x21fd:0x353
00000493  5D                pop bp
00000494  CB                retf

; ---- 192D:0495 ----
00000495  55                push bp
00000496  8BEC              mov bp,sp
00000498  4C                dec sp
00000499  4C                dec sp
0000049A  C646FF00          mov byte [bp-0x1],0x0
0000049E  9A1B00960A        call 0xa96:0x1b
000004A3  E9D100            jmp 0x577
000004A6  9A1F05FD21        call 0x21fd:0x51f
000004AB  9AA806960A        call 0xa96:0x6a8
000004B0  9A1B00960A        call 0xa96:0x1b
000004B5  9A4502FD21        call 0x21fd:0x245
000004BA  833EEA08FF        cmp word [0x8ea],byte -0x1
000004BF  757E              jnz 0x53f
000004C1  803E863D00        cmp byte [0x3d86],0x0
000004C6  740E              jz 0x4d6
000004C8  9A98062D19        call 0x192d:0x698
000004CD  EB00              jmp short 0x4cf
000004CF  803E863D00        cmp byte [0x3d86],0x0
000004D4  75F9              jnz 0x4cf
000004D6  803E873D00        cmp byte [0x3d87],0x0
000004DB  740E              jz 0x4eb
000004DD  9AC2062D19        call 0x192d:0x6c2
000004E2  EB00              jmp short 0x4e4
000004E4  803E873D00        cmp byte [0x3d87],0x0
000004E9  75F9              jnz 0x4e4
000004EB  803E883D00        cmp byte [0x3d88],0x0
000004F0  740E              jz 0x500
000004F2  9AE2062D19        call 0x192d:0x6e2
000004F7  EB00              jmp short 0x4f9
000004F9  803E883D00        cmp byte [0x3d88],0x0
000004FE  75F9              jnz 0x4f9
00000500  803E893D00        cmp byte [0x3d89],0x0
00000505  740E              jz 0x515
00000507  9A02072D19        call 0x192d:0x702
0000050C  EB00              jmp short 0x50e
0000050E  803E893D00        cmp byte [0x3d89],0x0
00000513  75F9              jnz 0x50e
00000515  803E8A3D00        cmp byte [0x3d8a],0x0
0000051A  740E              jz 0x52a
0000051C  9A22072D19        call 0x192d:0x722
00000521  EB00              jmp short 0x523
00000523  803E8A3D00        cmp byte [0x3d8a],0x0
00000528  75F9              jnz 0x523
0000052A  803E8B3D00        cmp byte [0x3d8b],0x0
0000052F  740E              jz 0x53f
00000531  9A42072D19        call 0x192d:0x742
00000536  EB00              jmp short 0x538
00000538  803E8B3D00        cmp byte [0x3d8b],0x0
0000053D  75F9              jnz 0x538
0000053F  803E823D00        cmp byte [0x3d82],0x0
00000544  740E              jz 0x554
00000546  9ADD052D19        call 0x192d:0x5dd
0000054B  EB00              jmp short 0x54d
0000054D  803E823D00        cmp byte [0x3d82],0x0
00000552  75F9              jnz 0x54d
00000554  803E633D00        cmp byte [0x3d63],0x0
00000559  740D              jz 0x568
0000055B  C646FF01          mov byte [bp-0x1],0x1
0000055F  EB00              jmp short 0x561
00000561  803E633D00        cmp byte [0x3d63],0x0
00000566  75F9              jnz 0x561
00000568  9AA806960A        call 0xa96:0x6a8
0000056D  B80100            mov ax,0x1
00000570  50                push ax
00000571  9ABA01FD21        call 0x21fd:0x1ba
00000576  59                pop cx
00000577  807EFF00          cmp byte [bp-0x1],0x0
0000057B  7503              jnz 0x580
0000057D  E926FF            jmp 0x4a6
00000580  B80100            mov ax,0x1
00000583  50                push ax
00000584  33C0              xor ax,ax
00000586  50                push ax
00000587  9A5400A721        call 0x21a7:0x54
0000058C  59                pop cx
0000058D  59                pop cx
0000058E  8BE5              mov sp,bp
00000590  5D                pop bp
00000591  CB                retf

; ---- 192D:0592 ----
00000592  55                push bp
00000593  8BEC              mov bp,sp
00000595  8B1EEA08          mov bx,[0x8ea]
00000599  D1E3              shl bx,1
0000059B  8B87900B          mov ax,[bx+0xb90]
0000059F  A38E2E            mov [0x2e8e],ax
000005A2  8B1EEA08          mov bx,[0x8ea]
000005A6  8A87980B          mov al,[bx+0xb98]
000005AA  A2BA45            mov [0x45ba],al
000005AD  8B1EEA08          mov bx,[0x8ea]
000005B1  8A879C0B          mov al,[bx+0xb9c]
000005B5  A21A41            mov [0x411a],al
000005B8  8B1EEA08          mov bx,[0x8ea]
000005BC  8A87A00B          mov al,[bx+0xba0]
000005C0  A2F24B            mov [0x4bf2],al
000005C3  8B1EEA08          mov bx,[0x8ea]
000005C7  8A87A40B          mov al,[bx+0xba4]
000005CB  A2302F            mov [0x2f30],al
000005CE  8B1EEA08          mov bx,[0x8ea]
000005D2  D1E3              shl bx,1
000005D4  8B87A80B          mov ax,[bx+0xba8]
000005D8  A30009            mov [0x900],ax
000005DB  5D                pop bp
000005DC  CB                retf

; ---- 192D:05DD ----
000005DD  55                push bp
000005DE  8BEC              mov bp,sp
000005E0  A1EA08            mov ax,[0x8ea]
000005E3  40                inc ax
000005E4  A3EA08            mov [0x8ea],ax
000005E7  3D0400            cmp ax,0x4
000005EA  7C0D              jl 0x5f9
000005EC  C706EA08FFFF      mov word [0x8ea],0xffff
000005F2  9A53062D19        call 0x192d:0x653
000005F7  EB45              jmp short 0x63e
000005F9  833EEA0800        cmp word [0x8ea],byte +0x0
000005FE  751C              jnz 0x61c
00000600  B80200            mov ax,0x2
00000603  50                push ax
00000604  B82A00            mov ax,0x2a
00000607  50                push ax
00000608  B81800            mov ax,0x18
0000060B  50                push ax
0000060C  B84700            mov ax,0x47
0000060F  50                push ax
00000610  B84500            mov ax,0x45
00000613  50                push ax
00000614  9AD704960A        call 0xa96:0x4d7
00000619  83C40A            add sp,byte +0xa
0000061C  0E                push cs
0000061D  E872FF            call 0x592
00000620  9AAC072D19        call 0x192d:0x7ac
00000625  9AFC072D19        call 0x192d:0x7fc
0000062A  9A4A082D19        call 0x192d:0x84a
0000062F  9A99082D19        call 0x192d:0x899
00000634  9AE7082D19        call 0x192d:0x8e7
00000639  9A35092D19        call 0x192d:0x935
0000063E  9A6C072D19        call 0x192d:0x76c
00000643  B80200            mov ax,0x2
00000646  50                push ax
00000647  33C0              xor ax,ax
00000649  50                push ax
0000064A  9A5400A721        call 0x21a7:0x54
0000064F  59                pop cx
00000650  59                pop cx
00000651  5D                pop bp
00000652  CB                retf

; ---- 192D:0653 ----
00000653  55                push bp
00000654  8BEC              mov bp,sp
00000656  56                push si
00000657  33F6              xor si,si
00000659  EB35              jmp short 0x690
0000065B  B80100            mov ax,0x1
0000065E  50                push ax
0000065F  B8FFFF            mov ax,0xffff
00000662  50                push ax
00000663  B80300            mov ax,0x3
00000666  50                push ax
00000667  33C0              xor ax,ax
00000669  50                push ax
0000066A  8BDE              mov bx,si
0000066C  D1E3              shl bx,1
0000066E  D1E3              shl bx,1
00000670  FFB7B20B          push word [bx+0xbb2]
00000674  FFB7B00B          push word [bx+0xbb0]
00000678  8BC6              mov ax,si
0000067A  BA0700            mov dx,0x7
0000067D  F7EA              imul dx
0000067F  054C00            add ax,0x4c
00000682  50                push ax
00000683  B84600            mov ax,0x46
00000686  50                push ax
00000687  9A4D00690A        call 0xa69:0x4d
0000068C  83C410            add sp,byte +0x10
0000068F  46                inc si
00000690  83FE05            cmp si,byte +0x5
00000693  7EC6              jng 0x65b
00000695  5E                pop si
00000696  5D                pop bp
00000697  CB                retf

; ---- 192D:0698 ----
00000698  55                push bp
00000699  8BEC              mov bp,sp
0000069B  A18E2E            mov ax,[0x2e8e]
0000069E  40                inc ax
0000069F  A38E2E            mov [0x2e8e],ax
000006A2  3D0400            cmp ax,0x4
000006A5  7C06              jl 0x6ad
000006A7  C7068E2E0000      mov word [0x2e8e],0x0
000006AD  9AAC072D19        call 0x192d:0x7ac
000006B2  B80200            mov ax,0x2
000006B5  50                push ax
000006B6  33C0              xor ax,ax
000006B8  50                push ax
000006B9  9A5400A721        call 0x21a7:0x54
000006BE  59                pop cx
000006BF  59                pop cx
000006C0  5D                pop bp
000006C1  CB                retf

; ---- 192D:06C2 ----
000006C2  55                push bp
000006C3  8BEC              mov bp,sp
000006C5  A0BA45            mov al,[0x45ba]
000006C8  3401              xor al,0x1
000006CA  A2BA45            mov [0x45ba],al
000006CD  9AFC072D19        call 0x192d:0x7fc
000006D2  B80200            mov ax,0x2
000006D5  50                push ax
000006D6  33C0              xor ax,ax
000006D8  50                push ax
000006D9  9A5400A721        call 0x21a7:0x54
000006DE  59                pop cx
000006DF  59                pop cx
000006E0  5D                pop bp
000006E1  CB                retf

; ---- 192D:06E2 ----
000006E2  55                push bp
000006E3  8BEC              mov bp,sp
000006E5  A01A41            mov al,[0x411a]
000006E8  3401              xor al,0x1
000006EA  A21A41            mov [0x411a],al
000006ED  9A4A082D19        call 0x192d:0x84a
000006F2  B80200            mov ax,0x2
000006F5  50                push ax
000006F6  33C0              xor ax,ax
000006F8  50                push ax
000006F9  9A5400A721        call 0x21a7:0x54
000006FE  59                pop cx
000006FF  59                pop cx
00000700  5D                pop bp
00000701  CB                retf

; ---- 192D:0702 ----
00000702  55                push bp
00000703  8BEC              mov bp,sp
00000705  A0F24B            mov al,[0x4bf2]
00000708  3401              xor al,0x1
0000070A  A2F24B            mov [0x4bf2],al
0000070D  9A99082D19        call 0x192d:0x899
00000712  B80200            mov ax,0x2
00000715  50                push ax
00000716  33C0              xor ax,ax
00000718  50                push ax
00000719  9A5400A721        call 0x21a7:0x54
0000071E  59                pop cx
0000071F  59                pop cx
00000720  5D                pop bp
00000721  CB                retf

; ---- 192D:0722 ----
00000722  55                push bp
00000723  8BEC              mov bp,sp
00000725  A0302F            mov al,[0x2f30]
00000728  3401              xor al,0x1
0000072A  A2302F            mov [0x2f30],al
0000072D  9AE7082D19        call 0x192d:0x8e7
00000732  B80200            mov ax,0x2
00000735  50                push ax
00000736  33C0              xor ax,ax
00000738  50                push ax
00000739  9A5400A721        call 0x21a7:0x54
0000073E  59                pop cx
0000073F  59                pop cx
00000740  5D                pop bp
00000741  CB                retf

; ---- 192D:0742 ----
00000742  55                push bp
00000743  8BEC              mov bp,sp
00000745  A10009            mov ax,[0x900]
00000748  40                inc ax
00000749  A30009            mov [0x900],ax
0000074C  3D0300            cmp ax,0x3
0000074F  7E06              jng 0x757
00000751  C70600090000      mov word [0x900],0x0
00000757  9A35092D19        call 0x192d:0x935
0000075C  B80200            mov ax,0x2
0000075F  50                push ax
00000760  33C0              xor ax,ax
00000762  50                push ax
00000763  9A5400A721        call 0x21a7:0x54
00000768  59                pop cx
00000769  59                pop cx
0000076A  5D                pop bp
0000076B  CB                retf

; ---- 192D:076C ----
0000076C  55                push bp
0000076D  8BEC              mov bp,sp
0000076F  833EEA08FF        cmp word [0x8ea],byte -0x1
00000774  751A              jnz 0x790
00000776  B001              mov al,0x1
00000778  50                push ax
00000779  1E                push ds
0000077A  B8740C            mov ax,0xc74
0000077D  50                push ax
0000077E  B83200            mov ax,0x32
00000781  50                push ax
00000782  B88A00            mov ax,0x8a
00000785  50                push ax
00000786  9AF9172D19        call 0x192d:0x17f9
0000078B  83C40A            add sp,byte +0xa
0000078E  EB1A              jmp short 0x7aa
00000790  B001              mov al,0x1
00000792  50                push ax
00000793  A1EA08            mov ax,[0x8ea]
00000796  40                inc ax
00000797  99                cwd
00000798  52                push dx
00000799  50                push ax
0000079A  B83200            mov ax,0x32
0000079D  50                push ax
0000079E  B88A00            mov ax,0x8a
000007A1  50                push ax
000007A2  9A55182D19        call 0x192d:0x1855
000007A7  83C40A            add sp,byte +0xa
000007AA  5D                pop bp
000007AB  CB                retf

; ---- 192D:07AC ----
000007AC  55                push bp
000007AD  8BEC              mov bp,sp
000007AF  B009              mov al,0x9
000007B1  50                push ax
000007B2  8B1E8E2E          mov bx,[0x2e8e]
000007B6  D1E3              shl bx,1
000007B8  D1E3              shl bx,1
000007BA  FFB7F239          push word [bx+0x39f2]
000007BE  FFB7F039          push word [bx+0x39f0]
000007C2  B83900            mov ax,0x39
000007C5  50                push ax
000007C6  B88A00            mov ax,0x8a
000007C9  50                push ax
000007CA  9AF9172D19        call 0x192d:0x17f9
000007CF  83C40A            add sp,byte +0xa
000007D2  A13453            mov ax,[0x5334]
000007D5  056400            add ax,0x64
000007D8  50                push ax
000007D9  B84500            mov ax,0x45
000007DC  50                push ax
000007DD  B81C00            mov ax,0x1c
000007E0  50                push ax
000007E1  B81500            mov ax,0x15
000007E4  50                push ax
000007E5  B86D00            mov ax,0x6d
000007E8  50                push ax
000007E9  A18E2E            mov ax,[0x2e8e]
000007EC  BA1500            mov dx,0x15
000007EF  F7EA              imul dx
000007F1  50                push ax
000007F2  9A3000960A        call 0xa96:0x30
000007F7  83C40C            add sp,byte +0xc
000007FA  5D                pop bp
000007FB  CB                retf

; ---- 192D:07FC ----
000007FC  55                push bp
000007FD  8BEC              mov bp,sp
000007FF  B009              mov al,0x9
00000801  50                push ax
00000802  A0BA45            mov al,[0x45ba]
00000805  B400              mov ah,0x0
00000807  D1E0              shl ax,1
00000809  D1E0              shl ax,1
0000080B  8BD8              mov bx,ax
0000080D  FFB78E39          push word [bx+0x398e]
00000811  FFB78C39          push word [bx+0x398c]
00000815  B84000            mov ax,0x40
00000818  50                push ax
00000819  B88A00            mov ax,0x8a
0000081C  50                push ax
0000081D  9AF9172D19        call 0x192d:0x17f9
00000822  83C40A            add sp,byte +0xa
00000825  A0BA45            mov al,[0x45ba]
00000828  50                push ax
00000829  B80200            mov ax,0x2
0000082C  50                push ax
0000082D  B81F00            mov ax,0x1f
00000830  50                push ax
00000831  B81E00            mov ax,0x1e
00000834  50                push ax
00000835  B80D00            mov ax,0xd
00000838  50                push ax
00000839  B84B00            mov ax,0x4b
0000083C  50                push ax
0000083D  33C0              xor ax,ax
0000083F  50                push ax
00000840  9AB0092D19        call 0x192d:0x9b0
00000845  83C40E            add sp,byte +0xe
00000848  5D                pop bp
00000849  CB                retf

; ---- 192D:084A ----
0000084A  55                push bp
0000084B  8BEC              mov bp,sp
0000084D  B009              mov al,0x9
0000084F  50                push ax
00000850  A01A41            mov al,[0x411a]
00000853  B400              mov ah,0x0
00000855  D1E0              shl ax,1
00000857  D1E0              shl ax,1
00000859  8BD8              mov bx,ax
0000085B  FFB78E39          push word [bx+0x398e]
0000085F  FFB78C39          push word [bx+0x398c]
00000863  B84700            mov ax,0x47
00000866  50                push ax
00000867  B88A00            mov ax,0x8a
0000086A  50                push ax
0000086B  9AF9172D19        call 0x192d:0x17f9
00000870  83C40A            add sp,byte +0xa
00000873  A01A41            mov al,[0x411a]
00000876  50                push ax
00000877  B80300            mov ax,0x3
0000087A  50                push ax
0000087B  B83600            mov ax,0x36
0000087E  50                push ax
0000087F  B81C00            mov ax,0x1c
00000882  50                push ax
00000883  B81500            mov ax,0x15
00000886  50                push ax
00000887  B84B00            mov ax,0x4b
0000088A  50                push ax
0000088B  B80D00            mov ax,0xd
0000088E  50                push ax
0000088F  9AB0092D19        call 0x192d:0x9b0
00000894  83C40E            add sp,byte +0xe
00000897  5D                pop bp
00000898  CB                retf

; ---- 192D:0899 ----
00000899  55                push bp
0000089A  8BEC              mov bp,sp
0000089C  B009              mov al,0x9
0000089E  50                push ax
0000089F  A0F24B            mov al,[0x4bf2]
000008A2  B400              mov ah,0x0
000008A4  D1E0              shl ax,1
000008A6  D1E0              shl ax,1
000008A8  8BD8              mov bx,ax
000008AA  FFB78E39          push word [bx+0x398e]
000008AE  FFB78C39          push word [bx+0x398c]
000008B2  B84E00            mov ax,0x4e
000008B5  50                push ax
000008B6  B88A00            mov ax,0x8a
000008B9  50                push ax
000008BA  9AF9172D19        call 0x192d:0x17f9
000008BF  83C40A            add sp,byte +0xa
000008C2  A0F24B            mov al,[0x4bf2]
000008C5  50                push ax
000008C6  33C0              xor ax,ax
000008C8  50                push ax
000008C9  B85500            mov ax,0x55
000008CC  50                push ax
000008CD  B82300            mov ax,0x23
000008D0  50                push ax
000008D1  B81E00            mov ax,0x1e
000008D4  50                push ax
000008D5  B84400            mov ax,0x44
000008D8  50                push ax
000008D9  B88500            mov ax,0x85
000008DC  50                push ax
000008DD  9AB0092D19        call 0x192d:0x9b0
000008E2  83C40E            add sp,byte +0xe
000008E5  5D                pop bp
000008E6  CB                retf

; ---- 192D:08E7 ----
000008E7  55                push bp
000008E8  8BEC              mov bp,sp
000008EA  B009              mov al,0x9
000008EC  50                push ax
000008ED  A0302F            mov al,[0x2f30]
000008F0  B400              mov ah,0x0
000008F2  D1E0              shl ax,1
000008F4  D1E0              shl ax,1
000008F6  8BD8              mov bx,ax
000008F8  FFB78E39          push word [bx+0x398e]
000008FC  FFB78C39          push word [bx+0x398c]
00000900  B85500            mov ax,0x55
00000903  50                push ax
00000904  B88A00            mov ax,0x8a
00000907  50                push ax
00000908  9AF9172D19        call 0x192d:0x17f9
0000090D  83C40A            add sp,byte +0xa
00000910  A0302F            mov al,[0x2f30]
00000913  50                push ax
00000914  33C0              xor ax,ax
00000916  50                push ax
00000917  B87D00            mov ax,0x7d
0000091A  50                push ax
0000091B  B82200            mov ax,0x22
0000091E  50                push ax
0000091F  B81500            mov ax,0x15
00000922  50                push ax
00000923  B84B00            mov ax,0x4b
00000926  50                push ax
00000927  B82200            mov ax,0x22
0000092A  50                push ax
0000092B  9AB0092D19        call 0x192d:0x9b0
00000930  83C40E            add sp,byte +0xe
00000933  5D                pop bp
00000934  CB                retf

; ---- 192D:0935 ----
00000935  55                push bp
00000936  8BEC              mov bp,sp
00000938  B009              mov al,0x9
0000093A  50                push ax
0000093B  8B1E0009          mov bx,[0x900]
0000093F  D1E3              shl bx,1
00000941  D1E3              shl bx,1
00000943  FFB7023A          push word [bx+0x3a02]
00000947  FFB7003A          push word [bx+0x3a00]
0000094B  B85C00            mov ax,0x5c
0000094E  50                push ax
0000094F  B88A00            mov ax,0x8a
00000952  50                push ax
00000953  9AF9172D19        call 0x192d:0x17f9
00000958  83C40A            add sp,byte +0xa
0000095B  833E000900        cmp word [0x900],byte +0x0
00000960  7521              jnz 0x983
00000962  B80200            mov ax,0x2
00000965  50                push ax
00000966  B81C00            mov ax,0x1c
00000969  50                push ax
0000096A  B81A00            mov ax,0x1a
0000096D  50                push ax
0000096E  A13453            mov ax,[0x5334]
00000971  056400            add ax,0x64
00000974  50                push ax
00000975  B8E100            mov ax,0xe1
00000978  50                push ax
00000979  9AD704960A        call 0xa96:0x4d7
0000097E  83C40A            add sp,byte +0xa
00000981  EB2B              jmp short 0x9ae
00000983  A13453            mov ax,[0x5334]
00000986  056400            add ax,0x64
00000989  50                push ax
0000098A  B8E100            mov ax,0xe1
0000098D  50                push ax
0000098E  B81C00            mov ax,0x1c
00000991  50                push ax
00000992  B81A00            mov ax,0x1a
00000995  50                push ax
00000996  B85100            mov ax,0x51
00000999  50                push ax
0000099A  A10009            mov ax,[0x900]
0000099D  BA1A00            mov dx,0x1a
000009A0  F7EA              imul dx
000009A2  051D00            add ax,0x1d
000009A5  50                push ax
000009A6  9A3000960A        call 0xa96:0x30
000009AB  83C40C            add sp,byte +0xc
000009AE  5D                pop bp
000009AF  CB                retf

; ---- 192D:09B0 ----
000009B0  55                push bp
000009B1  8BEC              mov bp,sp
000009B3  807E1200          cmp byte [bp+0x12],0x0
000009B7  7525              jnz 0x9de
000009B9  B80200            mov ax,0x2
000009BC  50                push ax
000009BD  FF760C            push word [bp+0xc]
000009C0  FF760A            push word [bp+0xa]
000009C3  A13453            mov ax,[0x5334]
000009C6  034610            add ax,[bp+0x10]
000009C9  056100            add ax,0x61
000009CC  50                push ax
000009CD  8B460E            mov ax,[bp+0xe]
000009D0  054500            add ax,0x45
000009D3  50                push ax
000009D4  9AD704960A        call 0xa96:0x4d7
000009D9  83C40A            add sp,byte +0xa
000009DC  EB25              jmp short 0xa03
000009DE  A13453            mov ax,[0x5334]
000009E1  034610            add ax,[bp+0x10]
000009E4  056100            add ax,0x61
000009E7  50                push ax
000009E8  8B460E            mov ax,[bp+0xe]
000009EB  054500            add ax,0x45
000009EE  50                push ax
000009EF  FF760C            push word [bp+0xc]
000009F2  FF760A            push word [bp+0xa]
000009F5  FF7608            push word [bp+0x8]
000009F8  FF7606            push word [bp+0x6]
000009FB  9A3000960A        call 0xa96:0x30
00000A00  83C40C            add sp,byte +0xc
00000A03  5D                pop bp
00000A04  CB                retf

; ---- 192D:0A05 ----
00000A05  55                push bp
00000A06  8BEC              mov bp,sp
00000A08  B80200            mov ax,0x2
00000A0B  50                push ax
00000A0C  33C0              xor ax,ax
00000A0E  50                push ax
00000A0F  9A5400A721        call 0x21a7:0x54
00000A14  59                pop cx
00000A15  59                pop cx
00000A16  9A260A2D19        call 0x192d:0xa26
00000A1B  9AB40A2D19        call 0x192d:0xab4
00000A20  0E                push cs
00000A21  E8A1F6            call 0xc5
00000A24  5D                pop bp
00000A25  CB                retf

; ---- 192D:0A26 ----
00000A26  55                push bp
00000A27  8BEC              mov bp,sp
00000A29  B80300            mov ax,0x3
00000A2C  50                push ax
00000A2D  1E                push ds
00000A2E  B8103A            mov ax,0x3a10
00000A31  50                push ax
00000A32  B81E00            mov ax,0x1e
00000A35  50                push ax
00000A36  B88E00            mov ax,0x8e
00000A39  50                push ax
00000A3A  9AAF162D19        call 0x192d:0x16af
00000A3F  83C40A            add sp,byte +0xa
00000A42  9A1B00960A        call 0xa96:0x1b
00000A47  B80100            mov ax,0x1
00000A4A  50                push ax
00000A4B  B8FFFF            mov ax,0xffff
00000A4E  50                push ax
00000A4F  B80300            mov ax,0x3
00000A52  50                push ax
00000A53  33C0              xor ax,ax
00000A55  50                push ax
00000A56  FF367A3C          push word [0x3c7a]
00000A5A  FF36783C          push word [0x3c78]
00000A5E  A13453            mov ax,[0x5334]
00000A61  056900            add ax,0x69
00000A64  50                push ax
00000A65  B84600            mov ax,0x46
00000A68  50                push ax
00000A69  9A4D00690A        call 0xa69:0x4d
00000A6E  83C410            add sp,byte +0x10
00000A71  9AC00D2D19        call 0x192d:0xdc0
00000A76  9A110E2D19        call 0x192d:0xe11
00000A7B  9A640E2D19        call 0x192d:0xe64
00000A80  9A8E0E2D19        call 0x192d:0xe8e
00000A85  9AAE0E2D19        call 0x192d:0xeae
00000A8A  9AD90E2D19        call 0x192d:0xed9
00000A8F  9AF80E2D19        call 0x192d:0xef8
00000A94  9A160F2D19        call 0x192d:0xf16
00000A99  9A6E0F2D19        call 0x192d:0xf6e
00000A9E  9AA806960A        call 0xa96:0x6a8
00000AA3  FF362E08          push word [0x82e]
00000AA7  9A3F0C960A        call 0xa96:0xc3f
00000AAC  59                pop cx
00000AAD  9A5303FD21        call 0x21fd:0x353
00000AB2  5D                pop bp
00000AB3  CB                retf

; ---- 192D:0AB4 ----
00000AB4  55                push bp
00000AB5  8BEC              mov bp,sp
00000AB7  4C                dec sp
00000AB8  4C                dec sp
00000AB9  C646FF00          mov byte [bp-0x1],0x0
00000ABD  9A1B00960A        call 0xa96:0x1b
00000AC2  E92801            jmp 0xbed
00000AC5  9A1F05FD21        call 0x21fd:0x51f
00000ACA  9AA806960A        call 0xa96:0x6a8
00000ACF  9A1B00960A        call 0xa96:0x1b
00000AD4  9A4502FD21        call 0x21fd:0x245
00000AD9  803E823D00        cmp byte [0x3d82],0x0
00000ADE  740E              jz 0xaee
00000AE0  9A080C2D19        call 0x192d:0xc08
00000AE5  EB00              jmp short 0xae7
00000AE7  803E823D00        cmp byte [0x3d82],0x0
00000AEC  75F9              jnz 0xae7
00000AEE  803E833D00        cmp byte [0x3d83],0x0
00000AF3  740E              jz 0xb03
00000AF5  9A340C2D19        call 0x192d:0xc34
00000AFA  EB00              jmp short 0xafc
00000AFC  803E833D00        cmp byte [0x3d83],0x0
00000B01  75F9              jnz 0xafc
00000B03  803E843D00        cmp byte [0x3d84],0x0
00000B08  740E              jz 0xb18
00000B0A  9A750C2D19        call 0x192d:0xc75
00000B0F  EB00              jmp short 0xb11
00000B11  803E843D00        cmp byte [0x3d84],0x0
00000B16  75F9              jnz 0xb11
00000B18  803E853D00        cmp byte [0x3d85],0x0
00000B1D  740E              jz 0xb2d
00000B1F  9A9E0C2D19        call 0x192d:0xc9e
00000B24  EB00              jmp short 0xb26
00000B26  803E853D00        cmp byte [0x3d85],0x0
00000B2B  75F9              jnz 0xb26
00000B2D  803E863D00        cmp byte [0x3d86],0x0
00000B32  740E              jz 0xb42
00000B34  9AC70C2D19        call 0x192d:0xcc7
00000B39  EB00              jmp short 0xb3b
00000B3B  803E863D00        cmp byte [0x3d86],0x0
00000B40  75F9              jnz 0xb3b
00000B42  803E873D00        cmp byte [0x3d87],0x0
00000B47  740E              jz 0xb57
00000B49  9AE70C2D19        call 0x192d:0xce7
00000B4E  EB00              jmp short 0xb50
00000B50  803E873D00        cmp byte [0x3d87],0x0
00000B55  75F9              jnz 0xb50
00000B57  803E923D00        cmp byte [0x3d92],0x0
00000B5C  7413              jz 0xb71
00000B5E  B8FFFF            mov ax,0xffff
00000B61  50                push ax
00000B62  9A100D2D19        call 0x192d:0xd10
00000B67  59                pop cx
00000B68  EB00              jmp short 0xb6a
00000B6A  803E923D00        cmp byte [0x3d92],0x0
00000B6F  75F9              jnz 0xb6a
00000B71  803E943D00        cmp byte [0x3d94],0x0
00000B76  7413              jz 0xb8b
00000B78  B80100            mov ax,0x1
00000B7B  50                push ax
00000B7C  9A100D2D19        call 0x192d:0xd10
00000B81  59                pop cx
00000B82  EB00              jmp short 0xb84
00000B84  803E943D00        cmp byte [0x3d94],0x0
00000B89  75F9              jnz 0xb84
00000B8B  803E8F3D00        cmp byte [0x3d8f],0x0
00000B90  7413              jz 0xba5
00000B92  B8FFFF            mov ax,0xffff
00000B95  50                push ax
00000B96  9A420D2D19        call 0x192d:0xd42
00000B9B  59                pop cx
00000B9C  EB00              jmp short 0xb9e
00000B9E  803E8F3D00        cmp byte [0x3d8f],0x0
00000BA3  75F9              jnz 0xb9e
00000BA5  803E973D00        cmp byte [0x3d97],0x0
00000BAA  7413              jz 0xbbf
00000BAC  B80100            mov ax,0x1
00000BAF  50                push ax
00000BB0  9A420D2D19        call 0x192d:0xd42
00000BB5  59                pop cx
00000BB6  EB00              jmp short 0xbb8
00000BB8  803E973D00        cmp byte [0x3d97],0x0
00000BBD  75F9              jnz 0xbb8
00000BBF  803E663D00        cmp byte [0x3d66],0x0
00000BC4  740E              jz 0xbd4
00000BC6  9A740D2D19        call 0x192d:0xd74
00000BCB  EB00              jmp short 0xbcd
00000BCD  803E663D00        cmp byte [0x3d66],0x0
00000BD2  75F9              jnz 0xbcd
00000BD4  803E633D00        cmp byte [0x3d63],0x0
00000BD9  740D              jz 0xbe8
00000BDB  C646FF01          mov byte [bp-0x1],0x1
00000BDF  EB00              jmp short 0xbe1
00000BE1  803E633D00        cmp byte [0x3d63],0x0
00000BE6  75F9              jnz 0xbe1
00000BE8  9AA806960A        call 0xa96:0x6a8
00000BED  807EFF00          cmp byte [bp-0x1],0x0
00000BF1  7503              jnz 0xbf6
00000BF3  E9CFFE            jmp 0xac5
00000BF6  B80100            mov ax,0x1
00000BF9  50                push ax
00000BFA  33C0              xor ax,ax
00000BFC  50                push ax
00000BFD  9A5400A721        call 0x21a7:0x54
00000C02  59                pop cx
00000C03  59                pop cx
00000C04  8BE5              mov sp,bp
00000C06  5D                pop bp
00000C07  CB                retf

; ---- 192D:0C08 ----
00000C08  55                push bp
00000C09  8BEC              mov bp,sp
00000C0B  A1F208            mov ax,[0x8f2]
00000C0E  05F401            add ax,0x1f4
00000C11  A3F208            mov [0x8f2],ax
00000C14  3DD007            cmp ax,0x7d0
00000C17  7E06              jng 0xc1f
00000C19  C706F208F401      mov word [0x8f2],0x1f4
00000C1F  9AC00D2D19        call 0x192d:0xdc0
00000C24  B80200            mov ax,0x2
00000C27  50                push ax
00000C28  33C0              xor ax,ax
00000C2A  50                push ax
00000C2B  9A5400A721        call 0x21a7:0x54
00000C30  59                pop cx
00000C31  59                pop cx
00000C32  5D                pop bp
00000C33  CB                retf

; ---- 192D:0C34 ----
00000C34  55                push bp
00000C35  8BEC              mov bp,sp
00000C37  8106F4081027      add word [0x8f4],0x2710
00000C3D  A1F408            mov ax,[0x8f4]
00000C40  8316F60800        adc word [0x8f6],byte +0x0
00000C45  8B16F608          mov dx,[0x8f6]
00000C49  0BD2              or dx,dx
00000C4B  7213              jc 0xc60
00000C4D  7705              ja 0xc54
00000C4F  3D50C3            cmp ax,0xc350
00000C52  760C              jna 0xc60
00000C54  C706F4081027      mov word [0x8f4],0x2710
00000C5A  C706F6080000      mov word [0x8f6],0x0
00000C60  9A110E2D19        call 0x192d:0xe11
00000C65  B80200            mov ax,0x2
00000C68  50                push ax
00000C69  33C0              xor ax,ax
00000C6B  50                push ax
00000C6C  9A5400A721        call 0x21a7:0x54
00000C71  59                pop cx
00000C72  59                pop cx
00000C73  5D                pop bp
00000C74  CB                retf

; ---- 192D:0C75 ----
00000C75  55                push bp
00000C76  8BEC              mov bp,sp
00000C78  A0F908            mov al,[0x8f9]
00000C7B  FEC0              inc al
00000C7D  A2F908            mov [0x8f9],al
00000C80  3C02              cmp al,0x2
00000C82  7E05              jng 0xc89
00000C84  C606F90800        mov byte [0x8f9],0x0
00000C89  9A640E2D19        call 0x192d:0xe64
00000C8E  B80200            mov ax,0x2
00000C91  50                push ax
00000C92  33C0              xor ax,ax
00000C94  50                push ax
00000C95  9A5400A721        call 0x21a7:0x54
00000C9A  59                pop cx
00000C9B  59                pop cx
00000C9C  5D                pop bp
00000C9D  CB                retf

; ---- 192D:0C9E ----
00000C9E  55                push bp
00000C9F  8BEC              mov bp,sp
00000CA1  A0F808            mov al,[0x8f8]
00000CA4  040A              add al,0xa
00000CA6  A2F808            mov [0x8f8],al
00000CA9  3C1E              cmp al,0x1e
00000CAB  7605              jna 0xcb2
00000CAD  C606F8080A        mov byte [0x8f8],0xa
00000CB2  9A8E0E2D19        call 0x192d:0xe8e
00000CB7  B80200            mov ax,0x2
00000CBA  50                push ax
00000CBB  33C0              xor ax,ax
00000CBD  50                push ax
00000CBE  9A5400A721        call 0x21a7:0x54
00000CC3  59                pop cx
00000CC4  59                pop cx
00000CC5  5D                pop bp
00000CC6  CB                retf

; ---- 192D:0CC7 ----
00000CC7  55                push bp
00000CC8  8BEC              mov bp,sp
00000CCA  A0F108            mov al,[0x8f1]
00000CCD  3401              xor al,0x1
00000CCF  A2F108            mov [0x8f1],al
00000CD2  9AAE0E2D19        call 0x192d:0xeae
00000CD7  B80200            mov ax,0x2
00000CDA  50                push ax
00000CDB  33C0              xor ax,ax
00000CDD  50                push ax
00000CDE  9A5400A721        call 0x21a7:0x54
00000CE3  59                pop cx
00000CE4  59                pop cx
00000CE5  5D                pop bp
00000CE6  CB                retf

; ---- 192D:0CE7 ----
00000CE7  55                push bp
00000CE8  8BEC              mov bp,sp
00000CEA  A0EC08            mov al,[0x8ec]
00000CED  FEC0              inc al
00000CEF  A2EC08            mov [0x8ec],al
00000CF2  3C04              cmp al,0x4
00000CF4  7E05              jng 0xcfb
00000CF6  C606EC0801        mov byte [0x8ec],0x1
00000CFB  9AD90E2D19        call 0x192d:0xed9
00000D00  B80200            mov ax,0x2
00000D03  50                push ax
00000D04  33C0              xor ax,ax
00000D06  50                push ax
00000D07  9A5400A721        call 0x21a7:0x54
00000D0C  59                pop cx
00000D0D  59                pop cx
00000D0E  5D                pop bp
00000D0F  CB                retf

; ---- 192D:0D10 ----
00000D10  55                push bp
00000D11  8BEC              mov bp,sp
00000D13  56                push si
00000D14  A13053            mov ax,[0x5330]
00000D17  034606            add ax,[bp+0x6]
00000D1A  8BF0              mov si,ax
00000D1C  83FE09            cmp si,byte +0x9
00000D1F  7F05              jg 0xd26
00000D21  83FE04            cmp si,byte +0x4
00000D24  7D02              jnl 0xd28
00000D26  EB17              jmp short 0xd3f
00000D28  89363053          mov [0x5330],si
00000D2C  9AF80E2D19        call 0x192d:0xef8
00000D31  B80200            mov ax,0x2
00000D34  50                push ax
00000D35  33C0              xor ax,ax
00000D37  50                push ax
00000D38  9A5400A721        call 0x21a7:0x54
00000D3D  59                pop cx
00000D3E  59                pop cx
00000D3F  5E                pop si
00000D40  5D                pop bp
00000D41  CB                retf

; ---- 192D:0D42 ----
00000D42  55                push bp
00000D43  8BEC              mov bp,sp
00000D45  56                push si
00000D46  A13253            mov ax,[0x5332]
00000D49  034606            add ax,[bp+0x6]
00000D4C  8BF0              mov si,ax
00000D4E  83FE06            cmp si,byte +0x6
00000D51  7F05              jg 0xd58
00000D53  83FE03            cmp si,byte +0x3
00000D56  7D02              jnl 0xd5a
00000D58  EB17              jmp short 0xd71
00000D5A  89363253          mov [0x5332],si
00000D5E  9A160F2D19        call 0x192d:0xf16
00000D63  B80200            mov ax,0x2
00000D66  50                push ax
00000D67  33C0              xor ax,ax
00000D69  50                push ax
00000D6A  9A5400A721        call 0x21a7:0x54
00000D6F  59                pop cx
00000D70  59                pop cx
00000D71  5E                pop si
00000D72  5D                pop bp
00000D73  CB                retf

; ---- 192D:0D74 ----
00000D74  55                push bp
00000D75  8BEC              mov bp,sp
00000D77  A1A710            mov ax,[0x10a7]
00000D7A  F7D8              neg ax
00000D7C  1BC0              sbb ax,ax
00000D7E  40                inc ax
00000D7F  A3A710            mov [0x10a7],ax
00000D82  9A6E0F2D19        call 0x192d:0xf6e
00000D87  B80200            mov ax,0x2
00000D8A  50                push ax
00000D8B  33C0              xor ax,ax
00000D8D  50                push ax
00000D8E  9A5400A721        call 0x21a7:0x54
00000D93  59                pop cx
00000D94  59                pop cx
00000D95  5D                pop bp
00000D96  CB                retf
00000D97  55                push bp
00000D98  8BEC              mov bp,sp
00000D9A  56                push si
00000D9B  8B7606            mov si,[bp+0x6]
00000D9E  8A843C08          mov al,[si+0x83c]
00000DA2  3401              xor al,0x1
00000DA4  88843C08          mov [si+0x83c],al
00000DA8  56                push si
00000DA9  9A340F2D19        call 0x192d:0xf34
00000DAE  59                pop cx
00000DAF  B80200            mov ax,0x2
00000DB2  50                push ax
00000DB3  33C0              xor ax,ax
00000DB5  50                push ax
00000DB6  9A5400A721        call 0x21a7:0x54
00000DBB  59                pop cx
00000DBC  59                pop cx
00000DBD  5E                pop si
00000DBE  5D                pop bp
00000DBF  CB                retf

; ---- 192D:0DC0 ----
00000DC0  55                push bp
00000DC1  8BEC              mov bp,sp
00000DC3  B80200            mov ax,0x2
00000DC6  50                push ax
00000DC7  B80600            mov ax,0x6
00000DCA  50                push ax
00000DCB  B82400            mov ax,0x24
00000DCE  50                push ax
00000DCF  A13453            mov ax,[0x5334]
00000DD2  052E00            add ax,0x2e
00000DD5  50                push ax
00000DD6  B8CF00            mov ax,0xcf
00000DD9  50                push ax
00000DDA  9AD704960A        call 0xa96:0x4d7
00000DDF  83C40A            add sp,byte +0xa
00000DE2  B80100            mov ax,0x1
00000DE5  50                push ax
00000DE6  B8FFFF            mov ax,0xffff
00000DE9  50                push ax
00000DEA  B80400            mov ax,0x4
00000DED  50                push ax
00000DEE  B80100            mov ax,0x1
00000DF1  50                push ax
00000DF2  B80500            mov ax,0x5
00000DF5  50                push ax
00000DF6  A1F208            mov ax,[0x8f2]
00000DF9  99                cwd
00000DFA  52                push dx
00000DFB  50                push ax
00000DFC  A13453            mov ax,[0x5334]
00000DFF  053200            add ax,0x32
00000E02  50                push ax
00000E03  B8D000            mov ax,0xd0
00000E06  50                push ax
00000E07  9A6401690A        call 0xa69:0x164
00000E0C  83C412            add sp,byte +0x12
00000E0F  5D                pop bp
00000E10  CB                retf

; ---- 192D:0E11 ----
00000E11  55                push bp
00000E12  8BEC              mov bp,sp
00000E14  B80200            mov ax,0x2
00000E17  50                push ax
00000E18  B80600            mov ax,0x6
00000E1B  50                push ax
00000E1C  B82400            mov ax,0x24
00000E1F  50                push ax
00000E20  A13453            mov ax,[0x5334]
00000E23  053500            add ax,0x35
00000E26  50                push ax
00000E27  B8CF00            mov ax,0xcf
00000E2A  50                push ax
00000E2B  9AD704960A        call 0xa96:0x4d7
00000E30  83C40A            add sp,byte +0xa
00000E33  B80100            mov ax,0x1
00000E36  50                push ax
00000E37  B8FFFF            mov ax,0xffff
00000E3A  50                push ax
00000E3B  B80400            mov ax,0x4
00000E3E  50                push ax
00000E3F  B80100            mov ax,0x1
00000E42  50                push ax
00000E43  B80500            mov ax,0x5
00000E46  50                push ax
00000E47  FF36F608          push word [0x8f6]
00000E4B  FF36F408          push word [0x8f4]
00000E4F  A13453            mov ax,[0x5334]
00000E52  053900            add ax,0x39
00000E55  50                push ax
00000E56  B8D000            mov ax,0xd0
00000E59  50                push ax
00000E5A  9A6401690A        call 0xa69:0x164
00000E5F  83C412            add sp,byte +0x12
00000E62  5D                pop bp
00000E63  CB                retf

; ---- 192D:0E64 ----
00000E64  55                push bp
00000E65  8BEC              mov bp,sp
00000E67  B009              mov al,0x9
00000E69  50                push ax
00000E6A  A0F908            mov al,[0x8f9]
00000E6D  98                cbw
00000E6E  D1E0              shl ax,1
00000E70  D1E0              shl ax,1
00000E72  8BD8              mov bx,ax
00000E74  FFB75A3A          push word [bx+0x3a5a]
00000E78  FFB7583A          push word [bx+0x3a58]
00000E7C  B84000            mov ax,0x40
00000E7F  50                push ax
00000E80  B88A00            mov ax,0x8a
00000E83  50                push ax
00000E84  9AF9172D19        call 0x192d:0x17f9
00000E89  83C40A            add sp,byte +0xa
00000E8C  5D                pop bp
00000E8D  CB                retf

; ---- 192D:0E8E ----
00000E8E  55                push bp
00000E8F  8BEC              mov bp,sp
00000E91  B002              mov al,0x2
00000E93  50                push ax
00000E94  A0F808            mov al,[0x8f8]
00000E97  B400              mov ah,0x0
00000E99  99                cwd
00000E9A  52                push dx
00000E9B  50                push ax
00000E9C  B84700            mov ax,0x47
00000E9F  50                push ax
00000EA0  B88A00            mov ax,0x8a
00000EA3  50                push ax
00000EA4  9A55182D19        call 0x192d:0x1855
00000EA9  83C40A            add sp,byte +0xa
00000EAC  5D                pop bp
00000EAD  CB                retf

; ---- 192D:0EAE ----
00000EAE  55                push bp
00000EAF  8BEC              mov bp,sp
00000EB1  B009              mov al,0x9
00000EB3  50                push ax
00000EB4  A0F108            mov al,[0x8f1]
00000EB7  B400              mov ah,0x0
00000EB9  D1E0              shl ax,1
00000EBB  D1E0              shl ax,1
00000EBD  8BD8              mov bx,ax
00000EBF  FFB7663A          push word [bx+0x3a66]
00000EC3  FFB7643A          push word [bx+0x3a64]
00000EC7  B84E00            mov ax,0x4e
00000ECA  50                push ax
00000ECB  B88A00            mov ax,0x8a
00000ECE  50                push ax
00000ECF  9AF9172D19        call 0x192d:0x17f9
00000ED4  83C40A            add sp,byte +0xa
00000ED7  5D                pop bp
00000ED8  CB                retf

; ---- 192D:0ED9 ----
00000ED9  55                push bp
00000EDA  8BEC              mov bp,sp
00000EDC  B001              mov al,0x1
00000EDE  50                push ax
00000EDF  A0EC08            mov al,[0x8ec]
00000EE2  98                cbw
00000EE3  99                cwd
00000EE4  52                push dx
00000EE5  50                push ax
00000EE6  B85500            mov ax,0x55
00000EE9  50                push ax
00000EEA  B88A00            mov ax,0x8a
00000EED  50                push ax
00000EEE  9A55182D19        call 0x192d:0x1855
00000EF3  83C40A            add sp,byte +0xa
00000EF6  5D                pop bp
00000EF7  CB                retf

; ---- 192D:0EF8 ----
00000EF8  55                push bp
00000EF9  8BEC              mov bp,sp
00000EFB  B001              mov al,0x1
00000EFD  50                push ax
00000EFE  A13053            mov ax,[0x5330]
00000F01  99                cwd
00000F02  52                push dx
00000F03  50                push ax
00000F04  B85F00            mov ax,0x5f
00000F07  50                push ax
00000F08  B88A00            mov ax,0x8a
00000F0B  50                push ax
00000F0C  9A55182D19        call 0x192d:0x1855
00000F11  83C40A            add sp,byte +0xa
00000F14  5D                pop bp
00000F15  CB                retf

; ---- 192D:0F16 ----
00000F16  55                push bp
00000F17  8BEC              mov bp,sp
00000F19  B001              mov al,0x1
00000F1B  50                push ax
00000F1C  A13253            mov ax,[0x5332]
00000F1F  99                cwd
00000F20  52                push dx
00000F21  50                push ax
00000F22  B85F00            mov ax,0x5f
00000F25  50                push ax
00000F26  B89600            mov ax,0x96
00000F29  50                push ax
00000F2A  9A55182D19        call 0x192d:0x1855
00000F2F  83C40A            add sp,byte +0xa
00000F32  5D                pop bp
00000F33  CB                retf

; ---- 192D:0F34 ----
00000F34  55                push bp
00000F35  8BEC              mov bp,sp
00000F37  56                push si
00000F38  8B7606            mov si,[bp+0x6]
00000F3B  B009              mov al,0x9
00000F3D  50                push ax
00000F3E  8A843C08          mov al,[si+0x83c]
00000F42  98                cbw
00000F43  D1E0              shl ax,1
00000F45  D1E0              shl ax,1
00000F47  8BD8              mov bx,ax
00000F49  FFB76E3A          push word [bx+0x3a6e]
00000F4D  FFB76C3A          push word [bx+0x3a6c]
00000F51  8BC6              mov ax,si
00000F53  05FEFF            add ax,0xfffe
00000F56  BA0700            mov dx,0x7
00000F59  F7EA              imul dx
00000F5B  056900            add ax,0x69
00000F5E  50                push ax
00000F5F  B88A00            mov ax,0x8a
00000F62  50                push ax
00000F63  9AF9172D19        call 0x192d:0x17f9
00000F68  83C40A            add sp,byte +0xa
00000F6B  5E                pop si
00000F6C  5D                pop bp
00000F6D  CB                retf

; ---- 192D:0F6E ----
00000F6E  55                push bp
00000F6F  8BEC              mov bp,sp
00000F71  B003              mov al,0x3
00000F73  50                push ax
00000F74  8B1EA710          mov bx,[0x10a7]
00000F78  D1E3              shl bx,1
00000F7A  D1E3              shl bx,1
00000F7C  FFB78E39          push word [bx+0x398e]
00000F80  FFB78C39          push word [bx+0x398c]
00000F84  B86900            mov ax,0x69
00000F87  50                push ax
00000F88  B88700            mov ax,0x87
00000F8B  50                push ax
00000F8C  9AF9172D19        call 0x192d:0x17f9
00000F91  83C40A            add sp,byte +0xa
00000F94  5D                pop bp
00000F95  CB                retf

; ---- 192D:0F96 ----
00000F96  55                push bp
00000F97  8BEC              mov bp,sp
00000F99  B80200            mov ax,0x2
00000F9C  50                push ax
00000F9D  33C0              xor ax,ax
00000F9F  50                push ax
00000FA0  9A5400A721        call 0x21a7:0x54
00000FA5  59                pop cx
00000FA6  59                pop cx
00000FA7  9AB70F2D19        call 0x192d:0xfb7
00000FAC  9AF8102D19        call 0x192d:0x10f8
00000FB1  0E                push cs
00000FB2  E810F1            call 0xc5
00000FB5  5D                pop bp
00000FB6  CB                retf

; ---- 192D:0FB7 ----
00000FB7  55                push bp
00000FB8  8BEC              mov bp,sp
00000FBA  C70634531B00      mov word [0x5334],0x1b
00000FC0  B82004            mov ax,0x420
00000FC3  50                push ax
00000FC4  9AB403FD21        call 0x21fd:0x3b4
00000FC9  59                pop cx
00000FCA  9A1B00960A        call 0xa96:0x1b
00000FCF  9A1B132D19        call 0x192d:0x131b
00000FD4  B85700            mov ax,0x57
00000FD7  50                push ax
00000FD8  B8C800            mov ax,0xc8
00000FDB  50                push ax
00000FDC  FF363453          push word [0x5334]
00000FE0  B83C00            mov ax,0x3c
00000FE3  50                push ax
00000FE4  9AFB142D19        call 0x192d:0x14fb
00000FE9  83C408            add sp,byte +0x8
00000FEC  B82000            mov ax,0x20
00000FEF  50                push ax
00000FF0  B8C800            mov ax,0xc8
00000FF3  50                push ax
00000FF4  A13453            mov ax,[0x5334]
00000FF7  03063453          add ax,[0x5334]
00000FFB  055700            add ax,0x57
00000FFE  50                push ax
00000FFF  B83C00            mov ax,0x3c
00001002  50                push ax
00001003  9AFB142D19        call 0x192d:0x14fb
00001008  83C408            add sp,byte +0x8
0000100B  9AA806960A        call 0xa96:0x6a8
00001010  B80100            mov ax,0x1
00001013  50                push ax
00001014  9ADB05FD21        call 0x21fd:0x5db
00001019  59                pop cx
0000101A  9A1B00960A        call 0xa96:0x1b
0000101F  B80400            mov ax,0x4
00001022  50                push ax
00001023  B8FCFF            mov ax,0xfffc
00001026  50                push ax
00001027  B80400            mov ax,0x4
0000102A  50                push ax
0000102B  B80100            mov ax,0x1
0000102E  50                push ax
0000102F  FF36763A          push word [0x3a76]
00001033  FF36743A          push word [0x3a74]
00001037  B84001            mov ax,0x140
0000103A  50                push ax
0000103B  A13453            mov ax,[0x5334]
0000103E  051E00            add ax,0x1e
00001041  50                push ax
00001042  33C0              xor ax,ax
00001044  50                push ax
00001045  9AD300690A        call 0xa69:0xd3
0000104A  83C412            add sp,byte +0x12
0000104D  9AA806960A        call 0xa96:0x6a8
00001052  33C0              xor ax,ax
00001054  50                push ax
00001055  9ADB05FD21        call 0x21fd:0x5db
0000105A  59                pop cx
0000105B  9A1B00960A        call 0xa96:0x1b
00001060  B80100            mov ax,0x1
00001063  50                push ax
00001064  B8FFFF            mov ax,0xffff
00001067  50                push ax
00001068  B80300            mov ax,0x3
0000106B  50                push ax
0000106C  33C0              xor ax,ax
0000106E  50                push ax
0000106F  FF367A3A          push word [0x3a7a]
00001073  FF36783A          push word [0x3a78]
00001077  A13453            mov ax,[0x5334]
0000107A  053200            add ax,0x32
0000107D  50                push ax
0000107E  B84600            mov ax,0x46
00001081  50                push ax
00001082  9A4D00690A        call 0xa69:0x4d
00001087  83C410            add sp,byte +0x10
0000108A  B80100            mov ax,0x1
0000108D  50                push ax
0000108E  B8FFFF            mov ax,0xffff
00001091  50                push ax
00001092  B80300            mov ax,0x3
00001095  50                push ax
00001096  33C0              xor ax,ax
00001098  50                push ax
00001099  FF367E3A          push word [0x3a7e]
0000109D  FF367C3A          push word [0x3a7c]
000010A1  A13453            mov ax,[0x5334]
000010A4  053900            add ax,0x39
000010A7  50                push ax
000010A8  B84600            mov ax,0x46
000010AB  50                push ax
000010AC  9A4D00690A        call 0xa69:0x4d
000010B1  83C410            add sp,byte +0x10
000010B4  B80100            mov ax,0x1
000010B7  50                push ax
000010B8  B8FFFF            mov ax,0xffff
000010BB  50                push ax
000010BC  B80400            mov ax,0x4
000010BF  50                push ax
000010C0  B80100            mov ax,0x1
000010C3  50                push ax
000010C4  FF36863A          push word [0x3a86]
000010C8  FF36843A          push word [0x3a84]
000010CC  B84001            mov ax,0x140
000010CF  50                push ax
000010D0  A13453            mov ax,[0x5334]
000010D3  054300            add ax,0x43
000010D6  50                push ax
000010D7  33C0              xor ax,ax
000010D9  50                push ax
000010DA  9AD300690A        call 0xa69:0xd3
000010DF  83C412            add sp,byte +0x12
000010E2  9AA806960A        call 0xa96:0x6a8
000010E7  FF362E08          push word [0x82e]
000010EB  9A3F0C960A        call 0xa96:0xc3f
000010F0  59                pop cx
000010F1  9A5303FD21        call 0x21fd:0x353
000010F6  5D                pop bp
000010F7  CB                retf

; ---- 192D:10F8 ----
000010F8  55                push bp
000010F9  8BEC              mov bp,sp
000010FB  4C                dec sp
000010FC  4C                dec sp
000010FD  C646FF00          mov byte [bp-0x1],0x0
00001101  9A1B00960A        call 0xa96:0x1b
00001106  EB57              jmp short 0x115f
00001108  9A1F05FD21        call 0x21fd:0x51f
0000110D  9AA806960A        call 0xa96:0x6a8
00001112  9A1B00960A        call 0xa96:0x1b
00001117  9A4502FD21        call 0x21fd:0x245
0000111C  803E6D3D00        cmp byte [0x3d6d],0x0
00001121  740E              jz 0x1131
00001123  9A77112D19        call 0x192d:0x1177
00001128  EB00              jmp short 0x112a
0000112A  803E6D3D00        cmp byte [0x3d6d],0x0
0000112F  75F9              jnz 0x112a
00001131  803E663D00        cmp byte [0x3d66],0x0
00001136  740E              jz 0x1146
00001138  9AB8112D19        call 0x192d:0x11b8
0000113D  EB00              jmp short 0x113f
0000113F  803E663D00        cmp byte [0x3d66],0x0
00001144  75F9              jnz 0x113f
00001146  803E633D00        cmp byte [0x3d63],0x0
0000114B  740D              jz 0x115a
0000114D  C646FF01          mov byte [bp-0x1],0x1
00001151  EB00              jmp short 0x1153
00001153  803E633D00        cmp byte [0x3d63],0x0
00001158  75F9              jnz 0x1153
0000115A  9AA806960A        call 0xa96:0x6a8
0000115F  807EFF00          cmp byte [bp-0x1],0x0
00001163  74A3              jz 0x1108
00001165  B80100            mov ax,0x1
00001168  50                push ax
00001169  33C0              xor ax,ax
0000116B  50                push ax
0000116C  9A5400A721        call 0x21a7:0x54
00001171  59                pop cx
00001172  59                pop cx
00001173  8BE5              mov sp,bp
00001175  5D                pop bp
00001176  CB                retf

; ---- 192D:1177 ----
00001177  55                push bp
00001178  8BEC              mov bp,sp
0000117A  B80200            mov ax,0x2
0000117D  50                push ax
0000117E  33C0              xor ax,ax
00001180  50                push ax
00001181  9A5400A721        call 0x21a7:0x54
00001186  59                pop cx
00001187  59                pop cx
00001188  9A39053623        call 0x2336:0x539
0000118D  3C01              cmp al,0x1
0000118F  7516              jnz 0x11a7
00001191  FF368A3A          push word [0x3a8a]
00001195  FF36883A          push word [0x3a88]
00001199  9A0C122D19        call 0x192d:0x120c
0000119E  59                pop cx
0000119F  59                pop cx
000011A0  C606FA0801        mov byte [0x8fa],0x1
000011A5  EB0F              jmp short 0x11b6
000011A7  FF368E3A          push word [0x3a8e]
000011AB  FF368C3A          push word [0x3a8c]
000011AF  9A0C122D19        call 0x192d:0x120c
000011B4  59                pop cx
000011B5  59                pop cx
000011B6  5D                pop bp
000011B7  CB                retf

; ---- 192D:11B8 ----
000011B8  55                push bp
000011B9  8BEC              mov bp,sp
000011BB  B80200            mov ax,0x2
000011BE  50                push ax
000011BF  33C0              xor ax,ax
000011C1  50                push ax
000011C2  9A5400A721        call 0x21a7:0x54
000011C7  59                pop cx
000011C8  59                pop cx
000011C9  803EFA0800        cmp byte [0x8fa],0x0
000011CE  7511              jnz 0x11e1
000011D0  FF36923A          push word [0x3a92]
000011D4  FF36903A          push word [0x3a90]
000011D8  9A0C122D19        call 0x192d:0x120c
000011DD  59                pop cx
000011DE  59                pop cx
000011DF  EB29              jmp short 0x120a
000011E1  9A0A003623        call 0x2336:0xa
000011E6  3C01              cmp al,0x1
000011E8  7511              jnz 0x11fb
000011EA  FF36963A          push word [0x3a96]
000011EE  FF36943A          push word [0x3a94]
000011F2  9A0C122D19        call 0x192d:0x120c
000011F7  59                pop cx
000011F8  59                pop cx
000011F9  EB0F              jmp short 0x120a
000011FB  FF369A3A          push word [0x3a9a]
000011FF  FF36983A          push word [0x3a98]
00001203  9A0C122D19        call 0x192d:0x120c
00001208  59                pop cx
00001209  59                pop cx
0000120A  5D                pop bp
0000120B  CB                retf

; ---- 192D:120C ----
0000120C  55                push bp
0000120D  8BEC              mov bp,sp
0000120F  B80200            mov ax,0x2
00001212  50                push ax
00001213  B80600            mov ax,0x6
00001216  50                push ax
00001217  B89C00            mov ax,0x9c
0000121A  50                push ax
0000121B  A13453            mov ax,[0x5334]
0000121E  03063453          add ax,[0x5334]
00001222  056600            add ax,0x66
00001225  50                push ax
00001226  B85100            mov ax,0x51
00001229  50                push ax
0000122A  9AD704960A        call 0xa96:0x4d7
0000122F  83C40A            add sp,byte +0xa
00001232  B80100            mov ax,0x1
00001235  50                push ax
00001236  B8FFFF            mov ax,0xffff
00001239  50                push ax
0000123A  B80300            mov ax,0x3
0000123D  50                push ax
0000123E  33C0              xor ax,ax
00001240  50                push ax
00001241  FF7608            push word [bp+0x8]
00001244  FF7606            push word [bp+0x6]
00001247  B84001            mov ax,0x140
0000124A  50                push ax
0000124B  A13453            mov ax,[0x5334]
0000124E  03063453          add ax,[0x5334]
00001252  056A00            add ax,0x6a
00001255  50                push ax
00001256  33C0              xor ax,ax
00001258  50                push ax
00001259  9AD300690A        call 0xa69:0xd3
0000125E  83C412            add sp,byte +0x12
00001261  5D                pop bp
00001262  CB                retf

; ---- 192D:1263 ----
00001263  55                push bp
00001264  8BEC              mov bp,sp
00001266  B81200            mov ax,0x12
00001269  50                push ax
0000126A  1E                push ds
0000126B  B8F43B            mov ax,0x3bf4
0000126E  50                push ax
0000126F  B81E00            mov ax,0x1e
00001272  50                push ax
00001273  B88C00            mov ax,0x8c
00001276  50                push ax
00001277  9AAF162D19        call 0x192d:0x16af
0000127C  83C40A            add sp,byte +0xa
0000127F  FF362E08          push word [0x82e]
00001283  9A3F0C960A        call 0xa96:0xc3f
00001288  59                pop cx
00001289  9A5303FD21        call 0x21fd:0x353
0000128E  EB00              jmp short 0x1290
00001290  803E633D00        cmp byte [0x3d63],0x0
00001295  74F9              jz 0x1290
00001297  EB00              jmp short 0x1299
00001299  803E633D00        cmp byte [0x3d63],0x0
0000129E  75F9              jnz 0x1299
000012A0  0E                push cs
000012A1  E821EE            call 0xc5
000012A4  5D                pop bp
000012A5  CB                retf

; ---- 192D:12A6 ----
000012A6  55                push bp
000012A7  8BEC              mov bp,sp
000012A9  803EFA0800        cmp byte [0x8fa],0x0
000012AE  7507              jnz 0x12b7
000012B0  C606CC3C01        mov byte [0x3ccc],0x1
000012B5  EB62              jmp short 0x1319
000012B7  B80300            mov ax,0x3
000012BA  50                push ax
000012BB  1E                push ds
000012BC  B8B03A            mov ax,0x3ab0
000012BF  50                push ax
000012C0  B81E00            mov ax,0x1e
000012C3  50                push ax
000012C4  B86400            mov ax,0x64
000012C7  50                push ax
000012C8  9AAF162D19        call 0x192d:0x16af
000012CD  83C40A            add sp,byte +0xa
000012D0  FF362E08          push word [0x82e]
000012D4  9A3F0C960A        call 0xa96:0xc3f
000012D9  59                pop cx
000012DA  9A2903FD21        call 0x21fd:0x329
000012DF  803E783D00        cmp byte [0x3d78],0x0
000012E4  7407              jz 0x12ed
000012E6  C606CC3C01        mov byte [0x3ccc],0x1
000012EB  EB1E              jmp short 0x130b
000012ED  803E653D00        cmp byte [0x3d65],0x0
000012F2  750E              jnz 0x1302
000012F4  803E5F3D00        cmp byte [0x3d5f],0x0
000012F9  7507              jnz 0x1302
000012FB  803E763D00        cmp byte [0x3d76],0x0
00001300  7407              jz 0x1309
00001302  C606CC3C02        mov byte [0x3ccc],0x2
00001307  EB02              jmp short 0x130b
00001309  EBD4              jmp short 0x12df
0000130B  B80100            mov ax,0x1
0000130E  50                push ax
0000130F  33C0              xor ax,ax
00001311  50                push ax
00001312  9A5400A721        call 0x21a7:0x54
00001317  59                pop cx
00001318  59                pop cx
00001319  5D                pop bp
0000131A  CB                retf

; ---- 192D:131B ----
0000131B  55                push bp
0000131C  8BEC              mov bp,sp
0000131E  4C                dec sp
0000131F  4C                dec sp
00001320  56                push si
00001321  57                push di
00001322  C746FEECFF        mov word [bp-0x2],0xffec
00001327  EB59              jmp short 0x1382
00001329  33F6              xor si,si
0000132B  0BF6              or si,si
0000132D  7511              jnz 0x1340
0000132F  B87800            mov ax,0x78
00001332  50                push ax
00001333  9ABA01FD21        call 0x21fd:0x1ba
00001338  59                pop cx
00001339  050F00            add ax,0xf
0000133C  8BF8              mov di,ax
0000133E  EB0F              jmp short 0x134f
00001340  B83C00            mov ax,0x3c
00001343  50                push ax
00001344  9ABA01FD21        call 0x21fd:0x1ba
00001349  59                pop cx
0000134A  057800            add ax,0x78
0000134D  8BF8              mov di,ax
0000134F  8BC6              mov ax,si
00001351  03C7              add ax,di
00001353  3D3101            cmp ax,0x131
00001356  7E07              jng 0x135f
00001358  B84001            mov ax,0x140
0000135B  2BC6              sub ax,si
0000135D  8BF8              mov di,ax
0000135F  57                push di
00001360  FF76FE            push word [bp-0x2]
00001363  56                push si
00001364  9A8F132D19        call 0x192d:0x138f
00001369  83C406            add sp,byte +0x6
0000136C  03F7              add si,di
0000136E  8BC6              mov ax,si
00001370  3D4001            cmp ax,0x140
00001373  7502              jnz 0x1377
00001375  EB02              jmp short 0x1379
00001377  EBB2              jmp short 0x132b
00001379  8B46FE            mov ax,[bp-0x2]
0000137C  052800            add ax,0x28
0000137F  8946FE            mov [bp-0x2],ax
00001382  817EFEC700        cmp word [bp-0x2],0xc7
00001387  7EA0              jng 0x1329
00001389  5F                pop di
0000138A  5E                pop si
0000138B  8BE5              mov sp,bp
0000138D  5D                pop bp
0000138E  CB                retf

; ---- 192D:138F ----
0000138F  55                push bp
00001390  8BEC              mov bp,sp
00001392  56                push si
00001393  57                push di
00001394  837E0800          cmp word [bp+0x8],byte +0x0
00001398  7D15              jnl 0x13af
0000139A  8B4608            mov ax,[bp+0x8]
0000139D  F7D8              neg ax
0000139F  8BF0              mov si,ax
000013A1  B82800            mov ax,0x28
000013A4  2BC6              sub ax,si
000013A6  8BF8              mov di,ax
000013A8  C746080000        mov word [bp+0x8],0x0
000013AD  EB18              jmp short 0x13c7
000013AF  817E08A000        cmp word [bp+0x8],0xa0
000013B4  7E0C              jng 0x13c2
000013B6  33F6              xor si,si
000013B8  B8C800            mov ax,0xc8
000013BB  2B4608            sub ax,[bp+0x8]
000013BE  8BF8              mov di,ax
000013C0  EB05              jmp short 0x13c7
000013C2  33F6              xor si,si
000013C4  BF2800            mov di,0x28
000013C7  0BFF              or di,di
000013C9  7F03              jg 0x13ce
000013CB  E9E200            jmp 0x14b0
000013CE  FF7608            push word [bp+0x8]
000013D1  FF7606            push word [bp+0x6]
000013D4  57                push di
000013D5  FF760A            push word [bp+0xa]
000013D8  56                push si
000013D9  B83C01            mov ax,0x13c
000013DC  2B460A            sub ax,[bp+0xa]
000013DF  50                push ax
000013E0  9ABA01FD21        call 0x21fd:0x1ba
000013E5  59                pop cx
000013E6  40                inc ax
000013E7  40                inc ax
000013E8  50                push ax
000013E9  9A3000960A        call 0xa96:0x30
000013EE  83C40C            add sp,byte +0xc
000013F1  837E0600          cmp word [bp+0x6],byte +0x0
000013F5  7E50              jng 0x1447
000013F7  FF7608            push word [bp+0x8]
000013FA  FF7606            push word [bp+0x6]
000013FD  57                push di
000013FE  B80200            mov ax,0x2
00001401  50                push ax
00001402  56                push si
00001403  33C0              xor ax,ax
00001405  50                push ax
00001406  9A3000960A        call 0xa96:0x30
0000140B  83C40C            add sp,byte +0xc
0000140E  0BF6              or si,si
00001410  7515              jnz 0x1427
00001412  8B4608            mov ax,[bp+0x8]
00001415  050400            add ax,0x4
00001418  50                push ax
00001419  8B4606            mov ax,[bp+0x6]
0000141C  050400            add ax,0x4
0000141F  50                push ax
00001420  9AB4142D19        call 0x192d:0x14b4
00001425  59                pop cx
00001426  59                pop cx
00001427  0BF6              or si,si
00001429  7505              jnz 0x1430
0000142B  83FF28            cmp di,byte +0x28
0000142E  7517              jnz 0x1447
00001430  8B4608            mov ax,[bp+0x8]
00001433  03C7              add ax,di
00001435  05F6FF            add ax,0xfff6
00001438  50                push ax
00001439  8B4606            mov ax,[bp+0x6]
0000143C  050400            add ax,0x4
0000143F  50                push ax
00001440  9AB4142D19        call 0x192d:0x14b4
00001445  59                pop cx
00001446  59                pop cx
00001447  8B4606            mov ax,[bp+0x6]
0000144A  03460A            add ax,[bp+0xa]
0000144D  3D4001            cmp ax,0x140
00001450  745E              jz 0x14b0
00001452  FF7608            push word [bp+0x8]
00001455  8B4606            mov ax,[bp+0x6]
00001458  03460A            add ax,[bp+0xa]
0000145B  05FEFF            add ax,0xfffe
0000145E  50                push ax
0000145F  57                push di
00001460  B80200            mov ax,0x2
00001463  50                push ax
00001464  56                push si
00001465  B83E01            mov ax,0x13e
00001468  50                push ax
00001469  9A3000960A        call 0xa96:0x30
0000146E  83C40C            add sp,byte +0xc
00001471  0BF6              or si,si
00001473  7518              jnz 0x148d
00001475  8B4608            mov ax,[bp+0x8]
00001478  050300            add ax,0x3
0000147B  50                push ax
0000147C  8B4606            mov ax,[bp+0x6]
0000147F  03460A            add ax,[bp+0xa]
00001482  05F5FF            add ax,0xfff5
00001485  50                push ax
00001486  9AB4142D19        call 0x192d:0x14b4
0000148B  59                pop cx
0000148C  59                pop cx
0000148D  0BF6              or si,si
0000148F  7505              jnz 0x1496
00001491  83FF28            cmp di,byte +0x28
00001494  751A              jnz 0x14b0
00001496  8B4608            mov ax,[bp+0x8]
00001499  03C7              add ax,di
0000149B  05F6FF            add ax,0xfff6
0000149E  50                push ax
0000149F  8B4606            mov ax,[bp+0x6]
000014A2  03460A            add ax,[bp+0xa]
000014A5  05F5FF            add ax,0xfff5
000014A8  50                push ax
000014A9  9AB4142D19        call 0x192d:0x14b4
000014AE  59                pop cx
000014AF  59                pop cx
000014B0  5F                pop di
000014B1  5E                pop si
000014B2  5D                pop bp
000014B3  CB                retf

; ---- 192D:14B4 ----
000014B4  55                push bp
000014B5  8BEC              mov bp,sp
000014B7  B80200            mov ax,0x2
000014BA  50                push ax
000014BB  9ABA01FD21        call 0x21fd:0x1ba
000014C0  59                pop cx
000014C1  8B5608            mov dx,[bp+0x8]
000014C4  4A                dec dx
000014C5  03D0              add dx,ax
000014C7  52                push dx
000014C8  B80200            mov ax,0x2
000014CB  50                push ax
000014CC  9ABA01FD21        call 0x21fd:0x1ba
000014D1  59                pop cx
000014D2  8B5606            mov dx,[bp+0x6]
000014D5  4A                dec dx
000014D6  03D0              add dx,ax
000014D8  52                push dx
000014D9  B80700            mov ax,0x7
000014DC  50                push ax
000014DD  B80700            mov ax,0x7
000014E0  50                push ax
000014E1  B84B00            mov ax,0x4b
000014E4  50                push ax
000014E5  B8D000            mov ax,0xd0
000014E8  50                push ax
000014E9  B82800            mov ax,0x28
000014EC  50                push ax
000014ED  B83000            mov ax,0x30
000014F0  50                push ax
000014F1  9A3B01960A        call 0xa96:0x13b
000014F6  83C410            add sp,byte +0x10
000014F9  5D                pop bp
000014FA  CB                retf

; ---- 192D:14FB ----
000014FB  55                push bp
000014FC  8BEC              mov bp,sp
000014FE  56                push si
000014FF  57                push di
00001500  8B7E06            mov di,[bp+0x6]
00001503  8B7608            mov si,[bp+0x8]
00001506  B80200            mov ax,0x2
00001509  50                push ax
0000150A  FF760C            push word [bp+0xc]
0000150D  FF760A            push word [bp+0xa]
00001510  56                push si
00001511  57                push di
00001512  9AD704960A        call 0xa96:0x4d7
00001517  83C40A            add sp,byte +0xa
0000151A  B80500            mov ax,0x5
0000151D  50                push ax
0000151E  9ABA01FD21        call 0x21fd:0x1ba
00001523  59                pop cx
00001524  0BC0              or ax,ax
00001526  7526              jnz 0x154e
00001528  8BC6              mov ax,si
0000152A  050400            add ax,0x4
0000152D  50                push ax
0000152E  8BC7              mov ax,di
00001530  050400            add ax,0x4
00001533  50                push ax
00001534  B80900            mov ax,0x9
00001537  50                push ax
00001538  B80900            mov ax,0x9
0000153B  50                push ax
0000153C  B83A00            mov ax,0x3a
0000153F  50                push ax
00001540  B83000            mov ax,0x30
00001543  50                push ax
00001544  9A3000960A        call 0xa96:0x30
00001549  83C40C            add sp,byte +0xc
0000154C  EB3C              jmp short 0x158a
0000154E  B80300            mov ax,0x3
00001551  50                push ax
00001552  9ABA01FD21        call 0x21fd:0x1ba
00001557  59                pop cx
00001558  8BD6              mov dx,si
0000155A  83C203            add dx,byte +0x3
0000155D  03D0              add dx,ax
0000155F  52                push dx
00001560  B80200            mov ax,0x2
00001563  50                push ax
00001564  9ABA01FD21        call 0x21fd:0x1ba
00001569  59                pop cx
0000156A  8BD7              mov dx,di
0000156C  83C203            add dx,byte +0x3
0000156F  03D0              add dx,ax
00001571  52                push dx
00001572  B80B00            mov ax,0xb
00001575  50                push ax
00001576  B80B00            mov ax,0xb
00001579  50                push ax
0000157A  B82F00            mov ax,0x2f
0000157D  50                push ax
0000157E  B83000            mov ax,0x30
00001581  50                push ax
00001582  9A3000960A        call 0xa96:0x30
00001587  83C40C            add sp,byte +0xc
0000158A  B80300            mov ax,0x3
0000158D  50                push ax
0000158E  9ABA01FD21        call 0x21fd:0x1ba
00001593  59                pop cx
00001594  8BD6              mov dx,si
00001596  83C203            add dx,byte +0x3
00001599  03D0              add dx,ax
0000159B  52                push dx
0000159C  B80200            mov ax,0x2
0000159F  50                push ax
000015A0  9ABA01FD21        call 0x21fd:0x1ba
000015A5  59                pop cx
000015A6  8BD7              mov dx,di
000015A8  03560A            add dx,[bp+0xa]
000015AB  83C2F2            add dx,byte -0xe
000015AE  2BD0              sub dx,ax
000015B0  52                push dx
000015B1  B80B00            mov ax,0xb
000015B4  50                push ax
000015B5  B80B00            mov ax,0xb
000015B8  50                push ax
000015B9  B82F00            mov ax,0x2f
000015BC  50                push ax
000015BD  B83000            mov ax,0x30
000015C0  50                push ax
000015C1  9A3000960A        call 0xa96:0x30
000015C6  83C40C            add sp,byte +0xc
000015C9  B80300            mov ax,0x3
000015CC  50                push ax
000015CD  9ABA01FD21        call 0x21fd:0x1ba
000015D2  59                pop cx
000015D3  8BD6              mov dx,si
000015D5  03560C            add dx,[bp+0xc]
000015D8  83C2F2            add dx,byte -0xe
000015DB  2BD0              sub dx,ax
000015DD  52                push dx
000015DE  B80200            mov ax,0x2
000015E1  50                push ax
000015E2  9ABA01FD21        call 0x21fd:0x1ba
000015E7  59                pop cx
000015E8  8BD7              mov dx,di
000015EA  03560A            add dx,[bp+0xa]
000015ED  83C2F2            add dx,byte -0xe
000015F0  2BD0              sub dx,ax
000015F2  52                push dx
000015F3  B80B00            mov ax,0xb
000015F6  50                push ax
000015F7  B80B00            mov ax,0xb
000015FA  50                push ax
000015FB  B82F00            mov ax,0x2f
000015FE  50                push ax
000015FF  B83000            mov ax,0x30
00001602  50                push ax
00001603  9A3000960A        call 0xa96:0x30
00001608  83C40C            add sp,byte +0xc
0000160B  B80300            mov ax,0x3
0000160E  50                push ax
0000160F  9ABA01FD21        call 0x21fd:0x1ba
00001614  59                pop cx
00001615  8BD6              mov dx,si
00001617  03560C            add dx,[bp+0xc]
0000161A  83C2F2            add dx,byte -0xe
0000161D  2BD0              sub dx,ax
0000161F  52                push dx
00001620  B80200            mov ax,0x2
00001623  50                push ax
00001624  9ABA01FD21        call 0x21fd:0x1ba
00001629  59                pop cx
0000162A  8BD7              mov dx,di
0000162C  83C203            add dx,byte +0x3
0000162F  03D0              add dx,ax
00001631  52                push dx
00001632  B80B00            mov ax,0xb
00001635  50                push ax
00001636  B80B00            mov ax,0xb
00001639  50                push ax
0000163A  B82F00            mov ax,0x2f
0000163D  50                push ax
0000163E  B83000            mov ax,0x30
00001641  50                push ax
00001642  9A3000960A        call 0xa96:0x30
00001647  83C40C            add sp,byte +0xc
0000164A  33C0              xor ax,ax
0000164C  50                push ax
0000164D  8B460A            mov ax,[bp+0xa]
00001650  48                dec ax
00001651  50                push ax
00001652  8BC6              mov ax,si
00001654  03460C            add ax,[bp+0xc]
00001657  50                push ax
00001658  57                push di
00001659  9A0B00690A        call 0xa69:0xb
0000165E  83C408            add sp,byte +0x8
00001661  33C0              xor ax,ax
00001663  50                push ax
00001664  8B460A            mov ax,[bp+0xa]
00001667  48                dec ax
00001668  50                push ax
00001669  8BC6              mov ax,si
0000166B  03460C            add ax,[bp+0xc]
0000166E  40                inc ax
0000166F  50                push ax
00001670  57                push di
00001671  9A0B00690A        call 0xa69:0xb
00001676  83C408            add sp,byte +0x8
00001679  33C0              xor ax,ax
0000167B  50                push ax
0000167C  8B460C            mov ax,[bp+0xc]
0000167F  40                inc ax
00001680  50                push ax
00001681  8BC6              mov ax,si
00001683  40                inc ax
00001684  50                push ax
00001685  8BC7              mov ax,di
00001687  48                dec ax
00001688  50                push ax
00001689  9A2C00690A        call 0xa69:0x2c
0000168E  83C408            add sp,byte +0x8
00001691  33C0              xor ax,ax
00001693  50                push ax
00001694  8B460C            mov ax,[bp+0xc]
00001697  40                inc ax
00001698  50                push ax
00001699  8BC6              mov ax,si
0000169B  40                inc ax
0000169C  50                push ax
0000169D  8BC7              mov ax,di
0000169F  05FEFF            add ax,0xfffe
000016A2  50                push ax
000016A3  9A2C00690A        call 0xa69:0x2c
000016A8  83C408            add sp,byte +0x8
000016AB  5F                pop di
000016AC  5E                pop si
000016AD  5D                pop bp
000016AE  CB                retf

; ---- 192D:16AF ----
000016AF  55                push bp
000016B0  8BEC              mov bp,sp
000016B2  56                push si
000016B3  57                push di
000016B4  B8C800            mov ax,0xc8
000016B7  2B4606            sub ax,[bp+0x6]
000016BA  BB0200            mov bx,0x2
000016BD  99                cwd
000016BE  F7FB              idiv bx
000016C0  A33453            mov [0x5334],ax
000016C3  8BF0              mov si,ax
000016C5  B82004            mov ax,0x420
000016C8  50                push ax
000016C9  9AB403FD21        call 0x21fd:0x3b4
000016CE  59                pop cx
000016CF  9A1B00960A        call 0xa96:0x1b
000016D4  0E                push cs
000016D5  E843FC            call 0x131b
000016D8  FF7606            push word [bp+0x6]
000016DB  B8D200            mov ax,0xd2
000016DE  50                push ax
000016DF  56                push si
000016E0  B83700            mov ax,0x37
000016E3  50                push ax
000016E4  0E                push cs
000016E5  E813FE            call 0x14fb
000016E8  83C408            add sp,byte +0x8
000016EB  9AA806960A        call 0xa96:0x6a8
000016F0  037608            add si,[bp+0x8]
000016F3  B80100            mov ax,0x1
000016F6  50                push ax
000016F7  9ADB05FD21        call 0x21fd:0x5db
000016FC  59                pop cx
000016FD  9A1B00960A        call 0xa96:0x1b
00001702  B80400            mov ax,0x4
00001705  50                push ax
00001706  B8FCFF            mov ax,0xfffc
00001709  50                push ax
0000170A  B80400            mov ax,0x4
0000170D  50                push ax
0000170E  B80100            mov ax,0x1
00001711  50                push ax
00001712  C45E0A            les bx,[bp+0xa]
00001715  26FF7702          push word [es:bx+0x2]
00001719  26FF37            push word [es:bx]
0000171C  B84001            mov ax,0x140
0000171F  50                push ax
00001720  56                push si
00001721  33C0              xor ax,ax
00001723  50                push ax
00001724  9AD300690A        call 0xa69:0xd3
00001729  83C412            add sp,byte +0x12
0000172C  9AA806960A        call 0xa96:0x6a8
00001731  33C0              xor ax,ax
00001733  50                push ax
00001734  9ADB05FD21        call 0x21fd:0x5db
00001739  59                pop cx
0000173A  8BC6              mov ax,si
0000173C  051400            add ax,0x14
0000173F  8BF0              mov si,ax
00001741  9A1B00960A        call 0xa96:0x1b
00001746  BF0100            mov di,0x1
00001749  8BC7              mov ax,di
0000174B  D1E0              shl ax,1
0000174D  D1E0              shl ax,1
0000174F  C45E0A            les bx,[bp+0xa]
00001752  03D8              add bx,ax
00001754  26C41F            les bx,[es:bx]
00001757  26803F40          cmp byte [es:bx],0x40
0000175B  7502              jnz 0x175f
0000175D  EB5B              jmp short 0x17ba
0000175F  8BC7              mov ax,di
00001761  D1E0              shl ax,1
00001763  D1E0              shl ax,1
00001765  C45E0A            les bx,[bp+0xa]
00001768  03D8              add bx,ax
0000176A  26C41F            les bx,[es:bx]
0000176D  268A07            mov al,[es:bx]
00001770  98                cbw
00001771  257F00            and ax,0x7f
00001774  3D2700            cmp ax,0x27
00001777  7509              jnz 0x1782
00001779  8BC6              mov ax,si
0000177B  050300            add ax,0x3
0000177E  8BF0              mov si,ax
00001780  EB35              jmp short 0x17b7
00001782  B80100            mov ax,0x1
00001785  50                push ax
00001786  B8FFFF            mov ax,0xffff
00001789  50                push ax
0000178A  B80300            mov ax,0x3
0000178D  50                push ax
0000178E  33C0              xor ax,ax
00001790  50                push ax
00001791  8BC7              mov ax,di
00001793  D1E0              shl ax,1
00001795  D1E0              shl ax,1
00001797  C45E0A            les bx,[bp+0xa]
0000179A  03D8              add bx,ax
0000179C  26FF7702          push word [es:bx+0x2]
000017A0  26FF37            push word [es:bx]
000017A3  56                push si
000017A4  B84600            mov ax,0x46
000017A7  50                push ax
000017A8  9A4D00690A        call 0xa69:0x4d
000017AD  83C410            add sp,byte +0x10
000017B0  8BC6              mov ax,si
000017B2  050700            add ax,0x7
000017B5  8BF0              mov si,ax
000017B7  47                inc di
000017B8  EB8F              jmp short 0x1749
000017BA  03760E            add si,[bp+0xe]
000017BD  B80100            mov ax,0x1
000017C0  50                push ax
000017C1  B8FFFF            mov ax,0xffff
000017C4  50                push ax
000017C5  B80400            mov ax,0x4
000017C8  50                push ax
000017C9  B80100            mov ax,0x1
000017CC  50                push ax
000017CD  8BC7              mov ax,di
000017CF  40                inc ax
000017D0  D1E0              shl ax,1
000017D2  D1E0              shl ax,1
000017D4  C45E0A            les bx,[bp+0xa]
000017D7  03D8              add bx,ax
000017D9  26FF7702          push word [es:bx+0x2]
000017DD  26FF37            push word [es:bx]
000017E0  B84001            mov ax,0x140
000017E3  50                push ax
000017E4  56                push si
000017E5  33C0              xor ax,ax
000017E7  50                push ax
000017E8  9AD300690A        call 0xa69:0xd3
000017ED  83C412            add sp,byte +0x12
000017F0  9AA806960A        call 0xa96:0x6a8
000017F5  5F                pop di
000017F6  5E                pop si
000017F7  5D                pop bp
000017F8  CB                retf

; ---- 192D:17F9 ----
000017F9  55                push bp
000017FA  8BEC              mov bp,sp
000017FC  B80200            mov ax,0x2
000017FF  50                push ax
00001800  B80600            mov ax,0x6
00001803  50                push ax
00001804  8A460E            mov al,[bp+0xe]
00001807  98                cbw
00001808  BA0600            mov dx,0x6
0000180B  F7EA              imul dx
0000180D  50                push ax
0000180E  A13453            mov ax,[0x5334]
00001811  034608            add ax,[bp+0x8]
00001814  05FCFF            add ax,0xfffc
00001817  50                push ax
00001818  8B4606            mov ax,[bp+0x6]
0000181B  054500            add ax,0x45
0000181E  50                push ax
0000181F  9AD704960A        call 0xa96:0x4d7
00001824  83C40A            add sp,byte +0xa
00001827  B80100            mov ax,0x1
0000182A  50                push ax
0000182B  B8FFFF            mov ax,0xffff
0000182E  50                push ax
0000182F  B80400            mov ax,0x4
00001832  50                push ax
00001833  B80100            mov ax,0x1
00001836  50                push ax
00001837  FF760C            push word [bp+0xc]
0000183A  FF760A            push word [bp+0xa]
0000183D  A13453            mov ax,[0x5334]
00001840  034608            add ax,[bp+0x8]
00001843  50                push ax
00001844  8B4606            mov ax,[bp+0x6]
00001847  054600            add ax,0x46
0000184A  50                push ax
0000184B  9A4D00690A        call 0xa69:0x4d
00001850  83C410            add sp,byte +0x10
00001853  5D                pop bp
00001854  CB                retf

; ---- 192D:1855 ----
00001855  55                push bp
00001856  8BEC              mov bp,sp
00001858  B80200            mov ax,0x2
0000185B  50                push ax
0000185C  B80600            mov ax,0x6
0000185F  50                push ax
00001860  8A460E            mov al,[bp+0xe]
00001863  98                cbw
00001864  BA0600            mov dx,0x6
00001867  F7EA              imul dx
00001869  50                push ax
0000186A  A13453            mov ax,[0x5334]
0000186D  034608            add ax,[bp+0x8]
00001870  05FCFF            add ax,0xfffc
00001873  50                push ax
00001874  8B4606            mov ax,[bp+0x6]
00001877  054500            add ax,0x45
0000187A  50                push ax
0000187B  9AD704960A        call 0xa96:0x4d7
00001880  83C40A            add sp,byte +0xa
00001883  B80100            mov ax,0x1
00001886  50                push ax
00001887  B8FFFF            mov ax,0xffff
0000188A  50                push ax
0000188B  B80400            mov ax,0x4
0000188E  50                push ax
0000188F  B80100            mov ax,0x1
00001892  50                push ax
00001893  8A460E            mov al,[bp+0xe]
00001896  98                cbw
00001897  50                push ax
00001898  FF760C            push word [bp+0xc]
0000189B  FF760A            push word [bp+0xa]
0000189E  A13453            mov ax,[0x5334]
000018A1  034608            add ax,[bp+0x8]
000018A4  50                push ax
000018A5  8B4606            mov ax,[bp+0x6]
000018A8  054600            add ax,0x46
000018AB  50                push ax
000018AC  9A1B01690A        call 0xa69:0x11b
000018B1  83C412            add sp,byte +0x12
000018B4  5D                pop bp
000018B5  CB                retf

; ---- 192D:18B6 ----
000018B6  55                push bp
000018B7  8BEC              mov bp,sp
000018B9  83EC04            sub sp,byte +0x4
000018BC  56                push si
000018BD  1E                push ds
000018BE  B8760C            mov ax,0xc76
000018C1  50                push ax
000018C2  FF368648          push word [0x4886]
000018C6  FF368448          push word [0x4884]
000018CA  9A0A0B6C18        call 0x186c:0xb0a
000018CF  83C408            add sp,byte +0x8
000018D2  C706CC370000      mov word [0x37cc],0x0
000018D8  C706CE370000      mov word [0x37ce],0x0
000018DE  A18648            mov ax,[0x4886]
000018E1  8B168448          mov dx,[0x4884]
000018E5  8916D037          mov [0x37d0],dx
000018E9  A3D237            mov [0x37d2],ax
000018EC  BE0100            mov si,0x1
000018EF  C746FC0000        mov word [bp-0x4],0x0
000018F4  C746FE0000        mov word [bp-0x2],0x0
000018F9  EB6A              jmp short 0x1965
000018FB  C41E8448          les bx,[0x4884]
000018FF  035EFC            add bx,[bp-0x4]
00001902  26803F0A          cmp byte [es:bx],0xa
00001906  7525              jnz 0x192d
00001908  C41E8448          les bx,[0x4884]
0000190C  035EFC            add bx,[bp-0x4]
0000190F  26C60700          mov byte [es:bx],0x0
00001913  46                inc si
00001914  A18648            mov ax,[0x4886]
00001917  8B168448          mov dx,[0x4884]
0000191B  0356FC            add dx,[bp-0x4]
0000191E  42                inc dx
0000191F  8BDE              mov bx,si
00001921  D1E3              shl bx,1
00001923  D1E3              shl bx,1
00001925  8997CC37          mov [bx+0x37cc],dx
00001929  8987CE37          mov [bx+0x37ce],ax
0000192D  C41E8448          les bx,[0x4884]
00001931  035EFC            add bx,[bp-0x4]
00001934  26803F0D          cmp byte [es:bx],0xd
00001938  750B              jnz 0x1945
0000193A  C41E8448          les bx,[0x4884]
0000193E  035EFC            add bx,[bp-0x4]
00001941  26C60700          mov byte [es:bx],0x0
00001945  C41E8448          les bx,[0x4884]
00001949  035EFC            add bx,[bp-0x4]
0000194C  26803F09          cmp byte [es:bx],0x9
00001950  750B              jnz 0x195d
00001952  C41E8448          les bx,[0x4884]
00001956  035EFC            add bx,[bp-0x4]
00001959  26C60720          mov byte [es:bx],0x20
0000195D  8346FC01          add word [bp-0x4],byte +0x1
00001961  8356FE00          adc word [bp-0x2],byte +0x0
00001965  837EFE00          cmp word [bp-0x2],byte +0x0
00001969  7F0F              jg 0x197a
0000196B  7C07              jl 0x1974
0000196D  817EFC9411        cmp word [bp-0x4],0x1194
00001972  7306              jnc 0x197a
00001974  81FE3001          cmp si,0x130
00001978  7C81              jl 0x18fb
0000197A  5E                pop si
0000197B  8BE5              mov sp,bp
0000197D  5D                pop bp
0000197E  CB                retf

; ---- 192D:197F ----
0000197F  55                push bp
00001980  8BEC              mov bp,sp
00001982  0E                push cs
00001983  E85DEA            call 0x3e3
00001986  803E814501        cmp byte [0x4581],0x1
0000198B  7502              jnz 0x198f
0000198D  EB13              jmp short 0x19a2
0000198F  9AA4192D19        call 0x192d:0x19a4
00001994  9A041A2D19        call 0x192d:0x1a04
00001999  9A901B2D19        call 0x192d:0x1b90
0000199E  0BC0              or ax,ax
000019A0  74ED              jz 0x198f
000019A2  5D                pop bp
000019A3  CB                retf

; ---- 192D:19A4 ----
000019A4  55                push bp
000019A5  8BEC              mov bp,sp
000019A7  B80A00            mov ax,0xa
000019AA  50                push ax
000019AB  1E                push ds
000019AC  B8203C            mov ax,0x3c20
000019AF  50                push ax
000019B0  B81E00            mov ax,0x1e
000019B3  50                push ax
000019B4  B86400            mov ax,0x64
000019B7  50                push ax
000019B8  0E                push cs
000019B9  E8F3FC            call 0x16af
000019BC  83C40A            add sp,byte +0xa
000019BF  9A1B00960A        call 0xa96:0x1b
000019C4  C606AA3700        mov byte [0x37aa],0x0
000019C9  EB0F              jmp short 0x19da
000019CB  A0AA37            mov al,[0x37aa]
000019CE  98                cbw
000019CF  50                push ax
000019D0  9A061B2D19        call 0x192d:0x1b06
000019D5  59                pop cx
000019D6  FE06AA37          inc byte [0x37aa]
000019DA  803EAA3704        cmp byte [0x37aa],0x4
000019DF  7CEA              jl 0x19cb
000019E1  9AA806960A        call 0xa96:0x6a8
000019E6  FF362E08          push word [0x82e]
000019EA  9A3F0C960A        call 0xa96:0xc3f
000019EF  59                pop cx
000019F0  33C0              xor ax,ax
000019F2  33D2              xor dx,dx
000019F4  50                push ax
000019F5  52                push dx
000019F6  9A6C02FD21        call 0x21fd:0x26c
000019FB  59                pop cx
000019FC  59                pop cx
000019FD  9A5303FD21        call 0x21fd:0x353
00001A02  5D                pop bp
00001A03  CB                retf

; ---- 192D:1A04 ----
00001A04  55                push bp
00001A05  8BEC              mov bp,sp
00001A07  4C                dec sp
00001A08  4C                dec sp
00001A09  C646FF00          mov byte [bp-0x1],0x0
00001A0D  9A1B00960A        call 0xa96:0x1b
00001A12  E99E00            jmp 0x1ab3
00001A15  9A1F05FD21        call 0x21fd:0x51f
00001A1A  9AA806960A        call 0xa96:0x6a8
00001A1F  9A1B00960A        call 0xa96:0x1b
00001A24  9A4502FD21        call 0x21fd:0x245
00001A29  803E823D00        cmp byte [0x3d82],0x0
00001A2E  7412              jz 0x1a42
00001A30  33C0              xor ax,ax
00001A32  50                push ax
00001A33  9ACE1A2D19        call 0x192d:0x1ace
00001A38  59                pop cx
00001A39  EB00              jmp short 0x1a3b
00001A3B  803E823D00        cmp byte [0x3d82],0x0
00001A40  75F9              jnz 0x1a3b
00001A42  803E833D00        cmp byte [0x3d83],0x0
00001A47  7413              jz 0x1a5c
00001A49  B80100            mov ax,0x1
00001A4C  50                push ax
00001A4D  9ACE1A2D19        call 0x192d:0x1ace
00001A52  59                pop cx
00001A53  EB00              jmp short 0x1a55
00001A55  803E833D00        cmp byte [0x3d83],0x0
00001A5A  75F9              jnz 0x1a55
00001A5C  803E843D00        cmp byte [0x3d84],0x0
00001A61  7413              jz 0x1a76
00001A63  B80200            mov ax,0x2
00001A66  50                push ax
00001A67  9ACE1A2D19        call 0x192d:0x1ace
00001A6C  59                pop cx
00001A6D  EB00              jmp short 0x1a6f
00001A6F  803E843D00        cmp byte [0x3d84],0x0
00001A74  75F9              jnz 0x1a6f
00001A76  803E853D00        cmp byte [0x3d85],0x0
00001A7B  7413              jz 0x1a90
00001A7D  B80300            mov ax,0x3
00001A80  50                push ax
00001A81  9ACE1A2D19        call 0x192d:0x1ace
00001A86  59                pop cx
00001A87  EB00              jmp short 0x1a89
00001A89  803E853D00        cmp byte [0x3d85],0x0
00001A8E  75F9              jnz 0x1a89
00001A90  803E633D00        cmp byte [0x3d63],0x0
00001A95  740D              jz 0x1aa4
00001A97  C646FF01          mov byte [bp-0x1],0x1
00001A9B  EB00              jmp short 0x1a9d
00001A9D  803E633D00        cmp byte [0x3d63],0x0
00001AA2  75F9              jnz 0x1a9d
00001AA4  9AA806960A        call 0xa96:0x6a8
00001AA9  B80100            mov ax,0x1
00001AAC  50                push ax
00001AAD  9ABA01FD21        call 0x21fd:0x1ba
00001AB2  59                pop cx
00001AB3  807EFF00          cmp byte [bp-0x1],0x0
00001AB7  7503              jnz 0x1abc
00001AB9  E959FF            jmp 0x1a15
00001ABC  B80100            mov ax,0x1
00001ABF  50                push ax
00001AC0  33C0              xor ax,ax
00001AC2  50                push ax
00001AC3  9A5400A721        call 0x21a7:0x54
00001AC8  59                pop cx
00001AC9  59                pop cx
00001ACA  8BE5              mov sp,bp
00001ACC  5D                pop bp
00001ACD  CB                retf

; ---- 192D:1ACE ----
00001ACE  55                push bp
00001ACF  8BEC              mov bp,sp
00001AD1  56                push si
00001AD2  8B7606            mov si,[bp+0x6]
00001AD5  80BC400801        cmp byte [si+0x840],0x1
00001ADA  7502              jnz 0x1ade
00001ADC  EB25              jmp short 0x1b03
00001ADE  FE843C08          inc byte [si+0x83c]
00001AE2  80BC3C0805        cmp byte [si+0x83c],0x5
00001AE7  7E05              jng 0x1aee
00001AE9  C6843C0800        mov byte [si+0x83c],0x0
00001AEE  56                push si
00001AEF  9A061B2D19        call 0x192d:0x1b06
00001AF4  59                pop cx
00001AF5  B80200            mov ax,0x2
00001AF8  50                push ax
00001AF9  33C0              xor ax,ax
00001AFB  50                push ax
00001AFC  9A5400A721        call 0x21a7:0x54
00001B01  59                pop cx
00001B02  59                pop cx
00001B03  5E                pop si
00001B04  5D                pop bp
00001B05  CB                retf

; ---- 192D:1B06 ----
00001B06  55                push bp
00001B07  8BEC              mov bp,sp
00001B09  56                push si
00001B0A  8B7606            mov si,[bp+0x6]
00001B0D  80BC400801        cmp byte [si+0x840],0x1
00001B12  7523              jnz 0x1b37
00001B14  B00A              mov al,0xa
00001B16  50                push ax
00001B17  FF363E3C          push word [0x3c3e]
00001B1B  FF363C3C          push word [0x3c3c]
00001B1F  8BC6              mov ax,si
00001B21  BA0700            mov dx,0x7
00001B24  F7EA              imul dx
00001B26  053200            add ax,0x32
00001B29  50                push ax
00001B2A  B85800            mov ax,0x58
00001B2D  50                push ax
00001B2E  0E                push cs
00001B2F  E8C7FC            call 0x17f9
00001B32  83C40A            add sp,byte +0xa
00001B35  EB56              jmp short 0x1b8d
00001B37  80BC3C0805        cmp byte [si+0x83c],0x5
00001B3C  742E              jz 0x1b6c
00001B3E  B00E              mov al,0xe
00001B40  50                push ax
00001B41  8A843C08          mov al,[si+0x83c]
00001B45  98                cbw
00001B46  D1E0              shl ax,1
00001B48  D1E0              shl ax,1
00001B4A  8BD8              mov bx,ax
00001B4C  FFB7423C          push word [bx+0x3c42]
00001B50  FFB7403C          push word [bx+0x3c40]
00001B54  8BC6              mov ax,si
00001B56  BA0700            mov dx,0x7
00001B59  F7EA              imul dx
00001B5B  053200            add ax,0x32
00001B5E  50                push ax
00001B5F  B85800            mov ax,0x58
00001B62  50                push ax
00001B63  0E                push cs
00001B64  E892FC            call 0x17f9
00001B67  83C40A            add sp,byte +0xa
00001B6A  EB21              jmp short 0x1b8d
00001B6C  B00E              mov al,0xe
00001B6E  50                push ax
00001B6F  FF367E3C          push word [0x3c7e]
00001B73  FF367C3C          push word [0x3c7c]
00001B77  8BC6              mov ax,si
00001B79  BA0700            mov dx,0x7
00001B7C  F7EA              imul dx
00001B7E  053200            add ax,0x32
00001B81  50                push ax
00001B82  B85800            mov ax,0x58
00001B85  50                push ax
00001B86  0E                push cs
00001B87  E86FFC            call 0x17f9
00001B8A  83C40A            add sp,byte +0xa
00001B8D  5E                pop si
00001B8E  5D                pop bp
00001B8F  CB                retf

; ---- 192D:1B90 ----
00001B90  55                push bp
00001B91  8BEC              mov bp,sp
00001B93  4C                dec sp
00001B94  4C                dec sp
00001B95  56                push si
00001B96  57                push di
00001B97  33F6              xor si,si
00001B99  E9BA01            jmp 0x1d56
00001B9C  80BC400801        cmp byte [si+0x840],0x1
00001BA1  7503              jnz 0x1ba6
00001BA3  E9AF01            jmp 0x1d55
00001BA6  80BC3C0800        cmp byte [si+0x83c],0x0
00001BAB  7505              jnz 0x1bb2
00001BAD  BF0100            mov di,0x1
00001BB0  EB0F              jmp short 0x1bc1
00001BB2  80BC3C0801        cmp byte [si+0x83c],0x1
00001BB7  7505              jnz 0x1bbe
00001BB9  BF0200            mov di,0x2
00001BBC  EB03              jmp short 0x1bc1
00001BBE  E99401            jmp 0x1d55
00001BC1  83FF01            cmp di,byte +0x1
00001BC4  7507              jnz 0x1bcd
00001BC6  C746FE2501        mov word [bp-0x2],0x125
00001BCB  EB05              jmp short 0x1bd2
00001BCD  C746FE2601        mov word [bp-0x2],0x126
00001BD2  B82800            mov ax,0x28
00001BD5  50                push ax
00001BD6  1E                push ds
00001BD7  B8543C            mov ax,0x3c54
00001BDA  50                push ax
00001BDB  B81E00            mov ax,0x1e
00001BDE  50                push ax
00001BDF  B86400            mov ax,0x64
00001BE2  50                push ax
00001BE3  0E                push cs
00001BE4  E8C8FA            call 0x16af
00001BE7  83C40A            add sp,byte +0xa
00001BEA  9A1B00960A        call 0xa96:0x1b
00001BEF  B00A              mov al,0xa
00001BF1  50                push ax
00001BF2  8B5EFE            mov bx,[bp-0x2]
00001BF5  D1E3              shl bx,1
00001BF7  D1E3              shl bx,1
00001BF9  FFB7CE37          push word [bx+0x37ce]
00001BFD  FFB7CC37          push word [bx+0x37cc]
00001C01  B82800            mov ax,0x28
00001C04  50                push ax
00001C05  8B5EFE            mov bx,[bp-0x2]
00001C08  D1E3              shl bx,1
00001C0A  D1E3              shl bx,1
00001C0C  FFB7CE37          push word [bx+0x37ce]
00001C10  FFB7CC37          push word [bx+0x37cc]
00001C14  9AE5420000        call 0x0:0x42e5
00001C19  59                pop cx
00001C1A  59                pop cx
00001C1B  BA0600            mov dx,0x6
00001C1E  F7EA              imul dx
00001C20  D1E8              shr ax,1
00001C22  BA5A00            mov dx,0x5a
00001C25  2BD0              sub dx,ax
00001C27  52                push dx
00001C28  0E                push cs
00001C29  E8CDFB            call 0x17f9
00001C2C  83C40A            add sp,byte +0xa
00001C2F  B00A              mov al,0xa
00001C31  50                push ax
00001C32  FF36763C          push word [0x3c76]
00001C36  FF36743C          push word [0x3c74]
00001C3A  B84600            mov ax,0x46
00001C3D  50                push ax
00001C3E  B82800            mov ax,0x28
00001C41  50                push ax
00001C42  0E                push cs
00001C43  E8B3FB            call 0x17f9
00001C46  83C40A            add sp,byte +0xa
00001C49  B014              mov al,0x14
00001C4B  50                push ax
00001C4C  FF366A3C          push word [0x3c6a]
00001C50  FF36683C          push word [0x3c68]
00001C54  B83700            mov ax,0x37
00001C57  50                push ax
00001C58  B82800            mov ax,0x28
00001C5B  50                push ax
00001C5C  0E                push cs
00001C5D  E899FB            call 0x17f9
00001C60  83C40A            add sp,byte +0xa
00001C63  9AA806960A        call 0xa96:0x6a8
00001C68  FF362E08          push word [0x82e]
00001C6C  9A3F0C960A        call 0xa96:0xc3f
00001C71  59                pop cx
00001C72  33C0              xor ax,ax
00001C74  33D2              xor dx,dx
00001C76  50                push ax
00001C77  52                push dx
00001C78  9A6C02FD21        call 0x21fd:0x26c
00001C7D  59                pop cx
00001C7E  59                pop cx
00001C7F  9A5303FD21        call 0x21fd:0x353
00001C84  9A1F05FD21        call 0x21fd:0x51f
00001C89  33C0              xor ax,ax
00001C8B  50                push ax
00001C8C  57                push di
00001C8D  9AE600C627        call 0x27c6:0xe6
00001C92  59                pop cx
00001C93  59                pop cx
00001C94  0BC0              or ax,ax
00001C96  7405              jz 0x1c9d
00001C98  33C0              xor ax,ax
00001C9A  E9C600            jmp 0x1d63
00001C9D  B80200            mov ax,0x2
00001CA0  50                push ax
00001CA1  33C0              xor ax,ax
00001CA3  50                push ax
00001CA4  9A5400A721        call 0x21a7:0x54
00001CA9  59                pop cx
00001CAA  59                pop cx
00001CAB  9A1B00960A        call 0xa96:0x1b
00001CB0  B014              mov al,0x14
00001CB2  50                push ax
00001CB3  FF366E3C          push word [0x3c6e]
00001CB7  FF366C3C          push word [0x3c6c]
00001CBB  B83700            mov ax,0x37
00001CBE  50                push ax
00001CBF  B82800            mov ax,0x28
00001CC2  50                push ax
00001CC3  0E                push cs
00001CC4  E832FB            call 0x17f9
00001CC7  83C40A            add sp,byte +0xa
00001CCA  9AA806960A        call 0xa96:0x6a8
00001CCF  FF362E08          push word [0x82e]
00001CD3  9A3F0C960A        call 0xa96:0xc3f
00001CD8  59                pop cx
00001CD9  9A1F05FD21        call 0x21fd:0x51f
00001CDE  B80100            mov ax,0x1
00001CE1  50                push ax
00001CE2  57                push di
00001CE3  9AE600C627        call 0x27c6:0xe6
00001CE8  59                pop cx
00001CE9  59                pop cx
00001CEA  0BC0              or ax,ax
00001CEC  7404              jz 0x1cf2
00001CEE  33C0              xor ax,ax
00001CF0  EB71              jmp short 0x1d63
00001CF2  B80200            mov ax,0x2
00001CF5  50                push ax
00001CF6  33C0              xor ax,ax
00001CF8  50                push ax
00001CF9  9A5400A721        call 0x21a7:0x54
00001CFE  59                pop cx
00001CFF  59                pop cx
00001D00  9A1B00960A        call 0xa96:0x1b
00001D05  B014              mov al,0x14
00001D07  50                push ax
00001D08  FF36723C          push word [0x3c72]
00001D0C  FF36703C          push word [0x3c70]
00001D10  B83700            mov ax,0x37
00001D13  50                push ax
00001D14  B82800            mov ax,0x28
00001D17  50                push ax
00001D18  0E                push cs
00001D19  E8DDFA            call 0x17f9
00001D1C  83C40A            add sp,byte +0xa
00001D1F  9AA806960A        call 0xa96:0x6a8
00001D24  FF362E08          push word [0x82e]
00001D28  9A3F0C960A        call 0xa96:0xc3f
00001D2D  59                pop cx
00001D2E  9A1F05FD21        call 0x21fd:0x51f
00001D33  B80200            mov ax,0x2
00001D36  50                push ax
00001D37  57                push di
00001D38  9AE600C627        call 0x27c6:0xe6
00001D3D  59                pop cx
00001D3E  59                pop cx
00001D3F  0BC0              or ax,ax
00001D41  7404              jz 0x1d47
00001D43  33C0              xor ax,ax
00001D45  EB1C              jmp short 0x1d63
00001D47  B80200            mov ax,0x2
00001D4A  50                push ax
00001D4B  33C0              xor ax,ax
00001D4D  50                push ax
00001D4E  9A5400A721        call 0x21a7:0x54
00001D53  59                pop cx
00001D54  59                pop cx
00001D55  46                inc si
00001D56  83FE04            cmp si,byte +0x4
00001D59  7D03              jnl 0x1d5e
00001D5B  E93EFE            jmp 0x1b9c
00001D5E  B8                db 0xb8
00001D5F  01                db 0x01
