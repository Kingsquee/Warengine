
/// <summary>
/// Defines the altitude of a squad, terrain, or terrain property
/// </summary>
public enum Altitude { unassigned = 0, sea = 1, land = 2, air = 3 }

/// <summary>
/// Is a TerrainType or PropertyType Capturable?
/// </summary>
public enum Capturable { True, False };

/// <summary>
/// Used for terrain autowalling
/// </summary>
public enum TerrainCornerType { Flat = 0, Inner = 1, Outer = 2 }