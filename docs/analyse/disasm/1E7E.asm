; segment 1E7E  file offset 0x21DE0  size 0xE50
00000000  8BE5              mov sp,bp
00000002  5D                pop bp
00000003  CB                retf

; ---- 1E7E:0004 ----
00000004  55                push bp
00000005  8BEC              mov bp,sp
00000007  83EC04            sub sp,byte +0x4
0000000A  56                push si
0000000B  57                push di
0000000C  9A1B00960A        call 0xa96:0x1b
00000011  B8C800            mov ax,0xc8
00000014  50                push ax
00000015  B84001            mov ax,0x140
00000018  50                push ax
00000019  33C0              xor ax,ax
0000001B  50                push ax
0000001C  33C0              xor ax,ax
0000001E  50                push ax
0000001F  B81800            mov ax,0x18
00000022  50                push ax
00000023  B81800            mov ax,0x18
00000026  50                push ax
00000027  B81000            mov ax,0x10
0000002A  50                push ax
0000002B  B82800            mov ax,0x28
0000002E  50                push ax
0000002F  9A6702960A        call 0xa96:0x267
00000034  83C410            add sp,byte +0x10
00000037  9AA806960A        call 0xa96:0x6a8
0000003C  33F6              xor si,si
0000003E  EB3B              jmp short 0x7b
00000040  9A1B00960A        call 0xa96:0x1b
00000045  33FF              xor di,di
00000047  EB27              jmp short 0x70
00000049  0BFF              or di,di
0000004B  7E19              jng 0x66
0000004D  83FF19            cmp di,byte +0x19
00000050  7D14              jnl 0x66
00000052  0BF6              or si,si
00000054  7E10              jng 0x66
00000056  83FE19            cmp si,byte +0x19
00000059  7D0B              jnl 0x66
0000005B  56                push si
0000005C  57                push di
0000005D  9AE8077E1E        call 0x1e7e:0x7e8
00000062  59                pop cx
00000063  59                pop cx
00000064  EB09              jmp short 0x6f
00000066  56                push si
00000067  57                push di
00000068  9AAA077E1E        call 0x1e7e:0x7aa
0000006D  59                pop cx
0000006E  59                pop cx
0000006F  47                inc di
00000070  83FF19            cmp di,byte +0x19
00000073  7ED4              jng 0x49
00000075  9AA806960A        call 0xa96:0x6a8
0000007A  46                inc si
0000007B  83FE19            cmp si,byte +0x19
0000007E  7EC0              jng 0x40
00000080  9A32017E1E        call 0x1e7e:0x132
00000085  9A9C047E1E        call 0x1e7e:0x49c
0000008A  B83200            mov ax,0x32
0000008D  50                push ax
0000008E  9ABA01FD21        call 0x21fd:0x1ba
00000093  59                pop cx
00000094  050A00            add ax,0xa
00000097  8946FC            mov [bp-0x4],ax
0000009A  9A1B00960A        call 0xa96:0x1b
0000009F  C746FE0000        mov word [bp-0x2],0x0
000000A4  EB3F              jmp short 0xe5
000000A6  B81700            mov ax,0x17
000000A9  50                push ax
000000AA  9ABA01FD21        call 0x21fd:0x1ba
000000AF  59                pop cx
000000B0  40                inc ax
000000B1  8BF8              mov di,ax
000000B3  B81700            mov ax,0x17
000000B6  50                push ax
000000B7  9ABA01FD21        call 0x21fd:0x1ba
000000BC  59                pop cx
000000BD  40                inc ax
000000BE  8BF0              mov si,ax
000000C0  8BC7              mov ax,di
000000C2  BA1A00            mov dx,0x1a
000000C5  F7EA              imul dx
000000C7  8BD8              mov bx,ax
000000C9  8A80F448          mov al,[bx+si+0x48f4]
000000CD  B400              mov ah,0x0
000000CF  BB0800            mov bx,0x8
000000D2  99                cwd
000000D3  F7FB              idiv bx
000000D5  0BC0              or ax,ax
000000D7  7509              jnz 0xe2
000000D9  56                push si
000000DA  57                push di
000000DB  9A12087E1E        call 0x1e7e:0x812
000000E0  59                pop cx
000000E1  59                pop cx
000000E2  FF46FE            inc word [bp-0x2]
000000E5  8B46FE            mov ax,[bp-0x2]
000000E8  3B46FC            cmp ax,[bp-0x4]
000000EB  7CB9              jl 0xa6
000000ED  9AA806960A        call 0xa96:0x6a8
000000F2  9A1B00960A        call 0xa96:0x1b
000000F7  C606AA3700        mov byte [0x37aa],0x0
000000FC  EB18              jmp short 0x116
000000FE  B000              mov al,0x0
00000100  50                push ax
00000101  A0AA37            mov al,[0x37aa]
00000104  98                cbw
00000105  50                push ax
00000106  9AF1087E1E        call 0x1e7e:0x8f1
0000010B  59                pop cx
0000010C  59                pop cx
0000010D  9AC4097E1E        call 0x1e7e:0x9c4
00000112  FE06AA37          inc byte [0x37aa]
00000116  803EAA3704        cmp byte [0x37aa],0x4
0000011B  7CE1              jl 0xfe
0000011D  9AA806960A        call 0xa96:0x6a8
00000122  FF362E08          push word [0x82e]
00000126  9A3F0C960A        call 0xa96:0xc3f
0000012B  59                pop cx
0000012C  5F                pop di
0000012D  5E                pop si
0000012E  8BE5              mov sp,bp
00000130  5D                pop bp
00000131  CB                retf

