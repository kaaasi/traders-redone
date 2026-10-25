; segment 218B  file offset 0x24EB0  size 0x1C0
00000000  5D                pop bp
00000001  CB                retf

; ---- 218B:0002 ----
00000002  55                push bp
00000003  8BEC              mov bp,sp
00000005  83EC04            sub sp,byte +0x4
00000008  56                push si
00000009  833E100901        cmp word [0x910],byte +0x1
0000000E  7404              jz 0x14
00000010  33C0              xor ax,ax
00000012  EB7B              jmp short 0x8f
00000014  B86400            mov ax,0x64
00000017  50                push ax
00000018  9A913B0000        call 0x0:0x3b91
0000001D  59                pop cx
0000001E  0BC2              or ax,dx
00000020  7504              jnz 0x26
00000022  33C0              xor ax,ax
00000024  EB69              jmp short 0x8f
00000026  B80600            mov ax,0x6
00000029  33DB              xor bx,bx
0000002B  CD64              int 0x64
0000002D  B80500            mov ax,0x5
00000030  33DB              xor bx,bx
00000032  CD64              int 0x64
00000034  B80100            mov ax,0x1
00000037  BA6020            mov dx,0x2060
0000003A  50                push ax
0000003B  52                push dx
0000003C  9AC6026C18        call 0x186c:0x2c6
00000041  59                pop cx
00000042  59                pop cx
00000043  8946FC            mov [bp-0x4],ax
00000046  8956FE            mov [bp-0x2],dx
00000049  33C9              xor cx,cx
0000004B  33DB              xor bx,bx
0000004D  8B56FE            mov dx,[bp-0x2]
00000050  8B46FC            mov ax,[bp-0x4]
00000053  9A654F0000        call 0x0:0x4f65
00000058  7504              jnz 0x5e
0000005A  33C0              xor ax,ax
0000005C  EB31              jmp short 0x8f
0000005E  33F6              xor si,si
00000060  EB1D              jmp short 0x7f
00000062  8B4EFE            mov cx,[bp-0x2]
00000065  8B56FC            mov dx,[bp-0x4]
00000068  8BDE              mov bx,si
0000006A  B81000            mov ax,0x10
0000006D  CD64              int 0x64
0000006F  46                inc si
00000070  33C9              xor cx,cx
00000072  BB040C            mov bx,0xc04
00000075  8CD2              mov dx,ss
00000077  8D46FC            lea ax,[bp-0x4]
0000007A  9A79500000        call 0x0:0x5079
0000007F  83FE18            cmp si,byte +0x18
00000082  7CDE              jl 0x62
00000084  C706A3100100      mov word [0x10a3],0x1
0000008A  B80100            mov ax,0x1
0000008D  EB00              jmp short 0x8f
0000008F  5E                pop si
00000090  8BE5              mov sp,bp
00000092  5D                pop bp
00000093  CB                retf

