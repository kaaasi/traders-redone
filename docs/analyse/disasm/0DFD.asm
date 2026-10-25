; segment 0DFD  file offset 0x115D0  size 0x340
00000000  008BF08B          add [bp+di-0x7410],cl
00000004  C6                db 0xc6
00000005  EB00              jmp short 0x7
00000007  5F                pop di
00000008  5E                pop si
00000009  8BE5              mov sp,bp
0000000B  5D                pop bp
0000000C  CB                retf

; ---- 0DFD:000D ----
0000000D  55                push bp
0000000E  8BEC              mov bp,sp
00000010  83EC04            sub sp,byte +0x4
00000013  9A7400FD0D        call 0xdfd:0x74
00000018  9A3201FD0D        call 0xdfd:0x132
0000001D  9A0D00AF21        call 0x21af:0xd
00000022  9A5801FD0D        call 0xdfd:0x158
00000027  33C0              xor ax,ax
00000029  50                push ax
0000002A  9ADB05FD21        call 0x21fd:0x5db
0000002F  59                pop cx
00000030  9AB6182D19        call 0x192d:0x18b6
00000035  1E                push ds
00000036  B87806            mov ax,0x678
00000039  50                push ax
0000003A  FF36C84B          push word [0x4bc8]
0000003E  FF36C64B          push word [0x4bc6]
00000042  9AB10A6C18        call 0x186c:0xab1
00000047  83C408            add sp,byte +0x8
0000004A  8946FC            mov [bp-0x4],ax
0000004D  8956FE            mov [bp-0x2],dx
00000050  FF76FE            push word [bp-0x2]
00000053  FF76FC            push word [bp-0x4]
00000056  FF36C84B          push word [0x4bc8]
0000005A  FF36C64B          push word [0x4bc6]
0000005E  9AA9016C18        call 0x186c:0x1a9
00000063  83C408            add sp,byte +0x8
00000066  9A3902FD0D        call 0xdfd:0x239
0000006B  9A8706FD21        call 0x21fd:0x687
00000070  8BE5              mov sp,bp
00000072  5D                pop bp
00000073  CB                retf

; ---- 0DFD:0074 ----
00000074  55                push bp
00000075  8BEC              mov bp,sp
00000077  83EC04            sub sp,byte +0x4
0000007A  833E0E0901        cmp word [0x90e],byte +0x1
0000007F  750C              jnz 0x8d
00000081  C746FC1443        mov word [bp-0x4],0x4314
00000086  C746FE0100        mov word [bp-0x2],0x1
0000008B  EB0A              jmp short 0x97
0000008D  C746FC14A0        mov word [bp-0x4],0xa014
00000092  C746FE0000        mov word [bp-0x2],0x0
00000097  FF76FE            push word [bp-0x2]
0000009A  FF76FC            push word [bp-0x4]
0000009D  9AC6026C18        call 0x186c:0x2c6
000000A2  59                pop cx
000000A3  59                pop cx
000000A4  A3D645            mov [0x45d6],ax
000000A7  8916D845          mov [0x45d8],dx
000000AB  33C0              xor ax,ax
000000AD  BA501F            mov dx,0x1f50
000000B0  50                push ax
000000B1  52                push dx
000000B2  9AC6026C18        call 0x186c:0x2c6
000000B7  59                pop cx
000000B8  59                pop cx
000000B9  A3C64B            mov [0x4bc6],ax
000000BC  8916C84B          mov [0x4bc8],dx
000000C0  33C0              xor ax,ax
000000C2  BAA411            mov dx,0x11a4
000000C5  50                push ax
000000C6  52                push dx
000000C7  9AC6026C18        call 0x186c:0x2c6
000000CC  59                pop cx
000000CD  59                pop cx
000000CE  A38448            mov [0x4884],ax
000000D1  89168648          mov [0x4886],dx
000000D5  33C0              xor ax,ax
000000D7  BA7608            mov dx,0x876
000000DA  50                push ax
000000DB  52                push dx
000000DC  9AC6026C18        call 0x186c:0x2c6
000000E1  59                pop cx
000000E2  59                pop cx
000000E3  A3AA3C            mov [0x3caa],ax
000000E6  8916AC3C          mov [0x3cac],dx
000000EA  C706DE400100      mov word [0x40de],0x1
000000F0  C706E0400000      mov word [0x40e0],0x0
000000F6  C706E2400200      mov word [0x40e2],0x2
000000FC  C706E4400000      mov word [0x40e4],0x0
00000102  B8FFFF            mov ax,0xffff
00000105  50                push ax
00000106  8B1E3008          mov bx,[0x830]
0000010A  D1E3              shl bx,1
0000010C  D1E3              shl bx,1
0000010E  FFB7E040          push word [bx+0x40e0]
00000112  FFB7DE40          push word [bx+0x40de]
00000116  8B1E2E08          mov bx,[0x82e]
0000011A  D1E3              shl bx,1
0000011C  D1E3              shl bx,1
0000011E  FFB7E040          push word [bx+0x40e0]
00000122  FFB7DE40          push word [bx+0x40de]
00000126  9A3802670B        call 0xb67:0x238
0000012B  83C40A            add sp,byte +0xa
0000012E  8BE5              mov sp,bp
00000130  5D                pop bp
00000131  CB                retf