; ---- 1E7E:0132 ----
00000132  55                push bp
00000133  8BEC              mov bp,sp
00000135  83EC0C            sub sp,byte +0xc
00000138  56                push si
00000139  57                push di
0000013A  33F6              xor si,si
0000013C  EB18              jmp short 0x156
0000013E  33FF              xor di,di
00000140  EB0E              jmp short 0x150
00000142  8BDF              mov bx,di
00000144  D1E3              shl bx,1
00000146  D1E3              shl bx,1
00000148  D1E3              shl bx,1
0000014A  C680784401        mov byte [bx+si+0x4478],0x1
0000014F  47                inc di
00000150  83FF07            cmp di,byte +0x7
00000153  7EED              jng 0x142
00000155  46                inc si
00000156  83FE07            cmp si,byte +0x7
00000159  7EE3              jng 0x13e
0000015B  B80400            mov ax,0x4
0000015E  50                push ax
0000015F  9ABA01FD21        call 0x21fd:0x1ba
00000164  59                pop cx
00000165  0BC0              or ax,ax
00000167  7507              jnz 0x170
00000169  C606663000        mov byte [0x3066],0x0
0000016E  EB0D              jmp short 0x17d
00000170  B83000            mov ax,0x30
00000173  50                push ax
00000174  9ABA01FD21        call 0x21fd:0x1ba
00000179  59                pop cx
0000017A  A26630            mov [0x3066],al
0000017D  C746F40000        mov word [bp-0xc],0x0
00000182  EB51              jmp short 0x1d5
00000184  B80700            mov ax,0x7
00000187  50                push ax
00000188  9ABA01FD21        call 0x21fd:0x1ba
0000018D  59                pop cx
0000018E  8BF8              mov di,ax
00000190  B80700            mov ax,0x7
00000193  50                push ax
00000194  9ABA01FD21        call 0x21fd:0x1ba
00000199  59                pop cx
0000019A  8BF0              mov si,ax
0000019C  83FF01            cmp di,byte +0x1
0000019F  7E05              jng 0x1a6
000001A1  83FF06            cmp di,byte +0x6
000001A4  7C0C              jl 0x1b2
000001A6  83FE01            cmp si,byte +0x1
000001A9  7E05              jng 0x1b0
000001AB  83FE06            cmp si,byte +0x6
000001AE  7C02              jl 0x1b2
000001B0  EB11              jmp short 0x1c3
000001B2  8BDF              mov bx,di
000001B4  D1E3              shl bx,1
000001B6  D1E3              shl bx,1
000001B8  D1E3              shl bx,1
000001BA  80B8784401        cmp byte [bx+si+0x4478],0x1
000001BF  7502              jnz 0x1c3
000001C1  EB02              jmp short 0x1c5
000001C3  EBBF              jmp short 0x184
000001C5  8BDF              mov bx,di
000001C7  D1E3              shl bx,1
000001C9  D1E3              shl bx,1
000001CB  D1E3              shl bx,1
000001CD  C680784400        mov byte [bx+si+0x4478],0x0
000001D2  FF46F4            inc word [bp-0xc]
000001D5  A06630            mov al,[0x3066]
000001D8  B400              mov ah,0x0
000001DA  3B46F4            cmp ax,[bp-0xc]
000001DD  7FA5              jg 0x184
000001DF  33F6              xor si,si
000001E1  E9AA02            jmp 0x48e
000001E4  33FF              xor di,di
000001E6  E99C02            jmp 0x485
000001E9  8BDF              mov bx,di
000001EB  D1E3              shl bx,1
000001ED  D1E3              shl bx,1
000001EF  D1E3              shl bx,1
000001F1  80B8784401        cmp byte [bx+si+0x4478],0x1
000001F6  7503              jnz 0x1fb
000001F8  E98902            jmp 0x484
000001FB  0BFF              or di,di
000001FD  7E13              jng 0x212
000001FF  8BDF              mov bx,di
00000201  D1E3              shl bx,1
00000203  D1E3              shl bx,1
00000205  D1E3              shl bx,1
00000207  80B8704401        cmp byte [bx+si+0x4470],0x1
0000020C  7504              jnz 0x212
0000020E  B001              mov al,0x1
00000210  EB02              jmp short 0x214
00000212  B000              mov al,0x0
00000214  8846F9            mov [bp-0x7],al
00000217  83FF07            cmp di,byte +0x7
0000021A  7D13              jnl 0x22f
0000021C  8BDF              mov bx,di
0000021E  D1E3              shl bx,1
00000220  D1E3              shl bx,1
00000222  D1E3              shl bx,1
00000224  80B8804401        cmp byte [bx+si+0x4480],0x1
00000229  7504              jnz 0x22f
0000022B  B001              mov al,0x1
0000022D  EB02              jmp short 0x231
0000022F  B000              mov al,0x0
00000231  8846FA            mov [bp-0x6],al
00000234  0BF6              or si,si
00000236  7E13              jng 0x24b
00000238  8BDF              mov bx,di
0000023A  D1E3              shl bx,1
0000023C  D1E3              shl bx,1
0000023E  D1E3              shl bx,1
00000240  80B8774401        cmp byte [bx+si+0x4477],0x1
00000245  7504              jnz 0x24b
00000247  B001              mov al,0x1
00000249  EB02              jmp short 0x24d
0000024B  B000              mov al,0x0
0000024D  8846FC            mov [bp-0x4],al
00000250  83FE07            cmp si,byte +0x7
00000253  7D13              jnl 0x268
00000255  8BDF              mov bx,di
00000257  D1E3              shl bx,1
00000259  D1E3              shl bx,1
0000025B  D1E3              shl bx,1
0000025D  80B8794401        cmp byte [bx+si+0x4479],0x1
00000262  7504              jnz 0x268
00000264  B001              mov al,0x1
00000266  EB02              jmp short 0x26a
00000268  B000              mov al,0x0
0000026A  8846FB            mov [bp-0x5],al
0000026D  8A46FC            mov al,[bp-0x4]
00000270  D0E0              shl al,1
00000272  8A56F9            mov dl,[bp-0x7]
00000275  02D0              add dl,al
00000277  8A46FA            mov al,[bp-0x6]
0000027A  D0E0              shl al,1
0000027C  D0E0              shl al,1
0000027E  02D0              add dl,al
00000280  8A46FB            mov al,[bp-0x5]
00000283  D0E0              shl al,1
00000285  D0E0              shl al,1
00000287  D0E0              shl al,1
00000289  02D0              add dl,al
0000028B  8856FE            mov [bp-0x2],dl
0000028E  0BFF              or di,di
00000290  7E17              jng 0x2a9
00000292  0BF6              or si,si
00000294  7E13              jng 0x2a9
00000296  8BDF              mov bx,di
00000298  D1E3              shl bx,1
0000029A  D1E3              shl bx,1
0000029C  D1E3              shl bx,1
0000029E  80B86F4401        cmp byte [bx+si+0x446f],0x1
000002A3  7504              jnz 0x2a9
000002A5  B001              mov al,0x1
000002A7  EB02              jmp short 0x2ab
000002A9  B000              mov al,0x0
000002AB  8846F8            mov [bp-0x8],al
000002AE  0BFF              or di,di
000002B0  7E18              jng 0x2ca
000002B2  83FE07            cmp si,byte +0x7
000002B5  7D13              jnl 0x2ca
000002B7  8BDF              mov bx,di
000002B9  D1E3              shl bx,1
000002BB  D1E3              shl bx,1
000002BD  D1E3              shl bx,1
000002BF  80B8714401        cmp byte [bx+si+0x4471],0x1
000002C4  7504              jnz 0x2ca
000002C6  B001              mov al,0x1
000002C8  EB02              jmp short 0x2cc
000002CA  B000              mov al,0x0
000002CC  8846F7            mov [bp-0x9],al
000002CF  8A46FE            mov al,[bp-0x2]
000002D2  24FB              and al,0xfb
000002D4  8A56F8            mov dl,[bp-0x8]
000002D7  B104              mov cl,0x4
000002D9  D2E2              shl dl,cl
000002DB  02C2              add al,dl
000002DD  8A56F7            mov dl,[bp-0x9]
000002E0  B105              mov cl,0x5
000002E2  D2E2              shl dl,cl
000002E4  02C2              add al,dl
000002E6  8846FD            mov [bp-0x3],al
000002E9  807EFD0B          cmp byte [bp-0x3],0xb
000002ED  7506              jnz 0x2f5
000002EF  C646FFC1          mov byte [bp-0x1],0xc1
000002F3  EB62              jmp short 0x357
000002F5  807EFD19          cmp byte [bp-0x3],0x19
000002F9  740C              jz 0x307
000002FB  807EFD09          cmp byte [bp-0x3],0x9
000002FF  7406              jz 0x307
00000301  807EFD1B          cmp byte [bp-0x3],0x1b
00000305  7506              jnz 0x30d
00000307  C646FF40          mov byte [bp-0x1],0x40
0000030B  EB4A              jmp short 0x357
0000030D  807EFD23          cmp byte [bp-0x3],0x23
00000311  740C              jz 0x31f
00000313  807EFD03          cmp byte [bp-0x3],0x3
00000317  7406              jz 0x31f
00000319  807EFD2B          cmp byte [bp-0x3],0x2b
0000031D  7506              jnz 0x325
0000031F  C646FF01          mov byte [bp-0x1],0x1
00000323  EB32              jmp short 0x357
00000325  B80200            mov ax,0x2
00000328  50                push ax
00000329  9ABA01FD21        call 0x21fd:0x1ba
0000032E  59                pop cx
0000032F  0BC0              or ax,ax
00000331  7506              jnz 0x339
00000333  C646FF00          mov byte [bp-0x1],0x0
00000337  EB1E              jmp short 0x357
00000339  B80100            mov ax,0x1
0000033C  50                push ax
0000033D  9ABA01FD21        call 0x21fd:0x1ba
00000342  59                pop cx
00000343  B104              mov cl,0x4
00000345  D3E0              shl ax,cl
00000347  8A56FE            mov dl,[bp-0x2]
0000034A  B600              mov dh,0x0
0000034C  03C2              add ax,dx
0000034E  8BD8              mov bx,ax
00000350  8A87320F          mov al,[bx+0xf32]
00000354  8846FF            mov [bp-0x1],al
00000357  9A1B00960A        call 0xa96:0x1b
0000035C  F646FF01          test byte [bp-0x1],0x1
00000360  7519              jnz 0x37b
00000362  8BC6              mov ax,si
00000364  BA0300            mov dx,0x3
00000367  F7EA              imul dx
00000369  40                inc ax
0000036A  50                push ax
0000036B  8BC7              mov ax,di
0000036D  BA0300            mov dx,0x3
00000370  F7EA              imul dx
00000372  40                inc ax
00000373  50                push ax
00000374  9AAA077E1E        call 0x1e7e:0x7aa
00000379  59                pop cx
0000037A  59                pop cx
0000037B  F646FF02          test byte [bp-0x1],0x2
0000037F  751A              jnz 0x39b
00000381  8BC6              mov ax,si
00000383  BA0300            mov dx,0x3
00000386  F7EA              imul dx
00000388  40                inc ax
00000389  50                push ax
0000038A  8BC7              mov ax,di
0000038C  BA0300            mov dx,0x3
0000038F  F7EA              imul dx
00000391  40                inc ax
00000392  40                inc ax
00000393  50                push ax
00000394  9AAA077E1E        call 0x1e7e:0x7aa
00000399  59                pop cx
0000039A  59                pop cx
0000039B  F646FF04          test byte [bp-0x1],0x4
0000039F  751B              jnz 0x3bc
000003A1  8BC6              mov ax,si
000003A3  BA0300            mov dx,0x3
000003A6  F7EA              imul dx
000003A8  40                inc ax
000003A9  50                push ax
000003AA  8BC7              mov ax,di
000003AC  BA0300            mov dx,0x3
000003AF  F7EA              imul dx
000003B1  050300            add ax,0x3
000003B4  50                push ax
000003B5  9AAA077E1E        call 0x1e7e:0x7aa
000003BA  59                pop cx
000003BB  59                pop cx
000003BC  F646FF08          test byte [bp-0x1],0x8
000003C0  751C              jnz 0x3de
000003C2  8BC6              mov ax,si
000003C4  BA0300            mov dx,0x3
000003C7  F7EA              imul dx
000003C9  40                inc ax
000003CA  40                inc ax
000003CB  50                push ax
000003CC  8BC7              mov ax,di
000003CE  BA0300            mov dx,0x3
000003D1  F7EA              imul dx
000003D3  050300            add ax,0x3
000003D6  50                push ax
000003D7  9AAA077E1E        call 0x1e7e:0x7aa
000003DC  59                pop cx
000003DD  59                pop cx
000003DE  F646FF10          test byte [bp-0x1],0x10
000003E2  751D              jnz 0x401
000003E4  8BC6              mov ax,si
000003E6  BA0300            mov dx,0x3
000003E9  F7EA              imul dx
000003EB  050300            add ax,0x3
000003EE  50                push ax
000003EF  8BC7              mov ax,di
000003F1  BA0300            mov dx,0x3
000003F4  F7EA              imul dx
000003F6  050300            add ax,0x3
000003F9  50                push ax
000003FA  9AAA077E1E        call 0x1e7e:0x7aa
000003FF  59                pop cx
00000400  59                pop cx
00000401  F646FF20          test byte [bp-0x1],0x20
00000405  751C              jnz 0x423
00000407  8BC6              mov ax,si
00000409  BA0300            mov dx,0x3
0000040C  F7EA              imul dx
0000040E  050300            add ax,0x3
00000411  50                push ax
00000412  8BC7              mov ax,di
00000414  BA0300            mov dx,0x3
00000417  F7EA              imul dx
00000419  40                inc ax
0000041A  40                inc ax
0000041B  50                push ax
0000041C  9AAA077E1E        call 0x1e7e:0x7aa
00000421  59                pop cx
00000422  59                pop cx
00000423  F646FF40          test byte [bp-0x1],0x40
00000427  751B              jnz 0x444
00000429  8BC6              mov ax,si
0000042B  BA0300            mov dx,0x3
0000042E  F7EA              imul dx
00000430  050300            add ax,0x3
00000433  50                push ax
00000434  8BC7              mov ax,di
00000436  BA0300            mov dx,0x3
00000439  F7EA              imul dx
0000043B  40                inc ax
0000043C  50                push ax
0000043D  9AAA077E1E        call 0x1e7e:0x7aa
00000442  59                pop cx
00000443  59                pop cx
00000444  F646FF80          test byte [bp-0x1],0x80
00000448  751A              jnz 0x464
0000044A  8BC6              mov ax,si
0000044C  BA0300            mov dx,0x3
0000044F  F7EA              imul dx
00000451  40                inc ax
00000452  40                inc ax
00000453  50                push ax
00000454  8BC7              mov ax,di
00000456  BA0300            mov dx,0x3
00000459  F7EA              imul dx
0000045B  40                inc ax
0000045C  50                push ax
0000045D  9AAA077E1E        call 0x1e7e:0x7aa
00000462  59                pop cx
00000463  59                pop cx
00000464  8BC6              mov ax,si
00000466  BA0300            mov dx,0x3
00000469  F7EA              imul dx
0000046B  40                inc ax
0000046C  40                inc ax
0000046D  50                push ax
0000046E  8BC7              mov ax,di
00000470  BA0300            mov dx,0x3
00000473  F7EA              imul dx
00000475  40                inc ax
00000476  40                inc ax
00000477  50                push ax
00000478  9AAA077E1E        call 0x1e7e:0x7aa
0000047D  59                pop cx
0000047E  59                pop cx
0000047F  9AA806960A        call 0xa96:0x6a8
00000484  47                inc di
00000485  83FF07            cmp di,byte +0x7
00000488  7F03              jg 0x48d
0000048A  E95CFD            jmp 0x1e9
0000048D  46                inc si
0000048E  83FE07            cmp si,byte +0x7
00000491  7F03              jg 0x496
00000493  E94EFD            jmp 0x1e4
00000496  5F                pop di
00000497  5E                pop si
00000498  8BE5              mov sp,bp
0000049A  5D                pop bp
0000049B  CB                retf

