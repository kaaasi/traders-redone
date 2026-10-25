; segment 150F  file offset 0x186F0  size 0x1C70
00000000  762D              jna 0x2f
00000002  46                inc si
00000003  83FE03            cmp si,byte +0x3
00000006  7EE7              jng 0xffef
00000008  5E                pop si
00000009  5D                pop bp
0000000A  CB                retf

; ---- 150F:000B ----
0000000B  55                push bp
0000000C  8BEC              mov bp,sp
0000000E  56                push si
0000000F  8B7606            mov si,[bp+0x6]
00000012  C684BA4BFF        mov byte [si+0x4bba],0xff
00000017  C6848034FF        mov byte [si+0x3480],0xff
0000001C  C684CD3C00        mov byte [si+0x3ccd],0x0
00000021  C684C73DFF        mov byte [si+0x3dc7],0xff
00000026  C684E23CFF        mov byte [si+0x3ce2],0xff
0000002B  C684E448FF        mov byte [si+0x48e4],0xff
00000030  C684A44B00        mov byte [si+0x4ba4],0x0
00000035  C684C643FF        mov byte [si+0x43c6],0xff
0000003A  C684C04501        mov byte [si+0x45c0],0x1
0000003F  C684AB3700        mov byte [si+0x37ab],0x0
00000044  C684C837FF        mov byte [si+0x37c8],0xff
00000049  5E                pop si
0000004A  5D                pop bp
0000004B  CB                retf

; ---- 150F:004C ----
0000004C  55                push bp
0000004D  8BEC              mov bp,sp
0000004F  56                push si
00000050  C606D42DFF        mov byte [0x2dd4],0xff
00000055  833E8E2E01        cmp word [0x2e8e],byte +0x1
0000005A  7F0A              jg 0x66
0000005C  803E814500        cmp byte [0x4581],0x0
00000061  7503              jnz 0x66
00000063  E9C500            jmp 0x12b
00000066  9ACB062508        call 0x825:0x6cb
0000006B  C606AA3700        mov byte [0x37aa],0x0
00000070  EB13              jmp short 0x85
00000072  A0AA37            mov al,[0x37aa]
00000075  98                cbw
00000076  8BD8              mov bx,ax
00000078  80BF2A3000        cmp byte [bx+0x302a],0x0
0000007D  7502              jnz 0x81
0000007F  EB0B              jmp short 0x8c
00000081  FE06AA37          inc byte [0x37aa]
00000085  803EAA3704        cmp byte [0x37aa],0x4
0000008A  7CE6              jl 0x72
0000008C  33C0              xor ax,ax
0000008E  BA0A00            mov dx,0xa
00000091  50                push ax
00000092  52                push dx
00000093  FF36F608          push word [0x8f6]
00000097  FF36F408          push word [0x8f4]
0000009B  9A904F0000        call 0x0:0x4f90
000000A0  50                push ax
000000A1  52                push dx
000000A2  33D2              xor dx,dx
000000A4  B80900            mov ax,0x9
000000A7  59                pop cx
000000A8  5B                pop bx
000000A9  9A4E4F0000        call 0x0:0x4f4e
000000AE  50                push ax
000000AF  A0AA37            mov al,[0x37aa]
000000B2  98                cbw
000000B3  D1E0              shl ax,1
000000B5  D1E0              shl ax,1
000000B7  8BD8              mov bx,ax
000000B9  3B97082F          cmp dx,[bx+0x2f08]
000000BD  58                pop ax
000000BE  7216              jc 0xd6
000000C0  7706              ja 0xc8
000000C2  3B87062F          cmp ax,[bx+0x2f06]
000000C6  760E              jna 0xd6
000000C8  B80400            mov ax,0x4
000000CB  50                push ax
000000CC  9ABA01FD21        call 0x21fd:0x1ba
000000D1  59                pop cx
000000D2  0BC0              or ax,ax
000000D4  7555              jnz 0x12b
000000D6  B80300            mov ax,0x3
000000D9  50                push ax
000000DA  9ABA01FD21        call 0x21fd:0x1ba
000000DF  59                pop cx
000000E0  A2AA37            mov [0x37aa],al
000000E3  33F6              xor si,si
000000E5  EB3F              jmp short 0x126
000000E7  A0AA37            mov al,[0x37aa]
000000EA  FEC0              inc al
000000EC  A2AA37            mov [0x37aa],al
000000EF  3C03              cmp al,0x3
000000F1  7E05              jng 0xf8
000000F3  C606AA3700        mov byte [0x37aa],0x0
000000F8  A0AA37            mov al,[0x37aa]
000000FB  98                cbw
000000FC  8BD8              mov bx,ax
000000FE  80BF400801        cmp byte [bx+0x840],0x1
00000103  7509              jnz 0x10e
00000105  803E814500        cmp byte [0x4581],0x0
0000010A  7502              jnz 0x10e
0000010C  EB17              jmp short 0x125
0000010E  A0AA37            mov al,[0x37aa]
00000111  98                cbw
00000112  8BD8              mov bx,ax
00000114  80BF423D00        cmp byte [bx+0x3d42],0x0
00000119  7602              jna 0x11d
0000011B  EB08              jmp short 0x125
0000011D  A0AA37            mov al,[0x37aa]
00000120  A2D42D            mov [0x2dd4],al
00000123  EB06              jmp short 0x12b
00000125  46                inc si
00000126  83FE03            cmp si,byte +0x3
00000129  7EBC              jng 0xe7
0000012B  5E                pop si
0000012C  5D                pop bp
0000012D  CB                retf

; ---- 150F:012E ----
0000012E  55                push bp
0000012F  8BEC              mov bp,sp
00000131  83EC06            sub sp,byte +0x6
00000134  56                push si
00000135  C606AA3700        mov byte [0x37aa],0x0
0000013A  EB1D              jmp short 0x159
0000013C  33F6              xor si,si
0000013E  EB10              jmp short 0x150
00000140  A0AA37            mov al,[0x37aa]
00000143  98                cbw
00000144  D1E0              shl ax,1
00000146  D1E0              shl ax,1
00000148  8BD8              mov bx,ax
0000014A  C680B33700        mov byte [bx+si+0x37b3],0x0
0000014F  46                inc si
00000150  83FE04            cmp si,byte +0x4
00000153  7CEB              jl 0x140
00000155  FE06AA37          inc byte [0x37aa]
00000159  803EAA3704        cmp byte [0x37aa],0x4
0000015E  7CDC              jl 0x13c
00000160  C746FC0000        mov word [bp-0x4],0x0
00000165  EB27              jmp short 0x18e
00000167  C746FE0000        mov word [bp-0x2],0x0
0000016C  EB17              jmp short 0x185
0000016E  8B5EFE            mov bx,[bp-0x2]
00000171  B104              mov cl,0x4
00000173  D3E3              shl bx,cl
00000175  8B46FC            mov ax,[bp-0x4]
00000178  D1E0              shl ax,1
0000017A  03D8              add bx,ax
0000017C  C78704488888      mov word [bx+0x4804],0x8888
00000182  FF46FE            inc word [bp-0x2]
00000185  837EFE07          cmp word [bp-0x2],byte +0x7
00000189  7EE3              jng 0x16e
0000018B  FF46FC            inc word [bp-0x4]
0000018E  837EFC07          cmp word [bp-0x4],byte +0x7
00000192  7ED3              jng 0x167
00000194  C606AA3700        mov byte [0x37aa],0x0
00000199  E99000            jmp 0x22c
0000019C  C606B54B00        mov byte [0x4bb5],0x0
000001A1  C606094101        mov byte [0x4109],0x1
000001A6  A0AA37            mov al,[0x37aa]
000001A9  98                cbw
000001AA  8BD8              mov bx,ax
000001AC  8A874C08          mov al,[bx+0x84c]
000001B0  B400              mov ah,0x0
000001B2  48                dec ax
000001B3  BB0300            mov bx,0x3
000001B6  99                cwd
000001B7  F7FB              idiv bx
000001B9  8946FE            mov [bp-0x2],ax
000001BC  A0AA37            mov al,[0x37aa]
000001BF  98                cbw
000001C0  8BD8              mov bx,ax
000001C2  8A875008          mov al,[bx+0x850]
000001C6  B400              mov ah,0x0
000001C8  48                dec ax
000001C9  BB0300            mov bx,0x3
000001CC  99                cwd
000001CD  F7FB              idiv bx
000001CF  8946FC            mov [bp-0x4],ax
000001D2  B000              mov al,0x0
000001D4  50                push ax
000001D5  8A46FC            mov al,[bp-0x4]
000001D8  50                push ax
000001D9  8A46FE            mov al,[bp-0x2]
000001DC  50                push ax
000001DD  9A38040F15        call 0x150f:0x438
000001E2  83C406            add sp,byte +0x6
000001E5  C606B44B00        mov byte [0x4bb4],0x0
000001EA  A0B54B            mov al,[0x4bb5]
000001ED  98                cbw
000001EE  BA1E00            mov dx,0x1e
000001F1  F7EA              imul dx
000001F3  50                push ax
000001F4  A0B44B            mov al,[0x4bb4]
000001F7  98                cbw
000001F8  5B                pop bx
000001F9  03D8              add bx,ax
000001FB  8A46FE            mov al,[bp-0x2]
000001FE  8887D52D          mov [bx+0x2dd5],al
00000202  A0B54B            mov al,[0x4bb5]
00000205  98                cbw
00000206  BA1E00            mov dx,0x1e
00000209  F7EA              imul dx
0000020B  50                push ax
0000020C  A0B44B            mov al,[0x4bb4]
0000020F  98                cbw
00000210  5B                pop bx
00000211  03D8              add bx,ax
00000213  8A46FC            mov al,[bp-0x4]
00000216  8887112E          mov [bx+0x2e11],al
0000021A  9AC2020F15        call 0x150f:0x2c2
0000021F  B002              mov al,0x2
00000221  50                push ax
00000222  9A5A060F15        call 0x150f:0x65a
00000227  59                pop cx
00000228  FE06AA37          inc byte [0x37aa]
0000022C  803EAA3704        cmp byte [0x37aa],0x4
00000231  7D03              jnl 0x236
00000233  E966FF            jmp 0x19c
00000236  C606AA3700        mov byte [0x37aa],0x0
0000023B  EB1C              jmp short 0x259
0000023D  C606B44BFF        mov byte [0x4bb4],0xff
00000242  9AD1040F15        call 0x150f:0x4d1
00000247  9AC2020F15        call 0x150f:0x2c2
0000024C  B001              mov al,0x1
0000024E  50                push ax
0000024F  9A5A060F15        call 0x150f:0x65a
00000254  59                pop cx
00000255  FE06AA37          inc byte [0x37aa]
00000259  803EAA3704        cmp byte [0x37aa],0x4
0000025E  7CDD              jl 0x23d
00000260  C606AA3700        mov byte [0x37aa],0x0
00000265  EB47              jmp short 0x2ae
00000267  33F6              xor si,si
00000269  EB3A              jmp short 0x2a5
0000026B  8BDE              mov bx,si
0000026D  D1E3              shl bx,1
0000026F  D1E3              shl bx,1
00000271  A0AA37            mov al,[0x37aa]
00000274  98                cbw
00000275  03D8              add bx,ax
00000277  8A87B337          mov al,[bx+0x37b3]
0000027B  98                cbw
0000027C  8946FA            mov [bp-0x6],ax
0000027F  A0AA37            mov al,[0x37aa]
00000282  98                cbw
00000283  D1E0              shl ax,1
00000285  D1E0              shl ax,1
00000287  8BD8              mov bx,ax
00000289  8A80B337          mov al,[bx+si+0x37b3]
0000028D  98                cbw
0000028E  3B46FA            cmp ax,[bp-0x6]
00000291  7E11              jng 0x2a4
00000293  A0AA37            mov al,[0x37aa]
00000296  98                cbw
00000297  D1E0              shl ax,1
00000299  D1E0              shl ax,1
0000029B  8A56FA            mov dl,[bp-0x6]
0000029E  8BD8              mov bx,ax
000002A0  8890B337          mov [bx+si+0x37b3],dl
000002A4  46                inc si
000002A5  83FE04            cmp si,byte +0x4
000002A8  7CC1              jl 0x26b
000002AA  FE06AA37          inc byte [0x37aa]
000002AE  803EAA3704        cmp byte [0x37aa],0x4
000002B3  7CB2              jl 0x267
000002B5  A0C737            mov al,[0x37c7]
000002B8  24FE              and al,0xfe
000002BA  A2C737            mov [0x37c7],al
000002BD  5E                pop si
000002BE  8BE5              mov sp,bp
000002C0  5D                pop bp
000002C1  CB                retf

; ---- 150F:02C2 ----
000002C2  55                push bp
000002C3  8BEC              mov bp,sp
000002C5  56                push si
000002C6  57                push di
000002C7  EB5D              jmp short 0x326
000002C9  C6060841FF        mov byte [0x4108],0xff
000002CE  33FF              xor di,di
000002D0  EB39              jmp short 0x30b
000002D2  33F6              xor si,si
000002D4  EB2C              jmp short 0x302
000002D6  57                push di
000002D7  A0B54B            mov al,[0x4bb5]
000002DA  98                cbw
000002DB  BA1E00            mov dx,0x1e
000002DE  F7EA              imul dx
000002E0  8BD8              mov bx,ax
000002E2  8A80112E          mov al,[bx+si+0x2e11]
000002E6  98                cbw
000002E7  50                push ax
000002E8  A0B54B            mov al,[0x4bb5]
000002EB  98                cbw
000002EC  BA1E00            mov dx,0x1e
000002EF  F7EA              imul dx
000002F1  8BD8              mov bx,ax
000002F3  8A80D52D          mov al,[bx+si+0x2dd5]
000002F7  98                cbw
000002F8  50                push ax
000002F9  9A31030F15        call 0x150f:0x331
000002FE  83C406            add sp,byte +0x6
00000301  46                inc si
00000302  A0B44B            mov al,[0x4bb4]
00000305  98                cbw
00000306  3BC6              cmp ax,si
00000308  7DCC              jnl 0x2d6
0000030A  47                inc di
0000030B  83FF01            cmp di,byte +0x1
0000030E  7EC2              jng 0x2d2
00000310  A00841            mov al,[0x4108]
00000313  A2B44B            mov [0x4bb4],al
00000316  A0B54B            mov al,[0x4bb5]
00000319  3401              xor al,0x1
0000031B  A2B54B            mov [0x4bb5],al
0000031E  A00941            mov al,[0x4109]
00000321  3401              xor al,0x1
00000323  A20941            mov [0x4109],al
00000326  803EB44B00        cmp byte [0x4bb4],0x0
0000032B  7D9C              jnl 0x2c9
0000032D  5F                pop di
0000032E  5E                pop si
0000032F  5D                pop bp
00000330  CB                retf

; ---- 150F:0331 ----
00000331  55                push bp
00000332  8BEC              mov bp,sp
00000334  83EC04            sub sp,byte +0x4
00000337  56                push si
00000338  8B760A            mov si,[bp+0xa]
0000033B  E9BB00            jmp 0x3f9
0000033E  8A847808          mov al,[si+0x878]
00000342  98                cbw
00000343  8B5606            mov dx,[bp+0x6]
00000346  03D0              add dx,ax
00000348  8956FE            mov [bp-0x2],dx
0000034B  8A848008          mov al,[si+0x880]
0000034F  98                cbw
00000350  8B5608            mov dx,[bp+0x8]
00000353  03D0              add dx,ax
00000355  8956FC            mov [bp-0x4],dx
00000358  837EFE00          cmp word [bp-0x2],byte +0x0
0000035C  7C12              jl 0x370
0000035E  837EFE07          cmp word [bp-0x2],byte +0x7
00000362  7F0C              jg 0x370
00000364  837EFC00          cmp word [bp-0x4],byte +0x0
00000368  7C06              jl 0x370
0000036A  837EFC07          cmp word [bp-0x4],byte +0x7
0000036E  7E03              jng 0x373
00000370  E98000            jmp 0x3f3
00000373  A0AA37            mov al,[0x37aa]
00000376  50                push ax
00000377  8A46FC            mov al,[bp-0x4]
0000037A  50                push ax
0000037B  8A46FE            mov al,[bp-0x2]
0000037E  50                push ax
0000037F  9A06040F15        call 0x150f:0x406
00000384  83C406            add sp,byte +0x6
00000387  3C08              cmp al,0x8
00000389  7402              jz 0x38d
0000038B  EB66              jmp short 0x3f3
0000038D  8B5EFE            mov bx,[bp-0x2]
00000390  D1E3              shl bx,1
00000392  D1E3              shl bx,1
00000394  D1E3              shl bx,1
00000396  035EFC            add bx,[bp-0x4]
00000399  80BF784400        cmp byte [bx+0x4478],0x0
0000039E  7502              jnz 0x3a2
000003A0  EB51              jmp short 0x3f3
000003A2  8A84D007          mov al,[si+0x7d0]
000003A6  50                push ax
000003A7  8A46FC            mov al,[bp-0x4]
000003AA  50                push ax
000003AB  8A46FE            mov al,[bp-0x2]
000003AE  50                push ax
000003AF  9A38040F15        call 0x150f:0x438
000003B4  83C406            add sp,byte +0x6
000003B7  A00841            mov al,[0x4108]
000003BA  FEC0              inc al
000003BC  A20841            mov [0x4108],al
000003BF  3C1E              cmp al,0x1e
000003C1  7D30              jnl 0x3f3
000003C3  A00941            mov al,[0x4109]
000003C6  98                cbw
000003C7  BA1E00            mov dx,0x1e
000003CA  F7EA              imul dx
000003CC  50                push ax
000003CD  A00841            mov al,[0x4108]
000003D0  98                cbw
000003D1  5B                pop bx
000003D2  03D8              add bx,ax
000003D4  8A46FE            mov al,[bp-0x2]
000003D7  8887D52D          mov [bx+0x2dd5],al
000003DB  A00941            mov al,[0x4109]
000003DE  98                cbw
000003DF  BA1E00            mov dx,0x1e
000003E2  F7EA              imul dx
000003E4  50                push ax
000003E5  A00841            mov al,[0x4108]
000003E8  98                cbw
000003E9  5B                pop bx
000003EA  03D8              add bx,ax
000003EC  8A46FC            mov al,[bp-0x4]
000003EF  8887112E          mov [bx+0x2e11],al
000003F3  8BC6              mov ax,si
000003F5  40                inc ax
000003F6  40                inc ax
000003F7  8BF0              mov si,ax
000003F9  83FE07            cmp si,byte +0x7
000003FC  7F03              jg 0x401
000003FE  E93DFF            jmp 0x33e
00000401  5E                pop si
00000402  8BE5              mov sp,bp
00000404  5D                pop bp
00000405  CB                retf

