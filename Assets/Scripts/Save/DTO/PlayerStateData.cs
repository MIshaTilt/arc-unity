namespace Scripts.Save.DTO
{
    /// <summary>
    /// Данные сохранения состояния игрока (HP, MP, кулдауны).
    /// </summary>
    [System.Serializable]
    public class PlayerStateData
    {
        public float currentHealth;
        public float maxHealth;
        public float magicCooldownTimer;
        public float physicalCooldownTimer;
        public bool isDead;
    }
}
