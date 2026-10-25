; segment 0825  file offset 0xB850  size 0xAA0
00000000  98                cbw
00000001  50                push ax
00000002  9A5E04AF21        call 0x21af:0x45e
00000007  59                pop cx
00000008  5D                pop bp
00000009  CB                retf

; ---- 0825:000A ----
0000000A  55                push bp
0000000B  8BEC              mov bp,sp
0000000D  9ACB062508        call 0x825:0x6cb
00000012  FF36CE08          push word [0x8ce]
00000016  9A02006C18        call 0x186c:0x2
0000001B  59                pop cx
0000001C  807E0601          cmp byte [bp+0x6],0x1
00000020  7505              jnz 0x27
00000022  9AC9092508        call 0x825:0x9c9
00000027  9A72002508        call 0x825:0x72
0000002C  9A78012508        call 0x825:0x178
00000031  9AD7022508        call 0x825:0x2d7
00000036  9A64032508        call 0x825:0x364
0000003B  9A7D042508        call 0x825:0x47d
00000040  9A60052508        call 0x825:0x560
00000045  FF362E08          push word [0x82e]
00000049  9A3F0C960A        call 0xa96:0xc3f
0000004E  59                pop cx
0000004F  1E                push ds
00000050  B8A800            mov ax,0xa8
00000053  50                push ax
00000054  9A6C02FD21        call 0x21fd:0x26c
00000059  59                pop cx
0000005A  59                pop cx
0000005B  9A2903FD21        call 0x21fd:0x329
00000060  807E0601          cmp byte [bp+0x6],0x1
00000064  750A              jnz 0x70
00000066  B80200            mov ax,0x2
00000069  50                push ax
0000006A  9A0400FD21        call 0x21fd:0x4
0000006F  59                pop cx
00000070  5D                pop bp
00000071  CB                retf

; ---- 0825:0072 ----
00000072  55                push bp
00000073  8BEC              mov bp,sp
00000075  9A1B00960A        call 0xa96:0x1b
0000007A  9AD91E0906        call 0x609:0x1ed9
0000007F  B8C800            mov ax,0xc8
00000082  50                push ax
00000083  B84001            mov ax,0x140
00000086  50                push ax
00000087  33C0              xor ax,ax
00000089  50                push ax
0000008A  33C0              xor ax,ax
0000008C  50                push ax
0000008D  9A56120906        call 0x609:0x1256
00000092  83C408            add sp,byte +0x8
00000095  B80500            mov ax,0x5
00000098  50                push ax
00000099  B87900            mov ax,0x79
0000009C  50                push ax
0000009D  B8C400            mov ax,0xc4
000000A0  50                push ax
000000A1  B81600            mov ax,0x16
000000A4  50                push ax
000000A5  B83700            mov ax,0x37
000000A8  50                push ax
000000A9  9AD704960A        call 0xa96:0x4d7
000000AE  83C40A            add sp,byte +0xa
000000B1  B80500            mov ax,0x5
000000B4  50                push ax
000000B5  B82A00            mov ax,0x2a
000000B8  50                push ax
000000B9  B8C400            mov ax,0xc4
000000BC  50                push ax
000000BD  B89500            mov ax,0x95
000000C0  50                push ax
000000C1  B83700            mov ax,0x37
000000C4  50                push ax
000000C5  9AD704960A        call 0xa96:0x4d7
000000CA  83C40A            add sp,byte +0xa
000000CD  B80900            mov ax,0x9
000000D0  50                push ax
000000D1  B82A00            mov ax,0x2a
000000D4  50                push ax
000000D5  B8C400            mov ax,0xc4
000000D8  50                push ax
000000D9  B89500            mov ax,0x95
000000DC  50                push ax
000000DD  B83700            mov ax,0x37
000000E0  50                push ax
000000E1  9A8104960A        call 0xa96:0x481
000000E6  83C40A            add sp,byte +0xa
000000E9  C606AA3700        mov byte [0x37aa],0x0
000000EE  EB32              jmp short 0x122
000000F0  A0AA37            mov al,[0x37aa]
000000F3  98                cbw
000000F4  8BD8              mov bx,ax
000000F6  8A87423D          mov al,[bx+0x3d42]
000000FA  B400              mov ah,0x0
000000FC  50                push ax
000000FD  A0AA37            mov al,[0x37aa]
00000100  98                cbw
00000101  8BD8              mov bx,ax
00000103  8A872A30          mov al,[bx+0x302a]
00000107  B400              mov ah,0x0
00000109  BA2C00            mov dx,0x2c
0000010C  F7EA              imul dx
0000010E  050E00            add ax,0xe
00000111  50                push ax
00000112  B80001            mov ax,0x100
00000115  50                push ax
00000116  9A79180906        call 0x609:0x1879
0000011B  83C406            add sp,byte +0x6
0000011E  FE06AA37          inc byte [0x37aa]
00000122  803EAA3704        cmp byte [0x37aa],0x4
00000127  7CC7              jl 0xf0
00000129  9AA806960A        call 0xa96:0x6a8
0000012E  B80100            mov ax,0x1
00000131  50                push ax
00000132  9ADB05FD21        call 0x21fd:0x5db
00000137  59                pop cx
00000138  9A1B00960A        call 0xa96:0x1b
0000013D  B80300            mov ax,0x3
00000140  50                push ax
00000141  B8FBFF            mov ax,0xfffb
00000144  50                push ax
00000145  B80A00            mov ax,0xa
00000148  50                push ax
00000149  B80500            mov ax,0x5
0000014C  50                push ax
0000014D  FF36F637          push word [0x37f6]
00000151  FF36F437          push word [0x37f4]
00000155  B84001            mov ax,0x140
00000158  50                push ax
00000159  B81000            mov ax,0x10
0000015C  50                push ax
0000015D  33C0              xor ax,ax
0000015F  50                push ax
00000160  9AD300690A        call 0xa69:0xd3
00000165  83C412            add sp,byte +0x12
00000168  9AA806960A        call 0xa96:0x6a8
0000016D  33C0              xor ax,ax
0000016F  50                push ax
00000170  9ADB05FD21        call 0x21fd:0x5db
00000175  59                pop cx
00000176  5D                pop bp
00000177  CB                retf

