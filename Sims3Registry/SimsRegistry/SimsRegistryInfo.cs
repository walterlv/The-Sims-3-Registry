using System;
using System.IO;
using Microsoft.Win32;

namespace m3i.SimsRegistry
{
    /// <summary>
    /// <para>表示模拟人生3一个游戏包的注册表信息</para>
    /// <para>通常以只读方式创建其对象, 但如果需要修改注册表信息, 请在创建时传入可读参数.</para>
    /// </summary>
    public class SimsRegistryInfo
    {
        #region 基本属性
        /// <summary>
        /// 获取与此注册表信息实例关联的游戏包
        /// </summary>
        public readonly GamePack Pack;
        /// <summary>
        /// 获取此注册表信息所属的游戏包名
        /// </summary>
        public GamePackNames Name
        {
            get { return Pack.Name; }
        }

        private bool _isWritable = false;
        /// <summary>
        /// 获取此注册表信息是否可写
        /// </summary>
        public bool IsWritable
        {
            get { return _isWritable; }
            private set { _isWritable = value; }
        }
        private RegistryReadonlyException ReadonlyException = new RegistryReadonlyException("This RegistryInfo is readonly. Writing is forbidden!");
        private InvalidOperationException EnableDisableAnUninstalledGameException = new InvalidOperationException("Cannot enable or disable a game that has not been installed!");
        #endregion

