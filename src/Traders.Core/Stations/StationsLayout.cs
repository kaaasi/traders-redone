namespace Traders.Core.Stations;

/// <summary>
/// DS offsets of the stations phase 1B03:0009 (docs/routines/1B03_0009_stationen.md, docs/analyse/DATENLAYOUT.md section
/// "Spur stationen"). Initialised tables come from T.EXE, the rest is BSS written by segment 1B03.
/// </summary>
public static class StationsLayout
{
    // ---- initialised tables -------------------------------------------------------------
    /// <summary>word[12] – x of each station on the V-RAKETE screen.</summary>
    public const int StationX = 0x0C84;

    /// <summary>byte[12] – y of each station (unsigned).</summary>
    public const int StationY = 0x0C9C;

    /// <summary>far pointer[12] – name text of each station (0A69:004D in 1B03:0548).</summary>
    public const int StationName = 0x0CA8;

    /// <summary>word[4] – colour of a player's rocket dot and selection mark.</summary>
    public const int RocketColor = 0x0CF8;

    /// <summary>word[4] – text colour of a player.</summary>
    public const int PlayerColor = 0x0D00;

    /// <summary>far pointer[3] – name text of each station kind.</summary>
    public const int KindName = 0x0D08;

    /// <summary>byte[] – per pirate position: the pirates start with chance 1/n per frame (1B03:0A62).</summary>
    public const int PirateChance = 0x0D14;

    /// <summary>word[90] at an odd offset – flight curve of a rocket, index = progress 0…59h (1B03:0CCF).</summary>
    public const int FlightCurve = 0x0D17;

    /// <summary>byte[12] – column of the station picture in the atlas (1B03:1178).</summary>
    public const int StationPicture = 0x0DCD;

    /// <summary>byte[2] – the two star colours (1B03:1178).</summary>
    public const int StarColor = 0x0DD9;

    /// <summary>byte[] tables of the pirate ship blit 1B03:128D: source x, source y (by argument), destination x, destination y.</summary>
    public const int PirateSourceX = 0x0DDC, PirateSourceY = 0x0DDF, PirateTargetX = 0x0DE1, PirateTargetY = 0x0DE4;

    /// <summary>Short strings in DS: station marker (1B03:0153) and confirmation mark (1B03:02C0).</summary>
    public const int StationMarkText = 0x0E3E, ConfirmMarkText = 0x0E40;

    /// <summary>word – picture number of V-RAKETE (186C:0002 in 1B03:0009).</summary>
    public const int Picture = 0x08D0;

    /// <summary>Palette of the stations screen (21FD:026C).</summary>
    public const int Palette = 0x0CD8;

    /// <summary>words – page indices: drawn page and shown page.</summary>
    public const int DrawPage = 0x082E, ShownPage = 0x0830;

    // Text pointer table entries (DS:37D0 + 4k) used as far pointer tables by 1B03:10EA and the number texts of 1B03:0EA9.
    public const int NewStationsText = 0x3AD0;    // texts 192…194
    public const int OccupiedText = 0x3ADC;       // texts 195…198
    public const int AllLostText = 0x3AEC;        // texts 199…205
    public const int OilBonusText = 0x3B08;       // texts 206…209 (number in 208)
    public const int GemBonusText = 0x3B18;       // texts 210…213 (number in 212)
    public const int CashBonusText = 0x3B28;      // texts 214…224 (number in 223 from its third character)
    public const int LostHeaderText = 0x3B54;     // text 225

    public const int OilNumberText = 0x3B10, GemNumberText = 0x3B20, CashNumberText = 0x3B4C;

    // ---- BSS of segment 1B03 ------------------------------------------------------------
    /// <summary>byte[12] – station state in the selection: 0 free, 1 taken, 2 confirmed.</summary>
    public const int Taken = 0x5428;

    /// <summary>word[4] – station under a player's cursor in the selection.</summary>
    public const int Cursor = 0x4888;

    /// <summary>byte[4] – 1 when the station value of the player's station names another player (1B03:0763).</summary>
    public const int Occupied = 0x546A;

    /// <summary>sbyte – pirate position (index into the tables at DS:0DDC…, DS:0D14), −1 = no pirates this week.</summary>
    public const int PiratePosition = 0x546E;

    /// <summary>sbyte – remaining steps of the palette flash (1B03:0C89).</summary>
    public const int Flash = 0x546F;

    /// <summary>sbyte – state of the pirate ship (1B03:0A62), 0 = waiting.</summary>
    public const int PirateState = 0x5338;

    /// <summary>byte – 1 when the attacked rocket was visible on the start of its explosion (1B03:0BE7).</summary>
    public const int ExplosionVisible = 0x5336;

    /// <summary>byte – animation frame 0…3 of the ship's lights (1B03:12D7).</summary>
    public const int LightFrame = 0x5337;

    /// <summary>word – frame counter of the light animation.</summary>
    public const int LightCounter = 0x53AA;

    /// <summary>byte – the argument of the last 1B03:1345 (1 = ship drawn at its second place, lights stopped).</summary>
    public const int ShipMoved = 0x53B1;

    /// <summary>byte[4] – rockets of a player put into the launch schedule (1B03:0849).</summary>
    public const int Launched = 0x53A6;

    /// <summary>byte[4] – rockets of a player lost to the pirates.</summary>
    public const int Lost = 0x53AD;

    /// <summary>sbyte – rocket loop variable of 1B03:0849 and 1B03:08ED.</summary>
    public const int RocketIndex = 0x53AC;

    /// <summary>sbyte – number of rockets this week (sum of the rocket fields).</summary>
    public const int RocketTotal = 0x53B2;

    /// <summary>byte – 1 when the pirates may attack (set by state 17 of 1B03:0A62).</summary>
    public const int PiratesReady = 0x53B3;

    /// <summary>sbyte – rockets still flying.</summary>
    public const int Flying = 0x53B4;

    /// <summary>sbyte – index of the rocket under attack, −1 = none.</summary>
    public const int Attacked = 0x53B5;

    /// <summary>word[54] – progress of each rocket: negative = waiting to start, 0…59h flying, above 59h arrived.</summary>
    public const int Progress = 0x53B6;

    /// <summary>word[54] – target x of each rocket (random(299) + 10).</summary>
    public const int Target = 0x533A;

    /// <summary>byte[54] – owner of each rocket.</summary>
    public const int Owner = 0x5434;

    /// <summary>byte – frame counter of the explosion (1B03:0BE7).</summary>
    public const int ExplosionFrame = 0x5422;

    /// <summary>words – position of the rocket being moved (1B03:0CCF).</summary>
    public const int RocketX = 0x5424, RocketY = 0x5426;

    /// <summary>Maximum number of rockets the schedule tables hold (DS:53B6 … DS:5422).</summary>
    public const int MaxRockets = 54;

    /// <summary>Object used for the explosion (records like docs/routines/21AF_000D_objekte.md).</summary>
    public const int ExplosionObject = 4;

    /// <summary>Object records of 1Ah bytes at DS:330C: +2 layer, +14h x, +16h y, +18h frame.</summary>
    public const int Objects = 0x330C, ObjectSize = 0x1A;

    public const int ObjectLayer = Objects + 0x02, ObjectX = Objects + 0x14, ObjectY = Objects + 0x16, ObjectFrame = Objects + 0x18;
}