; ---- 150F:0406 ----
00000406  55                push bp
00000407  8BEC              mov bp,sp
00000409  8A4606            mov al,[bp+0x6]
0000040C  B400              mov ah,0x0
0000040E  B104              mov cl,0x4
00000410  D3E0              shl ax,cl
00000412  8A5608            mov dl,[bp+0x8]
00000415  B600              mov dh,0x0
00000417  D1E2              shl dx,1
00000419  03C2              add ax,dx
0000041B  8BD8              mov bx,ax
0000041D  8B870448          mov ax,[bx+0x4804]
00000421  8A560A            mov dl,[bp+0xa]
00000424  B600              mov dh,0x0
00000426  D1E2              shl dx,1
00000428  50                push ax
00000429  52                push dx
0000042A  33D2              xor dx,dx
0000042C  5B                pop bx
0000042D  58                pop ax
0000042E  F7B7D807          div word [bx+0x7d8]
00000432  240F              and al,0xf
00000434  EB00              jmp short 0x436
00000436  5D                pop bp
00000437  CB                retf

; ---- 150F:0438 ----
00000438  55                push bp
00000439  8BEC              mov bp,sp
0000043B  8A4606            mov al,[bp+0x6]
0000043E  B400              mov ah,0x0
00000440  B104              mov cl,0x4
00000442  D3E0              shl ax,cl
00000444  8A5608            mov dl,[bp+0x8]
00000447  B600              mov dh,0x0
00000449  D1E2              shl dx,1
0000044B  03C2              add ax,dx
0000044D  50                push ax
0000044E  A0AA37            mov al,[0x37aa]
00000451  98                cbw
00000452  D1E0              shl ax,1
00000454  8BD8              mov bx,ax
00000456  8B87E807          mov ax,[bx+0x7e8]
0000045A  5B                pop bx
0000045B  21870448          and [bx+0x4804],ax
0000045F  8A4606            mov al,[bp+0x6]
00000462  B400              mov ah,0x0
00000464  B104              mov cl,0x4
00000466  D3E0              shl ax,cl
00000468  8A5608            mov dl,[bp+0x8]
0000046B  B600              mov dh,0x0
0000046D  D1E2              shl dx,1
0000046F  03C2              add ax,dx
00000471  8BD8              mov bx,ax
00000473  8B870448          mov ax,[bx+0x4804]
00000477  50                push ax
00000478  A0AA37            mov al,[0x37aa]
0000047B  98                cbw
0000047C  D1E0              shl ax,1
0000047E  8BD8              mov bx,ax
00000480  8B87E007          mov ax,[bx+0x7e0]
00000484  8A560A            mov dl,[bp+0xa]
00000487  B600              mov dh,0x0
00000489  F7EA              imul dx
0000048B  5A                pop dx
0000048C  03D0              add dx,ax
0000048E  8A4606            mov al,[bp+0x6]
00000491  B400              mov ah,0x0
00000493  B104              mov cl,0x4
00000495  D3E0              shl ax,cl
00000497  8A5E08            mov bl,[bp+0x8]
0000049A  B700              mov bh,0x0
0000049C  D1E3              shl bx,1
0000049E  03C3              add ax,bx
000004A0  8BD8              mov bx,ax
000004A2  89970448          mov [bx+0x4804],dx
000004A6  5D                pop bp
000004A7  CB                retf
000004A8  55                push bp
000004A9  8BEC              mov bp,sp
000004AB  807E0604          cmp byte [bp+0x6],0x4
000004AF  7C10              jl 0x4c1
000004B1  807E0804          cmp byte [bp+0x8],0x4
000004B5  7C04              jl 0x4bb
000004B7  B002              mov al,0x2
000004B9  EB02              jmp short 0x4bd
000004BB  B001              mov al,0x1
000004BD  EB10              jmp short 0x4cf
000004BF  EB0E              jmp short 0x4cf
000004C1  807E0804          cmp byte [bp+0x8],0x4
000004C5  7C04              jl 0x4cb
000004C7  B003              mov al,0x3
000004C9  EB02              jmp short 0x4cd
000004CB  B000              mov al,0x0
000004CD  EB00              jmp short 0x4cf
000004CF  5D                pop bp
000004D0  CB                retf

; ---- 150F:04D1 ----
000004D1  55                push bp
000004D2  8BEC              mov bp,sp
000004D4  83EC04            sub sp,byte +0x4
000004D7  A0AA37            mov al,[0x37aa]
000004DA  98                cbw
000004DB  8BD8              mov bx,ax
000004DD  8A874C08          mov al,[bx+0x84c]
000004E1  B400              mov ah,0x0
000004E3  48                dec ax
000004E4  BB0300            mov bx,0x3
000004E7  99                cwd
000004E8  F7FB              idiv bx
000004EA  8846FF            mov [bp-0x1],al
000004ED  A0AA37            mov al,[0x37aa]
000004F0  98                cbw
000004F1  8BD8              mov bx,ax
000004F3  8A875008          mov al,[bx+0x850]
000004F7  B400              mov ah,0x0
000004F9  48                dec ax
000004FA  BB0300            mov bx,0x3
000004FD  99                cwd
000004FE  F7FB              idiv bx
00000500  8846FE            mov [bp-0x2],al
00000503  A0AA37            mov al,[0x37aa]
00000506  98                cbw
00000507  8BD8              mov bx,ax
00000509  8A87F007          mov al,[bx+0x7f0]
0000050D  8846FD            mov [bp-0x3],al
00000510  A0AA37            mov al,[0x37aa]
00000513  98                cbw
00000514  8BD8              mov bx,ax
00000516  8A87F407          mov al,[bx+0x7f4]
0000051A  8846FC            mov [bp-0x4],al
0000051D  A0AA37            mov al,[0x37aa]
00000520  98                cbw
00000521  8BD8              mov bx,ax
00000523  8A87F807          mov al,[bx+0x7f8]
00000527  50                push ax
00000528  B000              mov al,0x0
0000052A  50                push ax
0000052B  8A46FD            mov al,[bp-0x3]
0000052E  50                push ax
0000052F  8A46FE            mov al,[bp-0x2]
00000532  50                push ax
00000533  8A46FF            mov al,[bp-0x1]
00000536  0246FD            add al,[bp-0x3]
00000539  50                push ax
0000053A  9ABB050F15        call 0x150f:0x5bb
0000053F  83C40A            add sp,byte +0xa
00000542  A0AA37            mov al,[0x37aa]
00000545  98                cbw
00000546  8BD8              mov bx,ax
00000548  8A87F807          mov al,[bx+0x7f8]
0000054C  50                push ax
0000054D  B000              mov al,0x0
0000054F  50                push ax
00000550  8A46FD            mov al,[bp-0x3]
00000553  50                push ax
00000554  8A46FE            mov al,[bp-0x2]
00000557  0246FC            add al,[bp-0x4]
0000055A  50                push ax
0000055B  8A46FF            mov al,[bp-0x1]
0000055E  0246FD            add al,[bp-0x3]
00000561  50                push ax
00000562  9ABB050F15        call 0x150f:0x5bb
00000567  83C40A            add sp,byte +0xa
0000056A  A0AA37            mov al,[0x37aa]
0000056D  98                cbw
0000056E  8BD8              mov bx,ax
00000570  8A87FC07          mov al,[bx+0x7fc]
00000574  50                push ax
00000575  8A46FC            mov al,[bp-0x4]
00000578  50                push ax
00000579  B000              mov al,0x0
0000057B  50                push ax
0000057C  8A46FE            mov al,[bp-0x2]
0000057F  0246FC            add al,[bp-0x4]
00000582  50                push ax
00000583  8A46FF            mov al,[bp-0x1]
00000586  0246FD            add al,[bp-0x3]
00000589  50                push ax
0000058A  9ABB050F15        call 0x150f:0x5bb
0000058F  83C40A            add sp,byte +0xa
00000592  A0AA37            mov al,[0x37aa]
00000595  98                cbw
00000596  8BD8              mov bx,ax
00000598  8A87FC07          mov al,[bx+0x7fc]
0000059C  50                push ax
0000059D  8A46FC            mov al,[bp-0x4]
000005A0  50                push ax
000005A1  B000              mov al,0x0
000005A3  50                push ax
000005A4  8A46FE            mov al,[bp-0x2]
000005A7  0246FC            add al,[bp-0x4]
000005AA  50                push ax
000005AB  8A46FF            mov al,[bp-0x1]
000005AE  50                push ax
000005AF  9ABB050F15        call 0x150f:0x5bb
000005B4  83C40A            add sp,byte +0xa
000005B7  8BE5              mov sp,bp
000005B9  5D                pop bp
000005BA  CB                retf

; ---- 150F:05BB ----
000005BB  55                push bp
000005BC  8BEC              mov bp,sp
000005BE  56                push si
000005BF  BE0100            mov si,0x1
000005C2  E98A00            jmp 0x64f
000005C5  8A460A            mov al,[bp+0xa]
000005C8  004606            add [bp+0x6],al
000005CB  8A460C            mov al,[bp+0xc]
000005CE  004608            add [bp+0x8],al
000005D1  8A4606            mov al,[bp+0x6]
000005D4  98                cbw
000005D5  D1E0              shl ax,1
000005D7  D1E0              shl ax,1
000005D9  D1E0              shl ax,1
000005DB  50                push ax
000005DC  8A4608            mov al,[bp+0x8]
000005DF  98                cbw
000005E0  5B                pop bx
000005E1  03D8              add bx,ax
000005E3  80BF784400        cmp byte [bx+0x4478],0x0
000005E8  7502              jnz 0x5ec
000005EA  EB62              jmp short 0x64e
000005EC  A0AA37            mov al,[0x37aa]
000005EF  50                push ax
000005F0  8A4608            mov al,[bp+0x8]
000005F3  50                push ax
000005F4  8A4606            mov al,[bp+0x6]
000005F7  50                push ax
000005F8  0E                push cs
000005F9  E80AFE            call 0x406
000005FC  83C406            add sp,byte +0x6
000005FF  3C08              cmp al,0x8
00000601  7402              jz 0x605
00000603  EB49              jmp short 0x64e
00000605  8A460E            mov al,[bp+0xe]
00000608  50                push ax
00000609  8A4608            mov al,[bp+0x8]
0000060C  50                push ax
0000060D  8A4606            mov al,[bp+0x6]
00000610  50                push ax
00000611  0E                push cs
00000612  E823FE            call 0x438
00000615  83C406            add sp,byte +0x6
00000618  FE06B44B          inc byte [0x4bb4]
0000061C  A0B54B            mov al,[0x4bb5]
0000061F  98                cbw
00000620  BA1E00            mov dx,0x1e
00000623  F7EA              imul dx
00000625  50                push ax
00000626  A0B44B            mov al,[0x4bb4]
00000629  98                cbw
0000062A  5B                pop bx
0000062B  03D8              add bx,ax
0000062D  8A4606            mov al,[bp+0x6]
00000630  8887D52D          mov [bx+0x2dd5],al
00000634  A0B54B            mov al,[0x4bb5]
00000637  98                cbw
00000638  BA1E00            mov dx,0x1e
0000063B  F7EA              imul dx
0000063D  50                push ax
0000063E  A0B44B            mov al,[0x4bb4]
00000641  98                cbw
00000642  5B                pop bx
00000643  03D8              add bx,ax
00000645  8A4608            mov al,[bp+0x8]
00000648  8887112E          mov [bx+0x2e11],al
0000064C  EB09              jmp short 0x657
0000064E  46                inc si
0000064F  83FE06            cmp si,byte +0x6
00000652  7F03              jg 0x657
00000654  E96EFF            jmp 0x5c5
00000657  5E                pop si
00000658  5D                pop bp
00000659  CB                retf

; ---- 150F:065A ----
0000065A  55                push bp
0000065B  8BEC              mov bp,sp
0000065D  4C                dec sp
0000065E  4C                dec sp
0000065F  C646FF00          mov byte [bp-0x1],0x0
00000663  EB75              jmp short 0x6da
00000665  8A46FF            mov al,[bp-0x1]
00000668  3A06AA37          cmp al,[0x37aa]
0000066C  7502              jnz 0x670
0000066E  EB67              jmp short 0x6d7
00000670  8A46FF            mov al,[bp-0x1]
00000673  98                cbw
00000674  D1E0              shl ax,1
00000676  D1E0              shl ax,1
00000678  50                push ax
00000679  A0AA37            mov al,[0x37aa]
0000067C  98                cbw
0000067D  5B                pop bx
0000067E  03D8              add bx,ax
00000680  80BFB33700        cmp byte [bx+0x37b3],0x0
00000685  7402              jz 0x689
00000687  EB4E              jmp short 0x6d7
00000689  A0AA37            mov al,[0x37aa]
0000068C  50                push ax
0000068D  8A46FF            mov al,[bp-0x1]
00000690  98                cbw
00000691  8BD8              mov bx,ax
00000693  8A874808          mov al,[bx+0x848]
00000697  B400              mov ah,0x0
00000699  48                dec ax
0000069A  BB0300            mov bx,0x3
0000069D  99                cwd
0000069E  F7FB              idiv bx
000006A0  50                push ax
000006A1  8A46FF            mov al,[bp-0x1]
000006A4  98                cbw
000006A5  8BD8              mov bx,ax
000006A7  8A874408          mov al,[bx+0x844]
000006AB  B400              mov ah,0x0
000006AD  48                dec ax
000006AE  BB0300            mov bx,0x3
000006B1  99                cwd
000006B2  F7FB              idiv bx
000006B4  50                push ax
000006B5  0E                push cs
000006B6  E84DFD            call 0x406
000006B9  83C406            add sp,byte +0x6
000006BC  3C08              cmp al,0x8
000006BE  7317              jnc 0x6d7
000006C0  8A46FF            mov al,[bp-0x1]
000006C3  98                cbw
000006C4  D1E0              shl ax,1
000006C6  D1E0              shl ax,1
000006C8  50                push ax
000006C9  A0AA37            mov al,[0x37aa]
000006CC  98                cbw
000006CD  5B                pop bx
000006CE  03D8              add bx,ax
000006D0  8A4606            mov al,[bp+0x6]
000006D3  8887B337          mov [bx+0x37b3],al
000006D7  FE46FF            inc byte [bp-0x1]
000006DA  807EFF03          cmp byte [bp-0x1],0x3
000006DE  7E85              jng 0x665
000006E0  8BE5              mov sp,bp
000006E2  5D                pop bp
000006E3  CB                retf

; ---- 150F:06E4 ----
000006E4  55                push bp
000006E5  8BEC              mov bp,sp
000006E7  C606AA3700        mov byte [0x37aa],0x0
000006EC  E9C700            jmp 0x7b6
000006EF  A0AA37            mov al,[0x37aa]
000006F2  98                cbw
000006F3  8BD8              mov bx,ax
000006F5  80BF400801        cmp byte [bx+0x840],0x1
000006FA  7403              jz 0x6ff
000006FC  E9B300            jmp 0x7b2
000006FF  A0AA37            mov al,[0x37aa]
00000702  98                cbw
00000703  8BD8              mov bx,ax
00000705  C687C33700        mov byte [bx+0x37c3],0x0
0000070A  A0AA37            mov al,[0x37aa]
0000070D  98                cbw
0000070E  8BD8              mov bx,ax
00000710  80BFB2430F        cmp byte [bx+0x43b2],0xf
00000715  7403              jz 0x71a
00000717  E99300            jmp 0x7ad
0000071A  A0AA37            mov al,[0x37aa]
0000071D  98                cbw
0000071E  8BD8              mov bx,ax
00000720  80BF5C4502        cmp byte [bx+0x455c],0x2
00000725  7444              jz 0x76b
00000727  A0AA37            mov al,[0x37aa]
0000072A  98                cbw
0000072B  8BD8              mov bx,ax
0000072D  80BFD24500        cmp byte [bx+0x45d2],0x0
00000732  750D              jnz 0x741
00000734  A0AA37            mov al,[0x37aa]
00000737  98                cbw
00000738  8BD8              mov bx,ax
0000073A  C6875C4502        mov byte [bx+0x455c],0x2
0000073F  EB28              jmp short 0x769
00000741  A0AA37            mov al,[0x37aa]
00000744  98                cbw
00000745  8A168834          mov dl,[0x3488]
00000749  8BD8              mov bx,ax
0000074B  8497C608          test [bx+0x8c6],dl
0000074F  740D              jz 0x75e
00000751  A0AA37            mov al,[0x37aa]
00000754  98                cbw
00000755  8BD8              mov bx,ax
00000757  C687C33710        mov byte [bx+0x37c3],0x10
0000075C  EB0B              jmp short 0x769
0000075E  A0AA37            mov al,[0x37aa]
00000761  98                cbw
00000762  8BD8              mov bx,ax
00000764  C687C33708        mov byte [bx+0x37c3],0x8
00000769  EB40              jmp short 0x7ab
0000076B  A0AA37            mov al,[0x37aa]
0000076E  98                cbw
0000076F  8A168834          mov dl,[0x3488]
00000773  8BD8              mov bx,ax
00000775  8497C608          test [bx+0x8c6],dl
00000779  740D              jz 0x788
0000077B  A0AA37            mov al,[0x37aa]
0000077E  98                cbw
0000077F  8BD8              mov bx,ax
00000781  C687C33704        mov byte [bx+0x37c3],0x4
00000786  EB23              jmp short 0x7ab
00000788  A11A3D            mov ax,[0x3d1a]
0000078B  BB3200            mov bx,0x32
0000078E  99                cwd
0000078F  F7FB              idiv bx
00000791  0BD2              or dx,dx
00000793  7402              jz 0x797
00000795  EB1B              jmp short 0x7b2
00000797  A0AA37            mov al,[0x37aa]
0000079A  98                cbw
0000079B  8BD8              mov bx,ax
0000079D  80BFD24500        cmp byte [bx+0x45d2],0x0
000007A2  7502              jnz 0x7a6
000007A4  EB0C              jmp short 0x7b2
000007A6  9AA0180F15        call 0x150f:0x18a0
000007AB  EB05              jmp short 0x7b2
000007AD  9AC2070F15        call 0x150f:0x7c2
000007B2  FE06AA37          inc byte [0x37aa]
000007B6  803EAA3704        cmp byte [0x37aa],0x4
000007BB  7D03              jnl 0x7c0
000007BD  E92FFF            jmp 0x6ef
000007C0  5D                pop bp
000007C1  CB                retf

