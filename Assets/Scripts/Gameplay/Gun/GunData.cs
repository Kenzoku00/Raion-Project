using UnityEngine;

[System.Serializable]
public class GunLevel
{
    public float fireRate = 0.4f;      
    [Min(1)] public int bulletsPerShot = 1;
    public float spreadAngle = 0f;    
}

[CreateAssetMenu(fileName = "GunData", menuName = "Game/Gun Data")]
public class GunData : ScriptableObject
{
    public GunLevel[] levels = new GunLevel[10];

    public int MaxLevel => levels.Length;
}