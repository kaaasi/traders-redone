; segment 0E9E  file offset 0x11FE0  size 0x400
00000000  F1                int1
00000001  010B              add [bp+di],cx
00000003  F67E03            idiv byte [bp+0x3]
00000006  E9B9FE            jmp 0xfec2
00000009  5E                pop si
0000000A  8BE5              mov sp,bp
0000000C  5D                pop bp
0000000D  CB                retf

; ---- 0E9E:000E ----
0000000E  55                push bp
0000000F  8BEC              mov bp,sp
00000011  9A35009E0E        call 0xe9e:0x35
00000016  C706102C0000      mov word [0x2c10],0x0
0000001C  EB0E              jmp short 0x2c
0000001E  9A64029E0E        call 0xe9e:0x264
00000023  9A12039E0E        call 0xe9e:0x312
00000028  FF06102C          inc word [0x2c10]
0000002C  833E102C04        cmp word [0x2c10],byte +0x4
00000031  7CEB              jl 0x1e
00000033  5D                pop bp
00000034  CB                retf

; ---- 0E9E:0035 ----
00000035  55                push bp
00000036  8BEC              mov bp,sp
00000038  83EC08            sub sp,byte +0x8
0000003B  56                push si
0000003C  57                push di
0000003D  C746FA0000        mov word [bp-0x6],0x0
00000042  E90402            jmp 0x249
00000045  8B5EFA            mov bx,[bp-0x6]
00000048  80BF400801        cmp byte [bx+0x840],0x1
0000004D  7503              jnz 0x52
0000004F  E9F401            jmp 0x246
00000052  C646FF00          mov byte [bp-0x1],0x0
00000056  8B5EFA            mov bx,[bp-0x6]
00000059  8A873C08          mov al,[bx+0x83c]
0000005D  98                cbw
0000005E  8BD8              mov bx,ax
00000060  83FB05            cmp bx,byte +0x5
00000063  7603              jna 0x68
00000065  E9D401            jmp 0x23c
00000068  D1E3              shl bx,1
0000006A  2EFFA75802        jmp [cs:bx+0x258]
0000006F  803E943D00        cmp byte [0x3d94],0x0
00000074  740A              jz 0x80
00000076  8A46FF            mov al,[bp-0x1]
00000079  FEC0              inc al
0000007B  8846FF            mov [bp-0x1],al
0000007E  EB0F              jmp short 0x8f
00000080  803E923D00        cmp byte [0x3d92],0x0
00000085  7408              jz 0x8f
00000087  8A46FF            mov al,[bp-0x1]
0000008A  0402              add al,0x2
0000008C  8846FF            mov [bp-0x1],al
0000008F  803E8F3D00        cmp byte [0x3d8f],0x0
00000094  740A              jz 0xa0
00000096  8A46FF            mov al,[bp-0x1]
00000099  0404              add al,0x4
0000009B  8846FF            mov [bp-0x1],al
0000009E  EB0F              jmp short 0xaf
000000A0  803E973D00        cmp byte [0x3d97],0x0
000000A5  7408              jz 0xaf
000000A7  8A46FF            mov al,[bp-0x1]
000000AA  0408              add al,0x8
000000AC  8846FF            mov [bp-0x1],al
000000AF  803E993D00        cmp byte [0x3d99],0x0
000000B4  7408              jz 0xbe
000000B6  8A46FF            mov al,[bp-0x1]
000000B9  0410              add al,0x10
000000BB  8846FF            mov [bp-0x1],al
000000BE  E97B01            jmp 0x23c
000000C1  803E743D00        cmp byte [0x3d74],0x0
000000C6  740A              jz 0xd2
000000C8  8A46FF            mov al,[bp-0x1]
000000CB  FEC0              inc al
000000CD  8846FF            mov [bp-0x1],al
000000D0  EB0F              jmp short 0xe1
000000D2  803E733D00        cmp byte [0x3d73],0x0
000000D7  7408              jz 0xe1
000000D9  8A46FF            mov al,[bp-0x1]
000000DC  0402              add al,0x2
000000DE  8846FF            mov [bp-0x1],al
000000E1  803E573D00        cmp byte [0x3d57],0x0
000000E6  740A              jz 0xf2
000000E8  8A46FF            mov al,[bp-0x1]
000000EB  0404              add al,0x4
000000ED  8846FF            mov [bp-0x1],al
000000F0  EB0F              jmp short 0x101
000000F2  803E653D00        cmp byte [0x3d65],0x0
000000F7  7408              jz 0x101
000000F9  8A46FF            mov al,[bp-0x1]
000000FC  0408              add al,0x8
000000FE  8846FF            mov [bp-0x1],al
00000101  803E803D00        cmp byte [0x3d80],0x0
00000106  7408              jz 0x110
00000108  8A46FF            mov al,[bp-0x1]
0000010B  0410              add al,0x10
0000010D  8846FF            mov [bp-0x1],al
00000110  E92901            jmp 0x23c
00000113  9AE007730D        call 0xd73:0x7e0
00000118  8846FF            mov [bp-0x1],al
0000011B  8A46FF            mov al,[bp-0x1]
0000011E  241F              and al,0x1f
00000120  8846FF            mov [bp-0x1],al
00000123  803EC53D00        cmp byte [0x3dc5],0x0
00000128  7408              jz 0x132
0000012A  8A46FF            mov al,[bp-0x1]
0000012D  0C10              or al,0x10
0000012F  8846FF            mov [bp-0x1],al
00000132  E90701            jmp 0x23c
00000135  8B5EFA            mov bx,[bp-0x6]
00000138  80BF3C0800        cmp byte [bx+0x83c],0x0
0000013D  7505              jnz 0x144
0000013F  BF0100            mov di,0x1
00000142  EB03              jmp short 0x147
00000144  BF0200            mov di,0x2
00000147  16                push ss
00000148  8D46FC            lea ax,[bp-0x4]
0000014B  50                push ax
0000014C  57                push di
0000014D  9A6802C627        call 0x27c6:0x268
00000152  83C406            add sp,byte +0x6
00000155  8946F8            mov [bp-0x8],ax
00000158  8A46FC            mov al,[bp-0x4]
0000015B  98                cbw
0000015C  99                cwd
0000015D  33C2              xor ax,dx
0000015F  2BC2              sub ax,dx
00000161  50                push ax
00000162  8A46FD            mov al,[bp-0x3]
00000165  98                cbw
00000166  99                cwd
00000167  33C2              xor ax,dx
00000169  2BC2              sub ax,dx
0000016B  5A                pop dx
0000016C  3BD0              cmp dx,ax
0000016E  7E20              jng 0x190
00000170  807EFC01          cmp byte [bp-0x4],0x1
00000174  7E0A              jng 0x180
00000176  8A46FF            mov al,[bp-0x1]
00000179  FEC0              inc al
0000017B  8846FF            mov [bp-0x1],al
0000017E  EB0E              jmp short 0x18e
00000180  807EFCFF          cmp byte [bp-0x4],0xff
00000184  7D08              jnl 0x18e
00000186  8A46FF            mov al,[bp-0x1]
00000189  0402              add al,0x2
0000018B  8846FF            mov [bp-0x1],al
0000018E  EB1E              jmp short 0x1ae
00000190  807EFD00          cmp byte [bp-0x3],0x0
00000194  7D0A              jnl 0x1a0
00000196  8A46FF            mov al,[bp-0x1]
00000199  0404              add al,0x4
0000019B  8846FF            mov [bp-0x1],al
0000019E  EB0E              jmp short 0x1ae
000001A0  807EFD01          cmp byte [bp-0x3],0x1
000001A4  7E08              jng 0x1ae
000001A6  8A46FF            mov al,[bp-0x1]
000001A9  0408              add al,0x8
000001AB  8846FF            mov [bp-0x1],al
000001AE  F746F80100        test word [bp-0x8],0x1
000001B3  7408              jz 0x1bd
000001B5  8A46FF            mov al,[bp-0x1]
000001B8  0410              add al,0x10
000001BA  8846FF            mov [bp-0x1],al
000001BD  EB7D              jmp short 0x23c
000001BF  B84000            mov ax,0x40
000001C2  8EC0              mov es,ax
000001C4  268B360800        mov si,[es:0x8]
000001C9  B001              mov al,0x1
000001CB  8A4EFA            mov cl,[bp-0x6]
000001CE  D2E0              shl al,cl
000001D0  34FF              xor al,0xff
000001D2  50                push ax
000001D3  56                push si
000001D4  9A97410000        call 0x0:0x4197
000001D9  59                pop cx
000001DA  59                pop cx
000001DB  8BC6              mov ax,si
000001DD  40                inc ax
000001DE  50                push ax
000001DF  9A633C0000        call 0x0:0x3c63
000001E4  59                pop cx
000001E5  8846FE            mov [bp-0x2],al
000001E8  F646FE80          test byte [bp-0x2],0x80
000001EC  7404              jz 0x1f2
000001EE  C646FF08          mov byte [bp-0x1],0x8
000001F2  8A46FE            mov al,[bp-0x2]
000001F5  B400              mov ah,0x0
000001F7  A94000            test ax,0x40
000001FA  7508              jnz 0x204
000001FC  8A46FF            mov al,[bp-0x1]
000001FF  0404              add al,0x4
00000201  8846FF            mov [bp-0x1],al
00000204  8A46FE            mov al,[bp-0x2]
00000207  B400              mov ah,0x0
00000209  A92000            test ax,0x20
0000020C  7508              jnz 0x216
0000020E  8A46FF            mov al,[bp-0x1]
00000211  0402              add al,0x2
00000213  8846FF            mov [bp-0x1],al
00000216  8A46FE            mov al,[bp-0x2]
00000219  B400              mov ah,0x0
0000021B  A91000            test ax,0x10
0000021E  7508              jnz 0x228
00000220  8A46FF            mov al,[bp-0x1]
00000223  FEC0              inc al
00000225  8846FF            mov [bp-0x1],al
00000228  8A46FE            mov al,[bp-0x2]
0000022B  B400              mov ah,0x0
0000022D  A90800            test ax,0x8
00000230  7508              jnz 0x23a
00000232  8A46FF            mov al,[bp-0x1]
00000235  0410              add al,0x10
00000237  8846FF            mov [bp-0x1],al
0000023A  EB00              jmp short 0x23c
0000023C  8B5EFA            mov bx,[bp-0x6]
0000023F  8A46FF            mov al,[bp-0x1]
00000242  8887C337          mov [bx+0x37c3],al
00000246  FF46FA            inc word [bp-0x6]
00000249  837EFA04          cmp word [bp-0x6],byte +0x4
0000024D  7D03              jnl 0x252
0000024F  E9F3FD            jmp 0x45
00000252  5F                pop di
00000253  5E                pop si
00000254  8BE5              mov sp,bp
00000256  5D                pop bp
00000257  CB                retf
00000258  350135            xor ax,0x3501
0000025B  0113              add [bp+di],dx
0000025D  01C1              add cx,ax
0000025F  006F00            add [bx+0x0],ch
00000262  BF0155            mov di,0x5501
00000265  8BEC              mov bp,sp
00000267  8B1E102C          mov bx,[0x2c10]
0000026B  8A87C337          mov al,[bx+0x37c3]
0000026F  240F              and al,0xf
00000271  8AD0              mov dl,al
00000273  0AD2              or dl,dl
00000275  7521              jnz 0x298
00000277  8B1E102C          mov bx,[0x2c10]
0000027B  80BF400800        cmp byte [bx+0x840],0x0
00000280  7516              jnz 0x298
00000282  8B1E102C          mov bx,[0x2c10]
00000286  D1E3              shl bx,1
00000288  D1E3              shl bx,1
0000028A  C78786370000      mov word [bx+0x3786],0x0
00000290  C78788370000      mov word [bx+0x3788],0x0
00000296  EB2C              jmp short 0x2c4
00000298  8B1E102C          mov bx,[0x2c10]
0000029C  D1E3              shl bx,1
0000029E  D1E3              shl bx,1
000002A0  83BF883700        cmp word [bx+0x3788],byte +0x0
000002A5  7C1D              jl 0x2c4
000002A7  7F07              jg 0x2b0
000002A9  83BF863700        cmp word [bx+0x3786],byte +0x0
000002AE  7614              jna 0x2c4
000002B0  8B1E102C          mov bx,[0x2c10]
000002B4  D1E3              shl bx,1
000002B6  D1E3              shl bx,1
000002B8  83AF863701        sub word [bx+0x3786],byte +0x1
000002BD  839F883700        sbb word [bx+0x3788],byte +0x0
000002C2  B200              mov dl,0x0
000002C4  F6C201            test dl,0x1
000002C7  7404              jz 0x2cd
000002C9  B001              mov al,0x1
000002CB  EB02              jmp short 0x2cf
000002CD  B000              mov al,0x0
000002CF  8B1E102C          mov bx,[0x2c10]
000002D3  8887BE44          mov [bx+0x44be],al
000002D7  F6C202            test dl,0x2
000002DA  7404              jz 0x2e0
000002DC  B001              mov al,0x1
000002DE  EB02              jmp short 0x2e2
000002E0  B000              mov al,0x0
000002E2  8B1E102C          mov bx,[0x2c10]
000002E6  8887F043          mov [bx+0x43f0],al
000002EA  F6C204            test dl,0x4
000002ED  7404              jz 0x2f3
000002EF  B001              mov al,0x1
000002F1  EB02              jmp short 0x2f5
000002F3  B000              mov al,0x0
000002F5  8B1E102C          mov bx,[0x2c10]
000002F9  88879D43          mov [bx+0x439d],al
000002FD  F6C208            test dl,0x8
00000300  7404              jz 0x306
00000302  B001              mov al,0x1
00000304  EB02              jmp short 0x308
00000306  B000              mov al,0x0
00000308  8B1E102C          mov bx,[0x2c10]
0000030C  8887BA44          mov [bx+0x44ba],al
00000310  5D                pop bp
00000311  CB                retf

