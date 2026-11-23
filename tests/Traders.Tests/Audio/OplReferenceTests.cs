using System.Security.Cryptography;
using System.Text;
using Traders.Audio.Opl;

namespace Traders.Tests.Audio;

/// <summary>
/// Fidelity of the OPL2 port against ymfm itself (lane klang, plan R9 F1 = B, review finding R9K-1). Every scenario is a
/// fixed register sequence; the expected value is the SHA-256 of the native 16-bit little-endian samples that
/// <b>ymfm</b> produces for it, never a value from the port.
///
/// Generator (ymfm commit 81aec25, BSD-3-Clause, https://github.com/aaronsgiles/ymfm; compiled locally, not part of the
/// repository): a C++ program that creates <c>ymfm::ym3812 chip(intf)</c> with a default <c>ymfm::ymfm_interface</c>,
/// calls <c>chip.reset()</c>, and per scenario reads "S name count" and "W pos reg value" lines from stdin. Before sample
/// <c>i</c> it applies every write with <c>pos ≤ i</c> as <c>chip.write(0, reg); chip.write(1, value)</c>, then calls
/// <c>chip.generate(&amp;o, 1)</c> and prints <c>o.data[0]</c> as a decimal. Build:
/// <c>g++ -std=c++14 -Isrc gen.cpp src/ymfm_opl.cpp src/ymfm_adpcm.cpp src/ymfm_pcm.cpp</c>. The input file is written by this
/// class when <c>OPL_REF_INPUT_OUT=&lt;file&gt;</c> is set (run any test of it); the hash of a scenario is the SHA-256 of
/// its printed samples as little-endian <c>int16</c>. Mutating the port (modulator delay, noise, bass drum ×2, attack
/// curve, KSL, sustain level 15, DAC rounding, PM/AM depth, feedback shift …) turns these tests red.
/// </summary>
public class OplReferenceTests
{
    private readonly record struct Write(int Pos, int Reg, int Value);

    private sealed record Scenario(string Name, int Count, List<Write> Writes);

    private static readonly List<Scenario> All = Build();

    static OplReferenceTests()
    {
        string? path = Environment.GetEnvironmentVariable("OPL_REF_INPUT_OUT");
        if (string.IsNullOrEmpty(path)) return;
        var sb = new StringBuilder();
        foreach (var s in All)
        {
            sb.Append($"S {s.Name} {s.Count}\n");
            foreach (var w in s.Writes) sb.Append($"W {w.Pos} {w.Reg} {w.Value}\n");
        }
        File.WriteAllText(path, sb.ToString());
    }

    public static IEnumerable<object[]> Names() => All.Select(s => new object[] { s.Name });

    [Theory]
    [MemberData(nameof(Names))]
    public void Samples_match_ymfm(string name)
    {
        var s = All.Single(x => x.Name == name);
        var chip = new Opl2();
        var bytes = new byte[s.Count * 2];
        int next = 0;
        for (int i = 0; i < s.Count; i++)
        {
            while (next < s.Writes.Count && s.Writes[next].Pos <= i)
            {
                chip.WriteRegister(s.Writes[next].Reg, (byte)s.Writes[next].Value);
                next++;
            }
            short v = chip.GenerateSample();
            bytes[2 * i] = (byte)v;
            bytes[2 * i + 1] = (byte)(v >> 8);
        }
        string hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        Assert.Equal(Expected[name], hash);
    }

    // ---- scenarios -------------------------------------------------------------------------------------------

    private static List<Write> Setup(params (int Reg, int Value)[] regs) => regs.Select(r => new Write(0, r.Reg, r.Value)).ToList();

    /// <summary>Channel 0: modulator (offset 0x00) and carrier (offset 0x03), a note around 440 Hz.</summary>
    private static List<Write> Channel0(int c0, int block = 4, int fnum = 0x241, int modTl = 0x14, int carTl = 0,
        int mod20 = 0x21, int car20 = 0x21, int mod60 = 0xf2, int car60 = 0xf1, int mod80 = 0x74, int car80 = 0x53)
    {
        var w = Setup((0x20, mod20), (0x23, car20), (0x40, modTl), (0x43, carTl), (0x60, mod60), (0x63, car60),
            (0x80, mod80), (0x83, car80), (0xc0, c0), (0xa0, fnum & 0xff));
        w.Add(new Write(0, 0xb0, 0x20 | (block << 2) | (fnum >> 8)));
        return w;
    }

