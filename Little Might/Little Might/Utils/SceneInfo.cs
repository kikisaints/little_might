namespace Little_Might.Utils
{
    internal class SceneInfo
    {
        public static string[] GetSceneMessage(Game1.SCENE scene, string deathCause = "")
        {
            switch (scene)
            {
                case Game1.SCENE.MENU:
                    return new string[] {
                        "LITTLE MIGHT",
                        "press ENTER to start"
                    };

                case Game1.SCENE.DIFFICULTY:
                    return new string[] {
                        "SELECT DIFFICULTY",
                    "1 - Baby's first survival game",
                    "2 - I play games",
                    "3 - Normal",
                    "4 - Challenge me!"
                    };

                case Game1.SCENE.DEATH:
                    return new string[] {
                    "YOU DIED" + deathCause,
                    "Press F to Rage Quit",
                    "Press R to Restart"
                    };

                default:
                    return new string[] { "" };
            }
        }

        public static float[] GetSceneScale(Game1.SCENE scene)
        {
            switch (scene)
            {
                case Game1.SCENE.MENU:
                    return new float[] { 2f, 1f };

                case Game1.SCENE.DIFFICULTY:
                    return new float[] { 2f, 1f, 1f, 1f, 1f };

                case Game1.SCENE.DEATH:
                    return new float[] { 2f, 1f, 1f };

                default:
                    return new float[] { 0f };
            }
        }
    }
}
