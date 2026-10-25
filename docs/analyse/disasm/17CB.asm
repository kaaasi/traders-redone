; segment 17CB  file offset 0x1B2B0  size 0xA10
00000000  C0EB00            shr bl,byte 0x0
00000003  5D                pop bp
00000004  CB                retf

; ---- 17CB:0005 ----
00000005  55                push bp
00000006  8BEC              mov bp,sp
00000008  9ACB062508        call 0x825:0x6cb
0000000D  9AD704CB17        call 0x17cb:0x4d7
00000012  9A1E00CB17        call 0x17cb:0x1e
00000017  9A3607CB17        call 0x17cb:0x736
0000001C  5D                pop bp
0000001D  CB                retf

; ---- 17CB:001E ----
0000001E  55                push bp
0000001F  8BEC              mov bp,sp
00000021  83EC0C            sub sp,byte +0xc
00000024  56                push si
00000025  B80300            mov ax,0x3
00000028  50                push ax
00000029  9ABA01FD21        call 0x21fd:0x1ba
0000002E  59                pop cx
0000002F  A2AA37            mov [0x37aa],al
00000032  A0AA37            mov al,[0x37aa]
00000035  98                cbw
00000036  8BD8              mov bx,ax
00000038  80BF2A3002        cmp byte [bx+0x302a],0x2
0000003D  7303              jnc 0x42
0000003F  E9D600            jmp 0x118
00000042  33F6              xor si,si
00000044  EB1C              jmp short 0x62
00000046  8A842A30          mov al,[si+0x302a]
0000004A  B400              mov ah,0x0
0000004C  50                push ax
0000004D  A0AA37            mov al,[0x37aa]
00000050  98                cbw
00000051  8BD8              mov bx,ax
00000053  8A872A30          mov al,[bx+0x302a]
00000057  B400              mov ah,0x0
00000059  48                dec ax
0000005A  5A                pop dx
0000005B  3BD0              cmp dx,ax
0000005D  7502              jnz 0x61
0000005F  EB06              jmp short 0x67
00000061  46                inc si
00000062  83FE03            cmp si,byte +0x3
00000065  7EDF              jng 0x46
00000067  33C0              xor ax,ax
00000069  BA0400            mov dx,0x4
0000006C  50                push ax
0000006D  52                push dx
0000006E  8BDE              mov bx,si
00000070  D1E3              shl bx,1
00000072  D1E3              shl bx,1
00000074  8B8F082F          mov cx,[bx+0x2f08]
00000078  8B87062F          mov ax,[bx+0x2f06]
0000007C  51                push cx
0000007D  50                push ax
0000007E  A0AA37            mov al,[0x37aa]
00000081  98                cbw
00000082  D1E0              shl ax,1
00000084  D1E0              shl ax,1
00000086  8BD8              mov bx,ax
00000088  58                pop ax
00000089  2B87062F          sub ax,[bx+0x2f06]
0000008D  59                pop cx
0000008E  1B8F082F          sbb cx,[bx+0x2f08]
00000092  33D2              xor dx,dx
00000094  50                push ax
00000095  B80300            mov ax,0x3
00000098  5B                pop bx
00000099  9A4E4F0000        call 0x0:0x4f4e
0000009E  52                push dx
0000009F  50                push ax
000000A0  9A894F0000        call 0x0:0x4f89
000000A5  8946FC            mov [bp-0x4],ax
000000A8  8956FE            mov [bp-0x2],dx
000000AB  A0AA37            mov al,[0x37aa]
000000AE  98                cbw
000000AF  D1E0              shl ax,1
000000B1  D1E0              shl ax,1
000000B3  8BD8              mov bx,ax
000000B5  8B87082F          mov ax,[bx+0x2f08]
000000B9  8B97062F          mov dx,[bx+0x2f06]
000000BD  D1E2              shl dx,1
000000BF  D1D0              rcl ax,1
000000C1  8BDE              mov bx,si
000000C3  D1E3              shl bx,1
000000C5  D1E3              shl bx,1
000000C7  3B87082F          cmp ax,[bx+0x2f08]
000000CB  7C16              jl 0xe3
000000CD  7F06              jg 0xd5
000000CF  3B97062F          cmp dx,[bx+0x2f06]
000000D3  760E              jna 0xe3
000000D5  B80900            mov ax,0x9
000000D8  50                push ax
000000D9  9ABA01FD21        call 0x21fd:0x1ba
000000DE  59                pop cx
000000DF  0BC0              or ax,ax
000000E1  7532              jnz 0x115
000000E3  837EFE00          cmp word [bp-0x2],byte +0x0
000000E7  7C2C              jl 0x115
000000E9  7F06              jg 0xf1
000000EB  837EFC00          cmp word [bp-0x4],byte +0x0
000000EF  7624              jna 0x115
000000F1  9A8609CB17        call 0x17cb:0x986
000000F6  FF76FE            push word [bp-0x2]
000000F9  FF76FC            push word [bp-0x4]
000000FC  9ABD02CB17        call 0x17cb:0x2bd
00000101  59                pop cx
00000102  59                pop cx
00000103  33C0              xor ax,ax
00000105  50                push ax
00000106  9A0400FD21        call 0x21fd:0x4
0000010B  59                pop cx
0000010C  33C0              xor ax,ax
0000010E  50                push ax
0000010F  9ADB05FD21        call 0x21fd:0x5db
00000114  59                pop cx
00000115  E9A001            jmp 0x2b8
00000118  33F6              xor si,si
0000011A  EB1C              jmp short 0x138
0000011C  8A842A30          mov al,[si+0x302a]
00000120  B400              mov ah,0x0
00000122  50                push ax
00000123  A0AA37            mov al,[0x37aa]
00000126  98                cbw
00000127  8BD8              mov bx,ax
00000129  8A872A30          mov al,[bx+0x302a]
0000012D  B400              mov ah,0x0
0000012F  40                inc ax
00000130  5A                pop dx
00000131  3BD0              cmp dx,ax
00000133  7502              jnz 0x137
00000135  EB06              jmp short 0x13d
00000137  46                inc si
00000138  83FE03            cmp si,byte +0x3
0000013B  7EDF              jng 0x11c
0000013D  8BDE              mov bx,si
0000013F  D1E3              shl bx,1
00000141  D1E3              shl bx,1
00000143  8B87082F          mov ax,[bx+0x2f08]
00000147  8B97062F          mov dx,[bx+0x2f06]
0000014B  D1E2              shl dx,1
0000014D  D1D0              rcl ax,1
0000014F  52                push dx
00000150  50                push ax
00000151  A0AA37            mov al,[0x37aa]
00000154  98                cbw
00000155  D1E0              shl ax,1
00000157  D1E0              shl ax,1
00000159  8BD8              mov bx,ax
0000015B  58                pop ax
0000015C  3B87082F          cmp ax,[bx+0x2f08]
00000160  58                pop ax
00000161  7C2A              jl 0x18d
00000163  7F06              jg 0x16b
00000165  3B87062F          cmp ax,[bx+0x2f06]
00000169  7622              jna 0x18d
0000016B  A0AA37            mov al,[0x37aa]
0000016E  98                cbw
0000016F  8BD8              mov bx,ax
00000171  8A87423D          mov al,[bx+0x3d42]
00000175  B400              mov ah,0x0
00000177  BA0500            mov dx,0x5
0000017A  F7EA              imul dx
0000017C  050800            add ax,0x8
0000017F  50                push ax
00000180  9ABA01FD21        call 0x21fd:0x1ba
00000185  59                pop cx
00000186  0BC0              or ax,ax
00000188  7403              jz 0x18d
0000018A  E92B01            jmp 0x2b8
0000018D  A0AA37            mov al,[0x37aa]
00000190  98                cbw
00000191  D1E0              shl ax,1
00000193  D1E0              shl ax,1
00000195  8BD8              mov bx,ax
00000197  8B87082F          mov ax,[bx+0x2f08]
0000019B  8B97062F          mov dx,[bx+0x2f06]
0000019F  8BDE              mov bx,si
000001A1  D1E3              shl bx,1
000001A3  D1E3              shl bx,1
000001A5  2B97062F          sub dx,[bx+0x2f06]
000001A9  1B87082F          sbb ax,[bx+0x2f08]
000001AD  8956F4            mov [bp-0xc],dx
000001B0  8946F6            mov [bp-0xa],ax
000001B3  A0AA37            mov al,[0x37aa]
000001B6  98                cbw
000001B7  8BD8              mov bx,ax
000001B9  80BF423D00        cmp byte [bx+0x3d42],0x0
000001BE  7526              jnz 0x1e6
000001C0  33C0              xor ax,ax
000001C2  BA0400            mov dx,0x4
000001C5  50                push ax
000001C6  52                push dx
000001C7  8B4EF6            mov cx,[bp-0xa]
000001CA  8B5EF4            mov bx,[bp-0xc]
000001CD  33D2              xor dx,dx
000001CF  B80300            mov ax,0x3
000001D2  9A4E4F0000        call 0x0:0x4f4e
000001D7  52                push dx
000001D8  50                push ax
000001D9  9A894F0000        call 0x0:0x4f89
000001DE  8946F8            mov [bp-0x8],ax
000001E1  8956FA            mov [bp-0x6],dx
000001E4  EB3E              jmp short 0x224
000001E6  33C0              xor ax,ax
000001E8  BA0300            mov dx,0x3
000001EB  50                push ax
000001EC  52                push dx
000001ED  FF76F6            push word [bp-0xa]
000001F0  FF76F4            push word [bp-0xc]
000001F3  9A894F0000        call 0x0:0x4f89
000001F8  8946F8            mov [bp-0x8],ax
000001FB  8956FA            mov [bp-0x6],dx
000001FE  837EFA00          cmp word [bp-0x6],byte +0x0
00000202  7C20              jl 0x224
00000204  7F07              jg 0x20d
00000206  817EF8ED03        cmp word [bp-0x8],0x3ed
0000020B  7617              jna 0x224
0000020D  B86400            mov ax,0x64
00000210  50                push ax
00000211  9ABA01FD21        call 0x21fd:0x1ba
00000216  59                pop cx
00000217  BAED03            mov dx,0x3ed
0000021A  2BD0              sub dx,ax
0000021C  8956F8            mov [bp-0x8],dx
0000021F  C746FA0000        mov word [bp-0x6],0x0
00000224  33C0              xor ax,ax
00000226  BA0200            mov dx,0x2
00000229  50                push ax
0000022A  52                push dx
0000022B  A0AA37            mov al,[0x37aa]
0000022E  98                cbw
0000022F  D1E0              shl ax,1
00000231  D1E0              shl ax,1
00000233  8BD8              mov bx,ax
00000235  FFB7D43C          push word [bx+0x3cd4]
00000239  FFB7D23C          push word [bx+0x3cd2]
0000023D  9A894F0000        call 0x0:0x4f89
00000242  3B56FA            cmp dx,[bp-0x6]
00000245  7F2B              jg 0x272
00000247  7C05              jl 0x24e
00000249  3B46F8            cmp ax,[bp-0x8]
0000024C  7324              jnc 0x272
0000024E  33C0              xor ax,ax
00000250  BA0200            mov dx,0x2
00000253  50                push ax
00000254  52                push dx
00000255  A0AA37            mov al,[0x37aa]
00000258  98                cbw
00000259  D1E0              shl ax,1
0000025B  D1E0              shl ax,1
0000025D  8BD8              mov bx,ax
0000025F  FFB7D43C          push word [bx+0x3cd4]
00000263  FFB7D23C          push word [bx+0x3cd2]
00000267  9A894F0000        call 0x0:0x4f89
0000026C  8946F8            mov [bp-0x8],ax
0000026F  8956FA            mov [bp-0x6],dx
00000272  A0AA37            mov al,[0x37aa]
00000275  98                cbw
00000276  8BD8              mov bx,ax
00000278  80BF400800        cmp byte [bx+0x840],0x0
0000027D  7407              jz 0x286
0000027F  833E8E2E03        cmp word [0x2e8e],byte +0x3
00000284  7D32              jnl 0x2b8
00000286  837EFA00          cmp word [bp-0x6],byte +0x0
0000028A  7C2C              jl 0x2b8
0000028C  7F06              jg 0x294
0000028E  837EF800          cmp word [bp-0x8],byte +0x0
00000292  7624              jna 0x2b8
00000294  9A8609CB17        call 0x17cb:0x986
00000299  FF76FA            push word [bp-0x6]
0000029C  FF76F8            push word [bp-0x8]
0000029F  9A9D03CB17        call 0x17cb:0x39d
000002A4  59                pop cx
000002A5  59                pop cx
000002A6  33C0              xor ax,ax
000002A8  50                push ax
000002A9  9A0400FD21        call 0x21fd:0x4
000002AE  59                pop cx
000002AF  33C0              xor ax,ax
000002B1  50                push ax
000002B2  9ADB05FD21        call 0x21fd:0x5db
000002B7  59                pop cx
000002B8  5E                pop si
000002B9  8BE5              mov sp,bp
000002BB  5D                pop bp
000002BC  CB                retf

