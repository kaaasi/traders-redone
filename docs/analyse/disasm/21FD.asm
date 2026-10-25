; segment 21FD  file offset 0x255D0  size 0x980
00000000  F4                hlt
00000001  5E                pop si
00000002  5D                pop bp
00000003  CB                retf

; ---- 21FD:0004 ----
00000004  55                push bp
00000005  8BEC              mov bp,sp
00000007  4C                dec sp
00000008  4C                dec sp
00000009  56                push si
0000000A  57                push di
0000000B  8B7E06            mov di,[bp+0x6]
0000000E  C646FF0F          mov byte [bp-0x1],0xf
00000012  9A1B00960A        call 0xa96:0x1b
00000017  803E814501        cmp byte [0x4581],0x1
0000001C  7505              jnz 0x23
0000001E  B89600            mov ax,0x96
00000021  EB03              jmp short 0x26
00000023  B86400            mov ax,0x64
00000026  8BF0              mov si,ax
00000028  E94301            jmp 0x16e
0000002B  9A1F05FD21        call 0x21fd:0x51f
00000030  FF362E08          push word [0x82e]
00000034  9A1C07CD23        call 0x23cd:0x71c
00000039  59                pop cx
0000003A  9AA806960A        call 0xa96:0x6a8
0000003F  9A1B00960A        call 0xa96:0x1b
00000044  9A4502FD21        call 0x21fd:0x245
00000049  0BF6              or si,si
0000004B  7E01              jng 0x4e
0000004D  4E                dec si
0000004E  803E553D00        cmp byte [0x3d55],0x0
00000053  7403              jz 0x58
00000055  E91F01            jmp 0x177
00000058  83FF04            cmp di,byte +0x4
0000005B  7505              jnz 0x62
0000005D  9A3E11631F        call 0x1f63:0x113e
00000062  9A7A039E0E        call 0xe9e:0x37a
00000067  C606AA3700        mov byte [0x37aa],0x0
0000006C  EB30              jmp short 0x9e
0000006E  A0AA37            mov al,[0x37aa]
00000071  98                cbw
00000072  8BD8              mov bx,ax
00000074  80BF400801        cmp byte [bx+0x840],0x1
00000079  751F              jnz 0x9a
0000007B  A0AA37            mov al,[0x37aa]
0000007E  98                cbw
0000007F  BA1400            mov dx,0x14
00000082  F7EA              imul dx
00000084  3BC6              cmp ax,si
00000086  7504              jnz 0x8c
00000088  B00A              mov al,0xa
0000008A  EB02              jmp short 0x8e
0000008C  B0FF              mov al,0xff
0000008E  50                push ax
0000008F  A0AA37            mov al,[0x37aa]
00000092  98                cbw
00000093  8BD8              mov bx,ax
00000095  58                pop ax
00000096  88870833          mov [bx+0x3308],al
0000009A  FE06AA37          inc byte [0x37aa]
0000009E  803EAA3704        cmp byte [0x37aa],0x4
000000A3  7CC9              jl 0x6e
000000A5  C606AA3700        mov byte [0x37aa],0x0
000000AA  E9A800            jmp 0x155
000000AD  A0AA37            mov al,[0x37aa]
000000B0  98                cbw
000000B1  8A56FF            mov dl,[bp-0x1]
000000B4  8BD8              mov bx,ax
000000B6  8497C608          test [bx+0x8c6],dl
000000BA  7503              jnz 0xbf
000000BC  E99200            jmp 0x151
000000BF  A0AA37            mov al,[0x37aa]
000000C2  98                cbw
000000C3  8BD8              mov bx,ax
000000C5  80BF08330A        cmp byte [bx+0x3308],0xa
000000CA  7403              jz 0xcf
000000CC  E98200            jmp 0x151
000000CF  A0AA37            mov al,[0x37aa]
000000D2  98                cbw
000000D3  B2FF              mov dl,0xff
000000D5  8BD8              mov bx,ax
000000D7  2A97C608          sub dl,[bx+0x8c6]
000000DB  8A46FF            mov al,[bp-0x1]
000000DE  22C2              and al,dl
000000E0  8846FF            mov [bp-0x1],al
000000E3  A0AA37            mov al,[0x37aa]
000000E6  98                cbw
000000E7  8BD8              mov bx,ax
000000E9  83FB03            cmp bx,byte +0x3
000000EC  7749              ja 0x137
000000EE  D1E3              shl bx,1
000000F0  2EFFA77D01        jmp [cs:bx+0x17d]
000000F5  B80300            mov ax,0x3
000000F8  50                push ax
000000F9  33C0              xor ax,ax
000000FB  50                push ax
000000FC  9A5400A721        call 0x21a7:0x54
00000101  59                pop cx
00000102  59                pop cx
00000103  EB32              jmp short 0x137
00000105  B80400            mov ax,0x4
00000108  50                push ax
00000109  B80100            mov ax,0x1
0000010C  50                push ax
0000010D  9A5400A721        call 0x21a7:0x54
00000112  59                pop cx
00000113  59                pop cx
00000114  EB21              jmp short 0x137
00000116  B80500            mov ax,0x5
00000119  50                push ax
0000011A  B80200            mov ax,0x2
0000011D  50                push ax
0000011E  9A5400A721        call 0x21a7:0x54
00000123  59                pop cx
00000124  59                pop cx
00000125  EB10              jmp short 0x137
00000127  B80600            mov ax,0x6
0000012A  50                push ax
0000012B  33C0              xor ax,ax
0000012D  50                push ax
0000012E  9A5400A721        call 0x21a7:0x54
00000133  59                pop cx
00000134  59                pop cx
00000135  EB00              jmp short 0x137
00000137  83FF01            cmp di,byte +0x1
0000013A  7505              jnz 0x141
0000013C  9ACC1F0906        call 0x609:0x1fcc
00000141  83FF02            cmp di,byte +0x2
00000144  750B              jnz 0x151
00000146  A0AA37            mov al,[0x37aa]
00000149  98                cbw
0000014A  50                push ax
0000014B  9A86062508        call 0x825:0x686
00000150  59                pop cx
00000151  FE06AA37          inc byte [0x37aa]
00000155  803EAA3704        cmp byte [0x37aa],0x4
0000015A  7D03              jnl 0x15f
0000015C  E94EFF            jmp 0xad
0000015F  9AA806960A        call 0xa96:0x6a8
00000164  FF362E08          push word [0x82e]
00000168  9AE906CD23        call 0x23cd:0x6e9
0000016D  59                pop cx
0000016E  807EFF00          cmp byte [bp-0x1],0x0
00000172  7403              jz 0x177
00000174  E9B4FE            jmp 0x2b
00000177  5F                pop di
00000178  5E                pop si
00000179  8BE5              mov sp,bp
0000017B  5D                pop bp
0000017C  CB                retf
0000017D  F5                cmc
0000017E  0005              add [di],al
00000180  01160127          add [0x2701],dx
00000184  01558B            add [di-0x75],dx
00000187  EC                in al,dx
00000188  83EC04            sub sp,byte +0x4
0000018B  C746FC0000        mov word [bp-0x4],0x0
00000190  C746FE0000        mov word [bp-0x2],0x0
00000195  EB0D              jmp short 0x1a4
00000197  9AD300730D        call 0xd73:0xd3
0000019C  8346FC01          add word [bp-0x4],byte +0x1
000001A0  8356FE00          adc word [bp-0x2],byte +0x0
000001A4  8B46FE            mov ax,[bp-0x2]
000001A7  8B56FC            mov dx,[bp-0x4]
000001AA  3B4608            cmp ax,[bp+0x8]
000001AD  7CE8              jl 0x197
000001AF  7505              jnz 0x1b6
000001B1  3B5606            cmp dx,[bp+0x6]
000001B4  72E1              jc 0x197
000001B6  8BE5              mov sp,bp
000001B8  5D                pop bp
000001B9  CB                retf

