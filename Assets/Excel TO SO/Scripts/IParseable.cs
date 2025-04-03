namespace Excel_TO_SO.Scripts
{
    // 定义支持协变的接口，T 只能出现在输出位置（如返回值）
    public interface IParseable
    {
        /// <summary>
        /// 用于将一行数据对类进行初始化
        /// </summary>
        /// <param name="line">一行的数据</param>
        /// <returns></returns>
        public void ParseDataAndInit(string[] line);
    }
}