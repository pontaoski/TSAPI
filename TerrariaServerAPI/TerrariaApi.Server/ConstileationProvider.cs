﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Terraria;

namespace TerrariaApi.Server;

/// <summary>
/// Cons"tile"ation provider is a tile adapter that will function similar to heaptile to reduce memory, but instead of using math to determine offsets, a TileData structure is used instead which already knows the layout.
/// </summary>
internal class ConstileationProvider : ModFramework.ICollection<ITile>
{
	private TileData[] data = null;

	public int Width => Main.maxTilesX + 1;
	public int Height => Main.maxTilesY + 1;

	public ITile this[int x, int y]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			data ??= new TileData[Width * Height];
			return new TileReference(ref data[Main.maxTilesY * x + y]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => new TileReference(ref data[Main.maxTilesY * x + y]).CopyFrom(value);
	}
}

/// <summary>
/// Maps tile data to memory in an organised format
/// </summary>
/// <remarks>When updating this, refer to <see cref="ITile"/> properties, and adjust the Size accordingly. Size is likely not required, but it make it clear how many bytes we expect to consume</remarks>
[StructLayout(LayoutKind.Sequential, Size = 14, Pack = 1)]
internal struct TileData
{
	public ushort type;
	public ushort wall;
	public byte liquid;
	public ushort sTileHeader;
	public byte bTileHeader;
	public byte bTileHeader2;
	public byte bTileHeader3;
	public short frameX;
	public short frameY;
}

/// <summary>
/// This class is intended to be issued back and forth between Terraria, while referring to a preallocated memory mapping of the tile data which avoids duplicating data and storing offsets.
/// </summary>
internal unsafe sealed class TileReference : Tile
{
	public override ushort type
	{
		get => _tile->type;
		set => _tile->type = value;
	}

	public override ushort wall
	{
		get => _tile->wall;
		set => _tile->wall = value;
	}

	public override byte liquid
	{
		get => _tile->liquid;
		set => _tile->liquid = value;
	}

	public override ushort sTileHeader
	{
		get => _tile->sTileHeader;
		set => _tile->sTileHeader = value;
	}

	public override byte bTileHeader
	{
		get => _tile->bTileHeader;
		set => _tile->bTileHeader = value;
	}

	public override byte bTileHeader2
	{
		get => _tile->bTileHeader2;
		set => _tile->bTileHeader2 = value;
	}

	public override byte bTileHeader3
	{
		get => _tile->bTileHeader3;
		set => _tile->bTileHeader3 = value;
	}

	public override short frameX
	{
		get => _tile->frameX;
		set => _tile->frameX = value;
	}

	public override short frameY
	{
		get => _tile->frameY;
		set => _tile->frameY = value;
	}

	public override void Initialise()
	{
		// this is called in ctor. we dont want to run the default code here.
	}

	readonly TileData* _tile;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TileReference(ref TileData tile)
	{
		_tile = (TileData*)Unsafe.AsPointer(ref tile);
	}
}
