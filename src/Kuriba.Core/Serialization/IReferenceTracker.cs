namespace Kuriba.Core.Serialization
{
    internal interface IReferenceTracker
    {
        void TrackObject(object? obj);
    }
}