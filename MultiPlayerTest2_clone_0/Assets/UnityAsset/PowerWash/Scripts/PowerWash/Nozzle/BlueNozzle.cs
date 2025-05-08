namespace PowerWash.Nozzle
{
    public class BlueNozzle : Nozzle
    {
        public override void Spray()
        {
            TryCleaningDirt();
        }
    }
}