; ---- 0825:0178 ----
00000178  55                push bp
00000179  8BEC              mov bp,sp
0000017B  4C                dec sp
0000017C  4C                dec sp
0000017D  56                push si
0000017E  57                push di
0000017F  9A1B00960A        call 0xa96:0x1b
00000184  C746FE0000        mov word [bp-0x2],0x0
00000189  EB32              jmp short 0x1bd
0000018B  B8FFFF            mov ax,0xffff
0000018E  50                push ax
0000018F  B80200            mov ax,0x2
00000192  50                push ax
00000193  8B5EFE            mov bx,[bp-0x2]
00000196  D1E3              shl bx,1
00000198  D1E3              shl bx,1
0000019A  FFB7FA37          push word [bx+0x37fa]
0000019E  FFB7F837          push word [bx+0x37f8]
000001A2  8B46FE            mov ax,[bp-0x2]
000001A5  BA0600            mov dx,0x6
000001A8  F7EA              imul dx
000001AA  059B00            add ax,0x9b
000001AD  50                push ax
000001AE  B83900            mov ax,0x39
000001B1  50                push ax
000001B2  9A2505960A        call 0xa96:0x525
000001B7  83C40C            add sp,byte +0xc
000001BA  FF46FE            inc word [bp-0x2]
000001BD  837EFE04          cmp word [bp-0x2],byte +0x4
000001C1  7EC8              jng 0x18b
000001C3  B8FFFF            mov ax,0xffff
000001C6  50                push ax
000001C7  B80200            mov ax,0x2
000001CA  50                push ax
000001CB  FF360E38          push word [0x380e]
000001CF  FF360C38          push word [0x380c]
000001D3  B8BC00            mov ax,0xbc
000001D6  50                push ax
000001D7  B83900            mov ax,0x39
000001DA  50                push ax
000001DB  9A2505960A        call 0xa96:0x525
000001E0  83C40C            add sp,byte +0xc
000001E3  C606AA3700        mov byte [0x37aa],0x0
000001E8  E9D700            jmp 0x2c2
000001EB  A0AA37            mov al,[0x37aa]
000001EE  98                cbw
000001EF  D1E0              shl ax,1
000001F1  8BD8              mov bx,ax
000001F3  8BBFC800          mov di,[bx+0xc8]
000001F7  A0AA37            mov al,[0x37aa]
000001FA  98                cbw
000001FB  BA2800            mov dx,0x28
000001FE  F7EA              imul dx
00000200  055D00            add ax,0x5d
00000203  8BF0              mov si,ax
00000205  B8FFFF            mov ax,0xffff
00000208  50                push ax
00000209  57                push di
0000020A  B80500            mov ax,0x5
0000020D  50                push ax
0000020E  A0AA37            mov al,[0x37aa]
00000211  98                cbw
00000212  D1E0              shl ax,1
00000214  D1E0              shl ax,1
00000216  8BD8              mov bx,ax
00000218  FFB7D43C          push word [bx+0x3cd4]
0000021C  FFB7D23C          push word [bx+0x3cd2]
00000220  B89B00            mov ax,0x9b
00000223  50                push ax
00000224  56                push si
00000225  9AFB05960A        call 0xa96:0x5fb
0000022A  83C40E            add sp,byte +0xe
0000022D  B8FFFF            mov ax,0xffff
00000230  50                push ax
00000231  57                push di
00000232  B80500            mov ax,0x5
00000235  50                push ax
00000236  9A6F082508        call 0x825:0x86f
0000023B  52                push dx
0000023C  50                push ax
0000023D  B8A100            mov ax,0xa1
00000240  50                push ax
00000241  56                push si
00000242  9AFB05960A        call 0xa96:0x5fb
00000247  83C40E            add sp,byte +0xe
0000024A  B8FFFF            mov ax,0xffff
0000024D  50                push ax
0000024E  57                push di
0000024F  B80500            mov ax,0x5
00000252  50                push ax
00000253  9A8D082508        call 0x825:0x88d
00000258  52                push dx
00000259  50                push ax
0000025A  B8A700            mov ax,0xa7
0000025D  50                push ax
0000025E  56                push si
0000025F  9AFB05960A        call 0xa96:0x5fb
00000264  83C40E            add sp,byte +0xe
00000267  B8FFFF            mov ax,0xffff
0000026A  50                push ax
0000026B  57                push di
0000026C  B80500            mov ax,0x5
0000026F  50                push ax
00000270  9AEB082508        call 0x825:0x8eb
00000275  52                push dx
00000276  50                push ax
00000277  B8AD00            mov ax,0xad
0000027A  50                push ax
0000027B  56                push si
0000027C  9AFB05960A        call 0xa96:0x5fb
00000281  83C40E            add sp,byte +0xe
00000284  B8FFFF            mov ax,0xffff
00000287  50                push ax
00000288  57                push di
00000289  B80500            mov ax,0x5
0000028C  50                push ax
0000028D  9A6D092508        call 0x825:0x96d
00000292  52                push dx
00000293  50                push ax
00000294  B8B300            mov ax,0xb3
00000297  50                push ax
00000298  56                push si
00000299  9AFB05960A        call 0xa96:0x5fb
0000029E  83C40E            add sp,byte +0xe
000002A1  B8FFFF            mov ax,0xffff
000002A4  50                push ax
000002A5  57                push di
000002A6  B80500            mov ax,0x5
000002A9  50                push ax
000002AA  9A13082508        call 0x825:0x813
000002AF  52                push dx
000002B0  50                push ax
000002B1  B8BC00            mov ax,0xbc
000002B4  50                push ax
000002B5  56                push si
000002B6  9AFB05960A        call 0xa96:0x5fb
000002BB  83C40E            add sp,byte +0xe
000002BE  FE06AA37          inc byte [0x37aa]
000002C2  803EAA3704        cmp byte [0x37aa],0x4
000002C7  7D03              jnl 0x2cc
000002C9  E91FFF            jmp 0x1eb
000002CC  9AA806960A        call 0xa96:0x6a8
000002D1  5F                pop di
000002D2  5E                pop si
000002D3  8BE5              mov sp,bp
000002D5  5D                pop bp
000002D6  CB                retf

