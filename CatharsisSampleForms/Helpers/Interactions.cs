using System;
using ReactiveUI;

namespace CatharsisSampleForms.Helpers
{
    public static class Interactions
    {
        public static Interaction<Exception, bool> ErrorMessage = new Interaction<Exception, bool>();
    }
}