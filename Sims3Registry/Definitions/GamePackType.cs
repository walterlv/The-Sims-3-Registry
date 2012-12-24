namespace m3i
{
    /// <summary>
    /// 表示游戏包类型 (原版或资料片或物品包)
    /// </summary>
    public enum GamePackTypes
    {
        /// <summary>
        /// 原版
        /// </summary>
        Base = 0x00,
        /// <summary>
        /// 资料片
        /// </summary>
        Ep = 0x01,
        /// <summary>
        /// 物品包
        /// </summary>
        Sp = 0x10
    }

    public class GamePackType
    {
        /// <summary>
        /// 获取游戏包类型的字符串表示
        /// </summary>
        /// <param name="type">游戏包类型</param>
        /// <returns>EP, SP, 或者空字符串.</returns>
        public static string GetName(GamePackTypes type)
        {
            switch (type)
            {
                case GamePackTypes.Ep:
                    return "EP";
                case GamePackTypes.Sp:
                    return "SP";
                default:
                    return string.Empty;
            }
        }
    }
}