; ---- 1E7E:049C ----
0000049C  55                push bp
0000049D  8BEC              mov bp,sp
0000049F  83EC08            sub sp,byte +0x8
000004A2  56                push si
000004A3  57                push di
000004A4  33FF              xor di,di
000004A6  E9F001            jmp 0x699
000004A9  9A1B00960A        call 0xa96:0x1b
000004AE  33F6              xor si,si
000004B0  E9D801            jmp 0x68b
000004B3  8BC6              mov ax,si
000004B5  BA1A00            mov dx,0x1a
000004B8  F7EA              imul dx
000004BA  8BD8              mov bx,ax
000004BC  8A81F448          mov al,[bx+di+0x48f4]
000004C0  B400              mov ah,0x0
000004C2  BB0800            mov bx,0x8
000004C5  99                cwd
000004C6  F7FB              idiv bx
000004C8  0BC0              or ax,ax
000004CA  7503              jnz 0x4cf
000004CC  E9BB01            jmp 0x68a
000004CF  0BF6              or si,si
000004D1  7E1D              jng 0x4f0
000004D3  8BC6              mov ax,si
000004D5  BA1A00            mov dx,0x1a
000004D8  F7EA              imul dx
000004DA  8BD8              mov bx,ax
000004DC  8A81DA48          mov al,[bx+di+0x48da]
000004E0  B400              mov ah,0x0
000004E2  BB0800            mov bx,0x8
000004E5  99                cwd
000004E6  F7FB              idiv bx
000004E8  0BC0              or ax,ax
000004EA  7504              jnz 0x4f0
000004EC  B000              mov al,0x0
000004EE  EB02              jmp short 0x4f2
000004F0  B001              mov al,0x1
000004F2  8846FB            mov [bp-0x5],al
000004F5  83FE19            cmp si,byte +0x19
000004F8  7D1D              jnl 0x517
000004FA  8BC6              mov ax,si
000004FC  BA1A00            mov dx,0x1a
000004FF  F7EA              imul dx
00000501  8BD8              mov bx,ax
00000503  8A810E49          mov al,[bx+di+0x490e]
00000507  B400              mov ah,0x0
00000509  BB0800            mov bx,0x8
0000050C  99                cwd
0000050D  F7FB              idiv bx
0000050F  0BC0              or ax,ax
00000511  7504              jnz 0x517
00000513  B000              mov al,0x0
00000515  EB02              jmp short 0x519
00000517  B001              mov al,0x1
00000519  8846FA            mov [bp-0x6],al
0000051C  0BFF              or di,di
0000051E  7E1D              jng 0x53d
00000520  8BC6              mov ax,si
00000522  BA1A00            mov dx,0x1a
00000525  F7EA              imul dx
00000527  8BD8              mov bx,ax
00000529  8A81F348          mov al,[bx+di+0x48f3]
0000052D  B400              mov ah,0x0
0000052F  BB0800            mov bx,0x8
00000532  99                cwd
00000533  F7FB              idiv bx
00000535  0BC0              or ax,ax
00000537  7504              jnz 0x53d
00000539  B000              mov al,0x0
0000053B  EB02              jmp short 0x53f
0000053D  B001              mov al,0x1
0000053F  8846F9            mov [bp-0x7],al
00000542  83FF19            cmp di,byte +0x19
00000545  7D1D              jnl 0x564
00000547  8BC6              mov ax,si
00000549  BA1A00            mov dx,0x1a
0000054C  F7EA              imul dx
0000054E  8BD8              mov bx,ax
00000550  8A81F548          mov al,[bx+di+0x48f5]
00000554  B400              mov ah,0x0
00000556  BB0800            mov bx,0x8
00000559  99                cwd
0000055A  F7FB              idiv bx
0000055C  0BC0              or ax,ax
0000055E  7504              jnz 0x564
00000560  B000              mov al,0x0
00000562  EB02              jmp short 0x566
00000564  B001              mov al,0x1
00000566  8846F8            mov [bp-0x8],al
00000569  8A46F8            mov al,[bp-0x8]
0000056C  B400              mov ah,0x0
0000056E  D1E0              shl ax,1
00000570  8A56FB            mov dl,[bp-0x5]
00000573  B600              mov dh,0x0
00000575  03D0              add dx,ax
00000577  8A46FA            mov al,[bp-0x6]
0000057A  B400              mov ah,0x0
0000057C  D1E0              shl ax,1
0000057E  D1E0              shl ax,1
00000580  03D0              add dx,ax
00000582  8A46F9            mov al,[bp-0x7]
00000585  B400              mov ah,0x0
00000587  D1E0              shl ax,1
00000589  D1E0              shl ax,1
0000058B  D1E0              shl ax,1
0000058D  03D0              add dx,ax
0000058F  8956FC            mov [bp-0x4],dx
00000592  C746FE0000        mov word [bp-0x2],0x0
00000597  EB2B              jmp short 0x5c4
00000599  8B5EFE            mov bx,[bp-0x2]
0000059C  D1E3              shl bx,1
0000059E  8A87520F          mov al,[bx+0xf52]
000005A2  B400              mov ah,0x0
000005A4  3B46FC            cmp ax,[bp-0x4]
000005A7  7518              jnz 0x5c1
000005A9  8B5EFE            mov bx,[bp-0x2]
000005AC  D1E3              shl bx,1
000005AE  8A87530F          mov al,[bx+0xf53]
000005B2  B400              mov ah,0x0
000005B4  50                push ax
000005B5  57                push di
000005B6  56                push si
000005B7  9AA7067E1E        call 0x1e7e:0x6a7
000005BC  83C406            add sp,byte +0x6
000005BF  EB09              jmp short 0x5ca
000005C1  FF46FE            inc word [bp-0x2]
000005C4  837EFE07          cmp word [bp-0x2],byte +0x7
000005C8  7ECF              jng 0x599
000005CA  837EFC0F          cmp word [bp-0x4],byte +0xf
000005CE  7403              jz 0x5d3
000005D0  E9B700            jmp 0x68a
000005D3  0BFF              or di,di
000005D5  7E57              jng 0x62e
000005D7  0BF6              or si,si
000005D9  7E27              jng 0x602
000005DB  8BC6              mov ax,si
000005DD  BA1A00            mov dx,0x1a
000005E0  F7EA              imul dx
000005E2  8BD8              mov bx,ax
000005E4  8A81D948          mov al,[bx+di+0x48d9]
000005E8  B400              mov ah,0x0
000005EA  BB0800            mov bx,0x8
000005ED  99                cwd
000005EE  F7FB              idiv bx
000005F0  0BC0              or ax,ax
000005F2  750E              jnz 0x602
000005F4  B80400            mov ax,0x4
000005F7  50                push ax
000005F8  57                push di
000005F9  56                push si
000005FA  9AA7067E1E        call 0x1e7e:0x6a7
000005FF  83C406            add sp,byte +0x6
00000602  83FE19            cmp si,byte +0x19
00000605  7D27              jnl 0x62e
00000607  8BC6              mov ax,si
00000609  BA1A00            mov dx,0x1a
0000060C  F7EA              imul dx
0000060E  8BD8              mov bx,ax
00000610  8A810D49          mov al,[bx+di+0x490d]
00000614  B400              mov ah,0x0
00000616  BB0800            mov bx,0x8
00000619  99                cwd
0000061A  F7FB              idiv bx
0000061C  0BC0              or ax,ax
0000061E  750E              jnz 0x62e
00000620  B80500            mov ax,0x5
00000623  50                push ax
00000624  57                push di
00000625  56                push si
00000626  9AA7067E1E        call 0x1e7e:0x6a7
0000062B  83C406            add sp,byte +0x6
0000062E  83FF19            cmp di,byte +0x19
00000631  7D57              jnl 0x68a
00000633  0BF6              or si,si
00000635  7E27              jng 0x65e
00000637  8BC6              mov ax,si
00000639  BA1A00            mov dx,0x1a
0000063C  F7EA              imul dx
0000063E  8BD8              mov bx,ax
00000640  8A81DB48          mov al,[bx+di+0x48db]
00000644  B400              mov ah,0x0
00000646  BB0800            mov bx,0x8
00000649  99                cwd
0000064A  F7FB              idiv bx
0000064C  0BC0              or ax,ax
0000064E  750E              jnz 0x65e
00000650  B80600            mov ax,0x6
00000653  50                push ax
00000654  57                push di
00000655  56                push si
00000656  9AA7067E1E        call 0x1e7e:0x6a7
0000065B  83C406            add sp,byte +0x6
0000065E  83FE19            cmp si,byte +0x19
00000661  7D27              jnl 0x68a
00000663  8BC6              mov ax,si
00000665  BA1A00            mov dx,0x1a
00000668  F7EA              imul dx
0000066A  8BD8              mov bx,ax
0000066C  8A810F49          mov al,[bx+di+0x490f]
00000670  B400              mov ah,0x0
00000672  BB0800            mov bx,0x8
00000675  99                cwd
00000676  F7FB              idiv bx
00000678  0BC0              or ax,ax
0000067A  750E              jnz 0x68a
0000067C  B80700            mov ax,0x7
0000067F  50                push ax
00000680  57                push di
00000681  56                push si
00000682  9AA7067E1E        call 0x1e7e:0x6a7
00000687  83C406            add sp,byte +0x6
0000068A  46                inc si
0000068B  83FE19            cmp si,byte +0x19
0000068E  7F03              jg 0x693
00000690  E920FE            jmp 0x4b3
00000693  9AA806960A        call 0xa96:0x6a8
00000698  47                inc di
00000699  83FF19            cmp di,byte +0x19
0000069C  7F03              jg 0x6a1
0000069E  E908FE            jmp 0x4a9
000006A1  5F                pop di
000006A2  5E                pop si
000006A3  8BE5              mov sp,bp
000006A5  5D                pop bp
000006A6  CB                retf

