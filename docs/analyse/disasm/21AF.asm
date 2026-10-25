; segment 21AF  file offset 0x250F0  size 0x4E0
00000000  EB09              jmp short 0xb
00000002  FF7608            push word [bp+0x8]
00000005  9ACC005028        call 0x2850:0xcc
0000000A  59                pop cx
0000000B  5D                pop bp
0000000C  CB                retf

; ---- 21AF:000D ----
0000000D  55                push bp
0000000E  8BEC              mov bp,sp
00000010  56                push si
00000011  33F6              xor si,si
00000013  E9A900            jmp 0xbf
00000016  8BC6              mov ax,si
00000018  BA5400            mov dx,0x54
0000001B  F7EA              imul dx
0000001D  05A634            add ax,0x34a6
00000020  50                push ax
00000021  8BC6              mov ax,si
00000023  BA1A00            mov dx,0x1a
00000026  F7EA              imul dx
00000028  8BD8              mov bx,ax
0000002A  58                pop ax
0000002B  89871033          mov [bx+0x3310],ax
0000002F  8C9F1233          mov [bx+0x3312],ds
00000033  8BC6              mov ax,si
00000035  BA5400            mov dx,0x54
00000038  F7EA              imul dx
0000003A  056830            add ax,0x3068
0000003D  50                push ax
0000003E  8BC6              mov ax,si
00000040  BA1A00            mov dx,0x1a
00000043  F7EA              imul dx
00000045  8BD8              mov bx,ax
00000047  58                pop ax
00000048  89871433          mov [bx+0x3314],ax
0000004C  8C9F1633          mov [bx+0x3316],ds
00000050  8BC6              mov ax,si
00000052  D1E0              shl ax,1
00000054  D1E0              shl ax,1
00000056  D1E0              shl ax,1
00000058  053834            add ax,0x3438
0000005B  50                push ax
0000005C  8BC6              mov ax,si
0000005E  BA1A00            mov dx,0x1a
00000061  F7EA              imul dx
00000063  8BD8              mov bx,ax
00000065  58                pop ax
00000066  89871833          mov [bx+0x3318],ax
0000006A  8C9F1A33          mov [bx+0x331a],ds
0000006E  8BC6              mov ax,si
00000070  D1E0              shl ax,1
00000072  D1E0              shl ax,1
00000074  D1E0              shl ax,1
00000076  05E433            add ax,0x33e4
00000079  50                push ax
0000007A  8BC6              mov ax,si
0000007C  BA1A00            mov dx,0x1a
0000007F  F7EA              imul dx
00000081  8BD8              mov bx,ax
00000083  58                pop ax
00000084  89871C33          mov [bx+0x331c],ax
00000088  8C9F1E33          mov [bx+0x331e],ds
0000008C  8BDE              mov bx,si
0000008E  D1E3              shl bx,1
00000090  D1E3              shl bx,1
00000092  D1E3              shl bx,1
00000094  33C0              xor ax,ax
00000096  33D2              xor dx,dx
00000098  8997E833          mov [bx+0x33e8],dx
0000009C  8987EA33          mov [bx+0x33ea],ax
000000A0  8BDE              mov bx,si
000000A2  D1E3              shl bx,1
000000A4  D1E3              shl bx,1
000000A6  D1E3              shl bx,1
000000A8  8997E433          mov [bx+0x33e4],dx
000000AC  8987E633          mov [bx+0x33e6],ax
000000B0  8BC6              mov ax,si
000000B2  BA1A00            mov dx,0x1a
000000B5  F7EA              imul dx
000000B7  8BD8              mov bx,ax
000000B9  C6870C3300        mov byte [bx+0x330c],0x0
000000BE  46                inc si
000000BF  83FE08            cmp si,byte +0x8
000000C2  7D03              jnl 0xc7
000000C4  E94FFF            jmp 0x16
000000C7  5E                pop si
000000C8  5D                pop bp
000000C9  CB                retf

