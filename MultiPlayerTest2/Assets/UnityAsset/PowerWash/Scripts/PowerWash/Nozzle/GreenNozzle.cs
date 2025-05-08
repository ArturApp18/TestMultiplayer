namespace PowerWash.Nozzle
{
    public class GreenNozzle : Nozzle
    {
        public override void Spray()
        {
            TryCleaningDirt();
        }
    }
}