; ---- 17CB:02BD ----
000002BD  55                push bp
000002BE  8BEC              mov bp,sp
000002C0  4C                dec sp
000002C1  4C                dec sp
000002C2  56                push si
000002C3  FF7608            push word [bp+0x8]
000002C6  FF7606            push word [bp+0x6]
000002C9  9A7E04CB17        call 0x17cb:0x47e
000002CE  59                pop cx
000002CF  59                pop cx
000002D0  8846FE            mov [bp-0x2],al
000002D3  8A46FE            mov al,[bp-0x2]
000002D6  98                cbw
000002D7  8BD8              mov bx,ax
000002D9  8A87AF09          mov al,[bx+0x9af]
000002DD  98                cbw
000002DE  8BF0              mov si,ax
000002E0  B84E00            mov ax,0x4e
000002E3  50                push ax
000002E4  33C0              xor ax,ax
000002E6  50                push ax
000002E7  9A5400A721        call 0x21a7:0x54
000002EC  59                pop cx
000002ED  59                pop cx
000002EE  33C0              xor ax,ax
000002F0  BA3200            mov dx,0x32
000002F3  50                push ax
000002F4  52                push dx
000002F5  9A8501FD21        call 0x21fd:0x185
000002FA  59                pop cx
000002FB  59                pop cx
000002FC  9A1B00960A        call 0xa96:0x1b
00000301  33C0              xor ax,ax
00000303  50                push ax
00000304  B81B00            mov ax,0x1b
00000307  50                push ax
00000308  B8F000            mov ax,0xf0
0000030B  50                push ax
0000030C  9A79180906        call 0x609:0x1879
00000311  83C406            add sp,byte +0x6
00000314  9A0D06FD21        call 0x21fd:0x60d
00000319  33C0              xor ax,ax
0000031B  BA1E00            mov dx,0x1e
0000031E  50                push ax
0000031F  52                push dx
00000320  9A8501FD21        call 0x21fd:0x185
00000325  59                pop cx
00000326  59                pop cx
00000327  8A46FE            mov al,[bp-0x2]
0000032A  98                cbw
0000032B  8BD8              mov bx,ax
0000032D  8A87AA09          mov al,[bx+0x9aa]
00000331  8846FF            mov [bp-0x1],al
00000334  8A46FF            mov al,[bp-0x1]
00000337  98                cbw
00000338  8BDE              mov bx,si
0000033A  03D8              add bx,ax
0000033C  D1E3              shl bx,1
0000033E  D1E3              shl bx,1
00000340  8B87CC37          mov ax,[bx+0x37cc]
00000344  40                inc ax
00000345  40                inc ax
00000346  FFB7CE37          push word [bx+0x37ce]
0000034A  50                push ax
0000034B  B80400            mov ax,0x4
0000034E  50                push ax
0000034F  FF7608            push word [bp+0x8]
00000352  FF7606            push word [bp+0x6]
00000355  9A01009522        call 0x2295:0x1
0000035A  83C40A            add sp,byte +0xa
0000035D  B80400            mov ax,0x4
00000360  50                push ax
00000361  8A46FE            mov al,[bp-0x2]
00000364  98                cbw
00000365  8BD8              mov bx,ax
00000367  8A87B409          mov al,[bx+0x9b4]
0000036B  98                cbw
0000036C  50                push ax
0000036D  1E                push ds
0000036E  8BC6              mov ax,si
00000370  D1E0              shl ax,1
00000372  D1E0              shl ax,1
00000374  05CC37            add ax,0x37cc
00000377  50                push ax
00000378  9AE009CB17        call 0x17cb:0x9e0
0000037D  83C408            add sp,byte +0x8
00000380  A0AA37            mov al,[0x37aa]
00000383  98                cbw
00000384  D1E0              shl ax,1
00000386  D1E0              shl ax,1
00000388  8B5608            mov dx,[bp+0x8]
0000038B  8B5E06            mov bx,[bp+0x6]
0000038E  87D8              xchg bx,ax
00000390  0187D23C          add [bx+0x3cd2],ax
00000394  1197D43C          adc [bx+0x3cd4],dx
00000398  5E                pop si
00000399  8BE5              mov sp,bp
0000039B  5D                pop bp
0000039C  CB                retf

