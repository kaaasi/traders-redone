; segment 186C  file offset 0x1BCC0  size 0xC10
00000000  5D                pop bp
00000001  CB                retf

; ---- 186C:0002 ----
00000002  55                push bp
00000003  8BEC              mov bp,sp
00000005  83EC0C            sub sp,byte +0xc
00000008  56                push si
00000009  57                push di
0000000A  8B4606            mov ax,[bp+0x6]
0000000D  BA1300            mov dx,0x13
00000010  F7EA              imul dx
00000012  05CE09            add ax,0x9ce
00000015  8946F8            mov [bp-0x8],ax
00000018  8C5EFA            mov [bp-0x6],ds
0000001B  833E0E0901        cmp word [0x90e],byte +0x1
00000020  7515              jnz 0x37
00000022  BE0003            mov si,0x300
00000025  C45EF8            les bx,[bp-0x8]
00000028  268B4702          mov ax,[es:bx+0x2]
0000002C  268B17            mov dx,[es:bx]
0000002F  8956FC            mov [bp-0x4],dx
00000032  8946FE            mov [bp-0x2],ax
00000035  EB13              jmp short 0x4a
00000037  33F6              xor si,si
00000039  C45EF8            les bx,[bp-0x8]
0000003C  268B4706          mov ax,[es:bx+0x6]
00000040  268B5704          mov dx,[es:bx+0x4]
00000044  8956FC            mov [bp-0x4],dx
00000047  8946FE            mov [bp-0x2],ax
0000004A  C45EF8            les bx,[bp-0x8]
0000004D  268B4711          mov ax,[es:bx+0x11]
00000051  268B570F          mov dx,[es:bx+0xf]
00000055  8956F4            mov [bp-0xc],dx
00000058  8946F6            mov [bp-0xa],ax
0000005B  8B46F4            mov ax,[bp-0xc]
0000005E  0B46F6            or ax,[bp-0xa]
00000061  7525              jnz 0x88
00000063  FF76FA            push word [bp-0x6]
00000066  FF76F8            push word [bp-0x8]
00000069  FF76FE            push word [bp-0x2]
0000006C  FF76FC            push word [bp-0x4]
0000006F  9A97096C18        call 0x186c:0x997
00000074  83C408            add sp,byte +0x8
00000077  C45EF8            les bx,[bp-0x8]
0000007A  268B4711          mov ax,[es:bx+0x11]
0000007E  268B570F          mov dx,[es:bx+0xf]
00000082  8956F4            mov [bp-0xc],dx
00000085  8946F6            mov [bp-0xa],ax
00000088  8B4606            mov ax,[bp+0x6]
0000008B  A3C809            mov [0x9c8],ax
0000008E  FF7606            push word [bp+0x6]
00000091  9A6D056C18        call 0x186c:0x56d
00000096  59                pop cx
00000097  8BF8              mov di,ax
00000099  83FFFF            cmp di,byte -0x1
0000009C  7522              jnz 0xc0
0000009E  C45EF8            les bx,[bp-0x8]
000000A1  268A4708          mov al,[es:bx+0x8]
000000A5  98                cbw
000000A6  50                push ax
000000A7  FF76FE            push word [bp-0x2]
000000AA  FF76FC            push word [bp-0x4]
000000AD  9A6C0A6C18        call 0x186c:0xa6c
000000B2  83C406            add sp,byte +0x6
000000B5  FF7606            push word [bp+0x6]
000000B8  9AAF056C18        call 0x186c:0x5af
000000BD  59                pop cx
000000BE  8BF8              mov di,ax
000000C0  83FFFF            cmp di,byte -0x1
000000C3  7413              jz 0xd8
000000C5  FF36D845          push word [0x45d8]
000000C9  FF36D645          push word [0x45d6]
000000CD  57                push di
000000CE  9A58096C18        call 0x186c:0x958
000000D3  83C406            add sp,byte +0x6
000000D6  EB16              jmp short 0xee
000000D8  FF76FE            push word [bp-0x2]
000000DB  FF76FC            push word [bp-0x4]
000000DE  FF36D845          push word [0x45d8]
000000E2  FF36D645          push word [0x45d6]
000000E6  9AB10A6C18        call 0x186c:0xab1
000000EB  83C408            add sp,byte +0x8
000000EE  C45EF8            les bx,[bp-0x8]
000000F1  26FF770D          push word [es:bx+0xd]
000000F5  26FF770B          push word [es:bx+0xb]
000000F9  FF36D845          push word [0x45d8]
000000FD  FF36D645          push word [0x45d6]
00000101  9AA9016C18        call 0x186c:0x1a9
00000106  83C408            add sp,byte +0x8
00000109  833E0E0901        cmp word [0x90e],byte +0x1
0000010E  7543              jnz 0x153
00000110  8BC6              mov ax,si
00000112  99                cwd
00000113  52                push dx
00000114  50                push ax
00000115  1E                push ds
00000116  B81750            mov ax,0x5017
00000119  50                push ax
0000011A  FF36D845          push word [0x45d8]
0000011E  FF36D645          push word [0x45d6]
00000122  9ACE04C627        call 0x27c6:0x4ce
00000127  83C40C            add sp,byte +0xc
0000012A  8BC6              mov ax,si
0000012C  99                cwd
0000012D  8B5EF6            mov bx,[bp-0xa]
00000130  8B4EF4            mov cx,[bp-0xc]
00000133  2BC8              sub cx,ax
00000135  1BDA              sbb bx,dx
00000137  53                push bx
00000138  51                push cx
00000139  FF36D845          push word [0x45d8]
0000013D  FF36D645          push word [0x45d6]
00000141  A1D645            mov ax,[0x45d6]
00000144  03C6              add ax,si
00000146  FF36D845          push word [0x45d8]
0000014A  50                push ax
0000014B  9ACE04C627        call 0x27c6:0x4ce
00000150  83C40C            add sp,byte +0xc
00000153  833E0E0902        cmp word [0x90e],byte +0x2
00000158  7407              jz 0x161
0000015A  833E0E0963        cmp word [0x90e],byte +0x63
0000015F  751B              jnz 0x17c
00000161  B80001            mov ax,0x100
00000164  50                push ax
00000165  B84001            mov ax,0x140
00000168  50                push ax
00000169  A1D645            mov ax,[0x45d6]
0000016C  050400            add ax,0x4
0000016F  FF36D845          push word [0x45d8]
00000173  50                push ax
00000174  9A6A16CD23        call 0x23cd:0x166a
00000179  83C408            add sp,byte +0x8
0000017C  833E0E0901        cmp word [0x90e],byte +0x1
00000181  751B              jnz 0x19e
00000183  B80001            mov ax,0x100
00000186  50                push ax
00000187  B84001            mov ax,0x140
0000018A  50                push ax
0000018B  A1D645            mov ax,[0x45d6]
0000018E  050400            add ax,0x4
00000191  FF36D845          push word [0x45d8]
00000195  50                push ax
00000196  9A7930CD23        call 0x23cd:0x3079
0000019B  83C408            add sp,byte +0x8
0000019E  9A0D00AF21        call 0x21af:0xd
000001A3  5F                pop di
000001A4  5E                pop si
000001A5  8BE5              mov sp,bp
000001A7  5D                pop bp
000001A8  CB                retf

