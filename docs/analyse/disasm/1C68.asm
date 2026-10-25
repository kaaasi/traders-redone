; segment 1C68  file offset 0x1FC80  size 0x1870
00000000  C2FE8B            ret 0x8bfe
00000003  D8899722          fmul dword [bx+di+0x2297]
00000007  335E5D            xor bx,[bp+0x5d]
0000000A  CB                retf

; ---- 1C68:000B ----
0000000B  55                push bp
0000000C  8BEC              mov bp,sp
0000000E  803E1A4100        cmp byte [0x411a],0x0
00000013  7410              jz 0x25
00000015  A0EC08            mov al,[0x8ec]
00000018  98                cbw
00000019  50                push ax
0000001A  A19A4B            mov ax,[0x4b9a]
0000001D  99                cwd
0000001E  5B                pop bx
0000001F  F7FB              idiv bx
00000021  0BD2              or dx,dx
00000023  7402              jz 0x27
00000025  EB42              jmp short 0x69
00000027  9AA500681C        call 0x1c68:0xa5
0000002C  9A6B00681C        call 0x1c68:0x6b
00000031  0AC0              or al,al
00000033  7502              jnz 0x37
00000035  EB32              jmp short 0x69
00000037  B84700            mov ax,0x47
0000003A  50                push ax
0000003B  9A7D03FD21        call 0x21fd:0x37d
00000040  59                pop cx
00000041  9A04007E1E        call 0x1e7e:0x4
00000046  9A2E010F15        call 0x150f:0x12e
0000004B  9A0B00C812        call 0x12c8:0xb
00000050  9A8A073505        call 0x535:0x78a
00000055  9AED083505        call 0x535:0x8ed
0000005A  9A0A003505        call 0x535:0xa
0000005F  9A0900DE0E        call 0xede:0x9
00000064  9ACE063505        call 0x535:0x6ce
00000069  5D                pop bp
0000006A  CB                retf

; ---- 1C68:006B ----
0000006B  55                push bp
0000006C  8BEC              mov bp,sp
0000006E  4C                dec sp
0000006F  4C                dec sp
00000070  C646FF00          mov byte [bp-0x1],0x0
00000074  C606AA3700        mov byte [0x37aa],0x0
00000079  EB1A              jmp short 0x95
0000007B  9A2203DE0E        call 0xede:0x322
00000080  A0AA37            mov al,[0x37aa]
00000083  98                cbw
00000084  8BD8              mov bx,ax
00000086  80BFD24501        cmp byte [bx+0x45d2],0x1
0000008B  7504              jnz 0x91
0000008D  C646FF01          mov byte [bp-0x1],0x1
00000091  FE06AA37          inc byte [0x37aa]
00000095  803EAA3704        cmp byte [0x37aa],0x4
0000009A  7CDF              jl 0x7b
0000009C  8A46FF            mov al,[bp-0x1]
0000009F  EB00              jmp short 0xa1
000000A1  8BE5              mov sp,bp
000000A3  5D                pop bp
000000A4  CB                retf

; ---- 1C68:00A5 ----
000000A5  55                push bp
000000A6  8BEC              mov bp,sp
000000A8  FF36DE08          push word [0x8de]
000000AC  9A02006C18        call 0x186c:0x2
000000B1  59                pop cx
000000B2  9A7F17681C        call 0x1c68:0x177f
000000B7  9AA801681C        call 0x1c68:0x1a8
000000BC  B85305            mov ax,0x553
000000BF  50                push ax
000000C0  9A7D03FD21        call 0x21fd:0x37d
000000C5  59                pop cx
000000C6  9A750B681C        call 0x1c68:0xb75
000000CB  9A0E00D616        call 0x16d6:0xe
000000D0  9AFE17681C        call 0x1c68:0x17fe
000000D5  1E                push ds
000000D6  B84C0E            mov ax,0xe4c
000000D9  50                push ax
000000DA  9A6C02FD21        call 0x21fd:0x26c
000000DF  59                pop cx
000000E0  59                pop cx
000000E1  9A2903FD21        call 0x21fd:0x329
000000E6  9A1B00960A        call 0xa96:0x1b
000000EB  9AFC00681C        call 0x1c68:0xfc
000000F0  9A7204AF21        call 0x21af:0x472
000000F5  9AC803681C        call 0x1c68:0x3c8
000000FA  5D                pop bp
000000FB  CB                retf

; ---- 1C68:00FC ----
000000FC  55                push bp
000000FD  8BEC              mov bp,sp
000000FF  9A1F05FD21        call 0x21fd:0x51f
00000104  FF362E08          push word [0x82e]
00000108  9A1C07CD23        call 0x23cd:0x71c
0000010D  59                pop cx
0000010E  9AA806960A        call 0xa96:0x6a8
00000113  9A1B00960A        call 0xa96:0x1b
00000118  833E7A5404        cmp word [0x547a],byte +0x4
0000011D  7503              jnz 0x122
0000011F  E97F00            jmp 0x1a1
00000122  9A4502FD21        call 0x21fd:0x245
00000127  9AF606D616        call 0x16d6:0x6f6
0000012C  9A0E009E0E        call 0xe9e:0xe
00000131  C606AA3700        mov byte [0x37aa],0x0
00000136  EB2F              jmp short 0x167
00000138  A0AA37            mov al,[0x37aa]
0000013B  98                cbw
0000013C  8BD8              mov bx,ax
0000013E  8A87B243          mov al,[bx+0x43b2]
00000142  B400              mov ah,0x0
00000144  3D0B00            cmp ax,0xb
00000147  740E              jz 0x157
00000149  3D0C00            cmp ax,0xc
0000014C  7402              jz 0x150
0000014E  EB13              jmp short 0x163
00000150  9A8106681C        call 0x1c68:0x681
00000155  EB0C              jmp short 0x163
00000157  9AA904681C        call 0x1c68:0x4a9
0000015C  9A4106681C        call 0x1c68:0x641
00000161  EB00              jmp short 0x163
00000163  FE06AA37          inc byte [0x37aa]
00000167  803EAA3704        cmp byte [0x37aa],0x4
0000016C  7CCA              jl 0x138
0000016E  9A2418681C        call 0x1c68:0x1824
00000173  9AB914681C        call 0x1c68:0x14b9
00000178  9A510B681C        call 0x1c68:0xb51
0000017D  9AA806960A        call 0xa96:0x6a8
00000182  FF362E08          push word [0x82e]
00000186  9AE906CD23        call 0x23cd:0x6e9
0000018B  59                pop cx
0000018C  A1720E            mov ax,[0xe72]
0000018F  40                inc ax
00000190  A3720E            mov [0xe72],ax
00000193  3D0300            cmp ax,0x3
00000196  7E06              jng 0x19e
00000198  C706720E0000      mov word [0xe72],0x0
0000019E  E95EFF            jmp 0xff
000001A1  9A6506FD21        call 0x21fd:0x665
000001A6  5D                pop bp
000001A7  CB                retf

; ---- 1C68:01A8 ----
000001A8  55                push bp
000001A9  8BEC              mov bp,sp
000001AB  83EC04            sub sp,byte +0x4
000001AE  C7067A540000      mov word [0x547a],0x0
000001B4  C606AA3700        mov byte [0x37aa],0x0
000001B9  E92301            jmp 0x2df
000001BC  33C0              xor ax,ax
000001BE  BA0A00            mov dx,0xa
000001C1  50                push ax
000001C2  52                push dx
000001C3  9A13082508        call 0x825:0x813
000001C8  52                push dx
000001C9  50                push ax
000001CA  9A894F0000        call 0x0:0x4f89
000001CF  8946FC            mov [bp-0x4],ax
000001D2  8956FE            mov [bp-0x2],dx
000001D5  837EFE00          cmp word [bp-0x2],byte +0x0
000001D9  7F13              jg 0x1ee
000001DB  7C07              jl 0x1e4
000001DD  817EFCF401        cmp word [bp-0x4],0x1f4
000001E2  730A              jnc 0x1ee
000001E4  C746FCF401        mov word [bp-0x4],0x1f4
000001E9  C746FE0000        mov word [bp-0x2],0x0
000001EE  837EFE00          cmp word [bp-0x2],byte +0x0
000001F2  7C13              jl 0x207
000001F4  7F07              jg 0x1fd
000001F6  817EFCD007        cmp word [bp-0x4],0x7d0
000001FB  760A              jna 0x207
000001FD  C746FCD007        mov word [bp-0x4],0x7d0
00000202  C746FE0000        mov word [bp-0x2],0x0
00000207  A0AA37            mov al,[0x37aa]
0000020A  98                cbw
0000020B  8BD8              mov bx,ax
0000020D  80BF400801        cmp byte [bx+0x840],0x1
00000212  752A              jnz 0x23e
00000214  33C0              xor ax,ax
00000216  BA6400            mov dx,0x64
00000219  50                push ax
0000021A  52                push dx
0000021B  FF76FE            push word [bp-0x2]
0000021E  FF76FC            push word [bp-0x4]
00000221  9A894F0000        call 0x0:0x4f89
00000226  50                push ax
00000227  A18E2E            mov ax,[0x2e8e]
0000022A  BB0C00            mov bx,0xc
0000022D  52                push dx
0000022E  F7EB              imul bx
00000230  99                cwd
00000231  59                pop cx
00000232  5B                pop bx
00000233  9A4E4F0000        call 0x0:0x4f4e
00000238  0146FC            add [bp-0x4],ax
0000023B  1156FE            adc [bp-0x2],dx
0000023E  A0AA37            mov al,[0x37aa]
00000241  98                cbw
00000242  D1E0              shl ax,1
00000244  D1E0              shl ax,1
00000246  8B56FE            mov dx,[bp-0x2]
00000249  8B5EFC            mov bx,[bp-0x4]
0000024C  87D8              xchg bx,ax
0000024E  8987F44B          mov [bx+0x4bf4],ax
00000252  8997F64B          mov [bx+0x4bf6],dx
00000256  A0AA37            mov al,[0x37aa]
00000259  98                cbw
0000025A  8BD8              mov bx,ax
0000025C  C687B73CFF        mov byte [bx+0x3cb7],0xff
00000261  A0AA37            mov al,[0x37aa]
00000264  98                cbw
00000265  8BD8              mov bx,ax
00000267  C687C244FF        mov byte [bx+0x44c2],0xff
0000026C  A0AA37            mov al,[0x37aa]
0000026F  98                cbw
00000270  8BD8              mov bx,ax
00000272  C687012FFF        mov byte [bx+0x2f01],0xff
00000277  A0AA37            mov al,[0x37aa]
0000027A  98                cbw
0000027B  BA0600            mov dx,0x6
0000027E  F7EA              imul dx
00000280  058C00            add ax,0x8c
00000283  50                push ax
00000284  A0AA37            mov al,[0x37aa]
00000287  98                cbw
00000288  BA1A00            mov dx,0x1a
0000028B  F7EA              imul dx
0000028D  8BD8              mov bx,ax
0000028F  58                pop ax
00000290  89872033          mov [bx+0x3320],ax
00000294  A0AA37            mov al,[0x37aa]
00000297  98                cbw
00000298  BA1A00            mov dx,0x1a
0000029B  F7EA              imul dx
0000029D  50                push ax
0000029E  A0AA37            mov al,[0x37aa]
000002A1  98                cbw
000002A2  056E00            add ax,0x6e
000002A5  5B                pop bx
000002A6  89872233          mov [bx+0x3322],ax
000002AA  A0AA37            mov al,[0x37aa]
000002AD  98                cbw
000002AE  8BD8              mov bx,ax
000002B0  C6873E4400        mov byte [bx+0x443e],0x0
000002B5  A0AA37            mov al,[0x37aa]
000002B8  98                cbw
000002B9  D1E0              shl ax,1
000002BB  D1E0              shl ax,1
000002BD  8BD8              mov bx,ax
000002BF  C78786370000      mov word [bx+0x3786],0x0
000002C5  C78788370000      mov word [bx+0x3788],0x0
000002CB  A0AA37            mov al,[0x37aa]
000002CE  98                cbw
000002CF  8BD8              mov bx,ax
000002D1  C687B2430B        mov byte [bx+0x43b2],0xb
000002D6  9AED02681C        call 0x1c68:0x2ed
000002DB  FE06AA37          inc byte [0x37aa]
000002DF  803EAA3704        cmp byte [0x37aa],0x4
000002E4  7D03              jnl 0x2e9
000002E6  E9D3FE            jmp 0x1bc
000002E9  8BE5              mov sp,bp
000002EB  5D                pop bp
000002EC  CB                retf