; ---- 0DFD:0132 ----
00000132  55                push bp
00000133  8BEC              mov bp,sp
00000135  9A1F05FD21        call 0x21fd:0x51f
0000013A  A1C84B            mov ax,[0x4bc8]
0000013D  8B16C64B          mov dx,[0x4bc6]
00000141  8916DE44          mov [0x44de],dx
00000145  A3E044            mov [0x44e0],ax
00000148  A1D845            mov ax,[0x45d8]
0000014B  8B16D645          mov dx,[0x45d6]
0000014F  8916BC45          mov [0x45bc],dx
00000153  A3BE45            mov [0x45be],ax
00000156  5D                pop bp
00000157  CB                retf

; ---- 0DFD:0158 ----
00000158  55                push bp
00000159  8BEC              mov bp,sp
0000015B  83EC04            sub sp,byte +0x4
0000015E  56                push si
0000015F  1E                push ds
00000160  B88106            mov ax,0x681
00000163  50                push ax
00000164  FF36AC3C          push word [0x3cac]
00000168  FF36AA3C          push word [0x3caa]
0000016C  9AB10A6C18        call 0x186c:0xab1
00000171  83C408            add sp,byte +0x8
00000174  8946FC            mov [bp-0x4],ax
00000177  8956FE            mov [bp-0x2],dx
0000017A  FF76FE            push word [bp-0x2]
0000017D  FF76FC            push word [bp-0x4]
00000180  FF36AC3C          push word [0x3cac]
00000184  FF36AA3C          push word [0x3caa]
00000188  9AA9016C18        call 0x186c:0x1a9
0000018D  83C408            add sp,byte +0x8
00000190  33F6              xor si,si
00000192  EB6D              jmp short 0x201
00000194  8BDE              mov bx,si
00000196  D1E3              shl bx,1
00000198  8B876006          mov ax,[bx+0x660]
0000019C  D1E0              shl ax,1
0000019E  8B16AC3C          mov dx,[0x3cac]
000001A2  8B1EAA3C          mov bx,[0x3caa]
000001A6  03D8              add bx,ax
000001A8  8BC6              mov ax,si
000001AA  B90A00            mov cx,0xa
000001AD  52                push dx
000001AE  F7E9              imul cx
000001B0  87D8              xchg bx,ax
000001B2  8987F02B          mov [bx+0x2bf0],ax
000001B6  58                pop ax
000001B7  8987F22B          mov [bx+0x2bf2],ax
000001BB  8BDE              mov bx,si
000001BD  D1E3              shl bx,1
000001BF  8B876606          mov ax,[bx+0x666]
000001C3  50                push ax
000001C4  8BC6              mov ax,si
000001C6  BA0A00            mov dx,0xa
000001C9  F7EA              imul dx
000001CB  8BD8              mov bx,ax
000001CD  58                pop ax
000001CE  8987F42B          mov [bx+0x2bf4],ax
000001D2  8BDE              mov bx,si
000001D4  D1E3              shl bx,1
000001D6  8B876C06          mov ax,[bx+0x66c]
000001DA  50                push ax
000001DB  8BC6              mov ax,si
000001DD  BA0A00            mov dx,0xa
000001E0  F7EA              imul dx
000001E2  8BD8              mov bx,ax
000001E4  58                pop ax
000001E5  8987F62B          mov [bx+0x2bf6],ax
000001E9  8BDE              mov bx,si
000001EB  D1E3              shl bx,1
000001ED  8B877206          mov ax,[bx+0x672]
000001F1  50                push ax
000001F2  8BC6              mov ax,si
000001F4  BA0A00            mov dx,0xa
000001F7  F7EA              imul dx
000001F9  8BD8              mov bx,ax
000001FB  58                pop ax
000001FC  8987F82B          mov [bx+0x2bf8],ax
00000200  46                inc si
00000201  83FE03            cmp si,byte +0x3
00000204  7C8E              jl 0x194
00000206  FF36E044          push word [0x44e0]
0000020A  FF36DE44          push word [0x44de]
0000020E  FF361845          push word [0x4518]
00000212  FF361645          push word [0x4516]
00000216  FF36BE45          push word [0x45be]
0000021A  FF36BC45          push word [0x45bc]
0000021E  1E                push ds
0000021F  B8F02B            mov ax,0x2bf0
00000222  50                push ax
00000223  9A8A05CD23        call 0x23cd:0x58a
00000228  83C410            add sp,byte +0x10
0000022B  33C0              xor ax,ax
0000022D  50                push ax
0000022E  9AB605CD23        call 0x23cd:0x5b6
00000233  59                pop cx
00000234  5E                pop si
00000235  8BE5              mov sp,bp
00000237  5D                pop bp
00000238  CB                retf