; ---- 150F:07C2 ----
000007C2  55                push bp
000007C3  8BEC              mov bp,sp
000007C5  A0AA37            mov al,[0x37aa]
000007C8  98                cbw
000007C9  8BD8              mov bx,ax
000007CB  80BFC643FF        cmp byte [bx+0x43c6],0xff
000007D0  7407              jz 0x7d9
000007D2  9A44080F15        call 0x150f:0x844
000007D7  EB69              jmp short 0x842
000007D9  A0AA37            mov al,[0x37aa]
000007DC  98                cbw
000007DD  8BD8              mov bx,ax
000007DF  80BFC837FF        cmp byte [bx+0x37c8],0xff
000007E4  7407              jz 0x7ed
000007E6  9A64090F15        call 0x150f:0x964
000007EB  EB55              jmp short 0x842
000007ED  A0AA37            mov al,[0x37aa]
000007F0  98                cbw
000007F1  8BD8              mov bx,ax
000007F3  80BF8034FF        cmp byte [bx+0x3480],0xff
000007F8  7407              jz 0x801
000007FA  9ACA090F15        call 0x150f:0x9ca
000007FF  EB41              jmp short 0x842
00000801  A0AA37            mov al,[0x37aa]
00000804  98                cbw
00000805  8BD8              mov bx,ax
00000807  80BFC73DFF        cmp byte [bx+0x3dc7],0xff
0000080C  7407              jz 0x815
0000080E  9A600C0F15        call 0x150f:0xc60
00000813  EB2D              jmp short 0x842
00000815  A0AA37            mov al,[0x37aa]
00000818  98                cbw
00000819  8BD8              mov bx,ax
0000081B  80BFE448FF        cmp byte [bx+0x48e4],0xff
00000820  7407              jz 0x829
00000822  9AB40A0F15        call 0x150f:0xab4
00000827  EB19              jmp short 0x842
00000829  A0AA37            mov al,[0x37aa]
0000082C  98                cbw
0000082D  8BD8              mov bx,ax
0000082F  80BFAB3701        cmp byte [bx+0x37ab],0x1
00000834  7507              jnz 0x83d
00000836  9AD40F0F15        call 0x150f:0xfd4
0000083B  EB05              jmp short 0x842
0000083D  9A0B100F15        call 0x150f:0x100b
00000842  5D                pop bp
00000843  CB                retf

; ---- 150F:0844 ----
00000844  55                push bp
00000845  8BEC              mov bp,sp
00000847  83EC08            sub sp,byte +0x8
0000084A  A0AA37            mov al,[0x37aa]
0000084D  98                cbw
0000084E  8BD8              mov bx,ax
00000850  8A87C643          mov al,[bx+0x43c6]
00000854  8846FF            mov [bp-0x1],al
00000857  A0AA37            mov al,[0x37aa]
0000085A  98                cbw
0000085B  8BD8              mov bx,ax
0000085D  8A87CA43          mov al,[bx+0x43ca]
00000861  8846FE            mov [bp-0x2],al
00000864  8A46FF            mov al,[bp-0x1]
00000867  98                cbw
00000868  BA1A00            mov dx,0x1a
0000086B  F7EA              imul dx
0000086D  50                push ax
0000086E  8A46FE            mov al,[bp-0x2]
00000871  98                cbw
00000872  5B                pop bx
00000873  03D8              add bx,ax
00000875  8A87F448          mov al,[bx+0x48f4]
00000879  8846F9            mov [bp-0x7],al
0000087C  F646F901          test byte [bp-0x7],0x1
00000880  7416              jz 0x898
00000882  B001              mov al,0x1
00000884  50                push ax
00000885  8A46FE            mov al,[bp-0x2]
00000888  50                push ax
00000889  8A46FF            mov al,[bp-0x1]
0000088C  50                push ax
0000088D  9AF2150F15        call 0x150f:0x15f2
00000892  83C406            add sp,byte +0x6
00000895  E9BD00            jmp 0x955
00000898  A0AA37            mov al,[0x37aa]
0000089B  98                cbw
0000089C  8BD8              mov bx,ax
0000089E  80BF5C4501        cmp byte [bx+0x455c],0x1
000008A3  7503              jnz 0x8a8
000008A5  E9AD00            jmp 0x955
000008A8  A0AA37            mov al,[0x37aa]
000008AB  98                cbw
000008AC  D1E0              shl ax,1
000008AE  8BD8              mov bx,ax
000008B0  8B87E643          mov ax,[bx+0x43e6]
000008B4  BB0400            mov bx,0x4
000008B7  99                cwd
000008B8  F7FB              idiv bx
000008BA  D0E0              shl al,1
000008BC  8846FA            mov [bp-0x6],al
000008BF  C646FB00          mov byte [bp-0x5],0x0
000008C3  EB77              jmp short 0x93c
000008C5  A0AA37            mov al,[0x37aa]
000008C8  98                cbw
000008C9  D1E0              shl ax,1
000008CB  8BD8              mov bx,ax
000008CD  8A87E24B          mov al,[bx+0x4be2]
000008D1  50                push ax
000008D2  8A46FA            mov al,[bp-0x6]
000008D5  98                cbw
000008D6  8BD8              mov bx,ax
000008D8  58                pop ax
000008D9  02877808          add al,[bx+0x878]
000008DD  8846FD            mov [bp-0x3],al
000008E0  A0AA37            mov al,[0x37aa]
000008E3  98                cbw
000008E4  D1E0              shl ax,1
000008E6  8BD8              mov bx,ax
000008E8  8A87EA4B          mov al,[bx+0x4bea]
000008EC  50                push ax
000008ED  8A46FA            mov al,[bp-0x6]
000008F0  98                cbw
000008F1  8BD8              mov bx,ax
000008F3  58                pop ax
000008F4  02878008          add al,[bx+0x880]
000008F8  8846FC            mov [bp-0x4],al
000008FB  8A46FD            mov al,[bp-0x3]
000008FE  98                cbw
000008FF  BA1A00            mov dx,0x1a
00000902  F7EA              imul dx
00000904  50                push ax
00000905  8A46FC            mov al,[bp-0x4]
00000908  98                cbw
00000909  5B                pop bx
0000090A  03D8              add bx,ax
0000090C  8A87F448          mov al,[bx+0x48f4]
00000910  B400              mov ah,0x0
00000912  BB0800            mov bx,0x8
00000915  99                cwd
00000916  F7FB              idiv bx
00000918  8846F8            mov [bp-0x8],al
0000091B  807EF806          cmp byte [bp-0x8],0x6
0000091F  7408              jz 0x929
00000921  807EF80D          cmp byte [bp-0x8],0xd
00000925  7302              jnc 0x929
00000927  EB19              jmp short 0x942
00000929  8A46FA            mov al,[bp-0x6]
0000092C  0402              add al,0x2
0000092E  8846FA            mov [bp-0x6],al
00000931  3C07              cmp al,0x7
00000933  7E04              jng 0x939
00000935  C646FA00          mov byte [bp-0x6],0x0
00000939  FE46FB            inc byte [bp-0x5]
0000093C  807EFB03          cmp byte [bp-0x5],0x3
00000940  7E83              jng 0x8c5
00000942  B000              mov al,0x0
00000944  50                push ax
00000945  8A46FC            mov al,[bp-0x4]
00000948  50                push ax
00000949  8A46FD            mov al,[bp-0x3]
0000094C  50                push ax
0000094D  9AF2150F15        call 0x150f:0x15f2
00000952  83C406            add sp,byte +0x6
00000955  A0AA37            mov al,[0x37aa]
00000958  98                cbw
00000959  8BD8              mov bx,ax
0000095B  C687C643FF        mov byte [bx+0x43c6],0xff
00000960  8BE5              mov sp,bp
00000962  5D                pop bp
00000963  CB                retf

; ---- 150F:0964 ----
00000964  55                push bp
00000965  8BEC              mov bp,sp
00000967  A0AA37            mov al,[0x37aa]
0000096A  98                cbw
0000096B  D1E0              shl ax,1
0000096D  50                push ax
0000096E  A0AA37            mov al,[0x37aa]
00000971  98                cbw
00000972  8BD8              mov bx,ax
00000974  8A87C837          mov al,[bx+0x37c8]
00000978  98                cbw
00000979  5B                pop bx
0000097A  3987E643          cmp [bx+0x43e6],ax
0000097E  750D              jnz 0x98d
00000980  A0AA37            mov al,[0x37aa]
00000983  98                cbw
00000984  8BD8              mov bx,ax
00000986  C687C837FF        mov byte [bx+0x37c8],0xff
0000098B  EB3B              jmp short 0x9c8
0000098D  A0AA37            mov al,[0x37aa]
00000990  98                cbw
00000991  D1E0              shl ax,1
00000993  8BD8              mov bx,ax
00000995  8A87E643          mov al,[bx+0x43e6]
00000999  50                push ax
0000099A  A0AA37            mov al,[0x37aa]
0000099D  98                cbw
0000099E  8BD8              mov bx,ax
000009A0  58                pop ax
000009A1  2A87C837          sub al,[bx+0x37c8]
000009A5  8AD0              mov dl,al
000009A7  0AC0              or al,al
000009A9  7D06              jnl 0x9b1
000009AB  8AC2              mov al,dl
000009AD  0410              add al,0x10
000009AF  8AD0              mov dl,al
000009B1  80FA08            cmp dl,0x8
000009B4  7D04              jnl 0x9ba
000009B6  B002              mov al,0x2
000009B8  EB02              jmp short 0x9bc
000009BA  B001              mov al,0x1
000009BC  50                push ax
000009BD  A0AA37            mov al,[0x37aa]
000009C0  98                cbw
000009C1  8BD8              mov bx,ax
000009C3  58                pop ax
000009C4  8887C337          mov [bx+0x37c3],al
000009C8  5D                pop bp
000009C9  CB                retf

; ---- 150F:09CA ----
000009CA  55                push bp
000009CB  8BEC              mov bp,sp
000009CD  4C                dec sp
000009CE  4C                dec sp
000009CF  A0AA37            mov al,[0x37aa]
000009D2  98                cbw
000009D3  8BD8              mov bx,ax
000009D5  8A878034          mov al,[bx+0x3480]
000009D9  8846FF            mov [bp-0x1],al
000009DC  A0AA37            mov al,[0x37aa]
000009DF  98                cbw
000009E0  8BD8              mov bx,ax
000009E2  8A878434          mov al,[bx+0x3484]
000009E6  8846FE            mov [bp-0x2],al
000009E9  A0AA37            mov al,[0x37aa]
000009EC  98                cbw
000009ED  8BD8              mov bx,ax
000009EF  80BFC04501        cmp byte [bx+0x45c0],0x1
000009F4  7403              jz 0x9f9
000009F6  E98100            jmp 0xa7a
000009F9  A0AA37            mov al,[0x37aa]
000009FC  98                cbw
000009FD  8BD8              mov bx,ax
000009FF  80BFCD3C01        cmp byte [bx+0x3ccd],0x1
00000A04  752F              jnz 0xa35
00000A06  8A46FF            mov al,[bp-0x1]
00000A09  98                cbw
00000A0A  BA1A00            mov dx,0x1a
00000A0D  F7EA              imul dx
00000A0F  50                push ax
00000A10  8A46FE            mov al,[bp-0x2]
00000A13  98                cbw
00000A14  5B                pop bx
00000A15  03D8              add bx,ax
00000A17  8A87F448          mov al,[bx+0x48f4]
00000A1B  B400              mov ah,0x0
00000A1D  BB0800            mov bx,0x8
00000A20  99                cwd
00000A21  F7FB              idiv bx
00000A23  0BC0              or ax,ax
00000A25  7407              jz 0xa2e
00000A27  9AB60B0F15        call 0x150f:0xbb6
00000A2C  EB05              jmp short 0xa33
00000A2E  9A29160F15        call 0x150f:0x1629
00000A33  EB43              jmp short 0xa78
00000A35  A0AA37            mov al,[0x37aa]
00000A38  98                cbw
00000A39  D1E0              shl ax,1
00000A3B  50                push ax
00000A3C  8A46FF            mov al,[bp-0x1]
00000A3F  98                cbw
00000A40  5B                pop bx
00000A41  3987E24B          cmp [bx+0x4be2],ax
00000A45  7512              jnz 0xa59
00000A47  A0AA37            mov al,[0x37aa]
00000A4A  98                cbw
00000A4B  D1E0              shl ax,1
00000A4D  50                push ax
00000A4E  8A46FE            mov al,[bp-0x2]
00000A51  98                cbw
00000A52  5B                pop bx
00000A53  3987EA4B          cmp [bx+0x4bea],ax
00000A57  741A              jz 0xa73
00000A59  A0AA37            mov al,[0x37aa]
00000A5C  98                cbw
00000A5D  8BD8              mov bx,ax
00000A5F  8A87C337          mov al,[bx+0x37c3]
00000A63  0C04              or al,0x4
00000A65  50                push ax
00000A66  A0AA37            mov al,[0x37aa]
00000A69  98                cbw
00000A6A  8BD8              mov bx,ax
00000A6C  58                pop ax
00000A6D  8887C337          mov [bx+0x37c3],al
00000A71  EB05              jmp short 0xa78
00000A73  9A29160F15        call 0x150f:0x1629
00000A78  EB36              jmp short 0xab0
00000A7A  8A46FE            mov al,[bp-0x2]
00000A7D  98                cbw
00000A7E  50                push ax
00000A7F  8A46FF            mov al,[bp-0x1]
00000A82  98                cbw
00000A83  50                push ax
00000A84  A0AA37            mov al,[0x37aa]
00000A87  98                cbw
00000A88  D1E0              shl ax,1
00000A8A  8BD8              mov bx,ax
00000A8C  FFB7EA4B          push word [bx+0x4bea]
00000A90  A0AA37            mov al,[0x37aa]
00000A93  98                cbw
00000A94  D1E0              shl ax,1
00000A96  8BD8              mov bx,ax
00000A98  FFB7E24B          push word [bx+0x4be2]
00000A9C  9ACF120F15        call 0x150f:0x12cf
00000AA1  83C408            add sp,byte +0x8
00000AA4  50                push ax
00000AA5  A0AA37            mov al,[0x37aa]
00000AA8  98                cbw
00000AA9  8BD8              mov bx,ax
00000AAB  58                pop ax
00000AAC  8887C837          mov [bx+0x37c8],al
00000AB0  8BE5              mov sp,bp
00000AB2  5D                pop bp
00000AB3  CB                retf