; ---- 0825:02D7 ----
000002D7  55                push bp
000002D8  8BEC              mov bp,sp
000002DA  83EC04            sub sp,byte +0x4
000002DD  C606AA3700        mov byte [0x37aa],0x0
000002E2  EB29              jmp short 0x30d
000002E4  A0AA37            mov al,[0x37aa]
000002E7  98                cbw
000002E8  8BD8              mov bx,ax
000002EA  80BF423D00        cmp byte [bx+0x3d42],0x0
000002EF  7518              jnz 0x309
000002F1  A0AA37            mov al,[0x37aa]
000002F4  98                cbw
000002F5  D1E0              shl ax,1
000002F7  D1E0              shl ax,1
000002F9  8BD8              mov bx,ax
000002FB  8B87082F          mov ax,[bx+0x2f08]
000002FF  8B97062F          mov dx,[bx+0x2f06]
00000303  8956FC            mov [bp-0x4],dx
00000306  8946FE            mov [bp-0x2],ax
00000309  FE06AA37          inc byte [0x37aa]
0000030D  803EAA3704        cmp byte [0x37aa],0x4
00000312  7CD0              jl 0x2e4
00000314  33C0              xor ax,ax
00000316  BA7800            mov dx,0x78
00000319  50                push ax
0000031A  52                push dx
0000031B  FF76FE            push word [bp-0x2]
0000031E  FF76FC            push word [bp-0x4]
00000321  9A894F0000        call 0x0:0x4f89
00000326  40                inc ax
00000327  A38415            mov [0x1584],ax
0000032A  833E84150A        cmp word [0x1584],byte +0xa
0000032F  7E14              jng 0x345
00000331  A18415            mov ax,[0x1584]
00000334  BB0A00            mov bx,0xa
00000337  99                cwd
00000338  F7FB              idiv bx
0000033A  BA0A00            mov dx,0xa
0000033D  F7EA              imul dx
0000033F  050A00            add ax,0xa
00000342  A38415            mov [0x1584],ax
00000345  833E841564        cmp word [0x1584],byte +0x64
0000034A  7E14              jng 0x360
0000034C  A18415            mov ax,[0x1584]
0000034F  BB6400            mov bx,0x64
00000352  99                cwd
00000353  F7FB              idiv bx
00000355  BA6400            mov dx,0x64
00000358  F7EA              imul dx
0000035A  056400            add ax,0x64
0000035D  A38415            mov [0x1584],ax
00000360  8BE5              mov sp,bp
00000362  5D                pop bp
00000363  CB                retf

; ---- 0825:0364 ----
00000364  55                push bp
00000365  8BEC              mov bp,sp
00000367  83EC04            sub sp,byte +0x4
0000036A  9A1B00960A        call 0xa96:0x1b
0000036F  C746FC0000        mov word [bp-0x4],0x0
00000374  C746FE0000        mov word [bp-0x2],0x0
00000379  E98B00            jmp 0x407
0000037C  B80100            mov ax,0x1
0000037F  50                push ax
00000380  B8FFFF            mov ax,0xffff
00000383  50                push ax
00000384  B80B00            mov ax,0xb
00000387  50                push ax
00000388  B80500            mov ax,0x5
0000038B  50                push ax
0000038C  B80500            mov ax,0x5
0000038F  50                push ax
00000390  A18415            mov ax,[0x1584]
00000393  99                cwd
00000394  50                push ax
00000395  52                push dx
00000396  8B56FE            mov dx,[bp-0x2]
00000399  8B46FC            mov ax,[bp-0x4]
0000039C  59                pop cx
0000039D  5B                pop bx
0000039E  9A4E4F0000        call 0x0:0x4f4e
000003A3  50                push ax
000003A4  52                push dx
000003A5  33D2              xor dx,dx
000003A7  B80A00            mov ax,0xa
000003AA  59                pop cx
000003AB  5B                pop bx
000003AC  9A4E4F0000        call 0x0:0x4f4e
000003B1  52                push dx
000003B2  50                push ax
000003B3  8B4EFE            mov cx,[bp-0x2]
000003B6  8B5EFC            mov bx,[bp-0x4]
000003B9  33D2              xor dx,dx
000003BB  B80A00            mov ax,0xa
000003BE  9A4E4F0000        call 0x0:0x4f4e
000003C3  BA9000            mov dx,0x90
000003C6  2BD0              sub dx,ax
000003C8  52                push dx
000003C9  B81000            mov ax,0x10
000003CC  50                push ax
000003CD  9A6401690A        call 0xa69:0x164
000003D2  83C412            add sp,byte +0x12
000003D5  B80900            mov ax,0x9
000003D8  50                push ax
000003D9  B8C400            mov ax,0xc4
000003DC  50                push ax
000003DD  8B4EFE            mov cx,[bp-0x2]
000003E0  8B5EFC            mov bx,[bp-0x4]
000003E3  33D2              xor dx,dx
000003E5  B80A00            mov ax,0xa
000003E8  9A4E4F0000        call 0x0:0x4f4e
000003ED  BA8E00            mov dx,0x8e
000003F0  2BD0              sub dx,ax
000003F2  52                push dx
000003F3  B83700            mov ax,0x37
000003F6  50                push ax
000003F7  9A0B00690A        call 0xa69:0xb
000003FC  83C408            add sp,byte +0x8
000003FF  8346FC01          add word [bp-0x4],byte +0x1
00000403  8356FE00          adc word [bp-0x2],byte +0x0
00000407  837EFE00          cmp word [bp-0x2],byte +0x0
0000040B  7D03              jnl 0x410
0000040D  E96CFF            jmp 0x37c
00000410  7509              jnz 0x41b
00000412  837EFC0D          cmp word [bp-0x4],byte +0xd
00000416  7303              jnc 0x41b
00000418  E961FF            jmp 0x37c
0000041B  A18415            mov ax,[0x1584]
0000041E  99                cwd
0000041F  50                push ax
00000420  52                push dx
00000421  33D2              xor dx,dx
00000423  B87800            mov ax,0x78
00000426  59                pop cx
00000427  5B                pop bx
00000428  9A4E4F0000        call 0x0:0x4f4e
0000042D  8946FC            mov [bp-0x4],ax
00000430  8956FE            mov [bp-0x2],dx
00000433  8B46FE            mov ax,[bp-0x2]
00000436  8B56FC            mov dx,[bp-0x4]
00000439  3B06F608          cmp ax,[0x8f6]
0000043D  7235              jc 0x474
0000043F  7506              jnz 0x447
00000441  3B16F408          cmp dx,[0x8f4]
00000445  722D              jc 0x474
00000447  B80200            mov ax,0x2
0000044A  50                push ax
0000044B  B8C400            mov ax,0xc4
0000044E  50                push ax
0000044F  A18415            mov ax,[0x1584]
00000452  99                cwd
00000453  52                push dx
00000454  50                push ax
00000455  FF36F608          push word [0x8f6]
00000459  FF36F408          push word [0x8f4]
0000045D  9A904F0000        call 0x0:0x4f90
00000462  BA8E00            mov dx,0x8e
00000465  2BD0              sub dx,ax
00000467  52                push dx
00000468  B83700            mov ax,0x37
0000046B  50                push ax
0000046C  9A0B00690A        call 0xa69:0xb
00000471  83C408            add sp,byte +0x8
00000474  9AA806960A        call 0xa96:0x6a8
00000479  8BE5              mov sp,bp
0000047B  5D                pop bp
0000047C  CB                retf