; ---- 186C:01A9 ----
000001A9  55                push bp
000001AA  8BEC              mov bp,sp
000001AC  83EC04            sub sp,byte +0x4
000001AF  8B5608            mov dx,[bp+0x8]
000001B2  8B4606            mov ax,[bp+0x6]
000001B5  33C9              xor cx,cx
000001B7  BB0400            mov bx,0x4
000001BA  9A01510000        call 0x0:0x5101
000001BF  52                push dx
000001C0  50                push ax
000001C1  8B5608            mov dx,[bp+0x8]
000001C4  8B4606            mov ax,[bp+0x6]
000001C7  33C9              xor cx,cx
000001C9  BB1000            mov bx,0x10
000001CC  9A01510000        call 0x0:0x5101
000001D1  52                push dx
000001D2  50                push ax
000001D3  8B5608            mov dx,[bp+0x8]
000001D6  8B4606            mov ax,[bp+0x6]
000001D9  8B4E0C            mov cx,[bp+0xc]
000001DC  8B5E0A            mov bx,[bp+0xa]
000001DF  9A01510000        call 0x0:0x5101
000001E4  52                push dx
000001E5  50                push ax
000001E6  9A08002128        call 0x2821:0x8
000001EB  83C40C            add sp,byte +0xc
000001EE  8946FC            mov [bp-0x4],ax
000001F1  8956FE            mov [bp-0x2],dx
000001F4  FF76FE            push word [bp-0x2]
000001F7  FF76FC            push word [bp-0x4]
000001FA  FF7608            push word [bp+0x8]
000001FD  FF7606            push word [bp+0x6]
00000200  8B5608            mov dx,[bp+0x8]
00000203  8B4606            mov ax,[bp+0x6]
00000206  33C9              xor cx,cx
00000208  BB1000            mov bx,0x10
0000020B  9A01510000        call 0x0:0x5101
00000210  52                push dx
00000211  50                push ax
00000212  9ACE04C627        call 0x27c6:0x4ce
00000217  83C40C            add sp,byte +0xc
0000021A  8BE5              mov sp,bp
0000021C  5D                pop bp
0000021D  CB                retf

; ---- 186C:021E ----
0000021E  55                push bp
0000021F  8BEC              mov bp,sp
00000221  83EC04            sub sp,byte +0x4
00000224  9A07290000        call 0x0:0x2907
00000229  A31853            mov [0x5318],ax
0000022C  89161A53          mov [0x531a],dx
00000230  812E1853E803      sub word [0x5318],0x3e8
00000236  831E1A5300        sbb word [0x531a],byte +0x0
0000023B  833E0E0902        cmp word [0x90e],byte +0x2
00000240  750A              jnz 0x24c
00000242  832E185300        sub word [0x5318],byte +0x0
00000247  831E1A5301        sbb word [0x531a],byte +0x1
0000024C  FF361A53          push word [0x531a]
00000250  FF361853          push word [0x5318]
00000254  9A7E4D0000        call 0x0:0x4d7e
00000259  59                pop cx
0000025A  59                pop cx
0000025B  8946FC            mov [bp-0x4],ax
0000025E  8956FE            mov [bp-0x2],dx
00000261  8B46FC            mov ax,[bp-0x4]
00000264  0B46FE            or ax,[bp-0x2]
00000267  750A              jnz 0x273
00000269  B80100            mov ax,0x1
0000026C  50                push ax
0000026D  9A0A270000        call 0x0:0x270a
00000272  59                pop cx
00000273  8B46FE            mov ax,[bp-0x2]
00000276  33D2              xor dx,dx
00000278  B104              mov cl,0x4
0000027A  9A37500000        call 0x0:0x5037
0000027F  8B5EFC            mov bx,[bp-0x4]
00000282  33C9              xor cx,cx
00000284  03D8              add bx,ax
00000286  13CA              adc cx,dx
00000288  891E2C53          mov [0x532c],bx
0000028C  890E2E53          mov [0x532e],cx
00000290  A11A53            mov ax,[0x531a]
00000293  8B161853          mov dx,[0x5318]
00000297  89162053          mov [0x5320],dx
0000029B  A32253            mov [0x5322],ax
0000029E  A12E53            mov ax,[0x532e]
000002A1  8B162C53          mov dx,[0x532c]
000002A5  89162853          mov [0x5328],dx
000002A9  A32A53            mov [0x532a],ax
000002AC  A12E53            mov ax,[0x532e]
000002AF  8B162C53          mov dx,[0x532c]
000002B3  03161853          add dx,[0x5318]
000002B7  13061A53          adc ax,[0x531a]
000002BB  89162453          mov [0x5324],dx
000002BF  A32653            mov [0x5326],ax
000002C2  8BE5              mov sp,bp
000002C4  5D                pop bp
000002C5  CB                retf