    /// <summary>Rhythm mode: channels 6–8 (operators 12–17 at offsets 0x10–0x15) tuned and ready, rhythm bits in BDh.</summary>
    private static List<Write> Rhythm(int bd)
    {
        var w = Setup((0x01, 0x20), (0xa6, 0x50), (0xb6, 0x09), (0xa7, 0xc0), (0xb7, 0x09), (0xa8, 0x80), (0xb8, 0x09),
            (0xc6, 0x06), (0xc7, 0x00), (0xc8, 0x00));
        int[] ops = { 0x10, 0x13, 0x11, 0x14, 0x12, 0x15 };
        int n = 0;
        foreach (int o in ops)
        {
            w.Add(new Write(0, 0x20 + o, 0x21 + (n & 1)));
            w.Add(new Write(0, 0x40 + o, 0x08 + 2 * n));
            w.Add(new Write(0, 0x60 + o, 0xf3 - n));
            w.Add(new Write(0, 0x80 + o, 0x56 + n));
            w.Add(new Write(0, 0xe0 + o, n % 3));
            n++;
        }
        w.Add(new Write(0, 0xbd, 0x20 | bd));
        return w;
    }

    private static List<Scenario> Build()
    {
        var list = new List<Scenario>();
        void Add(string name, int count, List<Write> writes) => list.Add(new Scenario(name, count, writes));

        // FM and additive connection with feedback; the second case also releases the note
        Add("fm_feedback5", 6000, Channel0(0x0a));
        Add("fm_feedback7", 6000, Channel0(0x0e, block: 3, fnum: 0x1a3));
        Add("additive_feedback3", 6000, Channel0(0x07));
        Add("additive_feedback7", 6000, Channel0(0x0f, block: 5, fnum: 0x2c0));
        var rel = Channel0(0x05, car80: 0x37);
        rel.Add(new Write(3000, 0xb0, (4 << 2) | 2));
        Add("fm_release", 12000, rel);

        // rhythm: each instrument alone (bass drum, snare, tom, cymbal, hi-hat) and all together
        Add("rhythm_bass_drum", 4000, Rhythm(0x10));
        Add("rhythm_snare", 4000, Rhythm(0x08));
        Add("rhythm_tom", 4000, Rhythm(0x04));
        Add("rhythm_cymbal", 4000, Rhythm(0x02));
        Add("rhythm_hihat", 4000, Rhythm(0x01));
        Add("rhythm_all", 6000, Rhythm(0x1f));

        // LFO: both operators with AM and PM, the four depth combinations over more than one AM period (13 440 samples)
        foreach (int depth in new[] { 0x00, 0x40, 0x80, 0xc0 })
        {
            var w = Channel0(0x01, block: 4, fnum: 0x3a0, modTl: 0x10, carTl: 0x10, mod20: 0xe1, car20: 0xe1,
                mod60: 0xf0, car60: 0xf0, mod80: 0x00, car80: 0x00);
            w.Add(new Write(0, 0xbd, depth));
            Add($"lfo_depth_{depth:x2}", 14000, w);
        }

        // key scale level 1–3 (register bits 7–6) at a high pitch, carrier only (additive, modulator without attack)
        foreach (int ksl in new[] { 1, 2, 3 })
            Add($"ksl_{ksl}", 2500, Channel0(0x01, block: 7, fnum: 0x3f0, modTl: 0x3f, carTl: (ksl << 6) | 0x04,
                mod60: 0x00, car60: 0xf0, car80: 0x00));

        // sustain level 15 (attenuation 93 dB) with a percussive envelope
        Add("sustain_level_15", 20000, Channel0(0x01, modTl: 0x3f, mod60: 0x00, car20: 0x01, car60: 0xf9, car80: 0xf5));

        // waveforms 0–3 (waveform select enabled)
        foreach (int wave in new[] { 0, 1, 2, 3 })
        {
            var w = Channel0(0x01, modTl: 0x3f, mod60: 0x00, car60: 0xf0, car80: 0x00);
            w.Add(new Write(0, 0x01, 0x20));
            w.Add(new Write(0, 0xe3, wave));
            Add($"waveform_{wave}", 2500, w);
        }

        // attack rates 4–60 (AR 1–15, key scale 0), low block so the key code does not add to the rate
        for (int ar = 1; ar <= 15; ar++)
            Add($"attack_rate_{ar * 4}", 40000, Channel0(0x01, block: 0, fnum: 0x100, modTl: 0x3f, carTl: 0x08,
                mod60: 0x00, car20: 0x21, car60: (ar << 4) | 0x00, car80: 0x00));

        return list;
    }