; ---- 0825:047D ----
0000047D  55                push bp
0000047E  8BEC              mov bp,sp
00000480  83EC04            sub sp,byte +0x4
00000483  56                push si
00000484  9A1B00960A        call 0xa96:0x1b
00000489  A19A4B            mov ax,[0x4b9a]
0000048C  05D9FF            add ax,0xffd9
0000048F  8BF0              mov si,ax
00000491  0BC0              or ax,ax
00000493  7D02              jnl 0x497
00000495  33F6              xor si,si
00000497  C746FC0000        mov word [bp-0x4],0x0
0000049C  C746FE0000        mov word [bp-0x2],0x0
000004A1  E99E00            jmp 0x542
000004A4  33C0              xor ax,ax
000004A6  BA0A00            mov dx,0xa
000004A9  50                push ax
000004AA  52                push dx
000004AB  8BC6              mov ax,si
000004AD  99                cwd
000004AE  0346FC            add ax,[bp-0x4]
000004B1  1356FE            adc dx,[bp-0x2]
000004B4  52                push dx
000004B5  50                push ax
000004B6  9A984F0000        call 0x0:0x4f98
000004BB  0BC2              or ax,dx
000004BD  757B              jnz 0x53a
000004BF  B80900            mov ax,0x9
000004C2  50                push ax
000004C3  B86F00            mov ax,0x6f
000004C6  50                push ax
000004C7  B82000            mov ax,0x20
000004CA  50                push ax
000004CB  8B4EFE            mov cx,[bp-0x2]
000004CE  8B5EFC            mov bx,[bp-0x4]
000004D1  33D2              xor dx,dx
000004D3  B80500            mov ax,0x5
000004D6  9A4E4F0000        call 0x0:0x4f4e
000004DB  053700            add ax,0x37
000004DE  50                push ax
000004DF  9A2C00690A        call 0xa69:0x2c
000004E4  83C408            add sp,byte +0x8
000004E7  837EFE00          cmp word [bp-0x2],byte +0x0
000004EB  7F4D              jg 0x53a
000004ED  7506              jnz 0x4f5
000004EF  837EFC25          cmp word [bp-0x4],byte +0x25
000004F3  7745              ja 0x53a
000004F5  837EFE00          cmp word [bp-0x2],byte +0x0
000004F9  7C3F              jl 0x53a
000004FB  7506              jnz 0x503
000004FD  837EFC02          cmp word [bp-0x4],byte +0x2
00000501  7237              jc 0x53a
00000503  B8FFFF            mov ax,0xffff
00000506  50                push ax
00000507  B80A00            mov ax,0xa
0000050A  50                push ax
0000050B  B80200            mov ax,0x2
0000050E  50                push ax
0000050F  8BC6              mov ax,si
00000511  99                cwd
00000512  0346FC            add ax,[bp-0x4]
00000515  1356FE            adc dx,[bp-0x2]
00000518  52                push dx
00000519  50                push ax
0000051A  B81C00            mov ax,0x1c
0000051D  50                push ax
0000051E  8B4EFE            mov cx,[bp-0x2]
00000521  8B5EFC            mov bx,[bp-0x4]
00000524  33D2              xor dx,dx
00000526  B80500            mov ax,0x5
00000529  9A4E4F0000        call 0x0:0x4f4e
0000052E  053200            add ax,0x32
00000531  50                push ax
00000532  9A7A05960A        call 0xa96:0x57a
00000537  83C40E            add sp,byte +0xe
0000053A  8346FC01          add word [bp-0x4],byte +0x1
0000053E  8356FE00          adc word [bp-0x2],byte +0x0
00000542  837EFE00          cmp word [bp-0x2],byte +0x0
00000546  7D03              jnl 0x54b
00000548  E959FF            jmp 0x4a4
0000054B  7509              jnz 0x556
0000054D  837EFC28          cmp word [bp-0x4],byte +0x28
00000551  7303              jnc 0x556
00000553  E94EFF            jmp 0x4a4
00000556  9AA806960A        call 0xa96:0x6a8
0000055B  5E                pop si
0000055C  8BE5              mov sp,bp
0000055E  5D                pop bp
0000055F  CB                retf