; ---- 150F:0AB4 ----
00000AB4  55                push bp
00000AB5  8BEC              mov bp,sp
00000AB7  4C                dec sp
00000AB8  4C                dec sp
00000AB9  A0AA37            mov al,[0x37aa]
00000ABC  98                cbw
00000ABD  8BD8              mov bx,ax
00000ABF  8A87E448          mov al,[bx+0x48e4]
00000AC3  8846FF            mov [bp-0x1],al
00000AC6  A0AA37            mov al,[0x37aa]
00000AC9  98                cbw
00000ACA  8BD8              mov bx,ax
00000ACC  8A87E848          mov al,[bx+0x48e8]
00000AD0  8846FE            mov [bp-0x2],al
00000AD3  A0AA37            mov al,[0x37aa]
00000AD6  98                cbw
00000AD7  8BD8              mov bx,ax
00000AD9  80BFC04501        cmp byte [bx+0x45c0],0x1
00000ADE  7403              jz 0xae3
00000AE0  E99900            jmp 0xb7c
00000AE3  A0AA37            mov al,[0x37aa]
00000AE6  98                cbw
00000AE7  8BD8              mov bx,ax
00000AE9  80BFA44B01        cmp byte [bx+0x4ba4],0x1
00000AEE  7547              jnz 0xb37
00000AF0  8A46FF            mov al,[bp-0x1]
00000AF3  98                cbw
00000AF4  BA1A00            mov dx,0x1a
00000AF7  F7EA              imul dx
00000AF9  50                push ax
00000AFA  8A46FE            mov al,[bp-0x2]
00000AFD  98                cbw
00000AFE  5B                pop bx
00000AFF  03D8              add bx,ax
00000B01  8A87F448          mov al,[bx+0x48f4]
00000B05  B400              mov ah,0x0
00000B07  BB0800            mov bx,0x8
00000B0A  99                cwd
00000B0B  F7FB              idiv bx
00000B0D  0BC0              or ax,ax
00000B0F  741F              jz 0xb30
00000B11  A0AA37            mov al,[0x37aa]
00000B14  98                cbw
00000B15  8BD8              mov bx,ax
00000B17  8A87C337          mov al,[bx+0x37c3]
00000B1B  0C04              or al,0x4
00000B1D  50                push ax
00000B1E  A0AA37            mov al,[0x37aa]
00000B21  98                cbw
00000B22  8BD8              mov bx,ax
00000B24  58                pop ax
00000B25  8887C337          mov [bx+0x37c3],al
00000B29  9AB60B0F15        call 0x150f:0xbb6
00000B2E  EB05              jmp short 0xb35
00000B30  9AE2150F15        call 0x150f:0x15e2
00000B35  EB43              jmp short 0xb7a
00000B37  A0AA37            mov al,[0x37aa]
00000B3A  98                cbw
00000B3B  D1E0              shl ax,1
00000B3D  50                push ax
00000B3E  8A46FF            mov al,[bp-0x1]
00000B41  98                cbw
00000B42  5B                pop bx
00000B43  3987E24B          cmp [bx+0x4be2],ax
00000B47  7512              jnz 0xb5b
00000B49  A0AA37            mov al,[0x37aa]
00000B4C  98                cbw
00000B4D  D1E0              shl ax,1
00000B4F  50                push ax
00000B50  8A46FE            mov al,[bp-0x2]
00000B53  98                cbw
00000B54  5B                pop bx
00000B55  3987EA4B          cmp [bx+0x4bea],ax
00000B59  741A              jz 0xb75
00000B5B  A0AA37            mov al,[0x37aa]
00000B5E  98                cbw
00000B5F  8BD8              mov bx,ax
00000B61  8A87C337          mov al,[bx+0x37c3]
00000B65  0C04              or al,0x4
00000B67  50                push ax
00000B68  A0AA37            mov al,[0x37aa]
00000B6B  98                cbw
00000B6C  8BD8              mov bx,ax
00000B6E  58                pop ax
00000B6F  8887C337          mov [bx+0x37c3],al
00000B73  EB05              jmp short 0xb7a
00000B75  9AE2150F15        call 0x150f:0x15e2
00000B7A  EB36              jmp short 0xbb2
00000B7C  8A46FE            mov al,[bp-0x2]
00000B7F  98                cbw
00000B80  50                push ax
00000B81  8A46FF            mov al,[bp-0x1]
00000B84  98                cbw
00000B85  50                push ax
00000B86  A0AA37            mov al,[0x37aa]
00000B89  98                cbw
00000B8A  D1E0              shl ax,1
00000B8C  8BD8              mov bx,ax
00000B8E  FFB7EA4B          push word [bx+0x4bea]
00000B92  A0AA37            mov al,[0x37aa]
00000B95  98                cbw
00000B96  D1E0              shl ax,1
00000B98  8BD8              mov bx,ax
00000B9A  FFB7E24B          push word [bx+0x4be2]
00000B9E  9ACF120F15        call 0x150f:0x12cf
00000BA3  83C408            add sp,byte +0x8
00000BA6  50                push ax
00000BA7  A0AA37            mov al,[0x37aa]
00000BAA  98                cbw
00000BAB  8BD8              mov bx,ax
00000BAD  58                pop ax
00000BAE  8887C837          mov [bx+0x37c8],al
00000BB2  8BE5              mov sp,bp
00000BB4  5D                pop bp
00000BB5  CB                retf

; ---- 150F:0BB6 ----
00000BB6  55                push bp
00000BB7  8BEC              mov bp,sp
00000BB9  A0AA37            mov al,[0x37aa]
00000BBC  98                cbw
00000BBD  D1E0              shl ax,1
00000BBF  8BD8              mov bx,ax
00000BC1  83BFA82E00        cmp word [bx+0x2ea8],byte +0x0
00000BC6  7541              jnz 0xc09
00000BC8  A0AA37            mov al,[0x37aa]
00000BCB  98                cbw
00000BCC  D1E0              shl ax,1
00000BCE  8BD8              mov bx,ax
00000BD0  83BFE83C00        cmp word [bx+0x3ce8],byte +0x0
00000BD5  7532              jnz 0xc09
00000BD7  A0AA37            mov al,[0x37aa]
00000BDA  98                cbw
00000BDB  D1E0              shl ax,1
00000BDD  D1E0              shl ax,1
00000BDF  8BD8              mov bx,ax
00000BE1  83BF9A3C00        cmp word [bx+0x3c9a],byte +0x0
00000BE6  7721              ja 0xc09
00000BE8  7207              jc 0xbf1
00000BEA  83BF983C01        cmp word [bx+0x3c98],byte +0x1
00000BEF  7318              jnc 0xc09
00000BF1  A0AA37            mov al,[0x37aa]
00000BF4  98                cbw
00000BF5  8BD8              mov bx,ax
00000BF7  8A87C337          mov al,[bx+0x37c3]
00000BFB  0C10              or al,0x10
00000BFD  50                push ax
00000BFE  A0AA37            mov al,[0x37aa]
00000C01  98                cbw
00000C02  8BD8              mov bx,ax
00000C04  58                pop ax
00000C05  8887C337          mov [bx+0x37c3],al
00000C09  5D                pop bp
00000C0A  CB                retf

; ---- 150F:0C0B ----
00000C0B  55                push bp
00000C0C  8BEC              mov bp,sp
00000C0E  A0AA37            mov al,[0x37aa]
00000C11  98                cbw
00000C12  D1E0              shl ax,1
00000C14  8BD8              mov bx,ax
00000C16  83BFA82E00        cmp word [bx+0x2ea8],byte +0x0
00000C1B  7541              jnz 0xc5e
00000C1D  A0AA37            mov al,[0x37aa]
00000C20  98                cbw
00000C21  D1E0              shl ax,1
00000C23  8BD8              mov bx,ax
00000C25  83BFE83C00        cmp word [bx+0x3ce8],byte +0x0
00000C2A  7532              jnz 0xc5e
00000C2C  A0AA37            mov al,[0x37aa]
00000C2F  98                cbw
00000C30  D1E0              shl ax,1
00000C32  D1E0              shl ax,1
00000C34  8BD8              mov bx,ax
00000C36  83BF9A3C00        cmp word [bx+0x3c9a],byte +0x0
00000C3B  7721              ja 0xc5e
00000C3D  7207              jc 0xc46
00000C3F  83BF983C19        cmp word [bx+0x3c98],byte +0x19
00000C44  7318              jnc 0xc5e
00000C46  A0AA37            mov al,[0x37aa]
00000C49  98                cbw
00000C4A  8BD8              mov bx,ax
00000C4C  8A87C337          mov al,[bx+0x37c3]
00000C50  0C10              or al,0x10
00000C52  50                push ax
00000C53  A0AA37            mov al,[0x37aa]
00000C56  98                cbw
00000C57  8BD8              mov bx,ax
00000C59  58                pop ax
00000C5A  8887C337          mov [bx+0x37c3],al
00000C5E  5D                pop bp
00000C5F  CB                retf

; ---- 150F:0C60 ----
00000C60  55                push bp
00000C61  8BEC              mov bp,sp
00000C63  A0AA37            mov al,[0x37aa]
00000C66  98                cbw
00000C67  8BD8              mov bx,ax
00000C69  80BF5C4501        cmp byte [bx+0x455c],0x1
00000C6E  7507              jnz 0xc77
00000C70  9AEC0D0F15        call 0x150f:0xdec
00000C75  EB05              jmp short 0xc7c
00000C77  9A7E0C0F15        call 0x150f:0xc7e
00000C7C  5D                pop bp
00000C7D  CB                retf

; ---- 150F:0C7E ----
00000C7E  55                push bp
00000C7F  8BEC              mov bp,sp
00000C81  4C                dec sp
00000C82  4C                dec sp
00000C83  56                push si
00000C84  A0AA37            mov al,[0x37aa]
00000C87  98                cbw
00000C88  8BD8              mov bx,ax
00000C8A  8A87C73D          mov al,[bx+0x3dc7]
00000C8E  8846FE            mov [bp-0x2],al
00000C91  A0AA37            mov al,[0x37aa]
00000C94  98                cbw
00000C95  D1E0              shl ax,1
00000C97  D1E0              shl ax,1
00000C99  8BD8              mov bx,ax
00000C9B  8BB76E37          mov si,[bx+0x376e]
00000C9F  8A46FE            mov al,[bp-0x2]
00000CA2  98                cbw
00000CA3  8BD8              mov bx,ax
00000CA5  8A87E740          mov al,[bx+0x40e7]
00000CA9  98                cbw
00000CAA  50                push ax
00000CAB  A0AA37            mov al,[0x37aa]
00000CAE  98                cbw
00000CAF  8BD8              mov bx,ax
00000CB1  8A87E740          mov al,[bx+0x40e7]
00000CB5  98                cbw
00000CB6  40                inc ax
00000CB7  5A                pop dx
00000CB8  3BD0              cmp dx,ax
00000CBA  7E12              jng 0xcce
00000CBC  0BF6              or si,si
00000CBE  7406              jz 0xcc6
00000CC0  0E                push cs
00000CC1  E847FF            call 0xc0b
00000CC4  EB05              jmp short 0xccb
00000CC6  9ACC0D0F15        call 0x150f:0xdcc
00000CCB  E9F900            jmp 0xdc7
00000CCE  8A46FE            mov al,[bp-0x2]
00000CD1  98                cbw
00000CD2  8BD8              mov bx,ax
00000CD4  80BFE74004        cmp byte [bx+0x40e7],0x4
00000CD9  7C0B              jl 0xce6
00000CDB  0BF6              or si,si
00000CDD  7407              jz 0xce6
00000CDF  0E                push cs
00000CE0  E828FF            call 0xc0b
00000CE3  E9E100            jmp 0xdc7
00000CE6  A0AA37            mov al,[0x37aa]
00000CE9  98                cbw
00000CEA  8BD8              mov bx,ax
00000CEC  8A870008          mov al,[bx+0x800]
00000CF0  FEC8              dec al
00000CF2  50                push ax
00000CF3  A0AA37            mov al,[0x37aa]
00000CF6  98                cbw
00000CF7  8BD8              mov bx,ax
00000CF9  58                pop ax
00000CFA  88870008          mov [bx+0x800],al
00000CFE  0AC0              or al,al
00000D00  7E03              jng 0xd05
00000D02  E9BE00            jmp 0xdc3
00000D05  A0AA37            mov al,[0x37aa]
00000D08  98                cbw
00000D09  8BD8              mov bx,ax
00000D0B  8A87BA4B          mov al,[bx+0x4bba]
00000D0F  8846FF            mov [bp-0x1],al
00000D12  A0AA37            mov al,[0x37aa]
00000D15  98                cbw
00000D16  8BD8              mov bx,ax
00000D18  C687BA4BFF        mov byte [bx+0x4bba],0xff
00000D1D  807EFF00          cmp byte [bp-0x1],0x0
00000D21  750E              jnz 0xd31
00000D23  B80200            mov ax,0x2
00000D26  50                push ax
00000D27  9ABA01FD21        call 0x21fd:0x1ba
00000D2C  59                pop cx
00000D2D  0BC0              or ax,ax
00000D2F  740D              jz 0xd3e
00000D31  8A46FE            mov al,[bp-0x2]
00000D34  98                cbw
00000D35  8BD8              mov bx,ax
00000D37  80BFB2430F        cmp byte [bx+0x43b2],0xf
00000D3C  7507              jnz 0xd45
00000D3E  9ACC0D0F15        call 0x150f:0xdcc
00000D43  EB7C              jmp short 0xdc1
00000D45  8A46FE            mov al,[bp-0x2]
00000D48  98                cbw
00000D49  D1E0              shl ax,1
00000D4B  8BD8              mov bx,ax
00000D4D  8B876437          mov ax,[bx+0x3764]
00000D51  BB0A00            mov bx,0xa
00000D54  99                cwd
00000D55  F7FB              idiv bx
00000D57  50                push ax
00000D58  8A46FE            mov al,[bp-0x2]
00000D5B  98                cbw
00000D5C  D1E0              shl ax,1
00000D5E  8BD8              mov bx,ax
00000D60  8B875C37          mov ax,[bx+0x375c]
00000D64  BB0A00            mov bx,0xa
00000D67  99                cwd
00000D68  F7FB              idiv bx
00000D6A  50                push ax
00000D6B  A0AA37            mov al,[0x37aa]
00000D6E  98                cbw
00000D6F  D1E0              shl ax,1
00000D71  8BD8              mov bx,ax
00000D73  8B876437          mov ax,[bx+0x3764]
00000D77  BB0A00            mov bx,0xa
00000D7A  99                cwd
00000D7B  F7FB              idiv bx
00000D7D  50                push ax
00000D7E  A0AA37            mov al,[0x37aa]
00000D81  98                cbw
00000D82  D1E0              shl ax,1
00000D84  8BD8              mov bx,ax
00000D86  8B875C37          mov ax,[bx+0x375c]
00000D8A  BB0A00            mov bx,0xa
00000D8D  99                cwd
00000D8E  F7FB              idiv bx
00000D90  50                push ax
00000D91  9ACF120F15        call 0x150f:0x12cf
00000D96  83C408            add sp,byte +0x8
00000D99  50                push ax
00000D9A  A0AA37            mov al,[0x37aa]
00000D9D  98                cbw
00000D9E  8BD8              mov bx,ax
00000DA0  58                pop ax
00000DA1  8887C837          mov [bx+0x37c8],al
00000DA5  3CFF              cmp al,0xff
00000DA7  740D              jz 0xdb6
00000DA9  A0AA37            mov al,[0x37aa]
00000DAC  98                cbw
00000DAD  8BD8              mov bx,ax
00000DAF  C687000801        mov byte [bx+0x800],0x1
00000DB4  EB0B              jmp short 0xdc1
00000DB6  A0AA37            mov al,[0x37aa]
00000DB9  98                cbw
00000DBA  8BD8              mov bx,ax
00000DBC  C68700080A        mov byte [bx+0x800],0xa
00000DC1  EB04              jmp short 0xdc7
00000DC3  0E                push cs
00000DC4  E8EFFD            call 0xbb6
00000DC7  5E                pop si
00000DC8  8BE5              mov sp,bp
00000DCA  5D                pop bp
00000DCB  CB                retf

; ---- 150F:0DCC ----
00000DCC  55                push bp
00000DCD  8BEC              mov bp,sp
00000DCF  9A29160F15        call 0x150f:0x1629
00000DD4  A0AA37            mov al,[0x37aa]
00000DD7  98                cbw
00000DD8  8BD8              mov bx,ax
00000DDA  C687C837FF        mov byte [bx+0x37c8],0xff
00000DDF  A0AA37            mov al,[0x37aa]
00000DE2  98                cbw
00000DE3  8BD8              mov bx,ax
00000DE5  C687C73DFF        mov byte [bx+0x3dc7],0xff
00000DEA  5D                pop bp
00000DEB  CB                retf