; ---- 1C68:02ED ----
000002ED  55                push bp
000002EE  8BEC              mov bp,sp
000002F0  A0AA37            mov al,[0x37aa]
000002F3  98                cbw
000002F4  B104              mov cl,0x4
000002F6  D3E0              shl ax,cl
000002F8  8BD8              mov bx,ax
000002FA  C787A4480000      mov word [bx+0x48a4],0x0
00000300  A0AA37            mov al,[0x37aa]
00000303  98                cbw
00000304  D1E0              shl ax,1
00000306  D1E0              shl ax,1
00000308  8BD8              mov bx,ax
0000030A  8B876C37          mov ax,[bx+0x376c]
0000030E  50                push ax
0000030F  A0AA37            mov al,[0x37aa]
00000312  98                cbw
00000313  B104              mov cl,0x4
00000315  D3E0              shl ax,cl
00000317  8BD8              mov bx,ax
00000319  58                pop ax
0000031A  8987A648          mov [bx+0x48a6],ax
0000031E  A0AA37            mov al,[0x37aa]
00000321  98                cbw
00000322  D1E0              shl ax,1
00000324  D1E0              shl ax,1
00000326  8BD8              mov bx,ax
00000328  8B876E37          mov ax,[bx+0x376e]
0000032C  50                push ax
0000032D  A0AA37            mov al,[0x37aa]
00000330  98                cbw
00000331  B104              mov cl,0x4
00000333  D3E0              shl ax,cl
00000335  8BD8              mov bx,ax
00000337  58                pop ax
00000338  8987A848          mov [bx+0x48a8],ax
0000033C  A0AA37            mov al,[0x37aa]
0000033F  98                cbw
00000340  D1E0              shl ax,1
00000342  8BD8              mov bx,ax
00000344  8B87F82E          mov ax,[bx+0x2ef8]
00000348  50                push ax
00000349  A0AA37            mov al,[0x37aa]
0000034C  98                cbw
0000034D  B104              mov cl,0x4
0000034F  D3E0              shl ax,cl
00000351  8BD8              mov bx,ax
00000353  58                pop ax
00000354  8987AA48          mov [bx+0x48aa],ax
00000358  A0AA37            mov al,[0x37aa]
0000035B  98                cbw
0000035C  B104              mov cl,0x4
0000035E  D3E0              shl ax,cl
00000360  50                push ax
00000361  A0AA37            mov al,[0x37aa]
00000364  98                cbw
00000365  8BD8              mov bx,ax
00000367  8A877C37          mov al,[bx+0x377c]
0000036B  B400              mov ah,0x0
0000036D  5B                pop bx
0000036E  8987AC48          mov [bx+0x48ac],ax
00000372  A0AA37            mov al,[0x37aa]
00000375  98                cbw
00000376  D1E0              shl ax,1
00000378  8BD8              mov bx,ax
0000037A  8B87EC40          mov ax,[bx+0x40ec]
0000037E  50                push ax
0000037F  A0AA37            mov al,[0x37aa]
00000382  98                cbw
00000383  B104              mov cl,0x4
00000385  D3E0              shl ax,cl
00000387  8BD8              mov bx,ax
00000389  58                pop ax
0000038A  8987AE48          mov [bx+0x48ae],ax
0000038E  A0AA37            mov al,[0x37aa]
00000391  98                cbw
00000392  D1E0              shl ax,1
00000394  8BD8              mov bx,ax
00000396  8B87DC33          mov ax,[bx+0x33dc]
0000039A  50                push ax
0000039B  A0AA37            mov al,[0x37aa]
0000039E  98                cbw
0000039F  B104              mov cl,0x4
000003A1  D3E0              shl ax,cl
000003A3  8BD8              mov bx,ax
000003A5  58                pop ax
000003A6  8987B048          mov [bx+0x48b0],ax
000003AA  A0AA37            mov al,[0x37aa]
000003AD  98                cbw
000003AE  D1E0              shl ax,1
000003B0  8BD8              mov bx,ax
000003B2  8B871630          mov ax,[bx+0x3016]
000003B6  50                push ax
000003B7  A0AA37            mov al,[0x37aa]
000003BA  98                cbw
000003BB  B104              mov cl,0x4
000003BD  D3E0              shl ax,cl
000003BF  8BD8              mov bx,ax
000003C1  58                pop ax
000003C2  8987B248          mov [bx+0x48b2],ax
000003C6  5D                pop bp
000003C7  CB                retf

; ---- 1C68:03C8 ----
000003C8  55                push bp
000003C9  8BEC              mov bp,sp
000003CB  C606AA3700        mov byte [0x37aa],0x0
000003D0  E9CA00            jmp 0x49d
000003D3  A0AA37            mov al,[0x37aa]
000003D6  98                cbw
000003D7  B104              mov cl,0x4
000003D9  D3E0              shl ax,cl
000003DB  8BD8              mov bx,ax
000003DD  8B87A648          mov ax,[bx+0x48a6]
000003E1  50                push ax
000003E2  A0AA37            mov al,[0x37aa]
000003E5  98                cbw
000003E6  D1E0              shl ax,1
000003E8  D1E0              shl ax,1
000003EA  8BD8              mov bx,ax
000003EC  58                pop ax
000003ED  89876C37          mov [bx+0x376c],ax
000003F1  A0AA37            mov al,[0x37aa]
000003F4  98                cbw
000003F5  B104              mov cl,0x4
000003F7  D3E0              shl ax,cl
000003F9  8BD8              mov bx,ax
000003FB  8B87A848          mov ax,[bx+0x48a8]
000003FF  50                push ax
00000400  A0AA37            mov al,[0x37aa]
00000403  98                cbw
00000404  D1E0              shl ax,1
00000406  D1E0              shl ax,1
00000408  8BD8              mov bx,ax
0000040A  58                pop ax
0000040B  89876E37          mov [bx+0x376e],ax
0000040F  A0AA37            mov al,[0x37aa]
00000412  98                cbw
00000413  B104              mov cl,0x4
00000415  D3E0              shl ax,cl
00000417  8BD8              mov bx,ax
00000419  8B87AA48          mov ax,[bx+0x48aa]
0000041D  50                push ax
0000041E  A0AA37            mov al,[0x37aa]
00000421  98                cbw
00000422  D1E0              shl ax,1
00000424  8BD8              mov bx,ax
00000426  58                pop ax
00000427  8987F82E          mov [bx+0x2ef8],ax
0000042B  A0AA37            mov al,[0x37aa]
0000042E  98                cbw
0000042F  B104              mov cl,0x4
00000431  D3E0              shl ax,cl
00000433  8BD8              mov bx,ax
00000435  8A87AC48          mov al,[bx+0x48ac]
00000439  50                push ax
0000043A  A0AA37            mov al,[0x37aa]
0000043D  98                cbw
0000043E  8BD8              mov bx,ax
00000440  58                pop ax
00000441  88877C37          mov [bx+0x377c],al
00000445  A0AA37            mov al,[0x37aa]
00000448  98                cbw
00000449  B104              mov cl,0x4
0000044B  D3E0              shl ax,cl
0000044D  8BD8              mov bx,ax
0000044F  8B87AE48          mov ax,[bx+0x48ae]
00000453  50                push ax
00000454  A0AA37            mov al,[0x37aa]
00000457  98                cbw
00000458  D1E0              shl ax,1
0000045A  8BD8              mov bx,ax
0000045C  58                pop ax
0000045D  8987EC40          mov [bx+0x40ec],ax
00000461  A0AA37            mov al,[0x37aa]
00000464  98                cbw
00000465  B104              mov cl,0x4
00000467  D3E0              shl ax,cl
00000469  8BD8              mov bx,ax
0000046B  8B87B048          mov ax,[bx+0x48b0]
0000046F  50                push ax
00000470  A0AA37            mov al,[0x37aa]
00000473  98                cbw
00000474  D1E0              shl ax,1
00000476  8BD8              mov bx,ax
00000478  58                pop ax
00000479  8987DC33          mov [bx+0x33dc],ax
0000047D  A0AA37            mov al,[0x37aa]
00000480  98                cbw
00000481  B104              mov cl,0x4
00000483  D3E0              shl ax,cl
00000485  8BD8              mov bx,ax
00000487  8B87B248          mov ax,[bx+0x48b2]
0000048B  50                push ax
0000048C  A0AA37            mov al,[0x37aa]
0000048F  98                cbw
00000490  D1E0              shl ax,1
00000492  8BD8              mov bx,ax
00000494  58                pop ax
00000495  89871630          mov [bx+0x3016],ax
00000499  FE06AA37          inc byte [0x37aa]
0000049D  803EAA3704        cmp byte [0x37aa],0x4
000004A2  7D03              jnl 0x4a7
000004A4  E92CFF            jmp 0x3d3
000004A7  5D                pop bp
000004A8  CB                retf

; ---- 1C68:04A9 ----
000004A9  55                push bp
000004AA  8BEC              mov bp,sp
000004AC  A0AA37            mov al,[0x37aa]
000004AF  98                cbw
000004B0  8BD8              mov bx,ax
000004B2  80BFBE4400        cmp byte [bx+0x44be],0x0
000004B7  740D              jz 0x4c6
000004B9  A0AA37            mov al,[0x37aa]
000004BC  98                cbw
000004BD  8BD8              mov bx,ax
000004BF  C6873E4402        mov byte [bx+0x443e],0x2
000004C4  EB18              jmp short 0x4de
000004C6  A0AA37            mov al,[0x37aa]
000004C9  98                cbw
000004CA  8BD8              mov bx,ax
000004CC  80BFF04300        cmp byte [bx+0x43f0],0x0
000004D1  740B              jz 0x4de
000004D3  A0AA37            mov al,[0x37aa]
000004D6  98                cbw
000004D7  8BD8              mov bx,ax
000004D9  C6873E44FE        mov byte [bx+0x443e],0xfe
000004DE  A0AA37            mov al,[0x37aa]
000004E1  98                cbw
000004E2  8BD8              mov bx,ax
000004E4  80BF9D4300        cmp byte [bx+0x439d],0x0
000004E9  7428              jz 0x513
000004EB  9A1505681C        call 0x1c68:0x515
000004F0  A0AA37            mov al,[0x37aa]
000004F3  98                cbw
000004F4  8BD8              mov bx,ax
000004F6  80BF400800        cmp byte [bx+0x840],0x0
000004FB  7516              jnz 0x513
000004FD  A0AA37            mov al,[0x37aa]
00000500  98                cbw
00000501  D1E0              shl ax,1
00000503  D1E0              shl ax,1
00000505  8BD8              mov bx,ax
00000507  C7878637E803      mov word [bx+0x3786],0x3e8
0000050D  C78788370000      mov word [bx+0x3788],0x0
00000513  5D                pop bp
00000514  CB                retf

; ---- 1C68:0515 ----
00000515  55                push bp
00000516  8BEC              mov bp,sp
00000518  56                push si
00000519  57                push di
0000051A  A0AA37            mov al,[0x37aa]
0000051D  98                cbw
0000051E  BA1A00            mov dx,0x1a
00000521  F7EA              imul dx
00000523  8BD8              mov bx,ax
00000525  8BBF2033          mov di,[bx+0x3320]
00000529  A0AA37            mov al,[0x37aa]
0000052C  98                cbw
0000052D  8BD8              mov bx,ax
0000052F  C6873E4400        mov byte [bx+0x443e],0x0
00000534  B83200            mov ax,0x32
00000537  50                push ax
00000538  33C0              xor ax,ax
0000053A  50                push ax
0000053B  9A5400A721        call 0x21a7:0x54
00000540  59                pop cx
00000541  59                pop cx
00000542  33F6              xor si,si
00000544  E9EE00            jmp 0x635
00000547  8BDE              mov bx,si
00000549  D1E3              shl bx,1
0000054B  D1E3              shl bx,1
0000054D  39BFB208          cmp [bx+0x8b2],di
00000551  7E03              jng 0x556
00000553  E9DE00            jmp 0x634
00000556  8BDE              mov bx,si
00000558  D1E3              shl bx,1
0000055A  D1E3              shl bx,1
0000055C  39BFB408          cmp [bx+0x8b4],di
00000560  7D03              jnl 0x565
00000562  E9CF00            jmp 0x634
00000565  A0AA37            mov al,[0x37aa]
00000568  98                cbw
00000569  8BD8              mov bx,ax
0000056B  C687B2430C        mov byte [bx+0x43b2],0xc
00000570  8A84850E          mov al,[si+0xe85]
00000574  50                push ax
00000575  A0AA37            mov al,[0x37aa]
00000578  98                cbw
00000579  8BD8              mov bx,ax
0000057B  58                pop ax
0000057C  8887B73C          mov [bx+0x3cb7],al
00000580  8A848A0E          mov al,[si+0xe8a]
00000584  50                push ax
00000585  A0AA37            mov al,[0x37aa]
00000588  98                cbw
00000589  8BD8              mov bx,ax
0000058B  58                pop ax
0000058C  8887C244          mov [bx+0x44c2],al
00000590  A0AA37            mov al,[0x37aa]
00000593  98                cbw
00000594  8BD8              mov bx,ax
00000596  8A87C244          mov al,[bx+0x44c2]
0000059A  98                cbw
0000059B  50                push ax
0000059C  9A280A681C        call 0x1c68:0xa28
000005A1  59                pop cx
000005A2  99                cwd
000005A3  50                push ax
000005A4  A0AA37            mov al,[0x37aa]
000005A7  98                cbw
000005A8  D1E0              shl ax,1
000005AA  D1E0              shl ax,1
000005AC  8BD8              mov bx,ax
000005AE  58                pop ax
000005AF  8987BC3C          mov [bx+0x3cbc],ax
000005B3  8997BE3C          mov [bx+0x3cbe],dx
000005B7  A0AA37            mov al,[0x37aa]
000005BA  98                cbw
000005BB  8BD8              mov bx,ax
000005BD  80BFB73C01        cmp byte [bx+0x3cb7],0x1
000005C2  755D              jnz 0x621
000005C4  A0AA37            mov al,[0x37aa]
000005C7  98                cbw
000005C8  D1E0              shl ax,1
000005CA  8BD8              mov bx,ax
000005CC  8B879008          mov ax,[bx+0x890]
000005D0  052F00            add ax,0x2f
000005D3  50                push ax
000005D4  A0AA37            mov al,[0x37aa]
000005D7  98                cbw
000005D8  D1E0              shl ax,1
000005DA  8BD8              mov bx,ax
000005DC  8B878808          mov ax,[bx+0x888]
000005E0  056100            add ax,0x61
000005E3  50                push ax
000005E4  9A8C0D681C        call 0x1c68:0xd8c
000005E9  59                pop cx
000005EA  59                pop cx
000005EB  33C0              xor ax,ax
000005ED  50                push ax
000005EE  A0AA37            mov al,[0x37aa]
000005F1  98                cbw
000005F2  D1E0              shl ax,1
000005F4  8BD8              mov bx,ax
000005F6  8B879008          mov ax,[bx+0x890]
000005FA  052E00            add ax,0x2e
000005FD  50                push ax
000005FE  A0AA37            mov al,[0x37aa]
00000601  98                cbw
00000602  D1E0              shl ax,1
00000604  8BD8              mov bx,ax
00000606  8B878808          mov ax,[bx+0x888]
0000060A  057B00            add ax,0x7b
0000060D  50                push ax
0000060E  9AEA0D681C        call 0x1c68:0xdea
00000613  83C406            add sp,byte +0x6
00000616  33C0              xor ax,ax
00000618  50                push ax
00000619  9A2214681C        call 0x1c68:0x1422
0000061E  59                pop cx
0000061F  EB11              jmp short 0x632
00000621  A0AA37            mov al,[0x37aa]
00000624  98                cbw
00000625  8BD8              mov bx,ax
00000627  80BFB73C00        cmp byte [bx+0x3cb7],0x0
0000062C  7504              jnz 0x632
0000062E  FF067A54          inc word [0x547a]
00000632  EB09              jmp short 0x63d
00000634  46                inc si
00000635  83FE04            cmp si,byte +0x4
00000638  7F03              jg 0x63d
0000063A  E90AFF            jmp 0x547
0000063D  5F                pop di
0000063E  5E                pop si
0000063F  5D                pop bp
00000640  CB                retf

