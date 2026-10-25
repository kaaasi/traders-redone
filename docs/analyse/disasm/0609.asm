; segment 0609  file offset 0x9690  size 0x21C0
00000000  005E5D            add [bp+0x5d],bl
00000003  CB                retf

; ---- 0609:0004 ----
00000004  55                push bp
00000005  8BEC              mov bp,sp
00000007  9A0D00AF21        call 0x21af:0xd
0000000C  C606AA3700        mov byte [0x37aa],0x0
00000011  EB55              jmp short 0x68
00000013  A0AA37            mov al,[0x37aa]
00000016  98                cbw
00000017  8BD8              mov bx,ax
00000019  80BFFE40FF        cmp byte [bx+0x40fe],0xff
0000001E  750D              jnz 0x2d
00000020  A0AA37            mov al,[0x37aa]
00000023  98                cbw
00000024  8BD8              mov bx,ax
00000026  C687B24302        mov byte [bx+0x43b2],0x2
0000002B  EB0B              jmp short 0x38
0000002D  A0AA37            mov al,[0x37aa]
00000030  98                cbw
00000031  8BD8              mov bx,ax
00000033  C687B24303        mov byte [bx+0x43b2],0x3
00000038  A0AA37            mov al,[0x37aa]
0000003B  98                cbw
0000003C  8BD8              mov bx,ax
0000003E  80BFC24B00        cmp byte [bx+0x4bc2],0x0
00000043  7514              jnz 0x59
00000045  803E052F00        cmp byte [0x2f05],0x0
0000004A  7E0D              jng 0x59
0000004C  A0AA37            mov al,[0x37aa]
0000004F  98                cbw
00000050  8BD8              mov bx,ax
00000052  C687FA4301        mov byte [bx+0x43fa],0x1
00000057  EB0B              jmp short 0x64
00000059  A0AA37            mov al,[0x37aa]
0000005C  98                cbw
0000005D  8BD8              mov bx,ax
0000005F  C687FA4300        mov byte [bx+0x43fa],0x0
00000064  FE06AA37          inc byte [0x37aa]
00000068  803EAA3704        cmp byte [0x37aa],0x4
0000006D  7CA4              jl 0x13
0000006F  C606821500        mov byte [0x1582],0x0
00000074  9AD6000906        call 0x609:0xd6
00000079  9AA90F0906        call 0x609:0xfa9
0000007E  5D                pop bp
0000007F  CB                retf

; ---- 0609:0080 ----
00000080  55                push bp
00000081  8BEC              mov bp,sp
00000083  9A0D00AF21        call 0x21af:0xd
00000088  C606AA3700        mov byte [0x37aa],0x0
0000008D  EB34              jmp short 0xc3
0000008F  A0AA37            mov al,[0x37aa]
00000092  98                cbw
00000093  8BD8              mov bx,ax
00000095  80BF904801        cmp byte [bx+0x4890],0x1
0000009A  7518              jnz 0xb4
0000009C  A0AA37            mov al,[0x37aa]
0000009F  98                cbw
000000A0  8BD8              mov bx,ax
000000A2  C687B24305        mov byte [bx+0x43b2],0x5
000000A7  A0AA37            mov al,[0x37aa]
000000AA  98                cbw
000000AB  8BD8              mov bx,ax
000000AD  C687FA4301        mov byte [bx+0x43fa],0x1
000000B2  EB0B              jmp short 0xbf
000000B4  A0AA37            mov al,[0x37aa]
000000B7  98                cbw
000000B8  8BD8              mov bx,ax
000000BA  C687B24306        mov byte [bx+0x43b2],0x6
000000BF  FE06AA37          inc byte [0x37aa]
000000C3  803EAA3704        cmp byte [0x37aa],0x4
000000C8  7CC5              jl 0x8f
000000CA  C606821501        mov byte [0x1582],0x1
000000CF  9AD6000906        call 0x609:0xd6
000000D4  5D                pop bp
000000D5  CB                retf

; ---- 0609:00D6 ----
000000D6  55                push bp
000000D7  8BEC              mov bp,sp
000000D9  FF36CE08          push word [0x8ce]
000000DD  9A02006C18        call 0x186c:0x2
000000E2  59                pop cx
000000E3  9A2D1F0906        call 0x609:0x1f2d
000000E8  B86606            mov ax,0x666
000000EB  50                push ax
000000EC  9A7D03FD21        call 0x21fd:0x37d
000000F1  59                pop cx
000000F2  1E                push ds
000000F3  B8A800            mov ax,0xa8
000000F6  50                push ax
000000F7  9A6C02FD21        call 0x21fd:0x26c
000000FC  59                pop cx
000000FD  59                pop cx
000000FE  9AE7110906        call 0x609:0x11e7
00000103  9ACB062508        call 0x825:0x6cb
00000108  C606AA3700        mov byte [0x37aa],0x0
0000010D  EB0F              jmp short 0x11e
0000010F  A0AA37            mov al,[0x37aa]
00000112  98                cbw
00000113  8BD8              mov bx,ax
00000115  C687AF3CFF        mov byte [bx+0x3caf],0xff
0000011A  FE06AA37          inc byte [0x37aa]
0000011E  803EAA3704        cmp byte [0x37aa],0x4
00000123  7CEA              jl 0x10f
00000125  9A60010906        call 0x609:0x160
0000012A  9AE51B0906        call 0x609:0x1be5
0000012F  9A2903FD21        call 0x21fd:0x329
00000134  9A04020906        call 0x609:0x204
00000139  9AA31E0906        call 0x609:0x1ea3
0000013E  9A2A030906        call 0x609:0x32a
00000143  803E821500        cmp byte [0x1582],0x0
00000148  750A              jnz 0x154
0000014A  B80100            mov ax,0x1
0000014D  50                push ax
0000014E  9A0400FD21        call 0x21fd:0x4
00000153  59                pop cx
00000154  9A1B00960A        call 0xa96:0x1b
00000159  9AE0030906        call 0x609:0x3e0
0000015E  5D                pop bp
0000015F  CB                retf

; ---- 0609:0160 ----
00000160  55                push bp
00000161  8BEC              mov bp,sp
00000163  C606AA3700        mov byte [0x37aa],0x0
00000168  E98D00            jmp 0x1f8
0000016B  A0AA37            mov al,[0x37aa]
0000016E  98                cbw
0000016F  8BD8              mov bx,ax
00000171  80BFB24306        cmp byte [bx+0x43b2],0x6
00000176  7502              jnz 0x17a
00000178  EB7A              jmp short 0x1f4
0000017A  A0AA37            mov al,[0x37aa]
0000017D  98                cbw
0000017E  8BD8              mov bx,ax
00000180  80BFFE40FF        cmp byte [bx+0x40fe],0xff
00000185  7541              jnz 0x1c8
00000187  A0AA37            mov al,[0x37aa]
0000018A  98                cbw
0000018B  BB0200            mov bx,0x2
0000018E  99                cwd
0000018F  F7FB              idiv bx
00000191  0BD2              or dx,dx
00000193  7504              jnz 0x199
00000195  B000              mov al,0x0
00000197  EB05              jmp short 0x19e
00000199  A0FC08            mov al,[0x8fc]
0000019C  FEC8              dec al
0000019E  50                push ax
0000019F  A0AA37            mov al,[0x37aa]
000001A2  98                cbw
000001A3  8BD8              mov bx,ax
000001A5  58                pop ax
000001A6  88874E30          mov [bx+0x304e],al
000001AA  803EAA3701        cmp byte [0x37aa],0x1
000001AF  7F04              jg 0x1b5
000001B1  B000              mov al,0x0
000001B3  EB05              jmp short 0x1ba
000001B5  A0FE08            mov al,[0x8fe]
000001B8  FEC8              dec al
000001BA  50                push ax
000001BB  A0AA37            mov al,[0x37aa]
000001BE  98                cbw
000001BF  8BD8              mov bx,ax
000001C1  58                pop ax
000001C2  88875A30          mov [bx+0x305a],al
000001C6  EB2C              jmp short 0x1f4
000001C8  A0AA37            mov al,[0x37aa]
000001CB  98                cbw
000001CC  8BD8              mov bx,ax
000001CE  8A87FE40          mov al,[bx+0x40fe]
000001D2  50                push ax
000001D3  A0AA37            mov al,[0x37aa]
000001D6  98                cbw
000001D7  8BD8              mov bx,ax
000001D9  58                pop ax
000001DA  88874E30          mov [bx+0x304e],al
000001DE  A0AA37            mov al,[0x37aa]
000001E1  98                cbw
000001E2  8BD8              mov bx,ax
000001E4  8A870241          mov al,[bx+0x4102]
000001E8  50                push ax
000001E9  A0AA37            mov al,[0x37aa]
000001EC  98                cbw
000001ED  8BD8              mov bx,ax
000001EF  58                pop ax
000001F0  88875A30          mov [bx+0x305a],al
000001F4  FE06AA37          inc byte [0x37aa]
000001F8  803EAA3704        cmp byte [0x37aa],0x4
000001FD  7D03              jnl 0x202
000001FF  E969FF            jmp 0x16b
00000202  5D                pop bp
00000203  CB                retf

; ---- 0609:0204 ----
00000204  55                push bp
00000205  8BEC              mov bp,sp
00000207  803E821501        cmp byte [0x1582],0x1
0000020C  756A              jnz 0x278
0000020E  B80100            mov ax,0x1
00000211  50                push ax
00000212  9ADB05FD21        call 0x21fd:0x5db
00000217  59                pop cx
00000218  B80500            mov ax,0x5
0000021B  50                push ax
0000021C  B80900            mov ax,0x9
0000021F  50                push ax
00000220  B80200            mov ax,0x2
00000223  50                push ax
00000224  1E                push ds
00000225  B8E037            mov ax,0x37e0
00000228  50                push ax
00000229  B84001            mov ax,0x140
0000022C  50                push ax
0000022D  B81B00            mov ax,0x1b
00000230  50                push ax
00000231  33C0              xor ax,ax
00000233  50                push ax
00000234  9A8802690A        call 0xa69:0x288
00000239  83C410            add sp,byte +0x10
0000023C  33C0              xor ax,ax
0000023E  50                push ax
0000023F  9ADB05FD21        call 0x21fd:0x5db
00000244  59                pop cx
00000245  33C0              xor ax,ax
00000247  BA3200            mov dx,0x32
0000024A  50                push ax
0000024B  52                push dx
0000024C  9A8501FD21        call 0x21fd:0x185
00000251  59                pop cx
00000252  59                pop cx
00000253  9A1B00960A        call 0xa96:0x1b
00000258  B82C00            mov ax,0x2c
0000025B  50                push ax
0000025C  B82001            mov ax,0x120
0000025F  50                push ax
00000260  B80600            mov ax,0x6
00000263  50                push ax
00000264  B81000            mov ax,0x10
00000267  50                push ax
00000268  9A041F0906        call 0x609:0x1f04
0000026D  83C408            add sp,byte +0x8
00000270  9A0D06FD21        call 0x21fd:0x60d
00000275  E9B000            jmp 0x328
00000278  9A1B00960A        call 0xa96:0x1b
0000027D  B80100            mov ax,0x1
00000280  50                push ax
00000281  B8FFFF            mov ax,0xffff
00000284  50                push ax
00000285  B80B00            mov ax,0xb
00000288  50                push ax
00000289  B80500            mov ax,0x5
0000028C  50                push ax
0000028D  FF36EA37          push word [0x37ea]
00000291  FF36E837          push word [0x37e8]
00000295  B81100            mov ax,0x11
00000298  50                push ax
00000299  B81201            mov ax,0x112
0000029C  50                push ax
0000029D  9A4D00690A        call 0xa69:0x4d
000002A2  83C410            add sp,byte +0x10
000002A5  B80100            mov ax,0x1
000002A8  50                push ax
000002A9  B8FFFF            mov ax,0xffff
000002AC  50                push ax
000002AD  B80A00            mov ax,0xa
000002B0  50                push ax
000002B1  B80200            mov ax,0x2
000002B4  50                push ax
000002B5  B80200            mov ax,0x2
000002B8  50                push ax
000002B9  A0A248            mov al,[0x48a2]
000002BC  98                cbw
000002BD  99                cwd
000002BE  52                push dx
000002BF  50                push ax
000002C0  B81800            mov ax,0x18
000002C3  50                push ax
000002C4  B81801            mov ax,0x118
000002C7  50                push ax
000002C8  9A1B01690A        call 0xa69:0x11b
000002CD  83C412            add sp,byte +0x12
000002D0  B80100            mov ax,0x1
000002D3  50                push ax
000002D4  B8FFFF            mov ax,0xffff
000002D7  50                push ax
000002D8  B80B00            mov ax,0xb
000002DB  50                push ax
000002DC  B80500            mov ax,0x5
000002DF  50                push ax
000002E0  FF36EE37          push word [0x37ee]
000002E4  FF36EC37          push word [0x37ec]
000002E8  B81F00            mov ax,0x1f
000002EB  50                push ax
000002EC  B81201            mov ax,0x112
000002EF  50                push ax
000002F0  9A4D00690A        call 0xa69:0x4d
000002F5  83C410            add sp,byte +0x10
000002F8  B80100            mov ax,0x1
000002FB  50                push ax
000002FC  B8FFFF            mov ax,0xffff
000002FF  50                push ax
00000300  B80A00            mov ax,0xa
00000303  50                push ax
00000304  B80200            mov ax,0x2
00000307  50                push ax
00000308  B80200            mov ax,0x2
0000030B  50                push ax
0000030C  A0262F            mov al,[0x2f26]
0000030F  98                cbw
00000310  99                cwd
00000311  52                push dx
00000312  50                push ax
00000313  B82600            mov ax,0x26
00000316  50                push ax
00000317  B81801            mov ax,0x118
0000031A  50                push ax
0000031B  9A1B01690A        call 0xa69:0x11b
00000320  83C412            add sp,byte +0x12
00000323  9A0D06FD21        call 0x21fd:0x60d
00000328  5D                pop bp
00000329  CB                retf

; ---- 0609:032A ----
0000032A  55                push bp
0000032B  8BEC              mov bp,sp
0000032D  C606E63C00        mov byte [0x3ce6],0x0
00000332  C606AA3700        mov byte [0x37aa],0x0
00000337  E99A00            jmp 0x3d4
0000033A  A0AA37            mov al,[0x37aa]
0000033D  98                cbw
0000033E  D1E0              shl ax,1
00000340  D1E0              shl ax,1
00000342  8BD8              mov bx,ax
00000344  C78786370000      mov word [bx+0x3786],0x0
0000034A  C78788370000      mov word [bx+0x3788],0x0
00000350  A0AA37            mov al,[0x37aa]
00000353  98                cbw
00000354  8BD8              mov bx,ax
00000356  80BFB24306        cmp byte [bx+0x43b2],0x6
0000035B  7502              jnz 0x35f
0000035D  EB71              jmp short 0x3d0
0000035F  A0AA37            mov al,[0x37aa]
00000362  98                cbw
00000363  D1E0              shl ax,1
00000365  8BD8              mov bx,ax
00000367  C78722300000      mov word [bx+0x3022],0x0
0000036D  A0AA37            mov al,[0x37aa]
00000370  98                cbw
00000371  8BD8              mov bx,ax
00000373  8A874E30          mov al,[bx+0x304e]
00000377  98                cbw
00000378  B105              mov cl,0x5
0000037A  D3E0              shl ax,cl
0000037C  8B161E45          mov dx,[0x451e]
00000380  03D0              add dx,ax
00000382  83C208            add dx,byte +0x8
00000385  A0AA37            mov al,[0x37aa]
00000388  98                cbw
00000389  BB1A00            mov bx,0x1a
0000038C  52                push dx
0000038D  F7EB              imul bx
0000038F  8BD8              mov bx,ax
00000391  58                pop ax
00000392  89872033          mov [bx+0x3320],ax
00000396  A0AA37            mov al,[0x37aa]
00000399  98                cbw
0000039A  8BD8              mov bx,ax
0000039C  8A875A30          mov al,[bx+0x305a]
000003A0  98                cbw
000003A1  BA1600            mov dx,0x16
000003A4  F7EA              imul dx
000003A6  8B162845          mov dx,[0x4528]
000003AA  03D0              add dx,ax
000003AC  83C20C            add dx,byte +0xc
000003AF  A0AA37            mov al,[0x37aa]
000003B2  98                cbw
000003B3  BB1A00            mov bx,0x1a
000003B6  52                push dx
000003B7  F7EB              imul bx
000003B9  8BD8              mov bx,ax
000003BB  58                pop ax
000003BC  89872233          mov [bx+0x3322],ax
000003C0  FE06E63C          inc byte [0x3ce6]
000003C4  803E821501        cmp byte [0x1582],0x1
000003C9  7505              jnz 0x3d0
000003CB  9ACC1F0906        call 0x609:0x1fcc
000003D0  FE06AA37          inc byte [0x37aa]
000003D4  803EAA3704        cmp byte [0x37aa],0x4
000003D9  7D03              jnl 0x3de
000003DB  E95CFF            jmp 0x33a
000003DE  5D                pop bp
000003DF  CB                retf

; ---- 0609:03E0 ----
000003E0  55                push bp
000003E1  8BEC              mov bp,sp
000003E3  E98400            jmp 0x46a
000003E6  9A1F05FD21        call 0x21fd:0x51f
000003EB  FF362E08          push word [0x82e]
000003EF  9A1C07CD23        call 0x23cd:0x71c
000003F4  59                pop cx
000003F5  9AA806960A        call 0xa96:0x6a8
000003FA  9A1B00960A        call 0xa96:0x1b
000003FF  B80500            mov ax,0x5
00000402  2B068E2E          sub ax,[0x2e8e]
00000406  99                cwd
00000407  52                push dx
00000408  50                push ax
00000409  9A8501FD21        call 0x21fd:0x185
0000040E  59                pop cx
0000040F  59                pop cx
00000410  9A7A039E0E        call 0xe9e:0x37a
00000415  9A4502FD21        call 0x21fd:0x245
0000041A  C606AA3700        mov byte [0x37aa],0x0
0000041F  EB33              jmp short 0x454
00000421  A0AA37            mov al,[0x37aa]
00000424  98                cbw
00000425  8BD8              mov bx,ax
00000427  80BFB24306        cmp byte [bx+0x43b2],0x6
0000042C  7502              jnz 0x430
0000042E  EB20              jmp short 0x450
00000430  A0AA37            mov al,[0x37aa]
00000433  98                cbw
00000434  D1E0              shl ax,1
00000436  8BD8              mov bx,ax
00000438  83BF223000        cmp word [bx+0x3022],byte +0x0
0000043D  7407              jz 0x446
0000043F  9AE01F0906        call 0x609:0x1fe0
00000444  EB0A              jmp short 0x450
00000446  9A7B040906        call 0x609:0x47b
0000044B  9A4D060906        call 0x609:0x64d
00000450  FE06AA37          inc byte [0x37aa]
00000454  803EAA3704        cmp byte [0x37aa],0x4
00000459  7CC6              jl 0x421
0000045B  9AA806960A        call 0xa96:0x6a8
00000460  FF362E08          push word [0x82e]
00000464  9AE906CD23        call 0x23cd:0x6e9
00000469  59                pop cx
0000046A  803EE63C00        cmp byte [0x3ce6],0x0
0000046F  7403              jz 0x474
00000471  E972FF            jmp 0x3e6
00000474  9A6506FD21        call 0x21fd:0x665
00000479  5D                pop bp
0000047A  CB                retf

