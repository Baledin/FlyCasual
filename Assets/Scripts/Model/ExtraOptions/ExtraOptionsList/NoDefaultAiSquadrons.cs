namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class NoDefaultAiSquadronsExtraOption : ExtraOption
        {
            public NoDefaultAiSquadronsExtraOption()
            {
                Name = "No Default Squadrons for the AI";
                Description =
                    "If this option is ON, the app only uses custom AI Squadrons instead of including Default AI Squadrons. Custom AI Squadrons can be placed in the following location:\n" +
                    "(Folders path (Windows): %userprofile%\\AppData\\LocalLow\\Baledin\\Fly Casual\\Second Edition\\AiSquadrons)\n" +
                    "(Folders path (MacOS): ~/Library/Application Support/unity.Baledin.Fly-Casual/Second Edition/AiSquadrons)";
            }

            protected override void Activate()
            {
                DebugManager.NoDefaultAiSquads = true;
            }

            protected override void Deactivate()
            {
                DebugManager.NoDefaultAiSquads = false;
            }
        }
    }
}