; ---- 17CB:039D ----
0000039D  55                push bp
0000039E  8BEC              mov bp,sp
000003A0  4C                dec sp
000003A1  4C                dec sp
000003A2  56                push si
000003A3  FF7608            push word [bp+0x8]
000003A6  FF7606            push word [bp+0x6]
000003A9  9A7E04CB17        call 0x17cb:0x47e
000003AE  59                pop cx
000003AF  59                pop cx
000003B0  8846FE            mov [bp-0x2],al
000003B3  8A46FE            mov al,[bp-0x2]
000003B6  98                cbw
000003B7  8BD8              mov bx,ax
000003B9  8A87BE09          mov al,[bx+0x9be]
000003BD  98                cbw
000003BE  8BF0              mov si,ax
000003C0  B84F00            mov ax,0x4f
000003C3  50                push ax
000003C4  33C0              xor ax,ax
000003C6  50                push ax
000003C7  9A5400A721        call 0x21a7:0x54
000003CC  59                pop cx
000003CD  59                pop cx
000003CE  33C0              xor ax,ax
000003D0  BA3200            mov dx,0x32
000003D3  50                push ax
000003D4  52                push dx
000003D5  9A8501FD21        call 0x21fd:0x185
000003DA  59                pop cx
000003DB  59                pop cx
000003DC  9A1B00960A        call 0xa96:0x1b
000003E1  B80300            mov ax,0x3
000003E4  50                push ax
000003E5  B81B00            mov ax,0x1b
000003E8  50                push ax
000003E9  B8F000            mov ax,0xf0
000003EC  50                push ax
000003ED  9A79180906        call 0x609:0x1879
000003F2  83C406            add sp,byte +0x6
000003F5  9A0D06FD21        call 0x21fd:0x60d
000003FA  33C0              xor ax,ax
000003FC  BA1E00            mov dx,0x1e
000003FF  50                push ax
00000400  52                push dx
00000401  9A8501FD21        call 0x21fd:0x185
00000406  59                pop cx
00000407  59                pop cx
00000408  8A46FE            mov al,[bp-0x2]
0000040B  98                cbw
0000040C  8BD8              mov bx,ax
0000040E  8A87B909          mov al,[bx+0x9b9]
00000412  8846FF            mov [bp-0x1],al
00000415  8A46FF            mov al,[bp-0x1]
00000418  98                cbw
00000419  8BDE              mov bx,si
0000041B  03D8              add bx,ax
0000041D  D1E3              shl bx,1
0000041F  D1E3              shl bx,1
00000421  8B87CC37          mov ax,[bx+0x37cc]
00000425  40                inc ax
00000426  40                inc ax
00000427  FFB7CE37          push word [bx+0x37ce]
0000042B  50                push ax
0000042C  B80400            mov ax,0x4
0000042F  50                push ax
00000430  FF7608            push word [bp+0x8]
00000433  FF7606            push word [bp+0x6]
00000436  9A01009522        call 0x2295:0x1
0000043B  83C40A            add sp,byte +0xa
0000043E  B80500            mov ax,0x5
00000441  50                push ax
00000442  8A46FE            mov al,[bp-0x2]
00000445  98                cbw
00000446  8BD8              mov bx,ax
00000448  8A87C309          mov al,[bx+0x9c3]
0000044C  98                cbw
0000044D  50                push ax
0000044E  1E                push ds
0000044F  8BC6              mov ax,si
00000451  D1E0              shl ax,1
00000453  D1E0              shl ax,1
00000455  05CC37            add ax,0x37cc
00000458  50                push ax
00000459  9AE009CB17        call 0x17cb:0x9e0
0000045E  83C408            add sp,byte +0x8
00000461  A0AA37            mov al,[0x37aa]
00000464  98                cbw
00000465  D1E0              shl ax,1
00000467  D1E0              shl ax,1
00000469  8B5608            mov dx,[bp+0x8]
0000046C  8B5E06            mov bx,[bp+0x6]
0000046F  87D8              xchg bx,ax
00000471  2987D23C          sub [bx+0x3cd2],ax
00000475  1997D43C          sbb [bx+0x3cd4],dx
00000479  5E                pop si
0000047A  8BE5              mov sp,bp
0000047C  5D                pop bp
0000047D  CB                retf

