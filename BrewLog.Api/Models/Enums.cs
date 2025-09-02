namespace BrewLog.Api.Models;

/// <summary>
/// Represents the roast level of coffee beans, from light to dark roasting
/// </summary>
public enum RoastLevel
{
    /// <summary>Light roast - bright, acidic, original flavors preserved</summary>
    Light,
    /// <summary>Medium-light roast - balanced acidity with some body</summary>
    MediumLight,
    /// <summary>Medium roast - balanced flavor, aroma, and acidity</summary>
    Medium,
    /// <summary>Medium-dark roast - rich flavor with some oil on surface</summary>
    MediumDark,
    /// <summary>Dark roast - bold, smoky flavor with oils on surface</summary>
    Dark
}

/// <summary>
/// Represents different coffee brewing methods and techniques
/// </summary>
public enum BrewMethod
{
    /// <summary>Espresso - high pressure extraction method producing concentrated coffee</summary>
    Espresso,
    /// <summary>French Press - full immersion brewing method using metal mesh filter</summary>
    FrenchPress,
    /// <summary>Pour Over - manual drip brewing method with controlled water pouring</summary>
    PourOver,
    /// <summary>Drip - automatic drip coffee maker with heated water and paper filter</summary>
    Drip,
    /// <summary>AeroPress - pressure-assisted immersion method with paper filter</summary>
    AeroPress,
    /// <summary>Cold Brew - long extraction with cold water over 12-24 hours</summary>
    ColdBrew
}

/// <summary>
/// Represents different types of coffee brewing equipment and their purposes
/// </summary>
public enum EquipmentType
{
    /// <summary>Espresso Machine - high-pressure brewing equipment for espresso extraction</summary>
    EspressoMachine,
    /// <summary>Grinder - equipment for grinding coffee beans to desired consistency</summary>
    Grinder,
    /// <summary>French Press - immersion brewing device with plunger and metal filter</summary>
    FrenchPress,
    /// <summary>Pour Over Setup - manual brewing equipment including dripper and filters</summary>
    PourOverSetup,
    /// <summary>Drip Machine - automatic coffee maker with heating element and carafe</summary>
    DripMachine,
    /// <summary>AeroPress - portable brewing device using air pressure and paper filters</summary>
    AeroPress
}