; ---- 21FD:01BA ----
000001BA  55                push bp
000001BB  8BEC              mov bp,sp
000001BD  837E0600          cmp word [bp+0x6],byte +0x0
000001C1  7518              jnz 0x1db
000001C3  33C0              xor ax,ax
000001C5  33D2              xor dx,dx
000001C7  50                push ax
000001C8  52                push dx
000001C9  9A05340000        call 0x0:0x3405
000001CE  59                pop cx
000001CF  59                pop cx
000001D0  A3D210            mov [0x10d2],ax
000001D3  8916D410          mov [0x10d4],dx
000001D7  33C0              xor ax,ax
000001D9  EB42              jmp short 0x21d
000001DB  B940BB            mov cx,0xbb40
000001DE  BB2DE6            mov bx,0xe62d
000001E1  8B16D410          mov dx,[0x10d4]
000001E5  A1D210            mov ax,[0x10d2]
000001E8  9A4E4F0000        call 0x0:0x4f4e
000001ED  A3D210            mov [0x10d2],ax
000001F0  8916D410          mov [0x10d4],dx
000001F4  8306D21001        add word [0x10d2],byte +0x1
000001F9  8316D41000        adc word [0x10d4],byte +0x0
000001FE  8B4606            mov ax,[bp+0x6]
00000201  40                inc ax
00000202  33D2              xor dx,dx
00000204  52                push dx
00000205  50                push ax
00000206  8B16D410          mov dx,[0x10d4]
0000020A  A1D210            mov ax,[0x10d2]
0000020D  B108              mov cl,0x8
0000020F  9A58500000        call 0x0:0x5058
00000214  52                push dx
00000215  50                push ax
00000216  9AA04F0000        call 0x0:0x4fa0
0000021B  EB00              jmp short 0x21d
0000021D  5D                pop bp
0000021E  CB                retf

; ---- 21FD:021F ----
0000021F  55                push bp
00000220  8BEC              mov bp,sp
00000222  B80200            mov ax,0x2
00000225  50                push ax
00000226  0E                push cs
00000227  E890FF            call 0x1ba
0000022A  59                pop cx
0000022B  A2E640            mov [0x40e6],al
0000022E  B80400            mov ax,0x4
00000231  50                push ax
00000232  0E                push cs
00000233  E884FF            call 0x1ba
00000236  59                pop cx
00000237  0BC0              or ax,ax
00000239  7508              jnz 0x243
0000023B  A0E640            mov al,[0x40e6]
0000023E  0403              add al,0x3
00000240  A2E640            mov [0x40e6],al
00000243  5D                pop bp
00000244  CB                retf

; ---- 21FD:0245 ----
00000245  55                push bp
00000246  8BEC              mov bp,sp
00000248  803E603D00        cmp byte [0x3d60],0x0
0000024D  741B              jz 0x26a
0000024F  EB00              jmp short 0x251
00000251  803E603D00        cmp byte [0x3d60],0x0
00000256  75F9              jnz 0x251
00000258  EB00              jmp short 0x25a
0000025A  803E603D00        cmp byte [0x3d60],0x0
0000025F  74F9              jz 0x25a
00000261  EB00              jmp short 0x263
00000263  803E603D00        cmp byte [0x3d60],0x0
00000268  75F9              jnz 0x263
0000026A  5D                pop bp
0000026B  CB                retf