; ---- 0825:0560 ----
00000560  55                push bp
00000561  8BEC              mov bp,sp
00000563  83EC0A            sub sp,byte +0xa
00000566  56                push si
00000567  57                push di
00000568  833E9A4B27        cmp word [0x4b9a],byte +0x27
0000056D  7F04              jg 0x573
0000056F  33C0              xor ax,ax
00000571  EB04              jmp short 0x577
00000573  A19A4B            mov ax,[0x4b9a]
00000576  40                inc ax
00000577  8BF0              mov si,ax
00000579  EB07              jmp short 0x582
0000057B  8BC6              mov ax,si
0000057D  05D8FF            add ax,0xffd8
00000580  8BF0              mov si,ax
00000582  83FE27            cmp si,byte +0x27
00000585  7FF4              jg 0x57b
00000587  33FF              xor di,di
00000589  E9C600            jmp 0x652
0000058C  3B3E9A4B          cmp di,[0x4b9a]
00000590  7E03              jng 0x595
00000592  E9C500            jmp 0x65a
00000595  9A1B00960A        call 0xa96:0x1b
0000059A  C606AA3700        mov byte [0x37aa],0x0
0000059F  E99400            jmp 0x636
000005A2  A18415            mov ax,[0x1584]
000005A5  99                cwd
000005A6  52                push dx
000005A7  50                push ax
000005A8  A0AA37            mov al,[0x37aa]
000005AB  98                cbw
000005AC  BAA000            mov dx,0xa0
000005AF  F7EA              imul dx
000005B1  8BD6              mov dx,si
000005B3  D1E2              shl dx,1
000005B5  D1E2              shl dx,1
000005B7  03C2              add ax,dx
000005B9  8BD8              mov bx,ax
000005BB  FFB71E41          push word [bx+0x411e]
000005BF  FFB71C41          push word [bx+0x411c]
000005C3  9A894F0000        call 0x0:0x4f89
000005C8  8946FE            mov [bp-0x2],ax
000005CB  837EFE78          cmp word [bp-0x2],byte +0x78
000005CF  7E05              jng 0x5d6
000005D1  C746FE7800        mov word [bp-0x2],0x78
000005D6  0BFF              or di,di
000005D8  7E42              jng 0x61c
000005DA  A0AA37            mov al,[0x37aa]
000005DD  98                cbw
000005DE  D1E0              shl ax,1
000005E0  8BD8              mov bx,ax
000005E2  FFB7C800          push word [bx+0xc8]
000005E6  B88E00            mov ax,0x8e
000005E9  2B46FE            sub ax,[bp-0x2]
000005EC  50                push ax
000005ED  8BC7              mov ax,di
000005EF  BA0500            mov dx,0x5
000005F2  F7EA              imul dx
000005F4  053700            add ax,0x37
000005F7  50                push ax
000005F8  A0AA37            mov al,[0x37aa]
000005FB  98                cbw
000005FC  D1E0              shl ax,1
000005FE  8D56F6            lea dx,[bp-0xa]
00000601  03C2              add ax,dx
00000603  8BD8              mov bx,ax
00000605  36FF37            push word [ss:bx]
00000608  8BC7              mov ax,di
0000060A  48                dec ax
0000060B  BA0500            mov dx,0x5
0000060E  F7EA              imul dx
00000610  053700            add ax,0x37
00000613  50                push ax
00000614  9A3304960A        call 0xa96:0x433
00000619  83C40A            add sp,byte +0xa
0000061C  A0AA37            mov al,[0x37aa]
0000061F  98                cbw
00000620  D1E0              shl ax,1
00000622  8D56F6            lea dx,[bp-0xa]
00000625  03C2              add ax,dx
00000627  BA8E00            mov dx,0x8e
0000062A  2B56FE            sub dx,[bp-0x2]
0000062D  8BD8              mov bx,ax
0000062F  368917            mov [ss:bx],dx
00000632  FE06AA37          inc byte [0x37aa]
00000636  803EAA3704        cmp byte [0x37aa],0x4
0000063B  7D03              jnl 0x640
0000063D  E962FF            jmp 0x5a2
00000640  9AA806960A        call 0xa96:0x6a8
00000645  8BC6              mov ax,si
00000647  40                inc ax
00000648  8BF0              mov si,ax
0000064A  3D2700            cmp ax,0x27
0000064D  7E02              jng 0x651
0000064F  33F6              xor si,si
00000651  47                inc di
00000652  83FF28            cmp di,byte +0x28
00000655  7D03              jnl 0x65a
00000657  E932FF            jmp 0x58c
0000065A  9A1B00960A        call 0xa96:0x1b
0000065F  B80900            mov ax,0x9
00000662  50                push ax
00000663  B87900            mov ax,0x79
00000666  50                push ax
00000667  B8C400            mov ax,0xc4
0000066A  50                push ax
0000066B  B81600            mov ax,0x16
0000066E  50                push ax
0000066F  B83700            mov ax,0x37
00000672  50                push ax
00000673  9A8104960A        call 0xa96:0x481
00000678  83C40A            add sp,byte +0xa
0000067B  9AA806960A        call 0xa96:0x6a8
00000680  5F                pop di
00000681  5E                pop si
00000682  8BE5              mov sp,bp
00000684  5D                pop bp
00000685  CB                retf

; ---- 0825:0686 ----
00000686  55                push bp
00000687  8BEC              mov bp,sp
00000689  9A1B00960A        call 0xa96:0x1b
0000068E  B82C00            mov ax,0x2c
00000691  50                push ax
00000692  B83000            mov ax,0x30
00000695  50                push ax
00000696  8B5E06            mov bx,[bp+0x6]
00000699  8A872A30          mov al,[bx+0x302a]
0000069D  B400              mov ah,0x0
0000069F  BA2C00            mov dx,0x2c
000006A2  F7EA              imul dx
000006A4  050E00            add ax,0xe
000006A7  50                push ax
000006A8  B80001            mov ax,0x100
000006AB  50                push ax
000006AC  B81600            mov ax,0x16
000006AF  50                push ax
000006B0  B82000            mov ax,0x20
000006B3  50                push ax
000006B4  B8B000            mov ax,0xb0
000006B7  50                push ax
000006B8  B8F000            mov ax,0xf0
000006BB  50                push ax
000006BC  9A6702960A        call 0xa96:0x267
000006C1  83C410            add sp,byte +0x10
000006C4  9A0D06FD21        call 0x21fd:0x60d
000006C9  5D                pop bp
000006CA  CB                retf