; ---- 150F:0DEC ----
00000DEC  55                push bp
00000DED  8BEC              mov bp,sp
00000DEF  4C                dec sp
00000DF0  4C                dec sp
00000DF1  56                push si
00000DF2  A0AA37            mov al,[0x37aa]
00000DF5  98                cbw
00000DF6  8BD8              mov bx,ax
00000DF8  8A87C73D          mov al,[bx+0x3dc7]
00000DFC  8846FF            mov [bp-0x1],al
00000DFF  A0AA37            mov al,[0x37aa]
00000E02  98                cbw
00000E03  D1E0              shl ax,1
00000E05  D1E0              shl ax,1
00000E07  8BD8              mov bx,ax
00000E09  8BB76E37          mov si,[bx+0x376e]
00000E0D  8A46FF            mov al,[bp-0x1]
00000E10  98                cbw
00000E11  8BD8              mov bx,ax
00000E13  8A87143D          mov al,[bx+0x3d14]
00000E17  3A06AA37          cmp al,[0x37aa]
00000E1B  7405              jz 0xe22
00000E1D  9AA30F0F15        call 0x150f:0xfa3
00000E22  8A46FF            mov al,[bp-0x1]
00000E25  98                cbw
00000E26  8BD8              mov bx,ax
00000E28  8A87E740          mov al,[bx+0x40e7]
00000E2C  98                cbw
00000E2D  50                push ax
00000E2E  A0AA37            mov al,[0x37aa]
00000E31  98                cbw
00000E32  8BD8              mov bx,ax
00000E34  8A87E740          mov al,[bx+0x40e7]
00000E38  98                cbw
00000E39  40                inc ax
00000E3A  5A                pop dx
00000E3B  3BD0              cmp dx,ax
00000E3D  7E12              jng 0xe51
00000E3F  0BF6              or si,si
00000E41  7406              jz 0xe49
00000E43  0E                push cs
00000E44  E8C4FD            call 0xc0b
00000E47  EB05              jmp short 0xe4e
00000E49  9AA30F0F15        call 0x150f:0xfa3
00000E4E  E94D01            jmp 0xf9e
00000E51  A0AA37            mov al,[0x37aa]
00000E54  98                cbw
00000E55  8BD8              mov bx,ax
00000E57  80BF423D00        cmp byte [bx+0x3d42],0x0
00000E5C  750B              jnz 0xe69
00000E5E  0BF6              or si,si
00000E60  7407              jz 0xe69
00000E62  0E                push cs
00000E63  E8A5FD            call 0xc0b
00000E66  E93501            jmp 0xf9e
00000E69  A0AA37            mov al,[0x37aa]
00000E6C  98                cbw
00000E6D  8BD8              mov bx,ax
00000E6F  8A870008          mov al,[bx+0x800]
00000E73  FEC8              dec al
00000E75  50                push ax
00000E76  A0AA37            mov al,[0x37aa]
00000E79  98                cbw
00000E7A  8BD8              mov bx,ax
00000E7C  58                pop ax
00000E7D  88870008          mov [bx+0x800],al
00000E81  0AC0              or al,al
00000E83  7E03              jng 0xe88
00000E85  E9E000            jmp 0xf68
00000E88  8A46FF            mov al,[bp-0x1]
00000E8B  98                cbw
00000E8C  D1E0              shl ax,1
00000E8E  8BD8              mov bx,ax
00000E90  FFB76437          push word [bx+0x3764]
00000E94  8A46FF            mov al,[bp-0x1]
00000E97  98                cbw
00000E98  D1E0              shl ax,1
00000E9A  8BD8              mov bx,ax
00000E9C  FFB75C37          push word [bx+0x375c]
00000EA0  9A44160F15        call 0x150f:0x1644
00000EA5  59                pop cx
00000EA6  59                pop cx
00000EA7  3C01              cmp al,0x1
00000EA9  7511              jnz 0xebc
00000EAB  A0AA37            mov al,[0x37aa]
00000EAE  98                cbw
00000EAF  8BD8              mov bx,ax
00000EB1  80BFBA4B00        cmp byte [bx+0x4bba],0x0
00000EB6  7404              jz 0xebc
00000EB8  B001              mov al,0x1
00000EBA  EB02              jmp short 0xebe
00000EBC  B000              mov al,0x0
00000EBE  50                push ax
00000EBF  A0AA37            mov al,[0x37aa]
00000EC2  98                cbw
00000EC3  8BD8              mov bx,ax
00000EC5  58                pop ax
00000EC6  8887D02D          mov [bx+0x2dd0],al
00000ECA  A0AA37            mov al,[0x37aa]
00000ECD  98                cbw
00000ECE  8BD8              mov bx,ax
00000ED0  8A87D02D          mov al,[bx+0x2dd0]
00000ED4  B400              mov ah,0x0
00000ED6  F7D8              neg ax
00000ED8  1BC0              sbb ax,ax
00000EDA  40                inc ax
00000EDB  50                push ax
00000EDC  A0AA37            mov al,[0x37aa]
00000EDF  98                cbw
00000EE0  8BD8              mov bx,ax
00000EE2  58                pop ax
00000EE3  8887CC2D          mov [bx+0x2dcc],al
00000EE7  8A46FF            mov al,[bp-0x1]
00000EEA  98                cbw
00000EEB  D1E0              shl ax,1
00000EED  8BD8              mov bx,ax
00000EEF  8B876437          mov ax,[bx+0x3764]
00000EF3  BB0A00            mov bx,0xa
00000EF6  99                cwd
00000EF7  F7FB              idiv bx
00000EF9  50                push ax
00000EFA  8A46FF            mov al,[bp-0x1]
00000EFD  98                cbw
00000EFE  D1E0              shl ax,1
00000F00  8BD8              mov bx,ax
00000F02  8B875C37          mov ax,[bx+0x375c]
00000F06  BB0A00            mov bx,0xa
00000F09  99                cwd
00000F0A  F7FB              idiv bx
00000F0C  50                push ax
00000F0D  A0AA37            mov al,[0x37aa]
00000F10  98                cbw
00000F11  D1E0              shl ax,1
00000F13  8BD8              mov bx,ax
00000F15  8B876437          mov ax,[bx+0x3764]
00000F19  BB0A00            mov bx,0xa
00000F1C  99                cwd
00000F1D  F7FB              idiv bx
00000F1F  50                push ax
00000F20  A0AA37            mov al,[0x37aa]
00000F23  98                cbw
00000F24  D1E0              shl ax,1
00000F26  8BD8              mov bx,ax
00000F28  8B875C37          mov ax,[bx+0x375c]
00000F2C  BB0A00            mov bx,0xa
00000F2F  99                cwd
00000F30  F7FB              idiv bx
00000F32  50                push ax
00000F33  9ACF120F15        call 0x150f:0x12cf
00000F38  83C408            add sp,byte +0x8
00000F3B  50                push ax
00000F3C  A0AA37            mov al,[0x37aa]
00000F3F  98                cbw
00000F40  8BD8              mov bx,ax
00000F42  58                pop ax
00000F43  8887C837          mov [bx+0x37c8],al
00000F47  A0AA37            mov al,[0x37aa]
00000F4A  98                cbw
00000F4B  8BD8              mov bx,ax
00000F4D  80BFC837FF        cmp byte [bx+0x37c8],0xff
00000F52  7504              jnz 0xf58
00000F54  B00A              mov al,0xa
00000F56  EB02              jmp short 0xf5a
00000F58  B001              mov al,0x1
00000F5A  50                push ax
00000F5B  A0AA37            mov al,[0x37aa]
00000F5E  98                cbw
00000F5F  8BD8              mov bx,ax
00000F61  58                pop ax
00000F62  88870008          mov [bx+0x800],al
00000F66  EB36              jmp short 0xf9e
00000F68  A0AA37            mov al,[0x37aa]
00000F6B  98                cbw
00000F6C  8BD8              mov bx,ax
00000F6E  80BFD02D00        cmp byte [bx+0x2dd0],0x0
00000F73  7404              jz 0xf79
00000F75  0E                push cs
00000F76  E83DFC            call 0xbb6
00000F79  A0AA37            mov al,[0x37aa]
00000F7C  98                cbw
00000F7D  8BD8              mov bx,ax
00000F7F  80BFCC2D00        cmp byte [bx+0x2dcc],0x0
00000F84  7418              jz 0xf9e
00000F86  A0AA37            mov al,[0x37aa]
00000F89  98                cbw
00000F8A  8BD8              mov bx,ax
00000F8C  8A87C337          mov al,[bx+0x37c3]
00000F90  0C04              or al,0x4
00000F92  50                push ax
00000F93  A0AA37            mov al,[0x37aa]
00000F96  98                cbw
00000F97  8BD8              mov bx,ax
00000F99  58                pop ax
00000F9A  8887C337          mov [bx+0x37c3],al
00000F9E  5E                pop si
00000F9F  8BE5              mov sp,bp
00000FA1  5D                pop bp
00000FA2  CB                retf

; ---- 150F:0FA3 ----
00000FA3  55                push bp
00000FA4  8BEC              mov bp,sp
00000FA6  A0AA37            mov al,[0x37aa]
00000FA9  98                cbw
00000FAA  8BD8              mov bx,ax
00000FAC  C6875C4500        mov byte [bx+0x455c],0x0
00000FB1  A0AA37            mov al,[0x37aa]
00000FB4  98                cbw
00000FB5  8BD8              mov bx,ax
00000FB7  C687E23CFF        mov byte [bx+0x3ce2],0xff
00000FBC  A0AA37            mov al,[0x37aa]
00000FBF  98                cbw
00000FC0  8BD8              mov bx,ax
00000FC2  C687C73DFF        mov byte [bx+0x3dc7],0xff
00000FC7  A0AA37            mov al,[0x37aa]
00000FCA  98                cbw
00000FCB  8BD8              mov bx,ax
00000FCD  C687BA4BFF        mov byte [bx+0x4bba],0xff
00000FD2  5D                pop bp
00000FD3  CB                retf

; ---- 150F:0FD4 ----
00000FD4  55                push bp
00000FD5  8BEC              mov bp,sp
00000FD7  A0AA37            mov al,[0x37aa]
00000FDA  98                cbw
00000FDB  D1E0              shl ax,1
00000FDD  D1E0              shl ax,1
00000FDF  8BD8              mov bx,ax
00000FE1  83BF9A3C00        cmp word [bx+0x3c9a],byte +0x0
00000FE6  7716              ja 0xffe
00000FE8  7207              jc 0xff1
00000FEA  83BF983C32        cmp word [bx+0x3c98],byte +0x32
00000FEF  730D              jnc 0xffe
00000FF1  A0AA37            mov al,[0x37aa]
00000FF4  98                cbw
00000FF5  8BD8              mov bx,ax
00000FF7  C687C33710        mov byte [bx+0x37c3],0x10
00000FFC  EB0B              jmp short 0x1009
00000FFE  A0AA37            mov al,[0x37aa]
00001001  98                cbw
00001002  8BD8              mov bx,ax
00001004  C687AB3700        mov byte [bx+0x37ab],0x0
00001009  5D                pop bp
0000100A  CB                retf

; ---- 150F:100B ----
0000100B  55                push bp
0000100C  8BEC              mov bp,sp
0000100E  83EC06            sub sp,byte +0x6
00001011  A0AA37            mov al,[0x37aa]
00001014  98                cbw
00001015  D1E0              shl ax,1
00001017  8BD8              mov bx,ax
00001019  8B87E24B          mov ax,[bx+0x4be2]
0000101D  48                dec ax
0000101E  BB0300            mov bx,0x3
00001021  99                cwd
00001022  F7FB              idiv bx
00001024  8846FF            mov [bp-0x1],al
00001027  A0AA37            mov al,[0x37aa]
0000102A  98                cbw
0000102B  D1E0              shl ax,1
0000102D  8BD8              mov bx,ax
0000102F  8B87EA4B          mov ax,[bx+0x4bea]
00001033  48                dec ax
00001034  BB0300            mov bx,0x3
00001037  99                cwd
00001038  F7FB              idiv bx
0000103A  8846FE            mov [bp-0x2],al
0000103D  8A46FE            mov al,[bp-0x2]
00001040  50                push ax
00001041  8A46FF            mov al,[bp-0x1]
00001044  50                push ax
00001045  9AC6130F15        call 0x150f:0x13c6
0000104A  59                pop cx
0000104B  59                pop cx
0000104C  8846FA            mov [bp-0x6],al
0000104F  A0AA37            mov al,[0x37aa]
00001052  98                cbw
00001053  8BD8              mov bx,ax
00001055  8A87E23C          mov al,[bx+0x3ce2]
00001059  8846FB            mov [bp-0x5],al
0000105C  807EFBFF          cmp byte [bp-0x5],0xff
00001060  7403              jz 0x1065
00001062  E9C900            jmp 0x112e
00001065  8A46FA            mov al,[bp-0x6]
00001068  3A06AA37          cmp al,[0x37aa]
0000106C  7403              jz 0x1071
0000106E  E9A500            jmp 0x1116
00001071  A0AA37            mov al,[0x37aa]
00001074  98                cbw
00001075  8BD8              mov bx,ax
00001077  80BF5C4502        cmp byte [bx+0x455c],0x2
0000107C  7403              jz 0x1081
0000107E  E98100            jmp 0x1102
00001081  A0AA37            mov al,[0x37aa]
00001084  98                cbw
00001085  D1E0              shl ax,1
00001087  50                push ax
00001088  A0AA37            mov al,[0x37aa]
0000108B  98                cbw
0000108C  8BD8              mov bx,ax
0000108E  8A874408          mov al,[bx+0x844]
00001092  B400              mov ah,0x0
00001094  5B                pop bx
00001095  3987E24B          cmp [bx+0x4be2],ax
00001099  7541              jnz 0x10dc
0000109B  A0AA37            mov al,[0x37aa]
0000109E  98                cbw
0000109F  D1E0              shl ax,1
000010A1  50                push ax
000010A2  A0AA37            mov al,[0x37aa]
000010A5  98                cbw
000010A6  8BD8              mov bx,ax
000010A8  8A874808          mov al,[bx+0x848]
000010AC  B400              mov ah,0x0
000010AE  5B                pop bx
000010AF  3987EA4B          cmp [bx+0x4bea],ax
000010B3  7527              jnz 0x10dc
000010B5  A0AA37            mov al,[0x37aa]
000010B8  98                cbw
000010B9  D1E0              shl ax,1
000010BB  D1E0              shl ax,1
000010BD  8BD8              mov bx,ax
000010BF  83BF9A3C00        cmp word [bx+0x3c9a],byte +0x0
000010C4  7714              ja 0x10da
000010C6  7207              jc 0x10cf
000010C8  83BF983C32        cmp word [bx+0x3c98],byte +0x32
000010CD  730B              jnc 0x10da
000010CF  A0AA37            mov al,[0x37aa]
000010D2  98                cbw
000010D3  8BD8              mov bx,ax
000010D5  C687C33710        mov byte [bx+0x37c3],0x10
000010DA  EB21              jmp short 0x10fd
000010DC  B000              mov al,0x0
000010DE  50                push ax
000010DF  A0AA37            mov al,[0x37aa]
000010E2  98                cbw
000010E3  8BD8              mov bx,ax
000010E5  8A874808          mov al,[bx+0x848]
000010E9  50                push ax
000010EA  A0AA37            mov al,[0x37aa]
000010ED  98                cbw
000010EE  8BD8              mov bx,ax
000010F0  8A874408          mov al,[bx+0x844]
000010F4  50                push ax
000010F5  9AAB150F15        call 0x150f:0x15ab
000010FA  83C406            add sp,byte +0x6
000010FD  E9E500            jmp 0x11e5
00001100  EB12              jmp short 0x1114
00001102  A0AA37            mov al,[0x37aa]
00001105  98                cbw
00001106  50                push ax
00001107  0E                push cs
00001108  E800EF            call 0xb
0000110B  59                pop cx
0000110C  9AA0180F15        call 0x150f:0x18a0
00001111  E9D100            jmp 0x11e5
00001114  EB16              jmp short 0x112c
00001116  A0AA37            mov al,[0x37aa]
00001119  50                push ax
0000111A  8A46FE            mov al,[bp-0x2]
0000111D  50                push ax
0000111E  8A46FF            mov al,[bp-0x1]
00001121  50                push ax
00001122  0E                push cs
00001123  E8E0F2            call 0x406
00001126  83C406            add sp,byte +0x6
00001129  8846FD            mov [bp-0x3],al
0000112C  EB2C              jmp short 0x115a
0000112E  8A46FA            mov al,[bp-0x6]
00001131  3A46FB            cmp al,[bp-0x5]
00001134  750E              jnz 0x1144
00001136  9A0E140F15        call 0x150f:0x140e
0000113B  3C01              cmp al,0x1
0000113D  7505              jnz 0x1144
0000113F  E9A300            jmp 0x11e5
00001142  EB16              jmp short 0x115a
00001144  8A46FB            mov al,[bp-0x5]
00001147  50                push ax
00001148  8A46FE            mov al,[bp-0x2]
0000114B  50                push ax
0000114C  8A46FF            mov al,[bp-0x1]
0000114F  50                push ax
00001150  0E                push cs
00001151  E8B2F2            call 0x406
00001154  83C406            add sp,byte +0x6
00001157  8846FD            mov [bp-0x3],al
0000115A  8A46FD            mov al,[bp-0x3]
0000115D  50                push ax
0000115E  8A46FE            mov al,[bp-0x2]
00001161  50                push ax
00001162  8A46FF            mov al,[bp-0x1]
00001165  50                push ax
00001166  9AE9110F15        call 0x150f:0x11e9
0000116B  83C406            add sp,byte +0x6
0000116E  8846FC            mov [bp-0x4],al
00001171  8A46FC            mov al,[bp-0x4]
00001174  98                cbw
00001175  8BD8              mov bx,ax
00001177  8A877808          mov al,[bx+0x878]
0000117B  0046FF            add [bp-0x1],al
0000117E  8A46FC            mov al,[bp-0x4]
00001181  98                cbw
00001182  8BD8              mov bx,ax
00001184  8A878008          mov al,[bx+0x880]
00001188  0046FE            add [bp-0x2],al
0000118B  8A46FF            mov al,[bp-0x1]
0000118E  98                cbw
0000118F  D1E0              shl ax,1
00001191  D1E0              shl ax,1
00001193  D1E0              shl ax,1
00001195  50                push ax
00001196  8A46FE            mov al,[bp-0x2]
00001199  98                cbw
0000119A  5B                pop bx
0000119B  03D8              add bx,ax
0000119D  80BF784400        cmp byte [bx+0x4478],0x0
000011A2  751E              jnz 0x11c2
000011A4  8A46FD            mov al,[bp-0x3]
000011A7  D0E0              shl al,1
000011A9  50                push ax
000011AA  A0AA37            mov al,[0x37aa]
000011AD  98                cbw
000011AE  8BD8              mov bx,ax
000011B0  58                pop ax
000011B1  8887C837          mov [bx+0x37c8],al
000011B5  A0AA37            mov al,[0x37aa]
000011B8  98                cbw
000011B9  8BD8              mov bx,ax
000011BB  C687AB3701        mov byte [bx+0x37ab],0x1
000011C0  EB23              jmp short 0x11e5
000011C2  B000              mov al,0x0
000011C4  50                push ax
000011C5  8A46FE            mov al,[bp-0x2]
000011C8  98                cbw
000011C9  BA0300            mov dx,0x3
000011CC  F7EA              imul dx
000011CE  0402              add al,0x2
000011D0  50                push ax
000011D1  8A46FF            mov al,[bp-0x1]
000011D4  98                cbw
000011D5  BA0300            mov dx,0x3
000011D8  F7EA              imul dx
000011DA  0402              add al,0x2
000011DC  50                push ax
000011DD  9AAB150F15        call 0x150f:0x15ab
000011E2  83C406            add sp,byte +0x6
000011E5  8BE5              mov sp,bp
000011E7  5D                pop bp
000011E8  CB                retf

