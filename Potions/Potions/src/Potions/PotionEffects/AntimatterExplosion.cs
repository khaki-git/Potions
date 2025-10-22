using Photon.Pun;
using Potions.APIs;

namespace Potions.PotionEffects;

public class AntimatterExplosion: PotionEffect
{
    public override void Drink(Character character, Item item)
    {
        var gO = PhotonNetwork.InstantiateItemRoom("Dynamite", item.transform.position, item.transform.rotation);
        gO.GetPhotonView().RPC("RPC_Explode", RpcTarget.All);
    }

    public override void Apply(Item item)
    {
    }
}