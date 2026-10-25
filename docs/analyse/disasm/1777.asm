; segment 1777  file offset 0x1AD70  size 0x540
00000000  98                cbw
00000001  8BD8              mov bx,ax
00000003  C687C33704        mov byte [bx+0x37c3],0x4
00000008  8BE5              mov sp,bp
0000000A  5D                pop bp
0000000B  CB                retf

; ---- 1777:000C ----
0000000C  55                push bp
0000000D  8BEC              mov bp,sp
0000000F  83EC04            sub sp,byte +0x4
00000012  56                push si
00000013  57                push di
00000014  BE0100            mov si,0x1
00000017  E9F700            jmp 0x111
0000001A  1E                push ds
0000001B  B81D09            mov ax,0x91d
0000001E  50                push ax
0000001F  8BC6              mov ax,si
00000021  D1E0              shl ax,1
00000023  D1E0              shl ax,1
00000025  C45E08            les bx,[bp+0x8]
00000028  03D8              add bx,ax
0000002A  26FF7702          push word [es:bx+0x2]
0000002E  26FF37            push word [es:bx]
00000031  9AA4420000        call 0x0:0x42a4
00000036  83C408            add sp,byte +0x8
00000039  0BC0              or ax,ax
0000003B  7506              jnz 0x43
0000003D  C7060E090100      mov word [0x90e],0x1
00000043  1E                push ds
00000044  B82109            mov ax,0x921
00000047  50                push ax
00000048  8BC6              mov ax,si
0000004A  D1E0              shl ax,1
0000004C  D1E0              shl ax,1
0000004E  C45E08            les bx,[bp+0x8]
00000051  03D8              add bx,ax
00000053  26FF7702          push word [es:bx+0x2]
00000057  26FF37            push word [es:bx]
0000005A  9AA4420000        call 0x0:0x42a4
0000005F  83C408            add sp,byte +0x8
00000062  0BC0              or ax,ax
00000064  7506              jnz 0x6c
00000066  C7060E090000      mov word [0x90e],0x0
0000006C  1E                push ds
0000006D  B82509            mov ax,0x925
00000070  50                push ax
00000071  8BC6              mov ax,si
00000073  D1E0              shl ax,1
00000075  D1E0              shl ax,1
00000077  C45E08            les bx,[bp+0x8]
0000007A  03D8              add bx,ax
0000007C  26FF7702          push word [es:bx+0x2]
00000080  26FF37            push word [es:bx]
00000083  9AA4420000        call 0x0:0x42a4
00000088  83C408            add sp,byte +0x8
0000008B  0BC0              or ax,ax
0000008D  7506              jnz 0x95
0000008F  C7060E090200      mov word [0x90e],0x2
00000095  1E                push ds
00000096  B82B09            mov ax,0x92b
00000099  50                push ax
0000009A  8BC6              mov ax,si
0000009C  D1E0              shl ax,1
0000009E  D1E0              shl ax,1
000000A0  C45E08            les bx,[bp+0x8]
000000A3  03D8              add bx,ax
000000A5  26FF7702          push word [es:bx+0x2]
000000A9  26FF37            push word [es:bx]
000000AC  9AA4420000        call 0x0:0x42a4
000000B1  83C408            add sp,byte +0x8
000000B4  0BC0              or ax,ax
000000B6  7506              jnz 0xbe
000000B8  C7060E096300      mov word [0x90e],0x63
000000BE  1E                push ds
000000BF  B83509            mov ax,0x935
000000C2  50                push ax
000000C3  8BC6              mov ax,si
000000C5  D1E0              shl ax,1
000000C7  D1E0              shl ax,1
000000C9  C45E08            les bx,[bp+0x8]
000000CC  03D8              add bx,ax
000000CE  26FF7702          push word [es:bx+0x2]
000000D2  26FF37            push word [es:bx]
000000D5  9AA4420000        call 0x0:0x42a4
000000DA  83C408            add sp,byte +0x8
000000DD  0BC0              or ax,ax
000000DF  7506              jnz 0xe7
000000E1  C70610090000      mov word [0x910],0x0
000000E7  1E                push ds
000000E8  B83C09            mov ax,0x93c
000000EB  50                push ax
000000EC  8BC6              mov ax,si
000000EE  D1E0              shl ax,1
000000F0  D1E0              shl ax,1
000000F2  C45E08            les bx,[bp+0x8]
000000F5  03D8              add bx,ax
000000F7  26FF7702          push word [es:bx+0x2]
000000FB  26FF37            push word [es:bx]
000000FE  9AA4420000        call 0x0:0x42a4
00000103  83C408            add sp,byte +0x8
00000106  0BC0              or ax,ax
00000108  7506              jnz 0x110
0000010A  C70612090100      mov word [0x912],0x1
00000110  46                inc si
00000111  3B7606            cmp si,[bp+0x6]
00000114  7D03              jnl 0x119
00000116  E901FF            jmp 0x1a
00000119  9AE4047717        call 0x1777:0x4e4
0000011E  8BF8              mov di,ax
00000120  0BFF              or di,di
00000122  7402              jz 0x126
00000124  EB66              jmp short 0x18c
00000126  9A0D00FD0D        call 0xdfd:0xd
0000012B  9A3A018B21        call 0x218b:0x13a
00000130  B400              mov ah,0x0
00000132  0BC0              or ax,ax
00000134  7505              jnz 0x13b
00000136  9A0D00A721        call 0x21a7:0xd
0000013B  9A3A018B21        call 0x218b:0x13a
00000140  B400              mov ah,0x0
00000142  0BC0              or ax,ax
00000144  751D              jnz 0x163
00000146  B80100            mov ax,0x1
00000149  50                push ax
0000014A  33C0              xor ax,ax
0000014C  50                push ax
0000014D  9A5400A721        call 0x21a7:0x54
00000152  59                pop cx
00000153  59                pop cx
00000154  B80200            mov ax,0x2
00000157  50                push ax
00000158  B80100            mov ax,0x1
0000015B  50                push ax
0000015C  9A94008B21        call 0x218b:0x94
00000161  59                pop cx
00000162  59                pop cx
00000163  9A07002D19        call 0x192d:0x7
00000168  0AC0              or al,al
0000016A  7502              jnz 0x16e
0000016C  EB1E              jmp short 0x18c
0000016E  803ECC3C01        cmp byte [0x3ccc],0x1
00000173  7505              jnz 0x17a
00000175  9A0300310E        call 0xe31:0x3
0000017A  C606FA0801        mov byte [0x8fa],0x1
0000017F  9AF8017717        call 0x1777:0x1f8
00000184  3C01              cmp al,0x1
00000186  7502              jnz 0x18a
00000188  EB02              jmp short 0x18c
0000018A  EBAF              jmp short 0x13b
0000018C  9A53018B21        call 0x218b:0x153
00000191  9A4300A721        call 0x21a7:0x43
00000196  9AA904730D        call 0xd73:0x4a9
0000019B  B80300            mov ax,0x3
0000019E  50                push ax
0000019F  9AB607730D        call 0xd73:0x7b6
000001A4  59                pop cx
000001A5  8BC7              mov ax,di
000001A7  0BC0              or ax,ax
000001A9  740C              jz 0x1b7
000001AB  3D0100            cmp ax,0x1
000001AE  7411              jz 0x1c1
000001B0  3D0200            cmp ax,0x2
000001B3  7416              jz 0x1cb
000001B5  EB1E              jmp short 0x1d5
000001B7  C746FC3E09        mov word [bp-0x4],0x93e
000001BC  8C5EFE            mov [bp-0x2],ds
000001BF  EB1E              jmp short 0x1df
000001C1  C746FC4609        mov word [bp-0x4],0x946
000001C6  8C5EFE            mov [bp-0x2],ds
000001C9  EB14              jmp short 0x1df
000001CB  C746FC5A09        mov word [bp-0x4],0x95a
000001D0  8C5EFE            mov [bp-0x2],ds
000001D3  EB0A              jmp short 0x1df
000001D5  C746FC8F09        mov word [bp-0x4],0x98f
000001DA  8C5EFE            mov [bp-0x2],ds
000001DD  EB00              jmp short 0x1df
000001DF  FF76FE            push word [bp-0x2]
000001E2  FF76FC            push word [bp-0x4]
000001E5  1E                push ds
000001E6  B8A509            mov ax,0x9a5
000001E9  50                push ax
000001EA  9A592E0000        call 0x0:0x2e59
000001EF  83C408            add sp,byte +0x8
000001F2  5F                pop di
000001F3  5E                pop si
000001F4  8BE5              mov sp,bp
000001F6  5D                pop bp
000001F7  CB                retf