; ---- 0E9E:0312 ----
00000312  55                push bp
00000313  8BEC              mov bp,sp
00000315  8B1E102C          mov bx,[0x2c10]
00000319  F687C33710        test byte [bx+0x37c3],0x10
0000031E  7404              jz 0x324
00000320  B001              mov al,0x1
00000322  EB02              jmp short 0x326
00000324  B000              mov al,0x0
00000326  8AD0              mov dl,al
00000328  0AD2              or dl,dl
0000032A  7516              jnz 0x342
0000032C  8B1E102C          mov bx,[0x2c10]
00000330  D1E3              shl bx,1
00000332  D1E3              shl bx,1
00000334  C7872E300000      mov word [bx+0x302e],0x0
0000033A  C78730300000      mov word [bx+0x3030],0x0
00000340  EB2C              jmp short 0x36e
00000342  8B1E102C          mov bx,[0x2c10]
00000346  D1E3              shl bx,1
00000348  D1E3              shl bx,1
0000034A  83BF303000        cmp word [bx+0x3030],byte +0x0
0000034F  7C1D              jl 0x36e
00000351  7F07              jg 0x35a
00000353  83BF2E3000        cmp word [bx+0x302e],byte +0x0
00000358  7614              jna 0x36e
0000035A  8B1E102C          mov bx,[0x2c10]
0000035E  D1E3              shl bx,1
00000360  D1E3              shl bx,1
00000362  83AF2E3001        sub word [bx+0x302e],byte +0x1
00000367  839F303000        sbb word [bx+0x3030],byte +0x0
0000036C  B200              mov dl,0x0
0000036E  8B1E102C          mov bx,[0x2c10]
00000372  8AC2              mov al,dl
00000374  88879448          mov [bx+0x4894],al
00000378  5D                pop bp
00000379  CB                retf