; ---- 1E7E:06A7 ----
000006A7  55                push bp
000006A8  8BEC              mov bp,sp
000006AA  4C                dec sp
000006AB  4C                dec sp
000006AC  56                push si
000006AD  57                push di
000006AE  8B7E06            mov di,[bp+0x6]
000006B1  8B7608            mov si,[bp+0x8]
000006B4  8A460A            mov al,[bp+0xa]
000006B7  040D              add al,0xd
000006B9  D0E0              shl al,1
000006BB  D0E0              shl al,1
000006BD  D0E0              shl al,1
000006BF  8846FF            mov [bp-0x1],al
000006C2  837E0A07          cmp word [bp+0xa],byte +0x7
000006C6  7F25              jg 0x6ed
000006C8  8A46FF            mov al,[bp-0x1]
000006CB  50                push ax
000006CC  56                push si
000006CD  57                push di
000006CE  8B5E0A            mov bx,[bp+0xa]
000006D1  8A876A0F          mov al,[bx+0xf6a]
000006D5  B400              mov ah,0x0
000006D7  50                push ax
000006D8  8B5E0A            mov bx,[bp+0xa]
000006DB  8A87620F          mov al,[bx+0xf62]
000006DF  B400              mov ah,0x0
000006E1  50                push ax
000006E2  9AB30D7E1E        call 0x1e7e:0xdb3
000006E7  83C40A            add sp,byte +0xa
000006EA  E9B700            jmp 0x7a4
000006ED  837E0A08          cmp word [bp+0xa],byte +0x8
000006F1  7529              jnz 0x71c
000006F3  8A46FF            mov al,[bp-0x1]
000006F6  50                push ax
000006F7  56                push si
000006F8  57                push di
000006F9  B80800            mov ax,0x8
000006FC  50                push ax
000006FD  B80200            mov ax,0x2
00000700  50                push ax
00000701  9ABA01FD21        call 0x21fd:0x1ba
00000706  59                pop cx
00000707  D1E0              shl ax,1
00000709  D1E0              shl ax,1
0000070B  D1E0              shl ax,1
0000070D  052800            add ax,0x28
00000710  50                push ax
00000711  9AB30D7E1E        call 0x1e7e:0xdb3
00000716  83C40A            add sp,byte +0xa
00000719  E98800            jmp 0x7a4
0000071C  837E0A09          cmp word [bp+0xa],byte +0x9
00000720  7528              jnz 0x74a
00000722  8A46FF            mov al,[bp-0x1]
00000725  50                push ax
00000726  56                push si
00000727  57                push di
00000728  B80200            mov ax,0x2
0000072B  50                push ax
0000072C  9ABA01FD21        call 0x21fd:0x1ba
00000731  59                pop cx
00000732  D1E0              shl ax,1
00000734  D1E0              shl ax,1
00000736  D1E0              shl ax,1
00000738  051000            add ax,0x10
0000073B  50                push ax
0000073C  B84000            mov ax,0x40
0000073F  50                push ax
00000740  9AB30D7E1E        call 0x1e7e:0xdb3
00000745  83C40A            add sp,byte +0xa
00000748  EB5A              jmp short 0x7a4
0000074A  837E0A0A          cmp word [bp+0xa],byte +0xa
0000074E  7528              jnz 0x778
00000750  8A46FF            mov al,[bp-0x1]
00000753  50                push ax
00000754  56                push si
00000755  57                push di
00000756  B82800            mov ax,0x28
00000759  50                push ax
0000075A  B80200            mov ax,0x2
0000075D  50                push ax
0000075E  9ABA01FD21        call 0x21fd:0x1ba
00000763  59                pop cx
00000764  D1E0              shl ax,1
00000766  D1E0              shl ax,1
00000768  D1E0              shl ax,1
0000076A  052800            add ax,0x28
0000076D  50                push ax
0000076E  9AB30D7E1E        call 0x1e7e:0xdb3
00000773  83C40A            add sp,byte +0xa
00000776  EB2C              jmp short 0x7a4
00000778  837E0A0B          cmp word [bp+0xa],byte +0xb
0000077C  7526              jnz 0x7a4
0000077E  8A46FF            mov al,[bp-0x1]
00000781  50                push ax
00000782  56                push si
00000783  57                push di
00000784  B80200            mov ax,0x2
00000787  50                push ax
00000788  9ABA01FD21        call 0x21fd:0x1ba
0000078D  59                pop cx
0000078E  D1E0              shl ax,1
00000790  D1E0              shl ax,1
00000792  D1E0              shl ax,1
00000794  051000            add ax,0x10
00000797  50                push ax
00000798  B82000            mov ax,0x20
0000079B  50                push ax
0000079C  9AB30D7E1E        call 0x1e7e:0xdb3
000007A1  83C40A            add sp,byte +0xa
000007A4  5F                pop di
000007A5  5E                pop si
000007A6  8BE5              mov sp,bp
000007A8  5D                pop bp
000007A9  CB                retf