; ---- 17CB:047E ----
0000047E  55                push bp
0000047F  8BEC              mov bp,sp
00000481  837E0800          cmp word [bp+0x8],byte +0x0
00000485  7F0D              jg 0x494
00000487  7C07              jl 0x490
00000489  817E06C800        cmp word [bp+0x6],0xc8
0000048E  7304              jnc 0x494
00000490  B000              mov al,0x0
00000492  EB3B              jmp short 0x4cf
00000494  837E0800          cmp word [bp+0x8],byte +0x0
00000498  7F0D              jg 0x4a7
0000049A  7C07              jl 0x4a3
0000049C  817E06F401        cmp word [bp+0x6],0x1f4
000004A1  7304              jnc 0x4a7
000004A3  B001              mov al,0x1
000004A5  EB28              jmp short 0x4cf
000004A7  837E0800          cmp word [bp+0x8],byte +0x0
000004AB  7F0D              jg 0x4ba
000004AD  7C07              jl 0x4b6
000004AF  817E06E803        cmp word [bp+0x6],0x3e8
000004B4  7304              jnc 0x4ba
000004B6  B002              mov al,0x2
000004B8  EB15              jmp short 0x4cf
000004BA  837E0800          cmp word [bp+0x8],byte +0x0
000004BE  7F0D              jg 0x4cd
000004C0  7C07              jl 0x4c9
000004C2  817E06D007        cmp word [bp+0x6],0x7d0
000004C7  7304              jnc 0x4cd
000004C9  B003              mov al,0x3
000004CB  EB02              jmp short 0x4cf
000004CD  B004              mov al,0x4
000004CF  8AD0              mov dl,al
000004D1  8AC2              mov al,dl
000004D3  EB00              jmp short 0x4d5
000004D5  5D                pop bp
000004D6  CB                retf