; ---- 21FD:026C ----
0000026C  55                push bp
0000026D  8BEC              mov bp,sp
0000026F  83EC08            sub sp,byte +0x8
00000272  56                push si
00000273  C746FCCB3D        mov word [bp-0x4],0x3dcb
00000278  8C5EFE            mov [bp-0x2],ds
0000027B  C746F81750        mov word [bp-0x8],0x5017
00000280  8C5EFA            mov [bp-0x6],ds
00000283  33F6              xor si,si
00000285  EB37              jmp short 0x2be
00000287  C45EF8            les bx,[bp-0x8]
0000028A  268A07            mov al,[es:bx]
0000028D  C45EFC            les bx,[bp-0x4]
00000290  268807            mov [es:bx],al
00000293  FF46F8            inc word [bp-0x8]
00000296  FF46FC            inc word [bp-0x4]
00000299  C45EF8            les bx,[bp-0x8]
0000029C  268A07            mov al,[es:bx]
0000029F  C45EFC            les bx,[bp-0x4]
000002A2  268807            mov [es:bx],al
000002A5  FF46F8            inc word [bp-0x8]
000002A8  FF46FC            inc word [bp-0x4]
000002AB  C45EF8            les bx,[bp-0x8]
000002AE  268A07            mov al,[es:bx]
000002B1  C45EFC            les bx,[bp-0x4]
000002B4  268807            mov [es:bx],al
000002B7  FF46F8            inc word [bp-0x8]
000002BA  FF46FC            inc word [bp-0x4]
000002BD  46                inc si
000002BE  81FE0001          cmp si,0x100
000002C2  7CC3              jl 0x287
000002C4  5E                pop si
000002C5  8BE5              mov sp,bp
000002C7  5D                pop bp
000002C8  CB                retf

; ---- 21FD:02C9 ----
000002C9  55                push bp
000002CA  8BEC              mov bp,sp
000002CC  56                push si
000002CD  8B7606            mov si,[bp+0x6]
000002D0  8B4608            mov ax,[bp+0x8]
000002D3  B108              mov cl,0x8
000002D5  D3E8              shr ax,cl
000002D7  D0E0              shl al,1
000002D9  D0E0              shl al,1
000002DB  D0E0              shl al,1
000002DD  50                push ax
000002DE  8BC6              mov ax,si
000002E0  BA0300            mov dx,0x3
000002E3  F7EA              imul dx
000002E5  8BD8              mov bx,ax
000002E7  58                pop ax
000002E8  8887CB3D          mov [bx+0x3dcb],al
000002EC  8B4608            mov ax,[bp+0x8]
000002EF  25F000            and ax,0xf0
000002F2  BB1000            mov bx,0x10
000002F5  99                cwd
000002F6  F7FB              idiv bx
000002F8  D0E0              shl al,1
000002FA  D0E0              shl al,1
000002FC  D0E0              shl al,1
000002FE  50                push ax
000002FF  8BC6              mov ax,si
00000301  BA0300            mov dx,0x3
00000304  F7EA              imul dx
00000306  8BD8              mov bx,ax
00000308  58                pop ax
00000309  8887CC3D          mov [bx+0x3dcc],al
0000030D  8BC6              mov ax,si
0000030F  BA0300            mov dx,0x3
00000312  F7EA              imul dx
00000314  8A5608            mov dl,[bp+0x8]
00000317  80E20F            and dl,0xf
0000031A  D0E2              shl dl,1
0000031C  D0E2              shl dl,1
0000031E  D0E2              shl dl,1
00000320  8BD8              mov bx,ax
00000322  8897CD3D          mov [bx+0x3dcd],dl
00000326  5E                pop si
00000327  5D                pop bp
00000328  CB                retf

; ---- 21FD:0329 ----
00000329  55                push bp
0000032A  8BEC              mov bp,sp
0000032C  56                push si
0000032D  BE0700            mov si,0x7
00000330  EB1A              jmp short 0x34c
00000332  56                push si
00000333  FF36D810          push word [0x10d8]
00000337  9AEB03FD21        call 0x21fd:0x3eb
0000033C  59                pop cx
0000033D  59                pop cx
0000033E  33C0              xor ax,ax
00000340  BA0300            mov dx,0x3
00000343  50                push ax
00000344  52                push dx
00000345  0E                push cs
00000346  E83CFE            call 0x185
00000349  59                pop cx
0000034A  59                pop cx
0000034B  4E                dec si
0000034C  0BF6              or si,si
0000034E  7DE2              jnl 0x332
00000350  5E                pop si
00000351  5D                pop bp
00000352  CB                retf

; ---- 21FD:0353 ----
00000353  55                push bp
00000354  8BEC              mov bp,sp
00000356  56                push si
00000357  BE0700            mov si,0x7
0000035A  EB1A              jmp short 0x376
0000035C  56                push si
0000035D  FF36D810          push word [0x10d8]
00000361  9AEB03FD21        call 0x21fd:0x3eb
00000366  59                pop cx
00000367  59                pop cx
00000368  33C0              xor ax,ax
0000036A  BA0100            mov dx,0x1
0000036D  50                push ax
0000036E  52                push dx
0000036F  0E                push cs
00000370  E812FE            call 0x185
00000373  59                pop cx
00000374  59                pop cx
00000375  4E                dec si
00000376  0BF6              or si,si
00000378  7DE2              jnl 0x35c
0000037A  5E                pop si
0000037B  5D                pop bp
0000037C  CB                retf