; ---- 1E7E:07AA ----
000007AA  55                push bp
000007AB  8BEC              mov bp,sp
000007AD  B030              mov al,0x30
000007AF  50                push ax
000007B0  FF7608            push word [bp+0x8]
000007B3  FF7606            push word [bp+0x6]
000007B6  B80200            mov ax,0x2
000007B9  50                push ax
000007BA  9ABA01FD21        call 0x21fd:0x1ba
000007BF  59                pop cx
000007C0  D1E0              shl ax,1
000007C2  D1E0              shl ax,1
000007C4  D1E0              shl ax,1
000007C6  051000            add ax,0x10
000007C9  50                push ax
000007CA  B80200            mov ax,0x2
000007CD  50                push ax
000007CE  9ABA01FD21        call 0x21fd:0x1ba
000007D3  59                pop cx
000007D4  D1E0              shl ax,1
000007D6  D1E0              shl ax,1
000007D8  D1E0              shl ax,1
000007DA  052800            add ax,0x28
000007DD  50                push ax
000007DE  9AB30D7E1E        call 0x1e7e:0xdb3
000007E3  83C40A            add sp,byte +0xa
000007E6  5D                pop bp
000007E7  CB                retf

; ---- 1E7E:07E8 ----
000007E8  55                push bp
000007E9  8BEC              mov bp,sp
000007EB  B000              mov al,0x0
000007ED  50                push ax
000007EE  FF7608            push word [bp+0x8]
000007F1  FF7606            push word [bp+0x6]
000007F4  33C0              xor ax,ax
000007F6  50                push ax
000007F7  B80300            mov ax,0x3
000007FA  50                push ax
000007FB  9ABA01FD21        call 0x21fd:0x1ba
00000800  59                pop cx
00000801  D1E0              shl ax,1
00000803  D1E0              shl ax,1
00000805  D1E0              shl ax,1
00000807  50                push ax
00000808  9AB30D7E1E        call 0x1e7e:0xdb3
0000080D  83C40A            add sp,byte +0xa
00000810  5D                pop bp
00000811  CB                retf

; ---- 1E7E:0812 ----
00000812  55                push bp
00000813  8BEC              mov bp,sp
00000815  B011              mov al,0x11
00000817  50                push ax
00000818  FF7608            push word [bp+0x8]
0000081B  FF7606            push word [bp+0x6]
0000081E  33C0              xor ax,ax
00000820  50                push ax
00000821  B80200            mov ax,0x2
00000824  50                push ax
00000825  9ABA01FD21        call 0x21fd:0x1ba
0000082A  59                pop cx
0000082B  D1E0              shl ax,1
0000082D  D1E0              shl ax,1
0000082F  D1E0              shl ax,1
00000831  052800            add ax,0x28
00000834  50                push ax
00000835  9AB30D7E1E        call 0x1e7e:0xdb3
0000083A  83C40A            add sp,byte +0xa
0000083D  5D                pop bp
0000083E  CB                retf

; ---- 1E7E:083F ----
0000083F  55                push bp
00000840  8BEC              mov bp,sp
00000842  8A460A            mov al,[bp+0xa]
00000845  0407              add al,0x7
00000847  D0E0              shl al,1
00000849  D0E0              shl al,1
0000084B  D0E0              shl al,1
0000084D  0403              add al,0x3
0000084F  50                push ax
00000850  FF7608            push word [bp+0x8]
00000853  FF7606            push word [bp+0x6]
00000856  33C0              xor ax,ax
00000858  50                push ax
00000859  8B460A            mov ax,[bp+0xa]
0000085C  D1E0              shl ax,1
0000085E  D1E0              shl ax,1
00000860  D1E0              shl ax,1
00000862  055000            add ax,0x50
00000865  50                push ax
00000866  9AB30D7E1E        call 0x1e7e:0xdb3
0000086B  83C40A            add sp,byte +0xa
0000086E  5D                pop bp
0000086F  CB                retf

; ---- 1E7E:0870 ----
00000870  55                push bp
00000871  8BEC              mov bp,sp
00000873  B01A              mov al,0x1a
00000875  50                push ax
00000876  FF7608            push word [bp+0x8]
00000879  FF7606            push word [bp+0x6]
0000087C  33C0              xor ax,ax
0000087E  50                push ax
0000087F  B82000            mov ax,0x20
00000882  50                push ax
00000883  9AB30D7E1E        call 0x1e7e:0xdb3
00000888  83C40A            add sp,byte +0xa
0000088B  5D                pop bp
0000088C  CB                retf

; ---- 1E7E:088D ----
0000088D  55                push bp
0000088E  8BEC              mov bp,sp
00000890  B023              mov al,0x23
00000892  50                push ax
00000893  FF7608            push word [bp+0x8]
00000896  FF7606            push word [bp+0x6]
00000899  B83000            mov ax,0x30
0000089C  50                push ax
0000089D  A0AA37            mov al,[0x37aa]
000008A0  98                cbw
000008A1  D1E0              shl ax,1
000008A3  D1E0              shl ax,1
000008A5  D1E0              shl ax,1
000008A7  052000            add ax,0x20
000008AA  50                push ax
000008AB  9AB30D7E1E        call 0x1e7e:0xdb3
000008B0  83C40A            add sp,byte +0xa
000008B3  5D                pop bp
000008B4  CB                retf

; ---- 1E7E:08B5 ----
000008B5  55                push bp
000008B6  8BEC              mov bp,sp
000008B8  B021              mov al,0x21
000008BA  50                push ax
000008BB  FF7608            push word [bp+0x8]
000008BE  FF7606            push word [bp+0x6]
000008C1  B83000            mov ax,0x30
000008C4  50                push ax
000008C5  B84000            mov ax,0x40
000008C8  50                push ax
000008C9  9AB30D7E1E        call 0x1e7e:0xdb3
000008CE  83C40A            add sp,byte +0xa
000008D1  5D                pop bp
000008D2  CB                retf

; ---- 1E7E:08D3 ----
000008D3  55                push bp
000008D4  8BEC              mov bp,sp
000008D6  B000              mov al,0x0
000008D8  50                push ax
000008D9  FF7608            push word [bp+0x8]
000008DC  FF7606            push word [bp+0x6]
000008DF  B82800            mov ax,0x28
000008E2  50                push ax
000008E3  B80800            mov ax,0x8
000008E6  50                push ax
000008E7  9AB30D7E1E        call 0x1e7e:0xdb3
000008EC  83C40A            add sp,byte +0xa
000008EF  5D                pop bp
000008F0  CB                retf

