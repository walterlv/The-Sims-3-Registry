using System;

namespace m3i
{
    public class GamePack
    {
        /// <summary>
        /// 获取此游戏包在所有所有游戏包中的位置 (按发布时间排序, 0代表原版)
        /// </summary>
        public readonly int Index;
        /// <summary>
        /// 获取此游戏包的类型 (是原版, 资料片还是物品包)
        /// </summary>
        public readonly GamePackTypes Type;
        /// <summary>
        /// 获取此游戏包在资料片或物品包中的位置 (按发布时间排序, 只有原版是0, 其它从1开始.)
        /// </summary>
        public readonly int PIndex;
        /// <summary>
        /// 获取此游戏包名称
        /// </summary>
        public readonly GamePackNames Name;
        /// <summary>
        /// 获取此游戏包名称在注册表中的字符串表示
        /// </summary>
        public readonly string RegName;
        /// <summary>
        /// 获取此游戏包的显示名称
        /// </summary>
        public readonly string DisplayName;
        /// <summary>
        /// 获取此游戏包的包名称 (如EP08, SP05等)
        /// </summary>
        public readonly string PackName;
        private const string Ts3Head = "TS3";
        /// <summary>
        /// <para>获取此游戏的可执行文件名称 (不包含扩展名)</para>
        /// <para>比如 TS3EP08</para>
        /// </summary>
        public readonly string Ts3Name;
        private GamePack(int index, GamePackTypes type, int pIndex, GamePackNames name, string game, string display)
        {
            this.Index = index;
            this.Type = type;
            this.PIndex = pIndex;
            this.Name = name;
            this.RegName = game;
            this.DisplayName = display;
            if (type == GamePackTypes.Base) this.PackName = String.Empty;
            else this.PackName = GamePackType.GetName(type) + pIndex.ToString().PadLeft(2, '0');
            this.Ts3Name = Ts3Head + this.PackName;
        }