; ---- 0DFD:0239 ----
00000239  55                push bp
0000023A  8BEC              mov bp,sp
0000023C  33C0              xor ax,ax
0000023E  50                push ax
0000023F  9AB403FD21        call 0x21fd:0x3b4
00000244  59                pop cx
00000245  B84504            mov ax,0x445
00000248  50                push ax
00000249  B80100            mov ax,0x1
0000024C  50                push ax
0000024D  9AC902FD21        call 0x21fd:0x2c9
00000252  59                pop cx
00000253  59                pop cx
00000254  B82201            mov ax,0x122
00000257  50                push ax
00000258  B80200            mov ax,0x2
0000025B  50                push ax
0000025C  9AC902FD21        call 0x21fd:0x2c9
00000261  59                pop cx
00000262  59                pop cx
00000263  B80007            mov ax,0x700
00000266  50                push ax
00000267  B80300            mov ax,0x3
0000026A  50                push ax
0000026B  9AC902FD21        call 0x21fd:0x2c9
00000270  59                pop cx
00000271  59                pop cx
00000272  B80100            mov ax,0x1
00000275  50                push ax
00000276  9ADB05FD21        call 0x21fd:0x5db
0000027B  59                pop cx
0000027C  9A1B00960A        call 0xa96:0x1b
00000281  B8FFFF            mov ax,0xffff
00000284  50                push ax
00000285  B80100            mov ax,0x1
00000288  50                push ax
00000289  1E                push ds
0000028A  B88A06            mov ax,0x68a
0000028D  50                push ax
0000028E  B84001            mov ax,0x140
00000291  50                push ax
00000292  B82800            mov ax,0x28
00000295  50                push ax
00000296  33C0              xor ax,ax
00000298  50                push ax
00000299  9A9000690A        call 0xa69:0x90
0000029E  83C40E            add sp,byte +0xe
000002A1  9A0D06FD21        call 0x21fd:0x60d
000002A6  33C0              xor ax,ax
000002A8  50                push ax
000002A9  9ADB05FD21        call 0x21fd:0x5db
000002AE  59                pop cx
000002AF  9A5303FD21        call 0x21fd:0x353
000002B4  9A1B00960A        call 0xa96:0x1b
000002B9  33C0              xor ax,ax
000002BB  50                push ax
000002BC  B80500            mov ax,0x5
000002BF  50                push ax
000002C0  B84001            mov ax,0x140
000002C3  50                push ax
000002C4  B89200            mov ax,0x92
000002C7  50                push ax
000002C8  33C0              xor ax,ax
000002CA  50                push ax
000002CB  9AD704960A        call 0xa96:0x4d7
000002D0  83C40A            add sp,byte +0xa
000002D3  B8FFFF            mov ax,0xffff
000002D6  50                push ax
000002D7  B80200            mov ax,0x2
000002DA  50                push ax
000002DB  1E                push ds
000002DC  B89E06            mov ax,0x69e
000002DF  50                push ax
000002E0  B84001            mov ax,0x140
000002E3  50                push ax
000002E4  B89600            mov ax,0x96
000002E7  50                push ax
000002E8  33C0              xor ax,ax
000002EA  50                push ax
000002EB  9A9000690A        call 0xa69:0x90
000002F0  83C40E            add sp,byte +0xe
000002F3  9A0D06FD21        call 0x21fd:0x60d
000002F8  33C0              xor ax,ax
000002FA  50                push ax
000002FB  33C0              xor ax,ax
000002FD  50                push ax
000002FE  9A94008B21        call 0x218b:0x94
00000303  59                pop cx
00000304  59                pop cx
00000305  FF36CE08          push word [0x8ce]
00000309  9A02006C18        call 0x186c:0x2
0000030E  59                pop cx
0000030F  FF36D008          push word [0x8d0]
00000313  9A02006C18        call 0x186c:0x2
00000318  59                pop cx
00000319  FF36D608          push word [0x8d6]
0000031D  9A02006C18        call 0x186c:0x2
00000322  59                pop cx
00000323  FF36D808          push word [0x8d8]
00000327  9A02006C18        call 0x186c:0x2
0000032C  59                pop cx
0000032D  FF36D408          push word [0x8d4]
00000331  9A02006C18        call 0x186c:0x2
00000336  59                pop cx
00000337  FF36D208          push word [0x8d2]
0000033B  9A02006C18        call 0x186c:0x2