; ---- 21FD:037D ----
0000037D  55                push bp
0000037E  8BEC              mov bp,sp
00000380  56                push si
00000381  33F6              xor si,si
00000383  EB19              jmp short 0x39e
00000385  56                push si
00000386  FF7606            push word [bp+0x6]
00000389  9AEB03FD21        call 0x21fd:0x3eb
0000038E  59                pop cx
0000038F  59                pop cx
00000390  33C0              xor ax,ax
00000392  BA0400            mov dx,0x4
00000395  50                push ax
00000396  52                push dx
00000397  0E                push cs
00000398  E8EAFD            call 0x185
0000039B  59                pop cx
0000039C  59                pop cx
0000039D  46                inc si
0000039E  83FE07            cmp si,byte +0x7
000003A1  7EE2              jng 0x385
000003A3  FF7606            push word [bp+0x6]
000003A6  9ADA04FD21        call 0x21fd:0x4da
000003AB  59                pop cx
000003AC  9ACD04AF21        call 0x21af:0x4cd
000003B1  5E                pop si
000003B2  5D                pop bp
000003B3  CB                retf

; ---- 21FD:03B4 ----
000003B4  55                push bp
000003B5  8BEC              mov bp,sp
000003B7  56                push si
000003B8  33F6              xor si,si
000003BA  EB19              jmp short 0x3d5
000003BC  56                push si
000003BD  FF7606            push word [bp+0x6]
000003C0  9AEB03FD21        call 0x21fd:0x3eb
000003C5  59                pop cx
000003C6  59                pop cx
000003C7  33C0              xor ax,ax
000003C9  BA0200            mov dx,0x2
000003CC  50                push ax
000003CD  52                push dx
000003CE  0E                push cs
000003CF  E8B3FD            call 0x185
000003D2  59                pop cx
000003D3  59                pop cx
000003D4  46                inc si
000003D5  83FE07            cmp si,byte +0x7
000003D8  7EE2              jng 0x3bc
000003DA  FF7606            push word [bp+0x6]
000003DD  9ADA04FD21        call 0x21fd:0x4da
000003E2  59                pop cx
000003E3  9ACD04AF21        call 0x21af:0x4cd
000003E8  5E                pop si
000003E9  5D                pop bp
000003EA  CB                retf

; ---- 21FD:03EB ----
000003EB  55                push bp
000003EC  8BEC              mov bp,sp
000003EE  83EC08            sub sp,byte +0x8
000003F1  56                push si
000003F2  57                push di
000003F3  8B4606            mov ax,[bp+0x6]
000003F6  B108              mov cl,0x8
000003F8  D3E8              shr ax,cl
000003FA  D0E0              shl al,1
000003FC  D0E0              shl al,1
000003FE  D0E0              shl al,1
00000400  8846FC            mov [bp-0x4],al
00000403  8B4606            mov ax,[bp+0x6]
00000406  25F000            and ax,0xf0
00000409  BB1000            mov bx,0x10
0000040C  99                cwd
0000040D  F7FB              idiv bx
0000040F  D0E0              shl al,1
00000411  D0E0              shl al,1
00000413  D0E0              shl al,1
00000415  8846FD            mov [bp-0x3],al
00000418  8A4606            mov al,[bp+0x6]
0000041B  240F              and al,0xf
0000041D  D0E0              shl al,1
0000041F  D0E0              shl al,1
00000421  D0E0              shl al,1
00000423  8846FE            mov [bp-0x2],al
00000426  33FF              xor di,di
00000428  E98D00            jmp 0x4b8
0000042B  8BC7              mov ax,di
0000042D  BA0300            mov dx,0x3
00000430  F7EA              imul dx
00000432  8BD8              mov bx,ax
00000434  8A87CB3D          mov al,[bx+0x3dcb]
00000438  8846F8            mov [bp-0x8],al
0000043B  8BC7              mov ax,di
0000043D  BA0300            mov dx,0x3
00000440  F7EA              imul dx
00000442  8BD8              mov bx,ax
00000444  8A87CC3D          mov al,[bx+0x3dcc]
00000448  8846F9            mov [bp-0x7],al
0000044B  8BC7              mov ax,di
0000044D  BA0300            mov dx,0x3
00000450  F7EA              imul dx
00000452  8BD8              mov bx,ax
00000454  8A87CD3D          mov al,[bx+0x3dcd]
00000458  8846FA            mov [bp-0x6],al
0000045B  33F6              xor si,si
0000045D  EB53              jmp short 0x4b2
0000045F  8A42F8            mov al,[bp+si-0x8]
00000462  3A42FC            cmp al,[bp+si-0x4]
00000465  7E1E              jng 0x485
00000467  8A42F8            mov al,[bp+si-0x8]
0000046A  8A5608            mov dl,[bp+0x8]
0000046D  D0E2              shl dl,1
0000046F  D0E2              shl dl,1
00000471  D0E2              shl dl,1
00000473  2AC2              sub al,dl
00000475  8842F8            mov [bp+si-0x8],al
00000478  3A42FC            cmp al,[bp+si-0x4]
0000047B  7D06              jnl 0x483
0000047D  8A42FC            mov al,[bp+si-0x4]
00000480  8842F8            mov [bp+si-0x8],al
00000483  EB1C              jmp short 0x4a1
00000485  8A42F8            mov al,[bp+si-0x8]
00000488  8A5608            mov dl,[bp+0x8]
0000048B  D0E2              shl dl,1
0000048D  D0E2              shl dl,1
0000048F  D0E2              shl dl,1
00000491  02C2              add al,dl
00000493  8842F8            mov [bp+si-0x8],al
00000496  3A42FC            cmp al,[bp+si-0x4]
00000499  7E06              jng 0x4a1
0000049B  8A42FC            mov al,[bp+si-0x4]
0000049E  8842F8            mov [bp+si-0x8],al
000004A1  8BC7              mov ax,di
000004A3  BA0300            mov dx,0x3
000004A6  F7EA              imul dx
000004A8  8A52F8            mov dl,[bp+si-0x8]
000004AB  8BD8              mov bx,ax
000004AD  8890E854          mov [bx+si+0x54e8],dl
000004B1  46                inc si
000004B2  83FE02            cmp si,byte +0x2
000004B5  76A8              jna 0x45f
000004B7  47                inc di
000004B8  81FFFF00          cmp di,0xff
000004BC  7703              ja 0x4c1
000004BE  E96AFF            jmp 0x42b
000004C1  833E0E0901        cmp word [0x90e],byte +0x1
000004C6  750C              jnz 0x4d4
000004C8  1E                push ds
000004C9  B8E854            mov ax,0x54e8
000004CC  50                push ax
000004CD  9AC5009522        call 0x2295:0xc5
000004D2  59                pop cx
000004D3  59                pop cx
000004D4  5F                pop di
000004D5  5E                pop si
000004D6  8BE5              mov sp,bp
000004D8  5D                pop bp
000004D9  CB                retf