; ---- 186C:02C6 ----
000002C6  55                push bp
000002C7  8BEC              mov bp,sp
000002C9  83EC04            sub sp,byte +0x4
000002CC  8B4608            mov ax,[bp+0x8]
000002CF  8B5606            mov dx,[bp+0x6]
000002D2  3B062253          cmp ax,[0x5322]
000002D6  7C0E              jl 0x2e6
000002D8  7F06              jg 0x2e0
000002DA  3B162053          cmp dx,[0x5320]
000002DE  7606              jna 0x2e6
000002E0  33D2              xor dx,dx
000002E2  33C0              xor ax,ax
000002E4  EB49              jmp short 0x32f
000002E6  FF7608            push word [bp+0x8]
000002E9  FF7606            push word [bp+0x6]
000002EC  33C0              xor ax,ax
000002EE  50                push ax
000002EF  9A09086C18        call 0x186c:0x809
000002F4  83C406            add sp,byte +0x6
000002F7  A12A53            mov ax,[0x532a]
000002FA  8B162853          mov dx,[0x5328]
000002FE  8956FC            mov [bp-0x4],dx
00000301  8946FE            mov [bp-0x2],ax
00000304  8B4608            mov ax,[bp+0x8]
00000307  8B5606            mov dx,[bp+0x6]
0000030A  01162853          add [0x5328],dx
0000030E  11062A53          adc [0x532a],ax
00000312  8B4608            mov ax,[bp+0x8]
00000315  8B5606            mov dx,[bp+0x6]
00000318  29162053          sub [0x5320],dx
0000031C  19062253          sbb [0x5322],ax
00000320  FF76FE            push word [bp-0x2]
00000323  FF76FC            push word [bp-0x4]
00000326  9A16056C18        call 0x186c:0x516
0000032B  59                pop cx
0000032C  59                pop cx
0000032D  EB00              jmp short 0x32f
0000032F  8BE5              mov sp,bp
00000331  5D                pop bp
00000332  CB                retf
00000333  55                push bp
00000334  8BEC              mov bp,sp
00000336  83EC04            sub sp,byte +0x4
00000339  8B4608            mov ax,[bp+0x8]
0000033C  33D2              xor dx,dx
0000033E  B104              mov cl,0x4
00000340  9A37500000        call 0x0:0x5037
00000345  8B5E06            mov bx,[bp+0x6]
00000348  33C9              xor cx,cx
0000034A  03D8              add bx,ax
0000034C  13CA              adc cx,dx
0000034E  895EFC            mov [bp-0x4],bx
00000351  894EFE            mov [bp-0x2],cx
00000354  8B46FE            mov ax,[bp-0x2]
00000357  8B56FC            mov dx,[bp-0x4]
0000035A  3B062653          cmp ax,[0x5326]
0000035E  771C              ja 0x37c
00000360  7506              jnz 0x368
00000362  3B162453          cmp dx,[0x5324]
00000366  7714              ja 0x37c
00000368  8B46FE            mov ax,[bp-0x2]
0000036B  8B56FC            mov dx,[bp-0x4]
0000036E  3B062A53          cmp ax,[0x532a]
00000372  720A              jc 0x37e
00000374  7706              ja 0x37c
00000376  3B162853          cmp dx,[0x5328]
0000037A  7602              jna 0x37e
0000037C  EB5A              jmp short 0x3d8
0000037E  8B46FE            mov ax,[bp-0x2]
00000381  8B56FC            mov dx,[bp-0x4]
00000384  3B062E53          cmp ax,[0x532e]
00000388  770A              ja 0x394
0000038A  7206              jc 0x392
0000038C  3B162C53          cmp dx,[0x532c]
00000390  7302              jnc 0x394
00000392  EB44              jmp short 0x3d8
00000394  A12A53            mov ax,[0x532a]
00000397  8B162853          mov dx,[0x5328]
0000039B  2B56FC            sub dx,[bp-0x4]
0000039E  1B46FE            sbb ax,[bp-0x2]
000003A1  8B1E2253          mov bx,[0x5322]
000003A5  8B0E2053          mov cx,[0x5320]
000003A9  03CA              add cx,dx
000003AB  13D8              adc bx,ax
000003AD  890E2053          mov [0x5320],cx
000003B1  891E2253          mov [0x5322],bx
000003B5  A12A53            mov ax,[0x532a]
000003B8  8B162853          mov dx,[0x5328]
000003BC  2B56FC            sub dx,[bp-0x4]
000003BF  1B46FE            sbb ax,[bp-0x2]
000003C2  50                push ax
000003C3  52                push dx
000003C4  9AD9086C18        call 0x186c:0x8d9
000003C9  59                pop cx
000003CA  59                pop cx
000003CB  8B46FE            mov ax,[bp-0x2]
000003CE  8B56FC            mov dx,[bp-0x4]
000003D1  89162853          mov [0x5328],dx
000003D5  A32A53            mov [0x532a],ax
000003D8  8BE5              mov sp,bp
000003DA  5D                pop bp
000003DB  CB                retf
000003DC  55                push bp
000003DD  8BEC              mov bp,sp
000003DF  83EC0C            sub sp,byte +0xc
000003E2  8B4608            mov ax,[bp+0x8]
000003E5  33D2              xor dx,dx
000003E7  B104              mov cl,0x4
000003E9  9A37500000        call 0x0:0x5037
000003EE  8B5E06            mov bx,[bp+0x6]
000003F1  33C9              xor cx,cx
000003F3  03D8              add bx,ax
000003F5  13CA              adc cx,dx
000003F7  895EFC            mov [bp-0x4],bx
000003FA  894EFE            mov [bp-0x2],cx
000003FD  8B46FE            mov ax,[bp-0x2]
00000400  8B56FC            mov dx,[bp-0x4]
00000403  3B062653          cmp ax,[0x5326]
00000407  771C              ja 0x425
00000409  7506              jnz 0x411
0000040B  3B162453          cmp dx,[0x5324]
0000040F  7714              ja 0x425
00000411  8B46FE            mov ax,[bp-0x2]
00000414  8B56FC            mov dx,[bp-0x4]
00000417  3B062A53          cmp ax,[0x532a]
0000041B  720D              jc 0x42a
0000041D  7706              ja 0x425
0000041F  3B162853          cmp dx,[0x5328]
00000423  7605              jna 0x42a
00000425  33C0              xor ax,ax
00000427  E9E800            jmp 0x512
0000042A  8B46FE            mov ax,[bp-0x2]
0000042D  8B56FC            mov dx,[bp-0x4]
00000430  3B062E53          cmp ax,[0x532e]
00000434  770D              ja 0x443
00000436  7206              jc 0x43e
00000438  3B162C53          cmp dx,[0x532c]
0000043C  7305              jnc 0x443
0000043E  33C0              xor ax,ax
00000440  E9CF00            jmp 0x512
00000443  8B460C            mov ax,[bp+0xc]
00000446  8B560A            mov dx,[bp+0xa]
00000449  3B062253          cmp ax,[0x5322]
0000044D  7C0D              jl 0x45c
0000044F  7F06              jg 0x457
00000451  3B162053          cmp dx,[0x5320]
00000455  7605              jna 0x45c
00000457  33C0              xor ax,ax
00000459  E9B600            jmp 0x512
0000045C  A12A53            mov ax,[0x532a]
0000045F  8B162853          mov dx,[0x5328]
00000463  2B56FC            sub dx,[bp-0x4]
00000466  1B46FE            sbb ax,[bp-0x2]
00000469  8956F4            mov [bp-0xc],dx
0000046C  8946F6            mov [bp-0xa],ax
0000046F  8B460C            mov ax,[bp+0xc]
00000472  8B560A            mov dx,[bp+0xa]
00000475  3B46F6            cmp ax,[bp-0xa]
00000478  7C46              jl 0x4c0
0000047A  7F05              jg 0x481
0000047C  3B56F4            cmp dx,[bp-0xc]
0000047F  763F              jna 0x4c0
00000481  8B460C            mov ax,[bp+0xc]
00000484  8B560A            mov dx,[bp+0xa]
00000487  2B56F4            sub dx,[bp-0xc]
0000048A  1B46F6            sbb ax,[bp-0xa]
0000048D  8956F8            mov [bp-0x8],dx
00000490  8946FA            mov [bp-0x6],ax
00000493  FF76FA            push word [bp-0x6]
00000496  FF76F8            push word [bp-0x8]
00000499  33C0              xor ax,ax
0000049B  50                push ax
0000049C  9A09086C18        call 0x186c:0x809
000004A1  83C406            add sp,byte +0x6
000004A4  8B46FA            mov ax,[bp-0x6]
000004A7  8B56F8            mov dx,[bp-0x8]
000004AA  01162853          add [0x5328],dx
000004AE  11062A53          adc [0x532a],ax
000004B2  8B46FA            mov ax,[bp-0x6]
000004B5  8B56F8            mov dx,[bp-0x8]
000004B8  29162053          sub [0x5320],dx
000004BC  19062253          sbb [0x5322],ax
000004C0  8B460C            mov ax,[bp+0xc]
000004C3  8B560A            mov dx,[bp+0xa]
000004C6  3B46F6            cmp ax,[bp-0xa]
000004C9  7F42              jg 0x50d
000004CB  7C05              jl 0x4d2
000004CD  3B56F4            cmp dx,[bp-0xc]
000004D0  733B              jnc 0x50d
000004D2  8B46F6            mov ax,[bp-0xa]
000004D5  8B56F4            mov dx,[bp-0xc]
000004D8  2B560A            sub dx,[bp+0xa]
000004DB  1B460C            sbb ax,[bp+0xc]
000004DE  8956F8            mov [bp-0x8],dx
000004E1  8946FA            mov [bp-0x6],ax
000004E4  FF76FA            push word [bp-0x6]
000004E7  FF76F8            push word [bp-0x8]
000004EA  9AD9086C18        call 0x186c:0x8d9
000004EF  59                pop cx
000004F0  59                pop cx
000004F1  8B46FA            mov ax,[bp-0x6]
000004F4  8B56F8            mov dx,[bp-0x8]
000004F7  29162853          sub [0x5328],dx
000004FB  19062A53          sbb [0x532a],ax
000004FF  8B46FA            mov ax,[bp-0x6]
00000502  8B56F8            mov dx,[bp-0x8]
00000505  01162053          add [0x5320],dx
00000509  11062253          adc [0x5322],ax
0000050D  B80100            mov ax,0x1
00000510  EB00              jmp short 0x512
00000512  8BE5              mov sp,bp
00000514  5D                pop bp
00000515  CB                retf