; ---- 1777:01F8 ----
000001F8  55                push bp
000001F9  8BEC              mov bp,sp
000001FB  9ADC027717        call 0x1777:0x2dc
00000200  9AC4072508        call 0x825:0x7c4
00000205  B001              mov al,0x1
00000207  50                push ax
00000208  9A0A002508        call 0x825:0xa
0000020D  59                pop cx
0000020E  803E553D00        cmp byte [0x3d55],0x0
00000213  7405              jz 0x21a
00000215  B000              mov al,0x0
00000217  E9C000            jmp 0x2da
0000021A  9A09001D11        call 0x111d:0x9
0000021F  9A78110906        call 0x609:0x1178
00000224  B000              mov al,0x0
00000226  50                push ax
00000227  9A0000A311        call 0x11a3:0x0
0000022C  59                pop cx
0000022D  33C0              xor ax,ax
0000022F  50                push ax
00000230  9A7709FD21        call 0x21fd:0x977
00000235  59                pop cx
00000236  9A04000906        call 0x609:0x4
0000023B  9A0500CB17        call 0x17cb:0x5
00000240  B80100            mov ax,0x1
00000243  50                push ax
00000244  9A7709FD21        call 0x21fd:0x977
00000249  59                pop cx
0000024A  9A0900031B        call 0x1b03:0x9
0000024F  9A1F02FD21        call 0x21fd:0x21f
00000254  9A0300A322        call 0x22a3:0x3
00000259  9A6B02A322        call 0x22a3:0x26b
0000025E  B80200            mov ax,0x2
00000261  50                push ax
00000262  9A7709FD21        call 0x21fd:0x977
00000267  59                pop cx
00000268  9AD302A322        call 0x22a3:0x2d3
0000026D  B80300            mov ax,0x3
00000270  50                push ax
00000271  9A7709FD21        call 0x21fd:0x977
00000276  59                pop cx
00000277  9A0B00681C        call 0x1c68:0xb
0000027C  9A0500B320        call 0x20b3:0x5
00000281  3C01              cmp al,0x1
00000283  7504              jnz 0x289
00000285  B001              mov al,0x1
00000287  EB51              jmp short 0x2da
00000289  9A78110906        call 0x609:0x1178
0000028E  B001              mov al,0x1
00000290  50                push ax
00000291  9A0000A311        call 0x11a3:0x0
00000296  59                pop cx
00000297  B80400            mov ax,0x4
0000029A  50                push ax
0000029B  9A7709FD21        call 0x21fd:0x977
000002A0  59                pop cx
000002A1  9A02009F0B        call 0xb9f:0x2
000002A6  9A0C00EF1D        call 0x1def:0xc
000002AB  B80500            mov ax,0x5
000002AE  50                push ax
000002AF  9A7709FD21        call 0x21fd:0x977
000002B4  59                pop cx
000002B5  9A0F00631F        call 0x1f63:0xf
000002BA  9AD711CF08        call 0x8cf:0x11d7
000002BF  B80600            mov ax,0x6
000002C2  50                push ax
000002C3  9A7709FD21        call 0x21fd:0x977
000002C8  59                pop cx
000002C9  9A0400CF08        call 0x8cf:0x4
000002CE  9A030B631F        call 0x1f63:0xb03
000002D3  FF069A4B          inc word [0x4b9a]
000002D7  E921FF            jmp 0x1fb
000002DA  5D                pop bp
000002DB  CB                retf