; ---- 0825:06CB ----
000006CB  55                push bp
000006CC  8BEC              mov bp,sp
000006CE  4C                dec sp
000006CF  4C                dec sp
000006D0  C606AA3700        mov byte [0x37aa],0x0
000006D5  EB33              jmp short 0x70a
000006D7  A0AA37            mov al,[0x37aa]
000006DA  98                cbw
000006DB  8BD8              mov bx,ax
000006DD  C687423D03        mov byte [bx+0x3d42],0x3
000006E2  A0AA37            mov al,[0x37aa]
000006E5  98                cbw
000006E6  8BD8              mov bx,ax
000006E8  C6872A3003        mov byte [bx+0x302a],0x3
000006ED  9A13082508        call 0x825:0x813
000006F2  50                push ax
000006F3  A0AA37            mov al,[0x37aa]
000006F6  98                cbw
000006F7  D1E0              shl ax,1
000006F9  D1E0              shl ax,1
000006FB  8BD8              mov bx,ax
000006FD  58                pop ax
000006FE  8987062F          mov [bx+0x2f06],ax
00000702  8997082F          mov [bx+0x2f08],dx
00000706  FE06AA37          inc byte [0x37aa]
0000070A  803EAA3704        cmp byte [0x37aa],0x4
0000070F  7CC6              jl 0x6d7
00000711  C606AA3700        mov byte [0x37aa],0x0
00000716  E99D00            jmp 0x7b6
00000719  A0AA37            mov al,[0x37aa]
0000071C  FEC0              inc al
0000071E  8846FF            mov [bp-0x1],al
00000721  E98500            jmp 0x7a9
00000724  A0AA37            mov al,[0x37aa]
00000727  98                cbw
00000728  D1E0              shl ax,1
0000072A  D1E0              shl ax,1
0000072C  8BD8              mov bx,ax
0000072E  8B87082F          mov ax,[bx+0x2f08]
00000732  8B97062F          mov dx,[bx+0x2f06]
00000736  8A5EFF            mov bl,[bp-0x1]
00000739  B700              mov bh,0x0
0000073B  D1E3              shl bx,1
0000073D  D1E3              shl bx,1
0000073F  3B87082F          cmp ax,[bx+0x2f08]
00000743  7C1E              jl 0x763
00000745  7506              jnz 0x74d
00000747  3B97062F          cmp dx,[bx+0x2f06]
0000074B  7216              jc 0x763
0000074D  A0AA37            mov al,[0x37aa]
00000750  98                cbw
00000751  8BD8              mov bx,ax
00000753  FE8F423D          dec byte [bx+0x3d42]
00000757  A0AA37            mov al,[0x37aa]
0000075A  98                cbw
0000075B  8BD8              mov bx,ax
0000075D  FE8F2A30          dec byte [bx+0x302a]
00000761  EB0B              jmp short 0x76e
00000763  8A46FF            mov al,[bp-0x1]
00000766  B400              mov ah,0x0
00000768  8BD8              mov bx,ax
0000076A  FE8F2A30          dec byte [bx+0x302a]
0000076E  8A46FF            mov al,[bp-0x1]
00000771  B400              mov ah,0x0
00000773  D1E0              shl ax,1
00000775  D1E0              shl ax,1
00000777  8BD8              mov bx,ax
00000779  8B87082F          mov ax,[bx+0x2f08]
0000077D  8B97062F          mov dx,[bx+0x2f06]
00000781  50                push ax
00000782  A0AA37            mov al,[0x37aa]
00000785  98                cbw
00000786  D1E0              shl ax,1
00000788  D1E0              shl ax,1
0000078A  8BD8              mov bx,ax
0000078C  58                pop ax
0000078D  3B87082F          cmp ax,[bx+0x2f08]
00000791  7C13              jl 0x7a6
00000793  7506              jnz 0x79b
00000795  3B97062F          cmp dx,[bx+0x2f06]
00000799  720B              jc 0x7a6
0000079B  8A46FF            mov al,[bp-0x1]
0000079E  B400              mov ah,0x0
000007A0  8BD8              mov bx,ax
000007A2  FE8F423D          dec byte [bx+0x3d42]
000007A6  FE46FF            inc byte [bp-0x1]
000007A9  807EFF03          cmp byte [bp-0x1],0x3
000007AD  7703              ja 0x7b2
000007AF  E972FF            jmp 0x724
000007B2  FE06AA37          inc byte [0x37aa]
000007B6  803EAA3704        cmp byte [0x37aa],0x4
000007BB  7D03              jnl 0x7c0
000007BD  E959FF            jmp 0x719
000007C0  8BE5              mov sp,bp
000007C2  5D                pop bp
000007C3  CB                retf

; ---- 0825:07C4 ----
000007C4  55                push bp
000007C5  8BEC              mov bp,sp
000007C7  56                push si
000007C8  8B369A4B          mov si,[0x4b9a]
000007CC  EB07              jmp short 0x7d5
000007CE  8BC6              mov ax,si
000007D0  05D8FF            add ax,0xffd8
000007D3  8BF0              mov si,ax
000007D5  83FE27            cmp si,byte +0x27
000007D8  7FF4              jg 0x7ce
000007DA  C606AA3700        mov byte [0x37aa],0x0
000007DF  EB28              jmp short 0x809
000007E1  9A13082508        call 0x825:0x813
000007E6  50                push ax
000007E7  A0AA37            mov al,[0x37aa]
000007EA  98                cbw
000007EB  BBA000            mov bx,0xa0
000007EE  52                push dx
000007EF  F7EB              imul bx
000007F1  8BD6              mov dx,si
000007F3  D1E2              shl dx,1
000007F5  D1E2              shl dx,1
000007F7  03C2              add ax,dx
000007F9  8BD8              mov bx,ax
000007FB  58                pop ax
000007FC  5A                pop dx
000007FD  89971C41          mov [bx+0x411c],dx
00000801  89871E41          mov [bx+0x411e],ax
00000805  FE06AA37          inc byte [0x37aa]
00000809  803EAA3704        cmp byte [0x37aa],0x4
0000080E  7CD1              jl 0x7e1
00000810  5E                pop si
00000811  5D                pop bp
00000812  CB                retf