; ---- 1C68:0641 ----
00000641  55                push bp
00000642  8BEC              mov bp,sp
00000644  56                push si
00000645  A0AA37            mov al,[0x37aa]
00000648  98                cbw
00000649  BA1A00            mov dx,0x1a
0000064C  F7EA              imul dx
0000064E  8BD8              mov bx,ax
00000650  8B872033          mov ax,[bx+0x3320]
00000654  50                push ax
00000655  A0AA37            mov al,[0x37aa]
00000658  98                cbw
00000659  8BD8              mov bx,ax
0000065B  8A873E44          mov al,[bx+0x443e]
0000065F  98                cbw
00000660  5A                pop dx
00000661  03D0              add dx,ax
00000663  8BF2              mov si,dx
00000665  0BF6              or si,si
00000667  7C15              jl 0x67e
00000669  81FE2D01          cmp si,0x12d
0000066D  7D0F              jnl 0x67e
0000066F  A0AA37            mov al,[0x37aa]
00000672  98                cbw
00000673  BA1A00            mov dx,0x1a
00000676  F7EA              imul dx
00000678  8BD8              mov bx,ax
0000067A  89B72033          mov [bx+0x3320],si
0000067E  5E                pop si
0000067F  5D                pop bp
00000680  CB                retf

; ---- 1C68:0681 ----
00000681  55                push bp
00000682  8BEC              mov bp,sp
00000684  56                push si
00000685  A0AA37            mov al,[0x37aa]
00000688  98                cbw
00000689  8BD8              mov bx,ax
0000068B  8A87B73C          mov al,[bx+0x3cb7]
0000068F  B400              mov ah,0x0
00000691  8BF0              mov si,ax
00000693  A0AA37            mov al,[0x37aa]
00000696  98                cbw
00000697  8BD8              mov bx,ax
00000699  80BFBA4400        cmp byte [bx+0x44ba],0x0
0000069E  743F              jz 0x6df
000006A0  0BF6              or si,si
000006A2  7504              jnz 0x6a8
000006A4  FF0E7A54          dec word [0x547a]
000006A8  A0AA37            mov al,[0x37aa]
000006AB  98                cbw
000006AC  8BD8              mov bx,ax
000006AE  C687C244FF        mov byte [bx+0x44c2],0xff
000006B3  9A6110681C        call 0x1c68:0x1061
000006B8  A0AA37            mov al,[0x37aa]
000006BB  98                cbw
000006BC  8BD8              mov bx,ax
000006BE  C687B73CFF        mov byte [bx+0x3cb7],0xff
000006C3  A0AA37            mov al,[0x37aa]
000006C6  98                cbw
000006C7  8BD8              mov bx,ax
000006C9  C687B2430B        mov byte [bx+0x43b2],0xb
000006CE  B83300            mov ax,0x33
000006D1  50                push ax
000006D2  33C0              xor ax,ax
000006D4  50                push ax
000006D5  9A5400A721        call 0x21a7:0x54
000006DA  59                pop cx
000006DB  59                pop cx
000006DC  E93001            jmp 0x80f
000006DF  A0AA37            mov al,[0x37aa]
000006E2  98                cbw
000006E3  8BD8              mov bx,ax
000006E5  80BFF04300        cmp byte [bx+0x43f0],0x0
000006EA  747C              jz 0x768
000006EC  A0AA37            mov al,[0x37aa]
000006EF  98                cbw
000006F0  8BD8              mov bx,ax
000006F2  8A87C244          mov al,[bx+0x44c2]
000006F6  FEC8              dec al
000006F8  50                push ax
000006F9  A0AA37            mov al,[0x37aa]
000006FC  98                cbw
000006FD  8BD8              mov bx,ax
000006FF  58                pop ax
00000700  8887C244          mov [bx+0x44c2],al
00000704  3A849808          cmp al,[si+0x898]
00000708  7D10              jnl 0x71a
0000070A  8A849D08          mov al,[si+0x89d]
0000070E  50                push ax
0000070F  A0AA37            mov al,[0x37aa]
00000712  98                cbw
00000713  8BD8              mov bx,ax
00000715  58                pop ax
00000716  8887C244          mov [bx+0x44c2],al
0000071A  A0AA37            mov al,[0x37aa]
0000071D  98                cbw
0000071E  8BD8              mov bx,ax
00000720  8A87C244          mov al,[bx+0x44c2]
00000724  98                cbw
00000725  50                push ax
00000726  9A280A681C        call 0x1c68:0xa28
0000072B  59                pop cx
0000072C  99                cwd
0000072D  50                push ax
0000072E  A0AA37            mov al,[0x37aa]
00000731  98                cbw
00000732  D1E0              shl ax,1
00000734  D1E0              shl ax,1
00000736  8BD8              mov bx,ax
00000738  58                pop ax
00000739  8987BC3C          mov [bx+0x3cbc],ax
0000073D  8997BE3C          mov [bx+0x3cbe],dx
00000741  B83400            mov ax,0x34
00000744  50                push ax
00000745  33C0              xor ax,ax
00000747  50                push ax
00000748  9A5400A721        call 0x21a7:0x54
0000074D  59                pop cx
0000074E  59                pop cx
0000074F  A0AA37            mov al,[0x37aa]
00000752  98                cbw
00000753  D1E0              shl ax,1
00000755  D1E0              shl ax,1
00000757  8BD8              mov bx,ax
00000759  C78786370300      mov word [bx+0x3786],0x3
0000075F  C78788370000      mov word [bx+0x3788],0x0
00000765  E9A700            jmp 0x80f
00000768  A0AA37            mov al,[0x37aa]
0000076B  98                cbw
0000076C  8BD8              mov bx,ax
0000076E  80BFBE4400        cmp byte [bx+0x44be],0x0
00000773  747B              jz 0x7f0
00000775  A0AA37            mov al,[0x37aa]
00000778  98                cbw
00000779  8BD8              mov bx,ax
0000077B  8A87C244          mov al,[bx+0x44c2]
0000077F  FEC0              inc al
00000781  50                push ax
00000782  A0AA37            mov al,[0x37aa]
00000785  98                cbw
00000786  8BD8              mov bx,ax
00000788  58                pop ax
00000789  8887C244          mov [bx+0x44c2],al
0000078D  3A849D08          cmp al,[si+0x89d]
00000791  7E10              jng 0x7a3
00000793  8A849808          mov al,[si+0x898]
00000797  50                push ax
00000798  A0AA37            mov al,[0x37aa]
0000079B  98                cbw
0000079C  8BD8              mov bx,ax
0000079E  58                pop ax
0000079F  8887C244          mov [bx+0x44c2],al
000007A3  A0AA37            mov al,[0x37aa]
000007A6  98                cbw
000007A7  8BD8              mov bx,ax
000007A9  8A87C244          mov al,[bx+0x44c2]
000007AD  98                cbw
000007AE  50                push ax
000007AF  9A280A681C        call 0x1c68:0xa28
000007B4  59                pop cx
000007B5  99                cwd
000007B6  50                push ax
000007B7  A0AA37            mov al,[0x37aa]
000007BA  98                cbw
000007BB  D1E0              shl ax,1
000007BD  D1E0              shl ax,1
000007BF  8BD8              mov bx,ax
000007C1  58                pop ax
000007C2  8987BC3C          mov [bx+0x3cbc],ax
000007C6  8997BE3C          mov [bx+0x3cbe],dx
000007CA  B83500            mov ax,0x35
000007CD  50                push ax
000007CE  33C0              xor ax,ax
000007D0  50                push ax
000007D1  9A5400A721        call 0x21a7:0x54
000007D6  59                pop cx
000007D7  59                pop cx
000007D8  A0AA37            mov al,[0x37aa]
000007DB  98                cbw
000007DC  D1E0              shl ax,1
000007DE  D1E0              shl ax,1
000007E0  8BD8              mov bx,ax
000007E2  C78786370300      mov word [bx+0x3786],0x3
000007E8  C78788370000      mov word [bx+0x3788],0x0
000007EE  EB1F              jmp short 0x80f
000007F0  A0AA37            mov al,[0x37aa]
000007F3  98                cbw
000007F4  8BD8              mov bx,ax
000007F6  80BF944800        cmp byte [bx+0x4894],0x0
000007FB  750D              jnz 0x80a
000007FD  A0AA37            mov al,[0x37aa]
00000800  98                cbw
00000801  8BD8              mov bx,ax
00000803  80BF9D4300        cmp byte [bx+0x439d],0x0
00000808  7405              jz 0x80f
0000080A  9A1208681C        call 0x1c68:0x812
0000080F  5E                pop si
00000810  5D                pop bp
00000811  CB                retf

; ---- 1C68:0812 ----
00000812  55                push bp
00000813  8BEC              mov bp,sp
00000815  83EC06            sub sp,byte +0x6
00000818  A0AA37            mov al,[0x37aa]
0000081B  98                cbw
0000081C  8BD8              mov bx,ax
0000081E  8A87C244          mov al,[bx+0x44c2]
00000822  98                cbw
00000823  8946FE            mov [bp-0x2],ax
00000826  837EFE08          cmp word [bp-0x2],byte +0x8
0000082A  7D23              jnl 0x84f
0000082C  A0AA37            mov al,[0x37aa]
0000082F  98                cbw
00000830  B104              mov cl,0x4
00000832  D3E0              shl ax,cl
00000834  8B56FE            mov dx,[bp-0x2]
00000837  D1E2              shl dx,1
00000839  03C2              add ax,dx
0000083B  8BD8              mov bx,ax
0000083D  8B87A448          mov ax,[bx+0x48a4]
00000841  8B5EFE            mov bx,[bp-0x2]
00000844  D1E3              shl bx,1
00000846  3B87A208          cmp ax,[bx+0x8a2]
0000084A  7203              jc 0x84f
0000084C  E94A01            jmp 0x999
0000084F  A0AA37            mov al,[0x37aa]
00000852  98                cbw
00000853  D1E0              shl ax,1
00000855  D1E0              shl ax,1
00000857  8BD8              mov bx,ax
00000859  8B87F64B          mov ax,[bx+0x4bf6]
0000085D  8B97F44B          mov dx,[bx+0x4bf4]
00000861  50                push ax
00000862  A0AA37            mov al,[0x37aa]
00000865  98                cbw
00000866  D1E0              shl ax,1
00000868  D1E0              shl ax,1
0000086A  8BD8              mov bx,ax
0000086C  2B97BC3C          sub dx,[bx+0x3cbc]
00000870  58                pop ax
00000871  1B87BE3C          sbb ax,[bx+0x3cbe]
00000875  8956FA            mov [bp-0x6],dx
00000878  8946FC            mov [bp-0x4],ax
0000087B  0BC0              or ax,ax
0000087D  7F09              jg 0x888
0000087F  7C04              jl 0x885
00000881  0BD2              or dx,dx
00000883  7303              jnc 0x888
00000885  E91101            jmp 0x999
00000888  A0AA37            mov al,[0x37aa]
0000088B  98                cbw
0000088C  D1E0              shl ax,1
0000088E  D1E0              shl ax,1
00000890  8B56FC            mov dx,[bp-0x4]
00000893  8B5EFA            mov bx,[bp-0x6]
00000896  87D8              xchg bx,ax
00000898  8987F44B          mov [bx+0x4bf4],ax
0000089C  8997F64B          mov [bx+0x4bf6],dx
000008A0  9AF60E681C        call 0x1c68:0xef6
000008A5  837EFE00          cmp word [bp-0x2],byte +0x0
000008A9  7503              jnz 0x8ae
000008AB  E98900            jmp 0x937
000008AE  837EFE08          cmp word [bp-0x2],byte +0x8
000008B2  7D1C              jnl 0x8d0
000008B4  A0AA37            mov al,[0x37aa]
000008B7  98                cbw
000008B8  B104              mov cl,0x4
000008BA  D3E0              shl ax,cl
000008BC  8B56FE            mov dx,[bp-0x2]
000008BF  D1E2              shl dx,1
000008C1  03C2              add ax,dx
000008C3  8BD8              mov bx,ax
000008C5  FF87A448          inc word [bx+0x48a4]
000008C9  9A410F681C        call 0x1c68:0xf41
000008CE  EB67              jmp short 0x937
000008D0  837EFE0C          cmp word [bp-0x2],byte +0xc
000008D4  7D36              jnl 0x90c
000008D6  8A46FE            mov al,[bp-0x2]
000008D9  04F8              add al,0xf8
000008DB  50                push ax
000008DC  A0AA37            mov al,[0x37aa]
000008DF  98                cbw
000008E0  8BD8              mov bx,ax
000008E2  58                pop ax
000008E3  8887012F          mov [bx+0x2f01],al
000008E7  9AE90F681C        call 0x1c68:0xfe9
000008EC  FF76FE            push word [bp-0x2]
000008EF  9A280A681C        call 0x1c68:0xa28
000008F4  59                pop cx
000008F5  99                cwd
000008F6  50                push ax
000008F7  A0AA37            mov al,[0x37aa]
000008FA  98                cbw
000008FB  D1E0              shl ax,1
000008FD  D1E0              shl ax,1
000008FF  8BD8              mov bx,ax
00000901  58                pop ax
00000902  8987BC3C          mov [bx+0x3cbc],ax
00000906  8997BE3C          mov [bx+0x3cbe],dx
0000090A  EB2B              jmp short 0x937
0000090C  8B46FE            mov ax,[bp-0x2]
0000090F  05F4FF            add ax,0xfff4
00000912  50                push ax
00000913  9A9D09681C        call 0x1c68:0x99d
00000918  59                pop cx
00000919  FF76FE            push word [bp-0x2]
0000091C  9A280A681C        call 0x1c68:0xa28
00000921  59                pop cx
00000922  99                cwd
00000923  50                push ax
00000924  A0AA37            mov al,[0x37aa]
00000927  98                cbw
00000928  D1E0              shl ax,1
0000092A  D1E0              shl ax,1
0000092C  8BD8              mov bx,ax
0000092E  58                pop ax
0000092F  8987BC3C          mov [bx+0x3cbc],ax
00000933  8997BE3C          mov [bx+0x3cbe],dx
00000937  B83600            mov ax,0x36
0000093A  50                push ax
0000093B  B80100            mov ax,0x1
0000093E  50                push ax
0000093F  9A5400A721        call 0x21a7:0x54
00000944  59                pop cx
00000945  59                pop cx
00000946  A0AA37            mov al,[0x37aa]
00000949  98                cbw
0000094A  8BD8              mov bx,ax
0000094C  80BF400800        cmp byte [bx+0x840],0x0
00000951  7546              jnz 0x999
00000953  837EFE08          cmp word [bp-0x2],byte +0x8
00000957  7C05              jl 0x95e
00000959  B81027            mov ax,0x2710
0000095C  EB03              jmp short 0x961
0000095E  B80700            mov ax,0x7
00000961  99                cwd
00000962  50                push ax
00000963  A0AA37            mov al,[0x37aa]
00000966  98                cbw
00000967  D1E0              shl ax,1
00000969  D1E0              shl ax,1
0000096B  8BD8              mov bx,ax
0000096D  58                pop ax
0000096E  89872E30          mov [bx+0x302e],ax
00000972  89973030          mov [bx+0x3030],dx
00000976  837EFE08          cmp word [bp-0x2],byte +0x8
0000097A  7C05              jl 0x981
0000097C  B81027            mov ax,0x2710
0000097F  EB03              jmp short 0x984
00000981  B80300            mov ax,0x3
00000984  99                cwd
00000985  50                push ax
00000986  A0AA37            mov al,[0x37aa]
00000989  98                cbw
0000098A  D1E0              shl ax,1
0000098C  D1E0              shl ax,1
0000098E  8BD8              mov bx,ax
00000990  58                pop ax
00000991  89878637          mov [bx+0x3786],ax
00000995  89978837          mov [bx+0x3788],dx
00000999  8BE5              mov sp,bp
0000099B  5D                pop bp
0000099C  CB                retf

