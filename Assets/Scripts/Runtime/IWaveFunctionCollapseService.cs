namespace WFC
{
    public interface IWaveFunctionCollapseService
    {
        public void AddCell(GridCell cell);

        public void Generate();

        public bool Finished { get; }
    }
}