    // SHA-256 of the samples ymfm 81aec25 produces for the scenario (see the class comment)
    private static readonly Dictionary<string, string> Expected = new()
    {
        ["fm_feedback5"] = "da812758c8929c054b8011823c0efbbd878ef50892964e27f2f9f7d23d1cfe22",
        ["fm_feedback7"] = "46955faa98956a3cd7c4ff4ccd2850dbbda05aacdc4f5ecd6b28bb6891520351",
        ["additive_feedback3"] = "aa4edaf46022a2cf966c26c0dd0c03b8417a5647d68780b7202d1d9ef91bb92c",
        ["additive_feedback7"] = "d672932dee18011ba0ab495b68d673d7f85393d15f820487faa4d34bf5e611c3",
        ["fm_release"] = "6bb30f521263d492b7d4354d792c9342a2036661b41955b071d643709e534e35",
        ["rhythm_bass_drum"] = "6a1d662933510b8b86b1e898a7ddb72753e20f5dcdbff621354aa662b6094750",
        ["rhythm_snare"] = "a8851c8388033ae007cfa1e305baf5e6a7065b136057a61af2f7e2bf5efa2371",
        ["rhythm_tom"] = "22e1a0a377af6d0b032289ebbe8c59e87abc6e044976fa4bd741a310c8767ba9",
        ["rhythm_cymbal"] = "d63309a48662fd68b812e5e34530446de6826e464d751ead70e265b1a590f2c0",
        ["rhythm_hihat"] = "551aecfad3c5a99e7b372e07c2971dc4b1575648106b320d54c6a81de63939d4",
        ["rhythm_all"] = "00e33e2271b228cb5f60073445669db141bbd9e6fc3734815f504b3821305f71",
        ["lfo_depth_00"] = "9e8853990b9b952edf3474858b3d9a463ced71bc25d724e4c7d4138f34695ea0",
        ["lfo_depth_40"] = "0a54b42203533d53d6da5c920b3e32db8423109c914b5695f1f650007cc56e4e",
        ["lfo_depth_80"] = "6538b1da077f0a1954dd3b1bad3571377760bc251f7a2e991849d4a1c4d0b311",
        ["lfo_depth_c0"] = "54fb509d99ec668d7c62f7efc8fcde178cf5763865f08a244a241d2e8ee2b412",
        ["ksl_1"] = "a18addd3e73c1ed9ff7e5b328faa61689072217aa9a35e6720facbe0317460aa",
        ["ksl_2"] = "30869ee7efb192af0fd85b89f1ef645de55d8782218a94e1b23cbb1383ed36d1",
        ["ksl_3"] = "9cfce93c1cb74f4fcd87b892fa82e465e7c4232316ca5ac69298005a1dc9f80d",
        ["sustain_level_15"] = "15af1b479dcf17a9b6f5361af38bf4fadd012cf9ef6e1d78ef81d2cdf3d970d9",
        ["waveform_0"] = "08e855a739cdbdc31e5a4b102e668e476da99621ced93d2cbb3b713ca265d776",
        ["waveform_1"] = "e8f49d62689f1e4e9c91778c46489e80ea6d6b50d8534f6a72d4e608b28fb881",
        ["waveform_2"] = "0ea59d95cb5eeb065a18902a466c3d4312170ca693fe6f2820162baea1c24785",
        ["waveform_3"] = "9891b12acc9d937789d751c55b0dc7022620c44d4b6f05149a168b6a43a73a17",
        ["attack_rate_4"] = "1690136a307e5e648e7b7df46007acad67aaaa45c5b04c1d042405f67215a53f",
        ["attack_rate_8"] = "68f801b07fed70700ccc1ab345d18574c2470bf905f06fa4e1d60e55b390c2f4",
        ["attack_rate_12"] = "787cbc08546b924f05dbb7b545c44bdcc8b7827bd3f1fa65e7abb94766500fa7",
        ["attack_rate_16"] = "20c366342a58ba8b5ce7453ff2450e5524fbf6501559664225ab0a12bf7e3a2a",
        ["attack_rate_20"] = "a056e143000c7eedcd148a03e45607b8acd083d7d64f816461e99c5de4c0bae1",
        ["attack_rate_24"] = "e0270f8fad489c2633682641dd283050afdd0840ff55cde446486e88a3f79bcd",
        ["attack_rate_28"] = "ba0f965b1a24e297c6682257d53e32458469c36fbbb5e16928f010a58b92c747",
        ["attack_rate_32"] = "ff81f1257a4886a97ef3c4d7b371af2dab4aadcecede5f7d31b90cfd20a6fefb",
        ["attack_rate_36"] = "2cd735eac438ca59ad4d3759968790001761d59d82289f3c10fe5011898fe4c2",
        ["attack_rate_40"] = "c23cc6cca10862a8da14572d8fedf8a73bdd56d8272a624767401e6a4f8278b1",
        ["attack_rate_44"] = "f96762dd92e6bd1491e3dbb6a104a5539840e711b36cd48d7fe906da8cd520bc",
        ["attack_rate_48"] = "57393a3da6ca858addde20fdbed779b4fc9c6c3fdd44384d8b39dd2002d8a452",
        ["attack_rate_52"] = "7e16cd9a0cbe65c0e79a73c9f2d695bc9bf7214ebc0c38fba10d161e3b688b22",
        ["attack_rate_56"] = "05d9a0f1afccd7e88b91534713dc2d6cfbdeb27e0e5238cfc318fb03c9178cec",
        ["attack_rate_60"] = "881a7fcb80b0babcfdf975715018953fafbdd770b336a6634d40e67d56747b24",
    };
}
