; segment 0000  file offset 0x3600  size 0x5350
00000000  BADC28            mov dx,0x28dc
00000003  2E89160802        mov [cs:0x208],dx
00000008  B430              mov ah,0x30
0000000A  CD21              int 0x21
0000000C  8B2E0200          mov bp,[0x2]
00000010  8B1E2C00          mov bx,[0x2c]
00000014  8EDA              mov ds,dx
00000016  A37B00            mov [0x7b],ax
00000019  8C067900          mov [0x79],es
0000001D  891E7500          mov [0x75],bx
00000021  892E8D00          mov [0x8d],bp
00000025  E80E01            call 0x136
00000028  C43E7300          les di,[0x73]
0000002C  8BC7              mov ax,di
0000002E  8BD8              mov bx,ax
00000030  B9FF7F            mov cx,0x7fff
00000033  FC                cld
00000034  F2AE              repne scasb
00000036  E33C              jcxz 0x74
00000038  43                inc bx
00000039  263805            cmp [es:di],al
0000003C  75F6              jnz 0x34
0000003E  80CD80            or ch,0x80
00000041  F7D9              neg cx
00000043  890E7300          mov [0x73],cx
00000047  B90200            mov cx,0x2
0000004A  D3E3              shl bx,cl
0000004C  83C310            add bx,byte +0x10
0000004F  83E3F0            and bx,byte -0x10
00000052  891E7700          mov [0x77],bx
00000056  8CD2              mov dx,ss
00000058  2BEA              sub bp,dx
0000005A  8B3E5614          mov di,[0x1456]
0000005E  81FF0002          cmp di,0x200
00000062  7307              jnc 0x6b
00000064  BF0002            mov di,0x200
00000067  893E5614          mov [0x1456],di
0000006B  B104              mov cl,0x4
0000006D  D3EF              shr di,cl
0000006F  47                inc di
00000070  3BEF              cmp bp,di
00000072  7303              jnc 0x77
00000074  E97A01            jmp 0x1f1
00000077  8BDF              mov bx,di
00000079  03DA              add bx,dx
0000007B  891E8500          mov [0x85],bx
0000007F  891E8900          mov [0x89],bx
00000083  A17900            mov ax,[0x79]
00000086  2BD8              sub bx,ax
00000088  8EC0              mov es,ax
0000008A  B44A              mov ah,0x4a
0000008C  57                push di
0000008D  CD21              int 0x21
0000008F  5F                pop di
00000090  D3E7              shl di,cl
00000092  FA                cli
00000093  8ED2              mov ss,dx
00000095  8BE7              mov sp,di
00000097  893E5614          mov [0x1456],di
0000009B  FB                sti
0000009C  33C0              xor ax,ax
0000009E  2E8E060802        mov es,[cs:0x208]
000000A3  BF7A15            mov di,0x157a
000000A6  B99C58            mov cx,0x589c
000000A9  2BCF              sub cx,di
000000AB  FC                cld
000000AC  F3AA              rep stosb
000000AE  B400              mov ah,0x0
000000B0  CD1A              int 0x1a
000000B2  89167F00          mov [0x7f],dx
000000B6  890E8100          mov [0x81],cx
000000BA  33ED              xor bp,bp
000000BC  B8DA28            mov ax,0x28da
000000BF  8ED8              mov ds,ax
000000C1  BE0000            mov si,0x0
000000C4  BF1200            mov di,0x12
000000C7  E8DC00            call 0x1a6
000000CA  2E8E1E0802        mov ds,[cs:0x208]
000000CF  FF367100          push word [0x71]
000000D3  FF366F00          push word [0x6f]
000000D7  FF366D00          push word [0x6d]
000000DB  FF366B00          push word [0x6b]
000000DF  FF366900          push word [0x69]
000000E3  9A0C007717        call 0x1777:0xc
000000E8  2EC606B80172      mov byte [cs:0x1b8],0x72
000000EE  2EC606A70100      mov byte [cs:0x1a7],0x0
000000F4  50                push ax
000000F5  9A0A270000        call 0x0:0x270a
000000FA  B8DB28            mov ax,0x28db
000000FD  8ED8              mov ds,ax
000000FF  BE0200            mov si,0x2
00000102  BF0200            mov di,0x2
00000105  E89E00            call 0x1a6
00000108  2E8E1E0802        mov ds,[cs:0x208]
0000010D  FF1E8612          call far [0x1286]
00000111  FF1E8A12          call far [0x128a]
00000115  FF1E8E12          call far [0x128e]
00000119  50                push ax

; ---- 0000:011A ----
0000011A  2E8E1E0802        mov ds,[cs:0x208]
0000011F  9A79010000        call 0x0:0x179
00000124  8BEC              mov bp,sp
00000126  B44C              mov ah,0x4c
00000128  8A4604            mov al,[bp+0x4]
0000012B  CD21              int 0x21
0000012D  B90E00            mov cx,0xe
00000130  BA2D00            mov dx,0x2d
00000133  E9C100            jmp 0x1f7
00000136  1E                push ds
00000137  B80035            mov ax,0x3500
0000013A  CD21              int 0x21
0000013C  891E5900          mov [0x59],bx
00000140  8C065B00          mov [0x5b],es
00000144  B80435            mov ax,0x3504
00000147  CD21              int 0x21
00000149  891E5D00          mov [0x5d],bx
0000014D  8C065F00          mov [0x5f],es
00000151  B80535            mov ax,0x3505
00000154  CD21              int 0x21
00000156  891E6100          mov [0x61],bx
0000015A  8C066300          mov [0x63],es
0000015E  B80635            mov ax,0x3506
00000161  CD21              int 0x21
00000163  891E6500          mov [0x65],bx
00000167  8C066700          mov [0x67],es
0000016B  B80025            mov ax,0x2500
0000016E  8CCA              mov dx,cs
00000170  8EDA              mov ds,dx
00000172  BA2D01            mov dx,0x12d
00000175  CD21              int 0x21
00000177  1F                pop ds
00000178  C3                ret

; ---- 0000:0179 ----
00000179  1E                push ds
0000017A  B80025            mov ax,0x2500
0000017D  C5165900          lds dx,[0x59]
00000181  CD21              int 0x21
00000183  1F                pop ds
00000184  1E                push ds
00000185  B80425            mov ax,0x2504
00000188  C5165D00          lds dx,[0x5d]
0000018C  CD21              int 0x21
0000018E  1F                pop ds
0000018F  1E                push ds
00000190  B80525            mov ax,0x2505
00000193  C5166100          lds dx,[0x61]
00000197  CD21              int 0x21
00000199  1F                pop ds
0000019A  1E                push ds
0000019B  B80625            mov ax,0x2506
0000019E  C5166500          lds dx,[0x65]
000001A2  CD21              int 0x21
000001A4  1F                pop ds
000001A5  CB                retf
000001A6  B4FF              mov ah,0xff
000001A8  8BD7              mov dx,di
000001AA  8BDE              mov bx,si
000001AC  3BDF              cmp bx,di
000001AE  7414              jz 0x1c4
000001B0  803FFF            cmp byte [bx],0xff
000001B3  740A              jz 0x1bf
000001B5  386701            cmp [bx+0x1],ah
000001B8  7705              ja 0x1bf
000001BA  8A6701            mov ah,[bx+0x1]
000001BD  8BD3              mov dx,bx
000001BF  83C306            add bx,byte +0x6
000001C2  EBE8              jmp short 0x1ac
000001C4  3BD7              cmp dx,di
000001C6  7420              jz 0x1e8
000001C8  8BDA              mov bx,dx
000001CA  1E                push ds
000001CB  07                pop es
000001CC  06                push es
000001CD  803F00            cmp byte [bx],0x0
000001D0  C607FF            mov byte [bx],0xff
000001D3  2E8E1E0802        mov ds,[cs:0x208]
000001D8  7407              jz 0x1e1
000001DA  26FF5F02          call far [es:bx+0x2]
000001DE  1F                pop ds
000001DF  EBC5              jmp short 0x1a6
000001E1  26FF5702          call [es:bx+0x2]
000001E5  1F                pop ds
000001E6  EBBE              jmp short 0x1a6
000001E8  C3                ret
000001E9  B440              mov ah,0x40
000001EB  BB0200            mov bx,0x2
000001EE  CD21              int 0x21
000001F0  C3                ret
000001F1  B91E00            mov cx,0x1e
000001F4  BA3B00            mov dx,0x3b
000001F7  2E8E1E0802        mov ds,[cs:0x208]
000001FC  E8EAFF            call 0x1e9
000001FF  B80300            mov ax,0x3
00000202  50                push ax
00000203  9A1A010000        call 0x0:0x11a
00000208  0000              add [bx+si],al
0000020A  04C0              add al,0xc0
0000020C  2802              sub [bp+si],al
0000020E  48                dec ax
0000020F  026802            add ch,[bx+si+0x2]
00000212  8802              mov [bp+si],al
00000214  A802              test al,0x2
00000216  C802E802          enter 0xe802,0x2
0000021A  0803              or [bp+di],al
0000021C  2803              sub [bp+di],al
0000021E  48                dec ax
0000021F  036803            add bp,[bx+si+0x3]
00000222  8803              mov [bp+di],al
00000224  A803              test al,0x3
00000226  C8030001          enter 0x3,0x1
0000022A  0203              add al,[bp+di]
0000022C  0405              add al,0x5
0000022E  06                push es
0000022F  0809              or [bx+di],cl
00000231  0A0B              or cl,[bp+di]
00000233  0C0D              or al,0xd
00000235  0E                push cs
00000236  0F1011            movups xmm2,oword [bx+di]
00000239  1213              adc dl,[bp+di]
0000023B  1415              adc al,0x15
0000023D  16                push ss
0000023E  17                pop ss
0000023F  191A              sbb [bp+si],bx
00000241  1B1C              sbb bx,[si]
00000243  1D1E1F            sbb ax,0x1f1e
00000246  2021              and [bx+di],ah
00000248  0001              add [bx+di],al
0000024A  0203              add al,[bp+di]
0000024C  0406              add al,0x6
0000024E  07                pop es
0000024F  090A              or [bp+si],cx
00000251  0B0C              or cx,[si]
00000253  0D0E0F            or ax,0xf0e
00000256  1011              adc [bx+di],dl
00000258  1213              adc dl,[bp+di]
0000025A  1415              adc al,0x15
0000025C  16                push ss
0000025D  17                pop ss
0000025E  181A              sbb [bp+si],bl
00000260  1B1C              sbb bx,[si]
00000262  1D1E1F            sbb ax,0x1f1e
00000265  2022              and [bp+si],ah
00000267  2400              and al,0x0
00000269  0102              add [bp+si],ax
0000026B  0304              add ax,[si]
0000026D  06                push es
0000026E  0809              or [bx+di],cl
00000270  0A0C              or cl,[si]
00000272  0D0E0F            or ax,0xf0e
00000275  1112              adc [bp+si],dx
00000277  1314              adc dx,[si]
00000279  151617            adc ax,0x1716
0000027C  191A              sbb [bp+si],bx
0000027E  1C1D              sbb al,0x1d
00000280  1E                push ds
00000281  1F                pop ds
00000282  2021              and [bx+di],ah
00000284  2224              and ah,[si]
00000286  252600            and ax,0x26
00000289  0102              add [bp+si],ax
0000028B  0304              add ax,[si]
0000028D  06                push es
0000028E  080A              or [bp+si],cl
00000290  0B0C              or cx,[si]
00000292  0D0E0F            or ax,0xf0e
00000295  1112              adc [bp+si],dx
00000297  1314              adc dx,[si]
00000299  151617            adc ax,0x1716
0000029C  181A              sbb [bp+si],bl
0000029E  1C1D              sbb al,0x1d
000002A0  1E                push ds
000002A1  1F                pop ds
000002A2  2021              and [bx+di],ah
000002A4  2325              and sp,[di]
000002A6  27                daa
000002A7  2800              sub [bx+si],al
000002A9  0102              add [bp+si],ax
000002AB  0304              add ax,[si]
000002AD  06                push es
000002AE  080A              or [bp+si],cl
000002B0  0B0C              or cx,[si]
000002B2  0D0E0F            or ax,0xf0e
000002B5  1113              adc [bp+di],dx
000002B7  151617            adc ax,0x1716
000002BA  1819              sbb [bx+di],bl
000002BC  1B1D              sbb bx,[di]
000002BE  1F                pop ds
000002BF  2021              and [bx+di],ah
000002C1  2223              and ah,[bp+di]
000002C3  2425              and al,0x25
000002C5  26282A            sub [es:bp+si],ch
000002C8  0001              add [bx+di],al
000002CA  0203              add al,[bp+di]
000002CC  050709            add ax,0x907
000002CF  0B0C              or cx,[si]
000002D1  0D0E0F            or ax,0xf0e
000002D4  1011              adc [bx+di],dl
000002D6  1315              adc dx,[di]
000002D8  16                push ss
000002D9  17                pop ss
000002DA  1819              sbb [bx+di],bl
000002DC  1B1D              sbb bx,[di]
000002DE  1F                pop ds
000002DF  2021              and [bx+di],ah
000002E1  2223              and ah,[bp+di]
000002E3  252729            and ax,0x2927
000002E6  2B2D              sub bp,[di]
000002E8  0001              add [bx+di],al
000002EA  0203              add al,[bp+di]
000002EC  050709            add ax,0x907
000002EF  0B0C              or cx,[si]
000002F1  0D0E0F            or ax,0xf0e
000002F4  1011              adc [bx+di],dl
000002F6  1315              adc dx,[di]
000002F8  16                push ss
000002F9  17                pop ss
000002FA  181A              sbb [bp+si],bl
000002FC  1C1E              sbb al,0x1e
000002FE  2124              and [si],sp
00000300  252627            and ax,0x2726
00000303  292B              sub [bp+di],bp
00000305  2D2F30            sub ax,0x302f
00000308  0001              add [bx+di],al
0000030A  0204              add al,[si]
0000030C  06                push es
0000030D  080A              or [bp+si],cl
0000030F  0C0D              or al,0xd
00000311  0E                push cs
00000312  0F1011            movups xmm2,oword [bx+di]
00000315  1315              adc dx,[di]
00000317  1819              sbb [bx+di],bl
00000319  1A1C              sbb bl,[si]
0000031B  1D1F21            sbb ax,0x211f
0000031E  2325              and sp,[di]
00000320  2627              es daa
00000322  292B              sub [bp+di],bp
00000324  2D2F30            sub ax,0x302f
00000327  3200              xor al,[bx+si]
00000329  0102              add [bp+si],ax
0000032B  0406              add al,0x6
0000032D  080A              or [bp+si],cl
0000032F  0D0E0F            or ax,0xf0e
00000332  1011              adc [bx+di],dl
00000334  1214              adc dl,[si]
00000336  17                pop ss
00000337  1A19              sbb bl,[bx+di]
00000339  1A1C              sbb bl,[si]
0000033B  1E                push ds
0000033C  2022              and [bp+si],ah
0000033E  252829            and ax,0x2928
00000341  2A2B              sub ch,[bp+di]
00000343  2D2F31            sub ax,0x312f
00000346  3335              xor si,[di]
00000348  0001              add [bx+di],al
0000034A  0305              add ax,[di]
0000034C  07                pop es
0000034D  090B              or [bp+di],cx
0000034F  0E                push cs
00000350  0F1012            movups xmm2,oword [bp+si]
00000353  1416              adc al,0x16
00000355  181A              sbb [bp+si],bl
00000357  1B1C              sbb bx,[si]
00000359  1D1E20            sbb ax,0x201e
0000035C  2224              and ah,[si]
0000035E  26292A            sub [es:bp+si],bp
00000361  2C2E              sub al,0x2e
00000363  3032              xor [bp+si],dh
00000365  3436              xor al,0x36
00000367  3900              cmp [bx+si],ax
00000369  0103              add [bp+di],ax
0000036B  050709            add ax,0x907
0000036E  0B0E0F10          or cx,[0x100f]
00000372  1214              adc dl,[si]
00000374  16                push ss
00000375  191B              sbb [bp+di],bx
00000377  1E                push ds
00000378  1F                pop ds
00000379  2123              and [bp+di],sp
0000037B  252729            and ax,0x2927
0000037E  2B2D              sub bp,[di]
00000380  2E2F              cs das
00000382  3132              xor [bp+si],si
00000384  3436              xor al,0x36
00000386  393C              cmp [si],di
00000388  0001              add [bx+di],al
0000038A  0305              add ax,[di]
0000038C  07                pop es
0000038D  0A0C              or cl,[si]
0000038F  0F1011            movups xmm2,oword [bx+di]
00000392  1315              adc dx,[di]
00000394  17                pop ss
00000395  191B              sbb [bp+di],bx
00000397  1E                push ds
00000398  1F                pop ds
00000399  2022              and [bp+si],ah
0000039B  2426              and al,0x26
0000039D  282B              sub [bp+di],ch
0000039F  2E2F              cs das
000003A1  3032              xor [bp+si],dh
000003A3  3436              xor al,0x36
000003A5  393C              cmp [si],di
000003A7  3F                aas
000003A8  0002              add [bp+si],al
000003AA  0406              add al,0x6
000003AC  080B              or [bp+di],cl
000003AE  0D1011            or ax,0x1110
000003B1  1214              adc dl,[si]
000003B3  16                push ss
000003B4  181B              sbb [bp+di],bl
000003B6  1E                push ds
000003B7  2122              and [bp+si],sp
000003B9  2325              and sp,[di]
000003BB  27                daa
000003BC  292C              sub [si],bp
000003BE  2F                das
000003BF  3233              xor dh,[bp+di]
000003C1  3436              xor al,0x36
000003C3  383B              cmp [bp+di],bh
000003C5  3441              xor al,0x41
000003C7  44                inc sp
000003C8  0002              add [bp+si],al
000003CA  0406              add al,0x6
000003CC  080B              or [bp+di],cl
000003CE  0D1112            or ax,0x1211
000003D1  1315              adc dx,[di]
000003D3  17                pop ss
000003D4  1A1D              sbb bl,[di]
000003D6  2023              and [bp+di],ah
000003D8  2425              and al,0x25
000003DA  27                daa
000003DB  292C              sub [si],bp
000003DD  2F                das
000003DE  3235              xor dh,[di]
000003E0  3637              ss aaa
000003E2  393B              cmp [bp+di],di
000003E4  3E41              ds inc cx
000003E6  44                inc sp
000003E7  47                inc di
000003E8  0000              add [bx+si],al
000003EA  0000              add [bx+si],al
000003EC  0000              add [bx+si],al
000003EE  0000              add [bx+si],al
000003F0  0000              add [bx+si],al
000003F2  0000              add [bx+si],al
000003F4  0101              add [bx+di],ax
000003F6  FF                db 0xff
000003F7  FF34              push word [si]
000003F9  1200              adc al,[bx+si]
000003FB  0000              add [bx+si],al
000003FD  0000              add [bx+si],al
000003FF  0000              add [bx+si],al
00000401  0000              add [bx+si],al
00000403  0000              add [bx+si],al
00000405  0000              add [bx+si],al
00000407  0000              add [bx+si],al
00000409  0000              add [bx+si],al
0000040B  0000              add [bx+si],al
0000040D  00FF              add bh,bh
0000040F  0000              add [bx+si],al
00000411  0000              add [bx+si],al
00000413  0000              add [bx+si],al
00000415  0000              add [bx+si],al
00000417  0000              add [bx+si],al
00000419  0000              add [bx+si],al
0000041B  0000              add [bx+si],al
0000041D  0000              add [bx+si],al
0000041F  0000              add [bx+si],al
00000421  0000              add [bx+si],al
00000423  0000              add [bx+si],al
00000425  0000              add [bx+si],al
00000427  0000              add [bx+si],al
00000429  0000              add [bx+si],al
0000042B  0000              add [bx+si],al
0000042D  0000              add [bx+si],al
0000042F  0000              add [bx+si],al
00000431  0000              add [bx+si],al
00000433  0000              add [bx+si],al
00000435  0000              add [bx+si],al
00000437  0000              add [bx+si],al
00000439  0000              add [bx+si],al
0000043B  0000              add [bx+si],al
0000043D  0000              add [bx+si],al
0000043F  0000              add [bx+si],al
00000441  0000              add [bx+si],al
00000443  0000              add [bx+si],al
00000445  0000              add [bx+si],al
00000447  0000              add [bx+si],al
00000449  0000              add [bx+si],al
0000044B  0000              add [bx+si],al
0000044D  0000              add [bx+si],al
0000044F  0000              add [bx+si],al
00000451  0000              add [bx+si],al
00000453  0000              add [bx+si],al
00000455  0000              add [bx+si],al
00000457  0000              add [bx+si],al
00000459  0000              add [bx+si],al
0000045B  0000              add [bx+si],al
0000045D  0000              add [bx+si],al
0000045F  0000              add [bx+si],al
00000461  0000              add [bx+si],al
00000463  0000              add [bx+si],al
00000465  0000              add [bx+si],al
00000467  0000              add [bx+si],al
00000469  0000              add [bx+si],al
0000046B  0000              add [bx+si],al
0000046D  0000              add [bx+si],al
0000046F  0000              add [bx+si],al
00000471  0000              add [bx+si],al
00000473  0000              add [bx+si],al
00000475  0000              add [bx+si],al
00000477  0000              add [bx+si],al
00000479  0000              add [bx+si],al
0000047B  0000              add [bx+si],al
0000047D  0000              add [bx+si],al
0000047F  0000              add [bx+si],al
00000481  0000              add [bx+si],al
00000483  0000              add [bx+si],al
00000485  0000              add [bx+si],al
00000487  0000              add [bx+si],al
00000489  0000              add [bx+si],al
0000048B  0000              add [bx+si],al
0000048D  0000              add [bx+si],al
0000048F  0000              add [bx+si],al
00000491  0000              add [bx+si],al
00000493  0000              add [bx+si],al
00000495  0000              add [bx+si],al
00000497  0000              add [bx+si],al
00000499  0000              add [bx+si],al
0000049B  0000              add [bx+si],al
0000049D  0000              add [bx+si],al
0000049F  0000              add [bx+si],al
000004A1  0000              add [bx+si],al
000004A3  0000              add [bx+si],al
000004A5  0000              add [bx+si],al
000004A7  0000              add [bx+si],al
000004A9  0000              add [bx+si],al
000004AB  0000              add [bx+si],al
000004AD  0000              add [bx+si],al
000004AF  0000              add [bx+si],al
000004B1  0000              add [bx+si],al
000004B3  0000              add [bx+si],al
000004B5  0000              add [bx+si],al
000004B7  0000              add [bx+si],al
000004B9  0000              add [bx+si],al
000004BB  0000              add [bx+si],al
000004BD  0000              add [bx+si],al
000004BF  0000              add [bx+si],al
000004C1  0000              add [bx+si],al
000004C3  0000              add [bx+si],al
000004C5  0000              add [bx+si],al
000004C7  0000              add [bx+si],al
000004C9  0000              add [bx+si],al
000004CB  0000              add [bx+si],al
000004CD  0000              add [bx+si],al
000004CF  0000              add [bx+si],al
000004D1  0000              add [bx+si],al
000004D3  0000              add [bx+si],al
000004D5  0000              add [bx+si],al
000004D7  0000              add [bx+si],al
000004D9  0000              add [bx+si],al
000004DB  0000              add [bx+si],al
000004DD  0000              add [bx+si],al
000004DF  0000              add [bx+si],al
000004E1  0000              add [bx+si],al
000004E3  0000              add [bx+si],al
000004E5  0000              add [bx+si],al
000004E7  0000              add [bx+si],al
000004E9  0000              add [bx+si],al
000004EB  0000              add [bx+si],al
000004ED  0000              add [bx+si],al
000004EF  0000              add [bx+si],al
000004F1  0000              add [bx+si],al
000004F3  0000              add [bx+si],al
000004F5  0000              add [bx+si],al
000004F7  0000              add [bx+si],al
000004F9  0000              add [bx+si],al
000004FB  0000              add [bx+si],al
000004FD  0000              add [bx+si],al
000004FF  0000              add [bx+si],al
00000501  0000              add [bx+si],al
00000503  0000              add [bx+si],al
00000505  0000              add [bx+si],al
00000507  0000              add [bx+si],al
00000509  0000              add [bx+si],al
0000050B  0000              add [bx+si],al
0000050D  0000              add [bx+si],al
0000050F  0000              add [bx+si],al
00000511  0000              add [bx+si],al
00000513  0000              add [bx+si],al
00000515  0000              add [bx+si],al
00000517  0000              add [bx+si],al
00000519  0000              add [bx+si],al
0000051B  0000              add [bx+si],al
0000051D  0000              add [bx+si],al
0000051F  0000              add [bx+si],al
00000521  0000              add [bx+si],al
00000523  0000              add [bx+si],al
00000525  0000              add [bx+si],al
00000527  0000              add [bx+si],al
00000529  0000              add [bx+si],al
0000052B  0000              add [bx+si],al
0000052D  0000              add [bx+si],al
0000052F  0000              add [bx+si],al
00000531  0000              add [bx+si],al
00000533  0000              add [bx+si],al
00000535  0000              add [bx+si],al
00000537  0000              add [bx+si],al
00000539  0000              add [bx+si],al
0000053B  0000              add [bx+si],al
0000053D  0000              add [bx+si],al
0000053F  0000              add [bx+si],al
00000541  0000              add [bx+si],al
00000543  0000              add [bx+si],al
00000545  0000              add [bx+si],al
00000547  0000              add [bx+si],al
00000549  0000              add [bx+si],al
0000054B  0000              add [bx+si],al
0000054D  0000              add [bx+si],al
0000054F  0000              add [bx+si],al
00000551  0000              add [bx+si],al
00000553  0000              add [bx+si],al
00000555  0000              add [bx+si],al
00000557  0000              add [bx+si],al
00000559  0000              add [bx+si],al
0000055B  0000              add [bx+si],al
0000055D  0000              add [bx+si],al
0000055F  0000              add [bx+si],al
00000561  0000              add [bx+si],al
00000563  0000              add [bx+si],al
00000565  0000              add [bx+si],al
00000567  0000              add [bx+si],al
00000569  0000              add [bx+si],al
0000056B  0000              add [bx+si],al
0000056D  0000              add [bx+si],al
0000056F  0000              add [bx+si],al
00000571  0000              add [bx+si],al
00000573  0000              add [bx+si],al
00000575  0000              add [bx+si],al
00000577  0000              add [bx+si],al
00000579  0000              add [bx+si],al
0000057B  0000              add [bx+si],al
0000057D  0000              add [bx+si],al
0000057F  0000              add [bx+si],al
00000581  0000              add [bx+si],al
00000583  0000              add [bx+si],al
00000585  0000              add [bx+si],al
00000587  0000              add [bx+si],al
00000589  0000              add [bx+si],al
0000058B  0000              add [bx+si],al
0000058D  0000              add [bx+si],al
0000058F  0000              add [bx+si],al
00000591  0000              add [bx+si],al
00000593  0000              add [bx+si],al
00000595  0000              add [bx+si],al
00000597  0000              add [bx+si],al
00000599  0000              add [bx+si],al
0000059B  0000              add [bx+si],al
0000059D  0000              add [bx+si],al
0000059F  0000              add [bx+si],al
000005A1  0000              add [bx+si],al
000005A3  0000              add [bx+si],al
000005A5  0000              add [bx+si],al
000005A7  0000              add [bx+si],al
000005A9  0000              add [bx+si],al
000005AB  0000              add [bx+si],al
000005AD  0000              add [bx+si],al
000005AF  0000              add [bx+si],al
000005B1  0000              add [bx+si],al
000005B3  0000              add [bx+si],al
000005B5  0000              add [bx+si],al
000005B7  0000              add [bx+si],al
000005B9  0000              add [bx+si],al
000005BB  0000              add [bx+si],al
000005BD  0000              add [bx+si],al
000005BF  0000              add [bx+si],al
000005C1  0000              add [bx+si],al
000005C3  0000              add [bx+si],al
000005C5  0000              add [bx+si],al
000005C7  0000              add [bx+si],al
000005C9  0000              add [bx+si],al
000005CB  0000              add [bx+si],al
000005CD  0000              add [bx+si],al
000005CF  0000              add [bx+si],al
000005D1  0000              add [bx+si],al
000005D3  0000              add [bx+si],al
000005D5  0000              add [bx+si],al
000005D7  0000              add [bx+si],al
000005D9  0000              add [bx+si],al
000005DB  0000              add [bx+si],al
000005DD  0000              add [bx+si],al
000005DF  0000              add [bx+si],al
000005E1  0000              add [bx+si],al
000005E3  0000              add [bx+si],al
000005E5  0000              add [bx+si],al
000005E7  0000              add [bx+si],al
000005E9  0000              add [bx+si],al
000005EB  0000              add [bx+si],al
000005ED  0000              add [bx+si],al
000005EF  0000              add [bx+si],al
000005F1  0000              add [bx+si],al
000005F3  0000              add [bx+si],al
000005F5  0000              add [bx+si],al
000005F7  0000              add [bx+si],al
000005F9  0000              add [bx+si],al
000005FB  0000              add [bx+si],al
000005FD  0000              add [bx+si],al
000005FF  0000              add [bx+si],al
00000601  0000              add [bx+si],al
00000603  0000              add [bx+si],al
00000605  0000              add [bx+si],al
00000607  0000              add [bx+si],al
00000609  0000              add [bx+si],al
0000060B  0000              add [bx+si],al
0000060D  0000              add [bx+si],al
0000060F  0000              add [bx+si],al
00000611  0000              add [bx+si],al
00000613  0000              add [bx+si],al
00000615  0000              add [bx+si],al
00000617  0000              add [bx+si],al
00000619  0000              add [bx+si],al
0000061B  0000              add [bx+si],al
0000061D  0000              add [bx+si],al
0000061F  0000              add [bx+si],al
00000621  0000              add [bx+si],al
00000623  0000              add [bx+si],al
00000625  0000              add [bx+si],al
00000627  0000              add [bx+si],al
00000629  0000              add [bx+si],al
0000062B  0000              add [bx+si],al
0000062D  0000              add [bx+si],al
0000062F  0000              add [bx+si],al
00000631  0000              add [bx+si],al
00000633  0000              add [bx+si],al
00000635  0000              add [bx+si],al
00000637  0000              add [bx+si],al
00000639  0000              add [bx+si],al
0000063B  0000              add [bx+si],al
0000063D  0000              add [bx+si],al
0000063F  0000              add [bx+si],al
00000641  0000              add [bx+si],al
00000643  0000              add [bx+si],al
00000645  0000              add [bx+si],al
00000647  0000              add [bx+si],al
00000649  0000              add [bx+si],al
0000064B  0000              add [bx+si],al
0000064D  0000              add [bx+si],al
0000064F  0000              add [bx+si],al
00000651  0000              add [bx+si],al
00000653  0000              add [bx+si],al
00000655  0000              add [bx+si],al
00000657  0000              add [bx+si],al
00000659  0000              add [bx+si],al
0000065B  0000              add [bx+si],al
0000065D  0000              add [bx+si],al
0000065F  0000              add [bx+si],al
00000661  0000              add [bx+si],al
00000663  0000              add [bx+si],al
00000665  0000              add [bx+si],al
00000667  0000              add [bx+si],al
00000669  0000              add [bx+si],al
0000066B  0000              add [bx+si],al
0000066D  0000              add [bx+si],al
0000066F  0000              add [bx+si],al
00000671  0000              add [bx+si],al
00000673  0000              add [bx+si],al
00000675  0000              add [bx+si],al
00000677  0000              add [bx+si],al
00000679  0000              add [bx+si],al
0000067B  0000              add [bx+si],al
0000067D  0000              add [bx+si],al
0000067F  0000              add [bx+si],al
00000681  0000              add [bx+si],al
00000683  0000              add [bx+si],al
00000685  0000              add [bx+si],al
00000687  0000              add [bx+si],al
00000689  0000              add [bx+si],al
0000068B  0000              add [bx+si],al
0000068D  0000              add [bx+si],al
0000068F  0000              add [bx+si],al
00000691  0000              add [bx+si],al
00000693  0000              add [bx+si],al
00000695  0000              add [bx+si],al
00000697  0000              add [bx+si],al
00000699  0000              add [bx+si],al
0000069B  0000              add [bx+si],al
0000069D  0000              add [bx+si],al
0000069F  0000              add [bx+si],al
000006A1  0000              add [bx+si],al
000006A3  0000              add [bx+si],al
000006A5  0000              add [bx+si],al
000006A7  0000              add [bx+si],al
000006A9  0000              add [bx+si],al
000006AB  0000              add [bx+si],al
000006AD  0000              add [bx+si],al
000006AF  0000              add [bx+si],al
000006B1  0000              add [bx+si],al
000006B3  0000              add [bx+si],al
000006B5  0000              add [bx+si],al
000006B7  0000              add [bx+si],al
000006B9  0000              add [bx+si],al
000006BB  0000              add [bx+si],al
000006BD  0000              add [bx+si],al
000006BF  0000              add [bx+si],al
000006C1  0000              add [bx+si],al
000006C3  0000              add [bx+si],al
000006C5  0000              add [bx+si],al
000006C7  0000              add [bx+si],al
000006C9  2B04              sub ax,[si]
000006CB  6E                outsb
000006CC  04B1              add al,0xb1
000006CE  04F4              add al,0xf4
000006D0  0437              add al,0x37
000006D2  057A05            add ax,0x57a
000006D5  BD0500            mov bp,0x5
000006D8  06                push es
000006D9  43                inc bx
000006DA  06                push es
000006DB  86060001          xchg al,[0x100]
000006DF  0208              add cl,[bx+si]
000006E1  090A              or [bp+si],cx
000006E3  1011              adc [bx+di],dl
000006E5  1234              adc dh,[si]
000006E7  014701            add [bx+0x1],ax
000006EA  5A                pop dx
000006EB  016F01            add [bx+0x1],bp
000006EE  8401              test [bx+di],al
000006F0  9C                pushf
000006F1  01B401CE          add [si-0x31ff],si
000006F5  01E9              add cx,bp
000006F7  0107              add [bx],ax
000006F9  0225              add ah,[di]
000006FB  024602            add al,[bp+0x2]

; ---- 0000:06FE ----
000006FE  1E                push ds
000006FF  06                push es
00000700  56                push si
00000701  57                push di
00000702  8CC8              mov ax,cs
00000704  8ED8              mov ds,ax
00000706  33DB              xor bx,bx
00000708  8B9FB215          mov bx,[bx+0x15b2]
0000070C  891E1504          mov [0x415],bx
00000710  E8FF00            call 0x812
00000713  C70619040000      mov word [0x419],0x0
00000719  C70617040000      mov word [0x417],0x0
0000071F  5F                pop di
00000720  5E                pop si
00000721  07                pop es
00000722  1F                pop ds
00000723  CB                retf

; ---- 0000:0724 ----
00000724  57                push di
00000725  56                push si
00000726  1E                push ds
00000727  8CC8              mov ax,cs
00000729  8ED8              mov ds,ax
0000072B  E8E400            call 0x812
0000072E  1F                pop ds
0000072F  5E                pop si
00000730  5F                pop di
00000731  CB                retf
00000732  50                push ax
00000733  53                push bx
00000734  51                push cx
00000735  52                push dx
00000736  57                push di
00000737  56                push si
00000738  1E                push ds
00000739  06                push es
0000073A  8CC8              mov ax,cs
0000073C  8ED8              mov ds,ax
0000073E  E82F00            call 0x770
00000741  E82201            call 0x866
00000744  E8D50A            call 0x121c
00000747  07                pop es
00000748  1F                pop ds
00000749  5E                pop si
0000074A  5F                pop di
0000074B  5A                pop dx
0000074C  59                pop cx
0000074D  5B                pop bx
0000074E  58                pop ax
0000074F  CB                retf

; ---- 0000:0750 ----
00000750  55                push bp
00000751  8BEC              mov bp,sp
00000753  1E                push ds
00000754  8CCB              mov bx,cs
00000756  8EDB              mov ds,bx
00000758  8B1E1704          mov bx,[0x417]
0000075C  8B4606            mov ax,[bp+0x6]
0000075F  89871B04          mov [bx+0x41b],ax
00000763  43                inc bx
00000764  83E30F            and bx,byte +0xf
00000767  891E1704          mov [0x417],bx
0000076B  1F                pop ds
0000076C  8BE5              mov sp,bp
0000076E  5D                pop bp
0000076F  CB                retf
00000770  1E                push ds
00000771  56                push si
00000772  57                push di
00000773  8CCB              mov bx,cs
00000775  8EDB              mov ds,bx
00000777  8B1E1904          mov bx,[0x419]
0000077B  3B1E1704          cmp bx,[0x417]
0000077F  7502              jnz 0x783
00000781  EB5B              jmp short 0x7de
00000783  8B9F1B04          mov bx,[bx+0x41b]
00000787  2AFF              sub bh,bh
00000789  D1E3              shl bx,1
0000078B  031E1504          add bx,[0x415]
0000078F  8B37              mov si,[bx]
00000791  2E8B1C            mov bx,[cs:si]
00000794  80FB00            cmp bl,0x0
00000797  7505              jnz 0x79e
00000799  C606F30300        mov byte [0x3f3],0x0
0000079E  8AC7              mov al,bh
000007A0  8ACB              mov cl,bl
000007A2  2AFF              sub bh,bh
000007A4  D1E3              shl bx,1
000007A6  8BBFC906          mov di,[bx+0x6c9]
000007AA  3A4508            cmp al,[di+0x8]
000007AD  7C24              jl 0x7d3
000007AF  E86D0B            call 0x131f
000007B2  884508            mov [di+0x8],al
000007B5  46                inc si
000007B6  46                inc si
000007B7  897503            mov [di+0x3],si
000007BA  C64525FF          mov byte [di+0x25],0xff
000007BE  C64526FF          mov byte [di+0x26],0xff
000007C2  C6450501          mov byte [di+0x5],0x1
000007C6  C606F00301        mov byte [0x3f0],0x1
000007CB  80FB12            cmp bl,0x12
000007CE  7403              jz 0x7d3
000007D0  E8EB0B            call 0x13be
000007D3  FF061904          inc word [0x419]
000007D7  832619040F        and word [0x419],byte +0xf
000007DC  EB99              jmp short 0x777
000007DE  5F                pop di
000007DF  5E                pop si
000007E0  1F                pop ds
000007E1  C3                ret
000007E2  55                push bp
000007E3  8BEC              mov bp,sp
000007E5  837E0600          cmp word [bp+0x6],byte +0x0
000007E9  7504              jnz 0x7ef
000007EB  8CC8              mov ax,cs
000007ED  EB1F              jmp short 0x80e
000007EF  837E0601          cmp word [bp+0x6],byte +0x1
000007F3  7506              jnz 0x7fb
000007F5  2EA11504          mov ax,[cs:0x415]
000007F9  EB13              jmp short 0x80e
000007FB  837E0602          cmp word [bp+0x6],byte +0x2
000007FF  7504              jnz 0x805
00000801  2EA11504          mov ax,[cs:0x415]
00000805  837E0603          cmp word [bp+0x6],byte +0x3
00000809  7503              jnz 0x80e
0000080B  B80D1B            mov ax,0x1b0d
0000080E  8BE5              mov sp,bp
00000810  5D                pop bp
00000811  CB                retf
00000812  C606F00300        mov byte [0x3f0],0x0
00000817  C706F8033412      mov word [0x3f8],0x1234
0000081D  B82001            mov ax,0x120
00000820  E8D709            call 0x11fa
00000823  B80008            mov ax,0x800
00000826  E8D109            call 0x11fa
00000829  B800BD            mov ax,0xbd00
0000082C  E8CB09            call 0x11fa
0000082F  C606F30300        mov byte [0x3f3],0x0
00000834  B90A00            mov cx,0xa
00000837  B700              mov bh,0x0
00000839  8AD9              mov bl,cl
0000083B  FECB              dec bl
0000083D  80FB09            cmp bl,0x9
00000840  7418              jz 0x85a
00000842  8AA7DD06          mov ah,[bx+0x6dd]
00000846  80C440            add ah,0x40
00000849  B03F              mov al,0x3f
0000084B  E8AC09            call 0x11fa
0000084E  8AA7DD06          mov ah,[bx+0x6dd]
00000852  80C443            add ah,0x43
00000855  B03F              mov al,0x3f
00000857  E8A009            call 0x11fa
0000085A  D0E3              shl bl,1
0000085C  8BBFC906          mov di,[bx+0x6c9]
00000860  E8BC0A            call 0x131f
00000863  E2D4              loop 0x839
00000865  C3                ret
00000866  803EF00300        cmp byte [0x3f0],0x0
0000086B  7501              jnz 0x86e
0000086D  C3                ret
0000086E  C606EF0309        mov byte [0x3ef],0x9
00000873  8A1EEF03          mov bl,[0x3ef]
00000877  B700              mov bh,0x0
00000879  D0E3              shl bl,1
0000087B  8BBFC906          mov di,[bx+0x6c9]
0000087F  8A4505            mov al,[di+0x5]
00000882  0AC0              or al,al
00000884  7503              jnz 0x889
00000886  E9B300            jmp 0x93c
00000889  BBDD06            mov bx,0x6dd
0000088C  021EEF03          add bl,[0x3ef]
00000890  8A07              mov al,[bx]
00000892  A2FC03            mov [0x3fc],al
00000895  807D4000          cmp byte [di+0x40],0x0
00000899  7406              jz 0x8a1
0000089B  A0F203            mov al,[0x3f2]
0000089E  884525            mov [di+0x25],al
000008A1  8A4526            mov al,[di+0x26]
000008A4  024525            add al,[di+0x25]
000008A7  884526            mov [di+0x26],al
000008AA  7203              jc 0x8af
000008AC  E98700            jmp 0x936
000008AF  FE4D05            dec byte [di+0x5]
000008B2  741C              jz 0x8d0
000008B4  8A4505            mov al,[di+0x5]
000008B7  3A4521            cmp al,[di+0x21]
000008BA  7503              jnz 0x8bf
000008BC  E8DB08            call 0x119a
000008BF  3A4535            cmp al,[di+0x35]
000008C2  750A              jnz 0x8ce
000008C4  803EEF0309        cmp byte [0x3ef],0x9
000008C9  7403              jz 0x8ce
000008CB  E8CC08            call 0x119a
000008CE  EB66              jmp short 0x936
000008D0  8B7503            mov si,[di+0x3]
000008D3  AD                lodsw
000008D4  0AC0              or al,al
000008D6  7902              jns 0x8da
000008D8  EB47              jmp short 0x921
000008DA  E8D807            call 0x10b5
000008DD  E88A08            call 0x116a
000008E0  E8DF08            call 0x11c2
000008E3  837D3200          cmp word [di+0x32],byte +0x0
000008E7  7402              jz 0x8eb
000008E9  7506              jnz 0x8f1
000008EB  837D3300          cmp word [di+0x33],byte +0x0
000008EF  7424              jz 0x915
000008F1  AC                lodsb
000008F2  884534            mov [di+0x34],al
000008F5  E8B109            call 0x12a9
000008F8  BA8803            mov dx,0x388
000008FB  8A26FC03          mov ah,[0x3fc]
000008FF  80C443            add ah,0x43
00000902  E8F508            call 0x11fa
00000905  E8D909            call 0x12e1
00000908  BA8803            mov dx,0x388
0000090B  8A26FC03          mov ah,[0x3fc]
0000090F  80C440            add ah,0x40
00000912  E8E508            call 0x11fa
00000915  A0EC03            mov al,[0x3ec]
00000918  0AC0              or al,al
0000091A  74B7              jz 0x8d3
0000091C  897503            mov [di+0x3],si
0000091F  EB15              jmp short 0x936
00000921  247F              and al,0x7f
00000923  3C70              cmp al,0x70
00000925  7E03              jng 0x92a
00000927  E9BB00            jmp 0x9e5
0000092A  8AD8              mov bl,al
0000092C  2AFF              sub bh,bh
0000092E  D0E3              shl bl,1
00000930  8B9F1E15          mov bx,[bx+0x151e]
00000934  FFE3              jmp bx
00000936  FF5529            call [di+0x29]
00000939  FF552B            call [di+0x2b]
0000093C  FE0EEF03          dec byte [0x3ef]
00000940  7803              js 0x945
00000942  E92EFF            jmp 0x873
00000945  C3                ret
00000946  886506            mov [di+0x6],ah
00000949  EB88              jmp short 0x8d3
0000094B  FE4D06            dec byte [di+0x6]
0000094E  7558              jnz 0x9a8
00000950  46                inc si
00000951  EB80              jmp short 0x8d3
00000953  56                push si
00000954  57                push di
00000955  53                push bx
00000956  8ADC              mov bl,ah
00000958  80FBFF            cmp bl,0xff
0000095B  7202              jc 0x95f
0000095D  EB3D              jmp short 0x99c
0000095F  2AFF              sub bh,bh
00000961  D1E3              shl bx,1
00000963  031E1504          add bx,[0x415]
00000967  8B37              mov si,[bx]
00000969  2E8B1C            mov bx,[cs:si]
0000096C  8AC7              mov al,bh
0000096E  8ACB              mov cl,bl
00000970  2AFF              sub bh,bh
00000972  D1E3              shl bx,1
00000974  8BBFC906          mov di,[bx+0x6c9]
00000978  3A4508            cmp al,[di+0x8]
0000097B  7C1F              jl 0x99c
0000097D  E89F09            call 0x131f
00000980  884508            mov [di+0x8],al
00000983  46                inc si
00000984  46                inc si
00000985  897503            mov [di+0x3],si
00000988  C64525FF          mov byte [di+0x25],0xff
0000098C  C64526FF          mov byte [di+0x26],0xff
00000990  C6450501          mov byte [di+0x5],0x1
00000994  C606F00301        mov byte [0x3f0],0x1
00000999  E8220A            call 0x13be
0000099C  5B                pop bx
0000099D  5F                pop di
0000099E  5E                pop si
0000099F  E931FF            jmp 0x8d3
000009A2  886535            mov [di+0x35],ah
000009A5  E92BFF            jmp 0x8d3
000009A8  8ACC              mov cl,ah
000009AA  AC                lodsb
000009AB  86E0              xchg ah,al
000009AD  8AC1              mov al,cl
000009AF  8BF0              mov si,ax
000009B1  E91FFF            jmp 0x8d3
000009B4  4E                dec si
000009B5  8A5D09            mov bl,[di+0x9]
000009B8  B700              mov bh,0x0
000009BA  D0E3              shl bl,1
000009BC  8BC6              mov ax,si
000009BE  40                inc ax
000009BF  40                inc ax
000009C0  89410A            mov [bx+di+0xa],ax
000009C3  FE4509            inc byte [di+0x9]
000009C6  AD                lodsw
000009C7  8BF0              mov si,ax
000009C9  E907FF            jmp 0x8d3
000009CC  4E                dec si
000009CD  FE4D09            dec byte [di+0x9]
000009D0  8A5D09            mov bl,[di+0x9]
000009D3  B700              mov bh,0x0
000009D5  D0E3              shl bl,1
000009D7  8B410A            mov ax,[bx+di+0xa]
000009DA  8BF0              mov si,ax
000009DC  E9F4FE            jmp 0x8d3
000009DF  886507            mov [di+0x7],ah
000009E2  E9EEFE            jmp 0x8d3
000009E5  C6450800          mov byte [di+0x8],0x0
000009E9  C745030000        mov word [di+0x3],0x0
000009EE  803EEF0309        cmp byte [0x3ef],0x9
000009F3  7403              jz 0x9f8
000009F5  E8A207            call 0x119a
000009F8  E941FF            jmp 0x93c
000009FB  E8C407            call 0x11c2
000009FE  897503            mov [di+0x3],si
00000A01  803EEF0309        cmp byte [0x3ef],0x9
00000A06  7403              jz 0xa0b
00000A08  E88F07            call 0x119a
00000A0B  A0EC03            mov al,[0x3ec]
00000A0E  0AC0              or al,al
00000A10  7403              jz 0xa15
00000A12  E921FF            jmp 0x936
00000A15  E9BBFE            jmp 0x8d3
00000A18  AC                lodsb
00000A19  E8DE07            call 0x11fa
00000A1C  E9B4FE            jmp 0x8d3
00000A1F  86E0              xchg ah,al
00000A21  E89106            call 0x10b5
00000A24  AC                lodsb
00000A25  86E0              xchg ah,al
00000A27  E89807            call 0x11c2
00000A2A  897503            mov [di+0x3],si
00000A2D  A0EC03            mov al,[0x3ec]
00000A30  0AC0              or al,al
00000A32  7403              jz 0xa37
00000A34  E9FFFE            jmp 0x936
00000A37  E999FE            jmp 0x8d3
00000A3A  886512            mov [di+0x12],ah
00000A3D  E993FE            jmp 0x8d3
00000A40  88653A            mov [di+0x3a],ah
00000A43  886539            mov [di+0x39],ah
00000A46  AC                lodsb
00000A47  88453B            mov [di+0x3b],al
00000A4A  88453C            mov [di+0x3c],al
00000A4D  AC                lodsb
00000A4E  88453D            mov [di+0x3d],al
00000A51  AD                lodsw
00000A52  89453E            mov [di+0x3e],ax
00000A55  C7452B6114        mov word [di+0x2b],0x1461
00000A5A  E976FE            jmp 0x8d3
00000A5D  A0EF03            mov al,[0x3ef]
00000A60  50                push ax
00000A61  57                push di
00000A62  8ADC              mov bl,ah
00000A64  B700              mov bh,0x0
00000A66  D0E3              shl bl,1
00000A68  8BBFC906          mov di,[bx+0x6c9]
00000A6C  C6450500          mov byte [di+0x5],0x0
00000A70  C6450800          mov byte [di+0x8],0x0
00000A74  C745030000        mov word [di+0x3],0x0
00000A79  5F                pop di
00000A7A  58                pop ax
00000A7B  A2EF03            mov [0x3ef],al
00000A7E  E952FE            jmp 0x8d3
00000A81  56                push si
00000A82  8A0EFC03          mov cl,[0x3fc]
00000A86  2BDB              sub bx,bx
00000A88  8ADC              mov bl,ah
00000A8A  D1E3              shl bx,1
00000A8C  8BB70D1B          mov si,[bx+0x1b0d]
00000A90  E8BF08            call 0x1352
00000A93  5E                pop si
00000A94  E93CFE            jmp 0x8d3
00000A97  886513            mov [di+0x13],ah
00000A9A  AC                lodsb
00000A9B  86E0              xchg ah,al
00000A9D  AC                lodsb
00000A9E  894515            mov [di+0x15],ax
00000AA1  C745298A14        mov word [di+0x29],0x148a
00000AA6  C64514FF          mov byte [di+0x14],0xff
00000AAA  E926FE            jmp 0x8d3
00000AAD  4E                dec si
00000AAE  C745290714        mov word [di+0x29],0x1407
00000AB3  C745150000        mov word [di+0x15],0x0
00000AB8  E918FE            jmp 0x8d3
00000ABB  886524            mov [di+0x24],ah
00000ABE  E912FE            jmp 0x8d3
00000AC1  88651D            mov [di+0x1d],ah
00000AC4  AC                lodsb
00000AC5  884519            mov [di+0x19],al
00000AC8  AC                lodsb
00000AC9  88451A            mov [di+0x1a],al
00000ACC  FE451A            inc byte [di+0x1a]
00000ACF  D0E0              shl al,1
00000AD1  88451B            mov [di+0x1b],al
00000AD4  AC                lodsb
00000AD5  88451C            mov [di+0x1c],al
00000AD8  C745290814        mov word [di+0x29],0x1408
00000ADD  E9F3FD            jmp 0x8d3
00000AE0  886508            mov [di+0x8],ah
00000AE3  E9EDFD            jmp 0x8d3
00000AE6  D0EC              shr ah,1
00000AE8  88260C04          mov [0x40c],ah
00000AEC  88260D04          mov [0x40d],ah
00000AF0  C6060E04FF        mov byte [0x40e],0xff
00000AF5  B400              mov ah,0x0
00000AF7  88260F04          mov [0x40f],ah
00000AFB  88261004          mov [0x410],ah
00000AFF  E9D1FD            jmp 0x8d3
00000B02  A01004            mov al,[0x410]
00000B05  0AC0              or al,al
00000B07  750C              jnz 0xb15
00000B09  22260F04          and ah,[0x40f]
00000B0D  7514              jnz 0xb23
00000B0F  FE061004          inc byte [0x410]
00000B13  EB0E              jmp short 0xb23
00000B15  22260F04          and ah,[0x40f]
00000B19  7408              jz 0xb23
00000B1B  C606100400        mov byte [0x410],0x0
00000B20  E9B0FD            jmp 0x8d3
00000B23  4E                dec si
00000B24  4E                dec si
00000B25  C6450501          mov byte [di+0x5],0x1
00000B29  897503            mov [di+0x3],si
00000B2C  E90DFE            jmp 0x93c
00000B2F  E9A1FD            jmp 0x8d3
00000B32  886520            mov [di+0x20],ah
00000B35  E84207            call 0x127a
00000B38  E998FD            jmp 0x8d3
00000B3B  E88406            call 0x11c2
00000B3E  897503            mov [di+0x3],si
00000B41  A0EC03            mov al,[0x3ec]
00000B44  0AC0              or al,al
00000B46  7403              jz 0xb4b
00000B48  E9EBFD            jmp 0x936
00000B4B  E985FD            jmp 0x8d3
00000B4E  E87106            call 0x11c2
00000B51  E81606            call 0x116a
00000B54  897503            mov [di+0x3],si
00000B57  A0EC03            mov al,[0x3ec]
00000B5A  0AC0              or al,al
00000B5C  7403              jz 0xb61
00000B5E  E9D5FD            jmp 0x936
00000B61  E96FFD            jmp 0x8d3
00000B64  80E407            and ah,0x7
00000B67  88652D            mov [di+0x2d],ah
00000B6A  E966FD            jmp 0x8d3
00000B6D  8826F203          mov [0x3f2],ah
00000B71  E95FFD            jmp 0x8d3
00000B74  4E                dec si
00000B75  C7452B0714        mov word [di+0x2b],0x1407
00000B7A  E956FD            jmp 0x8d3
00000B7D  886525            mov [di+0x25],ah
00000B80  E950FD            jmp 0x8d3
00000B83  886530            mov [di+0x30],ah
00000B86  E94AFD            jmp 0x8d3
00000B89  A0EF03            mov al,[0x3ef]
00000B8C  50                push ax
00000B8D  57                push di
00000B8E  AC                lodsb
00000B8F  8826EF03          mov [0x3ef],ah
00000B93  8ADC              mov bl,ah
00000B95  B700              mov bh,0x0
00000B97  D0E3              shl bl,1
00000B99  8BBFC906          mov di,[bx+0x6c9]
00000B9D  8805              mov [di],al
00000B9F  E8D806            call 0x127a
00000BA2  5F                pop di
00000BA3  58                pop ax
00000BA4  A2EF03            mov [0x3ef],al
00000BA7  E929FD            jmp 0x8d3
00000BAA  A0EF03            mov al,[0x3ef]
00000BAD  50                push ax
00000BAE  57                push di
00000BAF  8826EF03          mov [0x3ef],ah
00000BB3  2BDB              sub bx,bx
00000BB5  8ADC              mov bl,ah
00000BB7  AC                lodsb
00000BB8  D0E3              shl bl,1
00000BBA  8BBFC906          mov di,[bx+0x6c9]
00000BBE  0205              add al,[di]
00000BC0  8805              mov [di],al
00000BC2  E8B506            call 0x127a
00000BC5  5F                pop di
00000BC6  58                pop ax
00000BC7  A2EF03            mov [0x3ef],al
00000BCA  E906FD            jmp 0x8d3
00000BCD  80E401            and ah,0x1
00000BD0  B107              mov cl,0x7
00000BD2  D2E4              shl ah,cl
00000BD4  8A1EFB03          mov bl,[0x3fb]
00000BD8  80E37F            and bl,0x7f
00000BDB  0ADC              or bl,ah
00000BDD  881EFB03          mov [0x3fb],bl
00000BE1  B4BD              mov ah,0xbd
00000BE3  8AC3              mov al,bl
00000BE5  E81206            call 0x11fa
00000BE8  E9E8FC            jmp 0x8d3
00000BEB  80E401            and ah,0x1
00000BEE  B106              mov cl,0x6
00000BF0  D2E4              shl ah,cl
00000BF2  8A1EFB03          mov bl,[0x3fb]
00000BF6  80E3BF            and bl,0xbf
00000BF9  0ADC              or bl,ah
00000BFB  881EFB03          mov [0x3fb],bl
00000BFF  B4BD              mov ah,0xbd
00000C01  8AC3              mov al,bl
00000C03  E8F405            call 0x11fa
00000C06  E9CAFC            jmp 0x8d3
00000C09  026520            add ah,[di+0x20]
00000C0C  886520            mov [di+0x20],ah
00000C0F  E86806            call 0x127a
00000C12  E9BEFC            jmp 0x8d3
00000C15  A0EF03            mov al,[0x3ef]
00000C18  50                push ax
00000C19  57                push di
00000C1A  80FC00            cmp ah,0x0
00000C1D  7505              jnz 0xc24
00000C1F  C606F30300        mov byte [0x3f3],0x0
00000C24  8826EF03          mov [0x3ef],ah
00000C28  8ADC              mov bl,ah
00000C2A  B700              mov bh,0x0
00000C2C  D0E3              shl bl,1
00000C2E  8BBFC906          mov di,[bx+0x6c9]
00000C32  C6450500          mov byte [di+0x5],0x0
00000C36  C6450800          mov byte [di+0x8],0x0
00000C3A  C745030000        mov word [di+0x3],0x0
00000C3F  C60500            mov byte [di],0x0
00000C42  803EEF0309        cmp byte [0x3ef],0x9
00000C47  7430              jz 0xc79
00000C49  8ADC              mov bl,ah
00000C4B  32FF              xor bh,bh
00000C4D  8A8FDD06          mov cl,[bx+0x6dd]
00000C51  B4C0              mov ah,0xc0
00000C53  0226EF03          add ah,[0x3ef]
00000C57  B000              mov al,0x0
00000C59  E89E05            call 0x11fa
00000C5C  B443              mov ah,0x43
00000C5E  02E1              add ah,cl
00000C60  B03F              mov al,0x3f
00000C62  E89505            call 0x11fa
00000C65  B483              mov ah,0x83
00000C67  02E1              add ah,cl
00000C69  B0FF              mov al,0xff
00000C6B  E88C05            call 0x11fa
00000C6E  B4B0              mov ah,0xb0
00000C70  0226EF03          add ah,[0x3ef]
00000C74  B000              mov al,0x0
00000C76  E88105            call 0x11fa
00000C79  5F                pop di
00000C7A  58                pop ax
00000C7B  A2EF03            mov [0x3ef],al
00000C7E  E952FC            jmp 0x8d3
00000C81  AC                lodsb
00000C82  8BD8              mov bx,ax
00000C84  E88708            call 0x150e
00000C87  23C3              and ax,bx
00000C89  8A4D28            mov cl,[di+0x28]
00000C8C  8AF9              mov bh,cl
00000C8E  80E120            and cl,0x20
00000C91  8A5D27            mov bl,[di+0x27]
00000C94  80E71F            and bh,0x1f
00000C97  03D8              add bx,ax
00000C99  0AF9              or bh,cl
00000C9B  B4A0              mov ah,0xa0
00000C9D  0226EF03          add ah,[0x3ef]
00000CA1  8AC3              mov al,bl
00000CA3  E85405            call 0x11fa
00000CA6  B4B0              mov ah,0xb0
00000CA8  0226EF03          add ah,[0x3ef]
00000CAC  8AC7              mov al,bh
00000CAE  E84905            call 0x11fa
00000CB1  E91FFC            jmp 0x8d3
00000CB4  4E                dec si
00000CB5  C745290714        mov word [di+0x29],0x1407
00000CBA  E916FC            jmp 0x8d3
00000CBD  886542            mov [di+0x42],ah
00000CC0  8A4541            mov al,[di+0x41]
00000CC3  50                push ax
00000CC4  8AE8              mov ch,al
00000CC6  80E5F0            and ch,0xf0
00000CC9  8A5D07            mov bl,[di+0x7]
00000CCC  02EB              add ch,bl
00000CCE  240F              and al,0xf
00000CD0  8A5D12            mov bl,[di+0x12]
00000CD3  02C3              add al,bl
00000CD5  3C0C              cmp al,0xc
00000CD7  7D0B              jnl 0xce4
00000CD9  3C00              cmp al,0x0
00000CDB  7D0C              jnl 0xce9
00000CDD  040C              add al,0xc
00000CDF  80ED10            sub ch,0x10
00000CE2  EB05              jmp short 0xce9
00000CE4  2C0C              sub al,0xc
00000CE6  80C510            add ch,0x10
00000CE9  8AD8              mov bl,al
00000CEB  B700              mov bh,0x0
00000CED  D0E3              shl bl,1
00000CEF  8B87E606          mov ax,[bx+0x6e6]
00000CF3  2BD2              sub dx,dx
00000CF5  8A5524            mov dl,[di+0x24]
00000CF8  03C2              add ax,dx
00000CFA  D0ED              shr ch,1
00000CFC  D0ED              shr ch,1
00000CFE  80E51C            and ch,0x1c
00000D01  0AEC              or ch,ah
00000D03  8AC8              mov cl,al
00000D05  807D4200          cmp byte [di+0x42],0x0
00000D09  7C21              jl 0xd2c
00000D0B  32FF              xor bh,bh
00000D0D  8A5D41            mov bl,[di+0x41]
00000D10  80E30F            and bl,0xf
00000D13  FEC3              inc bl
00000D15  FEC3              inc bl
00000D17  D1E3              shl bx,1
00000D19  56                push si
00000D1A  8BB70C02          mov si,[bx+0x20c]
00000D1E  32FF              xor bh,bh
00000D20  8A5D42            mov bl,[di+0x42]
00000D23  8A00              mov al,[bx+si]
00000D25  5E                pop si
00000D26  32E4              xor ah,ah
00000D28  03C8              add cx,ax
00000D2A  EB22              jmp short 0xd4e
00000D2C  32FF              xor bh,bh
00000D2E  8A5D41            mov bl,[di+0x41]
00000D31  80E30F            and bl,0xf
00000D34  D1E3              shl bx,1
00000D36  56                push si
00000D37  8BB70C02          mov si,[bx+0x20c]
00000D3B  32FF              xor bh,bh
00000D3D  8A5D42            mov bl,[di+0x42]
00000D40  80F3FF            xor bl,0xff
00000D43  FEC3              inc bl
00000D45  8A00              mov al,[bx+si]
00000D47  5E                pop si
00000D48  32E4              xor ah,ah
00000D4A  2BC8              sub cx,ax
00000D4C  EB00              jmp short 0xd4e
00000D4E  B4A0              mov ah,0xa0
00000D50  8A4528            mov al,[di+0x28]
00000D53  2420              and al,0x20
00000D55  0AE8              or ch,al
00000D57  886D28            mov [di+0x28],ch
00000D5A  884D27            mov [di+0x27],cl
00000D5D  B4A0              mov ah,0xa0
00000D5F  0226EF03          add ah,[0x3ef]
00000D63  8AC1              mov al,cl
00000D65  E89204            call 0x11fa
00000D68  B4B0              mov ah,0xb0
00000D6A  0226EF03          add ah,[0x3ef]
00000D6E  8AC5              mov al,ch
00000D70  E88704            call 0x11fa
00000D73  58                pop ax
00000D74  E95CFB            jmp 0x8d3
00000D77  4E                dec si
00000D78  A0F203            mov al,[0x3f2]
00000D7B  884525            mov [di+0x25],al
00000D7E  E952FB            jmp 0x8d3
00000D81  4E                dec si
00000D82  E94EFB            jmp 0x8d3
00000D85  886536            mov [di+0x36],ah
00000D88  E948FB            jmp 0x8d3
00000D8B  0AE4              or ah,ah
00000D8D  7809              js 0xd98
00000D8F  026525            add ah,[di+0x25]
00000D92  730E              jnc 0xda2
00000D94  B4FF              mov ah,0xff
00000D96  EB0A              jmp short 0xda2
00000D98  026525            add ah,[di+0x25]
00000D9B  3A6525            cmp ah,[di+0x25]
00000D9E  7202              jc 0xda2
00000DA0  B401              mov ah,0x1
00000DA2  886525            mov [di+0x25],ah
00000DA5  E92BFB            jmp 0x8d3
00000DA8  E928FB            jmp 0x8d3
00000DAB  A0EF03            mov al,[0x3ef]
00000DAE  50                push ax
00000DAF  A0FC03            mov al,[0x3fc]
00000DB2  50                push ax
00000DB3  56                push si
00000DB4  C606EF0306        mov byte [0x3ef],0x6
00000DB9  BBDD06            mov bx,0x6dd
00000DBC  80C306            add bl,0x6
00000DBF  8A07              mov al,[bx]
00000DC1  A2FC03            mov [0x3fc],al
00000DC4  BA8803            mov dx,0x388
00000DC7  8A0EFC03          mov cl,[0x3fc]
00000DCB  2BDB              sub bx,bx
00000DCD  8ADC              mov bl,ah
00000DCF  D1E3              shl bx,1
00000DD1  8BB70D1B          mov si,[bx+0x1b0d]
00000DD5  8A4406            mov al,[si+0x6]
00000DD8  A2FD03            mov [0x3fd],al
00000DDB  E87405            call 0x1352
00000DDE  5E                pop si
00000DDF  AC                lodsb
00000DE0  56                push si
00000DE1  C606EF0307        mov byte [0x3ef],0x7
00000DE6  BBDD06            mov bx,0x6dd
00000DE9  80C307            add bl,0x7
00000DEC  8A27              mov ah,[bx]
00000DEE  8826FC03          mov [0x3fc],ah
00000DF2  BA8803            mov dx,0x388
00000DF5  8A0EFC03          mov cl,[0x3fc]
00000DF9  2BDB              sub bx,bx
00000DFB  8AD8              mov bl,al
00000DFD  D1E3              shl bx,1
00000DFF  8BB70D1B          mov si,[bx+0x1b0d]
00000E03  8A4405            mov al,[si+0x5]
00000E06  A2FF03            mov [0x3ff],al
00000E09  8A4406            mov al,[si+0x6]
00000E0C  A2FE03            mov [0x3fe],al
00000E0F  E84005            call 0x1352
00000E12  5E                pop si
00000E13  AC                lodsb
00000E14  56                push si
00000E15  C606EF0308        mov byte [0x3ef],0x8
00000E1A  BBDD06            mov bx,0x6dd
00000E1D  80C308            add bl,0x8
00000E20  8A27              mov ah,[bx]
00000E22  8826FC03          mov [0x3fc],ah
00000E26  BA8803            mov dx,0x388
00000E29  8A0EFC03          mov cl,[0x3fc]
00000E2D  2BDB              sub bx,bx
00000E2F  8AD8              mov bl,al
00000E31  D1E3              shl bx,1
00000E33  8BB70D1B          mov si,[bx+0x1b0d]
00000E37  8A4405            mov al,[si+0x5]
00000E3A  A20004            mov [0x400],al
00000E3D  8A4406            mov al,[si+0x6]
00000E40  A20104            mov [0x401],al
00000E43  E80C05            call 0x1352
00000E46  5E                pop si
00000E47  AC                lodsb
00000E48  242F              and al,0x2f
00000E4A  A2E505            mov [0x5e5],al
00000E4D  B4B6              mov ah,0xb6
00000E4F  E8A803            call 0x11fa
00000E52  AC                lodsb
00000E53  B4A6              mov ah,0xa6
00000E55  E8A203            call 0x11fa
00000E58  AC                lodsb
00000E59  242F              and al,0x2f
00000E5B  A22806            mov [0x628],al
00000E5E  B4B7              mov ah,0xb7
00000E60  E89703            call 0x11fa
00000E63  AC                lodsb
00000E64  B4A7              mov ah,0xa7
00000E66  E89103            call 0x11fa
00000E69  AC                lodsb
00000E6A  242F              and al,0x2f
00000E6C  A26B06            mov [0x66b],al
00000E6F  B4B8              mov ah,0xb8
00000E71  E88603            call 0x11fa
00000E74  AC                lodsb
00000E75  B4A8              mov ah,0xa8
00000E77  E88003            call 0x11fa
00000E7A  C606F10320        mov byte [0x3f1],0x20
00000E7F  58                pop ax
00000E80  A2FC03            mov [0x3fc],al
00000E83  58                pop ax
00000E84  A2EF03            mov [0x3ef],al
00000E87  E949FA            jmp 0x8d3
00000E8A  BA8803            mov dx,0x388
00000E8D  B0BD              mov al,0xbd
00000E8F  EE                out dx,al
00000E90  E8D703            call 0x126a
00000E93  42                inc dx
00000E94  86E0              xchg ah,al
00000E96  50                push ax
00000E97  241F              and al,0x1f
00000E99  34FF              xor al,0xff
00000E9B  2206F103          and al,[0x3f1]
00000E9F  0C20              or al,0x20
00000EA1  EE                out dx,al
00000EA2  E8B503            call 0x125a
00000EA5  58                pop ax
00000EA6  0A06F103          or al,[0x3f1]
00000EAA  A2F103            mov [0x3f1],al
00000EAD  0A06FB03          or al,[0x3fb]
00000EB1  0C20              or al,0x20
00000EB3  EE                out dx,al
00000EB4  E8A303            call 0x125a
00000EB7  E919FA            jmp 0x8d3
00000EBA  4E                dec si
00000EBB  A0FB03            mov al,[0x3fb]
00000EBE  C606F10300        mov byte [0x3f1],0x0
00000EC3  24C0              and al,0xc0
00000EC5  B4BD              mov ah,0xbd
00000EC7  E83003            call 0x11fa
00000ECA  E906FA            jmp 0x8d3
00000ECD  AC                lodsb
00000ECE  8BD8              mov bx,ax
00000ED0  F6C401            test ah,0x1
00000ED3  7419              jz 0xeee
00000ED5  A20904            mov [0x409],al
00000ED8  32E4              xor ah,ah
00000EDA  0206FF03          add al,[0x3ff]
00000EDE  02060404          add al,[0x404]
00000EE2  02060904          add al,[0x409]
00000EE6  E82203            call 0x120b
00000EE9  B451              mov ah,0x51
00000EEB  E80C03            call 0x11fa
00000EEE  8BC3              mov ax,bx
00000EF0  F6C402            test ah,0x2
00000EF3  7419              jz 0xf0e
00000EF5  A20B04            mov [0x40b],al
00000EF8  32E4              xor ah,ah
00000EFA  02060104          add al,[0x401]
00000EFE  02060604          add al,[0x406]
00000F02  02060B04          add al,[0x40b]
00000F06  E80203            call 0x120b
00000F09  B455              mov ah,0x55
00000F0B  E8EC02            call 0x11fa
00000F0E  8BC3              mov ax,bx
00000F10  F6C404            test ah,0x4
00000F13  7419              jz 0xf2e
00000F15  A20A04            mov [0x40a],al
00000F18  32E4              xor ah,ah
00000F1A  02060004          add al,[0x400]
00000F1E  02060504          add al,[0x405]
00000F22  02060A04          add al,[0x40a]
00000F26  E8E202            call 0x120b
00000F29  B452              mov ah,0x52
00000F2B  E8CC02            call 0x11fa
00000F2E  8BC3              mov ax,bx
00000F30  F6C408            test ah,0x8
00000F33  7419              jz 0xf4e
00000F35  A20804            mov [0x408],al
00000F38  32E4              xor ah,ah
00000F3A  0206FE03          add al,[0x3fe]
00000F3E  02060304          add al,[0x403]
00000F42  02060804          add al,[0x408]
00000F46  E8C202            call 0x120b
00000F49  B454              mov ah,0x54
00000F4B  E8AC02            call 0x11fa
00000F4E  8BC3              mov ax,bx
00000F50  F6C410            test ah,0x10
00000F53  7419              jz 0xf6e
00000F55  A20704            mov [0x407],al
00000F58  32E4              xor ah,ah
00000F5A  0206FD03          add al,[0x3fd]
00000F5E  02060204          add al,[0x402]
00000F62  02060704          add al,[0x407]
00000F66  E8A202            call 0x120b
00000F69  B453              mov ah,0x53
00000F6B  E88C02            call 0x11fa
00000F6E  E962F9            jmp 0x8d3
00000F71  AC                lodsb
00000F72  8BD8              mov bx,ax
00000F74  F6C401            test ah,0x1
00000F77  7419              jz 0xf92
00000F79  32E4              xor ah,ah
00000F7B  0206FF03          add al,[0x3ff]
00000F7F  02060404          add al,[0x404]
00000F83  02060904          add al,[0x409]
00000F87  E88102            call 0x120b
00000F8A  A20404            mov [0x404],al
00000F8D  B451              mov ah,0x51
00000F8F  E86802            call 0x11fa
00000F92  8BC3              mov ax,bx
00000F94  F6C402            test ah,0x2
00000F97  7419              jz 0xfb2
00000F99  32E4              xor ah,ah
00000F9B  02060104          add al,[0x401]
00000F9F  02060604          add al,[0x406]
00000FA3  02060B04          add al,[0x40b]
00000FA7  E86102            call 0x120b
00000FAA  A20604            mov [0x406],al
00000FAD  B455              mov ah,0x55
00000FAF  E84802            call 0x11fa
00000FB2  8BC3              mov ax,bx
00000FB4  F6C404            test ah,0x4
00000FB7  7419              jz 0xfd2
00000FB9  32E4              xor ah,ah
00000FBB  02060004          add al,[0x400]
00000FBF  02060504          add al,[0x405]
00000FC3  02060A04          add al,[0x40a]
00000FC7  E84102            call 0x120b
00000FCA  A20504            mov [0x405],al
00000FCD  B452              mov ah,0x52
00000FCF  E82802            call 0x11fa
00000FD2  8BC3              mov ax,bx
00000FD4  F6C408            test ah,0x8
00000FD7  7419              jz 0xff2
00000FD9  32E4              xor ah,ah
00000FDB  0206FE03          add al,[0x3fe]
00000FDF  02060304          add al,[0x403]
00000FE3  02060804          add al,[0x408]
00000FE7  E82102            call 0x120b
00000FEA  A20304            mov [0x403],al
00000FED  B454              mov ah,0x54
00000FEF  E80802            call 0x11fa
00000FF2  8BC3              mov ax,bx
00000FF4  F6C410            test ah,0x10
00000FF7  7419              jz 0x1012
00000FF9  32E4              xor ah,ah
00000FFB  0206FD03          add al,[0x3fd]
00000FFF  02060204          add al,[0x402]
00001003  02060704          add al,[0x407]
00001007  E80102            call 0x120b
0000100A  A20204            mov [0x402],al
0000100D  B453              mov ah,0x53
0000100F  E8E801            call 0x11fa
00001012  E9BEF8            jmp 0x8d3
00001015  AC                lodsb
00001016  8BD8              mov bx,ax
00001018  F6C401            test ah,0x1
0000101B  7415              jz 0x1032
0000101D  A20404            mov [0x404],al
00001020  32E4              xor ah,ah
00001022  0206FF03          add al,[0x3ff]
00001026  02060904          add al,[0x409]
0000102A  E8DE01            call 0x120b
0000102D  B451              mov ah,0x51
0000102F  E8C801            call 0x11fa
00001032  8BC3              mov ax,bx
00001034  F6C402            test ah,0x2
00001037  7415              jz 0x104e
00001039  A20604            mov [0x406],al
0000103C  32E4              xor ah,ah
0000103E  02060104          add al,[0x401]
00001042  02060B04          add al,[0x40b]
00001046  E8C201            call 0x120b
00001049  B455              mov ah,0x55
0000104B  E8AC01            call 0x11fa
0000104E  8BC3              mov ax,bx
00001050  F6C404            test ah,0x4
00001053  7415              jz 0x106a
00001055  A20504            mov [0x405],al
00001058  32E4              xor ah,ah
0000105A  02060004          add al,[0x400]
0000105E  02060A04          add al,[0x40a]
00001062  E8A601            call 0x120b
00001065  B452              mov ah,0x52
00001067  E89001            call 0x11fa
0000106A  8BC3              mov ax,bx
0000106C  F6C408            test ah,0x8
0000106F  7415              jz 0x1086
00001071  A20304            mov [0x403],al
00001074  32E4              xor ah,ah
00001076  0206FE03          add al,[0x3fe]
0000107A  02060804          add al,[0x408]
0000107E  E88A01            call 0x120b
00001081  B454              mov ah,0x54
00001083  E87401            call 0x11fa
00001086  8BC3              mov ax,bx
00001088  F6C410            test ah,0x10
0000108B  7415              jz 0x10a2
0000108D  A20204            mov [0x402],al
00001090  32E4              xor ah,ah
00001092  0206FD03          add al,[0x3fd]
00001096  02060704          add al,[0x407]
0000109A  E86E01            call 0x120b
0000109D  B453              mov ah,0x53
0000109F  E85801            call 0x11fa
000010A2  E92EF8            jmp 0x8d3
000010A5  886540            mov [di+0x40],ah
000010A8  E928F8            jmp 0x8d3
000010AB  886532            mov [di+0x32],ah
000010AE  AC                lodsb
000010AF  884533            mov [di+0x33],al
000010B2  E91EF8            jmp 0x8d3
000010B5  50                push ax
000010B6  884541            mov [di+0x41],al
000010B9  8AE8              mov ch,al
000010BB  80E5F0            and ch,0xf0
000010BE  8A5D07            mov bl,[di+0x7]
000010C1  02EB              add ch,bl
000010C3  240F              and al,0xf
000010C5  8A5D12            mov bl,[di+0x12]
000010C8  02C3              add al,bl
000010CA  3C0C              cmp al,0xc
000010CC  7D0B              jnl 0x10d9
000010CE  3C00              cmp al,0x0
000010D0  7D0C              jnl 0x10de
000010D2  040C              add al,0xc
000010D4  80ED10            sub ch,0x10
000010D7  EB05              jmp short 0x10de
000010D9  2C0C              sub al,0xc
000010DB  80C510            add ch,0x10
000010DE  8AD8              mov bl,al
000010E0  B700              mov bh,0x0
000010E2  D0E3              shl bl,1
000010E4  8B87E606          mov ax,[bx+0x6e6]
000010E8  2BD2              sub dx,dx
000010EA  8A5524            mov dl,[di+0x24]
000010ED  03C2              add ax,dx
000010EF  D0ED              shr ch,1
000010F1  D0ED              shr ch,1
000010F3  80E51C            and ch,0x1c
000010F6  0AEC              or ch,ah
000010F8  8AC8              mov cl,al
000010FA  807D4200          cmp byte [di+0x42],0x0
000010FE  7443              jz 0x1143
00001100  7C21              jl 0x1123
00001102  32FF              xor bh,bh
00001104  8A5D41            mov bl,[di+0x41]
00001107  80E30F            and bl,0xf
0000110A  FEC3              inc bl
0000110C  FEC3              inc bl
0000110E  D1E3              shl bx,1
00001110  56                push si
00001111  8BB70C02          mov si,[bx+0x20c]
00001115  32FF              xor bh,bh
00001117  8A5D42            mov bl,[di+0x42]
0000111A  8A00              mov al,[bx+si]
0000111C  5E                pop si
0000111D  32E4              xor ah,ah
0000111F  03C8              add cx,ax
00001121  EB20              jmp short 0x1143
00001123  32FF              xor bh,bh
00001125  8A5D41            mov bl,[di+0x41]
00001128  80E30F            and bl,0xf
0000112B  D1E3              shl bx,1
0000112D  56                push si
0000112E  8BB70C02          mov si,[bx+0x20c]
00001132  32FF              xor bh,bh
00001134  8A5D42            mov bl,[di+0x42]
00001137  80F3FF            xor bl,0xff
0000113A  FEC3              inc bl
0000113C  8A00              mov al,[bx+si]
0000113E  5E                pop si
0000113F  32E4              xor ah,ah
00001141  2BC8              sub cx,ax
00001143  B4A0              mov ah,0xa0
00001145  8A4528            mov al,[di+0x28]
00001148  2420              and al,0x20
0000114A  0AE8              or ch,al
0000114C  886D28            mov [di+0x28],ch
0000114F  884D27            mov [di+0x27],cl
00001152  B4A0              mov ah,0xa0
00001154  0226EF03          add ah,[0x3ef]
00001158  8AC1              mov al,cl
0000115A  E89D00            call 0x11fa
0000115D  B4B0              mov ah,0xb0
0000115F  0226EF03          add ah,[0x3ef]
00001163  8AC5              mov al,ch
00001165  E89200            call 0x11fa
00001168  58                pop ax
00001169  C3                ret
0000116A  50                push ax
0000116B  B4B0              mov ah,0xb0
0000116D  0226EF03          add ah,[0x3ef]
00001171  8A4528            mov al,[di+0x28]
00001174  0C20              or al,0x20
00001176  884528            mov [di+0x28],al
00001179  E87E00            call 0x11fa
0000117C  8A4D19            mov cl,[di+0x19]
0000117F  B009              mov al,0x9
00001181  2AC1              sub al,cl
00001183  8AC8              mov cl,al
00001185  8B4527            mov ax,[di+0x27]
00001188  25FF03            and ax,0x3ff
0000118B  D3E8              shr ax,cl
0000118D  B400              mov ah,0x0
0000118F  894517            mov [di+0x17],ax
00001192  8A4D1C            mov cl,[di+0x1c]
00001195  884D1F            mov [di+0x1f],cl
00001198  58                pop ax
00001199  C3                ret
0000119A  803EF10300        cmp byte [0x3f1],0x0
0000119F  7408              jz 0x11a9
000011A1  803EEF0306        cmp byte [0x3ef],0x6
000011A6  7C01              jl 0x11a9
000011A8  C3                ret
000011A9  803EEF0309        cmp byte [0x3ef],0x9
000011AE  7411              jz 0x11c1
000011B0  B4B0              mov ah,0xb0
000011B2  0226EF03          add ah,[0x3ef]
000011B6  8A4528            mov al,[di+0x28]
000011B9  24DF              and al,0xdf
000011BB  884528            mov [di+0x28],al
000011BE  E83900            call 0x11fa
000011C1  C3                ret
000011C2  8826EC03          mov [0x3ec],ah
000011C6  807D3600          cmp byte [di+0x36],0x0
000011CA  8ADC              mov bl,ah
000011CC  740C              jz 0x11da
000011CE  E83D03            call 0x150e
000011D1  224536            and al,[di+0x36]
000011D4  02D8              add bl,al
000011D6  885D05            mov [di+0x5],bl
000011D9  C3                ret
000011DA  807D2D00          cmp byte [di+0x2d],0x0
000011DE  7416              jz 0x11f6
000011E0  8ADC              mov bl,ah
000011E2  D0EC              shr ah,1
000011E4  D0EC              shr ah,1
000011E6  D0EC              shr ah,1
000011E8  8A4D2D            mov cl,[di+0x2d]
000011EB  32ED              xor ch,ch
000011ED  32C0              xor al,al
000011EF  02C4              add al,ah
000011F1  E2FC              loop 0x11ef
000011F3  884521            mov [di+0x21],al
000011F6  885D05            mov [di+0x5],bl
000011F9  C3                ret
000011FA  BA8803            mov dx,0x388
000011FD  86C4              xchg al,ah
000011FF  EE                out dx,al
00001200  E86700            call 0x126a
00001203  42                inc dx
00001204  86C4              xchg al,ah
00001206  EE                out dx,al
00001207  E85000            call 0x125a
0000120A  C3                ret
0000120B  3D0000            cmp ax,0x0
0000120E  7C06              jl 0x1216
00001210  3D3F00            cmp ax,0x3f
00001213  7F04              jg 0x1219
00001215  C3                ret
00001216  B000              mov al,0x0
00001218  C3                ret
00001219  B03F              mov al,0x3f
0000121B  C3                ret
0000121C  A00E04            mov al,[0x40e]
0000121F  0206F203          add al,[0x3f2]
00001223  A20E04            mov [0x40e],al
00001226  7201              jc 0x1229
00001228  C3                ret
00001229  FE0E0D04          dec byte [0x40d]
0000122D  7401              jz 0x1230
0000122F  C3                ret
00001230  A00C04            mov al,[0x40c]
00001233  A20D04            mov [0x40d],al
00001236  FE060F04          inc byte [0x40f]
0000123A  C3                ret
0000123B  55                push bp
0000123C  8BEC              mov bp,sp
0000123E  1E                push ds
0000123F  57                push di
00001240  8CC8              mov ax,cs
00001242  8ED8              mov ds,ax
00001244  8B5E06            mov bx,[bp+0x6]
00001247  2AFF              sub bh,bh
00001249  D1E3              shl bx,1
0000124B  8BBFC906          mov di,[bx+0x6c9]
0000124F  8A4505            mov al,[di+0x5]
00001252  32E4              xor ah,ah
00001254  5F                pop di
00001255  1F                pop ds
00001256  8BE5              mov sp,bp
00001258  5D                pop bp
00001259  CB                retf
0000125A  50                push ax
0000125B  51                push cx
0000125C  52                push dx
0000125D  BA8803            mov dx,0x388
00001260  B91700            mov cx,0x17
00001263  EC                in al,dx
00001264  E2FD              loop 0x1263
00001266  5A                pop dx
00001267  59                pop cx
00001268  58                pop ax
00001269  C3                ret
0000126A  50                push ax
0000126B  51                push cx
0000126C  52                push dx
0000126D  BA8803            mov dx,0x388
00001270  B90400            mov cx,0x4
00001273  EC                in al,dx
00001274  E2FD              loop 0x1273
00001276  5A                pop dx
00001277  59                pop cx
00001278  58                pop ax
00001279  C3                ret
0000127A  E82C00            call 0x12a9
0000127D  BBDD06            mov bx,0x6dd
00001280  021EEF03          add bl,[0x3ef]
00001284  8A0F              mov cl,[bx]
00001286  8AE1              mov ah,cl
00001288  80C443            add ah,0x43
0000128B  E86CFF            call 0x11fa
0000128E  807D3100          cmp byte [di+0x31],0x0
00001292  7414              jz 0x12a8
00001294  E84A00            call 0x12e1
00001297  BBDD06            mov bx,0x6dd
0000129A  021EEF03          add bl,[0x3ef]
0000129E  8A0F              mov cl,[bx]
000012A0  8AE1              mov ah,cl
000012A2  80C440            add ah,0x40
000012A5  E852FF            call 0x11fa
000012A8  C3                ret
000012A9  8A452F            mov al,[di+0x2f]
000012AC  243F              and al,0x3f
000012AE  024520            add al,[di+0x20]
000012B1  0205              add al,[di]
000012B3  024530            add al,[di+0x30]
000012B6  807D3300          cmp byte [di+0x33],0x0
000012BA  740E              jz 0x12ca
000012BC  8A4D33            mov cl,[di+0x33]
000012BF  FEC1              inc cl
000012C1  32FF              xor bh,bh
000012C3  8A5D34            mov bl,[di+0x34]
000012C6  D3E3              shl bx,cl
000012C8  2AC7              sub al,bh
000012CA  3C3F              cmp al,0x3f
000012CC  760A              jna 0x12d8
000012CE  3C00              cmp al,0x0
000012D0  7C04              jl 0x12d6
000012D2  B03F              mov al,0x3f
000012D4  EB02              jmp short 0x12d8
000012D6  B000              mov al,0x0
000012D8  8A5D2F            mov bl,[di+0x2f]
000012DB  80E3C0            and bl,0xc0
000012DE  0AC3              or al,bl
000012E0  C3                ret
000012E1  8A452E            mov al,[di+0x2e]
000012E4  243F              and al,0x3f
000012E6  807D3100          cmp byte [di+0x31],0x0
000012EA  7408              jz 0x12f4
000012EC  024520            add al,[di+0x20]
000012EF  0205              add al,[di]
000012F1  024530            add al,[di+0x30]
000012F4  807D3200          cmp byte [di+0x32],0x0
000012F8  740E              jz 0x1308
000012FA  8A4D32            mov cl,[di+0x32]
000012FD  FEC1              inc cl
000012FF  32FF              xor bh,bh
00001301  8A5D34            mov bl,[di+0x34]
00001304  D3E3              shl bx,cl
00001306  2AC7              sub al,bh
00001308  3C3F              cmp al,0x3f
0000130A  760A              jna 0x1316
0000130C  3C00              cmp al,0x0
0000130E  7C04              jl 0x1314
00001310  B03F              mov al,0x3f
00001312  EB02              jmp short 0x1316
00001314  B000              mov al,0x0
00001316  8A5D2E            mov bl,[di+0x2e]
00001319  80E3C0            and bl,0xc0
0000131C  0AC3              or al,bl
0000131E  C3                ret
0000131F  06                push es
00001320  50                push ax
00001321  51                push cx
00001322  8CD9              mov cx,ds
00001324  8EC1              mov es,cx
00001326  B94000            mov cx,0x40
00001329  2BC0              sub ax,ax
0000132B  57                push di
0000132C  83C703            add di,byte +0x3
0000132F  D1E9              shr cx,1
00001331  F2AB              repne stosw
00001333  13C9              adc cx,cx
00001335  F2AA              repne stosb
00001337  5F                pop di
00001338  C64525FF          mov byte [di+0x25],0xff
0000133C  C6450800          mov byte [di+0x8],0x0
00001340  C745290714        mov word [di+0x29],0x1407
00001345  C7452B0714        mov word [di+0x2b],0x1407
0000134A  C6453501          mov byte [di+0x35],0x1
0000134E  59                pop cx
0000134F  58                pop ax
00001350  07                pop es
00001351  C3                ret
00001352  B420              mov ah,0x20
00001354  02E1              add ah,cl
00001356  AC                lodsb
00001357  E8A0FE            call 0x11fa
0000135A  B423              mov ah,0x23
0000135C  02E1              add ah,cl
0000135E  AC                lodsb
0000135F  E898FE            call 0x11fa
00001362  B4C0              mov ah,0xc0
00001364  0226EF03          add ah,[0x3ef]
00001368  AC                lodsb
00001369  E88EFE            call 0x11fa
0000136C  2401              and al,0x1
0000136E  884531            mov [di+0x31],al
00001371  B4E0              mov ah,0xe0
00001373  02E1              add ah,cl
00001375  AC                lodsb
00001376  E881FE            call 0x11fa
00001379  B4E3              mov ah,0xe3
0000137B  02E1              add ah,cl
0000137D  AC                lodsb
0000137E  E879FE            call 0x11fa
00001381  B440              mov ah,0x40
00001383  02E1              add ah,cl
00001385  AC                lodsb
00001386  88452E            mov [di+0x2e],al
00001389  E855FF            call 0x12e1
0000138C  E86BFE            call 0x11fa
0000138F  B443              mov ah,0x43
00001391  02E1              add ah,cl
00001393  AC                lodsb
00001394  88452F            mov [di+0x2f],al
00001397  E80FFF            call 0x12a9
0000139A  E85DFE            call 0x11fa
0000139D  B460              mov ah,0x60
0000139F  02E1              add ah,cl
000013A1  AC                lodsb
000013A2  E855FE            call 0x11fa
000013A5  B463              mov ah,0x63
000013A7  02E1              add ah,cl
000013A9  AC                lodsb
000013AA  E84DFE            call 0x11fa
000013AD  B480              mov ah,0x80
000013AF  02E1              add ah,cl
000013B1  AC                lodsb
000013B2  E845FE            call 0x11fa
000013B5  B483              mov ah,0x83
000013B7  02E1              add ah,cl
000013B9  AC                lodsb
000013BA  E83DFE            call 0x11fa
000013BD  C3                ret
000013BE  803EF10300        cmp byte [0x3f1],0x0
000013C3  7406              jz 0x13cb
000013C5  80F906            cmp cl,0x6
000013C8  7C01              jl 0x13cb
000013CA  C3                ret
000013CB  BBDD06            mov bx,0x6dd
000013CE  8AE9              mov ch,cl
000013D0  02D9              add bl,cl
000013D2  8A0F              mov cl,[bx]
000013D4  B460              mov ah,0x60
000013D6  02E1              add ah,cl
000013D8  B0FF              mov al,0xff
000013DA  E81DFE            call 0x11fa
000013DD  B463              mov ah,0x63
000013DF  02E1              add ah,cl
000013E1  B0FF              mov al,0xff
000013E3  E814FE            call 0x11fa
000013E6  B480              mov ah,0x80
000013E8  02E1              add ah,cl
000013EA  B0FF              mov al,0xff
000013EC  E80BFE            call 0x11fa
000013EF  B483              mov ah,0x83
000013F1  02E1              add ah,cl
000013F3  B0FF              mov al,0xff
000013F5  E802FE            call 0x11fa
000013F8  B4B0              mov ah,0xb0
000013FA  02E5              add ah,ch
000013FC  B000              mov al,0x0
000013FE  E8F9FD            call 0x11fa
00001401  B020              mov al,0x20
00001403  E8F4FD            call 0x11fa
00001406  C3                ret
00001407  C3                ret
00001408  807D1F00          cmp byte [di+0x1f],0x0
0000140C  7404              jz 0x1412
0000140E  FE4D1F            dec byte [di+0x1f]
00001411  C3                ret
00001412  8A4D1E            mov cl,[di+0x1e]
00001415  024D1D            add cl,[di+0x1d]
00001418  884D1E            mov [di+0x1e],cl
0000141B  7201              jc 0x141e
0000141D  C3                ret
0000141E  8B5D17            mov bx,[di+0x17]
00001421  FE4D1A            dec byte [di+0x1a]
00001424  750D              jnz 0x1433
00001426  83F3FF            xor bx,byte -0x1
00001429  43                inc bx
0000142A  895D17            mov [di+0x17],bx
0000142D  8A451B            mov al,[di+0x1b]
00001430  88451A            mov [di+0x1a],al
00001433  8B4527            mov ax,[di+0x27]
00001436  25FF03            and ax,0x3ff
00001439  03C3              add ax,bx
0000143B  884527            mov [di+0x27],al
0000143E  8A4528            mov al,[di+0x28]
00001441  24FC              and al,0xfc
00001443  0AC4              or al,ah
00001445  884528            mov [di+0x28],al
00001448  B4A0              mov ah,0xa0
0000144A  0226EF03          add ah,[0x3ef]
0000144E  8A4527            mov al,[di+0x27]
00001451  E8A6FD            call 0x11fa
00001454  B4B0              mov ah,0xb0
00001456  0226EF03          add ah,[0x3ef]
0000145A  8A4528            mov al,[di+0x28]
0000145D  E89AFD            call 0x11fa
00001460  C3                ret
00001461  8A453A            mov al,[di+0x3a]
00001464  024539            add al,[di+0x39]
00001467  88453A            mov [di+0x3a],al
0000146A  7201              jc 0x146d
0000146C  C3                ret
0000146D  FE4D3C            dec byte [di+0x3c]
00001470  7906              jns 0x1478
00001472  8A453B            mov al,[di+0x3b]
00001475  88453C            mov [di+0x3c],al
00001478  8A653D            mov ah,[di+0x3d]
0000147B  0226FC03          add ah,[0x3fc]
0000147F  8A453C            mov al,[di+0x3c]
00001482  8B5D3E            mov bx,[di+0x3e]
00001485  D7                xlatb
00001486  E871FD            call 0x11fa
00001489  C3                ret
0000148A  8A4D14            mov cl,[di+0x14]
0000148D  024D13            add cl,[di+0x13]
00001490  884D14            mov [di+0x14],cl
00001493  7201              jc 0x1496
00001495  C3                ret
00001496  8A5D27            mov bl,[di+0x27]
00001499  8A7D28            mov bh,[di+0x28]
0000149C  80E703            and bh,0x3
0000149F  8A5528            mov dl,[di+0x28]
000014A2  8AF2              mov dh,dl
000014A4  80E21C            and dl,0x1c
000014A7  80E620            and dh,0x20
000014AA  8B4D15            mov cx,[di+0x15]
000014AD  0BC9              or cx,cx
000014AF  7C1C              jl 0x14cd
000014B1  03D9              add bx,cx
000014B3  81FBDE02          cmp bx,0x2de
000014B7  7C2E              jl 0x14e7
000014B9  D1EB              shr bx,1
000014BB  53                push bx
000014BC  81E3FF03          and bx,0x3ff
000014C0  7502              jnz 0x14c4
000014C2  5B                pop bx
000014C3  43                inc bx
000014C4  5B                pop bx
000014C5  80C204            add dl,0x4
000014C8  80E21C            and dl,0x1c
000014CB  EB1A              jmp short 0x14e7
000014CD  03D9              add bx,cx
000014CF  81FB8401          cmp bx,0x184
000014D3  7F12              jg 0x14e7
000014D5  D1E3              shl bx,1
000014D7  53                push bx
000014D8  81E3FF03          and bx,0x3ff
000014DC  7502              jnz 0x14e0
000014DE  5B                pop bx
000014DF  4B                dec bx
000014E0  5B                pop bx
000014E1  80EA04            sub dl,0x4
000014E4  80E21C            and dl,0x1c
000014E7  81E3FF03          and bx,0x3ff
000014EB  8BCA              mov cx,dx
000014ED  B4A0              mov ah,0xa0
000014EF  0226EF03          add ah,[0x3ef]
000014F3  8AC3              mov al,bl
000014F5  E802FD            call 0x11fa
000014F8  884527            mov [di+0x27],al
000014FB  B4B0              mov ah,0xb0
000014FD  0226EF03          add ah,[0x3ef]
00001501  0AF9              or bh,cl
00001503  0AFD              or bh,ch
00001505  8AC7              mov al,bh
00001507  E8F0FC            call 0x11fa
0000150A  884528            mov [di+0x28],al
0000150D  C3                ret
0000150E  A1F803            mov ax,[0x3f8]
00001511  054892            add ax,0x9248
00001514  D1C8              ror ax,1
00001516  D1C8              ror ax,1
00001518  D1C8              ror ax,1
0000151A  A3F803            mov [0x3f8],ax
0000151D  C3                ret
0000151E  46                inc si
0000151F  094B09            or [bp+di+0x9],cx
00001522  53                push bx
00001523  09A209A8          or [bp+si-0x57f7],sp
00001527  09B409CC          or [si-0x33f7],si
0000152B  09DF              or di,bx
0000152D  09E5              or bp,sp
0000152F  09FB              or bx,di
00001531  0918              or [bx+si],bx
00001533  0A1F              or bl,[bx]
00001535  0A3A              or bh,[bp+si]
00001537  0A400A            or al,[bx+si+0xa]
0000153A  5D                pop bp
0000153B  0AE5              or ah,ch
0000153D  09810A97          or [bx+di-0x68f6],ax
00001541  0AAD0ABB          or ch,[di-0x44f6]
00001545  0AE5              or ah,ch
00001547  09C1              or cx,ax
00001549  0AE5              or ah,ch
0000154B  09E5              or bp,sp
0000154D  09E5              or bp,sp
0000154F  09E5              or bp,sp
00001551  09E0              or ax,sp
00001553  0AE5              or ah,ch
00001555  09E6              or si,sp
00001557  0A02              or al,[bp+si]
00001559  0B32              or si,[bp+si]
0000155B  0BE5              or sp,bp
0000155D  093B              or [bp+di],di
0000155F  0B4E0B            or cx,[bp+0xb]
00001562  E509              in ax,0x9
00001564  E509              in ax,0x9
00001566  640BE5            fs or sp,bp
00001569  096D0B            or [di+0xb],bp
0000156C  740B              jz 0x1579
0000156E  E509              in ax,0x9
00001570  7D0B              jnl 0x157d
00001572  E509              in ax,0x9
00001574  830B89            or word [bp+di],byte -0x77
00001577  0BAA0BCD          or bp,[bp+si-0x32f5]
0000157B  0BEB              or bp,bx
0000157D  0B09              or cx,[bx+di]
0000157F  0CE5              or al,0xe5
00001581  09E5              or bp,sp
00001583  0915              or [di],dx
00001585  0CE5              or al,0xe5
00001587  09810CB4          or [bx+di-0x4bf4],ax
0000158B  0CE5              or al,0xe5
0000158D  09E5              or bp,sp
0000158F  09BD0C77          or [di+0x770c],di
00001593  0D810D            or ax,0xd81
00001596  850D              test [di],cx
00001598  8B0D              mov cx,[di]
0000159A  E509              in ax,0x9
0000159C  A80D              test al,0xd
0000159E  A80D              test al,0xd
000015A0  AB                stosw
000015A1  0D8A0E            or ax,0xe8a
000015A4  BA0ECD            mov dx,0xcd0e
000015A7  0E                push cs
000015A8  710F              jno 0x15b9
000015AA  1510E5            adc ax,0xe510
000015AD  09A510AB          or [di-0x54f0],sp
000015B1  10B615B6          adc [bp-0x49eb],dh
000015B5  156416            adc ax,0x1664
000015B8  7D16              jnl 0x15d0
000015BA  88169316          mov [0x1693],dl
000015BE  A4                movsb
000015BF  16                push ss
000015C0  B516              mov ch,0x16
000015C2  C6                db 0xc6
000015C3  16                push ss
000015C4  D7                xlatb
000015C5  16                push ss
000015C6  E616              out 0x16,al
000015C8  F5                cmc
000015C9  16                push ss
000015CA  0017              add [bx],dl
000015CC  0F171A            movhps qword [bp+si],xmm3
000015CF  17                pop ss
000015D0  2917              sub [bx],dx
000015D2  47                inc di
000015D3  17                pop ss
000015D4  52                push dx
000015D5  17                pop ss
000015D6  5D                pop bp
000015D7  17                pop ss
000015D8  681773            push word 0x7317
000015DB  17                pop ss
000015DC  8C17              mov [bx],ss
000015DE  9D                popf
000015DF  17                pop ss
000015E0  A6                cmpsb
000015E1  17                pop ss
000015E2  B717              mov bh,0x17
000015E4  C217E0            ret 0xe017
000015E7  17                pop ss
000015E8  EB17              jmp short 0x1601
000015EA  F617              not byte [bx]
000015EC  07                pop es
000015ED  1812              sbb [bp+si],dl
000015EF  181D              sbb [di],bl
000015F1  1828              sbb [bx+si],ch
000015F3  1833              sbb [bp+di],dh
000015F5  183E184D          sbb [0x4d18],bh
000015F9  186018            sbb [bx+si+0x18],ah
000015FC  7318              jnc 0x1616
000015FE  8618              xchg bl,[bx+si]
00001600  96                xchg ax,si
00001601  18A618B1          sbb [bp-0x4ee8],ah
00001605  18BA18CF          sbb [bp+si-0x30e8],bh
00001609  18DA              sbb dl,bl
0000160B  18E5              sbb ch,ah
0000160D  18F0              sbb al,dh
0000160F  18FB              sbb bl,bh
00001611  180A              sbb [bp+si],cl
00001613  1919              sbb [bx+di],bx
00001615  1928              sbb [bx+si],bp
00001617  1937              sbb [bx],si
00001619  194919            sbb [bx+di+0x19],cx
0000161C  54                push sp
0000161D  195F19            sbb [bx+0x19],bx
00001620  6A19              push byte +0x19
00001622  7519              jnz 0x163d
00001624  80192D            sbb byte [bx+di],0x2d
00001627  267A16            es jpe 0x1640
0000162A  7A16              jpe 0x1642
0000162C  97                xchg ax,di
0000162D  19A819B9          sbb [bx+si-0x46e7],bp
00001631  19D8              sbb ax,bx
00001633  19E9              sbb cx,bp
00001635  199719FA          sbb [bx-0x5e7],dx
00001639  1919              sbb [bx+di],bx
0000163B  1A431A            sbb al,[bp+di+0x1a]
0000163E  381A              cmp [bp+si],bl
00001640  43                inc bx
00001641  1A621A            sbb ah,[bp+si+0x1a]
00001644  771A              ja 0x1660
00001646  8E1A              mov ds,[bp+si]
00001648  A6                cmpsb
00001649  1A8E1ABE          sbb cl,[bp-0x41e6]
0000164D  1AD2              sbb dl,dl
0000164F  1A6018            sbb ah,[bx+si+0x18]
00001652  FB                sti
00001653  1AE9              sbb ch,cl
00001655  1A4919            sbb cl,[bx+di+0x19]
00001658  54                push sp
00001659  197519            sbb [di+0x19],si
0000165C  3824              cmp [si],ah
0000165E  17                pop ss
0000165F  252625            and ax,0x2526
00001662  352509            xor ax,0x925
00001665  0AB300B3          or dh,[bp+di-0x4d00]
00001669  01B302B3          add [bp+di-0x4cfe],si
0000166D  03B304B3          add si,[bp+di-0x4cfc]
00001671  05B306            add ax,0x6b3
00001674  B307              mov bl,0x7
00001676  B308              mov bl,0x8
00001678  C3                ret
00001679  8800              mov [bx+si],al
0000167B  00880000          add [bx+si+0x0],cl
0000167F  AB                stosw
00001680  08830190          or [bp+di-0x6fff],al
00001684  06                push es
00001685  40                inc ax
00001686  16                push ss
00001687  8800              mov [bx+si],al
00001689  00AB0883          add [bp+di-0x7cf8],ch
0000168D  01900670          add [bx+si+0x7006],dx
00001691  08880000          or [bx+si+0x0],cl
00001695  AB                stosw
00001696  08830190          or [bp+di-0x6fff],al
0000169A  06                push es
0000169B  60                pusha
0000169C  085D08            or [di+0x8],bl
0000169F  51                push cx
000016A0  085314            or [bp+di+0x14],dl
000016A3  8800              mov [bx+si],al
000016A5  00AB0883          add [bp+di-0x7cf8],ch
000016A9  01900666          add [bx+si+0x6606],dx
000016AD  086008            or [bx+si+0x8],ah
000016B0  6308              arpl [bx+si],cx
000016B2  671488            adc al,0x88
000016B5  0000              add [bx+si],al
000016B7  AB                stosw
000016B8  08830190          or [bp+di-0x6fff],al
000016BC  06                push es
000016BD  60                pusha
000016BE  086108            or [bx+di+0x8],ah
000016C1  6208              bound cx,[bx+si]
000016C3  54                push sp
000016C4  1488              adc al,0x88
000016C6  0000              add [bx+si],al
000016C8  AB                stosw
000016C9  08830190          or [bp+di-0x6fff],al
000016CD  06                push es
000016CE  60                pusha
000016CF  085708            or [bx+0x8],dl
000016D2  60                pusha
000016D3  085914            or [bx+di+0x14],bl
000016D6  8800              mov [bx+si],al
000016D8  00AB0883          add [bp+di-0x7cf8],ch
000016DC  01900050          add [bx+si+0x5000],dx
000016E0  1460              adc al,0x60
000016E2  1456              adc al,0x56
000016E4  1488              adc al,0x88
000016E6  0000              add [bx+si],al
000016E8  AB                stosw
000016E9  00830190          add [bp+di-0x6fff],al
000016ED  08917F00          or [bx+di+0x7f],dl
000016F1  0460              add al,0x60
000016F3  32880000          xor cl,[bx+si+0x0]
000016F7  AB                stosw
000016F8  08830190          or [bp+di-0x6fff],al
000016FC  03400C            add ax,[bx+si+0xc]
000016FF  8800              mov [bx+si],al
00001701  00AB0883          add [bp+di-0x7cf8],ch
00001705  01900860          add [bx+si+0x6008],dx
00001709  0E                push cs
0000170A  58                pop ax
0000170B  0E                push cs
0000170C  641488            fs adc al,0x88
0000170F  0000              add [bx+si],al
00001711  AB                stosw
00001712  0483              add al,0x83
00001714  01900640          add [bx+si+0x4006],dx
00001718  1488              adc al,0x88
0000171A  0000              add [bx+si],al
0000171C  AB                stosw
0000171D  08830190          or [bp+di-0x6fff],al
00001721  08480E            or [bx+si+0xe],cl
00001724  52                push dx
00001725  0E                push cs
00001726  5A                pop dx
00001727  1488              adc al,0x88
00001729  0000              add [bx+si],al
0000172B  AB                stosw
0000172C  02830190          add al,[bp+di-0x6fff]
00001730  104808            adc [bx+si+0x8],cl
00001733  89064808          mov [0x848],ax
00001737  89064808          mov [0x848],ax
0000173B  89069105          mov [0x591],ax
0000173F  0001              add [bx+di],al
00001741  40                inc ax
00001742  40                inc ax
00001743  92                xchg ax,dx
00001744  50                push ax
00001745  2488              and al,0x88
00001747  0100              add [bx+si],ax
00001749  AB                stosw
0000174A  02830190          add al,[bp+di-0x6fff]
0000174E  06                push es
0000174F  58                pop ax
00001750  0A880200          or cl,[bx+si+0x2]
00001754  AB                stosw
00001755  02830190          add al,[bp+di-0x6fff]
00001759  024002            add al,[bx+si+0x2]
0000175C  8800              mov [bx+si],al
0000175E  00AB0883          add [bp+di-0x7cf8],ch
00001762  01900058          add [bx+si+0x5800],dx
00001766  0488              add al,0x88
00001768  0000              add [bx+si],al
0000176A  AB                stosw
0000176B  08830190          or [bp+di-0x6fff],al
0000176F  06                push es
00001770  54                push sp
00001771  08880004          or [bx+si+0x400],cl
00001775  AB                stosw
00001776  00900140          add [bx+si+0x4001],dl
0000177A  02901183          add dl,[bx+si-0x7cef]
0000177E  0013              add [bp+di],dl
00001780  0280FFB5          add al,[bx+si-0x4a01]
00001784  05FFA0            add ax,0xa0ff
00001787  01818317          add [bx+di+0x1783],ax
0000178B  8802              mov [bp+si],al
0000178D  00AB0090          add [bp+di-0x7000],ch
00001791  1E                push ds
00001792  0102              add [bp+si],ax
00001794  90                nop
00001795  2691              es xchg ax,cx
00001797  FF                db 0xff
00001798  FF                db 0xff
00001799  FF05              inc word [di]
0000179B  28880100          sub [bx+si+0x1],cl
0000179F  830190            add word [bx+di],byte -0x70
000017A2  BC1401            mov sp,0x114
000017A5  8802              mov [bp+si],al
000017A7  00AB0090          add [bp+di-0x7000],ch
000017AB  1E                push ds
000017AC  0102              add [bp+si],ax
000017AE  90                nop
000017AF  2691              es xchg ax,cx
000017B1  FF                db 0xff
000017B2  FF                db 0xff
000017B3  FD                std
000017B4  2020              and [bx+si],ah
000017B6  8800              mov [bx+si],al
000017B8  00AB0C83          add [bp+di-0x7cf4],ch
000017BC  01900260          add [bx+si+0x6002],dx
000017C0  02880000          add cl,[bx+si+0x0]
000017C4  AB                stosw
000017C5  0C83              or al,0x83
000017C7  01900740          add [bx+si+0x4007],dx
000017CB  0E                push cs
000017CC  42                inc dx
000017CD  0E                push cs
000017CE  44                inc sp
000017CF  0E                push cs
000017D0  47                inc di
000017D1  0E                push cs
000017D2  50                push ax
000017D3  0E                push cs
000017D4  54                push sp
000017D5  0E                push cs
000017D6  58                pop ax
000017D7  0E                push cs
000017D8  61                popa
000017D9  0E                push cs
000017DA  A00484            mov al,[0x8404]
000017DD  CA1788            retf 0x8817
000017E0  0100              add [bx+si],ax
000017E2  AB                stosw
000017E3  02830190          add al,[bp+di-0x6fff]
000017E7  06                push es
000017E8  48                dec ax
000017E9  1488              adc al,0x88
000017EB  0000              add [bx+si],al
000017ED  AB                stosw
000017EE  02830190          add al,[bp+di-0x6fff]
000017F2  06                push es
000017F3  40                inc ax
000017F4  1488              adc al,0x88
000017F6  0000              add [bx+si],al
000017F8  AB                stosw
000017F9  08830190          or [bp+di-0x6fff],al
000017FD  819101FFFF36      adc word [bx+di-0xff],0x36ff
00001803  1484              adc al,0x84
00001805  0218              add bl,[bx+si]
00001807  0100              add [bx+si],ax
00001809  AB                stosw
0000180A  02830190          add al,[bp+di-0x6fff]
0000180E  086410            or [si+0x10],ah
00001811  8801              mov [bx+di],al
00001813  00AB0283          add [bp+di-0x7cfe],ch
00001817  01900860          add [bx+si+0x6008],dx
0000181B  10880100          adc [bx+si+0x1],cl
0000181F  AB                stosw
00001820  02830190          add al,[bp+di-0x6fff]
00001824  085810            or [bx+si+0x10],bl
00001827  8801              mov [bx+di],al
00001829  00AB0283          add [bp+di-0x7cfe],ch
0000182D  01900854          add [bx+si+0x5408],dx
00001831  10880100          adc [bx+si+0x1],cl
00001835  AB                stosw
00001836  02830190          add al,[bp+di-0x6fff]
0000183A  084810            or [bx+si+0x10],cl
0000183D  8801              mov [bx+di],al
0000183F  00AB0283          add [bp+di-0x7cfe],ch
00001843  01900950          add [bx+si+0x5009],dx
00001847  0A520A            or dl,[bp+si+0xa]
0000184A  54                push sp
0000184B  0A880200          or cl,[bx+si+0x2]
0000184F  AB                stosw
00001850  02830190          add al,[bp+di-0x6fff]
00001854  025803            add bl,[bx+si+0x3]
00001857  A02055            mov al,[0x5520]
0000185A  03A02084          add sp,[bx+si-0x7be0]
0000185E  55                push bp
0000185F  1802              sbb [bp+si],al
00001861  00AB0283          add [bp+di-0x7cfe],ch
00001865  01900258          add [bx+si+0x5802],dx
00001869  03A01455          add sp,[bx+si+0x5514]
0000186D  03A01484          add sp,[bx+si-0x7bec]
00001871  681802            push word 0x218
00001874  00AB0283          add [bp+di-0x7cfe],ch
00001878  01900258          add [bx+si+0x5802],dx
0000187C  03A00E55          add sp,[bx+si+0x550e]
00001880  03A00E84          add sp,[bx+si-0x7bf2]
00001884  7B18              jpo 0x189e
00001886  0000              add [bx+si],al
00001888  AB                stosw
00001889  00830190          add [bp+di-0x6fff],al
0000188D  0491              add al,0x91
0000188F  7F00              jg 0x1891
00001891  0144FF            add [si-0x1],ax
00001894  92                xchg ax,dx
00001895  8800              mov [bx+si],al
00001897  00AB0883          add [bp+di-0x7cf8],ch
0000189B  01900691          add [bx+si-0x6efa],dx
0000189F  FF                db 0xff
000018A0  FF                db 0xff
000018A1  F8                clc
000018A2  60                pusha
000018A3  1492              adc al,0x92
000018A5  8800              mov [bx+si],al
000018A7  07                pop es
000018A8  9E                sahf
000018A9  0A907914          or dl,[bx+si+0x1479]
000018AD  10890488          adc [bx+di-0x77fc],cl
000018B1  0000              add [bx+si],al
000018B3  90                nop
000018B4  7A20              jpe 0x18d6
000018B6  08890488          or [bx+di-0x77fc],cl
000018BA  0000              add [bx+si],al
000018BC  AB                stosw
000018BD  08830190          or [bp+di-0x6fff],al
000018C1  06                push es
000018C2  58                pop ax
000018C3  105B10            adc [bp+di+0x10],bl
000018C6  6210              bound dx,[bx+si]
000018C8  7010              jo 0x18da
000018CA  7010              jo 0x18dc
000018CC  7010              jo 0x18de
000018CE  8800              mov [bx+si],al
000018D0  00AB0883          add [bp+di-0x7cf8],ch
000018D4  01900658          add [bx+si+0x5806],dx
000018D8  1488              adc al,0x88
000018DA  0000              add [bx+si],al
000018DC  AB                stosw
000018DD  08830190          or [bp+di-0x6fff],al
000018E1  06                push es
000018E2  50                push ax
000018E3  1488              adc al,0x88
000018E5  0000              add [bx+si],al
000018E7  AB                stosw
000018E8  08830190          or [bp+di-0x6fff],al
000018EC  086014            or [bx+si+0x14],ah
000018EF  8800              mov [bx+si],al
000018F1  00AB0883          add [bp+di-0x7cf8],ch
000018F5  01900858          add [bx+si+0x5808],dx
000018F9  1488              adc al,0x88
000018FB  0000              add [bx+si],al
000018FD  AB                stosw
000018FE  08830190          or [bp+di-0x6fff],al
00001902  0491              add al,0x91
00001904  7FFF              jg 0x1905
00001906  FE                db 0xfe
00001907  54                push sp
00001908  1488              adc al,0x88
0000190A  0000              add [bx+si],al
0000190C  AB                stosw
0000190D  08830190          or [bp+di-0x6fff],al
00001911  0491              add al,0x91
00001913  7F00              jg 0x1915
00001915  015014            add [bx+si+0x14],dx
00001918  8801              mov [bx+di],al
0000191A  00AB0883          add [bp+di-0x7cf8],ch
0000191E  01904B91          add [bx+si-0x6eb5],dx
00001922  40                inc ax
00001923  FF                db 0xff
00001924  FE                db 0xfe
00001925  54                push sp
00001926  50                push ax
00001927  8801              mov [bx+di],al
00001929  00AB0883          add [bp+di-0x7cf8],ch
0000192D  01904B91          add [bx+si-0x6eb5],dx
00001931  40                inc ax
00001932  0002              add [bp+si],al
00001934  4A                dec dx
00001935  50                push ax
00001936  8800              mov [bx+si],al
00001938  00AB0883          add [bp+di-0x7cf8],ch
0000193C  01900691          add [bx+si-0x6efa],dx
00001940  FF00              inc word [bx+si]
00001942  2F                das
00001943  58                pop ax
00001944  1492              adc al,0x92
00001946  6410880000        adc [fs:bx+si+0x0],cl
0000194B  AB                stosw
0000194C  08830190          or [bp+di-0x6fff],al
00001950  06                push es
00001951  58                pop ax
00001952  28880000          sub [bx+si+0x0],cl
00001956  AB                stosw
00001957  08830190          or [bp+di-0x6fff],al
0000195B  06                push es
0000195C  48                dec ax
0000195D  28880000          sub [bx+si+0x0],cl
00001961  AB                stosw
00001962  08830190          or [bp+di-0x6fff],al
00001966  085804            or [bx+si+0x4],bl
00001969  8800              mov [bx+si],al
0000196B  00AB0883          add [bp+di-0x7cf8],ch
0000196F  01900850          add [bx+si+0x5008],dx
00001973  0488              add al,0x88
00001975  0100              add [bx+si],ax
00001977  AB                stosw
00001978  08830190          or [bp+di-0x6fff],al
0000197C  0440              add al,0x40
0000197E  06                push es
0000197F  8802              mov [bp+si],al
00001981  00AB1683          add [bp+di-0x7cea],ch
00001985  01904591          add [bx+si-0x6ebb],dx
00001989  39FF              cmp di,di
0000198B  FF21              jmp [bx+di]
0000198D  8091390001        adc byte [bx+di+0x39],0x1
00001992  20808488          and [bx+si-0x777c],al
00001996  1900              sbb [bx+si],ax
00001998  00AB0890          add [bp+di-0x6ff8],ch
0000199C  0A10              or dl,[bx+si]
0000199E  08902691          or [bx+si-0x6eda],dl
000019A2  FF                db 0xff
000019A3  FF                db 0xff
000019A4  FF062888          inc word [0x8828]
000019A8  0000              add [bx+si],al
000019AA  AB                stosw
000019AB  08830190          or [bp+di-0x6fff],al
000019AF  0491              add al,0x91
000019B1  30FF              xor bh,bh
000019B3  FF                db 0xff
000019B4  38804008          cmp [bx+si+0x840],al
000019B8  8800              mov [bx+si],al
000019BA  0490              add al,0x90
000019BC  2183FF18          and [bp+di+0x18ff],ax
000019C0  02800A91          add al,[bx+si-0x6ef6]
000019C4  FF                db 0xff
000019C5  FFCF              dec di
000019C7  A00391            mov al,[0x9103]
000019CA  FF00              inc word [bx+si]
000019CC  44                inc sp
000019CD  A00281            mov al,[0x8102]
000019D0  C3                ret
000019D1  19890192          sbb [bx+di-0x6dff],cx
000019D5  A04088            mov al,[0x8840]
000019D8  0200              add al,[bx+si]
000019DA  AB                stosw
000019DB  08830190          or [bp+di-0x6fff],al
000019DF  19919000          sbb [bx+di+0x90],dx
000019E3  015020            add [bx+si+0x20],dx
000019E6  84E4              test ah,ah
000019E8  1900              sbb [bx+si],ax
000019EA  00AB0890          add [bp+di-0x6ff8],ch
000019EE  0A160890          or dl,[0x9008]
000019F2  2691              es xchg ax,cx
000019F4  FF                db 0xff
000019F5  FF                db 0xff
000019F6  FF162888          call [0x8828]
000019FA  0004              add [si],al
000019FC  90                nop
000019FD  17                pop ss
000019FE  83FF30            cmp di,byte +0x30
00001A01  02800A91          add al,[bx+si-0x6ef6]
00001A05  FF                db 0xff
00001A06  FFCF              dec di
00001A08  A00391            mov al,[0x9103]
00001A0B  FF00              inc word [bx+si]
00001A0D  54                push sp
00001A0E  A00281            mov al,[0x8102]
00001A11  041A              add al,0x1a
00001A13  8901              mov [bx+di],ax
00001A15  92                xchg ax,dx
00001A16  A03088            mov al,[0x8830]
00001A19  0004              add [si],al
00001A1B  90                nop
00001A1C  17                pop ss
00001A1D  83FF26            cmp di,byte +0x26
00001A20  02800A91          add al,[bx+si-0x6ef6]
00001A24  FF                db 0xff
00001A25  FFCF              dec di
00001A27  A00391            mov al,[0x9103]
00001A2A  FF00              inc word [bx+si]
00001A2C  54                push sp
00001A2D  A00281            mov al,[0x8102]
00001A30  231A              and bx,[bp+si]
00001A32  8901              mov [bx+di],ax
00001A34  92                xchg ax,dx
00001A35  A03088            mov al,[0x8830]
00001A38  0407              add al,0x7
00001A3A  9E                sahf
00001A3B  01907910          add [bx+si+0x1079],dx
00001A3F  20890488          and [bx+di-0x77fc],cl
00001A43  0004              add [si],al
00001A45  90                nop
00001A46  17                pop ss
00001A47  83FF16            cmp di,byte +0x16
00001A4A  02800A91          add al,[bx+si-0x6ef6]
00001A4E  FF                db 0xff
00001A4F  FFCF              dec di
00001A51  A00391            mov al,[0x9103]
00001A54  FF00              inc word [bx+si]
00001A56  54                push sp
00001A57  A00281            mov al,[0x8102]
00001A5A  4D                dec bp
00001A5B  1A890192          sbb cl,[bx+di-0x6dff]
00001A5F  A03088            mov al,[0x8830]
00001A62  0000              add [bx+si],al
00001A64  AB                stosw
00001A65  08830190          or [bp+di-0x6fff],al
00001A69  1E                push ds
00001A6A  0102              add [bp+si],ax
00001A6C  90                nop
00001A6D  2A91FFFF          sub dl,[bx+di-0x1]
00001A71  FF1A              call far [bp+si]
00001A73  20890288          and [bx+di-0x77fe],cl
00001A77  0100              add [bx+si],ax
00001A79  AB                stosw
00001A7A  0C83              or al,0x83
00001A7C  01907991          add [bx+si-0x6e87],dx
00001A80  01FF              add di,di
00001A82  FF20              jmp [bx+si]
00001A84  60                pusha
00001A85  91                xchg ax,cx
00001A86  0100              add [bx+si],ax
00001A88  0124              add [si],sp
00001A8A  60                pusha
00001A8B  847F1A            test [bx+0x1a],bh
00001A8E  0000              add [bx+si],al
00001A90  AB                stosw
00001A91  08830190          or [bp+di-0x6fff],al
00001A95  2A9108FF          sub dl,[bx+di-0xf8]
00001A99  FF05              inc word [di]
00001A9B  359108            xor ax,0x891
00001A9E  0001              add [bx+di],al
00001AA0  0035              add [di],dh
00001AA2  81961A880000      adc word [bp-0x77e6],0x0
00001AA8  AB                stosw
00001AA9  08830190          or [bp+di-0x6fff],al
00001AAD  199170FF          sbb [bx+di-0x90],dx
00001AB1  FF6550            jmp [di+0x50]
00001AB4  91                xchg ax,cx
00001AB5  7000              jo 0x1ab7
00001AB7  015550            add [di+0x50],dx
00001ABA  81AE1A880100      sub word [bp-0x77e6],0x1
00001AC0  830190            add word [bx+di],byte -0x70
00001AC3  BE95FF            mov si,0xff95
00001AC6  06                push es
00001AC7  0300              add ax,[bx+si]
00001AC9  243A              and al,0x3a
00001ACB  91                xchg ax,cx
00001ACC  FF                db 0xff
00001ACD  FF                db 0xff
00001ACE  F9                stc
00001ACF  A02588            mov al,[0x8825]
00001AD2  0000              add [bx+si],al
00001AD4  AB                stosw
00001AD5  08830190          or [bp+di-0x6fff],al
00001AD9  1E                push ds
00001ADA  0102              add [bp+si],ax
00001ADC  90                nop
00001ADD  3691              ss xchg ax,cx
00001ADF  FF                db 0xff
00001AE0  FFD7              call di
00001AE2  7830              js 0x1b14
00001AE4  AB                stosw
00001AE5  FF900088          call [bx+si-0x7800]
00001AE9  0000              add [bx+si],al
00001AEB  AB                stosw
00001AEC  08830190          or [bp+di-0x6fff],al
00001AF0  06                push es
00001AF1  91                xchg ax,cx
00001AF2  50                push ax
00001AF3  FF                db 0xff
00001AF4  FF6040            jmp [bx+si+0x40]
00001AF7  92                xchg ax,dx
00001AF8  40                inc ax
00001AF9  29880000          sub [bx+si+0x0],cx
00001AFD  AB                stosw
00001AFE  08830190          or [bp+di-0x6fff],al
00001B02  06                push es
00001B03  91                xchg ax,cx
00001B04  50                push ax
00001B05  0030              add [bx+si],dh
00001B07  58                pop ax
00001B08  40                inc ax
00001B09  92                xchg ax,dx
00001B0A  6318              arpl [bx+si],bx
00001B0C  889D1CC9          mov [di-0x36e4],bl
00001B10  1CD4              sbb al,0xd4
00001B12  1CDF              sbb al,0xdf
00001B14  1CEA              sbb al,0xea
00001B16  1C00              sbb al,0x0
00001B18  1DF51C            sbb ax,0x1cf5
00001B1B  2C1D              sub al,0x1d
00001B1D  16                push ss
00001B1E  1D211D            sbb ax,0x1d21
00001B21  0B1D              or bx,[di]
00001B23  37                aaa
00001B24  1D421D            sbb ax,0x1d42
00001B27  4D                dec bp
00001B28  1D581D            sbb ax,0x1d58
00001B2B  631D              arpl [di],bx
00001B2D  6E                outsb
00001B2E  1D791D            sbb ax,0x1d79
00001B31  841D              test [di],bl
00001B33  8F                db 0x8f
00001B34  1D9A1D            sbb ax,0x1d9a
00001B37  B01D              mov al,0x1d
00001B39  A5                movsw
00001B3A  1DDC1D            sbb ax,0x1ddc
00001B3D  E71D              out 0x1d,ax
00001B3F  F21DFD1D          repne sbb ax,0x1dfd
00001B43  081E131E          or [0x1e13],bl
00001B47  1E                push ds
00001B48  1E                push ds
00001B49  291EBB1D          sub [0x1dbb],bx
00001B4D  C6                db 0xc6
00001B4E  1DD11D            sbb ax,0x1dd1
00001B51  341E              xor al,0x1e
00001B53  3F                aas
00001B54  1E                push ds
00001B55  4A                dec dx
00001B56  1E                push ds
00001B57  55                push bp
00001B58  1E                push ds
00001B59  60                pusha
00001B5A  1E                push ds
00001B5B  6B1E761E81        imul bx,[0x1e76],byte -0x7f
00001B60  1E                push ds
00001B61  8C1E971E          mov [0x1e97],ds
00001B65  C01FAD            rcr byte [bx],byte 0xad
00001B68  1E                push ds
00001B69  B81EC3            mov ax,0xc31e
00001B6C  1E                push ds
00001B6D  CE                into
00001B6E  1E                push ds
00001B6F  D91EE41E          fstp dword [0x1ee4]
00001B73  FA                cli
00001B74  1E                push ds
00001B75  101F              adc [bx],bl
00001B77  261F              es pop ds
00001B79  311F              xor [bx],bx
00001B7B  3C1F              cmp al,0x1f
00001B7D  681F73            push word 0x731f
00001B80  1F                pop ds
00001B81  7E1F              jng 0x1ba2
00001B83  52                push dx
00001B84  1F                pop ds
00001B85  5D                pop bp
00001B86  1F                pop ds
00001B87  B51F              mov ch,0x1f
00001B89  AA                stosb
00001B8A  1F                pop ds
00001B8B  CB                retf
00001B8C  1F                pop ds
00001B8D  4F                dec di
00001B8E  205A20            and [bp+si+0x20],bl
00001B91  7020              jo 0x1bb3
00001B93  65207B20          and [gs:bp+di+0x20],bh
00001B97  8620              xchg ah,[bx+si]
00001B99  2E2039            and [cs:bx+di],bh
00001B9C  204420            and [si+0x20],al
00001B9F  57                push di
00001BA0  216221            and [bp+si+0x21],sp
00001BA3  6D                insw
00001BA4  2191209C          and [bx+di-0x63e0],dx
00001BA8  20A720B2          and [bx-0x4de0],ah
00001BAC  20BD20C8          and [di-0x37e0],bh
00001BB0  20DE              and dh,bl
00001BB2  20E9              and cl,ch
00001BB4  20F4              and ah,dh
00001BB6  20FF              and bh,bh
00001BB8  200A              and [bp+si],cl
00001BBA  2115              and [di],dx
00001BBC  2120              and [bx+si],sp
00001BBE  212B              and [bp+di],bp
00001BC0  2129              and [bx+di],bp
00001BC2  1E                push ds
00001BC3  36214121          and [ss:bx+di+0x21],ax
00001BC7  4C                dec sp
00001BC8  21CA              and dx,cx
00001BCA  23D5              and dx,bp
00001BCC  23BF23B4          and di,[bx-0x4bdd]
00001BD0  239E23A9          and bx,[bp-0x56dd]
00001BD4  23882393          and cx,[bx+si-0x6cdd]
00001BD8  2329              and bp,[bx+di]
00001BDA  1E                push ds
00001BDB  291E291E          sub [0x1e29],bx
00001BDF  291E291E          sub [0x1e29],bx
00001BE3  291E291E          sub [0x1e29],bx
00001BE7  291E291E          sub [0x1e29],bx
00001BEB  291E291E          sub [0x1e29],bx
00001BEF  291E291E          sub [0x1e29],bx
00001BF3  291E291E          sub [0x1e29],bx
00001BF7  291EA21E          sub [0x1ea2],bx
00001BFB  D320              shl word [bx+si],cl
00001BFD  42                inc dx
00001BFE  1D9921            sbb ax,0x2199
00001C01  832178            and word [bx+di],byte +0x78
00001C04  213B              and [bp+di],di
00001C06  2330              and si,[bx+si]
00001C08  235123            and dx,[bx+di+0x23]
00001C0B  5C                pop sp
00001C0C  2329              and bp,[bx+di]
00001C0E  1E                push ds
00001C0F  67237223          and si,[edx+0x23]
00001C13  7D23              jnl 0x1c38
00001C15  46                inc si
00001C16  2329              and bp,[bx+di]
00001C18  1E                push ds
00001C19  291E291E          sub [0x1e29],bx
00001C1D  291E291E          sub [0x1e29],bx
00001C21  291E291E          sub [0x1e29],bx
00001C25  291E291E          sub [0x1e29],bx
00001C29  291E291E          sub [0x1e29],bx
00001C2D  0220              add ah,[bx+si]
00001C2F  2320              and sp,[bx+si]
00001C31  F71F              neg word [bx]
00001C33  0D2018            or ax,0x1820
00001C36  20EC              and ah,ch
00001C38  1F                pop ds
00001C39  E11F              loope 0x1c5a
00001C3B  291E291E          sub [0x1e29],bx
00001C3F  291E291E          sub [0x1e29],bx
00001C43  291E291E          sub [0x1e29],bx
00001C47  291E291E          sub [0x1e29],bx
00001C4B  291E291E          sub [0x1e29],bx
00001C4F  291E291E          sub [0x1e29],bx
00001C53  291E3E22          sub [0x223e],bx
00001C57  5A                pop dx
00001C58  207020            and [bx+si+0x20],dh
00001C5B  252375            and ax,0x7523
00001C5E  2286201D          and al,[bp+0x1d20]
00001C62  2228              and ch,[bx+si]
00001C64  2233              and dh,[bp+di]
00001C66  22D0              and dl,al
00001C68  21DB              and bx,bx
00001C6A  21DB              and bx,bx
00001C6C  218B2296          and [bp+di-0x69de],cx
00001C70  22A122AC          and ah,[bx+di-0x53de]
00001C74  22B7225F          and dh,[bx+0x5f22]
00001C78  22BA21C5          and bh,[bp+si-0x3adf]
00001C7C  21D8              and ax,bx
00001C7E  22FF              and bh,bh
00001C80  200A              and [bp+si],cl
00001C82  21F9              and cx,di
00001C84  2204              and al,[si]
00001C86  230F              and cx,[bx]
00001C88  234922            and cx,[bx+di+0x22]
00001C8B  1A23              sbb ah,[bp+di]
00001C8D  F1                int1
00001C8E  2112              and [bp+si],dx
00001C90  22AF21A4          and ch,[bx-0x5bdf]
00001C94  214826            and [bx+si+0x26],cx
00001C97  53                push bx
00001C98  265E              es pop si
00001C9A  26692601010801    imul sp,[es:0x101],word 0x108
00001CA1  0018              add [bx+si],bl
00001CA3  0054F2            add [si-0xe],dl
00001CA6  60                pusha
00001CA7  46                inc si
00001CA8  0000              add [bx+si],al
00001CAA  0000              add [bx+si],al
00001CAC  003F              add [bx],bh
00001CAE  3F                aas
00001CAF  0000              add [bx+si],al
00001CB1  0000              add [bx+si],al
00001CB3  0000              add [bx+si],al
00001CB5  0000              add [bx+si],al
00001CB7  0000              add [bx+si],al
00001CB9  00F1              add cl,dh
00001CBB  F1                int1
00001CBC  0000              add [bx+si],al
00001CBE  2321              and sp,[bx+di]
00001CC0  0200              add al,[bx+si]
00001CC2  0030              add [bx+si],dh
00001CC4  005FF0            add [bx-0x10],bl
00001CC7  0303              add ax,[bp+di]
00001CC9  2C21              sub al,0x21
00001CCB  06                push es
00001CCC  0200              add al,[bx+si]
00001CCE  16                push ss
00001CCF  00FE              add dh,bh
00001CD1  F6FF              idiv bh
00001CD3  FA                cli
00001CD4  3031              xor [bx+di],dh
00001CD6  0F0000            sldt [bx+si]
00001CD9  0800              or [bx+si],al
00001CDB  A5                movsw
00001CDC  F6                db 0xf6
00001CDD  88FF              mov bh,bh
00001CDF  3021              xor [bx+di],ah
00001CE1  06                push es
00001CE2  0200              add al,[bx+si]
00001CE4  52                push dx
00001CE5  00F5              add ch,dh
00001CE7  F2844421          repne test [si+0x21],al
00001CEB  2100              and [bx+si],ax
00001CED  0101              add [bx+di],ax
00001CEF  1400              adc al,0x0
00001CF1  A6                cmpsb
00001CF2  C25625            ret 0x2556
00001CF5  0200              add al,[bx+si]
00001CF7  0000              add [bx+si],al
00001CF9  002600F5          add [0xf500],ah
00001CFD  F27746            bnd ja 0x1d46
00001D00  37                aaa
00001D01  3E0E              ds push cs
00001D03  0201              add al,[bx+di]
00001D05  0A00              or al,[bx+si]
00001D07  F5                cmc
00001D08  F7                db 0xf7
00001D09  CF                iret
00001D0A  AF                scasw
00001D0B  3231              xor dh,[bx+di]
00001D0D  0202              add al,[bp+si]
00001D0F  000A              add [bp+si],cl
00001D11  00F8              add al,bh
00001D13  F4                hlt
00001D14  98                cbw
00001D15  46                inc si
00001D16  2121              and [bx+di],sp
00001D18  0001              add [bx+di],al
00001D1A  0008              add [bx+si],cl
00001D1C  0086F456          add [bp+0x56f4],al
00001D20  27                daa
00001D21  2121              and [bx+di],sp
00001D23  0001              add [bx+di],al
00001D25  010E0066          add [0x6600],cx
00001D29  D456              aam 0x56
00001D2B  2834              sub [si],dh
00001D2D  3102              xor [bp+si],ax
00001D2F  0100              add [bx+si],ax
00001D31  1000              adc [bx+si],al
00001D33  F8                clc
00001D34  F4                hlt
00001D35  88882021          mov [bx+si+0x2120],cl
00001D39  0003              add [bp+di],al
00001D3B  0110              add [bx+si],dx
00001D3D  0096C258          add [bp+0x58c2],dl
00001D41  250100            and ax,0x1
00001D44  0C00              or al,0x0
00001D46  0000              add [bx+si],al
00001D48  00F0              add al,dh
00001D4A  E312              jcxz 0x1d5e
00001D4C  150002            adc ax,0x200
00001D4F  0A01              or al,[bx+di]
00001D51  0000              add [bx+si],al
00001D53  004484            add [si-0x7c],al
00001D56  14F6              adc al,0xf6
00001D58  06                push es
00001D59  07                pop es
00001D5A  07                pop es
00001D5B  0303              add ax,[bp+di]
00001D5D  0000              add [bx+si],al
00001D5F  A090F9            mov al,[0xf990]
00001D62  F8                clc
00001D63  2222              and ah,[bp+si]
00001D65  0902              or [bp+si],ax
00001D67  0200              add al,[bx+si]
00001D69  00A1C307          add [bx+di+0x7c3],ah
00001D6D  07                pop es
00001D6E  1121              adc [bx+di],sp
00001D70  0800              or [bx+si],al
00001D72  000D              add [di],cl
00001D74  00D2              add dl,dl
00001D76  F1                int1
00001D77  0307              add ax,[bx]
00001D79  3121              xor [bx+di],sp
00001D7B  0C00              or al,0x0
00001D7D  004C00            add [si+0x0],cl
00001D80  A1A1F3            mov ax,[0xf3a1]
00001D83  150000            adc ax,0x0
00001D86  06                push es
00001D87  0000              add [bx+si],al
00001D89  0100              add [bx+si],ax
00001D8B  C9                leave
00001D8C  C474F7            les si,[si-0x9]
00001D8F  0E                push cs
00001D90  010E0200          add [0x2],cx
00001D94  0000              add [bx+si],al
00001D96  9F                lahf
00001D97  F22317            repne and dx,[bx]
00001D9A  0A08              or cl,[bx+si]
00001D9C  0800              or [bx+si],al
00001D9E  0200              add al,[bx+si]
00001DA0  0093A403          add [bp+di+0x3a4],dl
00001DA4  F5                cmc
00001DA5  0A21              or ah,[bx+di]
00001DA7  0800              or [bx+si],al
00001DA9  0300              add ax,[bx+si]
00001DAB  0093A903          add [bp+di+0x3a9],dl
00001DAF  050A08            add ax,0x80a
00001DB2  0800              or [bx+si],al
00001DB4  0200              add al,[bx+si]
00001DB6  0093A403          add [bp+di+0x3a4],dl
00001DBA  F9                stc
00001DBB  E220              loop 0x1ddd
00001DBD  0E                push cs
00001DBE  0301              add ax,[bx+di]
00001DC0  1000              adc [bx+si],al
00001DC2  89F0              mov ax,si
00001DC4  44                inc sp
00001DC5  6644              inc esp
00001DC7  310E0201          xor [0x102],cx
00001DCB  5B                pop bx
00001DCC  00E5              add ch,ah
00001DCE  7441              jz 0x1e11
00001DD0  FB                sti
00001DD1  6311              arpl [bx+di],dx
00001DD3  0000              add [bx+si],al
00001DD5  0118              add [bx+si],bx
00001DD7  0075F3            add [di-0xd],dh
00001DDA  7325              jnc 0x1e01
00001DDC  6311              arpl [bx+di],dx
00001DDE  0000              add [bx+si],al
00001DE0  01160065          add [0x6500],dx
00001DE4  F37524            rep jnz 0x1e0b
00001DE7  43                inc bx
00001DE8  11060101          adc [0x101],ax
00001DEC  0C00              or al,0x0
00001DEE  55                push bp
00001DEF  F355              rep push bp
00001DF1  2445              and al,0x45
00001DF3  1307              adc ax,[bx]
00001DF5  0001              add [bx+di],al
00001DF7  0000              add [bx+si],al
00001DF9  1414              adc al,0x14
00001DFB  55                push bp
00001DFC  FFD2              call dx
00001DFE  3401              xor al,0x1
00001E00  0000              add [bx+si],al
00001E02  010A              add [bp+si],cx
00001E04  43                inc bx
00001E05  4A                dec dx
00001E06  93                xchg ax,bx
00001E07  A5                movsw
00001E08  F2320C            repne xor cl,[si]
00001E0B  0003              add [bp+di],al
00001E0D  1A0A              sbb cl,[bp+si]
00001E0F  80F000            xor al,0x0
00001E12  08D1              or cl,dl
00001E14  320E0302          xor cl,[0x203]
00001E18  1B0A              sbb cx,[bp+si]
00001E1A  66F0FF08          lock dec dword [bx+si]
00001E1E  C1                db 0xc1
00001E1F  330E0302          xor cx,[0x203]
00001E23  190A              sbb [bp+si],cx
00001E25  3AF0              cmp dh,al
00001E27  FF08              dec word [bx+si]
00001E29  1211              adc dl,[bx+di]
00001E2B  06                push es
00001E2C  0100              add [bx+si],ax
00001E2E  FF                db 0xff
00001E2F  FF                db 0xff
00001E30  FF                db 0xff
00001E31  FF                db 0xff
00001E32  FF                db 0xff
00001E33  FF0A              dec word [bp+si]
00001E35  010C              add [si],cx
00001E37  0100              add [bx+si],ax
00001E39  0400              add al,0x0
00001E3B  82                db 0x82
00001E3C  A3F316            mov [0x16f3],ax
00001E3F  0A0C              or cl,[si]
00001E41  0802              or [bp+si],al
00001E43  0100              add [bx+si],ax
00001E45  0021              add [bx+di],ah
00001E47  6204              bound ax,[si]
00001E49  F60307            test byte [bp+di],0x7
00001E4C  06                push es
00001E4D  0303              add ax,[bp+di]
00001E4F  0000              add [bx+si],al
00001E51  315202            xor [bp+si+0x2],dx
00001E54  F34E              rep dec si
00001E56  40                inc ax
00001E57  0802              or [bp+si],al
00001E59  0304              add ax,[si]
00001E5B  0021              add [bx+di],ah
00001E5D  31F2              xor dx,si
00001E5F  0383420E          add ax,[bp+di+0xe42]
00001E63  0000              add [bx+si],al
00001E65  0000              add [bx+si],al
00001E67  41                inc cx
00001E68  A4                movsb
00001E69  01F4              add sp,si
00001E6B  4E                dec si
00001E6C  42                inc dx
00001E6D  06                push es
00001E6E  0300              add ax,[bx+si]
00001E70  0000              add [bx+si],al
00001E72  7282              jc 0x1df6
00001E74  FA                cli
00001E75  FB                sti
00001E76  42                inc dx
00001E77  40                inc ax
00001E78  0803              or [bp+di],al
00001E7A  0000              add [bx+si],al
00001E7C  0021              add [bx+di],ah
00001E7E  A0F2F3            mov al,[0xf3f2]
00001E81  80010C            add byte [bx+di],0xc
00001E84  0100              add [bx+si],ax
00001E86  0000              add [bx+si],al
00001E88  B090              mov al,0x90
00001E8A  F2F4              repne hlt
00001E8C  6C                insb
00001E8D  61                popa
00001E8E  0E                push cs
00001E8F  0000              add [bx+si],al
00001E91  0000              add [bx+si],al
00001E93  72F2              jc 0x1e87
00001E95  F1                int1
00001E96  F5                cmc
00001E97  62610E            bound sp,[bx+di+0xe]
00001E9A  0000              add [bx+si],al
00001E9C  1000              adc [bx+si],al
00001E9E  B2B3              mov dl,0xb3
00001EA0  F6F7              div bh
00001EA2  222E0800          and ch,[0x8]
00001EA6  030600A1          add ax,[0xa100]
00001EAA  C1                db 0xc1
00001EAB  F2F5              repne cmc
00001EAD  2F                das
00001EAE  2C0E              sub al,0xe
00001EB0  0000              add [bx+si],al
00001EB2  0000              add [bx+si],al
00001EB4  3122              xor [bp+si],sp
00001EB6  F0F36E            rep lock outsb
00001EB9  2008              and [bx+si],cl
00001EBB  0000              add [bx+si],al
00001EBD  0000              add [bx+si],al
00001EBF  6290FF0F          bound dx,[bx+si+0xfff]
00001EC3  202F              and [bx],ch
00001EC5  0000              add [bx+si],al
00001EC7  0007              add [bx],al
00001EC9  00C3              add bl,al
00001ECB  C3                ret
00001ECC  F5                cmc
00001ECD  F72E2D0E          imul word [0xe2d]
00001ED1  0200              add al,[bx+si]
00001ED3  0000              add [bx+si],al
00001ED5  50                push ax
00001ED6  3302              xor ax,[bp+si]
00001ED8  F32E2D0A02        cs rep sub ax,0x20a
00001EDD  0000              add [bx+si],al
00001EDF  00F0              add al,dh
00001EE1  F4                hlt
00001EE2  00F6              add dh,dh
00001EE4  232C              and bp,[si]
00001EE6  0A02              or al,[bp+si]
00001EE8  0000              add [bx+si],al
00001EEA  0080A400          add [bx+si+0xa4],al
00001EEE  F62E200A          imul byte [0xa20]
00001EF2  0200              add al,[bx+si]
00001EF4  0000              add [bx+si],al
00001EF6  F0D000            lock rol byte [bx+si],1
00001EF9  F4                hlt
00001EFA  2420              and al,0x20
00001EFC  0A00              or al,[bx+si]
00001EFE  0000              add [bx+si],al
00001F00  00F0              add al,dh
00001F02  D000              rol byte [bx+si],1
00001F04  F4                hlt
00001F05  2F                das
00001F06  2E0100            add [cs:bx+si],ax
00001F09  0000              add [bx+si],al
00001F0B  006677            add [bp+0x77],ah
00001F0E  F3232B            rep and bp,[bp+di]
00001F11  6A09              push byte +0x9
00001F13  0000              add [bx+si],al
00001F15  0000              add [bx+si],al
00001F17  58                pop ax
00001F18  58                pop ax
00001F19  F5                cmc
00001F1A  F4                hlt
00001F1B  2020              and [bx+si],ah
00001F1D  0A02              or al,[bp+si]
00001F1F  0000              add [bx+si],al
00001F21  007052            add [bx+si+0x52],dh
00001F24  0FFF              ud0
00001F26  2025              and [di],ah
00001F28  0A02              or al,[bp+si]
00001F2A  0000              add [bx+si],al
00001F2C  007052            add [bx+si+0x52],dh
00001F2F  0FFF              ud0
00001F31  234108            and ax,[bx+di+0x8]
00001F34  0003              add [bp+di],al
00001F36  0000              add [bx+si],al
00001F38  2131              and [bx+di],si
00001F3A  F2F22E0108        repne add [cs:bx+si],cx
00001F3F  0000              add [bx+si],al
00001F41  0000              add [bx+si],al
00001F43  2131              and [bx+di],si
00001F45  F1                int1
00001F46  F2250200          repne and ax,0x2
00001F4A  0000              add [bx+si],al
00001F4C  0000              add [bx+si],al
00001F4E  61                popa
00001F4F  33F1              xor si,cx
00001F51  F5                cmc
00001F52  2E0108            add [cs:bx+si],cx
00001F55  0000              add [bx+si],al
00001F57  0000              add [bx+si],al
00001F59  2131              and [bx+di],si
00001F5B  F1                int1
00001F5C  F26E              repne outsb
00001F5E  0008              add [bx+si],cl
00001F60  0000              add [bx+si],al
00001F62  0000              add [bx+si],al
00001F64  2131              and [bx+di],si
00001F66  F1                int1
00001F67  F20F0000          repne sldt [bx+si]
00001F6B  0000              add [bx+si],al
00001F6D  0000              add [bx+si],al
00001F6F  3170F1            xor [bx+si-0xf],si
00001F72  F5                cmc
00001F73  27                daa
00001F74  2000              and [bx+si],al
00001F76  0000              add [bx+si],al
00001F78  0000              add [bx+si],al
00001F7A  F1                int1
00001F7B  B601              mov dh,0x1
00001F7D  F3A14C08          rep mov ax,[0x84c]
00001F81  0002              add [bp+si],al
00001F83  0000              add [bx+si],al
00001F85  2281F5F4          and al,[bx+di-0xb0b]
00001F89  8F                db 0x8f
00001F8A  4F                dec di
00001F8B  0800              or [bx+si],al
00001F8D  0300              add ax,[bx+si]
00001F8F  0032              add [bp+si],dh
00001F91  70F3              jo 0x1f86
00001F93  F4                hlt
00001F94  2007              and [bx],al
00001F96  0000              add [bx+si],al
00001F98  0000              add [bx+si],al
00001F9A  0023              add [bp+di],ah
00001F9C  33F1              xor si,cx
00001F9E  F307              rep pop es
00001FA0  0109              add [bx+di],cx
00001FA2  0001              add [bx+di],al
00001FA4  0000              add [bx+si],al
00001FA6  6575F5            gs jnz 0x1f9e
00001FA9  F5                cmc
00001FAA  44                inc sp
00001FAB  0F                db 0x0f
00001FAC  0A00              or al,[bx+si]
00001FAE  0000              add [bx+si],al
00001FB0  006073            add [bx+si+0x73],ah
00001FB3  F2F78300080003    repne test word [bp+di+0x800],0x300
00001FBA  0000              add [bx+si],al
00001FBC  41                inc cx
00001FBD  91                xchg ax,cx
00001FBE  F2F343            rep inc bx
00001FC1  0008              add [bx+si],cl
00001FC3  0000              add [bx+si],al
00001FC5  0000              add [bx+si],al
00001FC7  91                xchg ax,cx
00001FC8  91                xchg ax,cx
00001FC9  F2F36320          rep arpl [bx+si],sp
00001FCD  0000              add [bx+si],al
00001FCF  0000              add [bx+si],al
00001FD1  0020              add [bx+si],ah
00001FD3  30F2              xor dl,dh
00001FD5  F342              rep inc dx
00001FD7  2309              and cx,[bx+di]
00001FD9  0003              add [bp+di],al
00001FDB  0000              add [bx+si],al
00001FDD  2030              and [bx+si],dh
00001FDF  F9                stc
00001FE0  FA                cli
00001FE1  3231              xor dh,[bx+di]
00001FE3  0801              or [bx+di],al
00001FE5  008C00F7          add [si-0x900],cl
00001FE9  F4                hlt
00001FEA  96                xchg ax,si
00001FEB  49                dec cx
00001FEC  F1                int1
00001FED  320A              xor cl,[bp+si]
00001FEF  0003              add [bp+di],al
00001FF1  180680F0          sbb [0xf080],al
00001FF5  0001              add [bx+di],al
00001FF7  0000              add [bx+si],al
00001FF9  0800              or [bx+si],al
00001FFB  000600F7          add [0xf700],al
00001FFF  F6FF              idiv bh
00002001  FF5202            call [bp+si+0x2]
00002004  0000              add [bx+si],al
00002006  0000              add [bx+si],al
00002008  10C8              adc al,cl
0000200A  F6                db 0xf6
0000200B  8F                db 0x8f
0000200C  BF0110            mov di,0x1001
0000200F  0100              add [bx+si],ax
00002011  0204              add al,[si]
00002013  00F5              add ch,dh
00002015  95                xchg ax,bp
00002016  5F                pop di
00002017  27                daa
00002018  0110              add [bx+si],dx
0000201A  0100              add [bx+si],ax
0000201C  020604F8          add al,[0xf804]
00002020  A6                cmpsb
00002021  FF37              push word [bx]
00002023  0110              add [bx+si],dx
00002025  0100              add [bx+si],ax
00002027  020600F8          add al,[0xf800]
0000202B  96                xchg ax,si
0000202C  FF27              jmp [bx]
0000202E  0301              add ax,[bx+di]
00002030  0E                push cs
00002031  0000              add [bx+si],al
00002033  0000              add [bx+si],al
00002035  F1                int1
00002036  1000              adc [bx+si],al
00002038  0403              add al,0x3
0000203A  010E0000          add [0x0],cx
0000203E  0000              add [bx+si],al
00002040  F1                int1
00002041  7000              jo 0x2043
00002043  04C3              add al,0xc3
00002045  2100              and [bx+si],ax
00002047  0103              add [bp+di],ax
00002049  050060            add ax,0x6000
0000204C  1F                pop ds
0000204D  030A              add cx,[bp+si]
0000204F  707E              jo 0x20cf
00002051  0000              add [bx+si],al
00002053  0008              add [bx+si],cl
00002055  00F5              add ch,dh
00002057  57                push di
00002058  7F7F              jg 0x20d9
0000205A  0A08              or cl,[bx+si]
0000205C  0800              or [bx+si],al
0000205E  0200              add al,[bx+si]
00002060  0093A403          add [bp+di+0x3a4],dl
00002064  F647080E          test byte [bx+0x8],0xe
00002068  0301              add ax,[bx+di]
0000206A  04D3              add al,0xd3
0000206C  98                cbw
0000206D  E9AE00            jmp 0x211e
00002070  0A08              or cl,[bx+si]
00002072  0800              or [bx+si],al
00002074  0200              add al,[bx+si]
00002076  0093A403          add [bp+di+0x3a4],dl
0000207A  F60400            test byte [si],0x0
0000207D  0800              or [bx+si],al
0000207F  0200              add al,[bx+si]
00002081  0093A403          add [bp+di+0x3a4],dl
00002085  F60101            test byte [bx+di],0x1
00002088  0000              add [bx+si],al
0000208A  0000              add [bx+si],al
0000208C  0010              add [bx+si],dl
0000208E  2800              sub [bx+si],al
00002090  022C              add ch,[si]
00002092  2108              and [bx+si],cx
00002094  0200              add al,[bx+si]
00002096  16                push ss
00002097  00FF              add bh,bh
00002099  C8FFFF2C          enter 0xffff,0x2c
0000209D  2108              and [bx+si],cx
0000209F  0200              add al,[bx+si]
000020A1  16                push ss
000020A2  00FF              add bh,bh
000020A4  C8FFFF0E          enter 0xffff,0xe
000020A8  0F0E              femms
000020AA  0200              add al,[bx+si]
000020AC  0002              add [bp+si],al
000020AE  9F                lahf
000020AF  A223F7            mov [0xf723],al
000020B2  0E                push cs
000020B3  0F0E              femms
000020B5  0200              add al,[bx+si]
000020B7  0002              add [bp+si],al
000020B9  9F                lahf
000020BA  A223F6            mov [0xf623],al
000020BD  0003              add [bp+di],al
000020BF  0E                push cs
000020C0  0000              add [bx+si],al
000020C2  0002              add [bp+si],al
000020C4  FF                db 0xff
000020C5  F8                clc
000020C6  25FF2C            and ax,0x2cff
000020C9  2108              and [bx+si],cx
000020CB  0200              add al,[bx+si]
000020CD  16                push ss
000020CE  00EF              add bh,ch
000020D0  58                pop ax
000020D1  FF                db 0xff
000020D2  FF2F              jmp far [bx]
000020D4  2F                das
000020D5  0802              or [bp+si],al
000020D7  0000              add [bx+si],al
000020D9  00EF              add bh,ch
000020DB  58                pop ax
000020DC  FF                db 0xff
000020DD  FF21              jmp [bx+di]
000020DF  2000              and [bx+si],al
000020E1  0300              add ax,[bx+si]
000020E3  0000              add [bx+si],al
000020E5  77C7              ja 0x20ae
000020E7  3CF6              cmp al,0xf6
000020E9  2120              and [bx+si],sp
000020EB  06                push es
000020EC  0300              add ax,[bx+si]
000020EE  0000              add [bx+si],al
000020F0  74F9              jz 0x20eb
000020F2  ED                in ax,dx
000020F3  3F                aas
000020F4  2120              and [bx+si],sp
000020F6  0E                push cs
000020F7  0300              add ax,[bx+si]
000020F9  0000              add [bx+si],al
000020FB  74F9              jz 0x20f6
000020FD  3DFF01            cmp ax,0x1ff
00002100  0101              add [bx+di],ax
00002102  0000              add [bx+si],al
00002104  3F                aas
00002105  0011              add [bx+di],dl
00002107  1101              adc [bx+di],ax
00002109  016060            add [bx+si+0x60],sp
0000210C  0003              add [bp+di],al
0000210E  0001              add [bx+di],al
00002110  0DF4F3            or ax,0xf3f4
00002113  E73F              out 0x3f,ax
00002115  CF                iret
00002116  42                inc dx
00002117  0A00              or al,[bx+si]
00002119  0000              add [bx+si],al
0000211B  0021              add [bx+di],ah
0000211D  F1                int1
0000211E  02FA              add bh,dl
00002120  21610F            and [bx+di+0xf],sp
00002123  0000              add [bx+si],al
00002125  0000              add [bx+si],al
00002127  F7F1              div cx
00002129  3DFF23            cmp ax,0x23ff
0000212C  61                popa
0000212D  0F0000            sldt [bx+si]
00002130  0000              add [bx+si],al
00002132  B697              mov dh,0x97
00002134  6F                outsw
00002135  50                push ax
00002136  0101              add [bx+di],ax
00002138  0400              add al,0x0
0000213A  0000              add [bx+si],al
0000213C  00E1              add cl,ah
0000213E  EB00              jmp short 0x2140
00002140  0001              add [bx+di],al
00002142  0104              add [si],ax
00002144  0000              add [bx+si],al
00002146  00C0              add al,al
00002148  EF                out dx,ax
00002149  EB02              jmp short 0x214d
0000214B  0402              add al,0x2
0000214D  0E                push cs
0000214E  0A00              or al,[bx+si]
00002150  0000              add [bx+si],al
00002152  06                push es
00002153  52                push dx
00002154  E104              loope 0x215a
00002156  07                pop es
00002157  42                inc dx
00002158  40                inc ax
00002159  0803              or [bp+di],al
0000215B  0000              add [bx+si],al
0000215D  0021              add [bx+di],ah
0000215F  A20124            mov [0x2401],al
00002162  D1                db 0xd1
00002163  310E0302          xor [0x203],cx
00002167  1B0A              sbb cx,[bp+si]
00002169  663DFF086061      cmp eax,0x616008ff
0000216F  0402              add al,0x2
00002171  0012              add [bp+si],dl
00002173  00F5              add ch,dh
00002175  B389              mov bl,0x89
00002177  4F                dec di
00002178  0001              add [bx+di],al
0000217A  0E                push cs
0000217B  0000              add [bx+si],al
0000217D  0000              add [bx+si],al
0000217F  F0E312            lock jcxz 0x2194
00002182  F5                cmc
00002183  0E                push cs
00002184  010E0000          add [0x0],cx
00002188  0000              add [bx+si],al
0000218A  F0E312            lock jcxz 0x219f
0000218D  F5                cmc
0000218E  0205              add al,[di]
00002190  0E                push cs
00002191  0000              add [bx+si],al
00002193  0000              add [bx+si],al
00002195  F0E312            lock jcxz 0x21aa
00002198  F5                cmc
00002199  05020E            add ax,0xe02
0000219C  0000              add [bx+si],al
0000219E  0300              add ax,[bx+si]
000021A0  50                push ax
000021A1  E313              jcxz 0x21b6
000021A3  F5                cmc
000021A4  1474              adc al,0x74
000021A6  0000              add [bx+si],al
000021A8  0000              add [bx+si],al
000021AA  C041F106          rol byte [bx+di-0xf],byte 0x6
000021AE  1F                pop ds
000021AF  00E0              add al,ah
000021B1  0100              add [bx+si],ax
000021B3  0100              add [bx+si],ax
000021B5  0041FF            add [bx+di-0x1],al
000021B8  06                push es
000021B9  16                push ss
000021BA  0008              add [bx+si],cl
000021BC  0800              or [bx+si],al
000021BE  0200              add al,[bx+si]
000021C0  0093A403          add [bp+di+0x3a4],dl
000021C4  F60008            test byte [bx+si],0x8
000021C7  0800              or [bx+si],al
000021C9  0200              add al,[bx+si]
000021CB  0093A403          add [bp+di+0x3a4],dl
000021CF  F60000            test byte [bx+si],0x0
000021D2  0803              or [bp+di],al
000021D4  0000              add [bx+si],al
000021D6  0093A405          add [bp+di+0x5a4],dl
000021DA  F9                stc
000021DB  0000              add [bx+si],al
000021DD  0100              add [bx+si],ax
000021DF  0000              add [bx+si],al
000021E1  00F1              add cl,dh
000021E3  F1                int1
000021E4  1F                pop ds
000021E5  1F                pop ds
000021E6  0000              add [bx+si],al
000021E8  0800              or [bx+si],al
000021EA  000600F7          add [0xf700],al
000021EE  F6FF              idiv bh
000021F0  FFE7              jmp di
000021F2  E008              loopne 0x21fc
000021F4  0002              add [bp+si],al
000021F6  0000              add [bx+si],al
000021F8  93                xchg ax,bx
000021F9  A4                movsb
000021FA  03F6              add si,si
000021FC  0110              add [bx+si],dx
000021FE  0100              add [bx+si],ax
00002200  0204              add al,[si]
00002202  00F5              add ch,dh
00002204  95                xchg ax,bp
00002205  5F                pop di
00002206  27                daa
00002207  0110              add [bx+si],dx
00002209  0100              add [bx+si],ax
0000220B  020604F8          add al,[0xf804]
0000220F  A6                cmpsb
00002210  FF37              push word [bx]
00002212  0301              add ax,[bx+di]
00002214  0000              add [bx+si],al
00002216  0200              add al,[bx+si]
00002218  004057            add [bx+si+0x57],al
0000221B  050503            add ax,0x305
0000221E  E101              loope 0x2221
00002220  0000              add [bx+si],al
00002222  0000              add [bx+si],al
00002224  41                inc cx
00002225  F1                int1
00002226  0312              add dx,[bp+si]
00002228  E4E9              in al,0xe9
0000222A  0000              add [bx+si],al
0000222C  0000              add [bx+si],al
0000222E  00A55784          add [di-0x7ba9],ah
00002232  7FC0              jg 0x21f4
00002234  2100              and [bx+si],ax
00002236  0103              add [bp+di],ax
00002238  0400              add al,0x0
0000223A  6C                insb
0000223B  92                xchg ax,dx
0000223C  0303              add ax,[bp+di]
0000223E  E0F9              loopne 0x2239
00002240  0000              add [bx+si],al
00002242  0000              add [bx+si],al
00002244  00A55780          add [di-0x7fa9],ah
00002248  7FC1              jg 0x220b
0000224A  2100              and [bx+si],ax
0000224C  0103              add [bp+di],ax
0000224E  0000              add [bx+si],al
00002250  6C                insb
00002251  92                xchg ax,dx
00002252  0303              add ax,[bp+di]
00002254  0A08              or cl,[bx+si]
00002256  0800              or [bx+si],al
00002258  0200              add al,[bx+si]
0000225A  0093A403          add [bp+di+0x3a4],dl
0000225E  F603E1            test byte [bp+di],0xe1
00002261  0100              add [bx+si],ax
00002263  0000              add [bx+si],al
00002265  0041F1            add [bx+di-0xf],al
00002268  06                push es
00002269  16                push ss
0000226A  0A08              or cl,[bx+si]
0000226C  0800              or [bx+si],al
0000226E  0200              add al,[bx+si]
00002270  0093A403          add [bp+di+0x3a4],dl
00002274  F6                db 0xf6
00002275  0A08              or cl,[bx+si]
00002277  0800              or [bx+si],al
00002279  0200              add al,[bx+si]
0000227B  0093A401          add [bp+di+0x1a4],dl
0000227F  F1                int1
00002280  0101              add [bx+di],ax
00002282  0000              add [bx+si],al
00002284  0000              add [bx+si],al
00002286  0010              add [bx+si],dl
00002288  2800              sub [bx+si],al
0000228A  0200              add al,[bx+si]
0000228C  A00003            mov al,[0x300]
0000228F  0000              add [bx+si],al
00002291  0041F1            add [bx+di-0xf],al
00002294  0312              add dx,[bp+si]
00002296  00A00000          add [bx+si+0x0],ah
0000229A  0300              add ax,[bx+si]
0000229C  00F1              add cl,dh
0000229E  FB                sti
0000229F  0FFF              ud0
000022A1  0008              add [bx+si],cl
000022A3  0800              or [bx+si],al
000022A5  0200              add al,[bx+si]
000022A7  0093A403          add [bp+di+0x3a4],dl
000022AB  F6262100          mul byte [0x21]
000022AF  0200              add al,[bx+si]
000022B1  0000              add [bx+si],al
000022B3  FFC9              dec cx
000022B5  FF                db 0xff
000022B6  FFE1              jmp cx
000022B8  E90000            jmp 0x22bb
000022BB  0000              add [bx+si],al
000022BD  00A55784          add [di-0x7ba9],ah
000022C1  7F21              jg 0x22e4
000022C3  2000              and [bx+si],al
000022C5  0300              add ax,[bx+si]
000022C7  0000              add [bx+si],al
000022C9  77C7              ja 0x2292
000022CB  3CF6              cmp al,0xf6
000022CD  2120              and [bx+si],sp
000022CF  06                push es
000022D0  0300              add ax,[bx+si]
000022D2  0000              add [bx+si],al
000022D4  74F9              jz 0x22cf
000022D6  ED                in ax,dx
000022D7  3F                aas
000022D8  0105              add [di],ax
000022DA  06                push es
000022DB  0000              add [bx+si],al
000022DD  0000              add [bx+si],al
000022DF  93                xchg ax,bx
000022E0  92                xchg ax,dx
000022E1  03F8              add di,ax
000022E3  0101              add [bx+di],ax
000022E5  0100              add [bx+si],ax
000022E7  003F              add [bx],bh
000022E9  0011              add [bx+di],dl
000022EB  1101              adc [bx+di],ax
000022ED  016060            add [bx+si+0x60],sp
000022F0  0003              add [bp+di],al
000022F2  0001              add [bx+di],al
000022F4  0DF4F3            or ax,0xf3f4
000022F7  E73F              out 0x3f,ax
000022F9  07                pop es
000022FA  050003            add ax,0x300
000022FD  0000              add [bx+si],al
000022FF  0093A503          add [bp+di+0x3a5],dl
00002303  F5                cmc
00002304  0000              add [bx+si],al
00002306  0000              add [bx+si],al
00002308  0200              add al,[bx+si]
0000230A  0093A403          add [bp+di+0x3a4],dl
0000230E  F040              lock inc ax
00002310  40                inc ax
00002311  0A02              or al,[bp+si]
00002313  0000              add [bx+si],al
00002315  00B16411          add [bx+di+0x1164],dh
00002319  FD                std
0000231A  0101              add [bx+di],ax
0000231C  0100              add [bx+si],ax
0000231E  003F              add [bx],bh
00002320  0011              add [bx+di],dl
00002322  11F0              adc ax,si
00002324  F20020            repne add [bx+si],ah
00002327  06                push es
00002328  0200              add al,[bx+si]
0000232A  0000              add [bx+si],al
0000232C  DA                db 0xda
0000232D  E103              loope 0x2332
0000232F  F4                hlt
00002330  E481              in al,0x81
00002332  0000              add [bx+si],al
00002334  00980056          add [bx+si+0x5600],bl
00002338  E173              loope 0x23ad
0000233A  43                inc bx
0000233B  0201              add al,[bx+di]
0000233D  0E                push cs
0000233E  0300              add ax,[bx+si]
00002340  2008              and [bx+si],cl
00002342  F7F5              div bp
00002344  9F                lahf
00002345  FB                sti
00002346  0803              or [bp+di],al
00002348  06                push es
00002349  0100              add [bx+si],ax
0000234B  0810              or [bx+si],dl
0000234D  F6F6              div dh
0000234F  7454              jz 0x23a5
00002351  07                pop es
00002352  0200              add al,[bx+si]
00002354  0000              add [bx+si],al
00002356  1800              sbb [bx+si],al
00002358  33F4              xor si,sp
0000235A  F5                cmc
0000235B  D507              aad 0x7
0000235D  0200              add al,[bx+si]
0000235F  0003              add [bp+di],al
00002361  1400              adc al,0x0
00002363  33F4              xor si,sp
00002365  F5                cmc
00002366  D400              aam 0x0
00002368  020A              add cl,[bp+si]
0000236A  0100              add [bx+si],ax
0000236C  0000              add [bx+si],al
0000236E  44                inc sp
0000236F  8414              test [si],dl
00002371  F60708            test byte [bx],0x8
00002374  0D0201            or ax,0x102
00002377  051440            add ax,0x4014
0000237A  8014F6            adc byte [si],0xf6
0000237D  0002              add [bp+si],al
0000237F  0C01              or al,0x1
00002381  0000              add [bx+si],al
00002383  00F4              add ah,dh
00002385  84F5              test ch,dh
00002387  F5                cmc
00002388  C0810F0100        rol byte [bx+di+0x10f],byte 0x0
0000238D  080C              or [si],cl
0000238F  CC                int3
00002390  A4                movsb
00002391  4A                dec dx
00002392  3BE1              cmp sp,cx
00002394  0103              add [bp+di],ax
00002396  0200              add al,[bx+si]
00002398  1408              adc al,0x8
0000239A  F23107            repne xor [bx],ax
0000239D  151222            adc ax,0x2212
000023A0  0E                push cs
000023A1  0000              add [bx+si],al
000023A3  2000              and [bx+si],al
000023A5  F9                stc
000023A6  83F417            xor sp,byte +0x17
000023A9  3333              xor si,[bp+di]
000023AB  0D0100            or ax,0x1
000023AE  3108              xor [bx+si],cx
000023B0  A1A2A5            mov ax,[0xa5a2]
000023B3  B504              mov ch,0x4
000023B5  200C              and [si],cl
000023B7  0201              add al,[bx+di]
000023B9  0F                db 0x0f
000023BA  007BC2            add [bp+di-0x3e],bh
000023BD  2525C1            and ax,0xc125
000023C0  210A              and [bp+si],cx
000023C2  0201              add al,[bx+di]
000023C4  07                pop es
000023C5  0053C2            add [bp+di-0x3e],dl
000023C8  1274D1            adc dh,[si-0x2f]
000023CB  810601000200      add word [0x1],0x2
000023D1  83B633110F        xor word [bp+0x1133],byte +0xf
000023D6  0100              add [bx+si],ax
000023D8  0003              add [bp+di],al
000023DA  0900              or [bx+si],ax
000023DC  94                xchg ax,sp
000023DD  55                push bp
000023DE  1124              adc [si],sp
000023E0  0101              add [bx+di],ax
000023E2  0A00              or al,[bx+si]
000023E4  001D              add [di],bl
000023E6  00F2              add dl,dh
000023E8  F5                cmc
000023E9  EF                out dx,ax
000023EA  7801              js 0x23ed
000023EC  210A              and [bp+si],cx
000023EE  0000              add [bx+si],al
000023F0  158025            adc ax,0x2580
000023F3  652F              gs das
000023F5  6C                insb
000023F6  41                inc cx
000023F7  13060000          adc ax,[0x0]
000023FB  9D                popf
000023FC  00F2              add dl,dh
000023FE  F251              repne push cx
00002400  F1                int1
00002401  351808            xor ax,0x818
00002404  0000              add [bx+si],al
00002406  8480A87D          test [bx+si+0x7da8],al
0000240A  1033              adc [bp+di],dh
0000240C  3221              xor ah,[bx+di]
0000240E  0E                push cs
0000240F  0000              add [bx+si],al
00002411  16                push ss
00002412  80737524          xor byte [bp+di+0x75],0x24
00002416  57                push di
00002417  30710C            xor [bx+di+0xc],dh
0000241A  0000              add [bx+si],al
0000241C  C880D561          enter 0xd580,0x61
00002420  191B              sbb [bp+di],bx
00002422  21A20700          and [bp+si+0x7],sp
00002426  00838D74          add [bp+di+0x748d],al
0000242A  6517              gs pop ss
0000242C  17                pop ss
0000242D  31610C            xor [bx+di+0xc],sp
00002430  0000              add [bx+si],al
00002432  1C80              sbb al,0x80
00002434  41                inc cx
00002435  92                xchg ax,dx
00002436  0B3B              or di,[bp+di]
00002438  0004              add [si],al
0000243A  AB                stosw
0000243B  00C8              add al,cl
0000243D  0187008C          add [bx-0x7400],ax
00002441  0083FFA0          add [bp+di-0x5f01],al
00002445  0090C4A0          add [bx+si-0x5f3c],dl
00002449  00B901A0          add [bx+di-0x5fff],bh
0000244D  001B              add [bp+di],bl
0000244F  0C89              or al,0x89
00002451  001B              add [bp+di],bl
00002453  0C89              or al,0x89
00002455  001B              add [bp+di],bl
00002457  0C89              or al,0x89
00002459  001B              add [bp+di],bl
0000245B  18890C19          sbb [bx+di+0x190c],cl
0000245F  0C89              or al,0x89
00002461  0019              add [bx+di],bl
00002463  0C89              or al,0x89
00002465  0019              add [bx+di],bl
00002467  0C89              or al,0x89
00002469  0019              add [bx+di],bl
0000246B  18890C1B          sbb [bx+di+0x1b0c],cl
0000246F  0C89              or al,0x89
00002471  001B              add [bp+di],bl
00002473  0C89              or al,0x89
00002475  001B              add [bp+di],bl
00002477  0C89              or al,0x89
00002479  001B              add [bp+di],bl
0000247B  18890C17          sbb [bx+di+0x170c],cl
0000247F  0C89              or al,0x89
00002481  0017              add [bx],dl
00002483  0C89              or al,0x89
00002485  0017              add [bx],dl
00002487  0C89              or al,0x89
00002489  0017              add [bx],dl
0000248B  18890C1B          sbb [bx+di+0x1b0c],cl
0000248F  0C89              or al,0x89
00002491  001B              add [bp+di],bl
00002493  0C89              or al,0x89
00002495  001B              add [bp+di],bl
00002497  0C89              or al,0x89
00002499  001B              add [bp+di],bl
0000249B  18890C19          sbb [bx+di+0x190c],cl
0000249F  0C89              or al,0x89
000024A1  0019              add [bx+di],bl
000024A3  0C89              or al,0x89
000024A5  0019              add [bx+di],bl
000024A7  0C89              or al,0x89
000024A9  0019              add [bx+di],bl
000024AB  18890C1B          sbb [bx+di+0x1b0c],cl
000024AF  0C89              or al,0x89
000024B1  001B              add [bp+di],bl
000024B3  0C89              or al,0x89
000024B5  001B              add [bp+di],bl
000024B7  0C89              or al,0x89
000024B9  001B              add [bp+di],bl
000024BB  18890C17          sbb [bx+di+0x170c],cl
000024BF  0C89              or al,0x89
000024C1  0017              add [bx],dl
000024C3  0C89              or al,0x89
000024C5  0017              add [bx],dl
000024C7  0C89              or al,0x89
000024C9  0017              add [bx],dl
000024CB  18890C24          sbb [bx+di+0x240c],cl
000024CF  0C89              or al,0x89
000024D1  0024              add [si],ah
000024D3  0C89              or al,0x89
000024D5  0024              add [si],ah
000024D7  0C89              or al,0x89
000024D9  0024              add [si],ah
000024DB  0C89              or al,0x89
000024DD  0022              add [bp+si],ah
000024DF  0C89              or al,0x89
000024E1  0022              add [bp+si],ah
000024E3  0C89              or al,0x89
000024E5  0022              add [bp+si],ah
000024E7  0C89              or al,0x89
000024E9  0022              add [bp+si],ah
000024EB  0C89              or al,0x89
000024ED  0021              add [bx+di],ah
000024EF  0C89              or al,0x89
000024F1  0021              add [bx+di],ah
000024F3  0C89              or al,0x89
000024F5  0021              add [bx+di],ah
000024F7  0C89              or al,0x89
000024F9  0021              add [bx+di],ah
000024FB  0C89              or al,0x89
000024FD  0C26              or al,0x26
000024FF  0C89              or al,0x89
00002501  00260C89          add [0x890c],ah
00002505  0024              add [si],ah
00002507  0C89              or al,0x89
00002509  00261889          add [0x8918],ah
0000250D  0024              add [si],ah
0000250F  2489              and al,0x89
00002511  00A03084          add [bx+si-0x7bd0],ah
00002515  44                inc sp
00002516  2401              and al,0x1
00002518  04AB              add al,0xab
0000251A  00C8              add al,cl
0000251C  0187008C          add [bx-0x7400],ax
00002520  0083FF84          add [bp+di-0x7b01],al
00002524  44                inc sp
00002525  2402              and al,0x2
00002527  04AB              add al,0xab
00002529  00C8              add al,cl
0000252B  0187008C          add [bx-0x7400],ax
0000252F  0083FF84          add [bp+di-0x7b01],al
00002533  44                inc sp
00002534  2403              and al,0x3
00002536  04AB              add al,0xab
00002538  06                push es
00002539  C8018700          enter 0x8701,0x0
0000253D  8C00              mov [bx+si],es
0000253F  83FFC4            cmp di,byte -0x3c
00002542  1F                pop ds
00002543  08A000A0          or [bx+si-0x6000],ah
00002547  00A000B9          add [bx+si-0x4700],ah
0000254B  01A000B9          add [bx+si-0x4700],sp
0000254F  F7A000C2          mul word [bx+si-0x3e00]
00002553  10A000C2          adc [bx+si-0x3e00],ah
00002557  02A006A0          add ah,[bx+si-0x5ffa]
0000255B  00A012C2          add [bx+si-0x3dee],ah
0000255F  02A006A0          add ah,[bx+si-0x5ffa]
00002563  2AC2              sub al,dl
00002565  10A000C2          adc [bx+si-0x3e00],ah
00002569  02A006A0          add ah,[bx+si-0x5ffa]
0000256D  00A012C2          add [bx+si-0x3dee],ah
00002571  02A006A0          add ah,[bx+si-0x5ffa]
00002575  2AC2              sub al,dl
00002577  10A000C2          adc [bx+si-0x3e00],ah
0000257B  02A006A0          add ah,[bx+si-0x5ffa]
0000257F  00A012C2          add [bx+si-0x3dee],ah
00002583  02A006A0          add ah,[bx+si-0x5ffa]
00002587  2AC2              sub al,dl
00002589  10A000C2          adc [bx+si-0x3e00],ah
0000258D  02A006A0          add ah,[bx+si-0x5ffa]
00002591  00A012C2          add [bx+si-0x3dee],ah
00002595  02A006A0          add ah,[bx+si-0x5ffa]
00002599  2AC2              sub al,dl
0000259B  10A000C2          adc [bx+si-0x3e00],ah
0000259F  02A006A0          add ah,[bx+si-0x5ffa]
000025A3  00A012C2          add [bx+si-0x3dee],ah
000025A7  02A006A0          add ah,[bx+si-0x5ffa]
000025AB  2AC2              sub al,dl
000025AD  10A000C2          adc [bx+si-0x3e00],ah
000025B1  02A006A0          add ah,[bx+si-0x5ffa]
000025B5  00A012C2          add [bx+si-0x3dee],ah
000025B9  02A006A0          add ah,[bx+si-0x5ffa]
000025BD  2AC2              sub al,dl
000025BF  10A000C2          adc [bx+si-0x3e00],ah
000025C3  02A006A0          add ah,[bx+si-0x5ffa]
000025C7  00A012C2          add [bx+si-0x3dee],ah
000025CB  02A006A0          add ah,[bx+si-0x5ffa]
000025CF  2AC2              sub al,dl
000025D1  10A000C2          adc [bx+si-0x3e00],ah
000025D5  02A006A0          add ah,[bx+si-0x5ffa]
000025D9  00A012C2          add [bx+si-0x3dee],ah
000025DD  02A006A0          add ah,[bx+si-0x5ffa]
000025E1  2AC2              sub al,dl
000025E3  10A000C2          adc [bx+si-0x3e00],ah
000025E7  02A006A0          add ah,[bx+si-0x5ffa]
000025EB  00A012C2          add [bx+si-0x3dee],ah
000025EF  02A006A0          add ah,[bx+si-0x5ffa]
000025F3  2AC2              sub al,dl
000025F5  10A000C2          adc [bx+si-0x3e00],ah
000025F9  02A006A0          add ah,[bx+si-0x5ffa]
000025FD  00A012C2          add [bx+si-0x3dee],ah
00002601  02A006A0          add ah,[bx+si-0x5ffa]
00002605  2AC2              sub al,dl
00002607  10A000C2          adc [bx+si-0x3e00],ah
0000260B  02A006A0          add ah,[bx+si-0x5ffa]
0000260F  00A012C2          add [bx+si-0x3dee],ah
00002613  02A006A0          add ah,[bx+si-0x5ffa]
00002617  2AC2              sub al,dl
00002619  10A000C2          adc [bx+si-0x3e00],ah
0000261D  02A006A0          add ah,[bx+si-0x5ffa]
00002621  00A012C2          add [bx+si-0x3dee],ah
00002625  02A006A0          add ah,[bx+si-0x5ffa]
00002629  2A844425          sub al,[si+0x2544]
0000262D  0904              or [si],ax
0000262F  AE                scasb
00002630  01AF01C1          add [bx-0x3eff],bp
00002634  C5                db 0xc5
00002635  C6C706            mov bh,0x6
00002638  000D              add [di],cl
0000263A  60                pusha
0000263B  07                pop es
0000263C  00A6D282          add [bp-0x7d2e],ah
00002640  53                push bx
00002641  82                db 0x82
00002642  54                push sp
00002643  82                db 0x82
00002644  55                push bp
00002645  82                db 0x82
00002646  56                push si
00002647  8801              mov [bx+di],al
00002649  11060000          adc [0x0],ax
0000264D  4F                dec di
0000264E  00F1              add cl,dh
00002650  D25374            rcl byte [bp+di+0x74],cl
00002653  0000              add [bx+si],al
00002655  0000              add [bx+si],al
00002657  000B              add [bp+di],cl
00002659  00A8D64C          add [bx+si+0x4cd6],ch
0000265D  4F                dec di
0000265E  0000              add [bx+si],al
00002660  0100              add [bx+si],ax
00002662  0000              add [bx+si],al
00002664  0000              add [bx+si],al
00002666  0000              add [bx+si],al
00002668  0000              add [bx+si],al
0000266A  0001              add [bx+di],al
0000266C  0000              add [bx+si],al
0000266E  0000              add [bx+si],al
00002670  00D6              add dh,dl
00002672  004F55            add [bx+0x55],cl
00002675  8BEC              mov bp,sp
00002677  56                push si
00002678  33C0              xor ax,ax
0000267A  50                push ax
0000267B  FF7608            push word [bp+0x8]
0000267E  FF7606            push word [bp+0x6]
00002681  9AA53A0000        call 0x0:0x3aa5
00002686  83C406            add sp,byte +0x6
00002689  8BF0              mov si,ax
0000268B  3DFFFF            cmp ax,0xffff
0000268E  7502              jnz 0x2692
00002690  EB1A              jmp short 0x26ac
00002692  F7460A0200        test word [bp+0xa],0x2
00002697  7406              jz 0x269f
00002699  F7C60100          test si,0x1
0000269D  7504              jnz 0x26a3
0000269F  33C0              xor ax,ax
000026A1  EB09              jmp short 0x26ac
000026A3  C7067D000500      mov word [0x7d],0x5
000026A9  B8FFFF            mov ax,0xffff
000026AC  5E                pop si
000026AD  5D                pop bp
000026AE  CB                retf
000026AF  55                push bp
000026B0  8BEC              mov bp,sp
000026B2  833E821120        cmp word [0x1182],byte +0x20
000026B7  7505              jnz 0x26be
000026B9  B80100            mov ax,0x1
000026BC  EB1C              jmp short 0x26da
000026BE  8B1E8211          mov bx,[0x1182]
000026C2  D1E3              shl bx,1
000026C4  D1E3              shl bx,1
000026C6  8B4608            mov ax,[bp+0x8]
000026C9  8B5606            mov dx,[bp+0x6]
000026CC  8997FC57          mov [bx+0x57fc],dx
000026D0  8987FE57          mov [bx+0x57fe],ax
000026D4  FF068211          inc word [0x1182]
000026D8  33C0              xor ax,ax
000026DA  5D                pop bp
000026DB  CB                retf

; ---- 0000:26DC ----
000026DC  55                push bp
000026DD  8BEC              mov bp,sp
000026DF  56                push si
000026E0  8B7606            mov si,[bp+0x6]
000026E3  0BF6              or si,si
000026E5  7C05              jl 0x26ec
000026E7  83FE14            cmp si,byte +0x14
000026EA  7C09              jl 0x26f5
000026EC  B80600            mov ax,0x6
000026EF  50                push ax
000026F0  E8FE16            call 0x3df1
000026F3  EB11              jmp short 0x2706
000026F5  8BDE              mov bx,si
000026F7  D1E3              shl bx,1
000026F9  C7872414FFFF      mov word [bx+0x1424],0xffff
000026FF  56                push si
00002700  9AC23A0000        call 0x0:0x3ac2
00002705  59                pop cx
00002706  5E                pop si
00002707  5D                pop bp
00002708  CB                retf
00002709  CB                retf

; ---- 0000:270A ----
0000270A  55                push bp
0000270B  8BEC              mov bp,sp
0000270D  EB0C              jmp short 0x271b
0000270F  8B1E8211          mov bx,[0x1182]
00002713  D1E3              shl bx,1
00002715  D1E3              shl bx,1
00002717  FF9FFC57          call far [bx+0x57fc]
0000271B  A18211            mov ax,[0x1182]
0000271E  FF0E8211          dec word [0x1182]
00002722  0BC0              or ax,ax
00002724  75E9              jnz 0x270f
00002726  FF7606            push word [bp+0x6]
00002729  E8CED9            call 0xfa
0000272C  59                pop cx
0000272D  5D                pop bp
0000272E  CB                retf

; ---- 0000:272F ----
0000272F  55                push bp
00002730  8BEC              mov bp,sp
00002732  56                push si
00002733  57                push di
00002734  8B4606            mov ax,[bp+0x6]
00002737  40                inc ax
00002738  8BF0              mov si,ax
0000273A  2B367900          sub si,[0x79]
0000273E  8BC6              mov ax,si
00002740  053F00            add ax,0x3f
00002743  B106              mov cl,0x6
00002745  D3E8              shr ax,cl
00002747  8BF0              mov si,ax
00002749  3B069212          cmp ax,[0x1292]
0000274D  7512              jnz 0x2761
0000274F  8B4606            mov ax,[bp+0x6]
00002752  8B5604            mov dx,[bp+0x4]
00002755  89168700          mov [0x87],dx
00002759  A38900            mov [0x89],ax
0000275C  B80100            mov ax,0x1
0000275F  EB48              jmp short 0x27a9
00002761  B106              mov cl,0x6
00002763  D3E6              shl si,cl
00002765  8B3E8D00          mov di,[0x8d]
00002769  8BC6              mov ax,si
0000276B  03067900          add ax,[0x79]
0000276F  3BC7              cmp ax,di
00002771  7608              jna 0x277b
00002773  8BC7              mov ax,di
00002775  2B067900          sub ax,[0x79]
00002779  8BF0              mov si,ax
0000277B  56                push si
0000277C  FF367900          push word [0x79]
00002780  9AC3410000        call 0x0:0x41c3
00002785  59                pop cx
00002786  59                pop cx
00002787  8BF8              mov di,ax
00002789  3DFFFF            cmp ax,0xffff
0000278C  750B              jnz 0x2799
0000278E  8BC6              mov ax,si
00002790  B106              mov cl,0x6
00002792  D3E8              shr ax,cl
00002794  A39212            mov [0x1292],ax
00002797  EBB6              jmp short 0x274f
00002799  A17900            mov ax,[0x79]
0000279C  03C7              add ax,di
0000279E  C7068B000000      mov word [0x8b],0x0
000027A4  A38D00            mov [0x8d],ax
000027A7  33C0              xor ax,ax
000027A9  5F                pop di
000027AA  5E                pop si
000027AB  5D                pop bp
000027AC  C20400            ret 0x4

; ---- 0000:27AF ----
000027AF  55                push bp
000027B0  8BEC              mov bp,sp
000027B2  8B0E8500          mov cx,[0x85]
000027B6  8B1E8300          mov bx,[0x83]
000027BA  8B5606            mov dx,[bp+0x6]
000027BD  8B4604            mov ax,[bp+0x4]
000027C0  E8F929            call 0x51bc
000027C3  7220              jc 0x27e5
000027C5  8B0E8D00          mov cx,[0x8d]
000027C9  8B1E8B00          mov bx,[0x8b]
000027CD  8B5606            mov dx,[bp+0x6]
000027D0  8B4604            mov ax,[bp+0x4]
000027D3  E8E629            call 0x51bc
000027D6  770D              ja 0x27e5
000027D8  FF7606            push word [bp+0x6]
000027DB  FF7604            push word [bp+0x4]
000027DE  E84EFF            call 0x272f
000027E1  0BC0              or ax,ax
000027E3  7505              jnz 0x27ea
000027E5  B8FFFF            mov ax,0xffff
000027E8  EB02              jmp short 0x27ec
000027EA  33C0              xor ax,ax
000027EC  5D                pop bp
000027ED  C3                ret

; ---- 0000:27EE ----
000027EE  55                push bp
000027EF  8BEC              mov bp,sp
000027F1  83EC08            sub sp,byte +0x8
000027F4  8B168900          mov dx,[0x89]
000027F8  A18700            mov ax,[0x87]
000027FB  8B4E06            mov cx,[bp+0x6]
000027FE  8B5E04            mov bx,[bp+0x4]
00002801  E8FA28            call 0x50fe
00002804  8946FC            mov [bp-0x4],ax
00002807  8956FE            mov [bp-0x2],dx
0000280A  8B0E8500          mov cx,[0x85]
0000280E  8B1E8300          mov bx,[0x83]
00002812  8B46FC            mov ax,[bp-0x4]
00002815  E8A429            call 0x51bc
00002818  7213              jc 0x282d
0000281A  8B0E8D00          mov cx,[0x8d]
0000281E  8B1E8B00          mov bx,[0x8b]
00002822  8B56FE            mov dx,[bp-0x2]
00002825  8B46FC            mov ax,[bp-0x4]
00002828  E89129            call 0x51bc
0000282B  7608              jna 0x2835
0000282D  BAFFFF            mov dx,0xffff
00002830  B8FFFF            mov ax,0xffff
00002833  EB22              jmp short 0x2857
00002835  A18900            mov ax,[0x89]
00002838  8B168700          mov dx,[0x87]
0000283C  8956F8            mov [bp-0x8],dx
0000283F  8946FA            mov [bp-0x6],ax
00002842  FF76FE            push word [bp-0x2]
00002845  FF76FC            push word [bp-0x4]
00002848  E8E4FE            call 0x272f
0000284B  0BC0              or ax,ax
0000284D  7502              jnz 0x2851
0000284F  EBDC              jmp short 0x282d
00002851  8B56FA            mov dx,[bp-0x6]
00002854  8B46F8            mov ax,[bp-0x8]
00002857  8BE5              mov sp,bp
00002859  5D                pop bp
0000285A  C3                ret

; ---- 0000:285B ----
0000285B  55                push bp
0000285C  8BEC              mov bp,sp
0000285E  56                push si
0000285F  BEFFFF            mov si,0xffff
00002862  C45E06            les bx,[bp+0x6]
00002865  268B4712          mov ax,[es:bx+0x12]
00002869  3BC3              cmp ax,bx
0000286B  7403              jz 0x2870
0000286D  E99200            jmp 0x2902
00002870  C45E06            les bx,[bp+0x6]
00002873  26837F0600        cmp word [es:bx+0x6],byte +0x0
00002878  7431              jz 0x28ab
0000287A  26833F00          cmp word [es:bx],byte +0x0
0000287E  7D11              jnl 0x2891
00002880  FF7608            push word [bp+0x8]
00002883  53                push bx
00002884  9A57290000        call 0x0:0x2957
00002889  59                pop cx
0000288A  59                pop cx
0000288B  0BC0              or ax,ax
0000288D  7402              jz 0x2891
0000288F  EB71              jmp short 0x2902
00002891  C45E06            les bx,[bp+0x6]
00002894  26F747020400      test word [es:bx+0x2],0x4
0000289A  740F              jz 0x28ab
0000289C  26FF770A          push word [es:bx+0xa]
000028A0  26FF7708          push word [es:bx+0x8]
000028A4  9A8E4C0000        call 0x0:0x4c8e
000028A9  59                pop cx
000028AA  59                pop cx
000028AB  C45E06            les bx,[bp+0x6]
000028AE  26807F0400        cmp byte [es:bx+0x4],0x0
000028B3  7C0E              jl 0x28c3
000028B5  268A4704          mov al,[es:bx+0x4]
000028B9  98                cbw
000028BA  50                push ax
000028BB  9ADC260000        call 0x0:0x26dc
000028C0  59                pop cx
000028C1  8BF0              mov si,ax
000028C3  C45E06            les bx,[bp+0x6]
000028C6  26C747020000      mov word [es:bx+0x2],0x0
000028CC  26C747060000      mov word [es:bx+0x6],0x0
000028D2  26C7070000        mov word [es:bx],0x0
000028D7  26C64704FF        mov byte [es:bx+0x4],0xff
000028DC  26837F1000        cmp word [es:bx+0x10],byte +0x0
000028E1  741F              jz 0x2902
000028E3  33C0              xor ax,ax
000028E5  33D2              xor dx,dx
000028E7  50                push ax
000028E8  52                push dx
000028E9  26FF7710          push word [es:bx+0x10]
000028ED  E8B60E            call 0x37a6
000028F0  52                push dx
000028F1  50                push ax
000028F2  9AFE450000        call 0x0:0x45fe
000028F7  59                pop cx
000028F8  59                pop cx
000028F9  C45E06            les bx,[bp+0x6]
000028FC  26C747100000      mov word [es:bx+0x10],0x0
00002902  8BC6              mov ax,si
00002904  5E                pop si
00002905  5D                pop bp
00002906  CB                retf

; ---- 0000:2907 ----
00002907  55                push bp
00002908  8BEC              mov bp,sp
0000290A  83EC04            sub sp,byte +0x4
0000290D  C746FC0000        mov word [bp-0x4],0x0
00002912  C746FE0000        mov word [bp-0x2],0x0
00002917  8B168D00          mov dx,[0x8d]
0000291B  A18B00            mov ax,[0x8b]
0000291E  8B0E8900          mov cx,[0x89]
00002922  8B1E8700          mov bx,[0x87]
00002926  E83528            call 0x515e
00002929  8946FC            mov [bp-0x4],ax
0000292C  8956FE            mov [bp-0x2],dx
0000292F  0BD2              or dx,dx
00002931  7210              jc 0x2943
00002933  7706              ja 0x293b
00002935  837EFC10          cmp word [bp-0x4],byte +0x10
00002939  7608              jna 0x2943
0000293B  836EFC10          sub word [bp-0x4],byte +0x10
0000293F  835EFE00          sbb word [bp-0x2],byte +0x0
00002943  8166FCF0FF        and word [bp-0x4],0xfff0
00002948  8166FEFFFF        and word [bp-0x2],0xffff
0000294D  8B56FE            mov dx,[bp-0x2]
00002950  8B46FC            mov ax,[bp-0x4]
00002953  8BE5              mov sp,bp
00002955  5D                pop bp
00002956  CB                retf

; ---- 0000:2957 ----
00002957  55                push bp
00002958  8BEC              mov bp,sp
0000295A  56                push si
0000295B  8B4606            mov ax,[bp+0x6]
0000295E  0B4608            or ax,[bp+0x8]
00002961  7508              jnz 0x296b
00002963  9A262A0000        call 0x0:0x2a26
00002968  E9B600            jmp 0x2a21
0000296B  C45E06            les bx,[bp+0x6]
0000296E  268B4712          mov ax,[es:bx+0x12]
00002972  3BC3              cmp ax,bx
00002974  7406              jz 0x297c
00002976  B8FFFF            mov ax,0xffff
00002979  E9A700            jmp 0x2a23
0000297C  C45E06            les bx,[bp+0x6]
0000297F  26833F00          cmp word [es:bx],byte +0x0
00002983  7C4E              jl 0x29d3
00002985  26F747020800      test word [es:bx+0x2],0x8
0000298B  7515              jnz 0x29a2
0000298D  268B470E          mov ax,[es:bx+0xe]
00002991  8B5606            mov dx,[bp+0x6]
00002994  83C205            add dx,byte +0x5
00002997  3B4608            cmp ax,[bp+0x8]
0000299A  7535              jnz 0x29d1
0000299C  2639570C          cmp [es:bx+0xc],dx
000029A0  752F              jnz 0x29d1
000029A2  C45E06            les bx,[bp+0x6]
000029A5  26C7070000        mov word [es:bx],0x0
000029AA  268B470E          mov ax,[es:bx+0xe]
000029AE  8B5606            mov dx,[bp+0x6]
000029B1  83C205            add dx,byte +0x5
000029B4  3B4608            cmp ax,[bp+0x8]
000029B7  7518              jnz 0x29d1
000029B9  2639570C          cmp [es:bx+0xc],dx
000029BD  7512              jnz 0x29d1
000029BF  268B470A          mov ax,[es:bx+0xa]
000029C3  268B5708          mov dx,[es:bx+0x8]
000029C7  2689570C          mov [es:bx+0xc],dx
000029CB  2689470E          mov [es:bx+0xe],ax
000029CF  EB50              jmp short 0x2a21
000029D1  EB4E              jmp short 0x2a21
000029D3  C45E06            les bx,[bp+0x6]
000029D6  268B4706          mov ax,[es:bx+0x6]
000029DA  260307            add ax,[es:bx]
000029DD  40                inc ax
000029DE  8BF0              mov si,ax
000029E0  268B07            mov ax,[es:bx]
000029E3  2BC6              sub ax,si
000029E5  268907            mov [es:bx],ax
000029E8  56                push si
000029E9  268B470A          mov ax,[es:bx+0xa]
000029ED  268B5708          mov dx,[es:bx+0x8]
000029F1  2689570C          mov [es:bx+0xc],dx
000029F5  2689470E          mov [es:bx+0xe],ax
000029F9  50                push ax
000029FA  52                push dx
000029FB  268A4704          mov al,[es:bx+0x4]
000029FF  98                cbw
00002A00  50                push ax
00002A01  9A4F380000        call 0x0:0x384f
00002A06  83C408            add sp,byte +0x8
00002A09  3BC6              cmp ax,si
00002A0B  7414              jz 0x2a21
00002A0D  C45E06            les bx,[bp+0x6]
00002A10  26F747020002      test word [es:bx+0x2],0x200
00002A16  7509              jnz 0x2a21
00002A18  26814F021000      or word [es:bx+0x2],0x10
00002A1E  E955FF            jmp 0x2976
00002A21  33C0              xor ax,ax
00002A23  5E                pop si
00002A24  5D                pop bp
00002A25  CB                retf

; ---- 0000:2A26 ----
00002A26  55                push bp
00002A27  8BEC              mov bp,sp
00002A29  83EC04            sub sp,byte +0x4
00002A2C  56                push si
00002A2D  57                push di
00002A2E  33FF              xor di,di
00002A30  BE1400            mov si,0x14
00002A33  C746FC9412        mov word [bp-0x4],0x1294
00002A38  8C5EFE            mov [bp-0x2],ds
00002A3B  EB1B              jmp short 0x2a58
00002A3D  C45EFC            les bx,[bp-0x4]
00002A40  26F747020300      test word [es:bx+0x2],0x3
00002A46  740C              jz 0x2a54
00002A48  FF76FE            push word [bp-0x2]
00002A4B  53                push bx
00002A4C  9A57290000        call 0x0:0x2957
00002A51  59                pop cx
00002A52  59                pop cx
00002A53  47                inc di
00002A54  8346FC14          add word [bp-0x4],byte +0x14
00002A58  8BC6              mov ax,si
00002A5A  4E                dec si
00002A5B  0BC0              or ax,ax
00002A5D  75DE              jnz 0x2a3d
00002A5F  8BC7              mov ax,di
00002A61  5F                pop di
00002A62  5E                pop si
00002A63  8BE5              mov sp,bp
00002A65  5D                pop bp
00002A66  CB                retf

; ---- 0000:2A67 ----
00002A67  55                push bp
00002A68  8BEC              mov bp,sp
00002A6A  56                push si
00002A6B  57                push di
00002A6C  33F6              xor si,si
00002A6E  33C9              xor cx,cx
00002A70  33FF              xor di,di
00002A72  C45E0C            les bx,[bp+0xc]
00002A75  FF460C            inc word [bp+0xc]
00002A78  268A07            mov al,[es:bx]
00002A7B  8AD0              mov dl,al
00002A7D  3C72              cmp al,0x72
00002A7F  7508              jnz 0x2a89
00002A81  BE0100            mov si,0x1
00002A84  BF0100            mov di,0x1
00002A87  EB1F              jmp short 0x2aa8
00002A89  80FA77            cmp dl,0x77
00002A8C  7505              jnz 0x2a93
00002A8E  BE0203            mov si,0x302
00002A91  EB08              jmp short 0x2a9b
00002A93  80FA61            cmp dl,0x61
00002A96  750B              jnz 0x2aa3
00002A98  BE0209            mov si,0x902
00002A9B  B98000            mov cx,0x80
00002A9E  BF0200            mov di,0x2
00002AA1  EB05              jmp short 0x2aa8
00002AA3  33C0              xor ax,ax
00002AA5  E98200            jmp 0x2b2a
00002AA8  C45E0C            les bx,[bp+0xc]
00002AAB  268A07            mov al,[es:bx]
00002AAE  8AD0              mov dl,al
00002AB0  FF460C            inc word [bp+0xc]
00002AB3  3C2B              cmp al,0x2b
00002AB5  7411              jz 0x2ac8
00002AB7  C45E0C            les bx,[bp+0xc]
00002ABA  26803F2B          cmp byte [es:bx],0x2b
00002ABE  7525              jnz 0x2ae5
00002AC0  3C74              cmp al,0x74
00002AC2  7404              jz 0x2ac8
00002AC4  3C62              cmp al,0x62
00002AC6  751D              jnz 0x2ae5
00002AC8  80FA2B            cmp dl,0x2b
00002ACB  7508              jnz 0x2ad5
00002ACD  C45E0C            les bx,[bp+0xc]
00002AD0  268A07            mov al,[es:bx]
00002AD3  8AD0              mov dl,al
00002AD5  8BC6              mov ax,si
00002AD7  25FCFF            and ax,0xfffc
00002ADA  0D0400            or ax,0x4
00002ADD  8BF0              mov si,ax
00002ADF  B98001            mov cx,0x180
00002AE2  BF0300            mov di,0x3
00002AE5  80FA74            cmp dl,0x74
00002AE8  7506              jnz 0x2af0
00002AEA  81CE0040          or si,0x4000
00002AEE  EB1E              jmp short 0x2b0e
00002AF0  80FA62            cmp dl,0x62
00002AF3  7506              jnz 0x2afb
00002AF5  81CE0080          or si,0x8000
00002AF9  EB0F              jmp short 0x2b0a
00002AFB  A14C14            mov ax,[0x144c]
00002AFE  2500C0            and ax,0xc000
00002B01  0BF0              or si,ax
00002B03  8BC6              mov ax,si
00002B05  A90080            test ax,0x8000
00002B08  7404              jz 0x2b0e
00002B0A  81CF4000          or di,0x40
00002B0E  C7068A127C39      mov word [0x128a],0x397c
00002B14  C7068C120000      mov word [0x128c],0x0
00002B1A  C45E08            les bx,[bp+0x8]
00002B1D  268937            mov [es:bx],si
00002B20  C45E04            les bx,[bp+0x4]
00002B23  8BC1              mov ax,cx
00002B25  268907            mov [es:bx],ax
00002B28  8BC7              mov ax,di
00002B2A  5F                pop di
00002B2B  5E                pop si
00002B2C  5D                pop bp
00002B2D  C20C00            ret 0xc

; ---- 0000:2B30 ----
00002B30  55                push bp
00002B31  8BEC              mov bp,sp
00002B33  83EC04            sub sp,byte +0x4
00002B36  FF7606            push word [bp+0x6]
00002B39  FF7604            push word [bp+0x4]
00002B3C  16                push ss
00002B3D  8D46FE            lea ax,[bp-0x2]
00002B40  50                push ax
00002B41  16                push ss
00002B42  8D46FC            lea ax,[bp-0x4]
00002B45  50                push ax
00002B46  E81EFF            call 0x2a67
00002B49  C45E0C            les bx,[bp+0xc]
00002B4C  26894702          mov [es:bx+0x2],ax
00002B50  0BC0              or ax,ax
00002B52  7426              jz 0x2b7a
00002B54  26807F0400        cmp byte [es:bx+0x4],0x0
00002B59  7D33              jnl 0x2b8e
00002B5B  FF76FC            push word [bp-0x4]
00002B5E  FF76FE            push word [bp-0x2]
00002B61  FF760A            push word [bp+0xa]
00002B64  FF7608            push word [bp+0x8]
00002B67  9AED3F0000        call 0x0:0x3fed
00002B6C  83C408            add sp,byte +0x8
00002B6F  C45E0C            les bx,[bp+0xc]
00002B72  26884704          mov [es:bx+0x4],al
00002B76  0AC0              or al,al
00002B78  7D14              jnl 0x2b8e
00002B7A  C45E0C            les bx,[bp+0xc]
00002B7D  26C64704FF        mov byte [es:bx+0x4],0xff
00002B82  26C747020000      mov word [es:bx+0x2],0x0
00002B88  33D2              xor dx,dx
00002B8A  33C0              xor ax,ax
00002B8C  EB67              jmp short 0x2bf5
00002B8E  C45E0C            les bx,[bp+0xc]
00002B91  268A4704          mov al,[es:bx+0x4]
00002B95  98                cbw
00002B96  50                push ax
00002B97  9A2E3E0000        call 0x0:0x3e2e
00002B9C  59                pop cx
00002B9D  0BC0              or ax,ax
00002B9F  7409              jz 0x2baa
00002BA1  C45E0C            les bx,[bp+0xc]
00002BA4  26814F020002      or word [es:bx+0x2],0x200
00002BAA  B80002            mov ax,0x200
00002BAD  50                push ax
00002BAE  C45E0C            les bx,[bp+0xc]
00002BB1  26854702          test [es:bx+0x2],ax
00002BB5  7405              jz 0x2bbc
00002BB7  B80100            mov ax,0x1
00002BBA  EB02              jmp short 0x2bbe
00002BBC  33C0              xor ax,ax
00002BBE  50                push ax
00002BBF  33C0              xor ax,ax
00002BC1  33D2              xor dx,dx
00002BC3  50                push ax
00002BC4  52                push dx
00002BC5  FF760E            push word [bp+0xe]
00002BC8  FF760C            push word [bp+0xc]
00002BCB  9A26320000        call 0x0:0x3226
00002BD0  83C40C            add sp,byte +0xc
00002BD3  0BC0              or ax,ax
00002BD5  740F              jz 0x2be6
00002BD7  FF760E            push word [bp+0xe]
00002BDA  FF760C            push word [bp+0xc]
00002BDD  9A5B280000        call 0x0:0x285b
00002BE2  59                pop cx
00002BE3  59                pop cx
00002BE4  EBA2              jmp short 0x2b88
00002BE6  C45E0C            les bx,[bp+0xc]
00002BE9  26C747100000      mov word [es:bx+0x10],0x0
00002BEF  8B560E            mov dx,[bp+0xe]
00002BF2  8B460C            mov ax,[bp+0xc]
00002BF5  8BE5              mov sp,bp
00002BF7  5D                pop bp
00002BF8  C20C00            ret 0xc

; ---- 0000:2BFB ----
00002BFB  55                push bp
00002BFC  8BEC              mov bp,sp
00002BFE  83EC04            sub sp,byte +0x4
00002C01  C746FC9412        mov word [bp-0x4],0x1294
00002C06  8C5EFE            mov [bp-0x2],ds
00002C09  C45EFC            les bx,[bp-0x4]
00002C0C  26807F0400        cmp byte [es:bx+0x4],0x0
00002C11  7C0C              jl 0x2c1f
00002C13  8B46FC            mov ax,[bp-0x4]
00002C16  8346FC14          add word [bp-0x4],byte +0x14
00002C1A  3D2414            cmp ax,0x1424
00002C1D  72EA              jc 0x2c09
00002C1F  C45EFC            les bx,[bp-0x4]
00002C22  26807F0400        cmp byte [es:bx+0x4],0x0
00002C27  7C06              jl 0x2c2f
00002C29  33D2              xor dx,dx
00002C2B  33C0              xor ax,ax
00002C2D  EB06              jmp short 0x2c35
00002C2F  8B56FE            mov dx,[bp-0x2]
00002C32  8B46FC            mov ax,[bp-0x4]
00002C35  8BE5              mov sp,bp
00002C37  5D                pop bp
00002C38  C3                ret

; ---- 0000:2C39 ----
00002C39  55                push bp
00002C3A  8BEC              mov bp,sp
00002C3C  83EC04            sub sp,byte +0x4
00002C3F  E8B9FF            call 0x2bfb
00002C42  8946FC            mov [bp-0x4],ax
00002C45  8956FE            mov [bp-0x2],dx
00002C48  0BC2              or ax,dx
00002C4A  7506              jnz 0x2c52
00002C4C  33D2              xor dx,dx
00002C4E  33C0              xor ax,ax
00002C50  EB15              jmp short 0x2c67
00002C52  FF76FE            push word [bp-0x2]
00002C55  FF76FC            push word [bp-0x4]
00002C58  FF7608            push word [bp+0x8]
00002C5B  FF7606            push word [bp+0x6]
00002C5E  FF760C            push word [bp+0xc]
00002C61  FF760A            push word [bp+0xa]
00002C64  E8C9FE            call 0x2b30
00002C67  8BE5              mov sp,bp
00002C69  5D                pop bp
00002C6A  CB                retf
00002C6B  55                push bp
00002C6C  8BEC              mov bp,sp
00002C6E  C45E0E            les bx,[bp+0xe]
00002C71  268B4712          mov ax,[es:bx+0x12]
00002C75  3BC3              cmp ax,bx
00002C77  7406              jz 0x2c7f
00002C79  33D2              xor dx,dx
00002C7B  33C0              xor ax,ax
00002C7D  EB22              jmp short 0x2ca1
00002C7F  FF7610            push word [bp+0x10]
00002C82  FF760E            push word [bp+0xe]
00002C85  9A5B280000        call 0x0:0x285b
00002C8A  59                pop cx
00002C8B  59                pop cx
00002C8C  FF7610            push word [bp+0x10]
00002C8F  FF760E            push word [bp+0xe]
00002C92  FF7608            push word [bp+0x8]
00002C95  FF7606            push word [bp+0x6]
00002C98  FF760C            push word [bp+0xc]
00002C9B  FF760A            push word [bp+0xa]
00002C9E  E88FFE            call 0x2b30
00002CA1  5D                pop bp
00002CA2  CB                retf
00002CA3  55                push bp
00002CA4  8BEC              mov bp,sp
00002CA6  83EC04            sub sp,byte +0x4
00002CA9  837E0600          cmp word [bp+0x6],byte +0x0
00002CAD  7C0D              jl 0x2cbc
00002CAF  E849FF            call 0x2bfb
00002CB2  8946FC            mov [bp-0x4],ax
00002CB5  8956FE            mov [bp-0x2],dx
00002CB8  0BC2              or ax,dx
00002CBA  7506              jnz 0x2cc2
00002CBC  33D2              xor dx,dx
00002CBE  33C0              xor ax,ax
00002CC0  EB1D              jmp short 0x2cdf
00002CC2  C45EFC            les bx,[bp-0x4]
00002CC5  8A4606            mov al,[bp+0x6]
00002CC8  26884704          mov [es:bx+0x4],al
00002CCC  FF76FE            push word [bp-0x2]
00002CCF  53                push bx
00002CD0  33C0              xor ax,ax
00002CD2  33D2              xor dx,dx
00002CD4  50                push ax
00002CD5  52                push dx
00002CD6  FF760A            push word [bp+0xa]
00002CD9  FF7608            push word [bp+0x8]
00002CDC  E851FE            call 0x2b30
00002CDF  8BE5              mov sp,bp
00002CE1  5D                pop bp
00002CE2  CB                retf

; ---- 0000:2CE3 ----
00002CE3  55                push bp
00002CE4  8BEC              mov bp,sp
00002CE6  83EC04            sub sp,byte +0x4
00002CE9  56                push si
00002CEA  57                push di
00002CEB  C45E04            les bx,[bp+0x4]
00002CEE  26833F00          cmp word [es:bx],byte +0x0
00002CF2  7D0A              jnl 0x2cfe
00002CF4  268B4706          mov ax,[es:bx+0x6]
00002CF8  260307            add ax,[es:bx]
00002CFB  40                inc ax
00002CFC  EB0B              jmp short 0x2d09
00002CFE  C45E04            les bx,[bp+0x4]
00002D01  268B07            mov ax,[es:bx]
00002D04  99                cwd
00002D05  33C2              xor ax,dx
00002D07  2BC2              sub ax,dx
00002D09  8BF0              mov si,ax
00002D0B  8BF8              mov di,ax
00002D0D  C45E04            les bx,[bp+0x4]
00002D10  26F747024000      test word [es:bx+0x2],0x40
00002D16  7402              jz 0x2d1a
00002D18  EB43              jmp short 0x2d5d
00002D1A  C45E04            les bx,[bp+0x4]
00002D1D  268B470E          mov ax,[es:bx+0xe]
00002D21  268B570C          mov dx,[es:bx+0xc]
00002D25  8956FC            mov [bp-0x4],dx
00002D28  8946FE            mov [bp-0x2],ax
00002D2B  26833F00          cmp word [es:bx],byte +0x0
00002D2F  7D25              jnl 0x2d56
00002D31  EB0D              jmp short 0x2d40
00002D33  FF4EFC            dec word [bp-0x4]
00002D36  C45EFC            les bx,[bp-0x4]
00002D39  26803F0A          cmp byte [es:bx],0xa
00002D3D  7501              jnz 0x2d40
00002D3F  47                inc di
00002D40  8BC6              mov ax,si
00002D42  4E                dec si
00002D43  0BC0              or ax,ax
00002D45  75EC              jnz 0x2d33
00002D47  EB14              jmp short 0x2d5d
00002D49  C45EFC            les bx,[bp-0x4]
00002D4C  FF46FC            inc word [bp-0x4]
00002D4F  26803F0A          cmp byte [es:bx],0xa
00002D53  7501              jnz 0x2d56
00002D55  47                inc di
00002D56  8BC6              mov ax,si
00002D58  4E                dec si
00002D59  0BC0              or ax,ax
00002D5B  75EC              jnz 0x2d49
00002D5D  8BC7              mov ax,di
00002D5F  5F                pop di
00002D60  5E                pop si
00002D61  8BE5              mov sp,bp
00002D63  5D                pop bp
00002D64  C20400            ret 0x4

; ---- 0000:2D67 ----
00002D67  55                push bp
00002D68  8BEC              mov bp,sp
00002D6A  FF7608            push word [bp+0x8]
00002D6D  FF7606            push word [bp+0x6]
00002D70  9A57290000        call 0x0:0x2957
00002D75  59                pop cx
00002D76  59                pop cx
00002D77  0BC0              or ax,ax
00002D79  7405              jz 0x2d80
00002D7B  B8FFFF            mov ax,0xffff
00002D7E  EB63              jmp short 0x2de3
00002D80  837E0E01          cmp word [bp+0xe],byte +0x1
00002D84  7517              jnz 0x2d9d
00002D86  C45E06            les bx,[bp+0x6]
00002D89  26833F00          cmp word [es:bx],byte +0x0
00002D8D  7E0E              jng 0x2d9d
00002D8F  FF7608            push word [bp+0x8]
00002D92  53                push bx
00002D93  E84DFF            call 0x2ce3
00002D96  99                cwd
00002D97  29460A            sub [bp+0xa],ax
00002D9A  19560C            sbb [bp+0xc],dx
00002D9D  C45E06            les bx,[bp+0x6]
00002DA0  268167025FFE      and word [es:bx+0x2],0xfe5f
00002DA6  26C7070000        mov word [es:bx],0x0
00002DAB  268B470A          mov ax,[es:bx+0xa]
00002DAF  268B5708          mov dx,[es:bx+0x8]
00002DB3  2689570C          mov [es:bx+0xc],dx
00002DB7  2689470E          mov [es:bx+0xe],ax
00002DBB  FF760E            push word [bp+0xe]
00002DBE  FF760C            push word [bp+0xc]
00002DC1  FF760A            push word [bp+0xa]
00002DC4  268A4704          mov al,[es:bx+0x4]
00002DC8  98                cbw
00002DC9  50                push ax
00002DCA  9A3F3E0000        call 0x0:0x3e3f
00002DCF  83C408            add sp,byte +0x8
00002DD2  83FAFF            cmp dx,byte -0x1
00002DD5  750A              jnz 0x2de1
00002DD7  3DFFFF            cmp ax,0xffff
00002DDA  7505              jnz 0x2de1
00002DDC  B8FFFF            mov ax,0xffff
00002DDF  EB02              jmp short 0x2de3
00002DE1  33C0              xor ax,ax
00002DE3  5D                pop bp
00002DE4  CB                retf
00002DE5  55                push bp
00002DE6  8BEC              mov bp,sp
00002DE8  83EC04            sub sp,byte +0x4
00002DEB  C45E06            les bx,[bp+0x6]
00002DEE  268A4704          mov al,[es:bx+0x4]
00002DF2  98                cbw
00002DF3  50                push ax
00002DF4  9A5D340000        call 0x0:0x345d
00002DF9  59                pop cx
00002DFA  8946FC            mov [bp-0x4],ax
00002DFD  8956FE            mov [bp-0x2],dx
00002E00  C45E06            les bx,[bp+0x6]
00002E03  26833F00          cmp word [es:bx],byte +0x0
00002E07  7D10              jnl 0x2e19
00002E09  FF7608            push word [bp+0x8]
00002E0C  53                push bx
00002E0D  E8D3FE            call 0x2ce3
00002E10  99                cwd
00002E11  0146FC            add [bp-0x4],ax
00002E14  1156FE            adc [bp-0x2],dx
00002E17  EB10              jmp short 0x2e29
00002E19  FF7608            push word [bp+0x8]
00002E1C  FF7606            push word [bp+0x6]
00002E1F  E8C1FE            call 0x2ce3
00002E22  99                cwd
00002E23  2946FC            sub [bp-0x4],ax
00002E26  1956FE            sbb [bp-0x2],dx
00002E29  8B56FE            mov dx,[bp-0x2]
00002E2C  8B46FC            mov ax,[bp-0x4]
00002E2F  8BE5              mov sp,bp
00002E31  5D                pop bp
00002E32  CB                retf

; ---- 0000:2E33 ----
00002E33  55                push bp
00002E34  8BEC              mov bp,sp
00002E36  B42A              mov ah,0x2a
00002E38  CD21              int 0x21
00002E3A  C45E06            les bx,[bp+0x6]
00002E3D  26890F            mov [es:bx],cx
00002E40  26895702          mov [es:bx+0x2],dx
00002E44  5D                pop bp
00002E45  CB                retf

; ---- 0000:2E46 ----
00002E46  55                push bp
00002E47  8BEC              mov bp,sp
00002E49  B42C              mov ah,0x2c
00002E4B  CD21              int 0x21
00002E4D  C45E06            les bx,[bp+0x6]
00002E50  26890F            mov [es:bx],cx
00002E53  26895702          mov [es:bx+0x2],dx
00002E57  5D                pop bp
00002E58  CB                retf

; ---- 0000:2E59 ----
00002E59  55                push bp
00002E5A  8BEC              mov bp,sp
00002E5C  B80B30            mov ax,0x300b
00002E5F  50                push ax
00002E60  1E                push ds
00002E61  B8A812            mov ax,0x12a8
00002E64  50                push ax
00002E65  FF7608            push word [bp+0x8]
00002E68  FF7606            push word [bp+0x6]
00002E6B  8D460A            lea ax,[bp+0xa]
00002E6E  50                push ax
00002E6F  E8BC17            call 0x462e
00002E72  5D                pop bp
00002E73  CB                retf

; ---- 0000:2E74 ----
00002E74  55                push bp
00002E75  8BEC              mov bp,sp
00002E77  C45E08            les bx,[bp+0x8]
00002E7A  268B07            mov ax,[es:bx]
00002E7D  48                dec ax
00002E7E  268907            mov [es:bx],ax
00002E81  FF760A            push word [bp+0xa]
00002E84  53                push bx
00002E85  8A4606            mov al,[bp+0x6]
00002E88  98                cbw
00002E89  50                push ax
00002E8A  9A942E0000        call 0x0:0x2e94
00002E8F  83C406            add sp,byte +0x6
00002E92  5D                pop bp
00002E93  CB                retf

; ---- 0000:2E94 ----
00002E94  55                push bp
00002E95  8BEC              mov bp,sp
00002E97  56                push si
00002E98  8A4606            mov al,[bp+0x6]
00002E9B  A27C58            mov [0x587c],al
00002E9E  C45E08            les bx,[bp+0x8]
00002EA1  26833FFF          cmp word [es:bx],byte -0x1
00002EA5  7D4F              jnl 0x2ef6
00002EA7  268B07            mov ax,[es:bx]
00002EAA  40                inc ax
00002EAB  268907            mov [es:bx],ax
00002EAE  268B470E          mov ax,[es:bx+0xe]
00002EB2  268B770C          mov si,[es:bx+0xc]
00002EB6  26FF470C          inc word [es:bx+0xc]
00002EBA  8A167C58          mov dl,[0x587c]
00002EBE  8EC0              mov es,ax
00002EC0  268814            mov [es:si],dl
00002EC3  8E460A            mov es,[bp+0xa]
00002EC6  26F747020800      test word [es:bx+0x2],0x8
00002ECC  7425              jz 0x2ef3
00002ECE  803E7C580A        cmp byte [0x587c],0xa
00002ED3  7407              jz 0x2edc
00002ED5  803E7C580D        cmp byte [0x587c],0xd
00002EDA  7517              jnz 0x2ef3
00002EDC  FF760A            push word [bp+0xa]
00002EDF  FF7608            push word [bp+0x8]
00002EE2  9A57290000        call 0x0:0x2957
00002EE7  59                pop cx
00002EE8  59                pop cx
00002EE9  0BC0              or ax,ax
00002EEB  7406              jz 0x2ef3
00002EED  B8FFFF            mov ax,0xffff
00002EF0  E9FE00            jmp 0x2ff1
00002EF3  E9F600            jmp 0x2fec
00002EF6  C45E08            les bx,[bp+0x8]
00002EF9  26F747029000      test word [es:bx+0x2],0x90
00002EFF  7508              jnz 0x2f09
00002F01  26F747020200      test word [es:bx+0x2],0x2
00002F07  750B              jnz 0x2f14
00002F09  C45E08            les bx,[bp+0x8]
00002F0C  26814F021000      or word [es:bx+0x2],0x10
00002F12  EBD9              jmp short 0x2eed
00002F14  C45E08            les bx,[bp+0x8]
00002F17  26814F020001      or word [es:bx+0x2],0x100
00002F1D  26837F0600        cmp word [es:bx+0x6],byte +0x0
00002F22  7467              jz 0x2f8b
00002F24  26833F00          cmp word [es:bx],byte +0x0
00002F28  7411              jz 0x2f3b
00002F2A  FF760A            push word [bp+0xa]
00002F2D  53                push bx
00002F2E  9A57290000        call 0x0:0x2957
00002F33  59                pop cx
00002F34  59                pop cx
00002F35  0BC0              or ax,ax
00002F37  7402              jz 0x2f3b
00002F39  EBB2              jmp short 0x2eed
00002F3B  C45E08            les bx,[bp+0x8]
00002F3E  268B4706          mov ax,[es:bx+0x6]
00002F42  F7D8              neg ax
00002F44  268907            mov [es:bx],ax
00002F47  268B470E          mov ax,[es:bx+0xe]
00002F4B  268B770C          mov si,[es:bx+0xc]
00002F4F  26FF470C          inc word [es:bx+0xc]
00002F53  8A167C58          mov dl,[0x587c]
00002F57  8EC0              mov es,ax
00002F59  268814            mov [es:si],dl
00002F5C  8E460A            mov es,[bp+0xa]
00002F5F  26F747020800      test word [es:bx+0x2],0x8
00002F65  7422              jz 0x2f89
00002F67  803E7C580A        cmp byte [0x587c],0xa
00002F6C  7407              jz 0x2f75
00002F6E  803E7C580D        cmp byte [0x587c],0xd
00002F73  7514              jnz 0x2f89
00002F75  FF760A            push word [bp+0xa]
00002F78  FF7608            push word [bp+0x8]
00002F7B  9A57290000        call 0x0:0x2957
00002F80  59                pop cx
00002F81  59                pop cx
00002F82  0BC0              or ax,ax
00002F84  7403              jz 0x2f89
00002F86  E964FF            jmp 0x2eed
00002F89  EB61              jmp short 0x2fec
00002F8B  803E7C580A        cmp byte [0x587c],0xa
00002F90  7527              jnz 0x2fb9
00002F92  C45E08            les bx,[bp+0x8]
00002F95  26F747024000      test word [es:bx+0x2],0x40
00002F9B  751C              jnz 0x2fb9
00002F9D  B80100            mov ax,0x1
00002FA0  50                push ax
00002FA1  1E                push ds
00002FA2  B85014            mov ax,0x1450
00002FA5  50                push ax
00002FA6  268A4704          mov al,[es:bx+0x4]
00002FAA  98                cbw
00002FAB  50                push ax
00002FAC  9A034B0000        call 0x0:0x4b03
00002FB1  83C408            add sp,byte +0x8
00002FB4  3D0100            cmp ax,0x1
00002FB7  751F              jnz 0x2fd8
00002FB9  B80100            mov ax,0x1
00002FBC  50                push ax
00002FBD  1E                push ds
00002FBE  B87C58            mov ax,0x587c
00002FC1  50                push ax
00002FC2  C45E08            les bx,[bp+0x8]
00002FC5  268A4704          mov al,[es:bx+0x4]
00002FC9  98                cbw
00002FCA  50                push ax
00002FCB  9A034B0000        call 0x0:0x4b03
00002FD0  83C408            add sp,byte +0x8
00002FD3  3D0100            cmp ax,0x1
00002FD6  7414              jz 0x2fec
00002FD8  C45E08            les bx,[bp+0x8]
00002FDB  26F747020002      test word [es:bx+0x2],0x200
00002FE1  7509              jnz 0x2fec
00002FE3  26814F021000      or word [es:bx+0x2],0x10
00002FE9  E901FF            jmp 0x2eed
00002FEC  A07C58            mov al,[0x587c]
00002FEF  B400              mov ah,0x0
00002FF1  5E                pop si
00002FF2  5D                pop bp
00002FF3  CB                retf
00002FF4  55                push bp
00002FF5  8BEC              mov bp,sp
00002FF7  56                push si
00002FF8  8B7606            mov si,[bp+0x6]
00002FFB  1E                push ds
00002FFC  B8A812            mov ax,0x12a8
00002FFF  50                push ax
00003000  56                push si
00003001  0E                push cs
00003002  E88FFE            call 0x2e94
00003005  83C406            add sp,byte +0x6
00003008  5E                pop si
00003009  5D                pop bp
0000300A  CB                retf
0000300B  55                push bp
0000300C  8BEC              mov bp,sp
0000300E  56                push si
0000300F  57                push di
00003010  8B7608            mov si,[bp+0x8]
00003013  8BFE              mov di,si
00003015  C45E04            les bx,[bp+0x4]
00003018  26F747020800      test word [es:bx+0x2],0x8
0000301E  742E              jz 0x304e
00003020  EB22              jmp short 0x3044
00003022  FF7606            push word [bp+0x6]
00003025  FF7604            push word [bp+0x4]
00003028  C45E0A            les bx,[bp+0xa]
0000302B  FF460A            inc word [bp+0xa]
0000302E  268A07            mov al,[es:bx]
00003031  98                cbw
00003032  50                push ax
00003033  0E                push cs
00003034  E85DFE            call 0x2e94
00003037  83C406            add sp,byte +0x6
0000303A  3DFFFF            cmp ax,0xffff
0000303D  7505              jnz 0x3044
0000303F  33C0              xor ax,ax
00003041  E96701            jmp 0x31ab
00003044  8BC6              mov ax,si
00003046  4E                dec si
00003047  0BC0              or ax,ax
00003049  75D7              jnz 0x3022
0000304B  E95B01            jmp 0x31a9
0000304E  C45E04            les bx,[bp+0x4]
00003051  26F747024000      test word [es:bx+0x2],0x40
00003057  7503              jnz 0x305c
00003059  E9C600            jmp 0x3122
0000305C  26837F0600        cmp word [es:bx+0x6],byte +0x0
00003061  7503              jnz 0x3066
00003063  E99A00            jmp 0x3100
00003066  26397706          cmp [es:bx+0x6],si
0000306A  7338              jnc 0x30a4
0000306C  26833F00          cmp word [es:bx],byte +0x0
00003070  7411              jz 0x3083
00003072  FF7606            push word [bp+0x6]
00003075  53                push bx
00003076  9A57290000        call 0x0:0x2957
0000307B  59                pop cx
0000307C  59                pop cx
0000307D  0BC0              or ax,ax
0000307F  7402              jz 0x3083
00003081  EBBC              jmp short 0x303f
00003083  56                push si
00003084  FF760C            push word [bp+0xc]
00003087  FF760A            push word [bp+0xa]
0000308A  C45E04            les bx,[bp+0x4]
0000308D  268A4704          mov al,[es:bx+0x4]
00003091  98                cbw
00003092  50                push ax
00003093  9A034B0000        call 0x0:0x4b03
00003098  83C408            add sp,byte +0x8
0000309B  3BC6              cmp ax,si
0000309D  7302              jnc 0x30a1
0000309F  EB9E              jmp short 0x303f
000030A1  E90501            jmp 0x31a9
000030A4  C45E04            les bx,[bp+0x4]
000030A7  268B07            mov ax,[es:bx]
000030AA  03C6              add ax,si
000030AC  7C26              jl 0x30d4
000030AE  26833F00          cmp word [es:bx],byte +0x0
000030B2  750C              jnz 0x30c0
000030B4  B8FFFF            mov ax,0xffff
000030B7  262B4706          sub ax,[es:bx+0x6]
000030BB  268907            mov [es:bx],ax
000030BE  EB14              jmp short 0x30d4
000030C0  FF7606            push word [bp+0x6]
000030C3  FF7604            push word [bp+0x4]
000030C6  9A57290000        call 0x0:0x2957
000030CB  59                pop cx
000030CC  59                pop cx
000030CD  0BC0              or ax,ax
000030CF  7403              jz 0x30d4
000030D1  E96BFF            jmp 0x303f
000030D4  56                push si
000030D5  FF760C            push word [bp+0xc]
000030D8  FF760A            push word [bp+0xa]
000030DB  C45E04            les bx,[bp+0x4]
000030DE  26FF770E          push word [es:bx+0xe]
000030E2  26FF770C          push word [es:bx+0xc]
000030E6  9A593F0000        call 0x0:0x3f59
000030EB  83C40A            add sp,byte +0xa
000030EE  C45E04            les bx,[bp+0x4]
000030F1  268B07            mov ax,[es:bx]
000030F4  03C6              add ax,si
000030F6  268907            mov [es:bx],ax
000030F9  2601770C          add [es:bx+0xc],si
000030FD  E9A900            jmp 0x31a9
00003100  56                push si
00003101  FF760C            push word [bp+0xc]
00003104  FF760A            push word [bp+0xa]
00003107  C45E04            les bx,[bp+0x4]
0000310A  268A4704          mov al,[es:bx+0x4]
0000310E  98                cbw
0000310F  50                push ax
00003110  9A034B0000        call 0x0:0x4b03
00003115  83C408            add sp,byte +0x8
00003118  3BC6              cmp ax,si
0000311A  7303              jnc 0x311f
0000311C  E920FF            jmp 0x303f
0000311F  E98700            jmp 0x31a9
00003122  C45E04            les bx,[bp+0x4]
00003125  26837F0600        cmp word [es:bx+0x6],byte +0x0
0000312A  745E              jz 0x318a
0000312C  EB53              jmp short 0x3181
0000312E  C45E04            les bx,[bp+0x4]
00003131  268B07            mov ax,[es:bx]
00003134  40                inc ax
00003135  268907            mov [es:bx],ax
00003138  0BC0              or ax,ax
0000313A  7D26              jnl 0x3162
0000313C  268B470E          mov ax,[es:bx+0xe]
00003140  50                push ax
00003141  53                push bx
00003142  268B5F0C          mov bx,[es:bx+0xc]
00003146  8BC3              mov ax,bx
00003148  5B                pop bx
00003149  26FF470C          inc word [es:bx+0xc]
0000314D  C45E0A            les bx,[bp+0xa]
00003150  FF460A            inc word [bp+0xa]
00003153  268A17            mov dl,[es:bx]
00003156  8BD8              mov bx,ax
00003158  07                pop es
00003159  268817            mov [es:bx],dl
0000315C  8AC2              mov al,dl
0000315E  B400              mov ah,0x0
00003160  EB17              jmp short 0x3179
00003162  FF7606            push word [bp+0x6]
00003165  FF7604            push word [bp+0x4]
00003168  C45E0A            les bx,[bp+0xa]
0000316B  FF460A            inc word [bp+0xa]
0000316E  268A07            mov al,[es:bx]
00003171  50                push ax
00003172  0E                push cs
00003173  E8FEFC            call 0x2e74
00003176  83C406            add sp,byte +0x6
00003179  3DFFFF            cmp ax,0xffff
0000317C  7503              jnz 0x3181
0000317E  E9BEFE            jmp 0x303f
00003181  8BC6              mov ax,si
00003183  4E                dec si
00003184  0BC0              or ax,ax
00003186  75A6              jnz 0x312e
00003188  EB1F              jmp short 0x31a9
0000318A  56                push si
0000318B  FF760C            push word [bp+0xc]
0000318E  FF760A            push word [bp+0xa]
00003191  C45E04            les bx,[bp+0x4]
00003194  268A4704          mov al,[es:bx+0x4]
00003198  98                cbw
00003199  50                push ax
0000319A  9A4F380000        call 0x0:0x384f
0000319F  83C408            add sp,byte +0x8
000031A2  3BC6              cmp ax,si
000031A4  7303              jnc 0x31a9
000031A6  E996FE            jmp 0x303f
000031A9  8BC7              mov ax,di
000031AB  5F                pop di
000031AC  5E                pop si
000031AD  5D                pop bp
000031AE  C20A00            ret 0xa
000031B1  A09812            mov al,[0x1298]
000031B4  98                cbw
000031B5  50                push ax
000031B6  9A2E3E0000        call 0x0:0x3e2e
000031BB  59                pop cx
000031BC  0BC0              or ax,ax
000031BE  7506              jnz 0x31c6
000031C0  81269612FFFD      and word [0x1296],0xfdff
000031C6  B80002            mov ax,0x200
000031C9  50                push ax
000031CA  85069612          test [0x1296],ax
000031CE  7405              jz 0x31d5
000031D0  B80100            mov ax,0x1
000031D3  EB02              jmp short 0x31d7
000031D5  33C0              xor ax,ax
000031D7  50                push ax
000031D8  33C0              xor ax,ax
000031DA  33D2              xor dx,dx
000031DC  50                push ax
000031DD  52                push dx
000031DE  1E                push ds
000031DF  B89412            mov ax,0x1294
000031E2  50                push ax
000031E3  9A26320000        call 0x0:0x3226
000031E8  83C40C            add sp,byte +0xc
000031EB  A0AC12            mov al,[0x12ac]
000031EE  98                cbw
000031EF  50                push ax
000031F0  9A2E3E0000        call 0x0:0x3e2e
000031F5  59                pop cx
000031F6  0BC0              or ax,ax
000031F8  7506              jnz 0x3200
000031FA  8126AA12FFFD      and word [0x12aa],0xfdff
00003200  B80002            mov ax,0x200
00003203  50                push ax
00003204  8506AA12          test [0x12aa],ax
00003208  7405              jz 0x320f
0000320A  B80200            mov ax,0x2
0000320D  EB02              jmp short 0x3211
0000320F  33C0              xor ax,ax
00003211  50                push ax
00003212  33C0              xor ax,ax
00003214  33D2              xor dx,dx
00003216  50                push ax
00003217  52                push dx
00003218  1E                push ds
00003219  B8A812            mov ax,0x12a8
0000321C  50                push ax
0000321D  9A26320000        call 0x0:0x3226
00003222  83C40C            add sp,byte +0xc
00003225  C3                ret

; ---- 0000:3226 ----
00003226  55                push bp
00003227  8BEC              mov bp,sp
00003229  56                push si
0000322A  57                push di
0000322B  8B7E0E            mov di,[bp+0xe]
0000322E  8B7610            mov si,[bp+0x10]
00003231  C45E06            les bx,[bp+0x6]
00003234  268B4712          mov ax,[es:bx+0x12]
00003238  3BC3              cmp ax,bx
0000323A  750B              jnz 0x3247
0000323C  83FF02            cmp di,byte +0x2
0000323F  7F06              jg 0x3247
00003241  81FEFF7F          cmp si,0x7fff
00003245  7606              jna 0x324d
00003247  B8FFFF            mov ax,0xffff
0000324A  E9F100            jmp 0x333e
0000324D  833E541400        cmp word [0x1454],byte +0x0
00003252  750F              jnz 0x3263
00003254  817E06A812        cmp word [bp+0x6],0x12a8
00003259  7508              jnz 0x3263
0000325B  C70654140100      mov word [0x1454],0x1
00003261  EB14              jmp short 0x3277
00003263  833E521400        cmp word [0x1452],byte +0x0
00003268  750D              jnz 0x3277
0000326A  817E069412        cmp word [bp+0x6],0x1294
0000326F  7506              jnz 0x3277
00003271  C70652140100      mov word [0x1452],0x1
00003277  C45E06            les bx,[bp+0x6]
0000327A  26833F00          cmp word [es:bx],byte +0x0
0000327E  7416              jz 0x3296
00003280  B80100            mov ax,0x1
00003283  50                push ax
00003284  33C0              xor ax,ax
00003286  33D2              xor dx,dx
00003288  50                push ax
00003289  52                push dx
0000328A  FF7608            push word [bp+0x8]
0000328D  53                push bx
0000328E  9A672D0000        call 0x0:0x2d67
00003293  83C40A            add sp,byte +0xa
00003296  C45E06            les bx,[bp+0x6]
00003299  26F747020400      test word [es:bx+0x2],0x4
0000329F  740F              jz 0x32b0
000032A1  26FF770A          push word [es:bx+0xa]
000032A5  26FF7708          push word [es:bx+0x8]
000032A9  9A8E4C0000        call 0x0:0x4c8e
000032AE  59                pop cx
000032AF  59                pop cx
000032B0  C45E06            les bx,[bp+0x6]
000032B3  26816702F3FF      and word [es:bx+0x2],0xfff3
000032B9  26C747060000      mov word [es:bx+0x6],0x0
000032BF  8B4608            mov ax,[bp+0x8]
000032C2  8B5606            mov dx,[bp+0x6]
000032C5  83C205            add dx,byte +0x5
000032C8  26895708          mov [es:bx+0x8],dx
000032CC  2689470A          mov [es:bx+0xa],ax
000032D0  2689570C          mov [es:bx+0xc],dx
000032D4  2689470E          mov [es:bx+0xe],ax
000032D8  83FF02            cmp di,byte +0x2
000032DB  745F              jz 0x333c
000032DD  0BF6              or si,si
000032DF  765B              jna 0x333c
000032E1  C7068612B439      mov word [0x1286],0x39b4
000032E7  C70688120000      mov word [0x1288],0x0
000032ED  8B460A            mov ax,[bp+0xa]
000032F0  0B460C            or ax,[bp+0xc]
000032F3  751F              jnz 0x3314
000032F5  56                push si
000032F6  9A744D0000        call 0x0:0x4d74
000032FB  59                pop cx
000032FC  89460A            mov [bp+0xa],ax
000032FF  89560C            mov [bp+0xc],dx
00003302  0BC2              or ax,dx
00003304  740B              jz 0x3311
00003306  C45E06            les bx,[bp+0x6]
00003309  26814F020400      or word [es:bx+0x2],0x4
0000330F  EB03              jmp short 0x3314
00003311  E933FF            jmp 0x3247
00003314  C45E06            les bx,[bp+0x6]
00003317  8B460C            mov ax,[bp+0xc]
0000331A  8B560A            mov dx,[bp+0xa]
0000331D  2689570C          mov [es:bx+0xc],dx
00003321  2689470E          mov [es:bx+0xe],ax
00003325  26895708          mov [es:bx+0x8],dx
00003329  2689470A          mov [es:bx+0xa],ax
0000332D  26897706          mov [es:bx+0x6],si
00003331  83FF01            cmp di,byte +0x1
00003334  7506              jnz 0x333c
00003336  26814F020800      or word [es:bx+0x2],0x8
0000333C  33C0              xor ax,ax
0000333E  5F                pop di
0000333F  5E                pop si
00003340  5D                pop bp
00003341  CB                retf
00003342  55                push bp
00003343  8BEC              mov bp,sp
00003345  56                push si
00003346  8B7608            mov si,[bp+0x8]
00003349  56                push si
0000334A  FF760C            push word [bp+0xc]
0000334D  FF760A            push word [bp+0xa]
00003350  C45E04            les bx,[bp+0x4]
00003353  26FF7702          push word [es:bx+0x2]
00003357  26FF37            push word [es:bx]
0000335A  9A593F0000        call 0x0:0x3f59
0000335F  83C40A            add sp,byte +0xa
00003362  C45E04            les bx,[bp+0x4]
00003365  260137            add [es:bx],si
00003368  53                push bx
00003369  06                push es
0000336A  268E4702          mov es,[es:bx+0x2]
0000336E  8CC0              mov ax,es
00003370  07                pop es
00003371  5B                pop bx
00003372  268B1F            mov bx,[es:bx]
00003375  8EC0              mov es,ax
00003377  26C60700          mov byte [es:bx],0x0
0000337B  8BC6              mov ax,si
0000337D  5E                pop si
0000337E  5D                pop bp
0000337F  C20A00            ret 0xa

; ---- 0000:3382 ----
00003382  55                push bp
00003383  8BEC              mov bp,sp
00003385  C45E06            les bx,[bp+0x6]
00003388  26C60700          mov byte [es:bx],0x0
0000338C  B84233            mov ax,0x3342
0000338F  50                push ax
00003390  16                push ss
00003391  8D4606            lea ax,[bp+0x6]
00003394  50                push ax
00003395  FF760C            push word [bp+0xc]
00003398  FF760A            push word [bp+0xa]
0000339B  8D460E            lea ax,[bp+0xe]
0000339E  50                push ax
0000339F  E88C12            call 0x462e
000033A2  5D                pop bp
000033A3  CB                retf
000033A4  55                push bp
000033A5  8BEC              mov bp,sp
000033A7  C45E06            les bx,[bp+0x6]
000033AA  26C60700          mov byte [es:bx],0x0
000033AE  B84233            mov ax,0x3342
000033B1  50                push ax
000033B2  16                push ss
000033B3  8D4606            lea ax,[bp+0x6]
000033B6  50                push ax
000033B7  FF760C            push word [bp+0xc]
000033BA  FF760A            push word [bp+0xa]
000033BD  FF760E            push word [bp+0xe]
000033C0  E86B12            call 0x462e
000033C3  5D                pop bp
000033C4  CB                retf
000033C5  55                push bp
000033C6  8BEC              mov bp,sp
000033C8  83EC08            sub sp,byte +0x8
000033CB  16                push ss
000033CC  8D46F8            lea ax,[bp-0x8]
000033CF  50                push ax
000033D0  16                push ss
000033D1  8D46FC            lea ax,[bp-0x4]
000033D4  50                push ax
000033D5  C45E06            les bx,[bp+0x6]
000033D8  26FF7702          push word [es:bx+0x2]
000033DC  26FF37            push word [es:bx]
000033DF  9AA4350000        call 0x0:0x35a4
000033E4  83C40C            add sp,byte +0xc
000033E7  16                push ss
000033E8  8D46FC            lea ax,[bp-0x4]
000033EB  50                push ax
000033EC  9ADF410000        call 0x0:0x41df
000033F1  59                pop cx
000033F2  59                pop cx
000033F3  16                push ss
000033F4  8D46F8            lea ax,[bp-0x8]
000033F7  50                push ax
000033F8  9AF4410000        call 0x0:0x41f4
000033FD  59                pop cx
000033FE  59                pop cx
000033FF  33C0              xor ax,ax
00003401  8BE5              mov sp,bp
00003403  5D                pop bp
00003404  CB                retf

; ---- 0000:3405 ----
00003405  55                push bp
00003406  8BEC              mov bp,sp
00003408  83EC0C            sub sp,byte +0xc
0000340B  16                push ss
0000340C  8D46FC            lea ax,[bp-0x4]
0000340F  50                push ax
00003410  9A332E0000        call 0x0:0x2e33
00003415  59                pop cx
00003416  59                pop cx
00003417  16                push ss
00003418  8D46F8            lea ax,[bp-0x8]
0000341B  50                push ax
0000341C  9A462E0000        call 0x0:0x2e46
00003421  59                pop cx
00003422  59                pop cx
00003423  16                push ss
00003424  8D46F8            lea ax,[bp-0x8]
00003427  50                push ax
00003428  16                push ss
00003429  8D46FC            lea ax,[bp-0x4]
0000342C  50                push ax
0000342D  9A77340000        call 0x0:0x3477
00003432  83C408            add sp,byte +0x8
00003435  8946F4            mov [bp-0xc],ax
00003438  8956F6            mov [bp-0xa],dx
0000343B  8B4606            mov ax,[bp+0x6]
0000343E  0B4608            or ax,[bp+0x8]
00003441  7410              jz 0x3453
00003443  C45E06            les bx,[bp+0x6]
00003446  8B46F6            mov ax,[bp-0xa]
00003449  8B56F4            mov dx,[bp-0xc]
0000344C  268917            mov [es:bx],dx
0000344F  26894702          mov [es:bx+0x2],ax
00003453  8B56F6            mov dx,[bp-0xa]
00003456  8B46F4            mov ax,[bp-0xc]
00003459  8BE5              mov sp,bp
0000345B  5D                pop bp
0000345C  CB                retf

; ---- 0000:345D ----
0000345D  55                push bp
0000345E  8BEC              mov bp,sp
00003460  B80100            mov ax,0x1
00003463  50                push ax
00003464  33C0              xor ax,ax
00003466  33D2              xor dx,dx
00003468  50                push ax
00003469  52                push dx
0000346A  FF7606            push word [bp+0x6]
0000346D  9A3F3E0000        call 0x0:0x3e3f
00003472  83C408            add sp,byte +0x8
00003475  5D                pop bp
00003476  CB                retf

; ---- 0000:3477 ----
00003477  55                push bp
00003478  8BEC              mov bp,sp
0000347A  83EC06            sub sp,byte +0x6
0000347D  56                push si
0000347E  57                push di
0000347F  9A35430000        call 0x0:0x4335
00003484  A1EE14            mov ax,[0x14ee]
00003487  8B16EC14          mov dx,[0x14ec]
0000348B  81C200A6          add dx,0xa600
0000348F  15CE12            adc ax,0x12ce
00003492  8956FC            mov [bp-0x4],dx
00003495  8946FE            mov [bp-0x2],ax
00003498  C45E06            les bx,[bp+0x6]
0000349B  268B07            mov ax,[es:bx]
0000349E  0544F8            add ax,0xf844
000034A1  8BF0              mov si,ax
000034A3  D1F8              sar ax,1
000034A5  D1F8              sar ax,1
000034A7  99                cwd
000034A8  50                push ax
000034A9  52                push dx
000034AA  BA8607            mov dx,0x786
000034AD  B8801F            mov ax,0x1f80
000034B0  59                pop cx
000034B1  5B                pop bx
000034B2  E8F01C            call 0x51a5
000034B5  0146FC            add [bp-0x4],ax
000034B8  1156FE            adc [bp-0x2],dx
000034BB  8BC6              mov ax,si
000034BD  250300            and ax,0x3
000034C0  99                cwd
000034C1  50                push ax
000034C2  52                push dx
000034C3  BAE101            mov dx,0x1e1
000034C6  B88033            mov ax,0x3380
000034C9  59                pop cx
000034CA  5B                pop bx
000034CB  E8D71C            call 0x51a5
000034CE  0146FC            add [bp-0x4],ax
000034D1  1156FE            adc [bp-0x2],dx
000034D4  F7C60300          test si,0x3
000034D8  7409              jz 0x34e3
000034DA  8146FC8051        add word [bp-0x4],0x5180
000034DF  8356FE01          adc word [bp-0x2],byte +0x1
000034E3  33FF              xor di,di
000034E5  C45E06            les bx,[bp+0x6]
000034E8  268A4703          mov al,[es:bx+0x3]
000034EC  98                cbw
000034ED  48                dec ax
000034EE  8BF0              mov si,ax
000034F0  EB08              jmp short 0x34fa
000034F2  4E                dec si
000034F3  8A845814          mov al,[si+0x1458]
000034F7  98                cbw
000034F8  03F8              add di,ax
000034FA  0BF6              or si,si
000034FC  7FF4              jg 0x34f2
000034FE  C45E06            les bx,[bp+0x6]
00003501  268A4702          mov al,[es:bx+0x2]
00003505  98                cbw
00003506  48                dec ax
00003507  03F8              add di,ax
00003509  26807F0302        cmp byte [es:bx+0x3],0x2
0000350E  7E08              jng 0x3518
00003510  26F7070300        test word [es:bx],0x3
00003515  7501              jnz 0x3518
00003517  47                inc di
00003518  8BC7              mov ax,di
0000351A  BA1800            mov dx,0x18
0000351D  F7EA              imul dx
0000351F  C45E0A            les bx,[bp+0xa]
00003522  268A5701          mov dl,[es:bx+0x1]
00003526  B600              mov dh,0x0
00003528  03C2              add ax,dx
0000352A  8946FA            mov [bp-0x6],ax
0000352D  833EF01400        cmp word [0x14f0],byte +0x0
00003532  741F              jz 0x3553
00003534  268A4701          mov al,[es:bx+0x1]
00003538  B400              mov ah,0x0
0000353A  50                push ax
0000353B  57                push di
0000353C  33C0              xor ax,ax
0000353E  50                push ax
0000353F  C45E06            les bx,[bp+0x6]
00003542  268B07            mov ax,[es:bx]
00003545  054EF8            add ax,0xf84e
00003548  50                push ax
00003549  E8D30F            call 0x451f
0000354C  0BC0              or ax,ax
0000354E  7403              jz 0x3553
00003550  FF4EFA            dec word [bp-0x6]
00003553  8B46FA            mov ax,[bp-0x6]
00003556  99                cwd
00003557  50                push ax
00003558  52                push dx
00003559  33D2              xor dx,dx
0000355B  B8100E            mov ax,0xe10
0000355E  59                pop cx
0000355F  5B                pop bx
00003560  E8421C            call 0x51a5
00003563  0146FC            add [bp-0x4],ax
00003566  1156FE            adc [bp-0x2],dx
00003569  C45E0A            les bx,[bp+0xa]
0000356C  268A07            mov al,[es:bx]
0000356F  B400              mov ah,0x0
00003571  99                cwd
00003572  50                push ax
00003573  52                push dx
00003574  33D2              xor dx,dx
00003576  B83C00            mov ax,0x3c
00003579  59                pop cx
0000357A  5B                pop bx
0000357B  E8271C            call 0x51a5
0000357E  C45E0A            les bx,[bp+0xa]
00003581  268A5F03          mov bl,[es:bx+0x3]
00003585  B700              mov bh,0x0
00003587  50                push ax
00003588  8BC3              mov ax,bx
0000358A  52                push dx
0000358B  99                cwd
0000358C  5B                pop bx
0000358D  59                pop cx
0000358E  03C8              add cx,ax
00003590  13DA              adc bx,dx
00003592  014EFC            add [bp-0x4],cx
00003595  115EFE            adc [bp-0x2],bx
00003598  8B56FE            mov dx,[bp-0x2]
0000359B  8B46FC            mov ax,[bp-0x4]
0000359E  5F                pop di
0000359F  5E                pop si
000035A0  8BE5              mov sp,bp
000035A2  5D                pop bp
000035A3  CB                retf

; ---- 0000:35A4 ----
000035A4  55                push bp
000035A5  8BEC              mov bp,sp
000035A7  9A35430000        call 0x0:0x4335
000035AC  A1EE14            mov ax,[0x14ee]
000035AF  8B16EC14          mov dx,[0x14ec]
000035B3  81C200A6          add dx,0xa600
000035B7  15CE12            adc ax,0x12ce
000035BA  295606            sub [bp+0x6],dx
000035BD  194608            sbb [bp+0x8],ax
000035C0  C45E0E            les bx,[bp+0xe]
000035C3  26C6470200        mov byte [es:bx+0x2],0x0
000035C8  33C0              xor ax,ax
000035CA  BA3C00            mov dx,0x3c
000035CD  50                push ax
000035CE  52                push dx
000035CF  FF7608            push word [bp+0x8]
000035D2  FF7606            push word [bp+0x6]
000035D5  E8BD19            call 0x4f95
000035D8  C45E0E            les bx,[bp+0xe]
000035DB  26884703          mov [es:bx+0x3],al
000035DF  33C0              xor ax,ax
000035E1  BA3C00            mov dx,0x3c
000035E4  50                push ax
000035E5  52                push dx
000035E6  FF7608            push word [bp+0x8]
000035E9  FF7606            push word [bp+0x6]
000035EC  E89719            call 0x4f86
000035EF  894606            mov [bp+0x6],ax
000035F2  895608            mov [bp+0x8],dx
000035F5  33C0              xor ax,ax
000035F7  BA3C00            mov dx,0x3c
000035FA  50                push ax
000035FB  52                push dx
000035FC  FF7608            push word [bp+0x8]
000035FF  FF7606            push word [bp+0x6]
00003602  E89019            call 0x4f95
00003605  C45E0E            les bx,[bp+0xe]
00003608  268807            mov [es:bx],al
0000360B  33C0              xor ax,ax
0000360D  BA3C00            mov dx,0x3c
00003610  50                push ax
00003611  52                push dx
00003612  FF7608            push word [bp+0x8]
00003615  FF7606            push word [bp+0x6]
00003618  E86B19            call 0x4f86
0000361B  894606            mov [bp+0x6],ax
0000361E  895608            mov [bp+0x8],dx
00003621  33C0              xor ax,ax
00003623  BAF888            mov dx,0x88f8
00003626  50                push ax
00003627  52                push dx
00003628  FF7608            push word [bp+0x8]
0000362B  FF7606            push word [bp+0x6]
0000362E  E85519            call 0x4f86
00003631  D1E0              shl ax,1
00003633  D1E0              shl ax,1
00003635  05BC07            add ax,0x7bc
00003638  C45E0A            les bx,[bp+0xa]
0000363B  268907            mov [es:bx],ax
0000363E  33C0              xor ax,ax
00003640  BAF888            mov dx,0x88f8
00003643  50                push ax
00003644  52                push dx
00003645  FF7608            push word [bp+0x8]
00003648  FF7606            push word [bp+0x6]
0000364B  E84719            call 0x4f95
0000364E  894606            mov [bp+0x6],ax
00003651  895608            mov [bp+0x8],dx
00003654  0BD2              or dx,dx
00003656  7C44              jl 0x369c
00003658  7F07              jg 0x3661
0000365A  817E065022        cmp word [bp+0x6],0x2250
0000365F  763B              jna 0x369c
00003661  816E065022        sub word [bp+0x6],0x2250
00003666  835E0800          sbb word [bp+0x8],byte +0x0
0000366A  C45E0A            les bx,[bp+0xa]
0000366D  26FF07            inc word [es:bx]
00003670  33C0              xor ax,ax
00003672  BA3822            mov dx,0x2238
00003675  50                push ax
00003676  52                push dx
00003677  FF7608            push word [bp+0x8]
0000367A  FF7606            push word [bp+0x6]
0000367D  E80619            call 0x4f86
00003680  C45E0A            les bx,[bp+0xa]
00003683  260107            add [es:bx],ax
00003686  33C0              xor ax,ax
00003688  BA3822            mov dx,0x2238
0000368B  50                push ax
0000368C  52                push dx
0000368D  FF7608            push word [bp+0x8]
00003690  FF7606            push word [bp+0x6]
00003693  E8FF18            call 0x4f95
00003696  894606            mov [bp+0x6],ax
00003699  895608            mov [bp+0x8],dx
0000369C  833EF01400        cmp word [0x14f0],byte +0x0
000036A1  743E              jz 0x36e1
000036A3  33C0              xor ax,ax
000036A5  BA1800            mov dx,0x18
000036A8  50                push ax
000036A9  52                push dx
000036AA  FF7608            push word [bp+0x8]
000036AD  FF7606            push word [bp+0x6]
000036B0  E8E218            call 0x4f95
000036B3  50                push ax
000036B4  33C0              xor ax,ax
000036B6  BA1800            mov dx,0x18
000036B9  50                push ax
000036BA  52                push dx
000036BB  FF7608            push word [bp+0x8]
000036BE  FF7606            push word [bp+0x6]
000036C1  E8C218            call 0x4f86
000036C4  50                push ax
000036C5  33C0              xor ax,ax
000036C7  50                push ax
000036C8  C45E0A            les bx,[bp+0xa]
000036CB  268B07            mov ax,[es:bx]
000036CE  054EF8            add ax,0xf84e
000036D1  50                push ax
000036D2  E84A0E            call 0x451f
000036D5  0BC0              or ax,ax
000036D7  7408              jz 0x36e1
000036D9  83460601          add word [bp+0x6],byte +0x1
000036DD  83560800          adc word [bp+0x8],byte +0x0
000036E1  33C0              xor ax,ax
000036E3  BA1800            mov dx,0x18
000036E6  50                push ax
000036E7  52                push dx
000036E8  FF7608            push word [bp+0x8]
000036EB  FF7606            push word [bp+0x6]
000036EE  E8A418            call 0x4f95
000036F1  C45E0E            les bx,[bp+0xe]
000036F4  26884701          mov [es:bx+0x1],al
000036F8  33C0              xor ax,ax
000036FA  BA1800            mov dx,0x18
000036FD  50                push ax
000036FE  52                push dx
000036FF  FF7608            push word [bp+0x8]
00003702  FF7606            push word [bp+0x6]
00003705  E87E18            call 0x4f86
00003708  894606            mov [bp+0x6],ax
0000370B  895608            mov [bp+0x8],dx
0000370E  83460601          add word [bp+0x6],byte +0x1
00003712  83560800          adc word [bp+0x8],byte +0x0
00003716  C45E0A            les bx,[bp+0xa]
00003719  26F7070300        test word [es:bx],0x3
0000371E  7533              jnz 0x3753
00003720  837E0800          cmp word [bp+0x8],byte +0x0
00003724  7C12              jl 0x3738
00003726  7F06              jg 0x372e
00003728  837E063C          cmp word [bp+0x6],byte +0x3c
0000372C  760A              jna 0x3738
0000372E  836E0601          sub word [bp+0x6],byte +0x1
00003732  835E0800          sbb word [bp+0x8],byte +0x0
00003736  EB1B              jmp short 0x3753
00003738  837E0800          cmp word [bp+0x8],byte +0x0
0000373C  7515              jnz 0x3753
0000373E  837E063C          cmp word [bp+0x6],byte +0x3c
00003742  750F              jnz 0x3753
00003744  C45E0A            les bx,[bp+0xa]
00003747  26C6470302        mov byte [es:bx+0x3],0x2
0000374C  26C647021D        mov byte [es:bx+0x2],0x1d
00003751  EB51              jmp short 0x37a4
00003753  C45E0A            les bx,[bp+0xa]
00003756  26C6470300        mov byte [es:bx+0x3],0x0
0000375B  EB1D              jmp short 0x377a
0000375D  C45E0A            les bx,[bp+0xa]
00003760  268A4703          mov al,[es:bx+0x3]
00003764  98                cbw
00003765  8BD8              mov bx,ax
00003767  8A875814          mov al,[bx+0x1458]
0000376B  98                cbw
0000376C  99                cwd
0000376D  294606            sub [bp+0x6],ax
00003770  195608            sbb [bp+0x8],dx
00003773  8B5E0A            mov bx,[bp+0xa]
00003776  26FE4703          inc byte [es:bx+0x3]
0000377A  C45E0A            les bx,[bp+0xa]
0000377D  268A4703          mov al,[es:bx+0x3]
00003781  98                cbw
00003782  8BD8              mov bx,ax
00003784  8A875814          mov al,[bx+0x1458]
00003788  98                cbw
00003789  99                cwd
0000378A  3B5608            cmp dx,[bp+0x8]
0000378D  7CCE              jl 0x375d
0000378F  7505              jnz 0x3796
00003791  3B4606            cmp ax,[bp+0x6]
00003794  72C7              jc 0x375d
00003796  C45E0A            les bx,[bp+0xa]
00003799  26FE4703          inc byte [es:bx+0x3]
0000379D  8A4606            mov al,[bp+0x6]
000037A0  26884702          mov [es:bx+0x2],al
000037A4  5D                pop bp
000037A5  CB                retf

; ---- 0000:37A6 ----
000037A6  55                push bp
000037A7  8BEC              mov bp,sp
000037A9  8B4606            mov ax,[bp+0x6]
000037AC  0B4608            or ax,[bp+0x8]
000037AF  7508              jnz 0x37b9
000037B1  C746068058        mov word [bp+0x6],0x5880
000037B6  8C5E08            mov [bp+0x8],ds
000037B9  C45E06            les bx,[bp+0x6]
000037BC  26C60700          mov byte [es:bx],0x0
000037C0  1E                push ds
000037C1  B86414            mov ax,0x1464
000037C4  50                push ax
000037C5  FF7608            push word [bp+0x8]
000037C8  53                push bx
000037C9  9A3C420000        call 0x0:0x423c
000037CE  83C408            add sp,byte +0x8
000037D1  B80A00            mov ax,0xa
000037D4  50                push ax
000037D5  8B4606            mov ax,[bp+0x6]
000037D8  050300            add ax,0x3
000037DB  FF7608            push word [bp+0x8]
000037DE  50                push ax
000037DF  33C0              xor ax,ax
000037E1  50                push ax
000037E2  FF7604            push word [bp+0x4]
000037E5  9A143F0000        call 0x0:0x3f14
000037EA  83C40A            add sp,byte +0xa
000037ED  1E                push ds
000037EE  B86814            mov ax,0x1468
000037F1  50                push ax
000037F2  FF7608            push word [bp+0x8]
000037F5  FF7606            push word [bp+0x6]
000037F8  9A3C420000        call 0x0:0x423c
000037FD  83C408            add sp,byte +0x8
00003800  8B5608            mov dx,[bp+0x8]
00003803  8B4606            mov ax,[bp+0x6]
00003806  5D                pop bp
00003807  C20600            ret 0x6
0000380A  55                push bp
0000380B  8BEC              mov bp,sp
0000380D  FF7608            push word [bp+0x8]
00003810  FF7606            push word [bp+0x6]
00003813  833E7E58FF        cmp word [0x587e],byte -0x1
00003818  7505              jnz 0x381f
0000381A  B80200            mov ax,0x2
0000381D  EB03              jmp short 0x3822
0000381F  B80100            mov ax,0x1
00003822  01067E58          add [0x587e],ax
00003826  A17E58            mov ax,[0x587e]
00003829  50                push ax
0000382A  E879FF            call 0x37a6
0000382D  894606            mov [bp+0x6],ax
00003830  895608            mov [bp+0x8],dx
00003833  33C0              xor ax,ax
00003835  50                push ax
00003836  52                push dx
00003837  FF7606            push word [bp+0x6]
0000383A  9A74260000        call 0x0:0x2674
0000383F  83C406            add sp,byte +0x6
00003842  3DFFFF            cmp ax,0xffff
00003845  75C6              jnz 0x380d
00003847  8B5608            mov dx,[bp+0x8]
0000384A  8B4606            mov ax,[bp+0x6]
0000384D  5D                pop bp
0000384E  CB                retf

; ---- 0000:384F ----
0000384F  55                push bp
00003850  8BEC              mov bp,sp
00003852  81EC8E00          sub sp,0x8e
00003856  56                push si
00003857  57                push di
00003858  8B460C            mov ax,[bp+0xc]
0000385B  40                inc ax
0000385C  3D0200            cmp ax,0x2
0000385F  7305              jnc 0x3866
00003861  33C0              xor ax,ax
00003863  E91001            jmp 0x3976
00003866  8B5E06            mov bx,[bp+0x6]
00003869  D1E3              shl bx,1
0000386B  F78724140080      test word [bx+0x1424],0x8000
00003871  7417              jz 0x388a
00003873  FF760C            push word [bp+0xc]
00003876  FF760A            push word [bp+0xa]
00003879  FF7608            push word [bp+0x8]
0000387C  FF7606            push word [bp+0x6]
0000387F  9A034B0000        call 0x0:0x4b03
00003884  83C408            add sp,byte +0x8
00003887  E9EC00            jmp 0x3976
0000388A  8B5E06            mov bx,[bp+0x6]
0000388D  D1E3              shl bx,1
0000388F  81A72414FFFD      and word [bx+0x1424],0xfdff
00003895  8B460A            mov ax,[bp+0xa]
00003898  8B5608            mov dx,[bp+0x8]
0000389B  8956F4            mov [bp-0xc],dx
0000389E  8946F6            mov [bp-0xa],ax
000038A1  8B460C            mov ax,[bp+0xc]
000038A4  8946FA            mov [bp-0x6],ax
000038A7  EB7A              jmp short 0x3923
000038A9  FF4EFA            dec word [bp-0x6]
000038AC  C45EF4            les bx,[bp-0xc]
000038AF  FF46F4            inc word [bp-0xc]
000038B2  268A07            mov al,[es:bx]
000038B5  8846F9            mov [bp-0x7],al
000038B8  3C0A              cmp al,0xa
000038BA  750A              jnz 0x38c6
000038BC  C45EFC            les bx,[bp-0x4]
000038BF  26C6070D          mov byte [es:bx],0xd
000038C3  FF46FC            inc word [bp-0x4]
000038C6  C45EFC            les bx,[bp-0x4]
000038C9  8A46F9            mov al,[bp-0x7]
000038CC  268807            mov [es:bx],al
000038CF  FF46FC            inc word [bp-0x4]
000038D2  8D8672FF          lea ax,[bp-0x8e]
000038D6  8B56FC            mov dx,[bp-0x4]
000038D9  33DB              xor bx,bx
000038DB  2BD0              sub dx,ax
000038DD  83DB00            sbb bx,byte +0x0
000038E0  0BDB              or bx,bx
000038E2  7C49              jl 0x392d
000038E4  7506              jnz 0x38ec
000038E6  81FA8000          cmp dx,0x80
000038EA  7241              jc 0x392d
000038EC  8D8672FF          lea ax,[bp-0x8e]
000038F0  8B56FC            mov dx,[bp-0x4]
000038F3  33DB              xor bx,bx
000038F5  2BD0              sub dx,ax
000038F7  83DB00            sbb bx,byte +0x0
000038FA  8BF2              mov si,dx
000038FC  52                push dx
000038FD  16                push ss
000038FE  50                push ax
000038FF  FF7606            push word [bp+0x6]
00003902  9A034B0000        call 0x0:0x4b03
00003907  83C408            add sp,byte +0x8
0000390A  8BF8              mov di,ax
0000390C  3BC6              cmp ax,si
0000390E  7413              jz 0x3923
00003910  0BC0              or ax,ax
00003912  7305              jnc 0x3919
00003914  B8FFFF            mov ax,0xffff
00003917  EB58              jmp short 0x3971
00003919  8B460C            mov ax,[bp+0xc]
0000391C  2B46FA            sub ax,[bp-0x6]
0000391F  EB4C              jmp short 0x396d
00003921  EB4E              jmp short 0x3971
00003923  8D8672FF          lea ax,[bp-0x8e]
00003927  8946FC            mov [bp-0x4],ax
0000392A  8C56FE            mov [bp-0x2],ss
0000392D  837EFA00          cmp word [bp-0x6],byte +0x0
00003931  7403              jz 0x3936
00003933  E973FF            jmp 0x38a9
00003936  8D8672FF          lea ax,[bp-0x8e]
0000393A  8B56FC            mov dx,[bp-0x4]
0000393D  33DB              xor bx,bx
0000393F  2BD0              sub dx,ax
00003941  83DB00            sbb bx,byte +0x0
00003944  8BF2              mov si,dx
00003946  8BC2              mov ax,dx
00003948  0BC0              or ax,ax
0000394A  7627              jna 0x3973
0000394C  52                push dx
0000394D  16                push ss
0000394E  8D8672FF          lea ax,[bp-0x8e]
00003952  50                push ax
00003953  FF7606            push word [bp+0x6]
00003956  9A034B0000        call 0x0:0x4b03
0000395B  83C408            add sp,byte +0x8
0000395E  8BF8              mov di,ax
00003960  3BC6              cmp ax,si
00003962  740F              jz 0x3973
00003964  0BC0              or ax,ax
00003966  7302              jnc 0x396a
00003968  EBAA              jmp short 0x3914
0000396A  8B460C            mov ax,[bp+0xc]
0000396D  03C7              add ax,di
0000396F  2BC6              sub ax,si
00003971  EB03              jmp short 0x3976
00003973  8B460C            mov ax,[bp+0xc]
00003976  5F                pop di
00003977  5E                pop si
00003978  8BE5              mov sp,bp
0000397A  5D                pop bp
0000397B  CB                retf
0000397C  55                push bp
0000397D  8BEC              mov bp,sp
0000397F  83EC04            sub sp,byte +0x4
00003982  56                push si
00003983  33F6              xor si,si
00003985  C746FC9412        mov word [bp-0x4],0x1294
0000398A  8C5EFE            mov [bp-0x2],ds
0000398D  EB1B              jmp short 0x39aa
0000398F  C45EFC            les bx,[bp-0x4]
00003992  26F747020300      test word [es:bx+0x2],0x3
00003998  740B              jz 0x39a5
0000399A  FF76FE            push word [bp-0x2]
0000399D  53                push bx
0000399E  9A5B280000        call 0x0:0x285b
000039A3  59                pop cx
000039A4  59                pop cx
000039A5  8346FC14          add word [bp-0x4],byte +0x14
000039A9  46                inc si
000039AA  83FE14            cmp si,byte +0x14
000039AD  7CE0              jl 0x398f
000039AF  5E                pop si
000039B0  8BE5              mov sp,bp
000039B2  5D                pop bp
000039B3  CB                retf
000039B4  55                push bp
000039B5  8BEC              mov bp,sp
000039B7  83EC04            sub sp,byte +0x4
000039BA  56                push si
000039BB  BE0400            mov si,0x4
000039BE  C746FC9412        mov word [bp-0x4],0x1294
000039C3  8C5EFE            mov [bp-0x2],ds
000039C6  EB1B              jmp short 0x39e3
000039C8  C45EFC            les bx,[bp-0x4]
000039CB  26F747020300      test word [es:bx+0x2],0x3
000039D1  740B              jz 0x39de
000039D3  FF76FE            push word [bp-0x2]
000039D6  53                push bx
000039D7  9A57290000        call 0x0:0x2957
000039DC  59                pop cx
000039DD  59                pop cx
000039DE  4E                dec si
000039DF  8346FC14          add word [bp-0x4],byte +0x14
000039E3  0BF6              or si,si
000039E5  75E1              jnz 0x39c8
000039E7  5E                pop si
000039E8  8BE5              mov sp,bp
000039EA  5D                pop bp
000039EB  CB                retf

; ---- 0000:39EC ----
000039EC  55                push bp
000039ED  8BEC              mov bp,sp
000039EF  56                push si
000039F0  57                push di
000039F1  06                push es
000039F2  55                push bp
000039F3  C47606            les si,[bp+0x6]
000039F6  FC                cld
000039F7  2BC0              sub ax,ax
000039F9  99                cwd
000039FA  B90A00            mov cx,0xa
000039FD  B700              mov bh,0x0
000039FF  BF8511            mov di,0x1185
00003A02  268A1C            mov bl,[es:si]
00003A05  46                inc si
00003A06  F60101            test byte [bx+di],0x1
00003A09  75F7              jnz 0x3a02
00003A0B  BD0000            mov bp,0x0
00003A0E  80FB2B            cmp bl,0x2b
00003A11  7406              jz 0x3a19
00003A13  80FB2D            cmp bl,0x2d
00003A16  7505              jnz 0x3a1d
00003A18  45                inc bp
00003A19  268A1C            mov bl,[es:si]
00003A1C  46                inc si
00003A1D  80FB39            cmp bl,0x39
00003A20  772F              ja 0x3a51
00003A22  80EB30            sub bl,0x30
00003A25  722A              jc 0x3a51
00003A27  F7E1              mul cx
00003A29  03C3              add ax,bx
00003A2B  12D6              adc dl,dh
00003A2D  74EA              jz 0x3a19
00003A2F  EB12              jmp short 0x3a43
00003A31  8BFA              mov di,dx
00003A33  B90A00            mov cx,0xa
00003A36  F7E1              mul cx
00003A38  97                xchg ax,di
00003A39  87D1              xchg dx,cx
00003A3B  F7E2              mul dx
00003A3D  92                xchg ax,dx
00003A3E  97                xchg ax,di
00003A3F  03C3              add ax,bx
00003A41  13D1              adc dx,cx
00003A43  268A1C            mov bl,[es:si]
00003A46  46                inc si
00003A47  80FB39            cmp bl,0x39
00003A4A  7705              ja 0x3a51
00003A4C  80EB30            sub bl,0x30
00003A4F  73E0              jnc 0x3a31
00003A51  4D                dec bp
00003A52  7C07              jl 0x3a5b
00003A54  F7DA              neg dx
00003A56  F7D8              neg ax
00003A58  83DA00            sbb dx,byte +0x0
00003A5B  5D                pop bp
00003A5C  07                pop es
00003A5D  5F                pop di
00003A5E  5E                pop si
00003A5F  5D                pop bp
00003A60  CB                retf
00003A61  55                push bp
00003A62  8BEC              mov bp,sp
00003A64  FF7608            push word [bp+0x8]
00003A67  FF7606            push word [bp+0x6]
00003A6A  0E                push cs
00003A6B  E87EFF            call 0x39ec
00003A6E  59                pop cx
00003A6F  59                pop cx
00003A70  5D                pop bp
00003A71  CB                retf

; ---- 0000:3A72 ----
00003A72  55                push bp
00003A73  8BEC              mov bp,sp
00003A75  8A6606            mov ah,[bp+0x6]
00003A78  8A460A            mov al,[bp+0xa]
00003A7B  8B5608            mov dx,[bp+0x8]
00003A7E  CD21              int 0x21
00003A80  5D                pop bp
00003A81  CB                retf

; ---- 0000:3A82 ----
00003A82  55                push bp
00003A83  8BEC              mov bp,sp
00003A85  8A6606            mov ah,[bp+0x6]
00003A88  CD16              int 0x16
00003A8A  740F              jz 0x3a9b
00003A8C  807E0601          cmp byte [bp+0x6],0x1
00003A90  7511              jnz 0x3aa3
00003A92  0BC0              or ax,ax
00003A94  750D              jnz 0x3aa3
00003A96  B8FFFF            mov ax,0xffff
00003A99  EB08              jmp short 0x3aa3
00003A9B  807E0601          cmp byte [bp+0x6],0x1
00003A9F  7502              jnz 0x3aa3
00003AA1  33C0              xor ax,ax
00003AA3  5D                pop bp
00003AA4  CB                retf

; ---- 0000:3AA5 ----
00003AA5  55                push bp
00003AA6  8BEC              mov bp,sp
00003AA8  1E                push ds
00003AA9  8B4E0C            mov cx,[bp+0xc]
00003AAC  B443              mov ah,0x43
00003AAE  8A460A            mov al,[bp+0xa]
00003AB1  C55606            lds dx,[bp+0x6]
00003AB4  CD21              int 0x21
00003AB6  1F                pop ds
00003AB7  7203              jc 0x3abc
00003AB9  91                xchg ax,cx
00003ABA  EB04              jmp short 0x3ac0
00003ABC  50                push ax
00003ABD  E83103            call 0x3df1
00003AC0  5D                pop bp
00003AC1  CB                retf

; ---- 0000:3AC2 ----
00003AC2  55                push bp
00003AC3  8BEC              mov bp,sp
00003AC5  B43E              mov ah,0x3e
00003AC7  8B5E06            mov bx,[bp+0x6]
00003ACA  CD21              int 0x21
00003ACC  720C              jc 0x3ada
00003ACE  D1E3              shl bx,1
00003AD0  C7872414FFFF      mov word [bx+0x1424],0xffff
00003AD6  33C0              xor ax,ax
00003AD8  EB04              jmp short 0x3ade
00003ADA  50                push ax
00003ADB  E81303            call 0x3df1
00003ADE  5D                pop bp
00003ADF  CB                retf

; ---- 0000:3AE0 ----
00003AE0  55                push bp
00003AE1  8BEC              mov bp,sp
00003AE3  83EC04            sub sp,byte +0x4
00003AE6  B80142            mov ax,0x4201
00003AE9  8B5E06            mov bx,[bp+0x6]
00003AEC  33C9              xor cx,cx
00003AEE  33D2              xor dx,dx
00003AF0  CD21              int 0x21
00003AF2  7224              jc 0x3b18
00003AF4  52                push dx
00003AF5  50                push ax
00003AF6  B80242            mov ax,0x4202
00003AF9  33C9              xor cx,cx
00003AFB  33D2              xor dx,dx
00003AFD  CD21              int 0x21
00003AFF  8946FC            mov [bp-0x4],ax
00003B02  8956FE            mov [bp-0x2],dx
00003B05  5A                pop dx
00003B06  59                pop cx
00003B07  720F              jc 0x3b18
00003B09  B80042            mov ax,0x4200
00003B0C  CD21              int 0x21
00003B0E  7208              jc 0x3b18
00003B10  8B56FE            mov dx,[bp-0x2]
00003B13  8B46FC            mov ax,[bp-0x4]
00003B16  EB05              jmp short 0x3b1d
00003B18  50                push ax
00003B19  E8D502            call 0x3df1
00003B1C  99                cwd
00003B1D  8BE5              mov sp,bp
00003B1F  5D                pop bp
00003B20  CB                retf

; ---- 0000:3B21 ----
00003B21  55                push bp
00003B22  8BEC              mov bp,sp
00003B24  83EC04            sub sp,byte +0x4
00003B27  56                push si
00003B28  57                push di
00003B29  C47E06            les di,[bp+0x6]
00003B2C  8CC0              mov ax,es
00003B2E  0BC7              or ax,di
00003B30  7420              jz 0x3b52
00003B32  B000              mov al,0x0
00003B34  268A25            mov ah,[es:di]
00003B37  B9FFFF            mov cx,0xffff
00003B3A  FC                cld
00003B3B  F2AE              repne scasb
00003B3D  F7D1              not cx
00003B3F  49                dec cx
00003B40  7410              jz 0x3b52
00003B42  C43E7615          les di,[0x1576]
00003B46  8C46FE            mov [bp-0x2],es
00003B49  8CC3              mov bx,es
00003B4B  0BDF              or bx,di
00003B4D  897EFC            mov [bp-0x4],di
00003B50  750D              jnz 0x3b5f
00003B52  33D2              xor dx,dx
00003B54  33C0              xor ax,ax
00003B56  EB33              jmp short 0x3b8b
00003B58  8346FC04          add word [bp-0x4],byte +0x4
00003B5C  C47EFC            les di,[bp-0x4]
00003B5F  26C43D            les di,[es:di]
00003B62  8CC3              mov bx,es
00003B64  0BDF              or bx,di
00003B66  74EA              jz 0x3b52
00003B68  268A05            mov al,[es:di]
00003B6B  0AC0              or al,al
00003B6D  74E3              jz 0x3b52
00003B6F  3AE0              cmp ah,al
00003B71  75E5              jnz 0x3b58
00003B73  8BD9              mov bx,cx
00003B75  2680393D          cmp byte [es:bx+di],0x3d
00003B79  75DD              jnz 0x3b58
00003B7B  1E                push ds
00003B7C  C57606            lds si,[bp+0x6]
00003B7F  F3A6              repe cmpsb
00003B81  1F                pop ds
00003B82  87CB              xchg cx,bx
00003B84  75D2              jnz 0x3b58
00003B86  47                inc di
00003B87  8BC7              mov ax,di
00003B89  8CC2              mov dx,es
00003B8B  5F                pop di
00003B8C  5E                pop si
00003B8D  8BE5              mov sp,bp
00003B8F  5D                pop bp
00003B90  CB                retf

; ---- 0000:3B91 ----
00003B91  55                push bp
00003B92  8BEC              mov bp,sp
00003B94  B435              mov ah,0x35
00003B96  8A4606            mov al,[bp+0x6]
00003B99  CD21              int 0x21
00003B9B  93                xchg ax,bx
00003B9C  8CC2              mov dx,es
00003B9E  5D                pop bp
00003B9F  CB                retf

; ---- 0000:3BA0 ----
00003BA0  55                push bp
00003BA1  8BEC              mov bp,sp
00003BA3  B425              mov ah,0x25
00003BA5  8A4606            mov al,[bp+0x6]
00003BA8  1E                push ds
00003BA9  C55608            lds dx,[bp+0x8]
00003BAC  CD21              int 0x21
00003BAE  1F                pop ds
00003BAF  5D                pop bp
00003BB0  CB                retf
00003BB1  50                push ax
00003BB2  53                push bx
00003BB3  51                push cx
00003BB4  52                push dx
00003BB5  06                push es
00003BB6  1E                push ds
00003BB7  56                push si
00003BB8  57                push di
00003BB9  55                push bp
00003BBA  BDDC28            mov bp,0x28dc
00003BBD  8EDD              mov ds,bp
00003BBF  8BEC              mov bp,sp
00003BC1  89269258          mov [0x5892],sp
00003BC5  FF7604            push word [bp+0x4]
00003BC8  FF7600            push word [bp+0x0]
00003BCB  FF7610            push word [bp+0x10]
00003BCE  8B4602            mov ax,[bp+0x2]
00003BD1  25FF00            and ax,0xff
00003BD4  50                push ax
00003BD5  FF1E8E58          call far [0x588e]
00003BD9  83C408            add sp,byte +0x8
00003BDC  894610            mov [bp+0x10],ax
00003BDF  5D                pop bp
00003BE0  5F                pop di
00003BE1  5E                pop si
00003BE2  1F                pop ds
00003BE3  07                pop es
00003BE4  5A                pop dx
00003BE5  59                pop cx
00003BE6  5B                pop bx
00003BE7  58                pop ax
00003BE8  CF                iret

; ---- 0000:3BE9 ----
00003BE9  55                push bp
00003BEA  8BEC              mov bp,sp
00003BEC  8B4608            mov ax,[bp+0x8]
00003BEF  8B5606            mov dx,[bp+0x6]
00003BF2  89168E58          mov [0x588e],dx
00003BF6  A39058            mov [0x5890],ax
00003BF9  0E                push cs
00003BFA  B8B13B            mov ax,0x3bb1
00003BFD  50                push ax
00003BFE  B82400            mov ax,0x24
00003C01  50                push ax
00003C02  9AA03B0000        call 0x0:0x3ba0
00003C07  83C406            add sp,byte +0x6
00003C0A  5D                pop bp
00003C0B  CB                retf
00003C0C  55                push bp
00003C0D  8BEC              mov bp,sp
00003C0F  56                push si
00003C10  57                push di
00003C11  8B4606            mov ax,[bp+0x6]
00003C14  8B269258          mov sp,[0x5892]
00003C18  5D                pop bp
00003C19  5F                pop di
00003C1A  5E                pop si
00003C1B  1F                pop ds
00003C1C  07                pop es
00003C1D  5A                pop dx
00003C1E  59                pop cx
00003C1F  5B                pop bx
00003C20  44                inc sp
00003C21  44                inc sp
00003C22  CF                iret
00003C23  5F                pop di
00003C24  5E                pop si
00003C25  5D                pop bp
00003C26  CB                retf
00003C27  55                push bp
00003C28  8BEC              mov bp,sp
00003C2A  56                push si
00003C2B  57                push di
00003C2C  33C0              xor ax,ax
00003C2E  50                push ax
00003C2F  50                push ax
00003C30  B85400            mov ax,0x54
00003C33  50                push ax
00003C34  9A723A0000        call 0x0:0x3a72
00003C39  83C406            add sp,byte +0x6
00003C3C  8B4606            mov ax,[bp+0x6]
00003C3F  A19258            mov ax,[0x5892]
00003C42  051A00            add ax,0x1a
00003C45  8BE0              mov sp,ax
00003C47  8BE8              mov bp,ax
00003C49  804E1401          or byte [bp+0x14],0x1
00003C4D  5B                pop bx
00003C4E  59                pop cx
00003C4F  5A                pop dx
00003C50  5E                pop si
00003C51  5F                pop di
00003C52  5D                pop bp
00003C53  1F                pop ds
00003C54  07                pop es
00003C55  CF                iret
00003C56  5F                pop di
00003C57  5E                pop si
00003C58  5D                pop bp
00003C59  CB                retf
00003C5A  55                push bp
00003C5B  8BEC              mov bp,sp
00003C5D  8B5606            mov dx,[bp+0x6]
00003C60  ED                in ax,dx
00003C61  5D                pop bp
00003C62  CB                retf

; ---- 0000:3C63 ----
00003C63  55                push bp
00003C64  8BEC              mov bp,sp
00003C66  8B5606            mov dx,[bp+0x6]
00003C69  EC                in al,dx
00003C6A  32E4              xor ah,ah
00003C6C  5D                pop bp
00003C6D  CB                retf

; ---- 0000:3C6E ----
00003C6E  55                push bp
00003C6F  8BEC              mov bp,sp
00003C71  83EC08            sub sp,byte +0x8
00003C74  16                push ss
00003C75  8D46F8            lea ax,[bp-0x8]
00003C78  50                push ax
00003C79  9AA3410000        call 0x0:0x41a3
00003C7E  59                pop cx
00003C7F  59                pop cx
00003C80  16                push ss
00003C81  8D46F8            lea ax,[bp-0x8]
00003C84  50                push ax
00003C85  FF760E            push word [bp+0xe]
00003C88  FF760C            push word [bp+0xc]
00003C8B  FF760A            push word [bp+0xa]
00003C8E  FF7608            push word [bp+0x8]
00003C91  FF7606            push word [bp+0x6]
00003C94  9AA03C0000        call 0x0:0x3ca0
00003C99  83C40E            add sp,byte +0xe
00003C9C  8BE5              mov sp,bp
00003C9E  5D                pop bp
00003C9F  CB                retf

; ---- 0000:3CA0 ----
00003CA0  55                push bp
00003CA1  8BEC              mov bp,sp
00003CA3  83EC0E            sub sp,byte +0xe
00003CA6  56                push si
00003CA7  57                push di
00003CA8  1E                push ds
00003CA9  8D4EF2            lea cx,[bp-0xe]
00003CAC  894EFC            mov [bp-0x4],cx
00003CAF  8C56FE            mov [bp-0x2],ss
00003CB2  C646F255          mov byte [bp-0xe],0x55
00003CB6  C646F3CD          mov byte [bp-0xd],0xcd
00003CBA  8B4606            mov ax,[bp+0x6]
00003CBD  8846F4            mov [bp-0xc],al
00003CC0  C746F55DCB        mov word [bp-0xb],0xcb5d
00003CC5  3C25              cmp al,0x25
00003CC7  7215              jc 0x3cde
00003CC9  3C26              cmp al,0x26
00003CCB  7711              ja 0x3cde
00003CCD  C646F536          mov byte [bp-0xb],0x36
00003CD1  C746F68F06        mov word [bp-0xa],0x68f
00003CD6  894EF8            mov [bp-0x8],cx
00003CD9  C746FA5DCB        mov word [bp-0x6],0xcb5d
00003CDE  C57610            lds si,[bp+0x10]
00003CE1  FF34              push word [si]
00003CE3  FF7406            push word [si+0x6]
00003CE6  C57608            lds si,[bp+0x8]
00003CE9  8B04              mov ax,[si]
00003CEB  8B5C02            mov bx,[si+0x2]
00003CEE  8B4C04            mov cx,[si+0x4]
00003CF1  8B5406            mov dx,[si+0x6]
00003CF4  8B7C0A            mov di,[si+0xa]
00003CF7  8B7408            mov si,[si+0x8]
00003CFA  1F                pop ds
00003CFB  07                pop es
00003CFC  FF5EFC            call far [bp-0x4]
00003CFF  9C                pushf
00003D00  9C                pushf
00003D01  56                push si
00003D02  1E                push ds
00003D03  06                push es
00003D04  C57610            lds si,[bp+0x10]
00003D07  8F04              pop word [si]
00003D09  8F4406            pop word [si+0x6]
00003D0C  C5760C            lds si,[bp+0xc]
00003D0F  8F4408            pop word [si+0x8]
00003D12  8F440E            pop word [si+0xe]
00003D15  8F440C            pop word [si+0xc]
00003D18  83640C01          and word [si+0xc],byte +0x1
00003D1C  897C0A            mov [si+0xa],di
00003D1F  895406            mov [si+0x6],dx
00003D22  894C04            mov [si+0x4],cx
00003D25  895C02            mov [si+0x2],bx
00003D28  8904              mov [si],ax
00003D2A  1F                pop ds
00003D2B  7406              jz 0x3d33
00003D2D  50                push ax
00003D2E  50                push ax
00003D2F  E8BF00            call 0x3df1
00003D32  58                pop ax
00003D33  5F                pop di
00003D34  5E                pop si
00003D35  8BE5              mov sp,bp
00003D37  5D                pop bp
00003D38  CB                retf
00003D39  55                push bp
00003D3A  8BEC              mov bp,sp
00003D3C  83EC08            sub sp,byte +0x8
00003D3F  16                push ss
00003D40  8D46F8            lea ax,[bp-0x8]
00003D43  50                push ax
00003D44  9AA3410000        call 0x0:0x41a3
00003D49  59                pop cx
00003D4A  59                pop cx
00003D4B  16                push ss
00003D4C  8D46F8            lea ax,[bp-0x8]
00003D4F  50                push ax
00003D50  FF760C            push word [bp+0xc]
00003D53  FF760A            push word [bp+0xa]
00003D56  FF7608            push word [bp+0x8]
00003D59  FF7606            push word [bp+0x6]
00003D5C  9A683D0000        call 0x0:0x3d68
00003D61  83C40C            add sp,byte +0xc
00003D64  8BE5              mov sp,bp
00003D66  5D                pop bp
00003D67  CB                retf

; ---- 0000:3D68 ----
00003D68  55                push bp
00003D69  8BEC              mov bp,sp
00003D6B  56                push si
00003D6C  57                push di
00003D6D  1E                push ds
00003D6E  C5760E            lds si,[bp+0xe]
00003D71  FF34              push word [si]
00003D73  FF7406            push word [si+0x6]
00003D76  C57606            lds si,[bp+0x6]
00003D79  8B04              mov ax,[si]
00003D7B  8B5C02            mov bx,[si+0x2]
00003D7E  8B4C04            mov cx,[si+0x4]
00003D81  8B5406            mov dx,[si+0x6]
00003D84  8B7C0A            mov di,[si+0xa]
00003D87  8B7408            mov si,[si+0x8]
00003D8A  1F                pop ds
00003D8B  07                pop es
00003D8C  55                push bp
00003D8D  CD21              int 0x21
00003D8F  5D                pop bp
00003D90  9C                pushf
00003D91  9C                pushf
00003D92  56                push si
00003D93  1E                push ds
00003D94  06                push es
00003D95  C5760E            lds si,[bp+0xe]
00003D98  8F04              pop word [si]
00003D9A  8F4406            pop word [si+0x6]
00003D9D  C5760A            lds si,[bp+0xa]
00003DA0  8F4408            pop word [si+0x8]
00003DA3  8F440E            pop word [si+0xe]
00003DA6  8F440C            pop word [si+0xc]
00003DA9  83640C01          and word [si+0xc],byte +0x1
00003DAD  897C0A            mov [si+0xa],di
00003DB0  895406            mov [si+0x6],dx
00003DB3  894C04            mov [si+0x4],cx
00003DB6  895C02            mov [si+0x2],bx
00003DB9  8904              mov [si],ax
00003DBB  1F                pop ds
00003DBC  7406              jz 0x3dc4
00003DBE  50                push ax
00003DBF  50                push ax
00003DC0  E82E00            call 0x3df1
00003DC3  58                pop ax
00003DC4  5F                pop di
00003DC5  5E                pop si
00003DC6  5D                pop bp
00003DC7  CB                retf

; ---- 0000:3DC8 ----
00003DC8  55                push bp
00003DC9  8BEC              mov bp,sp
00003DCB  1E                push ds
00003DCC  B444              mov ah,0x44
00003DCE  8A4608            mov al,[bp+0x8]
00003DD1  8B5E06            mov bx,[bp+0x6]
00003DD4  8B4E0E            mov cx,[bp+0xe]
00003DD7  C5560A            lds dx,[bp+0xa]
00003DDA  CD21              int 0x21
00003DDC  1F                pop ds
00003DDD  720C              jc 0x3deb
00003DDF  837E0800          cmp word [bp+0x8],byte +0x0
00003DE3  7504              jnz 0x3de9
00003DE5  8BC2              mov ax,dx
00003DE7  EB06              jmp short 0x3def
00003DE9  EB04              jmp short 0x3def
00003DEB  50                push ax
00003DEC  E80200            call 0x3df1
00003DEF  5D                pop bp
00003DF0  CB                retf

; ---- 0000:3DF1 ----
00003DF1  55                push bp
00003DF2  8BEC              mov bp,sp
00003DF4  56                push si
00003DF5  8B7604            mov si,[bp+0x4]
00003DF8  0BF6              or si,si
00003DFA  7C15              jl 0x3e11
00003DFC  83FE58            cmp si,byte +0x58
00003DFF  7E03              jng 0x3e04
00003E01  BE5700            mov si,0x57
00003E04  89366E14          mov [0x146e],si
00003E08  8A847014          mov al,[si+0x1470]
00003E0C  98                cbw
00003E0D  8BF0              mov si,ax
00003E0F  EB11              jmp short 0x3e22
00003E11  8BC6              mov ax,si
00003E13  F7D8              neg ax
00003E15  8BF0              mov si,ax
00003E17  3D2300            cmp ax,0x23
00003E1A  7FE5              jg 0x3e01
00003E1C  C7066E14FFFF      mov word [0x146e],0xffff
00003E22  89367D00          mov [0x7d],si
00003E26  B8FFFF            mov ax,0xffff
00003E29  5E                pop si
00003E2A  5D                pop bp
00003E2B  C20200            ret 0x2

; ---- 0000:3E2E ----
00003E2E  55                push bp
00003E2F  8BEC              mov bp,sp
00003E31  B80044            mov ax,0x4400
00003E34  8B5E06            mov bx,[bp+0x6]
00003E37  CD21              int 0x21
00003E39  92                xchg ax,dx
00003E3A  258000            and ax,0x80
00003E3D  5D                pop bp
00003E3E  CB                retf

; ---- 0000:3E3F ----
00003E3F  55                push bp
00003E40  8BEC              mov bp,sp
00003E42  8B5E06            mov bx,[bp+0x6]
00003E45  D1E3              shl bx,1
00003E47  81A72414FFFD      and word [bx+0x1424],0xfdff
00003E4D  B442              mov ah,0x42
00003E4F  8A460C            mov al,[bp+0xc]
00003E52  8B5E06            mov bx,[bp+0x6]
00003E55  8B4E0A            mov cx,[bp+0xa]
00003E58  8B5608            mov dx,[bp+0x8]
00003E5B  CD21              int 0x21
00003E5D  7202              jc 0x3e61
00003E5F  EB05              jmp short 0x3e66
00003E61  50                push ax
00003E62  E88CFF            call 0x3df1
00003E65  99                cwd
00003E66  5D                pop bp
00003E67  CB                retf

; ---- 0000:3E68 ----
00003E68  55                push bp
00003E69  8BEC              mov bp,sp
00003E6B  83EC22            sub sp,byte +0x22
00003E6E  56                push si
00003E6F  57                push di
00003E70  06                push es
00003E71  C47E0A            les di,[bp+0xa]
00003E74  8B5E08            mov bx,[bp+0x8]
00003E77  83FB24            cmp bx,byte +0x24
00003E7A  775C              ja 0x3ed8
00003E7C  80FB02            cmp bl,0x2
00003E7F  7257              jc 0x3ed8
00003E81  8B460E            mov ax,[bp+0xe]
00003E84  8B4E10            mov cx,[bp+0x10]
00003E87  0BC9              or cx,cx
00003E89  7D12              jnl 0x3e9d
00003E8B  807E0600          cmp byte [bp+0x6],0x0
00003E8F  740C              jz 0x3e9d
00003E91  26C6052D          mov byte [es:di],0x2d
00003E95  47                inc di
00003E96  F7D9              neg cx
00003E98  F7D8              neg ax
00003E9A  83D900            sbb cx,byte +0x0
00003E9D  8D76DE            lea si,[bp-0x22]
00003EA0  E310              jcxz 0x3eb2
00003EA2  91                xchg ax,cx
00003EA3  2BD2              sub dx,dx
00003EA5  F7F3              div bx
00003EA7  91                xchg ax,cx
00003EA8  F7F3              div bx
00003EAA  368814            mov [ss:si],dl
00003EAD  46                inc si
00003EAE  E30A              jcxz 0x3eba
00003EB0  EBF0              jmp short 0x3ea2
00003EB2  2BD2              sub dx,dx
00003EB4  F7F3              div bx
00003EB6  368814            mov [ss:si],dl
00003EB9  46                inc si
00003EBA  0BC0              or ax,ax
00003EBC  75F4              jnz 0x3eb2
00003EBE  8D4EDE            lea cx,[bp-0x22]
00003EC1  F7D9              neg cx
00003EC3  03CE              add cx,si
00003EC5  FC                cld
00003EC6  4E                dec si
00003EC7  368A04            mov al,[ss:si]
00003ECA  2C0A              sub al,0xa
00003ECC  7304              jnc 0x3ed2
00003ECE  043A              add al,0x3a
00003ED0  EB03              jmp short 0x3ed5
00003ED2  024604            add al,[bp+0x4]
00003ED5  AA                stosb
00003ED6  E2EE              loop 0x3ec6
00003ED8  B000              mov al,0x0
00003EDA  AA                stosb
00003EDB  07                pop es
00003EDC  8B560C            mov dx,[bp+0xc]
00003EDF  8B460A            mov ax,[bp+0xa]
00003EE2  5F                pop di
00003EE3  5E                pop si
00003EE4  8BE5              mov sp,bp
00003EE6  5D                pop bp
00003EE7  C20E00            ret 0xe
00003EEA  55                push bp
00003EEB  8BEC              mov bp,sp
00003EED  837E0C0A          cmp word [bp+0xc],byte +0xa
00003EF1  7506              jnz 0x3ef9
00003EF3  8B4606            mov ax,[bp+0x6]
00003EF6  99                cwd
00003EF7  EB05              jmp short 0x3efe
00003EF9  8B4606            mov ax,[bp+0x6]
00003EFC  33D2              xor dx,dx
00003EFE  52                push dx
00003EFF  50                push ax
00003F00  FF760A            push word [bp+0xa]
00003F03  FF7608            push word [bp+0x8]
00003F06  FF760C            push word [bp+0xc]
00003F09  B001              mov al,0x1
00003F0B  50                push ax
00003F0C  B061              mov al,0x61
00003F0E  50                push ax
00003F0F  E856FF            call 0x3e68
00003F12  5D                pop bp
00003F13  CB                retf

; ---- 0000:3F14 ----
00003F14  55                push bp
00003F15  8BEC              mov bp,sp
00003F17  FF7608            push word [bp+0x8]
00003F1A  FF7606            push word [bp+0x6]
00003F1D  FF760C            push word [bp+0xc]
00003F20  FF760A            push word [bp+0xa]
00003F23  FF760E            push word [bp+0xe]
00003F26  B000              mov al,0x0
00003F28  50                push ax
00003F29  B061              mov al,0x61
00003F2B  50                push ax
00003F2C  E839FF            call 0x3e68
00003F2F  5D                pop bp
00003F30  CB                retf
00003F31  55                push bp
00003F32  8BEC              mov bp,sp
00003F34  FF7608            push word [bp+0x8]
00003F37  FF7606            push word [bp+0x6]
00003F3A  FF760C            push word [bp+0xc]
00003F3D  FF760A            push word [bp+0xa]
00003F40  FF760E            push word [bp+0xe]
00003F43  837E0E0A          cmp word [bp+0xe],byte +0xa
00003F47  7505              jnz 0x3f4e
00003F49  B80100            mov ax,0x1
00003F4C  EB02              jmp short 0x3f50
00003F4E  33C0              xor ax,ax
00003F50  50                push ax
00003F51  B061              mov al,0x61
00003F53  50                push ax
00003F54  E811FF            call 0x3e68
00003F57  5D                pop bp
00003F58  CB                retf

; ---- 0000:3F59 ----
00003F59  55                push bp
00003F5A  8BEC              mov bp,sp
00003F5C  56                push si
00003F5D  57                push di
00003F5E  8CDA              mov dx,ds
00003F60  C47E06            les di,[bp+0x6]
00003F63  C5760A            lds si,[bp+0xa]
00003F66  8B4E0E            mov cx,[bp+0xe]
00003F69  D1E9              shr cx,1
00003F6B  FC                cld
00003F6C  F3A5              rep movsw
00003F6E  7301              jnc 0x3f71
00003F70  A4                movsb
00003F71  8EDA              mov ds,dx
00003F73  8B5608            mov dx,[bp+0x8]
00003F76  8B4606            mov ax,[bp+0x6]
00003F79  5F                pop di
00003F7A  5E                pop si
00003F7B  5D                pop bp
00003F7C  CB                retf

; ---- 0000:3F7D ----
00003F7D  55                push bp
00003F7E  8BEC              mov bp,sp
00003F80  57                push di
00003F81  C47E06            les di,[bp+0x6]
00003F84  8B4E0A            mov cx,[bp+0xa]
00003F87  8A460C            mov al,[bp+0xc]
00003F8A  8AE0              mov ah,al
00003F8C  FC                cld
00003F8D  F7C70100          test di,0x1
00003F91  7404              jz 0x3f97
00003F93  E309              jcxz 0x3f9e
00003F95  AA                stosb
00003F96  49                dec cx
00003F97  D1E9              shr cx,1
00003F99  F3AB              rep stosw
00003F9B  7301              jnc 0x3f9e
00003F9D  AA                stosb
00003F9E  5F                pop di
00003F9F  5D                pop bp
00003FA0  CB                retf

; ---- 0000:3FA1 ----
00003FA1  55                push bp
00003FA2  8BEC              mov bp,sp
00003FA4  8A460A            mov al,[bp+0xa]
00003FA7  50                push ax
00003FA8  FF760C            push word [bp+0xc]
00003FAB  FF7608            push word [bp+0x8]
00003FAE  FF7606            push word [bp+0x6]
00003FB1  0E                push cs
00003FB2  E8C8FF            call 0x3f7d
00003FB5  83C408            add sp,byte +0x8
00003FB8  8B5608            mov dx,[bp+0x8]
00003FBB  8B4606            mov ax,[bp+0x6]
00003FBE  5D                pop bp
00003FBF  CB                retf

; ---- 0000:3FC0 ----
00003FC0  55                push bp
00003FC1  8BEC              mov bp,sp
00003FC3  1E                push ds
00003FC4  8B4E04            mov cx,[bp+0x4]
00003FC7  B43C              mov ah,0x3c
00003FC9  C55606            lds dx,[bp+0x6]
00003FCC  CD21              int 0x21
00003FCE  1F                pop ds
00003FCF  7202              jc 0x3fd3
00003FD1  EB04              jmp short 0x3fd7
00003FD3  50                push ax
00003FD4  E81AFE            call 0x3df1
00003FD7  5D                pop bp
00003FD8  C20600            ret 0x6

; ---- 0000:3FDB ----
00003FDB  55                push bp
00003FDC  8BEC              mov bp,sp
00003FDE  8B5E04            mov bx,[bp+0x4]
00003FE1  2BC9              sub cx,cx
00003FE3  2BD2              sub dx,dx
00003FE5  B440              mov ah,0x40
00003FE7  CD21              int 0x21
00003FE9  5D                pop bp
00003FEA  C20200            ret 0x2

; ---- 0000:3FED ----
00003FED  55                push bp
00003FEE  8BEC              mov bp,sp
00003FF0  83EC04            sub sp,byte +0x4
00003FF3  56                push si
00003FF4  57                push di
00003FF5  8B760A            mov si,[bp+0xa]
00003FF8  F7C600C0          test si,0xc000
00003FFC  750C              jnz 0x400a
00003FFE  A14C14            mov ax,[0x144c]
00004001  2500C0            and ax,0xc000
00004004  8BD6              mov dx,si
00004006  0BD0              or dx,ax
00004008  8BF2              mov si,dx
0000400A  F7C60001          test si,0x100
0000400E  7503              jnz 0x4013
00004010  E98800            jmp 0x409b
00004013  A14E14            mov ax,[0x144e]
00004016  21460C            and [bp+0xc],ax
00004019  8B460C            mov ax,[bp+0xc]
0000401C  A98001            test ax,0x180
0000401F  7507              jnz 0x4028
00004021  B80100            mov ax,0x1
00004024  50                push ax
00004025  E8C9FD            call 0x3df1
00004028  33C0              xor ax,ax
0000402A  50                push ax
0000402B  FF7608            push word [bp+0x8]
0000402E  FF7606            push word [bp+0x6]
00004031  9AA53A0000        call 0x0:0x3aa5
00004036  83C406            add sp,byte +0x6
00004039  8946FE            mov [bp-0x2],ax
0000403C  3DFFFF            cmp ax,0xffff
0000403F  752C              jnz 0x406d
00004041  F7460C8000        test word [bp+0xc],0x80
00004046  7404              jz 0x404c
00004048  33C0              xor ax,ax
0000404A  EB03              jmp short 0x404f
0000404C  B80100            mov ax,0x1
0000404F  8946FE            mov [bp-0x2],ax
00004052  F7C6F000          test si,0xf0
00004056  742E              jz 0x4086
00004058  FF7608            push word [bp+0x8]
0000405B  FF7606            push word [bp+0x6]
0000405E  33C0              xor ax,ax
00004060  50                push ax
00004061  E85CFF            call 0x3fc0
00004064  8BF8              mov di,ax
00004066  0BC0              or ax,ax
00004068  7D13              jnl 0x407d
0000406A  E9C800            jmp 0x4135
0000406D  F7C60004          test si,0x400
00004071  7428              jz 0x409b
00004073  B85000            mov ax,0x50
00004076  50                push ax
00004077  E877FD            call 0x3df1
0000407A  E9BA00            jmp 0x4137
0000407D  57                push di
0000407E  9AC23A0000        call 0x0:0x3ac2
00004083  59                pop cx
00004084  EB1A              jmp short 0x40a0
00004086  FF7608            push word [bp+0x8]
00004089  FF7606            push word [bp+0x6]
0000408C  FF76FE            push word [bp-0x2]
0000408F  E82EFF            call 0x3fc0
00004092  8BF8              mov di,ax
00004094  0BC0              or ax,ax
00004096  7D7C              jnl 0x4114
00004098  E99A00            jmp 0x4135
0000409B  C746FE0000        mov word [bp-0x2],0x0
000040A0  56                push si
000040A1  FF7608            push word [bp+0x8]
000040A4  FF7606            push word [bp+0x6]
000040A7  9A3D410000        call 0x0:0x413d
000040AC  83C406            add sp,byte +0x6
000040AF  8BF8              mov di,ax
000040B1  0BC0              or ax,ax
000040B3  7C5F              jl 0x4114
000040B5  33C0              xor ax,ax
000040B7  50                push ax
000040B8  57                push di
000040B9  9AC83D0000        call 0x0:0x3dc8
000040BE  59                pop cx
000040BF  59                pop cx
000040C0  8946FC            mov [bp-0x4],ax
000040C3  A98000            test ax,0x80
000040C6  7423              jz 0x40eb
000040C8  81CE0020          or si,0x2000
000040CC  F7C60080          test si,0x8000
000040D0  7423              jz 0x40f5
000040D2  25FF00            and ax,0xff
000040D5  0D2000            or ax,0x20
000040D8  33D2              xor dx,dx
000040DA  52                push dx
000040DB  50                push ax
000040DC  B80100            mov ax,0x1
000040DF  50                push ax
000040E0  57                push di
000040E1  9AC83D0000        call 0x0:0x3dc8
000040E6  83C408            add sp,byte +0x8
000040E9  EB0A              jmp short 0x40f5
000040EB  F7C60002          test si,0x200
000040EF  7404              jz 0x40f5
000040F1  57                push di
000040F2  E8E6FE            call 0x3fdb
000040F5  837EFE00          cmp word [bp-0x2],byte +0x0
000040F9  7419              jz 0x4114
000040FB  F7C6F000          test si,0xf0
000040FF  7413              jz 0x4114
00004101  B80100            mov ax,0x1
00004104  50                push ax
00004105  50                push ax
00004106  FF7608            push word [bp+0x8]
00004109  FF7606            push word [bp+0x6]
0000410C  9AA53A0000        call 0x0:0x3aa5
00004111  83C408            add sp,byte +0x8
00004114  0BFF              or di,di
00004116  7C1D              jl 0x4135
00004118  F7C60003          test si,0x300
0000411C  7405              jz 0x4123
0000411E  B80010            mov ax,0x1000
00004121  EB02              jmp short 0x4125
00004123  33C0              xor ax,ax
00004125  8BD6              mov dx,si
00004127  81E2FFF8          and dx,0xf8ff
0000412B  0BD0              or dx,ax
0000412D  8BDF              mov bx,di
0000412F  D1E3              shl bx,1
00004131  89972414          mov [bx+0x1424],dx
00004135  8BC7              mov ax,di
00004137  5F                pop di
00004138  5E                pop si
00004139  8BE5              mov sp,bp
0000413B  5D                pop bp
0000413C  CB                retf

; ---- 0000:413D ----
0000413D  55                push bp
0000413E  8BEC              mov bp,sp
00004140  4C                dec sp
00004141  4C                dec sp
00004142  B001              mov al,0x1
00004144  8B4E0A            mov cx,[bp+0xa]
00004147  F7C10200          test cx,0x2
0000414B  750A              jnz 0x4157
0000414D  B002              mov al,0x2
0000414F  F7C10400          test cx,0x4
00004153  7502              jnz 0x4157
00004155  B000              mov al,0x0
00004157  1E                push ds
00004158  C55606            lds dx,[bp+0x6]
0000415B  B1F0              mov cl,0xf0
0000415D  224E0A            and cl,[bp+0xa]
00004160  0AC1              or al,cl
00004162  B43D              mov ah,0x3d
00004164  CD21              int 0x21
00004166  1F                pop ds
00004167  721A              jc 0x4183
00004169  8946FE            mov [bp-0x2],ax
0000416C  8B5EFE            mov bx,[bp-0x2]
0000416F  D1E3              shl bx,1
00004171  8B460A            mov ax,[bp+0xa]
00004174  25FFF8            and ax,0xf8ff
00004177  0D0080            or ax,0x8000
0000417A  89872414          mov [bx+0x1424],ax
0000417E  8B46FE            mov ax,[bp-0x2]
00004181  EB04              jmp short 0x4187
00004183  50                push ax
00004184  E86AFC            call 0x3df1
00004187  8BE5              mov sp,bp
00004189  5D                pop bp
0000418A  CB                retf
0000418B  55                push bp
0000418C  8BEC              mov bp,sp
0000418E  8B5606            mov dx,[bp+0x6]
00004191  8B4608            mov ax,[bp+0x8]
00004194  EF                out dx,ax
00004195  5D                pop bp
00004196  CB                retf

; ---- 0000:4197 ----
00004197  55                push bp
00004198  8BEC              mov bp,sp
0000419A  8B5606            mov dx,[bp+0x6]
0000419D  8A4608            mov al,[bp+0x8]
000041A0  EE                out dx,al
000041A1  5D                pop bp
000041A2  CB                retf

; ---- 0000:41A3 ----
000041A3  55                push bp
000041A4  8BEC              mov bp,sp
000041A6  56                push si
000041A7  8CC3              mov bx,es
000041A9  C47606            les si,[bp+0x6]
000041AC  26891C            mov [es:si],bx
000041AF  8B4604            mov ax,[bp+0x4]
000041B2  26894402          mov [es:si+0x2],ax
000041B6  268C5404          mov [es:si+0x4],ss
000041BA  268C5C06          mov [es:si+0x6],ds
000041BE  8EC3              mov es,bx
000041C0  5E                pop si
000041C1  5D                pop bp
000041C2  CB                retf

; ---- 0000:41C3 ----
000041C3  55                push bp
000041C4  8BEC              mov bp,sp
000041C6  B44A              mov ah,0x4a
000041C8  8B5E08            mov bx,[bp+0x8]
000041CB  8E4606            mov es,[bp+0x6]
000041CE  CD21              int 0x21
000041D0  7205              jc 0x41d7
000041D2  B8FFFF            mov ax,0xffff
000041D5  EB06              jmp short 0x41dd
000041D7  53                push bx
000041D8  50                push ax
000041D9  E815FC            call 0x3df1
000041DC  58                pop ax
000041DD  5D                pop bp
000041DE  CB                retf

; ---- 0000:41DF ----
000041DF  55                push bp
000041E0  8BEC              mov bp,sp
000041E2  56                push si
000041E3  B42B              mov ah,0x2b
000041E5  C47606            les si,[bp+0x6]
000041E8  268B0C            mov cx,[es:si]
000041EB  268B5402          mov dx,[es:si+0x2]
000041EF  CD21              int 0x21
000041F1  5E                pop si
000041F2  5D                pop bp
000041F3  CB                retf

; ---- 0000:41F4 ----
000041F4  55                push bp
000041F5  8BEC              mov bp,sp
000041F7  56                push si
000041F8  B42D              mov ah,0x2d
000041FA  C47606            les si,[bp+0x6]
000041FD  268B0C            mov cx,[es:si]
00004200  268B5402          mov dx,[es:si+0x2]
00004204  CD21              int 0x21
00004206  5E                pop si
00004207  5D                pop bp
00004208  CB                retf

; ---- 0000:4209 ----
00004209  55                push bp
0000420A  8BEC              mov bp,sp
0000420C  8B5E06            mov bx,[bp+0x6]
0000420F  B8DD34            mov ax,0x34dd
00004212  BA1200            mov dx,0x12
00004215  3BD3              cmp dx,bx
00004217  731A              jnc 0x4233
00004219  F7F3              div bx
0000421B  8BD8              mov bx,ax
0000421D  E461              in al,0x61
0000421F  A803              test al,0x3
00004221  7508              jnz 0x422b
00004223  0C03              or al,0x3
00004225  E661              out 0x61,al
00004227  B0B6              mov al,0xb6
00004229  E643              out 0x43,al
0000422B  8AC3              mov al,bl
0000422D  E642              out 0x42,al
0000422F  8AC7              mov al,bh
00004231  E642              out 0x42,al
00004233  5D                pop bp
00004234  CB                retf

; ---- 0000:4235 ----
00004235  E461              in al,0x61
00004237  24FC              and al,0xfc
00004239  E661              out 0x61,al
0000423B  CB                retf

; ---- 0000:423C ----
0000423C  55                push bp
0000423D  8BEC              mov bp,sp
0000423F  56                push si
00004240  57                push di
00004241  FC                cld
00004242  1E                push ds
00004243  C47E06            les di,[bp+0x6]
00004246  8BD7              mov dx,di
00004248  32C0              xor al,al
0000424A  B9FFFF            mov cx,0xffff
0000424D  F2AE              repne scasb
0000424F  06                push es
00004250  8D75FF            lea si,[di-0x1]
00004253  C47E0A            les di,[bp+0xa]
00004256  B9FFFF            mov cx,0xffff
00004259  F2AE              repne scasb
0000425B  F7D1              not cx
0000425D  2BF9              sub di,cx
0000425F  06                push es
00004260  1F                pop ds
00004261  07                pop es
00004262  87F7              xchg si,di
00004264  F7C60100          test si,0x1
00004268  7402              jz 0x426c
0000426A  A4                movsb
0000426B  49                dec cx
0000426C  D1E9              shr cx,1
0000426E  F3A5              rep movsw
00004270  7301              jnc 0x4273
00004272  A4                movsb
00004273  92                xchg ax,dx
00004274  8CC2              mov dx,es
00004276  1F                pop ds
00004277  5F                pop di
00004278  5E                pop si
00004279  5D                pop bp
0000427A  CB                retf

; ---- 0000:427B ----
0000427B  55                push bp
0000427C  8BEC              mov bp,sp
0000427E  56                push si
0000427F  57                push di
00004280  FC                cld
00004281  C47E0A            les di,[bp+0xa]
00004284  8BF7              mov si,di
00004286  32C0              xor al,al
00004288  B9FFFF            mov cx,0xffff
0000428B  F2AE              repne scasb
0000428D  F7D1              not cx
0000428F  1E                push ds
00004290  8CC0              mov ax,es
00004292  8ED8              mov ds,ax
00004294  C47E06            les di,[bp+0x6]
00004297  F3A4              rep movsb
00004299  1F                pop ds
0000429A  8B5608            mov dx,[bp+0x8]
0000429D  8B4606            mov ax,[bp+0x6]
000042A0  5F                pop di
000042A1  5E                pop si
000042A2  5D                pop bp
000042A3  CB                retf

; ---- 0000:42A4 ----
000042A4  55                push bp
000042A5  8BEC              mov bp,sp
000042A7  56                push si
000042A8  57                push di
000042A9  8CDA              mov dx,ds
000042AB  FC                cld
000042AC  C57606            lds si,[bp+0x6]
000042AF  C47E0A            les di,[bp+0xa]
000042B2  33C0              xor ax,ax
000042B4  8BD8              mov bx,ax
000042B6  B97A61            mov cx,0x617a
000042B9  AC                lodsb
000042BA  268A1D            mov bl,[es:di]
000042BD  0AC0              or al,al
000042BF  741C              jz 0x42dd
000042C1  AE                scasb
000042C2  74F5              jz 0x42b9
000042C4  3AC5              cmp al,ch
000042C6  7206              jc 0x42ce
000042C8  3AC1              cmp al,cl
000042CA  7702              ja 0x42ce
000042CC  2C20              sub al,0x20
000042CE  3ADD              cmp bl,ch
000042D0  7207              jc 0x42d9
000042D2  3AD9              cmp bl,cl
000042D4  7703              ja 0x42d9
000042D6  80EB20            sub bl,0x20
000042D9  3AC3              cmp al,bl
000042DB  74DC              jz 0x42b9
000042DD  2BC3              sub ax,bx
000042DF  8EDA              mov ds,dx
000042E1  5F                pop di
000042E2  5E                pop si
000042E3  5D                pop bp
000042E4  CB                retf

; ---- 0000:42E5 ----
000042E5  55                push bp
000042E6  8BEC              mov bp,sp
000042E8  57                push di
000042E9  C47E06            les di,[bp+0x6]
000042EC  33C0              xor ax,ax
000042EE  3B4608            cmp ax,[bp+0x8]
000042F1  7504              jnz 0x42f7
000042F3  3BC7              cmp ax,di
000042F5  740A              jz 0x4301
000042F7  FC                cld
000042F8  B9FFFF            mov cx,0xffff
000042FB  F2AE              repne scasb
000042FD  91                xchg ax,cx
000042FE  F7D0              not ax
00004300  48                dec ax
00004301  5F                pop di
00004302  5D                pop bp
00004303  CB                retf

; ---- 0000:4304 ----
00004304  55                push bp
00004305  8BEC              mov bp,sp
00004307  56                push si
00004308  57                push di
00004309  FC                cld
0000430A  C47E0A            les di,[bp+0xa]
0000430D  8BF7              mov si,di
0000430F  32C0              xor al,al
00004311  8B5E0E            mov bx,[bp+0xe]
00004314  8BCB              mov cx,bx
00004316  F2AE              repne scasb
00004318  2BD9              sub bx,cx
0000431A  1E                push ds
0000431B  8CC7              mov di,es
0000431D  8EDF              mov ds,di
0000431F  C47E06            les di,[bp+0x6]
00004322  87CB              xchg cx,bx
00004324  F3A4              rep movsb
00004326  8BCB              mov cx,bx
00004328  F3AA              rep stosb
0000432A  1F                pop ds
0000432B  8B5608            mov dx,[bp+0x8]
0000432E  8B4606            mov ax,[bp+0x6]
00004331  5F                pop di
00004332  5E                pop si
00004333  5D                pop bp
00004334  CB                retf

; ---- 0000:4335 ----
00004335  55                push bp
00004336  8BEC              mov bp,sp
00004338  83EC04            sub sp,byte +0x4
0000433B  56                push si
0000433C  1E                push ds
0000433D  B8F214            mov ax,0x14f2
00004340  50                push ax
00004341  9A213B0000        call 0x0:0x3b21
00004346  59                pop cx
00004347  59                pop cx
00004348  8946FC            mov [bp-0x4],ax
0000434B  8956FE            mov [bp-0x2],dx
0000434E  0BC2              or ax,dx
00004350  7503              jnz 0x4355
00004352  E99500            jmp 0x43ea
00004355  52                push dx
00004356  FF76FC            push word [bp-0x4]
00004359  9AE5420000        call 0x0:0x42e5
0000435E  59                pop cx
0000435F  59                pop cx
00004360  3D0400            cmp ax,0x4
00004363  7303              jnc 0x4368
00004365  E98200            jmp 0x43ea
00004368  C45EFC            les bx,[bp-0x4]
0000436B  268A07            mov al,[es:bx]
0000436E  98                cbw
0000436F  8BD8              mov bx,ax
00004371  8A878511          mov al,[bx+0x1185]
00004375  98                cbw
00004376  A90C00            test ax,0xc
00004379  746F              jz 0x43ea
0000437B  8B5EFC            mov bx,[bp-0x4]
0000437E  268A4701          mov al,[es:bx+0x1]
00004382  98                cbw
00004383  8BD8              mov bx,ax
00004385  8A878511          mov al,[bx+0x1185]
00004389  98                cbw
0000438A  A90C00            test ax,0xc
0000438D  745B              jz 0x43ea
0000438F  8B5EFC            mov bx,[bp-0x4]
00004392  268A4702          mov al,[es:bx+0x2]
00004396  98                cbw
00004397  8BD8              mov bx,ax
00004399  8A878511          mov al,[bx+0x1185]
0000439D  98                cbw
0000439E  A90C00            test ax,0xc
000043A1  7447              jz 0x43ea
000043A3  8B5EFC            mov bx,[bp-0x4]
000043A6  26807F032D        cmp byte [es:bx+0x3],0x2d
000043AB  7415              jz 0x43c2
000043AD  26807F032B        cmp byte [es:bx+0x3],0x2b
000043B2  740E              jz 0x43c2
000043B4  268A4703          mov al,[es:bx+0x3]
000043B8  98                cbw
000043B9  8BD8              mov bx,ax
000043BB  F687851102        test byte [bx+0x1185],0x2
000043C0  7428              jz 0x43ea
000043C2  C45EFC            les bx,[bp-0x4]
000043C5  268A4703          mov al,[es:bx+0x3]
000043C9  98                cbw
000043CA  8BD8              mov bx,ax
000043CC  8A878511          mov al,[bx+0x1185]
000043D0  98                cbw
000043D1  A90200            test ax,0x2
000043D4  7553              jnz 0x4429
000043D6  8B5EFC            mov bx,[bp-0x4]
000043D9  268A4704          mov al,[es:bx+0x4]
000043DD  98                cbw
000043DE  8BD8              mov bx,ax
000043E0  8A878511          mov al,[bx+0x1185]
000043E4  98                cbw
000043E5  A90200            test ax,0x2
000043E8  753F              jnz 0x4429
000043EA  C706F0140100      mov word [0x14f0],0x1
000043F0  C706EC145046      mov word [0x14ec],0x4650
000043F6  C706EE140000      mov word [0x14ee],0x0
000043FC  1E                push ds
000043FD  B8F514            mov ax,0x14f5
00004400  50                push ax
00004401  FF36E614          push word [0x14e6]
00004405  FF36E414          push word [0x14e4]
00004409  9A7B420000        call 0x0:0x427b
0000440E  83C408            add sp,byte +0x8
00004411  1E                push ds
00004412  B8F914            mov ax,0x14f9
00004415  50                push ax
00004416  FF36EA14          push word [0x14ea]
0000441A  FF36E814          push word [0x14e8]
0000441E  9A7B420000        call 0x0:0x427b
00004423  83C408            add sp,byte +0x8
00004426  E9F100            jmp 0x451a
00004429  B80400            mov ax,0x4
0000442C  50                push ax
0000442D  33C0              xor ax,ax
0000442F  50                push ax
00004430  FF36EA14          push word [0x14ea]
00004434  FF36E814          push word [0x14e8]
00004438  9AA13F0000        call 0x0:0x3fa1
0000443D  83C408            add sp,byte +0x8
00004440  B80300            mov ax,0x3
00004443  50                push ax
00004444  FF76FE            push word [bp-0x2]
00004447  FF76FC            push word [bp-0x4]
0000444A  FF36E614          push word [0x14e6]
0000444E  FF36E414          push word [0x14e4]
00004452  9A04430000        call 0x0:0x4304
00004457  83C40A            add sp,byte +0xa
0000445A  C41EE414          les bx,[0x14e4]
0000445E  26C6470300        mov byte [es:bx+0x3],0x0
00004463  8B46FC            mov ax,[bp-0x4]
00004466  050300            add ax,0x3
00004469  FF76FE            push word [bp-0x2]
0000446C  50                push ax
0000446D  9AEC390000        call 0x0:0x39ec
00004472  59                pop cx
00004473  59                pop cx
00004474  50                push ax
00004475  52                push dx
00004476  33D2              xor dx,dx
00004478  B8100E            mov ax,0xe10
0000447B  59                pop cx
0000447C  5B                pop bx
0000447D  E8250D            call 0x51a5
00004480  A3EC14            mov [0x14ec],ax
00004483  8916EE14          mov [0x14ee],dx
00004487  C706F0140000      mov word [0x14f0],0x0
0000448D  BE0300            mov si,0x3
00004490  EB7C              jmp short 0x450e
00004492  C45EFC            les bx,[bp-0x4]
00004495  268A00            mov al,[es:bx+si]
00004498  98                cbw
00004499  8BD8              mov bx,ax
0000449B  F68785110C        test byte [bx+0x1185],0xc
000044A0  746B              jz 0x450d
000044A2  8B46FC            mov ax,[bp-0x4]
000044A5  03C6              add ax,si
000044A7  FF76FE            push word [bp-0x2]
000044AA  50                push ax
000044AB  9AE5420000        call 0x0:0x42e5
000044B0  59                pop cx
000044B1  59                pop cx
000044B2  3D0300            cmp ax,0x3
000044B5  7263              jc 0x451a
000044B7  C45EFC            les bx,[bp-0x4]
000044BA  268A4001          mov al,[es:bx+si+0x1]
000044BE  98                cbw
000044BF  8BD8              mov bx,ax
000044C1  8A878511          mov al,[bx+0x1185]
000044C5  98                cbw
000044C6  A90C00            test ax,0xc
000044C9  744F              jz 0x451a
000044CB  8B5EFC            mov bx,[bp-0x4]
000044CE  268A4002          mov al,[es:bx+si+0x2]
000044D2  98                cbw
000044D3  8BD8              mov bx,ax
000044D5  8A878511          mov al,[bx+0x1185]
000044D9  98                cbw
000044DA  A90C00            test ax,0xc
000044DD  743B              jz 0x451a
000044DF  B80300            mov ax,0x3
000044E2  50                push ax
000044E3  8B46FC            mov ax,[bp-0x4]
000044E6  03C6              add ax,si
000044E8  FF76FE            push word [bp-0x2]
000044EB  50                push ax
000044EC  FF36EA14          push word [0x14ea]
000044F0  FF36E814          push word [0x14e8]
000044F4  9A04430000        call 0x0:0x4304
000044F9  83C40A            add sp,byte +0xa
000044FC  C41EE814          les bx,[0x14e8]
00004500  26C6470300        mov byte [es:bx+0x3],0x0
00004505  C706F0140100      mov word [0x14f0],0x1
0000450B  EB0D              jmp short 0x451a
0000450D  46                inc si
0000450E  C45EFC            les bx,[bp-0x4]
00004511  26803800          cmp byte [es:bx+si],0x0
00004515  7403              jz 0x451a
00004517  E978FF            jmp 0x4492
0000451A  5E                pop si
0000451B  8BE5              mov sp,bp
0000451D  5D                pop bp
0000451E  CB                retf

; ---- 0000:451F ----
0000451F  55                push bp
00004520  8BEC              mov bp,sp
00004522  56                push si
00004523  837E0600          cmp word [bp+0x6],byte +0x0
00004527  752C              jnz 0x4555
00004529  8B7608            mov si,[bp+0x8]
0000452C  837E083B          cmp word [bp+0x8],byte +0x3b
00004530  720C              jc 0x453e
00004532  8B4604            mov ax,[bp+0x4]
00004535  054600            add ax,0x46
00004538  A90300            test ax,0x3
0000453B  7501              jnz 0x453e
0000453D  4E                dec si
0000453E  C746060000        mov word [bp+0x6],0x0
00004543  EB03              jmp short 0x4548
00004545  FF4606            inc word [bp+0x6]
00004548  8B5E06            mov bx,[bp+0x6]
0000454B  D1E3              shl bx,1
0000454D  39B7CA14          cmp [bx+0x14ca],si
00004551  76F2              jna 0x4545
00004553  EB21              jmp short 0x4576
00004555  837E0603          cmp word [bp+0x6],byte +0x3
00004559  720B              jc 0x4566
0000455B  8B4604            mov ax,[bp+0x4]
0000455E  054600            add ax,0x46
00004561  A90300            test ax,0x3
00004564  7403              jz 0x4569
00004566  FF4E08            dec word [bp+0x8]
00004569  8B5E06            mov bx,[bp+0x6]
0000456C  4B                dec bx
0000456D  D1E3              shl bx,1
0000456F  8B87CA14          mov ax,[bx+0x14ca]
00004573  014608            add [bp+0x8],ax
00004576  837E0604          cmp word [bp+0x6],byte +0x4
0000457A  727B              jc 0x45f7
0000457C  7408              jz 0x4586
0000457E  837E060A          cmp word [bp+0x6],byte +0xa
00004582  7773              ja 0x45f7
00004584  756C              jnz 0x45f2
00004586  8B5E06            mov bx,[bp+0x6]
00004589  D1E3              shl bx,1
0000458B  837E0410          cmp word [bp+0x4],byte +0x10
0000458F  7E0F              jng 0x45a0
00004591  837E0604          cmp word [bp+0x6],byte +0x4
00004595  7509              jnz 0x45a0
00004597  8B8FC814          mov cx,[bx+0x14c8]
0000459B  83C107            add cx,byte +0x7
0000459E  EB04              jmp short 0x45a4
000045A0  8B8FCA14          mov cx,[bx+0x14ca]
000045A4  8B5E04            mov bx,[bp+0x4]
000045A7  81C3B207          add bx,0x7b2
000045AB  F6C303            test bl,0x3
000045AE  7401              jz 0x45b1
000045B0  49                dec cx
000045B1  8B5E04            mov bx,[bp+0x4]
000045B4  43                inc bx
000045B5  D1FB              sar bx,1
000045B7  D1FB              sar bx,1
000045B9  03D9              add bx,cx
000045BB  B86D01            mov ax,0x16d
000045BE  F76604            mul word [bp+0x4]
000045C1  03C3              add ax,bx
000045C3  050400            add ax,0x4
000045C6  33D2              xor dx,dx
000045C8  BB0700            mov bx,0x7
000045CB  F7F3              div bx
000045CD  2BCA              sub cx,dx
000045CF  8B4608            mov ax,[bp+0x8]
000045D2  837E0604          cmp word [bp+0x6],byte +0x4
000045D6  750E              jnz 0x45e6
000045D8  3BC1              cmp ax,cx
000045DA  7716              ja 0x45f2
000045DC  7519              jnz 0x45f7
000045DE  807E0A02          cmp byte [bp+0xa],0x2
000045E2  7213              jc 0x45f7
000045E4  EB0C              jmp short 0x45f2
000045E6  3BC1              cmp ax,cx
000045E8  7208              jc 0x45f2
000045EA  750B              jnz 0x45f7
000045EC  807E0A01          cmp byte [bp+0xa],0x1
000045F0  7705              ja 0x45f7
000045F2  B80100            mov ax,0x1
000045F5  EB02              jmp short 0x45f9
000045F7  33C0              xor ax,ax
000045F9  5E                pop si
000045FA  5D                pop bp
000045FB  C20800            ret 0x8

; ---- 0000:45FE ----
000045FE  55                push bp
000045FF  8BEC              mov bp,sp
00004601  1E                push ds
00004602  B441              mov ah,0x41
00004604  C55606            lds dx,[bp+0x6]
00004607  CD21              int 0x21
00004609  1F                pop ds
0000460A  7204              jc 0x4610
0000460C  33C0              xor ax,ax
0000460E  EB04              jmp short 0x4614
00004610  50                push ax
00004611  E8DDF7            call 0x3df1
00004614  5D                pop bp
00004615  CB                retf
00004616  8AC6              mov al,dh
00004618  E80200            call 0x461d
0000461B  8AC2              mov al,dl
0000461D  D410              aam 0x10
0000461F  86E0              xchg ah,al
00004621  E80200            call 0x4626
00004624  86E0              xchg ah,al
00004626  0490              add al,0x90
00004628  27                daa
00004629  1440              adc al,0x40
0000462B  27                daa
0000462C  AA                stosb
0000462D  C3                ret

; ---- 0000:462E ----
0000462E  55                push bp
0000462F  8BEC              mov bp,sp
00004631  81EC9600          sub sp,0x96
00004635  56                push si
00004636  57                push di
00004637  C746EE0000        mov word [bp-0x12],0x0
0000463C  C746EC5000        mov word [bp-0x14],0x50
00004641  C746EA0000        mov word [bp-0x16],0x0
00004646  EB4A              jmp short 0x4692
00004648  57                push di
00004649  B9FFFF            mov cx,0xffff
0000464C  32C0              xor al,al
0000464E  F2AE              repne scasb
00004650  F7D1              not cx
00004652  49                dec cx
00004653  5F                pop di
00004654  C3                ret
00004655  368805            mov [ss:di],al
00004658  47                inc di
00004659  FE4EEC            dec byte [bp-0x14]
0000465C  7E33              jng 0x4691
0000465E  53                push bx
0000465F  51                push cx
00004660  52                push dx
00004661  06                push es
00004662  8D866AFF          lea ax,[bp-0x96]
00004666  2BF8              sub di,ax
00004668  16                push ss
00004669  8D866AFF          lea ax,[bp-0x96]
0000466D  50                push ax
0000466E  57                push di
0000466F  FF760C            push word [bp+0xc]
00004672  FF760A            push word [bp+0xa]
00004675  FF560E            call [bp+0xe]
00004678  0BC0              or ax,ax
0000467A  7505              jnz 0x4681
0000467C  C746EA0100        mov word [bp-0x16],0x1
00004681  C746EC5000        mov word [bp-0x14],0x50
00004686  017EEE            add [bp-0x12],di
00004689  8DBE6AFF          lea di,[bp-0x96]
0000468D  07                pop es
0000468E  5A                pop dx
0000468F  59                pop cx
00004690  5B                pop bx
00004691  C3                ret
00004692  06                push es
00004693  FC                cld
00004694  8DBE6AFF          lea di,[bp-0x96]
00004698  897EFC            mov [bp-0x4],di
0000469B  8B7EFC            mov di,[bp-0x4]
0000469E  C47606            les si,[bp+0x6]
000046A1  26AC              es lodsb
000046A3  0AC0              or al,al
000046A5  7412              jz 0x46b9
000046A7  3C25              cmp al,0x25
000046A9  7411              jz 0x46bc
000046AB  368805            mov [ss:di],al
000046AE  47                inc di
000046AF  FE4EEC            dec byte [bp-0x14]
000046B2  7FED              jg 0x46a1
000046B4  E8A7FF            call 0x465e
000046B7  EBE8              jmp short 0x46a1
000046B9  E9F703            jmp 0x4ab3
000046BC  8976F0            mov [bp-0x10],si
000046BF  26AC              es lodsb
000046C1  3C25              cmp al,0x25
000046C3  74E6              jz 0x46ab
000046C5  897EFC            mov [bp-0x4],di
000046C8  33C9              xor cx,cx
000046CA  894EF2            mov [bp-0xe],cx
000046CD  C746FE2000        mov word [bp-0x2],0x20
000046D2  884EF5            mov [bp-0xb],cl
000046D5  C746F8FFFF        mov word [bp-0x8],0xffff
000046DA  C746F6FFFF        mov word [bp-0xa],0xffff
000046DF  EB02              jmp short 0x46e3
000046E1  26AC              es lodsb
000046E3  32E4              xor ah,ah
000046E5  8BD0              mov dx,ax
000046E7  8BD8              mov bx,ax
000046E9  80EB20            sub bl,0x20
000046EC  80FB60            cmp bl,0x60
000046EF  7313              jnc 0x4704
000046F1  8A9F0515          mov bl,[bx+0x1505]
000046F5  83FB17            cmp bx,byte +0x17
000046F8  7603              jna 0x46fd
000046FA  E9A203            jmp 0x4a9f
000046FD  D1E3              shl bx,1
000046FF  2EFFA7D34A        jmp [cs:bx+0x4ad3]
00004704  E99803            jmp 0x4a9f
00004707  80FD00            cmp ch,0x0
0000470A  77F8              ja 0x4704
0000470C  834EFE01          or word [bp-0x2],byte +0x1
00004710  EBCF              jmp short 0x46e1
00004712  80FD00            cmp ch,0x0
00004715  77ED              ja 0x4704
00004717  834EFE02          or word [bp-0x2],byte +0x2
0000471B  EBC4              jmp short 0x46e1
0000471D  80FD00            cmp ch,0x0
00004720  77E2              ja 0x4704
00004722  807EF52B          cmp byte [bp-0xb],0x2b
00004726  7403              jz 0x472b
00004728  8856F5            mov [bp-0xb],dl
0000472B  EBB4              jmp short 0x46e1
0000472D  8366FEDF          and word [bp-0x2],byte -0x21
00004731  EB04              jmp short 0x4737
00004733  834EFE20          or word [bp-0x2],byte +0x20
00004737  B505              mov ch,0x5
00004739  EBA6              jmp short 0x46e1
0000473B  80FD00            cmp ch,0x0
0000473E  774A              ja 0x478a
00004740  F746FE0200        test word [bp-0x2],0x2
00004745  7529              jnz 0x4770
00004747  834EFE08          or word [bp-0x2],byte +0x8
0000474B  B501              mov ch,0x1
0000474D  EB92              jmp short 0x46e1
0000474F  E94D03            jmp 0x4a9f
00004752  8B7E04            mov di,[bp+0x4]
00004755  368B05            mov ax,[ss:di]
00004758  83460402          add word [bp+0x4],byte +0x2
0000475C  80FD02            cmp ch,0x2
0000475F  7312              jnc 0x4773
00004761  0BC0              or ax,ax
00004763  7906              jns 0x476b
00004765  F7D8              neg ax
00004767  834EFE02          or word [bp-0x2],byte +0x2
0000476B  8946F8            mov [bp-0x8],ax
0000476E  B503              mov ch,0x3
00004770  E96EFF            jmp 0x46e1
00004773  80FD04            cmp ch,0x4
00004776  75D7              jnz 0x474f
00004778  8946F6            mov [bp-0xa],ax
0000477B  FEC5              inc ch
0000477D  E961FF            jmp 0x46e1
00004780  80FD04            cmp ch,0x4
00004783  73CA              jnc 0x474f
00004785  B504              mov ch,0x4
00004787  E957FF            jmp 0x46e1
0000478A  92                xchg ax,dx
0000478B  2C30              sub al,0x30
0000478D  98                cbw
0000478E  80FD02            cmp ch,0x2
00004791  7719              ja 0x47ac
00004793  B502              mov ch,0x2
00004795  8746F8            xchg ax,[bp-0x8]
00004798  0BC0              or ax,ax
0000479A  7CD4              jl 0x4770
0000479C  D1E0              shl ax,1
0000479E  8BD0              mov dx,ax
000047A0  D1E0              shl ax,1
000047A2  D1E0              shl ax,1
000047A4  03C2              add ax,dx
000047A6  0146F8            add [bp-0x8],ax
000047A9  E935FF            jmp 0x46e1
000047AC  80FD04            cmp ch,0x4
000047AF  759E              jnz 0x474f
000047B1  8746F6            xchg ax,[bp-0xa]
000047B4  0BC0              or ax,ax
000047B6  7CB8              jl 0x4770
000047B8  D1E0              shl ax,1
000047BA  8BD0              mov dx,ax
000047BC  D1E0              shl ax,1
000047BE  D1E0              shl ax,1
000047C0  03C2              add ax,dx
000047C2  0146F6            add [bp-0xa],ax
000047C5  E919FF            jmp 0x46e1
000047C8  834EFE10          or word [bp-0x2],byte +0x10
000047CC  E968FF            jmp 0x4737
000047CF  814EFE0001        or word [bp-0x2],0x100
000047D4  8366FEEF          and word [bp-0x2],byte -0x11
000047D8  E95CFF            jmp 0x4737
000047DB  B708              mov bh,0x8
000047DD  EB0A              jmp short 0x47e9
000047DF  B70A              mov bh,0xa
000047E1  EB0A              jmp short 0x47ed
000047E3  B710              mov bh,0x10
000047E5  B3E9              mov bl,0xe9
000047E7  02DA              add bl,dl
000047E9  C646F500          mov byte [bp-0xb],0x0
000047ED  8856FB            mov [bp-0x5],dl
000047F0  33D2              xor dx,dx
000047F2  8856FA            mov [bp-0x6],dl
000047F5  8B7E04            mov di,[bp+0x4]
000047F8  368B05            mov ax,[ss:di]
000047FB  EB10              jmp short 0x480d
000047FD  B70A              mov bh,0xa
000047FF  C646FA01          mov byte [bp-0x6],0x1
00004803  8856FB            mov [bp-0x5],dl
00004806  8B7E04            mov di,[bp+0x4]
00004809  368B05            mov ax,[ss:di]
0000480C  99                cwd
0000480D  47                inc di
0000480E  47                inc di
0000480F  897606            mov [bp+0x6],si
00004812  F746FE1000        test word [bp-0x2],0x10
00004817  7405              jz 0x481e
00004819  368B15            mov dx,[ss:di]
0000481C  47                inc di
0000481D  47                inc di
0000481E  897E04            mov [bp+0x4],di
00004821  8D7EBB            lea di,[bp-0x45]
00004824  0BC0              or ax,ax
00004826  752F              jnz 0x4857
00004828  0BD2              or dx,dx
0000482A  752B              jnz 0x4857
0000482C  837EF600          cmp word [bp-0xa],byte +0x0
00004830  7529              jnz 0x485b
00004832  8B7EFC            mov di,[bp-0x4]
00004835  8B4EF8            mov cx,[bp-0x8]
00004838  E31A              jcxz 0x4854
0000483A  83F9FF            cmp cx,byte -0x1
0000483D  7415              jz 0x4854
0000483F  8B46FE            mov ax,[bp-0x2]
00004842  250800            and ax,0x8
00004845  7404              jz 0x484b
00004847  B230              mov dl,0x30
00004849  EB02              jmp short 0x484d
0000484B  B220              mov dl,0x20
0000484D  8AC2              mov al,dl
0000484F  E803FE            call 0x4655
00004852  E2F9              loop 0x484d
00004854  E947FE            jmp 0x469e
00004857  834EFE04          or word [bp-0x2],byte +0x4
0000485B  52                push dx
0000485C  50                push ax
0000485D  16                push ss
0000485E  57                push di
0000485F  8AC7              mov al,bh
00004861  98                cbw
00004862  50                push ax
00004863  8A46FA            mov al,[bp-0x6]
00004866  50                push ax
00004867  53                push bx
00004868  E8FDF5            call 0x3e68
0000486B  16                push ss
0000486C  07                pop es
0000486D  8B56F6            mov dx,[bp-0xa]
00004870  0BD2              or dx,dx
00004872  7F03              jg 0x4877
00004874  E9F400            jmp 0x496b
00004877  E9FF00            jmp 0x4979
0000487A  8856FB            mov [bp-0x5],dl
0000487D  897606            mov [bp+0x6],si
00004880  8D7EBA            lea di,[bp-0x46]
00004883  8B5E04            mov bx,[bp+0x4]
00004886  36FF37            push word [ss:bx]
00004889  43                inc bx
0000488A  43                inc bx
0000488B  895E04            mov [bp+0x4],bx
0000488E  F746FE2000        test word [bp-0x2],0x20
00004893  7410              jz 0x48a5
00004895  368B17            mov dx,[ss:bx]
00004898  43                inc bx
00004899  43                inc bx
0000489A  895E04            mov [bp+0x4],bx
0000489D  16                push ss
0000489E  07                pop es
0000489F  E874FD            call 0x4616
000048A2  B03A              mov al,0x3a
000048A4  AA                stosb
000048A5  16                push ss
000048A6  07                pop es
000048A7  5A                pop dx
000048A8  E86BFD            call 0x4616
000048AB  36C60500          mov byte [ss:di],0x0
000048AF  C646FA00          mov byte [bp-0x6],0x0
000048B3  8366FEFB          and word [bp-0x2],byte -0x5
000048B7  8D4EBA            lea cx,[bp-0x46]
000048BA  2BF9              sub di,cx
000048BC  87CF              xchg cx,di
000048BE  8B56F6            mov dx,[bp-0xa]
000048C1  3BD1              cmp dx,cx
000048C3  7F02              jg 0x48c7
000048C5  8BD1              mov dx,cx
000048C7  E9A100            jmp 0x496b
000048CA  897606            mov [bp+0x6],si
000048CD  8856FB            mov [bp-0x5],dl
000048D0  8B7E04            mov di,[bp+0x4]
000048D3  368B05            mov ax,[ss:di]
000048D6  83460402          add word [bp+0x4],byte +0x2
000048DA  16                push ss
000048DB  07                pop es
000048DC  8D7EBB            lea di,[bp-0x45]
000048DF  32E4              xor ah,ah
000048E1  368905            mov [ss:di],ax
000048E4  B90100            mov cx,0x1
000048E7  E9BC00            jmp 0x49a6
000048EA  897606            mov [bp+0x6],si
000048ED  8856FB            mov [bp-0x5],dl
000048F0  8B7E04            mov di,[bp+0x4]
000048F3  F746FE2000        test word [bp-0x2],0x20
000048F8  750D              jnz 0x4907
000048FA  368B3D            mov di,[ss:di]
000048FD  83460402          add word [bp+0x4],byte +0x2
00004901  1E                push ds
00004902  07                pop es
00004903  0BFF              or di,di
00004905  EB0B              jmp short 0x4912
00004907  36C43D            les di,[ss:di]
0000490A  83460404          add word [bp+0x4],byte +0x4
0000490E  8CC0              mov ax,es
00004910  0BC7              or ax,di
00004912  7505              jnz 0x4919
00004914  1E                push ds
00004915  07                pop es
00004916  BFFE14            mov di,0x14fe
00004919  E82CFD            call 0x4648
0000491C  3B4EF6            cmp cx,[bp-0xa]
0000491F  7603              jna 0x4924
00004921  8B4EF6            mov cx,[bp-0xa]
00004924  E97F00            jmp 0x49a6
00004927  897606            mov [bp+0x6],si
0000492A  8856FB            mov [bp-0x5],dl
0000492D  8B7E04            mov di,[bp+0x4]
00004930  8B4EF6            mov cx,[bp-0xa]
00004933  0BC9              or cx,cx
00004935  7D03              jnl 0x493a
00004937  B90600            mov cx,0x6
0000493A  16                push ss
0000493B  57                push di
0000493C  51                push cx
0000493D  16                push ss
0000493E  8D5EBB            lea bx,[bp-0x45]
00004941  53                push bx
00004942  52                push dx
00004943  B80100            mov ax,0x1
00004946  2346FE            and ax,[bp-0x2]
00004949  50                push ax
0000494A  8B46FE            mov ax,[bp-0x2]
0000494D  A90001            test ax,0x100
00004950  7409              jz 0x495b
00004952  B80800            mov ax,0x8
00004955  8346040A          add word [bp+0x4],byte +0xa
00004959  EB07              jmp short 0x4962
0000495B  83460408          add word [bp+0x4],byte +0x8
0000495F  B80600            mov ax,0x6
00004962  50                push ax
00004963  E87708            call 0x51dd
00004966  16                push ss
00004967  07                pop es
00004968  8D7EBB            lea di,[bp-0x45]
0000496B  F746FE0800        test word [bp-0x2],0x8
00004970  7418              jz 0x498a
00004972  8B56F8            mov dx,[bp-0x8]
00004975  0BD2              or dx,dx
00004977  7E11              jng 0x498a
00004979  E8CCFC            call 0x4648
0000497C  26803D2D          cmp byte [es:di],0x2d
00004980  7501              jnz 0x4983
00004982  49                dec cx
00004983  2BD1              sub dx,cx
00004985  7E03              jng 0x498a
00004987  8956F2            mov [bp-0xe],dx
0000498A  8A46F5            mov al,[bp-0xb]
0000498D  0AC0              or al,al
0000498F  7412              jz 0x49a3
00004991  26803D2D          cmp byte [es:di],0x2d
00004995  740C              jz 0x49a3
00004997  836EF201          sub word [bp-0xe],byte +0x1
0000499B  8356F200          adc word [bp-0xe],byte +0x0
0000499F  4F                dec di
000049A0  268805            mov [es:di],al
000049A3  E8A2FC            call 0x4648
000049A6  8BF7              mov si,di
000049A8  8B7EFC            mov di,[bp-0x4]
000049AB  8B5EF8            mov bx,[bp-0x8]
000049AE  B80500            mov ax,0x5
000049B1  2346FE            and ax,[bp-0x2]
000049B4  3D0500            cmp ax,0x5
000049B7  7513              jnz 0x49cc
000049B9  8A66FB            mov ah,[bp-0x5]
000049BC  80FC6F            cmp ah,0x6f
000049BF  750D              jnz 0x49ce
000049C1  837EF200          cmp word [bp-0xe],byte +0x0
000049C5  7F05              jg 0x49cc
000049C7  C746F20100        mov word [bp-0xe],0x1
000049CC  EB1B              jmp short 0x49e9
000049CE  80FC78            cmp ah,0x78
000049D1  7405              jz 0x49d8
000049D3  80FC58            cmp ah,0x58
000049D6  7511              jnz 0x49e9
000049D8  834EFE40          or word [bp-0x2],byte +0x40
000049DC  4B                dec bx
000049DD  4B                dec bx
000049DE  836EF202          sub word [bp-0xe],byte +0x2
000049E2  7D05              jnl 0x49e9
000049E4  C746F20000        mov word [bp-0xe],0x0
000049E9  034EF2            add cx,[bp-0xe]
000049EC  F746FE0200        test word [bp-0x2],0x2
000049F1  750C              jnz 0x49ff
000049F3  EB06              jmp short 0x49fb
000049F5  B020              mov al,0x20
000049F7  E85BFC            call 0x4655
000049FA  4B                dec bx
000049FB  3BD9              cmp bx,cx
000049FD  7FF6              jg 0x49f5
000049FF  F746FE4000        test word [bp-0x2],0x40
00004A04  740B              jz 0x4a11
00004A06  B030              mov al,0x30
00004A08  E84AFC            call 0x4655
00004A0B  8A46FB            mov al,[bp-0x5]
00004A0E  E844FC            call 0x4655
00004A11  8B56F2            mov dx,[bp-0xe]
00004A14  0BD2              or dx,dx
00004A16  7E27              jng 0x4a3f
00004A18  2BCA              sub cx,dx
00004A1A  2BDA              sub bx,dx
00004A1C  268A04            mov al,[es:si]
00004A1F  3C2D              cmp al,0x2d
00004A21  7408              jz 0x4a2b
00004A23  3C20              cmp al,0x20
00004A25  7404              jz 0x4a2b
00004A27  3C2B              cmp al,0x2b
00004A29  7507              jnz 0x4a32
00004A2B  26AC              es lodsb
00004A2D  E825FC            call 0x4655
00004A30  49                dec cx
00004A31  4B                dec bx
00004A32  87CA              xchg cx,dx
00004A34  E307              jcxz 0x4a3d
00004A36  B030              mov al,0x30
00004A38  E81AFC            call 0x4655
00004A3B  E2F9              loop 0x4a36
00004A3D  87CA              xchg cx,dx
00004A3F  E312              jcxz 0x4a53
00004A41  2BD9              sub bx,cx
00004A43  26AC              es lodsb
00004A45  368805            mov [ss:di],al
00004A48  47                inc di
00004A49  FE4EEC            dec byte [bp-0x14]
00004A4C  7F03              jg 0x4a51
00004A4E  E80DFC            call 0x465e
00004A51  E2F0              loop 0x4a43
00004A53  0BDB              or bx,bx
00004A55  7E09              jng 0x4a60
00004A57  8BCB              mov cx,bx
00004A59  B020              mov al,0x20
00004A5B  E8F7FB            call 0x4655
00004A5E  E2F9              loop 0x4a59
00004A60  E93BFC            jmp 0x469e
00004A63  897606            mov [bp+0x6],si
00004A66  8B7E04            mov di,[bp+0x4]
00004A69  F746FE2000        test word [bp-0x2],0x20
00004A6E  750B              jnz 0x4a7b
00004A70  368B3D            mov di,[ss:di]
00004A73  83460402          add word [bp+0x4],byte +0x2
00004A77  1E                push ds
00004A78  07                pop es
00004A79  EB07              jmp short 0x4a82
00004A7B  36C43D            les di,[ss:di]
00004A7E  83460404          add word [bp+0x4],byte +0x4
00004A82  B85000            mov ax,0x50
00004A85  2A46EC            sub al,[bp-0x14]
00004A88  0346EE            add ax,[bp-0x12]
00004A8B  268905            mov [es:di],ax
00004A8E  F746FE1000        test word [bp-0x2],0x10
00004A93  7407              jz 0x4a9c
00004A95  47                inc di
00004A96  47                inc di
00004A97  26C7050000        mov word [es:di],0x0
00004A9C  E9FCFB            jmp 0x469b
00004A9F  8B76F0            mov si,[bp-0x10]
00004AA2  8E4608            mov es,[bp+0x8]
00004AA5  8B7EFC            mov di,[bp-0x4]
00004AA8  B025              mov al,0x25
00004AAA  E8A8FB            call 0x4655
00004AAD  26AC              es lodsb
00004AAF  0AC0              or al,al
00004AB1  75F7              jnz 0x4aaa
00004AB3  807EEC50          cmp byte [bp-0x14],0x50
00004AB7  7D03              jnl 0x4abc
00004AB9  E8A2FB            call 0x465e
00004ABC  07                pop es
00004ABD  837EEA00          cmp word [bp-0x16],byte +0x0
00004AC1  7405              jz 0x4ac8
00004AC3  B8FFFF            mov ax,0xffff
00004AC6  EB03              jmp short 0x4acb
00004AC8  8B46EE            mov ax,[bp-0x12]
00004ACB  5F                pop di
00004ACC  5E                pop si
00004ACD  8BE5              mov sp,bp
00004ACF  5D                pop bp
00004AD0  C20C00            ret 0xc
00004AD3  1D4707            sbb ax,0x747
00004AD6  47                inc di
00004AD7  52                push dx
00004AD8  47                inc di
00004AD9  124780            adc al,[bx-0x80]
00004ADC  47                inc di
00004ADD  8A47C8            mov al,[bx-0x38]
00004AE0  47                inc di
00004AE1  CF                iret
00004AE2  47                inc di
00004AE3  D447              aam 0x47
00004AE5  3B47FD            cmp ax,[bx-0x3]
00004AE8  47                inc di
00004AE9  DB47DF            fild dword [bx-0x21]
00004AEC  47                inc di
00004AED  E347              jcxz 0x4b36
00004AEF  7A48              jpe 0x4b39
00004AF1  27                daa
00004AF2  49                dec cx
00004AF3  CA48EA            retf 0xea48
00004AF6  48                dec ax
00004AF7  634A9F            arpl [bp+si-0x61],cx
00004AFA  4A                dec dx
00004AFB  9F                lahf
00004AFC  4A                dec dx
00004AFD  9F                lahf
00004AFE  4A                dec dx
00004AFF  2D4733            sub ax,0x3347
00004B02  47                inc di

; ---- 0000:4B03 ----
00004B03  55                push bp
00004B04  8BEC              mov bp,sp
00004B06  8B5E06            mov bx,[bp+0x6]
00004B09  D1E3              shl bx,1
00004B0B  F78724140008      test word [bx+0x1424],0x800
00004B11  7415              jz 0x4b28
00004B13  B80200            mov ax,0x2
00004B16  50                push ax
00004B17  33C0              xor ax,ax
00004B19  33D2              xor dx,dx
00004B1B  50                push ax
00004B1C  52                push dx
00004B1D  FF7606            push word [bp+0x6]
00004B20  9A3F3E0000        call 0x0:0x3e3f
00004B25  83C408            add sp,byte +0x8
00004B28  1E                push ds
00004B29  B440              mov ah,0x40
00004B2B  8B5E06            mov bx,[bp+0x6]
00004B2E  8B4E0C            mov cx,[bp+0xc]
00004B31  C55608            lds dx,[bp+0x8]
00004B34  CD21              int 0x21
00004B36  1F                pop ds
00004B37  720F              jc 0x4b48
00004B39  50                push ax
00004B3A  8B5E06            mov bx,[bp+0x6]
00004B3D  D1E3              shl bx,1
00004B3F  818F24140010      or word [bx+0x1424],0x1000
00004B45  58                pop ax
00004B46  EB04              jmp short 0x4b4c
00004B48  50                push ax
00004B49  E8A5F2            call 0x3df1
00004B4C  5D                pop bp
00004B4D  CB                retf
00004B4E  0000              add [bx+si],al
00004B50  0000              add [bx+si],al
00004B52  0000              add [bx+si],al
00004B54  0000              add [bx+si],al
00004B56  0000              add [bx+si],al
00004B58  0000              add [bx+si],al
00004B5A  0000              add [bx+si],al
00004B5C  2E3B16504B        cmp dx,[cs:0x4b50]
00004B61  7437              jz 0x4b9a
00004B63  8EDA              mov ds,dx
00004B65  8E1E0200          mov ds,[0x2]
00004B69  833E020000        cmp word [0x2],byte +0x0
00004B6E  7407              jz 0x4b77
00004B70  2E8C1E524B        mov [cs:0x4b52],ds
00004B75  EB38              jmp short 0x4baf
00004B77  8CD8              mov ax,ds
00004B79  2E3B06504B        cmp ax,[cs:0x4b50]
00004B7E  7415              jz 0x4b95
00004B80  A10800            mov ax,[0x8]
00004B83  2EA3524B          mov [cs:0x4b52],ax
00004B87  1E                push ds
00004B88  33C0              xor ax,ax
00004B8A  50                push ax
00004B8B  E8A100            call 0x4c2f
00004B8E  2E8E1E564B        mov ds,[cs:0x4b56]
00004B93  EB23              jmp short 0x4bb8
00004B95  2E8B16504B        mov dx,[cs:0x4b50]
00004B9A  2EC706504B0000    mov word [cs:0x4b50],0x0
00004BA1  2EC706524B0000    mov word [cs:0x4b52],0x0
00004BA8  2EC706544B0000    mov word [cs:0x4b54],0x0
00004BAF  2E8E1E564B        mov ds,[cs:0x4b56]
00004BB4  52                push dx
00004BB5  33C0              xor ax,ax
00004BB7  50                push ax
00004BB8  E8F4DB            call 0x27af
00004BBB  58                pop ax
00004BBC  58                pop ax
00004BBD  C3                ret
00004BBE  8EDA              mov ds,dx
00004BC0  1E                push ds
00004BC1  8E060200          mov es,[0x2]
00004BC5  C70602000000      mov word [0x2],0x0
00004BCB  8C060800          mov [0x8],es
00004BCF  2E3B16504B        cmp dx,[cs:0x4b50]
00004BD4  742E              jz 0x4c04
00004BD6  26833E020000      cmp word [es:0x2],byte +0x0
00004BDC  7526              jnz 0x4c04
00004BDE  A10000            mov ax,[0x0]
00004BE1  5B                pop bx
00004BE2  06                push es
00004BE3  2601060000        add [es:0x0],ax
00004BE8  8CC1              mov cx,es
00004BEA  03D0              add dx,ax
00004BEC  8EC2              mov es,dx
00004BEE  26833E020000      cmp word [es:0x2],byte +0x0
00004BF4  7507              jnz 0x4bfd
00004BF6  26890E0800        mov [es:0x8],cx
00004BFB  EB0A              jmp short 0x4c07
00004BFD  26890E0200        mov [es:0x2],cx
00004C02  EB03              jmp short 0x4c07
00004C04  E85100            call 0x4c58
00004C07  07                pop es
00004C08  8CC0              mov ax,es
00004C0A  2603060000        add ax,[es:0x0]
00004C0F  8ED8              mov ds,ax
00004C11  833E020000        cmp word [0x2],byte +0x0
00004C16  7401              jz 0x4c19
00004C18  C3                ret
00004C19  A10000            mov ax,[0x0]
00004C1C  2601060000        add [es:0x0],ax
00004C21  8CC0              mov ax,es
00004C23  8CDB              mov bx,ds
00004C25  031E0000          add bx,[0x0]
00004C29  8EC3              mov es,bx
00004C2B  26A30200          mov [es:0x2],ax
00004C2F  8CDB              mov bx,ds
00004C31  3B1E0600          cmp bx,[0x6]
00004C35  7419              jz 0x4c50
00004C37  8E060600          mov es,[0x6]
00004C3B  8E1E0400          mov ds,[0x4]
00004C3F  8C060600          mov [0x6],es
00004C43  268C1E0400        mov [es:0x4],ds
00004C48  2E8C1E544B        mov [cs:0x4b54],ds
00004C4D  8EDB              mov ds,bx
00004C4F  C3                ret
00004C50  2EC706544B0000    mov word [cs:0x4b54],0x0
00004C57  C3                ret
00004C58  2EA1544B          mov ax,[cs:0x4b54]
00004C5C  0BC0              or ax,ax
00004C5E  7420              jz 0x4c80
00004C60  8CD3              mov bx,ss
00004C62  FA                cli
00004C63  8ED0              mov ss,ax
00004C65  368E060600        mov es,[ss:0x6]
00004C6A  368C1E0600        mov [ss:0x6],ds
00004C6F  8C160400          mov [0x4],ss
00004C73  8ED3              mov ss,bx
00004C75  FB                sti
00004C76  268C1E0400        mov [es:0x4],ds
00004C7B  8C060600          mov [0x6],es
00004C7F  C3                ret
00004C80  2E8C1E544B        mov [cs:0x4b54],ds
00004C85  8C1E0400          mov [0x4],ds
00004C89  8C1E0600          mov [0x6],ds
00004C8D  C3                ret

; ---- 0000:4C8E ----
00004C8E  55                push bp
00004C8F  8BEC              mov bp,sp
00004C91  56                push si
00004C92  57                push di
00004C93  2E8C1E564B        mov [cs:0x4b56],ds
00004C98  8B5608            mov dx,[bp+0x8]
00004C9B  0BD2              or dx,dx
00004C9D  740F              jz 0x4cae
00004C9F  2E3B16524B        cmp dx,[cs:0x4b52]
00004CA4  7505              jnz 0x4cab
00004CA6  E8B3FE            call 0x4b5c
00004CA9  EB03              jmp short 0x4cae
00004CAB  E810FF            call 0x4bbe
00004CAE  2E8E1E564B        mov ds,[cs:0x4b56]
00004CB3  5F                pop di
00004CB4  5E                pop si
00004CB5  5D                pop bp
00004CB6  CB                retf
00004CB7  50                push ax
00004CB8  2E8E1E564B        mov ds,[cs:0x4b56]
00004CBD  33C0              xor ax,ax
00004CBF  50                push ax
00004CC0  50                push ax
00004CC1  E82ADB            call 0x27ee
00004CC4  5B                pop bx
00004CC5  5B                pop bx
00004CC6  250F00            and ax,0xf
00004CC9  7413              jz 0x4cde
00004CCB  BA1000            mov dx,0x10
00004CCE  2BD0              sub dx,ax
00004CD0  33C0              xor ax,ax
00004CD2  2E8E1E564B        mov ds,[cs:0x4b56]
00004CD7  50                push ax
00004CD8  52                push dx
00004CD9  E812DB            call 0x27ee
00004CDC  5B                pop bx
00004CDD  5B                pop bx
00004CDE  58                pop ax
00004CDF  50                push ax
00004CE0  33DB              xor bx,bx
00004CE2  8ADC              mov bl,ah
00004CE4  B104              mov cl,0x4
00004CE6  D3EB              shr bx,cl
00004CE8  D3E0              shl ax,cl
00004CEA  2E8E1E564B        mov ds,[cs:0x4b56]
00004CEF  53                push bx
00004CF0  50                push ax
00004CF1  E8FADA            call 0x27ee
00004CF4  5B                pop bx
00004CF5  5B                pop bx
00004CF6  5B                pop bx
00004CF7  3DFFFF            cmp ax,0xffff
00004CFA  7418              jz 0x4d14
00004CFC  2E8916504B        mov [cs:0x4b50],dx
00004D01  2E8916524B        mov [cs:0x4b52],dx
00004D06  8EDA              mov ds,dx
00004D08  891E0000          mov [0x0],bx
00004D0C  89160200          mov [0x2],dx
00004D10  B80400            mov ax,0x4
00004D13  C3                ret
00004D14  33C0              xor ax,ax
00004D16  99                cwd
00004D17  C3                ret
00004D18  50                push ax
00004D19  33DB              xor bx,bx
00004D1B  8ADC              mov bl,ah
00004D1D  B104              mov cl,0x4
00004D1F  D3EB              shr bx,cl
00004D21  D3E0              shl ax,cl
00004D23  2E8E1E564B        mov ds,[cs:0x4b56]
00004D28  53                push bx
00004D29  50                push ax
00004D2A  E8C1DA            call 0x27ee
00004D2D  5B                pop bx
00004D2E  5B                pop bx
00004D2F  5B                pop bx
00004D30  3DFFFF            cmp ax,0xffff
00004D33  7418              jz 0x4d4d
00004D35  2E8B0E524B        mov cx,[cs:0x4b52]
00004D3A  2E8916524B        mov [cs:0x4b52],dx
00004D3F  8EDA              mov ds,dx
00004D41  891E0000          mov [0x0],bx
00004D45  890E0200          mov [0x2],cx
00004D49  B80400            mov ax,0x4
00004D4C  C3                ret
00004D4D  33C0              xor ax,ax
00004D4F  99                cwd
00004D50  C3                ret
00004D51  8BDA              mov bx,dx
00004D53  29060000          sub [0x0],ax
00004D57  03160000          add dx,[0x0]
00004D5B  8EDA              mov ds,dx
00004D5D  A30000            mov [0x0],ax
00004D60  891E0200          mov [0x2],bx
00004D64  8BDA              mov bx,dx
00004D66  031E0000          add bx,[0x0]
00004D6A  8EDB              mov ds,bx
00004D6C  89160200          mov [0x2],dx
00004D70  B80400            mov ax,0x4
00004D73  C3                ret

; ---- 0000:4D74 ----
00004D74  55                push bp
00004D75  8BEC              mov bp,sp
00004D77  33D2              xor dx,dx
00004D79  8B4606            mov ax,[bp+0x6]
00004D7C  EB09              jmp short 0x4d87

; ---- 0000:4D7E ----
00004D7E  55                push bp
00004D7F  8BEC              mov bp,sp
00004D81  8B5608            mov dx,[bp+0x8]
00004D84  8B4606            mov ax,[bp+0x6]
00004D87  56                push si
00004D88  57                push di
00004D89  2E8C1E564B        mov [cs:0x4b56],ds
00004D8E  8BC8              mov cx,ax
00004D90  0BCA              or cx,dx
00004D92  745E              jz 0x4df2
00004D94  051300            add ax,0x13
00004D97  83D200            adc dx,byte +0x0
00004D9A  7241              jc 0x4ddd
00004D9C  F7C2F0FF          test dx,0xfff0
00004DA0  753B              jnz 0x4ddd
00004DA2  B104              mov cl,0x4
00004DA4  D3E8              shr ax,cl
00004DA6  D3E2              shl dx,cl
00004DA8  0AE2              or ah,dl
00004DAA  2E8B16504B        mov dx,[cs:0x4b50]
00004DAF  0BD2              or dx,dx
00004DB1  7420              jz 0x4dd3
00004DB3  2E8B16544B        mov dx,[cs:0x4b54]
00004DB8  0BD2              or dx,dx
00004DBA  7412              jz 0x4dce
00004DBC  8BDA              mov bx,dx
00004DBE  8EDA              mov ds,dx
00004DC0  39060000          cmp [0x0],ax
00004DC4  731C              jnc 0x4de2
00004DC6  8B160600          mov dx,[0x6]
00004DCA  3BD3              cmp dx,bx
00004DCC  75F0              jnz 0x4dbe
00004DCE  E847FF            call 0x4d18
00004DD1  EB1F              jmp short 0x4df2
00004DD3  E8E1FE            call 0x4cb7
00004DD6  EB1A              jmp short 0x4df2
00004DD8  E876FF            call 0x4d51
00004DDB  EB15              jmp short 0x4df2
00004DDD  33C0              xor ax,ax
00004DDF  99                cwd
00004DE0  EB10              jmp short 0x4df2
00004DE2  77F4              ja 0x4dd8
00004DE4  E848FE            call 0x4c2f
00004DE7  8B1E0800          mov bx,[0x8]
00004DEB  891E0200          mov [0x2],bx
00004DEF  B80400            mov ax,0x4
00004DF2  2E8E1E564B        mov ds,[cs:0x4b56]
00004DF7  5F                pop di
00004DF8  5E                pop si
00004DF9  5D                pop bp
00004DFA  CB                retf
00004DFB  53                push bx
00004DFC  2E8B36584B        mov si,[cs:0x4b58]
00004E01  56                push si
00004E02  2E8B365A4B        mov si,[cs:0x4b5a]
00004E07  56                push si
00004E08  9A7E4D0000        call 0x0:0x4d7e
00004E0D  5B                pop bx
00004E0E  5B                pop bx
00004E0F  0BD2              or dx,dx
00004E11  7502              jnz 0x4e15
00004E13  5B                pop bx
00004E14  C3                ret
00004E15  1F                pop ds
00004E16  8EC2              mov es,dx
00004E18  06                push es
00004E19  1E                push ds
00004E1A  53                push bx
00004E1B  8B160000          mov dx,[0x0]
00004E1F  FC                cld
00004E20  4A                dec dx
00004E21  BF0400            mov di,0x4
00004E24  8BF7              mov si,di
00004E26  B90600            mov cx,0x6
00004E29  F3A5              rep movsw
00004E2B  0BD2              or dx,dx
00004E2D  7437              jz 0x4e66
00004E2F  8CC0              mov ax,es
00004E31  40                inc ax
00004E32  8EC0              mov es,ax
00004E34  8CD8              mov ax,ds
00004E36  40                inc ax
00004E37  8ED8              mov ds,ax
00004E39  33FF              xor di,di
00004E3B  8BF7              mov si,di
00004E3D  8BCA              mov cx,dx
00004E3F  81F90010          cmp cx,0x1000
00004E43  7603              jna 0x4e48
00004E45  B90010            mov cx,0x1000
00004E48  D1E1              shl cx,1
00004E4A  D1E1              shl cx,1
00004E4C  D1E1              shl cx,1
00004E4E  F3A5              rep movsw
00004E50  81EA0010          sub dx,0x1000
00004E54  7610              jna 0x4e66
00004E56  8CC0              mov ax,es
00004E58  050010            add ax,0x1000
00004E5B  8EC0              mov es,ax
00004E5D  8CD8              mov ax,ds
00004E5F  050010            add ax,0x1000
00004E62  8ED8              mov ds,ax
00004E64  EBD3              jmp short 0x4e39
00004E66  2E8E1E564B        mov ds,[cs:0x4b56]
00004E6B  9A8E4C0000        call 0x0:0x4c8e
00004E70  5A                pop dx
00004E71  5A                pop dx
00004E72  5A                pop dx
00004E73  B80400            mov ax,0x4
00004E76  C3                ret
00004E77  2E3B1E524B        cmp bx,[cs:0x4b52]
00004E7C  7445              jz 0x4ec3
00004E7E  8BFB              mov di,bx
00004E80  03F8              add di,ax
00004E82  8EC7              mov es,di
00004E84  8BF1              mov si,cx
00004E86  2BF0              sub si,ax
00004E88  2689360000        mov [es:0x0],si
00004E8D  26891E0200        mov [es:0x2],bx
00004E92  06                push es
00004E93  50                push ax
00004E94  8EC3              mov es,bx
00004E96  26A30000          mov [es:0x0],ax
00004E9A  8BD3              mov dx,bx
00004E9C  03D1              add dx,cx
00004E9E  8EC2              mov es,dx
00004EA0  26833E020000      cmp word [es:0x2],byte +0x0
00004EA6  7407              jz 0x4eaf
00004EA8  26893E0200        mov [es:0x2],di
00004EAD  EB05              jmp short 0x4eb4
00004EAF  26893E0800        mov [es:0x8],di
00004EB4  8BF3              mov si,bx
00004EB6  9A8E4C0000        call 0x0:0x4c8e
00004EBB  5A                pop dx
00004EBC  5A                pop dx
00004EBD  8BD6              mov dx,si
00004EBF  B80400            mov ax,0x4
00004EC2  C3                ret
00004EC3  53                push bx
00004EC4  8EC3              mov es,bx
00004EC6  26A30000          mov [es:0x0],ax
00004ECA  03D8              add bx,ax
00004ECC  53                push bx
00004ECD  33C0              xor ax,ax
00004ECF  50                push ax
00004ED0  E8DCD8            call 0x27af
00004ED3  5A                pop dx
00004ED4  5A                pop dx
00004ED5  5A                pop dx
00004ED6  B80400            mov ax,0x4
00004ED9  C3                ret
00004EDA  55                push bp
00004EDB  8BEC              mov bp,sp
00004EDD  33D2              xor dx,dx
00004EDF  EB06              jmp short 0x4ee7
00004EE1  55                push bp
00004EE2  8BEC              mov bp,sp
00004EE4  8B560C            mov dx,[bp+0xc]
00004EE7  8B460A            mov ax,[bp+0xa]
00004EEA  8B5E08            mov bx,[bp+0x8]
00004EED  56                push si
00004EEE  57                push di
00004EEF  2E8C1E564B        mov [cs:0x4b56],ds
00004EF4  2E8916584B        mov [cs:0x4b58],dx
00004EF9  2EA35A4B          mov [cs:0x4b5a],ax
00004EFD  0BDB              or bx,bx
00004EFF  7432              jz 0x4f33
00004F01  8BC8              mov cx,ax
00004F03  0BCA              or cx,dx
00004F05  7435              jz 0x4f3c
00004F07  051300            add ax,0x13
00004F0A  83D200            adc dx,byte +0x0
00004F0D  B104              mov cl,0x4
00004F0F  D3E8              shr ax,cl
00004F11  D3E2              shl dx,cl
00004F13  0AE2              or ah,dl
00004F15  8EC3              mov es,bx
00004F17  268B0E0000        mov cx,[es:0x0]
00004F1C  3BC8              cmp cx,ax
00004F1E  720E              jc 0x4f2e
00004F20  7707              ja 0x4f29
00004F22  8BD3              mov dx,bx
00004F24  B80400            mov ax,0x4
00004F27  EB1C              jmp short 0x4f45
00004F29  E84BFF            call 0x4e77
00004F2C  EB17              jmp short 0x4f45
00004F2E  E8CAFE            call 0x4dfb
00004F31  EB12              jmp short 0x4f45
00004F33  52                push dx
00004F34  50                push ax
00004F35  9A7E4D0000        call 0x0:0x4d7e
00004F3A  EB07              jmp short 0x4f43
00004F3C  53                push bx
00004F3D  50                push ax
00004F3E  9A8E4C0000        call 0x0:0x4c8e
00004F43  5F                pop di
00004F44  5F                pop di
00004F45  2E8E1E564B        mov ds,[cs:0x4b56]
00004F4A  5F                pop di
00004F4B  5E                pop si
00004F4C  5D                pop bp
00004F4D  CB                retf

; ---- 0000:4F4E ----
00004F4E  56                push si
00004F4F  96                xchg ax,si
00004F50  92                xchg ax,dx
00004F51  85C0              test ax,ax
00004F53  7402              jz 0x4f57
00004F55  F7E3              mul bx
00004F57  E305              jcxz 0x4f5e
00004F59  91                xchg ax,cx
00004F5A  F7E6              mul si
00004F5C  03C1              add ax,cx
00004F5E  96                xchg ax,si
00004F5F  F7E3              mul bx
00004F61  03D6              add dx,si
00004F63  5E                pop si
00004F64  CB                retf

; ---- 0000:4F65 ----
00004F65  51                push cx
00004F66  8AE8              mov ch,al
00004F68  B104              mov cl,0x4
00004F6A  D3E8              shr ax,cl
00004F6C  03D0              add dx,ax
00004F6E  8AC5              mov al,ch
00004F70  8AE3              mov ah,bl
00004F72  D3EB              shr bx,cl
00004F74  59                pop cx
00004F75  03CB              add cx,bx
00004F77  8ADC              mov bl,ah
00004F79  250F00            and ax,0xf
00004F7C  83E30F            and bx,byte +0xf
00004F7F  3BD1              cmp dx,cx
00004F81  7502              jnz 0x4f85
00004F83  3BC3              cmp ax,bx
00004F85  CB                retf
00004F86  59                pop cx
00004F87  0E                push cs
00004F88  51                push cx

; ---- 0000:4F89 ----
00004F89  33C9              xor cx,cx
00004F8B  EB16              jmp short 0x4fa3
00004F8D  59                pop cx
00004F8E  0E                push cs
00004F8F  51                push cx

; ---- 0000:4F90 ----
00004F90  B90100            mov cx,0x1
00004F93  EB0E              jmp short 0x4fa3
00004F95  59                pop cx
00004F96  0E                push cs
00004F97  51                push cx

; ---- 0000:4F98 ----
00004F98  B90200            mov cx,0x2
00004F9B  EB06              jmp short 0x4fa3
00004F9D  59                pop cx
00004F9E  0E                push cs
00004F9F  51                push cx

; ---- 0000:4FA0 ----
00004FA0  B90300            mov cx,0x3
00004FA3  55                push bp
00004FA4  56                push si
00004FA5  57                push di
00004FA6  8BEC              mov bp,sp
00004FA8  8BF9              mov di,cx
00004FAA  8B460A            mov ax,[bp+0xa]
00004FAD  8B560C            mov dx,[bp+0xc]
00004FB0  8B5E0E            mov bx,[bp+0xe]
00004FB3  8B4E10            mov cx,[bp+0x10]
00004FB6  0BC9              or cx,cx
00004FB8  7508              jnz 0x4fc2
00004FBA  0BD2              or dx,dx
00004FBC  7469              jz 0x5027
00004FBE  0BDB              or bx,bx
00004FC0  7465              jz 0x5027
00004FC2  F7C70100          test di,0x1
00004FC6  751C              jnz 0x4fe4
00004FC8  0BD2              or dx,dx
00004FCA  790A              jns 0x4fd6
00004FCC  F7DA              neg dx
00004FCE  F7D8              neg ax
00004FD0  83DA00            sbb dx,byte +0x0
00004FD3  83CF0C            or di,byte +0xc
00004FD6  0BC9              or cx,cx
00004FD8  790A              jns 0x4fe4
00004FDA  F7D9              neg cx
00004FDC  F7DB              neg bx
00004FDE  83D900            sbb cx,byte +0x0
00004FE1  83F704            xor di,byte +0x4
00004FE4  8BE9              mov bp,cx
00004FE6  B92000            mov cx,0x20
00004FE9  57                push di
00004FEA  33FF              xor di,di
00004FEC  33F6              xor si,si
00004FEE  D1E0              shl ax,1
00004FF0  D1D2              rcl dx,1
00004FF2  D1D6              rcl si,1
00004FF4  D1D7              rcl di,1
00004FF6  3BFD              cmp di,bp
00004FF8  720B              jc 0x5005
00004FFA  7704              ja 0x5000
00004FFC  3BF3              cmp si,bx
00004FFE  7205              jc 0x5005
00005000  2BF3              sub si,bx
00005002  1BFD              sbb di,bp
00005004  40                inc ax
00005005  E2E7              loop 0x4fee
00005007  5B                pop bx
00005008  F7C30200          test bx,0x2
0000500C  7406              jz 0x5014
0000500E  8BC6              mov ax,si
00005010  8BD7              mov dx,di
00005012  D1EB              shr bx,1
00005014  F7C30400          test bx,0x4
00005018  7407              jz 0x5021
0000501A  F7DA              neg dx
0000501C  F7D8              neg ax
0000501E  83DA00            sbb dx,byte +0x0
00005021  5F                pop di
00005022  5E                pop si
00005023  5D                pop bp
00005024  CA0800            retf 0x8
00005027  F7F3              div bx
00005029  F7C70200          test di,0x2
0000502D  7401              jz 0x5030
0000502F  92                xchg ax,dx
00005030  33D2              xor dx,dx
00005032  EBED              jmp short 0x5021
00005034  5B                pop bx
00005035  0E                push cs
00005036  53                push bx

; ---- 0000:5037 ----
00005037  80F910            cmp cl,0x10
0000503A  7310              jnc 0x504c
0000503C  8BD8              mov bx,ax
0000503E  D3E0              shl ax,cl
00005040  D3E2              shl dx,cl
00005042  F6D9              neg cl
00005044  80C110            add cl,0x10
00005047  D3EB              shr bx,cl
00005049  0BD3              or dx,bx
0000504B  CB                retf
0000504C  80E910            sub cl,0x10
0000504F  92                xchg ax,dx
00005050  33C0              xor ax,ax
00005052  D3E2              shl dx,cl
00005054  CB                retf
00005055  5B                pop bx
00005056  0E                push cs
00005057  53                push bx

; ---- 0000:5058 ----
00005058  80F910            cmp cl,0x10
0000505B  7310              jnc 0x506d
0000505D  8BDA              mov bx,dx
0000505F  D3E8              shr ax,cl
00005061  D3EA              shr dx,cl
00005063  F6D9              neg cl
00005065  80C110            add cl,0x10
00005068  D3E3              shl bx,cl
0000506A  0BC3              or ax,bx
0000506C  CB                retf
0000506D  80E910            sub cl,0x10
00005070  92                xchg ax,dx
00005071  33D2              xor dx,dx
00005073  D3E8              shr ax,cl
00005075  CB                retf
00005076  07                pop es
00005077  0E                push cs
00005078  06                push es

; ---- 0000:5079 ----
00005079  8EC2              mov es,dx
0000507B  93                xchg ax,bx
0000507C  268B5702          mov dx,[es:bx+0x2]
00005080  53                push bx
00005081  268B1F            mov bx,[es:bx]
00005084  0BC9              or cx,cx
00005086  7D0C              jnl 0x5094
00005088  F7D0              not ax
0000508A  F7D1              not cx
0000508C  050100            add ax,0x1
0000508F  83D100            adc cx,byte +0x0
00005092  EB44              jmp short 0x50d8
00005094  03D8              add bx,ax
00005096  7304              jnc 0x509c
00005098  81C20010          add dx,0x1000
0000509C  8AE1              mov ah,cl
0000509E  B104              mov cl,0x4
000050A0  D2E4              shl ah,cl
000050A2  32C0              xor al,al
000050A4  03D0              add dx,ax
000050A6  8AEB              mov ch,bl
000050A8  D3EB              shr bx,cl
000050AA  03D3              add dx,bx
000050AC  8AC5              mov al,ch
000050AE  250F00            and ax,0xf
000050B1  5B                pop bx
000050B2  268907            mov [es:bx],ax
000050B5  26895702          mov [es:bx+0x2],dx
000050B9  CB                retf
000050BA  07                pop es
000050BB  0E                push cs
000050BC  06                push es
000050BD  8EC2              mov es,dx
000050BF  93                xchg ax,bx
000050C0  268B5702          mov dx,[es:bx+0x2]
000050C4  53                push bx
000050C5  268B1F            mov bx,[es:bx]
000050C8  0BC9              or cx,cx
000050CA  7D0C              jnl 0x50d8
000050CC  F7D0              not ax
000050CE  F7D1              not cx
000050D0  050100            add ax,0x1
000050D3  83D100            adc cx,byte +0x0
000050D6  EBBC              jmp short 0x5094
000050D8  2BD8              sub bx,ax
000050DA  7304              jnc 0x50e0
000050DC  81EA0010          sub dx,0x1000
000050E0  8AE1              mov ah,cl
000050E2  B104              mov cl,0x4
000050E4  D2E4              shl ah,cl
000050E6  32C0              xor al,al
000050E8  2BD0              sub dx,ax
000050EA  8AEB              mov ch,bl
000050EC  D3EB              shr bx,cl
000050EE  03D3              add dx,bx
000050F0  8AC5              mov al,ch
000050F2  250F00            and ax,0xf
000050F5  5B                pop bx
000050F6  268907            mov [es:bx],ax
000050F9  26895702          mov [es:bx+0x2],dx
000050FD  CB                retf
000050FE  07                pop es
000050FF  0E                push cs
00005100  06                push es

; ---- 0000:5101 ----
00005101  0BC9              or cx,cx
00005103  7D0C              jnl 0x5111
00005105  F7D3              not bx
00005107  F7D1              not cx
00005109  83C301            add bx,byte +0x1
0000510C  83D100            adc cx,byte +0x0
0000510F  EB2F              jmp short 0x5140
00005111  03C3              add ax,bx
00005113  7304              jnc 0x5119
00005115  81C20010          add dx,0x1000
00005119  8AE9              mov ch,cl
0000511B  B104              mov cl,0x4
0000511D  D2E5              shl ch,cl
0000511F  02F5              add dh,ch
00005121  8AE8              mov ch,al
00005123  D3E8              shr ax,cl
00005125  03D0              add dx,ax
00005127  8AC5              mov al,ch
00005129  250F00            and ax,0xf
0000512C  CB                retf
0000512D  07                pop es
0000512E  0E                push cs
0000512F  06                push es
00005130  0BC9              or cx,cx
00005132  7D0C              jnl 0x5140
00005134  F7D3              not bx
00005136  F7D1              not cx
00005138  83C301            add bx,byte +0x1
0000513B  83D100            adc cx,byte +0x0
0000513E  EBD1              jmp short 0x5111
00005140  2BC3              sub ax,bx
00005142  7304              jnc 0x5148
00005144  81EA0010          sub dx,0x1000
00005148  8AF9              mov bh,cl
0000514A  B104              mov cl,0x4
0000514C  D2E7              shl bh,cl
0000514E  32DB              xor bl,bl
00005150  2BD3              sub dx,bx
00005152  8AE8              mov ch,al
00005154  D3E8              shr ax,cl
00005156  03D0              add dx,ax
00005158  8AC5              mov al,ch
0000515A  250F00            and ax,0xf
0000515D  CB                retf
0000515E  07                pop es
0000515F  0E                push cs
00005160  06                push es
00005161  57                push di
00005162  8BF9              mov di,cx
00005164  8AEE              mov ch,dh
00005166  B104              mov cl,0x4
00005168  D3E2              shl dx,cl
0000516A  D2ED              shr ch,cl
0000516C  03D0              add dx,ax
0000516E  80D500            adc ch,0x0
00005171  8BC7              mov ax,di
00005173  D3E7              shl di,cl
00005175  D2EC              shr ah,cl
00005177  03DF              add bx,di
00005179  80D400            adc ah,0x0
0000517C  2BD3              sub dx,bx
0000517E  1AEC              sbb ch,ah
00005180  8AC5              mov al,ch
00005182  98                cbw
00005183  92                xchg ax,dx
00005184  5F                pop di
00005185  CB                retf
00005186  07                pop es
00005187  0E                push cs
00005188  06                push es

; ---- 0000:5189 ----
00005189  55                push bp
0000518A  8BEC              mov bp,sp
0000518C  56                push si
0000518D  57                push di
0000518E  1E                push ds
0000518F  C57606            lds si,[bp+0x6]
00005192  C47E0A            les di,[bp+0xa]
00005195  FC                cld
00005196  D1E9              shr cx,1
00005198  F3A5              rep movsw
0000519A  13C9              adc cx,cx
0000519C  F3A4              rep movsb
0000519E  1F                pop ds
0000519F  5F                pop di
000051A0  5E                pop si
000051A1  5D                pop bp
000051A2  CA0800            retf 0x8
000051A5  56                push si
000051A6  96                xchg ax,si
000051A7  92                xchg ax,dx
000051A8  85C0              test ax,ax
000051AA  7402              jz 0x51ae
000051AC  F7E3              mul bx
000051AE  E305              jcxz 0x51b5
000051B0  91                xchg ax,cx
000051B1  F7E6              mul si
000051B3  03C1              add ax,cx
000051B5  96                xchg ax,si
000051B6  F7E3              mul bx
000051B8  03D6              add dx,si
000051BA  5E                pop si
000051BB  C3                ret
000051BC  51                push cx
000051BD  8AE8              mov ch,al
000051BF  B104              mov cl,0x4
000051C1  D3E8              shr ax,cl
000051C3  03D0              add dx,ax
000051C5  8AC5              mov al,ch
000051C7  8AE3              mov ah,bl
000051C9  D3EB              shr bx,cl
000051CB  59                pop cx
000051CC  03CB              add cx,bx
000051CE  8ADC              mov bl,ah
000051D0  250F00            and ax,0xf
000051D3  83E30F            and bx,byte +0xf
000051D6  3BD1              cmp dx,cx
000051D8  7502              jnz 0x51dc
000051DA  3BC3              cmp ax,bx
000051DC  C3                ret
000051DD  FF267A15          jmp [0x157a]
000051E1  0000              add [bx+si],al
000051E3  8F066C15          pop word [0x156c]
000051E7  8F066E15          pop word [0x156e]
000051EB  8F067015          pop word [0x1570]
000051EF  2E8C1EE151        mov [cs:0x51e1],ds
000051F4  89367215          mov [0x1572],si
000051F8  893E7415          mov [0x1574],di
000051FC  FC                cld
000051FD  8E067900          mov es,[0x79]
00005201  BE8000            mov si,0x80
00005204  32E4              xor ah,ah
00005206  26AC              es lodsb
00005208  40                inc ax
00005209  8CC5              mov bp,es
0000520B  87D6              xchg dx,si
0000520D  93                xchg ax,bx
0000520E  8B367300          mov si,[0x73]
00005212  46                inc si
00005213  46                inc si
00005214  B90100            mov cx,0x1
00005217  803E7B0003        cmp byte [0x7b],0x3
0000521C  7211              jc 0x522f
0000521E  8E067500          mov es,[0x75]
00005222  8BFE              mov di,si
00005224  B17F              mov cl,0x7f
00005226  32C0              xor al,al
00005228  F2AE              repne scasb
0000522A  E36E              jcxz 0x529a
0000522C  80F17F            xor cl,0x7f
0000522F  50                push ax
00005230  8BC1              mov ax,cx
00005232  03C3              add ax,bx
00005234  40                inc ax
00005235  25FEFF            and ax,0xfffe
00005238  8BFC              mov di,sp
0000523A  2BF8              sub di,ax
0000523C  725C              jc 0x529a
0000523E  8BE7              mov sp,di
00005240  06                push es
00005241  1F                pop ds
00005242  16                push ss
00005243  07                pop es
00005244  51                push cx
00005245  49                dec cx
00005246  F3A4              rep movsb
00005248  32C0              xor al,al
0000524A  AA                stosb
0000524B  8EDD              mov ds,bp
0000524D  87F2              xchg si,dx
0000524F  87D9              xchg bx,cx
00005251  8BC3              mov ax,bx
00005253  8BD0              mov dx,ax
00005255  43                inc bx
00005256  E81900            call 0x5272
00005259  7707              ja 0x5262
0000525B  7242              jc 0x529f
0000525D  E81200            call 0x5272
00005260  77F9              ja 0x525b
00005262  3C20              cmp al,0x20
00005264  7408              jz 0x526e
00005266  3C0D              cmp al,0xd
00005268  7404              jz 0x526e
0000526A  3C09              cmp al,0x9
0000526C  75E8              jnz 0x5256
0000526E  32C0              xor al,al
00005270  EBE4              jmp short 0x5256
00005272  0BC0              or ax,ax
00005274  7407              jz 0x527d
00005276  42                inc dx
00005277  AA                stosb
00005278  0AC0              or al,al
0000527A  7501              jnz 0x527d
0000527C  43                inc bx
0000527D  86E0              xchg ah,al
0000527F  32C0              xor al,al
00005281  F9                stc
00005282  E315              jcxz 0x5299
00005284  AC                lodsb
00005285  49                dec cx
00005286  2C22              sub al,0x22
00005288  740F              jz 0x5299
0000528A  0422              add al,0x22
0000528C  3C5C              cmp al,0x5c
0000528E  7507              jnz 0x5297
00005290  803C22            cmp byte [si],0x22
00005293  7502              jnz 0x5297
00005295  AC                lodsb
00005296  49                dec cx
00005297  0BF6              or si,si
00005299  C3                ret
0000529A  EAF1010000        jmp 0x0:0x1f1
0000529F  59                pop cx
000052A0  03CA              add cx,dx
000052A2  2E8E1EE151        mov ds,[cs:0x51e1]
000052A7  891E6615          mov [0x1566],bx
000052AB  43                inc bx
000052AC  03DB              add bx,bx
000052AE  03DB              add bx,bx
000052B0  8BF4              mov si,sp
000052B2  8BEC              mov bp,sp
000052B4  2BEB              sub bp,bx
000052B6  72E2              jc 0x529a
000052B8  8BE5              mov sp,bp
000052BA  892E6815          mov [0x1568],bp
000052BE  8C166A15          mov [0x156a],ss
000052C2  E311              jcxz 0x52d5
000052C4  897600            mov [bp+0x0],si
000052C7  8C5602            mov [bp+0x2],ss
000052CA  83C504            add bp,byte +0x4
000052CD  36AC              ss lodsb
000052CF  0AC0              or al,al
000052D1  E0FA              loopne 0x52cd
000052D3  74ED              jz 0x52c2
000052D5  33C0              xor ax,ax
000052D7  894600            mov [bp+0x0],ax
000052DA  894602            mov [bp+0x2],ax
000052DD  2E8E1EE151        mov ds,[cs:0x51e1]
000052E2  8B367215          mov si,[0x1572]
000052E6  8B3E7415          mov di,[0x1574]
000052EA  FF367015          push word [0x1570]
000052EE  FF366E15          push word [0x156e]
000052F2  A16615            mov ax,[0x1566]
000052F5  A36900            mov [0x69],ax
000052F8  A16A15            mov ax,[0x156a]
000052FB  A36D00            mov [0x6d],ax
000052FE  A16815            mov ax,[0x1568]
00005301  A36B00            mov [0x6b],ax
00005304  FF266C15          jmp [0x156c]
00005308  56                push si
00005309  57                push di
0000530A  8E067500          mov es,[0x75]
0000530E  33FF              xor di,di
00005310  06                push es
00005311  FF367700          push word [0x77]
00005315  9A744D0000        call 0x0:0x4d74
0000531A  5B                pop bx
0000531B  8BD8              mov bx,ax
0000531D  07                pop es
0000531E  A37615            mov [0x1576],ax
00005321  89167815          mov [0x1578],dx
00005325  1E                push ds
00005326  8EDA              mov ds,dx
00005328  0BC2              or ax,dx
0000532A  7505              jnz 0x5331
0000532C  EAF1010000        jmp 0x0:0x1f1
00005331  33C0              xor ax,ax
00005333  B9FFFF            mov cx,0xffff
00005336  893F              mov [bx],di
00005338  8C4702            mov [bx+0x2],es
0000533B  83C304            add bx,byte +0x4
0000533E  F2AE              repne scasb
00005340  263805            cmp [es:di],al
00005343  75F1              jnz 0x5336
00005345  8907              mov [bx],ax
00005347  894702            mov [bx+0x2],ax
0000534A  1F                pop ds
0000534B  5F                pop di
0000534C  5E                pop si
0000534D  A17815            mov ax,[0x1578]