; ---- 150F:11E9 ----
000011E9  55                push bp
000011EA  8BEC              mov bp,sp
000011EC  4C                dec sp
000011ED  4C                dec sp
000011EE  807E0A08          cmp byte [bp+0xa],0x8
000011F2  754A              jnz 0x123e
000011F4  A0AA37            mov al,[0x37aa]
000011F7  98                cbw
000011F8  D1E0              shl ax,1
000011FA  8BD8              mov bx,ax
000011FC  8B87E24B          mov ax,[bx+0x4be2]
00001200  48                dec ax
00001201  BB0300            mov bx,0x3
00001204  99                cwd
00001205  F7FB              idiv bx
00001207  A0AA37            mov al,[0x37aa]
0000120A  98                cbw
0000120B  D1E0              shl ax,1
0000120D  8BD8              mov bx,ax
0000120F  8B87EA4B          mov ax,[bx+0x4bea]
00001213  48                dec ax
00001214  BB0300            mov bx,0x3
00001217  52                push dx
00001218  99                cwd
00001219  F7FB              idiv bx
0000121B  BB0300            mov bx,0x3
0000121E  8BC2              mov ax,dx
00001220  F7EB              imul bx
00001222  5A                pop dx
00001223  02D0              add dl,al
00001225  8856FF            mov [bp-0x1],dl
00001228  807EFF04          cmp byte [bp-0x1],0x4
0000122C  7E03              jng 0x1231
0000122E  FE4EFF            dec byte [bp-0x1]
00001231  8A46FF            mov al,[bp-0x1]
00001234  98                cbw
00001235  8BD8              mov bx,ax
00001237  8A870408          mov al,[bx+0x804]
0000123B  E98D00            jmp 0x12cb
0000123E  B80400            mov ax,0x4
00001241  50                push ax
00001242  9ABA01FD21        call 0x21fd:0x1ba
00001247  59                pop cx
00001248  0BC0              or ax,ax
0000124A  757A              jnz 0x12c6
0000124C  B80200            mov ax,0x2
0000124F  50                push ax
00001250  9ABA01FD21        call 0x21fd:0x1ba
00001255  59                pop cx
00001256  8A560A            mov dl,[bp+0xa]
00001259  02D0              add dl,al
0000125B  FECA              dec dl
0000125D  8856FE            mov [bp-0x2],dl
00001260  8AC2              mov al,dl
00001262  3C07              cmp al,0x7
00001264  7E06              jng 0x126c
00001266  C646FE00          mov byte [bp-0x2],0x0
0000126A  EB0A              jmp short 0x1276
0000126C  807EFE00          cmp byte [bp-0x2],0x0
00001270  7D04              jnl 0x1276
00001272  C646FE07          mov byte [bp-0x2],0x7
00001276  8A46FE            mov al,[bp-0x2]
00001279  98                cbw
0000127A  8BD8              mov bx,ax
0000127C  8A877808          mov al,[bx+0x878]
00001280  004606            add [bp+0x6],al
00001283  8A46FE            mov al,[bp-0x2]
00001286  98                cbw
00001287  8BD8              mov bx,ax
00001289  8A878008          mov al,[bx+0x880]
0000128D  004608            add [bp+0x8],al
00001290  807E0600          cmp byte [bp+0x6],0x0
00001294  7C30              jl 0x12c6
00001296  807E0607          cmp byte [bp+0x6],0x7
0000129A  7F2A              jg 0x12c6
0000129C  807E0800          cmp byte [bp+0x8],0x0
000012A0  7C24              jl 0x12c6
000012A2  807E0807          cmp byte [bp+0x8],0x7
000012A6  7F1E              jg 0x12c6
000012A8  8A4606            mov al,[bp+0x6]
000012AB  98                cbw
000012AC  D1E0              shl ax,1
000012AE  D1E0              shl ax,1
000012B0  D1E0              shl ax,1
000012B2  50                push ax
000012B3  8A4608            mov al,[bp+0x8]
000012B6  98                cbw
000012B7  5B                pop bx
000012B8  03D8              add bx,ax
000012BA  80BF784400        cmp byte [bx+0x4478],0x0
000012BF  7405              jz 0x12c6
000012C1  8A46FE            mov al,[bp-0x2]
000012C4  EB05              jmp short 0x12cb
000012C6  8A460A            mov al,[bp+0xa]
000012C9  EB00              jmp short 0x12cb
000012CB  8BE5              mov sp,bp
000012CD  5D                pop bp
000012CE  CB                retf

; ---- 150F:12CF ----
000012CF  55                push bp
000012D0  8BEC              mov bp,sp
000012D2  83EC08            sub sp,byte +0x8
000012D5  56                push si
000012D6  57                push di
000012D7  A0AA37            mov al,[0x37aa]
000012DA  98                cbw
000012DB  8BD8              mov bx,ax
000012DD  C687C04501        mov byte [bx+0x45c0],0x1
000012E2  8B460A            mov ax,[bp+0xa]
000012E5  3B4606            cmp ax,[bp+0x6]
000012E8  750D              jnz 0x12f7
000012EA  8B460C            mov ax,[bp+0xc]
000012ED  3B4608            cmp ax,[bp+0x8]
000012F0  7505              jnz 0x12f7
000012F2  B0FF              mov al,0xff
000012F4  E9C900            jmp 0x13c0
000012F7  8B460A            mov ax,[bp+0xa]
000012FA  3B4606            cmp ax,[bp+0x6]
000012FD  7E0E              jng 0x130d
000012FF  8B460A            mov ax,[bp+0xa]
00001302  2B4606            sub ax,[bp+0x6]
00001305  8946FA            mov [bp-0x6],ax
00001308  BE0100            mov si,0x1
0000130B  EB0B              jmp short 0x1318
0000130D  8B4606            mov ax,[bp+0x6]
00001310  2B460A            sub ax,[bp+0xa]
00001313  8946FA            mov [bp-0x6],ax
00001316  33F6              xor si,si
00001318  8B460C            mov ax,[bp+0xc]
0000131B  3B4608            cmp ax,[bp+0x8]
0000131E  7E10              jng 0x1330
00001320  8B460C            mov ax,[bp+0xc]
00001323  2B4608            sub ax,[bp+0x8]
00001326  8BF8              mov di,ax
00001328  8BC6              mov ax,si
0000132A  40                inc ax
0000132B  40                inc ax
0000132C  8BF0              mov si,ax
0000132E  EB08              jmp short 0x1338
00001330  8B4608            mov ax,[bp+0x8]
00001333  2B460C            sub ax,[bp+0xc]
00001336  8BF8              mov di,ax
00001338  3B7EFA            cmp di,[bp-0x6]
0000133B  7E1B              jng 0x1358
0000133D  8B46FA            mov ax,[bp-0x6]
00001340  BAE803            mov dx,0x3e8
00001343  F7EA              imul dx
00001345  99                cwd
00001346  F7FF              idiv di
00001348  99                cwd
00001349  8946FC            mov [bp-0x4],ax
0000134C  8956FE            mov [bp-0x2],dx
0000134F  8BC6              mov ax,si
00001351  050400            add ax,0x4
00001354  8BF0              mov si,ax
00001356  EB12              jmp short 0x136a
00001358  8BC7              mov ax,di
0000135A  BAE803            mov dx,0x3e8
0000135D  F7EA              imul dx
0000135F  99                cwd
00001360  F77EFA            idiv word [bp-0x6]
00001363  99                cwd
00001364  8946FC            mov [bp-0x4],ax
00001367  8956FE            mov [bp-0x2],dx
0000136A  8A840C08          mov al,[si+0x80c]
0000136E  98                cbw
0000136F  8BC8              mov cx,ax
00001371  8A841408          mov al,[si+0x814]
00001375  98                cbw
00001376  8946F8            mov [bp-0x8],ax
00001379  837EFE00          cmp word [bp-0x2],byte +0x0
0000137D  7C0E              jl 0x138d
0000137F  7F07              jg 0x1388
00001381  817EFCFA00        cmp word [bp-0x4],0xfa
00001386  7605              jna 0x138d
00001388  8B46F8            mov ax,[bp-0x8]
0000138B  03C8              add cx,ax
0000138D  837EFE00          cmp word [bp-0x2],byte +0x0
00001391  7C0E              jl 0x13a1
00001393  7F07              jg 0x139c
00001395  817EFCEE02        cmp word [bp-0x4],0x2ee
0000139A  7605              jna 0x13a1
0000139C  8B46F8            mov ax,[bp-0x8]
0000139F  03C8              add cx,ax
000013A1  83F910            cmp cx,byte +0x10
000013A4  7502              jnz 0x13a8
000013A6  33C9              xor cx,cx
000013A8  A0AA37            mov al,[0x37aa]
000013AB  98                cbw
000013AC  D1E0              shl ax,1
000013AE  8BD8              mov bx,ax
000013B0  8B87E643          mov ax,[bx+0x43e6]
000013B4  3BC1              cmp ax,cx
000013B6  7504              jnz 0x13bc
000013B8  B0FF              mov al,0xff
000013BA  EB04              jmp short 0x13c0
000013BC  8AC1              mov al,cl
000013BE  EB00              jmp short 0x13c0
000013C0  5F                pop di
000013C1  5E                pop si
000013C2  8BE5              mov sp,bp
000013C4  5D                pop bp
000013C5  CB                retf

; ---- 150F:13C6 ----
000013C6  55                push bp
000013C7  8BEC              mov bp,sp
000013C9  8A5608            mov dl,[bp+0x8]
000013CC  807E0606          cmp byte [bp+0x6],0x6
000013D0  7C18              jl 0x13ea
000013D2  80FA06            cmp dl,0x6
000013D5  7C04              jl 0x13db
000013D7  B002              mov al,0x2
000013D9  EB0B              jmp short 0x13e6
000013DB  80FA01            cmp dl,0x1
000013DE  7F04              jg 0x13e4
000013E0  B001              mov al,0x1
000013E2  EB02              jmp short 0x13e6
000013E4  B0FF              mov al,0xff
000013E6  EB24              jmp short 0x140c
000013E8  EB22              jmp short 0x140c
000013EA  807E0601          cmp byte [bp+0x6],0x1
000013EE  7F18              jg 0x1408
000013F0  80FA06            cmp dl,0x6
000013F3  7C04              jl 0x13f9
000013F5  B003              mov al,0x3
000013F7  EB0B              jmp short 0x1404
000013F9  80FA01            cmp dl,0x1
000013FC  7F04              jg 0x1402
000013FE  B000              mov al,0x0
00001400  EB02              jmp short 0x1404
00001402  B0FF              mov al,0xff
00001404  EB06              jmp short 0x140c
00001406  EB04              jmp short 0x140c
00001408  B0FF              mov al,0xff
0000140A  EB00              jmp short 0x140c
0000140C  5D                pop bp
0000140D  CB                retf

; ---- 150F:140E ----
0000140E  55                push bp
0000140F  8BEC              mov bp,sp
00001411  83EC08            sub sp,byte +0x8
00001414  C646F964          mov byte [bp-0x7],0x64
00001418  C646F8FF          mov byte [bp-0x8],0xff
0000141C  A0AA37            mov al,[0x37aa]
0000141F  98                cbw
00001420  8BD8              mov bx,ax
00001422  8A87E23C          mov al,[bx+0x3ce2]
00001426  8846FE            mov [bp-0x2],al
00001429  A0AA37            mov al,[0x37aa]
0000142C  98                cbw
0000142D  D1E0              shl ax,1
0000142F  8BD8              mov bx,ax
00001431  8A87E24B          mov al,[bx+0x4be2]
00001435  8846FB            mov [bp-0x5],al
00001438  A0AA37            mov al,[0x37aa]
0000143B  98                cbw
0000143C  D1E0              shl ax,1
0000143E  8BD8              mov bx,ax
00001440  8A87EA4B          mov al,[bx+0x4bea]
00001444  8846FA            mov [bp-0x6],al
00001447  C646FF00          mov byte [bp-0x1],0x0
0000144B  E98E00            jmp 0x14dc
0000144E  8A46FE            mov al,[bp-0x2]
00001451  98                cbw
00001452  BA0700            mov dx,0x7
00001455  F7EA              imul dx
00001457  50                push ax
00001458  8A46FF            mov al,[bp-0x1]
0000145B  98                cbw
0000145C  5B                pop bx
0000145D  03D8              add bx,ax
0000145F  80BFFE43FF        cmp byte [bx+0x43fe],0xff
00001464  7502              jnz 0x1468
00001466  EB71              jmp short 0x14d9
00001468  8A46FE            mov al,[bp-0x2]
0000146B  98                cbw
0000146C  BA0700            mov dx,0x7
0000146F  F7EA              imul dx
00001471  50                push ax
00001472  8A46FF            mov al,[bp-0x1]
00001475  98                cbw
00001476  5B                pop bx
00001477  03D8              add bx,ax
00001479  8A87FE43          mov al,[bx+0x43fe]
0000147D  2A46FB            sub al,[bp-0x5]
00001480  8846FD            mov [bp-0x3],al
00001483  0AC0              or al,al
00001485  7D08              jnl 0x148f
00001487  8A46FD            mov al,[bp-0x3]
0000148A  F6D8              neg al
0000148C  8846FD            mov [bp-0x3],al
0000148F  8A46FE            mov al,[bp-0x2]
00001492  98                cbw
00001493  BA0700            mov dx,0x7
00001496  F7EA              imul dx
00001498  50                push ax
00001499  8A46FF            mov al,[bp-0x1]
0000149C  98                cbw
0000149D  5B                pop bx
0000149E  03D8              add bx,ax
000014A0  8A871A44          mov al,[bx+0x441a]
000014A4  2A46FA            sub al,[bp-0x6]
000014A7  8846FC            mov [bp-0x4],al
000014AA  0AC0              or al,al
000014AC  7D08              jnl 0x14b6
000014AE  8A46FC            mov al,[bp-0x4]
000014B1  F6D8              neg al
000014B3  8846FC            mov [bp-0x4],al
000014B6  8A46FD            mov al,[bp-0x3]
000014B9  98                cbw
000014BA  50                push ax
000014BB  8A46FC            mov al,[bp-0x4]
000014BE  98                cbw
000014BF  5A                pop dx
000014C0  03D0              add dx,ax
000014C2  8A46F9            mov al,[bp-0x7]
000014C5  98                cbw
000014C6  3BD0              cmp dx,ax
000014C8  7D0F              jnl 0x14d9
000014CA  8A46FD            mov al,[bp-0x3]
000014CD  0246FC            add al,[bp-0x4]
000014D0  8846F9            mov [bp-0x7],al
000014D3  8A46FF            mov al,[bp-0x1]
000014D6  8846F8            mov [bp-0x8],al
000014D9  FE46FF            inc byte [bp-0x1]
000014DC  807EFF07          cmp byte [bp-0x1],0x7
000014E0  7D03              jnl 0x14e5
000014E2  E969FF            jmp 0x144e
000014E5  8A46FE            mov al,[bp-0x2]
000014E8  98                cbw
000014E9  D1E0              shl ax,1
000014EB  8BD8              mov bx,ax
000014ED  8A871C08          mov al,[bx+0x81c]
000014F1  2A46FB            sub al,[bp-0x5]
000014F4  8846FD            mov [bp-0x3],al
000014F7  0AC0              or al,al
000014F9  7D08              jnl 0x1503
000014FB  8A46FD            mov al,[bp-0x3]
000014FE  F6D8              neg al
00001500  8846FD            mov [bp-0x3],al
00001503  8A46FE            mov al,[bp-0x2]
00001506  98                cbw
00001507  D1E0              shl ax,1
00001509  8BD8              mov bx,ax
0000150B  8A872408          mov al,[bx+0x824]
0000150F  2A46FA            sub al,[bp-0x6]
00001512  8846FC            mov [bp-0x4],al
00001515  0AC0              or al,al
00001517  7D08              jnl 0x1521
00001519  8A46FC            mov al,[bp-0x4]
0000151C  F6D8              neg al
0000151E  8846FC            mov [bp-0x4],al
00001521  8A46FD            mov al,[bp-0x3]
00001524  98                cbw
00001525  50                push ax
00001526  8A46FC            mov al,[bp-0x4]
00001529  98                cbw
0000152A  5A                pop dx
0000152B  03D0              add dx,ax
0000152D  8A46F9            mov al,[bp-0x7]
00001530  98                cbw
00001531  3BD0              cmp dx,ax
00001533  7F2B              jg 0x1560
00001535  B001              mov al,0x1
00001537  50                push ax
00001538  8A46FE            mov al,[bp-0x2]
0000153B  98                cbw
0000153C  D1E0              shl ax,1
0000153E  8BD8              mov bx,ax
00001540  8A872408          mov al,[bx+0x824]
00001544  50                push ax
00001545  8A46FE            mov al,[bp-0x2]
00001548  98                cbw
00001549  D1E0              shl ax,1
0000154B  8BD8              mov bx,ax
0000154D  8A871C08          mov al,[bx+0x81c]
00001551  50                push ax
00001552  9AAB150F15        call 0x150f:0x15ab
00001557  83C406            add sp,byte +0x6
0000155A  B001              mov al,0x1
0000155C  EB49              jmp short 0x15a7
0000155E  EB47              jmp short 0x15a7
00001560  807EF8FF          cmp byte [bp-0x8],0xff
00001564  743D              jz 0x15a3
00001566  B001              mov al,0x1
00001568  50                push ax
00001569  8A46FE            mov al,[bp-0x2]
0000156C  98                cbw
0000156D  BA0700            mov dx,0x7
00001570  F7EA              imul dx
00001572  50                push ax
00001573  8A46F8            mov al,[bp-0x8]
00001576  98                cbw
00001577  5B                pop bx
00001578  03D8              add bx,ax
0000157A  8A871A44          mov al,[bx+0x441a]
0000157E  50                push ax
0000157F  8A46FE            mov al,[bp-0x2]
00001582  98                cbw
00001583  BA0700            mov dx,0x7
00001586  F7EA              imul dx
00001588  50                push ax
00001589  8A46F8            mov al,[bp-0x8]
0000158C  98                cbw
0000158D  5B                pop bx
0000158E  03D8              add bx,ax
00001590  8A87FE43          mov al,[bx+0x43fe]
00001594  50                push ax
00001595  9AAB150F15        call 0x150f:0x15ab
0000159A  83C406            add sp,byte +0x6
0000159D  B001              mov al,0x1
0000159F  EB06              jmp short 0x15a7
000015A1  EB04              jmp short 0x15a7
000015A3  B000              mov al,0x0
000015A5  EB00              jmp short 0x15a7
000015A7  8BE5              mov sp,bp
000015A9  5D                pop bp
000015AA  CB                retf