; ---- 0609:047B ----
0000047B  55                push bp
0000047C  8BEC              mov bp,sp
0000047E  56                push si
0000047F  57                push di
00000480  A0AA37            mov al,[0x37aa]
00000483  98                cbw
00000484  8BD8              mov bx,ax
00000486  8A874E30          mov al,[bx+0x304e]
0000048A  98                cbw
0000048B  8BF8              mov di,ax
0000048D  A0AA37            mov al,[0x37aa]
00000490  98                cbw
00000491  8BD8              mov bx,ax
00000493  8A875A30          mov al,[bx+0x305a]
00000497  98                cbw
00000498  8BF0              mov si,ax
0000049A  A0AA37            mov al,[0x37aa]
0000049D  98                cbw
0000049E  8BD8              mov bx,ax
000004A0  80BF400801        cmp byte [bx+0x840],0x1
000004A5  7509              jnz 0x4b0
000004A7  56                push si
000004A8  57                push di
000004A9  9A0E005D13        call 0x135d:0xe
000004AE  59                pop cx
000004AF  59                pop cx
000004B0  A0AA37            mov al,[0x37aa]
000004B3  98                cbw
000004B4  8BD8              mov bx,ax
000004B6  80BFB24304        cmp byte [bx+0x43b2],0x4
000004BB  750C              jnz 0x4c9
000004BD  56                push si
000004BE  57                push di
000004BF  9A8E0B0906        call 0x609:0xb8e
000004C4  59                pop cx
000004C5  59                pop cx
000004C6  E97801            jmp 0x641
000004C9  A0AA37            mov al,[0x37aa]
000004CC  98                cbw
000004CD  8BD8              mov bx,ax
000004CF  8A870833          mov al,[bx+0x3308]
000004D3  98                cbw
000004D4  8BD8              mov bx,ax
000004D6  83FB03            cmp bx,byte +0x3
000004D9  7603              jna 0x4de
000004DB  E94801            jmp 0x626
000004DE  D1E3              shl bx,1
000004E0  2EFFA74506        jmp [cs:bx+0x645]
000004E5  A0AA37            mov al,[0x37aa]
000004E8  98                cbw
000004E9  8BD8              mov bx,ax
000004EB  80BF4E3000        cmp byte [bx+0x304e],0x0
000004F0  7E34              jng 0x526
000004F2  A0AA37            mov al,[0x37aa]
000004F5  98                cbw
000004F6  D1E0              shl ax,1
000004F8  8BD8              mov bx,ax
000004FA  C787CA4B0100      mov word [bx+0x4bca],0x1
00000500  A0AA37            mov al,[0x37aa]
00000503  98                cbw
00000504  D1E0              shl ax,1
00000506  8BD8              mov bx,ax
00000508  C78722301100      mov word [bx+0x3022],0x11
0000050E  A0AA37            mov al,[0x37aa]
00000511  98                cbw
00000512  D1E0              shl ax,1
00000514  8BD8              mov bx,ax
00000516  C787D24B0800      mov word [bx+0x4bd2],0x8
0000051C  A0AA37            mov al,[0x37aa]
0000051F  98                cbw
00000520  8BD8              mov bx,ax
00000522  FE8F4E30          dec byte [bx+0x304e]
00000526  E9FD00            jmp 0x626
00000529  A0AA37            mov al,[0x37aa]
0000052C  98                cbw
0000052D  8BD8              mov bx,ax
0000052F  8A874E30          mov al,[bx+0x304e]
00000533  98                cbw
00000534  8B16FC08          mov dx,[0x8fc]
00000538  4A                dec dx
00000539  3BC2              cmp ax,dx
0000053B  7D34              jnl 0x571
0000053D  A0AA37            mov al,[0x37aa]
00000540  98                cbw
00000541  D1E0              shl ax,1
00000543  8BD8              mov bx,ax
00000545  C787CA4B0000      mov word [bx+0x4bca],0x0
0000054B  A0AA37            mov al,[0x37aa]
0000054E  98                cbw
0000054F  D1E0              shl ax,1
00000551  8BD8              mov bx,ax
00000553  C78722301100      mov word [bx+0x3022],0x11
00000559  A0AA37            mov al,[0x37aa]
0000055C  98                cbw
0000055D  D1E0              shl ax,1
0000055F  8BD8              mov bx,ax
00000561  C787D24B0800      mov word [bx+0x4bd2],0x8
00000567  A0AA37            mov al,[0x37aa]
0000056A  98                cbw
0000056B  8BD8              mov bx,ax
0000056D  FE874E30          inc byte [bx+0x304e]
00000571  E9B200            jmp 0x626
00000574  A0AA37            mov al,[0x37aa]
00000577  98                cbw
00000578  8BD8              mov bx,ax
0000057A  80BF5A3000        cmp byte [bx+0x305a],0x0
0000057F  7E47              jng 0x5c8
00000581  8BC7              mov ax,di
00000583  BA0600            mov dx,0x6
00000586  F7EA              imul dx
00000588  8BD8              mov bx,ax
0000058A  80B8B12E04        cmp byte [bx+si+0x2eb1],0x4
0000058F  7503              jnz 0x594
00000591  E99200            jmp 0x626
00000594  A0AA37            mov al,[0x37aa]
00000597  98                cbw
00000598  D1E0              shl ax,1
0000059A  8BD8              mov bx,ax
0000059C  C787CA4B0200      mov word [bx+0x4bca],0x2
000005A2  A0AA37            mov al,[0x37aa]
000005A5  98                cbw
000005A6  D1E0              shl ax,1
000005A8  8BD8              mov bx,ax
000005AA  C78722300C00      mov word [bx+0x3022],0xc
000005B0  A0AA37            mov al,[0x37aa]
000005B3  98                cbw
000005B4  D1E0              shl ax,1
000005B6  8BD8              mov bx,ax
000005B8  C787D24B0B00      mov word [bx+0x4bd2],0xb
000005BE  A0AA37            mov al,[0x37aa]
000005C1  98                cbw
000005C2  8BD8              mov bx,ax
000005C4  FE8F5A30          dec byte [bx+0x305a]
000005C8  EB5C              jmp short 0x626
000005CA  A0AA37            mov al,[0x37aa]
000005CD  98                cbw
000005CE  8BD8              mov bx,ax
000005D0  8A875A30          mov al,[bx+0x305a]
000005D4  98                cbw
000005D5  8B16FE08          mov dx,[0x8fe]
000005D9  4A                dec dx
000005DA  3BC2              cmp ax,dx
000005DC  7D46              jnl 0x624
000005DE  8BC7              mov ax,di
000005E0  BA0600            mov dx,0x6
000005E3  F7EA              imul dx
000005E5  8BD8              mov bx,ax
000005E7  80B8B22E04        cmp byte [bx+si+0x2eb2],0x4
000005EC  7502              jnz 0x5f0
000005EE  EB36              jmp short 0x626
000005F0  A0AA37            mov al,[0x37aa]
000005F3  98                cbw
000005F4  D1E0              shl ax,1
000005F6  8BD8              mov bx,ax
000005F8  C787CA4B0300      mov word [bx+0x4bca],0x3
000005FE  A0AA37            mov al,[0x37aa]
00000601  98                cbw
00000602  D1E0              shl ax,1
00000604  8BD8              mov bx,ax
00000606  C78722300C00      mov word [bx+0x3022],0xc
0000060C  A0AA37            mov al,[0x37aa]
0000060F  98                cbw
00000610  D1E0              shl ax,1
00000612  8BD8              mov bx,ax
00000614  C787D24B0100      mov word [bx+0x4bd2],0x1
0000061A  A0AA37            mov al,[0x37aa]
0000061D  98                cbw
0000061E  8BD8              mov bx,ax
00000620  FE875A30          inc byte [bx+0x305a]
00000624  EB00              jmp short 0x626
00000626  A0AA37            mov al,[0x37aa]
00000629  98                cbw
0000062A  D1E0              shl ax,1
0000062C  8BD8              mov bx,ax
0000062E  83BF223000        cmp word [bx+0x3022],byte +0x0
00000633  7407              jz 0x63c
00000635  9AA10E0906        call 0x609:0xea1
0000063A  EB05              jmp short 0x641
0000063C  9A5E210906        call 0x609:0x215e
00000641  5F                pop di
00000642  5E                pop si
00000643  5D                pop bp
00000644  CB                retf
00000645  2905              sub [di],ax
00000647  E504              in ax,0x4
00000649  7405              jz 0x650
0000064B  CA0555            retf 0x5505
0000064E  8BEC              mov bp,sp
00000650  A0AA37            mov al,[0x37aa]
00000653  98                cbw
00000654  8BD8              mov bx,ax
00000656  80BF08330A        cmp byte [bx+0x3308],0xa
0000065B  7538              jnz 0x695
0000065D  A0AA37            mov al,[0x37aa]
00000660  98                cbw
00000661  8BD8              mov bx,ax
00000663  8A87B243          mov al,[bx+0x43b2]
00000667  B400              mov ah,0x0
00000669  48                dec ax
0000066A  48                dec ax
0000066B  8BD8              mov bx,ax
0000066D  83FB03            cmp bx,byte +0x3
00000670  7723              ja 0x695
00000672  D1E3              shl bx,1
00000674  2EFFA79706        jmp [cs:bx+0x697]
00000679  9A9F060906        call 0x609:0x69f
0000067E  EB15              jmp short 0x695
00000680  9AD6070906        call 0x609:0x7d6
00000685  EB0E              jmp short 0x695
00000687  9A6A090906        call 0x609:0x96a
0000068C  EB07              jmp short 0x695
0000068E  9AEF0A0906        call 0x609:0xaef
00000693  EB00              jmp short 0x695
00000695  5D                pop bp
00000696  CB                retf
00000697  7906              jns 0x69f
00000699  800687068E        add byte [0x687],0x8e
0000069E  06                push es

; ---- 0609:069F ----
0000069F  55                push bp
000006A0  8BEC              mov bp,sp
000006A2  A0AA37            mov al,[0x37aa]
000006A5  98                cbw
000006A6  8BD8              mov bx,ax
000006A8  8A874E30          mov al,[bx+0x304e]
000006AC  A2B02E            mov [0x2eb0],al
000006AF  A0AA37            mov al,[0x37aa]
000006B2  98                cbw
000006B3  8BD8              mov bx,ax
000006B5  8A875A30          mov al,[bx+0x305a]
000006B9  A2002F            mov [0x2f00],al
000006BC  9A4D070906        call 0x609:0x74d
000006C1  0AC0              or al,al
000006C3  7503              jnz 0x6c8
000006C5  E98300            jmp 0x74b
000006C8  B80400            mov ax,0x4
000006CB  50                push ax
000006CC  A0AA37            mov al,[0x37aa]
000006CF  98                cbw
000006D0  50                push ax
000006D1  A0002F            mov al,[0x2f00]
000006D4  98                cbw
000006D5  50                push ax
000006D6  A0B02E            mov al,[0x2eb0]
000006D9  98                cbw
000006DA  50                push ax
000006DB  9AD2100906        call 0x609:0x10d2
000006E0  83C408            add sp,byte +0x8
000006E3  A0AA37            mov al,[0x37aa]
000006E6  98                cbw
000006E7  8BD8              mov bx,ax
000006E9  8A874E30          mov al,[bx+0x304e]
000006ED  50                push ax
000006EE  A0AA37            mov al,[0x37aa]
000006F1  98                cbw
000006F2  8BD8              mov bx,ax
000006F4  58                pop ax
000006F5  8887FE40          mov [bx+0x40fe],al
000006F9  A0AA37            mov al,[0x37aa]
000006FC  98                cbw
000006FD  8BD8              mov bx,ax
000006FF  8A875A30          mov al,[bx+0x305a]
00000703  50                push ax
00000704  A0AA37            mov al,[0x37aa]
00000707  98                cbw
00000708  8BD8              mov bx,ax
0000070A  58                pop ax
0000070B  88870241          mov [bx+0x4102],al
0000070F  A0002F            mov al,[0x2f00]
00000712  98                cbw
00000713  50                push ax
00000714  A0B02E            mov al,[0x2eb0]
00000717  98                cbw
00000718  50                push ax
00000719  9A831C0906        call 0x609:0x1c83
0000071E  59                pop cx
0000071F  59                pop cx
00000720  B80700            mov ax,0x7
00000723  50                push ax
00000724  33C0              xor ax,ax
00000726  50                push ax
00000727  9A5400A721        call 0x21a7:0x54
0000072C  59                pop cx
0000072D  59                pop cx
0000072E  A0AA37            mov al,[0x37aa]
00000731  98                cbw
00000732  8BD8              mov bx,ax
00000734  C687B24303        mov byte [bx+0x43b2],0x3
00000739  A0AA37            mov al,[0x37aa]
0000073C  98                cbw
0000073D  8BD8              mov bx,ax
0000073F  80BF400801        cmp byte [bx+0x840],0x1
00000744  7505              jnz 0x74b
00000746  9AB30A5D13        call 0x135d:0xab3
0000074B  5D                pop bp
0000074C  CB                retf

; ---- 0609:074D ----
0000074D  55                push bp
0000074E  8BEC              mov bp,sp
00000750  A0B02E            mov al,[0x2eb0]
00000753  98                cbw
00000754  BA0600            mov dx,0x6
00000757  F7EA              imul dx
00000759  50                push ax
0000075A  A0002F            mov al,[0x2f00]
0000075D  98                cbw
0000075E  5B                pop bx
0000075F  03D8              add bx,ax
00000761  80BFDA4503        cmp byte [bx+0x45da],0x3
00000766  7518              jnz 0x780
00000768  A0B02E            mov al,[0x2eb0]
0000076B  98                cbw
0000076C  BA0600            mov dx,0x6
0000076F  F7EA              imul dx
00000771  50                push ax
00000772  A0002F            mov al,[0x2f00]
00000775  98                cbw
00000776  5B                pop bx
00000777  03D8              add bx,ax
00000779  80BF4244FF        cmp byte [bx+0x4442],0xff
0000077E  7404              jz 0x784
00000780  B000              mov al,0x0
00000782  EB50              jmp short 0x7d4
00000784  803EB02E00        cmp byte [0x2eb0],0x0
00000789  7E1C              jng 0x7a7
0000078B  A0B02E            mov al,[0x2eb0]
0000078E  98                cbw
0000078F  BA0600            mov dx,0x6
00000792  F7EA              imul dx
00000794  50                push ax
00000795  A0002F            mov al,[0x2f00]
00000798  98                cbw
00000799  5B                pop bx
0000079A  03D8              add bx,ax
0000079C  80BFAB2E04        cmp byte [bx+0x2eab],0x4
000007A1  7504              jnz 0x7a7
000007A3  B000              mov al,0x0
000007A5  EB2D              jmp short 0x7d4
000007A7  A0B02E            mov al,[0x2eb0]
000007AA  98                cbw
000007AB  8B16FC08          mov dx,[0x8fc]
000007AF  4A                dec dx
000007B0  3BC2              cmp ax,dx
000007B2  7D1C              jnl 0x7d0
000007B4  A0B02E            mov al,[0x2eb0]
000007B7  98                cbw
000007B8  BA0600            mov dx,0x6
000007BB  F7EA              imul dx
000007BD  50                push ax
000007BE  A0002F            mov al,[0x2f00]
000007C1  98                cbw
000007C2  5B                pop bx
000007C3  03D8              add bx,ax
000007C5  80BFB72E04        cmp byte [bx+0x2eb7],0x4
000007CA  7504              jnz 0x7d0
000007CC  B000              mov al,0x0
000007CE  EB04              jmp short 0x7d4
000007D0  B001              mov al,0x1
000007D2  EB00              jmp short 0x7d4
000007D4  5D                pop bp
000007D5  CB                retf

; ---- 0609:07D6 ----
000007D6  55                push bp
000007D7  8BEC              mov bp,sp
000007D9  56                push si
000007DA  57                push di
000007DB  A0AA37            mov al,[0x37aa]
000007DE  98                cbw
000007DF  8BD8              mov bx,ax
000007E1  8A874E30          mov al,[bx+0x304e]
000007E5  98                cbw
000007E6  8BF8              mov di,ax
000007E8  A0AA37            mov al,[0x37aa]
000007EB  98                cbw
000007EC  8BD8              mov bx,ax
000007EE  8A875A30          mov al,[bx+0x305a]
000007F2  98                cbw
000007F3  8BF0              mov si,ax
000007F5  8BC7              mov ax,di
000007F7  BA0600            mov dx,0x6
000007FA  F7EA              imul dx
000007FC  8BD8              mov bx,ax
000007FE  8A804244          mov al,[bx+si+0x4442]
00000802  3A06AA37          cmp al,[0x37aa]
00000806  7403              jz 0x80b
00000808  E9DA00            jmp 0x8e5
0000080B  A0AA37            mov al,[0x37aa]
0000080E  98                cbw
0000080F  8BD8              mov bx,ax
00000811  8A87FE40          mov al,[bx+0x40fe]
00000815  98                cbw
00000816  3BC7              cmp ax,di
00000818  756D              jnz 0x887
0000081A  A0AA37            mov al,[0x37aa]
0000081D  98                cbw
0000081E  8BD8              mov bx,ax
00000820  8A870241          mov al,[bx+0x4102]
00000824  98                cbw
00000825  3BC6              cmp ax,si
00000827  755E              jnz 0x887
00000829  9ADD0E0906        call 0x609:0xedd
0000082E  B80800            mov ax,0x8
00000831  50                push ax
00000832  33C0              xor ax,ax
00000834  50                push ax
00000835  9A5400A721        call 0x21a7:0x54
0000083A  59                pop cx
0000083B  59                pop cx
0000083C  B80600            mov ax,0x6
0000083F  50                push ax
00000840  A0AA37            mov al,[0x37aa]
00000843  98                cbw
00000844  B106              mov cl,0x6
00000846  D3E0              shl ax,cl
00000848  052000            add ax,0x20
0000084B  50                push ax
0000084C  9A54180906        call 0x609:0x1854
00000851  59                pop cx
00000852  59                pop cx
00000853  33C0              xor ax,ax
00000855  BA3200            mov dx,0x32
00000858  50                push ax
00000859  52                push dx
0000085A  A0AA37            mov al,[0x37aa]
0000085D  98                cbw
0000085E  D1E0              shl ax,1
00000860  D1E0              shl ax,1
00000862  8BD8              mov bx,ax
00000864  FFB7D43C          push word [bx+0x3cd4]
00000868  FFB7D23C          push word [bx+0x3cd2]
0000086C  9A894F0000        call 0x0:0x4f89
00000871  50                push ax
00000872  A0AA37            mov al,[0x37aa]
00000875  98                cbw
00000876  D1E0              shl ax,1
00000878  D1E0              shl ax,1
0000087A  8BD8              mov bx,ax
0000087C  58                pop ax
0000087D  0187D23C          add [bx+0x3cd2],ax
00000881  1197D43C          adc [bx+0x3cd4],dx
00000885  EB5B              jmp short 0x8e2
00000887  A0AA37            mov al,[0x37aa]
0000088A  98                cbw
0000088B  8BD8              mov bx,ax
0000088D  C687B24304        mov byte [bx+0x43b2],0x4
00000892  A0AA37            mov al,[0x37aa]
00000895  98                cbw
00000896  8BD8              mov bx,ax
00000898  C687942EFF        mov byte [bx+0x2e94],0xff
0000089D  A0AA37            mov al,[0x37aa]
000008A0  98                cbw
000008A1  8BD8              mov bx,ax
000008A3  8A87AF3C          mov al,[bx+0x3caf]
000008A7  98                cbw
000008A8  3BC7              cmp ax,di
000008AA  7523              jnz 0x8cf
000008AC  A0AA37            mov al,[0x37aa]
000008AF  98                cbw
000008B0  8BD8              mov bx,ax
000008B2  8A87B33C          mov al,[bx+0x3cb3]
000008B6  98                cbw
000008B7  3BC6              cmp ax,si
000008B9  7514              jnz 0x8cf
000008BB  A0AA37            mov al,[0x37aa]
000008BE  98                cbw
000008BF  8BD8              mov bx,ax
000008C1  C687AF3CFF        mov byte [bx+0x3caf],0xff
000008C6  56                push si
000008C7  57                push di
000008C8  9A831C0906        call 0x609:0x1c83
000008CD  59                pop cx
000008CE  59                pop cx
000008CF  B80900            mov ax,0x9
000008D2  50                push ax
000008D3  33C0              xor ax,ax
000008D5  50                push ax
000008D6  9A5400A721        call 0x21a7:0x54
000008DB  59                pop cx
000008DC  59                pop cx
000008DD  9A730C0906        call 0x609:0xc73
000008E2  E98100            jmp 0x966
000008E5  56                push si
000008E6  57                push di
000008E7  9A0F100906        call 0x609:0x100f
000008EC  59                pop cx
000008ED  59                pop cx
000008EE  3C01              cmp al,0x1
000008F0  7574              jnz 0x966
000008F2  8BC7              mov ax,di
000008F4  BA0600            mov dx,0x6
000008F7  F7EA              imul dx
000008F9  8BD8              mov bx,ax
000008FB  80B84244FF        cmp byte [bx+si+0x4442],0xff
00000900  7564              jnz 0x966
00000902  A0AA37            mov al,[0x37aa]
00000905  98                cbw
00000906  8BD8              mov bx,ax
00000908  80BFFA4301        cmp byte [bx+0x43fa],0x1
0000090D  7557              jnz 0x966
0000090F  8BC7              mov ax,di
00000911  BA0600            mov dx,0x6
00000914  F7EA              imul dx
00000916  8BD8              mov bx,ax
00000918  8A80B12E          mov al,[bx+si+0x2eb1]
0000091C  98                cbw
0000091D  50                push ax
0000091E  A0AA37            mov al,[0x37aa]
00000921  98                cbw
00000922  50                push ax
00000923  56                push si
00000924  57                push di
00000925  9AD2100906        call 0x609:0x10d2
0000092A  83C408            add sp,byte +0x8
0000092D  B80A00            mov ax,0xa
00000930  50                push ax
00000931  33C0              xor ax,ax
00000933  50                push ax
00000934  9A5400A721        call 0x21a7:0x54
00000939  59                pop cx
0000093A  59                pop cx
0000093B  56                push si
0000093C  57                push di
0000093D  9AB11C0906        call 0x609:0x1cb1
00000942  59                pop cx
00000943  59                pop cx
00000944  A0AA37            mov al,[0x37aa]
00000947  98                cbw
00000948  8BD8              mov bx,ax
0000094A  C687FA4300        mov byte [bx+0x43fa],0x0
0000094F  9A971A0906        call 0x609:0x1a97
00000954  A0AA37            mov al,[0x37aa]
00000957  98                cbw
00000958  8BD8              mov bx,ax
0000095A  80BF400801        cmp byte [bx+0x840],0x1
0000095F  7505              jnz 0x966
00000961  9A35095D13        call 0x135d:0x935
00000966  5F                pop di
00000967  5E                pop si
00000968  5D                pop bp
00000969  CB                retf