        #region 安装属性
        private bool? _isInstalled;
        /// <summary>
        /// <para>获取此游戏包是否在注册表上有安装记录 (并不保证记录完整)</para>
        /// <para>如果正在利用此实例进行安装过程, 需要手动设置IsInstalled为true.</para>
        /// <para>但是如果设置其值为false, 将删除掉 (包括备份在内的) 所有的注册表键值!</para>
        /// </summary>
        public bool IsInstalled
        {
            get
            {
                if (_isInstalled == null)
                {
                    if (IsSimsKeyExisted) _isInstalled = true;
                    else _isInstalled = false;
                }
                return _isInstalled == true;
            }
        }
        private string _backupExtension = ".backup";
        /// <summary>
        /// 获取用于禁用注册表项的字符串后缀
        /// </summary>
        public string BackupExtension
        {
            get { return _backupExtension; }
            private set { _backupExtension = value; }
        }
        private bool? _isEnabled;
        /// <summary>
        /// 获取此游戏包是否对游戏启动器来说可用. (判断所在的注册表项是否添加了禁用后缀)
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                if (_isEnabled == null)
                {
                    if (IsSimsKeyExisted && OpenRegistry(SimsKeyPath) != null) _isEnabled = true;
                    else _isEnabled = false;
                }
                return _isEnabled == true;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsInstalled) throw EnableDisableAnUninstalledGameException;
                if (_isEnabled == value) return;
                if (value)
                {
                    CopyKeys(OpenCreateRegistry(SimsBackupKeyPath), OpenCreateRegistry(SimsKeyPath));
                    CopyKeys(OpenCreateRegistry(EaBackupKeyPath), OpenCreateRegistry(EaKeyPath));
                    RegistryKey key = OpenRegistry("Sims" + BackupExtension);
                    key.DeleteSubKeyTree(Pack.RegName);
                    if (key.SubKeyCount == 0)
                    {
                        key = SimsRoot;
                        key.DeleteSubKey("Sims" + BackupExtension);
                    }
                    key = OpenRegistry(@"Electronic Arts\Sims" + BackupExtension);
                    key.DeleteSubKeyTree(Pack.RegName);
                    if (key.SubKeyCount == 0)
                    {
                        key = OpenRegistry(@"Electronic Arts");
                        key.DeleteSubKey("Sims" + BackupExtension);
                    }
                }
                else
                {
                    CopyKeys(OpenCreateRegistry(SimsKeyPath), OpenCreateRegistry(SimsBackupKeyPath));
                    CopyKeys(OpenCreateRegistry(EaKeyPath), OpenCreateRegistry(EaBackupKeyPath));
                    RegistryKey key = OpenRegistry("Sims");
                    key.DeleteSubKeyTree(Pack.RegName);
                    if (key.SubKeyCount == 0)
                    {
                        key = SimsRoot;
                        key.DeleteSubKey("Sims");
                    }
                    key = OpenRegistry(@"Electronic Arts\Sims");
                    key.DeleteSubKeyTree(Pack.RegName);
                    if (key.SubKeyCount == 0)
                    {
                        key = OpenRegistry(@"Electronic Arts");
                        key.DeleteSubKey("Sims");
                    }
                }
            }
        }
        private void CopyKeys(RegistryKey oldKey, RegistryKey newKey)
        {
            string[] names = oldKey.GetValueNames();
            foreach (string name in names)
            {
                newKey.SetValue(name, oldKey.GetValue(name), oldKey.GetValueKind(name));
            }
            names = oldKey.GetSubKeyNames();
            foreach (string name in names)
            {
                newKey.CreateSubKey(name);
                CopyKeys(oldKey.OpenSubKey(name, IsWritable), newKey.OpenSubKey(name, IsWritable));
            }
        }
        private bool? _isBroken;
        /// <summary>
        /// 判断此游戏的注册表项是否被损坏 (仅检查项的完整性, 不对键值进行检查)
        /// </summary>
        public bool IsBroken
        {
            get
            {
                if (_isBroken == null)
                {
                    if (IsSimsKeyExisted && IsEaKeyExisted) _isBroken = false;
                    else if (!IsSimsKeyExisted && !IsEaKeyExisted) _isBroken = false;
                    else _isBroken = true;
                }
                return _isBroken == true;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// <para>创建一个用于获取模拟人生3游戏包注册表信息的一个实例</para>
        /// <para>如果需要修改注册表中的值, 请参见其重载构造方法.</para>
        /// </summary>
        /// <param name="name">游戏包名</param>
        public SimsRegistryInfo(GamePackNames name)
            : this(name, false) { }
        /// <summary>
        /// <para>创建一个用于获取模拟人生3游戏包注册表信息的一个实例</para>
        /// <para>如果需要修改注册表中的值, 请参见其重载构造方法.</para>
        /// </summary>
        /// <param name="index">0代表原版, 1代表World Adventures, 2代表High-End Loft Stuff, 以此类推.</param>
        public SimsRegistryInfo(int index)
            : this(index, false) { }
        /// <summary>
        /// <para>创建一个用于获取模拟人生3游戏包注册表信息的一个实例</para>
        /// <para>如果需要修改注册表中的值, 请参见其重载构造方法.</para>
        /// </summary>
        /// <param name="type">游戏类别</param>
        /// <param name="index">只有原版用0表示, 其它为EP01或SP01中的数字部分. 为了与官方表示方式一致, 此处序号从1开始.</param>
        public SimsRegistryInfo(GamePackTypes type, int index)
            : this(type, index, false) { }
        /// <summary>
        /// <para>创建一个用于获取模拟人生3游戏包注册表信息的一个实例</para>
        /// <para>如果需要修改注册表中的值, 请参见其重载构造方法.</para>
        /// </summary>
        /// <param name="pack">指定一个游戏包</param>
        public SimsRegistryInfo(GamePack pack)
            : this(pack, false) { }
        /// <summary>
        /// 创建一个用于具有指定读写权限的模拟人生3游戏包注册表信息的一个实例
        /// </summary>
        /// <param name="name">游戏包名</param>
        /// <param name="writable">是否以可读写的方式创建</param>
        public SimsRegistryInfo(GamePackNames name, bool writable)
            : this(GamePack.GetGamePackByName(name), writable) { }
        /// <summary>
        /// 创建一个用于具有指定读写权限的模拟人生3游戏包注册表信息的一个实例
        /// </summary>
        /// <param name="index">0代表原版, 1代表World Adventures, 2代表High-End Loft Stuff, 以此类推.</param>
        /// <param name="writable">是否以可读写的方式创建</param>
        public SimsRegistryInfo(int index, bool writable)
            : this(GamePack.GetGamePackByIndex(index), writable) { }
        /// <summary>
        /// 创建一个用于具有指定读写权限的模拟人生3游戏包注册表信息的一个实例
        /// </summary>
        /// <param name="type">游戏类别</param>
        /// <param name="index">只有原版用0表示, 其它为EP01或SP01中的数字部分. 为了与官方表示方式一致, 此处序号从1开始.</param>
        /// <param name="writable">是否以可读写的方式创建</param>
        public SimsRegistryInfo(GamePackTypes type, int index, bool writable)
            : this(GamePack.GetGamePackByType(type, index), writable) { }
        /// <summary>
        /// 创建一个用于具有指定读写权限的模拟人生3游戏包注册表信息的一个实例
        /// </summary>
        /// <param name="pack">指定一个游戏包</param>
        /// <param name="writable">是否以可读写的方式创建</param>
        public SimsRegistryInfo(GamePack pack, bool writable)
        {
            this.Pack = pack;
            this.IsWritable = writable;
        }
        #endregion

        #region 注册表操作
        /// <summary>
        /// 打开SOFTWARE下的一个注册表项
        /// </summary>
        /// <param name="keyPath">项路径, 以\分开.</param>
        private RegistryKey OpenRegistry(string keyPath)
        {
            string[] keys = keyPath.Split('\\');
            RegistryKey temp;
            temp = SimsRoot.OpenSubKey(keys[0], IsWritable);
            if (temp == null) return null;
            for (int i = 1; i < keys.Length; i++)
            {
                temp = temp.OpenSubKey(keys[i], IsWritable);
                if (temp == null) return null;
            }
            return temp;
        }
        /// <summary>
        /// 打开SOFTWARE下的一个注册表项, 如果不存在, 则创建.
        /// </summary>
        /// <param name="keyPath">项路径, 以\分开.</param>
        private RegistryKey OpenCreateRegistry(string keyPath)
        {
            string[] keys = keyPath.Split('\\');
            RegistryKey temp, tempBack;
            tempBack = SimsRoot;
            temp = SimsRoot.OpenSubKey(keys[0], IsWritable);
            if (temp == null) { tempBack.CreateSubKey(keys[0]); temp = tempBack.OpenSubKey(keys[0], IsWritable); }
            tempBack = temp;
            for (int i = 1; i < keys.Length; i++)
            {
                temp = temp.OpenSubKey(keys[i], IsWritable);
                if (temp == null) { tempBack.CreateSubKey(keys[i]); temp = tempBack.OpenSubKey(keys[i], IsWritable); }
                tempBack = temp;
            }
            return temp;
        }
        #region SimsRootKey
        private RegistryKey _simsRoot;
        private RegistryKey SimsRoot
        {
            get
            {
                if (_simsRoot == null)
                {
                    _simsRoot = Registry.LocalMachine;
                    _simsRoot = _simsRoot.OpenSubKey("SOFTWARE", IsWritable);
                }
                return _simsRoot;
            }
        }
        #endregion
        #region SimsKey
        private string SimsKeyPath
        {
            get { return @"Sims\" + Pack.RegName; }
        }
        private string SimsBackupKeyPath
        {
            get { return "Sims" + BackupExtension + "\\" + Pack.RegName; }
        }
        private bool? _isSimsKeyExisted;
        private bool IsSimsKeyExisted
        {
            get
            {
                if (_isSimsKeyExisted == null)
                {
                    if (OpenRegistry(SimsKeyPath) != null) _isSimsKeyExisted = true;
                    else if (OpenRegistry(SimsBackupKeyPath) != null) _isSimsKeyExisted = true;
                    else _isSimsKeyExisted = false;
                }
                return _isSimsKeyExisted == true;
            }
            set { _isSimsKeyExisted = value; }
        }
        private RegistryKey _simsKey;
        private RegistryKey SimsKey
        {
            get
            {
                if (_simsKey == null)
                {
                    _simsKey = OpenRegistry(SimsKeyPath);
                    if (_simsKey == null)
                    {
                        _simsKey = OpenRegistry(SimsBackupKeyPath);
                    }
                    if (_simsKey == null) IsSimsKeyExisted = false;
                    else IsSimsKeyExisted = true;
                }
                return _simsKey;
            }
        }
        #endregion
        #region EaKey
        private string EaKeyPath
        {
            get { return @"Electronic Arts\Sims\" + Pack.RegName; }
        }
        private string EaBackupKeyPath
        {
            get { return @"Electronic Arts\Sims.backup\" + Pack.RegName; }
        }
        private bool? _isEaKeyExisted;
        private bool IsEaKeyExisted
        {
            get
            {
                if (_isEaKeyExisted == null)
                {
                    if (OpenRegistry(EaKeyPath) != null) _isEaKeyExisted = true;
                    else if (OpenRegistry(EaBackupKeyPath) != null) _isEaKeyExisted = true;
                    else _isEaKeyExisted = false;
                }
                return _isEaKeyExisted == true;
            }
            set { _isEaKeyExisted = value; }
        }
        private RegistryKey _eaKey;
        private RegistryKey EaKey
        {
            get
            {
                if (_eaKey == null)
                {
                    _eaKey = OpenRegistry(EaKeyPath);
                    if (_eaKey == null)
                    {
                        _eaKey = OpenRegistry(EaBackupKeyPath);
                    }
                    if (_eaKey == null) IsEaKeyExisted = false;
                    else IsEaKeyExisted = true;
                }
                return _eaKey;
            }
        }
        #endregion
        #region ErgcKey
        private string ErgcKeyPath
        {
            get { return EaKeyPath + @"\ergc"; }
        }
        private bool? _isErgcKeyExisted;
        private bool IsErgcKeyExisted
        {
            get
            {
                if (_isErgcKeyExisted == null)
                {
                    if (EaKey == null) _isErgcKeyExisted = false;
                    else
                    {
                        RegistryKey temp = EaKey.OpenSubKey("ergc", IsWritable);
                        if (temp != null) _isErgcKeyExisted = true;
                        else _isErgcKeyExisted = false;
                    }
                }
                return _isErgcKeyExisted == true;
            }
            set { _isErgcKeyExisted = value; }
        }
        private RegistryKey _ergcKey;
        private RegistryKey ErgcKey
        {
            get
            {
                if (_ergcKey == null)
                {
                    if (EaKey == null) _ergcKey = null;
                    else _ergcKey = EaKey.OpenSubKey("ergc", IsWritable);
                    if (_ergcKey != null) IsErgcKeyExisted = true;
                    else IsErgcKeyExisted = false;
                }
                return _ergcKey;
            }
        }
        #endregion
        /// <summary>
        /// 将对此注册表信息的修改保存到磁盘
        /// </summary>
        /// <exception cref="IsReadonlyException" />
        public void Save()
        {
            if (!IsWritable) throw ReadonlyException;
            if (ErgcKey != null) ErgcKey.Flush();
            if (EaKey != null) EaKey.Flush();
            if (SimsKey != null) SimsKey.Flush();
        }
        /// <summary>
        /// 这将删除掉 (包括备份在内的) 所有的注册表键值!
        /// </summary>
        public void Delete()
        {
            if (!IsWritable) throw ReadonlyException;
            if (IsEnabled)
            {
                OpenCreateRegistry(SimsKeyPath);
                OpenCreateRegistry(EaKeyPath);
                RegistryKey key = OpenRegistry("Sims");
                key.DeleteSubKeyTree(Pack.RegName);
                if (key.SubKeyCount == 0)
                {
                    key = SimsRoot;
                    key.DeleteSubKey("Sims");
                }
                key = OpenRegistry(@"Electronic Arts\Sims");
                key.DeleteSubKeyTree(Pack.RegName);
                if (key.SubKeyCount == 0)
                {
                    key = OpenRegistry(@"Electronic Arts");
                    key.DeleteSubKey("Sims");
                }
            }
            else
            {
                OpenCreateRegistry(SimsBackupKeyPath);
                OpenCreateRegistry(EaBackupKeyPath);
                RegistryKey key = OpenRegistry("Sims" + BackupExtension);
                key.DeleteSubKeyTree(Pack.RegName);
                if (key.SubKeyCount == 0)
                {
                    key = SimsRoot;
                    key.DeleteSubKey("Sims" + BackupExtension);
                }
                key = OpenRegistry(@"Electronic Arts\Sims" + BackupExtension);
                key.DeleteSubKeyTree(Pack.RegName);
                if (key.SubKeyCount == 0)
                {
                    key = OpenRegistry(@"Electronic Arts");
                    key.DeleteSubKey("Sims" + BackupExtension);
                }
            }
            this._isInstalled = false;
            this.Save();
        }
        #endregion

        #region 注册表信息
        private const string ContentIdKey = "ContentID";
        private const string ContryKey = "Country";
        private const string DisplayNameKey = "DisplayName";
        private const string ErgcRegPathKey = "ErgcRegPath";
        private const string ExePathKey = "ExePath";
        private const string InstallDirKey = "Install Dir";
        private const string InstallStartKey = "InstallStart";
        private const string LocaleKey = "Locale";
        private const string ProductIdKey = "ProductID";
        private const string SkuKey = "SKU";
        private const string TelemetryKey = "Telemetry";
        private const string EaDisplayNameKey = "DisplayName";
        private const string EaLocaleKey = "Locale";
        private const string EaErgcKey = "";
        private string _contentId;
        public string ContentId
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_contentId == null) _contentId = SimsKey.GetValue(ContentIdKey) as string;
                return _contentId;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                SimsKey.SetValue(ContentIdKey, value);
                _contentId = value;
            }
        }
        private string _country;
        public string Country
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_country == null) _country = SimsKey.GetValue(ContryKey) as string;
                return _country;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                SimsKey.SetValue(ContryKey, value);
                _country = value;
            }
        }
        private string _displayName;
        public string DisplayName
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_displayName == null) _displayName = SimsKey.GetValue(DisplayNameKey) as string;
                return _displayName;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                SimsKey.SetValue(DisplayNameKey, value);
                _displayName = value;
            }
        }
        private string _ergcRegPath;
        /// <summary>
        /// 获取或设置ErgcRegPath的值. 当设置为null时, 会自动转换定位到Electronic Arts下.
        /// </summary>
        public string ErgcRegPath
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_ergcRegPath == null) _ergcRegPath = SimsKey.GetValue(ErgcRegPathKey) as string;
                return _ergcRegPath;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                if (value == null) SimsKey.SetValue(ErgcRegPathKey, @"Electronic Arts\Sims\" + Pack.RegName + @"\ergc");
                else SimsKey.SetValue(ErgcRegPathKey, value);
                _ergcRegPath = value;
            }
        }
        private string _exePath;
        public string ExePath
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_exePath == null) _exePath = SimsKey.GetValue(ExePathKey) as string;
                return _exePath;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                SimsKey.SetValue(ExePathKey, value);
                _exePath = value;
            }
        }
        private string _installDir;
        public string InstallDir
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_installDir == null) _installDir = SimsKey.GetValue(InstallDirKey) as string;
                return _installDir;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                SimsKey.SetValue(InstallDirKey, value);
                _installDir = value;
            }
        }
        private Int32? _installStart;
        public Int32? InstallStart
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_installStart == null) _installStart = SimsKey.GetValue(InstallStartKey) as Int32?;
                return _installStart;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                SimsKey.SetValue(InstallStartKey, value, RegistryValueKind.DWord);
                _installStart = value;
            }
        }
        private string _locale;
        public string Locale
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_locale == null)
                    _locale = SimsKey.GetValue(LocaleKey) as string;
                return _locale;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                SimsKey.SetValue(LocaleKey, value);
                _locale = value;
            }
        }
        /// <summary>
        /// 这是注册表中实际不存在的一个虚拟键值, 是Locale键值所代表的Locale ID.
        /// </summary>
        public int? LocaleId
        {
            get
            {
                if (Locale == null) return null;
                else return Locales.NameToId(this.Locale);
            }
            set
            {
                this.Locale = Locales.IdToName((int)value);
            }
        }
        private Int32? _productId;
        public Int32? ProductId
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_productId == null) _productId = SimsKey.GetValue(ProductIdKey) as Int32?;
                return _productId;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                SimsKey.SetValue(ProductIdKey, value, RegistryValueKind.DWord);
                _productId = value;
            }
        }
        private Int32? _sku;
        public Int32? Sku
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_sku == null) _sku = SimsKey.GetValue(SkuKey) as Int32?;
                return _sku;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                SimsKey.SetValue(SkuKey, value, RegistryValueKind.DWord);
                _sku = value;
            }
        }
        private Int32? _telemetry;
        public Int32? Telemetry
        {
            get
            {
                if (!IsSimsKeyExisted) return null;
                if (_telemetry == null) _telemetry = SimsKey.GetValue(TelemetryKey) as Int32?;
                return _telemetry;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsSimsKeyExisted) { _simsKey = OpenCreateRegistry(SimsKeyPath); IsSimsKeyExisted = true; }
                SimsKey.SetValue(TelemetryKey, value, RegistryValueKind.DWord);
                _telemetry = value;
            }
        }
        private string _eaDisplayName;
        public string EaDisplayName
        {
            get
            {
                if (!IsEaKeyExisted) return null;
                if (_eaDisplayName == null) _eaDisplayName = EaKey.GetValue(EaDisplayNameKey) as string;
                return _eaDisplayName;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsEaKeyExisted) { _eaKey = OpenCreateRegistry(EaKeyPath); IsEaKeyExisted = true; }
                EaKey.SetValue(EaDisplayNameKey, value);
                _eaDisplayName = value;
            }
        }
        private string _eaLocale;
        public string EaLocale
        {
            get
            {
                if (!IsEaKeyExisted) return null;
                if (_eaLocale == null) _eaLocale = EaKey.GetValue(EaLocaleKey) as string;
                return _eaLocale;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsEaKeyExisted) { _eaKey = OpenCreateRegistry(EaKeyPath); IsEaKeyExisted = true; }
                EaKey.SetValue(EaLocaleKey, value);
                _eaLocale = value;
            }
        }
        private string _eaErgc;
        public string EaErgc
        {
            get
            {
                if (!IsErgcKeyExisted) return null;
                if (_eaErgc == null) _eaErgc = ErgcKey.GetValue(EaErgcKey) as string;
                return _eaErgc;
            }
            set
            {
                if (!IsWritable) throw ReadonlyException;
                if (!IsErgcKeyExisted) { _ergcKey = OpenCreateRegistry(ErgcKeyPath); IsErgcKeyExisted = true; }
                ErgcKey.SetValue(EaErgcKey, value);
                _eaErgc = value;
            }
        }
        /// <summary>
        /// <para>根据必要的注册表信息自动补全所有的注册表键值</para>
        /// <para>此方法在写完之后会置IsInstalled为true, 并立即保存.</para>
        /// </summary>
        /// <param name="installDir">安装路径</param>
        /// <param name="country">国家</param>
        /// <param name="localId">区域码</param>
        /// <param name="sku">SKU</param>
        /// <param name="cdKey">CD-Key</param>
        public void SetInfo(string installDir, string country, int localId, int sku, string cdKey)
        {
            this.DisplayName = this.Pack.DisplayName;
            DirectoryInfo dir = new DirectoryInfo(installDir);
            this.Country = country;
            this.LocaleId = localId;
            this.InstallDir = dir.FullName;
            this.ExePath = dir.FullName + @"\Game\Bin\" + this.Pack.Ts3Name + ".exe";
            this.InstallStart = 0x00000000;
            if (this.Pack.Index == 0) this.ProductId = 0x000003E9;
            else this.ProductId = this.Pack.Index + 1;
            this.Sku = sku;
            this.Telemetry = 0x00000000;
            if (this.Pack.Index != 0)
            {
                this.ContentId = "Sims3_" + this.Pack.PackName + "_SKU1";
                this.ErgcRegPath = null;
                this.EaDisplayName = this.DisplayName;
                this.EaLocale = "en_US";
            }
            this.EaErgc = cdKey;
            this._isInstalled = true;
            this.Save();
        }
        #endregion

        #region 父类方法
        /// <summary>
        /// 比较注册表信息表示的两个游戏包是否是同样的游戏包
        /// </summary>
        /// <param name="obj">与此注册表信息相比较的注册表</param>
        /// <returns>如果相等, 则返回true. 如果不相等或者比较者并不是注册表类型, 返回false.</returns>
        public override bool Equals(object obj)
        {
            try
            {
                if (this.Pack.Index == (obj as SimsRegistryInfo).Pack.Index) return true;
                else return false;
            }
            catch { return false; }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 将此注册表信息表示其显示名称
        /// </summary>
        public override string ToString()
        {
            return this.Pack.DisplayName;
        }
        #endregion
    }
}