; ---- 1E7E:08F1 ----
000008F1  55                push bp
000008F2  8BEC              mov bp,sp
000008F4  83EC04            sub sp,byte +0x4
000008F7  56                push si
000008F8  57                push di
000008F9  8B7606            mov si,[bp+0x6]
000008FC  8A84012F          mov al,[si+0x2f01]
00000900  8846FD            mov [bp-0x3],al
00000903  33FF              xor di,di
00000905  EB5A              jmp short 0x961
00000907  C746FE0000        mov word [bp-0x2],0x0
0000090C  EB4C              jmp short 0x95a
0000090E  8A4608            mov al,[bp+0x8]
00000911  B400              mov ah,0x0
00000913  8BD8              mov bx,ax
00000915  8A87860F          mov al,[bx+0xf86]
00000919  50                push ax
0000091A  8A845008          mov al,[si+0x850]
0000091E  B400              mov ah,0x0
00000920  03C7              add ax,di
00000922  50                push ax
00000923  8A844C08          mov al,[si+0x84c]
00000927  B400              mov ah,0x0
00000929  0346FE            add ax,[bp-0x2]
0000092C  50                push ax
0000092D  8BC7              mov ax,di
0000092F  D1E0              shl ax,1
00000931  D1E0              shl ax,1
00000933  D1E0              shl ax,1
00000935  050800            add ax,0x8
00000938  50                push ax
00000939  8A4608            mov al,[bp+0x8]
0000093C  B400              mov ah,0x0
0000093E  BA7800            mov dx,0x78
00000941  F7EA              imul dx
00000943  8B56FE            mov dx,[bp-0x2]
00000946  D1E2              shl dx,1
00000948  D1E2              shl dx,1
0000094A  D1E2              shl dx,1
0000094C  03C2              add ax,dx
0000094E  50                push ax
0000094F  9AB30D7E1E        call 0x1e7e:0xdb3
00000954  83C40A            add sp,byte +0xa
00000957  FF46FE            inc word [bp-0x2]
0000095A  837EFE01          cmp word [bp-0x2],byte +0x1
0000095E  7EAE              jng 0x90e
00000960  47                inc di
00000961  83FF01            cmp di,byte +0x1
00000964  7EA1              jng 0x907
00000966  807EFDFF          cmp byte [bp-0x3],0xff
0000096A  744B              jz 0x9b7
0000096C  807E0801          cmp byte [bp+0x8],0x1
00000970  7445              jz 0x9b7
00000972  B8FFFF            mov ax,0xffff
00000975  50                push ax
00000976  8A84820F          mov al,[si+0xf82]
0000097A  B400              mov ah,0x0
0000097C  50                push ax
0000097D  8A46FD            mov al,[bp-0x3]
00000980  98                cbw
00000981  D1E0              shl ax,1
00000983  D1E0              shl ax,1
00000985  8BD8              mov bx,ax
00000987  FFB7740F          push word [bx+0xf74]
0000098B  FFB7720F          push word [bx+0xf72]
0000098F  8A845008          mov al,[si+0x850]
00000993  B400              mov ah,0x0
00000995  D1E0              shl ax,1
00000997  D1E0              shl ax,1
00000999  D1E0              shl ax,1
0000099B  050600            add ax,0x6
0000099E  50                push ax
0000099F  8A844C08          mov al,[si+0x84c]
000009A3  B400              mov ah,0x0
000009A5  D1E0              shl ax,1
000009A7  D1E0              shl ax,1
000009A9  D1E0              shl ax,1
000009AB  053E00            add ax,0x3e
000009AE  50                push ax
000009AF  9A2505960A        call 0xa96:0x525
000009B4  83C40C            add sp,byte +0xc
000009B7  8A4608            mov al,[bp+0x8]
000009BA  8884B64B          mov [si+0x4bb6],al
000009BE  5F                pop di
000009BF  5E                pop si
000009C0  8BE5              mov sp,bp
000009C2  5D                pop bp
000009C3  CB                retf

; ---- 1E7E:09C4 ----
000009C4  55                push bp
000009C5  8BEC              mov bp,sp
000009C7  B008              mov al,0x8
000009C9  50                push ax
000009CA  A0AA37            mov al,[0x37aa]
000009CD  98                cbw
000009CE  8BD8              mov bx,ax
000009D0  8A874808          mov al,[bx+0x848]
000009D4  B400              mov ah,0x0
000009D6  50                push ax
000009D7  A0AA37            mov al,[0x37aa]
000009DA  98                cbw
000009DB  8BD8              mov bx,ax
000009DD  8A874408          mov al,[bx+0x844]
000009E1  B400              mov ah,0x0
000009E3  50                push ax
000009E4  B83000            mov ax,0x30
000009E7  50                push ax
000009E8  A0AA37            mov al,[0x37aa]
000009EB  98                cbw
000009EC  D1E0              shl ax,1
000009EE  D1E0              shl ax,1
000009F0  D1E0              shl ax,1
000009F2  50                push ax
000009F3  9AB30D7E1E        call 0x1e7e:0xdb3
000009F8  83C40A            add sp,byte +0xa
000009FB  5D                pop bp
000009FC  CB                retf

