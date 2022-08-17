namespace ProductReservationSystem.Inventories
{
    internal interface ReactToAddOrUpdateStock
    {
        void Raise(string productId, uint quantity);
    }
}