; ---- 186C:0516 ----
00000516  55                push bp
00000517  8BEC              mov bp,sp
00000519  56                push si
0000051A  57                push di
0000051B  8B5608            mov dx,[bp+0x8]
0000051E  8B4606            mov ax,[bp+0x6]
00000521  B104              mov cl,0x4
00000523  9A58500000        call 0x0:0x5058
00000528  8BF8              mov di,ax
0000052A  8B4606            mov ax,[bp+0x6]
0000052D  250F00            and ax,0xf
00000530  8BF0              mov si,ax
00000532  0BF6              or si,si
00000534  7504              jnz 0x53a
00000536  4F                dec di
00000537  83C610            add si,byte +0x10
0000053A  8BD7              mov dx,di
0000053C  8BC6              mov ax,si
0000053E  EB00              jmp short 0x540
00000540  5F                pop di
00000541  5E                pop si
00000542  5D                pop bp
00000543  CB                retf

; ---- 186C:0544 ----
00000544  55                push bp
00000545  8BEC              mov bp,sp
00000547  83EC04            sub sp,byte +0x4
0000054A  56                push si
0000054B  C746FC044C        mov word [bp-0x4],0x4c04
00000550  8C5EFE            mov [bp-0x2],ds
00000553  33F6              xor si,si
00000555  EB0C              jmp short 0x563
00000557  C45EFC            les bx,[bp-0x4]
0000055A  26C607FF          mov byte [es:bx],0xff
0000055E  46                inc si
0000055F  8346FC0B          add word [bp-0x4],byte +0xb
00000563  83FE19            cmp si,byte +0x19
00000566  7CEF              jl 0x557
00000568  5E                pop si
00000569  8BE5              mov sp,bp
0000056B  5D                pop bp
0000056C  CB                retf

; ---- 186C:056D ----
0000056D  55                push bp
0000056E  8BEC              mov bp,sp
00000570  83EC04            sub sp,byte +0x4
00000573  56                push si
00000574  C746FC044C        mov word [bp-0x4],0x4c04
00000579  8C5EFE            mov [bp-0x2],ds
0000057C  33F6              xor si,si
0000057E  EB21              jmp short 0x5a1
00000580  C45EFC            les bx,[bp-0x4]
00000583  26803FFF          cmp byte [es:bx],0xff
00000587  7505              jnz 0x58e
00000589  B8FFFF            mov ax,0xffff
0000058C  EB1C              jmp short 0x5aa
0000058E  C45EFC            les bx,[bp-0x4]
00000591  268A07            mov al,[es:bx]
00000594  98                cbw
00000595  3B4606            cmp ax,[bp+0x6]
00000598  7502              jnz 0x59c
0000059A  EB0A              jmp short 0x5a6
0000059C  46                inc si
0000059D  8346FC0B          add word [bp-0x4],byte +0xb
000005A1  83FE19            cmp si,byte +0x19
000005A4  7CDA              jl 0x580
000005A6  8BC6              mov ax,si
000005A8  EB00              jmp short 0x5aa
000005AA  5E                pop si
000005AB  8BE5              mov sp,bp
000005AD  5D                pop bp
000005AE  CB                retf