; ---- 21AF:00CA ----
000000CA  55                push bp
000000CB  8BEC              mov bp,sp
000000CD  83EC08            sub sp,byte +0x8
000000D0  56                push si
000000D1  57                push di
000000D2  8B7E0C            mov di,[bp+0xc]
000000D5  8B360E09          mov si,[0x90e]
000000D9  B90400            mov cx,0x4
000000DC  BB1B02            mov bx,0x21b
000000DF  2E3937            cmp [cs:bx],si
000000E2  7406              jz 0xea
000000E4  43                inc bx
000000E5  43                inc bx
000000E6  E2F7              loop 0xdf
000000E8  EB79              jmp short 0x163
000000EA  2EFF6708          jmp [cs:bx+0x8]
000000EE  8BC7              mov ax,di
000000F0  BAA000            mov dx,0xa0
000000F3  F7EA              imul dx
000000F5  50                push ax
000000F6  8B460A            mov ax,[bp+0xa]
000000F9  BB0800            mov bx,0x8
000000FC  99                cwd
000000FD  F7FB              idiv bx
000000FF  8BD0              mov dx,ax
00000101  58                pop ax
00000102  03C2              add ax,dx
00000104  050400            add ax,0x4
00000107  99                cwd
00000108  8946FC            mov [bp-0x4],ax
0000010B  8956FE            mov [bp-0x2],dx
0000010E  EB53              jmp short 0x163
00000110  8BC7              mov ax,di
00000112  99                cwd
00000113  50                push ax
00000114  52                push dx
00000115  33D2              xor dx,dx
00000117  B84001            mov ax,0x140
0000011A  59                pop cx
0000011B  5B                pop bx
0000011C  9A4E4F0000        call 0x0:0x4f4e
00000121  50                push ax
00000122  8B460A            mov ax,[bp+0xa]
00000125  BB0400            mov bx,0x4
00000128  52                push dx
00000129  99                cwd
0000012A  F7FB              idiv bx
0000012C  99                cwd
0000012D  5B                pop bx
0000012E  59                pop cx
0000012F  03C8              add cx,ax
00000131  13DA              adc bx,dx
00000133  83C104            add cx,byte +0x4
00000136  83D300            adc bx,byte +0x0
00000139  894EFC            mov [bp-0x4],cx
0000013C  895EFE            mov [bp-0x2],bx
0000013F  EB22              jmp short 0x163
00000141  8BC7              mov ax,di
00000143  BAA000            mov dx,0xa0
00000146  F7EA              imul dx
00000148  50                push ax
00000149  8B460A            mov ax,[bp+0xa]
0000014C  BB0200            mov bx,0x2
0000014F  99                cwd
00000150  F7FB              idiv bx
00000152  8BD0              mov dx,ax
00000154  58                pop ax
00000155  03C2              add ax,dx
00000157  050400            add ax,0x4
0000015A  99                cwd
0000015B  8946FC            mov [bp-0x4],ax
0000015E  8956FE            mov [bp-0x2],dx
00000161  EB00              jmp short 0x163
00000163  833E0E0901        cmp word [0x90e],byte +0x1
00000168  753B              jnz 0x1a5
0000016A  8B16D845          mov dx,[0x45d8]
0000016E  A1D645            mov ax,[0x45d6]
00000171  8B4EFE            mov cx,[bp-0x2]
00000174  8B5EFC            mov bx,[bp-0x4]
00000177  9A01510000        call 0x0:0x5101
0000017C  8946F8            mov [bp-0x8],ax
0000017F  8956FA            mov [bp-0x6],dx
00000182  8B4606            mov ax,[bp+0x6]
00000185  BA5400            mov dx,0x54
00000188  F7EA              imul dx
0000018A  8B5608            mov dx,[bp+0x8]
0000018D  D1E2              shl dx,1
0000018F  D1E2              shl dx,1
00000191  03C2              add ax,dx
00000193  8B56FA            mov dx,[bp-0x6]
00000196  8B5EF8            mov bx,[bp-0x8]
00000199  87D8              xchg bx,ax
0000019B  8987A634          mov [bx+0x34a6],ax
0000019F  8997A834          mov [bx+0x34a8],dx
000001A3  EB26              jmp short 0x1cb
000001A5  8B4606            mov ax,[bp+0x6]
000001A8  BA5400            mov dx,0x54
000001AB  F7EA              imul dx
000001AD  8B5608            mov dx,[bp+0x8]
000001B0  D1E2              shl dx,1
000001B2  D1E2              shl dx,1
000001B4  03C2              add ax,dx
000001B6  8B16D845          mov dx,[0x45d8]
000001BA  8B1ED645          mov bx,[0x45d6]
000001BE  035EFC            add bx,[bp-0x4]
000001C1  87D8              xchg bx,ax
000001C3  8987A634          mov [bx+0x34a6],ax
000001C7  8997A834          mov [bx+0x34a8],dx
000001CB  8B4610            mov ax,[bp+0x10]
000001CE  BA1400            mov dx,0x14
000001D1  F7EA              imul dx
000001D3  50                push ax
000001D4  8B460E            mov ax,[bp+0xe]
000001D7  BB1000            mov bx,0x10
000001DA  99                cwd
000001DB  F7FB              idiv bx
000001DD  8BD0              mov dx,ax
000001DF  58                pop ax
000001E0  03C2              add ax,dx
000001E2  99                cwd
000001E3  8946FC            mov [bp-0x4],ax
000001E6  8956FE            mov [bp-0x2],dx
000001E9  8B4606            mov ax,[bp+0x6]
000001EC  BA5400            mov dx,0x54
000001EF  F7EA              imul dx
000001F1  8B5608            mov dx,[bp+0x8]
000001F4  D1E2              shl dx,1
000001F6  D1E2              shl dx,1
000001F8  03C2              add ax,dx
000001FA  8B56FC            mov dx,[bp-0x4]
000001FD  D1E2              shl dx,1
000001FF  8B1EC84B          mov bx,[0x4bc8]
00000203  8B0EC64B          mov cx,[0x4bc6]
00000207  03CA              add cx,dx
00000209  53                push bx
0000020A  8BD8              mov bx,ax
0000020C  898F6830          mov [bx+0x3068],cx
00000210  58                pop ax
00000211  89876A30          mov [bx+0x306a],ax
00000215  5F                pop di
00000216  5E                pop si
00000217  8BE5              mov sp,bp
00000219  5D                pop bp
0000021A  CB                retf
0000021B  0000              add [bx+si],al
0000021D  0100              add [bx+si],ax
0000021F  0200              add al,[bx+si]
00000221  6300              arpl [bx+si],ax
00000223  EE                out dx,al
00000224  0010              add [bx+si],dl
00000226  014101            add [bx+di+0x1],ax
00000229  41                inc cx
0000022A  01558B            add [di-0x75],dx
0000022D  EC                in al,dx
0000022E  83EC12            sub sp,byte +0x12
00000231  56                push si
00000232  57                push di
00000233  8B7E08            mov di,[bp+0x8]
00000236  8B760C            mov si,[bp+0xc]
00000239  A10E09            mov ax,[0x90e]
0000023C  8946EE            mov [bp-0x12],ax
0000023F  B90400            mov cx,0x4
00000242  BB0204            mov bx,0x402
00000245  2E8B07            mov ax,[cs:bx]
00000248  3B46EE            cmp ax,[bp-0x12]
0000024B  7407              jz 0x254
0000024D  43                inc bx
0000024E  43                inc bx
0000024F  E2F4              loop 0x245
00000251  E90B01            jmp 0x35f
00000254  2EFF6708          jmp [cs:bx+0x8]
00000258  8B460A            mov ax,[bp+0xa]
0000025B  BAA000            mov dx,0xa0
0000025E  F7EA              imul dx
00000260  50                push ax
00000261  8BC7              mov ax,di
00000263  BB0800            mov bx,0x8
00000266  99                cwd
00000267  F7FB              idiv bx
00000269  8BD0              mov dx,ax
0000026B  58                pop ax
0000026C  03C2              add ax,dx
0000026E  050400            add ax,0x4
00000271  99                cwd
00000272  8946FC            mov [bp-0x4],ax
00000275  8956FE            mov [bp-0x2],dx
00000278  8B460E            mov ax,[bp+0xe]
0000027B  BAA000            mov dx,0xa0
0000027E  F7EA              imul dx
00000280  50                push ax
00000281  8BC6              mov ax,si
00000283  BB0800            mov bx,0x8
00000286  99                cwd
00000287  F7FB              idiv bx
00000289  8BD0              mov dx,ax
0000028B  58                pop ax
0000028C  03C2              add ax,dx
0000028E  050400            add ax,0x4
00000291  99                cwd
00000292  8946F8            mov [bp-0x8],ax
00000295  8956FA            mov [bp-0x6],dx
00000298  E9C400            jmp 0x35f
0000029B  8BC7              mov ax,di
0000029D  050300            add ax,0x3
000002A0  BB0400            mov bx,0x4
000002A3  99                cwd
000002A4  F7FB              idiv bx
000002A6  D1E0              shl ax,1
000002A8  D1E0              shl ax,1
000002AA  8BF8              mov di,ax
000002AC  8BC6              mov ax,si
000002AE  050300            add ax,0x3
000002B1  BB0400            mov bx,0x4
000002B4  99                cwd
000002B5  F7FB              idiv bx
000002B7  D1E0              shl ax,1
000002B9  D1E0              shl ax,1
000002BB  8BF0              mov si,ax
000002BD  8B460A            mov ax,[bp+0xa]
000002C0  99                cwd
000002C1  50                push ax
000002C2  52                push dx
000002C3  33D2              xor dx,dx
000002C5  B84001            mov ax,0x140
000002C8  59                pop cx
000002C9  5B                pop bx
000002CA  9A4E4F0000        call 0x0:0x4f4e
000002CF  50                push ax
000002D0  8BC7              mov ax,di
000002D2  BB0400            mov bx,0x4
000002D5  52                push dx
000002D6  99                cwd
000002D7  F7FB              idiv bx
000002D9  99                cwd
000002DA  5B                pop bx
000002DB  59                pop cx
000002DC  03C8              add cx,ax
000002DE  13DA              adc bx,dx
000002E0  83C104            add cx,byte +0x4
000002E3  83D300            adc bx,byte +0x0
000002E6  894EFC            mov [bp-0x4],cx
000002E9  895EFE            mov [bp-0x2],bx
000002EC  8B460E            mov ax,[bp+0xe]
000002EF  99                cwd
000002F0  50                push ax
000002F1  52                push dx
000002F2  33D2              xor dx,dx
000002F4  B84001            mov ax,0x140
000002F7  59                pop cx
000002F8  5B                pop bx
000002F9  9A4E4F0000        call 0x0:0x4f4e
000002FE  50                push ax
000002FF  8BC6              mov ax,si
00000301  BB0400            mov bx,0x4
00000304  52                push dx
00000305  99                cwd
00000306  F7FB              idiv bx
00000308  99                cwd
00000309  5B                pop bx
0000030A  59                pop cx
0000030B  03C8              add cx,ax
0000030D  13DA              adc bx,dx
0000030F  83C104            add cx,byte +0x4
00000312  83D300            adc bx,byte +0x0
00000315  894EF8            mov [bp-0x8],cx
00000318  895EFA            mov [bp-0x6],bx
0000031B  EB42              jmp short 0x35f
0000031D  8B460A            mov ax,[bp+0xa]
00000320  BAA000            mov dx,0xa0
00000323  F7EA              imul dx
00000325  50                push ax
00000326  8BC7              mov ax,di
00000328  BB0200            mov bx,0x2
0000032B  99                cwd
0000032C  F7FB              idiv bx
0000032E  8BD0              mov dx,ax
00000330  58                pop ax
00000331  03C2              add ax,dx
00000333  050400            add ax,0x4
00000336  99                cwd
00000337  8946FC            mov [bp-0x4],ax
0000033A  8956FE            mov [bp-0x2],dx
0000033D  8B460E            mov ax,[bp+0xe]
00000340  BAA000            mov dx,0xa0
00000343  F7EA              imul dx
00000345  50                push ax
00000346  8BC6              mov ax,si
00000348  BB0200            mov bx,0x2
0000034B  99                cwd
0000034C  F7FB              idiv bx
0000034E  8BD0              mov dx,ax
00000350  58                pop ax
00000351  03C2              add ax,dx
00000353  050400            add ax,0x4
00000356  99                cwd
00000357  8946F8            mov [bp-0x8],ax
0000035A  8956FA            mov [bp-0x6],dx
0000035D  EB00              jmp short 0x35f
0000035F  833E0E0901        cmp word [0x90e],byte +0x1
00000364  7560              jnz 0x3c6
00000366  8B16D845          mov dx,[0x45d8]
0000036A  A1D645            mov ax,[0x45d6]
0000036D  8B4EFE            mov cx,[bp-0x2]
00000370  8B5EFC            mov bx,[bp-0x4]
00000373  9A01510000        call 0x0:0x5101
00000378  8946F4            mov [bp-0xc],ax
0000037B  8956F6            mov [bp-0xa],dx
0000037E  8B16D845          mov dx,[0x45d8]
00000382  A1D645            mov ax,[0x45d6]
00000385  8B4EFA            mov cx,[bp-0x6]
00000388  8B5EF8            mov bx,[bp-0x8]
0000038B  9A01510000        call 0x0:0x5101
00000390  8946F0            mov [bp-0x10],ax
00000393  8956F2            mov [bp-0xe],dx
00000396  8B5E06            mov bx,[bp+0x6]
00000399  D1E3              shl bx,1
0000039B  D1E3              shl bx,1
0000039D  D1E3              shl bx,1
0000039F  8B46F6            mov ax,[bp-0xa]
000003A2  8B56F4            mov dx,[bp-0xc]
000003A5  89973834          mov [bx+0x3438],dx
000003A9  89873A34          mov [bx+0x343a],ax
000003AD  8B5E06            mov bx,[bp+0x6]
000003B0  D1E3              shl bx,1
000003B2  D1E3              shl bx,1
000003B4  D1E3              shl bx,1
000003B6  8B46F2            mov ax,[bp-0xe]
000003B9  8B56F0            mov dx,[bp-0x10]
000003BC  89973C34          mov [bx+0x343c],dx
000003C0  89873E34          mov [bx+0x343e],ax
000003C4  EB36              jmp short 0x3fc
000003C6  A1D845            mov ax,[0x45d8]
000003C9  8B16D645          mov dx,[0x45d6]
000003CD  0356FC            add dx,[bp-0x4]
000003D0  8B5E06            mov bx,[bp+0x6]
000003D3  D1E3              shl bx,1
000003D5  D1E3              shl bx,1
000003D7  D1E3              shl bx,1
000003D9  89973834          mov [bx+0x3438],dx
000003DD  89873A34          mov [bx+0x343a],ax
000003E1  A1D845            mov ax,[0x45d8]
000003E4  8B16D645          mov dx,[0x45d6]
000003E8  0356F8            add dx,[bp-0x8]
000003EB  8B5E06            mov bx,[bp+0x6]
000003EE  D1E3              shl bx,1
000003F0  D1E3              shl bx,1
000003F2  D1E3              shl bx,1
000003F4  89973C34          mov [bx+0x343c],dx
000003F8  89873E34          mov [bx+0x343e],ax
000003FC  5F                pop di
000003FD  5E                pop si
000003FE  8BE5              mov sp,bp
00000400  5D                pop bp
00000401  CB                retf
00000402  0000              add [bx+si],al
00000404  0100              add [bx+si],ax
00000406  0200              add al,[bx+si]
00000408  6300              arpl [bx+si],ax
0000040A  58                pop ax
0000040B  029B021D          add bl,[bp+di+0x1d02]
0000040F  031D              add bx,[di]
00000411  03558B            add dx,[di-0x75]
00000414  EC                in al,dx
00000415  56                push si
00000416  8B7606            mov si,[bp+0x6]
00000419  8BC6              mov ax,si
0000041B  BA1A00            mov dx,0x1a
0000041E  F7EA              imul dx
00000420  8B5608            mov dx,[bp+0x8]
00000423  8BD8              mov bx,ax
00000425  89972433          mov [bx+0x3324],dx
00000429  8BDE              mov bx,si
0000042B  D1E3              shl bx,1
0000042D  D1E3              shl bx,1
0000042F  D1E3              shl bx,1
00000431  33C0              xor ax,ax
00000433  33D2              xor dx,dx
00000435  8997E833          mov [bx+0x33e8],dx
00000439  8987EA33          mov [bx+0x33ea],ax
0000043D  8BDE              mov bx,si
0000043F  D1E3              shl bx,1
00000441  D1E3              shl bx,1
00000443  D1E3              shl bx,1
00000445  8997E433          mov [bx+0x33e4],dx
00000449  8987E633          mov [bx+0x33e6],ax
0000044D  8BC6              mov ax,si
0000044F  BA1A00            mov dx,0x1a
00000452  F7EA              imul dx
00000454  8BD8              mov bx,ax
00000456  C6870C3301        mov byte [bx+0x330c],0x1
0000045B  5E                pop si
0000045C  5D                pop bp
0000045D  CB                retf