; ---- 17CB:04D7 ----
000004D7  55                push bp
000004D8  8BEC              mov bp,sp
000004DA  83EC08            sub sp,byte +0x8
000004DD  56                push si
000004DE  C606AA3700        mov byte [0x37aa],0x0
000004E3  EB13              jmp short 0x4f8
000004E5  A0AA37            mov al,[0x37aa]
000004E8  98                cbw
000004E9  8D56F8            lea dx,[bp-0x8]
000004EC  03C2              add ax,dx
000004EE  8BD8              mov bx,ax
000004F0  36C60703          mov byte [ss:bx],0x3
000004F4  FE06AA37          inc byte [0x37aa]
000004F8  803EAA3704        cmp byte [0x37aa],0x4
000004FD  7CE6              jl 0x4e5
000004FF  C606AA3700        mov byte [0x37aa],0x0
00000504  EB46              jmp short 0x54c
00000506  A0AA37            mov al,[0x37aa]
00000509  98                cbw
0000050A  40                inc ax
0000050B  8BF0              mov si,ax
0000050D  EB34              jmp short 0x543
0000050F  A0AA37            mov al,[0x37aa]
00000512  98                cbw
00000513  8BD8              mov bx,ax
00000515  8A87A84B          mov al,[bx+0x4ba8]
00000519  3A84A84B          cmp al,[si+0x4ba8]
0000051D  720E              jc 0x52d
0000051F  A0AA37            mov al,[0x37aa]
00000522  98                cbw
00000523  8D56F8            lea dx,[bp-0x8]
00000526  03C2              add ax,dx
00000528  8BD8              mov bx,ax
0000052A  36FE0F            dec byte [ss:bx]
0000052D  8A84A84B          mov al,[si+0x4ba8]
00000531  50                push ax
00000532  A0AA37            mov al,[0x37aa]
00000535  98                cbw
00000536  8BD8              mov bx,ax
00000538  58                pop ax
00000539  3A87A84B          cmp al,[bx+0x4ba8]
0000053D  7203              jc 0x542
0000053F  FE4AF8            dec byte [bp+si-0x8]
00000542  46                inc si
00000543  83FE03            cmp si,byte +0x3
00000546  7EC7              jng 0x50f
00000548  FE06AA37          inc byte [0x37aa]
0000054C  803EAA3704        cmp byte [0x37aa],0x4
00000551  7CB3              jl 0x506
00000553  B80300            mov ax,0x3
00000556  50                push ax
00000557  9ABA01FD21        call 0x21fd:0x1ba
0000055C  59                pop cx
0000055D  8946FE            mov [bp-0x2],ax
00000560  8D46F8            lea ax,[bp-0x8]
00000563  8B5EFE            mov bx,[bp-0x2]
00000566  03D8              add bx,ax
00000568  36803F01          cmp byte [ss:bx],0x1
0000056C  7F14              jg 0x582
0000056E  8B5EFE            mov bx,[bp-0x2]
00000571  80BF423D00        cmp byte [bx+0x3d42],0x0
00000576  770A              ja 0x582
00000578  8B5EFE            mov bx,[bp-0x2]
0000057B  80BFA84B07        cmp byte [bx+0x4ba8],0x7
00000580  7303              jnc 0x585
00000582  E9AC01            jmp 0x731
00000585  C746FCFFFF        mov word [bp-0x4],0xffff
0000058A  C606AA3700        mov byte [0x37aa],0x0
0000058F  EB1C              jmp short 0x5ad
00000591  A0AA37            mov al,[0x37aa]
00000594  98                cbw
00000595  8D56F8            lea dx,[bp-0x8]
00000598  03C2              add ax,dx
0000059A  8BD8              mov bx,ax
0000059C  36803F03          cmp byte [ss:bx],0x3
000005A0  7507              jnz 0x5a9
000005A2  A0AA37            mov al,[0x37aa]
000005A5  98                cbw
000005A6  8946FC            mov [bp-0x4],ax
000005A9  FE06AA37          inc byte [0x37aa]
000005AD  803EAA3704        cmp byte [0x37aa],0x4
000005B2  7CDD              jl 0x591
000005B4  837EFCFF          cmp word [bp-0x4],byte -0x1
000005B8  740A              jz 0x5c4
000005BA  8B5EFC            mov bx,[bp-0x4]
000005BD  80BF423D00        cmp byte [bx+0x3d42],0x0
000005C2  7503              jnz 0x5c7
000005C4  E96A01            jmp 0x731
000005C7  8B5EFC            mov bx,[bp-0x4]
000005CA  8A87A84B          mov al,[bx+0x4ba8]
000005CE  B400              mov ah,0x0
000005D0  BA0F00            mov dx,0xf
000005D3  F7EA              imul dx
000005D5  BB0A00            mov bx,0xa
000005D8  99                cwd
000005D9  F7FB              idiv bx
000005DB  8B5EFE            mov bx,[bp-0x2]
000005DE  8A97A84B          mov dl,[bx+0x4ba8]
000005E2  B600              mov dh,0x0
000005E4  3BC2              cmp ax,dx
000005E6  7E11              jng 0x5f9
000005E8  B80800            mov ax,0x8
000005EB  50                push ax
000005EC  9ABA01FD21        call 0x21fd:0x1ba
000005F1  59                pop cx
000005F2  0BC0              or ax,ax
000005F4  7403              jz 0x5f9
000005F6  E93801            jmp 0x731
000005F9  9A8609CB17        call 0x17cb:0x986
000005FE  B80500            mov ax,0x5
00000601  50                push ax
00000602  B80700            mov ax,0x7
00000605  50                push ax
00000606  1E                push ds
00000607  B84039            mov ax,0x3940
0000060A  50                push ax
0000060B  9AE009CB17        call 0x17cb:0x9e0
00000610  83C408            add sp,byte +0x8
00000613  8A46FE            mov al,[bp-0x2]
00000616  A2AA37            mov [0x37aa],al
00000619  B84F00            mov ax,0x4f
0000061C  50                push ax
0000061D  33C0              xor ax,ax
0000061F  50                push ax
00000620  9A5400A721        call 0x21a7:0x54
00000625  59                pop cx
00000626  59                pop cx
00000627  33C0              xor ax,ax
00000629  BA3200            mov dx,0x32
0000062C  50                push ax
0000062D  52                push dx
0000062E  9A8501FD21        call 0x21fd:0x185
00000633  59                pop cx
00000634  59                pop cx
00000635  9A1B00960A        call 0xa96:0x1b
0000063A  B80300            mov ax,0x3
0000063D  50                push ax
0000063E  B81B00            mov ax,0x1b
00000641  50                push ax
00000642  B82000            mov ax,0x20
00000645  50                push ax
00000646  9A79180906        call 0x609:0x1879
0000064B  83C406            add sp,byte +0x6
0000064E  9A0D06FD21        call 0x21fd:0x60d
00000653  33C0              xor ax,ax
00000655  BA1E00            mov dx,0x1e
00000658  50                push ax
00000659  52                push dx
0000065A  9A8501FD21        call 0x21fd:0x185
0000065F  59                pop cx
00000660  59                pop cx
00000661  8A46FC            mov al,[bp-0x4]
00000664  A2AA37            mov [0x37aa],al
00000667  B84E00            mov ax,0x4e
0000066A  50                push ax
0000066B  33C0              xor ax,ax
0000066D  50                push ax
0000066E  9A5400A721        call 0x21a7:0x54
00000673  59                pop cx
00000674  59                pop cx
00000675  33C0              xor ax,ax
00000677  BA3200            mov dx,0x32
0000067A  50                push ax
0000067B  52                push dx
0000067C  9A8501FD21        call 0x21fd:0x185
00000681  59                pop cx
00000682  59                pop cx
00000683  9A1B00960A        call 0xa96:0x1b
00000688  33C0              xor ax,ax
0000068A  50                push ax
0000068B  B81B00            mov ax,0x1b
0000068E  50                push ax
0000068F  B8F000            mov ax,0xf0
00000692  50                push ax
00000693  9A79180906        call 0x609:0x1879
00000698  83C406            add sp,byte +0x6
0000069B  9A0D06FD21        call 0x21fd:0x60d
000006A0  A1FC08            mov ax,[0x8fc]
000006A3  48                dec ax
000006A4  50                push ax
000006A5  9ABA01FD21        call 0x21fd:0x1ba
000006AA  59                pop cx
000006AB  A2B02E            mov [0x2eb0],al
000006AE  A1FE08            mov ax,[0x8fe]
000006B1  48                dec ax
000006B2  50                push ax
000006B3  9ABA01FD21        call 0x21fd:0x1ba
000006B8  59                pop cx
000006B9  A2002F            mov [0x2f00],al
000006BC  A0B02E            mov al,[0x2eb0]
000006BF  98                cbw
000006C0  BA0600            mov dx,0x6
000006C3  F7EA              imul dx
000006C5  50                push ax
000006C6  A0002F            mov al,[0x2f00]
000006C9  98                cbw
000006CA  5B                pop bx
000006CB  03D8              add bx,ax
000006CD  8A874244          mov al,[bx+0x4442]
000006D1  98                cbw
000006D2  3B46FE            cmp ax,[bp-0x2]
000006D5  751A              jnz 0x6f1
000006D7  A0B02E            mov al,[0x2eb0]
000006DA  98                cbw
000006DB  BA0600            mov dx,0x6
000006DE  F7EA              imul dx
000006E0  50                push ax
000006E1  A0002F            mov al,[0x2f00]
000006E4  98                cbw
000006E5  5B                pop bx
000006E6  03D8              add bx,ax
000006E8  80BFB12E04        cmp byte [bx+0x2eb1],0x4
000006ED  7402              jz 0x6f1
000006EF  EB02              jmp short 0x6f3
000006F1  EBAD              jmp short 0x6a0
000006F3  A0B02E            mov al,[0x2eb0]
000006F6  98                cbw
000006F7  BA0600            mov dx,0x6
000006FA  F7EA              imul dx
000006FC  50                push ax
000006FD  A0002F            mov al,[0x2f00]
00000700  98                cbw
00000701  5B                pop bx
00000702  03D8              add bx,ax
00000704  8A87B12E          mov al,[bx+0x2eb1]
00000708  98                cbw
00000709  50                push ax
0000070A  FF76FC            push word [bp-0x4]
0000070D  A0002F            mov al,[0x2f00]
00000710  98                cbw
00000711  50                push ax
00000712  A0B02E            mov al,[0x2eb0]
00000715  98                cbw
00000716  50                push ax
00000717  9AD2100906        call 0x609:0x10d2
0000071C  83C408            add sp,byte +0x8
0000071F  33C0              xor ax,ax
00000721  50                push ax
00000722  9A0400FD21        call 0x21fd:0x4
00000727  59                pop cx
00000728  33C0              xor ax,ax
0000072A  50                push ax
0000072B  9ADB05FD21        call 0x21fd:0x5db
00000730  59                pop cx
00000731  5E                pop si
00000732  8BE5              mov sp,bp
00000734  5D                pop bp
00000735  CB                retf

