"""PowerPacker PP20 decruncher (reference implementation for analysis)."""
import sys
def decrunch(data):
    assert data[:4]==b'PP20'
    eff=data[4:8]
    out_len=(data[-4]<<16)|(data[-3]<<8)|data[-2]
    skip=data[-1]
    src=data[8:-4]
    out=bytearray(out_len)
    pos=len(src)  # read backwards in 32-bit big-endian words
    def rd32():
        nonlocal pos
        pos-=4
        return int.from_bytes(src[pos:pos+4],'big')
    bitbuf=rd32(); nbits=32
    # first skip bits
    state={'buf':bitbuf,'n':32}
    def getbits(k):
        v=0
        for _ in range(k):
            if state['n']==0:
                state['buf']=rd32(); state['n']=32
            v=(v<<1)|(state['buf']&1)
            state['buf']>>=1; state['n']-=1
        return v
    getbits(skip)
    o=out_len
    while o>0:
        if getbits(1)==0:
            n=1
            while True:
                x=getbits(2); n+=x
                if x!=3: break
            for _ in range(n):
                o-=1; out[o]=getbits(8)
            if o<=0: break
        x=getbits(2)
        offbits=eff[x]
        n=x+2
        if x==3:
            if getbits(1)==0: offbits=7
            off=getbits(offbits)
            while True:
                y=getbits(3); n+=y
                if y!=7: break
        else:
            off=getbits(offbits)
        for _ in range(n):
            o-=1
            out[o]=out[o+off+1]
    return bytes(out)
if __name__=='__main__':
    d=decrunch(open(sys.argv[1],'rb').read())
    open(sys.argv[2],'wb').write(d); print(len(d))