        /// <summary>
        /// 比较两个游戏包是否是同样的游戏包
        /// </summary>
        /// <param name="obj">与此游戏包相比较的游戏包</param>
        /// <returns>如果相等, 则返回true. 如果不相等或者比较者并不是游戏包类型, 返回false.</returns>
        public override bool Equals(object obj)
        {
            try
            {
                if (this.Index == (obj as GamePack).Index) return true;
                else return false;
            }
            catch { return false; }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 将此游戏包表示成其显示名称
        /// </summary>
        public override string ToString()
        {
            return base.ToString();
        }


        /// <summary>
        /// 获取此M3库中已有的包括原版在内的所有游戏包的数目
        /// </summary>
        public const int Count = 18;
        /// <summary>
        /// 获取此M3库中已有的资料片的数目
        /// </summary>
        public const int EpCount = 9;
        /// <summary>
        /// 获取此M3库中已有的物品包的数目
        /// </summary>
        public const int SpCount = 8;

        /// <summary>
        /// 通过名称查找游戏包
        /// </summary>
        public static GamePack GetGamePackByName(GamePackNames name)
        {
            switch (name)
            {
                case GamePackNames.Base:
                    return Base;
                case GamePackNames.WorldAdventures:
                    return Ep01;
                case GamePackNames.HighEndLoftStuff:
                    return Sp01;
                case GamePackNames.Ambitions:
                    return Ep02;
                case GamePackNames.FastLaneStuff:
                    return Sp02;
                case GamePackNames.LateNight:
                    return Ep03;
                case GamePackNames.OutdoorLivingStuff:
                    return Sp03;
                case GamePackNames.Generations:
                    return Ep04;
                case GamePackNames.TownLifeStuff:
                    return Sp04;
                case GamePackNames.Pets:
                    return Ep05;
                case GamePackNames.MasterSuiteStuff:
                    return Sp05;
                case GamePackNames.Showtime:
                    return Ep06;
                case GamePackNames.KatyPerrySweetTreats:
                    return Sp06;
                case GamePackNames.DieselStuff:
                    return Sp07;
                case GamePackNames.Supernatural:
                    return Ep07;
                case GamePackNames.Seasons:
                    return Ep08;
                case GamePackNames._70_80_90s:
                    return Sp08;
                case GamePackNames.UniversityLife:
                    return Ep09;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 通过序号查找游戏包
        /// </summary>
        /// <param name="index">0代表原版, 1代表World Adventures, 2代表High-End Loft Stuff, 以此类推.</param>
        public static GamePack GetGamePackByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return Base;
                case 1:
                    return Ep01;
                case 2:
                    return Sp01;
                case 3:
                    return Ep02;
                case 4:
                    return Sp02;
                case 5:
                    return Ep03;
                case 6:
                    return Sp03;
                case 7:
                    return Ep04;
                case 8:
                    return Sp04;
                case 9:
                    return Ep05;
                case 10:
                    return Sp05;
                case 11:
                    return Ep06;
                case 12:
                    return Sp06;
                case 13:
                    return Sp07;
                case 14:
                    return Ep07;
                case 15:
                    return Ep08;
                case 16:
                    return Sp08;
                case 17:
                    return Ep09;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 通过类别和序号来查找游戏包
        /// </summary>
        /// <param name="type">类别</param>
        /// <param name="index">只有原版用0表示, 其它为EP01或SP01中的数字部分. 为了与官方表示方式一致, 此处序号从1开始.</param>
        public static GamePack GetGamePackByType(GamePackTypes type, int index)
        {
            switch (type)
            {
                case GamePackTypes.Base:
                    if (index == 0) return Base;
                    else break;
                case GamePackTypes.Ep:
                    if (index == 1) return Ep01;
                    else if (index == 2) return Ep02;
                    else if (index == 3) return Ep03;
                    else if (index == 4) return Ep04;
                    else if (index == 5) return Ep05;
                    else if (index == 6) return Ep06;
                    else if (index == 7) return Ep07;
                    else if (index == 8) return Ep08;
                    else if (index == 9) return Ep09;
                    else break;
                case GamePackTypes.Sp:
                    if (index == 1) return Sp01;
                    else if (index == 2) return Sp02;
                    else if (index == 3) return Sp03;
                    else if (index == 4) return Sp04;
                    else if (index == 5) return Sp05;
                    else if (index == 6) return Sp06;
                    else if (index == 7) return Sp07;
                    else if (index == 8) return Sp08;
                    else break;
                default:
                    break;
            }
            return null;
        }

        private static GamePack _base;
        /// <summary>
        /// The Base Version
        /// </summary>
        public static GamePack Base
        {
            get
            {
                if (_base == null) _base = new GamePack(0, GamePackTypes.Base, 0, GamePackNames.Base, "The Sims 3", "The Sims 3");
                return _base;
            }
        }

        private static GamePack _ep01;
        /// <summary>
        /// The Sims 3 World Adventures
        /// </summary>
        public static GamePack Ep01
        {
            get
            {
                if (_ep01 == null) _ep01 = new GamePack(1, GamePackTypes.Ep, 1, GamePackNames.WorldAdventures, "The Sims 3 World Adventures", "The Sims 3 World Adventures");
                return _ep01;
            }
        }

        private static GamePack _sp01;
        /// <summary>
        /// The Sims 3 High End Loft Stuff
        /// </summary>
        public static GamePack Sp01
        {
            get
            {
                if (_sp01 == null) _sp01 = new GamePack(2, GamePackTypes.Sp, 1, GamePackNames.HighEndLoftStuff, "The Sims 3 High-End Loft Stuff", "The Sims 3 High-End Loft Stuff");
                return _sp01;
            }
        }

        private static GamePack _ep02;
        /// <summary>
        /// The Sims 3 Ambitions
        /// </summary>
        public static GamePack Ep02
        {
            get
            {
                if (_ep02 == null) _ep02 = new GamePack(3, GamePackTypes.Ep, 2, GamePackNames.Ambitions, "The Sims 3 Ambitions", "The Sims 3 Ambitions");
                return _ep02;
            }
        }

        private static GamePack _sp02;
        /// <summary>
        /// The Sims 3 Fast Lane Stuff
        /// </summary>
        public static GamePack Sp02
        {
            get
            {
                if (_sp02 == null) _sp02 = new GamePack(4, GamePackTypes.Sp, 2, GamePackNames.FastLaneStuff, "The Sims 3 Fast Lane Stuff", "The Sims 3 Fast Lane Stuff");
                return _sp02;
            }
        }

        private static GamePack _ep03;
        /// <summary>
        /// The Sims 3 Late Night
        /// </summary>
        public static GamePack Ep03
        {
            get
            {
                if (_ep03 == null) _ep03 = new GamePack(5, GamePackTypes.Ep, 3, GamePackNames.LateNight, "The Sims 3 Late Night", "The Sims 3 Late Night");
                return _ep03;
            }
        }

        private static GamePack _sp03;
        /// <summary>
        /// The Sims 3 Outdoor Living Stuff
        /// </summary>
        public static GamePack Sp03
        {
            get
            {
                if (_sp03 == null) _sp03 = new GamePack(6, GamePackTypes.Sp, 3, GamePackNames.OutdoorLivingStuff, "The Sims 3 Outdoor Living Stuff", "The Sims 3 Outdoor Living Stuff");
                return _sp03;
            }
        }

        private static GamePack _ep04;
        /// <summary>
        /// The Sims 3 Generations
        /// </summary>
        public static GamePack Ep04
        {
            get
            {
                if (_ep04 == null) _ep04 = new GamePack(7, GamePackTypes.Ep, 4, GamePackNames.Generations, "The Sims 3 Generations", "The Sims 3 Generations");
                return _ep04;
            }
        }

        private static GamePack _sp04;
        /// <summary>
        /// The Sims 3 Town Life Stuff
        /// </summary>
        public static GamePack Sp04
        {
            get
            {
                if (_sp04 == null) _sp04 = new GamePack(8, GamePackTypes.Sp, 4, GamePackNames.TownLifeStuff, "The Sims 3 Town Life Stuff", "The Sims 3 Town Life Stuff");
                return _sp04;
            }
        }

        private static GamePack _ep05;
        /// <summary>
        /// The Sims 3 Pets
        /// </summary>
        public static GamePack Ep05
        {
            get
            {
                if (_ep05 == null) _ep05 = new GamePack(9, GamePackTypes.Ep, 5, GamePackNames.Pets, "The Sims 3 Pets", "The Sims 3 Pets");
                return _ep05;
            }
        }

        private static GamePack _sp05;
        /// <summary>
        /// The Sims 3 Master Suite Stuff
        /// </summary>
        public static GamePack Sp05
        {
            get
            {
                if (_sp05 == null) _sp05 = new GamePack(10, GamePackTypes.Sp, 5, GamePackNames.MasterSuiteStuff, "The Sims 3 Master Suite Stuff", "The Sims 3 Master Suite Stuff");
                return _sp05;
            }
        }

        private static GamePack _ep06;
        /// <summary>
        /// The Sims 3 Showtime
        /// </summary>
        public static GamePack Ep06
        {
            get
            {
                if (_ep06 == null) _ep06 = new GamePack(11, GamePackTypes.Ep, 6, GamePackNames.Showtime, "The Sims 3 Showtime", "The Sims 3 Showtime");
                return _ep06;
            }
        }

        private static GamePack _sp06;
        /// <summary>
        /// The Sims 3 Katy Perry's Sweet Treats
        /// </summary>
        public static GamePack Sp06
        {
            get
            {
                if (_sp06 == null) _sp06 = new GamePack(12, GamePackTypes.Sp, 6, GamePackNames.KatyPerrySweetTreats, "The Sims 3 Katy Perry Sweet Treats", "The Sims 3 Katy Perry's Sweet Treats");
                return _sp06;
            }
        }

        private static GamePack _sp07;
        /// <summary>
        /// The Sims 3 Diesel Stuff
        /// </summary>
        public static GamePack Sp07
        {
            get
            {
                if (_sp07 == null) _sp07 = new GamePack(13, GamePackTypes.Sp, 7, GamePackNames.DieselStuff, "The Sims 3 Diesel Stuff", "The Sims 3 Diesel Stuff");
                return _sp07;
            }
        }

        private static GamePack _ep07;
        /// <summary>
        /// The Sims 3 Supernatural
        /// </summary>
        public static GamePack Ep07
        {
            get
            {
                if (_ep07 == null) _ep07 = new GamePack(14, GamePackTypes.Ep, 7, GamePackNames.Supernatural, "The Sims 3 Supernatural", "The Sims 3 Supernatural");
                return _ep07;
            }
        }

        private static GamePack _ep08;
        /// <summary>
        /// The Sims 3 Seasons
        /// </summary>
        public static GamePack Ep08
        {
            get
            {
                if (_ep08 == null) _ep08 = new GamePack(15, GamePackTypes.Ep, 8, GamePackNames.Seasons, "The Sims 3 Seasons", "The Sims 3 Seasons");
                return _ep08;
            }
        }

        private static GamePack _sp08;
        /// <summary>
        /// The Sims 3 70's, 80's 90's
        /// </summary>
        public static GamePack Sp08
        {
            get
            {
                if (_sp08 == null) _sp08 = new GamePack(16, GamePackTypes.Sp, 8, GamePackNames._70_80_90s, "The Sims 3 70s 80s & 90s Stuff", "The Sims 3 70's, 80's & 90's");
                return _sp08;
            }
        }

        private static GamePack _ep09;
        /// <summary>
        /// The Sims 3 Supernatural
        /// </summary>
        public static GamePack Ep09
        {
            get
            {
                if (_ep09 == null) _ep09 = new GamePack(17, GamePackTypes.Ep, 9, GamePackNames.UniversityLife, "The Sims 3 University Life", "The Sims 3 University Life");
                return _ep09;
            }
        }
    }
}