; ---- 21FD:04DA ----
000004DA  55                push bp
000004DB  8BEC              mov bp,sp
000004DD  8B4606            mov ax,[bp+0x6]
000004E0  A3D810            mov [0x10d8],ax
000004E3  5D                pop bp
000004E4  CB                retf

; ---- 21FD:04E5 ----
000004E5  55                push bp
000004E6  8BEC              mov bp,sp
000004E8  8B4606            mov ax,[bp+0x6]
000004EB  A38037            mov [0x3780],ax
000004EE  8B4608            mov ax,[bp+0x8]
000004F1  A3162F            mov [0x2f16],ax
000004F4  A3182F            mov [0x2f18],ax
000004F7  5D                pop bp
000004F8  CB                retf

; ---- 21FD:04F9 ----
000004F9  55                push bp
000004FA  8BEC              mov bp,sp
000004FC  0E                push cs
000004FD  E845FD            call 0x245
00000500  A1182F            mov ax,[0x2f18]
00000503  48                dec ax
00000504  A3182F            mov [0x2f18],ax
00000507  0BC0              or ax,ax
00000509  750E              jnz 0x519
0000050B  FF0E8037          dec word [0x3780]
0000050F  A1162F            mov ax,[0x2f16]
00000512  A3182F            mov [0x2f18],ax
00000515  B001              mov al,0x1
00000517  EB04              jmp short 0x51d
00000519  B000              mov al,0x0
0000051B  EB00              jmp short 0x51d
0000051D  5D                pop bp
0000051E  CB                retf

; ---- 21FD:051F ----
0000051F  55                push bp
00000520  8BEC              mov bp,sp
00000522  81362E080100      xor word [0x82e],0x1
00000528  A12E08            mov ax,[0x82e]
0000052B  F7D8              neg ax
0000052D  1BC0              sbb ax,ax
0000052F  40                inc ax
00000530  A33008            mov [0x830],ax
00000533  8B1E2E08          mov bx,[0x82e]
00000537  D1E3              shl bx,1
00000539  D1E3              shl bx,1
0000053B  8B87E040          mov ax,[bx+0x40e0]
0000053F  8B97DE40          mov dx,[bx+0x40de]
00000543  89161645          mov [0x4516],dx
00000547  A31845            mov [0x4518],ax
0000054A  B8FFFF            mov ax,0xffff
0000054D  50                push ax
0000054E  8B1E3008          mov bx,[0x830]
00000552  D1E3              shl bx,1
00000554  D1E3              shl bx,1
00000556  FFB7E040          push word [bx+0x40e0]
0000055A  FFB7DE40          push word [bx+0x40de]
0000055E  FF361845          push word [0x4518]
00000562  FF361645          push word [0x4516]
00000566  9A3802670B        call 0xb67:0x238
0000056B  83C40A            add sp,byte +0xa
0000056E  803E953D00        cmp byte [0x3d95],0x0
00000573  7505              jnz 0x57a
00000575  9A8F02670B        call 0xb67:0x28f
0000057A  803E913D00        cmp byte [0x3d91],0x0
0000057F  740D              jz 0x58e
00000581  33C0              xor ax,ax
00000583  BA0400            mov dx,0x4
00000586  50                push ax
00000587  52                push dx
00000588  0E                push cs
00000589  E8F9FB            call 0x185
0000058C  59                pop cx
0000058D  59                pop cx
0000058E  5D                pop bp
0000058F  CB                retf

; ---- 21FD:0590 ----
00000590  55                push bp
00000591  8BEC              mov bp,sp
00000593  A13008            mov ax,[0x830]
00000596  A32E08            mov [0x82e],ax
00000599  8B1E2E08          mov bx,[0x82e]
0000059D  D1E3              shl bx,1
0000059F  D1E3              shl bx,1
000005A1  8B87E040          mov ax,[bx+0x40e0]
000005A5  8B97DE40          mov dx,[bx+0x40de]
000005A9  89161645          mov [0x4516],dx
000005AD  A31845            mov [0x4518],ax
000005B0  B8FFFF            mov ax,0xffff
000005B3  50                push ax
000005B4  8B1E3008          mov bx,[0x830]
000005B8  D1E3              shl bx,1
000005BA  D1E3              shl bx,1
000005BC  FFB7E040          push word [bx+0x40e0]
000005C0  FFB7DE40          push word [bx+0x40de]
000005C4  FF361845          push word [0x4518]
000005C8  FF361645          push word [0x4516]
000005CC  9A3802670B        call 0xb67:0x238
000005D1  83C40A            add sp,byte +0xa
000005D4  9A8F02670B        call 0xb67:0x28f
000005D9  5D                pop bp
000005DA  CB                retf