; ---- 1C68:099D ----
0000099D  55                push bp
0000099E  8BEC              mov bp,sp
000009A0  4C                dec sp
000009A1  4C                dec sp
000009A2  56                push si
000009A3  8B4606            mov ax,[bp+0x6]
000009A6  BB0700            mov bx,0x7
000009A9  99                cwd
000009AA  F7FB              idiv bx
000009AC  8BF0              mov si,ax
000009AE  8B4606            mov ax,[bp+0x6]
000009B1  BB0700            mov bx,0x7
000009B4  99                cwd
000009B5  F7FB              idiv bx
000009B7  BB0A00            mov bx,0xa
000009BA  8BC2              mov ax,dx
000009BC  F7EB              imul bx
000009BE  0409              add al,0x9
000009C0  8846FF            mov [bp-0x1],al
000009C3  A0AA37            mov al,[0x37aa]
000009C6  98                cbw
000009C7  BA0600            mov dx,0x6
000009CA  F7EA              imul dx
000009CC  8BD6              mov dx,si
000009CE  D1E2              shl dx,1
000009D0  03C2              add ax,dx
000009D2  8A56FF            mov dl,[bp-0x1]
000009D5  8BD8              mov bx,ax
000009D7  8897CE43          mov [bx+0x43ce],dl
000009DB  A0AA37            mov al,[0x37aa]
000009DE  98                cbw
000009DF  BA0600            mov dx,0x6
000009E2  F7EA              imul dx
000009E4  8BD6              mov dx,si
000009E6  D1E2              shl dx,1
000009E8  03C2              add ax,dx
000009EA  8A56FF            mov dl,[bp-0x1]
000009ED  8BD8              mov bx,ax
000009EF  8897CF43          mov [bx+0x43cf],dl
000009F3  56                push si
000009F4  A0AA37            mov al,[0x37aa]
000009F7  98                cbw
000009F8  D1E0              shl ax,1
000009FA  8BD8              mov bx,ax
000009FC  8B879008          mov ax,[bx+0x890]
00000A00  050300            add ax,0x3
00000A03  50                push ax
00000A04  A0AA37            mov al,[0x37aa]
00000A07  98                cbw
00000A08  D1E0              shl ax,1
00000A0A  8BD8              mov bx,ax
00000A0C  8B878808          mov ax,[bx+0x888]
00000A10  050800            add ax,0x8
00000A13  50                push ax
00000A14  9A870E681C        call 0x1c68:0xe87
00000A19  83C406            add sp,byte +0x6
00000A1C  56                push si
00000A1D  9A7213681C        call 0x1c68:0x1372
00000A22  59                pop cx
00000A23  5E                pop si
00000A24  8BE5              mov sp,bp
00000A26  5D                pop bp
00000A27  CB                retf

; ---- 1C68:0A28 ----
00000A28  55                push bp
00000A29  8BEC              mov bp,sp
00000A2B  83EC04            sub sp,byte +0x4
00000A2E  56                push si
00000A2F  57                push di
00000A30  837E0608          cmp word [bp+0x6],byte +0x8
00000A34  7D0B              jnl 0xa41
00000A36  8B5E06            mov bx,[bp+0x6]
00000A39  D1E3              shl bx,1
00000A3B  8BB78F0E          mov si,[bx+0xe8f]
00000A3F  EB65              jmp short 0xaa6
00000A41  837E060C          cmp word [bp+0x6],byte +0xc
00000A45  7D28              jnl 0xa6f
00000A47  8B5E06            mov bx,[bp+0x6]
00000A4A  D1E3              shl bx,1
00000A4C  8BB78F0E          mov si,[bx+0xe8f]
00000A50  A0AA37            mov al,[0x37aa]
00000A53  98                cbw
00000A54  8BD8              mov bx,ax
00000A56  8A87012F          mov al,[bx+0x2f01]
00000A5A  8846FF            mov [bp-0x1],al
00000A5D  3CFF              cmp al,0xff
00000A5F  740C              jz 0xa6d
00000A61  8A46FF            mov al,[bp-0x1]
00000A64  98                cbw
00000A65  D1E0              shl ax,1
00000A67  8BD8              mov bx,ax
00000A69  2BB79F0E          sub si,[bx+0xe9f]
00000A6D  EB37              jmp short 0xaa6
00000A6F  8A4606            mov al,[bp+0x6]
00000A72  04F4              add al,0xf4
00000A74  8846FD            mov [bp-0x3],al
00000A77  8A46FD            mov al,[bp-0x3]
00000A7A  98                cbw
00000A7B  BB0700            mov bx,0x7
00000A7E  99                cwd
00000A7F  F7FB              idiv bx
00000A81  D0E2              shl dl,1
00000A83  8856FE            mov [bp-0x2],dl
00000A86  8A46FE            mov al,[bp-0x2]
00000A89  98                cbw
00000A8A  BA4B00            mov dx,0x4b
00000A8D  F7EA              imul dx
00000A8F  8BF0              mov si,ax
00000A91  8A46FD            mov al,[bp-0x3]
00000A94  98                cbw
00000A95  BB0700            mov bx,0x7
00000A98  99                cwd
00000A99  F7FB              idiv bx
00000A9B  50                push ax
00000A9C  9AB00A681C        call 0x1c68:0xab0
00000AA1  59                pop cx
00000AA2  8BF8              mov di,ax
00000AA4  2BF7              sub si,di
00000AA6  8BC6              mov ax,si
00000AA8  EB00              jmp short 0xaaa
00000AAA  5F                pop di
00000AAB  5E                pop si
00000AAC  8BE5              mov sp,bp
00000AAE  5D                pop bp
00000AAF  CB                retf

; ---- 1C68:0AB0 ----
00000AB0  55                push bp
00000AB1  8BEC              mov bp,sp
00000AB3  56                push si
00000AB4  A0AA37            mov al,[0x37aa]
00000AB7  98                cbw
00000AB8  BA0600            mov dx,0x6
00000ABB  F7EA              imul dx
00000ABD  8B5606            mov dx,[bp+0x6]
00000AC0  D1E2              shl dx,1
00000AC2  03C2              add ax,dx
00000AC4  8BD8              mov bx,ax
00000AC6  8A87CE43          mov al,[bx+0x43ce]
00000ACA  B400              mov ah,0x0
00000ACC  BB0A00            mov bx,0xa
00000ACF  99                cwd
00000AD0  F7FB              idiv bx
00000AD2  50                push ax
00000AD3  A0AA37            mov al,[0x37aa]
00000AD6  98                cbw
00000AD7  BA0600            mov dx,0x6
00000ADA  F7EA              imul dx
00000ADC  8B5606            mov dx,[bp+0x6]
00000ADF  D1E2              shl dx,1
00000AE1  03C2              add ax,dx
00000AE3  8BD8              mov bx,ax
00000AE5  8A87CF43          mov al,[bx+0x43cf]
00000AE9  B400              mov ah,0x0
00000AEB  BB0A00            mov bx,0xa
00000AEE  99                cwd
00000AEF  F7FB              idiv bx
00000AF1  5A                pop dx
00000AF2  03D0              add dx,ax
00000AF4  8BF2              mov si,dx
00000AF6  8BC6              mov ax,si
00000AF8  BA4B00            mov dx,0x4b
00000AFB  F7EA              imul dx
00000AFD  EB00              jmp short 0xaff
00000AFF  5E                pop si
00000B00  5D                pop bp
00000B01  CB                retf

; ---- 1C68:0B02 ----
00000B02  55                push bp
00000B03  8BEC              mov bp,sp
00000B05  A0AA37            mov al,[0x37aa]
00000B08  98                cbw
00000B09  BA0600            mov dx,0x6
00000B0C  F7EA              imul dx
00000B0E  8B5606            mov dx,[bp+0x6]
00000B11  D1E2              shl dx,1
00000B13  03C2              add ax,dx
00000B15  8BD8              mov bx,ax
00000B17  8A87CE43          mov al,[bx+0x43ce]
00000B1B  B400              mov ah,0x0
00000B1D  BB0A00            mov bx,0xa
00000B20  99                cwd
00000B21  F7FB              idiv bx
00000B23  50                push ax
00000B24  A0AA37            mov al,[0x37aa]
00000B27  98                cbw
00000B28  BA0600            mov dx,0x6
00000B2B  F7EA              imul dx
00000B2D  8B5606            mov dx,[bp+0x6]
00000B30  D1E2              shl dx,1
00000B32  03C2              add ax,dx
00000B34  8BD8              mov bx,ax
00000B36  8A87CF43          mov al,[bx+0x43cf]
00000B3A  B400              mov ah,0x0
00000B3C  BB0A00            mov bx,0xa
00000B3F  99                cwd
00000B40  F7FB              idiv bx
00000B42  8BD0              mov dx,ax
00000B44  58                pop ax
00000B45  03C2              add ax,dx
00000B47  BB0200            mov bx,0x2
00000B4A  99                cwd
00000B4B  F7FB              idiv bx
00000B4D  EB00              jmp short 0xb4f
00000B4F  5D                pop bp
00000B50  CB                retf

; ---- 1C68:0B51 ----
00000B51  55                push bp
00000B52  8BEC              mov bp,sp
00000B54  A0720E            mov al,[0xe72]
00000B57  A2AA37            mov [0x37aa],al
00000B5A  A0AA37            mov al,[0x37aa]
00000B5D  98                cbw
00000B5E  8BD8              mov bx,ax
00000B60  80BFC244FF        cmp byte [bx+0x44c2],0xff
00000B65  7507              jnz 0xb6e
00000B67  9AA310681C        call 0x1c68:0x10a3
00000B6C  EB05              jmp short 0xb73
00000B6E  9ADC10681C        call 0x1c68:0x10dc
00000B73  5D                pop bp
00000B74  CB                retf