; ---- 0609:096A ----
0000096A  55                push bp
0000096B  8BEC              mov bp,sp
0000096D  83EC06            sub sp,byte +0x6
00000970  56                push si
00000971  57                push di
00000972  A0AA37            mov al,[0x37aa]
00000975  98                cbw
00000976  8BD8              mov bx,ax
00000978  8A874E30          mov al,[bx+0x304e]
0000097C  98                cbw
0000097D  8946FE            mov [bp-0x2],ax
00000980  A0AA37            mov al,[0x37aa]
00000983  98                cbw
00000984  8BD8              mov bx,ax
00000986  8A875A30          mov al,[bx+0x305a]
0000098A  98                cbw
0000098B  8946FC            mov [bp-0x4],ax
0000098E  A0AA37            mov al,[0x37aa]
00000991  98                cbw
00000992  8BD8              mov bx,ax
00000994  80BF942EFF        cmp byte [bx+0x2e94],0xff
00000999  7511              jnz 0x9ac
0000099B  B80B00            mov ax,0xb
0000099E  50                push ax
0000099F  33C0              xor ax,ax
000009A1  50                push ax
000009A2  9A5400A721        call 0x21a7:0x54
000009A7  59                pop cx
000009A8  59                pop cx
000009A9  E92D01            jmp 0xad9
000009AC  A0AA37            mov al,[0x37aa]
000009AF  98                cbw
000009B0  8BD8              mov bx,ax
000009B2  80BF942E04        cmp byte [bx+0x2e94],0x4
000009B7  7555              jnz 0xa0e
000009B9  A0AA37            mov al,[0x37aa]
000009BC  98                cbw
000009BD  8BD8              mov bx,ax
000009BF  8A87AF3C          mov al,[bx+0x3caf]
000009C3  98                cbw
000009C4  8BF8              mov di,ax
000009C6  A0AA37            mov al,[0x37aa]
000009C9  98                cbw
000009CA  8BD8              mov bx,ax
000009CC  8A87B33C          mov al,[bx+0x3cb3]
000009D0  98                cbw
000009D1  8946FA            mov [bp-0x6],ax
000009D4  A0AA37            mov al,[0x37aa]
000009D7  98                cbw
000009D8  8A56FE            mov dl,[bp-0x2]
000009DB  8BD8              mov bx,ax
000009DD  8897AF3C          mov [bx+0x3caf],dl
000009E1  A0AA37            mov al,[0x37aa]
000009E4  98                cbw
000009E5  8A56FC            mov dl,[bp-0x4]
000009E8  8BD8              mov bx,ax
000009EA  8897B33C          mov [bx+0x3cb3],dl
000009EE  FF76FC            push word [bp-0x4]
000009F1  FF76FE            push word [bp-0x2]
000009F4  9A831C0906        call 0x609:0x1c83
000009F9  59                pop cx
000009FA  59                pop cx
000009FB  83FFFF            cmp di,byte -0x1
000009FE  740B              jz 0xa0b
00000A00  FF76FA            push word [bp-0x6]
00000A03  57                push di
00000A04  9A831C0906        call 0x609:0x1c83
00000A09  59                pop cx
00000A0A  59                pop cx
00000A0B  E9CB00            jmp 0xad9
00000A0E  A0AA37            mov al,[0x37aa]
00000A11  98                cbw
00000A12  8BD8              mov bx,ax
00000A14  80BF942E03        cmp byte [bx+0x2e94],0x3
00000A19  7537              jnz 0xa52
00000A1B  A0AA37            mov al,[0x37aa]
00000A1E  98                cbw
00000A1F  D1E0              shl ax,1
00000A21  8BD8              mov bx,ax
00000A23  83BFAC4B00        cmp word [bx+0x4bac],byte +0x0
00000A28  7503              jnz 0xa2d
00000A2A  E9BC00            jmp 0xae9
00000A2D  8B46FE            mov ax,[bp-0x2]
00000A30  BA0600            mov dx,0x6
00000A33  F7EA              imul dx
00000A35  0346FC            add ax,[bp-0x4]
00000A38  8BD8              mov bx,ax
00000A3A  80BFDA4503        cmp byte [bx+0x45da],0x3
00000A3F  7503              jnz 0xa44
00000A41  E9A500            jmp 0xae9
00000A44  A0AA37            mov al,[0x37aa]
00000A47  98                cbw
00000A48  D1E0              shl ax,1
00000A4A  8BD8              mov bx,ax
00000A4C  FF8FAC4B          dec word [bx+0x4bac]
00000A50  EB4D              jmp short 0xa9f
00000A52  A0AA37            mov al,[0x37aa]
00000A55  98                cbw
00000A56  8BD8              mov bx,ax
00000A58  8A87942E          mov al,[bx+0x2e94]
00000A5C  98                cbw
00000A5D  40                inc ax
00000A5E  8BF0              mov si,ax
00000A60  3D0300            cmp ax,0x3
00000A63  7502              jnz 0xa67
00000A65  33F6              xor si,si
00000A67  A0AA37            mov al,[0x37aa]
00000A6A  98                cbw
00000A6B  D1E0              shl ax,1
00000A6D  D1E0              shl ax,1
00000A6F  D1E0              shl ax,1
00000A71  8BD6              mov dx,si
00000A73  D1E2              shl dx,1
00000A75  03C2              add ax,dx
00000A77  50                push ax
00000A78  A0A248            mov al,[0x48a2]
00000A7B  98                cbw
00000A7C  5B                pop bx
00000A7D  3987F43C          cmp [bx+0x3cf4],ax
00000A81  7D02              jnl 0xa85
00000A83  EB64              jmp short 0xae9
00000A85  A0AA37            mov al,[0x37aa]
00000A88  98                cbw
00000A89  D1E0              shl ax,1
00000A8B  D1E0              shl ax,1
00000A8D  D1E0              shl ax,1
00000A8F  8BD6              mov dx,si
00000A91  D1E2              shl dx,1
00000A93  03C2              add ax,dx
00000A95  50                push ax
00000A96  A0A248            mov al,[0x48a2]
00000A99  98                cbw
00000A9A  5B                pop bx
00000A9B  2987F43C          sub [bx+0x3cf4],ax
00000A9F  A0AA37            mov al,[0x37aa]
00000AA2  98                cbw
00000AA3  8BD8              mov bx,ax
00000AA5  8A87942E          mov al,[bx+0x2e94]
00000AA9  98                cbw
00000AAA  50                push ax
00000AAB  A0AA37            mov al,[0x37aa]
00000AAE  98                cbw
00000AAF  50                push ax
00000AB0  FF76FC            push word [bp-0x4]
00000AB3  FF76FE            push word [bp-0x2]
00000AB6  9AD2100906        call 0x609:0x10d2
00000ABB  83C408            add sp,byte +0x8
00000ABE  FF76FC            push word [bp-0x4]
00000AC1  FF76FE            push word [bp-0x2]
00000AC4  9A831C0906        call 0x609:0x1c83
00000AC9  59                pop cx
00000ACA  59                pop cx
00000ACB  B80C00            mov ax,0xc
00000ACE  50                push ax
00000ACF  33C0              xor ax,ax
00000AD1  50                push ax
00000AD2  9A5400A721        call 0x21a7:0x54
00000AD7  59                pop cx
00000AD8  59                pop cx
00000AD9  9A031A0906        call 0x609:0x1a03
00000ADE  A0AA37            mov al,[0x37aa]
00000AE1  98                cbw
00000AE2  8BD8              mov bx,ax
00000AE4  C687B24303        mov byte [bx+0x43b2],0x3
00000AE9  5F                pop di
00000AEA  5E                pop si
00000AEB  8BE5              mov sp,bp
00000AED  5D                pop bp
00000AEE  CB                retf

; ---- 0609:0AEF ----
00000AEF  55                push bp
00000AF0  8BEC              mov bp,sp
00000AF2  56                push si
00000AF3  57                push di
00000AF4  A0AA37            mov al,[0x37aa]
00000AF7  98                cbw
00000AF8  8BD8              mov bx,ax
00000AFA  8A874E30          mov al,[bx+0x304e]
00000AFE  98                cbw
00000AFF  8BF8              mov di,ax
00000B01  A0AA37            mov al,[0x37aa]
00000B04  98                cbw
00000B05  8BD8              mov bx,ax
00000B07  8A875A30          mov al,[bx+0x305a]
00000B0B  98                cbw
00000B0C  8BF0              mov si,ax
00000B0E  A0AA37            mov al,[0x37aa]
00000B11  98                cbw
00000B12  8BD8              mov bx,ax
00000B14  8A87FE40          mov al,[bx+0x40fe]
00000B18  98                cbw
00000B19  3BC7              cmp ax,di
00000B1B  7516              jnz 0xb33
00000B1D  A0AA37            mov al,[0x37aa]
00000B20  98                cbw
00000B21  8BD8              mov bx,ax
00000B23  8A870241          mov al,[bx+0x4102]
00000B27  98                cbw
00000B28  3BC6              cmp ax,si
00000B2A  7507              jnz 0xb33
00000B2C  9ADD0E0906        call 0x609:0xedd
00000B31  EB57              jmp short 0xb8a
00000B33  8BC7              mov ax,di
00000B35  BA0600            mov dx,0x6
00000B38  F7EA              imul dx
00000B3A  8BD8              mov bx,ax
00000B3C  80B84244FF        cmp byte [bx+si+0x4442],0xff
00000B41  7402              jz 0xb45
00000B43  EB45              jmp short 0xb8a
00000B45  B80D00            mov ax,0xd
00000B48  50                push ax
00000B49  33C0              xor ax,ax
00000B4B  50                push ax
00000B4C  9A5400A721        call 0x21a7:0x54
00000B51  59                pop cx
00000B52  59                pop cx
00000B53  8BC7              mov ax,di
00000B55  BA0600            mov dx,0x6
00000B58  F7EA              imul dx
00000B5A  8BD8              mov bx,ax
00000B5C  8A80B12E          mov al,[bx+si+0x2eb1]
00000B60  98                cbw
00000B61  50                push ax
00000B62  A0AA37            mov al,[0x37aa]
00000B65  98                cbw
00000B66  50                push ax
00000B67  56                push si
00000B68  57                push di
00000B69  9AD2100906        call 0x609:0x10d2
00000B6E  83C408            add sp,byte +0x8
00000B71  56                push si
00000B72  57                push di
00000B73  9AB11C0906        call 0x609:0x1cb1
00000B78  59                pop cx
00000B79  59                pop cx
00000B7A  A0AA37            mov al,[0x37aa]
00000B7D  98                cbw
00000B7E  8BD8              mov bx,ax
00000B80  C687FA4300        mov byte [bx+0x43fa],0x0
00000B85  9ADD0E0906        call 0x609:0xedd
00000B8A  5F                pop di
00000B8B  5E                pop si
00000B8C  5D                pop bp
00000B8D  CB                retf

; ---- 0609:0B8E ----
00000B8E  55                push bp
00000B8F  8BEC              mov bp,sp
00000B91  A0AA37            mov al,[0x37aa]
00000B94  98                cbw
00000B95  8BD8              mov bx,ax
00000B97  8A870833          mov al,[bx+0x3308]
00000B9B  98                cbw
00000B9C  3D0200            cmp ax,0x2
00000B9F  7407              jz 0xba8
00000BA1  3D0300            cmp ax,0x3
00000BA4  7422              jz 0xbc8
00000BA6  EB40              jmp short 0xbe8
00000BA8  FF7608            push word [bp+0x8]
00000BAB  FF7606            push word [bp+0x6]
00000BAE  B8FFFF            mov ax,0xffff
00000BB1  50                push ax
00000BB2  A0AA37            mov al,[0x37aa]
00000BB5  98                cbw
00000BB6  8BD8              mov bx,ax
00000BB8  8A87942E          mov al,[bx+0x2e94]
00000BBC  98                cbw
00000BBD  50                push ax
00000BBE  9AEA0B0906        call 0x609:0xbea
00000BC3  83C408            add sp,byte +0x8
00000BC6  EB20              jmp short 0xbe8
00000BC8  FF7608            push word [bp+0x8]
00000BCB  FF7606            push word [bp+0x6]
00000BCE  B80100            mov ax,0x1
00000BD1  50                push ax
00000BD2  A0AA37            mov al,[0x37aa]
00000BD5  98                cbw
00000BD6  8BD8              mov bx,ax
00000BD8  8A87942E          mov al,[bx+0x2e94]
00000BDC  98                cbw
00000BDD  50                push ax
00000BDE  9AEA0B0906        call 0x609:0xbea
00000BE3  83C408            add sp,byte +0x8
00000BE6  EB00              jmp short 0xbe8
00000BE8  5D                pop bp
00000BE9  CB                retf

; ---- 0609:0BEA ----
00000BEA  55                push bp
00000BEB  8BEC              mov bp,sp
00000BED  56                push si
00000BEE  8B7608            mov si,[bp+0x8]
00000BF1  017606            add [bp+0x6],si
00000BF4  837E0603          cmp word [bp+0x6],byte +0x3
00000BF8  7517              jnz 0xc11
00000BFA  8B460A            mov ax,[bp+0xa]
00000BFD  BA0600            mov dx,0x6
00000C00  F7EA              imul dx
00000C02  03460C            add ax,[bp+0xc]
00000C05  8BD8              mov bx,ax
00000C07  80BFDA4503        cmp byte [bx+0x45da],0x3
00000C0C  7503              jnz 0xc11
00000C0E  017606            add [bp+0x6],si
00000C11  837E0604          cmp word [bp+0x6],byte +0x4
00000C15  7514              jnz 0xc2b
00000C17  FF760C            push word [bp+0xc]
00000C1A  FF760A            push word [bp+0xa]
00000C1D  9A0D0F0906        call 0x609:0xf0d
00000C22  59                pop cx
00000C23  59                pop cx
00000C24  0AC0              or al,al
00000C26  7503              jnz 0xc2b
00000C28  017606            add [bp+0x6],si
00000C2B  837E06FF          cmp word [bp+0x6],byte -0x1
00000C2F  7C06              jl 0xc37
00000C31  837E0604          cmp word [bp+0x6],byte +0x4
00000C35  7E02              jng 0xc39
00000C37  EB37              jmp short 0xc70
00000C39  A0AA37            mov al,[0x37aa]
00000C3C  98                cbw
00000C3D  8A5606            mov dl,[bp+0x6]
00000C40  8BD8              mov bx,ax
00000C42  8897942E          mov [bx+0x2e94],dl
00000C46  9A730C0906        call 0x609:0xc73
00000C4B  A0AA37            mov al,[0x37aa]
00000C4E  98                cbw
00000C4F  D1E0              shl ax,1
00000C51  D1E0              shl ax,1
00000C53  8BD8              mov bx,ax
00000C55  C78786370400      mov word [bx+0x3786],0x4
00000C5B  C78788370000      mov word [bx+0x3788],0x0
00000C61  B80E00            mov ax,0xe
00000C64  50                push ax
00000C65  B80100            mov ax,0x1
00000C68  50                push ax
00000C69  9A5400A721        call 0x21a7:0x54
00000C6E  59                pop cx
00000C6F  59                pop cx
00000C70  5E                pop si
00000C71  5D                pop bp
00000C72  CB                retf

; ---- 0609:0C73 ----
00000C73  55                push bp
00000C74  8BEC              mov bp,sp
00000C76  9AE5180906        call 0x609:0x18e5
00000C7B  9A3C190906        call 0x609:0x193c
00000C80  A0AA37            mov al,[0x37aa]
00000C83  98                cbw
00000C84  8BD8              mov bx,ax
00000C86  8A87942E          mov al,[bx+0x2e94]
00000C8A  98                cbw
00000C8B  3DFFFF            cmp ax,0xffff
00000C8E  740C              jz 0xc9c
00000C90  3D0300            cmp ax,0x3
00000C93  7431              jz 0xcc6
00000C95  3D0400            cmp ax,0x4
00000C98  7433              jz 0xccd
00000C9A  EB5D              jmp short 0xcf9
00000C9C  B8FFFF            mov ax,0xffff
00000C9F  50                push ax
00000CA0  B80800            mov ax,0x8
00000CA3  50                push ax
00000CA4  FF36F237          push word [0x37f2]
00000CA8  FF36F037          push word [0x37f0]
00000CAC  B82300            mov ax,0x23
00000CAF  50                push ax
00000CB0  A0AA37            mov al,[0x37aa]
00000CB3  98                cbw
00000CB4  B106              mov cl,0x6
00000CB6  D3E0              shl ax,cl
00000CB8  052D00            add ax,0x2d
00000CBB  50                push ax
00000CBC  9A2505960A        call 0xa96:0x525
00000CC1  83C40C            add sp,byte +0xc
00000CC4  EB3A              jmp short 0xd00
00000CC6  9A78190906        call 0x609:0x1978
00000CCB  EB33              jmp short 0xd00
00000CCD  B8FFFF            mov ax,0xffff
00000CD0  50                push ax
00000CD1  B80D00            mov ax,0xd
00000CD4  50                push ax
00000CD5  B80400            mov ax,0x4
00000CD8  50                push ax
00000CD9  A14637            mov ax,[0x3746]
00000CDC  99                cwd
00000CDD  52                push dx
00000CDE  50                push ax
00000CDF  B82300            mov ax,0x23
00000CE2  50                push ax
00000CE3  A0AA37            mov al,[0x37aa]
00000CE6  98                cbw
00000CE7  B106              mov cl,0x6
00000CE9  D3E0              shl ax,cl
00000CEB  052A00            add ax,0x2a
00000CEE  50                push ax
00000CEF  9AFB05960A        call 0xa96:0x5fb
00000CF4  83C40E            add sp,byte +0xe
00000CF7  EB07              jmp short 0xd00
00000CF9  9A020D0906        call 0x609:0xd02
00000CFE  EB00              jmp short 0xd00
00000D00  5D                pop bp
00000D01  CB                retf

