; segment 2295  file offset 0x25F50  size 0xE0
00000000  CB                retf

; ---- 2295:0001 ----
00000001  55                push bp
00000002  8BEC              mov bp,sp
00000004  83EC16            sub sp,byte +0x16
00000007  56                push si
00000008  57                push di
00000009  8B7E0A            mov di,[bp+0xa]
0000000C  FF7608            push word [bp+0x8]
0000000F  FF7606            push word [bp+0x6]
00000012  57                push di
00000013  1E                push ds
00000014  B82711            mov ax,0x1127
00000017  50                push ax
00000018  16                push ss
00000019  8D46EA            lea ax,[bp-0x16]
0000001C  50                push ax
0000001D  9A82330000        call 0x0:0x3382
00000022  83C40E            add sp,byte +0xe
00000025  33F6              xor si,si
00000027  EB17              jmp short 0x40
00000029  807AEA20          cmp byte [bp+si-0x16],0x20
0000002D  7406              jz 0x35
0000002F  807AEA30          cmp byte [bp+si-0x16],0x30
00000033  7508              jnz 0x3d
00000035  A02611            mov al,[0x1126]
00000038  8842EA            mov [bp+si-0x16],al
0000003B  EB02              jmp short 0x3f
0000003D  EB08              jmp short 0x47
0000003F  46                inc si
00000040  8BC7              mov ax,di
00000042  48                dec ax
00000043  3BC6              cmp ax,si
00000045  7FE2              jg 0x29
00000047  33F6              xor si,si
00000049  EB0D              jmp short 0x58
0000004B  8A42EA            mov al,[bp+si-0x16]
0000004E  C45E0C            les bx,[bp+0xc]
00000051  268807            mov [es:bx],al
00000054  FF460C            inc word [bp+0xc]
00000057  46                inc si
00000058  3BF7              cmp si,di
0000005A  7CEF              jl 0x4b
0000005C  5F                pop di
0000005D  5E                pop si
0000005E  8BE5              mov sp,bp
00000060  5D                pop bp
00000061  CB                retf

; ---- 2295:0062 ----
00000062  55                push bp
00000063  8BEC              mov bp,sp
00000065  8A4606            mov al,[bp+0x6]
00000068  A22611            mov [0x1126],al
0000006B  5D                pop bp
0000006C  CB                retf

; ---- 2295:006D ----
0000006D  55                push bp
0000006E  8BEC              mov bp,sp
00000070  83EC04            sub sp,byte +0x4
00000073  8B4608            mov ax,[bp+0x8]
00000076  B108              mov cl,0x8
00000078  D3E8              shr ax,cl
0000007A  D0E0              shl al,1
0000007C  D0E0              shl al,1
0000007E  D0E0              shl al,1
00000080  8846FC            mov [bp-0x4],al
00000083  8B4608            mov ax,[bp+0x8]
00000086  25F000            and ax,0xf0
00000089  BB1000            mov bx,0x10
0000008C  99                cwd
0000008D  F7FB              idiv bx
0000008F  D0E0              shl al,1
00000091  D0E0              shl al,1
00000093  D0E0              shl al,1
00000095  8846FD            mov [bp-0x3],al
00000098  8A4608            mov al,[bp+0x8]
0000009B  240F              and al,0xf
0000009D  D0E0              shl al,1
0000009F  D0E0              shl al,1
000000A1  D0E0              shl al,1
000000A3  8846FE            mov [bp-0x2],al
000000A6  833E0E0901        cmp word [0x90e],byte +0x1
000000AB  7514              jnz 0xc1
000000AD  8CD0              mov ax,ss
000000AF  8EC0              mov es,ax
000000B1  8D46FC            lea ax,[bp-0x4]
000000B4  8BD0              mov dx,ax
000000B6  B90100            mov cx,0x1
000000B9  8B5E06            mov bx,[bp+0x6]
000000BC  9A2E04C627        call 0x27c6:0x42e
000000C1  8BE5              mov sp,bp
000000C3  5D                pop bp
000000C4  CB                retf

; ---- 2295:00C5 ----
000000C5  55                push bp
000000C6  8BEC              mov bp,sp
000000C8  9AD300730D        call 0xd73:0xd3
000000CD  833E0E0901        cmp word [0x90e],byte +0x1
000000D2  750D              jnz 0xe1
000000D4  C45606            les dx,[bp+0x6]
000000D7  B90001            mov cx,0x100
000000DA  33DB              xor bx,bx
000000DC  9A                db 0x9a
000000DD  2E04C6            cs add al,0xc6