; ---- 1E7E:09FD ----
000009FD  55                push bp
000009FE  8BEC              mov bp,sp
00000A00  83EC06            sub sp,byte +0x6
00000A03  56                push si
00000A04  57                push di
00000A05  833E880F00        cmp word [0xf88],byte +0x0
00000A0A  7E05              jng 0xa11
00000A0C  B000              mov al,0x0
00000A0E  E9B301            jmp 0xbc4
00000A11  A0AA37            mov al,[0x37aa]
00000A14  98                cbw
00000A15  D1E0              shl ax,1
00000A17  8BD8              mov bx,ax
00000A19  8B87E643          mov ax,[bx+0x43e6]
00000A1D  BB0400            mov bx,0x4
00000A20  99                cwd
00000A21  F7FB              idiv bx
00000A23  0BD2              or dx,dx
00000A25  7407              jz 0xa2e
00000A27  B000              mov al,0x0
00000A29  E99801            jmp 0xbc4
00000A2C  EB15              jmp short 0xa43
00000A2E  A0AA37            mov al,[0x37aa]
00000A31  98                cbw
00000A32  D1E0              shl ax,1
00000A34  8BD8              mov bx,ax
00000A36  8B87E643          mov ax,[bx+0x43e6]
00000A3A  BB0400            mov bx,0x4
00000A3D  99                cwd
00000A3E  F7FB              idiv bx
00000A40  8946FC            mov [bp-0x4],ax
00000A43  8B5EFC            mov bx,[bp-0x4]
00000A46  8A878A0F          mov al,[bx+0xf8a]
00000A4A  A2AE54            mov [0x54ae],al
00000A4D  8B5EFC            mov bx,[bp-0x4]
00000A50  8A878E0F          mov al,[bx+0xf8e]
00000A54  A2AF54            mov [0x54af],al
00000A57  A0AA37            mov al,[0x37aa]
00000A5A  98                cbw
00000A5B  D1E0              shl ax,1
00000A5D  8BD8              mov bx,ax
00000A5F  8B87E24B          mov ax,[bx+0x4be2]
00000A63  48                dec ax
00000A64  BB0300            mov bx,0x3
00000A67  99                cwd
00000A68  F7FB              idiv bx
00000A6A  8946FE            mov [bp-0x2],ax
00000A6D  A0AA37            mov al,[0x37aa]
00000A70  98                cbw
00000A71  D1E0              shl ax,1
00000A73  8BD8              mov bx,ax
00000A75  8B87EA4B          mov ax,[bx+0x4bea]
00000A79  48                dec ax
00000A7A  BB0300            mov bx,0x3
00000A7D  99                cwd
00000A7E  F7FB              idiv bx
00000A80  8BC8              mov cx,ax
00000A82  8B5EFE            mov bx,[bp-0x2]
00000A85  D1E3              shl bx,1
00000A87  D1E3              shl bx,1
00000A89  D1E3              shl bx,1
00000A8B  03D9              add bx,cx
00000A8D  80BF784401        cmp byte [bx+0x4478],0x1
00000A92  750F              jnz 0xaa3
00000A94  A0AE54            mov al,[0x54ae]
00000A97  98                cbw
00000A98  0146FE            add [bp-0x2],ax
00000A9B  A0AF54            mov al,[0x54af]
00000A9E  98                cbw
00000A9F  03C8              add cx,ax
00000AA1  EB40              jmp short 0xae3
00000AA3  A0AE54            mov al,[0x54ae]
00000AA6  98                cbw
00000AA7  8B56FE            mov dx,[bp-0x2]
00000AAA  2BD0              sub dx,ax
00000AAC  8BFA              mov di,dx
00000AAE  A0AF54            mov al,[0x54af]
00000AB1  98                cbw
00000AB2  8BD1              mov dx,cx
00000AB4  2BD0              sub dx,ax
00000AB6  8BF2              mov si,dx
00000AB8  0BFF              or di,di
00000ABA  7C0E              jl 0xaca
00000ABC  83FF07            cmp di,byte +0x7
00000ABF  7F09              jg 0xaca
00000AC1  0BF6              or si,si
00000AC3  7C05              jl 0xaca
00000AC5  83FE07            cmp si,byte +0x7
00000AC8  7E05              jng 0xacf
00000ACA  B000              mov al,0x0
00000ACC  E9F500            jmp 0xbc4
00000ACF  8BDF              mov bx,di
00000AD1  D1E3              shl bx,1
00000AD3  D1E3              shl bx,1
00000AD5  D1E3              shl bx,1
00000AD7  80B8784401        cmp byte [bx+si+0x4478],0x1
00000ADC  7405              jz 0xae3
00000ADE  B000              mov al,0x0
00000AE0  E9E100            jmp 0xbc4
00000AE3  8B5EFE            mov bx,[bp-0x2]
00000AE6  D1E3              shl bx,1
00000AE8  D1E3              shl bx,1
00000AEA  D1E3              shl bx,1
00000AEC  03D9              add bx,cx
00000AEE  80BF784400        cmp byte [bx+0x4478],0x0
00000AF3  7405              jz 0xafa
00000AF5  B000              mov al,0x0
00000AF7  E9CA00            jmp 0xbc4
00000AFA  8B46FE            mov ax,[bp-0x2]
00000AFD  BA0300            mov dx,0x3
00000B00  F7EA              imul dx
00000B02  40                inc ax
00000B03  A3AA54            mov [0x54aa],ax
00000B06  803EAE54FF        cmp byte [0x54ae],0xff
00000B0B  7508              jnz 0xb15
00000B0D  A1AA54            mov ax,[0x54aa]
00000B10  40                inc ax
00000B11  40                inc ax
00000B12  A3AA54            mov [0x54aa],ax
00000B15  8BC1              mov ax,cx
00000B17  BA0300            mov dx,0x3
00000B1A  F7EA              imul dx
00000B1C  40                inc ax
00000B1D  A3AC54            mov [0x54ac],ax
00000B20  803EAF54FF        cmp byte [0x54af],0xff
00000B25  7508              jnz 0xb2f
00000B27  A1AC54            mov ax,[0x54ac]
00000B2A  40                inc ax
00000B2B  40                inc ax
00000B2C  A3AC54            mov [0x54ac],ax
00000B2F  C746FA0000        mov word [bp-0x6],0x0
00000B34  8B7EFE            mov di,[bp-0x2]
00000B37  8BF1              mov si,cx
00000B39  EB34              jmp short 0xb6f
00000B3B  FF46FA            inc word [bp-0x6]
00000B3E  A0AE54            mov al,[0x54ae]
00000B41  98                cbw
00000B42  03F8              add di,ax
00000B44  A0AF54            mov al,[0x54af]
00000B47  98                cbw
00000B48  03F0              add si,ax
00000B4A  83FF07            cmp di,byte +0x7
00000B4D  7F1C              jg 0xb6b
00000B4F  0BFF              or di,di
00000B51  7C18              jl 0xb6b
00000B53  83FE07            cmp si,byte +0x7
00000B56  7F13              jg 0xb6b
00000B58  0BF6              or si,si
00000B5A  7C0F              jl 0xb6b
00000B5C  8BDF              mov bx,di
00000B5E  D1E3              shl bx,1
00000B60  D1E3              shl bx,1
00000B62  D1E3              shl bx,1
00000B64  80B8784402        cmp byte [bx+si+0x4478],0x2
00000B69  7504              jnz 0xb6f
00000B6B  B000              mov al,0x0
00000B6D  EB55              jmp short 0xbc4
00000B6F  8BDF              mov bx,di
00000B71  D1E3              shl bx,1
00000B73  D1E3              shl bx,1
00000B75  D1E3              shl bx,1
00000B77  80B8784400        cmp byte [bx+si+0x4478],0x0
00000B7C  74BD              jz 0xb3b
00000B7E  8B7EFE            mov di,[bp-0x2]
00000B81  8BF1              mov si,cx
00000B83  EB19              jmp short 0xb9e
00000B85  8BDF              mov bx,di
00000B87  D1E3              shl bx,1
00000B89  D1E3              shl bx,1
00000B8B  D1E3              shl bx,1
00000B8D  C680784402        mov byte [bx+si+0x4478],0x2
00000B92  A0AE54            mov al,[0x54ae]
00000B95  98                cbw
00000B96  03F8              add di,ax
00000B98  A0AF54            mov al,[0x54af]
00000B9B  98                cbw
00000B9C  03F0              add si,ax
00000B9E  8BDF              mov bx,di
00000BA0  D1E3              shl bx,1
00000BA2  D1E3              shl bx,1
00000BA4  D1E3              shl bx,1
00000BA6  80B8784400        cmp byte [bx+si+0x4478],0x0
00000BAB  74D8              jz 0xb85
00000BAD  8B46FA            mov ax,[bp-0x6]
00000BB0  BA0300            mov dx,0x3
00000BB3  F7EA              imul dx
00000BB5  A3880F            mov [0xf88],ax
00000BB8  A0C737            mov al,[0x37c7]
00000BBB  0C01              or al,0x1
00000BBD  A2C737            mov [0x37c7],al
00000BC0  B001              mov al,0x1
00000BC2  EB00              jmp short 0xbc4
00000BC4  5F                pop di
00000BC5  5E                pop si
00000BC6  8BE5              mov sp,bp
00000BC8  5D                pop bp
00000BC9  CB                retf

; ---- 1E7E:0BCA ----
00000BCA  55                push bp
00000BCB  8BEC              mov bp,sp
00000BCD  833E880F00        cmp word [0xf88],byte +0x0
00000BD2  7503              jnz 0xbd7
00000BD4  E9AE00            jmp 0xc85
00000BD7  A1920F            mov ax,[0xf92]
00000BDA  48                dec ax
00000BDB  A3920F            mov [0xf92],ax
00000BDE  0BC0              or ax,ax
00000BE0  7403              jz 0xbe5
00000BE2  E9A000            jmp 0xc85
00000BE5  803EAF5400        cmp byte [0x54af],0x0
00000BEA  7548              jnz 0xc34
00000BEC  33C0              xor ax,ax
00000BEE  50                push ax
00000BEF  FF36AC54          push word [0x54ac]
00000BF3  FF36AA54          push word [0x54aa]
00000BF7  9A870C7E1E        call 0x1e7e:0xc87
00000BFC  83C406            add sp,byte +0x6
00000BFF  B80400            mov ax,0x4
00000C02  50                push ax
00000C03  A1AC54            mov ax,[0x54ac]
00000C06  40                inc ax
00000C07  50                push ax
00000C08  FF36AA54          push word [0x54aa]
00000C0C  9A870C7E1E        call 0x1e7e:0xc87
00000C11  83C406            add sp,byte +0x6
00000C14  B80100            mov ax,0x1
00000C17  50                push ax
00000C18  A1AC54            mov ax,[0x54ac]
00000C1B  40                inc ax
00000C1C  40                inc ax
00000C1D  50                push ax
00000C1E  FF36AA54          push word [0x54aa]
00000C22  9A870C7E1E        call 0x1e7e:0xc87
00000C27  83C406            add sp,byte +0x6
00000C2A  A0AE54            mov al,[0x54ae]
00000C2D  98                cbw
00000C2E  0106AA54          add [0x54aa],ax
00000C32  EB47              jmp short 0xc7b
00000C34  B80200            mov ax,0x2
00000C37  50                push ax
00000C38  FF36AC54          push word [0x54ac]
00000C3C  FF36AA54          push word [0x54aa]
00000C40  9A870C7E1E        call 0x1e7e:0xc87
00000C45  83C406            add sp,byte +0x6
00000C48  B80500            mov ax,0x5
00000C4B  50                push ax
00000C4C  FF36AC54          push word [0x54ac]
00000C50  A1AA54            mov ax,[0x54aa]
00000C53  40                inc ax
00000C54  50                push ax
00000C55  9A870C7E1E        call 0x1e7e:0xc87
00000C5A  83C406            add sp,byte +0x6
00000C5D  B80300            mov ax,0x3
00000C60  50                push ax
00000C61  FF36AC54          push word [0x54ac]
00000C65  A1AA54            mov ax,[0x54aa]
00000C68  40                inc ax
00000C69  40                inc ax
00000C6A  50                push ax
00000C6B  9A870C7E1E        call 0x1e7e:0xc87
00000C70  83C406            add sp,byte +0x6
00000C73  A0AF54            mov al,[0x54af]
00000C76  98                cbw
00000C77  0106AC54          add [0x54ac],ax
00000C7B  FF0E880F          dec word [0xf88]
00000C7F  C706920F0A00      mov word [0xf92],0xa
00000C85  5D                pop bp
00000C86  CB                retf