; ---- 0609:0D02 ----
00000D02  55                push bp
00000D03  8BEC              mov bp,sp
00000D05  83EC06            sub sp,byte +0x6
00000D08  56                push si
00000D09  57                push di
00000D0A  A0AA37            mov al,[0x37aa]
00000D0D  98                cbw
00000D0E  8BD8              mov bx,ax
00000D10  8A87942E          mov al,[bx+0x2e94]
00000D14  98                cbw
00000D15  8BF0              mov si,ax
00000D17  8A84E708          mov al,[si+0x8e7]
00000D1B  98                cbw
00000D1C  8946FA            mov [bp-0x6],ax
00000D1F  8A84E408          mov al,[si+0x8e4]
00000D23  98                cbw
00000D24  8BF8              mov di,ax
00000D26  B81B00            mov ax,0x1b
00000D29  50                push ax
00000D2A  A0AA37            mov al,[0x37aa]
00000D2D  98                cbw
00000D2E  B106              mov cl,0x6
00000D30  D3E0              shl ax,cl
00000D32  052000            add ax,0x20
00000D35  50                push ax
00000D36  FF76FA            push word [bp-0x6]
00000D39  9A12190906        call 0x609:0x1912
00000D3E  83C406            add sp,byte +0x6
00000D41  B82300            mov ax,0x23
00000D44  50                push ax
00000D45  A0AA37            mov al,[0x37aa]
00000D48  98                cbw
00000D49  B106              mov cl,0x6
00000D4B  D3E0              shl ax,cl
00000D4D  052000            add ax,0x20
00000D50  50                push ax
00000D51  57                push di
00000D52  9A12190906        call 0x609:0x1912
00000D57  83C406            add sp,byte +0x6
00000D5A  A0AA37            mov al,[0x37aa]
00000D5D  98                cbw
00000D5E  D1E0              shl ax,1
00000D60  D1E0              shl ax,1
00000D62  D1E0              shl ax,1
00000D64  8B56FA            mov dx,[bp-0x6]
00000D67  D1E2              shl dx,1
00000D69  03C2              add ax,dx
00000D6B  8BD8              mov bx,ax
00000D6D  8B87F43C          mov ax,[bx+0x3cf4]
00000D71  8946FC            mov [bp-0x4],ax
00000D74  A0AA37            mov al,[0x37aa]
00000D77  98                cbw
00000D78  D1E0              shl ax,1
00000D7A  D1E0              shl ax,1
00000D7C  8BD8              mov bx,ax
00000D7E  8A81E72E          mov al,[bx+di+0x2ee7]
00000D82  98                cbw
00000D83  50                push ax
00000D84  A0262F            mov al,[0x2f26]
00000D87  98                cbw
00000D88  8BD0              mov dx,ax
00000D8A  58                pop ax
00000D8B  F7EA              imul dx
00000D8D  8946FE            mov [bp-0x2],ax
00000D90  B81C00            mov ax,0x1c
00000D93  50                push ax
00000D94  A0A248            mov al,[0x48a2]
00000D97  98                cbw
00000D98  50                push ax
00000D99  FF76FE            push word [bp-0x2]
00000D9C  FF76FC            push word [bp-0x4]
00000D9F  9AF90D0906        call 0x609:0xdf9
00000DA4  83C408            add sp,byte +0x8
00000DA7  A0AA37            mov al,[0x37aa]
00000DAA  98                cbw
00000DAB  D1E0              shl ax,1
00000DAD  D1E0              shl ax,1
00000DAF  D1E0              shl ax,1
00000DB1  8BD7              mov dx,di
00000DB3  D1E2              shl dx,1
00000DB5  03C2              add ax,dx
00000DB7  8BD8              mov bx,ax
00000DB9  8B87F43C          mov ax,[bx+0x3cf4]
00000DBD  8946FC            mov [bp-0x4],ax
00000DC0  A0AA37            mov al,[0x37aa]
00000DC3  98                cbw
00000DC4  D1E0              shl ax,1
00000DC6  D1E0              shl ax,1
00000DC8  8BD8              mov bx,ax
00000DCA  8A80E72E          mov al,[bx+si+0x2ee7]
00000DCE  98                cbw
00000DCF  50                push ax
00000DD0  A0262F            mov al,[0x2f26]
00000DD3  98                cbw
00000DD4  8BD0              mov dx,ax
00000DD6  58                pop ax
00000DD7  F7EA              imul dx
00000DD9  8946FE            mov [bp-0x2],ax
00000DDC  B82400            mov ax,0x24
00000DDF  50                push ax
00000DE0  A0262F            mov al,[0x2f26]
00000DE3  98                cbw
00000DE4  50                push ax
00000DE5  FF76FE            push word [bp-0x2]
00000DE8  FF76FC            push word [bp-0x4]
00000DEB  9AF90D0906        call 0x609:0xdf9
00000DF0  83C408            add sp,byte +0x8
00000DF3  5F                pop di
00000DF4  5E                pop si
00000DF5  8BE5              mov sp,bp
00000DF7  5D                pop bp
00000DF8  CB                retf

; ---- 0609:0DF9 ----
00000DF9  55                push bp
00000DFA  8BEC              mov bp,sp
00000DFC  83EC04            sub sp,byte +0x4
00000DFF  56                push si
00000E00  57                push di
00000E01  A0262F            mov al,[0x2f26]
00000E04  98                cbw
00000E05  50                push ax
00000E06  8B4606            mov ax,[bp+0x6]
00000E09  99                cwd
00000E0A  5B                pop bx
00000E0B  F7FB              idiv bx
00000E0D  40                inc ax
00000E0E  8BF0              mov si,ax
00000E10  83FE1F            cmp si,byte +0x1f
00000E13  7E03              jng 0xe18
00000E15  BE1F00            mov si,0x1f
00000E18  A0262F            mov al,[0x2f26]
00000E1B  98                cbw
00000E1C  50                push ax
00000E1D  8B4608            mov ax,[bp+0x8]
00000E20  99                cwd
00000E21  5B                pop bx
00000E22  F7FB              idiv bx
00000E24  8BF8              mov di,ax
00000E26  8B4608            mov ax,[bp+0x8]
00000E29  3B4606            cmp ax,[bp+0x6]
00000E2C  7E05              jng 0xe33
00000E2E  B80800            mov ax,0x8
00000E31  EB13              jmp short 0xe46
00000E33  8B4608            mov ax,[bp+0x8]
00000E36  03460A            add ax,[bp+0xa]
00000E39  3B4606            cmp ax,[bp+0x6]
00000E3C  7E05              jng 0xe43
00000E3E  B80E00            mov ax,0xe
00000E41  EB03              jmp short 0xe46
00000E43  B80D00            mov ax,0xd
00000E46  8946FC            mov [bp-0x4],ax
00000E49  FF76FC            push word [bp-0x4]
00000E4C  B80500            mov ax,0x5
00000E4F  50                push ax
00000E50  56                push si
00000E51  FF760C            push word [bp+0xc]
00000E54  A0AA37            mov al,[0x37aa]
00000E57  98                cbw
00000E58  B106              mov cl,0x6
00000E5A  D3E0              shl ax,cl
00000E5C  052E00            add ax,0x2e
00000E5F  50                push ax
00000E60  9AD704960A        call 0xa96:0x4d7
00000E65  83C40A            add sp,byte +0xa
00000E68  83FF1E            cmp di,byte +0x1e
00000E6B  7F2E              jg 0xe9b
00000E6D  A0AA37            mov al,[0x37aa]
00000E70  98                cbw
00000E71  B106              mov cl,0x6
00000E73  D3E0              shl ax,cl
00000E75  03C7              add ax,di
00000E77  052E00            add ax,0x2e
00000E7A  8946FE            mov [bp-0x2],ax
00000E7D  B80200            mov ax,0x2
00000E80  50                push ax
00000E81  8B460C            mov ax,[bp+0xc]
00000E84  050600            add ax,0x6
00000E87  50                push ax
00000E88  FF76FE            push word [bp-0x2]
00000E8B  8B460C            mov ax,[bp+0xc]
00000E8E  48                dec ax
00000E8F  50                push ax
00000E90  FF76FE            push word [bp-0x2]
00000E93  9A3304960A        call 0xa96:0x433
00000E98  83C40A            add sp,byte +0xa
00000E9B  5F                pop di
00000E9C  5E                pop si
00000E9D  8BE5              mov sp,bp
00000E9F  5D                pop bp
00000EA0  CB                retf

; ---- 0609:0EA1 ----
00000EA1  55                push bp
00000EA2  8BEC              mov bp,sp
00000EA4  803E821501        cmp byte [0x1582],0x1
00000EA9  7407              jz 0xeb2
00000EAB  803EBA4500        cmp byte [0x45ba],0x0
00000EB0  7504              jnz 0xeb6
00000EB2  B001              mov al,0x1
00000EB4  EB25              jmp short 0xedb
00000EB6  A0AA37            mov al,[0x37aa]
00000EB9  98                cbw
00000EBA  D1E0              shl ax,1
00000EBC  8BD8              mov bx,ax
00000EBE  83BF204500        cmp word [bx+0x4520],byte +0x0
00000EC3  7509              jnz 0xece
00000EC5  9ADD0E0906        call 0x609:0xedd
00000ECA  B000              mov al,0x0
00000ECC  EB0D              jmp short 0xedb
00000ECE  B0FF              mov al,0xff
00000ED0  50                push ax
00000ED1  9A721B0906        call 0x609:0x1b72
00000ED6  59                pop cx
00000ED7  B001              mov al,0x1
00000ED9  EB00              jmp short 0xedb
00000EDB  5D                pop bp
00000EDC  CB                retf

; ---- 0609:0EDD ----
00000EDD  55                push bp
00000EDE  8BEC              mov bp,sp
00000EE0  FE0EE63C          dec byte [0x3ce6]
00000EE4  A0AA37            mov al,[0x37aa]
00000EE7  98                cbw
00000EE8  8BD8              mov bx,ax
00000EEA  C687B24306        mov byte [bx+0x43b2],0x6
00000EEF  9ABA210906        call 0x609:0x21ba
00000EF4  B80600            mov ax,0x6
00000EF7  50                push ax
00000EF8  A0AA37            mov al,[0x37aa]
00000EFB  98                cbw
00000EFC  B106              mov cl,0x6
00000EFE  D3E0              shl ax,cl
00000F00  052000            add ax,0x20
00000F03  50                push ax
00000F04  9A54180906        call 0x609:0x1854
00000F09  59                pop cx
00000F0A  59                pop cx
00000F0B  5D                pop bp
00000F0C  CB                retf

; ---- 0609:0F0D ----
00000F0D  55                push bp
00000F0E  8BEC              mov bp,sp
00000F10  56                push si
00000F11  57                push di
00000F12  8B7E06            mov di,[bp+0x6]
00000F15  8B7608            mov si,[bp+0x8]
00000F18  56                push si
00000F19  8BC7              mov ax,di
00000F1B  40                inc ax
00000F1C  50                push ax
00000F1D  9A700F0906        call 0x609:0xf70
00000F22  59                pop cx
00000F23  59                pop cx
00000F24  3C01              cmp al,0x1
00000F26  7504              jnz 0xf2c
00000F28  B001              mov al,0x1
00000F2A  EB40              jmp short 0xf6c
00000F2C  56                push si
00000F2D  8BC7              mov ax,di
00000F2F  48                dec ax
00000F30  50                push ax
00000F31  9A700F0906        call 0x609:0xf70
00000F36  59                pop cx
00000F37  59                pop cx
00000F38  3C01              cmp al,0x1
00000F3A  7504              jnz 0xf40
00000F3C  B001              mov al,0x1
00000F3E  EB2C              jmp short 0xf6c
00000F40  8BC6              mov ax,si
00000F42  40                inc ax
00000F43  50                push ax
00000F44  57                push di
00000F45  9A700F0906        call 0x609:0xf70
00000F4A  59                pop cx
00000F4B  59                pop cx
00000F4C  3C01              cmp al,0x1
00000F4E  7504              jnz 0xf54
00000F50  B001              mov al,0x1
00000F52  EB18              jmp short 0xf6c
00000F54  8BC6              mov ax,si
00000F56  48                dec ax
00000F57  50                push ax
00000F58  57                push di
00000F59  9A700F0906        call 0x609:0xf70
00000F5E  59                pop cx
00000F5F  59                pop cx
00000F60  3C01              cmp al,0x1
00000F62  7504              jnz 0xf68
00000F64  B001              mov al,0x1
00000F66  EB04              jmp short 0xf6c
00000F68  B000              mov al,0x0
00000F6A  EB00              jmp short 0xf6c
00000F6C  5F                pop di
00000F6D  5E                pop si
00000F6E  5D                pop bp
00000F6F  CB                retf

; ---- 0609:0F70 ----
00000F70  55                push bp
00000F71  8BEC              mov bp,sp
00000F73  FF7608            push word [bp+0x8]
00000F76  FF7606            push word [bp+0x6]
00000F79  9AA7100906        call 0x609:0x10a7
00000F7E  59                pop cx
00000F7F  59                pop cx
00000F80  0AC0              or al,al
00000F82  7504              jnz 0xf88
00000F84  B000              mov al,0x0
00000F86  EB1F              jmp short 0xfa7
00000F88  8B4606            mov ax,[bp+0x6]
00000F8B  BA0600            mov dx,0x6
00000F8E  F7EA              imul dx
00000F90  034608            add ax,[bp+0x8]
00000F93  8BD8              mov bx,ax
00000F95  8A874244          mov al,[bx+0x4442]
00000F99  3A06AA37          cmp al,[0x37aa]
00000F9D  7504              jnz 0xfa3
00000F9F  B000              mov al,0x0
00000FA1  EB04              jmp short 0xfa7
00000FA3  B001              mov al,0x1
00000FA5  EB00              jmp short 0xfa7
00000FA7  5D                pop bp
00000FA8  CB                retf

; ---- 0609:0FA9 ----
00000FA9  55                push bp
00000FAA  8BEC              mov bp,sp
00000FAC  C606AA3700        mov byte [0x37aa],0x0
00000FB1  EB53              jmp short 0x1006
00000FB3  A0AA37            mov al,[0x37aa]
00000FB6  98                cbw
00000FB7  8BD8              mov bx,ax
00000FB9  80BFAF3CFF        cmp byte [bx+0x3caf],0xff
00000FBE  7502              jnz 0xfc2
00000FC0  EB40              jmp short 0x1002
00000FC2  B8FEFF            mov ax,0xfffe
00000FC5  50                push ax
00000FC6  B8FFFF            mov ax,0xffff
00000FC9  50                push ax
00000FCA  A0AA37            mov al,[0x37aa]
00000FCD  98                cbw
00000FCE  8BD8              mov bx,ax
00000FD0  8A87B33C          mov al,[bx+0x3cb3]
00000FD4  98                cbw
00000FD5  50                push ax
00000FD6  A0AA37            mov al,[0x37aa]
00000FD9  98                cbw
00000FDA  8BD8              mov bx,ax
00000FDC  8A87AF3C          mov al,[bx+0x3caf]
00000FE0  98                cbw
00000FE1  50                push ax
00000FE2  9AD2100906        call 0x609:0x10d2
00000FE7  83C408            add sp,byte +0x8
00000FEA  A14637            mov ax,[0x3746]
00000FED  99                cwd
00000FEE  50                push ax
00000FEF  A0AA37            mov al,[0x37aa]
00000FF2  98                cbw
00000FF3  D1E0              shl ax,1
00000FF5  D1E0              shl ax,1
00000FF7  8BD8              mov bx,ax
00000FF9  58                pop ax
00000FFA  0187D23C          add [bx+0x3cd2],ax
00000FFE  1197D43C          adc [bx+0x3cd4],dx
00001002  FE06AA37          inc byte [0x37aa]
00001006  803EAA3704        cmp byte [0x37aa],0x4
0000100B  7CA6              jl 0xfb3
0000100D  5D                pop bp
0000100E  CB                retf

; ---- 0609:100F ----
0000100F  55                push bp
00001010  8BEC              mov bp,sp
00001012  56                push si
00001013  57                push di
00001014  8B7E06            mov di,[bp+0x6]
00001017  8B7608            mov si,[bp+0x8]
0000101A  56                push si
0000101B  8BC7              mov ax,di
0000101D  48                dec ax
0000101E  50                push ax
0000101F  9A72100906        call 0x609:0x1072
00001024  59                pop cx
00001025  59                pop cx
00001026  3C01              cmp al,0x1
00001028  7504              jnz 0x102e
0000102A  B001              mov al,0x1
0000102C  EB40              jmp short 0x106e
0000102E  56                push si
0000102F  8BC7              mov ax,di
00001031  40                inc ax
00001032  50                push ax
00001033  9A72100906        call 0x609:0x1072
00001038  59                pop cx
00001039  59                pop cx
0000103A  3C01              cmp al,0x1
0000103C  7504              jnz 0x1042
0000103E  B001              mov al,0x1
00001040  EB2C              jmp short 0x106e
00001042  8BC6              mov ax,si
00001044  48                dec ax
00001045  50                push ax
00001046  57                push di
00001047  9A72100906        call 0x609:0x1072
0000104C  59                pop cx
0000104D  59                pop cx
0000104E  3C01              cmp al,0x1
00001050  7504              jnz 0x1056
00001052  B001              mov al,0x1
00001054  EB18              jmp short 0x106e
00001056  8BC6              mov ax,si
00001058  40                inc ax
00001059  50                push ax
0000105A  57                push di
0000105B  9A72100906        call 0x609:0x1072
00001060  59                pop cx
00001061  59                pop cx
00001062  3C01              cmp al,0x1
00001064  7504              jnz 0x106a
00001066  B001              mov al,0x1
00001068  EB04              jmp short 0x106e
0000106A  B000              mov al,0x0
0000106C  EB00              jmp short 0x106e
0000106E  5F                pop di
0000106F  5E                pop si
00001070  5D                pop bp
00001071  CB                retf

; ---- 0609:1072 ----
00001072  55                push bp
00001073  8BEC              mov bp,sp
00001075  FF7608            push word [bp+0x8]
00001078  FF7606            push word [bp+0x6]
0000107B  9AA7100906        call 0x609:0x10a7
00001080  59                pop cx
00001081  59                pop cx
00001082  3C01              cmp al,0x1
00001084  751B              jnz 0x10a1
00001086  8B4606            mov ax,[bp+0x6]
00001089  BA0600            mov dx,0x6
0000108C  F7EA              imul dx
0000108E  034608            add ax,[bp+0x8]
00001091  8BD8              mov bx,ax
00001093  8A874244          mov al,[bx+0x4442]
00001097  3A06AA37          cmp al,[0x37aa]
0000109B  7504              jnz 0x10a1
0000109D  B001              mov al,0x1
0000109F  EB04              jmp short 0x10a5
000010A1  B000              mov al,0x0
000010A3  EB00              jmp short 0x10a5
000010A5  5D                pop bp
000010A6  CB                retf

; ---- 0609:10A7 ----
000010A7  55                push bp
000010A8  8BEC              mov bp,sp
000010AA  837E0600          cmp word [bp+0x6],byte +0x0
000010AE  7C18              jl 0x10c8
000010B0  A1FC08            mov ax,[0x8fc]
000010B3  48                dec ax
000010B4  3B4606            cmp ax,[bp+0x6]
000010B7  7C0F              jl 0x10c8
000010B9  837E0800          cmp word [bp+0x8],byte +0x0
000010BD  7C09              jl 0x10c8
000010BF  A1FE08            mov ax,[0x8fe]
000010C2  48                dec ax
000010C3  3B4608            cmp ax,[bp+0x8]
000010C6  7D04              jnl 0x10cc
000010C8  B000              mov al,0x0
000010CA  EB04              jmp short 0x10d0
000010CC  B001              mov al,0x1
000010CE  EB00              jmp short 0x10d0
000010D0  5D                pop bp
000010D1  CB                retf

