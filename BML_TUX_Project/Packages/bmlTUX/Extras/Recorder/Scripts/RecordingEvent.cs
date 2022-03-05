namespace bmlTUX.Recorder
{
    public abstract class RecordingEvent {
    
    }



    public class InstantiatedEvent : RecordingEvent { }

    public class DestroyEvent : RecordingEvent { }
}