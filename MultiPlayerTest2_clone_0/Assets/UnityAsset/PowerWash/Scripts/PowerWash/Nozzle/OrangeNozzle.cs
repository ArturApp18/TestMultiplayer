namespace PowerWash.Nozzle
{
    public class OrangeNozzle : Nozzle
    {
        public override void Spray()
        {
            TryCleaningDirt();
        }
    }
}