; ---- 0609:10D2 ----
000010D2  55                push bp
000010D3  8BEC              mov bp,sp
000010D5  56                push si
000010D6  57                push di
000010D7  8B4E0C            mov cx,[bp+0xc]
000010DA  8B4606            mov ax,[bp+0x6]
000010DD  BA0600            mov dx,0x6
000010E0  F7EA              imul dx
000010E2  034608            add ax,[bp+0x8]
000010E5  8BD8              mov bx,ax
000010E7  8A874244          mov al,[bx+0x4442]
000010EB  98                cbw
000010EC  8BF0              mov si,ax
000010EE  8B4606            mov ax,[bp+0x6]
000010F1  BA0600            mov dx,0x6
000010F4  F7EA              imul dx
000010F6  034608            add ax,[bp+0x8]
000010F9  8BD8              mov bx,ax
000010FB  8A87B12E          mov al,[bx+0x2eb1]
000010FF  98                cbw
00001100  8BF8              mov di,ax
00001102  83FEFF            cmp si,byte -0x1
00001105  7506              jnz 0x110d
00001107  FE0E052F          dec byte [0x2f05]
0000110B  EB17              jmp short 0x1124
0000110D  FE8CA84B          dec byte [si+0x4ba8]
00001111  0BFF              or di,di
00001113  7C0F              jl 0x1124
00001115  83FF03            cmp di,byte +0x3
00001118  7F0A              jg 0x1124
0000111A  8BDE              mov bx,si
0000111C  D1E3              shl bx,1
0000111E  D1E3              shl bx,1
00001120  FE89E72E          dec byte [bx+di+0x2ee7]
00001124  837E0AFF          cmp word [bp+0xa],byte -0x1
00001128  7506              jnz 0x1130
0000112A  FE06052F          inc byte [0x2f05]
0000112E  EB1D              jmp short 0x114d
00001130  8B5E0A            mov bx,[bp+0xa]
00001133  FE87A84B          inc byte [bx+0x4ba8]
00001137  0BC9              or cx,cx
00001139  7C12              jl 0x114d
0000113B  83F903            cmp cx,byte +0x3
0000113E  7F0D              jg 0x114d
00001140  8B5E0A            mov bx,[bp+0xa]
00001143  D1E3              shl bx,1
00001145  D1E3              shl bx,1
00001147  03D9              add bx,cx
00001149  FE87E72E          inc byte [bx+0x2ee7]
0000114D  8B4606            mov ax,[bp+0x6]
00001150  BA0600            mov dx,0x6
00001153  F7EA              imul dx
00001155  034608            add ax,[bp+0x8]
00001158  8A560A            mov dl,[bp+0xa]
0000115B  8BD8              mov bx,ax
0000115D  88974244          mov [bx+0x4442],dl
00001161  8B4606            mov ax,[bp+0x6]
00001164  BA0600            mov dx,0x6
00001167  F7EA              imul dx
00001169  034608            add ax,[bp+0x8]
0000116C  8AD1              mov dl,cl
0000116E  8BD8              mov bx,ax
00001170  8897B12E          mov [bx+0x2eb1],dl
00001174  5F                pop di
00001175  5E                pop si
00001176  5D                pop bp
00001177  CB                retf

; ---- 0609:1178 ----
00001178  55                push bp
00001179  8BEC              mov bp,sp
0000117B  56                push si
0000117C  33F6              xor si,si
0000117E  C606AA3700        mov byte [0x37aa],0x0
00001183  EB2C              jmp short 0x11b1
00001185  C706D0450000      mov word [0x45d0],0x0
0000118B  EB19              jmp short 0x11a6
0000118D  A0AA37            mov al,[0x37aa]
00001190  98                cbw
00001191  D1E0              shl ax,1
00001193  D1E0              shl ax,1
00001195  0306D045          add ax,[0x45d0]
00001199  8BD8              mov bx,ax
0000119B  8A87E72E          mov al,[bx+0x2ee7]
0000119F  98                cbw
000011A0  03F0              add si,ax
000011A2  FF06D045          inc word [0x45d0]
000011A6  833ED04502        cmp word [0x45d0],byte +0x2
000011AB  7EE0              jng 0x118d
000011AD  FE06AA37          inc byte [0x37aa]
000011B1  803EAA3704        cmp byte [0x37aa],0x4
000011B6  7CCD              jl 0x1185
000011B8  8BC6              mov ax,si
000011BA  BB0200            mov bx,0x2
000011BD  99                cwd
000011BE  F7FB              idiv bx
000011C0  0406              add al,0x6
000011C2  A2A248            mov [0x48a2],al
000011C5  3C1E              cmp al,0x1e
000011C7  7E05              jng 0x11ce
000011C9  C606A2481E        mov byte [0x48a2],0x1e
000011CE  8BC6              mov ax,si
000011D0  BB0200            mov bx,0x2
000011D3  99                cwd
000011D4  F7FB              idiv bx
000011D6  0403              add al,0x3
000011D8  A2262F            mov [0x2f26],al
000011DB  3C1E              cmp al,0x1e
000011DD  7E05              jng 0x11e4
000011DF  C606262F1E        mov byte [0x2f26],0x1e
000011E4  5E                pop si
000011E5  5D                pop bp
000011E6  CB                retf

; ---- 0609:11E7 ----
000011E7  55                push bp
000011E8  8BEC              mov bp,sp
000011EA  803EE64003        cmp byte [0x40e6],0x3
000011EF  752F              jnz 0x1220
000011F1  B86407            mov ax,0x764
000011F4  50                push ax
000011F5  B80100            mov ax,0x1
000011F8  50                push ax
000011F9  9AC902FD21        call 0x21fd:0x2c9
000011FE  59                pop cx
000011FF  59                pop cx
00001200  B84206            mov ax,0x642
00001203  50                push ax
00001204  B80300            mov ax,0x3
00001207  50                push ax
00001208  9AC902FD21        call 0x21fd:0x2c9
0000120D  59                pop cx
0000120E  59                pop cx
0000120F  B82004            mov ax,0x420
00001212  50                push ax
00001213  B80400            mov ax,0x4
00001216  50                push ax
00001217  9AC902FD21        call 0x21fd:0x2c9
0000121C  59                pop cx
0000121D  59                pop cx
0000121E  EB34              jmp short 0x1254
00001220  803EE64005        cmp byte [0x40e6],0x5
00001225  752D              jnz 0x1254
00001227  B86606            mov ax,0x666
0000122A  50                push ax
0000122B  B80100            mov ax,0x1
0000122E  50                push ax
0000122F  9AC902FD21        call 0x21fd:0x2c9
00001234  59                pop cx
00001235  59                pop cx
00001236  B85505            mov ax,0x555
00001239  50                push ax
0000123A  B80300            mov ax,0x3
0000123D  50                push ax
0000123E  9AC902FD21        call 0x21fd:0x2c9
00001243  59                pop cx
00001244  59                pop cx
00001245  B83303            mov ax,0x333
00001248  50                push ax
00001249  B80400            mov ax,0x4
0000124C  50                push ax
0000124D  9AC902FD21        call 0x21fd:0x2c9
00001252  59                pop cx
00001253  59                pop cx
00001254  5D                pop bp
00001255  CB                retf

; ---- 0609:1256 ----
00001256  55                push bp
00001257  8BEC              mov bp,sp
00001259  56                push si
0000125A  57                push di
0000125B  8B7E06            mov di,[bp+0x6]
0000125E  8B7608            mov si,[bp+0x8]
00001261  56                push si
00001262  57                push di
00001263  B81600            mov ax,0x16
00001266  50                push ax
00001267  B81000            mov ax,0x10
0000126A  50                push ax
0000126B  33C0              xor ax,ax
0000126D  50                push ax
0000126E  B88000            mov ax,0x80
00001271  50                push ax
00001272  9A3000960A        call 0xa96:0x30
00001277  83C40C            add sp,byte +0xc
0000127A  56                push si
0000127B  8BC7              mov ax,di
0000127D  03460A            add ax,[bp+0xa]
00001280  05F0FF            add ax,0xfff0
00001283  50                push ax
00001284  B81600            mov ax,0x16
00001287  50                push ax
00001288  B81000            mov ax,0x10
0000128B  50                push ax
0000128C  33C0              xor ax,ax
0000128E  50                push ax
0000128F  B89000            mov ax,0x90
00001292  50                push ax
00001293  9A3000960A        call 0xa96:0x30
00001298  83C40C            add sp,byte +0xc
0000129B  8BC6              mov ax,si
0000129D  03460C            add ax,[bp+0xc]
000012A0  05EAFF            add ax,0xffea
000012A3  50                push ax
000012A4  57                push di
000012A5  B81600            mov ax,0x16
000012A8  50                push ax
000012A9  B81000            mov ax,0x10
000012AC  50                push ax
000012AD  B84200            mov ax,0x42
000012B0  50                push ax
000012B1  B88000            mov ax,0x80
000012B4  50                push ax
000012B5  9A3000960A        call 0xa96:0x30
000012BA  83C40C            add sp,byte +0xc
000012BD  8BC6              mov ax,si
000012BF  03460C            add ax,[bp+0xc]
000012C2  05EAFF            add ax,0xffea
000012C5  50                push ax
000012C6  8BC7              mov ax,di
000012C8  03460A            add ax,[bp+0xa]
000012CB  05F0FF            add ax,0xfff0
000012CE  50                push ax
000012CF  B81600            mov ax,0x16
000012D2  50                push ax
000012D3  B81000            mov ax,0x10
000012D6  50                push ax
000012D7  B84200            mov ax,0x42
000012DA  50                push ax
000012DB  B89000            mov ax,0x90
000012DE  50                push ax
000012DF  9A3000960A        call 0xa96:0x30
000012E4  83C40C            add sp,byte +0xc
000012E7  B80B00            mov ax,0xb
000012EA  50                push ax
000012EB  8B460A            mov ax,[bp+0xa]
000012EE  05E0FF            add ax,0xffe0
000012F1  50                push ax
000012F2  56                push si
000012F3  8BC7              mov ax,di
000012F5  051000            add ax,0x10
000012F8  50                push ax
000012F9  B80B00            mov ax,0xb
000012FC  50                push ax
000012FD  B82000            mov ax,0x20
00001300  50                push ax
00001301  B89A00            mov ax,0x9a
00001304  50                push ax
00001305  B82001            mov ax,0x120
00001308  50                push ax
00001309  9A6702960A        call 0xa96:0x267
0000130E  83C410            add sp,byte +0x10
00001311  B80B00            mov ax,0xb
00001314  50                push ax
00001315  8B460A            mov ax,[bp+0xa]
00001318  05E0FF            add ax,0xffe0
0000131B  50                push ax
0000131C  8BC6              mov ax,si
0000131E  03460C            add ax,[bp+0xc]
00001321  05F5FF            add ax,0xfff5
00001324  50                push ax
00001325  8BC7              mov ax,di
00001327  051000            add ax,0x10
0000132A  50                push ax
0000132B  B80B00            mov ax,0xb
0000132E  50                push ax
0000132F  B82000            mov ax,0x20
00001332  50                push ax
00001333  B8A500            mov ax,0xa5
00001336  50                push ax
00001337  B82001            mov ax,0x120
0000133A  50                push ax
0000133B  9A6702960A        call 0xa96:0x267
00001340  83C410            add sp,byte +0x10
00001343  8B460C            mov ax,[bp+0xc]
00001346  05D4FF            add ax,0xffd4
00001349  50                push ax
0000134A  B81000            mov ax,0x10
0000134D  50                push ax
0000134E  8BC6              mov ax,si
00001350  051600            add ax,0x16
00001353  50                push ax
00001354  57                push di
00001355  B82C00            mov ax,0x2c
00001358  50                push ax
00001359  B81000            mov ax,0x10
0000135C  50                push ax
0000135D  B81600            mov ax,0x16
00001360  50                push ax
00001361  B88000            mov ax,0x80
00001364  50                push ax
00001365  9A6702960A        call 0xa96:0x267
0000136A  83C410            add sp,byte +0x10
0000136D  8B460C            mov ax,[bp+0xc]
00001370  05D4FF            add ax,0xffd4
00001373  50                push ax
00001374  B81000            mov ax,0x10
00001377  50                push ax
00001378  8BC6              mov ax,si
0000137A  051600            add ax,0x16
0000137D  50                push ax
0000137E  8BC7              mov ax,di
00001380  03460A            add ax,[bp+0xa]
00001383  05F0FF            add ax,0xfff0
00001386  50                push ax
00001387  B82C00            mov ax,0x2c
0000138A  50                push ax
0000138B  B81000            mov ax,0x10
0000138E  50                push ax
0000138F  B81600            mov ax,0x16
00001392  50                push ax
00001393  B89000            mov ax,0x90
00001396  50                push ax
00001397  9A6702960A        call 0xa96:0x267
0000139C  83C410            add sp,byte +0x10
0000139F  8B460C            mov ax,[bp+0xc]
000013A2  05EAFF            add ax,0xffea
000013A5  50                push ax
000013A6  8B460A            mov ax,[bp+0xa]
000013A9  05E0FF            add ax,0xffe0
000013AC  50                push ax
000013AD  8BC6              mov ax,si
000013AF  050B00            add ax,0xb
000013B2  50                push ax
000013B3  8BC7              mov ax,di
000013B5  051000            add ax,0x10
000013B8  50                push ax
000013B9  9A041F0906        call 0x609:0x1f04
000013BE  83C408            add sp,byte +0x8
000013C1  5F                pop di
000013C2  5E                pop si
000013C3  5D                pop bp
000013C4  CB                retf

; ---- 0609:13C5 ----
000013C5  55                push bp
000013C6  8BEC              mov bp,sp
000013C8  4C                dec sp
000013C9  4C                dec sp
000013CA  56                push si
000013CB  57                push di
000013CC  8B7E06            mov di,[bp+0x6]
000013CF  8B7608            mov si,[bp+0x8]
000013D2  8BC6              mov ax,si
000013D4  05FCFF            add ax,0xfffc
000013D7  50                push ax
000013D8  8BC7              mov ax,di
000013DA  05F0FF            add ax,0xfff0
000013DD  50                push ax
000013DE  B80400            mov ax,0x4
000013E1  50                push ax
000013E2  B81000            mov ax,0x10
000013E5  50                push ax
000013E6  B86E00            mov ax,0x6e
000013E9  50                push ax
000013EA  B82001            mov ax,0x120
000013ED  50                push ax
000013EE  9A3000960A        call 0xa96:0x30
000013F3  83C40C            add sp,byte +0xc
000013F6  8BC6              mov ax,si
000013F8  05FCFF            add ax,0xfffc
000013FB  50                push ax
000013FC  8BC7              mov ax,di
000013FE  03460A            add ax,[bp+0xa]
00001401  50                push ax
00001402  B80400            mov ax,0x4
00001405  50                push ax
00001406  B81000            mov ax,0x10
00001409  50                push ax
0000140A  B86E00            mov ax,0x6e
0000140D  50                push ax
0000140E  B83001            mov ax,0x130
00001411  50                push ax
00001412  9A3000960A        call 0xa96:0x30
00001417  83C40C            add sp,byte +0xc
0000141A  8BC6              mov ax,si
0000141C  03460C            add ax,[bp+0xc]
0000141F  50                push ax
00001420  8BC7              mov ax,di
00001422  05F0FF            add ax,0xfff0
00001425  50                push ax
00001426  B80A00            mov ax,0xa
00001429  50                push ax
0000142A  B81000            mov ax,0x10
0000142D  50                push ax
0000142E  B87200            mov ax,0x72
00001431  50                push ax
00001432  B82001            mov ax,0x120
00001435  50                push ax
00001436  9A3000960A        call 0xa96:0x30
0000143B  83C40C            add sp,byte +0xc
0000143E  8BC6              mov ax,si
00001440  03460C            add ax,[bp+0xc]
00001443  50                push ax
00001444  8BC7              mov ax,di
00001446  03460A            add ax,[bp+0xa]
00001449  50                push ax
0000144A  B80A00            mov ax,0xa
0000144D  50                push ax
0000144E  B81000            mov ax,0x10
00001451  50                push ax
00001452  B87200            mov ax,0x72
00001455  50                push ax
00001456  B83001            mov ax,0x130
00001459  50                push ax
0000145A  9A3000960A        call 0xa96:0x30
0000145F  83C40C            add sp,byte +0xc
00001462  C746FE0000        mov word [bp-0x2],0x0
00001467  EB75              jmp short 0x14de
00001469  8BC6              mov ax,si
0000146B  05FCFF            add ax,0xfffc
0000146E  50                push ax
0000146F  8B46FE            mov ax,[bp-0x2]
00001472  B105              mov cl,0x5
00001474  D3E0              shl ax,cl
00001476  8BD7              mov dx,di
00001478  03D0              add dx,ax
0000147A  52                push dx
0000147B  B80400            mov ax,0x4
0000147E  50                push ax
0000147F  B82000            mov ax,0x20
00001482  50                push ax
00001483  B80100            mov ax,0x1
00001486  50                push ax
00001487  9ABA01FD21        call 0x21fd:0x1ba
0000148C  59                pop cx
0000148D  BA0E00            mov dx,0xe
00001490  F7EA              imul dx
00001492  057C00            add ax,0x7c
00001495  50                push ax
00001496  B82001            mov ax,0x120
00001499  50                push ax
0000149A  9A3000960A        call 0xa96:0x30
0000149F  83C40C            add sp,byte +0xc
000014A2  8BC6              mov ax,si
000014A4  03460C            add ax,[bp+0xc]
000014A7  50                push ax
000014A8  8B46FE            mov ax,[bp-0x2]
000014AB  B105              mov cl,0x5
000014AD  D3E0              shl ax,cl
000014AF  8BD7              mov dx,di
000014B1  03D0              add dx,ax
000014B3  52                push dx
000014B4  B80A00            mov ax,0xa
000014B7  50                push ax
000014B8  B82000            mov ax,0x20
000014BB  50                push ax
000014BC  B80100            mov ax,0x1
000014BF  50                push ax
000014C0  9ABA01FD21        call 0x21fd:0x1ba
000014C5  59                pop cx
000014C6  BA0E00            mov dx,0xe
000014C9  F7EA              imul dx
000014CB  058000            add ax,0x80
000014CE  50                push ax
000014CF  B82001            mov ax,0x120
000014D2  50                push ax
000014D3  9A3000960A        call 0xa96:0x30
000014D8  83C40C            add sp,byte +0xc
000014DB  FF46FE            inc word [bp-0x2]
000014DE  8B460A            mov ax,[bp+0xa]
000014E1  BB2000            mov bx,0x20
000014E4  99                cwd
000014E5  F7FB              idiv bx
000014E7  48                dec ax
000014E8  3B46FE            cmp ax,[bp-0x2]
000014EB  7C03              jl 0x14f0
000014ED  E979FF            jmp 0x1469
000014F0  FF760C            push word [bp+0xc]
000014F3  B81000            mov ax,0x10
000014F6  50                push ax
000014F7  56                push si
000014F8  8BC7              mov ax,di
000014FA  05F0FF            add ax,0xfff0
000014FD  50                push ax
000014FE  B85800            mov ax,0x58
00001501  50                push ax
00001502  B81000            mov ax,0x10
00001505  50                push ax
00001506  33C0              xor ax,ax
00001508  50                push ax
00001509  B82001            mov ax,0x120
0000150C  50                push ax
0000150D  9A6702960A        call 0xa96:0x267
00001512  83C410            add sp,byte +0x10
00001515  FF760C            push word [bp+0xc]
00001518  B81000            mov ax,0x10
0000151B  50                push ax
0000151C  56                push si
0000151D  8BC7              mov ax,di
0000151F  03460A            add ax,[bp+0xa]
00001522  50                push ax
00001523  B85800            mov ax,0x58
00001526  50                push ax
00001527  B81000            mov ax,0x10
0000152A  50                push ax
0000152B  33C0              xor ax,ax
0000152D  50                push ax
0000152E  B83001            mov ax,0x130
00001531  50                push ax
00001532  9A6702960A        call 0xa96:0x267
00001537  83C410            add sp,byte +0x10
0000153A  5F                pop di
0000153B  5E                pop si
0000153C  8BE5              mov sp,bp
0000153E  5D                pop bp
0000153F  CB                retf