; ---- 21FD:05DB ----
000005DB  55                push bp
000005DC  8BEC              mov bp,sp
000005DE  56                push si
000005DF  8B7606            mov si,[bp+0x6]
000005E2  56                push si
000005E3  9AB605CD23        call 0x23cd:0x5b6
000005E8  59                pop cx
000005E9  8A84DA10          mov al,[si+0x10da]
000005ED  B400              mov ah,0x0
000005EF  A3A83C            mov [0x3ca8],ax
000005F2  8A84DD10          mov al,[si+0x10dd]
000005F6  B400              mov ah,0x0
000005F8  A3A434            mov [0x34a4],ax
000005FB  FF36A434          push word [0x34a4]
000005FF  FF36A83C          push word [0x3ca8]
00000603  9A0A00960A        call 0xa96:0xa
00000608  59                pop cx
00000609  59                pop cx
0000060A  5E                pop si
0000060B  5D                pop bp
0000060C  CB                retf

; ---- 21FD:060D ----
0000060D  55                push bp
0000060E  8BEC              mov bp,sp
00000610  9AA806960A        call 0xa96:0x6a8
00000615  0E                push cs
00000616  E806FF            call 0x51f
00000619  9AA806960A        call 0xa96:0x6a8
0000061E  5D                pop bp
0000061F  CB                retf

; ---- 21FD:0620 ----
00000620  55                push bp
00000621  8BEC              mov bp,sp
00000623  FF362E08          push word [0x82e]
00000627  9A1C07CD23        call 0x23cd:0x71c
0000062C  59                pop cx
0000062D  9A1B00960A        call 0xa96:0x1b
00000632  5D                pop bp
00000633  CB                retf

; ---- 21FD:0634 ----
00000634  55                push bp
00000635  8BEC              mov bp,sp
00000637  9AA806960A        call 0xa96:0x6a8
0000063C  FF362E08          push word [0x82e]
00000640  9AE906CD23        call 0x23cd:0x6e9
00000645  59                pop cx
00000646  0E                push cs
00000647  E8D5FE            call 0x51f
0000064A  FF362E08          push word [0x82e]
0000064E  9A1C07CD23        call 0x23cd:0x71c
00000653  59                pop cx
00000654  9AA806960A        call 0xa96:0x6a8
00000659  FF362E08          push word [0x82e]
0000065D  9AE906CD23        call 0x23cd:0x6e9
00000662  59                pop cx
00000663  5D                pop bp
00000664  CB                retf

; ---- 21FD:0665 ----
00000665  55                push bp
00000666  8BEC              mov bp,sp
00000668  0E                push cs
00000669  E8B3FE            call 0x51f
0000066C  FF362E08          push word [0x82e]
00000670  9A1C07CD23        call 0x23cd:0x71c
00000675  59                pop cx
00000676  9AA806960A        call 0xa96:0x6a8
0000067B  FF362E08          push word [0x82e]
0000067F  9AE906CD23        call 0x23cd:0x6e9
00000684  59                pop cx
00000685  5D                pop bp
00000686  CB                retf