; ---- 1C68:0B75 ----
00000B75  55                push bp
00000B76  8BEC              mov bp,sp
00000B78  4C                dec sp
00000B79  4C                dec sp
00000B7A  56                push si
00000B7B  57                push di
00000B7C  9A1B00960A        call 0xa96:0x1b
00000B81  33C0              xor ax,ax
00000B83  50                push ax
00000B84  B84D00            mov ax,0x4d
00000B87  50                push ax
00000B88  B84001            mov ax,0x140
00000B8B  50                push ax
00000B8C  B87B00            mov ax,0x7b
00000B8F  50                push ax
00000B90  33C0              xor ax,ax
00000B92  50                push ax
00000B93  9AD704960A        call 0xa96:0x4d7
00000B98  83C40A            add sp,byte +0xa
00000B9B  B84B00            mov ax,0x4b
00000B9E  50                push ax
00000B9F  33C0              xor ax,ax
00000BA1  50                push ax
00000BA2  B83000            mov ax,0x30
00000BA5  50                push ax
00000BA6  B84001            mov ax,0x140
00000BA9  50                push ax
00000BAA  B8D000            mov ax,0xd0
00000BAD  50                push ax
00000BAE  33C0              xor ax,ax
00000BB0  50                push ax
00000BB1  9A3000960A        call 0xa96:0x30
00000BB6  83C40C            add sp,byte +0xc
00000BB9  33C0              xor ax,ax
00000BBB  50                push ax
00000BBC  B89000            mov ax,0x90
00000BBF  50                push ax
00000BC0  B84B00            mov ax,0x4b
00000BC3  50                push ax
00000BC4  B82000            mov ax,0x20
00000BC7  50                push ax
00000BC8  B88000            mov ax,0x80
00000BCB  50                push ax
00000BCC  33C0              xor ax,ax
00000BCE  50                push ax
00000BCF  9A3000960A        call 0xa96:0x30
00000BD4  83C40C            add sp,byte +0xc
00000BD7  9AA806960A        call 0xa96:0x6a8
00000BDC  C606AA3700        mov byte [0x37aa],0x0
00000BE1  E98E01            jmp 0xd72
00000BE4  A0AA37            mov al,[0x37aa]
00000BE7  98                cbw
00000BE8  D1E0              shl ax,1
00000BEA  8BD8              mov bx,ax
00000BEC  8BBF8808          mov di,[bx+0x888]
00000BF0  A0AA37            mov al,[0x37aa]
00000BF3  98                cbw
00000BF4  D1E0              shl ax,1
00000BF6  8BD8              mov bx,ax
00000BF8  8BB79008          mov si,[bx+0x890]
00000BFC  9A1B00960A        call 0xa96:0x1b
00000C01  B80B00            mov ax,0xb
00000C04  50                push ax
00000C05  B84B00            mov ax,0x4b
00000C08  50                push ax
00000C09  B89000            mov ax,0x90
00000C0C  50                push ax
00000C0D  56                push si
00000C0E  57                push di
00000C0F  9A8104960A        call 0xa96:0x481
00000C14  83C40A            add sp,byte +0xa
00000C17  A0AA37            mov al,[0x37aa]
00000C1A  98                cbw
00000C1B  8BD8              mov bx,ax
00000C1D  8A876408          mov al,[bx+0x864]
00000C21  B400              mov ah,0x0
00000C23  50                push ax
00000C24  B84900            mov ax,0x49
00000C27  50                push ax
00000C28  B88E00            mov ax,0x8e
00000C2B  50                push ax
00000C2C  8BC6              mov ax,si
00000C2E  40                inc ax
00000C2F  50                push ax
00000C30  8BC7              mov ax,di
00000C32  40                inc ax
00000C33  50                push ax
00000C34  9AD704960A        call 0xa96:0x4d7
00000C39  83C40A            add sp,byte +0xa
00000C3C  B80B00            mov ax,0xb
00000C3F  50                push ax
00000C40  B84600            mov ax,0x46
00000C43  50                push ax
00000C44  B82A00            mov ax,0x2a
00000C47  50                push ax
00000C48  8BC6              mov ax,si
00000C4A  050300            add ax,0x3
00000C4D  50                push ax
00000C4E  8BC7              mov ax,di
00000C50  050600            add ax,0x6
00000C53  50                push ax
00000C54  9AD704960A        call 0xa96:0x4d7
00000C59  83C40A            add sp,byte +0xa
00000C5C  B80100            mov ax,0x1
00000C5F  50                push ax
00000C60  B84600            mov ax,0x46
00000C63  50                push ax
00000C64  B82A00            mov ax,0x2a
00000C67  50                push ax
00000C68  8BC6              mov ax,si
00000C6A  40                inc ax
00000C6B  40                inc ax
00000C6C  50                push ax
00000C6D  8BC7              mov ax,di
00000C6F  050700            add ax,0x7
00000C72  50                push ax
00000C73  9A8104960A        call 0xa96:0x481
00000C78  83C40A            add sp,byte +0xa
00000C7B  8BC6              mov ax,si
00000C7D  050300            add ax,0x3
00000C80  50                push ax
00000C81  8BC7              mov ax,di
00000C83  050800            add ax,0x8
00000C86  50                push ax
00000C87  9A670E681C        call 0x1c68:0xe67
00000C8C  59                pop cx
00000C8D  59                pop cx
00000C8E  B80B00            mov ax,0xb
00000C91  50                push ax
00000C92  B82900            mov ax,0x29
00000C95  50                push ax
00000C96  B82A00            mov ax,0x2a
00000C99  50                push ax
00000C9A  8BC6              mov ax,si
00000C9C  050300            add ax,0x3
00000C9F  50                push ax
00000CA0  8BC7              mov ax,di
00000CA2  055E00            add ax,0x5e
00000CA5  50                push ax
00000CA6  9AD704960A        call 0xa96:0x4d7
00000CAB  83C40A            add sp,byte +0xa
00000CAE  B80100            mov ax,0x1
00000CB1  50                push ax
00000CB2  B82900            mov ax,0x29
00000CB5  50                push ax
00000CB6  B82A00            mov ax,0x2a
00000CB9  50                push ax
00000CBA  8BC6              mov ax,si
00000CBC  40                inc ax
00000CBD  40                inc ax
00000CBE  50                push ax
00000CBF  8BC7              mov ax,di
00000CC1  055F00            add ax,0x5f
00000CC4  50                push ax
00000CC5  9A8104960A        call 0xa96:0x481
00000CCA  83C40A            add sp,byte +0xa
00000CCD  B80B00            mov ax,0xb
00000CD0  50                push ax
00000CD1  B80700            mov ax,0x7
00000CD4  50                push ax
00000CD5  B82500            mov ax,0x25
00000CD8  50                push ax
00000CD9  8BC6              mov ax,si
00000CDB  054100            add ax,0x41
00000CDE  50                push ax
00000CDF  8BC7              mov ax,di
00000CE1  056100            add ax,0x61
00000CE4  50                push ax
00000CE5  9A8104960A        call 0xa96:0x481
00000CEA  83C40A            add sp,byte +0xa
00000CED  B80100            mov ax,0x1
00000CF0  50                push ax
00000CF1  B80700            mov ax,0x7
00000CF4  50                push ax
00000CF5  B82500            mov ax,0x25
00000CF8  50                push ax
00000CF9  8BC6              mov ax,si
00000CFB  054000            add ax,0x40
00000CFE  50                push ax
00000CFF  8BC7              mov ax,di
00000D01  056200            add ax,0x62
00000D04  50                push ax
00000D05  9AD704960A        call 0xa96:0x4d7
00000D0A  83C40A            add sp,byte +0xa
00000D0D  9AF60E681C        call 0x1c68:0xef6
00000D12  C746FE0000        mov word [bp-0x2],0x0
00000D17  EB4A              jmp short 0xd63
00000D19  8B46FE            mov ax,[bp-0x2]
00000D1C  BA1800            mov dx,0x18
00000D1F  F7EA              imul dx
00000D21  8BD6              mov dx,si
00000D23  03D0              add dx,ax
00000D25  83C207            add dx,byte +0x7
00000D28  52                push dx
00000D29  8BC7              mov ax,di
00000D2B  053500            add ax,0x35
00000D2E  50                push ax
00000D2F  9A8C0D681C        call 0x1c68:0xd8c
00000D34  59                pop cx
00000D35  59                pop cx
00000D36  FF76FE            push word [bp-0x2]
00000D39  8B46FE            mov ax,[bp-0x2]
00000D3C  BA1800            mov dx,0x18
00000D3F  F7EA              imul dx
00000D41  8BD6              mov dx,si
00000D43  03D0              add dx,ax
00000D45  83C206            add dx,byte +0x6
00000D48  52                push dx
00000D49  8BC7              mov ax,di
00000D4B  054F00            add ax,0x4f
00000D4E  50                push ax
00000D4F  9AEA0D681C        call 0x1c68:0xdea
00000D54  83C406            add sp,byte +0x6
00000D57  FF76FE            push word [bp-0x2]
00000D5A  9A7213681C        call 0x1c68:0x1372
00000D5F  59                pop cx
00000D60  FF46FE            inc word [bp-0x2]
00000D63  837EFE02          cmp word [bp-0x2],byte +0x2
00000D67  7EB0              jng 0xd19
00000D69  9AA806960A        call 0xa96:0x6a8
00000D6E  FE06AA37          inc byte [0x37aa]
00000D72  803EAA3704        cmp byte [0x37aa],0x4
00000D77  7D03              jnl 0xd7c
00000D79  E968FE            jmp 0xbe4
00000D7C  FF362E08          push word [0x82e]
00000D80  9A3F0C960A        call 0xa96:0xc3f
00000D85  59                pop cx
00000D86  5F                pop di
00000D87  5E                pop si
00000D88  8BE5              mov sp,bp
00000D8A  5D                pop bp
00000D8B  CB                retf

; ---- 1C68:0D8C ----
00000D8C  55                push bp
00000D8D  8BEC              mov bp,sp
00000D8F  56                push si
00000D90  33F6              xor si,si
00000D92  EB4E              jmp short 0xde2
00000D94  B80B00            mov ax,0xb
00000D97  50                push ax
00000D98  B80500            mov ax,0x5
00000D9B  50                push ax
00000D9C  B81A00            mov ax,0x1a
00000D9F  50                push ax
00000DA0  8BC6              mov ax,si
00000DA2  D1E0              shl ax,1
00000DA4  D1E0              shl ax,1
00000DA6  D1E0              shl ax,1
00000DA8  8B5608            mov dx,[bp+0x8]
00000DAB  03D0              add dx,ax
00000DAD  42                inc dx
00000DAE  52                push dx
00000DAF  8B4606            mov ax,[bp+0x6]
00000DB2  48                dec ax
00000DB3  50                push ax
00000DB4  9AD704960A        call 0xa96:0x4d7
00000DB9  83C40A            add sp,byte +0xa
00000DBC  B80100            mov ax,0x1
00000DBF  50                push ax
00000DC0  B80500            mov ax,0x5
00000DC3  50                push ax
00000DC4  B81A00            mov ax,0x1a
00000DC7  50                push ax
00000DC8  8BC6              mov ax,si
00000DCA  D1E0              shl ax,1
00000DCC  D1E0              shl ax,1
00000DCE  D1E0              shl ax,1
00000DD0  8B5608            mov dx,[bp+0x8]
00000DD3  03D0              add dx,ax
00000DD5  52                push dx
00000DD6  FF7606            push word [bp+0x6]
00000DD9  9A8104960A        call 0xa96:0x481
00000DDE  83C40A            add sp,byte +0xa
00000DE1  46                inc si
00000DE2  83FE01            cmp si,byte +0x1
00000DE5  7EAD              jng 0xd94
00000DE7  5E                pop si
00000DE8  5D                pop bp
00000DE9  CB                retf

; ---- 1C68:0DEA ----
00000DEA  55                push bp
00000DEB  8BEC              mov bp,sp
00000DED  56                push si
00000DEE  57                push di
00000DEF  8B7E06            mov di,[bp+0x6]
00000DF2  8B7608            mov si,[bp+0x8]
00000DF5  A0AA37            mov al,[0x37aa]
00000DF8  98                cbw
00000DF9  8BD8              mov bx,ax
00000DFB  8A876408          mov al,[bx+0x864]
00000DFF  B400              mov ah,0x0
00000E01  50                push ax
00000E02  B80F00            mov ax,0xf
00000E05  50                push ax
00000E06  B80F00            mov ax,0xf
00000E09  50                push ax
00000E0A  56                push si
00000E0B  57                push di
00000E0C  9AD704960A        call 0xa96:0x4d7
00000E11  83C40A            add sp,byte +0xa
00000E14  B80B00            mov ax,0xb
00000E17  50                push ax
00000E18  56                push si
00000E19  57                push di
00000E1A  B80700            mov ax,0x7
00000E1D  50                push ax
00000E1E  B80F00            mov ax,0xf
00000E21  50                push ax
00000E22  B84400            mov ax,0x44
00000E25  50                push ax
00000E26  8B460A            mov ax,[bp+0xa]
00000E29  B105              mov cl,0x5
00000E2B  D3E0              shl ax,cl
00000E2D  05D000            add ax,0xd0
00000E30  50                push ax
00000E31  9AAA01960A        call 0xa96:0x1aa
00000E36  83C40E            add sp,byte +0xe
00000E39  B80B00            mov ax,0xb
00000E3C  50                push ax
00000E3D  8BC6              mov ax,si
00000E3F  050800            add ax,0x8
00000E42  50                push ax
00000E43  57                push di
00000E44  B80700            mov ax,0x7
00000E47  50                push ax
00000E48  B80F00            mov ax,0xf
00000E4B  50                push ax
00000E4C  B84400            mov ax,0x44
00000E4F  50                push ax
00000E50  8B460A            mov ax,[bp+0xa]
00000E53  B105              mov cl,0x5
00000E55  D3E0              shl ax,cl
00000E57  05E000            add ax,0xe0
00000E5A  50                push ax
00000E5B  9AAA01960A        call 0xa96:0x1aa
00000E60  83C40E            add sp,byte +0xe
00000E63  5F                pop di
00000E64  5E                pop si
00000E65  5D                pop bp
00000E66  CB                retf

; ---- 1C68:0E67 ----
00000E67  55                push bp
00000E68  8BEC              mov bp,sp
00000E6A  56                push si
00000E6B  33F6              xor si,si
00000E6D  EB10              jmp short 0xe7f
00000E6F  56                push si
00000E70  FF7608            push word [bp+0x8]
00000E73  FF7606            push word [bp+0x6]
00000E76  9A870E681C        call 0x1c68:0xe87
00000E7B  83C406            add sp,byte +0x6
00000E7E  46                inc si
00000E7F  83FE02            cmp si,byte +0x2
00000E82  7EEB              jng 0xe6f
00000E84  5E                pop si
00000E85  5D                pop bp
00000E86  CB                retf

; ---- 1C68:0E87 ----
00000E87  55                push bp
00000E88  8BEC              mov bp,sp
00000E8A  56                push si
00000E8B  57                push di
00000E8C  8B760A            mov si,[bp+0xa]
00000E8F  56                push si
00000E90  0E                push cs
00000E91  E86EFC            call 0xb02
00000E94  59                pop cx
00000E95  8BF8              mov di,ax
00000E97  8A84A70E          mov al,[si+0xea7]
00000E9B  B400              mov ah,0x0
00000E9D  8B5608            mov dx,[bp+0x8]
00000EA0  03D0              add dx,ax
00000EA2  52                push dx
00000EA3  FF7606            push word [bp+0x6]
00000EA6  8A84AA0E          mov al,[si+0xeaa]
00000EAA  B400              mov ah,0x0
00000EAC  50                push ax
00000EAD  8A84820E          mov al,[si+0xe82]
00000EB1  B400              mov ah,0x0
00000EB3  50                push ax
00000EB4  8BDF              mov bx,di
00000EB6  D1E3              shl bx,1
00000EB8  FFB7740E          push word [bx+0xe74]
00000EBC  9AD602960A        call 0xa96:0x2d6
00000EC1  83C40A            add sp,byte +0xa
00000EC4  0BFF              or di,di
00000EC6  752A              jnz 0xef2
00000EC8  8A84A70E          mov al,[si+0xea7]
00000ECC  B400              mov ah,0x0
00000ECE  8B5608            mov dx,[bp+0x8]
00000ED1  03D0              add dx,ax
00000ED3  52                push dx
00000ED4  8B4606            mov ax,[bp+0x6]
00000ED7  052000            add ax,0x20
00000EDA  50                push ax
00000EDB  8A84AA0E          mov al,[si+0xeaa]
00000EDF  B400              mov ah,0x0
00000EE1  50                push ax
00000EE2  B8A000            mov ax,0xa0
00000EE5  50                push ax
00000EE6  B80001            mov ax,0x100
00000EE9  50                push ax
00000EEA  9A1103960A        call 0xa96:0x311
00000EEF  83C40A            add sp,byte +0xa
00000EF2  5F                pop di
00000EF3  5E                pop si
00000EF4  5D                pop bp
00000EF5  CB                retf

; ---- 1C68:0EF6 ----
00000EF6  55                push bp
00000EF7  8BEC              mov bp,sp
00000EF9  B80100            mov ax,0x1
00000EFC  50                push ax
00000EFD  B80B00            mov ax,0xb
00000F00  50                push ax
00000F01  B80500            mov ax,0x5
00000F04  50                push ax
00000F05  A0AA37            mov al,[0x37aa]
00000F08  98                cbw
00000F09  D1E0              shl ax,1
00000F0B  D1E0              shl ax,1
00000F0D  8BD8              mov bx,ax
00000F0F  FFB7F64B          push word [bx+0x4bf6]
00000F13  FFB7F44B          push word [bx+0x4bf4]
00000F17  A0AA37            mov al,[0x37aa]
00000F1A  98                cbw
00000F1B  D1E0              shl ax,1
00000F1D  8BD8              mov bx,ax
00000F1F  8B879008          mov ax,[bx+0x890]
00000F23  054500            add ax,0x45
00000F26  50                push ax
00000F27  A0AA37            mov al,[0x37aa]
00000F2A  98                cbw
00000F2B  D1E0              shl ax,1
00000F2D  8BD8              mov bx,ax
00000F2F  8B878808          mov ax,[bx+0x888]
00000F33  056300            add ax,0x63
00000F36  50                push ax
00000F37  9AFB05960A        call 0xa96:0x5fb
00000F3C  83C40E            add sp,byte +0xe
00000F3F  5D                pop bp
00000F40  CB                retf