; ---- 0609:1540 ----
00001540  55                push bp
00001541  8BEC              mov bp,sp
00001543  83EC10            sub sp,byte +0x10
00001546  56                push si
00001547  57                push di
00001548  8B4606            mov ax,[bp+0x6]
0000154B  B105              mov cl,0x5
0000154D  D3E0              shl ax,cl
0000154F  8B161E45          mov dx,[0x451e]
00001553  03D0              add dx,ax
00001555  8BFA              mov di,dx
00001557  8B4608            mov ax,[bp+0x8]
0000155A  BA1600            mov dx,0x16
0000155D  F7EA              imul dx
0000155F  8B162845          mov dx,[0x4528]
00001563  03D0              add dx,ax
00001565  8BF2              mov si,dx
00001567  FF7608            push word [bp+0x8]
0000156A  FF7606            push word [bp+0x6]
0000156D  0E                push cs
0000156E  E836FB            call 0x10a7
00001571  59                pop cx
00001572  59                pop cx
00001573  0AC0              or al,al
00001575  7503              jnz 0x157a
00001577  E9D402            jmp 0x184e
0000157A  8B4606            mov ax,[bp+0x6]
0000157D  BA0600            mov dx,0x6
00001580  F7EA              imul dx
00001582  034608            add ax,[bp+0x8]
00001585  8BD8              mov bx,ax
00001587  8A87B12E          mov al,[bx+0x2eb1]
0000158B  98                cbw
0000158C  8946F4            mov [bp-0xc],ax
0000158F  8B4606            mov ax,[bp+0x6]
00001592  BA0600            mov dx,0x6
00001595  F7EA              imul dx
00001597  034608            add ax,[bp+0x8]
0000159A  8BD8              mov bx,ax
0000159C  8A874244          mov al,[bx+0x4442]
000015A0  98                cbw
000015A1  8946F2            mov [bp-0xe],ax
000015A4  8B4606            mov ax,[bp+0x6]
000015A7  BA0600            mov dx,0x6
000015AA  F7EA              imul dx
000015AC  034608            add ax,[bp+0x8]
000015AF  8BD8              mov bx,ax
000015B1  8A87DA45          mov al,[bx+0x45da]
000015B5  98                cbw
000015B6  8946F0            mov [bp-0x10],ax
000015B9  8B46F0            mov ax,[bp-0x10]
000015BC  3D0200            cmp ax,0x2
000015BF  7476              jz 0x1637
000015C1  3D0300            cmp ax,0x3
000015C4  7403              jz 0x15c9
000015C6  E9C300            jmp 0x168c
000015C9  837EF4FE          cmp word [bp-0xc],byte -0x2
000015CD  751C              jnz 0x15eb
000015CF  56                push si
000015D0  57                push di
000015D1  B81600            mov ax,0x16
000015D4  50                push ax
000015D5  B82000            mov ax,0x20
000015D8  50                push ax
000015D9  B84200            mov ax,0x42
000015DC  50                push ax
000015DD  B8A000            mov ax,0xa0
000015E0  50                push ax
000015E1  9A3000960A        call 0xa96:0x30
000015E6  83C40C            add sp,byte +0xc
000015E9  EB49              jmp short 0x1634
000015EB  837EF404          cmp word [bp-0xc],byte +0x4
000015EF  7524              jnz 0x1615
000015F1  56                push si
000015F2  57                push di
000015F3  B81600            mov ax,0x16
000015F6  50                push ax
000015F7  B82000            mov ax,0x20
000015FA  50                push ax
000015FB  8B46F2            mov ax,[bp-0xe]
000015FE  BA1600            mov dx,0x16
00001601  F7EA              imul dx
00001603  054200            add ax,0x42
00001606  50                push ax
00001607  B82000            mov ax,0x20
0000160A  50                push ax
0000160B  9A3000960A        call 0xa96:0x30
00001610  83C40C            add sp,byte +0xc
00001613  EB1F              jmp short 0x1634
00001615  56                push si
00001616  57                push di
00001617  B81600            mov ax,0x16
0000161A  50                push ax
0000161B  B82000            mov ax,0x20
0000161E  50                push ax
0000161F  8B46F4            mov ax,[bp-0xc]
00001622  BA1600            mov dx,0x16
00001625  F7EA              imul dx
00001627  50                push ax
00001628  B82000            mov ax,0x20
0000162B  50                push ax
0000162C  9A3000960A        call 0xa96:0x30
00001631  83C40C            add sp,byte +0xc
00001634  E91702            jmp 0x184e
00001637  837EF4FE          cmp word [bp-0xc],byte -0x2
0000163B  752E              jnz 0x166b
0000163D  56                push si
0000163E  57                push di
0000163F  B81600            mov ax,0x16
00001642  50                push ax
00001643  B82000            mov ax,0x20
00001646  50                push ax
00001647  8B4606            mov ax,[bp+0x6]
0000164A  F76E08            imul word [bp+0x8]
0000164D  BB0300            mov bx,0x3
00001650  99                cwd
00001651  F7FB              idiv bx
00001653  BB1600            mov bx,0x16
00001656  8BC2              mov ax,dx
00001658  F7EB              imul bx
0000165A  055800            add ax,0x58
0000165D  50                push ax
0000165E  33C0              xor ax,ax
00001660  50                push ax
00001661  9A3000960A        call 0xa96:0x30
00001666  83C40C            add sp,byte +0xc
00001669  EB1E              jmp short 0x1689
0000166B  56                push si
0000166C  57                push di
0000166D  B81600            mov ax,0x16
00001670  50                push ax
00001671  B82000            mov ax,0x20
00001674  50                push ax
00001675  8B46F4            mov ax,[bp-0xc]
00001678  BA1600            mov dx,0x16
0000167B  F7EA              imul dx
0000167D  50                push ax
0000167E  33C0              xor ax,ax
00001680  50                push ax
00001681  9A3000960A        call 0xa96:0x30
00001686  83C40C            add sp,byte +0xc
00001689  E9C201            jmp 0x184e
0000168C  837EF000          cmp word [bp-0x10],byte +0x0
00001690  7507              jnz 0x1699
00001692  C746FE0000        mov word [bp-0x2],0x0
00001697  EB05              jmp short 0x169e
00001699  C746FE2000        mov word [bp-0x2],0x20
0000169E  33C0              xor ax,ax
000016A0  8946F6            mov [bp-0xa],ax
000016A3  8946F8            mov [bp-0x8],ax
000016A6  8946FA            mov [bp-0x6],ax
000016A9  8946FC            mov [bp-0x4],ax
000016AC  837E0800          cmp word [bp+0x8],byte +0x0
000016B0  7E1C              jng 0x16ce
000016B2  8B4606            mov ax,[bp+0x6]
000016B5  BA0600            mov dx,0x6
000016B8  F7EA              imul dx
000016BA  034608            add ax,[bp+0x8]
000016BD  8BD8              mov bx,ax
000016BF  8A87D945          mov al,[bx+0x45d9]
000016C3  98                cbw
000016C4  3B46F0            cmp ax,[bp-0x10]
000016C7  7505              jnz 0x16ce
000016C9  C746FC0200        mov word [bp-0x4],0x2
000016CE  837E0600          cmp word [bp+0x6],byte +0x0
000016D2  7E1C              jng 0x16f0
000016D4  8B4606            mov ax,[bp+0x6]
000016D7  BA0600            mov dx,0x6
000016DA  F7EA              imul dx
000016DC  034608            add ax,[bp+0x8]
000016DF  8BD8              mov bx,ax
000016E1  8A87D445          mov al,[bx+0x45d4]
000016E5  98                cbw
000016E6  3B46F0            cmp ax,[bp-0x10]
000016E9  7505              jnz 0x16f0
000016EB  C746F80100        mov word [bp-0x8],0x1
000016F0  A1FC08            mov ax,[0x8fc]
000016F3  48                dec ax
000016F4  3B4606            cmp ax,[bp+0x6]
000016F7  7E1C              jng 0x1715
000016F9  8B4606            mov ax,[bp+0x6]
000016FC  BA0600            mov dx,0x6
000016FF  F7EA              imul dx
00001701  034608            add ax,[bp+0x8]
00001704  8BD8              mov bx,ax
00001706  8A87E045          mov al,[bx+0x45e0]
0000170A  98                cbw
0000170B  3B46F0            cmp ax,[bp-0x10]
0000170E  7505              jnz 0x1715
00001710  C746F60100        mov word [bp-0xa],0x1
00001715  A1FE08            mov ax,[0x8fe]
00001718  48                dec ax
00001719  3B4608            cmp ax,[bp+0x8]
0000171C  7E1C              jng 0x173a
0000171E  8B4606            mov ax,[bp+0x6]
00001721  BA0600            mov dx,0x6
00001724  F7EA              imul dx
00001726  034608            add ax,[bp+0x8]
00001729  8BD8              mov bx,ax
0000172B  8A87DB45          mov al,[bx+0x45db]
0000172F  98                cbw
00001730  3B46F0            cmp ax,[bp-0x10]
00001733  7505              jnz 0x173a
00001735  C746FA0200        mov word [bp-0x6],0x2
0000173A  56                push si
0000173B  57                push di
0000173C  B80B00            mov ax,0xb
0000173F  50                push ax
00001740  B81000            mov ax,0x10
00001743  50                push ax
00001744  8B5EFC            mov bx,[bp-0x4]
00001747  035EF8            add bx,[bp-0x8]
0000174A  8A87D400          mov al,[bx+0xd4]
0000174E  B400              mov ah,0x0
00001750  50                push ax
00001751  8B5EFC            mov bx,[bp-0x4]
00001754  035EF8            add bx,[bp-0x8]
00001757  8A87D000          mov al,[bx+0xd0]
0000175B  B400              mov ah,0x0
0000175D  0346FE            add ax,[bp-0x2]
00001760  50                push ax
00001761  9A3000960A        call 0xa96:0x30
00001766  83C40C            add sp,byte +0xc
00001769  56                push si
0000176A  8BC7              mov ax,di
0000176C  051000            add ax,0x10
0000176F  50                push ax
00001770  B80B00            mov ax,0xb
00001773  50                push ax
00001774  B81000            mov ax,0x10
00001777  50                push ax
00001778  8B5EFC            mov bx,[bp-0x4]
0000177B  035EF6            add bx,[bp-0xa]
0000177E  8A87D400          mov al,[bx+0xd4]
00001782  B400              mov ah,0x0
00001784  50                push ax
00001785  8B5EFC            mov bx,[bp-0x4]
00001788  035EF6            add bx,[bp-0xa]
0000178B  8A87D000          mov al,[bx+0xd0]
0000178F  B400              mov ah,0x0
00001791  0346FE            add ax,[bp-0x2]
00001794  051000            add ax,0x10
00001797  50                push ax
00001798  9A3000960A        call 0xa96:0x30
0000179D  83C40C            add sp,byte +0xc
000017A0  8BC6              mov ax,si
000017A2  050B00            add ax,0xb
000017A5  50                push ax
000017A6  57                push di
000017A7  B80B00            mov ax,0xb
000017AA  50                push ax
000017AB  B81000            mov ax,0x10
000017AE  50                push ax
000017AF  8B5EFA            mov bx,[bp-0x6]
000017B2  035EF8            add bx,[bp-0x8]
000017B5  8A87D400          mov al,[bx+0xd4]
000017B9  B400              mov ah,0x0
000017BB  050B00            add ax,0xb
000017BE  50                push ax
000017BF  8B5EFA            mov bx,[bp-0x6]
000017C2  035EF8            add bx,[bp-0x8]
000017C5  8A87D000          mov al,[bx+0xd0]
000017C9  B400              mov ah,0x0
000017CB  0346FE            add ax,[bp-0x2]
000017CE  50                push ax
000017CF  9A3000960A        call 0xa96:0x30
000017D4  83C40C            add sp,byte +0xc
000017D7  8BC6              mov ax,si
000017D9  050B00            add ax,0xb
000017DC  50                push ax
000017DD  8BC7              mov ax,di
000017DF  051000            add ax,0x10
000017E2  50                push ax
000017E3  B80B00            mov ax,0xb
000017E6  50                push ax
000017E7  B81000            mov ax,0x10
000017EA  50                push ax
000017EB  8B5EFA            mov bx,[bp-0x6]
000017EE  035EF6            add bx,[bp-0xa]
000017F1  8A87D400          mov al,[bx+0xd4]
000017F5  B400              mov ah,0x0
000017F7  050B00            add ax,0xb
000017FA  50                push ax
000017FB  8B5EFA            mov bx,[bp-0x6]
000017FE  035EF6            add bx,[bp-0xa]
00001801  8A87D000          mov al,[bx+0xd0]
00001805  B400              mov ah,0x0
00001807  0346FE            add ax,[bp-0x2]
0000180A  051000            add ax,0x10
0000180D  50                push ax
0000180E  9A3000960A        call 0xa96:0x30
00001813  83C40C            add sp,byte +0xc
00001816  837EF4FE          cmp word [bp-0xc],byte -0x2
0000181A  7432              jz 0x184e
0000181C  56                push si
0000181D  57                push di
0000181E  B81600            mov ax,0x16
00001821  50                push ax
00001822  B82000            mov ax,0x20
00001825  50                push ax
00001826  8B46F4            mov ax,[bp-0xc]
00001829  BA1600            mov dx,0x16
0000182C  F7EA              imul dx
0000182E  50                push ax
0000182F  8B46FE            mov ax,[bp-0x2]
00001832  059000            add ax,0x90
00001835  50                push ax
00001836  8B46F4            mov ax,[bp-0xc]
00001839  BA1600            mov dx,0x16
0000183C  F7EA              imul dx
0000183E  50                push ax
0000183F  8B46FE            mov ax,[bp-0x2]
00001842  054000            add ax,0x40
00001845  50                push ax
00001846  9A3B01960A        call 0xa96:0x13b
0000184B  83C410            add sp,byte +0x10
0000184E  5F                pop di
0000184F  5E                pop si
00001850  8BE5              mov sp,bp
00001852  5D                pop bp
00001853  CB                retf

; ---- 0609:1854 ----
00001854  55                push bp
00001855  8BEC              mov bp,sp
00001857  A0AA37            mov al,[0x37aa]
0000185A  98                cbw
0000185B  8BD8              mov bx,ax
0000185D  8A87423D          mov al,[bx+0x3d42]
00001861  B400              mov ah,0x0
00001863  50                push ax
00001864  FF7608            push word [bp+0x8]
00001867  FF7606            push word [bp+0x6]
0000186A  9A79180906        call 0x609:0x1879
0000186F  83C406            add sp,byte +0x6
00001872  9A611A0906        call 0x609:0x1a61
00001877  5D                pop bp
00001878  CB                retf

; ---- 0609:1879 ----
00001879  55                push bp
0000187A  8BEC              mov bp,sp
0000187C  FF7608            push word [bp+0x8]
0000187F  FF7606            push word [bp+0x6]
00001882  B82C00            mov ax,0x2c
00001885  50                push ax
00001886  B83000            mov ax,0x30
00001889  50                push ax
0000188A  A0AA37            mov al,[0x37aa]
0000188D  98                cbw
0000188E  BA2C00            mov dx,0x2c
00001891  F7EA              imul dx
00001893  50                push ax
00001894  B8C000            mov ax,0xc0
00001897  50                push ax
00001898  9A3000960A        call 0xa96:0x30
0000189D  83C40C            add sp,byte +0xc
000018A0  837E0A00          cmp word [bp+0xa],byte +0x0
000018A4  7E3D              jng 0x18e3
000018A6  A0AA37            mov al,[0x37aa]
000018A9  98                cbw
000018AA  D1E0              shl ax,1
000018AC  8B5608            mov dx,[bp+0x8]
000018AF  8BD8              mov bx,ax
000018B1  0397D800          add dx,[bx+0xd8]
000018B5  52                push dx
000018B6  8B4606            mov ax,[bp+0x6]
000018B9  051000            add ax,0x10
000018BC  50                push ax
000018BD  B80B00            mov ax,0xb
000018C0  50                push ax
000018C1  B81000            mov ax,0x10
000018C4  50                push ax
000018C5  A0AA37            mov al,[0x37aa]
000018C8  98                cbw
000018C9  BA0B00            mov dx,0xb
000018CC  F7EA              imul dx
000018CE  05C600            add ax,0xc6
000018D1  50                push ax
000018D2  8B460A            mov ax,[bp+0xa]
000018D5  48                dec ax
000018D6  B104              mov cl,0x4
000018D8  D3E0              shl ax,cl
000018DA  50                push ax
000018DB  9A3000960A        call 0xa96:0x30
000018E0  83C40C            add sp,byte +0xc
000018E3  5D                pop bp
000018E4  CB                retf

; ---- 0609:18E5 ----
000018E5  55                push bp
000018E6  8BEC              mov bp,sp
000018E8  B80600            mov ax,0x6
000018EB  50                push ax
000018EC  A0AA37            mov al,[0x37aa]
000018EF  98                cbw
000018F0  B106              mov cl,0x6
000018F2  D3E0              shl ax,cl
000018F4  052000            add ax,0x20
000018F7  50                push ax
000018F8  B82C00            mov ax,0x2c
000018FB  50                push ax
000018FC  B83000            mov ax,0x30
000018FF  50                push ax
00001900  B8B000            mov ax,0xb0
00001903  50                push ax
00001904  B8C000            mov ax,0xc0
00001907  50                push ax
00001908  9A3000960A        call 0xa96:0x30
0000190D  83C40C            add sp,byte +0xc
00001910  5D                pop bp
00001911  CB                retf

; ---- 0609:1912 ----
00001912  55                push bp
00001913  8BEC              mov bp,sp
00001915  FF760A            push word [bp+0xa]
00001918  FF7608            push word [bp+0x8]
0000191B  B80700            mov ax,0x7
0000191E  50                push ax
0000191F  B81000            mov ax,0x10
00001922  50                push ax
00001923  B8DC00            mov ax,0xdc
00001926  50                push ax
00001927  8B4606            mov ax,[bp+0x6]
0000192A  B104              mov cl,0x4
0000192C  D3E0              shl ax,cl
0000192E  05C000            add ax,0xc0
00001931  50                push ax
00001932  9A3000960A        call 0xa96:0x30
00001937  83C40C            add sp,byte +0xc
0000193A  5D                pop bp
0000193B  CB                retf

; ---- 0609:193C ----
0000193C  55                push bp
0000193D  8BEC              mov bp,sp
0000193F  B80A00            mov ax,0xa
00001942  50                push ax
00001943  A0AA37            mov al,[0x37aa]
00001946  98                cbw
00001947  B106              mov cl,0x6
00001949  D3E0              shl ax,cl
0000194B  053100            add ax,0x31
0000194E  50                push ax
0000194F  B80E00            mov ax,0xe
00001952  50                push ax
00001953  B81000            mov ax,0x10
00001956  50                push ax
00001957  B8EE00            mov ax,0xee
0000195A  50                push ax
0000195B  A0AA37            mov al,[0x37aa]
0000195E  98                cbw
0000195F  8BD8              mov bx,ax
00001961  8A87942E          mov al,[bx+0x2e94]
00001965  98                cbw
00001966  B104              mov cl,0x4
00001968  D3E0              shl ax,cl
0000196A  054000            add ax,0x40
0000196D  50                push ax
0000196E  9A3000960A        call 0xa96:0x30
00001973  83C40C            add sp,byte +0xc
00001976  5D                pop bp
00001977  CB                retf

