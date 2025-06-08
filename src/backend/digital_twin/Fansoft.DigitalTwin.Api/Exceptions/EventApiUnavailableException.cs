namespace Fansoft.DigitalTwin.Api.Exceptions;

public class EventApiUnavailableException(string message, Exception? inner = null) : Exception(message, inner);