; ---- 21AF:045E ----
0000045E  55                push bp
0000045F  8BEC              mov bp,sp
00000461  8B4606            mov ax,[bp+0x6]
00000464  BA1A00            mov dx,0x1a
00000467  F7EA              imul dx
00000469  8BD8              mov bx,ax
0000046B  C6870C3300        mov byte [bx+0x330c],0x0
00000470  5D                pop bp
00000471  CB                retf

; ---- 21AF:0472 ----
00000472  55                push bp
00000473  8BEC              mov bp,sp
00000475  56                push si
00000476  33F6              xor si,si
00000478  EB07              jmp short 0x481
0000047A  56                push si
0000047B  0E                push cs
0000047C  E8DFFF            call 0x45e
0000047F  59                pop cx
00000480  46                inc si
00000481  83FE08            cmp si,byte +0x8
00000484  7CF4              jl 0x47a
00000486  5E                pop si
00000487  5D                pop bp
00000488  CB                retf
00000489  55                push bp
0000048A  8BEC              mov bp,sp
0000048C  8B5E06            mov bx,[bp+0x6]
0000048F  D1E3              shl bx,1
00000491  B001              mov al,0x1
00000493  8887D954          mov [bx+0x54d9],al
00000497  8B5E06            mov bx,[bp+0x6]
0000049A  D1E3              shl bx,1
0000049C  8887D854          mov [bx+0x54d8],al
000004A0  5D                pop bp
000004A1  CB                retf