; ---- 186C:05AF ----
000005AF  55                push bp
000005B0  8BEC              mov bp,sp
000005B2  83EC16            sub sp,byte +0x16
000005B5  56                push si
000005B6  8B4606            mov ax,[bp+0x6]
000005B9  BA1300            mov dx,0x13
000005BC  F7EA              imul dx
000005BE  05CE09            add ax,0x9ce
000005C1  8946FC            mov [bp-0x4],ax
000005C4  8C5EFE            mov [bp-0x2],ds
000005C7  C45EFC            les bx,[bp-0x4]
000005CA  268A4709          mov al,[es:bx+0x9]
000005CE  98                cbw
000005CF  8946F6            mov [bp-0xa],ax
000005D2  837EF600          cmp word [bp-0xa],byte +0x0
000005D6  7506              jnz 0x5de
000005D8  B8FFFF            mov ax,0xffff
000005DB  E92201            jmp 0x700
000005DE  833E0E0901        cmp word [0x90e],byte +0x1
000005E3  7512              jnz 0x5f7
000005E5  C45EFC            les bx,[bp-0x4]
000005E8  268B4702          mov ax,[es:bx+0x2]
000005EC  268B17            mov dx,[es:bx]
000005EF  8956F2            mov [bp-0xe],dx
000005F2  8946F4            mov [bp-0xc],ax
000005F5  EB11              jmp short 0x608
000005F7  C45EFC            les bx,[bp-0x4]
000005FA  268B4706          mov ax,[es:bx+0x6]
000005FE  268B5704          mov dx,[es:bx+0x4]
00000602  8956F2            mov [bp-0xe],dx
00000605  8946F4            mov [bp-0xc],ax
00000608  C45EFC            les bx,[bp-0x4]
0000060B  268B470D          mov ax,[es:bx+0xd]
0000060F  268B570B          mov dx,[es:bx+0xb]
00000613  8956F8            mov [bp-0x8],dx
00000616  8946FA            mov [bp-0x6],ax
00000619  8B46F8            mov ax,[bp-0x8]
0000061C  0B46FA            or ax,[bp-0x6]
0000061F  7525              jnz 0x646
00000621  FF76FE            push word [bp-0x2]
00000624  FF76FC            push word [bp-0x4]
00000627  FF76F4            push word [bp-0xc]
0000062A  FF76F2            push word [bp-0xe]
0000062D  9A97096C18        call 0x186c:0x997
00000632  83C408            add sp,byte +0x8
00000635  C45EFC            les bx,[bp-0x4]
00000638  268B470D          mov ax,[es:bx+0xd]
0000063C  268B570B          mov dx,[es:bx+0xb]
00000640  8956F8            mov [bp-0x8],dx
00000643  8946FA            mov [bp-0x6],ax
00000646  FF76F6            push word [bp-0xa]
00000649  9A05076C18        call 0x186c:0x705
0000064E  59                pop cx
0000064F  8946EA            mov [bp-0x16],ax
00000652  8956EC            mov [bp-0x14],dx
00000655  8B46EC            mov ax,[bp-0x14]
00000658  8B56EA            mov dx,[bp-0x16]
0000065B  0356F8            add dx,[bp-0x8]
0000065E  1346FA            adc ax,[bp-0x6]
00000661  3B062653          cmp ax,[0x5326]
00000665  720E              jc 0x675
00000667  7506              jnz 0x66f
00000669  3B162453          cmp dx,[0x5324]
0000066D  7206              jc 0x675
0000066F  B8FFFF            mov ax,0xffff
00000672  E98B00            jmp 0x700
00000675  FF76EC            push word [bp-0x14]
00000678  FF76EA            push word [bp-0x16]
0000067B  9A6E076C18        call 0x186c:0x76e
00000680  59                pop cx
00000681  59                pop cx
00000682  8BF0              mov si,ax
00000684  FF76FA            push word [bp-0x6]
00000687  FF76F8            push word [bp-0x8]
0000068A  56                push si
0000068B  9AB4076C18        call 0x186c:0x7b4
00000690  83C406            add sp,byte +0x6
00000693  FF76F4            push word [bp-0xc]
00000696  FF76F2            push word [bp-0xe]
00000699  FF76EC            push word [bp-0x14]
0000069C  FF76EA            push word [bp-0x16]
0000069F  0E                push cs
000006A0  E873FE            call 0x516
000006A3  59                pop cx
000006A4  59                pop cx
000006A5  52                push dx
000006A6  50                push ax
000006A7  9AB10A6C18        call 0x186c:0xab1
000006AC  83C408            add sp,byte +0x8
000006AF  8BC6              mov ax,si
000006B1  BA0B00            mov dx,0xb
000006B4  F7EA              imul dx
000006B6  05044C            add ax,0x4c04
000006B9  8946EE            mov [bp-0x12],ax
000006BC  8C5EF0            mov [bp-0x10],ds
000006BF  C45EEE            les bx,[bp-0x12]
000006C2  8A4606            mov al,[bp+0x6]
000006C5  268807            mov [es:bx],al
000006C8  C45EEE            les bx,[bp-0x12]
000006CB  8A46F6            mov al,[bp-0xa]
000006CE  26884702          mov [es:bx+0x2],al
000006D2  C45EEE            les bx,[bp-0x12]
000006D5  26C64701FF        mov byte [es:bx+0x1],0xff
000006DA  C45EEE            les bx,[bp-0x12]
000006DD  8B46EC            mov ax,[bp-0x14]
000006E0  8B56EA            mov dx,[bp-0x16]
000006E3  26895707          mov [es:bx+0x7],dx
000006E7  26894709          mov [es:bx+0x9],ax
000006EB  C45EEE            les bx,[bp-0x12]
000006EE  8B46FA            mov ax,[bp-0x6]
000006F1  8B56F8            mov dx,[bp-0x8]
000006F4  26895703          mov [es:bx+0x3],dx
000006F8  26894705          mov [es:bx+0x5],ax
000006FC  8BC6              mov ax,si
000006FE  EB00              jmp short 0x700
00000700  5E                pop si
00000701  8BE5              mov sp,bp
00000703  5D                pop bp
00000704  CB                retf

; ---- 186C:0705 ----
00000705  55                push bp
00000706  8BEC              mov bp,sp
00000708  83EC04            sub sp,byte +0x4
0000070B  56                push si
0000070C  A12A53            mov ax,[0x532a]
0000070F  8B162853          mov dx,[0x5328]
00000713  8956FC            mov [bp-0x4],dx
00000716  8946FE            mov [bp-0x2],ax
00000719  33F6              xor si,si
0000071B  EB3F              jmp short 0x75c
0000071D  8BC6              mov ax,si
0000071F  BA0B00            mov dx,0xb
00000722  F7EA              imul dx
00000724  8BD8              mov bx,ax
00000726  80BF044CFF        cmp byte [bx+0x4c04],0xff
0000072B  7502              jnz 0x72f
0000072D  EB32              jmp short 0x761
0000072F  8BC6              mov ax,si
00000731  BA0B00            mov dx,0xb
00000734  F7EA              imul dx
00000736  8BD8              mov bx,ax
00000738  8A87064C          mov al,[bx+0x4c06]
0000073C  98                cbw
0000073D  3B4606            cmp ax,[bp+0x6]
00000740  7D02              jnl 0x744
00000742  EB1D              jmp short 0x761
00000744  8BC6              mov ax,si
00000746  BA0B00            mov dx,0xb
00000749  F7EA              imul dx
0000074B  8BD8              mov bx,ax
0000074D  8B87094C          mov ax,[bx+0x4c09]
00000751  8B97074C          mov dx,[bx+0x4c07]
00000755  0156FC            add [bp-0x4],dx
00000758  1146FE            adc [bp-0x2],ax
0000075B  46                inc si
0000075C  83FE19            cmp si,byte +0x19
0000075F  7CBC              jl 0x71d
00000761  8B56FE            mov dx,[bp-0x2]
00000764  8B46FC            mov ax,[bp-0x4]
00000767  EB00              jmp short 0x769
00000769  5E                pop si
0000076A  8BE5              mov sp,bp
0000076C  5D                pop bp
0000076D  CB                retf

; ---- 186C:076E ----
0000076E  55                push bp
0000076F  8BEC              mov bp,sp
00000771  56                push si
00000772  33F6              xor si,si
00000774  EB32              jmp short 0x7a8
00000776  8BC6              mov ax,si
00000778  BA0B00            mov dx,0xb
0000077B  F7EA              imul dx
0000077D  8BD8              mov bx,ax
0000077F  80BF044CFF        cmp byte [bx+0x4c04],0xff
00000784  7502              jnz 0x788
00000786  EB25              jmp short 0x7ad
00000788  8BC6              mov ax,si
0000078A  BA0B00            mov dx,0xb
0000078D  F7EA              imul dx
0000078F  8BD8              mov bx,ax
00000791  8B870D4C          mov ax,[bx+0x4c0d]
00000795  8B970B4C          mov dx,[bx+0x4c0b]
00000799  3B4608            cmp ax,[bp+0x8]
0000079C  7209              jc 0x7a7
0000079E  7505              jnz 0x7a5
000007A0  3B5606            cmp dx,[bp+0x6]
000007A3  7202              jc 0x7a7
000007A5  EB06              jmp short 0x7ad
000007A7  46                inc si
000007A8  83FE19            cmp si,byte +0x19
000007AB  7CC9              jl 0x776
000007AD  8BC6              mov ax,si
000007AF  EB00              jmp short 0x7b1
000007B1  5E                pop si
000007B2  5D                pop bp
000007B3  CB                retf