; ---- 150F:15AB ----
000015AB  55                push bp
000015AC  8BEC              mov bp,sp
000015AE  A0AA37            mov al,[0x37aa]
000015B1  98                cbw
000015B2  8A5606            mov dl,[bp+0x6]
000015B5  8BD8              mov bx,ax
000015B7  8897E448          mov [bx+0x48e4],dl
000015BB  A0AA37            mov al,[0x37aa]
000015BE  98                cbw
000015BF  8A5608            mov dl,[bp+0x8]
000015C2  8BD8              mov bx,ax
000015C4  8897E848          mov [bx+0x48e8],dl
000015C8  A0AA37            mov al,[0x37aa]
000015CB  98                cbw
000015CC  8BD8              mov bx,ax
000015CE  C687C04500        mov byte [bx+0x45c0],0x0
000015D3  A0AA37            mov al,[0x37aa]
000015D6  98                cbw
000015D7  8A560A            mov dl,[bp+0xa]
000015DA  8BD8              mov bx,ax
000015DC  8897A44B          mov [bx+0x4ba4],dl
000015E0  5D                pop bp
000015E1  CB                retf

; ---- 150F:15E2 ----
000015E2  55                push bp
000015E3  8BEC              mov bp,sp
000015E5  A0AA37            mov al,[0x37aa]
000015E8  98                cbw
000015E9  8BD8              mov bx,ax
000015EB  C687E448FF        mov byte [bx+0x48e4],0xff
000015F0  5D                pop bp
000015F1  CB                retf

; ---- 150F:15F2 ----
000015F2  55                push bp
000015F3  8BEC              mov bp,sp
000015F5  A0AA37            mov al,[0x37aa]
000015F8  98                cbw
000015F9  8A5606            mov dl,[bp+0x6]
000015FC  8BD8              mov bx,ax
000015FE  88978034          mov [bx+0x3480],dl
00001602  A0AA37            mov al,[0x37aa]
00001605  98                cbw
00001606  8A5608            mov dl,[bp+0x8]
00001609  8BD8              mov bx,ax
0000160B  88978434          mov [bx+0x3484],dl
0000160F  A0AA37            mov al,[0x37aa]
00001612  98                cbw
00001613  8BD8              mov bx,ax
00001615  C687C04500        mov byte [bx+0x45c0],0x0
0000161A  A0AA37            mov al,[0x37aa]
0000161D  98                cbw
0000161E  8A560A            mov dl,[bp+0xa]
00001621  8BD8              mov bx,ax
00001623  8897CD3C          mov [bx+0x3ccd],dl
00001627  5D                pop bp
00001628  CB                retf

; ---- 150F:1629 ----
00001629  55                push bp
0000162A  8BEC              mov bp,sp
0000162C  A0AA37            mov al,[0x37aa]
0000162F  98                cbw
00001630  8BD8              mov bx,ax
00001632  C6878034FF        mov byte [bx+0x3480],0xff
00001637  A0AA37            mov al,[0x37aa]
0000163A  98                cbw
0000163B  8BD8              mov bx,ax
0000163D  C687C04500        mov byte [bx+0x45c0],0x0
00001642  5D                pop bp
00001643  CB                retf

; ---- 150F:1644 ----
00001644  55                push bp
00001645  8BEC              mov bp,sp
00001647  83EC06            sub sp,byte +0x6
0000164A  56                push si
0000164B  57                push di
0000164C  A0AA37            mov al,[0x37aa]
0000164F  98                cbw
00001650  D1E0              shl ax,1
00001652  8BD8              mov bx,ax
00001654  8B875C37          mov ax,[bx+0x375c]
00001658  8946FE            mov [bp-0x2],ax
0000165B  A0AA37            mov al,[0x37aa]
0000165E  98                cbw
0000165F  D1E0              shl ax,1
00001661  8BD8              mov bx,ax
00001663  8B876437          mov ax,[bx+0x3764]
00001667  8946FC            mov [bp-0x4],ax
0000166A  A0AA37            mov al,[0x37aa]
0000166D  98                cbw
0000166E  D1E0              shl ax,1
00001670  8BD8              mov bx,ax
00001672  8A87E643          mov al,[bx+0x43e6]
00001676  8AC8              mov cl,al
00001678  8AC1              mov al,cl
0000167A  98                cbw
0000167B  BB0400            mov bx,0x4
0000167E  99                cwd
0000167F  F7FB              idiv bx
00001681  8BDA              mov bx,dx
00001683  8A87BA06          mov al,[bx+0x6ba]
00001687  B400              mov ah,0x0
00001689  50                push ax
0000168A  A0AA37            mov al,[0x37aa]
0000168D  98                cbw
0000168E  BA0600            mov dx,0x6
00001691  F7EA              imul dx
00001693  8BD8              mov bx,ax
00001695  8A87D143          mov al,[bx+0x43d1]
00001699  B400              mov ah,0x0
0000169B  BB0A00            mov bx,0xa
0000169E  99                cwd
0000169F  F7FB              idiv bx
000016A1  40                inc ax
000016A2  8BD0              mov dx,ax
000016A4  58                pop ax
000016A5  F7EA              imul dx
000016A7  8846FB            mov [bp-0x5],al
000016AA  8AC1              mov al,cl
000016AC  98                cbw
000016AD  8BD8              mov bx,ax
000016AF  8A87DC06          mov al,[bx+0x6dc]
000016B3  98                cbw
000016B4  50                push ax
000016B5  8A46FB            mov al,[bp-0x5]
000016B8  98                cbw
000016B9  8BD0              mov dx,ax
000016BB  58                pop ax
000016BC  F7EA              imul dx
000016BE  8B56FE            mov dx,[bp-0x2]
000016C1  03D0              add dx,ax
000016C3  8BFA              mov di,dx
000016C5  8AC1              mov al,cl
000016C7  98                cbw
000016C8  8BD8              mov bx,ax
000016CA  8A87EC06          mov al,[bx+0x6ec]
000016CE  98                cbw
000016CF  50                push ax
000016D0  8A46FB            mov al,[bp-0x5]
000016D3  98                cbw
000016D4  8BD0              mov dx,ax
000016D6  58                pop ax
000016D7  F7EA              imul dx
000016D9  8B56FC            mov dx,[bp-0x4]
000016DC  03D0              add dx,ax
000016DE  8BF2              mov si,dx
000016E0  8B4606            mov ax,[bp+0x6]
000016E3  3B46FE            cmp ax,[bp-0x2]
000016E6  7E10              jng 0x16f8
000016E8  8BC7              mov ax,di
000016EA  052800            add ax,0x28
000016ED  3B4606            cmp ax,[bp+0x6]
000016F0  7D04              jnl 0x16f6
000016F2  B000              mov al,0x0
000016F4  EB3A              jmp short 0x1730
000016F6  EB0E              jmp short 0x1706
000016F8  8BC7              mov ax,di
000016FA  05D8FF            add ax,0xffd8
000016FD  3B4606            cmp ax,[bp+0x6]
00001700  7E04              jng 0x1706
00001702  B000              mov al,0x0
00001704  EB2A              jmp short 0x1730
00001706  8B4608            mov ax,[bp+0x8]
00001709  3B46FC            cmp ax,[bp-0x4]
0000170C  7E10              jng 0x171e
0000170E  8BC6              mov ax,si
00001710  052800            add ax,0x28
00001713  3B4608            cmp ax,[bp+0x8]
00001716  7D04              jnl 0x171c
00001718  B000              mov al,0x0
0000171A  EB14              jmp short 0x1730
0000171C  EB0E              jmp short 0x172c
0000171E  8BC6              mov ax,si
00001720  05D8FF            add ax,0xffd8
00001723  3B4608            cmp ax,[bp+0x8]
00001726  7E04              jng 0x172c
00001728  B000              mov al,0x0
0000172A  EB04              jmp short 0x1730
0000172C  B001              mov al,0x1
0000172E  EB00              jmp short 0x1730
00001730  5F                pop di
00001731  5E                pop si
00001732  8BE5              mov sp,bp
00001734  5D                pop bp
00001735  CB                retf

; ---- 150F:1736 ----
00001736  55                push bp
00001737  8BEC              mov bp,sp
00001739  4C                dec sp
0000173A  4C                dec sp
0000173B  C646FF00          mov byte [bp-0x1],0x0
0000173F  EB29              jmp short 0x176a
00001741  8A46FF            mov al,[bp-0x1]
00001744  98                cbw
00001745  8BD8              mov bx,ax
00001747  80BF400801        cmp byte [bx+0x840],0x1
0000174C  7519              jnz 0x1767
0000174E  8A46FF            mov al,[bp-0x1]
00001751  98                cbw
00001752  8BD8              mov bx,ax
00001754  8A87E23C          mov al,[bx+0x3ce2]
00001758  3A4606            cmp al,[bp+0x6]
0000175B  750A              jnz 0x1767
0000175D  8A46FF            mov al,[bp-0x1]
00001760  98                cbw
00001761  50                push ax
00001762  0E                push cs
00001763  E8A5E8            call 0xb
00001766  59                pop cx
00001767  FE46FF            inc byte [bp-0x1]
0000176A  807EFF03          cmp byte [bp-0x1],0x3
0000176E  7ED1              jng 0x1741
00001770  8BE5              mov sp,bp
00001772  5D                pop bp
00001773  CB                retf

; ---- 150F:1774 ----
00001774  55                push bp
00001775  8BEC              mov bp,sp
00001777  4C                dec sp
00001778  4C                dec sp
00001779  8A4606            mov al,[bp+0x6]
0000177C  98                cbw
0000177D  8BD8              mov bx,ax
0000177F  80BF400800        cmp byte [bx+0x840],0x0
00001784  7503              jnz 0x1789
00001786  E91301            jmp 0x189c
00001789  8A4606            mov al,[bp+0x6]
0000178C  98                cbw
0000178D  8BD8              mov bx,ax
0000178F  8A87C73D          mov al,[bx+0x3dc7]
00001793  3A06AA37          cmp al,[0x37aa]
00001797  750E              jnz 0x17a7
00001799  8A4606            mov al,[bp+0x6]
0000179C  98                cbw
0000179D  8BD8              mov bx,ax
0000179F  C687BA4BFF        mov byte [bx+0x4bba],0xff
000017A4  E9F500            jmp 0x189c
000017A7  A0AA37            mov al,[0x37aa]
000017AA  98                cbw
000017AB  8BD8              mov bx,ax
000017AD  8A87E740          mov al,[bx+0x40e7]
000017B1  98                cbw
000017B2  50                push ax
000017B3  8A4606            mov al,[bp+0x6]
000017B6  98                cbw
000017B7  8BD8              mov bx,ax
000017B9  8A87E740          mov al,[bx+0x40e7]
000017BD  98                cbw
000017BE  40                inc ax
000017BF  5A                pop dx
000017C0  3BD0              cmp dx,ax
000017C2  7E04              jng 0x17c8
000017C4  B001              mov al,0x1
000017C6  EB02              jmp short 0x17ca
000017C8  B000              mov al,0x0
000017CA  8846FF            mov [bp-0x1],al
000017CD  807EFF00          cmp byte [bp-0x1],0x0
000017D1  7415              jz 0x17e8
000017D3  8A4606            mov al,[bp+0x6]
000017D6  98                cbw
000017D7  D1E0              shl ax,1
000017D9  D1E0              shl ax,1
000017DB  8BD8              mov bx,ax
000017DD  83BF6E3700        cmp word [bx+0x376e],byte +0x0
000017E2  7504              jnz 0x17e8
000017E4  B001              mov al,0x1
000017E6  EB02              jmp short 0x17ea
000017E8  B000              mov al,0x0
000017EA  8846FE            mov [bp-0x2],al
000017ED  8A4606            mov al,[bp+0x6]
000017F0  98                cbw
000017F1  8BD8              mov bx,ax
000017F3  80BF5C4500        cmp byte [bx+0x455c],0x0
000017F8  7560              jnz 0x185a
000017FA  A0AA37            mov al,[0x37aa]
000017FD  98                cbw
000017FE  8BD8              mov bx,ax
00001800  80BF400801        cmp byte [bx+0x840],0x1
00001805  7510              jnz 0x1817
00001807  A0AA37            mov al,[0x37aa]
0000180A  98                cbw
0000180B  8BD8              mov bx,ax
0000180D  80BF5C4500        cmp byte [bx+0x455c],0x0
00001812  7503              jnz 0x1817
00001814  E98500            jmp 0x189c
00001817  8A4606            mov al,[bp+0x6]
0000181A  98                cbw
0000181B  8BD8              mov bx,ax
0000181D  8A87E23C          mov al,[bx+0x3ce2]
00001821  3A06AA37          cmp al,[0x37aa]
00001825  751B              jnz 0x1842
00001827  807EFF00          cmp byte [bp-0x1],0x0
0000182B  7415              jz 0x1842
0000182D  807EFE00          cmp byte [bp-0x2],0x0
00001831  740D              jz 0x1840
00001833  8A4606            mov al,[bp+0x6]
00001836  98                cbw
00001837  8BD8              mov bx,ax
00001839  C687E23CFF        mov byte [bx+0x3ce2],0xff
0000183E  EB5C              jmp short 0x189c
00001840  EB16              jmp short 0x1858
00001842  807E0800          cmp byte [bp+0x8],0x0
00001846  740E              jz 0x1856
00001848  B80300            mov ax,0x3
0000184B  50                push ax
0000184C  9ABA01FD21        call 0x21fd:0x1ba
00001851  59                pop cx
00001852  0BC0              or ax,ax
00001854  7402              jz 0x1858
00001856  EB44              jmp short 0x189c
00001858  EB08              jmp short 0x1862
0000185A  807EFE00          cmp byte [bp-0x2],0x0
0000185E  7402              jz 0x1862
00001860  EB3A              jmp short 0x189c
00001862  8A4606            mov al,[bp+0x6]
00001865  98                cbw
00001866  8BD8              mov bx,ax
00001868  C6878034FF        mov byte [bx+0x3480],0xff
0000186D  8A4606            mov al,[bp+0x6]
00001870  98                cbw
00001871  8BD8              mov bx,ax
00001873  C687C04500        mov byte [bx+0x45c0],0x0
00001878  8A4606            mov al,[bp+0x6]
0000187B  98                cbw
0000187C  8BD8              mov bx,ax
0000187E  C687C837FF        mov byte [bx+0x37c8],0xff
00001883  8A4606            mov al,[bp+0x6]
00001886  98                cbw
00001887  8A16AA37          mov dl,[0x37aa]
0000188B  8BD8              mov bx,ax
0000188D  8897C73D          mov [bx+0x3dc7],dl
00001891  8A4606            mov al,[bp+0x6]
00001894  98                cbw
00001895  8BD8              mov bx,ax
00001897  C687000801        mov byte [bx+0x800],0x1
0000189C  8BE5              mov sp,bp
0000189E  5D                pop bp
0000189F  CB                retf

; ---- 150F:18A0 ----
000018A0  55                push bp
000018A1  8BEC              mov bp,sp
000018A3  A0AA37            mov al,[0x37aa]
000018A6  98                cbw
000018A7  8BD8              mov bx,ax
000018A9  C6875C4502        mov byte [bx+0x455c],0x2
000018AE  A0AA37            mov al,[0x37aa]
000018B1  98                cbw
000018B2  8BD8              mov bx,ax
000018B4  80BF423D01        cmp byte [bx+0x3d42],0x1
000018B9  761B              jna 0x18d6
000018BB  9AF1180F15        call 0x150f:0x18f1
000018C0  A0AA37            mov al,[0x37aa]
000018C3  98                cbw
000018C4  8BD8              mov bx,ax
000018C6  80BFE23CFF        cmp byte [bx+0x3ce2],0xff
000018CB  7402              jz 0x18cf
000018CD  EB20              jmp short 0x18ef
000018CF  9AFE1B0F15        call 0x150f:0x1bfe
000018D4  EB19              jmp short 0x18ef
000018D6  9AFE1B0F15        call 0x150f:0x1bfe
000018DB  A0AA37            mov al,[0x37aa]
000018DE  98                cbw
000018DF  8BD8              mov bx,ax
000018E1  80BFC73DFF        cmp byte [bx+0x3dc7],0xff
000018E6  7402              jz 0x18ea
000018E8  EB05              jmp short 0x18ef
000018EA  9AF1180F15        call 0x150f:0x18f1
000018EF  5D                pop bp
000018F0  CB                retf

