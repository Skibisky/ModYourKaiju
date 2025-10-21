using Sentient.MeYouKaiju;
using System.Reflection;
using UnityEngine.VFX;

public class MykAssets
{
    public static VehicleDamageEffects.Args heliDamEfArgs;
    public static DamageAudio.Args heliDamAudio;

    public static PlayerMarkerSlot.Args PlayerMarkerSlotArgs;
    public static FieldInfo playerMarkerSlotArgsField = typeof(PlayerMarkerSlot).GetField("_args", BindingFlags.NonPublic | BindingFlags.Instance);

    public static VisualEffectAsset DefaultVisualEffect;

    public static MiniGun.Args heliGun;

    public static void FixPlayerMarker(PlayerMarkerSlot slot, PlayerMarkerSlot.Args args = null)
    {
        playerMarkerSlotArgsField.SetValue(slot, args ?? MykAssets.PlayerMarkerSlotArgs);
    }

}