; ---- 186C:07B4 ----
000007B4  55                push bp
000007B5  8BEC              mov bp,sp
000007B7  56                push si
000007B8  FF760A            push word [bp+0xa]
000007BB  FF7608            push word [bp+0x8]
000007BE  FF7606            push word [bp+0x6]
000007C1  9A09086C18        call 0x186c:0x809
000007C6  83C406            add sp,byte +0x6
000007C9  BE1700            mov si,0x17
000007CC  EB33              jmp short 0x801
000007CE  8BC6              mov ax,si
000007D0  BA0B00            mov dx,0xb
000007D3  F7EA              imul dx
000007D5  8BD8              mov bx,ax
000007D7  80BF044CFF        cmp byte [bx+0x4c04],0xff
000007DC  7502              jnz 0x7e0
000007DE  EB20              jmp short 0x800
000007E0  8BC6              mov ax,si
000007E2  BA0B00            mov dx,0xb
000007E5  F7EA              imul dx
000007E7  050F4C            add ax,0x4c0f
000007EA  1E                push ds
000007EB  50                push ax
000007EC  8BC6              mov ax,si
000007EE  BA0B00            mov dx,0xb
000007F1  F7EA              imul dx
000007F3  05044C            add ax,0x4c04
000007F6  1E                push ds
000007F7  50                push ax
000007F8  B90B00            mov cx,0xb
000007FB  9A89510000        call 0x0:0x5189
00000800  4E                dec si
00000801  3B7606            cmp si,[bp+0x6]
00000804  7DC8              jnl 0x7ce
00000806  5E                pop si
00000807  5D                pop bp
00000808  CB                retf

; ---- 186C:0809 ----
00000809  55                push bp
0000080A  8BEC              mov bp,sp
0000080C  83EC04            sub sp,byte +0x4
0000080F  56                push si
00000810  C746FC0C4D        mov word [bp-0x4],0x4d0c
00000815  8C5EFE            mov [bp-0x2],ds
00000818  BE1800            mov si,0x18
0000081B  E9AE00            jmp 0x8cc
0000081E  C45EFC            les bx,[bp-0x4]
00000821  26803FFF          cmp byte [es:bx],0xff
00000825  7503              jnz 0x82a
00000827  E99D00            jmp 0x8c7
0000082A  C45EFC            les bx,[bp-0x4]
0000082D  268B4709          mov ax,[es:bx+0x9]
00000831  268B5707          mov dx,[es:bx+0x7]
00000835  C45EFC            les bx,[bp-0x4]
00000838  26035703          add dx,[es:bx+0x3]
0000083C  26134705          adc ax,[es:bx+0x5]
00000840  035608            add dx,[bp+0x8]
00000843  13460A            adc ax,[bp+0xa]
00000846  3B062653          cmp ax,[0x5326]
0000084A  7211              jc 0x85d
0000084C  7506              jnz 0x854
0000084E  3B162453          cmp dx,[0x5324]
00000852  7209              jc 0x85d
00000854  C45EFC            les bx,[bp-0x4]
00000857  26C607FF          mov byte [es:bx],0xff
0000085B  EB6A              jmp short 0x8c7
0000085D  C45EFC            les bx,[bp-0x4]
00000860  26FF7705          push word [es:bx+0x5]
00000864  26FF7703          push word [es:bx+0x3]
00000868  C45EFC            les bx,[bp-0x4]
0000086B  268B4709          mov ax,[es:bx+0x9]
0000086F  268B5707          mov dx,[es:bx+0x7]
00000873  C45EFC            les bx,[bp-0x4]
00000876  26035703          add dx,[es:bx+0x3]
0000087A  26134705          adc ax,[es:bx+0x5]
0000087E  035608            add dx,[bp+0x8]
00000881  13460A            adc ax,[bp+0xa]
00000884  50                push ax
00000885  52                push dx
00000886  0E                push cs
00000887  E88CFC            call 0x516
0000088A  59                pop cx
0000088B  59                pop cx
0000088C  52                push dx
0000088D  50                push ax
0000088E  C45EFC            les bx,[bp-0x4]
00000891  268B4709          mov ax,[es:bx+0x9]
00000895  268B5707          mov dx,[es:bx+0x7]
00000899  C45EFC            les bx,[bp-0x4]
0000089C  26035703          add dx,[es:bx+0x3]
000008A0  26134705          adc ax,[es:bx+0x5]
000008A4  50                push ax
000008A5  52                push dx
000008A6  0E                push cs
000008A7  E86CFC            call 0x516
000008AA  59                pop cx
000008AB  59                pop cx
000008AC  52                push dx
000008AD  50                push ax
000008AE  9A8B04C627        call 0x27c6:0x48b
000008B3  83C40C            add sp,byte +0xc
000008B6  C45EFC            les bx,[bp-0x4]
000008B9  8B460A            mov ax,[bp+0xa]
000008BC  8B5608            mov dx,[bp+0x8]
000008BF  26015707          add [es:bx+0x7],dx
000008C3  26114709          adc [es:bx+0x9],ax
000008C7  4E                dec si
000008C8  836EFC0B          sub word [bp-0x4],byte +0xb
000008CC  3B7606            cmp si,[bp+0x6]
000008CF  7C03              jl 0x8d4
000008D1  E94AFF            jmp 0x81e
000008D4  5E                pop si
000008D5  8BE5              mov sp,bp
000008D7  5D                pop bp
000008D8  CB                retf

; ---- 186C:08D9 ----
000008D9  55                push bp
000008DA  8BEC              mov bp,sp
000008DC  83EC04            sub sp,byte +0x4
000008DF  56                push si
000008E0  C746FC044C        mov word [bp-0x4],0x4c04
000008E5  8C5EFE            mov [bp-0x2],ds
000008E8  33F6              xor si,si
000008EA  EB62              jmp short 0x94e
000008EC  C45EFC            les bx,[bp-0x4]
000008EF  26803FFF          cmp byte [es:bx],0xff
000008F3  7502              jnz 0x8f7
000008F5  EB5C              jmp short 0x953
000008F7  C45EFC            les bx,[bp-0x4]
000008FA  26FF7705          push word [es:bx+0x5]
000008FE  26FF7703          push word [es:bx+0x3]
00000902  C45EFC            les bx,[bp-0x4]
00000905  268B4709          mov ax,[es:bx+0x9]
00000909  268B5707          mov dx,[es:bx+0x7]
0000090D  2B5606            sub dx,[bp+0x6]
00000910  1B4608            sbb ax,[bp+0x8]
00000913  50                push ax
00000914  52                push dx
00000915  0E                push cs
00000916  E8FDFB            call 0x516
00000919  59                pop cx
0000091A  59                pop cx
0000091B  52                push dx
0000091C  50                push ax
0000091D  C45EFC            les bx,[bp-0x4]
00000920  26FF7709          push word [es:bx+0x9]
00000924  26FF7707          push word [es:bx+0x7]
00000928  0E                push cs
00000929  E8EAFB            call 0x516
0000092C  59                pop cx
0000092D  59                pop cx
0000092E  52                push dx
0000092F  50                push ax
00000930  9ACE04C627        call 0x27c6:0x4ce
00000935  83C40C            add sp,byte +0xc
00000938  C45EFC            les bx,[bp-0x4]
0000093B  8B4608            mov ax,[bp+0x8]
0000093E  8B5606            mov dx,[bp+0x6]
00000941  26295707          sub [es:bx+0x7],dx
00000945  26194709          sbb [es:bx+0x9],ax
00000949  46                inc si
0000094A  8346FC0B          add word [bp-0x4],byte +0xb
0000094E  83FE19            cmp si,byte +0x19
00000951  7C99              jl 0x8ec
00000953  5E                pop si
00000954  8BE5              mov sp,bp
00000956  5D                pop bp
00000957  CB                retf