; ---- 1C68:0F41 ----
00000F41  55                push bp
00000F42  8BEC              mov bp,sp
00000F44  A0AA37            mov al,[0x37aa]
00000F47  98                cbw
00000F48  8BD8              mov bx,ax
00000F4A  8A876408          mov al,[bx+0x864]
00000F4E  B400              mov ah,0x0
00000F50  50                push ax
00000F51  B80B00            mov ax,0xb
00000F54  50                push ax
00000F55  B80300            mov ax,0x3
00000F58  50                push ax
00000F59  A0AA37            mov al,[0x37aa]
00000F5C  98                cbw
00000F5D  B104              mov cl,0x4
00000F5F  D3E0              shl ax,cl
00000F61  50                push ax
00000F62  A0AA37            mov al,[0x37aa]
00000F65  98                cbw
00000F66  8BD8              mov bx,ax
00000F68  8A87C244          mov al,[bx+0x44c2]
00000F6C  98                cbw
00000F6D  D1E0              shl ax,1
00000F6F  5B                pop bx
00000F70  03D8              add bx,ax
00000F72  33C0              xor ax,ax
00000F74  50                push ax
00000F75  FFB7A448          push word [bx+0x48a4]
00000F79  A0AA37            mov al,[0x37aa]
00000F7C  98                cbw
00000F7D  D1E0              shl ax,1
00000F7F  8BD8              mov bx,ax
00000F81  8B879008          mov ax,[bx+0x890]
00000F85  053500            add ax,0x35
00000F88  50                push ax
00000F89  A0AA37            mov al,[0x37aa]
00000F8C  98                cbw
00000F8D  D1E0              shl ax,1
00000F8F  8BD8              mov bx,ax
00000F91  8B878808          mov ax,[bx+0x888]
00000F95  056800            add ax,0x68
00000F98  50                push ax
00000F99  9A7A05960A        call 0xa96:0x57a
00000F9E  83C40E            add sp,byte +0xe
00000FA1  5D                pop bp
00000FA2  CB                retf

; ---- 1C68:0FA3 ----
00000FA3  55                push bp
00000FA4  8BEC              mov bp,sp
00000FA6  A0AA37            mov al,[0x37aa]
00000FA9  98                cbw
00000FAA  8BD8              mov bx,ax
00000FAC  8A876408          mov al,[bx+0x864]
00000FB0  B400              mov ah,0x0
00000FB2  50                push ax
00000FB3  B80B00            mov ax,0xb
00000FB6  50                push ax
00000FB7  FF365A3B          push word [0x3b5a]
00000FBB  FF36583B          push word [0x3b58]
00000FBF  A0AA37            mov al,[0x37aa]
00000FC2  98                cbw
00000FC3  D1E0              shl ax,1
00000FC5  8BD8              mov bx,ax
00000FC7  8B879008          mov ax,[bx+0x890]
00000FCB  053500            add ax,0x35
00000FCE  50                push ax
00000FCF  A0AA37            mov al,[0x37aa]
00000FD2  98                cbw
00000FD3  D1E0              shl ax,1
00000FD5  8BD8              mov bx,ax
00000FD7  8B878808          mov ax,[bx+0x888]
00000FDB  056300            add ax,0x63
00000FDE  50                push ax
00000FDF  9A2505960A        call 0xa96:0x525
00000FE4  83C40C            add sp,byte +0xc
00000FE7  5D                pop bp
00000FE8  CB                retf

; ---- 1C68:0FE9 ----
00000FE9  55                push bp
00000FEA  8BEC              mov bp,sp
00000FEC  83EC04            sub sp,byte +0x4
00000FEF  56                push si
00000FF0  A0AA37            mov al,[0x37aa]
00000FF3  98                cbw
00000FF4  8BD8              mov bx,ax
00000FF6  8A87012F          mov al,[bx+0x2f01]
00000FFA  98                cbw
00000FFB  8BF0              mov si,ax
00000FFD  83FEFF            cmp si,byte -0x1
00001000  7507              jnz 0x1009
00001002  8CDA              mov dx,ds
00001004  B81B0F            mov ax,0xf1b
00001007  EB0E              jmp short 0x1017
00001009  8BDE              mov bx,si
0000100B  D1E3              shl bx,1
0000100D  D1E3              shl bx,1
0000100F  8B97AF0E          mov dx,[bx+0xeaf]
00001013  8B87AD0E          mov ax,[bx+0xead]
00001017  8946FC            mov [bp-0x4],ax
0000101A  8956FE            mov [bp-0x2],dx
0000101D  A0AA37            mov al,[0x37aa]
00001020  98                cbw
00001021  8BD8              mov bx,ax
00001023  8A876408          mov al,[bx+0x864]
00001027  B400              mov ah,0x0
00001029  50                push ax
0000102A  B80B00            mov ax,0xb
0000102D  50                push ax
0000102E  FF76FE            push word [bp-0x2]
00001031  FF76FC            push word [bp-0x4]
00001034  A0AA37            mov al,[0x37aa]
00001037  98                cbw
00001038  D1E0              shl ax,1
0000103A  8BD8              mov bx,ax
0000103C  8B879008          mov ax,[bx+0x890]
00001040  053500            add ax,0x35
00001043  50                push ax
00001044  A0AA37            mov al,[0x37aa]
00001047  98                cbw
00001048  D1E0              shl ax,1
0000104A  8BD8              mov bx,ax
0000104C  8B878808          mov ax,[bx+0x888]
00001050  056800            add ax,0x68
00001053  50                push ax
00001054  9A2505960A        call 0xa96:0x525
00001059  83C40C            add sp,byte +0xc
0000105C  5E                pop si
0000105D  8BE5              mov sp,bp
0000105F  5D                pop bp
00001060  CB                retf

; ---- 1C68:1061 ----
00001061  55                push bp
00001062  8BEC              mov bp,sp
00001064  A0AA37            mov al,[0x37aa]
00001067  98                cbw
00001068  8BD8              mov bx,ax
0000106A  8A876408          mov al,[bx+0x864]
0000106E  B400              mov ah,0x0
00001070  50                push ax
00001071  B81400            mov ax,0x14
00001074  50                push ax
00001075  B82E00            mov ax,0x2e
00001078  50                push ax
00001079  A0AA37            mov al,[0x37aa]
0000107C  98                cbw
0000107D  D1E0              shl ax,1
0000107F  8BD8              mov bx,ax
00001081  8B879008          mov ax,[bx+0x890]
00001085  052C00            add ax,0x2c
00001088  50                push ax
00001089  A0AA37            mov al,[0x37aa]
0000108C  98                cbw
0000108D  D1E0              shl ax,1
0000108F  8BD8              mov bx,ax
00001091  8B878808          mov ax,[bx+0x888]
00001095  055E00            add ax,0x5e
00001098  50                push ax
00001099  9AD704960A        call 0xa96:0x4d7
0000109E  83C40A            add sp,byte +0xa
000010A1  5D                pop bp
000010A2  CB                retf

; ---- 1C68:10A3 ----
000010A3  55                push bp
000010A4  8BEC              mov bp,sp
000010A6  B80B00            mov ax,0xb
000010A9  50                push ax
000010AA  B82700            mov ax,0x27
000010AD  50                push ax
000010AE  B82800            mov ax,0x28
000010B1  50                push ax
000010B2  A0AA37            mov al,[0x37aa]
000010B5  98                cbw
000010B6  D1E0              shl ax,1
000010B8  8BD8              mov bx,ax
000010BA  8B879008          mov ax,[bx+0x890]
000010BE  050300            add ax,0x3
000010C1  50                push ax
000010C2  A0AA37            mov al,[0x37aa]
000010C5  98                cbw
000010C6  D1E0              shl ax,1
000010C8  8BD8              mov bx,ax
000010CA  8B878808          mov ax,[bx+0x888]
000010CE  056000            add ax,0x60
000010D1  50                push ax
000010D2  9AD704960A        call 0xa96:0x4d7
000010D7  83C40A            add sp,byte +0xa
000010DA  5D                pop bp
000010DB  CB                retf

; ---- 1C68:10DC ----
000010DC  55                push bp
000010DD  8BEC              mov bp,sp
000010DF  83EC06            sub sp,byte +0x6
000010E2  56                push si
000010E3  57                push di
000010E4  A0AA37            mov al,[0x37aa]
000010E7  98                cbw
000010E8  8BD8              mov bx,ax
000010EA  8A87C244          mov al,[bx+0x44c2]
000010EE  8846FB            mov [bp-0x5],al
000010F1  A0AA37            mov al,[0x37aa]
000010F4  98                cbw
000010F5  D1E0              shl ax,1
000010F7  8BD8              mov bx,ax
000010F9  8BBF8808          mov di,[bx+0x888]
000010FD  A0AA37            mov al,[0x37aa]
00001100  98                cbw
00001101  D1E0              shl ax,1
00001103  8BD8              mov bx,ax
00001105  8BB79008          mov si,[bx+0x890]
00001109  807EFB0C          cmp byte [bp-0x5],0xc
0000110D  7203              jc 0x1112
0000110F  E98900            jmp 0x119b
00001112  807EFB00          cmp byte [bp-0x5],0x0
00001116  7506              jnz 0x111e
00001118  0E                push cs
00001119  E887FE            call 0xfa3
0000111C  EB10              jmp short 0x112e
0000111E  807EFB08          cmp byte [bp-0x5],0x8
00001122  7306              jnc 0x112a
00001124  0E                push cs
00001125  E819FE            call 0xf41
00001128  EB04              jmp short 0x112e
0000112A  0E                push cs
0000112B  E8BBFE            call 0xfe9
0000112E  8BC6              mov ax,si
00001130  050D00            add ax,0xd
00001133  50                push ax
00001134  8BC7              mov ax,di
00001136  056000            add ax,0x60
00001139  50                push ax
0000113A  B81800            mov ax,0x18
0000113D  50                push ax
0000113E  8A46FB            mov al,[bp-0x5]
00001141  B400              mov ah,0x0
00001143  8BD8              mov bx,ax
00001145  8A87D50E          mov al,[bx+0xed5]
00001149  B400              mov ah,0x0
0000114B  50                push ax
0000114C  8A46FB            mov al,[bp-0x5]
0000114F  B400              mov ah,0x0
00001151  D1E0              shl ax,1
00001153  8BD8              mov bx,ax
00001155  FFB7BD0E          push word [bx+0xebd]
00001159  9AAD03960A        call 0xa96:0x3ad
0000115E  83C40A            add sp,byte +0xa
00001161  8BC6              mov ax,si
00001163  050D00            add ax,0xd
00001166  50                push ax
00001167  8BC7              mov ax,di
00001169  058000            add ax,0x80
0000116C  50                push ax
0000116D  B81800            mov ax,0x18
00001170  50                push ax
00001171  8A46FB            mov al,[bp-0x5]
00001174  B400              mov ah,0x0
00001176  8BD8              mov bx,ax
00001178  8A87D50E          mov al,[bx+0xed5]
0000117C  B400              mov ah,0x0
0000117E  50                push ax
0000117F  8A46FB            mov al,[bp-0x5]
00001182  B400              mov ah,0x0
00001184  D1E0              shl ax,1
00001186  8BD8              mov bx,ax
00001188  8B87BD0E          mov ax,[bx+0xebd]
0000118C  052000            add ax,0x20
0000118F  50                push ax
00001190  9A1103960A        call 0xa96:0x311
00001195  83C40A            add sp,byte +0xa
00001198  E9F900            jmp 0x1294
0000119B  807EFB13          cmp byte [bp-0x5],0x13
0000119F  7304              jnc 0x11a5
000011A1  33C0              xor ax,ax
000011A3  EB0E              jmp short 0x11b3
000011A5  807EFB1A          cmp byte [bp-0x5],0x1a
000011A9  7305              jnc 0x11b0
000011AB  B80100            mov ax,0x1
000011AE  EB03              jmp short 0x11b3
000011B0  B80200            mov ax,0x2
000011B3  8946FC            mov [bp-0x4],ax
000011B6  8A46FB            mov al,[bp-0x5]
000011B9  B400              mov ah,0x0
000011BB  05F4FF            add ax,0xfff4
000011BE  BB0700            mov bx,0x7
000011C1  99                cwd
000011C2  F7FB              idiv bx
000011C4  8956FE            mov [bp-0x2],dx
000011C7  8BC6              mov ax,si
000011C9  050D00            add ax,0xd
000011CC  50                push ax
000011CD  8BC7              mov ax,di
000011CF  056000            add ax,0x60
000011D2  50                push ax
000011D3  B81800            mov ax,0x18
000011D6  50                push ax
000011D7  8B5EFC            mov bx,[bp-0x4]
000011DA  8A87820E          mov al,[bx+0xe82]
000011DE  B400              mov ah,0x0
000011E0  50                push ax
000011E1  8B5EFE            mov bx,[bp-0x2]
000011E4  D1E3              shl bx,1
000011E6  FFB7740E          push word [bx+0xe74]
000011EA  9AAD03960A        call 0xa96:0x3ad
000011EF  83C40A            add sp,byte +0xa
000011F2  837EFE00          cmp word [bp-0x2],byte +0x0
000011F6  7431              jz 0x1229
000011F8  8BC6              mov ax,si
000011FA  050D00            add ax,0xd
000011FD  50                push ax
000011FE  8BC7              mov ax,di
00001200  058000            add ax,0x80
00001203  50                push ax
00001204  B81800            mov ax,0x18
00001207  50                push ax
00001208  8B5EFC            mov bx,[bp-0x4]
0000120B  8A87820E          mov al,[bx+0xe82]
0000120F  B400              mov ah,0x0
00001211  50                push ax
00001212  8B5EFE            mov bx,[bp-0x2]
00001215  D1E3              shl bx,1
00001217  8B87740E          mov ax,[bx+0xe74]
0000121B  052000            add ax,0x20
0000121E  50                push ax
0000121F  9A1103960A        call 0xa96:0x311
00001224  83C40A            add sp,byte +0xa
00001227  EB20              jmp short 0x1249
00001229  8BC6              mov ax,si
0000122B  050D00            add ax,0xd
0000122E  50                push ax
0000122F  8BC7              mov ax,di
00001231  058000            add ax,0x80
00001234  50                push ax
00001235  B81800            mov ax,0x18
00001238  50                push ax
00001239  B8A000            mov ax,0xa0
0000123C  50                push ax
0000123D  B80001            mov ax,0x100
00001240  50                push ax
00001241  9A1103960A        call 0xa96:0x311
00001246  83C40A            add sp,byte +0xa
00001249  837EFC02          cmp word [bp-0x4],byte +0x2
0000124D  7520              jnz 0x126f
0000124F  B80B00            mov ax,0xb
00001252  50                push ax
00001253  B80400            mov ax,0x4
00001256  50                push ax
00001257  B82800            mov ax,0x28
0000125A  50                push ax
0000125B  8BC6              mov ax,si
0000125D  052100            add ax,0x21
00001260  50                push ax
00001261  8BC7              mov ax,di
00001263  056000            add ax,0x60
00001266  50                push ax
00001267  9AD704960A        call 0xa96:0x4d7
0000126C  83C40A            add sp,byte +0xa
0000126F  FF76FC            push word [bp-0x4]
00001272  8BC6              mov ax,si
00001274  052E00            add ax,0x2e
00001277  50                push ax
00001278  8BC7              mov ax,di
0000127A  057B00            add ax,0x7b
0000127D  50                push ax
0000127E  0E                push cs
0000127F  E868FB            call 0xdea
00001282  83C406            add sp,byte +0x6
00001285  8A46FB            mov al,[bp-0x5]
00001288  B400              mov ah,0x0
0000128A  05F4FF            add ax,0xfff4
0000128D  50                push ax
0000128E  9A2214681C        call 0x1c68:0x1422
00001293  59                pop cx
00001294  9A9F12681C        call 0x1c68:0x129f
00001299  5F                pop di
0000129A  5E                pop si
0000129B  8BE5              mov sp,bp
0000129D  5D                pop bp
0000129E  CB                retf