; ---- 21FD:0687 ----
00000687  55                push bp
00000688  8BEC              mov bp,sp
0000068A  83EC0A            sub sp,byte +0xa
0000068D  56                push si
0000068E  57                push di
0000068F  16                push ss
00000690  8D46F6            lea ax,[bp-0xa]
00000693  50                push ax
00000694  1E                push ds
00000695  B8FB10            mov ax,0x10fb
00000698  50                push ax
00000699  B90400            mov cx,0x4
0000069C  9A89510000        call 0x0:0x5189
000006A1  33C0              xor ax,ax
000006A3  50                push ax
000006A4  0E                push cs
000006A5  E812FB            call 0x1ba
000006A8  59                pop cx
000006A9  FF36DA08          push word [0x8da]
000006AD  9A02006C18        call 0x186c:0x2
000006B2  59                pop cx
000006B3  9A1D08FD21        call 0x21fd:0x81d
000006B8  B8FF03            mov ax,0x3ff
000006BB  50                push ax
000006BC  0E                push cs
000006BD  E8FAFA            call 0x1ba
000006C0  59                pop cx
000006C1  8946FA            mov [bp-0x6],ax
000006C4  8B46FA            mov ax,[bp-0x6]
000006C7  25FC03            and ax,0x3fc
000006CA  8946FA            mov [bp-0x6],ax
000006CD  8B46FA            mov ax,[bp-0x6]
000006D0  BB2000            mov bx,0x20
000006D3  99                cwd
000006D4  F7FB              idiv bx
000006D6  8956FE            mov [bp-0x2],dx
000006D9  8B46FA            mov ax,[bp-0x6]
000006DC  BB2000            mov bx,0x20
000006DF  99                cwd
000006E0  F7FB              idiv bx
000006E2  8946FC            mov [bp-0x4],ax
000006E5  33FF              xor di,di
000006E7  EB1A              jmp short 0x703
000006E9  8B46FC            mov ax,[bp-0x4]
000006EC  99                cwd
000006ED  52                push dx
000006EE  50                push ax
000006EF  8B46FE            mov ax,[bp-0x2]
000006F2  03C7              add ax,di
000006F4  99                cwd
000006F5  52                push dx
000006F6  50                push ax
000006F7  9AD308FD21        call 0x21fd:0x8d3
000006FC  83C408            add sp,byte +0x8
000006FF  8843F6            mov [bp+di-0xa],al
00000702  47                inc di
00000703  83FF04            cmp di,byte +0x4
00000706  7CE1              jl 0x6e9
00000708  8B46FE            mov ax,[bp-0x2]
0000070B  BB0400            mov bx,0x4
0000070E  99                cwd
0000070F  F7FB              idiv bx
00000711  0441              add al,0x41
00000713  C41EE810          les bx,[0x10e8]
00000717  26884706          mov [es:bx+0x6],al
0000071B  837EFC0A          cmp word [bp-0x4],byte +0xa
0000071F  7D18              jnl 0x739
00000721  C41EE810          les bx,[0x10e8]
00000725  8A46FC            mov al,[bp-0x4]
00000728  0430              add al,0x30
0000072A  26884708          mov [es:bx+0x8],al
0000072E  C41EE810          les bx,[0x10e8]
00000732  26C6470920        mov byte [es:bx+0x9],0x20
00000737  EB27              jmp short 0x760
00000739  8B46FC            mov ax,[bp-0x4]
0000073C  BB0A00            mov bx,0xa
0000073F  99                cwd
00000740  F7FB              idiv bx
00000742  0430              add al,0x30
00000744  C41EE810          les bx,[0x10e8]
00000748  26884708          mov [es:bx+0x8],al
0000074C  8B46FC            mov ax,[bp-0x4]
0000074F  BB0A00            mov bx,0xa
00000752  99                cwd
00000753  F7FB              idiv bx
00000755  80C230            add dl,0x30
00000758  C41EE810          les bx,[0x10e8]
0000075C  26885709          mov [es:bx+0x9],dl
00000760  B8FDFF            mov ax,0xfffd
00000763  50                push ax
00000764  1E                push ds
00000765  B8E010            mov ax,0x10e0
00000768  50                push ax
00000769  B81E00            mov ax,0x1e
0000076C  50                push ax
0000076D  B85000            mov ax,0x50
00000770  50                push ax
00000771  9AAF162D19        call 0x192d:0x16af
00000776  83C40A            add sp,byte +0xa
00000779  FF362E08          push word [0x82e]
0000077D  9A3F0C960A        call 0xa96:0xc3f
00000782  59                pop cx
00000783  33C0              xor ax,ax
00000785  33D2              xor dx,dx
00000787  50                push ax
00000788  52                push dx
00000789  0E                push cs
0000078A  E8DFFA            call 0x26c
0000078D  59                pop cx
0000078E  59                pop cx
0000078F  B86000            mov ax,0x60
00000792  50                push ax
00000793  B80900            mov ax,0x9
00000796  50                push ax
00000797  0E                push cs
00000798  E82EFB            call 0x2c9
0000079B  59                pop cx
0000079C  59                pop cx
0000079D  0E                push cs
0000079E  E888FB            call 0x329
000007A1  33FF              xor di,di
000007A3  EB6D              jmp short 0x812
000007A5  C606581100        mov byte [0x1158],0x0
000007AA  EB00              jmp short 0x7ac
000007AC  A05811            mov al,[0x1158]
000007AF  98                cbw
000007B0  0BC0              or ax,ax
000007B2  74F8              jz 0x7ac
000007B4  A05811            mov al,[0x1158]
000007B7  98                cbw
000007B8  8BF0              mov si,ax
000007BA  EB00              jmp short 0x7bc
000007BC  80BC473D00        cmp byte [si+0x3d47],0x0
000007C1  75F9              jnz 0x7bc
000007C3  83FE47            cmp si,byte +0x47
000007C6  7C0E              jl 0x7d6
000007C8  83FE51            cmp si,byte +0x51
000007CB  7F09              jg 0x7d6
000007CD  8A84A910          mov al,[si+0x10a9]
000007D1  98                cbw
000007D2  8BF0              mov si,ax
000007D4  EB01              jmp short 0x7d7
000007D6  4E                dec si
000007D7  83FE01            cmp si,byte +0x1
000007DA  7CC9              jl 0x7a5
000007DC  83FE09            cmp si,byte +0x9
000007DF  7FC4              jg 0x7a5
000007E1  8BC7              mov ax,di
000007E3  99                cwd
000007E4  52                push dx
000007E5  50                push ax
000007E6  8BC6              mov ax,si
000007E8  48                dec ax
000007E9  99                cwd
000007EA  52                push dx
000007EB  50                push ax
000007EC  9A6308FD21        call 0x21fd:0x863
000007F1  83C408            add sp,byte +0x8
000007F4  8A43F6            mov al,[bp+di-0xa]
000007F7  98                cbw
000007F8  3BC6              cmp ax,si
000007FA  EB15              jmp short 0x811
000007FC  B84F00            mov ax,0x4f
000007FF  50                push ax
00000800  33C0              xor ax,ax
00000802  50                push ax
00000803  9A5400A721        call 0x21a7:0x54
00000808  59                pop cx
00000809  59                pop cx
0000080A  9AAA08FD21        call 0x21fd:0x8aa
0000080F  EBFE              jmp short 0x80f
00000811  47                inc di
00000812  83FF04            cmp di,byte +0x4
00000815  7C8E              jl 0x7a5
00000817  5F                pop di
00000818  5E                pop si
00000819  8BE5              mov sp,bp
0000081B  5D                pop bp
0000081C  CB                retf

; ---- 21FD:081D ----
0000081D  55                push bp
0000081E  8BEC              mov bp,sp
00000820  56                push si
00000821  57                push di
00000822  33F6              xor si,si
00000824  EB34              jmp short 0x85a
00000826  33FF              xor di,di
00000828  8BDE              mov bx,si
0000082A  D1E3              shl bx,1
0000082C  D1E3              shl bx,1
0000082E  C49FE010          les bx,[bx+0x10e0]
00000832  26803900          cmp byte [es:bx+di],0x0
00000836  7502              jnz 0x83a
00000838  EB1F              jmp short 0x859
0000083A  8BDE              mov bx,si
0000083C  D1E3              shl bx,1
0000083E  D1E3              shl bx,1
00000840  C49FE010          les bx,[bx+0x10e0]
00000844  268A01            mov al,[es:bx+di]
00000847  04FD              add al,0xfd
00000849  8BDE              mov bx,si
0000084B  D1E3              shl bx,1
0000084D  D1E3              shl bx,1
0000084F  C49FE010          les bx,[bx+0x10e0]
00000853  268801            mov [es:bx+di],al
00000856  47                inc di
00000857  EBCF              jmp short 0x828
00000859  46                inc si
0000085A  83FE04            cmp si,byte +0x4
0000085D  7CC7              jl 0x826
0000085F  5F                pop di
00000860  5E                pop si
00000861  5D                pop bp
00000862  CB                retf

