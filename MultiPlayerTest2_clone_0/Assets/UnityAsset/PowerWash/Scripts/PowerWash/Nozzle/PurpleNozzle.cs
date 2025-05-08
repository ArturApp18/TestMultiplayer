namespace PowerWash.Nozzle
{
    public class PurpleNozzle : Nozzle
    {
        public override void Spray()
        {
            TryCleaningDirt();
        }
    }
}