; ---- 1C68:129F ----
0000129F  55                push bp
000012A0  8BEC              mov bp,sp
000012A2  83EC04            sub sp,byte +0x4
000012A5  56                push si
000012A6  57                push di
000012A7  A0AA37            mov al,[0x37aa]
000012AA  98                cbw
000012AB  D1E0              shl ax,1
000012AD  D1E0              shl ax,1
000012AF  8BD8              mov bx,ax
000012B1  8B87BE3C          mov ax,[bx+0x3cbe]
000012B5  8B97BC3C          mov dx,[bx+0x3cbc]
000012B9  8956FC            mov [bp-0x4],dx
000012BC  8946FE            mov [bp-0x2],ax
000012BF  8B46FC            mov ax,[bp-0x4]
000012C2  8BF0              mov si,ax
000012C4  0BC0              or ax,ax
000012C6  7D06              jnl 0x12ce
000012C8  8BC6              mov ax,si
000012CA  F7D8              neg ax
000012CC  8BF0              mov si,ax
000012CE  A0AA37            mov al,[0x37aa]
000012D1  98                cbw
000012D2  D1E0              shl ax,1
000012D4  D1E0              shl ax,1
000012D6  8BD8              mov bx,ax
000012D8  8B87F64B          mov ax,[bx+0x4bf6]
000012DC  8B97F44B          mov dx,[bx+0x4bf4]
000012E0  3B46FE            cmp ax,[bp-0x2]
000012E3  7C0C              jl 0x12f1
000012E5  7505              jnz 0x12ec
000012E7  3B56FC            cmp dx,[bp-0x4]
000012EA  7205              jc 0x12f1
000012EC  B80600            mov ax,0x6
000012EF  EB03              jmp short 0x12f4
000012F1  B80800            mov ax,0x8
000012F4  8BF8              mov di,ax
000012F6  B80B00            mov ax,0xb
000012F9  50                push ax
000012FA  57                push di
000012FB  B80500            mov ax,0x5
000012FE  50                push ax
000012FF  8BC6              mov ax,si
00001301  99                cwd
00001302  52                push dx
00001303  50                push ax
00001304  A0AA37            mov al,[0x37aa]
00001307  98                cbw
00001308  D1E0              shl ax,1
0000130A  8BD8              mov bx,ax
0000130C  8B879008          mov ax,[bx+0x890]
00001310  050800            add ax,0x8
00001313  50                push ax
00001314  A0AA37            mov al,[0x37aa]
00001317  98                cbw
00001318  D1E0              shl ax,1
0000131A  8BD8              mov bx,ax
0000131C  8B878808          mov ax,[bx+0x888]
00001320  056400            add ax,0x64
00001323  50                push ax
00001324  9AFB05960A        call 0xa96:0x5fb
00001329  83C40E            add sp,byte +0xe
0000132C  837EFE00          cmp word [bp-0x2],byte +0x0
00001330  7F3A              jg 0x136c
00001332  7C06              jl 0x133a
00001334  837EFC00          cmp word [bp-0x4],byte +0x0
00001338  7332              jnc 0x136c
0000133A  B80B00            mov ax,0xb
0000133D  50                push ax
0000133E  57                push di
0000133F  1E                push ds
00001340  B81F0F            mov ax,0xf1f
00001343  50                push ax
00001344  A0AA37            mov al,[0x37aa]
00001347  98                cbw
00001348  D1E0              shl ax,1
0000134A  8BD8              mov bx,ax
0000134C  8B879008          mov ax,[bx+0x890]
00001350  050800            add ax,0x8
00001353  50                push ax
00001354  A0AA37            mov al,[0x37aa]
00001357  98                cbw
00001358  D1E0              shl ax,1
0000135A  8BD8              mov bx,ax
0000135C  8B878808          mov ax,[bx+0x888]
00001360  056A00            add ax,0x6a
00001363  50                push ax
00001364  9A2505960A        call 0xa96:0x525
00001369  83C40C            add sp,byte +0xc
0000136C  5F                pop di
0000136D  5E                pop si
0000136E  8BE5              mov sp,bp
00001370  5D                pop bp
00001371  CB                retf

; ---- 1C68:1372 ----
00001372  55                push bp
00001373  8BEC              mov bp,sp
00001375  83EC04            sub sp,byte +0x4
00001378  56                push si
00001379  57                push di
0000137A  A0AA37            mov al,[0x37aa]
0000137D  98                cbw
0000137E  D1E0              shl ax,1
00001380  8BD8              mov bx,ax
00001382  8B878808          mov ax,[bx+0x888]
00001386  053600            add ax,0x36
00001389  8946FE            mov [bp-0x2],ax
0000138C  A0AA37            mov al,[0x37aa]
0000138F  98                cbw
00001390  D1E0              shl ax,1
00001392  8BD8              mov bx,ax
00001394  8B879008          mov ax,[bx+0x890]
00001398  50                push ax
00001399  8B4606            mov ax,[bp+0x6]
0000139C  BA1800            mov dx,0x18
0000139F  F7EA              imul dx
000013A1  5A                pop dx
000013A2  03D0              add dx,ax
000013A4  83C208            add dx,byte +0x8
000013A7  8BFA              mov di,dx
000013A9  33F6              xor si,si
000013AB  EB6A              jmp short 0x1417
000013AD  8B5E06            mov bx,[bp+0x6]
000013B0  D1E3              shl bx,1
000013B2  03DE              add bx,si
000013B4  A0AA37            mov al,[0x37aa]
000013B7  98                cbw
000013B8  BA0600            mov dx,0x6
000013BB  F7EA              imul dx
000013BD  03D8              add bx,ax
000013BF  8A87CE43          mov al,[bx+0x43ce]
000013C3  B400              mov ah,0x0
000013C5  BB0A00            mov bx,0xa
000013C8  99                cwd
000013C9  F7FB              idiv bx
000013CB  D1E0              shl ax,1
000013CD  D1E0              shl ax,1
000013CF  8946FC            mov [bp-0x4],ax
000013D2  B80100            mov ax,0x1
000013D5  50                push ax
000013D6  B80300            mov ax,0x3
000013D9  50                push ax
000013DA  B81800            mov ax,0x18
000013DD  50                push ax
000013DE  57                push di
000013DF  FF76FE            push word [bp-0x2]
000013E2  9AD704960A        call 0xa96:0x4d7
000013E7  83C40A            add sp,byte +0xa
000013EA  837EFC00          cmp word [bp-0x4],byte +0x0
000013EE  7E1F              jng 0x140f
000013F0  8B5E06            mov bx,[bp+0x6]
000013F3  D1E3              shl bx,1
000013F5  8A806C0E          mov al,[bx+si+0xe6c]
000013F9  B400              mov ah,0x0
000013FB  50                push ax
000013FC  B80300            mov ax,0x3
000013FF  50                push ax
00001400  FF76FC            push word [bp-0x4]
00001403  57                push di
00001404  FF76FE            push word [bp-0x2]
00001407  9AD704960A        call 0xa96:0x4d7
0000140C  83C40A            add sp,byte +0xa
0000140F  8BC7              mov ax,di
00001411  050800            add ax,0x8
00001414  8BF8              mov di,ax
00001416  46                inc si
00001417  83FE02            cmp si,byte +0x2
0000141A  7C91              jl 0x13ad
0000141C  5F                pop di
0000141D  5E                pop si
0000141E  8BE5              mov sp,bp
00001420  5D                pop bp
00001421  CB                retf

; ---- 1C68:1422 ----
00001422  55                push bp
00001423  8BEC              mov bp,sp
00001425  83EC04            sub sp,byte +0x4
00001428  56                push si
00001429  57                push di
0000142A  A0AA37            mov al,[0x37aa]
0000142D  98                cbw
0000142E  D1E0              shl ax,1
00001430  8BD8              mov bx,ax
00001432  8B878808          mov ax,[bx+0x888]
00001436  056200            add ax,0x62
00001439  8946FE            mov [bp-0x2],ax
0000143C  A0AA37            mov al,[0x37aa]
0000143F  98                cbw
00001440  D1E0              shl ax,1
00001442  8BD8              mov bx,ax
00001444  8B879008          mov ax,[bx+0x890]
00001448  053000            add ax,0x30
0000144B  8BF8              mov di,ax
0000144D  8B4606            mov ax,[bp+0x6]
00001450  BB0700            mov bx,0x7
00001453  99                cwd
00001454  F7FB              idiv bx
00001456  D1E2              shl dx,1
00001458  D1E2              shl dx,1
0000145A  8956FC            mov [bp-0x4],dx
0000145D  33F6              xor si,si
0000145F  EB4D              jmp short 0x14ae
00001461  B80100            mov ax,0x1
00001464  50                push ax
00001465  B80300            mov ax,0x3
00001468  50                push ax
00001469  B81800            mov ax,0x18
0000146C  50                push ax
0000146D  57                push di
0000146E  FF76FE            push word [bp-0x2]
00001471  9AD704960A        call 0xa96:0x4d7
00001476  83C40A            add sp,byte +0xa
00001479  837EFC00          cmp word [bp-0x4],byte +0x0
0000147D  7E27              jng 0x14a6
0000147F  8B4606            mov ax,[bp+0x6]
00001482  BB0700            mov bx,0x7
00001485  99                cwd
00001486  F7FB              idiv bx
00001488  D1E0              shl ax,1
0000148A  8BD8              mov bx,ax
0000148C  8A806C0E          mov al,[bx+si+0xe6c]
00001490  B400              mov ah,0x0
00001492  50                push ax
00001493  B80300            mov ax,0x3
00001496  50                push ax
00001497  FF76FC            push word [bp-0x4]
0000149A  57                push di
0000149B  FF76FE            push word [bp-0x2]
0000149E  9AD704960A        call 0xa96:0x4d7
000014A3  83C40A            add sp,byte +0xa
000014A6  8BC7              mov ax,di
000014A8  050800            add ax,0x8
000014AB  8BF8              mov di,ax
000014AD  46                inc si
000014AE  83FE02            cmp si,byte +0x2
000014B1  7CAE              jl 0x1461
000014B3  5F                pop di
000014B4  5E                pop si
000014B5  8BE5              mov sp,bp
000014B7  5D                pop bp
000014B8  CB                retf

; ---- 1C68:14B9 ----
000014B9  55                push bp
000014BA  8BEC              mov bp,sp
000014BC  9AD714681C        call 0x1c68:0x14d7
000014C1  9A2E15681C        call 0x1c68:0x152e
000014C6  9ABB15681C        call 0x1c68:0x15bb
000014CB  9A1716681C        call 0x1c68:0x1617
000014D0  9AF416681C        call 0x1c68:0x16f4
000014D5  5D                pop bp
000014D6  CB                retf

; ---- 1C68:14D7 ----
000014D7  55                push bp
000014D8  8BEC              mov bp,sp
000014DA  A07054            mov al,[0x5470]
000014DD  FEC8              dec al
000014DF  A27054            mov [0x5470],al
000014E2  3C0F              cmp al,0xf
000014E4  751E              jnz 0x1504
000014E6  B86300            mov ax,0x63
000014E9  50                push ax
000014EA  B86800            mov ax,0x68
000014ED  50                push ax
000014EE  B80500            mov ax,0x5
000014F1  50                push ax
000014F2  B82800            mov ax,0x28
000014F5  50                push ax
000014F6  B81001            mov ax,0x110
000014F9  50                push ax
000014FA  9A1103960A        call 0xa96:0x311
000014FF  83C40A            add sp,byte +0xa
00001502  EB28              jmp short 0x152c
00001504  803E705400        cmp byte [0x5470],0x0
00001509  7F21              jg 0x152c
0000150B  B86300            mov ax,0x63
0000150E  50                push ax
0000150F  B86800            mov ax,0x68
00001512  50                push ax
00001513  B80500            mov ax,0x5
00001516  50                push ax
00001517  B8E800            mov ax,0xe8
0000151A  50                push ax
0000151B  B86800            mov ax,0x68
0000151E  50                push ax
0000151F  9A1103960A        call 0xa96:0x311
00001524  83C40A            add sp,byte +0xa
00001527  C60670541E        mov byte [0x5470],0x1e
0000152C  5D                pop bp
0000152D  CB                retf