; ---- 186C:0958 ----
00000958  55                push bp
00000959  8BEC              mov bp,sp
0000095B  8B4606            mov ax,[bp+0x6]
0000095E  BA0B00            mov dx,0xb
00000961  F7EA              imul dx
00000963  8BD8              mov bx,ax
00000965  FFB7094C          push word [bx+0x4c09]
00000969  FFB7074C          push word [bx+0x4c07]
0000096D  FF760A            push word [bp+0xa]
00000970  FF7608            push word [bp+0x8]
00000973  8B4606            mov ax,[bp+0x6]
00000976  BA0B00            mov dx,0xb
00000979  F7EA              imul dx
0000097B  8BD8              mov bx,ax
0000097D  FFB70D4C          push word [bx+0x4c0d]
00000981  FFB70B4C          push word [bx+0x4c0b]
00000985  0E                push cs
00000986  E88DFB            call 0x516
00000989  59                pop cx
0000098A  59                pop cx
0000098B  52                push dx
0000098C  50                push ax
0000098D  9ACE04C627        call 0x27c6:0x4ce
00000992  83C40C            add sp,byte +0xc
00000995  5D                pop bp
00000996  CB                retf

; ---- 186C:0997 ----
00000997  55                push bp
00000998  8BEC              mov bp,sp
0000099A  83EC0C            sub sp,byte +0xc
0000099D  56                push si
0000099E  C45E0A            les bx,[bp+0xa]
000009A1  268A4708          mov al,[es:bx+0x8]
000009A5  98                cbw
000009A6  50                push ax
000009A7  FF7608            push word [bp+0x8]
000009AA  FF7606            push word [bp+0x6]
000009AD  9A6C0A6C18        call 0x186c:0xa6c
000009B2  83C406            add sp,byte +0x6
000009B5  B80100            mov ax,0x1
000009B8  50                push ax
000009B9  FF7608            push word [bp+0x8]
000009BC  FF7606            push word [bp+0x6]
000009BF  9AED3F0000        call 0x0:0x3fed
000009C4  83C406            add sp,byte +0x6
000009C7  8BF0              mov si,ax
000009C9  56                push si
000009CA  9AE03A0000        call 0x0:0x3ae0
000009CF  59                pop cx
000009D0  8946FC            mov [bp-0x4],ax
000009D3  8956FE            mov [bp-0x2],dx
000009D6  C45E0A            les bx,[bp+0xa]
000009D9  8B46FE            mov ax,[bp-0x2]
000009DC  8B56FC            mov dx,[bp-0x4]
000009DF  2689570B          mov [es:bx+0xb],dx
000009E3  2689470D          mov [es:bx+0xd],ax
000009E7  B80200            mov ax,0x2
000009EA  50                push ax
000009EB  B8FFFF            mov ax,0xffff
000009EE  BAFCFF            mov dx,0xfffc
000009F1  50                push ax
000009F2  52                push dx
000009F3  56                push si
000009F4  9A3F3E0000        call 0x0:0x3e3f
000009F9  83C408            add sp,byte +0x8
000009FC  33C0              xor ax,ax
000009FE  BA0400            mov dx,0x4
00000A01  50                push ax
00000A02  52                push dx
00000A03  16                push ss
00000A04  8D46F8            lea ax,[bp-0x8]
00000A07  50                push ax
00000A08  56                push si
00000A09  9A9A05730D        call 0xd73:0x59a
00000A0E  83C40A            add sp,byte +0xa
00000A11  8A46F8            mov al,[bp-0x8]
00000A14  B400              mov ah,0x0
00000A16  8946F4            mov [bp-0xc],ax
00000A19  C746F60000        mov word [bp-0xa],0x0
00000A1E  8B46F4            mov ax,[bp-0xc]
00000A21  C746F40000        mov word [bp-0xc],0x0
00000A26  8946F6            mov [bp-0xa],ax
00000A29  8A46F9            mov al,[bp-0x7]
00000A2C  B400              mov ah,0x0
00000A2E  33D2              xor dx,dx
00000A30  B108              mov cl,0x8
00000A32  9A37500000        call 0x0:0x5037
00000A37  8A5EFA            mov bl,[bp-0x6]
00000A3A  B700              mov bh,0x0
00000A3C  03C3              add ax,bx
00000A3E  83D200            adc dx,byte +0x0
00000A41  0146F4            add [bp-0xc],ax
00000A44  1156F6            adc [bp-0xa],dx
00000A47  C45E0A            les bx,[bp+0xa]
00000A4A  8B46F6            mov ax,[bp-0xa]
00000A4D  8B56F4            mov dx,[bp-0xc]
00000A50  2689570F          mov [es:bx+0xf],dx
00000A54  26894711          mov [es:bx+0x11],ax
00000A58  56                push si
00000A59  9ADC260000        call 0x0:0x26dc
00000A5E  59                pop cx
00000A5F  8B56FE            mov dx,[bp-0x2]
00000A62  8B46FC            mov ax,[bp-0x4]
00000A65  EB00              jmp short 0xa67
00000A67  5E                pop si
00000A68  8BE5              mov sp,bp
00000A6A  5D                pop bp
00000A6B  CB                retf

; ---- 186C:0A6C ----
00000A6C  55                push bp
00000A6D  8BEC              mov bp,sp
00000A6F  83EC04            sub sp,byte +0x4
00000A72  1E                push ds
00000A73  B86C0B            mov ax,0xb6c
00000A76  50                push ax
00000A77  FF7608            push word [bp+0x8]
00000A7A  FF7606            push word [bp+0x6]
00000A7D  9A392C0000        call 0x0:0x2c39
00000A82  83C408            add sp,byte +0x8
00000A85  8946FC            mov [bp-0x4],ax
00000A88  8956FE            mov [bp-0x2],dx
00000A8B  8B46FC            mov ax,[bp-0x4]
00000A8E  0B46FE            or ax,[bp-0x2]
00000A91  7402              jz 0xa95
00000A93  EB0B              jmp short 0xaa0
00000A95  FF760A            push word [bp+0xa]
00000A98  9A630B6C18        call 0x186c:0xb63
00000A9D  59                pop cx
00000A9E  EBD2              jmp short 0xa72
00000AA0  FF76FE            push word [bp-0x2]
00000AA3  FF76FC            push word [bp-0x4]
00000AA6  9A5B280000        call 0x0:0x285b
00000AAB  59                pop cx
00000AAC  59                pop cx
00000AAD  8BE5              mov sp,bp
00000AAF  5D                pop bp
00000AB0  CB                retf

