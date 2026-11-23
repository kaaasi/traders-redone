using Traders.Assets;
using Traders.RefEmu.Machine;

namespace Traders.Tests.Audio;

/// <summary>
/// The LZEXE unpacker of <see cref="FxDriverImage"/> against the RefEmu: FXDRIVER.EXE is run resident (its own
/// decompressor unpacks it in emulated memory), and the memory from the load segment on must equal the unpacked image
/// with the relocations applied. Expected values come only from the emulated original, never from the unpacker.
/// </summary>
public class MusicFxDriverImageTests
{
    private static (PcMachine Machine, ushort LoadSegment) Resident()
    {
        var m = new PcMachine(new MachineOptions { HostDirectory = TestGameData.DosDir, RecordSoundDriver = false });
        m.RunResidentProgram("FXDRIVER.EXE");
        ushort vectorSegment = m.Mem.Read16(0, 0x64 * 4 + 2);
        return (m, (ushort)(vectorSegment - FxDriverImage.CodeParagraph));
    }

    private static FxDriverImage Image() => FxDriverImage.Load(TestGameData.Dos("FXDRIVER.EXE"));

    // [DIFFERENTIAL-FX] 427D
    [GameDataFact]
    public void Int64_vector_points_to_the_entry_in_the_driver_code_segment()
    {
        var (m, load) = Resident();
        var image = Image();
        // The vector offset is the entry of the dispatcher; its first bytes are the register saves (push ax … push bp).
        ushort entry = m.Mem.Read16(0, 0x64 * 4);
        Assert.Equal(m.Mem.Read8((ushort)(load + FxDriverImage.CodeParagraph), entry),
            image.ReadByte(image.CodeBase + entry));
        Assert.Equal(0x427D, image.CodeBase + entry);
    }

    [GameDataFact]
    public void Unpacked_code_with_relocations_equals_the_memory_of_the_resident_driver()
    {
        var (m, load) = Resident();
        var image = Image();
        var expected = image.Image.ToArray();
        foreach (var (s, o) in image.Relocations)
        {
            int at = s * 16 + o;
            ushort w = (ushort)(expected[at] | expected[at + 1] << 8);
            w += load;
            (expected[at], expected[at + 1]) = ((byte)w, (byte)(w >> 8));
        }
        var differences = new List<int>();
        for (int i = 0; i < image.DataBase; i++)
            if (m.Mem.Read8((ushort)(load + i / 16), (ushort)(i % 16)) != expected[i]) differences.Add(i);
        // The resident driver's code (segment 0292 up to its data segment) is unchanged; the start-up of the main program
        // writes a few words into its own code segment (12 bytes), all below the driver's code.
        Assert.DoesNotContain(differences, i => i >= image.CodeBase);
        Assert.InRange(differences.Count, 0, 12);
    }

    // [DIFFERENTIAL-FX] 4287
    [GameDataFact]
    public void Data_segment_of_the_driver_is_the_one_it_loads_at_the_int64_entry()
    {
        var (m, load) = Resident();
        var image = Image();
        Assert.Equal((ushort)(load + image.DataParagraph), m.Mem.Read16((ushort)(load + FxDriverImage.CodeParagraph), 0x1967));
    }

    [GameDataFact]
    public void Constant_tables_of_the_driver_data_match_the_resident_driver()
    {
        // The installed driver has already initialised its state (volumes, operator parameters, mode), so only the
        // constant tables the player reads are compared: voice/slot tables and F-numbers (DS:0000-003F), percussion
        // masks, default operator parameters and the slot tables (DS:1531-15ED).
        var (m, load) = Resident();
        var image = Image();
        var differences = new List<int>();
        foreach (var (from, to) in new[] { (0x0000, 0x0040), (0x1531, 0x15EE) })
            for (int i = image.DataBase + from; i < image.DataBase + to; i++)
                if (m.Mem.Read8((ushort)(load + i / 16), (ushort)(i % 16)) != image.ReadByte(i)) differences.Add(i - image.DataBase);
        Assert.Equal(string.Empty, string.Join(" ", differences.Select(d => d.ToString("X4"))));
    }

    // R9M-2: format check by header fields and checksum, no bytes of the driver in the code
    [GameDataFact]
    public void Unpack_rejects_wrong_header_fields_and_wrong_checksum()
    {
        byte[] exe = File.ReadAllBytes(TestGameData.Dos("FXDRIVER.EXE"));
        Assert.Equal(FxDriverImage.Sha256, Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(exe)).ToLowerInvariant());
        var shortFile = exe[..^1];
        Assert.Contains("header fields", Assert.Throws<InvalidDataException>(() => FxDriverImage.Unpack(shortFile)).Message);
        var wrongEntry = (byte[])exe.Clone();
        wrongEntry[0x14] ^= 1;
        Assert.Contains("header fields", Assert.Throws<InvalidDataException>(() => FxDriverImage.Unpack(wrongEntry)).Message);
        var otherData = (byte[])exe.Clone();
        otherData[exe.Length - 1] ^= 1; // header intact, content changed
        Assert.Contains("checksum", Assert.Throws<InvalidDataException>(() => FxDriverImage.Unpack(otherData)).Message);
    }
}