; ---- 218B:0094 ----
00000094  55                push bp
00000095  8BEC              mov bp,sp
00000097  56                push si
00000098  8B7606            mov si,[bp+0x6]
0000009B  833EA71000        cmp word [0x10a7],byte +0x0
000000A0  7503              jnz 0xa5
000000A2  E99200            jmp 0x137
000000A5  833EA31000        cmp word [0x10a3],byte +0x0
000000AA  7503              jnz 0xaf
000000AC  E98800            jmp 0x137
000000AF  9A53018B21        call 0x218b:0x153
000000B4  3936A910          cmp [0x10a9],si
000000B8  7438              jz 0xf2
000000BA  8A84A010          mov al,[si+0x10a0]
000000BE  98                cbw
000000BF  50                push ax
000000C0  8BDE              mov bx,si
000000C2  D1E3              shl bx,1
000000C4  D1E3              shl bx,1
000000C6  FFB79610          push word [bx+0x1096]
000000CA  FFB79410          push word [bx+0x1094]
000000CE  9A6C0A6C18        call 0x186c:0xa6c
000000D3  83C406            add sp,byte +0x6
000000D6  8BDE              mov bx,si
000000D8  D1E3              shl bx,1
000000DA  D1E3              shl bx,1
000000DC  8B8F9410          mov cx,[bx+0x1094]
000000E0  8BDE              mov bx,si
000000E2  D1E3              shl bx,1
000000E4  D1E3              shl bx,1
000000E6  8B9F9610          mov bx,[bx+0x1096]
000000EA  33C0              xor ax,ax
000000EC  CD64              int 0x64
000000EE  8936A910          mov [0x10a9],si
000000F2  F746080200        test word [bp+0x8],0x2
000000F7  740A              jz 0x103
000000F9  B81800            mov ax,0x18
000000FC  BB0100            mov bx,0x1
000000FF  CD64              int 0x64
00000101  EB07              jmp short 0x10a
00000103  B81800            mov ax,0x18
00000106  33DB              xor bx,bx
00000108  CD64              int 0x64
0000010A  F746080100        test word [bp+0x8],0x1
0000010F  740C              jz 0x11d
00000111  B80300            mov ax,0x3
00000114  50                push ax
00000115  9AA4018B21        call 0x218b:0x1a4
0000011A  59                pop cx
0000011B  EB0A              jmp short 0x127
0000011D  B87F00            mov ax,0x7f
00000120  50                push ax
00000121  9AA4018B21        call 0x218b:0x1a4
00000126  59                pop cx
00000127  B81A00            mov ax,0x1a
0000012A  CD64              int 0x64
0000012C  B80100            mov ax,0x1
0000012F  CD64              int 0x64
00000131  C706A5100100      mov word [0x10a5],0x1
00000137  5E                pop si
00000138  5D                pop bp
00000139  CB                retf

; ---- 218B:013A ----
0000013A  55                push bp
0000013B  8BEC              mov bp,sp
0000013D  833EA31000        cmp word [0x10a3],byte +0x0
00000142  7504              jnz 0x148
00000144  B000              mov al,0x0
00000146  EB09              jmp short 0x151
00000148  B81200            mov ax,0x12
0000014B  CD64              int 0x64
0000014D  8AC3              mov al,bl
0000014F  EB00              jmp short 0x151
00000151  5D                pop bp
00000152  CB                retf

; ---- 218B:0153 ----
00000153  55                push bp
00000154  8BEC              mov bp,sp
00000156  0E                push cs
00000157  E8E0FF            call 0x13a
0000015A  B400              mov ah,0x0
0000015C  0BC0              or ax,ax
0000015E  7502              jnz 0x162
00000160  EB0B              jmp short 0x16d
00000162  B80300            mov ax,0x3
00000165  CD64              int 0x64
00000167  C706A5100000      mov word [0x10a5],0x0
0000016D  5D                pop bp
0000016E  CB                retf
0000016F  55                push bp
00000170  8BEC              mov bp,sp
00000172  0E                push cs
00000173  E8C4FF            call 0x13a
00000176  B400              mov ah,0x0
00000178  0BC0              or ax,ax
0000017A  7502              jnz 0x17e
0000017C  EB15              jmp short 0x193
0000017E  B80300            mov ax,0x3
00000181  50                push ax
00000182  9AA4018B21        call 0x218b:0x1a4
00000187  59                pop cx
00000188  B81900            mov ax,0x19
0000018B  CD64              int 0x64
0000018D  C706A5100000      mov word [0x10a5],0x0
00000193  5D                pop bp
00000194  CB                retf
00000195  55                push bp
00000196  8BEC              mov bp,sp
00000198  EB00              jmp short 0x19a
0000019A  0E                push cs
0000019B  E89CFF            call 0x13a
0000019E  0AC0              or al,al
000001A0  75F8              jnz 0x19a
000001A2  5D                pop bp
000001A3  CB                retf

; ---- 218B:01A4 ----
000001A4  55                push bp
000001A5  8BEC              mov bp,sp
000001A7  8B5E06            mov bx,[bp+0x6]
000001AA  B81B00            mov ax,0x1b
000001AD  CD64              int 0x64
000001AF  5D                pop bp
000001B0  CB                retf
000001B1  55                push bp
000001B2  8BEC              mov bp,sp
000001B4  833EA31000        cmp word [0x10a3],byte +0x0
000001B9  7502              jnz 0x1bd
000001BB  EB0E              jmp short 0x1cb
000001BD  B80600            mov ax,0x6