; ---- 0825:0813 ----
00000813  55                push bp
00000814  8BEC              mov bp,sp
00000816  83EC04            sub sp,byte +0x4
00000819  9A6F082508        call 0x825:0x86f
0000081E  50                push ax
0000081F  A0AA37            mov al,[0x37aa]
00000822  98                cbw
00000823  D1E0              shl ax,1
00000825  D1E0              shl ax,1
00000827  8BD8              mov bx,ax
00000829  8B87D43C          mov ax,[bx+0x3cd4]
0000082D  8B9FD23C          mov bx,[bx+0x3cd2]
00000831  59                pop cx
00000832  03D9              add bx,cx
00000834  13C2              adc ax,dx
00000836  53                push bx
00000837  50                push ax
00000838  9A8D082508        call 0x825:0x88d
0000083D  5B                pop bx
0000083E  59                pop cx
0000083F  03C8              add cx,ax
00000841  13DA              adc bx,dx
00000843  51                push cx
00000844  53                push bx
00000845  9AEB082508        call 0x825:0x8eb
0000084A  5B                pop bx
0000084B  59                pop cx
0000084C  03C8              add cx,ax
0000084E  13DA              adc bx,dx
00000850  51                push cx
00000851  53                push bx
00000852  9A6D092508        call 0x825:0x96d
00000857  5B                pop bx
00000858  59                pop cx
00000859  03C8              add cx,ax
0000085B  13DA              adc bx,dx
0000085D  894EFC            mov [bp-0x4],cx
00000860  895EFE            mov [bp-0x2],bx
00000863  8B56FE            mov dx,[bp-0x2]
00000866  8B46FC            mov ax,[bp-0x4]
00000869  EB00              jmp short 0x86b
0000086B  8BE5              mov sp,bp
0000086D  5D                pop bp
0000086E  CB                retf

; ---- 0825:086F ----
0000086F  55                push bp
00000870  8BEC              mov bp,sp
00000872  A0AA37            mov al,[0x37aa]
00000875  98                cbw
00000876  8BD8              mov bx,ax
00000878  8A87A84B          mov al,[bx+0x4ba8]
0000087C  B400              mov ah,0x0
0000087E  F72E4637          imul word [0x3746]
00000882  BB0300            mov bx,0x3
00000885  99                cwd
00000886  F7FB              idiv bx
00000888  99                cwd
00000889  EB00              jmp short 0x88b
0000088B  5D                pop bp
0000088C  CB                retf

; ---- 0825:088D ----
0000088D  55                push bp
0000088E  8BEC              mov bp,sp
00000890  83EC04            sub sp,byte +0x4
00000893  C746FC0000        mov word [bp-0x4],0x0
00000898  C746FE0000        mov word [bp-0x2],0x0
0000089D  C706D0450000      mov word [0x45d0],0x0
000008A3  EB33              jmp short 0x8d8
000008A5  A0AA37            mov al,[0x37aa]
000008A8  98                cbw
000008A9  D1E0              shl ax,1
000008AB  D1E0              shl ax,1
000008AD  D1E0              shl ax,1
000008AF  8B16D045          mov dx,[0x45d0]
000008B3  D1E2              shl dx,1
000008B5  03C2              add ax,dx
000008B7  8BD8              mov bx,ax
000008B9  8B87F43C          mov ax,[bx+0x3cf4]
000008BD  BB0A00            mov bx,0xa
000008C0  99                cwd
000008C1  F7FB              idiv bx
000008C3  8B1ED045          mov bx,[0x45d0]
000008C7  D1E3              shl bx,1
000008C9  F7AF9848          imul word [bx+0x4898]
000008CD  99                cwd
000008CE  0146FC            add [bp-0x4],ax
000008D1  1156FE            adc [bp-0x2],dx
000008D4  FF06D045          inc word [0x45d0]
000008D8  833ED04503        cmp word [0x45d0],byte +0x3
000008DD  7EC6              jng 0x8a5
000008DF  8B56FE            mov dx,[bp-0x2]
000008E2  8B46FC            mov ax,[bp-0x4]
000008E5  EB00              jmp short 0x8e7
000008E7  8BE5              mov sp,bp
000008E9  5D                pop bp
000008EA  CB                retf

; ---- 0825:08EB ----
000008EB  55                push bp
000008EC  8BEC              mov bp,sp
000008EE  803EF24B00        cmp byte [0x4bf2],0x0
000008F3  7506              jnz 0x8fb
000008F5  33D2              xor dx,dx
000008F7  33C0              xor ax,ax
000008F9  EB70              jmp short 0x96b
000008FB  33C0              xor ax,ax
000008FD  BA6400            mov dx,0x64
00000900  50                push ax
00000901  52                push dx
00000902  8B0EF23C          mov cx,[0x3cf2]
00000906  8B1EF03C          mov bx,[0x3cf0]
0000090A  8B163844          mov dx,[0x4438]
0000090E  A13644            mov ax,[0x4436]
00000911  9A4E4F0000        call 0x0:0x4f4e
00000916  52                push dx
00000917  50                push ax
00000918  9A894F0000        call 0x0:0x4f89
0000091D  50                push ax
0000091E  A0AA37            mov al,[0x37aa]
00000921  98                cbw
00000922  8BD8              mov bx,ax
00000924  8A87AF37          mov al,[bx+0x37af]
00000928  98                cbw
00000929  52                push dx
0000092A  99                cwd
0000092B  59                pop cx
0000092C  5B                pop bx
0000092D  9A4E4F0000        call 0x0:0x4f4e
00000932  50                push ax
00000933  52                push dx
00000934  33C0              xor ax,ax
00000936  BA6400            mov dx,0x64
00000939  50                push ax
0000093A  52                push dx
0000093B  A0AA37            mov al,[0x37aa]
0000093E  98                cbw
0000093F  D1E0              shl ax,1
00000941  8BD8              mov bx,ax
00000943  8B870E30          mov ax,[bx+0x300e]
00000947  99                cwd
00000948  50                push ax
00000949  52                push dx
0000094A  8B16F23C          mov dx,[0x3cf2]
0000094E  A1F03C            mov ax,[0x3cf0]
00000951  59                pop cx
00000952  5B                pop bx
00000953  9A4E4F0000        call 0x0:0x4f4e
00000958  52                push dx
00000959  50                push ax
0000095A  9A894F0000        call 0x0:0x4f89
0000095F  8BDA              mov bx,dx
00000961  5A                pop dx
00000962  8BC8              mov cx,ax
00000964  58                pop ax
00000965  03C1              add ax,cx
00000967  13D3              adc dx,bx
00000969  EB00              jmp short 0x96b
0000096B  5D                pop bp
0000096C  CB                retf

