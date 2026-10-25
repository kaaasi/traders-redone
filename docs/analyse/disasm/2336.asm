; segment 2336  file offset 0x26960  size 0x970
00000000  83C408            add sp,byte +0x8
00000003  9A0D06FD21        call 0x21fd:0x60d
00000008  5D                pop bp
00000009  CB                retf

; ---- 2336:000A ----
0000000A  55                push bp
0000000B  8BEC              mov bp,sp
0000000D  83EC08            sub sp,byte +0x8
00000010  56                push si
00000011  57                push di
00000012  A1D845            mov ax,[0x45d8]
00000015  8B16D645          mov dx,[0x45d6]
00000019  8956FC            mov [bp-0x4],dx
0000001C  8946FE            mov [bp-0x2],ax
0000001F  8B46FE            mov ax,[bp-0x2]
00000022  8B56FC            mov dx,[bp-0x4]
00000025  8916F857          mov [0x57f8],dx
00000029  A3FA57            mov [0x57fa],ax
0000002C  C606AA3700        mov byte [0x37aa],0x0
00000031  E97802            jmp 0x2ac
00000034  A0AA37            mov al,[0x37aa]
00000037  98                cbw
00000038  8BD8              mov bx,ax
0000003A  8A87FE40          mov al,[bx+0x40fe]
0000003E  98                cbw
0000003F  99                cwd
00000040  52                push dx
00000041  50                push ax
00000042  9A1E053623        call 0x2336:0x51e
00000047  59                pop cx
00000048  59                pop cx
00000049  A0AA37            mov al,[0x37aa]
0000004C  98                cbw
0000004D  8BD8              mov bx,ax
0000004F  8A870241          mov al,[bx+0x4102]
00000053  98                cbw
00000054  99                cwd
00000055  52                push dx
00000056  50                push ax
00000057  9A1E053623        call 0x2336:0x51e
0000005C  59                pop cx
0000005D  59                pop cx
0000005E  A0AA37            mov al,[0x37aa]
00000061  98                cbw
00000062  D1E0              shl ax,1
00000064  8BD8              mov bx,ax
00000066  8B87AC4B          mov ax,[bx+0x4bac]
0000006A  99                cwd
0000006B  52                push dx
0000006C  50                push ax
0000006D  9A1E053623        call 0x2336:0x51e
00000072  59                pop cx
00000073  59                pop cx
00000074  A0AA37            mov al,[0x37aa]
00000077  98                cbw
00000078  D1E0              shl ax,1
0000007A  8BD8              mov bx,ax
0000007C  8B872045          mov ax,[bx+0x4520]
00000080  99                cwd
00000081  52                push dx
00000082  50                push ax
00000083  9A1E053623        call 0x2336:0x51e
00000088  59                pop cx
00000089  59                pop cx
0000008A  A0AA37            mov al,[0x37aa]
0000008D  98                cbw
0000008E  D1E0              shl ax,1
00000090  8BD8              mov bx,ax
00000092  8B870E30          mov ax,[bx+0x300e]
00000096  99                cwd
00000097  52                push dx
00000098  50                push ax
00000099  9A1E053623        call 0x2336:0x51e
0000009E  59                pop cx
0000009F  59                pop cx
000000A0  A0AA37            mov al,[0x37aa]
000000A3  98                cbw
000000A4  8BD8              mov bx,ax
000000A6  8A87AF37          mov al,[bx+0x37af]
000000AA  98                cbw
000000AB  99                cwd
000000AC  52                push dx
000000AD  50                push ax
000000AE  9A1E053623        call 0x2336:0x51e
000000B3  59                pop cx
000000B4  59                pop cx
000000B5  A0AA37            mov al,[0x37aa]
000000B8  98                cbw
000000B9  D1E0              shl ax,1
000000BB  D1E0              shl ax,1
000000BD  8BD8              mov bx,ax
000000BF  FFB7D43C          push word [bx+0x3cd4]
000000C3  FFB7D23C          push word [bx+0x3cd2]
000000C7  9A1E053623        call 0x2336:0x51e
000000CC  59                pop cx
000000CD  59                pop cx
000000CE  A0AA37            mov al,[0x37aa]
000000D1  98                cbw
000000D2  D1E0              shl ax,1
000000D4  8BD8              mov bx,ax
000000D6  8B870E45          mov ax,[bx+0x450e]
000000DA  99                cwd
000000DB  52                push dx
000000DC  50                push ax
000000DD  9A1E053623        call 0x2336:0x51e
000000E2  59                pop cx
000000E3  59                pop cx
000000E4  A0AA37            mov al,[0x37aa]
000000E7  98                cbw
000000E8  8BD8              mov bx,ax
000000EA  8A87C24B          mov al,[bx+0x4bc2]
000000EE  B400              mov ah,0x0
000000F0  99                cwd
000000F1  52                push dx
000000F2  50                push ax
000000F3  9A1E053623        call 0x2336:0x51e
000000F8  59                pop cx
000000F9  59                pop cx
000000FA  A0AA37            mov al,[0x37aa]
000000FD  98                cbw
000000FE  D1E0              shl ax,1
00000100  8BD8              mov bx,ax
00000102  8B87F640          mov ax,[bx+0x40f6]
00000106  99                cwd
00000107  52                push dx
00000108  50                push ax
00000109  9A1E053623        call 0x2336:0x51e
0000010E  59                pop cx
0000010F  59                pop cx
00000110  A0AA37            mov al,[0x37aa]
00000113  98                cbw
00000114  8BD8              mov bx,ax
00000116  8A874008          mov al,[bx+0x840]
0000011A  B400              mov ah,0x0
0000011C  99                cwd
0000011D  52                push dx
0000011E  50                push ax
0000011F  9A1E053623        call 0x2336:0x51e
00000124  59                pop cx
00000125  59                pop cx
00000126  A0AA37            mov al,[0x37aa]
00000129  98                cbw
0000012A  8BD8              mov bx,ax
0000012C  8A873C08          mov al,[bx+0x83c]
00000130  98                cbw
00000131  99                cwd
00000132  52                push dx
00000133  50                push ax
00000134  9A1E053623        call 0x2336:0x51e
00000139  59                pop cx
0000013A  59                pop cx
0000013B  A0AA37            mov al,[0x37aa]
0000013E  98                cbw
0000013F  8BD8              mov bx,ax
00000141  8A87BE4B          mov al,[bx+0x4bbe]
00000145  B400              mov ah,0x0
00000147  99                cwd
00000148  52                push dx
00000149  50                push ax
0000014A  9A1E053623        call 0x2336:0x51e
0000014F  59                pop cx
00000150  59                pop cx
00000151  A0AA37            mov al,[0x37aa]
00000154  98                cbw
00000155  8BD8              mov bx,ax
00000157  8A87A84B          mov al,[bx+0x4ba8]
0000015B  B400              mov ah,0x0
0000015D  99                cwd
0000015E  52                push dx
0000015F  50                push ax
00000160  9A1E053623        call 0x2336:0x51e
00000165  59                pop cx
00000166  59                pop cx
00000167  C706D0450000      mov word [0x45d0],0x0
0000016D  EB43              jmp short 0x1b2
0000016F  A0AA37            mov al,[0x37aa]
00000172  98                cbw
00000173  D1E0              shl ax,1
00000175  D1E0              shl ax,1
00000177  0306D045          add ax,[0x45d0]
0000017B  8BD8              mov bx,ax
0000017D  8A87E72E          mov al,[bx+0x2ee7]
00000181  98                cbw
00000182  99                cwd
00000183  52                push dx
00000184  50                push ax
00000185  9A1E053623        call 0x2336:0x51e
0000018A  59                pop cx
0000018B  59                pop cx
0000018C  A0AA37            mov al,[0x37aa]
0000018F  98                cbw
00000190  D1E0              shl ax,1
00000192  D1E0              shl ax,1
00000194  D1E0              shl ax,1
00000196  8B16D045          mov dx,[0x45d0]
0000019A  D1E2              shl dx,1
0000019C  03C2              add ax,dx
0000019E  8BD8              mov bx,ax
000001A0  8B87F43C          mov ax,[bx+0x3cf4]
000001A4  99                cwd
000001A5  52                push dx
000001A6  50                push ax
000001A7  9A1E053623        call 0x2336:0x51e
000001AC  59                pop cx
000001AD  59                pop cx
000001AE  FF06D045          inc word [0x45d0]
000001B2  833ED04503        cmp word [0x45d0],byte +0x3
000001B7  7EB6              jng 0x16f
000001B9  33F6              xor si,si
000001BB  EB23              jmp short 0x1e0
000001BD  A0AA37            mov al,[0x37aa]
000001C0  98                cbw
000001C1  BAA000            mov dx,0xa0
000001C4  F7EA              imul dx
000001C6  8BD6              mov dx,si
000001C8  D1E2              shl dx,1
000001CA  D1E2              shl dx,1
000001CC  03C2              add ax,dx
000001CE  8BD8              mov bx,ax
000001D0  FFB71E41          push word [bx+0x411e]
000001D4  FFB71C41          push word [bx+0x411c]
000001D8  9A1E053623        call 0x2336:0x51e
000001DD  59                pop cx
000001DE  59                pop cx
000001DF  46                inc si
000001E0  83FE27            cmp si,byte +0x27
000001E3  7ED8              jng 0x1bd
000001E5  33F6              xor si,si
000001E7  EB1C              jmp short 0x205
000001E9  A0AA37            mov al,[0x37aa]
000001EC  98                cbw
000001ED  BA0600            mov dx,0x6
000001F0  F7EA              imul dx
000001F2  8BD8              mov bx,ax
000001F4  8A80CE43          mov al,[bx+si+0x43ce]
000001F8  B400              mov ah,0x0
000001FA  99                cwd
000001FB  52                push dx
000001FC  50                push ax
000001FD  9A1E053623        call 0x2336:0x51e
00000202  59                pop cx
00000203  59                pop cx
00000204  46                inc si
00000205  83FE05            cmp si,byte +0x5
00000208  7EDF              jng 0x1e9
0000020A  A0AA37            mov al,[0x37aa]
0000020D  98                cbw
0000020E  D1E0              shl ax,1
00000210  D1E0              shl ax,1
00000212  33D2              xor dx,dx
00000214  52                push dx
00000215  8BD8              mov bx,ax
00000217  FFB76C37          push word [bx+0x376c]
0000021B  9A1E053623        call 0x2336:0x51e
00000220  59                pop cx
00000221  59                pop cx
00000222  A0AA37            mov al,[0x37aa]
00000225  98                cbw
00000226  D1E0              shl ax,1
00000228  D1E0              shl ax,1
0000022A  33D2              xor dx,dx
0000022C  52                push dx
0000022D  8BD8              mov bx,ax
0000022F  FFB76E37          push word [bx+0x376e]
00000233  9A1E053623        call 0x2336:0x51e
00000238  59                pop cx
00000239  59                pop cx
0000023A  A0AA37            mov al,[0x37aa]
0000023D  98                cbw
0000023E  D1E0              shl ax,1
00000240  33D2              xor dx,dx
00000242  52                push dx
00000243  8BD8              mov bx,ax
00000245  FFB7F82E          push word [bx+0x2ef8]
00000249  9A1E053623        call 0x2336:0x51e
0000024E  59                pop cx
0000024F  59                pop cx
00000250  A0AA37            mov al,[0x37aa]
00000253  98                cbw
00000254  8BD8              mov bx,ax
00000256  8A877C37          mov al,[bx+0x377c]
0000025A  B400              mov ah,0x0
0000025C  99                cwd
0000025D  52                push dx
0000025E  50                push ax
0000025F  9A1E053623        call 0x2336:0x51e
00000264  59                pop cx
00000265  59                pop cx
00000266  A0AA37            mov al,[0x37aa]
00000269  98                cbw
0000026A  D1E0              shl ax,1
0000026C  33D2              xor dx,dx
0000026E  52                push dx
0000026F  8BD8              mov bx,ax
00000271  FFB7EC40          push word [bx+0x40ec]
00000275  9A1E053623        call 0x2336:0x51e
0000027A  59                pop cx
0000027B  59                pop cx
0000027C  A0AA37            mov al,[0x37aa]
0000027F  98                cbw
00000280  D1E0              shl ax,1
00000282  33D2              xor dx,dx
00000284  52                push dx
00000285  8BD8              mov bx,ax
00000287  FFB7DC33          push word [bx+0x33dc]
0000028B  9A1E053623        call 0x2336:0x51e
00000290  59                pop cx
00000291  59                pop cx
00000292  A0AA37            mov al,[0x37aa]
00000295  98                cbw
00000296  D1E0              shl ax,1
00000298  33D2              xor dx,dx
0000029A  52                push dx
0000029B  8BD8              mov bx,ax
0000029D  FFB71630          push word [bx+0x3016]
000002A1  9A1E053623        call 0x2336:0x51e
000002A6  59                pop cx
000002A7  59                pop cx
000002A8  FE06AA37          inc byte [0x37aa]
000002AC  803EAA3704        cmp byte [0x37aa],0x4
000002B1  7D03              jnl 0x2b6
000002B3  E97EFD            jmp 0x34
000002B6  C706D0450000      mov word [0x45d0],0x0
000002BC  EB18              jmp short 0x2d6
000002BE  8B1ED045          mov bx,[0x45d0]
000002C2  D1E3              shl bx,1
000002C4  8B879848          mov ax,[bx+0x4898]
000002C8  99                cwd
000002C9  52                push dx
000002CA  50                push ax
000002CB  9A1E053623        call 0x2336:0x51e
000002D0  59                pop cx
000002D1  59                pop cx
000002D2  FF06D045          inc word [0x45d0]
000002D6  833ED04503        cmp word [0x45d0],byte +0x3
000002DB  7EE1              jng 0x2be
000002DD  C606002F00        mov byte [0x2f00],0x0
000002E2  EB76              jmp short 0x35a
000002E4  C606B02E00        mov byte [0x2eb0],0x0
000002E9  EB64              jmp short 0x34f
000002EB  A0B02E            mov al,[0x2eb0]
000002EE  98                cbw
000002EF  BA0600            mov dx,0x6
000002F2  F7EA              imul dx
000002F4  50                push ax
000002F5  A0002F            mov al,[0x2f00]
000002F8  98                cbw
000002F9  5B                pop bx
000002FA  03D8              add bx,ax
000002FC  8A874244          mov al,[bx+0x4442]
00000300  98                cbw
00000301  99                cwd
00000302  52                push dx
00000303  50                push ax
00000304  9A1E053623        call 0x2336:0x51e
00000309  59                pop cx
0000030A  59                pop cx
0000030B  A0B02E            mov al,[0x2eb0]
0000030E  98                cbw
0000030F  BA0600            mov dx,0x6
00000312  F7EA              imul dx
00000314  50                push ax
00000315  A0002F            mov al,[0x2f00]
00000318  98                cbw
00000319  5B                pop bx
0000031A  03D8              add bx,ax
0000031C  8A87B12E          mov al,[bx+0x2eb1]
00000320  98                cbw
00000321  99                cwd
00000322  52                push dx
00000323  50                push ax
00000324  9A1E053623        call 0x2336:0x51e
00000329  59                pop cx
0000032A  59                pop cx
0000032B  A0B02E            mov al,[0x2eb0]
0000032E  98                cbw
0000032F  BA0600            mov dx,0x6
00000332  F7EA              imul dx
00000334  50                push ax
00000335  A0002F            mov al,[0x2f00]
00000338  98                cbw
00000339  5B                pop bx
0000033A  03D8              add bx,ax
0000033C  8A87DA45          mov al,[bx+0x45da]
00000340  98                cbw
00000341  99                cwd
00000342  52                push dx
00000343  50                push ax
00000344  9A1E053623        call 0x2336:0x51e
00000349  59                pop cx
0000034A  59                pop cx
0000034B  FE06B02E          inc byte [0x2eb0]
0000034F  803EB02E08        cmp byte [0x2eb0],0x8
00000354  7E95              jng 0x2eb
00000356  FE06002F          inc byte [0x2f00]
0000035A  803E002F05        cmp byte [0x2f00],0x5
0000035F  7E83              jng 0x2e4
00000361  A1FC08            mov ax,[0x8fc]
00000364  99                cwd
00000365  52                push dx
00000366  50                push ax
00000367  9A1E053623        call 0x2336:0x51e
0000036C  59                pop cx
0000036D  59                pop cx
0000036E  A1FE08            mov ax,[0x8fe]
00000371  99                cwd
00000372  52                push dx
00000373  50                push ax
00000374  9A1E053623        call 0x2336:0x51e
00000379  59                pop cx
0000037A  59                pop cx
0000037B  33F6              xor si,si
0000037D  EB35              jmp short 0x3b4
0000037F  8A841A2F          mov al,[si+0x2f1a]
00000383  B400              mov ah,0x0
00000385  99                cwd
00000386  52                push dx
00000387  50                push ax
00000388  9A1E053623        call 0x2336:0x51e
0000038D  59                pop cx
0000038E  59                pop cx
0000038F  8BDE              mov bx,si
00000391  D1E3              shl bx,1
00000393  8B87F444          mov ax,[bx+0x44f4]
00000397  99                cwd
00000398  52                push dx
00000399  50                push ax
0000039A  9A1E053623        call 0x2336:0x51e
0000039F  59                pop cx
000003A0  59                pop cx
000003A1  8BDE              mov bx,si
000003A3  D1E3              shl bx,1
000003A5  8B878A34          mov ax,[bx+0x348a]
000003A9  99                cwd
000003AA  52                push dx
000003AB  50                push ax
000003AC  9A1E053623        call 0x2336:0x51e
000003B1  59                pop cx
000003B2  59                pop cx
000003B3  46                inc si
000003B4  83FE0B            cmp si,byte +0xb
000003B7  7EC6              jng 0x37f
000003B9  A10009            mov ax,[0x900]
000003BC  99                cwd
000003BD  52                push dx
000003BE  50                push ax
000003BF  9A1E053623        call 0x2336:0x51e
000003C4  59                pop cx
000003C5  59                pop cx
000003C6  A0E640            mov al,[0x40e6]
000003C9  98                cbw
000003CA  99                cwd
000003CB  52                push dx
000003CC  50                push ax
000003CD  9A1E053623        call 0x2336:0x51e
000003D2  59                pop cx
000003D3  59                pop cx
000003D4  A0052F            mov al,[0x2f05]
000003D7  98                cbw
000003D8  99                cwd
000003D9  52                push dx
000003DA  50                push ax
000003DB  9A1E053623        call 0x2336:0x51e
000003E0  59                pop cx
000003E1  59                pop cx
000003E2  A19A4B            mov ax,[0x4b9a]
000003E5  99                cwd
000003E6  52                push dx
000003E7  50                push ax
000003E8  9A1E053623        call 0x2336:0x51e
000003ED  59                pop cx
000003EE  59                pop cx
000003EF  A01A41            mov al,[0x411a]
000003F2  B400              mov ah,0x0
000003F4  99                cwd
000003F5  52                push dx
000003F6  50                push ax
000003F7  9A1E053623        call 0x2336:0x51e
000003FC  59                pop cx
000003FD  59                pop cx
000003FE  A0BA45            mov al,[0x45ba]
00000401  B400              mov ah,0x0
00000403  99                cwd
00000404  52                push dx
00000405  50                push ax
00000406  9A1E053623        call 0x2336:0x51e
0000040B  59                pop cx
0000040C  59                pop cx
0000040D  A1F208            mov ax,[0x8f2]
00000410  99                cwd
00000411  52                push dx
00000412  50                push ax
00000413  9A1E053623        call 0x2336:0x51e
00000418  59                pop cx
00000419  59                pop cx
0000041A  FF36F608          push word [0x8f6]
0000041E  FF36F408          push word [0x8f4]
00000422  9A1E053623        call 0x2336:0x51e
00000427  59                pop cx
00000428  59                pop cx
00000429  A0F908            mov al,[0x8f9]
0000042C  98                cbw
0000042D  99                cwd
0000042E  52                push dx
0000042F  50                push ax
00000430  9A1E053623        call 0x2336:0x51e
00000435  59                pop cx
00000436  59                pop cx
00000437  A0F808            mov al,[0x8f8]
0000043A  B400              mov ah,0x0
0000043C  99                cwd
0000043D  52                push dx
0000043E  50                push ax
0000043F  9A1E053623        call 0x2336:0x51e
00000444  59                pop cx
00000445  59                pop cx
00000446  A08045            mov al,[0x4580]
00000449  98                cbw
0000044A  99                cwd
0000044B  52                push dx
0000044C  50                push ax
0000044D  9A1E053623        call 0x2336:0x51e
00000452  59                pop cx
00000453  59                pop cx
00000454  A1EA08            mov ax,[0x8ea]
00000457  99                cwd
00000458  52                push dx
00000459  50                push ax
0000045A  9A1E053623        call 0x2336:0x51e
0000045F  59                pop cx
00000460  59                pop cx
00000461  A18E2E            mov ax,[0x2e8e]
00000464  99                cwd
00000465  52                push dx
00000466  50                push ax
00000467  9A1E053623        call 0x2336:0x51e
0000046C  59                pop cx
0000046D  59                pop cx
0000046E  A0302F            mov al,[0x2f30]
00000471  B400              mov ah,0x0
00000473  99                cwd
00000474  52                push dx
00000475  50                push ax
00000476  9A1E053623        call 0x2336:0x51e
0000047B  59                pop cx
0000047C  59                pop cx
0000047D  A0F24B            mov al,[0x4bf2]
00000480  B400              mov ah,0x0
00000482  99                cwd
00000483  52                push dx
00000484  50                push ax
00000485  9A1E053623        call 0x2336:0x51e
0000048A  59                pop cx
0000048B  59                pop cx
0000048C  A0EC08            mov al,[0x8ec]
0000048F  98                cbw
00000490  99                cwd
00000491  52                push dx
00000492  50                push ax
00000493  9A1E053623        call 0x2336:0x51e
00000498  59                pop cx
00000499  59                pop cx
0000049A  FF36F23C          push word [0x3cf2]
0000049E  FF36F03C          push word [0x3cf0]
000004A2  9A1E053623        call 0x2336:0x51e
000004A7  59                pop cx
000004A8  59                pop cx
000004A9  FF363844          push word [0x4438]
000004AD  FF363644          push word [0x4436]
000004B1  9A1E053623        call 0x2336:0x51e
000004B6  59                pop cx
000004B7  59                pop cx
000004B8  B88000            mov ax,0x80
000004BB  50                push ax
000004BC  B80283            mov ax,0x8302
000004BF  50                push ax
000004C0  1E                push ds
000004C1  B83E11            mov ax,0x113e
000004C4  50                push ax
000004C5  9AED3F0000        call 0x0:0x3fed
000004CA  83C408            add sp,byte +0x8
000004CD  8BF8              mov di,ax
000004CF  83FFFF            cmp di,byte -0x1
000004D2  7502              jnz 0x4d6
000004D4  EB23              jmp short 0x4f9
000004D6  33C0              xor ax,ax
000004D8  BA0009            mov dx,0x900
000004DB  50                push ax
000004DC  52                push dx
000004DD  FF76FE            push word [bp-0x2]
000004E0  FF76FC            push word [bp-0x4]
000004E3  57                push di
000004E4  9A4E06730D        call 0xd73:0x64e
000004E9  83C40A            add sp,byte +0xa
000004EC  8946F8            mov [bp-0x8],ax
000004EF  8956FA            mov [bp-0x6],dx
000004F2  57                push di
000004F3  9ADC260000        call 0x0:0x26dc
000004F8  59                pop cx
000004F9  FF36DA08          push word [0x8da]
000004FD  9A02006C18        call 0x186c:0x2
00000502  59                pop cx
00000503  837EFA00          cmp word [bp-0x6],byte +0x0
00000507  7507              jnz 0x510
00000509  817EF80009        cmp word [bp-0x8],0x900
0000050E  7404              jz 0x514
00000510  B000              mov al,0x0
00000512  EB04              jmp short 0x518
00000514  B001              mov al,0x1
00000516  EB00              jmp short 0x518
00000518  5F                pop di
00000519  5E                pop si
0000051A  8BE5              mov sp,bp
0000051C  5D                pop bp
0000051D  CB                retf

