using Mi5hmasH.GameLaunchers.Steam.Types;

namespace QualityControl.xUnit;

public sealed class GameLaunchersTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public GameLaunchersTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }

    [Fact]
    public void SteamId_SetSteamIdFromInt64_ShouldReturnExpectedResults()
    {
        // Arrange
        const ulong inputSteamId64 = 76561197960287930;
        var steamId = new SteamId
        {
            AccountId = 0,
            Instance = SteamId.InstanceEnum.Group,
            AccountType = SteamId.AccountTypeEnum.Invalid,
            Universe = SteamId.UniverseEnum.IndividualUnspecified
        };

        // Act
        steamId.Set(inputSteamId64);
        var lowestBit = steamId.GetLowestBit();
        var highest31BitsOfAccountId = steamId.GetHighest31BitsOfAccountId();
        var steamId64 = steamId.GetSteamId64();
        var steam2Id = steamId.GetSteam2Id();
        var steam3Id = steamId.GetSteam3Id();
        var steam3IdWithoutBrackets = steamId.GetSteam3IdWithoutBrackets();
        var steam3Hex = steamId.GetSteam3Hex();
        
        // Assert
        Assert.Equal(0, lowestBit);
        Assert.Equal((uint)11101, highest31BitsOfAccountId);
        Assert.Equal(inputSteamId64, steamId64);
        Assert.Equal((uint)22202, steamId.AccountId);
        Assert.Equal("STEAM_1:0:11101", steam2Id);
        Assert.Equal("[U:1:22202]", steam3Id);
        Assert.Equal("U:1:22202", steam3IdWithoutBrackets);
        Assert.Equal("steam:1100001000056ba", steam3Hex);
    }
    
    [Fact]
    public void SteamId_SetSteamIdFromString_Int64_ShouldReturnExpectedResults()
    {
        // Arrange
        const string inputSteamIdString = "76561197960287930";
        var steamId = new SteamId();

        // Act
        var setResult = steamId.Set(inputSteamIdString);
        var lowestBit = steamId.GetLowestBit();
        var highest31BitsOfAccountId = steamId.GetHighest31BitsOfAccountId();
        var steamId64 = steamId.GetSteamId64();
        var steam2Id = steamId.GetSteam2Id();
        var steam3Id = steamId.GetSteam3Id();
        var steam3IdWithoutBrackets = steamId.GetSteam3IdWithoutBrackets();
        var steam3Hex = steamId.GetSteam3Hex();

        // Assert
        Assert.True(setResult);
        Assert.Equal(0, lowestBit);
        Assert.Equal((uint)11101, highest31BitsOfAccountId);
        Assert.Equal((ulong)76561197960287930, steamId64);
        Assert.Equal((uint)22202, steamId.AccountId);
        Assert.Equal("STEAM_1:0:11101", steam2Id);
        Assert.Equal("[U:1:22202]", steam3Id);
        Assert.Equal("U:1:22202", steam3IdWithoutBrackets);
        Assert.Equal("steam:1100001000056ba", steam3Hex);
    }

    [Fact]
    public void SteamId_SetSteamIdFromString_Uint32MaxValue_ShouldReturnExpectedResults()
    {
        // Arrange
        var inputSteamIdString = uint.MaxValue.ToString();
        var steamId = new SteamId();

        // Act
        var setResult = steamId.Set(inputSteamIdString);
        var lowestBit = steamId.GetLowestBit();
        var highest31BitsOfAccountId = steamId.GetHighest31BitsOfAccountId();
        var steamId64 = steamId.GetSteamId64();
        var steam2Id = steamId.GetSteam2Id();
        var steam3Id = steamId.GetSteam3Id();
        var steam3IdWithoutBrackets = steamId.GetSteam3IdWithoutBrackets();
        var steam3Hex = steamId.GetSteam3Hex();

        // Assert
        Assert.True(setResult);
        Assert.Equal(1, lowestBit);
        Assert.Equal((uint)2147483647, highest31BitsOfAccountId);
        Assert.Equal((ulong)76561202255233023, steamId64);
        Assert.Equal(4294967295, steamId.AccountId);
        Assert.Equal("STEAM_1:1:2147483647", steam2Id);
        Assert.Equal("[U:1:4294967295]", steam3Id);
        Assert.Equal("U:1:4294967295", steam3IdWithoutBrackets);
        Assert.Equal("steam:1100001ffffffff", steam3Hex);
    }

    [Fact]
    public void SteamId_SetSteamIdFromString_Uint32MaxValue_WithPredefinedValues_ShouldReturnExpectedResults()
    {
        // Arrange
        var inputSteamIdString = uint.MaxValue.ToString();
        var steamId = new SteamId
        {
            AccountId = 0,
            Instance = SteamId.InstanceEnum.Individual,
            AccountType = SteamId.AccountTypeEnum.Individual,
            Universe = SteamId.UniverseEnum.Public
        };

        // Act
        var setResult = steamId.Set(inputSteamIdString);
        var lowestBit = steamId.GetLowestBit();
        var highest31BitsOfAccountId = steamId.GetHighest31BitsOfAccountId();
        var steamId64 = steamId.GetSteamId64();
        var steam2Id = steamId.GetSteam2Id();
        var steam3Id = steamId.GetSteam3Id();
        var steam3IdWithoutBrackets = steamId.GetSteam3IdWithoutBrackets();
        var steam3Hex = steamId.GetSteam3Hex();

        // Assert
        Assert.True(setResult);
        Assert.Equal(1, lowestBit);
        Assert.Equal((uint)2147483647, highest31BitsOfAccountId);
        Assert.Equal((ulong)76561202255233023, steamId64);
        Assert.Equal(4294967295, steamId.AccountId);
        Assert.Equal("STEAM_1:1:2147483647", steam2Id);
        Assert.Equal("[U:1:4294967295]", steam3Id);
        Assert.Equal("U:1:4294967295", steam3IdWithoutBrackets);
        Assert.Equal("steam:1100001ffffffff", steam3Hex);
    }

    [Theory]
    [InlineData("STEAM_1:0:11101", "STEAM_1:0:11101", 0, 11101, 22202, "[U:1:22202]", "U:1:22202")]
    [InlineData("stEam_0:1:33303", "STEAM_0:1:33303", 1, 33303, 66607, "[U:0:66607]", "U:0:66607")]
    public void SteamId_SetSteam2IdFromString_ShouldReturnExpectedResults(string inputSteam2IdString, string expectedSteam2Id, byte expectedLowestBit, uint expectedHighest31BitsOfAccountId, uint expectedAccountId, string expectedSteam3Id, string expectedSteam3IdWithoutBrackets)
    {
        // Arrange
        var steamId = new SteamId();

        // Act
        var setResult = steamId.Set(inputSteam2IdString);
        var lowestBit = steamId.GetLowestBit();
        var highest31BitsOfAccountId = steamId.GetHighest31BitsOfAccountId();
        var steam2Id = steamId.GetSteam2Id();
        var steam3Id = steamId.GetSteam3Id();
        var steam3IdWithoutBrackets = steamId.GetSteam3IdWithoutBrackets();

        // Assert
        Assert.True(setResult);
        Assert.Equal(expectedLowestBit, lowestBit);
        Assert.Equal(expectedHighest31BitsOfAccountId, highest31BitsOfAccountId);
        Assert.Equal(expectedAccountId, steamId.AccountId);
        Assert.Equal(expectedSteam2Id, steam2Id);
        Assert.Equal(expectedSteam3Id, steam3Id);
        Assert.Equal(expectedSteam3IdWithoutBrackets, steam3IdWithoutBrackets);
    }

    [Theory]
    [InlineData("[U:1:22202]", "[U:1:22202]", "U:1:22202", 0, 11101, 22202, "STEAM_1:0:11101")]
    [InlineData("[Z:1:22202]", "[:1:22202]", ":1:22202", 0, 11101, 22202, "STEAM_1:0:11101")]
    [InlineData("[:1:22202]", "[:1:22202]", ":1:22202", 0, 11101, 22202, "STEAM_1:0:11101")]
    public void SteamId_SetSteam3IdFromString_ShouldReturnExpectedResults(string inputSteam3IdString, string expectedSteam3Id, string expectedSteam3IdWithoutBrackets, byte expectedLowestBit, uint expectedHighest31BitsOfAccountId, uint expectedAccountId, string expectedSteam2Id)
    {
        // Arrange
        var steamId = new SteamId();

        // Act
        var setResult = steamId.Set(inputSteam3IdString);
        var lowestBit = steamId.GetLowestBit();
        var highest31BitsOfAccountId = steamId.GetHighest31BitsOfAccountId();
        var steam2Id = steamId.GetSteam2Id();
        var steam3Id = steamId.GetSteam3Id();
        var steam3IdWithoutBrackets = steamId.GetSteam3IdWithoutBrackets();

        // Assert
        Assert.True(setResult);
        Assert.Equal(expectedLowestBit, lowestBit);
        Assert.Equal(expectedHighest31BitsOfAccountId, highest31BitsOfAccountId);
        Assert.Equal(expectedAccountId, steamId.AccountId);
        Assert.Equal(expectedSteam2Id, steam2Id);
        Assert.Equal(expectedSteam3Id, steam3Id);
        Assert.Equal(expectedSteam3IdWithoutBrackets, steam3IdWithoutBrackets);
    }

    [Theory]
    [InlineData("steam:1100001000056ba", "steam:1100001000056ba", 76561197960287930, "[U:1:22202]", "U:1:22202", 0, 11101, 22202, "STEAM_1:0:11101")]
    [InlineData("StEaM:1100001000056BA", "steam:1100001000056ba", 76561197960287930, "[U:1:22202]", "U:1:22202", 0, 11101, 22202, "STEAM_1:0:11101")] 
    public void SteamId_SetSteam3HexIdFromString_ShouldReturnExpectedResults(string inputSteam3HexIdString, string expectedSteam3Hex, ulong expectedSteamId64, string expectedSteam3Id, string expectedSteam3IdWithoutBrackets, byte expectedLowestBit, uint expectedHighest31BitsOfAccountId, uint expectedAccountId, string expectedSteam2Id)
    {
        // Arrange
        var steamId = new SteamId();

        // Act
        var setResult = steamId.Set(inputSteam3HexIdString);
        var lowestBit = steamId.GetLowestBit();
        var highest31BitsOfAccountId = steamId.GetHighest31BitsOfAccountId();
        var steamId64 = steamId.GetSteamId64();
        var steam2Id = steamId.GetSteam2Id();
        var steam3Id = steamId.GetSteam3Id();
        var steam3IdWithoutBrackets = steamId.GetSteam3IdWithoutBrackets();
        var steam3Hex = steamId.GetSteam3Hex();

        // Assert
        Assert.True(setResult);
        Assert.Equal(expectedLowestBit, lowestBit);
        Assert.Equal(expectedHighest31BitsOfAccountId, highest31BitsOfAccountId);
        Assert.Equal(expectedAccountId, steamId.AccountId);
        Assert.Equal(expectedSteamId64, steamId64);
        Assert.Equal(expectedSteam2Id, steam2Id);
        Assert.Equal(expectedSteam3Id, steam3Id);
        Assert.Equal(expectedSteam3IdWithoutBrackets, steam3IdWithoutBrackets);
        Assert.Equal(expectedSteam3Hex, steam3Hex);
    }

    [Fact]
    public void SteamId_SetSteam3HexIdFromString_ShouldReturnFalse()
    {
        // Arrange
        var steamId = new SteamId();

        // Act
        var setResult = steamId.Set("steam:1100001000056bz");

        // Assert
        Assert.False(setResult);
    }

    [Fact]
    public void SteamId_Equal_ShouldBeEqual()
    {
        // Arrange
        const ulong inputSteamId64 = 76561197960287930;
        var steamIdA = new SteamId(inputSteamId64);
        var steamIdB = new SteamId(inputSteamId64);
        
        // Assert
        Assert.Equal(steamIdA, steamIdB);
    }

    [Fact]
    public void SteamId_NotEqual_ShouldNotBeEqual()
    {
        // Arrange
        var steamIdA = new SteamId(76561197960287930);
        var steamIdB = new SteamId(76561197960287931);

        // Assert
        Assert.NotEqual(steamIdA, steamIdB);
    }

    [Fact]
    public void AppId_Equal_ShouldBeEqual()
    {
        // Arrange
        var appIdA = new AppId(70, AppId.AppTypeEnum.Game, "Half-Life", "Valve");
        var appIdB = new AppId(70, AppId.AppTypeEnum.Game, "Half-Life", "Valve");

        // Assert
        Assert.Equal(appIdA, appIdB);
    }

    [Fact]
    public void AppId_NotEqual_ShouldNotBeEqual()
    {
        // Arrange
        var appIdA = new AppId(70, AppId.AppTypeEnum.Game, "Half-Life", "Valve");
        var appIdB = new AppId(220, AppId.AppTypeEnum.Game, "Half-Life 2", "Valve");

        // Assert
        Assert.NotEqual(appIdA, appIdB);
    }
}