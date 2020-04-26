namespace ExportAmazonFromEvernote
{
    public class DataModel
    {
        /// <summary>
        /// ファイルパス
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// ファイルに記載されているURL
        /// </summary>
        public string OriginUrl { get; set; }

        /// <summary>
        /// AmazonURL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 書籍名
        /// </summary>
        public string BookTitle { get; set; }

        /// <summary>
        /// 著者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Kindle Unlimitedかどうか
        /// </summary>
        public bool IsKindleUnlimited { get; set; }
    }
}