; ---- 1E7E:0C87 ----
00000C87  55                push bp
00000C88  8BEC              mov bp,sp
00000C8A  83EC04            sub sp,byte +0x4
00000C8D  56                push si
00000C8E  57                push di
00000C8F  8B7E08            mov di,[bp+0x8]
00000C92  8B760A            mov si,[bp+0xa]
00000C95  8B4606            mov ax,[bp+0x6]
00000C98  BA1A00            mov dx,0x1a
00000C9B  F7EA              imul dx
00000C9D  8BD8              mov bx,ax
00000C9F  8A81F448          mov al,[bx+di+0x48f4]
00000CA3  B400              mov ah,0x0
00000CA5  BB0800            mov bx,0x8
00000CA8  99                cwd
00000CA9  F7FB              idiv bx
00000CAB  8846FC            mov [bp-0x4],al
00000CAE  807EFC04          cmp byte [bp-0x4],0x4
00000CB2  7406              jz 0xcba
00000CB4  807EFC03          cmp byte [bp-0x4],0x3
00000CB8  7503              jnz 0xcbd
00000CBA  E9F000            jmp 0xdad
00000CBD  807EFC07          cmp byte [bp-0x4],0x7
00000CC1  7209              jc 0xccc
00000CC3  807EFC0C          cmp byte [bp-0x4],0xc
00000CC7  7703              ja 0xccc
00000CC9  E9E100            jmp 0xdad
00000CCC  807EFC02          cmp byte [bp-0x4],0x2
00000CD0  7503              jnz 0xcd5
00000CD2  E9D800            jmp 0xdad
00000CD5  83FE04            cmp si,byte +0x4
00000CD8  751A              jnz 0xcf4
00000CDA  B000              mov al,0x0
00000CDC  50                push ax
00000CDD  57                push di
00000CDE  FF7606            push word [bp+0x6]
00000CE1  B81000            mov ax,0x10
00000CE4  50                push ax
00000CE5  B85000            mov ax,0x50
00000CE8  50                push ax
00000CE9  9AB30D7E1E        call 0x1e7e:0xdb3
00000CEE  83C40A            add sp,byte +0xa
00000CF1  E9B900            jmp 0xdad
00000CF4  83FE05            cmp si,byte +0x5
00000CF7  751A              jnz 0xd13
00000CF9  B000              mov al,0x0
00000CFB  50                push ax
00000CFC  57                push di
00000CFD  FF7606            push word [bp+0x6]
00000D00  B81000            mov ax,0x10
00000D03  50                push ax
00000D04  B86800            mov ax,0x68
00000D07  50                push ax
00000D08  9AB30D7E1E        call 0x1e7e:0xdb3
00000D0D  83C40A            add sp,byte +0xa
00000D10  E99A00            jmp 0xdad
00000D13  8A46FC            mov al,[bp-0x4]
00000D16  B400              mov ah,0x0
00000D18  0BC0              or ax,ax
00000D1A  740D              jz 0xd29
00000D1C  3D0600            cmp ax,0x6
00000D1F  7402              jz 0xd23
00000D21  EB0C              jmp short 0xd2f
00000D23  C646FF00          mov byte [bp-0x1],0x0
00000D27  EB38              jmp short 0xd61
00000D29  C646FF03          mov byte [bp-0x1],0x3
00000D2D  EB32              jmp short 0xd61
00000D2F  8B4606            mov ax,[bp+0x6]
00000D32  BA1A00            mov dx,0x1a
00000D35  F7EA              imul dx
00000D37  8BD8              mov bx,ax
00000D39  8A81F448          mov al,[bx+di+0x48f4]
00000D3D  B400              mov ah,0x0
00000D3F  BB0800            mov bx,0x8
00000D42  99                cwd
00000D43  F7FB              idiv bx
00000D45  8846FE            mov [bp-0x2],al
00000D48  8BC6              mov ax,si
00000D4A  BA0C00            mov dx,0xc
00000D4D  F7EA              imul dx
00000D4F  8A56FE            mov dl,[bp-0x2]
00000D52  B600              mov dh,0x0
00000D54  03C2              add ax,dx
00000D56  8BD8              mov bx,ax
00000D58  8A87B70F          mov al,[bx+0xfb7]
00000D5C  8846FF            mov [bp-0x1],al
00000D5F  EB00              jmp short 0xd61
00000D61  8A46FF            mov al,[bp-0x1]
00000D64  B400              mov ah,0x0
00000D66  D1E0              shl ax,1
00000D68  D1E0              shl ax,1
00000D6A  8BD8              mov bx,ax
00000D6C  8A80B40F          mov al,[bx+si+0xfb4]
00000D70  D0E0              shl al,1
00000D72  D0E0              shl al,1
00000D74  D0E0              shl al,1
00000D76  8846FD            mov [bp-0x3],al
00000D79  8A46FD            mov al,[bp-0x3]
00000D7C  50                push ax
00000D7D  57                push di
00000D7E  FF7606            push word [bp+0x6]
00000D81  8A46FF            mov al,[bp-0x1]
00000D84  B400              mov ah,0x0
00000D86  D1E0              shl ax,1
00000D88  D1E0              shl ax,1
00000D8A  8BD8              mov bx,ax
00000D8C  8A80A40F          mov al,[bx+si+0xfa4]
00000D90  B400              mov ah,0x0
00000D92  50                push ax
00000D93  8A46FF            mov al,[bp-0x1]
00000D96  B400              mov ah,0x0
00000D98  D1E0              shl ax,1
00000D9A  D1E0              shl ax,1
00000D9C  8BD8              mov bx,ax
00000D9E  8A80940F          mov al,[bx+si+0xf94]
00000DA2  B400              mov ah,0x0
00000DA4  50                push ax
00000DA5  9AB30D7E1E        call 0x1e7e:0xdb3
00000DAA  83C40A            add sp,byte +0xa
00000DAD  5F                pop di
00000DAE  5E                pop si
00000DAF  8BE5              mov sp,bp
00000DB1  5D                pop bp
00000DB2  CB                retf

; ---- 1E7E:0DB3 ----
00000DB3  55                push bp
00000DB4  8BEC              mov bp,sp
00000DB6  56                push si
00000DB7  57                push di
00000DB8  BE0800            mov si,0x8
00000DBB  8B460C            mov ax,[bp+0xc]
00000DBE  D1E0              shl ax,1
00000DC0  D1E0              shl ax,1
00000DC2  D1E0              shl ax,1
00000DC4  05FCFF            add ax,0xfffc
00000DC7  8BF8              mov di,ax
00000DC9  0BC0              or ax,ax
00000DCB  7D02              jnl 0xdcf
00000DCD  33FF              xor di,di
00000DCF  837E0C00          cmp word [bp+0xc],byte +0x0
00000DD3  750E              jnz 0xde3
00000DD5  8B4608            mov ax,[bp+0x8]
00000DD8  050400            add ax,0x4
00000DDB  894608            mov [bp+0x8],ax
00000DDE  BE0400            mov si,0x4
00000DE1  EB09              jmp short 0xdec
00000DE3  837E0C19          cmp word [bp+0xc],byte +0x19
00000DE7  7503              jnz 0xdec
00000DE9  BE0400            mov si,0x4
00000DEC  57                push di
00000DED  8B460A            mov ax,[bp+0xa]
00000DF0  D1E0              shl ax,1
00000DF2  D1E0              shl ax,1
00000DF4  D1E0              shl ax,1
00000DF6  053800            add ax,0x38
00000DF9  50                push ax
00000DFA  56                push si
00000DFB  FF7608            push word [bp+0x8]
00000DFE  FF7606            push word [bp+0x6]
00000E01  9A1103960A        call 0xa96:0x311
00000E06  83C40A            add sp,byte +0xa
00000E09  8B460A            mov ax,[bp+0xa]
00000E0C  BA1A00            mov dx,0x1a
00000E0F  F7EA              imul dx
00000E11  03460C            add ax,[bp+0xc]
00000E14  8A560E            mov dl,[bp+0xe]
00000E17  8BD8              mov bx,ax
00000E19  8897F448          mov [bx+0x48f4],dl
00000E1D  5F                pop di
00000E1E  5E                pop si
00000E1F  5D                pop bp
00000E20  CB                retf

; ---- 1E7E:0E21 ----
00000E21  55                push bp
00000E22  8BEC              mov bp,sp
00000E24  56                push si
00000E25  B80100            mov ax,0x1
00000E28  50                push ax
00000E29  9ABA01FD21        call 0x21fd:0x1ba
00000E2E  59                pop cx
00000E2F  D1E0              shl ax,1
00000E31  D1E0              shl ax,1
00000E33  D1E0              shl ax,1
00000E35  8BF0              mov si,ax
00000E37  FF7608            push word [bp+0x8]
00000E3A  FF7606            push word [bp+0x6]
00000E3D  B80800            mov ax,0x8
00000E40  50                push ax
00000E41  B80800            mov ax,0x8
00000E44  50                push ax
00000E45  B83F00            mov ax,0x3f
00000E48  50                push ax
00000E49  8BC6              mov ax,si
00000E4B  056000            add ax,0x60
00000E4E  50                push ax
00000E4F  B8                db 0xb8