; ---- 0609:1978 ----
00001978  55                push bp
00001979  8BEC              mov bp,sp
0000197B  56                push si
0000197C  33F6              xor si,si
0000197E  EB6F              jmp short 0x19ef
00001980  83FE04            cmp si,byte +0x4
00001983  7E37              jng 0x19bc
00001985  B82200            mov ax,0x22
00001988  50                push ax
00001989  A0AA37            mov al,[0x37aa]
0000198C  98                cbw
0000198D  B106              mov cl,0x6
0000198F  D3E0              shl ax,cl
00001991  8BD6              mov dx,si
00001993  83C2FB            add dx,byte -0x5
00001996  D1E2              shl dx,1
00001998  D1E2              shl dx,1
0000199A  D1E2              shl dx,1
0000199C  03C2              add ax,dx
0000199E  052600            add ax,0x26
000019A1  50                push ax
000019A2  B80900            mov ax,0x9
000019A5  50                push ax
000019A6  B80700            mov ax,0x7
000019A9  50                push ax
000019AA  B8C600            mov ax,0xc6
000019AD  50                push ax
000019AE  B8F000            mov ax,0xf0
000019B1  50                push ax
000019B2  9A3000960A        call 0xa96:0x30
000019B7  83C40C            add sp,byte +0xc
000019BA  EB32              jmp short 0x19ee
000019BC  B81800            mov ax,0x18
000019BF  50                push ax
000019C0  A0AA37            mov al,[0x37aa]
000019C3  98                cbw
000019C4  B106              mov cl,0x6
000019C6  D3E0              shl ax,cl
000019C8  8BD6              mov dx,si
000019CA  D1E2              shl dx,1
000019CC  D1E2              shl dx,1
000019CE  D1E2              shl dx,1
000019D0  03C2              add ax,dx
000019D2  052600            add ax,0x26
000019D5  50                push ax
000019D6  B80900            mov ax,0x9
000019D9  50                push ax
000019DA  B80700            mov ax,0x7
000019DD  50                push ax
000019DE  B8C600            mov ax,0xc6
000019E1  50                push ax
000019E2  B8F000            mov ax,0xf0
000019E5  50                push ax
000019E6  9A3000960A        call 0xa96:0x30
000019EB  83C40C            add sp,byte +0xc
000019EE  46                inc si
000019EF  A0AA37            mov al,[0x37aa]
000019F2  98                cbw
000019F3  D1E0              shl ax,1
000019F5  8BD8              mov bx,ax
000019F7  8B87AC4B          mov ax,[bx+0x4bac]
000019FB  48                dec ax
000019FC  3BC6              cmp ax,si
000019FE  7D80              jnl 0x1980
00001A00  5E                pop si
00001A01  5D                pop bp
00001A02  CB                retf

; ---- 0609:1A03 ----
00001A03  55                push bp
00001A04  8BEC              mov bp,sp
00001A06  56                push si
00001A07  A0AA37            mov al,[0x37aa]
00001A0A  98                cbw
00001A0B  8BD8              mov bx,ax
00001A0D  8A874E30          mov al,[bx+0x304e]
00001A11  98                cbw
00001A12  BA0600            mov dx,0x6
00001A15  F7EA              imul dx
00001A17  50                push ax
00001A18  A0AA37            mov al,[0x37aa]
00001A1B  98                cbw
00001A1C  8BD8              mov bx,ax
00001A1E  8A875A30          mov al,[bx+0x305a]
00001A22  98                cbw
00001A23  5B                pop bx
00001A24  03D8              add bx,ax
00001A26  8A87DA45          mov al,[bx+0x45da]
00001A2A  98                cbw
00001A2B  8BF0              mov si,ax
00001A2D  B80600            mov ax,0x6
00001A30  50                push ax
00001A31  A0AA37            mov al,[0x37aa]
00001A34  98                cbw
00001A35  B106              mov cl,0x6
00001A37  D3E0              shl ax,cl
00001A39  052000            add ax,0x20
00001A3C  50                push ax
00001A3D  B82C00            mov ax,0x2c
00001A40  50                push ax
00001A41  B83000            mov ax,0x30
00001A44  50                push ax
00001A45  8BC6              mov ax,si
00001A47  BA2C00            mov dx,0x2c
00001A4A  F7EA              imul dx
00001A4C  50                push ax
00001A4D  B8F000            mov ax,0xf0
00001A50  50                push ax
00001A51  9A3000960A        call 0xa96:0x30
00001A56  83C40C            add sp,byte +0xc
00001A59  9A611A0906        call 0x609:0x1a61
00001A5E  5E                pop si
00001A5F  5D                pop bp
00001A60  CB                retf

; ---- 0609:1A61 ----
00001A61  55                push bp
00001A62  8BEC              mov bp,sp
00001A64  A0AA37            mov al,[0x37aa]
00001A67  98                cbw
00001A68  8BD8              mov bx,ax
00001A6A  80BFFA4301        cmp byte [bx+0x43fa],0x1
00001A6F  7524              jnz 0x1a95
00001A71  B82A00            mov ax,0x2a
00001A74  50                push ax
00001A75  A0AA37            mov al,[0x37aa]
00001A78  98                cbw
00001A79  B106              mov cl,0x6
00001A7B  D3E0              shl ax,cl
00001A7D  052000            add ax,0x20
00001A80  50                push ax
00001A81  B80800            mov ax,0x8
00001A84  50                push ax
00001A85  B8C600            mov ax,0xc6
00001A88  50                push ax
00001A89  B8F800            mov ax,0xf8
00001A8C  50                push ax
00001A8D  9A1103960A        call 0xa96:0x311
00001A92  83C40A            add sp,byte +0xa
00001A95  5D                pop bp
00001A96  CB                retf

; ---- 0609:1A97 ----
00001A97  55                push bp
00001A98  8BEC              mov bp,sp
00001A9A  B82A00            mov ax,0x2a
00001A9D  50                push ax
00001A9E  A0AA37            mov al,[0x37aa]
00001AA1  98                cbw
00001AA2  B106              mov cl,0x6
00001AA4  D3E0              shl ax,cl
00001AA6  052000            add ax,0x20
00001AA9  50                push ax
00001AAA  B80800            mov ax,0x8
00001AAD  50                push ax
00001AAE  B82400            mov ax,0x24
00001AB1  50                push ax
00001AB2  B8C000            mov ax,0xc0
00001AB5  50                push ax
00001AB6  9A1103960A        call 0xa96:0x311
00001ABB  83C40A            add sp,byte +0xa
00001ABE  5D                pop bp
00001ABF  CB                retf

; ---- 0609:1AC0 ----
00001AC0  55                push bp
00001AC1  8BEC              mov bp,sp
00001AC3  56                push si
00001AC4  803EBA4501        cmp byte [0x45ba],0x1
00001AC9  750E              jnz 0x1ad9
00001ACB  A0AA37            mov al,[0x37aa]
00001ACE  98                cbw
00001ACF  D1E0              shl ax,1
00001AD1  8BD8              mov bx,ax
00001AD3  8B872045          mov ax,[bx+0x4520]
00001AD7  EB03              jmp short 0x1adc
00001AD9  B82800            mov ax,0x28
00001ADC  8BF0              mov si,ax
00001ADE  B80600            mov ax,0x6
00001AE1  50                push ax
00001AE2  A0AA37            mov al,[0x37aa]
00001AE5  98                cbw
00001AE6  B106              mov cl,0x6
00001AE8  D3E0              shl ax,cl
00001AEA  051000            add ax,0x10
00001AED  50                push ax
00001AEE  B82C00            mov ax,0x2c
00001AF1  50                push ax
00001AF2  B81000            mov ax,0x10
00001AF5  50                push ax
00001AF6  B8D000            mov ax,0xd0
00001AF9  50                push ax
00001AFA  B81001            mov ax,0x110
00001AFD  50                push ax
00001AFE  9A3000960A        call 0xa96:0x30
00001B03  83C40C            add sp,byte +0xc
00001B06  B82000            mov ax,0x20
00001B09  50                push ax
00001B0A  B81000            mov ax,0x10
00001B0D  50                push ax
00001B0E  B80700            mov ax,0x7
00001B11  50                push ax
00001B12  A0AA37            mov al,[0x37aa]
00001B15  98                cbw
00001B16  B106              mov cl,0x6
00001B18  D3E0              shl ax,cl
00001B1A  051000            add ax,0x10
00001B1D  50                push ax
00001B1E  B80800            mov ax,0x8
00001B21  50                push ax
00001B22  B81000            mov ax,0x10
00001B25  50                push ax
00001B26  B8F100            mov ax,0xf1
00001B29  50                push ax
00001B2A  B81001            mov ax,0x110
00001B2D  50                push ax
00001B2E  9A6702960A        call 0xa96:0x267
00001B33  83C410            add sp,byte +0x10
00001B36  56                push si
00001B37  B81000            mov ax,0x10
00001B3A  50                push ax
00001B3B  B82F00            mov ax,0x2f
00001B3E  2BC6              sub ax,si
00001B40  50                push ax
00001B41  A0AA37            mov al,[0x37aa]
00001B44  98                cbw
00001B45  B106              mov cl,0x6
00001B47  D3E0              shl ax,cl
00001B49  051000            add ax,0x10
00001B4C  50                push ax
00001B4D  B80800            mov ax,0x8
00001B50  50                push ax
00001B51  B81000            mov ax,0x10
00001B54  50                push ax
00001B55  A0AA37            mov al,[0x37aa]
00001B58  98                cbw
00001B59  D1E0              shl ax,1
00001B5B  D1E0              shl ax,1
00001B5D  D1E0              shl ax,1
00001B5F  05D100            add ax,0xd1
00001B62  50                push ax
00001B63  B81001            mov ax,0x110
00001B66  50                push ax
00001B67  9A6702960A        call 0xa96:0x267
00001B6C  83C410            add sp,byte +0x10
00001B6F  5E                pop si
00001B70  5D                pop bp
00001B71  CB                retf

; ---- 0609:1B72 ----
00001B72  55                push bp
00001B73  8BEC              mov bp,sp
00001B75  56                push si
00001B76  57                push di
00001B77  A0AA37            mov al,[0x37aa]
00001B7A  98                cbw
00001B7B  D1E0              shl ax,1
00001B7D  8BD8              mov bx,ax
00001B7F  8BB72045          mov si,[bx+0x4520]
00001B83  807E0601          cmp byte [bp+0x6],0x1
00001B87  7512              jnz 0x1b9b
00001B89  46                inc si
00001B8A  A0AA37            mov al,[0x37aa]
00001B8D  98                cbw
00001B8E  D1E0              shl ax,1
00001B90  D1E0              shl ax,1
00001B92  D1E0              shl ax,1
00001B94  05D100            add ax,0xd1
00001B97  8BF8              mov di,ax
00001B99  EB03              jmp short 0x1b9e
00001B9B  BFF100            mov di,0xf1
00001B9E  B82F00            mov ax,0x2f
00001BA1  2BC6              sub ax,si
00001BA3  50                push ax
00001BA4  A0AA37            mov al,[0x37aa]
00001BA7  98                cbw
00001BA8  B106              mov cl,0x6
00001BAA  D3E0              shl ax,cl
00001BAC  051000            add ax,0x10
00001BAF  50                push ax
00001BB0  B80100            mov ax,0x1
00001BB3  50                push ax
00001BB4  B81000            mov ax,0x10
00001BB7  50                push ax
00001BB8  8BC6              mov ax,si
00001BBA  BB0800            mov bx,0x8
00001BBD  99                cwd
00001BBE  F7FB              idiv bx
00001BC0  8BC7              mov ax,di
00001BC2  03C2              add ax,dx
00001BC4  50                push ax
00001BC5  B81001            mov ax,0x110
00001BC8  50                push ax
00001BC9  9A3000960A        call 0xa96:0x30
00001BCE  83C40C            add sp,byte +0xc
00001BD1  A0AA37            mov al,[0x37aa]
00001BD4  98                cbw
00001BD5  D1E0              shl ax,1
00001BD7  50                push ax
00001BD8  8A4606            mov al,[bp+0x6]
00001BDB  98                cbw
00001BDC  5B                pop bx
00001BDD  01872045          add [bx+0x4520],ax
00001BE1  5F                pop di
00001BE2  5E                pop si
00001BE3  5D                pop bp
00001BE4  CB                retf

; ---- 0609:1BE5 ----
00001BE5  55                push bp
00001BE6  8BEC              mov bp,sp
00001BE8  9A1B00960A        call 0xa96:0x1b
00001BED  9AD91E0906        call 0x609:0x1ed9
00001BF2  B83600            mov ax,0x36
00001BF5  50                push ax
00001BF6  B84001            mov ax,0x140
00001BF9  50                push ax
00001BFA  33C0              xor ax,ax
00001BFC  50                push ax
00001BFD  33C0              xor ax,ax
00001BFF  50                push ax
00001C00  0E                push cs
00001C01  E852F6            call 0x1256
00001C04  83C408            add sp,byte +0x8
00001C07  A1FE08            mov ax,[0x8fe]
00001C0A  BA1600            mov dx,0x16
00001C0D  F7EA              imul dx
00001C0F  50                push ax
00001C10  A1FC08            mov ax,[0x8fc]
00001C13  B105              mov cl,0x5
00001C15  D3E0              shl ax,cl
00001C17  50                push ax
00001C18  FF362845          push word [0x4528]
00001C1C  FF361E45          push word [0x451e]
00001C20  0E                push cs
00001C21  E8A1F7            call 0x13c5
00001C24  83C408            add sp,byte +0x8
00001C27  9AA806960A        call 0xa96:0x6a8
00001C2C  C606002F00        mov byte [0x2f00],0x0
00001C31  EB37              jmp short 0x1c6a
00001C33  C606B02E00        mov byte [0x2eb0],0x0
00001C38  EB1F              jmp short 0x1c59
00001C3A  9A1B00960A        call 0xa96:0x1b
00001C3F  A0002F            mov al,[0x2f00]
00001C42  98                cbw
00001C43  50                push ax
00001C44  A0B02E            mov al,[0x2eb0]
00001C47  98                cbw
00001C48  50                push ax
00001C49  9A831C0906        call 0x609:0x1c83
00001C4E  59                pop cx
00001C4F  59                pop cx
00001C50  9AA806960A        call 0xa96:0x6a8
00001C55  FE06B02E          inc byte [0x2eb0]
00001C59  A0B02E            mov al,[0x2eb0]
00001C5C  98                cbw
00001C5D  8B16FC08          mov dx,[0x8fc]
00001C61  4A                dec dx
00001C62  3BC2              cmp ax,dx
00001C64  7ED4              jng 0x1c3a
00001C66  FE06002F          inc byte [0x2f00]
00001C6A  A0002F            mov al,[0x2f00]
00001C6D  98                cbw
00001C6E  8B16FE08          mov dx,[0x8fe]
00001C72  4A                dec dx
00001C73  3BC2              cmp ax,dx
00001C75  7EBC              jng 0x1c33
00001C77  FF362E08          push word [0x82e]
00001C7B  9A3F0C960A        call 0xa96:0xc3f
00001C80  59                pop cx
00001C81  5D                pop bp
00001C82  CB                retf

; ---- 0609:1C83 ----
00001C83  55                push bp
00001C84  8BEC              mov bp,sp
00001C86  56                push si
00001C87  57                push di
00001C88  8B7E06            mov di,[bp+0x6]
00001C8B  8B7608            mov si,[bp+0x8]
00001C8E  56                push si
00001C8F  57                push di
00001C90  0E                push cs
00001C91  E813F4            call 0x10a7
00001C94  59                pop cx
00001C95  59                pop cx
00001C96  0AC0              or al,al
00001C98  7502              jnz 0x1c9c
00001C9A  EB11              jmp short 0x1cad
00001C9C  56                push si
00001C9D  57                push di
00001C9E  0E                push cs
00001C9F  E89EF8            call 0x1540
00001CA2  59                pop cx
00001CA3  59                pop cx
00001CA4  56                push si
00001CA5  57                push di
00001CA6  9AF41C0906        call 0x609:0x1cf4
00001CAB  59                pop cx
00001CAC  59                pop cx
00001CAD  5F                pop di
00001CAE  5E                pop si
00001CAF  5D                pop bp
00001CB0  CB                retf

; ---- 0609:1CB1 ----
00001CB1  55                push bp
00001CB2  8BEC              mov bp,sp
00001CB4  56                push si
00001CB5  57                push di
00001CB6  8B7E06            mov di,[bp+0x6]
00001CB9  8B7608            mov si,[bp+0x8]
00001CBC  56                push si
00001CBD  57                push di
00001CBE  0E                push cs
00001CBF  E8C1FF            call 0x1c83
00001CC2  59                pop cx
00001CC3  59                pop cx
00001CC4  56                push si
00001CC5  8BC7              mov ax,di
00001CC7  48                dec ax
00001CC8  50                push ax
00001CC9  0E                push cs
00001CCA  E8B6FF            call 0x1c83
00001CCD  59                pop cx
00001CCE  59                pop cx
00001CCF  56                push si
00001CD0  8BC7              mov ax,di
00001CD2  40                inc ax
00001CD3  50                push ax
00001CD4  0E                push cs
00001CD5  E8ABFF            call 0x1c83
00001CD8  59                pop cx
00001CD9  59                pop cx
00001CDA  8BC6              mov ax,si
00001CDC  48                dec ax
00001CDD  50                push ax
00001CDE  57                push di
00001CDF  0E                push cs
00001CE0  E8A0FF            call 0x1c83
00001CE3  59                pop cx
00001CE4  59                pop cx
00001CE5  8BC6              mov ax,si
00001CE7  40                inc ax
00001CE8  50                push ax
00001CE9  57                push di
00001CEA  0E                push cs
00001CEB  E895FF            call 0x1c83
00001CEE  59                pop cx
00001CEF  59                pop cx
00001CF0  5F                pop di
00001CF1  5E                pop si
00001CF2  5D                pop bp
00001CF3  CB                retf