; ---- 1777:02DC ----
000002DC  55                push bp
000002DD  8BEC              mov bp,sp
000002DF  83EC0C            sub sp,byte +0xc
000002E2  C746FC4042        mov word [bp-0x4],0x4240
000002E7  C746FE0F00        mov word [bp-0x2],0xf
000002EC  C746F80000        mov word [bp-0x8],0x0
000002F1  C746FA0000        mov word [bp-0x6],0x0
000002F6  C606AA3700        mov byte [0x37aa],0x0
000002FB  EB52              jmp short 0x34f
000002FD  A0AA37            mov al,[0x37aa]
00000300  98                cbw
00000301  D1E0              shl ax,1
00000303  D1E0              shl ax,1
00000305  8BD8              mov bx,ax
00000307  8B87D43C          mov ax,[bx+0x3cd4]
0000030B  8B97D23C          mov dx,[bx+0x3cd2]
0000030F  3B46FE            cmp ax,[bp-0x2]
00000312  7F1F              jg 0x333
00000314  7C05              jl 0x31b
00000316  3B56FC            cmp dx,[bp-0x4]
00000319  7318              jnc 0x333
0000031B  A0AA37            mov al,[0x37aa]
0000031E  98                cbw
0000031F  D1E0              shl ax,1
00000321  D1E0              shl ax,1
00000323  8BD8              mov bx,ax
00000325  8B87D43C          mov ax,[bx+0x3cd4]
00000329  8B97D23C          mov dx,[bx+0x3cd2]
0000032D  8956FC            mov [bp-0x4],dx
00000330  8946FE            mov [bp-0x2],ax
00000333  A0AA37            mov al,[0x37aa]
00000336  98                cbw
00000337  D1E0              shl ax,1
00000339  D1E0              shl ax,1
0000033B  8BD8              mov bx,ax
0000033D  8B87D43C          mov ax,[bx+0x3cd4]
00000341  8B97D23C          mov dx,[bx+0x3cd2]
00000345  0156F8            add [bp-0x8],dx
00000348  1146FA            adc [bp-0x6],ax
0000034B  FE06AA37          inc byte [0x37aa]
0000034F  803EAA3704        cmp byte [0x37aa],0x4
00000354  7CA7              jl 0x2fd
00000356  33C0              xor ax,ax
00000358  BA0400            mov dx,0x4
0000035B  50                push ax
0000035C  52                push dx
0000035D  FF76FA            push word [bp-0x6]
00000360  FF76F8            push word [bp-0x8]
00000363  9A894F0000        call 0x0:0x4f89
00000368  8946F4            mov [bp-0xc],ax
0000036B  8956F6            mov [bp-0xa],dx
0000036E  A1FC08            mov ax,[0x8fc]
00000371  F72EFE08          imul word [0x8fe]
00000375  50                push ax
00000376  A0052F            mov al,[0x2f05]
00000379  98                cbw
0000037A  8BD0              mov dx,ax
0000037C  58                pop ax
0000037D  2BC2              sub ax,dx
0000037F  D1E0              shl ax,1
00000381  40                inc ax
00000382  99                cwd
00000383  52                push dx
00000384  50                push ax
00000385  A19A4B            mov ax,[0x4b9a]
00000388  99                cwd
00000389  50                push ax
0000038A  52                push dx
0000038B  8B56FA            mov dx,[bp-0x6]
0000038E  8B46F8            mov ax,[bp-0x8]
00000391  59                pop cx
00000392  5B                pop bx
00000393  9A4E4F0000        call 0x0:0x4f4e
00000398  52                push dx
00000399  50                push ax
0000039A  9A984F0000        call 0x0:0x4f98
0000039F  A3984B            mov [0x4b98],ax
000003A2  A1984B            mov ax,[0x4b98]
000003A5  BB0400            mov bx,0x4
000003A8  99                cwd
000003A9  F7FB              idiv bx
000003AB  BA0A00            mov dx,0xa
000003AE  F7EA              imul dx
000003B0  05C800            add ax,0xc8
000003B3  99                cwd
000003B4  52                push dx
000003B5  50                push ax
000003B6  FF76FE            push word [bp-0x2]
000003B9  FF76FC            push word [bp-0x4]
000003BC  9A894F0000        call 0x0:0x4f89
000003C1  A3F443            mov [0x43f4],ax
000003C4  33C0              xor ax,ax
000003C6  BAE803            mov dx,0x3e8
000003C9  50                push ax
000003CA  52                push dx
000003CB  FF76F6            push word [bp-0xa]
000003CE  FF76F4            push word [bp-0xc]
000003D1  9A894F0000        call 0x0:0x4f89
000003D6  50                push ax
000003D7  A1F443            mov ax,[0x43f4]
000003DA  52                push dx
000003DB  99                cwd
000003DC  5B                pop bx
000003DD  3BDA              cmp bx,dx
000003DF  5A                pop dx
000003E0  7C1B              jl 0x3fd
000003E2  7F04              jg 0x3e8
000003E4  3BD0              cmp dx,ax
000003E6  7615              jna 0x3fd
000003E8  33C0              xor ax,ax
000003EA  BAE803            mov dx,0x3e8
000003ED  50                push ax
000003EE  52                push dx
000003EF  FF76F6            push word [bp-0xa]
000003F2  FF76F4            push word [bp-0xc]
000003F5  9A894F0000        call 0x0:0x4f89
000003FA  A3F443            mov [0x43f4],ax
000003FD  833EF44301        cmp word [0x43f4],byte +0x1
00000402  7D06              jnl 0x40a
00000404  C706F4430100      mov word [0x43f4],0x1
0000040A  33C0              xor ax,ax
0000040C  BA4600            mov dx,0x46
0000040F  50                push ax
00000410  52                push dx
00000411  FF76F6            push word [bp-0xa]
00000414  FF76F4            push word [bp-0xc]
00000417  9A894F0000        call 0x0:0x4f89
0000041C  40                inc ax
0000041D  A3A048            mov [0x48a0],ax
00000420  33C0              xor ax,ax
00000422  BA0500            mov dx,0x5
00000425  50                push ax
00000426  52                push dx
00000427  FF76FA            push word [bp-0x6]
0000042A  FF76F8            push word [bp-0x8]
0000042D  9A984F0000        call 0x0:0x4f98
00000432  0BC2              or ax,dx
00000434  7508              jnz 0x43e
00000436  A1A048            mov ax,[0x48a0]
00000439  D1E0              shl ax,1
0000043B  A3A048            mov [0x48a0],ax
0000043E  A0F908            mov al,[0x8f9]
00000441  98                cbw
00000442  0BC0              or ax,ax
00000444  740C              jz 0x452
00000446  3D0100            cmp ax,0x1
00000449  741E              jz 0x469
0000044B  3D0200            cmp ax,0x2
0000044E  7430              jz 0x480
00000450  EB45              jmp short 0x497
00000452  33C0              xor ax,ax
00000454  BA1400            mov dx,0x14
00000457  50                push ax
00000458  52                push dx
00000459  FF76F6            push word [bp-0xa]
0000045C  FF76F4            push word [bp-0xc]
0000045F  9A894F0000        call 0x0:0x4f89
00000464  A3A234            mov [0x34a2],ax
00000467  EB2E              jmp short 0x497
00000469  33C0              xor ax,ax
0000046B  BA0A00            mov dx,0xa
0000046E  50                push ax
0000046F  52                push dx
00000470  FF76F6            push word [bp-0xa]
00000473  FF76F4            push word [bp-0xc]
00000476  9A894F0000        call 0x0:0x4f89
0000047B  A3A234            mov [0x34a2],ax
0000047E  EB17              jmp short 0x497
00000480  33C0              xor ax,ax
00000482  BA0500            mov dx,0x5
00000485  50                push ax
00000486  52                push dx
00000487  FF76F6            push word [bp-0xa]
0000048A  FF76F4            push word [bp-0xc]
0000048D  9A894F0000        call 0x0:0x4f89
00000492  A3A234            mov [0x34a2],ax
00000495  EB00              jmp short 0x497
00000497  833EA23414        cmp word [0x34a2],byte +0x14
0000049C  7D06              jnl 0x4a4
0000049E  C706A2341400      mov word [0x34a2],0x14
000004A4  A1A234            mov ax,[0x34a2]
000004A7  BB2000            mov bx,0x20
000004AA  99                cwd
000004AB  F7FB              idiv bx
000004AD  A3CC40            mov [0x40cc],ax
000004B0  33C0              xor ax,ax
000004B2  BA0A00            mov dx,0xa
000004B5  50                push ax
000004B6  52                push dx
000004B7  FF76F6            push word [bp-0xa]
000004BA  FF76F4            push word [bp-0xc]
000004BD  9A894F0000        call 0x0:0x4f89
000004C2  A34637            mov [0x3746],ax
000004C5  833E463764        cmp word [0x3746],byte +0x64
000004CA  7D06              jnl 0x4d2
000004CC  C70646376400      mov word [0x3746],0x64
000004D2  813E46372602      cmp word [0x3746],0x226
000004D8  7E06              jng 0x4e0
000004DA  C70646372602      mov word [0x3746],0x226
000004E0  8BE5              mov sp,bp
000004E2  5D                pop bp
000004E3  CB                retf