; ---- 0825:096D ----
0000096D  55                push bp
0000096E  8BEC              mov bp,sp
00000970  83EC04            sub sp,byte +0x4
00000973  A0AA37            mov al,[0x37aa]
00000976  98                cbw
00000977  D1E0              shl ax,1
00000979  8BD8              mov bx,ax
0000097B  8B87AC4B          mov ax,[bx+0x4bac]
0000097F  50                push ax
00000980  A0AA37            mov al,[0x37aa]
00000983  98                cbw
00000984  D1E0              shl ax,1
00000986  D1E0              shl ax,1
00000988  8BD8              mov bx,ax
0000098A  8A87EA2E          mov al,[bx+0x2eea]
0000098E  98                cbw
0000098F  8BD0              mov dx,ax
00000991  58                pop ax
00000992  03C2              add ax,dx
00000994  F72EA048          imul word [0x48a0]
00000998  99                cwd
00000999  8946FC            mov [bp-0x4],ax
0000099C  8956FE            mov [bp-0x2],dx
0000099F  803EBA4501        cmp byte [0x45ba],0x1
000009A4  7517              jnz 0x9bd
000009A6  A0AA37            mov al,[0x37aa]
000009A9  98                cbw
000009AA  D1E0              shl ax,1
000009AC  8BD8              mov bx,ax
000009AE  8B872045          mov ax,[bx+0x4520]
000009B2  F72EF443          imul word [0x43f4]
000009B6  99                cwd
000009B7  0146FC            add [bp-0x4],ax
000009BA  1156FE            adc [bp-0x2],dx
000009BD  8B56FE            mov dx,[bp-0x2]
000009C0  8B46FC            mov ax,[bp-0x4]
000009C3  EB00              jmp short 0x9c5
000009C5  8BE5              mov sp,bp
000009C7  5D                pop bp
000009C8  CB                retf

; ---- 0825:09C9 ----
000009C9  55                push bp
000009CA  8BEC              mov bp,sp
000009CC  83EC04            sub sp,byte +0x4
000009CF  56                push si
000009D0  57                push di
000009D1  C746FC0000        mov word [bp-0x4],0x0
000009D6  9A9005FD21        call 0x21fd:0x590
000009DB  9A1B00960A        call 0xa96:0x1b
000009E0  33F6              xor si,si
000009E2  E99D00            jmp 0xa82
000009E5  81FEC700          cmp si,0xc7
000009E9  7F09              jg 0x9f4
000009EB  C746FE0000        mov word [bp-0x2],0x0
000009F0  8BFE              mov di,si
000009F2  EB0B              jmp short 0x9ff
000009F4  8BC6              mov ax,si
000009F6  0539FF            add ax,0xff39
000009F9  8946FE            mov [bp-0x2],ax
000009FC  BFC700            mov di,0xc7
000009FF  33C0              xor ax,ax
00000A01  50                push ax
00000A02  57                push di
00000A03  FF76FE            push word [bp-0x2]
00000A06  33C0              xor ax,ax
00000A08  50                push ax
00000A09  56                push si
00000A0A  9A3304960A        call 0xa96:0x433
00000A0F  83C40A            add sp,byte +0xa
00000A12  33C0              xor ax,ax
00000A14  50                push ax
00000A15  B8C700            mov ax,0xc7
00000A18  2BC7              sub ax,di
00000A1A  50                push ax
00000A1B  B83F01            mov ax,0x13f
00000A1E  2B46FE            sub ax,[bp-0x2]
00000A21  50                push ax
00000A22  B8C700            mov ax,0xc7
00000A25  50                push ax
00000A26  B83F01            mov ax,0x13f
00000A29  2BC6              sub ax,si
00000A2B  50                push ax
00000A2C  9A3304960A        call 0xa96:0x433
00000A31  83C40A            add sp,byte +0xa
00000A34  81FEC000          cmp si,0xc0
00000A38  7C1E              jl 0xa58
00000A3A  8BC6              mov ax,si
00000A3C  BB1000            mov bx,0x10
00000A3F  99                cwd
00000A40  F7FB              idiv bx
00000A42  0BD2              or dx,dx
00000A44  7512              jnz 0xa58
00000A46  8B46FC            mov ax,[bp-0x4]
00000A49  40                inc ax
00000A4A  8946FC            mov [bp-0x4],ax
00000A4D  50                push ax
00000A4E  33C0              xor ax,ax
00000A50  50                push ax
00000A51  9AEB03FD21        call 0x21fd:0x3eb
00000A56  59                pop cx
00000A57  59                pop cx
00000A58  833E0E0901        cmp word [0x90e],byte +0x1
00000A5D  750C              jnz 0xa6b
00000A5F  9AA806960A        call 0xa96:0x6a8
00000A64  9A1B00960A        call 0xa96:0x1b
00000A69  EB16              jmp short 0xa81
00000A6B  8BC6              mov ax,si
00000A6D  BB0300            mov bx,0x3
00000A70  99                cwd
00000A71  F7FB              idiv bx
00000A73  0BD2              or dx,dx
00000A75  750A              jnz 0xa81
00000A77  9AA806960A        call 0xa96:0x6a8
00000A7C  9A1B00960A        call 0xa96:0x1b
00000A81  46                inc si
00000A82  81FE3F01          cmp si,0x13f
00000A86  7F03              jg 0xa8b
00000A88  E95AFF            jmp 0x9e5
00000A8B  33C0              xor ax,ax
00000A8D  50                push ax
00000A8E  9ADA04FD21        call 0x21fd:0x4da
00000A93  59                pop cx
00000A94  9A1F05FD21        call 0x21fd:0x51f
00000A99  9ACD04AF21        call 0x21af:0x4cd
00000A9E  5F                pop di
00000A9F  5E                pop si