; ---- 0609:1CF4 ----
00001CF4  55                push bp
00001CF5  8BEC              mov bp,sp
00001CF7  83EC04            sub sp,byte +0x4
00001CFA  56                push si
00001CFB  57                push di
00001CFC  8B4606            mov ax,[bp+0x6]
00001CFF  BA0600            mov dx,0x6
00001D02  F7EA              imul dx
00001D04  034608            add ax,[bp+0x8]
00001D07  8BD8              mov bx,ax
00001D09  8A874244          mov al,[bx+0x4442]
00001D0D  98                cbw
00001D0E  8946FC            mov [bp-0x4],ax
00001D11  837EFCFF          cmp word [bp-0x4],byte -0x1
00001D15  7503              jnz 0x1d1a
00001D17  E94B01            jmp 0x1e65
00001D1A  8B4606            mov ax,[bp+0x6]
00001D1D  B105              mov cl,0x5
00001D1F  D3E0              shl ax,cl
00001D21  8B161E45          mov dx,[0x451e]
00001D25  03D0              add dx,ax
00001D27  8BFA              mov di,dx
00001D29  8B4608            mov ax,[bp+0x8]
00001D2C  BA1600            mov dx,0x16
00001D2F  F7EA              imul dx
00001D31  8B162845          mov dx,[0x4528]
00001D35  03D0              add dx,ax
00001D37  8BF2              mov si,dx
00001D39  8B5EFC            mov bx,[bp-0x4]
00001D3C  D1E3              shl bx,1
00001D3E  8B87C800          mov ax,[bx+0xc8]
00001D42  8946FE            mov [bp-0x2],ax
00001D45  B85555            mov ax,0x5555
00001D48  50                push ax
00001D49  9A8606960A        call 0xa96:0x686
00001D4E  59                pop cx
00001D4F  FF76FC            push word [bp-0x4]
00001D52  FF7608            push word [bp+0x8]
00001D55  8B4606            mov ax,[bp+0x6]
00001D58  48                dec ax
00001D59  50                push ax
00001D5A  9A6B1E0906        call 0x609:0x1e6b
00001D5F  83C406            add sp,byte +0x6
00001D62  3C01              cmp al,0x1
00001D64  7514              jnz 0x1d7a
00001D66  FF76FE            push word [bp-0x2]
00001D69  8BC6              mov ax,si
00001D6B  051500            add ax,0x15
00001D6E  50                push ax
00001D6F  57                push di
00001D70  56                push si
00001D71  57                push di
00001D72  9A3304960A        call 0xa96:0x433
00001D77  83C40A            add sp,byte +0xa
00001D7A  FF76FC            push word [bp-0x4]
00001D7D  FF7608            push word [bp+0x8]
00001D80  8B4606            mov ax,[bp+0x6]
00001D83  40                inc ax
00001D84  50                push ax
00001D85  9A6B1E0906        call 0x609:0x1e6b
00001D8A  83C406            add sp,byte +0x6
00001D8D  3C01              cmp al,0x1
00001D8F  751E              jnz 0x1daf
00001D91  FF76FE            push word [bp-0x2]
00001D94  8BC6              mov ax,si
00001D96  051500            add ax,0x15
00001D99  50                push ax
00001D9A  8BC7              mov ax,di
00001D9C  051F00            add ax,0x1f
00001D9F  50                push ax
00001DA0  56                push si
00001DA1  8BC7              mov ax,di
00001DA3  051F00            add ax,0x1f
00001DA6  50                push ax
00001DA7  9A3304960A        call 0xa96:0x433
00001DAC  83C40A            add sp,byte +0xa
00001DAF  FF76FC            push word [bp-0x4]
00001DB2  8B4608            mov ax,[bp+0x8]
00001DB5  48                dec ax
00001DB6  50                push ax
00001DB7  FF7606            push word [bp+0x6]
00001DBA  9A6B1E0906        call 0x609:0x1e6b
00001DBF  83C406            add sp,byte +0x6
00001DC2  3C01              cmp al,0x1
00001DC4  7514              jnz 0x1dda
00001DC6  FF76FE            push word [bp-0x2]
00001DC9  56                push si
00001DCA  8BC7              mov ax,di
00001DCC  051F00            add ax,0x1f
00001DCF  50                push ax
00001DD0  56                push si
00001DD1  57                push di
00001DD2  9A3304960A        call 0xa96:0x433
00001DD7  83C40A            add sp,byte +0xa
00001DDA  FF76FC            push word [bp-0x4]
00001DDD  8B4608            mov ax,[bp+0x8]
00001DE0  40                inc ax
00001DE1  50                push ax
00001DE2  FF7606            push word [bp+0x6]
00001DE5  9A6B1E0906        call 0x609:0x1e6b
00001DEA  83C406            add sp,byte +0x6
00001DED  3C01              cmp al,0x1
00001DEF  751E              jnz 0x1e0f
00001DF1  FF76FE            push word [bp-0x2]
00001DF4  8BC6              mov ax,si
00001DF6  051500            add ax,0x15
00001DF9  50                push ax
00001DFA  8BC7              mov ax,di
00001DFC  051F00            add ax,0x1f
00001DFF  50                push ax
00001E00  8BC6              mov ax,si
00001E02  051500            add ax,0x15
00001E05  50                push ax
00001E06  57                push di
00001E07  9A3304960A        call 0xa96:0x433
00001E0C  83C40A            add sp,byte +0xa
00001E0F  B8FFFF            mov ax,0xffff
00001E12  50                push ax
00001E13  9A8606960A        call 0xa96:0x686
00001E18  59                pop cx
00001E19  8B5EFC            mov bx,[bp-0x4]
00001E1C  8A87AF3C          mov al,[bx+0x3caf]
00001E20  98                cbw
00001E21  3B4606            cmp ax,[bp+0x6]
00001E24  753F              jnz 0x1e65
00001E26  8B5EFC            mov bx,[bp-0x4]
00001E29  8A87B33C          mov al,[bx+0x3cb3]
00001E2D  98                cbw
00001E2E  3B4608            cmp ax,[bp+0x8]
00001E31  7532              jnz 0x1e65
00001E33  FF76FE            push word [bp-0x2]
00001E36  8BC6              mov ax,si
00001E38  051500            add ax,0x15
00001E3B  50                push ax
00001E3C  8BC7              mov ax,di
00001E3E  051F00            add ax,0x1f
00001E41  50                push ax
00001E42  56                push si
00001E43  57                push di
00001E44  9A3304960A        call 0xa96:0x433
00001E49  83C40A            add sp,byte +0xa
00001E4C  FF76FE            push word [bp-0x2]
00001E4F  56                push si
00001E50  8BC7              mov ax,di
00001E52  051F00            add ax,0x1f
00001E55  50                push ax
00001E56  8BC6              mov ax,si
00001E58  051500            add ax,0x15
00001E5B  50                push ax
00001E5C  57                push di
00001E5D  9A3304960A        call 0xa96:0x433
00001E62  83C40A            add sp,byte +0xa
00001E65  5F                pop di
00001E66  5E                pop si
00001E67  8BE5              mov sp,bp
00001E69  5D                pop bp
00001E6A  CB                retf

; ---- 0609:1E6B ----
00001E6B  55                push bp
00001E6C  8BEC              mov bp,sp
00001E6E  FF7608            push word [bp+0x8]
00001E71  FF7606            push word [bp+0x6]
00001E74  0E                push cs
00001E75  E82FF2            call 0x10a7
00001E78  59                pop cx
00001E79  59                pop cx
00001E7A  0AC0              or al,al
00001E7C  7504              jnz 0x1e82
00001E7E  B001              mov al,0x1
00001E80  EB1F              jmp short 0x1ea1
00001E82  8B4606            mov ax,[bp+0x6]
00001E85  BA0600            mov dx,0x6
00001E88  F7EA              imul dx
00001E8A  034608            add ax,[bp+0x8]
00001E8D  8BD8              mov bx,ax
00001E8F  8A874244          mov al,[bx+0x4442]
00001E93  98                cbw
00001E94  3B460A            cmp ax,[bp+0xa]
00001E97  7404              jz 0x1e9d
00001E99  B001              mov al,0x1
00001E9B  EB04              jmp short 0x1ea1
00001E9D  B000              mov al,0x0
00001E9F  EB00              jmp short 0x1ea1
00001EA1  5D                pop bp
00001EA2  CB                retf

; ---- 0609:1EA3 ----
00001EA3  55                push bp
00001EA4  8BEC              mov bp,sp
00001EA6  9A1B00960A        call 0xa96:0x1b
00001EAB  C606AA3700        mov byte [0x37aa],0x0
00001EB0  EB19              jmp short 0x1ecb
00001EB2  A0AA37            mov al,[0x37aa]
00001EB5  98                cbw
00001EB6  8BD8              mov bx,ax
00001EB8  80BFB24306        cmp byte [bx+0x43b2],0x6
00001EBD  7408              jz 0x1ec7
00001EBF  0E                push cs
00001EC0  E8FDFB            call 0x1ac0
00001EC3  0E                push cs
00001EC4  E83CFB            call 0x1a03
00001EC7  FE06AA37          inc byte [0x37aa]
00001ECB  803EAA3704        cmp byte [0x37aa],0x4
00001ED0  7CE0              jl 0x1eb2
00001ED2  9A0D06FD21        call 0x21fd:0x60d
00001ED7  5D                pop bp
00001ED8  CB                retf

; ---- 0609:1ED9 ----
00001ED9  55                push bp
00001EDA  8BEC              mov bp,sp
00001EDC  B8C800            mov ax,0xc8
00001EDF  50                push ax
00001EE0  B84001            mov ax,0x140
00001EE3  50                push ax
00001EE4  33C0              xor ax,ax
00001EE6  50                push ax
00001EE7  33C0              xor ax,ax
00001EE9  50                push ax
00001EEA  B81600            mov ax,0x16
00001EED  50                push ax
00001EEE  B82000            mov ax,0x20
00001EF1  50                push ax
00001EF2  B85800            mov ax,0x58
00001EF5  50                push ax
00001EF6  B82001            mov ax,0x120
00001EF9  50                push ax
00001EFA  9A6702960A        call 0xa96:0x267
00001EFF  83C410            add sp,byte +0x10
00001F02  5D                pop bp
00001F03  CB                retf

; ---- 0609:1F04 ----
00001F04  55                push bp
00001F05  8BEC              mov bp,sp
00001F07  FF760C            push word [bp+0xc]
00001F0A  FF760A            push word [bp+0xa]
00001F0D  FF7608            push word [bp+0x8]
00001F10  FF7606            push word [bp+0x6]
00001F13  B81600            mov ax,0x16
00001F16  50                push ax
00001F17  B82000            mov ax,0x20
00001F1A  50                push ax
00001F1B  B8B000            mov ax,0xb0
00001F1E  50                push ax
00001F1F  B8F000            mov ax,0xf0
00001F22  50                push ax
00001F23  9A6702960A        call 0xa96:0x267
00001F28  83C410            add sp,byte +0x10
00001F2B  5D                pop bp
00001F2C  CB                retf

; ---- 0609:1F2D ----
00001F2D  55                push bp
00001F2E  8BEC              mov bp,sp
00001F30  56                push si
00001F31  C606AA3700        mov byte [0x37aa],0x0
00001F36  E98600            jmp 0x1fbf
00001F39  A0AA37            mov al,[0x37aa]
00001F3C  98                cbw
00001F3D  BA1A00            mov dx,0x1a
00001F40  F7EA              imul dx
00001F42  8BD8              mov bx,ax
00001F44  C7870E330A00      mov word [bx+0x330e],0xa
00001F4A  9ACD04AF21        call 0x21af:0x4cd
00001F4F  A0AA37            mov al,[0x37aa]
00001F52  98                cbw
00001F53  BA0A00            mov dx,0xa
00001F56  F7EA              imul dx
00001F58  052800            add ax,0x28
00001F5B  50                push ax
00001F5C  B82001            mov ax,0x120
00001F5F  50                push ax
00001F60  A0AA37            mov al,[0x37aa]
00001F63  98                cbw
00001F64  BA0A00            mov dx,0xa
00001F67  F7EA              imul dx
00001F69  50                push ax
00001F6A  B82001            mov ax,0x120
00001F6D  50                push ax
00001F6E  A0AA37            mov al,[0x37aa]
00001F71  98                cbw
00001F72  50                push ax
00001F73  9A2B02AF21        call 0x21af:0x22b
00001F78  83C40A            add sp,byte +0xa
00001F7B  33F6              xor si,si
00001F7D  EB37              jmp short 0x1fb6
00001F7F  A0AA37            mov al,[0x37aa]
00001F82  98                cbw
00001F83  BA0A00            mov dx,0xa
00001F86  F7EA              imul dx
00001F88  50                push ax
00001F89  8BC6              mov ax,si
00001F8B  B104              mov cl,0x4
00001F8D  D3E0              shl ax,cl
00001F8F  50                push ax
00001F90  A0AA37            mov al,[0x37aa]
00001F93  98                cbw
00001F94  BA0A00            mov dx,0xa
00001F97  F7EA              imul dx
00001F99  05C600            add ax,0xc6
00001F9C  50                push ax
00001F9D  8BC6              mov ax,si
00001F9F  B104              mov cl,0x4
00001FA1  D3E0              shl ax,cl
00001FA3  053000            add ax,0x30
00001FA6  50                push ax
00001FA7  56                push si
00001FA8  A0AA37            mov al,[0x37aa]
00001FAB  98                cbw
00001FAC  50                push ax
00001FAD  9ACA00AF21        call 0x21af:0xca
00001FB2  83C40C            add sp,byte +0xc
00001FB5  46                inc si
00001FB6  83FE09            cmp si,byte +0x9
00001FB9  7CC4              jl 0x1f7f
00001FBB  FE06AA37          inc byte [0x37aa]
00001FBF  803EAA3704        cmp byte [0x37aa],0x4
00001FC4  7D03              jnl 0x1fc9
00001FC6  E970FF            jmp 0x1f39
00001FC9  5E                pop si
00001FCA  5D                pop bp
00001FCB  CB                retf

; ---- 0609:1FCC ----
00001FCC  55                push bp
00001FCD  8BEC              mov bp,sp
00001FCF  33C0              xor ax,ax
00001FD1  50                push ax
00001FD2  A0AA37            mov al,[0x37aa]
00001FD5  98                cbw
00001FD6  50                push ax
00001FD7  9A1204AF21        call 0x21af:0x412
00001FDC  59                pop cx
00001FDD  59                pop cx
00001FDE  5D                pop bp
00001FDF  CB                retf

; ---- 0609:1FE0 ----
00001FE0  55                push bp
00001FE1  8BEC              mov bp,sp
00001FE3  56                push si
00001FE4  57                push di
00001FE5  A0AA37            mov al,[0x37aa]
00001FE8  98                cbw
00001FE9  BA1A00            mov dx,0x1a
00001FEC  F7EA              imul dx
00001FEE  8BD8              mov bx,ax
00001FF0  8BBF2433          mov di,[bx+0x3324]
00001FF4  A0AA37            mov al,[0x37aa]
00001FF7  98                cbw
00001FF8  D1E0              shl ax,1
00001FFA  8BD8              mov bx,ax
00001FFC  8B872230          mov ax,[bx+0x3022]
00002000  48                dec ax
00002001  50                push ax
00002002  A0AA37            mov al,[0x37aa]
00002005  98                cbw
00002006  D1E0              shl ax,1
00002008  8BD8              mov bx,ax
0000200A  58                pop ax
0000200B  89872230          mov [bx+0x3022],ax
0000200F  0BC0              or ax,ax
00002011  7505              jnz 0x2018
00002013  33F6              xor si,si
00002015  E91C01            jmp 0x2134
00002018  A0AA37            mov al,[0x37aa]
0000201B  98                cbw
0000201C  D1E0              shl ax,1
0000201E  8BD8              mov bx,ax
00002020  8B9FCA4B          mov bx,[bx+0x4bca]
00002024  83FB03            cmp bx,byte +0x3
00002027  7603              jna 0x202c
00002029  E9D600            jmp 0x2102
0000202C  D1E3              shl bx,1
0000202E  2EFFA75621        jmp [cs:bx+0x2156]
00002033  83FF05            cmp di,byte +0x5
00002036  7505              jnz 0x203d
00002038  B80600            mov ax,0x6
0000203B  EB03              jmp short 0x2040
0000203D  B80500            mov ax,0x5
00002040  8BF0              mov si,ax
00002042  A0AA37            mov al,[0x37aa]
00002045  98                cbw
00002046  BA1A00            mov dx,0x1a
00002049  F7EA              imul dx
0000204B  8BD8              mov bx,ax
0000204D  8B872033          mov ax,[bx+0x3320]
00002051  05FEFF            add ax,0xfffe
00002054  50                push ax
00002055  A0AA37            mov al,[0x37aa]
00002058  98                cbw
00002059  BA1A00            mov dx,0x1a
0000205C  F7EA              imul dx
0000205E  8BD8              mov bx,ax
00002060  58                pop ax
00002061  89872033          mov [bx+0x3320],ax
00002065  E99A00            jmp 0x2102
00002068  83FF07            cmp di,byte +0x7
0000206B  7505              jnz 0x2072
0000206D  B80800            mov ax,0x8
00002070  EB03              jmp short 0x2075
00002072  B80700            mov ax,0x7
00002075  8BF0              mov si,ax
00002077  A0AA37            mov al,[0x37aa]
0000207A  98                cbw
0000207B  BA1A00            mov dx,0x1a
0000207E  F7EA              imul dx
00002080  8BD8              mov bx,ax
00002082  8B872033          mov ax,[bx+0x3320]
00002086  40                inc ax
00002087  40                inc ax
00002088  50                push ax
00002089  A0AA37            mov al,[0x37aa]
0000208C  98                cbw
0000208D  BA1A00            mov dx,0x1a
00002090  F7EA              imul dx
00002092  8BD8              mov bx,ax
00002094  58                pop ax
00002095  89872033          mov [bx+0x3320],ax
00002099  EB67              jmp short 0x2102
0000209B  83FF03            cmp di,byte +0x3
0000209E  7505              jnz 0x20a5
000020A0  B80400            mov ax,0x4
000020A3  EB03              jmp short 0x20a8
000020A5  B80300            mov ax,0x3
000020A8  8BF0              mov si,ax
000020AA  A0AA37            mov al,[0x37aa]
000020AD  98                cbw
000020AE  BA1A00            mov dx,0x1a
000020B1  F7EA              imul dx
000020B3  8BD8              mov bx,ax
000020B5  8B872233          mov ax,[bx+0x3322]
000020B9  05FEFF            add ax,0xfffe
000020BC  50                push ax
000020BD  A0AA37            mov al,[0x37aa]
000020C0  98                cbw
000020C1  BA1A00            mov dx,0x1a
000020C4  F7EA              imul dx
000020C6  8BD8              mov bx,ax
000020C8  58                pop ax
000020C9  89872233          mov [bx+0x3322],ax
000020CD  EB33              jmp short 0x2102
000020CF  83FF01            cmp di,byte +0x1
000020D2  7505              jnz 0x20d9
000020D4  B80200            mov ax,0x2
000020D7  EB03              jmp short 0x20dc
000020D9  B80100            mov ax,0x1
000020DC  8BF0              mov si,ax
000020DE  A0AA37            mov al,[0x37aa]
000020E1  98                cbw
000020E2  BA1A00            mov dx,0x1a
000020E5  F7EA              imul dx
000020E7  8BD8              mov bx,ax
000020E9  8B872233          mov ax,[bx+0x3322]
000020ED  40                inc ax
000020EE  40                inc ax
000020EF  50                push ax
000020F0  A0AA37            mov al,[0x37aa]
000020F3  98                cbw
000020F4  BA1A00            mov dx,0x1a
000020F7  F7EA              imul dx
000020F9  8BD8              mov bx,ax
000020FB  58                pop ax
000020FC  89872233          mov [bx+0x3322],ax
00002100  EB00              jmp short 0x2102
00002102  A0AA37            mov al,[0x37aa]
00002105  98                cbw
00002106  D1E0              shl ax,1
00002108  8BD8              mov bx,ax
0000210A  83BFD24B00        cmp word [bx+0x4bd2],byte +0x0
0000210F  7E23              jng 0x2134
00002111  A0AA37            mov al,[0x37aa]
00002114  98                cbw
00002115  D1E0              shl ax,1
00002117  8BD8              mov bx,ax
00002119  8B87D24B          mov ax,[bx+0x4bd2]
0000211D  48                dec ax
0000211E  50                push ax
0000211F  A0AA37            mov al,[0x37aa]
00002122  98                cbw
00002123  D1E0              shl ax,1
00002125  8BD8              mov bx,ax
00002127  58                pop ax
00002128  8987D24B          mov [bx+0x4bd2],ax
0000212C  0BC0              or ax,ax
0000212E  7504              jnz 0x2134
00002130  0E                push cs
00002131  E8CFF8            call 0x1a03
00002134  A0AA37            mov al,[0x37aa]
00002137  98                cbw
00002138  BA1A00            mov dx,0x1a
0000213B  F7EA              imul dx
0000213D  8BD8              mov bx,ax
0000213F  89B72433          mov [bx+0x3324],si
00002143  B80F00            mov ax,0xf
00002146  50                push ax
00002147  B80200            mov ax,0x2
0000214A  50                push ax
0000214B  9A5400A721        call 0x21a7:0x54
00002150  59                pop cx
00002151  59                pop cx
00002152  5F                pop di
00002153  5E                pop si
00002154  5D                pop bp
00002155  CB                retf
00002156  682033            push word 0x3320
00002159  209B20CF          and [bp+di-0x30e0],bl
0000215D  20558B            and [di-0x75],dl
00002160  EC                in al,dx
00002161  A0AA37            mov al,[0x37aa]
00002164  98                cbw
00002165  8BD8              mov bx,ax
00002167  80BFE00000        cmp byte [bx+0xe0],0x0
0000216C  7431              jz 0x219f
0000216E  A0AA37            mov al,[0x37aa]
00002171  98                cbw
00002172  8BD8              mov bx,ax
00002174  FE8FE000          dec byte [bx+0xe0]
00002178  A0AA37            mov al,[0x37aa]
0000217B  98                cbw
0000217C  8BD8              mov bx,ax
0000217E  8A87E000          mov al,[bx+0xe0]
00002182  B400              mov ah,0x0
00002184  BB0200            mov bx,0x2
00002187  99                cwd
00002188  F7FB              idiv bx
0000218A  D1E2              shl dx,1
0000218C  A0AA37            mov al,[0x37aa]
0000218F  98                cbw
00002190  BB1A00            mov bx,0x1a
00002193  52                push dx
00002194  F7EB              imul bx
00002196  8BD8              mov bx,ax
00002198  58                pop ax
00002199  89872433          mov [bx+0x3324],ax
0000219D  EB19              jmp short 0x21b8
0000219F  B8C800            mov ax,0xc8
000021A2  50                push ax
000021A3  9ABA01FD21        call 0x21fd:0x1ba
000021A8  59                pop cx
000021A9  0BC0              or ax,ax
000021AB  750B              jnz 0x21b8
000021AD  A0AA37            mov al,[0x37aa]
000021B0  98                cbw
000021B1  8BD8              mov bx,ax
000021B3  C687E00013        mov byte [bx+0xe0],0x13
000021B8  5D                pop bp
000021B9  CB                retf

; ---- 0609:21BA ----
000021BA  55                push bp
000021BB  8BEC              mov bp,sp
000021BD  A0AA37            mov al,[0x37aa]