; ---- 1777:04E4 ----
000004E4  55                push bp
000004E5  8BEC              mov bp,sp
000004E7  833E0E09FF        cmp word [0x90e],byte -0x1
000004EC  7505              jnz 0x4f3
000004EE  9A1207730D        call 0xd73:0x712
000004F3  833E0E09FF        cmp word [0x90e],byte -0x1
000004F8  7505              jnz 0x4ff
000004FA  B80200            mov ax,0x2
000004FD  EB44              jmp short 0x543
000004FF  833E1009FF        cmp word [0x910],byte -0x1
00000504  7508              jnz 0x50e
00000506  9ACA04730D        call 0xd73:0x4ca
0000050B  A31009            mov [0x910],ax
0000050E  9A1E026C18        call 0x186c:0x21e
00000513  9A44056C18        call 0x186c:0x544
00000518  9AF800730D        call 0xd73:0xf8
0000051D  9AF903730D        call 0xd73:0x3f9
00000522  9AF506730D        call 0xd73:0x6f5
00000527  9A9B04730D        call 0xd73:0x49b
0000052C  9A02008B21        call 0x218b:0x2
00000531  9A0200670B        call 0xb67:0x2
00000536  0BC0              or ax,ax
00000538  7405              jz 0x53f
0000053A  B80100            mov ax,0x1
0000053D  EB04              jmp short 0x543
0000053F  33                db 0x33
