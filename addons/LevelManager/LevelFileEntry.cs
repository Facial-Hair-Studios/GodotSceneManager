using Godot;
using System;
using System.Text.Json.Serialization;

public class LevelFileEntry
{
    public Guid Guid { get; set; } // How I refer to the data
    public long Uid { get; set; }  // How Godot refers to it
    [JsonIgnore] public PackedScene File; // The uninstanced packed level
}