; ---- 1C68:152E ----
0000152E  55                push bp
0000152F  8BEC              mov bp,sp
00001531  A07154            mov al,[0x5471]
00001534  FEC8              dec al
00001536  A27154            mov [0x5471],al
00001539  0AC0              or al,al
0000153B  7F7C              jg 0x15b9
0000153D  803E725417        cmp byte [0x5472],0x17
00001542  7F26              jg 0x156a
00001544  A07254            mov al,[0x5472]
00001547  98                cbw
00001548  054B00            add ax,0x4b
0000154B  50                push ax
0000154C  B85000            mov ax,0x50
0000154F  50                push ax
00001550  B80100            mov ax,0x1
00001553  50                push ax
00001554  A07254            mov al,[0x5472]
00001557  98                cbw
00001558  058000            add ax,0x80
0000155B  50                push ax
0000155C  B83000            mov ax,0x30
0000155F  50                push ax
00001560  9A5F03960A        call 0xa96:0x35f
00001565  83C40A            add sp,byte +0xa
00001568  EB24              jmp short 0x158e
0000156A  A07254            mov al,[0x5472]
0000156D  98                cbw
0000156E  053300            add ax,0x33
00001571  50                push ax
00001572  B85000            mov ax,0x50
00001575  50                push ax
00001576  B80100            mov ax,0x1
00001579  50                push ax
0000157A  A07254            mov al,[0x5472]
0000157D  98                cbw
0000157E  05B800            add ax,0xb8
00001581  50                push ax
00001582  B85000            mov ax,0x50
00001585  50                push ax
00001586  9A5F03960A        call 0xa96:0x35f
0000158B  83C40A            add sp,byte +0xa
0000158E  A07254            mov al,[0x5472]
00001591  FEC0              inc al
00001593  A27254            mov [0x5472],al
00001596  3C2F              cmp al,0x2f
00001598  7E05              jng 0x159f
0000159A  C606725400        mov byte [0x5472],0x0
0000159F  803E725400        cmp byte [0x5472],0x0
000015A4  7407              jz 0x15ad
000015A6  803E725417        cmp byte [0x5472],0x17
000015AB  7507              jnz 0x15b4
000015AD  C606715433        mov byte [0x5471],0x33
000015B2  EB05              jmp short 0x15b9
000015B4  C606715403        mov byte [0x5471],0x3
000015B9  5D                pop bp
000015BA  CB                retf

; ---- 1C68:15BB ----
000015BB  55                push bp
000015BC  8BEC              mov bp,sp
000015BE  A07354            mov al,[0x5473]
000015C1  FEC8              dec al
000015C3  A27354            mov [0x5473],al
000015C6  0AC0              or al,al
000015C8  7F4B              jg 0x1615
000015CA  B84E00            mov ax,0x4e
000015CD  50                push ax
000015CE  A07454            mov al,[0x5474]
000015D1  98                cbw
000015D2  D1E0              shl ax,1
000015D4  8BD8              mov bx,ax
000015D6  FFB7ED0E          push word [bx+0xeed]
000015DA  B80B00            mov ax,0xb
000015DD  50                push ax
000015DE  A07454            mov al,[0x5474]
000015E1  98                cbw
000015E2  8BD8              mov bx,ax
000015E4  8A87E90E          mov al,[bx+0xee9]
000015E8  B400              mov ah,0x0
000015EA  50                push ax
000015EB  A07454            mov al,[0x5474]
000015EE  98                cbw
000015EF  D1E0              shl ax,1
000015F1  8BD8              mov bx,ax
000015F3  FFB7E10E          push word [bx+0xee1]
000015F7  9A5F03960A        call 0xa96:0x35f
000015FC  83C40A            add sp,byte +0xa
000015FF  A07454            mov al,[0x5474]
00001602  FEC0              inc al
00001604  A27454            mov [0x5474],al
00001607  3C03              cmp al,0x3
00001609  7E05              jng 0x1610
0000160B  C606745400        mov byte [0x5474],0x0
00001610  C606735429        mov byte [0x5473],0x29
00001615  5D                pop bp
00001616  CB                retf

; ---- 1C68:1617 ----
00001617  55                push bp
00001618  8BEC              mov bp,sp
0000161A  4C                dec sp
0000161B  4C                dec sp
0000161C  A07554            mov al,[0x5475]
0000161F  FEC8              dec al
00001621  A27554            mov [0x5475],al
00001624  0AC0              or al,al
00001626  7E03              jng 0x162b
00001628  E9C500            jmp 0x16f0
0000162B  803E765407        cmp byte [0x5476],0x7
00001630  7F4D              jg 0x167f
00001632  A07654            mov al,[0x5476]
00001635  98                cbw
00001636  8BD8              mov bx,ax
00001638  8A87F50E          mov al,[bx+0xef5]
0000163C  8846FF            mov [bp-0x1],al
0000163F  B85500            mov ax,0x55
00001642  50                push ax
00001643  8A46FF            mov al,[bp-0x1]
00001646  98                cbw
00001647  8BD8              mov bx,ax
00001649  8A87FC0E          mov al,[bx+0xefc]
0000164D  98                cbw
0000164E  05B800            add ax,0xb8
00001651  50                push ax
00001652  B80800            mov ax,0x8
00001655  50                push ax
00001656  8A46FF            mov al,[bp-0x1]
00001659  98                cbw
0000165A  8BD8              mov bx,ax
0000165C  8A87030F          mov al,[bx+0xf03]
00001660  98                cbw
00001661  50                push ax
00001662  B82800            mov ax,0x28
00001665  50                push ax
00001666  8A46FF            mov al,[bp-0x1]
00001669  98                cbw
0000166A  8BD8              mov bx,ax
0000166C  8A87FC0E          mov al,[bx+0xefc]
00001670  98                cbw
00001671  05E000            add ax,0xe0
00001674  50                push ax
00001675  9A3000960A        call 0xa96:0x30
0000167A  83C40C            add sp,byte +0xc
0000167D  EB46              jmp short 0x16c5
0000167F  A07654            mov al,[0x5476]
00001682  04F8              add al,0xf8
00001684  8846FF            mov [bp-0x1],al
00001687  B85500            mov ax,0x55
0000168A  50                push ax
0000168B  8A46FF            mov al,[bp-0x1]
0000168E  98                cbw
0000168F  8BD8              mov bx,ax
00001691  8A87FC0E          mov al,[bx+0xefc]
00001695  98                cbw
00001696  05B800            add ax,0xb8
00001699  50                push ax
0000169A  B80800            mov ax,0x8
0000169D  50                push ax
0000169E  8A46FF            mov al,[bp-0x1]
000016A1  98                cbw
000016A2  8BD8              mov bx,ax
000016A4  8A87030F          mov al,[bx+0xf03]
000016A8  98                cbw
000016A9  50                push ax
000016AA  B8DA00            mov ax,0xda
000016AD  50                push ax
000016AE  8A46FF            mov al,[bp-0x1]
000016B1  98                cbw
000016B2  8BD8              mov bx,ax
000016B4  8A87FC0E          mov al,[bx+0xefc]
000016B8  98                cbw
000016B9  05B800            add ax,0xb8
000016BC  50                push ax
000016BD  9A3000960A        call 0xa96:0x30
000016C2  83C40C            add sp,byte +0xc
000016C5  A07654            mov al,[0x5476]
000016C8  FEC0              inc al
000016CA  A27654            mov [0x5476],al
000016CD  3C0F              cmp al,0xf
000016CF  7E05              jng 0x16d6
000016D1  C606765400        mov byte [0x5476],0x0
000016D6  803E765407        cmp byte [0x5476],0x7
000016DB  7407              jz 0x16e4
000016DD  803E765400        cmp byte [0x5476],0x0
000016E2  7507              jnz 0x16eb
000016E4  C606755467        mov byte [0x5475],0x67
000016E9  EB05              jmp short 0x16f0
000016EB  C606755410        mov byte [0x5475],0x10
000016F0  8BE5              mov sp,bp
000016F2  5D                pop bp
000016F3  CB                retf

; ---- 1C68:16F4 ----
000016F4  55                push bp
000016F5  8BEC              mov bp,sp
000016F7  56                push si
000016F8  A07754            mov al,[0x5477]
000016FB  FEC8              dec al
000016FD  A27754            mov [0x5477],al
00001700  0AC0              or al,al
00001702  7F78              jg 0x177c
00001704  A17854            mov ax,[0x5478]
00001707  BB0800            mov bx,0x8
0000170A  99                cwd
0000170B  F7FB              idiv bx
0000170D  B105              mov cl,0x5
0000170F  D3E0              shl ax,cl
00001711  8BF0              mov si,ax
00001713  F70678540100      test word [0x5478],0x1
00001719  7526              jnz 0x1741
0000171B  B84D00            mov ax,0x4d
0000171E  50                push ax
0000171F  8BC6              mov ax,si
00001721  051000            add ax,0x10
00001724  50                push ax
00001725  B80700            mov ax,0x7
00001728  50                push ax
00001729  B82000            mov ax,0x20
0000172C  50                push ax
0000172D  B83000            mov ax,0x30
00001730  50                push ax
00001731  8BC6              mov ax,si
00001733  05E000            add ax,0xe0
00001736  50                push ax
00001737  9A3000960A        call 0xa96:0x30
0000173C  83C40C            add sp,byte +0xc
0000173F  EB24              jmp short 0x1765
00001741  B84D00            mov ax,0x4d
00001744  50                push ax
00001745  8BC6              mov ax,si
00001747  051000            add ax,0x10
0000174A  50                push ax
0000174B  B80700            mov ax,0x7
0000174E  50                push ax
0000174F  B82000            mov ax,0x20
00001752  50                push ax
00001753  B8D200            mov ax,0xd2
00001756  50                push ax
00001757  8BC6              mov ax,si
00001759  051000            add ax,0x10
0000175C  50                push ax
0000175D  9A3000960A        call 0xa96:0x30
00001762  83C40C            add sp,byte +0xc
00001765  A17854            mov ax,[0x5478]
00001768  40                inc ax
00001769  A37854            mov [0x5478],ax
0000176C  3D0F00            cmp ax,0xf
0000176F  7E06              jng 0x1777
00001771  C70678540000      mov word [0x5478],0x0
00001777  C606775415        mov byte [0x5477],0x15
0000177C  5E                pop si
0000177D  5D                pop bp
0000177E  CB                retf

; ---- 1C68:177F ----
0000177F  55                push bp
00001780  8BEC              mov bp,sp
00001782  C606AA3700        mov byte [0x37aa],0x0
00001787  EB6C              jmp short 0x17f5
00001789  A0AA37            mov al,[0x37aa]
0000178C  98                cbw
0000178D  BA1A00            mov dx,0x1a
00001790  F7EA              imul dx
00001792  8BD8              mov bx,ax
00001794  C7870E330B00      mov word [bx+0x330e],0xb
0000179A  B8F500            mov ax,0xf5
0000179D  50                push ax
0000179E  A0AA37            mov al,[0x37aa]
000017A1  98                cbw
000017A2  B105              mov cl,0x5
000017A4  D3E0              shl ax,cl
000017A6  058000            add ax,0x80
000017A9  50                push ax
000017AA  B8F500            mov ax,0xf5
000017AD  50                push ax
000017AE  A0AA37            mov al,[0x37aa]
000017B1  98                cbw
000017B2  B105              mov cl,0x5
000017B4  D3E0              shl ax,cl
000017B6  50                push ax
000017B7  A0AA37            mov al,[0x37aa]
000017BA  98                cbw
000017BB  50                push ax
000017BC  9A2B02AF21        call 0x21af:0x22b
000017C1  83C40A            add sp,byte +0xa
000017C4  B85100            mov ax,0x51
000017C7  50                push ax
000017C8  A0AA37            mov al,[0x37aa]
000017CB  98                cbw
000017CC  B104              mov cl,0x4
000017CE  D3E0              shl ax,cl
000017D0  50                push ax
000017D1  B81200            mov ax,0x12
000017D4  50                push ax
000017D5  A0AA37            mov al,[0x37aa]
000017D8  98                cbw
000017D9  B104              mov cl,0x4
000017DB  D3E0              shl ax,cl
000017DD  059000            add ax,0x90
000017E0  50                push ax
000017E1  33C0              xor ax,ax
000017E3  50                push ax
000017E4  A0AA37            mov al,[0x37aa]
000017E7  98                cbw
000017E8  50                push ax
000017E9  9ACA00AF21        call 0x21af:0xca
000017EE  83C40C            add sp,byte +0xc
000017F1  FE06AA37          inc byte [0x37aa]
000017F5  803EAA3704        cmp byte [0x37aa],0x4
000017FA  7C8D              jl 0x1789
000017FC  5D                pop bp
000017FD  CB                retf

; ---- 1C68:17FE ----
000017FE  55                push bp
000017FF  8BEC              mov bp,sp
00001801  C606AA3700        mov byte [0x37aa],0x0
00001806  EB13              jmp short 0x181b
00001808  33C0              xor ax,ax
0000180A  50                push ax
0000180B  A0AA37            mov al,[0x37aa]
0000180E  98                cbw
0000180F  50                push ax
00001810  9A1204AF21        call 0x21af:0x412
00001815  59                pop cx
00001816  59                pop cx
00001817  FE06AA37          inc byte [0x37aa]
0000181B  803EAA3704        cmp byte [0x37aa],0x4
00001820  7CE6              jl 0x1808
00001822  5D                pop bp
00001823  CB                retf

; ---- 1C68:1824 ----
00001824  55                push bp
00001825  8BEC              mov bp,sp
00001827  803E3E4400        cmp byte [0x443e],0x0
0000182C  7515              jnz 0x1843
0000182E  803E3F4400        cmp byte [0x443f],0x0
00001833  750E              jnz 0x1843
00001835  803E404400        cmp byte [0x4440],0x0
0000183A  7507              jnz 0x1843
0000183C  803E414400        cmp byte [0x4441],0x0
00001841  741D              jz 0x1860
00001843  803E0A0F00        cmp byte [0xf0a],0x0
00001848  7514              jnz 0x185e
0000184A  B83700            mov ax,0x37
0000184D  50                push ax
0000184E  B80200            mov ax,0x2
00001851  50                push ax
00001852  9A5400A721        call 0x21a7:0x54
00001857  59                pop cx
00001858  59                pop cx
00001859  C6060A0F01        mov byte [0xf0a],0x1
0000185E  EB1A              jmp short 0x187a
00001860  803E0A0F01        cmp byte [0xf0a],0x1
00001865  7513              jnz 0x187a
00001867  33C0              xor ax,ax
00001869  50                push ax
0000186A  B80200            mov ax,0x2
0000186D  50                push ax
0000186E  9A                db 0x9a
0000186F  54                push sp