; ---- 17CB:0736 ----
00000736  55                push bp
00000737  8BEC              mov bp,sp
00000739  4C                dec sp
0000073A  4C                dec sp
0000073B  56                push si
0000073C  57                push di
0000073D  9A5007A322        call 0x22a3:0x750
00000742  B80300            mov ax,0x3
00000745  50                push ax
00000746  9ABA01FD21        call 0x21fd:0x1ba
0000074B  59                pop cx
0000074C  8BF8              mov di,ax
0000074E  80BD423D01        cmp byte [di+0x3d42],0x1
00000753  7603              jna 0x758
00000755  E92802            jmp 0x980
00000758  80BD423D01        cmp byte [di+0x3d42],0x1
0000075D  7511              jnz 0x770
0000075F  B80600            mov ax,0x6
00000762  50                push ax
00000763  9ABA01FD21        call 0x21fd:0x1ba
00000768  59                pop cx
00000769  0BC0              or ax,ax
0000076B  7403              jz 0x770
0000076D  E91002            jmp 0x980
00000770  80BD423D00        cmp byte [di+0x3d42],0x0
00000775  7511              jnz 0x788
00000777  B80300            mov ax,0x3
0000077A  50                push ax
0000077B  9ABA01FD21        call 0x21fd:0x1ba
00000780  59                pop cx
00000781  0BC0              or ax,ax
00000783  7403              jz 0x788
00000785  E9F801            jmp 0x980
00000788  33F6              xor si,si
0000078A  E9EB01            jmp 0x978
0000078D  8BC7              mov ax,di
0000078F  BA0300            mov dx,0x3
00000792  F7EA              imul dx
00000794  8BD8              mov bx,ax
00000796  80B82C343C        cmp byte [bx+si+0x342c],0x3c
0000079B  7703              ja 0x7a0
0000079D  E9D701            jmp 0x977
000007A0  9A8609CB17        call 0x17cb:0x986
000007A5  C746FE1900        mov word [bp-0x2],0x19
000007AA  C606AA3700        mov byte [0x37aa],0x0
000007AF  EB66              jmp short 0x817
000007B1  A0AA37            mov al,[0x37aa]
000007B4  98                cbw
000007B5  BA0300            mov dx,0x3
000007B8  F7EA              imul dx
000007BA  8BD8              mov bx,ax
000007BC  80B82C3400        cmp byte [bx+si+0x342c],0x0
000007C1  7650              jna 0x813
000007C3  B84F00            mov ax,0x4f
000007C6  50                push ax
000007C7  33C0              xor ax,ax
000007C9  50                push ax
000007CA  9A5400A721        call 0x21a7:0x54
000007CF  59                pop cx
000007D0  59                pop cx
000007D1  33C0              xor ax,ax
000007D3  BA3200            mov dx,0x32
000007D6  50                push ax
000007D7  52                push dx
000007D8  9A8501FD21        call 0x21fd:0x185
000007DD  59                pop cx
000007DE  59                pop cx
000007DF  9A1B00960A        call 0xa96:0x1b
000007E4  B80300            mov ax,0x3
000007E7  50                push ax
000007E8  B81B00            mov ax,0x1b
000007EB  50                push ax
000007EC  FF76FE            push word [bp-0x2]
000007EF  9A79180906        call 0x609:0x1879
000007F4  83C406            add sp,byte +0x6
000007F7  9A0D06FD21        call 0x21fd:0x60d
000007FC  8B46FE            mov ax,[bp-0x2]
000007FF  053A00            add ax,0x3a
00000802  8946FE            mov [bp-0x2],ax
00000805  33C0              xor ax,ax
00000807  BA1400            mov dx,0x14
0000080A  50                push ax
0000080B  52                push dx
0000080C  9A8501FD21        call 0x21fd:0x185
00000811  59                pop cx
00000812  59                pop cx
00000813  FE06AA37          inc byte [0x37aa]
00000817  803EAA3704        cmp byte [0x37aa],0x4
0000081C  7C93              jl 0x7b1
0000081E  C746FEF800        mov word [bp-0x2],0xf8
00000823  C606AA3700        mov byte [0x37aa],0x0
00000828  EB65              jmp short 0x88f
0000082A  A0AA37            mov al,[0x37aa]
0000082D  98                cbw
0000082E  BA0300            mov dx,0x3
00000831  F7EA              imul dx
00000833  8BD8              mov bx,ax
00000835  80B82C3400        cmp byte [bx+si+0x342c],0x0
0000083A  754F              jnz 0x88b
0000083C  B84E00            mov ax,0x4e
0000083F  50                push ax
00000840  33C0              xor ax,ax
00000842  50                push ax
00000843  9A5400A721        call 0x21a7:0x54
00000848  59                pop cx
00000849  59                pop cx
0000084A  33C0              xor ax,ax
0000084C  BA3200            mov dx,0x32
0000084F  50                push ax
00000850  52                push dx
00000851  9A8501FD21        call 0x21fd:0x185
00000856  59                pop cx
00000857  59                pop cx
00000858  9A1B00960A        call 0xa96:0x1b
0000085D  33C0              xor ax,ax
0000085F  50                push ax
00000860  B81B00            mov ax,0x1b
00000863  50                push ax
00000864  FF76FE            push word [bp-0x2]
00000867  9A79180906        call 0x609:0x1879
0000086C  83C406            add sp,byte +0x6
0000086F  9A0D06FD21        call 0x21fd:0x60d
00000874  8B46FE            mov ax,[bp-0x2]
00000877  05C6FF            add ax,0xffc6
0000087A  8946FE            mov [bp-0x2],ax
0000087D  33C0              xor ax,ax
0000087F  BA1400            mov dx,0x14
00000882  50                push ax
00000883  52                push dx
00000884  9A8501FD21        call 0x21fd:0x185
00000889  59                pop cx
0000088A  59                pop cx
0000088B  FE06AA37          inc byte [0x37aa]
0000088F  803EAA3704        cmp byte [0x37aa],0x4
00000894  7C94              jl 0x82a
00000896  B80500            mov ax,0x5
00000899  50                push ax
0000089A  B80400            mov ax,0x4
0000089D  50                push ax
0000089E  1E                push ds
0000089F  8BC6              mov ax,si
000008A1  D1E0              shl ax,1
000008A3  D1E0              shl ax,1
000008A5  D1E0              shl ax,1
000008A7  D1E0              shl ax,1
000008A9  055C39            add ax,0x395c
000008AC  50                push ax
000008AD  9AE009CB17        call 0x17cb:0x9e0
000008B2  83C408            add sp,byte +0x8
000008B5  C606002F00        mov byte [0x2f00],0x0
000008BA  E99600            jmp 0x953
000008BD  C606B02E00        mov byte [0x2eb0],0x0
000008C2  EB7B              jmp short 0x93f
000008C4  A0B02E            mov al,[0x2eb0]
000008C7  98                cbw
000008C8  BA0600            mov dx,0x6
000008CB  F7EA              imul dx
000008CD  50                push ax
000008CE  A0002F            mov al,[0x2f00]
000008D1  98                cbw
000008D2  5B                pop bx
000008D3  03D8              add bx,ax
000008D5  8A87B12E          mov al,[bx+0x2eb1]
000008D9  98                cbw
000008DA  3BC6              cmp ax,si
000008DC  755D              jnz 0x93b
000008DE  A0B02E            mov al,[0x2eb0]
000008E1  98                cbw
000008E2  BA0600            mov dx,0x6
000008E5  F7EA              imul dx
000008E7  50                push ax
000008E8  A0002F            mov al,[0x2f00]
000008EB  98                cbw
000008EC  5B                pop bx
000008ED  03D8              add bx,ax
000008EF  8A874244          mov al,[bx+0x4442]
000008F3  98                cbw
000008F4  8BD8              mov bx,ax
000008F6  8A87423D          mov al,[bx+0x3d42]
000008FA  B400              mov ah,0x0
000008FC  BA0300            mov dx,0x3
000008FF  2BD0              sub dx,ax
00000901  52                push dx
00000902  9ABA01FD21        call 0x21fd:0x1ba
00000907  59                pop cx
00000908  0BC0              or ax,ax
0000090A  7502              jnz 0x90e
0000090C  EB2D              jmp short 0x93b
0000090E  B8FEFF            mov ax,0xfffe
00000911  50                push ax
00000912  A0B02E            mov al,[0x2eb0]
00000915  98                cbw
00000916  BA0600            mov dx,0x6
00000919  F7EA              imul dx
0000091B  50                push ax
0000091C  A0002F            mov al,[0x2f00]
0000091F  98                cbw
00000920  5B                pop bx
00000921  03D8              add bx,ax
00000923  8A874244          mov al,[bx+0x4442]
00000927  98                cbw
00000928  50                push ax
00000929  A0002F            mov al,[0x2f00]
0000092C  98                cbw
0000092D  50                push ax
0000092E  A0B02E            mov al,[0x2eb0]
00000931  98                cbw
00000932  50                push ax
00000933  9AD2100906        call 0x609:0x10d2
00000938  83C408            add sp,byte +0x8
0000093B  FE06B02E          inc byte [0x2eb0]
0000093F  A0B02E            mov al,[0x2eb0]
00000942  98                cbw
00000943  8B16FC08          mov dx,[0x8fc]
00000947  4A                dec dx
00000948  3BC2              cmp ax,dx
0000094A  7F03              jg 0x94f
0000094C  E975FF            jmp 0x8c4
0000094F  FE06002F          inc byte [0x2f00]
00000953  A0002F            mov al,[0x2f00]
00000956  98                cbw
00000957  8B16FE08          mov dx,[0x8fe]
0000095B  4A                dec dx
0000095C  3BC2              cmp ax,dx
0000095E  7F03              jg 0x963
00000960  E95AFF            jmp 0x8bd
00000963  33C0              xor ax,ax
00000965  50                push ax
00000966  9A0400FD21        call 0x21fd:0x4
0000096B  59                pop cx
0000096C  33C0              xor ax,ax
0000096E  50                push ax
0000096F  9ADB05FD21        call 0x21fd:0x5db
00000974  59                pop cx
00000975  EB09              jmp short 0x980
00000977  46                inc si
00000978  83FE02            cmp si,byte +0x2
0000097B  7F03              jg 0x980
0000097D  E90DFE            jmp 0x78d
00000980  5F                pop di
00000981  5E                pop si
00000982  8BE5              mov sp,bp
00000984  5D                pop bp
00000985  CB                retf

