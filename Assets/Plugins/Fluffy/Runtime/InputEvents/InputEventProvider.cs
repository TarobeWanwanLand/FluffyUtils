namespace Fluffy.InputEvents
{
    public static class InputEvent
    {
        private static Pointer _pointer;
        public static Pointer Pointer => _pointer ??= new();
    }
}