; ---- 2336:051E ----
0000051E  55                push bp
0000051F  8BEC              mov bp,sp
00000521  C41EF857          les bx,[0x57f8]
00000525  8B4608            mov ax,[bp+0x8]
00000528  8B5606            mov dx,[bp+0x6]
0000052B  268917            mov [es:bx],dx
0000052E  26894702          mov [es:bx+0x2],ax
00000532  8306F85704        add word [0x57f8],byte +0x4
00000537  5D                pop bp
00000538  CB                retf

; ---- 2336:0539 ----
00000539  55                push bp
0000053A  8BEC              mov bp,sp
0000053C  83EC04            sub sp,byte +0x4
0000053F  56                push si
00000540  57                push di
00000541  A1D845            mov ax,[0x45d8]
00000544  8B16D645          mov dx,[0x45d6]
00000548  8956FC            mov [bp-0x4],dx
0000054B  8946FE            mov [bp-0x2],ax
0000054E  8B46FE            mov ax,[bp-0x2]
00000551  8B56FC            mov dx,[bp-0x4]
00000554  8916F857          mov [0x57f8],dx
00000558  A3FA57            mov [0x57fa],ax
0000055B  1E                push ds
0000055C  B84B11            mov ax,0x114b
0000055F  50                push ax
00000560  FF76FE            push word [bp-0x2]
00000563  FF76FC            push word [bp-0x4]
00000566  9AB10A6C18        call 0x186c:0xab1
0000056B  83C408            add sp,byte +0x8
0000056E  8BF8              mov di,ax
00000570  81FF0009          cmp di,0x900
00000574  740F              jz 0x585
00000576  FF36DA08          push word [0x8da]
0000057A  9A02006C18        call 0x186c:0x2
0000057F  59                pop cx
00000580  B000              mov al,0x0
00000582  E9BC03            jmp 0x941
00000585  C606AA3700        mov byte [0x37aa],0x0
0000058A  E92402            jmp 0x7b1
0000058D  9A47093623        call 0x2336:0x947
00000592  50                push ax
00000593  A0AA37            mov al,[0x37aa]
00000596  98                cbw
00000597  8BD8              mov bx,ax
00000599  58                pop ax
0000059A  8887FE40          mov [bx+0x40fe],al
0000059E  9A47093623        call 0x2336:0x947
000005A3  50                push ax
000005A4  A0AA37            mov al,[0x37aa]
000005A7  98                cbw
000005A8  8BD8              mov bx,ax
000005AA  58                pop ax
000005AB  88870241          mov [bx+0x4102],al
000005AF  9A47093623        call 0x2336:0x947
000005B4  50                push ax
000005B5  A0AA37            mov al,[0x37aa]
000005B8  98                cbw
000005B9  D1E0              shl ax,1
000005BB  8BD8              mov bx,ax
000005BD  58                pop ax
000005BE  8987AC4B          mov [bx+0x4bac],ax
000005C2  9A47093623        call 0x2336:0x947
000005C7  50                push ax
000005C8  A0AA37            mov al,[0x37aa]
000005CB  98                cbw
000005CC  D1E0              shl ax,1
000005CE  8BD8              mov bx,ax
000005D0  58                pop ax
000005D1  89872045          mov [bx+0x4520],ax
000005D5  9A47093623        call 0x2336:0x947
000005DA  50                push ax
000005DB  A0AA37            mov al,[0x37aa]
000005DE  98                cbw
000005DF  D1E0              shl ax,1
000005E1  8BD8              mov bx,ax
000005E3  58                pop ax
000005E4  89870E30          mov [bx+0x300e],ax
000005E8  9A47093623        call 0x2336:0x947
000005ED  50                push ax
000005EE  A0AA37            mov al,[0x37aa]
000005F1  98                cbw
000005F2  8BD8              mov bx,ax
000005F4  58                pop ax
000005F5  8887AF37          mov [bx+0x37af],al
000005F9  9A47093623        call 0x2336:0x947
000005FE  50                push ax
000005FF  A0AA37            mov al,[0x37aa]
00000602  98                cbw
00000603  D1E0              shl ax,1
00000605  D1E0              shl ax,1
00000607  8BD8              mov bx,ax
00000609  58                pop ax
0000060A  8987D23C          mov [bx+0x3cd2],ax
0000060E  8997D43C          mov [bx+0x3cd4],dx
00000612  9A47093623        call 0x2336:0x947
00000617  50                push ax
00000618  A0AA37            mov al,[0x37aa]
0000061B  98                cbw
0000061C  D1E0              shl ax,1
0000061E  8BD8              mov bx,ax
00000620  58                pop ax
00000621  89870E45          mov [bx+0x450e],ax
00000625  9A47093623        call 0x2336:0x947
0000062A  50                push ax
0000062B  A0AA37            mov al,[0x37aa]
0000062E  98                cbw
0000062F  8BD8              mov bx,ax
00000631  58                pop ax
00000632  8887C24B          mov [bx+0x4bc2],al
00000636  9A47093623        call 0x2336:0x947
0000063B  50                push ax
0000063C  A0AA37            mov al,[0x37aa]
0000063F  98                cbw
00000640  D1E0              shl ax,1
00000642  8BD8              mov bx,ax
00000644  58                pop ax
00000645  8987F640          mov [bx+0x40f6],ax
00000649  9A47093623        call 0x2336:0x947
0000064E  50                push ax
0000064F  A0AA37            mov al,[0x37aa]
00000652  98                cbw
00000653  8BD8              mov bx,ax
00000655  58                pop ax
00000656  88874008          mov [bx+0x840],al
0000065A  9A47093623        call 0x2336:0x947
0000065F  50                push ax
00000660  A0AA37            mov al,[0x37aa]
00000663  98                cbw
00000664  8BD8              mov bx,ax
00000666  58                pop ax
00000667  88873C08          mov [bx+0x83c],al
0000066B  9A47093623        call 0x2336:0x947
00000670  50                push ax
00000671  A0AA37            mov al,[0x37aa]
00000674  98                cbw
00000675  8BD8              mov bx,ax
00000677  58                pop ax
00000678  8887BE4B          mov [bx+0x4bbe],al
0000067C  9A47093623        call 0x2336:0x947
00000681  50                push ax
00000682  A0AA37            mov al,[0x37aa]
00000685  98                cbw
00000686  8BD8              mov bx,ax
00000688  58                pop ax
00000689  8887A84B          mov [bx+0x4ba8],al
0000068D  C706D0450000      mov word [0x45d0],0x0
00000693  EB3C              jmp short 0x6d1
00000695  9A47093623        call 0x2336:0x947
0000069A  50                push ax
0000069B  A0AA37            mov al,[0x37aa]
0000069E  98                cbw
0000069F  D1E0              shl ax,1
000006A1  D1E0              shl ax,1
000006A3  0306D045          add ax,[0x45d0]
000006A7  8BD8              mov bx,ax
000006A9  58                pop ax
000006AA  8887E72E          mov [bx+0x2ee7],al
000006AE  9A47093623        call 0x2336:0x947
000006B3  50                push ax
000006B4  A0AA37            mov al,[0x37aa]
000006B7  98                cbw
000006B8  D1E0              shl ax,1
000006BA  D1E0              shl ax,1
000006BC  D1E0              shl ax,1
000006BE  8B16D045          mov dx,[0x45d0]
000006C2  D1E2              shl dx,1
000006C4  03C2              add ax,dx
000006C6  8BD8              mov bx,ax
000006C8  58                pop ax
000006C9  8987F43C          mov [bx+0x3cf4],ax
000006CD  FF06D045          inc word [0x45d0]
000006D1  833ED04503        cmp word [0x45d0],byte +0x3
000006D6  7EBD              jng 0x695
000006D8  33F6              xor si,si
000006DA  EB25              jmp short 0x701
000006DC  9A47093623        call 0x2336:0x947
000006E1  50                push ax
000006E2  A0AA37            mov al,[0x37aa]
000006E5  98                cbw
000006E6  BBA000            mov bx,0xa0
000006E9  52                push dx
000006EA  F7EB              imul bx
000006EC  8BD6              mov dx,si
000006EE  D1E2              shl dx,1
000006F0  D1E2              shl dx,1
000006F2  03C2              add ax,dx
000006F4  8BD8              mov bx,ax
000006F6  58                pop ax
000006F7  5A                pop dx
000006F8  89971C41          mov [bx+0x411c],dx
000006FC  89871E41          mov [bx+0x411e],ax
00000700  46                inc si
00000701  83FE27            cmp si,byte +0x27
00000704  7ED6              jng 0x6dc
00000706  33F6              xor si,si
00000708  EB17              jmp short 0x721
0000070A  9A47093623        call 0x2336:0x947
0000070F  50                push ax
00000710  A0AA37            mov al,[0x37aa]
00000713  98                cbw
00000714  BA0600            mov dx,0x6
00000717  F7EA              imul dx
00000719  8BD8              mov bx,ax
0000071B  58                pop ax
0000071C  8880CE43          mov [bx+si+0x43ce],al
00000720  46                inc si
00000721  83FE05            cmp si,byte +0x5
00000724  7EE4              jng 0x70a
00000726  9A47093623        call 0x2336:0x947
0000072B  50                push ax
0000072C  A0AA37            mov al,[0x37aa]
0000072F  98                cbw
00000730  D1E0              shl ax,1
00000732  D1E0              shl ax,1
00000734  8BD8              mov bx,ax
00000736  58                pop ax
00000737  89876C37          mov [bx+0x376c],ax
0000073B  9A47093623        call 0x2336:0x947
00000740  50                push ax
00000741  A0AA37            mov al,[0x37aa]
00000744  98                cbw
00000745  D1E0              shl ax,1
00000747  D1E0              shl ax,1
00000749  8BD8              mov bx,ax
0000074B  58                pop ax
0000074C  89876E37          mov [bx+0x376e],ax
00000750  9A47093623        call 0x2336:0x947
00000755  50                push ax
00000756  A0AA37            mov al,[0x37aa]
00000759  98                cbw
0000075A  D1E0              shl ax,1
0000075C  8BD8              mov bx,ax
0000075E  58                pop ax
0000075F  8987F82E          mov [bx+0x2ef8],ax
00000763  9A47093623        call 0x2336:0x947
00000768  50                push ax
00000769  A0AA37            mov al,[0x37aa]
0000076C  98                cbw
0000076D  8BD8              mov bx,ax
0000076F  58                pop ax
00000770  88877C37          mov [bx+0x377c],al
00000774  9A47093623        call 0x2336:0x947
00000779  50                push ax
0000077A  A0AA37            mov al,[0x37aa]
0000077D  98                cbw
0000077E  D1E0              shl ax,1
00000780  8BD8              mov bx,ax
00000782  58                pop ax
00000783  8987EC40          mov [bx+0x40ec],ax
00000787  9A47093623        call 0x2336:0x947
0000078C  50                push ax
0000078D  A0AA37            mov al,[0x37aa]
00000790  98                cbw
00000791  D1E0              shl ax,1
00000793  8BD8              mov bx,ax
00000795  58                pop ax
00000796  8987DC33          mov [bx+0x33dc],ax
0000079A  9A47093623        call 0x2336:0x947
0000079F  50                push ax
000007A0  A0AA37            mov al,[0x37aa]
000007A3  98                cbw
000007A4  D1E0              shl ax,1
000007A6  8BD8              mov bx,ax
000007A8  58                pop ax
000007A9  89871630          mov [bx+0x3016],ax
000007AD  FE06AA37          inc byte [0x37aa]
000007B1  803EAA3704        cmp byte [0x37aa],0x4
000007B6  7D03              jnl 0x7bb
000007B8  E9D2FD            jmp 0x58d
000007BB  C706D0450000      mov word [0x45d0],0x0
000007C1  EB13              jmp short 0x7d6
000007C3  9A47093623        call 0x2336:0x947
000007C8  8B1ED045          mov bx,[0x45d0]
000007CC  D1E3              shl bx,1
000007CE  89879848          mov [bx+0x4898],ax
000007D2  FF06D045          inc word [0x45d0]
000007D6  833ED04503        cmp word [0x45d0],byte +0x3
000007DB  7EE6              jng 0x7c3
000007DD  C606002F00        mov byte [0x2f00],0x0
000007E2  EB6A              jmp short 0x84e
000007E4  C606B02E00        mov byte [0x2eb0],0x0
000007E9  EB58              jmp short 0x843
000007EB  9A47093623        call 0x2336:0x947
000007F0  50                push ax
000007F1  A0B02E            mov al,[0x2eb0]
000007F4  98                cbw
000007F5  BA0600            mov dx,0x6
000007F8  F7EA              imul dx
000007FA  50                push ax
000007FB  A0002F            mov al,[0x2f00]
000007FE  98                cbw
000007FF  5B                pop bx
00000800  03D8              add bx,ax
00000802  58                pop ax
00000803  88874244          mov [bx+0x4442],al
00000807  9A47093623        call 0x2336:0x947
0000080C  50                push ax
0000080D  A0B02E            mov al,[0x2eb0]
00000810  98                cbw
00000811  BA0600            mov dx,0x6
00000814  F7EA              imul dx
00000816  50                push ax
00000817  A0002F            mov al,[0x2f00]
0000081A  98                cbw
0000081B  5B                pop bx
0000081C  03D8              add bx,ax
0000081E  58                pop ax
0000081F  8887B12E          mov [bx+0x2eb1],al
00000823  9A47093623        call 0x2336:0x947
00000828  50                push ax
00000829  A0B02E            mov al,[0x2eb0]
0000082C  98                cbw
0000082D  BA0600            mov dx,0x6
00000830  F7EA              imul dx
00000832  50                push ax
00000833  A0002F            mov al,[0x2f00]
00000836  98                cbw
00000837  5B                pop bx
00000838  03D8              add bx,ax
0000083A  58                pop ax
0000083B  8887DA45          mov [bx+0x45da],al
0000083F  FE06B02E          inc byte [0x2eb0]
00000843  803EB02E08        cmp byte [0x2eb0],0x8
00000848  7EA1              jng 0x7eb
0000084A  FE06002F          inc byte [0x2f00]
0000084E  803E002F05        cmp byte [0x2f00],0x5
00000853  7E8F              jng 0x7e4
00000855  9A47093623        call 0x2336:0x947
0000085A  A3FC08            mov [0x8fc],ax
0000085D  9A47093623        call 0x2336:0x947
00000862  A3FE08            mov [0x8fe],ax
00000865  33F6              xor si,si
00000867  EB24              jmp short 0x88d
00000869  9A47093623        call 0x2336:0x947
0000086E  88841A2F          mov [si+0x2f1a],al
00000872  9A47093623        call 0x2336:0x947
00000877  8BDE              mov bx,si
00000879  D1E3              shl bx,1
0000087B  8987F444          mov [bx+0x44f4],ax
0000087F  9A47093623        call 0x2336:0x947
00000884  8BDE              mov bx,si
00000886  D1E3              shl bx,1
00000888  89878A34          mov [bx+0x348a],ax
0000088C  46                inc si
0000088D  83FE0B            cmp si,byte +0xb
00000890  7ED7              jng 0x869
00000892  9A47093623        call 0x2336:0x947
00000897  A30009            mov [0x900],ax
0000089A  9A47093623        call 0x2336:0x947
0000089F  A2E640            mov [0x40e6],al
000008A2  9A47093623        call 0x2336:0x947
000008A7  A2052F            mov [0x2f05],al
000008AA  9A47093623        call 0x2336:0x947
000008AF  A39A4B            mov [0x4b9a],ax
000008B2  9A47093623        call 0x2336:0x947
000008B7  A21A41            mov [0x411a],al
000008BA  9A47093623        call 0x2336:0x947
000008BF  A2BA45            mov [0x45ba],al
000008C2  9A47093623        call 0x2336:0x947
000008C7  A3F208            mov [0x8f2],ax
000008CA  9A47093623        call 0x2336:0x947
000008CF  A3F408            mov [0x8f4],ax
000008D2  8916F608          mov [0x8f6],dx
000008D6  9A47093623        call 0x2336:0x947
000008DB  A2F908            mov [0x8f9],al
000008DE  9A47093623        call 0x2336:0x947
000008E3  A2F808            mov [0x8f8],al
000008E6  9A47093623        call 0x2336:0x947
000008EB  A28045            mov [0x4580],al
000008EE  9A47093623        call 0x2336:0x947
000008F3  A3EA08            mov [0x8ea],ax
000008F6  9A47093623        call 0x2336:0x947
000008FB  A38E2E            mov [0x2e8e],ax
000008FE  9A47093623        call 0x2336:0x947
00000903  A2302F            mov [0x2f30],al
00000906  9A47093623        call 0x2336:0x947
0000090B  A2F24B            mov [0x4bf2],al
0000090E  9A47093623        call 0x2336:0x947
00000913  A2EC08            mov [0x8ec],al
00000916  9A47093623        call 0x2336:0x947
0000091B  A3F03C            mov [0x3cf0],ax
0000091E  8916F23C          mov [0x3cf2],dx
00000922  9A47093623        call 0x2336:0x947
00000927  A33644            mov [0x4436],ax
0000092A  89163844          mov [0x4438],dx
0000092E  9A58002D19        call 0x192d:0x58
00000933  FF36DA08          push word [0x8da]
00000937  9A02006C18        call 0x186c:0x2
0000093C  59                pop cx
0000093D  B001              mov al,0x1
0000093F  EB00              jmp short 0x941
00000941  5F                pop di
00000942  5E                pop si
00000943  8BE5              mov sp,bp
00000945  5D                pop bp
00000946  CB                retf

; ---- 2336:0947 ----
00000947  55                push bp
00000948  8BEC              mov bp,sp
0000094A  83EC04            sub sp,byte +0x4
0000094D  C41EF857          les bx,[0x57f8]
00000951  268B4702          mov ax,[es:bx+0x2]
00000955  268B17            mov dx,[es:bx]
00000958  8956FC            mov [bp-0x4],dx
0000095B  8946FE            mov [bp-0x2],ax
0000095E  8306F85704        add word [0x57f8],byte +0x4
00000963  8B56FE            mov dx,[bp-0x2]
00000966  8B46FC            mov ax,[bp-0x4]
00000969  EB00              jmp short 0x96b
0000096B  8BE5              mov sp,bp
0000096D  5D                pop bp
0000096E  CB                retf
0000096F  00                db 0x00