; ---- 186C:0AB1 ----
00000AB1  55                push bp
00000AB2  8BEC              mov bp,sp
00000AB4  83EC04            sub sp,byte +0x4
00000AB7  56                push si
00000AB8  B80180            mov ax,0x8001
00000ABB  50                push ax
00000ABC  FF760C            push word [bp+0xc]
00000ABF  FF760A            push word [bp+0xa]
00000AC2  9AED3F0000        call 0x0:0x3fed
00000AC7  83C406            add sp,byte +0x6
00000ACA  8BF0              mov si,ax
00000ACC  83FEFF            cmp si,byte -0x1
00000ACF  7502              jnz 0xad3
00000AD1  EB2C              jmp short 0xaff
00000AD3  B8FFFF            mov ax,0xffff
00000AD6  BAFFFF            mov dx,0xffff
00000AD9  50                push ax
00000ADA  52                push dx
00000ADB  FF7608            push word [bp+0x8]
00000ADE  FF7606            push word [bp+0x6]
00000AE1  56                push si
00000AE2  9A9A05730D        call 0xd73:0x59a
00000AE7  83C40A            add sp,byte +0xa
00000AEA  8946FC            mov [bp-0x4],ax
00000AED  8956FE            mov [bp-0x2],dx
00000AF0  56                push si
00000AF1  9ADC260000        call 0x0:0x26dc
00000AF6  59                pop cx
00000AF7  8B56FE            mov dx,[bp-0x2]
00000AFA  8B46FC            mov ax,[bp-0x4]
00000AFD  EB06              jmp short 0xb05
00000AFF  33D2              xor dx,dx
00000B01  33C0              xor ax,ax
00000B03  EB00              jmp short 0xb05
00000B05  5E                pop si
00000B06  8BE5              mov sp,bp
00000B08  5D                pop bp
00000B09  CB                retf

; ---- 186C:0B0A ----
00000B0A  55                push bp
00000B0B  8BEC              mov bp,sp
00000B0D  83EC04            sub sp,byte +0x4
00000B10  56                push si
00000B11  B80140            mov ax,0x4001
00000B14  50                push ax
00000B15  FF760C            push word [bp+0xc]
00000B18  FF760A            push word [bp+0xa]
00000B1B  9AED3F0000        call 0x0:0x3fed
00000B20  83C406            add sp,byte +0x6
00000B23  8BF0              mov si,ax
00000B25  83FEFF            cmp si,byte -0x1
00000B28  7502              jnz 0xb2c
00000B2A  EB2C              jmp short 0xb58
00000B2C  B8FFFF            mov ax,0xffff
00000B2F  BAFFFF            mov dx,0xffff
00000B32  50                push ax
00000B33  52                push dx
00000B34  FF7608            push word [bp+0x8]
00000B37  FF7606            push word [bp+0x6]
00000B3A  56                push si
00000B3B  9A9A05730D        call 0xd73:0x59a
00000B40  83C40A            add sp,byte +0xa
00000B43  8946FC            mov [bp-0x4],ax
00000B46  8956FE            mov [bp-0x2],dx
00000B49  56                push si
00000B4A  9ADC260000        call 0x0:0x26dc
00000B4F  59                pop cx
00000B50  8B56FE            mov dx,[bp-0x2]
00000B53  8B46FC            mov ax,[bp-0x4]
00000B56  EB06              jmp short 0xb5e
00000B58  33D2              xor dx,dx
00000B5A  33C0              xor ax,ax
00000B5C  EB00              jmp short 0xb5e
00000B5E  5E                pop si
00000B5F  8BE5              mov sp,bp
00000B61  5D                pop bp
00000B62  CB                retf

; ---- 186C:0B63 ----
00000B63  55                push bp
00000B64  8BEC              mov bp,sp
00000B66  B87707            mov ax,0x777
00000B69  50                push ax
00000B6A  B80F00            mov ax,0xf
00000B6D  50                push ax
00000B6E  9AC902FD21        call 0x21fd:0x2c9
00000B73  59                pop cx
00000B74  59                pop cx
00000B75  33C0              xor ax,ax
00000B77  50                push ax
00000B78  33C0              xor ax,ax
00000B7A  50                push ax
00000B7B  9AC902FD21        call 0x21fd:0x2c9
00000B80  59                pop cx
00000B81  59                pop cx
00000B82  33C0              xor ax,ax
00000B84  50                push ax
00000B85  9ADB05FD21        call 0x21fd:0x5db
00000B8A  59                pop cx
00000B8B  9A1B00960A        call 0xa96:0x1b
00000B90  33C0              xor ax,ax
00000B92  50                push ax
00000B93  B8C800            mov ax,0xc8
00000B96  50                push ax
00000B97  B84001            mov ax,0x140
00000B9A  50                push ax
00000B9B  33C0              xor ax,ax
00000B9D  50                push ax
00000B9E  33C0              xor ax,ax
00000BA0  50                push ax
00000BA1  9AD704960A        call 0xa96:0x4d7
00000BA6  83C40A            add sp,byte +0xa
00000BA9  B8FFFF            mov ax,0xffff
00000BAC  50                push ax
00000BAD  B80F00            mov ax,0xf
00000BB0  50                push ax
00000BB1  FF368A3C          push word [0x3c8a]
00000BB5  FF36883C          push word [0x3c88]
00000BB9  B84001            mov ax,0x140
00000BBC  50                push ax
00000BBD  B86400            mov ax,0x64
00000BC0  50                push ax
00000BC1  33C0              xor ax,ax
00000BC3  50                push ax
00000BC4  9A9000690A        call 0xa69:0x90
00000BC9  83C40E            add sp,byte +0xe
00000BCC  9A0D06FD21        call 0x21fd:0x60d
00000BD1  9A5303FD21        call 0x21fd:0x353
00000BD6  33C0              xor ax,ax
00000BD8  50                push ax
00000BD9  9ADB05FD21        call 0x21fd:0x5db
00000BDE  59                pop cx
00000BDF  B80100            mov ax,0x1
00000BE2  50                push ax
00000BE3  33C0              xor ax,ax
00000BE5  50                push ax
00000BE6  9A5400A721        call 0x21a7:0x54
00000BEB  59                pop cx
00000BEC  59                pop cx
00000BED  9AD200C627        call 0x27c6:0xd2
00000BF2  9A1B00960A        call 0xa96:0x1b
00000BF7  33C0              xor ax,ax
00000BF9  50                push ax
00000BFA  B8C800            mov ax,0xc8
00000BFD  50                push ax
00000BFE  B84001            mov ax,0x140
00000C01  50                push ax
00000C02  33C0              xor ax,ax
00000C04  50                push ax
00000C05  33C0              xor ax,ax
00000C07  50                push ax
00000C08  9AD704960A        call 0xa96:0x4d7
00000C0D  83C40A            add sp,byte +0xa