; ---- 150F:18F1 ----
000018F1  55                push bp
000018F2  8BEC              mov bp,sp
000018F4  4C                dec sp
000018F5  4C                dec sp
000018F6  56                push si
000018F7  57                push di
000018F8  A0AA37            mov al,[0x37aa]
000018FB  98                cbw
000018FC  8BD8              mov bx,ax
000018FE  C687E23CFF        mov byte [bx+0x3ce2],0xff
00001903  A0AA37            mov al,[0x37aa]
00001906  98                cbw
00001907  D1E0              shl ax,1
00001909  D1E0              shl ax,1
0000190B  8BD8              mov bx,ax
0000190D  83BF6C3714        cmp word [bx+0x376c],byte +0x14
00001912  7303              jnc 0x1917
00001914  E97101            jmp 0x1a88
00001917  A0AA37            mov al,[0x37aa]
0000191A  98                cbw
0000191B  8BD8              mov bx,ax
0000191D  80BF423D00        cmp byte [bx+0x3d42],0x0
00001922  7520              jnz 0x1944
00001924  A0AA37            mov al,[0x37aa]
00001927  98                cbw
00001928  8BD8              mov bx,ax
0000192A  80BFB64B00        cmp byte [bx+0x4bb6],0x0
0000192F  7513              jnz 0x1944
00001931  A0AA37            mov al,[0x37aa]
00001934  98                cbw
00001935  50                push ax
00001936  9AD81A0F15        call 0x150f:0x1ad8
0000193B  59                pop cx
0000193C  3D0300            cmp ax,0x3
0000193F  7D03              jnl 0x1944
00001941  E94401            jmp 0x1a88
00001944  803ED42DFF        cmp byte [0x2dd4],0xff
00001949  7403              jz 0x194e
0000194B  E90901            jmp 0x1a57
0000194E  A0AA37            mov al,[0x37aa]
00001951  98                cbw
00001952  BA0600            mov dx,0x6
00001955  F7EA              imul dx
00001957  8BD8              mov bx,ax
00001959  8A87CE43          mov al,[bx+0x43ce]
0000195D  B400              mov ah,0x0
0000195F  BB0A00            mov bx,0xa
00001962  99                cwd
00001963  F7FB              idiv bx
00001965  50                push ax
00001966  A0AA37            mov al,[0x37aa]
00001969  98                cbw
0000196A  BA0600            mov dx,0x6
0000196D  F7EA              imul dx
0000196F  8BD8              mov bx,ax
00001971  8A87CF43          mov al,[bx+0x43cf]
00001975  B400              mov ah,0x0
00001977  BB0A00            mov bx,0xa
0000197A  99                cwd
0000197B  F7FB              idiv bx
0000197D  5A                pop dx
0000197E  03D0              add dx,ax
00001980  83FA04            cmp dx,byte +0x4
00001983  7D03              jnl 0x1988
00001985  E90001            jmp 0x1a88
00001988  B80300            mov ax,0x3
0000198B  50                push ax
0000198C  9ABA01FD21        call 0x21fd:0x1ba
00001991  59                pop cx
00001992  8946FE            mov [bp-0x2],ax
00001995  33F6              xor si,si
00001997  E9B300            jmp 0x1a4d
0000199A  8B46FE            mov ax,[bp-0x2]
0000199D  40                inc ax
0000199E  8946FE            mov [bp-0x2],ax
000019A1  3D0300            cmp ax,0x3
000019A4  7E05              jng 0x19ab
000019A6  C746FE0000        mov word [bp-0x2],0x0
000019AB  A0AA37            mov al,[0x37aa]
000019AE  98                cbw
000019AF  3B46FE            cmp ax,[bp-0x2]
000019B2  7503              jnz 0x19b7
000019B4  E99500            jmp 0x1a4c
000019B7  8B5EFE            mov bx,[bp-0x2]
000019BA  80BF400801        cmp byte [bx+0x840],0x1
000019BF  7524              jnz 0x19e5
000019C1  33C0              xor ax,ax
000019C3  BA0200            mov dx,0x2
000019C6  50                push ax
000019C7  52                push dx
000019C8  A0AA37            mov al,[0x37aa]
000019CB  98                cbw
000019CC  D1E0              shl ax,1
000019CE  D1E0              shl ax,1
000019D0  8BD8              mov bx,ax
000019D2  FFB7D43C          push word [bx+0x3cd4]
000019D6  FFB7D23C          push word [bx+0x3cd2]
000019DA  9A984F0000        call 0x0:0x4f98
000019DF  0BC2              or ax,dx
000019E1  7402              jz 0x19e5
000019E3  EB67              jmp short 0x1a4c
000019E5  FF76FE            push word [bp-0x2]
000019E8  9AD81A0F15        call 0x150f:0x1ad8
000019ED  59                pop cx
000019EE  8BF8              mov di,ax
000019F0  A0AA37            mov al,[0x37aa]
000019F3  98                cbw
000019F4  8BD8              mov bx,ax
000019F6  8A87E740          mov al,[bx+0x40e7]
000019FA  98                cbw
000019FB  3BC7              cmp ax,di
000019FD  7D02              jnl 0x1a01
000019FF  EB4B              jmp short 0x1a4c
00001A01  8B5EFE            mov bx,[bp-0x2]
00001A04  8A87143D          mov al,[bx+0x3d14]
00001A08  98                cbw
00001A09  3B46FE            cmp ax,[bp-0x2]
00001A0C  7517              jnz 0x1a25
00001A0E  8B5EFE            mov bx,[bp-0x2]
00001A11  8A87E740          mov al,[bx+0x40e7]
00001A15  50                push ax
00001A16  A0AA37            mov al,[0x37aa]
00001A19  98                cbw
00001A1A  8BD8              mov bx,ax
00001A1C  58                pop ax
00001A1D  3A87E740          cmp al,[bx+0x40e7]
00001A21  7E02              jng 0x1a25
00001A23  EB27              jmp short 0x1a4c
00001A25  FF76FE            push word [bp-0x2]
00001A28  9A8E1A0F15        call 0x150f:0x1a8e
00001A2D  59                pop cx
00001A2E  0AC0              or al,al
00001A30  7502              jnz 0x1a34
00001A32  EB18              jmp short 0x1a4c
00001A34  A0AA37            mov al,[0x37aa]
00001A37  98                cbw
00001A38  8A56FE            mov dl,[bp-0x2]
00001A3B  8BD8              mov bx,ax
00001A3D  8897E23C          mov [bx+0x3ce2],dl
00001A41  A0AA37            mov al,[0x37aa]
00001A44  98                cbw
00001A45  8BD8              mov bx,ax
00001A47  C6875C4500        mov byte [bx+0x455c],0x0
00001A4C  46                inc si
00001A4D  83FE03            cmp si,byte +0x3
00001A50  7F03              jg 0x1a55
00001A52  E945FF            jmp 0x199a
00001A55  EB31              jmp short 0x1a88
00001A57  A0D42D            mov al,[0x2dd4]
00001A5A  3A06AA37          cmp al,[0x37aa]
00001A5E  7428              jz 0x1a88
00001A60  A0D42D            mov al,[0x2dd4]
00001A63  98                cbw
00001A64  50                push ax
00001A65  9A8E1A0F15        call 0x150f:0x1a8e
00001A6A  59                pop cx
00001A6B  3C01              cmp al,0x1
00001A6D  7519              jnz 0x1a88
00001A6F  A0AA37            mov al,[0x37aa]
00001A72  98                cbw
00001A73  8A16D42D          mov dl,[0x2dd4]
00001A77  8BD8              mov bx,ax
00001A79  8897E23C          mov [bx+0x3ce2],dl
00001A7D  A0AA37            mov al,[0x37aa]
00001A80  98                cbw
00001A81  8BD8              mov bx,ax
00001A83  C6875C4500        mov byte [bx+0x455c],0x0
00001A88  5F                pop di
00001A89  5E                pop si
00001A8A  8BE5              mov sp,bp
00001A8C  5D                pop bp
00001A8D  CB                retf

; ---- 150F:1A8E ----
00001A8E  55                push bp
00001A8F  8BEC              mov bp,sp
00001A91  56                push si
00001A92  57                push di
00001A93  8B7E06            mov di,[bp+0x6]
00001A96  80BDB64B01        cmp byte [di+0x4bb6],0x1
00001A9B  7504              jnz 0x1aa1
00001A9D  B000              mov al,0x0
00001A9F  EB33              jmp short 0x1ad4
00001AA1  A0AA37            mov al,[0x37aa]
00001AA4  98                cbw
00001AA5  D1E0              shl ax,1
00001AA7  D1E0              shl ax,1
00001AA9  8BD8              mov bx,ax
00001AAB  8A81B337          mov al,[bx+di+0x37b3]
00001AAF  98                cbw
00001AB0  8BF0              mov si,ax
00001AB2  0BF6              or si,si
00001AB4  7504              jnz 0x1aba
00001AB6  B000              mov al,0x0
00001AB8  EB1A              jmp short 0x1ad4
00001ABA  83FE01            cmp si,byte +0x1
00001ABD  7511              jnz 0x1ad0
00001ABF  A0AA37            mov al,[0x37aa]
00001AC2  98                cbw
00001AC3  8BD8              mov bx,ax
00001AC5  80BF7C3700        cmp byte [bx+0x377c],0x0
00001ACA  7504              jnz 0x1ad0
00001ACC  B000              mov al,0x0
00001ACE  EB04              jmp short 0x1ad4
00001AD0  B001              mov al,0x1
00001AD2  EB00              jmp short 0x1ad4
00001AD4  5F                pop di
00001AD5  5E                pop si
00001AD6  5D                pop bp
00001AD7  CB                retf

; ---- 150F:1AD8 ----
00001AD8  55                push bp
00001AD9  8BEC              mov bp,sp
00001ADB  4C                dec sp
00001ADC  4C                dec sp
00001ADD  56                push si
00001ADE  57                push di
00001ADF  33F6              xor si,si
00001AE1  33FF              xor di,di
00001AE3  EB13              jmp short 0x1af8
00001AE5  8B4606            mov ax,[bp+0x6]
00001AE8  BA0700            mov dx,0x7
00001AEB  F7EA              imul dx
00001AED  8BD8              mov bx,ax
00001AEF  80B9FE43FF        cmp byte [bx+di+0x43fe],0xff
00001AF4  7401              jz 0x1af7
00001AF6  46                inc si
00001AF7  47                inc di
00001AF8  83FF07            cmp di,byte +0x7
00001AFB  7CE8              jl 0x1ae5
00001AFD  8B5E06            mov bx,[bp+0x6]
00001B00  D1E3              shl bx,1
00001B02  8BC6              mov ax,si
00001B04  BA0A00            mov dx,0xa
00001B07  F7EA              imul dx
00001B09  3987F82E          cmp [bx+0x2ef8],ax
00001B0D  7D12              jnl 0x1b21
00001B0F  8B5E06            mov bx,[bp+0x6]
00001B12  D1E3              shl bx,1
00001B14  8B87F82E          mov ax,[bx+0x2ef8]
00001B18  BB0A00            mov bx,0xa
00001B1B  33D2              xor dx,dx
00001B1D  F7F3              div bx
00001B1F  8BF0              mov si,ax
00001B21  8B5E06            mov bx,[bp+0x6]
00001B24  8A87012F          mov al,[bx+0x2f01]
00001B28  98                cbw
00001B29  8BC8              mov cx,ax
00001B2B  83F9FF            cmp cx,byte -0x1
00001B2E  750D              jnz 0x1b3d
00001B30  8BC6              mov ax,si
00001B32  BB0500            mov bx,0x5
00001B35  99                cwd
00001B36  F7FB              idiv bx
00001B38  8946FE            mov [bp-0x2],ax
00001B3B  EB0E              jmp short 0x1b4b
00001B3D  8BC1              mov ax,cx
00001B3F  40                inc ax
00001B40  F7EE              imul si
00001B42  BB0400            mov bx,0x4
00001B45  99                cwd
00001B46  F7FB              idiv bx
00001B48  8946FE            mov [bp-0x2],ax
00001B4B  8B46FE            mov ax,[bp-0x2]
00001B4E  EB00              jmp short 0x1b50
00001B50  5F                pop di
00001B51  5E                pop si
00001B52  8BE5              mov sp,bp
00001B54  5D                pop bp
00001B55  CB                retf

; ---- 150F:1B56 ----
00001B56  55                push bp
00001B57  8BEC              mov bp,sp
00001B59  A0AA37            mov al,[0x37aa]
00001B5C  98                cbw
00001B5D  D1E0              shl ax,1
00001B5F  D1E0              shl ax,1
00001B61  8BD8              mov bx,ax
00001B63  83BF6C3705        cmp word [bx+0x376c],byte +0x5
00001B68  770E              ja 0x1b78
00001B6A  A0AA37            mov al,[0x37aa]
00001B6D  98                cbw
00001B6E  8BD8              mov bx,ax
00001B70  C687E74000        mov byte [bx+0x40e7],0x0
00001B75  E98400            jmp 0x1bfc
00001B78  A0AA37            mov al,[0x37aa]
00001B7B  98                cbw
00001B7C  BA0600            mov dx,0x6
00001B7F  F7EA              imul dx
00001B81  8BD8              mov bx,ax
00001B83  8A87D043          mov al,[bx+0x43d0]
00001B87  B400              mov ah,0x0
00001B89  BB0A00            mov bx,0xa
00001B8C  99                cwd
00001B8D  F7FB              idiv bx
00001B8F  D1E0              shl ax,1
00001B91  D1E0              shl ax,1
00001B93  50                push ax
00001B94  A0AA37            mov al,[0x37aa]
00001B97  98                cbw
00001B98  BA0600            mov dx,0x6
00001B9B  F7EA              imul dx
00001B9D  8BD8              mov bx,ax
00001B9F  8A87D143          mov al,[bx+0x43d1]
00001BA3  B400              mov ah,0x0
00001BA5  BB0A00            mov bx,0xa
00001BA8  99                cwd
00001BA9  F7FB              idiv bx
00001BAB  8BD0              mov dx,ax
00001BAD  58                pop ax
00001BAE  03C2              add ax,dx
00001BB0  50                push ax
00001BB1  A0AA37            mov al,[0x37aa]
00001BB4  98                cbw
00001BB5  BA0600            mov dx,0x6
00001BB8  F7EA              imul dx
00001BBA  8BD8              mov bx,ax
00001BBC  8A87D243          mov al,[bx+0x43d2]
00001BC0  B400              mov ah,0x0
00001BC2  BB0A00            mov bx,0xa
00001BC5  99                cwd
00001BC6  F7FB              idiv bx
00001BC8  8BD0              mov dx,ax
00001BCA  58                pop ax
00001BCB  03C2              add ax,dx
00001BCD  50                push ax
00001BCE  A0AA37            mov al,[0x37aa]
00001BD1  98                cbw
00001BD2  BA0600            mov dx,0x6
00001BD5  F7EA              imul dx
00001BD7  8BD8              mov bx,ax
00001BD9  8A87D343          mov al,[bx+0x43d3]
00001BDD  B400              mov ah,0x0
00001BDF  BB0A00            mov bx,0xa
00001BE2  99                cwd
00001BE3  F7FB              idiv bx
00001BE5  8BD0              mov dx,ax
00001BE7  58                pop ax
00001BE8  03C2              add ax,dx
00001BEA  BB0700            mov bx,0x7
00001BED  99                cwd
00001BEE  F7FB              idiv bx
00001BF0  50                push ax
00001BF1  A0AA37            mov al,[0x37aa]
00001BF4  98                cbw
00001BF5  8BD8              mov bx,ax
00001BF7  58                pop ax
00001BF8  8887E740          mov [bx+0x40e7],al
00001BFC  5D                pop bp
00001BFD  CB                retf

; ---- 150F:1BFE ----
00001BFE  55                push bp
00001BFF  8BEC              mov bp,sp
00001C01  A0AA37            mov al,[0x37aa]
00001C04  98                cbw
00001C05  8BD8              mov bx,ax
00001C07  80BFB64B01        cmp byte [bx+0x4bb6],0x1
00001C0C  7502              jnz 0x1c10
00001C0E  EB6C              jmp short 0x1c7c
00001C10  B100              mov cl,0x0
00001C12  EB63              jmp short 0x1c77
00001C14  8AC1              mov al,cl
00001C16  3A06AA37          cmp al,[0x37aa]
00001C1A  7459              jz 0x1c75
00001C1C  8AC1              mov al,cl
00001C1E  98                cbw
00001C1F  8BD8              mov bx,ax
00001C21  8A87143D          mov al,[bx+0x3d14]
00001C25  3A06AA37          cmp al,[0x37aa]
00001C29  7402              jz 0x1c2d
00001C2B  EB48              jmp short 0x1c75
00001C2D  8AC1              mov al,cl
00001C2F  98                cbw
00001C30  8BD8              mov bx,ax
00001C32  8A87E740          mov al,[bx+0x40e7]
00001C36  98                cbw
00001C37  50                push ax
00001C38  A0AA37            mov al,[0x37aa]
00001C3B  98                cbw
00001C3C  8BD8              mov bx,ax
00001C3E  8A87E740          mov al,[bx+0x40e7]
00001C42  98                cbw
00001C43  40                inc ax
00001C44  5A                pop dx
00001C45  3BD0              cmp dx,ax
00001C47  7E13              jng 0x1c5c
00001C49  A0AA37            mov al,[0x37aa]
00001C4C  98                cbw
00001C4D  D1E0              shl ax,1
00001C4F  D1E0              shl ax,1
00001C51  8BD8              mov bx,ax
00001C53  83BF6E3700        cmp word [bx+0x376e],byte +0x0
00001C58  7702              ja 0x1c5c
00001C5A  EB19              jmp short 0x1c75
00001C5C  A0AA37            mov al,[0x37aa]
00001C5F  98                cbw
00001C60  8AD1              mov dl,cl
00001C62  8BD8              mov bx,ax
00001C64  8897C73D          mov [bx+0x3dc7],dl
00001C68  A0AA37            mov al,[0x37aa]
00001C6B  98                cbw
00001C6C  8BD8              mov bx,ax
00001C6E  C6                db 0xc6
00001C6F  87                db 0x87
