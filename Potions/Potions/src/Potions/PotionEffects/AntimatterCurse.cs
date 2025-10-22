// unused

using Photon.Pun;
using Potions.APIs;
using UnityEngine;
using Zorro.Core;

namespace Potions.PotionEffects;

public class AntimatterCurse: PotionEffect
{
    private const int MapX = 12;
    private const int MapY = 12;
    private const int TileSize = 10;
    
    public override void Drink(Character character, Item item)
    {
        var maze = new VoidMaze(character.Center, MapX, MapY, TileSize);
        GameObject.Find("Map").SetActive(false);
        character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, maze.StartPosition + Vector3.up * 5, true);
        var handler = Singleton<OrbFogHandler>.Instance;
        handler.sphere.gameObject.SetActive(false);
        // TODO: apply player effects
    }

    public override void Apply(Item item)
    {
    }
}