; ---- 21FD:0863 ----
00000863  55                push bp
00000864  8BEC              mov bp,sp
00000866  9A1B00960A        call 0xa96:0x1b
0000086B  B87100            mov ax,0x71
0000086E  50                push ax
0000086F  8B4E0C            mov cx,[bp+0xc]
00000872  8B5E0A            mov bx,[bp+0xa]
00000875  33D2              xor dx,dx
00000877  B80A00            mov ax,0xa
0000087A  9A4E4F0000        call 0x0:0x4f4e
0000087F  058C00            add ax,0x8c
00000882  50                push ax
00000883  B80800            mov ax,0x8
00000886  50                push ax
00000887  B80800            mov ax,0x8
0000088A  50                push ax
0000088B  B84800            mov ax,0x48
0000088E  50                push ax
0000088F  8B4606            mov ax,[bp+0x6]
00000892  D1E0              shl ax,1
00000894  D1E0              shl ax,1
00000896  D1E0              shl ax,1
00000898  053800            add ax,0x38
0000089B  50                push ax
0000089C  9A3000960A        call 0xa96:0x30
000008A1  83C40C            add sp,byte +0xc
000008A4  0E                push cs
000008A5  E865FD            call 0x60d
000008A8  5D                pop bp
000008A9  CB                retf

; ---- 21FD:08AA ----
000008AA  55                push bp
000008AB  8BEC              mov bp,sp
000008AD  9A1B00960A        call 0xa96:0x1b
000008B2  B00F              mov al,0xf
000008B4  50                push ax
000008B5  FF36EE10          push word [0x10ee]
000008B9  FF36EC10          push word [0x10ec]
000008BD  B84600            mov ax,0x46
000008C0  50                push ax
000008C1  B83600            mov ax,0x36
000008C4  50                push ax
000008C5  9AF9172D19        call 0x192d:0x17f9
000008CA  83C40A            add sp,byte +0xa
000008CD  0E                push cs
000008CE  E83CFD            call 0x60d
000008D1  5D                pop bp
000008D2  CB                retf

; ---- 21FD:08D3 ----
000008D3  55                push bp
000008D4  8BEC              mov bp,sp
000008D6  83EC04            sub sp,byte +0x4
000008D9  8B560C            mov dx,[bp+0xc]
000008DC  8B460A            mov ax,[bp+0xa]
000008DF  B105              mov cl,0x5
000008E1  9A37500000        call 0x0:0x5037
000008E6  034606            add ax,[bp+0x6]
000008E9  135608            adc dx,[bp+0x8]
000008EC  8946FC            mov [bp-0x4],ax
000008EF  8956FE            mov [bp-0x2],dx
000008F2  8B4EFE            mov cx,[bp-0x2]
000008F5  8B5EFC            mov bx,[bp-0x4]
000008F8  8B56FE            mov dx,[bp-0x2]
000008FB  8B46FC            mov ax,[bp-0x4]
000008FE  9A4E4F0000        call 0x0:0x4f4e
00000903  8946FC            mov [bp-0x4],ax
00000906  8956FE            mov [bp-0x2],dx
00000909  8B4EFE            mov cx,[bp-0x2]
0000090C  8B5EFC            mov bx,[bp-0x4]
0000090F  8B56FE            mov dx,[bp-0x2]
00000912  8B46FC            mov ax,[bp-0x4]
00000915  9A4E4F0000        call 0x0:0x4f4e
0000091A  8946FC            mov [bp-0x4],ax
0000091D  8956FE            mov [bp-0x2],dx
00000920  33C9              xor cx,cx
00000922  BBB77A            mov bx,0x7ab7
00000925  8B56FE            mov dx,[bp-0x2]
00000928  8B46FC            mov ax,[bp-0x4]
0000092B  9A4E4F0000        call 0x0:0x4f4e
00000930  8946FC            mov [bp-0x4],ax
00000933  8956FE            mov [bp-0x2],dx
00000936  33C0              xor ax,ax
00000938  BA0001            mov dx,0x100
0000093B  50                push ax
0000093C  52                push dx
0000093D  FF76FE            push word [bp-0x2]
00000940  FF76FC            push word [bp-0x4]
00000943  9A904F0000        call 0x0:0x4f90
00000948  8946FC            mov [bp-0x4],ax
0000094B  8956FE            mov [bp-0x2],dx
0000094E  33C0              xor ax,ax
00000950  BA0900            mov dx,0x9
00000953  50                push ax
00000954  52                push dx
00000955  FF76FE            push word [bp-0x2]
00000958  FF76FC            push word [bp-0x4]
0000095B  9AA04F0000        call 0x0:0x4fa0
00000960  8946FC            mov [bp-0x4],ax
00000963  8956FE            mov [bp-0x2],dx
00000966  8346FC01          add word [bp-0x4],byte +0x1
0000096A  8356FE00          adc word [bp-0x2],byte +0x0
0000096E  8B46FC            mov ax,[bp-0x4]
00000971  EB00              jmp short 0x973
00000973  8BE5              mov sp,bp
00000975  5D                pop bp
00000976  CB                retf

; ---- 21FD:0977 ----
00000977  55                push bp
00000978  8BEC              mov bp,sp
0000097A  5D                pop bp
0000097B  CB                retf
0000097C  55                push bp
0000097D  8BEC              mov bp,sp
0000097F  5D                pop bp