; ---- 17CB:0986 ----
00000986  55                push bp
00000987  8BEC              mov bp,sp
00000989  B80400            mov ax,0x4
0000098C  50                push ax
0000098D  9A7D03FD21        call 0x21fd:0x37d
00000992  59                pop cx
00000993  9A1B00960A        call 0xa96:0x1b
00000998  9AD91E0906        call 0x609:0x1ed9
0000099D  B8A000            mov ax,0xa0
000009A0  50                push ax
000009A1  B84001            mov ax,0x140
000009A4  50                push ax
000009A5  B81400            mov ax,0x14
000009A8  50                push ax
000009A9  33C0              xor ax,ax
000009AB  50                push ax
000009AC  9A56120906        call 0x609:0x1256
000009B1  83C408            add sp,byte +0x8
000009B4  9AA806960A        call 0xa96:0x6a8
000009B9  FF362E08          push word [0x82e]
000009BD  9A3F0C960A        call 0xa96:0xc3f
000009C2  59                pop cx
000009C3  B80100            mov ax,0x1
000009C6  50                push ax
000009C7  9ADB05FD21        call 0x21fd:0x5db
000009CC  59                pop cx
000009CD  1E                push ds
000009CE  B8A800            mov ax,0xa8
000009D1  50                push ax
000009D2  9A6C02FD21        call 0x21fd:0x26c
000009D7  59                pop cx
000009D8  59                pop cx
000009D9  9A2903FD21        call 0x21fd:0x329
000009DE  5D                pop bp
000009DF  CB                retf

; ---- 17CB:09E0 ----
000009E0  55                push bp
000009E1  8BEC              mov bp,sp
000009E3  B80500            mov ax,0x5
000009E6  50                push ax
000009E7  FF760C            push word [bp+0xc]
000009EA  FF760A            push word [bp+0xa]
000009ED  FF7608            push word [bp+0x8]
000009F0  FF7606            push word [bp+0x6]
000009F3  B84001            mov ax,0x140
000009F6  50                push ax
000009F7  8B460A            mov ax,[bp+0xa]
000009FA  BA0700            mov dx,0x7
000009FD  F7EA              imul dx
000009FF  BA8600            mov dx,0x86
00000A02  2BD0              sub dx,ax
00000A04  52                push dx
00000A05  33C0              xor ax,ax
00000A07  50                push ax
00000A08  9A8802690A        call 0xa69:0x288
00000A0D  83C410            add sp,byte +0x10
