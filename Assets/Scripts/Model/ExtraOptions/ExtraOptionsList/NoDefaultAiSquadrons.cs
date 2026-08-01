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
                    "Only uses custom AI Squadrons instead of including Default AI Squadrons. Custom AI Squadrons can be placed in the following locations:\n\n" +
                    "Windows: %userprofile%\\AppData\\LocalLow\\Baledin\\Fly Casual\\Second Edition\\AiSquadrons\n\n" +
                    "MacOS: ~/Library/Application Support/unity.Baledin.Fly-Casual/Second Edition/AiSquadrons\n\n" +
                    "Linux: ~/.config/unity3d/Baledin/Fly Casual/Second Edition/AiSquadrons\n\n" +
                    "Android: Android\\data\\com.Baledin.FlyCasual\\files\\Second Edition\\AiSquadrons";
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