; ---- 0E9E:037A ----
0000037A  55                push bp
0000037B  8BEC              mov bp,sp
0000037D  4C                dec sp
0000037E  4C                dec sp
0000037F  56                push si
00000380  0E                push cs
00000381  E88AFC            call 0xe
00000384  33F6              xor si,si
00000386  EB77              jmp short 0x3ff
00000388  B8FFFF            mov ax,0xffff
0000038B  8946FE            mov [bp-0x2],ax
0000038E  88840833          mov [si+0x3308],al
00000392  80BC400801        cmp byte [si+0x840],0x1
00000397  7502              jnz 0x39b
00000399  EB63              jmp short 0x3fe
0000039B  80BC944800        cmp byte [si+0x4894],0x0
000003A0  7407              jz 0x3a9
000003A2  C746FE0A00        mov word [bp-0x2],0xa
000003A7  EB36              jmp short 0x3df
000003A9  80BC9D4300        cmp byte [si+0x439d],0x0
000003AE  7407              jz 0x3b7
000003B0  C746FE0200        mov word [bp-0x2],0x2
000003B5  EB28              jmp short 0x3df
000003B7  80BCBA4400        cmp byte [si+0x44ba],0x0
000003BC  7407              jz 0x3c5
000003BE  C746FE0300        mov word [bp-0x2],0x3
000003C3  EB1A              jmp short 0x3df
000003C5  80BCBE4400        cmp byte [si+0x44be],0x0
000003CA  7407              jz 0x3d3
000003CC  C746FE0000        mov word [bp-0x2],0x0
000003D1  EB0C              jmp short 0x3df
000003D3  80BCF04300        cmp byte [si+0x43f0],0x0
000003D8  7405              jz 0x3df
000003DA  C746FE0100        mov word [bp-0x2],0x1
000003DF  837EFE0A          cmp word [bp-0x2],byte +0xa
000003E3  7512              jnz 0x3f7
000003E5  8BDE              mov bx,si
000003E7  D1E3              shl bx,1
000003E9  D1E3              shl bx,1
000003EB  C7872E30A086      mov word [bx+0x302e],0x86a0
000003F1  C78730300100      mov word [bx+0x3030],0x1
000003F7  8A46FE            mov al,[bp-0x2]
000003FA  88840833          mov [si+0x3308],al
000003FE  46                inc si
000003FF  83                db 0x83