; ---- 21AF:04A2 ----
000004A2  55                push bp
000004A3  8BEC              mov bp,sp
000004A5  8B5E06            mov bx,[bp+0x6]
000004A8  D1E3              shl bx,1
000004AA  D1E3              shl bx,1
000004AC  D1E3              shl bx,1
000004AE  33C0              xor ax,ax
000004B0  33D2              xor dx,dx
000004B2  8997E833          mov [bx+0x33e8],dx
000004B6  8987EA33          mov [bx+0x33ea],ax
000004BA  8B5E06            mov bx,[bp+0x6]
000004BD  D1E3              shl bx,1
000004BF  D1E3              shl bx,1
000004C1  D1E3              shl bx,1
000004C3  8997E433          mov [bx+0x33e4],dx
000004C7  8987E633          mov [bx+0x33e6],ax
000004CB  5D                pop bp
000004CC  CB                retf

; ---- 21AF:04CD ----
000004CD  55                push bp
000004CE  8BEC              mov bp,sp
000004D0  56                push si
000004D1  33F6              xor si,si
000004D3  EB07              jmp short 0x4dc
000004D5  56                push si
000004D6  0E                push cs
000004D7  E8C8FF            call 0x4a2
000004DA  59                pop cx
000004DB  46                inc si
000004DC  83FE08            cmp si,byte +0x8
000004DF  7C                db 0x7c
