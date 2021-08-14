using System;

[Serializable]
public enum Name
{
    PRESIDENT_BLUE,
    DR_GREEN,
    COLONEL_RED,
    AGENT_YELLOW
}

public static class NameExtensions
{
    public static string GetString(this Name name)
    {
        switch (name)
        {
            case Name.PRESIDENT_BLUE:
                return "President Blue";
            case Name.DR_GREEN:
                return "Dr. Green";
            case Name.COLONEL_RED:
                return "Colonel Red";
            case Name.AGENT_YELLOW:
                return "Agent Yellow";
            default:
                return "error";
        }
    }
}